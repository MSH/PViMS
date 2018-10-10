using Microsoft.AspNet.Identity;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

using VPS.Common.Repositories;

using PVIMS.Web.Models;
using PVIMS.Core.Entities;
using PVIMS.Core.Models;

namespace PVIMS.Web.Controllers
{
    [Authorize]
    public class UserAdministrationController : BaseController
    {
        private static string CurrentMenuItem = "AdminUser";

        private readonly UserInfoManager userManager;
        private readonly IUnitOfWorkInt unitOfWork;

        public UserAdministrationController(UserInfoManager userManager,
            IUnitOfWorkInt unitOfWork)
        {
            this.userManager = userManager;
            this.unitOfWork = unitOfWork;
        }

        [HttpGet]
        public ActionResult Index()
        {
            ViewBag.MenuItem = CurrentMenuItem;
            ViewBag.PopUpMessage = PreparePopUpMessage();

            var users = unitOfWork.Repository<User>().Queryable().OrderBy(u => u.UserName).ToList().Select(u => new UserListItem
            {
                UserId = u.Id,
                FirstName = u.FirstName,
                LastName = u.LastName,
                Username = u.UserName,
                IsActive = u.Active
            });

            ViewData.Model = users;

            return View();
        }

        [HttpGet]
        public ActionResult AddUser()
        {
            ViewBag.MenuItem = CurrentMenuItem;

            ViewBag.Roles = unitOfWork.Repository<Role>().Queryable().ToList().Select(r => new SelectListItem { Value = r.Id.ToString(), Text = r.Name }).ToList();

            ViewBag.Facilities = unitOfWork.Repository<Facility>().Queryable().OrderBy(f => f.FacilityName).ToList().Select(f => new SelectListItem { Value = f.Id.ToString(), Text = f.FacilityName }).ToList();

            return View();
        }

        [HttpPost]
        public async Task<ActionResult> AddUser(AddUserModel model)
        {
            ViewBag.MenuItem = CurrentMenuItem;

            ArrayList errors = new ArrayList();

            if (ModelState.IsValid)
            {
                // Validation
                if (String.IsNullOrEmpty(model.UserName.Trim())) {
                    errors.Add("User Name is mandatory");
                }
                if (model.SelectedRoles == null) {
                    errors.Add("At least one role must be selected");
                }
                if (model.SelectedFacilities == null) {
                    errors.Add("At least one facility must be selected");
                }
                if (unitOfWork.Repository<User>().Queryable().Any(u => u.UserName == model.UserName)) {
                    errors.Add("A user with the specified username already exists.");
                }

                if (errors.Count == 0)
                {
                    var user = new UserInfo() { FirstName = model.FirstName, LastName = model.LastName, UserName = model.UserName, Email = model.Email, Password = model.Password };
                    IdentityResult result = await userManager.CreateAsync(user, model.Password);
                    if (result.Succeeded)
                    {
                        var roleIdsInt64 = model.SelectedRoles.ConvertAll<Int64>(Convert.ToInt64).ToArray();

                        var roles = unitOfWork.Repository<Role>().Queryable()
                            .Where(r => roleIdsInt64.Contains(r.Id))
                            .Select(rk => rk.Key)
                            .ToArray();

                        IdentityResult roleResult = await userManager.AddToRolesAsync(user.Id, roles);

                        if (roleResult.Succeeded)
                        {
                            var utemp = unitOfWork.Repository<User>().Queryable().SingleOrDefault(u => u.Id == user.Id);
                            var facilityIdsInt64 = model.SelectedFacilities.ConvertAll<Int64>(Convert.ToInt64).ToArray();

                            ArrayList deleteCollection = new ArrayList();
                            foreach (UserFacility uf in utemp.Facilities)
                            {
                                if (!facilityIdsInt64.Contains(uf.Facility.Id))
                                {
                                    // remove
                                    deleteCollection.Add(uf);
                                }
                            }
                            ArrayList addCollection = new ArrayList();
                            foreach (Int64 id in facilityIdsInt64)
                            {
                                if (!utemp.Facilities.Any(uf => uf.Id == id))
                                {
                                    // add
                                    addCollection.Add(id);
                                }
                            }

                            foreach(UserFacility uf in deleteCollection) {
                                utemp.Facilities.Remove(uf);
                            }
                            foreach (Int64 id in addCollection)
                            {
                                var uf = new UserFacility() { Facility = unitOfWork.Repository<Facility>().Queryable().SingleOrDefault(f => f.Id == id), User = utemp };
                                utemp.Facilities.Add(uf);
                            }
                            utemp.Active = true;
                            unitOfWork.Repository<User>().Update(utemp);
                            unitOfWork.Complete();

                            HttpCookie cookie = new HttpCookie("PopUpMessage");
                            cookie.Value = "User record added successfully";
                            Response.Cookies.Add(cookie);

                            return RedirectToAction("Index");
                        }
                        else
                        {
                            AddErrors(roleResult);
                        }
                    }
                    else
                    {
                        AddErrors(result);
                    }
                }
                else // if (errors.Count == 0) 
                {
                    AddErrors(errors);
                }
            }

            ViewBag.Roles = unitOfWork.Repository<Role>().Queryable().ToList().Select(r => new SelectListItem { Value = r.Id.ToString(), Text = r.Name }).ToList();

            ViewBag.Facilities = unitOfWork.Repository<Facility>().Queryable().OrderBy(f => f.FacilityName).ToList().Select(f => new SelectListItem { Value = f.Id.ToString(), Text = f.FacilityName }).ToList();

            return View(model);
        }

        [HttpGet]
        public ActionResult EditUser(long id)
        {
            ViewBag.MenuItem = CurrentMenuItem;

            var user = unitOfWork.Repository<User>().Queryable().SingleOrDefault(u => u.Id == id);
            var selectedRoles = unitOfWork.Repository<UserRole>().Queryable()
                        .Where(ur => ur.User.Id == id)
                        .Select(ur => ur.Role.Id)
                        .ToList();

            var selectedFacilities = unitOfWork.Repository<UserFacility>().Queryable()
                        .Where(uf => uf.User.Id == id)
                        .Select(uf => uf.Facility.Id)
                        .ToList();

            EditUserModel model = new EditUserModel() { Email = user.Email, FirstName = user.FirstName, LastName = user.LastName, UserName = user.UserName, SelectedRoles = selectedRoles, SelectedFacilities = selectedFacilities, Active = user.Active, AllowDatasetDownload = user.AllowDatasetDownload };

            ViewData.Model = model;

            ViewBag.Id = id;

            ViewBag.Roles = unitOfWork.Repository<Role>().Queryable().ToList().Select(r => new SelectListItem { Value = r.Id.ToString(), Text = r.Name }).ToList();

            ViewBag.Facilities = unitOfWork.Repository<Facility>().Queryable().OrderBy(f => f.FacilityName).ToList().Select(f => new SelectListItem { Value = f.Id.ToString(), Text = f.FacilityName }).ToList();

            return View();
        }

        [HttpGet]
        public JsonResult GetUserDetails()
        {
            var user = unitOfWork.Repository<User>().Queryable().SingleOrDefault(u => u.UserName == User.Identity.Name);
            var userRoles = unitOfWork.Repository<UserRole>().Queryable()
                        .Where(ur => ur.User.Id == user.Id)
                        .Select(ur => ur.Role.Name)
                        .OrderBy( x=>x)
                        .ToList();

            var userFacilities = unitOfWork.Repository<UserFacility>().Queryable()
                        .Where(uf => uf.User.Id == user.Id)
                        .Select(uf => uf.Facility.FacilityName)
                        .OrderBy( x=>x)
                        .ToList();

            var userDetails = new UserDetailsDTO
            {
                Facilities = userFacilities,
                Roles = userRoles
            };

            return Json(userDetails,JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public async Task<ActionResult> EditUser(EditUserModel model)
        {
            ViewBag.MenuItem = CurrentMenuItem;

            ArrayList errors = new ArrayList();

            if (ModelState.IsValid)
            {
                // Validation
                if(String.IsNullOrEmpty(model.UserName.Trim())) {
                    errors.Add("User Name is mandatory");
                }
                if (model.SelectedRoles == null) 
                {
                    errors.Add("At least one role must be selected");
                }
                if (model.SelectedFacilities == null) 
                {
                    errors.Add("At least one facility must be selected");
                }
                var user = unitOfWork.Repository<User>().Queryable().SingleOrDefault(u => u.Id == model.Id);
                
                if (errors.Count == 0)
                {
                    user.Email = model.Email;
                    user.FirstName = model.FirstName;
                    user.LastName = model.LastName;
                    user.UserName = model.UserName;
                    user.Active = model.Active;
                    user.AllowDatasetDownload = model.AllowDatasetDownload;

                    unitOfWork.Repository<User>().Update(user);

                    // Roles
                    var roleIdsInt64 = model.SelectedRoles.ConvertAll<Int64>(Convert.ToInt64).ToArray();

                    var roles = unitOfWork.Repository<Role>().Queryable()
                        .Where(r => roleIdsInt64.Contains(r.Id))
                        .Select(rk => rk.Key);

                    // Determine what has been removed
                    ArrayList deleteCollection = new ArrayList();
                    ArrayList addCollection = new ArrayList();
                    foreach(var userRole in unitOfWork.Repository<UserRole>().Queryable().Include("Role").Where(r => r.User.Id == model.Id))
                    {
                        if(!roles.Contains(userRole.Role.Key)) {
                            deleteCollection.Add(userRole);
                        }
                    }
                    // Determine what needs to be added
                    foreach (string role in roles)
                    {
                        UserRole userRole = unitOfWork.Repository<UserRole>().Queryable().SingleOrDefault(ur => ur.User.Id == model.Id && ur.Role.Key == role);
                        if(userRole == null)
                        {
                            var newRole = unitOfWork.Repository<Role>().Queryable().SingleOrDefault(r => r.Key == role);
                            userRole = new UserRole() { Role = newRole, User = user };
                            addCollection.Add(userRole);
                        }
                    }
                    foreach (UserRole userRole in deleteCollection) {
                        unitOfWork.Repository<UserRole>().Delete(userRole);
                    }
                    foreach (UserRole userRole in addCollection) {
                        unitOfWork.Repository<UserRole>().Save(userRole);
                    }

                    // Facilities
                    var facilityIdsInt64 = model.SelectedFacilities.ConvertAll<Int64>(Convert.ToInt64).ToArray();

                    // Determine what has been removed
                    deleteCollection = new ArrayList();
                    addCollection = new ArrayList();
                    foreach (var userFacility in unitOfWork.Repository<UserFacility>().Queryable().Include("Facility").Where(f => f.User.Id == model.Id))
                    {
                        if (!facilityIdsInt64.Contains(userFacility.Facility.Id)) {
                            deleteCollection.Add(userFacility);
                        }
                    }
                    // Determine what needs to be added
                    foreach (int id in facilityIdsInt64)
                    {
                        UserFacility userFacility = unitOfWork.Repository<UserFacility>().Queryable().SingleOrDefault(uf => uf.User.Id == model.Id && uf.Facility.Id == id);
                        if (userFacility == null)
                        {
                            var newFacility = unitOfWork.Repository<Facility>().Queryable().SingleOrDefault(f => f.Id == id);
                            userFacility = new UserFacility() { Facility = newFacility, User = user };
                            addCollection.Add(userFacility);
                        }
                    }
                    foreach (UserFacility userFacility in deleteCollection) {
                        unitOfWork.Repository<UserFacility>().Delete(userFacility);
                    }
                    foreach (UserFacility userFacility in addCollection) {
                        unitOfWork.Repository<UserFacility>().Save(userFacility);
                    }

                    HttpCookie cookie = new HttpCookie("PopUpMessage");
                    cookie.Value = "User record updated successfully";
                    Response.Cookies.Add(cookie);

                    unitOfWork.Complete();
                    
                    return RedirectToAction("Index");
                }
                else 
                {
                    AddErrors(errors);
                }
            }

            ViewBag.Roles = unitOfWork.Repository<Role>().Queryable().ToList().Select(r => new SelectListItem { Value = r.Id.ToString(), Text = r.Name }).ToList();

            ViewBag.Facilities = unitOfWork.Repository<Facility>().Queryable().OrderBy(f => f.FacilityName).ToList().Select(f => new SelectListItem { Value = f.Id.ToString(), Text = f.FacilityName }).ToList();

            return View(model);
            
        }

        [HttpGet]
        public ActionResult DeleteUser(long id)
        {
            ViewBag.MenuItem = CurrentMenuItem;

            var user = unitOfWork.Repository<User>().Queryable().SingleOrDefault(u => u.Id == id);

            DeleteUserModel model = new DeleteUserModel() { Id = user.Id, FirstName = user.FirstName, LastName = user.LastName, UserName = user.UserName, Active = user.Active };

            ViewData.Model = model;

            ViewBag.Id = id;

            return View();
        }

        [HttpPost]
        public ActionResult DeleteUser(DeleteUserModel model)
        {
            ViewBag.MenuItem = CurrentMenuItem;

            ArrayList errors = new ArrayList();

            if (ModelState.IsValid)
            {
                var user = unitOfWork.Repository<User>().Queryable().SingleOrDefault(u => u.Id == model.Id);

                // Remove references
                ArrayList roleDeleteCollection = new ArrayList();
                ArrayList facilityDeleteCollection = new ArrayList();

                foreach (var userRole in unitOfWork.Repository<UserRole>().Queryable().Include("Role").Where(r => r.User.Id == model.Id))
                {
                    roleDeleteCollection.Add(userRole);
                }
                foreach (var userFacility in unitOfWork.Repository<UserFacility>().Queryable().Include("Facility").Where(f => f.User.Id == model.Id))
                {
                    facilityDeleteCollection.Add(userFacility);
                }

                try
                {
                    foreach (UserRole userRole in roleDeleteCollection)
                    {
                        unitOfWork.Repository<UserRole>().Delete(userRole);
                    }
                    foreach (UserFacility userFacility in facilityDeleteCollection)
                    {
                        unitOfWork.Repository<UserFacility>().Delete(userFacility);
                    }

                    unitOfWork.Repository<User>().Delete(user);
                    unitOfWork.Complete();
                }
                catch (DbUpdateException ex)
                {
                    if (ex.HResult == -2146233087)
                    {
                        errors.Add("Unable to delete user. Currently in use.");
                    }
                    else
                    {
                        errors.Add("Unable to delete user. " + ex.Message);
                    }
                }
                if (errors.Count == 0)
                {
                    HttpCookie cookie = new HttpCookie("PopUpMessage");
                    cookie.Value = "User record deleted successfully";
                    Response.Cookies.Add(cookie);
                        
                    return RedirectToAction("Index");
                }
                else
                {
                    AddErrors(errors);
                }
            }
            
            return View(model);
        }

        [HttpGet]
        public ActionResult ResetPassword(long id)
        {
            ViewBag.MenuItem = CurrentMenuItem;

            var user = unitOfWork.Repository<User>().Queryable().SingleOrDefault(u => u.Id == id);

            ResetPasswordViewModel model = new ResetPasswordViewModel() { Id = user.Id, FirstName = user.FirstName, LastName = user.LastName };

            model.ResetComplete = false;
            model.UserName = user.UserName;

            ViewData.Model = model;

            ViewBag.Id = id;

            return View();
        }

        [HttpPost]
        public async Task<ActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            ViewBag.MenuItem = CurrentMenuItem;
            ViewBag.ReturnUrl = "/UserAdministration";

            var user = await userManager.FindByIdAsync(model.Id);
            if (user == null)
            {
                model.UserName = "Unknown";
                ModelState.AddModelError("", "No user found.");
                return View();
            }

            model.UserName = user.UserName;

            if (ModelState.IsValid)
            {
                user.SecurityStamp = Guid.NewGuid().ToString("D");
                
                try
                {
                    String hashedNewPassword = userManager.PasswordHasher.HashPassword(model.Password);
                    var userTemp = unitOfWork.Repository<User>().Queryable().SingleOrDefault(u => u.Id == user.Id);
                    userTemp.PasswordHash = hashedNewPassword;
                    unitOfWork.Repository<User>().Update(userTemp);

                    HttpCookie cookie = new HttpCookie("PopUpMessage");
                    cookie.Value = "Password reset successfully";
                    Response.Cookies.Add(cookie);

                    unitOfWork.Complete();

                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    model.ResetComplete = true;
                    return View(model);
                }
            }

            return View(model);
        }

        [HttpGet]
        public ActionResult AcceptEula(long id)
        {
            ViewBag.MenuItem = CurrentMenuItem;

            var user = unitOfWork.Repository<User>().Queryable().SingleOrDefault(u => u.Id == id);

            AcceptEulaModel model = new AcceptEulaModel() { UserId = user.Id, FirstName = user.FirstName, LastName = user.LastName, UserName = user.UserName };

            return View(model);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult AcceptEula(AcceptEulaModel model)
        {
            if (ModelState.IsValid)
            {
                var user = unitOfWork.Repository<User>().Queryable().SingleOrDefault(u => u.Id == model.UserId);
                user.EulaAcceptanceDate = DateTime.Now;

                unitOfWork.Repository<User>().Update(user);
                unitOfWork.Complete();

                return RedirectToAction("Index", "Home");
            }

            return View(model);
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        private void AddErrors(ArrayList errors)
        {
            foreach (string error in errors)
            {
                ModelState.AddModelError("", error);
            }
        }
    }
}
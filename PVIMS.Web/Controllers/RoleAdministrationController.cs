using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using VPS.Common.Repositories;
using PVIMS.Core.Entities;
using PVIMS.Core.Models;

namespace PVIMS.Web.Controllers
{
    [Authorize]
    public class RoleAdministrationController : BaseController
    {
        private static string CurrentMenuItem = "AdminRole";

        private readonly IUnitOfWorkInt unitOfWork;

        public RoleAdministrationController(IUnitOfWorkInt unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        // GET: RoleAdministration
        [HttpGet]
        public ActionResult Index()
        {
            ViewBag.MenuItem = CurrentMenuItem;

            ViewData.Model = unitOfWork.Repository<Role>().Queryable()
                .ToList()
                .Select(r => new RoleListItem
                {
                    RoleId = r.Id,
                    RoleName = r.Name,
                    RoleKey = r.Key
                });

            return View();
        }

        [HttpGet]
        public ActionResult AddRole()
        {
            ViewBag.MenuItem = CurrentMenuItem;

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddRole(AddRoleModel model)
        {
            ViewBag.MenuItem = CurrentMenuItem;

            if (ModelState.IsValid)
            {
                unitOfWork.Repository<Role>().Save(new Role { Name = model.RoleName, Key = model.RoleKey });

                return RedirectToAction("Index");
            }

            return View(model);
        }
    }
}
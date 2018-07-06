using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using System.Web.Security.AntiXss;

using VPS.Common.Repositories;

using PVIMS.Core.Entities;
using PVIMS.Web.Models;

namespace PVIMS.Web.Controllers
{
    [Authorize]
    public class ContactDetailController : BaseController
    {
        private static string CurrentMenuItem = "AdminContact";

        private readonly IUnitOfWorkInt unitOfWork;

        public ContactDetailController(IUnitOfWorkInt unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        // GET: ContactDetail
        public ActionResult Index()
        {
            ViewBag.MenuItem = CurrentMenuItem;

            return View();
        }

        [HttpGet]
        public ActionResult EditContactDetail(int cId)
        {
            ViewBag.MenuItem = CurrentMenuItem;

            var contact = unitOfWork.Repository<SiteContactDetail>().Queryable().SingleOrDefault(c => c.Id == cId);

            return View(new ContactDetailEditModel { ContactDetailId = contact.Id, City = contact.City, ContactEmail = contact.ContactEmail, ContactFirstName = contact.ContactFirstName, ContactNumber = contact.ContactNumber, ContactSurname = contact.ContactSurname, ContactType = contact.ContactType.ToString(), CountryCode = contact.CountryCode, OrganisationName = contact.OrganisationName, PostCode = contact.PostCode, State = contact.State, StreetAddress = contact.StreetAddress });
        }

        [HttpPost]
        public ActionResult EditContactDetail(ContactDetailEditModel model)
        {
            ViewBag.MenuItem = CurrentMenuItem;

            if (ModelState.IsValid)
            {
                if (!String.IsNullOrEmpty(model.OrganisationName))
                {
                    if (Regex.Matches(model.OrganisationName, @"[a-zA-Z0-9 ]").Count < model.OrganisationName.Length)
                    {
                        ModelState.AddModelError("OrganisationName", "Value contains invalid characters (Enter A-Z, a-z, 0-9, space)");

                        return View(model);
                    }
                }

                if (!String.IsNullOrEmpty(model.ContactFirstName))
                {
                    if (Regex.Matches(model.ContactFirstName, @"[a-zA-Z ]").Count < model.ContactFirstName.Length)
                    {
                        ModelState.AddModelError("ContactFirstName", "Value contains invalid characters (Enter A-Z, a-z, space)");

                        return View(model);
                    }
                }

                if(!String.IsNullOrEmpty(model.ContactSurname))
                {
                    if (Regex.Matches(model.ContactSurname, @"[a-zA-Z ]").Count < model.ContactSurname.Length)
                    {
                        ModelState.AddModelError("ContactSurname", "Value contains invalid characters (Enter A-Z, a-z, space)");

                        return View(model);
                    }
                }

                if(!String.IsNullOrEmpty(model.StreetAddress))
                {
                    if (Regex.Matches(model.StreetAddress, @"[a-zA-Z0-9 ']").Count < model.StreetAddress.Length)
                    {
                        ModelState.AddModelError("StreetAddress", "Value contains invalid characters (Enter A-Z, a-z, 0-9, space, comma)");

                        return View(model);
                    }
                }

                if(!String.IsNullOrEmpty(model.City))
                {
                    if (Regex.Matches(model.City, @"[a-zA-Z ]").Count < model.City.Length)
                    {
                        ModelState.AddModelError("City", "Value contains invalid characters (Enter A-Z, a-z, space)");

                        return View(model);
                    }
                }

                if(!String.IsNullOrEmpty(model.State))
                {
                    if (Regex.Matches(model.State, @"[a-zA-Z ]").Count < model.State.Length)
                    {
                        ModelState.AddModelError("State", "Value contains invalid characters (Enter A-Z, a-z, space)");

                        return View(model);
                    }
                }

                if(!String.IsNullOrEmpty(model.PostCode))
                {
                    if (Regex.Matches(model.PostCode, @"[a-zA-Z0-9]").Count < model.PostCode.Length)
                    {
                        ModelState.AddModelError("PostCode", "Value contains invalid characters (Enter A-Z, a-z, 0-9)");

                        return View(model);
                    }
                }

                if(!String.IsNullOrEmpty(model.ContactNumber))
                {
                    if (Regex.Matches(model.ContactNumber, @"[-0-9]").Count < model.ContactNumber.Length)
                    {
                        ModelState.AddModelError("ContactNumber", "Value contains invalid characters (Enter 0-9, hyphen)");

                        return View(model);
                    }
                }

                if(!String.IsNullOrEmpty(model.ContactEmail))
                {
                    if (Regex.Matches(model.ContactEmail, @"[-a-zA-Z@._]").Count < model.ContactEmail.Length)
                    {
                        ModelState.AddModelError("ContactEmail", "Value contains invalid characters (Enter A-Z, a-z, hyphen, @, period, underscore)");

                        return View(model);
                    }
                }

                if(!String.IsNullOrEmpty(model.CountryCode))
                {
                    if (Regex.Matches(model.CountryCode, @"[0-9]").Count < model.CountryCode.Length)
                    {
                        ModelState.AddModelError("CountryCode", "Value contains invalid characters (Enter 0-9)");

                        return View(model);
                    }
                }

                var contact = unitOfWork.Repository<SiteContactDetail>().Queryable().SingleOrDefault(c => c.Id == model.ContactDetailId);

                if (contact == null)
                {
                    ModelState.AddModelError("ContactType", "Unable to update the contact detail. The record could not be found in the data store.");

                    return View(model);
                }

                try
                {
                    string encodedValue = "";
                    encodedValue = System.Web.Security.AntiXss.AntiXssEncoder.HtmlEncode(model.City, false);;
                    contact.City = encodedValue;
                    encodedValue = System.Web.Security.AntiXss.AntiXssEncoder.HtmlEncode(model.ContactEmail, false);;
                    contact.ContactEmail = encodedValue;
                    encodedValue = System.Web.Security.AntiXss.AntiXssEncoder.HtmlEncode(model.ContactFirstName, false);;
                    contact.ContactFirstName = encodedValue;
                    encodedValue = System.Web.Security.AntiXss.AntiXssEncoder.HtmlEncode(model.ContactNumber, false);;
                    contact.ContactNumber = encodedValue;
                    encodedValue = System.Web.Security.AntiXss.AntiXssEncoder.HtmlEncode(model.ContactSurname, false);;
                    contact.ContactSurname = encodedValue;
                    encodedValue = System.Web.Security.AntiXss.AntiXssEncoder.HtmlEncode(model.CountryCode, false);;
                    contact.CountryCode = encodedValue;
                    encodedValue = System.Web.Security.AntiXss.AntiXssEncoder.HtmlEncode(model.OrganisationName, false); ;
                    contact.OrganisationName = encodedValue;
                    encodedValue = System.Web.Security.AntiXss.AntiXssEncoder.HtmlEncode(model.PostCode, false); ;
                    contact.PostCode = encodedValue;
                    encodedValue = System.Web.Security.AntiXss.AntiXssEncoder.HtmlEncode(model.State, false);;
                    contact.State = encodedValue;
                    encodedValue = System.Web.Security.AntiXss.AntiXssEncoder.HtmlEncode(model.StreetAddress, false);;
                    contact.StreetAddress = encodedValue;

                    unitOfWork.Repository<SiteContactDetail>().Update(contact);
                    unitOfWork.Complete();

                    HttpCookie cookie = new HttpCookie("PopUpMessage");
                    cookie.Value = "Contact detail record updated successfully";
                    Response.Cookies.Add(cookie);

                    return Redirect("/Admin/ManageContactDetail.aspx");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", string.Format("Unable to update the configuration: {0}", ex.Message));
                }
            }

            return View(model);
        }

    }
}
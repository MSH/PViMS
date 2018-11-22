using System;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using PVIMS.Core.Entities;
using PVIMS.Web.Models;
using PVIMS.Web.ActionFilters;

using VPS.Common.Repositories;

namespace PVIMS.Web.Controllers
{
    [Authorize]
    public class AppointmentController : BaseController
    {
        private static string CurrentMenuItem = "PatientView";

        private readonly IUnitOfWorkInt unitOfWork;

        public AppointmentController(IUnitOfWorkInt unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        [HttpGet]
        [MvcUnitOfWork]
        public ActionResult AddAppointment(int pid, string cancelRedirectUrl)
        {
            ViewBag.MenuItem = CurrentMenuItem;

            TempData["cancelRedirectUrl"] = cancelRedirectUrl;

            Patient patient = pid > 0 ? unitOfWork.Repository<Patient>().Get(pid) : null;

            AppointmentAddModel appointmentAddModel = null;

            string patientName = "";
            if (patient != null)
            {
                patientName = patient.FullName;
                appointmentAddModel = new AppointmentAddModel { PatientId = patient.Id, Reason = "", AppointmentDate = DateTime.Today };
            }

            ViewBag.PatientName = patientName;
            ViewBag.CancelRedirectUrl = cancelRedirectUrl;

            return View(appointmentAddModel);
        }

        [HttpPost]
        [MvcUnitOfWork]
        public ActionResult AddAppointment(AppointmentAddModel model)
        {
            ViewBag.MenuItem = CurrentMenuItem;

            var cancelRedirectUrl = (TempData["cancelRedirectUrl"] ?? string.Empty).ToString();

            ViewBag.cancelRedirectUrl = cancelRedirectUrl;

            string patientName = "";

            Patient patient = model.PatientId > 0 ? unitOfWork.Repository<Patient>().Get(model.PatientId) : null;

            var appointment = unitOfWork.Repository<Appointment>()
                .Queryable()
                .Include(p => p.Patient)
                .SingleOrDefault(a => a.Patient.Id == patient.Id && a.AppointmentDate == model.AppointmentDate);

            if(appointment != null)
            {
                ModelState.AddModelError("AppointmentDate", "Patient already has an appointment for this date");
            }

            if (model.AppointmentDate > DateTime.Today.AddYears(2))
            {
                ModelState.AddModelError("AppointmentDate", "Appointment Date should be within 2 years");
            }
            if (model.AppointmentDate < DateTime.Today)
            {
                ModelState.AddModelError("AppointmentDate", "Appointment Date should be after current date");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    patientName = patient.FullName;

                    var currentUser = unitOfWork.Repository<User>().Queryable().SingleOrDefault(u => u.UserName == User.Identity.Name);

                    var newAppointment = new Appointment(patient)
                    {
                        AppointmentDate = model.AppointmentDate,
                        Reason = model.Reason,
                        DNA = false,
                        Cancelled = false
                    };

                    unitOfWork.Repository<Appointment>().Save(newAppointment);

                    HttpCookie cookie = new HttpCookie("PopUpMessage");
                    cookie.Value = "Appointment record added successfully";
                    Response.Cookies.Add(cookie);

                    return Redirect("/Patient/PatientView.aspx?pid=" + newAppointment.Patient.Id.ToString());
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("AppointmentDate", string.Format("Unable to add the appointment: {0}", ex.Message));
                }
            }

            ViewBag.PatientName = patientName;

            return View(model);
        }

        [MvcUnitOfWork]
        public ActionResult EditAppointment(int id, string cancelRedirectUrl)
        {
            ViewBag.MenuItem = CurrentMenuItem;

            TempData["cancelRedirectUrl"] = cancelRedirectUrl;

            var appointment = unitOfWork.Repository<Appointment>()
                .Queryable()
                .Include(p => p.Patient)
                .SingleOrDefault(e => e.Id == id);

            if (appointment == null)
            {
                ViewBag.Entity = "Appointment";
                return View("NotFound");
            }

            string patientName = appointment.Patient.FullName;

            var appointmentEditModel = new AppointmentEditModel
            {
                AppointmentId = appointment.Id,
                PatientFullName = appointment.Patient.FullName,
                AppointmentDate = appointment.AppointmentDate,
                Reason = appointment.Reason,
                Cancelled = appointment.Cancelled ? "Yes" : "No",
                CancellationReason = appointment.CancellationReason,
                Created = appointment.GetCreatedStamp(),
                Updated = appointment.GetLastUpdatedStamp()
            };

            // Prepare drop downs
            ViewBag.Cancellations = new[]
                {
                    new SelectListItem { Value = "No", Text = "No", Selected = true },
                    new SelectListItem { Value = "Yes", Text = "Yes" }
                };
            ViewBag.CancelRedirectUrl = cancelRedirectUrl;
            ViewBag.PatientName = patientName;

            return View(appointmentEditModel);
        }

        [HttpPost]
        [MvcUnitOfWork]
        public ActionResult EditAppointment(AppointmentEditModel model)
        {
            ViewBag.MenuItem = CurrentMenuItem;

            var cancelRedirectUrl = (TempData["cancelRedirectUrl"] ?? string.Empty).ToString();

            ViewBag.cancelRedirectUrl = cancelRedirectUrl;

            var appointmentRepository = unitOfWork.Repository<Appointment>();

            var appointment = appointmentRepository.Queryable().Include("Patient").Single(a => a.Id == model.AppointmentId);
            if (appointment == null)
            {
                ViewBag.Entity = "Appointment";
                return View("NotFound");
            }

            // If user has changed appointment date - then complete duplication check
            if (appointment.AppointmentDate != model.AppointmentDate)
            {
                var tempAppointment = unitOfWork.Repository<Appointment>()
                    .Queryable()
                    .Include(p => p.Patient)
                    .SingleOrDefault(a => a.Patient.Id == appointment.Patient.Id && a.AppointmentDate == model.AppointmentDate);

                if (tempAppointment != null)
                {
                    ModelState.AddModelError("AppointmentDate", "Patient already has an appointment for this date");
                }
            }

            if (model.AppointmentDate > DateTime.Today.AddYears(2))
            {
                ModelState.AddModelError("AppointmentDate", "Appointment Date should be within 2 years");
            }
            if (model.AppointmentDate < DateTime.Today)
            {
                ModelState.AddModelError("AppointmentDate", "Appointment Date should be after current date");
            }

            if (ModelState.IsValid)
            {
                appointment.AppointmentDate = model.AppointmentDate;
                appointment.Reason = model.Reason;
                appointment.Cancelled = model.Cancelled == "Yes" ? true : false;
                appointment.CancellationReason = model.CancellationReason;

                appointmentRepository.Update(appointment);
                unitOfWork.Complete();

                HttpCookie cookie = new HttpCookie("PopUpMessage");
                cookie.Value = "Appointment record updated successfully";
                Response.Cookies.Add(cookie);

                return Redirect("/Patient/PatientView.aspx?pid=" + appointment.Patient.Id.ToString());
            }

            string patientName = appointment.Patient.FullName;

            ViewBag.Cancellations = new[]
                {
                    new SelectListItem { Value = "No", Text = "No", Selected = true },
                    new SelectListItem { Value = "Yes", Text = "Yes" }
                };
            ViewBag.PatientName = patientName;

            return View(model);
        }

        [MvcUnitOfWork]
        [HttpGet]
        public ActionResult DeleteAppointment(int id)
        {
            ViewBag.MenuItem = CurrentMenuItem;

            var returnUrl = (Request.UrlReferrer ?? (object)string.Empty).ToString();
            TempData["returnUrl"] = returnUrl;

            ViewBag.ReturnUrl = returnUrl;

            var appointment = unitOfWork.Repository<Appointment>()
                .Queryable()
                .Include(i1 => i1.Patient)
                .SingleOrDefault(p => p.Id == id);

            if (appointment == null)
            {
                ViewBag.Entity = "Appointment";
                return View("NotFound");
            }

            var model = new AppointmentDeleteModel
            {
                AppointmentId = appointment.Id,
                AppointmentDate = appointment.AppointmentDate,
                PatientFullName = appointment.Patient.FullName 
            };

            return View(model);
        }

        [HttpPost]
        public ActionResult DeleteAppointment(AppointmentDeleteModel model)
        {
            var returnUrl = (TempData["returnUrl"] ?? string.Empty).ToString();
            ViewBag.ReturnUrl = returnUrl;

            var appointment = unitOfWork.Repository<Appointment>()
                .Queryable()
                .Include(i1 => i1.Patient)
                .SingleOrDefault(p => p.Id == model.AppointmentId);

            if (appointment != null)
            {
                var user = GetCurrentUser();

                if (user != null)
                {
                    if (ModelState.IsValid)
                    {
                        var reason = model.ArchiveReason ?? "** NO REASON SPECIFIED ** ";
                        appointment.Archived = true;
                        appointment.ArchivedDate = DateTime.Now;
                        appointment.ArchivedReason = reason;
                        appointment.AuditUser = user;
                        unitOfWork.Repository<Appointment>().Update(appointment);
                        unitOfWork.Complete();

                        HttpCookie cookie = new HttpCookie("PopUpMessage");
                        cookie.Value = "Appointment record deleted successfully";
                        Response.Cookies.Add(cookie);

                        return Redirect(returnUrl);
                    }
                }
            }

            TempData["returnUrl"] = returnUrl;

            return View(model);
        }

        private User GetCurrentUser()
        {
            return unitOfWork.Repository<User>().Queryable().SingleOrDefault(u => u.UserName == HttpContext.User.Identity.Name);
        }

        [HttpPost]
        public JsonResult CheckHoliday(DateTime appointmentDate)
        {
            var success = "OK";
            var message = "";

            try
            {
                var holiday = unitOfWork.Repository<Holiday>().Queryable().
                    FirstOrDefault(h => h.HolidayDate == appointmentDate);
                message = holiday == null ? String.Format("No holiday found for {0}", appointmentDate.ToString("yyyy-MM-dd")) : String.Format("Holiday found for {0}: {1}", appointmentDate.ToString("yyyy-MM-dd"), holiday.Description);
            }
            catch (Exception ex)
            {
                success = "FAILED";
                message = ex.Message;
            }

            var result = new { Success = success, Message = message };
            return Json(result, JsonRequestBehavior.AllowGet);
        }
    }
}
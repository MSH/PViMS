using Microsoft.AspNet.Identity;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using VPS.Common.Repositories;

using PVIMS.Web.Models;
using PVIMS.Core.Entities;
using PVIMS.Core.Models;
using PVIMS.Web.ActionFilters;

namespace PVIMS.Web.Controllers
{
    public class PatientController : BaseController
    {
        private static string CurrentMenuItem = "PatientView";

        private readonly IUnitOfWorkInt unitOfWork;

        public PatientController(IUnitOfWorkInt unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        // GET: Patient
        public ActionResult PatientSearch()
        {
            return View();
        }

        // GET: Patient
        public ActionResult PatientView()
        {
            return View();
        }

        [HttpGet]
        public ActionResult DeletePatient(long id, string cancelRedirectUrl)
        {
            ViewBag.MenuItem = CurrentMenuItem;

            TempData["cancelRedirectUrl"] = cancelRedirectUrl;
            ViewBag.CancelRedirectUrl = cancelRedirectUrl;

            var patient = unitOfWork.Repository<Patient>().Queryable().SingleOrDefault(u => u.Id == id);

            PatientDeleteModel model = new PatientDeleteModel() { PatientId = patient.Id, PatientFullName = patient.FullName };

            ViewData.Model = model;

            ViewBag.Id = id;
            ViewBag.AlertMessage = patient.HasClinicalData() ? "You are about to delete this record which has associated clinical data, appointments or encounters. This action is not reversible...." : "You are about to delete this record. This action is not reversible....";

            return View();
        }

        [HttpPost]
        public ActionResult DeletePatient(PatientDeleteModel model)
        {
            ViewBag.MenuItem = CurrentMenuItem;

            var cancelRedirectUrl = (TempData["cancelRedirectUrl"] ?? string.Empty).ToString();
            ViewBag.cancelRedirectUrl = cancelRedirectUrl;

            ArrayList errors = new ArrayList();

            if (ModelState.IsValid)
            {
                var patient = unitOfWork.Repository<Patient>().Queryable().SingleOrDefault(u => u.Id == model.PatientId);
                var currentUser = GetCurrentUser();

                try
                {
                    var reason = model.ArchiveReason.Trim() == "" ? "** NO REASON SPECIFIED ** " : model.ArchiveReason;
                    var archivedDate = DateTime.Now;

                    foreach (var appointment in patient.Appointments.Where(x => !x.Archived))
                    {
                        appointment.Archived = true;
                        appointment.ArchivedDate = archivedDate;
                        appointment.ArchivedReason = reason;
                        appointment.AuditUser = currentUser;
                        unitOfWork.Repository<Appointment>().Update(appointment);
                    }

                    foreach (var attachment in patient.Attachments.Where(x => !x.Archived))
                    {
                        attachment.Archived = true;
                        attachment.ArchivedDate = archivedDate;
                        attachment.ArchivedReason = reason;
                        attachment.AuditUser = currentUser;
                        unitOfWork.Repository<Attachment>().Update(attachment);
                    }

                    foreach (var enrolment in patient.CohortEnrolments.Where(x => !x.Archived))
                    {
                        enrolment.Archived = true;
                        enrolment.ArchivedDate = archivedDate;
                        enrolment.ArchivedReason = reason;
                        enrolment.AuditUser = currentUser;
                        unitOfWork.Repository<CohortGroupEnrolment>().Update(enrolment);
                    }

                    foreach (var encounter in patient.Encounters.Where(x => !x.Archived))
                    {
                        encounter.Archived = true;
                        encounter.ArchivedDate = archivedDate;
                        encounter.ArchivedReason = reason;
                        encounter.AuditUser = currentUser;
                        unitOfWork.Repository<Encounter>().Update(encounter);
                    }

                    foreach (var clinicalEvent in patient.PatientClinicalEvents.Where(x => !x.Archived))
                    {
                        clinicalEvent.Archived = true;
                        clinicalEvent.ArchivedDate = archivedDate;
                        clinicalEvent.ArchivedReason = reason;
                        clinicalEvent.AuditUser = currentUser;
                        unitOfWork.Repository<PatientClinicalEvent>().Update(clinicalEvent);
                    }

                    foreach (var condition in patient.PatientConditions.Where(x => !x.Archived))
                    {
                        condition.Archived = true;
                        condition.ArchivedDate = archivedDate;
                        condition.ArchivedReason = reason;
                        condition.AuditUser = currentUser;
                        unitOfWork.Repository<PatientCondition>().Update(condition);

                    }

                    foreach (var facility in patient.PatientFacilities.Where(x => !x.Archived))
                    {
                        facility.Archived = true;
                        facility.ArchivedDate = archivedDate;
                        facility.ArchivedReason = reason;
                        facility.AuditUser = currentUser;
                        unitOfWork.Repository<PatientFacility>().Update(facility);
                    }

                    foreach (var labTest in unitOfWork.Repository<PatientLabTest>().Queryable().Include(plt => plt.Patient).Include(plt => plt.LabTest).Include(plt => plt.TestUnit).Where(x => x.Patient.Id == patient.Id && !x.Archived))
                    {
                        labTest.Archived = true;
                        labTest.ArchivedDate = archivedDate;
                        labTest.ArchivedReason = reason;
                        labTest.AuditUser = currentUser;
                        unitOfWork.Repository<PatientLabTest>().Update(labTest);
                    }

                    foreach (var medication in patient.PatientMedications.Where(x => !x.Archived))
                    {
                        medication.Archived = true;
                        medication.ArchivedDate = archivedDate;
                        medication.ArchivedReason = reason;
                        medication.AuditUser = currentUser;
                        unitOfWork.Repository<PatientMedication>().Update(medication);
                    }

                    foreach (var status in patient.PatientStatusHistories.Where(x => !x.Archived))
                    {
                        status.Archived = true;
                        status.ArchivedDate = archivedDate;
                        status.ArchivedReason = reason;
                        status.AuditUser = currentUser;
                        unitOfWork.Repository<PatientStatusHistory>().Update(status);
                    }

                    patient.Archived = true;
                    patient.ArchivedDate = archivedDate;
                    patient.ArchivedReason = reason;
                    patient.AuditUser = currentUser;

                    unitOfWork.Repository<Patient>().Update(patient);
                    unitOfWork.Complete();

                }
                catch (DbUpdateException ex)
                {
                    errors.Add("Unable to archive patient. " + ex.Message);
                }
                catch (DbEntityValidationException ex)
                {
                    var err = string.Empty;
                    foreach (var eve in ex.EntityValidationErrors)
                    {
                        foreach (var ve in eve.ValidationErrors)
                        {
                            err += String.Format("- Property: \"{0}\", Error: \"{1}\"", ve.PropertyName, ve.ErrorMessage);
                        }
                    }
                    errors.Add(err);
                }
                if (errors.Count == 0)
                {
                    HttpCookie cookie = new HttpCookie("PopUpMessage");
                    cookie.Value = "Patient record deleted successfully";
                    Response.Cookies.Add(cookie);

                    return Redirect("/Patient/PatientSearch.aspx");
                }
                else
                {
                    AddErrors(errors);
                }
            }

            return View(model);
        }

        [MvcUnitOfWork]
        [HttpGet]
        public ActionResult DeleteAttachment(int id)
        {
            ViewBag.MenuItem = CurrentMenuItem;

            var returnUrl = (Request.UrlReferrer ?? (object)string.Empty).ToString();
            TempData["returnUrl"] = returnUrl;

            ViewBag.ReturnUrl = returnUrl;

            var attachment = unitOfWork.Repository<Attachment>()
                .Queryable()
                .Include(i1 => i1.Patient)
                .SingleOrDefault(p => p.Id == id);

            if (attachment == null)
            {
                ViewBag.Entity = "Attachment";
                return View("NotFound");
            }

            var model = new AttachmentDeleteModel
            {
                AttachmentId = attachment.Id,
                Description = attachment.Description,
                FileName = attachment.FileName,
                PatientFullName = attachment.Patient.FullName
            };

            return View(model);
        }

        [HttpPost]
        public ActionResult DeleteAttachment(AttachmentDeleteModel model)
        {
            var returnUrl = (TempData["returnUrl"] ?? string.Empty).ToString();
            ViewBag.ReturnUrl = returnUrl;

            var attachment = unitOfWork.Repository<Attachment>()
                .Queryable()
                .Include(i1 => i1.Patient)
                .SingleOrDefault(p => p.Id == model.AttachmentId);

            if (attachment != null)
            {
                var user = GetCurrentUser();

                if (user != null)
                {
                    if (ModelState.IsValid)
                    {
                        var reason = model.ArchiveReason ?? "** NO REASON SPECIFIED ** ";
                        attachment.Archived = true;
                        attachment.ArchivedDate = DateTime.Now;
                        attachment.ArchivedReason = reason;
                        attachment.AuditUser = user;
                        unitOfWork.Repository<Attachment>().Update(attachment);
                        unitOfWork.Complete();

                        HttpCookie cookie = new HttpCookie("PopUpMessage");
                        cookie.Value = "Attachment record deleted successfully";
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
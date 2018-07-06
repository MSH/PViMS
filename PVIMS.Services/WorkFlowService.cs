using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;

using VPS.Common.Repositories;
using VPS.Common.Utilities;

using VPS.CustomAttributes;

using PVIMS.Core;
using PVIMS.Core.Entities;
using PVIMS.Core.Models;
using PVIMS.Core.Services;

using SelectionDataItem = PVIMS.Core.Entities.SelectionDataItem;
using CustomAttributeConfiguration = PVIMS.Core.Entities.CustomAttributeConfiguration;

namespace PVIMS.Services
{
    public class WorkFlowService : IWorkFlowService 
    {
        private readonly IUnitOfWorkInt _unitOfWork;

        public IArtefactService _artefactService { get; set; }
        public ICustomAttributeService _attributeService { get; set; }
        public IPatientService _patientService { get; set; }

        public WorkFlowService(IUnitOfWorkInt unitOfWork, ICustomAttributeService attributeService, IPatientService patientService, IArtefactService artefactService)
        {
            Check.IsNotNull(unitOfWork, "unitOfWork may not be null");
            Check.IsNotNull(artefactService, "artefactService may not be null");
            Check.IsNotNull(attributeService, "attributeService may not be null");
            Check.IsNotNull(patientService, "patientService may not be null");

            _unitOfWork = unitOfWork;

            _artefactService = artefactService;
            _attributeService = attributeService;
            _patientService = patientService;
        }

        public void AddOrUpdateMedicationsForWorkFlowInstance(Guid contextGuid, List<ReportInstanceMedicationListItem> medications)
        {
            Check.IsNotNull(contextGuid, "contextGuid may not be null");
            Check.IsNotNull(medications, "medications may not be null");

            if (medications.Count == 0) { return; };

            ReportInstance reportInstance = _unitOfWork.Repository<ReportInstance>().Queryable().Include(ri => ri.Medications).SingleOrDefault(ri => ri.ContextGuid == contextGuid);
            if (reportInstance == null) { return; };

            // Full managements of medications list for report instance
            ArrayList addCollection = new ArrayList();
            ArrayList modifyCollection = new ArrayList();
            foreach (ReportInstanceMedicationListItem medication in medications)
            {
                if(reportInstance.Medications != null)
                {
                    var exists = reportInstance.Medications.Any(m => m.ReportInstanceMedicationGuid == medication.ReportInstanceMedicationGuid);
                    if (exists)
                    {
                        modifyCollection.Add(medication);
                    }
                    else
                    {
                        addCollection.Add(medication);
                    }
                }
                else
                {
                    addCollection.Add(medication);
                }
            }

            foreach (ReportInstanceMedicationListItem medication in addCollection)
            {
                var med = new ReportInstanceMedication() { MedicationIdentifier = medication.MedicationIdentifier, ReportInstance = reportInstance, ReportInstanceMedicationGuid = medication.ReportInstanceMedicationGuid };
                reportInstance.Medications.Add(med);

                _unitOfWork.Repository<ReportInstanceMedication>().Save(med);
            }
            foreach (ReportInstanceMedicationListItem medication in modifyCollection)
            {
                var med = reportInstance.Medications.Single(m => m.ReportInstanceMedicationGuid == medication.ReportInstanceMedicationGuid);
                med.MedicationIdentifier = medication.MedicationIdentifier;

                _unitOfWork.Repository<ReportInstanceMedication>().Update(med);
            }
        }

        public void CreateWorkFlowInstance(string workFlowName, Guid contextGuid, string patientIdentifier, string sourceIdentifier)
        {
            Check.IsEmpty(workFlowName, "workFlow may not be empty");
            Check.IsNotNull(contextGuid, "contextGuid may not be null");

            // Ensure instance does not exist for this context
            var workFlow = _unitOfWork.Repository<WorkFlow>().Queryable().SingleOrDefault(wf => wf.Description == workFlowName);
            Check.IsNotNull(workFlow, "contextGuid may not be null");

            var currentUser = UserContext.CurrentUser != null ? _unitOfWork.Repository<User>().Queryable().SingleOrDefault(u => u.Id == UserContext.CurrentUser.Id) : null;

            ReportInstance reportInstance = _unitOfWork.Repository<ReportInstance>().Queryable().SingleOrDefault(ri => ri.ContextGuid == contextGuid);
            if(reportInstance == null)
            {
                reportInstance = new ReportInstance(workFlow, currentUser)
                {
                    ContextGuid = contextGuid,
                    PatientIdentifier = patientIdentifier,
                    SourceIdentifier = sourceIdentifier
                };
                _unitOfWork.Repository<ReportInstance>().Save(reportInstance);

                reportInstance.SetIdentifier();

                _unitOfWork.Repository<ReportInstance>().Update(reportInstance);
                _unitOfWork.Complete();
            }
        }

        public int CheckWorkFlowInstanceCount(string workFlowName)
        {
            var config = _unitOfWork.Repository<Config>().Queryable().Where(c => c.ConfigType == ConfigType.ReportInstanceNewAlertCount).SingleOrDefault();
            if (config != null)
            {
                if (!String.IsNullOrEmpty(config.ConfigValue))
                {
                    var alertCount = Convert.ToInt32(config.ConfigValue);

                    // How many instances within the last alertcount
                    var compDate = DateTime.Now.AddDays(alertCount * -1);
                    return _unitOfWork.Repository<ReportInstance>().Queryable().Where(rp => rp.WorkFlow.Description == workFlowName && rp.Created >= compDate && rp.Finished == null).Count();
                }
            }
            return 0;
        }

        public void DeleteMedicationsFromWorkFlowInstance(Guid contextGuid, List<ReportInstanceMedicationListItem> medications)
        {
            Check.IsNotNull(contextGuid, "contextGuid may not be null");
            Check.IsNotNull(medications, "medications may not be null");

            if (medications.Count == 0) { return; };

            ReportInstance reportInstance = _unitOfWork.Repository<ReportInstance>().Queryable().Single(ri => ri.ContextGuid == contextGuid);

            // Full managements of medications list for report instance
            ArrayList deleteCollection = new ArrayList();
            foreach (ReportInstanceMedication medication in reportInstance.Medications)
            {
                var exists = medications.Any(m => m.ReportInstanceMedicationGuid == medication.ReportInstanceMedicationGuid);
                if (exists) { deleteCollection.Add(medication); };
            }

            foreach (ReportInstanceMedication medication in deleteCollection)
            {
                reportInstance.Medications.Remove(medication);
                _unitOfWork.Repository<ReportInstanceMedication>().Delete(medication);
            }

            _unitOfWork.Repository<ReportInstance>().Update(reportInstance);
        }

        public ActivityExecutionStatusEvent ExecuteActivity(Guid contextGuid, string newStatus, string comments, DateTime? contextDate, string contextCode)
        {
            var reportInstance = _unitOfWork.Repository<ReportInstance>().Queryable().Single(ri => ri.ContextGuid == contextGuid);
            var activityInstance = reportInstance.CurrentActivity;
            var newExecutionStatus = GetExecutionStatusForActivity(reportInstance, newStatus);
            var currentUser = UserContext.CurrentUser != null ? _unitOfWork.Repository<User>().Queryable().SingleOrDefault(u => u.Id == UserContext.CurrentUser.Id) : null;

            var newEvent = activityInstance.AddNewEvent(newExecutionStatus, currentUser, comments, contextDate, contextCode);

            _unitOfWork.Repository<ActivityInstance>().Update(activityInstance);

            if (activityInstance.CurrentStatus.Description == "E2BGENERATED")
            {
                CreatePatientSummaryAndLink(reportInstance, newEvent);
                CreatePatientExtractAndLink(reportInstance, newEvent);
                CreateE2BExtractAndLink(reportInstance, newEvent);
            }

            if (activityInstance.CurrentStatus.Description == "CONFIRMED")
            {
                activityInstance.Current = false;
                _unitOfWork.Repository<ActivityInstance>().Update(activityInstance);

                var newActivity = _unitOfWork.Repository<Activity>().Queryable().Include(a1 => a1.WorkFlow).Single(a => a.WorkFlow.Id == reportInstance.WorkFlow.Id && a.QualifiedName == "Set MedDRA and Causality");
                reportInstance.SetNewActivity(newActivity, currentUser);

                _unitOfWork.Repository<ReportInstance>().Update(reportInstance);
            }

            if (activityInstance.CurrentStatus.Description == "CAUSALITYSET")
            {
                activityInstance.Current = false;
                _unitOfWork.Repository<ActivityInstance>().Update(activityInstance);

                var newActivity = _unitOfWork.Repository<Activity>().Queryable().Include(a1 => a1.WorkFlow).Single(a => a.WorkFlow.Id == reportInstance.WorkFlow.Id && a.QualifiedName == "Extract E2B");
                reportInstance.SetNewActivity(newActivity, currentUser);

                _unitOfWork.Repository<ReportInstance>().Update(reportInstance);
            }

            _unitOfWork.Complete();

            return newEvent;
        }

        public TerminologyMedDra GetCurrentAdverseReaction(Patient patient)
        {
            foreach (PatientClinicalEvent clinicalEvent in patient.PatientClinicalEvents)
            {
                var term = GetTerminologyMedDraForReportInstance(clinicalEvent.PatientClinicalEventGuid);
                if (term != null) { return term; };
            };

            return null;
        }

        public ActivityExecutionStatus GetExecutionStatusForActivity(ReportInstance reportInstance, string getStatus)
        {
            var activityInstance = reportInstance.Activities.Single(a => a.Current == true);
            var activity = _unitOfWork.Repository<Activity>().Queryable().Single(a => a.QualifiedName == activityInstance.QualifiedName && a.WorkFlow.Id == reportInstance.WorkFlow.Id);
            return _unitOfWork.Repository<ActivityExecutionStatus>().Queryable().Single(aes => aes.Activity.Id == activity.Id && aes.Description == getStatus);
        }

        public List<ActivityExecutionStatusForPatient> GetExecutionStatusEventsForPatientView(Patient patient)
        {
            var clinicalEvents =
                _unitOfWork.Repository<PatientClinicalEvent>().Queryable()
                    .Where(pce => pce.Patient.Id == patient.Id && pce.Archived == false);

            List<ActivityExecutionStatusForPatient> results = new List<ActivityExecutionStatusForPatient>();

            foreach (var clinicalEvent in clinicalEvents)
            {
                var reportInstance = _unitOfWork.Repository<ReportInstance>().Queryable().SingleOrDefault(ri => ri.ContextGuid == clinicalEvent.PatientClinicalEventGuid);
                if(reportInstance != null)
                {
                    var result = new ActivityExecutionStatusForPatient();
                    result.PatientClinicalEvent = clinicalEvent;

                    var items = _unitOfWork.Repository<ActivityExecutionStatusEvent>().Queryable().Where(aese => aese.ActivityInstance.ReportInstance.Id == reportInstance.Id).OrderByDescending(aese => aese.EventDateTime).Take(1);
                    foreach (ActivityExecutionStatusEvent item in items)
                    {
                        var activityItem = new ActivityExecutionStatusForPatient.ActivityExecutionStatusInfo()
                        {
                            Comments = item.Comments,
                            Status = item.ExecutionStatus.FriendlyDescription,
                            StatusDate = item.EventDateTime.ToString("yyyy-MM-dd")
                        };
                        result.ActivityItems.Add(activityItem);
                    };

                    results.Add(result);
                }
            }

            return results;
        }

        public List<ActivityExecutionStatusForPatient> GetExecutionStatusEventsForEventView(PatientClinicalEvent clinicalEvent)
        {
            List<ActivityExecutionStatusForPatient> results = new List<ActivityExecutionStatusForPatient>();

            var reportInstance = _unitOfWork.Repository<ReportInstance>().Queryable().SingleOrDefault(ri => ri.ContextGuid == clinicalEvent.PatientClinicalEventGuid);
            if (reportInstance != null)
            {
                var result = new ActivityExecutionStatusForPatient();
                result.PatientClinicalEvent = clinicalEvent;

                var items = _unitOfWork.Repository<ActivityExecutionStatusEvent>().Queryable().Where(aese => aese.ActivityInstance.ReportInstance.Id == reportInstance.Id).OrderBy(aese => aese.EventDateTime);
                foreach (ActivityExecutionStatusEvent item in items)
                {
                    var activityItem = new ActivityExecutionStatusForPatient.ActivityExecutionStatusInfo()
                    {
                        Comments = item.Comments,
                        Status = item.ExecutionStatus.FriendlyDescription,
                        StatusDate = item.EventDateTime.ToString("yyyy-MM-dd")
                    };
                    result.ActivityItems.Add(activityItem);
                };

                results.Add(result);
            }

            return results;
        }

        public TerminologyMedDra GetTerminologyMedDraForReportInstance(Guid contextGuid)
        {
            Check.IsNotNull(contextGuid, "contextGuid may not be null");

            var reportInstance = _unitOfWork.Repository<ReportInstance>().Queryable().SingleOrDefault(ri => ri.ContextGuid == contextGuid);
            
            if(reportInstance != null)
            {
                return reportInstance.TerminologyMedDra;
            }
            return null;
        }

        public void UpdateIdentifiersForWorkFlowInstance(Guid contextGuid, string patientIdentifier, string sourceIdentifier)
        {
            Check.IsNotNull(contextGuid, "contextGuid may not be null");
            Check.IsNotNull(patientIdentifier, "patientIdentifier may not be null");
            Check.IsNotNull(sourceIdentifier, "sourceIdentifier may not be null");

            ReportInstance reportInstance = _unitOfWork.Repository<ReportInstance>().Queryable().SingleOrDefault(ri => ri.ContextGuid == contextGuid);
            if(reportInstance != null)
            {
                reportInstance.PatientIdentifier = patientIdentifier;
                reportInstance.SourceIdentifier = sourceIdentifier;
                _unitOfWork.Repository<ReportInstance>().Update(reportInstance);
            }
        }

        private void CreatePatientSummaryAndLink(ReportInstance reportInstance, ActivityExecutionStatusEvent newEvent)
        {
            ArtefactInfoModel path = null;
            if (reportInstance.WorkFlow.Description == "New Active Surveilliance Report")
            {
                var clinicalEvt = _unitOfWork.Repository<PatientClinicalEvent>().Queryable().Single(pce => pce.PatientClinicalEventGuid == reportInstance.ContextGuid);
                var extendable = (IExtendable)clinicalEvt;
                var extendableValue = extendable.GetAttributeValue("Is the adverse event serious?");
                var isSerious = extendableValue != null ? extendableValue.ToString() == "1" ? true : false : false;

                path = _artefactService.CreatePatientSummaryForActiveReport(reportInstance.ContextGuid, isSerious);
            }
            else
            {
                var sourceInstance = _unitOfWork.Repository<DatasetInstance>().Queryable().Single(di => di.DatasetInstanceGuid == reportInstance.ContextGuid);
                var isSerious = sourceInstance.GetInstanceValue(_unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.DatasetElementGuid.ToString() == "302C07C9-B0E0-46AB-9EF8-5D5C2F756BF1"));

                path = _artefactService.CreatePatientSummaryForSpontaneousReport(reportInstance.ContextGuid, (!String.IsNullOrWhiteSpace(isSerious)));
            }

            // Create patient summary and link to event
            Attachment att;
            AttachmentType attType = _unitOfWork.Repository<AttachmentType>().Queryable().Single(at => at.Key == "docx");
            FileStream tempFile = File.OpenRead(path.FullPath);

            if (tempFile.Length > 0)
            {
                BinaryReader rdr = new BinaryReader(tempFile);
                byte[] buffer = rdr.ReadBytes((int)tempFile.Length);

                // Create the attachment
                att = new Attachment
                {
                    ActivityExecutionStatusEvent = newEvent,
                    Description = "PatientSummary",
                    FileName = Path.GetFileName(path.FileName),
                    AttachmentType = attType,
                    Size = tempFile.Length,
                    Content = buffer
                };
                newEvent.Attachments.Add(att);

                _unitOfWork.Repository<Attachment>().Save(att);
            }
            tempFile.Close();
            tempFile = null;
        }

        private void CreatePatientExtractAndLink(ReportInstance reportInstance, ActivityExecutionStatusEvent newEvent)
        {
            ArtefactInfoModel path = null;
            if (reportInstance.WorkFlow.Description == "New Active Surveilliance Report")
            {
                var clinicalEvt = _unitOfWork.Repository<PatientClinicalEvent>().Queryable().Single(pce => pce.PatientClinicalEventGuid == reportInstance.ContextGuid);
                path = _artefactService.CreateActiveDatasetForDownload(clinicalEvt.Patient.Id);
            }
            else
            {
                var sourceInstance = _unitOfWork.Repository<DatasetInstance>().Queryable().Single(di => di.DatasetInstanceGuid == reportInstance.ContextGuid);
                path = _artefactService.CreateDatasetInstanceForDownload(sourceInstance.Id);
            }

            // Create patient summary and link to event
            Attachment att;
            AttachmentType attType = _unitOfWork.Repository<AttachmentType>().Queryable().Single(at => at.Key == "xlsx");
            FileStream tempFile = File.OpenRead(path.FullPath);

            if (tempFile.Length > 0)
            {
                BinaryReader rdr = new BinaryReader(tempFile);
                byte[] buffer = rdr.ReadBytes((int)tempFile.Length);

                // Create the attachment
                att = new Attachment
                {
                    ActivityExecutionStatusEvent = newEvent,
                    Description = "PatientExtract",
                    FileName = Path.GetFileName(path.FileName),
                    AttachmentType = attType,
                    Size = tempFile.Length,
                    Content = buffer
                };
                newEvent.Attachments.Add(att);

                _unitOfWork.Repository<Attachment>().Save(att);
            }
            tempFile.Close();
            tempFile = null;
        }

        private void CreateE2BExtractAndLink(ReportInstance reportInstance, ActivityExecutionStatusEvent newEvent)
        {
            ArtefactInfoModel path = null;

            var activityInstance = reportInstance.CurrentActivity;

            DatasetInstance datasetInstance = null;
            var evt = activityInstance.ExecutionEvents.OrderByDescending(ee => ee.EventDateTime).First(ee => ee.ExecutionStatus.Description == "E2BINITIATED");
            var tag = (reportInstance.WorkFlow.Description == "New Active Surveilliance Report") ? "Active" : "Spontaneous";

            datasetInstance
                = _unitOfWork.Repository<DatasetInstance>()
            .Queryable()
            .Include("DatasetInstanceValues.DatasetElement.Field.FieldType")
            .Include("DatasetInstanceValues.DatasetInstanceSubValues.DatasetElementSub.Field.FieldType")
            .Where(di => di.Tag == tag
                && di.ContextID == evt.Id).SingleOrDefault();

            path = _artefactService.CreateE2B(datasetInstance.Id);

            Attachment att;
            AttachmentType attType = _unitOfWork.Repository<AttachmentType>().Queryable().Single(at => at.Key == "xml");
            FileStream tempFile = File.OpenRead(path.FullPath);

            if (tempFile.Length > 0)
            {
                BinaryReader rdr = new BinaryReader(tempFile);
                byte[] buffer = rdr.ReadBytes((int)tempFile.Length);

                // Create the attachment
                att = new Attachment
                {
                    ActivityExecutionStatusEvent = newEvent,
                    Description = "E2b",
                    FileName = Path.GetFileName(path.FileName),
                    AttachmentType = attType,
                    Size = tempFile.Length,
                    Content = buffer
                };
                newEvent.Attachments.Add(att);

                _unitOfWork.Repository<Attachment>().Save(att);
            }
            tempFile.Close();
            tempFile = null;
        }
    }
}

using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using A = DocumentFormat.OpenXml.Drawing;
using DW = DocumentFormat.OpenXml.Drawing.Wordprocessing;
using PIC = DocumentFormat.OpenXml.Drawing.Pictures;

using OfficeOpenXml;
using OfficeOpenXml.Style;

using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web;
using System.Xml;

using VPS.Common.Repositories;
using VPS.Common.Utilities;

using VPS.CustomAttributes;

using PVIMS.Core.Entities;
using PVIMS.Core.Models;
using PVIMS.Core.Services;
using PVIMS.Core.Utilities;

using SelectionDataItem = PVIMS.Core.Entities.SelectionDataItem;
using CustomAttributeConfiguration = PVIMS.Core.Entities.CustomAttributeConfiguration;
using FrameworkCustomAttributeConfiguration = VPS.CustomAttributes.CustomAttributeConfiguration;

namespace PVIMS.Services
{
    public class ArtefactService : IArtefactService
    {
        private readonly IUnitOfWorkInt _unitOfWork;

        public ICustomAttributeService _attributeService { get; set; }
        public IPatientService _patientService { get; set; }

        public ArtefactService(IUnitOfWorkInt unitOfWork, ICustomAttributeService attributeService, IPatientService patientService)
        {
            Check.IsNotNull(unitOfWork, "unitOfWork may not be null");
            Check.IsNotNull(attributeService, "unitOfWork may not be null");
            Check.IsNotNull(patientService, "unitOfWork may not be null");

            _unitOfWork = unitOfWork;

            _attributeService = attributeService;
            _patientService = patientService;
        }

        public ArtefactInfoModel CreateActiveDatasetForDownload(long patientId, long cohortGroupId)
        {
            var model = new ArtefactInfoModel();
            var generatedDate = DateTime.Now;

            model.Path = String.Format("{0}\\Temp\\", System.AppDomain.CurrentDomain.BaseDirectory);
            model.FileName = string.Format("Active_DataExtract_{0}.xlsx", generatedDate.ToString("yyyyMMddhhmmss"));

            using (var pck = new ExcelPackage(new FileInfo(model.FullPath)))
            {
                // *************************************
                // Create sheet - Patient
                // *************************************
                var ws = pck.Workbook.Worksheets.Add("Patient");
                ws.View.ShowGridLines = true;

                var rowCount = 1;
                var colCount = 1;

                var patientquery = _unitOfWork.Repository<Patient>().Queryable().Where(p => p.Archived == false);
                if(patientId > 0)
                {
                    patientquery = patientquery.Where(p => p.Id == patientId);
                }
                if (cohortGroupId > 0)
                {
                    patientquery = patientquery.Where(p => p.CohortEnrolments.Any(cge => cge.CohortGroup.Id == cohortGroupId));
                }

                var patients = patientquery.OrderBy(p => p.Id).ToList();
                foreach (Patient patient in patients)
                {
                    ProcessEntity(patient, ref ws, ref rowCount, ref colCount, "Patient");
                }
                //format row
                using (var r = ws.Cells["A1:" + GetExcelColumnName(colCount) + rowCount])
                {
                    r.Style.Font.SetFromFont(new System.Drawing.Font("Arial", 10, FontStyle.Regular));
                    r.Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    r.AutoFitColumns();
                }

                // *************************************
                // Create sheet - PatientMedication
                // *************************************
                ws = pck.Workbook.Worksheets.Add("PatientMedication");
                ws.View.ShowGridLines = true;

                rowCount = 1;
                colCount = 1;

                var medicationquery = _unitOfWork.Repository<PatientMedication>().Queryable().Where(pm => pm.Archived == false);
                if (patientId > 0)
                {
                    medicationquery = medicationquery.Where(pm => pm.Patient.Id == patientId);
                }
                if (cohortGroupId > 0)
                {
                    medicationquery = medicationquery.Where(pm => pm.Patient.CohortEnrolments.Any(cge => cge.CohortGroup.Id == cohortGroupId));
                }
                var medications = medicationquery.OrderBy(pm => pm.Id).ToList();
                foreach (PatientMedication medication in medications)
                {
                    ProcessEntity(medication, ref ws, ref rowCount, ref colCount, "PatientMedication");
                }
                //format row
                using (var r = ws.Cells["A1:" + GetExcelColumnName(colCount) + rowCount])
                {
                    r.Style.Font.SetFromFont(new System.Drawing.Font("Arial", 10, FontStyle.Regular));
                    r.Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    r.AutoFitColumns();
                }

                // *************************************
                // Create sheet - PatientClinicalEvent
                // *************************************
                ws = pck.Workbook.Worksheets.Add("PatientClinicalEvent");
                ws.View.ShowGridLines = true;

                rowCount = 1;
                colCount = 1;

                var eventquery = _unitOfWork.Repository<PatientClinicalEvent>().Queryable().Where(pc => pc.Archived == false);
                if (patientId > 0)
                {
                    eventquery = eventquery.Where(pc => pc.Patient.Id == patientId);
                }
                if (cohortGroupId > 0)
                {
                    eventquery = eventquery.Where(pc => pc.Patient.CohortEnrolments.Any(cge => cge.CohortGroup.Id == cohortGroupId));
                }
                var events = eventquery.OrderBy(pc => pc.Id).ToList();
                foreach (PatientClinicalEvent clinicalEvent in events)
                {
                    ProcessEntity(clinicalEvent, ref ws, ref rowCount, ref colCount, "PatientClinicalEvent");
                }
                //format row
                using (var r = ws.Cells["A1:" + GetExcelColumnName(colCount) + rowCount])
                {
                    r.Style.Font.SetFromFont(new System.Drawing.Font("Arial", 10, FontStyle.Regular));
                    r.Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    r.AutoFitColumns();
                }

                // *************************************
                // Create sheet - PatientCondition
                // *************************************
                ws = pck.Workbook.Worksheets.Add("PatientCondition");
                ws.View.ShowGridLines = true;

                rowCount = 1;
                colCount = 1;

                var conditionquery = _unitOfWork.Repository<PatientCondition>().Queryable().Where(pc => pc.Archived == false);
                if (patientId > 0)
                {
                    conditionquery = conditionquery.Where(pc => pc.Patient.Id == patientId);
                }
                if (cohortGroupId > 0)
                {
                    conditionquery = conditionquery.Where(pc => pc.Patient.CohortEnrolments.Any(cge => cge.CohortGroup.Id == cohortGroupId));
                }
                var conditions = conditionquery.OrderBy(pc => pc.Id).ToList();
                foreach (PatientCondition condition in conditions)
                {
                    ProcessEntity(condition, ref ws, ref rowCount, ref colCount, "PatientCondition");
                }
                //format row
                using (var r = ws.Cells["A1:" + GetExcelColumnName(colCount) + rowCount])
                {
                    r.Style.Font.SetFromFont(new System.Drawing.Font("Arial", 10, FontStyle.Regular));
                    r.Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    r.AutoFitColumns();
                }

                // *************************************
                // Create sheet - PatientLabTest
                // *************************************
                ws = pck.Workbook.Worksheets.Add("PatientLabTest");
                ws.View.ShowGridLines = true;

                rowCount = 1;
                colCount = 1;

                var labtestquery = _unitOfWork.Repository<PatientLabTest>().Queryable().Where(pl => pl.Archived == false);
                if (patientId > 0)
                {
                    labtestquery = labtestquery.Where(pl => pl.Patient.Id == patientId);
                }
                if (cohortGroupId > 0)
                {
                    labtestquery = labtestquery.Where(pl => pl.Patient.CohortEnrolments.Any(cge => cge.CohortGroup.Id == cohortGroupId));
                }
                var labTests = labtestquery.OrderBy(pl => pl.Id).ToList();
                foreach (PatientLabTest labTest in labTests)
                {
                    ProcessEntity(labTest, ref ws, ref rowCount, ref colCount, "PatientLabTest");
                }
                //format row
                using (var r = ws.Cells["A1:" + GetExcelColumnName(colCount) + rowCount])
                {
                    r.Style.Font.SetFromFont(new System.Drawing.Font("Arial", 10, FontStyle.Regular));
                    r.Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    r.AutoFitColumns();
                }

                // *************************************
                // Create sheet - Encounter
                // *************************************
                ws = pck.Workbook.Worksheets.Add("Encounter");
                ws.View.ShowGridLines = true;

                rowCount = 1;
                colCount = 1;

                var encounterquery = _unitOfWork.Repository<Encounter>().Queryable().Where(e => e.Archived == false);
                if (patientId > 0)
                {
                    encounterquery = encounterquery.Where(e => e.Patient.Id == patientId);
                }
                if (cohortGroupId > 0)
                {
                    encounterquery = encounterquery.Where(e => e.Patient.CohortEnrolments.Any(cge => cge.CohortGroup.Id == cohortGroupId));
                }
                var encounters = encounterquery.OrderBy(e => e.Id).ToList();
                foreach (Encounter encounter in encounters)
                {
                    ProcessEntity(encounter, ref ws, ref rowCount, ref colCount, "Encounter");
                }
                //format row
                using (var r = ws.Cells["A1:" + GetExcelColumnName(colCount) + rowCount])
                {
                    r.Style.Font.SetFromFont(new System.Drawing.Font("Arial", 10, FontStyle.Regular));
                    r.Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    r.AutoFitColumns();
                }

                // *************************************
                // Create sheet - CohortGroupEnrolment
                // *************************************
                ws = pck.Workbook.Worksheets.Add("CohortGroupEnrolment");
                ws.View.ShowGridLines = true;

                rowCount = 1;
                colCount = 1;

                var enrolmentquery = _unitOfWork.Repository<CohortGroupEnrolment>().Queryable().Where(e => e.Archived == false);
                if (patientId > 0)
                {
                    enrolmentquery = enrolmentquery.Where(e => e.Patient.Id == patientId);
                }
                if (cohortGroupId > 0)
                {
                    enrolmentquery = enrolmentquery.Where(e => e.Patient.CohortEnrolments.Any(cge => cge.CohortGroup.Id == cohortGroupId));
                }
                var enrolments = enrolmentquery.OrderBy(e => e.Id).ToList();
                foreach (CohortGroupEnrolment enrolment in enrolments)
                {
                    ProcessEntity(enrolment, ref ws, ref rowCount, ref colCount, "CohortGroupEnrolment");
                }
                //format row
                using (var r = ws.Cells["A1:" + GetExcelColumnName(colCount) + rowCount])
                {
                    r.Style.Font.SetFromFont(new System.Drawing.Font("Arial", 10, FontStyle.Regular));
                    r.Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    r.AutoFitColumns();
                }

                // *************************************
                // Create sheet - PatientFacility
                // *************************************
                ws = pck.Workbook.Worksheets.Add("PatientFacility");
                ws.View.ShowGridLines = true;

                rowCount = 1;
                colCount = 1;

                var facilityquery = _unitOfWork.Repository<PatientFacility>().Queryable().Where(pf => pf.Archived == false);
                if (patientId > 0)
                {
                    facilityquery = facilityquery.Where(pf => pf.Patient.Id == patientId);
                }
                if (cohortGroupId > 0)
                {
                    facilityquery = facilityquery.Where(pf => pf.Patient.CohortEnrolments.Any(cge => cge.CohortGroup.Id == cohortGroupId));
                }
                var facilities = facilityquery.OrderBy(pf => pf.Id).ToList();
                foreach (PatientFacility facility in facilities)
                {
                    ProcessEntity(facility, ref ws, ref rowCount, ref colCount, "PatientFacility");
                }
                //format row
                using (var r = ws.Cells["A1:" + GetExcelColumnName(colCount) + rowCount])
                {
                    r.Style.Font.SetFromFont(new System.Drawing.Font("Arial", 10, FontStyle.Regular));
                    r.Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    r.AutoFitColumns();
                }

                pck.Save();
            }

            return model;
        }

        public ArtefactInfoModel CreateDatasetInstanceForDownload(long datasetInstanceId)
        {
            var model = new ArtefactInfoModel();
            var generatedDate = DateTime.Now;

            model.Path = String.Format("{0}\\Temp\\", System.AppDomain.CurrentDomain.BaseDirectory);
            model.FileName = string.Format("Instance_DataExtract_{0}.xlsx", generatedDate.ToString("yyyyMMddhhmmss"));

            using (var pck = new ExcelPackage(new FileInfo(model.FullPath)))
            {
                // Create XLS
                var ws = pck.Workbook.Worksheets.Add("Spontaneous ID " + datasetInstanceId);
                ws.View.ShowGridLines = true;

                // Write headers
                ws.Cells["A1"].Value = "Dataset Name";
                ws.Cells["B1"].Value = "Dataset Category";
                ws.Cells["C1"].Value = "Element Name";
                ws.Cells["D1"].Value = "Field Type";
                ws.Cells["E1"].Value = "Value";

                //Set the first header and format it
                using (var r = ws.Cells["A1:E1"])
                {
                    r.Style.Font.SetFromFont(new System.Drawing.Font("Arial", 10, FontStyle.Bold));
                    r.Style.Font.Color.SetColor(System.Drawing.Color.White);
                    r.Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;
                    r.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    r.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(23, 55, 93));
                }

                // Write content
                // Get instance
                DatasetInstance datasetInstance = _unitOfWork.Repository<DatasetInstance>()
                    .Queryable()
                    .Include(di1 => di1.Dataset)
                    .Include(di2 => di2.DatasetInstanceValues)
                    .SingleOrDefault(di => di.Id == datasetInstanceId);

                var count = 1;
                // Loop through and render main table
                foreach (var value in datasetInstance.DatasetInstanceValues.Where(div1 => div1.DatasetElement.System == false && div1.DatasetElement.Field.FieldType.Description != "Table").OrderBy(div2 => div2.Id))
                {
                    count += 1;
                    ws.Cells["A" + count].Value = datasetInstance.Dataset.DatasetName;
                    ws.Cells["B" + count].Value = value.DatasetElement.DatasetCategoryElements.Single(dce => dce.DatasetCategory.Dataset.Id == datasetInstance.Dataset.Id).DatasetCategory.DatasetCategoryName;
                    ws.Cells["C" + count].Value = value.DatasetElement.ElementName;
                    ws.Cells["D" + count].Value = value.DatasetElement.Field.FieldType.Description;
                    ws.Cells["E" + count].Value = value.InstanceValue;
                };

                // Loop through and render sub tables
                var maxcount = 5;
                var subcount = 1;
                foreach (var value in datasetInstance.DatasetInstanceValues.Where(div1 => div1.DatasetElement.System == false && div1.DatasetElement.Field.FieldType.Description == "Table").OrderBy(div2 => div2.Id))
                {
                    count += 2;
                    ws.Cells["A" + count].Value = datasetInstance.Dataset.DatasetName;
                    ws.Cells["B" + count].Value = value.DatasetElement.DatasetCategoryElements.Single(dce => dce.DatasetCategory.Dataset.Id == datasetInstance.Dataset.Id).DatasetCategory.DatasetCategoryName;
                    ws.Cells["C" + count].Value = value.DatasetElement.ElementName;
                    ws.Cells["D" + count].Value = value.DatasetElement.Field.FieldType.Description;
                    ws.Cells["E" + count].Value = string.Empty;

                    if (value.DatasetInstanceSubValues.Count > 0)
                    {
                        // Write headers
                        count += 1;
                        foreach (var subElement in value.DatasetElement.DatasetElementSubs.Where(des1 => des1.System == false).OrderBy(des2 => des2.Id))
                        {
                            ws.Cells[GetExcelColumnName(subcount) + count].Value = subElement.ElementName;
                            subcount++;
                            maxcount = subcount > maxcount ? subcount : maxcount;
                        }

                        //Set the sub header and format it
                        using (var r = ws.Cells["A" + count + ":" + GetExcelColumnName(subcount) + count])
                        {
                            r.Style.Font.SetFromFont(new System.Drawing.Font("Arial", 10, FontStyle.Bold));
                            r.Style.Font.Color.SetColor(System.Drawing.Color.White);
                            r.Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;
                            r.Style.Fill.PatternType = ExcelFillStyle.Solid;
                            r.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(23, 55, 93));
                        }

                        // Get unique contexts
                        var contexts = datasetInstance.GetInstanceSubValuesContext(value.DatasetElement);
                        foreach (var context in contexts)
                        {
                            count += 1;
                            subcount = 1;
                            foreach (var subvalue in datasetInstance.GetInstanceSubValues(value.DatasetElement, context))
                            {
                                subcount = value.DatasetElement.DatasetElementSubs.ToList().IndexOf(subvalue.DatasetElementSub) + 1;
                                ws.Cells[GetExcelColumnName(subcount) + count].Value = subvalue.InstanceValue;
                            }
                        }
                    }
                };

                //format row
                using (var r = ws.Cells["A1:" + GetExcelColumnName(maxcount) + count])
                {
                    r.Style.Font.SetFromFont(new System.Drawing.Font("Arial", 10, FontStyle.Regular));
                    r.Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    r.AutoFitColumns();
                }

                pck.Save();
            }

            return model;
        }

        public ArtefactInfoModel CreateE2B(long datasetInstanceId)
        {
            var model = new ArtefactInfoModel();
            var generatedDate = DateTime.Now;

            model.Path = String.Format("{0}\\Temp\\", System.AppDomain.CurrentDomain.BaseDirectory);
            model.FileName = string.Format("E_{0}_{1}.xml", datasetInstanceId.ToString(), DateTime.Today.ToString("yyyyMMddhhmmsss"));

            // Create XML file
            DatasetInstance datasetInstance = _unitOfWork.Repository<DatasetInstance>()
                .Queryable()
                .Include("Dataset")
                .SingleOrDefault(ds => ds.Id == datasetInstanceId);

            // Get the corresponding xml
            DatasetXml dsXml = datasetInstance.Dataset.DatasetXml;

            XmlDocument xmlDoc = new XmlDocument();

            // Get the root note
            DatasetXmlNode dsXmlRootNode = dsXml.ChildrenNodes.SingleOrDefault(c => c.NodeType == NodeType.RootNode);
            if (dsXmlRootNode != null)
            {
                var preparedNode = ProcessNode(dsXmlRootNode, ref xmlDoc, ref datasetInstance);
                xmlDoc.AppendChild(preparedNode);
            }

            var contentXml = FormatXML(xmlDoc);

            WriteXML(model.FullPath, contentXml);

            return model;
        }

        public ArtefactInfoModel CreatePatientSummaryForActiveReport(Guid contextGuid, bool isSerious)
        {
            var model = new ArtefactInfoModel();
            var generatedDate = DateTime.Now;

            var patientClinicalEvent = _unitOfWork.Repository<PatientClinicalEvent>().Queryable().Single(pce => pce.PatientClinicalEventGuid == contextGuid);
            Check.IsNotNull(patientClinicalEvent, "patientClinicalEvent may not be null");

            model.Path = String.Format("{0}\\Temp\\", System.AppDomain.CurrentDomain.BaseDirectory);
            model.FileName = string.Format("{2}_{0}_{1}.docx", patientClinicalEvent.Patient.Id.ToString(), generatedDate.ToString("yyyyMMddhhmmss"), isSerious ? "SAEReport_Active" : "PatientSummary_Active");

            using (var document = WordprocessingDocument.Create(string.Format("{0}{1}", model.Path, model.FileName), WordprocessingDocumentType.Document))
            {
                // Add a main document part. 
                MainDocumentPart mainPart = document.AddMainDocumentPart();

                // Create the document structure and add some text.
                mainPart.Document = new Document();
                Body body = new Body();

                SectionProperties sectionProps = new SectionProperties();
                PageMargin pageMargin = new PageMargin() { Top = 404, Right = (UInt32Value)504U, Bottom = 404, Left = (UInt32Value)504U, Header = (UInt32Value)720U, Footer = (UInt32Value)720U, Gutter = (UInt32Value)0U };
                sectionProps.Append(pageMargin);
                body.Append(sectionProps);

                mainPart.Document.AppendChild(body);
            };

            using (var document = WordprocessingDocument.Open(string.Format("{0}{1}", model.Path, model.FileName), true))
            {
                var doc = document.MainDocumentPart.Document;
                var body = doc.Body;

                // Add page header
                body.Append(AddPatientSummaryPageHeader("PATIENT SUMMARY", document));

                var tableHeader = AddTableHeader("A. BASIC PATIENT INFORMATION");
                body.Append(tableHeader);
                var basicInformationtable = AddBasicInformationTable(patientClinicalEvent);
                body.Append(basicInformationtable);
                var notesTable = AddNotesTable();
                body.Append(notesTable);

                tableHeader = AddTableHeader("B. PRE-EXISITING CONDITIONS");
                body.Append(tableHeader);
                var conditiontable = AddConditionTable(patientClinicalEvent);
                body.Append(conditiontable);

                tableHeader = AddTableHeader("C. ADVERSE EVENT INFORMATION");
                body.Append(tableHeader);
                var adverseEventtable = AddAdverseEventTable(patientClinicalEvent, isSerious);
                body.Append(adverseEventtable);
                notesTable = AddNotesTable();
                body.Append(notesTable);

                tableHeader = AddTableHeader("D. MEDICATIONS");
                body.Append(tableHeader);
                var medicationtable = AddMedicationTable(patientClinicalEvent);
                body.Append(medicationtable);
                notesTable = AddNotesForMedicationTable();
                body.Append(notesTable);

                tableHeader = AddTableHeader("E. CLINICAL EVALUATIONS");
                body.Append(tableHeader);
                var evaluationtable = AddEvaluationTable(patientClinicalEvent);
                body.Append(evaluationtable);
                notesTable = AddNotesTable();
                body.Append(notesTable);

                tableHeader = AddTableHeader("F. WEIGHT HISTORY");
                body.Append(tableHeader);
                var weighttable = AddWeightTable(patientClinicalEvent);
                body.Append(weighttable);
                notesTable = AddNotesTable();
                body.Append(notesTable);

                document.Save();
            }

            return model;
        }

        public ArtefactInfoModel CreatePatientSummaryForSpontaneousReport(Guid contextGuid, bool isSerious)
        {
            var model = new ArtefactInfoModel();
            var generatedDate = DateTime.Now;

            var datasetInstance = _unitOfWork.Repository<DatasetInstance>().Queryable().Single(di => di.DatasetInstanceGuid == contextGuid);
            Check.IsNotNull(datasetInstance, "datasetInstance may not be null");

            model.Path = String.Format("{0}\\Temp\\", System.AppDomain.CurrentDomain.BaseDirectory);
            model.FileName = string.Format("{2}_{0}_{1}.docx", datasetInstance.Id.ToString(), generatedDate.ToString("yyyyMMddhhmmss"), isSerious ? "SAEReport_Spontaneous" : "PatientSummary_Spontaneous");

            using (var document = WordprocessingDocument.Create(string.Format("{0}{1}", model.Path, model.FileName), WordprocessingDocumentType.Document))
            {
                // Add a main document part. 
                MainDocumentPart mainPart = document.AddMainDocumentPart();

                // Create the document structure and add some text.
                mainPart.Document = new Document();
                Body body = new Body();

                SectionProperties sectionProps = new SectionProperties();
                PageMargin pageMargin = new PageMargin() { Top = 404, Right = (UInt32Value)504U, Bottom = 404, Left = (UInt32Value)504U, Header = (UInt32Value)720U, Footer = (UInt32Value)720U, Gutter = (UInt32Value)0U };
                sectionProps.Append(pageMargin);
                body.Append(sectionProps);

                mainPart.Document.AppendChild(body);
            };

            using (var document = WordprocessingDocument.Open(string.Format("{0}{1}", model.Path, model.FileName), true))
            {
                var doc = document.MainDocumentPart.Document;
                var body = doc.Body;

                // Add page header
                body.Append(AddPatientSummaryPageHeader("PATIENT SUMMARY", document));

                var tableHeader = AddTableHeader("A. BASIC PATIENT INFORMATION");
                body.Append(tableHeader);
                var basicInformationtable = AddBasicInformationTable(datasetInstance);
                body.Append(basicInformationtable);
                var notesTable = AddNotesTable();
                body.Append(notesTable);

                tableHeader = AddTableHeader("B. ADVERSE EVENT INFORMATION");
                body.Append(tableHeader);
                var adverseEventtable = AddAdverseEventTable(datasetInstance, isSerious);
                body.Append(adverseEventtable);
                notesTable = AddNotesTable();
                body.Append(notesTable);

                tableHeader = AddTableHeader("C. MEDICATIONS");
                body.Append(tableHeader);
                var medicationtable = AddMedicationTable(datasetInstance);
                body.Append(medicationtable);
                notesTable = AddNotesForMedicationTable();
                body.Append(notesTable);

                tableHeader = AddTableHeader("D. CLINICAL EVALUATIONS");
                body.Append(tableHeader);
                var evaluationtable = AddEvaluationTable(datasetInstance);
                body.Append(evaluationtable);
                notesTable = AddNotesTable();
                body.Append(notesTable);

                document.Save();
            }

            return model;
        }

        public ArtefactInfoModel CreateSpontaneousDatasetForDownload()
        {
            var model = new ArtefactInfoModel();
            var generatedDate = DateTime.Now;

            model.Path = String.Format("{0}\\Temp\\", System.AppDomain.CurrentDomain.BaseDirectory);
            model.FileName = string.Format("Spontaneous_DataExtract_{0}.xlsx", generatedDate.ToString("yyyyMMddhhmmss"));

            using (var pck = new ExcelPackage(new FileInfo(model.FullPath)))
            {
                // *************************************
                // Create sheet - Main Spontaneous
                // *************************************
                var ws = pck.Workbook.Worksheets.Add("Spontaneous");
                ws.View.ShowGridLines = true;

                var rowCount = 1;
                var colCount = 2;
                var maxColCount = 1;

                List<string> columns = new List<string>();

                // Header
                ws.Cells["A1"].Value = "Unique Identifier";

                var dataset = _unitOfWork.Repository<Dataset>().Queryable().Single(ds => ds.DatasetName == "Spontaneous Report");
                foreach (DatasetCategory category in dataset.DatasetCategories)
                {
                    foreach (DatasetCategoryElement element in category.DatasetCategoryElements.Where(dce => dce.DatasetElement.System == false && dce.DatasetElement.Field.FieldType.Description != "Table"))
                    {
                        ws.Cells[GetExcelColumnName(colCount) + "1"].Value = element.DatasetElement.ElementName;
                        columns.Add(element.DatasetElement.ElementName);
                        colCount += 1;
                    }
                }
                maxColCount = colCount - 1;

                //Set the header and format it
                using (var r = ws.Cells["A1:" + GetExcelColumnName(maxColCount) + "1"])
                {
                    r.Style.Font.SetFromFont(new System.Drawing.Font("Arial", 10, FontStyle.Bold));
                    r.Style.Font.Color.SetColor(System.Drawing.Color.White);
                    r.Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;
                    r.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    r.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(23, 55, 93));
                }

                // Data
                foreach (ReportInstance reportInstance in _unitOfWork.Repository<ReportInstance>().Queryable().Include(ri => ri.WorkFlow).Include(ri => ri.Activities.Select(a => a.CurrentStatus)).Where(ri => ri.WorkFlow.WorkFlowGuid.ToString() == "4096D0A3-45F7-4702-BDA1-76AEDE41B986" && ri.Activities.Any(a => a.QualifiedName == "Verify Report Data" && a.CurrentStatus.Description != "DELETED")))
                {
                    DatasetInstance datasetInstance = _unitOfWork.Repository<DatasetInstance>().Queryable().Single(di => di.DatasetInstanceGuid == reportInstance.ContextGuid);

                    rowCount += 1;
                    ws.Cells["A" + rowCount].Value = datasetInstance.DatasetInstanceGuid.ToString();

                    foreach (var value in datasetInstance.DatasetInstanceValues.Where(div1 => div1.DatasetElement.System == false && div1.DatasetElement.Field.FieldType.Description != "Table").OrderBy(div2 => div2.Id))
                    {
                        colCount = columns.IndexOf(value.DatasetElement.ElementName) + 2;
                        ws.Cells[GetExcelColumnName(colCount) + rowCount].Value = value.InstanceValue;
                    };
                }

                //format row
                using (var r = ws.Cells["A1:" + GetExcelColumnName(maxColCount) + rowCount])
                {
                    r.Style.Font.SetFromFont(new System.Drawing.Font("Arial", 10, FontStyle.Regular));
                    r.Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    r.AutoFitColumns();
                }

                // *************************************
                // Create sheet - Sub tables
                // *************************************
                foreach (DatasetCategory category in dataset.DatasetCategories)
                {
                    foreach (DatasetCategoryElement element in category.DatasetCategoryElements.Where(dce => dce.DatasetElement.System == false && dce.DatasetElement.Field.FieldType.Description == "Table"))
                    {
                        ws = pck.Workbook.Worksheets.Add(element.DatasetElement.ElementName);
                        ws.View.ShowGridLines = true;

                        // Write headers
                        colCount = 2;
                        rowCount = 1;

                        ws.Cells["A1"].Value = "Unique Identifier";

                        foreach (var subElement in element.DatasetElement.DatasetElementSubs.Where(des1 => des1.System == false).OrderBy(des2 => des2.Id))
                        {
                            ws.Cells[GetExcelColumnName(colCount) + "1"].Value = subElement.ElementName;
                            colCount += 1;
                        }
                        maxColCount = colCount - 1;

                        //Set the header and format it
                        using (var r = ws.Cells["A1:" + GetExcelColumnName(maxColCount) + "1"])
                        {
                            r.Style.Font.SetFromFont(new System.Drawing.Font("Arial", 10, FontStyle.Bold));
                            r.Style.Font.Color.SetColor(System.Drawing.Color.White);
                            r.Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;
                            r.Style.Fill.PatternType = ExcelFillStyle.Solid;
                            r.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(23, 55, 93));
                        }

                        // Data
                        foreach (var value in _unitOfWork.Repository<DatasetInstanceValue>().Queryable().Include(divi => divi.DatasetInstance).Where(div1 => div1.DatasetElement.Id == element.DatasetElement.Id && div1.DatasetInstanceSubValues.Count > 0 && div1.DatasetInstance.Status == Core.ValueTypes.DatasetInstanceStatus.COMPLETE).OrderBy(div2 => div2.Id))
                        {
                            // Get report and ensure it is not deleted
                            ReportInstance reportInstance = _unitOfWork.Repository<ReportInstance>().Queryable().SingleOrDefault(ri => ri.ContextGuid == value.DatasetInstance.DatasetInstanceGuid);

                            if (reportInstance != null)
                            {
                                if (reportInstance.Activities.Any(a => a.QualifiedName == "Confirm Report Data" && a.CurrentStatus.Description != "DELETED"))
                                {
                                    // Get unique contexts
                                    var contexts = value.DatasetInstance.GetInstanceSubValuesContext(value.DatasetElement);
                                    foreach (var context in contexts)
                                    {
                                        rowCount += 1;
                                        ws.Cells["A" + rowCount].Value = value.DatasetInstance.DatasetInstanceGuid.ToString();

                                        foreach (var subvalue in value.DatasetInstance.GetInstanceSubValues(value.DatasetElement, context))
                                        {
                                            if (subvalue.DatasetElementSub.System == false)
                                            {
                                                colCount = value.DatasetElement.DatasetElementSubs.ToList().IndexOf(subvalue.DatasetElementSub) + 2;
                                                ws.Cells[GetExcelColumnName(colCount) + rowCount].Value = subvalue.InstanceValue;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        //format row
                        using (var r = ws.Cells["A1:" + GetExcelColumnName(maxColCount) + rowCount])
                        {
                            r.Style.Font.SetFromFont(new System.Drawing.Font("Arial", 10, FontStyle.Regular));
                            r.Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                            r.AutoFitColumns();
                        }
                    }
                }

                pck.Save();
            }

            return model;
        }

        #region "Word Processing"

        private Paragraph AddPatientSummaryPageHeader(string header, WordprocessingDocument document)
        {
            PrepareLogo(document);

            Paragraph paragraph = new Paragraph();
            ParagraphProperties pprop = new ParagraphProperties();
            Justification CenterHeading = new Justification() { Val = JustificationValues.Center };
            pprop.Append(CenterHeading);
            pprop.ParagraphStyleId = new ParagraphStyleId() { Val = "userheading" };
            paragraph.Append(pprop);

            RunProperties runProperties = new RunProperties();
            runProperties.AppendChild(new Bold());
            FontSize fs = new FontSize();
            fs.Val = "24";
            runProperties.AppendChild(fs);
            Run run = new Run();
            run.AppendChild(runProperties);
            run.AppendChild(new Text(header));
            paragraph.Append(run);

            return paragraph;
        }

        private Table AddTableHeader(string header)
        {
            UInt32Value rowHeight = 20;

            // Main header
            Table mainTable = new Table();

            TableProperties tprops = new TableProperties(
                new TableBorders(
                    new TopBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 5 },
                    new BottomBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 5 },
                    new LeftBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 5 },
                    new RightBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 5 },
                    new InsideHorizontalBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 5 },
                    new InsideVerticalBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 5 }
                    ),
                new TableGrid(
                    new GridColumn() { Width = "11352" })
                    );

            mainTable.AppendChild<TableProperties>(tprops);

            var tr = new TableRow();
            TableRowProperties rprops = new TableRowProperties(
                new TableRowHeight() { Val = rowHeight }
                );
            tr.AppendChild<TableRowProperties>(rprops);

            tr.Append(PrepareHeaderCell(header, 11352, false, true));
            mainTable.AppendChild<TableRow>(tr);

            return mainTable;
        }

        private Table AddBasicInformationTable(PatientClinicalEvent patientEvent)
        {
            IExtendable patientEventExtended = patientEvent;
            IExtendable patientExtended = patientEvent.Patient;

            var genderConfig = _unitOfWork.Repository<CustomAttributeConfiguration>().Queryable().SingleOrDefault(c => c.ExtendableTypeName == "Patient" && c.AttributeKey == "Gender");

            Table table = new Table();

            var headerWidth = 2500;
            var cellWidth = 3176;
            UInt32Value rowHeight = 12;

            TableProperties tprops = new TableProperties(
                new TableBorders(
                    new TopBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 5 },
                    new BottomBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 5 },
                    new LeftBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 5 },
                    new RightBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 5 },
                    new InsideHorizontalBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 5 },
                    new InsideVerticalBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 5 }
                    ),
                new TableGrid(
                    new GridColumn() { Width = headerWidth.ToString() },
                    new GridColumn() { Width = cellWidth.ToString() },
                    new GridColumn() { Width = headerWidth.ToString() },
                    new GridColumn() { Width = cellWidth.ToString() })
                    );

            table.AppendChild<TableProperties>(tprops);

            // Row 1
            var tr = new TableRow();
            TableRowProperties rprops = new TableRowProperties(
                new TableRowHeight() { Val = rowHeight }
                );
            tr.AppendChild<TableRowProperties>(rprops);

            tr.Append(PrepareHeaderCell("Patient Name", headerWidth));
            tr.Append(PrepareCell(patientEvent.Patient.FullName, cellWidth));
            tr.Append(PrepareHeaderCell("Date of Birth", headerWidth));
            tr.Append(PrepareCell(patientEvent.Patient.DateOfBirth != null ? Convert.ToDateTime(patientEvent.Patient.DateOfBirth).ToString("yyyy-MM-dd") : "", cellWidth));

            table.AppendChild<TableRow>(tr);

            // Row 2
            tr = new TableRow();
            rprops = new TableRowProperties(
                new TableRowHeight() { Val = rowHeight }
                );
            tr.AppendChild<TableRowProperties>(rprops);

            tr.Append(PrepareHeaderCell("Age Group", headerWidth));
            tr.Append(PrepareCell(patientEventExtended.GetAttributeValue("Age Group") != null ? patientEventExtended.GetAttributeValue("Age Group").ToString() : "", cellWidth));
            tr.Append(PrepareHeaderCell("Age at time of onset", headerWidth));
            if (patientEvent.OnsetDate != null && patientEvent.Patient.DateOfBirth != null)
            {
                tr.Append(PrepareCell(CalculateAge(Convert.ToDateTime(patientEvent.Patient.DateOfBirth), Convert.ToDateTime(patientEvent.OnsetDate)).ToString() + " years", cellWidth));
            }
            else
            {
                tr.Append(PrepareCell(string.Empty, cellWidth));
            }
            table.AppendChild<TableRow>(tr);

            // Row 3
            tr = new TableRow();
            rprops = new TableRowProperties(
                new TableRowHeight() { Val = rowHeight }
                );
            tr.AppendChild<TableRowProperties>(rprops);

            tr.Append(PrepareHeaderCell("Gender", headerWidth));
            tr.Append(PrepareCell(_attributeService.GetCustomAttributeValue(genderConfig, patientExtended), cellWidth));
            tr.Append(PrepareHeaderCell("Facility", headerWidth));
            tr.Append(PrepareCell(patientEvent.Patient.GetCurrentFacility().Facility.FacilityName, cellWidth));

            table.AppendChild<TableRow>(tr);

            // Row 4
            tr = new TableRow();
            rprops = new TableRowProperties(
                new TableRowHeight() { Val = rowHeight }
                );
            tr.AppendChild<TableRowProperties>(rprops);

            tr.Append(PrepareHeaderCell("Medical Record Number", headerWidth));
            tr.Append(PrepareCell(patientExtended.GetAttributeValue("Medical Record Number") != null ? patientExtended.GetAttributeValue("Medical Record Number").ToString() : "", cellWidth));
            tr.Append(PrepareHeaderCell("Identity Number", headerWidth));
            tr.Append(PrepareCell(patientExtended.GetAttributeValue("Patient Identity Number") != null ? patientExtended.GetAttributeValue("Patient Identity Number").ToString() : "", cellWidth));

            table.AppendChild<TableRow>(tr);

            // Row 5
            tr = new TableRow();
            rprops = new TableRowProperties(
                new TableRowHeight() { Val = rowHeight }
                );
            tr.AppendChild<TableRowProperties>(rprops);

            tr.Append(PrepareHeaderCell("Weight (kg)", headerWidth));
            tr.Append(PrepareCell("", cellWidth));
            tr.Append(PrepareHeaderCell("Height (cm)", headerWidth));
            tr.Append(PrepareCell("", cellWidth));

            table.AppendChild<TableRow>(tr);

            return table;
        }

        private Table AddConditionTable(PatientClinicalEvent patientEvent)
        {
            IExtendable patientEventExtended = patientEvent;
            IExtendable patientExtended = patientEvent.Patient;

            Table table = new Table();

            var headerWidth = 2500;
            var cellWidth = 8852;
            UInt32Value rowHeight = 12;

            TableProperties tprops = new TableProperties(
                new TableBorders(
                    new TopBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 5 },
                    new BottomBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 5 },
                    new LeftBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 5 },
                    new RightBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 5 },
                    new InsideHorizontalBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 5 },
                    new InsideVerticalBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 5 }
                    ),
                new TableGrid(
                    new GridColumn() { Width = headerWidth.ToString() },
                    new GridColumn() { Width = cellWidth.ToString() }
                    ));

            table.AppendChild<TableProperties>(tprops);

            // Conditions
            var i = 0;
            foreach (PatientCondition pc in patientEvent.Patient.PatientConditions.Where(pc => pc.DateStart <= patientEvent.OnsetDate).OrderByDescending(c => c.DateStart))
            {
                i += 1;
                // Row 1
                var tr = new TableRow();
                TableRowProperties rprops = new TableRowProperties(
                    new TableRowHeight() { Val = rowHeight }
                    );
                tr.AppendChild<TableRowProperties>(rprops);

                tr.Append(PrepareHeaderCell("Condition " + i.ToString(), headerWidth));
                tr.Append(PrepareCell(pc.TerminologyMedDra.MedDraTerm, cellWidth, false));

                table.AppendChild<TableRow>(tr);

                // Row 2
                tr = new TableRow();
                rprops = new TableRowProperties(
                    new TableRowHeight() { Val = rowHeight }
                    );
                tr.AppendChild<TableRowProperties>(rprops);

                tr.Append(PrepareHeaderCell("Start Date", headerWidth));
                tr.Append(PrepareCell(pc.DateStart.ToString("yyyy-MM-dd"), cellWidth, false));

                table.AppendChild<TableRow>(tr);

                var endDate = pc.OutcomeDate != null ? Convert.ToDateTime(pc.OutcomeDate).ToString("yyyy-MM-dd") : "";
                // Row 3
                tr = new TableRow();
                rprops = new TableRowProperties(
                    new TableRowHeight() { Val = rowHeight }
                    );
                tr.AppendChild<TableRowProperties>(rprops);

                tr.Append(PrepareHeaderCell("End Date", headerWidth));
                tr.Append(PrepareCell(endDate, cellWidth, false));

                table.AppendChild<TableRow>(tr);
            }

            return table;
        }

        private Table AddAdverseEventTable(PatientClinicalEvent patientEvent, bool isSerious)
        {
            IExtendable patientEventExtended = patientEvent;
            IExtendable patientExtended = patientEvent.Patient;

            var outcomeConfig = _unitOfWork.Repository<CustomAttributeConfiguration>().Queryable().SingleOrDefault(c => c.ExtendableTypeName == "PatientClinicalEvent" && c.AttributeKey == "Outcome");
            var seriousnessConfig = _unitOfWork.Repository<CustomAttributeConfiguration>().Queryable().SingleOrDefault(c => c.ExtendableTypeName == "PatientClinicalEvent" && c.AttributeKey == "Seriousness");
            var scaleConfig = _unitOfWork.Repository<CustomAttributeConfiguration>().Queryable().SingleOrDefault(c => c.ExtendableTypeName == "PatientClinicalEvent" && c.AttributeKey == "Severity Grading Scale");
            var severityConfig = _unitOfWork.Repository<CustomAttributeConfiguration>().Queryable().SingleOrDefault(c => c.ExtendableTypeName == "PatientClinicalEvent" && c.AttributeKey == "Severity Grade");
            var saeNumberConfig = _unitOfWork.Repository<CustomAttributeConfiguration>().Queryable().SingleOrDefault(c => c.ExtendableTypeName == "PatientClinicalEvent" && c.AttributeKey == "FDA SAE Number");

            Table table = new Table();

            var headerWidth = 2500;
            var cellWidth = 3176;
            UInt32Value rowHeight = 12;

            TableProperties tprops = new TableProperties(
                new TableBorders(
                    new TopBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 5 },
                    new BottomBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 5 },
                    new LeftBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 5 },
                    new RightBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 5 },
                    new InsideHorizontalBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 5 },
                    new InsideVerticalBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 5 }
                    ),
                new TableGrid(
                    new GridColumn() { Width = headerWidth.ToString() },
                    new GridColumn() { Width = cellWidth.ToString() },
                    new GridColumn() { Width = headerWidth.ToString() },
                    new GridColumn() { Width = cellWidth.ToString() })
                    );

            table.AppendChild<TableProperties>(tprops);

            // Row 1
            var tr = new TableRow();
            TableRowProperties rprops = new TableRowProperties(
                new TableRowHeight() { Val = rowHeight }
                );
            tr.AppendChild<TableRowProperties>(rprops);

            tr.Append(PrepareHeaderCell("Source Description", headerWidth));
            tr.Append(PrepareCell(patientEvent.SourceDescription, cellWidth));
            tr.Append(PrepareHeaderCell("MedDRA Term", headerWidth));
            tr.Append(PrepareCell(patientEvent.SourceTerminologyMedDra.MedDraTerm, cellWidth));

            table.AppendChild<TableRow>(tr);

            // Row 2
            var onsetDate = patientEvent.OnsetDate != null ? Convert.ToDateTime(patientEvent.OnsetDate).ToString("yyyy-MM-dd") : "";
            var resDate = patientEvent.ResolutionDate != null ? Convert.ToDateTime(patientEvent.ResolutionDate).ToString("yyyy-MM-dd") : "";
            tr = new TableRow();
            rprops = new TableRowProperties(
                new TableRowHeight() { Val = rowHeight }
                );
            tr.AppendChild<TableRowProperties>(rprops);

            tr.Append(PrepareHeaderCell("Onset Date", headerWidth));
            tr.Append(PrepareCell(onsetDate, cellWidth));
            tr.Append(PrepareHeaderCell("Resolution Date", headerWidth));
            tr.Append(PrepareCell(resDate, cellWidth));

            table.AppendChild<TableRow>(tr);

            // Row 3
            tr = new TableRow();
            rprops = new TableRowProperties(
                new TableRowHeight() { Val = rowHeight }
                );
            tr.AppendChild<TableRowProperties>(rprops);

            tr.Append(PrepareHeaderCell("Duration", headerWidth));
            if (!String.IsNullOrWhiteSpace(onsetDate) && !String.IsNullOrWhiteSpace(resDate))
            {
                var rduration = (Convert.ToDateTime(resDate) - Convert.ToDateTime(onsetDate)).Days;
                tr.Append(PrepareCell(rduration.ToString() + " days", cellWidth));
            }
            else
            {
                tr.Append(PrepareCell(string.Empty, cellWidth));
            }
            tr.Append(PrepareHeaderCell("Outcome", headerWidth));
            tr.Append(PrepareCell(_attributeService.GetCustomAttributeValue(outcomeConfig, patientEventExtended), cellWidth));

            table.AppendChild<TableRow>(tr);

            // Cater for seriousness fields
            if (isSerious)
            {
                // Row 4
                tr = new TableRow();
                rprops = new TableRowProperties(
                    new TableRowHeight() { Val = rowHeight }
                    );
                tr.AppendChild<TableRowProperties>(rprops);

                tr.Append(PrepareHeaderCell("Seriousness", headerWidth));
                tr.Append(PrepareCell(_attributeService.GetCustomAttributeValue(seriousnessConfig, patientEventExtended), cellWidth));
                tr.Append(PrepareHeaderCell("Grading Scale", headerWidth));
                tr.Append(PrepareCell(_attributeService.GetCustomAttributeValue(scaleConfig, patientEventExtended), cellWidth));

                table.AppendChild<TableRow>(tr);

                // Row 5
                tr = new TableRow();
                rprops = new TableRowProperties(
                    new TableRowHeight() { Val = rowHeight }
                    );
                tr.AppendChild<TableRowProperties>(rprops);

                tr.Append(PrepareHeaderCell("Severity Grade", headerWidth));
                tr.Append(PrepareCell(_attributeService.GetCustomAttributeValue(severityConfig, patientEventExtended), cellWidth));
                tr.Append(PrepareHeaderCell("SAE Number", headerWidth));
                tr.Append(PrepareCell(_attributeService.GetCustomAttributeValue(saeNumberConfig, patientEventExtended), cellWidth));

                table.AppendChild<TableRow>(tr);
            }

            return table;
        }

        private Table AddMedicationTable(PatientClinicalEvent patientEvent)
        {
            IExtendable patientEventExtended = patientEvent;
            IExtendable patientExtended = patientEvent.Patient;

            var reportInstance = _unitOfWork.Repository<ReportInstance>().Queryable().Single(ri => ri.ContextGuid == patientEvent.PatientClinicalEventGuid);

            var routeConfig = _unitOfWork.Repository<CustomAttributeConfiguration>().Queryable().SingleOrDefault(c => c.ExtendableTypeName == "PatientMedication" && c.AttributeKey == "Route");
            var indicationConfig = _unitOfWork.Repository<CustomAttributeConfiguration>().Queryable().SingleOrDefault(c => c.ExtendableTypeName == "PatientMedication" && c.AttributeKey == "Indication");

            Table table = new Table();

            UInt32Value rowHeight = 12;

            TableProperties tprops = new TableProperties(
                new TableBorders(
                    new TopBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 5 },
                    new BottomBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 5 },
                    new LeftBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 5 },
                    new RightBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 5 },
                    new InsideHorizontalBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 5 },
                    new InsideVerticalBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 5 }
                    ),
                new TableGrid(
                    new GridColumn() { Width = "2500" },
                    new GridColumn() { Width = "1250" },
                    new GridColumn() { Width = "1250" },
                    new GridColumn() { Width = "1250" },
                    new GridColumn() { Width = "1250" },
                    new GridColumn() { Width = "3852" }
                    ));

            table.AppendChild<TableProperties>(tprops);

            var i = 0;
            foreach (ReportInstanceMedication med in reportInstance.Medications)
            {
                i += 1;

                var patientMedication = _unitOfWork.Repository<PatientMedication>().Queryable().Single(pm => pm.PatientMedicationGuid == med.ReportInstanceMedicationGuid);
                IExtendable mcExtended = patientMedication;

                // Row 1
                var endDate = patientMedication.DateEnd != null ? Convert.ToDateTime(patientMedication.DateEnd).ToString("yyyy-MM-dd") : "";
                var tr = new TableRow();
                TableRowProperties rprops = new TableRowProperties(
                    new TableRowHeight() { Val = rowHeight }
                    );
                tr.AppendChild<TableRowProperties>(rprops);

                tr.Append(PrepareHeaderCell("Drug " + i.ToString(), 2500));
                tr.Append(PrepareHeaderCell("Start Date ", 1250, true, false));
                tr.Append(PrepareHeaderCell("End Date ", 1250, true, false));
                tr.Append(PrepareHeaderCell("Dose ", 1250, true, false));
                tr.Append(PrepareHeaderCell("Route ", 1250, true, false));
                tr.Append(PrepareHeaderCell("Indication ", 3852));

                table.AppendChild<TableRow>(tr);

                // Row 2
                tr = new TableRow();
                rprops = new TableRowProperties(
                    new TableRowHeight() { Val = rowHeight }
                    );
                tr.AppendChild<TableRowProperties>(rprops);

                tr.Append(PrepareCell(patientMedication.Medication.DrugName, 2500, false));
                tr.Append(PrepareCell(patientMedication.DateStart.ToString("yyyy-MM-dd"), 1250));
                tr.Append(PrepareCell(endDate, 1250));
                tr.Append(PrepareCell(patientMedication.Dose, 1250));
                tr.Append(PrepareCell(_attributeService.GetCustomAttributeValue(routeConfig, mcExtended), 1250));
                tr.Append(PrepareCell(_attributeService.GetCustomAttributeValue(indicationConfig, mcExtended), 3852, false));

                table.AppendChild<TableRow>(tr);

                // Row 3
                tr = new TableRow();
                rprops = new TableRowProperties(
                    new HorizontalMerge { Val = MergedCellValues.Continue },
                    new TableRowHeight() { Val = rowHeight }
                    );
                tr.AppendChild<TableRowProperties>(rprops);

                tr.Append(PrepareHeaderCell("C.1 CLINICIAN ACTION TAKEN WITH REGARD TO MEDICINE", 11352, false, false, 6));

                table.AppendChild<TableRow>(tr);

                // Row 4
                tr = new TableRow();
                rprops = new TableRowProperties(
                    new HorizontalMerge { Val = MergedCellValues.Continue },
                    new TableRowHeight() { Val = rowHeight }
                    );
                tr.AppendChild<TableRowProperties>(rprops);

                tr.Append(PrepareCell("", 11352, false, 6));

                table.AppendChild<TableRow>(tr);

                // Row 5
                tr = new TableRow();
                rprops = new TableRowProperties(
                    new HorizontalMerge { Val = MergedCellValues.Continue },
                    new TableRowHeight() { Val = rowHeight }
                    );
                tr.AppendChild<TableRowProperties>(rprops);

                tr.Append(PrepareHeaderCell("C.2 EFFECT OF DECHALLENGE/ RECHALLENGE", 11352, false, false, 6));

                table.AppendChild<TableRow>(tr);

                // Row 6
                tr = new TableRow();
                rprops = new TableRowProperties(
                    new HorizontalMerge { Val = MergedCellValues.Continue },
                    new TableRowHeight() { Val = rowHeight }
                    );
                tr.AppendChild<TableRowProperties>(rprops);

                tr.Append(PrepareCell("", 11352, false, 6));

                table.AppendChild<TableRow>(tr);
            }

            return table;
        }

        private Table AddEvaluationTable(PatientClinicalEvent patientEvent)
        {
            IExtendable patientEventExtended = patientEvent;
            IExtendable patientExtended = patientEvent.Patient;

            Table table = new Table();

            UInt32Value rowHeight = 12;

            TableProperties tprops = new TableProperties(
                new TableBorders(
                    new TopBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 5 },
                    new BottomBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 5 },
                    new LeftBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 5 },
                    new RightBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 5 },
                    new InsideHorizontalBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 5 },
                    new InsideVerticalBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 5 }
                    ),
                new TableGrid(
                    new GridColumn() { Width = "2500" },
                    new GridColumn() { Width = "2500" },
                    new GridColumn() { Width = "6352" }
                    ));

            table.AppendChild<TableProperties>(tprops);

            // Row 1
            var tr = new TableRow();
            TableRowProperties rprops = new TableRowProperties(
                new TableRowHeight() { Val = rowHeight }
                );
            tr.AppendChild<TableRowProperties>(rprops);

            tr.Append(PrepareHeaderCell("Test", 2500));
            tr.Append(PrepareHeaderCell("Test Date", 2500, true, false));
            tr.Append(PrepareHeaderCell("Test Result", 6352));

            table.AppendChild<TableRow>(tr);

            foreach (PatientLabTest labTest in patientEvent.Patient.PatientLabTests.Where(lt => lt.TestDate >= patientEvent.OnsetDate).OrderByDescending(lt => lt.TestDate))
            {
                // Row 2
                tr = new TableRow();
                rprops = new TableRowProperties(
                    new TableRowHeight() { Val = rowHeight }
                    );
                tr.AppendChild<TableRowProperties>(rprops);

                tr.Append(PrepareCell(labTest.LabTest.Description, 2500, false));
                tr.Append(PrepareCell(labTest.TestDate.ToString("yyyy-MM-dd"), 2500));
                tr.Append(PrepareCell(labTest.TestResult, 6352, false));

                table.AppendChild<TableRow>(tr);
            }

            return table;
        }

        private Table AddWeightTable(PatientClinicalEvent patientEvent)
        {
            IExtendable patientEventExtended = patientEvent;
            IExtendable patientExtended = patientEvent.Patient;

            Table table = new Table();

            UInt32Value rowHeight = 12;

            TableProperties tprops = new TableProperties(
                new TableBorders(
                    new TopBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 5 },
                    new BottomBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 5 },
                    new LeftBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 5 },
                    new RightBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 5 },
                    new InsideHorizontalBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 5 },
                    new InsideVerticalBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 5 }
                    ),
                new TableGrid(
                    new GridColumn() { Width = "2500" },
                    new GridColumn() { Width = "8852" }
                    ));

            table.AppendChild<TableProperties>(tprops);

            // Row 1
            var tr = new TableRow();
            TableRowProperties rprops = new TableRowProperties(
                new TableRowHeight() { Val = rowHeight }
                );
            tr.AppendChild<TableRowProperties>(rprops);

            tr.Append(PrepareHeaderCell("Weight Date", 2500));
            tr.Append(PrepareHeaderCell("Weight", 8852, true, false));

            table.AppendChild<TableRow>(tr);

            var weightModel = _patientService.GetElementValuesForPatient(patientEvent.Patient.Id, "Chronic Treatment", "Weight(kg)", 10);

            foreach (var weight in weightModel.Values.Where(v => v.Value != "NO VALUE"))
            {
                // Row 2
                tr = new TableRow();
                rprops = new TableRowProperties(
                    new TableRowHeight() { Val = rowHeight }
                    );
                tr.AppendChild<TableRowProperties>(rprops);

                tr.Append(PrepareCell(weight.ValueDate.ToString("yyyy-MM-dd"), 2500, false));
                tr.Append(PrepareCell(weight.Value, 6352));

                table.AppendChild<TableRow>(tr);
            }

            return table;
        }

        private Table AddBasicInformationTable(DatasetInstance datasetInstance)
        {
            Table table = new Table();
            DateTime tempdt;

            var temp = datasetInstance.GetInstanceValue(_unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.DatasetElementGuid.ToString() == "0D704069-5C50-4085-8FE1-355BB64EF196"));
            DateTime dob = DateTime.TryParse(temp, out tempdt) ? tempdt : DateTime.MinValue;

            temp = datasetInstance.GetInstanceValue(_unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.DatasetElementGuid.ToString() == "D314C438-5ABA-4ED2-855D-1A5B22B5A301"));
            string age = temp;
            temp = datasetInstance.GetInstanceValue(_unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.DatasetElementGuid.ToString() == "80C219DC-238C-487E-A3D5-8919ABA674B1"));
            age += " " + temp; // Include age unit

            var headerWidth = 2500;
            var cellWidth = 3176;
            UInt32Value rowHeight = 12;

            TableProperties tprops = new TableProperties(
                new TableBorders(
                    new TopBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 5 },
                    new BottomBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 5 },
                    new LeftBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 5 },
                    new RightBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 5 },
                    new InsideHorizontalBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 5 },
                    new InsideVerticalBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 5 }
                    ),
                new TableGrid(
                    new GridColumn() { Width = headerWidth.ToString() },
                    new GridColumn() { Width = cellWidth.ToString() },
                    new GridColumn() { Width = headerWidth.ToString() },
                    new GridColumn() { Width = cellWidth.ToString() })
                    );

            table.AppendChild<TableProperties>(tprops);

            // Row 1
            var tr = new TableRow();
            TableRowProperties rprops = new TableRowProperties(
                new TableRowHeight() { Val = rowHeight }
                );
            tr.AppendChild<TableRowProperties>(rprops);

            tr.Append(PrepareHeaderCell("Patient Name", headerWidth));
            tr.Append(PrepareCell(datasetInstance.GetInstanceValue(_unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.DatasetElementGuid.ToString() == "29CD2157-8FB6-4883-A4E6-A4B9EDE6B36B")), cellWidth));
            tr.Append(PrepareHeaderCell("Date of Birth", headerWidth));
            tr.Append(PrepareCell(dob == DateTime.MinValue ? "" : dob.ToString("yyyy-MM-dd"), cellWidth));

            table.AppendChild<TableRow>(tr);

            // Row 2
            tr = new TableRow();
            rprops = new TableRowProperties(
                new TableRowHeight() { Val = rowHeight }
                );
            tr.AppendChild<TableRowProperties>(rprops);

            tr.Append(PrepareHeaderCell("Age Group", headerWidth));
            tr.Append(PrepareCell("", cellWidth));
            tr.Append(PrepareHeaderCell("Age at time of onset", headerWidth));
            tr.Append(PrepareCell(age, cellWidth));

            table.AppendChild<TableRow>(tr);

            // Row 3
            tr = new TableRow();
            rprops = new TableRowProperties(
                new TableRowHeight() { Val = rowHeight }
                );
            tr.AppendChild<TableRowProperties>(rprops);

            tr.Append(PrepareHeaderCell("Gender", headerWidth));
            tr.Append(PrepareCell(datasetInstance.GetInstanceValue(_unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.DatasetElementGuid.ToString() == "E061D363-534E-4EA4-B6E5-F1C531931B12")), cellWidth));
            tr.Append(PrepareHeaderCell("Facility", headerWidth));
            tr.Append(PrepareCell("", cellWidth));

            table.AppendChild<TableRow>(tr);

            // Row 4
            tr = new TableRow();
            rprops = new TableRowProperties(
                new TableRowHeight() { Val = rowHeight }
                );
            tr.AppendChild<TableRowProperties>(rprops);

            tr.Append(PrepareHeaderCell("Medical Record Number", headerWidth));
            tr.Append(PrepareCell("", cellWidth));
            tr.Append(PrepareHeaderCell("Identity Number", headerWidth));
            tr.Append(PrepareCell(datasetInstance.GetInstanceValue(_unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.DatasetElementGuid.ToString() == "5A2E89A9-8240-4665-967D-0C655CF281B7")), cellWidth));

            table.AppendChild<TableRow>(tr);

            // Row 5
            tr = new TableRow();
            rprops = new TableRowProperties(
                new TableRowHeight() { Val = rowHeight }
                );
            tr.AppendChild<TableRowProperties>(rprops);

            tr.Append(PrepareHeaderCell("Weight (kg)", headerWidth));
            tr.Append(PrepareCell(datasetInstance.GetInstanceValue(_unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.DatasetElementGuid.ToString() == "985BD25D-54E7-4A24-8636-6DBC0F9C7B96")), cellWidth));
            tr.Append(PrepareHeaderCell("Height (cm)", headerWidth));
            tr.Append(PrepareCell("", cellWidth));

            table.AppendChild<TableRow>(tr);

            return table;
        }

        private Table AddAdverseEventTable(DatasetInstance datasetInstance, bool isSerious)
        {
            Table table = new Table();
            DateTime tempdt;

            var reportInstance = _unitOfWork.Repository<ReportInstance>().Queryable().Single(ri => ri.ContextGuid == datasetInstance.DatasetInstanceGuid);

            var headerWidth = 2500;
            var cellWidth = 3176;
            UInt32Value rowHeight = 12;

            TableProperties tprops = new TableProperties(
                new TableBorders(
                    new TopBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 5 },
                    new BottomBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 5 },
                    new LeftBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 5 },
                    new RightBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 5 },
                    new InsideHorizontalBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 5 },
                    new InsideVerticalBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 5 }
                    ),
                new TableGrid(
                    new GridColumn() { Width = headerWidth.ToString() },
                    new GridColumn() { Width = cellWidth.ToString() },
                    new GridColumn() { Width = headerWidth.ToString() },
                    new GridColumn() { Width = cellWidth.ToString() })
                    );

            table.AppendChild<TableProperties>(tprops);

            // Row 1
            var tr = new TableRow();
            TableRowProperties rprops = new TableRowProperties(
                new TableRowHeight() { Val = rowHeight }
                );
            tr.AppendChild<TableRowProperties>(rprops);

            tr.Append(PrepareHeaderCell("Source Description", headerWidth));
            tr.Append(PrepareCell(datasetInstance.GetInstanceValue(_unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.DatasetElementGuid.ToString() == "ACD938A4-76D1-44CE-A070-2B8DF0FE9E0F")), cellWidth));
            tr.Append(PrepareHeaderCell("MedDRA Term", headerWidth));
            tr.Append(PrepareCell(reportInstance.TerminologyMedDra != null ? reportInstance.TerminologyMedDra.MedDraTerm : string.Empty, cellWidth));

            table.AppendChild<TableRow>(tr);

            // Row 2
            var temp = datasetInstance.GetInstanceValue(_unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.DatasetElementGuid.ToString() == "F5EEB382-D4A5-41A1-A447-37D5ECA50B99"));
            DateTime onsetDate = DateTime.TryParse(temp, out tempdt) ? tempdt : DateTime.MinValue;

            temp = datasetInstance.GetInstanceValue(_unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.DatasetElementGuid.ToString() == "F977C2F8-C7DD-4AFE-BCAA-1C06BD54D155"));
            DateTime recoveryDate = DateTime.TryParse(temp, out tempdt) ? tempdt : DateTime.MinValue;

            tr = new TableRow();
            rprops = new TableRowProperties(
                new TableRowHeight() { Val = rowHeight }
                );
            tr.AppendChild<TableRowProperties>(rprops);

            tr.Append(PrepareHeaderCell("Onset Date", headerWidth));
            tr.Append(PrepareCell(onsetDate == DateTime.MinValue ? "" : onsetDate.ToString("yyyy-MM-dd"), cellWidth));
            tr.Append(PrepareHeaderCell("Resolution Date", headerWidth));
            tr.Append(PrepareCell(recoveryDate == DateTime.MinValue ? "" : recoveryDate.ToString("yyyy-MM-dd"), cellWidth));

            table.AppendChild<TableRow>(tr);

            // Row 3
            tr = new TableRow();
            rprops = new TableRowProperties(
                new TableRowHeight() { Val = rowHeight }
                );
            tr.AppendChild<TableRowProperties>(rprops);

            tr.Append(PrepareHeaderCell("Duration", headerWidth));
            if (onsetDate != DateTime.MinValue && recoveryDate != DateTime.MinValue)
            {
                var rduration = (onsetDate - recoveryDate).Days;
                tr.Append(PrepareCell(rduration.ToString() + " days", cellWidth));
            }
            else
            {
                tr.Append(PrepareCell(string.Empty, cellWidth));
            }
            tr.Append(PrepareHeaderCell("Outcome", headerWidth));
            tr.Append(PrepareCell(datasetInstance.GetInstanceValue(_unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.DatasetElementGuid.ToString() == "976F6C53-78F2-4007-8F39-54057E554EEB")), cellWidth));

            table.AppendChild<TableRow>(tr);

            // Cater for seriousness fields
            if (isSerious)
            {
                // Row 4
                tr = new TableRow();
                rprops = new TableRowProperties(
                    new TableRowHeight() { Val = rowHeight }
                    );
                tr.AppendChild<TableRowProperties>(rprops);

                tr.Append(PrepareHeaderCell("Seriousness", headerWidth));
                tr.Append(PrepareCell(datasetInstance.GetInstanceValue(_unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.DatasetElementGuid.ToString() == "302C07C9-B0E0-46AB-9EF8-5D5C2F756BF1")), cellWidth));
                tr.Append(PrepareHeaderCell("Grading Scale", headerWidth));
                tr.Append(PrepareCell("", cellWidth));

                table.AppendChild<TableRow>(tr);

                // Row 5
                tr = new TableRow();
                rprops = new TableRowProperties(
                    new TableRowHeight() { Val = rowHeight }
                    );
                tr.AppendChild<TableRowProperties>(rprops);

                tr.Append(PrepareHeaderCell("Severity Grade", headerWidth));
                tr.Append(PrepareCell("", cellWidth));
                tr.Append(PrepareHeaderCell("SAE Number", headerWidth));
                tr.Append(PrepareCell("", cellWidth));

                table.AppendChild<TableRow>(tr);
            }

            return table;
        }

        private Table AddMedicationTable(DatasetInstance datasetInstance)
        {
            Table table = new Table();

            var reportInstance = _unitOfWork.Repository<ReportInstance>().Queryable().Single(ri => ri.ContextGuid == datasetInstance.DatasetInstanceGuid);

            UInt32Value rowHeight = 12;

            TableProperties tprops = new TableProperties(
                new TableBorders(
                    new TopBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 5 },
                    new BottomBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 5 },
                    new LeftBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 5 },
                    new RightBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 5 },
                    new InsideHorizontalBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 5 },
                    new InsideVerticalBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 5 }
                    ),
                new TableGrid(
                    new GridColumn() { Width = "2500" },
                    new GridColumn() { Width = "1250" },
                    new GridColumn() { Width = "1250" },
                    new GridColumn() { Width = "1250" },
                    new GridColumn() { Width = "1250" },
                    new GridColumn() { Width = "3852" }
                    ));

            table.AppendChild<TableProperties>(tprops);

            var sourceProductElement = _unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.DatasetElementGuid.ToString() == "712CA632-0CD0-4418-9176-FB0B95AEE8A1");
            var sourceContexts = datasetInstance.GetInstanceSubValuesContext(sourceProductElement);

            var i = 0;
            foreach (ReportInstanceMedication med in reportInstance.Medications)
            {
                i += 1;

                var drugItemValues = datasetInstance.GetInstanceSubValues(sourceProductElement, med.ReportInstanceMedicationGuid);

                var startValue = drugItemValues.SingleOrDefault(div => div.DatasetElementSub.ElementName == "Product start date");
                var endValue = drugItemValues.SingleOrDefault(div => div.DatasetElementSub.ElementName == "Product end date");
                var dose = drugItemValues.SingleOrDefault(div => div.DatasetElementSub.ElementName == "Dose number");
                var route = drugItemValues.SingleOrDefault(div => div.DatasetElementSub.ElementName == "Product route of administration");
                var indication = drugItemValues.SingleOrDefault(div => div.DatasetElementSub.ElementName == "Product indication");

                // Row 1
                var tr = new TableRow();
                TableRowProperties rprops = new TableRowProperties(
                    new TableRowHeight() { Val = rowHeight }
                    );
                tr.AppendChild<TableRowProperties>(rprops);

                tr.Append(PrepareHeaderCell("Drug " + i.ToString(), 2500));
                tr.Append(PrepareHeaderCell("Start Date ", 1250, true, false));
                tr.Append(PrepareHeaderCell("End Date ", 1250, true, false));
                tr.Append(PrepareHeaderCell("Dose ", 1250, true, false));
                tr.Append(PrepareHeaderCell("Route ", 1250, true, false));
                tr.Append(PrepareHeaderCell("Indication ", 3852));

                table.AppendChild<TableRow>(tr);

                // Row 2
                tr = new TableRow();
                rprops = new TableRowProperties(
                    new TableRowHeight() { Val = rowHeight }
                    );
                tr.AppendChild<TableRowProperties>(rprops);

                tr.Append(PrepareCell(drugItemValues.SingleOrDefault(div => div.DatasetElementSub.ElementName == "Product").InstanceValue, 2500, false));
                tr.Append(PrepareCell(startValue != null ? startValue.InstanceValue : string.Empty, 1250));
                tr.Append(PrepareCell(endValue != null ? endValue.InstanceValue : string.Empty, 1250));
                tr.Append(PrepareCell(dose != null ? dose.InstanceValue : string.Empty, 1250));
                tr.Append(PrepareCell(route != null ? route.InstanceValue : string.Empty, 1250));
                tr.Append(PrepareCell(indication != null ? indication.InstanceValue : string.Empty, 3852, false));

                table.AppendChild<TableRow>(tr);

                // Row 3
                tr = new TableRow();
                rprops = new TableRowProperties(
                    new HorizontalMerge { Val = MergedCellValues.Continue },
                    new TableRowHeight() { Val = rowHeight }
                    );
                tr.AppendChild<TableRowProperties>(rprops);

                tr.Append(PrepareHeaderCell("C.1 CLINICIAN ACTION TAKEN WITH REGARD TO MEDICINE", 11352, false, false, 6));

                table.AppendChild<TableRow>(tr);

                // Row 4
                tr = new TableRow();
                rprops = new TableRowProperties(
                    new HorizontalMerge { Val = MergedCellValues.Continue },
                    new TableRowHeight() { Val = rowHeight }
                    );
                tr.AppendChild<TableRowProperties>(rprops);

                tr.Append(PrepareCell("", 11352, false, 5));

                table.AppendChild<TableRow>(tr);

                // Row 5
                tr = new TableRow();
                rprops = new TableRowProperties(
                    new HorizontalMerge { Val = MergedCellValues.Continue },
                    new TableRowHeight() { Val = rowHeight }
                    );
                tr.AppendChild<TableRowProperties>(rprops);

                tr.Append(PrepareHeaderCell("C.2 EFFECT OF DECHALLENGE/ RECHALLENGE", 11352, false, false, 6));

                table.AppendChild<TableRow>(tr);

                // Row 6
                tr = new TableRow();
                rprops = new TableRowProperties(
                    new HorizontalMerge { Val = MergedCellValues.Continue },
                    new TableRowHeight() { Val = rowHeight }
                    );
                tr.AppendChild<TableRowProperties>(rprops);

                tr.Append(PrepareCell("", 11352, false, 5));

                table.AppendChild<TableRow>(tr);
            }

            return table;
        }

        private Table AddEvaluationTable(DatasetInstance datasetInstance)
        {

            Table table = new Table();

            UInt32Value rowHeight = 12;

            TableProperties tprops = new TableProperties(
                new TableBorders(
                    new TopBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 5 },
                    new BottomBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 5 },
                    new LeftBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 5 },
                    new RightBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 5 },
                    new InsideHorizontalBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 5 },
                    new InsideVerticalBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 5 }
                    ),
                new TableGrid(
                    new GridColumn() { Width = "2500" },
                    new GridColumn() { Width = "2500" },
                    new GridColumn() { Width = "6352" }
                    ));

            table.AppendChild<TableProperties>(tprops);

            // Row 1
            var tr = new TableRow();
            TableRowProperties rprops = new TableRowProperties(
                new TableRowHeight() { Val = rowHeight }
                );
            tr.AppendChild<TableRowProperties>(rprops);

            tr.Append(PrepareHeaderCell("Test", 2500));
            tr.Append(PrepareHeaderCell("Test Date", 2500, true, false));
            tr.Append(PrepareHeaderCell("Test Result", 6352));

            table.AppendChild<TableRow>(tr);

            var sourceLabElement = _unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.DatasetElementGuid.ToString() == "12D7089D-1603-4309-99DE-60F20F9A005E");
            var sourceContexts = datasetInstance.GetInstanceSubValuesContext(sourceLabElement);

            foreach (Guid sourceContext in sourceContexts)
            {
                var labItemValues = datasetInstance.GetInstanceSubValues(sourceLabElement, sourceContext);

                var testDate = labItemValues.SingleOrDefault(div => div.DatasetElementSub.ElementName == "Test Date");
                var testResult = labItemValues.SingleOrDefault(div => div.DatasetElementSub.ElementName == "Test Result");

                // Row 2
                tr = new TableRow();
                rprops = new TableRowProperties(
                    new TableRowHeight() { Val = rowHeight }
                    );
                tr.AppendChild<TableRowProperties>(rprops);

                tr.Append(PrepareCell(labItemValues.SingleOrDefault(div => div.DatasetElementSub.ElementName == "Test Name").InstanceValue, 2500, false));
                tr.Append(PrepareCell(testDate != null ? testDate.InstanceValue : string.Empty, 2500));
                tr.Append(PrepareCell(testResult != null ? testResult.InstanceValue : string.Empty, 6352, false));

                table.AppendChild<TableRow>(tr);
            }

            return table;
        }

        private Table AddNotesTable()
        {
            Table table = new Table();

            var headerWidth = 2500;
            var cellWidth = 8852;
            UInt32Value rowHeight = 24;

            TableProperties tprops = new TableProperties(
                new TableBorders(
                    new TopBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 5 },
                    new BottomBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 5 },
                    new LeftBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 5 },
                    new RightBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 5 },
                    new InsideHorizontalBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 5 },
                    new InsideVerticalBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 5 }
                    ),
                new TableGrid(
                    new GridColumn() { Width = headerWidth.ToString() },
                    new GridColumn() { Width = cellWidth.ToString() }
                    ));

            table.AppendChild<TableProperties>(tprops);

            // Row 1
            var tr = new TableRow();
            TableRowProperties rprops = new TableRowProperties(
                new TableRowHeight() { Val = (UInt32Value)36, HeightType = HeightRuleValues.AtLeast }
                );
            tr.AppendChild<TableRowProperties>(rprops);

            tr.Append(PrepareHeaderCell("Notes", headerWidth));
            tr.Append(PrepareCell("", cellWidth));

            table.AppendChild<TableRow>(tr);

            return table;
        }

        private Table AddNotesForMedicationTable()
        {
            Table table = new Table();

            var cellWidth = 11352;
            UInt32Value rowHeight = 24;

            TableProperties tprops = new TableProperties(
                new TableBorders(
                    new TopBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 5 },
                    new BottomBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 5 },
                    new LeftBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 5 },
                    new RightBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 5 },
                    new InsideHorizontalBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 5 },
                    new InsideVerticalBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 5 }
                    ),
                new TableGrid(
                    new GridColumn() { Width = cellWidth.ToString() }
                    ));

            table.AppendChild<TableProperties>(tprops);

            // Row 1
            var tr = new TableRow();
            TableRowProperties rprops = new TableRowProperties(
                new TableRowHeight() { Val = (UInt32Value)36, HeightType = HeightRuleValues.AtLeast }
                );
            tr.AppendChild<TableRowProperties>(rprops);

            tr.Append(PrepareHeaderCell("C.3 Notes", cellWidth));

            table.AppendChild<TableRow>(tr);

            // Row 2
            tr = new TableRow();
            rprops = new TableRowProperties(
                new TableRowHeight() { Val = (UInt32Value)36, HeightType = HeightRuleValues.AtLeast }
                );
            tr.AppendChild<TableRowProperties>(rprops);

            tr.Append(PrepareCell("", cellWidth));

            table.AppendChild<TableRow>(tr);

            return table;
        }

        private TableCell PrepareHeaderCell(string text, int width, bool centre = false, bool bold = false, int mergeCount = 0)
        {
            var tc = new TableCell();

            TableCellProperties props = new TableCellProperties(
                new TableCellWidth { Width = width.ToString() },
                new Shading { Color = "auto", ThemeFillShade = "D9", Fill = "D9D9D9" },
                new TableCellMargin(
                    new BottomMargin { Width = "30" },
                    new TopMargin { Width = "30" },
                    new LeftMargin { Width = "30" },
                    new RightMargin { Width = "30" }
                    ),
                new TableCellVerticalAlignment { Val = TableVerticalAlignmentValues.Center }
                );
            if (mergeCount > 0) { props.Append(new GridSpan { Val = mergeCount }); };
            tc.AppendChild<TableCellProperties>(props);

            if (centre)
            {
                ParagraphProperties pprop = new ParagraphProperties();
                Justification CenterHeading = new Justification() { Val = JustificationValues.Center };
                pprop.Append(CenterHeading);
                tc.AppendChild<ParagraphProperties>(pprop);
            };

            RunProperties runProperties = new RunProperties();
            if (bold) { runProperties.AppendChild(new Bold()); };
            FontSize fs = new FontSize();
            fs.Val = "20";
            runProperties.AppendChild(fs);
            Run run = new Run();
            run.AppendChild(runProperties);
            run.AppendChild(new Text(text));

            tc.Append(new Paragraph(run));

            return tc;
        }

        private TableCell PrepareCell(string text, int width, bool centre = true, int mergeCount = 0)
        {
            var tc = new TableCell();

            TableCellProperties props = new TableCellProperties(
                new TableCellWidth { Width = width.ToString() },
                new TableCellMargin(
                    new BottomMargin { Width = "30" },
                    new TopMargin { Width = "30" },
                    new LeftMargin { Width = "30" },
                    new RightMargin { Width = "30" }
                    ),
                new TableCellVerticalAlignment { Val = TableVerticalAlignmentValues.Center }
                );
            if (mergeCount > 0) { props.Append(new GridSpan { Val = mergeCount }); };
            tc.AppendChild<TableCellProperties>(props);

            if (centre)
            {
                ParagraphProperties pprop = new ParagraphProperties();
                Justification CenterHeading = new Justification() { Val = JustificationValues.Center };
                pprop.Append(CenterHeading);
                tc.AppendChild<ParagraphProperties>(pprop);
            };

            RunProperties runProperties = new RunProperties();
            FontSize fs = new FontSize();
            fs.Val = "20";
            runProperties.AppendChild(fs);
            Run run = new Run();
            run.AppendChild(runProperties);
            run.AppendChild(new Text(text));

            tc.Append(new Paragraph(run));

            return tc;
        }

        private void PrepareLogo(WordprocessingDocument document)
        {
            ImagePart imagePart = document.MainDocumentPart.AddImagePart(ImagePartType.Jpeg);

            var fileName = HttpRuntime.AppDomainAppPath + "\\Templates\\SIAPS_USAID_Small.jpg";
            using (FileStream stream = new FileStream(fileName, FileMode.Open))
            {
                imagePart.FeedData(stream);
            }

            AddImageToBody(document, document.MainDocumentPart.GetIdOfPart(imagePart));
        }

        private static void AddImageToBody(WordprocessingDocument wordDoc, string relationshipId)
        {
            // Define the reference of the image.
            var element =
                 new Drawing(
                     new DW.Inline(
                         new DW.Extent() { Cx = 1757548L, Cy = 253064L },
                         new DW.EffectExtent()
                         {
                             LeftEdge = 0L,
                             TopEdge = 0L,
                             RightEdge = 0L,
                             BottomEdge = 0L
                         },
                         new DW.DocProperties()
                         {
                             Id = (UInt32Value)1U,
                             Name = "Picture 1"
                         },
                         new DW.NonVisualGraphicFrameDrawingProperties(
                             new A.GraphicFrameLocks() { NoChangeAspect = true }),
                         new A.Graphic(
                             new A.GraphicData(
                                 new PIC.Picture(
                                     new PIC.NonVisualPictureProperties(
                                         new PIC.NonVisualDrawingProperties()
                                         {
                                             Id = (UInt32Value)0U,
                                             Name = "New Bitmap Image.jpg"
                                         },
                                         new PIC.NonVisualPictureDrawingProperties()),
                                     new PIC.BlipFill(
                                         new A.Blip(
                                             new A.BlipExtensionList(
                                                 new A.BlipExtension()
                                                 {
                                                     Uri =
                                                        "{28A0092B-C50C-407E-A947-70E740481C1C}"
                                                 })
                                         )
                                         {
                                             Embed = relationshipId,
                                             CompressionState =
                                             A.BlipCompressionValues.Print
                                         },
                                         new A.Stretch(
                                             new A.FillRectangle())),
                                     new PIC.ShapeProperties(
                                         new A.Transform2D(
                                             new A.Offset() { X = 0L, Y = 0L },
                                             new A.Extents() { Cx = 1757548L, Cy = 253064L }),
                                         new A.PresetGeometry(
                                             new A.AdjustValueList()
                                         )
                                         { Preset = A.ShapeTypeValues.Rectangle }))
                             )
                             { Uri = "http://schemas.openxmlformats.org/drawingml/2006/picture" })
                     )
                     {
                         DistanceFromTop = (UInt32Value)0U,
                         DistanceFromBottom = (UInt32Value)0U,
                         DistanceFromLeft = (UInt32Value)0U,
                         DistanceFromRight = (UInt32Value)0U,
                         EditId = "50D07946"
                     });

            Paragraph paragraph = new Paragraph();
            ParagraphProperties pprop = new ParagraphProperties();
            Justification CenterHeading = new Justification() { Val = JustificationValues.Center };
            pprop.Append(CenterHeading);
            pprop.ParagraphStyleId = new ParagraphStyleId() { Val = "logoheading" };
            paragraph.Append(pprop);

            Run run = new Run();
            run.AppendChild(element);
            paragraph.Append(run);

            // Append the reference to body, the element should be in a Run.
            wordDoc.MainDocumentPart.Document.Body.AppendChild(paragraph);
        }

        #endregion

        #region "Private"

        private int CalculateAge(DateTime birthDate, DateTime onsetDate)
        {
            var age = onsetDate.Year - birthDate.Year;
            if (onsetDate > birthDate.AddYears(-age)) age--;
            return age;
        }

        private IEnumerable<MergeElement> GetPatientSummaryMergeElementsForActiveReport(PatientClinicalEvent patientEvent, bool isSerious)
        {
            IExtendable patientEventExtended = patientEvent;
            IExtendable patientExtended = patientEvent.Patient;

            var reportInstance = _unitOfWork.Repository<ReportInstance>().Queryable().Single(ri => ri.ContextGuid == patientEvent.PatientClinicalEventGuid);

            var genderConfig = _unitOfWork.Repository<CustomAttributeConfiguration>().Queryable().SingleOrDefault(c => c.ExtendableTypeName == "Patient" && c.AttributeKey == "Gender");
            var outcomeConfig = _unitOfWork.Repository<CustomAttributeConfiguration>().Queryable().SingleOrDefault(c => c.ExtendableTypeName == "PatientClinicalEvent" && c.AttributeKey == "Outcome");
            var seriousnessConfig = _unitOfWork.Repository<CustomAttributeConfiguration>().Queryable().SingleOrDefault(c => c.ExtendableTypeName == "PatientClinicalEvent" && c.AttributeKey == "Seriousness");
            var scaleConfig = _unitOfWork.Repository<CustomAttributeConfiguration>().Queryable().SingleOrDefault(c => c.ExtendableTypeName == "PatientClinicalEvent" && c.AttributeKey == "Severity Grading Scale");
            var severityConfig = _unitOfWork.Repository<CustomAttributeConfiguration>().Queryable().SingleOrDefault(c => c.ExtendableTypeName == "PatientClinicalEvent" && c.AttributeKey == "Severity Grade");
            var routeConfig = _unitOfWork.Repository<CustomAttributeConfiguration>().Queryable().SingleOrDefault(c => c.ExtendableTypeName == "PatientMedication" && c.AttributeKey == "Route");
            var indicationConfig = _unitOfWork.Repository<CustomAttributeConfiguration>().Queryable().SingleOrDefault(c => c.ExtendableTypeName == "PatientMedication" && c.AttributeKey == "Indication");

            var mergeElements = new List<MergeElement>();

            mergeElements.Add(new TextMergeElement
            {
                MergeField = "PatientName",
                MergeValue = patientEvent.Patient.FullName
            });
            mergeElements.Add(new TextMergeElement
            {
                MergeField = "BirthDate",
                MergeValue = patientEvent.Patient.DateOfBirth != null ? Convert.ToDateTime(patientEvent.Patient.DateOfBirth).ToString("yyyy-MM-dd") : ""
            });
            mergeElements.Add(new TextMergeElement
            {
                MergeField = "AgeGroup",
                MergeValue = patientEventExtended.GetAttributeValue("Age Group") != null ? patientEventExtended.GetAttributeValue("Age Group").ToString() : ""
            });
            if (patientEvent.OnsetDate != null && patientEvent.Patient.DateOfBirth != null)
            {
                mergeElements.Add(new TextMergeElement
                {
                    MergeField = "OnsetAge",
                    MergeValue = CalculateAge(Convert.ToDateTime(patientEvent.Patient.DateOfBirth), Convert.ToDateTime(patientEvent.OnsetDate)).ToString() + " years"
                });
            }
            else
            {
                mergeElements.Add(new TextMergeElement
                {
                    MergeField = "OnsetAge",
                    MergeValue = string.Empty
                });
            }
            mergeElements.Add(new TextMergeElement
            {
                MergeField = "Gender",
                MergeValue = _attributeService.GetCustomAttributeValue(genderConfig, patientExtended)
            });
            mergeElements.Add(new TextMergeElement
            {
                MergeField = "Facility",
                MergeValue = patientEvent.Patient.GetCurrentFacility().Facility.FacilityName
            });
            mergeElements.Add(new TextMergeElement
            {
                MergeField = "MedicalNumber",
                MergeValue = patientExtended.GetAttributeValue("Medical Record Number") != null ? patientExtended.GetAttributeValue("Medical Record Number").ToString() : ""
            });
            mergeElements.Add(new TextMergeElement
            {
                MergeField = "IdentityNumber",
                MergeValue = patientExtended.GetAttributeValue("Patient Identity Number") != null ? patientExtended.GetAttributeValue("Patient Identity Number").ToString() : ""
            });
            mergeElements.Add(new TextMergeElement
            {
                MergeField = "SourceDescription",
                MergeValue = patientEvent.SourceDescription
            });
            mergeElements.Add(new TextMergeElement
            {
                MergeField = "MeddraTerm",
                MergeValue = patientEvent.SourceTerminologyMedDra.MedDraTerm
            });
            var onsetDate = patientEvent.OnsetDate != null ? Convert.ToDateTime(patientEvent.OnsetDate).ToString("yyyy-MM-dd") : "";
            mergeElements.Add(new TextMergeElement
            {
                MergeField = "OnsetDate",
                MergeValue = onsetDate
            });
            var resDate = patientEvent.ResolutionDate != null ? Convert.ToDateTime(patientEvent.ResolutionDate).ToString("yyyy-MM-dd") : "";
            mergeElements.Add(new TextMergeElement
            {
                MergeField = "ResolutionDate",
                MergeValue = resDate
            });
            if (!String.IsNullOrWhiteSpace(onsetDate) && !String.IsNullOrWhiteSpace(resDate))
            {
                var rduration = (Convert.ToDateTime(onsetDate) - Convert.ToDateTime(resDate)).Days;
                mergeElements.Add(new TextMergeElement
                {
                    MergeField = "Duration",
                    MergeValue = rduration.ToString() + " days"
                });
            }
            else
            {
                mergeElements.Add(new TextMergeElement
                {
                    MergeField = "Duration",
                    MergeValue = string.Empty
                });
            }
            mergeElements.Add(new TextMergeElement
            {
                MergeField = "Outcome",
                MergeValue = _attributeService.GetCustomAttributeValue(outcomeConfig, patientExtended)
            });

            // Cater for seriousness fields
            if (isSerious)
            {
                mergeElements.Add(new TextMergeElement
                {
                    MergeField = "Seriousness",
                    MergeValue = _attributeService.GetCustomAttributeValue(seriousnessConfig, patientExtended)
                });
                mergeElements.Add(new TextMergeElement
                {
                    MergeField = "GradingScale",
                    MergeValue = _attributeService.GetCustomAttributeValue(scaleConfig, patientExtended)

                });
                mergeElements.Add(new TextMergeElement
                {
                    MergeField = "SeverityGrade",
                    MergeValue = _attributeService.GetCustomAttributeValue(severityConfig, patientExtended)
                });
                mergeElements.Add(new TextMergeElement
                {
                    MergeField = "SAENumber",
                    MergeValue = patientEventExtended.GetAttributeValue("FDA SAE Number") != null ? patientEventExtended.GetAttributeValue("FDA SAE Number").ToString() : ""
                });
            }

            mergeElements.Add(new TextMergeElement
            {
                MergeField = "Weight",
                MergeValue = string.Empty
            });
            mergeElements.Add(new TextMergeElement
            {
                MergeField = "Height",
                MergeValue = string.Empty
            });

            // lab tests
            var ct = 0;
            foreach (PatientLabTest labTest in patientEvent.Patient.PatientLabTests.Where(lt => lt.TestDate >= patientEvent.OnsetDate).OrderByDescending(lt => lt.TestDate).Take(4))
            {
                ct += 1;
                var cts = string.Format("_{0}", ct.ToString());

                mergeElements.Add(new TextMergeElement
                {
                    MergeField = "Test" + cts,
                    MergeValue = labTest.LabTest.Description
                });

                mergeElements.Add(new TextMergeElement
                {
                    MergeField = "TestDate" + cts,
                    MergeValue = labTest.TestDate.ToString("yyyy-MM-dd")
                });

                mergeElements.Add(new TextMergeElement
                {
                    MergeField = "TestResult" + cts,
                    MergeValue = labTest.TestResult
                });
            }
            while (ct < 5)
            {
                ct += 1;
                var cts = string.Format("_{0}", ct.ToString());

                mergeElements.Add(new TextMergeElement
                {
                    MergeField = "Test" + cts,
                    MergeValue = string.Empty
                });

                mergeElements.Add(new TextMergeElement
                {
                    MergeField = "TestDate" + cts,
                    MergeValue = string.Empty
                });

                mergeElements.Add(new TextMergeElement
                {
                    MergeField = "TestResult" + cts,
                    MergeValue = string.Empty
                });
            }

            // Medications
            ct = 0;
            foreach (ReportInstanceMedication med in reportInstance.Medications.Take(2))
            {
                ct += 1;

                var cts = string.Format("_{0}", ct.ToString());
                var patientMedication = _unitOfWork.Repository<PatientMedication>().Queryable().Single(pm => pm.PatientMedicationGuid == med.ReportInstanceMedicationGuid);
                IExtendable mcExtended = patientMedication;

                mergeElements.Add(new TextMergeElement
                {
                    MergeField = "Drug" + cts,
                    MergeValue = patientMedication.Medication.DrugName
                });

                mergeElements.Add(new TextMergeElement
                {
                    MergeField = "DStart" + cts,
                    MergeValue = patientMedication.DateStart.ToString("yyyy-MM-dd")
                });

                var endDate = patientMedication.DateEnd != null ? Convert.ToDateTime(patientMedication.DateEnd).ToString("yyyy-MM-dd") : "";
                mergeElements.Add(new TextMergeElement
                {
                    MergeField = "DEnd" + cts,
                    MergeValue = endDate
                });

                mergeElements.Add(new TextMergeElement
                {
                    MergeField = "Dose" + cts,
                    MergeValue = patientMedication.Dose
                });

                mergeElements.Add(new TextMergeElement
                {
                    MergeField = "Route" + cts,
                    MergeValue = _attributeService.GetCustomAttributeValue(routeConfig, mcExtended)
                });

                mergeElements.Add(new TextMergeElement
                {
                    MergeField = "Indication" + cts,
                    MergeValue = _attributeService.GetCustomAttributeValue(indicationConfig, mcExtended)
                });
            }
            while (ct < 3)
            {
                ct += 1;
                var cts = string.Format("_{0}", ct.ToString());

                mergeElements.Add(new TextMergeElement
                {
                    MergeField = "Drug" + cts,
                    MergeValue = string.Empty
                });

                mergeElements.Add(new TextMergeElement
                {
                    MergeField = "DStart" + cts,
                    MergeValue = string.Empty
                });

                mergeElements.Add(new TextMergeElement
                {
                    MergeField = "DEnd" + cts,
                    MergeValue = string.Empty
                });

                mergeElements.Add(new TextMergeElement
                {
                    MergeField = "Dose" + cts,
                    MergeValue = string.Empty
                });

                mergeElements.Add(new TextMergeElement
                {
                    MergeField = "Route" + cts,
                    MergeValue = string.Empty
                });

                mergeElements.Add(new TextMergeElement
                {
                    MergeField = "Indication" + cts,
                    MergeValue = string.Empty
                });
            }

            // Conditions
            ct = 0;
            foreach (PatientCondition pc in patientEvent.Patient.PatientConditions.Where(pc => pc.DateStart <= patientEvent.OnsetDate).OrderByDescending(c => c.DateStart).Take(2))
            {
                ct += 1;
                var cts = string.Format("_{0}", ct.ToString());

                mergeElements.Add(new TextMergeElement
                {
                    MergeField = "Condition" + cts,
                    MergeValue = pc.TerminologyMedDra.MedDraTerm
                });

                mergeElements.Add(new TextMergeElement
                {
                    MergeField = "StartDate" + cts,
                    MergeValue = pc.DateStart.ToString("yyyy-MM-dd")
                });

                var endDate = pc.OutcomeDate != null ? Convert.ToDateTime(pc.OutcomeDate).ToString("yyyy-MM-dd") : "";
                mergeElements.Add(new TextMergeElement
                {
                    MergeField = "EndDate" + cts,
                    MergeValue = endDate
                });
            }
            while (ct < 3)
            {
                ct += 1;
                var cts = string.Format("_{0}", ct.ToString());

                mergeElements.Add(new TextMergeElement
                {
                    MergeField = "Condition" + cts,
                    MergeValue = string.Empty
                });

                mergeElements.Add(new TextMergeElement
                {
                    MergeField = "StartDate" + cts,
                    MergeValue = string.Empty
                });

                mergeElements.Add(new TextMergeElement
                {
                    MergeField = "EndDate" + cts,
                    MergeValue = string.Empty
                });
            }

            return mergeElements;
        }

        private IEnumerable<MergeElement> GetPatientSummaryMergeElementsForSpontaneousReport(DatasetInstance datasetInstance, bool isSerious)
        {
            DateTime tempdt;

            var reportInstance = _unitOfWork.Repository<ReportInstance>().Queryable().Single(ri => ri.ContextGuid == datasetInstance.DatasetInstanceGuid);

            var mergeElements = new List<MergeElement>();

            var temp = datasetInstance.GetInstanceValue(_unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.DatasetElementGuid.ToString() == "0D704069-5C50-4085-8FE1-355BB64EF196"));
            DateTime dob = DateTime.TryParse(temp, out tempdt) ? tempdt : DateTime.MinValue;

            temp = datasetInstance.GetInstanceValue(_unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.DatasetElementGuid.ToString() == "D314C438-5ABA-4ED2-855D-1A5B22B5A301"));
            string age = temp;
            temp = datasetInstance.GetInstanceValue(_unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.DatasetElementGuid.ToString() == "80C219DC-238C-487E-A3D5-8919ABA674B1"));
            age += " " + temp; // Include age unit

            temp = datasetInstance.GetInstanceValue(_unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.DatasetElementGuid.ToString() == "F5EEB382-D4A5-41A1-A447-37D5ECA50B99"));
            DateTime onsetDate = DateTime.TryParse(temp, out tempdt) ? tempdt : DateTime.MinValue;

            temp = datasetInstance.GetInstanceValue(_unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.DatasetElementGuid.ToString() == "F977C2F8-C7DD-4AFE-BCAA-1C06BD54D155"));
            DateTime recoveryDate = DateTime.TryParse(temp, out tempdt) ? tempdt : DateTime.MinValue;

            mergeElements.Add(new TextMergeElement
            {
                MergeField = "PatientName",
                MergeValue = datasetInstance.GetInstanceValue(_unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.DatasetElementGuid.ToString() == "29CD2157-8FB6-4883-A4E6-A4B9EDE6B36B"))
            });
            mergeElements.Add(new TextMergeElement
            {
                MergeField = "BirthDate",
                MergeValue = dob == DateTime.MinValue ? "" : dob.ToString("yyyy-MM-dd")
            });
            mergeElements.Add(new TextMergeElement
            {
                MergeField = "AgeGroup",
                MergeValue = ""
            });
            mergeElements.Add(new TextMergeElement
            {
                MergeField = "OnsetAge",
                MergeValue = age
            });
            mergeElements.Add(new TextMergeElement
            {
                MergeField = "Gender",
                MergeValue = datasetInstance.GetInstanceValue(_unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.DatasetElementGuid.ToString() == "E061D363-534E-4EA4-B6E5-F1C531931B12"))

            });
            mergeElements.Add(new TextMergeElement
            {
                MergeField = "Facility",
                MergeValue = string.Empty
            });
            mergeElements.Add(new TextMergeElement
            {
                MergeField = "MedicalNumber",
                MergeValue = string.Empty
            });
            mergeElements.Add(new TextMergeElement
            {
                MergeField = "IdentityNumber",
                MergeValue = datasetInstance.GetInstanceValue(_unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.DatasetElementGuid.ToString() == "5A2E89A9-8240-4665-967D-0C655CF281B7"))
            });
            mergeElements.Add(new TextMergeElement
            {
                MergeField = "SourceDescription",
                MergeValue = datasetInstance.GetInstanceValue(_unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.DatasetElementGuid.ToString() == "ACD938A4-76D1-44CE-A070-2B8DF0FE9E0F"))
            });
            mergeElements.Add(new TextMergeElement
            {
                MergeField = "MeddraTerm",
                MergeValue = reportInstance.TerminologyMedDra != null ? reportInstance.TerminologyMedDra.MedDraTerm : string.Empty
            });
            mergeElements.Add(new TextMergeElement
            {
                MergeField = "OnsetDate",
                MergeValue = onsetDate == DateTime.MinValue ? "" : onsetDate.ToString("yyyy-MM-dd")
            });
            mergeElements.Add(new TextMergeElement
            {
                MergeField = "ResolutionDate",
                MergeValue = recoveryDate == DateTime.MinValue ? "" : recoveryDate.ToString("yyyy-MM-dd")
            });
            if (onsetDate != DateTime.MinValue && recoveryDate != DateTime.MinValue)
            {
                var rduration = (onsetDate - recoveryDate).Days;
                mergeElements.Add(new TextMergeElement
                {
                    MergeField = "Duration",
                    MergeValue = rduration.ToString() + " days"
                });
            }
            else
            {
                mergeElements.Add(new TextMergeElement
                {
                    MergeField = "Duration",
                    MergeValue = string.Empty
                });
            }
            mergeElements.Add(new TextMergeElement
            {
                MergeField = "Outcome",
                MergeValue = datasetInstance.GetInstanceValue(_unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.DatasetElementGuid.ToString() == "976F6C53-78F2-4007-8F39-54057E554EEB"))
            });

            // Cater for seriousness fields
            if (isSerious)
            {
                mergeElements.Add(new TextMergeElement
                {
                    MergeField = "Seriousness",
                    MergeValue = datasetInstance.GetInstanceValue(_unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.DatasetElementGuid.ToString() == "302C07C9-B0E0-46AB-9EF8-5D5C2F756BF1"))
                });
                mergeElements.Add(new TextMergeElement
                {
                    MergeField = "GradingScale",
                    MergeValue = string.Empty

                });
                mergeElements.Add(new TextMergeElement
                {
                    MergeField = "SeverityGrade",
                    MergeValue = string.Empty
                });
                mergeElements.Add(new TextMergeElement
                {
                    MergeField = "SAENumber",
                    MergeValue = string.Empty
                });
            }

            mergeElements.Add(new TextMergeElement
            {
                MergeField = "Weight",
                MergeValue = datasetInstance.GetInstanceValue(_unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.DatasetElementGuid.ToString() == "985BD25D-54E7-4A24-8636-6DBC0F9C7B96"))
            });
            mergeElements.Add(new TextMergeElement
            {
                MergeField = "Height",
                MergeValue = string.Empty
            });

            // lab tests
            var sourceLabElement = _unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.DatasetElementGuid.ToString() == "12D7089D-1603-4309-99DE-60F20F9A005E");
            var sourceContexts = datasetInstance.GetInstanceSubValuesContext(sourceLabElement);

            var ct = 0;
            foreach (Guid sourceContext in sourceContexts)
            {
                ct += 1;
                var cts = string.Format("_{0}", ct.ToString());

                var labItemValues = datasetInstance.GetInstanceSubValues(sourceLabElement, sourceContext);

                var testDate = labItemValues.SingleOrDefault(div => div.DatasetElementSub.ElementName == "Test Date");
                var testResult = labItemValues.SingleOrDefault(div => div.DatasetElementSub.ElementName == "Test Result");

                mergeElements.Add(new TextMergeElement
                {
                    MergeField = "Test" + cts,
                    MergeValue = labItemValues.SingleOrDefault(div => div.DatasetElementSub.ElementName == "Test Name").InstanceValue
                });

                mergeElements.Add(new TextMergeElement
                {
                    MergeField = "TestDate" + cts,
                    MergeValue = testDate != null ? testDate.InstanceValue : string.Empty
                });

                mergeElements.Add(new TextMergeElement
                {
                    MergeField = "TestResult" + cts,
                    MergeValue = testResult != null ? testResult.InstanceValue : string.Empty
                });
            }
            while (ct < 5)
            {
                ct += 1;
                var cts = string.Format("_{0}", ct.ToString());

                mergeElements.Add(new TextMergeElement
                {
                    MergeField = "Test" + cts,
                    MergeValue = string.Empty
                });

                mergeElements.Add(new TextMergeElement
                {
                    MergeField = "TestDate" + cts,
                    MergeValue = string.Empty
                });

                mergeElements.Add(new TextMergeElement
                {
                    MergeField = "TestResult" + cts,
                    MergeValue = string.Empty
                });
            }

            // Medications
            var sourceProductElement = _unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.DatasetElementGuid.ToString() == "712CA632-0CD0-4418-9176-FB0B95AEE8A1");
            sourceContexts = datasetInstance.GetInstanceSubValuesContext(sourceProductElement);

            ct = 0;
            foreach (ReportInstanceMedication med in reportInstance.Medications.Take(2))
            {
                ct += 1;
                var cts = string.Format("_{0}", ct.ToString());

                var drugItemValues = datasetInstance.GetInstanceSubValues(sourceProductElement, med.ReportInstanceMedicationGuid);

                var startValue = drugItemValues.SingleOrDefault(div => div.DatasetElementSub.ElementName == "Product start date");
                var endValue = drugItemValues.SingleOrDefault(div => div.DatasetElementSub.ElementName == "Product end date");
                var dose = drugItemValues.SingleOrDefault(div => div.DatasetElementSub.ElementName == "Dose number");
                var route = drugItemValues.SingleOrDefault(div => div.DatasetElementSub.ElementName == "Product route of administration");
                var indication = drugItemValues.SingleOrDefault(div => div.DatasetElementSub.ElementName == "Product indication");

                mergeElements.Add(new TextMergeElement
                {
                    MergeField = "Drug" + cts,
                    MergeValue = drugItemValues.SingleOrDefault(div => div.DatasetElementSub.ElementName == "Product").InstanceValue
                });

                mergeElements.Add(new TextMergeElement
                {
                    MergeField = "DStart" + cts,
                    MergeValue = startValue != null ? startValue.InstanceValue : string.Empty
                });

                mergeElements.Add(new TextMergeElement
                {
                    MergeField = "DEnd" + cts,
                    MergeValue = endValue != null ? endValue.InstanceValue : string.Empty
                });

                mergeElements.Add(new TextMergeElement
                {
                    MergeField = "Dose" + cts,
                    MergeValue = dose != null ? dose.InstanceValue : string.Empty
                });

                mergeElements.Add(new TextMergeElement
                {
                    MergeField = "Route" + cts,
                    MergeValue = route != null ? route.InstanceValue : string.Empty
                });

                mergeElements.Add(new TextMergeElement
                {
                    MergeField = "Indication" + cts,
                    MergeValue = indication != null ? indication.InstanceValue : string.Empty
                });
            }
            while (ct < 3)
            {
                ct += 1;
                var cts = string.Format("_{0}", ct.ToString());

                mergeElements.Add(new TextMergeElement
                {
                    MergeField = "Drug" + cts,
                    MergeValue = string.Empty
                });

                mergeElements.Add(new TextMergeElement
                {
                    MergeField = "DStart" + cts,
                    MergeValue = string.Empty
                });

                mergeElements.Add(new TextMergeElement
                {
                    MergeField = "DEnd" + cts,
                    MergeValue = string.Empty
                });

                mergeElements.Add(new TextMergeElement
                {
                    MergeField = "Dose" + cts,
                    MergeValue = string.Empty
                });

                mergeElements.Add(new TextMergeElement
                {
                    MergeField = "Route" + cts,
                    MergeValue = string.Empty
                });

                mergeElements.Add(new TextMergeElement
                {
                    MergeField = "Indication" + cts,
                    MergeValue = string.Empty
                });
            }

            // Conditions
            ct = 0;
            while (ct < 3)
            {
                ct += 1;
                var cts = string.Format("_{0}", ct.ToString());

                mergeElements.Add(new TextMergeElement
                {
                    MergeField = "Condition" + cts,
                    MergeValue = string.Empty
                });

                mergeElements.Add(new TextMergeElement
                {
                    MergeField = "StartDate" + cts,
                    MergeValue = string.Empty
                });

                mergeElements.Add(new TextMergeElement
                {
                    MergeField = "EndDate" + cts,
                    MergeValue = string.Empty
                });
            }

            return mergeElements;
        }

        private XmlNode ProcessNode(DatasetXmlNode dsXmlNode, ref XmlDocument xmlDoc, ref DatasetInstance datasetInstance, DatasetInstanceSubValue[] subItemValues = null)
        {
            XmlNode parentNode = null;

            switch (dsXmlNode.NodeType)
            {
                case NodeType.RootNode:
                case NodeType.StandardNode:
                    parentNode = PrepareNode(dsXmlNode, ref xmlDoc, ref datasetInstance);

                    if (dsXmlNode.DatasetElement != null)
                    {
                        var value = datasetInstance.GetInstanceValue(dsXmlNode.DatasetElement);
                        if (value.IndexOf("=") > -1)
                        {
                            value = value.Substring(0, value.IndexOf("="));
                        }
                        parentNode.InnerText = value;
                    }
                    else
                    {
                        if (dsXmlNode.ChildrenNodes.Count > 0)
                        {
                            foreach (DatasetXmlNode dsTempXmlNode in dsXmlNode.ChildrenNodes)
                            {
                                if (dsTempXmlNode.NodeType == NodeType.RepeatingNode)
                                {
                                    // Get corresponding table element
                                    var sourceContexts = datasetInstance.GetInstanceSubValuesContext(dsTempXmlNode.DatasetElement);
                                    foreach (Guid sourceContext in sourceContexts)
                                    {
                                        var values = datasetInstance.GetInstanceSubValues(dsTempXmlNode.DatasetElement, sourceContext);
                                        var childNode = ProcessNode(dsTempXmlNode, ref xmlDoc, ref datasetInstance, values);
                                        parentNode.AppendChild(childNode);
                                    }
                                }
                                else
                                {
                                    var childNode = ProcessNode(dsTempXmlNode, ref xmlDoc, ref datasetInstance);
                                    parentNode.AppendChild(childNode);
                                }
                            }
                        }
                    }
                    break;

                case NodeType.RepeatingNode:
                    // Main table node
                    parentNode = PrepareNode(dsXmlNode, ref xmlDoc, ref datasetInstance);

                    if (dsXmlNode.ChildrenNodes.Count > 0)
                    {
                        foreach (DatasetXmlNode dsTempXmlNode in dsXmlNode.ChildrenNodes)
                        {
                            var childNode = PrepareNode(dsTempXmlNode, ref xmlDoc, ref datasetInstance);
                            var subvalue = subItemValues.SingleOrDefault(siv => siv.DatasetElementSub.Id == dsTempXmlNode.DatasetElementSub.Id);
                            if (subvalue != null)
                            {
                                var value = subvalue.InstanceValue;
                                if (value.IndexOf("=") > -1)
                                {
                                    value = value.Substring(0, value.IndexOf("="));
                                }
                                childNode.InnerText = value;
                            }
                            parentNode.AppendChild(childNode);
                        }
                    }
                    break;

                default:
                    break;
            }

            return parentNode;
        }

        private XmlNode PrepareNode(DatasetXmlNode dsXmlNode, ref XmlDocument xmlDoc, ref DatasetInstance datasetInstance)
        {
            XmlAttribute attrib;

            XmlNode childNode = xmlDoc.CreateElement(dsXmlNode.NodeName, "");
            foreach (DatasetXmlAttribute dsXmlAttr in dsXmlNode.NodeAttributes)
            {
                attrib = xmlDoc.CreateAttribute(dsXmlAttr.AttributeName);
                if (dsXmlAttr.DatasetElement != null)
                {
                    var value = datasetInstance.GetInstanceValue(dsXmlAttr.DatasetElement);
                    if (value.IndexOf("=") > -1)
                    {
                        value = value.Substring(0, value.IndexOf("="));
                    }
                    attrib.InnerText = value;
                }
                else
                {
                    attrib.InnerText = dsXmlAttr.AttributeValue;
                }

                childNode.Attributes.Append(attrib);
            }

            return childNode;
        }

        static public string FormatXML(XmlDocument doc)
        {
            StringBuilder sb = new StringBuilder();
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.IndentChars = "  ";
            settings.NewLineChars = "\r\n";
            settings.NewLineHandling = NewLineHandling.Replace;
            using (XmlWriter writer = XmlWriter.Create(sb, settings))
            {
                doc.Save(writer);
            }
            return sb.ToString();
        }

        private void WriteXML(string xmlFileName, string xmlText)
        {
            // Write the string to a file.
            StreamWriter file = new System.IO.StreamWriter(xmlFileName);

            file.Write(xmlText);

            file.Close();
            file = null;
        }

        #endregion

        #region "Excel Processing"

        private void ProcessEntity(Object obj, ref ExcelWorksheet ws, ref int rowCount, ref int colCount, string entityName)
        {
            DateTime tempdt;

            // Write headers
            Type type = obj.GetType();
            PropertyInfo[] properties = type.GetProperties();

            var attributes = _unitOfWork.Repository<CustomAttributeConfiguration>().Queryable().Where(c => c.ExtendableTypeName == entityName).OrderBy(c => c.Id);

            IQueryable elements = null;
            if (entityName == "Encounter")
            {
                elements = _unitOfWork.Repository<DatasetCategoryElement>().Queryable().Include("DatasetCategory.Dataset").Include("DatasetElement").Where(dce => dce.DatasetCategory.Dataset.DatasetName == "Chronic Treatment").OrderBy(dce => dce.DatasetCategory.CategoryOrder).ThenBy(dce => dce.FieldOrder);
            }

            if (rowCount == 1)
            {
                foreach (PropertyInfo property in properties)
                {
                    if (property.Name != "CustomAttributesXmlSerialised")
                    {
                        if (property.PropertyType == typeof(DateTime?)
                            || property.PropertyType == typeof(DateTime)
                            || property.PropertyType == typeof(string)
                            || property.PropertyType == typeof(int)
                            || property.PropertyType == typeof(decimal)
                            || property.PropertyType == typeof(User)
                            || property.PropertyType == typeof(Medication)
                            || property.PropertyType == typeof(Patient)
                            || property.PropertyType == typeof(Encounter)
                            || property.PropertyType == typeof(TerminologyMedDra)
                            || property.PropertyType == typeof(Outcome)
                            || property.PropertyType == typeof(LabTest)
                            || property.PropertyType == typeof(LabTestUnit)
                            || property.PropertyType == typeof(CohortGroup)
                            || property.PropertyType == typeof(Facility)
                            || property.PropertyType == typeof(Priority)
                            || property.PropertyType == typeof(EncounterType)
                            || property.PropertyType == typeof(TreatmentOutcome)
                            || property.PropertyType == typeof(Guid)
                            || property.PropertyType == typeof(bool)
                            )
                        {
                            ws.Cells[GetExcelColumnName(colCount) + rowCount].Value = property.Name;
                            colCount += 1;
                        }
                    }
                }

                // Now process attributes
                foreach (CustomAttributeConfiguration attribute in attributes)
                {
                    ws.Cells[GetExcelColumnName(colCount) + rowCount].Value = attribute.AttributeKey;
                    colCount += 1;
                }

                // Process instance headers
                if (elements != null)
                {
                    foreach (DatasetCategoryElement dce in elements)
                    {
                        ws.Cells[GetExcelColumnName(colCount) + rowCount].Value = dce.DatasetElement.ElementName;
                        colCount += 1;
                    }
                }

                //Set the first header and format it
                using (var r = ws.Cells["A1:" + GetExcelColumnName(colCount - 1) + "1"])
                {
                    r.Style.Font.SetFromFont(new System.Drawing.Font("Arial", 10, FontStyle.Bold));
                    r.Style.Font.Color.SetColor(System.Drawing.Color.White);
                    r.Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;
                    r.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    r.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(23, 55, 93));
                }
            }

            rowCount += 1;
            colCount = 1;

            var subOutput = "**IGNORE**";

            // Write values
            foreach (PropertyInfo property in properties)
            {
                if (property.Name != "CustomAttributesXmlSerialised")
                {
                    subOutput = "**IGNORE**";
                    if (property.PropertyType == typeof(DateTime?)
                        || property.PropertyType == typeof(DateTime))
                    {
                        var dt = property.GetValue(obj, null) != null ? Convert.ToDateTime(property.GetValue(obj, null)).ToString("yyyy-MM-dd") : "";
                        subOutput = dt;
                    }
                    if (property.PropertyType == typeof(string)
                        || property.PropertyType == typeof(int)
                        || property.PropertyType == typeof(decimal)
                        || property.PropertyType == typeof(Guid)
                        || property.PropertyType == typeof(bool)
                        )
                    {
                        subOutput = property.GetValue(obj, null) != null ? property.GetValue(obj, null).ToString() : "";
                    }
                    if (property.PropertyType == typeof(User))
                    {
                        subOutput = "";
                        if (property.GetValue(obj, null) != null)
                        {
                            var user = (User)property.GetValue(obj, null);
                            subOutput = user.UserName;
                        }
                    }
                    if (property.PropertyType == typeof(Patient))
                    {
                        subOutput = "";
                        if (property.GetValue(obj, null) != null)
                        {
                            var patient = (Patient)property.GetValue(obj, null);
                            subOutput = patient.PatientGuid.ToString();
                        }
                    }
                    if (property.PropertyType == typeof(Medication))
                    {
                        subOutput = "";
                        if (property.GetValue(obj, null) != null)
                        {
                            var medication = (Medication)property.GetValue(obj, null);
                            subOutput = medication.DrugName;
                        }
                    }
                    if (property.PropertyType == typeof(Encounter))
                    {
                        subOutput = "";
                        if (property.GetValue(obj, null) != null)
                        {
                            var encounter = (Encounter)property.GetValue(obj, null);
                            subOutput = encounter.EncounterGuid.ToString();
                        }
                    }
                    if (property.PropertyType == typeof(TerminologyMedDra))
                    {
                        subOutput = "";
                        if (property.GetValue(obj, null) != null)
                        {
                            var meddra = (TerminologyMedDra)property.GetValue(obj, null);
                            subOutput = meddra.DisplayName;
                        }
                    }
                    if (property.PropertyType == typeof(Outcome))
                    {
                        subOutput = "";
                        if (property.GetValue(obj, null) != null)
                        {
                            var outcome = (Outcome)property.GetValue(obj, null);
                            subOutput = outcome.Description;
                        }
                    }
                    if (property.PropertyType == typeof(TreatmentOutcome))
                    {
                        subOutput = "";
                        if (property.GetValue(obj, null) != null)
                        {
                            var txOutcome = (TreatmentOutcome)property.GetValue(obj, null);
                            subOutput = txOutcome.Description;
                        }
                    }
                    if (property.PropertyType == typeof(LabTest))
                    {
                        subOutput = "";
                        if (property.GetValue(obj, null) != null)
                        {
                            var labTest = (LabTest)property.GetValue(obj, null);
                            subOutput = labTest.Description;
                        }
                    }
                    if (property.PropertyType == typeof(LabTestUnit))
                    {
                        subOutput = "";
                        if (property.GetValue(obj, null) != null)
                        {
                            var unit = (LabTestUnit)property.GetValue(obj, null);
                            subOutput = unit.Description;
                        }
                    }
                    if (property.PropertyType == typeof(CohortGroup))
                    {
                        subOutput = "";
                        if (property.GetValue(obj, null) != null)
                        {
                            var group = (CohortGroup)property.GetValue(obj, null);
                            subOutput = group.DisplayName;
                        }
                    }
                    if (property.PropertyType == typeof(Facility))
                    {
                        subOutput = "";
                        if (property.GetValue(obj, null) != null)
                        {
                            var facility = (Facility)property.GetValue(obj, null);
                            subOutput = facility.FacilityName;
                        }
                    }
                    if (property.PropertyType == typeof(Priority))
                    {
                        subOutput = "";
                        if (property.GetValue(obj, null) != null)
                        {
                            var priority = (Priority)property.GetValue(obj, null);
                            subOutput = priority.Description;
                        }
                    }
                    if (property.PropertyType == typeof(EncounterType))
                    {
                        subOutput = "";
                        if (property.GetValue(obj, null) != null)
                        {
                            var et = (EncounterType)property.GetValue(obj, null);
                            subOutput = et.Description;
                        }
                    }
                    if (subOutput != "**IGNORE**")
                    {
                        ws.Cells[GetExcelColumnName(colCount) + rowCount].Value = subOutput;
                        colCount += 1;
                    }
                }
            }

            IExtendable extended = null;
            switch (entityName)
            {
                case "Patient":
                    extended = (Patient)obj;
                    break;

                case "PatientMedication":
                    extended = (PatientMedication)obj;
                    break;

                case "PatientClinicalEvent":
                    extended = (PatientClinicalEvent)obj;
                    break;

                case "PatientCondition":
                    extended = (PatientCondition)obj;
                    break;

                case "PatientLabTest":
                    extended = (PatientLabTest)obj;
                    break;

                default:
                    break;
            }

            if (extended != null)
            {
                foreach (CustomAttributeConfiguration attribute in attributes)
                {
                    var output = "";
                    var val = extended.GetAttributeValue(attribute.AttributeKey);
                    if (val != null)
                    {
                        if (attribute.CustomAttributeType == CustomAttributeType.Selection)
                        {
                            var tempSDI = _unitOfWork.Repository<SelectionDataItem>().Queryable().SingleOrDefault(u => u.AttributeKey == attribute.AttributeKey && u.SelectionKey == val.ToString());
                            if (tempSDI != null)
                                output = tempSDI.Value;
                        }
                        else if (attribute.CustomAttributeType == CustomAttributeType.DateTime)
                        {
                            if (attribute != null && DateTime.TryParse(val.ToString(), out tempdt))
                            {
                                output = tempdt.ToString("yyyy-MM-dd");
                            }
                            else
                            {
                                output = val.ToString();
                            }
                        }
                        else
                        {
                            output = val.ToString();
                        }
                    }

                    ws.Cells[GetExcelColumnName(colCount) + rowCount].Value = output;
                    colCount += 1;
                }
            }

            if (elements != null)
            {
                var id = Convert.ToInt32(((Encounter)obj).Id);
                var instance = _unitOfWork.Repository<DatasetInstance>().Queryable().Include("Dataset").SingleOrDefault(di => di.Dataset.DatasetName == "Chronic Treatment" && di.ContextID == id);
                foreach (DatasetCategoryElement dce in elements)
                {
                    var eleOutput = "";
                    if (instance != null)
                    {
                        eleOutput = instance.GetInstanceValue(dce.DatasetElement);
                    }

                    ws.Cells[GetExcelColumnName(colCount) + rowCount].Value = eleOutput;
                    colCount += 1;
                }
            }
        }

        private string GetExcelColumnName(int columnNumber)
        {
            int dividend = columnNumber;
            string columnName = String.Empty;
            int modulo;

            while (dividend > 0)
            {
                modulo = (dividend - 1) % 26;
                columnName = Convert.ToChar(65 + modulo).ToString() + columnName;
                dividend = (int)((dividend - modulo) / 26);
            }

            return columnName;
        }

        #endregion
    }
}

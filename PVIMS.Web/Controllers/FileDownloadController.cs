using Ionic.Zip;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Xml;

using VPS.Common.Repositories;

using PVIMS.Core.Entities;
using PVIMS.Core.Services;

namespace PVIMS.Web.Controllers
{
    public class FileDownloadController : BaseController
    {
        private readonly IUnitOfWorkInt _unitOfWork;
        private readonly IArtefactService _artefactService;

        public FileDownloadController(IUnitOfWorkInt unitOfWork, IArtefactService artefactService)
        {
            this._unitOfWork = unitOfWork;
            _artefactService = artefactService;
        }

        public ActionResult DownloadActiveDataset()
        {
            var model = _artefactService.CreateActiveDatasetForDownload(0);
            return Json(new { success = true, model.FileName }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult DownloadSpontaneousDataset()
        {
            var model = _artefactService.CreateSpontaneousDatasetForDownload();
            return File(string.Format("{0}{1}", model.Path, model.FileName), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", model.FileName);
        }

        public ActionResult DownloadSingleAttachment(long attid)
        {
            var generatedDate = DateTime.Now;
            var documentDirectory = String.Format("{0}\\Temp\\", System.AppDomain.CurrentDomain.BaseDirectory);

            // Get attachment
            var attachment = _unitOfWork.Repository<Attachment>().Queryable().SingleOrDefault(a => a.Id == attid);
            if (attachment == null)
                return null;

            // Write file
            FileStream fs = new System.IO.FileStream(string.Format("{0}{1}", documentDirectory, attachment.FileName), System.IO.FileMode.Create, FileAccess.Write);
            // Writes a block of bytes to this stream using data from // a byte array.
            fs.Write(attachment.Content, 0, attachment.Content.Length);

            // close file stream
            fs.Close();

            switch (attachment.AttachmentType.Key)
            {
                case "docx":
                    return File(string.Format("{0}{1}", documentDirectory, attachment.FileName), "application/vnd.openxmlformats-officedocument.wordprocessingml.document", attachment.FileName);
                case "doc":
                    return File(string.Format("{0}{1}", documentDirectory, attachment.FileName), "application/msword", attachment.FileName);
                case "xlsx":
                    return File(string.Format("{0}{1}", documentDirectory, attachment.FileName), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", attachment.FileName);
                case "xls":
                    return File(string.Format("{0}{1}", documentDirectory, attachment.FileName), "application/vnd.ms-excel", attachment.FileName);
                case "pdf":
                    return File(string.Format("{0}{1}", documentDirectory, attachment.FileName), "application/pdf", attachment.FileName);
                case "png":
                    return File(string.Format("{0}{1}", documentDirectory, attachment.FileName), "image/png", attachment.FileName);
                case "jpg":
                case "jpeg":
                    return File(string.Format("{0}{1}", documentDirectory, attachment.FileName), "image/jpeg", attachment.FileName);
                case "bmp":
                    return File(string.Format("{0}{1}", documentDirectory, attachment.FileName), "image/bmp", attachment.FileName);
                case "xml":
                    return File(string.Format("{0}{1}", documentDirectory, attachment.FileName), "application/xhtml+xml", attachment.FileName);
            }
            return null;

        }

        public ActionResult DownloadPatientAttachment(long pid)
        {
            var generatedDate = DateTime.Now;
            var documentDirectory = String.Format("{0}\\Temp\\", System.AppDomain.CurrentDomain.BaseDirectory);
            var attachmentFileNames = new List<string>();

            // Get attachment
            var patient = _unitOfWork.Repository<Patient>().Queryable().SingleOrDefault(p => p.Id == pid);
            if (patient == null)
                return null;

            // Write files
            FileStream fs;
            foreach (Attachment att in patient.Attachments.Where(a => a.Archived == false))
            {
                fs = new System.IO.FileStream(string.Format("{0}{1}", documentDirectory, att.FileName), System.IO.FileMode.Create, FileAccess.Write);
                // Writes a block of bytes to this stream using data from // a byte array.
                fs.Write(att.Content, 0, att.Content.Length);

                // close file stream
                fs.Close();
                attachmentFileNames.Add(att.FileName);
            }

            var zipFileName = string.Format("Patient_{0}_Attachments_{1}.zip", pid.ToString(), generatedDate.ToString("yyyyMMddhhmmss"));

            using (var zip = new ZipFile())
            {
                zip.AddFiles(attachmentFileNames.Select(f => string.Format("{0}{1}", documentDirectory, f)).ToList(), string.Empty);
                zip.Save(string.Format("{0}{1}", documentDirectory, zipFileName));
            }

            return File(string.Format("{0}{1}", documentDirectory, zipFileName), "application/zip", zipFileName);
        }

        public ActionResult DownloadPatientSummaryForActiveReport(Guid contextGuid, bool isSerious)
        {
            var model = _artefactService.CreatePatientSummaryForActiveReport(contextGuid, isSerious);
            return File(string.Format("{0}\\{1}", model.Path, model.FileName), "application/vnd.openxmlformats-officedocument.wordprocessingml.document", model.FileName);
        }

        public ActionResult DownloadPatientSummaryForSpontaneousReport(Guid contextGuid, bool isSerious)
        {
            var model = _artefactService.CreatePatientSummaryForSpontaneousReport(contextGuid, isSerious);
            return File(string.Format("{0}\\{1}", model.Path, model.FileName), "application/vnd.openxmlformats-officedocument.wordprocessingml.document", model.FileName);
        }

        public FilePathResult DownloadDatasetScript(long datasetId)
        {
            var generatedDate = DateTime.Now;
            var documentDirectory = String.Format("{0}\\Temp\\", System.AppDomain.CurrentDomain.BaseDirectory);

            StringBuilder sb = new StringBuilder();

            Dataset dataset = _unitOfWork.Repository<Dataset>()
                .Queryable()
                .SingleOrDefault(ds => ds.Id == datasetId);

            string destName = string.Format("DS_{0}_{1}.sql", datasetId.ToString(), DateTime.Today.ToString("yyyyMMddhhmmsss"));
            string destFile = string.Format("{0}{1}", documentDirectory, destName);

            StreamWriter file = new System.IO.StreamWriter(destFile);

            file.WriteLine("/**************************************************");
            file.WriteLine("DATASET");
            file.WriteLine("**************************************************/");

            file.WriteLine("DECLARE @dsid int");
            file.WriteLine("DECLARE @dscid int");
            file.WriteLine("DECLARE @fid int");
            file.WriteLine("DECLARE @deid int");
            file.WriteLine("DECLARE @dceid int");

            sb.AppendLine("INSERT [dbo].[Dataset] ([DatasetName], [Active], [InitialiseProcess], [RulesProcess], [Help], [Created], [LastUpdated], [ContextType_Id], [CreatedBy_Id], [UpdatedBy_Id])");
            sb.AppendFormat("{1}VALUES ('{0}', ", dataset.DatasetName, "\t");
            sb.AppendFormat("{0}, ", dataset.Active ? 1 : 0);
            sb.AppendFormat("'{0}', ", dataset.InitialiseProcess);
            sb.AppendFormat("'{0}', ", dataset.RulesProcess);
            sb.AppendFormat("'{0}', ", dataset.Help);
            sb.AppendFormat("'{0}', ", dataset.Created.ToString("yyyy-MM-dd hh:mm"));
            sb.AppendFormat("'{0}', ", dataset.Created.ToString("yyyy-MM-dd hh:mm"));
            sb.AppendFormat("{0}, ", dataset.ContextType.Id);
            sb.AppendFormat("{0}, ", dataset.CreatedBy.Id);
            sb.AppendFormat("{0}) ", dataset.UpdatedBy.Id);
            file.WriteLine(sb.ToString());
            sb.Clear();

            file.WriteLine("set @dsid = (SELECT @@IDENTITY)");
            file.WriteLine("");

            foreach (DatasetCategory dc in dataset.DatasetCategories.Where(dc => dc.DatasetCategoryElements.Count > 0 && dc.DatasetCategoryName != "Required Information").OrderBy(cat => cat.CategoryOrder))
            {
                file.WriteLine("/**************************************************");
                file.WriteLine(String.Format("CATEGORY {0}", dc.DatasetCategoryName));
                file.WriteLine("**************************************************/");

                sb.AppendLine("INSERT [dbo].[DatasetCategory] ([DatasetCategoryName], [CategoryOrder], [Dataset_Id], [FriendlyName], [Help])");
                sb.AppendFormat("{1}VALUES ('{0}', ", dc.DatasetCategoryName, "\t");
                sb.AppendFormat("{0}, ", dc.CategoryOrder);
                sb.AppendFormat("{0}, ", "@dsid");
                sb.AppendFormat("'{0}', ", dc.FriendlyName != null ? dc.FriendlyName.Replace("'", "''") : "");
                sb.AppendFormat("'{0}') ", dc.Help != null ? dc.Help.Replace("'", "''") : "");
                file.WriteLine(sb.ToString());
                sb.Clear();

                file.WriteLine("set @dscid = (SELECT @@IDENTITY)");
                file.WriteLine("");

                foreach (DatasetCategoryElement dce in dc.DatasetCategoryElements.OrderBy(ele => ele.FieldOrder))
                {
                    file.WriteLine(String.Format("-- {0}", dce.DatasetElement.ElementName));

                    sb.AppendLine("INSERT [dbo].[Field] (Mandatory, MaxLength, Decimals, MaxSize, MinSize, Calculation, FileSize, FileExt, Anonymise, FieldType_Id)");
                    sb.AppendFormat("{1}VALUES ({0}, ", dce.DatasetElement.Field.Mandatory ? 1 : 0, "\t");
                    sb.AppendFormat("{0}, ", dce.DatasetElement.Field.MaxLength == null ? "NULL" : dce.DatasetElement.Field.MaxLength.ToString());
                    sb.AppendFormat("{0}, ", dce.DatasetElement.Field.Decimals == null ? "NULL" : dce.DatasetElement.Field.Decimals.ToString());
                    sb.AppendFormat("{0}, ", dce.DatasetElement.Field.MaxSize == null ? "NULL" : dce.DatasetElement.Field.MaxSize.ToString());
                    sb.AppendFormat("{0}, ", dce.DatasetElement.Field.MinSize == null ? "NULL" : dce.DatasetElement.Field.MinSize.ToString());
                    sb.AppendFormat("'{0}', ", dce.DatasetElement.Field.Calculation);
                    sb.AppendFormat("{0}, ", dce.DatasetElement.Field.FileSize == null ? "NULL" : dce.DatasetElement.Field.FileSize.ToString());
                    sb.AppendFormat("'{0}', ", dce.DatasetElement.Field.FileExt);
                    sb.AppendFormat("{0}, ", dce.DatasetElement.Field.Anonymise ? 1 : 0);
                    sb.AppendFormat("{0}) ", dce.DatasetElement.Field.FieldType.Id);
                    file.WriteLine(sb.ToString());
                    sb.Clear();

                    file.WriteLine("set @fid = (SELECT @@IDENTITY)");

                    if (dce.DatasetElement.Field.FieldValues.Count > 0)
                    {
                        foreach (FieldValue fv in dce.DatasetElement.Field.FieldValues.OrderBy(f => f.Id))
                        {
                            sb.AppendLine("INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id)");
                            sb.AppendFormat("{1}VALUES ('{0}', ", fv.Value.Replace("'", "''"), "\t");
                            sb.AppendFormat("{0}, ", fv.Default ? 1 : 0);
                            sb.AppendFormat("{0}, ", fv.Other ? 1 : 0);
                            sb.AppendFormat("{0}, ", fv.Unknown ? 1 : 0);
                            sb.AppendFormat("{0}) ", "@fid");
                            file.WriteLine(sb.ToString());
                            sb.Clear();
                        }
                    }

                    sb.AppendLine("INSERT [dbo].[DatasetElement] ([ElementName], [Field_Id], [DatasetElementType_Id], [OID], [DefaultValue], [System])");
                    sb.AppendFormat("{1}VALUES ('{0}', ", dce.DatasetElement.ElementName, "\t");
                    sb.AppendFormat("{0}, ", "@fid");
                    sb.AppendFormat("{0}, ", dce.DatasetElement.DatasetElementType.Id);
                    sb.AppendFormat("'{0}', ", dce.DatasetElement.OID);
                    sb.AppendFormat("'{0}', ", dce.DatasetElement.DefaultValue);
                    sb.AppendFormat("{0}) ", dce.DatasetElement.System ? 1 : 0);
                    file.WriteLine(sb.ToString());
                    sb.Clear();

                    file.WriteLine("set @deid = (SELECT @@IDENTITY)");

                    sb.AppendLine("INSERT [dbo].[DatasetCategoryElement] ([FieldOrder], [DatasetCategory_Id], [DatasetElement_Id], [Acute], [Chronic], [FriendlyName], [Help])");
                    sb.AppendFormat("{1}VALUES ({0}, ", dce.FieldOrder, "\t");
                    sb.AppendFormat("{0}, ", "@dscid");
                    sb.AppendFormat("{0}, ", "@deid");
                    sb.AppendFormat("{0}, ", dce.Acute ? 1 : 0);
                    sb.AppendFormat("{0}, ", dce.Chronic ? 1 : 0);
                    sb.AppendFormat("'{0}', ", dce.FriendlyName != null ? dce.FriendlyName.Replace("'", "''") : "");
                    sb.AppendFormat("'{0}') ", dce.Help != null ? dce.Help.Replace("'", "''") : "");
                    file.WriteLine(sb.ToString());
                    sb.Clear();

                    file.WriteLine("set @dceid = (SELECT @@IDENTITY)");
                    file.WriteLine("");
                }
                sb.Clear();
            }

            file.Close();
            file = null;

            return File(destFile, "text/plain ", destName);
        }

        public ActionResult DownloadDatasetInstanceSpreadsheet(long datasetInstanceId)
        {
            var model = _artefactService.CreateDatasetInstanceForDownload(datasetInstanceId);
            return File(string.Format("{0}{1}", model.Path, model.FileName), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", model.FileName);
        }

        public FilePathResult DownloadAuditLog(long auId)
        {
            // Create XML file
            var generatedDate = DateTime.Now;
            var documentDirectory = String.Format("{0}\\Temp\\", System.AppDomain.CurrentDomain.BaseDirectory);

            var audit = _unitOfWork.Repository<AuditLog>()
                .Queryable()
                .SingleOrDefault(au => au.Id == auId);

            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(audit.Log);

            var contentXml = FormatXML(xmlDoc);

            string destName = string.Format("AU_{0}_{1}.xml", auId.ToString(), DateTime.Today.ToString("yyyyMMddhhmmsss"));
            string destFile = string.Format("{0}{1}", documentDirectory, destName);

            WriteXML(destFile, contentXml);

            return File(destFile, "application/xml ", destName);
        }

        public ActionResult DownloadExcelFile (string fileName)
        {
            var documentDirectory = String.Format("{0}\\Temp\\", System.AppDomain.CurrentDomain.BaseDirectory);
            var fullPath = String.Format("{0}{1}", documentDirectory, fileName);

            return File(fullPath, "application/vnd.ms-excel", fileName);
        }

        public ActionResult DownloadWordFile(string fileName)
        {
            var documentDirectory = String.Format("{0}\\Temp\\", System.AppDomain.CurrentDomain.BaseDirectory);
            var fullPath = String.Format("{0}{1}", documentDirectory, fileName);

            return File(fullPath, "application/vnd.openxmlformats-officedocument.wordprocessingml.document", fileName);
        }

        #region "Private"

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
            string line = "********************************************************************";

            // Write the string to a file.
            StreamWriter file = new System.IO.StreamWriter(xmlFileName);

            file.Write(xmlText);

            file.Close();
            file = null;
        }

        #endregion

    }
}

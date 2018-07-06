using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml;

using PVIMS.Core;
using PVIMS.Core.Entities;
using PVIMS.Entities.EF;
using PVIMS.Core.Services;

namespace PVIMS.Services
{
    public class MedDraService : IMedDraService
    {
        private string _summary = "";
        private string _subDirectory = "";
        private string _currentVersion = "";
        private string _version = "";

        private ArrayList _movements;
        private ArrayList _updates;
        private ArrayList _additions;
        private int _refreshCount;

        List<String> _files = new List<String>() { "meddra_release.asc", "llt.asc", "pt.asc", "hlt.asc", "hlgt.asc", "soc.asc", "hlt_pt.asc", "hlgt_hlt.asc", "soc_hlgt.asc" };
        List<int> _elements = new List<int>() { 2, 3, 2, 2, 2, 2, 2, 2, 2 };

        XmlDocument _response;

        public MedDraService()
        {
        }

        #region "Public"

        public string ValidateSourceData(string fileName, string subdirectory)
        {
            _subDirectory = subdirectory;

            var err = false;

            PVIMSDbContext db = new PVIMSDbContext();

            var configValue = db.Configs.Single(c => c.ConfigType == ConfigType.MedDRAVersion).ConfigValue;
            _currentVersion = !String.IsNullOrEmpty(configValue) ? configValue : "Not Set";

            db = null;

            // Validate all files exists
            err = CheckAllFilesExist();
            if (!err) { err = CheckVersion(); };

            // **** SOC
            var file = String.Format("{0}{1}", _subDirectory, _files[5]);
            if (!err) { err = ValidateBase("SOC", file, _elements[5]); };

            // **** HLGT
            file = String.Format("{0}{1}", _subDirectory, _files[4]);
            if (!err) { err = ValidateBase("HLGT", file, _elements[4]); };

            // **** HLT
            file = String.Format("{0}{1}", _subDirectory, _files[3]);
            if (!err) { err = ValidateBase("HLT", file, _elements[3]); };

            // **** PT
            file = String.Format("{0}{1}", _subDirectory, _files[2]);
            if (!err) { err = ValidateBase("PT", file, _elements[2]); };

            // **** LLT
            file = String.Format("{0}{1}", _subDirectory, _files[1]);
            if (!err) { err = ValidateBase("LLT", file, _elements[1]); };

            // **** LINK PT TO HLT
            file = String.Format("{0}{1}", _subDirectory, _files[6]);
            if (!err) { err = ValidateLink("PT", file, _elements[6]); };

            // **** LINK HLT TO HLGT
            file = String.Format("{0}{1}", _subDirectory, _files[7]);
            if (!err) { err = ValidateLink("HLT", file, _elements[7]); };

            // **** LINK HLGT TO SOC
            file = String.Format("{0}{1}", _subDirectory, _files[8]);
            if (!err) { err = ValidateLink("HLGT", file, _elements[8]); };

            // Process response
            if (err) {
                _summary += String.Format("<li>ERROR: Import files not validated successfully...</li>");
            }
            else {
                _summary += String.Format("<li>INFO: Import files validated successfully...</li>");
            }

            return _summary;
        }

        public string ImportSourceData(string fileName, string subdirectory)
        {
            _subDirectory = subdirectory;

            var ns = ""; // urn:pvims-org:v3

            XmlNode rootNode = null;
            XmlNode responseNode;
            XmlNode outerNode;
            XmlNode innerNode;

            XmlAttribute attrib;

            PVIMSDbContext db = new PVIMSDbContext();

            var configValue = db.Configs.Single(c => c.ConfigType == ConfigType.MedDRAVersion).ConfigValue;
            _currentVersion = !String.IsNullOrEmpty(configValue) ? configValue : "Not Set";

            db = null;

            // Prepare for updates
            _movements = new ArrayList();
            _updates = new ArrayList();
            _additions = new ArrayList();
            _refreshCount = 0;

            // Process response
            _response = new XmlDocument();

            try
            {
                // **** SOC
                var file = String.Format("{0}{1}", _subDirectory, _files[5]);
                StoreBase("SOC", file, _elements[5]);

                // **** HLGT
                file = String.Format("{0}{1}", _subDirectory, _files[4]);
                StoreBase("HLGT", file, _elements[4]);

                // **** HLT
                file = String.Format("{0}{1}", _subDirectory, _files[3]);
                StoreBase("HLT", file, _elements[3]);

                // **** PT
                file = String.Format("{0}{1}", _subDirectory, _files[2]);
                StoreBase("PT", file, _elements[2]);

                // **** LLT
                file = String.Format("{0}{1}", _subDirectory, _files[1]);
                StoreBase("LLT", file, _elements[1]);

                // **** LINK PT TO HLT
                file = String.Format("{0}{1}", _subDirectory, _files[6]);
                StoreLink("PT", file, _elements[6]);

                // **** LINK HLT TO HLGT
                file = String.Format("{0}{1}", _subDirectory, _files[7]);
                StoreLink("HLT", file, _elements[7]);

                // **** LINK HLGT TO SOC
                file = String.Format("{0}{1}", _subDirectory, _files[8]);
                StoreLink("HLGT", file, _elements[8]);

            }
            catch (Exception ex)
            {
                rootNode = _response.CreateElement("Status", ns);
                attrib = _response.CreateAttribute("Code");
                attrib.InnerText = "900";
                rootNode.Attributes.Append(attrib);

                responseNode = _response.CreateElement("Description", ns);
                responseNode.InnerText = ex.Message;
                rootNode.AppendChild(responseNode);

                _summary += String.Format("<li>ERROR: Import not completed successfully. Please see log file...</li>");

                _response.AppendChild(rootNode);

                // invalid import
                LogAudit(AuditType.InValidMedDRAImport, String.Format("Unsuccessful meddra import (version {0})", _version), _response.InnerXml);
            }
            finally
            {
                rootNode = _response.CreateElement("Status", ns);
                attrib = _response.CreateAttribute("Code");
                attrib.InnerText = "200";
                rootNode.Attributes.Append(attrib);

                responseNode = _response.CreateElement("Description", ns);
                responseNode.InnerText = "Import completed successfully";
                rootNode.AppendChild(responseNode);

                outerNode = _response.CreateElement("Movements", ns);
                foreach (string item in _movements)
                {
                    innerNode = _response.CreateElement("Movement", ns);
                    attrib = _response.CreateAttribute("MedDRACode");
                    attrib.InnerText = item;
                    innerNode.Attributes.Append(attrib);

                    outerNode.AppendChild(innerNode);
                }
                rootNode.AppendChild(outerNode);
                _summary += String.Format("<li>INFO: # of movements processed ({0})...</li>", _movements.Count.ToString());

                outerNode = _response.CreateElement("Additions", ns);
                foreach (string item in _additions)
                {
                    innerNode = _response.CreateElement("Addition", ns);
                    attrib = _response.CreateAttribute("MedDRACode");
                    attrib.InnerText = item;
                    innerNode.Attributes.Append(attrib);

                    outerNode.AppendChild(innerNode);
                }
                rootNode.AppendChild(outerNode);
                _summary += String.Format("<li>INFO: # of additions processed ({0})...</li>", _additions.Count.ToString());

                outerNode = _response.CreateElement("Updates", ns);
                foreach (string item in _updates)
                {
                    innerNode = _response.CreateElement("Update", ns);
                    attrib = _response.CreateAttribute("MedDRACode");
                    attrib.InnerText = item;
                    innerNode.Attributes.Append(attrib);

                    outerNode.AppendChild(innerNode);
                }
                rootNode.AppendChild(outerNode);
                _summary += String.Format("<li>INFO: # of updates processed ({0})...</li>", _updates.Count.ToString());
                _summary += String.Format("<li>INFO: # of refreshes processed ({0})...</li>", _refreshCount.ToString());

                _summary += String.Format("<li>INFO: Import completed successfully. Please see log file...</li>");

                _response.AppendChild(rootNode);

                // valid import
                LogAudit(AuditType.ValidMedDRAImport, String.Format("Successful meddra import (version {0})", _version), _response.InnerXml);

                // ConfigType.MedDRAVersion
                db = new PVIMSDbContext();

                var config = db.Configs.Single(c => c.ConfigType == ConfigType.MedDRAVersion);
                config.ConfigValue = _version;
                db.SaveChanges();

                db = null;
            }

            // Clean up
            System.IO.Directory.Delete(_subDirectory, true);
            _summary += String.Format("<li>INFO: Extraction cleaned up...</li>");

            return _summary;
        }

        #endregion

        #region "Private"

        private bool CheckAllFilesExist()
        {
            bool allNotExist = false;

            foreach (String fileName in _files)
            {
                var file = String.Format("{0}{1}", _subDirectory, fileName);
                if (File.Exists(file) == false)
                {
                    _summary += String.Format("<li>ERROR: {0} does not exist...</li>", fileName);
                    allNotExist = true;
                }
            }

            if (!allNotExist)
            {
                _summary += String.Format("<li>INFO: All files exist...</li>");
            }

            return allNotExist;
        }

        private bool CheckVersion()
        {
            bool versionNotOk = false;
            decimal tempdec = (decimal)0.00;
            string line;

            System.IO.StreamReader file = new System.IO.StreamReader(_subDirectory + "meddra_release.asc");

            while ((line = file.ReadLine()) != null)
            {
                string[] array = line.Split('$');
                if (array.Length < 2)
                {
                    _summary += String.Format("<li>ERROR: invalid version configuration ({0})...</li>", line);
                    versionNotOk = true;
                }
                else
                {
                    var version = array[0];
                    if (Decimal.TryParse(version, NumberStyles.Any, CultureInfo.InvariantCulture, out tempdec))
                    {
                        _summary += String.Format("<li>INFO: File version for MedDRA ({0})...</li>", version);
                        _version = version;

                        if (_currentVersion != "Not Set")
                        {
                            if (Convert.ToDecimal(_currentVersion) >= tempdec)
                            {
                                _summary += String.Format("<li>ERROR: Current MedDRA version ({0}) same or newer than import file ({1})...</li>", _currentVersion, version);
                                versionNotOk = true;
                                break;
                            }
                        }
                    }
                    else
                    {
                        _summary += String.Format("<li>ERROR: invalid file version number ({0})...</li>", version);
                        versionNotOk = true;
                    }
                }

                break;
            }

            file.Close();
            file = null;

            return versionNotOk;
        }

        private bool ValidateBase(string termType, string fileName, int elementCount)
        {
            System.IO.StreamReader file = new System.IO.StreamReader(fileName);

            bool err = false;

            var lineCount = 0;
            string line;

            string childCode;
            string term;
            string parentCode;

            while ((line = file.ReadLine()) != null)
            {
                lineCount += 1;
                string[] array = line.Split('$');
                if (array.Length < elementCount)
                {
                    _summary += String.Format("<li>ERROR: invalid array length ({0})...</li>", fileName);
                    err = true;
                    break;
                }
                else
                {
                    childCode = "";
                    term = "";
                    parentCode = "";

                    if (elementCount == 2)
                    {
                        childCode = array[0];
                        term = array[1];
                    }
                    if (elementCount == 3)
                    {
                        childCode = array[0];
                        term = array[1];
                        parentCode = array[2];
                    }

                    // Validate array nodes - child code
                    if (!String.IsNullOrEmpty(childCode))
                    {
                        if (Regex.Matches(childCode, @"[0-9]").Count < childCode.Length)
                        {
                            _summary += String.Format("<li>ERROR: Non numeric character in child MedDRA code ({0})(line {1})...</li>", fileName, lineCount.ToString());
                            err = true;
                            break;
                        }
                    }
                    else
                    {
                        _summary += String.Format("<li>ERROR: no child MedDRA code ({0})(line {1})...</li>", fileName, lineCount.ToString());
                        err = true;
                        break;
                    }
                    // Validate array nodes - term
                    if (String.IsNullOrEmpty(term))
                    {
                        _summary += String.Format("<li>ERROR: no term specified ({0})(line {1})...</li>", fileName, lineCount.ToString());
                        err = true;
                        break;
                    }
                    // Validate array nodes - parent code
                    if (!String.IsNullOrEmpty(parentCode))
                    {
                        if (Regex.Matches(parentCode, @"[0-9]").Count < parentCode.Length)
                        {
                            _summary += String.Format("<li>ERROR: Non numeric character in parent MedDRA code ({0})(line {1})...</li>", fileName, lineCount.ToString());
                            err = true;
                            break;
                        }
                    }
                }
            }

            file.Close();
            file = null;

            return err;
        }

        private void StoreBase(string termType, string fileName, int elementCount)
        {
            System.IO.StreamReader file = new System.IO.StreamReader(fileName);

            string parentTermType = "";
            switch (termType)
            {
                case "HLGT":
                    parentTermType = "SOC";
                    break;
                case "HLT":
                    parentTermType = "HLGT";
                    break;
                case "PT":
                    parentTermType = "HLT";
                    break;
                case "LLT":
                    parentTermType = "PT";
                    break;
                default:
                    break;
            }

            var lineCount = 0;
            string line;

            string childCode;
            string term;
            string parentCode;

            while ((line = file.ReadLine()) != null)
            {
                lineCount += 1;
                string[] array = line.Split('$');

                childCode = "";
                term = "";
                parentCode = "";

                if (elementCount == 2)
                {
                    childCode = array[0];
                    term = array[1];
                }
                if (elementCount == 3)
                {
                    childCode = array[0];
                    term = array[1];
                    parentCode = array[2];
                }

                // If we get here, write record to database
                TerminologyMedDra terminology = null;
                TerminologyMedDra parent = null;

                PVIMSDbContext db = new PVIMSDbContext();

                if (String.IsNullOrEmpty(parentCode))
                {
                    terminology = db.TerminologyMedDras.SingleOrDefault(tm => tm.MedDraCode == childCode);
                }
                else
                {
                    terminology = db.TerminologyMedDras.SingleOrDefault(tm => tm.MedDraCode == childCode && tm.Parent.MedDraCode == parentCode);
                    parent = db.TerminologyMedDras.SingleOrDefault(tm => tm.MedDraCode == parentCode && tm.MedDraTermType == parentTermType);
                }

                if (terminology == null)
                {
                    // Addition
                    _additions.Add(childCode);

                    terminology = new TerminologyMedDra()
                    {
                        MedDraCode = childCode,
                        MedDraTerm = term,
                        MedDraTermType = termType,
                        MedDraVersion = _version,
                        Parent = parent
                    };
                    db.TerminologyMedDras.Add(terminology);
                }
                else
                {
                    if (terminology.MedDraTermType.ToLower().Trim() == termType.ToLower().Trim())
                    {
                        if(terminology.MedDraTerm == term) {
                            _refreshCount +=1;
                        }
                        else {
                            _updates.Add(childCode);
                            terminology.MedDraTermType = termType;
                            terminology.MedDraTerm = term;
                            terminology.MedDraVersion = _version;
                            terminology.Parent = parent;
                        }
                    }
                    else {
                        _movements.Add(childCode);
                        terminology.MedDraTermType = termType;
                        terminology.MedDraTerm = term;
                        terminology.MedDraVersion = _version;
                        terminology.Parent = parent;
                    }
                }
                db.SaveChanges();
                db = null;

            } // while ((line = file.ReadLine()) != null)

            file.Close();
            file = null;
        }

        private bool ValidateLink(string termType, string fileName, int elementCount)
        {
            System.IO.StreamReader file = new System.IO.StreamReader(fileName);

            bool err = false;

            var lineCount = 0;
            string line;

            string parentCode;
            string childCode;

            while ((line = file.ReadLine()) != null)
            {
                lineCount += 1;
                string[] array = line.Split('$');
                if (array.Length < elementCount)
                {
                    _summary += String.Format("<li>ERROR: invalid array length ({0})...</li>", fileName);
                    err = true;
                    break;
                }
                else
                {
                    parentCode = "";
                    childCode = "";

                    if (elementCount == 2)
                    {
                        parentCode = array[0];
                        childCode = array[1];
                    }

                    // Validate array nodes - parent code
                    if (!String.IsNullOrEmpty(parentCode))
                    {
                        if (Regex.Matches(parentCode, @"[0-9]").Count < parentCode.Length)
                        {
                            _summary += String.Format("<li>ERROR: Non numeric character in parent MedDRA code ({0})(line {1})...</li>", fileName, lineCount.ToString());
                            err = true;
                            break;
                        }
                    }
                    else
                    {
                        _summary += String.Format("<li>ERROR: no parent MedDRA code ({0})(line {1})...</li>", fileName, lineCount.ToString());
                        err = true;
                        break;
                    }
                    // Validate array nodes - child code
                    if (!String.IsNullOrEmpty(childCode))
                    {
                        if (Regex.Matches(childCode, @"[0-9]").Count < childCode.Length)
                        {
                            _summary += String.Format("<li>ERROR: Non numeric character in child MedDRA code ({0})(line {1})...</li>", fileName, lineCount.ToString());
                            err = true;
                            break;
                        }
                    }
                    else
                    {
                        _summary += String.Format("<li>ERROR: no child MedDRA code ({0})(line {1})...</li>", fileName, lineCount.ToString());
                        err = true;
                        break;
                    }
                }
            }

            file.Close();
            file = null;

            return err;
        }

        private void StoreLink(string termType, string fileName, int elementCount)
        {
            System.IO.StreamReader file = new System.IO.StreamReader(fileName);

            string parentTermType = "";
            switch (termType)
            {
                case "HLGT":
                    parentTermType = "SOC";
                    break;
                case "HLT":
                    parentTermType = "HLGT";
                    break;
                case "PT":
                    parentTermType = "HLT";
                    break;
                case "LLT":
                    parentTermType = "PT";
                    break;
                default:
                    break;
            }

            var lineCount = 0;
            string line;

            string parentCode;
            string childCode;

            while ((line = file.ReadLine()) != null)
            {
                lineCount += 1;
                string[] array = line.Split('$');

                parentCode = "";
                childCode = "";

                if (elementCount == 2)
                {
                    parentCode = array[0];
                    childCode = array[1];
                }

                // Get meddra item
                // If we get here, data is fine
                PVIMSDbContext db = new PVIMSDbContext();

                var terminology = db.TerminologyMedDras.Single(tm => tm.MedDraCode == childCode && tm.MedDraTermType == termType);
                var parent = db.TerminologyMedDras.Single(tm => tm.MedDraCode == parentCode && tm.MedDraTermType == parentTermType);
                terminology.Parent = parent;
                
                db.SaveChanges();
                db = null;
            }

            file.Close();
            file = null;
        }

        private void LogAudit(AuditType type, string details, string log)
        {
            PVIMSDbContext db = new PVIMSDbContext();

            var user = db.Users.Single(u => u.Id == UserContext.Current.User.Id);

            var audit = new AuditLog()
            {
                AuditType = type,
                User = user,
                ActionDate = DateTime.Now,
                Details = details,
                Log = log
            };
            db.AuditLogs.Add(audit);
            db.SaveChanges();

            db = null;
        }

        #endregion
    }
}

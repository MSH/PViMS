using Autofac;
using System;
using System.Collections;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.Services;
using System.Xml;

using VPS.CustomAttributes;

using PVIMS.Core.Entities;
using PVIMS.Core.ValueTypes;
using PVIMS.Entities.EF;

using SelectionDataItem = PVIMS.Core.Entities.SelectionDataItem;
using CustomAttributeConfiguration = PVIMS.Core.Entities.CustomAttributeConfiguration;
using FrameworkCustomAttributeConfiguration = VPS.CustomAttributes.CustomAttributeConfiguration;

namespace PVIMS.Web.Webservice
{
    /// <summary>
    /// Summary description for Interop_Upload
    /// </summary>
    [WebService(Namespace = "http://pvims.org/webservices/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]

    public class InteropGeneric : System.Web.Services.WebService
        {

        PVIMSDbContext _db = new PVIMSDbContext();

        [WebMethod]
        //public string Interop_Push(string subscriber, XmlDocument payload)
        public string Interop_Push(string subscriber, string payload)
        {
            //************************************************
            // Validate subscriber
            //************************************************
            var valid = IsSubscriberValid(subscriber);

            //************************************************
            // Start preparing output
            //************************************************
            XmlDocument source = new XmlDocument();
            XmlDocument response = new XmlDocument();

            var ns = ""; // urn:pvims-org:v3
            DateTime dttemp;

            XmlNode rootNode = null;
            XmlNode responseNode;
            XmlNode urlNode;
            XmlNode errorsNode;
            XmlNode errorNode;

            XmlNodeList patientNodes;
            XmlAttribute attrib;

            XmlDeclaration xmlDeclaration = response.CreateXmlDeclaration("1.0", "UTF-8", null);
            response.AppendChild(xmlDeclaration);

            if(valid == false)
            {
                rootNode = response.CreateElement("Response", ns);
                attrib = response.CreateAttribute("Code");
                attrib.InnerText = "900";
                rootNode.Attributes.Append(attrib);

                responseNode = response.CreateElement("Description", ns);
                responseNode.InnerText = "Invalid subscriber code";
                rootNode.AppendChild(responseNode);

                urlNode = response.CreateElement("URL", ns);
                urlNode.InnerText = "";
                rootNode.AppendChild(urlNode);

                response.AppendChild(rootNode);
            
                return response.InnerXml;
            }

            //************************************************
            // Now process payload
            //************************************************
            
            int errorCount = 0;
            ArrayList patientErrors = new ArrayList();
            ArrayList medicationErrors = new ArrayList();
            ArrayList labErrors = new ArrayList();
            ArrayList conditionErrors = new ArrayList();
            ArrayList eventErrors = new ArrayList();
            ArrayList encounterErrors = new ArrayList();

            try
            {
                source.LoadXml(payload);

                // Validate patient node
                patientNodes = source.SelectNodes("/Patients/Patient");

                foreach(XmlNode patientNode in patientNodes)
                {
                    ValidatePatientNode(patientNode, ref patientErrors);

                    DateTime? dob = DateTime.TryParse(patientNode.Attributes["DateOfBirth"].Value.Trim(), out dttemp) ? Convert.ToDateTime(patientNode.Attributes["DateOfBirth"].Value.Trim()) : (DateTime?)null;

                    ValidateMedications(patientNode, ref medicationErrors, dob);
                    ValidateLabTests(patientNode, ref labErrors, dob);
                    ValidateConditions(patientNode, ref conditionErrors, dob);
                    ValidateEvents(patientNode, ref eventErrors, dob);
                    ValidateEncounters(patientNode, ref encounterErrors, dob);
                }

                errorCount = patientErrors.Count + medicationErrors.Count + labErrors.Count + conditionErrors.Count + eventErrors.Count + encounterErrors.Count;
                if(errorCount == 0)
                {
                    var pid = 0;

                    foreach(XmlNode patientNode in patientNodes)
                    {
                        Patient patient = null;

                        Guid guid = new Guid(patientNode.Attributes["guid"].Value.Trim());
                        patient = _db.Patients.SingleOrDefault(u => u.PatientGuid == guid);

                        // Does patient exist already
                        if(patient == null) {
                            patient = CreatePatient(patientNode);
                        }
                        else {
                            UpdatePatient(patientNode, ref patient);
                        }
                        UpdatePatientAttributes(patientNode, ref patient);
                        pid = patient.Id;

                        Patient tempPatient = null;
                        var id = patient.Id;
                        tempPatient = _db.Patients.SingleOrDefault(u => u.Id == id);

                        XmlNodeList medicationNodes = patientNode.SelectNodes("Medications/Medication");
                        foreach (XmlNode medicationNode in medicationNodes)
                        {
                            PatientMedication patientMedication = null;

                            guid = new Guid(medicationNode.Attributes["guid"].Value.Trim());
                            patientMedication = _db.PatientMedications.Include(pm1 => pm1.Patient).Include(pm2 => pm2.Medication).SingleOrDefault(u => u.PatientMedicationGuid == guid);

                            if (patientMedication == null)
                            {
                                patientMedication = CreatePatientMedication(medicationNode, tempPatient);
                            }
                            else
                            {
                                UpdatePatientMedication(medicationNode, ref patientMedication);
                            }
                            UpdatePatientMedicationAttributes(medicationNode, ref patientMedication);
                        }

                        XmlNodeList conditionNodes = patientNode.SelectNodes("Conditions/Condition");
                        foreach (XmlNode conditionNode in conditionNodes)
                        {
                            PatientCondition patientCondition = null;

                            guid = new Guid(conditionNode.Attributes["guid"].Value.Trim());
                            patientCondition = _db.PatientConditions.Include(pc1 => pc1.Patient).Include(pc2 => pc2.TerminologyMedDra).Include(pc3 => pc3.Outcome).Include(pc4 => pc4.TreatmentOutcome).SingleOrDefault(u => u.PatientConditionGuid == guid);

                            if (patientCondition == null)
                            {
                                patientCondition = CreatePatientCondition(conditionNode, tempPatient);
                            }
                            else
                            {
                                UpdatePatientCondition(conditionNode, ref patientCondition);
                            }
                            UpdatePatientConditionAttributes(conditionNode, ref patientCondition);
                        }

                        XmlNodeList labNodes = patientNode.SelectNodes("LabTests/LabTest");
                        foreach (XmlNode labNode in labNodes)
                        {
                            PatientLabTest patientLabTest = null;

                            guid = new Guid(labNode.Attributes["guid"].Value.Trim());
                            patientLabTest = _db.PatientLabTests.Include(plt1 => plt1.Patient).Include(plt2 => plt2.LabTest).Include(plt3 => plt3.TestUnit).SingleOrDefault(u => u.PatientLabTestGuid == guid);

                            if (patientLabTest == null)
                            {
                                patientLabTest = CreatePatientLabTest(labNode, tempPatient);
                            }
                            else
                            {
                                UpdatePatientLabTest(labNode, ref patientLabTest);
                            }
                            UpdatePatientLabTestAttributes(labNode, ref patientLabTest);
                        }

                        XmlNodeList eventNodes = patientNode.SelectNodes("ClinicalEvents/ClinicalEvent");
                        foreach (XmlNode eventNode in eventNodes)
                        {
                            PatientClinicalEvent patientClinicalEvent = null;

                            guid = new Guid(eventNode.Attributes["guid"].Value.Trim());
                            patientClinicalEvent = _db.PatientClinicalEvents.Include(pce1 => pce1.Patient).Include(pce2 => pce2.SourceTerminologyMedDra).SingleOrDefault(u => u.PatientClinicalEventGuid == guid);

                            if (patientClinicalEvent == null)
                            {
                                patientClinicalEvent = CreatePatientClinicalEvent(eventNode, tempPatient);
                            }
                            else
                            {
                                UpdatePatientClinicalEvent(eventNode, ref patientClinicalEvent);
                            }
                            UpdatePatientClinicalEventAttributes(eventNode, ref patientClinicalEvent);
                        }

                        XmlNodeList encounterNodes = patientNode.SelectNodes("Encounters/Encounter");
                        foreach (XmlNode encounterNode in encounterNodes)
                        {
                            Encounter encounter = null;

                            guid = new Guid(encounterNode.Attributes["guid"].Value.Trim());
                            encounter = _db.Encounters.Include(e1 => e1.Patient).Include(e2 => e2.EncounterType).Include(e3 => e3.Priority).SingleOrDefault(u => u.EncounterGuid == guid);

                            if (encounter == null)
                            {
                                encounter = CreateEncounter(encounterNode, tempPatient);
                            }
                            else
                            {
                                UpdateEncounter(encounterNode, ref encounter);
                            }
                            UpdateEncounterValues(encounterNode, ref encounter);
                        }
                    }

                    // Response
                    rootNode = response.CreateElement("Response", ns);
                    attrib = response.CreateAttribute("Code");
                    attrib.InnerText = "200";
                    rootNode.Attributes.Append(attrib);

                    responseNode = response.CreateElement("Description", ns);
                    responseNode.InnerText = "Update completed successfully";
                    rootNode.AppendChild(responseNode);

                    urlNode = response.CreateElement("URL", ns);
                    urlNode.InnerText = String.Format("/Patient/PatientView.aspx?pid={0}", pid.ToString());
                    rootNode.AppendChild(urlNode);

                    response.AppendChild(rootNode);

                    // valid post
                    LogAudit(AuditType.ValidSubscriberPost, String.Format("Successful subscriber post ({0})", subscriber.Trim()), source.InnerXml);
                }
                else
                {
                    // Response
                    rootNode = response.CreateElement("Response", ns);
                    attrib = response.CreateAttribute("Code");
                    attrib.InnerText = "300";
                    rootNode.Attributes.Append(attrib);

                    responseNode = response.CreateElement("Description", ns);
                    responseNode.InnerText = "Update rejected. Please see error list.";
                    rootNode.AppendChild(responseNode);

                    errorsNode = response.CreateElement("PatientErrors", ns);
                    foreach(var err in patientErrors)
                    {
                        errorNode = response.CreateElement("Error", ns);
                        attrib = response.CreateAttribute("Description");
                        attrib.InnerText = err.ToString();
                        errorNode.Attributes.Append(attrib);

                        errorsNode.AppendChild(errorNode);
                    }
                    rootNode.AppendChild(errorsNode);

                    errorsNode = response.CreateElement("MedicationErrors", ns);
                    foreach (var err in medicationErrors)
                    {
                        errorNode = response.CreateElement("Error", ns);
                        attrib = response.CreateAttribute("Description");
                        attrib.InnerText = err.ToString();
                        errorNode.Attributes.Append(attrib);

                        errorsNode.AppendChild(errorNode);
                    }
                    rootNode.AppendChild(errorsNode);

                    errorsNode = response.CreateElement("LabErrors", ns);
                    foreach (var err in labErrors)
                    {
                        errorNode = response.CreateElement("Error", ns);
                        attrib = response.CreateAttribute("Description");
                        attrib.InnerText = err.ToString();
                        errorNode.Attributes.Append(attrib);

                        errorsNode.AppendChild(errorNode);
                    }
                    rootNode.AppendChild(errorsNode);

                    errorsNode = response.CreateElement("ConditionErrors", ns);
                    foreach (var err in conditionErrors)
                    {
                        errorNode = response.CreateElement("Error", ns);
                        attrib = response.CreateAttribute("Description");
                        attrib.InnerText = err.ToString();
                        errorNode.Attributes.Append(attrib);

                        errorsNode.AppendChild(errorNode);
                    }
                    rootNode.AppendChild(errorsNode);

                    errorsNode = response.CreateElement("EventErrors", ns);
                    foreach (var err in eventErrors)
                    {
                        errorNode = response.CreateElement("Error", ns);
                        attrib = response.CreateAttribute("Description");
                        attrib.InnerText = err.ToString();
                        errorNode.Attributes.Append(attrib);

                        errorsNode.AppendChild(errorNode);
                    }
                    rootNode.AppendChild(errorsNode);

                    errorsNode = response.CreateElement("EncounterErrors", ns);
                    foreach (var err in encounterErrors)
                    {
                        errorNode = response.CreateElement("Error", ns);
                        attrib = response.CreateAttribute("Description");
                        attrib.InnerText = err.ToString();
                        errorNode.Attributes.Append(attrib);

                        errorsNode.AppendChild(errorNode);
                    }
                    rootNode.AppendChild(errorsNode);

                    urlNode = response.CreateElement("URL", ns);
                    urlNode.InnerText = "";
                    rootNode.AppendChild(urlNode);

                    response.AppendChild(rootNode);

                    // valid post
                    LogAudit(AuditType.InValidSubscriberPost, String.Format("Unsuccessful subscriber post ({0}) ({1})", subscriber.Trim(), response.InnerXml), source.InnerXml);
                }
            }
            catch (Exception ex)
            {
                StringBuilder message = new StringBuilder();
                if (ex is DbEntityValidationException)
                {
                    message.Append(ex.Message.ToString());
                    foreach (var eve in ((DbEntityValidationException)ex).EntityValidationErrors)
                    {
                        foreach (var ve in eve.ValidationErrors)
                        {
                            message.AppendFormat("- Property: \"{0}\", Error: \"{1}\"", ve.PropertyName, ve.ErrorMessage);
                        }
                    }
                }
                else
                {
                    message.Append(ex.Message.ToString());
                }

                rootNode = response.CreateElement("Response", ns);
                attrib = response.CreateAttribute("Code");
                attrib.InnerText = "900";
                rootNode.Attributes.Append(attrib);

                responseNode = response.CreateElement("Description", ns);
                responseNode.InnerText = message.ToString();
                rootNode.AppendChild(responseNode);

                urlNode = response.CreateElement("URL", ns);
                urlNode.InnerText = "";
                rootNode.AppendChild(urlNode);

                response.AppendChild(rootNode);

                // valid post
                LogAudit(AuditType.InValidSubscriberPost, String.Format("Unsuccessful subscriber post ({0}) ({1})", subscriber.Trim(), response.InnerXml), source.InnerXml);
            }

            return response.InnerXml;
        }

        #region "Validation"

        private bool IsSubscriberValid(string subscriber)
        {
            // ConfigType.WebServiceSubscriberList
            var configValue = _db.Configs.Single(c => c.ConfigType == ConfigType.WebServiceSubscriberList).ConfigValue;
            string[] validValues = null;

            if (configValue.Contains(",")) {
                validValues = configValue.Split(',');
            }
            else
            {
                validValues = new string[1];
                validValues[0] = configValue;
            }

            if(validValues.ToArray().Any(v => v.Trim() == subscriber.Trim()))
            {
                // valid access
                LogAudit(AuditType.ValidSubscriberAccess, String.Format("Successful subscriber access ({0})", subscriber.Trim()), "");
                return true;
            }
            else
            {
                // invalid access
                LogAudit(AuditType.InvalidSubscriberAccess, String.Format("Unsuccessful subscriber access ({0})", subscriber.Trim()), "");
                return false;
            }
        }

        private void ValidatePatientNode(XmlNode patientNode, ref ArrayList errors)
        {
            DateTime dttemp;

            // Validate first class attributes
            // ************** FACILITY
            XmlAttribute attrib = patientNode.Attributes["Facility"];
            if (attrib == null)
            {
                errors.Add("Facility attribute is missing");
            }
            else
            {
                if (String.IsNullOrEmpty(attrib.Value))
                {
                    errors.Add("Facility is required");
                }
                else
                {
                    var facility = _db.Facilities.SingleOrDefault(u => u.FacilityName == attrib.Value.Trim());
                    if (facility == null)
                    {
                        errors.Add("Facility not defined");
                    }
                }
            }

            // ************** DATE OF BIRTH
            attrib = patientNode.Attributes["DateOfBirth"];
            if (attrib == null)
            {
                errors.Add("DateOfBirth attribute is missing");
            }
            else
            {
                if (String.IsNullOrEmpty(attrib.Value))
                {
                    errors.Add("Date Of Birth is required");
                }
                else
                {
                    if (DateTime.TryParse(attrib.Value, out dttemp))
                    {
                        dttemp = Convert.ToDateTime(attrib.Value);
                        if (dttemp > DateTime.Today)
                        {
                            errors.Add("Date of Birth should be before current date");
                        }
                        if (dttemp < DateTime.Today.AddYears(-120))
                        {
                            errors.Add("Date of Birth cannot be so far in the past");
                        }
                    }
                    else
                    {
                        errors.Add("Date of Birth has an invalid date format");
                    }
                }
            }

            // ************** MIDDLE NAME
            attrib = patientNode.Attributes["MiddleName"];
            if (attrib != null)
            {
                if (!String.IsNullOrEmpty(attrib.Value))
                {
                    var middle = attrib.Value;
                    if (Regex.Matches(middle, @"[a-zA-Z]").Count < middle.Length)
                    {
                        errors.Add("Middle Name contains invalid characters (Enter A-Z, a-z)");
                    }
                }
            }

            // ************** SURNAME
            attrib = patientNode.Attributes["Surname"];
            if (attrib == null)
            {
                errors.Add("Surname attribute is missing");
            }
            else
            {
                if (String.IsNullOrEmpty(attrib.Value))
                {
                    errors.Add("Surname is required");
                }
                else
                {
                    var surname = attrib.Value;
                    if (Regex.Matches(surname, @"[a-zA-Z]").Count < surname.Length)
                    {
                        errors.Add("Last Name contains invalid characters (Enter A-Z, a-z)");
                    }
                }
            }

            // ************** FIRSTNAME
            attrib = patientNode.Attributes["FirstName"];
            if (attrib == null)
            {
                errors.Add("FirstName attribute is missing");
            }
            else
            {
                if (String.IsNullOrEmpty(attrib.Value))
                {
                    errors.Add("First name is required");
                }
                else
                {
                    var firstName = attrib.Value;
                    if (Regex.Matches(firstName, @"[a-zA-Z]").Count < firstName.Length)
                    {
                        errors.Add("First Name contains invalid characters (Enter A-Z, a-z)");
                    }
                }
            }

            // Validate custom attributes
            CustomAttributeConfiguration con;

            XmlNodeList attributeNodes = patientNode.SelectNodes("Customattributes/AttributeKey");
            foreach (XmlNode attributeNode in attributeNodes)
            {
                // Get attribute
                attrib = attributeNode.Attributes["Name"];
                if (attrib == null)
                {
                    errors.Add("Name attribute missing for custom attribute");
                    continue;
                }

                con = _db.CustomAttributeConfigurations.SingleOrDefault(u => u.AttributeKey.Trim() == attrib.Value.Trim());
                if (con == null)
                {
                    errors.Add(String.Format("Unable to load configuration for custom attribute ({0})", attrib.Value));
                    continue;
                }

                var val = attributeNode.InnerText;
                if (string.IsNullOrWhiteSpace(val) && con.IsRequired)
                {
                    errors.Add(String.Format("{0} is required", con.AttributeKey));
                }

                if (!string.IsNullOrWhiteSpace(val) && con.AttributeKey == "Medical Record Number") 
                {
                    if (Regex.Matches(val, @"[a-zA-Z0-9-]").Count < val.Length)
                    {
                        errors.Add(String.Format("{0} contains invalid characters (Enter A-Z, a-z, 0-9, -)", con.AttributeKey));
                    }
                }
                if (!string.IsNullOrWhiteSpace(val) && con.AttributeKey == "Patient Identity Number") 
                {
                    if (Regex.Matches(val, @"[a-zA-Z0-9]").Count < val.Length)
                    {
                        errors.Add(String.Format("{0} contains invalid characters (Enter A-Z, a-z, 0-9)", con.AttributeKey));
                    }
                }
                if (!string.IsNullOrWhiteSpace(val) && con.AttributeKey == "Patient Contact Number")
                {
                    if (!Regex.IsMatch(val, @"^[0-9+]*$"))
                    {
                        errors.Add(String.Format("{0} contains invalid characters (Enter 0-9, +)", con.AttributeKey));
                    }
                }
                if (!string.IsNullOrWhiteSpace(val) && con.CustomAttributeType == CustomAttributeType.DateTime)
                {
                    if (!DateTime.TryParse(val, out dttemp))
                    {
                        errors.Add(String.Format("{0} has an invalid date format", con.AttributeKey));
                    }
                }
                if (!string.IsNullOrWhiteSpace(val) && con.CustomAttributeType == CustomAttributeType.Selection)
                {
                    var sel = _db.SelectionDataItems.SingleOrDefault(u => u.AttributeKey == con.AttributeKey && u.Value == val.Trim());
                    if (sel == null)
                    {
                        errors.Add(String.Format("Invalid selection value specified for ({0})", con.AttributeKey));
                    }
                }
            }

        }

        private void ValidateMedications(XmlNode patientNode, ref ArrayList errors, DateTime? dob)
        {
            DateTime dttemp;

            XmlNodeList medicationNodes = patientNode.SelectNodes("Medications/Medication");

            foreach (XmlNode medicationNode in medicationNodes)
            {
                // Validate first class attributes
                // ************** GUID
                XmlAttribute attrib = medicationNode.Attributes["guid"];
                if (attrib == null)
                {
                    errors.Add("Guid attribute is missing");
                }
                else
                {
                    if (String.IsNullOrEmpty(attrib.Value))
                    {
                        errors.Add("Guid is required");
                    }
                }

                // ************** MEDICATION
                attrib = medicationNode.Attributes["Medication"];
                if (attrib == null)
                {
                    errors.Add("Medication attribute is missing");
                }
                else
                {
                    if (String.IsNullOrEmpty(attrib.Value))
                    {
                        errors.Add("Medication is required");
                    }
                    else
                    {
                        var medication = _db.Medications.SingleOrDefault(u => u.DrugName == attrib.Value.Trim());
                        if (medication == null)
                        {
                            errors.Add("Medication not defined");
                        }
                    }
                }

                // ************** DATE START
                DateTime? dateStart = DateTime.Today;
                attrib = medicationNode.Attributes["DateStart"];
                if (attrib == null)
                {
                    errors.Add("DateStart attribute is missing");
                }
                else
                {
                    if (String.IsNullOrEmpty(attrib.Value))
                    {
                        errors.Add("Date Start is required");
                    }
                    else
                    {
                        if (DateTime.TryParse(attrib.Value, out dttemp))
                        {
                            dttemp = Convert.ToDateTime(attrib.Value);
                            dateStart = dttemp;

                            if(dob != null)
                            {
                                if (dttemp < Convert.ToDateTime(dob))
                                {
                                    errors.Add("Start Date should be after Date Of Birth");
                                }
                            }
                        }
                        else
                        {
                            errors.Add("Date Start has an invalid date format");
                        }
                    }
                }

                // ************** DATE END
                attrib = medicationNode.Attributes["DateEnd"];
                if (attrib == null)
                {
                    errors.Add("DateEnd attribute is missing");
                }
                else
                {
                    if (!String.IsNullOrEmpty(attrib.Value))
                    {
                        if (DateTime.TryParse(attrib.Value, out dttemp))
                        {
                            dttemp = Convert.ToDateTime(attrib.Value);
                            if (dttemp > DateTime.Today)
                            {
                                errors.Add("End Date should be before current date");
                            }
                            if (dttemp < dateStart)
                            {
                                errors.Add("End Date should be after Start Date");
                            }
                        }
                        else
                        {
                            errors.Add("Date End has an invalid date format");
                        }
                    }
                }

                // ************** DOSEUNIT
                attrib = medicationNode.Attributes["DoseUnit"];
                if (attrib == null)
                {
                    errors.Add("DoseUnit attribute is missing");
                }

                // ************** DOSEFREQUENCY
                attrib = medicationNode.Attributes["DoseFrequency"];
                if (attrib == null)
                {
                    errors.Add("DoseFrequency attribute is missing");
                }

                // ************** DOSE
                attrib = medicationNode.Attributes["Dose"];
                if (attrib == null)
                {
                    errors.Add("Dose attribute is missing");
                }

                // Validate custom attributes
                CustomAttributeConfiguration con;

                XmlNodeList attributeNodes = medicationNode.SelectNodes("Customattributes/AttributeKey");
                foreach (XmlNode attributeNode in attributeNodes)
                {
                    // Get attribute
                    attrib = attributeNode.Attributes["Name"];
                    if (attrib == null)
                    {
                        errors.Add("Name attribute missing for custom attribute");
                        continue;
                    }

                    con = _db.CustomAttributeConfigurations.SingleOrDefault(u => u.AttributeKey.Trim() == attrib.Value.Trim());
                    if (con == null)
                    {
                        errors.Add(String.Format("Unable to load configuration for custom attribute ({0})", attrib.Value));
                        continue;
                    }

                    var val = attributeNode.InnerText;
                    if (string.IsNullOrWhiteSpace(val) && con.IsRequired)
                    {
                        errors.Add(String.Format("{0} is required", con.AttributeKey));
                    }

                    if (!string.IsNullOrWhiteSpace(val) && con.CustomAttributeType == CustomAttributeType.DateTime)
                    {
                        if (!DateTime.TryParse(val, out dttemp))
                        {
                            errors.Add(String.Format("{0} has an invalid date format", con.AttributeKey));
                        }
                    }
                    if (!string.IsNullOrWhiteSpace(val) && con.CustomAttributeType == CustomAttributeType.Selection)
                    {
                        var sel = _db.SelectionDataItems.SingleOrDefault(u => u.AttributeKey == con.AttributeKey && u.Value == val.Trim());
                        if (sel == null)
                        {
                            errors.Add(String.Format("Invalid selection value specified for ({0})", con.AttributeKey));
                        }
                    }
                }
            }
        }

        private void ValidateLabTests(XmlNode patientNode, ref ArrayList errors, DateTime? dob)
        {
            DateTime dttemp;
            Decimal dectemp;

            XmlNodeList labNodes = patientNode.SelectNodes("LabTests/LabTest");

            foreach (XmlNode labNode in labNodes)
            {
                // Validate first class attributes
                // ************** GUID
                XmlAttribute attrib = labNode.Attributes["guid"];
                if (attrib == null)
                {
                    errors.Add("Guid attribute is missing");
                }
                else
                {
                    if (String.IsNullOrEmpty(attrib.Value))
                    {
                        errors.Add("Guid is required");
                    }
                }

                // ************** TEST
                attrib = labNode.Attributes["Test"];
                if (attrib == null)
                {
                    errors.Add("Test attribute is missing");
                }
                else
                {
                    if (String.IsNullOrEmpty(attrib.Value))
                    {
                        errors.Add("Test is required");
                    }
                    else
                    {
                        var labTest = _db.LabTests.SingleOrDefault(u => u.Description == attrib.Value.Trim());
                        if (labTest == null)
                        {
                            errors.Add("Test not defined");
                        }
                    }
                }

                // ************** DATE TEST
                attrib = labNode.Attributes["TestDate"];
                if (attrib == null)
                {
                    errors.Add("TestDate attribute is missing");
                }
                else
                {
                    if (String.IsNullOrEmpty(attrib.Value))
                    {
                        errors.Add("Test Date is required");
                    }
                    else
                    {
                        if (DateTime.TryParse(attrib.Value, out dttemp))
                        {
                            dttemp = Convert.ToDateTime(attrib.Value);

                            if (dob != null)
                            {
                                if (dttemp < Convert.ToDateTime(dob))
                                {
                                    errors.Add("Test Date should be after Date Of Birth");
                                }
                            }
                        }
                        else
                        {
                            errors.Add("Test Date has an invalid date format");
                        }
                    }
                }

                // ************** TESTRESULTVALUE
                attrib = labNode.Attributes["TestResultValue"];
                if (attrib == null)
                {
                    errors.Add("TestResultValue attribute is missing");
                }
                else
                {
                    if (!String.IsNullOrEmpty(attrib.Value))
                    {
                        if (!Decimal.TryParse(attrib.Value, out dectemp))
                        {
                            errors.Add("Test result value is not a valid decimal");
                        }
                    }
                }

                // ************** TESTUNIT
                attrib = labNode.Attributes["TestUnit"];
                if (attrib == null)
                {
                    errors.Add("TestUnit attribute is missing");
                }
                else
                {
                    if (!String.IsNullOrEmpty(attrib.Value))
                    {
                        var testUnit = _db.LabTestUnits.SingleOrDefault(u => u.Description == attrib.Value.Trim());
                        if (testUnit == null)
                        {
                            errors.Add("Test unit not defined");
                        }
                    }
                }

                // ************** TESTRESULTCODED
                attrib = labNode.Attributes["TestResultCoded"];
                if (attrib == null)
                {
                    errors.Add("TestResultCoded attribute is missing");
                }
                else
                {
                    if (!String.IsNullOrEmpty(attrib.Value))
                    {
                        var testResult = _db.LabResults.SingleOrDefault(u => u.Description == attrib.Value.Trim());
                        if (testResult == null)
                        {
                            errors.Add("Test result code not defined");
                        }
                    }
                }

                // Validate custom attributes
                CustomAttributeConfiguration con;

                XmlNodeList attributeNodes = labNode.SelectNodes("Customattributes/AttributeKey");
                foreach (XmlNode attributeNode in attributeNodes)
                {
                    // Get attribute
                    attrib = attributeNode.Attributes["Name"];
                    if (attrib == null)
                    {
                        errors.Add("Name attribute missing for custom attribute");
                        continue;
                    }

                    con = _db.CustomAttributeConfigurations.SingleOrDefault(u => u.AttributeKey.Trim() == attrib.Value.Trim());
                    if (con == null)
                    {
                        errors.Add(String.Format("Unable to load configuration for custom attribute ({0})", attrib.Value));
                        continue;
                    }

                    var val = attributeNode.InnerText;
                    if (string.IsNullOrWhiteSpace(val) && con.IsRequired)
                    {
                        errors.Add(String.Format("{0} is required", con.AttributeKey));
                    }

                    if (!string.IsNullOrWhiteSpace(val) && con.CustomAttributeType == CustomAttributeType.DateTime)
                    {
                        if (!DateTime.TryParse(val, out dttemp))
                        {
                            errors.Add(String.Format("{0} has an invalid date format", con.AttributeKey));
                        }
                    }
                    if (!string.IsNullOrWhiteSpace(val) && con.CustomAttributeType == CustomAttributeType.Selection)
                    {
                        var sel = _db.SelectionDataItems.SingleOrDefault(u => u.AttributeKey == con.AttributeKey && u.Value == val.Trim());
                        if (sel == null)
                        {
                            errors.Add(String.Format("Invalid selection value specified for ({0})", con.AttributeKey));
                        }
                    }
                }
            }
        }

        private void ValidateConditions(XmlNode patientNode, ref ArrayList errors, DateTime? dob)
        {
            DateTime dttemp;
            bool conditionGroupFound = false;

            XmlNodeList conditionNodes = patientNode.SelectNodes("Conditions/Condition");

            foreach (XmlNode conditionNode in conditionNodes)
            {
                // Validate first class attributes
                // ************** GUID
                XmlAttribute attrib = conditionNode.Attributes["guid"];
                if (attrib == null)
                {
                    errors.Add("Guid attribute is missing");
                }
                else
                {
                    if (String.IsNullOrEmpty(attrib.Value))
                    {
                        errors.Add("Guid is required");
                    }
                }

                // ************** TERMINOLOGY
                attrib = conditionNode.Attributes["Terminology"];
                if (attrib == null)
                {
                    errors.Add("Terminology attribute is missing");
                }
                else
                {
                    if (String.IsNullOrEmpty(attrib.Value))
                    {
                        errors.Add("Terminology is required");
                    }
                    else
                    {
                        var meddra = _db.TerminologyMedDras.SingleOrDefault(u => u.MedDraCode == attrib.Value.Trim() && u.MedDraTermType == "LLT");
                        if (meddra == null)
                        {
                            errors.Add("Terminology not defined");
                        }
                        // Patient must be bound to at least one condition group
                        if(meddra.ConditionMedDras.Count > 0)
                        {
                            conditionGroupFound = true;
                        }
                    }
                }

                // ************** OUTCOME
                Outcome outcome = null;
                attrib = conditionNode.Attributes["Outcome"];
                if (attrib == null)
                {
                    errors.Add("Outcome attribute is missing");
                }
                else
                {
                    if (!String.IsNullOrEmpty(attrib.Value))
                    {
                        outcome = _db.Outcomes.SingleOrDefault(u => u.Description == attrib.Value.Trim());
                        if (outcome == null)
                        {
                            errors.Add("Outcome not defined");
                        }
                    }
                }

                // ************** TREATMENTOUTCOME
                TreatmentOutcome treatmentOutcome = null;
                attrib = conditionNode.Attributes["TreatmentOutcome"];
                if (attrib == null)
                {
                    errors.Add("TreatmentOutcome attribute is missing");
                }
                else
                {
                    if (!String.IsNullOrEmpty(attrib.Value))
                    {
                        treatmentOutcome = _db.TreatmentOutcomes.SingleOrDefault(u => u.Description == attrib.Value.Trim());
                        if (treatmentOutcome == null)
                        {
                            errors.Add("Treatment Outcome not defined");
                        }
                    }
                }

                // ************** DATE START
                DateTime? dateStart = DateTime.Today;
                attrib = conditionNode.Attributes["DateStart"];
                if (attrib == null)
                {
                    errors.Add("DateStart attribute is missing");
                }
                else
                {
                    if (String.IsNullOrEmpty(attrib.Value))
                    {
                        errors.Add("Date Start is required");
                    }
                    else
                    {
                        if (DateTime.TryParse(attrib.Value, out dttemp))
                        {
                            dttemp = Convert.ToDateTime(attrib.Value);
                            dateStart = dttemp;

                            if (dttemp > DateTime.Today)
                            {
                                errors.Add("Start Date should be before current date");
                            }
                            if (dob != null)
                            {
                                if (dttemp < Convert.ToDateTime(dob))
                                {
                                    errors.Add("Start Date should be after Date Of Birth");
                                }
                            }
                        }
                        else
                        {
                            errors.Add("Date Start has an invalid date format");
                        }
                    }
                }

                // ************** DATE END
                DateTime? dateEnd = DateTime.Today;
                attrib = conditionNode.Attributes["DateEnd"];
                if (attrib == null)
                {
                    errors.Add("DateEnd attribute is missing");
                }
                else
                {
                    if (!String.IsNullOrEmpty(attrib.Value))
                    {
                        if (DateTime.TryParse(attrib.Value, out dttemp))
                        {
                            dttemp = Convert.ToDateTime(attrib.Value);
                            dateEnd = dttemp;

                            if (dttemp > DateTime.Today)
                            {
                                errors.Add("End Date should be before current date");
                            }
                            if (dttemp < dateStart)
                            {
                                errors.Add("End Date should be after Start Date");
                            }
                        }
                        else
                        {
                            errors.Add("Date End has an invalid date format");
                        }
                    }
                    else
                    {
                        if(outcome != null)
                        {
                            errors.Add("Condition Outcome Date is mandatory if Condition Outcome is set");
                        }
                    }
                }

                if (outcome != null || treatmentOutcome != null)
                {
                    var outcomeVal = outcome != null ? outcome.Description : "";
                    var treatmentOutcomeVal = treatmentOutcome != null ? treatmentOutcome.Description : "";

                    if (outcomeVal == "Fatal" && treatmentOutcomeVal != "Died")
                    {
                        errors.Add("Treatment Outcome not consistent with Condition Outcome");
                    }
                    if (outcomeVal != "Fatal" && treatmentOutcomeVal == "Died")
                    {
                        errors.Add("Condition Outcome not consistent with Treatment Outcome");
                    }
                }

                // Validate custom attributes
                CustomAttributeConfiguration con;

                XmlNodeList attributeNodes = conditionNode.SelectNodes("Customattributes/AttributeKey");
                foreach (XmlNode attributeNode in attributeNodes)
                {
                    // Get attribute
                    attrib = attributeNode.Attributes["Name"];
                    if (attrib == null)
                    {
                        errors.Add("Name attribute missing for custom attribute");
                        continue;
                    }

                    con = _db.CustomAttributeConfigurations.SingleOrDefault(u => u.AttributeKey.Trim() == attrib.Value.Trim());
                    if (con == null)
                    {
                        errors.Add(String.Format("Unable to load configuration for custom attribute ({0})", attrib.Value));
                        continue;
                    }

                    var val = attributeNode.InnerText;
                    if (string.IsNullOrWhiteSpace(val) && con.IsRequired)
                    {
                        errors.Add(String.Format("{0} is required", con.AttributeKey));
                    }

                    if (!string.IsNullOrWhiteSpace(val) && con.CustomAttributeType == CustomAttributeType.DateTime)
                    {
                        if (!DateTime.TryParse(val, out dttemp))
                        {
                            errors.Add(String.Format("{0} has an invalid date format", con.AttributeKey));
                        }
                    }
                    if (!string.IsNullOrWhiteSpace(val) && con.CustomAttributeType == CustomAttributeType.Selection)
                    {
                        var sel = _db.SelectionDataItems.SingleOrDefault(u => u.AttributeKey == con.AttributeKey && u.Value == val.Trim());
                        if (sel == null)
                        {
                            errors.Add(String.Format("Invalid selection value specified for ({0})", con.AttributeKey));
                        }
                    }
                }
            } // foreach (XmlNode conditionNode in conditionNodes)

            if(!conditionGroupFound)
            {
                errors.Add("No condition group found. Patient will not form part of analysis.");
            }
        }

        private void ValidateEvents(XmlNode patientNode, ref ArrayList errors, DateTime? dob)
        {
            DateTime dttemp;

            XmlNodeList eventNodes = patientNode.SelectNodes("ClinicalEvents/ClinicalEvent");

            foreach (XmlNode eventNode in eventNodes)
            {
                // Validate first class attributes
                // ************** GUID
                XmlAttribute attrib = eventNode.Attributes["guid"];
                if (attrib == null)
                {
                    errors.Add("Guid attribute is missing");
                }
                else
                {
                    if (String.IsNullOrEmpty(attrib.Value))
                    {
                        errors.Add("Guid is required");
                    }
                }

                // ************** SOURCE TERMINOLOGY
                attrib = eventNode.Attributes["SourceTerminology"];
                if (attrib == null)
                {
                    errors.Add("SourceTerminology attribute is missing");
                }
                else
                {
                    if (String.IsNullOrEmpty(attrib.Value))
                    {
                        errors.Add("Source terminology is required");
                    }
                    else
                    {
                        var meddra = _db.TerminologyMedDras.SingleOrDefault(u => u.MedDraCode == attrib.Value.Trim() && u.MedDraTermType == "LLT");
                        if (meddra == null)
                        {
                            errors.Add("Source terminology not defined");
                        }
                    }
                }

                // ************** SOURCE DESCRIPTION
                attrib = eventNode.Attributes["SourceDescription"];
                if (attrib == null)
                {
                    errors.Add("SourceDescription attribute is missing");
                }
                else
                {
                    if (String.IsNullOrEmpty(attrib.Value))
                    {
                        errors.Add("Source description is required");
                    }
                }

                // ************** DATE ONSET
                DateTime? dateOnset = DateTime.Today;
                attrib = eventNode.Attributes["OnsetDate"];
                if (attrib == null)
                {
                    errors.Add("OnsetDate attribute is missing");
                }
                else
                {
                    if (String.IsNullOrEmpty(attrib.Value))
                    {
                        errors.Add("Onset Date is required");
                    }
                    else
                    {
                        if (DateTime.TryParse(attrib.Value, out dttemp))
                        {
                            dttemp = Convert.ToDateTime(attrib.Value);
                            dateOnset = dttemp;

                            if (dttemp > DateTime.Today)
                            {
                                errors.Add("Onset Date should be before current date");
                            }
                            if (dob != null)
                            {
                                if (dttemp < Convert.ToDateTime(dob))
                                {
                                    errors.Add("Onset Date should be after Date Of Birth");
                                }
                            }
                        }
                        else
                        {
                            errors.Add("Onset Date has an invalid date format");
                        }
                    }
                }

                // ************** DATE RESOLUTION
                attrib = eventNode.Attributes["ResolutionDate"];
                if (attrib == null)
                {
                    errors.Add("ResolutionDate attribute is missing");
                }
                else
                {
                    if (!String.IsNullOrEmpty(attrib.Value))
                    {
                        if (DateTime.TryParse(attrib.Value, out dttemp))
                        {
                            dttemp = Convert.ToDateTime(attrib.Value);

                            if (dttemp > DateTime.Today)
                            {
                                errors.Add("Resolution Date should be before current date");
                            }
                            if (dttemp < dateOnset)
                            {
                                errors.Add("Resolution Date should be after Onset Date");
                            }
                            if (dob != null)
                            {
                                if (dttemp < Convert.ToDateTime(dob))
                                {
                                    errors.Add("Resolution Date should be after Date Of Birth");
                                }
                            }
                        }
                        else
                        {
                            errors.Add("Resolution Date has an invalid date format");
                        }
                    }
                }

                // Validate custom attributes
                CustomAttributeConfiguration con;

                XmlNodeList attributeNodes = eventNode.SelectNodes("Customattributes/AttributeKey");
                foreach (XmlNode attributeNode in attributeNodes)
                {
                    // Get attribute
                    attrib = attributeNode.Attributes["Name"];
                    if (attrib == null)
                    {
                        errors.Add("Name attribute missing for custom attribute");
                        continue;
                    }

                    con = _db.CustomAttributeConfigurations.SingleOrDefault(u => u.AttributeKey.Trim() == attrib.Value.Trim());
                    if (con == null)
                    {
                        errors.Add(String.Format("Unable to load configuration for custom attribute ({0})", attrib.Value));
                        continue;
                    }

                    var val = attributeNode.InnerText;
                    if (string.IsNullOrWhiteSpace(val) && con.IsRequired)
                    {
                        errors.Add(String.Format("{0} is required", con.AttributeKey));
                    }

                    if (!string.IsNullOrWhiteSpace(val) && con.CustomAttributeType == CustomAttributeType.DateTime)
                    {
                        if (!DateTime.TryParse(val, out dttemp))
                        {
                            errors.Add(String.Format("{0} has an invalid date format", con.AttributeKey));
                        }
                    }
                    if (!string.IsNullOrWhiteSpace(val) && con.CustomAttributeType == CustomAttributeType.Selection)
                    {
                        var sel = _db.SelectionDataItems.SingleOrDefault(u => u.AttributeKey == con.AttributeKey && u.Value == val.Trim());
                        if (sel == null)
                        {
                            errors.Add(String.Format("Invalid selection value specified for ({0})", con.AttributeKey));
                        }
                    }
                }
            }
        }

        private void ValidateEncounters(XmlNode patientNode, ref ArrayList errors, DateTime? dob)
        {
            DateTime dttemp;
            Decimal dectemp;
            Int32 inttemp;

            XmlNodeList encounterNodes = patientNode.SelectNodes("Encounters/Encounter");

            foreach (XmlNode encounterNode in encounterNodes)
            {
                // Validate first class attributes
                // ************** GUID
                XmlAttribute attrib = encounterNode.Attributes["guid"];
                if (attrib == null)
                {
                    errors.Add("Guid attribute is missing");
                }
                else
                {
                    if (String.IsNullOrEmpty(attrib.Value))
                    {
                        errors.Add("Guid is required");
                    }
                }

                // ************** ENCOUNTER TYPE
                attrib = encounterNode.Attributes["EncounterType"];
                if (attrib == null)
                {
                    errors.Add("EncounterType attribute is missing");
                }
                else
                {
                    if (String.IsNullOrEmpty(attrib.Value))
                    {
                        errors.Add("Encounter type is required");
                    }
                    else
                    {
                        var type = _db.EncounterTypes.SingleOrDefault(u => u.Description == attrib.Value.Trim());
                        if (type == null)
                        {
                            errors.Add("Encounter type not defined");
                        }
                    }
                }

                // ************** DATE ENCOUNTER
                attrib = encounterNode.Attributes["EncounterDate"];
                if (attrib == null)
                {
                    errors.Add("EncounterDate attribute is missing");
                }
                else
                {
                    if (String.IsNullOrEmpty(attrib.Value))
                    {
                        errors.Add("Encounter Date is required");
                    }
                    else
                    {
                        if (DateTime.TryParse(attrib.Value, out dttemp))
                        {
                            dttemp = Convert.ToDateTime(attrib.Value);

                            if (dttemp > DateTime.Today)
                            {
                                errors.Add("Encounter Date should be before current date");
                            }
                            if (dob != null)
                            {
                                if (dttemp < Convert.ToDateTime(dob))
                                {
                                    errors.Add("Encounter Date should be after Date Of Birth");
                                }
                            }
                        }
                        else
                        {
                            errors.Add("Encounter Date has an invalid date format");
                        }
                    }
                }

                // Validate instance values
                DatasetElement ele = null;
                XmlNodeList instanceNodes = encounterNode.SelectNodes("InstanceValues/InstanceValue");

                foreach (XmlNode instanceNode in instanceNodes)
                {
                    // Get attribute
                    attrib = instanceNode.Attributes["ElementName"];
                    if (attrib == null)
                    {
                        errors.Add("ElementName attribute missing for instance value");
                        continue;
                    }

                    ele = _db.DatasetElements.Include("Field").SingleOrDefault(u => u.ElementName.Trim() == attrib.Value.Trim() && u.DatasetCategoryElements.Any(dce => dce.DatasetCategory.Dataset.DatasetName == "Chronic Treatment"));
                    if (ele == null)
                    {
                        errors.Add(String.Format("Unable to load configuration for element ({0})", attrib.Value));
                        continue;
                    }

                    var val = instanceNode.InnerText;
                    if (string.IsNullOrWhiteSpace(val) && ele.Field.Mandatory)
                    {
                        errors.Add(String.Format("{0} is required", ele.ElementName));
                    }

                    if (!string.IsNullOrWhiteSpace(val) && ele.Field.FieldType.Description == "Date")
                    {
                        if (!DateTime.TryParse(val, out dttemp))
                        {
                            errors.Add(String.Format("{0} has an invalid date format", ele.ElementName));
                        }
                    }

                    string[] valid = { "Listbox", "DropDownList" };
                    if (!string.IsNullOrWhiteSpace(val) && valid.Contains(ele.Field.FieldType.Description))
                    {
                        var fv = _db.FieldValues.SingleOrDefault(u => u.Field.Id == ele.Field.Id && u.Value == val.Trim());
                        if (fv == null)
                        {
                            errors.Add(String.Format("Invalid selection value specified for ({0})",ele.ElementName));
                        }
                    }

                    if (!string.IsNullOrWhiteSpace(val) && ele.Field.FieldType.Description == "AlphaNumericTextbox")
                    {
                        if (val.Trim().Length > ele.Field.MaxLength)
                        {
                            errors.Add(String.Format("{0} exceeds the maximum field size", ele.ElementName));
                        }
                    }

                    if (!string.IsNullOrWhiteSpace(val) && ele.Field.FieldType.Description == "NumericTextbox")
                    {
                        if (ele.Field.Decimals > 0)
                        {
                            if (!Decimal.TryParse(val, out dectemp))
                            {
                                errors.Add(String.Format("{0} is not a valid decimal", ele.ElementName));
                            }
                        }
                        else
                        {
                            if (!Int32.TryParse(val, out inttemp))
                            {
                                errors.Add(String.Format("{0} is not a valid integer", ele.ElementName));
                            }
                        }
                    }
                }
            }
        }

        #endregion

        #region "Core Updates"
        
        private Patient CreatePatient(XmlNode patientNode)
        {
            DateTime dttemp;
   
            Guid guid = new Guid(patientNode.Attributes["guid"].Value.Trim());
            Patient patient = new Patient { PatientGuid = guid };

            patient.FirstName = patientNode.Attributes["FirstName"].Value.Trim();
            patient.MiddleName = patientNode.Attributes["MiddleName"] != null ? patientNode.Attributes["MiddleName"].Value.Trim() : string.Empty;
            patient.Surname = patientNode.Attributes["Surname"].Value.Trim();
            patient.DateOfBirth = DateTime.TryParse(patientNode.Attributes["DateOfBirth"].Value.Trim(), out dttemp) ? Convert.ToDateTime(patientNode.Attributes["DateOfBirth"].Value.Trim()) : (DateTime?)null;

            _db.Patients.Add(patient);

            var facilityName = patientNode.Attributes["Facility"].Value.Trim();
            var facility = _db.Facilities.SingleOrDefault(u => u.FacilityName == facilityName);
            var patientFacility = new PatientFacility { Patient = patient, Facility = facility, EnrolledDate = DateTime.Today };
            _db.PatientFacilities.Add(patientFacility);

            PatientStatusHistory status = null;
            status = new PatientStatusHistory()
            {
                Patient = patient,
                EffectiveDate = DateTime.Today,
                Comments = "New Patient",
                PatientStatus = _db.PatientStatus.Single(u => u.Description == "Active")
            };
            _db.PatientStatusHistories.Add(status);

            _db.SaveChanges();

            return patient;
        }

        private void UpdatePatient(XmlNode patientNode, ref Patient patient)
        {
            DateTime dttemp;

            patient.DateOfBirth = DateTime.TryParse(patientNode.Attributes["DateOfBirth"].Value.Trim(), out dttemp) ? Convert.ToDateTime(patientNode.Attributes["DateOfBirth"].Value.Trim()) : (DateTime?)null;
            patient.MiddleName = patientNode.Attributes["MiddleName"] != null ? patientNode.Attributes["MiddleName"].Value.Trim() : string.Empty;
            patient.Surname = patientNode.Attributes["Surname"].Value.Trim();
            patient.FirstName = patientNode.Attributes["FirstName"].Value.Trim();

            var facilityName = patientNode.Attributes["Facility"].Value.Trim();
            var facility = _db.Facilities.SingleOrDefault(u => u.FacilityName == facilityName);
            var id = patient.Id;
            var currentFacility = _db.PatientFacilities.Where(pf => pf.Patient.Id == id).OrderByDescending(f => f.EnrolledDate).ThenByDescending(f => f.Id).First();

            if (currentFacility.Facility.Id != facility.Id)
            {
                var patientFacility = new PatientFacility { Patient = patient, Facility = facility, EnrolledDate = DateTime.Today };
                _db.PatientFacilities.Add(patientFacility);
            }

            _db.SaveChanges();
        }

        private void UpdatePatientAttributes(XmlNode patientNode, ref Patient patient)
        {
            CustomAttributeConfiguration con;
            FrameworkCustomAttributeConfiguration frameworkCustumAttrConf;
            XmlAttribute attrib;

            IExtendable patientExtended = patient;

            XmlNodeList attributeNodes = patientNode.SelectNodes("Customattributes/AttributeKey");
            foreach (XmlNode attributeNode in attributeNodes)
            {
                // Get attribute
                attrib = attributeNode.Attributes["Name"];
                if (attrib == null)
                {
                    continue;
                }

                con = _db.CustomAttributeConfigurations.SingleOrDefault(u => u.AttributeKey.Trim() == attrib.Value.Trim());
                if (con == null)
                {
                    continue;
                }

                var val = attributeNode.InnerText;

                // Need to do this because the VPS.Framework version of customattributeconfig class is a different type with a Id of type long.
                frameworkCustumAttrConf = new FrameworkCustomAttributeConfiguration
                {
                    AttributeKey = con.AttributeKey,
                    CustomAttributeType = con.CustomAttributeType,
                    IsRequired = con.IsRequired,
                    StringMaxLength = con.StringMaxLength,
                    NumericMinValue = con.NumericMinValue,
                    NumericMaxValue = con.NumericMaxValue,
                    FutureDateOnly = con.FutureDateOnly,
                    PastDateOnly = con.PastDateOnly
                };

                User user = null;
                user = _db.Users.SingleOrDefault(u => u.UserName == "Admin");
                    
                switch (con.CustomAttributeType)
                {
                    case CustomAttributeType.Numeric:
                        patientExtended.ValidateAndSetAttributeValue(frameworkCustumAttrConf, string.IsNullOrEmpty(val) ? -1 : Decimal.Parse(val),
                            user.UserName);
                        break;
                    case CustomAttributeType.String:
                        patientExtended.ValidateAndSetAttributeValue(frameworkCustumAttrConf, val, user.UserName);
                        break;
                    case CustomAttributeType.Selection:
                        SelectionDataItem sel = null; 
                        sel = _db.SelectionDataItems.SingleOrDefault(u => u.AttributeKey == con.AttributeKey && u.Value == val.Trim());
                        patientExtended.ValidateAndSetAttributeValue(frameworkCustumAttrConf, string.IsNullOrEmpty(sel.SelectionKey) ? -1 : Int32.Parse(sel.SelectionKey),
                            user.UserName);
                        break;
                    case CustomAttributeType.DateTime:
                        if (!string.IsNullOrWhiteSpace(val))
                        {
                            patientExtended.ValidateAndSetAttributeValue(frameworkCustumAttrConf, DateTime.Parse(val), user.UserName);
                        }
                        break;
                    default:
                        break;
                }
            }

            _db.SaveChanges();

        }

        private PatientMedication CreatePatientMedication(XmlNode medicationNode, Patient patient)
        {
            DateTime dttemp;

            Guid guid = new Guid(medicationNode.Attributes["guid"].Value.Trim());

            PatientMedication patientMedication = null;
            Medication medication = null;

            var drugName = medicationNode.Attributes["Medication"].Value.Trim();
            medication = _db.Medications.SingleOrDefault(u => u.DrugName == drugName);

            patientMedication = new PatientMedication
            {
                Patient = patient,
                DateStart = Convert.ToDateTime(medicationNode.Attributes["DateStart"].Value.Trim()),
                DateEnd = DateTime.TryParse(medicationNode.Attributes["DateEnd"].Value.Trim(), out dttemp) ? Convert.ToDateTime(medicationNode.Attributes["DateEnd"].Value.Trim()) : (DateTime?)null,
                Medication = medication,
                Dose = medicationNode.Attributes["Dose"].Value.Trim(),
                DoseFrequency = medicationNode.Attributes["DoseFrequency"].Value.Trim(),
                DoseUnit = medicationNode.Attributes["DoseUnit"].Value.Trim(),
                PatientMedicationGuid = guid 
            };
            _db.PatientMedications.Add(patientMedication);
            _db.SaveChanges();

            return patientMedication;
        }

        private void UpdatePatientMedication(XmlNode medicationNode, ref PatientMedication patientMedication)
        {
            DateTime dttemp;

            Medication medication = null;
            var drugName = medicationNode.Attributes["Medication"].Value.Trim();
            medication = _db.Medications.SingleOrDefault(u => u.DrugName == drugName);

            patientMedication.DateStart = Convert.ToDateTime(medicationNode.Attributes["DateStart"].Value.Trim());
            patientMedication.DateEnd = DateTime.TryParse(medicationNode.Attributes["DateEnd"].Value.Trim(), out dttemp) ? Convert.ToDateTime(medicationNode.Attributes["DateEnd"].Value.Trim()) : (DateTime?)null;
            patientMedication.Medication = medication;
            patientMedication.Dose = medicationNode.Attributes["Dose"].Value.Trim();
            patientMedication.DoseFrequency = medicationNode.Attributes["DoseFrequency"].Value.Trim();
            patientMedication.DoseUnit = medicationNode.Attributes["DoseUnit"].Value.Trim();
            _db.SaveChanges();
        }

        private void UpdatePatientMedicationAttributes(XmlNode medicationNode, ref PatientMedication patientMedication)
        {
            CustomAttributeConfiguration con;
            FrameworkCustomAttributeConfiguration frameworkCustumAttrConf;
            XmlAttribute attrib;

            IExtendable patientMedicationExtended = patientMedication;

            XmlNodeList attributeNodes = medicationNode.SelectNodes("Customattributes/AttributeKey");
            foreach (XmlNode attributeNode in attributeNodes)
            {
                // Get attribute
                attrib = attributeNode.Attributes["Name"];
                if (attrib == null)
                {
                    continue;
                }

                con = _db.CustomAttributeConfigurations.SingleOrDefault(u => u.AttributeKey.Trim() == attrib.Value.Trim());
                if (con == null)
                {
                    continue;
                }

                var val = attributeNode.InnerText;

                // Need to do this because the VPS.Framework version of customattributeconfig class is a different type with a Id of type long.
                frameworkCustumAttrConf = new FrameworkCustomAttributeConfiguration
                {
                    AttributeKey = con.AttributeKey,
                    CustomAttributeType = con.CustomAttributeType,
                    IsRequired = con.IsRequired,
                    StringMaxLength = con.StringMaxLength,
                    NumericMinValue = con.NumericMinValue,
                    NumericMaxValue = con.NumericMaxValue,
                    FutureDateOnly = con.FutureDateOnly,
                    PastDateOnly = con.PastDateOnly
                };

                User user = null;
                user = _db.Users.SingleOrDefault(u => u.UserName == "Admin");

                switch (con.CustomAttributeType)
                {
                    case CustomAttributeType.Numeric:
                        patientMedicationExtended.ValidateAndSetAttributeValue(frameworkCustumAttrConf, string.IsNullOrEmpty(val) ? -1 : Decimal.Parse(val),
                            user.UserName);
                        break;
                    case CustomAttributeType.String:
                        patientMedicationExtended.ValidateAndSetAttributeValue(frameworkCustumAttrConf, val, user.UserName);
                        break;
                    case CustomAttributeType.Selection:
                        SelectionDataItem sel = null;
                        sel = _db.SelectionDataItems.SingleOrDefault(u => u.AttributeKey == con.AttributeKey && u.Value == val.Trim());
                        patientMedicationExtended.ValidateAndSetAttributeValue(frameworkCustumAttrConf, string.IsNullOrEmpty(sel.SelectionKey) ? -1 : Int32.Parse(sel.SelectionKey),
                            user.UserName);
                        break;
                    case CustomAttributeType.DateTime:
                        if (!string.IsNullOrWhiteSpace(val))
                        {
                            patientMedicationExtended.ValidateAndSetAttributeValue(frameworkCustumAttrConf, DateTime.Parse(val), user.UserName);
                        }
                        break;
                    default:
                        break;
                }
            }

            _db.SaveChanges();
        }

        private PatientCondition CreatePatientCondition(XmlNode conditionNode, Patient patient)
        {
            DateTime dttemp;

            Guid guid = new Guid(conditionNode.Attributes["guid"].Value.Trim());

            PatientCondition patientCondition = null;
            TerminologyMedDra meddra = null;
            Outcome outcome = null;
            TreatmentOutcome treatmentOutcome = null;

            var meddraCode = conditionNode.Attributes["Terminology"].Value.Trim();
            meddra = _db.TerminologyMedDras.SingleOrDefault(u => u.MedDraCode == meddraCode && u.MedDraTermType == "LLT");
            var outcomeValue = conditionNode.Attributes["Outcome"].Value != null ? conditionNode.Attributes["Outcome"].Value.Trim() : "";
            outcome = _db.Outcomes.SingleOrDefault(u => u.Description == outcomeValue);
            var treatmentOutcomeValue = conditionNode.Attributes["TreatmentOutcome"].Value != null ? conditionNode.Attributes["TreatmentOutcome"].Value.Trim() : "";
            treatmentOutcome = _db.TreatmentOutcomes.SingleOrDefault(u => u.Description == treatmentOutcomeValue);

            patientCondition = new PatientCondition
            {
                Patient = patient,
                TerminologyMedDra = meddra,
                DateStart = DateTime.TryParse(conditionNode.Attributes["DateStart"].Value.Trim(), out dttemp) ? Convert.ToDateTime(conditionNode.Attributes["DateStart"].Value.Trim()) : DateTime.Today,
                OutcomeDate = DateTime.TryParse(conditionNode.Attributes["DateEnd"].Value.Trim(), out dttemp) ? Convert.ToDateTime(conditionNode.Attributes["DateEnd"].Value.Trim()) : (DateTime?)null,
                PatientConditionGuid = guid,
                Outcome = outcome,
                TreatmentOutcome = treatmentOutcome
            };
            _db.PatientConditions.Add(patientCondition);
            _db.SaveChanges();

            return patientCondition;
        }

        private void UpdatePatientCondition(XmlNode conditionNode, ref PatientCondition patientCondition)
        {
            DateTime dttemp;

            TerminologyMedDra meddra = null;
            Outcome outcome = null;
            TreatmentOutcome treatmentOutcome = null;

            var meddraCode = conditionNode.Attributes["Terminology"].Value.Trim();
            meddra = _db.TerminologyMedDras.SingleOrDefault(u => u.MedDraCode == meddraCode && u.MedDraTermType == "LLT");
            var outcomeValue = conditionNode.Attributes["Outcome"].Value != null ? conditionNode.Attributes["Outcome"].Value.Trim() : "";
            outcome = _db.Outcomes.SingleOrDefault(u => u.Description == outcomeValue);
            var treatmentOutcomeValue = conditionNode.Attributes["TreatmentOutcome"].Value != null ? conditionNode.Attributes["TreatmentOutcome"].Value.Trim() : "";
            treatmentOutcome = _db.TreatmentOutcomes.SingleOrDefault(u => u.Description == treatmentOutcomeValue);

            patientCondition.TerminologyMedDra = meddra;
            patientCondition.DateStart = DateTime.TryParse(conditionNode.Attributes["DateStart"].Value.Trim(), out dttemp) ? Convert.ToDateTime(conditionNode.Attributes["DateStart"].Value.Trim()) : DateTime.Today;
            patientCondition.OutcomeDate = DateTime.TryParse(conditionNode.Attributes["DateEnd"].Value.Trim(), out dttemp) ? Convert.ToDateTime(conditionNode.Attributes["DateEnd"].Value.Trim()) : (DateTime?)null;
            patientCondition.Outcome = outcome;
            patientCondition.TreatmentOutcome = treatmentOutcome;

            _db.SaveChanges();
        }

        private void UpdatePatientConditionAttributes(XmlNode conditionNode, ref PatientCondition patientCondition)
        {
            CustomAttributeConfiguration con;
            FrameworkCustomAttributeConfiguration frameworkCustumAttrConf;
            XmlAttribute attrib;

            IExtendable patientConditionExtended = patientCondition;

            XmlNodeList attributeNodes = conditionNode.SelectNodes("Customattributes/AttributeKey");
            foreach (XmlNode attributeNode in attributeNodes)
            {
                // Get attribute
                attrib = attributeNode.Attributes["Name"];
                if (attrib == null)
                {
                    continue;
                }

                con = _db.CustomAttributeConfigurations.SingleOrDefault(u => u.AttributeKey.Trim() == attrib.Value.Trim());
                if (con == null)
                {
                    continue;
                }

                var val = attributeNode.InnerText;

                // Need to do this because the VPS.Framework version of customattributeconfig class is a different type with a Id of type long.
                frameworkCustumAttrConf = new FrameworkCustomAttributeConfiguration
                {
                    AttributeKey = con.AttributeKey,
                    CustomAttributeType = con.CustomAttributeType,
                    IsRequired = con.IsRequired,
                    StringMaxLength = con.StringMaxLength,
                    NumericMinValue = con.NumericMinValue,
                    NumericMaxValue = con.NumericMaxValue,
                    FutureDateOnly = con.FutureDateOnly,
                    PastDateOnly = con.PastDateOnly
                };

                User user = null;
                user = _db.Users.SingleOrDefault(u => u.UserName == "Admin");

                switch (con.CustomAttributeType)
                {
                    case CustomAttributeType.Numeric:
                        patientConditionExtended.ValidateAndSetAttributeValue(frameworkCustumAttrConf, string.IsNullOrEmpty(val) ? -1 : Decimal.Parse(val),
                            user.UserName);
                        break;
                    case CustomAttributeType.String:
                        patientConditionExtended.ValidateAndSetAttributeValue(frameworkCustumAttrConf, val, user.UserName);
                        break;
                    case CustomAttributeType.Selection:
                        SelectionDataItem sel = null;
                        sel = _db.SelectionDataItems.SingleOrDefault(u => u.AttributeKey == con.AttributeKey && u.Value == val.Trim());
                        patientConditionExtended.ValidateAndSetAttributeValue(frameworkCustumAttrConf, string.IsNullOrEmpty(sel.SelectionKey) ? -1 : Int32.Parse(sel.SelectionKey),
                            user.UserName);
                        break;
                    case CustomAttributeType.DateTime:
                        if (!string.IsNullOrWhiteSpace(val))
                        {
                            patientConditionExtended.ValidateAndSetAttributeValue(frameworkCustumAttrConf, DateTime.Parse(val), user.UserName);
                        }
                        break;
                    default:
                        break;
                }
            }

            _db.SaveChanges();
        }

        private PatientLabTest CreatePatientLabTest(XmlNode labNode, Patient patient)
        {
            Guid guid = new Guid(labNode.Attributes["guid"].Value.Trim());

            PatientLabTest patientLabTest = null;
            LabTest labTest = null;
            LabTestUnit testUnit = null;

            var testName = labNode.Attributes["Test"].Value.Trim();
            labTest = _db.LabTests.SingleOrDefault(u => u.Description == testName);
            var testUnitName = labNode.Attributes["TestUnit"].Value.Trim();
            testUnit = _db.LabTestUnits.SingleOrDefault(u => u.Description == testUnitName);

            patientLabTest = new PatientLabTest
            {
                Patient = patient,
                LabTest = labTest,
                TestDate = Convert.ToDateTime(labNode.Attributes["TestDate"].Value.Trim()),
                TestResult = labNode.Attributes["TestResultCoded"].Value.Trim(),
                LabValue = labNode.Attributes["TestResultValue"].Value.Trim(),
                TestUnit = testUnit,
                PatientLabTestGuid = guid
            };
            _db.PatientLabTests.Add(patientLabTest);
            _db.SaveChanges();

            return patientLabTest;
        }

        private void UpdatePatientLabTest(XmlNode labNode, ref PatientLabTest patientLabTest)
        {
            LabTest labTest = null;
            LabTestUnit testUnit = null;

            var testName = labNode.Attributes["Test"].Value.Trim();
            labTest = _db.LabTests.SingleOrDefault(u => u.Description == testName);
            var testUnitName = labNode.Attributes["TestUnit"].Value.Trim();
            testUnit = _db.LabTestUnits.SingleOrDefault(u => u.Description == testUnitName);

            patientLabTest.LabTest = labTest;
            patientLabTest.TestDate = Convert.ToDateTime(labNode.Attributes["TestDate"].Value.Trim());
            patientLabTest.TestResult = labNode.Attributes["TestResultCoded"].Value.Trim();
            patientLabTest.LabValue = labNode.Attributes["TestResultValue"].Value.Trim();
            patientLabTest.TestUnit = testUnit;

            _db.SaveChanges();
        }

        private void UpdatePatientLabTestAttributes(XmlNode labNode, ref PatientLabTest patientLabTest)
        {
            CustomAttributeConfiguration con;
            FrameworkCustomAttributeConfiguration frameworkCustumAttrConf;
            XmlAttribute attrib;

            IExtendable patientLabTestExtended = patientLabTest;

            XmlNodeList attributeNodes = labNode.SelectNodes("Customattributes/AttributeKey");
            foreach (XmlNode attributeNode in attributeNodes)
            {
                // Get attribute
                attrib = attributeNode.Attributes["Name"];
                if (attrib == null)
                {
                    continue;
                }

                con = _db.CustomAttributeConfigurations.SingleOrDefault(u => u.AttributeKey.Trim() == attrib.Value.Trim());
                if (con == null)
                {
                    continue;
                }

                var val = attributeNode.InnerText;

                // Need to do this because the VPS.Framework version of customattributeconfig class is a different type with a Id of type long.
                frameworkCustumAttrConf = new FrameworkCustomAttributeConfiguration
                {
                    AttributeKey = con.AttributeKey,
                    CustomAttributeType = con.CustomAttributeType,
                    IsRequired = con.IsRequired,
                    StringMaxLength = con.StringMaxLength,
                    NumericMinValue = con.NumericMinValue,
                    NumericMaxValue = con.NumericMaxValue,
                    FutureDateOnly = con.FutureDateOnly,
                    PastDateOnly = con.PastDateOnly
                };

                User user = null;
                user = _db.Users.SingleOrDefault(u => u.UserName == "Admin");

                switch (con.CustomAttributeType)
                {
                    case CustomAttributeType.Numeric:
                        patientLabTestExtended.ValidateAndSetAttributeValue(frameworkCustumAttrConf, string.IsNullOrEmpty(val) ? -1 : Decimal.Parse(val),
                            user.UserName);
                        break;
                    case CustomAttributeType.String:
                        patientLabTestExtended.ValidateAndSetAttributeValue(frameworkCustumAttrConf, val, user.UserName);
                        break;
                    case CustomAttributeType.Selection:
                        SelectionDataItem sel = null;
                        sel = _db.SelectionDataItems.SingleOrDefault(u => u.AttributeKey == con.AttributeKey && u.Value == val.Trim());
                        patientLabTestExtended.ValidateAndSetAttributeValue(frameworkCustumAttrConf, string.IsNullOrEmpty(sel.SelectionKey) ? -1 : Int32.Parse(sel.SelectionKey),
                            user.UserName);
                        break;
                    case CustomAttributeType.DateTime:
                        if (!string.IsNullOrWhiteSpace(val))
                        {
                            patientLabTestExtended.ValidateAndSetAttributeValue(frameworkCustumAttrConf, DateTime.Parse(val), user.UserName);
                        }
                        break;
                    default:
                        break;
                }
            }

            _db.SaveChanges();
        }

        private PatientClinicalEvent CreatePatientClinicalEvent(XmlNode eventNode, Patient patient)
        {
            Guid guid = new Guid(eventNode.Attributes["guid"].Value.Trim());

            DateTime dttemp;

            PatientClinicalEvent patientClinicalEvent = null;
            TerminologyMedDra meddra = null;

            var meddraCode = eventNode.Attributes["SourceTerminology"].Value.Trim();
            meddra = _db.TerminologyMedDras.SingleOrDefault(u => u.MedDraCode == meddraCode && u.MedDraTermType == "LLT");

            var currentUser = _db.Users.SingleOrDefault(u => u.UserName == "Admin");

            patientClinicalEvent = new PatientClinicalEvent
            {
                Patient = patient,
                SourceDescription = Convert.ToString(eventNode.Attributes["SourceDescription"].Value.Trim()),
                SourceTerminologyMedDra = meddra,
                OnsetDate = DateTime.TryParse(eventNode.Attributes["OnsetDate"].Value.Trim(), out dttemp) ? Convert.ToDateTime(eventNode.Attributes["OnsetDate"].Value.Trim()) : (DateTime?)null,
                ResolutionDate = DateTime.TryParse(eventNode.Attributes["ResolutionDate"].Value.Trim(), out dttemp) ? Convert.ToDateTime(eventNode.Attributes["ResolutionDate"].Value.Trim()) : (DateTime?)null,
                PatientClinicalEventGuid = guid
            };
            _db.PatientClinicalEvents.Add(patientClinicalEvent);
            _db.SaveChanges();

            return patientClinicalEvent;
        }

        private void UpdatePatientClinicalEvent(XmlNode eventNode, ref PatientClinicalEvent patientClinicalEvent)
        {
            DateTime dttemp;

            TerminologyMedDra meddra = null;
            var meddraCode = eventNode.Attributes["SourceTerminology"].Value.Trim();
            meddra = _db.TerminologyMedDras.SingleOrDefault(u => u.MedDraCode == meddraCode && u.MedDraTermType == "LLT");

            patientClinicalEvent.SourceDescription = Convert.ToString(eventNode.Attributes["SourceDescription"].Value.Trim());
            patientClinicalEvent.SourceTerminologyMedDra = meddra;
            patientClinicalEvent.OnsetDate = DateTime.TryParse(eventNode.Attributes["OnsetDate"].Value.Trim(), out dttemp) ? Convert.ToDateTime(eventNode.Attributes["OnsetDate"].Value.Trim()) : (DateTime?)null;
            patientClinicalEvent.ResolutionDate = DateTime.TryParse(eventNode.Attributes["ResolutionDate"].Value.Trim(), out dttemp) ? Convert.ToDateTime(eventNode.Attributes["ResolutionDate"].Value.Trim()) : (DateTime?)null;

            _db.SaveChanges();
        }

        private void UpdatePatientClinicalEventAttributes(XmlNode eventNode, ref PatientClinicalEvent patientClinicalEvent)
        {
            CustomAttributeConfiguration con;
            FrameworkCustomAttributeConfiguration frameworkCustumAttrConf;
            XmlAttribute attrib;

            IExtendable patientClinicalEventExtended = patientClinicalEvent;

            XmlNodeList attributeNodes = eventNode.SelectNodes("Customattributes/AttributeKey");
            foreach (XmlNode attributeNode in attributeNodes)
            {
                // Get attribute
                attrib = attributeNode.Attributes["Name"];
                if (attrib == null)
                {
                    continue;
                }

                con = _db.CustomAttributeConfigurations.SingleOrDefault(u => u.AttributeKey.Trim() == attrib.Value.Trim());
                if (con == null)
                {
                    continue;
                }

                var val = attributeNode.InnerText;

                // Need to do this because the VPS.Framework version of customattributeconfig class is a different type with a Id of type long.
                frameworkCustumAttrConf = new FrameworkCustomAttributeConfiguration
                {
                    AttributeKey = con.AttributeKey,
                    CustomAttributeType = con.CustomAttributeType,
                    IsRequired = con.IsRequired,
                    StringMaxLength = con.StringMaxLength,
                    NumericMinValue = con.NumericMinValue,
                    NumericMaxValue = con.NumericMaxValue,
                    FutureDateOnly = con.FutureDateOnly,
                    PastDateOnly = con.PastDateOnly
                };

                User user = null;
                user = _db.Users.SingleOrDefault(u => u.UserName == "Admin");

                switch (con.CustomAttributeType)
                {
                    case CustomAttributeType.Numeric:
                        patientClinicalEventExtended.ValidateAndSetAttributeValue(frameworkCustumAttrConf, string.IsNullOrEmpty(val) ? -1 : Decimal.Parse(val),
                            user.UserName);
                        break;
                    case CustomAttributeType.String:
                        patientClinicalEventExtended.ValidateAndSetAttributeValue(frameworkCustumAttrConf, val, user.UserName);
                        break;
                    case CustomAttributeType.Selection:
                        SelectionDataItem sel = null;
                        sel = _db.SelectionDataItems.SingleOrDefault(u => u.AttributeKey == con.AttributeKey && u.Value == val.Trim());
                        patientClinicalEventExtended.ValidateAndSetAttributeValue(frameworkCustumAttrConf, string.IsNullOrEmpty(sel.SelectionKey) ? -1 : Int32.Parse(sel.SelectionKey),
                            user.UserName);
                        break;
                    case CustomAttributeType.DateTime:
                        if (!string.IsNullOrWhiteSpace(val))
                        {
                            patientClinicalEventExtended.ValidateAndSetAttributeValue(frameworkCustumAttrConf, DateTime.Parse(val), user.UserName);
                        }
                        break;
                    default:
                        break;
                }
            }

            _db.SaveChanges();
        }

        private Encounter CreateEncounter(XmlNode encounterNode, Patient patient)
        {
            Guid guid = new Guid(encounterNode.Attributes["guid"].Value.Trim());

            var etdesc = encounterNode.Attributes["EncounterType"].Value.Trim();
            var encounterType = _db.EncounterTypes.SingleOrDefault(et => et.Description == etdesc);
            var priority = _db.Priorities.SingleOrDefault(p => p.Description == "Not Set");
            var encounterDate = Convert.ToDateTime(encounterNode.Attributes["EncounterDate"].Value);
            var currentUser = _db.Users.SingleOrDefault(u => u.UserName == "Admin");

            var newEncounter = new Encounter(patient)
            {
                EncounterType = encounterType,
                Priority = priority,
                EncounterDate = encounterDate,
                EncounterGuid = guid
            };
            _db.Encounters.Add(newEncounter);
            _db.SaveChanges();

            // Create instance if necessary
            var encounterTypeWorkPlan = _db.EncounterTypeWorkPlans
                .Include("WorkPlan.Dataset")
                .Where(et => et.EncounterType.Id == encounterType.Id)
                .SingleOrDefault();

            if (encounterTypeWorkPlan != null)
            {
                // Create a new instance
                var dataset = _db.Datasets 
                    .Include("DatasetCategories.DatasetCategoryElements.DatasetElement.Field.FieldType")
                    .Include("DatasetCategories.DatasetCategoryElements.DatasetElement.DatasetElementSubs.Field.FieldType")
                    .SingleOrDefault(d => d.Id == encounterTypeWorkPlan.WorkPlan.Dataset.Id);

                if (dataset != null)
                {
                    var datasetInstance = dataset.CreateInstance(newEncounter.Id, encounterTypeWorkPlan);

                    _db.DatasetInstances.Add(datasetInstance);
                    _db.SaveChanges();
                }
            }

            return newEncounter;
        }

        private void UpdateEncounter(XmlNode encounterNode, ref Encounter encounter)
        {
            var encounterDate = Convert.ToDateTime(encounterNode.Attributes["EncounterDate"].Value);

            encounter.EncounterDate = encounterDate;

            _db.SaveChanges();
        }

        private void UpdateEncounterValues(XmlNode encounterNode, ref Encounter encounter)
        {
            DatasetElement ele;
            XmlAttribute attrib;

            var id = encounter.Id;
            var etid = encounter.EncounterType.Id;

            DatasetInstance datasetInstance = _db.DatasetInstances
                .Include(di => di.Dataset)
                .Include("DatasetInstanceValues.DatasetElement.Field.FieldType")
                .Include("DatasetInstanceValues.DatasetInstanceSubValues.DatasetElementSub.Field.FieldType")
                .Where(di => di.Dataset.ContextType.Id == (int)ContextTypes.Encounter
                    && di.ContextID == id
                    && di.EncounterTypeWorkPlan.EncounterType.Id == etid).SingleOrDefault();

            if (datasetInstance != null)
            {
                XmlNodeList valueNodes = encounterNode.SelectNodes("InstanceValues/InstanceValue");
                foreach (XmlNode valueNode in valueNodes)
                {
                    // Get attribute
                    attrib = valueNode.Attributes["ElementName"];
                    if (attrib == null)
                    {
                        continue;
                    }

                    ele = _db.DatasetElements.SingleOrDefault(de => de.ElementName.Trim() == attrib.Value.Trim() && de.DatasetCategoryElements.Any(dce => dce.DatasetCategory.Dataset.DatasetName == "Chronic Treatment"));
                    if (ele == null)
                    {
                        continue;
                    }

                    var val = valueNode.InnerText;

                    datasetInstance.SetInstanceValue(ele, val);
                }

                _db.SaveChanges();
            }
        }

        #endregion

        #region "Audit Updates"

        private void LogAudit(AuditType type, string details, string log)
        {
            var audit = new AuditLog()
            {
                AuditType = type,
                User = null,
                ActionDate = DateTime.Now,
                Details = details,
                Log = log
            };
            _db.AuditLogs.Add(audit);
            _db.SaveChanges();
        }

        #endregion

        }
}

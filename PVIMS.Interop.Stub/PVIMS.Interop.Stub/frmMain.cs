using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

namespace PVIMS.Interop.Stub
{
    public partial class frmMain : Form
    {
        private string _payload = "";
        private string _log = "";

        private ArrayList _medications = new ArrayList();
        private ArrayList _evaluations = new ArrayList();
        private ArrayList _events = new ArrayList();
        private ArrayList _conditions = new ArrayList();
        private ArrayList _encounters = new ArrayList();
        
        public frmMain()
        {
            InitializeComponent();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        #region "XML"

        private void btnGenXML_Click(object sender, EventArgs e)
        {
            var documentDirectory = String.Format("{0}\\Temp\\", System.AppDomain.CurrentDomain.BaseDirectory);
            var ns = ""; // urn:pvims-org:v3

            string contentXml = string.Empty;
            string destName = string.Format("INTEROP_{0}.xml", DateTime.Now.ToString("yyyyMMddhhmmsss"));
            string destFile = string.Format("{0}{1}", documentDirectory, destName);

            // Create document
            XmlDocument xmlDoc = new XmlDocument();
            XmlNode rootNode;
            XmlNode patientNode;
            XmlNode customHeadNode;
            XmlNode medicationHeadNode;
            XmlNode evaluationHeadNode;
            XmlNode conditionHeadNode;
            XmlNode eventHeadNode;
            XmlNode encounterHeadNode;
            XmlAttribute attrib;
            XmlComment comment;

            XmlDeclaration xmlDeclaration = xmlDoc.CreateXmlDeclaration("1.0", "UTF-8", null);
            xmlDoc.AppendChild(xmlDeclaration);

            // Patients main node
            rootNode = xmlDoc.CreateElement("Patients", ns);

            // Write patient
            patientNode = xmlDoc.CreateElement("Patient", ns);

            attrib = xmlDoc.CreateAttribute("Archive");
            attrib.InnerText = "false";
            patientNode.Attributes.Append(attrib);

            attrib = xmlDoc.CreateAttribute("Facility");
            attrib.InnerText = txtPatientFacility.Text;
            patientNode.Attributes.Append(attrib);

            attrib = xmlDoc.CreateAttribute("DateOfBirth");
            attrib.InnerText = dtpPatientDOB.Checked == true ? dtpPatientDOB.Value.ToString("yyyy-MM-dd") : "";
            patientNode.Attributes.Append(attrib);

            attrib = xmlDoc.CreateAttribute("MiddleName");
            attrib.InnerText = txtPatientMiddleName.Text;
            patientNode.Attributes.Append(attrib);

            attrib = xmlDoc.CreateAttribute("Surname");
            attrib.InnerText = txtPatientLastName.Text;
            patientNode.Attributes.Append(attrib);

            attrib = xmlDoc.CreateAttribute("FirstName");
            attrib.InnerText = txtPatientFirstName.Text;
            patientNode.Attributes.Append(attrib);

            attrib = xmlDoc.CreateAttribute("LastUpdated");
            attrib.InnerText = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            patientNode.Attributes.Append(attrib);

            attrib = xmlDoc.CreateAttribute("CreatedDate");
            attrib.InnerText = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            patientNode.Attributes.Append(attrib);

            attrib = xmlDoc.CreateAttribute("guid");
            attrib.InnerText = txtPatientGUID.Text;
            patientNode.Attributes.Append(attrib);

            customHeadNode = xmlDoc.CreateElement("Customattributes", ns);
            PreparePatientAttributesXML(ref xmlDoc, ref customHeadNode, ns);

            medicationHeadNode = xmlDoc.CreateElement("Medications", ns);
            PrepareMedicationsXML(ref xmlDoc, ref medicationHeadNode, ns);

            evaluationHeadNode = xmlDoc.CreateElement("LabTests", ns);
            PrepareEvaluationsXML(ref xmlDoc, ref evaluationHeadNode, ns);

            conditionHeadNode = xmlDoc.CreateElement("Conditions", ns);
            PrepareConditionsXML(ref xmlDoc, ref conditionHeadNode, ns);

            eventHeadNode = xmlDoc.CreateElement("ClinicalEvents", ns);
            PrepareEventsXML(ref xmlDoc, ref eventHeadNode, ns);

            encounterHeadNode = xmlDoc.CreateElement("Encounters", ns);
            PrepareEncountersXML(ref xmlDoc, ref encounterHeadNode, ns);
            
            patientNode.AppendChild(customHeadNode);
            patientNode.AppendChild(medicationHeadNode);
            patientNode.AppendChild(evaluationHeadNode);
            patientNode.AppendChild(conditionHeadNode);
            patientNode.AppendChild(eventHeadNode);
            patientNode.AppendChild(encounterHeadNode);
            rootNode.AppendChild(patientNode);

            xmlDoc.AppendChild(rootNode);

            contentXml = FormatXML(xmlDoc);
            WriteXML(destFile, contentXml);

            txtXML.Text = contentXml;
            _payload = contentXml;
        }

        private void PreparePatientAttributesXML(ref XmlDocument xmlDoc, ref XmlNode customHeadNode, string ns)
        {
            XmlNode attributeNode;
            XmlAttribute attrib;

            // Write patient attributes

            attributeNode = xmlDoc.CreateElement("AttributeKey", ns);
            attrib = xmlDoc.CreateAttribute("Archive");
            attrib.InnerText = "false";
            attributeNode.Attributes.Append(attrib);
            attrib = xmlDoc.CreateAttribute("Name");
            attrib.InnerText = "Medical Record Number";
            attributeNode.Attributes.Append(attrib);
            attributeNode.InnerText = txtMedicalRecordNumber.Text;
            customHeadNode.AppendChild(attributeNode);

            attributeNode = xmlDoc.CreateElement("AttributeKey", ns);
            attrib = xmlDoc.CreateAttribute("Archive");
            attrib.InnerText = "false";
            attributeNode.Attributes.Append(attrib);
            attrib = xmlDoc.CreateAttribute("Name");
            attrib.InnerText = "Medical Record Number Type";
            attributeNode.Attributes.Append(attrib);
            attributeNode.InnerText = txtMedicalRecordNumberType.Text;
            customHeadNode.AppendChild(attributeNode);

            attributeNode = xmlDoc.CreateElement("AttributeKey", ns);
            attrib = xmlDoc.CreateAttribute("Archive");
            attrib.InnerText = "false";
            attributeNode.Attributes.Append(attrib);
            attrib = xmlDoc.CreateAttribute("Name");
            attrib.InnerText = "Patient Identity Number";
            attributeNode.Attributes.Append(attrib);
            attributeNode.InnerText = txtPatientIdentityNumber.Text;
            customHeadNode.AppendChild(attributeNode);

            attributeNode = xmlDoc.CreateElement("AttributeKey", ns);
            attrib = xmlDoc.CreateAttribute("Archive");
            attrib.InnerText = "false";
            attributeNode.Attributes.Append(attrib);
            attrib = xmlDoc.CreateAttribute("Name");
            attrib.InnerText = "Identity Type";
            attributeNode.Attributes.Append(attrib);
            attributeNode.InnerText = txtIdentityType.Text;
            customHeadNode.AppendChild(attributeNode);

            attributeNode = xmlDoc.CreateElement("AttributeKey", ns);
            attrib = xmlDoc.CreateAttribute("Archive");
            attrib.InnerText = "false";
            attributeNode.Attributes.Append(attrib);
            attrib = xmlDoc.CreateAttribute("Name");
            attrib.InnerText = "Gender";
            attributeNode.Attributes.Append(attrib);
            attributeNode.InnerText = txtGender.Text;
            customHeadNode.AppendChild(attributeNode);

            attributeNode = xmlDoc.CreateElement("AttributeKey", ns);
            attrib = xmlDoc.CreateAttribute("Archive");
            attrib.InnerText = "false";
            attributeNode.Attributes.Append(attrib);
            attrib = xmlDoc.CreateAttribute("Name");
            attrib.InnerText = "MaritalStatus";
            attributeNode.Attributes.Append(attrib);
            attributeNode.InnerText = txtMaritalStatus.Text;
            customHeadNode.AppendChild(attributeNode);

            attributeNode = xmlDoc.CreateElement("AttributeKey", ns);
            attrib = xmlDoc.CreateAttribute("Archive");
            attrib.InnerText = "false";
            attributeNode.Attributes.Append(attrib);
            attrib = xmlDoc.CreateAttribute("Name");
            attrib.InnerText = "Employment Status";
            attributeNode.Attributes.Append(attrib);
            attributeNode.InnerText = txtEmploymentStatus.Text;
            customHeadNode.AppendChild(attributeNode);

            attributeNode = xmlDoc.CreateElement("AttributeKey", ns);
            attrib = xmlDoc.CreateAttribute("Archive");
            attrib.InnerText = "false";
            attributeNode.Attributes.Append(attrib);
            attrib = xmlDoc.CreateAttribute("Name");
            attrib.InnerText = "Occupation";
            attributeNode.Attributes.Append(attrib);
            attributeNode.InnerText = txtOccupation.Text;
            customHeadNode.AppendChild(attributeNode);

            attributeNode = xmlDoc.CreateElement("AttributeKey", ns);
            attrib = xmlDoc.CreateAttribute("Archive");
            attrib.InnerText = "false";
            attributeNode.Attributes.Append(attrib);
            attrib = xmlDoc.CreateAttribute("Name");
            attrib.InnerText = "Language";
            attributeNode.Attributes.Append(attrib);
            attributeNode.InnerText = txtLanguage.Text;
            customHeadNode.AppendChild(attributeNode);

            attributeNode = xmlDoc.CreateElement("AttributeKey", ns);
            attrib = xmlDoc.CreateAttribute("Archive");
            attrib.InnerText = "false";
            attributeNode.Attributes.Append(attrib);
            attrib = xmlDoc.CreateAttribute("Name");
            attrib.InnerText = "Address";
            attributeNode.Attributes.Append(attrib);
            attributeNode.InnerText = txtAddress.Text;
            customHeadNode.AppendChild(attributeNode);

            attributeNode = xmlDoc.CreateElement("AttributeKey", ns);
            attrib = xmlDoc.CreateAttribute("Archive");
            attrib.InnerText = "false";
            attributeNode.Attributes.Append(attrib);
            attrib = xmlDoc.CreateAttribute("Name");
            attrib.InnerText = "Address Line 2";
            attributeNode.Attributes.Append(attrib);
            attributeNode.InnerText = txtAddressLine2.Text;
            customHeadNode.AppendChild(attributeNode);

            attributeNode = xmlDoc.CreateElement("AttributeKey", ns);
            attrib = xmlDoc.CreateAttribute("Archive");
            attrib.InnerText = "false";
            attributeNode.Attributes.Append(attrib);
            attrib = xmlDoc.CreateAttribute("Name");
            attrib.InnerText = "City";
            attributeNode.Attributes.Append(attrib);
            attributeNode.InnerText = txtCity.Text;
            customHeadNode.AppendChild(attributeNode);

            attributeNode = xmlDoc.CreateElement("AttributeKey", ns);
            attrib = xmlDoc.CreateAttribute("Archive");
            attrib.InnerText = "false";
            attributeNode.Attributes.Append(attrib);
            attrib = xmlDoc.CreateAttribute("Name");
            attrib.InnerText = "State";
            attributeNode.Attributes.Append(attrib);
            attributeNode.InnerText = txtState.Text;
            customHeadNode.AppendChild(attributeNode);

            attributeNode = xmlDoc.CreateElement("AttributeKey", ns);
            attrib = xmlDoc.CreateAttribute("Archive");
            attrib.InnerText = "false";
            attributeNode.Attributes.Append(attrib);
            attrib = xmlDoc.CreateAttribute("Name");
            attrib.InnerText = "Postal Code";
            attributeNode.Attributes.Append(attrib);
            attributeNode.InnerText = txtPostalCode.Text;
            customHeadNode.AppendChild(attributeNode);

            attributeNode = xmlDoc.CreateElement("AttributeKey", ns);
            attrib = xmlDoc.CreateAttribute("Archive");
            attrib.InnerText = "false";
            attributeNode.Attributes.Append(attrib);
            attrib = xmlDoc.CreateAttribute("Name");
            attrib.InnerText = "Patient Contact Number";
            attributeNode.Attributes.Append(attrib);
            attributeNode.InnerText = txtPatientContactNumber.Text;
            customHeadNode.AppendChild(attributeNode);

            attributeNode = xmlDoc.CreateElement("AttributeKey", ns);
            attrib = xmlDoc.CreateAttribute("Archive");
            attrib.InnerText = "false";
            attributeNode.Attributes.Append(attrib);
            attrib = xmlDoc.CreateAttribute("Name");
            attrib.InnerText = "Country of Birth";
            attributeNode.Attributes.Append(attrib);
            attributeNode.InnerText = txtCountryofBirth.Text;
            customHeadNode.AppendChild(attributeNode);

        }

        private void PrepareMedicationsXML(ref XmlDocument xmlDoc, ref XmlNode medicationHeadNode, string ns)
        {
            XmlNode medicationNode;
            XmlNode customHeadNode;
            XmlNode attributeNode;
            XmlAttribute attrib;

            // Write medication
            foreach(MedicationItem med in _medications)
            {
                medicationNode = xmlDoc.CreateElement("Medication", ns);
                attrib = xmlDoc.CreateAttribute("guid");
                attrib.InnerText = med.PatientMedicationGuid;
                medicationNode.Attributes.Append(attrib);
                attrib = xmlDoc.CreateAttribute("Medication");
                attrib.InnerText = med.Medication;
                medicationNode.Attributes.Append(attrib);
                attrib = xmlDoc.CreateAttribute("DoseUnit");
                attrib.InnerText = med.DoseUnit;
                medicationNode.Attributes.Append(attrib);
                attrib = xmlDoc.CreateAttribute("DoseFrequency");
                attrib.InnerText = med.Frequency;
                medicationNode.Attributes.Append(attrib);
                attrib = xmlDoc.CreateAttribute("Dose");
                attrib.InnerText = med.Dose;
                medicationNode.Attributes.Append(attrib);
                attrib = xmlDoc.CreateAttribute("DateEnd");
                attrib.InnerText = med.DateEnd != null ? Convert.ToDateTime(med.DateEnd).ToString("yyyy-MM-dd") : "";
                medicationNode.Attributes.Append(attrib);
                attrib = xmlDoc.CreateAttribute("DateStart");
                attrib.InnerText = med.DateStart != null ? Convert.ToDateTime(med.DateStart).ToString("yyyy-MM-dd") : "";
                medicationNode.Attributes.Append(attrib);

                customHeadNode = xmlDoc.CreateElement("Customattributes", ns);

                attributeNode = xmlDoc.CreateElement("AttributeKey", ns);
                attrib = xmlDoc.CreateAttribute("Archive");
                attrib.InnerText = "false";
                attributeNode.Attributes.Append(attrib);
                attrib = xmlDoc.CreateAttribute("Name");
                attrib.InnerText = "Route";
                attributeNode.Attributes.Append(attrib);
                attributeNode.InnerText = med.Route;
                customHeadNode.AppendChild(attributeNode);

                attributeNode = xmlDoc.CreateElement("AttributeKey", ns);
                attrib = xmlDoc.CreateAttribute("Archive");
                attrib.InnerText = "false";
                attributeNode.Attributes.Append(attrib);
                attrib = xmlDoc.CreateAttribute("Name");
                attrib.InnerText = "Days/week";
                attributeNode.Attributes.Append(attrib);
                attributeNode.InnerText = med.DaysWeek;
                customHeadNode.AppendChild(attributeNode);

                attributeNode = xmlDoc.CreateElement("AttributeKey", ns);
                attrib = xmlDoc.CreateAttribute("Archive");
                attrib.InnerText = "false";
                attributeNode.Attributes.Append(attrib);
                attrib = xmlDoc.CreateAttribute("Name");
                attrib.InnerText = "Still On Medication";
                attributeNode.Attributes.Append(attrib);
                attributeNode.InnerText = med.StillOnMedication;
                customHeadNode.AppendChild(attributeNode);

                attributeNode = xmlDoc.CreateElement("AttributeKey", ns);
                attrib = xmlDoc.CreateAttribute("Archive");
                attrib.InnerText = "false";
                attributeNode.Attributes.Append(attrib);
                attrib = xmlDoc.CreateAttribute("Name");
                attrib.InnerText = "Indication";
                attributeNode.Attributes.Append(attrib);
                attributeNode.InnerText = med.Indication;
                customHeadNode.AppendChild(attributeNode);

                attributeNode = xmlDoc.CreateElement("AttributeKey", ns);
                attrib = xmlDoc.CreateAttribute("Archive");
                attrib.InnerText = "false";
                attributeNode.Attributes.Append(attrib);
                attrib = xmlDoc.CreateAttribute("Name");
                attrib.InnerText = "Type of Indication";
                attributeNode.Attributes.Append(attrib);
                attributeNode.InnerText = med.TypeofIndication;
                customHeadNode.AppendChild(attributeNode);

                attributeNode = xmlDoc.CreateElement("AttributeKey", ns);
                attrib = xmlDoc.CreateAttribute("Archive");
                attrib.InnerText = "false";
                attributeNode.Attributes.Append(attrib);
                attrib = xmlDoc.CreateAttribute("Name");
                attrib.InnerText = "Reason For Stopping";
                attributeNode.Attributes.Append(attrib);
                attributeNode.InnerText = med.ReasonForStopping;
                customHeadNode.AppendChild(attributeNode);

                attributeNode = xmlDoc.CreateElement("AttributeKey", ns);
                attrib = xmlDoc.CreateAttribute("Archive");
                attrib.InnerText = "false";
                attributeNode.Attributes.Append(attrib);
                attrib = xmlDoc.CreateAttribute("Name");
                attrib.InnerText = "Clinician action taken with regard to medicine if related to AE";
                attributeNode.Attributes.Append(attrib);
                attributeNode.InnerText = med.Action;
                customHeadNode.AppendChild(attributeNode);

                attributeNode = xmlDoc.CreateElement("AttributeKey", ns);
                attrib = xmlDoc.CreateAttribute("Archive");
                attrib.InnerText = "false";
                attributeNode.Attributes.Append(attrib);
                attrib = xmlDoc.CreateAttribute("Name");
                attrib.InnerText = "Batch Number";
                attributeNode.Attributes.Append(attrib);
                attributeNode.InnerText = med.BatchNumber;
                customHeadNode.AppendChild(attributeNode);

                attributeNode = xmlDoc.CreateElement("AttributeKey", ns);
                attrib = xmlDoc.CreateAttribute("Archive");
                attrib.InnerText = "false";
                attributeNode.Attributes.Append(attrib);
                attrib = xmlDoc.CreateAttribute("Name");
                attrib.InnerText = "EFFECT OF DECHALLENGE (D) & RECHALLENGE (R)";
                attributeNode.Attributes.Append(attrib);
                attributeNode.InnerText = med.Effect;
                customHeadNode.AppendChild(attributeNode);

                medicationNode.AppendChild(customHeadNode);
                medicationHeadNode.AppendChild(medicationNode);
            }
        }

        private void PrepareEvaluationsXML(ref XmlDocument xmlDoc, ref XmlNode evaluationHeadNode, string ns)
        {
            XmlNode labTestNode;
            XmlNode customHeadNode;
            XmlNode attributeNode;
            XmlAttribute attrib;

            // Write evaluation
            foreach (EvaluationItem eva in _evaluations)
            {
                labTestNode = xmlDoc.CreateElement("LabTest", ns);
                attrib = xmlDoc.CreateAttribute("guid");
                attrib.InnerText = eva.PatientEvaluationGuid;
                labTestNode.Attributes.Append(attrib);
                attrib = xmlDoc.CreateAttribute("TestResultValue");
                attrib.InnerText = eva.TestResultValue;
                labTestNode.Attributes.Append(attrib);
                attrib = xmlDoc.CreateAttribute("TestUnit");
                attrib.InnerText = eva.TestUnit;
                labTestNode.Attributes.Append(attrib);
                attrib = xmlDoc.CreateAttribute("Test");
                attrib.InnerText = eva.Test;
                labTestNode.Attributes.Append(attrib);
                attrib = xmlDoc.CreateAttribute("TestResultCoded");
                attrib.InnerText = eva.TestResultCoded;
                labTestNode.Attributes.Append(attrib);
                attrib = xmlDoc.CreateAttribute("TestDate");
                attrib.InnerText = eva.DateTest != null ? Convert.ToDateTime(eva.DateTest).ToString("yyyy-MM-dd") : "";
                labTestNode.Attributes.Append(attrib);

                customHeadNode = xmlDoc.CreateElement("Customattributes", ns);

                attributeNode = xmlDoc.CreateElement("AttributeKey", ns);
                attrib = xmlDoc.CreateAttribute("Archive");
                attrib.InnerText = "false";
                attributeNode.Attributes.Append(attrib);
                attrib = xmlDoc.CreateAttribute("Name");
                attrib.InnerText = "Remarks";
                attributeNode.Attributes.Append(attrib);
                attributeNode.InnerText = eva.Remarks;
                customHeadNode.AppendChild(attributeNode);

                labTestNode.AppendChild(customHeadNode);
                evaluationHeadNode.AppendChild(labTestNode);
            }
        }

        private void PrepareConditionsXML(ref XmlDocument xmlDoc, ref XmlNode conditionHeadNode, string ns)
        {
            XmlNode conditionNode;
            XmlNode customHeadNode;
            XmlNode attributeNode;
            XmlAttribute attrib;

            // Write condition
            foreach (ConditionItem con in _conditions)
            {
                conditionNode = xmlDoc.CreateElement("Condition", ns);
                attrib = xmlDoc.CreateAttribute("guid");
                attrib.InnerText = con.PatientConditionGuid;
                conditionNode.Attributes.Append(attrib);
                attrib = xmlDoc.CreateAttribute("DateEnd");
                attrib.InnerText = con.DateEnd != null ? Convert.ToDateTime(con.DateEnd).ToString("yyyy-MM-dd") : "";
                conditionNode.Attributes.Append(attrib);
                attrib = xmlDoc.CreateAttribute("DateStart");
                attrib.InnerText = con.DateStart != null ? Convert.ToDateTime(con.DateStart).ToString("yyyy-MM-dd") : "";
                conditionNode.Attributes.Append(attrib);
                attrib = xmlDoc.CreateAttribute("TreatmentOutcome");
                attrib.InnerText = con.TreatmentOutcome;
                conditionNode.Attributes.Append(attrib);
                attrib = xmlDoc.CreateAttribute("Outcome");
                attrib.InnerText = con.ConditionOutcome;
                conditionNode.Attributes.Append(attrib);
                attrib = xmlDoc.CreateAttribute("Terminology");
                attrib.InnerText = con.Terminology;
                conditionNode.Attributes.Append(attrib);

                customHeadNode = xmlDoc.CreateElement("Customattributes", ns);

                attributeNode = xmlDoc.CreateElement("AttributeKey", ns);
                attrib = xmlDoc.CreateAttribute("Archive");
                attrib.InnerText = "false";
                attributeNode.Attributes.Append(attrib);
                attrib = xmlDoc.CreateAttribute("Name");
                attrib.InnerText = "Condition Ongoing";
                attributeNode.Attributes.Append(attrib);
                attributeNode.InnerText = con.ConditionOngoing;
                customHeadNode.AppendChild(attributeNode);

                conditionNode.AppendChild(customHeadNode);
                conditionHeadNode.AppendChild(conditionNode);
            }
        }

        private void PrepareEventsXML(ref XmlDocument xmlDoc, ref XmlNode eventHeadNode, string ns)
        {
            XmlNode eventNode;
            XmlNode customHeadNode;
            XmlNode attributeNode;
            XmlAttribute attrib;

            // Write clinical event
            foreach (ClinicalEventItem cli in _events)
            {
                eventNode = xmlDoc.CreateElement("ClinicalEvent", ns);
                attrib = xmlDoc.CreateAttribute("guid");
                attrib.InnerText = cli.PatientClinicalEventGuid;
                eventNode.Attributes.Append(attrib);
                attrib = xmlDoc.CreateAttribute("SourceDescription");
                attrib.InnerText = cli.Description;
                eventNode.Attributes.Append(attrib);
                attrib = xmlDoc.CreateAttribute("SourceTerminology");
                attrib.InnerText = cli.Terminology;
                eventNode.Attributes.Append(attrib);
                attrib = xmlDoc.CreateAttribute("ResolutionDate");
                attrib.InnerText = cli.DateResolution != null ? Convert.ToDateTime(cli.DateResolution).ToString("yyyy-MM-dd") : "";
                eventNode.Attributes.Append(attrib);
                attrib = xmlDoc.CreateAttribute("OnsetDate");
                attrib.InnerText = cli.DateOnset != null ? Convert.ToDateTime(cli.DateOnset).ToString("yyyy-MM-dd") : "";
                eventNode.Attributes.Append(attrib);

                customHeadNode = xmlDoc.CreateElement("Customattributes", ns);

                attributeNode = xmlDoc.CreateElement("AttributeKey", ns);
                attrib = xmlDoc.CreateAttribute("Archive");
                attrib.InnerText = "false";
                attributeNode.Attributes.Append(attrib);
                attrib = xmlDoc.CreateAttribute("Name");
                attrib.InnerText = "Intensity (Severity)";
                attributeNode.Attributes.Append(attrib);
                attributeNode.InnerText = cli.Intensity;
                customHeadNode.AppendChild(attributeNode);

                attributeNode = xmlDoc.CreateElement("AttributeKey", ns);
                attrib = xmlDoc.CreateAttribute("Archive");
                attrib.InnerText = "false";
                attributeNode.Attributes.Append(attrib);
                attrib = xmlDoc.CreateAttribute("Name");
                attrib.InnerText = "Treatment of Reaction";
                attributeNode.Attributes.Append(attrib);
                attributeNode.InnerText = cli.TreatmentReaction;
                customHeadNode.AppendChild(attributeNode);

                attributeNode = xmlDoc.CreateElement("AttributeKey", ns);
                attrib = xmlDoc.CreateAttribute("Archive");
                attrib.InnerText = "false";
                attributeNode.Attributes.Append(attrib);
                attrib = xmlDoc.CreateAttribute("Name");
                attrib.InnerText = "Was the AE attributed to one or more drugs?";
                attributeNode.Attributes.Append(attrib);
                attributeNode.InnerText = cli.Attributed;
                customHeadNode.AppendChild(attributeNode);

                attributeNode = xmlDoc.CreateElement("AttributeKey", ns);
                attrib = xmlDoc.CreateAttribute("Archive");
                attrib.InnerText = "false";
                attributeNode.Attributes.Append(attrib);
                attrib = xmlDoc.CreateAttribute("Name");
                attrib.InnerText = "Expected or Unexpected AE";
                attributeNode.Attributes.Append(attrib);
                attributeNode.InnerText = cli.Expected;
                customHeadNode.AppendChild(attributeNode);

                attributeNode = xmlDoc.CreateElement("AttributeKey", ns);
                attrib = xmlDoc.CreateAttribute("Archive");
                attrib.InnerText = "false";
                attributeNode.Attributes.Append(attrib);
                attrib = xmlDoc.CreateAttribute("Name");
                attrib.InnerText = "Outcome";
                attributeNode.Attributes.Append(attrib);
                attributeNode.InnerText = cli.Outcome;
                customHeadNode.AppendChild(attributeNode);

                attributeNode = xmlDoc.CreateElement("AttributeKey", ns);
                attrib = xmlDoc.CreateAttribute("Archive");
                attrib.InnerText = "false";
                attributeNode.Attributes.Append(attrib);
                attrib = xmlDoc.CreateAttribute("Name");
                attrib.InnerText = "Was the event reported to national PV?";
                attributeNode.Attributes.Append(attrib);
                attributeNode.InnerText = cli.Reported;
                customHeadNode.AppendChild(attributeNode);

                attributeNode = xmlDoc.CreateElement("AttributeKey", ns);
                attrib = xmlDoc.CreateAttribute("Archive");
                attrib.InnerText = "false";
                attributeNode.Attributes.Append(attrib);
                attrib = xmlDoc.CreateAttribute("Name");
                attrib.InnerText = "Is the adverse event serious?";
                attributeNode.Attributes.Append(attrib);
                attributeNode.InnerText = cli.IsSerious;
                customHeadNode.AppendChild(attributeNode);

                attributeNode = xmlDoc.CreateElement("AttributeKey", ns);
                attrib = xmlDoc.CreateAttribute("Archive");
                attrib.InnerText = "false";
                attributeNode.Attributes.Append(attrib);
                attrib = xmlDoc.CreateAttribute("Name");
                attrib.InnerText = "Seriousness";
                attributeNode.Attributes.Append(attrib);
                attributeNode.InnerText = cli.Seriousness;
                customHeadNode.AppendChild(attributeNode);

                attributeNode = xmlDoc.CreateElement("AttributeKey", ns);
                attrib = xmlDoc.CreateAttribute("Archive");
                attrib.InnerText = "false";
                attributeNode.Attributes.Append(attrib);
                attrib = xmlDoc.CreateAttribute("Name");
                attrib.InnerText = "Admission Date";
                attributeNode.Attributes.Append(attrib);
                attributeNode.InnerText = cli.DateAdmission != null ? Convert.ToDateTime(cli.DateAdmission).ToString("yyyy-MM-dd") : ""; ;
                customHeadNode.AppendChild(attributeNode);

                attributeNode = xmlDoc.CreateElement("AttributeKey", ns);
                attrib = xmlDoc.CreateAttribute("Archive");
                attrib.InnerText = "false";
                attributeNode.Attributes.Append(attrib);
                attrib = xmlDoc.CreateAttribute("Name");
                attrib.InnerText = "Discharge Date";
                attributeNode.Attributes.Append(attrib);
                attributeNode.InnerText = cli.DateDischarge != null ? Convert.ToDateTime(cli.DateDischarge).ToString("yyyy-MM-dd") : ""; ;
                customHeadNode.AppendChild(attributeNode);

                attributeNode = xmlDoc.CreateElement("AttributeKey", ns);
                attrib = xmlDoc.CreateAttribute("Archive");
                attrib.InnerText = "false";
                attributeNode.Attributes.Append(attrib);
                attrib = xmlDoc.CreateAttribute("Name");
                attrib.InnerText = "Date of Death";
                attributeNode.Attributes.Append(attrib);
                attributeNode.InnerText = cli.DateDeath != null ? Convert.ToDateTime(cli.DateDeath).ToString("yyyy-MM-dd") : ""; ; ;
                customHeadNode.AppendChild(attributeNode);

                attributeNode = xmlDoc.CreateElement("AttributeKey", ns);
                attrib = xmlDoc.CreateAttribute("Archive");
                attrib.InnerText = "false";
                attributeNode.Attributes.Append(attrib);
                attrib = xmlDoc.CreateAttribute("Name");
                attrib.InnerText = "Autopsy Done";
                attributeNode.Attributes.Append(attrib);
                attributeNode.InnerText = cli.Autopsy;
                customHeadNode.AppendChild(attributeNode);

                attributeNode = xmlDoc.CreateElement("AttributeKey", ns);
                attrib = xmlDoc.CreateAttribute("Archive");
                attrib.InnerText = "false";
                attributeNode.Attributes.Append(attrib);
                attrib = xmlDoc.CreateAttribute("Name");
                attrib.InnerText = "Severity Grading Scale";
                attributeNode.Attributes.Append(attrib);
                attributeNode.InnerText = cli.Scale;
                customHeadNode.AppendChild(attributeNode);

                attributeNode = xmlDoc.CreateElement("AttributeKey", ns);
                attrib = xmlDoc.CreateAttribute("Archive");
                attrib.InnerText = "false";
                attributeNode.Attributes.Append(attrib);
                attrib = xmlDoc.CreateAttribute("Name");
                attrib.InnerText = "Severity Grade";
                attributeNode.Attributes.Append(attrib);
                attributeNode.InnerText = cli.Grade;
                customHeadNode.AppendChild(attributeNode);

                attributeNode = xmlDoc.CreateElement("AttributeKey", ns);
                attrib = xmlDoc.CreateAttribute("Archive");
                attrib.InnerText = "false";
                attributeNode.Attributes.Append(attrib);
                attrib = xmlDoc.CreateAttribute("Name");
                attrib.InnerText = "Full Name of Reporter";
                attributeNode.Attributes.Append(attrib);
                attributeNode.InnerText = cli.FullNameReporter;
                customHeadNode.AppendChild(attributeNode);

                attributeNode = xmlDoc.CreateElement("AttributeKey", ns);
                attrib = xmlDoc.CreateAttribute("Archive");
                attrib.InnerText = "false";
                attributeNode.Attributes.Append(attrib);
                attrib = xmlDoc.CreateAttribute("Name");
                attrib.InnerText = "Date of Report";
                attributeNode.Attributes.Append(attrib);
                attributeNode.InnerText = cli.DateReport != null ? Convert.ToDateTime(cli.DateReport).ToString("yyyy-MM-dd") : ""; ; ;
                customHeadNode.AppendChild(attributeNode);

                attributeNode = xmlDoc.CreateElement("AttributeKey", ns);
                attrib = xmlDoc.CreateAttribute("Archive");
                attrib.InnerText = "false";
                attributeNode.Attributes.Append(attrib);
                attrib = xmlDoc.CreateAttribute("Name");
                attrib.InnerText = "Type of Reporter";
                attributeNode.Attributes.Append(attrib);
                attributeNode.InnerText = cli.TypeReporter;
                customHeadNode.AppendChild(attributeNode);

                attributeNode = xmlDoc.CreateElement("AttributeKey", ns);
                attrib = xmlDoc.CreateAttribute("Archive");
                attrib.InnerText = "false";
                attributeNode.Attributes.Append(attrib);
                attrib = xmlDoc.CreateAttribute("Name");
                attrib.InnerText = "Reporter Contact Number";
                attributeNode.Attributes.Append(attrib);
                attributeNode.InnerText = cli.ContactNumberReporter;
                customHeadNode.AppendChild(attributeNode);

                attributeNode = xmlDoc.CreateElement("AttributeKey", ns);
                attrib = xmlDoc.CreateAttribute("Archive");
                attrib.InnerText = "false";
                attributeNode.Attributes.Append(attrib);
                attrib = xmlDoc.CreateAttribute("Name");
                attrib.InnerText = "FDA SAE Number";
                attributeNode.Attributes.Append(attrib);
                attributeNode.InnerText = cli.SAENumber;
                customHeadNode.AppendChild(attributeNode);

                eventNode.AppendChild(customHeadNode);
                eventHeadNode.AppendChild(eventNode);
            }
        }

        private void PrepareEncountersXML(ref XmlDocument xmlDoc, ref XmlNode encounterHeadNode, string ns)
        {
            XmlNode encounterNode;
            XmlNode instanceHeadNode;
            XmlNode valueNode;
            XmlAttribute attrib;

            // Write encounter
            foreach (EncounterItem enc in _encounters)
            {
                encounterNode = xmlDoc.CreateElement("Encounter", ns);
                attrib = xmlDoc.CreateAttribute("guid");
                attrib.InnerText = enc.EncounterGuid;
                encounterNode.Attributes.Append(attrib);
                attrib = xmlDoc.CreateAttribute("CreatedDate");
                attrib.InnerText = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                encounterNode.Attributes.Append(attrib);
                attrib = xmlDoc.CreateAttribute("LastUpdated");
                attrib.InnerText = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                encounterNode.Attributes.Append(attrib);
                attrib = xmlDoc.CreateAttribute("EncounterType");
                attrib.InnerText = enc.Type;
                encounterNode.Attributes.Append(attrib);
                attrib = xmlDoc.CreateAttribute("EncounterDate");
                attrib.InnerText = enc.DateEncounter != null ? Convert.ToDateTime(enc.DateEncounter).ToString("yyyy-MM-dd") : "";
                encounterNode.Attributes.Append(attrib);

                instanceHeadNode = xmlDoc.CreateElement("InstanceValues", ns);

                valueNode = xmlDoc.CreateElement("InstanceValue", ns);
                attrib = xmlDoc.CreateAttribute("Archive");
                attrib.InnerText = "false";
                valueNode.Attributes.Append(attrib);
                attrib = xmlDoc.CreateAttribute("ElementName");
                attrib.InnerText = "Weight (kg)";
                valueNode.Attributes.Append(attrib);
                valueNode.InnerText = enc.Weight;
                instanceHeadNode.AppendChild(valueNode);

                valueNode = xmlDoc.CreateElement("InstanceValue", ns);
                attrib = xmlDoc.CreateAttribute("Archive");
                attrib.InnerText = "false";
                valueNode.Attributes.Append(attrib);
                attrib = xmlDoc.CreateAttribute("ElementName");
                attrib.InnerText = "Height (cm)";
                valueNode.Attributes.Append(attrib);
                valueNode.InnerText = enc.Height;
                instanceHeadNode.AppendChild(valueNode);

                valueNode = xmlDoc.CreateElement("InstanceValue", ns);
                attrib = xmlDoc.CreateAttribute("Archive");
                attrib.InnerText = "false";
                valueNode.Attributes.Append(attrib);
                attrib = xmlDoc.CreateAttribute("ElementName");
                attrib.InnerText = "Pregnancy Status";
                valueNode.Attributes.Append(attrib);
                valueNode.InnerText = enc.PregnancyStatus;
                instanceHeadNode.AppendChild(valueNode);

                valueNode = xmlDoc.CreateElement("InstanceValue", ns);
                attrib = xmlDoc.CreateAttribute("Archive");
                attrib.InnerText = "false";
                valueNode.Attributes.Append(attrib);
                attrib = xmlDoc.CreateAttribute("ElementName");
                attrib.InnerText = "Date of last menstrual period";
                valueNode.Attributes.Append(attrib);
                valueNode.InnerText = enc.DateLMP != null ? Convert.ToDateTime(enc.DateLMP).ToString("yyyy-MM-dd") : "";
                instanceHeadNode.AppendChild(valueNode);

                valueNode = xmlDoc.CreateElement("InstanceValue", ns);
                attrib = xmlDoc.CreateAttribute("Archive");
                attrib.InnerText = "false";
                valueNode.Attributes.Append(attrib);
                attrib = xmlDoc.CreateAttribute("ElementName");
                attrib.InnerText = "Estimated gestation (weeks)";
                valueNode.Attributes.Append(attrib);
                valueNode.InnerText = enc.Gestation;
                instanceHeadNode.AppendChild(valueNode);

                valueNode = xmlDoc.CreateElement("InstanceValue", ns);
                attrib = xmlDoc.CreateAttribute("Archive");
                attrib.InnerText = "false";
                valueNode.Attributes.Append(attrib);
                attrib = xmlDoc.CreateAttribute("ElementName");
                attrib.InnerText = "Breastfeeding mother";
                valueNode.Attributes.Append(attrib);
                valueNode.InnerText = enc.Breastfeeding;
                instanceHeadNode.AppendChild(valueNode);

                valueNode = xmlDoc.CreateElement("InstanceValue", ns);
                attrib = xmlDoc.CreateAttribute("Archive");
                attrib.InnerText = "false";
                valueNode.Attributes.Append(attrib);
                attrib = xmlDoc.CreateAttribute("ElementName");
                attrib.InnerText = "Injecting Drug Use Within Past Year";
                valueNode.Attributes.Append(attrib);
                valueNode.InnerText = enc.Drug;
                instanceHeadNode.AppendChild(valueNode);

                valueNode = xmlDoc.CreateElement("InstanceValue", ns);
                attrib = xmlDoc.CreateAttribute("Archive");
                attrib.InnerText = "false";
                valueNode.Attributes.Append(attrib);
                attrib = xmlDoc.CreateAttribute("ElementName");
                attrib.InnerText = "Excessive alcohol use within the past year";
                valueNode.Attributes.Append(attrib);
                valueNode.InnerText = enc.Alcahol;
                instanceHeadNode.AppendChild(valueNode);

                valueNode = xmlDoc.CreateElement("InstanceValue", ns);
                attrib = xmlDoc.CreateAttribute("Archive");
                attrib.InnerText = "false";
                valueNode.Attributes.Append(attrib);
                attrib = xmlDoc.CreateAttribute("ElementName");
                attrib.InnerText = "Tobacco use within the past year";
                valueNode.Attributes.Append(attrib);
                valueNode.InnerText = enc.Tobacco;
                instanceHeadNode.AppendChild(valueNode);

                valueNode = xmlDoc.CreateElement("InstanceValue", ns);
                attrib = xmlDoc.CreateAttribute("Archive");
                attrib.InnerText = "false";
                valueNode.Attributes.Append(attrib);
                attrib = xmlDoc.CreateAttribute("ElementName");
                attrib.InnerText = "Baseline Chest Xray Presentation";
                valueNode.Attributes.Append(attrib);
                valueNode.InnerText = enc.XRay;
                instanceHeadNode.AppendChild(valueNode);

                valueNode = xmlDoc.CreateElement("InstanceValue", ns);
                attrib = xmlDoc.CreateAttribute("Archive");
                attrib.InnerText = "false";
                valueNode.Attributes.Append(attrib);
                attrib = xmlDoc.CreateAttribute("ElementName");
                attrib.InnerText = "Previous TB treatment?";
                valueNode.Attributes.Append(attrib);
                valueNode.InnerText = enc.PreviousTBTx;
                instanceHeadNode.AppendChild(valueNode);

                valueNode = xmlDoc.CreateElement("InstanceValue", ns);
                attrib = xmlDoc.CreateAttribute("Archive");
                attrib.InnerText = "false";
                valueNode.Attributes.Append(attrib);
                attrib = xmlDoc.CreateAttribute("ElementName");
                attrib.InnerText = "Site of TB";
                valueNode.Attributes.Append(attrib);
                valueNode.InnerText = enc.SiteTB;
                instanceHeadNode.AppendChild(valueNode);

                valueNode = xmlDoc.CreateElement("InstanceValue", ns);
                attrib = xmlDoc.CreateAttribute("Archive");
                attrib.InnerText = "false";
                valueNode.Attributes.Append(attrib);
                attrib = xmlDoc.CreateAttribute("ElementName");
                attrib.InnerText = "Documented HIV infection";
                valueNode.Attributes.Append(attrib);
                valueNode.InnerText = enc.Documented;
                instanceHeadNode.AppendChild(valueNode);

                valueNode = xmlDoc.CreateElement("InstanceValue", ns);
                attrib = xmlDoc.CreateAttribute("Archive");
                attrib.InnerText = "false";
                valueNode.Attributes.Append(attrib);
                attrib = xmlDoc.CreateAttribute("ElementName");
                attrib.InnerText = "Ever received treatment with first line anti-TB drugs for >-1 month prior to this episode?";
                valueNode.Attributes.Append(attrib);
                valueNode.InnerText = enc.FirstLine;
                instanceHeadNode.AppendChild(valueNode);

                valueNode = xmlDoc.CreateElement("InstanceValue", ns);
                attrib = xmlDoc.CreateAttribute("Archive");
                attrib.InnerText = "false";
                valueNode.Attributes.Append(attrib);
                attrib = xmlDoc.CreateAttribute("ElementName");
                attrib.InnerText = "Ever received treatment with second line anti-TB drugs for >-1 month prior to this episode?";
                valueNode.Attributes.Append(attrib);
                valueNode.InnerText = enc.SecondLine;
                instanceHeadNode.AppendChild(valueNode);

                valueNode = xmlDoc.CreateElement("InstanceValue", ns);
                attrib = xmlDoc.CreateAttribute("Archive");
                attrib.InnerText = "false";
                valueNode.Attributes.Append(attrib);
                attrib = xmlDoc.CreateAttribute("ElementName");
                attrib.InnerText = "Isoniazid susceptibility by any laboratory test(s)";
                valueNode.Attributes.Append(attrib);
                valueNode.InnerText = enc.IsoniazidSusc;
                instanceHeadNode.AppendChild(valueNode);

                valueNode = xmlDoc.CreateElement("InstanceValue", ns);
                attrib = xmlDoc.CreateAttribute("Archive");
                attrib.InnerText = "false";
                valueNode.Attributes.Append(attrib);
                attrib = xmlDoc.CreateAttribute("ElementName");
                attrib.InnerText = "Isoniazid confirmation";
                valueNode.Attributes.Append(attrib);
                valueNode.InnerText = enc.IsoniazidConf;
                instanceHeadNode.AppendChild(valueNode);

                valueNode = xmlDoc.CreateElement("InstanceValue", ns);
                attrib = xmlDoc.CreateAttribute("Archive");
                attrib.InnerText = "false";
                valueNode.Attributes.Append(attrib);
                attrib = xmlDoc.CreateAttribute("ElementName");
                attrib.InnerText = "Rifampicin susceptibility by any laboratory test(s)";
                valueNode.Attributes.Append(attrib);
                valueNode.InnerText = enc.RifampSusc;
                instanceHeadNode.AppendChild(valueNode);

                valueNode = xmlDoc.CreateElement("InstanceValue", ns);
                attrib = xmlDoc.CreateAttribute("Archive");
                attrib.InnerText = "false";
                valueNode.Attributes.Append(attrib);
                attrib = xmlDoc.CreateAttribute("ElementName");
                attrib.InnerText = "Rifampicin confirmation";
                valueNode.Attributes.Append(attrib);
                valueNode.InnerText = enc.RifampConf;
                instanceHeadNode.AppendChild(valueNode);

                encounterNode.AppendChild(instanceHeadNode);
                encounterHeadNode.AppendChild(encounterNode);
            }
        }

        private string FormatXML(XmlDocument doc)
        {
            MemoryStream memoryStream = new MemoryStream();

            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.IndentChars = "  ";
            settings.NewLineChars = "\r\n";
            settings.NewLineHandling = NewLineHandling.Replace;
            settings.Encoding = new UTF8Encoding(false);
            using (XmlWriter writer = XmlWriter.Create(memoryStream, settings))
            {
                doc.Save(writer);
            }

            return Encoding.UTF8.GetString(memoryStream.ToArray());
        }

        private void WriteXML(string xmlFileName, string xmlText)
        {
            string line = "********************************************************************";

            // Write the string to a file.
            StreamWriter file = new System.IO.StreamWriter(xmlFileName, false, Encoding.UTF8);

            file.Write(xmlText);

            file.Close();
            file = null;
        }

        #endregion

        #region "Post"

        private void btnPost_Click(object sender, EventArgs e)
        {
            txtPost.Text = string.Empty;

            if(txtXML.Text == "")
            {
                AddLog("Please generate the XML payload first...");
                return;
            }
            _payload = txtXML.Text;

            Cursor.Current = Cursors.WaitCursor;

            //string xmlIdentifier = String.Format("I_PatientEnrollment-{0}", DateTime.Now.ToString("yyyyMMddhhmmss"));
            _log = string.Empty;

            XmlDocument xmlDoc = new XmlDocument();
            XmlNode rootNode;

            string contentXml = _payload;
            //string url = "http://pvims-phil.azurewebsites.net/webservice/InteropGeneric.asmx";
            string url = cboEndPoint.Text;

            AddLog("Preparing XML for delivery");

            string soapAction = "http://pvims.org/webservices/Interop_Push";
            string response;

            // Escape xml string for sending
            contentXml = contentXml.Replace("&", "&amp;");
            contentXml = contentXml.Replace("\"", "&quot;");
            contentXml = contentXml.Replace("<", "&lt;");
            contentXml = contentXml.Replace(">", "&gt;");
            contentXml = contentXml.Replace("'", "&apos;");

            string soapRequest =
                "<soap:Envelope xmlns:xsi='http://www.w3.org/2001/XMLSchema-instance' xmlns:xsd='http://www.w3.org/2001/XMLSchema' xmlns:soap='http://schemas.xmlsoap.org/soap/envelope/'>" + "\n" +
                "<soap:Body>  " + "\n" +
                "<Interop_Push xmlns='http://pvims.org/webservices/'>" + "\n" +
                "<payload>" + contentXml + "</payload>" + "\n" +
                "<subscriber>" + txtSubscriber.Text.Trim() + "</subscriber>" + "\n" +
                "</Interop_Push>" + "\n" +
                "</soap:Body>" + "\n" +
                "</soap:Envelope>" + "\n";

            try
            {
                response = PostWebservice(url, soapAction, soapRequest, 30000);

                xmlDoc.LoadXml(response);

                // Extract response
                rootNode = xmlDoc.DocumentElement;

                contentXml = rootNode.InnerXml;
                contentXml = contentXml.Replace("&amp;", "&");
                contentXml = contentXml.Replace("&quot;", "\"");
                contentXml = contentXml.Replace("&lt;", "<");
                contentXml = contentXml.Replace("&gt;", ">");
                contentXml = contentXml.Replace("&apos;", "'");

                AddLog("Response | " + contentXml);
                if (rootNode.InnerText.StartsWith("09"))
                    AddLog("Method failed " + rootNode.InnerText);
                else
                    AddLog("Post completed successfully");
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null)
                    AddLog("Method failed " + ex.Message + " ~ " + ex.InnerException.Message);
                else
                    AddLog("Method failed " + ex.Message);

                contentXml = soapRequest;
                contentXml = contentXml.Replace("&amp;", "&");
                contentXml = contentXml.Replace("&quot;", "\"");
                contentXml = contentXml.Replace("&lt;", "<");
                contentXml = contentXml.Replace("&gt;", ">");
                contentXml = contentXml.Replace("&apos;", "'");

                AddLog(contentXml);
            }

            //clean up
            rootNode = null;
            xmlDoc = null;
            
            Cursor.Current = Cursors.Default;
        }

        private void AddLog(string log)
        {
            StringBuilder sb = new StringBuilder(_log);
            sb.AppendLine(string.Format("{0}: {1}", DateTime.Now.ToShortTimeString(), log));
            _log = sb.ToString();

            txtPost.Text = _log;
        }

        private string PostWebservice(string asmxUrl, string soapActionUrl, string xmlBody, int timeout)
        {
            string response = string.Empty;
            HttpWebRequest myReq = (HttpWebRequest)WebRequest.Create(asmxUrl);
            HttpWebResponse myResponse;

            ASCIIEncoding encoding = new ASCIIEncoding();
            Byte[] buffer = encoding.GetBytes(xmlBody);
            Stream post;
            Stream responseData;
            StreamReader responseReader;

            myReq.Method = "Post";
            myReq.Timeout = timeout;
            myReq.ContentType = "text/xml;charset=\"utf-8\"";
            myReq.Accept = "text/xml";
            myReq.ContentLength = buffer.Length;
            myReq.Headers.Add("SOAPAction", soapActionUrl);

            //myReq.Credentials = New NetworkCredential("abc", "123")
            //myReq.PreAuthenticate = True
            //proxyaddress = proxy.GetProxy(myReq.RequestUri).ToString

            //Dim newUri As New Uri(proxyaddress)
            //myProxy.Address = newUri
            //myReq.Proxy = myProxy

            post = myReq.GetRequestStream();
            post.Write(buffer, 0, buffer.Length);
            post.Close();

            myResponse = (HttpWebResponse)myReq.GetResponse();
            responseData = myResponse.GetResponseStream();
            responseReader = new StreamReader(responseData);

            response = responseReader.ReadToEnd();

            return response;
        }
        #endregion

        #region "Events"

        private void btnGenPatientGUID_Click(object sender, EventArgs e)
        {
            txtPatientGUID.Text = Guid.NewGuid().ToString();
        }

        private void btnMedicationGUID_Click(object sender, EventArgs e)
        {
            txtMedicationGUID.Text = Guid.NewGuid().ToString();
        }

        private void btnLabTestGUID_Click(object sender, EventArgs e)
        {
            txtLabTestGUID.Text = Guid.NewGuid().ToString();
        }

        private void btnAddConditionGUID_Click(object sender, EventArgs e)
        {
            txtConditionGUID.Text = Guid.NewGuid().ToString();
        }

        private void btnClinicalEventGUID_Click(object sender, EventArgs e)
        {
            txtClinicalEventGUID.Text = Guid.NewGuid().ToString();
        }

        private void btmEncounterGUID_Click(object sender, EventArgs e)
        {
            txtEncounterGUID.Text = Guid.NewGuid().ToString();
        }

        private void btnAddMedication_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(txtMedicationGUID.Text) || String.IsNullOrEmpty(txtMedication.Text))
            {
                MessageBox.Show("GUID and Medication mandatory");
                return;
            }

            MedicationItem med = new MedicationItem();

            med.Action = txtClinicianAction.Text;
            med.BatchNumber = txtBatchNumber.Text;
            med.DateEnd = dtpMedicationEnd.Value;
            med.DateStart = dtpMedicationStart.Value;
            med.DaysWeek = txtDaysWeek.Text;
            med.Dose = txtDose.Text;
            med.DoseUnit = txtDoseUnit.Text;
            med.Effect = txtEffect.Text;
            med.Frequency = txtDoseFrequency.Text;
            med.Indication = txtIndication.Text;
            med.Medication = txtMedication.Text;
            med.PatientMedicationGuid = txtMedicationGUID.Text;
            med.ReasonForStopping = txtReasonForStopping.Text;
            med.Route = txtRoute.Text;
            med.StillOnMedication = txtStillOnMedication.Text;
            med.TypeofIndication = txtTypeofIndication.Text;

            _medications.Add(med);
            PopulateMedications();

            txtClinicianAction.Text = string.Empty;
            txtBatchNumber.Text = string.Empty;
            dtpMedicationEnd.Value = DateTime.Today;
            dtpMedicationStart.Value = DateTime.Today;
            txtDaysWeek.Text = string.Empty; ;
            txtDose.Text = string.Empty; ;
            txtDoseUnit.Text = string.Empty; ;
            txtEffect.Text = string.Empty; ;
            txtDoseFrequency.Text = string.Empty; ;
            txtIndication.Text = string.Empty; ;
            txtMedication.Text = string.Empty; ;
            txtMedicationGUID.Text = string.Empty; ;
            txtReasonForStopping.Text = string.Empty; ;
            txtRoute.Text = string.Empty; ;
            txtStillOnMedication.Text = string.Empty; ;
            txtTypeofIndication.Text = string.Empty; ;
        }

        protected void medication_Click(object sender, EventArgs e)
        {
            Button button = sender as Button;
            _medications.RemoveAt(Convert.ToInt32(button.Tag));
            PopulateMedications();
        }

        private void btnAddLabTest_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(txtLabTestGUID.Text) || String.IsNullOrEmpty(txtLabTest.Text))
            {
                MessageBox.Show("GUID and Lab Test mandatory");
                return;
            }

            EvaluationItem eva = new EvaluationItem();

            eva.DateTest = dtpLabTestDate.Value;
            eva.Test = txtLabTest.Text;
            eva.TestResultValue = txtLabValue.Text;
            eva.PatientEvaluationGuid = txtLabTestGUID.Text;
            eva.Remarks = txtRemarks.Text;
            eva.TestResultCoded = txtTestResult.Text;
            eva.TestUnit = txtLabTestUnit.Text;

            _evaluations.Add(eva);
            PopulateEvaluations();

            dtpLabTestDate.Value = DateTime.Today;
            txtLabTest.Text = string.Empty;
            txtLabValue.Text = string.Empty;
            txtLabTestGUID.Text = string.Empty;
            txtRemarks.Text = string.Empty;
            txtTestResult.Text = string.Empty;
            txtLabTestUnit.Text = string.Empty;
        }

        protected void evaluation_Click(object sender, EventArgs e)
        {
            Button button = sender as Button;
            _evaluations.RemoveAt(Convert.ToInt32(button.Tag));
            PopulateEvaluations();
        }

        private void btnAddCondition_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(txtConditionGUID.Text) || String.IsNullOrEmpty(txtTerminology.Text))
            {
                MessageBox.Show("GUID and Terminology mandatory");
                return;
            }

            ConditionItem con = new ConditionItem();

            con.ConditionOngoing = txtConditionOngoing.Text;
            con.DateEnd = dtpConditionEnd.Value;
            con.DateStart = dtpConditionStart.Value;
            con.TreatmentOutcome = txtTreatmentOutcome.Text;
            con.ConditionOutcome = txtConditionOutcome.Text;
            con.PatientConditionGuid = txtConditionGUID.Text;
            con.Terminology = txtTerminology.Text;

            _conditions.Add(con);
            PopulateConditions();

            txtConditionGUID.Text = string.Empty;
            txtTerminology.Text = string.Empty;
            dtpConditionEnd.Value = DateTime.Today;
            dtpConditionStart.Value = DateTime.Today;
            txtTreatmentOutcome.Text = string.Empty;
            txtConditionOutcome.Text = string.Empty;
            txtConditionOngoing.Text = string.Empty;
        }

        protected void condition_Click(object sender, EventArgs e)
        {
            Button button = sender as Button;
            _conditions.RemoveAt(Convert.ToInt32(button.Tag));
            PopulateConditions();
        }

        private void btnAddEvent_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(txtClinicalEventGUID.Text) || String.IsNullOrEmpty(txtSourceDescription.Text))
            {
                MessageBox.Show("GUID and Source Description mandatory");
                return;
            }

            ClinicalEventItem cli = new ClinicalEventItem();

            cli.Attributed = txtAttributed.Text;
            cli.Autopsy = txtAutopsyDone.Text;
            cli.ContactNumberReporter = txtReporterContactNumber.Text;
            cli.DateAdmission = dtpAdmissionDate.Value;
            cli.DateDeath = dtpDateofDeath.Value;
            cli.DateDischarge = dtpDischargeDate.Value;
            cli.DateOnset = dtpOnsetDate.Value;
            cli.DateReport = dtpReportDate.Value;
            cli.DateResolution = dtpResolutionDate.Value;
            cli.Description = txtSourceDescription.Text;
            cli.Expected = txtExpected.Text;
            cli.FullNameReporter = txtFullNameofReporter.Text;
            cli.Grade = txtSeverityGrade.Text;
            cli.Intensity = txtSeverityIntensity.Text;
            cli.IsSerious = txtSerious.Text;
            cli.Outcome = txtOutcome.Text;
            cli.PatientClinicalEventGuid = txtClinicalEventGUID.Text;
            cli.Reported = txtReported.Text;
            cli.SAENumber = txtSAENumber.Text;
            cli.Scale = txtSeverityGradingScale.Text;
            cli.Seriousness = txtSeriousness.Text;
            cli.Terminology = txtSourceTerminology.Text;
            cli.TreatmentReaction = txtReaction.Text;
            cli.TypeReporter = txtTypeofReporter.Text;

            _events.Add(cli);
            PopulateEvents();

            txtAttributed.Text = string.Empty;
            txtAutopsyDone.Text = string.Empty;
            txtReporterContactNumber.Text = string.Empty;
            dtpAdmissionDate.Value = DateTime.Today;
            dtpDateofDeath.Value = DateTime.Today;
            dtpDischargeDate.Value = DateTime.Today;
            dtpOnsetDate.Value = DateTime.Today;
            dtpReportDate.Value = DateTime.Today;
            dtpResolutionDate.Value = DateTime.Today;
            txtSourceDescription.Text = string.Empty;
            txtExpected.Text = string.Empty;
            txtFullNameofReporter.Text = string.Empty;
            txtSeverityGrade.Text = string.Empty;
            txtSeverityIntensity.Text = string.Empty;
            txtSerious.Text = string.Empty;
            txtOutcome.Text = string.Empty;
            txtClinicalEventGUID.Text = string.Empty;
            txtReported.Text = string.Empty;
            txtSAENumber.Text = string.Empty;
            txtSeverityGradingScale.Text = string.Empty;
            txtSeriousness.Text = string.Empty;
            txtSourceTerminology.Text = string.Empty;
            txtReaction.Text = string.Empty;
            txtTypeofReporter.Text = string.Empty;
        }

        protected void event_Click(object sender, EventArgs e)
        {
            Button button = sender as Button;
            _events.RemoveAt(Convert.ToInt32(button.Tag));
            PopulateEvents();
        }

        private void btnAddEncounter_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(txtEncounterGUID.Text) || String.IsNullOrEmpty(txtEncounterType.Text) || dtpEncounterDate.Checked == false)
            {
                MessageBox.Show("GUID, Type and Date mandatory");
                return;
            }

            EncounterItem enc = new EncounterItem();

            enc.Alcahol = txtExcessive.Text;
            enc.Breastfeeding = txtBreastfeeding.Text;
            enc.DateEncounter = dtpEncounterDate.Value;
            enc.DateLMP = dtpLMP.Value;
            enc.Documented = txtDocumented.Text;
            enc.Drug = txtInjecting.Text;
            enc.EncounterGuid = txtEncounterGUID.Text;
            enc.FirstLine = txtFirst.Text;
            enc.Gestation = txtGestation.Text;
            enc.Height = txtHeight.Text;
            enc.SiteTB = txtTBSite.Text;
            enc.IsoniazidConf = txtIsoniazidConfirmation.Text;
            enc.IsoniazidSusc = txtIsoniazidSusceptibility.Text;
            enc.PregnancyStatus = txtPregnancyStatus.Text;
            enc.PreviousTBTx = txtPreviousTBTreatment.Text;
            enc.RifampConf = txtRifampicinConfirmation.Text;
            enc.RifampSusc = txtRifampicinSusceptibility.Text;
            enc.SecondLine = txtSecond.Text;
            enc.Tobacco = txtTobacco.Text;
            enc.Type = txtEncounterType.Text;
            enc.Weight = txtWeight.Text;
            enc.XRay = txtBaselinePresentation.Text;

            _encounters.Add(enc);
            PopulateEncounters();

            txtExcessive.Text = string.Empty;
            txtBreastfeeding.Text = string.Empty;
            dtpEncounterDate.Value = DateTime.Today;
            dtpLMP.Value = DateTime.Today;
            txtDocumented.Text = string.Empty;
            txtInjecting.Text = string.Empty;
            txtEncounterGUID.Text = string.Empty;
            txtFirst.Text = string.Empty;
            txtGestation.Text = string.Empty;
            txtHeight.Text = string.Empty;
            txtTBSite.Text = string.Empty;
            txtIsoniazidConfirmation.Text = string.Empty;
            txtIsoniazidSusceptibility.Text = string.Empty;
            txtPregnancyStatus.Text = string.Empty;
            txtPreviousTBTreatment.Text = string.Empty;
            txtRifampicinConfirmation.Text = string.Empty;
            txtRifampicinSusceptibility.Text = string.Empty;
            txtSecond.Text = string.Empty;
            txtTobacco.Text = string.Empty;
            txtEncounterType.Text = string.Empty;
            txtWeight.Text = string.Empty;
            txtBaselinePresentation.Text = string.Empty;
        }

        protected void encounter_Click(object sender, EventArgs e)
        {
            Button button = sender as Button;
            _encounters.RemoveAt(Convert.ToInt32(button.Tag));
            PopulateEncounters();
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            txtPost.Text = string.Empty;
        }

        #endregion

        private void PopulateMedications()
        {
            flpMedications.Controls.Clear();
            var count = 0;

            foreach (MedicationItem med in _medications)
            {
                Button btn = new Button() { Text = String.Format("{0} ({1})", med.PatientMedicationGuid, med.Medication), AutoSize = true, Tag = count };
                btn.Click += new EventHandler(medication_Click);
                flpMedications.Controls.Add(btn);

                count += 1;
            }
        }

        private void PopulateEvaluations()
        {
            flpEvaluations.Controls.Clear();
            var count = 0;

            foreach (EvaluationItem eva in _evaluations)
            {
                Button btn = new Button() { Text = String.Format("{0} ({1})", eva.PatientEvaluationGuid, eva.Test), AutoSize = true, Tag = count };
                btn.Click += new EventHandler(evaluation_Click);
                flpEvaluations.Controls.Add(btn);

                count += 1;
            }
        }

        private void frmMain_Load(object sender, EventArgs e)
        {
            cboEndPoint.SelectedIndex = 0;
        }

        private void PopulateConditions()
        {
            flpConditions.Controls.Clear();
            var count = 0;

            foreach (ConditionItem con in _conditions)
            {
                Button btn = new Button() { Text = String.Format("{0} ({1})", con.PatientConditionGuid, con.Terminology), AutoSize = true, Tag = count };
                btn.Click += new EventHandler(condition_Click);
                flpConditions.Controls.Add(btn);

                count += 1;
            }
        }

        private void PopulateEvents()
        {
            flpEvents.Controls.Clear();
            var count = 0;

            foreach (ClinicalEventItem cli in _events)
            {
                Button btn = new Button() { Text = String.Format("{0} ({1})", cli.PatientClinicalEventGuid, cli.Description), AutoSize = true, Tag = count };
                btn.Click += new EventHandler(event_Click);
                flpEvents.Controls.Add(btn);

                count += 1;
            }
        }

        private void PopulateEncounters()
        {
            flpEncounters.Controls.Clear();
            var count = 0;

            foreach (EncounterItem enc in _encounters)
            {
                Button btn = new Button() { Text = String.Format("{0} ({1})", enc.EncounterGuid, enc.DateEncounter.ToString("yyyy-MM-dd")), AutoSize = true, Tag = count };
                btn.Click += new EventHandler(encounter_Click);
                flpEncounters.Controls.Add(btn);

                count += 1;
            }
        }

    }

    public class MedicationItem
    {
        public string PatientMedicationGuid { get; set; }
        public string Medication { get; set; }
        public string DoseUnit { get; set; }
        public string Frequency { get; set; }
        public DateTime? DateStart { get; set; }
        public DateTime? DateEnd { get; set; }
        public string Dose { get; set; }

        public string Route { get; set; }
        public string DaysWeek { get; set; }
        public string StillOnMedication { get; set; }
        public string Indication { get; set; }
        public string TypeofIndication { get; set; }
        public string ReasonForStopping { get; set; }
        public string Action { get; set; }
        public string BatchNumber { get; set; }
        public string Effect { get; set; }
    }

    public class EvaluationItem
    {
        public string PatientEvaluationGuid { get; set; }
        public string Test { get; set; }
        public string TestUnit { get; set; }
        public DateTime? DateTest { get; set; }
        public string TestResultValue { get; set; }
        public string TestResultCoded { get; set; }

        public string Remarks { get; set; }
    }

    public class ConditionItem
    {
        public string PatientConditionGuid { get; set; }
        public string Terminology { get; set; }
        public DateTime? DateStart { get; set; }
        public DateTime? DateEnd { get; set; }
        public string ConditionOutcome { get; set; }
        public string TreatmentOutcome { get; set; }

        public string ConditionOngoing { get; set; }
    }

    public class ClinicalEventItem
    {
        public string PatientClinicalEventGuid { get; set; }
        public string Description { get; set; }
        public string Terminology { get; set; }
        public DateTime? DateOnset { get; set; }
        public DateTime? DateResolution { get; set; }

        public string Outcome { get; set; }
        public string Intensity { get; set; }
        public string Scale { get; set; }
        public string Grade { get; set; }
        public string IsSerious { get; set; }
        public string Seriousness { get; set; }
        public DateTime? DateAdmission { get; set; }
        public DateTime? DateDischarge { get; set; }
        public DateTime? DateDeath { get; set; }
        public string Autopsy { get; set; }
        public string Attributed { get; set; }
        public string Reported { get; set; }
        public string SAENumber { get; set; }
        public string FullNameReporter { get; set; }
        public DateTime? DateReport { get; set; }
        public string TypeReporter { get; set; }
        public string ContactNumberReporter { get; set; }
        public string Expected { get; set; }
        public string TreatmentReaction { get; set; }
    }

    public class EncounterItem
    {
        public string EncounterGuid { get; set; }
        public string Type { get; set; }
        public DateTime DateEncounter { get; set; }

        public string Weight { get; set; }
        public string Height { get; set; }
        public string PreviousTBTx { get; set; }
        public string SiteTB { get; set; }
        public string PregnancyStatus { get; set; }
        public DateTime? DateLMP { get; set; }
        public string Gestation { get; set; }
        public string Breastfeeding { get; set; }
        public string Drug { get; set; }
        public string Alcahol { get; set; }
        public string Tobacco { get; set; }
        public string Documented { get; set; }
        public string XRay { get; set; }
        public string FirstLine { get; set; }
        public string SecondLine { get; set; }
        public string IsoniazidSusc { get; set; }
        public string IsoniazidConf { get; set; }
        public string RifampSusc { get; set; }
        public string RifampConf { get; set; }
    }
}

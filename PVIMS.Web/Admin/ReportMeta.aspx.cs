using System;
using System.Collections.Generic;

using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;

using System.Linq;
using System.Reflection;
using System.Text;

using System.Web.UI;
using System.Web.UI.WebControls;

using PVIMS.Core.Entities;
using PVIMS.Core.Services;

namespace PVIMS.Web
{
    public partial class ReportMeta : MainPageBase
    {
        private string _summary = "<ul>";

        List<String> _entities = new List<String>() { "Patient", "PatientClinicalEvent", "PatientCondition", "PatientFacility", "PatientLabTest", "PatientMedication", "Encounter", "CohortGroupEnrolment" };

        public IInfrastructureService _infrastructureService { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            Master.SetMenuActive("AdminReport");

            var config = _infrastructureService.GetOrCreateConfig(ConfigType.MetaDataLastUpdated);
            Master.SetPageHeader(new Models.PageHeaderDetail() { Title = "Report Meta Data", SubTitle = "", Icon = "fa fa-windows fa-fw", MetaDataLastUpdated = config.ConfigValue });

            if (!Page.IsPostBack)
            {
                RenderSummary();
                RenderItems();
            }
            
        }

        protected void btnRefresh_Click(object sender, EventArgs e)
        {
            bool err = false;

            //try
            //{
                // Ensure all meta definitions exist
                CheckEntitiesExist();
                CheckDependenciesExist();
                CheckColumnsExist();

                // Ensure all meta tables prepared
                DropMetaTables();
                CreateMetaTables();

                // Populate meta tables
                PopulateMetaTables();
                UpdateCustomAttributes();
                CreateMetaDependencies();

                // Rerender page
                RenderSummary();
                RenderItems();

                _infrastructureService.SetConfigValue(ConfigType.MetaDataLastUpdated, DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
            //}
            //catch (Exception ex)
           // {
            //    _summary += String.Format("<li>ERROR: {0}...</li>", ex.Message);
            //    err = true;
           // }

            _summary += "</ul>";
            spnSummary.InnerHtml = _summary;

        }

        #region "Rendering"

        private void RenderSummary()
        {
            string sql = string.Format(@"
                SELECT 'TableDefinition' AS Element, COUNT(*) AS Value FROM MetaTable 
                UNION
                SELECT 'DependencyDefinition' AS Element, COUNT(*) AS Value FROM MetaDependency 
                UNION
                SELECT 'ColumnDefinition' AS Element, COUNT(*) AS Value FROM MetaColumn 
                UNION
                SELECT 'PatientRowCount', ISNULL(SUM (row_count), 0) FROM sys.dm_db_partition_stats WHERE object_id=OBJECT_ID('MetaPatient') AND (index_id=0 or index_id=1);
                ");

            SqlParameter[] parameters = new SqlParameter[0];
            var results = UnitOfWork.Repository<MetaElementSetList>().ExecuteSql(sql, parameters);

            if (results.Count > 0)
            {
                foreach (MetaElementSetList item in results)
                {
                    switch (item.Element)
                    {
                        case "ColumnDefinition":
                            tdMetaColumnsDefined.InnerText = String.Format("{0} column(s) defined", item.Value.ToString());
                            break;
                        case "DependencyDefinition":
                            tdMetaDependencyDefined.InnerText = String.Format("{0} dependencies defined", item.Value.ToString());
                            break;
                        case "TableDefinition":
                            tdMetaTableDefined.InnerText = String.Format("{0} table(s) defined", item.Value.ToString());
                            break;
                        case "PatientRowCount":
                            tdPatientRecords.InnerText = String.Format("{0} row(s)", item.Value.ToString());
                            break;
                        default:
                            break;
                    }
                }
            }
            else
            {
                tdMetaTableDefined.InnerText = "No tables defined";
                tdMetaDependencyDefined.InnerText = "No tables defined";
                tdMetaColumnsDefined.InnerText = "No tables defined";
            }

        }

        private void RenderItems()
        {
            TableRow row;
            TableCell cell;

            // Loop through and render tables
            foreach (var mt in UnitOfWork.Repository<MetaTable>().Queryable().OrderBy(mt => mt.TableName))
            {
                row = new TableRow();

                cell = new TableCell();
                cell.Text = mt.TableName;
                row.Cells.Add(cell);

                cell = new TableCell();
                cell.Text = mt.FriendlyName;
                row.Cells.Add(cell);

                cell = new TableCell();
                cell.Text = mt.FriendlyDescription;
                row.Cells.Add(cell);

                cell = new TableCell();
                cell.Text = mt.TableType.Description;
                row.Cells.Add(cell);

                dt_1.Rows.Add(row);
            }

            foreach (var mc in UnitOfWork.Repository<MetaColumn>().Queryable().OrderBy(mc => mc.ColumnName))
            {
                row = new TableRow();

                cell = new TableCell();
                cell.Text = mc.Table.TableName;
                row.Cells.Add(cell);

                cell = new TableCell();
                cell.Text = mc.ColumnName;
                row.Cells.Add(cell);

                cell = new TableCell();
                cell.Text = mc.IsIdentity.ToString();
                row.Cells.Add(cell);

                cell = new TableCell();
                cell.Text = mc.IsNullable.ToString();
                row.Cells.Add(cell);

                cell = new TableCell();
                cell.Text = mc.ColumnType.Description;
                row.Cells.Add(cell);

                dt_basic_2.Rows.Add(row);
            }

            foreach (var md in UnitOfWork.Repository<MetaDependency>().Queryable().OrderBy(md => md.ParentColumnName))
            {
                row = new TableRow();

                cell = new TableCell();
                cell.Text = md.ParentTable.TableName;
                row.Cells.Add(cell);

                cell = new TableCell();
                cell.Text = md.ParentColumnName;
                row.Cells.Add(cell);

                cell = new TableCell();
                cell.Text = md.ReferenceTable.TableName;
                row.Cells.Add(cell);

                cell = new TableCell();
                cell.Text = md.ReferenceColumnName;
                row.Cells.Add(cell);

                dt_basic_3.Rows.Add(row);
            }
        }

        #endregion

        #region "Prepare Meta Definitions"

        private void CheckEntitiesExist()
        {
            foreach (String entity in _entities)
            {
                var metaTable = UnitOfWork.Repository<MetaTable>().Queryable().SingleOrDefault(mt => mt.TableName == entity);
                if(metaTable == null)
                {
                    switch (entity)
	                {
                        case "Patient":
                            metaTable = new MetaTable() 
                            {
                                FriendlyDescription = "Core patient table",
                                FriendlyName = "Patient",
                                metatable_guid = Guid.NewGuid(),
                                TableName = entity,
                                TableType = UnitOfWork.Repository<MetaTableType>().Queryable().SingleOrDefault(mtt => mtt.Description == "Core")
                            };
                            break;

                        case "PatientClinicalEvent":
                            metaTable = new MetaTable() 
                            {
                                FriendlyDescription = "Patient adverse event history",
                                FriendlyName = "Patient Adverse Events",
                                metatable_guid = Guid.NewGuid(),
                                TableName = entity,
                                TableType = UnitOfWork.Repository<MetaTableType>().Queryable().SingleOrDefault(mtt => mtt.Description == "CoreChild")
                            };
                            break;

                        case "PatientCondition":
                            metaTable = new MetaTable() 
                            {
                                FriendlyDescription = "Patient condition history",
                                FriendlyName = "Patient Conditions",
                                metatable_guid = Guid.NewGuid(),
                                TableName = entity,
                                TableType = UnitOfWork.Repository<MetaTableType>().Queryable().SingleOrDefault(mtt => mtt.Description == "CoreChild")
                            };
                            break;

                        case "PatientFacility":
                            metaTable = new MetaTable() 
                            {
                                FriendlyDescription = "Current facility",
                                FriendlyName = "Current Facility",
                                metatable_guid = Guid.NewGuid(),
                                TableName = entity,
                                TableType = UnitOfWork.Repository<MetaTableType>().Queryable().SingleOrDefault(mtt => mtt.Description == "History")
                            };
                            break;

                        case "PatientLabTest":
                            metaTable = new MetaTable() 
                            {
                                FriendlyDescription = "Patient evaluation history",
                                FriendlyName = "Patien Lab Tests",
                                metatable_guid = Guid.NewGuid(),
                                TableName = entity,
                                TableType = UnitOfWork.Repository<MetaTableType>().Queryable().SingleOrDefault(mtt => mtt.Description == "CoreChild")
                            };
                            break;

                        case "PatientMedication":
                            metaTable = new MetaTable() 
                            {
                                FriendlyDescription = "Patient medication history",
                                FriendlyName = "Patient Medications",
                                metatable_guid = Guid.NewGuid(),
                                TableName = entity,
                                TableType = UnitOfWork.Repository<MetaTableType>().Queryable().SingleOrDefault(mtt => mtt.Description == "CoreChild")
                            };
                            break;
                        
                        case "Encounter":
                            metaTable = new MetaTable() 
                            {
                                FriendlyDescription = "Patient encounter history",
                                FriendlyName = "Patient Encounters",
                                metatable_guid = Guid.NewGuid(),
                                TableName = entity,
                                TableType = UnitOfWork.Repository<MetaTableType>().Queryable().SingleOrDefault(mtt => mtt.Description == "Child")
                            };
                            break;

                        case "CohortGroupEnrolment":
                            metaTable = new MetaTable()
                            {
                                FriendlyDescription = "Patient cohort enrolments",
                                FriendlyName = "Cohort Enrolment",
                                metatable_guid = Guid.NewGuid(),
                                TableName = entity,
                                TableType = UnitOfWork.Repository<MetaTableType>().Queryable().SingleOrDefault(mtt => mtt.Description == "CoreChild")
                            };
                            break;

                        default:
                            break;
	                }
                    UnitOfWork.Repository<MetaTable>().Save(metaTable);
                }
            }

            _summary += String.Format("<li>INFO: All entities checked and verified...</li>");
        }

        private void CheckDependenciesExist()
        {
            var parentTable = UnitOfWork.Repository<MetaTable>().Queryable().SingleOrDefault(mt => mt.TableName == "Patient");

            // Dependency Patient --> PatientClinicalEvent
            var referenceTable = UnitOfWork.Repository<MetaTable>().Queryable().SingleOrDefault(mt => mt.TableName == "PatientClinicalEvent");
            var metaDependency = UnitOfWork.Repository<MetaDependency>().Queryable().SingleOrDefault(md => md.ParentTable.metatable_guid == parentTable.metatable_guid && md.ReferenceTable.metatable_guid == referenceTable.metatable_guid);
            if (metaDependency == null)
            {
                metaDependency = new MetaDependency()
                {
                    metadependency_guid = Guid.NewGuid(),
                    ParentColumnName = "Id",
                    ParentTable = parentTable,
                    ReferenceColumnName = "Patient_Id",
                    ReferenceTable = referenceTable
                };
                UnitOfWork.Repository<MetaDependency>().Save(metaDependency);
            }

            // Dependency Patient --> PatientCondition
            referenceTable = UnitOfWork.Repository<MetaTable>().Queryable().SingleOrDefault(mt => mt.TableName == "PatientCondition");
            metaDependency = UnitOfWork.Repository<MetaDependency>().Queryable().SingleOrDefault(md => md.ParentTable.metatable_guid == parentTable.metatable_guid && md.ReferenceTable.metatable_guid == referenceTable.metatable_guid);
            if (metaDependency == null)
            {
                metaDependency = new MetaDependency()
                {
                    metadependency_guid = Guid.NewGuid(),
                    ParentColumnName = "Id",
                    ParentTable = parentTable,
                    ReferenceColumnName = "Patient_Id",
                    ReferenceTable = referenceTable
                };
                UnitOfWork.Repository<MetaDependency>().Save(metaDependency);
            }

            // Dependency Patient --> PatientFacility
            referenceTable = UnitOfWork.Repository<MetaTable>().Queryable().SingleOrDefault(mt => mt.TableName == "PatientFacility");
            metaDependency = UnitOfWork.Repository<MetaDependency>().Queryable().SingleOrDefault(md => md.ParentTable.metatable_guid == parentTable.metatable_guid && md.ReferenceTable.metatable_guid == referenceTable.metatable_guid);
            if (metaDependency == null)
            {
                metaDependency = new MetaDependency()
                {
                    metadependency_guid = Guid.NewGuid(),
                    ParentColumnName = "Id",
                    ParentTable = parentTable,
                    ReferenceColumnName = "Patient_Id",
                    ReferenceTable = referenceTable
                };
                UnitOfWork.Repository<MetaDependency>().Save(metaDependency);
            }

            // Dependency Patient --> PatientLabTest
            referenceTable = UnitOfWork.Repository<MetaTable>().Queryable().SingleOrDefault(mt => mt.TableName == "PatientLabTest");
            metaDependency = UnitOfWork.Repository<MetaDependency>().Queryable().SingleOrDefault(md => md.ParentTable.metatable_guid == parentTable.metatable_guid && md.ReferenceTable.metatable_guid == referenceTable.metatable_guid);
            if (metaDependency == null)
            {
                metaDependency = new MetaDependency()
                {
                    metadependency_guid = Guid.NewGuid(),
                    ParentColumnName = "Id",
                    ParentTable = parentTable,
                    ReferenceColumnName = "Patient_Id",
                    ReferenceTable = referenceTable
                };
                UnitOfWork.Repository<MetaDependency>().Save(metaDependency);
            }

            // Dependency Patient --> PatientMedication
            referenceTable = UnitOfWork.Repository<MetaTable>().Queryable().SingleOrDefault(mt => mt.TableName == "PatientMedication");
            metaDependency = UnitOfWork.Repository<MetaDependency>().Queryable().SingleOrDefault(md => md.ParentTable.metatable_guid == parentTable.metatable_guid && md.ReferenceTable.metatable_guid == referenceTable.metatable_guid);
            if (metaDependency == null)
            {
                metaDependency = new MetaDependency()
                {
                    metadependency_guid = Guid.NewGuid(),
                    ParentColumnName = "Id",
                    ParentTable = parentTable,
                    ReferenceColumnName = "Patient_Id",
                    ReferenceTable = referenceTable
                };
                UnitOfWork.Repository<MetaDependency>().Save(metaDependency);
            }

            // Dependency Patient --> Encounter
            referenceTable = UnitOfWork.Repository<MetaTable>().Queryable().SingleOrDefault(mt => mt.TableName == "Encounter");
            metaDependency = UnitOfWork.Repository<MetaDependency>().Queryable().SingleOrDefault(md => md.ParentTable.metatable_guid == parentTable.metatable_guid && md.ReferenceTable.metatable_guid == referenceTable.metatable_guid);
            if (metaDependency == null)
            {
                metaDependency = new MetaDependency()
                {
                    metadependency_guid = Guid.NewGuid(),
                    ParentColumnName = "Id",
                    ParentTable = parentTable,
                    ReferenceColumnName = "Patient_Id",
                    ReferenceTable = referenceTable
                };
                UnitOfWork.Repository<MetaDependency>().Save(metaDependency);
            }

            // Dependency Patient --> CohortGroupEnrolment
            referenceTable = UnitOfWork.Repository<MetaTable>().Queryable().SingleOrDefault(mt => mt.TableName == "CohortGroupEnrolment");
            metaDependency = UnitOfWork.Repository<MetaDependency>().Queryable().SingleOrDefault(md => md.ParentTable.metatable_guid == parentTable.metatable_guid && md.ReferenceTable.metatable_guid == referenceTable.metatable_guid);
            if (metaDependency == null)
            {
                metaDependency = new MetaDependency()
                {
                    metadependency_guid = Guid.NewGuid(),
                    ParentColumnName = "Id",
                    ParentTable = parentTable,
                    ReferenceColumnName = "Patient_Id",
                    ReferenceTable = referenceTable
                };
                UnitOfWork.Repository<MetaDependency>().Save(metaDependency);
            }

            _summary += String.Format("<li>INFO: All dependencies checked and verified...</li>");
        }

        private void CheckColumnsExist()
        {
            Patient patient = new Patient();
            ProcessEntity(patient, "Patient");
            patient = null;

            PatientMedication patientMedication = new PatientMedication();
            ProcessEntity(patientMedication, "PatientMedication");
            patientMedication = null;

            PatientClinicalEvent patientClinicalEvent = new PatientClinicalEvent();
            ProcessEntity(patientClinicalEvent, "PatientClinicalEvent");
            patientClinicalEvent = null;

            PatientCondition patientCondition = new PatientCondition();
            ProcessEntity(patientCondition, "PatientCondition");
            patientCondition = null;

            PatientLabTest patientLabTest = new PatientLabTest();
            ProcessEntity(patientLabTest, "PatientLabTest");
            patientLabTest = null;

            Encounter encounter = new Encounter(patient);
            ProcessEntity(encounter, "Encounter");
            encounter = null;

            CohortGroupEnrolment cohortGroupEnrolment = new CohortGroupEnrolment();
            ProcessEntity(cohortGroupEnrolment, "CohortGroupEnrolment");
            cohortGroupEnrolment = null;

            PatientFacility patientFacility = new PatientFacility();
            ProcessEntity(patientFacility, "PatientFacility");
            patientFacility = null;

            _summary += String.Format("<li>INFO: All columns checked and verified...</li>");
        }

        private void ProcessEntity(Object obj, string entityName)
        {
            Type type = obj.GetType();
            PropertyInfo[] properties = type.GetProperties();
            var invalidProperties = new[] { "CustomAttributesXmlSerialised", "Archived", "ArchivedReason", "ArchivedDate", "AuditUser", "Age", "FullName", "DisplayName" };

            var metaTable = UnitOfWork.Repository<MetaTable>().Queryable().Single(mt => mt.TableName == entityName);
            var attributes = UnitOfWork.Repository<CustomAttributeConfiguration>().Queryable().Where(c => c.ExtendableTypeName == entityName).OrderBy(c => c.Id).ToList();

            List<DatasetCategoryElement> elements = null;
            if (entityName == "Encounter") {
                elements = UnitOfWork.Repository<DatasetCategoryElement>().Queryable().Include("DatasetCategory.Dataset").Include("DatasetElement").Where(dce => dce.DatasetCategory.Dataset.DatasetName == "Chronic Treatment").OrderBy(dce => dce.DatasetCategory.CategoryOrder).ThenBy(dce => dce.FieldOrder).ToList();
            }

            MetaColumn metaColumn;
            MetaColumnType metaColumnType;
            String range = "";

            foreach (PropertyInfo property in properties)
            {
                if (!invalidProperties.Contains(property.Name))
                {
                    var columnName = property.Name;
                    if (property.PropertyType == typeof(Patient) || property.PropertyType == typeof(Encounter)) {
                        columnName = property.Name + "_Id";
                    };

                    metaColumn = UnitOfWork.Repository<MetaColumn>().Queryable().SingleOrDefault(mc => mc.Table.TableName == entityName && mc.ColumnName == columnName);
                    if (metaColumn == null)
                    {
                        metaColumnType = null;
                        range = "";

                        if (property.PropertyType == typeof(DateTime?) || property.PropertyType == typeof(DateTime)) { metaColumnType = UnitOfWork.Repository<MetaColumnType>().Queryable().SingleOrDefault(mct => mct.Description == "datetime"); };
                        if (property.PropertyType == typeof(string)) { metaColumnType = UnitOfWork.Repository<MetaColumnType>().Queryable().SingleOrDefault(mct => mct.Description == "varchar"); };
                        if (property.PropertyType == typeof(int)) { metaColumnType = UnitOfWork.Repository<MetaColumnType>().Queryable().SingleOrDefault(mct => mct.Description == "int"); };
                        if (property.PropertyType == typeof(decimal)) { metaColumnType = UnitOfWork.Repository<MetaColumnType>().Queryable().SingleOrDefault(mct => mct.Description == "decimal"); };
                        if (property.PropertyType == typeof(Guid)) { metaColumnType = UnitOfWork.Repository<MetaColumnType>().Queryable().SingleOrDefault(mct => mct.Description == "uniqueidentifier"); };
                        if (property.PropertyType == typeof(bool)) { metaColumnType = UnitOfWork.Repository<MetaColumnType>().Queryable().SingleOrDefault(mct => mct.Description == "bit"); };
                        if (property.PropertyType == typeof(Patient) || property.PropertyType == typeof(Encounter)) { metaColumnType = UnitOfWork.Repository<MetaColumnType>().Queryable().SingleOrDefault(mct => mct.Description == "int"); };

                        if (property.PropertyType == typeof(EncounterType))
                        {
                            metaColumnType = UnitOfWork.Repository<MetaColumnType>().Queryable().SingleOrDefault(mct => mct.Description == "varchar");
                            range = "SOURCE:EncounterType.Description";
                        }
                        if (property.PropertyType == typeof(Facility))
                        {
                            metaColumnType = UnitOfWork.Repository<MetaColumnType>().Queryable().SingleOrDefault(mct => mct.Description == "varchar");
                            range = "SOURCE:Facility.FacilityName";
                        }
                        if (property.PropertyType == typeof(CohortGroup))
                        {
                            metaColumnType = UnitOfWork.Repository<MetaColumnType>().Queryable().SingleOrDefault(mct => mct.Description == "varchar");
                            range = "SOURCE:CohortGroup.CohortName";
                        }
                        if (property.PropertyType == typeof(LabTestUnit))
                        {
                            metaColumnType = UnitOfWork.Repository<MetaColumnType>().Queryable().SingleOrDefault(mct => mct.Description == "varchar");
                            range = "SOURCE:LabTestUnit.Description";
                        }
                        if (property.PropertyType == typeof(LabTest))
                        {
                            metaColumnType = UnitOfWork.Repository<MetaColumnType>().Queryable().SingleOrDefault(mct => mct.Description == "varchar");
                            range = "SOURCE:LabTest.Description";
                        }
                        if (property.PropertyType == typeof(Outcome))
                        {
                            metaColumnType = UnitOfWork.Repository<MetaColumnType>().Queryable().SingleOrDefault(mct => mct.Description == "varchar");
                            range = "SOURCE:Outcome.Description";
                        }
                        if (property.PropertyType == typeof(TerminologyMedDra))
                        {
                            metaColumnType = UnitOfWork.Repository<MetaColumnType>().Queryable().SingleOrDefault(mct => mct.Description == "varchar");
                        }
                        if (property.PropertyType == typeof(Medication))
                        {
                            metaColumnType = UnitOfWork.Repository<MetaColumnType>().Queryable().SingleOrDefault(mct => mct.Description == "varchar");
                            range = "SOURCE:Medication.DrugName";
                        }
                        if (property.PropertyType == typeof(User))
                        {
                            metaColumnType = UnitOfWork.Repository<MetaColumnType>().Queryable().SingleOrDefault(mct => mct.Description == "varchar");
                            range = "SOURCE:User.UserName";
                        }

                        if (metaColumnType != null)
                        {
                            metaColumn = new MetaColumn()
                            {
                                ColumnName = columnName,
                                ColumnType = metaColumnType,
                                IsIdentity = (property.Name == "Id"),
                                //IsNullable = Nullable.GetUnderlyingType(property.PropertyType) != null,
                                IsNullable = property.Name == "Id" ? false : true,
                                metacolumn_guid = Guid.NewGuid(),
                                Table = metaTable,
                                Range = range
                            };
                            UnitOfWork.Repository<MetaColumn>().Save(metaColumn);
                        }
                    }
                }
            }

            // Now process attributes
            foreach (CustomAttributeConfiguration attribute in attributes)
            {
                metaColumn = UnitOfWork.Repository<MetaColumn>().Queryable().SingleOrDefault(mc => mc.Table.TableName == entityName && mc.ColumnName == attribute.AttributeKey.Replace(" ", "").Replace("(", "").Replace(")", "").Replace("&", ""));
                if (metaColumn == null)
                {
                    metaColumnType = null;
                    range = "";

                    if (attribute.CustomAttributeType == VPS.CustomAttributes.CustomAttributeType.DateTime) { metaColumnType = UnitOfWork.Repository<MetaColumnType>().Queryable().SingleOrDefault(mct => mct.Description == "datetime"); };
                    if (attribute.CustomAttributeType == VPS.CustomAttributes.CustomAttributeType.Numeric) { metaColumnType = UnitOfWork.Repository<MetaColumnType>().Queryable().SingleOrDefault(mct => mct.Description == "int"); };
                    if (attribute.CustomAttributeType == VPS.CustomAttributes.CustomAttributeType.String) { metaColumnType = UnitOfWork.Repository<MetaColumnType>().Queryable().SingleOrDefault(mct => mct.Description == "varchar"); };
                    if (attribute.CustomAttributeType == VPS.CustomAttributes.CustomAttributeType.Selection) 
                    { 
                        metaColumnType = UnitOfWork.Repository<MetaColumnType>().Queryable().SingleOrDefault(mct => mct.Description == "varchar");
                        var selectionItems = UnitOfWork.Repository<SelectionDataItem>().Queryable().Where(sd => sd.AttributeKey == attribute.AttributeKey).Select(s => s.Value).ToList();
                        range = string.Join(",", selectionItems);
                    };

                    if (metaColumnType != null)
                    {
                        metaColumn = new MetaColumn()
                        {
                            ColumnName = attribute.AttributeKey.Replace(" ", "").Replace("(", "").Replace(")", "").Replace("&", "").Trim().Length > 100 ? attribute.AttributeKey.Replace(" ", "").Replace("(", "").Replace(")", "").Replace("&", "").Trim().Substring(0, 100) : attribute.AttributeKey.Replace(" ", "").Replace("(", "").Replace(")", "").Replace("&", "").Trim(),
                            ColumnType = metaColumnType,
                            IsIdentity = false,
                            IsNullable = true,
                            metacolumn_guid = Guid.NewGuid(),
                            Table = metaTable,
                            Range = range
                        };
                        UnitOfWork.Repository<MetaColumn>().Save(metaColumn);
                    }
                }
            }

            // Process instance headers
            if (elements != null)
            {
                foreach (DatasetCategoryElement dce in elements)
                {
                    metaColumn = UnitOfWork.Repository<MetaColumn>().Queryable().SingleOrDefault(mc => mc.Table.TableName == entityName && mc.ColumnName == dce.DatasetElement.ElementName.Replace(" ", "").Replace("(", "").Replace(")", "").Replace("&", ""));
                    if (metaColumn == null)
                    {
                        metaColumnType = null;
                        range = "";

                        if (dce.DatasetElement.Field.FieldType.Description == "Date") { metaColumnType = UnitOfWork.Repository<MetaColumnType>().Queryable().SingleOrDefault(mct => mct.Description == "date"); };
                        if (dce.DatasetElement.Field.FieldType.Description == "AlphaNumericTextbox") { metaColumnType = UnitOfWork.Repository<MetaColumnType>().Queryable().SingleOrDefault(mct => mct.Description == "varchar"); };
                        if (dce.DatasetElement.Field.FieldType.Description == "NumericTextbox" && dce.DatasetElement.Field.Decimals == 0) { metaColumnType = UnitOfWork.Repository<MetaColumnType>().Queryable().SingleOrDefault(mct => mct.Description == "int"); };
                        if (dce.DatasetElement.Field.FieldType.Description == "NumericTextbox" && dce.DatasetElement.Field.Decimals > 0) { metaColumnType = UnitOfWork.Repository<MetaColumnType>().Queryable().SingleOrDefault(mct => mct.Description == "decimal"); };
                        if (dce.DatasetElement.Field.FieldType.Description == "DropDownList") 
                        { 
                            metaColumnType = UnitOfWork.Repository<MetaColumnType>().Queryable().SingleOrDefault(mct => mct.Description == "varchar");
                            var fieldValues = UnitOfWork.Repository<FieldValue>().Queryable().Where(fv => fv.Field.Id == dce.DatasetElement.Field.Id).Select(s => s.Value).ToList();
                            range = string.Join(",", fieldValues);
                        };
                        if (dce.DatasetElement.Field.FieldType.Description == "YesNo") 
                        { 
                            metaColumnType = UnitOfWork.Repository<MetaColumnType>().Queryable().SingleOrDefault(mct => mct.Description == "varchar");
                            range = "Yes, No";
                        };

                        if (metaColumnType != null)
                        {
                            metaColumn = new MetaColumn()
                            {
                                ColumnName = dce.DatasetElement.ElementName.Replace(" ", "").Replace("(", "").Replace(")", "").Replace("&", ""),
                                ColumnType = metaColumnType,
                                IsIdentity = false,
                                IsNullable = true,
                                metacolumn_guid = Guid.NewGuid(),
                                Table = metaTable,
                                Range = range
                            };
                            UnitOfWork.Repository<MetaColumn>().Save(metaColumn);
                        }
                    }
                }
            }
        }

        #endregion

        #region "Handle Meta Tables"

        private void DropMetaTables()
        {
            StringBuilder sb;

            var metaTables = UnitOfWork.Repository<MetaTable>().Queryable().OrderByDescending(mt => mt.Id).ToList();
            foreach (MetaTable metaTable in metaTables)
            {
                sb = new StringBuilder();

                sb.AppendFormat("IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Meta{0}]') AND type in (N'U')) DROP TABLE [Meta{0}]", metaTable.TableName);

                UnitOfWork.Repository<Patient>().ExecuteSqlCommand(sb.ToString());
            }

            _summary += String.Format("<li>INFO: Meta tables dropped...</li>");
        }

        private void CreateMetaTables()
        {
            StringBuilder sb;
            var valid = new [] { "varchar", "nvarchar" };

            var metaTables = UnitOfWork.Repository<MetaTable>().Queryable().OrderBy(mt => mt.TableName).ToList();
            foreach(MetaTable metaTable in metaTables)
            {

                sb = new StringBuilder();

                sb.AppendFormat("CREATE TABLE [Meta{0}] (", metaTable.TableName);

                // Add each column
                foreach(MetaColumn metaColumn in metaTable.Columns)
                {
                    sb.AppendFormat("[{0}] [{1}] {2} {3},", metaColumn.ColumnName, metaColumn.ColumnType.Description, valid.Contains(metaColumn.ColumnType.Description) ? "(max)" : "", metaColumn.IsNullable ? "NULL" : "NOT NULL");
                }

                // Create constraint
                sb.AppendFormat("CONSTRAINT [{0}_PK] PRIMARY KEY CLUSTERED  ([Id] ASC))", metaTable.TableName);

                UnitOfWork.Repository<Patient>().ExecuteSqlCommand(sb.ToString());
            }

            _summary += String.Format("<li>INFO: Meta tables created...</li>");
        }

        private void CreateMetaDependencies()
        {
            StringBuilder sb;

            var metaDependencies = UnitOfWork.Repository<MetaDependency>().Queryable().OrderBy(md => md.ParentTable.Id).ToList();
            foreach (MetaDependency metaDependency in metaDependencies)
            {
                sb = new StringBuilder();

                sb.AppendFormat("ALTER TABLE [Meta{0}] WITH NOCHECK ADD  CONSTRAINT [FK_Meta{0}_Meta{1}] FOREIGN KEY([{2}]) REFERENCES [Meta{1}] ([{3}])", metaDependency.ReferenceTable.TableName, metaDependency.ParentTable.TableName, metaDependency.ReferenceColumnName, metaDependency.ParentColumnName);

                UnitOfWork.Repository<Patient>().ExecuteSqlCommand(sb.ToString());
            }

            _summary += String.Format("<li>INFO: Meta dependencies created...</li>");
        }

        private void PopulateMetaTables()
        {
            Patient patient = new Patient();
            ProcessInsertEntity(patient, "Patient");
            patient = null;

            PatientMedication patientMedication = new PatientMedication();
            ProcessInsertEntity(patientMedication, "PatientMedication");
            patientMedication = null;

            PatientClinicalEvent patientClinicalEvent = new PatientClinicalEvent();
            ProcessInsertEntity(patientClinicalEvent, "PatientClinicalEvent");
            patientClinicalEvent = null;

            PatientCondition patientCondition = new PatientCondition();
            ProcessInsertEntity(patientCondition, "PatientCondition");
            patientCondition = null;

            PatientLabTest patientLabTest = new PatientLabTest();
            ProcessInsertEntity(patientLabTest, "PatientLabTest");
            patientLabTest = null;

            Encounter encounter = new Encounter(patient);
            ProcessInsertEntity(encounter, "Encounter");
            encounter = null;

            CohortGroupEnrolment cohortGroupEnrolment = new CohortGroupEnrolment();
            ProcessInsertEntity(cohortGroupEnrolment, "CohortGroupEnrolment");
            cohortGroupEnrolment = null;

            PatientFacility patientFacility = new PatientFacility();
            ProcessInsertEntity(patientFacility, "PatientFacility");
            patientFacility = null;

            _summary += String.Format("<li>INFO: All meta data seeded...</li>");
        }

        private void UpdateCustomAttributes()
        {
            ProcessUpdateAttribute("Patient", "mp");
            ProcessUpdateAttribute("PatientMedication", "mpm");
            ProcessUpdateAttribute("PatientClinicalEvent", "mpce");
            ProcessUpdateAttribute("PatientCondition", "mpc");
            ProcessUpdateAttribute("PatientLabTest", "mplt");

            ProcessUpdateSelection("Patient", "mp");
            ProcessUpdateSelection("PatientMedication", "mpm");
            ProcessUpdateSelection("PatientClinicalEvent", "mpce");
            ProcessUpdateSelection("PatientCondition", "mpc");
            ProcessUpdateSelection("PatientLabTest", "mplt");

            _summary += String.Format("<li>INFO: All meta data attributes populated...</li>");
        }

        private void ProcessInsertEntity(Object obj, string entityName)
        {
            Type type = obj.GetType();
            PropertyInfo[] properties = type.GetProperties();
            var invalidProperties = new[] { "CustomAttributesXmlSerialised", "Archived", "ArchivedReason", "ArchivedDate", "AuditUser", "Age", "FullName", "AgeGroup", "DisplayName" };

            var metaTable = UnitOfWork.Repository<MetaTable>().Queryable().Single(mt => mt.TableName == entityName);

            StringBuilder sbMain = new StringBuilder(); ;
            sbMain.AppendFormat("INSERT INTO [Meta{0}] (", metaTable.TableName);
            StringBuilder sbJoins = new StringBuilder(); ;
            StringBuilder sbIntoFields = new StringBuilder(); ;
            StringBuilder sbSelectFields = new StringBuilder(); ;

            var userCount = 0;
            var terminologyCount = 0;

            foreach (PropertyInfo property in properties)
            {
                if (!invalidProperties.Contains(property.Name))
                {
                    var columnName = property.Name;
                    if (property.PropertyType == typeof(Patient) || property.PropertyType == typeof(Encounter)) {
                        columnName = property.Name + "_Id";
                    };

                    if (property.PropertyType == typeof(EncounterType))
                    {
                        sbIntoFields.AppendFormat("[{0}], ", columnName);
                        sbSelectFields.AppendFormat("et.[Description], ");
                        sbJoins.AppendFormat("LEFT JOIN [EncounterType] et ON tbl.{0}_Id = et.Id ", columnName);
                    }
                    if (property.PropertyType == typeof(Facility))
                    {
                        sbIntoFields.AppendFormat("[{0}], ", columnName);
                        sbSelectFields.AppendFormat("f.[FacilityName], ");
                        sbJoins.AppendFormat("LEFT JOIN [Facility] f ON tbl.{0}_Id = f.Id ", columnName);
                    }
                    if (property.PropertyType == typeof(CohortGroup))
                    {
                        sbIntoFields.AppendFormat("[{0}], ", columnName);
                        sbSelectFields.AppendFormat("cg.[CohortName], ");
                        sbJoins.AppendFormat("LEFT JOIN [CohortGroup] cg ON tbl.{0}_Id = cg.Id ", columnName);
                    }
                    if (property.PropertyType == typeof(LabTestUnit))
                    {
                        sbIntoFields.AppendFormat("[{0}], ", columnName);
                        sbSelectFields.AppendFormat("ltu.[Description], ");
                        sbJoins.AppendFormat("LEFT JOIN [LabTestUnit] ltu ON tbl.{0}_Id = ltu.Id ", columnName);
                    }
                    if (property.PropertyType == typeof(LabTest))
                    {
                        sbIntoFields.AppendFormat("[{0}], ", columnName);
                        sbSelectFields.AppendFormat("lt.[Description], ");
                        sbJoins.AppendFormat("LEFT JOIN [LabTest] lt ON tbl.{0}_Id = lt.Id ", columnName);
                    }
                    if (property.PropertyType == typeof(Outcome))
                    {
                        sbIntoFields.AppendFormat("[{0}], ", columnName);
                        sbSelectFields.AppendFormat("o.[Description], ");
                        sbJoins.AppendFormat("LEFT JOIN [Outcome] o ON tbl.{0}_Id = o.Id ", columnName);
                    }
                    if (property.PropertyType == typeof(TerminologyMedDra))
                    {
                        terminologyCount += 1;
                        sbIntoFields.AppendFormat("[{0}], ", columnName);
                        sbSelectFields.AppendFormat("tm{0}.MedDraTerm, ", terminologyCount.ToString());
                        sbJoins.AppendFormat("LEFT JOIN [TerminologyMedDra] tm{1} ON tbl.{0}_Id = tm{1}.Id ", columnName, terminologyCount.ToString());
                    }
                    if (property.PropertyType == typeof(Medication))
                    {
                        sbIntoFields.AppendFormat("[{0}], ", columnName);
                        sbSelectFields.AppendFormat("m.DrugName, ");
                        sbJoins.AppendFormat("LEFT JOIN [Medication] m ON tbl.{0}_Id = m.Id ", columnName);
                    }
                    if (property.PropertyType == typeof(User))
                    {
                        userCount += 1;
                        sbIntoFields.AppendFormat("[{0}], ", columnName);
                        sbSelectFields.AppendFormat("u{0}.UserName, ", userCount.ToString());
                        sbJoins.AppendFormat("LEFT JOIN [User] u{1} ON tbl.{0}_Id = u{1}.Id ", columnName, userCount.ToString());
                    }
                    if (property.PropertyType == typeof(Patient) || property.PropertyType == typeof(Encounter) || property.PropertyType == typeof(DateTime?) || property.PropertyType == typeof(DateTime) || property.PropertyType == typeof(string) || property.PropertyType == typeof(int) || property.PropertyType == typeof(decimal) || property.PropertyType == typeof(Guid) || property.PropertyType == typeof(bool))
                    {
                        sbIntoFields.AppendFormat("[{0}], ", columnName);
                        sbSelectFields.AppendFormat("tbl.[{0}], ", columnName);
                    };
                }
            }
            if(sbIntoFields.Length > 0) {
                sbIntoFields.Remove(sbIntoFields.Length - 2, 2);
            }
            if (sbSelectFields.Length > 0) {
                sbSelectFields.Remove(sbSelectFields.Length - 2, 2);
            }

            sbMain.AppendFormat("{0}) SELECT {1} FROM {2} tbl {3} WHERE Archived = 0", sbIntoFields.ToString(), sbSelectFields.ToString(), metaTable.TableName, sbJoins.ToString());
            UnitOfWork.Repository<Patient>().ExecuteSqlCommand(sbMain.ToString());
        }

        private void ProcessUpdateAttribute(string entityName, string alias)
        {
            StringBuilder sbMain = new StringBuilder();

            var attributes = UnitOfWork.Repository<CustomAttributeConfiguration>().Queryable()
                .Where(c => c.ExtendableTypeName == entityName)
                .OrderBy(c => c.Id).ToList();
            var attributeType = "";

            foreach (CustomAttributeConfiguration attribute in attributes)
            {
                sbMain.Clear();

                switch (attribute.CustomAttributeType)
                {
                    case VPS.CustomAttributes.CustomAttributeType.Numeric:
                    case VPS.CustomAttributes.CustomAttributeType.String:
                        attributeType = "CustomStringAttribute";
                        break;

                    case VPS.CustomAttributes.CustomAttributeType.Selection:
                    case VPS.CustomAttributes.CustomAttributeType.DateTime:
                        attributeType = "CustomSelectionAttribute";
                        break;
                }

                if(!String.IsNullOrWhiteSpace(attributeType))
                {
                    sbMain.AppendFormat(@"UPDATE {0} SET {0}.[{1}] = CustomAttributesXmlSerialised.value('(/CustomAttributeSet/{2}[Key=""{3}"" ]/Value)[1]', 'nvarchar(max)') from {4} as h inner join Meta{4} as {0} on h.Id = {0}.Id", alias, attribute.AttributeKey.Replace(" ", "").Replace("(", "").Replace(")", "").Replace("&", ""), attributeType, attribute.AttributeKey.Replace("&", "&amp;"), entityName);
                    UnitOfWork.Repository<Patient>().ExecuteSqlCommand(sbMain.ToString());
                }
            }
        }

        private void ProcessUpdateSelection(string entityName, string alias)
        {
            StringBuilder sbMain = new StringBuilder();

            var attributes = UnitOfWork.Repository<CustomAttributeConfiguration>().Queryable()
                .Where(c => c.ExtendableTypeName == entityName)
                .OrderBy(c => c.Id).ToList();

            foreach (CustomAttributeConfiguration attribute in attributes.Where(ca => ca.CustomAttributeType == VPS.CustomAttributes.CustomAttributeType.Selection))
            {
                sbMain.Clear();
                sbMain.AppendFormat(@"UPDATE {0} SET {0}.[{1}] = sdi.Value from Meta{2} as {0} inner join SelectionDataItem as sdi on sdi.AttributeKey = '{3}' and SelectionKey collate Latin1_General_CI_AS = {0}.[{1}] collate Latin1_General_CI_AS", alias, attribute.AttributeKey.Replace(" ", "").Replace("(", "").Replace(")", "").Replace("&", ""), entityName, attribute.AttributeKey);
                UnitOfWork.Repository<Patient>().ExecuteSqlCommand(sbMain.ToString());
            }
        }

        #endregion

    }
}
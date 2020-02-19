using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Infrastructure.Annotations;
using System.Data.Entity.Migrations;
using System.Data.Entity.Validation;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

using VPS.CustomAttributes;
using PVIMS.Core.Entities;
using VPS.EF;
using VPS.Common.Domain;
using PVIMS.Core;

using SelectionDataItem = PVIMS.Core.Entities.SelectionDataItem;
using CustomAttributeConfiguration = PVIMS.Core.Entities.CustomAttributeConfiguration;

namespace PVIMS.Entities.EF
{
    public class PVIMSDbContext : NonPluralDbContextBase
    {
        public PVIMSDbContext()
            : base("PVIMS")
        {
#if DEBUG
            this.Database.Log = message => Debug.WriteLine(message);
#endif
        }

        public virtual DbSet<Activity> Activities { get; set; }
        public virtual DbSet<ActivityExecutionStatus> ActivityExecutionStatuses { get; set; }
        public virtual DbSet<ActivityExecutionStatusEvent> ActivityExecutionStatusEvents { get; set; }
        public virtual DbSet<ActivityInstance> ActivityInstances { get; set; }
        public virtual DbSet<Appointment> Appointments { get; set; }
        public virtual DbSet<Attachment> Attachments { get; set; }
        public virtual DbSet<AttachmentType> AttachmentTypes { get; set; }
        public virtual DbSet<AuditLog> AuditLogs { get; set; }
        public virtual DbSet<CareEvent> CareEvents { get; set; }
        public virtual DbSet<CohortGroup> CohortGroups { get; set; }
        public virtual DbSet<CohortGroupEnrolment> CohortGroupEnrolments { get; set; }
        public virtual DbSet<Condition> Conditions { get; set; }
        public virtual DbSet<ConditionLabTest> ConditionLabTests { get; set; }
        public virtual DbSet<ConditionMedication> ConditionMedications { get; set; }
        public virtual DbSet<ConditionMedDra> ConditionMedDras { get; set; }
        public virtual DbSet<Config> Configs { get; set; }
        public virtual DbSet<ContextType> ContextTypes { get; set; }
        public virtual DbSet<CustomAttributeConfiguration> CustomAttributeConfigurations { get; set; }
        public virtual DbSet<Dataset> Datasets { get; set; }
        public virtual DbSet<DatasetCategory> DatasetCategories { get; set; }
        public virtual DbSet<DatasetCategoryCondition> DatasetCategoryConditions { get; set; }
        public virtual DbSet<DatasetCategoryElement> DatasetCategoryElements { get; set; }
        public virtual DbSet<DatasetCategoryElementCondition> DatasetCategoryElementConditions { get; set; }
        public virtual DbSet<DatasetElement> DatasetElements { get; set; }
        public virtual DbSet<DatasetElementSub> DatasetElementSubs { get; set; }
        public virtual DbSet<DatasetElementType> DatasetElementTypes { get; set; }
        public virtual DbSet<DatasetInstance> DatasetInstances { get; set; }
        public virtual DbSet<DatasetInstanceValue> DatasetInstanceValues { get; set; }
        public virtual DbSet<DatasetInstanceSubValue> DatasetInstanceSubValues { get; set; }
        public virtual DbSet<DatasetMapping> DatasetMappings { get; set; }
        public virtual DbSet<DatasetMappingValue> DatasetMappingValues { get; set; }
        public virtual DbSet<DatasetRule> DatasetRules { get; set; }
        public virtual DbSet<DatasetXml> DatasetXmls { get; set; }
        public virtual DbSet<DatasetXmlNode> DatasetXmlNodes { get; set; }
        public virtual DbSet<DatasetXmlAttribute> DatasetXmlAttributes { get; set; }
        public virtual DbSet<Encounter> Encounters { get; set; }
        public virtual DbSet<EncounterType> EncounterTypes { get; set; }
        public virtual DbSet<EncounterTypeWorkPlan> EncounterTypeWorkPlans { get; set; }
        public virtual DbSet<Facility> Facilities { get; set; }
        public virtual DbSet<FacilityType> FacilityTypes { get; set; }
        public virtual DbSet<Field> Fields { get; set; }
        public virtual DbSet<FieldType> FieldTypes { get; set; }
        public virtual DbSet<FieldValue> FieldValues { get; set; }
        public virtual DbSet<Holiday> Holidays { get; set; }
        public virtual DbSet<LabResult> LabResults { get; set; }
        public virtual DbSet<LabTest> LabTests { get; set; }
        public virtual DbSet<LabTestUnit> LabTestUnits { get; set; }
        public virtual DbSet<Language> Languages { get; set; }
        public virtual DbSet<Medication> Medications { get; set; }
        public virtual DbSet<MedicationForm> MedicationForms { get; set; }
        public virtual DbSet<MedDRAGrading> MedDRAGradings { get; set; }
        public virtual DbSet<MedDRAScale> MedDRAScales { get; set; }
        public virtual DbSet<MetaColumn> MetaColumns { get; set; }
        public virtual DbSet<MetaColumnType> MetaColumnTypes { get; set; }
        public virtual DbSet<MetaDependency> MetaDependencies { get; set; }
        public virtual DbSet<MetaPage> MetaPages { get; set; }
        public virtual DbSet<MetaReport> MetaReports { get; set; }
        public virtual DbSet<MetaTable> MetaTables { get; set; }
        public virtual DbSet<MetaTableType> MetaTableTypes { get; set; }
        public virtual DbSet<MetaWidget> MetaWidgets { get; set; }
        public virtual DbSet<MetaWidgetType> MetaWidgetTypes { get; set; }
        public virtual DbSet<OrgUnit> OrgUnits { get; set; }
        public virtual DbSet<OrgUnitType> OrgUnitTypes { get; set; }
        public virtual DbSet<Outcome> Outcomes { get; set; }
        public virtual DbSet<Patient> Patients { get; set; }
        public virtual DbSet<PatientClinicalEvent> PatientClinicalEvents { get; set; }
        public virtual DbSet<PatientCondition> PatientConditions { get; set; }
        public virtual DbSet<PatientFacility> PatientFacilities { get; set; }
        public virtual DbSet<PatientLabTest> PatientLabTests { get; set; }
        public virtual DbSet<PatientLanguage> PatientLanguages { get; set; }
        public virtual DbSet<PatientMedication> PatientMedications { get; set; }
        public virtual DbSet<PatientStatus> PatientStatus { get; set; }
        public virtual DbSet<PatientStatusHistory> PatientStatusHistories { get; set; }
        public virtual DbSet<PostDeployment> PostDeployments { get; set; }
        public virtual DbSet<Pregnancy> Pregnancies { get; set; }
        public virtual DbSet<Priority> Priorities { get; set; }
        public virtual DbSet<ReportInstance> ReportInstances { get; set; }
        public virtual DbSet<ReportInstanceMedication> ReportInstanceMedications { get; set; }
        public virtual DbSet<RiskFactor> RiskFactors { get; set; }
        public virtual DbSet<RiskFactorOption> RiskFactorOptions { get; set; }
        public virtual DbSet<Role> Roles { get; set; }
        public virtual DbSet<SelectionDataItem> SelectionDataItems { get; set; }
        public virtual DbSet<SiteContactDetail> SiteContactDetails { get; set; }
        public virtual DbSet<TerminologyIcd10> TerminologyIcd10 { get; set; }
        public virtual DbSet<TerminologyMedDra> TerminologyMedDras { get; set; }
        public virtual DbSet<TreatmentOutcome> TreatmentOutcomes { get; set; }
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<UserFacility> UserFacilities { get; set; }
        public virtual DbSet<UserRole> UserRoles { get; set; }
        public virtual DbSet<WorkFlow> WorkFlows { get; set; }
        public virtual DbSet<WorkPlan> WorkPlans { get; set; }
        public virtual DbSet<WorkPlanCareEvent> WorkPlanCareEvents { get; set; }
        public virtual DbSet<WorkPlanCareEventDatasetCategory> WorkPlanCareEventDatasetCategories { get; set; }

        public override int SaveChanges()
        {
            foreach (var changedAuditedEntity in ChangeTracker.Entries().Where(IsAuditedEntityThatHasChanged).Select(e => (AuditedEntity<int, User>)e.Entity))
            {
                // Need to retrieve the current user from the current dbcontext because UserContext.CurrentUser is populated, in the HttpContextUserContext object, from 
                // a different dbcontext. A duplicate User record is inserted in the DB when UserContext.CurrentUser is passed in to the AuditStamp operation, resulting in a 
                // duplicate user record. Not sure if the retrieval of a user from different dbcontexts is the root cause of the duplication but, adding the 
                // user retrieval operation below solves the issue of duplicate users being created.
                var currentUser = UserContext.CurrentUser != null ? Users.SingleOrDefault(u => u.Id == UserContext.CurrentUser.Id) : null;

                changedAuditedEntity.AuditStamp(currentUser);
            }

#if DEBUG
            try
            {
#endif
                return base.SaveChanges();
#if DEBUG
            }
            catch (DbEntityValidationException exception)
            {
                var err = string.Empty;
                foreach (var eve in exception.EntityValidationErrors)
                {
                    foreach (var ve in eve.ValidationErrors)
                    {
                        err += String.Format("- Property: \"{0}\", Error: \"{1}\"", ve.PropertyName, ve.ErrorMessage);
                    }
                }
                throw new Exception(err);
            }
            catch (Exception exception)
            {
                throw; // provided for debugging reasons.
            }
#endif
        }

        private bool IsAuditedEntityThatHasChanged(DbEntityEntry entry)
        {
            if (entry.State == EntityState.Unchanged) return false;

            if (entry.State == EntityState.Deleted) return false;

            return typeof(AuditedEntity<int, User>).IsAssignableFrom(entry.Entity.GetType());
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Appointment>()
                .HasOptional<User>(s => s.AuditUser)
                .WithMany(s => s.Appointments)
                .HasForeignKey(s => s.AuditUser_Id);

            modelBuilder.Entity<Attachment>()
                .HasOptional<User>(s => s.AuditUser)
                .WithMany(s => s.Attachments)
                .HasForeignKey(s => s.AuditUser_Id);

            modelBuilder.Entity<Encounter>()
                .HasOptional<User>(s => s.AuditUser)
                .WithMany(s => s.Encounters)
                .HasForeignKey(s => s.AuditUser_Id);

            modelBuilder.Entity<Patient>()
                .HasOptional<User>(s => s.AuditUser)
                .WithMany(s => s.Patients)
                .HasForeignKey(s => s.AuditUser_Id);

            modelBuilder.Entity<PatientStatusHistory>()
                .HasOptional<User>(s => s.AuditUser)
                .WithMany(s => s.PatientStatusHistories)
                .HasForeignKey(s => s.AuditUser_Id);

            modelBuilder.Entity<PostDeployment>()
                .Property(f => f.RunDate)
                .HasColumnType("datetime2")
                .HasPrecision(0);

            modelBuilder.Entity<PostDeployment>()
                .Property(f => f.ScriptFileName)
                .HasColumnAnnotation("Index", new IndexAnnotation(new IndexAttribute { IsUnique = true }));
        }

        public class CreateInitializer : DropCreateDatabaseAlways<PVIMSDbContext>
        {
            protected override void Seed(PVIMSDbContext context)
            {
                context.Seed(context);

                base.Seed(context);
            }
        }


        public void Seed(PVIMSDbContext context)
        {
            if (context.Roles.Any()) return;

            using (var transaction = new TransactionScope())
            {
                CreateRoles(context);
                CreateUser(context);
                CreateAttachmentTypes(context);
                CreatePriorities(context);
                CreateCareEvents(context);
                CreateDatasetElementTypes(context);
                CreateLabResults(context);
                CreateLabTestUnits(context);
                CreateLabTests(context);
                CreateOutcomes(context);
                CreateTreatmentOutcomes(context);
                CreateCustomAttributes(context);
                CreateSelectDataItem(context);
                CreateFacilityTypes(context);
                CreateFieldTypes(context);
                CreateContextTypes(context);
                CreateEncounterTypes(context);
                CreateStatus(context);
                CreateMetaTypes(context);
                CreatePostDeploymentScripts(context);
                CreateConfigValues(context);
                CreateContactTypes(context);

                //To be done through SQL Scripts
                //CreateEncounterDatasetElements(context);
                //CreateInitiationDataset(context);
                //CreateReviewDataset(context);
                //CreateGlobalDatasetElements(context);
                //CreateSpontaneousDataset(context);
                //CreateE2BDatasetElements(context);

                transaction.Complete();
            }
        }

        private static void CreateUser(PVIMSDbContext context)
        {
            // Password: P@55w0rd1
            var adminUser = new User
            {
                UserName = "Admin",
                FirstName = "Admin",
                LastName = "User",
                Active = true,
                Email = "admin@mail.com",
                PasswordHash = "AGW4fU8+b1lKknBmk1U78xlZwJvxihua6thtRmUJRFZhgFepPm9FfgnXiUMe2SkVEw=="
            };
            context.Users.AddOrUpdate(u => u.UserName, adminUser);

            context.UserRoles.Add(new UserRole
            {
                User = adminUser,
                Role = context.Roles.SingleOrDefault(r => r.Key == "Admin")
            });
            context.UserRoles.Add(new UserRole
            {
                User = adminUser,
                Role = context.Roles.SingleOrDefault(r => r.Key == "DataCap")
            });
            context.UserRoles.Add(new UserRole
            {
                User = adminUser,
                Role = context.Roles.SingleOrDefault(r => r.Key == "Analyst")
            });
            context.UserRoles.Add(new UserRole
            {
                User = adminUser,
                Role = context.Roles.SingleOrDefault(r => r.Key == "Reporter")
            });
            context.UserRoles.Add(new UserRole
            {
                User = adminUser,
                Role = context.Roles.SingleOrDefault(r => r.Key == "Publisher")
            });
            context.UserRoles.Add(new UserRole
            {
                User = adminUser,
                Role = context.Roles.SingleOrDefault(r => r.Key == "ReporterAdmin")
            });
            context.UserRoles.Add(new UserRole
            {
                User = adminUser,
                Role = context.Roles.SingleOrDefault(r => r.Key == "PublisherAdmin")
            });
            context.SaveChanges();
        }

        private static void CreateStatus(PVIMSDbContext context)
        {
            var statuses = new[]
            {
                new PatientStatus {Description ="Active"},
                new PatientStatus {Description ="Suspended"},
                new PatientStatus {Description ="Stopped ART"},
                new PatientStatus {Description ="Investigation"},
                new PatientStatus {Description ="LTF"},
                new PatientStatus {Description ="Stopped PMTCT"},
                new PatientStatus {Description ="Transferred Out"},
                new PatientStatus {Description ="Died"},
            };

            context.PatientStatus.AddOrUpdate(s => new { s.Description }, statuses);
            context.SaveChanges();
        }

        private static void CreateRoles(PVIMSDbContext context)
        {
            var roles = new[]
			{
				new Role { Name = "Administrator", Key = "Admin" },
				new Role { Name = "Registration Clerk", Key = "RegClerk" },
				new Role { Name = "Data Capturer", Key = "DataCap" },
                new Role { Name = "Clinician", Key = "Clinician" },
                new Role { Name = "Analytics", Key = "Analyst" },
                new Role { Name = "Reporter", Key = "Reporter" },
                new Role { Name = "Publisher", Key = "Publisher" },
                new Role { Name = "Reporter Administrator", Key = "ReporterAdmin" },
                new Role { Name = "Publisher Administrator", Key = "PublisherAdmin" }
			};

            context.Roles.AddOrUpdate(r => new { r.Name, r.Key }, roles);
            context.SaveChanges();
        }

        private static void CreateAttachmentTypes(PVIMSDbContext context)
        {
            var attachmentTypes = new[]
			{
				new AttachmentType {Description = "MS Word 2003-2007 Document", Key = "doc"},
				new AttachmentType {Description = "MS Excel 2003-2007 Document", Key = "xls"},
				new AttachmentType {Description = "MS Word Document", Key = "docx"},
				new AttachmentType {Description = "MS Excel Document", Key = "xlsx"},
                new AttachmentType {Description = "Portable Document Format", Key = "pdf"},
				new AttachmentType {Description = "Image | JPEG", Key = "jpg"},
				new AttachmentType {Description = "Image | JPEG", Key = "jpeg"},
				new AttachmentType {Description = "Image | PNG", Key = "png"},
				new AttachmentType {Description = "Image | BMP", Key = "bmp"},
                new AttachmentType {Description = "XML Document", Key = "xml"}
            };

            context.AttachmentTypes.AddOrUpdate(at => new { at.Description, at.Key }, attachmentTypes);
            context.SaveChanges();
        }

        private static void CreatePriorities(PVIMSDbContext context)
        {
            var priorities = new[]
			{
                new Priority { Description = "Not Set" },
				new Priority { Description = "Urgent" },
				new Priority { Description = "High" },
				new Priority { Description = "Medium" },
                new Priority { Description = "Low" }
			};

            context.Priorities.AddOrUpdate(p => new { p.Description }, priorities);
            context.SaveChanges();
        }

        private static void CreateCareEvents(PVIMSDbContext context)
        {
            var careEvents = new[]
			{
				new CareEvent {Description = "Capture Vitals"},
				new CareEvent {Description = "Counsel and Pill Count"},
				new CareEvent {Description = "Doctor Assessment"},
				new CareEvent {Description = "Nurse Assessment"},
				new CareEvent {Description = "Drug Dispensement"}
			};

            context.CareEvents.AddOrUpdate(ce => ce.Description, careEvents);
            context.SaveChanges();
        }

        private static void CreateDatasetElementTypes(PVIMSDbContext context)
        {
            var elementTypes = new[]
			{
				new DatasetElementType {Description = "Generic"}
			};

            context.DatasetElementTypes.AddOrUpdate(det => det.Description, elementTypes);
            context.SaveChanges();
        }

        private static void CreateLabResults(PVIMSDbContext context)
        {
            var labResults = new[]
			{
                new LabResult { Description = "Positive" },
                new LabResult { Description = "Negative" },
                new LabResult { Description = "Borderline" },
                new LabResult { Description = "Inconclusive" },
                new LabResult { Description = "Normal" },
                new LabResult { Description = "Abnormal" },
                new LabResult { Description = "Seronegative" },
                new LabResult { Description = "Seropositive" },
                new LabResult { Description = "Improved" },
                new LabResult { Description = "Stable" },
                new LabResult { Description = "Progressed" }
			};

            context.LabResults.AddOrUpdate(lt => lt.Description, labResults);
            context.SaveChanges();
        }

        private static void CreateLabTestUnits(PVIMSDbContext context)
        {
            var labTestUnits = new[]
			{
                new LabTestUnit { Description = "N/A" },
                new LabTestUnit { Description = "%" },
                new LabTestUnit { Description = "% hearing loss left ear" },
                new LabTestUnit { Description = "% hearing loss right ear" },
                new LabTestUnit { Description = "µg/dL" },
                new LabTestUnit { Description = "µg/L" },
                new LabTestUnit { Description = "beats per minute" },
                new LabTestUnit { Description = "breaths per minute" },
                new LabTestUnit { Description = "cavities" },
                new LabTestUnit { Description = "cells/mm 3" },
                new LabTestUnit { Description = "g/dL" },
                new LabTestUnit { Description = "g/L" },
                new LabTestUnit { Description = "IU/L" },
                new LabTestUnit { Description = "kg/m 2" },
                new LabTestUnit { Description = "mEq/L" },
                new LabTestUnit { Description = "mg/24 hr" },
                new LabTestUnit { Description = "mg/dL" },
                new LabTestUnit { Description = "min" },
                new LabTestUnit { Description = "mL/min" },
                new LabTestUnit { Description = "mm Hg" },
                new LabTestUnit { Description = "mm/h" },
                new LabTestUnit { Description = "mmol/kg" },
                new LabTestUnit { Description = "mmol/L" },
                new LabTestUnit { Description = "mOsm/kg" },
                new LabTestUnit { Description = "ms" },
                new LabTestUnit { Description = "ng/dL" },
                new LabTestUnit { Description = "ng/L" },
                new LabTestUnit { Description = "ng/mL" },
                new LabTestUnit { Description = "ng/mL/hr" },
                new LabTestUnit { Description = "nmol/L" },
                new LabTestUnit { Description = "pg/mL" },
                new LabTestUnit { Description = "pH" },
                new LabTestUnit { Description = "pmol/L" },
                new LabTestUnit { Description = "sec" },
                new LabTestUnit { Description = "U/L" },
                new LabTestUnit { Description = "X 10 3 /mm 3" },
                new LabTestUnit { Description = "X 10 6 /mm 3" },
                new LabTestUnit { Description = "X 10 9 /L" },
                new LabTestUnit { Description = "μg/dL" },
                new LabTestUnit { Description = "μg/L" },
                new LabTestUnit { Description = "μmol/L" },
                new LabTestUnit { Description = "μU/L" },
                new LabTestUnit { Description = "μU/mL" }
			};

            context.LabTestUnits.AddOrUpdate(lt => lt.Description, labTestUnits);
            context.SaveChanges();
        }

        private static void CreateLabTests(PVIMSDbContext context)
        {
            var labTests = new[]
			{
                new LabTest { Active = true, Description = "AFB Smear Result" },
                new LabTest { Active = true, Description = "Albumin" },
                new LabTest { Active = true, Description = "ALT (SGPT)" },
                new LabTest { Active = true, Description = "Amylase" },
                new LabTest { Active = true, Description = "AST (SGOT)" },
                new LabTest { Active = true, Description = "Audiometry" },
                new LabTest { Active = true, Description = "Blood Glucose" },
                new LabTest { Active = true, Description = "Chest X-Ray" },
                new LabTest { Active = true, Description = "CD4 Count" },
                new LabTest { Active = true, Description = "Creatinine Clearance" },
                new LabTest { Active = true, Description = "Culture Results" },
                new LabTest { Active = true, Description = "QTc interval" },
                new LabTest { Active = true, Description = "ESR" },
                new LabTest { Active = true, Description = "Glucose" },
                new LabTest { Active = true, Description = "Heart Rate" },
                new LabTest { Active = true, Description = "Haemoglobin" },
                new LabTest { Active = true, Description = "Hepatitis B Virus" },
                new LabTest { Active = true, Description = "Hepatitis C Virus" },
				new LabTest { Active = true, Description = "HIV Antibody" },
                new LabTest { Active = true, Description = "Lactic acid" },
                new LabTest { Active = true, Description = "Lipase" },
                new LabTest { Active = true, Description = "Neurological Exam" },
                new LabTest { Active = true, Description = "Platelet Count" },
                new LabTest { Active = true, Description = "Pregnancy Test" },
                new LabTest { Active = true, Description = "PR Interval" },
                new LabTest { Active = true, Description = "QRS Duration" },
                new LabTest { Active = true, Description = "QT Duration" },
                new LabTest { Active = true, Description = "RBC Count" },
                new LabTest { Active = true, Description = "Serum Creatinine (SCr)" },
                new LabTest { Active = true, Description = "Serum Potassium (K+)" },
                new LabTest { Active = true, Description = "Serum Magnesium (Mg++)" },
                new LabTest { Active = true, Description = "Serum Calcium (Ca++)" },
                new LabTest { Active = true, Description = "Smear" },
                new LabTest { Active = true, Description = "Total WBC" },
                new LabTest { Active = true, Description = "TSH" },
                new LabTest { Active = true, Description = "Visual acuity" },
                new LabTest { Active = true, Description = "WBC Count" }
			};

            context.LabTests.AddOrUpdate(lt => lt.Description, labTests);
            context.SaveChanges();
        }

        private static void CreateOutcomes(PVIMSDbContext context)
        {
            var outcomes = new[]
			{
				new Outcome { Description = "Recovered/Resolved" },
				new Outcome { Description = "Recovered/Resolved With Sequelae" },
				new Outcome { Description = "Recovering/Resolving" },
                new Outcome { Description = "Not Recovered/Not Resolved" },
                new Outcome { Description = "Fatal" },
                new Outcome { Description = "Unknown" }
			};

            context.Outcomes.AddOrUpdate(p => p.Description, outcomes);
            context.SaveChanges();
        }

        private static void CreateTreatmentOutcomes(PVIMSDbContext context)
        {
            var treatmentOutcomes = new[]
			{
				new TreatmentOutcome { Description = "Cured" },
				new TreatmentOutcome { Description = "Treatment Completed" },
				new TreatmentOutcome { Description = "Treatment Failed" },
                new TreatmentOutcome { Description = "Died" },
                new TreatmentOutcome { Description = "Lost to Follow-up" },
                new TreatmentOutcome { Description = "Not Evaluated" }
			};

            context.TreatmentOutcomes.AddOrUpdate(p => p.Description, treatmentOutcomes);
            context.SaveChanges();
        }

        private static void CreateCustomAttributes(PVIMSDbContext context)
        {
            var customAttributeConfigs = new[]
			{
				new CustomAttributeConfiguration
				{
					ExtendableTypeName = "PatientClinicalEvent",
					CustomAttributeType = CustomAttributeType.String,
					Category = "Custom",
					AttributeKey = "SAE Number",
                    StringMaxLength = 50
				},
				new CustomAttributeConfiguration
				{
					ExtendableTypeName = "PatientClinicalEvent",
					CustomAttributeType = CustomAttributeType.Selection,
					Category = "Custom",
					AttributeKey = "Outcome",
                    AttributeDetail = "For fatal outcomes, please ensure all conditions are updated to reflect the relevant condition outcome"
				},
				new CustomAttributeConfiguration
				{
					ExtendableTypeName = "PatientClinicalEvent",
					CustomAttributeType = CustomAttributeType.Selection,
					Category = "Custom",
					AttributeKey = "Intensity (Severity)"
				},
				new CustomAttributeConfiguration
				{
					ExtendableTypeName = "PatientClinicalEvent",
					CustomAttributeType = CustomAttributeType.Selection,
					Category = "Custom",
					AttributeKey = "Severity Grading Scale"
				},
				new CustomAttributeConfiguration
				{
					ExtendableTypeName = "PatientClinicalEvent",
					CustomAttributeType = CustomAttributeType.Selection,
					Category = "Custom",
					AttributeKey = "Severity Grade"
				},
				new CustomAttributeConfiguration
				{
					ExtendableTypeName = "PatientClinicalEvent",
					CustomAttributeType = CustomAttributeType.Selection,
					Category = "Custom",
					AttributeKey = "Is the adverse event serious?"
				},
				new CustomAttributeConfiguration
				{
					ExtendableTypeName = "PatientClinicalEvent",
					CustomAttributeType = CustomAttributeType.Selection,
					Category = "Custom",
					AttributeKey = "Seriousness"
				},
				new CustomAttributeConfiguration
				{
					ExtendableTypeName = "PatientClinicalEvent",
					CustomAttributeType = CustomAttributeType.DateTime,
					Category = "Custom",
					AttributeKey = "Admission Date",
                    PastDateOnly = true
				},
				new CustomAttributeConfiguration
				{
					ExtendableTypeName = "PatientClinicalEvent",
					CustomAttributeType = CustomAttributeType.DateTime,
					Category = "Custom",
					AttributeKey = "Discharge Date",
                    PastDateOnly = true
				},
				new CustomAttributeConfiguration
				{
					ExtendableTypeName = "PatientClinicalEvent",
					CustomAttributeType = CustomAttributeType.DateTime,
					Category = "Custom",
					AttributeKey = "Date of Death",
                    PastDateOnly = true
				},
				new CustomAttributeConfiguration
				{
					ExtendableTypeName = "PatientClinicalEvent",
					CustomAttributeType = CustomAttributeType.Selection,
					Category = "Custom",
					AttributeKey = "Autopsy Done"
				},
				new CustomAttributeConfiguration
				{
					ExtendableTypeName = "PatientClinicalEvent",
					CustomAttributeType = CustomAttributeType.Selection,
					Category = "Custom",
					AttributeKey = "Was the AE attributed to one or more drugs?"
				},
				new CustomAttributeConfiguration
				{
					ExtendableTypeName = "PatientClinicalEvent",
					CustomAttributeType = CustomAttributeType.Selection,
					Category = "Custom",
					AttributeKey = "Was the event reported to national PV?"
				},
				new CustomAttributeConfiguration
				{
					ExtendableTypeName = "PatientClinicalEvent",
					CustomAttributeType = CustomAttributeType.String,
					Category = "Custom",
					AttributeKey = "Reported By",
                    StringMaxLength = 50
				},
				new CustomAttributeConfiguration
				{
					ExtendableTypeName = "PatientClinicalEvent",
					CustomAttributeType = CustomAttributeType.DateTime,
					Category = "Custom",
					AttributeKey = "Date of Report",
                    PastDateOnly = true
				},
				new CustomAttributeConfiguration
				{
					ExtendableTypeName = "PatientClinicalEvent",
					CustomAttributeType = CustomAttributeType.String,
					Category = "Custom",
					AttributeKey = "Comments",
                    StringMaxLength = 100
				},
				new CustomAttributeConfiguration
				{
					ExtendableTypeName = "PatientCondition",
					CustomAttributeType = CustomAttributeType.Selection,
					Category = "Custom",
					AttributeKey = "Condition Ongoing"
				},
				new CustomAttributeConfiguration
				{
					ExtendableTypeName = "PatientMedication",
					CustomAttributeType = CustomAttributeType.Selection,
					Category = "Custom",
					AttributeKey = "Route"
				},
				new CustomAttributeConfiguration
				{
					ExtendableTypeName = "PatientMedication",
					CustomAttributeType = CustomAttributeType.Selection,
					Category = "Custom",
					AttributeKey = "Frequency in days per week"
				},
				new CustomAttributeConfiguration
				{
					ExtendableTypeName = "PatientMedication",
					CustomAttributeType = CustomAttributeType.Selection,
					Category = "Custom",
					AttributeKey = "Still On Medication"
				},
				new CustomAttributeConfiguration
				{
					ExtendableTypeName = "PatientMedication",
					CustomAttributeType = CustomAttributeType.String,
					Category = "Custom",
					AttributeKey = "Indication",
                    StringMaxLength = 50
				},
				new CustomAttributeConfiguration
				{
					ExtendableTypeName = "PatientMedication",
					CustomAttributeType = CustomAttributeType.Selection,
					Category = "Custom",
					AttributeKey = "Type of Indication"
				},
				new CustomAttributeConfiguration
				{
					ExtendableTypeName = "PatientMedication",
					CustomAttributeType = CustomAttributeType.Selection,
					Category = "Custom",
					AttributeKey = "Reason For Stopping"
				},
				new CustomAttributeConfiguration
				{
					ExtendableTypeName = "PatientMedication",
					CustomAttributeType = CustomAttributeType.Selection,
					Category = "Custom",
					AttributeKey = "Clinician action taken with regard to medicine if related to AE"
				},
				new CustomAttributeConfiguration
				{
					ExtendableTypeName = "PatientMedication",
					CustomAttributeType = CustomAttributeType.String,
					Category = "Custom",
					AttributeKey = "Batch Number",
                    StringMaxLength = 50
				},
				new CustomAttributeConfiguration
				{
					ExtendableTypeName = "PatientMedication",
					CustomAttributeType = CustomAttributeType.Selection,
					Category = "Custom",
					AttributeKey = "Effect OF Dechallenge (D) & Rechallenge (R)"
				},
				new CustomAttributeConfiguration
				{
					ExtendableTypeName = "PatientMedication",
					CustomAttributeType = CustomAttributeType.String,
					Category = "Custom",
					AttributeKey = "Comments",
                    StringMaxLength = 100
				},
				new CustomAttributeConfiguration
				{
					ExtendableTypeName = "Patient",
					CustomAttributeType = CustomAttributeType.String,
					Category = "Custom",
					AttributeKey = "Patient File Number",
                    IsRequired = true,
                    StringMaxLength = 50,
                    IsSearchable = true
				},
				new CustomAttributeConfiguration
				{
					ExtendableTypeName = "Patient",
					CustomAttributeType = CustomAttributeType.String,
					Category = "Custom",
					AttributeKey = "Patient Identity Number",
                    IsRequired = true,
                    StringMaxLength = 11
				},
				new CustomAttributeConfiguration
				{
					ExtendableTypeName = "Patient",
					CustomAttributeType = CustomAttributeType.Selection,
					Category = "Custom",
					AttributeKey = "Identity Type",
                    IsRequired = true
				},
				new CustomAttributeConfiguration
				{
					ExtendableTypeName = "Patient",
					CustomAttributeType = CustomAttributeType.Selection,
					Category = "Custom",
					AttributeKey = "Gender",
                    IsRequired = true
				},
				new CustomAttributeConfiguration
				{
					ExtendableTypeName = "Patient",
					CustomAttributeType = CustomAttributeType.Selection,
					Category = "Custom",
					AttributeKey = "Marital Status"
				},
				new CustomAttributeConfiguration
				{
					ExtendableTypeName = "Patient",
					CustomAttributeType = CustomAttributeType.Selection,
					Category = "Custom",
					AttributeKey = "Employment Status"
				},
				new CustomAttributeConfiguration
				{
					ExtendableTypeName = "Patient",
					CustomAttributeType = CustomAttributeType.String,
					Category = "Custom",
					AttributeKey = "Occupation",
                    StringMaxLength = 50
				},
				new CustomAttributeConfiguration
				{
					ExtendableTypeName = "Patient",
					CustomAttributeType = CustomAttributeType.Selection,
					Category = "Custom",
					AttributeKey = "Language"
				},
				new CustomAttributeConfiguration
				{
					ExtendableTypeName = "Patient",
					CustomAttributeType = CustomAttributeType.String,
					Category = "Custom",
					AttributeKey = "Address",
                    StringMaxLength = 100
				},
				new CustomAttributeConfiguration
				{
					ExtendableTypeName = "Patient",
					CustomAttributeType = CustomAttributeType.String,
					Category = "Custom",
					AttributeKey = "Address Line 2",
                    StringMaxLength = 100
				},
				new CustomAttributeConfiguration
				{
					ExtendableTypeName = "Patient",
					CustomAttributeType = CustomAttributeType.String,
					Category = "Custom",
					AttributeKey = "City",
                    StringMaxLength = 50
				},
				new CustomAttributeConfiguration
				{
					ExtendableTypeName = "Patient",
					CustomAttributeType = CustomAttributeType.String,
					Category = "Custom",
					AttributeKey = "State",
                    StringMaxLength = 50
				},
				new CustomAttributeConfiguration
				{
					ExtendableTypeName = "Patient",
					CustomAttributeType = CustomAttributeType.String,
					Category = "Custom",
					AttributeKey = "Postal Code",
                    StringMaxLength = 10
				},
				new CustomAttributeConfiguration
				{
					ExtendableTypeName = "Patient",
					CustomAttributeType = CustomAttributeType.String,
					Category = "Custom",
					AttributeKey = "Patient Contact Number",
                    StringMaxLength = 15
				},
                new CustomAttributeConfiguration
                {
                    ExtendableTypeName = "PatientLabTest",
                    CustomAttributeType = CustomAttributeType.String,
                    Category = "Lab Test",
                    AttributeKey = "Comments",
                    StringMaxLength = 255
                }
            };

            context.CustomAttributeConfigurations.AddOrUpdate(c => new { c.AttributeKey, c.ExtendableTypeName }, customAttributeConfigs);
            context.SaveChanges();
        }

        private static void CreateSelectDataItem(PVIMSDbContext context)
        {
            var selectionDataItems = new[]
			{
				new SelectionDataItem {AttributeKey = "Outcome", Value = "", SelectionKey = "0"},
				new SelectionDataItem {AttributeKey = "Outcome", Value = "Resolved", SelectionKey = "1"},
				new SelectionDataItem {AttributeKey = "Outcome", Value = "Resolved with sequelae", SelectionKey = "2"},
				new SelectionDataItem {AttributeKey = "Outcome", Value = "Fatal", SelectionKey = "3"},
				new SelectionDataItem {AttributeKey = "Outcome", Value = "Resolving", SelectionKey = "4"},
                new SelectionDataItem {AttributeKey = "Outcome", Value = "Not resolved", SelectionKey = "5"},
				new SelectionDataItem {AttributeKey = "Outcome", Value = "Unknown", SelectionKey = "6"},
				new SelectionDataItem {AttributeKey = "Intensity (Severity)", Value = "", SelectionKey = "0"},
                new SelectionDataItem {AttributeKey = "Intensity (Severity)", Value = "Mild", SelectionKey = "1"},
                new SelectionDataItem {AttributeKey = "Intensity (Severity)", Value = "Moderate", SelectionKey = "2"},
                new SelectionDataItem {AttributeKey = "Intensity (Severity)", Value = "Severe", SelectionKey = "3"},
				new SelectionDataItem {AttributeKey = "Severity Grading Scale", Value = "", SelectionKey = "0"},
                new SelectionDataItem {AttributeKey = "Severity Grading Scale", Value = "WHO Scale", SelectionKey = "1"},
                new SelectionDataItem {AttributeKey = "Severity Grading Scale", Value = "Clinician’s judgement", SelectionKey = "2"},
                new SelectionDataItem {AttributeKey = "Severity Grading Scale", Value = "CTCAE grading system", SelectionKey = "3"},
                new SelectionDataItem {AttributeKey = "Severity Grading Scale", Value = "DAIDS AE Grading Table", SelectionKey = "4"},
                new SelectionDataItem {AttributeKey = "Severity Grading Scale", Value = "Other", SelectionKey = "5"},
				new SelectionDataItem {AttributeKey = "Severity Grade", Value = "", SelectionKey = "0"},
                new SelectionDataItem {AttributeKey = "Severity Grade", Value = "Grade 1", SelectionKey = "1"},
                new SelectionDataItem {AttributeKey = "Severity Grade", Value = "Grade 2", SelectionKey = "2"},
                new SelectionDataItem {AttributeKey = "Severity Grade", Value = "Grade 3", SelectionKey = "3"},
                new SelectionDataItem {AttributeKey = "Severity Grade", Value = "Grade 4", SelectionKey = "4"},
                new SelectionDataItem {AttributeKey = "Severity Grade", Value = "Grade 5", SelectionKey = "5"},
				new SelectionDataItem {AttributeKey = "Is the adverse event serious?", Value = "", SelectionKey = "0"},
                new SelectionDataItem {AttributeKey = "Is the adverse event serious?", Value = "Yes", SelectionKey = "1"},
                new SelectionDataItem {AttributeKey = "Is the adverse event serious?", Value = "No", SelectionKey = "2"},
                new SelectionDataItem {AttributeKey = "Is the adverse event serious?", Value = "Unknown", SelectionKey = "3"},
				new SelectionDataItem {AttributeKey = "Seriousness", Value = "", SelectionKey = "0"},
                new SelectionDataItem {AttributeKey = "Seriousness", Value = "A congenital anomaly or birth defect", SelectionKey = "1"},
                new SelectionDataItem {AttributeKey = "Seriousness", Value = "Persistent or significant disability or incapacity", SelectionKey = "2"},
                new SelectionDataItem {AttributeKey = "Seriousness", Value = "Death", SelectionKey = "3"},
                new SelectionDataItem {AttributeKey = "Seriousness", Value = "Initial or prolonged hospitalization", SelectionKey = "4"},
                new SelectionDataItem {AttributeKey = "Seriousness", Value = "Life threatening", SelectionKey = "5"},
                new SelectionDataItem {AttributeKey = "Seriousness", Value = "A medically important event", SelectionKey = "6"},
				new SelectionDataItem {AttributeKey = "Effect OF Dechallenge (D) & Rechallenge (R)", Value = "", SelectionKey = "0"},
                new SelectionDataItem {AttributeKey = "Effect OF Dechallenge (D) & Rechallenge (R)", Value = "Not Applicable", SelectionKey = "1"},
                new SelectionDataItem {AttributeKey = "Effect OF Dechallenge (D) & Rechallenge (R)", Value = "D - AE improved/ resolved when medicine dose reduced/interrupted /withdrawn", SelectionKey = "2"},
                new SelectionDataItem {AttributeKey = "Effect OF Dechallenge (D) & Rechallenge (R)", Value = "D - AE did not improve/ resolve when medicine dose reduced/interrupted /withdrawn", SelectionKey = "3"},
                new SelectionDataItem {AttributeKey = "Effect OF Dechallenge (D) & Rechallenge (R)", Value = "D - Unknown", SelectionKey = "4"},
                new SelectionDataItem {AttributeKey = "Effect OF Dechallenge (D) & Rechallenge (R)", Value = "R - patient not re-exposed to the medicine", SelectionKey = "5"},
                new SelectionDataItem {AttributeKey = "Effect OF Dechallenge (D) & Rechallenge (R)", Value = "R - AE recurred on medicine re-administration/dose increase", SelectionKey = "6"},
                new SelectionDataItem {AttributeKey = "Effect OF Dechallenge (D) & Rechallenge (R)", Value = "R - AE did not recur on medicine re-administration/dose increase", SelectionKey = "7"},
                new SelectionDataItem {AttributeKey = "Effect OF Dechallenge (D) & Rechallenge (R)", Value = "R - Unknown", SelectionKey = "8"},
				new SelectionDataItem {AttributeKey = "Was the AE attributed to one or more drugs?", Value = "", SelectionKey = "0"},
                new SelectionDataItem {AttributeKey = "Was the AE attributed to one or more drugs?", Value = "Yes", SelectionKey = "1"},
                new SelectionDataItem {AttributeKey = "Was the AE attributed to one or more drugs?", Value = "No", SelectionKey = "2"},
                new SelectionDataItem {AttributeKey = "Was the AE attributed to one or more drugs?", Value = "Unknown", SelectionKey = "3"},
				new SelectionDataItem {AttributeKey = "Was the event reported to national PV?", Value = "", SelectionKey = "0"},
                new SelectionDataItem {AttributeKey = "Was the event reported to national PV?", Value = "Yes", SelectionKey = "1"},
                new SelectionDataItem {AttributeKey = "Was the event reported to national PV?", Value = "No", SelectionKey = "2"},
                new SelectionDataItem {AttributeKey = "Was the event reported to national PV?", Value = "Unknown", SelectionKey = "3"},
				new SelectionDataItem {AttributeKey = "Reason For Stopping", Value = "", SelectionKey = "0"},
                new SelectionDataItem {AttributeKey = "Reason For Stopping", Value = "Adverse Event", SelectionKey = "1"},
                new SelectionDataItem {AttributeKey = "Reason For Stopping", Value = "Cost", SelectionKey = "2"},
                new SelectionDataItem {AttributeKey = "Reason For Stopping", Value = "Course Completed", SelectionKey = "3"},
                new SelectionDataItem {AttributeKey = "Reason For Stopping", Value = "Cured", SelectionKey = "4"},
                new SelectionDataItem {AttributeKey = "Reason For Stopping", Value = "Lost To Follow-up", SelectionKey = "5"},
                new SelectionDataItem {AttributeKey = "Reason For Stopping", Value = "Medicine Out of Stock", SelectionKey = "6"},
                new SelectionDataItem {AttributeKey = "Reason For Stopping", Value = "No Longer Needed", SelectionKey = "7"},
                new SelectionDataItem {AttributeKey = "Reason For Stopping", Value = "Patient Died", SelectionKey = "8"},
                new SelectionDataItem {AttributeKey = "Reason For Stopping", Value = "Patient Withdrew Consent", SelectionKey = "9"},
                new SelectionDataItem {AttributeKey = "Reason For Stopping", Value = "Planned Medication Change", SelectionKey = "10"},
                new SelectionDataItem {AttributeKey = "Reason For Stopping", Value = "Poor Adherence", SelectionKey = "11"},
                new SelectionDataItem {AttributeKey = "Reason For Stopping", Value = "Pregnancy", SelectionKey = "12"},
                new SelectionDataItem {AttributeKey = "Reason For Stopping", Value = "Treatment Failure", SelectionKey = "13"},
                new SelectionDataItem {AttributeKey = "Reason For Stopping", Value = "Not Applicable", SelectionKey = "14"},
				new SelectionDataItem {AttributeKey = "Clinician action taken with regard to medicine if related to AE", Value = "", SelectionKey = "0"},
                new SelectionDataItem {AttributeKey = "Clinician action taken with regard to medicine if related to AE", Value = "Dose not changed", SelectionKey = "1"},
                new SelectionDataItem {AttributeKey = "Clinician action taken with regard to medicine if related to AE", Value = "Dose reduced", SelectionKey = "2"},
                new SelectionDataItem {AttributeKey = "Clinician action taken with regard to medicine if related to AE", Value = "Drug interrupted", SelectionKey = "3"},
                new SelectionDataItem {AttributeKey = "Clinician action taken with regard to medicine if related to AE", Value = "Drug withdrawn", SelectionKey = "4"},
                new SelectionDataItem {AttributeKey = "Clinician action taken with regard to medicine if related to AE", Value = "Not applicable", SelectionKey = "5"},
				new SelectionDataItem {AttributeKey = "Type of Indication", Value = "", SelectionKey = "0"},
                new SelectionDataItem {AttributeKey = "Type of Indication", Value = "Primary", SelectionKey = "1"},
                new SelectionDataItem {AttributeKey = "Type of Indication", Value = "Pre-existing condition", SelectionKey = "2"},
                new SelectionDataItem {AttributeKey = "Type of Indication", Value = "Treat AE", SelectionKey = "3"},
                new SelectionDataItem {AttributeKey = "Gender", Value = "", SelectionKey = "0"},
                new SelectionDataItem {AttributeKey = "Gender", Value = "Male", SelectionKey = "1"},
                new SelectionDataItem {AttributeKey = "Gender", Value = "Female", SelectionKey = "2"},
                new SelectionDataItem {AttributeKey = "Marital Status", Value = "", SelectionKey = "0"},
                new SelectionDataItem {AttributeKey = "Marital Status", Value = "Single", SelectionKey = "1"},
                new SelectionDataItem {AttributeKey = "Marital Status", Value = "Married", SelectionKey = "2"},
                new SelectionDataItem {AttributeKey = "Marital Status", Value = "Divorced", SelectionKey = "3"},
                new SelectionDataItem {AttributeKey = "Marital Status", Value = "Widowed", SelectionKey = "4"},
                new SelectionDataItem {AttributeKey = "Marital Status", Value = "Legally Seperated", SelectionKey = "5"},
                new SelectionDataItem {AttributeKey = "Employment Status", Value = "", SelectionKey = "0"},
                new SelectionDataItem {AttributeKey = "Employment Status", Value = "Employed", SelectionKey = "1"},
                new SelectionDataItem {AttributeKey = "Employment Status", Value = "Unemployed", SelectionKey = "2"},
                new SelectionDataItem {AttributeKey = "Employment Status", Value = "Student", SelectionKey = "3"},
                new SelectionDataItem {AttributeKey = "Employment Status", Value = "N/A", SelectionKey = "4"},
                new SelectionDataItem {AttributeKey = "Language", Value = "", SelectionKey = "0"},
                new SelectionDataItem {AttributeKey = "Language", Value = "English", SelectionKey = "1"},
                new SelectionDataItem {AttributeKey = "Language", Value = "French", SelectionKey = "2"},
                new SelectionDataItem {AttributeKey = "Still On Medication", Value = "", SelectionKey = "0"},
                new SelectionDataItem {AttributeKey = "Still On Medication", Value = "Yes", SelectionKey = "1"},
                new SelectionDataItem {AttributeKey = "Still On Medication", Value = "No", SelectionKey = "2"},
                new SelectionDataItem {AttributeKey = "Condition Ongoing", Value = "", SelectionKey = "0"},
                new SelectionDataItem {AttributeKey = "Condition Ongoing", Value = "Yes", SelectionKey = "1"},
                new SelectionDataItem {AttributeKey = "Condition Ongoing", Value = "No", SelectionKey = "2"},
                new SelectionDataItem {AttributeKey = "Identity Type", Value = "", SelectionKey = "0"},
                new SelectionDataItem {AttributeKey = "Identity Type", Value = "National identity", SelectionKey = "1"},
                new SelectionDataItem {AttributeKey = "Identity Type", Value = "Passport number", SelectionKey = "2"},
                new SelectionDataItem {AttributeKey = "Identity Type", Value = "Work permit number", SelectionKey = "3"},
                new SelectionDataItem {AttributeKey = "Route", Value = "", SelectionKey = "0"},
                new SelectionDataItem {AttributeKey = "Route", Value = "Auricular (otic)", SelectionKey = "1"},
                new SelectionDataItem {AttributeKey = "Route", Value = "Buccal", SelectionKey = "2"},
                new SelectionDataItem {AttributeKey = "Route", Value = "Cutaneous", SelectionKey = "3"},
                new SelectionDataItem {AttributeKey = "Route", Value = "Dental", SelectionKey = "4"},
                new SelectionDataItem {AttributeKey = "Route", Value = "Endocervical", SelectionKey = "5"},
                new SelectionDataItem {AttributeKey = "Route", Value = "Endosinusial", SelectionKey = "6"},
                new SelectionDataItem {AttributeKey = "Route", Value = "Endotracheal", SelectionKey = "7"},
                new SelectionDataItem {AttributeKey = "Route", Value = "Epidural", SelectionKey = "8"},
                new SelectionDataItem {AttributeKey = "Route", Value = "Extra-amniotic", SelectionKey = "9"},
                new SelectionDataItem {AttributeKey = "Route", Value = "Hemodialysis", SelectionKey = "10"},
                new SelectionDataItem {AttributeKey = "Route", Value = "Intra corpus cavernosum", SelectionKey = "11"},
                new SelectionDataItem {AttributeKey = "Route", Value = "Intra-amniotic", SelectionKey = "12"},
                new SelectionDataItem {AttributeKey = "Route", Value = "Intra-arterial", SelectionKey = "13"},
                new SelectionDataItem {AttributeKey = "Route", Value = "Intra-articular", SelectionKey = "14"},
                new SelectionDataItem {AttributeKey = "Route", Value = "Intra-uterine", SelectionKey = "15"},
                new SelectionDataItem {AttributeKey = "Route", Value = "Intracardiac", SelectionKey = "16"},
                new SelectionDataItem {AttributeKey = "Route", Value = "Intracavernous", SelectionKey = "17"},
                new SelectionDataItem {AttributeKey = "Route", Value = "Intracerebral", SelectionKey = "18"},
                new SelectionDataItem {AttributeKey = "Route", Value = "Intracervical", SelectionKey = "19"},
                new SelectionDataItem {AttributeKey = "Route", Value = "Intracisternal", SelectionKey = "20"},
                new SelectionDataItem {AttributeKey = "Route", Value = "Intracorneal", SelectionKey = "21"},
                new SelectionDataItem {AttributeKey = "Route", Value = "Intracoronary", SelectionKey = "22"},
                new SelectionDataItem {AttributeKey = "Route", Value = "Intradermal", SelectionKey = "23"},
                new SelectionDataItem {AttributeKey = "Route", Value = "Intradiscal (intraspinal)", SelectionKey = "24"},
                new SelectionDataItem {AttributeKey = "Route", Value = "Intrahepatic", SelectionKey = "25"},
                new SelectionDataItem {AttributeKey = "Route", Value = "Intralesional", SelectionKey = "26"},
                new SelectionDataItem {AttributeKey = "Route", Value = "Intralymphatic", SelectionKey = "27"},
                new SelectionDataItem {AttributeKey = "Route", Value = "Intramedullar (bone marrow)", SelectionKey = "28"},
                new SelectionDataItem {AttributeKey = "Route", Value = "Intrameningeal", SelectionKey = "29"},
                new SelectionDataItem {AttributeKey = "Route", Value = "Intramuscular", SelectionKey = "30"},
                new SelectionDataItem {AttributeKey = "Route", Value = "Intraocular", SelectionKey = "31"},
                new SelectionDataItem {AttributeKey = "Route", Value = "Intrapericardial", SelectionKey = "32"},
                new SelectionDataItem {AttributeKey = "Route", Value = "Intraperitoneal", SelectionKey = "33"},
                new SelectionDataItem {AttributeKey = "Route", Value = "Intrapleural", SelectionKey = "34"},
                new SelectionDataItem {AttributeKey = "Route", Value = "Intrasynovial", SelectionKey = "35"},
                new SelectionDataItem {AttributeKey = "Route", Value = "Intratumor", SelectionKey = "36"},
                new SelectionDataItem {AttributeKey = "Route", Value = "Intrathecal", SelectionKey = "37"},
                new SelectionDataItem {AttributeKey = "Route", Value = "Intrathoracic", SelectionKey = "38"},
                new SelectionDataItem {AttributeKey = "Route", Value = "Intratracheal", SelectionKey = "39"},
                new SelectionDataItem {AttributeKey = "Route", Value = "Intravenous bolus", SelectionKey = "40"},
                new SelectionDataItem {AttributeKey = "Route", Value = "Intravenous drip", SelectionKey = "41"},
                new SelectionDataItem {AttributeKey = "Route", Value = "Intravenous (not otherwise specified)", SelectionKey = "42"},
                new SelectionDataItem {AttributeKey = "Route", Value = "Intravesical", SelectionKey = "43"},
                new SelectionDataItem {AttributeKey = "Route", Value = "Iontophoresis", SelectionKey = "44"},
                new SelectionDataItem {AttributeKey = "Route", Value = "Nasal", SelectionKey = "45"},
                new SelectionDataItem {AttributeKey = "Route", Value = "Occlusive dressing technique", SelectionKey = "46"},
                new SelectionDataItem {AttributeKey = "Route", Value = "Ophthalmic", SelectionKey = "47"},
                new SelectionDataItem {AttributeKey = "Route", Value = "Oral", SelectionKey = "48"},
                new SelectionDataItem {AttributeKey = "Route", Value = "Oropharingeal", SelectionKey = "49"},
                new SelectionDataItem {AttributeKey = "Route", Value = "Other", SelectionKey = "50"},
                new SelectionDataItem {AttributeKey = "Route", Value = "Parenteral 051", SelectionKey = "51"},
                new SelectionDataItem {AttributeKey = "Route", Value = "Periarticular", SelectionKey = "52"},
                new SelectionDataItem {AttributeKey = "Route", Value = "Perineural", SelectionKey = "53"},
                new SelectionDataItem {AttributeKey = "Route", Value = "Rectal", SelectionKey = "54"},
                new SelectionDataItem {AttributeKey = "Route", Value = "Respiratory (inhalation)", SelectionKey = "55"},
                new SelectionDataItem {AttributeKey = "Route", Value = "Retrobulbar", SelectionKey = "56"},
                new SelectionDataItem {AttributeKey = "Route", Value = "Sunconjunctival", SelectionKey = "57"},
                new SelectionDataItem {AttributeKey = "Route", Value = "Subcutaneous", SelectionKey = "58"},
                new SelectionDataItem {AttributeKey = "Route", Value = "Subdermal", SelectionKey = "59"},
                new SelectionDataItem {AttributeKey = "Route", Value = "Sublingual", SelectionKey = "60"},
                new SelectionDataItem {AttributeKey = "Route", Value = "Topical", SelectionKey = "61"},
                new SelectionDataItem {AttributeKey = "Route", Value = "Transdermal", SelectionKey = "62"},
                new SelectionDataItem {AttributeKey = "Route", Value = "Transmammary", SelectionKey = "63"},
                new SelectionDataItem {AttributeKey = "Route", Value = "Transplacental", SelectionKey = "64"},
                new SelectionDataItem {AttributeKey = "Route", Value = "Unknown", SelectionKey = "65"},
                new SelectionDataItem {AttributeKey = "Route", Value = "Urethral", SelectionKey = "66"},
                new SelectionDataItem {AttributeKey = "Route", Value = "Vaginal", SelectionKey = "67"},
                new SelectionDataItem {AttributeKey = "Frequency in days per week", Value = "", SelectionKey = "0"},
                new SelectionDataItem {AttributeKey = "Frequency in days per week", Value = "1", SelectionKey = "1"},
                new SelectionDataItem {AttributeKey = "Frequency in days per week", Value = "2", SelectionKey = "2"},
                new SelectionDataItem {AttributeKey = "Frequency in days per week", Value = "3", SelectionKey = "3"},
                new SelectionDataItem {AttributeKey = "Frequency in days per week", Value = "4", SelectionKey = "4"},
                new SelectionDataItem {AttributeKey = "Frequency in days per week", Value = "5", SelectionKey = "5"},
                new SelectionDataItem {AttributeKey = "Frequency in days per week", Value = "6", SelectionKey = "6"},
                new SelectionDataItem {AttributeKey = "Frequency in days per week", Value = "7", SelectionKey = "7"},
                new SelectionDataItem {AttributeKey = "Autopsy Done", Value = "", SelectionKey = "0"},
                new SelectionDataItem {AttributeKey = "Autopsy Done", Value = "Yes", SelectionKey = "1"},
                new SelectionDataItem {AttributeKey = "Autopsy Done", Value = "No", SelectionKey = "2"}
            };

            context.SelectionDataItems.AddOrUpdate(s => new { s.AttributeKey, s.Value }, selectionDataItems);
            context.SaveChanges();
        }

        private static void CreateFacilityTypes(PVIMSDbContext context)
        {
            var types = new[]
			{
				new FacilityType { Description = "Unknown" },
				new FacilityType { Description = "Hospital" },
				new FacilityType { Description = "CHC" },
                new FacilityType { Description = "PHC" }
            };

            context.FacilityTypes.AddOrUpdate(ft => ft.Description, types);
            context.SaveChanges();
        }

        private static void CreateFieldTypes(PVIMSDbContext context)
        {
            var fieldTypes = new[]
			{
				new FieldType {Description = "Listbox"},
				new FieldType {Description = "DropDownList"},
				new FieldType {Description = "AlphaNumericTextbox"},
				new FieldType {Description = "NumericTextbox"},
				new FieldType {Description = "YesNo"},
                new FieldType {Description = "Date"},
                new FieldType {Description = "Table"},
                new FieldType {Description = "System"}
			};

            context.FieldTypes.AddOrUpdate(ft => ft.Description, fieldTypes);
            context.SaveChanges();
        }

        private static void CreateMetaTypes(PVIMSDbContext context)
        {
            var metaTableTypes = new[]
			{
				new MetaTableType { metatabletype_guid = Guid.NewGuid(), Description = "Core" },
                new MetaTableType { metatabletype_guid = Guid.NewGuid(), Description = "CoreChild" },
                new MetaTableType { metatabletype_guid = Guid.NewGuid(), Description = "Child" },
                new MetaTableType { metatabletype_guid = Guid.NewGuid(), Description = "History" },
                new MetaTableType { metatabletype_guid = Guid.NewGuid(), Description = "Lookup" }
			};

            context.MetaTableTypes.AddOrUpdate(mt => mt.Description, metaTableTypes);
            context.SaveChanges();

            var metaColumnTypes = new[]
			{
				new MetaColumnType { metacolumntype_guid = Guid.NewGuid(), Description = "bigint" },
				new MetaColumnType { metacolumntype_guid = Guid.NewGuid(), Description = "binary" },
				new MetaColumnType { metacolumntype_guid = Guid.NewGuid(), Description = "bit" },
				new MetaColumnType { metacolumntype_guid = Guid.NewGuid(), Description = "char" },
				new MetaColumnType { metacolumntype_guid = Guid.NewGuid(), Description = "date" },
				new MetaColumnType { metacolumntype_guid = Guid.NewGuid(), Description = "datetime" },
				new MetaColumnType { metacolumntype_guid = Guid.NewGuid(), Description = "decimal" },
				new MetaColumnType { metacolumntype_guid = Guid.NewGuid(), Description = "image" },
				new MetaColumnType { metacolumntype_guid = Guid.NewGuid(), Description = "int" },
				new MetaColumnType { metacolumntype_guid = Guid.NewGuid(), Description = "nchar" },
				new MetaColumnType { metacolumntype_guid = Guid.NewGuid(), Description = "nvarchar" },
				new MetaColumnType { metacolumntype_guid = Guid.NewGuid(), Description = "smallint" },
				new MetaColumnType { metacolumntype_guid = Guid.NewGuid(), Description = "time" },
				new MetaColumnType { metacolumntype_guid = Guid.NewGuid(), Description = "tinyint" },
				new MetaColumnType { metacolumntype_guid = Guid.NewGuid(), Description = "uniqueidentifier" },
				new MetaColumnType { metacolumntype_guid = Guid.NewGuid(), Description = "varbinary" },
				new MetaColumnType { metacolumntype_guid = Guid.NewGuid(), Description = "varchar" }
			};

            context.MetaColumnTypes.AddOrUpdate(mt => mt.Description, metaColumnTypes);
            context.SaveChanges();

            var metaWidgetTypes = new[]
			{
                new MetaWidgetType { metawidgettype_guid = Guid.NewGuid(), Description = "General" },
                new MetaWidgetType { metawidgettype_guid = Guid.NewGuid(), Description = "Wiki" },
                new MetaWidgetType { metawidgettype_guid = Guid.NewGuid(), Description = "ItemList" }
            };

            context.MetaWidgetTypes.AddOrUpdate(mt => mt.Description, metaWidgetTypes);
            context.SaveChanges();
        }

        private static void CreateConfigValues(PVIMSDbContext context)
        {
            var configs = new[]
			{
				new Config { ConfigType = ConfigType.E2BVersion, ConfigValue = "E2B(R2) ICH Report" },
				new Config { ConfigType = ConfigType.WebServiceSubscriberList, ConfigValue = "NOT SPECIFIED" },
				new Config { ConfigType = ConfigType.AssessmentScale, ConfigValue = "Both Scales" },
                new Config { ConfigType = ConfigType.MedDRAVersion, ConfigValue = "20.0" },
                new Config { ConfigType = ConfigType.ReportInstanceNewAlertCount, ConfigValue = "0" },
                new Config { ConfigType = ConfigType.MedicationOnsetCheckPeriodWeeks, ConfigValue = "5" },
                new Config { ConfigType = ConfigType.MetaDataLastUpdated, ConfigValue = DateTime.Now.ToString("yyyy-MM-dd HH:mm") }
            };

            context.Configs.AddOrUpdate(c => c.ConfigType, configs);
            context.SaveChanges();
        }

        private static void CreateContactTypes(PVIMSDbContext context)
        {
            var types = new[]
			{
                new SiteContactDetail { ContactType = ContactType.RegulatoryAuthority, ContactFirstName = "Not", ContactSurname = "Specified", StreetAddress = "None", City = "None", OrganisationName = "None" },
                new SiteContactDetail { ContactType = ContactType.ReportingAuthority, ContactFirstName = "Uppsala", ContactSurname = "Monitoring Centre", StreetAddress = "Bredgrand 7B", City = "Uppsala", State = "None", PostCode = "75320", ContactEmail = "info@who-umc.org", ContactNumber = "18656060", CountryCode = "46", OrganisationName = "UMC" }
            };

            context.SiteContactDetails.AddOrUpdate(c => c.ContactType, types);
            context.SaveChanges();
        }

        private static void CreateContextTypes(PVIMSDbContext context)
        {
            var contextTypes = new[]
			{
				new ContextType {Description = "Encounter"},
				new ContextType {Description = "Patient"},
				new ContextType {Description = "Pregnancy"},
                new ContextType {Description = "Global"},
                new ContextType {Description = "PatientClinicalEvent"},
                new ContextType {Description = "DatasetInstance"}
			};

            context.ContextTypes.AddOrUpdate(ct => ct.Description, contextTypes);
            context.SaveChanges();
        }

        private static void CreateEncounterTypes(PVIMSDbContext context)
        {
            var encounterTypes = new[]
			{
                new EncounterType { Description = "Pre-Treatment Visit", Help = "" },
                new EncounterType { Description = "Treatment Initiation Visit", Help = "" },
                new EncounterType { Description = "Scheduled Follow-Up Visit", Help = "" },
                new EncounterType { Description = "Unscheduled Visit", Help = "" }
			};

            context.EncounterTypes.AddOrUpdate(et => et.Description, encounterTypes);
            context.SaveChanges();
        }

        private static void CreatePostDeploymentScripts(PVIMSDbContext context)
        {
            var postDeployments = new[]
            {
               new PostDeployment { ScriptGuid = Guid.NewGuid(),   ScriptFileName = "dbo.spGenerateRiskFactors.sql", ScriptDescription="Analysis - Generate risk factors",RunRank = 1},
               new PostDeployment { ScriptGuid = Guid.NewGuid(),   ScriptFileName = "dbo.spGeneratePatientListCondition.sql", ScriptDescription="Analysis - Generate patient conditions",RunRank = 2},
               new PostDeployment { ScriptGuid = Guid.NewGuid(),   ScriptFileName = "dbo.spGeneratePatientListCohort.sql", ScriptDescription="Analysis - Generate patient cohorts",RunRank = 3},
               new PostDeployment { ScriptGuid = Guid.NewGuid(),   ScriptFileName = "dbo.spGenerateDrugList.sql", ScriptDescription="Analysis - Generate drug list",RunRank = 4},
               new PostDeployment { ScriptGuid = Guid.NewGuid(),   ScriptFileName = "dbo.spGenerateContingency.sql", ScriptDescription="Analysis - Generate contingency",RunRank = 5},
               new PostDeployment { ScriptGuid = Guid.NewGuid(),   ScriptFileName = "dbo.spGenerateAnalysis.sql", ScriptDescription="Analysis - Generate analysis",RunRank = 6},
               new PostDeployment { ScriptGuid = Guid.NewGuid(),   ScriptFileName = "dbo.CleanJsonForString.sql", ScriptDescription="JSON String Handler Function",RunRank = 7},
               new PostDeployment { ScriptGuid = Guid.NewGuid(),   ScriptFileName = "dbo.SeedMedications.sql", ScriptDescription="Seed Medications",RunRank = 8},
               new PostDeployment { ScriptGuid = Guid.NewGuid(),   ScriptFileName = "dbo.SeedMeta.sql", ScriptDescription="Seed META",RunRank = 9},
               new PostDeployment { ScriptGuid = Guid.NewGuid(),   ScriptFileName = "dbo.SeedMedDRA_1.sql", ScriptDescription="Seed MedDRA 10000",RunRank = 10},
               new PostDeployment { ScriptGuid = Guid.NewGuid(),   ScriptFileName = "dbo.SeedMedDRA_2.sql", ScriptDescription="Seed MedDRA 20000",RunRank = 11},
               new PostDeployment { ScriptGuid = Guid.NewGuid(),   ScriptFileName = "dbo.SeedMedDRA_3.sql", ScriptDescription="Seed MedDRA 30000",RunRank = 12},
               new PostDeployment { ScriptGuid = Guid.NewGuid(),   ScriptFileName = "dbo.SeedMedDRA_4.sql", ScriptDescription="Seed MedDRA 40000",RunRank = 13},
               new PostDeployment { ScriptGuid = Guid.NewGuid(),   ScriptFileName = "dbo.SeedMedDRA_5.sql", ScriptDescription="Seed MedDRA 50000",RunRank = 14},
               new PostDeployment { ScriptGuid = Guid.NewGuid(),   ScriptFileName = "dbo.SeedMedDRA_6.sql", ScriptDescription="Seed MedDRA 60000",RunRank = 15},
               new PostDeployment { ScriptGuid = Guid.NewGuid(),   ScriptFileName = "dbo.SeedMedDRA_7.sql", ScriptDescription="Seed MedDRA 70000",RunRank = 16},
               new PostDeployment { ScriptGuid = Guid.NewGuid(),   ScriptFileName = "dbo.SeedMedDRA_8.sql", ScriptDescription="Seed MedDRA 80000",RunRank = 17},
               new PostDeployment { ScriptGuid = Guid.NewGuid(),   ScriptFileName = "dbo.SeedMedDRA_9.sql", ScriptDescription="Seed MedDRA 90000",RunRank = 18},
               new PostDeployment { ScriptGuid = Guid.NewGuid(),   ScriptFileName = "dbo.SeedMedDRA_10.sql", ScriptDescription="Seed MedDRA 100000",RunRank = 19},
               new PostDeployment { ScriptGuid = Guid.NewGuid(),   ScriptFileName = "dbo.SeedConditions.sql", ScriptDescription="Seed Conditions",RunRank = 20},
               new PostDeployment { ScriptGuid = Guid.NewGuid(),   ScriptFileName = "dbo.Datapack.Chronic.sql", ScriptDescription="Seed Chronic Dataset",RunRank = 21},
               new PostDeployment { ScriptGuid = Guid.NewGuid(),   ScriptFileName = "dbo.Datapack.Spontaneous.sql", ScriptDescription="Seed Spontaneous Dataset",RunRank = 22},
               new PostDeployment { ScriptGuid = Guid.NewGuid(),   ScriptFileName = "dbo.Datapack.E2BR2.sql", ScriptDescription="Seed E2B R2 Dataset",RunRank = 23},
               new PostDeployment { ScriptGuid = Guid.NewGuid(),   ScriptFileName = "dbo.SeedWorkFlow.sql", ScriptDescription="Seed Work Flow Processes",RunRank = 24},
               new PostDeployment { ScriptGuid = Guid.NewGuid(),   ScriptFileName = "dbo.SeedRiskFactor.sql", ScriptDescription="Seed Risk Factors",RunRank = 25},
            };

            context.PostDeployments.AddOrUpdate(r => r.ScriptFileName, postDeployments);
            context.SaveChanges();
        }

        private static void CreateEncounterDatasetElements(PVIMSDbContext context)
        {
            var elementType = context.DatasetElementTypes.SingleOrDefault(u => u.Description == "Generic");

            // Weight (kg)
            var fieldType = context.FieldTypes.SingleOrDefault(u => u.Description == "NumericTextbox");
            var field = new Field() { Anonymise = false, Mandatory = false, FieldType = fieldType };

            field.Decimals = 1;
            field.MinSize = (decimal)1.1;
            field.MaxSize = (decimal)159.9;

            var element = new DatasetElement { System = false, DatasetElementType = elementType, ElementName = "Weight (kg)", Field = field };
            context.DatasetElements.Add(element);

            // Height (cm)
            fieldType = context.FieldTypes.SingleOrDefault(u => u.Description == "NumericTextbox");
            field = new Field() { Anonymise = false, Mandatory = false, FieldType = fieldType };

            field.Decimals = 0;
            field.MinSize = 1;
            field.MaxSize = 250;

            element = new DatasetElement { System = false, DatasetElementType = elementType, ElementName = "Height (cm)", Field = field };
            context.DatasetElements.Add(element);

            // BMI
            fieldType = context.FieldTypes.SingleOrDefault(u => u.Description == "NumericTextbox");
            field = new Field() { Anonymise = false, Mandatory = false, FieldType = fieldType };

            field.Decimals = 1;
            field.MinSize = 0;
            field.MaxSize = 60;

            element = new DatasetElement { System = true, DatasetElementType = elementType, ElementName = "BMI", Field = field };
            context.DatasetElements.Add(element);

            // Indication for Treatment (TB)
            fieldType = context.FieldTypes.SingleOrDefault(u => u.Description == "DropDownList");
            field = new Field() { Anonymise = false, Mandatory = false, FieldType = fieldType };

            var fieldValues = new[]
            {
                new FieldValue { Default = true, Field = field, Other = false, Unknown = false, Value = "Pulmonary TB" },
                new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "Extra-pulmonary TB" },
                new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "MDR-TB" },
                new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "Prophylaxis" }
            };
            context.FieldValues.AddRange(fieldValues);

            element = new DatasetElement { System = false, DatasetElementType = elementType, ElementName = "Indication for Treatment (TB)", Field = field };
            context.DatasetElements.Add(element);

            // Previous TB treatment?
            fieldType = context.FieldTypes.SingleOrDefault(u => u.Description == "DropDownList");
            field = new Field() { Anonymise = false, Mandatory = false, FieldType = fieldType };

            fieldValues = new[]
            {
                new FieldValue { Default = true, Field = field, Other = false, Unknown = false, Value = "Yes" },
                new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "No" },
                new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "Unknown" }
            };
            context.FieldValues.AddRange(fieldValues);

            element = new DatasetElement { System = false, DatasetElementType = elementType, ElementName = "Previous TB treatment?", Field = field };
            context.DatasetElements.Add(element);

            // Pregnant 
            fieldType = context.FieldTypes.SingleOrDefault(u => u.Description == "DropDownList");
            field = new Field() { Anonymise = false, Mandatory = false, FieldType = fieldType };

            fieldValues = new[]
            {
                new FieldValue { Default = true, Field = field, Other = false, Unknown = false, Value = "Yes" },
                new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "No" },
                new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "NA" },
                new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "Uncertain" }
            };
            context.FieldValues.AddRange(fieldValues);

            element = new DatasetElement { System = false, DatasetElementType = elementType, ElementName = "Pregnancy Status", Field = field };
            context.DatasetElements.Add(element);

            // Date of last LMP 
            fieldType = context.FieldTypes.SingleOrDefault(u => u.Description == "Date");
            field = new Field() { Anonymise = false, Mandatory = false, FieldType = fieldType };
            element = new DatasetElement { System = false, DatasetElementType = elementType, ElementName = "Date of last menstrual period", Field = field };
            context.DatasetElements.Add(element);

            // Estimated gestation (weeks)
            fieldType = context.FieldTypes.SingleOrDefault(u => u.Description == "NumericTextbox");
            field = new Field() { Anonymise = false, Mandatory = false, FieldType = fieldType };

            field.Decimals = 0;
            field.MinSize = 1;
            field.MaxSize = 44;

            element = new DatasetElement { System = false, DatasetElementType = elementType, ElementName = "Estimated gestation (weeks)", Field = field };
            context.DatasetElements.Add(element);

            // Breastfeeding mother
            fieldType = context.FieldTypes.SingleOrDefault(u => u.Description == "DropDownList");
            field = new Field() { Anonymise = false, Mandatory = false, FieldType = fieldType };

            fieldValues = new[]
            {
                new FieldValue { Default = true, Field = field, Other = false, Unknown = false, Value = "Yes" },
                new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "No" },
                new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "NA" },
                new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "Unknown" }
            };
            context.FieldValues.AddRange(fieldValues);

            element = new DatasetElement { System = false, DatasetElementType = elementType, ElementName = "Breastfeeding mother", Field = field };
            context.DatasetElements.Add(element);

            // Cavities on baseline chest x-ray
            fieldType = context.FieldTypes.SingleOrDefault(u => u.Description == "DropDownList");
            field = new Field() { Anonymise = false, Mandatory = false, FieldType = fieldType };

            fieldValues = new[]
            {
                new FieldValue { Default = true, Field = field, Other = false, Unknown = false, Value = "Yes" },
                new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "No" },
                new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "Unknown" }
            };
            context.FieldValues.AddRange(fieldValues);

            element = new DatasetElement { System = false, DatasetElementType = elementType, ElementName = "Cavities on baseline chest x-ray", Field = field };
            context.DatasetElements.Add(element);

            // Site of TB
            fieldType = context.FieldTypes.SingleOrDefault(u => u.Description == "DropDownList");
            field = new Field() { Anonymise = false, Mandatory = false, FieldType = fieldType };

            fieldValues = new[]
            {
                new FieldValue { Default = true, Field = field, Other = false, Unknown = false, Value = "PTB only" },
                new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "PTB+EPTB" },
                new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "EPTB only" },
                new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "Unknown" }
            };
            context.FieldValues.AddRange(fieldValues);

            element = new DatasetElement { System = false, DatasetElementType = elementType, ElementName = "Site of TB", Field = field };
            context.DatasetElements.Add(element);

            // Extrapulmonary TB site
            fieldType = context.FieldTypes.SingleOrDefault(u => u.Description == "DropDownList");
            field = new Field() { Anonymise = false, Mandatory = false, FieldType = fieldType };

            fieldValues = new[]
            {
                new FieldValue { Default = true, Field = field, Other = false, Unknown = false, Value = "Pleural" },
                new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "Lymphatic, intrathoracic" },
                new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "Lymphatic, extrathoracic" },
                new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "Genito-urinary" },
                new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "Osteo-articular" },
                new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "Disseminated" },
                new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "Peritoneal & Digestive" },
                new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "Central nervous system" },
                new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "Other" }
            };
            context.FieldValues.AddRange(fieldValues);

            element = new DatasetElement { System = false, DatasetElementType = elementType, ElementName = "Extrapulmonary TB site", Field = field };
            context.DatasetElements.Add(element);

            // Injecting Drug Use Within Past Year
            fieldType = context.FieldTypes.SingleOrDefault(u => u.Description == "DropDownList");
            field = new Field() { Anonymise = false, Mandatory = false, FieldType = fieldType };

            fieldValues = new[]
            {
                new FieldValue { Default = true, Field = field, Other = false, Unknown = false, Value = "Yes" },
                new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "No" },
                new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "Unknown" }
            };
            context.FieldValues.AddRange(fieldValues);

            element = new DatasetElement { System = false, DatasetElementType = elementType, ElementName = "Injecting Drug Use Within Past Year", Field = field };
            context.DatasetElements.Add(element);

            // Excessive alcohol use in the past year
            fieldType = context.FieldTypes.SingleOrDefault(u => u.Description == "DropDownList");
            field = new Field() { Anonymise = false, Mandatory = false, FieldType = fieldType };

            fieldValues = new[]
            {
                new FieldValue { Default = true, Field = field, Other = false, Unknown = false, Value = "Yes" },
                new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "No" },
                new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "Unknown" }
            };
            context.FieldValues.AddRange(fieldValues);

            element = new DatasetElement { System = false, DatasetElementType = elementType, ElementName = "Excessive alcohol use within the past year", Field = field };
            context.DatasetElements.Add(element);

            // Tobacco use within the past year
            fieldType = context.FieldTypes.SingleOrDefault(u => u.Description == "DropDownList");
            field = new Field() { Anonymise = false, Mandatory = false, FieldType = fieldType };

            fieldValues = new[]
            {
                new FieldValue { Default = true, Field = field, Other = false, Unknown = false, Value = "Yes" },
                new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "No" },
                new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "Unknown" }
            };
            context.FieldValues.AddRange(fieldValues);

            element = new DatasetElement { System = false, DatasetElementType = elementType, ElementName = "Tobacco use within the past year", Field = field };
            context.DatasetElements.Add(element);

            // Documented HIV infection
            fieldType = context.FieldTypes.SingleOrDefault(u => u.Description == "DropDownList");
            field = new Field() { Anonymise = false, Mandatory = false, FieldType = fieldType };

            fieldValues = new[]
            {
                new FieldValue { Default = true, Field = field, Other = false, Unknown = false, Value = "Yes" },
                new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "No" },
                new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "Unknown" }
            };
            context.FieldValues.AddRange(fieldValues);

            element = new DatasetElement { System = false, DatasetElementType = elementType, ElementName = "Documented HIV infection", Field = field };
            context.DatasetElements.Add(element);

            // End of treatment episode
            fieldType = context.FieldTypes.SingleOrDefault(u => u.Description == "YesNo");
            field = new Field() { Anonymise = false, Mandatory = false, FieldType = fieldType };
            element = new DatasetElement { System = false, DatasetElementType = elementType, ElementName = "End of treatment episode", Field = field };
            context.DatasetElements.Add(element);

            // Outcome
            fieldType = context.FieldTypes.SingleOrDefault(u => u.Description == "DropDownList");
            field = new Field() { Anonymise = false, Mandatory = false, FieldType = fieldType };

            fieldValues = new[]
            {
                new FieldValue { Default = true, Field = field, Other = false, Unknown = false, Value = "Cured" },
                new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "Completed" },
                new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "Treatment failed" },
                new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "Died" },
                new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "Loss to follow up" },
                new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "Not evaluated" }
            };
            context.FieldValues.AddRange(fieldValues);

            element = new DatasetElement { System = false, DatasetElementType = elementType, ElementName = "Outcome", Field = field };
            context.DatasetElements.Add(element);

            // Treatment outcome date
            fieldType = context.FieldTypes.SingleOrDefault(u => u.Description == "Date");
            field = new Field() { Anonymise = false, Mandatory = false, FieldType = fieldType };
            element = new DatasetElement { System = false, DatasetElementType = elementType, ElementName = "Treatment outcome date", Field = field };
            context.DatasetElements.Add(element);

            // Isoniazid susceptibility
            fieldType = context.FieldTypes.SingleOrDefault(u => u.Description == "DropDownList");
            field = new Field() { Anonymise = false, Mandatory = false, FieldType = fieldType };

            fieldValues = new[]
            {
                new FieldValue { Default = true, Field = field, Other = false, Unknown = false, Value = "Susceptible" },
                new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "Resistant" },
                new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "Indeterminate" },
                new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "Unknown" }
            };
            context.FieldValues.AddRange(fieldValues);

            element = new DatasetElement { System = false, DatasetElementType = elementType, ElementName = "Isoniazid susceptibility by any laboratory test(s)", Field = field };
            context.DatasetElements.Add(element);

            // Isoniazid confirmation
            fieldType = context.FieldTypes.SingleOrDefault(u => u.Description == "DropDownList");
            field = new Field() { Anonymise = false, Mandatory = false, FieldType = fieldType };

            fieldValues = new[]
            {
                new FieldValue { Default = true, Field = field, Other = false, Unknown = false, Value = "Xpert" },
                new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "LPA" },
                new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "Unknown" }
            };
            context.FieldValues.AddRange(fieldValues);

            element = new DatasetElement { System = false, DatasetElementType = elementType, ElementName = "Isoniazid confirmation", Field = field };
            context.DatasetElements.Add(element);

            // Rifampicin susceptibility
            fieldType = context.FieldTypes.SingleOrDefault(u => u.Description == "DropDownList");
            field = new Field() { Anonymise = false, Mandatory = false, FieldType = fieldType };

            fieldValues = new[]
            {
                new FieldValue { Default = true, Field = field, Other = false, Unknown = false, Value = "Susceptible" },
                new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "Resistant" },
                new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "Indeterminate" },
                new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "Unknown" }
            };
            context.FieldValues.AddRange(fieldValues);

            element = new DatasetElement { System = false, DatasetElementType = elementType, ElementName = "Rifampicin susceptibility by any laboratory test(s)", Field = field };
            context.DatasetElements.Add(element);

            // Rifampicin confirmation
            fieldType = context.FieldTypes.SingleOrDefault(u => u.Description == "DropDownList");
            field = new Field() { Anonymise = false, Mandatory = false, FieldType = fieldType };

            fieldValues = new[]
            {
                new FieldValue { Default = true, Field = field, Other = false, Unknown = false, Value = "Xpert" },
                new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "LPA" },
                new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "Unknown" }
            };
            context.FieldValues.AddRange(fieldValues);

            element = new DatasetElement { System = false, DatasetElementType = elementType, ElementName = "Rifampicin confirmation", Field = field };
            context.DatasetElements.Add(element);

            // Kanamycin susceptibility
            fieldType = context.FieldTypes.SingleOrDefault(u => u.Description == "DropDownList");
            field = new Field() { Anonymise = false, Mandatory = false, FieldType = fieldType };

            fieldValues = new[]
            {
                new FieldValue { Default = true, Field = field, Other = false, Unknown = false, Value = "Susceptible" },
                new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "Resistant" },
                new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "Indeterminate" },
                new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "Unknown" }
            };
            context.FieldValues.AddRange(fieldValues);

            element = new DatasetElement { System = false, DatasetElementType = elementType, ElementName = "Kanamycin susceptibility by any laboratory test(s)", Field = field };
            context.DatasetElements.Add(element);

            // Kanamycin confirmation
            fieldType = context.FieldTypes.SingleOrDefault(u => u.Description == "DropDownList");
            field = new Field() { Anonymise = false, Mandatory = false, FieldType = fieldType };

            fieldValues = new[]
            {
                new FieldValue { Default = true, Field = field, Other = false, Unknown = false, Value = "Xpert" },
                new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "LPA" },
                new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "Unknown" }
            };
            context.FieldValues.AddRange(fieldValues);

            element = new DatasetElement { System = false, DatasetElementType = elementType, ElementName = "Kanamycin confirmation", Field = field };
            context.DatasetElements.Add(element);

            // Amikacin susceptibility
            fieldType = context.FieldTypes.SingleOrDefault(u => u.Description == "DropDownList");
            field = new Field() { Anonymise = false, Mandatory = false, FieldType = fieldType };

            fieldValues = new[]
            {
                new FieldValue { Default = true, Field = field, Other = false, Unknown = false, Value = "Susceptible" },
                new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "Resistant" },
                new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "Indeterminate" },
                new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "Unknown" }
            };
            context.FieldValues.AddRange(fieldValues);

            element = new DatasetElement { System = false, DatasetElementType = elementType, ElementName = "Amikacin susceptibility by any laboratory test(s)", Field = field };
            context.DatasetElements.Add(element);

            // Amikacin confirmation
            fieldType = context.FieldTypes.SingleOrDefault(u => u.Description == "DropDownList");
            field = new Field() { Anonymise = false, Mandatory = false, FieldType = fieldType };

            fieldValues = new[]
            {
                new FieldValue { Default = true, Field = field, Other = false, Unknown = false, Value = "Xpert" },
                new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "LPA" },
                new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "Unknown" }
            };
            context.FieldValues.AddRange(fieldValues);

            element = new DatasetElement { System = false, DatasetElementType = elementType, ElementName = "Amikacin confirmation", Field = field };
            context.DatasetElements.Add(element);

            // Capreomycin susceptibility
            fieldType = context.FieldTypes.SingleOrDefault(u => u.Description == "DropDownList");
            field = new Field() { Anonymise = false, Mandatory = false, FieldType = fieldType };

            fieldValues = new[]
            {
                new FieldValue { Default = true, Field = field, Other = false, Unknown = false, Value = "Susceptible" },
                new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "Resistant" },
                new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "Indeterminate" },
                new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "Unknown" }
            };
            context.FieldValues.AddRange(fieldValues);

            element = new DatasetElement { System = false, DatasetElementType = elementType, ElementName = "Capreomycin susceptibility by any laboratory test(s)", Field = field };
            context.DatasetElements.Add(element);

            // Capreomycin confirmation
            fieldType = context.FieldTypes.SingleOrDefault(u => u.Description == "DropDownList");
            field = new Field() { Anonymise = false, Mandatory = false, FieldType = fieldType };

            fieldValues = new[]
            {
                new FieldValue { Default = true, Field = field, Other = false, Unknown = false, Value = "Xpert" },
                new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "LPA" },
                new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "Unknown" }
            };
            context.FieldValues.AddRange(fieldValues);

            element = new DatasetElement { System = false, DatasetElementType = elementType, ElementName = "Capreomycin confirmation", Field = field };
            context.DatasetElements.Add(element);

            // Ciprofloxacin susceptibility
            fieldType = context.FieldTypes.SingleOrDefault(u => u.Description == "DropDownList");
            field = new Field() { Anonymise = false, Mandatory = false, FieldType = fieldType };

            fieldValues = new[]
            {
                new FieldValue { Default = true, Field = field, Other = false, Unknown = false, Value = "Susceptible" },
                new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "Resistant" },
                new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "Indeterminate" },
                new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "Unknown" }
            };
            context.FieldValues.AddRange(fieldValues);

            element = new DatasetElement { System = false, DatasetElementType = elementType, ElementName = "Ciprofloxacin susceptibility by any laboratory test(s)", Field = field };
            context.DatasetElements.Add(element);

            // Ciprofloxacin confirmation
            fieldType = context.FieldTypes.SingleOrDefault(u => u.Description == "DropDownList");
            field = new Field() { Anonymise = false, Mandatory = false, FieldType = fieldType };

            fieldValues = new[]
            {
                new FieldValue { Default = true, Field = field, Other = false, Unknown = false, Value = "Xpert" },
                new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "LPA" },
                new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "Unknown" }
            };
            context.FieldValues.AddRange(fieldValues);

            element = new DatasetElement { System = false, DatasetElementType = elementType, ElementName = "Ciprofloxacin confirmation", Field = field };
            context.DatasetElements.Add(element);

            // Ofloxacin susceptibility
            fieldType = context.FieldTypes.SingleOrDefault(u => u.Description == "DropDownList");
            field = new Field() { Anonymise = false, Mandatory = false, FieldType = fieldType };

            fieldValues = new[]
            {
                new FieldValue { Default = true, Field = field, Other = false, Unknown = false, Value = "Susceptible" },
                new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "Resistant" },
                new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "Indeterminate" },
                new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "Unknown" }
            };
            context.FieldValues.AddRange(fieldValues);

            element = new DatasetElement { System = false, DatasetElementType = elementType, ElementName = "Ofloxacin susceptibility by any laboratory test(s)", Field = field };
            context.DatasetElements.Add(element);

            // Ofloxacin confirmation
            fieldType = context.FieldTypes.SingleOrDefault(u => u.Description == "DropDownList");
            field = new Field() { Anonymise = false, Mandatory = false, FieldType = fieldType };

            fieldValues = new[]
            {
                new FieldValue { Default = true, Field = field, Other = false, Unknown = false, Value = "Xpert" },
                new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "LPA" },
                new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "Unknown" }
            };
            context.FieldValues.AddRange(fieldValues);

            element = new DatasetElement { System = false, DatasetElementType = elementType, ElementName = "Ofloxacin confirmation", Field = field };
            context.DatasetElements.Add(element);

            // Levofloxacin susceptibility
            fieldType = context.FieldTypes.SingleOrDefault(u => u.Description == "DropDownList");
            field = new Field() { Anonymise = false, Mandatory = false, FieldType = fieldType };

            fieldValues = new[]
            {
                new FieldValue { Default = true, Field = field, Other = false, Unknown = false, Value = "Susceptible" },
                new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "Resistant" },
                new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "Indeterminate" },
                new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "Unknown" }
            };
            context.FieldValues.AddRange(fieldValues);

            element = new DatasetElement { System = false, DatasetElementType = elementType, ElementName = "Levofloxacin susceptibility by any laboratory test(s)", Field = field };
            context.DatasetElements.Add(element);

            // Levofloxacin confirmation
            fieldType = context.FieldTypes.SingleOrDefault(u => u.Description == "DropDownList");
            field = new Field() { Anonymise = false, Mandatory = false, FieldType = fieldType };

            fieldValues = new[]
            {
                new FieldValue { Default = true, Field = field, Other = false, Unknown = false, Value = "Xpert" },
                new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "LPA" },
                new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "Unknown" }
            };
            context.FieldValues.AddRange(fieldValues);

            element = new DatasetElement { System = false, DatasetElementType = elementType, ElementName = "Levofloxacin confirmation", Field = field };
            context.DatasetElements.Add(element);

            // Moxifloxacin susceptibility
            fieldType = context.FieldTypes.SingleOrDefault(u => u.Description == "DropDownList");
            field = new Field() { Anonymise = false, Mandatory = false, FieldType = fieldType };

            fieldValues = new[]
            {
                new FieldValue { Default = true, Field = field, Other = false, Unknown = false, Value = "Susceptible" },
                new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "Resistant" },
                new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "Indeterminate" },
                new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "Unknown" }
            };
            context.FieldValues.AddRange(fieldValues);

            element = new DatasetElement { System = false, DatasetElementType = elementType, ElementName = "Moxifloxacin susceptibility by any laboratory test(s)", Field = field };
            context.DatasetElements.Add(element);

            // Moxifloxacin confirmation
            fieldType = context.FieldTypes.SingleOrDefault(u => u.Description == "DropDownList");
            field = new Field() { Anonymise = false, Mandatory = false, FieldType = fieldType };

            fieldValues = new[]
            {
                new FieldValue { Default = true, Field = field, Other = false, Unknown = false, Value = "Xpert" },
                new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "LPA" },
                new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "Unknown" }
            };
            context.FieldValues.AddRange(fieldValues);

            element = new DatasetElement { System = false, DatasetElementType = elementType, ElementName = "Moxifloxacin confirmation", Field = field };
            context.DatasetElements.Add(element);

            context.SaveChanges();
        }

        private static void CreateInitiationDataset(PVIMSDbContext context)
        {
            var contextType = context.ContextTypes.SingleOrDefault(u => u.Description == "Encounter");
            var user = context.Users.SingleOrDefault(u => u.UserName == "Admin");

            // Dataset
            var dataset = new Dataset { Active = true, ContextType = contextType, DatasetName = "Treatment Initiation", Help = "Treatment Initiation Form - CEM TB Drugs (page 451 of WHO companion workbook)" };
            context.Datasets.Add(dataset);

            // Categories
            var datasetCategory = new DatasetCategory { CategoryOrder = 1, Dataset = dataset, DatasetCategoryName = "Medical Details" };
            context.DatasetCategories.Add(datasetCategory);

            // Link to elements
            var datasetElement = context.DatasetElements.SingleOrDefault(d => d.ElementName == "Weight (kg)");
            var datasetCategoryElement = new DatasetCategoryElement { DatasetCategory = datasetCategory, DatasetElement = datasetElement, FieldOrder = 1 };
            context.DatasetCategoryElements.Add(datasetCategoryElement);

            datasetElement = context.DatasetElements.SingleOrDefault(d => d.ElementName == "Height (cm)");
            datasetCategoryElement = new DatasetCategoryElement { DatasetCategory = datasetCategory, DatasetElement = datasetElement, FieldOrder = 2 };
            context.DatasetCategoryElements.Add(datasetCategoryElement);

            datasetElement = context.DatasetElements.SingleOrDefault(d => d.ElementName == "Indication for Treatment (TB)");
            datasetCategoryElement = new DatasetCategoryElement { DatasetCategory = datasetCategory, DatasetElement = datasetElement, FieldOrder = 3 };
            context.DatasetCategoryElements.Add(datasetCategoryElement);

            datasetElement = context.DatasetElements.SingleOrDefault(d => d.ElementName == "Prior exposure to anti medicines (TB)");
            datasetCategoryElement = new DatasetCategoryElement { DatasetCategory = datasetCategory, DatasetElement = datasetElement, FieldOrder = 4 };
            context.DatasetCategoryElements.Add(datasetCategoryElement);

            datasetElement = context.DatasetElements.SingleOrDefault(d => d.ElementName == "Pregnant");
            datasetCategoryElement = new DatasetCategoryElement { DatasetCategory = datasetCategory, DatasetElement = datasetElement, FieldOrder = 5 };
            context.DatasetCategoryElements.Add(datasetCategoryElement);

            datasetElement = context.DatasetElements.SingleOrDefault(d => d.ElementName == "Date of last LMP");
            datasetCategoryElement = new DatasetCategoryElement { DatasetCategory = datasetCategory, DatasetElement = datasetElement, FieldOrder = 6 };
            context.DatasetCategoryElements.Add(datasetCategoryElement);

            datasetElement = context.DatasetElements.SingleOrDefault(d => d.ElementName == "Estimated gestation (weeks)");
            datasetCategoryElement = new DatasetCategoryElement { DatasetCategory = datasetCategory, DatasetElement = datasetElement, FieldOrder = 7 };
            context.DatasetCategoryElements.Add(datasetCategoryElement);

            datasetElement = context.DatasetElements.SingleOrDefault(d => d.ElementName == "Breastfeeding an infant");
            datasetCategoryElement = new DatasetCategoryElement { DatasetCategory = datasetCategory, DatasetElement = datasetElement, FieldOrder = 8 };
            context.DatasetCategoryElements.Add(datasetCategoryElement);

            datasetElement = context.DatasetElements.SingleOrDefault(d => d.ElementName == "Injecting Drug Use Within Past Year");
            datasetCategoryElement = new DatasetCategoryElement { DatasetCategory = datasetCategory, DatasetElement = datasetElement, FieldOrder = 9 };
            context.DatasetCategoryElements.Add(datasetCategoryElement);

            datasetElement = context.DatasetElements.SingleOrDefault(d => d.ElementName == "Excessive alcohol use in the past year");
            datasetCategoryElement = new DatasetCategoryElement { DatasetCategory = datasetCategory, DatasetElement = datasetElement, FieldOrder = 10 };
            context.DatasetCategoryElements.Add(datasetCategoryElement);

            datasetElement = context.DatasetElements.SingleOrDefault(d => d.ElementName == "Tobacco use within the past year");
            datasetCategoryElement = new DatasetCategoryElement { DatasetCategory = datasetCategory, DatasetElement = datasetElement, FieldOrder = 11 };
            context.DatasetCategoryElements.Add(datasetCategoryElement);

            datasetElement = context.DatasetElements.SingleOrDefault(d => d.ElementName == "Documented HIV infection");
            datasetCategoryElement = new DatasetCategoryElement { DatasetCategory = datasetCategory, DatasetElement = datasetElement, FieldOrder = 12 };
            context.DatasetCategoryElements.Add(datasetCategoryElement);

            context.SaveChanges();
        }

        private static void CreateReviewDataset(PVIMSDbContext context)
        {
            var contextType = context.ContextTypes.SingleOrDefault(u => u.Description == "Encounter");
            var user = context.Users.SingleOrDefault(u => u.UserName == "Admin");

            // Dataset
            var dataset = new Dataset { Active = true, ContextType = contextType, DatasetName = "Treatment Initiation", Help = "Treatment Review Form - CEM TB Drugs (page 455 of WHO companion workbook)" };
            context.Datasets.Add(dataset);

            // Categories
            var datasetCategory = new DatasetCategory { CategoryOrder = 1, Dataset = dataset, DatasetCategoryName = "Medical Details" };
            context.DatasetCategories.Add(datasetCategory);

            // Link to elements
            var datasetElement = context.DatasetElements.SingleOrDefault(d => d.ElementName == "Weight (kg)");
            var datasetCategoryElement = new DatasetCategoryElement { DatasetCategory = datasetCategory, DatasetElement = datasetElement, FieldOrder = 1 };
            context.DatasetCategoryElements.Add(datasetCategoryElement);

            datasetElement = context.DatasetElements.SingleOrDefault(d => d.ElementName == "Height (cm)");
            datasetCategoryElement = new DatasetCategoryElement { DatasetCategory = datasetCategory, DatasetElement = datasetElement, FieldOrder = 2 };
            context.DatasetCategoryElements.Add(datasetCategoryElement);

            datasetElement = context.DatasetElements.SingleOrDefault(d => d.ElementName == "Indication for Treatment (TB)");
            datasetCategoryElement = new DatasetCategoryElement { DatasetCategory = datasetCategory, DatasetElement = datasetElement, FieldOrder = 3 };
            context.DatasetCategoryElements.Add(datasetCategoryElement);

            datasetElement = context.DatasetElements.SingleOrDefault(d => d.ElementName == "Prior exposure to anti medicines (TB)");
            datasetCategoryElement = new DatasetCategoryElement { DatasetCategory = datasetCategory, DatasetElement = datasetElement, FieldOrder = 4 };
            context.DatasetCategoryElements.Add(datasetCategoryElement);

            datasetElement = context.DatasetElements.SingleOrDefault(d => d.ElementName == "Pregnant");
            datasetCategoryElement = new DatasetCategoryElement { DatasetCategory = datasetCategory, DatasetElement = datasetElement, FieldOrder = 5 };
            context.DatasetCategoryElements.Add(datasetCategoryElement);

            datasetElement = context.DatasetElements.SingleOrDefault(d => d.ElementName == "Date of last LMP");
            datasetCategoryElement = new DatasetCategoryElement { DatasetCategory = datasetCategory, DatasetElement = datasetElement, FieldOrder = 6 };
            context.DatasetCategoryElements.Add(datasetCategoryElement);

            datasetElement = context.DatasetElements.SingleOrDefault(d => d.ElementName == "Estimated gestation (weeks)");
            datasetCategoryElement = new DatasetCategoryElement { DatasetCategory = datasetCategory, DatasetElement = datasetElement, FieldOrder = 7 };
            context.DatasetCategoryElements.Add(datasetCategoryElement);

            datasetElement = context.DatasetElements.SingleOrDefault(d => d.ElementName == "Breastfeeding an infant");
            datasetCategoryElement = new DatasetCategoryElement { DatasetCategory = datasetCategory, DatasetElement = datasetElement, FieldOrder = 8 };
            context.DatasetCategoryElements.Add(datasetCategoryElement);

            datasetElement = context.DatasetElements.SingleOrDefault(d => d.ElementName == "Injecting Drug Use Within Past Year");
            datasetCategoryElement = new DatasetCategoryElement { DatasetCategory = datasetCategory, DatasetElement = datasetElement, FieldOrder = 9 };
            context.DatasetCategoryElements.Add(datasetCategoryElement);

            datasetElement = context.DatasetElements.SingleOrDefault(d => d.ElementName == "Excessive alcohol use in the past year");
            datasetCategoryElement = new DatasetCategoryElement { DatasetCategory = datasetCategory, DatasetElement = datasetElement, FieldOrder = 10 };
            context.DatasetCategoryElements.Add(datasetCategoryElement);

            datasetElement = context.DatasetElements.SingleOrDefault(d => d.ElementName == "Tobacco use within the past year");
            datasetCategoryElement = new DatasetCategoryElement { DatasetCategory = datasetCategory, DatasetElement = datasetElement, FieldOrder = 11 };
            context.DatasetCategoryElements.Add(datasetCategoryElement);

            datasetElement = context.DatasetElements.SingleOrDefault(d => d.ElementName == "Documented HIV infection");
            datasetCategoryElement = new DatasetCategoryElement { DatasetCategory = datasetCategory, DatasetElement = datasetElement, FieldOrder = 12 };
            context.DatasetCategoryElements.Add(datasetCategoryElement);

            datasetElement = context.DatasetElements.SingleOrDefault(d => d.ElementName == "End of treatment episode");
            datasetCategoryElement = new DatasetCategoryElement { DatasetCategory = datasetCategory, DatasetElement = datasetElement, FieldOrder = 13 };
            context.DatasetCategoryElements.Add(datasetCategoryElement);

            datasetElement = context.DatasetElements.SingleOrDefault(d => d.ElementName == "Outcome");
            datasetCategoryElement = new DatasetCategoryElement { DatasetCategory = datasetCategory, DatasetElement = datasetElement, FieldOrder = 14 };
            context.DatasetCategoryElements.Add(datasetCategoryElement);

            datasetElement = context.DatasetElements.SingleOrDefault(d => d.ElementName == "Treatment outcome date");
            datasetCategoryElement = new DatasetCategoryElement { DatasetCategory = datasetCategory, DatasetElement = datasetElement, FieldOrder = 15 };
            context.DatasetCategoryElements.Add(datasetCategoryElement);

            context.SaveChanges();
        }

        private static void CreateGlobalDatasetElements(PVIMSDbContext context)
        {
            var elementType = context.DatasetElementTypes.SingleOrDefault(u => u.Description == "Generic");

            FieldType fieldType;
            Field field;
            DatasetElement element;

            //// Initials
            //fieldType = context.FieldTypes.SingleOrDefault(u => u.Description == "AlphaNumericTextbox");
            //field = new Field() { Anonymise = false, Mandatory = false, FieldType = fieldType };
            //field.MaxLength = 5;

            //element = new DatasetElement { System = false, DatasetElementType = elementType, ElementName = "Initials", Field = field };
            //context.DatasetElements.Add(element);

            //// Date of Birth
            //fieldType = context.FieldTypes.SingleOrDefault(u => u.Description == "Date");
            //field = new Field() { Anonymise = false, Mandatory = false, FieldType = fieldType };

            //element = new DatasetElement { System = false, DatasetElementType = elementType, ElementName = "Date of Birth", Field = field };
            //context.DatasetElements.Add(element);

            //// Age
            //fieldType = context.FieldTypes.SingleOrDefault(u => u.Description == "NumericTextbox");
            //field = new Field() { Anonymise = false, Mandatory = false, FieldType = fieldType };

            //field.Decimals = 0;
            //field.MinSize = 0;
            //field.MaxSize = 140;

            //element = new DatasetElement { System = false, DatasetElementType = elementType, ElementName = "Age", Field = field };
            //context.DatasetElements.Add(element);

            //// Age unit
            //fieldType = context.FieldTypes.SingleOrDefault(u => u.Description == "DropDownList");
            //field = new Field() { Anonymise = false, Mandatory = false, FieldType = fieldType };

            //var fieldValues = new[]
            //{
            //    new FieldValue { Default = true, Field = field, Other = false, Unknown = false, Value = "Years" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "Months" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "Weeks" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "Days" }
            //};
            //context.FieldValues.AddRange(fieldValues);

            //element = new DatasetElement { System = false, DatasetElementType = elementType, ElementName = "Age Unit", Field = field };
            //context.DatasetElements.Add(element);

            //// Weight (encounter element - ALREADY DEFINED)

            //// Sex
            //fieldType = context.FieldTypes.SingleOrDefault(u => u.Description == "DropDownList");
            //field = new Field() { Anonymise = false, Mandatory = false, FieldType = fieldType };

            //fieldValues = new[]
            //{
            //    new FieldValue { Default = true, Field = field, Other = false, Unknown = false, Value = "Male" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "Female" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "Unknown" }
            //};
            //context.FieldValues.AddRange(fieldValues);

            //element = new DatasetElement { System = false, DatasetElementType = elementType, ElementName = "Sex", Field = field };
            //context.DatasetElements.Add(element);

            //// Ethnic Group 
            //fieldType = context.FieldTypes.SingleOrDefault(u => u.Description == "DropDownList");
            //field = new Field() { Anonymise = false, Mandatory = false, FieldType = fieldType };

            //fieldValues = new[]
            //{
            //    new FieldValue { Default = true, Field = field, Other = false, Unknown = false, Value = "Asian" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "East Asian" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "South Asian" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "Southeast Asian" },
            //    new FieldValue { Default = false, Field = field, Other = true, Unknown = false, Value = "Black" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = true, Value = "White" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = true, Value = "Middle Eastern" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = true, Value = "Other" }
            //};
            //context.FieldValues.AddRange(fieldValues);

            //element = new DatasetElement { System = false, DatasetElementType = elementType, ElementName = "Ethnic Group", Field = field };
            //context.DatasetElements.Add(element);

            //// Name
            //fieldType = context.FieldTypes.SingleOrDefault(u => u.Description == "AlphaNumericTextbox");
            //field = new Field() { Anonymise = false, Mandatory = false, FieldType = fieldType };
            //field.MaxLength = 30;

            //element = new DatasetElement { System = false, DatasetElementType = elementType, ElementName = "Name", Field = field };
            //context.DatasetElements.Add(element);

            //// Identification Type
            //fieldType = context.FieldTypes.SingleOrDefault(u => u.Description == "DropDownList");
            //field = new Field() { Anonymise = false, Mandatory = false, FieldType = fieldType };

            //fieldValues = new[]
            //{
            //    new FieldValue { Default = true, Field = field, Other = false, Unknown = false, Value = "National Identity" },
            //    new FieldValue { Default = false, Field = field, Other = true, Unknown = false, Value = "Other" }
            //};
            //context.FieldValues.AddRange(fieldValues);

            //element = new DatasetElement { System = false, DatasetElementType = elementType, ElementName = "Identification Type", Field = field };
            //context.DatasetElements.Add(element);

            //// Identification Number
            //fieldType = context.FieldTypes.SingleOrDefault(u => u.Description == "AlphaNumericTextbox");
            //field = new Field() { Anonymise = false, Mandatory = false, FieldType = fieldType };
            //field.MaxLength = 30;

            //element = new DatasetElement { System = false, DatasetElementType = elementType, ElementName = "Identification Number", Field = field };
            //context.DatasetElements.Add(element);

            //// Date of Onset 
            //fieldType = context.FieldTypes.SingleOrDefault(u => u.Description == "Date");
            //field = new Field() { Anonymise = false, Mandatory = true, FieldType = fieldType };
            //element = new DatasetElement { System = false, DatasetElementType = elementType, ElementName = "Date of Onset", Field = field };
            //context.DatasetElements.Add(element);

            //// Date of Onset Estimated
            //fieldType = context.FieldTypes.SingleOrDefault(u => u.Description == "YesNo");
            //field = new Field() { Anonymise = false, Mandatory = false, FieldType = fieldType };
            //element = new DatasetElement { System = false, DatasetElementType = elementType, ElementName = "Date of Onset Estimated", Field = field };
            //context.DatasetElements.Add(element);

            //// ADR Outcome
            //fieldType = context.FieldTypes.SingleOrDefault(u => u.Description == "DropDownList");
            //field = new Field() { Anonymise = false, Mandatory = false, FieldType = fieldType };

            //fieldValues = new[]
            //{
            //    new FieldValue { Default = true, Field = field, Other = false, Unknown = false, Value = "Died - Drug may be contributory" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "Died - Due to adverse reaction" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "Died - Unrelated to drug" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "Not yet recovered" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "Recovered" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "Uncertain outcome" }
            //};
            //context.FieldValues.AddRange(fieldValues);

            //element = new DatasetElement { System = false, DatasetElementType = elementType, ElementName = "ADR Outcome", Field = field };
            //context.DatasetElements.Add(element);

            //// Date of Death
            //fieldType = context.FieldTypes.SingleOrDefault(u => u.Description == "Date");
            //field = new Field() { Anonymise = false, Mandatory = false, FieldType = fieldType };
            //element = new DatasetElement { System = false, DatasetElementType = elementType, ElementName = "Date of Death", Field = field };
            //context.DatasetElements.Add(element);

            //// Date of Recovery
            //fieldType = context.FieldTypes.SingleOrDefault(u => u.Description == "Date");
            //field = new Field() { Anonymise = false, Mandatory = false, FieldType = fieldType };
            //element = new DatasetElement { System = false, DatasetElementType = elementType, ElementName = "Date of Recovery", Field = field };
            //context.DatasetElements.Add(element);

            //// ADR Description
            //fieldType = context.FieldTypes.SingleOrDefault(u => u.Description == "AlphaNumericTextbox");
            //field = new Field() { Anonymise = false, Mandatory = true, FieldType = fieldType };
            //field.MaxLength = 500;
            //element = new DatasetElement { System = false, DatasetElementType = elementType, ElementName = "ADR Description", Field = field };
            //context.DatasetElements.Add(element);



            //// Suspected Drug 1
            //fieldType = context.FieldTypes.SingleOrDefault(u => u.Description == "AlphaNumericTextbox");
            //field = new Field() { Anonymise = false, Mandatory = false, FieldType = fieldType };
            //field.MaxLength = 50;
            //element = new DatasetElement { System = false, DatasetElementType = elementType, ElementName = "Suspected Drug 1", Field = field };
            //context.DatasetElements.Add(element);

            //// Suspected Drug 1 Type
            //fieldType = context.FieldTypes.SingleOrDefault(u => u.Description == "DropDownList");
            //field = new Field() { Anonymise = false, Mandatory = false, FieldType = fieldType };

            //fieldValues = new[]
            //{
            //    new FieldValue { Default = true, Field = field, Other = false, Unknown = false, Value = "Brand name" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "Active ingredient" }
            //};
            //context.FieldValues.AddRange(fieldValues);

            //element = new DatasetElement { System = false, DatasetElementType = elementType, ElementName = "Suspected Drug 1 Type", Field = field };
            //context.DatasetElements.Add(element);

            //// Suspected Drug 1 Dosage
            //fieldType = context.FieldTypes.SingleOrDefault(u => u.Description == "AlphaNumericTextbox");
            //field = new Field() { Anonymise = false, Mandatory = false, FieldType = fieldType };
            //field.MaxLength = 20;
            //element = new DatasetElement { System = false, DatasetElementType = elementType, ElementName = "Suspected Drug 1 Dosage", Field = field };
            //context.DatasetElements.Add(element);

            //// Suspected Drug 1 Dosage Unit
            //fieldType = context.FieldTypes.SingleOrDefault(u => u.Description == "DropDownList");
            //field = new Field() { Anonymise = false, Mandatory = false, FieldType = fieldType };

            //fieldValues = new[]
            //{
            //    new FieldValue { Default = true, Field = field, Other = false, Unknown = false, Value = "Cells/kg" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "Cells/dose" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "mL/dose" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "mL/kg" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "Dosage form" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "Drops" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "Grams" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "Grams/m(sqr)" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "International units" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "International units - in 1000s" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "International units - in millions" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "Kilograms" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "Litres" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "Micrograms" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "Microlitres" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "Milicurries" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "Milligram/m(sqr)" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "Milligrams" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "Millilitres" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "Percent" }
            //};
            //context.FieldValues.AddRange(fieldValues);

            //element = new DatasetElement { System = false, DatasetElementType = elementType, ElementName = "Suspected Drug 1 Dosage Unit", Field = field };
            //context.DatasetElements.Add(element);

            //// Suspected Drug 1 Frequency
            //fieldType = context.FieldTypes.SingleOrDefault(u => u.Description == "DropDownList");
            //field = new Field() { Anonymise = false, Mandatory = false, FieldType = fieldType };

            //fieldValues = new[]
            //{
            //    new FieldValue { Default = true, Field = field, Other = false, Unknown = false, Value = "1 timeStat" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "1 time daily" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "2 times daily" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "3 times daily" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "4 times daily" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "As required" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "Cyclical" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "Daily" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "Monthly" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "Weekly" }
            //};
            //context.FieldValues.AddRange(fieldValues);

            //element = new DatasetElement { System = false, DatasetElementType = elementType, ElementName = "Suspected Drug 1 Frequency", Field = field };
            //context.DatasetElements.Add(element);

            //// Suspected Drug 1 Route
            //fieldType = context.FieldTypes.SingleOrDefault(u => u.Description == "DropDownList");
            //field = new Field() { Anonymise = false, Mandatory = false, FieldType = fieldType };

            //fieldValues = new[]
            //{
            //    new FieldValue { Default = true, Field = field, Other = false, Unknown = false, Value = "Intravitreal" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "Oral" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "Intravenous" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "Nasogastric" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "Intravascular" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "Buccal" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "Conjunctival" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "Epidural" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "Implant" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "Inhalation" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "Intra-arterial" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "Intra-articular" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "Intra-cardiac" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "Intradermal" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "Intramuscular" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "Intraocular" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "Intraperitoneal" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "Intrapleural" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "Intrathecal" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "Intrasynovial" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "Intradiscal" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "Intracisternal" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "Nasal" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "Otic" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "Periarticular" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "Intralesional" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "Parenteral" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "Dental" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "Sublingual" }
            //};
            //context.FieldValues.AddRange(fieldValues);

            //element = new DatasetElement { System = false, DatasetElementType = elementType, ElementName = "Suspected Drug 1 Route", Field = field };
            //context.DatasetElements.Add(element);

            //// Suspected Drug 1 Date Started
            //fieldType = context.FieldTypes.SingleOrDefault(u => u.Description == "Date");
            //field = new Field() { Anonymise = false, Mandatory = false, FieldType = fieldType };
            //element = new DatasetElement { System = false, DatasetElementType = elementType, ElementName = "Suspected Drug 1 Date Started", Field = field };
            //context.DatasetElements.Add(element);

            //// Suspected Drug 1 Date Stopped
            //fieldType = context.FieldTypes.SingleOrDefault(u => u.Description == "Date");
            //field = new Field() { Anonymise = false, Mandatory = false, FieldType = fieldType };
            //element = new DatasetElement { System = false, DatasetElementType = elementType, ElementName = "Suspected Drug 1 Date Stopped", Field = field };
            //context.DatasetElements.Add(element);

            //// Suspected Drug 1 Indication
            //fieldType = context.FieldTypes.SingleOrDefault(u => u.Description == "AlphaNumericTextbox");
            //field = new Field() { Anonymise = false, Mandatory = false, FieldType = fieldType };
            //field.MaxLength = 50;
            //element = new DatasetElement { System = false, DatasetElementType = elementType, ElementName = "Suspected Drug 1 Indication", Field = field };
            //context.DatasetElements.Add(element);

            //// Suspected Drug 1 Batch Number
            //fieldType = context.FieldTypes.SingleOrDefault(u => u.Description == "AlphaNumericTextbox");
            //field = new Field() { Anonymise = false, Mandatory = false, FieldType = fieldType };
            //field.MaxLength = 20;
            //element = new DatasetElement { System = false, DatasetElementType = elementType, ElementName = "Suspected Drug 1 Batch Number", Field = field };
            //context.DatasetElements.Add(element);

            //// Suspected Drug 1 Duration
            //fieldType = context.FieldTypes.SingleOrDefault(u => u.Description == "AlphaNumericTextbox");
            //field = new Field() { Anonymise = false, Mandatory = false, FieldType = fieldType };
            //field.MaxLength = 20;
            //element = new DatasetElement { System = false, DatasetElementType = elementType, ElementName = "Suspected Drug 1 Duration", Field = field };
            //context.DatasetElements.Add(element);


            //// Suspected Drug 2
            //fieldType = context.FieldTypes.SingleOrDefault(u => u.Description == "AlphaNumericTextbox");
            //field = new Field() { Anonymise = false, Mandatory = false, FieldType = fieldType };
            //field.MaxLength = 50;
            //element = new DatasetElement { System = false, DatasetElementType = elementType, ElementName = "Suspected Drug 2", Field = field };
            //context.DatasetElements.Add(element);

            //// Suspected Drug 2 Type
            //fieldType = context.FieldTypes.SingleOrDefault(u => u.Description == "DropDownList");
            //field = new Field() { Anonymise = false, Mandatory = false, FieldType = fieldType };

            //fieldValues = new[]
            //{
            //    new FieldValue { Default = true, Field = field, Other = false, Unknown = false, Value = "Brand name" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "Active ingredient" }
            //};
            //context.FieldValues.AddRange(fieldValues);

            //element = new DatasetElement { System = false, DatasetElementType = elementType, ElementName = "Suspected Drug 2 Type", Field = field };
            //context.DatasetElements.Add(element);

            //// Suspected Drug 2 Dosage
            //fieldType = context.FieldTypes.SingleOrDefault(u => u.Description == "AlphaNumericTextbox");
            //field = new Field() { Anonymise = false, Mandatory = false, FieldType = fieldType };
            //field.MaxLength = 20;
            //element = new DatasetElement { System = false, DatasetElementType = elementType, ElementName = "Suspected Drug 2 Dosage", Field = field };
            //context.DatasetElements.Add(element);

            //// Suspected Drug 2 Dosage Unit
            //fieldType = context.FieldTypes.SingleOrDefault(u => u.Description == "DropDownList");
            //field = new Field() { Anonymise = false, Mandatory = false, FieldType = fieldType };

            //fieldValues = new[]
            //{
            //    new FieldValue { Default = true, Field = field, Other = false, Unknown = false, Value = "Cells/kg" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "Cells/dose" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "mL/dose" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "mL/kg" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "Dosage form" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "Drops" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "Grams" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "Grams/m(sqr)" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "International units" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "International units - in 1000s" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "International units - in millions" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "Kilograms" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "Litres" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "Micrograms" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "Microlitres" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "Milicurries" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "Milligram/m(sqr)" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "Milligrams" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "Millilitres" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "Percent" }
            //};
            //context.FieldValues.AddRange(fieldValues);

            //element = new DatasetElement { System = false, DatasetElementType = elementType, ElementName = "Suspected Drug 2 Dosage Unit", Field = field };
            //context.DatasetElements.Add(element);

            //// Suspected Drug 2 Frequency
            //fieldType = context.FieldTypes.SingleOrDefault(u => u.Description == "DropDownList");
            //field = new Field() { Anonymise = false, Mandatory = false, FieldType = fieldType };

            //fieldValues = new[]
            //{
            //    new FieldValue { Default = true, Field = field, Other = false, Unknown = false, Value = "1 timeStat" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "1 time daily" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "2 times daily" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "3 times daily" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "4 times daily" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "As required" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "Cyclical" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "Daily" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "Monthly" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "Weekly" }
            //};
            //context.FieldValues.AddRange(fieldValues);

            //element = new DatasetElement { System = false, DatasetElementType = elementType, ElementName = "Suspected Drug 2 Frequency", Field = field };
            //context.DatasetElements.Add(element);

            //// Suspected Drug 2 Route
            //fieldType = context.FieldTypes.SingleOrDefault(u => u.Description == "DropDownList");
            //field = new Field() { Anonymise = false, Mandatory = false, FieldType = fieldType };

            //fieldValues = new[]
            //{
            //    new FieldValue { Default = true, Field = field, Other = false, Unknown = false, Value = "Intravitreal" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "Oral" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "Intravenous" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "Nasogastric" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "Intravascular" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "Buccal" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "Conjunctival" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "Epidural" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "Implant" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "Inhalation" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "Intra-arterial" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "Intra-articular" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "Intra-cardiac" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "Intradermal" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "Intramuscular" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "Intraocular" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "Intraperitoneal" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "Intrapleural" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "Intrathecal" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "Intrasynovial" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "Intradiscal" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "Intracisternal" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "Nasal" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "Otic" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "Periarticular" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "Intralesional" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "Parenteral" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "Dental" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "Sublingual" }
            //};
            //context.FieldValues.AddRange(fieldValues);

            //element = new DatasetElement { System = false, DatasetElementType = elementType, ElementName = "Suspected Drug 2 Route", Field = field };
            //context.DatasetElements.Add(element);

            //// Suspected Drug 2 Date Started
            //fieldType = context.FieldTypes.SingleOrDefault(u => u.Description == "Date");
            //field = new Field() { Anonymise = false, Mandatory = false, FieldType = fieldType };
            //element = new DatasetElement { System = false, DatasetElementType = elementType, ElementName = "Suspected Drug 2 Date Started", Field = field };
            //context.DatasetElements.Add(element);

            //// Suspected Drug 2 Date Stopped
            //fieldType = context.FieldTypes.SingleOrDefault(u => u.Description == "Date");
            //field = new Field() { Anonymise = false, Mandatory = false, FieldType = fieldType };
            //element = new DatasetElement { System = false, DatasetElementType = elementType, ElementName = "Suspected Drug 2 Date Stopped", Field = field };
            //context.DatasetElements.Add(element);

            //// Suspected Drug 2 Indication
            //fieldType = context.FieldTypes.SingleOrDefault(u => u.Description == "AlphaNumericTextbox");
            //field = new Field() { Anonymise = false, Mandatory = false, FieldType = fieldType };
            //field.MaxLength = 50;
            //element = new DatasetElement { System = false, DatasetElementType = elementType, ElementName = "Suspected Drug 2 Indication", Field = field };
            //context.DatasetElements.Add(element);

            //// Suspected Drug 2 Batch Number
            //fieldType = context.FieldTypes.SingleOrDefault(u => u.Description == "AlphaNumericTextbox");
            //field = new Field() { Anonymise = false, Mandatory = false, FieldType = fieldType };
            //field.MaxLength = 20;
            //element = new DatasetElement { System = false, DatasetElementType = elementType, ElementName = "Suspected Drug 2 Batch Number", Field = field };
            //context.DatasetElements.Add(element);

            //// Suspected Drug 2 Duration
            //fieldType = context.FieldTypes.SingleOrDefault(u => u.Description == "AlphaNumericTextbox");
            //field = new Field() { Anonymise = false, Mandatory = false, FieldType = fieldType };
            //field.MaxLength = 20;
            //element = new DatasetElement { System = false, DatasetElementType = elementType, ElementName = "Suspected Drug 2 Duration", Field = field };
            //context.DatasetElements.Add(element);




            //// Suspected Drug 3
            //fieldType = context.FieldTypes.SingleOrDefault(u => u.Description == "AlphaNumericTextbox");
            //field = new Field() { Anonymise = false, Mandatory = false, FieldType = fieldType };
            //field.MaxLength = 50;
            //element = new DatasetElement { System = false, DatasetElementType = elementType, ElementName = "Suspected Drug 3", Field = field };
            //context.DatasetElements.Add(element);

            //// Suspected Drug 3 Type
            //fieldType = context.FieldTypes.SingleOrDefault(u => u.Description == "DropDownList");
            //field = new Field() { Anonymise = false, Mandatory = false, FieldType = fieldType };

            //fieldValues = new[]
            //{
            //    new FieldValue { Default = true, Field = field, Other = false, Unknown = false, Value = "Brand name" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "Active ingredient" }
            //};
            //context.FieldValues.AddRange(fieldValues);

            //element = new DatasetElement { System = false, DatasetElementType = elementType, ElementName = "Suspected Drug 3 Type", Field = field };
            //context.DatasetElements.Add(element);

            //// Suspected Drug 3 Dosage
            //fieldType = context.FieldTypes.SingleOrDefault(u => u.Description == "AlphaNumericTextbox");
            //field = new Field() { Anonymise = false, Mandatory = false, FieldType = fieldType };
            //field.MaxLength = 20;
            //element = new DatasetElement { System = false, DatasetElementType = elementType, ElementName = "Suspected Drug 3 Dosage", Field = field };
            //context.DatasetElements.Add(element);

            //// Suspected Drug 3 Dosage Unit
            //fieldType = context.FieldTypes.SingleOrDefault(u => u.Description == "DropDownList");
            //field = new Field() { Anonymise = false, Mandatory = false, FieldType = fieldType };

            //fieldValues = new[]
            //{
            //    new FieldValue { Default = true, Field = field, Other = false, Unknown = false, Value = "Cells/kg" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "Cells/dose" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "mL/dose" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "mL/kg" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "Dosage form" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "Drops" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "Grams" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "Grams/m(sqr)" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "International units" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "International units - in 1000s" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "International units - in millions" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "Kilograms" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "Litres" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "Micrograms" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "Microlitres" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "Milicurries" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "Milligram/m(sqr)" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "Milligrams" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "Millilitres" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "Percent" }
            //};
            //context.FieldValues.AddRange(fieldValues);

            //element = new DatasetElement { System = false, DatasetElementType = elementType, ElementName = "Suspected Drug 3 Dosage Unit", Field = field };
            //context.DatasetElements.Add(element);

            //// Suspected Drug 3 Frequency
            //fieldType = context.FieldTypes.SingleOrDefault(u => u.Description == "DropDownList");
            //field = new Field() { Anonymise = false, Mandatory = false, FieldType = fieldType };

            //fieldValues = new[]
            //{
            //    new FieldValue { Default = true, Field = field, Other = false, Unknown = false, Value = "1 timeStat" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "1 time daily" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "2 times daily" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "3 times daily" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "4 times daily" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "As required" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "Cyclical" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "Daily" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "Monthly" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "Weekly" }
            //};
            //context.FieldValues.AddRange(fieldValues);

            //element = new DatasetElement { System = false, DatasetElementType = elementType, ElementName = "Suspected Drug 3 Frequency", Field = field };
            //context.DatasetElements.Add(element);

            //// Suspected Drug 3 Route
            //fieldType = context.FieldTypes.SingleOrDefault(u => u.Description == "DropDownList");
            //field = new Field() { Anonymise = false, Mandatory = false, FieldType = fieldType };

            //fieldValues = new[]
            //{
            //    new FieldValue { Default = true, Field = field, Other = false, Unknown = false, Value = "Intravitreal" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "Oral" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "Intravenous" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "Nasogastric" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "Intravascular" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "Buccal" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "Conjunctival" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "Epidural" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "Implant" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "Inhalation" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "Intra-arterial" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "Intra-articular" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "Intra-cardiac" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "Intradermal" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "Intramuscular" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "Intraocular" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "Intraperitoneal" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "Intrapleural" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "Intrathecal" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "Intrasynovial" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "Intradiscal" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "Intracisternal" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "Nasal" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "Otic" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "Periarticular" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "Intralesional" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "Parenteral" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "Dental" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "Sublingual" }
            //};
            //context.FieldValues.AddRange(fieldValues);

            //element = new DatasetElement { System = false, DatasetElementType = elementType, ElementName = "Suspected Drug 3 Route", Field = field };
            //context.DatasetElements.Add(element);

            //// Suspected Drug 3 Date Started
            //fieldType = context.FieldTypes.SingleOrDefault(u => u.Description == "Date");
            //field = new Field() { Anonymise = false, Mandatory = false, FieldType = fieldType };
            //element = new DatasetElement { System = false, DatasetElementType = elementType, ElementName = "Suspected Drug 3 Date Started", Field = field };
            //context.DatasetElements.Add(element);

            //// Suspected Drug 3 Date Stopped
            //fieldType = context.FieldTypes.SingleOrDefault(u => u.Description == "Date");
            //field = new Field() { Anonymise = false, Mandatory = false, FieldType = fieldType };
            //element = new DatasetElement { System = false, DatasetElementType = elementType, ElementName = "Suspected Drug 3 Date Stopped", Field = field };
            //context.DatasetElements.Add(element);

            //// Suspected Drug 3 Indication
            //fieldType = context.FieldTypes.SingleOrDefault(u => u.Description == "AlphaNumericTextbox");
            //field = new Field() { Anonymise = false, Mandatory = false, FieldType = fieldType };
            //field.MaxLength = 50;
            //element = new DatasetElement { System = false, DatasetElementType = elementType, ElementName = "Suspected Drug 3 Indication", Field = field };
            //context.DatasetElements.Add(element);

            //// Suspected Drug 3 Batch Number
            //fieldType = context.FieldTypes.SingleOrDefault(u => u.Description == "AlphaNumericTextbox");
            //field = new Field() { Anonymise = false, Mandatory = false, FieldType = fieldType };
            //field.MaxLength = 20;
            //element = new DatasetElement { System = false, DatasetElementType = elementType, ElementName = "Suspected Drug 3 Batch Number", Field = field };
            //context.DatasetElements.Add(element);

            //// Suspected Drug 3 Duration
            //fieldType = context.FieldTypes.SingleOrDefault(u => u.Description == "AlphaNumericTextbox");
            //field = new Field() { Anonymise = false, Mandatory = false, FieldType = fieldType };
            //field.MaxLength = 20;
            //element = new DatasetElement { System = false, DatasetElementType = elementType, ElementName = "Suspected Drug 3 Duration", Field = field };
            //context.DatasetElements.Add(element);



            //// Concomitant Drug 1
            //fieldType = context.FieldTypes.SingleOrDefault(u => u.Description == "AlphaNumericTextbox");
            //field = new Field() { Anonymise = false, Mandatory = false, FieldType = fieldType };
            //field.MaxLength = 50;
            //element = new DatasetElement { System = false, DatasetElementType = elementType, ElementName = "Concomitant Drug 1", Field = field };
            //context.DatasetElements.Add(element);

            //// Concomitant Drug 1 Type
            //fieldType = context.FieldTypes.SingleOrDefault(u => u.Description == "DropDownList");
            //field = new Field() { Anonymise = false, Mandatory = false, FieldType = fieldType };

            //fieldValues = new[]
            //{
            //    new FieldValue { Default = true, Field = field, Other = false, Unknown = false, Value = "Brand name" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "Active ingredient" }
            //};
            //context.FieldValues.AddRange(fieldValues);

            //element = new DatasetElement { System = false, DatasetElementType = elementType, ElementName = "Concomitant Drug 1 Type", Field = field };
            //context.DatasetElements.Add(element);

            //// Concomitant Drug 1 Dosage
            //fieldType = context.FieldTypes.SingleOrDefault(u => u.Description == "AlphaNumericTextbox");
            //field = new Field() { Anonymise = false, Mandatory = false, FieldType = fieldType };
            //field.MaxLength = 20;
            //element = new DatasetElement { System = false, DatasetElementType = elementType, ElementName = "Concomitant Drug 1 Dosage", Field = field };
            //context.DatasetElements.Add(element);

            //// Concomitant Drug 1 Dosage Unit
            //fieldType = context.FieldTypes.SingleOrDefault(u => u.Description == "DropDownList");
            //field = new Field() { Anonymise = false, Mandatory = false, FieldType = fieldType };

            //fieldValues = new[]
            //{
            //    new FieldValue { Default = true, Field = field, Other = false, Unknown = false, Value = "Cells/kg" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "Cells/dose" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "mL/dose" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "mL/kg" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "Dosage form" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "Drops" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "Grams" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "Grams/m(sqr)" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "International units" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "International units - in 1000s" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "International units - in millions" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "Kilograms" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "Litres" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "Micrograms" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "Microlitres" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "Milicurries" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "Milligram/m(sqr)" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "Milligrams" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "Millilitres" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "Percent" }
            //};
            //context.FieldValues.AddRange(fieldValues);

            //element = new DatasetElement { System = false, DatasetElementType = elementType, ElementName = "Concomitant Drug 1 Dosage Unit", Field = field };
            //context.DatasetElements.Add(element);

            //// Concomitant Drug 1 Frequency
            //fieldType = context.FieldTypes.SingleOrDefault(u => u.Description == "DropDownList");
            //field = new Field() { Anonymise = false, Mandatory = false, FieldType = fieldType };

            //fieldValues = new[]
            //{
            //    new FieldValue { Default = true, Field = field, Other = false, Unknown = false, Value = "1 timeStat" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "1 time daily" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "2 times daily" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "3 times daily" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "4 times daily" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "As required" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "Cyclical" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "Daily" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "Monthly" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "Weekly" }
            //};
            //context.FieldValues.AddRange(fieldValues);

            //element = new DatasetElement { System = false, DatasetElementType = elementType, ElementName = "Concomitant Drug 1 Frequency", Field = field };
            //context.DatasetElements.Add(element);

            //// Concomitant Drug 1 Route
            //fieldType = context.FieldTypes.SingleOrDefault(u => u.Description == "DropDownList");
            //field = new Field() { Anonymise = false, Mandatory = false, FieldType = fieldType };

            //fieldValues = new[]
            //{
            //    new FieldValue { Default = true, Field = field, Other = false, Unknown = false, Value = "Intravitreal" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "Oral" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "Intravenous" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "Nasogastric" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "Intravascular" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "Buccal" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "Conjunctival" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "Epidural" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "Implant" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "Inhalation" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "Intra-arterial" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "Intra-articular" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "Intra-cardiac" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "Intradermal" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "Intramuscular" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "Intraocular" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "Intraperitoneal" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "Intrapleural" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "Intrathecal" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "Intrasynovial" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "Intradiscal" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "Intracisternal" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "Nasal" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "Otic" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "Periarticular" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "Intralesional" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "Parenteral" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "Dental" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "Sublingual" }
            //};
            //context.FieldValues.AddRange(fieldValues);

            //element = new DatasetElement { System = false, DatasetElementType = elementType, ElementName = "Concomitant Drug 1 Route", Field = field };
            //context.DatasetElements.Add(element);

            //// Concomitant Drug 1 Date Started
            //fieldType = context.FieldTypes.SingleOrDefault(u => u.Description == "Date");
            //field = new Field() { Anonymise = false, Mandatory = false, FieldType = fieldType };
            //element = new DatasetElement { System = false, DatasetElementType = elementType, ElementName = "Concomitant Drug 1 Date Started", Field = field };
            //context.DatasetElements.Add(element);

            //// Concomitant Drug 1 Date Stopped
            //fieldType = context.FieldTypes.SingleOrDefault(u => u.Description == "Date");
            //field = new Field() { Anonymise = false, Mandatory = false, FieldType = fieldType };
            //element = new DatasetElement { System = false, DatasetElementType = elementType, ElementName = "Concomitant Drug 1 Date Stopped", Field = field };
            //context.DatasetElements.Add(element);

            //// Concomitant Drug 1 Indication
            //fieldType = context.FieldTypes.SingleOrDefault(u => u.Description == "AlphaNumericTextbox");
            //field = new Field() { Anonymise = false, Mandatory = false, FieldType = fieldType };
            //field.MaxLength = 50;
            //element = new DatasetElement { System = false, DatasetElementType = elementType, ElementName = "Concomitant Drug 1 Indication", Field = field };
            //context.DatasetElements.Add(element);

            //// Concomitant Drug 1 Batch Number
            //fieldType = context.FieldTypes.SingleOrDefault(u => u.Description == "AlphaNumericTextbox");
            //field = new Field() { Anonymise = false, Mandatory = false, FieldType = fieldType };
            //field.MaxLength = 20;
            //element = new DatasetElement { System = false, DatasetElementType = elementType, ElementName = "Concomitant Drug 1 Batch Number", Field = field };
            //context.DatasetElements.Add(element);

            //// Concomitant Drug 1 Duration
            //fieldType = context.FieldTypes.SingleOrDefault(u => u.Description == "AlphaNumericTextbox");
            //field = new Field() { Anonymise = false, Mandatory = false, FieldType = fieldType };
            //field.MaxLength = 20;
            //element = new DatasetElement { System = false, DatasetElementType = elementType, ElementName = "Concomitant Drug 1 Duration", Field = field };
            //context.DatasetElements.Add(element);


            //// Concomitant Drug 2
            //fieldType = context.FieldTypes.SingleOrDefault(u => u.Description == "AlphaNumericTextbox");
            //field = new Field() { Anonymise = false, Mandatory = false, FieldType = fieldType };
            //field.MaxLength = 50;
            //element = new DatasetElement { System = false, DatasetElementType = elementType, ElementName = "Concomitant Drug 2", Field = field };
            //context.DatasetElements.Add(element);

            //// Concomitant Drug 2 Type
            //fieldType = context.FieldTypes.SingleOrDefault(u => u.Description == "DropDownList");
            //field = new Field() { Anonymise = false, Mandatory = false, FieldType = fieldType };

            //fieldValues = new[]
            //{
            //    new FieldValue { Default = true, Field = field, Other = false, Unknown = false, Value = "Brand name" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "Active ingredient" }
            //};
            //context.FieldValues.AddRange(fieldValues);

            //element = new DatasetElement { System = false, DatasetElementType = elementType, ElementName = "Concomitant Drug 2 Type", Field = field };
            //context.DatasetElements.Add(element);

            //// Concomitant Drug 2 Dosage
            //fieldType = context.FieldTypes.SingleOrDefault(u => u.Description == "AlphaNumericTextbox");
            //field = new Field() { Anonymise = false, Mandatory = false, FieldType = fieldType };
            //field.MaxLength = 20;
            //element = new DatasetElement { System = false, DatasetElementType = elementType, ElementName = "Concomitant Drug 2 Dosage", Field = field };
            //context.DatasetElements.Add(element);

            //// Concomitant Drug 2 Dosage Unit
            //fieldType = context.FieldTypes.SingleOrDefault(u => u.Description == "DropDownList");
            //field = new Field() { Anonymise = false, Mandatory = false, FieldType = fieldType };

            //fieldValues = new[]
            //{
            //    new FieldValue { Default = true, Field = field, Other = false, Unknown = false, Value = "Cells/kg" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "Cells/dose" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "mL/dose" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "mL/kg" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "Dosage form" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "Drops" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "Grams" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "Grams/m(sqr)" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "International units" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "International units - in 1000s" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "International units - in millions" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "Kilograms" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "Litres" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "Micrograms" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "Microlitres" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "Milicurries" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "Milligram/m(sqr)" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "Milligrams" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "Millilitres" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "Percent" }
            //};
            //context.FieldValues.AddRange(fieldValues);

            //element = new DatasetElement { System = false, DatasetElementType = elementType, ElementName = "Concomitant Drug 2 Dosage Unit", Field = field };
            //context.DatasetElements.Add(element);

            //// Concomitant Drug 2 Frequency
            //fieldType = context.FieldTypes.SingleOrDefault(u => u.Description == "DropDownList");
            //field = new Field() { Anonymise = false, Mandatory = false, FieldType = fieldType };

            //fieldValues = new[]
            //{
            //    new FieldValue { Default = true, Field = field, Other = false, Unknown = false, Value = "1 timeStat" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "1 time daily" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "2 times daily" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "3 times daily" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "4 times daily" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "As required" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "Cyclical" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "Daily" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "Monthly" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "Weekly" }
            //};
            //context.FieldValues.AddRange(fieldValues);

            //element = new DatasetElement { System = false, DatasetElementType = elementType, ElementName = "Concomitant Drug 2 Frequency", Field = field };
            //context.DatasetElements.Add(element);

            //// Concomitant Drug 2 Route
            //fieldType = context.FieldTypes.SingleOrDefault(u => u.Description == "DropDownList");
            //field = new Field() { Anonymise = false, Mandatory = false, FieldType = fieldType };

            //fieldValues = new[]
            //{
            //    new FieldValue { Default = true, Field = field, Other = false, Unknown = false, Value = "Intravitreal" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "Oral" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "Intravenous" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "Nasogastric" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "Intravascular" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "Buccal" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "Conjunctival" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "Epidural" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "Implant" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "Inhalation" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "Intra-arterial" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "Intra-articular" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "Intra-cardiac" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "Intradermal" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "Intramuscular" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "Intraocular" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "Intraperitoneal" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "Intrapleural" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "Intrathecal" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "Intrasynovial" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "Intradiscal" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "Intracisternal" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "Nasal" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "Otic" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "Periarticular" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "Intralesional" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "Parenteral" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "Dental" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "Sublingual" }
            //};
            //context.FieldValues.AddRange(fieldValues);

            //element = new DatasetElement { System = false, DatasetElementType = elementType, ElementName = "Concomitant Drug 2 Route", Field = field };
            //context.DatasetElements.Add(element);

            //// Concomitant Drug 2 Date Started
            //fieldType = context.FieldTypes.SingleOrDefault(u => u.Description == "Date");
            //field = new Field() { Anonymise = false, Mandatory = false, FieldType = fieldType };
            //element = new DatasetElement { System = false, DatasetElementType = elementType, ElementName = "Concomitant Drug 2 Date Started", Field = field };
            //context.DatasetElements.Add(element);

            //// Concomitant Drug 2 Date Stopped
            //fieldType = context.FieldTypes.SingleOrDefault(u => u.Description == "Date");
            //field = new Field() { Anonymise = false, Mandatory = false, FieldType = fieldType };
            //element = new DatasetElement { System = false, DatasetElementType = elementType, ElementName = "Concomitant Drug 2 Date Stopped", Field = field };
            //context.DatasetElements.Add(element);

            //// Concomitant Drug 2 Indication
            //fieldType = context.FieldTypes.SingleOrDefault(u => u.Description == "AlphaNumericTextbox");
            //field = new Field() { Anonymise = false, Mandatory = false, FieldType = fieldType };
            //field.MaxLength = 50;
            //element = new DatasetElement { System = false, DatasetElementType = elementType, ElementName = "Concomitant Drug 2 Indication", Field = field };
            //context.DatasetElements.Add(element);

            //// Concomitant Drug 2 Batch Number
            //fieldType = context.FieldTypes.SingleOrDefault(u => u.Description == "AlphaNumericTextbox");
            //field = new Field() { Anonymise = false, Mandatory = false, FieldType = fieldType };
            //field.MaxLength = 20;
            //element = new DatasetElement { System = false, DatasetElementType = elementType, ElementName = "Concomitant Drug 2 Batch Number", Field = field };
            //context.DatasetElements.Add(element);

            //// Concomitant Drug 2 Duration
            //fieldType = context.FieldTypes.SingleOrDefault(u => u.Description == "AlphaNumericTextbox");
            //field = new Field() { Anonymise = false, Mandatory = false, FieldType = fieldType };
            //field.MaxLength = 20;
            //element = new DatasetElement { System = false, DatasetElementType = elementType, ElementName = "Concomitant Drug 2 Duration", Field = field };
            //context.DatasetElements.Add(element);




            //// Concomitant Drug 3
            //fieldType = context.FieldTypes.SingleOrDefault(u => u.Description == "AlphaNumericTextbox");
            //field = new Field() { Anonymise = false, Mandatory = false, FieldType = fieldType };
            //field.MaxLength = 50;
            //element = new DatasetElement { System = false, DatasetElementType = elementType, ElementName = "Concomitant Drug 3", Field = field };
            //context.DatasetElements.Add(element);

            //// Concomitant Drug 3 Type
            //fieldType = context.FieldTypes.SingleOrDefault(u => u.Description == "DropDownList");
            //field = new Field() { Anonymise = false, Mandatory = false, FieldType = fieldType };

            //fieldValues = new[]
            //{
            //    new FieldValue { Default = true, Field = field, Other = false, Unknown = false, Value = "Brand name" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "Active ingredient" }
            //};
            //context.FieldValues.AddRange(fieldValues);

            //element = new DatasetElement { System = false, DatasetElementType = elementType, ElementName = "Concomitant Drug 3 Type", Field = field };
            //context.DatasetElements.Add(element);

            //// Concomitant Drug 3 Dosage
            //fieldType = context.FieldTypes.SingleOrDefault(u => u.Description == "AlphaNumericTextbox");
            //field = new Field() { Anonymise = false, Mandatory = false, FieldType = fieldType };
            //field.MaxLength = 20;
            //element = new DatasetElement { System = false, DatasetElementType = elementType, ElementName = "Concomitant Drug 3 Dosage", Field = field };
            //context.DatasetElements.Add(element);

            //// Concomitant Drug 3 Dosage Unit
            //fieldType = context.FieldTypes.SingleOrDefault(u => u.Description == "DropDownList");
            //field = new Field() { Anonymise = false, Mandatory = false, FieldType = fieldType };

            //fieldValues = new[]
            //{
            //    new FieldValue { Default = true, Field = field, Other = false, Unknown = false, Value = "Cells/kg" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "Cells/dose" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "mL/dose" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "mL/kg" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "Dosage form" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "Drops" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "Grams" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "Grams/m(sqr)" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "International units" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "International units - in 1000s" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "International units - in millions" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "Kilograms" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "Litres" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "Micrograms" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "Microlitres" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "Milicurries" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "Milligram/m(sqr)" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "Milligrams" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "Millilitres" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "Percent" }
            //};
            //context.FieldValues.AddRange(fieldValues);

            //element = new DatasetElement { System = false, DatasetElementType = elementType, ElementName = "Concomitant Drug 3 Dosage Unit", Field = field };
            //context.DatasetElements.Add(element);

            //// Concomitant Drug 3 Frequency
            //fieldType = context.FieldTypes.SingleOrDefault(u => u.Description == "DropDownList");
            //field = new Field() { Anonymise = false, Mandatory = false, FieldType = fieldType };

            //fieldValues = new[]
            //{
            //    new FieldValue { Default = true, Field = field, Other = false, Unknown = false, Value = "1 timeStat" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "1 time daily" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "2 times daily" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "3 times daily" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "4 times daily" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "As required" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "Cyclical" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "Daily" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "Monthly" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "Weekly" }
            //};
            //context.FieldValues.AddRange(fieldValues);

            //element = new DatasetElement { System = false, DatasetElementType = elementType, ElementName = "Concomitant Drug 3 Frequency", Field = field };
            //context.DatasetElements.Add(element);

            //// Concomitant Drug 3 Route
            //fieldType = context.FieldTypes.SingleOrDefault(u => u.Description == "DropDownList");
            //field = new Field() { Anonymise = false, Mandatory = false, FieldType = fieldType };

            //fieldValues = new[]
            //{
            //    new FieldValue { Default = true, Field = field, Other = false, Unknown = false, Value = "Intravitreal" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "Oral" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "Intravenous" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "Nasogastric" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "Intravascular" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "Buccal" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "Conjunctival" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "Epidural" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "Implant" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "Inhalation" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "Intra-arterial" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "Intra-articular" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "Intra-cardiac" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "Intradermal" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "Intramuscular" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "Intraocular" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "Intraperitoneal" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "Intrapleural" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "Intrathecal" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "Intrasynovial" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "Intradiscal" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "Intracisternal" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "Nasal" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "Otic" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "Periarticular" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "Intralesional" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "Parenteral" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "Dental" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "Sublingual" }
            //};
            //context.FieldValues.AddRange(fieldValues);

            //element = new DatasetElement { System = false, DatasetElementType = elementType, ElementName = "Concomitant Drug 3 Route", Field = field };
            //context.DatasetElements.Add(element);

            //// Concomitant Drug 3 Date Started
            //fieldType = context.FieldTypes.SingleOrDefault(u => u.Description == "Date");
            //field = new Field() { Anonymise = false, Mandatory = false, FieldType = fieldType };
            //element = new DatasetElement { System = false, DatasetElementType = elementType, ElementName = "Concomitant Drug 3 Date Started", Field = field };
            //context.DatasetElements.Add(element);

            //// Concomitant Drug 3 Date Stopped
            //fieldType = context.FieldTypes.SingleOrDefault(u => u.Description == "Date");
            //field = new Field() { Anonymise = false, Mandatory = false, FieldType = fieldType };
            //element = new DatasetElement { System = false, DatasetElementType = elementType, ElementName = "Concomitant Drug 3 Date Stopped", Field = field };
            //context.DatasetElements.Add(element);

            //// Concomitant Drug 3 Indication
            //fieldType = context.FieldTypes.SingleOrDefault(u => u.Description == "AlphaNumericTextbox");
            //field = new Field() { Anonymise = false, Mandatory = false, FieldType = fieldType };
            //field.MaxLength = 50;
            //element = new DatasetElement { System = false, DatasetElementType = elementType, ElementName = "Concomitant Drug 3 Indication", Field = field };
            //context.DatasetElements.Add(element);

            //// Concomitant Drug 3 Batch Number
            //fieldType = context.FieldTypes.SingleOrDefault(u => u.Description == "AlphaNumericTextbox");
            //field = new Field() { Anonymise = false, Mandatory = false, FieldType = fieldType };
            //field.MaxLength = 20;
            //element = new DatasetElement { System = false, DatasetElementType = elementType, ElementName = "Concomitant Drug 3 Batch Number", Field = field };
            //context.DatasetElements.Add(element);

            //// Concomitant Drug 3 Duration
            //fieldType = context.FieldTypes.SingleOrDefault(u => u.Description == "AlphaNumericTextbox");
            //field = new Field() { Anonymise = false, Mandatory = false, FieldType = fieldType };
            //field.MaxLength = 20;
            //element = new DatasetElement { System = false, DatasetElementType = elementType, ElementName = "Concomitant Drug 3 Duration", Field = field };
            //context.DatasetElements.Add(element);




            //// Other Relevant Information
            //fieldType = context.FieldTypes.SingleOrDefault(u => u.Description == "AlphaNumericTextbox");
            //field = new Field() { Anonymise = false, Mandatory = false, FieldType = fieldType };
            //field.MaxLength = 500;
            //element = new DatasetElement { System = false, DatasetElementType = elementType, ElementName = "Other Relevant Information", Field = field };
            //context.DatasetElements.Add(element);

            //// Hospitalisation
            //fieldType = context.FieldTypes.SingleOrDefault(u => u.Description == "DropDownList");
            //field = new Field() { Anonymise = false, Mandatory = false, FieldType = fieldType };

            //fieldValues = new[]
            //{
            //    new FieldValue { Default = true, Field = field, Other = false, Unknown = false, Value = "Yes" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "No" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "Already hospitalised" }
            //};
            //context.FieldValues.AddRange(fieldValues);

            //element = new DatasetElement { System = false, DatasetElementType = elementType, ElementName = "Hospitalisation", Field = field };
            //context.DatasetElements.Add(element);

            //// Causality
            //fieldType = context.FieldTypes.SingleOrDefault(u => u.Description == "DropDownList");
            //field = new Field() { Anonymise = false, Mandatory = false, FieldType = fieldType };

            //fieldValues = new[]
            //{
            //    new FieldValue { Default = true, Field = field, Other = false, Unknown = false, Value = "Certain" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "Probable" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "Possible" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "Unlikely" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "Unconfirmed" }
            //};
            //context.FieldValues.AddRange(fieldValues);

            //element = new DatasetElement { System = false, DatasetElementType = elementType, ElementName = "Causality", Field = field };
            //context.DatasetElements.Add(element);

            //// Serious Reaction
            //fieldType = context.FieldTypes.SingleOrDefault(u => u.Description == "DropDownList");
            //field = new Field() { Anonymise = false, Mandatory = false, FieldType = fieldType };

            //fieldValues = new[]
            //{
            //    new FieldValue { Default = true, Field = field, Other = false, Unknown = false, Value = "Yes" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "No" }
            //};
            //context.FieldValues.AddRange(fieldValues);

            //element = new DatasetElement { System = false, DatasetElementType = elementType, ElementName = "Serious Reaction", Field = field };
            //context.DatasetElements.Add(element);

            //// Serious Reaction Reason
            //fieldType = context.FieldTypes.SingleOrDefault(u => u.Description == "Listbox");
            //field = new Field() { Anonymise = false, Mandatory = false, FieldType = fieldType };

            //fieldValues = new[]
            //{
            //    new FieldValue { Default = true, Field = field, Other = false, Unknown = false, Value = "Patient died due to reaction" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "Life threatening" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "Congenital anomaly" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "Involved or prolonged in-patient hospitalisation" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "Involved persistent or significant disability or incapacity" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "Medically significant" }
            //};
            //context.FieldValues.AddRange(fieldValues);

            //element = new DatasetElement { System = false, DatasetElementType = elementType, ElementName = "Serious Reaction Reason", Field = field };
            //context.DatasetElements.Add(element);

            //// Medically Significant
            //fieldType = context.FieldTypes.SingleOrDefault(u => u.Description == "AlphaNumericTextbox");
            //field = new Field() { Anonymise = false, Mandatory = false, FieldType = fieldType };
            //field.MaxLength = 500;
            //element = new DatasetElement { System = false, DatasetElementType = elementType, ElementName = "Medically Significant", Field = field };
            //context.DatasetElements.Add(element);

            //// Sequelae (any permanent complications or injuries as a result of the ADR)?
            //fieldType = context.FieldTypes.SingleOrDefault(u => u.Description == "DropDownList");
            //field = new Field() { Anonymise = false, Mandatory = false, FieldType = fieldType };

            //fieldValues = new[]
            //{
            //    new FieldValue { Default = true, Field = field, Other = false, Unknown = false, Value = "Yes" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "No" }
            //};
            //context.FieldValues.AddRange(fieldValues);

            //element = new DatasetElement { System = false, DatasetElementType = elementType, ElementName = "Sequelae (any permanent complications or injuries as a result of the ADR)?", Field = field };
            //context.DatasetElements.Add(element);

            //// Was treatment given?
            //fieldType = context.FieldTypes.SingleOrDefault(u => u.Description == "DropDownList");
            //field = new Field() { Anonymise = false, Mandatory = false, FieldType = fieldType };

            //fieldValues = new[]
            //{
            //    new FieldValue { Default = true, Field = field, Other = false, Unknown = false, Value = "Yes" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "No" }
            //};
            //context.FieldValues.AddRange(fieldValues);

            //element = new DatasetElement { System = false, DatasetElementType = elementType, ElementName = "Was treatment given?", Field = field };
            //context.DatasetElements.Add(element);

            //// Treatment Given
            //fieldType = context.FieldTypes.SingleOrDefault(u => u.Description == "AlphaNumericTextbox");
            //field = new Field() { Anonymise = false, Mandatory = false, FieldType = fieldType };
            //field.MaxLength = 500;
            //element = new DatasetElement { System = false, DatasetElementType = elementType, ElementName = "Treatment Given", Field = field };
            //context.DatasetElements.Add(element);

            //// Reporter Name
            //fieldType = context.FieldTypes.SingleOrDefault(u => u.Description == "AlphaNumericTextbox");
            //field = new Field() { Anonymise = false, Mandatory = false, FieldType = fieldType };
            //field.MaxLength = 60;
            //element = new DatasetElement { System = false, DatasetElementType = elementType, ElementName = "Reporter Name", Field = field };
            //context.DatasetElements.Add(element);

            //// Reporter Profession
            //fieldType = context.FieldTypes.SingleOrDefault(u => u.Description == "DropDownList");
            //field = new Field() { Anonymise = false, Mandatory = false, FieldType = fieldType };

            //fieldValues = new[]
            //{
            //    new FieldValue { Default = true, Field = field, Other = false, Unknown = false, Value = "Dentist" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "Doctor" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "Drug company" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "Nurse" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "Pharmacist" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "Consumer" },
            //    new FieldValue { Default = false, Field = field, Other = true, Unknown = false, Value = "Other" },
            //    new FieldValue { Default = false, Field = field, Other = true, Unknown = false, Value = "TCM practioner" },
            //    new FieldValue { Default = false, Field = field, Other = true, Unknown = false, Value = "Research coordinator" }
            //};
            //context.FieldValues.AddRange(fieldValues);

            //element = new DatasetElement { System = false, DatasetElementType = elementType, ElementName = "Reporter Profession", Field = field };
            //context.DatasetElements.Add(element);

            //// Report Reference Number
            //fieldType = context.FieldTypes.SingleOrDefault(u => u.Description == "AlphaNumericTextbox");
            //field = new Field() { Anonymise = false, Mandatory = false, FieldType = fieldType };
            //field.MaxLength = 30;
            //element = new DatasetElement { System = false, DatasetElementType = elementType, ElementName = "Report Reference Number", Field = field };
            //context.DatasetElements.Add(element);

            //// Reporter Telephone Number
            //fieldType = context.FieldTypes.SingleOrDefault(u => u.Description == "AlphaNumericTextbox");
            //field = new Field() { Anonymise = false, Mandatory = false, FieldType = fieldType };
            //field.MaxLength = 30;
            //element = new DatasetElement { System = false, DatasetElementType = elementType, ElementName = "Reporter Telephone Number", Field = field };
            //context.DatasetElements.Add(element);

            //// Reporter Fax Number
            //fieldType = context.FieldTypes.SingleOrDefault(u => u.Description == "AlphaNumericTextbox");
            //field = new Field() { Anonymise = false, Mandatory = false, FieldType = fieldType };
            //field.MaxLength = 30;
            //element = new DatasetElement { System = false, DatasetElementType = elementType, ElementName = "Reporter Fax Number", Field = field };
            //context.DatasetElements.Add(element);

            //// Reporter Email
            //fieldType = context.FieldTypes.SingleOrDefault(u => u.Description == "AlphaNumericTextbox");
            //field = new Field() { Anonymise = false, Mandatory = false, FieldType = fieldType };
            //field.MaxLength = 50;
            //element = new DatasetElement { System = false, DatasetElementType = elementType, ElementName = "Reporter Email", Field = field };
            //context.DatasetElements.Add(element);

            //// Reporter Place of Practise
            //fieldType = context.FieldTypes.SingleOrDefault(u => u.Description == "AlphaNumericTextbox");
            //field = new Field() { Anonymise = false, Mandatory = false, FieldType = fieldType };
            //field.MaxLength = 50;
            //element = new DatasetElement { System = false, DatasetElementType = elementType, ElementName = "Reporter Place of Practise", Field = field };
            //context.DatasetElements.Add(element);

            //// Reporter Address
            //fieldType = context.FieldTypes.SingleOrDefault(u => u.Description == "AlphaNumericTextbox");
            //field = new Field() { Anonymise = false, Mandatory = false, FieldType = fieldType };
            //field.MaxLength = 250;
            //element = new DatasetElement { System = false, DatasetElementType = elementType, ElementName = "Reporter Address", Field = field };
            //context.DatasetElements.Add(element);

            // TerminologyMedDra
            //fieldType = context.FieldTypes.SingleOrDefault(u => u.Description == "System");
            //field = new Field() { Anonymise = false, Mandatory = false, FieldType = fieldType };
            //element = new DatasetElement { System = false, DatasetElementType = elementType, ElementName = "TerminologyMedDra", Field = field };
            //context.DatasetElements.Add(element);

            // SuspectDrug1_Naranjo
            //fieldType = context.FieldTypes.SingleOrDefault(u => u.Description == "System");
            //field = new Field() { Anonymise = false, Mandatory = false, FieldType = fieldType };
            //element = new DatasetElement { System = false, DatasetElementType = elementType, ElementName = "SuspectDrug1_Naranjo", Field = field };
            //context.DatasetElements.Add(element);

            //// SuspectDrug1_WHO
            //fieldType = context.FieldTypes.SingleOrDefault(u => u.Description == "System");
            //field = new Field() { Anonymise = false, Mandatory = false, FieldType = fieldType };
            //element = new DatasetElement { System = false, DatasetElementType = elementType, ElementName = "SuspectDrug1_WHO", Field = field };
            //context.DatasetElements.Add(element);

            //// SuspectDrug2_Naranjo
            //fieldType = context.FieldTypes.SingleOrDefault(u => u.Description == "System");
            //field = new Field() { Anonymise = false, Mandatory = false, FieldType = fieldType };
            //element = new DatasetElement { System = false, DatasetElementType = elementType, ElementName = "SuspectDrug2_Naranjo", Field = field };
            //context.DatasetElements.Add(element);

            //// SuspectDrug2_WHO
            //fieldType = context.FieldTypes.SingleOrDefault(u => u.Description == "System");
            //field = new Field() { Anonymise = false, Mandatory = false, FieldType = fieldType };
            //element = new DatasetElement { System = false, DatasetElementType = elementType, ElementName = "SuspectDrug2_WHO", Field = field };
            //context.DatasetElements.Add(element);

            //// SuspectDrug3_Naranjo
            //fieldType = context.FieldTypes.SingleOrDefault(u => u.Description == "System");
            //field = new Field() { Anonymise = false, Mandatory = false, FieldType = fieldType };
            //element = new DatasetElement { System = false, DatasetElementType = elementType, ElementName = "SuspectDrug3_Naranjo", Field = field };
            //context.DatasetElements.Add(element);

            //// SuspectDrug3_WHO
            //fieldType = context.FieldTypes.SingleOrDefault(u => u.Description == "System");
            //field = new Field() { Anonymise = false, Mandatory = false, FieldType = fieldType };
            //element = new DatasetElement { System = false, DatasetElementType = elementType, ElementName = "SuspectDrug3_WHO", Field = field };
            //context.DatasetElements.Add(element);

            // ConcomitantDrug1_Naranjo
            fieldType = context.FieldTypes.SingleOrDefault(u => u.Description == "System");
            field = new Field() { Anonymise = false, Mandatory = false, FieldType = fieldType };
            element = new DatasetElement { System = false, DatasetElementType = elementType, ElementName = "ConcomitantDrug1_Naranjo", Field = field };
            context.DatasetElements.Add(element);

            // ConcomitantDrug1_WHO
            fieldType = context.FieldTypes.SingleOrDefault(u => u.Description == "System");
            field = new Field() { Anonymise = false, Mandatory = false, FieldType = fieldType };
            element = new DatasetElement { System = false, DatasetElementType = elementType, ElementName = "ConcomitantDrug1_WHO", Field = field };
            context.DatasetElements.Add(element);

            // ConcomitantDrug2_Naranjo
            fieldType = context.FieldTypes.SingleOrDefault(u => u.Description == "System");
            field = new Field() { Anonymise = false, Mandatory = false, FieldType = fieldType };
            element = new DatasetElement { System = false, DatasetElementType = elementType, ElementName = "ConcomitantDrug2_Naranjo", Field = field };
            context.DatasetElements.Add(element);

            // ConcomitantDrug2_WHO
            fieldType = context.FieldTypes.SingleOrDefault(u => u.Description == "System");
            field = new Field() { Anonymise = false, Mandatory = false, FieldType = fieldType };
            element = new DatasetElement { System = false, DatasetElementType = elementType, ElementName = "ConcomitantDrug2_WHO", Field = field };
            context.DatasetElements.Add(element);

            // ConcomitantDrug3_Naranjo
            fieldType = context.FieldTypes.SingleOrDefault(u => u.Description == "System");
            field = new Field() { Anonymise = false, Mandatory = false, FieldType = fieldType };
            element = new DatasetElement { System = false, DatasetElementType = elementType, ElementName = "ConcomitantDrug3_Naranjo", Field = field };
            context.DatasetElements.Add(element);

            // ConcomitantDrug3_WHO
            fieldType = context.FieldTypes.SingleOrDefault(u => u.Description == "System");
            field = new Field() { Anonymise = false, Mandatory = false, FieldType = fieldType };
            element = new DatasetElement { System = false, DatasetElementType = elementType, ElementName = "ConcomitantDrug3_WHO", Field = field };
            context.DatasetElements.Add(element);

            context.SaveChanges();
        }

        private static void CreateSpontaneousDataset(PVIMSDbContext context)
        {
            var contextType = context.ContextTypes.Single(u => u.Description == "Global");
            var user = context.Users.Single(u => u.UserName == "Admin");

            // Dataset
            var dataset = new Dataset { Active = true, ContextType = contextType, DatasetName = "Spontaneous Report", Help = "SUSPECTED ADVERSE DRUG REACTION (ADR) ONLINE REPORTING FORM - http://eservice.hsa.gov.sg/adr/adr/adrOnline.do?action=loadOnlineForm" };
            context.Datasets.Add(dataset);

            // Categories
            var datasetCategory = new DatasetCategory { CategoryOrder = 1, Dataset = dataset, DatasetCategoryName = "Particulars of Patient" };
            context.DatasetCategories.Add(datasetCategory);

            // Link to elements
            var datasetElement = context.DatasetElements.SingleOrDefault(d => d.ElementName == "Initials");
            var datasetCategoryElement = new DatasetCategoryElement { DatasetCategory = datasetCategory, DatasetElement = datasetElement, FieldOrder = 1 };
            context.DatasetCategoryElements.Add(datasetCategoryElement);

            datasetElement = context.DatasetElements.SingleOrDefault(d => d.ElementName == "Date of Birth");
            datasetCategoryElement = new DatasetCategoryElement { DatasetCategory = datasetCategory, DatasetElement = datasetElement, FieldOrder = 2 };
            context.DatasetCategoryElements.Add(datasetCategoryElement);

            datasetElement = context.DatasetElements.SingleOrDefault(d => d.ElementName == "Age");
            datasetCategoryElement = new DatasetCategoryElement { DatasetCategory = datasetCategory, DatasetElement = datasetElement, FieldOrder = 3 };
            context.DatasetCategoryElements.Add(datasetCategoryElement);

            datasetElement = context.DatasetElements.SingleOrDefault(d => d.ElementName == "Age Unit");
            datasetCategoryElement = new DatasetCategoryElement { DatasetCategory = datasetCategory, DatasetElement = datasetElement, FieldOrder = 4 };
            context.DatasetCategoryElements.Add(datasetCategoryElement);

            datasetElement = context.DatasetElements.SingleOrDefault(d => d.ElementName == "Weight (kg)");
            datasetCategoryElement = new DatasetCategoryElement { DatasetCategory = datasetCategory, DatasetElement = datasetElement, FieldOrder = 5 };
            context.DatasetCategoryElements.Add(datasetCategoryElement);

            datasetElement = context.DatasetElements.SingleOrDefault(d => d.ElementName == "Sex");
            datasetCategoryElement = new DatasetCategoryElement { DatasetCategory = datasetCategory, DatasetElement = datasetElement, FieldOrder = 6 };
            context.DatasetCategoryElements.Add(datasetCategoryElement);

            datasetElement = context.DatasetElements.SingleOrDefault(d => d.ElementName == "Ethnic Group");
            datasetCategoryElement = new DatasetCategoryElement { DatasetCategory = datasetCategory, DatasetElement = datasetElement, FieldOrder = 7 };
            context.DatasetCategoryElements.Add(datasetCategoryElement);

            datasetElement = context.DatasetElements.SingleOrDefault(d => d.ElementName == "Name");
            datasetCategoryElement = new DatasetCategoryElement { DatasetCategory = datasetCategory, DatasetElement = datasetElement, FieldOrder = 8 };
            context.DatasetCategoryElements.Add(datasetCategoryElement);

            datasetElement = context.DatasetElements.SingleOrDefault(d => d.ElementName == "Identification Type");
            datasetCategoryElement = new DatasetCategoryElement { DatasetCategory = datasetCategory, DatasetElement = datasetElement, FieldOrder = 9 };
            context.DatasetCategoryElements.Add(datasetCategoryElement);

            datasetElement = context.DatasetElements.SingleOrDefault(d => d.ElementName == "Identification Number");
            datasetCategoryElement = new DatasetCategoryElement { DatasetCategory = datasetCategory, DatasetElement = datasetElement, FieldOrder = 10 };
            context.DatasetCategoryElements.Add(datasetCategoryElement);

            datasetCategory = new DatasetCategory { CategoryOrder = 2, Dataset = dataset, DatasetCategoryName = "Details of Adverse Drug Reaction" };
            context.DatasetCategories.Add(datasetCategory);

            datasetElement = context.DatasetElements.SingleOrDefault(d => d.ElementName == "Date of Onset");
            datasetCategoryElement = new DatasetCategoryElement { DatasetCategory = datasetCategory, DatasetElement = datasetElement, FieldOrder = 1 };
            context.DatasetCategoryElements.Add(datasetCategoryElement);

            datasetElement = context.DatasetElements.SingleOrDefault(d => d.ElementName == "Date of Onset Estimated");
            datasetCategoryElement = new DatasetCategoryElement { DatasetCategory = datasetCategory, DatasetElement = datasetElement, FieldOrder = 2 };
            context.DatasetCategoryElements.Add(datasetCategoryElement);

            datasetElement = context.DatasetElements.SingleOrDefault(d => d.ElementName == "ADR Outcome");
            datasetCategoryElement = new DatasetCategoryElement { DatasetCategory = datasetCategory, DatasetElement = datasetElement, FieldOrder = 3 };
            context.DatasetCategoryElements.Add(datasetCategoryElement);

            datasetElement = context.DatasetElements.SingleOrDefault(d => d.ElementName == "Date of Death");
            datasetCategoryElement = new DatasetCategoryElement { DatasetCategory = datasetCategory, DatasetElement = datasetElement, FieldOrder = 4 };
            context.DatasetCategoryElements.Add(datasetCategoryElement);

            datasetElement = context.DatasetElements.SingleOrDefault(d => d.ElementName == "Date of Recovery");
            datasetCategoryElement = new DatasetCategoryElement { DatasetCategory = datasetCategory, DatasetElement = datasetElement, FieldOrder = 5 };
            context.DatasetCategoryElements.Add(datasetCategoryElement);

            datasetElement = context.DatasetElements.SingleOrDefault(d => d.ElementName == "ADR Description");
            datasetCategoryElement = new DatasetCategoryElement { DatasetCategory = datasetCategory, DatasetElement = datasetElement, FieldOrder = 6 };
            context.DatasetCategoryElements.Add(datasetCategoryElement);

            datasetCategory = new DatasetCategory { CategoryOrder = 3, Dataset = dataset, DatasetCategoryName = "Suspected Drug Details" };
            context.DatasetCategories.Add(datasetCategory);

            datasetElement = context.DatasetElements.SingleOrDefault(d => d.ElementName == "Suspected Drug 1");
            datasetCategoryElement = new DatasetCategoryElement { DatasetCategory = datasetCategory, DatasetElement = datasetElement, FieldOrder = 1 };
            context.DatasetCategoryElements.Add(datasetCategoryElement);

            datasetElement = context.DatasetElements.SingleOrDefault(d => d.ElementName == "Suspected Drug 1 Type");
            datasetCategoryElement = new DatasetCategoryElement { DatasetCategory = datasetCategory, DatasetElement = datasetElement, FieldOrder = 2 };
            context.DatasetCategoryElements.Add(datasetCategoryElement);

            datasetElement = context.DatasetElements.SingleOrDefault(d => d.ElementName == "Suspected Drug 1 Dosage");
            datasetCategoryElement = new DatasetCategoryElement { DatasetCategory = datasetCategory, DatasetElement = datasetElement, FieldOrder = 3 };
            context.DatasetCategoryElements.Add(datasetCategoryElement);

            datasetElement = context.DatasetElements.SingleOrDefault(d => d.ElementName == "Suspected Drug 1 Dosage Unit");
            datasetCategoryElement = new DatasetCategoryElement { DatasetCategory = datasetCategory, DatasetElement = datasetElement, FieldOrder = 4 };
            context.DatasetCategoryElements.Add(datasetCategoryElement);

            datasetElement = context.DatasetElements.SingleOrDefault(d => d.ElementName == "Suspected Drug 1 Frequency");
            datasetCategoryElement = new DatasetCategoryElement { DatasetCategory = datasetCategory, DatasetElement = datasetElement, FieldOrder = 5 };
            context.DatasetCategoryElements.Add(datasetCategoryElement);

            datasetElement = context.DatasetElements.SingleOrDefault(d => d.ElementName == "Suspected Drug 1 Route");
            datasetCategoryElement = new DatasetCategoryElement { DatasetCategory = datasetCategory, DatasetElement = datasetElement, FieldOrder = 6 };
            context.DatasetCategoryElements.Add(datasetCategoryElement);

            datasetElement = context.DatasetElements.SingleOrDefault(d => d.ElementName == "Suspected Drug 1 Date Started");
            datasetCategoryElement = new DatasetCategoryElement { DatasetCategory = datasetCategory, DatasetElement = datasetElement, FieldOrder = 7 };
            context.DatasetCategoryElements.Add(datasetCategoryElement);

            datasetElement = context.DatasetElements.SingleOrDefault(d => d.ElementName == "Suspected Drug 1 Date Stopped");
            datasetCategoryElement = new DatasetCategoryElement { DatasetCategory = datasetCategory, DatasetElement = datasetElement, FieldOrder = 8 };
            context.DatasetCategoryElements.Add(datasetCategoryElement);

            datasetElement = context.DatasetElements.SingleOrDefault(d => d.ElementName == "Suspected Drug 1 Indication");
            datasetCategoryElement = new DatasetCategoryElement { DatasetCategory = datasetCategory, DatasetElement = datasetElement, FieldOrder = 9 };
            context.DatasetCategoryElements.Add(datasetCategoryElement);

            datasetElement = context.DatasetElements.SingleOrDefault(d => d.ElementName == "Suspected Drug 1 Batch Number");
            datasetCategoryElement = new DatasetCategoryElement { DatasetCategory = datasetCategory, DatasetElement = datasetElement, FieldOrder = 10 };
            context.DatasetCategoryElements.Add(datasetCategoryElement);

            datasetElement = context.DatasetElements.SingleOrDefault(d => d.ElementName == "Suspected Drug 1 Duration");
            datasetCategoryElement = new DatasetCategoryElement { DatasetCategory = datasetCategory, DatasetElement = datasetElement, FieldOrder = 11 };
            context.DatasetCategoryElements.Add(datasetCategoryElement);

            datasetElement = context.DatasetElements.SingleOrDefault(d => d.ElementName == "Suspected Drug 2");
            datasetCategoryElement = new DatasetCategoryElement { DatasetCategory = datasetCategory, DatasetElement = datasetElement, FieldOrder = 12 };
            context.DatasetCategoryElements.Add(datasetCategoryElement);

            datasetElement = context.DatasetElements.SingleOrDefault(d => d.ElementName == "Suspected Drug 2 Type");
            datasetCategoryElement = new DatasetCategoryElement { DatasetCategory = datasetCategory, DatasetElement = datasetElement, FieldOrder = 13 };
            context.DatasetCategoryElements.Add(datasetCategoryElement);

            datasetElement = context.DatasetElements.SingleOrDefault(d => d.ElementName == "Suspected Drug 2 Dosage");
            datasetCategoryElement = new DatasetCategoryElement { DatasetCategory = datasetCategory, DatasetElement = datasetElement, FieldOrder = 14 };
            context.DatasetCategoryElements.Add(datasetCategoryElement);

            datasetElement = context.DatasetElements.SingleOrDefault(d => d.ElementName == "Suspected Drug 2 Dosage Unit");
            datasetCategoryElement = new DatasetCategoryElement { DatasetCategory = datasetCategory, DatasetElement = datasetElement, FieldOrder = 15 };
            context.DatasetCategoryElements.Add(datasetCategoryElement);

            datasetElement = context.DatasetElements.SingleOrDefault(d => d.ElementName == "Suspected Drug 2 Frequency");
            datasetCategoryElement = new DatasetCategoryElement { DatasetCategory = datasetCategory, DatasetElement = datasetElement, FieldOrder = 16 };
            context.DatasetCategoryElements.Add(datasetCategoryElement);

            datasetElement = context.DatasetElements.SingleOrDefault(d => d.ElementName == "Suspected Drug 2 Route");
            datasetCategoryElement = new DatasetCategoryElement { DatasetCategory = datasetCategory, DatasetElement = datasetElement, FieldOrder = 17 };
            context.DatasetCategoryElements.Add(datasetCategoryElement);

            datasetElement = context.DatasetElements.SingleOrDefault(d => d.ElementName == "Suspected Drug 2 Date Started");
            datasetCategoryElement = new DatasetCategoryElement { DatasetCategory = datasetCategory, DatasetElement = datasetElement, FieldOrder = 18 };
            context.DatasetCategoryElements.Add(datasetCategoryElement);

            datasetElement = context.DatasetElements.SingleOrDefault(d => d.ElementName == "Suspected Drug 2 Date Stopped");
            datasetCategoryElement = new DatasetCategoryElement { DatasetCategory = datasetCategory, DatasetElement = datasetElement, FieldOrder = 19 };
            context.DatasetCategoryElements.Add(datasetCategoryElement);

            datasetElement = context.DatasetElements.SingleOrDefault(d => d.ElementName == "Suspected Drug 2 Indication");
            datasetCategoryElement = new DatasetCategoryElement { DatasetCategory = datasetCategory, DatasetElement = datasetElement, FieldOrder = 20 };
            context.DatasetCategoryElements.Add(datasetCategoryElement);

            datasetElement = context.DatasetElements.SingleOrDefault(d => d.ElementName == "Suspected Drug 2 Batch Number");
            datasetCategoryElement = new DatasetCategoryElement { DatasetCategory = datasetCategory, DatasetElement = datasetElement, FieldOrder = 21 };
            context.DatasetCategoryElements.Add(datasetCategoryElement);

            datasetElement = context.DatasetElements.SingleOrDefault(d => d.ElementName == "Suspected Drug 2 Duration");
            datasetCategoryElement = new DatasetCategoryElement { DatasetCategory = datasetCategory, DatasetElement = datasetElement, FieldOrder = 22 };
            context.DatasetCategoryElements.Add(datasetCategoryElement);

            datasetElement = context.DatasetElements.SingleOrDefault(d => d.ElementName == "Suspected Drug 3");
            datasetCategoryElement = new DatasetCategoryElement { DatasetCategory = datasetCategory, DatasetElement = datasetElement, FieldOrder = 23 };
            context.DatasetCategoryElements.Add(datasetCategoryElement);

            datasetElement = context.DatasetElements.SingleOrDefault(d => d.ElementName == "Suspected Drug 3 Type");
            datasetCategoryElement = new DatasetCategoryElement { DatasetCategory = datasetCategory, DatasetElement = datasetElement, FieldOrder = 24 };
            context.DatasetCategoryElements.Add(datasetCategoryElement);

            datasetElement = context.DatasetElements.SingleOrDefault(d => d.ElementName == "Suspected Drug 3 Dosage");
            datasetCategoryElement = new DatasetCategoryElement { DatasetCategory = datasetCategory, DatasetElement = datasetElement, FieldOrder = 25 };
            context.DatasetCategoryElements.Add(datasetCategoryElement);

            datasetElement = context.DatasetElements.SingleOrDefault(d => d.ElementName == "Suspected Drug 3 Dosage Unit");
            datasetCategoryElement = new DatasetCategoryElement { DatasetCategory = datasetCategory, DatasetElement = datasetElement, FieldOrder = 26 };
            context.DatasetCategoryElements.Add(datasetCategoryElement);

            datasetElement = context.DatasetElements.SingleOrDefault(d => d.ElementName == "Suspected Drug 3 Frequency");
            datasetCategoryElement = new DatasetCategoryElement { DatasetCategory = datasetCategory, DatasetElement = datasetElement, FieldOrder = 27 };
            context.DatasetCategoryElements.Add(datasetCategoryElement);

            datasetElement = context.DatasetElements.SingleOrDefault(d => d.ElementName == "Suspected Drug 3 Route");
            datasetCategoryElement = new DatasetCategoryElement { DatasetCategory = datasetCategory, DatasetElement = datasetElement, FieldOrder = 28 };
            context.DatasetCategoryElements.Add(datasetCategoryElement);

            datasetElement = context.DatasetElements.SingleOrDefault(d => d.ElementName == "Suspected Drug 3 Date Started");
            datasetCategoryElement = new DatasetCategoryElement { DatasetCategory = datasetCategory, DatasetElement = datasetElement, FieldOrder = 29 };
            context.DatasetCategoryElements.Add(datasetCategoryElement);

            datasetElement = context.DatasetElements.SingleOrDefault(d => d.ElementName == "Suspected Drug 3 Date Stopped");
            datasetCategoryElement = new DatasetCategoryElement { DatasetCategory = datasetCategory, DatasetElement = datasetElement, FieldOrder = 30 };
            context.DatasetCategoryElements.Add(datasetCategoryElement);

            datasetElement = context.DatasetElements.SingleOrDefault(d => d.ElementName == "Suspected Drug 3 Indication");
            datasetCategoryElement = new DatasetCategoryElement { DatasetCategory = datasetCategory, DatasetElement = datasetElement, FieldOrder = 31 };
            context.DatasetCategoryElements.Add(datasetCategoryElement);

            datasetElement = context.DatasetElements.SingleOrDefault(d => d.ElementName == "Suspected Drug 3 Batch Number");
            datasetCategoryElement = new DatasetCategoryElement { DatasetCategory = datasetCategory, DatasetElement = datasetElement, FieldOrder = 32 };
            context.DatasetCategoryElements.Add(datasetCategoryElement);

            datasetElement = context.DatasetElements.SingleOrDefault(d => d.ElementName == "Suspected Drug 3 Duration");
            datasetCategoryElement = new DatasetCategoryElement { DatasetCategory = datasetCategory, DatasetElement = datasetElement, FieldOrder = 33 };
            context.DatasetCategoryElements.Add(datasetCategoryElement);

            datasetCategory = new DatasetCategory { CategoryOrder = 4, Dataset = dataset, DatasetCategoryName = "Concomitant Drug Details" };
            context.DatasetCategories.Add(datasetCategory);

            datasetElement = context.DatasetElements.SingleOrDefault(d => d.ElementName == "Concomitant Drug 1");
            datasetCategoryElement = new DatasetCategoryElement { DatasetCategory = datasetCategory, DatasetElement = datasetElement, FieldOrder = 1 };
            context.DatasetCategoryElements.Add(datasetCategoryElement);

            datasetElement = context.DatasetElements.SingleOrDefault(d => d.ElementName == "Concomitant Drug 1 Type");
            datasetCategoryElement = new DatasetCategoryElement { DatasetCategory = datasetCategory, DatasetElement = datasetElement, FieldOrder = 2 };
            context.DatasetCategoryElements.Add(datasetCategoryElement);

            datasetElement = context.DatasetElements.SingleOrDefault(d => d.ElementName == "Concomitant Drug 1 Dosage");
            datasetCategoryElement = new DatasetCategoryElement { DatasetCategory = datasetCategory, DatasetElement = datasetElement, FieldOrder = 3 };
            context.DatasetCategoryElements.Add(datasetCategoryElement);

            datasetElement = context.DatasetElements.SingleOrDefault(d => d.ElementName == "Concomitant Drug 1 Dosage Unit");
            datasetCategoryElement = new DatasetCategoryElement { DatasetCategory = datasetCategory, DatasetElement = datasetElement, FieldOrder = 4 };
            context.DatasetCategoryElements.Add(datasetCategoryElement);

            datasetElement = context.DatasetElements.SingleOrDefault(d => d.ElementName == "Concomitant Drug 1 Frequency");
            datasetCategoryElement = new DatasetCategoryElement { DatasetCategory = datasetCategory, DatasetElement = datasetElement, FieldOrder = 5 };
            context.DatasetCategoryElements.Add(datasetCategoryElement);

            datasetElement = context.DatasetElements.SingleOrDefault(d => d.ElementName == "Concomitant Drug 1 Route");
            datasetCategoryElement = new DatasetCategoryElement { DatasetCategory = datasetCategory, DatasetElement = datasetElement, FieldOrder = 6 };
            context.DatasetCategoryElements.Add(datasetCategoryElement);

            datasetElement = context.DatasetElements.SingleOrDefault(d => d.ElementName == "Concomitant Drug 1 Date Started");
            datasetCategoryElement = new DatasetCategoryElement { DatasetCategory = datasetCategory, DatasetElement = datasetElement, FieldOrder = 7 };
            context.DatasetCategoryElements.Add(datasetCategoryElement);

            datasetElement = context.DatasetElements.SingleOrDefault(d => d.ElementName == "Concomitant Drug 1 Date Stopped");
            datasetCategoryElement = new DatasetCategoryElement { DatasetCategory = datasetCategory, DatasetElement = datasetElement, FieldOrder = 8 };
            context.DatasetCategoryElements.Add(datasetCategoryElement);

            datasetElement = context.DatasetElements.SingleOrDefault(d => d.ElementName == "Concomitant Drug 1 Indication");
            datasetCategoryElement = new DatasetCategoryElement { DatasetCategory = datasetCategory, DatasetElement = datasetElement, FieldOrder = 9 };
            context.DatasetCategoryElements.Add(datasetCategoryElement);

            datasetElement = context.DatasetElements.SingleOrDefault(d => d.ElementName == "Concomitant Drug 1 Batch Number");
            datasetCategoryElement = new DatasetCategoryElement { DatasetCategory = datasetCategory, DatasetElement = datasetElement, FieldOrder = 10 };
            context.DatasetCategoryElements.Add(datasetCategoryElement);

            datasetElement = context.DatasetElements.SingleOrDefault(d => d.ElementName == "Concomitant Drug 1 Duration");
            datasetCategoryElement = new DatasetCategoryElement { DatasetCategory = datasetCategory, DatasetElement = datasetElement, FieldOrder = 11 };
            context.DatasetCategoryElements.Add(datasetCategoryElement);

            datasetElement = context.DatasetElements.SingleOrDefault(d => d.ElementName == "Concomitant Drug 2");
            datasetCategoryElement = new DatasetCategoryElement { DatasetCategory = datasetCategory, DatasetElement = datasetElement, FieldOrder = 12 };
            context.DatasetCategoryElements.Add(datasetCategoryElement);

            datasetElement = context.DatasetElements.SingleOrDefault(d => d.ElementName == "Concomitant Drug 2 Type");
            datasetCategoryElement = new DatasetCategoryElement { DatasetCategory = datasetCategory, DatasetElement = datasetElement, FieldOrder = 13 };
            context.DatasetCategoryElements.Add(datasetCategoryElement);

            datasetElement = context.DatasetElements.SingleOrDefault(d => d.ElementName == "Concomitant Drug 2 Dosage");
            datasetCategoryElement = new DatasetCategoryElement { DatasetCategory = datasetCategory, DatasetElement = datasetElement, FieldOrder = 14 };
            context.DatasetCategoryElements.Add(datasetCategoryElement);

            datasetElement = context.DatasetElements.SingleOrDefault(d => d.ElementName == "Concomitant Drug 2 Dosage Unit");
            datasetCategoryElement = new DatasetCategoryElement { DatasetCategory = datasetCategory, DatasetElement = datasetElement, FieldOrder = 15 };
            context.DatasetCategoryElements.Add(datasetCategoryElement);

            datasetElement = context.DatasetElements.SingleOrDefault(d => d.ElementName == "Concomitant Drug 2 Frequency");
            datasetCategoryElement = new DatasetCategoryElement { DatasetCategory = datasetCategory, DatasetElement = datasetElement, FieldOrder = 16 };
            context.DatasetCategoryElements.Add(datasetCategoryElement);

            datasetElement = context.DatasetElements.SingleOrDefault(d => d.ElementName == "Concomitant Drug 2 Route");
            datasetCategoryElement = new DatasetCategoryElement { DatasetCategory = datasetCategory, DatasetElement = datasetElement, FieldOrder = 17 };
            context.DatasetCategoryElements.Add(datasetCategoryElement);

            datasetElement = context.DatasetElements.SingleOrDefault(d => d.ElementName == "Concomitant Drug 2 Date Started");
            datasetCategoryElement = new DatasetCategoryElement { DatasetCategory = datasetCategory, DatasetElement = datasetElement, FieldOrder = 18 };
            context.DatasetCategoryElements.Add(datasetCategoryElement);

            datasetElement = context.DatasetElements.SingleOrDefault(d => d.ElementName == "Concomitant Drug 2 Date Stopped");
            datasetCategoryElement = new DatasetCategoryElement { DatasetCategory = datasetCategory, DatasetElement = datasetElement, FieldOrder = 19 };
            context.DatasetCategoryElements.Add(datasetCategoryElement);

            datasetElement = context.DatasetElements.SingleOrDefault(d => d.ElementName == "Concomitant Drug 2 Indication");
            datasetCategoryElement = new DatasetCategoryElement { DatasetCategory = datasetCategory, DatasetElement = datasetElement, FieldOrder = 20 };
            context.DatasetCategoryElements.Add(datasetCategoryElement);

            datasetElement = context.DatasetElements.SingleOrDefault(d => d.ElementName == "Concomitant Drug 2 Batch Number");
            datasetCategoryElement = new DatasetCategoryElement { DatasetCategory = datasetCategory, DatasetElement = datasetElement, FieldOrder = 21 };
            context.DatasetCategoryElements.Add(datasetCategoryElement);

            datasetElement = context.DatasetElements.SingleOrDefault(d => d.ElementName == "Concomitant Drug 2 Duration");
            datasetCategoryElement = new DatasetCategoryElement { DatasetCategory = datasetCategory, DatasetElement = datasetElement, FieldOrder = 22 };
            context.DatasetCategoryElements.Add(datasetCategoryElement);

            datasetElement = context.DatasetElements.SingleOrDefault(d => d.ElementName == "Concomitant Drug 3");
            datasetCategoryElement = new DatasetCategoryElement { DatasetCategory = datasetCategory, DatasetElement = datasetElement, FieldOrder = 23 };
            context.DatasetCategoryElements.Add(datasetCategoryElement);

            datasetElement = context.DatasetElements.SingleOrDefault(d => d.ElementName == "Concomitant Drug 3 Type");
            datasetCategoryElement = new DatasetCategoryElement { DatasetCategory = datasetCategory, DatasetElement = datasetElement, FieldOrder = 24 };
            context.DatasetCategoryElements.Add(datasetCategoryElement);

            datasetElement = context.DatasetElements.SingleOrDefault(d => d.ElementName == "Concomitant Drug 3 Dosage");
            datasetCategoryElement = new DatasetCategoryElement { DatasetCategory = datasetCategory, DatasetElement = datasetElement, FieldOrder = 25 };
            context.DatasetCategoryElements.Add(datasetCategoryElement);

            datasetElement = context.DatasetElements.SingleOrDefault(d => d.ElementName == "Concomitant Drug 3 Dosage Unit");
            datasetCategoryElement = new DatasetCategoryElement { DatasetCategory = datasetCategory, DatasetElement = datasetElement, FieldOrder = 26 };
            context.DatasetCategoryElements.Add(datasetCategoryElement);

            datasetElement = context.DatasetElements.SingleOrDefault(d => d.ElementName == "Concomitant Drug 3 Frequency");
            datasetCategoryElement = new DatasetCategoryElement { DatasetCategory = datasetCategory, DatasetElement = datasetElement, FieldOrder = 27 };
            context.DatasetCategoryElements.Add(datasetCategoryElement);

            datasetElement = context.DatasetElements.SingleOrDefault(d => d.ElementName == "Concomitant Drug 3 Route");
            datasetCategoryElement = new DatasetCategoryElement { DatasetCategory = datasetCategory, DatasetElement = datasetElement, FieldOrder = 28 };
            context.DatasetCategoryElements.Add(datasetCategoryElement);

            datasetElement = context.DatasetElements.SingleOrDefault(d => d.ElementName == "Concomitant Drug 3 Date Started");
            datasetCategoryElement = new DatasetCategoryElement { DatasetCategory = datasetCategory, DatasetElement = datasetElement, FieldOrder = 29 };
            context.DatasetCategoryElements.Add(datasetCategoryElement);

            datasetElement = context.DatasetElements.SingleOrDefault(d => d.ElementName == "Concomitant Drug 3 Date Stopped");
            datasetCategoryElement = new DatasetCategoryElement { DatasetCategory = datasetCategory, DatasetElement = datasetElement, FieldOrder = 30 };
            context.DatasetCategoryElements.Add(datasetCategoryElement);

            datasetElement = context.DatasetElements.SingleOrDefault(d => d.ElementName == "Concomitant Drug 3 Indication");
            datasetCategoryElement = new DatasetCategoryElement { DatasetCategory = datasetCategory, DatasetElement = datasetElement, FieldOrder = 31 };
            context.DatasetCategoryElements.Add(datasetCategoryElement);

            datasetElement = context.DatasetElements.SingleOrDefault(d => d.ElementName == "Concomitant Drug 3 Batch Number");
            datasetCategoryElement = new DatasetCategoryElement { DatasetCategory = datasetCategory, DatasetElement = datasetElement, FieldOrder = 32 };
            context.DatasetCategoryElements.Add(datasetCategoryElement);

            datasetElement = context.DatasetElements.SingleOrDefault(d => d.ElementName == "Concomitant Drug 3 Duration");
            datasetCategoryElement = new DatasetCategoryElement { DatasetCategory = datasetCategory, DatasetElement = datasetElement, FieldOrder = 33 };
            context.DatasetCategoryElements.Add(datasetCategoryElement);

            datasetCategory = new DatasetCategory { CategoryOrder = 5, Dataset = dataset, DatasetCategoryName = "Other Relevant Information" };
            context.DatasetCategories.Add(datasetCategory);

            datasetElement = context.DatasetElements.SingleOrDefault(d => d.ElementName == "Other Relevant Information");
            datasetCategoryElement = new DatasetCategoryElement { DatasetCategory = datasetCategory, DatasetElement = datasetElement, FieldOrder = 1 };
            context.DatasetCategoryElements.Add(datasetCategoryElement);

            datasetCategory = new DatasetCategory { CategoryOrder = 6, Dataset = dataset, DatasetCategoryName = "Management of Reaction" };
            context.DatasetCategories.Add(datasetCategory);

            datasetElement = context.DatasetElements.SingleOrDefault(d => d.ElementName == "Hospitalisation");
            datasetCategoryElement = new DatasetCategoryElement { DatasetCategory = datasetCategory, DatasetElement = datasetElement, FieldOrder = 1 };
            context.DatasetCategoryElements.Add(datasetCategoryElement);

            datasetElement = context.DatasetElements.SingleOrDefault(d => d.ElementName == "Causality");
            datasetCategoryElement = new DatasetCategoryElement { DatasetCategory = datasetCategory, DatasetElement = datasetElement, FieldOrder = 2 };
            context.DatasetCategoryElements.Add(datasetCategoryElement);

            datasetElement = context.DatasetElements.SingleOrDefault(d => d.ElementName == "Serious Reaction");
            datasetCategoryElement = new DatasetCategoryElement { DatasetCategory = datasetCategory, DatasetElement = datasetElement, FieldOrder = 3 };
            context.DatasetCategoryElements.Add(datasetCategoryElement);

            datasetElement = context.DatasetElements.SingleOrDefault(d => d.ElementName == "Serious Reaction Reason");
            datasetCategoryElement = new DatasetCategoryElement { DatasetCategory = datasetCategory, DatasetElement = datasetElement, FieldOrder = 4 };
            context.DatasetCategoryElements.Add(datasetCategoryElement);

            datasetElement = context.DatasetElements.SingleOrDefault(d => d.ElementName == "Medically Significant");
            datasetCategoryElement = new DatasetCategoryElement { DatasetCategory = datasetCategory, DatasetElement = datasetElement, FieldOrder = 5 };
            context.DatasetCategoryElements.Add(datasetCategoryElement);

            datasetElement = context.DatasetElements.SingleOrDefault(d => d.ElementName == "Sequelae (any permanent complications or injuries as a result of the ADR)?");
            datasetCategoryElement = new DatasetCategoryElement { DatasetCategory = datasetCategory, DatasetElement = datasetElement, FieldOrder = 6 };
            context.DatasetCategoryElements.Add(datasetCategoryElement);

            datasetElement = context.DatasetElements.SingleOrDefault(d => d.ElementName == "Was treatment given?");
            datasetCategoryElement = new DatasetCategoryElement { DatasetCategory = datasetCategory, DatasetElement = datasetElement, FieldOrder = 7 };
            context.DatasetCategoryElements.Add(datasetCategoryElement);

            datasetElement = context.DatasetElements.SingleOrDefault(d => d.ElementName == "Treatment Given");
            datasetCategoryElement = new DatasetCategoryElement { DatasetCategory = datasetCategory, DatasetElement = datasetElement, FieldOrder = 8 };
            context.DatasetCategoryElements.Add(datasetCategoryElement);

            datasetCategory = new DatasetCategory { CategoryOrder = 7, Dataset = dataset, DatasetCategoryName = "Your Particulars" };
            context.DatasetCategories.Add(datasetCategory);

            datasetElement = context.DatasetElements.SingleOrDefault(d => d.ElementName == "Reporter Name");
            datasetCategoryElement = new DatasetCategoryElement { DatasetCategory = datasetCategory, DatasetElement = datasetElement, FieldOrder = 1 };
            context.DatasetCategoryElements.Add(datasetCategoryElement);

            datasetElement = context.DatasetElements.SingleOrDefault(d => d.ElementName == "Reporter Profession");
            datasetCategoryElement = new DatasetCategoryElement { DatasetCategory = datasetCategory, DatasetElement = datasetElement, FieldOrder = 2 };
            context.DatasetCategoryElements.Add(datasetCategoryElement);

            datasetElement = context.DatasetElements.SingleOrDefault(d => d.ElementName == "Report Reference Number");
            datasetCategoryElement = new DatasetCategoryElement { DatasetCategory = datasetCategory, DatasetElement = datasetElement, FieldOrder = 3 };
            context.DatasetCategoryElements.Add(datasetCategoryElement);

            datasetElement = context.DatasetElements.SingleOrDefault(d => d.ElementName == "Reporter Telephone Number");
            datasetCategoryElement = new DatasetCategoryElement { DatasetCategory = datasetCategory, DatasetElement = datasetElement, FieldOrder = 4 };
            context.DatasetCategoryElements.Add(datasetCategoryElement);

            datasetElement = context.DatasetElements.SingleOrDefault(d => d.ElementName == "Reporter Fax Number");
            datasetCategoryElement = new DatasetCategoryElement { DatasetCategory = datasetCategory, DatasetElement = datasetElement, FieldOrder = 5 };
            context.DatasetCategoryElements.Add(datasetCategoryElement);

            datasetElement = context.DatasetElements.SingleOrDefault(d => d.ElementName == "Reporter Email");
            datasetCategoryElement = new DatasetCategoryElement { DatasetCategory = datasetCategory, DatasetElement = datasetElement, FieldOrder = 6 };
            context.DatasetCategoryElements.Add(datasetCategoryElement);

            datasetElement = context.DatasetElements.SingleOrDefault(d => d.ElementName == "Reporter Place of Practise");
            datasetCategoryElement = new DatasetCategoryElement { DatasetCategory = datasetCategory, DatasetElement = datasetElement, FieldOrder = 7 };
            context.DatasetCategoryElements.Add(datasetCategoryElement);

            datasetElement = context.DatasetElements.SingleOrDefault(d => d.ElementName == "Reporter Address");
            datasetCategoryElement = new DatasetCategoryElement { DatasetCategory = datasetCategory, DatasetElement = datasetElement, FieldOrder = 8 };
            context.DatasetCategoryElements.Add(datasetCategoryElement);

            context.SaveChanges();
        }

        private static void CreateE2BDatasetElements(PVIMSDbContext context)
        {
            var elementType = context.DatasetElementTypes.SingleOrDefault(u => u.Description == "Generic");

            FieldType fieldType;
            Field field;
            DatasetElement element;
            DatasetElementSub elementSub;

            //fieldType = context.FieldTypes.SingleOrDefault(u => u.Description == "NumericTextbox");
            //field = new Field() { Anonymise = false, Mandatory = false, FieldType = fieldType, Decimals = 0 };
            //element = new DatasetElement { System = true, DatasetElementType = elementType, ElementName = "N.1.1 Type of Messages in Batch", Field = field, OID = "2.16.840.1.113883.3.989.2.1.1.1", DefaultValue = "1" };
            //context.DatasetElements.Add(element);

            //fieldType = context.FieldTypes.SingleOrDefault(u => u.Description == "AlphaNumericTextbox");
            //field = new Field() { Anonymise = false, Mandatory = false, FieldType = fieldType, MaxLength = 100 };
            //element = new DatasetElement { System = true, DatasetElementType = elementType, ElementName = "N.1.2 Batch Number", Field = field, OID = "2.16.840.1.113883.3.989.2.1.3.22" };
            //context.DatasetElements.Add(element);

            //fieldType = context.FieldTypes.SingleOrDefault(u => u.Description == "AlphaNumericTextbox");
            //field = new Field() { Anonymise = false, Mandatory = false, FieldType = fieldType, MaxLength = 60 };
            //element = new DatasetElement { System = true, DatasetElementType = elementType, ElementName = "N.1.3 Batch Sender Identifier", Field = field, OID = "2.16.840.1.113883.3.989.2.1.3.13", DefaultValue = "MSH.PViMS" };
            //context.DatasetElements.Add(element);

            //fieldType = context.FieldTypes.SingleOrDefault(u => u.Description == "AlphaNumericTextbox");
            //field = new Field() { Anonymise = false, Mandatory = false, FieldType = fieldType, MaxLength = 60 };
            //element = new DatasetElement { System = true, DatasetElementType = elementType, ElementName = "N.1.4 Batch Receiver Identifier", Field = field, OID = "2.16.840.1.113883.3.989.2.1.3.14", DefaultValue = "ICHTEST" };
            //context.DatasetElements.Add(element);

            //fieldType = context.FieldTypes.SingleOrDefault(u => u.Description == "Date");
            //field = new Field() { Anonymise = false, Mandatory = false, FieldType = fieldType };
            //element = new DatasetElement { System = true, DatasetElementType = elementType, ElementName = "N.1.5 Date of Batch Transmission", Field = field };
            //context.DatasetElements.Add(element);

            //fieldType = context.FieldTypes.SingleOrDefault(u => u.Description == "AlphaNumericTextbox");
            //field = new Field() { Anonymise = false, Mandatory = false, FieldType = fieldType, MaxLength = 100 };
            //element = new DatasetElement { System = true, DatasetElementType = elementType, ElementName = "N.2.r.1 Message Identifier", Field = field, OID = "2.16.840.1.113883.3.989.2.1.3.1" };
            //context.DatasetElements.Add(element);

            //fieldType = context.FieldTypes.SingleOrDefault(u => u.Description == "AlphaNumericTextbox");
            //field = new Field() { Anonymise = false, Mandatory = false, FieldType = fieldType, MaxLength = 60 };
            //element = new DatasetElement { System = true, DatasetElementType = elementType, ElementName = "N.2.r.2 Message Sender Identifier", Field = field, OID = "2.16.840.1.113883.3.989.2.1.3.11", DefaultValue = "MSH.PViMS" };
            //context.DatasetElements.Add(element);

            //fieldType = context.FieldTypes.SingleOrDefault(u => u.Description == "AlphaNumericTextbox");
            //field = new Field() { Anonymise = false, Mandatory = false, FieldType = fieldType, MaxLength = 60 };
            //element = new DatasetElement { System = true, DatasetElementType = elementType, ElementName = "N.2.r.3 Message Receiver Identifier", Field = field, OID = "2.16.840.1.113883.3.989.2.1.3.12", DefaultValue = "ICHTEST" };
            //context.DatasetElements.Add(element);

            //fieldType = context.FieldTypes.SingleOrDefault(u => u.Description == "Date");
            //field = new Field() { Anonymise = false, Mandatory = false, FieldType = fieldType };
            //element = new DatasetElement { System = true, DatasetElementType = elementType, ElementName = "N.2.r.4 Date of Message Creation", Field = field };
            //context.DatasetElements.Add(element);

            //fieldType = context.FieldTypes.SingleOrDefault(u => u.Description == "AlphaNumericTextbox");
            //field = new Field() { Anonymise = false, Mandatory = false, FieldType = fieldType, MaxLength = 100 };
            //element = new DatasetElement { System = true, DatasetElementType = elementType, ElementName = "C.1.1 Sender’s (case) Safety Report Unique Identifier", Field = field, OID = "2.16.840.1.113883.3.989.2.1.3.1" };
            //context.DatasetElements.Add(element);

            //fieldType = context.FieldTypes.SingleOrDefault(u => u.Description == "Date");
            //field = new Field() { Anonymise = false, Mandatory = false, FieldType = fieldType };
            //element = new DatasetElement { System = true, DatasetElementType = elementType, ElementName = "C.1.2 Date of Creation", Field = field };
            //context.DatasetElements.Add(element);

            //fieldType = context.FieldTypes.SingleOrDefault(u => u.Description == "DropDownList");
            //field = new Field() { Anonymise = false, Mandatory = false, FieldType = fieldType };
            //var fieldValues = new[]
            //{
            //    new FieldValue { Default = true, Field = field, Other = false, Unknown = false, Value = "1=Spontaneous report" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "2=Report from study" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "3=Other" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "4=Not available to sender (unknown)" }
            //};
            //context.FieldValues.AddRange(fieldValues);
            //element = new DatasetElement { System = false, DatasetElementType = elementType, ElementName = "C.1.3 Type of Report", Field = field, OID = "2.16.840.1.113883.3.989.2.1.1.2" };
            //context.DatasetElements.Add(element);

            //fieldType = context.FieldTypes.SingleOrDefault(u => u.Description == "Date");
            //field = new Field() { Anonymise = false, Mandatory = false, FieldType = fieldType };
            //element = new DatasetElement { System = false, DatasetElementType = elementType, ElementName = "C.1.4 Date Report Was First Received from Source", Field = field };
            //context.DatasetElements.Add(element);

            //fieldType = context.FieldTypes.SingleOrDefault(u => u.Description == "Date");
            //field = new Field() { Anonymise = false, Mandatory = false, FieldType = fieldType };
            //element = new DatasetElement { System = false, DatasetElementType = elementType, ElementName = "C.1.5 Date of Most Recent Information for This Report", Field = field };
            //context.DatasetElements.Add(element);

            //fieldType = context.FieldTypes.SingleOrDefault(u => u.Description == "DropDownList");
            //field = new Field() { Anonymise = false, Mandatory = false, FieldType = fieldType, MaxLength = 100 };
            //fieldValues = new[]
            //{
            //    new FieldValue { Default = true, Field = field, Other = false, Unknown = false, Value = "No" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "Yes" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "Unknown" },
            //};
            //context.FieldValues.AddRange(fieldValues);
            //element = new DatasetElement { System = false, DatasetElementType = elementType, ElementName = "C.1.6.1 Are Additional Documents Available?", Field = field, DefaultValue = "" };
            //context.DatasetElements.Add(element);

            //fieldType = context.FieldTypes.SingleOrDefault(u => u.Description == "DropDownList");
            //field = new Field() { Anonymise = false, Mandatory = false, FieldType = fieldType };
            //fieldValues = new[]
            //{
            //    new FieldValue { Default = true, Field = field, Other = false, Unknown = false, Value = "No" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "Yes" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "Unknown" },
            //};
            //context.FieldValues.AddRange(fieldValues);
            //element = new DatasetElement { System = false, DatasetElementType = elementType, ElementName = "C.1.7 Does This Case Fulfil the Local Criteria for an Expedited Report?", Field = field };
            //context.DatasetElements.Add(element);

            //fieldType = context.FieldTypes.SingleOrDefault(u => u.Description == "AlphaNumericTextbox");
            //field = new Field() { Anonymise = false, Mandatory = false, FieldType = fieldType, MaxLength = 100 };
            //element = new DatasetElement { System = true, DatasetElementType = elementType, ElementName = "C.1.8.1 Worldwide Unique Case Identification Number", Field = field, DefaultValue = "", OID = "2.16.840.1.113883.3.989.2.1.3.2" };
            //context.DatasetElements.Add(element);

            //fieldType = context.FieldTypes.SingleOrDefault(u => u.Description == "DropDownList");
            //field = new Field() { Anonymise = false, Mandatory = false, FieldType = fieldType };
            //fieldValues = new[]
            //{
            //    new FieldValue { Default = true, Field = field, Other = false, Unknown = false, Value = "1=Regulator" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "2=Other" },
            //};
            //context.FieldValues.AddRange(fieldValues);
            //element = new DatasetElement { System = false, DatasetElementType = elementType, ElementName = "C.1.8.2 First Sender of This Case", Field = field, OID = "2.16.840.1.113883.3.989.2.1.1.3" };
            //context.DatasetElements.Add(element);

            //fieldType = context.FieldTypes.SingleOrDefault(u => u.Description == "DropDownList");
            //field = new Field() { Anonymise = false, Mandatory = false, FieldType = fieldType };
            //fieldValues = new[]
            //{
            //    new FieldValue { Default = true, Field = field, Other = false, Unknown = false, Value = "1=Nullification" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "2=Amendment" },
            //};
            //context.FieldValues.AddRange(fieldValues);
            //element = new DatasetElement { System = false, DatasetElementType = elementType, ElementName = "C.1.11.1 Report Nullification / Amendment", Field = field, OID = "2.16.840.1.113883.3.989.2.1.1.5" };
            //context.DatasetElements.Add(element);

            //fieldType = context.FieldTypes.SingleOrDefault(u => u.Description == "AlphaNumericTextbox");
            //field = new Field() { Anonymise = false, Mandatory = false, FieldType = fieldType, MaxLength = 2000 };
            //element = new DatasetElement { System = false, DatasetElementType = elementType, ElementName = "C.1.11.2 Reason for Nullification / Amendment", Field = field };
            //context.DatasetElements.Add(element);

            //fieldType = context.FieldTypes.SingleOrDefault(u => u.Description == "AlphaNumericTextbox");
            //field = new Field() { Anonymise = false, Mandatory = false, FieldType = fieldType, MaxLength = 50 };
            //element = new DatasetElement { System = false, DatasetElementType = elementType, ElementName = "C.2.r.1.1 Reporter’s Title", Field = field };
            //context.DatasetElements.Add(element);

            //fieldType = context.FieldTypes.SingleOrDefault(u => u.Description == "AlphaNumericTextbox");
            //field = new Field() { Anonymise = false, Mandatory = false, FieldType = fieldType, MaxLength = 60 };
            //element = new DatasetElement { System = false, DatasetElementType = elementType, ElementName = "C.2.r.1.2 Reporter’s Given Name", Field = field };
            //context.DatasetElements.Add(element);

            //fieldType = context.FieldTypes.SingleOrDefault(u => u.Description == "AlphaNumericTextbox");
            //field = new Field() { Anonymise = false, Mandatory = false, FieldType = fieldType, MaxLength = 60 };
            //element = new DatasetElement { System = false, DatasetElementType = elementType, ElementName = "C.2.r.1.3 Reporter’s Middle Name", Field = field };
            //context.DatasetElements.Add(element);

            //fieldType = context.FieldTypes.SingleOrDefault(u => u.Description == "AlphaNumericTextbox");
            //field = new Field() { Anonymise = false, Mandatory = false, FieldType = fieldType, MaxLength = 60 };
            //element = new DatasetElement { System = false, DatasetElementType = elementType, ElementName = "C.2.r.1.4 Reporter’s Family Name", Field = field };
            //context.DatasetElements.Add(element);

            //fieldType = context.FieldTypes.SingleOrDefault(u => u.Description == "AlphaNumericTextbox");
            //field = new Field() { Anonymise = false, Mandatory = false, FieldType = fieldType, MaxLength = 60 };
            //element = new DatasetElement { System = false, DatasetElementType = elementType, ElementName = "C.2.r.2.1 Reporter’s Organisation", Field = field };
            //context.DatasetElements.Add(element);

            //fieldType = context.FieldTypes.SingleOrDefault(u => u.Description == "AlphaNumericTextbox");
            //field = new Field() { Anonymise = false, Mandatory = false, FieldType = fieldType, MaxLength = 60 };
            //element = new DatasetElement { System = false, DatasetElementType = elementType, ElementName = "C.2.r.2.2 Reporter’s Department", Field = field };
            //context.DatasetElements.Add(element);

            //fieldType = context.FieldTypes.SingleOrDefault(u => u.Description == "AlphaNumericTextbox");
            //field = new Field() { Anonymise = false, Mandatory = false, FieldType = fieldType, MaxLength = 100 };
            //element = new DatasetElement { System = false, DatasetElementType = elementType, ElementName = "C.2.r.2.3 Reporter’s Street", Field = field };
            //context.DatasetElements.Add(element);

            //fieldType = context.FieldTypes.SingleOrDefault(u => u.Description == "AlphaNumericTextbox");
            //field = new Field() { Anonymise = false, Mandatory = false, FieldType = fieldType, MaxLength = 35 };
            //element = new DatasetElement { System = false, DatasetElementType = elementType, ElementName = "C.2.r.2.4 Reporter’s City", Field = field };
            //context.DatasetElements.Add(element);

            //fieldType = context.FieldTypes.SingleOrDefault(u => u.Description == "AlphaNumericTextbox");
            //field = new Field() { Anonymise = false, Mandatory = false, FieldType = fieldType, MaxLength = 40 };
            //element = new DatasetElement { System = false, DatasetElementType = elementType, ElementName = "C.2.r.2.5 Reporter’s State or Province", Field = field };
            //context.DatasetElements.Add(element);

            //fieldType = context.FieldTypes.SingleOrDefault(u => u.Description == "AlphaNumericTextbox");
            //field = new Field() { Anonymise = false, Mandatory = false, FieldType = fieldType, MaxLength = 15 };
            //element = new DatasetElement { System = false, DatasetElementType = elementType, ElementName = "C.2.r.2.6 Reporter’s Postcode", Field = field };
            //context.DatasetElements.Add(element);

            //fieldType = context.FieldTypes.SingleOrDefault(u => u.Description == "AlphaNumericTextbox");
            //field = new Field() { Anonymise = false, Mandatory = false, FieldType = fieldType, MaxLength = 33 };
            //element = new DatasetElement { System = false, DatasetElementType = elementType, ElementName = "C.2.r.2.7 Reporter’s Telephone", Field = field };
            //context.DatasetElements.Add(element);

            //fieldType = context.FieldTypes.SingleOrDefault(u => u.Description == "AlphaNumericTextbox");
            //field = new Field() { Anonymise = false, Mandatory = false, FieldType = fieldType, MaxLength = 2 };
            //element = new DatasetElement { System = true, DatasetElementType = elementType, ElementName = "C.2.r.3 Reporter’s Country Code", Field = field, OID = "1.0.3166.1.2.2", DefaultValue = "ZA" };
            //context.DatasetElements.Add(element);

            //fieldType = context.FieldTypes.SingleOrDefault(u => u.Description == "DropDownList");
            //field = new Field() { Anonymise = false, Mandatory = false, FieldType = fieldType };
            //fieldValues = new[]
            //{
            //    new FieldValue { Default = true, Field = field, Other = false, Unknown = false, Value = "1=Physician" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "2=Pharmacist" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "3=Other health professional" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "4=Lawyer" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "5=Consumer or other non health professional" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "nullFlavor: UNK" }
            //};
            //context.FieldValues.AddRange(fieldValues);
            //element = new DatasetElement { System = false, DatasetElementType = elementType, ElementName = "C.2.r.4 Qualification", Field = field, OID = "2.16.840.1.113883.3.989.2.1.1.6" };
            //context.DatasetElements.Add(element);

            //fieldType = context.FieldTypes.SingleOrDefault(u => u.Description == "AlphaNumericTextbox");
            //field = new Field() { Anonymise = false, Mandatory = false, FieldType = fieldType, MaxLength = 1 };
            //element = new DatasetElement { System = true, DatasetElementType = elementType, ElementName = "C.2.r.5 Primary Source for Regulatory Purposes", Field = field, DefaultValue = "1" };
            //context.DatasetElements.Add(element);

            //fieldType = context.FieldTypes.SingleOrDefault(u => u.Description == "DropDownList");
            //field = new Field() { Anonymise = false, Mandatory = false, FieldType = fieldType };
            //fieldValues = new[]
            //{
            //    new FieldValue { Default = true, Field = field, Other = false, Unknown = false, Value = "1=Pharmaceutical Company" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "2=Regulatory Authority" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "3=Health Professional" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "4=Regional Pharmacovigilance Centre" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "5=WHO collaborating centres for international drug monitoring" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "6=Other (e.g. distributor or other organisation)" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "7=Patient / Consumer" }
            //};
            //context.FieldValues.AddRange(fieldValues);
            //element = new DatasetElement { System = false, DatasetElementType = elementType, ElementName = "C.3.1 Sender Type", Field = field, OID = "2.16.840.1.113883.3.989.2.1.1.7" };
            //context.DatasetElements.Add(element);

            //fieldType = context.FieldTypes.SingleOrDefault(u => u.Description == "AlphaNumericTextbox");
            //field = new Field() { Anonymise = false, Mandatory = false, FieldType = fieldType, MaxLength = 100 };
            //element = new DatasetElement { System = false, DatasetElementType = elementType, ElementName = "C.3.2 Sender’s Organisation", Field = field };
            //context.DatasetElements.Add(element);

            //fieldType = context.FieldTypes.SingleOrDefault(u => u.Description == "AlphaNumericTextbox");
            //field = new Field() { Anonymise = false, Mandatory = false, FieldType = fieldType, MaxLength = 60 };
            //element = new DatasetElement { System = false, DatasetElementType = elementType, ElementName = "C.3.3.1 Sender’s Department", Field = field };
            //context.DatasetElements.Add(element);

            //fieldType = context.FieldTypes.SingleOrDefault(u => u.Description == "AlphaNumericTextbox");
            //field = new Field() { Anonymise = false, Mandatory = false, FieldType = fieldType, MaxLength = 50 };
            //element = new DatasetElement { System = false, DatasetElementType = elementType, ElementName = "C.3.3.2 Sender’s Title", Field = field };
            //context.DatasetElements.Add(element);

            //fieldType = context.FieldTypes.SingleOrDefault(u => u.Description == "AlphaNumericTextbox");
            //field = new Field() { Anonymise = false, Mandatory = false, FieldType = fieldType, MaxLength = 60 };
            //element = new DatasetElement { System = false, DatasetElementType = elementType, ElementName = "C.3.3.3 Sender’s Given Name", Field = field };
            //context.DatasetElements.Add(element);

            //fieldType = context.FieldTypes.SingleOrDefault(u => u.Description == "AlphaNumericTextbox");
            //field = new Field() { Anonymise = false, Mandatory = false, FieldType = fieldType, MaxLength = 60 };
            //element = new DatasetElement { System = false, DatasetElementType = elementType, ElementName = "C.3.3.4 Sender’s Middle Name", Field = field };
            //context.DatasetElements.Add(element);

            //fieldType = context.FieldTypes.SingleOrDefault(u => u.Description == "AlphaNumericTextbox");
            //field = new Field() { Anonymise = false, Mandatory = false, FieldType = fieldType, MaxLength = 60 };
            //element = new DatasetElement { System = false, DatasetElementType = elementType, ElementName = "C.3.3.5 Sender’s Family Name", Field = field };
            //context.DatasetElements.Add(element);

            //fieldType = context.FieldTypes.SingleOrDefault(u => u.Description == "AlphaNumericTextbox");
            //field = new Field() { Anonymise = false, Mandatory = false, FieldType = fieldType, MaxLength = 100 };
            //element = new DatasetElement { System = false, DatasetElementType = elementType, ElementName = "C.3.4.1 Sender’s Street Address", Field = field };
            //context.DatasetElements.Add(element);

            //fieldType = context.FieldTypes.SingleOrDefault(u => u.Description == "AlphaNumericTextbox");
            //field = new Field() { Anonymise = false, Mandatory = false, FieldType = fieldType, MaxLength = 35 };
            //element = new DatasetElement { System = false, DatasetElementType = elementType, ElementName = "C.3.4.2 Sender’s City", Field = field };
            //context.DatasetElements.Add(element);

            //fieldType = context.FieldTypes.SingleOrDefault(u => u.Description == "AlphaNumericTextbox");
            //field = new Field() { Anonymise = false, Mandatory = false, FieldType = fieldType, MaxLength = 40 };
            //element = new DatasetElement { System = false, DatasetElementType = elementType, ElementName = "C.3.4.3 Sender’s State or Province", Field = field };
            //context.DatasetElements.Add(element);

            //fieldType = context.FieldTypes.SingleOrDefault(u => u.Description == "AlphaNumericTextbox");
            //field = new Field() { Anonymise = false, Mandatory = false, FieldType = fieldType, MaxLength = 15 };
            //element = new DatasetElement { System = false, DatasetElementType = elementType, ElementName = "C.3.4.4 Sender’s Postcode", Field = field };
            //context.DatasetElements.Add(element);

            //fieldType = context.FieldTypes.SingleOrDefault(u => u.Description == "AlphaNumericTextbox");
            //field = new Field() { Anonymise = false, Mandatory = false, FieldType = fieldType, MaxLength = 2 };
            //element = new DatasetElement { System = true, DatasetElementType = elementType, ElementName = "C.3.4.5 Sender’s Country Code", Field = field, OID = "1.0.3166.1.2.2", DefaultValue = "ZA" };
            //context.DatasetElements.Add(element);

            //fieldType = context.FieldTypes.SingleOrDefault(u => u.Description == "AlphaNumericTextbox");
            //field = new Field() { Anonymise = false, Mandatory = false, FieldType = fieldType, MaxLength = 33 };
            //element = new DatasetElement { System = false, DatasetElementType = elementType, ElementName = "C.3.4.6 Sender’s Telephone", Field = field };
            //context.DatasetElements.Add(element);

            //fieldType = context.FieldTypes.SingleOrDefault(u => u.Description == "AlphaNumericTextbox");
            //field = new Field() { Anonymise = false, Mandatory = false, FieldType = fieldType, MaxLength = 33 };
            //element = new DatasetElement { System = false, DatasetElementType = elementType, ElementName = "C.3.4.7 Sender’s Fax", Field = field };
            //context.DatasetElements.Add(element);

            //fieldType = context.FieldTypes.SingleOrDefault(u => u.Description == "AlphaNumericTextbox");
            //field = new Field() { Anonymise = false, Mandatory = false, FieldType = fieldType, MaxLength = 100 };
            //element = new DatasetElement { System = false, DatasetElementType = elementType, ElementName = "C.3.4.8 Sender’s E-mail Address", Field = field };
            //context.DatasetElements.Add(element);

            //fieldType = context.FieldTypes.SingleOrDefault(u => u.Description == "AlphaNumericTextbox");
            //field = new Field() { Anonymise = false, Mandatory = false, FieldType = fieldType, MaxLength = 20 };
            //element = new DatasetElement { System = false, DatasetElementType = elementType, ElementName = "D.1.1.3 Patient Medical Record Number (Hospital Record Number)", Field = field, OID = "2.16.840.1.113883.3.989.2.1.3.9" };
            //context.DatasetElements.Add(element);

            //fieldType = context.FieldTypes.SingleOrDefault(u => u.Description == "Date");
            //field = new Field() { Anonymise = false, Mandatory = false, FieldType = fieldType };
            //element = new DatasetElement { System = false, DatasetElementType = elementType, ElementName = "D.2.1 Date of Birth", Field = field };
            //context.DatasetElements.Add(element);

            //fieldType = context.FieldTypes.SingleOrDefault(u => u.Description == "NumericTextbox");
            //field = new Field() { Anonymise = false, Mandatory = false, FieldType = fieldType, Decimals = 0 };
            //element = new DatasetElement { System = false, DatasetElementType = elementType, ElementName = "D.2.2a Age at Time of Onset of Reaction / Event", Field = field };
            //context.DatasetElements.Add(element);

            //fieldType = context.FieldTypes.SingleOrDefault(u => u.Description == "DropDownList");
            //field = new Field() { Anonymise = false, Mandatory = false, FieldType = fieldType };
            //fieldValues = new[]
            //{
            //    new FieldValue { Default = true, Field = field, Other = false, Unknown = false, Value = "Year" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "Month" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "Week" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "Day" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "Hour" }
            //};
            //context.FieldValues.AddRange(fieldValues);
            //element = new DatasetElement { System = false, DatasetElementType = elementType, ElementName = "D.2.2bAge at Time of Onset of Reaction / Event (unit)", Field = field, OID = "2.16.840.1.113883.3.989.2.1.1.26" };
            //context.DatasetElements.Add(element);

            //fieldType = context.FieldTypes.SingleOrDefault(u => u.Description == "NumericTextbox");
            //field = new Field() { Anonymise = false, Mandatory = false, FieldType = fieldType, Decimals = 0, MinSize = 0, MaxSize = 50 };
            //element = new DatasetElement { System = false, DatasetElementType = elementType, ElementName = "D.2.2.1a Gestation Period When Reaction / Event Was Observed in the Foetus (number)", Field = field };
            //context.DatasetElements.Add(element);

            //fieldType = context.FieldTypes.SingleOrDefault(u => u.Description == "DropDownList");
            //field = new Field() { Anonymise = false, Mandatory = false, FieldType = fieldType };
            //fieldValues = new[]
            //{
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "Month" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "Week" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "Day" }
            //};
            //context.FieldValues.AddRange(fieldValues);
            //element = new DatasetElement { System = false, DatasetElementType = elementType, ElementName = "D.2.2.1b Gestation Period When Reaction/Event Was Observed in the Foetus (unit)", Field = field, OID = "2.16.840.1.113883.3.989.2.1.1.26" };
            //context.DatasetElements.Add(element);

            //fieldType = context.FieldTypes.SingleOrDefault(u => u.Description == "DropDownList");
            //field = new Field() { Anonymise = false, Mandatory = false, FieldType = fieldType };
            //fieldValues = new[]
            //{
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "0=Foetus" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "1=Neonate (Preterm and Term newborns)" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "2=Infant" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "3=Child" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "4=Adolescent" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "5=Adult" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "6=Elderly" }
            //};
            //context.FieldValues.AddRange(fieldValues);
            //element = new DatasetElement { System = false, DatasetElementType = elementType, ElementName = "D.2.3 Patient Age Group (as per reporter)", Field = field, OID = "2.16.840.1.113883.3.989.2.1.1.9" };
            //context.DatasetElements.Add(element);

            //fieldType = context.FieldTypes.SingleOrDefault(u => u.Description == "NumericTextbox");
            //field = new Field() { Anonymise = false, Mandatory = false, FieldType = fieldType, Decimals = 0, MinSize = 0, MaxSize = 150 };
            //element = new DatasetElement { System = false, DatasetElementType = elementType, ElementName = "D.3 Body Weight (kg)", Field = field };
            //context.DatasetElements.Add(element);

            //fieldType = context.FieldTypes.SingleOrDefault(u => u.Description == "NumericTextbox");
            //field = new Field() { Anonymise = false, Mandatory = false, FieldType = fieldType, Decimals = 0, MinSize = 0, MaxSize = 500 };
            //element = new DatasetElement { System = false, DatasetElementType = elementType, ElementName = "D.4 Height (cm)", Field = field };
            //context.DatasetElements.Add(element);

            //fieldType = context.FieldTypes.SingleOrDefault(u => u.Description == "DropDownList");
            //field = new Field() { Anonymise = false, Mandatory = false, FieldType = fieldType };
            //fieldValues = new[]
            //{
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "1=Male" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "2=Female" }
            //};
            //context.FieldValues.AddRange(fieldValues);
            //element = new DatasetElement { System = false, DatasetElementType = elementType, ElementName = "D.5 Sex", Field = field, OID = "1.0.5218" };
            //context.DatasetElements.Add(element);

            //fieldType = context.FieldTypes.SingleOrDefault(u => u.Description == "Date");
            //field = new Field() { Anonymise = false, Mandatory = false, FieldType = fieldType };
            //element = new DatasetElement { System = false, DatasetElementType = elementType, ElementName = "D.6 Last Menstrual Period Date", Field = field };
            //context.DatasetElements.Add(element);

            //fieldType = context.FieldTypes.SingleOrDefault(u => u.Description == "Table");
            //field = new Field() { Anonymise = false, Mandatory = false, FieldType = fieldType };
            //element = new DatasetElement { System = false, DatasetElementType = elementType, ElementName = "D.7 Concurrent Conditions", Field = field };
            //context.DatasetElements.Add(element);

            //fieldType = context.FieldTypes.SingleOrDefault(u => u.Description == "AlphaNumericTextbox");
            //field = new Field() { Anonymise = false, Mandatory = false, FieldType = fieldType, MaxLength = 4 };
            //elementSub = new DatasetElementSub { System = true, DatasetElement = element, ElementName = "D.7.1.r.1a MedDRA Version for Medical History", Field = field, FieldOrder = 1, DefaultValue = "18.0" };
            //context.DatasetElementSubs.Add(elementSub);

            //fieldType = context.FieldTypes.SingleOrDefault(u => u.Description == "NumericTextbox");
            //field = new Field() { Anonymise = false, Mandatory = false, FieldType = fieldType };
            //elementSub = new DatasetElementSub { System = false, DatasetElement = element, ElementName = "D.7.1.r.1b MedDRA Code for Medical History", Field = field, FieldOrder = 2, OID = "2.16.840.1.113883.6.163" };
            //context.DatasetElementSubs.Add(elementSub);

            //fieldType = context.FieldTypes.SingleOrDefault(u => u.Description == "Date");
            //field = new Field() { Anonymise = false, Mandatory = false, FieldType = fieldType };
            //elementSub = new DatasetElementSub { System = false, DatasetElement = element, ElementName = "D.7.1.r.2 Start Date", Field = field, FieldOrder = 3 };
            //context.DatasetElementSubs.Add(elementSub);

            //fieldType = context.FieldTypes.SingleOrDefault(u => u.Description == "DropDownList");
            //field = new Field() { Anonymise = false, Mandatory = false, FieldType = fieldType };
            //fieldValues = new[]
            //    {
            //        new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "false" },
            //        new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "true" }
            //    };
            //context.FieldValues.AddRange(fieldValues);
            //elementSub = new DatasetElementSub { System = false, DatasetElement = element, ElementName = "D.7.1.r.3 Continuing", Field = field, FieldOrder = 4 };
            //context.DatasetElementSubs.Add(elementSub);

            //fieldType = context.FieldTypes.SingleOrDefault(u => u.Description == "Date");
            //field = new Field() { Anonymise = false, Mandatory = false, FieldType = fieldType };
            //elementSub = new DatasetElementSub { System = false, DatasetElement = element, ElementName = "D.7.1.r.4 End Date", Field = field, FieldOrder = 5 };
            //context.DatasetElementSubs.Add(elementSub);

            //fieldType = context.FieldTypes.SingleOrDefault(u => u.Description == "AlphaNumericTextbox");
            //field = new Field() { Anonymise = false, Mandatory = false, FieldType = fieldType, MaxLength = 2000 };
            //elementSub = new DatasetElementSub { System = false, DatasetElement = element, ElementName = "D.7.1.r.5 Comments", Field = field, FieldOrder = 6 };
            //context.DatasetElementSubs.Add(elementSub);

            //fieldType = context.FieldTypes.SingleOrDefault(u => u.Description == "AlphaNumericTextbox");
            //field = new Field() { Anonymise = false, Mandatory = false, FieldType = fieldType, MaxLength = 10000 };
            //element = new DatasetElement { System = false, DatasetElementType = elementType, ElementName = "D.7.2 Other Relevant Medical History and Concurrent Conditions", Field = field };
            //context.DatasetElements.Add(element);

            //fieldType = context.FieldTypes.SingleOrDefault(u => u.Description == "Table");
            //field = new Field() { Anonymise = false, Mandatory = false, FieldType = fieldType };
            //element = new DatasetElement { System = false, DatasetElementType = elementType, ElementName = "D.8 Relevant Past Drug History", Field = field };
            //context.DatasetElements.Add(element);

            //fieldType = context.FieldTypes.SingleOrDefault(u => u.Description == "AlphaNumericTextbox");
            //field = new Field() { Anonymise = false, Mandatory = false, FieldType = fieldType, MaxLength = 250 };
            //elementSub = new DatasetElementSub { System = false, DatasetElement = element, ElementName = "D.8.r.1 Name of Drug as Reported", Field = field, FieldOrder = 1 };
            //context.DatasetElementSubs.Add(elementSub);

            //fieldType = context.FieldTypes.SingleOrDefault(u => u.Description == "Date");
            //field = new Field() { Anonymise = false, Mandatory = false, FieldType = fieldType };
            //elementSub = new DatasetElementSub { System = false, DatasetElement = element, ElementName = "D.8.r.4 Start Date", Field = field, FieldOrder = 2 };
            //context.DatasetElementSubs.Add(elementSub);

            //fieldType = context.FieldTypes.SingleOrDefault(u => u.Description == "Date");
            //field = new Field() { Anonymise = false, Mandatory = false, FieldType = fieldType };
            //elementSub = new DatasetElementSub { System = false, DatasetElement = element, ElementName = "D.8.r.5 End Date", Field = field, FieldOrder = 3 };
            //context.DatasetElementSubs.Add(elementSub);

            //fieldType = context.FieldTypes.SingleOrDefault(u => u.Description == "AlphaNumericTextbox");
            //field = new Field() { Anonymise = false, Mandatory = false, FieldType = fieldType, MaxLength = 4 };
            //elementSub = new DatasetElementSub { System = true, DatasetElement = element, ElementName = "D.8.r.6a MedDRA Version for Indication", Field = field, FieldOrder = 4, DefaultValue = "18.0" };
            //context.DatasetElementSubs.Add(elementSub);

            //fieldType = context.FieldTypes.SingleOrDefault(u => u.Description == "NumericTextbox");
            //field = new Field() { Anonymise = false, Mandatory = false, FieldType = fieldType };
            //elementSub = new DatasetElementSub { System = false, DatasetElement = element, ElementName = "D.8.r.6b MedDRA Code for Indication", Field = field, FieldOrder = 5, OID = "2.16.840.1.113883.6.163" };
            //context.DatasetElementSubs.Add(elementSub);

            //fieldType = context.FieldTypes.SingleOrDefault(u => u.Description == "AlphaNumericTextbox");
            //field = new Field() { Anonymise = false, Mandatory = false, FieldType = fieldType, MaxLength = 4 };
            //elementSub = new DatasetElementSub { System = true, DatasetElement = element, ElementName = "D.8.r.7a MedDRA Version for Reaction", Field = field, FieldOrder = 4, DefaultValue = "18.0" };
            //context.DatasetElementSubs.Add(elementSub);

            //fieldType = context.FieldTypes.SingleOrDefault(u => u.Description == "NumericTextbox");
            //field = new Field() { Anonymise = false, Mandatory = false, FieldType = fieldType };
            //elementSub = new DatasetElementSub { System = false, DatasetElement = element, ElementName = "D.8.r.7b MedDRA Code for Reaction", Field = field, FieldOrder = 5, OID = "2.16.840.1.113883.6.163" };
            //context.DatasetElementSubs.Add(elementSub);

            //fieldType = context.FieldTypes.SingleOrDefault(u => u.Description == "Date");
            //field = new Field() { Anonymise = false, Mandatory = false, FieldType = fieldType };
            //element = new DatasetElement { System = false, DatasetElementType = elementType, ElementName = "D.9.1 Date of Death", Field = field };
            //context.DatasetElements.Add(element);

            //fieldType = context.FieldTypes.SingleOrDefault(u => u.Description == "Table");
            //field = new Field() { Anonymise = false, Mandatory = false, FieldType = fieldType };
            //element = new DatasetElement { System = false, DatasetElementType = elementType, ElementName = "D.9.2 Reported Cause(s) of Death", Field = field };
            //context.DatasetElements.Add(element);

            //fieldType = context.FieldTypes.SingleOrDefault(u => u.Description == "AlphaNumericTextbox");
            //field = new Field() { Anonymise = false, Mandatory = false, FieldType = fieldType, MaxLength = 4 };
            //elementSub = new DatasetElementSub { System = true, DatasetElement = element, ElementName = "D.9.2.r.1a MedDRA Version for Reported Cause(s) of Death", Field = field, FieldOrder = 1, DefaultValue = "18.0" };
            //context.DatasetElementSubs.Add(elementSub);

            //fieldType = context.FieldTypes.SingleOrDefault(u => u.Description == "NumericTextbox");
            //field = new Field() { Anonymise = false, Mandatory = false, FieldType = fieldType };
            //elementSub = new DatasetElementSub { System = false, DatasetElement = element, ElementName = "D.9.2.r.1b MedDRA Code for Reported Cause(s) of Death", Field = field, FieldOrder = 2, OID = "2.16.840.1.113883.6.163" };
            //context.DatasetElementSubs.Add(elementSub);

            //fieldType = context.FieldTypes.SingleOrDefault(u => u.Description == "AlphaNumericTextbox");
            //field = new Field() { Anonymise = false, Mandatory = false, FieldType = fieldType, MaxLength = 250 };
            //elementSub = new DatasetElementSub { System = false, DatasetElement = element, ElementName = "D.9.2.r.2 Reported Cause(s) of Death", Field = field, FieldOrder = 3 };
            //context.DatasetElementSubs.Add(elementSub);

            //fieldType = context.FieldTypes.SingleOrDefault(u => u.Description == "DropDownList");
            //field = new Field() { Anonymise = false, Mandatory = false, FieldType = fieldType };
            //fieldValues = new[]
            //{
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "false" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "true" }
            //};
            //context.FieldValues.AddRange(fieldValues);
            //element = new DatasetElement { System = false, DatasetElementType = elementType, ElementName = "D.9.3 Was Autopsy Done?", Field = field };
            //context.DatasetElements.Add(element);

            //fieldType = context.FieldTypes.SingleOrDefault(u => u.Description == "Table");
            //field = new Field() { Anonymise = false, Mandatory = false, FieldType = fieldType };
            //element = new DatasetElement { System = false, DatasetElementType = elementType, ElementName = "D.9.4 Autopsy-determined Cause(s) of Death", Field = field };
            //context.DatasetElements.Add(element);

            //fieldType = context.FieldTypes.SingleOrDefault(u => u.Description == "AlphaNumericTextbox");
            //field = new Field() { Anonymise = false, Mandatory = false, FieldType = fieldType, MaxLength = 4 };
            //elementSub = new DatasetElementSub { System = true, DatasetElement = element, ElementName = "D.9.4.r.1a MedDRA Version for Autopsy-determined Cause(s) of Death", Field = field, FieldOrder = 1, DefaultValue = "18.0" };
            //context.DatasetElementSubs.Add(elementSub);

            //fieldType = context.FieldTypes.SingleOrDefault(u => u.Description == "NumericTextbox");
            //field = new Field() { Anonymise = false, Mandatory = false, FieldType = fieldType };
            //elementSub = new DatasetElementSub { System = false, DatasetElement = element, ElementName = "D.9.4.r.1b MedDRA Code for Autopsy-determined Cause(s) of Death", Field = field, FieldOrder = 2, OID = "2.16.840.1.113883.6.163" };
            //context.DatasetElementSubs.Add(elementSub);

            //fieldType = context.FieldTypes.SingleOrDefault(u => u.Description == "AlphaNumericTextbox");
            //field = new Field() { Anonymise = false, Mandatory = false, FieldType = fieldType, MaxLength = 250 };
            //elementSub = new DatasetElementSub { System = false, DatasetElement = element, ElementName = "D.9.4.r.2 Autopsy-determined Cause(s) of Death", Field = field, FieldOrder = 3 };
            //context.DatasetElementSubs.Add(elementSub);

            //fieldType = context.FieldTypes.SingleOrDefault(u => u.Description == "AlphaNumericTextbox");
            //field = new Field() { Anonymise = false, Mandatory = false, FieldType = fieldType, MaxLength = 250 };
            //element = new DatasetElement { System = false, DatasetElementType = elementType, ElementName = "E.i.1.1a Reaction / Event as Reported by the Primary Source in Native Language", Field = field };
            //context.DatasetElements.Add(element);

            //fieldType = context.FieldTypes.SingleOrDefault(u => u.Description == "AlphaNumericTextbox");
            //field = new Field() { Anonymise = false, Mandatory = false, FieldType = fieldType, MaxLength = 250 };
            //element = new DatasetElement { System = true, DatasetElementType = elementType, ElementName = "E.i.1.1b Reaction / Event as Reported by the Primary Source Language", Field = field, OID = "2.16.840.1.113883.6.100", DefaultValue = "eng" };
            //context.DatasetElements.Add(element);

            //fieldType = context.FieldTypes.SingleOrDefault(u => u.Description == "AlphaNumericTextbox");
            //field = new Field() { Anonymise = false, Mandatory = false, FieldType = fieldType, MaxLength = 250 };
            //element = new DatasetElement { System = false, DatasetElementType = elementType, ElementName = "E.i.1.2 Reaction / Event as Reported by the Primary Source for Translation", Field = field };
            //context.DatasetElements.Add(element);

            //fieldType = context.FieldTypes.SingleOrDefault(u => u.Description == "AlphaNumericTextbox");
            //field = new Field() { Anonymise = false, Mandatory = false, FieldType = fieldType, MaxLength = 4 };
            //element = new DatasetElement { System = true, DatasetElementType = elementType, ElementName = "E.i.2.1a MedDRA Version for Reaction / Event", Field = field, DefaultValue = "18.0" };
            //context.DatasetElements.Add(element);

            //fieldType = context.FieldTypes.SingleOrDefault(u => u.Description == "AlphaNumericTextbox");
            //field = new Field() { Anonymise = false, Mandatory = false, FieldType = fieldType, MaxLength = 50 };
            //element = new DatasetElement { System = true, DatasetElementType = elementType, ElementName = "E.i.2.1b MedDRA Code for Reaction / Event", Field = field, OID = "2.16.840.1.113883.6.163" };
            //context.DatasetElements.Add(element);

            //fieldType = context.FieldTypes.SingleOrDefault(u => u.Description == "DropDownList");
            //field = new Field() { Anonymise = false, Mandatory = false, FieldType = fieldType };
            //fieldValues = new[]
            //{
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "1=Yes, highlighted by the reporter, NOT serious" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "2=No, not highlighted by the reporter, NOT serious" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "3=Yes, highlighted by the reporter, SERIOUS" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "4=No, not highlighted by the reporter, SERIOUS" }
            //};
            //context.FieldValues.AddRange(fieldValues);
            //element = new DatasetElement { System = false, DatasetElementType = elementType, ElementName = "E.i.3.1 Term Highlighted by the Reporter", Field = field, OID = "2.16.840.1.113883.3.989.2.1.1.10" };
            //context.DatasetElements.Add(element);

            //fieldType = context.FieldTypes.SingleOrDefault(u => u.Description == "DropDownList");
            //field = new Field() { Anonymise = false, Mandatory = false, FieldType = fieldType };
            //fieldValues = new[]
            //{
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "Yes" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "Unknown" }
            //};
            //context.FieldValues.AddRange(fieldValues);
            //element = new DatasetElement { System = false, DatasetElementType = elementType, ElementName = "E.i.3.2a Results in Death", Field = field };
            //context.DatasetElements.Add(element);

            //fieldType = context.FieldTypes.SingleOrDefault(u => u.Description == "DropDownList");
            //field = new Field() { Anonymise = false, Mandatory = false, FieldType = fieldType };
            //fieldValues = new[]
            //{
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "Yes" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "Unknown" }
            //};
            //context.FieldValues.AddRange(fieldValues);
            //element = new DatasetElement { System = false, DatasetElementType = elementType, ElementName = "E.i.3.2b Life Threatening", Field = field };
            //context.DatasetElements.Add(element);

            //fieldType = context.FieldTypes.SingleOrDefault(u => u.Description == "DropDownList");
            //field = new Field() { Anonymise = false, Mandatory = false, FieldType = fieldType };
            //fieldValues = new[]
            //{
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "Yes" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "Unknown" }
            //};
            //context.FieldValues.AddRange(fieldValues);
            //element = new DatasetElement { System = false, DatasetElementType = elementType, ElementName = "E.i.3.2c Caused / Prolonged Hospitalisation", Field = field };
            //context.DatasetElements.Add(element);

            //fieldType = context.FieldTypes.SingleOrDefault(u => u.Description == "DropDownList");
            //field = new Field() { Anonymise = false, Mandatory = false, FieldType = fieldType };
            //fieldValues = new[]
            //{
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "Yes" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "Unknown" }
            //};
            //context.FieldValues.AddRange(fieldValues);
            //element = new DatasetElement { System = false, DatasetElementType = elementType, ElementName = "E.i.3.2d Disabling / Incapacitating", Field = field };
            //context.DatasetElements.Add(element);

            //fieldType = context.FieldTypes.SingleOrDefault(u => u.Description == "DropDownList");
            //field = new Field() { Anonymise = false, Mandatory = false, FieldType = fieldType };
            //fieldValues = new[]
            //{
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "Yes" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "Unknown" }
            //};
            //context.FieldValues.AddRange(fieldValues);
            //element = new DatasetElement { System = false, DatasetElementType = elementType, ElementName = "E.i.3.2e Congenital Anomaly / Birth Defect", Field = field };
            //context.DatasetElements.Add(element);

            //fieldType = context.FieldTypes.SingleOrDefault(u => u.Description == "DropDownList");
            //field = new Field() { Anonymise = false, Mandatory = false, FieldType = fieldType };
            //fieldValues = new[]
            //{
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "Yes" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "Unknown" }
            //};
            //context.FieldValues.AddRange(fieldValues);
            //element = new DatasetElement { System = false, DatasetElementType = elementType, ElementName = "E.i.3.2f Other Medically Important Condition", Field = field };
            //context.DatasetElements.Add(element);

            //fieldType = context.FieldTypes.SingleOrDefault(u => u.Description == "Date");
            //field = new Field() { Anonymise = false, Mandatory = false, FieldType = fieldType };
            //element = new DatasetElement { System = false, DatasetElementType = elementType, ElementName = "E.i.4 Date of Start of Reaction / Event", Field = field };
            //context.DatasetElements.Add(element);

            //fieldType = context.FieldTypes.SingleOrDefault(u => u.Description == "Date");
            //field = new Field() { Anonymise = false, Mandatory = false, FieldType = fieldType };
            //element = new DatasetElement { System = false, DatasetElementType = elementType, ElementName = "E.i.5 Date of End of Reaction / Event", Field = field };
            //context.DatasetElements.Add(element);

            //fieldType = context.FieldTypes.SingleOrDefault(u => u.Description == "NumericTextbox");
            //field = new Field() { Anonymise = false, Mandatory = false, FieldType = fieldType };
            //element = new DatasetElement { System = false, DatasetElementType = elementType, ElementName = "E.i.6a Duration of Reaction / Event (number)", Field = field };
            //context.DatasetElements.Add(element);

            //fieldType = context.FieldTypes.SingleOrDefault(u => u.Description == "DropDownList");
            //field = new Field() { Anonymise = false, Mandatory = false, FieldType = fieldType };
            //fieldValues = new[]
            //{
            //    new FieldValue { Default = true, Field = field, Other = false, Unknown = false, Value = "Years" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "Months" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "Weeks" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "Days" }
            //};
            //context.FieldValues.AddRange(fieldValues);
            //element = new DatasetElement { System = false, DatasetElementType = elementType, ElementName = "E.i.6b Duration of Reaction / Event (unit)", Field = field };
            //context.DatasetElements.Add(element);

            //fieldType = context.FieldTypes.SingleOrDefault(u => u.Description == "DropDownList");
            //field = new Field() { Anonymise = false, Mandatory = false, FieldType = fieldType };
            //fieldValues = new[]
            //{
            //    new FieldValue { Default = true, Field = field, Other = false, Unknown = false, Value = "1=recovered/resolved" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "2=recovering/resolving" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "3=not recovered/not resolved/ongoing" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "4=recovered/resolved with sequelae" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "5=fatal" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "0=unknown" }
            //};
            //context.FieldValues.AddRange(fieldValues);
            //element = new DatasetElement { System = false, DatasetElementType = elementType, ElementName = "E.i.7 Outcome of Reaction / Event at the Time of Last Observation", Field = field, OID = "2.16.840.1.113883.3.989.2.1.1.11" };
            //context.DatasetElements.Add(element);

            //fieldType = context.FieldTypes.SingleOrDefault(u => u.Description == "DropDownList");
            //field = new Field() { Anonymise = false, Mandatory = false, FieldType = fieldType };
            //fieldValues = new[]
            //{
            //    new FieldValue { Default = true, Field = field, Other = false, Unknown = false, Value = "No" },
            //    new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "Yes" }
            //};
            //context.FieldValues.AddRange(fieldValues);
            //element = new DatasetElement { System = false, DatasetElementType = elementType, ElementName = "E.i.8 Medical Confirmation by Healthcare Professional", Field = field };
            //context.DatasetElements.Add(element);

            //fieldType = context.FieldTypes.SingleOrDefault(u => u.Description == "AlphaNumericTextbox");
            //field = new Field() { Anonymise = false, Mandatory = false, FieldType = fieldType, MaxLength = 2 };
            //element = new DatasetElement { System = true, DatasetElementType = elementType, ElementName = "E.i.9 Identification of the Country Where the Reaction / Event Occurred", Field = field, OID = "1.0.3166.1.2.2", DefaultValue = "ZA" };
            //context.DatasetElements.Add(element);

            //fieldType = context.FieldTypes.SingleOrDefault(u => u.Description == "Table");
            //field = new Field() { Anonymise = false, Mandatory = false, FieldType = fieldType };
            //element = new DatasetElement { System = false, DatasetElementType = elementType, ElementName = "F.r Results of Tests and Procedures Relevant", Field = field };
            //context.DatasetElements.Add(element);

            //fieldType = context.FieldTypes.SingleOrDefault(u => u.Description == "Date");
            //field = new Field() { Anonymise = false, Mandatory = false, FieldType = fieldType };
            //elementSub = new DatasetElementSub { System = true, DatasetElement = element, ElementName = "F.r.1 Test Date", Field = field, FieldOrder = 1 };
            //context.DatasetElementSubs.Add(elementSub);

            //fieldType = context.FieldTypes.SingleOrDefault(u => u.Description == "AlphaNumericTextbox");
            //field = new Field() { Anonymise = false, Mandatory = false, FieldType = fieldType, MaxLength = 250 };
            //elementSub = new DatasetElementSub { System = false, DatasetElement = element, ElementName = "F.r.2.1 Test Name", Field = field, FieldOrder = 2 };
            //context.DatasetElementSubs.Add(elementSub);

            //fieldType = context.FieldTypes.SingleOrDefault(u => u.Description == "AlphaNumericTextbox");
            //field = new Field() { Anonymise = false, Mandatory = false, FieldType = fieldType, MaxLength = 4 };
            //elementSub = new DatasetElementSub { System = true, DatasetElement = element, ElementName = "F.r.2.2a MedDRA Version for Test Name", Field = field, DefaultValue = "18.0", FieldOrder = 3 };
            //context.DatasetElementSubs.Add(elementSub);

            //fieldType = context.FieldTypes.SingleOrDefault(u => u.Description == "NumericTextbox");
            //field = new Field() { Anonymise = false, Mandatory = false, FieldType = fieldType };
            //elementSub = new DatasetElementSub { System = true, DatasetElement = element, ElementName = "F.r.2.2a MedDRA Code for Test Name", Field = field, OID = "2.16.840.1.113883.6.163", FieldOrder = 4 };
            //context.DatasetElementSubs.Add(elementSub);

            //fieldType = context.FieldTypes.SingleOrDefault(u => u.Description == "DropDownList");
            //field = new Field() { Anonymise = false, Mandatory = false, FieldType = fieldType };
            //fieldValues = new[]
            //    {
            //        new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "1=Positive" },
            //        new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "2=Negative" },
            //        new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "3=Borderline" },
            //        new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "4=Inconclusive" }
            //    };
            //context.FieldValues.AddRange(fieldValues);
            //elementSub = new DatasetElementSub { System = false, DatasetElement = element, ElementName = "F.r.3.1 Test Result (code)", Field = field, OID = "2.16.840.1.113883.3.989.2.1.1.12", FieldOrder = 5 };
            //context.DatasetElementSubs.Add(elementSub);

            //fieldType = context.FieldTypes.SingleOrDefault(u => u.Description == "NumericTextbox");
            //field = new Field() { Anonymise = false, Mandatory = false, FieldType = fieldType };
            //elementSub = new DatasetElementSub { System = false, DatasetElement = element, ElementName = "F.r.3.2 Test Result (value / qualifier)", Field = field, FieldOrder = 6 };
            //context.DatasetElementSubs.Add(elementSub);

            //fieldType = context.FieldTypes.SingleOrDefault(u => u.Description == "AlphaNumericTextbox");
            //field = new Field() { Anonymise = false, Mandatory = false, FieldType = fieldType, MaxLength = 20 };
            //elementSub = new DatasetElementSub { System = false, DatasetElement = element, ElementName = "F.r.3.3 Test Result (unit)", Field = field, FieldOrder = 7 };
            //context.DatasetElementSubs.Add(elementSub);

            //fieldType = context.FieldTypes.SingleOrDefault(u => u.Description == "AlphaNumericTextbox");
            //field = new Field() { Anonymise = false, Mandatory = false, FieldType = fieldType, MaxLength = 2000 };
            //elementSub = new DatasetElementSub { System = false, DatasetElement = element, ElementName = "F.r.3.4 Result Unstructured Data", Field = field, FieldOrder = 8 };
            //context.DatasetElementSubs.Add(elementSub);

            //fieldType = context.FieldTypes.SingleOrDefault(u => u.Description == "AlphaNumericTextbox");
            //field = new Field() { Anonymise = false, Mandatory = false, FieldType = fieldType, MaxLength = 50 };
            //elementSub = new DatasetElementSub { System = false, DatasetElement = element, ElementName = "F.r.4 Normal Low Value", Field = field, FieldOrder = 9 };
            //context.DatasetElementSubs.Add(elementSub);

            //fieldType = context.FieldTypes.SingleOrDefault(u => u.Description == "AlphaNumericTextbox");
            //field = new Field() { Anonymise = false, Mandatory = false, FieldType = fieldType, MaxLength = 50 };
            //elementSub = new DatasetElementSub { System = false, DatasetElement = element, ElementName = "F.r.5 Normal High Value", Field = field, FieldOrder = 10 };
            //context.DatasetElementSubs.Add(elementSub);

            //fieldType = context.FieldTypes.SingleOrDefault(u => u.Description == "AlphaNumericTextbox");
            //field = new Field() { Anonymise = false, Mandatory = false, FieldType = fieldType, MaxLength = 2000 };
            //elementSub = new DatasetElementSub { System = false, DatasetElement = element, ElementName = "F.r.6 Comments", Field = field, FieldOrder = 11 };
            //context.DatasetElementSubs.Add(elementSub);

            //fieldType = context.FieldTypes.SingleOrDefault(u => u.Description == "DropDownList");
            //field = new Field() { Anonymise = false, Mandatory = false, FieldType = fieldType };
            //fieldValues = new[]
            //    {
            //        new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "No" },
            //        new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "Yes" }
            //    };
            //context.FieldValues.AddRange(fieldValues);
            //elementSub = new DatasetElementSub { System = false, DatasetElement = element, ElementName = "F.r.7 More Information Available", Field = field, FieldOrder = 12 };
            //context.DatasetElementSubs.Add(elementSub);

            //fieldType = context.FieldTypes.SingleOrDefault(u => u.Description == "Table");
            //field = new Field() { Anonymise = false, Mandatory = false, FieldType = fieldType };
            //element = new DatasetElement { System = false, DatasetElementType = elementType, ElementName = "G.k DRUG(S) INFORMATION", Field = field };
            //context.DatasetElements.Add(element);

            //fieldType = context.FieldTypes.SingleOrDefault(u => u.Description == "DropDownList");
            //field = new Field() { Anonymise = false, Mandatory = false, FieldType = fieldType };
            //fieldValues = new[]
            //    {
            //        new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "1=Suspect" },
            //        new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "2=Concomitant" },
            //        new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "3=Interacting" },
            //        new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "4=Drug Not Administered" }
            //    };
            //context.FieldValues.AddRange(fieldValues);
            //elementSub = new DatasetElementSub { System = false, DatasetElement = element, ElementName = "G.k.1 Characterisation of Drug Role", Field = field, FieldOrder = 1, OID = "2.16.840.1.113883.3.989.2.1.1.13" };
            //context.DatasetElementSubs.Add(elementSub);

            //fieldType = context.FieldTypes.SingleOrDefault(u => u.Description == "AlphaNumericTextbox");
            //field = new Field() { Anonymise = false, Mandatory = false, FieldType = fieldType, MaxLength = 250 };
            //elementSub = new DatasetElementSub { System = false, DatasetElement = element, ElementName = "G.k.2.2 Medicinal Product Name as Reported by the Primary Source", Field = field, FieldOrder = 2 };
            //context.DatasetElementSubs.Add(elementSub);

            //fieldType = context.FieldTypes.SingleOrDefault(u => u.Description == "AlphaNumericTextbox");
            //field = new Field() { Anonymise = false, Mandatory = false, FieldType = fieldType, MaxLength = 250 };
            //elementSub = new DatasetElementSub { System = false, DatasetElement = element, ElementName = "G.k.2.3.r.1 Substance / Specified Substance Name", Field = field, FieldOrder = 3 };
            //context.DatasetElementSubs.Add(elementSub);

            //fieldType = context.FieldTypes.SingleOrDefault(u => u.Description == "NumericTextbox");
            //field = new Field() { Anonymise = false, Mandatory = false, FieldType = fieldType };
            //elementSub = new DatasetElementSub { System = false, DatasetElement = element, ElementName = "G.k.2.3.r.3a Strength", Field = field, FieldOrder = 4 };
            //context.DatasetElementSubs.Add(elementSub);

            //fieldType = context.FieldTypes.SingleOrDefault(u => u.Description == "AlphaNumericTextbox");
            //field = new Field() { Anonymise = false, Mandatory = false, FieldType = fieldType, MaxLength = 20 };
            //elementSub = new DatasetElementSub { System = false, DatasetElement = element, ElementName = "G.k.2.3.r.3b Strength (unit)", Field = field, FieldOrder = 5 };
            //context.DatasetElementSubs.Add(elementSub);

            //fieldType = context.FieldTypes.SingleOrDefault(u => u.Description == "AlphaNumericTextbox");
            //field = new Field() { Anonymise = false, Mandatory = false, FieldType = fieldType, MaxLength = 2 };
            //elementSub = new DatasetElementSub { System = false, DatasetElement = element, ElementName = "G.k.2.4 Identification of the Country Where the Drug Was Obtained", Field = field, OID = "1.0.3166.1.2.2", DefaultValue = "ZA", FieldOrder = 6 };
            //context.DatasetElementSubs.Add(elementSub);

            //fieldType = context.FieldTypes.SingleOrDefault(u => u.Description == "NumericTextbox");
            //field = new Field() { Anonymise = false, Mandatory = false, FieldType = fieldType };
            //elementSub = new DatasetElementSub { System = false, DatasetElement = element, ElementName = "G.k.4.r.1a Dose (number)", Field = field, FieldOrder = 7 };
            //context.DatasetElementSubs.Add(elementSub);

            //fieldType = context.FieldTypes.SingleOrDefault(u => u.Description == "AlphaNumericTextbox");
            //field = new Field() { Anonymise = false, Mandatory = false, FieldType = fieldType, MaxLength = 20 };
            //elementSub = new DatasetElementSub { System = false, DatasetElement = element, ElementName = "G.k.4.r.1b Dose (unit)", Field = field, FieldOrder = 8, OID = "2.16.840.1.113883.3.989.2.1.1.25" };
            //context.DatasetElementSubs.Add(elementSub);

            //fieldType = context.FieldTypes.SingleOrDefault(u => u.Description == "NumericTextbox");
            //field = new Field() { Anonymise = false, Mandatory = false, FieldType = fieldType };
            //elementSub = new DatasetElementSub { System = false, DatasetElement = element, ElementName = "G.k.4.r.2 Number of Units in the Interval", Field = field, FieldOrder = 9 };
            //context.DatasetElementSubs.Add(elementSub);

            //fieldType = context.FieldTypes.SingleOrDefault(u => u.Description == "AlphaNumericTextbox");
            //field = new Field() { Anonymise = false, Mandatory = false, FieldType = fieldType, MaxLength = 20 };
            //elementSub = new DatasetElementSub { System = false, DatasetElement = element, ElementName = "G.k.4.r.3 Definition of the Time Interval Unit", Field = field, FieldOrder = 10, OID = "2.16.840.1.113883.3.989.2.1.1.26" };
            //context.DatasetElementSubs.Add(elementSub);

            //fieldType = context.FieldTypes.SingleOrDefault(u => u.Description == "Date");
            //field = new Field() { Anonymise = false, Mandatory = false, FieldType = fieldType };
            //elementSub = new DatasetElementSub { System = false, DatasetElement = element, ElementName = "G.k.4.r.4 Date of Start of Drug", Field = field, FieldOrder = 11 };
            //context.DatasetElementSubs.Add(elementSub);

            //fieldType = context.FieldTypes.SingleOrDefault(u => u.Description == "Date");
            //field = new Field() { Anonymise = false, Mandatory = false, FieldType = fieldType };
            //elementSub = new DatasetElementSub { System = false, DatasetElement = element, ElementName = "G.k.4.r.5 Date of Last Administration", Field = field, FieldOrder = 12 };
            //context.DatasetElementSubs.Add(elementSub);

            //fieldType = context.FieldTypes.SingleOrDefault(u => u.Description == "NumericTextbox");
            //field = new Field() { Anonymise = false, Mandatory = false, FieldType = fieldType };
            //elementSub = new DatasetElementSub { System = false, DatasetElement = element, ElementName = "G.k.4.r.6a Duration of Drug Administration (number)", Field = field, FieldOrder = 13 };
            //context.DatasetElementSubs.Add(elementSub);

            //fieldType = context.FieldTypes.SingleOrDefault(u => u.Description == "AlphaNumericTextbox");
            //field = new Field() { Anonymise = false, Mandatory = false, FieldType = fieldType, MaxLength = 50 };
            //elementSub = new DatasetElementSub { System = false, DatasetElement = element, ElementName = "G.k.4.r.6b Duration of Drug Administration (unit)", Field = field, FieldOrder = 14, OID = "2.16.840.1.113883.3.989.2.1.1.26" };
            //context.DatasetElementSubs.Add(elementSub);

            //fieldType = context.FieldTypes.SingleOrDefault(u => u.Description == "AlphaNumericTextbox");
            //field = new Field() { Anonymise = false, Mandatory = false, FieldType = fieldType, MaxLength = 35 };
            //elementSub = new DatasetElementSub { System = false, DatasetElement = element, ElementName = "G.k.4.r.7 Batch / Lot Number", Field = field, FieldOrder = 15 };
            //context.DatasetElementSubs.Add(elementSub);

            //fieldType = context.FieldTypes.SingleOrDefault(u => u.Description == "AlphaNumericTextbox");
            //field = new Field() { Anonymise = false, Mandatory = false, FieldType = fieldType, MaxLength = 2000 };
            //elementSub = new DatasetElementSub { System = false, DatasetElement = element, ElementName = "G.k.4.r.8 Dosage Text", Field = field, FieldOrder = 16 };
            //context.DatasetElementSubs.Add(elementSub);

            //fieldType = context.FieldTypes.SingleOrDefault(u => u.Description == "AlphaNumericTextbox");
            //field = new Field() { Anonymise = false, Mandatory = false, FieldType = fieldType, MaxLength = 60 };
            //elementSub = new DatasetElementSub { System = false, DatasetElement = element, ElementName = "G.k.4.r.10.1 Route of Administration", Field = field, FieldOrder = 17 };
            //context.DatasetElementSubs.Add(elementSub);

            //fieldType = context.FieldTypes.SingleOrDefault(u => u.Description == "NumericTextbox");
            //field = new Field() { Anonymise = false, Mandatory = false, FieldType = fieldType };
            //elementSub = new DatasetElementSub { System = false, DatasetElement = element, ElementName = "G.k.5a Cumulative Dose to First Reaction (number)", Field = field, FieldOrder = 18 };
            //context.DatasetElementSubs.Add(elementSub);

            //fieldType = context.FieldTypes.SingleOrDefault(u => u.Description == "AlphaNumericTextbox");
            //field = new Field() { Anonymise = false, Mandatory = false, FieldType = fieldType, MaxLength = 50 };
            //elementSub = new DatasetElementSub { System = false, DatasetElement = element, ElementName = "G.k.5b Cumulative Dose to First Reaction (unit)", Field = field, FieldOrder = 19, OID = "2.16.840.1.113883.3.989.2.1.1.25" };
            //context.DatasetElementSubs.Add(elementSub);

            //fieldType = context.FieldTypes.SingleOrDefault(u => u.Description == "NumericTextbox");
            //field = new Field() { Anonymise = false, Mandatory = false, FieldType = fieldType };
            //elementSub = new DatasetElementSub { System = false, DatasetElement = element, ElementName = "G.k.6a Gestation Period at Time of Exposure (number)", Field = field, FieldOrder = 20 };
            //context.DatasetElementSubs.Add(elementSub);

            //fieldType = context.FieldTypes.SingleOrDefault(u => u.Description == "AlphaNumericTextbox");
            //field = new Field() { Anonymise = false, Mandatory = false, FieldType = fieldType, MaxLength = 50 };
            //elementSub = new DatasetElementSub { System = false, DatasetElement = element, ElementName = "G.k.6b Gestation Period at Time of Exposure (unit)", Field = field, FieldOrder = 21, OID = "2.16.840.1.113883.3.989.2.1.1.26" };
            //context.DatasetElementSubs.Add(elementSub);

            //fieldType = context.FieldTypes.SingleOrDefault(u => u.Description == "DropDownList");
            //field = new Field() { Anonymise = false, Mandatory = false, FieldType = fieldType };
            //fieldValues = new[]
            //    {
            //        new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "1=Drug withdrawn" },
            //        new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "2=Dose reduced" },
            //        new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "3=Dose increased" },
            //        new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "4=Dose not changed" },
            //        new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "0=Unknown" },
            //        new FieldValue { Default = false, Field = field, Other = false, Unknown = false, Value = "9=Not applicable" }
            //    };
            //context.FieldValues.AddRange(fieldValues);
            //elementSub = new DatasetElementSub { System = false, DatasetElement = element, ElementName = "G.k.8 Action(s) Taken with Drug", Field = field, FieldOrder = 22, OID = "2.16.840.1.113883.3.989.2.1.1.15" };
            //context.DatasetElementSubs.Add(elementSub);

            //fieldType = context.FieldTypes.SingleOrDefault(u => u.Description == "AlphaNumericTextbox");
            //field = new Field() { Anonymise = false, Mandatory = false, FieldType = fieldType, MaxLength = 500 };
            //element = new DatasetElement { System = false, DatasetElementType = elementType, ElementName = "H.1 Case Narrative", Field = field };
            //context.DatasetElements.Add(element);

            //fieldType = context.FieldTypes.SingleOrDefault(u => u.Description == "AlphaNumericTextbox");
            //field = new Field() { Anonymise = false, Mandatory = false, FieldType = fieldType, MaxLength = 500 };
            //element = new DatasetElement { System = false, DatasetElementType = elementType, ElementName = "H.2 Reporter's Comments", Field = field };
            //context.DatasetElements.Add(element);

            //fieldType = context.FieldTypes.SingleOrDefault(u => u.Description == "AlphaNumericTextbox");
            //field = new Field() { Anonymise = false, Mandatory = false, FieldType = fieldType, MaxLength = 500 };
            //element = new DatasetElement { System = false, DatasetElementType = elementType, ElementName = "H.4 Sender's Comments", Field = field };
            //context.DatasetElements.Add(element);

            context.SaveChanges();
        }
    }
}

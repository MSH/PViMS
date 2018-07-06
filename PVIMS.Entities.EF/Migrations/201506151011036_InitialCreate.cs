namespace PVIMS.Entities.EF.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Appointment",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        AppointmentDate = c.DateTime(nullable: false),
                        Reason = c.String(nullable: false, maxLength: 250),
                        DNA = c.Boolean(nullable: false),
                        Cancelled = c.Boolean(nullable: false),
                        CancellationReason = c.String(maxLength: 250),
                        Created = c.DateTime(nullable: false),
                        LastUpdated = c.DateTime(),
                        CreatedBy_Id = c.Int(),
                        Patient_Id = c.Int(nullable: false),
                        UpdatedBy_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.User", t => t.CreatedBy_Id)
                .ForeignKey("dbo.Patient", t => t.Patient_Id)
                .ForeignKey("dbo.User", t => t.UpdatedBy_Id)
                .Index(t => t.CreatedBy_Id)
                .Index(t => t.Patient_Id)
                .Index(t => t.UpdatedBy_Id);
            
            CreateTable(
                "dbo.User",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Email = c.String(maxLength: 256),
                        EmailConfirmed = c.Boolean(nullable: false),
                        PasswordHash = c.String(),
                        SecurityStamp = c.String(),
                        PhoneNumber = c.String(),
                        PhoneNumberConfirmed = c.Boolean(nullable: false),
                        TwoFactorEnabled = c.Boolean(nullable: false),
                        LockoutEndDateUtc = c.DateTime(),
                        LockoutEnabled = c.Boolean(nullable: false),
                        AccessFailedCount = c.Int(nullable: false),
                        UserName = c.String(nullable: false, maxLength: 256),
                        FirstName = c.String(),
                        LastName = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Patient",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        DateOfBirth = c.DateTime(storeType: "date"),
                        FirstName = c.String(nullable: false),
                        Surname = c.String(nullable: false),
                        Notes = c.String(),
                        PatientGuid = c.Guid(nullable: false),
                        Created = c.DateTime(nullable: false),
                        LastUpdated = c.DateTime(),
                        CreatedBy_Id = c.Int(),
                        UpdatedBy_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.User", t => t.CreatedBy_Id)
                .ForeignKey("dbo.User", t => t.UpdatedBy_Id)
                .Index(t => t.CreatedBy_Id)
                .Index(t => t.UpdatedBy_Id);
            
            CreateTable(
                "dbo.Attachment",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Content = c.Binary(nullable: false),
                        Description = c.String(maxLength: 100),
                        FileName = c.String(nullable: false, maxLength: 50),
                        Size = c.Long(nullable: false),
                        CustomAttributesXmlSerialised = c.String(storeType: "xml"),
                        Created = c.DateTime(nullable: false),
                        LastUpdated = c.DateTime(),
                        AttachmentType_Id = c.Int(),
                        CreatedBy_Id = c.Int(),
                        Encounter_Id = c.Int(),
                        Patient_Id = c.Int(),
                        UpdatedBy_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AttachmentType", t => t.AttachmentType_Id)
                .ForeignKey("dbo.User", t => t.CreatedBy_Id)
                .ForeignKey("dbo.Encounter", t => t.Encounter_Id)
                .ForeignKey("dbo.Patient", t => t.Patient_Id)
                .ForeignKey("dbo.User", t => t.UpdatedBy_Id)
                .Index(t => t.AttachmentType_Id)
                .Index(t => t.CreatedBy_Id)
                .Index(t => t.Encounter_Id)
                .Index(t => t.Patient_Id)
                .Index(t => t.UpdatedBy_Id);
            
            CreateTable(
                "dbo.AttachmentType",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Description = c.String(nullable: false, maxLength: 50),
                        Key = c.String(nullable: false, maxLength: 4),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Encounter",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        EncounterDate = c.DateTime(nullable: false),
                        Notes = c.String(),
                        EncounterGuid = c.Guid(nullable: false),
                        Discharged = c.Boolean(nullable: false),
                        CustomAttributesXmlSerialised = c.String(),
                        Created = c.DateTime(nullable: false),
                        LastUpdated = c.DateTime(),
                        CreatedBy_Id = c.Int(),
                        EncounterType_Id = c.Int(),
                        Patient_Id = c.Int(),
                        Pregnancy_Id = c.Int(),
                        Priority_Id = c.Int(),
                        UpdatedBy_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.User", t => t.CreatedBy_Id)
                .ForeignKey("dbo.EncounterType", t => t.EncounterType_Id)
                .ForeignKey("dbo.Patient", t => t.Patient_Id)
                .ForeignKey("dbo.Pregnancy", t => t.Pregnancy_Id)
                .ForeignKey("dbo.Priority", t => t.Priority_Id)
                .ForeignKey("dbo.User", t => t.UpdatedBy_Id)
                .Index(t => t.CreatedBy_Id)
                .Index(t => t.EncounterType_Id)
                .Index(t => t.Patient_Id)
                .Index(t => t.Pregnancy_Id)
                .Index(t => t.Priority_Id)
                .Index(t => t.UpdatedBy_Id);
            
            CreateTable(
                "dbo.EncounterType",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Description = c.String(nullable: false, maxLength: 50),
                        Help = c.String(maxLength: 250),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.EncounterTypeWorkPlan",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        CohortGroup_Id = c.Int(),
                        EncounterType_Id = c.Int(),
                        WorkPlan_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.CohortGroup", t => t.CohortGroup_Id)
                .ForeignKey("dbo.EncounterType", t => t.EncounterType_Id)
                .ForeignKey("dbo.WorkPlan", t => t.WorkPlan_Id)
                .Index(t => t.CohortGroup_Id)
                .Index(t => t.EncounterType_Id)
                .Index(t => t.WorkPlan_Id);
            
            CreateTable(
                "dbo.CohortGroup",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        CohortName = c.String(nullable: false, maxLength: 50),
                        CohortCode = c.String(nullable: false, maxLength: 5),
                        LastPatientNo = c.Int(nullable: false),
                        StartDate = c.DateTime(nullable: false),
                        FinishDate = c.DateTime(),
                        MinEnrolment = c.Int(nullable: false),
                        MaxEnrolment = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.CohortGroupEnrolment",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        EnroledDate = c.DateTime(nullable: false),
                        CohortGroup_Id = c.Int(),
                        Patient_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.CohortGroup", t => t.CohortGroup_Id)
                .ForeignKey("dbo.Patient", t => t.Patient_Id)
                .Index(t => t.CohortGroup_Id)
                .Index(t => t.Patient_Id);
            
            CreateTable(
                "dbo.WorkPlan",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Description = c.String(nullable: false, maxLength: 50),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.WorkPlanCareEvent",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Order = c.Short(nullable: false),
                        Active = c.Boolean(nullable: false),
                        CareEvent_Id = c.Int(),
                        WorkPlan_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.CareEvent", t => t.CareEvent_Id)
                .ForeignKey("dbo.WorkPlan", t => t.WorkPlan_Id)
                .Index(t => t.CareEvent_Id)
                .Index(t => t.WorkPlan_Id);
            
            CreateTable(
                "dbo.CareEvent",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Description = c.String(nullable: false, maxLength: 50),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.WorkPlanCareEventDatasetCategory",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        DatasetCategory_Id = c.Long(),
                        WorkPlanCareEvent_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.DatasetCategory", t => t.DatasetCategory_Id)
                .ForeignKey("dbo.WorkPlanCareEvent", t => t.WorkPlanCareEvent_Id)
                .Index(t => t.DatasetCategory_Id)
                .Index(t => t.WorkPlanCareEvent_Id);
            
            CreateTable(
                "dbo.DatasetCategory",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Dataset_Id = c.Long(nullable: false),
                        DatasetCategoryName = c.String(nullable: false, maxLength: 50),
                        CategoryOrder = c.Short(nullable: false),
                        Dataset_Id1 = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Dataset", t => t.Dataset_Id1)
                .Index(t => t.Dataset_Id1);
            
            CreateTable(
                "dbo.Dataset",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        DatasetName = c.String(nullable: false, maxLength: 50),
                        Active = c.Boolean(nullable: false),
                        InitialiseProcess = c.String(maxLength: 100),
                        RulesProcess = c.String(maxLength: 100),
                        Help = c.String(maxLength: 250),
                        Created = c.DateTime(nullable: false),
                        LastUpdated = c.DateTime(),
                        ContextType_Id = c.Int(),
                        CreatedBy_Id = c.Int(),
                        UpdatedBy_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.ContextType", t => t.ContextType_Id)
                .ForeignKey("dbo.User", t => t.CreatedBy_Id)
                .ForeignKey("dbo.User", t => t.UpdatedBy_Id)
                .Index(t => t.ContextType_Id)
                .Index(t => t.CreatedBy_Id)
                .Index(t => t.UpdatedBy_Id);
            
            CreateTable(
                "dbo.ContextType",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Description = c.String(nullable: false, maxLength: 50),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.DatasetCategoryElement",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        FieldOrder = c.Short(nullable: false),
                        DatasetCategory_Id = c.Long(),
                        DatasetElement_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.DatasetCategory", t => t.DatasetCategory_Id)
                .ForeignKey("dbo.DatasetElement", t => t.DatasetElement_Id)
                .Index(t => t.DatasetCategory_Id)
                .Index(t => t.DatasetElement_Id);
            
            CreateTable(
                "dbo.DatasetElement",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ElementName = c.String(maxLength: 50),
                        Field_Id = c.Int(),
                        DatasetElementType_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Field", t => t.Field_Id)
                .ForeignKey("dbo.DatasetElementType", t => t.DatasetElementType_Id)
                .Index(t => t.Field_Id)
                .Index(t => t.DatasetElementType_Id);
            
            CreateTable(
                "dbo.DatasetElementSub",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ElementName = c.String(nullable: false, maxLength: 50),
                        FieldOrder = c.Short(nullable: false),
                        Context = c.Guid(nullable: false),
                        DatasetElement_Id = c.Int(),
                        Field_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.DatasetElement", t => t.DatasetElement_Id)
                .ForeignKey("dbo.Field", t => t.Field_Id)
                .Index(t => t.DatasetElement_Id)
                .Index(t => t.Field_Id);
            
            CreateTable(
                "dbo.Field",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Mandatory = c.Boolean(nullable: false),
                        MaxLength = c.Short(),
                        RegEx = c.String(maxLength: 100),
                        Decimals = c.Short(),
                        MaxSize = c.Decimal(precision: 18, scale: 2),
                        MinSize = c.Decimal(precision: 18, scale: 2),
                        Calculation = c.String(maxLength: 100),
                        Image = c.Binary(storeType: "image"),
                        FileSize = c.Short(),
                        FileExt = c.String(maxLength: 100),
                        Anonymise = c.Boolean(nullable: false),
                        FieldType_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.FieldType", t => t.FieldType_Id)
                .Index(t => t.FieldType_Id);
            
            CreateTable(
                "dbo.FieldType",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Description = c.String(nullable: false, maxLength: 50),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.FieldValue",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Value = c.String(nullable: false, maxLength: 100),
                        Default = c.Boolean(nullable: false),
                        Other = c.Boolean(nullable: false),
                        Unknown = c.Boolean(nullable: false),
                        Field_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Field", t => t.Field_Id)
                .Index(t => t.Field_Id);
            
            CreateTable(
                "dbo.DatasetElementType",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Description = c.String(nullable: false, maxLength: 50),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.PatientClinicalEvent",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Description = c.String(maxLength: 100),
                        OnsetDate = c.DateTime(storeType: "date"),
                        ResolutionDate = c.DateTime(storeType: "date"),
                        NaranjoCausality = c.String(maxLength: 10),
                        WhoCausality = c.String(maxLength: 30),
                        CustomAttributesXmlSerialised = c.String(storeType: "xml"),
                        Medication_Id = c.Int(),
                        CausativeMedication_Id = c.Int(),
                        Encounter_Id = c.Int(),
                        Medication_Id1 = c.Int(),
                        TerminologyMedDra_Id = c.Int(),
                        Patient_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Medication", t => t.Medication_Id)
                .ForeignKey("dbo.Medication", t => t.CausativeMedication_Id)
                .ForeignKey("dbo.Encounter", t => t.Encounter_Id)
                .ForeignKey("dbo.Medication", t => t.Medication_Id1)
                .ForeignKey("dbo.TerminologyMedDra", t => t.TerminologyMedDra_Id)
                .ForeignKey("dbo.Patient", t => t.Patient_Id)
                .Index(t => t.Medication_Id)
                .Index(t => t.CausativeMedication_Id)
                .Index(t => t.Encounter_Id)
                .Index(t => t.Medication_Id1)
                .Index(t => t.TerminologyMedDra_Id)
                .Index(t => t.Patient_Id);
            
            CreateTable(
                "dbo.Medication",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        DrugName = c.String(nullable: false, maxLength: 50),
                        Active = c.Boolean(nullable: false),
                        PackSize = c.Int(nullable: false),
                        Strength = c.String(nullable: false, maxLength: 40),
                        CatalogNo = c.String(maxLength: 10),
                        MedicationForm_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.MedicationForm", t => t.MedicationForm_Id)
                .Index(t => t.MedicationForm_Id);
            
            CreateTable(
                "dbo.ConditionMedication",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Condition_Id = c.Int(),
                        Medication_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Condition", t => t.Condition_Id)
                .ForeignKey("dbo.Medication", t => t.Medication_Id)
                .Index(t => t.Condition_Id)
                .Index(t => t.Medication_Id);
            
            CreateTable(
                "dbo.Condition",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Description = c.String(nullable: false, maxLength: 50),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.PatientCondition",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        DateStart = c.DateTime(nullable: false),
                        DateEnd = c.DateTime(),
                        TreatmentStartDate = c.DateTime(),
                        Comments = c.String(maxLength: 250),
                        CustomAttributesXmlSerialised = c.String(storeType: "xml"),
                        Condition_Id = c.Int(),
                        Patient_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Condition", t => t.Condition_Id)
                .ForeignKey("dbo.Patient", t => t.Patient_Id)
                .Index(t => t.Condition_Id)
                .Index(t => t.Patient_Id);
            
            CreateTable(
                "dbo.MedicationForm",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Description = c.String(nullable: false, maxLength: 50),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.PatientMedication",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        DateStart = c.DateTime(nullable: false),
                        DateEnd = c.DateTime(),
                        Dose = c.String(maxLength: 30),
                        DoseFrequency = c.String(maxLength: 30),
                        DoseUnit = c.String(maxLength: 10),
                        CustomAttributesXmlSerialised = c.String(storeType: "xml"),
                        Medication_Id = c.Int(),
                        Patient_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Medication", t => t.Medication_Id)
                .ForeignKey("dbo.Patient", t => t.Patient_Id)
                .Index(t => t.Medication_Id)
                .Index(t => t.Patient_Id);
            
            CreateTable(
                "dbo.TerminologyMedDra",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        MedDraTerm = c.String(nullable: false, maxLength: 100),
                        MedDraCode = c.String(nullable: false, maxLength: 10),
                        MedDraTermType = c.String(nullable: false, maxLength: 4),
                        Parent_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.TerminologyMedDra", t => t.Parent_Id)
                .Index(t => t.Parent_Id);
            
            CreateTable(
                "dbo.Pregnancy",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        StartDate = c.DateTime(nullable: false),
                        FinishDate = c.DateTime(),
                        PreferredFeedingChoice = c.String(nullable: false, maxLength: 10),
                        InitialGestation = c.Short(),
                        ExpectedDeliveryDate = c.DateTime(),
                        ActualDeliveryDate = c.DateTime(),
                        Created = c.DateTime(nullable: false),
                        LastUpdated = c.DateTime(),
                        CreatedBy_Id = c.Int(),
                        Patient_Id = c.Int(),
                        UpdatedBy_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.User", t => t.CreatedBy_Id)
                .ForeignKey("dbo.Patient", t => t.Patient_Id)
                .ForeignKey("dbo.User", t => t.UpdatedBy_Id)
                .Index(t => t.CreatedBy_Id)
                .Index(t => t.Patient_Id)
                .Index(t => t.UpdatedBy_Id);
            
            CreateTable(
                "dbo.Priority",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Description = c.String(nullable: false, maxLength: 50),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.PatientFacility",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        EnrolledDate = c.DateTime(nullable: false),
                        Facility_Id = c.Int(),
                        Patient_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Facility", t => t.Facility_Id)
                .ForeignKey("dbo.Patient", t => t.Patient_Id)
                .Index(t => t.Facility_Id)
                .Index(t => t.Patient_Id);
            
            CreateTable(
                "dbo.Facility",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        FacilityCode = c.String(nullable: false, maxLength: 10),
                        FacilityName = c.String(nullable: false, maxLength: 50),
                        FacilityType_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.FacilityType", t => t.FacilityType_Id)
                .Index(t => t.FacilityType_Id);
            
            CreateTable(
                "dbo.FacilityType",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Description = c.String(nullable: false, maxLength: 50),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.PatientLabTest",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        TestDate = c.DateTime(nullable: false),
                        TestResult = c.String(maxLength: 50),
                        TestUnit = c.String(maxLength: 20),
                        CustomAttributesXmlSerialised = c.String(storeType: "xml"),
                        LabTest_Id = c.Int(),
                        Patient_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.LabTest", t => t.LabTest_Id)
                .ForeignKey("dbo.Patient", t => t.Patient_Id)
                .Index(t => t.LabTest_Id)
                .Index(t => t.Patient_Id);
            
            CreateTable(
                "dbo.LabTest",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Active = c.Boolean(nullable: false),
                        Description = c.String(nullable: false, maxLength: 50),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.PatientLanguage",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Preferred = c.Boolean(nullable: false),
                        Language_Id = c.Int(),
                        Patient_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Language", t => t.Language_Id)
                .ForeignKey("dbo.Patient", t => t.Patient_Id)
                .Index(t => t.Language_Id)
                .Index(t => t.Patient_Id);
            
            CreateTable(
                "dbo.Language",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Description = c.String(maxLength: 20),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.PatientStatusHistory",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        EffectiveDate = c.DateTime(nullable: false),
                        Details = c.String(maxLength: 100),
                        Created = c.DateTime(nullable: false),
                        LastUpdated = c.DateTime(),
                        CreatedBy_Id = c.Int(),
                        Patient_Id = c.Int(),
                        PatientStatus_Id = c.Int(),
                        UpdatedBy_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.User", t => t.CreatedBy_Id)
                .ForeignKey("dbo.Patient", t => t.Patient_Id)
                .ForeignKey("dbo.PatientStatus", t => t.PatientStatus_Id)
                .ForeignKey("dbo.User", t => t.UpdatedBy_Id)
                .Index(t => t.CreatedBy_Id)
                .Index(t => t.Patient_Id)
                .Index(t => t.PatientStatus_Id)
                .Index(t => t.UpdatedBy_Id);
            
            CreateTable(
                "dbo.PatientStatus",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Description = c.String(nullable: false, maxLength: 50),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.CustomAttributeConfiguration",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ExtendableTypeName = c.String(),
                        CustomAttributeType = c.Int(nullable: false),
                        Category = c.String(),
                        AttributeKey = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Holiday",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        HolidayDate = c.DateTime(nullable: false),
                        Description = c.String(nullable: false, maxLength: 100),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.SelectionDataItem",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        AttributeKey = c.String(),
                        SelectionKey = c.String(),
                        Value = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.TerminologyIcd10",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(maxLength: 20),
                        Description = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Appointment", "UpdatedBy_Id", "dbo.User");
            DropForeignKey("dbo.Appointment", "Patient_Id", "dbo.Patient");
            DropForeignKey("dbo.Patient", "UpdatedBy_Id", "dbo.User");
            DropForeignKey("dbo.PatientStatusHistory", "UpdatedBy_Id", "dbo.User");
            DropForeignKey("dbo.PatientStatusHistory", "PatientStatus_Id", "dbo.PatientStatus");
            DropForeignKey("dbo.PatientStatusHistory", "Patient_Id", "dbo.Patient");
            DropForeignKey("dbo.PatientStatusHistory", "CreatedBy_Id", "dbo.User");
            DropForeignKey("dbo.PatientLanguage", "Patient_Id", "dbo.Patient");
            DropForeignKey("dbo.PatientLanguage", "Language_Id", "dbo.Language");
            DropForeignKey("dbo.PatientLabTest", "Patient_Id", "dbo.Patient");
            DropForeignKey("dbo.PatientLabTest", "LabTest_Id", "dbo.LabTest");
            DropForeignKey("dbo.PatientFacility", "Patient_Id", "dbo.Patient");
            DropForeignKey("dbo.PatientFacility", "Facility_Id", "dbo.Facility");
            DropForeignKey("dbo.Facility", "FacilityType_Id", "dbo.FacilityType");
            DropForeignKey("dbo.PatientClinicalEvent", "Patient_Id", "dbo.Patient");
            DropForeignKey("dbo.Patient", "CreatedBy_Id", "dbo.User");
            DropForeignKey("dbo.Attachment", "UpdatedBy_Id", "dbo.User");
            DropForeignKey("dbo.Attachment", "Patient_Id", "dbo.Patient");
            DropForeignKey("dbo.Encounter", "UpdatedBy_Id", "dbo.User");
            DropForeignKey("dbo.Encounter", "Priority_Id", "dbo.Priority");
            DropForeignKey("dbo.Pregnancy", "UpdatedBy_Id", "dbo.User");
            DropForeignKey("dbo.Pregnancy", "Patient_Id", "dbo.Patient");
            DropForeignKey("dbo.Encounter", "Pregnancy_Id", "dbo.Pregnancy");
            DropForeignKey("dbo.Pregnancy", "CreatedBy_Id", "dbo.User");
            DropForeignKey("dbo.PatientClinicalEvent", "TerminologyMedDra_Id", "dbo.TerminologyMedDra");
            DropForeignKey("dbo.TerminologyMedDra", "Parent_Id", "dbo.TerminologyMedDra");
            DropForeignKey("dbo.PatientClinicalEvent", "Medication_Id1", "dbo.Medication");
            DropForeignKey("dbo.PatientClinicalEvent", "Encounter_Id", "dbo.Encounter");
            DropForeignKey("dbo.PatientClinicalEvent", "CausativeMedication_Id", "dbo.Medication");
            DropForeignKey("dbo.PatientMedication", "Patient_Id", "dbo.Patient");
            DropForeignKey("dbo.PatientMedication", "Medication_Id", "dbo.Medication");
            DropForeignKey("dbo.PatientClinicalEvent", "Medication_Id", "dbo.Medication");
            DropForeignKey("dbo.Medication", "MedicationForm_Id", "dbo.MedicationForm");
            DropForeignKey("dbo.ConditionMedication", "Medication_Id", "dbo.Medication");
            DropForeignKey("dbo.PatientCondition", "Patient_Id", "dbo.Patient");
            DropForeignKey("dbo.PatientCondition", "Condition_Id", "dbo.Condition");
            DropForeignKey("dbo.ConditionMedication", "Condition_Id", "dbo.Condition");
            DropForeignKey("dbo.Encounter", "Patient_Id", "dbo.Patient");
            DropForeignKey("dbo.WorkPlanCareEventDatasetCategory", "WorkPlanCareEvent_Id", "dbo.WorkPlanCareEvent");
            DropForeignKey("dbo.WorkPlanCareEventDatasetCategory", "DatasetCategory_Id", "dbo.DatasetCategory");
            DropForeignKey("dbo.DatasetElement", "DatasetElementType_Id", "dbo.DatasetElementType");
            DropForeignKey("dbo.FieldValue", "Field_Id", "dbo.Field");
            DropForeignKey("dbo.Field", "FieldType_Id", "dbo.FieldType");
            DropForeignKey("dbo.DatasetElementSub", "Field_Id", "dbo.Field");
            DropForeignKey("dbo.DatasetElement", "Field_Id", "dbo.Field");
            DropForeignKey("dbo.DatasetElementSub", "DatasetElement_Id", "dbo.DatasetElement");
            DropForeignKey("dbo.DatasetCategoryElement", "DatasetElement_Id", "dbo.DatasetElement");
            DropForeignKey("dbo.DatasetCategoryElement", "DatasetCategory_Id", "dbo.DatasetCategory");
            DropForeignKey("dbo.Dataset", "UpdatedBy_Id", "dbo.User");
            DropForeignKey("dbo.DatasetCategory", "Dataset_Id1", "dbo.Dataset");
            DropForeignKey("dbo.Dataset", "CreatedBy_Id", "dbo.User");
            DropForeignKey("dbo.Dataset", "ContextType_Id", "dbo.ContextType");
            DropForeignKey("dbo.WorkPlanCareEvent", "WorkPlan_Id", "dbo.WorkPlan");
            DropForeignKey("dbo.WorkPlanCareEvent", "CareEvent_Id", "dbo.CareEvent");
            DropForeignKey("dbo.EncounterTypeWorkPlan", "WorkPlan_Id", "dbo.WorkPlan");
            DropForeignKey("dbo.EncounterTypeWorkPlan", "EncounterType_Id", "dbo.EncounterType");
            DropForeignKey("dbo.EncounterTypeWorkPlan", "CohortGroup_Id", "dbo.CohortGroup");
            DropForeignKey("dbo.CohortGroupEnrolment", "Patient_Id", "dbo.Patient");
            DropForeignKey("dbo.CohortGroupEnrolment", "CohortGroup_Id", "dbo.CohortGroup");
            DropForeignKey("dbo.Encounter", "EncounterType_Id", "dbo.EncounterType");
            DropForeignKey("dbo.Encounter", "CreatedBy_Id", "dbo.User");
            DropForeignKey("dbo.Attachment", "Encounter_Id", "dbo.Encounter");
            DropForeignKey("dbo.Attachment", "CreatedBy_Id", "dbo.User");
            DropForeignKey("dbo.Attachment", "AttachmentType_Id", "dbo.AttachmentType");
            DropForeignKey("dbo.Appointment", "CreatedBy_Id", "dbo.User");
            DropIndex("dbo.PatientStatusHistory", new[] { "UpdatedBy_Id" });
            DropIndex("dbo.PatientStatusHistory", new[] { "PatientStatus_Id" });
            DropIndex("dbo.PatientStatusHistory", new[] { "Patient_Id" });
            DropIndex("dbo.PatientStatusHistory", new[] { "CreatedBy_Id" });
            DropIndex("dbo.PatientLanguage", new[] { "Patient_Id" });
            DropIndex("dbo.PatientLanguage", new[] { "Language_Id" });
            DropIndex("dbo.PatientLabTest", new[] { "Patient_Id" });
            DropIndex("dbo.PatientLabTest", new[] { "LabTest_Id" });
            DropIndex("dbo.Facility", new[] { "FacilityType_Id" });
            DropIndex("dbo.PatientFacility", new[] { "Patient_Id" });
            DropIndex("dbo.PatientFacility", new[] { "Facility_Id" });
            DropIndex("dbo.Pregnancy", new[] { "UpdatedBy_Id" });
            DropIndex("dbo.Pregnancy", new[] { "Patient_Id" });
            DropIndex("dbo.Pregnancy", new[] { "CreatedBy_Id" });
            DropIndex("dbo.TerminologyMedDra", new[] { "Parent_Id" });
            DropIndex("dbo.PatientMedication", new[] { "Patient_Id" });
            DropIndex("dbo.PatientMedication", new[] { "Medication_Id" });
            DropIndex("dbo.PatientCondition", new[] { "Patient_Id" });
            DropIndex("dbo.PatientCondition", new[] { "Condition_Id" });
            DropIndex("dbo.ConditionMedication", new[] { "Medication_Id" });
            DropIndex("dbo.ConditionMedication", new[] { "Condition_Id" });
            DropIndex("dbo.Medication", new[] { "MedicationForm_Id" });
            DropIndex("dbo.PatientClinicalEvent", new[] { "Patient_Id" });
            DropIndex("dbo.PatientClinicalEvent", new[] { "TerminologyMedDra_Id" });
            DropIndex("dbo.PatientClinicalEvent", new[] { "Medication_Id1" });
            DropIndex("dbo.PatientClinicalEvent", new[] { "Encounter_Id" });
            DropIndex("dbo.PatientClinicalEvent", new[] { "CausativeMedication_Id" });
            DropIndex("dbo.PatientClinicalEvent", new[] { "Medication_Id" });
            DropIndex("dbo.FieldValue", new[] { "Field_Id" });
            DropIndex("dbo.Field", new[] { "FieldType_Id" });
            DropIndex("dbo.DatasetElementSub", new[] { "Field_Id" });
            DropIndex("dbo.DatasetElementSub", new[] { "DatasetElement_Id" });
            DropIndex("dbo.DatasetElement", new[] { "DatasetElementType_Id" });
            DropIndex("dbo.DatasetElement", new[] { "Field_Id" });
            DropIndex("dbo.DatasetCategoryElement", new[] { "DatasetElement_Id" });
            DropIndex("dbo.DatasetCategoryElement", new[] { "DatasetCategory_Id" });
            DropIndex("dbo.Dataset", new[] { "UpdatedBy_Id" });
            DropIndex("dbo.Dataset", new[] { "CreatedBy_Id" });
            DropIndex("dbo.Dataset", new[] { "ContextType_Id" });
            DropIndex("dbo.DatasetCategory", new[] { "Dataset_Id1" });
            DropIndex("dbo.WorkPlanCareEventDatasetCategory", new[] { "WorkPlanCareEvent_Id" });
            DropIndex("dbo.WorkPlanCareEventDatasetCategory", new[] { "DatasetCategory_Id" });
            DropIndex("dbo.WorkPlanCareEvent", new[] { "WorkPlan_Id" });
            DropIndex("dbo.WorkPlanCareEvent", new[] { "CareEvent_Id" });
            DropIndex("dbo.CohortGroupEnrolment", new[] { "Patient_Id" });
            DropIndex("dbo.CohortGroupEnrolment", new[] { "CohortGroup_Id" });
            DropIndex("dbo.EncounterTypeWorkPlan", new[] { "WorkPlan_Id" });
            DropIndex("dbo.EncounterTypeWorkPlan", new[] { "EncounterType_Id" });
            DropIndex("dbo.EncounterTypeWorkPlan", new[] { "CohortGroup_Id" });
            DropIndex("dbo.Encounter", new[] { "UpdatedBy_Id" });
            DropIndex("dbo.Encounter", new[] { "Priority_Id" });
            DropIndex("dbo.Encounter", new[] { "Pregnancy_Id" });
            DropIndex("dbo.Encounter", new[] { "Patient_Id" });
            DropIndex("dbo.Encounter", new[] { "EncounterType_Id" });
            DropIndex("dbo.Encounter", new[] { "CreatedBy_Id" });
            DropIndex("dbo.Attachment", new[] { "UpdatedBy_Id" });
            DropIndex("dbo.Attachment", new[] { "Patient_Id" });
            DropIndex("dbo.Attachment", new[] { "Encounter_Id" });
            DropIndex("dbo.Attachment", new[] { "CreatedBy_Id" });
            DropIndex("dbo.Attachment", new[] { "AttachmentType_Id" });
            DropIndex("dbo.Patient", new[] { "UpdatedBy_Id" });
            DropIndex("dbo.Patient", new[] { "CreatedBy_Id" });
            DropIndex("dbo.Appointment", new[] { "UpdatedBy_Id" });
            DropIndex("dbo.Appointment", new[] { "Patient_Id" });
            DropIndex("dbo.Appointment", new[] { "CreatedBy_Id" });
            DropTable("dbo.TerminologyIcd10");
            DropTable("dbo.SelectionDataItem");
            DropTable("dbo.Holiday");
            DropTable("dbo.CustomAttributeConfiguration");
            DropTable("dbo.PatientStatus");
            DropTable("dbo.PatientStatusHistory");
            DropTable("dbo.Language");
            DropTable("dbo.PatientLanguage");
            DropTable("dbo.LabTest");
            DropTable("dbo.PatientLabTest");
            DropTable("dbo.FacilityType");
            DropTable("dbo.Facility");
            DropTable("dbo.PatientFacility");
            DropTable("dbo.Priority");
            DropTable("dbo.Pregnancy");
            DropTable("dbo.TerminologyMedDra");
            DropTable("dbo.PatientMedication");
            DropTable("dbo.MedicationForm");
            DropTable("dbo.PatientCondition");
            DropTable("dbo.Condition");
            DropTable("dbo.ConditionMedication");
            DropTable("dbo.Medication");
            DropTable("dbo.PatientClinicalEvent");
            DropTable("dbo.DatasetElementType");
            DropTable("dbo.FieldValue");
            DropTable("dbo.FieldType");
            DropTable("dbo.Field");
            DropTable("dbo.DatasetElementSub");
            DropTable("dbo.DatasetElement");
            DropTable("dbo.DatasetCategoryElement");
            DropTable("dbo.ContextType");
            DropTable("dbo.Dataset");
            DropTable("dbo.DatasetCategory");
            DropTable("dbo.WorkPlanCareEventDatasetCategory");
            DropTable("dbo.CareEvent");
            DropTable("dbo.WorkPlanCareEvent");
            DropTable("dbo.WorkPlan");
            DropTable("dbo.CohortGroupEnrolment");
            DropTable("dbo.CohortGroup");
            DropTable("dbo.EncounterTypeWorkPlan");
            DropTable("dbo.EncounterType");
            DropTable("dbo.Encounter");
            DropTable("dbo.AttachmentType");
            DropTable("dbo.Attachment");
            DropTable("dbo.Patient");
            DropTable("dbo.User");
            DropTable("dbo.Appointment");
        }
    }
}

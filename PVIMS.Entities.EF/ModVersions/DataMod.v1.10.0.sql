DECLARE @ScriptFileName nvarchar(200)
	, @RunDate datetime
SET @ScriptFileName = 'Mods.Data.v1.10.0.sql'

SELECT @RunDate = RunDate  FROM PostDeployment  WHERE ScriptFileName = @ScriptFileName

IF(@RunDate IS NOT NULL) BEGIN
	
	DECLARE @VersionErrorMessage VARCHAR(1024)
	SET @VersionErrorMessage = 'This script has already been executed.'
	RAISERROR(@VersionErrorMessage,16,1)
	RETURN;
	
END

SET NOCOUNT ON

BEGIN TRAN A1

	if(NOT EXISTS(SELECT Id from dbo.WorkFlow)) begin
		INSERT INTO WorkFlow ([Description], WorkFlowGuid)
			SELECT 'New Active Surveilliance Report', '892F3305-7819-4F18-8A87-11CBA3AEE219'
		INSERT INTO WorkFlow ([Description], WorkFlowGuid)
			SELECT 'New Spontaneous Surveilliance Report', '4096D0A3-45F7-4702-BDA1-76AEDE41B986'

		INSERT INTO Activity ([WorkFlow_Id], QualifiedName, ActivityType)
			SELECT Id, 'Confirm Report Data', 1 FROM WorkFlow where [Description] = 'New Active Surveilliance Report'
		INSERT INTO Activity ([WorkFlow_Id], QualifiedName, ActivityType)
			SELECT Id, 'Set MedDRA and Causality', 1 FROM WorkFlow where [Description] = 'New Active Surveilliance Report'
		INSERT INTO Activity ([WorkFlow_Id], QualifiedName, ActivityType)
			SELECT Id, 'Extract E2B', 1 FROM WorkFlow where [Description] = 'New Active Surveilliance Report'
		INSERT INTO Activity ([WorkFlow_Id], QualifiedName, ActivityType)
			SELECT Id, 'Confirm Report Data', 1 FROM WorkFlow where [Description] = 'New Spontaneous Surveilliance Report'
		INSERT INTO Activity ([WorkFlow_Id], QualifiedName, ActivityType)
			SELECT Id, 'Set MedDRA and Causality', 1 FROM WorkFlow where [Description] = 'New Spontaneous Surveilliance Report'
		INSERT INTO Activity ([WorkFlow_Id], QualifiedName, ActivityType)
			SELECT Id, 'Extract E2B', 1 FROM WorkFlow where [Description] = 'New Spontaneous Surveilliance Report'

		INSERT INTO ActivityExecutionStatus (Activity_Id, [Description], [FriendlyDescription])
			SELECT a.Id, 'UNCONFIRMED', 'Report submitted for confirmation' FROM Activity a INNER JOIN WorkFlow wf on a.WorkFlow_Id = wf.Id WHERE QualifiedName = 'Confirm Report Data' and wf.[Description] = 'New Active Surveilliance Report' 
		INSERT INTO ActivityExecutionStatus (Activity_Id, [Description], [FriendlyDescription])
			SELECT a.Id, 'CONFIRMED', 'Report confirmed by technician' FROM Activity a INNER JOIN WorkFlow wf on a.WorkFlow_Id = wf.Id WHERE QualifiedName = 'Confirm Report Data' and wf.[Description] = 'New Active Surveilliance Report' 
		INSERT INTO ActivityExecutionStatus (Activity_Id, [Description], [FriendlyDescription])
			SELECT a.Id, 'DELETED', 'Report deleted by technician' FROM Activity a INNER JOIN WorkFlow wf on a.WorkFlow_Id = wf.Id WHERE QualifiedName = 'Confirm Report Data' and wf.[Description] = 'New Active Surveilliance Report' 
		INSERT INTO ActivityExecutionStatus (Activity_Id, [Description], [FriendlyDescription])
			SELECT a.Id, 'UNCONFIRMED', 'Report submitted for confirmation' FROM Activity a INNER JOIN WorkFlow wf on a.WorkFlow_Id = wf.Id WHERE QualifiedName = 'Confirm Report Data' and wf.[Description] = 'New Spontaneous Surveilliance Report' 
		INSERT INTO ActivityExecutionStatus (Activity_Id, [Description], [FriendlyDescription])
			SELECT a.Id, 'CONFIRMED', 'Report confirmed by technician' FROM Activity a INNER JOIN WorkFlow wf on a.WorkFlow_Id = wf.Id WHERE QualifiedName = 'Confirm Report Data' and wf.[Description] = 'New Spontaneous Surveilliance Report' 
		INSERT INTO ActivityExecutionStatus (Activity_Id, [Description], [FriendlyDescription])
			SELECT a.Id, 'DELETED', 'Report deleted by technician' FROM Activity a INNER JOIN WorkFlow wf on a.WorkFlow_Id = wf.Id WHERE QualifiedName = 'Confirm Report Data' and wf.[Description] = 'New Spontaneous Surveilliance Report' 

		INSERT INTO ActivityExecutionStatus (Activity_Id, [Description], [FriendlyDescription])
			SELECT a.Id, 'NOTSET', 'Report ready for MedDRA and Causality' FROM Activity a INNER JOIN WorkFlow wf on a.WorkFlow_Id = wf.Id WHERE QualifiedName = 'Set MedDRA and Causality' and wf.[Description] = 'New Active Surveilliance Report' 
		INSERT INTO ActivityExecutionStatus (Activity_Id, [Description], [FriendlyDescription])
			SELECT a.Id, 'MEDDRASET', 'MedDRA term set by technician' FROM Activity a INNER JOIN WorkFlow wf on a.WorkFlow_Id = wf.Id WHERE QualifiedName = 'Set MedDRA and Causality' and wf.[Description] = 'New Active Surveilliance Report' 
		INSERT INTO ActivityExecutionStatus (Activity_Id, [Description], [FriendlyDescription])
			SELECT a.Id, 'CAUSALITYSET', 'Causality set by technician' FROM Activity a INNER JOIN WorkFlow wf on a.WorkFlow_Id = wf.Id WHERE QualifiedName = 'Set MedDRA and Causality' and wf.[Description] = 'New Active Surveilliance Report' 
		INSERT INTO ActivityExecutionStatus (Activity_Id, [Description], [FriendlyDescription])
			SELECT a.Id, 'NOTSET', 'Report ready for MedDRA and Causality' FROM Activity a INNER JOIN WorkFlow wf on a.WorkFlow_Id = wf.Id WHERE QualifiedName = 'Set MedDRA and Causality' and wf.[Description] = 'New Spontaneous Surveilliance Report' 
		INSERT INTO ActivityExecutionStatus (Activity_Id, [Description], [FriendlyDescription])
			SELECT a.Id, 'MEDDRASET', 'MedDRA term set by technician' FROM Activity a INNER JOIN WorkFlow wf on a.WorkFlow_Id = wf.Id WHERE QualifiedName = 'Set MedDRA and Causality' and wf.[Description] = 'New Spontaneous Surveilliance Report' 
		INSERT INTO ActivityExecutionStatus (Activity_Id, [Description], [FriendlyDescription])
			SELECT a.Id, 'CAUSALITYSET', 'Causality set by technician' FROM Activity a INNER JOIN WorkFlow wf on a.WorkFlow_Id = wf.Id WHERE QualifiedName = 'Set MedDRA and Causality' and wf.[Description] = 'New Spontaneous Surveilliance Report' 

		INSERT INTO ActivityExecutionStatus (Activity_Id, [Description], [FriendlyDescription])
			SELECT a.Id, 'NOTGENERATED', 'Report ready for E2B submission' FROM Activity a INNER JOIN WorkFlow wf on a.WorkFlow_Id = wf.Id WHERE QualifiedName = 'Extract E2B' and wf.[Description] = 'New Active Surveilliance Report' 
		INSERT INTO ActivityExecutionStatus (Activity_Id, [Description], [FriendlyDescription])
			SELECT a.Id, 'E2BINITIATED', 'E2B data generated for report' FROM Activity a INNER JOIN WorkFlow wf on a.WorkFlow_Id = wf.Id WHERE QualifiedName = 'Extract E2B' and wf.[Description] = 'New Active Surveilliance Report' 
		INSERT INTO ActivityExecutionStatus (Activity_Id, [Description], [FriendlyDescription])
			SELECT a.Id, 'E2BGENERATED', 'E2B report generated' FROM Activity a INNER JOIN WorkFlow wf on a.WorkFlow_Id = wf.Id WHERE QualifiedName = 'Extract E2B' and wf.[Description] = 'New Active Surveilliance Report' 
		INSERT INTO ActivityExecutionStatus (Activity_Id, [Description], [FriendlyDescription])
			SELECT a.Id, 'E2BSUBMITTED', 'E2B report submitted' FROM Activity a INNER JOIN WorkFlow wf on a.WorkFlow_Id = wf.Id WHERE QualifiedName = 'Extract E2B' and wf.[Description] = 'New Active Surveilliance Report' 
		INSERT INTO ActivityExecutionStatus (Activity_Id, [Description], [FriendlyDescription])
			SELECT a.Id, 'NOTGENERATED', 'Report ready for E2B submission' FROM Activity a INNER JOIN WorkFlow wf on a.WorkFlow_Id = wf.Id WHERE QualifiedName = 'Extract E2B' and wf.[Description] = 'New Spontaneous Surveilliance Report' 
		INSERT INTO ActivityExecutionStatus (Activity_Id, [Description], [FriendlyDescription])
			SELECT a.Id, 'E2BINITIATED', 'E2B data generated for report' FROM Activity a INNER JOIN WorkFlow wf on a.WorkFlow_Id = wf.Id WHERE QualifiedName = 'Extract E2B' and wf.[Description] = 'New Spontaneous Surveilliance Report' 
		INSERT INTO ActivityExecutionStatus (Activity_Id, [Description], [FriendlyDescription])
			SELECT a.Id, 'E2BGENERATED', 'E2B report generated' FROM Activity a INNER JOIN WorkFlow wf on a.WorkFlow_Id = wf.Id WHERE QualifiedName = 'Extract E2B' and wf.[Description] = 'New Spontaneous Surveilliance Report' 
		INSERT INTO ActivityExecutionStatus (Activity_Id, [Description], [FriendlyDescription])
			SELECT a.Id, 'E2BSUBMITTED', 'E2B report submitted' FROM Activity a INNER JOIN WorkFlow wf on a.WorkFlow_Id = wf.Id WHERE QualifiedName = 'Extract E2B' and wf.[Description] = 'New Spontaneous Surveilliance Report' 
		
		-- Create report instances for existing clinical events
		INSERT INTO dbo.ReportInstance
				(ReportInstanceGuid, Created, LastUpdated, CreatedBy_Id, UpdatedBy_Id, WorkFlow_Id, ContextGuid, Identifier, PatientIdentifier, SourceIdentifier)
			SELECT NEWID(), OnsetDate, GETDATE(), 1, 1, (select top 1 Id from dbo.WorkFlow where [Description] = 'New Active Surveilliance Report'), PatientClinicalEventGuid, 'TBD', p.FirstName + ' ' + p.Surname, tm.MedDraTerm
				FROM dbo.PatientClinicalEvent pce 
					INNER JOIN dbo.Patient p on pce.Patient_Id = p.Id
					INNER JOIN dbo.TerminologyMedDra tm on pce.SourceTerminologyMedDra_Id = tm.Id
					
		-- Update new report instance with new identifiers
		UPDATE dbo.ReportInstance
			SET Identifier = cast(WorkFlow_Id as varchar) + '/' + cast(YEAR(Created) as varchar) + '/' + RIGHT('00000'+CAST(Id AS VARCHAR(5)),5)
	
		-- Import Medication
		INSERT INTO dbo.ReportInstanceMedication
				(MedicationIdentifier, ReportInstance_Id, ReportInstanceMedicationGuid)
			SELECT m.DrugName + '; ' + pm.Dose + ' ' + pm.DoseUnit, ri.Id, PatientMedicationGuid
				FROM dbo.PatientClinicalEvent pce 
					INNER JOIN dbo.ReportInstance ri on pce.PatientClinicalEventGuid = ri.ContextGuid
					INNER JOIN dbo.Patient p on pce.Patient_Id = p.Id
					INNER JOIN dbo.PatientMedication pm on pm.Patient_Id = p.Id
					INNER JOIN dbo.Medication m on pm.Medication_Id = m.Id
				WHERE (pm.DateEnd is null and pm.DateStart <= pce.OnsetDate)
					OR (pm.DateEnd is not null and pm.DateStart <= pce.OnsetDate and pm.DateEnd >= pce.OnsetDate)
					
		-- Activity instance
		INSERT INTO dbo.ActivityInstance
				(QualifiedName, Created, LastUpdated, CreatedBy_Id, UpdatedBy_Id, CurrentStatus_Id, ReportInstance_Id, [Current])
			SELECT 'Confirm Report Data', GETDATE(), GETDATE(), 1, 1, (select iaes.Id from dbo.ActivityExecutionStatus iaes inner join dbo.Activity ia on iaes.Activity_Id = ia.Id where iaes.[Description] = 'UNCONFIRMED' and ia.WorkFlow_Id = ri.WorkFlow_Id), ri.Id, 1
				FROM dbo.PatientClinicalEvent pce 
					INNER JOIN dbo.ReportInstance ri on pce.PatientClinicalEventGuid = ri.ContextGuid
		
		-- Activity instance default action
		INSERT INTO dbo.ActivityExecutionStatusEvent
				(EventDateTime, Comments, ActivityInstance_Id, EventCreatedBy_Id, ExecutionStatus_Id)
			SELECT GETDATE(), '', ai.Id, 1, ai.CurrentStatus_Id
				FROM dbo.PatientClinicalEvent pce 
					INNER JOIN dbo.ReportInstance ri on pce.PatientClinicalEventGuid = ri.ContextGuid
					INNER JOIN dbo.ActivityInstance ai on ri.Id = ai.ReportInstance_Id and ai.[Current] = 1
				
	end

	-- New config
	if(NOT EXISTS(SELECT Id from dbo.Config where ConfigType = 5)) begin
		INSERT INTO dbo.Config (ConfigType, ConfigValue, Created, LastUpdated, CreatedBy_Id, UpdatedBy_Id)
			SELECT 5, 1, GETDATE(), GETDATE(), 1, 1 
	end

	-- Additional	
	UPDATE DatasetInstance SET DatasetInstanceGuid = NEWID()

	if(NOT EXISTS(SELECT Id from dbo.AttachmentType where [Key] = 'xml')) begin
		INSERT INTO dbo.AttachmentType ([Description], [Key])
			SELECT 'XML Document', 'xml' 
	end
	
	-- Risk factors
	-- Gender
	declare @id int
	insert into riskfactor
			(FactorName, Criteria, Display, IsSystem, Active)
		select 'Gender', '(select dbo.CleanJsonForString(cast(CustomAttributesXmlSerialised.query(''/CustomAttributeSet/CustomSelectionAttribute[Key/node() = "Gender"]/Value[1]'') as varchar(max))) from Patient where Id = PatientId) = ''#ContextOption#''', 'Gender', 1, 1
	set @id = (SELECT @@IDENTITY )

	insert into riskfactoroption 
			(OptionName, Criteria, Display, RiskFactor_Id)
		select 'Male', '1', 'Is Male', @id
		union  select 'Female', '2', 'Is Female', @id

	-- Pregnancy Status
	insert into riskfactor
			(FactorName, Criteria, Display, IsSystem, Active)
		select 'Pregnancy Status', '(select top 1 div.InstanceValue from Encounter e INNER JOIN DatasetInstance di on e.Id = di.ContextID INNER JOIN DatasetInstanceValue div on div.DatasetInstance_Id = di.Id INNER JOIN DatasetElement de on div.DatasetElement_Id = de.Id where de.DatasetElementGuid = ''9EF95B01-3EA7-433B-B687-E30BB35DE8CA'' and e.Patient_Id = PatientId and e.EncounterDate <= ''#ContextStart#'' order by e.EncounterDate desc) = ''#ContextOption#''', 'Pregnancy Status', 1, 1
	set @id = (SELECT @@IDENTITY )

	insert into riskfactoroption 
			(OptionName, Criteria, Display, RiskFactor_Id)
		select 'Yes', 'Yes', 'Is Pregnant', @id
		union  select 'No', 'No', 'Is Not Pregnant', @id

	-- Breastfeeding
	insert into riskfactor
			(FactorName, Criteria, Display, IsSystem, Active)
		select 'Breastfeeding Status', '(select top 1 div.InstanceValue from Encounter e INNER JOIN DatasetInstance di on e.Id = di.ContextID INNER JOIN DatasetInstanceValue div on div.DatasetInstance_Id = di.Id INNER JOIN DatasetElement de on div.DatasetElement_Id = de.Id where de.DatasetElementGuid = ''DDFACDDF-C383-49AA-8BAE-C36E4BFE64EE'' and e.Patient_Id = PatientId and e.EncounterDate <= ''#ContextStart#'' order by e.EncounterDate desc) = ''#ContextOption#''', 'Breastfeeding Status', 1, 1
	set @id = (SELECT @@IDENTITY )

	insert into riskfactoroption 
			(OptionName, Criteria, Display, RiskFactor_Id)
		select 'Yes', 'Yes', 'Is Breastfeeding', @id
		union  select 'No', 'No', 'Is Not Breastfeeding', @id
	

SELECT * FROM WorkFlow
SELECT * FROM Activity
SELECT * FROM ActivityExecutionStatus
SELECT * FROM dbo.Config 

INSERT INTO PostDeployment
		(ScriptGuid, ScriptFileName, ScriptDescription, RunDate, StatusCode, StatusMessage, RunRank)
	select NEWID(), @ScriptFileName, @ScriptFileName, GETDATE(), 200, NULL, (select max(RunRank) + 1 from PostDeployment)

COMMIT TRAN A1

PRINT ''
PRINT '** SCRIPT COMPLETED SUCCESSFULLY **'

--delete Attachment where ActivityExecutionStatusEvent_Id is not null
--delete ActivityExecutionStatusevent
--delete ActivityExecutionStatus
--delete ActivityInstance
--delete Activity
--delete ReportInstanceMedication
--delete ReportInstance
--delete workflow


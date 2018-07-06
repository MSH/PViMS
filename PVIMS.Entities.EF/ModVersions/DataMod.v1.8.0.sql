DECLARE @ScriptFileName nvarchar(200)
	, @RunDate datetime
SET @ScriptFileName = 'Mods.Data.v1.8.0.sql'

SELECT @RunDate = RunDate  FROM PostDeployment  WHERE ScriptFileName = @ScriptFileName

IF(@RunDate IS NOT NULL) BEGIN
	
	DECLARE @VersionErrorMessage VARCHAR(1024)
	SET @VersionErrorMessage = 'This script has already been executed.'
	RAISERROR(@VersionErrorMessage,16,1)
	RETURN;
	
END

SET NOCOUNT ON

BEGIN TRAN A1

IF EXISTS(select Id from CustomAttributeConfiguration where AttributeKey = 'Remarks' and ExtendableTypeName = 'PatientLabTest') begin
	update CustomAttributeConfiguration set AttributeKey = 'Comments' where AttributeKey = 'Remarks' and ExtendableTypeName = 'PatientLabTest'
end

IF EXISTS(select Id from EncounterType where [Description] = 'Not Set') begin
	declare @count int
	SELECT @count = COUNT(*) from Encounter e inner join EncounterType et on e.EncounterType_Id = et.Id where et.[Description] = 'Not Set'
	-- update encounter set encountertype_id = 2 where encountertype_id = 1
	--update datasetinstance set EncounterTypeWorkPlan_Id = 2 where EncounterTypeWorkPlan_Id = 1
	if @count > 0 begin
		ROLLBACK TRAN A1
		SET @VersionErrorMessage = 'Encounter type not set has existing encounters'
		RAISERROR(@VersionErrorMessage,16,1)
		RETURN;
	end
	delete wp from EncounterType et inner join EncounterTypeWorkPlan wp on et.Id = wp.EncounterType_Id where [Description] = 'Not Set'
	delete EncounterType where [Description] = 'Not Set'
end


UPDATE TerminologyMedDra SET Common = 1 where MedDraTermType = 'LLT' and MedDraTerm IN ('Abdominal pain NOS', 'Allergic reaction (NOS)', 'Anorexia', 'Arthralgia', 'Depression', 'Diarrhoea', 'Dizziness', 'Electrolyte disturbance', 'Gastritis', 'Headache', 'Hearing loss', 'Hepatitis', 'Hepatotoxicity', 'Hypothyroidism', 'Nausea', 'Nephrotoxicity', 'Neuropathy peripheral', 'Psychosis', 'QT prolonged', 'Rash', 'Renal failure', 'Seizures', 'Sleep disturbances', 'Suicidal ideation', 'Tinnitus', 'Vertigo', 'Visual disturbances', 'Vomiting')
SELECT * FROM TerminologyMedDra WHERE MedDraTermType = 'LLT' and MedDraTerm IN ('Abdominal pain NOS', 'Allergic reaction (NOS)', 'Anorexia', 'Arthralgia', 'Depression', 'Diarrhoea', 'Dizziness', 'Electrolyte disturbance', 'Gastritis', 'Headache', 'Hearing loss', 'Hepatitis', 'Hepatotoxicity', 'Hypothyroidism', 'Nausea', 'Nephrotoxicity', 'Neuropathy peripheral', 'Psychosis', 'QT prolonged', 'Rash', 'Renal failure', 'Seizures', 'Sleep disturbances', 'Suicidal ideation', 'Tinnitus', 'Vertigo', 'Visual disturbances', 'Vomiting')

INSERT INTO PostDeployment
		(ScriptGuid, ScriptFileName, ScriptDescription, RunDate, StatusCode, StatusMessage, RunRank)
	select NEWID(), @ScriptFileName, @ScriptFileName, GETDATE(), 200, NULL, (select max(RunRank) + 1 from PostDeployment)

COMMIT TRAN A1

PRINT ''
PRINT '** SCRIPT COMPLETED SUCCESSFULLY **'

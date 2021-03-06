DECLARE @ScriptFileName nvarchar(200)
	, @RunDate datetime
SET @ScriptFileName = 'Mods.Data.v1.5.18.sql'

SELECT @RunDate = RunDate  FROM PostDeployment  WHERE ScriptFileName = @ScriptFileName

IF(@RunDate IS NOT NULL) BEGIN
	
	DECLARE @VersionErrorMessage VARCHAR(1024)
	SET @VersionErrorMessage = 'This script has already been executed.'
	RAISERROR(@VersionErrorMessage,16,1)
	RETURN;
	
END

SET NOCOUNT ON

BEGIN TRAN A1

IF NOT EXISTS(select Id from CustomAttributeConfiguration where ExtendableTypeName = 'PatientClinicalEvent' and AttributeKey = 'Comments') begin
	insert into CustomAttributeConfiguration (ExtendableTypeName, CustomAttributeType, Category, AttributeKey, IsRequired, StringMaxLength, FutureDateOnly, PastDateOnly, IsSearchable) values ('PatientClinicalEvent', 2, 'Custom', 'Comments', 0, 100, 0, 0, 0)
end

IF NOT EXISTS(select Id from CustomAttributeConfiguration where ExtendableTypeName = 'PatientMedication' and AttributeKey = 'Comments') begin
	insert into CustomAttributeConfiguration (ExtendableTypeName, CustomAttributeType, Category, AttributeKey, IsRequired, StringMaxLength, FutureDateOnly, PastDateOnly, IsSearchable) values ('PatientMedication', 2, 'Custom', 'Comments', 0, 100, 0, 0, 0)
end

PRINT 'All Data Modified'

INSERT INTO PostDeployment
		(ScriptGuid, ScriptFileName, ScriptDescription, RunDate, StatusCode, StatusMessage, RunRank)
	select NEWID(), @ScriptFileName, @ScriptFileName, GETDATE(), 200, NULL, (select max(RunRank) + 1 from PostDeployment)

COMMIT TRAN A1

PRINT ''
PRINT '** SCRIPT COMPLETED SUCCESSFULLY **'

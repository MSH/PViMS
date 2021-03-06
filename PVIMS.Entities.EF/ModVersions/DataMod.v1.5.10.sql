DECLARE @ScriptFileName nvarchar(200)
	, @RunDate datetime
SET @ScriptFileName = 'Mods.Data.v1.5.10.sql'

SELECT @RunDate = RunDate  FROM PostDeployment  WHERE ScriptFileName = @ScriptFileName

IF(@RunDate IS NOT NULL) BEGIN
	
	DECLARE @VersionErrorMessage VARCHAR(1024)
	SET @VersionErrorMessage = 'This script has already been executed.'
	RAISERROR(@VersionErrorMessage,16,1)
	RETURN;
	
END

SET NOCOUNT ON

BEGIN TRAN A1

-- Custom attributes
IF EXISTS(select Id from CustomAttributeConfiguration where ExtendableTypeName = 'PatientMedication' and AttributeKey = 'Days/week') begin
	update CustomAttributeConfiguration set AttributeKey = 'Frequency in days per week' where ExtendableTypeName = 'PatientMedication' and AttributeKey = 'Days/week'
	update SelectionDataItem set AttributeKey = 'Frequency in days per week' where AttributeKey = 'Days/week'
end

UPDATE DatasetElement SET DefaultValue = '2' where ElementName in ('Additional Document', 'Duplicate', 'Fulfill Expedite Criteria')
UPDATE DatasetElement SET DefaultValue = 'Phillipines Department of Health' where ElementName in ('Sender Organization')

PRINT 'All Data Modified'

INSERT INTO PostDeployment
		(ScriptGuid, ScriptFileName, ScriptDescription, RunDate, StatusCode, StatusMessage, RunRank)
	select NEWID(), @ScriptFileName, @ScriptFileName, GETDATE(), 200, NULL, (select max(RunRank) + 1 from PostDeployment)

COMMIT TRAN A1

PRINT ''
PRINT '** SCRIPT COMPLETED SUCCESSFULLY **'

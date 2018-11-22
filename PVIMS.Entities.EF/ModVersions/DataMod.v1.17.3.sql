DECLARE @ScriptFileName nvarchar(200)
	, @RunDate datetime
SET @ScriptFileName = 'Mods.Data.v1.17.3.sql'

SELECT @RunDate = RunDate  FROM PostDeployment  WHERE ScriptFileName = @ScriptFileName

IF(@RunDate IS NOT NULL) BEGIN
	
	DECLARE @VersionErrorMessage VARCHAR(1024)
	SET @VersionErrorMessage = 'This script has already been executed.'
	RAISERROR(@VersionErrorMessage,16,1)
	RETURN;
	
END

SET NOCOUNT ON

BEGIN TRAN A1

	UPDATE MetaWidgetType SET [Description] = 'SubItems' WHERE metawidgettype_guid = '58DD7339-B9E0-4CBB-A47B-B2C027D6DF86'
 
INSERT INTO PostDeployment
		(ScriptGuid, ScriptFileName, ScriptDescription, RunDate, StatusCode, StatusMessage, RunRank)
	select NEWID(), @ScriptFileName, @ScriptFileName, GETDATE(), 200, NULL, (select max(RunRank) + 1 from PostDeployment)

COMMIT TRAN A1

PRINT ''
PRINT '** SCRIPT COMPLETED SUCCESSFULLY **'

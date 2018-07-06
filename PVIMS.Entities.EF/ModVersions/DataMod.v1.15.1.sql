DECLARE @ScriptFileName nvarchar(200)
	, @RunDate datetime
SET @ScriptFileName = 'Mods.Data.v1.15.1.sql'

SELECT @RunDate = RunDate  FROM PostDeployment  WHERE ScriptFileName = @ScriptFileName

IF(@RunDate IS NOT NULL) BEGIN
	
	DECLARE @VersionErrorMessage VARCHAR(1024)
	SET @VersionErrorMessage = 'This script has already been executed.'
	RAISERROR(@VersionErrorMessage,16,1)
	RETURN;
	
END

SET NOCOUNT ON

BEGIN TRAN A1

	IF NOT EXISTS(SELECT Id FROM Config WHERE ConfigType = 7) begin
		INSERT INTO Config (ConfigType, ConfigValue, Created, LastUpdated, CreatedBy_Id, UpdatedBy_Id) VALUES (7, '', GETDATE(), GETDATE(), 1, 1)
	end

	SELECT * FROM Config

INSERT INTO PostDeployment
		(ScriptGuid, ScriptFileName, ScriptDescription, RunDate, StatusCode, StatusMessage, RunRank)
	select NEWID(), @ScriptFileName, @ScriptFileName, GETDATE(), 200, NULL, (select max(RunRank) + 1 from PostDeployment)

COMMIT TRAN A1

PRINT ''
PRINT '** SCRIPT COMPLETED SUCCESSFULLY **'

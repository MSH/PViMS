DECLARE @ScriptFileName nvarchar(200)
	, @RunDate datetime
SET @ScriptFileName = 'Mods.Data.v1.5.3.sql'

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
IF EXISTS(select Id from CustomAttributeConfiguration where ExtendableTypeName = 'Patient' and AttributeKey = 'MaritalStatus') begin
	update CustomAttributeConfiguration set AttributeKey = 'Marital Status' where ExtendableTypeName = 'Patient' and AttributeKey = 'MaritalStatus'
	update SelectionDataItem set AttributeKey = 'Marital Status' where AttributeKey = 'MaritalStatus'
end

--Encounter elements
update DatasetElement set ElementName = 'Injecting drug use within past year' where ElementName = 'Injecting Drug Use Within Past Year'
update DatasetElement set [System] = 1 where elementName = 'BMI'
update DatasetCategory set DatasetCategoryName = 'First-line Susceptibility' where DatasetCategoryName = '1st Line Susceptibility'
update DatasetCategory set DatasetCategoryName = 'Second-line Susceptibility' where DatasetCategoryName = '2nd Line Susceptibility'
update Medication set Drugname = REPLACE(DrugName, N'‐', '-')
update Medication set Drugname = 'fresh-frozen plasma' where DrugName = 'fresh–frozen plasma           '
update Medication set Drugname = 'Lugol''s solution' where DrugName like 'Lug%'

PRINT 'All Data Modified'

INSERT INTO PostDeployment
		(ScriptGuid, ScriptFileName, ScriptDescription, RunDate, StatusCode, StatusMessage, RunRank)
	select NEWID(), @ScriptFileName, @ScriptFileName, GETDATE(), 200, NULL, (select max(RunRank) + 1 from PostDeployment)

COMMIT TRAN A1

PRINT ''
PRINT '** SCRIPT COMPLETED SUCCESSFULLY **'

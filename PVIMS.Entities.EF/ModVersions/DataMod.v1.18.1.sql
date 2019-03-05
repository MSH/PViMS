DECLARE @ScriptFileName nvarchar(200)
	, @RunDate datetime
SET @ScriptFileName = 'Mods.Data.v1.18.1.sql'

SELECT @RunDate = RunDate  FROM PostDeployment  WHERE ScriptFileName = @ScriptFileName

IF(@RunDate IS NOT NULL) BEGIN
	
	DECLARE @VersionErrorMessage VARCHAR(1024)
	SET @VersionErrorMessage = 'This script has already been executed.'
	RAISERROR(@VersionErrorMessage,16,1)
	RETURN;
	
END

SET NOCOUNT ON

BEGIN TRAN A1
	
	-- Change ordering of product information elements for spontaneous report
	DECLARE @ds_id int
	SELECT @ds_id = Id FROM Dataset where  DatasetName = 'Spontaneous Report'
	
	UPDATE desu SET desu.FieldOrder = 3
		FROM Dataset ds
			INNER JOIN DatasetCategory dc ON ds.Id = dc.Dataset_Id 
			INNER JOIN DatasetCategoryElement dce ON dc.Id = dce.DatasetCategory_Id 
			INNER JOIN DatasetElement de ON dce.DatasetElement_Id = de.Id 
			INNER JOIN DatasetElementSub desu ON desu.DatasetElement_Id = de.Id 
			INNER JOIN Field f ON de.Field_Id = f.Id
			INNER JOIN FieldType ft ON f.FieldType_Id = ft.Id
		WHERE ds.Id = @ds_id and de.ElementName = 'Product Information' and desu.ElementName = 'Drug strength'

	UPDATE desu SET desu.FieldOrder = 2
		FROM Dataset ds
			INNER JOIN DatasetCategory dc ON ds.Id = dc.Dataset_Id 
			INNER JOIN DatasetCategoryElement dce ON dc.Id = dce.DatasetCategory_Id 
			INNER JOIN DatasetElement de ON dce.DatasetElement_Id = de.Id 
			INNER JOIN DatasetElementSub desu ON desu.DatasetElement_Id = de.Id 
			INNER JOIN Field f ON de.Field_Id = f.Id
			INNER JOIN FieldType ft ON f.FieldType_Id = ft.Id
		WHERE ds.Id = @ds_id and de.ElementName = 'Product Information' and desu.ElementName = 'Product Suspected'
	 
	SELECT ds.DatasetName, dc.DatasetCategoryName, de.ElementName, desu.ElementName, desu.FieldOrder, ROW_NUMBER() OVER(ORDER BY ds.DatasetName, dc.DatasetCategoryName, de.ElementName, desu.ElementName, desu.FieldOrder ASC) AS Row#
		FROM Dataset ds
			INNER JOIN DatasetCategory dc ON ds.Id = dc.Dataset_Id 
			INNER JOIN DatasetCategoryElement dce ON dc.Id = dce.DatasetCategory_Id 
			INNER JOIN DatasetElement de ON dce.DatasetElement_Id = de.Id 
			INNER JOIN DatasetElementSub desu ON desu.DatasetElement_Id = de.Id 
			INNER JOIN Field f ON de.Field_Id = f.Id
			INNER JOIN FieldType ft ON f.FieldType_Id = ft.Id
		where ds.Id = @ds_id and de.ElementName = 'Product Information'
		order by dc.CategoryOrder, dc.DatasetCategoryName, dce.Id, desu.FieldOrder
 
	-- Remove unused meta columns
	DELETE MetaColumn WHERE ColumnName in ('CurrentFacilityName', 'LatestEncounterDate')
 
INSERT INTO PostDeployment
		(ScriptGuid, ScriptFileName, ScriptDescription, RunDate, StatusCode, StatusMessage, RunRank)
	select NEWID(), @ScriptFileName, @ScriptFileName, GETDATE(), 200, NULL, (select max(RunRank) + 1 from PostDeployment)

COMMIT TRAN A1

PRINT ''
PRINT '** SCRIPT COMPLETED SUCCESSFULLY **'

DECLARE @ScriptFileName nvarchar(200)
	, @RunDate datetime
SET @ScriptFileName = 'Mods.Data.v1.12.0.sql'

SELECT @RunDate = RunDate  FROM PostDeployment  WHERE ScriptFileName = @ScriptFileName

IF(@RunDate IS NOT NULL) BEGIN
	
	DECLARE @VersionErrorMessage VARCHAR(1024)
	SET @VersionErrorMessage = 'This script has already been executed.'
	RAISERROR(@VersionErrorMessage,16,1)
	RETURN;
	
END

SET NOCOUNT ON

BEGIN TRAN A1

	DELETE MetaWidgetType 

	SET IDENTITY_INSERT MetaWidgetType ON	
	INSERT INTO MetaWidgetType (Id, metawidgettype_guid, [Description]) VALUES (1, NEWID(), 'General')
	INSERT INTO MetaWidgetType (Id, metawidgettype_guid, [Description]) VALUES (2, NEWID(), 'Wiki')
	INSERT INTO MetaWidgetType (Id, metawidgettype_guid, [Description]) VALUES (3, NEWID(), 'ItemList')
	SET IDENTITY_INSERT MetaWidgetType OFF

	SELECT * FROM MetaWidgetType
	
	IF NOT EXISTS(select id from Config where ConfigType = 6) begin
		INSERT INTO Config (ConfigType, ConfigValue, Created, LastUpdated, CreatedBy_Id, UpdatedBy_Id) VALUES (6, '', GETDATE(), GETDATE(), 1, 1)
	end
	SELECT * FROM Config

	-- Remove reaction serious field
	DECLARE @fid int
	SELECT @fid = de.Field_Id 
		FROM Dataset ds
		INNER JOIN DatasetCategory dc ON ds.Id = dc.Dataset_Id 
		INNER JOIN DatasetCategoryElement dce ON dc.Id = dce.DatasetCategory_Id 
		INNER JOIN DatasetElement de ON dce.DatasetElement_Id = de.Id 
		INNER JOIN Field f ON de.Field_Id = f.Id
		INNER JOIN FieldType ft ON f.FieldType_Id = ft.Id
		LEFT JOIN FieldValue fv ON f.Id = fv.Field_Id 
	where ds.DatasetName = 'Spontaneous Report'
		and de.Elementname = 'Reaction serious'
	Update de set de.Field_Id = null
		FROM Dataset ds
		INNER JOIN DatasetCategory dc ON ds.Id = dc.Dataset_Id 
		INNER JOIN DatasetCategoryElement dce ON dc.Id = dce.DatasetCategory_Id 
		INNER JOIN DatasetElement de ON dce.DatasetElement_Id = de.Id 
		INNER JOIN Field f ON de.Field_Id = f.Id
		INNER JOIN FieldType ft ON f.FieldType_Id = ft.Id
		LEFT JOIN FieldValue fv ON f.Id = fv.Field_Id 
	where ds.DatasetName = 'Spontaneous Report'
		and de.Elementname = 'Reaction serious'
	DELETE Field where Id = @Fid
	DELETE dmv
		FROM Dataset ds
		INNER JOIN DatasetCategory dc ON ds.Id = dc.Dataset_Id 
		INNER JOIN DatasetCategoryElement dce ON dc.Id = dce.DatasetCategory_Id 
		INNER JOIN DatasetElement de ON dce.DatasetElement_Id = de.Id 
		INNER JOIN DatasetMapping dm ON dm.SourceElement_Id = dce.Id
		INNER JOIN DatasetMappingValue dmv ON dm.Id = dmv.Mapping_Id
	where ds.DatasetName = 'Spontaneous Report'
	and de.Elementname = 'Reaction serious'
	DELETE dm
		FROM Dataset ds
		INNER JOIN DatasetCategory dc ON ds.Id = dc.Dataset_Id 
		INNER JOIN DatasetCategoryElement dce ON dc.Id = dce.DatasetCategory_Id 
		INNER JOIN DatasetElement de ON dce.DatasetElement_Id = de.Id 
		INNER JOIN DatasetMapping dm ON dm.SourceElement_Id = dce.Id
	where ds.DatasetName = 'Spontaneous Report'
	and de.Elementname = 'Reaction serious'
	DELETE dce
		FROM Dataset ds
		INNER JOIN DatasetCategory dc ON ds.Id = dc.Dataset_Id 
		INNER JOIN DatasetCategoryElement dce ON dc.Id = dce.DatasetCategory_Id 
		INNER JOIN DatasetElement de ON dce.DatasetElement_Id = de.Id 
	where ds.DatasetName = 'Spontaneous Report'
	and de.Elementname = 'Reaction serious'
	DELETE div
		FROM DatasetElement de
			INNER JOIN DatasetInstanceValue div ON de.Id = div.DatasetElement_Id 
	where Elementname = 'Reaction serious'
	DELETE DatasetElement WHERE Elementname = 'Reaction serious'

INSERT INTO PostDeployment
		(ScriptGuid, ScriptFileName, ScriptDescription, RunDate, StatusCode, StatusMessage, RunRank)
	select NEWID(), @ScriptFileName, @ScriptFileName, GETDATE(), 200, NULL, (select max(RunRank) + 1 from PostDeployment)

COMMIT TRAN A1

PRINT ''
PRINT '** SCRIPT COMPLETED SUCCESSFULLY **'

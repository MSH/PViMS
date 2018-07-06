DECLARE @Id int
SELECT @Id = Id   FROM Dataset  WHERE DatasetName = 'Chronic Treatment'

IF(@Id IS NOT NULL) BEGIN
	
	DECLARE @VersionErrorMessage VARCHAR(1024)
	SET @VersionErrorMessage = 'This script has already been executed.'
	RAISERROR(@VersionErrorMessage,16,1)
	RETURN;
	
END

SET NOCOUNT ON

BEGIN TRAN A1

/**************************************************
DATASET
**************************************************/
DECLARE @dsid int
INSERT [dbo].[Dataset] ([DatasetName], [Active], [InitialiseProcess], [RulesProcess], [Help], [Created], [LastUpdated], [ContextType_Id], [CreatedBy_Id], [UpdatedBy_Id], [EncounterTypeWorkPlan_Id]) 
	VALUES (N'Chronic Treatment', 1, NULL, NULL, N'Standard treatment dataset for chronic patients', GETDATE(), GETDATE(), 1, 1, 1, NULL)
set @dsid = (SELECT @@IDENTITY)

/**************************************************
CATEGORY Medical Details
**************************************************/
DECLARE @dscid int	
INSERT [dbo].[DatasetCategory] ([DatasetCategoryName], [CategoryOrder], [Dataset_Id]) 
	VALUES (N'Medical Details', 1, @dsid)
set @dscid = (SELECT @@IDENTITY)

-- Weight (kg)
DECLARE @fid int
DECLARE @deid int
DECLARE @dceid int
INSERT [dbo].[Field] ([Anonymise], [Mandatory], [Decimals], [MaxSize], [MinSize], [FieldType_Id]) 
	VALUES (0, 1, 1, 159.90, 1.10, 4)
set @fid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetElement] ([ElementName], [Field_Id], [DatasetElementType_Id], [OID], [DefaultValue], [System]) 
	VALUES (N'Weight (kg)', @fid, 1, NULL, NULL, 0)
set @deid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetCategoryElement] ([FieldOrder], [DatasetCategory_Id], [DatasetElement_Id], [Acute], [Chronic]) 
	select ISNULL(MAX(FieldOrder) + 1, 1), @dscid, @deid, 1, 0 from [DatasetCategoryElement] where [DatasetCategory_Id] = @dscid
set @dceid = (SELECT @@IDENTITY)

-- Height (cm)
INSERT [dbo].[Field] ([Anonymise], [Mandatory], [Decimals], [MaxSize], [MinSize], [FieldType_Id]) 
	VALUES (0, 0, 0, 250.00, 1.00, 4)
set @fid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetElement] ([ElementName], [Field_Id], [DatasetElementType_Id], [OID], [DefaultValue], [System]) 
	VALUES (N'Height (cm)', @fid, 1, NULL, NULL, 0)
set @deid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetCategoryElement] ([FieldOrder], [DatasetCategory_Id], [DatasetElement_Id], [Acute], [Chronic]) 
	select ISNULL(MAX(FieldOrder) + 1, 1), @dscid, @deid, 1, 0 from [DatasetCategoryElement] where [DatasetCategory_Id] = @dscid
set @dceid = (SELECT @@IDENTITY)

-- BMI
INSERT [dbo].[Field] ([Anonymise], [Mandatory], [Decimals], [MaxSize], [MinSize], [FieldType_Id]) 
	VALUES (0, 0, 1, 60.00, 0.00, 4)
set @fid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetElement] ([ElementName], [Field_Id], [DatasetElementType_Id], [OID], [DefaultValue], [System]) 
	VALUES (N'BMI', @fid, 1, NULL, NULL, 1)
set @deid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetCategoryElement] ([FieldOrder], [DatasetCategory_Id], [DatasetElement_Id], [Acute], [Chronic]) 
	select ISNULL(MAX(FieldOrder) + 1, 1), @dscid, @deid, 1, 0 from [DatasetCategoryElement] where [DatasetCategory_Id] = @dscid
set @dceid = (SELECT @@IDENTITY)

-- Pregnancy status
INSERT [dbo].[Field] ([Anonymise], [Mandatory], [FieldType_Id]) 
	VALUES (0, 0, 2)
set @fid = (SELECT @@IDENTITY)
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
	VALUES ('Yes', 1, 0, 0, @fid)
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
	VALUES ('No', 0, 0, 0, @fid)
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
	VALUES ('NA', 0, 0, 0, @fid)
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
	VALUES ('Uncertain', 0, 0, 0, @fid)
INSERT [dbo].[DatasetElement] ([ElementName], [Field_Id], [DatasetElementType_Id], [OID], [DefaultValue], [System]) 
	VALUES (N'Pregnancy status', @fid, 1, NULL, NULL, 0)
set @deid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetCategoryElement] ([FieldOrder], [DatasetCategory_Id], [DatasetElement_Id], [Acute], [Chronic]) 
	select ISNULL(MAX(FieldOrder) + 1, 1), @dscid, @deid, 0, 0 from [DatasetCategoryElement] where [DatasetCategory_Id] = @dscid
set @dceid = (SELECT @@IDENTITY)

-- Date of last menstrual period
INSERT [dbo].[Field] ([Anonymise], [Mandatory], [FieldType_Id]) 
	VALUES (0, 0, 6)
set @fid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetElement] ([ElementName], [Field_Id], [DatasetElementType_Id], [OID], [DefaultValue], [System]) 
	VALUES (N'Date of last menstrual period', @fid, 1, NULL, NULL, 0)
set @deid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetCategoryElement] ([FieldOrder], [DatasetCategory_Id], [DatasetElement_Id], [Acute], [Chronic]) 
	select ISNULL(MAX(FieldOrder) + 1, 1), @dscid, @deid, 1, 0 from [DatasetCategoryElement] where [DatasetCategory_Id] = @dscid
set @dceid = (SELECT @@IDENTITY)

-- Estimated gestation (weeks)
INSERT [dbo].[Field] ([Anonymise], [Mandatory], [Decimals], [MaxSize], [MinSize], [FieldType_Id]) 
	VALUES (0, 0, 0, 44.00, 1.00, 4)
set @fid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetElement] ([ElementName], [Field_Id], [DatasetElementType_Id], [OID], [DefaultValue], [System]) 
	VALUES (N'Estimated gestation (weeks)', @fid, 1, NULL, NULL, 0)
set @deid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetCategoryElement] ([FieldOrder], [DatasetCategory_Id], [DatasetElement_Id], [Acute], [Chronic]) 
	select ISNULL(MAX(FieldOrder) + 1, 1), @dscid, @deid, 1, 0 from [DatasetCategoryElement] where [DatasetCategory_Id] = @dscid
set @dceid = (SELECT @@IDENTITY)

-- Breastfeeding mother
INSERT [dbo].[Field] ([Anonymise], [Mandatory], [FieldType_Id]) 
	VALUES (0, 0, 2)
set @fid = (SELECT @@IDENTITY)
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
	VALUES ('Yes', 1, 0, 0, @fid)
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
	VALUES ('No', 0, 0, 0, @fid)
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
	VALUES ('NA', 0, 0, 0, @fid)
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
	VALUES ('Unknown', 0, 0, 0, @fid)
INSERT [dbo].[DatasetElement] ([ElementName], [Field_Id], [DatasetElementType_Id], [OID], [DefaultValue], [System]) 
	VALUES (N'Breastfeeding mother', @fid, 1, NULL, NULL, 0)
set @deid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetCategoryElement] ([FieldOrder], [DatasetCategory_Id], [DatasetElement_Id], [Acute], [Chronic]) 
	select ISNULL(MAX(FieldOrder) + 1, 1), @dscid, @deid, 0, 0 from [DatasetCategoryElement] where [DatasetCategory_Id] = @dscid
set @dceid = (SELECT @@IDENTITY)

-- Injecting drug use within past year
INSERT [dbo].[Field] ([Anonymise], [Mandatory], [FieldType_Id]) 
	VALUES (0, 0, 2)
set @fid = (SELECT @@IDENTITY)
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
	VALUES ('Yes', 1, 0, 0, @fid)
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
	VALUES ('No', 0, 0, 0, @fid)
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
	VALUES ('Unknown', 0, 0, 0, @fid)
INSERT [dbo].[DatasetElement] ([ElementName], [Field_Id], [DatasetElementType_Id], [OID], [DefaultValue], [System]) 
	VALUES (N'Injecting drug use within past year', @fid, 1, NULL, NULL, 0)
set @deid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetCategoryElement] ([FieldOrder], [DatasetCategory_Id], [DatasetElement_Id], [Acute], [Chronic]) 
	select ISNULL(MAX(FieldOrder) + 1, 1), @dscid, @deid, 0, 0 from [DatasetCategoryElement] where [DatasetCategory_Id] = @dscid
set @dceid = (SELECT @@IDENTITY)

-- Excessive alcohol use within the past year
INSERT [dbo].[Field] ([Anonymise], [Mandatory], [FieldType_Id]) 
	VALUES (0, 0, 2)
set @fid = (SELECT @@IDENTITY)
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
	VALUES ('Yes', 1, 0, 0, @fid)
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
	VALUES ('No', 0, 0, 0, @fid)
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
	VALUES ('Unknown', 0, 0, 0, @fid)
INSERT [dbo].[DatasetElement] ([ElementName], [Field_Id], [DatasetElementType_Id], [OID], [DefaultValue], [System]) 
	VALUES (N'Excessive alcohol use within the past year', @fid, 1, NULL, NULL, 0)
set @deid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetCategoryElement] ([FieldOrder], [DatasetCategory_Id], [DatasetElement_Id], [Acute], [Chronic]) 
	select ISNULL(MAX(FieldOrder) + 1, 1), @dscid, @deid, 0, 0 from [DatasetCategoryElement] where [DatasetCategory_Id] = @dscid
set @dceid = (SELECT @@IDENTITY)

-- Tobacco use within the past year
INSERT [dbo].[Field] ([Anonymise], [Mandatory], [FieldType_Id]) 
	VALUES (0, 0, 2)
set @fid = (SELECT @@IDENTITY)
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
	VALUES ('Yes', 1, 0, 0, @fid)
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
	VALUES ('No', 0, 0, 0, @fid)
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
	VALUES ('Unknown', 0, 0, 0, @fid)
INSERT [dbo].[DatasetElement] ([ElementName], [Field_Id], [DatasetElementType_Id], [OID], [DefaultValue], [System]) 
	VALUES (N'Tobacco use within the past year', @fid, 1, NULL, NULL, 0)
set @deid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetCategoryElement] ([FieldOrder], [DatasetCategory_Id], [DatasetElement_Id], [Acute], [Chronic]) 
	select ISNULL(MAX(FieldOrder) + 1, 1), @dscid, @deid, 0, 0 from [DatasetCategoryElement] where [DatasetCategory_Id] = @dscid
set @dceid = (SELECT @@IDENTITY)

/**************************************************
CATEGORY TB Condition
**************************************************/
INSERT [dbo].[DatasetCategory] ([DatasetCategoryName], [CategoryOrder], [Dataset_Id]) 
	VALUES (N'TB Condition', 2, @dsid)
set @dscid = (SELECT @@IDENTITY)

-- Condition MedDra Terminology
--insert into Field
--		(Mandatory, Anonymise, FieldType_Id)
--	select 0, 0, 8
--set @fid = (SELECT @@IDENTITY)
--insert into DatasetElement
--		(ElementName, Field_Id, DatasetElementType_Id, OID, DefaultValue, [System])
--	select 'Condition MedDRA Terminology', @fid, 1, NULL, NULL, 1
--set @deid = (SELECT @@IDENTITY)
--insert into DatasetCategoryElement
--		(FieldOrder, DatasetCategory_Id, DatasetElement_Id, Acute, Chronic)
--	select (select isnull(max(FieldOrder) + 1, 1) from DatasetCategoryElement where DatasetCategory_Id = @dscid), @dscid, @deid, 1, 0

---- Condition Start Date
--insert into Field
--		(Mandatory, Anonymise, FieldType_Id)
--	select 0, 0, 8
--set @fid = (SELECT @@IDENTITY)
--insert into DatasetElement
--		(ElementName, Field_Id, DatasetElementType_Id, OID, DefaultValue, [System])
--	select 'Condition Start Date', @fid, 1, NULL, NULL, 1
--set @deid = (SELECT @@IDENTITY)
--insert into DatasetCategoryElement
--		(FieldOrder, DatasetCategory_Id, DatasetElement_Id, Acute, Chronic)
--	select (select max(FieldOrder) + 1 from DatasetCategoryElement where DatasetCategory_Id = @dscid), @dscid, @deid, 1, 0

-- Previous TB treatment?
INSERT [dbo].[Field] ([Anonymise], [Mandatory], [FieldType_Id]) 
	VALUES (0, 0, 2)
set @fid = (SELECT @@IDENTITY)
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
	VALUES ('Yes', 1, 0, 0, @fid)
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
	VALUES ('No', 0, 0, 0, @fid)
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
	VALUES ('Unknown', 0, 0, 0, @fid)
INSERT [dbo].[DatasetElement] ([ElementName], [Field_Id], [DatasetElementType_Id], [OID], [DefaultValue], [System]) 
	VALUES (N'Previous TB treatment?', @fid, 1, NULL, NULL, 0)
set @deid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetCategoryElement] ([FieldOrder], [DatasetCategory_Id], [DatasetElement_Id], [Acute], [Chronic]) 
	select ISNULL(MAX(FieldOrder) + 1, 1), @dscid, @deid, 0, 1 from [DatasetCategoryElement] where [DatasetCategory_Id] = @dscid
set @dceid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetCategoryElementCondition] (Condition_Id, DatasetCategoryElement_Id) 
	select Id, @dceid From Condition where [Description] = 'TB'

-- Site of TB
INSERT [dbo].[Field] ([Anonymise], [Mandatory], [FieldType_Id]) 
	VALUES (0, 0, 2)
set @fid = (SELECT @@IDENTITY)
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
	VALUES ('PTB only', 1, 0, 0, @fid)
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
	VALUES ('PTB+EPTB', 0, 0, 0, @fid)
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
	VALUES ('EPTB only', 0, 0, 0, @fid)
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
	VALUES ('Unknown', 0, 0, 0, @fid)
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
	VALUES ('Pleural Lymphatic', 0, 0, 0, @fid)
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
	VALUES ('Intrathoracic Lymphatic', 0, 0, 0, @fid)
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
	VALUES ('Extrathroracic', 0, 0, 0, @fid)
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
	VALUES ('Genito-urinary', 0, 0, 0, @fid)
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
	VALUES ('Osteo-articular', 0, 0, 0, @fid)
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
	VALUES ('Disseminated', 0, 0, 0, @fid)
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
	VALUES ('Peritoneal & Digestive', 0, 0, 0, @fid)
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
	VALUES ('Central nervous system', 0, 0, 0, @fid)
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
	VALUES ('Other', 0, 1, 0, @fid)
INSERT [dbo].[DatasetElement] ([ElementName], [Field_Id], [DatasetElementType_Id], [OID], [DefaultValue], [System]) 
	VALUES (N'Site of TB', @fid, 1, NULL, NULL, 0)
set @deid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetCategoryElement] ([FieldOrder], [DatasetCategory_Id], [DatasetElement_Id], [Acute], [Chronic]) 
	select ISNULL(MAX(FieldOrder) + 1, 1), @dscid, @deid, 0, 1 from [DatasetCategoryElement] where [DatasetCategory_Id] = @dscid
set @dceid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetCategoryElementCondition] (Condition_Id, DatasetCategoryElement_Id) 
	select Id, @dceid From Condition where [Description] = 'TB'

-- Documented HIV infection
INSERT [dbo].[Field] ([Anonymise], [Mandatory], [FieldType_Id]) 
	VALUES (0, 0, 2)
set @fid = (SELECT @@IDENTITY)
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
	VALUES ('Yes', 1, 0, 0, @fid)
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
	VALUES ('No', 0, 0, 0, @fid)
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
	VALUES ('Unknown', 0, 0, 0, @fid)
INSERT [dbo].[DatasetElement] ([ElementName], [Field_Id], [DatasetElementType_Id], [OID], [DefaultValue], [System]) 
	VALUES (N'Documented HIV infection', @fid, 1, NULL, NULL, 0)
set @deid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetCategoryElement] ([FieldOrder], [DatasetCategory_Id], [DatasetElement_Id], [Acute], [Chronic]) 
	select ISNULL(MAX(FieldOrder) + 1, 1), @dscid, @deid, 0, 1 from [DatasetCategoryElement] where [DatasetCategory_Id] = @dscid
set @dceid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetCategoryElementCondition] (Condition_Id, DatasetCategoryElement_Id) 
	select Id, @dceid From Condition where [Description] = 'TB'

update dce set dce.FieldOrder = 1 from DatasetCategoryElement dce inner join DatasetElement de on dce.DatasetElement_Id = de.Id where ElementName = 'Condition MedDRA Terminology'
update dce set dce.FieldOrder = 2 from DatasetCategoryElement dce inner join DatasetElement de on dce.DatasetElement_Id = de.Id where ElementName = 'Condition Start Date'
update dce set dce.FieldOrder = 3 from DatasetCategoryElement dce inner join DatasetElement de on dce.DatasetElement_Id = de.Id where ElementName = 'Previous TB treatment?'
update dce set dce.FieldOrder = 4 from DatasetCategoryElement dce inner join DatasetElement de on dce.DatasetElement_Id = de.Id where ElementName = 'Site of TB'
update dce set dce.FieldOrder = 5 from DatasetCategoryElement dce inner join DatasetElement de on dce.DatasetElement_Id = de.Id where ElementName = 'Documented HIV infection'
update dce set dce.FieldOrder = 6 from DatasetCategoryElement dce inner join DatasetElement de on dce.DatasetElement_Id = de.Id where ElementName = 'Ever received treatment with first line anti-TB drugs for >-1 month prior to this episode?'
update dce set dce.FieldOrder = 7 from DatasetCategoryElement dce inner join DatasetElement de on dce.DatasetElement_Id = de.Id where ElementName = 'Ever received treatment with second line anti-TB drugs for >-1 month prior to this episode?'
select dc.DatasetCategoryName, dce.Id, dce.FieldOrder, de.ElementName from DatasetCategoryElement dce inner join DatasetCategory dc on dce.DatasetCategory_Id = dc.Id inner join DatasetElement de on dce.DatasetElement_Id = de.Id where dc.DatasetCategoryName = 'TB Condition' order by FieldOrder

/**************************************************
CATEGORY First Line Susceptibility
**************************************************/
INSERT [dbo].[DatasetCategory] ([DatasetCategoryName], [CategoryOrder], [Dataset_Id]) 
	VALUES (N'First Line Susceptibility', 3, @dsid)
set @dscid = (SELECT @@IDENTITY)

-- Isoniazid susceptibility by any laboratory test(s)
INSERT [dbo].[Field] ([Anonymise], [Mandatory], [FieldType_Id]) 
	VALUES (0, 0, 2)
set @fid = (SELECT @@IDENTITY)
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
	VALUES ('Susceptible', 1, 0, 0, @fid)
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
	VALUES ('Resistant', 0, 0, 0, @fid)
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
	VALUES ('Indeterminate', 0, 0, 0, @fid)
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
	VALUES ('Unknown', 0, 0, 0, @fid)
INSERT [dbo].[DatasetElement] ([ElementName], [Field_Id], [DatasetElementType_Id], [OID], [DefaultValue], [System]) 
	VALUES (N'Isoniazid susceptibility by any laboratory test(s)', @fid, 1, NULL, NULL, 0)
set @deid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetCategoryElement] ([FieldOrder], [DatasetCategory_Id], [DatasetElement_Id], [Acute], [Chronic]) 
	select ISNULL(MAX(FieldOrder) + 1, 1), @dscid, @deid, 0, 0 from [DatasetCategoryElement] where [DatasetCategory_Id] = @dscid
set @dceid = (SELECT @@IDENTITY)

-- Isoniazid confirmation
INSERT [dbo].[Field] ([Anonymise], [Mandatory], [FieldType_Id]) 
	VALUES (0, 0, 2)
set @fid = (SELECT @@IDENTITY)
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
	VALUES ('Xpert', 1, 0, 0, @fid)
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
	VALUES ('LPA', 0, 0, 0, @fid)
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
	VALUES ('Unknown', 0, 0, 0, @fid)
INSERT [dbo].[DatasetElement] ([ElementName], [Field_Id], [DatasetElementType_Id], [OID], [DefaultValue], [System]) 
	VALUES (N'Isoniazid confirmation', @fid, 1, NULL, NULL, 0)
set @deid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetCategoryElement] ([FieldOrder], [DatasetCategory_Id], [DatasetElement_Id], [Acute], [Chronic]) 
	select ISNULL(MAX(FieldOrder) + 1, 1), @dscid, @deid, 0, 0 from [DatasetCategoryElement] where [DatasetCategory_Id] = @dscid
set @dceid = (SELECT @@IDENTITY)

-- Rifampicin susceptibility by any laboratory test(s)
INSERT [dbo].[Field] ([Anonymise], [Mandatory], [FieldType_Id]) 
	VALUES (0, 0, 2)
set @fid = (SELECT @@IDENTITY)
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
	VALUES ('Susceptible', 1, 0, 0, @fid)
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
	VALUES ('Resistant', 0, 0, 0, @fid)
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
	VALUES ('Indeterminate', 0, 0, 0, @fid)
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
	VALUES ('Unknown', 0, 0, 0, @fid)
INSERT [dbo].[DatasetElement] ([ElementName], [Field_Id], [DatasetElementType_Id], [OID], [DefaultValue], [System]) 
	VALUES (N'Rifampicin susceptibility by any laboratory test(s)', @fid, 1, NULL, NULL, 0)
set @deid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetCategoryElement] ([FieldOrder], [DatasetCategory_Id], [DatasetElement_Id], [Acute], [Chronic]) 
	select ISNULL(MAX(FieldOrder) + 1, 1), @dscid, @deid, 0, 0 from [DatasetCategoryElement] where [DatasetCategory_Id] = @dscid
set @dceid = (SELECT @@IDENTITY)

-- Rifampicin confirmation
INSERT [dbo].[Field] ([Anonymise], [Mandatory], [FieldType_Id]) 
	VALUES (0, 0, 2)
set @fid = (SELECT @@IDENTITY)
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
	VALUES ('Xpert', 1, 0, 0, @fid)
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
	VALUES ('LPA', 0, 0, 0, @fid)
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
	VALUES ('Unknown', 0, 0, 0, @fid)
INSERT [dbo].[DatasetElement] ([ElementName], [Field_Id], [DatasetElementType_Id], [OID], [DefaultValue], [System]) 
	VALUES (N'Rifampicin confirmation', @fid, 1, NULL, NULL, 0)
set @deid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetCategoryElement] ([FieldOrder], [DatasetCategory_Id], [DatasetElement_Id], [Acute], [Chronic]) 
	select ISNULL(MAX(FieldOrder) + 1, 1), @dscid, @deid, 0, 0 from [DatasetCategoryElement] where [DatasetCategory_Id] = @dscid
set @dceid = (SELECT @@IDENTITY)

-- Ethambutol susceptibility by any laboratory test(s)
INSERT [dbo].[Field] ([Anonymise], [Mandatory], [FieldType_Id]) 
	VALUES (0, 0, 2)
set @fid = (SELECT @@IDENTITY)
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
	VALUES ('Susceptible', 1, 0, 0, @fid)
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
	VALUES ('Resistant', 0, 0, 0, @fid)
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
	VALUES ('Indeterminate', 0, 0, 0, @fid)
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
	VALUES ('Unknown', 0, 0, 0, @fid)
INSERT [dbo].[DatasetElement] ([ElementName], [Field_Id], [DatasetElementType_Id], [OID], [DefaultValue], [System]) 
	VALUES (N'Ethambutol susceptibility by any laboratory test(s)', @fid, 1, NULL, NULL, 0)
set @deid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetCategoryElement] ([FieldOrder], [DatasetCategory_Id], [DatasetElement_Id], [Acute], [Chronic]) 
	select ISNULL(MAX(FieldOrder) + 1, 1), @dscid, @deid, 0, 0 from [DatasetCategoryElement] where [DatasetCategory_Id] = @dscid
set @dceid = (SELECT @@IDENTITY)

-- Ethambutol confirmation
INSERT [dbo].[Field] ([Anonymise], [Mandatory], [FieldType_Id]) 
	VALUES (0, 0, 2)
set @fid = (SELECT @@IDENTITY)
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
	VALUES ('Xpert', 1, 0, 0, @fid)
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
	VALUES ('LPA', 0, 0, 0, @fid)
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
	VALUES ('Unknown', 0, 0, 0, @fid)
INSERT [dbo].[DatasetElement] ([ElementName], [Field_Id], [DatasetElementType_Id], [OID], [DefaultValue], [System]) 
	VALUES (N'Ethambutol confirmation', @fid, 1, NULL, NULL, 0)
set @deid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetCategoryElement] ([FieldOrder], [DatasetCategory_Id], [DatasetElement_Id], [Acute], [Chronic]) 
	select ISNULL(MAX(FieldOrder) + 1, 1), @dscid, @deid, 0, 0 from [DatasetCategoryElement] where [DatasetCategory_Id] = @dscid
set @dceid = (SELECT @@IDENTITY)

-- Pyrazinamide susceptibility by any laboratory test(s)
INSERT [dbo].[Field] ([Anonymise], [Mandatory], [FieldType_Id]) 
	VALUES (0, 0, 2)
set @fid = (SELECT @@IDENTITY)
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
	VALUES ('Susceptible', 1, 0, 0, @fid)
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
	VALUES ('Resistant', 0, 0, 0, @fid)
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
	VALUES ('Indeterminate', 0, 0, 0, @fid)
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
	VALUES ('Unknown', 0, 0, 0, @fid)
INSERT [dbo].[DatasetElement] ([ElementName], [Field_Id], [DatasetElementType_Id], [OID], [DefaultValue], [System]) 
	VALUES (N'Pyrazinamide susceptibility by any laboratory test(s)', @fid, 1, NULL, NULL, 0)
set @deid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetCategoryElement] ([FieldOrder], [DatasetCategory_Id], [DatasetElement_Id], [Acute], [Chronic]) 
	select ISNULL(MAX(FieldOrder) + 1, 1), @dscid, @deid, 0, 0 from [DatasetCategoryElement] where [DatasetCategory_Id] = @dscid
set @dceid = (SELECT @@IDENTITY)

-- Pyrazinamide confirmation
INSERT [dbo].[Field] ([Anonymise], [Mandatory], [FieldType_Id]) 
	VALUES (0, 0, 2)
set @fid = (SELECT @@IDENTITY)
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
	VALUES ('Xpert', 1, 0, 0, @fid)
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
	VALUES ('LPA', 0, 0, 0, @fid)
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
	VALUES ('Unknown', 0, 0, 0, @fid)
INSERT [dbo].[DatasetElement] ([ElementName], [Field_Id], [DatasetElementType_Id], [OID], [DefaultValue], [System]) 
	VALUES (N'Pyrazinamide confirmation', @fid, 1, NULL, NULL, 0)
set @deid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetCategoryElement] ([FieldOrder], [DatasetCategory_Id], [DatasetElement_Id], [Acute], [Chronic]) 
	select ISNULL(MAX(FieldOrder) + 1, 1), @dscid, @deid, 0, 0 from [DatasetCategoryElement] where [DatasetCategory_Id] = @dscid
set @dceid = (SELECT @@IDENTITY)

-- Streptomycin susceptibility by any laboratory test(s)
INSERT [dbo].[Field] ([Anonymise], [Mandatory], [FieldType_Id]) 
	VALUES (0, 0, 2)
set @fid = (SELECT @@IDENTITY)
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
	VALUES ('Susceptible', 1, 0, 0, @fid)
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
	VALUES ('Resistant', 0, 0, 0, @fid)
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
	VALUES ('Indeterminate', 0, 0, 0, @fid)
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
	VALUES ('Unknown', 0, 0, 0, @fid)
INSERT [dbo].[DatasetElement] ([ElementName], [Field_Id], [DatasetElementType_Id], [OID], [DefaultValue], [System]) 
	VALUES (N'Streptomycin susceptibility by any laboratory test(s)', @fid, 1, NULL, NULL, 0)
set @deid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetCategoryElement] ([FieldOrder], [DatasetCategory_Id], [DatasetElement_Id], [Acute], [Chronic]) 
	select ISNULL(MAX(FieldOrder) + 1, 1), @dscid, @deid, 0, 0 from [DatasetCategoryElement] where [DatasetCategory_Id] = @dscid
set @dceid = (SELECT @@IDENTITY)

-- Streptomycin confirmation
INSERT [dbo].[Field] ([Anonymise], [Mandatory], [FieldType_Id]) 
	VALUES (0, 0, 2)
set @fid = (SELECT @@IDENTITY)
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
	VALUES ('Xpert', 1, 0, 0, @fid)
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
	VALUES ('LPA', 0, 0, 0, @fid)
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
	VALUES ('Unknown', 0, 0, 0, @fid)
INSERT [dbo].[DatasetElement] ([ElementName], [Field_Id], [DatasetElementType_Id], [OID], [DefaultValue], [System]) 
	VALUES (N'Streptomycin confirmation', @fid, 1, NULL, NULL, 0)
set @deid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetCategoryElement] ([FieldOrder], [DatasetCategory_Id], [DatasetElement_Id], [Acute], [Chronic]) 
	select ISNULL(MAX(FieldOrder) + 1, 1), @dscid, @deid, 0, 0 from [DatasetCategoryElement] where [DatasetCategory_Id] = @dscid
set @dceid = (SELECT @@IDENTITY)

/**************************************************
CATEGORY Second Line Susceptibility
**************************************************/
INSERT [dbo].[DatasetCategory] ([DatasetCategoryName], [CategoryOrder], [Dataset_Id]) 
	VALUES (N'Second Line Susceptibility', 4, @dsid)
set @dscid = (SELECT @@IDENTITY)

-- Amikacin susceptibility by any laboratory test(s)
INSERT [dbo].[Field] ([Anonymise], [Mandatory], [FieldType_Id]) 
	VALUES (0, 0, 2)
set @fid = (SELECT @@IDENTITY)
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
	VALUES ('Susceptible', 1, 0, 0, @fid)
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
	VALUES ('Resistant', 0, 0, 0, @fid)
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
	VALUES ('Indeterminate', 0, 0, 0, @fid)
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
	VALUES ('Unknown', 0, 0, 0, @fid)
INSERT [dbo].[DatasetElement] ([ElementName], [Field_Id], [DatasetElementType_Id], [OID], [DefaultValue], [System]) 
	VALUES (N'Amikacin susceptibility by any laboratory test(s)', @fid, 1, NULL, NULL, 0)
set @deid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetCategoryElement] ([FieldOrder], [DatasetCategory_Id], [DatasetElement_Id], [Acute], [Chronic]) 
	select ISNULL(MAX(FieldOrder) + 1, 1), @dscid, @deid, 0, 0 from [DatasetCategoryElement] where [DatasetCategory_Id] = @dscid
set @dceid = (SELECT @@IDENTITY)

-- Amikacin confirmation
INSERT [dbo].[Field] ([Anonymise], [Mandatory], [FieldType_Id]) 
	VALUES (0, 0, 2)
set @fid = (SELECT @@IDENTITY)
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
	VALUES ('Xpert', 1, 0, 0, @fid)
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
	VALUES ('LPA', 0, 0, 0, @fid)
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
	VALUES ('Unknown', 0, 0, 0, @fid)
INSERT [dbo].[DatasetElement] ([ElementName], [Field_Id], [DatasetElementType_Id], [OID], [DefaultValue], [System]) 
	VALUES (N'Amikacin confirmation', @fid, 1, NULL, NULL, 0)
set @deid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetCategoryElement] ([FieldOrder], [DatasetCategory_Id], [DatasetElement_Id], [Acute], [Chronic]) 
	select ISNULL(MAX(FieldOrder) + 1, 1), @dscid, @deid, 0, 0 from [DatasetCategoryElement] where [DatasetCategory_Id] = @dscid
set @dceid = (SELECT @@IDENTITY)

-- Capreomycin susceptibility by any laboratory test(s)
INSERT [dbo].[Field] ([Anonymise], [Mandatory], [FieldType_Id]) 
	VALUES (0, 0, 2)
set @fid = (SELECT @@IDENTITY)
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
	VALUES ('Susceptible', 1, 0, 0, @fid)
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
	VALUES ('Resistant', 0, 0, 0, @fid)
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
	VALUES ('Indeterminate', 0, 0, 0, @fid)
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
	VALUES ('Unknown', 0, 0, 0, @fid)
INSERT [dbo].[DatasetElement] ([ElementName], [Field_Id], [DatasetElementType_Id], [OID], [DefaultValue], [System]) 
	VALUES (N'Capreomycin susceptibility by any laboratory test(s)', @fid, 1, NULL, NULL, 0)
set @deid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetCategoryElement] ([FieldOrder], [DatasetCategory_Id], [DatasetElement_Id], [Acute], [Chronic]) 
	select ISNULL(MAX(FieldOrder) + 1, 1), @dscid, @deid, 0, 0 from [DatasetCategoryElement] where [DatasetCategory_Id] = @dscid
set @dceid = (SELECT @@IDENTITY)

-- Capreomycin confirmation
INSERT [dbo].[Field] ([Anonymise], [Mandatory], [FieldType_Id]) 
	VALUES (0, 0, 2)
set @fid = (SELECT @@IDENTITY)
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
	VALUES ('Xpert', 1, 0, 0, @fid)
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
	VALUES ('LPA', 0, 0, 0, @fid)
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
	VALUES ('Unknown', 0, 0, 0, @fid)
INSERT [dbo].[DatasetElement] ([ElementName], [Field_Id], [DatasetElementType_Id], [OID], [DefaultValue], [System]) 
	VALUES (N'Capreomycin confirmation', @fid, 1, NULL, NULL, 0)
set @deid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetCategoryElement] ([FieldOrder], [DatasetCategory_Id], [DatasetElement_Id], [Acute], [Chronic]) 
	select ISNULL(MAX(FieldOrder) + 1, 1), @dscid, @deid, 0, 0 from [DatasetCategoryElement] where [DatasetCategory_Id] = @dscid
set @dceid = (SELECT @@IDENTITY)

-- Ciprofloxacin susceptibility by any laboratory test(s)
INSERT [dbo].[Field] ([Anonymise], [Mandatory], [FieldType_Id]) 
	VALUES (0, 0, 2)
set @fid = (SELECT @@IDENTITY)
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
	VALUES ('Susceptible', 1, 0, 0, @fid)
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
	VALUES ('Resistant', 0, 0, 0, @fid)
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
	VALUES ('Indeterminate', 0, 0, 0, @fid)
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
	VALUES ('Unknown', 0, 0, 0, @fid)
INSERT [dbo].[DatasetElement] ([ElementName], [Field_Id], [DatasetElementType_Id], [OID], [DefaultValue], [System]) 
	VALUES (N'Ciprofloxacin susceptibility by any laboratory test(s)', @fid, 1, NULL, NULL, 0)
set @deid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetCategoryElement] ([FieldOrder], [DatasetCategory_Id], [DatasetElement_Id], [Acute], [Chronic]) 
	select ISNULL(MAX(FieldOrder) + 1, 1), @dscid, @deid, 0, 0 from [DatasetCategoryElement] where [DatasetCategory_Id] = @dscid
set @dceid = (SELECT @@IDENTITY)

-- Ciprofloxacin confirmation
INSERT [dbo].[Field] ([Anonymise], [Mandatory], [FieldType_Id]) 
	VALUES (0, 0, 2)
set @fid = (SELECT @@IDENTITY)
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
	VALUES ('Xpert', 1, 0, 0, @fid)
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
	VALUES ('LPA', 0, 0, 0, @fid)
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
	VALUES ('Unknown', 0, 0, 0, @fid)
INSERT [dbo].[DatasetElement] ([ElementName], [Field_Id], [DatasetElementType_Id], [OID], [DefaultValue], [System]) 
	VALUES (N'Ciprofloxacin confirmation', @fid, 1, NULL, NULL, 0)
set @deid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetCategoryElement] ([FieldOrder], [DatasetCategory_Id], [DatasetElement_Id], [Acute], [Chronic]) 
	select ISNULL(MAX(FieldOrder) + 1, 1), @dscid, @deid, 0, 0 from [DatasetCategoryElement] where [DatasetCategory_Id] = @dscid
set @dceid = (SELECT @@IDENTITY)

-- Kanamycin susceptibility by any laboratory test(s)
INSERT [dbo].[Field] ([Anonymise], [Mandatory], [FieldType_Id]) 
	VALUES (0, 0, 2)
set @fid = (SELECT @@IDENTITY)
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
	VALUES ('Susceptible', 1, 0, 0, @fid)
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
	VALUES ('Resistant', 0, 0, 0, @fid)
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
	VALUES ('Indeterminate', 0, 0, 0, @fid)
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
	VALUES ('Unknown', 0, 0, 0, @fid)
INSERT [dbo].[DatasetElement] ([ElementName], [Field_Id], [DatasetElementType_Id], [OID], [DefaultValue], [System]) 
	VALUES (N'Kanamycin susceptibility by any laboratory test(s)', @fid, 1, NULL, NULL, 0)
set @deid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetCategoryElement] ([FieldOrder], [DatasetCategory_Id], [DatasetElement_Id], [Acute], [Chronic]) 
	select ISNULL(MAX(FieldOrder) + 1, 1), @dscid, @deid, 0, 0 from [DatasetCategoryElement] where [DatasetCategory_Id] = @dscid
set @dceid = (SELECT @@IDENTITY)

-- Kanamycin confirmation
INSERT [dbo].[Field] ([Anonymise], [Mandatory], [FieldType_Id]) 
	VALUES (0, 0, 2)
set @fid = (SELECT @@IDENTITY)
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
	VALUES ('Xpert', 1, 0, 0, @fid)
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
	VALUES ('LPA', 0, 0, 0, @fid)
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
	VALUES ('Unknown', 0, 0, 0, @fid)
INSERT [dbo].[DatasetElement] ([ElementName], [Field_Id], [DatasetElementType_Id], [OID], [DefaultValue], [System]) 
	VALUES (N'Kanamycin confirmation', @fid, 1, NULL, NULL, 0)
set @deid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetCategoryElement] ([FieldOrder], [DatasetCategory_Id], [DatasetElement_Id], [Acute], [Chronic]) 
	select ISNULL(MAX(FieldOrder) + 1, 1), @dscid, @deid, 0, 0 from [DatasetCategoryElement] where [DatasetCategory_Id] = @dscid
set @dceid = (SELECT @@IDENTITY)

-- Levofloxacin susceptibility by any laboratory test(s)
INSERT [dbo].[Field] ([Anonymise], [Mandatory], [FieldType_Id]) 
	VALUES (0, 0, 2)
set @fid = (SELECT @@IDENTITY)
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
	VALUES ('Susceptible', 1, 0, 0, @fid)
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
	VALUES ('Resistant', 0, 0, 0, @fid)
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
	VALUES ('Indeterminate', 0, 0, 0, @fid)
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
	VALUES ('Unknown', 0, 0, 0, @fid)
INSERT [dbo].[DatasetElement] ([ElementName], [Field_Id], [DatasetElementType_Id], [OID], [DefaultValue], [System]) 
	VALUES (N'Levofloxacin susceptibility by any laboratory test(s)', @fid, 1, NULL, NULL, 0)
set @deid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetCategoryElement] ([FieldOrder], [DatasetCategory_Id], [DatasetElement_Id], [Acute], [Chronic]) 
	select ISNULL(MAX(FieldOrder) + 1, 1), @dscid, @deid, 0, 0 from [DatasetCategoryElement] where [DatasetCategory_Id] = @dscid
set @dceid = (SELECT @@IDENTITY)

-- Levofloxacin confirmation
INSERT [dbo].[Field] ([Anonymise], [Mandatory], [FieldType_Id]) 
	VALUES (0, 0, 2)
set @fid = (SELECT @@IDENTITY)
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
	VALUES ('Xpert', 1, 0, 0, @fid)
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
	VALUES ('LPA', 0, 0, 0, @fid)
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
	VALUES ('Unknown', 0, 0, 0, @fid)
INSERT [dbo].[DatasetElement] ([ElementName], [Field_Id], [DatasetElementType_Id], [OID], [DefaultValue], [System]) 
	VALUES (N'Levofloxacin confirmation', @fid, 1, NULL, NULL, 0)
set @deid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetCategoryElement] ([FieldOrder], [DatasetCategory_Id], [DatasetElement_Id], [Acute], [Chronic]) 
	select ISNULL(MAX(FieldOrder) + 1, 1), @dscid, @deid, 0, 0 from [DatasetCategoryElement] where [DatasetCategory_Id] = @dscid
set @dceid = (SELECT @@IDENTITY)

-- Moxifloxacin susceptibility by any laboratory test(s)
INSERT [dbo].[Field] ([Anonymise], [Mandatory], [FieldType_Id]) 
	VALUES (0, 0, 2)
set @fid = (SELECT @@IDENTITY)
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
	VALUES ('Susceptible', 1, 0, 0, @fid)
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
	VALUES ('Resistant', 0, 0, 0, @fid)
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
	VALUES ('Indeterminate', 0, 0, 0, @fid)
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
	VALUES ('Unknown', 0, 0, 0, @fid)
INSERT [dbo].[DatasetElement] ([ElementName], [Field_Id], [DatasetElementType_Id], [OID], [DefaultValue], [System]) 
	VALUES (N'Moxifloxacin susceptibility by any laboratory test(s)', @fid, 1, NULL, NULL, 0)
set @deid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetCategoryElement] ([FieldOrder], [DatasetCategory_Id], [DatasetElement_Id], [Acute], [Chronic]) 
	select ISNULL(MAX(FieldOrder) + 1, 1), @dscid, @deid, 0, 0 from [DatasetCategoryElement] where [DatasetCategory_Id] = @dscid
set @dceid = (SELECT @@IDENTITY)

-- Moxifloxacin confirmation
INSERT [dbo].[Field] ([Anonymise], [Mandatory], [FieldType_Id]) 
	VALUES (0, 0, 2)
set @fid = (SELECT @@IDENTITY)
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
	VALUES ('Xpert', 1, 0, 0, @fid)
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
	VALUES ('LPA', 0, 0, 0, @fid)
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
	VALUES ('Unknown', 0, 0, 0, @fid)
INSERT [dbo].[DatasetElement] ([ElementName], [Field_Id], [DatasetElementType_Id], [OID], [DefaultValue], [System]) 
	VALUES (N'Moxifloxacin confirmation', @fid, 1, NULL, NULL, 0)
set @deid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetCategoryElement] ([FieldOrder], [DatasetCategory_Id], [DatasetElement_Id], [Acute], [Chronic]) 
	select ISNULL(MAX(FieldOrder) + 1, 1), @dscid, @deid, 0, 0 from [DatasetCategoryElement] where [DatasetCategory_Id] = @dscid
set @dceid = (SELECT @@IDENTITY)

-- Ofloxacin susceptibility by any laboratory test(s)
INSERT [dbo].[Field] ([Anonymise], [Mandatory], [FieldType_Id]) 
	VALUES (0, 0, 2)
set @fid = (SELECT @@IDENTITY)
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
	VALUES ('Susceptible', 1, 0, 0, @fid)
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
	VALUES ('Resistant', 0, 0, 0, @fid)
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
	VALUES ('Indeterminate', 0, 0, 0, @fid)
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
	VALUES ('Unknown', 0, 0, 0, @fid)
INSERT [dbo].[DatasetElement] ([ElementName], [Field_Id], [DatasetElementType_Id], [OID], [DefaultValue], [System]) 
	VALUES (N'Ofloxacin susceptibility by any laboratory test(s)', @fid, 1, NULL, NULL, 0)
set @deid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetCategoryElement] ([FieldOrder], [DatasetCategory_Id], [DatasetElement_Id], [Acute], [Chronic]) 
	select ISNULL(MAX(FieldOrder) + 1, 1), @dscid, @deid, 0, 0 from [DatasetCategoryElement] where [DatasetCategory_Id] = @dscid
set @dceid = (SELECT @@IDENTITY)

-- Ofloxacin confirmation
INSERT [dbo].[Field] ([Anonymise], [Mandatory], [FieldType_Id]) 
	VALUES (0, 0, 2)
set @fid = (SELECT @@IDENTITY)
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
	VALUES ('Xpert', 1, 0, 0, @fid)
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
	VALUES ('LPA', 0, 0, 0, @fid)
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
	VALUES ('Unknown', 0, 0, 0, @fid)
INSERT [dbo].[DatasetElement] ([ElementName], [Field_Id], [DatasetElementType_Id], [OID], [DefaultValue], [System]) 
	VALUES (N'Ofloxacin confirmation', @fid, 1, NULL, NULL, 0)
set @deid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetCategoryElement] ([FieldOrder], [DatasetCategory_Id], [DatasetElement_Id], [Acute], [Chronic]) 
	select ISNULL(MAX(FieldOrder) + 1, 1), @dscid, @deid, 0, 0 from [DatasetCategoryElement] where [DatasetCategory_Id] = @dscid
set @dceid = (SELECT @@IDENTITY)

select * from fieldtype
select * from dataset
select * from datasetcategory where Dataset_Id = @dsid

SELECT ds.DatasetName, dc.DatasetCategoryName, de.ElementName FROM Dataset ds
	INNER JOIN DatasetCategory dc ON ds.Id = dc.Dataset_Id 
	INNER JOIN DatasetCategoryElement dce ON dc.Id = dce.DatasetCategory_Id 
	INNER JOIN DatasetElement de ON dce.DatasetElement_Id = de.Id 
where ds.Id = @dsid
	
--ROLLBACK TRAN A1
COMMIT TRAN A1


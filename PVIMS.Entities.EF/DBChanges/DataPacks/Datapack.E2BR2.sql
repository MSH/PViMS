DECLARE @Id int
SELECT @Id = Id   FROM Dataset  WHERE DatasetName = 'E2B(R2) ICH Report'

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
DECLARE @elementname varchar(100)

DECLARE @dsid int
DECLARE @dmid int
DECLARE @dmsid int
DECLARE @dsxid int
DECLARE @dsxnid int
DECLARE @dsxcnid int
DECLARE @dsxcnid2 int
DECLARE @dsxcnid3 int
DECLARE @dsxcnid4 int
DECLARE @dsxenid int
INSERT [dbo].[Dataset] ([DatasetName], [Active], [InitialiseProcess], [RulesProcess], [Help], [Created], [LastUpdated], [ContextType_Id], [CreatedBy_Id], [UpdatedBy_Id], [EncounterTypeWorkPlan_Id]) 
	VALUES (N'E2B(R2) ICH Report', 1, NULL, NULL, N'ICH ICSR E2B v2 Dataset', GETDATE(), GETDATE(), 5, 1, 1, NULL)
set @dsid = (SELECT @@IDENTITY)

-- base xml
INSERT [dbo].[DatasetXml] ([Description], Created, LastUpdated, CreatedBy_Id, UpdatedBy_Id)
	VALUES ('E2B(R2) XML', GETDATE(), GETDATE(), 1, 1)
set @dsxid = (SELECT @@IDENTITY)
UPDATE Dataset SET DatasetXml_Id = @dsxid where Id = @dsid

-- base node
INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElement_Id, UpdatedBy_Id, DatasetXml_Id)
	VALUES ('ichicsr', 1, NULL, GETDATE(), GETDATE(), NULL, 1, NULL, 1, @dsxid)
set @dsxnid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetXmlAttribute] (AttributeName, AttributeValue, Created, LastUpdated, CreatedBy_Id, DatasetElement_Id, ParentNode_Id, UpdatedBy_Id)
	VALUES ('lang', 'en', GETDATE(), GETDATE(), 1, NULL, @dsxnid, 1)

/**************************************************
CATEGORY ichicsrmessageheader
**************************************************/
DECLARE @dscid int
INSERT [dbo].[DatasetCategory] ([DatasetCategoryName], [CategoryOrder], [Dataset_Id]) 
	VALUES (N'Message Header', 1, @dsid)
set @dscid = (SELECT @@IDENTITY)

-- category node
INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElement_Id, UpdatedBy_Id, DatasetXml_Id)
	VALUES ('ichicsrmessageheader', 2, NULL, GETDATE(), GETDATE(), @dsxnid, 1, NULL, 1, @dsxid)
set @dsxcnid = (SELECT @@IDENTITY)

-- messagetype
DECLARE @fid int
DECLARE @deid int
DECLARE @desid int
DECLARE @dceid int
DECLARE @sdceid int
INSERT [dbo].[Field] ([Anonymise], [Mandatory], [MaxLength], [RegEx], [FieldType_Id]) 
	VALUES (0, 0, 50, NULL, 8)
set @fid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetElement] ([ElementName], [Field_Id], [DatasetElementType_Id], [OID], [DefaultValue], [System], [DatasetElementGuid]) 
	VALUES (N'Message Type', @fid, 1, NULL, 'ICHICSR', 1, '866CE390-4850-43E0-9C85-A4A66F70904A')
set @deid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetCategoryElement] ([FieldOrder], [DatasetCategory_Id], [DatasetElement_Id], [Acute], [Chronic]) 
	select ISNULL(MAX(FieldOrder) + 1, 1), @dscid, @deid, 0, 0 from [DatasetCategoryElement] where [DatasetCategory_Id] = @dscid
-- element node
INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElement_Id, UpdatedBy_Id, DatasetXml_Id)
	VALUES ('messagetype', 2, NULL, GETDATE(), GETDATE(), @dsxcnid, 1, @deid, 1, @dsxid)
set @dsxenid = (SELECT @@IDENTITY)

-- messageformatversion
INSERT [dbo].[Field] ([Anonymise], [Mandatory], [MaxLength], [RegEx], [FieldType_Id]) 
	VALUES (0, 0, 10, NULL, 8)
set @fid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetElement] ([ElementName], [Field_Id], [DatasetElementType_Id], [OID], [DefaultValue], [System], [DatasetElementGuid]) 
	VALUES (N'Message Format Version', @fid, 1, NULL, '2.1', 1, NEWID())
set @deid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetCategoryElement] ([FieldOrder], [DatasetCategory_Id], [DatasetElement_Id], [Acute], [Chronic]) 
	select ISNULL(MAX(FieldOrder) + 1, 1), @dscid, @deid, 0, 0 from [DatasetCategoryElement] where [DatasetCategory_Id] = @dscid
-- element node
INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElement_Id, UpdatedBy_Id, DatasetXml_Id)
	VALUES ('messageformatversion', 2, NULL, GETDATE(), GETDATE(), @dsxcnid, 1, @deid, 1, @dsxid)
set @dsxenid = (SELECT @@IDENTITY)

-- messageformatrelease
INSERT [dbo].[Field] ([Anonymise], [Mandatory], [MaxLength], [RegEx], [FieldType_Id]) 
	VALUES (0, 0, 10, NULL, 8)
set @fid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetElement] ([ElementName], [Field_Id], [DatasetElementType_Id], [OID], [DefaultValue], [System], [DatasetElementGuid]) 
	VALUES (N'Message Format Release', @fid, 1, NULL, '1.0', 1, NEWID())
set @deid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetCategoryElement] ([FieldOrder], [DatasetCategory_Id], [DatasetElement_Id], [Acute], [Chronic]) 
	select ISNULL(MAX(FieldOrder) + 1, 1), @dscid, @deid, 0, 0 from [DatasetCategoryElement] where [DatasetCategory_Id] = @dscid
-- element node
INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElement_Id, UpdatedBy_Id, DatasetXml_Id)
	VALUES ('messageformatrelease', 2, NULL, GETDATE(), GETDATE(), @dsxcnid, 1, @deid, 1, @dsxid)
set @dsxenid = (SELECT @@IDENTITY)

-- messagenumb
INSERT [dbo].[Field] ([Anonymise], [Mandatory], [MaxLength], [RegEx], [FieldType_Id]) 
	VALUES (0, 0, 10, NULL, 8)
set @fid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetElement] ([ElementName], [Field_Id], [DatasetElementType_Id], [OID], [DefaultValue], [System], [DatasetElementGuid]) 
	VALUES (N'Message Number', @fid, 1, NULL, NULL, 1, NEWID())
set @deid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetCategoryElement] ([FieldOrder], [DatasetCategory_Id], [DatasetElement_Id], [Acute], [Chronic]) 
	select ISNULL(MAX(FieldOrder) + 1, 1), @dscid, @deid, 0, 0 from [DatasetCategoryElement] where [DatasetCategory_Id] = @dscid
-- element node
INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElement_Id, UpdatedBy_Id, DatasetXml_Id)
	VALUES ('messagenumb', 2, NULL, GETDATE(), GETDATE(), @dsxcnid, 1, @deid, 1, @dsxid)
set @dsxenid = (SELECT @@IDENTITY)

-- messagesenderidentifier
INSERT [dbo].[Field] ([Anonymise], [Mandatory], [MaxLength], [RegEx], [FieldType_Id]) 
	VALUES (0, 0, 50, NULL, 8)
set @fid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetElement] ([ElementName], [Field_Id], [DatasetElementType_Id], [OID], [DefaultValue], [System], [DatasetElementGuid]) 
	VALUES (N'Message Sender Identifier', @fid, 1, NULL, 'FDA', 1, NEWID())
set @deid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetCategoryElement] ([FieldOrder], [DatasetCategory_Id], [DatasetElement_Id], [Acute], [Chronic]) 
	select ISNULL(MAX(FieldOrder) + 1, 1), @dscid, @deid, 0, 0 from [DatasetCategoryElement] where [DatasetCategory_Id] = @dscid
-- element node
INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElement_Id, UpdatedBy_Id, DatasetXml_Id)
	VALUES ('messagesenderidentifier', 2, NULL, GETDATE(), GETDATE(), @dsxcnid, 1, @deid, 1, @dsxid)
set @dsxenid = (SELECT @@IDENTITY)

-- messagereceiveridentifier
INSERT [dbo].[Field] ([Anonymise], [Mandatory], [MaxLength], [RegEx], [FieldType_Id]) 
	VALUES (0, 0, 50, NULL, 8)
set @fid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetElement] ([ElementName], [Field_Id], [DatasetElementType_Id], [OID], [DefaultValue], [System], [DatasetElementGuid]) 
	VALUES (N'Message Receiver Identifier', @fid, 1, NULL, 'UMC', 1, NEWID())
set @deid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetCategoryElement] ([FieldOrder], [DatasetCategory_Id], [DatasetElement_Id], [Acute], [Chronic]) 
	select ISNULL(MAX(FieldOrder) + 1, 1), @dscid, @deid, 0, 0 from [DatasetCategoryElement] where [DatasetCategory_Id] = @dscid
-- element node
INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElement_Id, UpdatedBy_Id, DatasetXml_Id)
	VALUES ('messagereceiveridentifier', 2, NULL, GETDATE(), GETDATE(), @dsxcnid, 1, @deid, 1, @dsxid)
set @dsxenid = (SELECT @@IDENTITY)

-- messagedateformat
INSERT [dbo].[Field] ([Anonymise], [Mandatory], [MaxLength], [RegEx], [FieldType_Id]) 
	VALUES (0, 0, 10, NULL, 8)
set @fid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetElement] ([ElementName], [Field_Id], [DatasetElementType_Id], [OID], [DefaultValue], [System], [DatasetElementGuid]) 
	VALUES (N'Message Date Format', @fid, 1, NULL, '204', 1, NEWID())
set @deid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetCategoryElement] ([FieldOrder], [DatasetCategory_Id], [DatasetElement_Id], [Acute], [Chronic]) 
	select ISNULL(MAX(FieldOrder) + 1, 1), @dscid, @deid, 0, 0 from [DatasetCategoryElement] where [DatasetCategory_Id] = @dscid
-- element node
INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElement_Id, UpdatedBy_Id, DatasetXml_Id)
	VALUES ('messagedateformat', 2, NULL, GETDATE(), GETDATE(), @dsxcnid, 1, @deid, 1, @dsxid)
set @dsxenid = (SELECT @@IDENTITY)

-- messagedate
INSERT [dbo].[Field] ([Anonymise], [Mandatory], [MaxLength], [RegEx], [FieldType_Id]) 
	VALUES (0, 0, 10, NULL, 8)
set @fid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetElement] ([ElementName], [Field_Id], [DatasetElementType_Id], [OID], [DefaultValue], [System], [DatasetElementGuid]) 
	VALUES (N'Message Date', @fid, 1, NULL, NULL, 1, NEWID())
set @deid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetCategoryElement] ([FieldOrder], [DatasetCategory_Id], [DatasetElement_Id], [Acute], [Chronic]) 
	select ISNULL(MAX(FieldOrder) + 1, 1), @dscid, @deid, 0, 0 from [DatasetCategoryElement] where [DatasetCategory_Id] = @dscid
-- element node
INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElement_Id, UpdatedBy_Id, DatasetXml_Id)
	VALUES ('messagedate', 2, NULL, GETDATE(), GETDATE(), @dsxcnid, 1, @deid, 1, @dsxid)
set @dsxenid = (SELECT @@IDENTITY)


/**************************************************
CATEGORY safetyreport
**************************************************/
INSERT [dbo].[DatasetCategory] ([DatasetCategoryName], [CategoryOrder], [Dataset_Id]) 
	VALUES (N'Safety Report', 1, @dsid)
set @dscid = (SELECT @@IDENTITY)

-- category node
INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElement_Id, UpdatedBy_Id, DatasetXml_Id)
	VALUES ('safetyreport', 2, NULL, GETDATE(), GETDATE(), @dsxnid, 1, NULL, 1, @dsxid)
set @dsxcnid = (SELECT @@IDENTITY)

-- safetyreportversion
INSERT [dbo].[Field] ([Anonymise], [Mandatory], [MaxLength], [RegEx], [FieldType_Id]) 
	VALUES (0, 0, 10, NULL, 8)
set @fid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetElement] ([ElementName], [Field_Id], [DatasetElementType_Id], [OID], [DefaultValue], [System], [DatasetElementGuid]) 
	VALUES (N'Safety Report Version', @fid, 1, NULL, '1', 1, NEWID())
set @deid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetCategoryElement] ([FieldOrder], [DatasetCategory_Id], [DatasetElement_Id], [Acute], [Chronic]) 
	select ISNULL(MAX(FieldOrder) + 1, 1), @dscid, @deid, 0, 0 from [DatasetCategoryElement] where [DatasetCategory_Id] = @dscid
-- element node
INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElement_Id, UpdatedBy_Id, DatasetXml_Id)
	VALUES ('safetyreportversion', 2, NULL, GETDATE(), GETDATE(), @dsxcnid, 1, @deid, 1, @dsxid)
set @dsxenid = (SELECT @@IDENTITY)

-- safetyreportid
INSERT [dbo].[Field] ([Anonymise], [Mandatory], [MaxLength], [RegEx], [FieldType_Id]) 
	VALUES (0, 0, 20, NULL, 8)
set @fid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetElement] ([ElementName], [Field_Id], [DatasetElementType_Id], [OID], [DefaultValue], [System], [DatasetElementGuid]) 
	VALUES (N'Safety Report ID', @fid, 1, NULL, NULL, 0, NEWID())
set @deid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetCategoryElement] ([FieldOrder], [DatasetCategory_Id], [DatasetElement_Id], [Acute], [Chronic]) 
	select ISNULL(MAX(FieldOrder) + 1, 1), @dscid, @deid, 0, 0 from [DatasetCategoryElement] where [DatasetCategory_Id] = @dscid
-- element node
INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElement_Id, UpdatedBy_Id, DatasetXml_Id)
	VALUES ('safetyreportid', 2, NULL, GETDATE(), GETDATE(), @dsxcnid, 1, @deid, 1, @dsxid)
set @dsxenid = (SELECT @@IDENTITY)

-- primarysourcecountry
INSERT [dbo].[Field] ([Anonymise], [Mandatory], [MaxLength], [RegEx], [FieldType_Id]) 
	VALUES (0, 0, 10, NULL, 8)
set @fid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetElement] ([ElementName], [Field_Id], [DatasetElementType_Id], [OID], [DefaultValue], [System], [DatasetElementGuid]) 
	VALUES (N'Primary Source Country', @fid, 1, NULL, 'PH', 0, NEWID())
set @deid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetCategoryElement] ([FieldOrder], [DatasetCategory_Id], [DatasetElement_Id], [Acute], [Chronic]) 
	select ISNULL(MAX(FieldOrder) + 1, 1), @dscid, @deid, 0, 0 from [DatasetCategoryElement] where [DatasetCategory_Id] = @dscid
-- element node
INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElement_Id, UpdatedBy_Id, DatasetXml_Id)
	VALUES ('primarysourcecountry', 2, NULL, GETDATE(), GETDATE(), @dsxcnid, 1, @deid, 1, @dsxid)
set @dsxenid = (SELECT @@IDENTITY)

-- occurcountry
INSERT [dbo].[Field] ([Anonymise], [Mandatory], [MaxLength], [RegEx], [FieldType_Id]) 
	VALUES (0, 0, 10, NULL, 8)
set @fid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetElement] ([ElementName], [Field_Id], [DatasetElementType_Id], [OID], [DefaultValue], [System], [DatasetElementGuid]) 
	VALUES (N'Occur Country', @fid, 1, NULL, 'PH', 0, NEWID())
set @deid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetCategoryElement] ([FieldOrder], [DatasetCategory_Id], [DatasetElement_Id], [Acute], [Chronic]) 
	select ISNULL(MAX(FieldOrder) + 1, 1), @dscid, @deid, 0, 0 from [DatasetCategoryElement] where [DatasetCategory_Id] = @dscid
-- element node
INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElement_Id, UpdatedBy_Id, DatasetXml_Id)
	VALUES ('occurcountry', 2, NULL, GETDATE(), GETDATE(), @dsxcnid, 1, @deid, 1, @dsxid)
set @dsxenid = (SELECT @@IDENTITY)

-- transmissiondateformat
INSERT [dbo].[Field] ([Anonymise], [Mandatory], [MaxLength], [RegEx], [FieldType_Id]) 
	VALUES (0, 0, 10, NULL, 8)
set @fid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetElement] ([ElementName], [Field_Id], [DatasetElementType_Id], [OID], [DefaultValue], [System], [DatasetElementGuid]) 
	VALUES (N'Transmission Date Format', @fid, 1, NULL, '102', 1, NEWID())
set @deid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetCategoryElement] ([FieldOrder], [DatasetCategory_Id], [DatasetElement_Id], [Acute], [Chronic]) 
	select ISNULL(MAX(FieldOrder) + 1, 1), @dscid, @deid, 0, 0 from [DatasetCategoryElement] where [DatasetCategory_Id] = @dscid
-- element node
INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElement_Id, UpdatedBy_Id, DatasetXml_Id)
	VALUES ('transmissiondateformat', 2, NULL, GETDATE(), GETDATE(), @dsxcnid, 1, @deid, 1, @dsxid)
set @dsxenid = (SELECT @@IDENTITY)

-- transmissiondate
INSERT [dbo].[Field] ([Anonymise], [Mandatory], [MaxLength], [RegEx], [FieldType_Id]) 
	VALUES (0, 0, 10, NULL, 8)
set @fid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetElement] ([ElementName], [Field_Id], [DatasetElementType_Id], [OID], [DefaultValue], [System], [DatasetElementGuid]) 
	VALUES (N'Transmission Date', @fid, 1, NULL, NULL, 1, NEWID())
set @deid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetCategoryElement] ([FieldOrder], [DatasetCategory_Id], [DatasetElement_Id], [Acute], [Chronic]) 
	select ISNULL(MAX(FieldOrder) + 1, 1), @dscid, @deid, 0, 0 from [DatasetCategoryElement] where [DatasetCategory_Id] = @dscid
-- element node
INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElement_Id, UpdatedBy_Id, DatasetXml_Id)
	VALUES ('transmissiondate', 2, NULL, GETDATE(), GETDATE(), @dsxcnid, 1, @deid, 1, @dsxid)
set @dsxenid = (SELECT @@IDENTITY)

-- reporttype
INSERT [dbo].[Field] ([Anonymise], [Mandatory], [MaxLength], [RegEx], [FieldType_Id]) 
	VALUES (0, 0, 1, NULL, 8)
set @fid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetElement] ([ElementName], [Field_Id], [DatasetElementType_Id], [OID], [DefaultValue], [System], [DatasetElementGuid]) 
	VALUES (N'Report Type', @fid, 1, NULL, NULL, 1, NEWID())
set @deid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetCategoryElement] ([FieldOrder], [DatasetCategory_Id], [DatasetElement_Id], [Acute], [Chronic]) 
	select ISNULL(MAX(FieldOrder) + 1, 1), @dscid, @deid, 0, 0 from [DatasetCategoryElement] where [DatasetCategory_Id] = @dscid
-- element node
INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElement_Id, UpdatedBy_Id, DatasetXml_Id)
	VALUES ('reporttype', 2, NULL, GETDATE(), GETDATE(), @dsxcnid, 1, @deid, 1, @dsxid)
set @dsxenid = (SELECT @@IDENTITY)

-- serious
INSERT [dbo].[Field] ([Anonymise], [Mandatory], [FieldType_Id]) 
	VALUES (0, 0, 2)
set @fid = (SELECT @@IDENTITY)
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
	VALUES ('1=Yes', 1, 0, 0, @fid)
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
	VALUES ('2=No', 0, 0, 0, @fid)
INSERT [dbo].[DatasetElement] ([ElementName], [Field_Id], [DatasetElementType_Id], [OID], [DefaultValue], [System], [DatasetElementGuid]) 
	VALUES (N'Serious', @fid, 1, NULL, NULL, 0, NEWID())
set @deid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetCategoryElement] ([FieldOrder], [DatasetCategory_Id], [DatasetElement_Id], [Acute], [Chronic]) 
	select ISNULL(MAX(FieldOrder) + 1, 1), @dscid, @deid, 0, 0 from [DatasetCategoryElement] where [DatasetCategory_Id] = @dscid
set @dceid = (SELECT @@IDENTITY)
-- element node
INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElement_Id, UpdatedBy_Id, DatasetXml_Id)
	VALUES ('serious', 2, NULL, GETDATE(), GETDATE(), @dsxcnid, 1, @deid, 1, @dsxid)
set @dsxenid = (SELECT @@IDENTITY)
-- mapping
INSERT [dbo].[DatasetMapping] (Tag, MappingType, MappingOption, DestinationElement_Id, SourceElement_Id)
	VALUES ('Spontaneous', 1, '', @dceid, (select top 1 dce.Id from [dbo].[DatasetCategoryElement] dce inner join [dbo].[DatasetElement] de on dce.DatasetElement_Id = de.Id where de.ElementName = 'Reaction serious'))
set @dmid = (SELECT @@IDENTITY)
-- mapping values
INSERT [dbo].[DatasetMappingValue] (SourceValue, DestinationValue, Active, Mapping_Id)
	SELECT 'Yes', '1=Yes', 1, @dmid UNION
	SELECT 'No', '2=No', 1, @dmid

-- seriousnessdeath 
INSERT [dbo].[Field] ([Anonymise], [Mandatory], [FieldType_Id]) 
	VALUES (0, 0, 2)
set @fid = (SELECT @@IDENTITY)
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
	VALUES ('1=Yes', 1, 0, 0, @fid)
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
	VALUES ('2=No', 0, 0, 0, @fid)
INSERT [dbo].[DatasetElement] ([ElementName], [Field_Id], [DatasetElementType_Id], [OID], [DefaultValue], [System], [DatasetElementGuid]) 
	VALUES (N'Seriousness Death', @fid, 1, NULL, NULL, 0, NEWID())
set @deid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetCategoryElement] ([FieldOrder], [DatasetCategory_Id], [DatasetElement_Id], [Acute], [Chronic]) 
	select ISNULL(MAX(FieldOrder) + 1, 1), @dscid, @deid, 0, 0 from [DatasetCategoryElement] where [DatasetCategory_Id] = @dscid
-- element node
INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElement_Id, UpdatedBy_Id, DatasetXml_Id)
	VALUES ('seriousnessdeath', 2, NULL, GETDATE(), GETDATE(), @dsxcnid, 1, @deid, 1, @dsxid)
set @dsxenid = (SELECT @@IDENTITY)

-- seriousnesslifethreatening 
INSERT [dbo].[Field] ([Anonymise], [Mandatory], [FieldType_Id]) 
	VALUES (0, 0, 2)
set @fid = (SELECT @@IDENTITY)
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
	VALUES ('1=Yes', 1, 0, 0, @fid)
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
	VALUES ('2=No', 0, 0, 0, @fid)
INSERT [dbo].[DatasetElement] ([ElementName], [Field_Id], [DatasetElementType_Id], [OID], [DefaultValue], [System], [DatasetElementGuid]) 
	VALUES (N'Seriousness Life Threatening', @fid, 1, NULL, NULL, 0, NEWID())
set @deid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetCategoryElement] ([FieldOrder], [DatasetCategory_Id], [DatasetElement_Id], [Acute], [Chronic]) 
	select ISNULL(MAX(FieldOrder) + 1, 1), @dscid, @deid, 0, 0 from [DatasetCategoryElement] where [DatasetCategory_Id] = @dscid
-- element node
INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElement_Id, UpdatedBy_Id, DatasetXml_Id)
	VALUES ('seriousnesslifethreatening', 2, NULL, GETDATE(), GETDATE(), @dsxcnid, 1, @deid, 1, @dsxid)
set @dsxenid = (SELECT @@IDENTITY)

-- seriousnesshospitalization 
INSERT [dbo].[Field] ([Anonymise], [Mandatory], [FieldType_Id]) 
	VALUES (0, 0, 2)
set @fid = (SELECT @@IDENTITY)
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
	VALUES ('1=Yes', 1, 0, 0, @fid)
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
	VALUES ('2=No', 0, 0, 0, @fid)
INSERT [dbo].[DatasetElement] ([ElementName], [Field_Id], [DatasetElementType_Id], [OID], [DefaultValue], [System], [DatasetElementGuid]) 
	VALUES (N'Seriousness Hospitalization', @fid, 1, NULL, NULL, 0, NEWID())
set @deid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetCategoryElement] ([FieldOrder], [DatasetCategory_Id], [DatasetElement_Id], [Acute], [Chronic]) 
	select ISNULL(MAX(FieldOrder) + 1, 1), @dscid, @deid, 0, 0 from [DatasetCategoryElement] where [DatasetCategory_Id] = @dscid
-- element node
INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElement_Id, UpdatedBy_Id, DatasetXml_Id)
	VALUES ('seriousnesshospitalization', 2, NULL, GETDATE(), GETDATE(), @dsxcnid, 1, @deid, 1, @dsxid)
set @dsxenid = (SELECT @@IDENTITY)

-- seriousnessdisabling 
INSERT [dbo].[Field] ([Anonymise], [Mandatory], [FieldType_Id]) 
	VALUES (0, 0, 2)
set @fid = (SELECT @@IDENTITY)
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
	VALUES ('1=Yes', 1, 0, 0, @fid)
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
	VALUES ('2=No', 0, 0, 0, @fid)
INSERT [dbo].[DatasetElement] ([ElementName], [Field_Id], [DatasetElementType_Id], [OID], [DefaultValue], [System], [DatasetElementGuid]) 
	VALUES (N'Seriousness Disabling', @fid, 1, NULL, NULL, 0, NEWID())
set @deid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetCategoryElement] ([FieldOrder], [DatasetCategory_Id], [DatasetElement_Id], [Acute], [Chronic]) 
	select ISNULL(MAX(FieldOrder) + 1, 1), @dscid, @deid, 0, 0 from [DatasetCategoryElement] where [DatasetCategory_Id] = @dscid
-- element node
INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElement_Id, UpdatedBy_Id, DatasetXml_Id)
	VALUES ('seriousnessdisabling', 2, NULL, GETDATE(), GETDATE(), @dsxcnid, 1, @deid, 1, @dsxid)
set @dsxenid = (SELECT @@IDENTITY)

-- seriousnesscongenitalanomali 
INSERT [dbo].[Field] ([Anonymise], [Mandatory], [FieldType_Id]) 
	VALUES (0, 0, 2)
set @fid = (SELECT @@IDENTITY)
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
	VALUES ('1=Yes', 1, 0, 0, @fid)
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
	VALUES ('2=No', 0, 0, 0, @fid)
INSERT [dbo].[DatasetElement] ([ElementName], [Field_Id], [DatasetElementType_Id], [OID], [DefaultValue], [System], [DatasetElementGuid]) 
	VALUES (N'Seriousness Congenital Anomaly', @fid, 1, NULL, NULL, 0, NEWID())
set @deid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetCategoryElement] ([FieldOrder], [DatasetCategory_Id], [DatasetElement_Id], [Acute], [Chronic]) 
	select ISNULL(MAX(FieldOrder) + 1, 1), @dscid, @deid, 0, 0 from [DatasetCategoryElement] where [DatasetCategory_Id] = @dscid
-- element node
INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElement_Id, UpdatedBy_Id, DatasetXml_Id)
	VALUES ('seriousnesscongenitalanomali', 2, NULL, GETDATE(), GETDATE(), @dsxcnid, 1, @deid, 1, @dsxid)
set @dsxenid = (SELECT @@IDENTITY)

-- seriousnessother 
INSERT [dbo].[Field] ([Anonymise], [Mandatory], [FieldType_Id]) 
	VALUES (0, 0, 2)
set @fid = (SELECT @@IDENTITY)
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
	VALUES ('1=Yes', 1, 0, 0, @fid)
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
	VALUES ('2=No', 0, 0, 0, @fid)
INSERT [dbo].[DatasetElement] ([ElementName], [Field_Id], [DatasetElementType_Id], [OID], [DefaultValue], [System], [DatasetElementGuid]) 
	VALUES (N'Seriousness Other', @fid, 1, NULL, NULL, 0, NEWID())
set @deid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetCategoryElement] ([FieldOrder], [DatasetCategory_Id], [DatasetElement_Id], [Acute], [Chronic]) 
	select ISNULL(MAX(FieldOrder) + 1, 1), @dscid, @deid, 0, 0 from [DatasetCategoryElement] where [DatasetCategory_Id] = @dscid
-- element node
INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElement_Id, UpdatedBy_Id, DatasetXml_Id)
	VALUES ('seriousnessother', 2, NULL, GETDATE(), GETDATE(), @dsxcnid, 1, @deid, 1, @dsxid)
set @dsxenid = (SELECT @@IDENTITY)

-- receivedateformat
INSERT [dbo].[Field] ([Anonymise], [Mandatory], [MaxLength], [RegEx], [FieldType_Id]) 
	VALUES (0, 0, 10, NULL, 8)
set @fid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetElement] ([ElementName], [Field_Id], [DatasetElementType_Id], [OID], [DefaultValue], [System], [DatasetElementGuid]) 
	VALUES (N'Receive Date Format', @fid, 1, NULL, '102', 1, NEWID())
set @deid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetCategoryElement] ([FieldOrder], [DatasetCategory_Id], [DatasetElement_Id], [Acute], [Chronic]) 
	select ISNULL(MAX(FieldOrder) + 1, 1), @dscid, @deid, 0, 0 from [DatasetCategoryElement] where [DatasetCategory_Id] = @dscid
-- element node
INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElement_Id, UpdatedBy_Id, DatasetXml_Id)
	VALUES ('receivedateformat', 2, NULL, GETDATE(), GETDATE(), @dsxcnid, 1, @deid, 1, @dsxid)
set @dsxenid = (SELECT @@IDENTITY)

-- receivedate
INSERT [dbo].[Field] ([Anonymise], [Mandatory], [FieldType_Id]) 
	VALUES (0, 0, 6)
set @fid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetElement] ([ElementName], [Field_Id], [DatasetElementType_Id], [OID], [DefaultValue], [System], [DatasetElementGuid]) 
	VALUES (N'Date report was first received', @fid, 1, NULL, NULL, 0, NEWID())
set @deid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetCategoryElement] ([FieldOrder], [DatasetCategory_Id], [DatasetElement_Id], [Acute], [Chronic]) 
	select ISNULL(MAX(FieldOrder) + 1, 1), @dscid, @deid, 0, 0 from [DatasetCategoryElement] where [DatasetCategory_Id] = @dscid
-- element node
INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElement_Id, UpdatedBy_Id, DatasetXml_Id)
	VALUES ('receivedate', 2, NULL, GETDATE(), GETDATE(), @dsxcnid, 1, @deid, 1, @dsxid)
set @dsxenid = (SELECT @@IDENTITY)

-- receiptdateformat
INSERT [dbo].[Field] ([Anonymise], [Mandatory], [MaxLength], [RegEx], [FieldType_Id]) 
	VALUES (0, 0, 10, NULL, 8)
set @fid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetElement] ([ElementName], [Field_Id], [DatasetElementType_Id], [OID], [DefaultValue], [System], [DatasetElementGuid]) 
	VALUES (N'Receipt Date Format', @fid, 1, NULL, '102', 1, NEWID())
set @deid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetCategoryElement] ([FieldOrder], [DatasetCategory_Id], [DatasetElement_Id], [Acute], [Chronic]) 
	select ISNULL(MAX(FieldOrder) + 1, 1), @dscid, @deid, 0, 0 from [DatasetCategoryElement] where [DatasetCategory_Id] = @dscid
-- element node
INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElement_Id, UpdatedBy_Id, DatasetXml_Id)
	VALUES ('receiptdateformat', 2, NULL, GETDATE(), GETDATE(), @dsxcnid, 1, @deid, 1, @dsxid)
set @dsxenid = (SELECT @@IDENTITY)

-- receiptdate
INSERT [dbo].[Field] ([Anonymise], [Mandatory], [FieldType_Id]) 
	VALUES (0, 0, 6)
set @fid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetElement] ([ElementName], [Field_Id], [DatasetElementType_Id], [OID], [DefaultValue], [System], [DatasetElementGuid]) 
	VALUES (N'Date of most recent info', @fid, 1, NULL, NULL, 0, NEWID())
set @deid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetCategoryElement] ([FieldOrder], [DatasetCategory_Id], [DatasetElement_Id], [Acute], [Chronic]) 
	select ISNULL(MAX(FieldOrder) + 1, 1), @dscid, @deid, 0, 0 from [DatasetCategoryElement] where [DatasetCategory_Id] = @dscid
-- element node
INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElement_Id, UpdatedBy_Id, DatasetXml_Id)
	VALUES ('receiptdate', 2, NULL, GETDATE(), GETDATE(), @dsxcnid, 1, @deid, 1, @dsxid)
set @dsxenid = (SELECT @@IDENTITY)

-- additionaldocument
INSERT [dbo].[Field] ([Anonymise], [Mandatory], [FieldType_Id]) 
	VALUES (0, 0, 2)
set @fid = (SELECT @@IDENTITY)
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
	VALUES ('1=Yes', 1, 0, 0, @fid)
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
	VALUES ('2=No', 0, 0, 0, @fid)
INSERT [dbo].[DatasetElement] ([ElementName], [Field_Id], [DatasetElementType_Id], [OID], [DefaultValue], [System], [DatasetElementGuid]) 
	VALUES (N'Additional Document', @fid, 1, NULL, NULL, 0, NEWID())
set @deid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetCategoryElement] ([FieldOrder], [DatasetCategory_Id], [DatasetElement_Id], [Acute], [Chronic]) 
	select ISNULL(MAX(FieldOrder) + 1, 1), @dscid, @deid, 0, 0 from [DatasetCategoryElement] where [DatasetCategory_Id] = @dscid
-- element node
INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElement_Id, UpdatedBy_Id, DatasetXml_Id)
	VALUES ('additionaldocument', 2, NULL, GETDATE(), GETDATE(), @dsxcnid, 1, @deid, 1, @dsxid)
set @dsxenid = (SELECT @@IDENTITY)

-- documentlist
INSERT [dbo].[Field] ([Anonymise], [Mandatory], [MaxLength], [RegEx], [FieldType_Id]) 
	VALUES (0, 0, 100, NULL, 3)
set @fid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetElement] ([ElementName], [Field_Id], [DatasetElementType_Id], [OID], [DefaultValue], [System], [DatasetElementGuid]) 
	VALUES (N'Document List', @fid, 1, NULL, '', 0, NEWID())
set @deid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetCategoryElement] ([FieldOrder], [DatasetCategory_Id], [DatasetElement_Id], [Acute], [Chronic]) 
	select ISNULL(MAX(FieldOrder) + 1, 1), @dscid, @deid, 0, 0 from [DatasetCategoryElement] where [DatasetCategory_Id] = @dscid
-- element node
INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElement_Id, UpdatedBy_Id, DatasetXml_Id)
	VALUES ('documentlist', 2, NULL, GETDATE(), GETDATE(), @dsxcnid, 1, @deid, 1, @dsxid)
set @dsxenid = (SELECT @@IDENTITY)

-- fulfillexpeditecriteria
INSERT [dbo].[Field] ([Anonymise], [Mandatory], [FieldType_Id]) 
	VALUES (0, 0, 2)
set @fid = (SELECT @@IDENTITY)
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
	VALUES ('1=Yes', 1, 0, 0, @fid)
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
	VALUES ('2=No', 0, 0, 0, @fid)
INSERT [dbo].[DatasetElement] ([ElementName], [Field_Id], [DatasetElementType_Id], [OID], [DefaultValue], [System], [DatasetElementGuid]) 
	VALUES (N'Fulfill Expedite Criteria', @fid, 1, NULL, '2=No', 0, NEWID())
set @deid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetCategoryElement] ([FieldOrder], [DatasetCategory_Id], [DatasetElement_Id], [Acute], [Chronic]) 
	select ISNULL(MAX(FieldOrder) + 1, 1), @dscid, @deid, 0, 0 from [DatasetCategoryElement] where [DatasetCategory_Id] = @dscid
-- element node
INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElement_Id, UpdatedBy_Id, DatasetXml_Id)
	VALUES ('fulfillexpeditecriteria', 2, NULL, GETDATE(), GETDATE(), @dsxcnid, 1, @deid, 1, @dsxid)
set @dsxenid = (SELECT @@IDENTITY)

-- duplicate
INSERT [dbo].[Field] ([Anonymise], [Mandatory], [MaxLength], [RegEx], [FieldType_Id]) 
	VALUES (0, 0, 10, NULL, 8)
set @fid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetElement] ([ElementName], [Field_Id], [DatasetElementType_Id], [OID], [DefaultValue], [System], [DatasetElementGuid]) 
	VALUES (N'Duplicate', @fid, 1, NULL, '2', 1, NEWID())
set @deid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetCategoryElement] ([FieldOrder], [DatasetCategory_Id], [DatasetElement_Id], [Acute], [Chronic]) 
	select ISNULL(MAX(FieldOrder) + 1, 1), @dscid, @deid, 0, 0 from [DatasetCategoryElement] where [DatasetCategory_Id] = @dscid
-- element node
INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElement_Id, UpdatedBy_Id, DatasetXml_Id)
	VALUES ('duplicate', 2, NULL, GETDATE(), GETDATE(), @dsxcnid, 1, @deid, 1, @dsxid)
set @dsxenid = (SELECT @@IDENTITY)

-- casenullification 
INSERT [dbo].[Field] ([Anonymise], [Mandatory], [FieldType_Id]) 
	VALUES (0, 0, 2)
set @fid = (SELECT @@IDENTITY)
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
	VALUES ('1=Yes', 1, 0, 0, @fid)
INSERT [dbo].[DatasetElement] ([ElementName], [Field_Id], [DatasetElementType_Id], [OID], [DefaultValue], [System], [DatasetElementGuid]) 
	VALUES (N'Case Nullification', @fid, 1, NULL, NULL, 0, NEWID())
set @deid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetCategoryElement] ([FieldOrder], [DatasetCategory_Id], [DatasetElement_Id], [Acute], [Chronic]) 
	select ISNULL(MAX(FieldOrder) + 1, 1), @dscid, @deid, 0, 0 from [DatasetCategoryElement] where [DatasetCategory_Id] = @dscid
-- element node
INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElement_Id, UpdatedBy_Id, DatasetXml_Id)
	VALUES ('casenullification', 2, NULL, GETDATE(), GETDATE(), @dsxcnid, 1, @deid, 1, @dsxid)
set @dsxenid = (SELECT @@IDENTITY)

-- nullificationreason
INSERT [dbo].[Field] ([Anonymise], [Mandatory], [MaxLength], [RegEx], [FieldType_Id]) 
	VALUES (0, 0, 200, NULL, 3)
set @fid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetElement] ([ElementName], [Field_Id], [DatasetElementType_Id], [OID], [DefaultValue], [System], [DatasetElementGuid]) 
	VALUES (N'Nullification Reason', @fid, 1, NULL, NULL, 0, NEWID())
set @deid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetCategoryElement] ([FieldOrder], [DatasetCategory_Id], [DatasetElement_Id], [Acute], [Chronic]) 
	select ISNULL(MAX(FieldOrder) + 1, 1), @dscid, @deid, 0, 0 from [DatasetCategoryElement] where [DatasetCategory_Id] = @dscid
-- element node
INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElement_Id, UpdatedBy_Id, DatasetXml_Id)
	VALUES ('nullificationreason', 2, NULL, GETDATE(), GETDATE(), @dsxcnid, 1, @deid, 1, @dsxid)
set @dsxenid = (SELECT @@IDENTITY)

-- medicallyconfirm
INSERT [dbo].[Field] ([Anonymise], [Mandatory], [FieldType_Id]) 
	VALUES (0, 0, 2)
set @fid = (SELECT @@IDENTITY)
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
	VALUES ('1=Yes', 1, 0, 0, @fid)
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
	VALUES ('2=No', 0, 0, 0, @fid)
INSERT [dbo].[DatasetElement] ([ElementName], [Field_Id], [DatasetElementType_Id], [OID], [DefaultValue], [System], [DatasetElementGuid]) 
	VALUES (N'Medically Confirm', @fid, 1, NULL, NULL, 0, NEWID())
set @deid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetCategoryElement] ([FieldOrder], [DatasetCategory_Id], [DatasetElement_Id], [Acute], [Chronic]) 
	select ISNULL(MAX(FieldOrder) + 1, 1), @dscid, @deid, 0, 0 from [DatasetCategoryElement] where [DatasetCategory_Id] = @dscid
-- element node
INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElement_Id, UpdatedBy_Id, DatasetXml_Id)
	VALUES ('medicallyconfirm', 2, NULL, GETDATE(), GETDATE(), @dsxcnid, 1, @deid, 1, @dsxid)
set @dsxenid = (SELECT @@IDENTITY)

/**************************************************
CATEGORY primarysource
**************************************************/
INSERT [dbo].[DatasetCategory] ([DatasetCategoryName], [CategoryOrder], [Dataset_Id]) 
	VALUES (N'Primary Source', 1, @dsid)
set @dscid = (SELECT @@IDENTITY)

-- category node
INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElement_Id, UpdatedBy_Id, DatasetXml_Id)
	VALUES ('primarysource', 2, NULL, GETDATE(), GETDATE(), @dsxcnid, 1, NULL, 1, @dsxid)
set @dsxcnid2 = (SELECT @@IDENTITY)

-- reportertitle
INSERT [dbo].[Field] ([Anonymise], [Mandatory], [MaxLength], [RegEx], [FieldType_Id]) 
	VALUES (0, 0, 50, NULL, 3)
set @fid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetElement] ([ElementName], [Field_Id], [DatasetElementType_Id], [OID], [DefaultValue], [System], [DatasetElementGuid]) 
	VALUES (N'Reporter Title', @fid, 1, NULL, NULL, 0, NEWID())
set @deid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetCategoryElement] ([FieldOrder], [DatasetCategory_Id], [DatasetElement_Id], [Acute], [Chronic]) 
	select ISNULL(MAX(FieldOrder) + 1, 1), @dscid, @deid, 0, 0 from [DatasetCategoryElement] where [DatasetCategory_Id] = @dscid
-- element node
INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElement_Id, UpdatedBy_Id, DatasetXml_Id)
	VALUES ('reportertitle', 2, NULL, GETDATE(), GETDATE(), @dsxcnid2, 1, @deid, 1, @dsxid)
set @dsxenid = (SELECT @@IDENTITY)

-- reportergivename
INSERT [dbo].[Field] ([Anonymise], [Mandatory], [MaxLength], [RegEx], [FieldType_Id]) 
	VALUES (0, 0, 35, NULL, 3)
set @fid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetElement] ([ElementName], [Field_Id], [DatasetElementType_Id], [OID], [DefaultValue], [System], [DatasetElementGuid]) 
	VALUES (N'Reporter Given Name', @fid, 1, NULL, NULL, 0, NEWID())
set @deid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetCategoryElement] ([FieldOrder], [DatasetCategory_Id], [DatasetElement_Id], [Acute], [Chronic]) 
	select ISNULL(MAX(FieldOrder) + 1, 1), @dscid, @deid, 0, 0 from [DatasetCategoryElement] where [DatasetCategory_Id] = @dscid
-- element node
INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElement_Id, UpdatedBy_Id, DatasetXml_Id)
	VALUES ('reportergivename', 2, NULL, GETDATE(), GETDATE(), @dsxcnid2, 1, @deid, 1, @dsxid)
set @dsxenid = (SELECT @@IDENTITY)

-- reportermiddlename
INSERT [dbo].[Field] ([Anonymise], [Mandatory], [MaxLength], [RegEx], [FieldType_Id]) 
	VALUES (0, 0, 15, NULL, 3)
set @fid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetElement] ([ElementName], [Field_Id], [DatasetElementType_Id], [OID], [DefaultValue], [System], [DatasetElementGuid]) 
	VALUES (N'Reporter Middle Name', @fid, 1, NULL, NULL, 0, NEWID())
set @deid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetCategoryElement] ([FieldOrder], [DatasetCategory_Id], [DatasetElement_Id], [Acute], [Chronic]) 
	select ISNULL(MAX(FieldOrder) + 1, 1), @dscid, @deid, 0, 0 from [DatasetCategoryElement] where [DatasetCategory_Id] = @dscid
-- element node
INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElement_Id, UpdatedBy_Id, DatasetXml_Id)
	VALUES ('reportermiddlename', 2, NULL, GETDATE(), GETDATE(), @dsxcnid2, 1, @deid, 1, @dsxid)
set @dsxenid = (SELECT @@IDENTITY)

-- reporterfamilyname
INSERT [dbo].[Field] ([Anonymise], [Mandatory], [MaxLength], [RegEx], [FieldType_Id]) 
	VALUES (0, 0, 50, NULL, 3)
set @fid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetElement] ([ElementName], [Field_Id], [DatasetElementType_Id], [OID], [DefaultValue], [System], [DatasetElementGuid]) 
	VALUES (N'Reporter Family Name', @fid, 1, NULL, NULL, 0, NEWID())
set @deid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetCategoryElement] ([FieldOrder], [DatasetCategory_Id], [DatasetElement_Id], [Acute], [Chronic]) 
	select ISNULL(MAX(FieldOrder) + 1, 1), @dscid, @deid, 0, 0 from [DatasetCategoryElement] where [DatasetCategory_Id] = @dscid
-- element node
INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElement_Id, UpdatedBy_Id, DatasetXml_Id)
	VALUES ('reporterfamilyname', 2, NULL, GETDATE(), GETDATE(), @dsxcnid2, 1, @deid, 1, @dsxid)
set @dsxenid = (SELECT @@IDENTITY)

-- reporterorganization
INSERT [dbo].[Field] ([Anonymise], [Mandatory], [MaxLength], [RegEx], [FieldType_Id]) 
	VALUES (0, 0, 60, NULL, 3)
set @fid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetElement] ([ElementName], [Field_Id], [DatasetElementType_Id], [OID], [DefaultValue], [System], [DatasetElementGuid]) 
	VALUES (N'Reporter Organization', @fid, 1, NULL, NULL, 0, NEWID())
set @deid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetCategoryElement] ([FieldOrder], [DatasetCategory_Id], [DatasetElement_Id], [Acute], [Chronic]) 
	select ISNULL(MAX(FieldOrder) + 1, 1), @dscid, @deid, 0, 0 from [DatasetCategoryElement] where [DatasetCategory_Id] = @dscid
set @dceid = (SELECT @@IDENTITY)
-- element node
INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElement_Id, UpdatedBy_Id, DatasetXml_Id)
	VALUES ('reporterorganization', 2, NULL, GETDATE(), GETDATE(), @dsxcnid2, 1, @deid, 1, @dsxid)
set @dsxenid = (SELECT @@IDENTITY)
-- mapping
INSERT [dbo].[DatasetMapping] (Tag, MappingType, MappingOption, DestinationElement_Id, SourceElement_Id)
	VALUES ('Spontaneous', 0, '', @dceid, (select top 1 dce.Id from [dbo].[DatasetCategoryElement] dce inner join [dbo].[DatasetElement] de on dce.DatasetElement_Id = de.Id where de.ElementName = 'Reporter Place of Practice'))

-- reporterdepartment
INSERT [dbo].[Field] ([Anonymise], [Mandatory], [MaxLength], [RegEx], [FieldType_Id]) 
	VALUES (0, 0, 60, NULL, 3)
set @fid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetElement] ([ElementName], [Field_Id], [DatasetElementType_Id], [OID], [DefaultValue], [System], [DatasetElementGuid]) 
	VALUES (N'Reporter Department', @fid, 1, NULL, NULL, 0, NEWID())
set @deid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetCategoryElement] ([FieldOrder], [DatasetCategory_Id], [DatasetElement_Id], [Acute], [Chronic]) 
	select ISNULL(MAX(FieldOrder) + 1, 1), @dscid, @deid, 0, 0 from [DatasetCategoryElement] where [DatasetCategory_Id] = @dscid
-- element node
INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElement_Id, UpdatedBy_Id, DatasetXml_Id)
	VALUES ('reporterdepartment', 2, NULL, GETDATE(), GETDATE(), @dsxcnid2, 1, @deid, 1, @dsxid)
set @dsxenid = (SELECT @@IDENTITY)

-- reporterstreet
INSERT [dbo].[Field] ([Anonymise], [Mandatory], [MaxLength], [RegEx], [FieldType_Id]) 
	VALUES (0, 0, 100, NULL, 3)
set @fid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetElement] ([ElementName], [Field_Id], [DatasetElementType_Id], [OID], [DefaultValue], [System], [DatasetElementGuid]) 
	VALUES (N'Reporter Street', @fid, 1, NULL, NULL, 0, NEWID())
set @deid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetCategoryElement] ([FieldOrder], [DatasetCategory_Id], [DatasetElement_Id], [Acute], [Chronic]) 
	select ISNULL(MAX(FieldOrder) + 1, 1), @dscid, @deid, 0, 0 from [DatasetCategoryElement] where [DatasetCategory_Id] = @dscid
set @dceid = (SELECT @@IDENTITY)
-- element node
INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElement_Id, UpdatedBy_Id, DatasetXml_Id)
	VALUES ('reporterstreet', 2, NULL, GETDATE(), GETDATE(), @dsxcnid2, 1, @deid, 1, @dsxid)
set @dsxenid = (SELECT @@IDENTITY)

-- reportercity
INSERT [dbo].[Field] ([Anonymise], [Mandatory], [MaxLength], [RegEx], [FieldType_Id]) 
	VALUES (0, 0, 35, NULL, 3)
set @fid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetElement] ([ElementName], [Field_Id], [DatasetElementType_Id], [OID], [DefaultValue], [System], [DatasetElementGuid]) 
	VALUES (N'Reporter City', @fid, 1, NULL, NULL, 0, NEWID())
set @deid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetCategoryElement] ([FieldOrder], [DatasetCategory_Id], [DatasetElement_Id], [Acute], [Chronic]) 
	select ISNULL(MAX(FieldOrder) + 1, 1), @dscid, @deid, 0, 0 from [DatasetCategoryElement] where [DatasetCategory_Id] = @dscid
-- element node
INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElement_Id, UpdatedBy_Id, DatasetXml_Id)
	VALUES ('reportercity', 2, NULL, GETDATE(), GETDATE(), @dsxcnid2, 1, @deid, 1, @dsxid)
set @dsxenid = (SELECT @@IDENTITY)

-- reporterstate
INSERT [dbo].[Field] ([Anonymise], [Mandatory], [MaxLength], [RegEx], [FieldType_Id]) 
	VALUES (0, 0, 40, NULL, 3)
set @fid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetElement] ([ElementName], [Field_Id], [DatasetElementType_Id], [OID], [DefaultValue], [System], [DatasetElementGuid]) 
	VALUES (N'Reporter State', @fid, 1, NULL, NULL, 0, NEWID())
set @deid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetCategoryElement] ([FieldOrder], [DatasetCategory_Id], [DatasetElement_Id], [Acute], [Chronic]) 
	select ISNULL(MAX(FieldOrder) + 1, 1), @dscid, @deid, 0, 0 from [DatasetCategoryElement] where [DatasetCategory_Id] = @dscid
-- element node
INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElement_Id, UpdatedBy_Id, DatasetXml_Id)
	VALUES ('reporterstate', 2, NULL, GETDATE(), GETDATE(), @dsxcnid2, 1, @deid, 1, @dsxid)
set @dsxenid = (SELECT @@IDENTITY)

-- reporterpostcode
INSERT [dbo].[Field] ([Anonymise], [Mandatory], [MaxLength], [RegEx], [FieldType_Id]) 
	VALUES (0, 0, 15, NULL, 3)
set @fid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetElement] ([ElementName], [Field_Id], [DatasetElementType_Id], [OID], [DefaultValue], [System], [DatasetElementGuid]) 
	VALUES (N'Reporter Postcode', @fid, 1, NULL, NULL, 0, NEWID())
set @deid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetCategoryElement] ([FieldOrder], [DatasetCategory_Id], [DatasetElement_Id], [Acute], [Chronic]) 
	select ISNULL(MAX(FieldOrder) + 1, 1), @dscid, @deid, 0, 0 from [DatasetCategoryElement] where [DatasetCategory_Id] = @dscid
-- element node
INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElement_Id, UpdatedBy_Id, DatasetXml_Id)
	VALUES ('reporterpostcode', 2, NULL, GETDATE(), GETDATE(), @dsxcnid2, 1, @deid, 1, @dsxid)
set @dsxenid = (SELECT @@IDENTITY)

-- reportercountry
INSERT [dbo].[Field] ([Anonymise], [Mandatory], [MaxLength], [RegEx], [FieldType_Id]) 
	VALUES (0, 0, 2, NULL, 3)
set @fid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetElement] ([ElementName], [Field_Id], [DatasetElementType_Id], [OID], [DefaultValue], [System], [DatasetElementGuid]) 
	VALUES (N'Reporter Country', @fid, 1, NULL, 'PH', 0, NEWID())
set @deid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetCategoryElement] ([FieldOrder], [DatasetCategory_Id], [DatasetElement_Id], [Acute], [Chronic]) 
	select ISNULL(MAX(FieldOrder) + 1, 1), @dscid, @deid, 0, 0 from [DatasetCategoryElement] where [DatasetCategory_Id] = @dscid
-- element node
INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElement_Id, UpdatedBy_Id, DatasetXml_Id)
	VALUES ('reportercountry', 2, NULL, GETDATE(), GETDATE(), @dsxcnid2, 1, @deid, 1, @dsxid)
set @dsxenid = (SELECT @@IDENTITY)

-- qualification
INSERT [dbo].[Field] ([Anonymise], [Mandatory], [FieldType_Id]) 
	VALUES (0, 0, 2)
set @fid = (SELECT @@IDENTITY)
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
	VALUES ('1=Physician', 1, 0, 0, @fid)
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
	VALUES ('2=Pharmacist', 0, 0, 0, @fid)
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
	VALUES ('3=Other Health Professional', 0, 0, 0, @fid)
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
	VALUES ('4=Lawyer', 0, 0, 0, @fid)
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
	VALUES ('5=Consumer or other non health professional', 0, 0, 0, @fid)
INSERT [dbo].[DatasetElement] ([ElementName], [Field_Id], [DatasetElementType_Id], [OID], [DefaultValue], [System], [DatasetElementGuid]) 
	VALUES (N'Qualification', @fid, 1, NULL, NULL, 0, NEWID())
set @deid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetCategoryElement] ([FieldOrder], [DatasetCategory_Id], [DatasetElement_Id], [Acute], [Chronic]) 
	select ISNULL(MAX(FieldOrder) + 1, 1), @dscid, @deid, 0, 0 from [DatasetCategoryElement] where [DatasetCategory_Id] = @dscid
set @dceid = (SELECT @@IDENTITY)
-- element node
INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElement_Id, UpdatedBy_Id, DatasetXml_Id)
	VALUES ('qualification', 2, NULL, GETDATE(), GETDATE(), @dsxcnid2, 1, @deid, 1, @dsxid)
set @dsxenid = (SELECT @@IDENTITY)
-- mapping
INSERT [dbo].[DatasetMapping] (Tag, MappingType, MappingOption, DestinationElement_Id, SourceElement_Id)
	VALUES ('Spontaneous', 1, '', @dceid, (select top 1 dce.Id from [dbo].[DatasetCategoryElement] dce inner join [dbo].[DatasetElement] de on dce.DatasetElement_Id = de.Id where de.ElementName = 'Reporter Profession'))
set @dmid = (SELECT @@IDENTITY)
-- mapping values
INSERT [dbo].[DatasetMappingValue] (SourceValue, DestinationValue, Active, Mapping_Id)
	SELECT 'Physician', '1=Physician', 1, @dmid UNION
	SELECT 'Pharmacist', '2=Pharmacist', 1, @dmid UNION
	SELECT 'Other Health Professional', '3=Other Health Professional', 1, @dmid UNION
	SELECT 'Consumer or other non-health professional', '5=Consumer or other non health professional', 1, @dmid UNION
	SELECT 'Lawyer', '4=Lawyer', 1, @dmid

-- studyname
INSERT [dbo].[Field] ([Anonymise], [Mandatory], [MaxLength], [RegEx], [FieldType_Id]) 
	VALUES (0, 0, 100, NULL, 3)
set @fid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetElement] ([ElementName], [Field_Id], [DatasetElementType_Id], [OID], [DefaultValue], [System], [DatasetElementGuid]) 
	VALUES (N'Study Name', @fid, 1, NULL, NULL, 0, NEWID())
set @deid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetCategoryElement] ([FieldOrder], [DatasetCategory_Id], [DatasetElement_Id], [Acute], [Chronic]) 
	select ISNULL(MAX(FieldOrder) + 1, 1), @dscid, @deid, 0, 0 from [DatasetCategoryElement] where [DatasetCategory_Id] = @dscid
-- element node
INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElement_Id, UpdatedBy_Id, DatasetXml_Id)
	VALUES ('studyname', 2, NULL, GETDATE(), GETDATE(), @dsxcnid2, 1, @deid, 1, @dsxid)
set @dsxenid = (SELECT @@IDENTITY)

-- sponsorstudynumb
INSERT [dbo].[Field] ([Anonymise], [Mandatory], [MaxLength], [RegEx], [FieldType_Id]) 
	VALUES (0, 0, 35, NULL, 3)
set @fid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetElement] ([ElementName], [Field_Id], [DatasetElementType_Id], [OID], [DefaultValue], [System], [DatasetElementGuid]) 
	VALUES (N'Sponsor Study Number', @fid, 1, NULL, NULL, 0, NEWID())
set @deid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetCategoryElement] ([FieldOrder], [DatasetCategory_Id], [DatasetElement_Id], [Acute], [Chronic]) 
	select ISNULL(MAX(FieldOrder) + 1, 1), @dscid, @deid, 0, 0 from [DatasetCategoryElement] where [DatasetCategory_Id] = @dscid
set @dceid = (SELECT @@IDENTITY)
-- element node
INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElement_Id, UpdatedBy_Id, DatasetXml_Id)
	VALUES ('sponsorstudynumb', 2, NULL, GETDATE(), GETDATE(), @dsxcnid2, 1, @deid, 1, @dsxid)
set @dsxenid = (SELECT @@IDENTITY)

-- observestudytype
INSERT [dbo].[Field] ([Anonymise], [Mandatory], [FieldType_Id]) 
	VALUES (0, 0, 2)
set @fid = (SELECT @@IDENTITY)
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
	VALUES ('1=Clinical trials', 1, 0, 0, @fid)
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
	VALUES ('2=Individual patient use', 0, 0, 0, @fid)
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
	VALUES ('3=Other studies', 0, 0, 0, @fid)
INSERT [dbo].[DatasetElement] ([ElementName], [Field_Id], [DatasetElementType_Id], [OID], [DefaultValue], [System], [DatasetElementGuid]) 
	VALUES (N'Observation Study Type', @fid, 1, NULL, NULL, 0, NEWID())
set @deid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetCategoryElement] ([FieldOrder], [DatasetCategory_Id], [DatasetElement_Id], [Acute], [Chronic]) 
	select ISNULL(MAX(FieldOrder) + 1, 1), @dscid, @deid, 0, 0 from [DatasetCategoryElement] where [DatasetCategory_Id] = @dscid
-- element node
INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElement_Id, UpdatedBy_Id, DatasetXml_Id)
	VALUES ('observestudytype', 2, NULL, GETDATE(), GETDATE(), @dsxcnid2, 1, @deid, 1, @dsxid)
set @dsxenid = (SELECT @@IDENTITY)

/**************************************************
CATEGORY sender
**************************************************/
INSERT [dbo].[DatasetCategory] ([DatasetCategoryName], [CategoryOrder], [Dataset_Id]) 
	VALUES (N'Sender', 1, @dsid)
set @dscid = (SELECT @@IDENTITY)

-- category node
INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElement_Id, UpdatedBy_Id, DatasetXml_Id)
	VALUES ('sender', 2, NULL, GETDATE(), GETDATE(), @dsxcnid, 1, NULL, 1, @dsxid)
set @dsxcnid2 = (SELECT @@IDENTITY)

-- sendertype
INSERT [dbo].[Field] ([Anonymise], [Mandatory], [FieldType_Id]) 
	VALUES (0, 0, 2)
set @fid = (SELECT @@IDENTITY)
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
	VALUES ('1=Pharmaceutical Company', 1, 0, 0, @fid)
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
	VALUES ('2=Regulatory Authority', 0, 0, 0, @fid)
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
	VALUES ('3=Health professional', 0, 0, 0, @fid)
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
	VALUES ('4=Regional Pharmacovigilance Center', 0, 0, 0, @fid)
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
	VALUES ('5=WHO Collaborating Center for International Drug Monitoring', 0, 0, 0, @fid)
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
	VALUES ('6=Other', 0, 1, 0, @fid)
INSERT [dbo].[DatasetElement] ([ElementName], [Field_Id], [DatasetElementType_Id], [OID], [DefaultValue], [System], [DatasetElementGuid]) 
	VALUES (N'Sender Type', @fid, 1, NULL, NULL, 0, NEWID())
set @deid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetCategoryElement] ([FieldOrder], [DatasetCategory_Id], [DatasetElement_Id], [Acute], [Chronic]) 
	select ISNULL(MAX(FieldOrder) + 1, 1), @dscid, @deid, 0, 0 from [DatasetCategoryElement] where [DatasetCategory_Id] = @dscid
-- element node
INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElement_Id, UpdatedBy_Id, DatasetXml_Id)
	VALUES ('sendertype', 2, NULL, GETDATE(), GETDATE(), @dsxcnid2, 1, @deid, 1, @dsxid)
set @dsxenid = (SELECT @@IDENTITY)

-- senderorganization
INSERT [dbo].[Field] ([Anonymise], [Mandatory], [MaxLength], [RegEx], [FieldType_Id]) 
	VALUES (0, 0, 60, NULL, 3)
set @fid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetElement] ([ElementName], [Field_Id], [DatasetElementType_Id], [OID], [DefaultValue], [System], [DatasetElementGuid]) 
	VALUES (N'Sender Organization', @fid, 1, NULL, NULL, 0, NEWID())
set @deid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetCategoryElement] ([FieldOrder], [DatasetCategory_Id], [DatasetElement_Id], [Acute], [Chronic]) 
	select ISNULL(MAX(FieldOrder) + 1, 1), @dscid, @deid, 0, 0 from [DatasetCategoryElement] where [DatasetCategory_Id] = @dscid
-- element node
INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElement_Id, UpdatedBy_Id, DatasetXml_Id)
	VALUES ('senderorganization', 2, NULL, GETDATE(), GETDATE(), @dsxcnid2, 1, @deid, 1, @dsxid)
set @dsxenid = (SELECT @@IDENTITY)

-- senderdepartment
INSERT [dbo].[Field] ([Anonymise], [Mandatory], [MaxLength], [RegEx], [FieldType_Id]) 
	VALUES (0, 0, 60, NULL, 3)
set @fid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetElement] ([ElementName], [Field_Id], [DatasetElementType_Id], [OID], [DefaultValue], [System], [DatasetElementGuid]) 
	VALUES (N'Sender Department', @fid, 1, NULL, NULL, 0, NEWID())
set @deid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetCategoryElement] ([FieldOrder], [DatasetCategory_Id], [DatasetElement_Id], [Acute], [Chronic]) 
	select ISNULL(MAX(FieldOrder) + 1, 1), @dscid, @deid, 0, 0 from [DatasetCategoryElement] where [DatasetCategory_Id] = @dscid
-- element node
INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElement_Id, UpdatedBy_Id, DatasetXml_Id)
	VALUES ('senderdepartment', 2, NULL, GETDATE(), GETDATE(), @dsxcnid2, 1, @deid, 1, @dsxid)
set @dsxenid = (SELECT @@IDENTITY)

-- sendertitle
INSERT [dbo].[Field] ([Anonymise], [Mandatory], [MaxLength], [RegEx], [FieldType_Id]) 
	VALUES (0, 0, 10, NULL, 3)
set @fid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetElement] ([ElementName], [Field_Id], [DatasetElementType_Id], [OID], [DefaultValue], [System], [DatasetElementGuid]) 
	VALUES (N'Sender Title', @fid, 1, NULL, NULL, 0, NEWID())
set @deid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetCategoryElement] ([FieldOrder], [DatasetCategory_Id], [DatasetElement_Id], [Acute], [Chronic]) 
	select ISNULL(MAX(FieldOrder) + 1, 1), @dscid, @deid, 0, 0 from [DatasetCategoryElement] where [DatasetCategory_Id] = @dscid
-- element node
INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElement_Id, UpdatedBy_Id, DatasetXml_Id)
	VALUES ('sendertitle', 2, NULL, GETDATE(), GETDATE(), @dsxcnid2, 1, @deid, 1, @dsxid)
set @dsxenid = (SELECT @@IDENTITY)

-- sendergivename
INSERT [dbo].[Field] ([Anonymise], [Mandatory], [MaxLength], [RegEx], [FieldType_Id]) 
	VALUES (0, 0, 35, NULL, 3)
set @fid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetElement] ([ElementName], [Field_Id], [DatasetElementType_Id], [OID], [DefaultValue], [System], [DatasetElementGuid]) 
	VALUES (N'Sender Given Name', @fid, 1, NULL, NULL, 0, NEWID())
set @deid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetCategoryElement] ([FieldOrder], [DatasetCategory_Id], [DatasetElement_Id], [Acute], [Chronic]) 
	select ISNULL(MAX(FieldOrder) + 1, 1), @dscid, @deid, 0, 0 from [DatasetCategoryElement] where [DatasetCategory_Id] = @dscid
-- element node
INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElement_Id, UpdatedBy_Id, DatasetXml_Id)
	VALUES ('sendergivename', 2, NULL, GETDATE(), GETDATE(), @dsxcnid2, 1, @deid, 1, @dsxid)
set @dsxenid = (SELECT @@IDENTITY)

-- sendermiddlename
INSERT [dbo].[Field] ([Anonymise], [Mandatory], [MaxLength], [RegEx], [FieldType_Id]) 
	VALUES (0, 0, 15, NULL, 3)
set @fid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetElement] ([ElementName], [Field_Id], [DatasetElementType_Id], [OID], [DefaultValue], [System], [DatasetElementGuid]) 
	VALUES (N'Sender Middle Name', @fid, 1, NULL, NULL, 0, NEWID())
set @deid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetCategoryElement] ([FieldOrder], [DatasetCategory_Id], [DatasetElement_Id], [Acute], [Chronic]) 
	select ISNULL(MAX(FieldOrder) + 1, 1), @dscid, @deid, 0, 0 from [DatasetCategoryElement] where [DatasetCategory_Id] = @dscid
-- element node
INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElement_Id, UpdatedBy_Id, DatasetXml_Id)
	VALUES ('sendermiddlename', 2, NULL, GETDATE(), GETDATE(), @dsxcnid2, 1, @deid, 1, @dsxid)
set @dsxenid = (SELECT @@IDENTITY)

-- senderfamilyname
INSERT [dbo].[Field] ([Anonymise], [Mandatory], [MaxLength], [RegEx], [FieldType_Id]) 
	VALUES (0, 0, 35, NULL, 3)
set @fid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetElement] ([ElementName], [Field_Id], [DatasetElementType_Id], [OID], [DefaultValue], [System], [DatasetElementGuid]) 
	VALUES (N'Sender Family Name', @fid, 1, NULL, NULL, 0, NEWID())
set @deid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetCategoryElement] ([FieldOrder], [DatasetCategory_Id], [DatasetElement_Id], [Acute], [Chronic]) 
	select ISNULL(MAX(FieldOrder) + 1, 1), @dscid, @deid, 0, 0 from [DatasetCategoryElement] where [DatasetCategory_Id] = @dscid
-- element node
INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElement_Id, UpdatedBy_Id, DatasetXml_Id)
	VALUES ('senderfamilyname', 2, NULL, GETDATE(), GETDATE(), @dsxcnid2, 1, @deid, 1, @dsxid)
set @dsxenid = (SELECT @@IDENTITY)

-- senderstreetaddress
INSERT [dbo].[Field] ([Anonymise], [Mandatory], [MaxLength], [RegEx], [FieldType_Id]) 
	VALUES (0, 0, 100, NULL, 3)
set @fid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetElement] ([ElementName], [Field_Id], [DatasetElementType_Id], [OID], [DefaultValue], [System], [DatasetElementGuid]) 
	VALUES (N'Sender Street Address', @fid, 1, NULL, NULL, 0, NEWID())
set @deid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetCategoryElement] ([FieldOrder], [DatasetCategory_Id], [DatasetElement_Id], [Acute], [Chronic]) 
	select ISNULL(MAX(FieldOrder) + 1, 1), @dscid, @deid, 0, 0 from [DatasetCategoryElement] where [DatasetCategory_Id] = @dscid
-- element node
INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElement_Id, UpdatedBy_Id, DatasetXml_Id)
	VALUES ('senderstreetaddress', 2, NULL, GETDATE(), GETDATE(), @dsxcnid2, 1, @deid, 1, @dsxid)
set @dsxenid = (SELECT @@IDENTITY)

-- sendercity
INSERT [dbo].[Field] ([Anonymise], [Mandatory], [MaxLength], [RegEx], [FieldType_Id]) 
	VALUES (0, 0, 35, NULL, 3)
set @fid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetElement] ([ElementName], [Field_Id], [DatasetElementType_Id], [OID], [DefaultValue], [System], [DatasetElementGuid]) 
	VALUES (N'Sender City', @fid, 1, NULL, NULL, 0, NEWID())
set @deid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetCategoryElement] ([FieldOrder], [DatasetCategory_Id], [DatasetElement_Id], [Acute], [Chronic]) 
	select ISNULL(MAX(FieldOrder) + 1, 1), @dscid, @deid, 0, 0 from [DatasetCategoryElement] where [DatasetCategory_Id] = @dscid
-- element node
INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElement_Id, UpdatedBy_Id, DatasetXml_Id)
	VALUES ('sendercity', 2, NULL, GETDATE(), GETDATE(), @dsxcnid2, 1, @deid, 1, @dsxid)
set @dsxenid = (SELECT @@IDENTITY)

-- senderstate
INSERT [dbo].[Field] ([Anonymise], [Mandatory], [MaxLength], [RegEx], [FieldType_Id]) 
	VALUES (0, 0, 40, NULL, 3)
set @fid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetElement] ([ElementName], [Field_Id], [DatasetElementType_Id], [OID], [DefaultValue], [System], [DatasetElementGuid]) 
	VALUES (N'Sender State', @fid, 1, NULL, NULL, 0, NEWID())
set @deid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetCategoryElement] ([FieldOrder], [DatasetCategory_Id], [DatasetElement_Id], [Acute], [Chronic]) 
	select ISNULL(MAX(FieldOrder) + 1, 1), @dscid, @deid, 0, 0 from [DatasetCategoryElement] where [DatasetCategory_Id] = @dscid
-- element node
INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElement_Id, UpdatedBy_Id, DatasetXml_Id)
	VALUES ('senderstate', 2, NULL, GETDATE(), GETDATE(), @dsxcnid2, 1, @deid, 1, @dsxid)
set @dsxenid = (SELECT @@IDENTITY)

-- senderpostcode
INSERT [dbo].[Field] ([Anonymise], [Mandatory], [MaxLength], [RegEx], [FieldType_Id]) 
	VALUES (0, 0, 15, NULL, 3)
set @fid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetElement] ([ElementName], [Field_Id], [DatasetElementType_Id], [OID], [DefaultValue], [System], [DatasetElementGuid]) 
	VALUES (N'Sender Postcode', @fid, 1, NULL, NULL, 0, NEWID())
set @deid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetCategoryElement] ([FieldOrder], [DatasetCategory_Id], [DatasetElement_Id], [Acute], [Chronic]) 
	select ISNULL(MAX(FieldOrder) + 1, 1), @dscid, @deid, 0, 0 from [DatasetCategoryElement] where [DatasetCategory_Id] = @dscid
-- element node
INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElement_Id, UpdatedBy_Id, DatasetXml_Id)
	VALUES ('senderpostcode', 2, NULL, GETDATE(), GETDATE(), @dsxcnid2, 1, @deid, 1, @dsxid)
set @dsxenid = (SELECT @@IDENTITY)

-- sendercountry
INSERT [dbo].[Field] ([Anonymise], [Mandatory], [MaxLength], [RegEx], [FieldType_Id]) 
	VALUES (0, 0, 2, NULL, 3)
set @fid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetElement] ([ElementName], [Field_Id], [DatasetElementType_Id], [OID], [DefaultValue], [System], [DatasetElementGuid]) 
	VALUES (N'Sender Country', @fid, 1, NULL, 'PH', 0, NEWID())
set @deid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetCategoryElement] ([FieldOrder], [DatasetCategory_Id], [DatasetElement_Id], [Acute], [Chronic]) 
	select ISNULL(MAX(FieldOrder) + 1, 1), @dscid, @deid, 0, 0 from [DatasetCategoryElement] where [DatasetCategory_Id] = @dscid
-- element node
INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElement_Id, UpdatedBy_Id, DatasetXml_Id)
	VALUES ('sendercountry', 2, NULL, GETDATE(), GETDATE(), @dsxcnid2, 1, @deid, 1, @dsxid)
set @dsxenid = (SELECT @@IDENTITY)

-- sendertel
INSERT [dbo].[Field] ([Anonymise], [Mandatory], [MaxLength], [RegEx], [FieldType_Id]) 
	VALUES (0, 0, 10, NULL, 3)
set @fid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetElement] ([ElementName], [Field_Id], [DatasetElementType_Id], [OID], [DefaultValue], [System], [DatasetElementGuid]) 
	VALUES (N'Sender Tel Number', @fid, 1, NULL, NULL, 0, NEWID())
set @deid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetCategoryElement] ([FieldOrder], [DatasetCategory_Id], [DatasetElement_Id], [Acute], [Chronic]) 
	select ISNULL(MAX(FieldOrder) + 1, 1), @dscid, @deid, 0, 0 from [DatasetCategoryElement] where [DatasetCategory_Id] = @dscid
-- element node
INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElement_Id, UpdatedBy_Id, DatasetXml_Id)
	VALUES ('sendertel', 2, NULL, GETDATE(), GETDATE(), @dsxcnid2, 1, @deid, 1, @dsxid)
set @dsxenid = (SELECT @@IDENTITY)

-- sendertelextension
INSERT [dbo].[Field] ([Anonymise], [Mandatory], [MaxLength], [RegEx], [FieldType_Id]) 
	VALUES (0, 0, 5, NULL, 3)
set @fid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetElement] ([ElementName], [Field_Id], [DatasetElementType_Id], [OID], [DefaultValue], [System], [DatasetElementGuid]) 
	VALUES (N'Sender Tel Extension', @fid, 1, NULL, NULL, 0, NEWID())
set @deid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetCategoryElement] ([FieldOrder], [DatasetCategory_Id], [DatasetElement_Id], [Acute], [Chronic]) 
	select ISNULL(MAX(FieldOrder) + 1, 1), @dscid, @deid, 0, 0 from [DatasetCategoryElement] where [DatasetCategory_Id] = @dscid
-- element node
INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElement_Id, UpdatedBy_Id, DatasetXml_Id)
	VALUES ('sendertelextension', 2, NULL, GETDATE(), GETDATE(), @dsxcnid2, 1, @deid, 1, @dsxid)
set @dsxenid = (SELECT @@IDENTITY)

-- sendertelcountrycode
INSERT [dbo].[Field] ([Anonymise], [Mandatory], [MaxLength], [RegEx], [FieldType_Id]) 
	VALUES (0, 0, 3, NULL, 3)
set @fid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetElement] ([ElementName], [Field_Id], [DatasetElementType_Id], [OID], [DefaultValue], [System], [DatasetElementGuid]) 
	VALUES (N'Sender Tel Country Code', @fid, 1, NULL, NULL, 0, NEWID())
set @deid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetCategoryElement] ([FieldOrder], [DatasetCategory_Id], [DatasetElement_Id], [Acute], [Chronic]) 
	select ISNULL(MAX(FieldOrder) + 1, 1), @dscid, @deid, 0, 0 from [DatasetCategoryElement] where [DatasetCategory_Id] = @dscid
-- element node
INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElement_Id, UpdatedBy_Id, DatasetXml_Id)
	VALUES ('sendertelcountrycode', 2, NULL, GETDATE(), GETDATE(), @dsxcnid2, 1, @deid, 1, @dsxid)
set @dsxenid = (SELECT @@IDENTITY)

-- senderfax
INSERT [dbo].[Field] ([Anonymise], [Mandatory], [MaxLength], [RegEx], [FieldType_Id]) 
	VALUES (0, 0, 10, NULL, 3)
set @fid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetElement] ([ElementName], [Field_Id], [DatasetElementType_Id], [OID], [DefaultValue], [System], [DatasetElementGuid]) 
	VALUES (N'Sender Fax', @fid, 1, NULL, NULL, 0, NEWID())
set @deid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetCategoryElement] ([FieldOrder], [DatasetCategory_Id], [DatasetElement_Id], [Acute], [Chronic]) 
	select ISNULL(MAX(FieldOrder) + 1, 1), @dscid, @deid, 0, 0 from [DatasetCategoryElement] where [DatasetCategory_Id] = @dscid
-- element node
INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElement_Id, UpdatedBy_Id, DatasetXml_Id)
	VALUES ('senderfax', 2, NULL, GETDATE(), GETDATE(), @dsxcnid2, 1, @deid, 1, @dsxid)
set @dsxenid = (SELECT @@IDENTITY)

-- senderfaxextension
INSERT [dbo].[Field] ([Anonymise], [Mandatory], [MaxLength], [RegEx], [FieldType_Id]) 
	VALUES (0, 0, 5, NULL, 3)
set @fid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetElement] ([ElementName], [Field_Id], [DatasetElementType_Id], [OID], [DefaultValue], [System], [DatasetElementGuid]) 
	VALUES (N'Sender Fax Extension', @fid, 1, NULL, NULL, 0, NEWID())
set @deid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetCategoryElement] ([FieldOrder], [DatasetCategory_Id], [DatasetElement_Id], [Acute], [Chronic]) 
	select ISNULL(MAX(FieldOrder) + 1, 1), @dscid, @deid, 0, 0 from [DatasetCategoryElement] where [DatasetCategory_Id] = @dscid
-- element node
INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElement_Id, UpdatedBy_Id, DatasetXml_Id)
	VALUES ('senderfaxextension', 2, NULL, GETDATE(), GETDATE(), @dsxcnid2, 1, @deid, 1, @dsxid)
set @dsxenid = (SELECT @@IDENTITY)

-- senderfaxcountrycode
INSERT [dbo].[Field] ([Anonymise], [Mandatory], [MaxLength], [RegEx], [FieldType_Id]) 
	VALUES (0, 0, 3, NULL, 3)
set @fid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetElement] ([ElementName], [Field_Id], [DatasetElementType_Id], [OID], [DefaultValue], [System], [DatasetElementGuid]) 
	VALUES (N'Sender Fax Country Code', @fid, 1, NULL, NULL, 0, NEWID())
set @deid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetCategoryElement] ([FieldOrder], [DatasetCategory_Id], [DatasetElement_Id], [Acute], [Chronic]) 
	select ISNULL(MAX(FieldOrder) + 1, 1), @dscid, @deid, 0, 0 from [DatasetCategoryElement] where [DatasetCategory_Id] = @dscid
-- element node
INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElement_Id, UpdatedBy_Id, DatasetXml_Id)
	VALUES ('senderfaxcountrycode', 2, NULL, GETDATE(), GETDATE(), @dsxcnid2, 1, @deid, 1, @dsxid)
set @dsxenid = (SELECT @@IDENTITY)

-- senderemailaddress
INSERT [dbo].[Field] ([Anonymise], [Mandatory], [MaxLength], [RegEx], [FieldType_Id]) 
	VALUES (0, 0, 100, NULL, 3)
set @fid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetElement] ([ElementName], [Field_Id], [DatasetElementType_Id], [OID], [DefaultValue], [System], [DatasetElementGuid]) 
	VALUES (N'Sender Email Address', @fid, 1, NULL, NULL, 0, NEWID())
set @deid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetCategoryElement] ([FieldOrder], [DatasetCategory_Id], [DatasetElement_Id], [Acute], [Chronic]) 
	select ISNULL(MAX(FieldOrder) + 1, 1), @dscid, @deid, 0, 0 from [DatasetCategoryElement] where [DatasetCategory_Id] = @dscid
-- element node
INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElement_Id, UpdatedBy_Id, DatasetXml_Id)
	VALUES ('senderemailaddress', 2, NULL, GETDATE(), GETDATE(), @dsxcnid2, 1, @deid, 1, @dsxid)
set @dsxenid = (SELECT @@IDENTITY)

/**************************************************
CATEGORY receiver
**************************************************/
INSERT [dbo].[DatasetCategory] ([DatasetCategoryName], [CategoryOrder], [Dataset_Id]) 
	VALUES (N'Receiver', 1, @dsid)
set @dscid = (SELECT @@IDENTITY)

-- category node
INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElement_Id, UpdatedBy_Id, DatasetXml_Id)
	VALUES ('receiver', 2, NULL, GETDATE(), GETDATE(), @dsxcnid, 1, NULL, 1, @dsxid)
set @dsxcnid2 = (SELECT @@IDENTITY)

-- receivertype
INSERT [dbo].[Field] ([Anonymise], [Mandatory], [FieldType_Id]) 
	VALUES (0, 0, 2)
set @fid = (SELECT @@IDENTITY)
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
	VALUES ('1=Pharmaceutical Company', 1, 0, 0, @fid)
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
	VALUES ('2=Regulatory Authority', 0, 0, 0, @fid)
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
	VALUES ('3=Health professional', 0, 0, 0, @fid)
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
	VALUES ('4=Regional Pharmacovigilance Center', 0, 0, 0, @fid)
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
	VALUES ('5=WHO Collaborating Center for International Drug Monitoring', 0, 0, 0, @fid)
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
	VALUES ('6=Other', 0, 1, 0, @fid)
INSERT [dbo].[DatasetElement] ([ElementName], [Field_Id], [DatasetElementType_Id], [OID], [DefaultValue], [System], [DatasetElementGuid]) 
	VALUES (N'Receiver Type', @fid, 1, NULL, NULL, 0, NEWID())
set @deid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetCategoryElement] ([FieldOrder], [DatasetCategory_Id], [DatasetElement_Id], [Acute], [Chronic]) 
	select ISNULL(MAX(FieldOrder) + 1, 1), @dscid, @deid, 0, 0 from [DatasetCategoryElement] where [DatasetCategory_Id] = @dscid
-- element node
INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElement_Id, UpdatedBy_Id, DatasetXml_Id)
	VALUES ('receivertype', 2, NULL, GETDATE(), GETDATE(), @dsxcnid2, 1, @deid, 1, @dsxid)
set @dsxenid = (SELECT @@IDENTITY)

-- receiverorganization
INSERT [dbo].[Field] ([Anonymise], [Mandatory], [MaxLength], [RegEx], [FieldType_Id]) 
	VALUES (0, 0, 60, NULL, 3)
set @fid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetElement] ([ElementName], [Field_Id], [DatasetElementType_Id], [OID], [DefaultValue], [System], [DatasetElementGuid]) 
	VALUES (N'Receiver Organization', @fid, 1, NULL, NULL, 0, NEWID())
set @deid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetCategoryElement] ([FieldOrder], [DatasetCategory_Id], [DatasetElement_Id], [Acute], [Chronic]) 
	select ISNULL(MAX(FieldOrder) + 1, 1), @dscid, @deid, 0, 0 from [DatasetCategoryElement] where [DatasetCategory_Id] = @dscid
-- element node
INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElement_Id, UpdatedBy_Id, DatasetXml_Id)
	VALUES ('receiverorganization', 2, NULL, GETDATE(), GETDATE(), @dsxcnid2, 1, @deid, 1, @dsxid)
set @dsxenid = (SELECT @@IDENTITY)

-- receiverdepartment
INSERT [dbo].[Field] ([Anonymise], [Mandatory], [MaxLength], [RegEx], [FieldType_Id]) 
	VALUES (0, 0, 60, NULL, 3)
set @fid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetElement] ([ElementName], [Field_Id], [DatasetElementType_Id], [OID], [DefaultValue], [System], [DatasetElementGuid]) 
	VALUES (N'Receiver Department', @fid, 1, NULL, NULL, 0, NEWID())
set @deid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetCategoryElement] ([FieldOrder], [DatasetCategory_Id], [DatasetElement_Id], [Acute], [Chronic]) 
	select ISNULL(MAX(FieldOrder) + 1, 1), @dscid, @deid, 0, 0 from [DatasetCategoryElement] where [DatasetCategory_Id] = @dscid
-- element node
INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElement_Id, UpdatedBy_Id, DatasetXml_Id)
	VALUES ('receiverdepartment', 2, NULL, GETDATE(), GETDATE(), @dsxcnid2, 1, @deid, 1, @dsxid)
set @dsxenid = (SELECT @@IDENTITY)

-- receivertitle
INSERT [dbo].[Field] ([Anonymise], [Mandatory], [MaxLength], [RegEx], [FieldType_Id]) 
	VALUES (0, 0, 10, NULL, 3)
set @fid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetElement] ([ElementName], [Field_Id], [DatasetElementType_Id], [OID], [DefaultValue], [System], [DatasetElementGuid]) 
	VALUES (N'Receiver Title', @fid, 1, NULL, NULL, 0, NEWID())
set @deid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetCategoryElement] ([FieldOrder], [DatasetCategory_Id], [DatasetElement_Id], [Acute], [Chronic]) 
	select ISNULL(MAX(FieldOrder) + 1, 1), @dscid, @deid, 0, 0 from [DatasetCategoryElement] where [DatasetCategory_Id] = @dscid
-- element node
INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElement_Id, UpdatedBy_Id, DatasetXml_Id)
	VALUES ('receivertitle', 2, NULL, GETDATE(), GETDATE(), @dsxcnid2, 1, @deid, 1, @dsxid)
set @dsxenid = (SELECT @@IDENTITY)

-- receivergivename
INSERT [dbo].[Field] ([Anonymise], [Mandatory], [MaxLength], [RegEx], [FieldType_Id]) 
	VALUES (0, 0, 35, NULL, 3)
set @fid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetElement] ([ElementName], [Field_Id], [DatasetElementType_Id], [OID], [DefaultValue], [System], [DatasetElementGuid]) 
	VALUES (N'Receiver Given Name', @fid, 1, NULL, NULL, 0, NEWID())
set @deid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetCategoryElement] ([FieldOrder], [DatasetCategory_Id], [DatasetElement_Id], [Acute], [Chronic]) 
	select ISNULL(MAX(FieldOrder) + 1, 1), @dscid, @deid, 0, 0 from [DatasetCategoryElement] where [DatasetCategory_Id] = @dscid
-- element node
INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElement_Id, UpdatedBy_Id, DatasetXml_Id)
	VALUES ('receivergivename', 2, NULL, GETDATE(), GETDATE(), @dsxcnid2, 1, @deid, 1, @dsxid)
set @dsxenid = (SELECT @@IDENTITY)

-- receivermiddlename
INSERT [dbo].[Field] ([Anonymise], [Mandatory], [MaxLength], [RegEx], [FieldType_Id]) 
	VALUES (0, 0, 15, NULL, 3)
set @fid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetElement] ([ElementName], [Field_Id], [DatasetElementType_Id], [OID], [DefaultValue], [System], [DatasetElementGuid]) 
	VALUES (N'Receiver Middle Name', @fid, 1, NULL, NULL, 0, NEWID())
set @deid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetCategoryElement] ([FieldOrder], [DatasetCategory_Id], [DatasetElement_Id], [Acute], [Chronic]) 
	select ISNULL(MAX(FieldOrder) + 1, 1), @dscid, @deid, 0, 0 from [DatasetCategoryElement] where [DatasetCategory_Id] = @dscid
-- element node
INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElement_Id, UpdatedBy_Id, DatasetXml_Id)
	VALUES ('receivermiddlename', 2, NULL, GETDATE(), GETDATE(), @dsxcnid2, 1, @deid, 1, @dsxid)
set @dsxenid = (SELECT @@IDENTITY)

-- receiverfamilyname
INSERT [dbo].[Field] ([Anonymise], [Mandatory], [MaxLength], [RegEx], [FieldType_Id]) 
	VALUES (0, 0, 35, NULL, 3)
set @fid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetElement] ([ElementName], [Field_Id], [DatasetElementType_Id], [OID], [DefaultValue], [System], [DatasetElementGuid]) 
	VALUES (N'Receiver Family Name', @fid, 1, NULL, NULL, 0, NEWID())
set @deid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetCategoryElement] ([FieldOrder], [DatasetCategory_Id], [DatasetElement_Id], [Acute], [Chronic]) 
	select ISNULL(MAX(FieldOrder) + 1, 1), @dscid, @deid, 0, 0 from [DatasetCategoryElement] where [DatasetCategory_Id] = @dscid
-- element node
INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElement_Id, UpdatedBy_Id, DatasetXml_Id)
	VALUES ('receiverfamilyname', 2, NULL, GETDATE(), GETDATE(), @dsxcnid2, 1, @deid, 1, @dsxid)
set @dsxenid = (SELECT @@IDENTITY)

-- receiverstreetaddress
INSERT [dbo].[Field] ([Anonymise], [Mandatory], [MaxLength], [RegEx], [FieldType_Id]) 
	VALUES (0, 0, 100, NULL, 3)
set @fid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetElement] ([ElementName], [Field_Id], [DatasetElementType_Id], [OID], [DefaultValue], [System], [DatasetElementGuid]) 
	VALUES (N'Receiver Street Address', @fid, 1, NULL, NULL, 0, NEWID())
set @deid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetCategoryElement] ([FieldOrder], [DatasetCategory_Id], [DatasetElement_Id], [Acute], [Chronic]) 
	select ISNULL(MAX(FieldOrder) + 1, 1), @dscid, @deid, 0, 0 from [DatasetCategoryElement] where [DatasetCategory_Id] = @dscid
-- element node
INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElement_Id, UpdatedBy_Id, DatasetXml_Id)
	VALUES ('receiverstreetaddress', 2, NULL, GETDATE(), GETDATE(), @dsxcnid2, 1, @deid, 1, @dsxid)
set @dsxenid = (SELECT @@IDENTITY)

-- receivercity
INSERT [dbo].[Field] ([Anonymise], [Mandatory], [MaxLength], [RegEx], [FieldType_Id]) 
	VALUES (0, 0, 35, NULL, 3)
set @fid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetElement] ([ElementName], [Field_Id], [DatasetElementType_Id], [OID], [DefaultValue], [System], [DatasetElementGuid]) 
	VALUES (N'Receiver City', @fid, 1, NULL, NULL, 0, NEWID())
set @deid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetCategoryElement] ([FieldOrder], [DatasetCategory_Id], [DatasetElement_Id], [Acute], [Chronic]) 
	select ISNULL(MAX(FieldOrder) + 1, 1), @dscid, @deid, 0, 0 from [DatasetCategoryElement] where [DatasetCategory_Id] = @dscid
-- element node
INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElement_Id, UpdatedBy_Id, DatasetXml_Id)
	VALUES ('receivercity', 2, NULL, GETDATE(), GETDATE(), @dsxcnid2, 1, @deid, 1, @dsxid)
set @dsxenid = (SELECT @@IDENTITY)

-- receiverstate
INSERT [dbo].[Field] ([Anonymise], [Mandatory], [MaxLength], [RegEx], [FieldType_Id]) 
	VALUES (0, 0, 40, NULL, 3)
set @fid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetElement] ([ElementName], [Field_Id], [DatasetElementType_Id], [OID], [DefaultValue], [System], [DatasetElementGuid]) 
	VALUES (N'Receiver State', @fid, 1, NULL, NULL, 0, NEWID())
set @deid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetCategoryElement] ([FieldOrder], [DatasetCategory_Id], [DatasetElement_Id], [Acute], [Chronic]) 
	select ISNULL(MAX(FieldOrder) + 1, 1), @dscid, @deid, 0, 0 from [DatasetCategoryElement] where [DatasetCategory_Id] = @dscid
-- element node
INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElement_Id, UpdatedBy_Id, DatasetXml_Id)
	VALUES ('receiverstate', 2, NULL, GETDATE(), GETDATE(), @dsxcnid2, 1, @deid, 1, @dsxid)
set @dsxenid = (SELECT @@IDENTITY)

-- receiverpostcode
INSERT [dbo].[Field] ([Anonymise], [Mandatory], [MaxLength], [RegEx], [FieldType_Id]) 
	VALUES (0, 0, 15, NULL, 3)
set @fid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetElement] ([ElementName], [Field_Id], [DatasetElementType_Id], [OID], [DefaultValue], [System], [DatasetElementGuid]) 
	VALUES (N'Receiver Postcode', @fid, 1, NULL, NULL, 0, NEWID())
set @deid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetCategoryElement] ([FieldOrder], [DatasetCategory_Id], [DatasetElement_Id], [Acute], [Chronic]) 
	select ISNULL(MAX(FieldOrder) + 1, 1), @dscid, @deid, 0, 0 from [DatasetCategoryElement] where [DatasetCategory_Id] = @dscid
-- element node
INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElement_Id, UpdatedBy_Id, DatasetXml_Id)
	VALUES ('receiverpostcode', 2, NULL, GETDATE(), GETDATE(), @dsxcnid2, 1, @deid, 1, @dsxid)
set @dsxenid = (SELECT @@IDENTITY)

-- receivercountry
INSERT [dbo].[Field] ([Anonymise], [Mandatory], [MaxLength], [RegEx], [FieldType_Id]) 
	VALUES (0, 0, 2, NULL, 3)
set @fid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetElement] ([ElementName], [Field_Id], [DatasetElementType_Id], [OID], [DefaultValue], [System], [DatasetElementGuid]) 
	VALUES (N'Receiver Country', @fid, 1, NULL, 'PH', 0, NEWID())
set @deid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetCategoryElement] ([FieldOrder], [DatasetCategory_Id], [DatasetElement_Id], [Acute], [Chronic]) 
	select ISNULL(MAX(FieldOrder) + 1, 1), @dscid, @deid, 0, 0 from [DatasetCategoryElement] where [DatasetCategory_Id] = @dscid
-- element node
INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElement_Id, UpdatedBy_Id, DatasetXml_Id)
	VALUES ('receivercountry', 2, NULL, GETDATE(), GETDATE(), @dsxcnid2, 1, @deid, 1, @dsxid)
set @dsxenid = (SELECT @@IDENTITY)

-- receivertel
INSERT [dbo].[Field] ([Anonymise], [Mandatory], [MaxLength], [RegEx], [FieldType_Id]) 
	VALUES (0, 0, 10, NULL, 3)
set @fid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetElement] ([ElementName], [Field_Id], [DatasetElementType_Id], [OID], [DefaultValue], [System], [DatasetElementGuid]) 
	VALUES (N'Receiver Tel', @fid, 1, NULL, NULL, 0, NEWID())
set @deid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetCategoryElement] ([FieldOrder], [DatasetCategory_Id], [DatasetElement_Id], [Acute], [Chronic]) 
	select ISNULL(MAX(FieldOrder) + 1, 1), @dscid, @deid, 0, 0 from [DatasetCategoryElement] where [DatasetCategory_Id] = @dscid
-- element node
INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElement_Id, UpdatedBy_Id, DatasetXml_Id)
	VALUES ('receivertel', 2, NULL, GETDATE(), GETDATE(), @dsxcnid2, 1, @deid, 1, @dsxid)
set @dsxenid = (SELECT @@IDENTITY)

-- receivertelextension
INSERT [dbo].[Field] ([Anonymise], [Mandatory], [MaxLength], [RegEx], [FieldType_Id]) 
	VALUES (0, 0, 5, NULL, 3)
set @fid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetElement] ([ElementName], [Field_Id], [DatasetElementType_Id], [OID], [DefaultValue], [System], [DatasetElementGuid]) 
	VALUES (N'Receiver Tel Extension', @fid, 1, NULL, NULL, 0, NEWID())
set @deid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetCategoryElement] ([FieldOrder], [DatasetCategory_Id], [DatasetElement_Id], [Acute], [Chronic]) 
	select ISNULL(MAX(FieldOrder) + 1, 1), @dscid, @deid, 0, 0 from [DatasetCategoryElement] where [DatasetCategory_Id] = @dscid
-- element node
INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElement_Id, UpdatedBy_Id, DatasetXml_Id)
	VALUES ('receivertelextension', 2, NULL, GETDATE(), GETDATE(), @dsxcnid2, 1, @deid, 1, @dsxid)
set @dsxenid = (SELECT @@IDENTITY)

-- receivertelcountrycode
INSERT [dbo].[Field] ([Anonymise], [Mandatory], [MaxLength], [RegEx], [FieldType_Id]) 
	VALUES (0, 0, 3, NULL, 3)
set @fid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetElement] ([ElementName], [Field_Id], [DatasetElementType_Id], [OID], [DefaultValue], [System], [DatasetElementGuid]) 
	VALUES (N'Receiver Tel Country Code', @fid, 1, NULL, NULL, 0, NEWID())
set @deid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetCategoryElement] ([FieldOrder], [DatasetCategory_Id], [DatasetElement_Id], [Acute], [Chronic]) 
	select ISNULL(MAX(FieldOrder) + 1, 1), @dscid, @deid, 0, 0 from [DatasetCategoryElement] where [DatasetCategory_Id] = @dscid
-- element node
INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElement_Id, UpdatedBy_Id, DatasetXml_Id)
	VALUES ('receivertelcountrycode', 2, NULL, GETDATE(), GETDATE(), @dsxcnid2, 1, @deid, 1, @dsxid)
set @dsxenid = (SELECT @@IDENTITY)

-- receiverfax
INSERT [dbo].[Field] ([Anonymise], [Mandatory], [MaxLength], [RegEx], [FieldType_Id]) 
	VALUES (0, 0, 10, NULL, 3)
set @fid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetElement] ([ElementName], [Field_Id], [DatasetElementType_Id], [OID], [DefaultValue], [System], [DatasetElementGuid]) 
	VALUES (N'Receiver Fax', @fid, 1, NULL, NULL, 0, NEWID())
set @deid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetCategoryElement] ([FieldOrder], [DatasetCategory_Id], [DatasetElement_Id], [Acute], [Chronic]) 
	select ISNULL(MAX(FieldOrder) + 1, 1), @dscid, @deid, 0, 0 from [DatasetCategoryElement] where [DatasetCategory_Id] = @dscid
-- element node
INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElement_Id, UpdatedBy_Id, DatasetXml_Id)
	VALUES ('receiverfax', 2, NULL, GETDATE(), GETDATE(), @dsxcnid2, 1, @deid, 1, @dsxid)
set @dsxenid = (SELECT @@IDENTITY)

-- receiverfaxextension
INSERT [dbo].[Field] ([Anonymise], [Mandatory], [MaxLength], [RegEx], [FieldType_Id]) 
	VALUES (0, 0, 5, NULL, 3)
set @fid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetElement] ([ElementName], [Field_Id], [DatasetElementType_Id], [OID], [DefaultValue], [System], [DatasetElementGuid]) 
	VALUES (N'Receiver Fax Extension', @fid, 1, NULL, NULL, 0, NEWID())
set @deid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetCategoryElement] ([FieldOrder], [DatasetCategory_Id], [DatasetElement_Id], [Acute], [Chronic]) 
	select ISNULL(MAX(FieldOrder) + 1, 1), @dscid, @deid, 0, 0 from [DatasetCategoryElement] where [DatasetCategory_Id] = @dscid
-- element node
INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElement_Id, UpdatedBy_Id, DatasetXml_Id)
	VALUES ('receiverfaxextension', 2, NULL, GETDATE(), GETDATE(), @dsxcnid2, 1, @deid, 1, @dsxid)
set @dsxenid = (SELECT @@IDENTITY)

-- receiverfaxcountrycode
INSERT [dbo].[Field] ([Anonymise], [Mandatory], [MaxLength], [RegEx], [FieldType_Id]) 
	VALUES (0, 0, 3, NULL, 3)
set @fid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetElement] ([ElementName], [Field_Id], [DatasetElementType_Id], [OID], [DefaultValue], [System], [DatasetElementGuid]) 
	VALUES (N'Receiver Fax Country Code', @fid, 1, NULL, NULL, 0, NEWID())
set @deid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetCategoryElement] ([FieldOrder], [DatasetCategory_Id], [DatasetElement_Id], [Acute], [Chronic]) 
	select ISNULL(MAX(FieldOrder) + 1, 1), @dscid, @deid, 0, 0 from [DatasetCategoryElement] where [DatasetCategory_Id] = @dscid
-- element node
INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElement_Id, UpdatedBy_Id, DatasetXml_Id)
	VALUES ('receiverfaxcountrycode', 2, NULL, GETDATE(), GETDATE(), @dsxcnid2, 1, @deid, 1, @dsxid)
set @dsxenid = (SELECT @@IDENTITY)

-- receiveremailaddress
INSERT [dbo].[Field] ([Anonymise], [Mandatory], [MaxLength], [RegEx], [FieldType_Id]) 
	VALUES (0, 0, 100, NULL, 3)
set @fid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetElement] ([ElementName], [Field_Id], [DatasetElementType_Id], [OID], [DefaultValue], [System], [DatasetElementGuid]) 
	VALUES (N'Receiver Email Address', @fid, 1, NULL, NULL, 0, NEWID())
set @deid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetCategoryElement] ([FieldOrder], [DatasetCategory_Id], [DatasetElement_Id], [Acute], [Chronic]) 
	select ISNULL(MAX(FieldOrder) + 1, 1), @dscid, @deid, 0, 0 from [DatasetCategoryElement] where [DatasetCategory_Id] = @dscid
-- element node
INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElement_Id, UpdatedBy_Id, DatasetXml_Id)
	VALUES ('receiveremailaddress', 2, NULL, GETDATE(), GETDATE(), @dsxcnid2, 1, @deid, 1, @dsxid)
set @dsxenid = (SELECT @@IDENTITY)

/**************************************************
CATEGORY patient
**************************************************/
INSERT [dbo].[DatasetCategory] ([DatasetCategoryName], [CategoryOrder], [Dataset_Id]) 
	VALUES (N'Patient', 1, @dsid)
set @dscid = (SELECT @@IDENTITY)

-- category node
INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElement_Id, UpdatedBy_Id, DatasetXml_Id)
	VALUES ('patient', 2, NULL, GETDATE(), GETDATE(), @dsxcnid, 1, NULL, 1, @dsxid)
set @dsxcnid2 = (SELECT @@IDENTITY)

-- patientinitial
INSERT [dbo].[Field] ([Anonymise], [Mandatory], [MaxLength], [RegEx], [FieldType_Id]) 
	VALUES (0, 0, 10, NULL, 3)
set @fid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetElement] ([ElementName], [Field_Id], [DatasetElementType_Id], [OID], [DefaultValue], [System], [DatasetElementGuid]) 
	VALUES (N'Patient Initial', @fid, 1, NULL, NULL, 0, NEWID())
set @deid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetCategoryElement] ([FieldOrder], [DatasetCategory_Id], [DatasetElement_Id], [Acute], [Chronic]) 
	select ISNULL(MAX(FieldOrder) + 1, 1), @dscid, @deid, 0, 0 from [DatasetCategoryElement] where [DatasetCategory_Id] = @dscid
set @dceid = (SELECT @@IDENTITY)
-- element node
INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElement_Id, UpdatedBy_Id, DatasetXml_Id)
	VALUES ('patientinitial', 2, NULL, GETDATE(), GETDATE(), @dsxcnid2, 1, @deid, 1, @dsxid)
set @dsxenid = (SELECT @@IDENTITY)
-- mapping
INSERT [dbo].[DatasetMapping] (Tag, MappingType, MappingOption, DestinationElement_Id, SourceElement_Id)
	VALUES ('Spontaneous', 0, '', @dceid, (select top 1 dce.Id from [dbo].[DatasetCategoryElement] dce inner join [dbo].[DatasetElement] de on dce.DatasetElement_Id = de.Id where de.ElementName = 'Initials'))

-- patientgpmedicalrecordnumb
INSERT [dbo].[Field] ([Anonymise], [Mandatory], [MaxLength], [RegEx], [FieldType_Id]) 
	VALUES (0, 0, 20, NULL, 3)
set @fid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetElement] ([ElementName], [Field_Id], [DatasetElementType_Id], [OID], [DefaultValue], [System], [DatasetElementGuid]) 
	VALUES (N'Patient GP Medical Record Number', @fid, 1, NULL, NULL, 0, NEWID())
set @deid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetCategoryElement] ([FieldOrder], [DatasetCategory_Id], [DatasetElement_Id], [Acute], [Chronic]) 
	select ISNULL(MAX(FieldOrder) + 1, 1), @dscid, @deid, 0, 0 from [DatasetCategoryElement] where [DatasetCategory_Id] = @dscid
set @dceid = (SELECT @@IDENTITY)
-- element node
INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElement_Id, UpdatedBy_Id, DatasetXml_Id)
	VALUES ('patientgpmedicalrecordnumb', 2, NULL, GETDATE(), GETDATE(), @dsxcnid2, 1, @deid, 1, @dsxid)
set @dsxenid = (SELECT @@IDENTITY)
-- mapping
INSERT [dbo].[DatasetMapping] (Tag, MappingType, MappingOption, DestinationElement_Id, PropertyPath, Property)
	VALUES ('Active', 2, '', @dceid, 'Patient', 'Medical Record Number')

-- patientspecialistrecordnumb
INSERT [dbo].[Field] ([Anonymise], [Mandatory], [MaxLength], [RegEx], [FieldType_Id]) 
	VALUES (0, 0, 20, NULL, 3)
set @fid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetElement] ([ElementName], [Field_Id], [DatasetElementType_Id], [OID], [DefaultValue], [System], [DatasetElementGuid]) 
	VALUES (N'Patient Specialist Record Number', @fid, 1, NULL, NULL, 0, NEWID())
set @deid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetCategoryElement] ([FieldOrder], [DatasetCategory_Id], [DatasetElement_Id], [Acute], [Chronic]) 
	select ISNULL(MAX(FieldOrder) + 1, 1), @dscid, @deid, 0, 0 from [DatasetCategoryElement] where [DatasetCategory_Id] = @dscid
-- element node
INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElement_Id, UpdatedBy_Id, DatasetXml_Id)
	VALUES ('patientspecialistrecordnumb', 2, NULL, GETDATE(), GETDATE(), @dsxcnid2, 1, @deid, 1, @dsxid)
set @dsxenid = (SELECT @@IDENTITY)

INSERT [dbo].[Field] ([Anonymise], [Mandatory], [MaxLength], [RegEx], [FieldType_Id]) 
	VALUES (0, 0, 20, NULL, 3)
set @fid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetElement] ([ElementName], [Field_Id], [DatasetElementType_Id], [OID], [DefaultValue], [System], [DatasetElementGuid]) 
	VALUES ('Patient Hospital Record Number', @fid, 1, NULL, NULL, 0, NEWID())
set @deid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetCategoryElement] ([FieldOrder], [DatasetCategory_Id], [DatasetElement_Id], [Acute], [Chronic]) 
	select ISNULL(MAX(FieldOrder) + 1, 1), @dscid, @deid, 0, 0 from [DatasetCategoryElement] where [DatasetCategory_Id] = @dscid
-- element node
INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElement_Id, UpdatedBy_Id, DatasetXml_Id)
	VALUES ('patienthospitalrecordnumb', 2, NULL, GETDATE(), GETDATE(), @dsxcnid2, 1, @deid, 1, @dsxid)
set @dsxenid = (SELECT @@IDENTITY)

INSERT [dbo].[Field] ([Anonymise], [Mandatory], [MaxLength], [RegEx], [FieldType_Id]) 
	VALUES (0, 0, 20, NULL, 3)
set @fid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetElement] ([ElementName], [Field_Id], [DatasetElementType_Id], [OID], [DefaultValue], [System], [DatasetElementGuid]) 
	VALUES ('Patient Investigation Number', @fid, 1, NULL, NULL, 0, NEWID())
set @deid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetCategoryElement] ([FieldOrder], [DatasetCategory_Id], [DatasetElement_Id], [Acute], [Chronic]) 
	select ISNULL(MAX(FieldOrder) + 1, 1), @dscid, @deid, 0, 0 from [DatasetCategoryElement] where [DatasetCategory_Id] = @dscid
-- element node
INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElement_Id, UpdatedBy_Id, DatasetXml_Id)
	VALUES ('patientinvestigationnumb', 2, NULL, GETDATE(), GETDATE(), @dsxcnid2, 1, @deid, 1, @dsxid)
set @dsxenid = (SELECT @@IDENTITY)

INSERT [dbo].[Field] ([Anonymise], [Mandatory], [MaxLength], [RegEx], [FieldType_Id]) 
	VALUES (0, 0, 10, NULL, 8)
set @fid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetElement] ([ElementName], [Field_Id], [DatasetElementType_Id], [OID], [DefaultValue], [System], [DatasetElementGuid]) 
	VALUES ('Patient Birthdate Format', @fid, 1, NULL, '102', 1, NEWID())
set @deid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetCategoryElement] ([FieldOrder], [DatasetCategory_Id], [DatasetElement_Id], [Acute], [Chronic]) 
	select ISNULL(MAX(FieldOrder) + 1, 1), @dscid, @deid, 0, 0 from [DatasetCategoryElement] where [DatasetCategory_Id] = @dscid
-- element node
INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElement_Id, UpdatedBy_Id, DatasetXml_Id)
	VALUES ('patientbirthdateformat', 2, NULL, GETDATE(), GETDATE(), @dsxcnid2, 1, @deid, 1, @dsxid)
set @dsxenid = (SELECT @@IDENTITY)

INSERT [dbo].[Field] ([Anonymise], [Mandatory], [FieldType_Id]) 
	VALUES (0, 0, 6)
set @fid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetElement] ([ElementName], [Field_Id], [DatasetElementType_Id], [OID], [DefaultValue], [System], [DatasetElementGuid]) 
	VALUES ('Patient Birthdate', @fid, 1, NULL, NULL, 0, NEWID())
set @deid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetCategoryElement] ([FieldOrder], [DatasetCategory_Id], [DatasetElement_Id], [Acute], [Chronic]) 
	select ISNULL(MAX(FieldOrder) + 1, 1), @dscid, @deid, 0, 0 from [DatasetCategoryElement] where [DatasetCategory_Id] = @dscid
-- element node
INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElement_Id, UpdatedBy_Id, DatasetXml_Id)
	VALUES ('patientbirthdate', 2, NULL, GETDATE(), GETDATE(), @dsxcnid2, 1, @deid, 1, @dsxid)
set @dsxenid = (SELECT @@IDENTITY)

INSERT [dbo].[Field] ([Anonymise], [Mandatory], [Decimals], [MaxSize], [MinSize], [FieldType_Id]) 
	VALUES (0, 0, 0, 999999, 1, 4)
set @fid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetElement] ([ElementName], [Field_Id], [DatasetElementType_Id], [OID], [DefaultValue], [System], [DatasetElementGuid]) 
	VALUES ('Patient Onset Age', @fid, 1, NULL, NULL, 0, NEWID())
set @deid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetCategoryElement] ([FieldOrder], [DatasetCategory_Id], [DatasetElement_Id], [Acute], [Chronic]) 
	select ISNULL(MAX(FieldOrder) + 1, 1), @dscid, @deid, 0, 0 from [DatasetCategoryElement] where [DatasetCategory_Id] = @dscid
set @dceid = (SELECT @@IDENTITY)
-- element node
INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElement_Id, UpdatedBy_Id, DatasetXml_Id)
	VALUES ('patientonsetage', 2, NULL, GETDATE(), GETDATE(), @dsxcnid2, 1, @deid, 1, @dsxid)
set @dsxenid = (SELECT @@IDENTITY)
-- mapping
INSERT [dbo].[DatasetMapping] (Tag, MappingType, MappingOption, DestinationElement_Id, SourceElement_Id)
	VALUES ('Spontaneous', 0, '', @dceid, (select top 1 dce.Id from [dbo].[DatasetCategoryElement] dce inner join [dbo].[DatasetElement] de on dce.DatasetElement_Id = de.Id where de.ElementName = 'Age'))

INSERT [dbo].[Field] ([Anonymise], [Mandatory], [FieldType_Id]) 
	VALUES (0, 0, 2)
set @fid = (SELECT @@IDENTITY)
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
	VALUES ('800=Decade', 1, 0, 0, @fid)
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
	VALUES ('801=Year', 0, 0, 0, @fid)
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
	VALUES ('802=Month', 0, 0, 0, @fid)
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
	VALUES ('803=Week', 0, 0, 0, @fid)
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
	VALUES ('804=Day', 0, 0, 0, @fid)
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
	VALUES ('805=Hour', 0, 1, 0, @fid)
INSERT [dbo].[DatasetElement] ([ElementName], [Field_Id], [DatasetElementType_Id], [OID], [DefaultValue], [System], [DatasetElementGuid]) 
	VALUES ('Patient Onset Age Unit', @fid, 1, NULL, NULL, 0, NEWID())
set @deid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetCategoryElement] ([FieldOrder], [DatasetCategory_Id], [DatasetElement_Id], [Acute], [Chronic]) 
	select ISNULL(MAX(FieldOrder) + 1, 1), @dscid, @deid, 0, 0 from [DatasetCategoryElement] where [DatasetCategory_Id] = @dscid
set @dceid = (SELECT @@IDENTITY)
-- element node
INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElement_Id, UpdatedBy_Id, DatasetXml_Id)
	VALUES ('patientonsetageunit', 2, NULL, GETDATE(), GETDATE(), @dsxcnid2, 1, @deid, 1, @dsxid)
set @dsxenid = (SELECT @@IDENTITY)
-- mapping
INSERT [dbo].[DatasetMapping] (Tag, MappingType, MappingOption, DestinationElement_Id, SourceElement_Id)
	VALUES ('Spontaneous', 1, '', @dceid, (select top 1 dce.Id from [dbo].[DatasetCategoryElement] dce inner join [dbo].[DatasetElement] de on dce.DatasetElement_Id = de.Id where de.ElementName = 'Age Unit'))
set @dmid = (SELECT @@IDENTITY)
-- mapping values
INSERT [dbo].[DatasetMappingValue] (SourceValue, DestinationValue, Active, Mapping_Id)
	SELECT 'Years', '801=Year', 1, @dmid UNION
	SELECT 'Months', '802=Month', 1, @dmid UNION
	SELECT 'Weeks', '803=Week', 1, @dmid UNION
	SELECT 'Days', '804=Day', 1, @dmid

INSERT [dbo].[Field] ([Anonymise], [Mandatory], [Decimals], [MaxSize], [MinSize], [FieldType_Id]) 
	VALUES (0, 0, 0, 50, 1, 4)
set @fid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetElement] ([ElementName], [Field_Id], [DatasetElementType_Id], [OID], [DefaultValue], [System], [DatasetElementGuid]) 
	VALUES ('Gestation Period', @fid, 1, NULL, NULL, 0, NEWID())
set @deid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetCategoryElement] ([FieldOrder], [DatasetCategory_Id], [DatasetElement_Id], [Acute], [Chronic]) 
	select ISNULL(MAX(FieldOrder) + 1, 1), @dscid, @deid, 0, 0 from [DatasetCategoryElement] where [DatasetCategory_Id] = @dscid
-- element node
INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElement_Id, UpdatedBy_Id, DatasetXml_Id)
	VALUES ('gestationperiod', 2, NULL, GETDATE(), GETDATE(), @dsxcnid2, 1, @deid, 1, @dsxid)
set @dsxenid = (SELECT @@IDENTITY)

INSERT [dbo].[Field] ([Anonymise], [Mandatory], [FieldType_Id]) 
	VALUES (0, 0, 2)
set @fid = (SELECT @@IDENTITY)
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
	VALUES ('802=Month', 1, 0, 0, @fid)
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
	VALUES ('803=Week', 0, 0, 0, @fid)
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
	VALUES ('804=Day', 0, 0, 0, @fid)
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
	VALUES ('810=Trimester', 0, 0, 0, @fid)
INSERT [dbo].[DatasetElement] ([ElementName], [Field_Id], [DatasetElementType_Id], [OID], [DefaultValue], [System], [DatasetElementGuid]) 
	VALUES ('Gestation Period Unit', @fid, 1, NULL, NULL, 0, NEWID())
set @deid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetCategoryElement] ([FieldOrder], [DatasetCategory_Id], [DatasetElement_Id], [Acute], [Chronic]) 
	select ISNULL(MAX(FieldOrder) + 1, 1), @dscid, @deid, 0, 0 from [DatasetCategoryElement] where [DatasetCategory_Id] = @dscid
-- element node
INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElement_Id, UpdatedBy_Id, DatasetXml_Id)
	VALUES ('gestationperiodunit', 2, NULL, GETDATE(), GETDATE(), @dsxcnid2, 1, @deid, 1, @dsxid)
set @dsxenid = (SELECT @@IDENTITY)

INSERT [dbo].[Field] ([Anonymise], [Mandatory], [FieldType_Id]) 
	VALUES (0, 0, 2)
set @fid = (SELECT @@IDENTITY)
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
	VALUES ('1=Neonate', 1, 0, 0, @fid)
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
	VALUES ('2=Infant', 0, 0, 0, @fid)
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
	VALUES ('3=Child', 0, 0, 0, @fid)
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
	VALUES ('4=Adolescent', 0, 0, 0, @fid)
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
	VALUES ('5=Adult', 0, 0, 0, @fid)
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
	VALUES ('6=Elderly', 0, 0, 0, @fid)
INSERT [dbo].[DatasetElement] ([ElementName], [Field_Id], [DatasetElementType_Id], [OID], [DefaultValue], [System], [DatasetElementGuid]) 
	VALUES ('Patient Age Group', @fid, 1, NULL, NULL, 0, NEWID())
set @deid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetCategoryElement] ([FieldOrder], [DatasetCategory_Id], [DatasetElement_Id], [Acute], [Chronic]) 
	select ISNULL(MAX(FieldOrder) + 1, 1), @dscid, @deid, 0, 0 from [DatasetCategoryElement] where [DatasetCategory_Id] = @dscid
-- element node
INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElement_Id, UpdatedBy_Id, DatasetXml_Id)
	VALUES ('patientagegroup', 2, NULL, GETDATE(), GETDATE(), @dsxcnid2, 1, @deid, 1, @dsxid)
set @dsxenid = (SELECT @@IDENTITY)

INSERT [dbo].[Field] ([Anonymise], [Mandatory], [Decimals], [MaxSize], [MinSize], [FieldType_Id]) 
	VALUES (0, 0, 1, 200.0, 1.0, 4)
set @fid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetElement] ([ElementName], [Field_Id], [DatasetElementType_Id], [OID], [DefaultValue], [System], [DatasetElementGuid]) 
	VALUES ('Patient Weight', @fid, 1, NULL, NULL, 0, NEWID())
set @deid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetCategoryElement] ([FieldOrder], [DatasetCategory_Id], [DatasetElement_Id], [Acute], [Chronic]) 
	select ISNULL(MAX(FieldOrder) + 1, 1), @dscid, @deid, 0, 0 from [DatasetCategoryElement] where [DatasetCategory_Id] = @dscid
set @dceid = (SELECT @@IDENTITY)
-- element node
INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElement_Id, UpdatedBy_Id, DatasetXml_Id)
	VALUES ('patientweight', 2, NULL, GETDATE(), GETDATE(), @dsxcnid2, 1, @deid, 1, @dsxid)
set @dsxenid = (SELECT @@IDENTITY)
-- mapping
INSERT [dbo].[DatasetMapping] (Tag, MappingType, MappingOption, DestinationElement_Id, SourceElement_Id)
	VALUES ('Spontaneous', 0, '', @dceid, (select top 1 dce.Id from [dbo].[DatasetCategoryElement] dce inner join [dbo].[DatasetElement] de on dce.DatasetElement_Id = de.Id inner join dbo.[DatasetCategory] dc on dce.DatasetCategory_Id = dc.Id where dc.DatasetCategoryName = 'Patient Information' and de.ElementName = 'Weight (kg)'))

INSERT [dbo].[Field] ([Anonymise], [Mandatory], [Decimals], [MaxSize], [MinSize], [FieldType_Id]) 
	VALUES (0, 0, 0, 300, 1, 4)
set @fid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetElement] ([ElementName], [Field_Id], [DatasetElementType_Id], [OID], [DefaultValue], [System], [DatasetElementGuid]) 
	VALUES ('Patient Height', @fid, 1, NULL, NULL, 0, NEWID())
set @deid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetCategoryElement] ([FieldOrder], [DatasetCategory_Id], [DatasetElement_Id], [Acute], [Chronic]) 
	select ISNULL(MAX(FieldOrder) + 1, 1), @dscid, @deid, 0, 0 from [DatasetCategoryElement] where [DatasetCategory_Id] = @dscid
-- element node
INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElement_Id, UpdatedBy_Id, DatasetXml_Id)
	VALUES ('patientheight', 2, NULL, GETDATE(), GETDATE(), @dsxcnid2, 1, @deid, 1, @dsxid)
set @dsxenid = (SELECT @@IDENTITY)

INSERT [dbo].[Field] ([Anonymise], [Mandatory], [FieldType_Id]) 
	VALUES (0, 0, 2)
set @fid = (SELECT @@IDENTITY)
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
	VALUES ('1=Male', 1, 0, 0, @fid)
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
	VALUES ('2=Female', 0, 0, 0, @fid)
INSERT [dbo].[DatasetElement] ([ElementName], [Field_Id], [DatasetElementType_Id], [OID], [DefaultValue], [System], [DatasetElementGuid]) 
	VALUES ('Patient Sex', @fid, 1, NULL, NULL, 0, NEWID())
set @deid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetCategoryElement] ([FieldOrder], [DatasetCategory_Id], [DatasetElement_Id], [Acute], [Chronic]) 
	select ISNULL(MAX(FieldOrder) + 1, 1), @dscid, @deid, 0, 0 from [DatasetCategoryElement] where [DatasetCategory_Id] = @dscid
set @dceid = (SELECT @@IDENTITY)
-- element node
INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElement_Id, UpdatedBy_Id, DatasetXml_Id)
	VALUES ('patientsex', 2, NULL, GETDATE(), GETDATE(), @dsxcnid2, 1, @deid, 1, @dsxid)
set @dsxenid = (SELECT @@IDENTITY)
-- mapping 
INSERT [dbo].[DatasetMapping] (Tag, MappingType, MappingOption, DestinationElement_Id, SourceElement_Id)
	VALUES ('Spontaneous', 1, '', @dceid, (select top 1 dce.Id from [dbo].[DatasetCategoryElement] dce inner join [dbo].[DatasetElement] de on dce.DatasetElement_Id = de.Id where de.ElementName = 'Sex'))
set @dmid = (SELECT @@IDENTITY)
-- mapping values
INSERT [dbo].[DatasetMappingValue] (SourceValue, DestinationValue, Active, Mapping_Id)
	SELECT 'Male', '1=Male', 1, @dmid UNION
	SELECT 'Female', '2=Female', 1, @dmid
-- mapping 
INSERT [dbo].[DatasetMapping] (Tag, MappingType, MappingOption, DestinationElement_Id, PropertyPath, Property)
	VALUES ('Active', 3, '', @dceid, 'Patient', 'Gender')
set @dmid = (SELECT @@IDENTITY)
-- mapping values
INSERT [dbo].[DatasetMappingValue] (SourceValue, DestinationValue, Active, Mapping_Id)
	SELECT '1', '1=Male', 1, @dmid UNION
	SELECT '2', '2=Female', 1, @dmid

INSERT [dbo].[Field] ([Anonymise], [Mandatory], [MaxLength], [RegEx], [FieldType_Id]) 
	VALUES (0, 0, 10, NULL, 8)
set @fid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetElement] ([ElementName], [Field_Id], [DatasetElementType_Id], [OID], [DefaultValue], [System], [DatasetElementGuid]) 
	VALUES ('Last Menstrual Date Format', @fid, 1, NULL, '102', 1, NEWID())
set @deid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetCategoryElement] ([FieldOrder], [DatasetCategory_Id], [DatasetElement_Id], [Acute], [Chronic]) 
	select ISNULL(MAX(FieldOrder) + 1, 1), @dscid, @deid, 0, 0 from [DatasetCategoryElement] where [DatasetCategory_Id] = @dscid
-- element node
INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElement_Id, UpdatedBy_Id, DatasetXml_Id)
	VALUES ('lastmenstrualdateformat', 2, NULL, GETDATE(), GETDATE(), @dsxcnid2, 1, @deid, 1, @dsxid)
set @dsxenid = (SELECT @@IDENTITY)

INSERT [dbo].[Field] ([Anonymise], [Mandatory], [FieldType_Id]) 
	VALUES (0, 0, 6)
set @fid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetElement] ([ElementName], [Field_Id], [DatasetElementType_Id], [OID], [DefaultValue], [System], [DatasetElementGuid]) 
	VALUES ('Patient Last Menstrual Date', @fid, 1, NULL, NULL, 0, NEWID())
set @deid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetCategoryElement] ([FieldOrder], [DatasetCategory_Id], [DatasetElement_Id], [Acute], [Chronic]) 
	select ISNULL(MAX(FieldOrder) + 1, 1), @dscid, @deid, 0, 0 from [DatasetCategoryElement] where [DatasetCategory_Id] = @dscid
-- element node
INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElement_Id, UpdatedBy_Id, DatasetXml_Id)
	VALUES ('patientlastmenstrualdate', 2, NULL, GETDATE(), GETDATE(), @dsxcnid2, 1, @deid, 1, @dsxid)
set @dsxenid = (SELECT @@IDENTITY)

INSERT [dbo].[Field] ([Anonymise], [Mandatory], [MaxLength], [RegEx], [FieldType_Id]) 
	VALUES (0, 0, 500, NULL, 3)
set @fid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetElement] ([ElementName], [Field_Id], [DatasetElementType_Id], [OID], [DefaultValue], [System], [DatasetElementGuid]) 
	VALUES ('Patient Medical History Text', @fid, 1, NULL, NULL, 0, NEWID())
set @deid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetCategoryElement] ([FieldOrder], [DatasetCategory_Id], [DatasetElement_Id], [Acute], [Chronic]) 
	select ISNULL(MAX(FieldOrder) + 1, 1), @dscid, @deid, 0, 0 from [DatasetCategoryElement] where [DatasetCategory_Id] = @dscid
-- element node
INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElement_Id, UpdatedBy_Id, DatasetXml_Id)
	VALUES ('patientmedicalhistorytext', 2, NULL, GETDATE(), GETDATE(), @dsxcnid2, 1, @deid, 1, @dsxid)
set @dsxenid = (SELECT @@IDENTITY)

INSERT [dbo].[Field] ([Anonymise], [Mandatory], [MaxLength], [RegEx], [FieldType_Id]) 
	VALUES (0, 0, 500, NULL, 3)
set @fid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetElement] ([ElementName], [Field_Id], [DatasetElementType_Id], [OID], [DefaultValue], [System], [DatasetElementGuid]) 
	VALUES ('Results Tests Procedures', @fid, 1, NULL, NULL, 0, NEWID())
set @deid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetCategoryElement] ([FieldOrder], [DatasetCategory_Id], [DatasetElement_Id], [Acute], [Chronic]) 
	select ISNULL(MAX(FieldOrder) + 1, 1), @dscid, @deid, 0, 0 from [DatasetCategoryElement] where [DatasetCategory_Id] = @dscid
-- element node
INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElement_Id, UpdatedBy_Id, DatasetXml_Id)
	VALUES ('resultstestsprocedures', 2, NULL, GETDATE(), GETDATE(), @dsxcnid2, 1, @deid, 1, @dsxid)
set @dsxenid = (SELECT @@IDENTITY)

/**************************************************
CATEGORY medicalhistoryepisode
**************************************************/
INSERT [dbo].[DatasetCategory] ([DatasetCategoryName], [CategoryOrder], [Dataset_Id]) 
	VALUES (N'Medical History Episode', 1, @dsid)
set @dscid = (SELECT @@IDENTITY)

-- Medical History Episode Table
INSERT [dbo].[Field] (Mandatory, MaxLength, Decimals, MaxSize, MinSize, Calculation, FileSize, FileExt, Anonymise, FieldType_Id)
	VALUES (0, NULL, NULL, NULL, NULL, '', NULL, '', 0, 7) 
set @fid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetElement] ([ElementName], [Field_Id], [DatasetElementType_Id], [OID], [DefaultValue], [System])
	VALUES ('Medical History Episodes', @fid, 1, '', '', 0) 
set @deid = (SELECT @@IDENTITY)
-- category node
INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElement_Id, UpdatedBy_Id, DatasetXml_Id)
	VALUES ('medicalhistoryepisode', 3, NULL, GETDATE(), GETDATE(), @dsxcnid2, 1, @deid, 1, @dsxid)
set @dsxcnid3 = (SELECT @@IDENTITY)

INSERT [dbo].[DatasetCategoryElement] ([FieldOrder], [DatasetCategory_Id], [DatasetElement_Id], [Acute], [Chronic], [FriendlyName], [Help])
	VALUES (10, @dscid, @deid, 0, 0, 'Medical History Episodes', 'Medical History Episodes') 
set @dceid = (SELECT @@IDENTITY)

		INSERT [dbo].[Field] ([Anonymise], [Mandatory], [MaxLength], [RegEx], [FieldType_Id]) 
			VALUES (0, 0, 10, NULL, 8)
		set @fid = (SELECT @@IDENTITY)
		INSERT [dbo].[DatasetElementSub] ([ElementName], [Field_Id], [DatasetElement_Id], [OID], [DefaultValue], [System], FieldOrder)
			VALUES ('Patient Episode Name MedDRA Version', @fid, @deid, '', 'v20', 1, 1)
		set @desid = (SELECT @@IDENTITY)
		-- element node
		INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElementSub_Id, UpdatedBy_Id, DatasetXml_Id)
			VALUES ('patientepisodenamemeddraversion', 3, NULL, GETDATE(), GETDATE(), @dsxcnid3, 1, @desid, 1, @dsxid)
		set @dsxenid = (SELECT @@IDENTITY)

		INSERT [dbo].[Field] ([Anonymise], [Mandatory], [MaxLength], [RegEx], [FieldType_Id]) 
			VALUES (0, 0, 250, NULL, 3)
		set @fid = (SELECT @@IDENTITY)
		INSERT [dbo].[DatasetElementSub] ([ElementName], [Field_Id], [DatasetElement_Id], [OID], [DefaultValue], [System], FieldOrder)
			VALUES ('Patient Episode Name', @fid, @deid, '', '', 0, 2)
		set @desid = (SELECT @@IDENTITY)
		-- element node
		INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElementSub_Id, UpdatedBy_Id, DatasetXml_Id)
			VALUES ('patientepisodename', 3, NULL, GETDATE(), GETDATE(), @dsxcnid3, 1, @desid, 1, @dsxid)
		set @dsxenid = (SELECT @@IDENTITY)

		INSERT [dbo].[Field] ([Anonymise], [Mandatory], [MaxLength], [RegEx], [FieldType_Id]) 
			VALUES (0, 0, 10, NULL, 8)
		set @fid = (SELECT @@IDENTITY)
		INSERT [dbo].[DatasetElementSub] ([ElementName], [Field_Id], [DatasetElement_Id], [OID], [DefaultValue], [System], FieldOrder)
			VALUES ('Patient Medical Start Date Format', @fid, @deid, '', '102', 1, 3)
		set @desid = (SELECT @@IDENTITY)
		-- element node
		INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElementSub_Id, UpdatedBy_Id, DatasetXml_Id)
			VALUES ('patientmedicalstartdateformat', 3, NULL, GETDATE(), GETDATE(), @dsxcnid3, 1, @desid, 1, @dsxid)
		set @dsxenid = (SELECT @@IDENTITY)

		INSERT [dbo].[Field] ([Anonymise], [Mandatory], [FieldType_Id]) 
			VALUES (0, 0, 6)
		set @fid = (SELECT @@IDENTITY)
		INSERT [dbo].[DatasetElementSub] ([ElementName], [Field_Id], [DatasetElement_Id], [OID], [DefaultValue], [System], FieldOrder)
			VALUES ('Patient Medical Start Date', @fid, @deid, '', '', 0, 4)
		set @desid = (SELECT @@IDENTITY)
		-- element node
		INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElementSub_Id, UpdatedBy_Id, DatasetXml_Id)
			VALUES ('patientmedicalstartdate', 3, NULL, GETDATE(), GETDATE(), @dsxcnid3, 1, @desid, 1, @dsxid)
		set @dsxenid = (SELECT @@IDENTITY)

		INSERT [dbo].[Field] ([Anonymise], [Mandatory], [FieldType_Id]) 
			VALUES (0, 0, 2)
		set @fid = (SELECT @@IDENTITY)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('1=Yes', 1, 0, 0, @fid)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('2=No', 0, 0, 0, @fid)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('3=Unknown', 0, 0, 0, @fid)
		INSERT [dbo].[DatasetElementSub] ([ElementName], [Field_Id], [DatasetElement_Id], [OID], [DefaultValue], [System], FieldOrder)
			VALUES ('Patient Medical Continue', @fid, @deid, '', '', 0, 5)
		set @desid = (SELECT @@IDENTITY)
		-- element node
		INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElementSub_Id, UpdatedBy_Id, DatasetXml_Id)
			VALUES ('patientmedicalcontinue', 3, NULL, GETDATE(), GETDATE(), @dsxcnid3, 1, @desid, 1, @dsxid)
		set @dsxenid = (SELECT @@IDENTITY)

		INSERT [dbo].[Field] ([Anonymise], [Mandatory], [MaxLength], [RegEx], [FieldType_Id]) 
			VALUES (0, 0, 10, NULL, 8)
		set @fid = (SELECT @@IDENTITY)
		INSERT [dbo].[DatasetElementSub] ([ElementName], [Field_Id], [DatasetElement_Id], [OID], [DefaultValue], [System], FieldOrder)
			VALUES ('Patient Medical End Date Format', @fid, @deid, '', '102', 1, 6)
		set @desid = (SELECT @@IDENTITY)
		-- element node
		INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElementSub_Id, UpdatedBy_Id, DatasetXml_Id)
			VALUES ('patientmedicalenddateformat', 3, NULL, GETDATE(), GETDATE(), @dsxcnid3, 1, @desid, 1, @dsxid)
		set @dsxenid = (SELECT @@IDENTITY)

		INSERT [dbo].[Field] ([Anonymise], [Mandatory], [FieldType_Id]) 
			VALUES (0, 0, 6)
		set @fid = (SELECT @@IDENTITY)
		INSERT [dbo].[DatasetElementSub] ([ElementName], [Field_Id], [DatasetElement_Id], [OID], [DefaultValue], [System], FieldOrder)
			VALUES ('Patient Medical End Date', @fid, @deid, '', '', 0, 7)
		set @desid = (SELECT @@IDENTITY)
		-- element node
		INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElementSub_Id, UpdatedBy_Id, DatasetXml_Id)
			VALUES ('patientmedicalenddate', 3, NULL, GETDATE(), GETDATE(), @dsxcnid3, 1, @desid, 1, @dsxid)
		set @dsxenid = (SELECT @@IDENTITY)

		INSERT [dbo].[Field] ([Anonymise], [Mandatory], [MaxLength], [RegEx], [FieldType_Id]) 
			VALUES (0, 0, 100, NULL, 3)
		set @fid = (SELECT @@IDENTITY)
		INSERT [dbo].[DatasetElementSub] ([ElementName], [Field_Id], [DatasetElement_Id], [OID], [DefaultValue], [System], FieldOrder)
			VALUES ('Patient Medical Comment', @fid, @deid, '', '', 0, 8)
		set @desid = (SELECT @@IDENTITY)
		-- element node
		INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElementSub_Id, UpdatedBy_Id, DatasetXml_Id)
			VALUES ('patientmedicalcomment', 3, NULL, GETDATE(), GETDATE(), @dsxcnid3, 1, @desid, 1, @dsxid)
		set @dsxenid = (SELECT @@IDENTITY)

/**************************************************
CATEGORY patientpastdrugtherapy
**************************************************/
INSERT [dbo].[DatasetCategory] ([DatasetCategoryName], [CategoryOrder], [Dataset_Id]) 
	VALUES (N'Past Drug Therapy', 1, @dsid)
set @dscid = (SELECT @@IDENTITY)

-- Past Drug Therapy Table
INSERT [dbo].[Field] (Mandatory, MaxLength, Decimals, MaxSize, MinSize, Calculation, FileSize, FileExt, Anonymise, FieldType_Id)
	VALUES (0, NULL, NULL, NULL, NULL, '', NULL, '', 0, 7) 
set @fid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetElement] ([ElementName], [Field_Id], [DatasetElementType_Id], [OID], [DefaultValue], [System])
	VALUES ('Past Drug Therapy', @fid, 1, '', '', 0) 
set @deid = (SELECT @@IDENTITY)

-- category node
INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElement_Id, UpdatedBy_Id, DatasetXml_Id)
	VALUES ('patientpastdrugtherapy', 3, NULL, GETDATE(), GETDATE(), @dsxcnid2, 1, @deid, 1, @dsxid)
set @dsxcnid3 = (SELECT @@IDENTITY)

INSERT [dbo].[DatasetCategoryElement] ([FieldOrder], [DatasetCategory_Id], [DatasetElement_Id], [Acute], [Chronic], [FriendlyName], [Help])
	VALUES (10, @dscid, @deid, 0, 0, 'Past Drug Therapy', 'Past Drug Therapy') 
set @dceid = (SELECT @@IDENTITY)

		INSERT [dbo].[Field] ([Anonymise], [Mandatory], [MaxLength], [RegEx], [FieldType_Id]) 
			VALUES (0, 0, 100, NULL, 3)
		set @fid = (SELECT @@IDENTITY)
		INSERT [dbo].[DatasetElementSub] ([ElementName], [Field_Id], [DatasetElement_Id], [OID], [DefaultValue], [System], FieldOrder)
			VALUES ('Drug Name', @fid, @deid, '', '', 0, 1) 
		set @desid = (SELECT @@IDENTITY)
		-- element node
		INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElementSub_Id, UpdatedBy_Id, DatasetXml_Id)
			VALUES ('patientdrugname', 3, NULL, GETDATE(), GETDATE(), @dsxcnid3, 1, @desid, 1, @dsxid)
		set @dsxenid = (SELECT @@IDENTITY)

		INSERT [dbo].[Field] ([Anonymise], [Mandatory], [MaxLength], [RegEx], [FieldType_Id]) 
			VALUES (0, 0, 10, NULL, 8)
		set @fid = (SELECT @@IDENTITY)
		INSERT [dbo].[DatasetElementSub] ([ElementName], [Field_Id], [DatasetElement_Id], [OID], [DefaultValue], [System], FieldOrder)
			VALUES ('Drug Start Date Format', @fid, @deid, '', '102', 0, 1) 
		set @desid = (SELECT @@IDENTITY)
		-- element node
		INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElementSub_Id, UpdatedBy_Id, DatasetXml_Id)
			VALUES ('patientdrugstartdateformat', 3, NULL, GETDATE(), GETDATE(), @dsxcnid3, 1, @desid, 1, @dsxid)
		set @dsxenid = (SELECT @@IDENTITY)

		INSERT [dbo].[Field] ([Anonymise], [Mandatory], [FieldType_Id]) 
			VALUES (0, 0, 6)
		set @fid = (SELECT @@IDENTITY)
		INSERT [dbo].[DatasetElementSub] ([ElementName], [Field_Id], [DatasetElement_Id], [OID], [DefaultValue], [System], FieldOrder)
			VALUES ('Drug Start Date', @fid, @deid, '', '', 0, 1) 
		set @desid = (SELECT @@IDENTITY)
		-- element node
		INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElementSub_Id, UpdatedBy_Id, DatasetXml_Id)
			VALUES ('patientdrugstartdate', 3, NULL, GETDATE(), GETDATE(), @dsxcnid3, 1, @desid, 1, @dsxid)
		set @dsxenid = (SELECT @@IDENTITY)

		INSERT [dbo].[Field] ([Anonymise], [Mandatory], [MaxLength], [RegEx], [FieldType_Id]) 
			VALUES (0, 0, 10, NULL, 8)
		set @fid = (SELECT @@IDENTITY)
		INSERT [dbo].[DatasetElementSub] ([ElementName], [Field_Id], [DatasetElement_Id], [OID], [DefaultValue], [System], FieldOrder)
			VALUES ('Drug End Date Format', @fid, @deid, '', '102', 0, 1) 
		set @desid = (SELECT @@IDENTITY)
		-- element node
		INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElementSub_Id, UpdatedBy_Id, DatasetXml_Id)
			VALUES ('patientdrugenddateformat', 3, NULL, GETDATE(), GETDATE(), @dsxcnid3, 1, @desid, 1, @dsxid)
		set @dsxenid = (SELECT @@IDENTITY)

		INSERT [dbo].[Field] ([Anonymise], [Mandatory], [FieldType_Id]) 
			VALUES (0, 0, 6)
		set @fid = (SELECT @@IDENTITY)
		INSERT [dbo].[DatasetElementSub] ([ElementName], [Field_Id], [DatasetElement_Id], [OID], [DefaultValue], [System], FieldOrder)
			VALUES ('Drug End Date', @fid, @deid, '', '', 0, 1) 
		set @desid = (SELECT @@IDENTITY)
		-- element node
		INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElementSub_Id, UpdatedBy_Id, DatasetXml_Id)
			VALUES ('patientdrugenddate', 3, NULL, GETDATE(), GETDATE(), @dsxcnid3, 1, @desid, 1, @dsxid)
		set @dsxenid = (SELECT @@IDENTITY)

		INSERT [dbo].[Field] ([Anonymise], [Mandatory], [MaxLength], [RegEx], [FieldType_Id]) 
			VALUES (0, 0, 10, NULL, 8)
		set @fid = (SELECT @@IDENTITY)
		INSERT [dbo].[DatasetElementSub] ([ElementName], [Field_Id], [DatasetElement_Id], [OID], [DefaultValue], [System], FieldOrder)
			VALUES ('Indication MedDRA Version', @fid, @deid, '', 'v20', 0, 1) 
		set @desid = (SELECT @@IDENTITY)
		-- element node
		INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElementSub_Id, UpdatedBy_Id, DatasetXml_Id)
			VALUES ('patientindicationmeddraversion', 3, NULL, GETDATE(), GETDATE(), @dsxcnid3, 1, @desid, 1, @dsxid)
		set @dsxenid = (SELECT @@IDENTITY)

		INSERT [dbo].[Field] ([Anonymise], [Mandatory], [MaxLength], [RegEx], [FieldType_Id]) 
			VALUES (0, 0, 250, NULL, 3)
		set @fid = (SELECT @@IDENTITY)
		INSERT [dbo].[DatasetElementSub] ([ElementName], [Field_Id], [DatasetElement_Id], [OID], [DefaultValue], [System], FieldOrder)
			VALUES ('Indication', @fid, @deid, '', '', 0, 1) 
		set @desid = (SELECT @@IDENTITY)
		-- element node
		INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElementSub_Id, UpdatedBy_Id, DatasetXml_Id)
			VALUES ('patientdrugindication', 3, NULL, GETDATE(), GETDATE(), @dsxcnid3, 1, @desid, 1, @dsxid)
		set @dsxenid = (SELECT @@IDENTITY)

		INSERT [dbo].[Field] ([Anonymise], [Mandatory], [MaxLength], [RegEx], [FieldType_Id]) 
			VALUES (0, 0, 10, NULL, 8)
		set @fid = (SELECT @@IDENTITY)
		INSERT [dbo].[DatasetElementSub] ([ElementName], [Field_Id], [DatasetElement_Id], [OID], [DefaultValue], [System], FieldOrder)
			VALUES ('Drug Reaction MedDRA Version', @fid, @deid, '', '', 0, 1) 
		set @desid = (SELECT @@IDENTITY)
		-- element node
		INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElementSub_Id, UpdatedBy_Id, DatasetXml_Id)
			VALUES ('patientdrgreactionmeddraversion', 3, NULL, GETDATE(), GETDATE(), @dsxcnid3, 1, @desid, 1, @dsxid)
		set @dsxenid = (SELECT @@IDENTITY)

		INSERT [dbo].[Field] ([Anonymise], [Mandatory], [MaxLength], [RegEx], [FieldType_Id]) 
			VALUES (0, 0, 250, NULL, 3)
		set @fid = (SELECT @@IDENTITY)
		INSERT [dbo].[DatasetElementSub] ([ElementName], [Field_Id], [DatasetElement_Id], [OID], [DefaultValue], [System], FieldOrder)
			VALUES ('Drug Reaction', @fid, @deid, '', '', 0, 1) 
		set @desid = (SELECT @@IDENTITY)
		-- element node
		INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElementSub_Id, UpdatedBy_Id, DatasetXml_Id)
			VALUES ('patientdrugreaction', 3, NULL, GETDATE(), GETDATE(), @dsxcnid3, 1, @desid, 1, @dsxid)
		set @dsxenid = (SELECT @@IDENTITY)

/**************************************************
CATEGORY patientdeath
**************************************************/
INSERT [dbo].[DatasetCategory] ([DatasetCategoryName], [CategoryOrder], [Dataset_Id]) 
	VALUES (N'Patient Death', 1, @dsid)
set @dscid = (SELECT @@IDENTITY)

-- category node
INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElement_Id, UpdatedBy_Id, DatasetXml_Id)
	VALUES ('patientdeath', 2, NULL, GETDATE(), GETDATE(), @dsxcnid2, 1, NULL, 1, @dsxid)
set @dsxcnid3 = (SELECT @@IDENTITY)

INSERT [dbo].[Field] ([Anonymise], [Mandatory], [MaxLength], [RegEx], [FieldType_Id]) 
	VALUES (0, 0, 10, NULL, 8)
set @fid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetElement] ([ElementName], [Field_Id], [DatasetElementType_Id], [OID], [DefaultValue], [System], [DatasetElementGuid]) 
	VALUES ('Patient Death Date Format', @fid, 1, NULL, '102', 1, NEWID())
set @deid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetCategoryElement] ([FieldOrder], [DatasetCategory_Id], [DatasetElement_Id], [Acute], [Chronic]) 
	select ISNULL(MAX(FieldOrder) + 1, 1), @dscid, @deid, 0, 0 from [DatasetCategoryElement] where [DatasetCategory_Id] = @dscid
-- element node
INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElement_Id, UpdatedBy_Id, DatasetXml_Id)
	VALUES ('patientdeathdateformat', 2, NULL, GETDATE(), GETDATE(), @dsxcnid3, 1, @deid, 1, @dsxid)
set @dsxenid = (SELECT @@IDENTITY)

INSERT [dbo].[Field] ([Anonymise], [Mandatory], [FieldType_Id]) 
	VALUES (0, 0, 6)
set @fid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetElement] ([ElementName], [Field_Id], [DatasetElementType_Id], [OID], [DefaultValue], [System], [DatasetElementGuid]) 
	VALUES ('Patient Death Date', @fid, 1, NULL, NULL, 0, NEWID())
set @deid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetCategoryElement] ([FieldOrder], [DatasetCategory_Id], [DatasetElement_Id], [Acute], [Chronic]) 
	select ISNULL(MAX(FieldOrder) + 1, 1), @dscid, @deid, 0, 0 from [DatasetCategoryElement] where [DatasetCategory_Id] = @dscid
set @dceid = (SELECT @@IDENTITY)
-- element node
INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElement_Id, UpdatedBy_Id, DatasetXml_Id)
	VALUES ('patientdeathdate', 2, NULL, GETDATE(), GETDATE(), @dsxcnid3, 1, @deid, 1, @dsxid)
set @dsxenid = (SELECT @@IDENTITY)
-- mapping
INSERT [dbo].[DatasetMapping] (Tag, MappingType, MappingOption, DestinationElement_Id, SourceElement_Id)
	VALUES ('Spontaneous', 0, 'yyyyMMdd', @dceid, (select top 1 dce.Id from [dbo].[DatasetCategoryElement] dce inner join [dbo].[DatasetElement] de on dce.DatasetElement_Id = de.Id where de.ElementName = 'Reaction date of death'))
-- mapping
INSERT [dbo].[DatasetMapping] (Tag, MappingType, MappingOption, DestinationElement_Id, PropertyPath, Property)
	VALUES ('Active', 2, 'yyyyMMdd', @dceid, '', 'Date of Death')

INSERT [dbo].[Field] ([Anonymise], [Mandatory], [FieldType_Id]) 
	VALUES (0, 0, 2)
set @fid = (SELECT @@IDENTITY)
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
	VALUES ('1=Yes', 1, 0, 0, @fid)
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
	VALUES ('2=No', 0, 0, 0, @fid)
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
	VALUES ('3=Unknown', 0, 0, 0, @fid)
INSERT [dbo].[DatasetElement] ([ElementName], [Field_Id], [DatasetElementType_Id], [OID], [DefaultValue], [System], [DatasetElementGuid]) 
	VALUES ('Patient Autopsy', @fid, 1, NULL, NULL, 0, NEWID())
set @deid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetCategoryElement] ([FieldOrder], [DatasetCategory_Id], [DatasetElement_Id], [Acute], [Chronic]) 
	select ISNULL(MAX(FieldOrder) + 1, 1), @dscid, @deid, 0, 0 from [DatasetCategoryElement] where [DatasetCategory_Id] = @dscid
set @dceid = (SELECT @@IDENTITY)
-- element node
INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElement_Id, UpdatedBy_Id, DatasetXml_Id)
	VALUES ('patientautopsyyesno', 2, NULL, GETDATE(), GETDATE(), @dsxcnid3, 1, @deid, 1, @dsxid)
set @dsxenid = (SELECT @@IDENTITY)
-- mapping 
INSERT [dbo].[DatasetMapping] (Tag, MappingType, MappingOption, DestinationElement_Id, PropertyPath, Property)
	VALUES ('Active', 3, '', @dceid, '', 'Autopsy Done')
set @dmid = (SELECT @@IDENTITY)
-- mapping values
INSERT [dbo].[DatasetMappingValue] (SourceValue, DestinationValue, Active, Mapping_Id)
	SELECT 'Yes', '1=Yes', 1, @dmid UNION
	SELECT 'No', '2=No', 1, @dmid

INSERT [dbo].[Field] ([Anonymise], [Mandatory], [MaxLength], [RegEx], [FieldType_Id]) 
	VALUES (0, 0, 10, NULL, 8)
set @fid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetElement] ([ElementName], [Field_Id], [DatasetElementType_Id], [OID], [DefaultValue], [System], [DatasetElementGuid]) 
	VALUES ('Patient Death Report MedDRA Version', @fid, 1, NULL, 'v20', 1, NEWID())
set @deid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetCategoryElement] ([FieldOrder], [DatasetCategory_Id], [DatasetElement_Id], [Acute], [Chronic]) 
	select ISNULL(MAX(FieldOrder) + 1, 1), @dscid, @deid, 0, 0 from [DatasetCategoryElement] where [DatasetCategory_Id] = @dscid
-- element node
INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElement_Id, UpdatedBy_Id, DatasetXml_Id)
	VALUES ('patientdeathcause', 2, NULL, GETDATE(), GETDATE(), @dsxcnid3, 1, @deid, 1, @dsxid)
set @dsxcnid4 = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElement_Id, UpdatedBy_Id, DatasetXml_Id)
	VALUES ('patientdeathreportmeddraversion', 2, NULL, GETDATE(), GETDATE(), @dsxcnid4, 1, @deid, 1, @dsxid)
set @dsxenid = (SELECT @@IDENTITY)

INSERT [dbo].[Field] ([Anonymise], [Mandatory], [MaxLength], [RegEx], [FieldType_Id]) 
	VALUES (0, 0, 250, NULL, 3)
set @fid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetElement] ([ElementName], [Field_Id], [DatasetElementType_Id], [OID], [DefaultValue], [System], [DatasetElementGuid]) 
	VALUES ('Patient Death Report', @fid, 1, NULL, NULL, 0, NEWID())
set @deid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetCategoryElement] ([FieldOrder], [DatasetCategory_Id], [DatasetElement_Id], [Acute], [Chronic]) 
	select ISNULL(MAX(FieldOrder) + 1, 1), @dscid, @deid, 0, 0 from [DatasetCategoryElement] where [DatasetCategory_Id] = @dscid
-- element node
INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElement_Id, UpdatedBy_Id, DatasetXml_Id)
	VALUES ('patientdeathreport', 2, NULL, GETDATE(), GETDATE(), @dsxcnid4, 1, @deid, 1, @dsxid)
set @dsxenid = (SELECT @@IDENTITY)

INSERT [dbo].[Field] ([Anonymise], [Mandatory], [MaxLength], [RegEx], [FieldType_Id]) 
	VALUES (0, 0, 10, NULL, 8)
set @fid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetElement] ([ElementName], [Field_Id], [DatasetElementType_Id], [OID], [DefaultValue], [System], [DatasetElementGuid]) 
	VALUES ('Patient Determined Autopsy MedDRA Version', @fid, 1, NULL, 'v20', 1, NEWID())
set @deid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetCategoryElement] ([FieldOrder], [DatasetCategory_Id], [DatasetElement_Id], [Acute], [Chronic]) 
	select ISNULL(MAX(FieldOrder) + 1, 1), @dscid, @deid, 0, 0 from [DatasetCategoryElement] where [DatasetCategory_Id] = @dscid
-- element node
INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElement_Id, UpdatedBy_Id, DatasetXml_Id)
	VALUES ('patientautopsy', 2, NULL, GETDATE(), GETDATE(), @dsxcnid3, 1, @deid, 1, @dsxid)
set @dsxcnid4 = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElement_Id, UpdatedBy_Id, DatasetXml_Id)
	VALUES ('patientdetermautopsmeddraversion', 2, NULL, GETDATE(), GETDATE(), @dsxcnid4, 1, @deid, 1, @dsxid)
set @dsxenid = (SELECT @@IDENTITY)

INSERT [dbo].[Field] ([Anonymise], [Mandatory], [MaxLength], [RegEx], [FieldType_Id]) 
	VALUES (0, 0, 250, NULL, 3)
set @fid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetElement] ([ElementName], [Field_Id], [DatasetElementType_Id], [OID], [DefaultValue], [System], [DatasetElementGuid]) 
	VALUES ('Patient Determine Autopsy', @fid, 1, NULL, NULL, 0, NEWID())
set @deid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetCategoryElement] ([FieldOrder], [DatasetCategory_Id], [DatasetElement_Id], [Acute], [Chronic]) 
	select ISNULL(MAX(FieldOrder) + 1, 1), @dscid, @deid, 0, 0 from [DatasetCategoryElement] where [DatasetCategory_Id] = @dscid
-- element node
INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElement_Id, UpdatedBy_Id, DatasetXml_Id)
	VALUES ('patientdetermineautopsy', 2, NULL, GETDATE(), GETDATE(), @dsxcnid4, 1, @deid, 1, @dsxid)
set @dsxenid = (SELECT @@IDENTITY)

/**************************************************
CATEGORY reaction
**************************************************/
INSERT [dbo].[DatasetCategory] ([DatasetCategoryName], [CategoryOrder], [Dataset_Id]) 
	VALUES (N'Reaction', 1, @dsid)
set @dscid = (SELECT @@IDENTITY)

-- category node
INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElement_Id, UpdatedBy_Id, DatasetXml_Id)
	VALUES ('reaction', 2, NULL, GETDATE(), GETDATE(), @dsxcnid2, 1, NULL, 1, @dsxid)
set @dsxcnid3 = (SELECT @@IDENTITY)

INSERT [dbo].[Field] ([Anonymise], [Mandatory], [MaxLength], [RegEx], [FieldType_Id]) 
	VALUES (0, 0, 200, NULL, 3)
set @fid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetElement] ([ElementName], [Field_Id], [DatasetElementType_Id], [OID], [DefaultValue], [System], [DatasetElementGuid]) 
	VALUES ('Primary Source Reaction', @fid, 1, NULL, NULL, 0, NEWID())
set @deid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetCategoryElement] ([FieldOrder], [DatasetCategory_Id], [DatasetElement_Id], [Acute], [Chronic]) 
	select ISNULL(MAX(FieldOrder) + 1, 1), @dscid, @deid, 0, 0 from [DatasetCategoryElement] where [DatasetCategory_Id] = @dscid
set @dceid = (SELECT @@IDENTITY)
-- element node
INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElement_Id, UpdatedBy_Id, DatasetXml_Id)
	VALUES ('primarysourcereaction', 2, NULL, GETDATE(), GETDATE(), @dsxcnid3, 1, @deid, 1, @dsxid)
set @dsxenid = (SELECT @@IDENTITY)
-- mapping
INSERT [dbo].[DatasetMapping] (Tag, MappingType, MappingOption, DestinationElement_Id, SourceElement_Id)
	VALUES ('Spontaneous', 0, '', @dceid, (select top 1 dce.Id from [dbo].[DatasetCategoryElement] dce inner join [dbo].[DatasetElement] de on dce.DatasetElement_Id = de.Id where de.ElementName = 'Description of reaction'))
-- mapping
INSERT [dbo].[DatasetMapping] (Tag, MappingType, MappingOption, DestinationElement_Id, PropertyPath, Property)
	VALUES ('Active', 4, '', @dceid, '', 'SourceDescription')

INSERT [dbo].[Field] ([Anonymise], [Mandatory], [MaxLength], [RegEx], [FieldType_Id]) 
	VALUES (0, 0, 10, NULL, 8)
set @fid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetElement] ([ElementName], [Field_Id], [DatasetElementType_Id], [OID], [DefaultValue], [System], [DatasetElementGuid]) 
	VALUES ('Reaction MedDRA Version LLT', @fid, 1, NULL, 'v20', 1, NEWID())
set @deid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetCategoryElement] ([FieldOrder], [DatasetCategory_Id], [DatasetElement_Id], [Acute], [Chronic]) 
	select ISNULL(MAX(FieldOrder) + 1, 1), @dscid, @deid, 0, 0 from [DatasetCategoryElement] where [DatasetCategory_Id] = @dscid
-- element node
INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElement_Id, UpdatedBy_Id, DatasetXml_Id)
	VALUES ('reactionmeddraversionllt', 2, NULL, GETDATE(), GETDATE(), @dsxcnid3, 1, @deid, 1, @dsxid)
set @dsxenid = (SELECT @@IDENTITY)

INSERT [dbo].[Field] ([Anonymise], [Mandatory], [MaxLength], [RegEx], [FieldType_Id]) 
	VALUES (0, 0, 250, NULL, 3)
set @fid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetElement] ([ElementName], [Field_Id], [DatasetElementType_Id], [OID], [DefaultValue], [System], [DatasetElementGuid]) 
	VALUES ('Reaction MedDRA LLT', @fid, 1, NULL, NULL, 0, NEWID())
set @deid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetCategoryElement] ([FieldOrder], [DatasetCategory_Id], [DatasetElement_Id], [Acute], [Chronic]) 
	select ISNULL(MAX(FieldOrder) + 1, 1), @dscid, @deid, 0, 0 from [DatasetCategoryElement] where [DatasetCategory_Id] = @dscid
-- element node
INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElement_Id, UpdatedBy_Id, DatasetXml_Id)
	VALUES ('reactionmeddrallt', 2, NULL, GETDATE(), GETDATE(), @dsxcnid3, 1, @deid, 1, @dsxid)
set @dsxenid = (SELECT @@IDENTITY)

INSERT [dbo].[Field] ([Anonymise], [Mandatory], [MaxLength], [RegEx], [FieldType_Id]) 
	VALUES (0, 0, 10, NULL, 8)
set @fid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetElement] ([ElementName], [Field_Id], [DatasetElementType_Id], [OID], [DefaultValue], [System], [DatasetElementGuid]) 
	VALUES ('Reaction MedDRA Version PT', @fid, 1, NULL, 'v20', 1, NEWID())
set @deid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetCategoryElement] ([FieldOrder], [DatasetCategory_Id], [DatasetElement_Id], [Acute], [Chronic]) 
	select ISNULL(MAX(FieldOrder) + 1, 1), @dscid, @deid, 0, 0 from [DatasetCategoryElement] where [DatasetCategory_Id] = @dscid
-- element node
INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElement_Id, UpdatedBy_Id, DatasetXml_Id)
	VALUES ('reactionmeddraversionpt', 2, NULL, GETDATE(), GETDATE(), @dsxcnid3, 1, @deid, 1, @dsxid)
set @dsxenid = (SELECT @@IDENTITY)

INSERT [dbo].[Field] ([Anonymise], [Mandatory], [MaxLength], [RegEx], [FieldType_Id]) 
	VALUES (0, 0, 250, NULL, 3)
set @fid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetElement] ([ElementName], [Field_Id], [DatasetElementType_Id], [OID], [DefaultValue], [System], [DatasetElementGuid]) 
	VALUES ('Reaction MedDRA PT', @fid, 1, NULL, NULL, 0, NEWID())
set @deid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetCategoryElement] ([FieldOrder], [DatasetCategory_Id], [DatasetElement_Id], [Acute], [Chronic]) 
	select ISNULL(MAX(FieldOrder) + 1, 1), @dscid, @deid, 0, 0 from [DatasetCategoryElement] where [DatasetCategory_Id] = @dscid
-- element node
INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElement_Id, UpdatedBy_Id, DatasetXml_Id)
	VALUES ('reactionmeddrapt', 2, NULL, GETDATE(), GETDATE(), @dsxcnid3, 1, @deid, 1, @dsxid)
set @dsxenid = (SELECT @@IDENTITY)

INSERT [dbo].[Field] ([Anonymise], [Mandatory], [FieldType_Id]) 
	VALUES (0, 0, 2)
set @fid = (SELECT @@IDENTITY)
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
	VALUES ('1=Yes, highlighted by the reporter, NOT serious', 1, 0, 0, @fid)
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
	VALUES ('2=No, not highlighted by the reporter, NOT serious', 0, 0, 0, @fid)
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
	VALUES ('3=Yes, highlighted by the reporter, SERIOUS', 0, 0, 0, @fid)
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
	VALUES ('4=No, not highlighted by the reporter, SERIOUS', 0, 0, 0, @fid)
INSERT [dbo].[DatasetElement] ([ElementName], [Field_Id], [DatasetElementType_Id], [OID], [DefaultValue], [System], [DatasetElementGuid]) 
	VALUES ('Term Highlighted', @fid, 1, NULL, NULL, 0, NEWID())
set @deid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetCategoryElement] ([FieldOrder], [DatasetCategory_Id], [DatasetElement_Id], [Acute], [Chronic]) 
	select ISNULL(MAX(FieldOrder) + 1, 1), @dscid, @deid, 0, 0 from [DatasetCategoryElement] where [DatasetCategory_Id] = @dscid
-- element node
INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElement_Id, UpdatedBy_Id, DatasetXml_Id)
	VALUES ('termhighlighted', 2, NULL, GETDATE(), GETDATE(), @dsxcnid3, 1, @deid, 1, @dsxid)
set @dsxenid = (SELECT @@IDENTITY)

INSERT [dbo].[Field] ([Anonymise], [Mandatory], [MaxLength], [RegEx], [FieldType_Id]) 
	VALUES (0, 0, 10, NULL, 8)
set @fid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetElement] ([ElementName], [Field_Id], [DatasetElementType_Id], [OID], [DefaultValue], [System], [DatasetElementGuid]) 
	VALUES ('Reaction Start Date Format', @fid, 1, NULL, '102', 1, NEWID())
set @deid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetCategoryElement] ([FieldOrder], [DatasetCategory_Id], [DatasetElement_Id], [Acute], [Chronic]) 
	select ISNULL(MAX(FieldOrder) + 1, 1), @dscid, @deid, 0, 0 from [DatasetCategoryElement] where [DatasetCategory_Id] = @dscid
-- element node
INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElement_Id, UpdatedBy_Id, DatasetXml_Id)
	VALUES ('reactionstartdateformat', 2, NULL, GETDATE(), GETDATE(), @dsxcnid3, 1, @deid, 1, @dsxid)
set @dsxenid = (SELECT @@IDENTITY)

INSERT [dbo].[Field] ([Anonymise], [Mandatory], [FieldType_Id]) 
	VALUES (0, 0, 6)
set @fid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetElement] ([ElementName], [Field_Id], [DatasetElementType_Id], [OID], [DefaultValue], [System], [DatasetElementGuid]) 
	VALUES ('Reaction Start Date', @fid, 1, NULL, NULL, 0, NEWID())
set @deid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetCategoryElement] ([FieldOrder], [DatasetCategory_Id], [DatasetElement_Id], [Acute], [Chronic]) 
	select ISNULL(MAX(FieldOrder) + 1, 1), @dscid, @deid, 0, 0 from [DatasetCategoryElement] where [DatasetCategory_Id] = @dscid
-- element node
INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElement_Id, UpdatedBy_Id, DatasetXml_Id)
	VALUES ('reactionstartdate', 2, NULL, GETDATE(), GETDATE(), @dsxcnid3, 1, @deid, 1, @dsxid)
set @dsxenid = (SELECT @@IDENTITY)

INSERT [dbo].[Field] ([Anonymise], [Mandatory], [MaxLength], [RegEx], [FieldType_Id]) 
	VALUES (0, 0, 10, NULL, 8)
set @fid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetElement] ([ElementName], [Field_Id], [DatasetElementType_Id], [OID], [DefaultValue], [System], [DatasetElementGuid]) 
	VALUES ('Reaction End Date Format', @fid, 1, NULL, '102', 1, NEWID())
set @deid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetCategoryElement] ([FieldOrder], [DatasetCategory_Id], [DatasetElement_Id], [Acute], [Chronic]) 
	select ISNULL(MAX(FieldOrder) + 1, 1), @dscid, @deid, 0, 0 from [DatasetCategoryElement] where [DatasetCategory_Id] = @dscid
-- element node
INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElement_Id, UpdatedBy_Id, DatasetXml_Id)
	VALUES ('reactionenddateformat', 2, NULL, GETDATE(), GETDATE(), @dsxcnid3, 1, @deid, 1, @dsxid)
set @dsxenid = (SELECT @@IDENTITY)

INSERT [dbo].[Field] ([Anonymise], [Mandatory], [FieldType_Id]) 
	VALUES (0, 0, 6)
set @fid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetElement] ([ElementName], [Field_Id], [DatasetElementType_Id], [OID], [DefaultValue], [System], [DatasetElementGuid]) 
	VALUES ('Reaction End Date', @fid, 1, NULL, NULL, 0, NEWID())
set @deid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetCategoryElement] ([FieldOrder], [DatasetCategory_Id], [DatasetElement_Id], [Acute], [Chronic]) 
	select ISNULL(MAX(FieldOrder) + 1, 1), @dscid, @deid, 0, 0 from [DatasetCategoryElement] where [DatasetCategory_Id] = @dscid
set @dceid = (SELECT @@IDENTITY)
-- element node
INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElement_Id, UpdatedBy_Id, DatasetXml_Id)
	VALUES ('reactionenddate', 2, NULL, GETDATE(), GETDATE(), @dsxcnid3, 1, @deid, 1, @dsxid)
set @dsxenid = (SELECT @@IDENTITY)
-- mapping
INSERT [dbo].[DatasetMapping] (Tag, MappingType, MappingOption, DestinationElement_Id, SourceElement_Id)
	VALUES ('Spontaneous', 0, 'yyyyMMdd', @dceid, (select top 1 dce.Id from [dbo].[DatasetCategoryElement] dce inner join [dbo].[DatasetElement] de on dce.DatasetElement_Id = de.Id where de.ElementName = 'Reaction date of recovery'))

INSERT [dbo].[Field] ([Anonymise], [Mandatory], [Decimals], [MaxSize], [MinSize], [FieldType_Id]) 
	VALUES (0, 0, 0, 150, 0, 4)
set @fid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetElement] ([ElementName], [Field_Id], [DatasetElementType_Id], [OID], [DefaultValue], [System], [DatasetElementGuid]) 
	VALUES ('Reaction Duration', @fid, 1, NULL, NULL, 0, NEWID())
set @deid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetCategoryElement] ([FieldOrder], [DatasetCategory_Id], [DatasetElement_Id], [Acute], [Chronic]) 
	select ISNULL(MAX(FieldOrder) + 1, 1), @dscid, @deid, 0, 0 from [DatasetCategoryElement] where [DatasetCategory_Id] = @dscid
-- element node
INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElement_Id, UpdatedBy_Id, DatasetXml_Id)
	VALUES ('reactionduration', 2, NULL, GETDATE(), GETDATE(), @dsxcnid3, 1, @deid, 1, @dsxid)
set @dsxenid = (SELECT @@IDENTITY)

INSERT [dbo].[Field] ([Anonymise], [Mandatory], [FieldType_Id]) 
	VALUES (0, 0, 2)
set @fid = (SELECT @@IDENTITY)
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
	VALUES ('800=Decade', 1, 0, 0, @fid)
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
	VALUES ('801=Year', 0, 0, 0, @fid)
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
	VALUES ('802=Month', 0, 0, 0, @fid)
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
	VALUES ('803=Week', 0, 0, 0, @fid)
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
	VALUES ('804=Day', 0, 0, 0, @fid)
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
	VALUES ('805=Hour', 0, 0, 0, @fid)
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
	VALUES ('806=Minute', 0, 0, 0, @fid)
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
	VALUES ('807=Second', 0, 0, 0, @fid)
INSERT [dbo].[DatasetElement] ([ElementName], [Field_Id], [DatasetElementType_Id], [OID], [DefaultValue], [System], [DatasetElementGuid]) 
	VALUES ('Reaction Duration Unit', @fid, 1, NULL, NULL, 0, NEWID())
set @deid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetCategoryElement] ([FieldOrder], [DatasetCategory_Id], [DatasetElement_Id], [Acute], [Chronic]) 
	select ISNULL(MAX(FieldOrder) + 1, 1), @dscid, @deid, 0, 0 from [DatasetCategoryElement] where [DatasetCategory_Id] = @dscid
-- element node
INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElement_Id, UpdatedBy_Id, DatasetXml_Id)
	VALUES ('reactiondurationunit', 2, NULL, GETDATE(), GETDATE(), @dsxcnid3, 1, @deid, 1, @dsxid)
set @dsxenid = (SELECT @@IDENTITY)

INSERT [dbo].[Field] ([Anonymise], [Mandatory], [Decimals], [MaxSize], [MinSize], [FieldType_Id]) 
	VALUES (0, 0, 0, 150, 1, 4)
set @fid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetElement] ([ElementName], [Field_Id], [DatasetElementType_Id], [OID], [DefaultValue], [System], [DatasetElementGuid]) 
	VALUES ('Reaction First Time', @fid, 1, NULL, NULL, 0, NEWID())
set @deid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetCategoryElement] ([FieldOrder], [DatasetCategory_Id], [DatasetElement_Id], [Acute], [Chronic]) 
	select ISNULL(MAX(FieldOrder) + 1, 1), @dscid, @deid, 0, 0 from [DatasetCategoryElement] where [DatasetCategory_Id] = @dscid
-- element node
INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElement_Id, UpdatedBy_Id, DatasetXml_Id)
	VALUES ('reactionfirsttime', 2, NULL, GETDATE(), GETDATE(), @dsxcnid3, 1, @deid, 1, @dsxid)
set @dsxenid = (SELECT @@IDENTITY)

INSERT [dbo].[Field] ([Anonymise], [Mandatory], [FieldType_Id]) 
	VALUES (0, 0, 2)
set @fid = (SELECT @@IDENTITY)
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
	VALUES ('800=Decade', 1, 0, 0, @fid)
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
	VALUES ('801=Year', 0, 0, 0, @fid)
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
	VALUES ('802=Month', 0, 0, 0, @fid)
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
	VALUES ('803=Week', 0, 0, 0, @fid)
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
	VALUES ('804=Day', 0, 0, 0, @fid)
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
	VALUES ('805=Hour', 0, 0, 0, @fid)
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
	VALUES ('806=Minute', 0, 0, 0, @fid)
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
	VALUES ('807=Second', 0, 0, 0, @fid)
INSERT [dbo].[DatasetElement] ([ElementName], [Field_Id], [DatasetElementType_Id], [OID], [DefaultValue], [System], [DatasetElementGuid]) 
	VALUES ('Reaction First Time Unit', @fid, 1, NULL, NULL, 0, NEWID())
set @deid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetCategoryElement] ([FieldOrder], [DatasetCategory_Id], [DatasetElement_Id], [Acute], [Chronic]) 
	select ISNULL(MAX(FieldOrder) + 1, 1), @dscid, @deid, 0, 0 from [DatasetCategoryElement] where [DatasetCategory_Id] = @dscid
-- element node
INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElement_Id, UpdatedBy_Id, DatasetXml_Id)
	VALUES ('reactionfirsttimeunit', 2, NULL, GETDATE(), GETDATE(), @dsxcnid3, 1, @deid, 1, @dsxid)
set @dsxenid = (SELECT @@IDENTITY)

INSERT [dbo].[Field] ([Anonymise], [Mandatory], [Decimals], [MaxSize], [MinSize], [FieldType_Id]) 
	VALUES (0, 0, 0, 150, 1, 4)
set @fid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetElement] ([ElementName], [Field_Id], [DatasetElementType_Id], [OID], [DefaultValue], [System], [DatasetElementGuid]) 
	VALUES ('Reaction Last Time', @fid, 1, NULL, NULL, 0, NEWID())
set @deid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetCategoryElement] ([FieldOrder], [DatasetCategory_Id], [DatasetElement_Id], [Acute], [Chronic]) 
	select ISNULL(MAX(FieldOrder) + 1, 1), @dscid, @deid, 0, 0 from [DatasetCategoryElement] where [DatasetCategory_Id] = @dscid
-- element node
INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElement_Id, UpdatedBy_Id, DatasetXml_Id)
	VALUES ('reactionlasttime', 2, NULL, GETDATE(), GETDATE(), @dsxcnid3, 1, @deid, 1, @dsxid)
set @dsxenid = (SELECT @@IDENTITY)

INSERT [dbo].[Field] ([Anonymise], [Mandatory], [FieldType_Id]) 
	VALUES (0, 0, 2)
set @fid = (SELECT @@IDENTITY)
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
	VALUES ('800=Decade', 1, 0, 0, @fid)
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
	VALUES ('801=Year', 0, 0, 0, @fid)
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
	VALUES ('802=Month', 0, 0, 0, @fid)
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
	VALUES ('803=Week', 0, 0, 0, @fid)
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
	VALUES ('804=Day', 0, 0, 0, @fid)
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
	VALUES ('805=Hour', 0, 0, 0, @fid)
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
	VALUES ('806=Minute', 0, 0, 0, @fid)
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
	VALUES ('807=Second', 0, 0, 0, @fid)
INSERT [dbo].[DatasetElement] ([ElementName], [Field_Id], [DatasetElementType_Id], [OID], [DefaultValue], [System], [DatasetElementGuid]) 
	VALUES ('Reaction Last Time Unit', @fid, 1, NULL, NULL, 0, NEWID())
set @deid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetCategoryElement] ([FieldOrder], [DatasetCategory_Id], [DatasetElement_Id], [Acute], [Chronic]) 
	select ISNULL(MAX(FieldOrder) + 1, 1), @dscid, @deid, 0, 0 from [DatasetCategoryElement] where [DatasetCategory_Id] = @dscid
-- element node
INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElement_Id, UpdatedBy_Id, DatasetXml_Id)
	VALUES ('reactionlasttimeunit', 2, NULL, GETDATE(), GETDATE(), @dsxcnid3, 1, @deid, 1, @dsxid)
set @dsxenid = (SELECT @@IDENTITY)

INSERT [dbo].[Field] ([Anonymise], [Mandatory], [FieldType_Id]) 
	VALUES (0, 0, 2)
set @fid = (SELECT @@IDENTITY)
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
	VALUES ('1=recovered/resolved', 1, 0, 0, @fid)
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
	VALUES ('2=recovering/resolving', 0, 0, 0, @fid)
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
	VALUES ('3=not recovered/not resolved', 0, 0, 0, @fid)
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
	VALUES ('4=recovered/resolved with sequelae', 0, 0, 0, @fid)
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
	VALUES ('5=fatal', 0, 0, 0, @fid)
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
	VALUES ('6=unknown', 0, 0, 0, @fid)
INSERT [dbo].[DatasetElement] ([ElementName], [Field_Id], [DatasetElementType_Id], [OID], [DefaultValue], [System], [DatasetElementGuid]) 
	VALUES ('Reaction Outcome', @fid, 1, NULL, NULL, 0, NEWID())
set @deid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetCategoryElement] ([FieldOrder], [DatasetCategory_Id], [DatasetElement_Id], [Acute], [Chronic]) 
	select ISNULL(MAX(FieldOrder) + 1, 1), @dscid, @deid, 0, 0 from [DatasetCategoryElement] where [DatasetCategory_Id] = @dscid
set @dceid = (SELECT @@IDENTITY)
-- element node
INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElement_Id, UpdatedBy_Id, DatasetXml_Id)
	VALUES ('reactionoutcome', 2, NULL, GETDATE(), GETDATE(), @dsxcnid3, 1, @deid, 1, @dsxid)
set @dsxenid = (SELECT @@IDENTITY)
-- mapping
INSERT [dbo].[DatasetMapping] (Tag, MappingType, MappingOption, DestinationElement_Id, SourceElement_Id)
	VALUES ('Spontaneous', 1, '', @dceid, (select top 1 dce.Id from [dbo].[DatasetCategoryElement] dce inner join [dbo].[DatasetElement] de on dce.DatasetElement_Id = de.Id where de.ElementName = 'Outcome of reaction'))
set @dmid = (SELECT @@IDENTITY)
-- mapping values
INSERT [dbo].[DatasetMappingValue] (SourceValue, DestinationValue, Active, Mapping_Id)
	SELECT 'Recovering/resolving', '2=recovering/resolving', 1, @dmid UNION
	SELECT 'Not recovered/not resolved', '3=not recovered/not resolved', 1, @dmid UNION
	SELECT 'Recovered/resolved with permanent complications', '4=recovered/resolved with sequelae', 1, @dmid UNION
	SELECT 'Fatal', '5=fatal', 1, @dmid UNION
	SELECT 'Recovered/resolved', '1=recovered/resolved', 1, @dmid UNION
	SELECT 'Unknown', '6=unknown', 1, @dmid
-- mapping
INSERT [dbo].[DatasetMapping] (Tag, MappingType, MappingOption, DestinationElement_Id, PropertyPath, Property)
	VALUES ('Active', 3, '', @dceid, '', 'Outcome')
set @dmid = (SELECT @@IDENTITY)
-- mapping values
INSERT [dbo].[DatasetMappingValue] (SourceValue, DestinationValue, Active, Mapping_Id)
	SELECT '4', '2=recovering/resolving', 1, @dmid UNION
	SELECT '3', '5=fatal', 1, @dmid UNION
	SELECT '2', '4=recovered/resolved with sequelae', 1, @dmid UNION
	SELECT '5', '3=not recovered/not resolved', 1, @dmid UNION
	SELECT '1', '1=recovered/resolved', 1, @dmid UNION
	SELECT '6', '6=unknown', 1, @dmid

/**************************************************
CATEGORY test
**************************************************/
INSERT [dbo].[DatasetCategory] ([DatasetCategoryName], [CategoryOrder], [Dataset_Id]) 
	VALUES (N'Test', 1, @dsid)
set @dscid = (SELECT @@IDENTITY)

-- Test Table
INSERT [dbo].[Field] (Mandatory, MaxLength, Decimals, MaxSize, MinSize, Calculation, FileSize, FileExt, Anonymise, FieldType_Id)
	VALUES (0, NULL, NULL, NULL, NULL, '', NULL, '', 0, 7) 
set @fid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetElement] ([ElementName], [Field_Id], [DatasetElementType_Id], [OID], [DefaultValue], [System])
	VALUES ('Test History', @fid, 1, '', '', 0) 
set @deid = (SELECT @@IDENTITY)

-- category node
INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElement_Id, UpdatedBy_Id, DatasetXml_Id)
	VALUES ('test', 3, NULL, GETDATE(), GETDATE(), @dsxcnid2, 1, @deid, 1, @dsxid)
set @dsxcnid3 = (SELECT @@IDENTITY)

INSERT [dbo].[DatasetCategoryElement] ([FieldOrder], [DatasetCategory_Id], [DatasetElement_Id], [Acute], [Chronic], [FriendlyName], [Help])
	VALUES (10, @dscid, @deid, 0, 0, 'Test History', 'Test History') 
set @dceid = (SELECT @@IDENTITY)

		set @sdceid = (select top 1 dce.Id from [dbo].[DatasetCategoryElement] dce inner join [dbo].[DatasetElement] de on dce.DatasetElement_Id = de.Id where de.ElementName = 'Test Results')
		-- mapping
		INSERT [dbo].[DatasetMapping] (Tag, MappingType, MappingOption, DestinationElement_Id, SourceElement_Id)
			VALUES ('Spontaneous', 0, '', @dceid, @sdceid)
		set @dmid = (SELECT @@IDENTITY)

		INSERT [dbo].[Field] ([Anonymise], [Mandatory], [MaxLength], [RegEx], [FieldType_Id]) 
			VALUES (0, 0, 10, NULL, 8)
		set @fid = (SELECT @@IDENTITY)
		INSERT [dbo].[DatasetElementSub] ([ElementName], [Field_Id], [DatasetElement_Id], [OID], [DefaultValue], [System], FieldOrder)
			VALUES ('Test Date Format', @fid, @deid, '', '102', 0, 1) 
		set @desid = (SELECT @@IDENTITY)
		-- element node
		INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElementSub_Id, UpdatedBy_Id, DatasetXml_Id)
			VALUES ('testdateformat', 3, NULL, GETDATE(), GETDATE(), @dsxcnid3, 1, @desid, 1, @dsxid)
		set @dsxenid = (SELECT @@IDENTITY)

		INSERT [dbo].[Field] ([Anonymise], [Mandatory], [FieldType_Id]) 
			VALUES (0, 0, 6)
		set @fid = (SELECT @@IDENTITY)
		INSERT [dbo].[DatasetElementSub] ([ElementName], [Field_Id], [DatasetElement_Id], [OID], [DefaultValue], [System], FieldOrder)
			VALUES ('Test Date', @fid, @deid, '', '', 0, 1) 
		set @desid = (SELECT @@IDENTITY)
		-- element node
		INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElementSub_Id, UpdatedBy_Id, DatasetXml_Id)
			VALUES ('testdate', 3, NULL, GETDATE(), GETDATE(), @dsxcnid3, 1, @desid, 1, @dsxid)
		set @dsxenid = (SELECT @@IDENTITY)
		-- submapping
		INSERT [dbo].[DatasetMappingSub] (MappingType, MappingOption, DestinationElement_Id, Mapping_Id, SourceElement_Id)
			VALUES (0, 'yyyyMMdd', @desid, @dmid, (select top 1 desu.Id from [dbo].[DatasetElementSub] desu inner join [dbo].[DatasetElement] de on desu.DatasetElement_Id = de.Id where de.ElementName = 'Test Results' and desu.ElementName = 'Test Date'))
		set @dmsid = (SELECT @@IDENTITY)

		INSERT [dbo].[Field] ([Anonymise], [Mandatory], [MaxLength], [RegEx], [FieldType_Id]) 
			VALUES (0, 0, 100, NULL, 3)
		set @fid = (SELECT @@IDENTITY)
		INSERT [dbo].[DatasetElementSub] ([ElementName], [Field_Id], [DatasetElement_Id], [OID], [DefaultValue], [System], FieldOrder)
			VALUES ('Test Name', @fid, @deid, '', '', 0, 1) 
		set @desid = (SELECT @@IDENTITY)
		-- element node
		INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElementSub_Id, UpdatedBy_Id, DatasetXml_Id)
			VALUES ('testname', 3, NULL, GETDATE(), GETDATE(), @dsxcnid3, 1, @desid, 1, @dsxid)
		set @dsxenid = (SELECT @@IDENTITY)
		-- submapping
		INSERT [dbo].[DatasetMappingSub] (MappingType, MappingOption, DestinationElement_Id, Mapping_Id, SourceElement_Id)
			VALUES (0, '', @desid, @dmid, (select top 1 desu.Id from [dbo].[DatasetElementSub] desu inner join [dbo].[DatasetElement] de on desu.DatasetElement_Id = de.Id where de.ElementName = 'Test Results' and desu.ElementName = 'Test Name'))
		set @dmsid = (SELECT @@IDENTITY)
		
		INSERT [dbo].[Field] ([Anonymise], [Mandatory], [MaxLength], [RegEx], [FieldType_Id]) 
			VALUES (0, 0, 50, NULL, 3)
		set @fid = (SELECT @@IDENTITY)
		INSERT [dbo].[DatasetElementSub] ([ElementName], [Field_Id], [DatasetElement_Id], [OID], [DefaultValue], [System], FieldOrder)
			VALUES ('Test Result', @fid, @deid, '', '', 0, 1) 
		set @desid = (SELECT @@IDENTITY)
		-- element node
		INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElementSub_Id, UpdatedBy_Id, DatasetXml_Id)
			VALUES ('testresult', 3, NULL, GETDATE(), GETDATE(), @dsxcnid3, 1, @desid, 1, @dsxid)
		set @dsxenid = (SELECT @@IDENTITY)
		-- submapping
		INSERT [dbo].[DatasetMappingSub] (MappingType, MappingOption, DestinationElement_Id, Mapping_Id, SourceElement_Id)
			VALUES (0, '', @desid, @dmid, (select top 1 desu.Id from [dbo].[DatasetElementSub] desu inner join [dbo].[DatasetElement] de on desu.DatasetElement_Id = de.Id where de.ElementName = 'Test Results' and desu.ElementName = 'Test Result'))
		set @dmsid = (SELECT @@IDENTITY)

		INSERT [dbo].[Field] ([Anonymise], [Mandatory], [MaxLength], [RegEx], [FieldType_Id]) 
			VALUES (0, 0, 35, NULL, 3)
		set @fid = (SELECT @@IDENTITY)
		INSERT [dbo].[DatasetElementSub] ([ElementName], [Field_Id], [DatasetElement_Id], [OID], [DefaultValue], [System], FieldOrder)
			VALUES ('Test Unit', @fid, @deid, '', '', 0, 1) 
		set @desid = (SELECT @@IDENTITY)
		-- element node
		INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElementSub_Id, UpdatedBy_Id, DatasetXml_Id)
			VALUES ('testunit', 3, NULL, GETDATE(), GETDATE(), @dsxcnid3, 1, @desid, 1, @dsxid)
		set @dsxenid = (SELECT @@IDENTITY)
		-- submapping
		INSERT [dbo].[DatasetMappingSub] (MappingType, MappingOption, DestinationElement_Id, Mapping_Id, SourceElement_Id)
			VALUES (0, '', @desid, @dmid, (select top 1 desu.Id from [dbo].[DatasetElementSub] desu inner join [dbo].[DatasetElement] de on desu.DatasetElement_Id = de.Id where de.ElementName = 'Test Results' and desu.ElementName = 'Test Unit'))
		set @dmsid = (SELECT @@IDENTITY)

		INSERT [dbo].[Field] ([Anonymise], [Mandatory], [MaxLength], [RegEx], [FieldType_Id]) 
			VALUES (0, 0, 50, NULL, 3)
		set @fid = (SELECT @@IDENTITY)
		INSERT [dbo].[DatasetElementSub] ([ElementName], [Field_Id], [DatasetElement_Id], [OID], [DefaultValue], [System], FieldOrder)
			VALUES ('Low Test Range', @fid, @deid, '', '', 0, 1) 
		set @desid = (SELECT @@IDENTITY)
		-- element node
		INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElementSub_Id, UpdatedBy_Id, DatasetXml_Id)
			VALUES ('lowtestrange', 3, NULL, GETDATE(), GETDATE(), @dsxcnid3, 1, @desid, 1, @dsxid)
		set @dsxenid = (SELECT @@IDENTITY)
		-- submapping
		INSERT [dbo].[DatasetMappingSub] (MappingType, MappingOption, DestinationElement_Id, Mapping_Id, SourceElement_Id)
			VALUES (0, '', @desid, @dmid, (select top 1 desu.Id from [dbo].[DatasetElementSub] desu inner join [dbo].[DatasetElement] de on desu.DatasetElement_Id = de.Id where de.ElementName = 'Test Results' and desu.ElementName = 'Low Test Range'))
		set @dmsid = (SELECT @@IDENTITY)

		INSERT [dbo].[Field] ([Anonymise], [Mandatory], [MaxLength], [RegEx], [FieldType_Id]) 
			VALUES (0, 0, 50, NULL, 3)
		set @fid = (SELECT @@IDENTITY)
		INSERT [dbo].[DatasetElementSub] ([ElementName], [Field_Id], [DatasetElement_Id], [OID], [DefaultValue], [System], FieldOrder)
			VALUES ('High Test Range', @fid, @deid, '', '', 0, 1) 
		set @desid = (SELECT @@IDENTITY)
		-- element node
		INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElementSub_Id, UpdatedBy_Id, DatasetXml_Id)
			VALUES ('hightestrange', 3, NULL, GETDATE(), GETDATE(), @dsxcnid3, 1, @desid, 1, @dsxid)
		set @dsxenid = (SELECT @@IDENTITY)
		-- submapping
		INSERT [dbo].[DatasetMappingSub] (MappingType, MappingOption, DestinationElement_Id, Mapping_Id, SourceElement_Id)
			VALUES (0, '', @desid, @dmid, (select top 1 desu.Id from [dbo].[DatasetElementSub] desu inner join [dbo].[DatasetElement] de on desu.DatasetElement_Id = de.Id where de.ElementName = 'Test Results' and desu.ElementName = 'High Test Range'))
		set @dmsid = (SELECT @@IDENTITY)

		INSERT [dbo].[Field] ([Anonymise], [Mandatory], [FieldType_Id]) 
			VALUES (0, 0, 2)
		set @fid = (SELECT @@IDENTITY)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('1=Yes', 1, 0, 0, @fid)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('2=No', 0, 0, 0, @fid)
		INSERT [dbo].[DatasetElementSub] ([ElementName], [Field_Id], [DatasetElement_Id], [OID], [DefaultValue], [System], FieldOrder)
			VALUES ('More Information', @fid, @deid, '', '', 0, 1) 
		set @desid = (SELECT @@IDENTITY)
		-- element node
		INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElementSub_Id, UpdatedBy_Id, DatasetXml_Id)
			VALUES ('moreinformation', 3, NULL, GETDATE(), GETDATE(), @dsxcnid3, 1, @desid, 1, @dsxid)
		set @dsxenid = (SELECT @@IDENTITY)
		-- submapping
		INSERT [dbo].[DatasetMappingSub] (MappingType, MappingOption, DestinationElement_Id, Mapping_Id, SourceElement_Id)
			VALUES (1, '', @desid, @dmid, (select top 1 desu.Id from [dbo].[DatasetElementSub] desu inner join [dbo].[DatasetElement] de on desu.DatasetElement_Id = de.Id where de.ElementName = 'Test Results' and desu.ElementName = 'More Information'))
		set @dmsid = (SELECT @@IDENTITY)
		-- mapping values
		INSERT [dbo].[DatasetMappingValue] (SourceValue, DestinationValue, Active, Mapping_Id, SubMapping_Id)
			SELECT 'Yes', '1=Yes', 1, @dmid, @dmsid UNION
			SELECT 'No', '2=No', 1, @dmid, @dmsid

/**************************************************
CATEGORY drug
**************************************************/
INSERT [dbo].[DatasetCategory] ([DatasetCategoryName], [CategoryOrder], [Dataset_Id]) 
	VALUES (N'Drug', 1, @dsid)
set @dscid = (SELECT @@IDENTITY)

-- Medicinal Products Table
INSERT [dbo].[Field] (Mandatory, MaxLength, Decimals, MaxSize, MinSize, Calculation, FileSize, FileExt, Anonymise, FieldType_Id)
	VALUES (0, NULL, NULL, NULL, NULL, '', NULL, '', 0, 7) 
set @fid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetElement] ([ElementName], [Field_Id], [DatasetElementType_Id], [OID], [DefaultValue], [System])
	VALUES ('Medicinal Products', @fid, 1, '', '', 0) 
set @deid = (SELECT @@IDENTITY)
-- category node
INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElement_Id, UpdatedBy_Id, DatasetXml_Id)
	VALUES ('drug', 3, NULL, GETDATE(), GETDATE(), @dsxcnid2, 1, @deid, 1, @dsxid)
set @dsxcnid3 = (SELECT @@IDENTITY)

INSERT [dbo].[DatasetCategoryElement] ([FieldOrder], [DatasetCategory_Id], [DatasetElement_Id], [Acute], [Chronic], [FriendlyName], [Help])
	VALUES (10, @dscid, @deid, 0, 0, 'Medicinal Products', 'Medicinal Products') 
set @dceid = (SELECT @@IDENTITY)

		set @sdceid = (select top 1 dce.Id from [dbo].[DatasetCategoryElement] dce inner join [dbo].[DatasetElement] de on dce.DatasetElement_Id = de.Id where de.ElementName = 'Product Information')
		-- mapping
		INSERT [dbo].[DatasetMapping] (Tag, MappingType, MappingOption, DestinationElement_Id, SourceElement_Id)
			VALUES ('Spontaneous', 0, '', @dceid, @sdceid)
		set @dmid = (SELECT @@IDENTITY)
		
		INSERT [dbo].[Field] ([Anonymise], [Mandatory], [FieldType_Id]) 
			VALUES (0, 0, 2)
		set @fid = (SELECT @@IDENTITY)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('1=Suspect', 1, 0, 0, @fid)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('2=Concomitant', 0, 0, 0, @fid)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('3=Interacting', 0, 0, 0, @fid)
		INSERT [dbo].[DatasetElementSub] ([ElementName], [Field_Id], [DatasetElement_Id], [OID], [DefaultValue], [System], FieldOrder)
			VALUES ('Drug Characterization', @fid, @deid, '', '', 0, 1) 
		set @desid = (SELECT @@IDENTITY)
		-- element node
		INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElementSub_Id, UpdatedBy_Id, DatasetXml_Id)
			VALUES ('drugcharacterization', 3, NULL, GETDATE(), GETDATE(), @dsxcnid3, 1, @desid, 1, @dsxid)
		set @dsxenid = (SELECT @@IDENTITY)
		-- submapping
		INSERT [dbo].[DatasetMappingSub] (MappingType, MappingOption, DestinationElement_Id, Mapping_Id, SourceElement_Id)
			VALUES (1, '', @desid, @dmid, (select top 1 desu.Id from [dbo].[DatasetElementSub] desu inner join [dbo].[DatasetElement] de on desu.DatasetElement_Id = de.Id where de.ElementName = 'Product Information' and desu.ElementName = 'Product Suspected'))
		set @dmsid = (SELECT @@IDENTITY)
		-- mapping values
		INSERT [dbo].[DatasetMappingValue] (SourceValue, DestinationValue, Active, Mapping_Id, SubMapping_Id)
			SELECT 'Yes', '1=Suspect', 1, @dmid, @dmsid UNION
			SELECT 'No', '2=Concomitant', 1, @dmid, @dmsid

		INSERT [dbo].[Field] ([Anonymise], [Mandatory], [MaxLength], [RegEx], [FieldType_Id]) 
			VALUES (0, 0, 70, NULL, 3)
		set @fid = (SELECT @@IDENTITY)
		INSERT [dbo].[DatasetElementSub] ([ElementName], [Field_Id], [DatasetElement_Id], [OID], [DefaultValue], [System], FieldOrder)
			VALUES ('Medicinal Product', @fid, @deid, '', '', 0, 1) 
		set @desid = (SELECT @@IDENTITY)
		-- element node
		INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElementSub_Id, UpdatedBy_Id, DatasetXml_Id)
			VALUES ('medicinalproduct', 3, NULL, GETDATE(), GETDATE(), @dsxcnid3, 1, @desid, 1, @dsxid)
		set @dsxenid = (SELECT @@IDENTITY)
		-- submapping
		INSERT [dbo].[DatasetMappingSub] (MappingType, MappingOption, DestinationElement_Id, Mapping_Id, SourceElement_Id)
			VALUES (0, '', @desid, @dmid, (select top 1 desu.Id from [dbo].[DatasetElementSub] desu inner join [dbo].[DatasetElement] de on desu.DatasetElement_Id = de.Id where de.ElementName = 'Product Information' and desu.ElementName = 'Product'))
		set @dmsid = (SELECT @@IDENTITY)

		INSERT [dbo].[Field] ([Anonymise], [Mandatory], [MaxLength], [RegEx], [FieldType_Id]) 
			VALUES (0, 0, 2, NULL, 3)
		set @fid = (SELECT @@IDENTITY)
		INSERT [dbo].[DatasetElementSub] ([ElementName], [Field_Id], [DatasetElement_Id], [OID], [DefaultValue], [System], FieldOrder)
			VALUES ('Obtain Drug Country', @fid, @deid, '', '', 0, 1) 
		set @desid = (SELECT @@IDENTITY)
		-- element node
		INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElementSub_Id, UpdatedBy_Id, DatasetXml_Id)
			VALUES ('obtaindrugcountry', 3, NULL, GETDATE(), GETDATE(), @dsxcnid3, 1, @desid, 1, @dsxid)
		set @dsxenid = (SELECT @@IDENTITY)

		INSERT [dbo].[Field] ([Anonymise], [Mandatory], [MaxLength], [RegEx], [FieldType_Id]) 
			VALUES (0, 0, 35, NULL, 3)
		set @fid = (SELECT @@IDENTITY)
		INSERT [dbo].[DatasetElementSub] ([ElementName], [Field_Id], [DatasetElement_Id], [OID], [DefaultValue], [System], FieldOrder)
			VALUES ('Batch Number', @fid, @deid, '', '', 0, 1) 
		set @desid = (SELECT @@IDENTITY)
		-- element node
		INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElementSub_Id, UpdatedBy_Id, DatasetXml_Id)
			VALUES ('drugbatchnumb', 3, NULL, GETDATE(), GETDATE(), @dsxcnid3, 1, @desid, 1, @dsxid)
		set @dsxenid = (SELECT @@IDENTITY)
		-- submapping
		INSERT [dbo].[DatasetMappingSub] (MappingType, MappingOption, DestinationElement_Id, Mapping_Id, SourceElement_Id)
			VALUES (0, '', @desid, @dmid, (select top 1 desu.Id from [dbo].[DatasetElementSub] desu inner join [dbo].[DatasetElement] de on desu.DatasetElement_Id = de.Id where de.ElementName = 'Product Information' and desu.ElementName = 'Product batch Number'))
		set @dmsid = (SELECT @@IDENTITY)

		INSERT [dbo].[Field] ([Anonymise], [Mandatory], [MaxLength], [RegEx], [FieldType_Id]) 
			VALUES (0, 0, 35, NULL, 3)
		set @fid = (SELECT @@IDENTITY)
		INSERT [dbo].[DatasetElementSub] ([ElementName], [Field_Id], [DatasetElement_Id], [OID], [DefaultValue], [System], FieldOrder)
			VALUES ('Authorization Number', @fid, @deid, '', '', 0, 1) 
		set @desid = (SELECT @@IDENTITY)
		-- element node
		INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElementSub_Id, UpdatedBy_Id, DatasetXml_Id)
			VALUES ('drugauthorizationnumb', 3, NULL, GETDATE(), GETDATE(), @dsxcnid3, 1, @desid, 1, @dsxid)
		set @dsxenid = (SELECT @@IDENTITY)

		INSERT [dbo].[Field] ([Anonymise], [Mandatory], [MaxLength], [RegEx], [FieldType_Id]) 
			VALUES (0, 0, 2, NULL, 3)
		set @fid = (SELECT @@IDENTITY)
		INSERT [dbo].[DatasetElementSub] ([ElementName], [Field_Id], [DatasetElement_Id], [OID], [DefaultValue], [System], FieldOrder)
			VALUES ('Authorization Country', @fid, @deid, '', '', 0, 1) 
		set @desid = (SELECT @@IDENTITY)
		-- element node
		INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElementSub_Id, UpdatedBy_Id, DatasetXml_Id)
			VALUES ('drugauthorizationcountry', 3, NULL, GETDATE(), GETDATE(), @dsxcnid3, 1, @desid, 1, @dsxid)
		set @dsxenid = (SELECT @@IDENTITY)

		INSERT [dbo].[Field] ([Anonymise], [Mandatory], [MaxLength], [RegEx], [FieldType_Id]) 
			VALUES (0, 0, 60, NULL, 3)
		set @fid = (SELECT @@IDENTITY)
		INSERT [dbo].[DatasetElementSub] ([ElementName], [Field_Id], [DatasetElement_Id], [OID], [DefaultValue], [System], FieldOrder)
			VALUES ('Authorization Holder', @fid, @deid, '', '', 0, 1) 
		set @desid = (SELECT @@IDENTITY)
		-- element node
		INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElementSub_Id, UpdatedBy_Id, DatasetXml_Id)
			VALUES ('drugauthorizationholder', 3, NULL, GETDATE(), GETDATE(), @dsxcnid3, 1, @desid, 1, @dsxid)
		set @dsxenid = (SELECT @@IDENTITY)

		INSERT [dbo].[Field] ([Anonymise], [Mandatory], [Decimals], [MaxSize], [MinSize], [FieldType_Id]) 
			VALUES (0, 0, 0, 99999999, 1, 4)
		set @fid = (SELECT @@IDENTITY)
		INSERT [dbo].[DatasetElementSub] ([ElementName], [Field_Id], [DatasetElement_Id], [OID], [DefaultValue], [System], FieldOrder)
			VALUES ('Structured Dosage', @fid, @deid, '', '', 0, 1) 
		set @desid = (SELECT @@IDENTITY)
		-- element node
		INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElementSub_Id, UpdatedBy_Id, DatasetXml_Id)
			VALUES ('drugstructuredosagenumb', 3, NULL, GETDATE(), GETDATE(), @dsxcnid3, 1, @desid, 1, @dsxid)
		set @dsxenid = (SELECT @@IDENTITY)
		-- submapping
		INSERT [dbo].[DatasetMappingSub] (MappingType, MappingOption, DestinationElement_Id, Mapping_Id, SourceElement_Id)
			VALUES (0, '', @desid, @dmid, (select top 1 desu.Id from [dbo].[DatasetElementSub] desu inner join [dbo].[DatasetElement] de on desu.DatasetElement_Id = de.Id where de.ElementName = 'Product Information' and desu.ElementName = 'Dose Number'))
		set @dmsid = (SELECT @@IDENTITY)

		INSERT [dbo].[Field] ([Anonymise], [Mandatory], [FieldType_Id]) 
			VALUES (0, 0, 2)
		set @fid = (SELECT @@IDENTITY)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('001=kg kilogram(s)', 1, 0, 0, @fid)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('002=G gram(s)', 0, 0, 0, @fid)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('003=Mg milligram(s)', 0, 0, 0, @fid)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('004=ug microgram(s)', 0, 0, 0, @fid)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('005=ng nanogram(s)', 0, 0, 0, @fid)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('006=pg picogram(s)', 0, 1, 0, @fid)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('007=mg/kg milligram(s)/kilogram', 0, 1, 0, @fid)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('008=ug/kg microgram(s)/kilogram', 0, 1, 0, @fid)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('009=mg/m 2 milligram(s)/sq. meter', 0, 1, 0, @fid)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('010=ug/ m 2 microgram(s)/ sq. Meter', 0, 1, 0, @fid)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('011=l litre(s)', 0, 1, 0, @fid)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('012=ml millilitre(s)', 0, 1, 0, @fid)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('013=ul microlitre(s)', 0, 1, 0, @fid)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('014=Bq becquerel(s)', 0, 1, 0, @fid)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('015=GBq gigabecquerel(s)', 0, 1, 0, @fid)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('016=MBq megabecquerel(s)', 0, 1, 0, @fid)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('017=Kbq kilobecquerel(s)', 0, 1, 0, @fid)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('018=Ci curie(s)', 0, 1, 0, @fid)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('019=MCi millicurie(s)', 0, 1, 0, @fid)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('020=uCi microcurie(s)', 0, 1, 0, @fid)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('021=NCi nanocurie(s)', 0, 1, 0, @fid)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('022=Mol mole(s)', 0, 1, 0, @fid)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('023=Mmol millimole(s)', 0, 1, 0, @fid)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('024=umol micromole(s)', 0, 1, 0, @fid)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('025=Iu international unit(s)', 0, 1, 0, @fid)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('026=Kiu iu(1000s)', 0, 1, 0, @fid)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('027=Miu iu(1,000,000s)', 0, 1, 0, @fid)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('028=iu/kg iu/kilogram', 0, 1, 0, @fid)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('029=Meq milliequivalent(s)', 0, 1, 0, @fid)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('030=% percent', 0, 1, 0, @fid)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('031=Gtt drop(s)', 0, 1, 0, @fid)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('032=DF dosage form', 0, 1, 0, @fid)
		INSERT [dbo].[DatasetElementSub] ([ElementName], [Field_Id], [DatasetElement_Id], [OID], [DefaultValue], [System], FieldOrder)
			VALUES ('Structured Dosage Unit', @fid, @deid, '', '', 0, 1) 
		set @desid = (SELECT @@IDENTITY)
		-- element node
		INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElementSub_Id, UpdatedBy_Id, DatasetXml_Id)
			VALUES ('drugstructuredosageunit', 3, NULL, GETDATE(), GETDATE(), @dsxcnid3, 1, @desid, 1, @dsxid)
		set @dsxenid = (SELECT @@IDENTITY)
		-- submapping
		INSERT [dbo].[DatasetMappingSub] (MappingType, MappingOption, DestinationElement_Id, Mapping_Id, SourceElement_Id)
			VALUES (1, '', @desid, @dmid, (select top 1 desu.Id from [dbo].[DatasetElementSub] desu inner join [dbo].[DatasetElement] de on desu.DatasetElement_Id = de.Id where de.ElementName = 'Product Information' and desu.ElementName = 'Dose Unit'))
		set @dmsid = (SELECT @@IDENTITY)
		-- mapping values
		INSERT [dbo].[DatasetMappingValue] (SourceValue, DestinationValue, Active, Mapping_Id, SubMapping_Id)
			SELECT 'tablet(s)', '032=DF dosage form', 1, @dmid, @dmsid UNION
			SELECT 'capsule(s)', '032=DF dosage form', 1, @dmid, @dmsid UNION
			SELECT 'drop(s)', '031=Gtt drop(s)', 1, @dmid, @dmsid UNION
			SELECT 'teaspoon(s)', '032=DF dosage form', 1, @dmid, @dmsid UNION
			SELECT 'tablespoon(s)', '032=DF dosage form', 1, @dmid, @dmsid UNION
			SELECT 'milliliter(s)', '012=ml millilitre(s)', 1, @dmid, @dmsid UNION
			SELECT 'suppository(ies)', '032=DF dosage form', 1, @dmid, @dmsid UNION
			SELECT 'injection(s)', '032=DF dosage form', 1, @dmid, @dmsid UNION
			SELECT 'puff(s)', '032=DF dosage form', 1, @dmid, @dmsid UNION
			SELECT 'inhalation(s)', '032=DF dosage form', 1, @dmid, @dmsid UNION
			SELECT 'patch(es)', '032=DF dosage form', 1, @dmid, @dmsid UNION
			SELECT 'Other', '032=DF dosage form', 1, @dmid, @dmsid UNION
			SELECT 'gram(s)', '002=G gram(s)', 1, @dmid, @dmsid UNION
			SELECT 'milligram(s)', '003=Mg milligram(s)', 1, @dmid, @dmsid UNION
			SELECT 'Milligram/m(sqr)', '009=mg/m 2 milligram(s)/sq. meter', 1, @dmid, @dmsid

		INSERT [dbo].[Field] ([Anonymise], [Mandatory], [Decimals], [MaxSize], [MinSize], [FieldType_Id]) 
			VALUES (0, 0, 0, 999, 1, 4)
		set @fid = (SELECT @@IDENTITY)
		INSERT [dbo].[DatasetElementSub] ([ElementName], [Field_Id], [DatasetElement_Id], [OID], [DefaultValue], [System], FieldOrder)
			VALUES ('Number Seperate Dosages', @fid, @deid, '', '', 0, 1) 
		set @desid = (SELECT @@IDENTITY)
		-- element node
		INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElementSub_Id, UpdatedBy_Id, DatasetXml_Id)
			VALUES ('drugseparatedosagenumb', 3, NULL, GETDATE(), GETDATE(), @dsxcnid3, 1, @desid, 1, @dsxid)
		set @dsxenid = (SELECT @@IDENTITY)

		INSERT [dbo].[Field] ([Anonymise], [Mandatory], [Decimals], [MaxSize], [MinSize], [FieldType_Id]) 
			VALUES (0, 0, 0, 999, 1, 4)
		set @fid = (SELECT @@IDENTITY)
		INSERT [dbo].[DatasetElementSub] ([ElementName], [Field_Id], [DatasetElement_Id], [OID], [DefaultValue], [System], FieldOrder)
			VALUES ('Number Units In Interval', @fid, @deid, '', '', 0, 1) 
		set @desid = (SELECT @@IDENTITY)
		-- element node
		INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElementSub_Id, UpdatedBy_Id, DatasetXml_Id)
			VALUES ('drugintervaldosageunitnumb', 3, NULL, GETDATE(), GETDATE(), @dsxcnid3, 1, @desid, 1, @dsxid)
		set @dsxenid = (SELECT @@IDENTITY)

		INSERT [dbo].[Field] ([Anonymise], [Mandatory], [FieldType_Id]) 
			VALUES (0, 0, 2)
		set @fid = (SELECT @@IDENTITY)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('800=Decade', 1, 0, 0, @fid)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('801=Year', 0, 0, 0, @fid)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('802=Month', 0, 0, 0, @fid)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('803=Week', 0, 0, 0, @fid)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('804=Day', 0, 0, 0, @fid)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('805=Hour', 0, 0, 0, @fid)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('806=Minute', 0, 0, 0, @fid)
		INSERT [dbo].[DatasetElementSub] ([ElementName], [Field_Id], [DatasetElement_Id], [OID], [DefaultValue], [System], FieldOrder)
			VALUES ('Interval Definition', @fid, @deid, '', '', 0, 1) 
		set @desid = (SELECT @@IDENTITY)
		-- element node
		INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElementSub_Id, UpdatedBy_Id, DatasetXml_Id)
			VALUES ('drugintervaldosagedefinition', 3, NULL, GETDATE(), GETDATE(), @dsxcnid3, 1, @desid, 1, @dsxid)
		set @dsxenid = (SELECT @@IDENTITY)

		INSERT [dbo].[Field] ([Anonymise], [Mandatory], [Decimals], [MaxSize], [MinSize], [FieldType_Id]) 
			VALUES (0, 0, 0, 9999999999, 1, 4)
		set @fid = (SELECT @@IDENTITY)
		INSERT [dbo].[DatasetElementSub] ([ElementName], [Field_Id], [DatasetElement_Id], [OID], [DefaultValue], [System], FieldOrder)
			VALUES ('Cumulative Dose to First Number', @fid, @deid, '', '', 0, 1) 
		set @desid = (SELECT @@IDENTITY)
		-- element node
		INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElementSub_Id, UpdatedBy_Id, DatasetXml_Id)
			VALUES ('drugcumulativedosagenumb', 3, NULL, GETDATE(), GETDATE(), @dsxcnid3, 1, @desid, 1, @dsxid)
		set @dsxenid = (SELECT @@IDENTITY)

		INSERT [dbo].[Field] ([Anonymise], [Mandatory], [Decimals], [MaxSize], [MinSize], [FieldType_Id]) 
			VALUES (0, 0, 0, 999, 1, 4)
		set @fid = (SELECT @@IDENTITY)
		INSERT [dbo].[DatasetElementSub] ([ElementName], [Field_Id], [DatasetElement_Id], [OID], [DefaultValue], [System], FieldOrder)
			VALUES ('Cumulative Dose to First Unit', @fid, @deid, '', '', 0, 1) 
		set @desid = (SELECT @@IDENTITY)
		-- element node
		INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElementSub_Id, UpdatedBy_Id, DatasetXml_Id)
			VALUES ('drugcumulativedosageunit', 3, NULL, GETDATE(), GETDATE(), @dsxcnid3, 1, @desid, 1, @dsxid)
		set @dsxenid = (SELECT @@IDENTITY)

		INSERT [dbo].[Field] ([Anonymise], [Mandatory], [MaxLength], [RegEx], [FieldType_Id]) 
			VALUES (0, 0, 100, NULL, 3)
		set @fid = (SELECT @@IDENTITY)
		INSERT [dbo].[DatasetElementSub] ([ElementName], [Field_Id], [DatasetElement_Id], [OID], [DefaultValue], [System], FieldOrder)
			VALUES ('Drug Dosage Text', @fid, @deid, '', '', 0, 1) 
		set @desid = (SELECT @@IDENTITY)
		-- element node
		INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElementSub_Id, UpdatedBy_Id, DatasetXml_Id)
			VALUES ('drugdosagetext', 3, NULL, GETDATE(), GETDATE(), @dsxcnid3, 1, @desid, 1, @dsxid)
		set @dsxenid = (SELECT @@IDENTITY)

		INSERT [dbo].[Field] ([Anonymise], [Mandatory], [MaxLength], [RegEx], [FieldType_Id]) 
			VALUES (0, 0, 50, NULL, 3)
		set @fid = (SELECT @@IDENTITY)
		INSERT [dbo].[DatasetElementSub] ([ElementName], [Field_Id], [DatasetElement_Id], [OID], [DefaultValue], [System], FieldOrder)
			VALUES ('Drug Dosage Form', @fid, @deid, '', '', 0, 1) 
		set @desid = (SELECT @@IDENTITY)
		-- element node
		INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElementSub_Id, UpdatedBy_Id, DatasetXml_Id)
			VALUES ('drugdosageform', 3, NULL, GETDATE(), GETDATE(), @dsxcnid3, 1, @desid, 1, @dsxid)
		set @dsxenid = (SELECT @@IDENTITY)

		INSERT [dbo].[Field] ([Anonymise], [Mandatory], [FieldType_Id]) 
			VALUES (0, 0, 2)
		set @fid = (SELECT @@IDENTITY)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('001=Auricular (otic)', 1, 0, 0, @fid)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('002=Buccal', 0, 0, 0, @fid)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('003=Cutaneous', 0, 0, 0, @fid)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('004=Dental', 0, 0, 0, @fid)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('005=Endocervical', 0, 0, 0, @fid)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('006=Endosinusial', 0, 0, 0, @fid)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('007=Endotracheal', 0, 0, 0, @fid)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('008=Epidural', 0, 0, 0, @fid)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('009=Extra-amniotic', 0, 0, 0, @fid)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('010=Hemodialysis', 0, 0, 0, @fid)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('011=Intra corpus cavernosum', 0, 0, 0, @fid)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('012=Intra-amniotic', 0, 0, 0, @fid)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('013=Intra-arterial', 0, 0, 0, @fid)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('014=Intra-articular', 0, 0, 0, @fid)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('015=Intra-uterine', 0, 0, 0, @fid)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('016=Intracardiac', 0, 0, 0, @fid)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('017=Intracavernous', 0, 0, 0, @fid)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('018=Intracerebral', 0, 0, 0, @fid)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('019=Intracervical', 0, 0, 0, @fid)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('020=Intracisternal', 0, 0, 0, @fid)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('021=Intracorneal', 0, 0, 0, @fid)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('022=Intracoronary', 0, 0, 0, @fid)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('023=Intradermal', 0, 0, 0, @fid)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('024=Intradiscal (intraspinal)', 0, 0, 0, @fid)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('025=Intrahepatic', 0, 0, 0, @fid)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('026=Intralesional', 0, 0, 0, @fid)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('027=Intralymphatic', 0, 0, 0, @fid)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('028=Intramedullar (bone marrow)', 0, 0, 0, @fid)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('029=Intrameningeal', 0, 0, 0, @fid)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('030=Intramuscular', 0, 0, 0, @fid)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('031=Intraocular', 0, 0, 0, @fid)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('032=Intrapericardial', 0, 0, 0, @fid)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('033=Intraperitoneal', 0, 0, 0, @fid)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('034=Intrapleural', 0, 0, 0, @fid)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('035=Intrasynovial', 0, 0, 0, @fid)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('036=Intratumor', 0, 0, 0, @fid)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('037=Intrathecal', 0, 0, 0, @fid)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('038=Intrathoracic', 0, 0, 0, @fid)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('039=Intratracheal', 0, 0, 0, @fid)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('040=Intravenous bolus', 0, 0, 0, @fid)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('041=Intravenous drip', 0, 0, 0, @fid)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('042=Intravenous (not otherwise specified)', 0, 0, 0, @fid)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('043=Intravesical', 0, 0, 0, @fid)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('044=Iontophoresis', 0, 0, 0, @fid)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('045=Nasal', 0, 0, 0, @fid)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('046=Occlusive dressing technique', 0, 0, 0, @fid)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('047=Ophthalmic', 0, 0, 0, @fid)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('048=Oral', 0, 0, 0, @fid)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('049=Oropharingeal', 0, 0, 0, @fid)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('050=Other', 0, 0, 0, @fid)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('051=Parenteral', 0, 0, 0, @fid)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('052=Periarticular', 0, 0, 0, @fid)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('053=Perineural', 0, 0, 0, @fid)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('054=Rectal', 0, 0, 0, @fid)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('055=Respiratory (inhalation)', 0, 0, 0, @fid)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('056=Retrobulbar', 0, 0, 0, @fid)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('057=Sunconjunctival', 0, 0, 0, @fid)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('058=Subcutaneous', 0, 0, 0, @fid)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('059=Subdermal', 0, 0, 0, @fid)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('060=Sublingual', 0, 0, 0, @fid)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('061=Topical', 0, 0, 0, @fid)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('062=Transdermal', 0, 0, 0, @fid)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('063=Transmammary', 0, 0, 0, @fid)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('064=Transplacental', 0, 0, 0, @fid)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('065=Unknown', 0, 0, 0, @fid)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('066=Urethral', 0, 0, 0, @fid)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('067=Vaginal', 0, 0, 0, @fid)
		INSERT [dbo].[DatasetElementSub] ([ElementName], [Field_Id], [DatasetElement_Id], [OID], [DefaultValue], [System], FieldOrder)
			VALUES ('Drug Administration Route', @fid, @deid, '', '', 0, 1) 
		set @desid = (SELECT @@IDENTITY)
		-- element node
		INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElementSub_Id, UpdatedBy_Id, DatasetXml_Id)
			VALUES ('drugadministrationroute', 3, NULL, GETDATE(), GETDATE(), @dsxcnid3, 1, @desid, 1, @dsxid)
		set @dsxenid = (SELECT @@IDENTITY)
		-- submapping
		INSERT [dbo].[DatasetMappingSub] (MappingType, MappingOption, DestinationElement_Id, Mapping_Id, SourceElement_Id)
			VALUES (1, '', @desid, @dmid, (select top 1 desu.Id from [dbo].[DatasetElementSub] desu inner join [dbo].[DatasetElement] de on desu.DatasetElement_Id = de.Id where de.ElementName = 'Product Information' and desu.ElementName = 'Drug route of administration'))
		set @dmsid = (SELECT @@IDENTITY)
		-- mapping values
		INSERT [dbo].[DatasetMappingValue] (SourceValue, DestinationValue, Active, Mapping_Id, SubMapping_Id)
			SELECT 'By mouth', '048=Oral', 1, @dmid, @dmsid UNION
			SELECT 'Taken under the tongue', '060=Sublingual', 1, @dmid, @dmsid UNION
			SELECT 'Applied to a surface, usually skin', '003=Cutaneous', 1, @dmid, @dmsid UNION
			SELECT 'Inhalation', '065=Unknown', 1, @dmid, @dmsid UNION
			SELECT 'Applied as a medicated patch to skin', '062=Transdermal', 1, @dmid, @dmsid UNION
			SELECT 'Given into/under the skin', '058=Subcutaneous', 1, @dmid, @dmsid UNION
			SELECT 'Into a vein', '042=Intravenous (not otherwise specified)', 1, @dmid, @dmsid UNION
			SELECT 'Into a muscle', '030=Intramuscular', 1, @dmid, @dmsid UNION
			SELECT 'Into the ear', '001=Auricular (otic)', 1, @dmid, @dmsid UNION
			SELECT 'Into the eye', '031=Intraocular', 1, @dmid, @dmsid UNION
			SELECT 'Rectal', '054=Rectal', 1, @dmid, @dmsid UNION
			SELECT 'Vaginal', '067=Vaginal', 1, @dmid, @dmsid UNION
			SELECT 'Other', '050=Other', 1, @dmid, @dmsid		

		INSERT [dbo].[Field] ([Anonymise], [Mandatory], [FieldType_Id]) 
			VALUES (0, 0, 2)
		set @fid = (SELECT @@IDENTITY)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('001=Auricular (otic)', 1, 0, 0, @fid)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('002=Buccal', 0, 0, 0, @fid)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('003=Cutaneous', 0, 0, 0, @fid)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('004=Dental', 0, 0, 0, @fid)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('005=Endocervical', 0, 0, 0, @fid)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('006=Endosinusial', 0, 0, 0, @fid)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('007=Endotracheal', 0, 0, 0, @fid)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('008=Epidural', 0, 0, 0, @fid)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('009=Extra-amniotic', 0, 0, 0, @fid)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('010=Hemodialysis', 0, 0, 0, @fid)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('011=Intra corpus cavernosum', 0, 0, 0, @fid)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('012=Intra-amniotic', 0, 0, 0, @fid)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('013=Intra-arterial', 0, 0, 0, @fid)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('014=Intra-articular', 0, 0, 0, @fid)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('015=Intra-uterine', 0, 0, 0, @fid)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('016=Intracardiac', 0, 0, 0, @fid)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('017=Intracavernous', 0, 0, 0, @fid)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('018=Intracerebral', 0, 0, 0, @fid)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('019=Intracervical', 0, 0, 0, @fid)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('020=Intracisternal', 0, 0, 0, @fid)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('021=Intracorneal', 0, 0, 0, @fid)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('022=Intracoronary', 0, 0, 0, @fid)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('023=Intradermal', 0, 0, 0, @fid)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('024=Intradiscal (intraspinal)', 0, 0, 0, @fid)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('025=Intrahepatic', 0, 0, 0, @fid)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('026=Intralesional', 0, 0, 0, @fid)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('027=Intralymphatic', 0, 0, 0, @fid)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('028=Intramedullar (bone marrow)', 0, 0, 0, @fid)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('029=Intrameningeal', 0, 0, 0, @fid)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('030=Intramuscular', 0, 0, 0, @fid)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('031=Intraocular', 0, 0, 0, @fid)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('032=Intrapericardial', 0, 0, 0, @fid)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('033=Intraperitoneal', 0, 0, 0, @fid)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('034=Intrapleural', 0, 0, 0, @fid)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('035=Intrasynovial', 0, 0, 0, @fid)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('036=Intratumor', 0, 0, 0, @fid)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('037=Intrathecal', 0, 0, 0, @fid)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('038=Intrathoracic', 0, 0, 0, @fid)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('039=Intratracheal', 0, 0, 0, @fid)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('040=Intravenous bolus', 0, 0, 0, @fid)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('041=Intravenous drip', 0, 0, 0, @fid)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('042=Intravenous (not otherwise specified)', 0, 0, 0, @fid)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('043=Intravesical', 0, 0, 0, @fid)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('044=Iontophoresis', 0, 0, 0, @fid)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('045=Nasal', 0, 0, 0, @fid)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('046=Occlusive dressing technique', 0, 0, 0, @fid)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('047=Ophthalmic', 0, 0, 0, @fid)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('048=Oral', 0, 0, 0, @fid)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('049=Oropharingeal', 0, 0, 0, @fid)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('050=Other', 0, 0, 0, @fid)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('051=Parenteral', 0, 0, 0, @fid)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('052=Periarticular', 0, 0, 0, @fid)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('053=Perineural', 0, 0, 0, @fid)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('054=Rectal', 0, 0, 0, @fid)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('055=Respiratory (inhalation)', 0, 0, 0, @fid)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('056=Retrobulbar', 0, 0, 0, @fid)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('057=Sunconjunctival', 0, 0, 0, @fid)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('058=Subcutaneous', 0, 0, 0, @fid)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('059=Subdermal', 0, 0, 0, @fid)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('060=Sublingual', 0, 0, 0, @fid)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('061=Topical', 0, 0, 0, @fid)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('062=Transdermal', 0, 0, 0, @fid)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('063=Transmammary', 0, 0, 0, @fid)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('064=Transplacental', 0, 0, 0, @fid)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('065=Unknown', 0, 0, 0, @fid)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('066=Urethral', 0, 0, 0, @fid)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('067=Vaginal', 0, 0, 0, @fid)
		INSERT [dbo].[DatasetElementSub] ([ElementName], [Field_Id], [DatasetElement_Id], [OID], [DefaultValue], [System], FieldOrder)
			VALUES ('Drug Paradministration', @fid, @deid, '', '', 0, 1) 
		set @desid = (SELECT @@IDENTITY)
		-- element node
		INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElementSub_Id, UpdatedBy_Id, DatasetXml_Id)
			VALUES ('drugparadministration', 3, NULL, GETDATE(), GETDATE(), @dsxcnid3, 1, @desid, 1, @dsxid)
		set @dsxenid = (SELECT @@IDENTITY)

		INSERT [dbo].[Field] ([Anonymise], [Mandatory], [Decimals], [MaxSize], [MinSize], [FieldType_Id]) 
			VALUES (0, 0, 0, 999, 1, 4)
		set @fid = (SELECT @@IDENTITY)
		INSERT [dbo].[DatasetElementSub] ([ElementName], [Field_Id], [DatasetElement_Id], [OID], [DefaultValue], [System], FieldOrder)
			VALUES ('Reaction Gestation Period', @fid, @deid, '', '', 0, 1) 
		set @desid = (SELECT @@IDENTITY)
		-- element node
		INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElementSub_Id, UpdatedBy_Id, DatasetXml_Id)
			VALUES ('reactiongestationperiod', 3, NULL, GETDATE(), GETDATE(), @dsxcnid3, 1, @desid, 1, @dsxid)
		set @dsxenid = (SELECT @@IDENTITY)

		INSERT [dbo].[Field] ([Anonymise], [Mandatory], [FieldType_Id]) 
			VALUES (0, 0, 2)
		set @fid = (SELECT @@IDENTITY)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('802=Month', 0, 0, 0, @fid)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('803=Week', 0, 0, 0, @fid)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('804=Day', 0, 0, 0, @fid)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('810=Trimester', 0, 1, 0, @fid)
		INSERT [dbo].[DatasetElementSub] ([ElementName], [Field_Id], [DatasetElement_Id], [OID], [DefaultValue], [System], FieldOrder)
			VALUES ('Reaction Gestation Period Unit', @fid, @deid, '', '', 0, 1) 
		set @desid = (SELECT @@IDENTITY)
		-- element node
		INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElementSub_Id, UpdatedBy_Id, DatasetXml_Id)
			VALUES ('reactiongestationperiodunit', 3, NULL, GETDATE(), GETDATE(), @dsxcnid3, 1, @desid, 1, @dsxid)
		set @dsxenid = (SELECT @@IDENTITY)
			
		INSERT [dbo].[Field] ([Anonymise], [Mandatory], [MaxLength], [RegEx], [FieldType_Id]) 
			VALUES (0, 0, 10, NULL, 8)
		set @fid = (SELECT @@IDENTITY)
		INSERT [dbo].[DatasetElementSub] ([ElementName], [Field_Id], [DatasetElement_Id], [OID], [DefaultValue], [System], FieldOrder)
			VALUES ('Drug Indication MedDRA Version', @fid, @deid, '', 'v20', 0, 1) 
		set @desid = (SELECT @@IDENTITY)
		-- element node
		INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElementSub_Id, UpdatedBy_Id, DatasetXml_Id)
			VALUES ('drugindicationmeddraversion', 3, NULL, GETDATE(), GETDATE(), @dsxcnid3, 1, @desid, 1, @dsxid)
		set @dsxenid = (SELECT @@IDENTITY)

		INSERT [dbo].[Field] ([Anonymise], [Mandatory], [MaxLength], [RegEx], [FieldType_Id]) 
			VALUES (0, 0, 250, NULL, 3)
		set @fid = (SELECT @@IDENTITY)
		INSERT [dbo].[DatasetElementSub] ([ElementName], [Field_Id], [DatasetElement_Id], [OID], [DefaultValue], [System], FieldOrder)
			VALUES ('Drug Indication', @fid, @deid, '', '', 0, 1) 
		set @desid = (SELECT @@IDENTITY)
		-- element node
		INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElementSub_Id, UpdatedBy_Id, DatasetXml_Id)
			VALUES ('drugindication', 3, NULL, GETDATE(), GETDATE(), @dsxcnid3, 1, @desid, 1, @dsxid)
		set @dsxenid = (SELECT @@IDENTITY)
		-- submapping
		INSERT [dbo].[DatasetMappingSub] (MappingType, MappingOption, DestinationElement_Id, Mapping_Id, SourceElement_Id)
			VALUES (0, '', @desid, @dmid, (select top 1 desu.Id from [dbo].[DatasetElementSub] desu inner join [dbo].[DatasetElement] de on desu.DatasetElement_Id = de.Id where de.ElementName = 'Product Information' and desu.ElementName = 'Drug Indication'))
		set @dmsid = (SELECT @@IDENTITY)

		INSERT [dbo].[Field] ([Anonymise], [Mandatory], [MaxLength], [RegEx], [FieldType_Id]) 
			VALUES (0, 0, 10, NULL, 8)
		set @fid = (SELECT @@IDENTITY)
		INSERT [dbo].[DatasetElementSub] ([ElementName], [Field_Id], [DatasetElement_Id], [OID], [DefaultValue], [System], FieldOrder)
			VALUES ('Drug Start Date Format', @fid, @deid, '', '102', 0, 1) 
		set @desid = (SELECT @@IDENTITY)
		-- element node
		INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElementSub_Id, UpdatedBy_Id, DatasetXml_Id)
			VALUES ('drugstartdateformat', 3, NULL, GETDATE(), GETDATE(), @dsxcnid3, 1, @desid, 1, @dsxid)
		set @dsxenid = (SELECT @@IDENTITY)

		INSERT [dbo].[Field] ([Anonymise], [Mandatory], [FieldType_Id]) 
			VALUES (0, 0, 6)
		set @fid = (SELECT @@IDENTITY)
		INSERT [dbo].[DatasetElementSub] ([ElementName], [Field_Id], [DatasetElement_Id], [OID], [DefaultValue], [System], FieldOrder)
			VALUES ('Drug Start Date', @fid, @deid, '', '', 0, 1) 
		set @desid = (SELECT @@IDENTITY)
		-- element node
		INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElementSub_Id, UpdatedBy_Id, DatasetXml_Id)
			VALUES ('drugstartdate', 3, NULL, GETDATE(), GETDATE(), @dsxcnid3, 1, @desid, 1, @dsxid)
		set @dsxenid = (SELECT @@IDENTITY)
		-- submapping
		INSERT [dbo].[DatasetMappingSub] (MappingType, MappingOption, DestinationElement_Id, Mapping_Id, SourceElement_Id)
			VALUES (0, 'yyyyMMdd', @desid, @dmid, (select top 1 desu.Id from [dbo].[DatasetElementSub] desu inner join [dbo].[DatasetElement] de on desu.DatasetElement_Id = de.Id where de.ElementName = 'Product Information' and desu.ElementName = 'Drug Start Date'))
		set @dmsid = (SELECT @@IDENTITY)

		INSERT [dbo].[Field] ([Anonymise], [Mandatory], [Decimals], [MaxSize], [MinSize], [FieldType_Id]) 
			VALUES (0, 0, 0, 99999, 1, 4)
		set @fid = (SELECT @@IDENTITY)
		INSERT [dbo].[DatasetElementSub] ([ElementName], [Field_Id], [DatasetElement_Id], [OID], [DefaultValue], [System], FieldOrder)
			VALUES ('Drug Start Period', @fid, @deid, '', '', 0, 1) 
		set @desid = (SELECT @@IDENTITY)
		-- element node
		INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElementSub_Id, UpdatedBy_Id, DatasetXml_Id)
			VALUES ('drugstartperiod', 3, NULL, GETDATE(), GETDATE(), @dsxcnid3, 1, @desid, 1, @dsxid)
		set @dsxenid = (SELECT @@IDENTITY)

		INSERT [dbo].[Field] ([Anonymise], [Mandatory], [FieldType_Id]) 
			VALUES (0, 0, 2)
		set @fid = (SELECT @@IDENTITY)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('801=Year', 1, 0, 0, @fid)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('802=Month', 0, 0, 0, @fid)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('803=Week', 0, 0, 0, @fid)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('804=Day', 0, 0, 0, @fid)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('805=Hour', 0, 0, 0, @fid)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('806=Minute', 0, 0, 0, @fid)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('807=Second', 0, 0, 0, @fid)
		INSERT [dbo].[DatasetElementSub] ([ElementName], [Field_Id], [DatasetElement_Id], [OID], [DefaultValue], [System], FieldOrder)
			VALUES ('Drug Start Period Unit', @fid, @deid, '', '', 0, 1) 
		set @desid = (SELECT @@IDENTITY)
		-- element node
		INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElementSub_Id, UpdatedBy_Id, DatasetXml_Id)
			VALUES ('drugstartperiodunit', 3, NULL, GETDATE(), GETDATE(), @dsxcnid3, 1, @desid, 1, @dsxid)
		set @dsxenid = (SELECT @@IDENTITY)

		INSERT [dbo].[Field] ([Anonymise], [Mandatory], [Decimals], [MaxSize], [MinSize], [FieldType_Id]) 
			VALUES (0, 0, 0, 99999, 1, 4)
		set @fid = (SELECT @@IDENTITY)
		INSERT [dbo].[DatasetElementSub] ([ElementName], [Field_Id], [DatasetElement_Id], [OID], [DefaultValue], [System], FieldOrder)
			VALUES ('Drug Last Period', @fid, @deid, '', '', 0, 1) 
		set @desid = (SELECT @@IDENTITY)
		-- element node
		INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElementSub_Id, UpdatedBy_Id, DatasetXml_Id)
			VALUES ('druglastperiod', 3, NULL, GETDATE(), GETDATE(), @dsxcnid3, 1, @desid, 1, @dsxid)
		set @dsxenid = (SELECT @@IDENTITY)

		INSERT [dbo].[Field] ([Anonymise], [Mandatory], [FieldType_Id]) 
			VALUES (0, 0, 2)
		set @fid = (SELECT @@IDENTITY)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('801=Year', 1, 0, 0, @fid)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('802=Month', 0, 0, 0, @fid)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('803=Week', 0, 0, 0, @fid)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('804=Day', 0, 0, 0, @fid)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('805=Hour', 0, 0, 0, @fid)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('806=Minute', 0, 0, 0, @fid)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('807=Second', 0, 0, 0, @fid)
		INSERT [dbo].[DatasetElementSub] ([ElementName], [Field_Id], [DatasetElement_Id], [OID], [DefaultValue], [System], FieldOrder)
			VALUES ('Drug Last Period Unit', @fid, @deid, '', '', 0, 1) 
		set @desid = (SELECT @@IDENTITY)
		-- element node
		INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElementSub_Id, UpdatedBy_Id, DatasetXml_Id)
			VALUES ('druglastperiodunit', 3, NULL, GETDATE(), GETDATE(), @dsxcnid3, 1, @desid, 1, @dsxid)
		set @dsxenid = (SELECT @@IDENTITY)

		INSERT [dbo].[Field] ([Anonymise], [Mandatory], [MaxLength], [RegEx], [FieldType_Id]) 
			VALUES (0, 0, 10, NULL, 8)
		set @fid = (SELECT @@IDENTITY)
		INSERT [dbo].[DatasetElementSub] ([ElementName], [Field_Id], [DatasetElement_Id], [OID], [DefaultValue], [System], FieldOrder)
			VALUES ('Drug End Date Format', @fid, @deid, '', '102', 0, 1) 
		set @desid = (SELECT @@IDENTITY)
		-- element node
		INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElementSub_Id, UpdatedBy_Id, DatasetXml_Id)
			VALUES ('drugenddateformat', 3, NULL, GETDATE(), GETDATE(), @dsxcnid3, 1, @desid, 1, @dsxid)
		set @dsxenid = (SELECT @@IDENTITY)

		INSERT [dbo].[Field] ([Anonymise], [Mandatory], [FieldType_Id]) 
			VALUES (0, 0, 6)
		set @fid = (SELECT @@IDENTITY)
		INSERT [dbo].[DatasetElementSub] ([ElementName], [Field_Id], [DatasetElement_Id], [OID], [DefaultValue], [System], FieldOrder)
			VALUES ('Drug End Date', @fid, @deid, '', '', 0, 1) 
		set @desid = (SELECT @@IDENTITY)
		-- element node
		INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElementSub_Id, UpdatedBy_Id, DatasetXml_Id)
			VALUES ('drugenddate', 3, NULL, GETDATE(), GETDATE(), @dsxcnid3, 1, @desid, 1, @dsxid)
		set @dsxenid = (SELECT @@IDENTITY)
		-- submapping
		INSERT [dbo].[DatasetMappingSub] (MappingType, MappingOption, DestinationElement_Id, Mapping_Id, SourceElement_Id)
			VALUES (0, 'yyyyMMdd', @desid, @dmid, (select top 1 desu.Id from [dbo].[DatasetElementSub] desu inner join [dbo].[DatasetElement] de on desu.DatasetElement_Id = de.Id where de.ElementName = 'Product Information' and desu.ElementName = 'Drug End Date'))
		set @dmsid = (SELECT @@IDENTITY)

		INSERT [dbo].[Field] ([Anonymise], [Mandatory], [Decimals], [MaxSize], [MinSize], [FieldType_Id]) 
			VALUES (0, 0, 0, 99999, 0, 4)
		set @fid = (SELECT @@IDENTITY)
		INSERT [dbo].[DatasetElementSub] ([ElementName], [Field_Id], [DatasetElement_Id], [OID], [DefaultValue], [System], FieldOrder)
			VALUES ('Drug Treatment Duration', @fid, @deid, '', '', 0, 1) 
		set @desid = (SELECT @@IDENTITY)
		-- element node
		INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElementSub_Id, UpdatedBy_Id, DatasetXml_Id)
			VALUES ('drugtreatmentduration', 3, NULL, GETDATE(), GETDATE(), @dsxcnid3, 1, @desid, 1, @dsxid)
		set @dsxenid = (SELECT @@IDENTITY)

		INSERT [dbo].[Field] ([Anonymise], [Mandatory], [FieldType_Id]) 
			VALUES (0, 0, 2)
		set @fid = (SELECT @@IDENTITY)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('801=Year', 1, 0, 0, @fid)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('802=Month', 0, 0, 0, @fid)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('803=Week', 0, 0, 0, @fid)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('804=Day', 0, 0, 0, @fid)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('805=Hour', 0, 0, 0, @fid)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('806=Minute', 0, 0, 0, @fid)
		INSERT [dbo].[DatasetElementSub] ([ElementName], [Field_Id], [DatasetElement_Id], [OID], [DefaultValue], [System], FieldOrder)
			VALUES ('Drug Treatment Duration Unit', @fid, @deid, '', '', 0, 1) 
		set @desid = (SELECT @@IDENTITY)
		-- element node
		INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElementSub_Id, UpdatedBy_Id, DatasetXml_Id)
			VALUES ('drugtreatmentdurationunit', 3, NULL, GETDATE(), GETDATE(), @dsxcnid3, 1, @desid, 1, @dsxid)
		set @dsxenid = (SELECT @@IDENTITY)

		INSERT [dbo].[Field] ([Anonymise], [Mandatory], [FieldType_Id]) 
			VALUES (0, 0, 2)
		set @fid = (SELECT @@IDENTITY)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('1=Drug withdrawn', 1, 0, 0, @fid)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('2=Dose reduced', 0, 0, 0, @fid)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('3=Dose increased', 0, 0, 0, @fid)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('4=Dose not changed', 0, 0, 0, @fid)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('5=Unknown', 0, 0, 0, @fid)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('6=Not applicable', 0, 0, 0, @fid)
		INSERT [dbo].[DatasetElementSub] ([ElementName], [Field_Id], [DatasetElement_Id], [OID], [DefaultValue], [System], FieldOrder)
			VALUES ('Drug Action', @fid, @deid, '', '', 0, 1) 
		set @desid = (SELECT @@IDENTITY)
		-- element node
		INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElementSub_Id, UpdatedBy_Id, DatasetXml_Id)
			VALUES ('actiondrug', 3, NULL, GETDATE(), GETDATE(), @dsxcnid3, 1, @desid, 1, @dsxid)
		set @dsxenid = (SELECT @@IDENTITY)
		-- submapping
		INSERT [dbo].[DatasetMappingSub] (MappingType, MappingOption, DestinationElement_Id, Mapping_Id, SourceElement_Id)
			VALUES (1, '', @desid, @dmid, (select top 1 desu.Id from [dbo].[DatasetElementSub] desu inner join [dbo].[DatasetElement] de on desu.DatasetElement_Id = de.Id where de.ElementName = 'Product Information' and desu.ElementName = 'Actions taken with product'))
		set @dmsid = (SELECT @@IDENTITY)
		-- mapping values
		INSERT [dbo].[DatasetMappingValue] (SourceValue, DestinationValue, Active, Mapping_Id, SubMapping_Id)
			SELECT 'Product withdrawn', '1=Drug withdrawn', 1, @dmid, @dmsid UNION
			SELECT 'Dose reduced', '2=Dose reduced', 1, @dmid, @dmsid UNION
			SELECT 'Dose increased', '3=Dose increased', 1, @dmid, @dmsid UNION
			SELECT 'Dose not changed', '4=Dose not changed', 1, @dmid, @dmsid UNION
			SELECT 'Unknown', '5=Unknown', 1, @dmid, @dmsid UNION
			SELECT 'Not applicable', '6=Not applicable', 1, @dmid, @dmsid

		INSERT [dbo].[Field] ([Anonymise], [Mandatory], [FieldType_Id]) 
			VALUES (0, 0, 2)
		set @fid = (SELECT @@IDENTITY)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('1=Yes', 1, 0, 0, @fid)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('2=No', 0, 0, 0, @fid)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('3=Unknown', 0, 0, 0, @fid)
		INSERT [dbo].[DatasetElementSub] ([ElementName], [Field_Id], [DatasetElement_Id], [OID], [DefaultValue], [System], FieldOrder)
			VALUES ('Recurrence Administration', @fid, @deid, '', '', 0, 1) 
		set @desid = (SELECT @@IDENTITY)
		-- element node
		INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElementSub_Id, UpdatedBy_Id, DatasetXml_Id)
			VALUES ('drugrecurreadministration', 3, NULL, GETDATE(), GETDATE(), @dsxcnid3, 1, @desid, 1, @dsxid)
		set @dsxenid = (SELECT @@IDENTITY)

		INSERT [dbo].[Field] ([Anonymise], [Mandatory], [FieldType_Id]) 
			VALUES (0, 0, 2)
		set @fid = (SELECT @@IDENTITY)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('WHO Causality Scale', 1, 0, 0, @fid)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('Naranjo Causality Scale', 0, 0, 0, @fid)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id) 
			VALUES ('Unknown', 0, 0, 0, @fid)
		INSERT [dbo].[DatasetElementSub] ([ElementName], [Field_Id], [DatasetElement_Id], [OID], [DefaultValue], [System], FieldOrder)
			VALUES ('Source of Assessment', @fid, @deid, '', '', 0, 1) 
		set @desid = (SELECT @@IDENTITY)
		-- element node
		INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElementSub_Id, UpdatedBy_Id, DatasetXml_Id)
			VALUES ('drugassessmentsource', 3, NULL, GETDATE(), GETDATE(), @dsxcnid3, 1, @desid, 1, @dsxid)
		set @dsxenid = (SELECT @@IDENTITY)

		INSERT [dbo].[Field] ([Anonymise], [Mandatory], [MaxLength], [RegEx], [FieldType_Id]) 
			VALUES (0, 0, 35, NULL, 3)
		set @fid = (SELECT @@IDENTITY)
		INSERT [dbo].[DatasetElementSub] ([ElementName], [Field_Id], [DatasetElement_Id], [OID], [DefaultValue], [System], FieldOrder)
			VALUES ('Assessment Result', @fid, @deid, '', '', 0, 1) 
		set @desid = (SELECT @@IDENTITY)
		-- element node
		INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElementSub_Id, UpdatedBy_Id, DatasetXml_Id)
			VALUES ('drugresult', 3, NULL, GETDATE(), GETDATE(), @dsxcnid3, 1, @desid, 1, @dsxid)
		set @dsxenid = (SELECT @@IDENTITY)

		INSERT [dbo].[Field] ([Anonymise], [Mandatory], [MaxLength], [RegEx], [FieldType_Id]) 
			VALUES (0, 0, 250, NULL, 3)
		set @fid = (SELECT @@IDENTITY)
		INSERT [dbo].[DatasetElementSub] ([ElementName], [Field_Id], [DatasetElement_Id], [OID], [DefaultValue], [System], FieldOrder)
			VALUES ('Additional Information', @fid, @deid, '', '', 0, 1) 
		set @desid = (SELECT @@IDENTITY)
		-- element node
		INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElementSub_Id, UpdatedBy_Id, DatasetXml_Id)
			VALUES ('drugadditional', 3, NULL, GETDATE(), GETDATE(), @dsxcnid3, 1, @desid, 1, @dsxid)
		set @dsxenid = (SELECT @@IDENTITY)

/**************************************************
CATEGORY summary
**************************************************/
INSERT [dbo].[DatasetCategory] ([DatasetCategoryName], [CategoryOrder], [Dataset_Id]) 
	VALUES (N'Summary', 1, @dsid)
set @dscid = (SELECT @@IDENTITY)

-- category node
INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElement_Id, UpdatedBy_Id, DatasetXml_Id)
	VALUES ('summary', 2, NULL, GETDATE(), GETDATE(), @dsxcnid2, 1, NULL, 1, @dsxid)
set @dsxcnid3 = (SELECT @@IDENTITY)

INSERT [dbo].[Field] ([Anonymise], [Mandatory], [MaxLength], [RegEx], [FieldType_Id]) 
	VALUES (0, 0, 1000, NULL, 3)
set @fid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetElement] ([ElementName], [Field_Id], [DatasetElementType_Id], [OID], [DefaultValue], [System], [DatasetElementGuid]) 
	VALUES ('Narrative Include Clinical', @fid, 1, NULL, NULL, 0, NEWID())
set @deid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetCategoryElement] ([FieldOrder], [DatasetCategory_Id], [DatasetElement_Id], [Acute], [Chronic]) 
	select ISNULL(MAX(FieldOrder) + 1, 1), @dscid, @deid, 0, 0 from [DatasetCategoryElement] where [DatasetCategory_Id] = @dscid
set @dceid = (SELECT @@IDENTITY)
-- element node
INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElement_Id, UpdatedBy_Id, DatasetXml_Id)
	VALUES ('narrativeincludeclinical', 2, NULL, GETDATE(), GETDATE(), @dsxcnid3, 1, @deid, 1, @dsxid)
set @dsxenid = (SELECT @@IDENTITY)
-- mapping
INSERT [dbo].[DatasetMapping] (Tag, MappingType, MappingOption, DestinationElement_Id, PropertyPath, Property)
	VALUES ('Active', 2, '', @dceid, 'PatientClinicalEvent', 'Comments')

INSERT [dbo].[Field] ([Anonymise], [Mandatory], [MaxLength], [RegEx], [FieldType_Id]) 
	VALUES (0, 0, 500, NULL, 3)
set @fid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetElement] ([ElementName], [Field_Id], [DatasetElementType_Id], [OID], [DefaultValue], [System], [DatasetElementGuid]) 
	VALUES ('Reporter Comment', @fid, 1, NULL, NULL, 0, NEWID())
set @deid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetCategoryElement] ([FieldOrder], [DatasetCategory_Id], [DatasetElement_Id], [Acute], [Chronic]) 
	select ISNULL(MAX(FieldOrder) + 1, 1), @dscid, @deid, 0, 0 from [DatasetCategoryElement] where [DatasetCategory_Id] = @dscid
set @dceid = (SELECT @@IDENTITY)
-- element node
INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElement_Id, UpdatedBy_Id, DatasetXml_Id)
	VALUES ('reportercomment', 2, NULL, GETDATE(), GETDATE(), @dsxcnid3, 1, @deid, 1, @dsxid)
set @dsxenid = (SELECT @@IDENTITY)

INSERT [dbo].[Field] ([Anonymise], [Mandatory], [MaxLength], [RegEx], [FieldType_Id]) 
	VALUES (0, 0, 10, NULL, 8)
set @fid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetElement] ([ElementName], [Field_Id], [DatasetElementType_Id], [OID], [DefaultValue], [System], [DatasetElementGuid]) 
	VALUES ('Sender Diagnosis MedDRA Version', @fid, 1, NULL, '', 1, NEWID())
set @deid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetCategoryElement] ([FieldOrder], [DatasetCategory_Id], [DatasetElement_Id], [Acute], [Chronic]) 
	select ISNULL(MAX(FieldOrder) + 1, 1), @dscid, @deid, 0, 0 from [DatasetCategoryElement] where [DatasetCategory_Id] = @dscid
-- element node
INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElement_Id, UpdatedBy_Id, DatasetXml_Id)
	VALUES ('senderdiagnosismeddraversion', 2, NULL, GETDATE(), GETDATE(), @dsxcnid3, 1, @deid, 1, @dsxid)
set @dsxenid = (SELECT @@IDENTITY)

INSERT [dbo].[Field] ([Anonymise], [Mandatory], [MaxLength], [RegEx], [FieldType_Id]) 
	VALUES (0, 0, 250, NULL, 3)
set @fid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetElement] ([ElementName], [Field_Id], [DatasetElementType_Id], [OID], [DefaultValue], [System], [DatasetElementGuid]) 
	VALUES ('Sender Diagnosis', @fid, 1, NULL, NULL, 0, NEWID())
set @deid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetCategoryElement] ([FieldOrder], [DatasetCategory_Id], [DatasetElement_Id], [Acute], [Chronic]) 
	select ISNULL(MAX(FieldOrder) + 1, 1), @dscid, @deid, 0, 0 from [DatasetCategoryElement] where [DatasetCategory_Id] = @dscid
-- element node
INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElement_Id, UpdatedBy_Id, DatasetXml_Id)
	VALUES ('senderdiagnosis', 2, NULL, GETDATE(), GETDATE(), @dsxcnid3, 1, @deid, 1, @dsxid)
set @dsxenid = (SELECT @@IDENTITY)

INSERT [dbo].[Field] ([Anonymise], [Mandatory], [MaxLength], [RegEx], [FieldType_Id]) 
	VALUES (0, 0, 1000, NULL, 3)
set @fid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetElement] ([ElementName], [Field_Id], [DatasetElementType_Id], [OID], [DefaultValue], [System], [DatasetElementGuid]) 
	VALUES ('Sender Comment', @fid, 1, NULL, NULL, 0, NEWID())
set @deid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetCategoryElement] ([FieldOrder], [DatasetCategory_Id], [DatasetElement_Id], [Acute], [Chronic]) 
	select ISNULL(MAX(FieldOrder) + 1, 1), @dscid, @deid, 0, 0 from [DatasetCategoryElement] where [DatasetCategory_Id] = @dscid
-- element node
INSERT [dbo].[DatasetXmlNode] (NodeName, NodeType, NodeValue, Created, LastUpdated, ParentNode_Id, CreatedBy_Id, DatasetElement_Id, UpdatedBy_Id, DatasetXml_Id)
	VALUES ('sendercomment', 2, NULL, GETDATE(), GETDATE(), @dsxcnid3, 1, @deid, 1, @dsxid)
set @dsxenid = (SELECT @@IDENTITY)

select * from fieldtype
select * from dataset
select * from datasetxml
select * from datasetcategory where Dataset_Id = @dsid
select * from datasetxmlNode
select * from datasetxmlattribute
select * from datasetmapping

SELECT ds.DatasetName, dc.DatasetCategoryName, de.ElementName FROM Dataset ds
	INNER JOIN DatasetCategory dc ON ds.Id = dc.Dataset_Id 
	INNER JOIN DatasetCategoryElement dce ON dc.Id = dce.DatasetCategory_Id 
	INNER JOIN DatasetElement de ON dce.DatasetElement_Id = de.Id 
where ds.Id = @dsid
	
--ROLLBACK TRAN A1
COMMIT TRAN A1



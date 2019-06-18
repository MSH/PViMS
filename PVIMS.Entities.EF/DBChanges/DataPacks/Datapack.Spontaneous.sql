DECLARE @Id int
SELECT @Id = Id   FROM Dataset  WHERE DatasetName = 'Spontaneous Report'

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
DECLARE @dscid int
DECLARE @fid int
DECLARE @deid int
DECLARE @dceid int
INSERT [dbo].[Dataset] ([DatasetName], [Active], [InitialiseProcess], [RulesProcess], [Help], [Created], [LastUpdated], [ContextType_Id], [CreatedBy_Id], [UpdatedBy_Id])
	VALUES ('Spontaneous Report', 1, '', '', 'Suspected adverse drug reaction (ADR) online reporting form', '2017-04-11 04:11', '2017-04-11 04:11', 4, 1, 1) 
set @dsid = (SELECT @@IDENTITY)

/**************************************************
CATEGORY Patient Information
**************************************************/
INSERT [dbo].[DatasetCategory] ([DatasetCategoryName], [CategoryOrder], [Dataset_Id], [FriendlyName], [Help])
	VALUES ('Patient Information', 1, @dsid, 'Patient Information', 'Please enter some information about the person who had the adverse reaction.') 
set @dscid = (SELECT @@IDENTITY)

-- Initials
INSERT [dbo].[Field] (Mandatory, MaxLength, Decimals, MaxSize, MinSize, Calculation, FileSize, FileExt, Anonymise, FieldType_Id)
	VALUES (1, 5, NULL, NULL, NULL, '', NULL, '', 0, 3) 
set @fid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetElement] ([ElementName], [Field_Id], [DatasetElementType_Id], [OID], [DefaultValue], [System])
	VALUES ('Initials', @fid, 1, '', '', 0) 
set @deid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetCategoryElement] ([FieldOrder], [DatasetCategory_Id], [DatasetElement_Id], [Acute], [Chronic], [FriendlyName], [Help])
	VALUES (1, @dscid, @deid, 0, 0, 'Initials of Patient', 'Enter patient''s initials here OR their ID number and type below.') 
set @dceid = (SELECT @@IDENTITY)

-- Identification Number
INSERT [dbo].[Field] (Mandatory, MaxLength, Decimals, MaxSize, MinSize, Calculation, FileSize, FileExt, Anonymise, FieldType_Id)
	VALUES (0, 30, NULL, NULL, NULL, '', NULL, '', 0, 3) 
set @fid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetElement] ([ElementName], [Field_Id], [DatasetElementType_Id], [OID], [DefaultValue], [System])
	VALUES ('Identification Number', @fid, 1, '', '', 0) 
set @deid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetCategoryElement] ([FieldOrder], [DatasetCategory_Id], [DatasetElement_Id], [Acute], [Chronic], [FriendlyName], [Help])
	VALUES (2, @dscid, @deid, 1, 0, 'Identification Number', 'Enter patient''s ID number OR enter their initials above..') 
set @dceid = (SELECT @@IDENTITY)

-- Identification Type
INSERT [dbo].[Field] (Mandatory, MaxLength, Decimals, MaxSize, MinSize, Calculation, FileSize, FileExt, Anonymise, FieldType_Id)
	VALUES (0, NULL, NULL, NULL, NULL, '', NULL, '', 0, 2) 
set @fid = (SELECT @@IDENTITY)
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id)
	VALUES ('National Identity', 1, 0, 0, @fid) 
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id)
	VALUES ('Other', 0, 1, 0, @fid) 
INSERT [dbo].[DatasetElement] ([ElementName], [Field_Id], [DatasetElementType_Id], [OID], [DefaultValue], [System])
	VALUES ('Identification Type', @fid, 1, '', '', 0) 
set @deid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetCategoryElement] ([FieldOrder], [DatasetCategory_Id], [DatasetElement_Id], [Acute], [Chronic], [FriendlyName], [Help])
	VALUES (3, @dscid, @deid, 1, 0, 'Identification Type', 'If you entered a patient ID number, specify the ID type here.') 
set @dceid = (SELECT @@IDENTITY)

-- Date of Birth
INSERT [dbo].[Field] (Mandatory, MaxLength, Decimals, MaxSize, MinSize, Calculation, FileSize, FileExt, Anonymise, FieldType_Id)
	VALUES (0, NULL, NULL, NULL, NULL, '', NULL, '', 0, 6) 
set @fid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetElement] ([ElementName], [Field_Id], [DatasetElementType_Id], [OID], [DefaultValue], [System])
	VALUES ('Date of Birth', @fid, 1, '', '', 0) 
set @deid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetCategoryElement] ([FieldOrder], [DatasetCategory_Id], [DatasetElement_Id], [Acute], [Chronic], [FriendlyName], [Help])
	VALUES (4, @dscid, @deid, 1, 0, 'Patient Date of Birth', 'Enter the patient''s date of birth here OR enter their age below.') 
set @dceid = (SELECT @@IDENTITY)

-- Age
INSERT [dbo].[Field] (Mandatory, MaxLength, Decimals, MaxSize, MinSize, Calculation, FileSize, FileExt, Anonymise, FieldType_Id)
	VALUES (0, NULL, 0, 140.00, 0.00, '', NULL, '', 0, 4) 
set @fid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetElement] ([ElementName], [Field_Id], [DatasetElementType_Id], [OID], [DefaultValue], [System])
	VALUES ('Age', @fid, 1, '', '', 0) 
set @deid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetCategoryElement] ([FieldOrder], [DatasetCategory_Id], [DatasetElement_Id], [Acute], [Chronic], [FriendlyName], [Help])
	VALUES (5, @dscid, @deid, 1, 0, 'Age', 'Enter the patient''s age here OR enter their date of birth above.') 
set @dceid = (SELECT @@IDENTITY)

-- Age Unit
INSERT [dbo].[Field] (Mandatory, MaxLength, Decimals, MaxSize, MinSize, Calculation, FileSize, FileExt, Anonymise, FieldType_Id)
	VALUES (0, NULL, NULL, NULL, NULL, '', NULL, '', 0, 2) 
set @fid = (SELECT @@IDENTITY)
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id)
	VALUES ('Years', 1, 0, 0, @fid) 
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id)
	VALUES ('Months', 0, 0, 0, @fid) 
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id)
	VALUES ('Weeks', 0, 0, 0, @fid) 
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id)
	VALUES ('Days', 0, 0, 0, @fid) 
INSERT [dbo].[DatasetElement] ([ElementName], [Field_Id], [DatasetElementType_Id], [OID], [DefaultValue], [System])
	VALUES ('Age Unit', @fid, 1, '', '', 0) 
set @deid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetCategoryElement] ([FieldOrder], [DatasetCategory_Id], [DatasetElement_Id], [Acute], [Chronic], [FriendlyName], [Help])
	VALUES (6, @dscid, @deid, 1, 0, 'Age Unit of Measure', 'Enter weeks, months, or years for the patient''s age here.') 
set @dceid = (SELECT @@IDENTITY)

-- Weight (kg)
INSERT [dbo].[Field] (Mandatory, MaxLength, Decimals, MaxSize, MinSize, Calculation, FileSize, FileExt, Anonymise, FieldType_Id)
	VALUES (0, NULL, 1, 159.90, 1.10, '', NULL, '', 0, 4) 
set @fid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetElement] ([ElementName], [Field_Id], [DatasetElementType_Id], [OID], [DefaultValue], [System])
	VALUES ('Weight  (kg)', @fid, 1, '', '', 0) 
set @deid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetCategoryElement] ([FieldOrder], [DatasetCategory_Id], [DatasetElement_Id], [Acute], [Chronic], [FriendlyName], [Help])
	VALUES (7, @dscid, @deid, 1, 0, 'Patient''s weight (kg)', '') 
set @dceid = (SELECT @@IDENTITY)

-- Sex
INSERT [dbo].[Field] (Mandatory, MaxLength, Decimals, MaxSize, MinSize, Calculation, FileSize, FileExt, Anonymise, FieldType_Id)
	VALUES (0, NULL, NULL, NULL, NULL, '', NULL, '', 0, 2) 
set @fid = (SELECT @@IDENTITY)
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id)
	VALUES ('Male', 1, 0, 0, @fid) 
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id)
	VALUES ('Female', 0, 0, 0, @fid) 
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id)
	VALUES ('Unknown', 0, 0, 0, @fid) 
INSERT [dbo].[DatasetElement] ([ElementName], [Field_Id], [DatasetElementType_Id], [OID], [DefaultValue], [System])
	VALUES ('Sex', @fid, 1, '', '', 0) 
set @deid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetCategoryElement] ([FieldOrder], [DatasetCategory_Id], [DatasetElement_Id], [Acute], [Chronic], [FriendlyName], [Help])
	VALUES (8, @dscid, @deid, 0, 0, 'Sex', '') 
set @dceid = (SELECT @@IDENTITY)

-- Ethnic Group
INSERT [dbo].[Field] (Mandatory, MaxLength, Decimals, MaxSize, MinSize, Calculation, FileSize, FileExt, Anonymise, FieldType_Id)
	VALUES (0, NULL, NULL, NULL, NULL, '', NULL, '', 0, 2) 
set @fid = (SELECT @@IDENTITY)
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id)
	VALUES ('Asian', 1, 0, 0, @fid) 
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id)
	VALUES ('East Asian', 0, 0, 0, @fid) 
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id)
	VALUES ('South Asian', 0, 0, 0, @fid) 
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id)
	VALUES ('Southeast Asian', 0, 0, 0, @fid) 
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id)
	VALUES ('Black', 0, 0, 0, @fid) 
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id)
	VALUES ('White', 0, 0, 0, @fid) 
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id)
	VALUES ('Middle Eastern', 0, 0, 0, @fid) 
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id)
	VALUES ('Other', 0, 1, 0, @fid) 
INSERT [dbo].[DatasetElement] ([ElementName], [Field_Id], [DatasetElementType_Id], [OID], [DefaultValue], [System])
	VALUES ('Ethnic Group', @fid, 1, '', '', 0) 
set @deid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetCategoryElement] ([FieldOrder], [DatasetCategory_Id], [DatasetElement_Id], [Acute], [Chronic], [FriendlyName], [Help])
	VALUES (9, @dscid, @deid, 1, 0, 'Ethnic Group of Patient', '') 
set @dceid = (SELECT @@IDENTITY)

/**************************************************
CATEGORY Product Information
**************************************************/
INSERT [dbo].[DatasetCategory] ([DatasetCategoryName], [CategoryOrder], [Dataset_Id], [FriendlyName], [Help])
	VALUES ('Product Information', 2, @dsid, 'Product Information', 'Please enter information about the product you suspect caused the reaction and about other products taken.') 
set @dscid = (SELECT @@IDENTITY)

-- Product Information Table
INSERT [dbo].[Field] (Mandatory, MaxLength, Decimals, MaxSize, MinSize, Calculation, FileSize, FileExt, Anonymise, FieldType_Id)
	VALUES (0, NULL, NULL, NULL, NULL, '', NULL, '', 0, 7) 
set @fid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetElement] ([ElementName], [Field_Id], [DatasetElementType_Id], [OID], [DefaultValue], [System])
	VALUES ('Product Information', @fid, 1, '', '', 0) 
set @deid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetCategoryElement] ([FieldOrder], [DatasetCategory_Id], [DatasetElement_Id], [Acute], [Chronic], [FriendlyName], [Help])
	VALUES (10, @dscid, @deid, 0, 0, 'Name of product', 'Name of product.') 
set @dceid = (SELECT @@IDENTITY)

		-- Product
		INSERT [dbo].[Field] (Mandatory, MaxLength, Decimals, MaxSize, MinSize, Calculation, FileSize, FileExt, Anonymise, FieldType_Id)
			VALUES (1, 100, NULL, NULL, NULL, '', NULL, '', 0, 3) 
		set @fid = (SELECT @@IDENTITY)
		INSERT [dbo].[DatasetElementSub] ([ElementName], [Field_Id], [DatasetElement_Id], [OID], [DefaultValue], [System], FieldOrder)
			VALUES ('Product', @fid, @deid, '', '', 0, 1) 

		-- Product Suspected
		INSERT [dbo].[Field] (Mandatory, MaxLength, Decimals, MaxSize, MinSize, Calculation, FileSize, FileExt, Anonymise, FieldType_Id)
			VALUES (1, NULL, NULL, NULL, NULL, '', NULL, '', 0, 5) 
		set @fid = (SELECT @@IDENTITY)
		INSERT [dbo].[DatasetElementSub] ([ElementName], [Field_Id], [DatasetElement_Id], [OID], [DefaultValue], [System], FieldOrder)
			VALUES ('Product Suspected', @fid, @deid, '', '', 0, 2) 

		-- Drug strength
		INSERT [dbo].[Field] (Mandatory, MaxLength, Decimals, MaxSize, MinSize, Calculation, FileSize, FileExt, Anonymise, FieldType_Id)
			VALUES (0, NULL, 0, 99999999.00, 1.00, '', NULL, '', 0, 4) 
		set @fid = (SELECT @@IDENTITY)
		INSERT [dbo].[DatasetElementSub] ([ElementName], [Field_Id], [DatasetElement_Id], [OID], [DefaultValue], [System], FieldOrder)
			VALUES ('Drug Strength', @fid, @deid, '', '', 0, 3) 

		-- Drug strength unit
		INSERT [dbo].[Field] (Mandatory, MaxLength, Decimals, MaxSize, MinSize, Calculation, FileSize, FileExt, Anonymise, FieldType_Id)
			VALUES (0, NULL, NULL, NULL, NULL, '', NULL, '', 0, 2) 
		set @fid = (SELECT @@IDENTITY)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id)
			VALUES ('milligrams (mg)', 0, 0, 0, @fid) 
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id)
			VALUES ('milligrams/milliliters (mg/ml)', 0, 0, 0, @fid) 
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id)
			VALUES ('grams (gm)', 0, 0, 0, @fid) 
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id)
			VALUES ('kilograms (kg)', 0, 0, 0, @fid) 
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id)
			VALUES ('micrograms (mcg)', 0, 0, 0, @fid) 
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id)
			VALUES ('milliliters (ml)', 0, 0, 0, @fid) 
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id)
			VALUES ('liters (l)', 0, 0, 0, @fid) 
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id)
			VALUES ('milliequivalents (meq)', 0, 0, 0, @fid) 
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id)
			VALUES ('percent (%)', 0, 0, 0, @fid) 
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id)
			VALUES ('drops (gtt)', 0, 0, 0, @fid) 
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id)
			VALUES ('Other', 0, 1, 0, @fid) 
		INSERT [dbo].[DatasetElementSub] ([ElementName], [Field_Id], [DatasetElement_Id], [OID], [DefaultValue], [System], FieldOrder)
			VALUES ('Drug strength unit', @fid, @deid, '', '', 0, 4) 

		-- Dose Number
		INSERT [dbo].[Field] (Mandatory, MaxLength, Decimals, MaxSize, MinSize, Calculation, FileSize, FileExt, Anonymise, FieldType_Id)
			VALUES (0, NULL, 0, 99999999.00, 1.00, '', NULL, '', 0, 4) 
		set @fid = (SELECT @@IDENTITY)
		INSERT [dbo].[DatasetElementSub] ([ElementName], [Field_Id], [DatasetElement_Id], [OID], [DefaultValue], [System], FieldOrder)
			VALUES ('Dose Number', @fid, @deid, '', '', 0, 5) 

		-- Dose Unit
		INSERT [dbo].[Field] (Mandatory, MaxLength, Decimals, MaxSize, MinSize, Calculation, FileSize, FileExt, Anonymise, FieldType_Id)
			VALUES (0, NULL, NULL, NULL, NULL, '', NULL, '', 0, 2) 
		set @fid = (SELECT @@IDENTITY)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id)
			VALUES ('tablet(s)', 1, 0, 0, @fid) 
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id)
			VALUES ('capsule(s)', 0, 0, 0, @fid) 
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id)
			VALUES ('drop(s)', 0, 0, 0, @fid) 
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id)
			VALUES ('teaspoon(s)', 0, 0, 0, @fid) 
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id)
			VALUES ('tablespoon(s)', 0, 0, 0, @fid) 
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id)
			VALUES ('milliliter(s)', 0, 0, 0, @fid) 
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id)
			VALUES ('suppository(ies)', 0, 0, 0, @fid) 
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id)
			VALUES ('injection(s)', 0, 0, 0, @fid) 
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id)
			VALUES ('puff(s)', 0, 0, 0, @fid) 
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id)
			VALUES ('inhalation(s)', 0, 0, 0, @fid) 
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id)
			VALUES ('patch(es)', 0, 0, 0, @fid) 
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id)
			VALUES ('Other', 0, 1, 0, @fid) 
		INSERT [dbo].[DatasetElementSub] ([ElementName], [Field_Id], [DatasetElement_Id], [OID], [DefaultValue], [System], FieldOrder)
			VALUES ('Dose Unit', @fid, @deid, '', '', 0, 6) 

		-- Drug route of administration
		INSERT [dbo].[Field] (Mandatory, MaxLength, Decimals, MaxSize, MinSize, Calculation, FileSize, FileExt, Anonymise, FieldType_Id)
			VALUES (0, NULL, NULL, NULL, NULL, '', NULL, '', 0, 2) 
		set @fid = (SELECT @@IDENTITY)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id)
			VALUES ('By mouth', 0, 0, 0, @fid) 
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id)
			VALUES ('Taken under the tongue', 0, 0, 0, @fid) 
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id)
			VALUES ('Applied to a surface, usually skin', 0, 0, 0, @fid) 
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id)
			VALUES ('Inhalation', 0, 0, 0, @fid) 
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id)
			VALUES ('Applied as a medicated patch to skin', 0, 0, 0, @fid) 
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id)
			VALUES ('Given into/under the skin', 0, 0, 0, @fid) 
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id)
			VALUES ('Into a vein', 0, 0, 0, @fid) 
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id)
			VALUES ('Into a muscle', 0, 0, 0, @fid) 
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id)
			VALUES ('Into the ear', 0, 0, 0, @fid) 
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id)
			VALUES ('Into the eye', 0, 0, 0, @fid) 
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id)
			VALUES ('Rectal', 0, 0, 0, @fid) 
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id)
			VALUES ('Vaginal', 0, 0, 0, @fid) 
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id)
			VALUES ('Other', 0, 1, 0, @fid) 
		INSERT [dbo].[DatasetElementSub] ([ElementName], [Field_Id], [DatasetElement_Id], [OID], [DefaultValue], [System], FieldOrder)
			VALUES ('Drug route of administration', @fid, @deid, '', '', 0, 7) 

		-- Drug Start Date
		INSERT [dbo].[Field] (Mandatory, MaxLength, Decimals, MaxSize, MinSize, Calculation, FileSize, FileExt, Anonymise, FieldType_Id)
			VALUES (0, NULL, NULL, NULL, NULL, '', NULL, '', 0, 6) 
		set @fid = (SELECT @@IDENTITY)
		INSERT [dbo].[DatasetElementSub] ([ElementName], [Field_Id], [DatasetElement_Id], [OID], [DefaultValue], [System], FieldOrder)
			VALUES ('Drug Start Date', @fid, @deid, '', '', 0, 8) 

		-- Drug End Date
		INSERT [dbo].[Field] (Mandatory, MaxLength, Decimals, MaxSize, MinSize, Calculation, FileSize, FileExt, Anonymise, FieldType_Id)
			VALUES (0, NULL, NULL, NULL, NULL, '', NULL, '', 0, 6) 
		set @fid = (SELECT @@IDENTITY)
		INSERT [dbo].[DatasetElementSub] ([ElementName], [Field_Id], [DatasetElement_Id], [OID], [DefaultValue], [System], FieldOrder)
			VALUES ('Drug End Date', @fid, @deid, '', '', 0, 9) 

		-- Drug Treatment Duration
		INSERT [dbo].[Field] (Mandatory, MaxLength, Decimals, MaxSize, MinSize, Calculation, FileSize, FileExt, Anonymise, FieldType_Id)
			VALUES (0, NULL, 0, 99999.00, 1.00, '', NULL, '', 0, 4) 
		set @fid = (SELECT @@IDENTITY)
		INSERT [dbo].[DatasetElementSub] ([ElementName], [Field_Id], [DatasetElement_Id], [OID], [DefaultValue], [System], FieldOrder)
			VALUES ('Drug Treatment Duration', @fid, @deid, '', '', 0, 10) 

		-- Drug Treatment Duration Unit
		INSERT [dbo].[Field] (Mandatory, MaxLength, Decimals, MaxSize, MinSize, Calculation, FileSize, FileExt, Anonymise, FieldType_Id)
			VALUES (0, NULL, NULL, NULL, NULL, '', NULL, '', 0, 2) 
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
			VALUES ('Drug Treatment Duration Unit', @fid, @deid, '', '', 0, 11) 

		-- Drug Indication
		INSERT [dbo].[Field] (Mandatory, MaxLength, Decimals, MaxSize, MinSize, Calculation, FileSize, FileExt, Anonymise, FieldType_Id)
			VALUES (0, 250, NULL, NULL, NULL, '', NULL, '', 0, 3) 
		set @fid = (SELECT @@IDENTITY)
		INSERT [dbo].[DatasetElementSub] ([ElementName], [Field_Id], [DatasetElement_Id], [OID], [DefaultValue], [System], FieldOrder)
			VALUES ('Drug Indication', @fid, @deid, '', '', 0, 12) 

		-- Product Frequency
		INSERT [dbo].[Field] (Mandatory, MaxLength, Decimals, MaxSize, MinSize, Calculation, FileSize, FileExt, Anonymise, FieldType_Id)
			VALUES (0, 50, NULL, NULL, NULL, '', NULL, '', 0, 3) 
		set @fid = (SELECT @@IDENTITY)
		INSERT [dbo].[DatasetElementSub] ([ElementName], [Field_Id], [DatasetElement_Id], [OID], [DefaultValue], [System], FieldOrder)
			VALUES ('Product Frequency', @fid, @deid, '', '', 0, 12) 
			
		-- Product Batch Number
		INSERT [dbo].[Field] (Mandatory, MaxLength, Decimals, MaxSize, MinSize, Calculation, FileSize, FileExt, Anonymise, FieldType_Id)
			VALUES (0, 25, NULL, NULL, NULL, '', NULL, '', 0, 3) 
		set @fid = (SELECT @@IDENTITY)
		INSERT [dbo].[DatasetElementSub] ([ElementName], [Field_Id], [DatasetElement_Id], [OID], [DefaultValue], [System], FieldOrder)
			VALUES ('Product Batch Number', @fid, @deid, '', '', 0, 12) 
						
		-- Actions taken with product
		INSERT [dbo].[Field] (Mandatory, MaxLength, Decimals, MaxSize, MinSize, Calculation, FileSize, FileExt, Anonymise, FieldType_Id)
			VALUES (0, NULL, NULL, NULL, NULL, '', NULL, '', 0, 2) 
		set @fid = (SELECT @@IDENTITY)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id)
			VALUES ('Product withdrawn', 0, 0, 0, @fid) 
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id)
			VALUES ('Dose reduced', 0, 0, 0, @fid) 
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id)
			VALUES ('Dose increased', 0, 0, 0, @fid) 
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id)
			VALUES ('Dose not changed', 0, 0, 0, @fid) 
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id)
			VALUES ('Unknown', 0, 0, 0, @fid) 
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id)
			VALUES ('Not applicable', 0, 0, 0, @fid) 
		INSERT [dbo].[DatasetElementSub] ([ElementName], [Field_Id], [DatasetElement_Id], [OID], [DefaultValue], [System], FieldOrder)
			VALUES ('Actions taken with product', @fid, @deid, '', '', 0, 13) 

		-- Product challenge
		INSERT [dbo].[Field] (Mandatory, MaxLength, Decimals, MaxSize, MinSize, Calculation, FileSize, FileExt, Anonymise, FieldType_Id)
			VALUES (0, NULL, NULL, NULL, NULL, '', NULL, '', 0, 2) 
		set @fid = (SELECT @@IDENTITY)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id)
			VALUES ('Yes', 0, 0, 0, @fid) 
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id)
			VALUES ('No', 0, 0, 0, @fid) 
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id)
			VALUES ('Unknown', 0, 0, 0, @fid) 
		INSERT [dbo].[DatasetElementSub] ([ElementName], [Field_Id], [DatasetElement_Id], [OID], [DefaultValue], [System], FieldOrder)
			VALUES ('Product challenge', @fid, @deid, '', '', 0, 14) 

		-- Product rechallenge
		INSERT [dbo].[Field] (Mandatory, MaxLength, Decimals, MaxSize, MinSize, Calculation, FileSize, FileExt, Anonymise, FieldType_Id)
			VALUES (0, NULL, NULL, NULL, NULL, '', NULL, '', 0, 2) 
		set @fid = (SELECT @@IDENTITY)
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id)
			VALUES ('Yes', 1, 0, 0, @fid) 
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id)
			VALUES ('No', 0, 0, 0, @fid) 
		INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id)
			VALUES ('Didn''t restart', 0, 0, 0, @fid) 
		INSERT [dbo].[DatasetElementSub] ([ElementName], [Field_Id], [DatasetElement_Id], [OID], [DefaultValue], [System], FieldOrder)
			VALUES ('Product rechallenge', @fid, @deid, '', '', 0, 15) 

/**************************************************
CATEGORY Reaction and Treatment
**************************************************/
INSERT [dbo].[DatasetCategory] ([DatasetCategoryName], [CategoryOrder], [Dataset_Id], [FriendlyName], [Help])
	VALUES ('Reaction and Treatment', 3, @dsid, 'Reaction and Treatment', 'Enter information about what happened and how it was treated.') 
set @dscid = (SELECT @@IDENTITY)

-- Description of reaction
INSERT [dbo].[Field] (Mandatory, MaxLength, Decimals, MaxSize, MinSize, Calculation, FileSize, FileExt, Anonymise, FieldType_Id)
	VALUES (1, 500, NULL, NULL, NULL, '', NULL, '', 0, 3) 
set @fid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetElement] ([ElementName], [Field_Id], [DatasetElementType_Id], [OID], [DefaultValue], [System])
	VALUES ('Description of reaction', @fid, 1, '', '', 0) 
set @deid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetCategoryElement] ([FieldOrder], [DatasetCategory_Id], [DatasetElement_Id], [Acute], [Chronic], [FriendlyName], [Help])
	VALUES (84, @dscid, @deid, 0, 0, '', '') 
set @dceid = (SELECT @@IDENTITY)

-- Reaction start date
INSERT [dbo].[Field] (Mandatory, MaxLength, Decimals, MaxSize, MinSize, Calculation, FileSize, FileExt, Anonymise, FieldType_Id)
	VALUES (0, NULL, NULL, NULL, NULL, '', NULL, '', 0, 6) 
set @fid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetElement] ([ElementName], [Field_Id], [DatasetElementType_Id], [OID], [DefaultValue], [System])
	VALUES ('Reaction start date', @fid, 1, '', '', 0) 
set @deid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetCategoryElement] ([FieldOrder], [DatasetCategory_Id], [DatasetElement_Id], [Acute], [Chronic], [FriendlyName], [Help])
	VALUES (85, @dscid, @deid, 0, 0, 'Start date of reaction', 'Enter the start date of the reaction OR enter the estimated start date in the next field.') 
set @dceid = (SELECT @@IDENTITY)

-- Reaction estimated start date
INSERT [dbo].[Field] (Mandatory, MaxLength, Decimals, MaxSize, MinSize, Calculation, FileSize, FileExt, Anonymise, FieldType_Id)
	VALUES (0, NULL, NULL, NULL, NULL, '', NULL, '', 0, 6) 
set @fid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetElement] ([ElementName], [Field_Id], [DatasetElementType_Id], [OID], [DefaultValue], [System])
	VALUES ('Reaction estimated start date', @fid, 1, '', '', 0) 
set @deid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetCategoryElement] ([FieldOrder], [DatasetCategory_Id], [DatasetElement_Id], [Acute], [Chronic], [FriendlyName], [Help])
	VALUES (90, @dscid, @deid, 0, 0, 'Estimated start date of reaction', 'If you don''t know the exact start date of the reaction, enter the estimated start date here.') 
set @dceid = (SELECT @@IDENTITY)

-- Reaction serious details
INSERT [dbo].[Field] (Mandatory, MaxLength, Decimals, MaxSize, MinSize, Calculation, FileSize, FileExt, Anonymise, FieldType_Id)
	VALUES (0, NULL, NULL, NULL, NULL, '', NULL, '', 0, 1) 
set @fid = (SELECT @@IDENTITY)
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id)
	VALUES ('Resulted in death', 0, 0, 0, @fid) 
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id)
	VALUES ('Is life-threatening', 0, 0, 0, @fid) 
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id)
	VALUES ('Requires inpatient hospitalization or prolongation of existing hospitalization', 0, 0, 0, @fid) 
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id)
	VALUES ('Results in persistent or significant disability/incapacity (as per reporter''s opinion)', 0, 0, 0, @fid) 
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id)
	VALUES ('Is a congenital anomaly/birth defect', 0, 0, 0, @fid) 
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id)
	VALUES ('Other medically important condition', 0, 1, 0, @fid) 
INSERT [dbo].[DatasetElement] ([ElementName], [Field_Id], [DatasetElementType_Id], [OID], [DefaultValue], [System])
	VALUES ('Reaction serious details', @fid, 1, '', '', 0) 
set @deid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetCategoryElement] ([FieldOrder], [DatasetCategory_Id], [DatasetElement_Id], [Acute], [Chronic], [FriendlyName], [Help])
	VALUES (91, @dscid, @deid, 0, 0, 'Did any of these reactions happen?', '') 
set @dceid = (SELECT @@IDENTITY)

-- Treatment given for reaction
INSERT [dbo].[Field] (Mandatory, MaxLength, Decimals, MaxSize, MinSize, Calculation, FileSize, FileExt, Anonymise, FieldType_Id)
	VALUES (0, NULL, NULL, NULL, NULL, '', NULL, '', 0, 5) 
set @fid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetElement] ([ElementName], [Field_Id], [DatasetElementType_Id], [OID], [DefaultValue], [System])
	VALUES ('Treatment given for reaction', @fid, 1, '', '', 0) 
set @deid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetCategoryElement] ([FieldOrder], [DatasetCategory_Id], [DatasetElement_Id], [Acute], [Chronic], [FriendlyName], [Help])
	VALUES (93, @dscid, @deid, 0, 0, 'Was treatment given for the reaction?', '') 
set @dceid = (SELECT @@IDENTITY)

-- Treatment given for reaction details
INSERT [dbo].[Field] (Mandatory, MaxLength, Decimals, MaxSize, MinSize, Calculation, FileSize, FileExt, Anonymise, FieldType_Id)
	VALUES (0, 500, NULL, NULL, NULL, '', NULL, '', 0, 3) 
set @fid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetElement] ([ElementName], [Field_Id], [DatasetElementType_Id], [OID], [DefaultValue], [System])
	VALUES ('Treatment given for reaction details', @fid, 1, '', '', 0) 
set @deid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetCategoryElement] ([FieldOrder], [DatasetCategory_Id], [DatasetElement_Id], [Acute], [Chronic], [FriendlyName], [Help])
	VALUES (94, @dscid, @deid, 0, 0, 'What treatment was given for the reaction?', '') 
set @dceid = (SELECT @@IDENTITY)

-- Outcome of reaction
INSERT [dbo].[Field] (Mandatory, MaxLength, Decimals, MaxSize, MinSize, Calculation, FileSize, FileExt, Anonymise, FieldType_Id)
	VALUES (0, NULL, NULL, NULL, NULL, '', NULL, '', 0, 2) 
set @fid = (SELECT @@IDENTITY)
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id)
	VALUES ('Recovered/resolved', 0, 0, 0, @fid) 
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id)
	VALUES ('Recovering/resolving', 0, 0, 0, @fid) 
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id)
	VALUES ('Not recovered/not resolved', 0, 0, 0, @fid) 
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id)
	VALUES ('Recovered/resolved with permanent complications', 0, 0, 0, @fid) 
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id)
	VALUES ('Fatal', 0, 0, 0, @fid) 
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id)
	VALUES ('Unknown', 0, 0, 0, @fid) 
INSERT [dbo].[DatasetElement] ([ElementName], [Field_Id], [DatasetElementType_Id], [OID], [DefaultValue], [System])
	VALUES ('Outcome of reaction', @fid, 1, '', '', 0) 
set @deid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetCategoryElement] ([FieldOrder], [DatasetCategory_Id], [DatasetElement_Id], [Acute], [Chronic], [FriendlyName], [Help])
	VALUES (95, @dscid, @deid, 0, 0, 'What was the outcome of the reaction?', '') 
set @dceid = (SELECT @@IDENTITY)

-- Reaction date of recovery
INSERT [dbo].[Field] (Mandatory, MaxLength, Decimals, MaxSize, MinSize, Calculation, FileSize, FileExt, Anonymise, FieldType_Id)
	VALUES (0, NULL, NULL, NULL, NULL, '', NULL, '', 0, 6) 
set @fid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetElement] ([ElementName], [Field_Id], [DatasetElementType_Id], [OID], [DefaultValue], [System])
	VALUES ('Reaction date of recovery', @fid, 1, '', '', 0) 
set @deid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetCategoryElement] ([FieldOrder], [DatasetCategory_Id], [DatasetElement_Id], [Acute], [Chronic], [FriendlyName], [Help])
	VALUES (96, @dscid, @deid, 0, 0, 'What was the date of recovery from the reaction?', '') 
set @dceid = (SELECT @@IDENTITY)

-- Reaction date of death
INSERT [dbo].[Field] (Mandatory, MaxLength, Decimals, MaxSize, MinSize, Calculation, FileSize, FileExt, Anonymise, FieldType_Id)
	VALUES (0, NULL, NULL, NULL, NULL, '', NULL, '', 0, 6) 
set @fid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetElement] ([ElementName], [Field_Id], [DatasetElementType_Id], [OID], [DefaultValue], [System])
	VALUES ('Reaction date of death', @fid, 1, '', '', 0) 
set @deid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetCategoryElement] ([FieldOrder], [DatasetCategory_Id], [DatasetElement_Id], [Acute], [Chronic], [FriendlyName], [Help])
	VALUES (97, @dscid, @deid, 0, 0, 'Enter date if patient died from the reaction', '') 
set @dceid = (SELECT @@IDENTITY)

-- Reaction other relevant info
INSERT [dbo].[Field] (Mandatory, MaxLength, Decimals, MaxSize, MinSize, Calculation, FileSize, FileExt, Anonymise, FieldType_Id)
	VALUES (0, 500, NULL, NULL, NULL, '', NULL, '', 0, 3) 
set @fid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetElement] ([ElementName], [Field_Id], [DatasetElementType_Id], [OID], [DefaultValue], [System])
	VALUES ('Reaction other relevant info', @fid, 1, '', '', 0) 
set @deid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetCategoryElement] ([FieldOrder], [DatasetCategory_Id], [DatasetElement_Id], [Acute], [Chronic], [FriendlyName], [Help])
	VALUES (98, @dscid, @deid, 0, 0, 'Other relevant information', 'For example, does the patient have other medical problems?') 
set @dceid = (SELECT @@IDENTITY)

/**************************************************
CATEGORY Test Results
**************************************************/
INSERT [dbo].[DatasetCategory] ([DatasetCategoryName], [CategoryOrder], [Dataset_Id], [FriendlyName], [Help])
	VALUES ('Test Results', 4, @dsid, 'Test Results', 'Enter information about any tests done for the reaction, along with the results.') 
set @dscid = (SELECT @@IDENTITY)

-- Test Results Table
INSERT [dbo].[Field] (Mandatory, MaxLength, Decimals, MaxSize, MinSize, Calculation, FileSize, FileExt, Anonymise, FieldType_Id)
	VALUES (0, NULL, NULL, NULL, NULL, '', NULL, '', 0, 7) 
set @fid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetElement] ([ElementName], [Field_Id], [DatasetElementType_Id], [OID], [DefaultValue], [System])
	VALUES ('Test Results', @fid, 1, '', '', 0) 
set @deid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetCategoryElement] ([FieldOrder], [DatasetCategory_Id], [DatasetElement_Id], [Acute], [Chronic], [FriendlyName], [Help])
	VALUES (10, @dscid, @deid, 0, 0, 'Test Results', 'Test Results') 
set @dceid = (SELECT @@IDENTITY)

	-- Test Date
	INSERT [dbo].[Field] (Mandatory, MaxLength, Decimals, MaxSize, MinSize, Calculation, FileSize, FileExt, Anonymise, FieldType_Id)
		VALUES (0, NULL, NULL, NULL, NULL, '', NULL, '', 0, 6) 
	set @fid = (SELECT @@IDENTITY)
	INSERT [dbo].[DatasetElementSub] ([ElementName], [Field_Id], [DatasetElement_Id], [OID], [DefaultValue], [System], FieldOrder)
		VALUES ('Test Date', @fid, @deid, '', '', 0, 1) 

	-- Test Name
	INSERT [dbo].[Field] (Mandatory, MaxLength, Decimals, MaxSize, MinSize, Calculation, FileSize, FileExt, Anonymise, FieldType_Id)
		VALUES (0, 100, NULL, NULL, NULL, '', NULL, '', 0, 3) 
	set @fid = (SELECT @@IDENTITY)
	INSERT [dbo].[DatasetElementSub] ([ElementName], [Field_Id], [DatasetElement_Id], [OID], [DefaultValue], [System], FieldOrder)
		VALUES ('Test Name', @fid, @deid, '', '', 0, 2) 

	-- Test Result
	INSERT [dbo].[Field] (Mandatory, MaxLength, Decimals, MaxSize, MinSize, Calculation, FileSize, FileExt, Anonymise, FieldType_Id)
		VALUES (0, 50, NULL, NULL, NULL, '', NULL, '', 0, 3) 
	set @fid = (SELECT @@IDENTITY)
	INSERT [dbo].[DatasetElementSub] ([ElementName], [Field_Id], [DatasetElement_Id], [OID], [DefaultValue], [System], FieldOrder)
		VALUES ('Test Result', @fid, @deid, '', '', 0, 3) 

	-- Test Unit
	INSERT [dbo].[Field] (Mandatory, MaxLength, Decimals, MaxSize, MinSize, Calculation, FileSize, FileExt, Anonymise, FieldType_Id)
		VALUES (0, 35, NULL, NULL, NULL, '', NULL, '', 0, 3) 
	set @fid = (SELECT @@IDENTITY)
	INSERT [dbo].[DatasetElementSub] ([ElementName], [Field_Id], [DatasetElement_Id], [OID], [DefaultValue], [System], FieldOrder)
		VALUES ('Test Unit', @fid, @deid, '', '', 0, 4) 

	-- Low Test Range
	INSERT [dbo].[Field] (Mandatory, MaxLength, Decimals, MaxSize, MinSize, Calculation, FileSize, FileExt, Anonymise, FieldType_Id)
		VALUES (0, 50, NULL, NULL, NULL, '', NULL, '', 0, 3) 
	set @fid = (SELECT @@IDENTITY)
	INSERT [dbo].[DatasetElementSub] ([ElementName], [Field_Id], [DatasetElement_Id], [OID], [DefaultValue], [System], FieldOrder)
		VALUES ('Low Test Range', @fid, @deid, '', '', 0, 5) 

	-- High Test Range
	INSERT [dbo].[Field] (Mandatory, MaxLength, Decimals, MaxSize, MinSize, Calculation, FileSize, FileExt, Anonymise, FieldType_Id)
		VALUES (0, 50, NULL, NULL, NULL, '', NULL, '', 0, 3) 
	set @fid = (SELECT @@IDENTITY)
	INSERT [dbo].[DatasetElementSub] ([ElementName], [Field_Id], [DatasetElement_Id], [OID], [DefaultValue], [System], FieldOrder)
		VALUES ('High Test Range', @fid, @deid, '', '', 0, 6) 

	-- More Information
	INSERT [dbo].[Field] (Mandatory, MaxLength, Decimals, MaxSize, MinSize, Calculation, FileSize, FileExt, Anonymise, FieldType_Id)
		VALUES (0, NULL, NULL, NULL, NULL, '', NULL, '', 0, 2) 
	set @fid = (SELECT @@IDENTITY)
	INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id)
		VALUES ('Yes', 1, 0, 0, @fid) 
	INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id)
		VALUES ('No', 0, 0, 0, @fid) 
	INSERT [dbo].[DatasetElementSub] ([ElementName], [Field_Id], [DatasetElement_Id], [OID], [DefaultValue], [System], FieldOrder)
		VALUES ('More Information', @fid, @deid, '', '', 0, 7) 

/**************************************************
CATEGORY Reporter Information
**************************************************/
INSERT [dbo].[DatasetCategory] ([DatasetCategoryName], [CategoryOrder], [Dataset_Id], [FriendlyName], [Help])
	VALUES ('Reporter Information', 5, @dsid, 'Reporter Information', 'Enter information about the person reporting the reaction.') 
set @dscid = (SELECT @@IDENTITY)

-- Reporter Name
INSERT [dbo].[Field] (Mandatory, MaxLength, Decimals, MaxSize, MinSize, Calculation, FileSize, FileExt, Anonymise, FieldType_Id)
	VALUES (1, 60, NULL, NULL, NULL, '', NULL, '', 0, 3) 
set @fid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetElement] ([ElementName], [Field_Id], [DatasetElementType_Id], [OID], [DefaultValue], [System])
	VALUES ('Reporter Name', @fid, 1, '', '', 0) 
set @deid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetCategoryElement] ([FieldOrder], [DatasetCategory_Id], [DatasetElement_Id], [Acute], [Chronic], [FriendlyName], [Help])
	VALUES (91, @dscid, @deid, 1, 0, 'Name or initials of person reporting information', '') 
set @dceid = (SELECT @@IDENTITY)

-- Reporter Contact Information
INSERT [dbo].[Field] (Mandatory, MaxLength, Decimals, MaxSize, MinSize, Calculation, FileSize, FileExt, Anonymise, FieldType_Id)
	VALUES (0, 100, NULL, NULL, NULL, '', NULL, '', 0, 3) 
set @fid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetElement] ([ElementName], [Field_Id], [DatasetElementType_Id], [OID], [DefaultValue], [System])
	VALUES ('Reporter Contact Information', @fid, 1, '', '', 0) 
set @deid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetCategoryElement] ([FieldOrder], [DatasetCategory_Id], [DatasetElement_Id], [Acute], [Chronic], [FriendlyName], [Help])
	VALUES (93, @dscid, @deid, 0, 0, 'Contact information for reporter', '') 
set @dceid = (SELECT @@IDENTITY)

-- Reporter Profession
INSERT [dbo].[Field] (Mandatory, MaxLength, Decimals, MaxSize, MinSize, Calculation, FileSize, FileExt, Anonymise, FieldType_Id)
	VALUES (0, NULL, NULL, NULL, NULL, '', NULL, '', 0, 2) 
set @fid = (SELECT @@IDENTITY)
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id)
	VALUES ('Physician', 0, 0, 0, @fid) 
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id)
	VALUES ('Pharmacist', 0, 0, 0, @fid) 
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id)
	VALUES ('Consumer or other non-health professional', 0, 0, 0, @fid) 
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id)
	VALUES ('Other Health Professional', 0, 0, 0, @fid) 
INSERT [dbo].[FieldValue] (Value, [Default], Other, Unknown, Field_Id)
	VALUES ('Lawyer', 0, 0, 0, @fid) 
INSERT [dbo].[DatasetElement] ([ElementName], [Field_Id], [DatasetElementType_Id], [OID], [DefaultValue], [System])
	VALUES ('Reporter Profession', @fid, 1, '', '', 0) 
set @deid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetCategoryElement] ([FieldOrder], [DatasetCategory_Id], [DatasetElement_Id], [Acute], [Chronic], [FriendlyName], [Help])
	VALUES (94, @dscid, @deid, 1, 0, 'Profession of reporter', '') 
set @dceid = (SELECT @@IDENTITY)

-- Report Reference Number
INSERT [dbo].[Field] (Mandatory, MaxLength, Decimals, MaxSize, MinSize, Calculation, FileSize, FileExt, Anonymise, FieldType_Id)
	VALUES (0, 30, NULL, NULL, NULL, '', NULL, '', 0, 3) 
set @fid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetElement] ([ElementName], [Field_Id], [DatasetElementType_Id], [OID], [DefaultValue], [System])
	VALUES ('Report Reference Number', @fid, 1, '', '', 0) 
set @deid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetCategoryElement] ([FieldOrder], [DatasetCategory_Id], [DatasetElement_Id], [Acute], [Chronic], [FriendlyName], [Help])
	VALUES (96, @dscid, @deid, 0, 0, 'Report reference number (if any)', '') 
set @dceid = (SELECT @@IDENTITY)

-- Reporter Place of Practice
INSERT [dbo].[Field] (Mandatory, MaxLength, Decimals, MaxSize, MinSize, Calculation, FileSize, FileExt, Anonymise, FieldType_Id)
	VALUES (0, 50, NULL, NULL, NULL, '', NULL, '', 0, 3) 
set @fid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetElement] ([ElementName], [Field_Id], [DatasetElementType_Id], [OID], [DefaultValue], [System])
	VALUES ('Reporter Place of Practice', @fid, 1, '', '', 0) 
set @deid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetCategoryElement] ([FieldOrder], [DatasetCategory_Id], [DatasetElement_Id], [Acute], [Chronic], [FriendlyName], [Help])
	VALUES (98, @dscid, @deid, 1, 0, 'Reporter place of practise', '') 
set @dceid = (SELECT @@IDENTITY)

-- Keep Reporter Confidential
INSERT [dbo].[Field] (Mandatory, MaxLength, Decimals, MaxSize, MinSize, Calculation, FileSize, FileExt, Anonymise, FieldType_Id)
	VALUES (0, NULL, NULL, NULL, NULL, '', NULL, '', 0, 5) 
set @fid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetElement] ([ElementName], [Field_Id], [DatasetElementType_Id], [OID], [DefaultValue], [System])
	VALUES ('Keep Reporter Confidential', @fid, 1, '', '', 0) 
set @deid = (SELECT @@IDENTITY)
INSERT [dbo].[DatasetCategoryElement] ([FieldOrder], [DatasetCategory_Id], [DatasetElement_Id], [Acute], [Chronic], [FriendlyName], [Help])
	VALUES (100, @dscid, @deid, 0, 0, 'Keep reporter confidential?', 'Do you want your identity kept confidential except to be contacted by the national medical regulatory authority or the World Health Organization if they need additional information?') 
set @dceid = (SELECT @@IDENTITY)

/**************************************************
SET ORDERING
**************************************************/
-- Category order
--UPDATE a set a.CategoryOrder = a.Row# from (SELECT ds.DatasetName, dc.DatasetCategoryName, dc.CategoryOrder, ROW_NUMBER() OVER(ORDER BY dc.Id, dc.CategoryOrder, dc.DatasetCategoryName ASC) AS Row#
--	FROM Dataset ds
--	INNER JOIN DatasetCategory dc ON ds.Id = dc.Dataset_Id 
--where ds.Id = @Id and dc.System = 0) as a

---- Element order
--UPDATE a set a.FieldOrder = a.Row# from (SELECT ds.DatasetName, dc.DatasetCategoryName, dc.CategoryOrder, de.Id AS DatasetElementID, de.ElementName, dce.FieldOrder, de.DefaultValue, dce.Acute, dce.Chronic, dce.[System], f.MaxLength, f.Decimals, ft.Description, ROW_NUMBER() OVER(ORDER BY dc.CategoryOrder, dc.DatasetCategoryName, dce.Id, dce.FieldOrder ASC) AS Row#
--	FROM Dataset ds
--	INNER JOIN DatasetCategory dc ON ds.Id = dc.Dataset_Id 
--	INNER JOIN DatasetCategoryElement dce ON dc.Id = dce.DatasetCategory_Id 
--	INNER JOIN DatasetElement de ON dce.DatasetElement_Id = de.Id 
--	INNER JOIN Field f ON de.Field_Id = f.Id
--	INNER JOIN FieldType ft ON f.FieldType_Id = ft.Id
--where ds.Id = @Id and dc.System = 0) as a

SELECT ds.DatasetName, dc.DatasetCategoryName, de.ElementName FROM Dataset ds
	INNER JOIN DatasetCategory dc ON ds.Id = dc.Dataset_Id 
	INNER JOIN DatasetCategoryElement dce ON dc.Id = dce.DatasetCategory_Id 
	INNER JOIN DatasetElement de ON dce.DatasetElement_Id = de.Id 
where ds.Id = @dsid
	
--ROLLBACK TRAN A1
COMMIT TRAN A1


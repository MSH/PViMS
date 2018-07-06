DECLARE @ScriptFileName nvarchar(200)
	, @RunDate datetime
SET @ScriptFileName = 'Mods.Data.v1.5.20.sql'

SELECT @RunDate = RunDate  FROM PostDeployment  WHERE ScriptFileName = @ScriptFileName

IF(@RunDate IS NOT NULL) BEGIN
	
	DECLARE @VersionErrorMessage VARCHAR(1024)
	SET @VersionErrorMessage = 'This script has already been executed.'
	RAISERROR(@VersionErrorMessage,16,1)
	RETURN;
	
END

SET NOCOUNT ON

BEGIN TRAN A1

UPDATE [dbo].[Medication] set MedicationForm_Id = null
DELETE [dbo].[MedicationForm]
dbcc checkident('[dbo].[MedicationForm]', reseed, 0)

SET IDENTITY_INSERT [dbo].[MedicationForm] ON
INSERT [dbo].[MedicationForm] ([Id], [Description]) VALUES (1, N'Ampoule')
INSERT [dbo].[MedicationForm] ([Id], [Description]) VALUES (2, N'Bottle ')
INSERT [dbo].[MedicationForm] ([Id], [Description]) VALUES (3, N'Cartridge')
INSERT [dbo].[MedicationForm] ([Id], [Description]) VALUES (4, N'Condom ')
INSERT [dbo].[MedicationForm] ([Id], [Description]) VALUES (5, N'Cream')
INSERT [dbo].[MedicationForm] ([Id], [Description]) VALUES (6, N'Disc for lab testing')
INSERT [dbo].[MedicationForm] ([Id], [Description]) VALUES (7, N'Disposable')
INSERT [dbo].[MedicationForm] ([Id], [Description]) VALUES (8, N'Each')
INSERT [dbo].[MedicationForm] ([Id], [Description]) VALUES (9, N'Ear drops')
INSERT [dbo].[MedicationForm] ([Id], [Description]) VALUES (10, N'Elixir')
INSERT [dbo].[MedicationForm] ([Id], [Description]) VALUES (11, N'Enema')
INSERT [dbo].[MedicationForm] ([Id], [Description]) VALUES (12, N'Gas')
INSERT [dbo].[MedicationForm] ([Id], [Description]) VALUES (13, N'Gel')
INSERT [dbo].[MedicationForm] ([Id], [Description]) VALUES (14, N'Granules')
INSERT [dbo].[MedicationForm] ([Id], [Description]) VALUES (15, N'Inhaler')
INSERT [dbo].[MedicationForm] ([Id], [Description]) VALUES (16, N'IUD')
INSERT [dbo].[MedicationForm] ([Id], [Description]) VALUES (17, N'Liquid')
INSERT [dbo].[MedicationForm] ([Id], [Description]) VALUES (18, N'Lotion')
INSERT [dbo].[MedicationForm] ([Id], [Description]) VALUES (19, N'Nasal drops')
INSERT [dbo].[MedicationForm] ([Id], [Description]) VALUES (20, N'Nasal spray')
INSERT [dbo].[MedicationForm] ([Id], [Description]) VALUES (21, N'Net')
INSERT [dbo].[MedicationForm] ([Id], [Description]) VALUES (22, N'Ointment')
INSERT [dbo].[MedicationForm] ([Id], [Description]) VALUES (23, N'Ophthalmic drops')
INSERT [dbo].[MedicationForm] ([Id], [Description]) VALUES (24, N'Ophthalmic ointment')
INSERT [dbo].[MedicationForm] ([Id], [Description]) VALUES (25, N'Ophthalmic strips')
INSERT [dbo].[MedicationForm] ([Id], [Description]) VALUES (26, N'Oral drops')
INSERT [dbo].[MedicationForm] ([Id], [Description]) VALUES (27, N'Oral gel')
INSERT [dbo].[MedicationForm] ([Id], [Description]) VALUES (28, N'Pessary')
INSERT [dbo].[MedicationForm] ([Id], [Description]) VALUES (29, N'Powder')
INSERT [dbo].[MedicationForm] ([Id], [Description]) VALUES (30, N'Rectal tube')
INSERT [dbo].[MedicationForm] ([Id], [Description]) VALUES (31, N'Respiratory solution')
INSERT [dbo].[MedicationForm] ([Id], [Description]) VALUES (32, N'Rod')
INSERT [dbo].[MedicationForm] ([Id], [Description]) VALUES (33, N'Shampoo')
INSERT [dbo].[MedicationForm] ([Id], [Description]) VALUES (34, N'Solid')
INSERT [dbo].[MedicationForm] ([Id], [Description]) VALUES (35, N'Solution')
INSERT [dbo].[MedicationForm] ([Id], [Description]) VALUES (36, N'Spray')
INSERT [dbo].[MedicationForm] ([Id], [Description]) VALUES (37, N'Suppository')
INSERT [dbo].[MedicationForm] ([Id], [Description]) VALUES (38, N'Suspension')
INSERT [dbo].[MedicationForm] ([Id], [Description]) VALUES (39, N'Syringe')
INSERT [dbo].[MedicationForm] ([Id], [Description]) VALUES (40, N'Syrup')
INSERT [dbo].[MedicationForm] ([Id], [Description]) VALUES (41, N'Tablet or capsule')
INSERT [dbo].[MedicationForm] ([Id], [Description]) VALUES (42, N'Test')
INSERT [dbo].[MedicationForm] ([Id], [Description]) VALUES (43, N'Tincture')
INSERT [dbo].[MedicationForm] ([Id], [Description]) VALUES (44, N'Transdermal patch')
INSERT [dbo].[MedicationForm] ([Id], [Description]) VALUES (45, N'Vial')
INSERT [dbo].[MedicationForm] ([Id], [Description]) VALUES (46, N'Unknown')
SET IDENTITY_INSERT [dbo].[MedicationForm] OFF

UPDATE [dbo].[Medication] set MedicationForm_Id = 46

UPDATE f SET Mandatory = 1 FROM Field f	
		INNER JOIN DatasetElement de on f.Id = de.Field_ID
	where de.ElementName = 'Weight (kg)'

-- Remove legacy spontaneous reports fields
declare @ele table (id int)
insert into @ele (id) 
	select de.Id from DatasetCategory dc inner join DatasetCategoryElement dce on dc.Id = dce.DatasetCategory_Id inner join DatasetElement de on dce.DatasetElement_Id = de.Id inner join Field f on de.Field_Id = f.Id where dc.DatasetCategoryName = 'Particulars of Patient' and  de.ElementName = 'Name'
	UNION
	select de.Id from DatasetCategory dc inner join DatasetCategoryElement dce on dc.Id = dce.DatasetCategory_Id inner join DatasetElement de on dce.DatasetElement_Id = de.Id inner join Field f on de.Field_Id = f.Id where dc.DatasetCategoryName = 'Management of Reaction' and  de.ElementName = 'Causality'
declare @field table (id int)
insert into @field (id) 
	select Id from Field where Id not in (select Field_Id from DatasetElement) and Id not in (select Field_Id from DatasetElementSub)	

delete datasetinstancevalue where DatasetElement_Id in (select id from @ele)
delete dce from DatasetCategoryElement dce inner join DatasetElement de on dce.DatasetElement_Id = de.Id inner join Field f on de.Field_Id = f.Id where dce.DatasetElement_Id in (select id from @ele)
delete de from DatasetElement de inner join Field f on de.Field_Id = f.Id where de.Id in (select id from @ele)
delete fv from field f inner join FieldValue fv on f.Id = fv.Field_Id where f.Id  in (select id from @field)
delete f from field f where f.Id  in (select id from @field)

PRINT 'All Data Modified'

INSERT INTO PostDeployment
		(ScriptGuid, ScriptFileName, ScriptDescription, RunDate, StatusCode, StatusMessage, RunRank)
	select NEWID(), @ScriptFileName, @ScriptFileName, GETDATE(), 200, NULL, (select max(RunRank) + 1 from PostDeployment)

COMMIT TRAN A1

PRINT ''
PRINT '** SCRIPT COMPLETED SUCCESSFULLY **'

DECLARE @ScriptFileName nvarchar(200)
	, @RunDate datetime
SET @ScriptFileName = 'Mods.Data.v1.5.26.sql'

SELECT @RunDate = RunDate  FROM PostDeployment  WHERE ScriptFileName = @ScriptFileName

IF(@RunDate IS NOT NULL) BEGIN
	
	DECLARE @VersionErrorMessage VARCHAR(1024)
	SET @VersionErrorMessage = 'This script has already been executed.'
	RAISERROR(@VersionErrorMessage,16,1)
	RETURN;
	
END

SET NOCOUNT ON

BEGIN TRAN A1

UPDATE dc set [Public] = 0
	FROM Dataset ds
	INNER JOIN DatasetCategory dc ON ds.Id = dc.Dataset_Id 
	INNER JOIN DatasetCategoryElement dce ON dc.Id = dce.DatasetCategory_Id 
	INNER JOIN DatasetElement de ON dce.DatasetElement_Id = de.Id 
	INNER JOIN Field f ON de.Field_Id = f.Id
	INNER JOIN FieldType ft ON f.FieldType_Id = ft.Id
	LEFT JOIN FieldValue fv ON f.Id = fv.Field_Id 
where ds.DatasetName = 'E2B(R2) ICH Report'

UPDATE dce set [Public] = 0
	FROM Dataset ds
	INNER JOIN DatasetCategory dc ON ds.Id = dc.Dataset_Id 
	INNER JOIN DatasetCategoryElement dce ON dc.Id = dce.DatasetCategory_Id 
	INNER JOIN DatasetElement de ON dce.DatasetElement_Id = de.Id 
	INNER JOIN Field f ON de.Field_Id = f.Id
	INNER JOIN FieldType ft ON f.FieldType_Id = ft.Id
	LEFT JOIN FieldValue fv ON f.Id = fv.Field_Id 
where ds.DatasetName = 'E2B(R2) ICH Report'

UPDATE dc set [Public] = 1
	FROM Dataset ds
	INNER JOIN DatasetCategory dc ON ds.Id = dc.Dataset_Id 
	INNER JOIN DatasetCategoryElement dce ON dc.Id = dce.DatasetCategory_Id 
	INNER JOIN DatasetElement de ON dce.DatasetElement_Id = de.Id 
	INNER JOIN Field f ON de.Field_Id = f.Id
	INNER JOIN FieldType ft ON f.FieldType_Id = ft.Id
	LEFT JOIN FieldValue fv ON f.Id = fv.Field_Id 
where ds.DatasetName = 'E2B(R2) ICH Report'
	and dc.DatasetCategoryName in ('Safety Report', 'Primary Source', 'Patient', 'Patient Death', 'Reaction', 'Test', 'Test (2)', 'Test (3)', 'Drug (1)', 'Drug (2)', 'Drug (3)', 'Drug (4)', 'Drug (5)', 'Drug (6)', 'Summary')

UPDATE dce set [Public] = 1
	FROM Dataset ds
	INNER JOIN DatasetCategory dc ON ds.Id = dc.Dataset_Id 
	INNER JOIN DatasetCategoryElement dce ON dc.Id = dce.DatasetCategory_Id 
	INNER JOIN DatasetElement de ON dce.DatasetElement_Id = de.Id 
	INNER JOIN Field f ON de.Field_Id = f.Id
	INNER JOIN FieldType ft ON f.FieldType_Id = ft.Id
	LEFT JOIN FieldValue fv ON f.Id = fv.Field_Id 
where ds.DatasetName = 'E2B(R2) ICH Report' and dc.DatasetCategoryName = 'Safety Report'
	and de.ElementName in ('Serious', 'Seriousness Death', 'Seriousness Life Threatening', 'Seriousness Hospitalization', 'Seriousness Disabling', 'Seriousness Congenital Anomaly', 'Seriousness Other')
	
UPDATE dce set [Public] = 1
	FROM Dataset ds
	INNER JOIN DatasetCategory dc ON ds.Id = dc.Dataset_Id 
	INNER JOIN DatasetCategoryElement dce ON dc.Id = dce.DatasetCategory_Id 
	INNER JOIN DatasetElement de ON dce.DatasetElement_Id = de.Id 
	INNER JOIN Field f ON de.Field_Id = f.Id
	INNER JOIN FieldType ft ON f.FieldType_Id = ft.Id
	LEFT JOIN FieldValue fv ON f.Id = fv.Field_Id 
where ds.DatasetName = 'E2B(R2) ICH Report' and dc.DatasetCategoryName = 'Primary Source'
	and de.ElementName in ('Reporter Title', 'Reporter Given Name', 'Reporter Middle Name', 'Reporter Family Name', 'Reporter Street', 'Reporter City', 'Reporter State', 'Reporter Postcode', 'Reporter Country', 'Qualification')

UPDATE dce set [Public] = 1
	FROM Dataset ds
	INNER JOIN DatasetCategory dc ON ds.Id = dc.Dataset_Id 
	INNER JOIN DatasetCategoryElement dce ON dc.Id = dce.DatasetCategory_Id 
	INNER JOIN DatasetElement de ON dce.DatasetElement_Id = de.Id 
	INNER JOIN Field f ON de.Field_Id = f.Id
	INNER JOIN FieldType ft ON f.FieldType_Id = ft.Id
	LEFT JOIN FieldValue fv ON f.Id = fv.Field_Id 
where ds.DatasetName = 'E2B(R2) ICH Report' and dc.DatasetCategoryName = 'Patient'
	and de.ElementName in ('Patient Initial', 'Patient Hospital Record Number', 'Patient Birthdate', 'Patient Onset Age', 'Patient Onset Age Unit', 'Gestation Period', 'Gestation Period Unit', 'Patient Age Group', 'Patient Weight', 'Patient Height', 'Patient Sex', 'Patient Last Menstrual Date')

UPDATE dce set [Public] = 1
	FROM Dataset ds
	INNER JOIN DatasetCategory dc ON ds.Id = dc.Dataset_Id 
	INNER JOIN DatasetCategoryElement dce ON dc.Id = dce.DatasetCategory_Id 
	INNER JOIN DatasetElement de ON dce.DatasetElement_Id = de.Id 
	INNER JOIN Field f ON de.Field_Id = f.Id
	INNER JOIN FieldType ft ON f.FieldType_Id = ft.Id
	LEFT JOIN FieldValue fv ON f.Id = fv.Field_Id 
where ds.DatasetName = 'E2B(R2) ICH Report' and dc.DatasetCategoryName = 'Patient Death'
	and de.ElementName in ('Patient Deathdate', 'Patient Autopsy')

UPDATE dce set [Public] = 1
	FROM Dataset ds
	INNER JOIN DatasetCategory dc ON ds.Id = dc.Dataset_Id 
	INNER JOIN DatasetCategoryElement dce ON dc.Id = dce.DatasetCategory_Id 
	INNER JOIN DatasetElement de ON dce.DatasetElement_Id = de.Id 
	INNER JOIN Field f ON de.Field_Id = f.Id
	INNER JOIN FieldType ft ON f.FieldType_Id = ft.Id
	LEFT JOIN FieldValue fv ON f.Id = fv.Field_Id 
where ds.DatasetName = 'E2B(R2) ICH Report' and dc.DatasetCategoryName = 'Reaction'
	and de.ElementName in ('Primary Source Reaction', 'Reaction Start Date', 'Reaction End Date', 'Reaction Outcome')

UPDATE dce set [Public] = 1
	FROM Dataset ds
	INNER JOIN DatasetCategory dc ON ds.Id = dc.Dataset_Id 
	INNER JOIN DatasetCategoryElement dce ON dc.Id = dce.DatasetCategory_Id 
	INNER JOIN DatasetElement de ON dce.DatasetElement_Id = de.Id 
	INNER JOIN Field f ON de.Field_Id = f.Id
	INNER JOIN FieldType ft ON f.FieldType_Id = ft.Id
	LEFT JOIN FieldValue fv ON f.Id = fv.Field_Id 
where ds.DatasetName = 'E2B(R2) ICH Report' and dc.DatasetCategoryName in ('Test', 'Test (2)', 'Test (3)')
	and de.ElementName in ('Test Date', 'Test Name', 'Test Result', 'Test Unit', 'Low Test Range', 'High Test Range', 'More Information', 'Test Date (2)', 'Test Name (2)', 'Test Result (2)', 'Test Unit (2)', 'Low Test Range (2)', 'High Test Range (2)', 'More Information (2)', 'Test Date (3)', 'Test Name (3)', 'Test Result (3)', 'Test Unit (3)', 'Low Test Range (3)', 'High Test Range (3)', 'More Information (3)')

UPDATE dce set [Public] = 1
	FROM Dataset ds
	INNER JOIN DatasetCategory dc ON ds.Id = dc.Dataset_Id 
	INNER JOIN DatasetCategoryElement dce ON dc.Id = dce.DatasetCategory_Id 
	INNER JOIN DatasetElement de ON dce.DatasetElement_Id = de.Id 
	INNER JOIN Field f ON de.Field_Id = f.Id
	INNER JOIN FieldType ft ON f.FieldType_Id = ft.Id
	LEFT JOIN FieldValue fv ON f.Id = fv.Field_Id 
where ds.DatasetName = 'E2B(R2) ICH Report' and dc.DatasetCategoryName in ('Drug (1)', 'Drug (2)', 'Drug (3)', 'Drug (4)', 'Drug (5)', 'Drug (6)')
	and de.ElementName in ('Drug Characterization (1)', 'Medicinal Product (1)', 'Dose Number (1)', 'Dose Unit (1)', 'Drug Dosage Form (1)', 'Drug Administration Route (1)', 'Drug Indication (1)', 'Drug Start Date (1)', 'Drug End Date (1)',
	'Drug Characterization (2)', 'Medicinal Product (2)', 'Dose Number (2)', 'Dose Unit (2)', 'Drug Dosage Form (2)', 'Drug Administration Route (2)', 'Drug Indication (2)', 'Drug Start Date (2)', 'Drug End Date (2)',
	'Drug Characterization (3)', 'Medicinal Product (3)', 'Dose Number (3)', 'Dose Unit (3)', 'Drug Dosage Form (3)', 'Drug Administration Route (3)', 'Drug Indication (3)', 'Drug Start Date (3)', 'Drug End Date (3)',
	'Drug Characterization (4)', 'Medicinal Product (4)', 'Dose Number (4)', 'Dose Unit (4)', 'Drug Dosage Form (4)', 'Drug Administration Route (4)', 'Drug Indication (4)', 'Drug Start Date (4)', 'Drug End Date (4)',
	'Drug Characterization (5)', 'Medicinal Product (5)', 'Dose Number (5)', 'Dose Unit (5)', 'Drug Dosage Form (5)', 'Drug Administration Route (5)', 'Drug Indication (5)', 'Drug Start Date (5)', 'Drug End Date (5)',
	'Drug Characterization (6)', 'Medicinal Product (6)', 'Dose Number (6)', 'Dose Unit (6)', 'Drug Dosage Form (6)', 'Drug Administration Route (6)', 'Drug Indication (6)', 'Drug Start Date (6)', 'Drug End Date (6)')

UPDATE dce set [Public] = 1
	FROM Dataset ds
	INNER JOIN DatasetCategory dc ON ds.Id = dc.Dataset_Id 
	INNER JOIN DatasetCategoryElement dce ON dc.Id = dce.DatasetCategory_Id 
	INNER JOIN DatasetElement de ON dce.DatasetElement_Id = de.Id 
	INNER JOIN Field f ON de.Field_Id = f.Id
	INNER JOIN FieldType ft ON f.FieldType_Id = ft.Id
	LEFT JOIN FieldValue fv ON f.Id = fv.Field_Id 
where ds.DatasetName = 'E2B(R2) ICH Report' and dc.DatasetCategoryName = 'Summary'
	and de.ElementName in ('Reporter Comment')

-- Dataset rule
INSERT INTO DatasetRule (RuleType, RuleActive, Dataset_Id)
	SELECT 2, 1, Id	FROM Dataset WHERE DatasetName = 'Spontaneous Report'

-- DatasetElement rule
INSERT INTO DatasetRule (RuleType, RuleActive, DatasetElement_Id)
	SELECT 1, 1, Id	FROM DatasetElement WHERE ElementName IN ('')
		
PRINT 'All Data Modified'

INSERT INTO PostDeployment
		(ScriptGuid, ScriptFileName, ScriptDescription, RunDate, StatusCode, StatusMessage, RunRank)
	select NEWID(), @ScriptFileName, @ScriptFileName, GETDATE(), 200, NULL, (select max(RunRank) + 1 from PostDeployment)

COMMIT TRAN A1

PRINT ''
PRINT '** SCRIPT COMPLETED SUCCESSFULLY **'

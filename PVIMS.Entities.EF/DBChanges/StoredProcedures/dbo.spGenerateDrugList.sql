/**************************************************************************************************************************
**
**	Function: Create analytical stored procedure 
**  Sub Function: Create drug list
**
***************************************************************************************************************************/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[spGenerateDrugList]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[spGenerateDrugList]
GO

CREATE Procedure [dbo].[spGenerateDrugList]
(
	@StartDate date
	, @FinishDate date
	, @TermId int
	, @RiskFactorXml XML
	, @DebugMode bit = 0
)
--WITH ENCRYPTION
AS

/******************************************************************************
**	File: 
**	Name: dbo.spGenerateDrugList
**	Desc: 
**
**	This template can be customized:
**              
**	Return values:
** 
**	Called by:   
**              
**	Parameters:
**	Input				Output
**	----------			-----------
**
**	Auth: S Krog
**	Date: 31 October 2016
**  Current: v3
**
****************************************************************************************************************************
**	Change History
****************************************************************************************************************************
**	Date:				Description:
**	--------			-------------------------------------------
**  2018-01-11			Replace MedicationCausality with ReportInstanceMedication 
**  2018-01-23			Cater for archiving
**
***************************************************************************************************************************/
BEGIN

	SET NOCOUNT ON

	-- DEBUGGING
	--DECLARE @StartDate date
	--DECLARE @FinishDate date
	--DECLARE @TermID int
	--DECLARE @RiskFactorXml XML
	--DECLARE @DebugMode int

	--SET @StartDate = '2009-07-01'
	--SET @FinishDate = '2016-10-28'
	--SET @TermID = 0
	--SET @TermID = 86077
	--SET @RiskFactorXml = '<Factors><Factor><Name>Age Group</Name><Option>Adolescent</Option></Factor></Factors>'
	--SET @DebugMode = 1

	-- WRITE A HISTORY OF ALL DRUG CHANGES FOR ALL PATIENTS ON PATIENT LIST
	IF (SELECT COUNT(*) FROM #PatientListRiskFactors) = 0 begin
		-- WE DO NOT NEED TO INCLUDE RISK FACTORS
		INSERT INTO #Druglist 
				(Patient_Id, Drug, Medication_Id, StartDate, FinishDate, DaysContributed, ADR)
			SELECT pm.Patient_Id, m.DrugName, m.Id, pm.DateStart, pm.DateEnd, 0, 0
				FROM PatientMedication pm
					INNER JOIN Medication m ON pm.Medication_Id = m.Id
					INNER JOIN #PatientList p ON pm.Patient_Id = p.Id 
				WHERE pm.Archived = 0 
	end 
	else begin
		-- WE NEED TO INCLUDE RISK FACTORS

		DECLARE @RiskFactor varchar(20), @RiskFactorOption varchar(30)
		DECLARE @sql nvarchar(max)

		SET @sql  = N'
		INSERT INTO #Druglist 
				(Patient_Id, Drug, Medication_Id, StartDate, FinishDate, DaysContributed, ADR)
			SELECT pm.Patient_Id, m.DrugName, m.Id, pm.DateStart, pm.DateEnd, 0, 0
				FROM PatientMedication pm
					INNER JOIN Medication m ON pm.Medication_Id = m.Id
					INNER JOIN #PatientList p ON pm.Patient_Id = p.Id 
				WHERE pm.Archived = 0 '
					
		DECLARE factor_cursor CURSOR FOR   
			SELECT  
				   Tbl.Col.value('Name[1]', 'varchar(20)'),  
				   Tbl.Col.value('Option[1]', 'varchar(30)')
			FROM   @RiskFactorXml.nodes('//Factors/Factor') Tbl(Col) 
		  
		OPEN factor_cursor  
		  
		FETCH NEXT FROM factor_cursor  
		INTO @RiskFactor, @RiskFactorOption
		  
		WHILE @@FETCH_STATUS = 0  
		BEGIN  
			IF CHARINDEX('WHERE', @SQL) = 0 begin
				SET @sql = @sql + ' WHERE EXISTS (select Id from #PatientListRiskFactors pf where pf.PatientId = p.Id and pf.RiskFactor = ''' + @RiskFactor + ''' and pf.RiskFactorOption = ''' + @RiskFactorOption + ''' and pf.FactorMet = 1) '
			end
			else begin
				SET @sql = @sql + ' AND EXISTS (select Id from #PatientListRiskFactors pf where pf.PatientId = p.Id and pf.RiskFactor = ''' + @RiskFactor + ''' and pf.RiskFactorOption = ''' + @RiskFactorOption + ''' and pf.FactorMet = 1) '
			end
				
			FETCH NEXT FROM factor_cursor   
			INTO @RiskFactor, @RiskFactorOption
		END   
		CLOSE factor_cursor     
		DEALLOCATE factor_cursor     	

		if @DebugMode = 1 begin
			select @SQL
		end
	
		EXEC(@sql)
	end

	-- REMOVE ALL DRUGS PRIOR TO REPORTING START DATE
	DELETE #Druglist WHERE FinishDate <= @StartDate

	-- REMOVE ALL DRUGS PRIOR TO REPORTING START DATE
	DELETE d FROM #Druglist d INNER JOIN #PatientList p ON d.Patient_Id = p.Id WHERE d.StartDate > p.FinishDate
	DELETE d FROM #Druglist d INNER JOIN #PatientList p ON d.Patient_Id = p.Id WHERE d.FinishDate < p.StartDate

	-- REMOVE ALL DRUGS AFTER REPORTING FINISH DATE
	DELETE #Druglist WHERE StartDate >= @FinishDate

	-- RESET START DATES TO MATCH REPORTING START DATE
	UPDATE #Druglist SET StartDate = @StartDate WHERE StartDate < @StartDate 
	UPDATE d SET d.StartDate = p.StartDate from #Druglist d INNER JOIN #PatientList p ON d.Patient_Id = p.Id where p.StartDate > d.StartDate

	-- RESET FINISH DATES TO MATCH REPORTING FINISH DATE
	UPDATE #Druglist SET FinishDate = @FinishDate WHERE FinishDate is null
	UPDATE #Druglist SET FinishDate = @FinishDate WHERE FinishDate > @FinishDate 
	UPDATE d SET d.FinishDate = p.FinishDate from #Druglist d INNER JOIN #PatientList p ON d.Patient_Id = p.Id where p.FinishDate < d.FinishDate


	-- CALCULATE CONTRIBUTION
	UPDATE #Druglist SET DaysContributed = DATEDIFF(dd, StartDate, FinishDate)

	IF @TermID > 0 begin
		-- SET ADR CONTRIBUTION
		UPDATE #DrugList SET ADR = 
			CASE WHEN EXISTS 
				(SELECT iPCE.Id FROM PatientClinicalEvent iPCE 
					INNER JOIN ReportInstance iRI on iPCE.PatientClinicalEventGuid = iRI.ContextGuid 
					INNER JOIN ReportInstanceMedication iRIM ON iRI.Id = iRIM.ReportInstance_Id 
					INNER JOIN PatientMedication iPM ON iRIM.ReportInstanceMedicationGuid = iPM.PatientMedicationGuid 
					INNER JOIN Medication iM ON iPM.Medication_Id = iM.Id 
				WHERE iPCE.Patient_Id = d.Patient_Id and iPCE.Archived = 0 and iPM.Archived = 0  
					AND iPCE.OnsetDate BETWEEN d.StartDate AND d.FinishDate 
					AND d.Medication_Id = iM.Id 
					AND iRI.TerminologyMedDra_Id = @TermID 
					AND (iRIM.[NaranjoCausality] IN ('Definite', 'Probable', 'Possible') OR iRIM.[WHOCausality] IN ('Certain', 'Probable', 'Possible'))
				) 
				THEN 1 ELSE 0 END 
			FROM #DrugList d
		
		-- EXTRACT DEDUPED MEDICATION LIST PER PATIENT
		INSERT INTO #DrugListDeduped 
				(Patient_Id, Drug, Medication_Id, StartDate, FinishDate, DaysContributed, ADR)
			SELECT Patient_Id, Drug, Medication_Id, MIN([StartDate]), MAX([FinishDate]), SUM([DaysContributed]), MAX([ADR])
				FROM #DrugList 
			GROUP BY Patient_Id, Drug, Medication_Id
	end

END

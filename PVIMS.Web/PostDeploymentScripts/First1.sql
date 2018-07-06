/**************************************************************************************************************************
**
**	Function: Create analytical stored procedure 
**  Sub Function: Create drug list
**
***************************************************************************************************************************/
CREATE Procedure [dbo].[spGenerateRiskFactors]
(
	@StartDate date
	, @FinishDate date
	, @DebugMode bit = 0
)
--WITH ENCRYPTION
AS

/******************************************************************************
**	File: 
**	Name: dbo.spGenerateRiskFactors
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
**  Current: v1
**
****************************************************************************************************************************
**	Change History
****************************************************************************************************************************
**	Date:			Version		Author:		Description:
**	--------		--------	-------		-------------------------------------------
**
***************************************************************************************************************************/
BEGIN

	SET NOCOUNT ON

	-- DEBUGGING
	--DECLARE @StartDate date
	--DECLARE @FinishDate date
	--DECLARE @DebugMode int

	--SET @StartDate = '2009-07-01'
	--SET @FinishDate = '2016-10-28'
	--SET @DebugMode = 1

	/***********************************************************************************
	ADJUSTED - CREATE LIST OF PATIENTS RISK FACTORS
	************************************************************************************/
	
	IF OBJECT_ID('tempdb..#PatientList', 'U') IS NULL begin
		return 0
	end
	IF OBJECT_ID('tempdb..#PatientListRiskFactors', 'U') IS NULL begin
		return 0
	end

	SET NOCOUNT ON;  
  
	INSERT INTO #PatientListRiskFactors
			(PatientID, RiskFactor, RiskFactorCriteria, RiskFactorOption, RiskFactorOptionCriteria, FactorMet)
		SELECT p.Id, rf.Display, rf.Criteria, rfo.Display, rfo.Criteria, 0
			FROM #PatientList p, RiskFactor rf INNER JOIN RiskFactorOption rfo on rf.Id = rfo.RiskFactor_Id

	DECLARE @id int, @pid int, @RiskFactor varchar(20), @RiskFactorCriteria varchar(MAX), @RiskFactorOption varchar(30), @RiskFactorOptionCriteria varchar(250)
	DECLARE @sql nvarchar(max)
  
	DECLARE factor_cursor CURSOR FOR   
	SELECT Id, PatientID, RiskFactor, RiskFactorCriteria, RiskFactorOption, RiskFactorOptionCriteria
		FROM #PatientListRiskFactors
	  
	OPEN factor_cursor  
	  
	FETCH NEXT FROM factor_cursor  
	INTO @id, @pid, @RiskFactor, @RiskFactorCriteria, @RiskFactorOption, @RiskFactorOptionCriteria
	  
	WHILE @@FETCH_STATUS = 0  
	BEGIN 
		IF(CHARINDEX('#ContextOption#', @RiskFactorCriteria) > 0) begin
			SET @RiskFactorCriteria = REPLACE(@RiskFactorCriteria, '#ContextStart#', convert(varchar(10), @StartDate, 120))
			SET @RiskFactorCriteria = REPLACE(@RiskFactorCriteria, '#ContextFinish#', convert(varchar(10), @FinishDate, 120))
			SET @RiskFactorCriteria = REPLACE(@RiskFactorCriteria, '#ContextOption#', @RiskFactorOptionCriteria)

			SELECT @sql = N'update #PatientListRiskFactors
				set FactorMet = case when ' + @RiskFactorCriteria + ' then 1 else 0 end where Id = ' + cast(@id as varchar)
			EXEC(@sql)
		end
		else begin
			SET @RiskFactorCriteria = REPLACE(@RiskFactorCriteria, '#ContextStart#', convert(varchar(10), @StartDate, 120))
			SET @RiskFactorCriteria = REPLACE(@RiskFactorCriteria, '#ContextFinish#', convert(varchar(10), @FinishDate, 120))

			SELECT @sql = N'update #PatientListRiskFactors
				set FactorMet = case when ' + @RiskFactorCriteria + @RiskFactorOptionCriteria + ' then 1 else 0 end where Id = ' + cast(@id as varchar)
			EXEC(@sql)
		end

		if @DebugMode = 1 begin
			select @SQL
		end
			
		FETCH NEXT FROM factor_cursor   
		INTO @id, @pid, @RiskFactor, @RiskFactorCriteria, @RiskFactorOption, @RiskFactorOptionCriteria
	END   
	CLOSE factor_cursor     
	DEALLOCATE factor_cursor     	
	
END

/**************************************************************************************************************************
**
**	Function: Create analytical stored procedure 
**  Sub Function: Create patient list
**
***************************************************************************************************************************/
CREATE Procedure [dbo].[spGeneratePatientListCondition]
(
	@ConditionId int
	, @StartDate date
	, @FinishDate date
	
)
--WITH ENCRYPTION
AS

/******************************************************************************
**	File: 
**	Name: dbo.spGeneratePatientListCondition
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
**  Current: v1
**
****************************************************************************************************************************
**	Change History
****************************************************************************************************************************
**	Date:			Version		Author:		Description:
**	--------		--------	-------		-------------------------------------------
**	28/11/2016		v2			SIK			Replace treatment start date with condition start date
**
***************************************************************************************************************************/
BEGIN

	SET NOCOUNT ON

	-- DEBUGGING
	--DECLARE @ConditionId int
	--DECLARE @StartDate date
	--DECLARE @FinishDate date

	--SET @ConditionId = 1
	--SET @StartDate = '2009-07-01'
	--SET @FinishDate = '2016-10-28'

	IF OBJECT_ID('tempdb..#PatientListTemp', 'U') IS NOT NULL
	BEGIN
		DROP TABLE #PatientListTemp
	END
	CREATE TABLE #PatientListTemp
		(Id int, StartDate date, FinishDate date)

	/***********************************************************************************
	GENERAL - CREATE LIST OF PATIENTS CONTRIBUTING TO REPORTING PERIOD
	************************************************************************************/
	-- WRITE PATIENT RECORD FOR PATIENTS REGISTERED BEFORE AND ACTIVE BY END OF REPORT
	INSERT INTO #PatientListTemp
			(Id, StartDate, FinishDate)
			SELECT P.Id, @StartDate, @FinishDate 
				FROM Patient p
					INNER JOIN PatientCondition pc ON p.Id = pc.Patient_Id 
					INNER JOIN TerminologyMedDra tm ON pc.TerminologyMedDra_Id = tm.Id 
					INNER JOIN ConditionMedDra cm ON tm.Id = cm.TerminologyMedDra_Id 
					INNER JOIN Condition c ON cm.Condition_Id = c.Id 
				WHERE c.Id = @ConditionId 
					AND DateStart < @StartDate AND DateStart < @FinishDate
					AND (OutcomeDate is null OR (OutcomeDate is not null and OutcomeDate > @FinishDate))

	-- WRITE PATIENT RECORD FOR PATIENTS REGISTERED BEFORE AND ACTIVE DURING BUT INACTIVATE BY END
	INSERT INTO #PatientListTemp
			(Id, StartDate, FinishDate)
			SELECT P.Id, @StartDate, OutcomeDate 
				FROM Patient P
					INNER JOIN PatientCondition pc ON p.Id = pc.Patient_Id 
					INNER JOIN TerminologyMedDra tm ON pc.TerminologyMedDra_Id = tm.Id 
					INNER JOIN ConditionMedDra cm ON tm.Id = cm.TerminologyMedDra_Id 
					INNER JOIN Condition c ON cm.Condition_Id = c.Id 
				WHERE c.Id = @ConditionId 
					AND DateStart < @StartDate AND DateStart < @FinishDate
					AND OutcomeDate is not null and OutcomeDate <= @FinishDate

	-- WRITE PATIENT RECORD FOR PATIENTS REGISTERED AFTER REPORT START BUT BEFORE END AND ACTIVE BY END OF REPORT
	INSERT INTO #PatientListTemp
			(Id, StartDate, FinishDate)
			SELECT P.Id, DateStart, @FinishDate 
				FROM Patient P
					INNER JOIN PatientCondition pc ON p.Id = pc.Patient_Id 
					INNER JOIN TerminologyMedDra tm ON pc.TerminologyMedDra_Id = tm.Id 
					INNER JOIN ConditionMedDra cm ON tm.Id = cm.TerminologyMedDra_Id 
					INNER JOIN Condition c ON cm.Condition_Id = c.Id 
				WHERE c.Id = @ConditionId 
					AND DateStart >= @StartDate AND DateStart < @FinishDate
					AND (OutcomeDate is null OR (OutcomeDate is not null and OutcomeDate > @FinishDate))

	-- WRITE PATIENT RECORD FOR PATIENTS REGISTERED AFTER REPORT START BUT BEFORE END AND ACTIVE DURING BUT INACTIVATE BY END
	INSERT INTO #PatientListTemp
			(Id, StartDate, FinishDate)
			SELECT P.Id, DateStart, OutcomeDate 
				FROM Patient P
					INNER JOIN PatientCondition pc ON p.Id = pc.Patient_Id 
					INNER JOIN TerminologyMedDra tm ON pc.TerminologyMedDra_Id = tm.Id 
					INNER JOIN ConditionMedDra cm ON tm.Id = cm.TerminologyMedDra_Id 
					INNER JOIN Condition c ON cm.Condition_Id = c.Id 
				WHERE c.Id = @ConditionId 
					AND DateStart >= @StartDate AND DateStart < @FinishDate
					AND OutcomeDate is not null and OutcomeDate <= @FinishDate

	INSERT INTO #PatientList
			(Id, StartDate, FinishDate)
		SELECT Id, Min(StartDate), MAX(FinishDate)
			 FROM #PatientListTemp GROUP BY Id

END

/**************************************************************************************************************************
**
**	Function: Create analytical stored procedure 
**  Sub Function: Create patient list
**
***************************************************************************************************************************/
CREATE Procedure [dbo].[spGeneratePatientListCohort]
(
	@CohortId int
	, @StartDate date
	, @FinishDate date
	
)
--WITH ENCRYPTION
AS

/******************************************************************************
**	File: 
**	Name: dbo.spGeneratePatientListCohort
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
**	Date: 21 December 2016
**  Current: v1
**
****************************************************************************************************************************
**	Change History
****************************************************************************************************************************
**	Date:			Version		Author:		Description:
**	--------		--------	-------		-------------------------------------------
**
***************************************************************************************************************************/
BEGIN

	SET NOCOUNT ON

	-- DEBUGGING
	--DECLARE @CohortId int
	--DECLARE @StartDate date
	--DECLARE @FinishDate date

	--SET @CohortId = 1
	--SET @StartDate = '2009-07-01'
	--SET @FinishDate = '2016-10-28'

	IF OBJECT_ID('tempdb..#PatientListTemp', 'U') IS NOT NULL
	BEGIN
		DROP TABLE #PatientListTemp
	END
	CREATE TABLE #PatientListTemp
		(Id int, StartDate date, FinishDate date)

	/***********************************************************************************
	GENERAL - CREATE LIST OF PATIENTS CONTRIBUTING TO REPORTING PERIOD
	************************************************************************************/
	-- WRITE PATIENT RECORD FOR PATIENTS REGISTERED BEFORE AND ACTIVE BY END OF REPORT
	INSERT INTO #PatientListTemp
			(Id, StartDate, FinishDate)
			SELECT P.Id, @StartDate, @FinishDate 
				FROM Patient p
					INNER JOIN CohortGroupEnrolment cge ON p.Id = cge.Patient_Id 
				WHERE cge.CohortGroup_Id = @CohortId 
					AND EnroledDate < @StartDate AND EnroledDate < @FinishDate
					AND (DeenroledDate is null OR (DeenroledDate is not null and DeenroledDate > @FinishDate))

	-- WRITE PATIENT RECORD FOR PATIENTS REGISTERED BEFORE AND ACTIVE DURING BUT INACTIVATE BY END
	INSERT INTO #PatientListTemp
			(Id, StartDate, FinishDate)
			SELECT P.Id, @StartDate, DeenroledDate 
				FROM Patient P
					INNER JOIN CohortGroupEnrolment cge ON p.Id = cge.Patient_Id 
				WHERE cge.CohortGroup_Id = @CohortId 
					AND EnroledDate < @StartDate AND EnroledDate < @FinishDate
					AND DeenroledDate is not null and DeenroledDate <= @FinishDate

	-- WRITE PATIENT RECORD FOR PATIENTS REGISTERED AFTER REPORT START BUT BEFORE END AND ACTIVE BY END OF REPORT
	INSERT INTO #PatientListTemp
			(Id, StartDate, FinishDate)
			SELECT P.Id, EnroledDate, @FinishDate 
				FROM Patient P
					INNER JOIN CohortGroupEnrolment cge ON p.Id = cge.Patient_Id 
				WHERE cge.CohortGroup_Id = @CohortId 
					AND EnroledDate >= @StartDate AND EnroledDate < @FinishDate
					AND (DeenroledDate is null OR (DeenroledDate is not null and DeenroledDate > @FinishDate))

	-- WRITE PATIENT RECORD FOR PATIENTS REGISTERED AFTER REPORT START BUT BEFORE END AND ACTIVE DURING BUT INACTIVATE BY END
	INSERT INTO #PatientListTemp
			(Id, StartDate, FinishDate)
			SELECT P.Id, EnroledDate, DeenroledDate 
				FROM Patient P
					INNER JOIN CohortGroupEnrolment cge ON p.Id = cge.Patient_Id 
				WHERE cge.CohortGroup_Id = @CohortId 
					AND EnroledDate >= @StartDate AND EnroledDate < @FinishDate
					AND DeenroledDate is not null and DeenroledDate <= @FinishDate

	INSERT INTO #PatientList
			(Id, StartDate, FinishDate)
		SELECT Id, Min(StartDate), MAX(FinishDate)
			 FROM #PatientListTemp GROUP BY Id

END

/**************************************************************************************************************************
**
**	Function: Create analytical stored procedure 
**  Sub Function: Create drug list
**
***************************************************************************************************************************/
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
**  Current: v1
**
****************************************************************************************************************************
**	Change History
****************************************************************************************************************************
**	Date:			Version		Author:		Description:
**	--------		--------	-------		-------------------------------------------
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
					INNER JOIN #PatientList p ON pm.Patient_Id = p.Id '
					
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
			CASE WHEN EXISTS (SELECT iPCE.Id FROM PatientClinicalEvent iPCE INNER JOIN MedicationCausality iMC ON iPCE.Id = iMC.ClinicalEvent_Id INNER JOIN PatientMedication iPM ON iMC.Medication_Id = iPM.Id INNER JOIN Medication iM ON iPM.Medication_Id = iM.Id WHERE iPCE.Patient_Id = d.Patient_Id AND iPCE.OnsetDate BETWEEN d.StartDate AND d.FinishDate AND d.Medication_Id = iM.Id AND iPCE.TerminologyMedDra_Id1 = @TermID AND (iMC.[NaranjoCausality] IN ('Definite', 'Probable', 'Possible') OR iMC.[WHOCausality] IN ('Certain', 'Probable', 'Possible'))) 
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

/**************************************************************************************************************************
**
**	Function: Create analytical stored procedure 
**  Sub Function: Create drug list
**
***************************************************************************************************************************/
CREATE Procedure [dbo].[spGenerateContingency]
(
	@StartDate date
	, @FinishDate date
	, @RateByCount bit = 1
	, @DebugMode bit = 0
)
--WITH ENCRYPTION
AS

/******************************************************************************
**	File: 
**	Name: dbo.spGenerateContingency
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
**  Current: v2
**
****************************************************************************************************************************
**	Change History
****************************************************************************************************************************
**	Date:			Version		Author:		Description:
**	--------		--------	-------		-------------------------------------------
**  12/01/2017		v2			SK			Fixed non exposed case and non case counts
**
***************************************************************************************************************************/
BEGIN

	SET NOCOUNT ON

	-- DEBUGGING
	--DECLARE @StartDate date
	--DECLARE @FinishDate date
	--DECLARE @RateByCount bit
	--DECLARE @DebugMode int

	--SET @StartDate = '2009-07-01'
	--SET @FinishDate = '2016-10-28'
	--SET @RateByCount = 1
	--SET @DebugMode = 1

	/***********************************************************************************
	EXPOSED
	************************************************************************************/
	INSERT #ContingencyExposed
			(Drug, Medication_Id, ExposedCases, ExposedNonCases, ExposedPopulation, ExposedIncidenceRate)
			SELECT	DrugName, Id
					, (SELECT COUNT(*) FROM #DrugListDeduped WHERE Medication_Id = M.Id AND [ADR] = 1)
					, (SELECT COUNT(*) FROM #DrugListDeduped WHERE Medication_Id = M.Id AND [ADR] = 0)
					, ROUND((SELECT SUM([DaysContributed]) FROM #DrugListDeduped WHERE Medication_Id = M.Id) / 365.25, 2)
					, 0
				FROM Medication M
			where M.Id in (SELECT DISTINCT(Medication_Id) FROM #DrugListDeduped)

	-- SET EXPOSED IR
	IF @RateByCount = 1 begin
		UPDATE #ContingencyExposed SET ExposedIncidenceRate = (CAST([ExposedCases] AS decimal(9,2))/(CAST([ExposedCases] AS decimal(9,2)) + CAST([ExposedNonCases] AS decimal(9,2)))) * 1000 WHERE [ExposedCases] > 0 OR [ExposedNonCases] > 0
	end
	else begin
		UPDATE #ContingencyExposed SET ExposedIncidenceRate = (CAST([ExposedCases] AS decimal(9,2))/(CAST([ExposedCases] AS decimal(9,2)) + CAST([ExposedPopulation] AS decimal(9,2)))) * 1000 WHERE [ExposedCases] > 0 OR [ExposedPopulation] > 0	
	end

	/***********************************************************************************
	NON EXPOSED
	************************************************************************************/
	INSERT #ContingencyNonExposed
			(Drug, Medication_Id, NonExposedCases, NonExposedNonCases, NonExposedPopulation, NonExposedIncidenceRate)
			SELECT	DrugName, Id
					, (SELECT COUNT(DISTINCT(A.[Patient_Id])) FROM (SELECT Patient_Id, MAX(ADR) AS ADR FROM #DrugListDeduped WHERE Patient_Id NOT IN (SELECT Patient_Id FROM #DrugListDeduped WHERE Medication_Id = M.Id) GROUP BY Patient_Id) As A WHERE A.[ADR] = 1)
					, (SELECT COUNT(DISTINCT(A.[Patient_Id])) FROM (SELECT Patient_Id, MAX(ADR) AS ADR FROM #DrugListDeduped WHERE Patient_Id NOT IN (SELECT Patient_Id FROM #DrugListDeduped WHERE Medication_Id = M.Id) GROUP BY Patient_Id) As A WHERE A.[ADR] = 0)
					, ROUND((SELECT SUM([DaysContributed]) FROM #DrugListDeduped WHERE Medication_Id <> M.Id) / 365.25, 2)
					, 0
				FROM Medication M
			where M.Id in (SELECT DISTINCT(Medication_Id) FROM #DrugListDeduped)

	IF (SELECT COUNT(*) FROM #PatientListRiskFactors) = 0 begin
		-- WE DO NOT NEED TO INCLUDE RISK FACTORS
		-- SET NONEXPOSED IR AND RISK RATIO

		IF @RateByCount = 1 begin
			UPDATE #ContingencyNonExposed SET NonExposedIncidenceRate = (CAST([NonExposedCases] AS decimal(9,2))/(CAST([NonExposedCases] AS decimal(9,2)) + CAST([NonExposedNonCases] AS decimal(9,2)))) * 1000 WHERE [NonExposedCases] > 0 OR [NonExposedNonCases] > 0

			INSERT #ContingencyRiskRatio
					(Drug, Medication_Id, UnAdjustedRelativeRisk, ConfidenceIntervalLow, ConfidenceIntervalHigh)
					SELECT	M.DrugName, M.Id
							, (E.[ExposedIncidenceRate] / NE.[NonExposedIncidenceRate])
							, (((E.[ExposedIncidenceRate] / NE.[NonExposedIncidenceRate]) * EXP((-1.96*SQRT(((CAST([ExposedNonCases] AS decimal(9,2))) / ((CAST([ExposedCases] AS decimal(9,2)))*((CAST([ExposedCases] AS decimal(9,2)))+(CAST([ExposedNonCases] AS decimal(9,2)))))) + ((CAST([NonExposedNonCases] AS decimal(9,2))) / ((CAST([NonExposedCases] AS decimal(9,2)))*((CAST([NonExposedCases] AS decimal(9,2)))+(CAST([NonExposedNonCases] AS decimal(9,2)))))))))))
							, (((E.[ExposedIncidenceRate] / NE.[NonExposedIncidenceRate]) * EXP((+1.96*SQRT(((CAST([ExposedNonCases] AS decimal(9,2))) / ((CAST([ExposedCases] AS decimal(9,2)))*((CAST([ExposedCases] AS decimal(9,2)))+(CAST([ExposedNonCases] AS decimal(9,2)))))) + ((CAST([NonExposedNonCases] AS decimal(9,2))) / ((CAST([NonExposedCases] AS decimal(9,2)))*((CAST([NonExposedCases] AS decimal(9,2)))+(CAST([NonExposedNonCases] AS decimal(9,2)))))))))))
						FROM Medication M
							INNER JOIN #ContingencyExposed E ON M.Id = E.Medication_Id
							INNER JOIN #ContingencyNonExposed NE ON M.Id = NE.Medication_Id
						WHERE (E.[ExposedIncidenceRate] > 0 AND NE.[NonExposedIncidenceRate] > 0)
		end
		else begin
			UPDATE #ContingencyNonExposed SET NonExposedIncidenceRate = (CAST([NonExposedCases] AS decimal(9,2))/(CAST([NonExposedCases] AS decimal(9,2)) + CAST([NonExposedPopulation] AS decimal(9,2)))) * 1000 WHERE [NonExposedCases] > 0 OR [NonExposedPopulation] > 0
			
			INSERT #ContingencyRiskRatio
					(Drug, Medication_Id, UnAdjustedRelativeRisk, ConfidenceIntervalLow, ConfidenceIntervalHigh)
					SELECT	M.DrugName, M.Id
							, (E.[ExposedIncidenceRate] / NE.[NonExposedIncidenceRate])
							, (((E.[ExposedIncidenceRate] / NE.[NonExposedIncidenceRate]) * EXP((-1.96*SQRT(((CAST([ExposedPopulation] AS decimal(9,2))) / ((CAST([ExposedCases] AS decimal(9,2)))*((CAST([ExposedCases] AS decimal(9,2)))+(CAST([ExposedPopulation] AS decimal(9,2)))))) + ((CAST([NonExposedPopulation] AS decimal(9,2))) / ((CAST([NonExposedCases] AS decimal(9,2)))*((CAST([NonExposedCases] AS decimal(9,2)))+(CAST([NonExposedPopulation] AS decimal(9,2)))))))))))
							, (((E.[ExposedIncidenceRate] / NE.[NonExposedIncidenceRate]) * EXP((+1.96*SQRT(((CAST([ExposedPopulation] AS decimal(9,2))) / ((CAST([ExposedCases] AS decimal(9,2)))*((CAST([ExposedCases] AS decimal(9,2)))+(CAST([ExposedPopulation] AS decimal(9,2)))))) + ((CAST([NonExposedPopulation] AS decimal(9,2))) / ((CAST([NonExposedCases] AS decimal(9,2)))*((CAST([NonExposedCases] AS decimal(9,2)))+(CAST([NonExposedPopulation] AS decimal(9,2)))))))))))
						FROM Medication M 
							INNER JOIN #ContingencyExposed E ON M.Id = E.Medication_Id
							INNER JOIN #ContingencyNonExposed NE ON M.Id = NE.Medication_Id
						WHERE (E.[ExposedIncidenceRate] > 0 AND NE.[NonExposedIncidenceRate] > 0)
		end
	end
	else begin
		-- WE NEED TO INCLUDE RISK FACTORS
		-- SET NONEXPOSED IR AND RISK RATIO

		IF @RateByCount = 1 begin
			UPDATE #ContingencyNonExposed SET NonExposedIncidenceRate = (CAST([NonExposedCases] AS decimal(9,2))/(CAST([NonExposedCases] AS decimal(9,2)) + CAST([NonExposedNonCases] AS decimal(9,2)))) * 1000 WHERE [NonExposedCases] > 0 OR [NonExposedNonCases] > 0

			INSERT #ContingencyAdjustedRiskRatio
					(Drug, Medication_Id, AdjustedRelativeRisk, ConfidenceIntervalLow, ConfidenceIntervalHigh)
					SELECT	M.DrugName, M.Id
							, (E.[ExposedIncidenceRate] / NE.[NonExposedIncidenceRate])
							, (((E.[ExposedIncidenceRate] / NE.[NonExposedIncidenceRate]) * EXP((-1.96*SQRT(((CAST([ExposedNonCases] AS decimal(9,2))) / ((CAST([ExposedCases] AS decimal(9,2)))*((CAST([ExposedCases] AS decimal(9,2)))+(CAST([ExposedNonCases] AS decimal(9,2)))))) + ((CAST([NonExposedNonCases] AS decimal(9,2))) / ((CAST([NonExposedCases] AS decimal(9,2)))*((CAST([NonExposedCases] AS decimal(9,2)))+(CAST([NonExposedNonCases] AS decimal(9,2)))))))))))
							, (((E.[ExposedIncidenceRate] / NE.[NonExposedIncidenceRate]) * EXP((+1.96*SQRT(((CAST([ExposedNonCases] AS decimal(9,2))) / ((CAST([ExposedCases] AS decimal(9,2)))*((CAST([ExposedCases] AS decimal(9,2)))+(CAST([ExposedNonCases] AS decimal(9,2)))))) + ((CAST([NonExposedNonCases] AS decimal(9,2))) / ((CAST([NonExposedCases] AS decimal(9,2)))*((CAST([NonExposedCases] AS decimal(9,2)))+(CAST([NonExposedNonCases] AS decimal(9,2)))))))))))
						FROM Medication M
							INNER JOIN #ContingencyExposed E ON M.Id = E.Medication_Id
							INNER JOIN #ContingencyNonExposed NE ON M.Id = NE.Medication_Id
						WHERE (E.[ExposedIncidenceRate] > 0 AND NE.[NonExposedIncidenceRate] > 0)
		end
		else begin
			UPDATE #ContingencyNonExposed SET NonExposedIncidenceRate = (CAST([NonExposedCases] AS decimal(9,2))/(CAST([NonExposedCases] AS decimal(9,2)) + CAST([NonExposedPopulation] AS decimal(9,2)))) * 1000 WHERE [NonExposedCases] > 0 OR [NonExposedPopulation] > 0
			
			INSERT #ContingencyAdjustedRiskRatio
					(Drug, Medication_Id, AdjustedRelativeRisk, ConfidenceIntervalLow, ConfidenceIntervalHigh)
					SELECT	M.DrugName, M.Id
							, (E.[ExposedIncidenceRate] / NE.[NonExposedIncidenceRate])
							, (((E.[ExposedIncidenceRate] / NE.[NonExposedIncidenceRate]) * EXP((-1.96*SQRT(((CAST([ExposedPopulation] AS decimal(9,2))) / ((CAST([ExposedCases] AS decimal(9,2)))*((CAST([ExposedCases] AS decimal(9,2)))+(CAST([ExposedPopulation] AS decimal(9,2)))))) + ((CAST([NonExposedPopulation] AS decimal(9,2))) / ((CAST([NonExposedCases] AS decimal(9,2)))*((CAST([NonExposedCases] AS decimal(9,2)))+(CAST([NonExposedPopulation] AS decimal(9,2)))))))))))
							, (((E.[ExposedIncidenceRate] / NE.[NonExposedIncidenceRate]) * EXP((+1.96*SQRT(((CAST([ExposedPopulation] AS decimal(9,2))) / ((CAST([ExposedCases] AS decimal(9,2)))*((CAST([ExposedCases] AS decimal(9,2)))+(CAST([ExposedPopulation] AS decimal(9,2)))))) + ((CAST([NonExposedPopulation] AS decimal(9,2))) / ((CAST([NonExposedCases] AS decimal(9,2)))*((CAST([NonExposedCases] AS decimal(9,2)))+(CAST([NonExposedPopulation] AS decimal(9,2)))))))))))
						FROM Medication M 
							INNER JOIN #ContingencyExposed E ON M.Id = E.Medication_Id
							INNER JOIN #ContingencyNonExposed NE ON M.Id = NE.Medication_Id
						WHERE (E.[ExposedIncidenceRate] > 0 AND NE.[NonExposedIncidenceRate] > 0)
		end
	end

END

/**************************************************************************************************************************
**
**	Function: Create analytical stored procedure 
**
***************************************************************************************************************************/
CREATE Procedure [dbo].[spGenerateAnalysis]
(
	@ConditionId int
	, @CohortId int
	, @StartDate date
	, @FinishDate date
	, @TermID int
	, @IncludeRiskFactor bit = 0
	, @RateByCount bit = 1
	, @DebugPatientList bit = 0
	, @RiskFactorXml XML
	, @DebugMode bit = 0
)
--WITH ENCRYPTION
AS

/******************************************************************************
**	File: 
**	Name: dbo.spGenerateAnalysis
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
**	Date: 09 October 2015
**  Current: v4
**
****************************************************************************************************************************
**	Change History
****************************************************************************************************************************
**	Date:			Version		Author:		Description:
**	--------		--------	-------		-------------------------------------------
**  16/03/10		2			SK			FIXED  BUGS (Inner join on PatientMedication from MedicationCausality, Output Field Name for First table retun (List of adverse events) 
**  16/10/15		3			SK			FIXED  BUGS COUNT(DISTINCT) ON PATIENTID WHEN DETERMINING NON EXPOSED CASES
**	27/12/16		4			SK			Include cohort for population analysis
**
***************************************************************************************************************************/
BEGIN

	SET NOCOUNT ON

	-- DEBUGGING
	--DECLARE @ConditionId int
	--DECLARE @CohortId int
	--DECLARE @StartDate date
	--DECLARE @FinishDate date
	--DECLARE @TermID int
	--DECLARE @IncludeRiskFactor bit
	--DECLARE @RateByCount bit
	--DECLARE @DebugPatientList bit
	--DECLARE @RiskFactorXml XML
	--DECLARE @DebugMode int

	--SET @StartDate = '2009-07-01'
	--SET @FinishDate = '2016-10-28'
	--SET @ConditionId = 1
	--SET @CohortId = 1
	--SET @TermID = 86077
	----SET @TermID = 0
	--SET @IncludeRiskFactor = 0
	--SET @RateByCount = 1
	--SET @DebugPatientList = 0
	--SET @RiskFactorXml = '<Factors><Factor><Name>Age Group</Name><Option>Adolescent</Option></Factor></Factors>'
	--SET @DebugMode = 1
	
	/***********************************************************************************
	CLEAN UP FROM PREVIOUS RUN
	************************************************************************************/
	IF OBJECT_ID('tempdb..#PatientList', 'U') IS NOT NULL
	BEGIN
		DROP TABLE #PatientList
	END
	IF OBJECT_ID('tempdb..#PatientListRiskFactors', 'U') IS NOT NULL
	BEGIN
		DROP TABLE #PatientListRiskFactors
	END
	IF OBJECT_ID('tempdb..#DrugList', 'U') IS NOT NULL
	BEGIN
		DROP TABLE #DrugList
	END
	IF OBJECT_ID('tempdb..#DrugListDeduped', 'U') IS NOT NULL
	BEGIN
		DROP TABLE #DrugListDeduped
	END
	IF OBJECT_ID('tempdb..#ContingencyExposed', 'U') IS NOT NULL
	BEGIN
		DROP TABLE #ContingencyExposed
	END
	IF OBJECT_ID('tempdb..#ContingencyNonExposed', 'U') IS NOT NULL
	BEGIN
		DROP TABLE #ContingencyNonExposed
	END
	IF OBJECT_ID('tempdb..#ContingencyRiskRatio', 'U') IS NOT NULL
	BEGIN
		DROP TABLE #ContingencyRiskRatio
	END
	IF OBJECT_ID('tempdb..#ContingencyAdjustedRiskRatio', 'U') IS NOT NULL
	BEGIN
		DROP TABLE #ContingencyAdjustedRiskRatio
	END
	IF OBJECT_ID('tempdb..#PatientListRiskFactors', 'U') IS NOT NULL
	BEGIN
		DROP TABLE #PatientListRiskFactors
	END

	/***********************************************************************************
	PREPARE TEMP TABLES FOR NEW RUN
	************************************************************************************/
	CREATE TABLE #PatientList
		(Id int, StartDate date, FinishDate date)
	CREATE TABLE #Druglist
		(Patient_Id int, Drug nvarchar(100), Medication_Id int, StartDate date, FinishDate date, DaysContributed int, ADR int)
	CREATE TABLE #DrugListDeduped
		(Patient_Id int, Drug nvarchar(100), Medication_Id int, StartDate date, FinishDate date, DaysContributed int, ADR int)
	CREATE CLUSTERED INDEX [ix_deduped] ON #DrugListDeduped(Patient_Id)
	CREATE TABLE #ContingencyExposed
		(Drug nvarchar(100), Medication_Id int, ExposedCases int, ExposedNonCases int, ExposedPopulation decimal(9,2), ExposedIncidenceRate decimal(9,2))
	CREATE TABLE #ContingencyNonExposed
		(Drug nvarchar(100), Medication_Id int, NonExposedCases int, NonExposedNonCases int, NonExposedPopulation decimal(9,2), NonExposedIncidenceRate decimal(9,2))
	CREATE TABLE #ContingencyRiskRatio
		(Drug nvarchar(100), Medication_Id int, UnAdjustedRelativeRisk decimal(9,2), ConfidenceIntervalLow decimal(9,2), ConfidenceIntervalHigh decimal(9,2))
	CREATE TABLE #ContingencyAdjustedRiskRatio
		(Drug nvarchar(100), Medication_Id int, AdjustedRelativeRisk decimal(9,2), ConfidenceIntervalLow decimal(9,2), ConfidenceIntervalHigh decimal(9,2))
	CREATE TABLE #PatientListRiskFactors
		(Id int primary key IDENTITY(1,1) NOT NULL, PatientID int, RiskFactor varchar(20), RiskFactorCriteria varchar(MAX), RiskFactorOption varchar(30), RiskFactorOptionCriteria varchar(250), FactorMet bit default 0)

	/************************************************************************************
	GENERAL - PREPARE TEMPORARY PATIENT LIST TABLE (ALL PATIENTS CONTRIBUTING TO ANALYSIS 
	AND THE PERIOD OF TIME THEY ARE CONTRIBUTING)
	*************************************************************************************/
	if(@ConditionId > 0) begin
		EXEC spGeneratePatientListCondition @ConditionId = @ConditionId, @StartDate = @StartDate, @FinishDate = @FinishDate 
	end
	if(@CohortId > 0) begin
		EXEC spGeneratePatientListCohort @CohortId = @CohortId, @StartDate = @StartDate, @FinishDate = @FinishDate 
	end

	if(@DebugMode = 1) begin
		select * from #PatientList order by Id
	end
	
	/***********************************************************************************
	UNADJUSTED PREPARE DEDUPED DRUG HISTORY PER PATIENT 
	************************************************************************************/
	EXEC spGenerateDrugList @StartDate = @StartDate, @FinishDate = @FinishDate , @TermID = @TermId, @RiskFactorXml = @RiskFactorXml, @DebugMode = @DebugMode
	
	-- IF FRONT END HAS ONLY REQUESTED A LIST OF EVENTS THEN RETURN THIS NOW AND FALL OUT
	IF @TermID = 0 AND @DebugPatientList = 0
	BEGIN	
		SELECT iPCE.TerminologyMedDra_Id1 AS TerminologyMedDra_Id, itm.[MedDRATerm] 
			FROM PatientClinicalEvent iPCE 
				INNER JOIN MedicationCausality iMC ON iPCE.Id = iMC.ClinicalEvent_Id 
				INNER JOIN #Druglist iD ON iPCE.Patient_Id = iD.Patient_Id 
				INNER JOIN TerminologyMedDra itm ON iPCE.TerminologyMedDra_Id1 = itm.Id 
			WHERE (iMC.[NaranjoCausality] IN ('Definite', 'Probable', 'Possible') 
				OR iMC.[WHOCausality] IN ('Certain', 'Probable', 'Possible')) 
		GROUP BY iPCE.TerminologyMedDra_Id1, itm.MedDRATerm ORDER BY itm.MedDRATerm ASC
		RETURN
	END
	
	if(@DebugMode = 1) begin
		--SELECT PLRF.*, PL.[StartDate], PL.[FinishDate] FROM #PatientList PL INNER JOIN #PatientListRiskFactors PLRF ON PL.[PatientID] = PLRF.[PatientID] ORDER BY PL.[PatientID]  
		select * from #DrugListDeduped order by Patient_Id
	end

	/***********************************************************************************
	UNADJUSTED - CREATE CONTINGENCY
	************************************************************************************/
	EXEC spGenerateContingency @StartDate = @StartDate, @FinishDate = @FinishDate , @RateByCount = @RateByCount, @DebugMode = @DebugMode

	if(@DebugMode = 1) begin
		select * from #ContingencyExposed
		select * from #ContingencyNonExposed
		select * from #ContingencyRiskRatio
	end

	IF @IncludeRiskFactor = 0 -- RETURN CONTINGENCY TABLE FOR UNADJUSTED RISK RATIOS OF NO CONFOUNDING RISK FACTORS
	BEGIN
		-- ******* OUTPUT
		IF @DebugPatientList = 1
			BEGIN
				SELECT p.FirstName + ' ' + p.Surname AS PatientName, pl.Id, CONVERT(varchar(10), DLD.[StartDate], 120) AS StartDate, CONVERT(varchar(10), DLD.[FinishDate], 120) AS FinishDate, DLD.Drug, DLD.[DaysContributed], DLD.[ADR]
					FROM #PatientList pl
						INNER JOIN #DruglistDeduped dld ON pl.Id = dld.Patient_Id
						INNER JOIN Medication M ON dld.Medication_Id = m.Id
						INNER JOIN [Patient] P ON pl.Id = p.Id
					ORDER BY p.Surname, p.FirstName, dld.StartDate 
			END
		ELSE
			SELECT E.*, NE.[NonExposedCases], NE.[NonExposedNonCases], NE.[NonExposedPopulation], NE.[NonExposedIncidenceRate], ISNULL(R.[UnAdjustedRelativeRisk], 0) AS 'UnAdjustedRelativeRisk', ISNULL(R.[ConfidenceIntervalLow], 0) AS 'ConfidenceIntervalLow', ISNULL(R.[ConfidenceIntervalHigh], 0) AS 'ConfidenceIntervalHigh'
				FROM #ContingencyExposed E 
					INNER JOIN #ContingencyNonExposed NE ON E.Medication_Id = NE.Medication_Id
					LEFT JOIN #ContingencyRiskRatio R ON E.Medication_Id = R.Medication_Id 
				WHERE E.ExposedPopulation IS NOT NULL
				ORDER BY E.Medication_Id ASC
		RETURN -- EXIT STORED PROC
	END

	-- ********** NOTE: IF WE GET HERE THEN USER IS CHECKING RISK FACTORS

	/***********************************************************************************
	ADJUSTED - CREATE LIST OF PATIENTS RISK FACTORS
	************************************************************************************/
	EXEC spGenerateRiskFactors @StartDate = @StartDate, @FinishDate = @FinishDate, @DebugMode = @DebugMode

	if(@DebugMode = 1) begin
		select * from #PatientListRiskFactors
	end

	/***********************************************************************************
	ADJUSTED PREPARE DEDUPED DRUG HISTORY PER PATIENT WHO MEETS RISK FACTORS
	************************************************************************************/
	-- CLEAR TEMP TABLES USED PREVIOUSLY
	DELETE #Druglist
	DELETE #DrugListDeduped
	DELETE #ContingencyExposed
	DELETE #ContingencyNonExposed

	EXEC spGenerateDrugList @StartDate = @StartDate, @FinishDate = @FinishDate , @TermID = @TermId, @RiskFactorXml = @RiskFactorXml, @DebugMode = @DebugMode

	if(@DebugMode = 1) begin
		select plrf.*, pl.StartDate, pl.FinishDate FROM #PatientList pl INNER JOIN #PatientListRiskFactors plrf ON pl.Id = plrf.PatientID order by pl.Id
		select * from #DrugListDeduped order by Patient_Id
	end

	/***********************************************************************************
	ADJUSTED - CREATE CONTINGENCY
	************************************************************************************/
	EXEC spGenerateContingency @StartDate = @StartDate, @FinishDate = @FinishDate , @RateByCount = @RateByCount, @DebugMode = @DebugMode

	if(@DebugMode = 1) begin
		select * from #ContingencyExposed
		select * from #ContingencyNonExposed
		select * from #ContingencyAdjustedRiskRatio
	end

	/***********************************************************************************
	FINAL OUTPUT
	************************************************************************************/
	IF @DebugPatientList = 1
		BEGIN
			SELECT p.FirstName + ' ' + P.Surname AS PatientName, pl.Id, CONVERT(varchar(10), dld.[StartDate], 120) AS StartDate, CONVERT(varchar(10), dld.[FinishDate], 120) AS FinishDate, dld.Drug, dld.[DaysContributed], case when dld.ADR = 1 then 'Yes' else 'No' end as Reaction, prf.RiskFactor, prf.RiskFactorOption, case when FactorMet = 1 then 'Yes' else 'No' end as FactorMet
				FROM #PatientList pl
					INNER JOIN #DruglistDeduped dld ON pl.Id = dld.Patient_Id
					INNER JOIN Medication m ON dld.Medication_Id = m.Id
					INNER JOIN [Patient] P ON pl.Id = p.Id
					INNER JOIN #PatientListRiskFactors prf ON pl.Id = prf.PatientID 
				ORDER BY p.Surname, p.FirstName, dld.StartDate 
		END
	ELSE
		SELECT E.*, NE.[NonExposedCases], NE.[NonExposedNonCases], NE.[NonExposedPopulation], NE.[NonExposedIncidenceRate], ISNULL(R.[UnAdjustedRelativeRisk], 0) AS 'UnAdjustedRelativeRisk', ISNULL(AR.[AdjustedRelativeRisk], 0) AS 'AdjustedRelativeRisk', ISNULL(AR.[ConfidenceIntervalLow], 0) AS 'ConfidenceIntervalLow', ISNULL(AR.[ConfidenceIntervalHigh], 0) AS 'ConfidenceIntervalHigh'
			FROM #ContingencyExposed E 
				INNER JOIN #ContingencyNonExposed NE ON E.Medication_Id = NE.Medication_Id
				LEFT JOIN #ContingencyRiskRatio R ON E.Medication_Id = R.Medication_Id 
				LEFT JOIN #ContingencyAdjustedRiskRatio AR ON E.Medication_Id = AR.Medication_Id 
			WHERE E.ExposedPopulation IS NOT NULL
			ORDER BY E.Medication_Id ASC
	RETURN -- EXIT STORED PROC

	/***********************************************************************************
	CLEAN UP CURRENT RUN
	************************************************************************************/
	IF OBJECT_ID('tempdb..#PatientList', 'U') IS NOT NULL
	BEGIN
		DROP TABLE #PatientList
	END
	IF OBJECT_ID('tempdb..#PatientListRiskFactors', 'U') IS NOT NULL
	BEGIN
		DROP TABLE #PatientListRiskFactors
	END
	IF OBJECT_ID('tempdb..#DrugList', 'U') IS NOT NULL
	BEGIN
		DROP TABLE #DrugList
	END
	IF OBJECT_ID('tempdb..#DrugListDeduped', 'U') IS NOT NULL
	BEGIN
		DROP TABLE #DrugListDeduped
	END
	IF OBJECT_ID('tempdb..#ContingencyExposed', 'U') IS NOT NULL
	BEGIN
		DROP TABLE #ContingencyExposed
	END
	IF OBJECT_ID('tempdb..#ContingencyNonExposed', 'U') IS NOT NULL
	BEGIN
		DROP TABLE #ContingencyNonExposed
	END
	IF OBJECT_ID('tempdb..#ContingencyRiskRatio', 'U') IS NOT NULL
	BEGIN
		DROP TABLE #ContingencyRiskRatio
	END
	IF OBJECT_ID('tempdb..#ContingencyAdjustedRiskRatio', 'U') IS NOT NULL
	BEGIN
		DROP TABLE #ContingencyAdjustedRiskRatio
	END
	IF OBJECT_ID('tempdb..#PatientListRiskFactors', 'U') IS NOT NULL
	BEGIN
		DROP TABLE #PatientListRiskFactors
	END
	
END

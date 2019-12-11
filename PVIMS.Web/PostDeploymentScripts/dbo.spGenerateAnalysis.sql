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
**  Current: v5
**
****************************************************************************************************************************
**	Change History
****************************************************************************************************************************
**	Date:			Description:
**	--------		-------------------------------------------
**  16/03/10		FIXED  BUGS (Inner join on PatientMedication from MedicationCausality, Output Field Name for First table retun (List of adverse events) 
**  16/10/15		FIXED  BUGS COUNT(DISTINCT) ON PATIENTID WHEN DETERMINING NON EXPOSED CASES
**	27/12/16		Include cohort for population analysis
**	11/01/18		Replace MedicationCausality with ReportInstanceMedication and get terminology from ReportInstance
**	25/01/18		Cater for archiving
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
		SELECT iRI.TerminologyMedDra_Id, itm.[MedDRATerm] 
			FROM PatientClinicalEvent iPCE 
				INNER JOIN ReportInstance iRI on iPCE.PatientClinicalEventGuid = iRI.ContextGuid
				INNER JOIN ReportInstanceMedication iRIM on iRI.Id = iRIM.ReportInstance_Id
				INNER JOIN #Druglist iD ON iPCE.Patient_Id = iD.Patient_Id 
				INNER JOIN TerminologyMedDra itm ON iRI.TerminologyMedDra_Id = itm.Id 
			WHERE (iRIM.[NaranjoCausality] IN ('Definite', 'Probable', 'Possible') 
				OR iRIM.[WHOCausality] IN ('Certain', 'Probable', 'Possible')) AND iPCE.Archived = 0 
		GROUP BY iRI.TerminologyMedDra_Id, itm.MedDRATerm ORDER BY itm.MedDRATerm ASC
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

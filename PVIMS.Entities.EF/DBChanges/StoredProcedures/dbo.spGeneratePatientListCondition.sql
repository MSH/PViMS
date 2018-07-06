/**************************************************************************************************************************
**
**	Function: Create analytical stored procedure 
**  Sub Function: Create patient list
**
***************************************************************************************************************************/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[spGeneratePatientList]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[spGeneratePatientList]
GO
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[spGeneratePatientListCondition]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[spGeneratePatientListCondition]
GO

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
**	25/01/2018		v3			SIK			Check for record archiving
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
				WHERE c.Id = @ConditionId AND p.Archived = 0 and pc.Archived = 0 
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
				WHERE c.Id = @ConditionId AND p.Archived = 0 and pc.Archived = 0 
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
				WHERE c.Id = @ConditionId AND p.Archived = 0 and pc.Archived = 0  
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
				WHERE c.Id = @ConditionId AND p.Archived = 0 and pc.Archived = 0  
					AND DateStart >= @StartDate AND DateStart < @FinishDate
					AND OutcomeDate is not null and OutcomeDate <= @FinishDate

	INSERT INTO #PatientList
			(Id, StartDate, FinishDate)
		SELECT Id, Min(StartDate), MAX(FinishDate)
			 FROM #PatientListTemp GROUP BY Id

END

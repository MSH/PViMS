/**************************************************************************************************************************
**
**	Function: Create analytical stored procedure 
**  Sub Function: Create patient list
**
***************************************************************************************************************************/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[spGeneratePatientListCohort]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[spGeneratePatientListCohort]
GO

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
**  Current: v2
**
****************************************************************************************************************************
**	Change History
****************************************************************************************************************************
**	Date:			Version		Author:		Description:
**	--------		--------	-------		-------------------------------------------
**	25/01/2018		v2			SIK			Cater for record archiving
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
				WHERE cge.CohortGroup_Id = @CohortId and p.Archived = 0 and cge.Archived = 0 
					AND EnroledDate < @StartDate AND EnroledDate < @FinishDate
					AND (DeenroledDate is null OR (DeenroledDate is not null and DeenroledDate > @FinishDate))

	-- WRITE PATIENT RECORD FOR PATIENTS REGISTERED BEFORE AND ACTIVE DURING BUT INACTIVATE BY END
	INSERT INTO #PatientListTemp
			(Id, StartDate, FinishDate)
			SELECT P.Id, @StartDate, DeenroledDate 
				FROM Patient P
					INNER JOIN CohortGroupEnrolment cge ON p.Id = cge.Patient_Id 
				WHERE cge.CohortGroup_Id = @CohortId and p.Archived = 0 and cge.Archived = 0  
					AND EnroledDate < @StartDate AND EnroledDate < @FinishDate
					AND DeenroledDate is not null and DeenroledDate <= @FinishDate

	-- WRITE PATIENT RECORD FOR PATIENTS REGISTERED AFTER REPORT START BUT BEFORE END AND ACTIVE BY END OF REPORT
	INSERT INTO #PatientListTemp
			(Id, StartDate, FinishDate)
			SELECT P.Id, EnroledDate, @FinishDate 
				FROM Patient P
					INNER JOIN CohortGroupEnrolment cge ON p.Id = cge.Patient_Id 
				WHERE cge.CohortGroup_Id = @CohortId and p.Archived = 0 and cge.Archived = 0  
					AND EnroledDate >= @StartDate AND EnroledDate < @FinishDate
					AND (DeenroledDate is null OR (DeenroledDate is not null and DeenroledDate > @FinishDate))

	-- WRITE PATIENT RECORD FOR PATIENTS REGISTERED AFTER REPORT START BUT BEFORE END AND ACTIVE DURING BUT INACTIVATE BY END
	INSERT INTO #PatientListTemp
			(Id, StartDate, FinishDate)
			SELECT P.Id, EnroledDate, DeenroledDate 
				FROM Patient P
					INNER JOIN CohortGroupEnrolment cge ON p.Id = cge.Patient_Id 
				WHERE cge.CohortGroup_Id = @CohortId and p.Archived = 0 and cge.Archived = 0  
					AND EnroledDate >= @StartDate AND EnroledDate < @FinishDate
					AND DeenroledDate is not null and DeenroledDate <= @FinishDate

	INSERT INTO #PatientList
			(Id, StartDate, FinishDate)
		SELECT Id, Min(StartDate), MAX(FinishDate)
			 FROM #PatientListTemp GROUP BY Id

END

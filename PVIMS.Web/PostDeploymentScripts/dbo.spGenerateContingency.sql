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
**  12/01/2017		v2			SK			Fixed non exposed case and non case counts and get terminology from ReportInstance
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

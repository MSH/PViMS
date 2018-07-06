/**************************************************************************************************************************
**
**	Function: Create analytical stored procedure 
**  Sub Function: Create drug list
**
***************************************************************************************************************************/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[spGenerateRiskFactors]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[spGenerateRiskFactors]
GO

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

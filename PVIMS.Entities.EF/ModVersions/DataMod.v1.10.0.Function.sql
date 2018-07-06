/****** Object:  UserDefinedFunction [dbo].[CleanJson]    Script Date: 11/08/2017 13:03:34 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[CleanJsonForString]') AND type in (N'FN', N'IF', N'TF', N'FS', N'FT'))
DROP FUNCTION [dbo].[CleanJsonForString]
GO

CREATE FUNCTION [dbo].[CleanJsonForString] (@temp varchar(max))
RETURNS varchar(max)
WITH EXECUTE AS CALLER
AS
BEGIN

	SET @temp = REPLACE(@temp, '{','')
	SET @temp = REPLACE(@temp, '}','')
	SET @temp = REPLACE(@temp, '<Value>','')
	SET @temp = REPLACE(@temp, '</Value>','')
	SET @temp = REPLACE(@temp, '<Value />','')
	SET @temp = REPLACE(@temp, '<Value/>','')
	SET @temp = REPLACE(@temp, '"Id": ','')
	SET @temp = REPLACE(@temp, '"Display": ','')

	IF(@temp IS NULL or @temp = '') begin return -1 end

	IF(CHARINDEX(',', @temp)) > 0
	begin
		SET @temp = LEFT(@temp, CHARINDEX(',', @temp) - 1)
		SET @temp = REPLACE(@temp, ' ','')
		
		-- Remove non numeric characters
		SELECT @temp = LEFT(SUBSTRING(@temp, PATINDEX('%[0-9.-]%', @temp), 8000), PATINDEX('%[^0-9.-]%', SUBSTRING(@temp, PATINDEX('%[0-9.-]%', @temp), 8000) + 'X') -1)
	end
	IF CHARINDEX('.', @temp) > 0 begin
		SET @temp = 0
	end
	
    RETURN(cast(@temp as varchar(max)));
END;



GO



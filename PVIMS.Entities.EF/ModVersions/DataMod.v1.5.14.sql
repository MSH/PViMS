DECLARE @ScriptFileName nvarchar(200)
	, @RunDate datetime
SET @ScriptFileName = 'Mods.Data.v1.5.14.sql'

SELECT @RunDate = RunDate  FROM PostDeployment  WHERE ScriptFileName = @ScriptFileName

IF(@RunDate IS NOT NULL) BEGIN
	
	DECLARE @VersionErrorMessage VARCHAR(1024)
	SET @VersionErrorMessage = 'This script has already been executed.'
	RAISERROR(@VersionErrorMessage,16,1)
	RETURN;
	
END

SET NOCOUNT ON

BEGIN TRAN A1

IF NOT EXISTS(select Id from SiteContactDetail where ContactType = 1) begin
	insert into SiteContactDetail (ContactType, OrganisationName, ContactFirstName, ContactSurname, StreetAddress, City, Created, CreatedBy_Id) values (1, '', '', '', '', '', GETDATE(), (select top 1 Id from [User] where UserName = 'Admin'))
end

IF NOT EXISTS(select Id from SiteContactDetail where ContactType = 2) begin
	insert into SiteContactDetail (ContactType, OrganisationName, ContactFirstName, ContactSurname, StreetAddress, City, Created, CreatedBy_Id) values (2, '', '', '', '', '', GETDATE(), (select top 1 Id from [User] where UserName = 'Admin'))
end

PRINT 'All Data Modified'

INSERT INTO PostDeployment
		(ScriptGuid, ScriptFileName, ScriptDescription, RunDate, StatusCode, StatusMessage, RunRank)
	select NEWID(), @ScriptFileName, @ScriptFileName, GETDATE(), 200, NULL, (select max(RunRank) + 1 from PostDeployment)

COMMIT TRAN A1

PRINT ''
PRINT '** SCRIPT COMPLETED SUCCESSFULLY **'

select * from sitecontactdetail
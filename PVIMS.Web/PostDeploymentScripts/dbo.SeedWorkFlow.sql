/**************************************************************************************************************************
**
**	Function:  SEED WORK FLOW
**
***************************************************************************************************************************/
               
-- ***************** WORK FLOW
INSERT INTO WorkFlow ([Description], WorkFlowGuid)
	SELECT 'New Active Surveilliance Report', '892F3305-7819-4F18-8A87-11CBA3AEE219'
INSERT INTO WorkFlow ([Description], WorkFlowGuid)
	SELECT 'New Spontaneous Surveilliance Report', '4096D0A3-45F7-4702-BDA1-76AEDE41B986'

-- ***************** ACTIVITIES
INSERT INTO Activity ([WorkFlow_Id], QualifiedName, ActivityType)
	SELECT Id, 'Confirm Report Data', 1 FROM WorkFlow where [Description] = 'New Active Surveilliance Report'
INSERT INTO Activity ([WorkFlow_Id], QualifiedName, ActivityType)
	SELECT Id, 'Set MedDRA and Causality', 1 FROM WorkFlow where [Description] = 'New Active Surveilliance Report'
INSERT INTO Activity ([WorkFlow_Id], QualifiedName, ActivityType)
	SELECT Id, 'Extract E2B', 1 FROM WorkFlow where [Description] = 'New Active Surveilliance Report'
INSERT INTO Activity ([WorkFlow_Id], QualifiedName, ActivityType)
	SELECT Id, 'Confirm Report Data', 1 FROM WorkFlow where [Description] = 'New Spontaneous Surveilliance Report'
INSERT INTO Activity ([WorkFlow_Id], QualifiedName, ActivityType)
	SELECT Id, 'Set MedDRA and Causality', 1 FROM WorkFlow where [Description] = 'New Spontaneous Surveilliance Report'
INSERT INTO Activity ([WorkFlow_Id], QualifiedName, ActivityType)
	SELECT Id, 'Extract E2B', 1 FROM WorkFlow where [Description] = 'New Spontaneous Surveilliance Report'

-- ***************** ACTIVITY EXECUTION
INSERT INTO ActivityExecutionStatus (Activity_Id, [Description], [FriendlyDescription])
	SELECT a.Id, 'UNCONFIRMED', 'Report submitted for confirmation' FROM Activity a INNER JOIN WorkFlow wf on a.WorkFlow_Id = wf.Id WHERE QualifiedName = 'Confirm Report Data' and wf.[Description] = 'New Active Surveilliance Report' 
INSERT INTO ActivityExecutionStatus (Activity_Id, [Description], [FriendlyDescription])
	SELECT a.Id, 'CONFIRMED', 'Report confirmed by technician' FROM Activity a INNER JOIN WorkFlow wf on a.WorkFlow_Id = wf.Id WHERE QualifiedName = 'Confirm Report Data' and wf.[Description] = 'New Active Surveilliance Report' 
INSERT INTO ActivityExecutionStatus (Activity_Id, [Description], [FriendlyDescription])
	SELECT a.Id, 'DELETED', 'Report deleted by technician' FROM Activity a INNER JOIN WorkFlow wf on a.WorkFlow_Id = wf.Id WHERE QualifiedName = 'Confirm Report Data' and wf.[Description] = 'New Active Surveilliance Report' 
INSERT INTO ActivityExecutionStatus (Activity_Id, [Description], [FriendlyDescription])
	SELECT a.Id, 'UNCONFIRMED', 'Report submitted for confirmation' FROM Activity a INNER JOIN WorkFlow wf on a.WorkFlow_Id = wf.Id WHERE QualifiedName = 'Confirm Report Data' and wf.[Description] = 'New Spontaneous Surveilliance Report' 
INSERT INTO ActivityExecutionStatus (Activity_Id, [Description], [FriendlyDescription])
	SELECT a.Id, 'CONFIRMED', 'Report confirmed by technician' FROM Activity a INNER JOIN WorkFlow wf on a.WorkFlow_Id = wf.Id WHERE QualifiedName = 'Confirm Report Data' and wf.[Description] = 'New Spontaneous Surveilliance Report' 
INSERT INTO ActivityExecutionStatus (Activity_Id, [Description], [FriendlyDescription])
	SELECT a.Id, 'DELETED', 'Report deleted by technician' FROM Activity a INNER JOIN WorkFlow wf on a.WorkFlow_Id = wf.Id WHERE QualifiedName = 'Confirm Report Data' and wf.[Description] = 'New Spontaneous Surveilliance Report' 

INSERT INTO ActivityExecutionStatus (Activity_Id, [Description], [FriendlyDescription])
	SELECT a.Id, 'NOTSET', 'Report ready for MedDRA and Causality' FROM Activity a INNER JOIN WorkFlow wf on a.WorkFlow_Id = wf.Id WHERE QualifiedName = 'Set MedDRA and Causality' and wf.[Description] = 'New Active Surveilliance Report' 
INSERT INTO ActivityExecutionStatus (Activity_Id, [Description], [FriendlyDescription])
	SELECT a.Id, 'MEDDRASET', 'MedDRA term set by technician' FROM Activity a INNER JOIN WorkFlow wf on a.WorkFlow_Id = wf.Id WHERE QualifiedName = 'Set MedDRA and Causality' and wf.[Description] = 'New Active Surveilliance Report' 
INSERT INTO ActivityExecutionStatus (Activity_Id, [Description], [FriendlyDescription])
	SELECT a.Id, 'CAUSALITYSET', 'Causality set by technician' FROM Activity a INNER JOIN WorkFlow wf on a.WorkFlow_Id = wf.Id WHERE QualifiedName = 'Set MedDRA and Causality' and wf.[Description] = 'New Active Surveilliance Report' 
INSERT INTO ActivityExecutionStatus (Activity_Id, [Description], [FriendlyDescription])
	SELECT a.Id, 'NOTSET', 'Report ready for MedDRA and Causality' FROM Activity a INNER JOIN WorkFlow wf on a.WorkFlow_Id = wf.Id WHERE QualifiedName = 'Set MedDRA and Causality' and wf.[Description] = 'New Spontaneous Surveilliance Report' 
INSERT INTO ActivityExecutionStatus (Activity_Id, [Description], [FriendlyDescription])
	SELECT a.Id, 'MEDDRASET', 'MedDRA term set by technician' FROM Activity a INNER JOIN WorkFlow wf on a.WorkFlow_Id = wf.Id WHERE QualifiedName = 'Set MedDRA and Causality' and wf.[Description] = 'New Spontaneous Surveilliance Report' 
INSERT INTO ActivityExecutionStatus (Activity_Id, [Description], [FriendlyDescription])
	SELECT a.Id, 'CAUSALITYSET', 'Causality set by technician' FROM Activity a INNER JOIN WorkFlow wf on a.WorkFlow_Id = wf.Id WHERE QualifiedName = 'Set MedDRA and Causality' and wf.[Description] = 'New Spontaneous Surveilliance Report' 

INSERT INTO ActivityExecutionStatus (Activity_Id, [Description], [FriendlyDescription])
	SELECT a.Id, 'NOTGENERATED', 'Report ready for E2B submission' FROM Activity a INNER JOIN WorkFlow wf on a.WorkFlow_Id = wf.Id WHERE QualifiedName = 'Extract E2B' and wf.[Description] = 'New Active Surveilliance Report' 
INSERT INTO ActivityExecutionStatus (Activity_Id, [Description], [FriendlyDescription])
	SELECT a.Id, 'E2BINITIATED', 'E2B data generated for report' FROM Activity a INNER JOIN WorkFlow wf on a.WorkFlow_Id = wf.Id WHERE QualifiedName = 'Extract E2B' and wf.[Description] = 'New Active Surveilliance Report' 
INSERT INTO ActivityExecutionStatus (Activity_Id, [Description], [FriendlyDescription])
	SELECT a.Id, 'E2BGENERATED', 'E2B report generated' FROM Activity a INNER JOIN WorkFlow wf on a.WorkFlow_Id = wf.Id WHERE QualifiedName = 'Extract E2B' and wf.[Description] = 'New Active Surveilliance Report' 
INSERT INTO ActivityExecutionStatus (Activity_Id, [Description], [FriendlyDescription])
	SELECT a.Id, 'E2BSUBMITTED', 'E2B report submitted' FROM Activity a INNER JOIN WorkFlow wf on a.WorkFlow_Id = wf.Id WHERE QualifiedName = 'Extract E2B' and wf.[Description] = 'New Active Surveilliance Report' 
INSERT INTO ActivityExecutionStatus (Activity_Id, [Description], [FriendlyDescription])
	SELECT a.Id, 'NOTGENERATED', 'Report ready for E2B submission' FROM Activity a INNER JOIN WorkFlow wf on a.WorkFlow_Id = wf.Id WHERE QualifiedName = 'Extract E2B' and wf.[Description] = 'New Spontaneous Surveilliance Report' 
INSERT INTO ActivityExecutionStatus (Activity_Id, [Description], [FriendlyDescription])
	SELECT a.Id, 'E2BINITIATED', 'E2B data generated for report' FROM Activity a INNER JOIN WorkFlow wf on a.WorkFlow_Id = wf.Id WHERE QualifiedName = 'Extract E2B' and wf.[Description] = 'New Spontaneous Surveilliance Report' 
INSERT INTO ActivityExecutionStatus (Activity_Id, [Description], [FriendlyDescription])
	SELECT a.Id, 'E2BGENERATED', 'E2B report generated' FROM Activity a INNER JOIN WorkFlow wf on a.WorkFlow_Id = wf.Id WHERE QualifiedName = 'Extract E2B' and wf.[Description] = 'New Spontaneous Surveilliance Report' 
INSERT INTO ActivityExecutionStatus (Activity_Id, [Description], [FriendlyDescription])
	SELECT a.Id, 'E2BSUBMITTED', 'E2B report submitted' FROM Activity a INNER JOIN WorkFlow wf on a.WorkFlow_Id = wf.Id WHERE QualifiedName = 'Extract E2B' and wf.[Description] = 'New Spontaneous Surveilliance Report' 

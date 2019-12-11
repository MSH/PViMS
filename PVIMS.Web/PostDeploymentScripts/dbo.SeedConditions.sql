/**************************************************************************************************************************
**
**	Function:  SEED CONDITIONS
**
***************************************************************************************************************************/
               
-- ***************** CONDITIONS
SET IDENTITY_INSERT [dbo].[Condition] ON
INSERT [dbo].[Condition] ([Id], [Description], [Chronic]) VALUES (1, N'TB', 1)
INSERT [dbo].[Condition] ([Id], [Description], [Chronic]) VALUES (2, N'HIV', 1)
INSERT [dbo].[Condition] ([Id], [Description], [Chronic]) VALUES (3, N'Malaria', 1)
SET IDENTITY_INSERT [dbo].[Condition] OFF

SET IDENTITY_INSERT [dbo].[ConditionMedDra] ON 
INSERT [dbo].[ConditionMedDra] ([Id], [Condition_Id], [TerminologyMedDra_Id]) VALUES (1, 1, 52627)
INSERT [dbo].[ConditionMedDra] ([Id], [Condition_Id], [TerminologyMedDra_Id]) VALUES (2, 2, 28879)
INSERT [dbo].[ConditionMedDra] ([Id], [Condition_Id], [TerminologyMedDra_Id]) VALUES (3, 2, 35344)
INSERT [dbo].[ConditionMedDra] ([Id], [Condition_Id], [TerminologyMedDra_Id]) VALUES (4, 2, 35311)
INSERT [dbo].[ConditionMedDra] ([Id], [Condition_Id], [TerminologyMedDra_Id]) VALUES (5, 2, 57137)
INSERT [dbo].[ConditionMedDra] ([Id], [Condition_Id], [TerminologyMedDra_Id]) VALUES (6, 1, 34473)
INSERT [dbo].[ConditionMedDra] ([Id], [Condition_Id], [TerminologyMedDra_Id]) VALUES (7, 1, 36279)
INSERT [dbo].[ConditionMedDra] ([Id], [Condition_Id], [TerminologyMedDra_Id]) VALUES (8, 1, 40826)
INSERT [dbo].[ConditionMedDra] ([Id], [Condition_Id], [TerminologyMedDra_Id]) VALUES (9, 3, 32803)
INSERT [dbo].[ConditionMedDra] ([Id], [Condition_Id], [TerminologyMedDra_Id]) VALUES (10, 3, 32811)
INSERT [dbo].[ConditionMedDra] ([Id], [Condition_Id], [TerminologyMedDra_Id]) VALUES (11, 3, 32812)
INSERT [dbo].[ConditionMedDra] ([Id], [Condition_Id], [TerminologyMedDra_Id]) VALUES (12, 3, 35375)
INSERT [dbo].[ConditionMedDra] ([Id], [Condition_Id], [TerminologyMedDra_Id]) VALUES (13, 2, 91921)
SET IDENTITY_INSERT [dbo].[ConditionMedDra] OFF


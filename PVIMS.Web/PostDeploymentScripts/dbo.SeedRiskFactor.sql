/**************************************************************************************************************************
**
**	Function:  SEED RISK FACTORS
**
***************************************************************************************************************************/
               
-- ***************** RISK FACTOR
SET IDENTITY_INSERT [dbo].[RiskFactor] ON 
INSERT [dbo].[RiskFactor] ([Id], [FactorName], [Criteria], [Display], [IsSystem], [Active]) VALUES (1, N'Age Group', N'(select datediff(mm, DateOfBirth, ''#ContextStart#'') from Patient where Id = PatientId)', N'Age Group', 1, 1)
INSERT [dbo].[RiskFactor] ([Id], [FactorName], [Criteria], [Display], [IsSystem], [Active]) VALUES (2, N'Secondary Condition', N'exists (select pc.Id from PatientCondition pc inner join TerminologyMedDRA tm on pc.TerminologyMedDRA_Id = tm.Id inner join ConditionMedDRA cm on tm.Id = cm.TerminologyMedDRA_Id inner join Condition c on cm.Condition_Id = c.Id where pc.Patient_Id = PatientId and c.[Description] = ''#ContextOption#'')', N'Secondary Condition', 1, 1)
INSERT [dbo].[RiskFactor] ([Id], [FactorName], [Criteria], [Display], [IsSystem], [Active]) VALUES (3, N'Gender', N'(select dbo.CleanJsonForString(cast(CustomAttributesXmlSerialised.query(''/CustomAttributeSet/CustomSelectionAttribute[Key/node() = "Gender"]/Value[1]'') as varchar(max))) from Patient where Id = PatientId) = ''#ContextOption#''', N'Gender', 1, 1)
INSERT [dbo].[RiskFactor] ([Id], [FactorName], [Criteria], [Display], [IsSystem], [Active]) VALUES (4, N'Pregnancy Status', N'(select top 1 div.InstanceValue from Encounter e INNER JOIN DatasetInstance di on e.Id = di.ContextID INNER JOIN DatasetInstanceValue div on div.DatasetInstance_Id = di.Id INNER JOIN DatasetElement de on div.DatasetElement_Id = de.Id where de.DatasetElementGuid = ''9EF95B01-3EA7-433B-B687-E30BB35DE8CA'' and e.Patient_Id = PatientId and e.EncounterDate <= ''#ContextStart#'' order by e.EncounterDate desc) = ''#ContextOption#''', N'Pregnancy Status', 1, 1)
INSERT [dbo].[RiskFactor] ([Id], [FactorName], [Criteria], [Display], [IsSystem], [Active]) VALUES (5, N'Breastfeeding Status', N'(select top 1 div.InstanceValue from Encounter e INNER JOIN DatasetInstance di on e.Id = di.ContextID INNER JOIN DatasetInstanceValue div on div.DatasetInstance_Id = di.Id INNER JOIN DatasetElement de on div.DatasetElement_Id = de.Id where de.DatasetElementGuid = ''DDFACDDF-C383-49AA-8BAE-C36E4BFE64EE'' and e.Patient_Id = PatientId and e.EncounterDate <= ''#ContextStart#'' order by e.EncounterDate desc) = ''#ContextOption#''', N'Breastfeeding Status', 1, 1)
SET IDENTITY_INSERT [dbo].[RiskFactor] OFF

-- ***************** RISK FACTOR CRITERIA
SET IDENTITY_INSERT [dbo].[RiskFactorOption] ON 
INSERT [dbo].[RiskFactorOption] ([Id], [OptionName], [Criteria], [Display], [RiskFactor_Id]) VALUES (1, N'Adolescent <= 16 years', N'between 133 and 192', N'Adolescent (11 to 16 years)', 1)
INSERT [dbo].[RiskFactorOption] ([Id], [OptionName], [Criteria], [Display], [RiskFactor_Id]) VALUES (2, N'Adult <= 69 years', N'between 193 and 828', N'Adult <= (16 to 69 years)', 1)
INSERT [dbo].[RiskFactorOption] ([Id], [OptionName], [Criteria], [Display], [RiskFactor_Id]) VALUES (3, N'Child <=11 years', N'between 49 and 132', N'Child (4 to 11 years)', 1)
INSERT [dbo].[RiskFactorOption] ([Id], [OptionName], [Criteria], [Display], [RiskFactor_Id]) VALUES (4, N'Elderly > 69 years', N'> 828', N'Elderly > (over 69 years)', 1)
INSERT [dbo].[RiskFactorOption] ([Id], [OptionName], [Criteria], [Display], [RiskFactor_Id]) VALUES (5, N'Infant <=4 years', N'between 2 and 48', N'Infant (1 month to 4 years)', 1)
INSERT [dbo].[RiskFactorOption] ([Id], [OptionName], [Criteria], [Display], [RiskFactor_Id]) VALUES (6, N'Neonate <= 1 month', N'between 0 and 1', N'Neonate (0 to 1 month)', 1)
INSERT [dbo].[RiskFactorOption] ([Id], [OptionName], [Criteria], [Display], [RiskFactor_Id]) VALUES (7, N'HIV', N'HIV', N'Has HIV', 2)
INSERT [dbo].[RiskFactorOption] ([Id], [OptionName], [Criteria], [Display], [RiskFactor_Id]) VALUES (8, N'Malaria', N'Malaria', N'Has Malaria', 2)
INSERT [dbo].[RiskFactorOption] ([Id], [OptionName], [Criteria], [Display], [RiskFactor_Id]) VALUES (9, N'TB', N'TB', N'Has TB', 2)
INSERT [dbo].[RiskFactorOption] ([Id], [OptionName], [Criteria], [Display], [RiskFactor_Id]) VALUES (10, N'Male', N'1', N'Is Male', 3)
INSERT [dbo].[RiskFactorOption] ([Id], [OptionName], [Criteria], [Display], [RiskFactor_Id]) VALUES (11, N'Female', N'2', N'Is Female', 3)
INSERT [dbo].[RiskFactorOption] ([Id], [OptionName], [Criteria], [Display], [RiskFactor_Id]) VALUES (12, N'Yes', N'Yes', N'Is Pregnant', 4)
INSERT [dbo].[RiskFactorOption] ([Id], [OptionName], [Criteria], [Display], [RiskFactor_Id]) VALUES (13, N'No', N'No', N'Is Not Pregnant', 4)
INSERT [dbo].[RiskFactorOption] ([Id], [OptionName], [Criteria], [Display], [RiskFactor_Id]) VALUES (14, N'Yes', N'Yes', N'Is Breastfeeding', 5)
INSERT [dbo].[RiskFactorOption] ([Id], [OptionName], [Criteria], [Display], [RiskFactor_Id]) VALUES (15, N'No', N'No', N'Is Not Breastfeeding', 5)
SET IDENTITY_INSERT [dbo].[RiskFactorOption] OFF

/**************************************************************************************************************************
**
**	Function: SEED METAPAGES
**			  SEED METAREPORTS
**
***************************************************************************************************************************/

-- ***************** METAPAGES
SET IDENTITY_INSERT [dbo].[MetaPage] ON
INSERT [dbo].[MetaPage] ([Id], [metapage_guid], [PageName], [PageDefinition], [MetaDefinition], [Breadcrumb], [IsSystem], [IsVisible]) 
	VALUES (1, N'a63e9f29-a22f-43df-87a0-d0c8dec50548', N'Home', N'Home', N'*** NOT DEFINED ***', N'Home', 1, 1)
INSERT [dbo].[MetaPage] ([Id], [metapage_guid], [PageName], [PageDefinition], [MetaDefinition], [Breadcrumb], [IsSystem], [IsVisible]) 
	VALUES (2, N'cde6d1b5-6a09-47b2-a304-abbcc2c94dac', N'Reference Page 1', N'Reference Page 1', N'*** NOT DEFINED ***', N'Reference/FDA Drug Safety Communication', 1, 0)
INSERT [dbo].[MetaPage] ([Id], [metapage_guid], [PageName], [PageDefinition], [MetaDefinition], [Breadcrumb], [IsSystem], [IsVisible]) 
	VALUES (3, N'8d62faa0-6fcd-4f1f-8714-924afdc5b03d', N'Reference Page 2', N'Reference Page 2', N'*** NOT DEFINED ***', N'Reference/Standards', 1, 0)
INSERT [dbo].[MetaPage] ([Id], [metapage_guid], [PageName], [PageDefinition], [MetaDefinition], [Breadcrumb], [IsSystem], [IsVisible]) 
	VALUES (4, N'14589ffb-506a-4299-9e1a-fef1c88ae881', N'Reference Page 3', N'Reference Page 3', N'*** NOT DEFINED ***', N'Reference/Causality Scales', 1, 0)
INSERT [dbo].[MetaPage] ([Id], [metapage_guid], [PageName], [PageDefinition], [MetaDefinition], [Breadcrumb], [IsSystem], [IsVisible]) 
	VALUES (5, N'1e7fb80d-2ce2-4481-a9ba-d0538d425a5d', N'Reference Page 4', N'Reference Page 4', N'*** NOT DEFINED ***', N'Reference/Grading Scales', 1, 0)
INSERT [dbo].[MetaPage] ([Id], [metapage_guid], [PageName], [PageDefinition], [MetaDefinition], [Breadcrumb], [IsSystem], [IsVisible]) 
	VALUES (6, N'89cadd84-eb24-4c70-b769-9183a5a7405a', N'Reference', N'Reference', N'*** NOT DEFINED ***', N'Reference', 1, 1)
INSERT [dbo].[MetaPage] ([Id], [metapage_guid], [PageName], [PageDefinition], [MetaDefinition], [Breadcrumb], [IsSystem], [IsVisible]) 
	VALUES (7, N'942f501a-1f47-49a3-9f60-6814ea46c482', N'FAQ', N'FAQ', N'*** NOT DEFINED ***', N'FAQ', 1, 1)
SET IDENTITY_INSERT [dbo].[MetaPage] OFF

-- ***************** METAWIDGETS
INSERT [dbo].[MetaWidget] ([metawidget_guid], [WidgetName], [WidgetDefinition], [Content], [WidgetType_Id], [MetaPage_Id], [WidgetLocation], [WidgetStatus], [Icon]) 
SELECT N'283ff0bf-cf13-4200-8f32-1ead616409a2', N'National Guidelines', N'', N'
	<WidgetList>
		<ListItem>
			<Title>07 January 2016 - FDC Use</Title>
			<Content>
				<p><a href="http://www.sahivsoc.org//upload/documents/FDC%20in%20place%20of%203TC%20updated.pdf" target="_blank">NDoH Advisory: use of FDCs to reduce use of single-agent lamivudine tablets</a><br /><br />This document, and the associated circular (FDC ARV Circular, dated 25.05.2015) provides guidance on how to prescribe FDCs. This reduces the need to use single-agent lamivudine tablets.</p>
			</Content>
		</ListItem> 
		<ListItem>
			<Title>06 August 2015 - FDC Circular</Title>
			<Content>
				<p><a href="http://www.sahivsoc.org//upload/documents/FDC%20ARV%20Circular%2025052015.pdf" target="_blank">Circular</a><br /><br />There are four fixed-dose ARV combinations available. Here are the NDoH guidelines for their use in first and second-line ART regimens.</p>
			</Content>
		</ListItem> 
		<ListItem>
			<Title>04 June 2015 - Consolidated ART guidelines</Title>
			<Content>
				<p><a href="http://www.sahivsoc.org//upload/documents/ART%20Guidelines%2015052015.pdf" target="_blank">New Department of Health National Consolidated ART Guidelines</a><br />April 2015 guidelines for the prevention of mother-to-child transmission of HIV (PMTCT) and the management of HIV in children, adolescents and adults</p>
			</Content>
		</ListItem> 
	</WidgetList>
	', 3, Id, 1, 1, NULL
	FROM MetaPage WHERE [metapage_guid] = 'a63e9f29-a22f-43df-87a0-d0c8dec50548'

INSERT [dbo].[MetaWidget] ([metawidget_guid], [WidgetName], [WidgetDefinition], [Content], [WidgetType_Id], [MetaPage_Id], [WidgetLocation], [WidgetStatus], [Icon]) 
SELECT N'5b19c085-af51-4e6b-934d-1a1ea5049803', N'Pharmacovigilance Study', N'', N'
	<WidgetList>
		<ListItem>
			<Title>22 November 2012 - Vietnam sentinel survey</Title>
			<Content>
				<p><a href="http://127.0.0.1:83/temp/sentinel.ppt" target="_blank">Sentinel survey</a><br />Progress with the implementation of the Sentinel Site-based Active Surveillance for Safety of Antiretrovirals in Vietnam</p>
			</Content>
		</ListItem> 
	</WidgetList>
	', 3, Id, 2, 1, NULL
	FROM MetaPage WHERE [metapage_guid] = 'a63e9f29-a22f-43df-87a0-d0c8dec50548'

INSERT [dbo].[MetaWidget] ([metawidget_guid], [WidgetName], [WidgetDefinition], [Content], [WidgetType_Id], [MetaPage_Id], [WidgetLocation], [WidgetStatus], [Icon]) 
SELECT N'd1cacd7a-b5a3-46f7-bf90-76768f5617a9', N'FDA Drug Safety Communication', N'', N'
	<WidgetList>
		<ListItem>
			<Title>Safety Announcement</Title>
			<Content>
				<p>The U.S. Food and Drug Administration (FDA) is asking drug manufacturers to limit the strength of acetaminophen in prescription drug products, which are predominantly combinations of acetaminophen and opioids. This action will limit the amount of acetaminophen in these products to 325 mg per tablet, capsule, or other dosage unit, making these products safer for patients.</p>
				<p>In addition, a Boxed Warning highlighting the potential for severe liver injury and a Warning highlighting the potential for allergic reactions (e.g., swelling of the face, mouth, and throat, difficulty breathing, itching, or rash) are being added to the label of all prescription drug products that contain acetaminophen.</p>
				<p>These actions will help to reduce the risk of severe liver injury and allergic reactions associated with acetaminophen.</p>
				<p>Acetaminophen is widely and effectively used in both prescription and over-the-counter (OTC) products to reduce pain and fever. It is one of the most commonly-used drugs in the United States. Examples of prescription products that contain acetaminophen include hydrocodone with acetaminophen (Vicodin, Lortab), and oxycodone with acetaminophen (Tylox, Percocet).</p>
				<p>OTC products containing acetaminophen (e.g., Tylenol) are not affected by this action. Information about the potential for liver injury is already required on the label for OTC products containing acetaminophen. FDA is continuing to evaluate ways to reduce the risk of acetaminophen related liver injury from OTC products. Additional safety measures relating to OTC acetaminophen products will be taken through separate action, such as a rulemaking as part of the ongoing OTC monograph proceeding for internal analgesic drug products</p>
			</Content>
		</ListItem> 
		<ListItem>
			<Title>Additional Information For Patients</Title>
			<Content>
				<ul>
				<li>Acetaminophen-containing prescription products are safe and effective when used as directed, though all medications carry some risks.</li>
				<li>Do not stop taking your prescription pain medicine unless told to do so by your healthcare professional.</li>
				<li>Carefully read all labels for prescription and OTC medicines and ask the pharmacist if your prescription pain medicine contains acetaminophen.</li>
				<li>Do not take more than one product that contains acetaminophen at any given time.</li>
				<li>Do not take more of an acetaminophen-containing medicine than directed.</li>
				<li>Do not drink alcohol when taking medicines that contain acetaminophen.</li>
				<li>Stop taking your medication and seek medical help immediately if you:</li>
				<ul>
				<li>Think you have taken more acetaminophen than directed or</li>
				<li>Experience allergic reactions such as swelling of the face, mouth, and throat, difficulty breathing, itching, or rash.</li>
				</ul>
				<li>Report side effects to FDA''s MedWatch program using the information in the "Contact Us" box at the bottom of the page.</li>
				</ul>
			</Content>
		</ListItem> 
		<ListItem>
			<Title>Additional Information For Healthcare Professionals</Title>
			<Content>
				<ul>
				<li>The maximum amount of acetaminophen in a prescription tablet, capsule, or other dosage unit will be limited to 325 mg. However, the total number of tablets or capsules that may be prescribed and the time intervals at which they may be prescribed will not change as a result of the lower amount of acetaminophen. For example, for a product that previously contained 500 mg of acetaminophen with an opioid and was prescribed as 1-2 tablets every 4-6 hours, once reformulated to contain 325 mg of acetaminophen, the dosing instructions can remain unchanged.
				<ul>
				<li>Advise patients not to exceed the acetaminophen maximum total daily dose (4 grams/day).</li>
				<li>Severe liver injury, including cases of acute liver failure resulting in liver transplant and death, has been reported with the use of acetaminophen.</li>
				<li>Educate patients about the importance of reading all prescription and OTC labels to ensure they are not taking multiple acetaminophen-containing products.</li>
				<li>Advise patients not to drink alcohol while taking acetaminophen-containing medications.</li>
				<li>Rare cases of anaphylaxis and other hypersensitivity reactions have occurred with the use of acetaminophen.</li>
				<li>Advise patients to seek medical help immediately if they have taken more acetaminophen than directed or experience swelling of the face, mouth, and throat, difficulty breathing, itching, and rash.</li>
				<li>Report adverse events to FDA''s MedWatch program using the information in the "Contact Us" box at the bottom of the page</li>
				</ul>
				</li>
				</ul>
			</Content>
		</ListItem> 
		<ListItem>
			<Title>Data Summary and Discussion</Title>
			<Content>
				<ul>
				<li>A number of studies have tried to answer the question of how common liver injury is in relation to the use of acetaminophen. Although many questions remain about the full scope of the problem, the following examples indicate what is known about the extent of liver failure cases reported in the medical literature and clearly indicates a reason for concern:
				<ul>
				<li>From 1998 to 2003, acetaminophen was the leading cause of acute liver failure in the United States, with 48% of acetaminophen-related cases (131 of 275) associated with accidental overdose.</li>
				<li>A 2007 Centers for Disease Control and Prevention (CDC) population-based report estimates that, nationally, there are 1600 cases of acute liver failure (ALF) each year (all causes). Acetaminophen-related ALF was the most common etiology.</li>
				<li>Summarizing data from three different surveillance systems, there were an estimated 56,000 emergency room visits, 26,000 hospitalizations, and 458 deaths related to acetaminophen-associated overdoses per year during the 1990-1998 period.</li>
				<li>In a study that combined data from 22 specialty medical centers in the United States, acetaminophen-related liver injury was the leading cause of ALF for the years 1998 through 2003.1 This study also found that a high percentage of cases of liver injury due to acetaminophen were related to unintentional overdose, in which the patient mistakenly took too much acetaminophen. This finding was confirmed in a later study (2007).2 Many other cases of acute liver injury are caused by intentional overdoses of acetaminophen (i.e., associated with self-harm).</li>
				<li>Across various studies, consumers were found to have taken more than the recommended dose when using an OTC product, a prescription product, or both. The Toxic Exposure Surveillance System (TESS), now named the National Poison Data System (NPDS), which captures data from calls to 61 poison control centers, provides additional data on acetaminophen overdose and serious injury. In 2005, TESS showed that calls about poisoning cases that resulted in major injury numbered 1,187 for OTC single-ingredient products, 653 for OTC combination products, and 1,470 for prescription-opioid combination products.</li>
				</ul>
				<p>The risk of liver injury associated with the use of acetaminophen was discussed at the Joint Meeting of the FDA Drug Safety and Risk Management Advisory Committee, Nonprescription Drugs Advisory Committee, and Anesthetic and Life Support Drugs Advisory Committee, held on June 29-30, 2009 (for complete safety reviews and background information discussed at this meeting).</p>
				<p>The Advisory Committee recommended a range of additional regulatory actions such as adding a boxed warning to prescription acetaminophen products, withdrawing prescription combination products from the market, or reducing the amount of acetaminophen in each dosage unit. FDA considered the Committee''s advice for OTC products when deciding to limit the amount of acetaminophen per dosage unit in prescription products.</p>
				<p>By limiting the maximum amount of acetaminophen in prescription products to 325 mg per dosage unit, patients will be less likely to overdose on acetaminophen if they mistakenly take too many doses of acetaminophen-containing products.</p>
				<p>For more information on safety considerations for acetaminophen, visit the following link on the FDA web site: Acetaminophen Information</p>
				</li>
				</ul>
			</Content>
		</ListItem> 
	</WidgetList>
	', 3, Id, 1, 1, NULL
	FROM MetaPage WHERE [metapage_guid] = 'cde6d1b5-6a09-47b2-a304-abbcc2c94dac'

INSERT [dbo].[MetaWidget] ([metawidget_guid], [WidgetName], [WidgetDefinition], [Content], [WidgetType_Id], [MetaPage_Id], [WidgetLocation], [WidgetStatus], [Icon]) 
SELECT N'ada3fb30-d74f-44d9-8275-8752ab926e0b', N'PViMS Standards Used', N'', N'
	<WidgetList>
		<ListItem>
			<Title>Medical Dictionary For Regulatory Activities</Title>
			<Content>
				<ul>
				<li>
				<p>In the late 1990s, the International Conference on Harmonisation of Technical Requirements for Registration of Pharmaceuticals for Human Use (ICH) developed MedDRA, a rich and highly specific standardised medical terminology to facilitate sharing of regulatory information internationally for medical products used by humans. ICH?s powerful tool, MedDRA is available to all for use in the registration, documentation and safety monitoring of medical products both before and after a product has been authorised for sale. Products covered by the scope of MedDRA include pharmaceuticals, biologics, vaccines and drug-device combination products. Today, its growing use worldwide by regulatory authorities, pharmaceutical companies, clinical research organisations and health care professionals allows better global protection of patient health.</p>
				<p><a href="http://www.meddra.org/" target="_blank">Go to the MedDRA website...</a></p>
				</li>
				</ul>
			</Content>
		</ListItem> 
		<ListItem>
			<Title>International Classification of Diseases</Title>
			<Content>
				<ul>
				<li>
				<p>The International Classification of Diseases (ICD) is the standard diagnostic tool for epidemiology, health management and clinical purposes. This includes the analysis of the general health situation of population groups. It is used to monitor the incidence and prevalence of diseases and other health problems, proving a picture of the general health situation of countries and populations.</p>
				<p><a href="http://www.who.int/classifications/icd/en/" target="_blank">Go to the ICD10 website...</a></p>
				</li>
				</ul>
			</Content>
		</ListItem> 		
		<ListItem>
			<Title>Health Level Seven</Title>
			<Content>
				<ul>
				<li>
				<p>Founded in 1987, Health Level Seven International (HL7) is a not-for-profit, ANSI-accredited standards developing organization dedicated to providing a comprehensive framework and related standards for the exchange, integration, sharing, and retrieval of electronic health information that supports clinical practice and the management, delivery and evaluation of health services. HL7 is supported by more than 1,600 members from over 50 countries, including 500+ corporate members representing healthcare providers, government stakeholders, payers, pharmaceutical companies, vendors/suppliers, and consulting firms.</p>
				<p><a href="http://www.hl7.org/" target="_blank">Go to the HL7 website...</a></p>
				</li>
				</ul>
			</Content>
		</ListItem> 		
		<ListItem>
			<Title>Electronic Transmission of Individual Case Safety Reports</Title>
			<Content>
				<ul>
				<li>
				<p>Conceptually, an ICSR is a report of information describing adverse event(s) / reaction(s) experienced by an individual patient. The event(s)/reaction(s) can be related to the administration of one or more medicinal products at a particular point in time. The ICSR can also be used for exchange of other information, such as medication error(s) that do not involve adverse events(s)/reaction(s).</p>
				<p>This ICH IG focuses on medicinal products and therapeutic biologics for human use. However, the ICH is aware of other regional applications of the messaging standard that have a wider scope, such as pharmacovigilance activities related to vaccines, herbal products, cosmetics, veterinary products or medical devices. The primary ICH application is for the exchange of pharmacovigilance information between and among the pharmaceutical industry and regulatory authorities.</p>
				<p><a href="http://www.fda.gov/downloads/Drugs/GuidanceComplianceRegulatoryInformation/Guidances/UCM275638.pdf" target="_blank">View the E2B R3 Implementation Guide...</a></p>
				</li>
				</ul>
			</Content>
		</ListItem> 		
	</WidgetList>
	', 3, Id, 1, 1, NULL
	FROM MetaPage WHERE [metapage_guid] = '8d62faa0-6fcd-4f1f-8714-924afdc5b03d'

INSERT [dbo].[MetaWidget] ([metawidget_guid], [WidgetName], [WidgetDefinition], [Content], [WidgetType_Id], [MetaPage_Id], [WidgetLocation], [WidgetStatus], [Icon]) 
SELECT N'34115e68-44e5-4ace-8d6c-0d4679f911b0', N'Causality Scales Used', N'', N'
	<WidgetList>
		<ListItem>
			<Title>Naranjo Adverse Drug Reaction Propability Scale</Title>
			<Content>
				<ul>
				<li>
				<p>The Adverse Drug Reaction (ADR) Probability Scale was developed in 1991 by Naranjo and coworkers from the University of Toronto and is often referred to as the Naranjo Scale. This scale was developed to help standardize assessment of causality for all adverse drug reactions and was not designed specifically for drug induced liver injury. The scale was also designed for use in controlled trials and registration studies of new medications, rather than in routine clinical practice. Nevertheless, it is simple to apply and widely used. Many publications on drug induced liver injury mention results of applying the ADR Probability Scale.</p>
				<p><a href="http://www.pmidcalc.org/?sid=7249508&amp;newtest=Y" target="_blank">Go to an online Naranjo calculator...</a></p>
				</li>
				</ul>
			</Content>
		</ListItem> 
		<ListItem>
			<Title>WHO Adverse Drug Reaction Propability Scale</Title>
			<Content>
				<ul>
				<li>
				<p>The WHO-UMC system has been developed in consultation with the National Centres participating in the Programme for International Drug Monitoring and is meant as a practical tool for the assessment of case reports. It is basically a combined assessment taking into account the clinical-pharmacological aspects of the case history and the quality of the documentation of the observation. Since pharmacovigilance is particularly concerned with the detection of unknown and unexpected adverse reactions, other criteria such as previous knowledge and statistical chance play a less prominent role in the system. It is recognised that the semantics of the definitions are critical and that individual judgements may therefore differ. There are other algorithms that are either very complex or too specific for general use. This method gives guidance to the general arguments which should be used to select one category over another.</p>
				<p><a href="http://who-umc.org/Graphics/24734.pdf" target="_blank">The use of the WHO-UMC system for standardised case causality assessment ...</a></p>
				</li>
				</ul>
			</Content>
		</ListItem> 		
	</WidgetList>
	', 3, Id, 1, 1, NULL
	FROM MetaPage WHERE [metapage_guid] = '14589FFB-506A-4299-9E1A-FEF1C88AE881'

INSERT [dbo].[MetaWidget] ([metawidget_guid], [WidgetName], [WidgetDefinition], [Content], [WidgetType_Id], [MetaPage_Id], [WidgetLocation], [WidgetStatus], [Icon]) 
SELECT N'61a3be10-0ed0-462e-b7db-ead7561502e0', N'Grading Scales Used', N'', N'
	<WidgetList>
		<ListItem>
			<Title>Common Terminoloy Criteria for Adverse Events</Title>
			<Content>
				<ul>
				<li>
				<p>The NCI Common Terminology Criteria for Adverse Events is a descriptive terminology which can be utilized for Adverse Event (AE) reporting. A grading (severity) scale is provided for each AE term.</p>
				<p><a href="http://evs.nci.nih.gov/ftp1/CTCAE/CTCAE_4.03_2010-06-14_QuickReference_5x7.pdf" target="_blank">View list of CCTAE gradings...</a></p>
				</li>
				</ul>
			</Content>
		</ListItem> 
		<ListItem>
			<Title>Division of AIDS (DAIDS) Table for Grading the Severity of Adult and Paediatric Adverse Events</Title>
			<Content>
				<ul>
				<li>
				<p>The Division of AIDS (DAIDS) oversees clinical trials throughout the world which it sponsors and supports. The clinical trials evaluate the safety and efficacy of therapeutic products, vaccines, and other preventive modalities. Adverse event (AE) data collected during these clinical trials form the basis for subsequent safety and efficacy analyses of pharmaceutical products and medical devices. Incorrect and inconsistent AE severity grading can lead to inaccurate data analyses and interpretation, which in turn can impact the safety and well-being of clinical trial participants and future patients using pharmaceutical products.</p>
				<p>The DAIDS AE grading table is a shared tool for assessing the severity of AEs (including clinical and laboratory abnormalities) in participants enrolled in clinical trials. Over the years as scientific knowledge and experience have expanded, revisions to the DAIDS AE grading table have become necessary.</p>
				<p><a href="http://rsc.tech-res.com/Document/safetyandpharmacovigilance/DAIDS_AE_Grading_Table_v2_NOV2014.pdf" target="_blank">View list of DAIDS gradings...</a></p>
				</li>
				</ul>
			</Content>
		</ListItem> 
		<ListItem>
			<Title>ANRS Scale to grade the severity of adverse events in adults</Title>
			<Content>
				<ul>
				<li>
				<p>This severity scale is a working guide intended to harmonise evaluation and grading practices for symptomatology in ANRS biomedical research protocols.</p>
				<p>In practice, the items evaluated are grouped according to the system taking the form of a non-exhaustive symptomatic table (and not a classification of pathologies). Our choices focus on the most frequently observed clinical and biological signs or those whose monitoring is essential to ensure the protection of the subjects participating in the research</p>
				<p><a href="http://www.anrs.fr/content/download/2242/12805/file/ANRS-GradeEI-V1-En-2008.pdf" target="_blank">View list of ANRS gradings...</a></p>
				</li>
				</ul>
			</Content>
		</ListItem> 					
	</WidgetList>
	', 3, Id, 1, 1, NULL
	FROM MetaPage WHERE [metapage_guid] = '1E7FB80D-2CE2-4481-A9BA-D0538D425A5D'

INSERT [dbo].[MetaWidget] ([metawidget_guid], [WidgetName], [WidgetDefinition], [Content], [WidgetType_Id], [MetaPage_Id], [WidgetLocation], [WidgetStatus], [Icon]) 
SELECT N'936f01d6-f341-4802-9243-7c7ae9de66f2', N'Drug Safety', N'', N'
	<WidgetList>
		<ListItem>
			<Title>FDA Drug Safety Communication</Title>
			<SubTitle>Prescription Acetaminophen Products</SubTitle>
			<ContentPage>2</ContentPage>
		</ListItem> 
	</WidgetList>
	', 2, Id, 1, 1, NULL
	FROM MetaPage WHERE [metapage_guid] = '89CADD84-EB24-4C70-B769-9183A5A7405A'

INSERT [dbo].[MetaWidget] ([metawidget_guid], [WidgetName], [WidgetDefinition], [Content], [WidgetType_Id], [MetaPage_Id], [WidgetLocation], [WidgetStatus], [Icon]) 
SELECT N'f0805a0f-3977-4ae9-b27b-bcfd7618edec', N'Standards', N'', N'
	<WidgetList>
		<ListItem>
			<Title>MedDRA</Title>
			<SubTitle>Medical Dictionary for Regulatory Activities</SubTitle>
			<ContentPage>3</ContentPage>
		</ListItem> 
		<ListItem>
			<Title>ICD10</Title>
			<SubTitle>International Classification of Diseases</SubTitle>
			<ContentPage>3</ContentPage>
		</ListItem> 
		<ListItem>
			<Title>HL7</Title>
			<SubTitle>Health Level Seven</SubTitle>
			<ContentPage>3</ContentPage>
		</ListItem> 
		<ListItem>
			<Title>E2B</Title>
			<SubTitle>Electronic Transmission of Individual Case Safety Reports</SubTitle>
			<ContentPage>3</ContentPage>
		</ListItem> 
	</WidgetList>
	', 2, Id, 3, 1, NULL
	FROM MetaPage WHERE [metapage_guid] = '89CADD84-EB24-4C70-B769-9183A5A7405A'

INSERT [dbo].[MetaWidget] ([metawidget_guid], [WidgetName], [WidgetDefinition], [Content], [WidgetType_Id], [MetaPage_Id], [WidgetLocation], [WidgetStatus], [Icon]) 
SELECT N'f7315d56-43f6-49c9-88d4-3902eef95ccf', N'Causality Scales', N'', N'
	<WidgetList>
		<ListItem>
			<Title>Naranjo</Title>
			<SubTitle>Naranjo Adverse Drug Reaction Probability Scale</SubTitle>
			<ContentPage>4</ContentPage>
		</ListItem> 
		<ListItem>
			<Title>WHO</Title>
			<SubTitle>WHO Adverse Drug Reaction Probability Scale</SubTitle>
			<ContentPage>4</ContentPage>
		</ListItem> 
	</WidgetList>
	', 2, Id, 5, 1, NULL
	FROM MetaPage WHERE [metapage_guid] = '89CADD84-EB24-4C70-B769-9183A5A7405A'

INSERT [dbo].[MetaWidget] ([metawidget_guid], [WidgetName], [WidgetDefinition], [Content], [WidgetType_Id], [MetaPage_Id], [WidgetLocation], [WidgetStatus], [Icon]) 
SELECT N'75e26026-4ad1-41bc-b96e-aadc3ab9c005', N'Grading Scales', N'', N'
	<WidgetList>
		<ListItem>
			<Title>CCTAE</Title>
			<SubTitle>Common Terminology Criteria for Adverse Events</SubTitle>
			<ContentPage>5</ContentPage>
		</ListItem> 
		<ListItem>
			<Title>DAIDS</Title>
			<SubTitle>Division of AIDS (DAIDS) Table for Grading the Severity of Adult and Pediatric Adverse Events</SubTitle>
			<ContentPage>5</ContentPage>
		</ListItem> 
		<ListItem>
			<Title>ANRS</Title>
			<SubTitle>ANRS scale to grade the severity of adverse events in adults</SubTitle>
			<ContentPage>5</ContentPage>
		</ListItem> 
	</WidgetList>
	', 2, Id, 2, 1, NULL
	FROM MetaPage WHERE [metapage_guid] = '89CADD84-EB24-4C70-B769-9183A5A7405A'

INSERT [dbo].[MetaWidget] ([metawidget_guid], [WidgetName], [WidgetDefinition], [Content], [WidgetType_Id], [MetaPage_Id], [WidgetLocation], [WidgetStatus], [Icon]) 
SELECT N'1a50686f-ebbd-40d7-a9ba-99741660d2c4', N'Frequently Asked Questions on Bedaquiline', N'', N'
	<WidgetList>
		<ListItem>
			<Title>Why is the introduction of Bedaquiline significant?</Title>
			<Content>
				<p>The last time a drug was introduced specifically for the treatment of TB was in the late 1960s. That drug was rifampicin. Since then, resistance to rifampicin has been increasingly reported in the world. This is a major concern given that it remains among the most effective anti-TB drugs available today.<br /><br />Bedaquiline has been released specifically to treat TB patients with bacteria that are resistant to rifampicin as well as to isoniazid, another core anti-TB drug, and thus suffer from multidrug-resistant tuberculosis (MDR-TB).<br /><br />While bedaquiline has shown beneficial effect in studies including two Phase IIb trials, Phase III trials have not been completed. However, given the serious threat posed by MDR-TB both to the individual patient and to the community, some regulatory authorities have used an accelerated procedure for the approval of bedaquiline in order to ensure that eligible patients may benefit from this new drug when used under defined conditions. In order to guide countries on the use of bedaquiline in the treatment of MDR-TB, WHO is issuing interim guidance which will be reviewed in 2015 or before based on the results of further research(1).</p>
			</Content>
		</ListItem> 
		<ListItem>
			<Title>What is Bedaquiline and how does the drug work?</Title>
			<Content>
				<p>Bedaquiline is a bactericidal drug which belongs to a new class of antibiotics (diarylquinolines). Although the drug is active against many different bacteria, it has been registered specifically for the treatment of MDR-TB(2). The drug is unique among the anti-tuberculosis drugs currently used in that it interferes with the function of an enzyme required by the tuberculosis bacterium to produce energy and to replicate. The drug has been tested for the treatment of MDR-TB in addition to conventional treatment regimens including several drugs.</p>
			</Content>
		</ListItem> 
		<ListItem>
			<Title>Should all TB patients now be treated with Bedaquiline?</Title>
			<Content>
				<p>No. Up to now, bedaquiline has only been approved for use in patients who have MDR-TB and when options to treat this condition using existing drugs have been exhausted. The drug is to be given in addition to the multidrug treatment regimen recommended by WHO. Given the limited experience on its use, bedaquiline is recommended for use in adults affected with pulmonary (lung) MDR-TB. Special caution is needed when the drug is used in the elderly, in pregnant women, and in persons living with HIV who are taking antiretroviral medication (see cautions on the use of specific anti-retroviral drugs below). Bedaquiline should not be used to treat latent TB infection.</p>
			</Content>
		</ListItem> 
		<ListItem>
			<Title>What are the conditions under which Bedaquiline should be introduced?</Title>
			<Content>
				<p>Programmes introducing bedaquiline in their national MDR-TB regimens should take measures to ensure its proper use. It is important that all efforts are made to avoid bacterial resistance to this new drug as a result of misuse. In the interim guidance, WHO recommends that bedaquiline is used in line with existing guidance on the combination treatment of MDR-TB, that the response to treatment is monitored closely, that patients are well informed of both the expected benefits and possible harms of the drug, and that particular attention is given to detect and report any adverse events that develop. As with all other TB drugs, bedaquiline should not be used alone but as part of a combination therapy, and never added alone to a failing regimen.</p>
			</Content>
		</ListItem> 
		<ListItem>
			<Title>What are the side-effects of Bedaquiline? Are there any special measures that need to be taken when Bedaquiline is introduced?</Title>
			<Content>
				<p>Bedaquiline has been reported to disturb the function of the heart and liver in particular. Interactions with other drugs, especially lopinavir and efavirenz (used in the treatment of HIV), ketoconazole, as well as other drugs used in the treatment of MDR-TB (eg moxifloxacin, clofazimine) may be expected. More deaths were reported among patients taking bedaquiline during the studies carried out to investigate the drug, although it is not clear whether this was due to the drug. For all these reasons, it is important that patients are closely monitored and that adverse events are systematically reported (?active pharmacovigilance?), particularly those that are serious and life-threatening. Clinical monitoring of symptoms, performance of special tests at appropriate intervals, and engagement of the patient to report untoward consequences of treatment to the appropriate pharmacovigilance institution are the cornerstones for the effective management of side effects in a timely fashion.</p>
			</Content>
		</ListItem> 
		<ListItem>
			<Title>What support will WHO provide to countries who wish to introduce Bedaquiline?</Title>
			<Content>
				<p>In addition to the WHO interim policy, a guide on clinical management of patients will be released in the coming months. WHO will work on other programmatic planning and implementation tools. WHO will monitor closely the new developments concerning this drug and modify the interim policy accordingly. The Organization is also available to advise programmes on practical implementation issues and on how to monitor the use of the drug for effectiveness and harms. WHO is collaborating with other partners to enable support for rational introduction.</p>
			</Content>
		</ListItem> 
		<ListItem>
			<Title>How long is the treatment with Bedaquiline? What is the dose?</Title>
			<Content>
				<p>Bedaquiline should be given for a maximum of six months on top of the WHO recommended combination treatment regimen. The manufacturer recommends 400 mg daily (4 tablets) for 2 weeks followed by 200 mg 3 times per week for the remaining 22 weeks. For more information on dosage and conditions for use please see the references at the end (2).</p>
			</Content>
		</ListItem> 
		<ListItem>
			<Title>Can Bedaquiline be used to shorten treatment of MDR-TB?</Title>
			<Content>
				<p>No, there is no evidence as yet that this drug can reduce treatment duration. Moreover there is no experience of the use of this drug in short MDR-TB treatment regimens. While bedaquiline is expected to improve the likelihood of a successful outcome for individual patients, its overall impact on public health and transmission of MDR-TB in countries cannot as yet be established.</p>
			</Content>
		</ListItem> 
		<ListItem>
			<Title>Will there be other new drugs to treat TB?</Title>
			<Content>
				<p>The development of new drugs requires substantial investment and research usually lasting many years. Other new drugs are presently being investigated and it is expected that some may be released within the next few years. (3)</p>
			</Content>
		</ListItem> 
	</WidgetList>
	', 3, Id, 1, 1, NULL
	FROM MetaPage WHERE [metapage_guid] = '942F501A-1F47-49A3-9F60-6814EA46C482'

-- ***************** METAREPORTS
INSERT INTO MetaReport (metareport_guid, ReportName, ReportDefinition, MetaDefinition, Breadcrumb, SQLDefinition, IsSystem) 
	SELECT NEWID(), 'Patients on Study', 'Patients on Study', '** NOT DEFINED **', '** NOT DEFINED **', 'SELECT f.FacilityName, f.Id as FacilityId,	(select count(distinct(ip.Id)) from Encounter ie inner join Patient ip on ie.Patient_Id = ip.Id inner join PatientFacility ipf on ip.Id = ipf.Patient_Id AND ipf.EnrolledDate = (select MAX(EnrolledDate) FROM PatientFacility iipf WHERE iipf.Patient_Id = ip.Id) inner join Facility ifa on ipf.Facility_Id = ifa.Id where ie.EncounterDate between ''{0}'' and ''{1}'' and ifa.Id = f.Id) AS PatientCount,	(select count(distinct(ip.Id)) from Encounter ie inner join Patient ip on ie.Patient_Id = ip.Id inner join PatientFacility ipf on ip.Id = ipf.Patient_Id AND ipf.EnrolledDate = (select MAX(EnrolledDate) FROM PatientFacility iipf WHERE iipf.Patient_Id = ip.Id) inner join Facility ifa on ipf.Facility_Id = ifa.Id inner join PatientClinicalEvent ipce on ip.Id = ipce.Patient_Id where ie.EncounterDate between ''{0}'' and ''{1}'' and ifa.Id = f.Id) AS PatientWithEventCount FROM Facility f ORDER BY f.FacilityName ', 1
	UNION SELECT NEWID(), 'Adverse Events', 'Adverse Events', '** NOT DEFINED **', '** NOT DEFINED **', 'SELECT t.MedDraTerm + ''('' + [MedDraCode] + '')'' AS ''Description'', CASE WHEN DATEDIFF(dd, p.DateOfBirth, GETDATE()) < 5844 THEN ''<16'' WHEN DATEDIFF(dd, p.DateOfBirth, GETDATE()) BETWEEN 5844 AND 9131 THEN ''Between 16 and 25'' WHEN DATEDIFF(dd, p.DateOfBirth, GETDATE()) BETWEEN 9132 AND 12784 THEN ''Between 26 and 35'' WHEN DATEDIFF(dd, p.DateOfBirth, GETDATE()) BETWEEN 12785 AND 16436 THEN ''Between 36 and 45'' WHEN DATEDIFF(dd, p.DateOfBirth, GETDATE()) BETWEEN 16437 AND 20089 THEN ''Between 46 and 55'' WHEN DATEDIFF(dd, p.DateOfBirth, GETDATE()) > 20089 THEN ''>55'' END AS ''Criteria'', COUNT(*) AS PatientCount FROM PatientClinicalEvent pce INNER JOIN Patient p ON pce.Patient_Id = p.Id INNER JOIN TerminologyMedDra t ON pce.SourceTerminologyMedDra_Id = t.Id WHERE pce.OnsetDate BETWEEN ''{0}'' AND ''{1}'' GROUP BY t.MedDraTerm + ''('' + [MedDraCode] + '')'', CASE WHEN DATEDIFF(dd, p.DateOfBirth, GETDATE()) < 5844 THEN ''<16'' WHEN DATEDIFF(dd, p.DateOfBirth, GETDATE()) BETWEEN 5844 AND 9131 THEN ''Between 16 and 25'' WHEN DATEDIFF(dd, p.DateOfBirth, GETDATE()) BETWEEN 9132 AND 12784 THEN ''Between 26 and 35'' WHEN DATEDIFF(dd, p.DateOfBirth, GETDATE()) BETWEEN 12785 AND 16436 THEN ''Between 36 and 45'' WHEN DATEDIFF(dd, p.DateOfBirth, GETDATE()) BETWEEN 16437 AND 20089 THEN ''Between 46 and 55'' WHEN DATEDIFF(dd, p.DateOfBirth, GETDATE()) > 20089 THEN ''>55'' END ORDER BY t.MedDraTerm + ''('' + [MedDraCode] + '')'' asc, Criteria asc', 1
	UNION SELECT NEWID(), 'Causality Not Set', 'Causality Not Set', '** NOT DEFINED **', '** NOT DEFINED **', 'SELECT p.Id AS Patient_Id, p.FirstName, p.Surname, t.MedDraTerm + ''('' + [MedDraCode] + '')'' AS AdverseEvent, pce.OnsetDate, mc.NaranjoCausality, mc.WhoCausality FROM Patient p INNER JOIN PatientClinicalEvent pce ON p.Id = pce.Patient_Id INNER JOIN TerminologyMedDra t ON pce.SourceTerminologyMedDra_Id = t.Id INNER JOIN MedicationCausality mc ON pce.Id = mc.ClinicalEvent_Id WHERE pce.OnsetDate BETWEEN ''{0}'' AND ''{1}'' AND (mc.NaranjoCausality IS NULL OR mc.WhoCausality IS NULL) ORDER BY pce.OnsetDate asc ', 1 
	UNION SELECT NEWID(), 'Patients by Drug', 'Patients by Drug', '** NOT DEFINED **', '** NOT DEFINED **', 'SELECT m.DrugName, m.Id as MedicationId,	(select count(distinct(ip.Id)) from Patient ip inner join PatientMedication ipm on ip.Id = ipm.Patient_Id inner join Medication im on ipm.Medication_Id = im.Id where im.Id = m.Id) AS PatientCount FROM Medication m ORDER BY m.DrugName ', 1
	UNION SELECT NEWID(), 'Outstanding Visits', 'Outstanding Visits', '** NOT DEFINED **', '** NOT DEFINED **', 'SELECT p.Id AS Patient_Id, p.FirstName, p.Surname, a.AppointmentDate FROM Patient p INNER JOIN Appointment a ON p.Id = a.Patient_Id WHERE a.AppointmentDate < DATEADD(dd, 3, GETDATE()) AND a.AppointmentDate BETWEEN ''{0}'' AND ''{1}'' AND a.Cancelled = 0 AND NOT EXISTS(SELECT Id FROM Encounter ie WHERE ie.Patient_Id = p.Id AND ie.EncounterDate BETWEEN DATEADD(dd, -3, a.AppointmentDate) AND DATEADD(dd, 3, a.AppointmentDate)) ORDER BY a.AppointmentDate desc', 1                

<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="WikiContent.aspx.cs" MasterPageFile="/Main.Master" Inherits="PVIMS.Web.WikiContent" Title="Wiki" %>

<asp:Content runat="server" ID="Body" ContentPlaceHolderID="BodyContentPlaceHolder">

	<div class="row">
		<div class="col-xs-12 col-sm-7 col-md-7 col-lg-4">
			<h1 class="page-title txt-color-blueDark">
				<i class="fa fa-home fa-fw "></i> 
				Wiki
			</h1>
		</div>
	</div>

    <section id="widget-grid" class="">

	    <div class="row">

            <div>
		        <!-- NEW WIDGET START -->
		        <article class="col-xs-12 col-sm-12 col-md-12 col-lg-10" id="divArticle1" runat="server">
				
			        <!-- Widget ID (each widget will need unique ID)-->
			        <div class="jarviswidget" id="wid-id-1"  data-widget-editbutton="false" data-widget-custombutton="false" data-widget-deletebutton="false" data-widget-colorbutton="false">
				        <header>
					        <span class="widget-icon"> <i class="fa fa-table"></i> </span>
					        <h2>FDA Drug Safety Communication</h2>
				        </header>
				
				        <!-- widget div-->
				        <div>
				
					        <!-- widget edit box -->
					        <div class="jarviswidget-editbox">
						        <!-- This area used as dropdown edit box -->
				
					        </div>
					        <!-- end widget edit box -->
				
					        <!-- widget content -->
					        <div class="widget-body padding">
				
						        <div class="panel-group smart-accordion-default" id="accordion-2">
							        <div class="panel panel-default">
								        <div class="panel-heading">
									        <h4 class="panel-title"><a data-toggle="collapse" data-parent="#accordion-2" href="#collapseOne-1"> <i class="fa fa-fw fa-plus-circle txt-color-green"></i> <i class="fa fa-fw fa-minus-circle txt-color-red"></i> Safety Announcement </a></h4>
								        </div>
								        <div id="collapseOne-1" class="panel-collapse collapse in">
									        <div class="panel-body">
                                                The U.S. Food and Drug Administration (FDA) is asking drug manufacturers to limit the strength of acetaminophen in prescription drug products, which are predominantly combinations of acetaminophen and opioids. This action will limit the amount of acetaminophen in these products to 325 mg per tablet, capsule, or other dosage unit, making these products safer for patients. 
                                                    <p>In addition, a Boxed Warning highlighting the potential for severe liver injury and a Warning highlighting the potential for allergic reactions (e.g., swelling of the face, mouth, and throat, difficulty breathing, itching, or rash) are being added to the label of all prescription drug products that contain acetaminophen. </p>
                                                    <p>These actions will help to reduce the risk of severe liver injury and allergic reactions associated with acetaminophen. </p>
                                                    <p>Acetaminophen is widely and effectively used in both prescription and over-the-counter (OTC) products to reduce pain and fever. It is one of the most commonly-used drugs in the United States. Examples of prescription products that contain acetaminophen include hydrocodone with acetaminophen (Vicodin, Lortab), and oxycodone with acetaminophen (Tylox, Percocet). </p>
                                                    <p>OTC products containing acetaminophen (e.g., Tylenol) are not affected by this action. Information about the potential for liver injury is already required on the label for OTC products containing acetaminophen. FDA is continuing to evaluate ways to reduce the risk of acetaminophen related liver injury from OTC products. Additional safety measures relating to OTC acetaminophen products will be taken through separate action, such as a rulemaking as part of the ongoing OTC monograph proceeding for internal analgesic drug products.</p>
									        </div>
								        </div>
							        </div>
							        <div class="panel panel-default">
								        <div class="panel-heading">
									        <h4 class="panel-title"><a data-toggle="collapse" data-parent="#accordion-2" href="#collapseTwo-1" class="collapsed"> <i class="fa fa-fw fa-plus-circle txt-color-green"></i> <i class="fa fa-fw fa-minus-circle txt-color-red"></i> Additional Information for Patients </a></h4>
								        </div>
								        <div id="collapseTwo-1" class="panel-collapse collapse">
									        <div class="panel-body">
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
                                                    <li>Report side effects to FDA's MedWatch program using the information in the "Contact Us" box at the bottom of the page.</li>                                       
                                                </ul>
									        </div>
								        </div>
							        </div>
							        <div class="panel panel-default">
								        <div class="panel-heading">
									        <h4 class="panel-title"><a data-toggle="collapse" data-parent="#accordion-2" href="#collapseThree-1" class="collapsed"> <i class="fa fa-fw fa-plus-circle txt-color-green"></i> <i class="fa fa-fw fa-minus-circle txt-color-red"></i> Additional Information for HealthCare Professionals? </a></h4>
								        </div>
								        <div id="collapseThree-1" class="panel-collapse collapse">
									        <div class="panel-body">
                                                The maximum amount of acetaminophen in a prescription tablet, capsule, or other dosage unit will be limited to 325 mg. However, the total number of tablets or capsules that may be prescribed and the time intervals at which they may be prescribed will not change as a result of the lower amount of acetaminophen. For example, for a product that previously contained 500 mg of acetaminophen with an opioid and was prescribed as 1-2 tablets every 4-6 hours, once reformulated to contain 325 mg of acetaminophen, the dosing instructions can remain unchanged. 
                                                <ul>
                                                    <li>Advise patients not to exceed the acetaminophen maximum total daily dose (4 grams/day).</li>
                                                    <li>Severe liver injury, including cases of acute liver failure resulting in liver transplant and death, has been reported with the use of acetaminophen.</li>
                                                    <li>Educate patients about the importance of reading all prescription and OTC labels to ensure they are not taking multiple acetaminophen-containing products.</li>
                                                    <li>Advise patients not to drink alcohol while taking acetaminophen-containing medications.</li>
                                                    <li>Rare cases of anaphylaxis and other hypersensitivity reactions have occurred with the use of acetaminophen.</li>
                                                    <li>Advise patients to seek medical help immediately if they have taken more acetaminophen than directed or experience swelling of the face, mouth, and throat, difficulty breathing, itching, and rash.</li>
                                                    <li>Report adverse events to FDA's MedWatch program using the information in the "Contact Us" box at the bottom of the page.</li>
                                                </ul>
									        </div>
								        </div>
							        </div>
							        <div class="panel panel-default">
								        <div class="panel-heading">
									        <h4 class="panel-title"><a data-toggle="collapse" data-parent="#accordion-2" href="#collapseFour-1" class="collapsed"> <i class="fa fa-fw fa-plus-circle txt-color-green"></i> <i class="fa fa-fw fa-minus-circle txt-color-red"></i> Data Summary and Discussion </a></h4>
								        </div>
								        <div id="collapseFour-1" class="panel-collapse collapse">
									        <div class="panel-body">
                                                A number of studies have tried to answer the question of how common liver injury is in relation to the use of acetaminophen. Although many questions remain about the full scope of the problem, the following examples indicate what is known about the extent of liver failure cases reported in the medical literature and clearly indicates a reason for concern: 
                                                <ul>
                                                    <li>From 1998 to 2003, acetaminophen was the leading cause of acute liver failure in the United States, with 48% of acetaminophen-related cases (131 of 275) associated with accidental overdose.</li>
                                                    <li>A 2007 Centers for Disease Control and Prevention (CDC) population-based report estimates that, nationally, there are 1600 cases of acute liver failure (ALF) each year (all causes). Acetaminophen-related ALF was the most common etiology.</li>
                                                    <li>Summarizing data from three different surveillance systems, there were an estimated 56,000 emergency room visits, 26,000 hospitalizations, and 458 deaths related to acetaminophen-associated overdoses per year during the 1990-1998 period.</li>
                                                    <li>In a study that combined data from 22 specialty medical centers in the United States, acetaminophen-related liver injury was the leading cause of ALF for the years 1998 through 2003.1 This study also found that a high percentage of cases of liver injury due to acetaminophen were related to unintentional overdose, in which the patient mistakenly took too much acetaminophen. This finding was confirmed in a later study (2007).2 Many other cases of acute liver injury are caused by intentional overdoses of acetaminophen (i.e., associated with self-harm).</li>
                                                    <li>Across various studies, consumers were found to have taken more than the recommended dose when using an OTC product, a prescription product, or both. The Toxic Exposure Surveillance System (TESS), now named the National Poison Data System (NPDS), which captures data from calls to 61 poison control centers, provides additional data on acetaminophen overdose and serious injury. In 2005, TESS showed that calls about poisoning cases that resulted in major injury numbered 1,187 for OTC single-ingredient products, 653 for OTC combination products, and 1,470 for prescription-opioid combination products.</li>
                                                </ul>
                                                <p>The risk of liver injury associated with the use of acetaminophen was discussed at the Joint Meeting of the FDA Drug Safety and Risk Management Advisory Committee, Nonprescription Drugs Advisory Committee, and Anesthetic and Life Support Drugs Advisory Committee, held on June 29-30, 2009 (for complete safety reviews and background information discussed at this meeting). </p>
                                                <p>The Advisory Committee recommended a range of additional regulatory actions such as adding a boxed warning to prescription acetaminophen products, withdrawing prescription combination products from the market, or reducing the amount of acetaminophen in each dosage unit. FDA considered the Committee's advice for OTC products when deciding to limit the amount of acetaminophen per dosage unit in prescription products. </p>
                                                <p>By limiting the maximum amount of acetaminophen in prescription products to 325 mg per dosage unit, patients will be less likely to overdose on acetaminophen if they mistakenly take too many doses of acetaminophen-containing products. </p>
                                                <p>For more information on safety considerations for acetaminophen, visit the following link on the FDA web site: Acetaminophen Information</p>
									        </div>
								        </div>
							        </div>
						        </div>

					        </div>
					        <!-- end widget content -->
				
				        </div>
				        <!-- end widget div -->
				
			        </div>
			        <!-- end widget -->
				
		        </article>
		        <!-- WIDGET END -->
            </div>

            <div >
		        <!-- NEW WIDGET START -->
		        <article class="col-xs-12 col-sm-12 col-md-12 col-lg-10" id="divArticle2" runat="server">
				
			        <!-- Widget ID (each widget will need unique ID)-->
			        <div class="jarviswidget" id="wid-id-2"  data-widget-editbutton="false" data-widget-custombutton="false" data-widget-deletebutton="false" data-widget-colorbutton="false">
				        <header>
					        <span class="widget-icon"> <i class="fa fa-table"></i> </span>
					        <h2>Standards</h2>
				        </header>
				
				        <!-- widget div-->
				        <div>
				
					        <!-- widget edit box -->
					        <div class="jarviswidget-editbox">
						        <!-- This area used as dropdown edit box -->
				
					        </div>
					        <!-- end widget edit box -->
				
					        <!-- widget content -->
					        <div class="widget-body padding">
				
						        <div class="panel-group smart-accordion-default" id="accordion-3">
							        <div class="panel panel-default">
								        <div class="panel-heading">
									        <h4 class="panel-title"><a data-toggle="collapse" data-parent="#accordion-3" href="#collapse3One-1"> <i class="fa fa-fw fa-plus-circle txt-color-green"></i> <i class="fa fa-fw fa-minus-circle txt-color-red"></i> Medical Dictionary for Regulatory Activities </a></h4>
								        </div>
								        <div id="collapse3One-1" class="panel-collapse collapse in">
									        <div class="panel-body">
                                                <p>In the late 1990s, the International Conference on Harmonisation of Technical Requirements for Registration of Pharmaceuticals for Human Use (ICH) developed MedDRA, a rich and highly specific standardised medical terminology to facilitate sharing of regulatory information internationally for medical products used by humans. ICH’s powerful tool, MedDRA is available to all for use in the registration, documentation and safety monitoring of medical products both before and after a product has been authorised for sale. Products covered by the scope of MedDRA include pharmaceuticals, biologics, vaccines and drug-device combination products. Today, its growing use worldwide by regulatory authorities, pharmaceutical companies, clinical research organisations and health care professionals allows better global protection of patient health.</p>
                                                <p><a href="http://www.meddra.org/" target="_blank">Go to the MedDRA website...</a></p>
									        </div>
								        </div>
							        </div>
							        <div class="panel panel-default">
								        <div class="panel-heading">
									        <h4 class="panel-title"><a data-toggle="collapse" data-parent="#accordion-3" href="#collapse3Two-1" class="collapsed"> <i class="fa fa-fw fa-plus-circle txt-color-green"></i> <i class="fa fa-fw fa-minus-circle txt-color-red"></i> International Classification of Diseases </a></h4>
								        </div>
								        <div id="collapse3Two-1" class="panel-collapse collapse">
									        <div class="panel-body">
                                                <p>The International Classification of Diseases (ICD) is the standard diagnostic tool for epidemiology, health management and clinical purposes. This includes the analysis of the general health situation of population groups. It is used to monitor the incidence and prevalence of diseases and other health problems, proving a picture of the general health situation of countries and populations.</p>
                                                <p><a href="http://www.who.int/classifications/icd/en/" target="_blank">Go to the ICD10 website...</a></p>
									        </div>
								        </div>
							        </div>
							        <div class="panel panel-default">
								        <div class="panel-heading">
									        <h4 class="panel-title"><a data-toggle="collapse" data-parent="#accordion-3" href="#collapse3Three-1" class="collapsed"> <i class="fa fa-fw fa-plus-circle txt-color-green"></i> <i class="fa fa-fw fa-minus-circle txt-color-red"></i> Health Level Seven </a></h4>
								        </div>
								        <div id="collapse3Three-1" class="panel-collapse collapse">
									        <div class="panel-body">
                                                <p>Founded in 1987, Health Level Seven International (HL7) is a not-for-profit, ANSI-accredited standards developing organization dedicated to providing a comprehensive framework and related standards for the exchange, integration, sharing, and retrieval of electronic health information that supports clinical practice and the management, delivery and evaluation of health services. HL7 is supported by more than 1,600 members from over 50 countries, including 500+ corporate members representing healthcare providers, government stakeholders, payers, pharmaceutical companies, vendors/suppliers, and consulting firms.</p>
                                                <p><a href="http://www.hl7.org/" target="_blank">Go to the HL7 website...</a></p>
									        </div>
								        </div>
							        </div>
							        <div class="panel panel-default">
								        <div class="panel-heading">
									        <h4 class="panel-title"><a data-toggle="collapse" data-parent="#accordion-3" href="#collapse3Four-1" class="collapsed"> <i class="fa fa-fw fa-plus-circle txt-color-green"></i> <i class="fa fa-fw fa-minus-circle txt-color-red"></i> Electronic Transmission of Individual Case Safety Reports </a></h4>
								        </div>
								        <div id="collapse3Four-1" class="panel-collapse collapse">
									        <div class="panel-body">
                                                <p>
                                                    Conceptually, an ICSR is a report of information describing adverse event(s) / reaction(s) experienced by
                                                    an individual patient. The event(s)/reaction(s) can be related to the administration of one or more
                                                    medicinal products at a particular point in time. The ICSR can also be used for exchange of other
                                                    information, such as medication error(s) that do not involve adverse events(s)/reaction(s).
                                                </p>
                                                <p>
                                                    This ICH IG focuses on medicinal products and therapeutic biologics for human use. However, the ICH is
                                                    aware of other regional applications of the messaging standard that have a wider scope, such as
                                                    pharmacovigilance activities related to vaccines, herbal products, cosmetics, veterinary products or
                                                    medical devices. The primary ICH application is for the exchange of pharmacovigilance information
                                                    between and among the pharmaceutical industry and regulatory authorities. 
                                                </p>
                                                <p><a href="http://www.fda.gov/downloads/Drugs/GuidanceComplianceRegulatoryInformation/Guidances/UCM275638.pdf" target="_blank">View the E2B R3 Implementation Guide...</a></p>
									        </div>
								        </div>
							        </div>
						        </div>

					        </div>
					        <!-- end widget content -->
				
				        </div>
				        <!-- end widget div -->
				
			        </div>
			        <!-- end widget -->
				
		        </article>
		        <!-- WIDGET END -->
            </div>

            <div>
		        <!-- NEW WIDGET START -->
		        <article class="col-xs-12 col-sm-12 col-md-12 col-lg-10" id="divArticle3" runat="server">
				
			        <!-- Widget ID (each widget will need unique ID)-->
			        <div class="jarviswidget" id="wid-id-3"  data-widget-editbutton="false" data-widget-custombutton="false" data-widget-deletebutton="false" data-widget-colorbutton="false">
				        <header>
					        <span class="widget-icon"> <i class="fa fa-table"></i> </span>
					        <h2>Grading Scales</h2>
				        </header>
				
				        <!-- widget div-->
				        <div>
				
					        <!-- widget edit box -->
					        <div class="jarviswidget-editbox">
						        <!-- This area used as dropdown edit box -->
				
					        </div>
					        <!-- end widget edit box -->
				
					        <!-- widget content -->
					        <div class="widget-body padding">
				
						        <div class="panel-group smart-accordion-default" id="accordion-4">
							        <div class="panel panel-default">
								        <div class="panel-heading">
									        <h4 class="panel-title"><a data-toggle="collapse" data-parent="#accordion-4" href="#collapse4One-1"> <i class="fa fa-fw fa-plus-circle txt-color-green"></i> <i class="fa fa-fw fa-minus-circle txt-color-red"></i> Naranjo Adverse Drug Reaction Probability Scale </a></h4>
								        </div>
								        <div id="collapse4One-1" class="panel-collapse collapse in">
									        <div class="panel-body">
                                                <p>The Adverse Drug Reaction (ADR) Probability Scale was developed in 1991 by Naranjo and coworkers from the University of Toronto and is often referred to as the Naranjo Scale.  This scale was developed to help standardize assessment of causality for all adverse drug reactions and was not designed specifically for drug induced liver injury.  The scale was also designed for use in controlled trials and registration studies of new medications, rather than in routine clinical practice.  Nevertheless, it is simple to apply and widely used.  Many publications on drug induced liver injury mention results of applying the ADR Probability Scale.</p>
                                                <p><a href="http://www.pmidcalc.org/?sid=7249508&newtest=Y" target="_blank">Go to an online Naranjo calculator...</a></p>
									        </div>
								        </div>
							        </div>
							        <div class="panel panel-default">
								        <div class="panel-heading">
									        <h4 class="panel-title"><a data-toggle="collapse" data-parent="#accordion-4" href="#collapse4Two-1" class="collapsed"> <i class="fa fa-fw fa-plus-circle txt-color-green"></i> <i class="fa fa-fw fa-minus-circle txt-color-red"></i> WHO Adverse Drug Reaction Probability Scale </a></h4>
								        </div>
								        <div id="collapse4Two-1" class="panel-collapse collapse">
									        <div class="panel-body">
                                                <p>The WHO-UMC system has been developed in consultation with the National Centres participating in the Programme for International Drug Monitoring and is meant as a practical tool for the assessment of case reports. It is basically a combined assessment taking into account the clinical-pharmacological aspects of the case history and the quality of the documentation of the observation. Since pharmacovigilance is particularly concerned with the detection of unknown and unexpected adverse reactions, other criteria such as previous knowledge and statistical chance play a less prominent role in the system. It is recognised that the semantics of the definitions are critical and that individual judgements may therefore differ. There are other algorithms that are either very complex or too specific for general use. This method gives guidance to the general arguments which should be used to select one category over another. </p>
                                                <p><a href="http://who-umc.org/Graphics/24734.pdf" target="_blank">The use of the WHO-UMC system for standardised case causality assessment ...</a></p>
									        </div>
								        </div>
							        </div>
						        </div>

					        </div>
					        <!-- end widget content -->
				
				        </div>
				        <!-- end widget div -->
				
			        </div>
			        <!-- end widget -->
				
		        </article>
		        <!-- WIDGET END -->
            </div>

            <div>
		        <!-- NEW WIDGET START -->
		        <article class="col-xs-12 col-sm-12 col-md-12 col-lg-10" id="divArticle4" runat="server">
				
			        <!-- Widget ID (each widget will need unique ID)-->
			        <div class="jarviswidget" id="wid-id-4"  data-widget-editbutton="false" data-widget-custombutton="false" data-widget-deletebutton="false" data-widget-colorbutton="false">
				        <header>
					        <span class="widget-icon"> <i class="fa fa-table"></i> </span>
					        <h2>Grading Scales</h2>
				        </header>
				
				        <!-- widget div-->
				        <div>
				
					        <!-- widget edit box -->
					        <div class="jarviswidget-editbox">
						        <!-- This area used as dropdown edit box -->
				
					        </div>
					        <!-- end widget edit box -->
				
					        <!-- widget content -->
					        <div class="widget-body padding">
				
						        <div class="panel-group smart-accordion-default" id="accordion-5">
							        <div class="panel panel-default">
								        <div class="panel-heading">
									        <h4 class="panel-title"><a data-toggle="collapse" data-parent="#accordion-5" href="#collapse5One-1"> <i class="fa fa-fw fa-plus-circle txt-color-green"></i> <i class="fa fa-fw fa-minus-circle txt-color-red"></i> Common Terminology Criteria for Adverse Events </a></h4>
								        </div>
								        <div id="collapse5One-1" class="panel-collapse collapse in">
									        <div class="panel-body">
                                                <p>The NCI Common Terminology Criteria for Adverse Events is a descriptive terminology which can be utilized for Adverse Event (AE) reporting. A grading (severity) scale is provided for each AE term. </p>
                                                <p><a href="http://evs.nci.nih.gov/ftp1/CTCAE/CTCAE_4.03_2010-06-14_QuickReference_5x7.pdf" target="_blank">View list of CCTAE gradings...</a></p>
									        </div>
								        </div>
							        </div>
							        <div class="panel panel-default">
								        <div class="panel-heading">
									        <h4 class="panel-title"><a data-toggle="collapse" data-parent="#accordion-5" href="#collapse5Two-1" class="collapsed"> <i class="fa fa-fw fa-plus-circle txt-color-green"></i> <i class="fa fa-fw fa-minus-circle txt-color-red"></i> Division of AIDS (DAIDS) Table for Grading the Severity of Adult and Pediatric Adverse Events </a></h4>
								        </div>
								        <div id="collapse5Two-1" class="panel-collapse collapse">
									        <div class="panel-body">
                                                <p>The Division of AIDS (DAIDS) oversees clinical trials throughout the world which it sponsors and supports. The clinical trials evaluate the safety and efficacy of therapeutic products, vaccines, and other preventive modalities. Adverse event (AE) data collected during these clinical trials form the basis for subsequent safety and efficacy analyses of pharmaceutical products and medical devices. Incorrect and inconsistent AE severity grading can lead to inaccurate data analyses and interpretation, which in turn can impact the safety and well-being of clinical trial participants and future patients using pharmaceutical products.</p>
                                                <p>The DAIDS AE grading table is a shared tool for assessing the severity of AEs (including clinical and laboratory abnormalities) in participants enrolled in clinical trials. Over the years as scientific knowledge and experience have expanded, revisions to the DAIDS AE grading table have become necessary.</p>
                                                <p><a href="http://rsc.tech-res.com/Document/safetyandpharmacovigilance/DAIDS_AE_Grading_Table_v2_NOV2014.pdf" target="_blank">View list of DAIDS gradings...</a></p>
									        </div>
								        </div>
							        </div>
							        <div class="panel panel-default">
								        <div class="panel-heading">
									        <h4 class="panel-title"><a data-toggle="collapse" data-parent="#accordion-5" href="#collapse5Three-1" class="collapsed"> <i class="fa fa-fw fa-plus-circle txt-color-green"></i> <i class="fa fa-fw fa-minus-circle txt-color-red"></i> ANRS scale to grade the severity of adverse events in adults </a></h4>
								        </div>
								        <div id="collapse5Three-1" class="panel-collapse collapse">
									        <div class="panel-body">
                                                <p>This severity scale is a working guide intended to harmonise evaluation and grading practices for symptomatology in ANRS biomedical research protocols.</p>
                                                <p>In practice, the items evaluated are grouped according to the system taking the form of a non-exhaustive symptomatic table (and not a classification of pathologies). Our choices focus on the most frequently observed clinical and biological signs or those whose monitoring is essential to ensure the protection of the subjects participating in the research</p>
                                                <p><a href="http://www.anrs.fr/content/download/2242/12805/file/ANRS-GradeEI-V1-En-2008.pdf" target="_blank">View list of ANRS gradings...</a></p>
									        </div>
								        </div>
							        </div>
						        </div>

					        </div>
					        <!-- end widget content -->
				
				        </div>
				        <!-- end widget div -->
				
			        </div>
			        <!-- end widget -->
				
		        </article>
		        <!-- WIDGET END -->
            </div>

	    </div>
    
    </section>
				
</asp:Content>

<asp:Content runat="server" ID="Scripts" ContentPlaceHolderID="scriptsPlaceholder">
    <script>

	</script>

</asp:Content>



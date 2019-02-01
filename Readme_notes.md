//0.2.0/20150831
SL-0002: Order conditions, labs and medications drop down lists 
SL-0004: Modified configuration to remove ART terminology in encounter types
SL-0011: Display facility name and number in patient search form
SL-0012: FIXED BUG: Server error when downloading attachment
SL-0033: Modified configuration to include new medication Bedaquiline 100mg (188 tabs), RHZE, RH, RHZ, amikacin, amoxicillin + clavulanate, bedaquiline, capreomycin, clarithromycin, clofazimine, cycloserine, ethambutol, ethionamide, high dose isoniazid, imipenem + cilastatin, kanamycin, levofloxacin, linezolid, meropenem + clavulanate, moxifloxacin, PAS, prothionamide, pyrazinamide, terizidone, thioacetazone
SL-0022: Modify medication custom entity to reflect still on medication
SL-0024: Include duplicate check on facility add.
SL-0025: Include successfull message on adding of new facility
SL-0026: FIXED BUG: Facilities not appearing on patient search screen (even if captured)
SL-0028: FIXED BUG: Unable to delete attachment. Relationship to entity error.
SL-0029: Condition drop down list changed to use drop down list from legacy DCAT
SL-0030: Modified condition configuration to include ongoing condition option
SL-0031: Modified medication configuration to reflect new field order: move the "dose unit" to right under the "dose" field and above the "dose frequency" field
SL-0035/0036: Occupation updated to reflect N/A
SL-0035/0036: Increased width of age field on patient demographics
SL-0035/0036: Ability to add identity numbers, contact numbers in patient demographics
SL-0035/0036: Highlight mandatory fields in patient demographics
SL-0035/0036: Include validation on DOB, cannot be in the future
SL-0035/0036: Include message that patient has been added successfully
SL-0035/0036: Include not selected option for each medication drop down list
SL-0040: Enhanced layout of add attachment panel and aligned test case with on screen formatting
SL-0056: Include patient name in patient view header
Rebranding (SIAPS and USAID)

//0.3.0/20150921
SL-0005 - Lab test converted to drop down list
SL-0049 - Include search for appointments in encounter search - appointment with no encounter 
SL-0050 - appointment with DNA set 
SL-0051 - Search for appointment with encounters
SL-0052 - Mark appointment as DNA

//0.4.1/20151007
SL-0085 - Encounter search facilities drop down list not displaying any facilities for user pgill (HasFacility check failing)
SL-0086 - Fixed bug - Object variable not set when accesing dataset elements
SL-0087 - Facilities drop down not displaying down icon on encounter search

//0.4.2/20151008
SL-0053 - Able to enter appointment in the past - Able to enter appointment on public holiday
SL-0054 - Able to edt appointment to one in the past
SL-0070 - Display successful appt added message
SL-0076 - Check in twice - multiple appts on same day

//0.4.4/20151015
Include middle name
Include Naranjo and WHO causality scales
Include MedDRA terminology

// 0.4.5/20151018
Always default to clinical portal when logging in
Reporting portal
Fix WHO causality issue

// 0.5.0/20151116
Additional reports
Adjusted risk factors
Charting

// 0.5.1/20151118
FIXED BUG: Cast error saving user

SL-0007 - COnditional display of fields
		- Build in dependencies
SL-0038 - Hide all colour options
SL-0041 - ability to delete encounter
SL-0055 - Delete appointment option
SL-0059 - Future date detection on conditions and medications
SL-0063 - error viewing patient
SL-0071 - Missing delete appointment function
SL-0072 - Search for patient error message
SL-0073 - Number is name field
SL-0074 - Delete patient function missing
SL-0078 - showing appt from past
SL-0081 custom attribute menu


SL-0006 - Press enter on patient
SL-0008 - lab ranges
SL-0020 - searchable medication
SL-0058 - Return to appropriate tab once adding patient sub record
SL-0060 - Ability to order attributes
SL-0061 - Ability to detect duplicate conditions

// 0.9.0/20160224
** Fixed home page logo
** Fixed top banner logo per page
** Add spontaneous report
** Paginate user list
** Remove packsize and strength from drug names
** Changed adverse drug reaction header to MedDRA terminology
** Order user list by user name
** Set user inactivity

// 0.9.1/20160225
** Adverse event grid to allow setting of terminology and causality from action drop down list, rather than in field

// 0.10.0/20160227
** Cater for multiple causative assessments (assessment per drug)
** Adverse event and Patients by drug reports cater for multiple causative assessments
** Analyser to cater for multiple causative assessments
** Cater for E2B report creation
** Cater for E2B report XML export

// 0.10.1/20160229
** FIXED BUG: Setting terminology fails due to no includes on PatientClinicalEvent
** FIXED BUG: View cohort resulting in error if not instance found for encounter
** Display drug that has causality been set

// 0.11.0/20160309
** Remove leading and trailing spaces on first name and surname patient and encounter searches
** FIXED BUG: loading Patient on Study report
** Ability to reset password via administration screen
** When editing patient, do not display bottom tabs
** Conditions admin function - first option for labs and meds should be select item to add to list

// 0.11.1/20160310
** FIXED BUG: loading Patient on Study report
** FIXED BUG: loading Patients by drug report
** FIXED BUG: Adverse events report not loading data for defined events (not source) - error on FK
** User must be active to allow login
** Include WHO and Naranjo legend
** Reformatting on WHO and Naranjo scales
** WHO grading Q17 to result in Unclassified if no is answered
** Display admin function for reporting and bpublication portals

// 0.12.0/20160325
** Cater for MedDRA grading
** FIXED BUG: Causality not set report not loading all items where causality not set
** FIXED BUG: Spontaneous report error handling and ability to view newly added report;
** Include wiki and faq system pages

// 0.13.0/20160331
** SL-0183 - New users to have active set to true
** SL-0186/0193 - Date of birth minimum range
** SL-0189 - Ability to navigate to patient from encounter
** SL-0191 - Ability to navigate to patient search from patient view
** SL-0192 - Rename Occupation to Employment Status and add free format Occupation
** SL-0195 - Display mandatory message on patient view when adding or editing
** SL-0198 - Header for appointment status column changed to outcome
** SL-0200 - Do not default gender to male for new patients
** SL-0206/0209 Check role when rendering encounter tab in patient view
** SL-0215 - Update patient medication routes to be drop down list. Custom attribute config.
** SL-0229/0248/0258 - MVC Routing for patient sub tables (contitions etc.)
** SL-0232 - Moved route field under patient medication to top of additional information
** SL-0234 - Disallow widget colour change on both main widgets on encounter form
** SL-0235 - Patient auditability, First name then surname
** SL-0267 - Rename Date of last LMP to Date of last menstrual period

// 0.14.0/20160405
** SL-0201 - Facility and identity number contains invalid characters
** SL-0202 - Check for invalid date of birth (front end control allows 5 digits in year)
** SL-0231/0246 - Patient medication/adverse events/lab tests/condition - mandatory validation
** SL-0242 – Patient name within header when adding medication/condition/lab test/medication
** SL-0255 - Do not allow duplicate capturing of encounters on same day for same patient

// 0.15.0/20160406
** SL-0278 - FIXED BUG: Invalid characters in facility and identity numbers

// 0.16.0/20160408
** SL-0271 - Cohort view, rename adverse event header to adverse reaction
** SL-0272 - Ability to return to cohort when viewing a patient
** SL-0286 - Validate date range for calendar date control
** SL-0288 - Do not lose filter drop down criteria when searching for patient
** SL-0298 - Display patient row count when searching for patient

** 0.17.0/20160411
** SL-0165 - NRIC field rename for spontaneous dataset
** SL-0197 - Patient contact number cannot have alpha characters
** SL-0222 - Do not allow condition start and end dates in the future
** SL-0233 - Do not default medication at outcome at end of treatment episode to cured
** SL-0249 - Open encounter label changed to add encounter
** SL-0261 - Do not allow lab tests in the future
** SL-0296/0314 - Improved message for no results found (patient and encounter search)
** SL-0299 - Name and surname cannot contain non alpha characters
** SL-0303 - Rename calendar to appointments and remove ability to search by encounter from this view
** SL-0305 - Include are you sure message when deleting appointment
** SL-0313 - Error message for adding appointment in the past improved
** SL-0316 - Do not allow editing of appointment in the past
** SL-0317 - Do not display return to cohort button if user did not get there from cohort view
** SL-0319 - Do not present user with option of deleting appointment if in the past
** Rename MedDRA term abbreviations to full length name

** 0.18.0/20160412
** SL-0167 - Dose unit converted to drop down
** SL-0211 - Treatment start date cannot be after condition end date
** SL-0213 - Treatment start date cannot be before condition start date
** SL-0218 - Convert patient medication days/week to drop down list
** SL-0227 - Medication end date cannot be before start date
** SL-0259 - Medication start date cannot be in the future
** SL-0322 - Postal code changed to alphanumeric

** 0.19.0/20160414
** SL-0168 - Patient condition converted to MedDRA

** 0.20.0/20160421
** SL-0159 - Patient on study label change
** SL-0216 - Ordering of term type
** SL-0227 - Improved medication date handling
** Location of mandatory asterisk in relation to label
** SL-0334 - Enrolled label on patient view for facility
** SL-0338 - Mandatory dates to not impact searching for clinical terms
** SL-0339 - Ensure MedDRA term selected when adding new adverse event

** 0.21.0/20160427
** SL-0169 - Test result converted to drop down list
** SL-0170 - Include lab value
** SL-0171 - Lab test unit renamed and new items included in drop down list
** SL-0172 - Update lab tests
** SL-0184/SL-0194/SL-0199 - Improved handling of appointment add and edit
** SL-0188 - Rename label to patient facility number
** SL-0190 - Rename label to patient identity number
** SL-0197 - Validation on patient contact number
** SL-0276 - Change lab test reference to clinical evaluation
** SL-0286 - Improved error message for calendar view appointment date
** SL-0288 - Fixed patient search by facility
** SL-0310 - Confirm attachment deletion
** SL-0300/SL-0318 - Handle DNA appointment from calendar and encounter (fix usability issues)
** SL-0323 - Warn of duplicate encounter when adding
** SL-0329 - Spelling of enroll
** SL-0332 - Change order by on lab test unit
** SL-0344 - Order of adverse events changed to use onset date
** SL-0348 - Delete patient archive
** SL-0353 - Patient search wording on patient view
** SL-0355 - Improved date handling on encounter search
** SL-0356 - Patient demo error messages improved
** SL-0363 - Patient search by facility not facttoring in patient's current facility correctly
** SL-0368 - Encounter search by facility not returning results
** Improved date handling for patient search, clinical events, conditions, lab tests, medications, all system reports
** Changed ordering of user email address in user admin

** 0.22.0/20160428
** SL-0351 - Remove enrollment
** SL-0352 - De-enrollment
** Sl-0370 - Check for numerics in first and last name on patient and encounter search
** SL-0371 - Middle name validation on patient view
** SL-0372 - Note actions for invalid characters in patient view

** 0.23.0/20160504
** SL-0154 - Remove Meddra Codes from terminology searches
** SL-0173 - E2B field c.1.6.1 and c.1.8.1 modifications
** SL-0175 - E2B Field mappings for C.1.7
** SL-0360 - Start date label for medication corrected
** SL-0365 - Cancelling new spontaneous report results in error message
** SL-0367 - Do not remove defaulted values for E2B forms (check if value exists before updating)
** SL-0374 - Cancel button working when receive duplicate encounter message
** SL-0377 - Change discharged to open/close in encounter search
** SL-0378 - Colour coding for encounter searches corrected
** SL-0380 - Allow appointment searching in the future
** SL-0394 - Only allow single selection of MedDRA terminology
** SL-0395 - Move rechallenge to medications
** SL-0399 - Medication label change
** SL-0400/0401 - Removed treatment outcome and date from medication and moved to new TB Treatment category in encounter
** SL-0402/0403 - Medication primary field renamed and ddl updated
** SL-0404 - New field on medication - Indication Judgement
** SL-0409 - Adverse event severity label change
** SL-0410 - Cater for new field on adverse event - posted to national PV
** SL-0413 - Remove leading and trailing ** from status values in encounter search

** 0.24.0/20160504
** SL-0421 - Encounter list in descending order in patient view
** SL-0423 - Do not allow encounters in the future
** SL-0432 - Allow editing of E2B sub tables
** Ability to set meddra term for spontaneous reports
** Fixed search for MedDRA term by name in analytical
** Default spontaneous data into E2B report
** Ability to view spontaneous report in analytical portal
** Remove chronic element indicator on encounter edit/view
** Rename patient facility number to patient file number

** 0.25.0/20160516
** SL-0238 - All drop downs changed to include down arrow
** SL-0336 - Prevent spont system from capturing duplicate reports
** SL-0349 - Ability to delete encounter (archive with reason)
** SL-0359 - Error saving patient medication (unable to populate list items if validation error)
** SL-0364 - MedDRA search selection lost when validation error on submission
** SL-0418 - Do not allow editing of patient if user doews not have access to patient facility
** SL-0422 - Include dob as search option on patient search
** SL-0429 - Rename details field to notes for patient status
** SL-0438 - E2B field C.1.6.1 Are Additional Documents Available? converted to drop down
** SL-0439 - E2B Nullflavor changed to Unknown
** SL-0444 - Filter div on encounter and patient searches set to auto height with improved padding
** SL-0445 - Do not display encounter success message not displayed when validation error with filter criteria
** SL-0446 - Do not display patient success message not displayed when validation error with filter criteria
** SL-0449 - Updated list of common MedDRA terms
** SL-0451 - Do not display ignored medications in active and spontaneous reports
** SL-0452 - DOB to not be mandatory when searching for patient
** SL-0453 - Standardised date control (HTML5 compliant)

** 0.26.0/20160520
** SL-0457 - Selected tab to have orange background colour
** SL-0458 - Order for active and spontaneous actions to changes - set terminology first
** Move notes tab to end of clinical categories in encounter view
** Reporting and analytical portal updated to reflect standardised error handling and row counts

** 0.26.1/20160525
** SL-0450 - Include verbatim event description for adverse events

** 0.27.0/20160606
** SL-0269/0365 - Display user facilities and roles as part of user profile
** SL-0469 - Ability to export reports to pdf, xml, xls 

** 0.28.0/20160610
** SL-0464 - Activate condition to terminology referencing
** SL-0468 - Gender related fields conditional on encounter view
** SL-0470 - Remove references to ajax.googleapis.com in bootstrap template 

** 0.29.0/20160615
** SL-0461 - Display user name when resetting password
** SL-0462 - Heading text change for edit a user
** SL-0463 - Display user full name when editing a user
** SL-0465 - Ability to manage encounter type and work plans
** SL-0466 - Ability to edit encounter type work plan
** SL-0480 - Include EULA for acceptance on first user login

** 1.0.1/20160624
** FIXED BUG: Object variable not set error when adding new user

** 1.0.2/20160629
** Default MedDRA search term type to LLT in adverse event and conditions views
** Ability to define is searchable parameter for custom attributes

** 1.0.3/20160630
** Calculate BMI automatically
** Reorder BMI
** Round BMI to 2 decimals
** Display event duration for adverse events with an onset and resolution date
** Include default value of spaces and a key of 0 for newly added selection custom attributes

** 1.0.4/20160701
** SL-0464 Input string not in correct format when adding patient
** SL-0464 Do not display text version of facility number in patient view when adding patient
** SL-0467 Download all attachments bug fixed
** SL-0469 Lab test and medication drop downs include blank value
** SL-0471 Date and time displaying in medication start date and lab test date
** SL-0472 Realtime rendering of BMI
** SL-0474 User name must be clickable in all footers to allow viewing of roles and facilities
** SL-0475 Remove tx start date from condition (first class property)
** SL-0478 Display patient search button for readonly patients
** SL-0479 Reposition readonly display in patient view
** Include hospitalisation admission and discharge dates as well as date of death and autopsy fields

** 1.0.5/20160704
** FIXED BUG: Readonly label appearing when adding new patients
** FIXED BUG: Medications overlap prevented (combined start and end dates)
** Reinstate treatment start date on patient condition
** Edit mode for conditions and adverse events to compress MedDRA search on first load
** SL-0479 Cater for archived patient medication records when checking for overlapping meds

** 1.0.6/20160705
** FIXED BUG: Medications overlap prevented (combined start and end dates)
** FIXED BUG: Clinical Events overlap prevented (combined onset and resolution dates)
** FIXED BUG: Conditions overlap prevented (combined start and end dates)

** 1.0.7/20160713
** FIXED BUG: Conditions/Clinical Events overlap prevented (source terminology not set resulted in non validation of overlapping dates)

** 1.1.0/20160715
** Real time calculation of event duration in adverse events
** Ability to search on patient using custom attributes
** FIXED BUG: MVC related menu items to remain expanded
** Include base scripts for post deployment process
** Merge core data elements post Georgia release
** Update offline logo

** 1.2.0/20160911
** Fix encounter search offline (search by name and surname)
** Fix meddra search offline (conditions and adverse events)
** Fix formatting for patient search offline 
** Lab value must not be mandatory in offline capture mode
** Remove option to decline going into online mode from offline

** 1.2.4/20161010
** Include additional facility contact details
** Patient summary merge

** 1.2.5/20161011
** SL-1001 Fix formatting of add encounter date
** Adverse events renamed to Event Description (As stated by patient)
** Move identifiers and audit information to end of patient view
** Include admin function for lab results
** Display list of condition groups in encounter view

** 1.2.6/20161013
** Modify ordering of lab test units
** Order element values alphanumerically
** Manage configurations
** Automated E2B handler
** FIXED BUG: Interop service not processing adverse event custom attributes
** Ability to conditionally display dataset categories
** Display condition group summary in encounter view

** 1.2.7/20161015
** FIXED BUG: Analytical calculation  distinct on patient id when calculation non exposed cases
** Remove MedDRA code from causality not set report
** Removed ** in patient view (lab test and cohorts)

** 1.2.8/20161018
** Allow adding of new encounter during patient add
** Enforce capturing of condition during during patient add

** 1.2.9/20161024
** Conditional check on Is Serious on adverse event view
** Fixed error handling encounter GUID in interop

** 1.3.0/20161025
** Security vulnerability fixes
** Fixed issue where all elements are removed in E2B extract if error is found

** 1.4.0/20161103
** Redesign of analyser for improved UX 
** Fixed bug rendering adjusted risk factors
** Include patient list in analyser
** Include dataset dump function in analyser
** Ability to configure assessment scale (WHO, Naranjo or both)
** Include admin audit trail
** Bind interop service to audit trails
** Include MedDRA refresh
** Update EULA to reflect MedDRA and analyser disclaimers
** Update analyser to reflect exposed case disclaimer

** 1.4.1/20161107
** Update branding

** 1.4.2/20161110
** Fix meta data updating

** 1.4.3/20161113
** Causality not set report uses config setting for assessment scale
** Improve text on analyser (graph headers and disclaimer text)
** Include not set in Priority seeding
** Improve error handling when saving new dataset category in dataset admin
** Improve dataset element management within dataset admin
** FIXED BUG: Saving new element value (object variable null error)
** Include indication type in medication grid
** Change order of medication grid
** Fixed formatting for appointment and encounter date
** Patient and encounter search to do contains search on First and surname if search value is more than 3 characters. Starts with if less than 3 characters
** Ability to sort clinical information in patient view
** Include ability to search for encounter by custom attributes
** Removed link between clinical event and encounter
** Include error trapping when running analyser
** Improved analyser graphing
** When adding new patient display error message if condition group not selected
** Include Indication type in WHO and Naranjo assessment grids
** Duplication check on facility code when adding or editing a facility
** For medications, conditions, adverse events and evaluations. return to add view after adding a new record
** Display source event when setting causality
** Do not allow adding of lab tests, evaluations, clinical events, medications from patient view if readonly
** Include patient view link in active reporting
** Fixed bug saving facility with no contact numbers
** Fixed facility field naming for admin
** Remove specific validation on is serious for adverse events (certain fields displayed)
** Include custom error handler
** Lab test value to not have a default value of 0 when adding a lab test
** Display attribute detail on adverse events, medication, clinical events and evaluations
** Include age group category for patient and clinical event
** Treatment start date to be mandatory if meddra term linked to condition group
** Prompt user to confirm patient is marked as deceased
** Auto set patient status to deceased if outcome set to fatal
** Display condition group summary in patient view

** 1.4.4/20161123
** Remove condition treatment start date
** Include condition treatment outcome
** Condition outcome date to be mandatory if outcome is set
** Condition groups - only display open cases and word change (encounter and patient view)
** Fix bug on dataset management. Chronic conditions not loading correctly per dataset category
** Replace treatment start date with condition start date in analysis
** Include label outlining need for start and outcome dates an add and edit conditions
** Remove MedDRA term code from adverse events report
** Allow current date for past and future date attribute
** Display condition groups on encounter view corrected
** Download dataset to be user configurable

** 1.4.5/20161129
** Display naranjo total score on calculation
** Ensure all WHO and Naranjo questions answered before saving
** Display source and selection meddra terms when setting causality for WHO or Naranjo
** Display ignored status for medications (Naranjo and WHO in active reporting)
** Display all medicines (even not set) (Naranjo and WHO in active reporting)
** Ensure active reporting causality colour coding is consistent (green if included in analysis and blue if not)
** Advise user to check medication history if condition outcome changes
** Interop service to cater for spaces in subsriber code and cater for subscriber code in stub

** 1.4.6/20161206
** Conditions and medications alert for analytical contribution
** Display condition meddra term and start date in TB Condition tab in encounter view
** Rename clinical evaluations to test and procedures (adjust field names)
** Move appointments to below cohorts in menu and patient search to be home page
** Event description (as stated by patient or reporter) text change
** Include alert for adverse events that are death related
** E2B context management for datasets
** Custom viewer modifications
** Fixed E2B routing (using tag)
** Remove site of TB from interop stub
** Fixed bug displaying ADR status in analysis patient view

** 1.5.0/20161227
** Include condition group for cohort management
** Check for dataset context when getting current weight for patient in cohort view
** LLT MedDRA term can only be linked to a single condition group
** Only allow enrollment into cohort group if patient is linked to same condition group as for cohort
** Analyser condition tab changed to population
** Analyser population tab, rename condition to primary condition risk factor and include cohort selection
** When adding new patient, rename patient condition to primary condition group, rename conditions label to condition groups and include ability to enrol patient
** Auto scroll on patient view
** Include MedDRA term and start date in encounter view for condition groups
** Cater for cohorts in analytical processes
** Fix EF Context bloating for MedDRA import
** Update common MedDRA terminology ID's as per v19.1 import

** 1.5.1/20170103
** Fixed bug: null object variable when editing cohort
** Return MedDRA terms in alphabetical order when assigning MedDRA terms to condition group
** Return MedDRA terms in alphabetical order when assigning MedDRA term to adverse event
** Remove variable header on patient view for new patient and condition and include Additional Information legend
** Highlight OR relationship between condition group and cohort drop down lists in analyser

** 1.5.2/20170106
** Fixed bug: EntityValidation error when saving PatientCondition on interop. Patient not included when getting this record.
** Align Interop with condition, labs and encounter entity changes

** 1.5.3/20170124
** Various spelling fixes
** Various cosmetic fixes (patient view)
** Various cosmetic fixes (manage condition)
** Various cosmetic fixes (manage scale gradings)
** Remove additional details section under conditions
** Remove duplication of menu items
** Fix non standard characters in medication import
** Remove cohort view sub link
** Dose to not be mandatory
** Changed read only message on patient view
** Always display Add Encounter button in patient view
** Rename causality not set report
** Cater for reference lower and upper for patient lab test
** FIXED BUG: lab test unit not displayed on patient view
** Appointment cancelled drop down field to include arrow
** When displaying common error page, use DB context to load user facilities (UnitOfWork not injected)
** FIXED BUG: Causality system report. Not set not returning correct values.
** MedDRA admin to default SOC search (not LLT)
** FIXED BUG: Removing cohort results in patient record being deleted
** Ability to set enrolled and de-enrolled date for patient cohorts

** 1.5.4/20170131
** Surname renamed to last name (encounter search and active reporting)
** Increased facility name length to 100 and screen changes to accomodate
** Allow username of 30 characters
** Do not display comments for patient status history in patient view
** Convert lab value from decimal to text(20)
** When adding a new patient, allow adding of cohort enrollment date if allocating to cohort
** Allow space in first, middle and last name on patient view
** Display lab test range in patient and encounter lab list view

** 1.5.5/20170209
** Adverse event, condition, medications and lab tests, primary attribute to be readonly in edit mode
** Fixed range header for lab tests in encounter view
** Standardise error message for cohort enrollment date on patient view
** Do not allow selection of attachment file if read only
** Fixed bug saving encounter fields that appear after hidden gender based fields
** Fixed bug displaying encounter MVC validation summaries
** Ability to view encounter details in encounter view (date and type)

** 1.5.6/20170214
** Do not set enrollment date for new patient if condition selected. Only of cohort selected.
** Activate confirmation messages for web forms
** Include * for deletion reasons

** 1.5.7/20170214
** Fixed issue with assigning datasets to work plan
** Activate confirmation messages for mvc forms
** fixed deletion of clinical event, condition, medication and lab test

** 1.5.8/20170221
** Allow period in facility name administration and convert facility management to MVC
** Improve patient validation and save process
** Remove NOT SELECTED default selection item in medication controller
** Display verbatim source in terminology setting for analytical portal

** 1.5.9/20170302
** Include upper and lower wording in range limits in patient and encounter view
** Include combined header for patient table in analyser
** Store currentcontext in session

** 1.5.10/20170317
** Include descriptions in analytical legend for exposed and non exposed cases and non cases
** Remove encounter status from patient view encounter grid and encounter search grid
** Display user full name and username when resetting password
** Improved duplicate encounter date message when adding encounter
** Setting causality to only be applied to medications that fall within onset date range
** Display onset, start and end date for medications on causality screens
** Include warning for email address in user management
** Pop up message fors appointment editing
** Ignore archived records for active reporting

** 1.5.11/20170327
** Duplicate check on amended appointment date and commit date on modification
** Fixed patient add and update pop up messages
** Patient deletion mandatory asterisk
** Refactoring (pop u message, buttons) for patient cohorts
** E2b Spontaneous initialisation

** 1.5.12/20170411
** Pop up message display for analytical update actions
** Include dataset mapping for E2B spontaneous R2

** 1.5.13/20170507
** Include ability to edit spontaneous reports in analytical portal

** 1.5.14/20170512
** Reporting and regulatory authorities for E2B mapping

** 1.5.15/20170517
** Remove encounter ID from encounter search grid
** Adverse Event count by drug chart in analyser to use set interval for y axis
** Criteria label not displaying on active and spontaneous reporting
** Include round brackets as valid characters for facility name
** E2B dataset to be labeled under analytical portal
** Remove all reference to demo.css and demo.js
** Cater for archived value on patient when checking for duplicates

** 1.5.16/20170528
** Include E2B R2 active mappings

** 1.5.17/20170531
** E2B R2 active mapping corrections

** 1.5.18/20170602
** E2B R2 active mapping corrections (lab tests in relation to onset date)

** 1.5.19/20170609
** E2B R2 active mapping corrections (cater for comments in patientclinicalevent and patientmedication)

** 1.5.20/20170613
** E2B R2 active and spontneous mapping corrections (UMC feedback)
** Display medication form in patient medicaton and map to E2B R2 for active

** 1.5.21/20170617
** E2B active and spontaneous attribute to value mappings corrected and E2B R2 refactoring

** 1.5.22/20170623
** Include inline logos for portal selection

** 1.5.23/20170623
** When selecting MedDRA terms using hierarchy, only display sub drop downs when previous drow down has been selected (e.g. only display HLGT when SOC has been selected)
** Do not allow causality scale capturing until drug has been selected
** When causality has been set, return to initial causality screen (do not show selected drug)
** Accept cohorts with End Date that is later than the current date (but within 20 years)
** Improve result handling for search MeddDRA by code and term functionality
** Colour coding for notification blocks in analyser
** Ensure analytical tabs use standardised background and foreground colour
** MedDRA term must be set before causality can be set
** Weight to be mandatory (datapack mod)
** Encounters associated to an appointment criteria modified (between 1 and 5 days) - no longer (between -5 and 5 days)
** Fixed alignment of widgets on encounter view

** 1.5.24/20170717
** Patient lab refactoring (resetting nullable entities, error handling
** User deletion refactoring
** Condition comments increased to max length of 500
** Refactoring of all widgets - appearance and buttons

** 1.5.25/20170718
** Include irreversable warning for patient deletion
** Include irreversable warning for clinical event, condition, medication and clinical evaluation deletion
** Include irreversable warning for appointment deletion and remove delete button confirmation check
** Check for no patient record loaded when viewing archived patient
** Include irreversable warning for cohort enrollment and de-enrollment
** Patient view, cohort view, calendar view, manage functions, encounter view action drop downs refactored
** Include patient and encounter into edit and delete buttons in patient and encounter view

** 1.6.0/20170801
** Include dataset rules
** Group by required information in spontaneous reporting
** Admin function for dataset management to cater for friendlyname and help fields for category and category element
** Display friendly category and element names in dataset views
** Display element and category help
** Cater for bootstrap wizard for dataset modification (add spontaneous)
** Include category and element ordering ability in dataset admin

** 1.6.1/20170808
** Fixed bug reloading grids on dataset edit administration
** Fixed bug delete element value causing object variable error

** 1.6.2/20170811
** Generate SQL script for dataset

** 1.7.0/20170817
** Include friendly name and help on sub elements
** Cater for inline table rendering for sub tables

** 1.7.1/20170820
** Auto create terminology elements for Meddra, WHO and Naranjo
** Update spontaneous reporting to use new elements
** Update E2b spontaneous views to reflect tables and new formatting

** 1.7.2/20170822
** For sub tables, prevent multiple submission of sub table rows by undbinding click event

** 1.7.3/20170823
** Include ability to download spontaneous extract

** 1.7.4/20170824
** Fixed formatting and display of spontaneous medication and causality 
** Fixed bug ignoring medicine which overwrites product name and not terminology

** 1.7.5/20170825
** Include sub element mappings
** Include sub tables when generating E2B XML dump file

** 1.7.6/20170827
** Fix dataset element deletion
** Fix bug rendering reaction start for causality scales (duplicate element)

** 1.8.0/20170913
** Remove dependency on reaction serious field in E2B mapping for spontaneous datasets
** Include option to stay online if offline detected
** Include org unit hierarchy for facilities
** Do not default start dates when adding condition or medication
** Do not default start date when adding cohort
** Ensure Additional Details header appears for Patient Conditions
** Allow searching for future appointments
** Display date format for date fields in spontaneous report view
** Remove ** not set ** as encounter type
** Order all sub element field values by value name (ascending when populating ddl)
** Appointment deletion refactoring
** Fixed hiding and showing of causality grids
** Cater for sub element help and friendlyname
** Fixed issue deleting sub record from table element
** Include ability to reorder sub elements
** Fix duplication of causality when ignoring drug
** Fix display for accept EULA button
** Fix deletion of patient record
** Defaulty Meddra common list using common attribute on entity
** Check dataset when validating dataset elements in interop
** Fixed Meddra term and causality not set for spontaneous reporting 
** Modify dataset element admin list
** Include weight history in encounver view

** 1.8.1/20170927
** Fixed bug saving MedDRA terminology for spontaneous report

** 1.9.0/20171107
** Do not display <<Table>> value when rendering patient extract
** Handle referrer when deleting sub instance values and include pop up message
** Fix error displaying action column for WHO and Naranjo causality for spontaneous reports
** Remove no as option from WHO causality - question 17

** 1.9.1/20171113
** Fix patient summary file corruption
** Fixed display of verbatim source and meddra term for terminoloy setting

** 1.10.0/20171219
** Work flow processing

** 1.10.1/20171228
** E2B dataset to be linked to work flow processing
** Drop MedicationCausality entity
** Display work flow activity in patient view
** E2B mappings for active sub tables (drugs and labs)
** Include MVC for attachment delete

** 1.10.2/20180117
** Fix rendering of causality status in naranjo and who causality setting (not set always displays)
** Removed additional occurrences of MedicationCausality table
** Creation of report service
** Include medication into causality report
** Move all reports to reporting service and cater for archived records

** 1.11.0/20180122
** Include unnecessary as option for Q5 WHO causality
** Activity comments for workflow not mandatory
** Cater for treatment outcome in dataset download
** Cater for spontaneous dataset download in analyser
** Cater for serious adverse event report
** Cater for activity history records with no created user (public)
** Reroute to new report list from edit public report

** 1.11.1/20180206
** Setting activity execution status searching for clinical event for spontaneous records. Fixed.
** Check for guid when setting spontaneous causality (WHO and Naranjo)
** Attachment error handling improved

** 1.11.2/20180209
** Fixed bug processing encounter validation in interop

** 1.11.3/20180211
** Include serious indication in adverse event grids (patient and encounter view)
** Fixed loading of cohort view - get latest meddra from work flow
** Include analytical reporting activity in clinical event view

** 1.12.0/20180221
** Refactoring of publication portal
** Include ability to delete cohort
** Fixed routing of patient information - always route back to patient view
** Patient summary extracts refactored to generate dynamically (do not use templates)
** Fixed mapping for E2B spontaneous seriousness
** Fixed rerouting for E2B (once committed)
** Custom info page viewer

** 1.13.0/20180411
** Refactoring of publication portal
** Allow modification of custom attributes

** 1.13.1/20180412
** Refactoring of publication portal - adding and editing widget content

** 1.13.2/20180416
** Refactoring of publication portal - admin functions for info portal improved

** 1.14.0/20180417
** Refactoring of publication portal - admin functions for info portal improved
** Include ability to move widgets to new page
** Cater for unpublished widgets

** 1.15.0/20180427
** Cater for artefact service
** Move report meta to admin
** Allow selection of icon when managing widget
** Do not allow viewing of clinical information in patient view if user does not have access to facility

** 1.15.1/20180430
** Content change for analyser incidence rate text

** 1.15.2/20180502
** Distribution for OpenXmlPowerTools
** Reporting portal refactoring - Move reports to metatable

** 1.16.0/20180706
** Renaming of all namespaces to PVIMS

** 1.16.1/20180812
** Alignment with manuals (user interface refactoring)

** 1.16.2/20181010
** Alignment with manuals (admin interface refactoring)
** Fixed password reset issue

** 1.17.0/20181010
** Move admin menu to admin portal

** 1.17.1/20181101
** Information portal - rename text for widget location drop down list

** 1.17.2/20181106
** Bypass application cache for non-https implementations
** Fixed spelling of Management Sciences

** 1.17.3/20181113
** Patient Condition start date should be earlier than the Cohort enrollment date
** Rename meta widget type from wiki to subitems
** Error checking added on all datepicker (regex check on format)

** 1.18.0/20181114
** Ability to add page from within subitem widget
** Display alert if an appointment is being generated on a holiday
** Correcct auto population of custom attributes
** Include serious into causality report
** Include grade 5 and grade unknown into quarterly and annual adverse event reports
** Adverse event report stratified by seriousness and include cohort as criteria
** Store MetaDataLastUpdated time when refreshing
** Patients on treatment report fixed to reflect serious events
** Display last meta refresh data on each system report and admin page for meta refresh
** Cohort view modified to display serious and non serious patient counts (total and per patient)
** Include in progress waiting screen for active dataset download, analyser, E2B creation
** All system reports to include date and time of extract when extracting to XLSX
** Display grading scales when selecting severity grade scale in adverse event (add and edit)
** Microsoft.Data.Odata updated to version 5.8.4 in line with OData Denial of Service Vulnerability
** When adding a new widget in the info portal, ensure the widget is set to an unpublished status and location on change to be based off jquery and not postback
** Decode widget name and definition when rendering in widget admin
** Fix customisation of existing custom reports
** Ability to downlaod analytical dataset for specific cohorts only

** 1.18.1/20190102
** Fixed formatting of sub tables
** Changed page icon from stethoscope to dashboard for E2B views in analytical portal

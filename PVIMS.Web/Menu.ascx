<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Menu.ascx.cs" Inherits="PVIMS.Web.Menu" %>
<%@ Import Namespace="PVIMS.Core" %>

<div>

	<nav id="navReporter" runat="server">
		<!-- NOTE: Notice the gaps after each icon usage <i></i>..
				Please note that these links work a bit different than
				traditional href="" links. See documentation for details.
				-->
		<ul>
			<li id="reportadmin" runat="server" visible="true">
                <a href="#" title="System Reports"><i class="fa fa-lg fa-fw fa-bar-chart-o"></i><span class="menu-item-parent">Standard Reports</span></a>
                <ul>
			        <li id="reportpxonstudy" runat="server">
                        <a href="/Reports/System/ReportPxOnStudy.aspx" title="Patients on Treatment Report"><span class="menu-item-parent">Patients on Treatment</span></a>
			        </li>
			        <li id="reportadverseevent" runat="server">
                        <a href="/Reports/System/ReportAdverseEvents.aspx" title="Adverse Events Report"><span class="menu-item-parent">Adverse Events</span></a>
			        </li>
			        <li id="reportadverseeventq" runat="server">
                        <a href="/Reports/System/ReportAdverseEventQuarterly.aspx" title="Quarterly Adverse Events Report"><span class="menu-item-parent">Quarterly Adverse Events</span></a>
			        </li>
			        <li id="reportadverseeventa" runat="server">
                        <a href="/Reports/System/ReportAdverseEventAnnual.aspx" title="Annual Adverse Events Report"><span class="menu-item-parent">Annual Adverse Events</span></a>
			        </li>
			        <li id="reportcausalitynotset" runat="server">
                        <a href="/Reports/System/ReportCausality.aspx" title="Causality Report"><span class="menu-item-parent">Causality</span></a>
			        </li>
			        <li id="reportdrug" runat="server">
                        <a href="/Reports/System/ReportDrug.aspx" title="Patients on Drug Report"><span class="menu-item-parent">Patients by Drug</span></a>
			        </li>
			        <li id="reportoutstandingvisit" runat="server">
                        <a href="/Reports/System/ReportOutstandingVisit.aspx" title="Outstanding Visits Report"><span class="menu-item-parent">Outstanding Visits</span></a>
			        </li>
			        <li id="reportsae" runat="server" visible="false">
                        <a href="/Reports/System/ReportSAE.aspx" title="SAE Report"><span class="menu-item-parent">Serious Adverse Events</span></a>
			        </li>
                </ul>
            </li>
			<li id="reportlist" runat="server" visible="false">
                <a href="/Reports/ReportList.aspx"><span class="menu-item-parent"><i class="fa fa-lg fa-fw fa-windows"></i>Report List</span></a>
			</li>
		</ul>
        <span id="spnCustomReportList" runat="server"></span>
	</nav>

	<nav id="navAnalytical" runat="server">
		<!-- NOTE: Notice the gaps after each icon usage <i></i>..
				Please note that these links work a bit different than
				traditional href="" links. See documentation for details.
				-->
		<ul>
			<li id="spontaneous" runat="server">
                <a href="#" title="Spontaneous"><i class="fa fa-lg fa-fw fa-dashboard"></i><span class="menu-item-parent">Spontaneous</span></a>
                <ul>
			        <li id="spontaneousreporting" runat="server">
                        <a href="/Analytical/ReportInstanceList.aspx?wuid=4096D0A3-45F7-4702-BDA1-76AEDE41B986" title="Reporting"><span class="menu-item-parent">Reports</span> <span class="badge bg-color-red pull-right inbox-badge" id="spnSpontaneousCount" runat="server" style="display:none;">0</span></a>
			        </li>
			        <li id="spontaneousanalyserview" runat="server">
                        <a href="/Analytical/SpontaneousAnalyser.aspx" title="Analyser"><span class="menu-item-parent">Analyser</span></a>
			        </li>
                </ul>
			</li>
			<li id="active" runat="server">
                <a href="#" title="Active"><i class="fa fa-lg fa-fw fa-dashboard"></i><span class="menu-item-parent">Active</span></a>
                <ul>
			        <li id="activereporting" runat="server">
                        <a href="/Analytical/ReportInstanceList.aspx?wuid=892F3305-7819-4F18-8A87-11CBA3AEE219" title="Reporting"></i><span class="menu-item-parent">Reports</span> <span class="badge bg-color-red pull-right inbox-badge" id="spnActiveCount" runat="server" style="display:none;">0</span></a>
			        </li>
			        <li id="analyserview" runat="server">
                        <a href="/Analytical/Analyser.aspx" title="Analyser"><span class="menu-item-parent">Analyser</span></a>
			        </li>
                </ul>
			</li>
		</ul>
	</nav>

	<nav id="navPublisher" runat="server">
		<!-- NOTE: Notice the gaps after each icon usage <i></i>..
				Please note that these links work a bit different than
				traditional href="" links. See documentation for details.
				-->
        <span id="spnCustomPublisherList" runat="server"></span>
		<ul>
			<li id="publishadmin" runat="server" visible="false">
                <a href="/Publisher/PageCustom.aspx?id=0" title="Add new page"><i class="fa fa-lg fa-fw fa-windows"></i><span class="menu-item-parent">Add new page</span></a>
			</li>
			<li id="publishadminlist" runat="server" visible="false">
                <a href="/Publisher/PageList.aspx" title="List Pages"><i class="fa fa-lg fa-fw fa-windows"></i><span class="menu-item-parent">List Non-Visible Pages</span></a>
			</li>
		</ul>
	</nav>

	<nav id="navOLTP" runat="server">
		<!-- NOTE: Notice the gaps after each icon usage <i></i>..
				Please note that these links work a bit different than
				traditional href="" links. See documentation for details.
				-->
		<ul>
			<li id="patientview" runat="server">
                <a href="/Patient/PatientSearch.aspx" title="Patients"><i class="fa fa-lg fa-fw fa-group"></i><span class="menu-item-parent">Patients</span></a>
			</li>
			<li id="encounterview" runat="server">
                <a href="/Encounter/EncounterSearch.aspx" title="Encounters"><i class="fa fa-lg fa-fw fa-file-text-o"></i><span class="menu-item-parent">Encounters</span></a>
			</li>
			<li id="cohortview" runat="server">
                <a href="/Cohort/CohortSearch.aspx" title="Cohorts"><i class="fa fa-lg fa-fw fa-cogs"></i><span class="menu-item-parent">Cohorts</span></a>
			</li>
			<li id="calendarview" runat="server">
                <a href="/Calendar/CalendarView.aspx" title="Calendar"><i class="fa fa-lg fa-fw fa-calendar"></i><span class="menu-item-parent">Appointments</span></a>
			</li>
		</ul>
	</nav>

    <nav id="navAdmin" runat="server">
        <ul>
			<li id="adminview" runat="server">
				<a href="#" title="Administration"><i class="fa fa-lg fa-fw fa-windows"></i><span class="menu-item-parent">Administration</span></a>
				<ul>
					<li id="audittrail" runat="server">
						<a href="/Admin/AuditTrail.aspx">Audit Trail</a>
					</li>
					<li>
						<a href="#" title="Reference Data"><span class="menu-item-parent">Reference Data</span></a>
						<ul>
					        <li id="admincondition" runat="server">
						        <a href="/Admin/ManageCondition.aspx" title="Condition Groups"><span class="menu-item-parent">Condition Groups</span></a>
					        </li>
					        <li id="admingrading" runat="server">
						        <a href="/Admin/ManageGrading.aspx" title="Gradings"><span class="menu-item-parent">Scale Gradings</span></a>
					        </li>
					        <li id="adminlabresult" runat="server">
						        <a href="/Admin/ManageLabResult.aspx" title="Test Results"><span class="menu-item-parent">Test Results</span></a>
					        </li>
					        <li id="adminlabtest" runat="server">
						        <a href="/Admin/ManageLabTest.aspx" title="Tests and Procedures"><span class="menu-item-parent">Tests and Procedures</span></a>
					        </li>
					        <li id="adminmedicine" runat="server">
						        <a href="/Admin/ManageMedicine.aspx" title="Medicines"><span class="menu-item-parent">Medicines</span></a>
					        </li>
					        <li id="adminmeddra" runat="server">
						        <a href="/Admin/ManageMedDRA" title="Terminology MedDRA"><span class="menu-item-parent">MedDRA</span></a>
					        </li>
						</ul>
					</li>
					<li>
						<a href="#" title="System Configuration"><span class="menu-item-parent">System Config</span></a>
						<ul>
					        <li id="adminconfig" runat="server">
						        <a href="/Admin/ManageConfig.aspx" title="Configurations"><span class="menu-item-parent">Configurations</span></a>
					        </li>
					        <li id="admincontact" runat="server">
						        <a href="/Admin/ManageContactDetail.aspx" title="Contact Details"><span class="menu-item-parent">Contact Details</span></a>
					        </li>
					        <li id="adminfacility" runat="server">
						        <a href="/Facility/Index" title="Facilities"><span class="menu-item-parent">Facilities</span></a>
					        </li>
                            <li id="adminholiday" runat="server">
                                <a href="/Admin/ManageHoliday.aspx" title="Holidays"><span class="menu-item-parent">Public Holidays</span></a>
                            </li>
                            <li id="adminreport" runat="server">
                                <a href="/Admin/ReportMeta.aspx" title="Holidays"><span class="menu-item-parent">Report Meta Data</span></a>
                            </li>
						</ul>
					</li>
					<li>
						<a href="#" title="User Configuration"><span class="menu-item-parent">User Config</span></a>
						<ul>
					        <li id="adminuser" runat="server" visible="true">
						        <a href="/UserAdministration" title="User Administration"><span class="menu-item-parent">Users</span></a>
					        </li>
					        <li id="adminrole" runat="server" visible="true">
						        <a href="/RoleAdministration" title="Role Administration"><span class="menu-item-parent">Roles</span></a>
					        </li>
						</ul>
					</li>
					<li>
						<a href="#" title="Work Configuration"><span class="menu-item-parent">Work Config</span></a>
						<ul>
					        <li id="admincustom" runat="server">
						        <a href="/CustomAttributeConfig" title="Custom Attribute Administration"><span class="menu-item-parent">Custom Attributes</span></a>
					        </li>
					        <li id="admindataset" runat="server" visible="true">
						        <a href="/Admin/ManageDataset.aspx" title="Work Plans"><span class="menu-item-parent">Datasets</span></a>
					        </li>
					        <li id="admindatasetelement" runat="server" visible="true">
						        <a href="/Admin/ManageDatasetElement.aspx" title="Work Plans"><span class="menu-item-parent">Dataset Elements</span></a>
					        </li>
					        <li id="admincareevent" runat="server" visible="true">
						        <a href="/Admin/ManageCareEvent.aspx" title="Care Events"><span class="menu-item-parent">Care Events</span></a>
					        </li>
					        <li id="adminencountertype" runat="server" visible="true">
						        <a href="/Admin/ManageEncounterType.aspx" title="Encounter Types"><span class="menu-item-parent">Encounter Types</span></a>
					        </li>
					        <li id="adminworkplan" runat="server" visible="true">
						        <a href="/Admin/ManageWorkPlan.aspx" title="Work Plans"><span class="menu-item-parent">Work Plans</span></a>
					        </li>
						</ul>
					</li>
				</ul>
			</li>

        </ul>
    </nav>

	<span class="minifyme" data-action="minifyMenu">
		<i class="fa fa-arrow-circle-left hit"></i>
	</span>
</div>


<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="PatientView.aspx.cs" Inherits="PVIMS.Web.PatientView" MasterPageFile="~/Main.Master" Title="Patient View" %>
<%@ Import Namespace="System.Web.Optimization" %>

<%@ Register Assembly="CKEditor.NET" Namespace="CKEditor.NET" TagPrefix="CKEditor" %>

<asp:Content runat="server" ID="Breadcrumbcontainer" ContentPlaceHolderID="breadcrumbcontainer"></asp:Content>

<asp:Content runat="server" ID="Body" ClientIDMode="Static" ContentPlaceHolderID="BodyContentPlaceHolder">
    <iframe id="ifrmDownload" style="display: none;"></iframe>
    <asp:HiddenField runat="server" ID="hfPosition" Value="" />

    <section novalidate>

        <!-- widget grid -->
        <div id="widget-grid" class="">

            <div class="row">

                <div id="divLeftPane" class="col-sm-12 col-md-12 col-lg-9">

                    <%-- Patient information --%>
                    <div class="row">

                        <article class="col-sm-12 col-md-12 col-lg-12">
                            <div class="jarviswidget" id="wid-id-1" data-widget-editbutton="false" data-widget-custombutton="false" data-widget-deletebutton="false" data-widget-colorbutton="false">
                                <header>
                                    <span class="widget-icon"><i class="fa fa-comments"></i></span>
                                    <h2>Patient Information </h2>
                                </header>

                                <!-- widget div-->
                                <div>
                                    <asp:HiddenField runat="server" ID="hidLastTab" Value="0" />
                                    <!-- widget edit box -->
                                    <div class="jarviswidget-editbox">
                                        <!-- This area used as dropdown edit box -->

                                    </div>
                                    <!-- end widget edit box -->

                                    <!-- widget content -->
                                    <div class="widget-body padding" id="tabs">
                                        <div class="alert alert-success fade in" runat="server" id="divstatus" name="divstatus" visible="false">
                                            <strong>Success</strong> Patient saved successfully!&nbsp&nbsp&nbsp
                                   
                                            <button type="button" class="btn btn-primary" onclick="javascript:window.location = 'PatientSearch.aspx';">Patient Search</button>
                                        </div>
                                        <div class="alert alert-danger alert-block" runat="server" id="diverror" name="diverror" visible="false">
                                            <strong>Error!</strong> Please ensure all details are captured correctly...&nbsp&nbsp&nbsp
                                        </div>
                                        <div class="alert alert-info alert-block" id="divReadonly" runat="server">
                                            <strong>You have read-only access to this patient record as you have not been assigned to this facility...</strong>
                                        </div>

                                        <ul class="nav nav-tabs bordered">
                                            <li id="liDetails" runat="server" class="active">
                                                <a href="#tab1" data-toggle="tab" aria-expanded="true">Details</a>
                                            </li>
                                            <li id="liNotes" runat="server">
                                                <a href="#tab2" data-toggle="tab" aria-expanded="true">Notes</a>
                                            </li>
                                        </ul>
                                        <div class="tab-content">

                                            <div id="tab1" class="tab-pane active smart-form" runat="server">

                                                <fieldset>
                                                    <legend>Basic Information</legend>
                                                    <div class="row">
                                                        <section class="col col-6">
                                                            <label runat="server" id="lblFirstName" class="input">
                                                                <em class="fa fa-asterisk text-danger" style="padding-right: 3px; font-size: 75%; vertical-align: top;"></em>First Name
											           
                                                                <input class="form-control" id="txtFirstName" type="text" maxlength="30" runat="server">
                                                            </label>
                                                        </section>
                                                        <section class="col col-6">
                                                            <label runat="server" id="lblSurname" class="input">
                                                                <em class="fa fa-asterisk text-danger" style="padding-right: 3px; font-size: 75%; vertical-align: top;"></em>Last Name
											           
                                                                <input class="form-control" id="txtSurname" type="text" maxlength="30" runat="server">
                                                            </label>
                                                        </section>
                                                    </div>
                                                    <div class="row">
                                                        <section class="col col-6">
                                                            <label runat="server" id="lblMiddleName" class="input">
                                                                Middle Name
											           
                                                                <input class="form-control" id="txtMiddleName" type="text" maxlength="30" runat="server">
                                                            </label>
                                                        </section>
                                                    </div>
                                                    <div class="row">
                                                        <section class="col col-6">
                                                            <label runat="server" id="lblDOB" class="input">
                                                                <em class="fa fa-asterisk text-danger" style="padding-right: 3px; font-size: 75%; vertical-align: top;"></em>Date of Birth
                                                       
                                                                <input id="txtDOB" type="text" style="color: black" placeholder="yyyy-mm-dd" runat="server" class="form-control datepicker" width="250px">
                                                            </label>
                                                        </section>
                                                        <section class="col col-2">
                                                            <div id="divAge" runat="server" visible="false">
                                                                <label class="input">
                                                                    Age
                                                           
                                                                    <input class="form-control" id="txtAge" type="text" readonly="readonly" style="background-color: lightyellow; text-align: center;" runat="server">
                                                                </label>
                                                            </div>
                                                        </section>
                                                        <section class="col col-4">
                                                            <div id="divAgeGroup" runat="server" visible="false">
                                                                <label class="input">
                                                                    Age Group
                                                           
                                                                    <input class="form-control" id="txtAgeGroup" type="text" readonly="readonly" style="background-color: lightyellow; text-align: center;" runat="server">
                                                                </label>
                                                            </div>
                                                        </section>
                                                    </div>
                                                    <div class="row">
                                                        <section class="col col-6">
                                                            <label runat="server" id="lblFacility" class="input">
                                                                <em class="fa fa-asterisk text-danger" style="padding-right: 3px; font-size: 75%; vertical-align: top;"></em>Facility:
                                                       
                                                                <input class="form-control" id="txtFacility" type="text" readonly="readonly" style="background-color: #EBEBE4;" runat="server">
                                                                <asp:DropDownList ID="ddlFacility" name="ddlFacility" runat="server" Style="color: black" class="form-control">
                                                                </asp:DropDownList>
                                                            </label>
                                                        </section>
                                                        <section class="col col-2">
                                                            <div id="divFacilityEnrolled" runat="server" visible="false">
                                                                <label class="input">
                                                                    Date Entered In System
                                                                    <input class="form-control" id="txtEnrolledDate" type="text" readonly="readonly" style="background-color: lightyellow; text-align: center;" runat="server">
                                                                </label>
                                                            </div>
                                                        </section>
                                                    </div>
                                                    <legend>Patient Demographic Information</legend><br />
                                                    <div class="row">
                                                        <div class="table-responsive">
                                                            <asp:Table ID="dt_1" runat="server" class="table table-striped table-bordered table-hover" Width="100%">
                                                                <asp:TableHeaderRow TableSection="TableHeader">
                                                                    <asp:TableHeaderCell Width="40%"></asp:TableHeaderCell>
                                                                    <asp:TableHeaderCell Width="40%"></asp:TableHeaderCell>
                                                                    <asp:TableHeaderCell Width="20%"></asp:TableHeaderCell>
                                                                </asp:TableHeaderRow>
                                                            </asp:Table>
                                                        </div>
                                                    </div>
                                                </fieldset>

                                            </div>

                                            <div id="tab2" class="tab-pane" basepath="/ckeditor/" runat="server">
                                                <CKEditor:CKEditorControl ID="CKEditor1" runat="server">
                                                </CKEditor:CKEditorControl>
                                            </div>

                                        </div>
                                        <br />
                                        <div class="smart-form">
                                            <footer>
                                                <span id="spnButtons" runat="server"></span>
                                            </footer>
                                        </div>

                                    </div>
                                    <!-- end widget content -->

                                </div>
                                <!-- end widget div -->

                            </div>
                        </article>

                    </div>

                    <%-- initial additional patient widget start--%>
                    <div class="row" runat="server" id="divPatientCondition">

                        <article class="col-sm-12 col-md-12 col-lg-12">

                            <div class="jarviswidget" id="divPatientCondition1"  data-widget-editbutton="false" data-widget-custombutton="false" data-widget-deletebutton="false" data-widget-colorbutton="false">

                                <header>
                                    <span class="widget-icon"><i class="fa fa-folder-open"></i></span>
                                    <h2>Additional Information</h2>
                                </header>

                                <!-- widget div-->
                                <div>

                                    <!-- widget content -->

                                    <div class="tab-content">

                                        <div id="conditionMain1" class="tab-pane active smart-form" runat="server">

                                            <div id="conditionMain2" runat="server">
                                                <fieldset>
                                                    <legend>Primary Condition Group</legend>
                                                    <div class="row">
                                                        <section class="col col-3">
                                                            <label id="lblConditionGroup" runat="server" class="input">
                                                                Condition Groups
                                                                <asp:DropDownList ID="ddlConditions" runat="server" class="form-control" Style="color: black" OnSelectedIndexChanged="ddlConditions_SelectedIndexChanged" AutoPostBack="true"></asp:DropDownList>
                                                            </label>
                                                        </section>
                                                        <section class="col col-3">
                                                            <label id="lblMedDraTerm" runat="server" class="input">
                                                                MedDRA Terms
											                    <asp:DropDownList ID="ddlConditionMedDras" runat="server" class="form-control" Style="color: black"></asp:DropDownList>
                                                            </label>
                                                        </section>
                                                        <section class="col col-3">
                                                            <label id="lblCohort" runat="server" class="input">
                                                                Cohorts
											                    <asp:DropDownList ID="ddlCohort" runat="server" class="form-control" Style="color: black" OnSelectedIndexChanged="ddlCohort_SelectedIndexChanged" AutoPostBack="true"></asp:DropDownList>
                                                            </label>
                                                        </section>
                                                        <section class="col col-3">
                                                            <label id="lblCohortEnrollmentDate" runat="server" class="input">
                                                                Enrollment Date
											                    <input id="txtCohortDateEnrollmentPatientAdd" type="text" style="color: black" placeholder="yyyy-mm-dd" runat="server" class="form-control datepicker">
                                                            </label>
                                                        </section>
                                                    </div>
                                                </fieldset>
                                            </div>

                                            <div id="divConditionDetails" runat="server" visible="false">
                                                <fieldset>
                                                    <div class="row">
                                                        <section class="col col-3">
                                                            <label id="lblConditionStartDate" runat="server" class="input">
                                                                <em class="fa fa-asterisk text-danger" style="padding-right: 3px; font-size: 75%; vertical-align: top;"></em>Start Date
                                                                <input id="txtStartDate" type="text" style="color: black" placeholder="yyyy-mm-dd" runat="server" class="form-control datepicker">
                                                            </label>
                                                        </section>
                                                        <section class="col col-3">
                                                            <label id="lblConditionEndDate" runat="server" class="input">
                                                                Outcome Date
                                                                <input id="txtOutcomeDate" type="text" style="color: black" placeholder="yyyy-mm-dd" runat="server" class="form-control datepicker">
                                                            </label>
                                                        </section>
                                                    </div>
                                                    <div class="row">
                                                        <section class="col col-8">
                                                            <label class="input">
                                                                Comments
                                                                <asp:TextBox ID="txtComments" runat="server" CssClass="form-control" TextMode="MultiLine" Rows="2" MaxLength="250"></asp:TextBox>
                                                            </label>
                                                        </section>
                                                    </div>
                                                    <legend>Condition Information</legend>
                                                    <div class="row">
                                                        <section class="col col-8">
                                                            <div class="table-responsive">
                                                                <asp:Table ID="dtConditionAttributes" runat="server" class="table table-striped table-bordered table-hover" Width="100%">
                                                                    <asp:TableHeaderRow TableSection="TableHeader">
                                                                        <asp:TableHeaderCell Width="60%"></asp:TableHeaderCell>
                                                                        <asp:TableHeaderCell Width="40%"></asp:TableHeaderCell>
                                                                    </asp:TableHeaderRow>
                                                                </asp:Table>
                                                            </div>
                                                        </section>
                                                    </div>
                                                </fieldset>
                                            </div>

                                            <div id="divEncounter">
                                                <fieldset>
                                                    <legend>Encounter Information</legend>
                                                    <div class="row">
                                                        <section class="col col-3">
                                                            <label class="input">
                                                                <em class="fa fa-asterisk text-danger" style="padding-right: 3px; font-size: 75%; vertical-align: top;"></em>Encounter Type
                                                                <asp:DropDownList ID="ddlEncounterType" runat="server" class="form-control" Style="color: black"></asp:DropDownList>
                                                            </label>
                                                        </section>
                                                        <section class="col col-3">
                                                            <label class="input">
                                                                <em class="fa fa-asterisk text-danger" style="padding-right: 3px; font-size: 75%; vertical-align: top;"></em>Priority
                                                                <asp:DropDownList ID="ddlPriority" runat="server" class="form-control" Style="color: black"></asp:DropDownList>
                                                            </label>
                                                        </section>
                                                    </div>
                                                    <div class="row">
                                                        <section class="col col-3">
                                                            <label id="lblEncounterDate" runat="server" class="input">
                                                                <em class="fa fa-asterisk text-danger" style="padding-right: 3px; font-size: 75%; vertical-align: top;"></em>Encounter Date
                                                                <input id="txtEncounterDate" type="text" style="color: black" placeholder="yyyy-mm-dd" runat="server" class="form-control datepicker">
                                                            </label>
                                                        </section>
                                                    </div>
                                                </fieldset>
                                            </div>

                                        </div>

                                    </div>
                                    <br />
                                    <div class="smart-form">
                                        <footer>
                                            <span id="spnAddButtons" runat="server"></span>
                                        </footer>
                                    </div>

                                </div>
                                <!-- end widget content -->

                            </div>

                        </article>

                    </div>

                    <div class="row" runat="server" id="divAdditional">

                        <article class="col-sm-12 col-md-12 col-lg-12">

                            <div class="jarviswidget" id="wid-id-2"  data-widget-editbutton="false" data-widget-custombutton="false" data-widget-deletebutton="false" data-widget-colorbutton="false">
                                <header>
                                    <span class="widget-icon"><i class="fa fa-comments"></i></span>
                                    <h2>Additional Information </h2>
                                </header>

                                <!-- widget div-->
                                <div>

                                    <!-- widget edit box -->
                                    <div class="jarviswidget-editbox">
                                        <!-- This area used as dropdown edit box -->

                                    </div>
                                    <!-- end widget edit box -->

                                    <!-- widget content -->
                                    <div class="widget-body padding" id="PatientViewAdditionalTab">
                                        <ul class="nav nav-tabs bordered">
                                            <li id="liAppointments" runat="server" class="active">
                                                <a href="#tab3" data-toggle="tab" aria-expanded="true">Appointments</a>
                                            </li>
                                            <li id="liAttachments" runat="server">
                                                <a href="#tab4" data-toggle="tab" aria-expanded="true">Attachments</a>
                                            </li>
                                            <li id="liEncounters" runat="server">
                                                <a href="#tab5" data-toggle="tab" aria-expanded="true">Encounters</a>
                                            </li>
                                            <li id="liStatus" runat="server">
                                                <a href="#tab6" data-toggle="tab" aria-expanded="true">Patient Status</a>
                                            </li>
                                            <li id="liCohort" runat="server">
                                                <a href="#tab7" data-toggle="tab" aria-expanded="true">Cohorts</a>
                                            </li>

                                        </ul>
                                        <div class="tab-content">

                                            <div class="alert alert-danger alert-block" runat="server" id="diverror2" name="diverror2" visible="false">
                                                <span id="spnErrors" runat="server"></span>
                                            </div>

                                            <div id="tab3" class="tab-pane active" runat="server">
                                                <br />
                                                <div class="well well-sm bg-color-blueLight txt-color-white text-right">
                                                    <span id="spnAppointmentbuttons" runat="server"></span>
                                                </div>
                                                <div class="table-responsive">
                                                    <asp:Table ID="dt_2" runat="server" class="table table-striped table-bordered table-hover" Width="100%">
                                                        <asp:TableHeaderRow TableSection="TableHeader">
                                                            <asp:TableHeaderCell Width="15%">Date</asp:TableHeaderCell>
                                                            <asp:TableHeaderCell Width="35%">Reason</asp:TableHeaderCell>
                                                            <asp:TableHeaderCell Width="25%">Appointment Outcome</asp:TableHeaderCell>
                                                            <asp:TableHeaderCell Width="30%">Action</asp:TableHeaderCell>
                                                        </asp:TableHeaderRow>
                                                    </asp:Table>
                                                </div>
                                            </div>

                                            <div id="tab4" runat="server" class="tab-pane">
                                                <br />
                                                <div class="well well-sm bg-color-blueLight txt-color-white text-right">
                                                    <div class="row" id="divAttachment" runat="server">
                                                        <div class="smart-form">
                                                            <section class="col col-4">
                                                                <label class="select txt-color-white text-left">
                                                                    Select File...
                                                           
                                                                    <input type="file" class="btn btn-default form-control" id="filAttachment" runat="server">
                                                                </label>
                                                            </section>
                                                            <section class="col col-3">
                                                                <label class="input txt-color-white text-left">
                                                                    File Description
                                                           
                                                                    <input type="text" class="form-control" id="txtFileDescription" name="txtFileDescription" placeholder="Description" runat="server" style="color: black;">
                                                                </label>
                                                            </section>
                                                            <section class="col col-4">
                                                                <br />
                                                                <asp:Button ID="btnAddAttachment" runat="server" Text="Add Attachment" OnClick="btnAddAttachment_Click" class="btn btn-default btn-sm" />
                                                                <%-- <a href="../FileDownload/DownloadPatientAttachment?pid=<%=Request.QueryString["pid"] %>" class="btn btn-default btn-sm">Download All</a>--%>
                                                                <a href="javascript:void(0);" id="hrefDownloadAll" runat="server" class="btn btn-default btn-sm" onclick="showAllDownloadNotification(this);">Download All</a>
                                                            </section>
                                                        </div>
                                                    </div>
                                                </div>
                                                <div class="alert alert-danger alert-block" runat="server" id="divErrorFileType" name="divErrorFileType" visible="false">
                                                    <strong>Error</strong>&nbsp;&nbsp;File must be of type docx | doc | xlsx | xls | pdf | jpg | png | bmp
                                                </div>
                                                <div class="alert alert-danger alert-block" runat="server" id="divErrorSize" name="divErrorSize" visible="false">
                                                    <strong>Error</strong>&nbsp;&nbsp;Size cannot be greater than 500 kB
                                                </div>
                                                <div class="alert alert-danger alert-block" runat="server" id="divErrorDuplicate" name="divErrorDuplicate" visible="false">
                                                    <strong>Error</strong>&nbsp;&nbsp;File with same name already added to this case...
                                                </div>
                                                <div class="alert alert-danger alert-block" runat="server" id="divErrorFileName" name="divErrorFileName" visible="false">
                                                    <strong>Error</strong>&nbsp;&nbsp;File name longer than 50 characters...
                                                </div>

                                                <div class="alert alert-success fade in" runat="server" id="divdownloadstatus" name="divdownloadstatus" hidden="hidden">
                                                    <strong>Success</strong> Attachment downloaded successfully!&nbsp&nbsp&nbsp                                   
                                                </div>

                                                <div class="table-responsive">

                                                    <asp:Table ID="dt_3" runat="server" class="table table-striped table-bordered table-hover" Width="100%">
                                                        <asp:TableHeaderRow TableSection="TableHeader">
                                                            <asp:TableHeaderCell Width="15%">Type</asp:TableHeaderCell>
                                                            <asp:TableHeaderCell Width="20%">Name</asp:TableHeaderCell>
                                                            <asp:TableHeaderCell Width="25%">Description</asp:TableHeaderCell>
                                                            <asp:TableHeaderCell Width="20%">Created By</asp:TableHeaderCell>
                                                            <asp:TableHeaderCell Width="20%">Action</asp:TableHeaderCell>
                                                        </asp:TableHeaderRow>
                                                    </asp:Table>

                                                </div>

                                            </div>

                                            <div id="tab5" class="tab-pane" runat="server">
                                                <br />
                                                <div class="well well-sm bg-color-blueLight txt-color-white text-right">
                                                    <span id="spnEncounterButtons" runat="server"></span>
                                                </div>
                                                <div class="table-responsive">
                                                    <asp:Table ID="dt_4" runat="server" class="table table-striped table-bordered table-hover" Width="100%">
                                                        <asp:TableHeaderRow TableSection="TableHeader">
                                                            <asp:TableHeaderCell Width="20%">Date</asp:TableHeaderCell>
                                                            <asp:TableHeaderCell Width="60%">Type</asp:TableHeaderCell>
                                                            <asp:TableHeaderCell Width="20%">Action</asp:TableHeaderCell>
                                                        </asp:TableHeaderRow>
                                                    </asp:Table>
                                                </div>
                                            </div>

                                            <div id="tab6" runat="server" class="tab-pane">
                                                <br />
                                                <div class="well well-sm bg-color-blueLight txt-color-white text-right">
                                                    <span id="spnStatusButton" runat="server"></span>
                                                </div>
                                                <div class="table-responsive">
                                                    <asp:Table ID="dt_basic_6" runat="server" class="table table-striped table-bordered table-hover" Width="100%">
                                                        <asp:TableHeaderRow TableSection="TableHeader">
                                                            <asp:TableHeaderCell Width="15%">Effective Date</asp:TableHeaderCell>
                                                            <asp:TableHeaderCell Width="15%">Status</asp:TableHeaderCell>
                                                            <asp:TableHeaderCell Width="70%">Created</asp:TableHeaderCell>
                                                        </asp:TableHeaderRow>
                                                    </asp:Table>
                                                </div>
                                            </div>

                                            <div id="tab7" runat="server" class="tab-pane">
                                                <br />
                                                <div class="table-responsive">
                                                    <asp:Table ID="dt_basic_7" runat="server" class="table table-striped table-bordered table-hover" Width="100%">
                                                        <asp:TableHeaderRow TableSection="TableHeader">
                                                            <asp:TableHeaderCell Width="30%">Cohort</asp:TableHeaderCell>
                                                            <asp:TableHeaderCell Width="15%">Cohort Start</asp:TableHeaderCell>
                                                            <asp:TableHeaderCell Width="15%">Enrolled Date</asp:TableHeaderCell>
                                                            <asp:TableHeaderCell Width="20%">De-enrolled&nbsp;Date</asp:TableHeaderCell>
                                                            <asp:TableHeaderCell Width="20%">Action</asp:TableHeaderCell>
                                                        </asp:TableHeaderRow>
                                                    </asp:Table>
                                                </div>
                                            </div>


                                        </div>

                                    </div>

                                    <!-- end widget content -->

                                </div>
                                <!-- end widget div -->

                            </div>

                            <!-- Appointment Modal -->
                            <div class="modal" id="appointmentModal">
                                <div class="modal-dialog">
                                    <div class="modal-content">
                                        <div class="modal-header txt-color-white">
                                            <button type="button" class="close" data-dismiss="modal" aria-label="Close">
                                                <span aria-hidden="true">&times;</span></button>Patient Appointments
							
                                        </div>

                                        <div class="modal-body smart-form">

                                            <fieldset>
                                                <legend>Details</legend>
                                                <input class="form-control" id="txtAppointmentUID" type="hidden" runat="server" value="0">
                                                <div class="row">
                                                    <section class="col col-6">
                                                        <label class="input">
                                                            Appointment Date
                                                  
                                                            <input id="txtAppointmentDate" type="text" onblur="validate()" style="color: black" placeholder="yyyy-mm-dd" runat="server" class="form-control datepicker" width="250px">
                                                            <span id="errorMesDate" class="input state-error" runat="server"></span>
                                                        </label>
                                                    </section>
                                                </div>
                                                <div class="row">
                                                    <section class="col col-8">
                                                        <label class="input">
                                                            Reason
											       
                                                            <textarea class="form-control" id="txtAppointmentReason" maxlength="250" runat="server" rows="3" />
                                                        </label>
                                                    </section>
                                                </div>
                                                <div class="row">
                                                    <section class="col col-6">
                                                        <div id="errors"></div>
                                                    </section>
                                                </div>
                                                <div id="divAppointmentCancellation" style="display: none;">
                                                    <legend>Cancellation</legend>
                                                    <br />
                                                    <div class="row">
                                                        <section class="col col-6">
                                                            <label class="input">
                                                                Cancelled
													
                                                                <select id="ddlAppointmentCancelled" name="ddlAppointmentCancelled" class="input-sm form-control" runat="server">
                                                                    <option value="No" selected="selected">No</option>
                                                                    <option value="Yes">Yes</option>
                                                                </select>
                                                            </label>
                                                        </section>
                                                    </div>
                                                    <div class="row">
                                                        <section class="col col-8">
                                                            <label class="input">
                                                                Reason
											           
                                                                <textarea class="form-control" id="txtAppointmentCancelledReason" maxlength="250" runat="server" rows="3" />
                                                            </label>
                                                        </section>
                                                    </div>
                                                </div>

                                            </fieldset>

                                            <div id="divAppointmentAudit" style="display: none;">
                                                <fieldset>
                                                    <legend>Audit</legend>
                                                    <div class="row">
                                                        <section class="col col-8">
                                                            <label class="input">
                                                                Created...
											           
                                                                <input class="form-control" id="txtAppointmentCreated" type="text" readonly="readonly" runat="server" style="background-color: #EBEBE4;">
                                                            </label>
                                                        </section>
                                                    </div>
                                                    <div class="row">
                                                        <section class="col col-8">
                                                            <label class="input">
                                                                Updated...
											           
                                                                <input class="form-control" id="txtAppointmentUpdated" type="text" readonly="readonly" runat="server" style="background-color: #EBEBE4;">
                                                            </label>
                                                        </section>
                                                    </div>
                                                </fieldset>
                                            </div>
                                        </div>
                                        <div class="modal-footer">
                                            <span id="divAppointmentSave">
                                                <asp:Button ID="btnSaveAppointment" runat="server" Text="Save" OnClick="btnSaveAppointment_Click" class="btn btn-default" />

                                            </span>
                                            <span id="divAppointmentDNA">
                                                <asp:Button ID="btnDNAAppointment" runat="server" Text="Did Not Arrive" OnClick="btnDNAAppointment_Click" class="btn btn-default" />
                                            </span>
                                            <button type="button" class="btn btn-default" data-dismiss="modal" aria-label="Close">Cancel</button>
                                        </div>

                                    </div>
                                    <!-- /.modal-content -->
                                </div>
                                <!-- /.modal-dialog -->
                            </div>
                            <!-- /.modal -->
                            <script>
                                function validate() {

                                    var errors = 0;
                                    var tempdt;
                                    var userdate = document.getElementById("txtAppointmentDate").value
                                    var tempdt = new Date(userdate);
                                    //tempdt = formatDate(tempdt);
                                    //var dateToday = new Date();
                                    //dateToday = formatDate(dateToday);
                                    this.errorMes = document.getElementById("errorMesDate");
                                    var error = "";
                                    var usrYear, usrMonth = tempdt.getMonth() + 1;
                                    var curYear, curMonth = dateToday.getMonth() + 1;
                                    if ((usrYear = tempdt.getFullYear()) < (curYear = dateToday.getFullYear())) {
                                        curMonth += (curYear - usrYear) * 12;
                                    }
                                    var diffMonths = curMonth - usrMonth;
                                    if (parseInt(diffMonths) > 12) {
                                        this.errorMes = document.getElementById("errorMesDate");
                                        var error = "";
                                        error = "Appointment date must be within 12 months from today...\n";
                                        this.errorMes.innerHTML = error;
                                    }

                                    function formatDate(date) {
                                        var d = new Date(date),
                                            month = '' + (d.getMonth() + 1),
                                            day = '' + d.getDate(),
                                            year = d.getFullYear();

                                        if (month.length < 2) month = '0' + month;
                                        if (day.length < 2) day = '0' + day;

                                        return [year, month, day].join('-');
                                    }
                                }

                            </script>
                            <!-- Status Modal -->
                            <div class="modal" id="statusModal">
                                <div class="modal-dialog">
                                    <div class="modal-content">
                                        <div class="modal-header txt-color-white">
                                            <button type="button" class="close" data-dismiss="modal" aria-label="Close">
                                                <span aria-hidden="true">&times;</span></button>Patient Status
							
                                        </div>
                                        <div class="modal-body smart-form">
                                            <fieldset>
                                                <legend>Details</legend>
                                                <input class="form-control" id="txtStatusUID" type="hidden" runat="server" value="0">
                                                <div class="row">
                                                    <section class="col col-6">
                                                        <label class="input">
                                                            Effective Date
                                                   
                                                            <input id="txtStatusDate" type="text" style="color: black" placeholder="yyyy-mm-dd" runat="server" class="form-control datepicker" width="250px">
                                                        </label>
                                                    </section>
                                                </div>
                                                <div class="row">
                                                    <section class="col col-6">
                                                        <label class="input">
                                                            Status
													
                                                            <select id="ddlStatus" name="ddlStatus" class="input-sm form-control" runat="server">
                                                            </select>
                                                        </label>
                                                    </section>
                                                </div>
                                                <div class="row">
                                                    <section class="col col-8">
                                                        <label class="input">
                                                            Comments
											       
                                                            <textarea class="form-control" id="txtStatusDetails" maxlength="250" runat="server" rows="3" />
                                                        </label>
                                                    </section>
                                                </div>
                                            </fieldset>
                                            <div id="divStatusAudit" style="display: none;">
                                                <fieldset>
                                                    <legend>Audit</legend>
                                                    <div class="row">
                                                        <section class="col col-8">
                                                            <label class="input">
                                                                Created...
											           
                                                                <input class="form-control" id="txtStatusCreated" type="text" readonly="readonly" runat="server" style="background-color: #EBEBE4;">
                                                            </label>
                                                        </section>
                                                    </div>
                                                    <div class="row">
                                                        <section class="col col-8">
                                                            <label class="input">
                                                                Updated...
											           
                                                                <input class="form-control" id="txtStatusUpdated" type="text" readonly="readonly" runat="server" style="background-color: #EBEBE4;">
                                                            </label>
                                                        </section>
                                                    </div>
                                                </fieldset>
                                            </div>
                                        </div>
                                        <div class="modal-footer">
                                            <span id="divStatusSave">
                                                <asp:Button ID="btnSaveStatus" runat="server" Text="Save" OnClick="btnSaveStatus_Click" class="btn btn-default" />
                                            </span>
                                            <span id="divStatusDelete">
                                                <asp:Button ID="btnDeleteStatus" runat="server" Text="Delete" OnClick="btnDeleteStatus_Click" class="btn btn-default" />
                                            </span>
                                            <button type="button" class="btn btn-default" data-dismiss="modal" aria-label="Close">Cancel</button>
                                        </div>

                                    </div>
                                    <!-- /.modal-content -->
                                </div>
                                <!-- /.modal-dialog -->
                            </div>
                            <!-- /.modal -->
                    
                            <!-- Cohort Modal -->
                            <div class="modal" id="cohortModal">
                                <div class="modal-dialog">
                                    <div class="modal-content">
                                        <div class="modal-header" style="color:white;">
                                            <button type="button" class="close" data-dismiss="modal" aria-label="Close">
                                                <span aria-hidden="true">&times;</span></button>Cohort Enrollment
							
                                        </div>
                                        <div class="modal-body smart-form">
                                            <div class="alert alert-danger alert-block">
                                                <strong>Please note!</strong> You are about to enroll this patient. Please ensure you use the correct enrollment date as this date cannot be amended once set....
                                            </div>

                                            <fieldset>
                                                <legend>Cohort Details</legend>
                                                <input class="form-control" id="txtCohortUID" type="hidden" runat="server" value="0">
                                                <div class="row">
                                                    <section class="col col-10">
                                                        <label class="input">
                                                            Cohort
                                                   
                                                            <input class="form-control" id="txtCohortName" readonly="readonly" runat="server" style="background-color: #EBEBE4;" type="text" value="">
                                                        </label>
                                                    </section>
                                                </div>
                                                <div class="row">
                                                    <section class="col col-6">
                                                        <label class="input">
                                                            Condition Start Date
                                                   
                                                            <input class="form-control" id="txtConditionStartDate" readonly="readonly" runat="server" style="background-color: #EBEBE4;" type="text" value="">
                                                        </label>
                                                    </section>
                                                </div>
                                                <div class="row">
                                                    <section class="col col-6">
                                                        <label class="input" id="lblCohortDateEnrolment">
                                                            <em class="fa fa-asterisk text-danger" style="padding-right:3px; font-size:75%; vertical-align:top;"></em>Enrollment Date
                                                   
                                                            <input type="text" id="txtCohortDateEnrolment" name="txtCohortDateEnrolment" style="color: black" placeholder="yyyy-mm-dd" runat="server" class="form-control datepicker" width="250px" />
                                                            <div class="note note-error hidden" id="divCohortDateEnrolmentErrorMessage">Cohort Enrollment Date should be entered and before current date and condition start date</div>
                                                        </label>
                                                    </section>
                                                </div>
                                            </fieldset>
                                        </div>
                                        <div class="modal-footer">
                                            <button type="button" id="btnEnrolCohortValidator" class="btn btn-default" onclick="validateCohortEnrol()">Enroll</button>
                                            <asp:Button ID="btnEnrolCohort" runat="server" Text="Enroll" OnClick="btnEnrolCohort_Click" class="btn btn-default hidden" />
                                            <button type="button" class="btn btn-default" data-dismiss="modal" aria-label="Close">Cancel</button>
                                        </div>

                                    </div>
                                    <!-- /.modal-content -->
                                </div>
                                <!-- /.modal-dialog -->
                            </div>

                            <div class="modal" id="cohortModal-de-enrol">
                                <div class="modal-dialog">
                                    <div class="modal-content">
                                        <div class="modal-header" style="color:white;">
                                            <button type="button" class="close" data-dismiss="modal" aria-label="Close">
                                                <span aria-hidden="true">&times;</span></button>Cohort De-enrollment
							
                                        </div>
                                        <div class="modal-body smart-form">
                                            <div class="alert alert-danger alert-block">
                                                <strong>Please note!</strong> You are about to de-enroll this patient. Please ensure you use the correct de-enrollment date as this date cannot be amended once set....
                                            </div>

                                            <fieldset>
                                                <legend>Cohort Details</legend>
                                                <input class="form-control" id="txtCohortUIDDeenrol" type="hidden" runat="server" value="0">
                                                <div class="row">
                                                    <section class="col col-10">
                                                        <label class="input">
                                                            Cohort
                                                   
                                                            <input class="form-control" id="txtCohortNameDeenrolment" readonly="readonly" runat="server" style="background-color: #EBEBE4;" type="text" value="">
                                                        </label>
                                                    </section>
                                                </div>
                                                <div class="row">
                                                    <section class="col col-6">
                                                        <label class="input" >
                                                            Enrollment Date
                                                   
                                                            <input type="text" id="txtCohortDateEnrolmentRead" name="txtCohortDateEnrolmentRead" readonly="readonly" style="background-color: #EBEBE4;" runat="server" />
                                                        </label>
                                                    </section>
                                                </div>
                                                <div class="row">
                                                    <section class="col col-6">
                                                        <label class="input" id="lblCohortDateDeenrolment">
                                                            <em class="fa fa-asterisk text-danger" style="padding-right:3px; font-size:75%; vertical-align:top;"></em>De-enrollment Date
                                                   
                                                            <input type="text" id="txtCohortDateDeenrolment" name="txtCohortDateDeenrolment" style="color: black" placeholder="yyyy-mm-dd" runat="server" class="form-control datepicker" width="250px" />
                                                            <div class="note note-error hidden" id="divCohortDateDeenrolmentErrorMessage">Valid de-enrollment date must be selected</div>
                                                        </label>
                                                    </section>
                                                </div>
                                            </fieldset>
                                        </div>
                                        <div class="modal-footer">
                                            <button type="button" id="btnDeenrolCohortValidator" class="btn btn-default" onclick="validateCohortDeenrol()">De-enroll</button>
                                            <asp:Button ID="btnCohortDeenrol" runat="server" Text="De-enroll" OnClick="btnCohortDeenrol_Click" class="btn btn-default hidden" />
                                            <button type="button" class="btn btn-default" data-dismiss="modal" aria-label="Close">Cancel</button>
                                        </div>

                                    </div>
                                    <!-- /.modal-content -->
                                </div>
                                <!-- /.modal-dialog -->
                            </div>

                            <div class="modal" id="cohortModal-remove">
                                <div class="modal-dialog">
                                    <div class="modal-content">
                                        <div class="modal-header" style="color:white;">
                                            <button type="button" class="close" data-dismiss="modal" aria-label="Close">
                                                <span aria-hidden="true">&times;</span></button>Cohort Enrollment Deletion
							
                                        </div>
                                        <div class="modal-body smart-form">
                                            <fieldset>
                                                <legend>Cohort Details</legend>
                                                <input class="form-control" id="txtCohortUIDRemove" type="hidden" runat="server" value="0">
                                                <div class="row">
                                                    <section class="col col-10">
                                                        <label class="input">
                                                            Cohort
                                                   
                                                            <input class="form-control" id="txtCohortNameRemove" readonly="readonly" runat="server" style="background-color: #EBEBE4;" type="text" value="">
                                                        </label>
                                                    </section>
                                                </div>
                                                <div class="row">
                                                    <section class="col col-8">
                                                        <label class="input" id="lblCohortRemovalReason">
                                                            <em class="fa fa-asterisk text-danger" style="padding-right:3px; font-size:75%; vertical-align:top;"></em>Reason
											       
                                                            <textarea class="form-control" id="txtCohortRemoveReason" maxlength="250" runat="server" rows="3" style="width: 529px;" />
                                                            <div class="note note-error hidden" id="cohortRemovalReasonErrorMessage">Reason is required</div>
                                                        </label>
                                                    </section>
                                                </div>
                                            </fieldset>
                                        </div>
                                        <div class="modal-footer">
                                            <button type="button" id="btnCohortRemoveValidator" class="btn btn-default" onclick="validateTxtCohortRemoveReason()">Delete</button>
                                            <asp:Button ID="btnCohortRemove" runat="server" Text="Remove" OnClick="btnCohortRemove_Click" class="btn btn-default hidden" />
                                            <button type="button" class="btn btn-default" data-dismiss="modal" aria-label="Close">Cancel</button>
                                        </div>

                                    </div>
                                    <!-- /.modal-content -->
                                </div>
                                <!-- /.modal-dialog -->
                            </div>
                        </article>

                    </div>

                    <div class="row" runat="server" id="divClinicalinfo">

                        <article class="col-sm-12 col-md-12 col-lg-12">

                            <div class="jarviswidget" id="idfurtherinfo"  data-widget-editbutton="false" data-widget-custombutton="false" data-widget-deletebutton="false" data-widget-colorbutton="false">

                                <header>
                                    <span class="widget-icon"><i class="fa fa-comments"></i></span>
                                    <h2>Clinical Information </h2>
                                    <div class="widget-toolbar">
                                        <div class="btn-group" id="divAddClinical" runat="server">
                                            <button type="button" class="btn btn-info btn-xs">Add:</button>
                                            <button type="button" class="btn btn-info btn-xs dropdown-toggle" data-toggle="dropdown">
                                                <span class="caret"></span>
                                                <span class="sr-only">Toggle Dropdown</span>
                                            </button>
                                            <ul class="dropdown-menu" role="menu">
                                                <li>

                                                    <a href="/PatientCondition/AddPatientCondition?id=<%=Request.QueryString["pid"]%>">Patient Condition
                                                    </a>
                                                    <a href="/PatientClinicalEvent/AddPatientClinicalEvent?id=<%=Request.QueryString["pid"]%>">Adverse Event
                                                    </a>
                                                    <a href="/PatientMedication/AddPatientMedication?id=<%=Request.QueryString["pid"]%>">Patient Medication
                                                    </a>
                                                    <a href="/PatientLabTest/AddPatientLabTest?id=<%=Request.QueryString["pid"]%>">Tests and Procedures
                                                    </a>
                                                </li>
                                            </ul>
                                        </div>
                                    </div>
                                </header>

                                <%--       provide ability to amend adverse events, medications, conditions and lab tests from patient view-->--%>
                                <div>

                                    <div class="widget-body padding" id="additionalTabs">

                                        <ul class="nav nav-tabs bordered">
                                            <li class="active">
                                                <a href="#patientConditionsTab" data-toggle="tab" aria-expanded="true">Patient Conditions</a>
                                            </li>
                                            <li>
                                                <a href="#patientClinicalEventsTab" data-toggle="tab" aria-expanded="true">Adverse Events</a>
                                            </li>
                                            <li>
                                                <a href="#patientMedicationTab" data-toggle="tab" aria-expanded="true">Patient Medication</a>
                                            </li>
                                            <li>
                                                <a href="#patientLabTestsTab" data-toggle="tab" aria-expanded="true">Tests and Procedures</a>
                                            </li>
                                        </ul>
                                        <div class="tab-content">

                                            <div class="alert alert-danger alert-block" runat="server" id="div15" name="diverror2" visible="false">
                                                <span id="Span7" runat="server"></span>
                                            </div>
                                            <div id="patientConditionsTab" runat="server" class="tab-pane">
                                                <br />
                                                <div class="table-responsive">
                                                    <asp:Table ID="dt_basic_8" runat="server" class="table table-bordered table-striped" Width="100%">
                                                        <asp:TableHeaderRow TableSection="TableHeader">
                                                            <asp:TableHeaderCell Width="35%">Condition Name</asp:TableHeaderCell>
                                                            <asp:TableHeaderCell Width="15%">Start Date</asp:TableHeaderCell>
                                                            <asp:TableHeaderCell Width="15%">Outcome Date</asp:TableHeaderCell>
                                                            <asp:TableHeaderCell Width="20%">Outcome</asp:TableHeaderCell>
                                                            <asp:TableHeaderCell Width="15%">Actions</asp:TableHeaderCell>
                                                        </asp:TableHeaderRow>
                                                    </asp:Table>
                                                </div>
                                            </div>


                                            <div id="patientClinicalEventsTab" runat="server" class="tab-pane">
                                                <br />
                                                <div class="table-responsive">
                                                    <asp:Table ID="dt_basic_9" runat="server" class="table table-striped table-bordered table-hover" Width="100%">
                                                        <asp:TableHeaderRow TableSection="TableHeader">
                                                            <asp:TableHeaderCell Width="35%">Description</asp:TableHeaderCell>
                                                            <asp:TableHeaderCell Width="15%">Onset Date</asp:TableHeaderCell>
                                                            <asp:TableHeaderCell Width="15%">Reported Date</asp:TableHeaderCell>
                                                            <asp:TableHeaderCell Width="15%">Resolution Date</asp:TableHeaderCell>
                                                            <asp:TableHeaderCell Width="10%">Is Serious</asp:TableHeaderCell>
                                                            <asp:TableHeaderCell Width="10%">Actions</asp:TableHeaderCell>
                                                        </asp:TableHeaderRow>
                                                    </asp:Table>
                                                </div>
                                            </div>


                                            <div id="patientMedicationTab" runat="server" class="tab-pane">
                                                <br />
                                                <div class="table-responsive">
                                                    <asp:Table ID="dt_basic_10" runat="server" class="table table-striped table-bordered table-hover" Width="100%">
                                                        <asp:TableHeaderRow TableSection="TableHeader">
                                                            <asp:TableHeaderCell Width="20%">Drug Name</asp:TableHeaderCell>
                                                            <asp:TableHeaderCell Width="10%">Dose</asp:TableHeaderCell>
                                                            <asp:TableHeaderCell Width="10%">Dose Unit</asp:TableHeaderCell>
                                                            <asp:TableHeaderCell Width="10%">Dose Frequency</asp:TableHeaderCell>
                                                            <asp:TableHeaderCell Width="10%">Start Date</asp:TableHeaderCell>
                                                            <asp:TableHeaderCell Width="10%">End Date</asp:TableHeaderCell>
                                                            <asp:TableHeaderCell Width="15%">Indication Type</asp:TableHeaderCell>
                                                            <asp:TableHeaderCell Width="15%">Actions</asp:TableHeaderCell>
                                                        </asp:TableHeaderRow>
                                                    </asp:Table>
                                                </div>
                                            </div>

                                            <div id="patientLabTestsTab" runat="server" class="tab-pane">
                                                <br />
                                                <div class="table-responsive">
                                                    <asp:Table ID="dt_basic_11" runat="server" class="table table-striped table-bordered table-hover" Width="100%">
                                                        <asp:TableHeaderRow TableSection="TableHeader">
                                                            <asp:TableHeaderCell Width="25%">Test</asp:TableHeaderCell>
                                                            <asp:TableHeaderCell Width="10%">Test Date</asp:TableHeaderCell>
                                                            <asp:TableHeaderCell Width="15%">Test Result (Coded)</asp:TableHeaderCell>
                                                            <asp:TableHeaderCell Width="15%">Test Result (Value)</asp:TableHeaderCell>
                                                            <asp:TableHeaderCell Width="7%">Test Unit</asp:TableHeaderCell>
                                                            <asp:TableHeaderCell Width="15%">Range Limits</asp:TableHeaderCell>
                                                            <asp:TableHeaderCell Width="7%">Actions</asp:TableHeaderCell>
                                                        </asp:TableHeaderRow>
                                                    </asp:Table>
                                                </div>
                                            </div>

                                        </div>

                                    </div>

                                    <!-- end widget content -->

                                </div>
                               <%-- <%-- provide ability to amend adverse events, medications, conditions and lab tests from patient view--%>
                                </div>

                        </article>

                    </div>

                    <div class="row" runat="server" id="divIdentifyAudit">

                        <article class="col-sm-12 col-md-12 col-lg-12">

                            <div class="jarviswidget" id="idaudit"  data-widget-editbutton="false" data-widget-custombutton="false" data-widget-deletebutton="false" data-widget-colorbutton="false" data-widget-collapsed="true">

                                <header>
                                    <span class="widget-icon"><i class="fa fa-comments"></i></span>
                                    <h2>Identifiers and Audit information </h2>
                                </header>

                                <!-- widget div-->
                                <div>

                                    <!-- widget content -->
                                    <div class="tab-content">

                                        <div id="auditidentify" class="tab-pane active smart-form" runat="server">
                                            <div id="divIdentifier" runat="server">
                                                <fieldset>
                                                    <legend>Identifiers</legend>
                                                    <div class="row">
                                                        <section class="col col-3">
                                                            <label class="input">
                                                                Unique ID

                                                                    <input class="form-control" id="txtUID" type="text" readonly="readonly" runat="server" style="background-color: #EBEBE4;">
                                                            </label>
                                                        </section>
                                                        <section class="col col-6">
                                                            <label class="input">
                                                                GUID
											               
                                                                    <input class="form-control" id="txtGUID" type="text" readonly="readonly" runat="server" style="background-color: #EBEBE4;">
                                                            </label>
                                                        </section>
                                                    </div>
                                                </fieldset>
                                            </div>

                                            <div id="divAudit" runat="server">
                                                <fieldset>
                                                    <legend>Audit</legend>
                                                    <div class="row">
                                                        <section class="col col-8">
                                                            <label class="input">
                                                                Created...
											               
                                                                    <input class="form-control" id="txtCreated" type="text" readonly="readonly" runat="server" style="background-color: #EBEBE4;">
                                                            </label>
                                                        </section>
                                                    </div>
                                                    <div class="row">
                                                        <section class="col col-8">
                                                            <label class="input">
                                                                Updated...
											               
                                                                    <input class="form-control" id="txtUpdated" type="text" readonly="readonly" runat="server" style="background-color: #EBEBE4;">
                                                            </label>
                                                        </section>
                                                    </div>
                                                </fieldset>
                                            </div>

                                        </div>

                                        <div id="Div9" class="tab-pane" basepath="/ckeditor/" runat="server">
                                            <CKEditor:CKEditorControl ID="CKEditorControl1" runat="server">
                                            </CKEditor:CKEditorControl>
                                        </div>

                                    </div>
                                    <br />
                                    <div class="smart-form">
                                        <footer>
                                            <span id="Span1" runat="server"></span>
                                        </footer>
                                    </div>

                                </div>
                                <!-- end widget content -->
                            </div>

                        </article>

                    </div>

                </div>

                <div id="divRightPane" class="col-sm-12 col-md-12 col-lg-3" runat="server">

                    <div class="row">
                        <div class="well" id="wid-id-0 ">
                            <h5 class="margin-top-0">&nbsp;Condition Groups </h5>
                            <div>
                                <div class="widget-body padding">
                                    <div id="divConditionItems" runat="server">

                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>

                    <div class="row">
                        <div class="well" id="wid-id-1 ">
                            <h5 class="margin-top-0">&nbsp;Analytical Reporting </h5>
                            <div>
                                <div class="widget-body padding">
                                    <div id="divReportingItems" runat="server">

                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>

                </div>

            </div>

        </div>
        <!-- end widget grid -->
  
    </section>
</asp:Content>

<asp:Content runat="server" ID="Scripts" ContentPlaceHolderID="scriptsPlaceholder">
    <script>
        var url = '';
        $('#appointmentModal').on('show.bs.modal', function (e) {
            //get data-id attribute of the clicked element
            var id = $(e.relatedTarget).data('id');
            var evt = $(e.relatedTarget).data('evt');
            var date = $(e.relatedTarget).data('date');
            var reason = $(e.relatedTarget).data('reason');
            var cancelled = $(e.relatedTarget).data('cancelled');
            var dna = $(e.relatedTarget).data('dna');
            var cancelledreason = $(e.relatedTarget).data('cancelledreason');
            var created = $(e.relatedTarget).data('created');
            var updated = $(e.relatedTarget).data('updated');

            //populate modal form
            $('#txtAppointmentUID').val(id);
            $('#txtAppointmentDate').val(date);
            $('#txtAppointmentReason').val(reason);
            if (cancelled != "") {
                $('#ddlAppointmentCancelled').val(cancelled);
            }
            $('#txtAppointmentCancelledReason').val(cancelledreason);
            $('#txtAppointmentCreated').val(created);
            $('#txtAppointmentUpdated').val(updated);

            if (evt == "add") {
                $('#divAppointmentAudit').hide();
                $('#divAppointmentCancellation').hide();
                $('#divAppointmentDelete').hide();
                $('#divAppointmentDNA').hide();
            }
            if (evt == "edit") {
                $('#divAppointmentAudit').show();
                $('#divAppointmentCancellation').show();
                $('#divAppointmentDelete').hide();
                $('#divAppointmentDNA').hide();

                if (dna == "Yes") {
                    $('#txtAppointmentDate').attr("disabled", true);
                    $('#txtAppointmentReason').attr("disabled", true);
                    $('#ddlAppointmentCancelled').attr("disabled", true);
                    $('#txtAppointmentCancelledReason').attr("disabled", true);
                    $('#divAppointmentSave').hide();
                }
            }
            if (evt == "dna") {
                $('#divAppointmentAudit').show();
                $('#divAppointmentCancellation').show();

                $('#txtAppointmentDate').attr("disabled", true);
                $('#txtAppointmentReason').attr("disabled", true);
                $('#ddlAppointmentCancelled').attr("disabled", true);
                $('#txtAppointmentCancelledReason').attr("disabled", true);

                $('#divAppointmentSave').hide();
                $('#divAppointmentDelete').hide();
                $('#divAppointmentDNA').show();
            }
        });

        $('#statusModal').on('show.bs.modal', function (e) {
            //get data-id attribute of the clicked element
            var id = $(e.relatedTarget).data('id');
            var evt = $(e.relatedTarget).data('evt');
            var date = $(e.relatedTarget).data('date');
            var details = $(e.relatedTarget).data('details');
            var status = $(e.relatedTarget).data('status');
            var created = $(e.relatedTarget).data('created');
            var updated = $(e.relatedTarget).data('updated');

            //populate modal form
            $('#txtStatusUID').val(id);
            $('#txtStatusDate').val(date);
            $('#ddlStatus').val(status);
            $('#txtStatusDetails').val(details);
            $('#txtStatusCreated').val(created);
            $('#txtStatusUpdated').val(updated);

            if (evt == "add") {
                $('#divStatusAudit').hide();
                $('#divStatusDelete').hide();

            }
            if (evt == "edit") {
                $('#divStatusAudit').show();
                $('#divStatusDelete').hide();

            }
        });

        $('#cohortModal').on('show.bs.modal', function (e) {
            //get data-id attribute of the clicked element
            var id = $(e.relatedTarget).data('id');
            var cohort = $(e.relatedTarget).data('cohort');
            var conditionStartDate = $(e.relatedTarget).data('conditionstartdate');

            //populate modal form
            $('#txtCohortUID').val(id);
            $('#txtConditionStartDate').val(conditionStartDate);
            $('#txtCohortName').val(cohort);
        });

        $('#cohortModal-de-enrol').on('show.bs.modal', function (e) {
            //get data-id attribute of the clicked element
            var id = $(e.relatedTarget).data('id');
            var cohort = $(e.relatedTarget).data('cohort');
            var enrolmentdate = $(e.relatedTarget).data('startdate');

            //populate modal form
            $('#txtCohortUIDDeenrol').val(id);
            $('#txtCohortNameDeenrolment').val(cohort);
            $('#txtCohortDateEnrolmentRead').val(enrolmentdate);
        });

        $('#cohortModal-remove').on('show.bs.modal', function (e) {
            //get data-id attribute of the clicked element
            var id = $(e.relatedTarget).data('id');
            var cohort = $(e.relatedTarget).data('cohort');

            //populate modal form
            $('#txtCohortUIDRemove').val(id);
            $('#txtCohortNameRemove').val(cohort);
        });

    </script>

   <script type="text/javascript">

       function showAllDownloadNotification(sender) {
           $('#divdownloadstatus').hide();

           var downloadUri = '../FileDownload/DownloadPatientAttachment?pid=' + sender.dataset.pid;

           $.ajax({
               url: downloadUri,
               type: 'GET',
               success: function (data) {
                   var downloadMessage = "<strong>Success</strong> Attachment(s) downloaded successfully!&nbsp&nbsp&nbsp";
                   document.getElementById('ifrmDownload').src = downloadUri;
                   $('#divdownloadstatus').html(downloadMessage);
                   $('#divdownloadstatus').show();
               },
               error: function () {
                   //console.log("Download error");
               }
           });
       }

       function showDownloadNotification(sender) {
           $('#divdownloadstatus').hide();

           var downloadUri = '../FileDownload/DownloadSingleAttachment?attid=' + sender.dataset.attachmentid;

           $.ajax({
               url: downloadUri,
               type: 'GET',
               success: function (data) {
                   var downloadMessage = "<strong>Success</strong> Attachment (" + sender.dataset.filename + ") downloaded successfully!&nbsp&nbsp&nbsp";
                   document.getElementById('ifrmDownload').src = downloadUri;
                   $('#divdownloadstatus').html(downloadMessage);
                   $('#divdownloadstatus').show();
               },
               error: function () {
                   //console.log("Download error");
               }
           });
       }

       function validateTxtCohortRemoveReason() {
           var isSuccessful = ValidateDeletionReason('#txtCohortRemoveReason', '#cohortRemovalReasonErrorMessage', '#lblDeletionReason');
           if (isSuccessful) {
               $('#btnCohortRemove').click();
           }
       }

       $(function () {

           //maintaining selected tab during postback   
           var selectedIndex = $("#<%=hidLastTab.ClientID %>").val();
           console.log("Selected index is " + selectedIndex);
           if (selectedIndex == "") {
               console.log("Retrieved index  for Patient Informamtion  " + localStorage.getItem("PatientTab"))
               var selectedIndex = localStorage.getItem("PatientTab"); //get it from local storage item
               $("#tabs").tabs({ active: selectedIndex });


           }
           else {

               $("#tabs").tabs({ active: selectedIndex }); // be default it will just go to the first tab

           }

           //Change the text of the button on tab change   
           $("#tabs").on("tabsactivate", function () {
               var index = $("#tabs").tabs("option", "active");
               $("#<%=hidLastTab.ClientID %>").val(index); //setting the hiddenfield value
               var anchor = $("#<%=hidLastTab.ClientID %>").val(); // current tab value
               localStorage.clear("PatientTab"); //clear storage
               console.log("Hidden value set  current tab index  for Patient Information is   " + anchor + " and is now being set to local storage");
               localStorage.setItem("PatientTab", anchor); //store reference specific to that tab
           });


       });

   </script>

   <script type="text/javascript">
        $(function () {

            //maintaining selected tab during postback for additional information
            //Change the text of the button on tab change
            $('#PatientViewAdditionalTab').on("tabsactivate", function () {
                var index = $("#PatientViewAdditionalTab").tabs("option", "active");
                console.log("Selected tab index for additional information tab is   " + index + " and has been  set to local storage");
                localStorage.setItem("ViewPatientAdditionalTab", index); //store reference specific to that tab
            })
        });

    </script>

   <script>

        $(function () {
            $('#PatientViewAdditionalTab').tabs();
            console.log("Retrieved index for Patient Additional Information  " + localStorage.getItem("ViewPatientAdditionalTab"))
            var selectedIndex = localStorage.getItem("ViewPatientAdditionalTab"); //get it from local storage item
            console.log("Activating Patient Additional Information tab : " + selectedIndex)
            $("#PatientViewAdditionalTab").tabs({ active: selectedIndex });

        });

    </script>

   <script type="text/javascript">

       $(function () {
            $("#tabs").tabs();
            $('#tabs').click('tabsselect', function (event, ui) {
                var active = $("#tabs").tabs("option", "active"); // retrieve zero based  index of tab
                console.log("Before verification " + active);
                console.log("Extract ref " + $("#tabs ul > li a").eq(active).attr('href'));
                localStorage.setItem("PatientNotesTab", $("#tabs ul > li a").eq(active).attr('href')); //store reference specific to that tab

            });


        });
    </script>

   <script type="text/javascript">

       $(function () {
            //maintaining selected tab during postback for additional information
            //Change the text of the button on tab change
            $('#additionalTabs').click('tabsselect', function (event, ui) {
                var index = $("#additionalTabs").tabs("option", "active");
                console.log("Selected tab index for additional information tab is   " + index + " and has been  set to local storage");
                localStorage.setItem("PatientAdditionalTab", index); //store reference specific to that tab
            });
        });

        $(function () {
            $('#additionalTabs').tabs();
            console.log("Retrieved index from local Storage since current selected index for additional tab has been is  " + localStorage.getItem("PatientAdditionalTab"));
            var selectedIndex = localStorage.getItem("PatientAdditionalTab"); //get it from local storage item
            console.log("Activating tab : " + selectedIndex);
            $("#additionalTabs").tabs({ active: selectedIndex });

        });

        $(function () {
            $('#additionalTabs').tabs();
            console.log("Retrieved index from local Storage since current selected index for additional tab has been is  " + localStorage.getItem("PatientAdditionalTab"));
            var selectedIndex = localStorage.getItem("PatientAdditionalTab"); //get it from local storage item
            console.log("Activating tab : " + selectedIndex);
            $("#additionalTabs").tabs({ active: selectedIndex });
        });

        function validateCohortEnrol() {
            var enrolmentDateValue = $.trim($(txtCohortDateEnrolment).val());
            var conditionStartDateValue = $.trim($(txtConditionStartDate).val());

            if (enrolmentDateValue == "") {
                $('#divCohortDateEnrolmentErrorMessage').removeClass('hidden');
                $('#divCohortDateEnrolmentErrorMessage').addClass('show');
                $('#lblCohortDateEnrolment').addClass('state-error');
            }
            else
            {
                if (new Date(enrolmentDateValue) > new Date()) {
                    $('#divCohortDateEnrolmentErrorMessage').removeClass('hidden');
                    $('#divCohortDateEnrolmentErrorMessage').addClass('show');
                    $('#lblCohortDateEnrolment').addClass('state-error');
                }
                else
                {
                    if (new Date(enrolmentDateValue) < new Date(conditionStartDateValue)) {
                        $('#divCohortDateEnrolmentErrorMessage').removeClass('hidden');
                        $('#divCohortDateEnrolmentErrorMessage').addClass('show');
                        $('#lblCohortDateEnrolment').addClass('state-error');
                    }
                    else
                    {
                        $('#lblCohortDateEnrolment').removeClass('state-error');
                        $('#divCohortDateEnrolmentErrorMessage').removeClass('show');
                        $('#divCohortDateEnrolmentErrorMessage').addClass('hidden');
                        $('#btnEnrolCohort').click();
                    }
                }
            }
        }

        function validateCohortDeenrol() {
            var enrolmentDateValue = $.trim($(txtCohortDateEnrolmentRead).val());
            var deenrolmentDateValue = $.trim($(txtCohortDateDeenrolment).val());

            if (deenrolmentDateValue == "") {
                $('#divCohortDateDeenrolmentErrorMessage').removeClass('hidden');
                $('#divCohortDateDeenrolmentErrorMessage').addClass('show');
                $('#lblCohortDateDeenrolment').addClass('state-error');
            }
            else {
                if (new Date(deenrolmentDateValue) > new Date() || new Date(deenrolmentDateValue) < new Date(enrolmentDateValue)) {
                    $('#divCohortDateDeenrolmentErrorMessage').removeClass('hidden');
                    $('#divCohortDateDeenrolmentErrorMessage').addClass('show');
                    $('#lblCohortDateDeenrolment').addClass('state-error');
                }
                else {
                    $('#lblCohortDateDeenrolment').removeClass('state-error');
                    $('#divCohortDateDeenrolmentErrorMessage').removeClass('show');
                    $('#divCohortDateDeenrolmentErrorMessage').addClass('hidden');
                    $('#btnCohortDeenrol').click();
                }
            }
        }

    </script>

    <script type="text/javascript">

        $(function () {
            var f = $("#<%=hfPosition.ClientID%>");
            window.onload = function () {
                var position = parseInt(f.val());
                if (!isNaN(position)) {
                    $(window).scrollTop(position);
                }
            };
            window.onscroll = function () {
                var position = $(window).scrollTop();
                f.val(position);
            };
        });

    </script>
                   
</asp:Content>

<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Analyser.aspx.cs" Inherits="PVIMS.Web.Analyser" MasterPageFile="~/Main.Master" Title="Analyser" ViewStateMode="Enabled" %>

<%@ Register Assembly="System.Web.DataVisualization, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" Namespace="System.Web.UI.DataVisualization.Charting" TagPrefix="asp" %>
<%@ Register Assembly="CKEditor.NET" Namespace="CKEditor.NET" TagPrefix="CKEditor" %>

<asp:Content runat="server" ID="Breadcrumbcontainer" ContentPlaceHolderID="breadcrumbcontainer"></asp:Content>

<asp:Content runat="server" ID="Body" ClientIDMode="Static" ContentPlaceHolderID="BodyContentPlaceHolder">

    <style>

        .Waiting {
          display: block;
          visibility: visible;
          position: absolute;
          z-index: 999;
          top: 0px;
          left: 0px;
          width: 105%;
          height: 105%;
          background-color:white;
          vertical-align:bottom;
          padding-top: 20%;
          filter: alpha(opacity=75);
          opacity: 0.75;
          font-size:large;
          color:blue;
          font-style:italic;
          font-weight:400;
          background-image: url("/img/loading-red.gif");
          background-repeat: no-repeat;
          background-attachment: fixed;
          background-position: center;
          }

    </style>

    <asp:HiddenField runat="server" ID="hfPosition" Value="" />

    <section novalidate>
        
		<!-- widget grid -->
		<div id="widget-grid" class="">

			<div class="row">
					
				<div class="col-sm-12 col-md-12 col-lg-6">

                    <div class="jarviswidget" id="wid-id-1"  data-widget-editbutton="false" data-widget-custombutton="false" data-widget-deletebutton="false" data-widget-colorbutton="false">
						<header>
							<h2>Analysis Criteria for Active Reporting  </h2>
						</header>
								
						<!-- widget div-->
						<div>
									
							<!-- widget edit box -->
							<div class="jarviswidget-editbox">
								<!-- This area used as dropdown edit box -->
										
							</div>
							<!-- end widget edit box -->
									
							<!-- widget content -->
							<div class="widget-body padding" id="tabs">
                                <div class="alert alert-success fade in" runat="server" id="divstatus" name="divstatus" visible="false">
                                    <strong>Success</strong> Patient saved successfully!&nbsp&nbsp&nbsp
                                    <button type="button" class="btn btn-primary" onclick="javascript:window.location = 'PatientSearch.aspx';">Search For Patients</button>
                                </div>		
								
								<ul class="nav nav-tabs bordered">
									<li id="liCondition" runat="server" class="active">
										<a href="#tab0" data-toggle="tab" aria-expanded="true">Patient Population</a>
									</li>
									<li id="liCriteriaRange" runat="server">
										<a href="#tab1" data-toggle="tab" aria-expanded="true">Date Range</a>
									</li>
									<li id="liRiskFactors" runat="server">
										<a href="#tab2" data-toggle="tab" aria-expanded="true">Risk Factors</a>
									</li>
								</ul>

                                <div class="tab-content">

                                    <div class="well bg-color-grayDark txt-color-white text-center">
                                        To determine the correct population for the analysis, select either a cohort OR primary condition group risk factor.
                                    </div>
                                    <div id="tab0" class="tab-pane active smart-form"  runat="server">

                                        <div id="divCondition" runat="server">
								            <fieldset>
									            <div class="row">
                                                    <section class="col col-6">
										                <label runat="server" id="lblCondition" class="input">Primary Condition Group Risk Factor
				                                            <asp:DropDownList ID="ddlCondition" name="ddlCondition" runat="server" Style="color: black" CssClass="form-control">
                                                                <asp:ListItem Selected="True" Text="" Value="0"></asp:ListItem>
				                                            </asp:DropDownList>
                                                        </label>
                                                    </section>
									            </div>
									            <div class="row">
                                                    <section class="col col-6">
										                <label runat="server" id="Label4" class="input"><b>-- OR --</b>
                                                        </label>
                                                    </section>
									            </div>
									            <div class="row">
                                                    <section class="col col-6">
										                <label runat="server" id="lblCohort" class="input">Cohort
				                                            <asp:DropDownList ID="ddlCohort" name="ddlCohort" runat="server" Style="color: black" CssClass="form-control">
                                                                <asp:ListItem Selected="True" Text="" Value="0"></asp:ListItem>
				                                            </asp:DropDownList>
                                                        </label>
                                                    </section>
									            </div>
									            <div class="row" style="padding:10px;" id="divCohort">
                                                    <section class="col col-6">
                                                        <span id="spnCohort" id="spnCohort">
                                                        </span>
                                                    </section>
									            </div>
								            </fieldset>
                                        </div>
                                    </div>

                                    <div id="tab1" class="tab-pane smart-form"  runat="server">
                                        <div id="divIdentifier" runat="server">
								            <fieldset>
									            <div class="row">
                                                    <section class="col col-6">
										                <label runat="server" id="lblSearchFrom" class="input">Search From
											                <input id="txtSearchFrom" type="text" style="color: black" placeholder="yyyy-mm-dd" runat="server" class="form-control datepicker" width="250px">
                                                        </label>
                                                    </section>
                                                    <section class="col col-6">
										                <label runat="server" id="lblSearchTo" class="input">Search To
											                <input id="txtSearchTo" type="text" style="color: black" placeholder="yyyy-mm-dd" runat="server" class="form-control datepicker" width="250px">
                                                        </label>
                                                    </section>
									            </div>
								            </fieldset>
                                        </div>
                                    </div>

                                    <div id="tab2" class="tab-pane" runat="server">
							            <div class="well well-sm bg-color-blueLight txt-color-white text-left">
								            <div class="row">
                                                <div class="smart-form">
                                                    <section class="col col-5">
										                <label class="select txt-color-white">Risk Factor</label>
				                                        <asp:DropDownList ID="ddlRiskFactor" name="ddlRiskFactor" runat="server" Style="color: black" CssClass="form-control" OnSelectedIndexChanged="ddlRiskFactor_SelectedIndexChanged" AutoPostBack="true">
					                                        <asp:ListItem Value="0" Selected="True">-- Please select a factor --</asp:ListItem>
				                                        </asp:DropDownList>
                                                    </section>
                                                    <section class="col col-5">
										                <label class="select txt-color-white">Option</label>
				                                        <asp:DropDownList ID="ddlFactorOption" name="ddlFactorOption" runat="server" Style="color: black" CssClass="form-control">
					                                        <asp:ListItem Value="0" Selected="True">-- Please select an option --</asp:ListItem>
				                                        </asp:DropDownList>
                                                    </section>
                                                </div>
                                                <section class="col col-2">
										            <label class="input txt-color-white"><br />
                                                        &nbsp;&nbsp;&nbsp;
                                                        <asp:Button ID="btnAdd" runat="server" Text="Add Factor" OnClick="btnAddFactor_Click" class="btn btn-default" />
                                                        &nbsp;&nbsp;&nbsp;
                                                        <span class="label label-warning" id="spnError" runat="server" visible="false"></span>
                                                    </label>
                                                </section>
								            </div>
							            </div>
                                        <asp:Table id="dt_2" runat="server" class="table table-striped table-bordered table-hover"  width="100%">
                                            <asp:TableHeaderRow TableSection="TableHeader">
                                                <asp:TableHeaderCell Width="50%">Factor</asp:TableHeaderCell> 
                                                <asp:TableHeaderCell Width="30%">Condition</asp:TableHeaderCell> 
                                                <asp:TableHeaderCell Width="20%">Remove</asp:TableHeaderCell> 
                                            </asp:TableHeaderRow>
                                        </asp:Table>
                                    </div>

                                </div>
                                <br />
                                <div class="smart-form">
								    <footer>
                                        <asp:Button ID="btnAnalyse" runat="server" Text="Analyse" OnClick="btnAnalyse_Click" class="btn btn-primary" />
								    </footer>
                                    <div class="alert">
									    <label runat="server" id="lblCustomError" class="input">
                                        </label>
                                    </div>
                                </div>

                                <legend>Results</legend>
                                <div class="alert alert-success alert-block" runat="server" id="divRows" name="divRows" visible="false">
                                    <span id="spnRows" runat="server"></span>
                                </div>

                                <div id="divTerms" runat="server" class="smart-form" visible="false">
									<div class="row">
                                        <section class="col col-6">
				                            <asp:DropDownList ID="ddlReactions" name="ddlReactions" runat="server" Style="color: black" CssClass="form-control" OnSelectedIndexChanged="ddlReactions_SelectedIndexChanged" AutoPostBack="true">
				                            </asp:DropDownList>
                                        </section>
									</div>
                                </div>

                                <legend>Legend</legend>
                                <div class="well well-sm" style="height:auto; padding-top:10px; padding-left:10px; padding-right:10px; padding-bottom:0px;">
                                    <div class="row">
                                        <div class="smart-form">
                                            <section class="col col-2">
                                                <span class="label" style="padding:4px; background-color:lightgray;"> <b> Exposed </b> </span>
                                            </section>
                                            <section class="col col-4">
                                                <label runat="server" id="Label5" class="input">
                                                    Number of patients in the target population treated or having been treated with the medicine of interest during the reporting period
                                                </label>
                                            </section>
                                            <section class="col col-2">
                                                <span class="label" style="padding:4px; background-color:lightgray;"> <b> Unexposed </b> </span>
                                            </section>
                                            <section class="col col-4">
                                                <label runat="server" id="Label6" class="input">
                                                    Number of patients in the target population who have not been treated with the medicine of interest during the reporting period
                                                </label>
                                            </section>
                                        </div>
                                    </div>
                                    <div class="row">
                                        <div class="smart-form">
                                            <section class="col col-2">
                                                <span class="label" style="padding:4px; background-color:lightgray;"> <b> Cases </b> </span>
                                            </section>
                                            <section class="col col-4">
                                                <label runat="server" id="Label7" class="input">
                                                    Number of patients in the target population who did experience the adverse event of interest during the reporting period
                                                </label>
                                            </section>
                                            <section class="col col-2">
                                                <span class="label" style="padding:4px; background-color:lightgray;"> <b> Non-Cases </b> </span>
                                            </section>
                                            <section class="col col-4">
                                                <label runat="server" id="Label8" class="input">
                                                    Number of patients in the target population who did not experience the adverse event of interest during the reporting period 
                                                </label>
                                            </section>
                                        </div>
                                    </div>
                                    <div class="row">
                                        <div class="smart-form">
                                            <section class="col col-2">
                                                <span class="label" style="padding:4px; background-color:lightgray;"> <b> Population </b> </span>
                                            </section>
                                            <section class="col col-4">
                                                <label runat="server" id="Label1" class="input">
                                                    Calculated in patient years over analysis period
                                                </label>
                                            </section>
                                            <section class="col col-2">
                                                <span class="label" style="padding:4px; background-color:lightgray;"> <b> IR </b> </span>
                                            </section>
                                            <section class="col col-4">
                                                <label runat="server" id="lblIncidenceRate" class="input">
                                                    Incidence Rate per Thousand Person-Years: ((Number of ADRs) / (Cohort Population * Study Duration in Years)) * 1000
                                                </label>
                                            </section>
                                        </div>
                                    </div>
                                    <div class="row">
                                        <div class="smart-form">
                                            <section class="col col-2">
                                                <span class="label" style="padding:4px; background-color:lightgray;"> <b> Unadj. RR </b> </span>
                                            </section>
                                            <section class="col col-4">
                                                <label runat="server" id="Label3" class="input">
                                                    Unadjusted Relative Risk - Incidence in exposed/incidence in non-exposed with no risk factor adjustment
                                                </label>
                                            </section>
                                            <section class="col col-2">
                                                <span class="label" style="padding:4px; background-color:lightgray;"> <b> Adj. RR </b> </span>
                                            </section>
                                            <section class="col col-4">
                                                <label runat="server" id="Label9" class="input">
                                                    Relative Risk- Incidence in exposed/incidence in non-exposed with risk factor adjustment
                                                </label>
                                            </section>
                                        </div>
                                    </div>
                                </div>
                                <div class="well bg-color-greenSuccess txt-color-greenDark text-center">
                                    PLEASE NOTE: Exposed Cases ONLY include cases where the Adverse Drug Reaction falls within the start and end date of the medication.
                                </div>
                                <div id="divDownload" runat="server" visible="false">
                                    <div class="well bg-color-grayDark txt-color-white text-center">
                                        For third party statistical analysis, to download all patient related data click the button below.
                                    </div>
                                    <div class="smart-form">
								        <footer>
                                            <a href="javascript:void(0);" class="btn btn-primary" id="modal-download">Download Dataset</a>
								        </footer>
                                    </div>
                                </div>
							</div>
							<!-- end widget content -->
									
						</div>
						<!-- end widget div -->
								
					</div>

				</div>

                <div class="col-sm-12 col-md-12 col-lg-6">

                    <div class="jarviswidget" id="wid-id-graph-1"  data-widget-editbutton="false" data-widget-custombutton="false" data-widget-deletebutton="false" data-widget-colorbutton="false">

						<div class="well row" style="height:1100px;">
                            <div> 

								<ul class="nav nav-tabs">
									<li class="active">
										<a href="#tabct0" data-toggle="tab" aria-expanded="true">Adverse Events</a>
									</li>
									<li>
										<a href="#tabct1" data-toggle="tab" aria-expanded="true">Relative Risk</a>
									</li>
								</ul>

                                <div class="tab-content">

                                    <div id="tabct0" class="tab-pane active smart-form"  runat="server">
                                        <br />
                                        <div id="divChart1Empty" class="tab-pane active"  runat="server">
                                            <asp:Image runat="server" AlternateText="No Data" ImageUrl="~/img/NoDataGraph.bmp" />
                                        </div>

                                        <div id="divChart1" class="tab-pane"  runat="server">
                                            <asp:Chart ID="chtAdverse" runat="server" ImageStorageMode="UseImageLocation" Palette="BrightPastel" Width="650" Height="650" BackColor="#D3DFF0" BorderlineDashStyle="Solid" BackGradientStyle="TopBottom" BorderWidth="2" BorderColor="181, 64, 1"> 
                                                <titles> 
                                                    <asp:Title ShadowColor="32, 0, 0, 0" Font="Trebuchet MS, 14.25pt, style=Bold" ShadowOffset="3" Text="Adverse Event Count Per Drug" Name="Title1" ForeColor="#000000"></asp:Title> 
                                                </titles>
                                                <legends> 
                                                    <asp:Legend TitleFont="Microsoft Sans Serif, 8pt, style=Bold" BackColor="Transparent" Font="Trebuchet MS, 8.25pt, style=Bold" IsTextAutoFit="False" Enabled="False" Name="Default"></asp:Legend> 
                                                </legends> 
                                                <borderskin SkinStyle="Emboss"></borderskin>
                                                <Series> 
                                                    <asp:Series Name="AdverseEvents" YValueType="Double" ChartType="Column" ChartArea="MainChartArea" BorderColor="180, 26, 59, 105">  
                                                        <Points> 
                                                        </Points> 
                                                    </asp:Series> 
                                                </Series> 
                                                <ChartAreas> 
                                                    <asp:ChartArea Name="MainChartArea" BorderColor="64, 64, 64, 64" BackSecondaryColor="White" BackColor="#D3DFF0" ShadowColor="Transparent" BackGradientStyle="TopBottom"> 
									                    <axisy LineColor="64, 64, 64, 64" Title="Number Events" TitleFont="Microsoft Sans Serif, 12pt, style=Bold">
										                    <LabelStyle Font="Trebuchet MS, 10.25pt" />
										                    <MajorGrid LineColor="64, 64, 64, 64" />
									                    </axisy>
									                    <axisx LineColor="64, 64, 64, 64" Title="Drug Name" TitleFont="Microsoft Sans Serif, 12pt, style=Bold">
										                    <LabelStyle Font="Trebuchet MS, 10.25pt" IsEndLabelVisible="False" />
										                    <MajorGrid LineColor="64, 64, 64, 64" />
									                    </axisx>
                                                    </asp:ChartArea> 
                                                </ChartAreas> 
                                            </asp:Chart>
                                        </div>
                                    </div>

                                    <div id="tabct1" class="tab-pane">
                                        <br />
                                        <div id="divChart2Empty" class="tab-pane active"  runat="server">
                                            <asp:Image runat="server" AlternateText="No Data" ImageUrl="~/img/NoDataGraph.bmp" />
                                            <br /><br /><br /><br />
                                        </div>

                                        <div id="divChart2" class="tab-pane" runat="server">
                                            <asp:Chart ID="chtRelative" runat="server" ImageStorageMode="UseImageLocation" Palette="BrightPastel" Width="650" Height="650" BackColor="#D3DFF0" BorderlineDashStyle="Solid" BackGradientStyle="TopBottom" BorderWidth="2" BorderColor="181, 64, 1"> 
                                                <titles> 
                                                    <asp:Title ShadowColor="32, 0, 0, 0" Font="Trebuchet MS, 14.25pt, style=Bold" ShadowOffset="3" Text="Relative Risk Per Drug" Name="Title1" ForeColor="#000000"></asp:Title> 
                                                </titles>
                                                <legends> 
                                                    <asp:Legend TitleFont="Microsoft Sans Serif, 8pt, style=Bold" BackColor="Transparent" Font="Trebuchet MS, 8.25pt, style=Bold" IsTextAutoFit="False" Enabled="False" Name="Default"></asp:Legend> 
                                                </legends> 
                                                <borderskin SkinStyle="Emboss"></borderskin>
                                                <Series> 
                                                    <asp:Series Name="RelativeRisks" YValueType="Double" ChartType="Column" ChartArea="MainChartArea" BorderColor="180, 26, 59, 105">  
                                                        <Points> 
                                                        </Points> 
                                                    </asp:Series> 
                                                </Series> 
                                                <ChartAreas> 
                                                    <asp:ChartArea Name="MainChartArea" BorderColor="64, 64, 64, 64" BackSecondaryColor="White" BackColor="#D3DFF0" ShadowColor="Transparent" BackGradientStyle="TopBottom"> 
									                    <axisy LineColor="64, 64, 64, 64" Title="Relative Risk" TitleFont="Microsoft Sans Serif, 12pt, style=Bold">
										                    <LabelStyle Font="Trebuchet MS, 10.25pt" />
										                    <MajorGrid LineColor="64, 64, 64, 64" />
									                    </axisy>
									                    <axisx LineColor="64, 64, 64, 64" Title="Drug Name" TitleFont="Microsoft Sans Serif, 12pt, style=Bold">
										                    <LabelStyle Font="Trebuchet MS, 10.25pt" IsEndLabelVisible="False" />
										                    <MajorGrid LineColor="64, 64, 64, 64" />
									                    </axisx>
                                                    </asp:ChartArea> 
                                                </ChartAreas> 
                                            </asp:Chart>

                                        </div>
                                    </div>

                               </div>

                            </div>
						</div>
					</div>

				</div>

			</div>
			<!-- end row -->

			<div class="row">
					
				<div class="col-sm-12 col-md-12 col-lg-12">

                    <div class="jarviswidget" id="wid-id-5"  data-widget-editbutton="false" data-widget-custombutton="false" data-widget-deletebutton="false" data-widget-colorbutton="false">
						<header>
							<h2>Results</h2>
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

                                <asp:Table id="dt_1" runat="server" class="table table-striped table-bordered table-hover"  width="100%">
                                    <asp:TableHeaderRow TableSection="TableHeader">
                                        <asp:TableHeaderCell Width="14%"></asp:TableHeaderCell> 
                                        <asp:TableHeaderCell ColumnSpan="4">Exposed</asp:TableHeaderCell> 
                                        <asp:TableHeaderCell ColumnSpan="4">Unexposed</asp:TableHeaderCell> 
                                        <asp:TableHeaderCell ColumnSpan="4"></asp:TableHeaderCell> 
                                    </asp:TableHeaderRow>
                                    <asp:TableHeaderRow TableSection="TableHeader">
                                        <asp:TableHeaderCell Width="14%">Drug</asp:TableHeaderCell> 
                                        <asp:TableHeaderCell Width="8%">Cases</asp:TableHeaderCell> 
                                        <asp:TableHeaderCell Width="8%">Non-Cases</asp:TableHeaderCell> 
                                        <asp:TableHeaderCell Width="8%">Population</asp:TableHeaderCell> 
                                        <asp:TableHeaderCell Width="8%">IR</asp:TableHeaderCell> 
                                        <asp:TableHeaderCell Width="8%">Cases</asp:TableHeaderCell> 
                                        <asp:TableHeaderCell Width="8%">Non-Cases</asp:TableHeaderCell> 
                                        <asp:TableHeaderCell Width="8%">Population</asp:TableHeaderCell> 
                                        <asp:TableHeaderCell Width="8%">IR</asp:TableHeaderCell> 
                                        <asp:TableHeaderCell Width="7%">Unadj. RR</asp:TableHeaderCell> 
                                        <asp:TableHeaderCell Width="7%">Adj. RR</asp:TableHeaderCell> 
                                        <asp:TableHeaderCell Width="8%">CI 95%</asp:TableHeaderCell> 
                                    </asp:TableHeaderRow>
                                </asp:Table>

							</div>
							<!-- end widget content -->
									
						</div>
						<!-- end widget div -->
								
					</div>

				</div>

			</div>
			<!-- end row -->

			<div class="row">
					
				<div class="col-sm-12 col-md-12 col-lg-12">

                    <div class="jarviswidget" id="wid-id-6"  data-widget-editbutton="false" data-widget-custombutton="false" data-widget-deletebutton="false" data-widget-colorbutton="false">

						<header>
							<h2>Patient List</h2>
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

                                <asp:Table id="dt_basic" runat="server" class="table table-striped table-bordered table-hover"  width="100%">
                                    <asp:TableHeaderRow TableSection="TableHeader">
                                        <asp:TableHeaderCell ColumnSpan="2"></asp:TableHeaderCell> 
                                        <asp:TableHeaderCell ColumnSpan="4">Contribution</asp:TableHeaderCell> 
                                        <asp:TableHeaderCell ColumnSpan="3">Risk Factor</asp:TableHeaderCell> 
                                    </asp:TableHeaderRow>
                                    <asp:TableHeaderRow TableSection="TableHeader">
                                        <asp:TableHeaderCell Width="20%">Patient Name</asp:TableHeaderCell> 
                                        <asp:TableHeaderCell Width="15%">Drug</asp:TableHeaderCell> 
                                        <asp:TableHeaderCell Width="10%">Start Date</asp:TableHeaderCell> 
                                        <asp:TableHeaderCell Width="10%">Finish Date</asp:TableHeaderCell> 
                                        <asp:TableHeaderCell Width="5%">Days</asp:TableHeaderCell> 
                                        <asp:TableHeaderCell Width="5%">Reaction</asp:TableHeaderCell> 
                                        <asp:TableHeaderCell Width="10%">Factor</asp:TableHeaderCell> 
                                        <asp:TableHeaderCell Width="15%">Criteria</asp:TableHeaderCell> 
                                        <asp:TableHeaderCell Width="10%">Criteria Met</asp:TableHeaderCell> 
                                    </asp:TableHeaderRow>
                                </asp:Table>

							</div>
							<!-- end widget content -->
									
						</div>
						<!-- end widget div -->
								
					</div>

				</div>

			</div>
			<!-- end row -->

		</div>
		<!-- end widget grid -->

    </section>

    <!-- use this modal to run the dataset download -->
    <div id="dialog-message-download" title="Dialog Simple Title">
	    <p>
		    Please select a cohort that you would like to download the extract for ...
	    </p>
		<div class="form-group">
			<label for="CohortGroupId">Cohort Group:</label>
			<select class="input-sm" id="CohortGroupId" name="CohortGroupId">
			</select>
		</div>

	    <div class="hr hr-12 hr-double"></div>
	
    </div><!-- #dialog-message -->

    <div id="waitingScreen"  class="Waiting"></div>.

</asp:Content>

<asp:Content runat="server" ID="Scripts" ContentPlaceHolderID="scriptsPlaceholder">

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

    <script>

        $(window).on('load', function () {
            $("#waitingScreen").hide();
            });

        $("#btnAnalyse").click(function () {
            $("#waitingScreen").show();
        });

    </script>

    <script type="text/javascript">

		$(function () {
			$.ajax({
				type: "GET",
				url: "../Api/CohortApi/GetCohortGroups",
				contentType: "application/json; charset=utf-8",
				dataType: "json",
				async: false,
				success: function (data) {
					$('#CohortGroupId').empty();

                    $('#CohortGroupId').append('<option value="0">-- All Cohorts --</option>');
					$.each(data, function (index, item) {
                        $('#CohortGroupId').append('<option value="' + item.Id + '">' + item.CohortName + '</option>');
					});
				}
            });

            $('#modal-download').click(function () {
                $('#dialog-message-download').dialog('open');
                return false;
            });            $("#dialog-message-download").dialog({
                autoOpen: false,
                modal: true,
                title: "Confirm",
                buttons: [{
                    html: "Cancel",
                    "class": "btn btn-default",
                    click: function () {
                        $(this).dialog("close");
                    }
                }, {
                    html: "<i class='fa fa-check'></i>&nbsp; OK",
                    "class": "btn btn-primary",
                    click: function () {
                        $(this).dialog("close");

                        $.ajax({
                            url: "../FileDownload/DownloadActiveDataset",
                            data: { cohortGroupId: $('#CohortGroupId').val() },
                            cache: false, 
                            type: "POST",
                            dataType: "html",
                            beforeSend: function () {
                                $("#waitingScreen").show();
                            },
                            success: function (data, textStatus, XMLHttpRequest) {
                                var response = JSON.parse(data);

                                var response = JSON.parse(data);
                                $("#waitingScreen").hide();
                                window.location = "../FileDownload/DownloadExcelFile?fileName=" + response.FileName;
                            },
                            error: function () {
                            }
                        });
                    }
                }]
            });
		});

        $("#ddlCohort").change(function () {
            $("#spnCohort").empty();
            var cohortId = $(this).val();
            if (cohortId > 0) {
            $.getJSON("../../Api/AnalyserApi/GetCohortDetails", { id: cohortId },
                function (data) {
                    $("#spnCohort").html(data);
                });
            }
        });

    </script>

</asp:Content>
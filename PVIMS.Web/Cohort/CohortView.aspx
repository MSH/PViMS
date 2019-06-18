<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="/Main.Master" CodeBehind="CohortView.aspx.cs" Inherits="PVIMS.Web.CohortView" Title="Cohort View" %>

<asp:Content runat="server" ID="Body" ContentPlaceHolderID="BodyContentPlaceHolder">

	<section id="widget-grid" class="">

        <div class="row">

            <article class="col-sm-12 col-md-12 col-lg-6">
                <div class="well no-padding" style="height:auto;">
                    <h2 style="font-weight:bold; text-align:center;">Cohort Details</h2>
                    <div class="smart-form">
                        <fieldset>
                            <div class="row">
                                <section class="col col-6">
                                    Cohort Name
                                    <label class="input">
                                        <input type="text" readonly="readonly" style="background-color:#EFEFEF; text-align:center;" id="txtCohortName" runat="server" />
                                    </label>
                                </section>
                                <section class="col col-3">
                                    Cohort Code
                                    <label class="input">
                                        <input type="text" readonly="readonly" style="background-color:#EFEFEF; text-align:center;" id="txtCohortCode" runat="server" />
                                    </label>
                                </section>
                                <section class="col col-3">
                                    Primary Condition
                                    <label class="input">
                                        <input type="text" readonly="readonly" style="background-color:#EFEFEF; text-align:center;" id="txtCohortCondition" runat="server" />
                                    </label>
                                </section>
                            </div>

                        </fieldset>
                    </div>
                </div>
            </article>

            <article class="col-sm-12 col-md-12 col-lg-4">
                <div class="well no-padding" style="height:auto;">
                    <h2 style="font-weight:bold; text-align:center;">Event Summary</h2>
                    <div class="smart-form">
                        <fieldset>
                            <div class="row">
                                <section class="col col-4">
                                    Patient Count
                                    <label class="input">
                                        <input type="text" readonly="readonly" style="background-color:#EFEFEF; text-align:center;" id="txtPatientCount" runat="server" />
                                    </label>
                                </section>
                                <section class="col col-4">
                                    Non-Serious Events
                                    <label class="input">
                                        <input type="text" readonly="readonly" style="background-color:#EFEFEF; text-align:center;" id="txtNonSeriousCount" runat="server" />
                                    </label>
                                </section>
                                <section class="col col-4">
                                    Serious Events
                                    <label class="input">
                                        <input type="text" readonly="readonly" style="background-color:#EFEFEF; text-align:center;" id="txtSeriousCount" runat="server" />
                                    </label>
                                </section>
                            </div>

                        </fieldset>
                    </div>
                </div>
            </article>

        </div>
		<div class="row">
				
			<article class="col-xs-12 col-sm-12 col-md-12 col-lg-12">
				
				<!-- Widget ID (each widget will need unique ID)-->
				<div class="jarviswidget well" id="wid-id-5" 
                    data-widget-editbutton="false" 
                    data-widget-custombutton="false" 
                    data-widget-deletebutton="false" 
                    data-widget-colorbutton="false">

					<header>
						<span class="widget-icon"> <i class="fa fa-table"></i> </span>
						<h2>Cohort View</h2>
					</header>
				
					<!-- widget div-->
					<div>
				
						<!-- widget edit box -->
						<div class="jarviswidget-editbox">
							<!-- This area used as dropdown edit box -->
				
						</div>
						<!-- end widget edit box -->
				
						<!-- widget content -->
						<div class="widget-body no-padding">
				
                            <asp:Table id="dt_basic" runat="server" ClientIDMode="Static" class="table table-striped table-bordered table-hover"  width="100%">
                                <asp:TableHeaderRow TableSection="TableHeader">
                                    <asp:TableHeaderCell>Patient Name</asp:TableHeaderCell> 
                                    <asp:TableHeaderCell>Facility</asp:TableHeaderCell> 
                                    <asp:TableHeaderCell>Age</asp:TableHeaderCell> 
                                    <asp:TableHeaderCell>Date of Last Encounter</asp:TableHeaderCell> 
                                    <asp:TableHeaderCell>Current Weight</asp:TableHeaderCell> 
                                    <asp:TableHeaderCell>Non-Serious Events</asp:TableHeaderCell> 
                                    <asp:TableHeaderCell>Serious Events</asp:TableHeaderCell> 
                                    <asp:TableHeaderCell>Action</asp:TableHeaderCell> 
                                </asp:TableHeaderRow>
                            </asp:Table>

						</div>
						<!-- end widget content -->
				
					</div>
					<!-- end widget div -->
				
				</div>
				<!-- end widget -->

			</article>

		</div>
			
	</section>

</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="scriptsPlaceholder">
	<script>

	</script>	
</asp:Content>


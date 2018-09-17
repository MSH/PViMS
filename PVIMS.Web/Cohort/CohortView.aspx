<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="/Main.Master" CodeBehind="CohortView.aspx.cs" Inherits="PVIMS.Web.CohortView" Title="Cohort View" %>

<asp:Content runat="server" ID="Body" ContentPlaceHolderID="BodyContentPlaceHolder">

	<!-- widget grid -->
	<section id="widget-grid" class="">

		<div class="row">
				
			<!-- NEW WIDGET START -->
			<article class="col-xs-12 col-sm-12 col-md-12 col-lg-12">
				
				<!-- Widget ID (each widget will need unique ID)-->
				<div class="jarviswidget" id="wid-id-5"  data-widget-editbutton="false" data-widget-custombutton="false" data-widget-deletebutton="false" data-widget-colorbutton="false">
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
				
							<div class="well well-sm bg-color-darken txt-color-white text-center">
								<div class="row">
                                    <section class="col col-12">
										<label class="input">Cohort Name
											<input class="form-control" id="txtCohortName" type="text" runat="server" readonly="readonly" style="background-color:#92A2A8; color:white;">
                                        </label>
										<label class="input">Cohort Code
											<input class="form-control" id="txtCohortCode" type="text" runat="server" readonly="readonly" style="background-color:#92A2A8; color:white;">
                                        </label>
                                    </section>
								</div>
							</div>

                            <asp:Table id="dt_basic" runat="server" ClientIDMode="Static" class="table table-striped table-bordered table-hover"  width="100%">
                                <asp:TableHeaderRow TableSection="TableHeader">
                                    <asp:TableHeaderCell width="15%">Patient Name</asp:TableHeaderCell> 
                                    <asp:TableHeaderCell width="15%">Facility</asp:TableHeaderCell> 
                                    <asp:TableHeaderCell width="10%">Age</asp:TableHeaderCell> 
                                    <asp:TableHeaderCell width="20%">Last Encounter</asp:TableHeaderCell> 
                                    <asp:TableHeaderCell width="10%">Current Weight</asp:TableHeaderCell> 
                                    <asp:TableHeaderCell width="20%">Adverse Reactions (Confirmed)</asp:TableHeaderCell> 
                                    <asp:TableHeaderCell width="10%">Action</asp:TableHeaderCell> 
                                </asp:TableHeaderRow>
                            </asp:Table>

						</div>
						<!-- end widget content -->
				
					</div>
					<!-- end widget div -->
				
				</div>
				<!-- end widget -->

			</article>
			<!-- WIDGET END -->

		</div>
				
		<!-- end row -->
			
	</section>
	<!-- end widget grid -->

</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="scriptsPlaceholder">
	<script>

	</script>	
</asp:Content>


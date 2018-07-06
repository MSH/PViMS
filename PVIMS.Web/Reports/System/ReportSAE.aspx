<%@ Page Language="C#" AutoEventWireup="True" CodeBehind="ReportSAE.aspx.cs" MasterPageFile="/Main.Master" Inherits="PVIMS.Web.ReportSAE" Title="Report - Serious Adverse Events" %>

<asp:Content runat="server" ID="Body" ContentPlaceHolderID="BodyContentPlaceHolder">

	<div class="row">
		<div class="col-xs-12 col-sm-7 col-md-7 col-lg-4">
			<h1 class="page-title txt-color-blueDark">
				<i class="fa fa-home fa-fw "></i> 
				Report - Serious Adverse Events
			</h1>
		</div>
	</div>
				
	<!-- widget grid -->
	<section id="widget-grid" class="">

		<div class="row">
				
			<!-- NEW WIDGET START -->
			<article class="col-xs-12 col-sm-12 col-md-12 col-lg-12">
				
				<!-- Widget ID (each widget will need unique ID)-->
				<div class="jarviswidget" id="wid-id-5"  data-widget-editbutton="false" data-widget-custombutton="false" data-widget-deletebutton="false" data-widget-colorbutton="false">
					<header>
						<span class="widget-icon"> <i class="fa fa-table"></i> </span>
						<h2>Events</h2>
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
				
							<div class="well well-sm bg-color-blueLight txt-color-white text-left">
								<div class="row">
                                    <div class="smart-form">
                                        <section class="col col-3">
										    <label class="select txt-color-white">Criteria</label>
				                            <asp:DropDownList ID="ddlCriteria" name="ddlCriteria" runat="server" Style="color: black" CssClass="form-control">
					                            <asp:ListItem Value="1" Selected="True">All Adverse Events</asp:ListItem>
					                            <asp:ListItem Value="2">Causality not set</asp:ListItem>
					                            <asp:ListItem Value="3">MedDRA terminology not set</asp:ListItem>
				                            </asp:DropDownList>
                                        </section>
                                        <section class="col col-2">
										    <label class="input txt-color-white">Range
        							            <input type="text" id="txtSearchFrom" name="txtSearchFrom" style="color: black" placeholder="yyyy-mm-dd" runat="server" class="form-control datepicker" width="250px" />
                                            </label>
                                        </section>
                                        <section class="col col-2">
										    <label class="input txt-color-white">to
        							            <input type="text" id="txtSearchTo" name="txtSearchTo" style="color: black" placeholder="yyyy-mm-dd" runat="server" class="form-control datepicker" width="250px" />
                                            </label>
                                        </section>
                                    </div>
                                    <section class="col col-2">
										<label class="input txt-color-white"><br />
                                            &nbsp;&nbsp;&nbsp;
                                            <asp:Button ID="btnSubmit" runat="server" Text="Search" OnClick="btnSubmit_Click" class="btn btn-primary" />
                                            <span id="spnButtons" runat="server">
                                            </span>
                                            &nbsp;&nbsp;&nbsp;
                                            <span class="label label-warning" id="spnRows" runat="server" visible="false"></span>
                                        </label>
                                    </section>
								</div>
							</div>

                            <div class="alert alert-danger alert-block" runat="server" id="divError" name="diverror" visible="false">
                                <span id="spnErrors" runat="server"></span>
                            </div>

                            <asp:Table id="dt_basic" runat="server" ClientIDMode="Static" class="table table-striped table-bordered table-hover"  width="100%">
                                <asp:TableHeaderRow TableSection="TableHeader">
                                    <asp:TableHeaderCell width="10%">First Name</asp:TableHeaderCell> 
                                    <asp:TableHeaderCell width="10%">Last Name</asp:TableHeaderCell> 
                                    <asp:TableHeaderCell width="10%">Facility</asp:TableHeaderCell> 
                                    <asp:TableHeaderCell width="15%">Adverse Event</asp:TableHeaderCell> 
                                    <asp:TableHeaderCell width="10%">Onset Date</asp:TableHeaderCell> 
                                    <asp:TableHeaderCell width="10%">Naranjo Causality</asp:TableHeaderCell> 
                                    <asp:TableHeaderCell width="10%">WHO Causality</asp:TableHeaderCell> 
                                    <asp:TableHeaderCell width="15%">Adverse Drug Reaction</asp:TableHeaderCell> 
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

<asp:Content runat="server" ID="Scripts" ContentPlaceHolderID="scriptsPlaceholder">
    <script>

	</script>

</asp:Content>



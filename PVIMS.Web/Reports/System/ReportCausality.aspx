<%@ Page Language="C#" AutoEventWireup="True" CodeBehind="ReportCausality.aspx.cs" MasterPageFile="/Main.Master" Inherits="PVIMS.Web.ReportCausality" Title="Report - Causality" %>

<asp:Content runat="server" ID="Body" ContentPlaceHolderID="BodyContentPlaceHolder">

	<!-- widget grid -->
	<section id="widget-grid" class="">

		<div class="row">
				
			<!-- NEW WIDGET START -->
			<article class="col-xs-12 col-sm-12 col-md-12 col-lg-12">
				
				<!-- Widget ID (each widget will need unique ID)-->
				<div class="jarviswidget well" id="wid-id-5" 
                    data-widget-editbutton="false" 
                    data-widget-custombutton="false" 
                    data-widget-deletebutton="false" 
                    data-widget-colorbutton="false">

					<header>
						<span class="widget-icon"> <i class="fa fa-table"></i> </span>
						<h2>Causality</h2>
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
                            <div class="alert alert-danger alert-block" runat="server" id="divError" name="divError" visible="false">
                                <strong>Error!</strong> Please ensure all details are captured correctly...
                            </div>					
							<div class="well well-sm" style="height:auto; padding-top:10px; padding-left:10px; padding-right:10px; padding-bottom:0px;">
								<div class="row">
                                    <div class="smart-form">
                                        <section class="col col-3">
                                            <label runat="server" id="lblFacility" class="select">Facility</label>
                                            <asp:DropDownList ID="ddlFacility" name="ddlFacility" runat="server" Style="color: black" CssClass="form-control">
                                                <asp:ListItem Value="" Selected="True">All Facilities</asp:ListItem>
                                            </asp:DropDownList>
                                        </section>
                                        <section class="col col-2">
										    <label class="select">Criteria</label>
				                            <asp:DropDownList ID="ddlCriteria" name="ddlCriteria" runat="server" Style="color: black" CssClass="form-control">
					                            <asp:ListItem Value="1" Selected="True">Causality set</asp:ListItem>
					                            <asp:ListItem Value="2">Causality not set</asp:ListItem>
				                            </asp:DropDownList>
                                        </section>
                                        <section class="col col-2">
										    <label runat="server" id="lblSearchFrom" class="input">Range
        							            <input type="text" id="txtSearchFrom" name="txtSearchFrom" style="color: black" placeholder="yyyy-mm-dd" runat="server" class="form-control datepicker" width="250px" />
                                            </label>
                                        </section>
                                        <section class="col col-2">
										    <label runat="server" id="lblSearchTo" class="input">to
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
                                        </label>
                                    </section>
								</div>
							</div>
                            <div class="alert alert-block" style="text-align:center; height:30px;">
                                <span class="label txt-color-red" id="spnNoRows" runat="server" visible="false" style="font-size: 100%;"></span>
                                <span class="label txt-color-red" id="spnRows" runat="server" visible="false" style="font-size: 100%;"></span>
                            </div>

                            <div class="well well-sm" style="height:auto; padding-top:10px; padding-left:10px; padding-right:10px; padding-bottom:10px;text-align:right">
                                <label class="select"><b>Export to | </b></label>
                                <a href="javascript:void(0);" runat="server" onserverclick="btnExportXml_Click" title="XML"><i class="fa fa-lg fa-fw fa-file-text-o"></i></a>
                                <a href="javascript:void(0);" runat="server" onserverclick="btnExportXls_Click" title="EXCEL"><i class="fa fa-lg fa-fw fa-file-excel-o"></i></a>
                                <a href="javascript:void(0);" runat="server" onserverclick="btnExportPdf_Click" title="PDF"><i class="fa fa-lg fa-fw fa-file-pdf-o"></i></a>
                            </div>
                            <asp:Table id="dt_basic" runat="server" ClientIDMode="Static" class="table table-striped table-bordered table-hover"  width="100%">
                                <asp:TableHeaderRow TableSection="TableHeader">
                                    <asp:TableHeaderCell>First Name</asp:TableHeaderCell> 
                                    <asp:TableHeaderCell>Last Name</asp:TableHeaderCell> 
                                    <asp:TableHeaderCell>Facility</asp:TableHeaderCell> 
                                    <asp:TableHeaderCell>Adverse Event</asp:TableHeaderCell> 
                                    <asp:TableHeaderCell>Serious</asp:TableHeaderCell> 
                                    <asp:TableHeaderCell>Onset Date</asp:TableHeaderCell> 
                                    <asp:TableHeaderCell>Medication</asp:TableHeaderCell> 
                                    <asp:TableHeaderCell>Naranjo Causality</asp:TableHeaderCell> 
                                    <asp:TableHeaderCell>WHO Causality</asp:TableHeaderCell> 
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



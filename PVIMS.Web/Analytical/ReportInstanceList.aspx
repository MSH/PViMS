<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ReportInstanceList.aspx.cs" MasterPageFile="/Main.Master" Inherits="PVIMS.Web.ReportInstanceList" Title="Adverse Event List" %>

<asp:Content runat="server" ID="Body" ContentPlaceHolderID="BodyContentPlaceHolder">
				
	<!-- widget grid -->
	<section id="widget-grid" class="">

		<div class="row">
				
			<!-- NEW WIDGET START -->
			<article class="col-xs-12 col-sm-12 col-md-12 col-lg-12">
				
				<!-- Widget ID (each widget will need unique ID)-->
				<div class="jarviswidget"  data-widget-editbutton="false" data-widget-custombutton="false" data-widget-deletebutton="false" data-widget-colorbutton="false">
				
					<!-- widget div-->
					<div>
				
						<!-- widget edit box -->
						<div class="jarviswidget-editbox">
							<!-- This area used as dropdown edit box -->
				
						</div>
						<!-- end widget edit box -->
				
						<!-- widget content -->
						<div class="widget-body no-padding">
							<div class="well well-sm" style="height:auto; padding-top:10px; padding-left:10px; padding-right:10px; padding-bottom:0px;">
								<div class="row">
                                    <div class="smart-form">
                                        <section class="col col-3">
										    <label class="select" runat="server" id="lblReportCriteria">Report Criteria
				                                <asp:DropDownList ID="ddlReportCriteria" name="ddlReportCriteria" runat="server" Style="color: black" CssClass="form-control">
				                                </asp:DropDownList>
                                            </label>
                                        </section>
                                        <section class="col col-2">
										    <label class="input" runat="server" id="lblSearchFrom">Report Date From
        							            <input type="text" id="txtSearchFrom" name="txtSearchFrom" style="color: black" placeholder="yyyy-mm-dd" runat="server" class="form-control datepicker" width="250px" />
                                            </label>
                                        </section>
                                        <section class="col col-2">
										    <label class="input" runat="server" id="lblSearchTo">Report Date To
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

                            <asp:Table id="dt_basic" runat="server" ClientIDMode="Static" class="table table-striped table-bordered table-hover"  width="100%">
                                <asp:TableHeaderRow TableSection="TableHeader">
                                    <asp:TableHeaderCell width="8%">Created</asp:TableHeaderCell> 
                                    <asp:TableHeaderCell width="8%">Identifier</asp:TableHeaderCell> 
                                    <asp:TableHeaderCell width="12%">Patient</asp:TableHeaderCell> 
                                    <asp:TableHeaderCell width="25%">Medication Summary<span class="badge txt-color-white bg-color-red pull-right" style="padding:5px;width:80px;"> Naranjo </span> <span class="badge txt-color-white bg-color-darken pull-right" style="padding:5px;width:80px;"> WHO </span></asp:TableHeaderCell> 
                                    <asp:TableHeaderCell width="11%">Adverse Event</asp:TableHeaderCell> 
                                    <asp:TableHeaderCell width="11%">MedDRA Term</asp:TableHeaderCell> 
                                    <asp:TableHeaderCell width="14%">Status</asp:TableHeaderCell> 
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



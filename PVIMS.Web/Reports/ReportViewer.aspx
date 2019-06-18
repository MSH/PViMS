<%@ Page Language="C#" AutoEventWireup="True" CodeBehind="ReportViewer.aspx.cs" MasterPageFile="/Main.Master" Inherits="PVIMS.Web.ReportViewer" Title="Report Viewer" %>

<asp:Content runat="server" ID="Body" ContentPlaceHolderID="BodyContentPlaceHolder">

	<!-- widget grid -->
	<section id="widget-grid" class="">

		<div class="row">
				
			<!-- NEW WIDGET START -->
			<article class="col-xs-12 col-sm-12 col-md-12 col-lg-12">
				
				<!-- Widget ID (each widget will need unique ID)-->
				<div class="well">
					<div>
				
						<div class="widget-body no-padding">
                            <div class="alert alert-danger alert-block" runat="server" id="divError" name="divError" visible="false">
                                <strong>Error!</strong> Please ensure all details are captured correctly...
                            </div>				

                            <div class="smart-form">
							    <div class="well" style="height:auto; padding-top:10px; padding-left:10px; padding-right:10px; padding-bottom:0px;">
                                    <span id="spnFilter" runat="server"></span>
							    </div>
								<footer>
                                    <asp:Button ID="btnSubmit" runat="server" Text="Search" OnClick="btnSubmit_Click" class="btn btn-primary" />
								</footer>
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
                            <asp:Table ID="dt_basic" runat="server" ClientIDMode="Static" class="table table-striped table-bordered table-hover" Width="100%">
                                <asp:TableHeaderRow TableSection="TableHeader">
                                    <asp:TableHeaderCell Width="1%"></asp:TableHeaderCell>
                                    <asp:TableHeaderCell Width="1%"></asp:TableHeaderCell>
                                    <asp:TableHeaderCell Width="1%"></asp:TableHeaderCell>
                                    <asp:TableHeaderCell Width="1%"></asp:TableHeaderCell>
                                    <asp:TableHeaderCell Width="1%"></asp:TableHeaderCell>
                                    <asp:TableHeaderCell Width="1%"></asp:TableHeaderCell>
                                    <asp:TableHeaderCell Width="1%"></asp:TableHeaderCell>
                                    <asp:TableHeaderCell Width="1%"></asp:TableHeaderCell>
                                    <asp:TableHeaderCell Width="1%"></asp:TableHeaderCell>
                                    <asp:TableHeaderCell Width="1%"></asp:TableHeaderCell>
                                    <asp:TableHeaderCell Width="1%"></asp:TableHeaderCell>
                                    <asp:TableHeaderCell Width="1%"></asp:TableHeaderCell>
                                    <asp:TableHeaderCell Width="1%"></asp:TableHeaderCell>
                                    <asp:TableHeaderCell Width="1%"></asp:TableHeaderCell>
                                    <asp:TableHeaderCell Width="1%"></asp:TableHeaderCell>
                                    <asp:TableHeaderCell Width="1%"></asp:TableHeaderCell>
                                    <asp:TableHeaderCell Width="1%"></asp:TableHeaderCell>
                                    <asp:TableHeaderCell Width="1%"></asp:TableHeaderCell>
                                    <asp:TableHeaderCell Width="1%"></asp:TableHeaderCell>
                                    <asp:TableHeaderCell Width="1%"></asp:TableHeaderCell>
                                </asp:TableHeaderRow>
                            </asp:Table>
						</div>
				
					</div>
				
				</div>
				
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



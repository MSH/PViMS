    <%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ReportMeta.aspx.cs" Inherits="PVIMS.Web.ReportMeta" MasterPageFile="~/Main.Master" Title="Meta Data" %>

<asp:Content runat="server" ID="Breadcrumbcontainer" ContentPlaceHolderID="breadcrumbcontainer">
    <li>Administration</li>
</asp:Content>

<asp:Content runat="server" ID="Body" ClientIDMode="Static" ContentPlaceHolderID="BodyContentPlaceHolder">
    <div class="row">
		<div class="col-xs-12 col-sm-7 col-md-7 col-lg-4">
			<h1 class="page-title txt-color-blueDark">
				<i class="fa fa-table fa-fw "></i> 
					Administration 
				<span>> 
					Report Meta Data
				</span>
			</h1>
		</div>
	</div>

	<!-- widget grid -->
	<section id="widget-grid" class="">

		<div class="well row">

            <article class="col-sm-12 col-md-12 col-lg-9">
				<div class="well">
                    <h4>Meta Data Summary</h4>
			        <div class="col col-md-12">

				        <table class="table ">
					        <thead>
					        <tr>
						        <th></th>
						        <th></th>
					        </tr>
					        </thead>
					        <tbody>
					            <tr>
						            <th>Meta tables defined</th>
						            <td class="text-left" id="tdMetaTableDefined" runat="server">
						            </td>
					            </tr>
					            <tr>
						            <th>Meta dependencies defined</th>
						            <td class="text-left" id="tdMetaDependencyDefined" runat="server">
						            </td>
					            </tr>
					            <tr>
						            <th>Meta Columns Defined</th>
						            <td class="text-left" id="tdMetaColumnsDefined" runat="server">
						            </td>
					            </tr>
					            <tr>
						            <th>Total number of patient record</th>
						            <td class="text-left" id="tdPatientRecords" runat="server">
						            </td>
					            </tr>
					        </tbody>

				        </table>

                        <div class="smart-form">
							<footer>
                                <asp:Button ID="btnRefresh" OnClick="btnRefresh_Click" runat="server" Text="Refresh Meta" CssClass="btn btn-default" />
							</footer>
                        </div>

			        </div>
                    <div class="col col-md-4">
                    </div>
                    <div>
                        <legend>Summary</legend>
                        <span id="spnSummary" runat="server"></span>
                    </div>
				</div>
			</article>

        </div>

		<div class="well row">

			<article class="col-sm-12 col-md-12 col-lg-9">
				
				<!-- Widget ID (each widget will need unique ID)-->
				<div class="jarviswidget" id="wid-id-1"  data-widget-editbutton="false" data-widget-custombutton="false" data-widget-deletebutton="false" data-widget-colorbutton="false"">
					<header>
						<span class="widget-icon"> <i class="fa fa-edit"></i> </span>
						<h2>Report Meta Data</h2>
				
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
							<ul class="nav nav-tabs bordered">
								<li id="liMetaTable" runat="server" class="active">
									<a href="#tab1" data-toggle="tab" aria-expanded="true">Meta Tables</a>
								</li>
								<li id="liMetaColumn" runat="server">
									<a href="#tab2" data-toggle="tab" aria-expanded="true">Meta Columns</a>
								</li>								
								<li id="liMetaDependency" runat="server">
									<a href="#tab3" data-toggle="tab" aria-expanded="true">Meta Dependencies</a>
								</li>								
							</ul>
                            <div class="tab-content">
                                
                                <div id="tab1" class="tab-pane active smart-form"  runat="server">
						            <div class="table-responsive">
							            <asp:Table ID="dt_1" runat="server" class="table table-striped table-bordered table-hover" Width="100%">
								            <asp:TableHeaderRow TableSection="TableHeader">
									            <asp:TableHeaderCell Width="25%">Name</asp:TableHeaderCell>
                                                <asp:TableHeaderCell Width="25%">Friendly Name</asp:TableHeaderCell>
                                                <asp:TableHeaderCell Width="25%">Description</asp:TableHeaderCell>
                                                <asp:TableHeaderCell Width="25%">Type</asp:TableHeaderCell>
								            </asp:TableHeaderRow>
							            </asp:Table>
						            </div>
                                </div>

								<div id="tab2" class="tab-pane" runat="server">
						            <div class="table-responsive">
							            <asp:Table ID="dt_basic_2" runat="server" class="table table-striped table-bordered table-hover" Width="100%">
								            <asp:TableHeaderRow TableSection="TableHeader">
									            <asp:TableHeaderCell Width="20%">Table</asp:TableHeaderCell>
									            <asp:TableHeaderCell Width="20%">Name</asp:TableHeaderCell>
                                                <asp:TableHeaderCell Width="20%">Identity</asp:TableHeaderCell>
                                                <asp:TableHeaderCell Width="20%">Nullable</asp:TableHeaderCell>
                                                <asp:TableHeaderCell Width="20%">Type</asp:TableHeaderCell>
								            </asp:TableHeaderRow>
							            </asp:Table>
						            </div>
								</div>

								<div id="tab3" class="tab-pane" runat="server">
						            <div class="table-responsive">
							            <asp:Table ID="dt_basic_3" runat="server" class="table table-striped table-bordered table-hover" Width="100%">
								            <asp:TableHeaderRow TableSection="TableHeader">
									            <asp:TableHeaderCell Width="25%">Parent Table</asp:TableHeaderCell>
                                                <asp:TableHeaderCell Width="25%">Parent Column</asp:TableHeaderCell>
                                                <asp:TableHeaderCell Width="25%">Reference Table</asp:TableHeaderCell>
                                                <asp:TableHeaderCell Width="25%">Reference Column</asp:TableHeaderCell>
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
				<!-- end widget -->
				
			</article>

		</div>
		
	</section>
    <!-- end widget grid -->
</asp:Content>

<asp:Content runat="server" ID="Scripts" ContentPlaceHolderID="scriptsPlaceholder">
    
</asp:Content>
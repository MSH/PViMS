<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ManageDataset.aspx.cs" Inherits="PVIMS.Web.ManageDataset" Title="Manage Datasets" MasterPageFile="~/Main.Master" %>

<asp:Content runat="server" ID="Breadcrumbcontainer" ContentPlaceHolderID="breadcrumbcontainer">
	<li>Administration</li>
</asp:Content>

<asp:Content runat="server" ID="Body" ClientIDMode="Static" ContentPlaceHolderID="BodyContentPlaceHolder">
    
    <div class="row">
		<div class="col-xs-12 col-sm-7 col-md-7 col-lg-4">
			<h1 class="page-title txt-color-blueDark">
				<i class="fa fa-windows fa-fw "></i> 
					Datasets
			</h1>
		</div>
    </div>
				
	<!-- widget grid -->
	<section id="widget-grid" class="">
				
		<!-- row -->
		<div class="row">
				
			<!-- NEW WIDGET START -->
			<article class="col-xs-12 col-sm-12 col-md-12 col-lg-12">
				
				<!-- Widget ID (each widget will need unique ID)-->
				<div class="jarviswidget" id="wid-id-0"  data-widget-editbutton="false" data-widget-custombutton="false" data-widget-deletebutton="false" data-widget-colorbutton="false">
					<header>
						<span class="widget-icon"> <i class="fa fa-table"></i> </span>
						<h2>Datasets</h2>
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
							<div class="well well-sm bg-color-blueLight txt-color-white text-right">
                                <span id="spnbuttons" runat="server" >

                                </span>
							</div>                                        
                            <asp:Table id="dt_basic" runat="server" class="table table-striped table-bordered table-hover"  width="100%">
                                <asp:TableHeaderRow TableSection="TableHeader">
                                    <asp:TableHeaderCell Width="20%">Name</asp:TableHeaderCell> 
                                    <asp:TableHeaderCell Width="10%">Created</asp:TableHeaderCell> 
                                    <asp:TableHeaderCell Width="10%">Updated</asp:TableHeaderCell> 
                                    <asp:TableHeaderCell Width="10%">Context</asp:TableHeaderCell> 
                                    <asp:TableHeaderCell Width="10%">Active</asp:TableHeaderCell> 
                                    <asp:TableHeaderCell Width="25%">Help</asp:TableHeaderCell> 
                                    <asp:TableHeaderCell Width="15%">Action</asp:TableHeaderCell> 
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



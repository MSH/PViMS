<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ManageDatasetElement.aspx.cs" Inherits="PVIMS.Web.ManageDatasetElement" Title="Manage Dataset Elements" MasterPageFile="~/Main.Master" %>

<asp:Content runat="server" ID="Breadcrumbcontainer" ContentPlaceHolderID="breadcrumbcontainer">
	<li>Administration</li>
</asp:Content>

<asp:Content runat="server" ID="Body" ClientIDMode="Static" ContentPlaceHolderID="BodyContentPlaceHolder">
    
	<!-- widget grid -->
	<section id="widget-grid" class="">
				
		<!-- row -->
		<div class="row">
            				
			<!-- NEW WIDGET START -->
			<article class="col-xs-12 col-sm-12 col-md-12 col-lg-9">
				
				<!-- Widget ID (each widget will need unique ID)-->
				<div class="jarviswidget well" id="wid-id-0" 
                    data-widget-editbutton="false" 
                    data-widget-custombutton="false" 
                    data-widget-deletebutton="false" 
                    data-widget-colorbutton="false">

					<header>
						<span class="widget-icon"> <i class="fa fa-table"></i> </span>
						<h2>Dataset Elements</h2>
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
                                    <asp:TableHeaderCell Width="40%">Element Name</asp:TableHeaderCell> 
                                    <asp:TableHeaderCell Width="15%">Categories</asp:TableHeaderCell> 
                                    <asp:TableHeaderCell Width="15%">Field Type</asp:TableHeaderCell> 
                                    <asp:TableHeaderCell Width="15%">Mandatory</asp:TableHeaderCell> 
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

        $('body').popover({
            selector: 'a[data-toggle=popover]',
            container: 'body'
        }).tooltip({
            selector: "a[data-toggle=tooltip]",
            container: 'body'
        });

	</script>

</asp:Content>



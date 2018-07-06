<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ManageReference.aspx.cs" Inherits="VPS.Dcat.Web.ManageReference" Title="Manage Reference Data" MasterPageFile="~/Main.Master" %>

<asp:Content runat="server" ID="Breadcrumbcontainer" ContentPlaceHolderID="breadcrumbcontainer">
	<li>Administration</li>
</asp:Content>

<asp:Content runat="server" ID="Body" ClientIDMode="Static" ContentPlaceHolderID="BodyContentPlaceHolder">
    
    <div class="row">
		<div class="col-xs-12 col-sm-7 col-md-7 col-lg-4">
			<h1 class="page-title txt-color-blueDark">
				<i class="fa fa-table fa-fw "></i> 
					Reference Data
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
				<div class="jarviswidget jarviswidget-color-greenDark" id="wid-id-0"
					data-widget-togglebutton="false"
						data-widget-deletebutton="false"
						data-widget-fullscreenbutton="false" data-widget-editbutton="false" data-widget-colorbutton="false">
					<!-- widget options:
					usage: <div class="jarviswidget" id="wid-id-0" data-widget-editbutton="false">
				
						data-widget-togglebutton="false"
						data-widget-deletebutton="false"
						data-widget-fullscreenbutton="false"

					data-widget-colorbutton="false"
					data-widget-togglebutton="false"
								
								
					data-widget-custombutton="false"
					data-widget-collapsed="true"
					data-widget-sortable="false"
				
					-->
					<header>
						<span class="widget-icon"> <i class="fa fa-table"></i> </span>
						<h2>Reference Data</h2>
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
                            <asp:Table id="dt_basic" runat="server" class="table table-striped table-bordered table-hover"  width="50%">
                                <asp:TableHeaderRow TableSection="TableHeader">
                                    <asp:TableHeaderCell Width="50%">Description</asp:TableHeaderCell> 
                                    <asp:TableHeaderCell Width="50%">Action</asp:TableHeaderCell> 
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




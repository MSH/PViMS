<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ManageContactDetail.aspx.cs" Inherits="PVIMS.Web.ManageContactDetail" Title="Manage Contact Details" MasterPageFile="~/Main.Master" %>

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
						<h2>Contact Details</h2>
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
                            <div class="alert alert-success fade in" runat="server" id="divstatus" name="divstatus" visible="false">
                                <strong>Success</strong> Contact detail saved successfully!&nbsp&nbsp&nbsp
                            </div>		
                            <asp:Table id="dt_basic" runat="server" class="table table-striped table-bordered table-hover"  width="100%">
                                <asp:TableHeaderRow TableSection="TableHeader">
                                    <asp:TableHeaderCell Width="20%">Contact Type</asp:TableHeaderCell> 
                                    <asp:TableHeaderCell Width="20%">Contact First Name</asp:TableHeaderCell> 
                                    <asp:TableHeaderCell Width="20%">Contact Last Name</asp:TableHeaderCell> 
                                    <asp:TableHeaderCell Width="20%">City</asp:TableHeaderCell> 
                                    <asp:TableHeaderCell Width="20%">Action</asp:TableHeaderCell> 
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

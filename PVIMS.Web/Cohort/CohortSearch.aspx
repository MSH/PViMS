<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="/Main.Master" CodeBehind="CohortSearch.aspx.cs" Inherits="PVIMS.Web.CohortSearch" Title="Cohorts" %>

<asp:Content runat="server" ID="Body" ContentPlaceHolderID="BodyContentPlaceHolder">

	<div class="row">
		<div class="col-xs-12 col-sm-7 col-md-7 col-lg-4">
			<h1 class="page-title txt-color-blueDark">
				<i class="fa fa-cogs fa-fw "></i> 
				Cohorts
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
						<h2>Cohorts</h2>
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
                                <strong>Success</strong> Cohort saved successfully!&nbsp&nbsp&nbsp
                            </div>		
							<div class="well well-sm bg-color-blueLight txt-color-white text-right">
                                <span id="spnbuttons" runat="server" >

                                </span>
							</div>
                            <div class="alert alert-danger alert-block" runat="server" id="divError" name="diverror" visible="false">
                                <span id="spnErrors" runat="server"></span>
                            </div>		

                            <asp:Table id="dt_basic" runat="server" ClientIDMode="Static" class="table table-striped table-bordered table-hover"  width="100%">
                                <asp:TableHeaderRow TableSection="TableHeader">
                                    <asp:TableHeaderCell width="5%">ID</asp:TableHeaderCell> 
                                    <asp:TableHeaderCell width="25%">Cohort Name</asp:TableHeaderCell> 
                                    <asp:TableHeaderCell width="15%">Cohort Code</asp:TableHeaderCell> 
                                    <asp:TableHeaderCell width="10%"># Patients</asp:TableHeaderCell> 
                                    <asp:TableHeaderCell width="15%">Start Date</asp:TableHeaderCell> 
                                    <asp:TableHeaderCell width="15%">Finish Date</asp:TableHeaderCell> 
                                    <asp:TableHeaderCell width="15%">Action</asp:TableHeaderCell> 
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


<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AuditTrail.aspx.cs" Inherits="PVIMS.Web.AuditTrail" MasterPageFile="~/Main.Master" Title="Audit Trail" %>

<asp:Content runat="server" ID="Breadcrumbcontainer" ContentPlaceHolderID="breadcrumbcontainer">
    
</asp:Content>


<asp:Content runat="server" ID="Body" ClientIDMode="Static" ContentPlaceHolderID="BodyContentPlaceHolder">
    <div novalidate>
		<div class="row">
			<div class="col-xs-12 col-sm-7 col-md-7 col-lg-4">
				<h1 class="page-title txt-color-blueDark">
					<i class="fa fa-table fa-fw "></i> 
						Administration
					<span>> 
						Audit Trail
					</span>
				</h1>
			</div>
		</div>
				
		<!-- widget grid -->
		<section id="widget-grid" class="">

			<!-- row -->
			<div class="well row">
				
				<!-- NEW WIDGET START -->
				<article class="col-xs-12 col-sm-12 col-md-12 col-lg-10">
				
					<!-- Widget ID (each widget will need unique ID)-->
					<div class="jarviswidget" id="wid-id-1"  data-widget-editbutton="false" data-widget-custombutton="false" data-widget-deletebutton="false" data-widget-colorbutton="false">

						<header>
							<span class="widget-icon"> <i class="fa fa-table"></i> </span>
							<h2>History</h2>
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
				
                                <div class="smart-form">

							        <fieldset>
								        <div class="row">
									        <section class="col col-3">
                                                <label class="input">Audit Type:
                                                    <asp:DropDownList ID="ddlAuditType" name="ddlAuditType" runat="server" style="color:black;" class="form-control">
                                                        <asp:ListItem Value="Subscriber Access" Selected="True">Subscriber Access</asp:ListItem>
                                                        <asp:ListItem Value="Subscriber Post">Subscriber Post</asp:ListItem>
                                                        <asp:ListItem Value="MedDRA Import">MedDRA Import</asp:ListItem>
                                                        <asp:ListItem Value="User Logins">User logins</asp:ListItem>
                                                    </asp:DropDownList>
										        </label>
									        </section>
									        <section class="col col-3">
                                                <label class="input" id="lblDateFrom" runat="server">From:
							                        <input type="text" id="txtDateFrom" name="txtDateFrom" style="color:black;" placeholder="yyyy-mm-dd" runat="server" class="form-control datepicker" width="250px" />
										        </label>
									        </section>
									        <section class="col col-3">
                                                <label class="input" id="lblDateTo" runat="server" >To:
							                        <input type="text" id="txtDateTo" name="txtDateTo" style="color:black;" placeholder="yyyy-mm-dd" runat="server" class="form-control datepicker" width="250px" />
										        </label>
									        </section>
								        </div>

								        <footer>
                                            <asp:Button ID="btnSearch" runat="server" Text="Search" OnClick="btnSearch_Click" class="btn btn-primary btn-sm" />
								        </footer>

							        </fieldset>

                                </div>

                                <asp:Table id="dt_basic" runat="server" class="table table-striped table-bordered table-hover"  width="100%">
                                    <asp:TableHeaderRow TableSection="TableHeader">
                                        <asp:TableHeaderCell>Audit Type</asp:TableHeaderCell> 
                                        <asp:TableHeaderCell>Details</asp:TableHeaderCell> 
                                        <asp:TableHeaderCell>Creation</asp:TableHeaderCell> 
                                        <asp:TableHeaderCell>Log</asp:TableHeaderCell> 
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
    </div>
</asp:Content>
    <%@ Page Language="C#" AutoEventWireup="true" CodeBehind="PageCustom.aspx.cs" Inherits="PVIMS.Web.PageCustom" MasterPageFile="~/Main.Master" Title="Manage Publication Pages" %>

<asp:Content runat="server" ID="Breadcrumbcontainer" ContentPlaceHolderID="breadcrumbcontainer">
    <li>Administration</li>
</asp:Content>

<asp:Content runat="server" ID="Body" ClientIDMode="Static" ContentPlaceHolderID="BodyContentPlaceHolder">

    <asp:HiddenField runat="server" ID="hfPosition" Value="" />

	<!-- widget grid -->
	<section id="widget-grid" class="">

		<!-- row -->
		<div class="row">

			<!-- NEW COL START -->
			<article class="col-sm-12 col-md-12 col-lg-9">
				
				<!-- Widget ID (each widget will need unique ID)-->
				<div class="jarviswidget" id="wid-id-1"  data-widget-editbutton="false" data-widget-custombutton="false" data-widget-deletebutton="false" data-widget-colorbutton="false">
					<header>
						<span class="widget-icon"> <i class="fa fa-edit"></i> </span>
						<h2>Manage Pages </h2>
					</header>
				
					<!-- widget div-->
					<div>
				
						<!-- widget edit box -->
						<div class="jarviswidget-editbox">
							<!-- This area used as dropdown edit box -->
				
						</div>
						<!-- end widget edit box -->
				
						<!-- widget content -->
						<div class="widget-body padding smart-form">
				
							<fieldset>
                                <legend>Details</legend>
								<div class="row" id="divUnique" runat="server">
                                    <section class="col col-4">
										<label class="input">Unique Identifier
											<input class="form-control" id="txtUID" type="text" runat="server" readonly="readonly" style="background-color:#EBEBE4;">
                                        </label>
                                    </section>
								</div>
								<div class="row">
                                    <section class="col col-8">
										<label class="input" id="lblName" runat="server">Page Name
											<input class="form-control" id="txtName" type="text" runat="server" maxlength="50" required>
                                        </label>
                                    </section>
								</div>
								<div class="row">
                                    <section class="col col-8">
										<label class="input" id="lblDefinition" runat="server">Definition
											<textarea class="form-control" id="txtDefinition" runat="server" maxlength="250" rows="4"/>
                                        </label>
                                    </section>
								</div>
								<div class="row">
                                    <section class="col col-4">
										<label class="input">Breadcrumb
											<input class="form-control" id="txtBreadcrumb" type="text" runat="server" maxlength="250" >
                                        </label>
                                    </section>
								</div>
								<div class="row">
                                    <section class="col col-4">
										<label class="input">Visible To Menu
											<select id="ddlVisible" name="ddlVisible" class="input form-control"  runat="server">
												<option value="No" selected="selected">No</option>
												<option value="Yes">Yes</option>
											</select>
                                        </label>
                                    </section>
								</div>
							</fieldset>

                            <br />
                            <div class="smart-form">
							    <footer>
								    <span id="spnButtons" runat="server">

								    </span>
							    </footer>
                                <div class="alert alert-danger alert-block" runat="server" id="divError" name="divError" visible="false">
                                    <span id="spnErrors" runat="server"></span>
                                </div>		
                            </div>      		
						</div>
						<!-- end widget content -->
				
					</div>
					<!-- end widget div -->
				
				</div>
				<!-- end widget -->
				
			</article>
			<!-- END COL -->					
					
		</div>

		<!-- row -->
		<div class="row" id="divWidget" runat="server">

			<!-- NEW COL START -->
			<article class="col-sm-12 col-md-12 col-lg-9">
				
				<!-- Widget ID (each widget will need unique ID)-->
				<div class="jarviswidget" id="wid_id_2"  data-widget-editbutton="false" data-widget-custombutton="false" data-widget-deletebutton="false" data-widget-colorbutton="false">
					<header>
						<span class="widget-icon"> <i class="fa fa-edit"></i> </span>
						<h2>Widgets </h2>
				
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
				
							<div>
                                <div class="row">
                                    <div class="smart-form">
							            <footer>
								            <span id="spnWidgetButtons" runat="server">

								            </span>
							            </footer>
                                    </div>
                                </div>

								<asp:Table ID="dt_1" runat="server" class="table table-striped table-bordered table-hover" Width="100%">
									<asp:TableHeaderRow TableSection="TableHeader">
										<asp:TableHeaderCell Width="20%">Widget Name</asp:TableHeaderCell>
										<asp:TableHeaderCell Width="30%">Definition</asp:TableHeaderCell>
										<asp:TableHeaderCell Width="20%">Widget Type</asp:TableHeaderCell>
                                        <asp:TableHeaderCell Width="20%">Widget Position</asp:TableHeaderCell>
										<asp:TableHeaderCell Width="10%">Action</asp:TableHeaderCell>
									</asp:TableHeaderRow>
								</asp:Table>
							</div>  		

						</div>
						<!-- end widget content -->
				
					</div>
					<!-- end widget div -->
				
				</div>
				<!-- end widget -->
				
			</article>
			<!-- END COL -->					

		</div>
		
	</section>
    <!-- end widget grid -->
</asp:Content>

<asp:Content runat="server" ID="Scripts" ContentPlaceHolderID="scriptsPlaceholder">
    
    <script type="text/javascript">
        $(function () {
            var f = $("#<%=hfPosition.ClientID%>");
            window.onload = function () {
                var position = parseInt(f.val());
                if (!isNaN(position)) {
                    $(window).scrollTop(position);
                }
            };
            window.onscroll = function () {
                var position = $(window).scrollTop();
                f.val(position);
            };
        });
    </script>

</asp:Content>
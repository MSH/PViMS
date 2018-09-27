    <%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ManageWorkPlanEdit.aspx.cs" Inherits="PVIMS.Web.ManageWorkPlanEdit" MasterPageFile="~/Main.Master" Title="Manage Work Plans" %>

<asp:Content runat="server" ID="Breadcrumbcontainer" ContentPlaceHolderID="breadcrumbcontainer">
    <li>Administration</li>
</asp:Content>

<asp:Content runat="server" ID="Body" ClientIDMode="Static" ContentPlaceHolderID="BodyContentPlaceHolder">

	<!-- widget grid -->
	<section id="widget-grid" class="">

		<!-- row -->
		<div class="row">

			<!-- NEW COL START -->
			<article class="col-sm-12 col-md-12 col-lg-12">
				
				<!-- Widget ID (each widget will need unique ID)-->
				<div class="jarviswidget well" id="wid-id-1" 
                    data-widget-editbutton="false" 
                    data-widget-custombutton="false" 
                    data-widget-deletebutton="false" 
                    data-widget-colorbutton="false">

					<header>
						<span class="widget-icon"> <i class="fa fa-edit"></i> </span>
						<h2>Manage Work Plans </h2>
				
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
                                    <section class="col col-2">
										<label class="input">Unique ID
											<input class="form-control" id="txtUID" type="text" runat="server" maxlength="50" readonly="readonly" style="background-color:#EBEBE4;">
                                        </label>
                                    </section>
								</div>
								<div class="row">
                                    <section class="col col-6">
										<label class="input">Work Plan Name
											<input class="form-control" id="txtName" type="text" runat="server" maxlength="50" required>
                                        </label>
                                    </section>
								</div>
								<div class="row">
                                    <section class="col col-6">
										<label class="input">Dataset
                                            <asp:DropDownList ID="ddlDataset" runat="server" CssClass="input-sm form-control">
                                            </asp:DropDownList>
                                            <input class="form-control" id="txtDataset" type="text" runat="server" readonly="readonly" style="background-color:#EBEBE4;">
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
                                <div class="alert alert-success fade in" runat="server" id="divstatus" name="divstatus" visible="false">
                                    <i class="fa-fw fa fa-check"></i>
                                    <strong>Success</strong> Work Plan saved successfully!&nbsp&nbsp&nbsp
                                </div>		
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
		<div class="row">

			<!-- NEW COL START -->
			<article class="col-sm-12 col-md-12 col-lg-6">
				
				<!-- Widget ID (each widget will need unique ID)-->
				<div class="jarviswidget" id="wid_id_2"  data-widget-editbutton="false" data-widget-custombutton="false" data-widget-deletebutton="false" data-widget-colorbutton="false">
					<header>
						<span class="widget-icon"> <i class="fa fa-edit"></i> </span>
						<h2>Care Events </h2>
				
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
				
							<div class="table-responsive">
								<div class="well well-sm bg-color-blueLight txt-color-white text-right" style="height:50px;">
									<label class="col-md-2 control-label">Care Event</label>
									<div class="col-md-4">
				                        <asp:DropDownList ID="ddlCareEvent" name="ddlCareEvent" runat="server" Style="color: black" CssClass="form-control">
				                        </asp:DropDownList>										
									</div>
                                    <asp:Button ID="btnAddCareEvent" runat="server" Text="Add Care Event" OnClick="btnAddCareEvent_Click" class="btn btn-default btn-sm" />
								</div>
								<asp:Table ID="dt_1" runat="server" class="table table-striped table-bordered table-hover" Width="100%">
									<asp:TableHeaderRow TableSection="TableHeader">
										<asp:TableHeaderCell Width="60%">Care Event</asp:TableHeaderCell>
										<asp:TableHeaderCell Width="20%"># Categories</asp:TableHeaderCell>
										<asp:TableHeaderCell Width="20%">Action</asp:TableHeaderCell>
									</asp:TableHeaderRow>
								</asp:Table>
							</div>  		

						</div>
						<!-- end widget content -->
				
					</div>
					<!-- end widget div -->
				
				</div>
				<!-- end widget -->
				
                <!-- Care Event Modal -->
				<div class="modal" id="careModal">
					<div class="modal-dialog">
						<div class="modal-content">
							<div class="modal-header txt-color-white">
				                <button type="button" class="close" data-dismiss="modal" aria-label="Close">
					                <span aria-hidden="true">&times;</span></button>Work Plan Care Events
							</div>
							<div class="modal-body smart-form">
								<fieldset>
                                    <legend>Details</legend>
                                    <input class="form-control" id="txtCareEventUID" type="hidden" runat="server" value="0">
									<div class="row">
                                        <section class="col col-8">
										    <label class="input">Care Event
                                                <input class="form-control" id="txtMCareEvent" type="text" maxlength="50" runat="server" value="">
                                            </label>
                                        </section>
									</div>
                                </fieldset>
							</div>
			                <div class="modal-footer">
                                <span id="divCareDelete">
    				                <asp:Button ID="btnDeleteCare" runat="server" Text="Delete" OnClick="btnDeleteCare_Click" class="btn btn-default" />
                                </span>
				                <button type="button" class="btn btn-default" data-dismiss="modal" aria-label="Close">Cancel</button>
			                </div>
						</div><!-- /.modal-content -->
					</div><!-- /.modal-dialog -->
				</div><!-- /.modal -->

			</article>
			<!-- END COL -->					

			<!-- NEW COL START -->
			<article class="col-sm-12 col-md-12 col-lg-6">
				
				<!-- Widget ID (each widget will need unique ID)-->
				<div class="jarviswidget" id="wid_id_3"  data-widget-editbutton="false" data-widget-custombutton="false" data-widget-deletebutton="false" data-widget-colorbutton="false">
					<header>
						<span class="widget-icon"> <i class="fa fa-edit"></i> </span>
						<h2>Categories </h2>
				
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
				
							<div class="table-responsive">
								<div class="well well-sm bg-color-blueLight txt-color-white text-right" style="height:50px;">
									<label class="col-md-2 control-label">Care Event</label>
									<div class="col-md-4">
				                        <asp:DropDownList ID="ddlWorkPlanCareEvent" name="ddlWorkPlanCareEvent" runat="server" Style="color: black" CssClass="form-control" OnSelectedIndexChanged="ddlWorkPlanCareEvent_SelectedIndexChanged" AutoPostBack="true">
				                        </asp:DropDownList>										
									</div>
									<div class="col-md-4">
				                        <asp:DropDownList ID="ddlCategory" name="ddlCategory" runat="server" Style="color: black" CssClass="form-control">
				                        </asp:DropDownList>										
									</div>
                                    <asp:Button ID="btnAddCategory" runat="server" Text="Add Category" OnClick="btnAddCategory_Click" class="btn btn-default btn-sm" />
								</div>
								<asp:Table ID="dt_2" runat="server" class="table table-striped table-bordered table-hover" Width="100%">
									<asp:TableHeaderRow TableSection="TableHeader">
										<asp:TableHeaderCell Width="60%">Category</asp:TableHeaderCell>
										<asp:TableHeaderCell Width="20%">Elements</asp:TableHeaderCell>
										<asp:TableHeaderCell Width="20%">Action</asp:TableHeaderCell>
									</asp:TableHeaderRow>
								</asp:Table>
							</div>  		

						</div>
						<!-- end widget content -->
				
					</div>
					<!-- end widget div -->
				
				</div>
				<!-- end widget -->
				
                <!-- Category Modal -->
				<div class="modal" id="categoryModal">
					<div class="modal-dialog">
						<div class="modal-content">
							<div class="modal-header txt-color-white">
				                <button type="button" class="close" data-dismiss="modal" aria-label="Close">
					                <span aria-hidden="true">&times;</span></button>Dataset Categories
							</div>
							<div class="modal-body smart-form">
								<fieldset>
                                    <legend>Details</legend>
                                    <input class="form-control" id="txtCategoryUID" type="hidden" runat="server" value="0">
									<div class="row">
                                        <section class="col col-8">
										    <label class="input">Category
                                                <input class="form-control" id="txtMCategory" type="text" maxlength="50" runat="server" value="">
                                            </label>
                                        </section>
									</div>
                                </fieldset>
							</div>
			                <div class="modal-footer">
                                <span id="divCategoryDelete">
        				            <asp:Button ID="btnDeleteCategory" runat="server" Text="Delete" OnClick="btnDeleteCategory_Click" class="btn btn-default" />
                                </span>
				                <button type="button" class="btn btn-default" data-dismiss="modal" aria-label="Close">Cancel</button>
			                </div>
						</div><!-- /.modal-content -->
					</div><!-- /.modal-dialog -->
				</div><!-- /.modal -->

			</article>
			<!-- END COL -->					
					
		</div>
		
	</section>
    <!-- end widget grid -->
</asp:Content>

<asp:Content runat="server" ID="Scripts" ContentPlaceHolderID="scriptsPlaceholder">
    
    <script>
        
        $('#careModal').on('show.bs.modal', function (e) {
            //get data-id attribute of the clicked element
            var id = $(e.relatedTarget).data('id');
            var evt = $(e.relatedTarget).data('evt');
            var name = $(e.relatedTarget).data('name');

            //populate modal form
            $('#txtCareEventUID').val(id);
            $('#txtMCareEvent').val(name);

            if (evt == "delete") {
                $('#txtMCareEvent').attr("disabled", true);
                $('#txtMCareEvent').css("background-color", "#EBEBE4");

                $('#divCareDelete').show();
            }
        });

        $('#categoryModal').on('show.bs.modal', function (e) {
            //get data-id attribute of the clicked element
            var id = $(e.relatedTarget).data('id');
            var name = $(e.relatedTarget).data('name');
            var evt = $(e.relatedTarget).data('evt');

            //populate modal form
            $('#txtCategoryUID').val(id);
            $('#txtMCategory').val(name);

            if (evt == "delete") {
                $('#txtMCategory').attr("disabled", true);
                $('#txtMCategory').css("background-color", "#EBEBE4");

                $('#divCategoryDelete').show();
            }

            $('#txtMCategory').attr("disabled", true);
            $('#txtMCategory').css("background-color", "#EBEBE4");
        });

	</script>

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
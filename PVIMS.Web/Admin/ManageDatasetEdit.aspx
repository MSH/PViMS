    <%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ManageDatasetEdit.aspx.cs" Inherits="PVIMS.Web.ManageDatasetEdit" MasterPageFile="~/Main.Master" Title="Manage Datasets" %>

<asp:Content runat="server" ID="Breadcrumbcontainer" ContentPlaceHolderID="breadcrumbcontainer">
    <li>Administration</li>
</asp:Content>

<asp:Content runat="server" ID="Body" ClientIDMode="Static" ContentPlaceHolderID="BodyContentPlaceHolder">

    <asp:HiddenField runat="server" ID="hfPosition" Value="" />
    <div class="row">
		<div class="col-xs-12 col-sm-7 col-md-7 col-lg-4">
			<h1 class="page-title txt-color-blueDark">
				<i class="fa fa-windows fa-fw "></i> 
					Administration 
				<span>> 
					Datasets
				</span>
			</h1>
		</div>
	</div>

	<!-- widget grid -->
	<section id="widget-grid" class="">

		<!-- row -->
		<div class="row">

			<!-- NEW COL START -->
			<article class="col-sm-12 col-md-12 col-lg-12">
				
				<!-- Widget ID (each widget will need unique ID)-->
				<div class="jarviswidget" id="wid-id-1"  data-widget-editbutton="false" data-widget-custombutton="false" data-widget-deletebutton="false" data-widget-colorbutton="false">
					<header>
						<span class="widget-icon"> <i class="fa fa-edit"></i> </span>
						<h2>Manage Datasets </h2>
				
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
										<label class="input">Unique ID
											<input class="form-control" id="txtUID" type="text" runat="server" maxlength="50" readonly="readonly" style="background-color:#EBEBE4;">
                                        </label>
                                    </section>
								</div>
								<div class="row">
                                    <section class="col col-8">
										<label class="input">Dataset Name
											<input class="form-control" id="txtName" type="text" runat="server" maxlength="50" required>
                                        </label>
                                    </section>
								</div>
								<div class="row">
                                    <section class="col col-8">
										<label class="input">Help
											<textarea class="form-control" id="txtHelp" runat="server" maxlength="250" rows="4"/>
                                        </label>
                                    </section>
								</div>
								<div class="row" id="divContext" runat="server">
                                    <section class="col col-4">
										<label class="input">Context Type
											<input class="form-control" id="txtContextType" type="text" runat="server" maxlength="50" readonly="readonly" style="background-color:#EBEBE4;">
                                        </label>
                                    </section>
								</div>
							</fieldset>

                            <div id="divRules" runat="server">
								<fieldset>
                                    <legend>Rules</legend>
									<div class="row">
                                        <section class="col col-8">
										    <label class="input">Mandatory Fields Prominent
                                                <asp:DropDownList ID="ddlMandatoryFieldRule" name="ddlMandatoryFieldRule" runat="server" Style="color: black" class="form-control">
                                                    <asp:ListItem Text ="Yes" Value="Y" Selected="True"></asp:ListItem>
                                                    <asp:ListItem Text ="No" Value="N"></asp:ListItem>
                                                </asp:DropDownList>
                                            </label>
                                        </section>
									</div>
								</fieldset>
                            </div>

                            <br />
                            <div id="divAudit" runat="server">
								<fieldset>
                                    <legend>Audit</legend>
									<div class="row">
                                        <section class="col col-8">
										    <label class="input">Created...
											    <input class="form-control" id="txtCreated" type="text" readonly="readonly" runat="server" style="background-color:#EBEBE4;">
                                            </label>
                                        </section>
									</div>
									<div class="row">
                                        <section class="col col-8">
										    <label class="input">Updated...
											    <input class="form-control" id="txtUpdated" type="text" readonly="readonly" runat="server" style="background-color:#EBEBE4;">
                                            </label>
                                        </section>
									</div>
								</fieldset>
                            </div>

                            <br />
                            <div class="smart-form">
							    <footer>
								    <span id="spnButtons" runat="server">

								    </span>
							    </footer>
                                <div class="alert alert-success fade in" runat="server" id="divstatus" name="divstatus" visible="false">
                                    <i class="fa-fw fa fa-check"></i>
                                    <strong>Success</strong> Dataset saved successfully!&nbsp&nbsp&nbsp
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
				
							<div class="well well-sm" style="height:auto; padding-top:10px; padding-left:10px; padding-right:10px; padding-bottom:0px;">
                                <div class="row">
                                    <div class="smart-form"  style="height:50px;">
                                        <section class="col col-6">
                                            <label runat="server" id="lblCategoryName" class="input">
                                                <input type="text" class="form-control" id="txtCategoryName" name="txtCategoryName" placeholder="Category Name" runat="server" style="color:black;" maxlength="50">
                                            </label>
                                        </section>
                                        <section class="col col-4"></section>
                                        <section class="col col-2">
                                            <asp:Button ID="btnAddCategory" runat="server" Text="Add Category" OnClick="btnAddCategory_Click" class="btn btn-default btn-sm" />
                                        </section>
                                    </div>
                                </div>

								<asp:Table ID="dt_1" runat="server" class="table table-striped table-bordered table-hover" Width="100%">
									<asp:TableHeaderRow TableSection="TableHeader">
										<asp:TableHeaderCell Width="50%">Category</asp:TableHeaderCell>
										<asp:TableHeaderCell Width="20%">Elements</asp:TableHeaderCell>
                                        <asp:TableHeaderCell Width="10%">Order</asp:TableHeaderCell>
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
									<div class="row">
                                        <section class="col col-8">
										    <label class="input">Friendly Name
                                                <input class="form-control" id="txtMCategoryFriendlyName" type="text" maxlength="50" runat="server" value="">
                                            </label>
                                        </section>
									</div>
								    <div class="row">
                                        <section class="col col-8">
										    <label class="input">Help
											    <textarea class="form-control" id="txtMCategoryHelp" runat="server" maxlength="350" rows="4"/>
                                            </label>
                                        </section>
									</div>

									<div class="row">
                                        <section class="col col-8">
										    <label class="input">Acute
											    <select id="ddlCatAcute" name="ddlCatAcute" class="input-sm form-control"  runat="server">
												    <option value="No" selected="selected">No</option>
												    <option value="Yes">Yes</option>
											    </select>
                                            </label>
                                        </section>
									</div>
									<div class="row">
                                        <section class="col col-8">
										    <label class="input">Chronic
											    <select id="ddlCatChronic" name="ddlCatChronic" class="input-sm form-control"  runat="server" onchange="CheckCatCondition()">
												    <option value="No" selected="selected">No</option>
												    <option value="Yes">Yes</option>
											    </select>
                                            </label>
                                        </section>
									</div>
									<div class="row" id="divCatConditions">
                                        <section class="col col-8">
										    <label class="input">Conditions
                                                <asp:ListBox runat="server" ID="lbCatCondition" class="form-control" Rows="8" SelectionMode="Multiple" Height="150">

                                                </asp:ListBox>
                                            </label>
                                        </section>
									</div>
                                </fieldset>
							</div>
			                <div class="modal-footer">
                                <span id="divCategorySave">
    				                <asp:Button ID="btnSaveCategory" runat="server" Text="Save" OnClick="btnSaveCategory_Click" class="btn btn-default" />
                                </span>
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

			<!-- NEW COL START -->
			<article class="col-sm-12 col-md-12 col-lg-6">
				
				<!-- Widget ID (each widget will need unique ID)-->
				<div class="jarviswidget" id="wid_id_3"  data-widget-editbutton="false" data-widget-custombutton="false" data-widget-deletebutton="false" data-widget-colorbutton="false">
					<header>
						<span class="widget-icon"> <i class="fa fa-edit"></i> </span>
						<h2>Elements </h2>
				
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

							<div class="well well-sm" style="height:auto; padding-top:10px; padding-left:10px; padding-right:10px; padding-bottom:0px;">
                                <div class="row">
                                    <div class="smart-form"  style="height:50px;">
                                        <section class="col col-5">
                                            <label runat="server" id="lblElementCategory" class="input">
				                                <asp:DropDownList ID="ddlCategory" name="ddlCategory" runat="server" Style="color: black" CssClass="form-control" OnSelectedIndexChanged="ddlCategory_SelectedIndexChanged" AutoPostBack="true">
				                                </asp:DropDownList>										
                                            </label>
                                        </section>
                                        <section class="col col-5">
                                            <label runat="server" id="lblElement" class="input">
				                                <asp:DropDownList ID="ddlElement" name="ddlElement" runat="server" Style="color: black" CssClass="form-control">
				                                </asp:DropDownList>										
                                            </label>
                                        </section>
                                        <section class="col col-2">
                                            <asp:Button ID="btnAddElement" runat="server" Text="Add Element" OnClick="btnAddElement_Click" class="btn btn-default btn-sm" />
                                        </section>
                                    </div>
                                </div>

								<asp:Table ID="dt_2" runat="server" class="table table-striped table-bordered table-hover" Width="100%">
									<asp:TableHeaderRow TableSection="TableHeader">
										<asp:TableHeaderCell Width="70%">Element</asp:TableHeaderCell>
                                        <asp:TableHeaderCell Width="10%">Order</asp:TableHeaderCell>
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
				
                <!-- Element Modal -->
				<div class="modal" id="elementModal">
					<div class="modal-dialog">
						<div class="modal-content">
							<div class="modal-header txt-color-white">
				                <button type="button" class="close" data-dismiss="modal" aria-label="Close">
					                <span aria-hidden="true">&times;</span></button>Dataset Elements
							</div>
							<div class="modal-body smart-form">
                                <div class="alert alert-danger alert-block" id="divWarning" runat="server" style="display:none;">
                                    <strong>Please note!</strong> Unable to delete this record as there are existing mappings....
                                </div>
								<fieldset>
                                    <legend>Details</legend>
                                    <input class="form-control" id="txtElementUID" type="hidden" runat="server" value="0">
									<div class="row">
                                        <section class="col col-8">
										    <label class="input">Element
                                                <input class="form-control" id="txtMElement" type="text" maxlength="50" runat="server" value="">
                                            </label>
                                        </section>
									</div>
									<div class="row">
                                        <section class="col col-8">
										    <label class="input">Friendly Name
                                                <input class="form-control" id="txtMElementFriendlyName" type="text" maxlength="50" runat="server" value="">
                                            </label>
                                        </section>
									</div>
								    <div class="row">
                                        <section class="col col-8">
										    <label class="input">Help
											    <textarea class="form-control" id="txtMElementHelp" runat="server" maxlength="350" rows="4"/>
                                            </label>
                                        </section>
									</div>

									<div class="row">
                                        <section class="col col-8">
										    <label class="input">Acute
											    <select id="ddlAcute" name="ddlAcute" class="input-sm form-control"  runat="server">
												    <option value="No" selected="selected">No</option>
												    <option value="Yes">Yes</option>
											    </select>
                                            </label>
                                        </section>
									</div>
									<div class="row">
                                        <section class="col col-8">
										    <label class="input">Chronic
											    <select id="ddlChronic" name="ddlChronic" class="input-sm form-control"  runat="server" onchange="CheckCondition()">
												    <option value="No" selected="selected">No</option>
												    <option value="Yes">Yes</option>
											    </select>
                                            </label>
                                        </section>
									</div>
									<div class="row" id="divConditions">
                                        <section class="col col-8">
										    <label class="input">Conditions
                                                <asp:ListBox runat="server" ID="lbCondition" class="form-control" Rows="8" SelectionMode="Multiple" Height="150">

                                                </asp:ListBox>
                                            </label>
                                        </section>
									</div>
                                </fieldset>
							</div>
			                <div class="modal-footer">
                                <span id="divElementSave">
        				            <asp:Button ID="btnSaveElement" runat="server" Text="Save" OnClick="btnSaveElement_Click" class="btn btn-default" />
                                </span>
                                <span id="divElementDelete">
        				            <asp:Button ID="btnDeleteElement" runat="server" Text="Delete" OnClick="btnDeleteElement_Click" class="btn btn-default" />
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

    <script>
        
        $('#categoryModal').on('show.bs.modal', function (e)
        {
            //get data-id attribute of the clicked element
            var id = $(e.relatedTarget).data('id');
            var evt = $(e.relatedTarget).data('evt');
            var name = $(e.relatedTarget).data('name');
            var acute = $(e.relatedTarget).data('acute');
            var chronic = $(e.relatedTarget).data('chronic');
            var conditions = $(e.relatedTarget).data('conditions');
            var friendlyname = $(e.relatedTarget).data('friendlyname');
            var help = $(e.relatedTarget).data('help');

            //populate modal form
            $('#txtCategoryUID').val(id);
            $('#txtMCategory').val(name);
            $('#txtMCategoryFriendlyName').val(friendlyname);
            $('#txtMCategoryHelp').val(help);

            $('#ddlCatAcute').val(acute);
            $('#ddlCatChronic').val(chronic);

            if (chronic == "Yes") { $('#divCatConditions').show(); } else { $('#divCatConditions').hide(); }

            // Initialise all conditions
            $("#lbCatCondition option").prop("selected", false);

            // Load conditions
            if (conditions != "")
            {
                if (conditions.toString().indexOf(",") == -1)
                {
                    $("#lbCatCondition option[value='" + conditions + "']").prop("selected", true);
                }
                else
                {
                    var array = conditions.split(',');
                    for (i = 0; i < array.length; i++)
                    {
                        $("#lbCatCondition option[value='" + array[i] + "']").prop("selected", true);
                    }
                }
            }

            if (evt == "edit")
            {
                $('#divCategoryDelete').hide();
                $('#divCategorySave').show();
            }
            if (evt == "delete")
            {
                $('#txtMCategory').attr("disabled", true);
                $('#txtMCategory').css("background-color", "#EBEBE4");

                $('#txtMCategoryFriendlyName').attr("disabled", true);
                $('#txtMCategoryFriendlyName').css("background-color", "#EBEBE4");

                $('#txtMCategoryHelp').attr("disabled", true);
                $('#txtMCategoryHelp').css("background-color", "#EBEBE4");

                $('#ddlCatAcute').attr("disabled", true);
                $('#ddlCatAcute').css("background-color", "#EBEBE4");

                $('#ddlCatChronic').attr("disabled", true);
                $('#ddlCatChronic').css("background-color", "#EBEBE4");

                $('#lbCatCondition').attr("disabled", true);
                $('#lbCatCondition').css("background-color", "#EBEBE4");

                $('#divCategorySave').hide();
                $('#divCategoryDelete').show();
            }
        });

        $('#elementModal').on('show.bs.modal', function (e)
        {
            //get data-id attribute of the clicked element
            var id = $(e.relatedTarget).data('id');
            var name = $(e.relatedTarget).data('name');
            var evt = $(e.relatedTarget).data('evt');
            var acute = $(e.relatedTarget).data('acute');
            var chronic = $(e.relatedTarget).data('chronic');
            var warning = $(e.relatedTarget).data('warning');
            var conditions = $(e.relatedTarget).data('conditions');
            var friendlyname = $(e.relatedTarget).data('friendlyname');
            var help = $(e.relatedTarget).data('help');

            //populate modal form
            $('#txtElementUID').val(id);
            $('#txtMElement').val(name);
            $('#txtMElementFriendlyName').val(friendlyname);
            $('#txtMElementHelp').val(help);

            $('#ddlAcute').val(acute);
            $('#ddlChronic').val(chronic);

            if (chronic == "Yes")
            {
                $('#divConditions').show();
            }
            else
            {
                $('#divConditions').hide();
            }

            // Initialise all conditions
            $("#lbCondition option").prop("selected", false);

            // Load conditions
            if (conditions != "") {
                if (conditions.toString().indexOf(",") == -1) {
                    $("#lbCondition option[value='" + conditions + "']").prop("selected", true);
                }
                else {
                    var array = conditions.split(',');
                    for (i = 0; i < array.length; i++) {
                        $("#lbCondition option[value='" + array[i] + "']").prop("selected", true);
                    }
                }
            }

            if (evt == "edit")
            {
                $('#divElementDelete').hide();
                $('#divElementSave').show();
            }
            if (evt == "delete")
            {
                $('#txtMCategory').attr("disabled", true);
                $('#txtMCategory').css("background-color", "#EBEBE4");

                $('#txtMElementFriendlyName').attr("disabled", true);
                $('#txtMElementFriendlyName').css("background-color", "#EBEBE4");

                $('#txtMElementHelp').attr("disabled", true);
                $('#txtMElementHelp').css("background-color", "#EBEBE4");

                $('#ddlAcute').attr("disabled", true);
                $('#ddlAcute').css("background-color", "#EBEBE4");

                $('#ddlChronic').attr("disabled", true);
                $('#ddlChronic').css("background-color", "#EBEBE4");

                $('#lbCondition').attr("disabled", true);
                $('#lbCondition').css("background-color", "#EBEBE4");

                if (warning == "Yes")
                {
                    $('#divElementDelete').hide();
                    $('#divWarning').show();
                }
                else
                {
                    $('#divElementDelete').show();
                    $('#divWarning').hide();
                }
                $('#divElementSave').hide();
            }

            $('#txtMElement').attr("disabled", true);
            $('#txtMElement').css("background-color", "#EBEBE4");
        });

	</script>

    <script>
        function CheckCondition()
        {
            if ($('#ddlChronic').val() == "Yes") {
                $('#divConditions').show();
            }
            else {
                $('#divConditions').hide();
            }
        }
        function CheckCatCondition() {
            if ($('#ddlCatChronic').val() == "Yes") {
                $('#divCatConditions').show();
            }
            else {
                $('#divCatConditions').hide();
            }
        }
    </script>

</asp:Content>
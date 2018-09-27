    <%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ManageDatasetElementEdit.aspx.cs" Inherits="PVIMS.Web.ManageDatasetElementEdit" MasterPageFile="~/Main.Master" Title="Manage Dataset Elements" %>

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
				<div class="jarviswidget well" id="wid-id-1" 
                    data-widget-editbutton="false" 
                    data-widget-custombutton="false" 
                    data-widget-deletebutton="false" 
                    data-widget-colorbutton="false">

					<header>
						<span class="widget-icon"> <i class="fa fa-edit"></i> </span>
						<h2>Manage Dataset Elements </h2>
				
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
				
                            <div class="alert alert-danger alert-block" id="divSystem" runat="server" style="display:none;">
                                <strong>Please note!</strong> This is a system element and cannot be modified...
                            </div>
							<ul class="nav nav-tabs bordered">
								<li id="liElement" runat="server" class="active">
									<a href="#tab1" data-toggle="tab" aria-expanded="true">Element</a>
								</li>
								<li id="liCategories" runat="server">
									<a href="#tab2" data-toggle="tab" aria-expanded="true">Categories</a>
								</li>								
								<li id="liValues" runat="server">
									<a href="#tab3" data-toggle="tab" aria-expanded="true">Values</a>
								</li>								
							</ul>
                            <div class="tab-content">

                                <div id="tab1" class="tab-pane active smart-form"  runat="server">
                                    <div id="divElementDetail" runat="server" visible ="false">
							            <fieldset>
                                            <legend>Table Element</legend>
								            <div class="row">
                                                <section class="col col-4">
										            <label class="input">Unique ID
											            <input class="form-control" id="txtMainElementID" type="text" runat="server" maxlength="50" readonly="readonly" style="background-color:#92A2A8; color:white;">
                                                    </label>
                                                </section>
								            </div>
								            <div class="row">
                                                <section class="col col-8">
										            <label class="input">Description
											            <input class="form-control" id="txtMainElement" type="text" runat="server" maxlength="50" readonly="readonly" style="background-color:#92A2A8; color:white;">
                                                    </label>
                                                </section>
								            </div>
							            </fieldset>
                                    </div>

							        <fieldset>
                                        <legend>Details</legend>
								        <div class="row">
                                            <section class="col col-4">
										        <label class="input">Unique ID
											        <input class="form-control" id="txtUID" type="text" runat="server" maxlength="50" readonly="readonly" style="background-color:#EBEBE4;">
                                                </label>
                                            </section>
								        </div>
								        <div class="row">
                                            <section class="col col-8">
										        <label class="input">Element Name
											        <input class="form-control" id="txtDescription" type="text" runat="server" maxlength="100" required>
                                                </label>
                                            </section>
								        </div>
                                        <div id="divSubElement" runat="server" style="display:none;">
								            <div class="row">
                                                <section class="col col-8">
										            <label class="input">Friendly Name
											            <input class="form-control" id="txtFriendlyName" type="text" runat="server" maxlength="150">
                                                    </label>
                                                </section>
								            </div>
								            <div class="row">
                                                <section class="col col-8">
										            <label class="input">Help
											            <input class="form-control" id="txtHelp" type="text" runat="server" maxlength="350">
                                                    </label>
                                                </section>
								            </div>
                                        </div>
								        <div class="row">
                                            <section class="col col-8">
										        <label class="input">OID
											        <input class="form-control" id="txtOID" type="text" runat="server" maxlength="50" required>
                                                </label>
                                            </section>
								        </div>
								        <div class="row">
                                            <section class="col col-8">
										        <label class="input">Default Value
											        <input class="form-control" id="txtDefaultValue" type="text" runat="server" maxlength="250">
                                                </label>
                                            </section>
								        </div>
								        <div class="row">
                                            <section class="col col-4">
										        <label class="input">Mandatory
											        <select id="ddlMandatory" name="ddlMandatory" class="input-sm form-control"  runat="server">
												        <option value="No" selected="selected">No</option>
												        <option value="Yes">Yes</option>
											        </select>
                                                </label>
                                            </section>
                                            <section class="col col-4">
										        <label class="input">Anonymised
											        <select id="ddlAnon" name="ddlAnon" class="input-sm form-control"  runat="server">
												        <option value="No" selected="selected">No</option>
												        <option value="Yes">Yes</option>
											        </select>
                                                </label>
                                            </section>
								        </div>
								        <div class="row">
                                            <section class="col col-4">
										        <label class="input">System
											        <select id="ddlSystem" name="ddlSystem" class="input-sm form-control"  runat="server">
												        <option value="No" selected="selected">No</option>
												        <option value="Yes">Yes</option>
											        </select>
                                                </label>
                                            </section>
								        </div>
							        </fieldset>

								    <fieldset>
                                        <legend>Rules</legend>
									    <div class="row">
                                            <section class="col col-8">
										        <label class="input">Element Can Only Link To Single Dataset
                                                    <asp:DropDownList ID="ddlSingleDatasetRule" name="ddlSingleDatasetRule" runat="server" Style="color: black" class="form-control">
                                                        <asp:ListItem Text ="Yes" Value="Y" Selected="True"></asp:ListItem>
                                                        <asp:ListItem Text ="No" Value="N"></asp:ListItem>
                                                    </asp:DropDownList>
                                                </label>
                                            </section>
									    </div>
								    </fieldset>

							        <fieldset>
                                        <legend>Field Type</legend>
								        <div class="row">
                                            <section class="col col-3">
										        <label class="input">Field Type
                                                    <asp:DropDownList ID="ddlFieldType" runat="server" CssClass="input-sm form-control" AutoPostBack="true" OnSelectedIndexChanged="SelectedIndexChanged">
                                                    </asp:DropDownList>
                                                </label>
                                            </section>
                                        </div>
							        </fieldset>

                                    <div id="divAlpha" class="smart-form"  runat="server">
                                        <fieldset>
                                            <legend>AlphaNumeric</legend>
								            <div class="row">
                                                <section class="col col-3">
										            <label class="input">Maximum Length
											            <input class="form-control" id="txtMaxLength" type="number" runat="server" maxlength="3" required>
                                                    </label>
                                                </section>
								            </div>
                                        </fieldset>
                                    </div>

                                    <div id="divNumeric" class="smart-form"  runat="server">
                                        <fieldset>
                                            <legend>Numeric</legend>
								            <div class="row">
                                                <section class="col col-3">
										            <label class="input">Decimals
											            <input class="form-control" id="txtDecimal" type="number" runat="server" maxlength="1" required>
                                                    </label>
                                                </section>
								            </div>
								            <div class="row">
                                                <section class="col col-3">
										            <label class="input">Minimum
											            <input class="form-control" id="txtMinimum" type="number" runat="server" maxlength="6" required>
                                                    </label>
                                                </section>
								            </div>
								            <div class="row">
                                                <section class="col col-3">
										            <label class="input">Maximum
											            <input class="form-control" id="txtMaximum" type="number" runat="server" maxlength="6" required>
                                                    </label>
                                                </section>
								            </div>
                                        </fieldset>
                                    </div>
                                </div>

								<div id="tab2" class="tab-pane" runat="server">
                                    <br />
                                    <div class="alert alert-warning fade in" runat="server" id="divCategoryMessage" name="divCategoryMessage" visible="false">
                                        There are no categories currently associated to this data element...
                                    </div>
						            <div class="table-responsive">
							            <asp:Table ID="dt_4" runat="server" class="table table-striped table-bordered table-hover" Width="100%">
								            <asp:TableHeaderRow TableSection="TableHeader">
									            <asp:TableHeaderCell Width="50%">Dataset</asp:TableHeaderCell>
									            <asp:TableHeaderCell Width="50%">Category</asp:TableHeaderCell>
								            </asp:TableHeaderRow>
							            </asp:Table>
						            </div>
								</div>

								<div id="tab3" class="tab-pane" runat="server">
                                    <br />
                                    <div class="alert alert-warning fade in" runat="server" id="divValueMessage" name="divValueMessage" visible="false">
                                        There are no data values currently associated to this data element...
                                    </div>
						            <div class="table-responsive">
							            <asp:Table ID="dt_basic" runat="server" class="table table-striped table-bordered table-hover" Width="100%">
								            <asp:TableHeaderRow TableSection="TableHeader">
									            <asp:TableHeaderCell Width="100%">Value</asp:TableHeaderCell>
                                                <asp:TableHeaderCell Width="100%">Count</asp:TableHeaderCell>
								            </asp:TableHeaderRow>
							            </asp:Table>
						            </div>
								</div>

                            </div>
                            <br />
                            <div class="smart-form">
							    <footer>
								    <span id="spnButtons" runat="server">

								    </span>
							    </footer>
                                <div class="alert alert-success fade in" runat="server" id="divstatus" name="divstatus" visible="false">
                                    <strong>Success</strong> Dataset element saved successfully!&nbsp&nbsp&nbsp
                                </div>		
                                <div class="alert alert-danger alert-block" runat="server" id="divError" name="diverror" visible="false">
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
		
        <div class="row">

            <article class="col-sm-12 col-md-12 col-lg-9">
				
				<!-- Widget ID (each widget will need unique ID)-->
				<div class="jarviswidget" id="wid_id_2"  data-widget-editbutton="false" data-widget-custombutton="false" data-widget-deletebutton="false" data-widget-colorbutton="false" runat="server">
					<header>
						<span class="widget-icon"> <i class="fa fa-edit"></i> </span>
						<h2>Element Values </h2>		
                        		
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
				            <legend>Values</legend>
							<div class="well well-sm text-right">
                                <span id="spnValueButtons" runat="server" >

                                </span>
							</div>
                            <div id="divDropDownList" runat="server">
						        <div class="table-responsive">
							        <asp:Table ID="dt_1" runat="server" class="table table-striped table-bordered table-hover" Width="100%">
								        <asp:TableHeaderRow TableSection="TableHeader">
									        <asp:TableHeaderCell Width="60%">Value</asp:TableHeaderCell>
									        <asp:TableHeaderCell Width="20%">Default</asp:TableHeaderCell>
									        <asp:TableHeaderCell Width="20%">Action</asp:TableHeaderCell>
								        </asp:TableHeaderRow>
							        </asp:Table>
						        </div>
                            </div>

                            <div id="divListBox" runat="server">
						        <div class="table-responsive">
							        <asp:Table ID="dt_2" runat="server" class="table table-striped table-bordered table-hover" Width="100%">
								        <asp:TableHeaderRow TableSection="TableHeader">
									        <asp:TableHeaderCell Width="80%">Value</asp:TableHeaderCell>
									        <asp:TableHeaderCell Width="20%">Action</asp:TableHeaderCell>
								        </asp:TableHeaderRow>
							        </asp:Table>
						        </div>
                            </div>

                            <div id="divTable" class="tab-pane"  runat="server">
						        <div class="table-responsive">
							        <asp:Table ID="dt_3" runat="server" class="table table-striped table-bordered table-hover" Width="100%">
								        <asp:TableHeaderRow TableSection="TableHeader">
									        <asp:TableHeaderCell Width="35%">Element</asp:TableHeaderCell>
									        <asp:TableHeaderCell Width="35%">Field Type</asp:TableHeaderCell>
                                            <asp:TableHeaderCell Width="10%">Order</asp:TableHeaderCell>
									        <asp:TableHeaderCell Width="20%">Action</asp:TableHeaderCell>
								        </asp:TableHeaderRow>
							        </asp:Table>
						        </div>
                            </div>
						</div>
						<!-- end widget content -->
					</div>
					<!-- end widget div -->
				</div>
				<!-- end widget -->
				
                <!-- Value Modal -->
				<div class="modal" id="valueModal" runat="server">
					<div class="modal-dialog">
						<div class="modal-content">
							<div class="modal-header txt-color-white">
				                <button type="button" class="close" data-dismiss="modal" aria-label="Close">
					                <span aria-hidden="true">&times;</span></button>Element Values
							</div>
							<div class="modal-body smart-form">
								<fieldset>
                                    <legend>Details</legend>
                                    <input class="form-control" id="txtValueUID" type="hidden" runat="server" value="0">
                                    <input class="form-control" id="txtDefaultO" type="hidden" runat="server" value="No">
                                    <input class="form-control" id="txtOtherO" type="hidden" runat="server" value="No">
                                    <input class="form-control" id="txtUnknownO" type="hidden" runat="server" value="No">
                                    <input class="form-control" id="txtInUse" type="hidden" runat="server" value="No">
									<div class="row">
                                        <section class="col col-8">
										    <label class="input">Value
                                                <input class="form-control" id="txtValue" type="text" maxlength="100" runat="server" value="" required>
                                            </label>
                                        </section>
									</div>
									<div class="row" id="divDefault">
                                        <section class="col col-3">
										    <label class="input">Default
                                                <asp:DropDownList ID="ddlValueDefault" name="ddlValueDefault" runat="server" style="color:black" class="form-control">
                                                    <asp:ListItem Text="No" Value="No" Selected="True"></asp:ListItem>
                                                    <asp:ListItem Text="Yes" Value="Yes"></asp:ListItem>
                                                </asp:DropDownList>
                                            </label>
                                        </section>
									</div>
									<div class="row">
                                        <section class="col col-3">
										    <label class="input">Other
                                                <asp:DropDownList ID="ddlValueOther" name="ddlValueOther" runat="server" style="color:black" class="form-control">
                                                    <asp:ListItem Text="No" Value="No" Selected="True"></asp:ListItem>
                                                    <asp:ListItem Text="Yes" Value="Yes"></asp:ListItem>
                                                </asp:DropDownList>
                                            </label>
                                        </section>
									</div>
									<div class="row">
                                        <section class="col col-3">
										    <label class="input">Unknown
                                                <asp:DropDownList ID="ddlValueUnknown" name="ddlValueUnknown" runat="server" style="color:black" class="form-control">
                                                    <asp:ListItem Text="No" Value="No" Selected="True"></asp:ListItem>
                                                    <asp:ListItem Text="Yes" Value="Yes"></asp:ListItem>
                                                </asp:DropDownList>
                                            </label>
                                        </section>
									</div>
									<div class="row">
                                        <section class="col col-12">
										    <label class="input state-error" id="lblValueError">
                                            </label>
                                        </section>
									</div>
                                </fieldset>
							</div>
			                <div class="modal-footer">
				                <button type="button" class="btn btn-default" data-dismiss="modal" aria-label="Close">Cancel</button>
                                <span id="divValueSave">
    				                <asp:Button ID="btnSaveValue" runat="server" Text="Save" OnClick="btnSaveValue_Click" class="btn btn-default" />
                                </span>
                                <span id="divValueDelete">
    				                <asp:Button ID="btnDeleteValue" runat="server" Text="Delete" OnClick="btnDeleteValue_Click" class="btn btn-default" />
                                </span>
			                </div>
						</div><!-- /.modal-content -->
					</div><!-- /.modal-dialog -->
				</div><!-- /.modal -->
			</article>

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
        
        $('#valueModal').on('show.bs.modal', function (e)
        {
            //get data-id attribute of the clicked element
            var id = $(e.relatedTarget).data('id');
            var evt = $(e.relatedTarget).data('evt');
            var typ = $(e.relatedTarget).data('typ');
            var value = $(e.relatedTarget).data('value');
            var def = $(e.relatedTarget).data('default');
            var defo = $(e.relatedTarget).data('defaulto');
            var other = $(e.relatedTarget).data('other');
            var othero = $(e.relatedTarget).data('othero');
            var unknown = $(e.relatedTarget).data('unknown');
            var unknowno = $(e.relatedTarget).data('unknowno');
            var inuse = $(e.relatedTarget).data('inuse');

            //populate modal form
            $('#txtValueUID').val(id);
            $('#txtValue').val(value);
            $('#ddlValueDefault').val(def);
            $('#ddlValueOther').val(other);
            $('#ddlValueUnknown').val(unknown);
            $('#txtDefaultO').val(defo);
            $('#txtOtherO').val(othero);
            $('#txtUnknownO').val(unknowno);
            $('#txtInUse').val(inuse);

            $("#lblValueError").empty();

            if (typ == "Listbox")
            {
                $('#divDefault').hide();
            }

            if (evt == "add")
            {
                $('#txtValue').attr("disabled", false);
                $('#txtValue').css("background-color", "#FFFFFF");

                $('#ddlValueDefault').attr("disabled", false);
                $('#ddlValueOther').attr("disabled", false);
                $('#ddlValueUnknown').attr("disabled", false);

                $('#divValueDelete').hide();
                $('#divValueSave').show();
            }
            if (evt == "edit")
            {
                $('#txtValue').attr("disabled", true);
                $('#txtValue').css("background-color", "#EBEBE4");

                $('#ddlValueDefault').attr("disabled", false);
                $('#ddlValueOther').attr("disabled", false);
                $('#ddlValueUnknown').attr("disabled", false);

                $('#divValueDelete').hide();
                $('#divValueSave').show();
            }
            if (evt == "delete")
            {
                $('#txtValue').attr("disabled", true);
                $('#txtValue').css("background-color", "#EBEBE4");

                $('#ddlValueDefault').attr("disabled", true);
                $('#ddlValueOther').attr("disabled", true);
                $('#ddlValueUnknown').attr("disabled", true);

                $('#divValueSave').hide();
                $('#divValueDelete').show();
            }
        });

        $(function () {
            $("#btnSaveValue").click(function (e)
            {
                var def = $("#ddlValueDefault").val();
                var other = $("#ddlValueOther").val();
                var unknown = $("#ddlValueUnknown").val();

                var defo = $("#txtDefaultO").val();
                var othero = $("#txtOtherO").val();
                var unknowno = $("#txtUnknownO").val();

                var error = false;
                $("#lblValueError").empty();

                if (def == "Yes" && defo == "Yes")
                {
                    error = true;
                    $("#lblValueError").append("<div class=\"note note-error\">Default value already specified</div>");
                }
                if (other == "Yes" && othero == "Yes")
                {
                    error = true;
                    $("#lblValueError").append("<div class=\"note note-error\">Other value already specified</div>");
                }
                if (unknown == "Yes" && unknowno == "Yes")
                {
                    error = true;
                    $("#lblValueError").append("<div class=\"note note-error\">Unknown value already specified</div>");
                }

                if (error)
                {
                    $("#lblValueError").removeAttr("class");
                    $("#lblValueError").attr("class", "input state-error");
                    e.preventDefault();
                }
            })
        })

        $(function () {
            $("#btnDeleteValue").click(function (e) {
                var inuse = $("#txtInUse").val();

                var error = false;
                $("#lblValueError").empty();

                if (inuse == "Yes")
                {
                    error = true;
                    $("#lblValueError").append("<div class=\"note note-error\">Element is in use and cannot be removed</div>");
                }

                if (error)
                {
                    $("#lblValueError").removeAttr("class");
                    $("#lblValueError").attr("class", "input state-error");
                    e.preventDefault();
                }
            })
        })

	</script>

</asp:Content>
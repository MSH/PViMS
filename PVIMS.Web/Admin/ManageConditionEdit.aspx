    <%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ManageConditionEdit.aspx.cs" Inherits="PVIMS.Web.ManageConditionEdit" MasterPageFile="~/Main.Master" Title="Manage Condition Groups" %>

<asp:Content runat="server" ID="Breadcrumbcontainer" ContentPlaceHolderID="breadcrumbcontainer">
    <li>Administration</li>
</asp:Content>

<asp:Content runat="server" ID="Body" ClientIDMode="Static" ContentPlaceHolderID="BodyContentPlaceHolder">
    <div class="row">
		<div class="col-xs-12 col-sm-7 col-md-7 col-lg-4">
			<h1 class="page-title txt-color-blueDark">
				<i class="fa fa-windows fa-fw "></i> 
					Administration 
				<span>> 
					Condition Groups
				</span>
			</h1>
		</div>
	</div>

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
						<h2>Manage Condition Groups</h2>
				
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
				
							<ul class="nav nav-tabs bordered">
								<li id="liCondition" runat="server" class="active">
									<a href="#tab1" data-toggle="tab" aria-expanded="true">Condition</a>
								</li>
								<li id="liLabs" runat="server">
									<a href="#tab2" data-toggle="tab" aria-expanded="true">Lab Tests</a>
								</li>								
								<li id="liMedications" runat="server">
									<a href="#tab3" data-toggle="tab" aria-expanded="true">Medications</a>
								</li>								
								<li id="liMedDras" runat="server">
									<a href="#tab4" data-toggle="tab" aria-expanded="true">MedDra Terms</a>
								</li>								
							</ul>
                            <div class="tab-content">

                                <div id="tab1" class="tab-pane active smart-form"  runat="server">
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
										        <label class="input">Description
											        <input class="form-control" id="txtDescription" type="text" runat="server" maxlength="50" required>
                                                </label>
                                            </section>
								        </div>
								        <div class="row">
                                            <section class="col col-2">
										        <label class="input">Chronic
											        <select id="ddlChronic" name="ddlChronic" class="input-sm form-control"  runat="server">
												        <option value="No" selected="selected">No</option>
												        <option value="Yes">Yes</option>
											        </select>
                                                </label>
                                            </section>
								        </div>
							        </fieldset>
                                </div>

								<div id="tab2" class="tab-pane" runat="server">
                                    <br />
							        <div class="table-responsive">
								        <div class="well well-sm bg-color-blueLight txt-color-white text-right" style="height:50px;">
									        <label class="col-md-2 control-label">Lab Test</label>
									        <div class="col-md-4">
				                                <asp:DropDownList ID="ddlLabTest" name="ddlLabTest" runat="server" Style="color: black" CssClass="form-control" AutoPostBack="false">
                                                    <asp:ListItem Selected="True" Value="0" Text="-- Select a lab test --"></asp:ListItem>
				                                </asp:DropDownList>										
									        </div>
                                            <asp:Button ID="btnAddLabTest" runat="server" Text="Add Lab Test" OnClick="btnAddLabTest_Click" class="btn btn-default btn-sm" />
								        </div>
                                        <div class="alert alert-danger alert-block" runat="server" id="divLabErrorDuplicate" name="divLabErrorDuplicate" visible="false">
                                            <strong>Error</strong>&nbsp;&nbsp;Lab test with same name already added to this condition...
                                        </div>	

								        <asp:Table ID="dt_1" runat="server" class="table table-striped table-bordered table-hover" Width="100%">
									        <asp:TableHeaderRow TableSection="TableHeader">
										        <asp:TableHeaderCell Width="80%">Lab Test</asp:TableHeaderCell>
										        <asp:TableHeaderCell Width="20%">Action</asp:TableHeaderCell>
									        </asp:TableHeaderRow>
								        </asp:Table>
							        </div>  		
								</div>

								<div id="tab3" class="tab-pane" runat="server">
                                    <br />
							        <div class="table-responsive">
								        <div class="well well-sm bg-color-blueLight txt-color-white text-right" style="height:50px;">
									        <label class="col-md-2 control-label">Medication</label>
									        <div class="col-md-4">
				                                <asp:DropDownList ID="ddlMedication" name="ddlMedication" runat="server" Style="color: black" CssClass="form-control" AutoPostBack="false">
												    <asp:ListItem Selected="True" Value="0" Text="-- Select a medication --"></asp:ListItem>
				                                </asp:DropDownList>										
									        </div>
                                            <asp:Button ID="btnAddMedication" runat="server" Text="Add Medication" OnClick="btnAddMedication_Click" class="btn btn-default btn-sm" />
								        </div>
                                        <div class="alert alert-danger alert-block" runat="server" id="divMedErrorDuplicate" name="divMedErrorDuplicate" visible="false">
                                            <strong>Error</strong>&nbsp;&nbsp;Medication with same name already added to this condition...
                                        </div>	

								        <asp:Table ID="dt_2" runat="server" class="table table-striped table-bordered table-hover" Width="100%">
									        <asp:TableHeaderRow TableSection="TableHeader">
										        <asp:TableHeaderCell Width="80%">Medication</asp:TableHeaderCell>
										        <asp:TableHeaderCell Width="20%">Action</asp:TableHeaderCell>
									        </asp:TableHeaderRow>
								        </asp:Table>
							        </div>  		
								</div>

								<div id="tab4" class="tab-pane" runat="server">
                                    <div class="smart-form">
								        <fieldset>
									        <div class="row">
                                                <section class="col col-6">
										            <label class="input">Find by Term: 
                                                        <input class="form-control" id="txtTerm" type="text" maxlength="100" runat="server">
                                                    </label>
                                                    <div class="note">
                                                        Must select using Lowest Level Term (LLT)....
                                                    </div>
                                                </section>
									        </div>
                                            <div class="row">
                                                <section class="col col-6">
                                                    <asp:Button ID="btnSearch" runat="server" Text="Search" OnClick="btnSearch_Click" class="btn btn-primary" Width="80" Height="30" />
                                                </section>
                                            </div>
									        <div class="row">
                                                <section class="col col-6">
										            <label class="input">Results:<br />
											            <asp:ListBox ID="lstTermResult" runat="server" Width="550" Height="250">
                                                        </asp:ListBox>
                                                    </label>
                                                </section>
									        </div>
                                            <div class="row">
                                                <section class="col col-6">
                                                    <asp:Button ID="btnAddMedDra" runat="server" Text="Add" OnClick="btnAddMedDra_Click" class="btn btn-primary" Width="80" Height="30" />
                                                </section>
                                            </div>
								        </fieldset>
                                    </div>

                                    <br />
							        <div class="table-responsive">
                                        <div class="alert alert-danger alert-block" runat="server" id="divMError" name="divMError" visible="false">
                                            <span id="spnMError" runat="server"></span>
                                        </div>		

								        <asp:Table ID="dt_3" runat="server" class="table table-striped table-bordered table-hover" Width="100%">
									        <asp:TableHeaderRow TableSection="TableHeader">
										        <asp:TableHeaderCell Width="80%">MedDRA Term</asp:TableHeaderCell>
										        <asp:TableHeaderCell Width="20%">Action</asp:TableHeaderCell>
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
                                    <strong>Success</strong> Condition saved successfully!&nbsp&nbsp&nbsp
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

                <!-- Item Modal -->
				<div class="modal" id="itemModal">
					<div class="modal-dialog">
						<div class="modal-content">
							<div class="modal-header">
				                <button type="button" class="close" data-dismiss="modal" aria-label="Close">
					                <span aria-hidden="true">&times;</span></button>Condition Items
							</div>
							<div class="modal-body smart-form">
								<fieldset>
                                    <legend>Details</legend>
                                    <input class="form-control" id="txtItemUID" type="hidden" runat="server" value="0">
                                    <input class="form-control" id="txtItemType" type="hidden" runat="server" value="">
									<div class="row">
                                        <section class="col col-8">
										    <label class="input">Item
                                                <input class="form-control" id="txtItem" type="text" maxlength="100" runat="server" value="">
                                            </label>
                                        </section>
									</div>
                                </fieldset>
							</div>
			                <div class="modal-footer">
    				            <asp:Button ID="btnDeleteItem" runat="server" Text="Delete" OnClick="btnDeleteItem_Click" class="btn btn-default" />
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
        
        $('#itemModal').on('show.bs.modal', function (e) {
            //get data-id attribute of the clicked element
            var id = $(e.relatedTarget).data('id');
            var type = $(e.relatedTarget).data('type');
            var name = $(e.relatedTarget).data('name');

            //populate modal form
            $('#txtItemUID').val(id);
            $('#txtItemType').val(type);
            $('#txtItem').val(name);

            $('#txtItem').attr("disabled", true);
            $('#txtItem').css("background-color", "#EBEBE4");
        });

	</script>

</asp:Content>
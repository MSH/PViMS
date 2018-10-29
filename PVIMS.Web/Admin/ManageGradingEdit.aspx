    <%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ManageGradingEdit.aspx.cs" Inherits="PVIMS.Web.ManageGradingEdit" MasterPageFile="~/Main.Master" Title="Manage Gradings" %>

<asp:Content runat="server" ID="Breadcrumbcontainer" ContentPlaceHolderID="breadcrumbcontainer">
    <li>Administration</li>
</asp:Content>

<asp:Content runat="server" ID="Body" ClientIDMode="Static" ContentPlaceHolderID="BodyContentPlaceHolder">

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
						<h2>Manage Scale Gradings</h2>
				
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
				
                            <div class="smart-form">
							    <fieldset>
                                <legend>Details</legend>
								<div class="row">
                                    <section class="col col-2">
										<label class="input">Unique ID
											<input class="form-control" id="txtUID" type="text" runat="server" maxlength="50" readonly="readonly" style="background-color:#EBEBE4;">
                                        </label>
                                    </section>
								</div>
                                <div id="divView" runat="server">
								    <div class="row">
                                        <section class="col col-6">
										    <label class="input">Scale
                                                <input class="form-control" id="txtScale" type="text" runat="server" maxlength="100" readonly="readonly" style="background-color:#EBEBE4;">
                                            </label>
                                        </section>
								    </div>
								    <div class="row">
                                        <section class="col col-6">
										    <label class="input">MedDRA Term
                                                <input class="form-control" id="txtMeddraTerm" type="text" runat="server" maxlength="100" readonly="readonly" style="background-color:#EBEBE4;">
                                            </label>
                                        </section>
								    </div>
                                </div>
                                <div id="divAdd" runat="server">
								    <div class="row">
                                        <section class="col col-2">
										    <label class="input">Scale
											    <select id="ddlScale" name="ddlScale" class="input-sm form-control"  runat="server">
											    </select>
                                            </label>
                                        </section>
								    </div>
                                    <legend>MedDRA Event</legend>
                                    <div class="row">
                                        <section class="col col-6">
								            <fieldset>
									            <div class="row">
                                                    <section class="col col-6">
										                <label class="input">Find by Term: 
                                                            <input class="form-control" id="txtTerm" type="text" maxlength="100" runat="server">
                                                        </label>
                                                    </section>
									            </div>
									            <div class="row">
                                                    <section class="col col-6">
										                <label class="input">Term Type: 
                                                            <asp:DropDownList ID="ddlTermType" name="ddlTermType" runat="server" style="color:black" class="form-control">
                                                                <asp:ListItem Text="System Organ Class" Value="SOC"></asp:ListItem>
                                                                <asp:ListItem Text="High Level Group Term" Value="HLGT"></asp:ListItem>
                                                                <asp:ListItem Text="High Level Term" Value="HLT"></asp:ListItem>
                                                                <asp:ListItem Text="Preferred Term" Value="PT"></asp:ListItem>
                                                                <asp:ListItem Text="Lowest Level Term" Value="LLT"></asp:ListItem>
                                                            </asp:DropDownList>
                                                        </label>
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
											                <asp:ListBox ID="lstTermResult" runat="server" Width="450" Height="250">
                                                            </asp:ListBox>
                                                        </label>
                                                    </section>
									            </div>
								            </fieldset>
                                        </section>
                                    </div>
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
                                    <strong>Success</strong> Grade saved successfully!&nbsp&nbsp&nbsp
                                </div>		
                                <div class="alert alert-danger alert-block" runat="server" id="divError" name="diverror" visible="false">
                                    <span id="spnErrors" runat="server"></span>
                                </div>		
                            </div>    
                            <div id="divGrades" runat="server">
                                <legend>Grades</legend>
							    <div class="table-responsive">
								    <asp:Table ID="dt_1" runat="server" class="table table-striped table-bordered table-hover" Width="100%">
									    <asp:TableHeaderRow TableSection="TableHeader">
										    <asp:TableHeaderCell Width="40%">Grade</asp:TableHeaderCell>
                                            <asp:TableHeaderCell Width="40%">Description</asp:TableHeaderCell>
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

                <!-- Item Modal -->
				<div class="modal" id="itemModal">
					<div class="modal-dialog">
						<div class="modal-content">
							<div class="modal-header">
				                <button type="button" class="close" data-dismiss="modal" aria-label="Close">
					                <span aria-hidden="true">&times;</span></button>Grades
							</div>
							<div class="modal-body smart-form">
								<fieldset>
                                    <legend>Details</legend>
                                    <input class="form-control" id="txtItemUID" type="hidden" runat="server" value="0">
									<div class="row">
                                        <section class="col col-10">
										    <label class="input">Description
                                                <input class="form-control" id="txtDescription" type="text" maxlength="100" runat="server" value="" required>
                                            </label>
                                        </section>
									</div>
                                </fieldset>
							</div>
			                <div class="modal-footer">
    				            <asp:Button ID="btnSaveItem" runat="server" Text="Save" OnClick="btnSaveItem_Click" class="btn btn-default" />
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
            var desc = $(e.relatedTarget).data('desc');

            //populate modal form
            $('#txtItemUID').val(id);
            $('#txtDescription').val(desc);
        });

	</script>

</asp:Content>
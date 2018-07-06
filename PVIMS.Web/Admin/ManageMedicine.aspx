<%@ Page Language="C#" AutoEventWireup="True" CodeBehind="ManageMedicine.aspx.cs" Inherits="PVIMS.Web.ManageMedicine" Title="Manage Medicines" MasterPageFile="~/Main.Master" %>

<asp:Content runat="server" ID="Breadcrumbcontainer" ContentPlaceHolderID="breadcrumbcontainer">
	<li>Administration</li>
</asp:Content>

<asp:Content runat="server" ID="Body" ClientIDMode="Static" ContentPlaceHolderID="BodyContentPlaceHolder">
    
    <div class="row">
		<div class="col-xs-12 col-sm-7 col-md-7 col-lg-4">
			<h1 class="page-title txt-color-blueDark">
				<i class="fa fa-windows fa-fw "></i> 
					Medicines
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
				<div class="jarviswidget" id="wid-id-0"  data-widget-editbutton="false" data-widget-custombutton="false" data-widget-deletebutton="false" data-widget-colorbutton="false">
					<header>
						<span class="widget-icon"> <i class="fa fa-table"></i> </span>
						<h2>Medicines</h2>
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
                                <div class="alert alert-success fade in" runat="server" id="divstatus" name="divstatus" visible="false">
                                    <strong>Success</strong> Medication saved successfully!&nbsp&nbsp&nbsp
                                </div>		
                                <span id="spnbuttons" runat="server" >

                                </span>
							</div>                                        
                            <div class="alert alert-danger alert-block" runat="server" id="divErrorDuplicate" name="divErrorDuplicate" visible="false">
                                <strong>Error</strong>&nbsp;&nbsp;Medication with same name already added...
                            </div>	
                            <asp:Table id="dt_basic" runat="server" class="table table-striped table-bordered table-hover"  width="100%">
                                <asp:TableHeaderRow TableSection="TableHeader">
                                    <asp:TableHeaderCell Width="25%">Drug Name</asp:TableHeaderCell> 
                                    <asp:TableHeaderCell Width="10%">Pack Size</asp:TableHeaderCell> 
                                    <asp:TableHeaderCell Width="10%">Strength</asp:TableHeaderCell> 
                                    <asp:TableHeaderCell Width="20%">Catalog #</asp:TableHeaderCell> 
                                    <asp:TableHeaderCell Width="20%">Form</asp:TableHeaderCell> 
                                    <asp:TableHeaderCell Width="15%">Action</asp:TableHeaderCell> 
                                </asp:TableHeaderRow>
                            </asp:Table>

						</div>
						<!-- end widget content -->
				
					</div>
					<!-- end widget div -->
				
				</div>
				<!-- end widget -->
				
                <!-- Medication Modal -->
				<div class="modal" id="medicationModal">
					<div class="modal-dialog">
						<div class="modal-content">
							<div class="modal-header">
				                <button type="button" class="close" data-dismiss="modal" aria-label="Close">
					                <span aria-hidden="true">&times;</span></button>Medicines
							</div>
							<div class="modal-body smart-form">
								<fieldset>
                                    <legend>Details</legend>
                                    <input class="form-control" id="txtUID" type="hidden" runat="server" value="0">
									<div class="row">
                                        <section class="col col-6">
										    <label class="input">Drug Name
                                                <input class="form-control" id="txtDrugName" type="text" maxlength="100" runat="server" value="" required>
                                            </label>
                                        </section>
									</div>
									<div class="row">
                                        <section class="col col-3">
										    <label class="input">Pack Size
											    <input class="form-control" id="txtPackSize" maxlength="10" runat="server" required type="number" min="0" max="32000" >
                                            </label>
                                        </section>
									</div>
									<div class="row">
                                        <section class="col col-3">
										    <label class="input">Strength
											    <input class="form-control" id="txtStrength" maxlength="40" runat="server" required >
                                            </label>
                                        </section>
									</div>
									<div class="row">
                                        <section class="col col-4">
										    <label class="input">Catalog
											    <input class="form-control" id="txtCatalog" maxlength="10" runat="server" >
                                            </label>
                                        </section>
									</div>
									<div class="row">
                                        <section class="col col-6">
										    <label class="input">Form
                                                <asp:DropDownList ID="ddlMedicationForm" name="ddlMedicationForm" runat="server" style="color:black" class="form-control">
                                                </asp:DropDownList>
                                            </label>
                                        </section>
									</div>
                                </fieldset>
							</div>
			                <div class="modal-footer">
                                <span id="divMedicationSave">
    				                <asp:Button ID="btnSaveMedication" runat="server" Text="Save" OnClick="btnSaveMedication_Click" class="btn btn-default" />
                                </span>
                                <span id="divMedicationDelete">
    				                <asp:Button ID="btnDeleteMedication" runat="server" Text="Delete" OnClick="btnDeleteMedication_Click" class="btn btn-default" />
                                </span>
				                <button type="button" class="btn btn-default" data-dismiss="modal" aria-label="Close">Cancel</button>
			                </div>

						</div><!-- /.modal-content -->
					</div><!-- /.modal-dialog -->
				</div><!-- /.modal -->	

			</article>
			<!-- WIDGET END -->
				
		</div>
				
		<!-- end row -->
				
	</section>
	<!-- end widget grid -->

</asp:Content>

<asp:Content runat="server" ID="Scripts" ContentPlaceHolderID="scriptsPlaceholder">
    <script>

        $('#medicationModal').on('show.bs.modal', function (e) {
            //get data-id attribute of the clicked element
            var id = $(e.relatedTarget).data('id');
            var evt = $(e.relatedTarget).data('evt');
            var drugname = $(e.relatedTarget).data('drugname');
            var packsize = $(e.relatedTarget).data('packsize');
            var strength = $(e.relatedTarget).data('strength');
            var catalog = $(e.relatedTarget).data('catalog');
            var formId = $(e.relatedTarget).data('form');

            //populate modal form
            $('#txtUID').val(id);
            $('#txtDrugName').val(drugname);
            $('#txtPackSize').val(packsize);
            $('#txtStrength').val(strength);
            $('#txtCatalog').val(catalog);
            $('#ddlMedicationForm').val(formId);

            if (evt == "add") {
                $('#divMedicationDelete').hide();
            }
            if (evt == "edit") {
                $('#divMedicationDelete').hide();
            } if (evt == "delete") {
                $('#txtDrugName').attr("disabled", true);
                $('#txtPackSize').attr("disabled", true);
                $('#txtStrength').attr("disabled", true);
                $('#txtCatalog').attr("disabled", true);
                $('#ddlMedicationForm').attr("disabled", true);

                $('#divMedicationSave').hide();
                $('#divMedicationDelete').show();
            }
        });

	</script>

</asp:Content>



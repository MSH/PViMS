<%@ Page Language="C#" AutoEventWireup="True" CodeBehind="ManageLabResult.aspx.cs" Inherits="PVIMS.Web.ManageLabResult" Title="Manage Lab Results" MasterPageFile="~/Main.Master" %>

<asp:Content runat="server" ID="Breadcrumbcontainer" ContentPlaceHolderID="breadcrumbcontainer">
	<li>Administration</li>
</asp:Content>

<asp:Content runat="server" ID="Body" ClientIDMode="Static" ContentPlaceHolderID="BodyContentPlaceHolder">
    
    <div class="row">
		<div class="col-xs-12 col-sm-7 col-md-7 col-lg-4">
			<h1 class="page-title txt-color-blueDark">
				<i class="fa fa-windows fa-fw "></i> 
					Test Results
			</h1>
		</div>
    </div>
				
	<!-- widget grid -->
	<section id="widget-grid" class="">
				
		<!-- row -->
		<div class="row">
				
			<!-- NEW WIDGET START -->
			<article class="col-xs-12 col-sm-12 col-md-12 col-lg-6">
				
				<!-- Widget ID (each widget will need unique ID)-->
				<div class="jarviswidget" id="wid-id-0"  data-widget-editbutton="false" data-widget-custombutton="false" data-widget-deletebutton="false" data-widget-colorbutton="false">
					<header>
						<span class="widget-icon"> <i class="fa fa-table"></i> </span>
						<h2>Test Results</h2>
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
                                <strong>Success</strong> Test Result saved successfully!&nbsp&nbsp&nbsp
                            </div>		
							<div class="well well-sm bg-color-blueLight txt-color-white text-right">
                                <span id="spnbuttons" runat="server" >

                                </span>
							</div>                                        
                            <div class="alert alert-danger alert-block" runat="server" id="divErrorDuplicate" name="divErrorDuplicate" visible="false">
                                <strong>Error</strong>&nbsp;&nbsp;Test Result with same name already added...
                            </div>	
                            <asp:Table id="dt_basic" runat="server" class="table table-striped table-bordered table-hover"  width="100%">
                                <asp:TableHeaderRow TableSection="TableHeader">
                                    <asp:TableHeaderCell Width="70%">Description</asp:TableHeaderCell> 
                                    <asp:TableHeaderCell Width="30%">Action</asp:TableHeaderCell> 
                                </asp:TableHeaderRow>
                            </asp:Table>

						</div>
						<!-- end widget content -->
				
					</div>
					<!-- end widget div -->
				
				</div>
				<!-- end widget -->

                <!-- Lab Result Modal -->
				<div class="modal" id="labresultModal">
					<div class="modal-dialog">
						<div class="modal-content">
							<div class="modal-header">
				                <button type="button" class="close" data-dismiss="modal" aria-label="Close">
					                <span aria-hidden="true">&times;</span></button>Test Result
							</div>
							<div class="modal-body smart-form">
								<fieldset>
                                    <legend>Details</legend>
                                    <input class="form-control" id="txtUID" type="hidden" runat="server" value="0">
									<div class="row">
                                        <section class="col col-6">
										    <label class="input">Description
                                                <input class="form-control" id="txtDescription" type="text" maxlength="50" runat="server" value="" required>
                                            </label>
                                        </section>
									</div>
                                </fieldset>
							</div>
			                <div class="modal-footer">
                                <span id="divLabResultSave">
    				                <asp:Button ID="btnSaveLabResult" runat="server" Text="Save" OnClick="btnSaveLabResult_Click" class="btn btn-default" />
                                </span>
                                <span id="divLabResultDelete">
    				                <asp:Button ID="btnDeleteLabResult" runat="server" Text="Delete" OnClick="btnDeleteLabResult_Click" class="btn btn-default" />
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

        $('#labresultModal').on('show.bs.modal', function (e) {
            //get data-id attribute of the clicked element
            var id = $(e.relatedTarget).data('id');
            var evt = $(e.relatedTarget).data('evt');
            var name = $(e.relatedTarget).data('name');

            //populate modal form
            $('#txtUID').val(id);
            $('#txtDescription').val(name);

            if (evt == "add") {
                $('#divLabResultDelete').hide();
            }
            if (evt == "edit") {
                $('#divLabResultDelete').hide();
            } if (evt == "delete") {
                $('#txtDescription').attr("disabled", true);

                $('#divLabResultSave').hide();
                $('#divLabResultDelete').show();
            }
        });

	</script>

</asp:Content>



<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ManageHoliday.aspx.cs" Inherits="PVIMS.Web.ManageHoliday" Title="Manage Holidays" MasterPageFile="~/Main.Master" %>

<asp:Content runat="server" ID="Breadcrumbcontainer" ContentPlaceHolderID="breadcrumbcontainer">
	<li>Administration</li>
</asp:Content>

<asp:Content runat="server" ID="Body" ClientIDMode="Static" ContentPlaceHolderID="BodyContentPlaceHolder">
    			
	<!-- widget grid -->
	<section id="widget-grid" class="">
				
		<!-- row -->
		<div class="row">
				
			<!-- NEW WIDGET START -->
			<article class="col-xs-12 col-sm-12 col-md-12 col-lg-6">
				
				<!-- Widget ID (each widget will need unique ID)-->
				<div class="jarviswidget well" id="wid-id-0" 
                    data-widget-editbutton="false" 
                    data-widget-custombutton="false" 
                    data-widget-deletebutton="false" 
                    data-widget-colorbutton="false">

					<header>
						<span class="widget-icon"> <i class="fa fa-table"></i> </span>
						<h2>Holidays</h2>
				
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
						        <label class="select">
				                    <asp:DropDownList ID="ddlCriteria" name="ddlCriteria" runat="server" Style="color: black" CssClass="form-control" AutoPostBack="true" OnSelectedIndexChanged="ddlCriteria_SelectedIndexChanged">
				                    </asp:DropDownList>
						        </label>
                                &nbsp;|&nbsp;
                                <span id="spnHolidaybuttons" runat="server" >

                                </span>
							</div>                                        
                            <asp:Table id="dt_basic" runat="server" class="table table-striped table-bordered table-hover"  width="100%">
                                <asp:TableHeaderRow TableSection="TableHeader">
                                    <asp:TableHeaderCell Width="30%">Date</asp:TableHeaderCell> 
                                    <asp:TableHeaderCell Width="40%">Description</asp:TableHeaderCell> 
                                    <asp:TableHeaderCell Width="30%">Action</asp:TableHeaderCell> 
                                </asp:TableHeaderRow>
                            </asp:Table>

						</div>
						<!-- end widget content -->
				
					</div>
					<!-- end widget div -->
				
				</div>
				<!-- end widget -->
				
                <!-- Holiday Modal -->
				<div class="modal" id="holidayModal">
					<div class="modal-dialog">
						<div class="modal-content">
							<div class="modal-header">
				                <button type="button" class="close" data-dismiss="modal" aria-label="Close">
					                <span aria-hidden="true">&times;</span></button>Public Holidays
							</div>
							<div class="modal-body smart-form">
								<fieldset>
                                    <legend>Details</legend>
                                    <input class="form-control" id="txtHolidayUID" type="hidden" runat="server" value="0">
									<div class="row">
                                        <section class="col col-6">
										    <label class="input">Holiday Date
                                                <input id="txtHolidayDate" type="text" style="color: black" placeholder="yyyy-mm-dd" runat="server" class="form-control datepicker" width="250px">
                                            </label>
                                        </section>
									</div>
									<div class="row">
                                        <section class="col col-8">
										    <label class="input">Description
											    <input class="form-control" id="txtHolidayReason" maxlength="100" runat="server" required >
                                            </label>
                                        </section>
									</div>
                                </fieldset>
							</div>
			                <div class="modal-footer">
                                <span id="divHolidaySave">
    				                <asp:Button ID="btnSaveHoliday" runat="server" Text="Save" OnClick="btnSaveHoliday_Click" class="btn btn-default" />
                                </span>
                                <span id="divHolidayDelete">
    				                <asp:Button ID="btnDeleteHoliday" runat="server" Text="Delete" OnClick="btnDeleteHoliday_Click" class="btn btn-default" />
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

        $('#holidayModal').on('show.bs.modal', function (e) {
            //get data-id attribute of the clicked element
            var id = $(e.relatedTarget).data('id');
            var evt = $(e.relatedTarget).data('evt');
            var date = $(e.relatedTarget).data('date');
            var reason = $(e.relatedTarget).data('reason');

            //populate modal form
            $('#txtHolidayUID').val(id);
            $('#txtHolidayDate').val(date);
            $('#txtHolidayReason').val(reason);
            
            if (evt == "add") {
                $('#divHolidayDelete').hide();
            }
            if (evt == "edit") {
                $('#divHolidayDelete').hide();
            } if (evt == "delete") {
                $('#txtHolidayDate').attr("disabled", true);
                $('#txtHolidayReason').attr("disabled", true);

                $('#divHolidaySave').hide();
                $('#divHolidayDelete').show();
            }
        });

	</script>

</asp:Content>
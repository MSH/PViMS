<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="/Main.Master" CodeBehind="CalendarView.aspx.cs" Inherits="PVIMS.Web.CalendarView" Title="Appointment" %>

<asp:Content runat="server" ID="Body" ContentPlaceHolderID="BodyContentPlaceHolder">
				
	<!-- widget grid -->
	<section id="widget-grid" class="">

		<div class="row">
				
			<!-- NEW WIDGET START -->
			<article class="col-xs-12 col-sm-12 col-md-12 col-lg-8">
				
				<!-- Widget ID (each widget will need unique ID)-->
				<div class="jarviswidget well" id="wid-id-5"  data-widget-editbutton="false" data-widget-custombutton="false" data-widget-deletebutton="false" data-widget-colorbutton="false">
					<header>
						<span class="widget-icon"> <i class="fa fa-table"></i> </span>
						<h2>Appointments</h2>
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
                            <div class="alert alert-danger alert-block" runat="server" id="divError" name="divError" visible="false">
                                <strong>Error!</strong> Please ensure all details are captured correctly...
                            </div>
                            <div class="well well-sm" style="height:70px;">
								<div class="row">
                                    <div class="smart-form">
                                        <section class="col col-3">
                                            <label>Show appointments for: </label>
                                            <label runat="server" id="lblCurrentDate" class="input">
                                                <input id="txtCurrentDate" type="text" style="color: black" placeholder="yyyy-mm-dd" runat="server" class="form-control datepicker" width="250px">
                                            </label>
                                        </section>
                                    </div>
                                    <section class="col col-2">
										<label class="input"><br />
                                            &nbsp;&nbsp;&nbsp;
                                            <asp:Button ID="btnSubmit" runat="server" Text="Search" OnClick="btnSubmit_Click" class="btn btn-primary" />
                                            <span id="spnButtons" runat="server">

                                            </span>
                                        </label>
                                    </section>
                                </div>
                            </div>
                            <div class="alert alert-warning fade in" runat="server" id="divHoliday" name="divHoliday" visible="false">
                                <i class="fa-fw fa fa-check"></i>
                                <strong>Warning!</strong>&nbsp;&nbsp;<span id="divHolidayMessage" runat="server"></span>
                            </div>
                            <div class="alert alert-block" style="text-align:center; height:30px;">
                                <span class="label txt-color-red" id="spnNoRows" runat="server" visible="false" style="font-size: 100%;"></span>
                                <span class="label txt-color-red" id="spnRows" runat="server" visible="false" style="font-size: 100%;"></span>
                            </div>
                            <asp:Table id="dt_basic" runat="server" ClientIDMode="Static" class="table table-striped table-bordered table-hover"  width="100%">
                                <asp:TableHeaderRow TableSection="TableHeader">
                                    <asp:TableHeaderCell width="20%">Patient Name</asp:TableHeaderCell> 
                                    <asp:TableHeaderCell width="30%">Details</asp:TableHeaderCell> 
                                    <asp:TableHeaderCell width="30%">Activity</asp:TableHeaderCell> 
                                    <asp:TableHeaderCell width="20%">Action</asp:TableHeaderCell> 
                                </asp:TableHeaderRow>
                            </asp:Table>
						    <asp:Table ID="dt_basic_2" runat="server" class="table table-striped table-bordered table-hover" Width="100%" Visible="false">
							    <asp:TableHeaderRow TableSection="TableHeader">
                                    <asp:TableHeaderCell width="20%">Patient Name</asp:TableHeaderCell> 
								    <asp:TableHeaderCell Width="20%">Date</asp:TableHeaderCell>
								    <asp:TableHeaderCell Width="20%">Type</asp:TableHeaderCell>
								    <asp:TableHeaderCell Width="20%">Discharged</asp:TableHeaderCell>
								    <asp:TableHeaderCell Width="20%">Action</asp:TableHeaderCell>
							    </asp:TableHeaderRow>
						    </asp:Table>
						</div>
						<!-- end widget content -->
				
					</div>
					<!-- end widget div -->
				
				</div>
				<!-- end widget -->
				
                <!-- Appointment Modal -->
				<div class="modal" id="appointmentModal">
					<div class="modal-dialog">
						<div class="modal-content">
							<div class="modal-header">
				                <button type="button" class="close" data-dismiss="modal" aria-label="Close">
					                <span aria-hidden="true">&times;</span></button>Patient Appointments
							</div>
							<div class="modal-body smart-form">
								<fieldset>
                                    <legend>Details</legend>
                                    <input class="form-control" id="txtAppointmentUID" type="hidden" runat="server" value="0">
									<div class="row">
                                        <section class="col col-6">
										    <label class="input">Appointment Date
                                                <input id="txtAppointmentDate" type="text" style="color: black" placeholder="yyyy-mm-dd" runat="server" class="form-control datepicker" width="250px">
                                            </label>
                                        </section>
									</div>
									<div class="row">
                                        <section class="col col-8">
										    <label class="input">Reason
											    <textarea class="form-control" id="txtAppointmentReason" maxlength="250" runat="server" rows="3" />
                                            </label>
                                        </section>
									</div>
                                    <div id="divAppointmentCancellation" style="display:none;">
                                        <legend>Cancellation</legend>
                                        <br />
									    <div class="row">
                                            <section class="col col-6">
										        <label class="input">Cancelled
													<select id="ddlAppointmentCancelled" name="ddlAppointmentCancelled" class="input-sm form-control"  runat="server">
														<option value="No" selected="selected">No</option>
														<option value="Yes">Yes</option>
													</select>
                                                </label>
                                            </section>
									    </div>
									    <div class="row">
                                            <section class="col col-8">
										        <label class="input">Reason
											        <textarea class="form-control" id="txtAppointmentCancelledReason" maxlength="250" runat="server" rows="3" />
                                                </label>
                                            </section>
									    </div>
                                    </div>
                                </fieldset>
                                <div id="divAppointmentAudit" style="display:none;">
								    <fieldset>
                                        <legend>Audit</legend>
									    <div class="row">
                                            <section class="col col-8">
										        <label class="input">Created...
											        <input class="form-control" id="txtAppointmentCreated" type="text" readonly="readonly" runat="server" style="background-color:#EBEBE4;">
                                                </label>
                                            </section>
									    </div>
									    <div class="row">
                                            <section class="col col-8">
										        <label class="input">Updated...
											        <input class="form-control" id="txtAppointmentUpdated" type="text" readonly="readonly" runat="server" style="background-color:#EBEBE4;">
                                                </label>
                                            </section>
									    </div>
								    </fieldset>
                                </div>
							</div>
			                <div class="modal-footer">
                                <span id="divAppointmentDNA">
    				                <asp:Button ID="btnDNAAppointment" runat="server" Text="Did Not Arrive" OnClick="btnDNAAppointment_Click" class="btn btn-default" />
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

        $('#appointmentModal').on('show.bs.modal', function (e) {
            //get data-id attribute of the clicked element
            var id = $(e.relatedTarget).data('id');
            var evt = $(e.relatedTarget).data('evt');
            var date = $(e.relatedTarget).data('date');
            var reason = $(e.relatedTarget).data('reason');
            var cancelled = $(e.relatedTarget).data('cancelled');
            var dna = $(e.relatedTarget).data('dna');
            var cancelledreason = $(e.relatedTarget).data('cancelledreason');
            var created = $(e.relatedTarget).data('created');
            var updated = $(e.relatedTarget).data('updated');

            //populate modal form
            $('#BodyContentPlaceHolder_txtAppointmentUID').val(id);
            $('#BodyContentPlaceHolder_txtAppointmentDate').val(date);
            $('#BodyContentPlaceHolder_txtAppointmentReason').val(reason);
            if (cancelled != "") {
                $('#BodyContentPlaceHolder_ddlAppointmentCancelled').val(cancelled);
            }
            $('#BodyContentPlaceHolder_txtAppointmentCancelledReason').val(cancelledreason);
            $('#BodyContentPlaceHolder_txtAppointmentCreated').val(created);
            $('#BodyContentPlaceHolder_txtAppointmentUpdated').val(updated);

            if (evt == "dna") {
                $('#divAppointmentAudit').show();
                $('#divAppointmentCancellation').show();

                $('#txtAppointmentDate').attr("disabled", true);
                $('#txtAppointmentReason').attr("disabled", true);
                $('#ddlAppointmentCancelled').attr("disabled", true);
                $('#txtAppointmentCancelledReason').attr("disabled", true);

                $('#divAppointmentDNA').show();
            }
        });

	</script>

</asp:Content>


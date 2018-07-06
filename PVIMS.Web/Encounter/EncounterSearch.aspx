<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="EncounterSearch.aspx.cs" MasterPageFile="/Main.Master" Inherits="PVIMS.Web.EncounterSearch" Title="Encounter Search" %>

<asp:Content runat="server" ID="Body" ContentPlaceHolderID="BodyContentPlaceHolder">

	<div class="row">
		<div class="col-xs-12 col-sm-7 col-md-7 col-lg-4">
			<h1 class="page-title txt-color-blueDark">
				<i class="fa fa-home fa-fw "></i> 
				Encounters
			</h1>
		</div>
	</div>
				
	<!-- widget grid -->
	<section id="widget-grid" class="">

		<div class="row">
				
			<!-- NEW WIDGET START -->
			<article class="col-xs-12 col-sm-12 col-md-12 col-lg-12">
				
				<!-- Widget ID (each widget will need unique ID)-->
				<div class="jarviswidget" id="wid-id-5"  data-widget-editbutton="false" data-widget-custombutton="false" data-widget-deletebutton="false" data-widget-colorbutton="false">
					<header>
						<span class="widget-icon"> <i class="fa fa-table"></i> </span>
						<h2>Encounters</h2>
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
							<div class="well well-sm" style="height:auto; padding-top:10px; padding-left:10px; padding-right:10px; padding-bottom:0px;" runat="server" id="divFilter">
								<div class="row">
                                    <div class="smart-form">
                                        <section class="col col-3">
										    <label runat="server" id="lblFacility" class="select">Facility</label>
				                            <asp:DropDownList ID="ddlFacility" name="ddlFacility" runat="server" Style="color: black" CssClass="form-control">
					                            <asp:ListItem Value="" Selected="True">All Facilities</asp:ListItem>
				                            </asp:DropDownList>
                                        </section>
                                        <section class="col col-2">
										    <label runat="server" id="lblUniqueID" class="input">Unique ID
        							            <input type="text" id="txtUniqueID" name="txtUniqueID" style="color:black" placeholder="Unique ID" runat="server" class="form-control" Width="250px" />
                                            </label>
                                        </section>
                                        <section class="col col-2">
										    <label runat="server" id="lblFirstName" class="input">First Name
        							            <input type="text" id="txtFirstName" name="txtFirstName" style="color:black" placeholder="First Name" runat="server" class="form-control" />
                                            </label>
                                        </section>
                                        <section class="col col-2">
										    <label runat="server" id="lblSurname" class="input">Last Name
        							            <input type="text" id="txtSurname" name="txtSurname" style="color:black" placeholder="Last Name" runat="server" class="form-control" />
                                            </label>
                                        </section>
								    </div>
								</div>
								<div class="row">
                                    <div class="smart-form">
                                        <section class="col col-3">
										    <label class="select">Criteria</label>
				                            <asp:DropDownList ID="ddlCriteria" name="ddlCriteria" runat="server" Style="color: black" CssClass="form-control">
					                            <asp:ListItem Value="1" Selected="True">All Encounters</asp:ListItem>
					                            <asp:ListItem Value="2">All Appointments</asp:ListItem>
					                            <asp:ListItem Value="3">Appointments with missed encounter</asp:ListItem>
					                            <asp:ListItem Value="4">Appointments with Did Not Arrive Status</asp:ListItem>
					                            <asp:ListItem Value="5">Appointments with encounter</asp:ListItem>
				                            </asp:DropDownList>
                                        </section>
                                        <section class="col col-2">
										    <label runat="server" id="lblSearchFrom" class="input">Search From
        							            <input type="text" id="txtSearchFrom" name="txtSearchFrom" style="color: black" placeholder="yyyy-mm-dd" runat="server" class="form-control datepicker" width="250px" />
                                            </label>
                                        </section>
                                        <section class="col col-2">
										    <label runat="server" id="lblSearchTo" class="input">Search To
        							            <input type="text" id="txtSearchTo" name="txtSearchTo" style="color: black" placeholder="yyyy-mm-dd" runat="server" class="form-control datepicker" width="250px" />
                                            </label>
                                        </section>
                                    </div>
								</div>
                                <div class="row" id="divCustomSearch" runat="server" visible="false">
                                    <div class="smart-form">
                                        <section class="col col-2">
                                            <label runat="server" id="lblCustomAttribute" class="select">Custom Attribute</label>
                                            <asp:DropDownList ID="ddlCustomAttribute" name="ddlCustomAttribute" runat="server" Style="color: black" CssClass="form-control" OnSelectedIndexChanged="ddlCustomAttribute_SelectedIndexChanged" AutoPostBack="true" >
                                                <asp:ListItem Value="0" Selected="True">-- Select --</asp:ListItem>
                                            </asp:DropDownList>
                                        </section>
                                        <section class="col col-2">
                                            <div id="divCustomValue" runat="server" visible="false" >
                                                <label runat="server" id="lblCustomValue" class="select" visible="false">Search Value</label>
                                                <span id="spnCustomValue" runat="server"></span>
                                            </div>
                                        </section>
                                    </div>
                                </div>
                                <div class="smart-form">
								    <footer>
                                        <asp:Button ID="btnSubmit" runat="server" Text="Search" OnClick="btnSubmit_Click" class="btn btn-primary" />
                                        <span id="spnButtons" runat="server">

                                        </span>
								    </footer>
                                </div>
							</div>
                            <div class="alert alert-block" style="text-align:center; height:30px;">
                                <span class="label txt-color-red" id="spnNoRows" runat="server" visible="false" style="font-size: 100%;"></span>
                                <span class="label txt-color-red" id="spnRows" runat="server" visible="false" style="font-size: 100%;"></span>
                            </div>
                            <asp:Table id="dt_basic" runat="server" ClientIDMode="Static" class="table table-striped table-bordered table-hover"  width="100%">
                                <asp:TableHeaderRow TableSection="TableHeader">
                                    <asp:TableHeaderCell width="15%">First Name</asp:TableHeaderCell> 
                                    <asp:TableHeaderCell width="15%">Last Name</asp:TableHeaderCell> 
                                    <asp:TableHeaderCell width="30%">Facility</asp:TableHeaderCell> 
                                    <asp:TableHeaderCell width="20%">Encounter Type</asp:TableHeaderCell> 
                                    <asp:TableHeaderCell width="10%">Date</asp:TableHeaderCell> 
                                    <asp:TableHeaderCell width="10%">Action</asp:TableHeaderCell> 
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

            if (evt == "add") {
                $('#divAppointmentAudit').hide();
                $('#divAppointmentCancellation').hide();
                $('#divAppointmentDelete').hide();
                $('#divAppointmentDNA').hide();
            }
            if (evt == "edit") {
                $('#divAppointmentAudit').show();
                $('#divAppointmentCancellation').show();
                $('#divAppointmentDelete').hide();
                $('#divAppointmentDNA').hide();

                if (dna == "Yes") {
                    $('#BodyContentPlaceHolder_txtAppointmentDate').attr("disabled", true);
                    $('#BodyContentPlaceHolder_txtAppointmentReason').attr("disabled", true);
                    $('#BodyContentPlaceHolder_ddlAppointmentCancelled').attr("disabled", true);
                    $('#BodyContentPlaceHolder_txtAppointmentCancelledReason').attr("disabled", true);
                    $('#BodyContentPlaceHolder_divAppointmentSave').hide();
                }
            }
            if (evt == "delete") {
                $('#divAppointmentAudit').show();
                $('#divAppointmentCancellation').show();

                $('#BodyContentPlaceHolder_txtAppointmentDate').attr("disabled", true);
                $('#BodyContentPlaceHolder_txtAppointmentReason').attr("disabled", true);
                $('#BodyContentPlaceHolder_ddlAppointmentCancelled').attr("disabled", true);
                $('#BodyContentPlaceHolder_txtAppointmentCancelledReason').attr("disabled", true);

                $('#divAppointmentSave').hide();
                $('#divAppointmentDelete').show();
                $('#divAppointmentDNA').hide();
            }
            if (evt == "dna") {
                $('#divAppointmentAudit').show();
                $('#divAppointmentCancellation').show();

                $('#BodyContentPlaceHolder_txtAppointmentDate').attr("disabled", true);
                $('#BodyContentPlaceHolder_txtAppointmentReason').attr("disabled", true);
                $('#BodyContentPlaceHolder_ddlAppointmentCancelled').attr("disabled", true);
                $('#BodyContentPlaceHolder_txtAppointmentCancelledReason').attr("disabled", true);

                $('#divAppointmentSave').hide();
                $('#divAppointmentDelete').hide();
                $('#divAppointmentDNA').show();
            }
        });

	</script>

</asp:Content>



<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CausalityWHO.aspx.cs" MasterPageFile="/Main.Master" Inherits="PVIMS.Web.CausalityWHO" Title="Causality WHO" %>

<asp:Content runat="server" ID="Body" ContentPlaceHolderID="BodyContentPlaceHolder" ClientIDMode="Static">
    <asp:HiddenField runat="server" ID="hfPosition" Value="" />

	<!-- widget grid -->
	<section id="widget-grid" class="">

		<div class="row">

            <div class="col-sm-12 col-md-12 col-lg-8">

                <div class="well well-sm" style="height:auto; padding-top:10px; padding-left:10px; padding-right:10px; padding-bottom:0px;">
                    <div class="row">
                        <div class="smart-form">
                            <section class="col col-3">
                                <span class="label" style="padding:5px; background-color:lightgray;"> <b> Onset Date </b> </span>
                            </section>
                            <section class="col col-6">
                                <label runat="server" id="lblOnsetDate" class="input" style="padding:5px;">
                                </label>
                            </section>
                        </div>
                    </div>
                    <div class="row">
                        <div class="smart-form">
                            <section class="col col-3">
                                <span class="label" style="padding:5px; background-color:lightgray;"> <b> Facility Level MedDRA Term </b> </span>
                            </section>
                            <section class="col col-6">
                                <label runat="server" id="lblSource" class="input" style="padding:5px;">
                                </label>
                            </section>
                        </div>
                    </div>
                    <div class="row" id="divSelection" runat="server" visible="false">
                        <div class="smart-form">
                            <section class="col col-3">
                                <span class="label" style="padding:5px; background-color:lightgray;"> <b> Central Level MedDRA Term </b> </span>
                            </section>
                            <section class="col col-6">
                                <label runat="server" id="lblSelection" class="input" style="padding:5px;">
                                </label>
                            </section>
                        </div>
                    </div>
				</div>

			</div>

		</div>

        <div>

                <div class="alert alert-danger alert-block" runat="server" id="divSetTerm" name="divSetTerm" visible="false">
                    <strong>Note!</strong> Please ensure terminology is set...
                </div>

		        <div class="row" id="divDesc" runat="server" style="display:none;">

                    <div class="col-sm-12 col-md-12 col-lg-8">

                        <div class="well well-sm" style="height:auto; padding-top:10px; padding-left:10px; padding-right:10px; padding-bottom:0px;">
                            <div class="row">
                                <div class="smart-form">
                                    <section class="col col-2">
                                        <span class="label" style="padding:5px; background-color:lightgray;"> <b> Certain </b> </span>
                                    </section>
                                    <section class="col col-4">
                                        <label runat="server" id="lblUniqueID" class="input">
                                            Item 1 to 5 answered in the affirmative
                                        </label>
                                    </section>
                                    <section class="col col-2">
                                        <span class="label" style="padding:5px; background-color:lightgray;"> <b> Probable/Likely </b> </span>
                                    </section>
                                    <section class="col col-4">
                                        <label runat="server" id="Label1" class="input">
                                            Item 6 to 9 answered in the affirmative
                                        </label>
                                    </section>
                                </div>
                            </div>
                            <div class="row">
                                <div class="smart-form">
                                    <section class="col col-2">
                                        <span class="label" style="padding:5px; background-color:lightgray;"> <b> Possible </b> </span>
                                    </section>
                                    <section class="col col-4">
                                        <label runat="server" id="Label2" class="input">
                                            Item 10 to 12 answered in the affirmative
                                        </label>
                                    </section>
                                    <section class="col col-2">
                                        <span class="label" style="padding:5px; background-color:lightgray;"> <b> Unlikely </b> </span>
                                    </section>
                                    <section class="col col-4">
                                        <label runat="server" id="Label3" class="input">
                                            Item 13 to 14 answered in the affirmative
                                        </label>
                                    </section>
                                </div>
                            </div>
                            <div class="row">
                                <div class="smart-form">
                                    <section class="col col-2">
                                        <span class="label" style="padding:5px; background-color:lightgray;"> <b> Conditional </b> </span>
                                    </section>
                                    <section class="col col-4">
                                        <label runat="server" id="Label4" class="input">
                                            Item 15 to 16 answered in the affirmative
                                        </label>
                                    </section>
                                    <section class="col col-2">
                                        <span class="label" style="padding:5px; background-color:lightgray;"> <b> Unassessable </b> </span>
                                    </section>
                                    <section class="col col-4">
                                        <label runat="server" id="Label5" class="input">
                                            Item 17 to 18 answered in the affirmative
                                        </label>
                                    </section>
                                </div>
                            </div>
				        </div>

			        </div>

		        </div>

		        <div class="row" id="divMedications" runat="server" style="display:block;">

			        <!-- NEW COL START -->
			        <div class="col-sm-12 col-md-12 col-lg-8">
				
				        <!-- Widget ID (each widget will need unique ID)-->
				        <div class="jarviswidget" id="wid_id_11"  data-widget-editbutton="false" data-widget-custombutton="false" data-widget-deletebutton="false" data-widget-colorbutton="false">
					        <header>
						        <span class="widget-icon"> <i class="fa fa-edit"></i> </span>
						        <h2>Medications </h2>
				
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
								        <asp:Table ID="dt_1" runat="server" class="table table-striped table-bordered table-hover" Width="100%">
									        <asp:TableHeaderRow TableSection="TableHeader">
										        <asp:TableHeaderCell Width="30%">Medication</asp:TableHeaderCell>
										        <asp:TableHeaderCell Width="10%">Start Date</asp:TableHeaderCell>
                                                <asp:TableHeaderCell Width="10%">End Date</asp:TableHeaderCell>
										        <asp:TableHeaderCell Width="15%">Indication Type</asp:TableHeaderCell>
										        <asp:TableHeaderCell Width="15%">Causality</asp:TableHeaderCell>
										        <asp:TableHeaderCell Width="20%">Action</asp:TableHeaderCell>
									        </asp:TableHeaderRow>
								        </asp:Table>
							        </div>  		
							        <div class="well well-sm bg-color-darken txt-color-white text-center" id="divCausality" runat="server">
								        <div class="row">
                                            <section class="col col-12">
										        <label class="input">
											        <input class="form-control" id="txtMedicine" type="text" runat="server" readonly="readonly" style="background-color:#92A2A8; color:white; text-align:center;">
                                                </label>
                                            </section>
								        </div>
							        </div>

						        </div>
						        <!-- end widget content -->
				
					        </div>
					        <!-- end widget div -->
				
				        </div>
				        <!-- end widget -->
				
			        </div>
			        <!-- END COL -->					

                </div>

		        <div class="row" id="divTool" runat="server" style="display:block;">
				
			        <!-- NEW WIDGET START -->
                    <article class="col-xs-12 col-sm-12 col-md-12 col-lg-8">
				
				        <!-- Widget ID (each widget will need unique ID)-->
				        <div class="jarviswidget" id="wid-id-12"  data-widget-editbutton="false" data-widget-custombutton="false" data-widget-deletebutton="false" data-widget-colorbutton="false">
					        <header>
						        <span class="widget-icon"> <i class="fa fa-table"></i> </span>
						        <h2>WHO Causality Tool</h2>
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
                                    <div class="row">

                                        <table style="width:100%;">
                                            <tbody>
                                                <tr style="border: 1px solid black;">
                                                    <th style="border: 1px solid black; background-color: #e4e1da; padding: 5px;" colspan="3">Certain</th>
                                                </tr>
                                                <tr>
                                                    <td style="padding:5px; width: 15%">1.</td>
                                                    <td style="width: 70%; padding:5px;">Event or laboratory test abnormality, with plausible time relationship to drug intake</td>
                                                    <td style="padding:5px; width: 15%">
                                                        <select id="q1">
                                                            <option value="--SELECT--">--SELECT--</option>
                                                            <option value="Yes">Yes</option>
                                                            <option value="No">No</option>
                                                        </select> 
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td style="padding:5px; width: 15%">2.</td>
                                                    <td style="width: 70%; padding:5px;">Cannot be explained by disease or other drugs</td>
                                                    <td style="padding:5px; width: 15%">
                                                        <select id="q2">
                                                            <option value="--SELECT--">--SELECT--</option>
                                                            <option value="Yes">Yes</option>
                                                            <option value="No">No</option>
                                                        </select> 
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td style="padding:5px; width: 15%">3.</td>
                                                    <td style="width: 70%; padding:5px;">Response to withdrawal plausible (pharmacologically, pathologically)</td>
                                                    <td style="padding:5px; width: 15%">
                                                        <select id="q3">
                                                            <option value="--SELECT--">--SELECT--</option>
                                                            <option value="Yes">Yes</option>
                                                            <option value="No">No</option>
                                                        </select> 
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td style="padding:5px; width: 15%">4.</td>
                                                    <td style="width: 70%; padding:5px;">Event definitive pharmacologically or phenomenologically</td>
                                                    <td style="padding:5px; width: 15%">
                                                        <select id="q4">
                                                            <option value="--SELECT--">--SELECT--</option>
                                                            <option value="Yes">Yes</option>
                                                            <option value="No">No</option>
                                                        </select> 
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td style="padding:5px; width: 15%">5.</td>
                                                    <td style="width: 70%; padding:5px;">Rechallenge satisfactory, if necessary</td>
                                                    <td style="padding:5px; width: 15%">
                                                        <select id="q5">
                                                            <option value="--SELECT--">--SELECT--</option>
                                                            <option value="Yes">Yes</option>
                                                            <option value="No">No</option>
                                                            <option value="Unnecessary">Unnecessary</option>
                                                        </select> 
                                                    </td>
                                                </tr>
                                            </tbody>
                                        </table>
                                        <div id="probabletoggle" style="padding: 0px; border: 0px solid black; display: none;">
                                            <table style="width:100%;">
                                                <tbody>
                                                    <tr style="border: 1px solid black;">
                                                        <th style="border: 1px solid black; background-color: #e4e1da; padding: 5px;" colspan="3">Probable/Likely</th>
                                                    </tr>
                                                    <tr>
                                                        <td style="padding:5px; width: 15%">6.</td>
                                                        <td style="width: 70%; padding:5px;">Event or laboratory test abnormality, with reasonable time relationship to drug intake</td>
                                                        <td style="padding:5px; width: 15%">
                                                            <select id="q6">
                                                                <option value="--SELECT--">--SELECT--</option>
                                                                <option value="Yes">Yes</option>
                                                                <option value="No">No</option>
                                                            </select> 
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td style="padding:5px; width: 15%">7.</td>
                                                        <td style="width: 70%; padding:5px;">Unlikely to be attributed to disease or other drugs </td>
                                                        <td style="padding:5px; width: 15%">
                                                            <select id="q7">
                                                                <option value="--SELECT--">--SELECT--</option>
                                                                <option value="Yes">Yes</option>
                                                                <option value="No">No</option>
                                                            </select> 
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td style="padding:5px; width: 15%">8.</td>
                                                        <td style="width: 70%; padding:5px;">Response to withdrawal clinically reasonable </td>
                                                        <td style="padding:5px; width: 15%">
                                                            <select id="q8">
                                                                <option value="--SELECT--">--SELECT--</option>
                                                                <option value="Yes">Yes</option>
                                                                <option value="No">No</option>
                                                            </select> 
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td style="padding:5px; width: 15%">9.</td>
                                                        <td style="width: 70%; padding:5px;">Rechallenge not required</td>
                                                        <td style="padding:5px; width: 15%">
                                                            <select id="q9">
                                                                <option value="--SELECT--">--SELECT--</option>
                                                                <option value="Yes">Yes</option>
                                                                <option value="No">No</option>
                                                            </select> 
                                                        </td>
                                                    </tr>
                                                </tbody>
                                            </table>
                                        </div>
                                        <div id="possibletoggle" style="padding: 0px; border: 0px solid black; display: none;">
                                            <table style="width:100%;">
                                                <tbody>
                                                    <tr style="border: 1px solid black;">
                                                        <th style="border: 1px solid black; background-color: #e4e1da; padding: 5px;" colspan="3">Possible</th>
                                                    </tr>
                                                    <tr>
                                                        <td style="padding:5px; width: 15%">10.</td>
                                                        <td style="width: 70%; padding:5px;">Event or laboratory test abnormality, with reasonable time relationship to drug intake</td>
                                                        <td style="padding:5px; width: 15%">
                                                            <select id="q10">
                                                                <option value="--SELECT--">--SELECT--</option>
                                                                <option value="Yes">Yes</option>
                                                                <option value="No">No</option>
                                                            </select> 
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td style="padding:5px; width: 15%">11.</td>
                                                        <td style="width: 70%; padding:5px;">Could also be explained by disease or other drugs</td>
                                                        <td style="padding:5px; width: 15%">
                                                            <select id="q11">
                                                                <option value="--SELECT--">--SELECT--</option>
                                                                <option value="Yes">Yes</option>
                                                                <option value="No">No</option>
                                                            </select> 
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td style="padding:5px; width: 15%">12.</td>
                                                        <td style="width: 70%; padding:5px;">Information on drug withdrawal may be lacking or unclear</td>
                                                        <td style="padding:5px; width: 15%">
                                                            <select id="q12">
                                                                <option value="--SELECT--">--SELECT--</option>
                                                                <option value="Yes">Yes</option>
                                                                <option value="No">No</option>
                                                            </select> 
                                                        </td>
                                                    </tr>
                                                </tbody>
                                            </table>
                                        </div>
                                        <div id="unlikelytoggle" style="padding: 0px; border: 0px solid black; display: none;">
                                            <table style="width:100%;">
                                                <tbody>
                                                    <tr style="border: 1px solid black;">
                                                        <th style="border: 1px solid black; background-color: #e4e1da; padding: 5px;" colspan="3">Unlikely</th>
                                                    </tr>
                                                    <tr>
                                                        <td style="padding:5px; width: 15%">13.</td>
                                                        <td style="width: 70%; padding:5px;">Event or laboratory test abnormality, with a time to drug intake that makes a relationship improbable </td>
                                                        <td style="padding:5px; width: 15%">
                                                            <select id="q13">
                                                                <option value="--SELECT--">--SELECT--</option>
                                                                <option value="Yes">Yes</option>
                                                                <option value="No">No</option>
                                                            </select> 
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td style="padding:5px; width: 15%">14.</td>
                                                        <td style="width: 70%; padding:5px;">Disease or other drugs provide plausible explanations </td>
                                                        <td style="padding:5px; width: 15%">
                                                            <select id="q14">
                                                                <option value="--SELECT--">--SELECT--</option>
                                                                <option value="Yes">Yes</option>
                                                                <option value="No">No</option>
                                                            </select> 
                                                        </td>
                                                    </tr>
                                                </tbody>
                                            </table>
                                        </div>
                                        <div id="conditionaltoggle" style="padding: 0px; border: 0px solid black; display: none;">
                                            <table style="width:100%;">
                                                <tbody>
                                                    <tr style="border: 1px solid black;">
                                                        <th style="border: 1px solid black; background-color: #e4e1da; padding: 5px;" colspan="3">Conditional/Unclassified</th>
                                                    </tr>
                                                    <tr>
                                                        <td style="padding:5px; width: 15%">15.</td>
                                                        <td style="width: 70%; padding:5px;">Event or laboratory test abnormality</td>
                                                        <td style="padding:5px; width: 15%">
                                                            <select id="q15">
                                                                <option value="--SELECT--">--SELECT--</option>
                                                                <option value="Yes">Yes</option>
                                                                <option value="No">No</option>
                                                            </select> 
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td style="padding:5px; width: 15%">16.</td>
                                                        <td style="width: 70%; padding:5px;">More data for proper assessment needed or additional data under examination</td>
                                                        <td style="padding:5px; width: 15%">
                                                            <select id="q16">
                                                                <option value="--SELECT--">--SELECT--</option>
                                                                <option value="Yes">Yes</option>
                                                                <option value="No">No</option>
                                                            </select> 
                                                        </td>
                                                    </tr>
                                                </tbody>
                                            </table>
                                        </div>
                                        <div id="unassessabletoggle" style="padding: 0px; border: 0px solid black; display: none;">
                                            <table style="width:100%;">
                                                <tbody>
                                                    <tr style="border: 1px solid black;">
                                                        <th style="border: 1px solid black; background-color: #e4e1da; padding: 5px;" colspan="3">Unassessable/Unclassified</th>
                                                    </tr>
                                                    <tr>
                                                        <td style="padding:5px; width: 15%">17.</td>
                                                        <td style="width: 70%; padding:5px;">Report suggesting an adverse reaction cannot be judged because information is insufficient or contradictory</td>
                                                        <td style="padding:5px; width: 15%">
                                                            <select id="q17">
                                                                <option value="--SELECT--">--SELECT--</option>
                                                                <option value="Yes">Yes</option>
                                                                <option value="No">No</option>
                                                            </select>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td style="padding:5px; width: 15%">18.</td>
                                                        <td style="width: 70%; padding:5px;">Data cannot be supplemented or verified</td>
                                                        <td style="padding:5px; width: 15%">
                                                            <select id="q18">
                                                                <option value="--SELECT--">--SELECT--</option>
                                                                <option value="Yes">Yes</option>
                                                                <option value="No">No</option>
                                                            </select>
                                                        </td>
                                                    </tr>
                                                </tbody>
                                            </table>
                                        </div>
                                        <table style="width:100%;">
                                            <tbody>
                                                <tr>
                                                    <td> </td>
                                                    <td style="width: 70%; padding:5px;"> <b> Causality </b> </td>
                                                    <td style="width: 250px; white-space: nowrap; background-color: lightgray; padding:5px;"> <span id="causalityA" style="align-items:center; width=150px;" runat="server"></span> <input type="hidden" id="hidCausality" name="hidCausality" runat="server" /><input type="hidden" id="hidMedication" name="hidMedication" runat="server" /></td>
                                                </tr>
                                            </tbody>
                                        </table>

                                    </div>
                                    <br />
                                    <div class="smart-form">
							            <footer >
								            <span id="spnButtons" runat="server">

								            </span>
							            </footer>
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
			        <!-- WIDGET END -->
				
		        </div>

		        <div class="row">

                    <article class="col-xs-12 col-sm-12 col-md-12 col-lg-8">
                        <div class="smart-form">
						    <footer >
							    <span id="spnReturn" runat="server">

							    </span>
						    </footer>
                        </div>  
                    </article>

		        </div>

        </div>

	</section>
	<!-- end widget grid -->

</asp:Content>

<asp:Content runat="server" ID="Scripts" ContentPlaceHolderID="scriptsPlaceholder">

    <script>

        $('#form1').submit(function (e) {
            if ($("#hidCausality").val() == "")
            {
                $.SmartMessageBox({
                    title: "Assessment",
                    content: "Please ensure all questions are completed correctly",
                    buttons: '[OK]'
                }, function (ButtonPressed) {
                    if (ButtonPressed === "OK") {
                        return false;
                    }
                });
                e.preventDefault();
            }
            else {
                return true;
            }
            return true;
        });

    </script>

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

        $('#q1').add('#q2').add('#q3').add('#q4').add('#q5').change(function (e) {
            handleProbable();
        });
        $('#q6').add('#q7').add('#q8').add('#q9').change(function (e) {
            handlePossible();
        });
        $('#q10').add('#q11').add('#q12').change(function (e) {
            handleUnlikely();
        });
        $('#q13').add('#q14').change(function (e) {
            handleConditional();
        });
        $('#q15').add('#q16').change(function (e) {
            handleUnassessable();
        });
        $('#q17').add('#q18').change(function (e) {
            handleFinal();
        });

        var handleProbable = function () {
            $("#probabletoggle").hide();
            $("#possibletoggle").hide();
            $("#unlikelytoggle").hide();
            $("#conditionaltoggle").hide();
            $("#unassessabletoggle").hide();

            if ($("#q1").val() == '--SELECT--' || $("#q2").val() == '--SELECT--' || $("#q3").val() == '--SELECT--' || $("#q4").val() == '--SELECT--' || $("#q5").val() == '--SELECT--') {
                $("#causalityA").html("");
                $("#hidCausality").val("");
                return;
            }
            if ($("#q1").val() == 'Yes' && $("#q2").val() == 'Yes' && $("#q3").val() == 'Yes' && $("#q4").val() == 'Yes' && $("#q5").val() == 'Yes') {
                $("#causalityA").html("Certain");
                $("#hidCausality").val("Certain");
                return;
            }
            $("#probabletoggle").show();
            $("#causalityA").html("");
            $("#hidCausality").val("");
        };

        var handlePossible = function () {
            $("#possibletoggle").hide();
            $("#unlikelytoggle").hide();
            $("#conditionaltoggle").hide();
            $("#unassessabletoggle").hide();

            if ($("#q6").val() == '--SELECT--' || $("#q7").val() == '--SELECT--' || $("#q8").val() == '--SELECT--' || $("#q9").val() == '--SELECT--') {
                $("#causalityA").html("");
                $("#hidCausality").val("");
                return;
            }
            if ($("#q6").val() == 'Yes' && $("#q7").val() == 'Yes' && $("#q8").val() == 'Yes' && $("#q9").val() == 'Yes') {
                $("#causalityA").html("Probable");
                $("#hidCausality").val("Probable");
                return;
            }
            $("#possibletoggle").show();
            $("#causalityA").html("");
            $("#hidCausality").val("");
        };

        var handleUnlikely = function () {
            $("#unlikelytoggle").hide();
            $("#conditionaltoggle").hide();
            $("#unassessabletoggle").hide();

            if ($("#q10").val() == '--SELECT--' || $("#q11").val() == '--SELECT--' || $("#q12").val() == '--SELECT--') {
                $("#causalityA").html("");
                $("#hidCausality").val("");
                return;
            }
            if ($("#q10").val() == 'Yes' && $("#q11").val() == 'Yes' && $("#q12").val() == 'Yes') {
                $("#causalityA").html("Possible");
                $("#hidCausality").val("Possible");
                return;
            }
            $("#unlikelytoggle").show();
            $("#causalityA").html("");
            $("#hidCausality").val("");
        };

        var handleConditional = function () {
            $("#conditionaltoggle").hide();
            $("#unassessabletoggle").hide();

            if ($("#q13").val() == '--SELECT--' || $("#q14").val() == '--SELECT--') {
                $("#causalityA").html("");
                $("#hidCausality").val("");
                return;
            }
            if ($("#q13").val() == 'Yes' && $("#q14").val() == 'Yes') {
                $("#causalityA").html("Unlikely");
                $("#hidCausality").val("Unlikely");
                return;
            }
            $("#conditionaltoggle").show();
            $("#causalityA").html("");
            $("#hidCausality").val("");
        };

        var handleUnassessable = function () {
            $("#unassessabletoggle").hide();

            if ($("#q15").val() == '--SELECT--' || $("#q16").val() == '--SELECT--') {
                $("#causalityA").html("");
                $("#hidCausality").val("");
                return;
            }
            if ($("#q15").val() == 'Yes' && $("#q16").val() == 'Yes') {
                $("#causalityA").html("Conditional");
                $("#hidCausality").val("Conditional");
                return;
            }
            $("#unassessabletoggle").show();
            $("#causalityA").html("");
            $("#hidCausality").val("");
        };

        var handleFinal = function () {
            if ($("#q17").val() == '--SELECT--' || $("#q18").val() == '--SELECT--') {
                $("#causalityA").html("");
                $("#hidCausality").val("");
                return;
            }
            if ($("#q17").val() == 'Yes' && $("#q18").val() == 'Yes') {
                $("#causalityA").html("Unassessable");
                $("#hidCausality").val("Unassessable");
                return;
            }
            if ($("#q17").val() == "No" || $("#q18").val() == "No") {
                $.SmartMessageBox({
                    title: "Assessment",
                    content: "The answer to questions 17 and 18 cannot be 'no'. Please review all of the questions to which you answered 'no' in the list above to determine which one of them should be 'yes'",
                    buttons: '[OK]'
                }, function (ButtonPressed) {
                    if (ButtonPressed === "OK") {
                        return;
                    }
                });
            }

            $("#causalityA").html("");
            $("#hidCausality").val("");
        };

	</script>

</asp:Content>



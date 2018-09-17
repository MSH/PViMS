<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CausalityNaranjo.aspx.cs" MasterPageFile="/Main.Master" Inherits="PVIMS.Web.CausalityNaranjo" Title="Causality Naranjo" %>

<asp:Content runat="server" ID="Body" ContentPlaceHolderID="BodyContentPlaceHolder" ClientIDMode="Static">
    <asp:HiddenField runat="server" ID="hfPosition" Value="" />

	<!-- widget grid -->
	<section id="widget-grid" class="">

		<div class="row">

            <article class="col-sm-12 col-md-12 col-lg-7">

                <div class="well well-sm" style="height:auto; padding-top:10px; padding-left:10px; padding-right:10px; padding-bottom:0px;">
                    <div class="row">
                        <div class="smart-form">
                            <section class="col col-4">
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
                            <section class="col col-4">
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
                            <section class="col col-4">
                                <span class="label" style="padding:5px; background-color:lightgray;"> <b> Central Level MedDRA Term </b> </span>
                            </section>
                            <section class="col col-6">
                                <label runat="server" id="lblSelection" class="input" style="padding:5px;">
                                </label>
                            </section>
                        </div>
                    </div>
				</div>

			</article>

		</div>

        <div>
            
                <div class="alert alert-danger alert-block" runat="server" id="divSetTerm" name="divSetTerm" visible="false">
                    <strong>Note!</strong> Please ensure terminology is set...
                </div>

		        <div class="row" id="divDesc" runat="server" style="display:none;">

                    <div class="col-sm-12 col-md-12 col-lg-7">

                        <div class="well well-sm" style="height:auto; padding-top:10px; padding-left:10px; padding-right:10px; padding-bottom:0px;">
                            <div class="row">
                                <div class="smart-form">
                                    <section class="col col-2">
                                        <span class="label" style="padding:5px; background-color:lightgray;"> <b> Definite </b> </span>
                                    </section>
                                    <section class="col col-4">
                                        <label runat="server" id="lblUniqueID" class="input">
                                            Score is greater than or equal to 9
                                        </label>
                                    </section>
                                    <section class="col col-2">
                                        <span class="label" style="padding:5px; background-color:lightgray;"> <b> Probable </b> </span>
                                    </section>
                                    <section class="col col-4">
                                        <label runat="server" id="Label1" class="input">
                                            Score is between 5 and 8
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
                                            Score is between 1 and 4
                                        </label>
                                    </section>
                                    <section class="col col-2">
                                        <span class="label" style="padding:5px; background-color:lightgray;"> <b> Doubtful </b> </span>
                                    </section>
                                    <section class="col col-4">
                                        <label runat="server" id="Label3" class="input">
                                            Score is less than 1
                                        </label>
                                    </section>
                                </div>
                            </div>
				        </div>

			        </div>

		        </div>

		        <div class="row" id="divMedications" runat="server" style="display:block;">

			        <div class="col-sm-12 col-md-12 col-lg-7">
				
				        <!-- Widget ID (each widget will need unique ID)-->
				        <div class="jarviswidget" id="wid_id_21"  data-widget-editbutton="false" data-widget-custombutton="false" data-widget-deletebutton="false" data-widget-colorbutton="false">
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
											        <input class="form-control" id="txtMedicine" type="text" runat="server" readonly="readonly" style="background-color:#92A2A8; color:white;; text-align:center;">
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

                </div>

		        <div class="row"  id="divTool" runat="server" style="display:block;">
				
			        <!-- NEW WIDGET START -->
			        <div class="col-xs-12 col-sm-12 col-md-12 col-lg-7">
				
				        <!-- Widget ID (each widget will need unique ID)-->
				        <div class="jarviswidget" id="wid-id-22"  data-widget-editbutton="false" data-widget-custombutton="false" data-widget-deletebutton="false" data-widget-colorbutton="false">
					        <header>
						        <span class="widget-icon"> <i class="fa fa-table"></i> </span>
						        <h2>Naranjo Causality Tool</h2>
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
                                                <tr>
                                                    <td style="padding:5px; width: 13%">1.</td>
                                                    <td style="width: 70%; padding:5px;">Are there previous conclusive reports on this reaction?</td>
                                                    <td style="padding:5px; width: 13%">
                                                        <select id="q1">
                                                            <option value="--SELECT--">--SELECT--</option>
                                                            <option value="Yes">Yes</option>
                                                            <option value="No">No</option>
                                                            <option value="Do not know">Do not know</option>
                                                        </select> 
                                                    </td>
                                                    <td style="background-color: lightgray; padding:5px; width: 4%;">&nbsp;<span id="q1A">0</span>&nbsp;</td>
                                                </tr>
                                                <tr>
                                                    <td style="padding:5px; width: 13%">2.</td>
                                                    <td style="width: 70%; padding:5px;">Did the adverse event appear after the suspected drug was administered?</td>
                                                    <td style="padding:5px; width: 13%">
                                                        <select id="q2">
                                                            <option value="--SELECT--">--SELECT--</option>
                                                            <option value="Yes">Yes</option>
                                                            <option value="No">No</option>
                                                            <option value="Do not know">Do not know</option>
                                                        </select> 
                                                    </td>
                                                    <td style="background-color: lightgray; padding:5px; width: 4%;">&nbsp;<span id="q2A">0</span>&nbsp;</td>
                                                </tr>
                                                <tr>
                                                    <td style="padding:5px; width: 13%">3.</td>
                                                    <td style="width: 70%; padding:5px;">Did the adverse reaction improve when the drug was discontinued or a specific antagonist was administered?</td>
                                                    <td style="padding:5px; width: 13%">
                                                        <select id="q3">
                                                            <option value="--SELECT--">--SELECT--</option>
                                                            <option value="Yes">Yes</option>
                                                            <option value="No">No</option>
                                                            <option value="Do not know">Do not know</option>
                                                        </select> 
                                                    </td>
                                                    <td style="background-color: lightgray; padding:5px; width: 4%;">&nbsp;<span id="q3A">0</span>&nbsp;</td>
                                                </tr>
                                                <tr>
                                                    <td style="padding:5px; width: 13%">4.</td>
                                                    <td style="width: 70%; padding:5px;">Did the adverse reaction reappear when the drug was readministered?</td>
                                                    <td style="padding:5px; width: 13%">
                                                        <select id="q4">
                                                            <option value="--SELECT--">--SELECT--</option>
                                                            <option value="Yes">Yes</option>
                                                            <option value="No">No</option>
                                                            <option value="Do not know">Do not know</option>
                                                        </select> 
                                                    </td>
                                                    <td style="background-color: lightgray; padding:5px; width: 4%;">&nbsp;<span id="q4A">0</span>&nbsp;</td>
                                                </tr>
                                                <tr>
                                                    <td style="padding:5px; width: 13%">5.</td>
                                                    <td style="width: 70%; padding:5px;">Are there alternative causes (other than the drug) that could on their own have caused the reaction?</td>
                                                    <td style="padding:5px; width: 13%">
                                                        <select id="q5">
                                                            <option value="--SELECT--">--SELECT--</option>
                                                            <option value="Yes">Yes</option>
                                                            <option value="No">No</option>
                                                            <option value="Do not know">Do not know</option>
                                                        </select> 
                                                    </td>
                                                    <td style="background-color: lightgray; padding:5px; width: 4%;">&nbsp;<span id="q5A">0</span>&nbsp;</td>
                                                </tr>
                                                <tr>
                                                    <td style="padding:5px; width: 13%">6.</td>
                                                    <td style="width: 70%; padding:5px;">Did the reaction reappear when a placebo was given? </td>
                                                    <td style="padding:5px; width: 13%">
                                                        <select id="q6">
                                                            <option value="--SELECT--">--SELECT--</option>
                                                            <option value="Yes">Yes</option>
                                                            <option value="No">No</option>
                                                            <option value="Do not know">Do not know</option>
                                                        </select> 
                                                    </td>
                                                    <td style="background-color: lightgray; padding:5px; width: 4%;">&nbsp;<span id="q6A">0</span>&nbsp;</td>
                                                </tr>
                                                <tr>
                                                    <td style="padding:5px; width: 13%">7.</td>
                                                    <td style="width: 70%; padding:5px;">Was the drug detected in the blood (or other fluids) in concentrations known to be toxic? </td>
                                                    <td style="padding:5px; width: 13%">
                                                        <select id="q7">
                                                            <option value="--SELECT--">--SELECT--</option>
                                                            <option value="Yes">Yes</option>
                                                            <option value="No">No</option>
                                                            <option value="Do not know">Do not know</option>
                                                        </select> 
                                                    </td>
                                                    <td style="background-color: lightgray; padding:5px; width: 4%;">&nbsp;<span id="q7A">0</span>&nbsp;</td>
                                                </tr>
                                                <tr>
                                                    <td style="padding:5px; width: 13%">8.</td>
                                                    <td style="width: 70%; padding:5px;">Was the reaction more severe when the dose was increased, or less severe when the dose was decreased? </td>
                                                    <td style="padding:5px; width: 13%">
                                                        <select id="q8">
                                                            <option value="--SELECT--">--SELECT--</option>
                                                            <option value="Yes">Yes</option>
                                                            <option value="No">No</option>
                                                            <option value="Do not know">Do not know</option>
                                                        </select> 
                                                    </td>
                                                    <td style="background-color: lightgray; padding:5px; width: 4%;">&nbsp;<span id="q8A">0</span>&nbsp;</td>
                                                </tr>
                                                <tr>
                                                    <td style="padding:5px; width: 13%">9.</td>
                                                    <td style="width: 70%; padding:5px;">Did the patient have a similar reaction to the same or similar drugs in any previous exposure?</td>
                                                    <td style="padding:5px; width: 13%">
                                                        <select id="q9">
                                                            <option value="--SELECT--">--SELECT--</option>
                                                            <option value="Yes">Yes</option>
                                                            <option value="No">No</option>
                                                            <option value="Do not know">Do not know</option>
                                                        </select> 
                                                    </td>
                                                    <td style="background-color: lightgray; padding:5px; width: 4%;">&nbsp;<span id="q9A">0</span>&nbsp;</td>
                                                </tr>
                                                <tr>
                                                    <td style="padding:5px; width: 13%">10.</td>
                                                    <td style="width: 70%; padding:5px;">Was the adverse event confirmed by any objective evidence? </td>
                                                    <td style="padding:5px; width: 13%">
                                                        <select id="q10">
                                                            <option value="--SELECT--">--SELECT--</option>
                                                            <option value="Yes">Yes</option>
                                                            <option value="No">No</option>
                                                            <option value="Do not know">Do not know</option>
                                                        </select> 
                                                    </td>
                                                    <td style="background-color: lightgray; padding:5px; width: 4%;">&nbsp;<span id="q10A">0</span>&nbsp;</td>
                                                </tr>
                                                <tr>
                                                    <td align="right" colspan="2"> <button onclick="return Calculate();" type="submit">Calculate</button>  </td>
                                                    <td style="padding:5px; width: 10%;"> <span id="causalityA" runat="server"></span> <input type="hidden" id="hidCausality" name="hidCausality" runat="server" /> <input type="hidden" id="hidMedication" name="hidMedication" runat="server" /></td>
                                                    <td style="background-color: lightgray; padding:5px; width: 4%;">&nbsp;<span id="totalScore">0</span>&nbsp;</td>
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
				
			        </div>
			        <!-- WIDGET END -->
				
		        </div>

		        <div class="row">

                    <article class="col-xs-12 col-sm-12 col-md-12 col-lg-7">
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

        /*
         * SmartAlerts
         */
        // With Callback
        $('#form1').submit(function (e) {
            if ($("#hidCausality").val() == "" || $("#BodyContentPlaceHolder_hidCausality").val() == "Incomplete")
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

        function Calculate() {
            try {
                var ele = document.getElementById('q1');
                var eleA = document.getElementById('q1A');
                var q1;
                var complete = true;
                switch (ele.options[ele.selectedIndex].value) {
                    case 'Yes':
                        q1 = '1';
                        break;
                    case 'No':
                        q1 = '0';
                        break;
                    case 'Do not know':
                        q1 = '0';
                        break;
                    default:
                        q1 = '0';
                        complete = false;
                        break;
                }
                eleA.innerHTML = q1;

                eleA = document.getElementById('q2A');
                ele = document.getElementById('q2');
                var q2;
                switch (ele.options[ele.selectedIndex].value) {
                    case 'Yes':
                        q2 = '2';
                        break;
                    case 'No':
                        q2 = '-1';
                        break;
                    case 'Do not know':
                        q2 = '0';
                        break;
                    default:
                        q2 = '0';
                        complete = false;
                        break;
                }
                eleA.innerHTML = q2;

                eleA = document.getElementById('q3A');
                ele = document.getElementById('q3');
                var q3;
                switch (ele.options[ele.selectedIndex].value) {
                    case 'Yes':
                        q3 = '1';
                        break;
                    case 'No':
                        q3 = '0';
                        break;
                    case 'Do not know':
                        q3 = '0';
                        break;
                    default:
                        q3 = '0';
                        complete = false;
                        break;
                }
                eleA.innerHTML = q3;

                eleA = document.getElementById('q4A');
                ele = document.getElementById('q4');
                var q4;
                switch (ele.options[ele.selectedIndex].value) {
                    case 'Yes':
                        q4 = '2';
                        break;
                    case 'No':
                        q4 = '-1';
                        break;
                    case 'Do not know':
                        q4 = '0';
                        break;
                    default:
                        q4 = '0';
                        complete = false;
                        break;
                }
                eleA.innerHTML = q4;

                eleA = document.getElementById('q5A');
                ele = document.getElementById('q5');
                var q5;
                switch (ele.options[ele.selectedIndex].value) {
                    case 'Yes':
                        q5 = '-1';
                        break;
                    case 'No':
                        q5 = '2';
                        break;
                    case 'Do not know':
                        q5 = '0';
                        break;
                    default:
                        q5 = '0';
                        complete = false;
                        break;
                }
                eleA.innerHTML = q5;

                eleA = document.getElementById('q6A');
                ele = document.getElementById('q6');
                var q6;
                switch (ele.options[ele.selectedIndex].value) {
                    case 'Yes':
                        q6 = '-1';
                        break;
                    case 'No':
                        q6 = '1';
                        break;
                    case 'Do not know':
                        q6 = '0';
                        break;
                    default:
                        q6 = '0';
                        complete = false;
                        break;
                }
                eleA.innerHTML = q6;

                eleA = document.getElementById('q7A');
                ele = document.getElementById('q7');
                var q7;
                switch (ele.options[ele.selectedIndex].value) {
                    case 'Yes':
                        q7 = '1';
                        break;
                    case 'No':
                        q7 = '0';
                        break;
                    case 'Do not know':
                        q7 = '0';
                        break;
                    default:
                        q7 = '0';
                        complete = false;
                        break;
                }
                eleA.innerHTML = q7;

                eleA = document.getElementById('q8A');
                ele = document.getElementById('q8');
                var q8;
                switch (ele.options[ele.selectedIndex].value) {
                    case 'Yes':
                        q8 = '1';
                        break;
                    case 'No':
                        q8 = '0';
                        break;
                    case 'Do not know':
                        q8 = '0';
                        break;
                    default:
                        q8 = '0';
                        complete = false;
                        break;
                }
                eleA.innerHTML = q8;

                eleA = document.getElementById('q9A');
                ele = document.getElementById('q9');
                var q9;
                switch (ele.options[ele.selectedIndex].value) {
                    case 'Yes':
                        q9 = '1';
                        break;
                    case 'No':
                        q9 = '0';
                        break;
                    case 'Do not know':
                        q9 = '0';
                        break;
                    default:
                        q9 = '0';
                        complete = false;
                        break;
                }
                eleA.innerHTML = q9;

                eleA = document.getElementById('q10A');
                ele = document.getElementById('q10');
                var q10;
                switch (ele.options[ele.selectedIndex].value) {
                    case 'Yes':
                        q10 = '1';
                        break;
                    case 'No':
                        q10 = '0';
                        break;
                    case 'Do not know':
                        q10 = '0';
                        break;
                    default:
                        q10 = '0';
                        complete = false;
                        break;
                }
                eleA.innerHTML = q10;

                // Calculate causality
                var score = parseInt(q1) + parseInt(q2) + parseInt(q3) + parseInt(q4) + parseInt(q5) + parseInt(q6) + parseInt(q7) + parseInt(q8) + parseInt(q9) + parseInt(q10);
                var causality = '';

                eleCausality = document.getElementById('causalityA');
                eleTotalScore = document.getElementById('totalScore');
                var hidCausality = document.getElementById('hidCausality');

                if (complete)
                {
                    switch (true) {
                        case (score >= 9):
                            causality = 'Definite';
                            break;
                        case (score > 4 && score < 9):
                            causality = 'Probable';
                            break;
                        case (score > 0 && score < 5):
                            causality = 'Possible';
                            break;
                        case (score <= 0):
                            causality = 'Doubtful';
                            break;
                    }
                }
                else
                {
                    causality = 'Incomplete';
                }
                eleCausality.innerHTML = causality;
                eleTotalScore.innerHTML = score;
                hidCausality.value = causality;
            }
            catch (err) {
                txt = "There was an error on this page.\n\n";
                txt += "Error description: " + err.message + "\n\n";
                txt += "Click OK to continue.\n\n";
                alert(txt);
            }

            return false;
        }
    </script>

</asp:Content>



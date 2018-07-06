<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="DisplayError.aspx.cs" Inherits="PVIMS.Web.DisplayError" Title="Display Error"  MasterPageFile="/Main.Master" %>

<asp:Content runat="server" ID="Body" ContentPlaceHolderID="BodyContentPlaceHolder">

			<div class="row">
			    <div class="col-xs-12">
			    <% if (!String.IsNullOrWhiteSpace(Request["backUrl"]))
				    {
				    %>
				    <a href="<%= Request["backUrl"] %>">Back<%= String.IsNullOrWhiteSpace(Request["backtext"]) ? "": " to " + Request["Backtext"]%></a>
					    <%
			       } %>
			    </div>
			</div>

	        <div class="row">
		        <div class="col-xs-12 col-sm-7 col-md-7 col-lg-4">
			        <h1 class="page-title txt-color-blueDark">
				        <i class="fa fa-home fa-fw "></i> 
				        Errors
			        </h1>
		        </div>
	        </div>

	        <!-- widget grid -->
	        <section id="widget-grid" class="">

				<div class="row">
					<article class="col-sm-12 col-md-12 col-lg-6">

                       <div class="well well-sm" style="height:350px; padding-top:10px; padding-left:10px; padding-right:10px; padding-bottom:0px;">

						    <h2 class="font-xl  text-center"><strong>Oooops, Something went wrong!</strong></h2>
                            <p class="lead semi-bold">
							    <small>
								    We are sorry, you have experienced a technical error. Please may you assist with the following: -
							    </small>
                            </p>
						    <ul class="text-left font-md">
							    <li><small>Take a screenshot of the error message highlighted below </small></li>
							    <li><small>Send the screenshot through to your PViMS administrator</small></li>
							    <li><small>Include a description of what you were doing on the system at the time of the error</small></li>
						    </ul>
                            <p class="lead semi-bold">
							    <small>
								    Please use one of the links below to return to PViMS: -
							    </small>
                            </p>
						    <ul class="text-left font-md">
							    <li><a href="/Patient/PatientSearch.aspx"><small>Go to Patient Search <i class="fa fa-arrow-right"></i></small></a></li>
							    <li><a href="/Encounter/EncounterSearch.aspx"><small>Go to Encounter Search <i class="fa fa-arrow-right"></i></small></a></li>
						    </ul>

                        </div>

				    </article>
                </div>
                <div class="row">
					<article class="col-sm-12 col-md-12 col-lg-6">

                        <div class="well well-sm text-center" style="height:200px; padding-top:10px; padding-left:10px; padding-right:10px; padding-bottom:0px;">

                            <h2><i class="fa fa-fw fa-warning fa-lg text-warning"></i>Error Message </h2>
                            <p class="lead">
                                <asp:Label ID="lblMessage" runat="server" /><br />
                            </p>
                            <p class="lead semi-bold">
                                <small>
                                    <asp:Label ID="lblInnerMessage" runat="server" /><br />
                                </small>
                            </p>
                            <p class="lead semi-bold" id="Local" runat="server" visible="false">
                                <strong>Trace</strong><br>
                                <small>
                                    <asp:Label ID="lblInnerTrace" runat="server" /><br />
                                </small>
                            </p>

                        </div>

				    </article>
				
				</div>

	        </section>
	        <!-- end widget grid -->

</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="scriptsPlaceholder">
	<script>


	</script>	
</asp:Content>

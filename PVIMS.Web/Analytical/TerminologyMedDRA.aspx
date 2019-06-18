<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="TerminologyMedDRA.aspx.cs" MasterPageFile="/Main.Master" Inherits="PVIMS.Web.TerminologyMedDRA" Title="Assign MedDRA Terminology" %>

<asp:Content runat="server" ID="Body" ContentPlaceHolderID="BodyContentPlaceHolder" ClientIDMode="Static">

	<!-- widget grid -->
	<section id="widget-grid" class="">

		<div class="well row">

            <article class="col-sm-12 col-md-12 col-lg-9">

                <div class="well well-sm" style="height:auto; padding-top:10px; padding-left:10px; padding-right:10px; padding-bottom:0px;">
                    <div class="row">
                        <div class="smart-form">
                            <section class="col col-4">
                                <span class="label" style="padding:5px; background-color:#F1F1F1;" id="lblVerbatimLabel" runat="server"> </span>
                            </section>
                            <section class="col col-8">
                                <label runat="server" id="lblVerbatim" class="input" style="padding:5px;">
                                </label>
                            </section>
                        </div>
                    </div>
                    <div class="row" id="divSource" runat="server" style="display:block;">
                        <div class="smart-form">
                            <section class="col col-4">
                                <span class="label" style="padding:5px; background-color:#F1F1F1;"> <b> Facility Level MedDRA Term </b> </span>
                            </section>
                            <section class="col col-8">
                                <label runat="server" id="lblSource" class="input" style="padding:5px;">
                                </label>
                            </section>
                        </div>
                    </div>
				</div>

			</article>

		</div>

		<div class="well row">
				
			<!-- NEW WIDGET START -->
			<article class="col-xs-12 col-sm-12 col-md-12 col-lg-9">
				
				<!-- Widget ID (each widget will need unique ID)-->
				<div class="jarviswidget" id="wid-id-5"  data-widget-editbutton="false" data-widget-custombutton="false" data-widget-deletebutton="false" data-widget-colorbutton="false">
					<header>
						<span class="widget-icon"> <i class="fa fa-table"></i> </span>
						<h2>Search MedDRA</h2>
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
								<li id="liCommon" class="active" runat="server">
									<a href="#tab1" data-toggle="tab" aria-expanded="true">Common</a>
								</li>
								<li id="liList" runat="server">
									<a href="#tab2" data-toggle="tab" aria-expanded="true">List</a>
								</li>
								<li id="liMTerm" runat="server">
									<a href="#tab3" data-toggle="tab" aria-expanded="true">MedDRA Term</a>
								</li>
								<li id="liMCode" runat="server">
									<a href="#tab4" data-toggle="tab" aria-expanded="true">MedDRA Code</a>
								</li>
							</ul>

                            <div class="tab-content">

                                <div id="tab1" class="tab-pane active smart-form" runat="server">
                                    <div>
								        <fieldset>
									        <div class="row">
                                                <section class="col col-3">
										            <label class="input">
											            <asp:ListBox ID="lstCommon" runat="server" Width="350" Height="250">
                                                        </asp:ListBox>
                                                    </label>
                                                </section>
									        </div>
								        </fieldset>
                                    </div>
                                </div>

                                <div id="tab2" class="tab-pane smart-form" runat="server">
                                    <div>
								        <fieldset>
									        <div class="row">
                                                <section class="col col-6">
										            <label class="select">Find by System Organ Class (SOC): 
                                                        <asp:DropDownList ID="ddlSOC" name="ddlSOC" runat="server" style="color:black" class="form-control" AutoPostBack="true" OnSelectedIndexChanged="ddlSOC_SelectedIndexChanged">
                                                        </asp:DropDownList>
                                                    </label>
                                                </section>
									        </div>
									        <div class="row" id="divHLGT" runat="server" style="display:none;">
                                                <section class="col col-6">
										            <label class="select">Find by High Level Group Term (HLGT): 
                                                        <asp:DropDownList ID="ddlHLGT" name="ddlHLGT" runat="server" style="color:black" class="form-control" AutoPostBack="true" OnSelectedIndexChanged="ddlHLGT_SelectedIndexChanged">
                                                        </asp:DropDownList>
                                                    </label>
                                                </section>
									        </div>
									        <div class="row" id="divHLT" runat="server" style="display:none;">
                                                <section class="col col-6">
										            <label class="select">Find by High Level Term (HLT): 
                                                        <asp:DropDownList ID="ddlHLT" name="ddlHLT" runat="server" style="color:black" class="form-control" AutoPostBack="true" OnSelectedIndexChanged="ddlHLT_SelectedIndexChanged">
                                                        </asp:DropDownList>
                                                    </label>
                                                </section>
									        </div>
									        <div class="row" id="divPT" runat="server" style="display:none;">
                                                <section class="col col-6">
										            <label class="select">Find by Preferred Term (PT): 
                                                        <asp:DropDownList ID="ddlPT" name="ddlPT" runat="server" style="color:black" class="form-control"  AutoPostBack="true" OnSelectedIndexChanged="ddlPT_SelectedIndexChanged">
                                                        </asp:DropDownList>
                                                    </label>
                                                </section>
									        </div>
									        <div class="row" id="divLLT" runat="server" style="display:none;">
                                                <section class="col col-6">
										            <label class="input">Find by Lowest Level Term (LLT):<br />
											            <asp:ListBox ID="lstLLT" runat="server" Width="450" Height="250">
                                                        </asp:ListBox>
                                                    </label>
                                                </section>
									        </div>
								        </fieldset>
                                    </div>
                                </div>

                                <div id="tab3" class="tab-pane smart-form" runat="server">
                                    <div>
								        <fieldset>
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
                                                    <label runat="server" id="lblTerm" class="input">
                                                        Find by Term:
                                                        <input class="form-control" id="txtTerm" type="text" maxlength="100" runat="server">
                                                    </label>
                                                </section>
									        </div>
                                            <div class="row">
                                                <section class="col col-6">
                                                    <asp:Button ID="btnSearch" runat="server" Text="Search" OnClick="btnSearch_Click" class="btn btn-default" Width="100" Height="30" />
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
                                    </div>
                                </div>

                                <div id="tab4" class="tab-pane smart-form" runat="server">
                                    <div>
								        <fieldset>
									        <div class="row">
                                                <section class="col col-6">
                                                    <label runat="server" id="lblCode" class="input">
                                                        Find by Code:
                                                        <input class="form-control" id="txtCode" type="text" maxlength="20" runat="server">
                                                    </label>
                                                </section>
									        </div>
                                            <div class="row">
                                                <section class="col col-6">
                                                    <asp:Button ID="btnSearchCode" runat="server" Text="Search" OnClick="btnSearchCode_Click" class="btn btn-default" Width="100" Height="30" />
                                                </section>
                                            </div>
									        <div class="row">
                                                <section class="col col-6">
										            <label class="input">Results:<br />
											            <asp:ListBox ID="lstCodeResult" runat="server" Width="450" Height="250">
                                                        </asp:ListBox>
                                                    </label>
                                                </section>
									        </div>
								        </fieldset>
                                    </div>
                                </div>
                            </div>

                            <br />
                            <div class="smart-form">
							    <footer >
								    <span id="spnButtons" runat="server">

								    </span>
							    </footer>
                                <div class="alert alert-danger alert-block" runat="server" id="divError" name="diverror" visible="false">
                                    <label class="input" id="lblError" runat="server"></label>
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
				
		<!-- end row -->
			
	</section>
	<!-- end widget grid -->

</asp:Content>

<asp:Content runat="server" ID="Scripts" ContentPlaceHolderID="scriptsPlaceholder">


</asp:Content>



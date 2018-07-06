<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="/Main.Master" CodeBehind="PatientSearch.aspx.cs" Inherits="PVIMS.Web.PatientSearch" Title="Patient Search" %>

<asp:Content runat="server" ID="Body" ContentPlaceHolderID="BodyContentPlaceHolder">

    <!-- widget grid -->
    <section id="widget-grid" class="">

        <div class="row">

            <!-- NEW WIDGET START -->
            <article class="col-xs-12 col-sm-12 col-md-12 col-lg-12">

                <!-- Widget ID (each widget will need unique ID)-->
                <div class="jarviswidget" id="wid-id-5"  data-widget-editbutton="false" data-widget-custombutton="false" data-widget-deletebutton="false" data-widget-colorbutton="false">
                    <header>
                        <span class="widget-icon"><i class="fa fa-table"></i></span>
                        <h2>Patients</h2>
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
                            <div class="alert alert-success fade in" runat="server" id="divDelete" name="divDelete" visible="false">
                                <strong>Success!</strong> Patient deleted successfully!
                            </div>
                            <div class="well well-sm" style="height:auto; padding-top:10px; padding-left:10px; padding-right:10px; padding-bottom:0px;">
                                <div class="row">
                                    <div class="smart-form">
                                        <section class="col col-3">
                                            <label runat="server" id="lblFacility" class="input">Facility</label>
                                            <asp:DropDownList ID="ddlFacility" name="ddlFacility" runat="server" Style="color: black" CssClass="form-control">
                                                <asp:ListItem Value="" Selected="True">All Facilities</asp:ListItem>
                                            </asp:DropDownList>
                                        </section>
                                        <section class="col col-2">
                                            <label runat="server" id="lblUniqueID" class="input">
                                                Unique ID
        							            <input type="text" id="txtUniqueID" name="txtUniqueID" style="color: black" placeholder="Unique ID" runat="server" class="form-control" />
                                            </label>
                                        </section>
                                        <section class="col col-2">
                                            <label runat="server" id="lblFirstName" class="input">
                                                First Name
        							            <input type="text" id="txtFirstName" name="txtFirstName" style="color: black" placeholder="First Name" runat="server" class="form-control" />
                                            </label>
                                        </section>
                                        <section class="col col-2">
                                            <label runat="server" id="lblSurname" class="input">
                                                Last Name
        							            <input type="text" id="txtSurname" name="txtSurname" style="color: black" placeholder="Last Name" runat="server" class="form-control" />
                                            </label>
                                        </section>
                                    </div>
                                </div>
                                <div class="row" id="divCustomSearch" runat="server" visible="false">
                                    <div class="smart-form">
                                        <section class="col col-2">
                                            <label runat="server" id="lblDOB" class="input">
                                                Date of Birth
        							            <input type="text" id="txtDateBirth" name="txtDateBirth" style="color: black" placeholder="yyyy-mm-dd" runat="server" class="form-control datepicker" width="250px" />
                                            </label>
                                        </section>
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
                                        <span id="spnButtons" runat="server"></span>
								    </footer>
                                </div>
                            </div>
                            <div class="alert alert-block" style="text-align: center; height: 30px;">
                                <span class="label txt-color-red" id="spnNoRows" runat="server" visible="false" style="font-size: 100%;"></span>
                                <span class="label txt-color-red" id="spnRows" runat="server" visible="false" style="font-size: 100%;"></span>
                            </div>
                            <asp:Table ID="dt_basic" runat="server" ClientIDMode="Static" class="table table-striped table-bordered table-hover" Width="100%">
                                <asp:TableHeaderRow TableSection="TableHeader">
                                    <asp:TableHeaderCell Width="5%">ID</asp:TableHeaderCell>
                                    <asp:TableHeaderCell Width="15%">First Name</asp:TableHeaderCell>
                                    <asp:TableHeaderCell Width="15%">Last Name</asp:TableHeaderCell>
                                    <asp:TableHeaderCell Width="20%">Facility</asp:TableHeaderCell>
                                    <asp:TableHeaderCell Width="15%">Medical Record Number</asp:TableHeaderCell>
                                    <asp:TableHeaderCell Width="10%">Date of Birth (Age)</asp:TableHeaderCell>
                                    <asp:TableHeaderCell Width="10%">Last Encounter</asp:TableHeaderCell>
                                    <asp:TableHeaderCell Width="10%">Action</asp:TableHeaderCell>
                                </asp:TableHeaderRow>
                            </asp:Table>

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

        <!-- row -->
        <div class="row">
        </div>

        <!-- end row -->

    </section>
    <!-- end widget grid -->

</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="scriptsPlaceholder">
    <script type="text/javascript">
        $(function () {
            $('.datepicker').datepicker({
                prevText: '<i class="fa fa-chevron-left"></i>',
                nextText: '<i class="fa fa-chevron-right"></i>',
            });
            $('.datepicker').datepicker('option', 'maxDate', new Date());
        });
    </script>
</asp:Content>


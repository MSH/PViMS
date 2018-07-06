<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ReportCustom.aspx.cs" Inherits="PVIMS.Web.ReportCustom" MasterPageFile="~/Main.Master" %>

<asp:Content runat="server" ID="Breadcrumbcontainer" ContentPlaceHolderID="breadcrumbcontainer">
	<li>Reports</li>
</asp:Content>
<asp:Content runat="server" ID="Body" ClientIDMode="Static" ContentPlaceHolderID="BodyContentPlaceHolder">

    <asp:HiddenField runat="server" ID="hfPosition" Value="" />
	<div class="row">
		<div class="col-xs-12 col-sm-7 col-md-7 col-lg-4">
			<h1 class="page-title txt-color-blueDark">
				<i class="fa fa-table fa-fw "></i>
				Reports <span>> Custom</span>
			</h1>
		</div>
	</div>

	<!-- widget grid -->
	<section id="widget-grid" class="">

		<div class="row">

			<!-- NEW WIDGET START -->
			<div class="col-sm-12 col-md-12 col-lg-9">

				<div class="jarviswidget" id="wid-id-1" data-widget-editbutton="false" data-widget-custombutton="false" data-widget-deletebutton="false" data-widget-colorbutton="false">

					<header>
						<span class="widget-icon"> <i class="glyphicon glyphicon-stats txt-color-darken"></i> </span>
						<h2>Criteria </h2>

					</header>

					<!-- widget div-->
					<div>
						<!-- widget edit box -->
						<div class="jarviswidget-editbox">

						</div>
						<!-- end widget edit box -->

						<div class="widget-body padding">

                            <div style="padding:10px;" runat="server" id="divDefinition" visible="true">
                                <div class="row">
                                    <div class="col-md-8">
                                        <span class="col-md-6">
						                    <b>1. Define the report...</b>
                                        </span>
                                    </div>
                                </div>
                                <div class="row">
                                    <div class="smart-form">
                                        <fieldset>
                                            <div class="row" style="background-color:lightyellow;">
					                            <section class="col col-4">
						                            <label class="input" runat="server" id="lblReportName">Report Name
                                                        <input type="text" id="txtReportName" name="txtReportName" style="color:black;" runat="server" class="form-control" maxlength="50" />
						                            </label>
					                            </section>
                                            </div>
                                            <div class="row" style="background-color:lightyellow;">
					                            <section class="col col-10">
						                            <label class="input" runat="server" id="lblDefinition">Definition
                                                        <input type="text" id="txtDefinition" name="txtDefinition" style="color:black;" runat="server" class="form-control" maxlength="250" />
						                            </label>
					                            </section>
                                            </div>
                                        </fieldset>
                                    </div>
                                </div>
                            </div>

                            <div style="padding:10px;" runat="server" id="divType" visible="true">
                                <div class="row">
                                    <div class="col-md-8">
                                        <span class="col-md-6">
						                    <b>2. Select a report type...</b>
                                        </span>
                                    </div>
                                </div>
                                <div class="row">
                                    <div class="smart-form">
                                        <fieldset>
                                            <div class="row" style="background-color:lightyellow; padding:10px;">
					                            <section class="col col-3">
						                            <label class="select" runat="server" id="lblReportType">Report Type<br />
                                                        <asp:DropDownList ID="ddlType" name="ddlType" runat="server" style="color:black;" class="input-sm" AutoPostBack="true" OnSelectedIndexChanged="ddlType_SelectedIndexChanged">
                                                            <asp:ListItem Value="" Selected="True">-- Select a type --</asp:ListItem>
                                                            <asp:ListItem Value="Summary">Summary</asp:ListItem>
                                                            <asp:ListItem Value="List">List</asp:ListItem>
                                                        </asp:DropDownList>
						                            </label>
					                            </section>
                                            </div>
                                        </fieldset>
                                    </div>
                                </div>
                            </div>
                            <div style="padding:10px;" runat="server" id="divEntity" visible="false">
                                <div class="row">
                                    <div class="col-md-8">
                                        <span class="col-md-6">
						                    <b>3. Select an entity to report on...</b>
                                        </span>
                                    </div>
                                </div>
                                <div class="row">
                                    <div class="smart-form">
                                        <fieldset>
                                            <div class="row" style="background-color:lightyellow; padding:10px;">
					                            <section class="col col-4">
						                            <label class="select" runat="server" id="lblEntity">Core Entity<br />
                                                        <asp:DropDownList ID="ddlEntity" name="ddlEntity" runat="server" style="color:black;" class="input-sm" AutoPostBack="true" OnSelectedIndexChanged="ddlEntity_SelectedIndexChanged">
                                                            <asp:ListItem Value="" Selected="True">-- Select an entity --</asp:ListItem>
                                                        </asp:DropDownList>
						                            </label>
					                            </section>
                                            </div>
                                        </fieldset>
                                    </div>
                                </div>
                            </div>
                            <br/>
                            <div style="padding:10px;" id="divStratify" runat="server" visible="false">

                                <div class="row">
                                    <div class="col-md-8">
                                        <span class="col-md-6">
						                    <b>4. Stratify by the following attributes...</b>
                                        </span>
                                    </div>
                                </div>
                                <div class="row">
                                    <div class="smart-form">
                                        <fieldset>
                                            <div class="row" style="background-color:lightyellow; padding:10px;">
					                            <section class="col col-4">
						                            <label class="label">Attribute</label>
						                            <label class="select">
                                                        <asp:DropDownList ID="ddlElement" name="ddlElement" runat="server" style="color:black;" class="form-control">
                                                        </asp:DropDownList>
						                            </label>
					                            </section>
					                            <section class="col col-4">
						                            <label class="label">Display</label>
						                            <label class="input">
                                                        <input type="text" id="txtDisplayName" name="txtDisplayName" style="color:black;" runat="server" class="form-control" />
						                            </label>
					                            </section>
					                            <section class="col col-2">
                                                    <br /><br />
						                            <asp:LinkButton ID="btnAddStratify" runat="server" Text="Add new stratification" OnClick="btnAddStratify_Click" />
					                            </section>
                                            </div>
                                        </fieldset>
                                    </div>
                                </div>
                                <div class="well row">
                                    <div class="col-md-9">
                                        <table class="table" id="tblStratify" runat="server">
                                            <thead>
                                                <tr>
                                                    <th>Stratification</th>
                                                </tr>
                                            </thead>
                                            <tbody>
                                            </tbody>
                                        </table>
                                    </div>
                                </div>

                            </div>
                            <br />
                            <div style="padding:10px;" id="divList" runat="server" visible="false">

                                <div class="row">
                                    <div class="col-md-8">
                                        <span class="col-md-6">
						                    <b>4. List by the following attributes...</b>
                                        </span>
                                    </div>
                                </div>
                                <div class="row">
                                    <div class="smart-form">
                                        <fieldset>
                                            <div class="row" style="background-color:lightyellow; padding:10px;">
					                            <section class="col col-4">
						                            <label class="label">Attribute</label>
						                            <label class="select">
                                                        <asp:DropDownList ID="ddlListElement" name="ddlListElement" runat="server" style="color:black;" class="form-control">
                                                        </asp:DropDownList>
						                            </label>
					                            </section>
					                            <section class="col col-4">
						                            <label class="label">Display</label>
						                            <label class="select">
                                                        <input type="text" id="txtListDisplayName" name="txtListDisplayName" style="color:black;" runat="server" class="form-control" />
						                            </label>
					                            </section>
					                            <section class="col col-2">
                                                    <br /><br />
						                            <asp:LinkButton ID="btnAddList" runat="server" Text="Add new list" OnClick="btnAddList_Click" />
					                            </section>
                                            </div>
                                        </fieldset>
                                    </div>
                                </div>
                                <div class="well row">
                                    <div class="col-md-9">
                                        <table class="table" id="tblList" runat="server">
                                            <thead>
                                                <tr>
                                                    <th>List</th>
                                                </tr>
                                            </thead>
                                            <tbody>
                                            </tbody>
                                        </table>
                                    </div>
                                </div>

                            </div>
                            <br />
                            <div style="padding:10px;" runat="server" id="divFilter" visible="false">

                                <div class="row">
                                    <div class="col-md-8">
                                        <span class="col-md-6">
						                    <b>5. Filter by the following attributes...</b>
                                        </span>
                                    </div>
                                </div>
                                <div>
                                    <div class="smart-form">
                                        <fieldset>
                                            <div class="row" style="background-color:lightyellow; padding:10px;">
					                            <section class="col col-1" id="spnRelation" runat="server">
						                            <label class="label">Relationship</label>
						                            <label class="select">
                                                        <asp:DropDownList ID="ddlFilterRelation" name="ddlFilterRelation" runat="server" style="color:black;" class="form-control">
                                                            <asp:ListItem Value="And" Text="And" Selected="true" />
                                                            <asp:ListItem Value="Or" Text="Or" />
                                                        </asp:DropDownList>
						                            </label>
					                            </section>
					                            <section class="col col-3">
						                            <label class="label">Attribute</label>
						                            <label class="select">
                                                        <asp:DropDownList ID="ddlFilterElement" name="ddlFilterElement" runat="server" style="color:black;" class="form-control" OnSelectedIndexChanged="ddlFilterElement_SelectedIndexChanged" AutoPostBack="true">
                                                        </asp:DropDownList>
						                            </label>
					                            </section>
					                            <section class="col col-2">
						                            <label class="label">Operator</label>
						                            <label class="select">
                                                        <asp:DropDownList ID="ddlFilterOperator" name="ddlFilterOperator" runat="server" style="color:black;" class="form-control" OnSelectedIndexChanged="ddlFilterOperator_SelectedIndexChanged" AutoPostBack="true">
                                                        </asp:DropDownList>
						                            </label>
					                            </section>
					                            <section class="col col-3">
						                            <label class="label">Field Value</label>
						                            <label class="input" id="lblFilterNumericValue" runat="server">
                                                        <input type="number" id="txtFilterNumericValue" name="txtFilterNumericValue" style="color:black;" runat="server" class="form-control" />
						                            </label>
						                            <label class="input" id="lblFilterNumericRange" runat="server" visible="false">
                                                        <input type="number" id="txtFilterNumericFrom" name="txtFilterNumericFrom" style="color:black;" runat="server" class="form-control" /> and 
                                                        <input type="number" id="txtFilterNumericTo" name="txtFilterNumericTo" style="color:black;" runat="server" class="form-control" />
						                            </label>
						                            <label class="input" id="lblFilterTextValue" runat="server" visible="false">
                                                        <input type="text" id="txtFilterTextValue" name="txtFilterTextValue" style="color:black;" runat="server" class="form-control" />
						                            </label>
						                            <label class="input" id="lblFilterDateValue" runat="server" visible="false">
                                                        <input type="text" id="txtFilterDateValue" name="txtFilterDateValue" style="color:black;" runat="server" class="form-control" />
						                            </label>
						                            <label class="input" id="lblFilterDateRange" runat="server" visible="false">
                                                        <input type="text" id="txtFilterDateFrom" name="txtFilterDateFrom" style="color:black;" runat="server" class="form-control datepicker" /> and 
                                                        <input type="text" id="txtFilterDateTo" name="txtFilterDateTo" style="color:black;" runat="server" class="form-control datepicker" />
						                            </label>
						                            <label class="select" id="lblFilterSelectValue" runat="server" visible="false">
                                                        <asp:DropDownList ID="ddlFilterSelect" name="ddlFilterSelect" runat="server" style="color:black;" class="form-control">
                                                        </asp:DropDownList>
						                            </label>
						                            <label class="select" id="lblFilterInValue" runat="server" visible="false">
                                                        <asp:ListBox ID="lbFilterSelect" name="lbFilterSelect" runat="server" style="color:black; height:150px;" class="form-control" Rows="6" SelectionMode="Multiple">
                                                        </asp:ListBox>
						                            </label>
					                            </section>
					                            <section class="col col-2">
                                                    <br /><br />
						                            <asp:LinkButton ID="btnAddFilter" runat="server" Text="Add new filter" OnClick="btnAddFilter_Click" />
					                            </section>
                                            </div>
                                        </fieldset>
                                    </div>
                                </div>
                                <div class="well row">
                                    <div class="col-md-9">
                                        <table class="table " id="tblFilter" runat="server">
                                            <thead>
                                                <tr>
                                                    <th>Filter</th>
                                                </tr>
                                            </thead>
                                            <tbody>
                                            </tbody>
                                        </table>
                                    </div>
                                </div>

                            </div>
                                
                            <div class="smart-form">
				                <!-- end widget content -->
                                <footer>
                                    <asp:Button ID="btnPublish" runat="server" Text="Publish" OnClick="btnPublish_Click" class="btn btn-primary" />
                                    <asp:Button ID="btnSubmit" runat="server" Text="View Results" OnClick="btnSubmit_Click" class="btn btn-default" />
                                    <span id="spnButtons" runat="server" visible="false">

                                    </span>
                                </footer>
                            </div>
                            <div style="padding:10px;">
                                <span id="spnSummary" runat="server"></span>
                            </div>
						</div>
					</div>
				</div>
			</div>

        </div>

        <div class="row">

            <article class="col-xs-12 col-sm-12 col-md-12 col-lg-9">

                <div class="well">
                    <legend>Results</legend>
                    <div>
                        <div>
                            <div class="well well-sm" style="height:auto; padding-top:10px; padding-left:10px; padding-right:10px; padding-bottom:10px;text-align:right">
                                <label class="select"><b>Export to | </b></label>
                                <a href="javascript:void(0);" runat="server" onserverclick="btnExportXml_Click" title="XML"><i class="fa fa-lg fa-fw fa-file-text-o"></i></a>
                                <a href="javascript:void(0);" runat="server" onserverclick="btnExportXls_Click" title="EXCEL"><i class="fa fa-lg fa-fw fa-file-excel-o"></i></a>
                            </div>
                            <asp:GridView ID="gvOutput" AllowSorting="false" AllowPaging="false" runat="server" AutoGenerateColumns="true" Width="100%" CssClass="table table-striped table-bordered table-hover">
                                <Columns>
                                </Columns>
                            </asp:GridView>
                        </div>
                    </div>

                </div>

            </article>

        </div>

	</section>
	<!-- end widget grid -->

</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="scriptsPlaceholder">

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

</asp:Content>
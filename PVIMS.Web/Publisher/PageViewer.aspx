<%@ Page Language="C#" AutoEventWireup="True" CodeBehind="PageViewer.aspx.cs" MasterPageFile="/Main.Master" Inherits="PVIMS.Web.PageViewer" Title="Info Page Viewer" %>

<asp:Content runat="server" ID="Body" ContentPlaceHolderID="BodyContentPlaceHolder" ClientIDMode="Static">

	<!-- widget grid -->
	<section id="widget-grid" class="">

        <div id="spnUnpublished" runat="server" class="well row"><h2>Unpublished Widgets</h2></div>
        <div class="row">
		    <section class="col col-md-6">
                <div class="row">
                    <span id="spnTopLeft" runat="server"></span>
                </div>
                <div class="row">
                    <span id="spnMiddleLeft" runat="server"></span>
                </div>
                <div class="row">
                    <span id="spnBottomLeft" runat="server"></span>
                </div>
		    </section>
		    <div class="col col-md-6">
                <div class="row">
                    <span id="spnTopRight" runat="server"></span>
                </div>
                <div class="row">
                    <span id="spnMiddleRight" runat="server"></span>
                </div>
                <div class="row">
                    <span id="spnBottomRight" runat="server"></span>
                </div>
		    </div>
        </div>
	    <div class="row" id="divReturn" runat="server">
		    <div class="col-md-1 col-md-offset-11" id="divEdit" runat="server">
                <span id="spnButtons" runat="server"></span>
		    </div>
	    </div>

	</section>
	<!-- end widget grid -->

</asp:Content>

<asp:Content runat="server" ID="Scripts" ContentPlaceHolderID="scriptsPlaceholder">

    <script>

	</script>

</asp:Content>



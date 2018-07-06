<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SpontaneousAnalyser.aspx.cs" Inherits="PVIMS.Web.SpontaneousAnalyser" MasterPageFile="~/Main.Master" Title="Spontaneous Analyser" ViewStateMode="Enabled" %>

<%@ Register Assembly="System.Web.DataVisualization, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" Namespace="System.Web.UI.DataVisualization.Charting" TagPrefix="asp" %>
<%@ Register Assembly="CKEditor.NET" Namespace="CKEditor.NET" TagPrefix="CKEditor" %>

<asp:Content runat="server" ID="Breadcrumbcontainer" ContentPlaceHolderID="breadcrumbcontainer"></asp:Content>

<asp:Content runat="server" ID="Body" ClientIDMode="Static" ContentPlaceHolderID="BodyContentPlaceHolder">
    <asp:HiddenField runat="server" ID="hfPosition" Value="" />

    <section novalidate>
        
		<div class="row">
			<div class="col-xs-12 col-sm-7 col-md-7 col-lg-4">
				<h1 class="page-title txt-color-blueDark" id="hHead" runat="server">
					
				</h1>
			</div>
		</div>
				
		<!-- widget grid -->
		<div id="widget-grid" class="">

			<div class="row">
					
				<div class="col-sm-12 col-md-12 col-lg-6">

                    <div class="jarviswidget" id="wid-id-1"  data-widget-editbutton="false" data-widget-custombutton="false" data-widget-deletebutton="false" data-widget-colorbutton="false">
						<header>
							<span class="widget-icon"> <i class="fa fa-comments"></i> </span>
							<h2>Analysis Criteria for Spontaneous Reporting  </h2>
						</header>
								
						<!-- widget div-->
						<div>
									
							<!-- widget edit box -->
							<div class="jarviswidget-editbox">
								<!-- This area used as dropdown edit box -->
										
							</div>
							<!-- end widget edit box -->
									
							<!-- widget content -->
							<div class="widget-body padding" id="tabs">
                                <div id="divDownload" runat="server" visible="false">
                                    <div class="well bg-color-grayDark txt-color-white text-center">
                                        For third party statistical analysis, to download all spontaneous related data click the button below.
                                    </div>
                                    <div class="smart-form">
								        <footer>
                                            <a href="../FileDownload/DownloadSpontaneousDataset" class="btn btn-primary">Download Dataset</a>
								        </footer>
                                    </div>
                                </div>
							</div>
							<!-- end widget content -->
									
						</div>
						<!-- end widget div -->
								
					</div>

				</div>

			</div>
			<!-- end row -->

		</div>
		<!-- end widget grid -->

    </section>

</asp:Content>

<asp:Content runat="server" ID="Scripts" ContentPlaceHolderID="scriptsPlaceholder">

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
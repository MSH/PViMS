<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="PageCustomWidget.aspx.cs" Inherits="PVIMS.Web.PageCustomWidget" MasterPageFile="~/Main.Master" Title="Manage Page Widgets" %>

<%@ Register Assembly="CKEditor.NET" Namespace="CKEditor.NET" TagPrefix="CKEditor" %>

<asp:Content runat="server" ID="Breadcrumbcontainer" ContentPlaceHolderID="breadcrumbcontainer">
    <li>Administration</li>
</asp:Content>

<asp:Content runat="server" ID="Body" ClientIDMode="Static" ContentPlaceHolderID="BodyContentPlaceHolder">
    <asp:HiddenField runat="server" ID="hfPosition" Value="" />

	<style>
		/*
			* Glyphicons
			*
			* Special styles for displaying the icons and their classes in the docs.
			*/
		
		.bs-icons {
			padding-left: 0;
			padding-bottom: 1px;
			margin-bottom: 20px;
			list-style: none;
			overflow: hidden;
		}
		.bs-icons li {
			float: left;
			width: 25%;
			height: 85px;
			padding: 15px;
			margin: 0 -1px -1px 0;
			font-size: 12px;
			line-height: 1.4;
			text-align: center;
			border: 0px solid #ddd;
		}
		.bs-icons .icon {
			margin-top: 5px;
			margin-bottom: 10px;
			font-size: 24px;
		}
		.bs-icons .icon-class {
			display: block;
			text-align: center;
		}
		.bs-icons li:hover {
            cursor: pointer;
			background-color: rgba(86,61,124,.1);
		}
		
		@media (min-width: 768px) {
			.bs-icons li {
				width: 12.5%;
			}
		}
		
	</style>

	<!-- widget grid -->
	<section id="widget-grid" class="">

		<!-- row -->
		<div class="row">

			<!-- NEW COL START -->
			<article class="col-sm-12 col-md-12 col-lg-9">
				
				<!-- Widget ID (each widget will need unique ID)-->
				<div class="jarviswidget well" id="wid-id-1" 
                    data-widget-editbutton="false" 
                    data-widget-custombutton="false" 
                    data-widget-deletebutton="false" 
                    data-widget-colorbutton="false">

					<header>
						<span class="widget-icon"> <i class="fa fa-edit"></i> </span>
						<h2>Manage Page Widgets </h2>
				
					</header>
				
					<!-- widget div-->
					<div>
				
						<!-- widget edit box -->
						<div class="jarviswidget-editbox">
							<!-- This area used as dropdown edit box -->
				
						</div>
						<!-- end widget edit box -->
				
						<!-- widget content -->
						<div class="widget-body no-padding smart-form">

							<fieldset>
                                <legend>Page</legend>
								<div class="row">
                                    <section class="col col-2">
										<label class="input">Unique ID
											<input class="form-control" id="txtPageID" type="text" runat="server" maxlength="50" readonly="readonly" style="background-color:#92A2A8; color:white;">
                                        </label>
                                    </section>
								</div>
								<div class="row">
                                    <section class="col col-8">
										<label class="input">Page Name
											<input class="form-control" id="txtPageName" type="text" runat="server" maxlength="50" readonly="readonly" style="background-color:#92A2A8; color:white;">
                                        </label>
                                    </section>
								</div>
							</fieldset>

                            <div id="divMainContent" runat="server">
							    <fieldset>
                                    <legend>Name</legend>
								    <div class="row">
                                        <section class="col col-4">
										    <label class="input">Unique ID
											    <input class="form-control" id="txtUID" type="text" runat="server" maxlength="50" readonly="readonly" style="background-color:#EBEBE4;">
                                            </label>
                                        </section>
								    </div>
								    <div class="row">
                                        <section class="col col-4">
										    <label class="input" id="lblName" runat="server">Widget Name
											    <input class="form-control" id="txtName" type="text" runat="server" maxlength="50" required>
                                            </label>
                                        </section>
								    </div>
								    <div class="row" style="display:none;">
                                        <section class="col col-8">
										    <label class="input">Widget Definition
											    <input class="form-control" id="txtDefinition" type="text" runat="server" maxlength="250">
                                            </label>
                                        </section>
								    </div>
							    </fieldset>

							    <fieldset>
                                    <legend>Details</legend>
								    <div class="row">
                                        <section class="col col-3">
										    <label class="input" id="lblWidgetType" runat="server">Widget Type
                                                <asp:DropDownList ID="ddlWidgetType" runat="server" CssClass="input-sm form-control" AutoPostBack="false">
                                                </asp:DropDownList>
                                            </label>
                                        </section>
                                        <section class="col col-3">
										    <label class="input" id="lblWidgetStatus" runat="server">Widget Status
                                                <input class="form-control" id="txtWidgetStatus" type="text" runat="server" readonly="readonly" style="background-color:#EBEBE4;">
                                                <asp:DropDownList ID="ddlWidgetStatus" runat="server" CssClass="input-sm form-control">
                                                    <asp:ListItem Value="1" Text="Published" selected="True"></asp:ListItem>
                                                    <asp:ListItem Value="2" Text="Unpublished"></asp:ListItem>
                                                </asp:DropDownList>
                                            </label>
                                        </section>
                                    </div>
								    <div class="row" id="divWidgetLocation" runat="server">
                                        <section class="col col-3">
										    <label class="input">Current Location
											    <input class="form-control" id="txtWidgetLocation" type="text" runat="server" maxlength="50" readonly="readonly" style="background-color:#EBEBE4;">
                                            </label>
                                        </section>
                                        <section class="col col-3">
										    <label class="input" id="lblWidgetLocation" runat="server">Widget Location
                                                <asp:DropDownList ID="ddlWidgetLocation" runat="server" CssClass="input-sm form-control">
                                                </asp:DropDownList>
                                            </label>
                                        </section>
                                    </div>
							    </fieldset>

							    <fieldset>
                                    <legend>Icon</legend>
										<ul class="bs-icons">
											<li id="fa-bar-chart-o">
												<i class="fa fa-lg fa-fw fa-4x fa-bar-chart-o"></i>
												<span class="icon-class">fa-bar-chart-o</span>
											</li>
											<li id="fa-book">
												<i class="fa fa-lg fa-fw fa-4x fa-book"></i>
												<span class="icon-class">fa-book</span>
											</li>
											<li id="fa-bullhorn">
												<i class="fa fa-lg fa-fw fa-4x fa-bullhorn"></i>
												<span class="icon-class">fa-bullhorn</span>
											</li>
											<li id="fa-calendar">
												<i class="fa fa-lg fa-fw fa-4x fa-calendar"></i>
												<span class="icon-class">fa-calendar</span>
											</li>
											<li id="fa-check">
												<i class="fa fa-lg fa-fw fa-4x fa-check"></i>
												<span class="icon-class">fa-check</span>
											</li>
											<li id="fa-check-square-o">
												<i class="fa fa-lg fa-fw fa-4x fa-check-square-o"></i>
												<span class="icon-class">fa-check-square-o</span>
											</li>
											<li id="fa-compass">
												<i class="fa fa-lg fa-fw fa-4x fa-compass"></i>
												<span class="icon-class">fa-compass</span>
											</li>
											<li id="fa-exclamation">
												<i class="fa fa-lg fa-fw fa-4x fa-exclamation"></i>
												<span class="icon-class">fa-exclamation</span>
											</li>
											<li id="fa-folder-o">
												<i class="fa fa-lg fa-fw fa-4x fa-folder-o"></i>
												<span class="icon-class">fa-folder-o</span>
											</li>
											<li id="fa-folder-open-o">
												<i class="fa fa-lg fa-fw fa-4x fa-folder-open-o"></i>
												<span class="icon-class">fa-folder-open-o</span>
											</li>
											<li id="fa-globe">
												<i class="fa fa-lg fa-fw fa-4x fa-globe"></i>
												<span class="icon-class">fa-globe</span>
											</li>
											<li id="fa-info">
												<i class="fa fa-lg fa-fw fa-4x fa-info"></i>
												<span class="icon-class">fa-info</span>
											</li>
											<li id="fa-info-circle">
												<i class="fa fa-lg fa-fw fa-4x fa-info-circle"></i>
												<span class="icon-class">fa-info-circle</span>
											</li>
											<li id="fa-key">
												<i class="fa fa-lg fa-fw fa-4x fa-key"></i>
												<span class="icon-class">fa-key</span>
											</li>
											<li id="fa-legal">
												<i class="fa fa-lg fa-fw fa-4x fa-legal"></i>
												<span class="icon-class">fa-legal</span>
											</li>
											<li id="fa-lightbulb-o">
												<i class="fa fa-lg fa-fw fa-4x fa-lightbulb-o"></i>
												<span class="icon-class">fa-lightbulb-o</span>
											</li>
											<li id="fa-medkit">
												<i class="fa fa-lg fa-fw fa-4x fa-medkit"></i>
												<span class="icon-class">fa-medkit</span>
											</li>
											<li id="fa-pencil">
												<i class="fa fa-lg fa-fw fa-4x fa-pencil"></i>
												<span class="icon-class">fa-pencil</span>
											</li>
											<li id="fa-star-o">
												<i class="fa fa-lg fa-fw fa-4x fa-star-o"></i>
												<span class="icon-class">fa-star-o</span>
											</li>
                                        </ul>
								    <div class="row">
                                        <section class="col col-3">
										    <label class="input" id="lblWidgetIcon" runat="server">Icon
                                                <input class="form-control" id="txtIcon" type="text" runat="server" maxlength="50" readonly="readonly" style="background-color:#EBEBE4;">
                                            </label>
                                        </section>
                                    </div>
							    </fieldset>
                            </div>

                            <div id="divWikiContent" runat="server">
                                <div id="divContentGeneral" runat="server">
                                    <fieldset>
                                        <legend>General Content</legend>
								        <div class="row">
                                            <div id="divContent" basepath="/ckeditor/" runat="server">
                                                <CKEditor:CKEditorControl ID="CKEditor1" runat="server">
                                                </CKEditor:CKEditorControl>
                                            </div>
								        </div>
                                    </fieldset>
                                </div>

                                <div id="divContentWiki" runat="server">
                                    <fieldset>
                                        <legend>Wiki Content</legend>
                                    </fieldset>

                                    <span id="spnWikiList" runat="server">

                                    </span>
                                </div>

                                <div id="divContentItemList"  runat="server">
                                    <fieldset>
                                        <legend>Item Content</legend>
                                        <span id="spnItemList" runat="server">

                                        </span>
                                    </fieldset>
                                </div>
                            </div>

                            <br />
                            <div class="smart-form">
							    <footer>
								    <span id="spnButtons" runat="server">

								    </span>
							    </footer>
                            </div>      		

                        </div>
						<!-- end widget content -->
				
					</div>
					<!-- end widget div -->
				
				</div>
				<!-- end widget -->
				
			</article>
			<!-- END COL -->					

		</div>
		
	</section>

    <!-- ui-dialog -->
    <div id="dialog_warning_maximum" title="Dialog Simple Title">
	    <p>
            Maximum number of items reached
	    </p>
    </div>
    <div id="dialog_warning_minimum" title="Dialog Simple Title">
	    <p>
            Mimimum number of items reached
	    </p>
    </div>

    <!-- use this modal to add a new page to the system -->
    <div id="dialog-add-new-page" title="Dialog Simple Title">
	    <p>
		    Please specify the main details for the new page...
	    </p>
		<div class="form-group">
			<label for="txtNewPageName">Page Name:</label>
            <input type="text" id="txtNewPageName" class="form-control" />
		</div>

	    <div class="hr hr-12 hr-double"></div>
    </div>
    
    <div id="dialog_page_info_success">
	    <p><br />
            Page has been added successfully ...
	    </p>
    </div>
    <div id="dialog_page_info_error">
	    <p><br />
            Page has NOT been added successfully ...
	    </p>
    </div>

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

        $('.btn-add').click(function () {
            //var num_tabs = $("ul.nav li").length;
            var last_tab_index = $("ul.nav").find("li:visible:last").index();

            if (last_tab_index >= 14)
            {
                $('#dialog_warning_maximum').dialog('open');
                return false;
            };

            // remove active style for current tab and div
            $("ul.nav li.active").removeClass("active");
            $(".tab-content div.active").removeClass("active");

            // Get next tab and set as active
            $("#li-" + (last_tab_index + 2)).addClass("active");
            $("#li-" + (last_tab_index + 2)).show();
            $("#tab-" + (last_tab_index + 2)).addClass("active");

            return false;
        });

        $('.btn-remove').click(function () {
            var last_tab_index = $("ul.nav").find("li:visible:last").index();

            if (last_tab_index < 1) {
                $('#dialog_warning_minimum').dialog('open');
                return false;
            };

            // remove active style for current tab and div
            $("ul.nav li.active").removeClass("active");
            $(".tab-content div.active").removeClass("active");

            // Remove content from last tab
            var title = $("#tab-" + (last_tab_index + 1)).find("#title-" + (last_tab_index + 1));
            title.val('');
            var subtitle = $("#tab-" + (last_tab_index + 1)).find("#subtitle-" + (last_tab_index + 1));
            if (subtitle != undefined) { subtitle.val('') };

            // hide last tab
            $("#li-" + (last_tab_index + 1)).hide();

            // Get previous tab and set as active
            $("#li-" + last_tab_index).addClass("active");
            $("#li-" + last_tab_index).show();
            $("#tab-" + last_tab_index).addClass("active");

            return false;
        });

        $('#dialog_warning_maximum').dialog({
            autoOpen: false,
            width: 600,
            resizable: false,
            modal: true,
            title: "Unable to add new item",
            buttons: [{
                html: "Close",
                "class": "btn btn-primary",
                click: function () {
                    $(this).dialog("close");
                }
            }]
        });

        $('#dialog_warning_minimum').dialog({
            autoOpen: false,
            width: 600,
            resizable: false,
            modal: true,
            title: "Unable to remove item",
            buttons: [{
                html: "Close",
                "class": "btn btn-primary",
                click: function () {
                    $(this).dialog("close");
                }
            }]
        });

        $(".bs-icons li").click(function () {
            $("#txtIcon").val(this.id);
        });

         $('.btn-add-page').click(function(e){
             e.preventDefault();
             $('#dialog-add-new-page').dialog('open');
        });

        $('#dialog_page_info_error').dialog({
            autoOpen: false,
            width: 600,
            resizable: false,
            modal: true,
            dialogClass: "no-titlebar",
            buttons: [{
                html: "Close",
                "class": "btn btn-primary",
                click: function () {
                    $(this).dialog("close");
                }
            }]
        });

        $('#dialog_page_info_success').dialog({
            autoOpen: false,
            width: 600,
            resizable: false,
            modal: true,
            dialogClass: "no-titlebar",
            buttons: [{
                html: "Close",
                "class": "btn btn-primary",
                click: function () {
                    $(this).dialog("close");
                    location.reload();
                }
            }]
        });
        $(".ui-dialog-titlebar").hide();

        $("#dialog-add-new-page").dialog({
            autoOpen: false,
            modal: true,
            title: "Confirm",
            buttons: [{
                html: "Cancel",
                "class": "btn btn-default",
                click: function () {
                    $(this).dialog("close");
                }
            }, {
                html: "<i class='fa fa-check'></i>&nbsp; OK",
                "class": "btn btn-primary",
                click: function () {
                    $(this).dialog("close");

                    var link = "Linked to widget on page " + $("#txtPageName").val();

                    $.ajax({
                        url: "/Publisher/AddMetaPage",
                        data: { pageName: $("#txtNewPageName").val(), widgetName: link },
                        cache: false, 
                        type: "POST",
                        dataType: "html",
                        beforeSend: function () {
                        },
                        success: function (data, textStatus, XMLHttpRequest) {
                            var response = JSON.parse(data);

                            if (response.Success == "OK") {
                                $('#dialog_page_info_success').dialog('open');
                            }
                            else {
                                $('#dialog_page_info_error').dialog('open');
                            }
                        },
                        error: function () {
                            $('#dialog_page_info_error').dialog('open');
                        }
                    });
                }
            }]
        });

        $('#ddlWidgetStatus').change(function (e) {
            if ($('#ddlWidgetStatus').val() == "1") {
                $('#divWidgetLocation').show();
            }
            else {
                $('#divWidgetLocation').hide();
            }
        });

    </script>

</asp:Content>
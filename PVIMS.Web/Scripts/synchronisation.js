define('statusController',
    ['config', 'jquery', 'notify'],
    function (config, $) {
        var displayOnlineStatus = $('#onlinestatus'),
            isOnline = function () {
                displayOnlineStatus.switchClass('txt-color-red', 'txt-color-darkgreen');
                displayOnlineStatus.text("Online");
            },

            isOffline = function () {
                displayOnlineStatus.switchClass('txt-color-green', 'txt-color-red');
                displayOnlineStatus.text("Offline");
            },
            
            runCheckConnection = function () {
                if (Offline.state === 'up')
                    Offline.check();
            };

        setInterval(runCheckConnection, 3000);

        var notifyIsOn = true;

        Offline.on('confirmed-down', function () {
            isOffline();
            config.appIsOnline = false;

            if($("#main").attr("data-mode") == "Online")
            {
                if(!notifyIsOn)
                {                    
                    $.SmartMessageBox({
                        title: "<i class='fa fa-sign-out txt-color-orangeDark'></i> <span class='txt-color-orangeDark'><strong>Offline Alert!</strong></span>",
                        content: "You lost connection! Would you like to change to offline mode?",
                        buttons: "[Change to Offline][Stay Online]"
                    }, function (ButtonPressed) {
                        if (ButtonPressed === "Change to Offline") {
                            window.location.href = "/offline/index";
                        }
                        if (ButtonPressed === "Stay Online") {
                            // Do nothing
                        }

                    });

                    $('.botTempo').filter(function () {
                        return $.trim($(this).text()) == "No"
                    }).css('visibility', 'hidden');
                    
                    notifyIsOn = true;
                }                
            }
            else
            {
                if (notifyIsOn)
                {
                    $('.notifyjs-wrapper').trigger('notify-hide');  //$(".notifyjs-corner").hide();
                    notifyIsOn = false;
                }
            }
        });

        Offline.on('confirmed-up', function () {
            isOnline();
            config.appIsOnline = true;

            if ($("#cacheProgress").length)
                return;

            if ($("#main").attr("data-mode") == "Offline") {
                if (!notifyIsOn) {
                    $.notify({
                        title: 'You are connected! Would you like to change to online mode?',
                        button1: 'Online'
                    }, {
                        style: 'connection',
                        autoHide: false,
                        clickToHide: false
                    });

                    notifyIsOn = true;
                }
            }
            else {
                if (notifyIsOn)
                {
                    // Close offline alert and show connection is back
                    $('.botTempo').filter(function () {
                        return $.trim($(this).text()) == "No"
                    }).click();
                    notifyIsOn = false;
                }
            }
        });

        // subscribe for status indicator change
        config.idleStatusIndicator.subscribe(function (newValue) {
            var statusIndicator = $('#synchronisationstatus');
            statusIndicator.text(newValue);
        });

        //add a new style 'connection'
        $.notify.addStyle('connection', {
            html:
              "<div>" +
                "<div class='clearfix'>" +
                  "<div class='title' data-notify-html='title'/>" +
                  "<div class='buttons'>" +
                    "<button class='yes' data-notify-text='button1'></button>" +
                  "</div>" +
                "</div>" +
              "</div>"
        });

        //listen for click events from this style
        $(document).on('click', '.notifyjs-connection-base .no', function () {
            notifyIsOn = false;
            //programmatically trigger propogating hide event
            $(this).trigger('notify-hide');
        });
        $(document).on('click', '.notifyjs-connection-base .yes', function () {
            //show button text
            //alert($(this).text() + " clicked!");
            
            window.location.href = "/Patient/PatientSearch.aspx";
        });
    });

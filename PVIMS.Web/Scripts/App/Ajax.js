var Ajax = {}
Ajax.Success = "Success";
Ajax.Error = Ajax.Failed = "Failed";
Ajax.registerGlobalHandlers = function() {
	$(document).ajaxError(function (event, jqxhr, settings, thrownError) {
		
		$.smallBox({
			title: "Error",
			content: thrownError,
			color: "#c26565",
			//timeout: 8000,
			icon: "fa fa-bell swing animated"
		});
		console.log(jqxhr);
	});
}

// global ajax functions
$(function() {
	Ajax.registerGlobalHandlers();
});
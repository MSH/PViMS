window.Enums = {};
var _app = function() {
	var self = this;
	self.initializePopovers =  function() {
			$('[data-toggle="popover"').popover();
	}
	
	self.init = function() {
		$('.org-unit-selector').orgUnitSelector();
	}
}

var App = new _app();

$(function() {
	App.init();
});



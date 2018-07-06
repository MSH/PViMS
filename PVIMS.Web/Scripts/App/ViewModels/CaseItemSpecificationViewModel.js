var CaseItemSpecificationViewModel = function(input, model) {
	var self = this;
	self.value = ko.observable();
	self.rule = ko.observable(new SpecificationViewModel(input));
	self.edit= function(modalElement) {
		var context = ko.dataFor($(modalElement)[0]);
		context.Child(self.rule());
		$(modalElement).modal('show');
	}
	self.friendlyString = function () {
		return ' WHEN ' + self.rule().friendlyString() + ' THEN ' + self.value();
	}
	self.toJs = function () {
		return {
			value: self.value(),
			rule: self.rule().toJs()
		};
	}
	if (model) {
		self.value(model.value);
		self.rule().load(model.rule);
	}
}
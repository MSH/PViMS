var AndOrSpecificationViewModel = function (parent, op) {
	var self = this;
	self.parent = ko.observable(parent);
	self.input = ko.computed(function() {
		return self.parent().input();
	});

	//self.enumValues = ko.observableArray();
	self.left = ko.observable(new SpecificationViewModel(null, self));
	self.right = ko.observable(new SpecificationViewModel(null, self));
	self.op = ko.observable(op);
	//self.inputName.subscribe(function(newValue) {
	//	self.left().inputName(newValue);
	//	self.right().inputName(newValue);
	//});
	//self.inputType.subscribe(function (newValue) {
	//	self.left().inputType(newValue);
	//	self.right().inputType(newValue);
	//	if (self.left().enumValues) {
	//		self.right().enumValues(self.enumValues());
	//	}
	//});

	self.friendlyHtml = ko.computed(function () {
		if (self.left() && self.right()) {
			return '(' + self.left().friendlyHtml() + ' <label class="label label-info">' + op + '</label> ' + self.right().friendlyHtml() + ')';
		}
		return '';
	});

	self.friendlyString = ko.computed(function () {
		if (self.left() && self.right()) {
			return '(' + self.left().friendlyString() + ' ' + op + ' ' + self.right().friendlyString() + ')';
		}
		return '';
	});


	self.toJs = ko.computed(function() {
		return {
			//inputName: self.inputName(),
			//inputType: self.inputType(),
			left: self.left() ? self.left().toJs() : null,
			right: self.right() ? self.right().toJs() : null,
			op: self.op()
		};
	});

	self.load = function (model) {
		//self.inputName(model.inputName);
		//self.inputType(model.inputType);
		self.op(model.op);
		self.left().load(model.left);
		self.right().load(model.right);

	}
}
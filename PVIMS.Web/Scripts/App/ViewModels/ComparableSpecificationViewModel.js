var ComparableSpecificationViewModel = function (op, parent) {
	var self = this;
	self.parent = ko.observable(parent);
	self.value = ko.observable();
	self.op = ko.observable(op);

	self.input = ko.computed(function() {
		return self.parent().input();
	});

	self.enumValues = ko.computed(function() {
		return self.parent().input().enumValues;
	});
 
	self.hasEnumValues = ko.computed(function() {
		return self.parent().input().enumValues && self.parent().input().enumValues.length > 0;
	});

	self.friendlyHtml = ko.computed(function () {
		if (op === "Equals") {
			return self.input().name + ' <label class="label label-success">=</label> ' + (self.value() || '...');
		}
		if (op === "GreaterThan") {
			return self.input().name + ' <label class="label label-success">&gt;</label> ' + (self.value() || '...');
		}
		if (op === "LessThan") {
			return self.input().name + ' <label class="label label-success">&lt;</label> ' + (self.value() || '...');
		}
	});

	self.friendlyString = ko.computed(function () {
		if (op === "Equals") {
			return self.input().name + ' = ' + (self.value() || '...');
		}
		if (op === "GreaterThan") {
			return self.input().name + ' > ' + (self.value() || '...');
		}
		if (op === "LessThan") {
			return self.input().name + ' < ' + (self.value() || '...');
		}
	});

	self.toJs = ko.computed(function() {
		return {
			//inputName: self.inputName(),
			//inputType: self.inputType(),
			value: self.value(),
			op: self.op()
		};
	});

	self.load = function (model) {
		//self.inputName(model.inputName);
		//self.inputType(model.inputType);
		self.value(model.value);
	}
}
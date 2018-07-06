var PropertyRuleSpecificationViewModel = function (parent) {
	var self = this;
	self.parent = ko.observable(parent);
	self.input = ko.computed(function () {
		return self.parent().input();
	});

	self.selectedType = ko.observable();
	self.rule = ko.observable(new SpecificationViewModel(self.selectedType(), self));
	self.isLoading = ko.observable(true);

	self.availableProperties = ko.observableArray();

	self.selectedType.subscribe(function (newValue) {
		//self.rule().inputName(self.inputName() + '.' + newValue.name());
		//self.rule().inputType(newValue.systemType());
		//self.rule().enumValues(newValue.enumValues());
		// -> junk. TODO: make it use input()
		self.rule()._input(self.selectedType());
	});

	self.friendlyHtml = ko.computed(function () {
		return self.input().name + '.' + self.rule().friendlyHtml();
	});

	self.friendlyString = ko.computed(function () {
		return self.input().name + '.' + self.rule().friendlyString();
	});

	self.toJs = ko.computed(function () {
		if (!self.selectedType()) {
			return null;
		}
		return {
			//inputName: self.inputName(),
			//inputType: self.inputType(),
			systemType: self.selectedType().systemType,
			name: self.selectedType().name,
			rule: self.rule().toJs()
		};
	});

	self.load = function (model) {
		//self.inputName(model.inputName);
		//self.inputType(model.inputType);
		self.isLoading(true);
		self.getAvailableProperties(function() {
			for (var i = 0; i < self.availableProperties().length; i++) {
				if (self.availableProperties()[i].name === model.name) {
					self.selectedType(self.availableProperties()[i]);
				}
			}
			self.rule().load(model.rule);
		});
	}

	self.getAvailableProperties = function (callback) {
		$.getJSON('/Program/GetPropertiesForType', { typeName: self.input().systemType }, function (data) {
			self.availableProperties.removeAll();
			for (var i = 0; i < data.length; i++) {
				self.availableProperties.push(data[i]);
			}
			self.isLoading(false);
			if ($.isFunction(callback)) {
				callback();
			}
		});
	}

	self.getAvailableProperties();
}
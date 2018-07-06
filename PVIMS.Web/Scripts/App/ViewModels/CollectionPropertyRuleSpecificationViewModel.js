var CollectionPropertyRuleSpecificationViewModel = function (operator, parent) {
	var self = this;
	//self.inputName = ko.observable(inputName);
	//self.inputType = ko.observable(inputType);
	self.parent = ko.observable(parent);
	self.input = ko.computed(function () {
		return self.parent().input();
	});
	//self.enumValues = ko.observableArray();
	self.selectedType = ko.observable();
	self.filterRule = ko.observable(new SpecificationViewModel(self.selectedType(), self));
	self.aggregationOperator = ko.observable(operator);
	self.aggregationRule = ko.observable(new SpecificationViewModel({name : '', systemType : 'System.Int32'}, self));
	self.availableProperties = ko.observableArray();
	self.isLoading = ko.observable(true);

	self.selectedType.subscribe(function(newValue) {
		if (newValue) {
			// -> junk. TODO: make it use input()
			self.filterRule()._input({
				name: newValue.name,
				systemType: newValue.itemSystemType,
				enumValues: newValue.itemEnumValues
			});
		} else {
			self.filterRule()._input({});
		}
	});



	self.friendlyHtml = ko.computed(function () {
		return self.aggregationOperator() + "(" + self.filterRule().friendlyHtml() + ")" + self.aggregationRule().friendlyHtml() ;
	});

	self.friendlyString = ko.computed(function () {
		return self.aggregationOperator() + "(" + self.filterRule().friendlyString() + ")" + self.aggregationRule().friendlyString();
	});

	self.toJs = ko.computed(function () {
		if (!self.selectedType()) {
			return null;
		}
		return {
			itemSystemType: self.selectedType().itemSystemType,
			name: self.selectedType().name,
			filterRule: self.filterRule().toJs(),
			aggregationOperator : self.aggregationOperator(),
			aggregationRule : self.aggregationRule().toJs()
		};
	});

	self.load = function (model) {
		//self.inputName(model.inputName);
		//self.inputType(model.inputType);
		self.isLoading(true);
		self.getAvailableProperties(function () {
			self.aggregationOperator(model.aggregationOperator);
			for (var i = 0; i < self.availableProperties().length; i++) {
				if (self.availableProperties()[i].name === model.name) {
					self.selectedType(self.availableProperties()[i]);
				}
			}
			self.filterRule().load(model.filterRule);			
			self.aggregationRule().load(model.aggregationRule);
		});
	}

	self.getAvailableProperties = function (callback) {
		$.getJSON('/Program/GetPropertiesForType', { typeName: self.input().systemType }, function (data) {
			self.availableProperties.removeAll();
			for (var i = 0; i < data.length; i++) {
				if (data[i].isCollection) {
					self.availableProperties.push(data[i]);
				}
			}
			self.isLoading(false);
			if ($.isFunction(callback)) {
				callback();
			}
		});
	}

	self.getAvailableProperties();
}
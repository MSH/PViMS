var SpecificationViewModel = function(input, parent) {
	var self = this;

	self._input = ko.observable(input);
	self._parent = parent;
	self.input = function() {
		if (self._input() === null) {
			return self._parent.input();
		}
		return self._input();
	}


	self.type = ko.observable();
	self.child = ko.observable();
	self.templateName = ko.observable();
	self.type.subscribe2(function (newValue,oldValue) {
		self.templateName("placeholder-specification-editor");
		if (newValue === "Or" || newValue === "And") {
			var child = new AndOrSpecificationViewModel(self, newValue);
			if (self.child()) {
				var newSpecModel = new SpecificationViewModel(null, child);
				newSpecModel.type(oldValue);
				newSpecModel.child(self.child());
				
				child.left(newSpecModel);
			}
			self.child(child).templateName("and-or-specification-editor");
			return;
		}
		if (newValue === "Equals" || newValue === "GreaterThan" || newValue === "LessThan") {
			self.child(new ComparableSpecificationViewModel(newValue, self)).templateName("comparable-specification-editor");
			return;
		}
		if (newValue === "Property") {
			self.child(new PropertyRuleSpecificationViewModel(self)).templateName("property-specification-editor");
			return;
		}
		if (newValue === "Count") {
			self.child(new CollectionPropertyRuleSpecificationViewModel("Count", self)).templateName("collection-property-specification-editor");
			return;
		}
		self.templateName("placeholder-specification-editor").child(null);
	});


	self.friendlyHtml = ko.computed(function() {
		if (self.child() == null) {
			return "";
		}

		return self.child().friendlyHtml();
	});

	self.friendlyString = ko.computed(function () {
		if (self.child() == null) {
			return "";
		}
		
		return self.child().friendlyString();
	});

	self.toJs = ko.computed(function() {
		return {
			type: self.type(),
			data: self.child() ? self.child().toJs() : null
		};
	});

	self.load = function (model) {
		if (model && model.type) {
			self.type(model.type);
			if (self.child()) {
				self.child().load(model.data);
			}
		}
	}
}
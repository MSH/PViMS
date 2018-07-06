var CaseSpecificationViewModel = function(input) {
	var self = this;
	self.defaultValue = ko.observable();
	self.cases = ko.observableArray();

	self.load = function(model) {
		self.defaultValue(model.defaultValue);
		if (model.cases) {
			for (var i = 0; i < model.cases.length; i++) {
				self.cases.push(new CaseItemSpecificationViewModel(input, model.cases[i]));
			}
		}
	}

	self.addCase = function() {
		self.cases.push(new CaseItemSpecificationViewModel(input));
	}
	self.deleteCase = function(caseItem) {
		self.cases.remove(caseItem);
	}
	self.friendlyString = function () {
		if (self.cases().length == 0) {
			return self.defaultValue();
		}
		var fstring = '';
		for (var i = 0; i < self.cases().length; i++) {
			fstring = fstring + self.cases()[i].friendlyString();
		}

		return (fstring + ' ELSE ' + self.defaultValue()).trim();

	}
	self.toJs = function () {
		var casesArray = new Array();
		for (var i = 0; i < self.cases().length; i++) {
			casesArray.push(self.cases()[i].toJs());
		}

		return {
			defaultValue: self.defaultValue(),
			cases : casesArray
		};
	};

}

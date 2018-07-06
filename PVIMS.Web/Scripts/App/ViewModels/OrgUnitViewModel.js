var OrgUnitViewModel = function(model) {
	var self = this;

	self.id = ko.observable(model.id);
	self.name = ko.observable(model.name);
	self.orgUnitType = ko.observable(model.orgUnitType);
	self.children = ko.observableArray();
	
	self.checked = ko.observable(false);
	self.expanded = ko.observable(false);
	self.isLoading = ko.observable();
	self.orgUnitTypeName = ko.observable();
	self.filter = ko.observable();
	
	self.areEventsEnabled = true;

	self.icon = ko.computed(function() {
		switch (self.orgUnitType()) {
		case Enums.OrgUnitTypes.National:
			return "fa-globe";
		case Enums.OrgUnitTypes.District:
			return "fa-flag";
		case Enums.OrgUnitTypes.CommunityCouncil:
			return "fa-university";
		case Enums.OrgUnitTypes.ElectoralDevision:
			return "fa-certificate";
		case Enums.OrgUnitTypes.Village:
			return "fa-home";
		default:
			return "";
		}
	});

	self.filter.subscribe(function(newValue) {
		if (newValue.length > 2) {
			$.get('/OrgUnit/Search?name=' + newValue, function(data) {
				self.load(data);
			});
		}
	});

	self.filteredChildren = ko.computed(function () {
		if (!self.filter() || self.filter().length < 2) {
			return self.children();
		}

		var result = new Array;
		for (var i = 0; i < self.children().length; i++) {
			if (self.children()[i].name().toLowerCase().indexOf(self.filter().toLowerCase()) === 0) {
				result.push(self.children()[i]);
			}
		}
		return result;
	});

	self.selectedIds = ko.computed(function() {
		var listToReturn = [];
		if (self.checked() === true) {
			return [self.id()];
		}

		//var comma = "";
		for (var i = 0; i < self.children().length; i++) {
			listToReturn = listToReturn.concat(self.children()[i].selectedIds());
		}

		return listToReturn;

	});
	self.toCommaSeperatedList = ko.computed(function() {
		var result = "";
		var comma = "";
		var list = self.selectedIds();
		for (var j = 0; j < list.length; j++) {
			result += comma + list[j];
			comma = ", ";
		}
		return result;
	});

	// event listeners
	self.checked.subscribe(function(newValue) {
		if (self.areEventsEnabled) {
			for (var i = 0; i < self.children().length; i++) {
				self.children()[i].checked(newValue);
			}
		}
	});

	self.expanded.subscribe(function(newValue) {
		if (self.areEventsEnabled) {


			if (!newValue) {
				return;
			}
			if (self.children().length === 0) {
				self.refresh();
			}
		}
	});


	// Commands
	self.toggleExpanded = function() {
		self.expanded(!self.expanded());
	};
	self.refresh = function(callback) {
		self.isLoading(true);
		$.get("/OrgUnit/GetChildren?id=" + self.id(), function(data) {
			self.children.removeAll();
			for (var i = 0; i < data.length; i++) {
				var child = new OrgUnitViewModel(data[i]);
				if (self.checked() === true) {
					child.checked(true);
				}
				child.checked.subscribe(self.refreshChecked);
				self.children.push(child);
			}
			self.isLoading(false);
			if ($.isFunction(callback)) {
				callback();
			}
		});

	};
	self.refreshChecked = function(newValue) {
		self.areEventsEnabled = false;
		if (newValue === false) {
			self.checked(false);
		} else {
			var allchecked = true;
			for (var j = 0; j < self.children().length; j++) {
				if (!self.children()[j].checked()) {
					allchecked = false;
				}
			}
			self.checked(allchecked);
		}
		// if all children checked, then we can get checked.
		self.areEventsEnabled = true;
	};

	self.load = function(model) {
		self.id(model.id);
		self.name(model.name);
		self.orgUnitType(model.orgUnitType);

		if (model.children && model.children.length > 0) {
			self.refresh(function() {
				self.areEventsEnabled = false;

				self.expanded(true);
				for (var i = 0; i < model.children.length; i++) {
					for (var j = 0; j < self.children().length; j++) {
						if (self.children()[j].id() === model.children[i].id) {
							self.children.remove(self.children()[j]);
							var child = new OrgUnitViewModel(model.children[i]);
							child.checked.subscribe(self.refreshChecked);
							self.children.push(child);
							break;
						}
					}
				}

				self.children.sort(function(left, right) {
					return left.name() === right.name() ? 0 : (left.name() < right.name() ? -1 : 1);
				})
				self.areEventsEnabled = true;
			});
		}

		self.areEventsEnabled = false;
		self.checked(model.checked);
		self.areEventsEnabled = true;
	};

	self.load(model);
}
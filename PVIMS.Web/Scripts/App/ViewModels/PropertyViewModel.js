var PropertyViewModel = function(name, systemType, itemSystemType, enumValues) {
	var self = this;
	self.name = ko.observable(name);
	self.systemType = ko.observable(systemType);
	self.itemSystemType = ko.observable(itemSystemType);
	self.enumValues = ko.observableArray();

	if (enumValues) {
		for (var i = 0; i < enumValues.length; i++) {
			self.enumValues.push(enumValues[i]);
		}
	}
	
}
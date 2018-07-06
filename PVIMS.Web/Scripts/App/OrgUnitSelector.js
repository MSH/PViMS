$.fn.orgUnitSelector = function() {
	$.each(this, function(index, value) {
		var outerElement = $(value).wrap("<div class=\"org-unit-tree tree\">" +
			+
			"</div>");
		$.attr(value, "data-bind", "value:toCommaSeperatedList");
		$.attr(value, "type", "hidden");

		$(value).after("<ul data-bind=\"template: { name: " + "'org-unit-tree-item'" + "}\"></ul>");
		//$(value).before("<input type='text' data-bind='textInput: filter'/>");
		if ($(value).val() !== "") {
			$.get("/OrgUnit/GetTree?selectedIds=" + $(value).val(), function(result) {
				var viewModel = new OrgUnitViewModel(result, true);
				console.log(outerElement);

				ko.applyBindings(viewModel, $(value).closest('div')[0]);
			});
		} else {
			$.get("/OrgUnit/GetChildren", function(data) {
				var viewModel = new OrgUnitViewModel(data[0]);
				ko.applyBindings(viewModel, $(value).closest('div')[0]);
			});
		};
	});
}
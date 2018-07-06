ko.subscribable.fn.subscribe2 = function (callback) {
	var previousValue;
	this.subscribe(function (_previousValue) {
		previousValue = _previousValue;
	}, undefined, 'beforeChange');
	this.subscribe(function (latestValue) {
		callback(latestValue, previousValue);
	});
};
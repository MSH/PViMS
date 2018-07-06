function unescape2(val) {
	var d = $("<div>");
	d.html(val);
	return d.text;
}
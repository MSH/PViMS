/// <reference path="../../../App/ViewModels/SpecificationViewModel.js" />
/// <reference path="../../../App/ViewModels/AndOrSpecificationViewModel.js" />
/// <reference path="../../../App/ViewModels/CollectionPropertyRuleSpecificationViewModel.js" />
/// <reference path="../../../App/ViewModels/ComparableSpecificationViewModel.js" />
/// <reference path="../../../App/ViewModels/PropertyRuleSpecificationViewModel.js" />
/// <reference path="../../../knockout-3.2.0.js" />
/// <reference path="../../../App/ViewModels/PropertyViewModel.js" />
/// <reference path="../../../App/knockout.extensions.js" />
/// <reference path="../../../jquery-1.10.2.js" />


QUnit.test('can load from JSON - Comparable', function () {
	var obj = {
		type: 'Equals',
		data: { value: 1 }
	};
	var comparable = new SpecificationViewModel({
		name: 'int',
		systemType: 'Int32'
	});
	comparable.load(obj);
	QUnit.assert.equal(comparable.type(), 'Equals');
	QUnit.assert.equal(comparable.child().op(), 'Equals');
	QUnit.assert.equal(comparable.child().value(), 1);
});

QUnit.test('can load from JSON - And', function () {
	var obj = {
		type: 'And',
		data: {
			left:
				{
					type: 'GreaterThan',
					data: { value: 1 }
				},
			right:
				{
					type: 'LessThan',
					data: { value: 3 }
				}
		}
	};
	var andSpec = new SpecificationViewModel({
		name: 'int',
		systemType: 'Int32'
	});
	andSpec.load(obj);
	QUnit.assert.equal(andSpec.type(), 'And');
	QUnit.assert.equal(andSpec.child().left().type(), 'GreaterThan');
	QUnit.assert.equal(andSpec.child().left().child().value(), 1);
	QUnit.assert.equal(andSpec.child().right().type(), 'LessThan');
	QUnit.assert.equal(andSpec.child().right().child().value(), 3);
});

QUnit.skip('can load from JSON - Property', function () {


	var obj = {
		type: 'Property',
		data: {
			name: 'Number',
			systemType: 'System.Int32',
			rule: {
				type: 'And',
				data: {
					left:
					{
						type: 'GreaterThan',
						data: { value: 1 }
					},
					right:
					{
						type: 'LessThan',
						data: { value: 3 }
					}
				}
			}
		}
	};
	var propertySpec = new SpecificationViewModel({
		name: 'Dummy',
		systemType: 'Dummy'
	});

	propertySpec.load(obj);

	QUnit.assert.equal(propertySpec.type(), 'Property');
	QUnit.assert.equal(propertySpec.child().rule().type(), 'And');
	QUnit.assert.ok(propertySpec.child().rule().child());
	QUnit.assert.ok(propertySpec.child().rule().child().left());
	QUnit.assert.ok(propertySpec.child().rule().child().right());
});
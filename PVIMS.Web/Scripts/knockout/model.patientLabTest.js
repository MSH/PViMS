define('model.patientLabTest',
    ['ko','moment'],
    function (ko, moment) {

        var patientLabTest = function () {

            var self = this;

            self.PatientLabTestIdentifier = ko.observable();
            self.PatientId = ko.observable();
            self.PatientLabTestId = ko.observable();                   
            self.TestName = ko.observable().extend({ required: true });
            self.TestDate = ko.observable().extend({ required: { params: true, message: 'The Test Date field is required.' } });
            self.TestResult = ko.observable();
            self.TestUnit = ko.observable();
            self.unitObject = ko.observable(); // for reference should not be saved
            self.LabValue = ko.observable().extend({ number: true });
            self.customAttributes = ko.observableArray();

            self.formattedTestDate = ko.computed({
                read: function () {
                    var myDate = self.TestDate();
                    return myDate ? moment(myDate).format("YYYY-MM-DD") : myDate;
                },

                write: function (value) {
                    self.TestDate(moment(value).format("YYYY-MM-DDTHH:mm:ss"));
                }
            });

            return self;
        };

        return patientLabTest;
    });
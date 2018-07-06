define('model.patientMedication',
    ['ko', 'moment'],
    function (ko, moment) {

        var patientMedication = function () {

            var self = this;

            self.PatientMedicationIdentifier = ko.observable();
            self.PatientMedicationId = ko.observable();
            self.MedicationId = ko.observable().extend({ required: { params: true, message: 'The Medication field is required.' } });
            self.PatientId = ko.observable();
            self.StartDate = ko.observable().extend({ required: { params: true, message: 'The Start Date field is required.' } });
            self.EndDate = ko.observable();
            self.Dose = ko.observable();
            self.DoseFrequency = ko.observable();
            self.DoseUnit = ko.observable();
            self.customAttributes = ko.observableArray();

            self.formattedStartDate = ko.computed({
                read: function () {
                    var myDate = self.StartDate();
                    return myDate ? moment(myDate).format("YYYY-MM-DD") : myDate;
                },

                write: function (value) {
                    self.StartDate(moment(value).format("YYYY-MM-DDTHH:mm:ss"));
                }
            });

            self.formattedEndDate = ko.computed({
                read: function () {
                    var myDate = self.EndDate();
                    return myDate ? moment(myDate).format("YYYY-MM-DD") : myDate;
                },

                write: function (value) {
                    self.EndDate(moment(value).format("YYYY-MM-DDTHH:mm:ss"));
                }
            });
        };

        return patientMedication;
    });
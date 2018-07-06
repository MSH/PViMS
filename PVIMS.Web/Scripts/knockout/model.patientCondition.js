define('model.patientCondition',
    ['ko', 'moment'],
    function (ko, moment) {

        var patientCondition = function () {

            var self = this;

            self.PatientConditionIdentifier = ko.observable();
            self.PatientConditionId = ko.observable();
            self.MedDraId = ko.observable();
            self.medDra = ko.observable(); // should not be saved
            //self.ConditionId = ko.observable().extend({ required: { params: true, message: 'The Condition field is required.' } });
            self.PatientId = ko.observable();
            self.StartDate = ko.observable().extend({ required: { params: true, message: 'The Start Date field is required.' } });
            self.EndDate = ko.observable();
            self.TreatmentStartDate = ko.observable();
            self.Comments = ko.observable();
            self.customAttributes = ko.observableArray();

            self.formattedStartDate = ko.computed({
                read: function () {
                    var myDate = self.StartDate();
                    return myDate ? moment(myDate).format("YYYY-MM-DD") : myDate;
                },

                write: function (value) {
                    self.StartDate(moment(value).format("YYYY-MM-DDTHH:mm:ss"));
                }
            }).extend({ required: { params: true, message: 'The Start Date field is required.' } });

            self.formattedEndDate = ko.computed({
                read: function () {
                    var myDate = self.EndDate();
                    return myDate ? moment(myDate).format("YYYY-MM-DD") : myDate;
                },

                write: function (value) {
                    self.EndDate(moment(value).format("YYYY-MM-DDTHH:mm:ss"));
                }
            });

            self.formattedTreatmentStartDate = ko.computed({
                read: function () {
                    var myDate = self.TreatmentStartDate();
                    return myDate ? moment(myDate).format("YYYY-MM-DD") : myDate;
                },

                write: function (value) {
                    self.TreatmentStartDate(moment(value).format("YYYY-MM-DDTHH:mm:ss"));
                }
            });
        };

        return patientCondition;
    });
define('model.patientClinicalEvent',
    ['ko', 'moment'],
    function (ko, moment) {

        var patientClinicalEvent = function () {

            var self = this;

            self.PatientClinicalEventIdentifier = ko.observable();
            self.PatientClinicalEventId = ko.observable();
            self.EncounterId = ko.observable();
            self.PatientId = ko.observable();
            self.Description = ko.observable().extend({ required: true })
                                              .extend({ maxLength: 500 });
            self.OnsetDate = ko.observable().extend({ required: true });
            self.ResolutionDate = ko.observable();
            self.ReportedDate = ko.observable();
            self.MedDraId = ko.observable();
            self.medDra = ko.observable(); // should not be saved
            self.customAttributes = ko.observableArray();

            self.formattedOnsetDate = ko.computed({
                read: function () {
                    var myDate = self.OnsetDate();
                    return myDate? moment(myDate).format("YYYY-MM-DD") : "";
                },

                write: function (value) {
                    if (moment(value).isValid())
                        self.OnsetDate(moment(value).format("YYYY-MM-DDTHH:mm:ss"));
                }
            });

            self.formattedResolutionDate = ko.computed({
                read: function () {
                    var myDate = self.ResolutionDate();
                    return myDate ? moment(myDate).format("YYYY-MM-DD") : myDate;
                },

                write: function (value) {
                    self.ResolutionDate(moment(value).format("YYYY-MM-DDTHH:mm:ss"));
                }
            });

            self.formattedReportedDate = ko.computed({
                read: function () {
                    var myDate = self.ReportedDate();
                    return myDate ? moment(myDate).format("YYYY-MM-DD") : myDate;
                },

                write: function (value) {
                    self.ReportedDate(moment(value).format("YYYY-MM-DDTHH:mm:ss"));
                }
            });

            self.EventDuration = ko.computed({
                read: function () {
                  return  moment(self.ResolutionDate()).diff(moment(self.OnsetDate()), 'days');
                }
            });

            return self;
        };

        return patientClinicalEvent;
    });
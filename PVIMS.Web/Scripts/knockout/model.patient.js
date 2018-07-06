define('model.patient',
    ['ko','moment'],
    function (ko, moment) {

        var patient = function () {
            var self = this;

            self.PatientId = ko.observable();
            self.PatientUniqueIdentifier = ko.observable();
            self.PatientFirstName = ko.observable().extend({ required: true });
            self.PatientMiddleName = ko.observable()
            self.PatientLastName = ko.observable().extend({ required: true });
            self.PatientDateOfBirth = ko.observable().extend({ required: true });
            self.FacilityId = ko.observable().extend({ required: true });
            self.PatientCreatedDate = ko.observable();
            self.PatientUpdatedDate = ko.observable();
            self.CreatedBy = ko.observable();
            self.UpdatedBy = ko.observable();
            self.Notes = ko.observable().extend({ notify: 'always' });
            self.customAttributes = ko.observableArray();

            self.Age = ko.computed({
                read: function () {
                    return moment().diff(moment(self.PatientDateOfBirth()), 'years');
                }
            });

            self.formattedDate = ko.computed({
                read: function () {
                    var myDate = self.PatientDateOfBirth();
                    return myDate ? moment(myDate).format("YYYY-MM-DD") : myDate;
                },

                write: function (value) {
                    self.PatientDateOfBirth(moment(value).format("YYYY-MM-DDTHH:mm:ss"));
                }
            });

            self.formattedCreatedBy = ko.computed({
                read: function () {
                    var myDate = self.PatientCreatedDate();
                    if (self.CreatedBy())
                        return 'Created by ' + self.CreatedBy() + ' on ' + (myDate ? moment(myDate).format("YYYY-MM-DD") : myDate);
                    else
                        return "";
                }
            });

            self.formattedUpdatedBy = ko.computed({
                read: function () {
                    var myDate = self.PatientUpdatedDate();
                    if (self.UpdatedBy())
                        return 'Updated by ' + self.UpdatedBy() + ' on ' + (myDate ? moment(myDate).format("YYYY-MM-DD") : myDate);
                    else
                        return "";
                }
            });

            self.dirtyFlag = new ko.DirtyFlag([
                   self.PatientFirstName,
                   self.PatientLastName,
                   self.PatientDateOfBirth]);

            return self;
        };

        return patient;

    });
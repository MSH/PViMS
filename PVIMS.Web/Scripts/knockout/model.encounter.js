define('model.encounter',
    ['ko', 'moment'],
    function (ko, moment) {

        var encounter = function () {
            var self = this;

            self.EncounterId = ko.observable();
            self.PatientId = ko.observable();
            self.Patient = ko.observable(); // should not be saved
            self.EncounterIdentifier = ko.observable();
            self.EncounterType = ko.observable().extend({ required: true });
            self.EncounterDate = ko.observable().extend({ required: true });
            self.EncounterPriority = ko.observable().extend({ required: true });
            self.EncounterCreatedDate = ko.observable();
            self.EncounterUpdatedDate = ko.observable();
            self.CreatedBy = ko.observable();
            self.UpdatedBy = ko.observable();
            self.Notes = ko.observable();
            self.customAttributes = ko.observableArray();

            self.formattedEncounterDate = ko.computed({
                read: function () {
                    var myDate = self.EncounterDate();
                    return myDate ? moment(myDate).format("YYYY-MM-DD") : myDate;
                },

                write: function (value) {
                    self.EncounterDate(moment(value).format("YYYY-MM-DDTHH:mm:ss"));
                }
            });

            self.formattedCreatedBy = ko.computed({
                read: function () {
                    var myDate = self.EncounterCreatedDate();
                    if (self.CreatedBy())
                        return 'Created by ' + self.CreatedBy() + ' on ' + (myDate ? moment(myDate).format("YYYY-MM-DD") : myDate);
                    else
                        return "";
                }
            });

            self.formattedUpdatedBy = ko.computed({
                read: function () {
                    var myDate = self.EncounterUpdatedDate();
                    if (self.UpdatedBy())
                        return 'Updated by ' + self.UpdatedBy() + ' on ' + (myDate ? moment(myDate).format("YYYY-MM-DD") : myDate);
                    else
                        return "";
                }
            });

            self.dirtyFlag = new ko.DirtyFlag([
                   self.EncounterType,
                   self.EncounterDate,
                   self.EncounterPriority]);

            return self;
        };

        return encounter;

    });
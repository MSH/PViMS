define('vm.addEncounter',
    ['jquery', 'ko', 'moment', 'config', 'dataservice', 'model', 'model.mapper', 'uuid', 'utils'],
    function ($, ko, moment, config, dataservice, model, mapper, uuid, utils) {

        var
			   // Properties

			   patient = ko.observable(new model.Patient()),
			   encounter = ko.observable(new model.Encounter()),
			   encounterTypes = ko.observableArray(),
			   priorities = ko.observableArray(),
               selectedEncounterType = ko.observable(),
               selectedPriority = ko.observable(),

               heading = ko.observable("Add Encounter"),

               error = ko.observable(),
               saved = ko.observable(false), // use to show sucessfully message on view
               saveInvalid = ko.observable(false), // show invalid messages on view

			   validationErrors = ko.computed(function () {
			       var valArray = encounter() ? ko.validation.group(encounter())() : [];
			       return valArray;
			   }),

    			validationErrors = ko.computed(function () {
    			    var valArray = encounter() ? ko.validation.group(encounter())() : [];
    			    return valArray;
    			})

        canEdit = ko.computed(function () {
            return true;
        }),

        isDirty = ko.computed(function () {
            return canEdit() ? encounter().dirtyFlag().isDirty() : false;
        }),

        isValid = ko.computed(function () {
            return canEdit() ? validationErrors().length === 0 : true;
        }),


        // Methods

       activate = function () {
           var data = {};
           var id = utils.getUrlVar('id');

           dataservice.dataServicePatient.localService.getById('PatientId', parseInt(id))
              .done(function (pat) {
                  patient = mapper.patient.fromDto(pat);
                  encounter().PatientId(id);
                  heading("Add Encounter (" + pat.PatientFirstName + " " + pat.PatientLastName + ")");

                  dataservice.encounterService.localService.getEncounterTypes()
                  .then(function (items) {
                      encounterTypes(items);

                      //selectedEncounterType(ko.utils.arrayFirst(encounterTypes(), function (item) { return true }) || {});

                      return dataservice.encounterService.localService.getPriorities();
                  })
                  .then(function (items) {
                      priorities(items);

                     // selectedPriority(ko.utils.arrayFirst(priorities(), function (item) { return true }) || {});
                  });
              });
       },

        save = function () {

            if (isDirty() && isValid()) {

                var plainEncounter = mapper.encounter.toDto(encounter);

                plainEncounter.synchStatus = "synchronization required";

                if (encounter().EncounterIdentifier()) {
                    plainEncounter.PatientUpdatedDate = moment(new Date()).format("YYYY-MM-DDTHH:mm:ss");
                    plainEncounter.UpdatedBy = localStorage.getItem("User") || "Offline";
                    dataservice.encounterService.localService.addOrUpdate(plainEncounter)
                    .done(function () {
                        encounter(mapper.encounter.fromDto(plainEncounter));
                        saved(true);
                        saveInvalid(false);
                    })
                     .always(function () { /*isExecuting(false); */});
                }
                else // new encounter
                {
                    var nextId = 1;

                    dataservice.encounterService.localService.getSequenceId()
                        .done(function (id) {
                            nextId = id;

                            plainEncounter.EncounterIdentifier = uuid.v4();
                            plainEncounter.EncounterId = nextId;
                            plainEncounter.EncounterCreatedDate = moment(new Date()).format("YYYY-MM-DDTHH:mm:ss");
                            plainEncounter.synchStatus = "synchronization required";
                            plainEncounter.CreatedBy = localStorage.getItem("User") || "Offline"; 
                            dataservice.encounterService.localService.addOrUpdate(plainEncounter)
                                .done(function () {
                                    encounter(mapper.encounter.fromDto(plainEncounter));
                                    //formMode(mode.viewMode);
                                    saved(true);
                                    saveInvalid(false);
                                })
                                .always(function () {
                                    /*isExecuting(false);*/
                                    window.location.href = "/Patient/PatientSearch"
                                });
                        });
                }


            }
            else if (!isValid()) {
                //saveInvalid(true);
                ko.validation.group(encounter()).showAllMessages();
            }


        };

        //activate();

        // subscribe for heading change
        heading.subscribe(function (newValue) {
            var element = $('#breadcrumb-heading');
            element.text(newValue);

            element = $('#title-heading');
            element.text(newValue);
        });

        return {
            activate: activate,
            encounter: encounter,
            encounterTypes: encounterTypes,
            priorities: priorities,
            selectedEncounterType: selectedEncounterType,
            selectedPriority: selectedPriority,
            save: save,
            heading: heading
        }

    });
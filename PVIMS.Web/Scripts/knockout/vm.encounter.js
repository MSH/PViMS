/// <reference path="../_references.js" />
define('vm.encounter',
    ['jquery', 'ko', 'model', 'dataservice', 'model.mapper','utils'],
    function ($, ko, model, dataservice, mapper, utils) {

        var
            // Properties

            mode = {
                viewMode: 'View',
                editMode: 'Edit'
            },

            patient = ko.observable(new model.Patient()),

            encounter = ko.observable(new model.Encounter()),
            encounters = ko.observableArray(),

            error = ko.observable(),

            originalEncounter = {},
            patientLabTests = ko.observableArray(),
            patientClinicalEvents = ko.observableArray(),
            patientMedications = ko.observableArray(),
            patientConditions = ko.observableArray(),

            medications = ko.observableArray(),
            conditions = ko.observableArray(),

            formMode = ko.observable(mode.viewMode),

            formNotes = ko.observable(),
            heading = ko.observable("Encounter"),

            searchParamPatientId = ko.observable(),
            searchParamFirstname = ko.observable(),
            searchParamSurname = ko.observable(),
            searchParamEncounterDate = ko.observable(),

            // Methods

        activate = function () {
        var data = {};
        var id = utils.getUrlVar('id');
        var encounterId = utils.getUrlVar('encounterId')

        dataservice.dataServicePatient.localService.getById('PatientId', parseInt(id))
            .done(function (pat) {
                var itemPatient = mapper.patient.fromDto(pat);
                heading("Encounter (" + pat.PatientFirstName + " " + pat.PatientLastName + ")");

                dataservice.encounterService.localService.getById('EncounterId', parseInt(encounterId))
                .then(function (item) {
                    encounter(mapper.encounter.fromDto(item));
                    originalEncounter = item;
                    return  dataservice.patientLabTest.localService.getAllById('PatientId', parseInt(id))
                })                       
                    .then(function (items) {                               

                        patientLabTests(ko.utils.arrayMap(items, function (item) {
                            var mapObj = mapper.patientLabTest.fromDto(item);
                            return mapObj;
                        }));

                        return dataservice.patientClinicalEvent.localService.getAllById('PatientId', parseInt(id));
                    })
                    .then(function (items) {
                        patientClinicalEvents(ko.utils.arrayMap(items, function (item) {
                            var obj = mapper.patientClinicalEvent.fromDto(item);
                            return obj;
                        }));

                        return dataservice.patientMedication.localService.getAllById('PatientId', parseInt(id));
                    })
                    .then(function (items) {
                        patientMedications(ko.utils.arrayMap(items, function (item) {
                            var obj = mapper.patientMedication.fromDto(item);
                            return obj;
                        }));

                        return dataservice.patientCondition.localService.getAllById('PatientId', parseInt(id));
                    })
                    .then(function (items) {
                        patientConditions(ko.utils.arrayMap(items, function (item) {
                            var obj = mapper.patientCondition.fromDto(item);
                            return obj;
                        }));

                        return dataservice.patientMedication.localService.getMedication();
                    })
                    .then(function (items) {
                        medications(items);

                        return dataservice.patientCondition.localService.getConditions();
                    })
                    .then(function (data) {
                        conditions(data);
                    })
                    .done(function () {
                        patient(itemPatient);
                    });
            });                               
    },

        getMedicationName = function (id) {
            if (!id)
                return "";
            var selected = ko.utils.arrayFirst(medications(), function (item) {
                return item.MedicationId == id;
            });

            if (selected)
                return selected.DrugName;
            else
                return "";
        },

        getConditionName = function (id) {
            if (!id)
                return "";
            var selected = ko.utils.arrayFirst(conditions(), function (item) {
                return item.ConditionId == id;
            });

            if (selected)
                return selected.Description;
            else
                return "";
        },

        removePatientLabTest = function (identifier) {
            dataservice.patientLabTest.localService.remove(identifier)
                .done(function () {
                    patientLabTests.remove(function (item) {
                        return item.PatientLabTestIdentifier() === identifier;
                    });
                });
        },

        removePatientClinicalEvent = function (identifier) {
            dataservice.patientClinicalEvent.localService.remove(identifier)
                    .done(function () {
                        patientClinicalEvents.remove(function (item) {
                            return item.PatientClinicalEventIdentifier() === identifier;
                        });
                    });
        },

        removePatientMedication = function (identifier) {
            dataservice.patientMedication.localService.remove(identifier)
                    .done(function () {
                        patientMedications.remove(function (item) {
                            return item.PatientMedicationIdentifier() === identifier;
                        });
                    });
        },

        removePatientCondition = function (identifier) {
            dataservice.patientCondition.localService.remove(identifier)
                    .done(function () {
                        patientConditions.remove(function (item) {
                            return item.PatientConditionIdentifier() === identifier;
                        });
                    });
        },

        createURL = function (actionName, controllerName, parameters) {
            console.log('here 2');
            var url = "",
                countParameters = 0;
            if (actionName && controllerName)
            {
                if (parameters)
                {
                    url = "/" + controllerName + "/" + actionName;

                    for (var key in parameters) {
                        if (parameters.hasOwnProperty(key)) {

                            if(countParameters > 0)
                                url = url + "&" + key + "=" + parameters[key];
                            else
                                url = url + "#" + key + "=" + parameters[key];

                            countParameters++;
                        }
                    }
                }                       
                else
                    url = "/" + controllerName + "/" + actionName;
            }
            return url;
        };

        cancel = function () {
            encounter(mapper.encounter.fromDto(originalEncounter));
            formMode(mode.viewMode);
        };

        edit = function () {
            formMode(mode.editMode);
        };

        gotoPatient = function () {
            window.location.href = "/Patient/PatientView#id=" + patient().PatientId();
        };

        save = function () {
            if (encounter().EncounterIdentifier()) {
                var plainEncounter = mapper.encounter.toDto(encounter);
                plainEncounter.synchStatus = "synchronization required";
                plainEncounter.EncounterUpdatedDate = moment(new Date()).format("YYYY-MM-DDTHH:mm:ss");
                plainEncounter.UpdatedBy = localStorage.getItem("User") || "Offline";
                dataservice.encounterService.localService.addOrUpdate(plainEncounter)
                .done(function () {
                    encounter(mapper.encounter.fromDto(plainEncounter));
                    originalEncounter = plainEncounter;
                })
                 .always(function () { formMode(mode.viewMode); });
            }
        };

        searchEncounters = function (searchform) {
            //console.log('here');
            var searchesCriteria = [],
                searchObject = {};

            // If primary key is defined, lookup using only the primary key
            if (searchParamPatientId()) {
                searchObject = { PatientIdentifier: searchParamPatientId() };
                searchesCriteria.push(searchObject);

                dataservice.encounterService.localService.searchByIds(searchesCriteria, true)
                    .done(function (data) {
                        encounters(data);
                    });
            }
            else
            {
                if (searchParamFirstname()) {
                    searchObject = { PatientFirstName: searchParamFirstname() };
                    searchesCriteria.push(searchObject);
                }

                if (searchParamSurname()) {
                    searchObject = { PatientLastName: searchParamSurname() };
                    searchesCriteria.push(searchObject);
                }

                if (searchParamEncounterDate()) {
                    searchObject = { EncounterDate: searchParamEncounterDate() };
                    searchesCriteria.push(searchObject);
                }

                dataservice.encounterService.localService.searchByIds(searchesCriteria, false)
                    .done(function (data) {
                        encounters(data);
                    });
            }
        };

        getEncounterTypeById = function (EncounterType) {
            var desc = "UNKNOWN";

            dataservice.encounterService.localService.getEncounterTypeById(EncounterType)
                .done(function (data) {
                    desc = et.Description;
                });

            return desc;
        };

        // execute methods
        // activate();

        // SUBSCRIBE EVENTS FOR TAB NOTES
        formNotes.subscribe(function (newValue) {
            encounter().Notes(newValue);
        });

        encounter.subscribe(function (newValue) {
            formNotes(newValue.Notes());
        });

        // END SUBSCRIBE EVENTS FOR TAB NOTES

        // Extend kockout binding handlers to handle Thirdy Party Libray - CKEDITOR
        ko.bindingHandlers.ckeditor = {
            init: function (element, valueAccessor) {

                var modelValue = valueAccessor();
                var value = ko.utils.unwrapObservable(valueAccessor());
                var element$ = $(element);

                // Set initial value and create the CKEditor
                $(element).html(value);
                $(element).ckeditor();
                var editor = $(element).ckeditorGet();

                // bind to change events and link it to the observable
                editor.on('blur', function (e) {
                    var self = this; debugger;
                    if (ko.isWriteableObservable(self)) {
                        self($(e.listenerData).val());
                    }
                }, modelValue, element);


                /* Handle disposal if KO removes an editor 
                 * through template binding */
                ko.utils.domNodeDisposal.addDisposeCallback(element,
                    function () {
                        if (editor) {
                            CKEDITOR.remove(editor);
                        };
                    });
            },

            /* Hook and handle the binding updating so we write 
             * back to the observable */
            update: function (element, valueAccessor, allBindingsAccessor) {
                var element$ = $(element);
                var newValue = ko.utils.unwrapObservable(valueAccessor());
                if (element$.ckeditorGet().getData() != newValue) {
                    element$.ckeditorGet().setData(newValue);
                }

                var allBindings = allBindingsAccessor(),
                enable = (allBindings.enable !== undefined) ? ko.utils.unwrapObservable(allBindings.enable) : true;

                if (enable) {
                    element$.ckeditorGet().setReadOnly(false);
                }
                else {
                    element$.ckeditorGet().setReadOnly(true);
                }
            }
        };

        // subscribe for heading change
        heading.subscribe(function (newValue) {
            var element = $('#breadcrumb-heading');
            element.text(newValue);

            element = $('#title-heading');
            element.text(newValue);
        });

        return {
            activate: activate,
            patient: patient,
            encounter: encounter,
            encounters: encounters,
            error: error,
            formNotes: formNotes,
            formMode: formMode,
            patientLabTests: patientLabTests,
            patientClinicalEvents: patientClinicalEvents,
            patientMedications: patientMedications,
            patientConditions: patientConditions,
            getMedicationName: getMedicationName,
            getConditionName: getConditionName,
            removePatientLabTest: removePatientLabTest,
            removePatientClinicalEvent: removePatientClinicalEvent,
            removePatientMedication: removePatientMedication,
            removePatientCondition: removePatientCondition,
            gotoPatient: gotoPatient,
            save: save,
            createURL: createURL,
            createURLParams: utils.createURL,
            searchEncounters: searchEncounters,
            getEncounterTypeById: getEncounterTypeById,
            searchParamPatientId: searchParamPatientId,
            searchParamFirstname: searchParamFirstname,
            searchParamSurname: searchParamSurname,
            searchParamEncounterDate: searchParamEncounterDate
        };


});
define('vm.patientClinicalEvent',
    ['jquery', 'ko', 'model', 'model.mapper', 'dataservice', 'utils', 'Dexie'],
    function ($, ko, model, mapper, dataservice, utils, Dexie) {

        var
            patientClinicalEvent = ko.observable(new model.PatientClinicalEvent()),

            selectionData = ko.observableArray(),

            showErrors = ko.observable(false),

            widgetTitle = ko.observable(),

            encounterId = ko.observable(),

            termTypes = ko.observableArray([
                { Id: "SOC", Description: "System Organ Class" },
                { Id: "HLGT", Description: "High Level Group Term" },
                { Id: "HLT", Description: "High Level Term" },
                { Id: "PT", Description: "Preferred Term" },
                { Id: "LLT", Description: "Lowest Level Term" }
            ]),

            termType = ko.observable(termTypes()[4].Id),

            termResults = ko.observableArray([{ Id: "", Description: "" }]),

            termFind = ko.observable(),

            validationErrors = ko.computed(function () {
                var valArray = patientClinicalEvent() ? ko.validation.group(patientClinicalEvent())() : [];
                return valArray;
            }),

             isValid = ko.computed(function () {
                 return validationErrors().length === 0;
             }),

             showMedDraForm = ko.observable(true),

             showMedDraLabel = ko.observable(false),

             ToggleMedDra = function () {
                 showMedDraForm(true);
             },

             isExecuting = ko.observable(false),

            activate = function () {

                var id = utils.getUrlVar('patientClinicalEventId'),
                    patientId = utils.getUrlVar('patientId');

                encounterId(utils.getUrlVar('encounterId'));

                dataservice.dataServicePatient.localService.getById('PatientId', patientId)
                  .done(function (pat) {
                      widgetTitle("Patient Adverse Event Information (" + pat.PatientFirstName + " " + pat.PatientLastName + ")");
                  });

                if (!id) {

                    var patientId = utils.getUrlVar('patientId');
                    patientClinicalEvent().PatientId(patientId);                    

                    dataservice.customAttribute.localService.getSelectionData() // load custom attributes data
                           .done(function (data) {
                               selectionData(data);
                           });

                    dataservice.patientClinicalEvent.localService.getCustomAttributes({}, 'PatientClinicalEvent')
                        .then(function (customs) { // Dexie promise is returned instead of jquery promise
                            patientClinicalEvent().customAttributes(ko.utils.arrayMap(customs, function (item) {
                                var currentValue = ko.observable(item['currentValue']);
                                currentValue = utils.setCustomValidationRules(currentValue, item);
                                item['currentValue'] = currentValue;
                                return ko.observable(item);
                            }));
                        });
                }
                else {
                    var clinicalEvent;
                    dataservice.patientClinicalEvent.localService.getById('PatientClinicalEventId', parseInt(id))
                    .then(function (item) {
                        clinicalEvent = item;
                        return dataservice.customAttribute.localService.getSelectionData() // load custom attributes data
                             .done(function (data) {
                                 selectionData(data);
                             });
                    })
                    .done(function () {
                        var Obj = mapper.patientClinicalEvent.fromDto(clinicalEvent);
                        patientClinicalEvent(Obj);
                        if(clinicalEvent.medDra)
                        {
                            showMedDraLabel(true);
                            showMedDraForm(false);
                        }
                    });
                }

            },

            filterAttributes = function (attributeName) {
                return selectionData().filter(function (selected) {
                    return selected.AttributeKey === attributeName;
                });
            },

            search = function () {
                dataservice.patientClinicalEvent.localService.getTerminologyMedDra(termType(), termFind())
                            .done(function (terms) {
                                termResults(terms);
                            });
            },

            save = function () {
                if (!isExecuting() && isValid()) {

                    isExecuting(true);

                    var plainObject = mapper.patientClinicalEvent.toDto(patientClinicalEvent);

                    plainObject.synchStatus = "synchronization required";

                    if (!patientClinicalEvent().PatientClinicalEventIdentifier()) { // new 
                        dataservice.patientClinicalEvent.localService.getSequenceId()
                            .done(function (id) {
                                plainObject.PatientClinicalEventIdentifier = uuid.v4();
                                plainObject.PatientClinicalEventId = id;
                                plainObject.PatientId = parseInt(utils.getUrlVar('patientId'));
                                plainObject.EncounterId = parseInt(utils.getUrlVar('encounterId'));

                                dataservice.patientClinicalEvent.localService.addOrUpdate(plainObject)
                                    .done(function () {
                                        window.location.href = "/Encounter/ViewEncounterOffline#encounterId=" + plainObject.EncounterId + "&id=" + plainObject.PatientId;
                                    })
                                    .always(function () {
                                        isExecuting(false);                                        
                                    });
                            });
                    }
                    else {
                        dataservice.patientClinicalEvent.localService.addOrUpdate(plainObject)
                            .done(function () {
                                window.location.href = "/Encounter/ViewEncounterOffline#encounterId=" + plainObject.EncounterId + "&id=" + plainObject.PatientId;
                            })
                            .always(function () {
                                isExecuting(false);                                
                            });
                    }

                }
                else {
                    if (!isValid()) {
                        showErrors(true);
                        ko.validation.group(patientClinicalEvent()).showAllMessages();
                    }

                }

            };

        activate();

        return {
            patientClinicalEvent: patientClinicalEvent,
            widgetTitle: widgetTitle,
            isValid: isValid,
            filterAttributes: filterAttributes,
            showErrors: showErrors,
            createURL: utils.createURL,
            encounterId: encounterId,
            termFind: termFind,
            termType: termType,
            termTypes: termTypes,
            termResults: termResults,
            search: search,
            showMedDraForm: showMedDraForm,
            showMedDraLabel: showMedDraLabel,
            ToggleMedDra: ToggleMedDra,
            save: save
        };
    });
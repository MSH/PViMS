define('vm.patientCondition',
    ['jquery', 'ko', 'model', 'model.mapper', 'dataservice', 'utils', 'Dexie'],
    function ($, ko, model, mapper, dataservice, utils, Dexie) {

        var
            patientCondition = ko.observable(new model.PatientCondition()),

            conditions = ko.observableArray(),

            selectionData = ko.observableArray(),

            encounterId = ko.observable(),

            widgetTitle = ko.observable(),

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

            showErrors = ko.observable(false),

            validationErrors = ko.computed(function () {
                var valArray = patientCondition() ? ko.validation.group(patientCondition())() : [];
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

                var id = utils.getUrlVar('patientConditionId'),
                    patientId = utils.getUrlVar('patientId');

                encounterId(utils.getUrlVar('encounterId'));

                dataservice.dataServicePatient.localService.getById('PatientId', patientId)
                  .done(function (pat) {                      
                      widgetTitle("Patient Condition Information (" + pat.PatientFirstName + " " + pat.PatientLastName +")");                     
                  });

                if (!id) {

                    var patientId = utils.getUrlVar('patientId');
                    patientCondition().PatientId(patientId);

                    dataservice.patientCondition.localService.getConditions() // load conditions
                            .done(function (items) {
                                conditions(items);
                            });

                    dataservice.customAttribute.localService.getSelectionData() // load custom attributes data
                           .done(function (data) {
                               selectionData(data);
                           });

                    dataservice.patientCondition.localService.getCustomAttributes({}, 'PatientCondition')
                        .then(function (customs) { // Dexie promise is returned instead of jquery promise
                            patientCondition().customAttributes(ko.utils.arrayMap(customs, function (item) {
                                var currentValue = ko.observable(item['currentValue']);
                                currentValue = utils.setCustomValidationRules(currentValue, item);
                                item['currentValue'] = currentValue;
                                return ko.observable(item);
                            }));
                        });
                }
                else {
                    var patientCond;
                    dataservice.patientCondition.localService.getById('PatientConditionId', parseInt(id))
                    .then(function (item) {
                        patientCond = item;
                        return dataservice.patientCondition.localService.getConditions() // load conditions
                            .done(function (items) {
                                conditions(items);
                            });
                    }).then(function () {
                        return dataservice.customAttribute.localService.getSelectionData() // load custom attributes data
                             .done(function (data) {
                                 selectionData(data);
                             });
                    })
                    .done(function () {
                        var patientObj = mapper.patientCondition.fromDto(patientCond);
                        patientCondition(patientObj);
                        if (patientCond.medDra) {
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
                 dataservice.patientCondition.localService.getTerminologyMedDra(termType(), termFind())
                             .done(function (terms) {
                                 termResults(terms);
                             });
             },

            save = function () {
                if (!isExecuting() && isValid()) {

                    isExecuting(true);

                    var plainObject = mapper.patientCondition.toDto(patientCondition);

                    plainObject.synchStatus = "synchronization required";

                    if (!patientCondition().PatientConditionIdentifier()) { // new 
                        dataservice.patientCondition.localService.getSequenceId()
                            .done(function (id) {
                                plainObject.PatientConditionIdentifier = uuid.v4();
                                plainObject.PatientConditionId = id;
                                plainObject.PatientId = parseInt(utils.getUrlVar('patientId'));

                                dataservice.patientCondition.localService.addOrUpdate(plainObject)
                                    .done(function () {

                                    })
                                    .always(function () {
                                        isExecuting(false);
                                        window.location.href = "/Encounter/ViewEncounterOffline#encounterId=" + parseInt(utils.getUrlVar('encounterId')) + "&id=" + plainObject.PatientId;
                                    });
                            });
                    }
                    else { // updating
                        dataservice.patientCondition.localService.addOrUpdate(plainObject)
                            .done(function () {

                            })
                            .always(function () {
                                isExecuting(false);
                                window.location.href = "/Encounter/ViewEncounterOffline#encounterId=" + parseInt(utils.getUrlVar('encounterId')) + "&id=" + plainObject.PatientId;
                            });
                    }

                }
                else {
                    if (!isValid()) {
                        showErrors(true);
                        ko.validation.group(patientCondition()).showAllMessages();
                    }

                }

            };

        //activate();

        return {
            activate: activate,
            patientCondition: patientCondition,
            widgetTitle: widgetTitle,
            isValid: isValid,
            conditions: conditions,
            filterAttributes: filterAttributes,
            showErrors: showErrors,
            createURL: utils.createURL,
            encounterId: encounterId,
            termFind: termFind,
            termType: termType,
            termTypes: termTypes,
            termResults: termResults,
            showMedDraForm: showMedDraForm,
            showMedDraLabel: showMedDraLabel,
            ToggleMedDra: ToggleMedDra,
            search: search,
            save: save
        };
    });
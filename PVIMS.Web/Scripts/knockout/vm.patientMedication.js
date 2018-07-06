define('vm.patientMedication',
    ['jquery', 'ko', 'model', 'model.mapper', 'dataservice', 'utils', 'Dexie'],
    function ($, ko, model, mapper, dataservice, utils, Dexie) {

        var
            patientMedication = ko.observable(new model.PatientMedication()),

            medications = ko.observableArray(),

            selectionData = ko.observableArray(),

            encounterId = ko.observable(),

            widgetTitle = ko.observable(),

            showErrors = ko.observable(false),

            doseUnits = ko.observableArray([
                { Id: "", Description: "" },
                { Id: "Bq", Description: "becquerel" },
                { Id: "Ci", Description: "curie" },
                { Id: "{DF}", Description: "Dosage form" },
                { Id: "[drp]", Description: "drop" },
                { Id: "GBq", Description: "gigabecquerel" },
                { Id: "g", Description: "gram" },
                { Id: "[iU]", Description: "International unit" },
                { Id: "[iU]/kg", Description: "International unit/kilogram" },
                { Id: "kBq", Description: "killobecquerel" },
                { Id: "kg", Description: "kilogram" },
                { Id: "k[iU]", Description: "kilo-international unit" },
                { Id: "L", Description: "liter" },
                { Id: "MBq", Description: "megabecquerel" },
                { Id: "M[iU]", Description: "mega-international unit" },
                { Id: "uCi", Description: "microcurie" },
                { Id: "ug", Description: "microgram" },
                { Id: "ug/kg", Description: "microgram/kilogram" },
                { Id: "uL", Description: "microliter" },
                { Id: "mCi", Description: "millicurie" },
                { Id: "meq", Description: "milliequivalent" },
                { Id: "mg", Description: "milligram" },
                { Id: "mg/kg", Description: "milligram/kilogram" },
                { Id: "mg/m2", Description: "milligram/sq.meter" },
                { Id: "ug/m2", Description: "microgram/sq.meter" },
                { Id: "mL", Description: "milliliter" },
                { Id: "mmol", Description: "millimole" },
                { Id: "mol", Description: "mole" },
                { Id: "nCi", Description: "nanocurie" },
                { Id: "ng", Description: "nanogram" },
                { Id: "%", Description: "percent" },
                { Id: "pg", Description: "picogram" }
            ]),

            validationErrors = ko.computed(function () {
                var valArray = patientMedication() ? ko.validation.group(patientMedication())() : [];
                return valArray;
            }),

             isValid = ko.computed(function () {
                 return validationErrors().length === 0;
             }),

             isExecuting = ko.observable(false),

            activate = function () {

                var id = utils.getUrlVar('patientMedicationId'),
                    patientId = utils.getUrlVar('patientId');

                encounterId(utils.getUrlVar('encounterId'));

                dataservice.dataServicePatient.localService.getById('PatientId', patientId)
                  .done(function (pat) {
                      widgetTitle("Patient Medication Information (" + pat.PatientFirstName + " " + pat.PatientLastName + ")");
                  });

                if (!id) {

                    var patientId = utils.getUrlVar('patientId');
                    patientMedication().PatientId(patientId);

                    dataservice.patientMedication.localService.getMedication() // load lab test type
                            .done(function (items) {
                                medications(items);
                            });

                    dataservice.customAttribute.localService.getSelectionData() // load custom attributes data
                           .done(function (data) {
                               selectionData(data);
                           });

                    dataservice.patientMedication.localService.getCustomAttributes({}, 'PatientMedication')
                        .then(function (customs) { // Dexie promise is returned instead of jquery promise
                            patientMedication().customAttributes(ko.utils.arrayMap(customs, function (item) {
                                var currentValue = ko.observable(item['currentValue']);
                                currentValue = utils.setCustomValidationRules(currentValue, item);
                                item['currentValue'] = currentValue;
                                return ko.observable(item);
                            }));
                        });
                }
                else {
                    var patMed;
                    dataservice.patientMedication.localService.getById('PatientMedicationId', parseInt(id))
                    .then(function (item) {
                        patMed = item;
                        return dataservice.patientMedication.localService.getMedication() // load lab test type
                            .done(function (items) {
                                medications(items);
                            });
                    }).then(function () {
                        return dataservice.customAttribute.localService.getSelectionData() // load custom attributes data
                             .done(function (data) {
                                 selectionData(data);
                             });
                    })
                    .done(function () {
                        var medicationObj = mapper.patientMedication.fromDto(patMed);
                        patientMedication(medicationObj);
                    });
                }

            },

            filterAttributes = function (attributeName) {
                return selectionData().filter(function (selected) {
                    return selected.AttributeKey === attributeName;
                });
            },

            save = function () {
                if (!isExecuting() && isValid()) {

                    isExecuting(true);

                    var plainObject = mapper.patientMedication.toDto(patientMedication);

                    plainObject.synchStatus = "synchronization required";

                    if (!patientMedication().PatientMedicationIdentifier()) { // new 
                        dataservice.patientMedication.localService.getSequenceId()
                            .done(function (id) {
                                plainObject.PatientMedicationIdentifier = uuid.v4();
                                plainObject.PatientMedicationId = id;
                                plainObject.PatientId = parseInt(utils.getUrlVar('patientId'));

                                dataservice.patientMedication.localService.addOrUpdate(plainObject)
                                    .done(function () {

                                    })
                                    .always(function () {
                                        isExecuting(false);
                                        window.location.href = "/Encounter/ViewEncounterOffline#encounterId=" + encounterId() + "&id=" + plainObject.PatientId;
                                    });
                            });
                    }
                    else { // updating
                        dataservice.patientMedication.localService.addOrUpdate(plainObject)
                            .done(function () {
                                
                            })
                            .always(function () {
                                isExecuting(false);
                                window.location.href = "/Encounter/ViewEncounterOffline#encounterId=" + encounterId() + "&id=" + plainObject.PatientId;
                            });
                    }

                }
                else {
                    if (!isValid()) {
                        showErrors(true);
                        ko.validation.group(patientMedication()).showAllMessages();
                    }

                }

            };

        activate();

        return {
            patientMedication: patientMedication,
            widgetTitle: widgetTitle,
            isValid: isValid,
            medications: medications,
            doseUnits: doseUnits,
            filterAttributes: filterAttributes,
            showErrors: showErrors,
            createURL: utils.createURL,
            encounterId: encounterId,
            save: save
        };
    });
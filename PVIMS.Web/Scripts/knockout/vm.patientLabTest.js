define('vm.patientLabTest',
    ['jquery', 'ko','model', 'model.mapper','dataservice', 'utils','Dexie'],
    function ($, ko, model, mapper, dataservice, utils,Dexie) {

        var
            patientLabTest = ko.observable(new model.PatientLabTest()),

            testLabTypes = ko.observableArray(),

            labTestUnits = ko.observableArray(),

            selectionData = ko.observableArray(),

            widgetTitle = ko.observable(),

            showErrors = ko.observable(false),

            encounterId = ko.observable(),

            TestResults = ko.observableArray([
                { Id: "", Description: "" },
                { Id: "Positive", Description: "Positive" },
                { Id: "Negative", Description: "Negative" },
                { Id: "Borderline", Description: "Borderline" },
                { Id: "Inconclusive", Description: "Inconclusive" },
            ]),

            validationErrors = ko.computed(function () {
                var valArray = patientLabTest() ? ko.validation.group(patientLabTest())() : [];
                return valArray;
            }),

             isValid = ko.computed(function () {
                 return validationErrors().length === 0;
             }),

             isExecuting = ko.observable(false),

            activate = function () {

                var id = utils.getUrlVar('patientLabTestId'),
                    patientId = utils.getUrlVar('patientId');

                encounterId(utils.getUrlVar('encounterId'));

                dataservice.dataServicePatient.localService.getById('PatientId', patientId)
                  .done(function (pat) {
                      widgetTitle("Patient Clinical Evaluations Information (" + pat.PatientFirstName + " " + pat.PatientLastName + ")");
                  });

                if (!id) {

                    var patientId = utils.getUrlVar('patientId');
                    patientLabTest().PatientId(patientId);

                    dataservice.patientLabTest.localService.getLabTestType() // load lab test type
                            .done(function (items) {
                                testLabTypes(ko.utils.arrayMap(items, function (item) {
                                    return item.Text;
                                }));
                            });

                    dataservice.patientLabTest.localService.getLabTestUnit() // load lab test unit
                            .done(function (items) {
                                var labUnits = [{ Id: "", Description: "" }];
                                ko.utils.arrayForEach(items, function (item) {
                                    labUnits.push(item);
                                });
                                labTestUnits(labUnits);
                            });

                    dataservice.customAttribute.localService.getSelectionData() // load custom attributes data
                           .done(function (data) {
                               selectionData(data);
                           });

                    dataservice.patientLabTest.localService.getCustomAttributes({}, 'PatientLabTest')
                        .then(function (customs) { // Dexie promise is returned instead of jquery promise
                            patientLabTest().customAttributes(ko.utils.arrayMap(customs, function (item) {
                                var currentValue = ko.observable(item['currentValue']);
                                currentValue = utils.setCustomValidationRules(currentValue, item);
                                item['currentValue'] = currentValue;
                                return ko.observable(item);
                            }));
                        });
                }
                else {
                    var labTest;
                    dataservice.patientLabTest.localService.getById('PatientLabTestId', parseInt(id))
                    .then(function (item) {                       
                        labTest = item;
                        return dataservice.patientLabTest.localService.getLabTestType() // load lab test type
                            .done(function (items) {
                                testLabTypes(ko.utils.arrayMap(items, function (item) {
                                    return item.Text;
                                }));                                
                            });
                    })
                        .then(function () {
                           return dataservice.patientLabTest.localService.getLabTestUnit() // load lab test unit
                                .done(function (items) {
                                    var labUnits = [{ Id: "", Description: "" }];
                                    ko.utils.arrayForEach(items, function (item) {
                                        labUnits.push(item);
                                    });
                                    labTestUnits(labUnits);
                                });
                        })
                        .then(function () {
                           return dataservice.customAttribute.localService.getSelectionData() // load custom attributes data
                                .done(function (data) {
                                    selectionData(data);
                                });
                        })
                    .done(function () {
                        var labTestObj = mapper.patientLabTest.fromDto(labTest);
                        patientLabTest(labTestObj);
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

                    var plainObject = mapper.patientLabTest.toDto(patientLabTest);

                    plainObject.synchStatus = "synchronization required";

                    if (!patientLabTest().PatientLabTestIdentifier()) { // new lab test
                        dataservice.patientLabTest.localService.getSequenceId()
                            .done(function (id) {
                                plainObject.PatientLabTestIdentifier = uuid.v4();
                                plainObject.PatientLabTestId = id;
                                plainObject.PatientId = parseInt(utils.getUrlVar('patientId'));

                                dataservice.patientLabTest.localService.addOrUpdate(plainObject)
                                    .done(function () {
                                        
                                    })
                                    .always(function () {
                                        isExecuting(false);
                                        window.location.href = "/Encounter/ViewEncounterOffline#encounterId=" + parseInt(utils.getUrlVar('encounterId')) + "&id=" + plainObject.PatientId;
                                    });
                            });
                    }
                    else {
                        dataservice.patientLabTest.localService.addOrUpdate(plainObject)
                            .done(function () {
                                //var labTest = mapper.patientLabTest.fromDto(plainObject);
                                //patientLabTest(labTest);
                            })
                            .always(function () {
                                isExecuting(false);
                                window.location.href = "/Encounter/ViewEncounterOffline#encounterId=" + parseInt(utils.getUrlVar('encounterId')) + "&id=" + plainObject.PatientId;
                            });
                    }
                    
                }
                else {
                    if (!isValid())
                    {
                        showErrors(true);
                        ko.validation.group(patientLabTest()).showAllMessages();
                    }
                        
                }

            };

        activate();

        return {
            patientLabTest: patientLabTest,
            widgetTitle: widgetTitle,
            isValid: isValid,
            testLabTypes: testLabTypes,
            labTestUnits: labTestUnits,
            TestResults: TestResults,
            filterAttributes: filterAttributes,
            showErrors: showErrors,
            createURL: utils.createURL,
            encounterId: encounterId,
            save: save
        };
    });
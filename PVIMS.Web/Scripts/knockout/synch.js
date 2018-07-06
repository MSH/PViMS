define('synch',
    [
        'jquery',
        'dataservice',
        'config',
        'Dexie',
        'moment',
        'utils'
    ],
    function ($,dataservice, config, Dexie, moment,utils) {
        
        var synchData = function () {
            if (utils.online())
                pushData().then(function () {
                    pullData();
                });                    
            },

            pushData = function () {
                var returnData = {};
                return $.Deferred(function (def) {
                    // get data from local storage
                    dataservice.dataServicePatient.localService.pull().then(function (items) {
                        // push data to online storage
                        return dataservice.dataServicePatient.remoteService.push({ results: returnData, data: items }).then(function () {
                            if (returnData.patientData)
                            {
                                // update local Ids with new values assigned on online storage to keep integrity 
                                return dataservice.dataServicePatient.localService.addBatch($.map(returnData.patientData, function (el) {
                                    el.synchStatus = "synchronized";
                                    return el;
                                }));
                            }
                        });
                    })
                        .then(function () {
                            // get data from local storage
                            dataservice.encounterService.localService.pull().then(function (items) {
                                // push data to online storage
                                return dataservice.encounterService.remoteService.push({ results: returnData, data: items }).then(function () {
                                    if (returnData.encounterData) {
                                        // update local Ids with new values assigned on online storage to keep integrity 
                                        return dataservice.encounterService.localService.addBatch($.map(returnData.encounterData, function (el) {
                                            el.synchStatus = "synchronized";
                                            return el;
                                        }));
                                    }
                                });
                            })
                        })

                        .then(function () {
                            // get data from local storage
                            dataservice.patientClinicalEvent.localService.pull().then(function (items) {
                                // push data to online storage
                                return dataservice.patientClinicalEvent.remoteService.push({ results: returnData, data: items }).then(function () {
                                    if (returnData.patientClinicalEventData)
                                    {
                                        // update local Ids with new values assigned on online storage to keep integrity 
                                        return dataservice.patientClinicalEvent.localService.addBatch($.map(returnData.patientClinicalEventData, function (el) {
                                            el.synchStatus = "synchronized";
                                            return el;
                                        }));
                                    }
                                });
                            });
                        })

                        .then(function () {
                            // get data from local storage
                            dataservice.patientCondition.localService.pull().then(function (items) {
                                // push data to online storage
                                return dataservice.patientCondition.remoteService.push({ results: returnData, data: items }).then(function () {
                                    if (returnData.patientConditionData)
                                    {
                                        // update local Ids with new values assigned on online storage to keep integrity 
                                        return dataservice.patientCondition.localService.addBatch($.map(returnData.patientConditionData, function (el) {
                                            el.synchStatus = "synchronized";
                                            return el;
                                        }));
                                    }
                                });
                            });
                        })

                        .then(function () {
                            // get data from local storage
                            dataservice.patientLabTest.localService.pull().then(function (items) {
                                // push data to online storage
                                return dataservice.patientLabTest.remoteService.push({ results: returnData, data: items }).then(function () {
                                    if (returnData.patientLabTestData)
                                    {
                                        // update local Ids with new values assigned on online storage to keep integrity 
                                        return dataservice.patientLabTest.localService.addBatch($.map(returnData.patientLabTestData, function (el) {
                                            el.synchStatus = "synchronized";
                                            return el;
                                        }));
                                    }
                                });
                            });
                        })

                        .then(function () {
                            // get data from local storage
                            dataservice.patientMedication.localService.pull().then(function (items) {
                                // push data to online storage
                                return dataservice.patientMedication.remoteService.push({ results: returnData, data: items }).then(function () {
                                    if (returnData.patientMedicationData)
                                    {
                                        // update local Ids with new values assigned on online storage to keep integrity 
                                        return dataservice.patientMedication.localService.addBatch($.map(returnData.patientMedicationData, function (el) {
                                            el.synchStatus = "synchronized";
                                            return el;
                                        }));
                                    }
                                });
                            });
                        })

                        .done(function () { def.resolve();})
                        .fail(function () { def.reject(); });
                }).promise();
               
            },

            pullData = function () {

            var promises = [];

            var data = {
                patientData: {},
                customAttributes: [],
                selectionDataItems: []
            };

            config.changeIdleStatusIndicator(config.indicatorMessages.synch);

            var lastUpdatedDate = localStorage.getItem("lastUpdatedDate") || "";
            var patientsUri = "/api/patientapi/lastpatients/" + (lastUpdatedDate && lastUpdatedDate != "undefined" ?
                "?id=" + moment(lastUpdatedDate).format("YYYY-MM-DD hh:mmA") : ""),

                encountersUri = lastUpdatedDate && lastUpdatedDate != "undefined" ? "/api/encounterapi/lookUpByDate/?id="
                + moment(lastUpdatedDate).format("YYYY-MM-DD hh:mmA") : undefined;

            // add load operation from server to promises array
            promises.push(dataservice.dataServicePatient.remoteService.getData(patientsUri, 'GET', { results: data }));
            promises.push(dataservice.dataServicePatient.remoteService.getFacilities(undefined, 'GET', { results: data }));

            promises.push(dataservice.customAttribute.remoteService.getCustomAttributes(undefined, 'GET', { results: data }));
            promises.push(dataservice.customAttribute.remoteService.getSelectionData(undefined, 'GET', { results: data }));

            promises.push(dataservice.encounterService.remoteService.getData(encountersUri, 'GET', { results: data }));

            promises.push(dataservice.patientLabTest.remoteService.getPatientLabTests(undefined, 'GET', { results: data }));
            promises.push(dataservice.patientLabTest.remoteService.getLabTestType(undefined, 'GET', { results: data }));
            promises.push(dataservice.patientLabTest.remoteService.getLabTestUnit(undefined, 'GET', { results: data }));

            promises.push(dataservice.patientClinicalEvent.remoteService.getPatientClinicalEvent(undefined, 'GET', { results: data }));
            promises.push(dataservice.patientClinicalEvent.remoteService.getTerminologyMedDra(undefined, 'GET', { results: data }));

            promises.push(dataservice.patientMedication.remoteService.getPatientMedication(undefined, 'GET', { results: data }));
            promises.push(dataservice.patientMedication.remoteService.getMedication(undefined, 'GET', { results: data }));

            promises.push(dataservice.patientCondition.remoteService.getPatientCondition(undefined, 'GET', { results: data }));
            promises.push(dataservice.patientCondition.remoteService.getCondition(undefined, 'GET', { results: data }));

            // config data TODO: Only load when not exists
            promises.push(dataservice.encounterService.remoteService.getEncounterTypes(undefined, 'GET', { results: data }));
            promises.push(dataservice.encounterService.remoteService.getPriorities(undefined, 'GET', { results: data }));

            // executes promises to load data and save locally
            $.when.apply($, promises) // when all promises are resolved go next
                .then(function () {
                    dataservice.dataServicePatient.localService.addBatch(data.patientData.patients);
                })

                .then(function () {
                    dataservice.dataServicePatient.localService.addFacilities(data.facilities);
                })

                .then(function () {
                    dataservice.customAttribute.localService.addBatchcustomAttributes(data.customAttributes);
                })

                .then(function () {
                    dataservice.customAttribute.localService.addBatchSelectionDataItem(data.selectionDataItems);
                })

                .then(function () {
                    dataservice.encounterService.localService.addBatch(data.encountersData);
                })

                .then(function () {
                    dataservice.encounterService.localService.addBatchEncounterTypes(data.encounterTypes);
                })

                .then(function () {
                    dataservice.encounterService.localService.addBatchPriorities(data.priorities);
                })

                .then(function () {
                    dataservice.patientLabTest.localService.addBatch(data.labTestsData);
                    dataservice.patientLabTest.localService.addLabTestType(data.labTestsType);
                    dataservice.patientLabTest.localService.addLabTestUnit(data.labTestsUnit);
                })
                
                .then(function () {
                    dataservice.patientClinicalEvent.localService.addBatch(data.clinicalEventData);
                })

                .then(function () {
                    dataservice.patientClinicalEvent.localService.addTerminologyMedDra(data.teminologyMedDra);
                })

                .then(function () {
                    dataservice.patientMedication.localService.addBatch(data.patientMedicationData);
                })

                .then(function () {
                    dataservice.patientMedication.localService.addMedication(data.medications);
                })

                .then(function () {
                    dataservice.patientCondition.localService.addBatch(data.patientConditionData);
                })

                .then(function () {
                    dataservice.patientCondition.localService.addConditions(data.conditions);
                })

                .done(function () {
                    console.log("Got ajax response. The objects has been added.");                    
                    localStorage.setItem("lastUpdatedDate", data.patientData.lastUpdatedDate);
                    config.changeIdleStatusIndicator(config.indicatorMessages.idle);
                    return $.when(true);
                        
                });
        },

        runSynchData = function () {
            var intervals = localStorage.getItem("synchIntervals") || config.synchIntervals;
            setInterval(synchData, intervals);
        };

        return {
            synchData: synchData,
            runSynchData: runSynchData
        }


    });
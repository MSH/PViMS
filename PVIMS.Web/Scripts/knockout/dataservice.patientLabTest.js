define('dataservice.patientLabTest',
    ['jquery', 'db', 'Dexie', 'dataservice.common'],
    function ($, db, Dexie, common) {

        var
            remoteService = (function () {
                var
                    patientLabTestUri = "/api/PatientLabTestapi",

                    getPatientLabTests = function (uri, method, options) {
                        return $.ajax({
                            type: method,
                            url: uri || patientLabTestUri,
                            dataType: 'json',
                            contentType: 'application/json',
                            data: options.data ? JSON.stringify(options.data) : null
                        }).done(function (data) {
                            options.results.labTestsData = data;
                        })
                            .fail(function (jqXHR, textStatus, errorThrown) {

                            });
                    },

                     getLabTestType = function (uri, method, options) {
                         return $.ajax({
                             type: method,
                             url: uri || patientLabTestUri + '/getLabTestType',
                             dataType: 'json',
                             contentType: 'application/json',
                             data: options && options.data ? JSON.stringify(options.data) : null
                         }).done(function (result) {
                             options.results.labTestsType = result;
                         }).fail(function (jqXHR, textStatus, errorThrown) {

                         });
                     },

                getLabTestUnit = function (uri, method, options) {
                    return $.ajax({
                        type: method,
                        url: uri || patientLabTestUri + '/getLabTestUnit',
                        dataType: 'json',
                        contentType: 'application/json',
                        data: options && options.data ? JSON.stringify(options.data) : null
                    }).done(function (result) {
                        options.results.labTestsUnit = result;
                    }).fail(function (jqXHR, textStatus, errorThrown) {

                    });
                },

                push = function (options) {
                    return $.ajax({
                        type: "POST",
                        url: patientLabTestUri,
                        data: options && options.data ? { items: options.data } : null
                    }).done(function (result) {
                        options.results.patientLabTestData = result;
                    }).fail(function (jqXHR, textStatus, errorThrown) {
                        console.log(textStatus + " Error pushing Patient Lab Test " + errorThrown);
                        console.log(jqXHR.responseText.Message);
                    });
                };

                return {
                    getPatientLabTests: getPatientLabTests,
                    getLabTestType: getLabTestType,
                    getLabTestUnit: getLabTestUnit,
                    push: push
                };

            })(),

            localService = (function () {
                var
                    getAllById = function (keyId, keyValue) {
                        return $.Deferred(function (def) {
                            return db.patientLabTest.where(keyId).equals(keyValue).toArray(function (data) {
                                return Dexie.Promise.all(data.map(function (patTest) {
                                    return db.labTestUnit.where('Id').equals(patTest.TestUnit).first(function (TestUnit) {
                                        patTest.unitObject = TestUnit;
                                        return patTest;
                                    });
                                })).then(function (patientTest) {
                                    def.resolve(patientTest);
                                });
                            }).catch(function (error) {
                                def.reject();
                            });
                        }).promise();

                    },

                    getById = function (keyId, keyValue) {
                        return $.Deferred(function (def) {
                            return db.patientLabTest.where(keyId).equals(keyValue).first(function (item) {
                                return common.getCustomAttributes(item, 'PatientLabTest').then(function (customAttributes) {
                                    item.customAttributes = customAttributes;
                                    return db.labTestUnit.where('Id').equals(item.TestUnit).first(function (TestUnit) {
                                        item.unitObject = TestUnit;
                                        def.resolve(item);
                                    });
                                });
                            }).catch(function (error) {
                                def.reject();
                            });
                        }).promise();

                    },

                    getSequenceId = function () {
                        return $.Deferred(function (def) {
                            common.getSequenceId('patientLabTest', 'PatientLabTestId').done(function (id) {
                                def.resolve(id);
                            }).fail(function () {
                                def.reject();
                            });
                        }).promise();
                    },

                    getCustomAttributes = common.getCustomAttributes,

                    addOrUpdate = function (item) {
                        return $.Deferred(function (def) {
                            return db.patientLabTest.put(item).then(function () {
                                def.resolve();
                            }).catch(function (error) {
                                def.reject();
                            });
                        }).promise();

                    },

                    addLabTestType = function (items) {
                        return $.Deferred(function (def) {
                            return db.transaction('rw', db.labTestType, function () {

                                var numObjects = items.length;

                                for (var i = 0; i < numObjects; i++) {
                                    var obj = items[i];
                                    db.labTestType.put(obj);
                                }

                            }).then(function () {
                                def.resolve();

                            }).catch(function (err) {

                                console.log(err);
                                def.reject();
                            });

                        }).promise();
                    },

                    getLabTestType = function () {

                        return $.Deferred(function (def) {
                            return db.labTestType.toArray().then(function (data) {
                                def.resolve(data);
                            }).catch(function (error) {
                                def.reject();
                            });
                        }).promise();

                    },

                    getLabTestUnit = function () {

                        return $.Deferred(function (def) {
                            return db.labTestUnit.toArray().then(function (data) {
                                def.resolve(data);
                            }).catch(function (error) {
                                def.reject();
                            });
                        }).promise();

                    },

                    addLabTestUnit = function (items) {
                        return $.Deferred(function (def) {
                            return db.transaction('rw', db.labTestUnit, function () {

                                var numObjects = items.length;

                                for (var i = 0; i < numObjects; i++) {
                                    var obj = items[i];
                                    db.labTestUnit.put(obj);
                                }

                            }).then(function () {
                                def.resolve();

                            }).catch(function (err) {

                                console.log(err);
                                def.reject();
                            });

                        }).promise();
                    },

                    addBatch = function (items) {
                        return $.Deferred(function (def) {
                            return db.transaction('rw', db.patientLabTest, function () {

                                var numObjects = items.length;

                                for (var i = 0; i < numObjects; i++) {
                                    var obj = items[i];
                                    db.patientLabTest.put(obj);
                                }

                            }).then(function () {
                                def.resolve();

                            }).catch(function (err) {

                                console.log(err);
                                def.reject();
                            });

                        }).promise();
                    },

                    pull = function () {
                        return $.Deferred(function (def) {
                            return db.patientLabTest.where('synchStatus').notEqual('synchronized').toArray(function (data) {
                                return Dexie.Promise.all(data.map(function (patMedication) {
                                    return getCustomAttributes(patMedication, 'PatientLabTest').then(function (attributes) {
                                        patMedication.customAttributes = attributes;
                                        return patMedication;
                                    });
                                })).then(function (patientMedications) {
                                    def.resolve(patientMedications);
                                });
                            }).catch(function (error) {
                                def.reject();
                            });
                        }).promise();

                    },

                    remove = function (identifier) {
                        return $.Deferred(function (def) {
                            return db.patientLabTest.delete(identifier).then(function () {
                                def.resolve();
                            }).catch(function (error) {
                                def.reject();
                            });
                        }).promise();
                    };

                return {
                    getAllById: getAllById,
                    getById: getById,
                    getSequenceId: getSequenceId,
                    getCustomAttributes: getCustomAttributes,
                    addOrUpdate: addOrUpdate,
                    addBatch: addBatch,
                    addLabTestType: addLabTestType,
                    addLabTestUnit: addLabTestUnit,
                    getLabTestType: getLabTestType,
                    getLabTestUnit: getLabTestUnit,
                    pull: pull,
                    remove: remove
                }
            })();

        return {
            remoteService: remoteService,
            localService: localService
        };
});
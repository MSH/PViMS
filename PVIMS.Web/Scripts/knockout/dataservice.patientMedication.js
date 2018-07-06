define('dataservice.patientMedication',
    ['jquery', 'db', 'Dexie', 'dataservice.common'],
    function ($, db, Dexie, common) {

        var
            remoteService = (function () {
                var
                    defaultUri = "/api/PatientMedicationapi",

                    getPatientMedication = function (uri, method, options) {
                        return $.ajax({
                            type: method,
                            url: uri || defaultUri,
                            dataType: 'json',
                            contentType: 'application/json',
                            data: options.data ? JSON.stringify(options.data) : null
                        }).done(function (data) {
                            options.results.patientMedicationData = data;
                        })
                            .fail(function (jqXHR, textStatus, errorThrown) {

                            });
                    },

                     getMedication = function (uri, method, options) {
                         return $.ajax({
                             type: method,
                             url: uri || defaultUri + '/getMedications',
                             dataType: 'json',
                             contentType: 'application/json',
                             data: options && options.data ? JSON.stringify(options.data) : null
                         }).done(function (result) {
                             options.results.medications = result;
                         }).fail(function (jqXHR, textStatus, errorThrown) {

                         });
                     },

                     push = function (options) {
                         return $.ajax({
                             type: "POST",
                             url: defaultUri,
                             dataType: 'json',
                             //contentType: 'application/json',
                             data: options && options.data ? { items: options.data } : null
                         }).done(function (result) {
                             options.results.patientMedicationData = result;
                         }).fail(function (jqXHR, textStatus, errorThrown) {
                             console.log(textStatus + " Error pushing Patient Medication " + errorThrown);
                         });
                     };

                return {
                    getPatientMedication: getPatientMedication,
                    getMedication: getMedication,
                    push: push
                };

            })(),

            localService = (function () {
                var
                    getAllById = function (keyId, keyValue) {
                        return $.Deferred(function (def) {
                            return db.patientMedication.where(keyId).equals(keyValue).toArray(function (data) {
                                def.resolve(data);
                            }).catch(function (error) {
                                def.reject();
                            });
                        }).promise();

                    },

                    getById = function (keyId, keyValue) {
                        return $.Deferred(function (def) {
                            return db.patientMedication.where(keyId).equals(keyValue).first(function (item) {
                                return common.getCustomAttributes(item, 'PatientMedication').then(function (customAttributes) {
                                    item.customAttributes = customAttributes;
                                    def.resolve(item);
                                });
                            }).catch(function (error) {
                                def.reject();
                            });
                        }).promise();

                    },

                    getSequenceId = function () {
                        return $.Deferred(function (def) {
                            common.getSequenceId('patientMedication', 'PatientMedicationId').done(function (id) {
                                def.resolve(id);
                            }).fail(function () {
                                def.reject();
                            });
                        }).promise();
                    },

                    getCustomAttributes = common.getCustomAttributes,

                    addOrUpdate = function (item) {
                        return $.Deferred(function (def) {
                            return db.patientMedication.put(item).then(function () {
                                def.resolve();
                            }).catch(function (error) {
                                def.reject();
                            });
                        }).promise();

                    },

                    addMedication = function (items) {
                        return $.Deferred(function (def) {
                            return db.transaction('rw', db.medication, function () {

                                var numObjects = items.length;

                                for (var i = 0; i < numObjects; i++) {
                                    var obj = items[i];
                                    db.medication.put(obj);
                                }

                            }).then(function () {
                                def.resolve();

                            }).catch(function (err) {

                                console.log(err);
                                def.reject();
                            });

                        }).promise();
                    },

                    getMedication = function () {

                        return $.Deferred(function (def) {
                            return db.medication.toArray().then(function (data) {
                                def.resolve(data);
                            }).catch(function (error) {
                                def.reject();
                            });
                        }).promise();

                    },

                    addBatch = function (items) {
                        return $.Deferred(function (def) {
                            return db.transaction('rw', db.patientMedication, function () {

                                var numObjects = items.length;

                                for (var i = 0; i < numObjects; i++) {
                                    var obj = items[i];
                                    db.patientMedication.put(obj);
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
                            return db.patientMedication.where('synchStatus').notEqual('synchronized').toArray(function (data) {
                                return Dexie.Promise.all(data.map(function (patMedication) {
                                    return getCustomAttributes(patMedication,'PatientMedication').then(function (attributes) {
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
                            return db.patientMedication.delete(identifier).then(function () {
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
                    addMedication: addMedication,
                    getMedication: getMedication,
                    pull: pull,
                    remove: remove
                }
            })();

        return {
            remoteService: remoteService,
            localService: localService
        };
    });
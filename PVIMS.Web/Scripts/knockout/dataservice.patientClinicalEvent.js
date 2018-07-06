define('dataservice.patientClinicalEvent',
    ['jquery', 'db', 'Dexie', 'dataservice.common'],
    function ($, db, Dexie, common) {

        var
            remoteService = (function () {
                var
                    patientClinicalEventUri = "/api/PatientClinicalEventApi",

                    getPatientClinicalEvent = function (uri, method, options) {
                        return $.ajax({
                            type: method,
                            url: uri || patientClinicalEventUri,
                            dataType: 'json',
                            contentType: 'application/json',
                            data: options.data ? JSON.stringify(options.data) : null
                        }).done(function (data) {
                            options.results.clinicalEventData = data;
                        })
                            .fail(function (jqXHR, textStatus, errorThrown) {

                            });
                    },
                    
                    getTerminologyMedDra = function (uri, method, options) {
                        return $.ajax({
                            type: method,
                            url: uri || patientClinicalEventUri + '/getTerminologyMedDra',
                            dataType: 'json',
                            contentType: 'application/json',
                            data: options && options.data ? JSON.stringify(options.data) : null
                        }).done(function (result) {
                            options.results.teminologyMedDra = result;
                        }).fail(function (jqXHR, textStatus, errorThrown) {

                        });
                    },
                    
                    push = function (options) {
                        return $.ajax({
                            type: "POST",
                            url: patientClinicalEventUri,
                            data: options && options.data ? { items: options.data } : null
                        }).done(function (result) {
                            options.results.patientClinicalEventData = result;
                        }).fail(function (jqXHR, textStatus, errorThrown) {
                            console.log(textStatus + " Error pushing Patient clinical event " + errorThrown);
                            console.log(jqXHR.responseText.Message);
                        });
                    };

                return {
                    getPatientClinicalEvent: getPatientClinicalEvent,
                    getTerminologyMedDra: getTerminologyMedDra,
                    push: push
                };

            })(),

            localService = (function () {
                var
                    getAllById = function (keyId, keyValue) {
                        return $.Deferred(function (def) {
                            return db.patientClinicalEvent.where(keyId).equals(keyValue).toArray(function (data) {
                                return Dexie.Promise.all(data.map(function (patClinicalEvent) {
                                    return db.terminologyMedDra.where('Id').equals(patClinicalEvent.MedDraId).first(function (medDra) {
                                        patClinicalEvent.medDra = medDra;
                                        return patClinicalEvent;
                                    });
                                })).then(function (patientClinicalEvents) {
                                    def.resolve(patientClinicalEvents);
                                });                                
                            }).catch(function (error) {
                                def.reject();
                            });
                        }).promise();

                    },

                    getById = function (keyId, keyValue) {
                        return $.Deferred(function (def) {
                            return db.patientClinicalEvent.where(keyId).equals(keyValue).first(function (item) {
                                return common.getCustomAttributes(item, 'PatientClinicalEvent').then(function (customAttributes) {
                                    item.customAttributes = customAttributes;
                                    return db.terminologyMedDra.where('Id').equals(item.MedDraId).first(function (medDra) {
                                        item.medDra = medDra;
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
                            common.getSequenceId('patientClinicalEvent', 'PatientClinicalEventId').done(function (id) {
                                def.resolve(id);
                            }).fail(function () {
                                def.reject();
                            });
                        }).promise();
                    },

                    getCustomAttributes = common.getCustomAttributes,

                    addOrUpdate = function (item) {
                        return $.Deferred(function (def) {
                            return db.patientClinicalEvent.put(item).then(function () {
                                def.resolve();
                            }).catch(function (error) {
                                def.reject();
                            });
                        }).promise();

                    },

                    addTerminologyMedDra = function (items) {
                        return $.Deferred(function (def) {
                            return db.transaction('rw', db.terminologyMedDra, function () {

                                var numObjects = items.length;

                                for (var i = 0; i < numObjects; i++) {
                                    var obj = items[i];
                                    db.terminologyMedDra.put(obj);
                                }

                            }).then(function () {
                                def.resolve();

                            }).catch(function (err) {

                                console.log(err);
                                def.reject();
                            });

                        }).promise();
                    },

                    getTerminologyMedDra = function (termType, termFind) {

                        return $.Deferred(function (def) {
                            return db.terminologyMedDra.where('MedDraTermType').startsWith(termType)
                                .and(function (tt) {
                                    return tt.Description.toLowerCase().startsWith(termFind.toLowerCase());
                                })
                                .toArray().then(function (data) {
                                    def.resolve(data);
                                }).catch(function (error) {
                                    def.reject();
                                });
                        }).promise();

                    },

                    addBatch = function (items) {
                        return $.Deferred(function (def) {
                            return db.transaction('rw', db.patientClinicalEvent, function () {

                                var numObjects = items.length;

                                for (var i = 0; i < numObjects; i++) {
                                    var obj = items[i];
                                    db.patientClinicalEvent.put(obj);
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
                            return db.patientClinicalEvent.where('synchStatus').notEqual('synchronized').toArray(function (data) {
                                return Dexie.Promise.all(data.map(function (patClinicalEvent) {
                                    return getCustomAttributes(patClinicalEvent, 'PatientClinicalEvent').then(function (attributes) {
                                        patClinicalEvent.customAttributes = attributes;
                                        return patClinicalEvent;
                                    });
                                })).then(function (patientClinicalEvents) {
                                    def.resolve(patientClinicalEvents);
                                });
                            }).catch(function (error) {
                                def.reject();
                            });
                        }).promise();

                    },

                    remove = function (identifier) {
                        return $.Deferred(function (def) {
                            return db.patientClinicalEvent.delete(identifier).then(function () {
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
                    getTerminologyMedDra: getTerminologyMedDra,
                    addTerminologyMedDra: addTerminologyMedDra,
                    addOrUpdate: addOrUpdate,
                    addBatch: addBatch,
                    pull: pull,
                    remove: remove
                }
            })();

        return {
            remoteService: remoteService,
            localService: localService
        };
    });
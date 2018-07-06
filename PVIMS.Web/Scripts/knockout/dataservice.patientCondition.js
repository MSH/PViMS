define('dataservice.patientCondition',
    ['jquery', 'db', 'dataservice.common','Dexie'],
    function ($, db, common, Dexie) {

        var
            remoteService = (function () {
                var
                    defaultUri = "/api/PatientConditionApi",

                    getPatientCondition = function (uri, method, options) {
                        return $.ajax({
                            type: method,
                            url: uri || defaultUri,
                            dataType: 'json',
                            contentType: 'application/json',
                            data: options.data ? JSON.stringify(options.data) : null
                        }).done(function (data) {
                            options.results.patientConditionData = data;
                        })
                            .fail(function (jqXHR, textStatus, errorThrown) {

                            });
                    },

                     getCondition = function (uri, method, options) {
                         return $.ajax({
                             type: method,
                             url: uri || defaultUri + '/getConditions',
                             dataType: 'json',
                             contentType: 'application/json',
                             data: options && options.data ? JSON.stringify(options.data) : null
                         }).done(function (result) {
                             options.results.conditions = result;
                         }).fail(function (jqXHR, textStatus, errorThrown) {

                         });
                     },

                     push = function (options) {
                         return $.ajax({
                             type: "POST",
                             url: defaultUri,
                             //contentType: "application/json",
                             data: options && options.data ? {items:options.data}: null
                         }).done(function (result) {
                             options.results.patientConditionData = result;
                         }).fail(function (jqXHR, textStatus, errorThrown) {
                             console.log(textStatus + " Error pushing Patient Condition " + errorThrown);
                             console.log(jqXHR.responseText.Message);
                         });
                     };

                return {
                    getPatientCondition: getPatientCondition,
                    getCondition: getCondition,
                    push: push
                };

            })(),

            localService = (function () {
                var
                    getAllById = function (keyId, keyValue) {
                        return $.Deferred(function (def) {
                            return db.patientCondition.where(keyId).equals(keyValue).toArray(function (data) {
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
                            return db.patientCondition.where(keyId).equals(keyValue).first(function (item) {
                                return common.getCustomAttributes(item, 'PatientCondition').then(function (customAttributes) {
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
                            common.getSequenceId('patientCondition', 'PatientConditionId').done(function (id) {
                                def.resolve(id);
                            }).fail(function () {
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

                    getCustomAttributes = common.getCustomAttributes,

                    addOrUpdate = function (item) {
                        return $.Deferred(function (def) {
                            return db.patientCondition.put(item).then(function () {
                                def.resolve();
                            }).catch(function (error) {
                                def.reject();
                            });
                        }).promise();

                    },

                    addConditions = function (values) {
                        items = values || [];
                        return $.Deferred(function (def) {
                            return db.transaction('rw', db.condition, function () {

                                var numObjects = items.length;

                                for (var i = 0; i < numObjects; i++) {
                                    var obj = items[i];
                                    db.condition.put(obj);
                                }

                            }).then(function () {
                                def.resolve();

                            }).catch(function (err) {

                                console.log(err);
                                def.reject();
                            });

                        }).promise();
                    },

                    getConditions = function () {

                        return $.Deferred(function (def) {
                            return db.condition.toArray().then(function (data) {
                                def.resolve(data);
                            }).catch(function (error) {
                                def.reject();
                            });
                        }).promise();

                    },

                    addBatch = function (values) {
                        items = values || [];
                        return $.Deferred(function (def) {
                            return db.transaction('rw', db.patientCondition, function () {

                                var numObjects = items.length;

                                for (var i = 0; i < numObjects; i++) {
                                    var obj = items[i];
                                    db.patientCondition.put(obj);
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
                            return db.patientCondition.where('synchStatus').notEqual('synchronized').toArray(function (data) {
                                return Dexie.Promise.all(data.map(function (patCondition) {
                                    return getCustomAttributes(patCondition, 'PatientCondition').then(function (attributes) {
                                        patCondition.customAttributes = attributes;
                                        return patCondition;
                                    });
                                })).then(function (patientConditions) {
                                    def.resolve(patientConditions);
                                });
                            }).catch(function (error) {
                                def.reject();
                            });
                        }).promise();

                    },

                    remove = function (identifier) {
                        return $.Deferred(function (def) {
                            return db.patientCondition.delete(identifier).then(function () {
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
                    addConditions: addConditions,
                    getConditions: getConditions,
                    getTerminologyMedDra,
                    pull: pull,
                    remove: remove
                }
            })();

        return {
            remoteService: remoteService,
            localService: localService
        };
    });
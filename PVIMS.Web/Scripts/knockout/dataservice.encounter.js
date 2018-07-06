/// <reference path="../_references.js" />

define('dataservice.encounter',
    ['jquery', 'db', 'Dexie', 'underscore', 'moment'],
    function ($, db, Dexie, _, moment) {

        var remoteService = (function () {
            var encountersUri = "/api/encounterapi",

            getById = function (id, data) {
                return $.ajax({
                    type: 'GET',
                    url: encountersUri + '/?id=' + id,
                    dataType: 'json',
                    contentType: 'application/json',
                    data: data ? JSON.stringify(data) : null
                }).fail(function (jqXHR, textStatus, errorThrown) {

                });

            },

             getTodayEncounters = function (uri, method, data) {
                 return $.ajax({
                     type: method,
                     url: uri || encountersUri + '/lookUpTodayEncounters',
                     dataType: 'json',
                     contentType: 'application/json',
                     data: data ? JSON.stringify(data) : null
                 }).done(function (result) {
                     return data;
                 }).fail(function (jqXHR, textStatus, errorThrown) {

                 });
             },

             getEncounterTypes = function (uri, method, options) {
                 return $.ajax({
                     type: method,
                     url: uri || encountersUri + '/encounterTypes',
                     dataType: 'json',
                     contentType: 'application/json',
                     data: options.data ? JSON.stringify(options.data) : null
                 }).done(function (data) {
                     options.results.encounterTypes = data;
                 })
                     .fail(function (jqXHR, textStatus, errorThrown) {

                     });
             },

             getPriorities = function (uri, method, options) {
                 return $.ajax({
                     type: method,
                     url: uri || encountersUri + '/priorities',
                     dataType: 'json',
                     contentType: 'application/json',
                     data: options.data ? JSON.stringify(options.data) : null
                 }).done(function (data) {
                     options.results.priorities = data;
                 })
                     .fail(function (jqXHR, textStatus, errorThrown) {

                     });
             },

            getData = function (uri, method, options) {
                return $.ajax({
                    type: method,
                    url: uri || encountersUri,
                    dataType: 'json',
                    contentType: 'application/json',
                    data: options.data ? JSON.stringify(options.data) : null
                }).done(function (data) {
                    options.results.encountersData = data;
                })
                    .fail(function (jqXHR, textStatus, errorThrown) {

                    });
            },

            push = function (options) {
                return $.ajax({
                    type: "POST",
                    url: encountersUri,
                    data: options && options.data ? { items: options.data } : null
                }).done(function (result) {
                    options.results.encounterData = result;
                }).fail(function (jqXHR, textStatus, errorThrown) {
                    console.log(textStatus + " Error pushing encounter " + errorThrown);
                    console.log(jqXHR.responseText.Message);
                });
            };

            return {
                getData: getData,
                getById: getById,
                getTodayEncounters: getTodayEncounters,
                getEncounterTypes: getEncounterTypes,
                getPriorities: getPriorities,
                push: push
            };

        })(),

        localService = (function () {

            var getById = function (keyId, keyValue) {
                return $.Deferred(function (def) {
                    return db.encounters.where(keyId).equals(keyValue).first(function (item) {
                        def.resolve(item);
                    }).catch(function (error) {
                        def.reject();
                    });
                }).promise();

            },

            getTodayEncounters = function () {
                var today = moment(Date.now()).format("YYYY-MM-DD") + "T00:00:00";
                return $.Deferred(function (def) {
                    return db.encounters.where('EncounterDate').equals(today).toArray(function (data) {
                        def.resolve(data);
                    }).catch(function (error) {
                        def.reject();
                    });
                }).promise();
            },

            getEncounterTypes = function () {
                return $.Deferred(function (def) {
                    return db.encounterType.toArray(function (items) {
                        def.resolve(items);
                    }).catch(function (error) {
                        def.reject();
                    });
                }).promise();
            },

            getPriorities = function () {
                return $.Deferred(function (def) {
                    return db.priority.toArray(function (items) {
                        def.resolve(items);
                    }).catch(function (error) {
                        def.reject();
                    });
                }).promise();
            },

            getSequenceId = function () {

                return $.Deferred(function (def) {
                    return db.encounters.toCollection().count().then(function (count) {
                        if (count > 0)
                            db.encounters.orderBy('EncounterId').keys(function (Ids) {
                                var nextId = Ids[Ids.length - 1] + 1;
                                def.resolve(nextId);
                            });
                        else
                            def.resolve(1);
                    }).catch(function (error) {
                        def.reject();
                    });
                }).promise();

            },

            addOrUpdate = function (item) {

                return $.Deferred(function (def) {
                    return db.encounters.put(item).then(function () {
                        def.resolve();
                    }).catch(function (error) {
                        def.reject();
                    });
                }).promise();

            },

            searchByIds = function (criterias, useIdentifier) {

                var useIdentifier = typeof useIdentifier !== 'undefined' ? useIdentifier : false;
                var isFirstQsItem = true;
                var query;

                // if primary keys is defined using it to fetch the record
                //return getByPatientId2(criterias[0].PatientIdentifier);
                if (useIdentifier)
                    return getByPatientId(criterias[0].PatientIdentifier);

                // loop throught the criteria to create the query
                criterias.forEach(function (criteria)
                {
                    var criteriaKeys = Object.keys(criteria);

                    if (isFirstQsItem)
                    {
                        query = db.patients.where(criteriaKeys[0])
                            .startsWithIgnoreCase(criteria[criteriaKeys[0]]);
                        isFirstQsItem = false;
                    }
                    else {
                        query = query.and(function (patient) {
                            return (patients[criteriaKeys[0]]).toLowerCase()
                                .startsWith((criteria[criteriaKeys[0]]).toLowerCase());
                        });
                    }
                });

                // execute the query
                var encounters = [];
                if (query) {
                    return $.Deferred(function (def) {
                        return query.toArray().then(function (data) {
                            data.forEach(function (patient) {
                                return db.encounters.where("PatientId").equals(parseInt(patient.PatientId)).toArray(function (encounterdata) {
                                    encounterdata.forEach(function (encounter) {
                                        encounter.Patient = patient;
                                        encounters.push(encounter);
                                    });

                                    def.resolve(encounters);
                                });
                            });
                        }).catch(function (error) {
                            def.reject();
                        });
                    }).promise();
                }
            },

            getByPatientId = function (id) {
                return $.Deferred(function (def) {
                    return db.encounters.where("PatientId").equals(parseInt(id)).toArray(function (data) {
                        return db.patients.where('PatientId').equals(parseInt(id)).first(function (patientdata) {
                            data.forEach(function (encounter) {
                                encounter.Patient = patientdata;
                            });
                            
                            def.resolve(data);
                        });
                    }).catch(function (error) {
                        def.reject();
                    });
                }).promise();
            },

            getByPatientId2 = function (id) {
                var encounters = [];
                return $.Deferred(function (def) {
                    return db.patients.where("PatientFirstName").startsWith("Hannah").toArray(function (patient) {
                        return db.encounters.where("PatientId").equals(parseInt(id)).toArray(function (data) {
                            data.forEach(function (encounter) {
                                encounter.Patient = patient[0];
                                encounters.push(encounter);
                            });
                            def.resolve(encounters);
                        });
                    }).catch(function (error) {
                        def.reject();
                    });
                }).promise();
            },

            getEncounterTypeById = function (id) {

                return $.Deferred(function (def) {
                    return db.encounterType.get(id).then(function (item) {
                        def.resolve(item);
                    }).catch(function (error) {
                        def.reject();
                    });
                }).promise();

            },

            addBatchEncounterTypes = function (items) {
                 return $.Deferred(function (def) {
                     return db.transaction('rw', db.encounterType, function () {

                         var numObjects = items.length;

                         for (var i = 0; i < numObjects; i++) {
                             var obj = items[i];
                             db.encounterType.put(obj);
                         }

                     }).then(function () {
                         def.resolve();

                     }).catch(function (err) {

                         console.log(err);
                         def.reject();
                     });

                 }).promise();
             },

            addBatchPriorities = function (items) {
                  return $.Deferred(function (def) {
                      return db.transaction('rw', db.priority, function () {

                          var numObjects = items.length;

                          for (var i = 0; i < numObjects; i++) {
                              var obj = items[i];
                              db.priority.put(obj);
                          }

                      }).then(function () {
                          def.resolve();

                      }).catch(function (err) {

                          console.log(err);
                          def.reject();
                      });

                  }).promise();
              }

            addBatch = function (items) {
                return $.Deferred(function (def) {
                    return db.transaction('rw', db.encounters, function () {

                        var numObjects = items.length;

                        for (var i = 0; i < numObjects; i++) {
                            var obj = items[i];
                            db.encounters.put(obj);
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
                    return db.encounters.where('synchStatus').notEqual('synchronized').toArray(function (data) {
                        def.resolve(data);
                    }).catch(function (error) {
                        def.reject();
                    });
                }).promise();

            };

            return {
                getById: getById,
                getEncounterTypeById: getEncounterTypeById,
                addBatch: addBatch,
                getTodayEncounters: getTodayEncounters,
                searchByIds: searchByIds,
                getEncounterTypes: getEncounterTypes,
                getPriorities: getPriorities,
                getSequenceId: getSequenceId,
                addBatchEncounterTypes: addBatchEncounterTypes,
                addBatchPriorities: addBatchPriorities,
                pull: pull,
                addOrUpdate: addOrUpdate
            }
        })();

        return {
            remoteService: remoteService,
            localService: localService,
        };

 });
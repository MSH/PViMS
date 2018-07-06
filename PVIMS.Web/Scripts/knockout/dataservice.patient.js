/// <reference path="../_references.js" />

define('dataservice.patient',
    ['jquery', 'db', 'Dexie', 'underscore'],
    function ($, db, Dexie, _) {

        var remoteService = (function () {
            var patientsUri = "/api/patientapi",

            getById = function (id,data) {
                return $.ajax({
                    type: 'GET',
                    url: patientsUri + '/?id=' + id,
                    dataType: 'json',
                    contentType: 'application/json',
                    data: data ? JSON.stringify(data) : null
                }).fail(function (jqXHR, textStatus, errorThrown) {

                });

            },

            addOrUpdate = function (id,data) {
                return $.ajax({
                    type: 'POST',
                    url: patientsUri,
                    dataType: 'json',
                    contentType: 'application/json',
                    data: data ? data: null
                }).fail(function (jqXHR, textStatus, errorThrown) {

                });
            },
             
            getData = function (uri, method, options) {                
                return $.ajax({
                    type: method,
                    url: uri || patientsUri,
                    dataType: 'json',
                    contentType: 'application/json',
                    data: options.data ? JSON.stringify(options.data) : null
                }).done(function (data) {
                    options.results.patientData = data;
                })
                    .fail(function (jqXHR, textStatus, errorThrown) {
                    
                });
            },

            getFacilities = function (uri, method, options) {
                return $.ajax({
                    type: method,
                    url: uri || patientsUri + '/getFacilities',
                    dataType: 'json',
                    contentType: 'application/json',
                    data: options && options.data ? JSON.stringify(options.data) : null
                }).done(function (result) {
                    options.results.facilities = result;
                }).fail(function (jqXHR, textStatus, errorThrown) {

                });
            },

            push = function (options) {
                return $.ajax({
                    type: "POST",
                    url: patientsUri + '/PostAll',
                    data: options && options.data ? { items: options.data } : null
                }).done(function (result) {
                    options.results.patientData = result;
                }).fail(function (jqXHR, textStatus, errorThrown) {
                    console.log(textStatus + " Error pushing Patient " + errorThrown);
                    console.log(jqXHR.responseText.Message);
                });
            };

            return {
                getData: getData,
                getById: getById,
                getFacilities: getFacilities,
                push: push,
                addOrUpdate: addOrUpdate
            };

        })(),

        localService = (function () {

            if (typeof String.prototype.startsWith != 'function') {
                // add implementation!
                String.prototype.startsWith = function (searchString, position) {
                    position = position || 0;
                    return this.substr(position, searchString.length) === searchString;
                };
            }

            var getData = function () {

                return $.Deferred(function (def) {
                    return db.patients.toArray().then(function (data) {
                       return Dexie.Promise.all(data.map(function(patient){
                           return getCustomAttributes(patient).then(function(attributes){
                                patient.customAttributes = attributes;
                                return patient;
                            });
                        })).then(function(patientCustomAtt){
                            def.resolve(patientCustomAtt);
                        });                        
                    }).catch(function (error) {
                        def.reject();
                    });
                }).promise();

            },

             pull = function () {
                 return $.Deferred(function (def) {
                     return db.patients.where('synchStatus').notEqual('synchronized').toArray(function (data) {
                         return Dexie.Promise.all(data.map(function (pat) {
                             return getCustomAttributes(pat).then(function (attributes) {
                                 pat.customAttributes = attributes;
                                 return pat;
                             });
                         })).then(function (patients) {
                             def.resolve(patients);
                         });
                     }).catch(function (error) {
                         def.reject();
                     });
                 }).promise();

             },

            removeById = function (id) {
                // TODO
            },

            getById = function (keyId, keyValue) {
                return $.Deferred(function (def) {
                    return db.patients.where(keyId).equals(parseInt(keyValue)).first(function (item) {
                        return getCustomAttributes(item).then(function (customAttributes) {
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
                    return db.patients.toCollection().count().then(function (count) {
                        if (count > 0)
                            db.patients.orderBy('PatientId').keys(function (Ids) {
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
                    return db.patients.put(item).then(function () {
                        def.resolve();
                    }).catch(function (error) {
                        def.reject();
                    });
                }).promise();

            },

            getByUniqueIdentifier = function (uniqueIdentifier) { // Guid Value

                return $.Deferred(function (def) {
                    return db.patients.get(uniqueIdentifier).then(function (item) {
                       return getCustomAttributes(item).then(function (customAttributes) {
                            item.customAttributes = customAttributes;
                            def.resolve(item);
                        });
                        
                    }).catch(function (error) {
                        def.reject();
                    });
                }).promise();                

            },

            getByUniqueId = function (uniqueIdentifier) { // PatientId Value

                return $.Deferred(function (def) {
                    return db.patients.where('PatientId').equals(parseInt(uniqueIdentifier)).first(function (item) {
                        return getCustomAttributes(item).then(function (customAttributes) {
                            item.customAttributes = customAttributes;
                            def.resolve(item);
                        });

                    }).catch(function (error) {
                        def.reject();
                    });
                }).promise();

            },

            getCustomAttributes = function (patient) {
                return new Dexie.Promise(function (resolve, reject) {
                    db.customAttributes.where('EntityName').equals('Patient').toArray(function (customAttributes) {
                        var objAttributes = patient.customAttributes;
                        if (objAttributes === undefined || objAttributes.length === 0) {
                            resolve(customAttributes.map(function (attribute) {
                                attribute.currentValue = "";
                                attribute.lastUpdated = "";
                                attribute.lastUpdatedUser = "";
                                return attribute;
                            }));
                        }
                        else {
                            var parsedCustomAttributes = customAttributes.map(function (attribute) {
                                var obj = _.findWhere(objAttributes, { CustomAttributeConfigId: attribute.CustomAttributeConfigId });
                                if (obj) {
                                    attribute.currentValue = obj.currentValue;
                                    attribute.lastUpdatedUser = obj.lastUpdatedUser;
                                    attribute.lastUpdated = obj.lastUpdated;
                                    if (obj.lastUpdated == '0001-01-01T00:00:00')
                                        attribute.lastUpdated = '';
                                }
                                else {
                                    attribute.currentValue = "";
                                    attribute.lastUpdated = "";
                                    attribute.lastUpdatedUser = "";
                                }

                                return attribute;
                            });

                            resolve(parsedCustomAttributes);
                        }
                    });
                });
            },

            searchByIds = function (criterias, useIdentifier) {

                var useIdentifier = typeof useIdentifier !== 'undefined' ? useIdentifier : false;
                var isFirstQsItem = true;
                var query;

                // if primary keys is defined using it to fetch the record
                if (useIdentifier)
                    return getByUniqueId(criterias[0].PatientId);

                // loop throught the criteria to create the query
                criterias.forEach(function (criteria) {
                    var criteriaKeys = Object.keys(criteria);                    

                    if (isFirstQsItem) {
                        if ($.isNumeric(criteria[criteriaKeys[0]]))
                            query = db.patients.where(criteriaKeys[0])
                                    .equals(criteria[criteriaKeys[0]]);
                        else
                            query = db.patients.where(criteriaKeys[0])
                            .startsWithIgnoreCase(criteria[criteriaKeys[0]]);

                        isFirstQsItem = false;
                    }
                    else {

                        if ($.isNumeric(criteria[criteriaKeys[0]]))
                            query = query.and(function (patient) {
                                return (patient[criteriaKeys[0]]) == criteria[criteriaKeys[0]];
                            });
                        else
                            query = query.and(function (patient) {
                                return (patient[criteriaKeys[0]]).toLowerCase()
                                    .startsWith((criteria[criteriaKeys[0]]).toLowerCase());
                            });
                    }
                });

                // execute the query
                if (query) {
                    return $.Deferred(function (def) {
                        return query.toArray().then(function (data) {
                            // load custom attributes
                            return Dexie.Promise.all(data.map(function (patient) {
                                return getCustomAttributes(patient).then(function (attributes) {
                                    patient.customAttributes = attributes;
                                    return patient;
                                });
                            })).then(function (patientCustomAtt) {
                                def.resolve(patientCustomAtt);
                            });
                        }).catch(function (error) {
                            def.reject();
                        });
                    }).promise();
                }
                else {
                    return getData();
                }
            },

            addFacilities = function (items) {
                return $.Deferred(function (def) {
                    return db.transaction('rw', db.facility, function () {

                        var numObjects = items.length;

                        for (var i = 0; i < numObjects; i++) {
                            var obj = items[i];
                            db.facility.put(obj);
                        }

                    }).then(function () {
                        def.resolve();

                    }).catch(function (err) {

                        console.log(err);
                        def.reject();
                    });

                }).promise();
            },

            getFacilities = function () {

                return $.Deferred(function (def) {
                    return db.facility.toArray().then(function (data) {
                        def.resolve(data);
                    }).catch(function (error) {
                        def.reject();
                    });
                }).promise();

            },

            addBatch = function (items) {
                return $.Deferred(function (def) {
                    db.transaction('rw', db.patients, function () {

                        var numObjects = items.length;                 

                        for (var i = 0; i < numObjects; i++) {
                            var patientObj = items[i];                            
                            db.patients.put(patientObj);
                        }

                    }).then(function () {
                        def.resolve();

                    }).catch(function (err) {

                        console.log(err);
                        def.reject();
                    });
               
                }).promise();
            };

            return {
                getData: getData,
                getById: getById,
                getFacilities: getFacilities,
                addFacilities: addFacilities,
                addOrUpdate: addOrUpdate,
                getByUniqueIdentifier: getByUniqueIdentifier,
                removeById: removeById,
                searchByIds: searchByIds,
                getSequenceId: getSequenceId,
                addBatch: addBatch,
                pull: pull
            }

        })();

        return {
            remoteService: remoteService,
            localService: localService
        };
    });
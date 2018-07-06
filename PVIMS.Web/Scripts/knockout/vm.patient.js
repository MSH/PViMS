/// <reference path="../_references.js" />

define('vm.patient',
    ['jquery', 'ko', 'moment', 'config', 'dataservice', 'model','model.mapper','uuid','utils'],
    function ($, ko, moment, config, dataservice, model, mapper, uuid, utils) {

        var
            // Properties

            mode = {
                viewMode: 'View',
                editMode: 'Edit'
            },

            patient = ko.observable(new model.Patient()),
            patients = ko.observableArray(),

            formNotes = ko.observable(),

            error = ko.observable(),
            saved = ko.observable(false), // use to show sucessfully message on view
            saveInvalid = ko.observable(false), // show invalid messages on view

            selectionData = ko.observableArray(),
            appointements = ko.observableArray(),
            encounters = ko.observableArray(),
            facilities = ko.observableArray(),
            todayEncounters = [],

            heading = ko.observable("Patient"),
            formMode = ko.observable(mode.viewMode),
            isExecuting = ko.observable(false),

            searchParamFacility = ko.observable(),
            searchParamUniqueId = ko.observable(),
            searchParamFirstname = ko.observable(),
            searchParamSurname = ko.observable(),

            validationErrors = ko.computed(function () {            
                var valArray = patient() ? ko.validation.group(patient())() : [];
                return valArray;
            })

            canEdit = ko.computed(function () {
                return true;
            }),

            isDirty = ko.computed(function () {
                return canEdit() ? patient().dirtyFlag().isDirty() : false;
            }),

            isValid = ko.computed(function () {
                console.log(canEdit());
                //return canEdit() ? validationErrors().length === 0 : true;
            });

        var patientsUri = "/api/patientapi",

        activate = function () {
            var data = {};
            var id = utils.getUrlVar('id');

            //if (utils.online()) {
            //    dataservice.customAttribute.remoteService.getSelectionData(undefined, 'GET', { results: data })
            //        .pipe(function () {
            //            return dataservice.encounterService.remoteService.getTodayEncounters(undefined, 'GET')
            //                .done(function (items) {
            //                    todayEncounters = items;
            //                });
            //        })                    
            //       .done(function () {
            //           selectionData(data.selectionDataItems);

            //           if (id)
            //               getPatientsById(id, 'PatientId');
            //           else
            //               getAllPatients();
            //       });
            //}
            //else {
                var results = [];
                dataservice.customAttribute.localService.getSelectionData()
                    .then(function (data) {
                        results = data
                        return dataservice.encounterService.localService.getTodayEncounters()
                            .done(function (items) {
                                todayEncounters = items;
                            });
                    })
                    .then(function () {
                        return dataservice.dataServicePatient.localService.getFacilities()
                            .done(function (items) {
                                var extendItems = [{ Id: 0, FacilityName: "All Facilities" }];
                                items.forEach(function (item) {
                                    extendItems.push(item);
                                });
                                facilities(extendItems);
                            });
                    })
                   .done(function () {
                       selectionData(results);

                       if (id)
                           getPatientsById(id, 'PatientId');
                       else
                           getAllPatients();
                   });
           // }
        },

        ajaxHelper = function (uri, method, data) {
            error('');
            return $.ajax({
                type: method,
                url: uri,
                dataType: 'json',
                contentType: 'application/json',
                data: data ? JSON.stringify(data) : null
            }).fail(function (jqXHR, textStatus, errorThrown) {
                error(errorThrown);
            });
        },

        getAllPatients = function () {

            //if (utils.online()) {
            //    //alert('gettting patients from server');
            //    ajaxHelper(patientsUri, 'GET').done(function (data) {
            //        patients(data);
            //    });
            //}
            //else {

                dataservice.dataServicePatient.localService.getData()
                    .done(function (data) {
                        patients(data);
                    });
            //}
        },

        filterAttributes = function (attributeName) {
            return selectionData().filter(function (selected) {
                return selected.AttributeKey === attributeName;
            });
        },

        getAttribute = function (attributeId, attributeName) {
            if (!attributeId)
                return "";
            var selected = ko.utils.arrayFirst(selectionData(), function (item) {
                return item.SelectionKey == attributeId && item.AttributeKey == attributeName;
            });

            if (selected)
                return selected.DataItemValue;
            else
                return "";
        },

        selectTab = function (tab) {
            console.log(tab);
            // ADD CODE HERE TO PERFORM EXTRA ACTIONS WHEN TAB IS SELECTED!
        },

        getPatientsById = function (value,patientKeyId) {

            //if (utils.online()) {
            //    dataservice.dataServicePatient.remoteService.getById(value)
            //        .done(function (pat) {
            //            var item = mapper.patient.fromDto(pat);
            //            patient(item);
            //    });
            //}
            //else {
                dataservice.dataServicePatient.localService.getById(patientKeyId, value)
                   .done(function (pat) {
                       var item = mapper.patient.fromDto(pat);
                       heading("Patient (" + pat.PatientFirstName + " " + pat.PatientLastName + ")");
                       patient(item);
                 });
            //}

        },

        cancel = function () {
            formMode(mode.viewMode);
            saveInvalid(false);
            //TODO: add cancel logic
        },

        edit = function () {
            if (canEdit())
            {
                formMode(mode.editMode);
                saved(false);
            }
        },

        newPatient = function () {
            formMode(mode.editMode);
        },

        parse = function (o) {
            var obj = {};
            obj.PatientUniqueIdentifier = patient().PatientUniqueIdentifier();
            obj.PatientId = patient().PatientId();
            obj.PatientFirstName = patient().PatientFirstName();
            obj.PatientLastName = patient().PatientLastName();
            obj.PatientDateOfBirth = patient().PatientDateOfBirth();
            obj.PatientMiddleName = patient().PatientMiddleName();
            obj.FacilityId = patient().FacilityId();
            obj.CreatedBy = patient().CreatedBy();
            obj.UpdatedBy = patient().UpdatedBy();
            obj.PatientCreatedDate = patient().PatientCreatedDate();
            obj.PatientUpdatedDate = patient().PatientUpdatedDate();
            obj.Notes = patient().Notes();
            obj.customAttributes = ko.utils.arrayMap(patient().customAttributes(), function (item) {
                var currentValue = item().currentValue(),
                    obj = item();
                obj['currentValue'] = currentValue
                return obj;
            });

            return obj;
        }

        save = function () {

            if (!isExecuting() && validationErrors().length === 0) {

                isExecuting(true);

                var plainPatient = parse(patient());

                plainPatient.synchStatus = "synchronization required";

                //if (utils.online())
                //    dataservice.dataServicePatient.remoteService.addOrUpdate(0, JSON.stringify(plainPatient))
                //        .done(function (pat) {
                //            var item = mapper.patient.fromDto(pat);
                //            patient(item);
                //            formMode(mode.viewMode);
                //            saved(true);
                //        })
                //        .always(function () { isExecuting(false); });
                //else { // offline

                    if (patient().PatientUniqueIdentifier()) {
                        plainPatient.PatientUpdatedDate = moment(new Date()).format("YYYY-MM-DDTHH:mm:ss");
                        plainPatient.UpdatedBy = localStorage.getItem("User") || "Offline";
                        dataservice.dataServicePatient.localService.addOrUpdate(plainPatient)
                        .done(function () {
                            patient(mapper.patient.fromDto(plainPatient));
                            formMode(mode.viewMode);
                            saved(true);
                            saveInvalid(false);
                        })
                         .always(function () { isExecuting(false); });
                    }
                    else // new patient
                    {
                        var nextId = 1;

                        dataservice.dataServicePatient.localService.getSequenceId()
                            .done(function (id) {
                                nextId = id;

                                plainPatient.PatientUniqueIdentifier = uuid.v4();
                                plainPatient.PatientId = nextId;
                                plainPatient.PatientCreatedDate = moment(new Date()).format("YYYY-MM-DDTHH:mm:ss");
                                plainPatient.CreatedBy = localStorage.getItem("User") || "Offline";
                                dataservice.dataServicePatient.localService.addOrUpdate(plainPatient)
                                    .done(function () {
                                        patient(mapper.patient.fromDto(plainPatient));
                                        formMode(mode.viewMode);
                                        saved(true);
                                        saveInvalid(false);
                                    })
                                    .always(function () { isExecuting(false); });
                            });
                    }
                //}

            }
            else if (!(validationErrors().length === 0)) {
                saveInvalid(true);
            }


        },

        searchPatients = function (searchform) {

            //if (utils.online()) {

            //    var queryString = '?';
            //    var isFirstQsItem = true;

            //    if (searchParamFacility()) {
            //        queryString += (isFirstQsItem ? '' : '&') + 'facilityId=' + searchParamFacility();
            //        isFirstQsItem = false;
            //    }

            //    if (searchParamUniqueId()) {
            //        queryString += (isFirstQsItem ? '' : '&') + 'puid=' + searchParamUniqueId();
            //        isFirstQsItem = false;
            //    }

            //    if (searchParamFirstname()) {
            //        queryString += (isFirstQsItem ? '' : '&') + 'firstName=' + searchParamFirstname();
            //        isFirstQsItem = false;
            //    }

            //    if (searchParamSurname()) {
            //        queryString += (isFirstQsItem ? '' : '&') + 'surname=' + searchParamSurname();
            //        isFirstQsItem = false;
            //    }

            //    ajaxHelper(patientsUri + '/search' + queryString, 'GET').done(function (data) {
            //        patients(data);
            //    });
            //}
            //else {
                var searchesCriteria = [],
                    searchObject = {};

                // If primary key is defined, lookup using only the primary key
                if (searchParamUniqueId()) {
                    searchObject = { PatientId: searchParamUniqueId() };
                    searchesCriteria.push(searchObject);

                    dataservice.dataServicePatient.localService.searchByIds(searchesCriteria, true)
                        .done(function (data) {
                            patients(data);
                        });

                }
                else {
                    if (searchParamFacility() && searchParamFacility() > 0) {
                        searchObject = { FacilityId: searchParamFacility() };
                        searchesCriteria.push(searchObject);
                    }
                    if (searchParamFirstname()) {
                        searchObject = { PatientFirstName: searchParamFirstname() };
                        searchesCriteria.push(searchObject);
                    }

                    if (searchParamSurname()) {
                        searchObject = { PatientLastName: searchParamSurname() };
                        searchesCriteria.push(searchObject);
                    }

                    dataservice.dataServicePatient.localService.searchByIds(searchesCriteria)
                        .done(function (data) {
                            patients(data);
                        });
                }

            //}
        },

        patientHasEncounterForToday = function (patientId) {
            var found = false;
            var encounter = ko.utils.arrayFirst(todayEncounters, function (item) {
                return item.PatientId == patientId;
            });

            if (encounter)
                found = true;
            return found;
        },

        getPatientEncounterForToday = function (patientId) {
            var encounter = ko.utils.arrayFirst(todayEncounters, function (item) {
                return item.PatientId == patientId;
            });

            if (encounter)
                return encounter.EncounterId;

            return 0;
        },

        tmplName = function () {
            return formMode() === mode.viewMode ? 'custom-view' : 'custom-edit';
        },

        formatDate = function (myDate, user) {
            return myDate && user ? "By " + user+ " on " + moment(myDate).format("YYYY-MM-DD") : "";
        },
        
        createURL = function (controllerName, actionName,id) {
           
            if (controllerName && actionName) {
                if (id)
                    return "/" + controllerName + "/" + actionName + "#id=" + id;
                else
                    return "/" + controllerName + "/" + actionName;
            }            
            return "";
        };

        // SUBSCRIBE EVENTS FOR TAB NOTES
        formNotes.subscribe(function (newValue) {
            patient().Notes(newValue);
        });

        patient.subscribe(function (newValue) {
            formNotes(newValue.Notes());
        });

        // END SUBSCRIBE EVENTS FOR TAB NOTES

        var initialCheck = function () {
            var upFunc = function () {
                activate();
                config.appIsOnline = true;
                unbind();
            }
            var downFunc = function () {
                activate();
                config.appIsOnline = false;
                unbind();
            }
            var unbind = function () {
                Offline.off('confirmed-up', upFunc);
                Offline.off('confirmed-down', downFunc);
            }

            Offline.on('confirmed-up', upFunc);
            Offline.on('confirmed-down', downFunc);

            Offline.check();
        };

        initialCheck();
        //activate();

        // Extend kockout binding handlers to handle Thirdy Party Libray - CKEDITOR
        ko.bindingHandlers.ckeditor = {
            init: function (element, valueAccessor) {

                var modelValue = valueAccessor();
                var value = ko.utils.unwrapObservable(valueAccessor());
                var element$ = $(element);

                // Set initial value and create the CKEditor
                $(element).html(value);
                $(element).ckeditor();
                var editor = $(element).ckeditorGet();

                // bind to change events and link it to the observable
                editor.on('blur', function (e) {
                    var self = this; debugger;
                    if (ko.isWriteableObservable(self)) {
                        self($(e.listenerData).val());
                    }
                }, modelValue, element);


                /* Handle disposal if KO removes an editor 
                 * through template binding */
                ko.utils.domNodeDisposal.addDisposeCallback(element,
                    function () {
                        if (editor) {
                            CKEDITOR.remove(editor);
                        };
                    });
            },

            /* Hook and handle the binding updating so we write 
             * back to the observable */
            update: function (element, valueAccessor, allBindingsAccessor) {
                var element$ = $(element);
                var newValue = ko.utils.unwrapObservable(valueAccessor());
                if (element$.ckeditorGet().getData() != newValue) {
                    element$.ckeditorGet().setData(newValue);
                }

                var allBindings = allBindingsAccessor(),
                enable = (allBindings.enable !== undefined) ? ko.utils.unwrapObservable(allBindings.enable) : true;

                if (enable) {
                    element$.ckeditorGet().setReadOnly(false);
                }
                else {
                    element$.ckeditorGet().setReadOnly(true);
                }
            }
        };

        // subscribe for heading change
        heading.subscribe(function (newValue) {
            var element = $('#breadcrumb-heading');
            element.text(newValue);

            element = $('#title-heading');
            element.text(newValue);
        });


        return {
            patient: patient,
            patients: patients,
            error: error,
            formMode: formMode,
            isValid: isValid,
            saved: saved,
            saveInvalid: saveInvalid,
            getAllPatients: getAllPatients,
            facilities: facilities,
            filterAttributes: filterAttributes,
            getAttribute: getAttribute,
            searchPatients: searchPatients,
            searchParamFacility: searchParamFacility,
            searchParamUniqueId: searchParamUniqueId,
            searchParamFirstname: searchParamFirstname,
            searchParamSurname: searchParamSurname,
            selectTab: selectTab,
            selectionData: selectionData,
            appointements: appointements,
            encounters: encounters,
            cancel: cancel,
            edit: edit,
            newPatient: newPatient,
            save: save,
            tmplName: tmplName,
            formatDate: formatDate,
            formNotes: formNotes,
            createURL: createURL,
            createURLParams: utils.createURL,
            getPatientEncounterForToday: getPatientEncounterForToday,
            patientHasEncounterForToday: patientHasEncounterForToday
        };
    });




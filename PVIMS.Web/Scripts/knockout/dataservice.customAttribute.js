/// <reference path="../_references.js" />

define('dataservice.customAttribute',
    ['jquery', 'db'],
    function ($, db) {

        var remoteService = (function () {
            var customAttributeUri = "/api/CustomAttributeConfigApi",
                
            getCustomAttributes = function (uri, method, options) {
                return $.ajax({
                    type: method,
                    url: uri || customAttributeUri,
                    dataType: 'json',
                    contentType: 'application/json',
                    data: options.data ? JSON.stringify(options.data) : null
                }).done(function (result) {
                    options.results.customAttributes = result;
                })
                    .fail(function (jqXHR, textStatus, errorThrown) {

                });
            },

            getSelectionData = function (uri, method, options) {
                return $.ajax({
                    type: method,
                    url: uri || customAttributeUri + '/selectiondata',
                    dataType: 'json',
                    contentType: 'application/json',
                    data: options && options.data ? JSON.stringify(options.data) : null
                }).done(function (result) {
                    options.results.selectionDataItems = result;
                }).fail(function (jqXHR, textStatus, errorThrown) {

                });
            };

            return {
                getCustomAttributes: getCustomAttributes,
                getSelectionData: getSelectionData
            };

        })(),

        localService = (function () {

            var getCustomAttributes = function () {

                return $.Deferred(function (def) {
                    return db.customAttributes.toArray().then(function (data) {
                        def.resolve(data);
                    }).catch(function (error) {
                        def.reject();
                    });
                }).promise();

            },

            getSelectionData = function () {

                return $.Deferred(function (def) {
                    return db.selectionDataItem.toArray().then(function (data) {
                        def.resolve(data);
                    }).catch(function (error) {
                        def.reject();
                    });
                }).promise();

            },

            getSelectionDataById = function (keyId, keyValue) {
                return $.Deferred(function (def) {
                    return db.selectionDataItem.where(keyId).equals(keyValue).first(function (item) {
                        def.resolve(item);
                    }).catch(function (error) {
                        def.reject();
                    });
                }).promise();

            },

            addBatchcustomAttributes = function (items) {
                return $.Deferred(function (def) {
                    db.transaction('rw', db.customAttributes, function () {

                        var numObjects = items.length;

                        for (var i = 0; i < numObjects; i++) {
                            var obj = items[i];
                            db.customAttributes.put(obj);
                        }

                    }).then(function () {
                        def.resolve();

                    }).catch(function (err) {

                        console.log(err);
                        def.reject();
                    });

                }).promise();
            },

            addBatchSelectionDataItem = function (items) {
                return $.Deferred(function (def) {
                    db.transaction('rw', db.selectionDataItem, function () {

                        var numObjects = items.length;

                        for (var i = 0; i < numObjects; i++) {
                            var obj = items[i];
                            db.selectionDataItem.put(obj);
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
                getSelectionData: getSelectionData,
                getSelectionDataById: getSelectionDataById,
                getCustomAttributes: getCustomAttributes,
                addBatchcustomAttributes: addBatchcustomAttributes,
                addBatchSelectionDataItem: addBatchSelectionDataItem
            }

        })();

        return {
            remoteService: remoteService,
            localService: localService,
        };
    });
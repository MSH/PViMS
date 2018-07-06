define('dataservice.common',
    ['jquery', 'ko', 'Dexie', 'db', 'underscore'],
    function (jquery, ko, Dexie, db, _) {

        var

        getCustomAttributes = function (obj, entity) {
            return new Dexie.Promise(function (resolve, reject) {
                db.customAttributes.where('EntityName').equals(entity).toArray(function (customAttributes) {
                    var objAttributes = obj.customAttributes;
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

        getSequenceId = function (table, key) {

            return $.Deferred(function (def) {
                return db[table].toCollection().count().then(function (count) {
                    if (count > 0)
                        db[table].orderBy(key).keys(function (Ids) {
                            var nextId = Ids[Ids.length - 1] + 1;
                            def.resolve(nextId);
                        });
                    else
                        def.resolve(1);
                }).catch(function (error) {
                    def.reject();
                });
            }).promise();

        };

        return {
            getCustomAttributes: getCustomAttributes,
            getSequenceId: getSequenceId
        };
    });
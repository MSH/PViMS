define('utils',
['jquery', 'moment','config'],
    function ($, moment, config) {

        if (typeof String.prototype.startsWith != 'function') {
            // add implementation!
            String.prototype.startsWith = function (searchString, position) {
                position = position || 0;
                return this.substr(position, searchString.length) === searchString;
            };
        }

        var
            getUrlVars = function () {
                var vars = [], hash;
                var hashes = window.location.href.slice(window.location.href.indexOf('#') + 1).split('&');
                for (var i = 0; i < hashes.length; i++) {
                    hash = hashes[i].split('=');
                    vars.push(hash[0]);
                    vars[hash[0]] = hash[1];
                }
                return vars;
            },

        getUrlVar = function (name) {
            return getUrlVars()[name];
        };

        var online = function () {            
            return config.appIsOnline;
        };

        

        var setCustomValidationRules = function (obj, rules) {
            if (rules['Required'])
                obj.extend({ required: true });

            if (rules['StringMaxLength'])
                obj.extend({ maxLength: rules['StringMaxLength'] });

            if (rules['NumericMinValue'])
                obj.extend({ number: true })
                    .extend({ min: rules['NumericMinValue'] });

            if (rules['NumericMaxValue'])
                obj.extend({ number: true })
                    .extend({ max: rules['NumericMaxValue'] });

            if(rules['FutureDateOnly'])
                obj.extend({ date: true })
                    .extend({
                    validation: {
                        validator: function (val, attr) {
                            if (val === undefined || val === null || val === "")
                                return true;
                            return moment(val).isAfter();
                        },
                        message: '{0} may only be in the future',
                        params: rules['AttributeName']
                    }
                });

            if (rules['PastDateOnly'])
                obj.extend({ date: true })
                    .extend({
                    validation: {
                        validator: function (val, attr) {
                            if (val === undefined || val === null || val === "")
                                return true;
                            return moment(val).isBefore();
                        },
                        message: '{0} may only be in the past',
                        params: rules['AttributeName']
                    }
                    });

            return obj;

        };

        var createURL = function (actionName, controllerName, parameters) {
            var url = "",
                countParameters = 0;
            if (actionName && controllerName) {
                if (parameters) {
                    url = "/" + controllerName + "/" + actionName;

                    for (var key in parameters) {
                        if (parameters.hasOwnProperty(key)) {

                            if (countParameters > 0)
                                url = url + "&" + key + "=" + parameters[key];
                            else
                                url = url + "#" + key + "=" + parameters[key];

                            countParameters++;
                        }
                    }
                }
                else
                    url = "/" + controllerName + "/" + actionName;
            }
            return url;
        };

        return {
            setCustomValidationRules: setCustomValidationRules,
            getUrlVar: getUrlVar,
            createURL: createURL,
            online: online
        };

});
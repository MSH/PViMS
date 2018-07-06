(function () {
    var root = this;

    define3rdPartyModules();
    loadPluginsAndBoot();

    function define3rdPartyModules() {
        // These are already loaded via bundles. 
        // We define them and put them in the root object.
        define('jquery', [], function () { return root.jQuery; });
        define('ko', [], function () { return root.ko; });
        define('moment', [], function () { return root.moment; });
        define('uuid', [], function () { return root.uuid; });
        define('underscore', [], function () { return root._; });
    }
    
    function loadPluginsAndBoot() {
        // Load additional required plugin if exists
        // and bootstrap de app
        require.config({
            baseUrl: '/Scripts/knockout',
            waitSeconds: 200,
            paths: {
                "Dexie": "/Scripts/Dexie",
                "statusController": "/Scripts/synchronisation",
                //jqueryValidate: "/Scripts/jquery.validate.min",
                //jqueryValidateUnobtrusive: "/Scripts/jquery.validate.unobtrusive",
                notify: "/Scripts/notify.min"
            },
            shim: {
                //jqueryValidate: ["jquery"],
                //jqueryValidateUnobtrusive: ["jquery", "jqueryValidate"],
                notify: ["jquery"]
                //"Dexie": {
                //    deps: [],
                //    exports: "Dexie"
                //}
            }
        });

        requirejs([
                'Dexie',
                'statusController',
                //'jqueryValidate',
                //'jqueryValidateUnobtrusive',
                'notify'
        ], boot);
    }
    
    function boot() {
        require(['bootstrapper'], function (bs) { bs.run(); });
    }
})();
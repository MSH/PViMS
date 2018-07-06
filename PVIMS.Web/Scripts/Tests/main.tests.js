(function () {
    // Establish the root object, `window` in the browser, or `global` on the server.
    var root = this;

    
     // Load the 3rd party libraries
    registerNonAmdLibs();
    // Load our app/custom plug-ins and bootstrap the app
    loadExtensionsAndBoot();

    function registerNonAmdLibs() {
        // Load the 3rd party libraries that the app needs.
        // These are in the bundle (BundleConfig.cs).
        // These are the core libraries that many others depend on.
        define('jquery', [], function () { return root.jQuery; });
        define('ko', [], function () { return root.ko; });
        define('Dexie', [], function () { return root.Dexie; });
        define('underscore', [], function () { return root._; });

    }
    
    // Load our app/custom plug-ins and bootstrap the app
    function loadExtensionsAndBoot() {

    }
    
    function boot() {
        // no op
    }
    
})();
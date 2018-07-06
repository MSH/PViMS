define('bootstrapper',
    ['jquery', 'config','binder','synch'], 
    function ($, config, binder,synch) {
        var
            run = function () {               

                $.when(true)
                    .done(binder.bind)
                    .done(synch.runSynchData);
            };

        return {
            run: run
        };
    });
define('binder',
    ['jquery', 'ko', 'config', 'vm'],
    function ($, ko, config, vm) {
        var
            ids = config.viewIds,

            bind = function () {
                if (getView(ids.patient))
                    ko.applyBindings(vm.patient, getView(ids.patient));

                if (getView(ids.patientDetails))
                    ko.applyBindings(vm.patient, getView(ids.patientDetails));

                if (getView(ids.patientSearch))
                    ko.applyBindings(vm.patient, getView(ids.patientSearch));

                if (getView(ids.encounterSearch))
                    ko.applyBindings(vm.encounter, getView(ids.encounterSearch));

                if (getView(ids.encounterView))
                {
                    vm.encounter.activate();
                    ko.applyBindings(vm.encounter, getView(ids.encounterView));
                }

                if (getView(ids.addEncounter))
                {
                    vm.addEncounter.activate();
                    ko.applyBindings(vm.addEncounter, getView(ids.addEncounter));
                }                    

                if (getView(ids.patientLabTest))
                    ko.applyBindings(vm.patientLabTest, getView(ids.patientLabTest));

                if (getView(ids.patientClinicalEvent))
                    ko.applyBindings(vm.patientClinicalEvent, getView(ids.patientClinicalEvent));

                if (getView(ids.patientMedication))
                    ko.applyBindings(vm.patientMedication, getView(ids.patientMedication));

                if (getView(ids.patientCondition))
                {
                    vm.patientCondition.activate();
                    ko.applyBindings(vm.patientCondition, getView(ids.patientCondition));
                }

                if (getView(config.statusMessagesIds.idleStatusIndicator))
                {
                    // clean up previous bind if exists... Multiple times binding causes error!
                    ko.cleanNode(getView(config.statusMessagesIds.idleStatusIndicator));

                    ko.applyBindings(config, getView(config.statusMessagesIds.idleStatusIndicator));
                }
               
            },
            
            getView = function (viewName) {
                return $(viewName).get(0);
            };
            
        return {
            bind: bind
        };
});
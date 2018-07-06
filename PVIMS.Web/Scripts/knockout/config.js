define('config',
    ['ko'],
    function (ko) {

        var
            // properties
            //-----------------

            currentUserId = 1,
            currentUser = ko.observable(),

            idleStatusIndicator = ko.observable('Idle'),

            synchIntervals = 10 * 60 * 1000, // 10min

            appIsOnline = false,

            viewIds = {
                patient: '#patient-view',
                patientDetails: "#patient-details",
                patientSearch: "#patient-searchresults",
                encounterSearch: "#encounter-searchresults",
                encounters: '#encounter-view',
                addEncounter: '#addEncounter-view',
                patientSearchResults: '#patient-searchresults',
                encounterView: '#view-encounter',
                patientLabTest: '#patientLabTest-edit',
                patientClinicalEvent: "#view-patientClinicalEvent",
                patientMedication: "#patientMedication-view",
                patientCondition: "#patientCondition-view"
            },

            statusMessagesIds = {
                idleStatusIndicator: '#synchronisationstatus'
            },

            indicatorMessages = {
                synch: 'Synchronising',
                synchDone: 'Synchronize Complete',
                idle: 'Idle'
            },

            validationInit = function () {
                ko.validation.init({
                    registerExtenders: true,    
                    messagesOnModified: true,   
                    insertMessages: true,       
                    parseInputAttributes: true, 
                    writeInputAttributes: true, 
                    messageTemplate: null,      
                    decorateInputElement: true,
                    //errorElementClass: 'errorFill',
                    grouping: { deep: true, observable: true, live: false }
                });
            },

        dataBaseName = "dcatoffline",

        changeIdleStatusIndicator = function (message) {
            idleStatusIndicator(message);
        };

        validationInit();

        Offline.options = { checkOnLoad: false, interceptRequests: false, requests: false, checks: { image: { url: 'https://www.google.com/images/srpr/logo11w.png' }, active: 'image' } };
            

        return {
            currentUserId: currentUserId,
            currentUser: currentUser,
            idleStatusIndicator: idleStatusIndicator,
            dataBaseName: dataBaseName,
            viewIds: viewIds,
            synchIntervals: synchIntervals,
            indicatorMessages: indicatorMessages,
            statusMessagesIds: statusMessagesIds,
            changeIdleStatusIndicator: changeIdleStatusIndicator,
            appIsOnline: appIsOnline,
            window: window
        };
    });

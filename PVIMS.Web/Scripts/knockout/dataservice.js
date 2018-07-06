define('dataservice',
    [
        'dataservice.patient',
        'dataservice.customAttribute',
        'dataservice.encounter',
        'dataservice.patientLabTest',
        'dataservice.patientClinicalEvent',
        'dataservice.patientMedication',
        'dataservice.patientCondition'
    ],
    function (dataservicepatient, customAttribute, encounterService, patientLabTest,
                patientClinicalEvent, patientMedication, patientCondition) {
        return {
            dataServicePatient: dataservicepatient,
            customAttribute: customAttribute,
            encounterService: encounterService,
            patientLabTest: patientLabTest,
            patientClinicalEvent: patientClinicalEvent,
            patientMedication: patientMedication,
            patientCondition: patientCondition
        };
    });
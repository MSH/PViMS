define('model',
    [
        'model.patient',
        'model.patientLabTest',
        'model.patientClinicalEvent',
        'model.patientMedication',
        'model.patientCondition',
        'model.encounter'
    ],
    function (patient, patientLabTest, patientClinicalEvent
        , patientMedication, patientCondition, encounter) {
        var
            model = {
                Patient: patient,
                PatientLabTest: patientLabTest,
                PatientClinicalEvent: patientClinicalEvent,
                PatientMedication: patientMedication,
                PatientCondition: patientCondition,
                Encounter: encounter
            };

         return model;
    });
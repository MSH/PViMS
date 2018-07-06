define('vm',
[
        'vm.patient',
        'vm.encounter',
        'vm.patientLabTest',
        'vm.patientClinicalEvent',
        'vm.patientMedication',
        'vm.patientCondition',
        'vm.addEncounter'

],
    function (patient, encounter, labTest, clinicalEvent, patientMedication, patientCondition, addEncounter) {
        return {
            patient: patient,
            encounter: encounter,
            patientLabTest: labTest,
            patientClinicalEvent: clinicalEvent,
            patientMedication: patientMedication,
            patientCondition: patientCondition,
            addEncounter: addEncounter
          
    };
});
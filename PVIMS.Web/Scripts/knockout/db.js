/// <reference path="../_references.js" />

define('db', ['Dexie', 'config'], function (Dexie, config) {

    // Declare Dexie instance
    var db = new Dexie(config.dataBaseName);

    // Define database schema

    //schema for version 1
    db.version(1).stores({
        patients: "PatientUniqueIdentifier,PatientFirstName,PatientLastName"
    });

    //schema for version 2
    db.version(2).stores({
        patients: "PatientUniqueIdentifier,PatientId,PatientFirstName,PatientLastName"
    });

    //schema for version 3
    db.version(3).stores({
        customAttributes: "CustomAttributeConfigId,EntityName",
        selectionDataItem: "SelectionDataItemId,AttributeKey"
    });

    //schema for version 4
    db.version(4).stores({
        encounters: "EncounterIdentifier,EncounterId,EncounterDate,PatientId"
    });

    //schema for version 5
    db.version(5).stores({
        patientLabTest: "PatientLabTestIdentifier,PatientLabTestId,PatientId"
    });

    //schema for version 6
    db.version(6).stores({
        labTestType: "Value"
    });

    //schema for version 7
    db.version(7).stores({
        patientClinicalEvent: "PatientClinicalEventIdentifier,PatientClinicalEventId,EncounterId,PatientId"
    });

    //schema for version 8
    db.version(8).stores({
        patientMedication: "PatientMedicationIdentifier,PatientMedicationId,PatientId",
        medication: "MedicationId, DrugName, Active"
    });

    //schema for version 9
    db.version(9).stores({
        patientCondition: "PatientConditionIdentifier,PatientConditionId,PatientId",
        condition: "ConditionId"
    });

    // schema for version 10 -- add synchronization verification column
    db.version(10).stores({
        patientCondition: "PatientConditionIdentifier,PatientConditionId,PatientId,synchStatus",
        patientMedication: "PatientMedicationIdentifier,PatientMedicationId,PatientId,synchStatus",
        patientClinicalEvent: "PatientClinicalEventIdentifier,PatientClinicalEventId,EncounterId,PatientId,synchStatus",
        patientLabTest: "PatientLabTestIdentifier,PatientLabTestId,PatientId,synchStatus",
        encounters: "EncounterIdentifier,EncounterId,EncounterDate,PatientId,synchStatus",
        patients: "PatientUniqueIdentifier,PatientId,PatientFirstName,PatientLastName,synchStatus"
    });


    //schema for version 11
    db.version(11).stores({
        encounterType: "Id",
        priority: "Id"
    });

    //schema for version 12
    db.version(12).stores({
        labTestUnit: "Id",
        terminologyMedDra: "Id,MedDraCode,MedDraTermType,Description"
    });

     //schema for version 13
    db.version(13).stores({
        facility: "Id, FacilityCode, FacilityType"
    });

    //schema for version 14 - index FacilityId
    db.version(14).stores({
        patients: "PatientUniqueIdentifier,PatientId,PatientFirstName,PatientLastName,synchStatus,FacilityId"
    });

    // Populate ground data
    db.on('populate', function () {

        // Populate patients
        //db.patients.add({ PatientUniqueIdentifier: "28212EF5-779D-494E-9558-1C40E6B0846A", PatientFirstName: "Siphokuhle", PatientLastName: "Ntshingila", PatientDateOfBirth: "1993-10-02" });
        //db.patients.add({ PatientUniqueIdentifier: "9D1A6775-88F7-428E-A1BE-DBB437960D08", PatientFirstName: "Sindisiwe", PatientLastName: "Sikhakhane", PatientDateOfBirth: "1987-09-13" });
        //db.patients.add({ PatientUniqueIdentifier: "20097D35-A71A-414C-B734-319069FDDC69", PatientFirstName: "Nozipho", PatientLastName: "Gumede", PatientDateOfBirth: "1988-03-22" });

    });

    db.on('blocked', function () {
        console.log('database blocked');
    });

    Dexie.Promise.on('error', function (err) {
        // Log to console or show en error indicator somewhere in your GUI...
        console.log("Uncaught error: " + err);
    });

    // Open database
    db.open();

    return db;
});
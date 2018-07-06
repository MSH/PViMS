/// <reference path="../_references.js" />

define('model.mapper',
['ko', 'model','utils', 'moment'],
    function (ko,model, utils, moment) {
        var

            patient = {
                getDtoId: function (dto) { return dto.id; },
                fromDto: function (dto, item) {
                    item = item || new model.Patient().PatientId(dto.PatientId);
                    item.PatientFirstName(dto.PatientFirstName)
                        .PatientLastName(dto.PatientLastName)
                        .PatientDateOfBirth(dto.PatientDateOfBirth)
                        .PatientUniqueIdentifier(dto.PatientUniqueIdentifier)
                        .PatientCreatedDate(dto.PatientCreatedDate)
                        .PatientUpdatedDate(dto.PatientUpdatedDate)
                        .PatientMiddleName(dto.PatientMiddleName)
                        .FacilityId(dto.FacilityId)
                        .CreatedBy(dto.CreatedBy)
                        .UpdatedBy(dto.UpdatedBy)
                        .Notes(dto.Notes)
                        .customAttributes(ko.utils.arrayMap(dto.customAttributes, function (item) {
                            var currentValue = ko.observable(item['currentValue']);
                            currentValue = utils.setCustomValidationRules(currentValue, item);                            
                            item['currentValue'] = currentValue;
                            return ko.observable(item);
                        }));
                    return item;
                }
            },

            encounter = {
                getDtoId: function (dto) { return dto.id; },
                fromDto: function (dto, item) {
                    item = item || new model.Encounter().EncounterId(dto.EncounterId);
                    item.EncounterIdentifier(dto.EncounterIdentifier)
                        .PatientId(dto.PatientId)
                        .EncounterType(dto.EncounterType)
                        .EncounterDate(dto.EncounterDate)
                        .EncounterPriority(dto.EncounterPriority)
                        .EncounterCreatedDate(dto.EncounterCreatedDate)
                        .EncounterUpdatedDate(dto.EncounterUpdatedDate)
                        .CreatedBy(dto.CreatedBy)
                        .UpdatedBy(dto.UpdatedBy)
                        .Notes(dto.Notes)
                        .customAttributes(ko.utils.arrayMap(dto.customAttributes, function (item) {
                            var currentValue = ko.observable(item['currentValue']);
                            currentValue = utils.setCustomValidationRules(currentValue, item);
                            item['currentValue'] = currentValue;
                            return ko.observable(item);
                        }));
                    return item;
                },
                toDto: function (item) {
                    var obj = {};
                    obj.EncounterId = item().EncounterId();
                    obj.PatientId = item().PatientId();
                    obj.EncounterIdentifier = item().EncounterIdentifier();
                    obj.EncounterDate = moment(item().EncounterDate()).format("YYYY-MM-DD") + "T00:00:00";
                    obj.EncounterType = item().EncounterType();
                    obj.EncounterPriority = item().EncounterPriority();
                    obj.EncounterUpdatedDate = item().EncounterUpdatedDate();
                    obj.EncounterCreatedDate = item().EncounterCreatedDate();
                    obj.CreatedBy = item().CreatedBy();
                    obj.UpdatedBy = item().UpdatedBy();
                    obj.Notes = item().Notes();
                    obj.customAttributes = ko.utils.arrayMap(item().customAttributes(), function (ct) {
                        var currentValue = ct().currentValue(),
                            obj = ct();
                        obj['currentValue'] = currentValue;
                        return obj;
                    });

                    return obj;
                }
            },

            patientLabTest = {
                getDtoId: function (dto) { return dto.id; },
                fromDto: function (dto, item) {
                    item = item || new model.PatientLabTest().PatientLabTestId(dto.PatientLabTestId);
                    item.PatientLabTestIdentifier(dto.PatientLabTestIdentifier)
                        .PatientId(dto.PatientId)
                        .TestName(dto.TestName)
                        .TestDate(dto.TestDate)
                        .TestResult(dto.TestResult)
                        .TestUnit(dto.TestUnit)
                        .unitObject(dto.unitObject || {})
                        .LabValue(dto.LabValue)
                        .customAttributes(ko.utils.arrayMap(dto.customAttributes, function (item) {
                            var currentValue = ko.observable(item['currentValue']);
                            currentValue = utils.setCustomValidationRules(currentValue, item);                            
                            item['currentValue'] = currentValue;
                            return ko.observable(item);
                        }));

                    return item;
                },
                toDto: function (item) {
                    var obj = {};
                    obj.PatientLabTestIdentifier = item().PatientLabTestIdentifier();
                    obj.PatientLabTestId = item().PatientLabTestId();
                    obj.PatientId = item().PatientId();
                    obj.TestName = item().TestName();
                    obj.TestDate = item().TestDate();
                    obj.TestResult = item().TestResult();
                    obj.TestUnit = item().TestUnit();
                    obj.LabValue = item().LabValue();
                    obj.customAttributes = ko.utils.arrayMap(item().customAttributes(), function (ct) {
                        var currentValue = ct().currentValue(),
                            obj = ct();
                        obj['currentValue'] = currentValue
                        return obj;
                    });

                    return obj;
                }
            },

            patientClinicalEvent = {
                getDtoId: function (dto) { return dto.id; },
                fromDto: function (dto, item) {
                    item = item || new model.PatientClinicalEvent().PatientClinicalEventId(dto.PatientClinicalEventId);
                    item.PatientClinicalEventIdentifier(dto.PatientClinicalEventIdentifier)
                        .PatientId(dto.PatientId)
                        .EncounterId(dto.EncounterId)
                        .medDra(dto.medDra)
                        .Description(dto.Description)
                        .OnsetDate(dto.OnsetDate)
                        .ResolutionDate(dto.ResolutionDate)
                        .ReportedDate(dto.ReportedDate)
                        .MedDraId(dto.MedDraId)
                        .customAttributes(ko.utils.arrayMap(dto.customAttributes, function (item) {
                            var currentValue = ko.observable(item['currentValue']);
                            currentValue = utils.setCustomValidationRules(currentValue, item);
                            item['currentValue'] = currentValue;
                            return ko.observable(item);
                        }));

                    return item;
                },
                toDto: function (item) {
                    var obj = {};
                    obj.PatientClinicalEventIdentifier = item().PatientClinicalEventIdentifier();
                    obj.PatientClinicalEventId = item().PatientClinicalEventId();
                    obj.PatientId = item().PatientId();
                    obj.EncounterId = item().EncounterId();
                    obj.OnsetDate = item().OnsetDate();
                    obj.ResolutionDate = item().ResolutionDate();
                    obj.ReportedDate = item().ReportedDate();
                    obj.Description = item().Description();
                    obj.MedDraId = item().MedDraId();
                    obj.customAttributes = ko.utils.arrayMap(item().customAttributes(), function (ct) {
                        var currentValue = ct().currentValue(),
                            obj = ct();
                        obj['currentValue'] = currentValue
                        return obj;
                    });

                    return obj;
                }
            },
            
            patientMedication = {

                getDtoId: function (dto) { return dto.id; },
                fromDto: function (dto, item) {
                    item = item || new model.PatientMedication().PatientMedicationId(dto.PatientMedicationId);
                    item.PatientMedicationIdentifier(dto.PatientMedicationIdentifier)
                        .PatientId(dto.PatientId)
                        .MedicationId(dto.MedicationId)
                        .StartDate(dto.StartDate)
                        .EndDate(dto.EndDate)
                        .Dose(dto.Dose)
                        .DoseFrequency(dto.DoseFrequency)
                        .DoseUnit(dto.DoseUnit)
                        .customAttributes(ko.utils.arrayMap(dto.customAttributes, function (item) {
                            var currentValue = ko.observable(item['currentValue']);
                            currentValue = utils.setCustomValidationRules(currentValue, item);
                            item['currentValue'] = currentValue;
                            return ko.observable(item);
                        }));

                    return item;
                },
                toDto: function (item) {
                    var obj = {};
                    obj.PatientMedicationIdentifier = item().PatientMedicationIdentifier();
                    obj.PatientMedicationId = item().PatientMedicationId();
                    obj.PatientId = item().PatientId();
                    obj.MedicationId = item().MedicationId();
                    obj.StartDate = item().StartDate();
                    obj.EndDate = item().EndDate();
                    obj.Dose = item().Dose();
                    obj.DoseFrequency = item().DoseFrequency();
                    obj.DoseUnit = item().DoseUnit();
                    obj.customAttributes = ko.utils.arrayMap(item().customAttributes(), function (ct) {
                        var currentValue = ct().currentValue(),
                            obj = ct();
                        obj['currentValue'] = currentValue
                        return obj;
                    });

                    return obj;
                }
            };

        patientCondition = {

            getDtoId: function (dto) { return dto.id; },
            fromDto: function (dto, item) {
                item = item || new model.PatientCondition().PatientConditionId(dto.PatientConditionId);
                item.PatientConditionIdentifier(dto.PatientConditionIdentifier)
                    .PatientId(dto.PatientId)
                    .MedDraId(dto.MedDraId)
                    .medDra(dto.medDra)
                    //.ConditionId(dto.ConditionId)
                    .StartDate(dto.StartDate)
                    .EndDate(dto.EndDate)
                    .TreatmentStartDate(dto.TreatmentStartDate)
                    .Comments(dto.Comments)
                    .customAttributes(ko.utils.arrayMap(dto.customAttributes, function (item) {
                        var currentValue = ko.observable(item['currentValue']);
                        currentValue = utils.setCustomValidationRules(currentValue, item);
                        item['currentValue'] = currentValue;
                        return ko.observable(item);
                    }));

                return item;
            },
            toDto: function (item) {
                var obj = {};
                obj.PatientConditionIdentifier = item().PatientConditionIdentifier();
                obj.PatientConditionId = item().PatientConditionId();
                obj.PatientId = item().PatientId();
                obj.MedDraId = item().MedDraId();
                //obj.ConditionId = item().ConditionId();
                obj.StartDate = item().StartDate();
                obj.EndDate = item().EndDate();
                obj.TreatmentStartDate = item().TreatmentStartDate();
                obj.Comments = item().Comments();
                obj.customAttributes = ko.utils.arrayMap(item().customAttributes(), function (ct) {
                    var currentValue = ct().currentValue(),
                        obj = ct();
                    obj['currentValue'] = currentValue
                    return obj;
                });

                return obj;
            }
        };

        return {
            patient: patient,
            encounter: encounter,
            patientLabTest: patientLabTest,
            patientClinicalEvent: patientClinicalEvent,
            patientMedication: patientMedication,
            patientCondition: patientCondition
        };
    });
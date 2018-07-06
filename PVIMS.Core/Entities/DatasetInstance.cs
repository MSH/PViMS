using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Reflection;

using VPS.CustomAttributes;

using PVIMS.Core.ValueTypes;
using PVIMS.Core.Exceptions;

namespace PVIMS.Core.Entities
{
    public class DatasetInstance : AuditedEntityBase
    {
        public DatasetInstance()
        {
            DatasetInstanceGuid = Guid.NewGuid();
            Status = DatasetInstanceStatus.INCOMPLETE;

            DatasetInstanceValues = new HashSet<DatasetInstanceValue>();
        }

        [Required]
        public virtual Dataset Dataset { get; set; }
        public Guid DatasetInstanceGuid { get; set; }

        [Required]
        public int ContextID { get; set; }

        public string Tag { get; set; }

        public virtual EncounterTypeWorkPlan EncounterTypeWorkPlan { get; set; }
        public virtual DatasetInstanceStatus Status { get; set; }

        public virtual ICollection<DatasetInstanceValue> DatasetInstanceValues { get; set; }

        public void InitialiseValues(string tag, DatasetInstance sourceInstance, PatientClinicalEvent clinicalEvent)
        {
            foreach (DatasetCategory dc in Dataset.DatasetCategories)
            {
                foreach (DatasetCategoryElement dce in dc.DatasetCategoryElements)
                {
                    // Default using default value
                    if (dce.DatasetElement.DefaultValue != null && dce.DatasetElement.DefaultValue != "")
                    {
                        SetInstanceValue(dce.DatasetElement, dce.DatasetElement.DefaultValue);
                    }
                    else
                    {
                        // Default using mapped value
                        if(sourceInstance != null)
                        {
                            MapValuesUsingInstance(dce, tag, sourceInstance);
                        }
                        if (clinicalEvent != null)
                        {
                            MapValuesUsingEvent(dce, tag, clinicalEvent);
                        }
                    }
                }
            }

        }

        private void MapValuesUsingInstance(DatasetCategoryElement dce, string tag, DatasetInstance sourceInstance)
        {
            var mapping = dce.DestinationMappings.SingleOrDefault(dm => dm.Tag == tag);
            if(mapping != null)
            {
                if(dce.DatasetElement.Field.FieldType.Description != "Table")
                {
                    var sourceValue = mapping.SourceElement != null ? sourceInstance.GetInstanceValue(mapping.SourceElement.DatasetElement) : "";
                    if (!String.IsNullOrWhiteSpace(sourceValue))
                    {
                        var formattedValue = TranslateSourceValueForElement(mapping, sourceValue);

                        if (!String.IsNullOrWhiteSpace(formattedValue))
                        {
                            SetInstanceValue(dce.DatasetElement, formattedValue);
                        }
                    }
                }
                else
                {
                    // we need to process mapping using sub elements
                    var sourceContexts = sourceInstance.GetInstanceSubValuesContext(mapping.SourceElement.DatasetElement);
                    foreach (Guid sourceContext in sourceContexts)
                    {
                        var newContext = Guid.NewGuid();
                        var subItemValues = sourceInstance.GetInstanceSubValues(mapping.SourceElement.DatasetElement, sourceContext);
                        foreach(DatasetMappingSub subMapping in mapping.SubMappings)
                        {
                            var sourceSubValue = subMapping.SourceElement != null ? subItemValues.SingleOrDefault(siv => siv.DatasetElementSub.Id == subMapping.SourceElement.Id) : null;
                            if (sourceSubValue != null)
                            {
                                var formattedValue = TranslateSourceValueForSubElement(subMapping, sourceSubValue.InstanceValue);

                                if (!String.IsNullOrWhiteSpace(formattedValue))
                                {
                                    SetInstanceSubValue(subMapping.DestinationElement, formattedValue, newContext);
                                }
                            }
                        }
                    }
                }
            }
        }

        private void MapValuesUsingEvent(DatasetCategoryElement dce, string tag, PatientClinicalEvent clinicalEvent)
        {
            IExtendable ptExtended = clinicalEvent.Patient;
            IExtendable ceExtended = clinicalEvent;

            if (dce.DestinationMappings.Where(dm => dm.Tag == tag).Count() > 0)
            {
                // Get the value to be translated
                var mapping = dce.DestinationMappings.Single(dm => dm.Tag == tag);
                string sourceValue = string.Empty;
                object objectValue;

                if (mapping.MappingType == MappingType.AttributeToElement || mapping.MappingType == MappingType.AttributeToValue)
                {
                    if (!String.IsNullOrWhiteSpace(mapping.PropertyPath) && !String.IsNullOrWhiteSpace(mapping.Property))
                    {
                        switch (mapping.PropertyPath)
                        {
                            case "Patient":
                                objectValue = ptExtended.GetAttributeValue(mapping.Property);
                                sourceValue = objectValue != null ? objectValue.ToString() : "";
                                break;

                            case "PatientClinicalEvent":
                                objectValue = ceExtended.GetAttributeValue(mapping.Property);
                                sourceValue = objectValue != null ? objectValue.ToString() : "";
                                break;

                            default:
                                break;
                        }
                    }
                    else
                    {
                        if (!String.IsNullOrWhiteSpace(mapping.Property))
                        {
                            objectValue = ceExtended.GetAttributeValue(mapping.Property);
                            sourceValue = objectValue != null ? objectValue.ToString() : "";
                        }
                    }
                }
                else
                {
                    Object src = clinicalEvent;
                    if (mapping.MappingType == MappingType.FirstClassToElement || mapping.MappingType == MappingType.FirstClassToValue)
                    {
                        if (!String.IsNullOrWhiteSpace(mapping.PropertyPath))
                        {
                            switch (mapping.PropertyPath)
                            {
                                case "Patient":
                                    src = clinicalEvent.Patient;
                                    break;

                                default:
                                    break;
                            }
                        }
                        objectValue = src.GetType().GetProperty(mapping.Property).GetValue(src, null);
                        sourceValue = objectValue != null ? objectValue.ToString() : "";
                    }
                }

                // Translate the value
                if (!String.IsNullOrWhiteSpace(sourceValue))
                {
                    var formattedValue = TranslateSourceValueForElement(mapping, sourceValue);
                        
                    if (!String.IsNullOrWhiteSpace(formattedValue))
                    {
                        SetInstanceValue(dce.DatasetElement, formattedValue);
                    }
                }
            }
        }

        private string TranslateSourceValueForElement(DatasetMapping mapping, string sourceValue)
        {
            var formattedValue = sourceValue.Trim();
            switch (mapping.MappingType)
            {
                case MappingType.FirstClassToElement:
                case MappingType.AttributeToElement:
                    // Length restriction for alpha numerics
                    if (mapping.DestinationElement.DatasetElement.Field.FieldType.Description == "AlphaNumericTextbox")
                    {
                        var len = mapping.DestinationElement.DatasetElement.Field.MaxLength != null ? Convert.ToInt32(mapping.DestinationElement.DatasetElement.Field.MaxLength) : 0;
                        formattedValue = len > 0 ? formattedValue.Length > len ? formattedValue.Substring(0, len - 1) : formattedValue : formattedValue;
                    }
                    if (mapping.DestinationElement.DatasetElement.Field.FieldType.Description == "Date")
                    {
                        DateTime tempdt;
                        if (DateTime.TryParse(formattedValue, out tempdt))
                        {
                            if (!String.IsNullOrWhiteSpace(mapping.MappingOption))
                            {
                                formattedValue = Convert.ToDateTime(formattedValue).ToString(mapping.MappingOption);
                            }
                        }
                        else
                        {
                            formattedValue = "";
                        }
                    }
                    if (mapping.DestinationElement.DatasetElement.Field.FieldType.Description == "NumericTextbox" && mapping.DestinationElement.DatasetElement.Field.Decimals == 0)
                    {
                        Int32 tempi;
                        formattedValue = Int32.TryParse(formattedValue, out tempi) ? Convert.ToInt32(formattedValue).ToString() : "";
                    }
                    if (mapping.DestinationElement.DatasetElement.Field.FieldType.Description == "NumericTextbox" && mapping.DestinationElement.DatasetElement.Field.Decimals > 0)
                    {
                        Decimal tempd;
                        formattedValue = Decimal.TryParse(formattedValue, out tempd) ? Convert.ToDecimal(formattedValue).ToString() : "";
                    }
                    break;

                case MappingType.ValueToValue:
                case MappingType.AttributeToValue:
                case MappingType.FirstClassToValue:
                    // Map value to value
                    var mappedValue = mapping.Values.SingleOrDefault(mv => mv.SourceValue == sourceValue && mv.Active == true);
                    if (mappedValue != null)
                    {
                        formattedValue = mappedValue.DestinationValue;
                    }
                    else
                    {
                        formattedValue = string.Empty; // didnt find a mapping
                    }
                    break;

                case MappingType.ElementToElement:
                    // Length restriction for alpha numerics
                    if (mapping.DestinationElement.DatasetElement.Field.FieldType.Description == "AlphaNumericTextbox")
                    {
                        var len = mapping.DestinationElement.DatasetElement.Field.MaxLength != null ? Convert.ToInt32(mapping.DestinationElement.DatasetElement.Field.MaxLength) : 0;
                        formattedValue = len > 0 ? formattedValue.Length > len ? formattedValue.Substring(0, len - 1) : formattedValue : formattedValue;
                    }
                    if (mapping.DestinationElement.DatasetElement.Field.FieldType.Description == "Date")
                    {
                        DateTime tempdt;
                        if (DateTime.TryParse(formattedValue, out tempdt))
                        {
                            if (!String.IsNullOrWhiteSpace(mapping.MappingOption))
                            {
                                formattedValue = Convert.ToDateTime(formattedValue).ToString(mapping.MappingOption);
                            }
                        }
                        else
                        {
                            formattedValue = "";
                        }
                    }
                    if (mapping.DestinationElement.DatasetElement.Field.FieldType.Description == "NumericTextbox" && mapping.DestinationElement.DatasetElement.Field.Decimals == 0)
                    {
                        Int32 tempi;
                        formattedValue = Int32.TryParse(formattedValue, out tempi) ? Convert.ToInt32(formattedValue).ToString() : "";
                    }
                    if (mapping.DestinationElement.DatasetElement.Field.FieldType.Description == "NumericTextbox" && mapping.DestinationElement.DatasetElement.Field.Decimals > 0)
                    {
                        Decimal tempd;
                        formattedValue = Decimal.TryParse(formattedValue, out tempd) ? Convert.ToDecimal(formattedValue).ToString() : "";
                    }
                    break;

                default:
                    break;
            }

            return formattedValue;
        }

        private string TranslateSourceValueForSubElement(DatasetMappingSub subMapping, string sourceValue)
        {
            var formattedValue = sourceValue.Trim();
            switch (subMapping.MappingType)
            {
                case MappingType.FirstClassToElement:
                case MappingType.AttributeToElement:
                    // Length restriction for alpha numerics
                    if (subMapping.DestinationElement.Field.FieldType.Description == "AlphaNumericTextbox")
                    {
                        var len = subMapping.DestinationElement.DatasetElement.Field.MaxLength != null ? Convert.ToInt32(subMapping.DestinationElement.DatasetElement.Field.MaxLength) : 0;
                        formattedValue = len > 0 ? formattedValue.Length > len ? formattedValue.Substring(0, len - 1) : formattedValue : formattedValue;
                    }
                    if (subMapping.DestinationElement.DatasetElement.Field.FieldType.Description == "Date")
                    {
                        DateTime tempdt;
                        if (DateTime.TryParse(formattedValue, out tempdt))
                        {
                            if (!String.IsNullOrWhiteSpace(subMapping.MappingOption))
                            {
                                formattedValue = Convert.ToDateTime(formattedValue).ToString(subMapping.MappingOption);
                            }
                        }
                        else
                        {
                            formattedValue = "";
                        }
                    }
                    if (subMapping.DestinationElement.DatasetElement.Field.FieldType.Description == "NumericTextbox" && subMapping.DestinationElement.DatasetElement.Field.Decimals == 0)
                    {
                        Int32 tempi;
                        formattedValue = Int32.TryParse(formattedValue, out tempi) ? Convert.ToInt32(formattedValue).ToString() : "";
                    }
                    if (subMapping.DestinationElement.DatasetElement.Field.FieldType.Description == "NumericTextbox" && subMapping.DestinationElement.DatasetElement.Field.Decimals > 0)
                    {
                        Decimal tempd;
                        formattedValue = Decimal.TryParse(formattedValue, out tempd) ? Convert.ToDecimal(formattedValue).ToString() : "";
                    }
                    break;

                case MappingType.ValueToValue:
                case MappingType.AttributeToValue:
                case MappingType.FirstClassToValue:
                    // Map value to value
                    var mappedValue = subMapping.Values.SingleOrDefault(mv => mv.SourceValue == sourceValue && mv.Active == true);
                    if (mappedValue != null)
                    {
                        formattedValue = mappedValue.DestinationValue;
                    }
                    else
                    {
                        formattedValue = string.Empty; // didnt find a mapping
                    }
                    break;

                case MappingType.ElementToElement:
                    // Length restriction for alpha numerics
                    if (subMapping.DestinationElement.DatasetElement.Field.FieldType.Description == "AlphaNumericTextbox")
                    {
                        var len = subMapping.DestinationElement.DatasetElement.Field.MaxLength != null ? Convert.ToInt32(subMapping.DestinationElement.DatasetElement.Field.MaxLength) : 0;
                        formattedValue = len > 0 ? formattedValue.Length > len ? formattedValue.Substring(0, len - 1) : formattedValue : formattedValue;
                    }
                    if (subMapping.DestinationElement.DatasetElement.Field.FieldType.Description == "Date")
                    {
                        DateTime tempdt;
                        if (DateTime.TryParse(formattedValue, out tempdt))
                        {
                            if (!String.IsNullOrWhiteSpace(subMapping.MappingOption))
                            {
                                formattedValue = Convert.ToDateTime(formattedValue).ToString(subMapping.MappingOption);
                            }
                        }
                        else
                        {
                            formattedValue = "";
                        }
                    }
                    if (subMapping.DestinationElement.DatasetElement.Field.FieldType.Description == "NumericTextbox" && subMapping.DestinationElement.DatasetElement.Field.Decimals == 0)
                    {
                        Int32 tempi;
                        formattedValue = Int32.TryParse(formattedValue, out tempi) ? Convert.ToInt32(formattedValue).ToString() : "";
                    }
                    if (subMapping.DestinationElement.DatasetElement.Field.FieldType.Description == "NumericTextbox" && subMapping.DestinationElement.DatasetElement.Field.Decimals > 0)
                    {
                        Decimal tempd;
                        formattedValue = Decimal.TryParse(formattedValue, out tempd) ? Convert.ToDecimal(formattedValue).ToString() : "";
                    }
                    break;

                default:
                    break;
            }

            return formattedValue;
        }

        public DatasetInstanceValue AddInstanceValue(DatasetElement datasetElement, string instanceValue)
        {
            var datasetInstanceValue = new DatasetInstanceValue(datasetElement, this, instanceValue);

            DatasetInstanceValues.Add(datasetInstanceValue);

            return datasetInstanceValue;
        }

        public string GetInstanceValue(DatasetElement datasetElement)
        {
            if (datasetElement == null) { return string.Empty; };
            var instanceValue = DatasetInstanceValues.SingleOrDefault(div => div.DatasetElement.Id == datasetElement.Id);

            return instanceValue != null ? instanceValue.InstanceValue : string.Empty;
        }

        public DatasetInstanceValue SetInstanceValue(DatasetElement datasetElement, string instanceValue)
        {
            datasetElement.Validate(instanceValue);
            var datasetInstanceValue = DatasetInstanceValues.SingleOrDefault(div => div.DatasetElement.Id == datasetElement.Id);

            if (datasetInstanceValue == null)
            {
                if (!string.IsNullOrWhiteSpace(instanceValue))
                {
                    datasetInstanceValue = AddInstanceValue(datasetElement, instanceValue);
                }
            }
            else
            {
                if (string.IsNullOrWhiteSpace(instanceValue))
                {
                    DatasetInstanceValues.Remove(datasetInstanceValue);
                    datasetInstanceValue = null;
                }
                else
                {
                    datasetInstanceValue.InstanceValue = instanceValue;
                }
            }

            return datasetInstanceValue;
        }

        public DatasetInstanceSubValue SetInstanceSubValue(DatasetElementSub datasetElementSub, string instanceSubValue, Guid contextValue = default(Guid))
        {
            datasetElementSub.Validate(instanceSubValue);
            var datasetInstanceValue = DatasetInstanceValues.SingleOrDefault(div => div.DatasetElement.Id == datasetElementSub.DatasetElement.Id);
            if (datasetInstanceValue == null)
            {
                if (datasetElementSub.DatasetElement.Field.FieldType.Id == (int)FieldTypes.Table)
                {
                    datasetInstanceValue = SetInstanceValue(datasetElementSub.DatasetElement, "<<TABLE>>");
                }
                else
                {
                    throw new Exception("Cannot set an instance sub value without setting the parent instance value");
                }
            }

            var datasetInstanceSubValue = datasetInstanceValue.DatasetInstanceSubValues.SingleOrDefault(disv => disv.DatasetElementSub.Id == datasetElementSub.Id && disv.ContextValue == contextValue);
            if (datasetInstanceSubValue == null)
            {
                if (!string.IsNullOrWhiteSpace(instanceSubValue))
                {
                    datasetInstanceSubValue = datasetInstanceValue.AddDatasetInstanceSubValue(datasetElementSub, instanceSubValue, contextValue);
                }
                else 
                {
                    if (datasetElementSub.Field.Mandatory)
                    {
                        throw new DatasetFieldSetException(datasetElementSub.Id.ToString(), string.Format("{0} is required", datasetElementSub.ElementName));
                    }
                }
            }
            else
            {
                if (string.IsNullOrWhiteSpace(instanceSubValue))
                {
                    if (datasetElementSub.Field.Mandatory)
                    {
                        throw new DatasetFieldSetException(datasetElementSub.Id.ToString(), string.Format("{0} is required", datasetElementSub.ElementName));
                    }

                    datasetInstanceValue.DatasetInstanceSubValues.Remove(datasetInstanceSubValue);
                    datasetInstanceSubValue = null;
                }
                else
                {
                    datasetInstanceSubValue.InstanceValue = instanceSubValue;
                }
            }

            return datasetInstanceSubValue;
        }

        public Guid[] GetInstanceSubValuesContext(DatasetElement datasetElement)
        {
            var datasetInstanceValue = DatasetInstanceValues.SingleOrDefault(div => div.DatasetElement.Id == datasetElement.Id);

            if (datasetInstanceValue != null && datasetInstanceValue.DatasetInstanceSubValues.Any())
            {
                return datasetInstanceValue.DatasetInstanceSubValues.Select(disv => disv.ContextValue).Distinct().ToArray();
            }

            return new Guid[0];
        }

        public Guid? GetContextForInstanceSubValue(DatasetElement datasetElement, DatasetElementSub datasetSubElement, string instanceValue)
        {
            var datasetInstanceValue = DatasetInstanceValues.SingleOrDefault(div => div.DatasetElement.Id == datasetElement.Id);

            if (datasetInstanceValue != null && datasetInstanceValue.DatasetInstanceSubValues.Any())
            {
                return datasetInstanceValue.DatasetInstanceSubValues.Where(disv => disv.DatasetElementSub.Id == datasetSubElement.Id && disv.InstanceValue.Trim() == instanceValue.Trim()).Select(disv => disv.ContextValue).First();
            }

            return null;
        }

        public DatasetInstanceSubValue[] GetInstanceSubValues(DatasetElement datasetElement, Guid context)
        {
            var datasetInstanceValue = DatasetInstanceValues.SingleOrDefault(div => div.DatasetElement.Id == datasetElement.Id);

            if (datasetInstanceValue != null && datasetInstanceValue.DatasetInstanceSubValues.Any())
            {
                return datasetInstanceValue.DatasetInstanceSubValues.Where(disv => disv.ContextValue == context).ToArray();
            }

            return new DatasetInstanceSubValue[0];
        }
    }
}
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace VPS.CustomAttributes.Tests
{
    [TestFixture]
    public class TypeExtensionHandlerTests
    {
        private Mock<ICustomAttributeConfigRepository> attributeConfigRepository;
        private Mock<ISelectionDataRepository> selectionDataRepository;
        private TypeExtensionHandler handlerUnderTest;
        private IExtendable extendedObject;
        private IExtendable emptyExtendable;
        private IExtendable extendedObjectWithSelectionAttribute;

        [SetUp]
        public void SetUp()
        {
            attributeConfigRepository = new Mock<ICustomAttributeConfigRepository>();
            selectionDataRepository = new Mock<ISelectionDataRepository>();
            handlerUnderTest = new TypeExtensionHandler(attributeConfigRepository.Object, selectionDataRepository.Object);
            extendedObjectWithSelectionAttribute = new ExtendableClass { CustomAttributesXmlSerialised = "<CustomAttributeSet><CustomSelectionAttribute><Key>DwellingHeating</Key><Value>3</Value></CustomSelectionAttribute></CustomAttributeSet>" };
            extendedObject = new ExtendableClass { CustomAttributesXmlSerialised = "<CustomAttributeSet><CustomStringAttribute><Key>HouseName</Key><Value>TestName</Value></CustomStringAttribute><CustomDateTimeAttribute><Key>OccupiedSince</Key><Value>2000-01-01T00:00:00</Value></CustomDateTimeAttribute><CustomNumericAttribute><Key>MemberCount</Key><Value>5</Value></CustomNumericAttribute></CustomAttributeSet>" };
            emptyExtendable = new ExtendableClass();
        }

        [Test]
        public void BuildEditModel_RetrievesAttribueConfigsForSpecifiedType()
        {
            // Act
            handlerUnderTest.BuildModelExtension(extendedObject);

            // Assert
            attributeConfigRepository.Verify(c => c.RetrieveAttributeConfigurationsForType(extendedObject.GetType().Name), Times.Exactly(1));
        }

        [Test]
        public void BuildEditModel_WhenNoAttributeConfigs_ReturnsEmptyModelExtension()
        {
            // Arrange
            attributeConfigRepository.Setup(r => r.RetrieveAttributeConfigurationsForType(It.IsAny<string>()))
                .Returns(new List<CustomAttributeConfiguration>());

            // Act
            var actual = handlerUnderTest.BuildModelExtension(extendedObject);

            // Assert
            Assert.AreEqual(0, actual.Count, "Dynamic Model Element count.");
        }

        [Test]
        public void BuildEditModel_ReturnsModelExtensionWithCustomAttributes()
        {
            // Arrange
            var attributeConfigs = new List<CustomAttributeConfiguration>
            {
                new CustomAttributeConfiguration
                {
                    ExtendableTypeName = typeof(ExtendableClass).Name,
                    Category = "General",
                    AttributeKey = "HouseName",
                    CustomAttributeType = CustomAttributeType.String
                },
                new CustomAttributeConfiguration
                {
                    ExtendableTypeName = typeof(ExtendableClass).Name,
                    Category = "Residence",
                    AttributeKey = "OccupiedSince",
                    CustomAttributeType = CustomAttributeType.DateTime
                },                
                new CustomAttributeConfiguration
                {
                    ExtendableTypeName = typeof(ExtendableClass).Name,
                    Category = "Members",
                    AttributeKey = "MemberCount",
                    CustomAttributeType = CustomAttributeType.Numeric
                },
            };

            attributeConfigRepository.Setup(r => r.RetrieveAttributeConfigurationsForType(It.IsAny<string>()))
                .Returns(attributeConfigs);

            // Act
            var actual = handlerUnderTest.BuildModelExtension(extendedObject);

            // Assert
            Assert.AreEqual(3, actual.Count, "Dynamic Model Element count.");
            var firstItem = actual.First();
            Assert.AreEqual("HouseName", firstItem.AttributeKey, "First Item Key");
            Assert.AreEqual("General", firstItem.Category, "First Item Category");
            Assert.AreEqual("TestName", firstItem.Value, "First Item Value");
            Assert.AreEqual(CustomAttributeType.String, firstItem.Type, "First Item Type");
        }

        [Test]
        public void BuildModel_WhenExtendableHasSelectionCustomAttribute_RetrievesAttributeSelectionDataSet()
        {
            // Arrange
            var selectionAttributeConfig = new List<CustomAttributeConfiguration>
            {
                new CustomAttributeConfiguration
                {
                    ExtendableTypeName = typeof(ExtendableClass).Name,
                    Category = "General",
                    AttributeKey = "DwellingHeating",
                    CustomAttributeType = CustomAttributeType.Selection
                },
                new CustomAttributeConfiguration
                {
                    ExtendableTypeName = typeof(ExtendableClass).Name,
                    Category = "General",
                    AttributeKey = "NonExistent",
                    CustomAttributeType = CustomAttributeType.Selection
                }
            };

            attributeConfigRepository.Setup(r => r.RetrieveAttributeConfigurationsForType(It.IsAny<string>()))
                .Returns(selectionAttributeConfig);

            // Act
            var actual = handlerUnderTest.BuildModelExtension(extendedObject);

            // Assert
            selectionDataRepository.Verify(r => r.RetrieveSelectionDataForAttribute("DwellingHeating"), Times.Exactly(1));
        }

        [Test]
        public void BuildModel_WhenExtendableHasSelectionCustomAttribute_ReturnsModelExtensionWithAttributeAndRefData()
        {
            // Arrange
            var selectionAttributeConfig = new List<CustomAttributeConfiguration>
            {
                new CustomAttributeConfiguration
                {
                    ExtendableTypeName = typeof(ExtendableClass).Name,
                    Category = "General",
                    AttributeKey = "DwellingHeating",
                    CustomAttributeType = CustomAttributeType.Selection
                }
            };

            attributeConfigRepository.Setup(r => r.RetrieveAttributeConfigurationsForType(It.IsAny<string>()))
                .Returns(selectionAttributeConfig);

            var selectionDataSet = new List<SelectionDataItem>
            { 
                new SelectionDataItem{AttributeKey = "DwellingHeating", Id = 1L, Value = "Electricity"},
                new SelectionDataItem{AttributeKey = "DwellingHeating", Id = 2L, Value = "Gas"},
                new SelectionDataItem{AttributeKey = "DwellingHeating", Id = 3L, Value = "Paraffin"},
                new SelectionDataItem{AttributeKey = "DwellingHeating", Id = 4L, Value = "Coal"},
                new SelectionDataItem{AttributeKey = "DwellingHeating", Id = 5L, Value = "Wood"},
                new SelectionDataItem{AttributeKey = "DwellingHeating", Id = 6L, Value = "Cow Dung"},
                new SelectionDataItem{AttributeKey = "DwellingHeating", Id = 7L, Value = "Crop Waste"},
                new SelectionDataItem{AttributeKey = "DwellingHeating", Id = 8L, Value = "Other"},
                new SelectionDataItem{AttributeKey = "DwellingHeating", Id = 9L, Value = "None"}
            };
            
            selectionDataRepository.Setup(r => r.RetrieveSelectionDataForAttribute("DwellingHeating"))
                .Returns(selectionDataSet);

            // Act
            var actual = handlerUnderTest.BuildModelExtension(extendedObject);

            // Assert
            Assert.AreEqual(1, actual.Count, "Dynamic Model Element count.");
            var firstItem = actual.First();
            Assert.AreEqual(9, firstItem.RefData.Count, "Selection Count");
        }

        [Test]
        public void UpdateExtendable_WhenCustomAttributesNull_ReturnsUnchangedExtendable()
        {
            // Act
            handlerUnderTest.UpdateExtendable(emptyExtendable, null, "UserName");

            // Assert
            Assert.AreEqual(0, emptyExtendable.CustomAttributes.CustomNumericAttributes.Count, "Numeric Attribute Count");
            Assert.AreEqual(0, emptyExtendable.CustomAttributes.CustomDateTimeAttributes.Count, "DateTime Attribute Count");
            Assert.AreEqual(0, emptyExtendable.CustomAttributes.CustomSelectionAttributes.Count, "Selection Attribute Count");
            Assert.AreEqual(0, emptyExtendable.CustomAttributes.CustomStringAttributes.Count, "String Attribute Count");
        }

        [Test]
        public void UpdateExtendable_SetsNumericCustomAttribute()
        {
            // Arrange
            var attributeDetails = new List<CustomAttributeDetail>
            {
                new CustomAttributeDetail
                {
                    AttributeKey = "NumericAttribute",
                    Type = CustomAttributeType.Numeric,
                    Value = "1.0"
                }
            };

            // Act
            handlerUnderTest.UpdateExtendable(emptyExtendable, attributeDetails, "UserName");

            // Assert
            Assert.AreEqual(1, emptyExtendable.CustomAttributes.CustomNumericAttributes.Count, "Numeric Attribute Count");
            Assert.AreEqual(typeof(decimal), emptyExtendable.GetAttributeValue("NumericAttribute").GetType(), "NumericAttribute type");
            Assert.AreEqual(1.0, Convert.ToDecimal(emptyExtendable.GetAttributeValue("NumericAttribute")), "NumericAttribute value");
        }

        [Test]
        public void UpdateExtendable_SetsStringCustomAttribute()
        {
            // Arrange
            var attributeDetails = new List<CustomAttributeDetail>
            {
                new CustomAttributeDetail
                {
                    AttributeKey = "StringAttribute",
                    Type = CustomAttributeType.String,
                    Value = "StringAttributeValue"
                }
            };

            // Act
            handlerUnderTest.UpdateExtendable(emptyExtendable, attributeDetails, "UserName");

            // Assert
            Assert.AreEqual(1, emptyExtendable.CustomAttributes.CustomStringAttributes.Count, "String Attribute Count");
            Assert.AreEqual(typeof(string), emptyExtendable.GetAttributeValue("StringAttribute").GetType(), "StringAttribute type");
            Assert.AreEqual("StringAttributeValue", emptyExtendable.GetAttributeValue("StringAttribute").ToString(), "StringAttribute value");
        }

        [Test]
        public void UpdateExtendable_SetsDateTimeCustomAttribute()
        {
            // Arrange
            var attributeDetails = new List<CustomAttributeDetail>
            {
                new CustomAttributeDetail
                {
                    AttributeKey = "DateTimeAttribute",
                    Type = CustomAttributeType.DateTime,
                    Value = "01/01/0001 00:00:00"
                }
            };

            // Act
            handlerUnderTest.UpdateExtendable(emptyExtendable, attributeDetails, "UserName");

            // Assert
            Assert.AreEqual(1, emptyExtendable.CustomAttributes.CustomDateTimeAttributes.Count, "DateTime Attribute Count");
            Assert.AreEqual(typeof(DateTime), emptyExtendable.GetAttributeValue("DateTimeAttribute").GetType(), "DateTimeAttribute type");
            Assert.AreEqual(new DateTime(0001, 01, 01), Convert.ToDateTime(emptyExtendable.GetAttributeValue("DateTimeAttribute")), "DateTimeAttribute value");
        }

        [Test]
        public void UpdateExtendable_SetsSelectionCustomAttribute()
        {
            // Arrange
            var attributeDetails = new List<CustomAttributeDetail>
            {
                new CustomAttributeDetail
                {
                    AttributeKey = "SelectionAttribute",
                    Type = CustomAttributeType.Selection,
                    Value = "5"
                }
            };

            // Act
            handlerUnderTest.UpdateExtendable(emptyExtendable, attributeDetails, "UserName");

            // Assert
            Assert.AreEqual(1, emptyExtendable.CustomAttributes.CustomSelectionAttributes.Count, "Selection Attribute Count");
            Assert.AreEqual(typeof(int), emptyExtendable.GetAttributeValue("SelectionAttribute").GetType(), "SelectionAttribute type");
            Assert.AreEqual(5, Convert.ToInt32(emptyExtendable.GetAttributeValue("SelectionAttribute")), "SelectionAttribute value");
        }

        [Test]
        public void UpdateExtenable_HandlesArrayValues()
        {
            // Arrange
            var attributeDetails = new List<CustomAttributeDetail>
            {
                new CustomAttributeDetail
                {
                    AttributeKey = "SelectionAttribute",
                    Type = CustomAttributeType.Selection,
                    Value = new[]{ "5" }
                }
            };

            // Act
            handlerUnderTest.UpdateExtendable(emptyExtendable, attributeDetails, "UserName");

            // Assert
            Assert.AreEqual(1, emptyExtendable.CustomAttributes.CustomSelectionAttributes.Count, "Selection Attribute Count");
            Assert.AreEqual(typeof(int), emptyExtendable.GetAttributeValue("SelectionAttribute").GetType(), "SelectionAttribute type");
            Assert.AreEqual(5, Convert.ToInt32(emptyExtendable.GetAttributeValue("SelectionAttribute")), "SelectionAttribute value");
        }

        [Test]
        public void RefreshReferenceData_WhenCustomAttributesNull_ReturnsEmptyCustomAttributeCollection()
        {
            // Act
            var customAttributeList = handlerUnderTest.RefreshReferenceData(null);

            // Assert
            Assert.AreEqual(0, customAttributeList.Count);
        }

        [Test]
        public void RefreshReferenceData_RefreshesReferenceData()
        {
            // Arrange
            var attributeDetails = new List<CustomAttributeDetail>
            {
                new CustomAttributeDetail
                {
                    AttributeKey = "SelectionAttribute",
                    Type = CustomAttributeType.Selection,
                    Value = "5"
                },
                new CustomAttributeDetail
                {
                    AttributeKey = "StringAttribute",
                    Type = CustomAttributeType.String,
                    Value = "StringAttributeValue"
                }
            };

            var selectionDataItems = new Collection<SelectionDataItem>
            {
                new SelectionDataItem{AttributeKey="SelectionAttribute",Id = 1, Value = "First Item"},
                new SelectionDataItem{AttributeKey="SelectionAttribute",Id = 2, Value = "Second Item"}
            };

            selectionDataRepository.Setup(r => r.RetrieveSelectionDataForAttribute(It.IsAny<string>())).Returns(selectionDataItems);

            // Act
            attributeDetails = handlerUnderTest.RefreshReferenceData(attributeDetails);

            // Assert
            Assert.AreEqual(2, attributeDetails.SingleOrDefault(a => a.AttributeKey == "SelectionAttribute").RefData.Count, "selectionRefData");
            selectionDataRepository.Verify(r => r.RetrieveSelectionDataForAttribute(It.IsAny<string>()), Times.Exactly(1));
        }

        [Test]
        public void RefreshReferenceData_HandlesArrayValue()
        {
            // Arrange
            var attributeDetails = new List<CustomAttributeDetail>
            {
                new CustomAttributeDetail
                {
                    AttributeKey = "SelectionAttribute",
                    Type = CustomAttributeType.Selection,
                    Value = new[]{ "5" }
                }
            };

            var selectionDataItems = new Collection<SelectionDataItem>
            {
                new SelectionDataItem{AttributeKey="SelectionAttribute",Id = 1, Value = "First Item"},
                new SelectionDataItem{AttributeKey="SelectionAttribute",Id = 2, Value = "Second Item"}
            };

            selectionDataRepository.Setup(r => r.RetrieveSelectionDataForAttribute(It.IsAny<string>())).Returns(selectionDataItems);

            // Act
            attributeDetails = handlerUnderTest.RefreshReferenceData(attributeDetails);

            // Assert
            Assert.AreEqual(2, attributeDetails.SingleOrDefault(a => a.AttributeKey == "SelectionAttribute").RefData.Count, "selectionRefData");
            selectionDataRepository.Verify(r => r.RetrieveSelectionDataForAttribute(It.IsAny<string>()), Times.Exactly(1));
        }
    }


    class ExtendableClass : IExtendable
    {
        private CustomAttributeSet customAttributes = new CustomAttributeSet();

        CustomAttributeSet IExtendable.CustomAttributes
        {
            get { return customAttributes; }
        }
        public string CustomAttributesXmlSerialised
        {
            get { return XmlSerialisationHelper.SerialiseToXmlString(customAttributes); }
            set
            {
                if (string.IsNullOrEmpty(value))
                    customAttributes = new CustomAttributeSet();
                else
                    customAttributes = XmlSerialisationHelper.DeserialiseFromXmlString<CustomAttributeSet>(value);
            }

        }

        public void SetAttributeValue<T>(string attributeKey, T attributeValue, string updatedByUser)
        {
            customAttributes.SetAttributeValue(attributeKey, attributeValue, updatedByUser);
        }

        public object GetAttributeValue(string attributeKey)
        {
            return customAttributes.GetAttributeValue(attributeKey);
        }

        public void ValidateAndSetAttributeValue<T>(CustomAttributeConfiguration attributeConfig, T attributeValue, string updatedByUser)
        {
            customAttributes.ValidateAndSetAttributeValue(attributeConfig, attributeValue, updatedByUser);
        }

        public DateTime GetUpdatedDate(string attributeKey)
        {
            return customAttributes.GetUpdatedDate(attributeKey);
        }

        public string GetUpdatedByUser(string attributeKey)
        {
            return customAttributes.GetUpdatedByUser(attributeKey);
        }
    }

    static class XmlSerialisationHelper
    {
        private static string serialisedXmlString = string.Empty;

        public static string SerialiseToXmlString<T>(T objectToSerialise)
        {
            var serialiser = new XmlSerializer(typeof(T));
            var ns = new XmlSerializerNamespaces();
            ns.Add("", "");

            using (var stream = new MemoryStream())
            using (var streamReader = new StreamReader(stream))
            {
                serialiser.Serialize(stream, objectToSerialise, ns);
                stream.Position = 0;
                serialisedXmlString = streamReader.ReadToEnd();
            }

            return serialisedXmlString;
        }

        public static T DeserialiseFromXmlString<T>(string xmlString)
        {
            if (string.IsNullOrEmpty(xmlString)) return default(T);

            T returnType;

            var serialiser = new XmlSerializer(typeof(T));

            using (var textReader = new StringReader(xmlString))
            {
                returnType = (T)serialiser.Deserialize(textReader);
            }

            return returnType;
        } 
    }
}

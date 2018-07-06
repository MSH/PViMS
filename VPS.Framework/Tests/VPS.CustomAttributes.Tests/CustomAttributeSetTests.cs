using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VPS.CustomAttributes.Tests
{
    [TestFixture]
    public class CustomAttributeSetTests
    {
        private CustomAttributeSet classUnderTest;

        [SetUp]
        public void SetUp()
        {
            classUnderTest = new CustomAttributeSet();
        }

        [Test]
        public void ConstructorInitialisesAllAttributeLists()
        {
            Assert.IsNotNull(classUnderTest.CustomDateTimeAttributes);
            Assert.IsNotNull(classUnderTest.CustomNumericAttributes);
            Assert.IsNotNull(classUnderTest.CustomSelectionAttributes);
            Assert.IsNotNull(classUnderTest.CustomStringAttributes);
        }

        [Test]
        public void SetAttributeValue_WhenDecimalSpecified_SetsNumericAtttibuteOnly()
        {
            // Act
            classUnderTest.SetAttributeValue("testnumeric", 1m, "UserName");

            // Assert
            Assert.AreEqual(1, classUnderTest.CustomNumericAttributes.Count);
            Assert.AreEqual(0, classUnderTest.CustomSelectionAttributes.Count);
            Assert.AreEqual(0, classUnderTest.CustomDateTimeAttributes.Count);
            Assert.AreEqual(0, classUnderTest.CustomStringAttributes.Count);
            var numericAttribute = classUnderTest.CustomNumericAttributes.SingleOrDefault(a => a.Key == "testnumeric");
            Assert.IsNotNull(numericAttribute);
            Assert.AreEqual(1m, numericAttribute.Value);
        }

        [Test]
        [ExpectedException(typeof(CustomAttributeException))]
        public void SetAttributeValue_WhenLongSpecified_ThrowsCustomAttributeException()
        {
            classUnderTest.SetAttributeValue("testlong", 1L, "UserName");
        }

        [Test]
        [ExpectedException(typeof(CustomAttributeException))]
        public void SetAttributeValue_WhenDoubleSpecified_ThrowsCustomAttributeException()
        {
            classUnderTest.SetAttributeValue("testlong", 1d, "UserName");
        }

        [ExpectedException(typeof(CustomAttributeException))]
        [Test]
        public void SetAttributeValue_WhenSingleSpecified_ThrowsCustomAttributeException()
        {
            classUnderTest.SetAttributeValue("testlong", 1f, "UserName");
        }

        [Test]
        public void SetAttributeValue_WhenDateTimeSpecified_SetsDateTimeAtttibuteOnly()
        {
            // Arrange
            DateTime attributeValue = new DateTime(2014, 11, 1, 15, 3, 30, 123);

            // Act
            classUnderTest.SetAttributeValue("testdatetime", attributeValue, "UserName");

            // Assert
            Assert.AreEqual(0, classUnderTest.CustomNumericAttributes.Count);
            Assert.AreEqual(0, classUnderTest.CustomSelectionAttributes.Count);
            Assert.AreEqual(1, classUnderTest.CustomDateTimeAttributes.Count);
            Assert.AreEqual(0, classUnderTest.CustomStringAttributes.Count);
            CustomDateTimeAttribute dateTimeAttribute = classUnderTest.CustomDateTimeAttributes.SingleOrDefault(a => a.Key == "testdatetime");
            Assert.IsNotNull(dateTimeAttribute);
            Assert.AreEqual(attributeValue, dateTimeAttribute.Value);
        }

        [Test]
        public void SetAttributeValue_WhenStringSpecified_SetsStringAtttibuteOnly()
        {
            // Arrange
            string attributeValue = "this is my test string";

            // Act
            classUnderTest.SetAttributeValue("teststring", attributeValue, "UserName");

            // Assert
            Assert.AreEqual(0, classUnderTest.CustomNumericAttributes.Count);
            Assert.AreEqual(0, classUnderTest.CustomSelectionAttributes.Count);
            Assert.AreEqual(0, classUnderTest.CustomDateTimeAttributes.Count);
            Assert.AreEqual(1, classUnderTest.CustomStringAttributes.Count);
            CustomStringAttribute customStringAttribute = classUnderTest.CustomStringAttributes.SingleOrDefault(a => a.Key == "teststring");
            Assert.IsNotNull(customStringAttribute);
            Assert.AreEqual(attributeValue, customStringAttribute.Value);
        }

        [Test]
        public void SetAttributeValue_WhenSelectionSpecified_SetsSelectionAtttibuteOnly()
        {
            // Act
            classUnderTest.SetAttributeValue("testselection", 1, "UserName");

            // Assert
            Assert.AreEqual(0, classUnderTest.CustomNumericAttributes.Count);
            Assert.AreEqual(1, classUnderTest.CustomSelectionAttributes.Count);
            Assert.AreEqual(0, classUnderTest.CustomDateTimeAttributes.Count);
            Assert.AreEqual(0, classUnderTest.CustomStringAttributes.Count);
            CustomSelectionAttribute selectionAttribute = classUnderTest.CustomSelectionAttributes.SingleOrDefault(a => a.Key == "testselection");
            Assert.IsNotNull(selectionAttribute);
            Assert.AreEqual(1, selectionAttribute.Value);
        }

        [Test]
        public void SetAttributeValue_WhenSelectionSpecifiedAndAttributeExitst_ReplacesExistingValue()
        {
            // Arrange
            classUnderTest.SetAttributeValue("testselection", 1, "UserName");

            // Act
            classUnderTest.SetAttributeValue("testselection", 2, "UserName");

            // Asssrt
            Assert.AreEqual(1, classUnderTest.CustomSelectionAttributes.Count);
            CustomSelectionAttribute selectionAttribute = classUnderTest.CustomSelectionAttributes.SingleOrDefault(a => a.Key == "testselection");
            Assert.IsNotNull(selectionAttribute);
            Assert.AreEqual(2, selectionAttribute.Value);
        }

        [Test]
        public void SetAttributeValue_WhenStringSpecifiedAndAttributeExitst_ReplacesExistingValue()
        {
            // Arrange
            classUnderTest.SetAttributeValue("teststring", "This is the initial string.", "UserName");

            // Act
            classUnderTest.SetAttributeValue("teststring", "This is the new string.", "UserName");

            // Asssrt
            Assert.AreEqual(1, classUnderTest.CustomStringAttributes.Count);
            var actualAttribute = classUnderTest.CustomStringAttributes.SingleOrDefault(a => a.Key == "teststring");
            Assert.IsNotNull(actualAttribute);
            Assert.AreEqual("This is the new string.", actualAttribute.Value);
        }

        [Test]
        public void SetAttributeValue_WhenDateTimeSpecifiedAndAttributeExitst_ReplacesExistingValue()
        {
            // Arrange
            var attributeKey = "testdatetime";
            DateTime intialDate = new DateTime(2014, 11, 1, 15, 3, 30, 123);
            DateTime newDate = new DateTime(2015, 11, 1, 15, 3, 30, 123);

            classUnderTest.SetAttributeValue(attributeKey, intialDate, "UserName");

            // Act
            classUnderTest.SetAttributeValue(attributeKey, newDate, "UserName");

            // Asssrt
            Assert.AreEqual(1, classUnderTest.CustomDateTimeAttributes.Count);
            var actualAttribute = classUnderTest.CustomDateTimeAttributes.SingleOrDefault(a => a.Key == attributeKey);
            Assert.IsNotNull(actualAttribute);
            Assert.AreEqual(newDate, actualAttribute.Value);
        }

        [Test]
        public void SetAttributeValue_WhenDecimalSpecifiedAndAttributeExitst_ReplacesExistingValue()
        {
            // Arrange
            var attributeKey = "testnumeric";
            classUnderTest.SetAttributeValue(attributeKey, 1m, "UserName");

            // Act
            classUnderTest.SetAttributeValue(attributeKey, 2.1m, "UserName");

            // Asssrt
            Assert.AreEqual(1, classUnderTest.CustomNumericAttributes.Count);
            var actualAttribute = classUnderTest.CustomNumericAttributes.SingleOrDefault(a => a.Key == attributeKey);
            Assert.IsNotNull(actualAttribute);
            Assert.AreEqual(2.1m, actualAttribute.Value);
        }

        [Test]
        public void ValidateAndSetAttributeValue_WhenStringDoesNotExeedStringMaxLength_SetsValue()
        {
            // Arrange
            var stringCustomAttributeConfig = new CustomAttributeConfiguration
            {
                AttributeKey = "StringWithMaxLength",
                CustomAttributeType = CustomAttributeType.String,
                StringMaxLength = 500
            };

            // Act
            classUnderTest.ValidateAndSetAttributeValue(stringCustomAttributeConfig, "ValidString", "UserName");

            // Assert
            var customStringAttribute = classUnderTest.CustomStringAttributes.SingleOrDefault(a => a.Key == "StringWithMaxLength");
            Assert.IsNotNull(customStringAttribute);
            Assert.AreEqual("ValidString", customStringAttribute.Value);
        }

        [Test, ExpectedException(typeof(CustomAttributeValidationException))]
        public void ValidateAndSetAttributeValue_WhenStringAndExceedsStringMaxLength_ThrowsCustomAttributeException()
        {
            // Arrange
            var stringCustomAttributeConfig = new CustomAttributeConfiguration
            {
                AttributeKey = "StringWithMaxLength",
                CustomAttributeType = CustomAttributeType.String,
                StringMaxLength = 5
            };

            // Act
            classUnderTest.ValidateAndSetAttributeValue(stringCustomAttributeConfig, "StringLongerThan5Characters", "UserName");
        }

        [Test, ExpectedException(typeof(CustomAttributeValidationException))]
        public void ValidateAndSetAttributeValue_WhenNumericAndLowerThanMinValue_ThrowsCustomAttributeException()
        {
            // Arrange
            var numericCustomAttributeConfig = new CustomAttributeConfiguration
            {
                CustomAttributeType = CustomAttributeType.Numeric,
                NumericMinValue = 5
            };

            // Act
            classUnderTest.ValidateAndSetAttributeValue(numericCustomAttributeConfig, 4m, "UserName");
        }

        [Test, ExpectedException(typeof(CustomAttributeValidationException))]
        public void ValidateAndSetAttributeValue_WhenNumericAndHigherThanMaxValue_ThrowsCustomAttributeException()
        {
            // Arrange
            var numericCustomAttributeConfig = new CustomAttributeConfiguration
            {
                CustomAttributeType = CustomAttributeType.Numeric,
                NumericMaxValue = 5
            };

            // Act
            classUnderTest.ValidateAndSetAttributeValue(numericCustomAttributeConfig, 6m, "UserName");
        }

        [Test]
        public void ValidateAndSetAttributeValue_WhenNumericAndWithInValidRange_SetsValue()
        {
            // Arrange
            var numericCustomAttributeConfig = new CustomAttributeConfiguration
            {
                AttributeKey = "ValidNumeric",
                CustomAttributeType = CustomAttributeType.Numeric,
                NumericMinValue = 5,
                NumericMaxValue = 10
            };

            // Act
            classUnderTest.ValidateAndSetAttributeValue(numericCustomAttributeConfig, 6m, "UserName");

            // Assert
            var customNumericAttribute = classUnderTest.CustomNumericAttributes.SingleOrDefault(a => a.Key == "ValidNumeric");
            Assert.IsNotNull(customNumericAttribute);
            Assert.AreEqual(6m, customNumericAttribute.Value);
        }

        [Test, ExpectedException(typeof(CustomAttributeValidationException))]
        public void ValidateAndSetAttributeValue_WhenRequiredAndNotSpecified_ThrowsCustomAttributeValidationException()
        {
            // Arrange
            var stringCustomAttributeConfig = new CustomAttributeConfiguration
            {
                CustomAttributeType = CustomAttributeType.String,
                IsRequired = true
            };

            // Act
            classUnderTest.ValidateAndSetAttributeValue(stringCustomAttributeConfig, default(string), "UserName");
        }

        [Test, ExpectedException(typeof(CustomAttributeValidationException))]
        public void ValidateAndSetCustomAttribute_WhenDateTimeMustBeInPastAndIsNot_ThrowsCustomAttributeValidationException()
        {
            // Arrange
            var dateTimeCustomAttributeConfig = new CustomAttributeConfiguration
            {
                CustomAttributeType = CustomAttributeType.DateTime,
                PastDateOnly = true
            };

            // Act
            classUnderTest.ValidateAndSetAttributeValue(dateTimeCustomAttributeConfig, DateTime.Now.AddDays(1), "UserName");
        }

        [Test, ExpectedException(typeof(CustomAttributeValidationException))]
        public void ValidateAndSetCustomAttribute_WhenDateTimeMustBeInFutureAndIsNot_ThrowsCustomAttributeValidationException()
        {
            // Arrange
            var dateTimeCustomAttributeConfig = new CustomAttributeConfiguration
            {
                CustomAttributeType = CustomAttributeType.DateTime,
                FutureDateOnly = true
            };

            // Act
            classUnderTest.ValidateAndSetAttributeValue(dateTimeCustomAttributeConfig, DateTime.Now.AddDays(-1), "UserName");
        }
    }
}

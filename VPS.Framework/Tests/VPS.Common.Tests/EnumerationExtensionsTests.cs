using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VPS.Common.Utilities;

namespace VPS.Common.Tests
{
    [TestFixture]
    public class EnumerationExtensionsTests
    {
        [Test]
        public void IsInReturnsTrueIfCurrentEnumerationValueIsPresentInCollectionToTest()
        {
            Assert.IsTrue(TestEnum.Value1.IsIn(TestEnum.Value1, TestEnum.Value2));
        }

        [Test]
        public void IsInReturnsFalseIfCurrentEnumerationValueIsNotPresentInCollectionToTest()
        {
            Assert.IsFalse(TestEnum.Value1.IsIn(TestEnum.Value2, TestEnum.Value3));
        }

        [Test]
        public void IsInReturnsFalseIfEnumTypesAreDifferent()
        {
            Assert.IsFalse(TestEnum.Value1.IsIn(OtherTestEnum.Value1, OtherTestEnum.Value2));
        }

        private enum TestEnum
        {
            Value1,
            Value2,
            Value3
        }

        private enum OtherTestEnum
        {
            Value1,
            Value2,
            Value3
        }
    }
}

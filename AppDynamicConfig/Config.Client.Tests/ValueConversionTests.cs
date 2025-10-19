using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Config.Client.Tests
{
    public class ValueConversionTests
    {
        [Theory]
        [InlineData("string", "hello", "hello")]
        [InlineData("int", "42", 42)]
        [InlineData("double", "3.14", 3.14)]
        public void Converts_Primitives_Invariant(string type, string input, object expected)
        {
            var boxed = ConfigurationReader.ConvertBoxed(type, input);

            if (expected is double de)
            {
                Assert.Equal(de, Assert.IsType<double>(boxed), precision: 10);
            }
            else if (expected is int ie)
            {
                Assert.Equal(ie, Assert.IsType<int>(boxed));
            }
            else
            {
                Assert.Equal(expected, boxed);
            }
        }

        [Theory]
        [InlineData("true", true)]
        [InlineData("false", false)]
        [InlineData("1", true)]
        public void Converts_Bool_Variants(string input, bool expected)
        {
            var boxed = ConfigurationReader.ConvertBoxed("bool", input);
            Assert.IsType<bool>(boxed);
            Assert.Equal(expected, (bool)boxed);
        }

        [Fact]
        public void Unsupported_Type_Throws()
        {
            Assert.Throws<InvalidOperationException>(() =>
                ConfigurationReader.ConvertBoxed("date", "2025-01-01"));
        }

        [Fact]
        public void Double_Uses_InvariantCulture()
        {
            var prev = CultureInfo.CurrentCulture;
            try
            {
                CultureInfo.CurrentCulture = new CultureInfo("tr-TR");
                var boxed = ConfigurationReader.ConvertBoxed("double", "12.5");
                Assert.Equal(12.5, Assert.IsType<double>(boxed));
            }
            finally
            {
                CultureInfo.CurrentCulture = prev;
            }
        }
    }
}

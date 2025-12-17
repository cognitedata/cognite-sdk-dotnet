using System.Collections.Generic;
using CogniteSdk.Alpha;
using Xunit;

namespace Test.CSharp
{
    public class SimulatorValueTests
    {

        [Fact]
        public void TestStringArrayCreation()
        {
            var values = new List<string> { "value1", "value2", "value3" };

            var stringArray = new SimulatorValue.StringArray(values);

            Assert.NotNull(stringArray);
            Assert.Equal(SimulatorValueType.STRING_ARRAY, stringArray.Type);
            Assert.Equal(values, stringArray.Value);
        }

        [Fact]
        public void TestStringArrayCreateFactory()
        {
            var values = new List<string> { "value1", "value2" };

            var stringArray = SimulatorValue.Create(values);

            Assert.NotNull(stringArray);
            Assert.IsType<SimulatorValue.StringArray>(stringArray);
            Assert.Equal(SimulatorValueType.STRING_ARRAY, stringArray.Type);
        }

        [Fact]
        public void TestStringArrayToString()
        {
            var values = new List<string> { "value1", "value2", "value3" };
            var stringArray = new SimulatorValue.StringArray(values);

            var result = stringArray.ToString();

            Assert.Equal("value1, value2, value3", result);
        }

        [Fact]
        public void TestStringArrayToStringWithEmptyArray()
        {
            var values = new List<string>();
            var stringArray = new SimulatorValue.StringArray(values);

            var result = stringArray.ToString();

            Assert.Equal("", result);
        }

        [Fact]
        public void TestStringArrayEquals()
        {
            var values1 = new List<string> { "value1", "value2", "value3" };
            var values2 = new List<string> { "value1", "value2", "value3" };
            var stringArray1 = new SimulatorValue.StringArray(values1);
            var stringArray2 = new SimulatorValue.StringArray(values2);

            Assert.True(stringArray1.Equals(stringArray2));
        }

        [Fact]
        public void TestStringArrayNotEqualsDifferentContent()
        {
            var values1 = new List<string> { "value1", "value2" };
            var values2 = new List<string> { "value1", "value3" };
            var stringArray1 = new SimulatorValue.StringArray(values1);
            var stringArray2 = new SimulatorValue.StringArray(values2);

            Assert.False(stringArray1.Equals(stringArray2));
        }

        [Fact]
        public void TestStringArrayNotEqualsDifferentLength()
        {
            var values1 = new List<string> { "value1", "value2", "value3" };
            var values2 = new List<string> { "value1", "value2" };
            var stringArray1 = new SimulatorValue.StringArray(values1);
            var stringArray2 = new SimulatorValue.StringArray(values2);

            Assert.False(stringArray1.Equals(stringArray2));
        }

        [Fact]
        public void TestStringArrayNotEqualsNull()
        {
            var values = new List<string> { "value1", "value2" };
            var stringArray = new SimulatorValue.StringArray(values);

            Assert.False(stringArray.Equals(null));
        }

        [Fact]
        public void TestStringArrayNotEqualsDifferentType()
        {
            var stringValues = new List<string> { "value1", "value2" };
            var doubleValues = new List<double> { 1.0, 2.0 };
            var stringArray = new SimulatorValue.StringArray(stringValues);
            var doubleArray = new SimulatorValue.DoubleArray(doubleValues);

            Assert.False(stringArray.Equals(doubleArray));
        }

        [Fact]
        public void TestStringArrayGetHashCode()
        {
            var values1 = new List<string> { "value1", "value2", "value3" };
            var values2 = new List<string> { "value1", "value2", "value3" };
            var stringArray1 = new SimulatorValue.StringArray(values1);
            var stringArray2 = new SimulatorValue.StringArray(values2);

            var hash1 = stringArray1.GetHashCode();
            var hash2 = stringArray2.GetHashCode();

            Assert.Equal(hash1, hash2);
        }

        [Fact]
        public void TestStringArrayGetHashCodeDifferent()
        {
            var values1 = new List<string> { "x", "y", "z" };
            var values2 = new List<string> { "a", "b", "c" };
            var stringArray1 = new SimulatorValue.StringArray(values1);
            var stringArray2 = new SimulatorValue.StringArray(values2);

            var hash1 = stringArray1.GetHashCode();
            var hash2 = stringArray2.GetHashCode();

            Assert.NotEqual(hash1, hash2);
        }

        [Fact]
        public void TestDoubleArrayCreation()
        {
            var values = new List<double> { 1.5, 2.7, 3.9 };

            var doubleArray = new SimulatorValue.DoubleArray(values);

            Assert.NotNull(doubleArray);
            Assert.Equal(SimulatorValueType.DOUBLE_ARRAY, doubleArray.Type);
            Assert.Equal(values, doubleArray.Value);
        }

        [Fact]
        public void TestDoubleArrayCreateFactory()
        {
            var values = new List<double> { 10.5, 20.3 };

            var doubleArray = SimulatorValue.Create(values);

            Assert.NotNull(doubleArray);
            Assert.IsType<SimulatorValue.DoubleArray>(doubleArray);
            Assert.Equal(SimulatorValueType.DOUBLE_ARRAY, doubleArray.Type);
        }

        [Fact]
        public void TestDoubleArrayToString()
        {
            var values = new List<double> { 1.5, 2.5, 3.5 };
            var doubleArray = new SimulatorValue.DoubleArray(values);

            var result = doubleArray.ToString();

            Assert.Equal("1.5, 2.5, 3.5", result);
        }

        [Fact]
        public void TestDoubleArrayToStringWithEmptyArray()
        {
            var values = new List<double>();
            var doubleArray = new SimulatorValue.DoubleArray(values);

            var result = doubleArray.ToString();

            Assert.Equal("", result);
        }

        [Fact]
        public void TestDoubleArrayEquals()
        {
            var values1 = new List<double> { 1.1, 2.2, 3.3 };
            var values2 = new List<double> { 1.1, 2.2, 3.3 };
            var doubleArray1 = new SimulatorValue.DoubleArray(values1);
            var doubleArray2 = new SimulatorValue.DoubleArray(values2);

            Assert.True(doubleArray1.Equals(doubleArray2));
        }

        [Fact]
        public void TestDoubleArrayNotEqualsDifferentContent()
        {
            var values1 = new List<double> { 1.1, 2.2 };
            var values2 = new List<double> { 1.1, 3.3 };
            var doubleArray1 = new SimulatorValue.DoubleArray(values1);
            var doubleArray2 = new SimulatorValue.DoubleArray(values2);

            Assert.False(doubleArray1.Equals(doubleArray2));
        }

        [Fact]
        public void TestDoubleArrayNotEqualsDifferentLength()
        {
            var values1 = new List<double> { 1.1, 2.2, 3.3 };
            var values2 = new List<double> { 1.1, 2.2 };
            var doubleArray1 = new SimulatorValue.DoubleArray(values1);
            var doubleArray2 = new SimulatorValue.DoubleArray(values2);

            Assert.False(doubleArray1.Equals(doubleArray2));
        }

        [Fact]
        public void TestDoubleArrayNotEqualsNull()
        {
            var values = new List<double> { 1.1, 2.2 };
            var doubleArray = new SimulatorValue.DoubleArray(values);

            Assert.False(doubleArray.Equals(null));
        }

        [Fact]
        public void TestDoubleArrayNotEqualsDifferentType()
        {
            var doubleValues = new List<double> { 1.0, 2.0 };
            var stringValues = new List<string> { "1", "2" };
            var doubleArray = new SimulatorValue.DoubleArray(doubleValues);
            var stringArray = new SimulatorValue.StringArray(stringValues);

            Assert.False(doubleArray.Equals(stringArray));
        }

        [Fact]
        public void TestDoubleArrayGetHashCode()
        {
            var values1 = new List<double> { 5.5, 6.6, 7.7 };
            var values2 = new List<double> { 5.5, 6.6, 7.7 };
            var doubleArray1 = new SimulatorValue.DoubleArray(values1);
            var doubleArray2 = new SimulatorValue.DoubleArray(values2);

            var hash1 = doubleArray1.GetHashCode();
            var hash2 = doubleArray2.GetHashCode();

            Assert.Equal(hash1, hash2);
        }

        [Fact]
        public void TestDoubleArrayGetHashCodeDifferent()
        {
            var values1 = new List<double> { 1.0, 2.0, 3.0 };
            var values2 = new List<double> { 4.0, 5.0, 6.0 };
            var doubleArray1 = new SimulatorValue.DoubleArray(values1);
            var doubleArray2 = new SimulatorValue.DoubleArray(values2);

            var hash1 = doubleArray1.GetHashCode();
            var hash2 = doubleArray2.GetHashCode();

            Assert.NotEqual(hash1, hash2);
        }

    }
}


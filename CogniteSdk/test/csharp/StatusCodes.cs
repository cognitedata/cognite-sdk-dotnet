using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CogniteSdk;
using Xunit;
using ValueType = CogniteSdk.ValueType;

namespace Test.CSharp
{
    public class StatusCodeTests
    {
        [Fact]
        public void TestFromCategory()
        {
            // Just check that this doesn't fail.
            Assert.Equal("Good", StatusCode.FromCategory(StatusCodeCategory.Good).ToString());
            Assert.Equal("BadBrowseDirectionInvalid", StatusCode.FromCategory(StatusCodeCategory.BadBrowseDirectionInvalid).ToString());
            Assert.Equal("UncertainDataSubNormal", StatusCode.FromCategory(StatusCodeCategory.UncertainDataSubNormal).ToString());
        }

        [Fact]
        public void TestParse()
        {
            Assert.Equal("Good, StructureChanged, Calculated", StatusCode.Parse("Good, StructureChanged, Calculated").ToString());
            Assert.Equal("UncertainSensorCalibration, Overflow, ExtraData",
                StatusCode.Parse("UncertainSensorCalibration, Overflow, ExtraData").ToString());

            Assert.Equal("Good", StatusCode.Create(0).ToString());

            Assert.Throws<InvalidStatusCodeException>(() => StatusCode.Create(12345));
            Assert.Throws<InvalidStatusCodeException>(() => StatusCode.Parse("Bad, Whoop"));
        }

        [Fact]
        public void TestModify()
        {
            var code = StatusCode.Create(0);
            Assert.Equal(Severity.Good, code.Severity);
            code.Severity = Severity.Uncertain;
            Assert.Equal(Severity.Uncertain, code.Severity);
            code.Severity = Severity.Bad;
            Assert.Equal(Severity.Bad, code.Severity);


            Assert.False(code.StructureChanged);
            code.StructureChanged = true;
            Assert.True(code.StructureChanged);
            code.StructureChanged = false;
            Assert.False(code.StructureChanged);
            code.StructureChanged = true;

            Assert.False(code.SemanticsChanged);
            code.SemanticsChanged = true;
            Assert.True(code.SemanticsChanged);
            code.SemanticsChanged = false;
            Assert.False(code.SemanticsChanged);
            code.SemanticsChanged = true;

            Assert.False(code.IsDataValueInfoType);
            code.IsDataValueInfoType = true;
            Assert.True(code.IsDataValueInfoType);
            code.IsDataValueInfoType = false;
            Assert.False(code.IsDataValueInfoType);
            code.IsDataValueInfoType = true;

            Assert.Equal(Limit.None, code.Limit);
            code.Limit = Limit.High;
            Assert.Equal(Limit.High, code.Limit);
            code.Limit = Limit.Constant;
            Assert.Equal(Limit.Constant, code.Limit);

            Assert.Equal(StatusCodeCategory.Bad, code.Category);
            code.Category = StatusCodeCategory.BadAggregateInvalidInputs;
            Assert.Equal(StatusCodeCategory.BadAggregateInvalidInputs, code.Category);
            code.Category = StatusCodeCategory.UncertainDataSubNormal;
            Assert.Equal(StatusCodeCategory.UncertainDataSubNormal, code.Category);

            Assert.False(code.IsOverflow);
            code.IsOverflow = true;
            Assert.True(code.IsOverflow);
            code.IsOverflow = false;
            Assert.False(code.IsOverflow);
            code.IsOverflow = true;

            Assert.False(code.IsMultiValue);
            code.IsMultiValue = true;
            Assert.True(code.IsMultiValue);
            code.IsMultiValue = false;
            Assert.False(code.IsMultiValue);
            code.IsMultiValue = true;

            Assert.False(code.HasExtraData);
            code.HasExtraData = true;
            Assert.True(code.HasExtraData);
            code.HasExtraData = false;
            Assert.False(code.HasExtraData);
            code.HasExtraData = true;

            Assert.False(code.IsPartial);
            code.IsPartial = true;
            Assert.True(code.IsPartial);
            code.IsPartial = false;
            Assert.False(code.IsPartial);
            code.IsPartial = true;

            Assert.Equal(ValueType.Raw, code.ValueType);
            code.ValueType = ValueType.Calculated;
            Assert.Equal(ValueType.Calculated, code.ValueType);
            code.ValueType = ValueType.Interpolated;
            Assert.Equal(ValueType.Interpolated, code.ValueType);


            Assert.Equal(Severity.Uncertain, code.Severity);
            Assert.True(code.StructureChanged);
            Assert.True(code.SemanticsChanged);
            Assert.True(code.IsDataValueInfoType);
            Assert.Equal(Limit.Constant, code.Limit);
            Assert.Equal(StatusCodeCategory.UncertainDataSubNormal, code.Category);
            Assert.True(code.IsOverflow);
            Assert.True(code.IsMultiValue);
            Assert.True(code.HasExtraData);
            Assert.True(code.IsPartial);
            Assert.Equal(ValueType.Interpolated, code.ValueType);
        }

    }
}

// Copyright 2026 Cognite AS
// SPDX-License-Identifier: Apache-2.0

using System.Text.Json;
using CogniteSdk;
using Xunit;

namespace Test.CSharp
{
    public class MultiValueConverterTests
    {
        [Fact]
        public void TestWriteNullMultiValue()
        {
            MultiValue nullRef = null;
            var json = JsonSerializer.Serialize(nullRef, Oryx.Cognite.Common.jsonOptions);
            Assert.Equal("null", json);
        }

        [Fact]
        public void TestWriteNullSentinel()
        {
            var json = JsonSerializer.Serialize(MultiValue.Create(), Oryx.Cognite.Common.jsonOptions);
            Assert.Equal("null", json);
        }

        [Fact]
        public void TestWriteAndReadMixedArrayWithNulls()
        {
            var values = new MultiValue[]
            {
                null,
                MultiValue.Create(),
                MultiValue.Create("hello"),
                MultiValue.Create(1.5),
                MultiValue.Create(42L)
            };

            var json = JsonSerializer.Serialize(values, Oryx.Cognite.Common.jsonOptions);
            Assert.Equal(@"[null,null,""hello"",1.5,42]", json);

            var roundTrip = JsonSerializer.Deserialize<MultiValue[]>(json, Oryx.Cognite.Common.jsonOptions);
            Assert.Equal(5, roundTrip.Length);
            Assert.IsType<MultiValue.Null>(roundTrip[0]);
            Assert.IsType<MultiValue.Null>(roundTrip[1]);
            Assert.Equal("hello", ((MultiValue.String)roundTrip[2]).Value);
            Assert.Equal(1.5, ((MultiValue.Double)roundTrip[3]).Value);
            Assert.Equal(42L, ((MultiValue.Long)roundTrip[4]).Value);
        }
    }
}

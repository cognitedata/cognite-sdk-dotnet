using System.Text.Json;
using CogniteSdk.Alpha;
using Xunit;

namespace Test.CSharp
{
    public class SimulatorRoutineRevisionTests
    {
        [Fact]
        public void TestLogicalCheckSerializationWithAllFields()
        {
            var original = new SimulatorRoutineRevisionLogicalCheck
            {
                Enabled = true,
                TimeseriesExternalId = "timeseries-1",
                Aggregate = "avg",
                Operator = "gt",
                Value = 100.5
            };

            var json = JsonSerializer.Serialize(original);
            var deserialized = JsonSerializer.Deserialize<SimulatorRoutineRevisionLogicalCheck>(json);

            Assert.NotNull(deserialized);
            Assert.Equal(original.Enabled, deserialized.Enabled);
            Assert.Equal(original.TimeseriesExternalId, deserialized.TimeseriesExternalId);
            Assert.Equal(original.Aggregate, deserialized.Aggregate);
            Assert.Equal(original.Operator, deserialized.Operator);
            Assert.Equal(original.Value, deserialized.Value);
        }

        [Fact]
        public void TestLogicalCheckSerializationWithNullValue()
        {
            var original = new SimulatorRoutineRevisionLogicalCheck
            {
                Enabled = false,
                TimeseriesExternalId = "ts-123",
                Aggregate = "max",
                Operator = "lt",
                Value = null
            };

            var json = JsonSerializer.Serialize(original);
            var deserialized = JsonSerializer.Deserialize<SimulatorRoutineRevisionLogicalCheck>(json);

            Assert.NotNull(deserialized);
            Assert.Equal(original.Enabled, deserialized.Enabled);
            Assert.Equal(original.TimeseriesExternalId, deserialized.TimeseriesExternalId);
            Assert.Equal(original.Aggregate, deserialized.Aggregate);
            Assert.Equal(original.Operator, deserialized.Operator);
            Assert.Null(deserialized.Value);
        }

        [Fact]
        public void TestSteadyStateDetectionSerializationWithAllFields()
        {
            var original = new SimulatorRoutineRevisionSteadyStateDetection
            {
                Enabled = true,
                TimeseriesExternalId = "timeseries-steady-1",
                Aggregate = "avg",
                MinSectionSize = 10,
                VarThreshold = 0.05,
                SlopeThreshold = 0.01
            };

            var json = JsonSerializer.Serialize(original);
            var deserialized = JsonSerializer.Deserialize<SimulatorRoutineRevisionSteadyStateDetection>(json);

            Assert.NotNull(deserialized);
            Assert.Equal(original.Enabled, deserialized.Enabled);
            Assert.Equal(original.TimeseriesExternalId, deserialized.TimeseriesExternalId);
            Assert.Equal(original.Aggregate, deserialized.Aggregate);
            Assert.Equal(original.MinSectionSize, deserialized.MinSectionSize);
            Assert.Equal(original.VarThreshold, deserialized.VarThreshold);
            Assert.Equal(original.SlopeThreshold, deserialized.SlopeThreshold);
        }

        [Fact]
        public void TestSteadyStateDetectionSerializationWithNullFields()
        {
            var original = new SimulatorRoutineRevisionSteadyStateDetection
            {
                Enabled = false,
                TimeseriesExternalId = "ts-steady-456",
                Aggregate = "min",
                MinSectionSize = null,
                VarThreshold = 0.1,
                SlopeThreshold = null
            };

            var json = JsonSerializer.Serialize(original);
            var deserialized = JsonSerializer.Deserialize<SimulatorRoutineRevisionSteadyStateDetection>(json);

            Assert.NotNull(deserialized);
            Assert.Equal(original.Enabled, deserialized.Enabled);
            Assert.Equal(original.TimeseriesExternalId, deserialized.TimeseriesExternalId);
            Assert.Equal(original.Aggregate, deserialized.Aggregate);
            Assert.Null(deserialized.MinSectionSize);
            Assert.Equal(original.VarThreshold, deserialized.VarThreshold);
            Assert.Null(deserialized.SlopeThreshold);
        }

    }
}


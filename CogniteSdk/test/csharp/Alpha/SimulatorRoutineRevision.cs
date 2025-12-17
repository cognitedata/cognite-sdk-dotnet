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
            var expectedObject = new SimulatorRoutineRevisionLogicalCheck
            {
                Enabled = true,
                TimeseriesExternalId = "timeseries-1",
                Aggregate = "avg",
                Operator = "gt",
                Value = 100.5
            };

            var json = "{\"Enabled\":true,\"TimeseriesExternalId\":\"timeseries-1\",\"Aggregate\":\"avg\",\"Operator\":\"gt\",\"Value\":100.5}";

            var deserialized = JsonSerializer.Deserialize<SimulatorRoutineRevisionLogicalCheck>(json);
            
            Assert.NotNull(deserialized);
            Assert.Equal(expectedObject.Enabled, deserialized.Enabled);
            Assert.Equal(expectedObject.TimeseriesExternalId, deserialized.TimeseriesExternalId);
            Assert.Equal(expectedObject.Aggregate, deserialized.Aggregate);
            Assert.Equal(expectedObject.Operator, deserialized.Operator);
            Assert.Equal(expectedObject.Value, deserialized.Value);

            var serializedJson = JsonSerializer.Serialize(expectedObject);
            using var expectedDoc = JsonDocument.Parse(json);
            using var actualDoc = JsonDocument.Parse(serializedJson);
            Assert.Equal(expectedDoc.RootElement.GetRawText(), actualDoc.RootElement.GetRawText());
        }

        [Fact]
        public void TestLogicalCheckSerializationWithNullValue()
        {
            var expectedObject = new SimulatorRoutineRevisionLogicalCheck
            {
                Enabled = false,
                TimeseriesExternalId = "ts-123",
                Aggregate = "max",
                Operator = "lt",
                Value = null
            };

            var json = "{\"Enabled\":false,\"TimeseriesExternalId\":\"ts-123\",\"Aggregate\":\"max\",\"Operator\":\"lt\",\"Value\":null}";

            var deserialized = JsonSerializer.Deserialize<SimulatorRoutineRevisionLogicalCheck>(json);

            Assert.NotNull(deserialized);
            Assert.Equal(expectedObject.Enabled, deserialized.Enabled);
            Assert.Equal(expectedObject.TimeseriesExternalId, deserialized.TimeseriesExternalId);
            Assert.Equal(expectedObject.Aggregate, deserialized.Aggregate);
            Assert.Equal(expectedObject.Operator, deserialized.Operator);
            Assert.Null(deserialized.Value);

            var serializedJson = JsonSerializer.Serialize(expectedObject);
            using var expectedDoc = JsonDocument.Parse(json);
            using var actualDoc = JsonDocument.Parse(serializedJson);
            Assert.Equal(expectedDoc.RootElement.GetRawText(), actualDoc.RootElement.GetRawText());
        }

        [Fact]
        public void TestSteadyStateDetectionSerializationWithAllFields()
        {
            var expectedObject = new SimulatorRoutineRevisionSteadyStateDetection
            {
                Enabled = true,
                TimeseriesExternalId = "timeseries-steady-1",
                Aggregate = "avg",
                MinSectionSize = 10,
                VarThreshold = 0.05,
                SlopeThreshold = 0.01
            };

            var json = "{\"Enabled\":true,\"TimeseriesExternalId\":\"timeseries-steady-1\",\"Aggregate\":\"avg\",\"MinSectionSize\":10,\"VarThreshold\":0.05,\"SlopeThreshold\":0.01}";

            var deserialized = JsonSerializer.Deserialize<SimulatorRoutineRevisionSteadyStateDetection>(json);

            Assert.NotNull(deserialized);
            Assert.Equal(expectedObject.Enabled, deserialized.Enabled);
            Assert.Equal(expectedObject.TimeseriesExternalId, deserialized.TimeseriesExternalId);
            Assert.Equal(expectedObject.Aggregate, deserialized.Aggregate);
            Assert.Equal(expectedObject.MinSectionSize, deserialized.MinSectionSize);
            Assert.Equal(expectedObject.VarThreshold, deserialized.VarThreshold);
            Assert.Equal(expectedObject.SlopeThreshold, deserialized.SlopeThreshold);

            var serializedJson = JsonSerializer.Serialize(expectedObject);
            using var expectedDoc = JsonDocument.Parse(json);
            using var actualDoc = JsonDocument.Parse(serializedJson);
            Assert.Equal(expectedDoc.RootElement.GetRawText(), actualDoc.RootElement.GetRawText());
        }

        [Fact]
        public void TestSteadyStateDetectionSerializationWithNullFields()
        {
            var expectedObject = new SimulatorRoutineRevisionSteadyStateDetection
            {
                Enabled = false,
                TimeseriesExternalId = "ts-steady-456",
                Aggregate = "min",
                MinSectionSize = null,
                VarThreshold = 0.1,
                SlopeThreshold = null
            };

            var json = "{\"Enabled\":false,\"TimeseriesExternalId\":\"ts-steady-456\",\"Aggregate\":\"min\",\"MinSectionSize\":null,\"VarThreshold\":0.1,\"SlopeThreshold\":null}";

            var deserialized = JsonSerializer.Deserialize<SimulatorRoutineRevisionSteadyStateDetection>(json);

            Assert.NotNull(deserialized);
            Assert.Equal(expectedObject.Enabled, deserialized.Enabled);
            Assert.Equal(expectedObject.TimeseriesExternalId, deserialized.TimeseriesExternalId);
            Assert.Equal(expectedObject.Aggregate, deserialized.Aggregate);
            Assert.Null(deserialized.MinSectionSize);
            Assert.Equal(expectedObject.VarThreshold, deserialized.VarThreshold);
            Assert.Null(deserialized.SlopeThreshold);

            var serializedJson = JsonSerializer.Serialize(expectedObject);
            using var expectedDoc = JsonDocument.Parse(json);
            using var actualDoc = JsonDocument.Parse(serializedJson);
            Assert.Equal(expectedDoc.RootElement.GetRawText(), actualDoc.RootElement.GetRawText());
        }

    }
}


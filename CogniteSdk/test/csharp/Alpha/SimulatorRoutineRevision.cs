using CogniteSdk.Alpha;
using Xunit;

namespace Test.CSharp
{
    public class SimulatorRoutineRevisionTests
    {
        [Fact]
        public void TestLogicalCheckCreation()
        {
            var logicalCheck = new SimulatorRoutineRevisionLogicalCheck
            {
                Enabled = true,
                TimeseriesExternalId = "timeseries-1",
                Aggregate = "avg",
                Operator = "gt",
                Value = 100.5
            };

            Assert.True(logicalCheck.Enabled);
            Assert.Equal("timeseries-1", logicalCheck.TimeseriesExternalId);
            Assert.Equal("avg", logicalCheck.Aggregate);
            Assert.Equal("gt", logicalCheck.Operator);
            Assert.Equal(100.5, logicalCheck.Value);
        }

        [Fact]
        public void TestLogicalCheckDisabled()
        {
            var logicalCheck = new SimulatorRoutineRevisionLogicalCheck
            {
                Enabled = false
            };

            Assert.False(logicalCheck.Enabled);
        }

        [Fact]
        public void TestLogicalCheckToString()
        {
            var logicalCheck = new SimulatorRoutineRevisionLogicalCheck
            {
                Enabled = true,
                TimeseriesExternalId = "ts-123",
                Aggregate = "max",
                Operator = "lt",
                Value = 50.0
            };

            var result = logicalCheck.ToString();

            Assert.NotNull(result);
            Assert.NotEmpty(result);
        }

        [Fact]
        public void TestLogicalCheckWithEmptyStrings()
        {
            var logicalCheck = new SimulatorRoutineRevisionLogicalCheck
            {
                Enabled = true,
                TimeseriesExternalId = "",
                Aggregate = "",
                Operator = ""
            };

            Assert.Equal("", logicalCheck.TimeseriesExternalId);
            Assert.Equal("", logicalCheck.Aggregate);
            Assert.Equal("", logicalCheck.Operator);
        }

        [Fact]
        public void TestSteadyStateDetectionCreation()
        {
            var steadyState = new SimulatorRoutineRevisionSteadyStateDetection
            {
                Enabled = true,
                TimeseriesExternalId = "timeseries-steady-1",
                Aggregate = "avg",
                MinSectionSize = 10,
                VarThreshold = 0.05,
                SlopeThreshold = 0.01
            };

            Assert.True(steadyState.Enabled);
            Assert.Equal("timeseries-steady-1", steadyState.TimeseriesExternalId);
            Assert.Equal("avg", steadyState.Aggregate);
            Assert.Equal(10, steadyState.MinSectionSize);
            Assert.Equal(0.05, steadyState.VarThreshold);
            Assert.Equal(0.01, steadyState.SlopeThreshold);
        }

        [Fact]
        public void TestSteadyStateDetectionDisabled()
        {
            var steadyState = new SimulatorRoutineRevisionSteadyStateDetection
            {
                Enabled = false
            };

            Assert.False(steadyState.Enabled);
        }

        [Fact]
        public void TestSteadyStateDetectionToString()
        {
            var steadyState = new SimulatorRoutineRevisionSteadyStateDetection
            {
                Enabled = true,
                TimeseriesExternalId = "ts-steady-456",
                Aggregate = "min",
                MinSectionSize = 5,
                VarThreshold = 0.1,
                SlopeThreshold = 0.02
            };

            var result = steadyState.ToString();

            Assert.NotNull(result);
            Assert.NotEmpty(result);
        }

        [Fact]
        public void TestSteadyStateDetectionWithEmptyStrings()
        {
            var steadyState = new SimulatorRoutineRevisionSteadyStateDetection
            {
                Enabled = false,
                TimeseriesExternalId = "",
                Aggregate = ""
            };

            Assert.Equal("", steadyState.TimeseriesExternalId);
            Assert.Equal("", steadyState.Aggregate);
        }

    }
}


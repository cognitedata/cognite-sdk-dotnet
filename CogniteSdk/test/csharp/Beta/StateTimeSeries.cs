// Copyright 2026 Cognite AS
// SPDX-License-Identifier: Apache-2.0

using System;
using System.Linq;
using System.Threading.Tasks;
using CogniteSdk;
using CogniteSdk.DataModels;
using Com.Cognite.V1.Timeseries.Proto;
using Xunit;

namespace Test.CSharp.Integration.Beta
{
    public class StateTimeSeriesFixture : TestFixture, IAsyncLifetime
    {
        public Client Write => WriteClient;
        public string TestSpace { get; private set; }

        public override async Task InitializeAsync()
        {
            TestSpace = $"{Prefix}StateTsSpace";
            await Write.DataModels.UpsertSpaces(new[] { new SpaceCreate { Space = TestSpace } });
        }

        public override async Task DisposeAsync()
        {
            try { await Write.DataModels.DeleteSpaces(new[] { TestSpace }); }
            catch { /* best-effort */ }
        }
    }

    public class StateTimeSeriesTests : IClassFixture<StateTimeSeriesFixture>
    {
        private static readonly ViewIdentifier StateSetView = new("cdf_cdm", "CogniteStateSet", "v1");
        private static readonly ViewIdentifier TimeSeriesView = new("cdf_cdm", "CogniteTimeSeries", "v1");

        private readonly StateTimeSeriesFixture _fx;

        public StateTimeSeriesTests(StateTimeSeriesFixture fx) => _fx = fx;

        [Fact]
        public async Task UpsertStateSetIngestAndQueryDatapoints()
        {
            var space = _fx.TestSpace;
            var stateSetXid = "valve_states_" + Guid.NewGuid().ToString("N");
            var tsXid = "valve_001_state_" + Guid.NewGuid().ToString("N");

            // State sets and state time series are private beta — both the
            // instance upserts and the data point operations need to go
            // through the beta surface so the `cdf-version: beta` header is
            // attached to every request.

            // Create the state set
            await _fx.Write.Beta.DataModels.UpsertInstances(new InstanceWriteRequest
            {
                Replace = true,
                Items = new BaseInstanceWrite[]
                {
                    new NodeWrite
                    {
                        Space = space,
                        ExternalId = stateSetXid,
                        Sources = new InstanceData[] { new InstanceData<object>
                        {
                            Source = StateSetView,
                            Properties = new
                            {
                                name = "Valve Position States",
                                description = "Standard position states for industrial valves",
                                states = new[]
                                {
                                    new { numericValue = 0, stringValue = "CLOSED" },
                                    new { numericValue = 1, stringValue = "OPEN" },
                                    new { numericValue = 2, stringValue = "TRANSITIONING" }
                                }
                            }
                        }}
                    }
                }
            });

            // Create the state time series
            await _fx.Write.Beta.DataModels.UpsertInstances(new InstanceWriteRequest
            {
                Replace = true,
                Items = new BaseInstanceWrite[]
                {
                    new NodeWrite
                    {
                        Space = space,
                        ExternalId = tsXid,
                        Sources = new InstanceData[] { new InstanceData<object>
                        {
                            Source = TimeSeriesView,
                            Properties = new
                            {
                                name = "Valve 001 Position",
                                description = "Integration test state time series",
                                type = "state",
                                stateSet = new { space, externalId = stateSetXid }
                            }
                        }}
                    }
                }
            });

            try
            {
                // Ingest some state datapoints
                var datapoints = new StateDatapoints();
                datapoints.Datapoints.Add(new StateDatapoint { Timestamp = 1609459200000L, NumericValue = 0L, StringValue = "CLOSED" });
                datapoints.Datapoints.Add(new StateDatapoint { Timestamp = 1609462800000L, NumericValue = 1L, StringValue = "OPEN" });
                datapoints.Datapoints.Add(new StateDatapoint { Timestamp = 1609466400000L, NumericValue = 0L, StringValue = "CLOSED" });

                var insertion = new DataPointInsertionRequest();
                insertion.Items.Add(new DataPointInsertionItem
                {
                    InstanceId = new InstanceId { Space = space, ExternalId = tsXid },
                    StateDatapoints = datapoints
                });

                await _fx.Write.Beta.DataPoints.CreateAsync(insertion);

                var item = new[] { new DataPointsQueryItem { InstanceId = new InstanceIdentifier(space, tsXid) } };

                // Raw query
                var raw = (await _fx.Write.Beta.DataPoints.ListAsync(new DataPointsQuery
                {
                    Start = "1609459200000",
                    End = "1609545600000",
                    Items = item
                })).Items.First();

                Assert.Equal(DataPointListItem.DatapointTypeOneofCase.StateDatapoints, raw.DatapointTypeCase);
                Assert.Equal(3, raw.StateDatapoints.Datapoints.Count);

                // Aggregate query
                var agg = (await _fx.Write.Beta.DataPoints.ListAsync(new DataPointsQuery
                {
                    Start = "1609459200000",
                    End = "1609545600000",
                    Granularity = "1d",
                    Aggregates = new[] { "stateCount", "stateTransitions", "stateDuration" },
                    Items = item
                })).Items.First();

                Assert.Equal(DataPointListItem.DatapointTypeOneofCase.AggregateDatapoints, agg.DatapointTypeCase);
                var stateAggregates = agg.AggregateDatapoints.Datapoints.First().StateAggregates;
                Assert.NotEmpty(stateAggregates);

                var closed = stateAggregates.Single(s => s.NumericValue == 0L);
                var open = stateAggregates.Single(s => s.NumericValue == 1L);
                Assert.Equal("CLOSED", closed.StringValue);
                Assert.Equal("OPEN", open.StringValue);
                Assert.Equal(2L, closed.StateCount);
                Assert.Equal(1L, open.StateCount);
                Assert.Equal(2L, closed.StateTransitions);
                Assert.Equal(1L, open.StateTransitions);
                Assert.Equal(3600000L, open.StateDuration);
                Assert.True(closed.StateDuration > 0L);
            }
            finally
            {
                await _fx.Write.DataModels.DeleteInstances(new[]
                {
                    new InstanceIdentifierWithType(InstanceType.node, new InstanceIdentifier(space, tsXid)),
                    new InstanceIdentifierWithType(InstanceType.node, new InstanceIdentifier(space, stateSetXid))
                });
            }
        }
    }
}

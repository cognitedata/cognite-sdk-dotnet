// Copyright 2026 Cognite AS
// SPDX-License-Identifier: Apache-2.0

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CogniteSdk;
using CogniteSdk.DataModels;
using CogniteSdk.DataModels.Core;
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
        private readonly StateTimeSeriesFixture _fx;

        public StateTimeSeriesTests(StateTimeSeriesFixture fx) => _fx = fx;

        [Fact]
        public async Task UpsertStateSetIngestAndQueryDatapoints()
        {
            var space = _fx.TestSpace;
            var stateSetXid = "valve_states_" + Guid.NewGuid().ToString("N");
            var tsXid = "valve_001_state_" + Guid.NewGuid().ToString("N");

            var stateSetId = new InstanceIdentifierWithType(InstanceType.node, new InstanceIdentifier(space, stateSetXid));
            var tsId = new InstanceIdentifierWithType(InstanceType.node, new InstanceIdentifier(space, tsXid));

            try
            {
                await UpsertStateSetAndStateTimeSeries(
                    space, stateSetXid, tsXid,
                    stateSetName: "Valve Position States",
                    states: new[]
                    {
                        new CogniteState { NumericValue = 0, StringValue = "CLOSED" },
                        new CogniteState { NumericValue = 1, StringValue = "OPEN" },
                        new CogniteState { NumericValue = 2, StringValue = "TRANSITIONING" }
                    },
                    tsName: "Valve 001 Position",
                    stateSetDescription: "Standard position states for industrial valves",
                    tsDescription: "Integration test state time series");

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

                // Latest data point should return the most recent state with both numeric and string values populated
                var latest = (await _fx.Write.Beta.DataPoints.LatestAsync(new DataPointsLatestQuery
                {
                    Items = new[] { IdentityWithBefore.Create(new InstanceIdentifier(space, tsXid)) }
                })).Items.First();

                Assert.Equal(DataPointListItem.DatapointTypeOneofCase.StateDatapoints, latest.DatapointTypeCase);
                var latestPoint = latest.StateDatapoints.Datapoints.Single();
                Assert.Equal(1609466400000L, latestPoint.Timestamp);
                Assert.Equal(0L, latestPoint.NumericValue);
                Assert.Equal("CLOSED", latestPoint.StringValue);
            }
            finally
            {
                await _fx.Write.DataModels.DeleteInstances(new[] { tsId, stateSetId });
            }
        }

        /// <summary>
        /// Upsert a state set and a state time series referencing it, using the beta state set and
        /// time series resources. Shared setup for tests that need a ready-to-use state set/time series pair.
        /// </summary>
        private async Task UpsertStateSetAndStateTimeSeries(
            string space,
            string stateSetXid,
            string tsXid,
            string stateSetName,
            IEnumerable<CogniteState> states,
            string tsName,
            string stateSetDescription = null,
            string tsDescription = null)
        {
            await _fx.Write.Beta.StateSets.UpsertAsync(new[]
            {
                new SourcedNodeWrite<CogniteStateSet>
                {
                    Space = space,
                    ExternalId = stateSetXid,
                    Properties = new CogniteStateSet
                    {
                        Name = stateSetName,
                        Description = stateSetDescription,
                        States = states
                    }
                }
            });

            // State time series are only available in beta, so this must go through the beta API.
            await _fx.Write.Beta.TimeSeries.UpsertAsync(new[]
            {
                new SourcedNodeWrite<CogniteTimeSeriesBase>
                {
                    Space = space,
                    ExternalId = tsXid,
                    Properties = new CogniteTimeSeriesBase
                    {
                        Name = tsName,
                        Description = tsDescription,
                        Type = CogniteSdk.DataModels.Core.TimeSeriesType.State,
                        StateSet = new DirectRelationIdentifier(space, stateSetXid)
                    }
                }
            }, new UpsertOptions { Replace = true });
        }

        [Fact]
        public async Task UpsertAndRetrieveStateSetAndTimeSeriesTyped()
        {
            var space = _fx.TestSpace;
            var stateSetXid = "valve_states_typed_" + Guid.NewGuid().ToString("N");
            var tsXid = "valve_001_state_typed_" + Guid.NewGuid().ToString("N");

            var stateSetId = new InstanceIdentifierWithType(InstanceType.node, new InstanceIdentifier(space, stateSetXid));
            var tsId = new InstanceIdentifierWithType(InstanceType.node, new InstanceIdentifier(space, tsXid));

            var stateSets = _fx.Write.Beta.StateSets;

            try
            {
                await UpsertStateSetAndStateTimeSeries(
                    space, stateSetXid, tsXid,
                    stateSetName: "Valve Position States",
                    states: new[]
                    {
                        new CogniteState { NumericValue = 0, StringValue = "CLOSED" },
                        new CogniteState { NumericValue = 1, StringValue = "OPEN" },
                        new CogniteState { NumericValue = 2, StringValue = "TRANSITIONING" }
                    },
                    tsName: "Valve 001 Position",
                    stateSetDescription: "Standard position states for industrial valves",
                    tsDescription: "Typed state time series round-trip test");

                // Retrieve the state set and assert its states round-trip.
                var retrievedStateSet = (await stateSets.RetrieveAsync(new[] { stateSetId })).Single().Properties;
                Assert.Equal("Valve Position States", retrievedStateSet.Name);
                var states = retrievedStateSet.States.ToList();
                Assert.Equal(3, states.Count);
                Assert.Contains(states, s => s.NumericValue == 0 && s.StringValue == "CLOSED");
                Assert.Contains(states, s => s.NumericValue == 1 && s.StringValue == "OPEN");
                Assert.Contains(states, s => s.NumericValue == 2 && s.StringValue == "TRANSITIONING");

                // Retrieve the time series via the beta time series resource and assert the StateSet
                // direct relation round-trips.
                var retrievedTs = (await _fx.Write.Beta.TimeSeries.RetrieveAsync(new[] { tsId })).Single().Properties;
                Assert.Equal(CogniteSdk.DataModels.Core.TimeSeriesType.State, retrievedTs.Type);
                Assert.NotNull(retrievedTs.StateSet);
                Assert.Equal(space, retrievedTs.StateSet.Space);
                Assert.Equal(stateSetXid, retrievedTs.StateSet.ExternalId);
            }
            finally
            {
                await _fx.Write.DataModels.DeleteInstances(new[] { tsId, stateSetId });
            }
        }

        [Fact]
        public async Task CreateStateSetStateTimeSeriesAndAddDatapoints()
        {
            var space = _fx.TestSpace;
            var stateSetXid = "pump_states_" + Guid.NewGuid().ToString("N");
            var tsXid = "pump_001_state_" + Guid.NewGuid().ToString("N");

            var stateSetId = new InstanceIdentifierWithType(InstanceType.node, new InstanceIdentifier(space, stateSetXid));
            var tsId = new InstanceIdentifierWithType(InstanceType.node, new InstanceIdentifier(space, tsXid));

            try
            {
                await UpsertStateSetAndStateTimeSeries(
                    space, stateSetXid, tsXid,
                    stateSetName: "Pump Run States",
                    states: new[]
                    {
                        new CogniteState { NumericValue = 0, StringValue = "STOPPED" },
                        new CogniteState { NumericValue = 1, StringValue = "RUNNING" }
                    },
                    tsName: "Pump 001 State");

                // Add a state data point to the new time series.
                var datapoints = new StateDatapoints();
                datapoints.Datapoints.Add(new StateDatapoint { Timestamp = 1609459200000L, NumericValue = 1L, StringValue = "RUNNING" });

                var insertion = new DataPointInsertionRequest();
                insertion.Items.Add(new DataPointInsertionItem
                {
                    InstanceId = new InstanceId { Space = space, ExternalId = tsXid },
                    StateDatapoints = datapoints
                });

                await _fx.Write.Beta.DataPoints.CreateAsync(insertion);

                // Verify the data point was added correctly.
                var latest = (await _fx.Write.Beta.DataPoints.LatestAsync(new DataPointsLatestQuery
                {
                    Items = new[] { IdentityWithBefore.Create(new InstanceIdentifier(space, tsXid)) }
                })).Items.First();

                Assert.Equal(DataPointListItem.DatapointTypeOneofCase.StateDatapoints, latest.DatapointTypeCase);
                var latestPoint = latest.StateDatapoints.Datapoints.Single();
                Assert.Equal(1609459200000L, latestPoint.Timestamp);
                Assert.Equal(1L, latestPoint.NumericValue);
                Assert.Equal("RUNNING", latestPoint.StringValue);
            }
            finally
            {
                await _fx.Write.DataModels.DeleteInstances(new[] { tsId, stateSetId });
            }
        }
    }
}

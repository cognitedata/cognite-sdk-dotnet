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

        private static string NewExternalId(string prefix) => $"{prefix}{Guid.NewGuid():N}";

        private (string Space, string StateSetXid, string TsXid, InstanceIdentifierWithType StateSetId, InstanceIdentifierWithType TsId)
            CreateStateTimeSeriesTestContext(string stateSetPrefix, string tsPrefix)
        {
            var space = _fx.TestSpace;
            var stateSetXid = NewExternalId(stateSetPrefix);
            var tsXid = NewExternalId(tsPrefix);

            return (
                space,
                stateSetXid,
                tsXid,
                new InstanceIdentifierWithType(InstanceType.node, new InstanceIdentifier(space, stateSetXid)),
                new InstanceIdentifierWithType(InstanceType.node, new InstanceIdentifier(space, tsXid))
            );
        }

        [Fact]
        public async Task UpsertStateSetIngestAndQueryDatapoints()
        {
            var (space, stateSetXid, tsXid, stateSetId, tsId) =
                CreateStateTimeSeriesTestContext("valve_states_", "valve_001_state_");

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

                var instanceId = new InstanceIdentifier(space, tsXid);

                // Raw query
                var raw = (await _fx.Write.Beta.DataPoints.ListAsync(new DataPointsQuery
                {
                    Items = new[]
                    {
                        new DataPointsQueryItem
                        {
                            InstanceId = instanceId,
                            Start = "1609459200000",
                            End = "1609545600000",
                            Limit = 10
                        }
                    }
                })).Items.First();

                Assert.Equal(DataPointListItem.DatapointTypeOneofCase.StateDatapoints, raw.DatapointTypeCase);
                Assert.Equal(3, raw.StateDatapoints.Datapoints.Count);

                // Aggregate query
                var agg = (await _fx.Write.Beta.DataPoints.ListAsync(new DataPointsQuery
                {
                    Items = new[]
                    {
                        new DataPointsQueryItem
                        {
                            InstanceId = instanceId,
                            Start = "1609459200000",
                            End = "1609545600000",
                            Granularity = "1d",
                            Aggregates = new[]
                            {
                                "count", "countGood", "countUncertain",
                                "stateCount", "stateTransitions", "stateDuration"
                            },
                            TreatUncertainAsBad = false
                        }
                    }
                })).Items.First();

                Assert.Equal(DataPointListItem.DatapointTypeOneofCase.StateAggregateDatapoints, agg.DatapointTypeCase);
                var aggregateDatapoint = agg.StateAggregateDatapoints.Datapoints.Single();
                Assert.Equal(3D, aggregateDatapoint.Count);
                Assert.Equal(3D, aggregateDatapoint.CountGood);
                Assert.Equal(0D, aggregateDatapoint.CountUncertain);

                var stateAggregates = aggregateDatapoint.StateAggregates;
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
                    Items = new[] { IdentityWithBefore.Create(instanceId) }
                })).Items.First();

                Assert.Equal(DataPointListItem.DatapointTypeOneofCase.StateDatapoints, latest.DatapointTypeCase);
                var latestPoint = latest.StateDatapoints.Datapoints.Single();
                Assert.Equal(1609466400000L, latestPoint.Timestamp);
                Assert.Equal(0L, latestPoint.NumericValue);
                Assert.Equal("CLOSED", latestPoint.StringValue);
            }
            finally
            {
                await Retry.RunAsync(() => _fx.Write.DataModels.DeleteInstances(new[] { tsId, stateSetId }));
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
            string tsDescription = null,
            ViewIdentifier timeSeriesView = null)
        {
            await UpsertStateSetAndStateTimeSeries(
                space,
                stateSetXid,
                tsXid,
                new CogniteStateSet
                {
                    Name = stateSetName,
                    Description = stateSetDescription,
                    States = states
                },
                new CogniteTimeSeriesBase
                {
                    Name = tsName,
                    Description = tsDescription,
                    Type = CogniteSdk.DataModels.Core.TimeSeriesType.State,
                    StateSet = new DirectRelationIdentifier(space, stateSetXid)
                },
                timeSeriesView,
                new UpsertOptions { Replace = true });
        }

        /// <summary>
        /// Upsert a state set and a state time series referencing it, using generic property types.
        /// Allows tests to verify custom subtypes and/or writing time series to a custom view.
        /// </summary>
        private async Task UpsertStateSetAndStateTimeSeries<TStateSet, TTimeSeries>(
            string space,
            string stateSetXid,
            string tsXid,
            TStateSet stateSetProperties,
            TTimeSeries timeSeriesProperties,
            ViewIdentifier timeSeriesView = null,
            UpsertOptions timeSeriesUpsertOptions = null)
            where TStateSet : CogniteStateSet
            where TTimeSeries : CogniteTimeSeriesBase
        {
            await _fx.Write.Beta.StateSets.UpsertAsync<TStateSet>(new[]
            {
                new SourcedNodeWrite<TStateSet>
                {
                    Space = space,
                    ExternalId = stateSetXid,
                    Properties = stateSetProperties
                }
            }, null);

            // State time series are only available in beta, so this must go through the beta API.
            await _fx.Write.Beta.TimeSeries.UpsertAsync<TTimeSeries>(new[]
            {
                new SourcedNodeWrite<TTimeSeries>
                {
                    Space = space,
                    ExternalId = tsXid,
                    Properties = timeSeriesProperties
                }
            }, timeSeriesView, timeSeriesUpsertOptions ?? new UpsertOptions { Replace = true });
        }

        [Fact]
        public async Task UpsertAndRetrieveStateSetAndTimeSeriesTyped()
        {
            var (space, stateSetXid, tsXid, stateSetId, tsId) =
                CreateStateTimeSeriesTestContext("valve_states_typed_", "valve_001_state_typed_");

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
                var retrievedStateSet = (await Retry.RunAsync(
                    async () => (await stateSets.RetrieveAsync(new[] { stateSetId })).Single())).Properties;
                Assert.Equal("Valve Position States", retrievedStateSet.Name);
                var states = retrievedStateSet.States.ToList();
                Assert.Equal(3, states.Count);
                Assert.Contains(states, s => s.NumericValue == 0 && s.StringValue == "CLOSED");
                Assert.Contains(states, s => s.NumericValue == 1 && s.StringValue == "OPEN");
                Assert.Contains(states, s => s.NumericValue == 2 && s.StringValue == "TRANSITIONING");

                // Retrieve the time series via the beta time series resource and assert the StateSet
                // direct relation round-trips.
                var retrievedTs = (await Retry.RunAsync(
                    async () => (await _fx.Write.Beta.TimeSeries.RetrieveAsync(new[] { tsId })).Single())).Properties;
                Assert.Equal(CogniteSdk.DataModels.Core.TimeSeriesType.State, retrievedTs.Type);
                Assert.NotNull(retrievedTs.StateSet);
                Assert.Equal(space, retrievedTs.StateSet.Space);
                Assert.Equal(stateSetXid, retrievedTs.StateSet.ExternalId);
            }
            finally
            {
                await Retry.RunAsync(() => _fx.Write.DataModels.DeleteInstances(new[] { tsId, stateSetId }));
            }
        }

        [Fact]
        public async Task CreateStateSetStateTimeSeriesAndAddDatapoints()
        {
            var (space, stateSetXid, tsXid, stateSetId, tsId) =
                CreateStateTimeSeriesTestContext("pump_states_", "pump_001_state_");

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
                await Retry.RunAsync(() => _fx.Write.DataModels.DeleteInstances(new[] { tsId, stateSetId }));
            }
        }
        [Fact]
        public async Task UpsertStateSetAndTimeSeriesWithNoStates()
        {
            var (space, stateSetXid, tsXid, stateSetId, tsId) =
                CreateStateTimeSeriesTestContext("empty_states_", "empty_states_ts_");

            try
            {
                await UpsertStateSetAndStateTimeSeries(
                    space, stateSetXid, tsXid,
                    stateSetName: "Empty State Set",
                    states: Array.Empty<CogniteState>(),
                    tsName: "Empty States Time Series",
                    stateSetDescription: "State set intentionally created with no states",
                    tsDescription: "Time series referencing an empty state set");

                // Retrieve the state set and verify it round-trips with no states.
                var retrievedStateSet = (await Retry.RunAsync(
                    async () => (await _fx.Write.Beta.StateSets.RetrieveAsync(new[] { stateSetId })).Single())).Properties;
                Assert.Equal("Empty State Set", retrievedStateSet.Name);
                Assert.True(retrievedStateSet.States == null || !retrievedStateSet.States.Any());

                // Retrieve the time series and verify the direct relation to the (empty) state set.
                var retrievedTs = (await Retry.RunAsync(
                    async () => (await _fx.Write.Beta.TimeSeries.RetrieveAsync(new[] { tsId })).Single())).Properties;
                Assert.Equal(CogniteSdk.DataModels.Core.TimeSeriesType.State, retrievedTs.Type);
                Assert.NotNull(retrievedTs.StateSet);
                Assert.Equal(space, retrievedTs.StateSet.Space);
                Assert.Equal(stateSetXid, retrievedTs.StateSet.ExternalId);
            }
            finally
            {
                await Retry.RunAsync(() => _fx.Write.DataModels.DeleteInstances(new[] { tsId, stateSetId }));
            }
        }

        [Fact]
        public async Task UpsertWithNullThrows()
        {
            await Assert.ThrowsAnyAsync<Exception>(() =>
                _fx.Write.Beta.StateSets.UpsertAsync(null));
            await Assert.ThrowsAnyAsync<Exception>(() =>
                _fx.Write.Beta.TimeSeries.UpsertAsync(null));
        }

        [Fact]
        public async Task UpsertAndRetrieveStateSetAndTimeSeriesWithGenericCustomType()
        {
            var (space, stateSetXid, tsXid, stateSetId, tsId) =
                CreateStateTimeSeriesTestContext("valve_states_generic_", "valve_001_state_generic_");

            try
            {
                // Exercise the generic UpsertAsync<T> overloads directly, using custom subtypes
                // of CogniteStateSet / CogniteTimeSeriesBase instead of the base types.
                await UpsertStateSetAndStateTimeSeries(
                    space,
                    stateSetXid,
                    tsXid,
                    new CustomStateSet
                    {
                        Name = "Valve Position States (generic)",
                        States = new[]
                        {
                            new CogniteState { NumericValue = 0, StringValue = "CLOSED" },
                            new CogniteState { NumericValue = 1, StringValue = "OPEN" }
                        }
                    },
                    new CustomTimeSeries
                    {
                        Name = "Valve 001 Position (generic)",
                        Type = CogniteSdk.DataModels.Core.TimeSeriesType.State,
                        StateSet = new DirectRelationIdentifier(space, stateSetXid)
                    });

                // Exercise the generic RetrieveAsync<T> overloads, deserializing into the custom subtypes.
                var retrievedStateSet = await Retry.RunAsync(
                    async () => (await _fx.Write.Beta.StateSets.RetrieveAsync<CustomStateSet>(new[] { stateSetId })).Single());
                Assert.IsType<CustomStateSet>(retrievedStateSet.Properties);
                Assert.Equal("Valve Position States (generic)", retrievedStateSet.Properties.Name);
                var states = retrievedStateSet.Properties.States.ToList();
                Assert.Equal(2, states.Count);
                Assert.Contains(states, s => s.NumericValue == 0 && s.StringValue == "CLOSED");
                Assert.Contains(states, s => s.NumericValue == 1 && s.StringValue == "OPEN");

                var retrievedTs = await Retry.RunAsync(
                    async () => (await _fx.Write.Beta.TimeSeries.RetrieveAsync<CustomTimeSeries>(new[] { tsId }, null)).Single());
                Assert.IsType<CustomTimeSeries>(retrievedTs.Properties);
                Assert.Equal(CogniteSdk.DataModels.Core.TimeSeriesType.State, retrievedTs.Properties.Type);
                Assert.NotNull(retrievedTs.Properties.StateSet);
                Assert.Equal(space, retrievedTs.Properties.StateSet.Space);
                Assert.Equal(stateSetXid, retrievedTs.Properties.StateSet.ExternalId);
            }
            finally
            {
                await Retry.RunAsync(() => _fx.Write.DataModels.DeleteInstances(new[] { tsId, stateSetId }));
            }
        }

        [Fact]
        public async Task UpsertTimeSeriesWithCustomView()
        {
            var (space, stateSetXid, tsXid, stateSetId, tsId) =
                CreateStateTimeSeriesTestContext("valve_states_custom_view_", "valve_001_custom_view_");
            var customViewExternalId = NewExternalId("custom_ts_view_");

            try
            {
                // Create a custom view extending the core time series view
                var customView = new ViewIdentifier(space, customViewExternalId, "v1");
                await _fx.Write.DataModels.UpsertViews(new[]
                {
                    new ViewCreate
                    {
                        Space = space,
                        ExternalId = customViewExternalId,
                        Version = "v1",
                        Name = "Custom Time Series View",
                        Implements = new[] { CogniteSdk.Resources.DataModels.CoreTimeSeriesResource<CogniteTimeSeriesBase>.DefaultView }
                    }
                });

                // Upsert state set/time series to custom view using shared helper.
                await UpsertStateSetAndStateTimeSeries(
                    space,
                    stateSetXid,
                    tsXid,
                    stateSetName: "Valve States",
                    states: new[]
                    {
                        new CogniteState { NumericValue = 0, StringValue = "CLOSED" },
                        new CogniteState { NumericValue = 1, StringValue = "OPEN" }
                    },
                    tsName: "Valve Position Custom View",
                    timeSeriesView: customView);

                // Retrieve from the custom view using the generic overload
                var retrievedTs = await Retry.RunAsync(
                    async () => (await _fx.Write.Beta.TimeSeries.RetrieveAsync<CogniteTimeSeriesBase>(
                        new[] { tsId }, customView)).Single());
                Assert.Equal("Valve Position Custom View", retrievedTs.Properties.Name);
                Assert.Equal(CogniteSdk.DataModels.Core.TimeSeriesType.State, retrievedTs.Properties.Type);
                Assert.NotNull(retrievedTs.Properties.StateSet);
            }
            finally
            {
                await Retry.RunAsync(() => _fx.Write.DataModels.DeleteInstances(new[] { tsId, stateSetId }));
                try { await _fx.Write.DataModels.DeleteViews(new[] { new FDMExternalId(customViewExternalId, space, "v1") }); }
                catch { /* best-effort */ }
            }
        }

        [Fact]
        public async Task RetrieveTimeSeriesWithCustomView()
        {
            var (space, stateSetXid, tsXid, stateSetId, tsId) =
                CreateStateTimeSeriesTestContext("valve_states_retrieve_custom_view_", "valve_001_retrieve_custom_view_");
            var customViewExternalId = NewExternalId("custom_ts_retrieve_view_");

            try
            {
                // Set up state set and time series using default view
                await UpsertStateSetAndStateTimeSeries(
                    space, stateSetXid, tsXid,
                    stateSetName: "Pump States",
                    states: new[]
                    {
                        new CogniteState { NumericValue = 0, StringValue = "OFF" },
                        new CogniteState { NumericValue = 1, StringValue = "ON" }
                    },
                    tsName: "Pump Position Default View");

                // Create a custom view extending the core time series view
                var customView = new ViewIdentifier(space, customViewExternalId, "v1");
                await _fx.Write.DataModels.UpsertViews(new[]
                {
                    new ViewCreate
                    {
                        Space = space,
                        ExternalId = customViewExternalId,
                        Version = "v1",
                        Name = "Custom Time Series Retrieve View",
                        Implements = new[] { CogniteSdk.Resources.DataModels.CoreTimeSeriesResource<CogniteTimeSeriesBase>.DefaultView }
                    }
                });

                // Retrieve from the regular view using the non-generic overload
                var retrieved = await Retry.RunAsync(
                    async () => (await _fx.Write.Beta.TimeSeries.RetrieveAsync(
                        new[] { tsId })).Single());
                Assert.Equal("Pump Position Default View", retrieved.Properties.Name);
                Assert.Equal(CogniteSdk.DataModels.Core.TimeSeriesType.State, retrieved.Properties.Type);

                // Retrieve using the generic overload with custom view
                var retrievedCustom = await Retry.RunAsync(
                    async () => (await _fx.Write.Beta.TimeSeries.RetrieveAsync<CogniteTimeSeriesBase>(
                        new[] { tsId }, customView)).Single());
                Assert.Equal("Pump Position Default View", retrievedCustom.Properties.Name);
            }
            finally
            {
                await Retry.RunAsync(() => _fx.Write.DataModels.DeleteInstances(new[] { tsId, stateSetId }));
                try { await _fx.Write.DataModels.DeleteViews(new[] { new FDMExternalId(customViewExternalId, space, "v1") }); }
                catch { /* best-effort */ }
            }
        }

        [Fact]
        public async Task UpsertAndRetrieveTimeSeriesWithCustomViewAndExtraProperty()
        {
            var (space, stateSetXid, tsXid, stateSetId, tsId) =
                CreateStateTimeSeriesTestContext("valve_states_extra_prop_", "valve_001_extra_prop_");
            var customViewExternalId = NewExternalId("custom_ts_extra_prop_view_");
            var customContainerExternalId = NewExternalId("custom_ts_extra_prop_container_");
            var customView = new ViewIdentifier(space, customViewExternalId, "v1");
            var customContainer = new ContainerIdentifier(space, customContainerExternalId);

            try
            {
                await _fx.Write.DataModels.UpsertContainers(new[]
                {
                    new ContainerCreate
                    {
                        Space = space,
                        ExternalId = customContainerExternalId,
                        Name = "Custom TS extra property container",
                        UsedFor = UsedFor.node,
                        Properties = new Dictionary<string, ContainerPropertyDefinition>
                        {
                            { "extraProperty", new ContainerPropertyDefinition
                                {
                                    Type = BasePropertyType.Text(),
                                    Nullable = true
                                }
                            }
                        }
                    }
                });

                await _fx.Write.DataModels.UpsertViews(new[]
                {
                    new ViewCreate
                    {
                        Space = space,
                        ExternalId = customViewExternalId,
                        Version = "v1",
                        Name = "Custom Time Series View With Extra Property",
                        Implements = new[] { CogniteSdk.Resources.DataModels.CoreTimeSeriesResource<CogniteTimeSeriesBase>.DefaultView },
                        Properties = new Dictionary<string, ICreateViewProperty>
                        {
                            { "extraProperty", new ViewPropertyCreate
                                {
                                    Container = customContainer,
                                    ContainerPropertyIdentifier = "extraProperty",
                                    Name = "Extra Property"
                                }
                            }
                        }
                    }
                });

                const string expectedExtraProperty = "round-trip-value";

                await UpsertStateSetAndStateTimeSeries(
                    space,
                    stateSetXid,
                    tsXid,
                    new CogniteStateSet
                    {
                        Name = "Valve States",
                        States = new[]
                        {
                            new CogniteState { NumericValue = 0, StringValue = "CLOSED" },
                            new CogniteState { NumericValue = 1, StringValue = "OPEN" }
                        }
                    },
                    new CustomTimeSeriesWithExtraProperties
                    {
                        Name = "Valve Position With Extra Property",
                        Type = CogniteSdk.DataModels.Core.TimeSeriesType.State,
                        StateSet = new DirectRelationIdentifier(space, stateSetXid),
                        ExtraProperty = expectedExtraProperty
                    },
                    customView,
                    new UpsertOptions { Replace = true });

                var retrieved = await Retry.RunAsync(
                    async () => (await _fx.Write.Beta.TimeSeries.RetrieveAsync<CustomTimeSeriesWithExtraProperties>(
                        new[] { tsId }, customView)).Single());

                Assert.IsType<CustomTimeSeriesWithExtraProperties>(retrieved.Properties);
                Assert.Equal("Valve Position With Extra Property", retrieved.Properties.Name);
                Assert.Equal(CogniteSdk.DataModels.Core.TimeSeriesType.State, retrieved.Properties.Type);
                Assert.NotNull(retrieved.Properties.StateSet);
                Assert.Equal(space, retrieved.Properties.StateSet.Space);
                Assert.Equal(stateSetXid, retrieved.Properties.StateSet.ExternalId);
                Assert.Equal(expectedExtraProperty, retrieved.Properties.ExtraProperty);
            }
            finally
            {
                await Retry.RunAsync(() => _fx.Write.DataModels.DeleteInstances(new[] { tsId, stateSetId }));
                try { await _fx.Write.DataModels.DeleteViews(new[] { customView.FDMExternalId() }); }
                catch { /* best-effort */ }
                try { await _fx.Write.DataModels.DeleteContainers(new[] { customContainer.ContainerId() }); }
                catch { /* best-effort */ }
            }
        }
    }

    /// <summary>
    /// Custom subtype of <see cref="CogniteStateSet"/> used to verify that the generic
    /// UpsertAsync/RetrieveAsync overloads on <see cref="CogniteSdk.Resources.Beta.StateSetsResource"/>
    /// work with types other than the base <see cref="CogniteStateSet"/>.
    /// </summary>
    internal class CustomStateSet : CogniteStateSet
    {
    }

    /// <summary>
    /// Custom subtype of <see cref="CogniteTimeSeriesBase"/> used to verify that the generic
    /// UpsertAsync/RetrieveAsync overloads on <see cref="CogniteSdk.Resources.Beta.TimeSeriesResource"/>
    /// work with types other than the base <see cref="CogniteTimeSeriesBase"/>.
    /// </summary>
    internal class CustomTimeSeries : CogniteTimeSeriesBase
    {
    }

    internal class CustomTimeSeriesWithExtraProperties : CogniteTimeSeriesBase
    {
        public string ExtraProperty { get; set; }
    }
}

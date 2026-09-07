// Copyright 2026 Cognite AS
// SPDX-License-Identifier: Apache-2.0

using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using CogniteSdk.Beta;
using CogniteSdk.DataModels;
using Oryx.Cognite;
using Xunit;

namespace Test.CSharp
{
    /// <summary>
    /// Serialization tests for referencing record views in stream records requests.
    /// </summary>
    public class StreamRecordViewsTests
    {
        private readonly JsonSerializerOptions _options = Common.jsonOptions;

        private static readonly ViewIdentifier View = new ViewIdentifier("mySpace", "myView", "v1");
        private static readonly ContainerIdentifier Container = new ContainerIdentifier("mySpace", "myContainer");

        // System.Text.Json writes properties declared on the derived type before the base type's.
        private const string ViewJson = @"{""version"":""v1"",""type"":""view"",""space"":""mySpace"",""externalId"":""myView""}";
        private const string ContainerJson = @"{""type"":""container"",""space"":""mySpace"",""externalId"":""myContainer""}";

        [Fact]
        public void TestPropertyReference()
        {
            Assert.Equal(new[] { "mySpace", "myView/v1", "prop" }, View.PropertyReference("prop"));
            Assert.Equal(new[] { "mySpace", "myContainer", "prop" }, Container.PropertyReference("prop"));
        }

        [Fact]
        public void TestRetrieveWithViewSourceFilterAndTargetUnits()
        {
            var request = new StreamRecordsRetrieve
            {
                Sources = new[]
                {
                    new StreamRecordSource { Source = View, Properties = new[] { "*" } },
                    new StreamRecordSource { Source = Container, Properties = new[] { "prop" } },
                },
                Filter = new AndFilter
                {
                    And = new IDMSFilter[]
                    {
                        new HasDataFilter { HasData = new SourceIdentifier[] { View, Container } },
                        new EqualsFilter
                        {
                            Property = View.PropertyReference("severity"),
                            Value = new RawPropertyValue<string>("CRITICAL"),
                        },
                    }
                },
                Limit = 5,
                TargetUnits = StreamRecordTargetUnits.ForUnitSystem("SI"),
            };

            var expectedJson =
                @"{""sources"":[{""source"":" + ViewJson + @",""properties"":[""*""]},"
                + @"{""source"":" + ContainerJson + @",""properties"":[""prop""]}],"
                + @"""filter"":{""and"":[{""hasData"":[" + ViewJson + "," + ContainerJson + @"]},"
                + @"{""equals"":{""property"":[""mySpace"",""myView/v1"",""severity""],""value"":""CRITICAL""}}]},"
                + @"""limit"":5,"
                + @"""targetUnits"":{""unitSystemName"":""SI""}}";

            var json = JsonSerializer.Serialize(request, _options);
            Assert.Equal(expectedJson, json);

            var back = JsonSerializer.Deserialize<StreamRecordsRetrieve>(json, _options);
            Assert.NotNull(back);
            var sources = back.Sources.ToList();
            Assert.Equal(2, sources.Count);
            var viewSource = Assert.IsType<ViewIdentifier>(sources[0].Source);
            Assert.Equal("myView", viewSource.ExternalId);
            Assert.Equal("v1", viewSource.Version);
            Assert.IsType<ContainerIdentifier>(sources[1].Source);

            var and = Assert.IsType<AndFilter>(back.Filter);
            var hasData = Assert.IsType<HasDataFilter>(and.And.First());
            Assert.IsType<ViewIdentifier>(hasData.HasData.First());
            Assert.Equal("SI", back.TargetUnits.UnitSystemName);
            Assert.Null(back.TargetUnits.Properties);
        }

        [Fact]
        public void TestSyncWithViewSource()
        {
            var request = new StreamRecordsSync
            {
                InitializeCursor = "1d-ago",
                Limit = 10,
                Sources = new[]
                {
                    new StreamRecordSource { Source = View, Properties = new[] { "temperature" } },
                },
            };

            var expectedJson =
                @"{""sources"":[{""source"":" + ViewJson + @",""properties"":[""temperature""]}],"
                + @"""limit"":10,""initializeCursor"":""1d-ago""}";

            var json = JsonSerializer.Serialize(request, _options);
            Assert.Equal(expectedJson, json);

            var back = JsonSerializer.Deserialize<StreamRecordsSync>(json, _options);
            Assert.NotNull(back);
            var source = Assert.IsType<ViewIdentifier>(Assert.Single(back.Sources).Source);
            Assert.Equal("mySpace", source.Space);
            Assert.Equal("v1", source.Version);
            Assert.Null(back.TargetUnits);
        }

        [Fact]
        public void TestAggregateWithViewPropertyAndTargetUnits()
        {
            var request = new StreamRecordsAggregate
            {
                Aggregates = new Dictionary<string, IStreamRecordAggregate>
                {
                    { "avg_temp", new AvgStreamRecordAggregate { Property = View.PropertyReference("temperature") } },
                },
                TargetUnits = StreamRecordTargetUnits.ForProperties(new[]
                {
                    new StreamRecordPropertyTargetUnit
                    {
                        Property = View.PropertyReference("temperature"),
                        Unit = StreamRecordTargetUnit.FromExternalId("temperature:k"),
                    },
                    new StreamRecordPropertyTargetUnit
                    {
                        Property = Container.PropertyReference("pressure"),
                        Unit = StreamRecordTargetUnit.FromUnitSystem("SI"),
                    },
                }),
            };

            const string expectedJson =
                @"{""aggregates"":{""avg_temp"":{""avg"":{""property"":[""mySpace"",""myView/v1"",""temperature""]}}},"
                + @"""targetUnits"":{""properties"":["
                + @"{""property"":[""mySpace"",""myView/v1"",""temperature""],""unit"":{""externalId"":""temperature:k""}},"
                + @"{""property"":[""mySpace"",""myContainer"",""pressure""],""unit"":{""unitSystemName"":""SI""}}]}}";

            var json = JsonSerializer.Serialize(request, _options);
            Assert.Equal(expectedJson, json);
            Assert.DoesNotContain("null", json);

            var back = JsonSerializer.Deserialize<StreamRecordsAggregate>(json, _options);
            Assert.NotNull(back);
            var avg = Assert.IsType<AvgStreamRecordAggregate>(back.Aggregates["avg_temp"]);
            Assert.Equal(new[] { "mySpace", "myView/v1", "temperature" }, avg.Property);
            Assert.Null(back.TargetUnits.UnitSystemName);
            var units = back.TargetUnits.Properties.ToList();
            Assert.Equal(2, units.Count);
            Assert.Equal("temperature:k", units[0].Unit.ExternalId);
            Assert.Null(units[0].Unit.UnitSystemName);
            Assert.Equal("SI", units[1].Unit.UnitSystemName);
            Assert.Null(units[1].Unit.ExternalId);
        }

        [Fact]
        public void TestIngestWithViewSource()
        {
            var record = new StreamRecordWrite
            {
                ExternalId = "rec-1",
                Space = "mySpace",
                Sources = new[]
                {
                    new InstanceData<StandardInstanceWriteData>
                    {
                        Source = View,
                        Properties = new StandardInstanceWriteData
                        {
                            { "temperature", new RawPropertyValue<double>(21.5) },
                        }
                    }
                }
            };

            var json = JsonSerializer.Serialize(record, _options);
            Assert.Contains(@"""source"":" + ViewJson, json);
            Assert.Contains(@"""properties"":{""temperature"":21.5}", json);
        }
    }
}

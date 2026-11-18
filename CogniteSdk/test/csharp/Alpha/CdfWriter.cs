// Copyright 2026 Cognite AS
// SPDX-License-Identifier: Apache-2.0

using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using CogniteSdk;
using CogniteSdk.Alpha;
using Xunit;

namespace Test.CSharp.Alpha
{
    public class CdfWriterTests
    {
        // Mirrors the converters registered by the SDK's own serializer options.
        private static readonly JsonSerializerOptions _jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull,
            Converters = { new StatusCodeConverter(), new MultiValueConverter() },
        };

        [Fact]
        public void TestSetupRequestSerialization()
        {
            var request = new CdfWriterSetupRequest
            {
                IntegrationId = "my-integration",
                ExtractorType = "cognite-opcua",
                ExtractorVersion = "1.0.0",
                Merge = "preserve",
                Tasks = new[]
                {
                    new CdfWriterTaskDeclaration { Name = "Browse", Resources = new[] { "CogniteTimeseries", "CogniteSourceSystem" } },
                },
                EmittedViews = new Dictionary<string, CdfWriterViewReference>
                {
                    ["CogniteTimeseries"] = new CdfWriterViewReference { Space = "cdf_cdm", ExternalId = "CogniteTimeSeries", Version = "v1" },
                },
                Mappings = new[]
                {
                    new CdfWriterMapping
                    {
                        ExternalId = "fba8cb22-e44a-4ee1-9282-57ce84f23d7c",
                        Name = "Norway",
                        Kind = "spaceRouting",
                        Resources = new Dictionary<string, CdfWriterMappingResource>
                        {
                            ["CogniteTimeseries"] = new CdfWriterMappingResource
                            {
                                Parameters = new CdfWriterSpaceRoutingParameters
                                {
                                    Property = new[] { "externalId" },
                                    Rules = new[] { new CdfWriterSpaceRoutingRule { Pattern = "nor-.*", Space = "sp-norway" } },
                                    Default = "sp-other",
                                },
                                Properties = "input",
                                AutoCreateDirectRelations = false,
                            },
                            ["CogniteSourceSystem"] = new CdfWriterMappingResource
                            {
                                Parameters = new CdfWriterSpaceRoutingParameters { Default = "sp-norway" },
                            },
                        },
                    },
                },
                Bindings = new[]
                {
                    new CdfWriterBinding { TaskIds = new[] { "Browse" }, MappingId = "fba8cb22-e44a-4ee1-9282-57ce84f23d7c" },
                },
            };

            var json = JsonSerializer.Serialize(request, _jsonOptions);
            var expected = JsonNode.Parse("""
                {
                    "integrationId": "my-integration",
                    "extractorType": "cognite-opcua",
                    "extractorVersion": "1.0.0",
                    "merge": "preserve",
                    "tasks": [{ "name": "Browse", "resources": ["CogniteTimeseries", "CogniteSourceSystem"] }],
                    "emittedViews": {
                        "CogniteTimeseries": { "space": "cdf_cdm", "externalId": "CogniteTimeSeries", "version": "v1" }
                    },
                    "mappings": [{
                        "externalId": "fba8cb22-e44a-4ee1-9282-57ce84f23d7c",
                        "name": "Norway",
                        "kind": "spaceRouting",
                        "resources": {
                            "CogniteTimeseries": {
                                "parameters": {
                                    "property": ["externalId"],
                                    "rules": [{ "pattern": "nor-.*", "space": "sp-norway" }],
                                    "default": "sp-other"
                                },
                                "properties": "input",
                                "autoCreateDirectRelations": false
                            },
                            "CogniteSourceSystem": {
                                "parameters": { "default": "sp-norway" }
                            }
                        }
                    }],
                    "bindings": [{ "taskIds": ["Browse"], "mappingId": "fba8cb22-e44a-4ee1-9282-57ce84f23d7c" }]
                }
                """);
            Assert.True(JsonNode.DeepEquals(expected, JsonNode.Parse(json)), json);
        }

        [Fact]
        public void TestPayloadRequestSerialization()
        {
            var request = new CdfWriterPayloadRequest
            {
                IntegrationId = "my-integration",
                Data = new[]
                {
                    new CdfWriterTaskData
                    {
                        TaskId = "Stream data to CDF",
                        Resources = new Dictionary<string, CdfWriterResourceOperations>
                        {
                            ["CogniteTimeseries"] = new CdfWriterResourceOperations
                            {
                                Upsert = new[]
                                {
                                    new CdfWriterUpsertItem
                                    {
                                        ExternalId = "ts1",
                                        Type = "ts-type",
                                        Properties = new Dictionary<string, object> { ["type"] = "numeric", ["name"] = "TS 1" },
                                    },
                                },
                                Datapoints = new[]
                                {
                                    new CdfWriterDatapointsItem
                                    {
                                        ExternalId = "ts1",
                                        Type = "numeric",
                                        Datapoints = new[]
                                        {
                                            new CdfWriterDatapoint { Timestamp = 1000, Value = MultiValue.Create(1.5) },
                                            new CdfWriterDatapoint { Timestamp = 2000, Status = StatusCode.Create(0x80000000) },
                                        },
                                    },
                                    new CdfWriterDatapointsItem
                                    {
                                        ExternalId = "ts2",
                                        Type = "state",
                                        Datapoints = new[]
                                        {
                                            new CdfWriterDatapoint { Timestamp = 1000, Value = MultiValue.Create(3L), StringValue = "" },
                                        },
                                    },
                                },
                            },
                            ["edges"] = new CdfWriterResourceOperations
                            {
                                Delete = new[] { new CdfWriterDeleteItem { ExternalId = "e1", InstanceType = "edge" } },
                            },
                        },
                    },
                },
            };

            var json = JsonSerializer.Serialize(request, _jsonOptions);
            var expected = JsonNode.Parse("""
                {
                    "integrationId": "my-integration",
                    "data": [{
                        "taskId": "Stream data to CDF",
                        "resources": {
                            "CogniteTimeseries": {
                                "upsert": [{ "externalId": "ts1", "type": "ts-type", "properties": { "type": "numeric", "name": "TS 1" } }],
                                "datapoints": [
                                    {
                                        "externalId": "ts1",
                                        "type": "numeric",
                                        "datapoints": [
                                            { "timestamp": 1000, "value": 1.5 },
                                            { "timestamp": 2000, "value": null, "status": { "code": 2147483648 } }
                                        ]
                                    },
                                    {
                                        "externalId": "ts2",
                                        "type": "state",
                                        "datapoints": [{ "timestamp": 1000, "value": 3, "stringValue": "" }]
                                    }
                                ]
                            },
                            "edges": {
                                "delete": [{ "externalId": "e1", "instanceType": "edge" }]
                            }
                        }
                    }]
                }
                """);
            Assert.True(JsonNode.DeepEquals(expected, JsonNode.Parse(json)), json);
        }

        [Fact]
        public void TestPayloadResponseDeserialization()
        {
            var response = JsonSerializer.Deserialize<CdfWriterPayloadResponse>("""
                {
                    "data": [{
                        "taskId": "Stream data to CDF",
                        "resources": {
                            "CogniteTimeseries": {
                                "upsert": {
                                    "mappingId": "m1",
                                    "status": 200,
                                    "body": { "items": [] },
                                    "created": 1,
                                    "updated": 0,
                                    "typeMismatches": [{ "itemIndex": 0, "space": "sp", "externalId": "ts1", "requestedType": "state", "storedType": "numeric" }],
                                    "placementConflicts": [{ "itemIndex": 0, "externalId": "ts1", "recordedSpace": "a", "evaluatedSpace": "b" }],
                                    "mismatched": [{ "space": "sp", "externalId": "ts1", "property": "type", "sent": "state", "found": "numeric" }]
                                },
                                "datapoints": { "mappingId": "m1", "status": null, "body": null }
                            }
                        }
                    }]
                }
                """, _jsonOptions);

            var task = Assert.Single(response.Data);
            Assert.Equal("Stream data to CDF", task.TaskId);
            var upsert = task.Resources["CogniteTimeseries"]["upsert"];
            Assert.Equal("m1", upsert.MappingId);
            Assert.Equal(200, upsert.Status);
            Assert.Equal(JsonValueKind.Object, upsert.Body.Value.ValueKind);
            Assert.Equal(1, upsert.Created);
            Assert.Null(upsert.Skipped);
            Assert.Equal("numeric", Assert.Single(upsert.TypeMismatches).StoredType);
            Assert.Equal("a", Assert.Single(upsert.PlacementConflicts).RecordedSpace);
            Assert.Equal("numeric", Assert.Single(upsert.Mismatched).Found.Value.GetString());
            var datapoints = task.Resources["CogniteTimeseries"]["datapoints"];
            Assert.Null(datapoints.Status);
            Assert.True(datapoints.Body is null || datapoints.Body.Value.ValueKind == JsonValueKind.Null);
        }

        [Fact]
        public async Task TestPayloadRejectsNullRequest()
        {
            using var httpClient = new HttpClient();
            var client = Client.Builder.Create(httpClient)
                .SetAppId("test")
                .SetProject("project")
                .SetTokenProvider(_ => Task.FromResult("token"))
                .Build();

            await Assert.ThrowsAsync<ArgumentNullException>(() => client.Alpha.CdfWriter.PayloadAsync(null));
        }

        [Fact]
        public async Task TestSetupRejectsNullRequest()
        {
            using var httpClient = new HttpClient();
            var client = Client.Builder.Create(httpClient)
                .SetAppId("test")
                .SetProject("project")
                .SetTokenProvider(_ => Task.FromResult("token"))
                .Build();

            await Assert.ThrowsAsync<ArgumentNullException>(() => client.Alpha.CdfWriter.SetupAsync(null));
        }
    }
}

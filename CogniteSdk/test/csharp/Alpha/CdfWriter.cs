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
        private static readonly JsonSerializerOptions _jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull,
        };

        [Fact]
        public void TestSetupRequestSerialization()
        {
            var request = new CdfWriterSetupRequest
            {
                IntegrationId = "my-integration",
                ExtractorType = "cognite-opcua",
                ExtractorVersion = "1.0.0",
                Items = new List<CdfWriterSetupItem>
                {
                    new CdfWriterSetupItem
                    {
                        TaskName = "task-1",
                        Type = "space_routing",
                        Destination = "/timeseries/update",
                        Parameters = JsonSerializer.Deserialize<JsonElement>("{\"abcd.*\":\"space-1\",\"nested\":{\"list\":[\"1\",\"true\"]}}"),
                    },
                    new CdfWriterSetupItem
                    {
                        TaskName = "task-2",
                        Type = "custom",
                        Destination = "/timeseries/update",
                        Fields = new Dictionary<string, string>
                        {
                            ["externalId"] = "input.id",
                            ["space"] = "if(regex_match(input.name, 'nor.*'), 'norway', 'sweden')",
                        },
                    },
                    new CdfWriterSetupItem
                    {
                        TaskName = "task-3",
                        Type = "no_parameters",
                    },
                },
            };

            var json = JsonSerializer.Serialize(request, _jsonOptions);
            var expected = JsonNode.Parse("""
                {
                    "integrationId": "my-integration",
                    "extractorType": "cognite-opcua",
                    "extractorVersion": "1.0.0",
                    "items": [
                        {
                            "taskName": "task-1",
                            "type": "space_routing",
                            "parameters": { "abcd.*": "space-1", "nested": { "list": ["1", "true"] } },
                            "destination": "/timeseries/update"
                        },
                        {
                            "taskName": "task-2",
                            "type": "custom",
                            "destination": "/timeseries/update",
                            "fields": {
                                "externalId": "input.id",
                                "space": "if(regex_match(input.name, 'nor.*'), 'norway', 'sweden')"
                            }
                        },
                        {
                            "taskName": "task-3",
                            "type": "no_parameters"
                        }
                    ]
                }
                """);
            Assert.True(JsonNode.DeepEquals(expected, JsonNode.Parse(json)), json);
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

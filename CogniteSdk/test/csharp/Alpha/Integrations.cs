// Copyright 2026 Cognite AS
// SPDX-License-Identifier: Apache-2.0

using System.Collections.Generic;
using System.Text.Json;
using CogniteSdk.Alpha;
using Xunit;

namespace Test.CSharp.Alpha
{
    public class IntegrationsTests
    {
        private static readonly JsonSerializerOptions _jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull,
        };

        [Theory]
        [InlineData(ActionStatus.pending, "pending")]
        [InlineData(ActionStatus.running, "running")]
        [InlineData(ActionStatus.failed, "failed")]
        [InlineData(ActionStatus.succeeded, "succeeded")]
        [InlineData(ActionStatus.cancel_pending, "cancel_pending")]
        [InlineData(ActionStatus.canceled, "canceled")]
        public void TestActionStatusSerialization(ActionStatus status, string expected)
        {
            var json = JsonSerializer.Serialize(status, _jsonOptions);
            Assert.Equal($"\"{expected}\"", json);
        }

        [Fact]
        public void TestIntegrationActionDeserialization()
        {
            var json = """
                {
                    "externalId": "action-123",
                    "actionName": "restart-task",
                    "status": "pending",
                    "callMetadata": {"key": "value"},
                    "createdTime": 1700000000000,
                    "lastUpdatedTime": 1700000001000,
                    "resultMessage": null,
                    "resultMetadata": null
                }
                """;

            var action = JsonSerializer.Deserialize<IntegrationAction>(json, _jsonOptions);

            Assert.NotNull(action);
            Assert.Equal("action-123", action.ExternalId);
            Assert.Equal("restart-task", action.ActionName);
            Assert.Equal(ActionStatus.pending, action.Status);
            Assert.Equal(1700000000000L, action.CreatedTime);
            Assert.Equal(1700000001000L, action.LastUpdatedTime);
            Assert.Single(action.CallMetadata);
            Assert.Equal("value", action.CallMetadata["key"]);
            Assert.Null(action.ResultMessage);
            Assert.Null(action.ResultMetadata);
        }

        [Fact]
        public void TestCheckInResponseWithPendingActions()
        {
            var json = """
                {
                    "externalId": "my-integration",
                    "lastConfigRevision": 3,
                    "pendingActions": [
                        {
                            "externalId": "act-1",
                            "actionName": "run-batch",
                            "status": "pending",
                            "createdTime": 1700000000000,
                            "lastUpdatedTime": 1700000000000
                        },
                        {
                            "externalId": "act-2",
                            "actionName": "stop-stream",
                            "status": "cancel_pending",
                            "createdTime": 1700000002000,
                            "lastUpdatedTime": 1700000003000
                        }
                    ]
                }
                """;

            var response = JsonSerializer.Deserialize<CheckInResponse>(json, _jsonOptions);

            Assert.NotNull(response);
            Assert.Equal("my-integration", response.ExternalId);
            Assert.Equal(3, response.LastConfigRevision);
            Assert.NotNull(response.PendingActions);

            var actions = new List<IntegrationAction>(response.PendingActions);
            Assert.Equal(2, actions.Count);
            Assert.Equal("act-1", actions[0].ExternalId);
            Assert.Equal(ActionStatus.pending, actions[0].Status);
            Assert.Equal("act-2", actions[1].ExternalId);
            Assert.Equal(ActionStatus.cancel_pending, actions[1].Status);
        }

        [Fact]
        public void TestCheckInResponseWithoutPendingActions()
        {
            // Backward compatibility: PendingActions field is absent
            var json = """{"externalId": "my-integration", "lastConfigRevision": 1}""";

            var response = JsonSerializer.Deserialize<CheckInResponse>(json, _jsonOptions);

            Assert.NotNull(response);
            Assert.Equal(1, response.LastConfigRevision);
            Assert.Null(response.PendingActions);
        }

        [Fact]
        public void TestCheckInRequestWithActionUpdatesSerialization()
        {
            var request = new CheckInRequest
            {
                ExternalId = "my-integration",
                ActionUpdates = new[]
                {
                    new ActionUpdate
                    {
                        ExternalId = "act-1",
                        Status = ActionStatus.succeeded,
                        ResultMessage = "Done",
                        ResultMetadata = new Dictionary<string, string> { ["rows"] = "42" },
                    }
                }
            };

            var json = JsonSerializer.Serialize(request, _jsonOptions);
            var doc = JsonDocument.Parse(json);

            var updates = doc.RootElement.GetProperty("actionUpdates");
            Assert.Equal(1, updates.GetArrayLength());

            var update = updates[0];
            Assert.Equal("act-1", update.GetProperty("externalId").GetString());
            Assert.Equal("succeeded", update.GetProperty("status").GetString());
            Assert.Equal("Done", update.GetProperty("resultMessage").GetString());
            Assert.Equal("42", update.GetProperty("resultMetadata").GetProperty("rows").GetString());
        }

        [Fact]
        public void TestActionUpdateNullFieldsNotSerialized()
        {
            var update = new ActionUpdate
            {
                ExternalId = "act-1",
                Status = ActionStatus.failed,
            };

            var json = JsonSerializer.Serialize(update, _jsonOptions);
            var doc = JsonDocument.Parse(json);

            Assert.False(doc.RootElement.TryGetProperty("resultMessage", out _));
            Assert.False(doc.RootElement.TryGetProperty("resultMetadata", out _));
        }

        [Fact]
        public void TestStartupRequestWithAvailableActionsSerialization()
        {
            var request = new StartupRequest
            {
                ExternalId = "my-integration",
                AvailableActions = new[]
                {
                    new AvailableActionWrite
                    {
                        Name = "start-batch",
                        Type = ActionType.start_task,
                        Task = "batch-task",
                        Description = "Starts the batch task",
                    },
                    new AvailableActionWrite
                    {
                        Name = "custom-action",
                        Type = ActionType.custom,
                        // Task and Description intentionally omitted
                    }
                }
            };

            var json = JsonSerializer.Serialize(request, _jsonOptions);
            var doc = JsonDocument.Parse(json);

            var actions = doc.RootElement.GetProperty("availableActions");
            Assert.Equal(2, actions.GetArrayLength());

            var first = actions[0];
            Assert.Equal("start-batch", first.GetProperty("name").GetString());
            Assert.Equal("start_task", first.GetProperty("type").GetString());
            Assert.Equal("batch-task", first.GetProperty("task").GetString());
            Assert.Equal("Starts the batch task", first.GetProperty("description").GetString());

            var second = actions[1];
            Assert.Equal("custom-action", second.GetProperty("name").GetString());
            Assert.Equal("custom", second.GetProperty("type").GetString());
            Assert.False(second.TryGetProperty("task", out _));
            Assert.False(second.TryGetProperty("description", out _));
        }

        [Fact]
        public void TestActionsQueryToQueryParams_AllFields()
        {
            var query = new ActionsQuery
            {
                ExternalId = "my-integration",
                CreatedAfter = 1700000000000L,
                IncludeCompleted = false,
                Limit = 50,
                Cursor = "next-page",
            };

            var @params = query.ToQueryParams();

            Assert.Contains(("externalId", "my-integration"), @params);
            Assert.Contains(("createdAfter", "1700000000000"), @params);
            Assert.Contains(("includeCompleted", "false"), @params);
            Assert.Contains(("limit", "50"), @params);
            Assert.Contains(("cursor", "next-page"), @params);
        }

        [Fact]
        public void TestActionsQueryToQueryParams_OptionalFieldsOmitted()
        {
            var query = new ActionsQuery();
            var @params = query.ToQueryParams();

            Assert.DoesNotContain(@params, p => p.Item1 == "externalId");
            Assert.DoesNotContain(@params, p => p.Item1 == "createdAfter");
            Assert.DoesNotContain(@params, p => p.Item1 == "includeCompleted");
            Assert.DoesNotContain(@params, p => p.Item1 == "limit");
            Assert.DoesNotContain(@params, p => p.Item1 == "cursor");
        }
    }
}

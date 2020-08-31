using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Xunit;

using CogniteSdk;

namespace Test.CSharp.Integration {

    [Collection("TestBase")]
    public class AssetTests : TestFixture {


        [Fact]
        [Trait("Description", "Ensures listing Assets returns number of assets equal to 'limit' value")]
        public void ListingAssetsRespectsLimit() {
            // Arrange
            const int limit = 10;
            var option = new AssetQuery
            {
                Limit = limit
            };

            // Act
            var res = ReadClient.Assets.List(option);

            // Assert
            Assert.True(limit == res.Items.Count(), "Expected the number of assets to be the same as the Limit value");
        }
        [Fact]
        [Trait("Description", "Ensures that getting the Asset count matching the query returns a number")]
        public void CountAssetsReturnInt() {
            // Arrange
            var option = new AssetQuery {
                Filter = new AssetFilter(){
                    Metadata = new Dictionary<string, string>{
                        {"WMT_SAFETYCRITICALELEMENT_ID", "1060"}
                    }
                }
            };
            // Act
            var count = ReadClient.Assets.Aggregate(option);

            // Assert
            Assert.True(count > 0);
        }
        [Fact]
        [Trait("Description", "Ensures that getting the Asset count when filter has no matches returns zero")]
        public void CountAssetsWithNoMatchesReturnZero() {
            // Arrange
            var option = new AssetQuery {
                Filter = new AssetFilter(){
                    Metadata = new Dictionary<string, string>{
                        {"ThisMetaDataFilterhasNoMatches", "ThereIsNoSpoon"}
                    }
                }
            };
            // Act
            var count = ReadClient.Assets.Aggregate(option);

            // Assert
            Assert.True(count == 0);
        }

        [Fact]
        [Trait("Description","Ensures that getting an asset by ID returns the correct asset")]

        public void AssetByIdReturnsCorrectAsset() {
            // Arrange
            const long assetId = 130452390632424;

            // Act
            var res = ReadClient.Assets.Get(assetId);

            // Assert
            Assert.True(assetId == res.Id, "The received Asset doesn't match the ID");
        }

        [Fact]
        [Trait("Description", "Ensures that getting an asset with an invalid Id doesnt return anything")]
        public void AssetByInvalidIdReturnsError() {
            // Arrange
            var assetId = 0;
            bool caughtException = false;

            // Act
            try {
                var res = ReadClient.Assets.Get(assetId);
            } catch (ResponseException) {
                caughtException = true;
            }

            // Assert
            Assert.True(caughtException, "Expected request to fail with 'CogniteSdk.ResponseException' but didnt");
        }

        [Fact]
        [Trait("Description", "Gets multiple Assets with a list of ids and ensures they are the correct assets")]
        public void AssetsByIdsRetrivesTheCorrectAssetsAsync() {
            // Arrange
            var ids = new List<long>() {
                130452390632424,
                126847700303897,
                124419735577853
            };

            // Act
            var res = ReadClient.Assets.Retrieve(ids);

            // Assert
            var returnedIds = res.Select(asset => asset.Id);
            var resCount = res.Count();
            Assert.True(ids.Count == resCount, $"Expected {ids.Count} assets but got {resCount}");
            Assert.True(returnedIds.Intersect(ids).Count() == returnedIds.Count(), "One of the received Assets dont match the requested IDs");
        }

        [Fact]
        [Trait("Description", "Search Asset returns the correct number of assets")]
        public void AssetSearchReturnsExpectedNumberOfAssets() {
            // Arrange
            var numOfAssets = 10;
            var query = new AssetSearch()
            {
                Limit = numOfAssets,
                Search = new Search()
                {
                    Name = "23"
                }
            };

            // Act
            var res = ReadClient.Assets.Search(query);

            // Assert
            var resCount = res.Count();
            Assert.True(numOfAssets == resCount, $"Expected {query.Limit} assets but got {resCount}");
        }

        [Fact]
        [Trait("Description", "Listing Assets with filter returns the expected number of assets")]
        public void FilterAssetsReturnsTheExpectedNumberOfAssets() {
            // Arrange
            var numOfAssets = 10;
            var id = 6687602007296940;
            var query = new AssetQuery() {
                Limit = numOfAssets,
                Filter = new AssetFilter() {
                    RootIds = new List<Identity>() { Identity.Create(id) }
                }
            };

            // Act
            var res = ReadClient.Assets.List(query);

            // Assert
            var resCount = res.Items.Count();
            Assert.True(numOfAssets == resCount, $"Expected {query.Limit} assets but got {resCount}");
        }

        [Fact]
        [Trait("Description", "Creating an asset and deletes it works")]
        public void CreateAndDeleteAssetWorkAsExpected() {
            // Arrange
            var externalIdString = Guid.NewGuid().ToString();
            var newAsset = new AssetCreate
            {
                ExternalId = externalIdString,
                Name = "Create Assets c# sdk test",
                Description = "Just a test"
            };
            var deletes = new AssetDelete
            {
                Items = new List<Identity>() {Identity.Create(externalIdString)}
            };

            // Act
            var res = WriteClient.Assets.Create(new List<AssetCreate>() { newAsset });
            WriteClient.Assets.Delete(deletes);

            // Assert
            var resCount = res.Count();
            Assert.True(1 == resCount, $"Expected 1 created asset but got {resCount}");
            Assert.True(externalIdString == res.First().ExternalId, "Created externalId doesnt match expected");
        }

        [Fact]
        [Trait("Description", "Deleting an asset that does not exist fails with ResponseException")]
        public void AssetDeleteFailsWhenIdIsInvalid() {
            // Arrange
            var id = 0;
            var caughtException = false;

            var query = new AssetDelete
            {
                Items = new List<Identity> { Identity.Create(id) }
            };

            // Act
            try {
                WriteClient.Assets.Delete(query);
            } catch (ResponseException) {
                caughtException = true;
            }

            // Assert
            Assert.True(caughtException, "Expected request to fail with 'CogniteSdk.ResponseException' but didnt");
        }

        [Fact]
        [Trait("Description", "Updating asset performs expected changes")]
        public void UpdatedAssetsPerformsExpectedChanges() {
            // Arrange
            var externalIdString = Guid.NewGuid().ToString();
            var newMetadata = new Dictionary<string, string>() {
                { "key1", "value1" },
                { "key2", "value2" }
            };
            var newAsset = new AssetCreate
            {
                ExternalId = externalIdString,
                Name = "Update Assets c# sdk test",
                Description = "Just a test",
                Metadata = new Dictionary<string, string>() {{"oldkey1", "oldvalue1"}, {"oldkey2", "oldvalue2"}}
            };
            var newName = "Updated update asset";

            var update = new List<AssetUpdateItem>
            {
                new AssetUpdateItem(externalId: externalIdString)
                {
                    Update = new AssetUpdate()
                    {
                        Name = new Update<string>(newName),
                        Metadata = new UpdateDictionary<string>(add: newMetadata, remove: new List<string> { "oldkey1" })
                    }
                }
            };

            // Act
            _ = WriteClient.Assets.Create(new List<AssetCreate>() { newAsset });
            WriteClient.Assets.Update(update);

            var getRes = WriteClient.Assets.Retrieve(new List<string>() { externalIdString });
            WriteClient.Assets.Delete(new List<string>() { externalIdString });

            // Assert
            var resCount = getRes.Count();
            Assert.True(resCount == 1, $"Expected a single Asset but got {resCount}");
            var resAsset = getRes.First();
            Assert.True(externalIdString == resAsset.ExternalId, $"Asset doest have expected ExternalId. Was '{resAsset.ExternalId}' but expected '{externalIdString}'");
            Assert.True(newName == resAsset.Name, $"Expected the Asset name to update to '{newName}' but was '{resAsset.Name}'");
            Assert.True(resAsset.Metadata.ContainsKey("key1") && resAsset.Metadata.ContainsKey("key2"), "Asset wasnt update with new metadata values");
            Assert.True(resAsset.Metadata.ContainsKey("oldkey2") && !resAsset.Metadata.ContainsKey("oldkey1"), "Asset update changed unintended metadata values");
        }

        [Fact]
        [Trait("Description", "Updating assets label performs expected changes")]
        public async Task UpdatedAssetsLabelPerformsExpectedChangesAsync() {
            // Arrange
            var externalIdString = Guid.NewGuid().ToString();

            var initialAsset = new AssetCreate
            {
                ExternalId = externalIdString,
                Name = "Update Assets Label c# sdk test",
                Description = "Just a test",
                Labels = new List<CogniteExternalId>{ new CogniteExternalId("AssetTestUpdateLabel1") }
            };

            var newName = "Updated asset name";
            var newLabels = new List<CogniteExternalId> { new CogniteExternalId("AssetTestUpdateLabel2")};
            var update = new List<AssetUpdateItem>
            {
                new AssetUpdateItem(externalId: externalIdString)
                {
                    Update = new AssetUpdate()
                    {
                        Name = new Update<string>(newName),
                        Labels = new UpdateLabels<IEnumerable<CogniteExternalId>>(addLabels: newLabels, removeLabels: new List<CogniteExternalId> { new CogniteExternalId("AssetTestUpdateLabel1") })
                    }
                }
            };

            // Act
            _ = await WriteClient.Playground.Assets.CreateAsync(new List<AssetCreate>() { initialAsset }).ConfigureAwait(false);
            await WriteClient.Playground.Assets.UpdateAsync(update);

            var getRes = await WriteClient.Playground.Assets.RetrieveAsync(new List<string>() { externalIdString });
            await WriteClient.Playground.Assets.DeleteAsync(new List<string>() { externalIdString });

            // Assert
            var resCount = getRes.Count();
            Assert.True(resCount == 1, $"Expected a single Asset but got {resCount}");
            var resAsset = getRes.First();
            Assert.True(externalIdString == resAsset.ExternalId, $"Asset doest have expected ExternalId. Was '{resAsset.ExternalId}' but expected '{externalIdString}'");

            Assert.True(resAsset.Labels.Count() == 1, $"Expected asset to have one label but was '{resAsset.Labels.Count()}'");
            Assert.True(resAsset.Labels.ElementAt(0).ExternalId == newLabels.ElementAt(0).ExternalId, $"Expected label to be '{newLabels.ElementAt(0).ExternalId}' but was '{resAsset.Labels.ElementAt(0).ExternalId}'");
        }
    }
}

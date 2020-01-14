using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xunit;

using CogniteSdk.Assets;
using CogniteSdk;

namespace Test.CSharp.Integration {

    [Collection("TestBase")]
    public class Assets : TestFixture {


        [Fact]
        [Trait("Description", "Ensures listing Assets returns number of assets equal to 'limit' value")]
        public void ListingAssetsRespectsLimit() {
            // Arrange
            var limit = 10;
            var option = AssetQuery.Limit(limit);

            // Act
            var res = ReadClient.Assets.ListAsync(new List<AssetQuery> { option });

            // Assert
            Assert.True(limit == res.Result.Items.Count(), "Expected the number of assets to be the same as the Limit value");
        }

        [Fact]
        [Trait("Description","Ensures that getting an asset by ID returns the correct asset")]

        public async Task AssetByIdReturnsCorrectAssetAsync() {
            // Arrange
            var assetId = 130452390632424;

            // Act
            var res = await ReadClient.Assets.GetAsync(assetId);

            // Assert
            Assert.True(assetId == res.Id, "The received Asset doesn't match the ID");
        }

        [Fact]
        [Trait("Description", "Ensures that getting an asset with an invalid Id doesnt return anything")]
        public async Task AssetByInvalidIdReturnsErrorAsync() {
            // Arrange
            var assetId = 0;
            bool caughtException = false;

            // Act
            try {
                var res = await ReadClient.Assets.GetAsync(assetId);
            } catch (ResponseException) {
                caughtException = true;
            }

            // Assert
            Assert.True(caughtException, "Expected request to fail with 'CogniteSdk.ResponseException' but didnt");
        }

        [Fact]
        [Trait("Description", "Gets multiple Assets with a list of ids and ensures theyre the correct assets")]
        public async Task AssetsByIdsRetrivesTheCorrectAssetsAsync() {
            // Arrange
            var ids = new List<long>() {
                130452390632424,
                126847700303897,
                124419735577853
            };

            // Act
            var res = await ReadClient.Assets.GetByIdsAsync(ids);

            // Assert
            var returnedIds = res.Select(asset => asset.Id);
            var resCount = res.Count();
            Assert.True(ids.Count == resCount, $"Expected {ids.Count} assets but got {resCount}");
            Assert.True(returnedIds.Intersect(ids).Count() == returnedIds.Count(), "One of the received Assets dont match the requested IDs");
        }

        [Fact]
        [Trait("Description", "Search Asset returns the correct number of assets")]
        public async Task AssetSearchReturnsExpectedNumberOfAssetsAsync() {
            // Arrange
            var options = new List<AssetSearch>() {
                AssetSearch.Name("23")
            };
            var numOfAssets = 10;

            // Act
            var res = await ReadClient.Assets.SearchAsync(numOfAssets, options);

            // Assert
            var resCount = res.Count();
            Assert.True(numOfAssets == resCount, $"Expected {options.Count} assets but got {resCount}");
        }

        [Fact]
        [Trait("Description", "Listing Assets with filter returns the expected number of assets")]
        public async Task FilterAssetsReturnsTheExpectedNumberOfAssetsAsync() {
            // Arrange
            var numOfAssets = 10;
            var id = 6687602007296940;
            var options = new List<AssetQuery>() {
                AssetQuery.Limit(numOfAssets)
            };
            var filter = new List<AssetFilter>() {
                AssetFilter.RootIds(new List<Identity>() { Identity.Id(id) })
            };

            // Act
            var res = await ReadClient.Assets.ListAsync(options, filter);

            // Assert
            var resCount = res.Items.Count();
            Assert.True(numOfAssets == resCount, $"Expected {options.Count} assets but got {resCount}");
        }

        [Fact]
        [Trait("Description", "Creating an asset and deletes it works")]
        public async Task CreateAndDeleteAssetWorkAsExpectedAsync() {
            // Arrange
            var externalIdString = Guid.NewGuid().ToString();
            var newAsset = new AssetEntity();
            newAsset.ExternalId = externalIdString;
            newAsset.Name = "Create Assets c# sdk test";
            newAsset.Description = "Just a test";


            // Act
            var res = await WriteClient.Assets.CreateAsync(new List<AssetEntity>() { newAsset });
            await WriteClient.Assets.DeleteAsync(new List<string>() { externalIdString }, false);

            // Assert
            var resCount = res.Count();
            Assert.True(1 == resCount, $"Expected 1 created asset but got {resCount}");
            Assert.True(externalIdString == res.First().ExternalId, "Created externalId doesnt match expected");
        }

        [Fact]
        [Trait("Description", "Deleting an asset that does not exist fails with ResponseException")]
        public async Task AssetDeleteFailsWhenIdIsInvalidAsync() {
            // Arrange
            var id = 0;
            var caughtException = false;

            // Act
            try {
                await WriteClient.Assets.DeleteAsync(new List<long>() { id }, false);
            } catch (ResponseException) {
                caughtException = true;
            }

            // Assert
            Assert.True(caughtException, "Expected request to fail with 'CogniteSdk.ResponseException' but didnt");
        }

        [Fact]
        [Trait("Description", "Deleting an asset that exist fails with ResponseException")]
        public async Task UpdatedAssetsPerformsExpectedChangesAsync() {
            // Arrange
            var externalIdString = Guid.NewGuid().ToString();
            var newAsset = new AssetEntity();
            var newMetadata = new Dictionary<string, string>() {
                { "key1", "value1" },
                { "key2", "value2" }
            };
            newAsset.ExternalId = externalIdString;
            newAsset.Name = "Update Assets c# sdk test";
            newAsset.Description = "Just a test";
            newAsset.MetaData = new Dictionary<string, string>() {
                { "oldkey1", "oldvalue1" },
                { "oldkey2", "oldvalue2" }
            };

            var newName = "Updated update asset";

            // Act
            var res = await WriteClient.Assets.CreateAsync(new List<AssetEntity>() { newAsset });
            await WriteClient.Assets.UpdateAsync(new List<(Identity, IEnumerable<AssetUpdate>)>() {
                (Identity.ExternalId(externalIdString), new List<AssetUpdate>() {
                    AssetUpdate.SetName(newName),
                    AssetUpdate.ChangeMetaData(newMetadata, new List<string>() { "oldkey1" })
                })
            });

            var getRes = await WriteClient.Assets.GetByIdsAsync(new List<string>() { externalIdString });
            await WriteClient.Assets.DeleteAsync(new List<string>() { externalIdString }, false);

            // Assert
            var resCount = getRes.Count();
            Assert.True(resCount == 1, $"Expected a single Asset but got {resCount}");
            var resAsset = getRes.First();
            Assert.True(externalIdString == resAsset.ExternalId, $"Asset doest have expected ExternalId. Was '{resAsset.ExternalId}' but expected '{externalIdString}'");
            Assert.True(newName == resAsset.Name, $"Expected the Asset name to update to '{newName}' but was '{resAsset.Name}'");
            Assert.True(resAsset.MetaData.ContainsKey("key1") && resAsset.MetaData.ContainsKey("key2"), "Asset wasnt update with new metadata values");
            Assert.True(resAsset.MetaData.ContainsKey("oldkey2") && !resAsset.MetaData.ContainsKey("oldkey1"), "Asset update changed unintended metadata values");
        }
    }
}

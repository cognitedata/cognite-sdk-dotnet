using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using CogniteSdk.Assets;
using System.Threading.Tasks;
using CogniteSdk;
using System.Diagnostics;

namespace Test.CSharp.Integration {

    [TestClass]
    public class Assets : TestBase {

        [TestMethod]
        [TestCategory(Category.Asset)]
        [Description("Ensures listing Assets returns number of assets equal to 'limit' value")]

        public void ListingAssetsRespectsLimit() {
            // Arrange
            var limit = 10;
            var option = AssetQuery.Limit(limit);

            // Act
            var res = ReadClient.Assets.ListAsync(new List<AssetQuery> { option });

            // Assert
            Assert.AreEqual(limit, res.Result.Items.Count(), "Expected the number of assets to be the same as the Limit value");
        }

        [TestMethod]
        [TestCategory(Category.Asset)]
        [Description("Ensures that getting an asset by ID returns the correct asset")]

        public void AssetByIdReturnsCorrectAsset() {
            // Arrange
            var assetId = 130452390632424;

            // Act
            var res = ReadClient.Assets.GetAsync(assetId).Result;

            // Assert
            Assert.AreEqual(assetId, res.Id, "The received Asset doesn't match the ID");
        }

        [TestMethod]
        [TestCategory(Category.Asset)]
        [Description("Ensures that getting an asset with an invalid Id doesnt return anything")]

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
            Assert.IsTrue(caughtException, "Expected request to fail with 'CogniteSdk.ResponseException' but didnt");
        }

        [TestMethod]
        [TestCategory(Category.Asset)]
        [Description("Gets multiple Assets with a list of ids and ensures theyre the correct assets")]
        public void AssetsByIdsRetrivesTheCorrectAssets() {
            // Arrange
            var ids = new List<long>() {
                130452390632424,
                126847700303897,
                124419735577853
            };

            // Act
            var res = ReadClient.Assets.GetByIdsAsync(ids).Result;

            // Assert
            var returnedIds = res.Select(asset => asset.Id);
            var resCount = res.Count();
            Assert.AreEqual(ids.Count, resCount, $"Expected {ids.Count} assets but got {resCount}");
            Assert.AreEqual(returnedIds.Intersect(ids).Count(), returnedIds.Count(), "One of the received Assets dont match the requested IDs");
        }

        [TestMethod]
        [TestCategory(Category.Asset)]
        [Description("Search Asset returns the correct number of assets")]
        public void AssetSearchReturnsExpectedNumberOfAssets() {
            // Arrange
            var options = new List<AssetSearch>() {
                AssetSearch.Name("23")
            };
            var numOfAssets = 10;

            // Act
            var res = ReadClient.Assets.SearchAsync(numOfAssets, options).Result;

            // Assert
            var resCount = res.Count();
            Assert.AreEqual(numOfAssets, resCount, $"Expected {options.Count} assets but got {resCount}");
        }

        [TestMethod]
        [TestCategory(Category.Asset)]
        [Description("Listing Assets with filter returns the expected number of assets")]
        public void FilterAssetsReturnsTheExpectedNumberOfAssets() {
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
            var res = ReadClient.Assets.ListAsync(options, filter).Result;

            // Assert
            var resCount = res.Items.Count();
            Assert.AreEqual(numOfAssets, resCount, $"Expected {options.Count} assets but got {resCount}");
        }

        [TestMethod]
        [TestCategory(Category.Asset)]
        [Description("Creating an asset and deletes it works")]
        public void CreateAndDeleteAssetWorkAsExpected() {
            // Arrange
            var externalIdString = "createDeleteTestAssets";
            var newAsset = new AssetEntity();
            newAsset.ExternalId = externalIdString;
            newAsset.Name = "Create Assets c# sdk test";
            newAsset.Description = "Just a test";


            // Act
            var res = WriteClient.Assets.CreateAsync(new List<AssetEntity>() { newAsset }).Result;
            WriteClient.Assets.DeleteAsync(new List<string>() { externalIdString }, false);

            // Assert
            var resCount = res.Count();
            Assert.AreEqual(1, resCount, $"Expected 1 created asset but got {resCount}");
            Assert.AreEqual(externalIdString, res.First().ExternalId, "Created externalId doesnt match expected");
        }

        [TestMethod]
        [TestCategory(Category.Asset)]
        [Description("Deleting an asset that exist fails with ResponseException")]
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
            Assert.IsTrue(caughtException, "Expected request to fail with 'CogniteSdk.ResponseException' but didnt");
        }

        [TestMethod]
        [TestCategory(Category.Asset)]
        [Description("Deleting an asset that exist fails with ResponseException")]
        public async Task UpdatedAssetsPerformsExpectedChangesAsync() {
            // Arrange
            var externalIdString = "updateAssetTest";
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
            Assert.IsTrue(resCount == 1, $"Expected a single Asset but got {resCount}");
            var resAsset = getRes.First();
            Assert.AreEqual(externalIdString, resAsset.ExternalId, $"Asset doest have expected ExternalId. Was '{resAsset.ExternalId}' but expected '{externalIdString}'");
            Assert.AreEqual(newName, resAsset.Name, $"Expected the Asset name to update to '{newName}' but was '{resAsset.Name}'");
            Assert.IsTrue(resAsset.MetaData.ContainsKey("key1") && resAsset.MetaData.ContainsKey("key2"), "Asset wasnt update with new metadata values");
            Assert.IsTrue(resAsset.MetaData.ContainsKey("oldkey2") && !resAsset.MetaData.ContainsKey("oldkey1"), "Asset update changed unintended metadata values");
        }


    }
    
}

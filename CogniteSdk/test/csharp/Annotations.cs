using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Xunit;

using CogniteSdk;

namespace Test.CSharp.Integration
{
    [Collection("TestBase")]
    public class AnnotationTest : TestFixture
    {
        [Fact]
        [Trait("Description", "Deleting an annotation that does not exist fails with ResponseException")]
        public async Task AnnotationDeleteFailsWhenIdIsInvalidAsync()
        {
            // Arrange
            const int id = 0;
            var caughtException = false;

            var query = new AnnotationDelete
            {
                Items = new List<AnnotationId> { new AnnotationId { Id = id } }
            };

            // Act
            try
            {
                await WriteClient.Playground.Annotations.DeleteAsync(query);
            }
            catch (ResponseException)
            {
                caughtException = true;
            }
            // Assert
            Assert.True(caughtException, "Expected request to fail with 'CogniteSdk.ResponseException' but didnt");
        }

        [Fact]
        [Trait("Description", "Suggest an annotation and delete it.")]
        public async Task AnnotationSuggestDeleteAsync()
        {
            // Create the annotated resource (threedmodel and the annotation)
            var createThreeDModelQuery = new ThreeDModelCreate() { Name = "dotnet-integration-test-for-annotations-suggest" };
            var threeDModel = (await WriteClient.ThreeDModels.CreateAsync(new List<ThreeDModelCreate> { createThreeDModelQuery })).FirstOrDefault();
            var boundingVolume = new BoundingVolume()
            {
                Region = new List<Geometry>()
                {
                    new Geometry()
                    {
                        Cylinder = new Cylinder()
                        {
                            CenterA = new float[] {0, 1, 2},
                            CenterB = new float[] {2, 3, 4},
                            Radius = 3

                        }
                    }
                }
            };
            var suggestAnnotationQuery = new AnnotationSuggest()
            {
                AnnotationType = "pointcloud.BoundingVolume",
                AnnotatedResourceId = threeDModel.Id,
                AnnotatedResourceType = "threedmodel",
                CreatingApp = "dotnet-integration-test",
                CreatingAppVersion = "1.2.1",
                CreatingUser = "user",
                Data = boundingVolume
            };
            var annotation = (
                await WriteClient.Playground.Annotations.SuggestAsync(new List<AnnotationSuggest> { suggestAnnotationQuery })

            ).FirstOrDefault();

            Assert.True(annotation.Status == "suggested", $"Expected the `suggested` Status but got {annotation.Status}");

            // Delete the created annotation and threedmodel
            await WriteClient.Playground.Annotations.DeleteAsync(items: new AnnotationDelete()
            { Items = new List<AnnotationId>() { new AnnotationId { Id = annotation.Id } } });
            await WriteClient.ThreeDModels.DeleteAsync(ids: new Identity[] { new Identity(threeDModel.Id) });

        }

        [Fact]
        [Trait("Description ", value: "Create, retrieve, update and delete annotation is OK")]
        public async Task AnnotationCrudValidAsync()
        {
            // Create the annotated resource (threedmodel and the annotation)
            var createThreeDModelQuery = new ThreeDModelCreate() { Name = "dotnet-integration-test-for-annotations" };
            var threeDModel = (await WriteClient.ThreeDModels.CreateAsync(new List<ThreeDModelCreate> { createThreeDModelQuery })).FirstOrDefault();
            var boundingVolume = new BoundingVolume()
            {
                Region = new List<Geometry>()
                {
                    new Geometry()
                    {
                        Cylinder = new Cylinder()
                        {
                            CenterA = new float[] {0, 1, 2},
                            CenterB = new float[] {2, 3, 4},
                            Radius = 3

                        }
                    }
                }
            };
            var createAnnotationQuery = new AnnotationCreate()
            {
                AnnotationType = "pointcloud.BoundingVolume",
                AnnotatedResourceId = threeDModel.Id,
                AnnotatedResourceType = "threedmodel",
                CreatingApp = "dotnet-integration-test",
                CreatingAppVersion = "1.2.1",
                CreatingUser = "",
                Data = boundingVolume,
                Status = "suggested"
            };
            var annotation = (
                await WriteClient.Playground.Annotations.CreateAsync(new List<AnnotationCreate> { createAnnotationQuery })

            ).FirstOrDefault();

            // Retrieve annotations
            var listAnnotationQuery = new AnnotationQuery()
            {
                Filter = new AnnotationFilter()
                {
                    AnnotatedResourceType = "threedmodel",
                    AnnotatedResourceIds = new Identity[] { new Identity(threeDModel.Id) }
                }
            };

            var listedAnnotation = (await WriteClient.Playground.Annotations.ListAsync(listAnnotationQuery)).Items;
            var annotationCount = listedAnnotation.Count();

            Assert.True(annotationCount == 1, $"Expected a single Annotation but got {annotationCount}");

            var resAnnotation = listedAnnotation.First();

            Assert.True(resAnnotation.Id == annotation.Id, "The retrieved annotation id doesn't match with the created one.");

            Assert.True(resAnnotation.Status == annotation.Status, "The status of the retrieved annotation doesn't match with the created one");

            // Update the annotation
            var updateBoundingVolume = new BoundingVolume
            {
                Region = new List<Geometry> {
                    new Geometry{Box=new Box {
                        Matrix = new float[] {0, 1, 2, 3,4,5,6,7,8,9,10,11,12,13,14,15}
                    }}
                }
            };
            var updateAnnotationQuery = new List<AnnotationUpdateItem> {
                new AnnotationUpdateItem(annotation.Id){
                    Update = new AnnotationUpdate {
                        Status = new Update<string>("rejected"),
                        Data = new Update<BoundingVolume>(updateBoundingVolume)
                    }
                }
            };
            var updatedAnnotation = (await WriteClient.Playground.Annotations.UpdateAsync(updateAnnotationQuery)
                .ConfigureAwait(false)).FirstOrDefault();
            Assert.True(updatedAnnotation.Id == annotation.Id, "The updated annotation id doesn't match with the created one.");
            Assert.True(updatedAnnotation.Status == "rejected", "The status of the annotation is not updated.");
            Assert.True(updatedAnnotation.Data.Region.First().Cylinder is null, "The data of the annotation is not updated.");
            Assert.True(updatedAnnotation.Data.Region.First().Box is not null, "The data of the annotation is not updated.");

            // Delete the created annotation and threedmodel
            await WriteClient.Playground.Annotations.DeleteAsync(items: new AnnotationDelete()
            { Items = new List<AnnotationId>() { new AnnotationId { Id = annotation.Id } } });
            await WriteClient.ThreeDModels.DeleteAsync(ids: new Identity[] { new Identity(threeDModel.Id) });

        }
    }
}

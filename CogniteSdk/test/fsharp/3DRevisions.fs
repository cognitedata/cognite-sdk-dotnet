module Tests.Integration.ThreeDRevisions

open System
open System.Collections.Generic

open FSharp.Control.Tasks
open Swensen.Unquote
open Oryx
open Xunit

open CogniteSdk
open Common

[<Fact>]
let ``List 3D revisions with limit is Ok`` () = task {
    // Arrange
    let query = ThreeDRevisionQuery(Limit = Nullable 1)
    let modelId = 4715379429968321L

    // Act
    let! res = readClient.ThreeDRevisions.ListAsync(modelId, query)
    let len = Seq.length res.Items
    let revision = res.Items |> Seq.head

    // Assert
    test <@ len = 1 @>
    test <@ revision.Id = 5688854005909501L @>
    test <@ revision.Status = "Done" @>
    test <@ revision.AssetMappingCount = 92L @>
}

[<Fact>]
let ``Get 3D revision by id is Ok`` () = task {
    // Arrange
    let modelId = 4715379429968321L
    let revisionId = 5688854005909501L

    // Act
    let! revision = readClient.ThreeDRevisions.RetrieveAsync(modelId, revisionId)

    // Assert
    test <@ revision.Id = 5688854005909501L @>
    test <@ revision.Status = "Done" @>
    test <@ revision.AssetMappingCount = 92L @>
}

// Must create a 3D file to create this test.
// [<Fact>]
// let ``Create and delete 3D revisions is Ok`` () = task {
//     // Arrange
//     let name = "sdk-test-model-" + Guid.NewGuid().ToString();
//     let rotation = [0.1; 0.2; 0.3]
//     let modelDto = ThreeDModelCreate(Name=name)
//     let revisionDto = ThreeDRevisionCreate(Published=false, Rotation=rotation)

//     // Act
//     let! models = writeClient.ThreeDModels.CreateAsync [modelDto]
//     let model = Seq.head models

//     let! revisions = writeClient.ThreeDRevisions.CreateAsync(model.Id, [revisionDto])
//     let revision = Seq.head revisions
//     let! delRevisionRes = writeClient.ThreeDRevisions.DeleteAsync(model.Id, [revision.Id])
//     let! delModelRes = writeClient.ThreeDModels.DeleteAsync [model.Id]

//     // Assert
//     test <@ model.Name = name @>
//     test <@ not revision.Published @>
//     test <@ revision.Rotation |> Seq.toList = rotation @>
// }

[<Fact>]
let ``List 3D revisions logs is Ok`` () = task {
    // Arrange
    let query = ThreeDRevisionLogQuery(Severity=Nullable 3L)
    let modelId = 4715379429968321L
    let revisionId = 5688854005909501L

    // Act
    let! res = readClient.ThreeDRevisions.ListLogsAsync(modelId, revisionId, query)
    let len = Seq.length res.Items
    let log = Seq.head res.Items

    // Assert
    test <@ len > 0 @>
    test <@ log.Severity = 3 @>
}

[<Fact>]
let ``List 3D nodes is Ok`` () = task {
    // Arrange
    let query = ThreeDNodeQuery(Limit=Nullable 10)
    let modelId = 4715379429968321L
    let revisionId = 5688854005909501L

    // Act
    let! res = readClient.ThreeDRevisions.ListNodesAsync(modelId, revisionId, query)
    let len = Seq.length res.Items
    let node = Seq.head res.Items

    // Assert
    test <@ len > 0 @>
}
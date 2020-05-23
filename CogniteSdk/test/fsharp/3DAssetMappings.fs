module Tests.Integration.ThreeDAssetMappings

open System
open System.Collections.Generic

open FSharp.Control.Tasks.V2.ContextInsensitive
open Swensen.Unquote
open Oryx
open Xunit

open CogniteSdk
open Common

[<Fact>]
let ``List 3D AssetMappings with limit is Ok`` () = task {
    // Arrange
    let query = ThreeDAssetMappingQuery()
    let modelId = 4715379429968321L
    let revisionId = 5688854005909501L

    // Act
    let! res = readClient.ThreeDAssetMappings.ListAsync(modelId, revisionId, query)
    let len = Seq.length res.Items
    let assetMapping = res.Items |> Seq.head

    // Assert
    test <@ len = 10 @>
    test <@ assetMapping.SubtreeSize > 0L @>
}

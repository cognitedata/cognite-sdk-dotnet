module Tests.Integration.Assets

open System
open System.Net.Http

open Swensen.Unquote
open Xunit

open CogniteSdk
open Common
open FSharp.Control.Tasks.V2.ContextInsensitive
open Oryx

[<Fact>]
let ``List assets with limit is Ok`` () = task {
    // Arrange
    let query = Assets.AssetQueryDto().WithLimit(10)

    // // Act
    let! res = readClient.Assets.ListAsync(query)
    let len = Seq.length res.Items

    // // Assert
    test <@ 10 = 10 @>
}

[<Fact>]
let ``Get asset by id is Ok`` () = task {
    // Arrange
    let assetId = 130452390632424L

    // Act
    let! res = readClient.Assets.GetAsync assetId

    let resId = res.Id

    // Assert
    test <@ resId = assetId @>
}

[<Fact>]
let ``Get asset by missing id is Error`` () = task {
    // Arrange
    let assetId = 0L

    // Act
    let! res =
        task {
            try
                let! a = readClient.Assets.GetAsync assetId
                return Ok a
            with
            | :? ResponseException as e -> return Error e
        }

    let err = Result.getError res

    // Assert
    test <@ Result.isError res @>
    test <@ err.Code = 400 @>
    test <@ err.Message.Contains "violations" @>
    test <@ not (isNull err.RequestId) @>
}

[<Fact>]
let ``Get asset by ids is Ok`` () = task {
    // Arrange
    let assetIds =
        [ 130452390632424L; 126847700303897L; 124419735577853L ]

    // Act
    let! res = readClient.Assets.RetrieveAsync assetIds

    let len = Seq.length res

    // Assert
    test <@ len = 3 @>
}

[<Fact>]
let ``Filter assets is Ok`` () = task {
    // Arrange
    let filter = Assets.AssetFilterDto(RootIds = [ Identity.CreateId(6687602007296940L) ])
    let query = Assets.AssetQueryDto(Limit = Nullable 10, Filter = filter)

    // Act
    let! res = readClient.Assets.ListAsync query

    let len = Seq.length res.Items

    // Assert
    test <@ len = 10 @>
}
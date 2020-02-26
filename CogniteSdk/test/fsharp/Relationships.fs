module Tests.Integration.Relationships

open System
open System.Collections.Generic

open FSharp.Control.Tasks.V2.ContextInsensitive
open Swensen.Unquote
open Oryx
open Xunit

open CogniteSdk
open CogniteSdk.Relationships
open Common

[<Fact>]
let ``Get relationship by ids is Ok`` () = task {
    // Arrange
    let relationshipIds = [ "relationship-test" ]

    // Act
    let! res = writeClient.Playground.Relationships.RetrieveAsync relationshipIds

    let len = Seq.length res.Items

    // Assert
    test <@ len = 1 @>
}

[<Fact>]
let ``Filter relationships on sources is Ok`` () = task {
    // Arrange
    let sources = RelationshipResource(Resource="asset", ResourceId="relationship-asset") |> Seq.singleton
    let sourcesList = new List<RelationshipResource>(sources)
    let filter = RelationshipFilterDto(Sources=sourcesList)
    let query = RelationshipQueryDto(Limit = Nullable 10, Filter = filter)

    // Act
    let! res = writeClient.Playground.Relationships.ListAsync query

    let len = Seq.length res.Items

    // Assert
    test <@ len = 1 @>
}

[<Fact>]
let ``Filter relationships on targets is Ok`` () = task {
    // Arrange
    let targets = RelationshipResource(Resource="timeseries", ResourceId="timeseries-relationship") |> Seq.singleton
    let targetsList = new List<RelationshipResource>(targets)
    let filter = RelationshipFilterDto(Targets=targetsList)
    let query = RelationshipQueryDto(Limit = Nullable 10, Filter = filter)

    // Act
    let! res = writeClient.Playground.Relationships.ListAsync query

    let len = Seq.length res.Items

    // Assert
    test <@ len = 1 @>
}

[<Fact>]
let ``Create and delete Relationships is Ok`` () = task {
    // Arrange
    let externalId = Guid.NewGuid().ToString();
    let source = RelationshipResource(Resource="asset", ResourceId=Guid.NewGuid().ToString();)
    let target = RelationshipResource(Resource="asset", ResourceId=Guid.NewGuid().ToString();)
    let dto =
        RelationshipWriteDto(
            ExternalId = externalId,
            Source = source,
            Target = target,
            Confidence = 1.0F,
            DataSet = "test",
            RelationshipType = "flowsTo"
        )

    // Act
    let! res = writeClient.Playground.Relationships.CreateAsync [ dto ]
    let! delRes = writeClient.Playground.Relationships.DeleteAsync [ externalId ]

    let resExternalId = (Seq.head res.Items).ExternalId

    // Assert
    test <@ resExternalId = externalId @>
}

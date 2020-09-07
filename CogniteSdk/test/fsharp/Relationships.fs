module Tests.Integration.Relationships

open System
open System.Collections.Generic

open FSharp.Control.Tasks.V2.ContextInsensitive
open Swensen.Unquote
open Oryx
open Xunit

open CogniteSdk
open Common

[<Fact>]
let ``Get relationship by ids is Ok`` () = task {
    // Arrange
    let relationshipIds = [ "relationship-test" ]

    // Act
    let! res = writeClient.Playground.Relationships.RetrieveAsync relationshipIds

    let len = Seq.length res

    // Assert
    test <@ len = 1 @>
}

[<Fact>]
let ``Filter relationships on sources is Ok`` () = task {
    // Arrange
    let sources = RelationshipResource(Resource="asset", ResourceId="relationship-asset") |> Seq.singleton
    let sourcesList = new List<RelationshipResource>(sources)
    let filter = RelationshipFilter(Sources=sourcesList)
    let query = RelationshipQuery(Limit = Nullable 10, Filter = filter)

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
    let filter = RelationshipFilter(Targets=targetsList)
    let query = RelationshipQuery(Limit = Nullable 10, Filter = filter)

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
        RelationshipCreate(
            ExternalId = externalId,
            Source = source,
            Target = target,
            Confidence = 1.0F,
            DataSet = "test",
            RelationshipType = "flowsTo",
            StartTime = 0L,
            EndTime = 1L
        )

    // Act
    let! res = writeClient.Playground.Relationships.CreateAsync [ dto ]
    let! delRes = writeClient.Playground.Relationships.DeleteAsync [ externalId ]

    let resExternalId = (Seq.head res).ExternalId

    // Assert
    test <@ resExternalId = externalId @>
}

[<Fact>]
let ``BETA: Relationships resource available`` () = task {
    // Arrange
    let filter = Beta.RelationshipFilter()
    let query = Beta.RelationshipQuery(Filter=filter, Limit=Nullable 1)
    // Act
    let! res = writeClient.Beta.Relationships.ListAsync query

    let len = Seq.length res.Items

    // Assert
    test <@ len <= 1 @>
}

[<Fact>]
let ``BETA: create and delete relationships is ok`` () = task {
    // Arrange
    // NOTE: Requires the Label: "RelationshipsTestCreateAndDeleteLabel" to be defined
    // Define Relationships to create
    let externalId = Guid.NewGuid().ToString()
    let label = "RelationshipsTestCreateAndDeleteLabel"
    let relationshipCreateObject =
        seq {
            Beta.RelationshipCreate(
                ExternalId=externalId,
                SourceExternalId="RelationshipTestCreateAndDeleteSource",
                SourceType="asset",
                TargetExternalId="RelationshipTestCreateAndDeleteTarget",
                TargetType="asset",
                Confidence=Nullable 0.999F,
                Labels=seq{CogniteExternalId(label)}
            )
        }

    let listFilter =
        Beta.RelationshipQuery(
            Filter=Beta.RelationshipFilter(Labels=LabelFilter.Any(label))
        )

    // Act
    let! createRes = writeClient.Beta.Relationships.CreateAsync relationshipCreateObject
    let! getExtIds = writeClient.Beta.Relationships.ListAsync listFilter

    let relationshipWasCreated = createRes |> Seq.map (fun r -> r.ExternalId = externalId) |> Seq.contains true

    // cleanup
    let deleteIds = getExtIds.Items |> Seq.map (fun relationship -> relationship.ExternalId)
    let! deleteResult = writeClient.Beta.Relationships.DeleteAsync deleteIds

    // Assert
    test <@ relationshipWasCreated@>
    test <@ createRes |> Seq.length >= 1 @>
    test <@ getExtIds.Items |> Seq.length > 0 @>
}

[<Fact>]
let ``BETA: retrieve relationship is ok`` () = task{
    // Arrange
    // create relationship to use for retrieval test
    let srcExternalId = "Temp-RelationshipTestRetrieveSource"
    let externalId = Guid.NewGuid().ToString()
    let relationshipCreateObject =
        seq {
            Beta.RelationshipCreate(
                ExternalId=externalId,
                SourceExternalId=srcExternalId,
                SourceType="asset",
                TargetExternalId="Temp-RelationshipTestRetrieveTarget",
                TargetType="asset",
                Confidence=Nullable 0.999F
            )
        }

    //ids to retrieve
    let retrieveIds =
        seq{
            externalId
        }
    // Act
    //create the relationship
    let! createRes =  writeClient.Beta.Relationships.CreateAsync relationshipCreateObject
    // retrieve the relationship by its externalId
    let! retrievedRelationships = writeClient.Beta.Relationships.RetrieveAsync retrieveIds

    // Cleanup: Delete all relationships that has the srcExternalId label
    let relationshipsFilter =
        Beta.RelationshipQuery(
            Filter=Beta.RelationshipFilter(
                SourceExternalIds=(srcExternalId|>Seq.singleton)
            )
        )
    let! relationshipsBySourceExtId = writeClient.Beta.Relationships.ListAsync relationshipsFilter
    let deleteIds = relationshipsBySourceExtId.Items |> Seq.map (fun relationship -> relationship.ExternalId)

    let! deleteResult = writeClient.Beta.Relationships.DeleteAsync deleteIds

    // Assert
    test <@ createRes |> Seq.length = 1 @>
    test <@ retrievedRelationships |> Seq.length > 0 @>
}

[<Fact>]
let ``BETA: filter by SourceExternalId is ok`` () = task {
    // Arrange
    // create relationship to use for retrieval test
    let targetExternalId = "test - filter by SourceExternalId is ok - deletable"
    let srcExternalIdToMatch = "test - filter by SourceExternalId is ok - shouldmatch"
    let srcExternalIdDontMatch = "test - filter by SourceExternalId is ok - shouldnotmatch"

    let externalId1 = Guid.NewGuid().ToString()
    let externalId2 = Guid.NewGuid().ToString()

    let relationshipCreateObject =
        seq {
            Beta.RelationshipCreate(
                ExternalId=externalId1,
                SourceExternalId=srcExternalIdToMatch,
                SourceType="asset",
                TargetExternalId=targetExternalId,
                TargetType="asset"
            );
            Beta.RelationshipCreate(
                ExternalId=externalId2,
                SourceExternalId=srcExternalIdDontMatch,
                SourceType="asset",
                TargetExternalId=targetExternalId,
                TargetType="asset"
            )
        }

    // Act
    //create the relationships for test
    let! createRes =  writeClient.Beta.Relationships.CreateAsync relationshipCreateObject
    // retrieve the relationship by sourceExternalId
    let relationshipsFilter =
        Beta.RelationshipQuery(
            Filter=Beta.RelationshipFilter(
                SourceExternalIds=(srcExternalIdToMatch|>Seq.singleton)
            )
        )
    let! relationships = writeClient.Beta.Relationships.ListAsync relationshipsFilter

    let allResultsMatchFilter =
        relationships.Items
        |> Seq.map (fun relationship -> relationship.SourceExternalId = srcExternalIdToMatch)
        |> Seq.forall id

    // Cleanup: Delete all relationships that has the target (deletable) label
    let! deleteRelationships =
        writeClient.Beta.Relationships.ListAsync (
            Beta.RelationshipQuery(
                    Filter=Beta.RelationshipFilter(
                        TargetExternalIds=(targetExternalId|>Seq.singleton)
                    )
                )
        )
    let deleteIds = deleteRelationships.Items|> Seq.map (fun r -> r.ExternalId)
    let! deleteResult = writeClient.Beta.Relationships.DeleteAsync (deleteIds)

    // Assert
    test <@ createRes |> Seq.length = 2 @>
    test <@ relationships.Items |> Seq.length > 0 @>
    test <@ allResultsMatchFilter @>
}

[<Fact>]
let ``BETA: filter by TargetExternalId is ok`` () = task {
    // Arrange
    // create relationship to use for retrieval test
    let sourceExternalId = "test - filter by TargetExternalId is ok - deletable"
    let targetExternalIdToMatch = "test - filter by TargetExternalId is ok - shouldmatch"
    let targetExternalIdDontMatch = "test - filter by TargetExternalId is ok - shouldnotmatch"

    let externalId1 = Guid.NewGuid().ToString()
    let externalId2 = Guid.NewGuid().ToString()

    let relationshipCreateObject =
        seq {
            Beta.RelationshipCreate(
                ExternalId=externalId1,
                SourceExternalId=sourceExternalId,
                SourceType="asset",
                TargetExternalId=targetExternalIdToMatch,
                TargetType="asset"
            );
            Beta.RelationshipCreate(
                ExternalId=externalId2,
                SourceExternalId=sourceExternalId,
                SourceType="asset",
                TargetExternalId=targetExternalIdDontMatch,
                TargetType="asset"
            )
        }

    // Act
    //create the relationships for test
    let! createRes =  writeClient.Beta.Relationships.CreateAsync relationshipCreateObject
    // retrieve the relationship by sourceExternalId
    let relationshipsFilter =
        Beta.RelationshipQuery(
            Filter=Beta.RelationshipFilter(
                TargetExternalIds=(targetExternalIdToMatch|>Seq.singleton)
            )
        )
    let! relationships = writeClient.Beta.Relationships.ListAsync relationshipsFilter

    let allResultsMatchFilter =
        relationships.Items
        |> Seq.map (fun relationship -> relationship.TargetExternalId = targetExternalIdToMatch)
        |> Seq.forall id

    // Cleanup: Delete all relationships that has the target (deletable) label
    let! deleteRelationships =
        writeClient.Beta.Relationships.ListAsync (
            Beta.RelationshipQuery(
                    Filter=Beta.RelationshipFilter(
                        SourceExternalIds = (sourceExternalId |> Seq.singleton)
                    )
                )
        )
    let deleteIds = deleteRelationships.Items |> Seq.map (fun r -> r.ExternalId)
    let! deleteResult = writeClient.Beta.Relationships.DeleteAsync (deleteIds)

    // Assert
    test <@ createRes |> Seq.length = 2 @>
    test <@ relationships.Items |> Seq.length > 0 @>
    test <@ allResultsMatchFilter @>
}

[<Fact>]
let ``BETA: filter by SourceType is ok`` () = task {
    // Arrange
    // create relationship to use for retrieval test
    let sourceExternalId = "test - filter by SourceType is ok - deletable"
    let targetExternalId = "test - filter by SourceType is ok - deletable"

    let sourceTypeToMatch = "event"
    let sourceTypeDontMatch = "asset"

    let externalId1 = Guid.NewGuid().ToString()
    let externalId2 = Guid.NewGuid().ToString()

    let relationshipCreateObject =
        seq {
            Beta.RelationshipCreate(
                ExternalId=externalId1,
                SourceExternalId=sourceExternalId,
                TargetExternalId=targetExternalId,
                TargetType="asset",

                SourceType=sourceTypeToMatch
            );
            Beta.RelationshipCreate(
                ExternalId=externalId2,
                SourceExternalId=sourceExternalId,
                TargetExternalId=targetExternalId,
                TargetType="asset",

                SourceType=sourceTypeDontMatch
            )
        }

    // Act
    //create the relationships for test
    let! createRes =  writeClient.Beta.Relationships.CreateAsync relationshipCreateObject
    // retrieve the relationship by sourceExternalId
    let relationshipsFilter =
        Beta.RelationshipQuery(
            Filter=Beta.RelationshipFilter(
                SourceTypes=(sourceTypeToMatch|>Seq.singleton)
            )
        )
    let! relationships = writeClient.Beta.Relationships.ListAsync relationshipsFilter

    let allResultsMatchFilter =
        relationships.Items
        |> Seq.map (fun relationship -> relationship.SourceType = sourceTypeToMatch)
        |> Seq.forall id

    // Cleanup: Delete all relationships that has this test's (deletable) sourceExternalId
    let! deleteRelationships =
        writeClient.Beta.Relationships.ListAsync (
            Beta.RelationshipQuery(
                    Filter=Beta.RelationshipFilter(
                        SourceExternalIds = (sourceExternalId |> Seq.singleton)
                    )
                )
        )
    let deleteIds = deleteRelationships.Items |> Seq.map (fun r -> r.ExternalId)
    let! deleteResult = writeClient.Beta.Relationships.DeleteAsync (deleteIds)

    // Assert
    test <@ createRes |> Seq.length = 2 @>
    test <@ relationships.Items |> Seq.length > 0 @>
    test <@ allResultsMatchFilter @>
}

[<Fact>]
let ``BETA: filter by TargetType is ok`` () = task {
    // Arrange
    // create relationship to use for retrieval test
    let sourceExternalId = "test - filter by TargetType is ok - deletable"
    let targetExternalId = "test - filter by TargetType is ok - deletable"

    let targetTypeToMatch = "event"
    let targetTypeDontMatch = "asset"

    let externalId1 = Guid.NewGuid().ToString()
    let externalId2 = Guid.NewGuid().ToString()

    let relationshipCreateObject =
        seq {
            Beta.RelationshipCreate(
                ExternalId=externalId1,
                SourceExternalId=sourceExternalId,
                TargetExternalId=targetExternalId,
                SourceType="asset",

                TargetType=targetTypeToMatch
            );
            Beta.RelationshipCreate(
                ExternalId=externalId2,
                SourceExternalId=sourceExternalId,
                TargetExternalId=targetExternalId,
                SourceType="asset",

                TargetType=targetTypeDontMatch
            )
        }

    // Act
    //create the relationships for test
    let! createRes =  writeClient.Beta.Relationships.CreateAsync relationshipCreateObject
    // retrieve the relationship by targetTypeToMatch
    let relationshipsFilter =
        Beta.RelationshipQuery(
            Filter=Beta.RelationshipFilter(
                TargetTypes = ( targetTypeToMatch |> Seq.singleton )
            )
        )
    let! relationships = writeClient.Beta.Relationships.ListAsync relationshipsFilter

    let allResultsMatchFilter =
        relationships.Items
        |> Seq.forall (fun relationship -> relationship.TargetType = targetTypeToMatch)

    // Cleanup: Delete all relationships that has this test's (deletable) sourceExternalId
    let! deleteRelationships =
        writeClient.Beta.Relationships.ListAsync (
            Beta.RelationshipQuery(
                    Filter=Beta.RelationshipFilter(
                        SourceExternalIds = (sourceExternalId |> Seq.singleton)
                    )
                )
        )
    let deleteIds = deleteRelationships.Items |> Seq.map (fun r -> r.ExternalId)
    let! deleteResult = writeClient.Beta.Relationships.DeleteAsync (deleteIds)

    // Assert
    test <@ createRes |> Seq.length = 2 @>
    test <@ relationships.Items |> Seq.length > 0 @>
    test <@ allResultsMatchFilter @>
}

[<Fact>]
let ``BETA: filter by Confidence is ok`` () = task {
    // Arrange
    // create relationship to use for retrieval test
    let sourceExternalId = "test - filter by Confidence is ok - deletable"
    let targetExternalId = "test - filter by Confidence is ok - deletable"

    let relationshiptype = "asset"

    let externalId1 = Guid.NewGuid().ToString()
    let externalId2 = Guid.NewGuid().ToString()

    let relationshipCreateObject =
        seq {
            Beta.RelationshipCreate(
                ExternalId=externalId1,
                SourceExternalId=sourceExternalId,
                TargetExternalId=targetExternalId,
                SourceType=relationshiptype,
                TargetType=relationshiptype,

                Confidence = Nullable 1.0F
            );
            Beta.RelationshipCreate(
                ExternalId=externalId2,
                SourceExternalId=sourceExternalId,
                TargetExternalId=targetExternalId,
                SourceType=relationshiptype,
                TargetType=relationshiptype,

                Confidence = Nullable 0.5F
            )
        }

    // Act
    //create the relationships for test
    let! createRes =  writeClient.Beta.Relationships.CreateAsync relationshipCreateObject
    // retrieve the relationship by targetTypeToMatch
    let relationshipsFilter =
        Beta.RelationshipQuery(
            Filter=Beta.RelationshipFilter(
                SourceExternalIds = ( sourceExternalId |> Seq.singleton ),
                Confidence = RangeFloat(Min = Nullable 0.99F)
            )
        )
    let! relationships = writeClient.Beta.Relationships.ListAsync relationshipsFilter

    let allResultsMatchFilter =
        relationships.Items
        |> Seq.forall ( fun relationship -> relationship.Confidence = Nullable 1.0F )

    // Cleanup: Delete all relationships that has this test's (deletable) sourceExternalId
    let! deleteRelationships =
        writeClient.Beta.Relationships.ListAsync (
            Beta.RelationshipQuery(
                    Filter=Beta.RelationshipFilter(
                        SourceExternalIds = (sourceExternalId |> Seq.singleton)
                    )
                )
        )
    let deleteIds = deleteRelationships.Items |> Seq.map (fun r -> r.ExternalId)
    let! deleteResult = writeClient.Beta.Relationships.DeleteAsync (deleteIds)

    // Assert
    test <@ createRes |> Seq.length = 2 @>
    test <@ relationships.Items |> Seq.length > 0 @>
    test <@ allResultsMatchFilter @>
}

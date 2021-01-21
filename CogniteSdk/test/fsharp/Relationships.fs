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
let ``Relationships resource available`` () = task {
    // Arrange
    let filter = RelationshipFilter()
    let query = RelationshipQuery(Filter=filter, Limit=Nullable 1)
    // Act
    let! res = writeClient.Relationships.ListAsync query

    let len = Seq.length res.Items

    // Assert
    test <@ len <= 1 @>
}

[<Fact>]
let ``create and delete relationships is ok`` () = task {
    // Arrange
    // NOTE: Requires the Label: "RelationshipsTestCreateAndDeleteLabel" to be defined
    // Define Relationships to create
    let externalId = Guid.NewGuid().ToString()
    let label = "RelationshipsTestCreateAndDeleteLabel"
    let relationshipCreateObject =
        seq {
            RelationshipCreate(
                ExternalId = externalId,
                SourceExternalId = "RelationshipTestCreateAndDeleteSource",
                SourceType = RelationshipVertexType.Asset,
                TargetExternalId = "RelationshipTestCreateAndDeleteTarget",
                TargetType = RelationshipVertexType.Asset,
                Confidence = Nullable 0.999F,
                Labels = ( CogniteExternalId(label) |> Seq.singleton )
            )
        }

    let listFilter =
        RelationshipQuery(
            Filter=RelationshipFilter(Labels=LabelFilter.Any(label))
        )

    // Act
    let! createRes = writeClient.Relationships.CreateAsync relationshipCreateObject
    let! getExtIds = writeClient.Relationships.ListAsync listFilter

    let relationshipWasCreated = createRes |> Seq.map (fun r -> r.ExternalId = externalId) |> Seq.contains true

    // cleanup
    let deleteIds = getExtIds.Items |> Seq.map (fun relationship -> relationship.ExternalId)
    let! deleteResult = writeClient.Relationships.DeleteAsync deleteIds

    // Assert
    test <@ relationshipWasCreated@>
    test <@ createRes |> Seq.length >= 1 @>
    test <@ getExtIds.Items |> Seq.length > 0 @>
}

[<Fact>]
let ``retrieve relationship is ok`` () = task{
    // Arrange
    // create relationship to use for retrieval test
    let srcExternalId = "Temp-RelationshipTestRetrieveSource"
    let externalId = Guid.NewGuid().ToString()
    let relationshipCreateObject =
        seq {
            RelationshipCreate(
                ExternalId = externalId,
                SourceExternalId = srcExternalId,
                SourceType = RelationshipVertexType.Asset,
                TargetExternalId = "Temp-RelationshipTestRetrieveTarget",
                TargetType = RelationshipVertexType.Asset,
                Confidence = Nullable 0.999F
            )
        }

    //ids to retrieve
    let retrieveIds = externalId |> Seq.singleton

    // Act
    //create the relationship
    let! createRes =  writeClient.Relationships.CreateAsync relationshipCreateObject
    // retrieve the relationship by its externalId
    let! retrievedRelationships = writeClient.Relationships.RetrieveAsync retrieveIds

    // Cleanup: Delete all relationships that has the srcExternalId label
    let relationshipsFilter =
        RelationshipQuery(
            Filter=RelationshipFilter(
                SourceExternalIds=(srcExternalId|>Seq.singleton)
            )
        )
    let! relationshipsBySourceExtId = writeClient.Relationships.ListAsync relationshipsFilter
    let deleteIds = relationshipsBySourceExtId.Items |> Seq.map (fun relationship -> relationship.ExternalId)

    let! deleteResult = writeClient.Relationships.DeleteAsync deleteIds

    // Assert
    test <@ createRes |> Seq.length = 1 @>
    test <@ retrievedRelationships |> Seq.length > 0 @>
}

[<Fact>]
let ``filter by SourceExternalId is ok`` () = task {
    // Arrange
    // create relationship to use for retrieval test
    let targetExternalId = "test - filter by SourceExternalId is ok - deletable"
    let srcExternalIdToMatch = "test - filter by SourceExternalId is ok - shouldmatch"
    let srcExternalIdDontMatch = "test - filter by SourceExternalId is ok - shouldnotmatch"

    let externalId1 = Guid.NewGuid().ToString()
    let externalId2 = Guid.NewGuid().ToString()

    let relationshipCreateObject =
        seq {
            RelationshipCreate(
                ExternalId=externalId1,
                SourceExternalId=srcExternalIdToMatch,
                SourceType=RelationshipVertexType.Asset,
                TargetExternalId=targetExternalId,
                TargetType=RelationshipVertexType.Asset
            );
            RelationshipCreate(
                ExternalId=externalId2,
                SourceExternalId=srcExternalIdDontMatch,
                SourceType=RelationshipVertexType.Asset,
                TargetExternalId=targetExternalId,
                TargetType=RelationshipVertexType.Asset
            )
        }

    // Act
    //create the relationships for test
    let! createRes =  writeClient.Relationships.CreateAsync relationshipCreateObject
    // retrieve the relationship by sourceExternalId
    let relationshipsFilter =
        RelationshipQuery(
            Filter = RelationshipFilter(
                SourceExternalIds = (srcExternalIdToMatch|>Seq.singleton)
            )
        )
    let! relationships = writeClient.Relationships.ListAsync relationshipsFilter

    let allResultsMatchFilter =
        relationships.Items
        |> Seq.map (fun relationship -> relationship.SourceExternalId = srcExternalIdToMatch)
        |> Seq.forall id

    // Cleanup: Delete all relationships that has the target (deletable) label
    let! deleteRelationships =
        writeClient.Relationships.ListAsync (
            RelationshipQuery(
                    Filter = RelationshipFilter(
                        TargetExternalIds = (targetExternalId |> Seq.singleton)
                    )
                )
        )
    let deleteIds = deleteRelationships.Items|> Seq.map (fun r -> r.ExternalId)
    let! deleteResult = writeClient.Relationships.DeleteAsync (deleteIds)

    // Assert
    test <@ createRes |> Seq.length = 2 @>
    test <@ relationships.Items |> Seq.length > 0 @>
    test <@ allResultsMatchFilter @>
}

[<Fact>]
let ``filter by TargetExternalId is ok`` () = task {
    // Arrange
    // create relationship to use for retrieval test
    let sourceExternalId = "test - filter by TargetExternalId is ok - deletable"
    let targetExternalIdToMatch = "test - filter by TargetExternalId is ok - shouldmatch"
    let targetExternalIdDontMatch = "test - filter by TargetExternalId is ok - shouldnotmatch"

    let externalId1 = Guid.NewGuid().ToString()
    let externalId2 = Guid.NewGuid().ToString()

    let relationshipCreateObject =
        seq {
            RelationshipCreate(
                ExternalId = externalId1,
                SourceExternalId = sourceExternalId,
                SourceType = RelationshipVertexType.Asset,
                TargetExternalId = targetExternalIdToMatch,
                TargetType = RelationshipVertexType.Asset
            );
            RelationshipCreate(
                ExternalId = externalId2,
                SourceExternalId = sourceExternalId,
                SourceType = RelationshipVertexType.Asset,
                TargetExternalId = targetExternalIdDontMatch,
                TargetType = RelationshipVertexType.Asset
            )
        }

    // Act
    //create the relationships for test
    let! createRes =  writeClient.Relationships.CreateAsync relationshipCreateObject
    // retrieve the relationship by sourceExternalId
    let relationshipsFilter =
        RelationshipQuery(
            Filter = RelationshipFilter(
                TargetExternalIds = (targetExternalIdToMatch|>Seq.singleton)
            )
        )
    let! relationships = writeClient.Relationships.ListAsync relationshipsFilter

    let allResultsMatchFilter =
        relationships.Items
        |> Seq.map (fun relationship -> relationship.TargetExternalId = targetExternalIdToMatch)
        |> Seq.forall id

    // Cleanup: Delete all relationships that has the target (deletable) label
    let! deleteRelationships =
        writeClient.Relationships.ListAsync (
            RelationshipQuery(
                    Filter = RelationshipFilter(
                        SourceExternalIds = (sourceExternalId |> Seq.singleton)
                    )
                )
        )
    let deleteIds = deleteRelationships.Items |> Seq.map (fun r -> r.ExternalId)
    let! deleteResult = writeClient.Relationships.DeleteAsync (deleteIds)

    // Assert
    test <@ createRes |> Seq.length = 2 @>
    test <@ relationships.Items |> Seq.length > 0 @>
    test <@ allResultsMatchFilter @>
}

[<Fact>]
let ``filter by SourceType is ok`` () = task {
    // Arrange
    // create relationship to use for retrieval test
    let sourceExternalId = "test - filter by SourceType is ok - deletable"
    let targetExternalId = "test - filter by SourceType is ok - deletable"

    let sourceTypeToMatch = RelationshipVertexType.Event
    let sourceTypeDontMatch = RelationshipVertexType.Asset

    let externalId1 = Guid.NewGuid().ToString()
    let externalId2 = Guid.NewGuid().ToString()

    let relationshipCreateObject =
        seq {
            RelationshipCreate(
                ExternalId=externalId1,
                SourceExternalId=sourceExternalId,
                TargetExternalId=targetExternalId,
                TargetType=RelationshipVertexType.Asset,

                SourceType=sourceTypeToMatch
            );
            RelationshipCreate(
                ExternalId=externalId2,
                SourceExternalId=sourceExternalId,
                TargetExternalId=targetExternalId,
                TargetType=RelationshipVertexType.Asset,

                SourceType=sourceTypeDontMatch
            )
        }

    // Act
    //create the relationships for test
    let! createRes =  writeClient.Relationships.CreateAsync relationshipCreateObject
    // retrieve the relationship by the expected sourcetype
    let relationshipsFilter =
        RelationshipQuery(
            Filter=RelationshipFilter(
                SourceTypes=(sourceTypeToMatch|>Seq.singleton)
            )
        )
    let! relationships = writeClient.Relationships.ListAsync relationshipsFilter

    let allResultsMatchFilter =
        relationships.Items
        |> Seq.map (fun relationship -> relationship.SourceType = sourceTypeToMatch)
        |> Seq.forall id

    // Cleanup: Delete all relationships that has this test's (deletable) sourceExternalId
    let! deleteRelationships =
        writeClient.Relationships.ListAsync (
            RelationshipQuery(
                    Filter=RelationshipFilter(
                        SourceExternalIds = (sourceExternalId |> Seq.singleton)
                    )
                )
        )
    let deleteIds = deleteRelationships.Items |> Seq.map (fun r -> r.ExternalId)
    let! deleteResult = writeClient.Relationships.DeleteAsync (deleteIds)

    // Assert
    test <@ createRes |> Seq.length = 2 @>
    test <@ relationships.Items |> Seq.length > 0 @>
    test <@ allResultsMatchFilter @>
}

[<Fact>]
let ``filter by TargetType is ok`` () = task {
    // Arrange
    // create relationship to use for retrieval test
    let sourceExternalId = "test - filter by TargetType is ok - deletable"
    let targetExternalId = "test - filter by TargetType is ok - deletable"

    let targetTypeToMatch = RelationshipVertexType.Event
    let targetTypeDontMatch = RelationshipVertexType.Asset

    let externalId1 = Guid.NewGuid().ToString()
    let externalId2 = Guid.NewGuid().ToString()

    let relationshipCreateObject =
        seq {
            RelationshipCreate(
                ExternalId = externalId1,
                SourceExternalId = sourceExternalId,
                TargetExternalId = targetExternalId,
                SourceType = RelationshipVertexType.Asset,

                TargetType = targetTypeToMatch
            );
            RelationshipCreate(
                ExternalId = externalId2,
                SourceExternalId = sourceExternalId,
                TargetExternalId = targetExternalId,
                SourceType = RelationshipVertexType.Asset,

                TargetType = targetTypeDontMatch
            )
        }

    // Act
    //create the relationships for test
    let! createRes =  writeClient.Relationships.CreateAsync relationshipCreateObject
    // retrieve the relationship by targetTypeToMatch
    let relationshipsFilter =
        RelationshipQuery(
            Filter=RelationshipFilter(
                TargetTypes = ( targetTypeToMatch |> Seq.singleton )
            )
        )
    let! relationships = writeClient.Relationships.ListAsync relationshipsFilter

    let allResultsMatchFilter =
        relationships.Items
        |> Seq.forall (fun relationship -> relationship.TargetType = targetTypeToMatch)

    // Cleanup: Delete all relationships that has this test's (deletable) sourceExternalId
    let! deleteRelationships =
        writeClient.Relationships.ListAsync (
            RelationshipQuery(
                    Filter = RelationshipFilter(
                        SourceExternalIds = (sourceExternalId |> Seq.singleton)
                    )
                )
        )
    let deleteIds = deleteRelationships.Items |> Seq.map (fun r -> r.ExternalId)
    let! deleteResult = writeClient.Relationships.DeleteAsync (deleteIds)

    // Assert
    test <@ createRes |> Seq.length = 2 @>
    test <@ relationships.Items |> Seq.length > 0 @>
    test <@ allResultsMatchFilter @>
}

[<Fact>]
let ``filter by Confidence is ok`` () = task {
    // Arrange
    // create relationship to use for retrieval test
    let sourceExternalId = "test - filter by Confidence is ok - deletable"
    let targetExternalId = "test - filter by Confidence is ok - deletable"

    let relationshiptype = RelationshipVertexType.Asset

    let externalId1 = Guid.NewGuid().ToString()
    let externalId2 = Guid.NewGuid().ToString()

    let relationshipCreateObject =
        seq {
            RelationshipCreate(
                ExternalId = externalId1,
                SourceExternalId = sourceExternalId,
                TargetExternalId = targetExternalId,
                SourceType = relationshiptype,
                TargetType = relationshiptype,

                Confidence = Nullable 1.0F
            );
            RelationshipCreate(
                ExternalId = externalId2,
                SourceExternalId = sourceExternalId,
                TargetExternalId = targetExternalId,
                SourceType = relationshiptype,
                TargetType = relationshiptype,

                Confidence = Nullable 0.5F
            )
        }

    // Act
    //create the relationships for test
    let! createRes =  writeClient.Relationships.CreateAsync relationshipCreateObject
    // retrieve the relationship by range that includes expected confidence
    let relationshipsFilter =
        RelationshipQuery(
            Filter=RelationshipFilter(
                SourceExternalIds = ( sourceExternalId |> Seq.singleton ),
                Confidence = RangeFloat(Min = Nullable 0.99F)
            )
        )
    let! relationships = writeClient.Relationships.ListAsync relationshipsFilter

    let allResultsMatchFilter =
        relationships.Items
        |> Seq.forall ( fun relationship -> relationship.Confidence = Nullable 1.0F )

    // Cleanup: Delete all relationships that has this test's (deletable) sourceExternalId
    let! deleteRelationships =
        writeClient.Relationships.ListAsync (
            RelationshipQuery(
                    Filter=RelationshipFilter(
                        SourceExternalIds = (sourceExternalId |> Seq.singleton)
                    )
                )
        )
    let deleteIds = deleteRelationships.Items |> Seq.map (fun r -> r.ExternalId)
    let! deleteResult = writeClient.Relationships.DeleteAsync (deleteIds)

    // Assert
    test <@ createRes |> Seq.length = 2 @>
    test <@ relationships.Items |> Seq.length > 0 @>
    test <@ allResultsMatchFilter @>
}

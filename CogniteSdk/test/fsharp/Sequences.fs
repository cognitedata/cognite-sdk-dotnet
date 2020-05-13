module Tests.Integration.Sequences

open System
open System.Collections.Generic
open System.Net.Http
open FSharp.Control.Tasks.V2.ContextInsensitive

open Xunit
open Swensen.Unquote

open CogniteSdk

open Common

[<Trait("resource", "sequences")>]
[<Fact>]
let ``List Sequences with limit is Ok`` () = task {
    // Arrange
    let query =
        SequenceQuery(
            Limit = Nullable 10
        )

    // Act
    let! res = writeClient.Sequences.ListAsync query

    let dtos = res.Items
    let len = Seq.length dtos

    // Assert
    test <@ len > 0 @>
}

[<Trait("resource", "sequences")>]
[<Fact>]
let ``List Sequences with limit and filter is Ok`` () = task {
    // Arrange
    let query =
        SequenceQuery(
            Limit = Nullable 10,
            Filter = SequenceFilter(Name="sdk-test-sequence")
        )

    // Act
    let! res = writeClient.Sequences.ListAsync query

    // Act
    let! res = writeClient.Sequences.ListAsync query

    let dtos = res.Items
    let len = Seq.length dtos

    // Assert
    test <@ len > 0 @>}

[<Trait("resource", "sequences")>]
[<Fact>]
let ``Count Sequences is Ok`` () = task {
    // Arrange
    let query = SequenceQuery()

    // Act
    let! count = writeClient.Sequences.CountAsync query

    // Assert
    test <@ count > 0 @>
}


[<Trait("resource", "sequences")>]
[<Fact>]
let ``Get sequences by ids is Ok`` () = task {
    // Arrange
    let sequencesIds = [ 5702374195409554L ]

    // Act
    let! dtos = writeClient.Sequences.RetrieveAsync sequencesIds

    let len = Seq.length dtos

    test <@ len = 1 @>
}

[<Trait("resource", "sequences")>]
[<Fact>]
let ``Get sequence rows by ids is Ok`` () = task {
    // Arrange
    let sequencesId = 5702374195409554L
    let query =
        SequenceRowQuery(
            Id = Nullable sequencesId
        )

    // Act
    let! dto = writeClient.Sequences.ListRowsAsync query

    let colLength = Seq.length dto.Columns
    let rowLength = Seq.length dto.Rows
    let values =
        dto.Rows
        |> Seq.collect (fun x -> x.Values)
        |> Seq.map (fun value ->
            match value with
            | :? MultiValue.String as value -> value.Value
            | _ -> failwith "Expected string value"
        )
        |> List.ofSeq

    test <@ rowLength = 2 @>
    test <@ colLength = 1 @>
    test <@ dto.Id = Nullable 5702374195409554L @>
    test <@ Seq.length dto.Rows = 2 @>
    test <@ values = ["row1"; "row2"] @>
}

[<Trait("resource", "sequences")>]
[<Fact>]
let ``Create and delete sequences is Ok`` () = task {
    // Arrange
    let columnExternalIdString = Guid.NewGuid().ToString()
    let externalIdString = Guid.NewGuid().ToString()
    let name = Guid.NewGuid().ToString()
    let column =
        SequenceColumnWrite(
            Name = "Create column sdk test",
            ExternalId = columnExternalIdString,
            Description = "dotnet sdk test",
            ValueType = MultiValueType.DOUBLE
        )
    let dto =
        SequenceCreate(
            ExternalId = externalIdString,
            Name = name,
            Description = "dotnet sdk test",
            Columns = [column]
        )
    let externalId = Identity externalIdString

    // Act
    let! dtos = writeClient.Sequences.CreateAsync [ dto ]
    let! delRes = writeClient.Sequences.DeleteAsync [ externalId ]

    let len = Seq.length dtos

    let resExternalId =
        let h = Seq.tryHead dtos
        match h with
        | Some sequenceResponse -> sequenceResponse.ExternalId
        | None -> null

    // Assert
    test <@ resExternalId = externalIdString @>
}

[<Trait("resource", "sequences")>]
[<Fact>]
let ``Create and delete sequences rows is Ok`` () = task {
    // Arrange
    let columnExternalIdString = Guid.NewGuid().ToString()
    let externalIdString = Guid.NewGuid().ToString()
    let externalId = externalIdString
    let name = Guid.NewGuid().ToString()
    let column =
        SequenceColumnWrite(
            Name = "Create column sdk test",
            ExternalId = columnExternalIdString,
            Description = "dotnet sdk test",
            ValueType = MultiValueType.STRING
        )
    let dto =
        SequenceCreate(
            ExternalId = externalIdString,
            Name = name,
            Description = "dotnet sdk test",
            Columns = [column]
        )
    let deleteDto =
        SequenceRowDelete(
            Rows = [],
            ExternalId = externalId
        )


    let rows =
        [
            SequenceRow(RowNumber = 1L, Values = [MultiValue.Create "row1"])
            SequenceRow(RowNumber = 2L, Values = [MultiValue.Create "row2"])
        ] |> Seq.ofList

    let rowDto =
        SequenceDataCreate(
            Columns = ["sdk-column"],
            Rows = rows,
            Id = Nullable 5702374195409554L
        )

    // Act
    let! res = writeClient.Sequences.CreateAsync [ dto ]
    let! rowRes = writeClient.Sequences.CreateRowsAsync [ rowDto ]
    let! rowDelRes = writeClient.Sequences.DeleteRowsAsync [ deleteDto ]
    let! delRes = writeClient.Sequences.DeleteAsync [ externalId ]

    let resExternalId = (Seq.head res).ExternalId

    // Assert
    test <@ resExternalId = externalIdString @>
}

[<Trait("resource", "sequences")>]
[<Fact>]
let ``Search sequences is Ok`` () = task {
    // Arrange
    let query =
        SequenceSearch(
            Search=Search(Name = "sdk-test"),
            Limit = Nullable 10
        )

    // Act
    let! dtos = writeClient.Sequences.SearchAsync query

    let len = Seq.length dtos

    // Assert
    test <@ len > 0 @>
}

[<Trait("resource", "sequences")>]
[<Fact>]
let ``Update sequences is Ok`` () = task {
    // Arrange
    let columnExternalIdString = Guid.NewGuid().ToString()
    let externalIdString = Guid.NewGuid().ToString()
    let newMetadata = Dictionary(dict [
        "key1", "value1"
        "key2", "value2"
    ])
    let oldMetadata = Dictionary(dict [
            "oldkey1", "oldvalue1"
            "oldkey2", "oldvalue2"
        ])
    let column =
        SequenceColumnWrite(
            Name = "Create column sdk test",
            ExternalId = columnExternalIdString,
            Description = "dotnet sdk test",
            ValueType = MultiValueType.DOUBLE
        )
    let dto =
        SequenceCreate(
            ExternalId = externalIdString,
            Name = "Create Sequences sdk test",
            Description = "dotnet sdk test",
            Metadata = oldMetadata,
            AssetId = Nullable 5409900891232494L,
            Columns = [column]
        )

    let newName = "UpdatedName"
    let newExternalId = Guid.NewGuid().ToString();

    // Act
    let! createRes = writeClient.Sequences.CreateAsync [ dto ]
    let! updateRes =
        writeClient.Sequences.UpdateAsync [
            SequenceUpdateItem(
                externalId = externalIdString,
                Update=SequenceUpdate(
                    Name = UpdateNullable(newName),
                    ExternalId = UpdateNullable(newExternalId),
                    Metadata = UpdateDictionary(newMetadata, [ "oldkey1" ])
                )
            )
        ]
    let! getRes = writeClient.Sequences.RetrieveAsync [ Identity newExternalId ]

    let resName, resExternalId, resMetaData =
        let h = Seq.tryHead getRes
        match h with
        | Some sequenceResponse -> sequenceResponse.Name, sequenceResponse.ExternalId, sequenceResponse.Metadata
        | None -> "", "", Dictionary ()

    let metaDataOk =
        resMetaData.["key1"] = "value1"
        && resMetaData.["key2"] = "value2"
        && resMetaData.ContainsKey "oldkey2"
        && not (resMetaData.ContainsKey "oldkey1")

    // Assert get
    test <@ resExternalId = newExternalId @>
    test <@ resName = newName @>
    test <@ metaDataOk @>

    let newDescription = "updatedDescription"
    let newMetadata = Dictionary (dict [("newKey", "newValue")])

    let! updateRes2 =
        writeClient.Sequences.UpdateAsync [
            SequenceUpdateItem(
                externalId = newExternalId,
                Update=SequenceUpdate(
                    Description = UpdateNullable(newDescription),
                    AssetId = UpdateNullable(Nullable 5409900891232494L),
                    Metadata = UpdateDictionary(newMetadata)
                )
            )]

    let! getRes2 = writeClient.Sequences.RetrieveAsync [ Identity newExternalId ]

    let resDescription, resAssetId, resMetaData2, identity =
        let h = Seq.tryHead getRes2
        match h with
        | Some sequenceResponse ->
            sequenceResponse.Description, sequenceResponse.AssetId, sequenceResponse.Metadata, sequenceResponse.Id
        | None -> "", Nullable 0L, Dictionary(), 0L

    // Assert get2
    test <@ resDescription = newDescription @>
    test <@ resAssetId = Nullable 5409900891232494L @>
    test <@ resMetaData2.["newKey"] = "newValue" @>

    let! updateRes3 =
        writeClient.Sequences.UpdateAsync [
            SequenceUpdateItem(
                id = identity,
                Update=SequenceUpdate(
                    ExternalId = UpdateNullable(null),
                    AssetId = UpdateNullable(Nullable ()),
                    Metadata = UpdateDictionary(Dictionary(), ["newKey"])
                )
            )]

    let! getRes3 = writeClient.Sequences.RetrieveAsync [ Identity identity ]
    let! delRes = writeClient.Sequences.DeleteAsync [ Identity identity]

    let resExternalId2, resAssetId2, resMetaData3 =
        let h = Seq.tryHead getRes3
        match h with
        | Some sequenceResponse ->
            sequenceResponse.ExternalId, sequenceResponse.AssetId, sequenceResponse.Metadata
        | None -> "", Nullable 0L, Dictionary ()

    let hasValue = resAssetId2.HasValue
    // Assert get2
    test <@ isNull resExternalId2 @>
    test <@ not hasValue @>
    test <@ resMetaData3.Count = 0 @>
}

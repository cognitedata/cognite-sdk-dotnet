module Tests.Integration.DataSets

open System
open System.Collections.Generic

open FSharp.Control.Tasks
open Swensen.Unquote
open Oryx
open Xunit

open CogniteSdk
open Common

[<Fact>]
let ``List data sets with limit is OK`` () = task {
    // Arrange
    let query = DataSetQuery(Limit= Nullable 2)

    // Act
    let! res = readClient.DataSets.ListAsync(query)
    let len = Seq.length res.Items

    // Assert
    test <@ len = 2 @>
}

[<Fact>]
let ``Filter data sets by metadata is OK`` () = task {
    // Arrange
    let meta = Dictionary (dict [("consoleSource", "{\"names\":[\"PI\"]}")])
    let filter = DataSetFilter(Metadata = meta)
    let query = DataSetQuery(Filter = filter)

    // Act
    let! res = readClient.DataSets.ListAsync(query)
    let len = Seq.length res.Items
    let item = Seq.item 0 res.Items

    // Assert
    test <@ len = 1 @>
    test <@ (item.Metadata.Item "consoleSource") = "{\"names\":[\"PI\"]}" @>
    test <@ item.Name = "Valhall system 23 time series" @>
}

let ``Filter data sets by created time is OK`` () = task {
    // Arrange
    let range = TimeRange(Min = 1586596603465L, Max = 1586596605465L)
    let filter = DataSetFilter(CreatedTime = range)
    let query = DataSetQuery(Filter = filter)

    // Act
    let! res = readClient.DataSets.ListAsync(query)
    let len = Seq.length res.Items
    let item = Seq.item 0 res.Items

    // Assert
    test <@ len = 1 @>
    test <@ item.CreatedTime = 1586596604465L @>
    test <@ item.Name = "Valhall system 23 time series" @>
}

let ``Filter data sets by last updated time is OK`` () = task {
    // Arrange
    let range = TimeRange(Min = 1586953700640L, Max = 1586953700840L)
    let filter = DataSetFilter(CreatedTime = range)
    let query = DataSetQuery(Filter = filter)

    // Act
    let! res = readClient.DataSets.ListAsync(query)
    let len = Seq.length res.Items
    let item = Seq.item 0 res.Items

    // Assert
    test <@ len = 1 @>
    test <@ item.LastUpdatedTime = 1586953700740L @>
    test <@ item.Name = "Valhall system 23 time series" @>
}

let ``Filter data sets by externalIdPrefix is OK`` () = task {
    // Arrange
    let filter = DataSetFilter(ExternalIdPrefix = "OID/VAL/")
    let query = DataSetQuery(Filter = filter)

    // Act
    let! res = readClient.DataSets.ListAsync(query)
    let len = Seq.length res.Items

    // Assert
    test <@ len = 2 @>
}

let ``Filter data sets by writeProtected is OK`` () = task {
    // Arrange
    let filter = DataSetFilter(WriteProtected = true)
    let query = DataSetQuery(Filter = filter)

    let! res = readClient.DataSets.ListAsync(query)
    let len = Seq.length res.Items

    // Assert
    test <@ len = 0 @>
}

let ``List data sets without metadata is OK`` () = task {
    // Arrange
    let query = DataSetQuery()

    // Act
    let! res = readClient.DataSets.ListAsync<DataSetWithoutMetadata>(query)
    let len = Seq.length res.Items
    let item = Seq.item 0 res.Items

    // Assert
    test <@ len = 3 @>
    test <@ isNull item.Metadata @>
}

let ``Aggregate data sets with filter is OK`` = task {
    // Arrange
    let filter = DataSetFilter(ExternalIdPrefix = "OID/VAL/")
    let query = DataSetQuery(Filter = filter)

    // Act
    let! res = readClient.DataSets.AggregateAsync(query)
    
    // Assert
    test <@ res = 2 @>
}

let ``Retrieve data sets mixed is OK`` = task {
    // Arrange
    let ids = [ Identity.Create(6973832320392714L); Identity.Create("OID/VAL/Assets") ]

    // Act
    let! res = readClient.DataSets.RetrieveAsync ids
    let len = Seq.length res

    // Assert
    test <@ len = 2 @>
}

let ``Retrieve data sets without metadata is OK`` = task {
    // Arrange
    let ids = [ Identity.Create(6973832320392714L); Identity.Create("OID/VAL/Assets") ]

    // Act
    let! res = readClient.DataSets.RetrieveAsync<DataSetWithoutMetadata> ids
    let len = Seq.length res

    // Assert
    test <@ len = 2 @>
    test <@ Seq.forall (fun (e: DataSetWithoutMetadata) -> isNull e.Metadata) res @>
}

let ``Retrieve data sets by id is OK`` = task {
    // Arrange
    let ids = [ 5272329852941732L; 2452112635370053L ]

    // Act
    let! res = readClient.DataSets.RetrieveAsync ids
    let len = Seq.length res

    // Assert
    test <@ len = 2 @>
}

let ``Retrieve data sets by externalId is OK`` = task {
    // Arrange
    let ids = [ "VAL/FILES/PNIDS"; "OID/VAL/Assets" ]

    // Act
    let! res = readClient.DataSets.RetrieveAsync ids
    let len = Seq.length res

    // Assert
    test <@ len = 2 @>
}
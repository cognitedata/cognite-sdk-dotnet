module Tests.Integration.Timeseries

open System
open System.Net.Http

open FSharp.Control.Tasks.V2.ContextInsensitive
open Oryx
open Swensen.Unquote
open Xunit

open CogniteSdk
open CogniteSdk.TimeSeries

open Tests
open Common
open System.Collections.Generic

[<Fact>]
let ``Get timeseries is Ok`` () = task {
    // Arrange
    let query = TimeSeriesQueryDto(Limit = Nullable 10)

    // Act
    let! response = readClient.TimeSeries.ListAsync query

    let len = Seq.length response.Items

    // Assert
    test <@ len = 10 @>
}

[<Fact>]
let ``Get timeseries by ids is Ok`` () = task {
    // Arrange
    let id = 613312137748079L

    // Act
    let! dtos = readClient.TimeSeries.RetrieveAsync [ id ]

    let len = Seq.length dtos

    let resId =
        let h = Seq.tryHead dtos
        match h with
        | Some dto -> dto.Id
        | None -> 0L

    // Assert
    test <@ resId = id @>
    test <@ len > 0 @>
}

[<Fact>]
let ``Get timeseries by missing id is Error`` () = task {
    // Arrange
    let id = Identity 0L

    // Act
    Assert.ThrowsAsync<ArgumentException>(fun () -> readClient.TimeSeries.RetrieveAsync [ id ] :> _)
    |> ignore
}

[<Fact>]
let ``Create and delete timeseries is Ok`` () = task {
    // Arrange
    let externalIdString = Guid.NewGuid().ToString();
    let dto =
        TimeSeriesWrite(
            ExternalId = externalIdString,
            Name = "Create Timeseries sdk test",
            Description = "dotnet sdk test",
            IsStep = false
        )

    // Act
    let! timeSereiesResponses = writeClient.TimeSeries.CreateAsync [ dto ]
    let! delRes = writeClient.TimeSeries.DeleteAsync [ externalIdString ]

    let resExternalId =
        let h = Seq.tryHead timeSereiesResponses
        match h with
        | Some timeSereiesResponse -> timeSereiesResponse.ExternalId
        | None -> String.Empty

    // Assert
    test <@ resExternalId = externalIdString @>
}

[<Fact>]
let ``Search timeseries is Ok`` () = task {
    // Arrange
    let query =
        TimeSeriesSearch(
            Filter = TimeSeriesFilter(Name = "VAL_23-TT-96136-08:Z.X.Value"),
            Limit = Nullable 10
        )

    // Act
    let! dtos = readClient.TimeSeries.SearchAsync query
    let len = Seq.length dtos

    // Assert
    test <@ len = 1 @>
}

[<Fact>]
let ``Search timeseries on CreatedTime Ok`` () = task {
    // Arrange
    let timerange =
        TimeRange(
            Min = Nullable 1567707299032L,
            Max = Nullable 1567707299052L
        )
    let query =
        TimeSeriesSearch(
            Filter = TimeSeriesFilter(CreatedTime = timerange),
            Limit = Nullable 10
        )

    // Act
    let! dtos = writeClient.TimeSeries.SearchAsync query

    let len = Seq.length dtos

    let createdTime = (Seq.head dtos).CreatedTime

    // Assert
    test <@ len = 2 @>
    test <@ createdTime = 1567707299042L @>
}

[<Fact>]
let ``Search timeseries on LastUpdatedTime Ok`` () = task {
    // Arrange
    let timerange =
        TimeRange(
            Min = Nullable 1567707299032L,
            Max = Nullable 1567707299052L
        )

    let query =
        TimeSeriesSearch(
            Filter = TimeSeriesFilter(LastUpdatedTime = timerange),
            Limit = Nullable 10
        )

    // Act
    let! dtos = writeClient.TimeSeries.SearchAsync query
    let len = Seq.length dtos

    let lastUpdatedTime = (Seq.head dtos).LastUpdatedTime

    // Assert
    test <@ len = 2 @>
    test <@ lastUpdatedTime = 1567707299042L @>
}

[<Fact>]
let ``Search timeseries on AssetIds is Ok`` () = task {
    // Arrange
    let query =
        TimeSeriesSearch(
            Filter = TimeSeriesFilter(AssetIds = [ 4293345866058133L ]),
            Limit = Nullable 10
        )

    // Act
    let! dtos = readClient.TimeSeries.SearchAsync query

    let len = Seq.length dtos

    let assetIds = Seq.map (fun (d : TimeSeriesRead) -> d.AssetId.Value) dtos

    // Assert
    test <@ len = 1 @>
    test <@ Seq.forall ((=) (4293345866058133L)) assetIds @>
}

[<Fact>]
let ``Search timeseries on ExternalIdPrefix is Ok`` () = task {
    // Arrange
    let query =
        TimeSeriesSearch(
            Filter = TimeSeriesFilter(ExternalIdPrefix = "VAL_45"),
            Limit = Nullable 10
        )

    // Act
    let! dtos = readClient.TimeSeries.SearchAsync query
    let len = Seq.length dtos

    let externalIds = Seq.map (fun (d : TimeSeriesRead) -> d.ExternalId) dtos

    // Assert
    test <@ len = 10 @>
    test <@ Seq.forall (fun (e: string) -> e.StartsWith("VAL_45")) externalIds @>
}

[<Fact>]
let ``Search timeseries on IsStep is Ok`` () = task {
    // Arrange
    let query =
        TimeSeriesSearch(
            Filter = TimeSeriesFilter(IsStep = Nullable true),
            Limit = Nullable 10
        )

    // Act
    let! dtos = readClient.TimeSeries.SearchAsync query

    let len = Seq.length dtos

    let isSteps = Seq.map (fun (d : TimeSeriesRead) -> d.IsStep) dtos

    // Assert
    test <@ len = 5 @>
    test <@ Seq.forall id isSteps @>
}

[<Fact>]
let ``Search timeseries on IsString is Ok`` () = task {
    // Arrange
    let query =
        TimeSeriesSearch(
            Filter = TimeSeriesFilter(IsString = Nullable true),
            Limit = Nullable 10
        )

    // Act
    let! dtos = readClient.TimeSeries.SearchAsync query

    let len = Seq.length dtos
    let isStrings = Seq.map (fun (d: TimeSeriesRead) -> d.IsString) dtos

    // Assert
    test <@ len = 6 @>
    test <@ Seq.forall id isStrings @>
}

[<Fact>]
let ``Search timeseries on Unit is Ok`` () = task {
    // Arrange
    let query =
        TimeSeriesSearch(
            Filter = TimeSeriesFilter(Unit = "et"),
            Limit = Nullable 10
        )
    // Act
    let! dtos = writeClient.TimeSeries.SearchAsync query

    let len = Seq.length dtos

    let units = Seq.map (fun (d: TimeSeriesRead) -> d.Unit) dtos

    // Assert
    test <@ len = 2 @>
    test <@ Seq.forall ((=) "et") units @>
}

[<Fact>]
let ``Search timeseries on MetaData is Ok`` () = task {
    // Arrange
    let query =
        TimeSeriesSearch(
            Filter = TimeSeriesFilter(Metadata = (dict ["pointid", "160909"] |> Dictionary)),
            Limit = Nullable 10
        )
    // Act
    let! dtos = readClient.TimeSeries.SearchAsync query

    let len = Seq.length dtos
    let ms = Seq.map (fun (d: TimeSeriesRead)  -> d.Metadata) dtos

    // Assert
    test <@ len = 1 @>
    test <@ Seq.forall (fun (m : Dictionary<string,string>) -> m.["pointid"] = "160909") ms @>
}

[<Fact>]
let ``FuzzySearch timeseries on Name is Ok`` () = task {
    // Arrange
    let query =
        TimeSeriesSearch(
            Search = Search(Name = "92529_SILch0"),
            Limit = Nullable 10
        )
    // Act
    let! dtos = readClient.TimeSeries.SearchAsync query

    let len = Seq.length dtos

    let names = Seq.map (fun (d: TimeSeriesRead) -> d.Name) dtos

    // Assert
    test <@ len = 9 @>
    test <@ Seq.forall (fun (n: string) -> n.Contains("SILch0") || n.Contains("92529")) names @>
}

[<Fact>]
let ``FuzzySearch timeseries on Description is Ok`` () = task {
    // Arrange
    let query =
        TimeSeriesSearch(
            Search = Search(Description = "Tube y"),
            Limit = Nullable 10
        )

    // Act
    let! dtos = readClient.TimeSeries.SearchAsync query

    let len = Seq.length dtos

    let descriptions = Seq.map (fun (d: TimeSeriesRead) -> d.Description) dtos

    // Assert
    test <@ len = 10 @>
    test <@ Seq.forall (fun (n: string) -> n.Contains("Tube")) descriptions @>
}

[<Fact>]
let ``Update timeseries is Ok`` () = task {
    // Arrange

    let newMetadata =
        dict [
            "key1", "value1"
            "key2", "value2"
        ] |> Dictionary
    let dto =
        TimeSeriesWrite(
            Metadata = (dict [
                "oldkey1", "oldvalue1"
                "oldkey2", "oldvalue2"
            ] |> Dictionary),
            ExternalId = "testupdate",
            Name = "testupdate",
            IsString = false,
            IsStep = false
        )
    let externalId = dto.ExternalId
    let newExternalId = "testupdatenew"
    let newDescription = "testdescription"

    // Act
    let! createRes = writeClient.TimeSeries.CreateAsync [ dto ]
    let! updateRes =
        writeClient.TimeSeries.UpdateAsync [
            TimeSeriesUpdateItem(
                externalId = externalId,
                Update = TimeSeriesUpdate(
                    ExternalId = Update(newExternalId),
                    Metadata = DictUpdate(newMetadata, [ "oldkey1" ]),
                    Description = Update(newDescription),
                    Name = Update(null),
                    Unit = Update("unit")
                )
            )
        ]
    let! getRes = writeClient.TimeSeries.RetrieveAsync [ newExternalId ]
    let! deleteRes = writeClient.TimeSeries.DeleteAsync [ newExternalId ]

    let resExternalId, resMetaData, resDescription =
        let head = Seq.tryHead getRes
        match head with
        | Some tsresp -> tsresp.ExternalId, tsresp.Metadata, tsresp.Description
        | None -> "", Dictionary (), ""

    let metaDataOk =
        resMetaData.ContainsKey "key1"
        && resMetaData.ContainsKey "key2"
        && resMetaData.ContainsKey "oldkey2"
        && not (resMetaData.ContainsKey "oldkey1")

    // Assert get
    test <@ resExternalId = newExternalId @>
    test <@ resDescription = newDescription @>
    test <@ metaDataOk @>

    // Assert delete
}

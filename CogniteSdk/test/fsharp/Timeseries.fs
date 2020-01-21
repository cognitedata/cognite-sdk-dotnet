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

[<Fact>]
let ``Get timeseries is Ok`` () = task {
    // Arrange
    let query = TimeSeriesQuery(Limit = Nullable 10)

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
        TimeSeriesWriteDto(
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
        TimeSeries.TimeSeriesSearch(
            Filter = TimeSeriesFilterDto(Name = "VAL_23-TT-96136-08:Z.X.Value"),
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
            Min = 1567707299032L,
            Max = 1567707299052L
        )
    let query =
        TimeSeriesSearch(
            Filter = TimeSeriesFilterDto(CreatedTime = timerange),
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
            Min = 1567707299032L,
            Max = 1567707299052L
        )

    let query =
        TimeSeriesSearch(
            Filter = TimeSeriesFilterDto(LastUpdatedTime = timerange),
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
            Filter = TimeSeriesFilterDto(AssetIds = [ 4293345866058133L ]),
            Limit = Nullable 10
        )

    // Act
    let! dtos = readClient.TimeSeries.SearchAsync query

    let len = Seq.length dtos

    let assetIds = Seq.map (fun (d : TimeSeriesReadDto) -> d.AssetId.Value) dtos

    // Assert
    test <@ len = 1 @>
    test <@ Seq.forall ((=) (4293345866058133L)) assetIds @>
}

[<Fact>]
let ``Search timeseries on ExternalIdPrefix is Ok`` () = task {
    // Arrange
    let query =
        TimeSeriesSearch(
            Filter = TimeSeriesFilterDto(ExternalIdPrefix = "VAL_45"),
            Limit = Nullable 10
        )

    // Act
    let! dtos = readClient.TimeSeries.SearchAsync query
    let len = Seq.length dtos

    let externalIds = Seq.map (fun (d : TimeSeriesReadDto) -> d.ExternalId) dtos

    // Assert
    test <@ len = 10 @>
    test <@ Seq.forall (fun (e: string) -> e.StartsWith("VAL_45")) externalIds @>
}

[<Fact>]
let ``Search timeseries on IsStep is Ok`` () = task {
    // Arrange
    let query =
        TimeSeriesSearch(
            Filter = TimeSeriesFilterDto(IsStep = Nullable true),
            Limit = Nullable 10
        )

    // Act
    let! dtos = readClient.TimeSeries.SearchAsync query

    let len = Seq.length dtos

    let isSteps = Seq.map (fun (d : TimeSeriesReadDto) -> d.IsStep.Value) dtos

    // Assert
    test <@ len = 5 @>
    test <@ Seq.forall id isSteps @>
}

[<Fact>]
let ``Search timeseries on IsString is Ok`` () = task {
    // Arrange
    let query =
        TimeSeriesSearch(
            Filter = TimeSeriesFilterDto(IsString = Nullable true),
            Limit = Nullable 10
        )

    // Act
    let! dtos = readClient.TimeSeries.SearchAsync query

    let len = Seq.length dtos
    let isStrings = Seq.map (fun (d: TimeSeriesReadDto) -> d.IsString.Value) dtos

    // Assert
    test <@ len = 6 @>
    test <@ Seq.forall id isStrings @>
}
(*

[<Fact>]
let ``Search timeseries on Unit is Ok`` () = task {
    // Arrange
    let ctx = writeCtx ()

    // Act
    let! res =
        TimeSeries.Search.searchAsync 10 [] [ TimeSeriesFilter.Unit "et" ] ctx

    let ctx' =
        match res with
        | Ok ctx -> ctx
        | Error (ResponseError error) -> raise <| error.ToException ()
        | Error (Panic error) -> raise error

    let dtos = ctx'.Response
    let len = Seq.length dtos

    let units = Seq.collect (fun d -> d.Unit |> optionToSeq) dtos

    // Assert
    test <@ len = 1 @>
    test <@ Seq.forall ((=) "et") units @>
    test <@ ctx'.Request.Method = HttpMethod.Post @>
    test <@ ctx'.Request.Extra.["resource"] = "/timeseries/search" @>
}

[<Fact>]
let ``Search timeseries on MetaData is Ok`` () = task {
    // Arrange
    let ctx = readCtx ()

    // Act
    let! res =
        TimeSeries.Search.searchAsync 10 [] [ TimeSeriesFilter.MetaData (Map.ofList ["pointid", "160909"]) ] ctx

    let ctx' =
        match res with
        | Ok ctx -> ctx
        | Error (ResponseError error) -> raise <| error.ToException ()
        | Error (Panic error) -> raise error

    let dtos = ctx'.Response
    let len = Seq.length dtos

    let ms = Seq.map (fun d -> d.MetaData) dtos

    // Assert
    test <@ len = 1 @>
    test <@ Seq.forall (fun m -> (Map.tryFind "pointid" m) = Some "160909") ms @>
    test <@ ctx'.Request.Method = HttpMethod.Post @>
    test <@ ctx'.Request.Extra.["resource"] = "/timeseries/search" @>
}

[<Fact>]
let ``FuzzySearch timeseries on Name is Ok`` () = task {
    // Arrange
    let ctx = readCtx ()

    // Act
    let! res =
        TimeSeries.Search.searchAsync 10 [ TimeSeriesSearch.Name "92529_SILch0" ] [ ] ctx

    let ctx' =
        match res with
        | Ok ctx -> ctx
        | Error (ResponseError error) -> raise <| error.ToException ()
        | Error (Panic error) -> raise error

    let dtos = ctx'.Response
    let len = Seq.length dtos

    let names = Seq.collect (fun d -> d.Name |> optionToSeq) dtos

    // Assert
    test <@ len = 9 @>
    test <@ Seq.forall (fun (n: string) -> n.Contains("SILch0") || n.Contains("92529")) names @>
    test <@ ctx'.Request.Method = HttpMethod.Post @>
    test <@ ctx'.Request.Extra.["resource"] = "/timeseries/search" @>
}

[<Fact>]
let ``FuzzySearch timeseries on Description is Ok`` () = task {
    // Arrange
    let ctx = readCtx ()

    // Act
    let! res =
        TimeSeries.Search.searchAsync 10 [ TimeSeriesSearch.Description "Tube y" ] [ ] ctx

    let ctx' =
        match res with
        | Ok ctx -> ctx
        | Error (ResponseError error) -> raise <| error.ToException ()
        | Error (Panic error) -> raise error

    let dtos = ctx'.Response
    let len = Seq.length dtos

    let descriptions = Seq.collect (fun d -> d.Description |> optionToSeq) dtos

    // Assert
    test <@ len = 10 @>
    test <@ Seq.forall (fun (n: string) -> n.Contains("Tube")) descriptions @>
    test <@ ctx'.Request.Method = HttpMethod.Post @>
    test <@ ctx'.Request.Extra.["resource"] = "/timeseries/search" @>
}

[<Fact>]
let ``Update timeseries is Ok`` () = task {
    // Arrange
    let wctx = writeCtx ()

    let newMetadata = ([
        "key1", "value1"
        "key2", "value2"
    ]
    |> Map.ofList)
    let dto : TimeSeries.TimeSeriesWriteDto = {
        Metadata = [
            "oldkey1", "oldvalue1"
            "oldkey2", "oldvalue2"
        ] |> Map.ofList
        ExternalId = Some "testupdate"
        Name = Some "testupdate"
        LegacyName = None
        Description = None
        IsString = false
        IsStep = false
        Unit = None
        AssetId = None
        SecurityCategories = Seq.empty
    }
    let externalId = Identity.ExternalId dto.ExternalId.Value
    let newExternalId = "testupdatenew"
    let newDescription = "testdescription"

    // Act
    let! createRes = TimeSeries.Create.createAsync [ dto ] wctx
    let! updateRes =
        TimeSeries.Update.updateAsync [
            (externalId, [
                TimeSeriesUpdate.SetExternalId (Some newExternalId)
                TimeSeriesUpdate.ChangeMetaData (newMetadata, [ "oldkey1" ] |> Seq.ofList)
                TimeSeriesUpdate.SetDescription (Some newDescription)
                TimeSeriesUpdate.SetName None
                TimeSeriesUpdate.SetUnit (Some "unit")
            ])
        ] wctx
    let! getRes = TimeSeries.Retrieve.getByIdsAsync [ Identity.ExternalId newExternalId ] wctx
    let! deleteRes = TimeSeries.Delete.deleteAsync [ Identity.ExternalId newExternalId ] wctx

    let getCtx' =
        match getRes with
        | Ok ctx -> ctx
        | Error (ResponseError error) -> raise <| error.ToException ()
        | Error (Panic error) -> raise error

    let resExternalId, resMetaData, resDescription =
        let head = Seq.tryHead getCtx'.Response
        match head with
        | Some tsresp -> tsresp.ExternalId, tsresp.MetaData, tsresp.Description
        | None -> Some "", Map.empty, Some ""

    let metaDataOk =
        resMetaData.ContainsKey "key1"
        && resMetaData.ContainsKey "key2"
        && resMetaData.ContainsKey "oldkey2"
        && not (resMetaData.ContainsKey "oldkey1")

    let createCtx' =
        match createRes with
        | Ok ctx -> ctx
        | Error (ResponseError error) -> raise <| error.ToException ()
        | Error (Panic error) -> raise error

    let updateCtx' =
        match updateRes with
        | Ok ctx -> ctx
        | Error (ResponseError error) -> raise <| error.ToException ()
        | Error (Panic error) -> raise error

    let deleteCtx' =
        match deleteRes with
        | Ok ctx -> ctx
        | Error (ResponseError error) -> raise <| error.ToException ()
        | Error (Panic error) -> raise error

    // Assert create
    test <@ createCtx'.Request.Method = HttpMethod.Post @>
    test <@ createCtx'.Request.Extra.["resource"] = "/timeseries" @>
    test <@ createCtx'.Request.Query.IsEmpty @>

    // Assert update
    test <@ updateCtx'.Request.Method = HttpMethod.Post @>
    test <@ updateCtx'.Request.Extra.["resource"] = "/timeseries/update" @>
    test <@ updateCtx'.Request.Query.IsEmpty @>

    // Assert get
    test <@ getCtx'.Request.Method = HttpMethod.Post @>
    test <@ getCtx'.Request.Extra.["resource"] = "/timeseries/byids" @>
    test <@ getCtx'.Request.Query.IsEmpty @>
    test <@ resExternalId = Some newExternalId @>
    test <@ resDescription = Some newDescription @>
    test <@ metaDataOk @>

    // Assert delete
    test <@ deleteCtx'.Request.Method = HttpMethod.Post @>
    test <@ deleteCtx'.Request.Extra.["resource"] = "/timeseries/delete" @>
    test <@ deleteCtx'.Request.Query.IsEmpty @>

}
*)
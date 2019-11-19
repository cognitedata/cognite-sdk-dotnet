module Tests.Integration.Timeseries

open System
open System.Net.Http

open Swensen.Unquote
open Xunit

open Oryx
open CogniteSdk
open CogniteSdk.TimeSeries
open Tests
open Common
open FSharp.Control.Tasks.V2.ContextInsensitive

[<Fact>]
let ``Get timeseries is Ok`` () = task {
    // Arrange
    let ctx = readCtx ()
    let options = [ TimeSeriesQuery.Limit 10 ]

    // Act
    let! res = TimeSeries.Items.listAsync options ctx

    let ctx' =
        match res with
        | Ok ctx -> ctx
        | Error (ResponseError error) -> raise <| error.ToException ()
        | Error (Panic error) -> raise error

    let dtos = ctx'.Response
    let len = Seq.length dtos.Items

    // Assert
    test <@ len = 10 @>
    test <@ ctx'.Request.Method = HttpMethod.Get @>
    test <@ ctx'.Request.Extra.["resource"] = "/timeseries" @>
}

[<Fact>]
let ``Get timeseries by ids is Ok`` () = task {
    // Arrange
    let ctx = readCtx ()
    let id = Identity.Id 613312137748079L

    // Act
    let! res = TimeSeries.Retrieve.getByIdsAsync [ id ] ctx

    let ctx' =
        match res with
        | Ok ctx -> ctx
        | Error (ResponseError error) -> raise <| error.ToException ()
        | Error (Panic error) -> raise error

    let dtos = ctx'.Response
    let len = Seq.length dtos

    let resId =
        let h = Seq.tryHead dtos
        match h with
        | Some dto -> Identity.Id dto.Id
        | None -> Identity.Id 0L

    // Assert
    test <@ resId = id @>
    test <@ len > 0 @>
    test <@ ctx'.Request.Method = HttpMethod.Post @>
    test <@ ctx'.Request.Extra.["resource"] = "/timeseries/byids" @>
    test <@ ctx'.Request.Query.IsEmpty @>
}

[<Fact>]
let ``Get timeseries by missing id is Error`` () = task {
    // Arrange
    let ctx = readCtx ()
    let id = Identity.Id 0L

    // Act
    let! res = TimeSeries.Retrieve.getByIdsAsync [ id ] ctx

    let err =
        match res with
        | Ok _ -> ResponseError.empty
        | Error (ResponseError err) -> err
        | Error (Panic err) -> raise err

    // Assert
    test <@ Result.isError res @>
    test <@ err.Code = 400 @>
    test <@ err.Message = "timeseries ids not found: (id: 0 | externalId: null)" @>
}

[<Fact>]
let ``Create and delete timeseries is Ok`` () = task {
    // Arrange
    let ctx = writeCtx ()
    let externalIdString = Guid.NewGuid().ToString();
    let dto: TimeSeries.TimeSeriesWriteDto = {
        ExternalId = Some externalIdString
        Name = Some "Create Timeseries sdk test"
        LegacyName = None
        Description = Some "dotnet sdk test"
        IsString = false
        MetaData = Map.empty
        Unit = None
        AssetId = None
        IsStep = false
        SecurityCategories = Seq.empty
    }
    let externalId = Identity.ExternalId externalIdString

    // Act
    let! res = TimeSeries.Create.createAsync [ dto ] ctx
    let! delRes = TimeSeries.Delete.deleteAsync [ externalId ] ctx

    let ctx' =
        match res with
        | Ok ctx -> ctx
        | Error (ResponseError error) -> raise <| error.ToException ()
        | Error (Panic error) -> raise error

    let resExternalId =
        let timeSereiesResponses = ctx'.Response
        let h = Seq.tryHead timeSereiesResponses.Items
        match h with
        | Some timeSereiesResponse -> timeSereiesResponse.ExternalId
        | None -> None

    let delCtx' =
        match delRes with
        | Ok ctx -> ctx
        | Error (ResponseError error) -> raise <| error.ToException ()
        | Error (Panic error) -> raise error

    let dtos = ctx'.Response

    // Assert
    test <@ resExternalId = Some externalIdString @>
    test <@ ctx'.Request.Method = HttpMethod.Post @>
    test <@ ctx'.Request.Extra.["resource"] = "/timeseries" @>
    test <@ ctx'.Request.Query.IsEmpty @>

    test <@ delCtx'.Request.Method = HttpMethod.Post @>
    test <@ delCtx'.Request.Extra.["resource"] = "/timeseries/delete" @>
    test <@ delCtx'.Request.Query.IsEmpty @>
}

[<Fact>]
let ``Search timeseries is Ok`` () = task {
    // Arrange
    let ctx = readCtx ()

    // Act
    let! res =
        TimeSeries.Search.searchAsync 10 [] [ TimeSeriesFilter.Name "VAL_23-TT-96136-08:Z.X.Value" ] ctx

    let ctx' =
        match res with
        | Ok ctx -> ctx
        | Error (ResponseError error) -> raise <| error.ToException ()
        | Error (Panic error) -> raise error

    let dtos = ctx'.Response
    let len = Seq.length dtos

    // Assert
    test <@ len = 1 @>
    test <@ ctx'.Request.Method = HttpMethod.Post @>
    test <@ ctx'.Request.Extra.["resource"] = "/timeseries/search" @>
}

[<Fact>]
let ``Search timeseries on CreatedTime Ok`` () = task {
    // Arrange
    let ctx = writeCtx ()
    let timerange = {
        Min = DateTimeOffset.FromUnixTimeMilliseconds(1567707299032L)
        Max = DateTimeOffset.FromUnixTimeMilliseconds(1567707299052L)
    }

    // Act
    let! res =
        TimeSeries.Search.searchAsync 10 [] [ TimeSeriesFilter.CreatedTime timerange ] ctx

    let ctx' =
        match res with
        | Ok ctx -> ctx
        | Error (ResponseError error) -> raise <| error.ToException ()
        | Error (Panic error) -> raise error

    let dtos = ctx'.Response
    let len = Seq.length dtos

    let createdTime = (Seq.head dtos).CreatedTime

    // Assert
    test <@ len = 2 @>
    test <@ createdTime = 1567707299042L @>
    test <@ ctx'.Request.Method = HttpMethod.Post @>
    test <@ ctx'.Request.Extra.["resource"] = "/timeseries/search" @>
}

[<Fact>]
let ``Search timeseries on LastUpdatedTime Ok`` () = task {

    // Arrange
    let ctx = writeCtx ()
    let timerange = {
        Min = DateTimeOffset.FromUnixTimeMilliseconds(1567707299032L)
        Max = DateTimeOffset.FromUnixTimeMilliseconds(1567707299052L)
    }
    // Act
    let! res =
        TimeSeries.Search.searchAsync 10 [] [ TimeSeriesFilter.LastUpdatedTime timerange] ctx

    let ctx' =
        match res with
        | Ok ctx -> ctx
        | Error (ResponseError error) -> raise <| error.ToException ()
        | Error (Panic error) -> raise error

    let dtos = ctx'.Response
    let len = Seq.length dtos

    let lastUpdatedTime = (Seq.head dtos).LastUpdatedTime

    // Assert
    test <@ len = 2 @>
    test <@ lastUpdatedTime = 1567707299042L @>
    test <@ ctx'.Request.Method = HttpMethod.Post @>
    test <@ ctx'.Request.Extra.["resource"] = "/timeseries/search" @>
}

[<Fact>]
let ``Search timeseries on AssetIds is Ok`` () = task {
    // Arrange
    let ctx = readCtx ()

    // Act
    let! res =
        TimeSeries.Search.searchAsync 10 [] [ TimeSeriesFilter.AssetIds [4293345866058133L] ] ctx

    let ctx' =
        match res with
        | Ok ctx -> ctx
        | Error (ResponseError error) -> raise <| error.ToException ()
        | Error (Panic error) -> raise error

    let dtos = ctx'.Response
    let len = Seq.length dtos

    let assetIds = Seq.map (fun d -> d.AssetId) dtos

    // Assert
    test <@ len = 1 @>
    test <@ Seq.forall ((=) (Some 4293345866058133L)) assetIds @>
    test <@ ctx'.Request.Method = HttpMethod.Post @>
    test <@ ctx'.Request.Extra.["resource"] = "/timeseries/search" @>
}

[<Fact>]
let ``Search timeseries on ExternalIdPrefix is Ok`` () = task {
    // Arrange
    let ctx = readCtx ()

    // Act
    let! res =
        TimeSeries.Search.searchAsync 10 [] [ TimeSeriesFilter.ExternalIdPrefix "VAL_45" ] ctx

    let ctx' =
        match res with
        | Ok ctx -> ctx
        | Error (ResponseError error) -> raise <| error.ToException ()
        | Error (Panic error) -> raise error

    let dtos = ctx'.Response
    let len = Seq.length dtos

    let externalIds = Seq.collect (fun d -> d.ExternalId |> optionToSeq) dtos

    // Assert
    test <@ len = 10 @>
    test <@ Seq.forall (fun (e: string) -> e.StartsWith("VAL_45")) externalIds @>
    test <@ ctx'.Request.Method = HttpMethod.Post @>
    test <@ ctx'.Request.Extra.["resource"] = "/timeseries/search" @>
}

[<Fact>]
let ``Search timeseries on IsStep is Ok`` () = task {
    // Arrange
    let ctx = readCtx ()

    // Act
    let! res =
        TimeSeries.Search.searchAsync 10 [] [ TimeSeriesFilter.IsStep true ] ctx

    let ctx' =
        match res with
        | Ok ctx -> ctx
        | Error (ResponseError error) -> raise <| error.ToException ()
        | Error (Panic error) -> raise error

    let dtos = ctx'.Response
    let len = Seq.length dtos

    let isSteps = Seq.map (fun d -> d.IsStep) dtos

    // Assert
    test <@ len = 5 @>
    test <@ Seq.forall id isSteps @>
    test <@ ctx'.Request.Method = HttpMethod.Post @>
    test <@ ctx'.Request.Extra.["resource"] = "/timeseries/search" @>
}

[<Fact>]
let ``Search timeseries on IsString is Ok`` () = task {
    // Arrange
    let ctx = readCtx ()

    // Act
    let! res =
        TimeSeries.Search.searchAsync 10 [] [ TimeSeriesFilter.IsString true ] ctx

    let ctx' =
        match res with
        | Ok ctx -> ctx
        | Error (ResponseError error) -> raise <| error.ToException ()
        | Error (Panic error) -> raise error

    let dtos = ctx'.Response
    let len = Seq.length dtos

    let isStrings = Seq.map (fun d -> d.IsString) dtos

    // Assert
    test <@ len = 6 @>
    test <@ Seq.forall id isStrings @>
    test <@ ctx'.Request.Method = HttpMethod.Post @>
    test <@ ctx'.Request.Extra.["resource"] = "/timeseries/search" @>
}

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
        MetaData = [
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

module Tests.Integration.Timeseries

open System.Net.Http
open Swensen.Unquote
open Xunit

open Oryx
open CogniteSdk
open CogniteSdk.TimeSeries
open Tests
open Common

[<Fact>]
let ``Get timeseries is Ok`` () = async {
    // Arrange
    let ctx = readCtx ()
    let options = [ TimeSeriesQuery.Limit 10 ]

    // Act
    let! res = TimeSeries.Items.listAsync options ctx

    let len =
        match res.Result with
        | Ok dtos -> Seq.length dtos.Items
        | Error _ -> 0

    // Assert
    test <@ Result.isOk res.Result @>
    test <@ len = 10 @>
    test <@ res.Request.Method = HttpMethod.Get @>
    test <@ res.Request.Extra.["resource"] = "/timeseries" @>
}

[<Fact>]
let ``Get timeseries by ids is Ok`` () = async {
    // Arrange
    let ctx = readCtx ()
    let id = Identity.Id 613312137748079L

    // Act
    let! res = TimeSeries.Retrieve.getByIdsAsync [ id ] ctx

    let resId =
        match res.Result with
        | Ok dtos ->
            let h = Seq.tryHead dtos
            match h with
            | Some dto -> dto.Id
            | None -> 0L
        | Error _ -> 0L
        |> Identity.Id

    let len =
        match res.Result with
        | Ok dtos -> Seq.length dtos
        | Error _ -> 0

    // Assert
    test <@ Result.isOk res.Result @>
    test <@ resId = id @>
    test <@ len > 0 @>
    test <@ res.Request.Method = HttpMethod.Post @>
    test <@ res.Request.Extra.["resource"] = "/timeseries/byids" @>
    test <@ res.Request.Query.IsEmpty @>
}

[<Fact>]
let ``Get timeseries by missing id is Error`` () = async {
    // Arrange
    let ctx = readCtx ()
    let id = Identity.Id 0L

    // Act
    let! res = TimeSeries.Retrieve.getByIdsAsync [ id ] ctx

    let err =
        match res.Result with
        | Ok _ -> ResponseError.empty
        | Error err -> err

    // Assert
    test <@ Result.isError res.Result @>
    test <@ err.Code = 400 @>
    test <@ err.Message = "timeseries ids not found: (id: 0 | externalId: null)" @>
}

[<Fact>]
let ``Create and delete timeseries is Ok`` () = async {
    // Arrange
    let ctx = writeCtx ()
    let externalIdString = "createDeleteTest"
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
    let resExternalId =
        match res.Result with
        | Ok timeSereiesResponses ->
            let h = Seq.tryHead timeSereiesResponses.Items
            match h with
            | Some timeSereiesResponse -> timeSereiesResponse.ExternalId
            | None -> None
        | Error _ -> None

    // Assert
    test <@ Result.isOk res.Result @>
    test <@ resExternalId = Some externalIdString @>
    test <@ res.Request.Method = HttpMethod.Post @>
    test <@ res.Request.Extra.["resource"] = "/timeseries" @>
    test <@ res.Request.Query.IsEmpty @>

    test <@ Result.isOk delRes.Result @>
    test <@ delRes.Request.Method = HttpMethod.Post @>
    test <@ delRes.Request.Extra.["resource"] = "/timeseries/delete" @>
    test <@ delRes.Request.Query.IsEmpty @>
}

[<Fact>]
let ``Search timeseries is Ok`` () = async {
    // Arrange
    let ctx = readCtx ()

    // Act
    let! res =
        TimeSeries.Search.searchAsync 10 [] [ TimeSeriesFilter.Name "VAL_23-TT-96136-08:Z.X.Value" ] ctx

    let len =
        match res.Result with
        | Ok dtos -> Seq.length dtos
        | Error _ -> 0

    // Assert
    test <@ Result.isOk res.Result @>
    test <@ len = 1 @>
    test <@ res.Request.Method = HttpMethod.Post @>
    test <@ res.Request.Extra.["resource"] = "/timeseries/search" @>
}

[<Fact>]
let ``Update timeseries is Ok`` () = async {
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
    let resExternalId, resMetaData, resDescription =
        match getRes.Result with
        | Ok resp ->
            let head = Seq.tryHead resp
            match head with
            | Some tsresp -> tsresp.ExternalId, tsresp.MetaData, tsresp.Description
            | None -> Some "", Map.empty, Some ""
        | Error _ -> Some "", Map.empty, Some ""

    let metaDataOk =
        resMetaData.ContainsKey "key1"
        && resMetaData.ContainsKey "key2"
        && resMetaData.ContainsKey "oldkey2"
        && not (resMetaData.ContainsKey "oldkey1")

    // Assert create
    test <@ Result.isOk createRes.Result @>
    test <@ createRes.Request.Method = HttpMethod.Post @>
    test <@ createRes.Request.Extra.["resource"] = "/timeseries" @>
    test <@ createRes.Request.Query.IsEmpty @>

    // Assert update
    test <@ Result.isOk updateRes.Result @>
    test <@ updateRes.Request.Method = HttpMethod.Post @>
    test <@ updateRes.Request.Extra.["resource"] = "/timeseries/update" @>
    test <@ updateRes.Request.Query.IsEmpty @>

    // Assert get
    test <@ Result.isOk getRes.Result @>
    test <@ getRes.Request.Method = HttpMethod.Post @>
    test <@ getRes.Request.Extra.["resource"] = "/timeseries/byids" @>
    test <@ getRes.Request.Query.IsEmpty @>
    test <@ resExternalId = Some newExternalId @>
    test <@ resDescription = Some newDescription @>
    test <@ metaDataOk @>

    // Assert delete
    test <@ Result.isOk deleteRes.Result @>
    test <@ deleteRes.Request.Method = HttpMethod.Post @>
    test <@ deleteRes.Request.Extra.["resource"] = "/timeseries/delete" @>
    test <@ deleteRes.Request.Query.IsEmpty @>

}

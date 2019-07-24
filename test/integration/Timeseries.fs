module Tests.Integration.Timeseries

open Xunit
open Swensen.Unquote

open Fusion
open Fusion.Timeseries
open Tests
open Common

[<Fact>]
let ``Get timeseries is Ok`` () = async {
    // Arrange
    let ctx = readCtx ()
    let options = [ GetTimeseries.Option.Limit 10 ]

    // Act
    let! res = getTimeseriesAsync options ctx

    let len =
        match res.Result with
        | Ok dtos -> Seq.length dtos.Items
        | Error _ -> 0

    // Assert
    test <@ Result.isOk res.Result @>
    test <@ len = 10 @>
    test <@ res.Request.Method = RequestMethod.GET @>
    test <@ res.Request.Resource = "/timeseries" @>
}

[<Fact>]
let ``Get timeseries by ids is Ok`` () = async {
    // Arrange
    let ctx = readCtx ()
    let id = Identity.Id 613312137748079L

    // Act
    let! res = getTimeseriesByIdsAsync [ id ] ctx

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
    test <@ res.Request.Method = RequestMethod.POST @>
    test <@ res.Request.Resource = "/timeseries/byids" @>
    test <@ res.Request.Query.IsEmpty @>
}

[<Fact>]
let ``Get timeseries by missing id is Error`` () = async {
    // Arrange
    let ctx = readCtx ()
    let id = Identity.Id 0L

    // Act
    let! res = getTimeseriesByIdsAsync [ id ] ctx

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
    let dto: TimeseriesWriteDto = {
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
    let! res = createTimeseriesAsync [ dto ] ctx
    let! delRes = deleteTimeseriesAsync [ externalId ] ctx
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
    test <@ res.Request.Method = RequestMethod.POST @>
    test <@ res.Request.Resource = "/timeseries" @>
    test <@ res.Request.Query.IsEmpty @>

    test <@ Result.isOk delRes.Result @>
    test <@ delRes.Request.Method = RequestMethod.POST @>
    test <@ delRes.Request.Resource = "/timeseries/delete" @>
    test <@ delRes.Request.Query.IsEmpty @>
}

[<Fact>]
let ``Search timeseries is Ok`` () = async {
    // Arrange
    let ctx = readCtx ()

    // Act
    let! res =
        searchTimeseriesAsync 10 [] [ SearchTimeseries.Filter.Name "VAL_23-TT-96136-08:Z.X.Value" ] ctx

    let len =
        match res.Result with
        | Ok dtos -> Seq.length dtos
        | Error _ -> 0

    // Assert
    test <@ Result.isOk res.Result @>
    test <@ len = 1 @>
    test <@ res.Request.Method = RequestMethod.POST @>
    test <@ res.Request.Resource = "/timeseries/search" @>
}

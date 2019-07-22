module Tests.Integration.Timeseries

open System
open System.Net.Http

open Xunit
open Swensen.Unquote

open Fusion
open Fusion.Timeseries
open Tests

let testApiKeyWrite = Environment.GetEnvironmentVariable "TEST_API_KEY_WRITE"
let testApiKeyRead = Environment.GetEnvironmentVariable "TEST_API_KEY_READ"
let testApiProjectRead = Environment.GetEnvironmentVariable "TEST_API_PROJECT_READ"
let testApiProjectWrite = Environment.GetEnvironmentVariable "TEST_API_PROJECT_WRITE"

let createCtx key project serviceUrl =
    let client = new HttpClient ()
    defaultContext
    |> setHttpClient client
    |> addHeader ("api-key", key)
    |> setProject project
    |> setServiceUrl serviceUrl

let readCtx = createCtx testApiKeyRead "publicdata" "https://api.cognitedata.com"
let writeCtx = createCtx testApiKeyWrite "fusiondotnet-tests" "https://greenfield.cognitedata.com"

[<Fact>]
let ``Get timeseries by ids is Ok`` () = async {
    // Arrange
    let ctx = readCtx
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
let ``Create and delete timeseries is Ok`` () = async {
    // Arrange
    let ctx = writeCtx
    let externalIdString = "testThisShouldBeDeleted"
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


module Tests.Integration.Datapoints

open System
open System.Net.Http

open Xunit
open Swensen.Unquote

open Cognite.Sdk
open Tests

let testApiKeyWrite = Environment.GetEnvironmentVariable "TEST_API_KEY_WRITE"
let testApiKeyRead = Environment.GetEnvironmentVariable "TEST_API_KEY_READ"
let testApiProjectRead = Environment.GetEnvironmentVariable "TEST_API_PROJECT_READ"
let testApiProjectWrite = Environment.GetEnvironmentVariable "TEST_API_PROJECT_WRITE"

let createCtx key project =
    let client = new HttpClient ()
    defaultContext
    |> setHttpClient client
    |> addHeader ("api-key", key)
    |> setProject project

let readCtx = createCtx testApiKeyRead testApiProjectRead
let writeCtx = createCtx testApiKeyWrite testApiProjectWrite

[<Fact>]
let ``Get datapoints by ids is Ok`` () = async {
    // Arrange
    let ctx = readCtx
    let defaultOptions = [
        GetDataPoints.Option.Start "1559797200000"
        GetDataPoints.Option.End "1559808000000"
    ]
    let id = 126999346342304L

    // Act
    let! res = getDataPointsAsync defaultOptions [ (id, Seq.empty) ] ctx

    let resId =
        match res.Result with
        | Ok dtos ->
            let h = Seq.tryHead dtos
            match h with
            | Some dto -> dto.Id
            | None -> 0L
        | Error _ -> 0L
        |> Identity.Id

    let datapoints =
        match res.Result with
        | Ok dtos ->
            seq {
                for datapointDto in dtos do
                for datapoints in datapointDto.DataPoints do
                yield datapoints
            }
        | Error _ -> Seq.empty

    // Assert
    test <@ Result.isOk res.Result @>
    test <@ resId = Identity.Id id @>
    test <@ Seq.length datapoints = 15 @>
    test <@ res.Request.Method = RequestMethod.POST @>
    test <@ res.Request.Resource = "/timeseries/data/list" @>
    test <@ res.Request.Query.IsEmpty @>
}
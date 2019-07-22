module Tests.Integration.Datapoints

open System
open System.Net.Http

open Xunit
open Swensen.Unquote

open Fusion
open Tests

let testApiKeyWrite = Environment.GetEnvironmentVariable "TEST_API_KEY_WRITE"
let testApiKeyRead = Environment.GetEnvironmentVariable "TEST_API_KEY_READ"

let createCtx key project =
    let client = new HttpClient ()
    defaultContext
    |> setHttpClient client
    |> addHeader ("api-key", key)
    |> setProject project

let readCtx = createCtx testApiKeyRead "publicdata"
let writeCtx = createCtx testApiKeyWrite "fusiondotnet-tests"

[<Fact>]
let ``Get datapoints by ids is Ok`` () = async {
    // Arrange
    let ctx = readCtx
    let options = [
        GetDataPoints.QueryOption.Start "1563175800000"
        GetDataPoints.QueryOption.End "1563181200000"
    ]
    let id = 613312137748079L

    // Act
    let! res = getDataPointsAsync id options ctx

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
    test <@ Seq.length datapoints = 9 @>
    test <@ res.Request.Method = RequestMethod.POST @>
    test <@ res.Request.Resource = "/timeseries/data/list" @>
    test <@ res.Request.Query.IsEmpty @>
}
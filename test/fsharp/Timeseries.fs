module Tests.Timeseries

open System
open System.IO

open Xunit
open Swensen.Unquote

open Cognite.Sdk
open Cognite.Sdk.Timeseries
open Cognite.Sdk.Request


[<Fact>]
let ``Create timeseries is Ok`` () = async {
    // Arrenge
    let json = File.ReadAllText "Timeseries.json" // FIXME:
    let fetch = Fetch.fromJson json

    let ctx =
        defaultContext
        |> addHeader ("api-key", "test-key")

    // Act
    let! res = CreateTimeseries.createTimeseries [] fetch Async.single ctx

    // Assert
    test <@ Result.isOk res.Result @>
    test <@ res.Request.Method = RequestMethod.POST @>
    test <@ res.Request.Resource = "/timeseries" @>
    test <@ res.Request.Query.IsEmpty @>
}

[<Fact>]
let ``Get timeseries by ids is Ok`` () = async {
    // Arrenge
    let json = File.ReadAllText "Timeseries.json"
    let fetch = Fetch.fromJson json

    let ctx =
        defaultContext
        |> addHeader ("api-key", "test-key")

    // Act
    let! res = GetTimeseriesByIds.getTimeseriesByIds [ Identity.Id 0L ] fetch Async.single ctx

    // Assert
    test <@ Result.isOk res.Result @>
    test <@ res.Request.Method = RequestMethod.POST @>
    test <@ res.Request.Resource = "/timeseries/byids" @>
    test <@ res.Request.Query.IsEmpty @>
}

[<Fact>]
let ``Delete timeseries is Ok`` () = async {
    // Arrenge
    let json = File.ReadAllText "Timeseries.json"
    let fetch = Fetch.fromJson json

    let ctx =
        defaultContext
        |> addHeader ("api-key", "test-key")

    // Act
    let! res = DeleteTimeseries.deleteTimeseries [ Identity.Id 42L] fetch Async.single ctx

    // Assert
    test <@ Result.isOk res.Result @>
    test <@ res.Request.Method = RequestMethod.POST @>
    test <@ res.Request.Resource = "/timeseries/delete" @>
    test <@ res.Request.Query.IsEmpty @>
}

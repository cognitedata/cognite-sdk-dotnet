module Tests.Timeseries

open System.IO
open System.Net.Http

open Xunit
open Swensen.Unquote

open Oryx
open CogniteSdk
open CogniteSdk.TimeSeries
open CogniteSdk.DataPoints

[<Fact>]
let ``Create timeseries is Ok`` () = async {
    // Arrenge
    let json = File.ReadAllText "Timeseries.json" // FIXME:
    let fetch = Fetch.fromJson json

    let ctx =
        Context.create ()
        |> Context.setAppId "test"
        |> Context.addHeader ("api-key", "test-key")

    // Act
    let! res = TimeSeries.Create.createCore [] fetch Async.single ctx

    // Assert
    test <@ Result.isOk res.Result @>
    test <@ res.Request.Method = HttpMethod.Post @>
    test <@ res.Request.Extra.["resource"] = "/timeseries" @>
    test <@ res.Request.Query.IsEmpty @>
}

[<Fact>]
let ``Get timeseries by ids is Ok`` () = async {
    // Arrenge
    let json = File.ReadAllText "Timeseries.json"
    let fetch = Fetch.fromJson json

    let ctx =
        Context.create ()
        |> Context.setAppId "test"
        |> Context.addHeader ("api-key", "test-key")

    // Act
    let! res = TimeSeries.Retrieve.getByIdsCore [ Identity.Id 0L ] fetch Async.single ctx

    // Assert
    test <@ Result.isOk res.Result @>
    test <@ res.Request.Method = HttpMethod.Post @>
    test <@ res.Request.Extra.["resource"] = "/timeseries/byids" @>
    test <@ res.Request.Query.IsEmpty @>
}

[<Fact>]
let ``Delete timeseries is Ok`` () = async {
    // Arrenge
    let json = File.ReadAllText "Timeseries.json"
    let fetch = Fetch.fromJson json

    let ctx =
        Context.create ()
        |> Context.setAppId "test"
        |> Context.addHeader ("api-key", "test-key")

    // Act
    let! res = TimeSeries.Delete.deleteCore [ Identity.Id 42L] fetch Async.single ctx

    // Assert
    test <@ Result.isOk res.Result @>
    test <@ res.Request.Method = HttpMethod.Post @>
    test <@ res.Request.Extra.["resource"] = "/timeseries/delete" @>
    test <@ res.Request.Query.IsEmpty @>
}

[<Fact>]
let ``Parse granularity works`` () =
    // Arrenge
    let input = [
        "3d"; "d"; "5h"; "h"; "42m"; "m"; "300s"; "s"
    ]
    let expected = [
        Granularity.Day 3
        Granularity.Day 1
        Granularity.Hour 5
        Granularity.Hour 1
        Granularity.Minute 42
        Granularity.Minute 1
        Granularity.Second 300
        Granularity.Second 1
    ]

    // Act
    let granularity = input |> List.map Granularity.FromString

    // Assert
    test <@ granularity = expected @>


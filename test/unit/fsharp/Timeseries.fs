module Tests.Timeseries

open System.IO

open Xunit
open Swensen.Unquote

open Fusion
open System.Net.Http


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
    test <@ res.Request.Resource = "/timeseries" @>
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
    let! res = TimeSeries.GetByIds.getByIdsCore [ Identity.Id 0L ] fetch Async.single ctx

    // Assert
    test <@ Result.isOk res.Result @>
    test <@ res.Request.Method = HttpMethod.Post @>
    test <@ res.Request.Resource = "/timeseries/byids" @>
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
    test <@ res.Request.Resource = "/timeseries/delete" @>
    test <@ res.Request.Query.IsEmpty @>
}

[<Fact>]
let ``Parse granularity works`` () =
    // Arrenge
    let input = [
        "3d"; "d"; "5h"; "h"; "42m"; "m"; "300s"; "s"
    ]
    let expected = [
        DataPoints.Aggregated.Granularity.Day 3
        DataPoints.Aggregated.Granularity.Day 1
        DataPoints.Aggregated.Granularity.Hour 5
        DataPoints.Aggregated.Granularity.Hour 1
        DataPoints.Aggregated.Granularity.Minute 42
        DataPoints.Aggregated.Granularity.Minute 1
        DataPoints.Aggregated.Granularity.Second 300
        DataPoints.Aggregated.Granularity.Second 1
    ]

    // Act
    let granularity = input |> List.map DataPoints.Aggregated.Granularity.FromString

    // Assert
    test <@ granularity = expected @>


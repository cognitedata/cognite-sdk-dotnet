module Tests.Timeseries

open System.IO

open Xunit
open Swensen.Unquote

open Fusion


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

[<Fact>]
let ``Parse granularity works`` () =
    // Arrenge
    let input = [
        "3d"; "d"; "5h"; "h"; "42m"; "m"; "300s"; "s"
    ]
    let expected = [
        GetAggregatedDataPoints.Granularity.Day 3
        GetAggregatedDataPoints.Granularity.Day 1
        GetAggregatedDataPoints.Granularity.Hour 5
        GetAggregatedDataPoints.Granularity.Hour 1
        GetAggregatedDataPoints.Granularity.Minute 42
        GetAggregatedDataPoints.Granularity.Minute 1
        GetAggregatedDataPoints.Granularity.Second 300
        GetAggregatedDataPoints.Granularity.Second 1
    ]

    // Act
    let granularity = input |> List.map GetAggregatedDataPoints.Granularity.FromString

    // Assert
    test <@ granularity = expected @>


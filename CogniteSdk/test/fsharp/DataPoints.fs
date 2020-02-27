module Tests.Integration.Datapoints

open System
open System.Net.Http

open FSharp.Control.Tasks.V2.ContextInsensitive
open Oryx
open Swensen.Unquote
open Xunit
open Com.Cognite.V1.Timeseries.Proto

open CogniteSdk
open CogniteSdk.TimeSeries
open CogniteSdk.DataPoints


open Common
open Tests

[<Fact>]
let ``Get datapoints by id with options is Ok`` () = task {
    // Arrange
    let id = 613312137748079L
    let query =
        DataPointsQuery(
            Start = "1563175800000",
            End  = "1563181200000",
            Items = [
                DataPointsQueryItem(Id = Nullable id)
            ]
        )

    // Act
    let! res = readClient.DataPoints.ListAsync query


    let dtos = res.Items
    let resId =
        let h = Seq.tryHead dtos
        match h with
        | Some dto -> dto.Id
        | None -> 0L

    let datapoints =
        seq {
            for DataPoint in dtos do
                for dp in DataPoint.NumericDatapoints.Datapoints do
                    yield dp
        }

    // Assert
    test <@ resId = id @>
    test <@ Seq.length datapoints = 9 @>
}
[<Fact>]
let ``Get datapoints by id with limit is Ok`` () = task {
    // Arrange
    let id = 613312137748079L
    let query =
        DataPointsQuery(
            Items = [
                DataPointsQueryItem(Id = Nullable id)
            ],
            Limit = Nullable 20
        )
    // Act
    let! dtos = readClient.DataPoints.ListAsync query

    let resId =
        let h = Seq.tryHead dtos.Items
        match h with
        | Some dto -> dto.Id
        | None -> 0L

    let datapoints =
        seq {
            for DataPoint in dtos.Items do
                for dp in DataPoint.NumericDatapoints.Datapoints do
                    yield dp
        }

    // Assert
    test <@ resId = id @>
    test <@ Seq.length datapoints = 20 @>
}

[<Fact>]
let ``Get datapoints by id with limit and timerange is Ok`` () = task {
    // Arrange
    let id = 613312137748079L

    let query =
        DataPointsQuery(
            Start = "1562976000000",
            End  = "1563062399000",
            Items = [
                DataPointsQueryItem(Id = Nullable id)
            ]
        )

    // Act
    let! dtos = readClient.DataPoints.ListAsync query

    let resId =
        let h = Seq.tryHead dtos.Items
        match h with
        | Some dto -> dto.Id
        | None -> 0L

    let datapoints =
        seq {
            for DataPoint in dtos.Items do
                for dp in DataPoint.NumericDatapoints.Datapoints do
                    yield dp
        }

    // Assert
    test <@ resId = id @>
    test <@ Seq.length datapoints = 100 @>
}


[<Fact>]
let ``Get datapoints by multiple id with limit is Ok`` () = task {
    // Arrange
    let idA = 613312137748079L
    let idB = 613312137748079L
    let query =
        DataPointsQuery(
            Items = [
                DataPointsQueryItem(Id = Nullable idA, Limit=Nullable 10)
                DataPointsQueryItem(Id = Nullable idB, Limit=Nullable 10)
            ]
        )

    // Act
    let! response = readClient.DataPoints.ListAsync query

    let resIds =
        response.Items
        |> Seq.map (fun d -> d.Id)

    let datapoints =
        seq {
            for DataPoint in response.Items do
                for dp in DataPoint.NumericDatapoints.Datapoints do
                    yield dp
        }

    // Assert
    test <@ Seq.contains idA resIds && Seq.contains idB resIds @>
    test <@ Seq.length datapoints = 20 @>
}

[<Fact>]
let ``Get datapoints by id with aggregate is Ok`` () = task {
    // Arrange
    let id = 605574483685900L
    let query =
        DataPointsQuery(
            Start = "1563048800000",
            End = "1563135200000",
            Granularity = "1h",
            Aggregates = [ "average"; "sum"; "min" ],

            Items = [
                DataPointsQueryItem(Id = Nullable id)
            ]
        )

    // Act
    let! res = readClient.DataPoints.ListAsync query

    let resId =
        let h = Seq.tryHead res.Items
        match h with
        | Some dto -> dto.Id
        | None -> 0L

    let datapoints =
        seq {
            for DataPoint in res.Items do
                for dp in DataPoint.AggregateDatapoints.Datapoints do
                    yield dp
        }

    let first = Seq.head datapoints

    let greaterThanZero = function
        | x when x = nan -> false
        | x -> x > 0.0

    // Assert
    test <@ resId = id @>
    test <@ Seq.length datapoints = 25 @>
    test <@ greaterThanZero first.Average && greaterThanZero first.Min && greaterThanZero first.Sum @>
}

[<Fact>]
let ``Retrieve latest datapoints by id is Ok`` () = task {
    // Arrange
    let id = 613312137748079L
    let query =
        DataPointsLatestQuery(
            Items = [
                IdentityWithBefore(id, "2w-ago")
            ]
        )


    // Act
    let! dtos = readClient.DataPoints.LatestAsync query

    let resId =
        let h = Seq.tryHead dtos
        match h with
        | Some dto -> dto.Id
        | None -> 0L

    let datapoints =
        seq {
            for DataPoint in dtos do
                yield! DataPoint.DataPoints
        }

    // Assert
    test <@ resId = id @>
    test <@ Seq.length datapoints = 1 @>
}

[<Fact>]
let ``Insert datapoints is Ok`` () = task {
    let externalIdString = Guid.NewGuid().ToString();
    let dto =
        TimeSeriesWrite(
            ExternalId = externalIdString,
            Name = "Delete datapoints test",
            Description = "dotnet sdk test",
            IsString = false
        )


    let dataPoints = NumericDatapoints()
    dataPoints.Datapoints.Add(NumericDatapoint(Timestamp = 1563048800000L, Value = 3.0))

    let points = DataPointInsertionRequest()
    points.Items.Add [
        DataPointInsertionItem(ExternalId = externalIdString, NumericDatapoints = dataPoints)
    ]

    // Act
    let! _ = writeClient.TimeSeries.CreateAsync [ dto ]
    let! _ = writeClient.DataPoints.CreateAsync points
    let! _ = writeClient.TimeSeries.DeleteAsync [ externalIdString ]

    ()
    // Assert
}

[<Fact>]
let ``Delete datapoints is Ok`` () = task {
    // Arrange
    let externalIdString = Guid.NewGuid().ToString();
    let dto =
        TimeSeriesWrite(
            ExternalId = externalIdString,
            Name = "Delete datapoints test",
            Description = "dotnet sdk test",
            IsString = false
        )

    let startTimestamp =     1563048800000L;
    let endDeleteTimestamp = 1563048800051L
    let endTimestamp =       1563048800100L

    let dataPoints = NumericDatapoints()
    for ts in startTimestamp..endTimestamp do
        dataPoints.Datapoints.Add(NumericDatapoint(Timestamp = ts, Value = 1.0))

    let points = DataPointInsertionRequest()
    points.Items.Add [
        DataPointInsertionItem(ExternalId = externalIdString, NumericDatapoints = dataPoints)
    ]

    let delete =
        DataPointsDelete(
            Items=[
                IdentityWithRange(
                    ExternalId = externalIdString,
                    InclusiveBegin = startTimestamp,
                    ExclusiveEnd = Nullable endDeleteTimestamp
                )
            ]
        )

    // Act
    let! _ = writeClient.TimeSeries.CreateAsync [ dto ]
    let! _ = writeClient.DataPoints.CreateAsync points
    let! res = writeClient.DataPoints.DeleteAsync delete
    let! _ = writeClient.TimeSeries.DeleteAsync [ externalIdString ]

    // Assert
    ()
}


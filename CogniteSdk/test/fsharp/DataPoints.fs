module Tests.Integration.Datapoints

open System

open System.Threading.Tasks
open FSharp.Control.TaskBuilder
open Swensen.Unquote
open Xunit
open Com.Cognite.V1.Timeseries.Proto

open CogniteSdk

open Common
open Tests

[<Fact>]
let ``Get datapoints by id with options is Ok`` () = task {
    // Arrange
    let id = 6190956317771L
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
            for dps in dtos do
                for dp in dps.NumericDatapoints.Datapoints do
                    yield dp
        }

    // Assert
    test <@ resId = id @>
    test <@ Seq.length datapoints = 27 @>
}
[<Fact>]
let ``Get datapoints by id with limit is Ok`` () = task {
    // Arrange
    let id = 6190956317771L
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
            for dps in dtos.Items do
                for dp in dps.NumericDatapoints.Datapoints do
                    yield dp
        }

    // Assert
    test <@ resId = id @>
    test <@ Seq.length datapoints = 20 @>
}

[<Fact>]
let ``Get datapoints by id with limit and timerange is Ok`` () = task {
    // Arrange
    let id = 6190956317771L

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
            for dps in dtos.Items do
                for dp in dps.NumericDatapoints.Datapoints do
                    yield dp
        }

    // Assert
    test <@ resId = id @>
    test <@ Seq.length datapoints = 100 @>
}


[<Fact>]
let ``Get datapoints by multiple id with limit is Ok`` () = task {
    // Arrange
    let idA = 6190956317771L
    let idB = 25870989735584L
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
            for dps in response.Items do
                for dp in dps.NumericDatapoints.Datapoints do
                    yield dp
        }

    // Assert
    test <@ Seq.contains idA resIds && Seq.contains idB resIds @>
    test <@ Seq.length datapoints = 20 @>
}

[<Fact>]
let ``Get datapoints by id with aggregate is Ok`` () = task {
    // Arrange
    let id = 138649441615650L
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
            for dps in res.Items do
                for dp in dps.AggregateDatapoints.Datapoints do
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
    let id = 6190956317771L
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
            for dps in dtos do
                yield! dps.DataPoints
        }

    // Assert
    test <@ resId = id @>
    test <@ Seq.length datapoints = 1 @>
}

[<Fact>]
let ``Insert datapoints is Ok`` () = task {
    let externalIdString = Guid.NewGuid().ToString();
    let dto =
        TimeSeriesCreate(
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
    let! _ = writeClient.DataPoints.CreateAsync (points, System.IO.Compression.CompressionLevel.Fastest)
    let! _ = writeClient.TimeSeries.DeleteAsync [ externalIdString ]

    ()
    // Assert
}

[<Fact>]
let ``Delete datapoints is Ok`` () = task {
    // Arrange
    let externalIdString = Guid.NewGuid().ToString();
    let dto =
        TimeSeriesCreate(
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

[<Fact>]
let ``Interact with datapoints using the new unit capabilities is Ok`` () = task {
    // Arrange
    let externalIdString = Guid.NewGuid().ToString();
    let unitExternalId = "temperature:deg_c"
    let dto =
        TimeSeriesCreate(
            ExternalId = externalIdString,
            Name = "Interact with datapoints test",
            Description = "dotnet sdk test",
            IsString = false,
            UnitExternalId = unitExternalId
        )

    let startTimestamp = 1704672000000L;
    let timestamps = [ startTimestamp; startTimestamp + 3600000L ; startTimestamp + 7200000L ]
    let values = [ 30.5; 29.4; 13.2 ]
    let valuesFahrenheit = values |> List.map (fun v -> v * 1.8 + 32.0)
    let valuesKelvin = values |> List.map (fun v -> v + 273.15)

    let dataPoints = NumericDatapoints()
    dataPoints.Datapoints.AddRange(
        List.zip timestamps values |> List.map (
            fun (ts, v) -> NumericDatapoint(Timestamp = ts, Value = v)
        )
    )
    let dataPointsRequest = DataPointInsertionRequest()
    dataPointsRequest.Items.Add [
        DataPointInsertionItem(ExternalId = externalIdString, NumericDatapoints = dataPoints)
    ]
    
    let query1 =
        DataPointsQuery(
            Start = startTimestamp.ToString(),
            Items = [
                DataPointsQueryItem(ExternalId = externalIdString)
            ]
        )
    let query2 = 
        DataPointsQuery(
            Start = startTimestamp.ToString(),
            Items = [
                DataPointsQueryItem(ExternalId = externalIdString, TargetUnit = "temperature:deg_f")
            ]
        )
    let query3 =
        DataPointsQuery(
            Start = startTimestamp.ToString(),
            Items = [
                DataPointsQueryItem(ExternalId = externalIdString, TargetUnitSystem = "SI")
            ]
    )

    // Act
    let! _ = writeClient.TimeSeries.CreateAsync [ dto ]
    do! Task.Delay 1000 // Wait for 1 second
    
    let! _ = writeClient.DataPoints.CreateAsync dataPointsRequest
    do! Task.Delay 1000 // Wait for 1 second
    
    let! resWithoutConversion = writeClient.DataPoints.ListAsync query1
    let! resWithTargetUnit = writeClient.DataPoints.ListAsync query2
    let! resWithTargetUnitSystem = writeClient.DataPoints.ListAsync query3
    let! _ = writeClient.TimeSeries.DeleteAsync [ externalIdString ]
    
    let valuesWithoutConversion = resWithoutConversion.Items
                                  |> Seq.head
                                  |> fun dps -> dps.NumericDatapoints.Datapoints
                                  |> Seq.map (fun dp -> dp.Value)
    let valuesWithTargetUnit = resWithTargetUnit.Items
                               |> Seq.head
                               |> fun dps -> dps.NumericDatapoints.Datapoints
                               |> Seq.map (fun dp -> dp.Value)
    let valuesWithTargetUnitSystem = resWithTargetUnitSystem.Items
                                     |> Seq.head
                                     |> fun dps -> dps.NumericDatapoints.Datapoints
                                     |> Seq.map (fun dp -> dp.Value)

    // Assert
    test <@ Seq.compareWith compare valuesWithoutConversion values = 0 @> // No conversion
    test <@ Seq.compareWith compare valuesWithTargetUnit valuesFahrenheit = 0 @> // Conversion from Celsius to Fahrenheit
    test <@ Seq.compareWith compare valuesWithTargetUnitSystem valuesKelvin = 0 @> // Conversion from Celsius to Kelvin
}
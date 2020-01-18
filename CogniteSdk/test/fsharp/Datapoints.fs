module Tests.Integration.Datapoints

open System
open System.Net.Http

open Xunit
open Swensen.Unquote

open Oryx
open CogniteSdk
open CogniteSdk.TimeSeries
open CogniteSdk.DataPoints
open Common
open Tests
open FSharp.Control.Tasks.V2.ContextInsensitive

[<Fact>]
let ``Get datapoints by id with options is Ok`` () = task {
    // Arrange
    let ctx = readCtx ()
    let options = [
        DataPoints.DataPointQuery.Start "1563175800000"
        DataPointQuery.End "1563181200000"
    ]
    let id = 613312137748079L

    // Act
    let! res = DataPoints.Items.listAsync id options ctx

    let ctx' =
        match res with
        | Ok ctx -> ctx
        | Error (ResponseError error) -> raise <| error.ToException ()
        | Error (Panic error) -> raise error

    let dtos = ctx'.Response
    let resId =
        let h = Seq.tryHead dtos
        match h with
        | Some dto -> Identity.Id dto.Id
        | None -> Identity.Id 0L

    let datapoints =
        seq {
            for datapointDto in dtos do
                match datapointDto.DataPoints with
                | Numeric dps -> yield! dps
                | String dps -> failwith "Unexpected string datapoints"
        }

    // Assert
    test <@ resId = Identity.Id id @>
    test <@ Seq.length datapoints = 9 @>
    test <@ ctx'.Request.Method = HttpMethod.Post @>
    test <@ ctx'.Request.Extra.["resource"] = "/timeseries/data/list" @>
    test <@ ctx'.Request.Query.IsEmpty @>
}

[<Fact>]
let ``Get datapoints by id with limit is Ok`` () = task {
    // Arrange
    let ctx = readCtx ()
    let options = [
        DataPointQuery.Limit 20
    ]
    let id = 613312137748079L

    // Act
    let! res = DataPoints.Items.listAsync id options ctx

    let ctx' =
        match res with
        | Ok ctx -> ctx
        | Error (ResponseError error) -> raise <| error.ToException ()
        | Error (Panic error) -> raise error

    let dtos = ctx'.Response
    let resId =
        let h = Seq.tryHead dtos
        match h with
        | Some dto -> Identity.Id dto.Id
        | None -> Identity.Id 0L

    let datapoints =
        seq {
            for datapointDto in dtos do
                match datapointDto.DataPoints with
                | Numeric dps -> yield! dps
                | String dps -> failwith "Unexpected string datapoints"
        }

    // Assert
    test <@ resId = Identity.Id id @>
    test <@ Seq.length datapoints = 20 @>
    test <@ ctx'.Request.Method = HttpMethod.Post @>
    test <@ ctx'.Request.Extra.["resource"] = "/timeseries/data/list" @>
    test <@ ctx'.Request.Query.IsEmpty @>
}

[<Fact>]
let ``Get datapoints by id with limit and timerange is Ok`` () = task {
    // Arrange
    let ctx = readCtx ()
    let options = [
        DataPointQuery.Start "1562976000000"
        DataPointQuery.End   "1563062399000"
    ]
    let id = 613312137748079L

    // Act
    let! res = DataPoints.Items.listAsync id options ctx

    let ctx' =
        match res with
        | Ok ctx -> ctx
        | Error (ResponseError error) -> raise <| error.ToException ()
        | Error (Panic error) -> raise error

    let dtos = ctx'.Response

    let resId =
        let h = Seq.tryHead dtos
        match h with
        | Some dto -> Identity.Id dto.Id
        | None -> Identity.Id 0L

    let datapoints =
        seq {
            for datapointDto in dtos do
                match datapointDto.DataPoints with
                | Numeric dps -> yield! dps
                | String dps -> failwith "Unexpected string datapoints"
        }

    // Assert
    test <@ resId = Identity.Id id @>
    test <@ Seq.length datapoints = 100 @>
    test <@ ctx'.Request.Method = HttpMethod.Post @>
    test <@ ctx'.Request.Extra.["resource"] = "/timeseries/data/list" @>
    test <@ ctx'.Request.Query.IsEmpty @>
}


[<Fact>]
let ``Get datapoints by multiple id with limit is Ok`` () = task {
    // Arrange
    let ctx = readCtx ()

    let a: DataPointMultipleQuery = {
        Id = Identity.Id 613312137748079L
        QueryOptions = [ DataPointQuery.Limit 10 ]
    }
    let b: DataPointMultipleQuery = {
        Id = Identity.Id 605574483685900L
        QueryOptions = [ DataPointQuery.Limit 10 ]
    }
    let query: DataPointMultipleQuery seq = Seq.ofList [ a; b ]

    // Act
    let! res = DataPoints.Items.listMultipleAsync query [] ctx

    let ctx' =
        match res with
        | Ok ctx -> ctx
        | Error (ResponseError error) -> raise <| error.ToException ()
        | Error (Panic error) -> raise error

    let dtos = ctx'.Response

    let resIds =
        dtos
        |> Seq.map (fun (d: DataPoints.Items.DataPoints) -> Identity.Id d.Id)

    let datapoints =
        seq {
            for datapointDto in dtos do
                match datapointDto.DataPoints with
                | Numeric dps -> yield! dps
                | String dps -> failwith "Unexpected string datapoints"
        }

    // Assert
    test <@ Seq.contains a.Id resIds && Seq.contains b.Id resIds @>
    test <@ Seq.length datapoints = 20 @>
    test <@ ctx'.Request.Method = HttpMethod.Post @>
    test <@ ctx'.Request.Extra.["resource"] = "/timeseries/data/list" @>
    test <@ ctx'.Request.Query.IsEmpty @>
}

[<Fact>]
let ``Get datapoints by id with aggregate is Ok`` () = task {
    // Arrange
    let ctx = readCtx ()
    let options = [
        AggregateQuery.Start "1563048800000"
        AggregateQuery.End "1563135200000"
        AggregateQuery.Granularity (Granularity.Hour 1)
        AggregateQuery.Aggregates [
            Aggregate.Average
            Aggregate.Sum
            Aggregate.Min
        ]
    ]

    let id = 605574483685900L

    // Act
    let! res = DataPoints.Aggregated.getAggregatedAsync (Identity.Id id) options ctx

    let ctx' =
        match res with
        | Ok ctx -> ctx
        | Error (ResponseError error) -> raise <| error.ToException ()
        | Error (Panic error) -> raise error

    let dtos = ctx'.Response

    let resId =
        let h = Seq.tryHead dtos
        match h with
        | Some dto -> dto.Id
        | None -> 0L

    let datapoints =
        seq {
            for datapointDto in dtos do
                yield! datapointDto.DataPoints
        }

    let first = Seq.head datapoints

    let greaterThanZero = function
        | Some x -> x > 0.0
        | None -> false

    // Assert
    test <@ resId = id @>
    test <@ Seq.length datapoints = 25 @>
    test <@ greaterThanZero first.Average && greaterThanZero first.Min && greaterThanZero first.Sum @>
    test <@ ctx'.Request.Method = HttpMethod.Post @>
    test <@ ctx'.Request.Extra.["resource"] = "/timeseries/data/list" @>
    test <@ ctx'.Request.Query.IsEmpty @>
}


[<Fact>]
let ``Retrieve latest datapoints by id is Ok`` () = task {
    // Arrange
    let ctx = readCtx ()
    let latestDataPointRequest: DataPoints.Latest.LatestRequest = {
        Before = Some "2w-ago"
        Identity = Identity.Id 613312137748079L
    }

    let id = 613312137748079L

    // Act
    let! res = DataPoints.Latest.getAsync [ latestDataPointRequest ] ctx

    let ctx' =
        match res with
        | Ok ctx -> ctx
        | Error (ResponseError error) -> raise <| error.ToException ()
        | Error (Panic error) -> raise error

    let dtos = ctx'.Response

    let resId =
        let h = Seq.tryHead dtos
        match h with
        | Some dto -> Identity.Id dto.Id
        | None -> Identity.Id 0L

    let datapoints =
        seq {
            for datapointDto in dtos do
                match datapointDto.DataPoints with
                | Numeric dps -> yield! dps
                | String _ -> failwith "Unexpected string datapoints"
        }

    // Assert
    test <@ resId = Identity.Id id @>
    test <@ Seq.length datapoints = 1 @>
    test <@ ctx'.Request.Method = HttpMethod.Post @>
    test <@ ctx'.Request.Extra.["resource"] = "/timeseries/data/latest" @>
    test <@ ctx'.Request.Query.IsEmpty @>
}

[<Fact>]
let ``Insert datapoints is Ok`` () = task {
    // Arrange
    let ctx = writeCtx ()
    let externalIdString = Guid.NewGuid().ToString();
    let dto: TimeSeriesWriteDto = {
        ExternalId = Some externalIdString
        Name = Some "Insert datapoints test"
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

    let points: NumericDataPointDto seq = Seq.ofList [ { TimeStamp = 1563048800000L; Value = 3.0} ]

    let datapoints: DataPoints.Insert.DataPoints = {
        DataPoints = Numeric points
        Identity = Identity.ExternalId externalIdString
    }

    // Act
    let! _ = TimeSeries.Create.createAsync [ dto ] ctx
    let! res = DataPoints.Insert.insertAsync [ datapoints ] ctx
    let! _ = TimeSeries.Delete.deleteAsync [ externalId ] ctx

    let ctx' =
        match res with
        | Ok ctx -> ctx
        | Error (ResponseError error) -> raise <| error.ToException ()
        | Error (Panic error) -> raise error

    // Assert
    test <@ Result.isOk res @>
    test <@ ctx'.Request.Method = HttpMethod.Post @>
    test <@ ctx'.Request.Extra.["resource"] = "/timeseries/data" @>
}

[<Fact>]
let ``Delete datapoints is Ok`` () = task {
    // Arrange
    let ctx = writeCtx ()
    let externalIdString = Guid.NewGuid().ToString();
    let dto: TimeSeriesWriteDto = {
        ExternalId = Some externalIdString
        Name = Some "Delete datapoints test"
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

    let startTimestamp =     1563048800000L;
    let endDeleteTimestamp = 1563048800051L
    let endTimestamp =       1563048800100L
    let points : NumericDataPointDto seq = seq { for i in startTimestamp..endTimestamp do yield { TimeStamp = i; Value = 3.0 } }

    let datapoints : DataPoints.Insert.DataPoints = {
        DataPoints = Numeric points
        Identity = Identity.ExternalId externalIdString
    }

    // Act
    let request =
        oryx {
            (*
            let! tss = TimeSeries.GetByIds.getByIds [ Identity.ExternalId externalIdString ]
            if not (Seq.empty tss) then
                do! TimeSeries.Delete.delete [ externalId ]
            *)
            let! _ = TimeSeries.Create.create [ dto ]
            let! _ = DataPoints.Insert.insert [ datapoints ]
            let! res = DataPoints.Delete.delete [{ InclusiveBegin = startTimestamp; ExclusiveEnd = Some endDeleteTimestamp; Id = externalId }]
            let! _ = TimeSeries.Delete.delete [ externalId ]
            return res
        }
    let! res = runAsync request ctx
    // Assert
    test <@ Result.isOk res @>
}

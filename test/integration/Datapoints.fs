module Tests.Integration.Datapoints

open Xunit
open Swensen.Unquote

open Fusion
open Tests
open Fusion.TimeSeries
open Common
open System.Net.Http

[<Fact>]
let ``Get datapoints by id with options is Ok`` () = async {
    // Arrange
    let ctx = readCtx ()
    let options = [
        DataPoints.List.QueryOption.Start "1563175800000"
        DataPoints.List.QueryOption.End "1563181200000"
    ]
    let id = 613312137748079L

    // Act
    let! res = DataPoints.List.listAsync id options ctx

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
                    match datapointDto.DataPoints with
                    | Numeric dps -> yield! dps
                    | String dps -> failwith "Unexpected string datapoints"
            }
        | Error _ -> Seq.empty

    // Assert
    test <@ Result.isOk res.Result @>
    test <@ resId = Identity.Id id @>
    test <@ Seq.length datapoints = 9 @>
    test <@ res.Request.Method = HttpMethod.Post @>
    test <@ res.Request.Resource = "/timeseries/data/list" @>
    test <@ res.Request.Query.IsEmpty @>
}

[<Fact>]
let ``Get datapoints by id with limit is Ok`` () = async {
    // Arrange
    let ctx = readCtx ()
    let options = [
        DataPoints.List.QueryOption.Limit 20
    ]
    let id = 613312137748079L

    // Act
    let! res = DataPoints.List.listAsync id options ctx

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
                    match datapointDto.DataPoints with
                    | Numeric dps -> yield! dps
                    | String dps -> failwith "Unexpected string datapoints"
            }
        | Error _ -> Seq.empty

    // Assert
    test <@ Result.isOk res.Result @>
    test <@ resId = Identity.Id id @>
    test <@ Seq.length datapoints = 20 @>
    test <@ res.Request.Method = HttpMethod.Post @>
    test <@ res.Request.Resource = "/timeseries/data/list" @>
    test <@ res.Request.Query.IsEmpty @>
}

[<Fact>]
let ``Get datapoints by id with limit and timerange is Ok`` () = async {
    // Arrange
    let ctx = readCtx ()
    let options = [
        DataPoints.List.QueryOption.Start "1562976000000"
        DataPoints.List.QueryOption.End   "1563062399000"
    ]
    let id = 613312137748079L

    // Act
    let! res = DataPoints.List.listAsync id options ctx

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
                    match datapointDto.DataPoints with
                    | Numeric dps -> yield! dps
                    | String dps -> failwith "Unexpected string datapoints"
            }
        | Error _ -> Seq.empty

    // Assert
    test <@ Result.isOk res.Result @>
    test <@ resId = Identity.Id id @>
    test <@ Seq.length datapoints = 100 @>
    test <@ res.Request.Method = HttpMethod.Post @>
    test <@ res.Request.Resource = "/timeseries/data/list" @>
    test <@ res.Request.Query.IsEmpty @>
}


[<Fact>]
let ``Get datapoints by multiple id with limit is Ok`` () = async {
    // Arrange
    let ctx = readCtx ()

    let a: DataPoints.List.Option = {
        Id = Identity.Id 613312137748079L
        QueryOptions = [ DataPoints.List.QueryOption.Limit 10 ]
    }
    let b: DataPoints.List.Option = {
        Id = Identity.Id 605574483685900L
        QueryOptions = [ DataPoints.List.QueryOption.Limit 10 ]
    }
    let defaultOptions: DataPoints.List.Option seq = Seq.ofList [ a; b ]

    // Act
    let! res = DataPoints.List.listMultipleAsync defaultOptions [] ctx

    let resIds =
        match res.Result with
        | Ok dtos ->
            Seq.map (fun (d: DataPoints.List.DataPoints) -> d.Id) dtos
        | Error _ -> Seq.ofList [ 0L ]
        |> Seq.map Identity.Id

    let datapoints =
        match res.Result with
        | Ok dtos ->
            seq {
                for datapointDto in dtos do
                    match datapointDto.DataPoints with
                    | Numeric dps -> yield! dps
                    | String dps -> failwith "Unexpected string datapoints"
            }
        | Error _ -> Seq.empty

    // Assert
    test <@ Result.isOk res.Result @>
    test <@ Seq.contains a.Id resIds && Seq.contains b.Id resIds @>
    test <@ Seq.length datapoints = 20 @>
    test <@ res.Request.Method = HttpMethod.Post @>
    test <@ res.Request.Resource = "/timeseries/data/list" @>
    test <@ res.Request.Query.IsEmpty @>
}

[<Fact>]
let ``Get datapoints by id with aggregate is Ok`` () = async {
    // Arrange
    let ctx = readCtx ()
    let options = [
        DataPoints.Aggregated.QueryOption.Start "1563048800000"
        DataPoints.Aggregated.QueryOption.End "1563135200000"
        DataPoints.Aggregated.QueryOption.Granularity (DataPoints.Aggregated.Granularity.Hour 1)
        DataPoints.Aggregated.QueryOption.Aggregates [
            DataPoints.Aggregated.Aggregate.Average
            DataPoints.Aggregated.Aggregate.Sum
            DataPoints.Aggregated.Aggregate.Min
        ]
    ]

    let id = 605574483685900L

    // Act
    let! res = DataPoints.Aggregated.getAggregatedAsync (Identity.Id id) options ctx
    let resId =
        match res.Result with
        | Ok dtos ->
            let h = Seq.tryHead dtos
            match h with
            | Some dto -> dto.Id
            | None -> 0L
        | Error _ -> 0L

    let datapoints =
        match res.Result with
        | Ok dtos ->
            seq {
                for datapointDto in dtos do
                    yield! datapointDto.DataPoints
            }
        | Error err -> err |> printfn "%A"; Seq.empty

    let first = Seq.head datapoints

    let greaterThanZero = function
        | Some x -> x > 0.0
        | None -> false

    // Assert
    test <@ Result.isOk res.Result @>
    test <@ resId = id @>
    test <@ Seq.length datapoints = 25 @>
    test <@ greaterThanZero first.Average && greaterThanZero first.Min && greaterThanZero first.Sum @>
    test <@ res.Request.Method = HttpMethod.Post @>
    test <@ res.Request.Resource = "/timeseries/data/list" @>
    test <@ res.Request.Query.IsEmpty @>
}


[<Fact>]
let ``Retrieve latest datapoints by id is Ok`` () = async {
    // Arrange
    let ctx = readCtx ()
    let latestDataPointRequest: DataPoints.Latest.LatestRequest = {
        Before = Some "2w-ago"
        Identity = Identity.Id 613312137748079L
    }

    let id = 613312137748079L

    // Act
    let! res = DataPoints.Latest.getAsync [ latestDataPointRequest ] ctx

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
                    match datapointDto.DataPoints with
                    | Numeric dps -> yield! dps
                    | String dps -> failwith "Unexpected string datapoints"
            }
        | Error _ -> Seq.empty

    // Assert
    test <@ Result.isOk res.Result @>
    test <@ resId = Identity.Id id @>
    test <@ Seq.length datapoints = 1 @>
    test <@ res.Request.Method = HttpMethod.Post @>
    test <@ res.Request.Resource = "/timeseries/data/latest" @>
    test <@ res.Request.Query.IsEmpty @>
}

[<Fact>]
let ``Insert datapoints is Ok`` () = async {
    // Arrange
    let ctx = writeCtx ()
    let externalIdString = "insertDatapointsTest"
    let dto: TimeSeries.WriteDto = {
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

    // Assert
    test <@ Result.isOk res.Result @>
    test <@ res.Request.Method = HttpMethod.Post @>
    test <@ res.Request.Resource = "/timeseries/data" @>
}

[<Fact>]
let ``Delete datapoints is Ok`` () = async {
    // Arrange
    let ctx = writeCtx ()
    let externalIdString = "deleteDatapointsTest"
    let dto: TimeSeries.WriteDto = {
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
        fusion {
            let! _ = TimeSeries.Create.create [ dto ]
            do! DataPoints.Insert.insert [ datapoints ]
            let! res = DataPoints.Delete.delete [{ InclusiveBegin = startTimestamp; ExclusiveEnd = Some endDeleteTimestamp; Id = externalId }]
            do! TimeSeries.Delete.delete [ externalId ]
            return res
        }
    let! res = runHandler request ctx
    // Assert
    test <@ Result.isOk res @>
}

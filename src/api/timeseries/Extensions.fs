namespace Cognite.Sdk.Api

open System
open System.Collections.Generic
open System.Threading.Tasks;
open System.Runtime.InteropServices
open System.Runtime.CompilerServices

open Cognite.Sdk
open Cognite.Sdk.Api
open Cognite.Sdk.Timeseries
open Cognite.Sdk.Common



[<Extension>]
type Timeseries =
    /// Create a new Asset with name and description (non optional).
    /// Optional properties can then be added using Set* methods such as e.g {SetMetaData}.
    [<Extension>]
    static member Create () : TimeseriesCreateDto =
        {
            ExternalId = None
            Name = None
            IsString = false
            MetaData = Map.empty
            Unit = None
            AssetId = None
            IsStep = false
            Description = None
            SecurityCategories = []
        }

    /// Set Name of timeseries.
    [<Extension>]
    static member SetName (this: TimeseriesCreateDto, name: string) : TimeseriesCreateDto =
        { this with Name = Some name }

    /// Set ExternalId of timeseries.
    [<Extension>]
    static member SetExternalId (this: TimeseriesCreateDto, externalId: string) : TimeseriesCreateDto =
        { this with ExternalId = Some externalId }

    /// Set Unit of timeseries.
    [<Extension>]
    static member SetUnit (this: TimeseriesCreateDto, unit: string) : TimeseriesCreateDto =
        { this with Unit = Some unit }

    /// Set IsString of timeseries.
    [<Extension>]
    static member SetIsString (this: TimeseriesCreateDto, isString: bool) : TimeseriesCreateDto =
        { this with IsString = isString }

    // Set IsStep of timeseries.
    [<Extension>]
    static member SetIsStep (this: TimeseriesCreateDto, isStep: bool) : TimeseriesCreateDto =
        { this with IsStep = isStep }

    /// Set custom, application specific metadata. String key -> String value.
    [<Extension>]
    static member SetMetaData (this: TimeseriesCreateDto, metaData: Dictionary<string, string>) : TimeseriesCreateDto =
        let map =
            metaData
            |> Seq.map (|KeyValue|)
            |> Map.ofSeq
        { this with MetaData = map }

    /// Set AssetId of timeseries.
    [<Extension>]
    static member SetAssetId (this: TimeseriesCreateDto, assetId: int64) : TimeseriesCreateDto =
        { this with AssetId = Some assetId }

    /// Set Description of timeseries.
    [<Extension>]
    static member SetDescription (this: TimeseriesCreateDto, description: string) : TimeseriesCreateDto =
        { this with Description = Some description }


    /// Set Security Categories of timeseries.
    [<Extension>]
    static member SetSequrityCategories (this: TimeseriesCreateDto, sc: seq<int64>) : TimeseriesCreateDto =
        { this with SecurityCategories = sc }

    [<Extension>]
    static member TryGetValue (this: DataPointDto, [<Out>] value: byref<Int64>) =
        match this.Value with
        | NumInteger value' ->
            value <- value'
            true
        | _ -> false

    [<Extension>]
    static member TryGetValue (this: DataPointDto, [<Out>] value: byref<string>) =
        match this.Value with
        | NumString value' ->
            value <- value'
            true
        | _ -> false

    [<Extension>]
    static member TryGetValue (this: DataPointDto, [<Out>] value: byref<float>) =
        match this.Value with
        | NumFloat value' ->
            value <- value'
            true
        | _ -> false

    [<Extension>]
    static member TryGetAverage (this: AggregateDataPointReadDto, [<Out>] value: byref<float>) =
        match this.Average with
        | Some value' ->
            value <- value'
            true
        | _ -> false

    [<Extension>]
    static member TryGetMax (this: AggregateDataPointReadDto, [<Out>] value: byref<float>) =
        match this.Max with
        | Some value' ->
            value <- value'
            true
        | _ -> false

    [<Extension>]
    static member TryGetMin (this: AggregateDataPointReadDto, [<Out>] value: byref<float>) =
        match this.Min with
        | Some value' ->
            value <- value'
            true
        | _ -> false

    [<Extension>]
    static member TryGetCount (this: AggregateDataPointReadDto, [<Out>] value: byref<int>) =
        match this.Count with
        | Some value' ->
            value <- value'
            true
        | _ -> false

    [<Extension>]
    static member TryGetSum (this: AggregateDataPointReadDto, [<Out>] value: byref<float>) =
        match this.Sum with
        | Some value' ->
            value <- value'
            true
        | _ -> false

    [<Extension>]
    static member TryGetInterpolation (this: AggregateDataPointReadDto, [<Out>] value: byref<float>) =
        match this.Interpolation with
        | Some value' ->
            value <- value'
            true
        | _ -> false

    [<Extension>]
    static member TryGetStepInterpolation (this: AggregateDataPointReadDto, [<Out>] value: byref<float>) =
        match this.StepInterpolation with
        | Some value' ->
            value <- value'
            true
        | _ -> false

    [<Extension>]
    static member TryGetContinousVariance (this: AggregateDataPointReadDto, [<Out>] value: byref<float>) =
        match this.ContinousVariance with
        | Some value' ->
            value <- value'
            true
        | _ -> false

    [<Extension>]
    static member TryGetDiscreteVariance (this: AggregateDataPointReadDto, [<Out>] value: byref<float>) =
        match this.DiscreteVariance with
        | Some value' ->
            value <- value'
            true
        | _ -> false

    [<Extension>]
    static member TryGetTotalVariation (this: AggregateDataPointReadDto, [<Out>] value: byref<float>) =
        match this.TotalVariation with
        | Some value' ->
            value <- value'
            true
        | _ -> false

type Query (parameters : QueryParam list) =
    let _params = parameters

    member this.Limit (limit: int) =
        Query (QueryParam.Limit limit :: _params)

    member this.Cursor (cursor: string) =
        Query (Cursor cursor :: _params)

    member this.IncludeMetaData (flag: bool) =
        Query (IncludeMetaData flag :: _params)

    member this.AssetIds (assetIds: int64 seq) =
        Query (AssetIds assetIds :: _params)

    member internal this.Params = Seq.ofList _params

type QueryData (query: QueryDataParam list) =

    let query = query

    member this.Start (start: string) =
        QueryData (Start start :: query)

    member this.End (endTime: string) =
        QueryData (End endTime :: query)

    member this.Limit (limit: int) =
        QueryData (QueryDataParam.Limit limit :: query)

    member this.Aggregates (aggregate: Aggregate seq) =
        QueryData (QueryDataParam.Aggregates aggregate :: query)

    member this.IncludeOutsidePoints (iop: bool) =
        QueryData (IncludeOutsidePoints iop :: query)

    member internal this.Query = Seq.ofList query

    static member Create () =
        QueryData []

type QueryDataLatest (latest: LatestDataRequest) =
    let latest : LatestDataRequest = latest

    member this.Before (before: string) =
        QueryDataLatest { latest with Before = Some before}

    member this.Id (id: int64) =
        QueryDataLatest { latest with Identity = Common.Identity.Id id }

    member this.ExternalId (externalId: string) =
        QueryDataLatest { latest with Identity = Common.Identity.ExternalId externalId }

    member internal this.Latest = latest

    static member Create () =
        QueryDataLatest { Identity = Common.Identity.Id 0L; Before = None }


[<Extension>]
type ClientTimeseriesExtensions =
    /// <summary>
    /// Retrieves a list of data points from multiple time series in the same project.
    /// </summary>
    /// <param name="defaultQuery">Parameters describing a query for multiple datapoints. If fields in individual
    /// datapoint query items are omitted, top-level values are used instead.</param>
    /// <param name="query">Parameters describing a query for multiple datapoints.</param>
    /// <param name="items">The list of data points to insert.</param>
    /// <returns>Http status code.</returns>
    [<Extension>]
    static member GetTimeseriesDataAsync (this: Client) (defaultQuery: QueryData) (query: Tuple<int64, QueryData> seq) : Task<seq<PointResponseDataPoints>> =
        let worker () : Async<seq<PointResponseDataPoints>> = async {
            let defaultQuery' = defaultQuery.Query
            let query' = query |> Seq.map (fun (id, query) -> (id, query.Query))
            let! result = Internal.getTimeseriesDataResult defaultQuery' query' this.Fetch this.Ctx
            match result with
            | Ok response ->
                return response
            | Error error ->
               return raise (Error.error2Exception error)
        }

        worker () |> Async.StartAsTask

    /// <summary>
    /// Retrieves the single latest data point in a time series.
    /// </summary>
    /// <param name="queryParams">Instance of QueryDataLatest object with query parameters.</param>
    /// <param name="client">The list of data points to insert.</param>
    /// <returns>Http status code.</returns>
    [<Extension>]
    static member GetTimeseriesLatestDataAsync (this: Client) (queryParams: QueryDataLatest seq) : Task<seq<PointResponseDataPoints>> =
        let worker () : Async<seq<PointResponseDataPoints>> = async {
            let query = queryParams |> Seq.map (fun p -> p.Latest)
            let! result = Internal.getTimeseriesLatestDataResult query this.Fetch this.Ctx
            match result with
            | Ok response ->
                return response
            | Error error ->
               return raise (Error.error2Exception error)
        }

        worker () |> Async.StartAsTask

    /// <summary>
    /// Insert data into named time series.
    /// </summary>
    /// <param name="name">The name of the timeseries to insert data into.</param>
    /// <param name="items">The list of data points to insert.</param>
    /// <returns>Http status code.</returns>
    [<Extension>]
    static member InsertDataAsync (this: Client) (items: DataPoints seq) : Task<int> =
        let items' =
            Seq.map  (fun (it :  DataPoints) ->
                {
                    DataPoints = Seq.map (fun point ->
                        match box point with
                        | :? DataPointInteger as number ->
                            { Value = NumInteger number.Value; TimeStamp = number.TimeStamp }
                        | :? DataPointFloat as real ->
                            { Value = NumFloat real.Value; TimeStamp = real.TimeStamp }
                        | :? DataPointString as string ->
                            { Value = NumString string.Value; TimeStamp = string.TimeStamp }
                        | _ -> failwith "Unknown point type"
                    ) it.DataPoints
                    Identity =
                        match it.Identity with
                        | :? IdentityId as id ->
                            Common.Identity.Id id.Value
                        | :? IdentityExternalId as ex ->
                            Common.Identity.ExternalId ex.Value
                        | _ -> failwith "unknow identity"
                }
            ) items

        let worker () : Async<int> = async {
            let! result = Internal.insertDataResult items' this.Fetch this.Ctx
            match result with
            | Ok response ->
                return response.StatusCode
            | Error error ->
               return raise (Error.error2Exception error)
        }

        worker () |> Async.StartAsTask

    /// <summary>
    /// Create a new timeseries.
    /// </summary>
    /// <param name="items">The list of timeseries to create.</param>
    /// <returns>Http status code.</returns>
    [<Extension>]
    static member CreateTimeseriesAsync (this: Client) (items: seq<TimeseriesCreateDto>) : Task<TimeseriesResponse> =
        let worker () : Async<TimeseriesResponse> = async {
            let! result = Internal.createTimeseriesResult items this.Fetch this.Ctx
            match result with
            | Ok response ->
                return response
            | Error error ->
               return raise (Error.error2Exception error)
        }

        worker () |> Async.StartAsTask

    /// <summary>
    /// Get timeseries
    /// </summary>
    /// <param name="id">The id of the timeseries to get.</param>
    /// <returns>The timeseries with the given id.</returns>
    [<Extension>]
    static member GetTimeseriesAsync (this: Client) (queryParams: Query) : Task<TimeseriesResponse> =
        let worker () : Async<TimeseriesResponse> = async {
            let! result = Internal.getTimeseriesResult queryParams.Params this.Fetch this.Ctx

            match result with
            | Ok response ->
                return response
            | Error error ->
               return raise (Error.error2Exception error)
        }

        worker () |> Async.StartAsTask

    /// <summary>
    /// Get timeseries with given id.
    /// </summary>
    /// <param name="id">The id of the timeseries to get.</param>
    /// <returns>The timeseries with the given id.</returns>
    [<Extension>]
    static member GetTimeseriesByIdsAsync (this: Client) (ids: seq<int64>) : Task<seq<TimeseriesReadDto>> =
        let worker () : Async<seq<TimeseriesReadDto>> = async {
            let! result = Internal.getTimeseriesByIdsResult ids this.Fetch this.Ctx

            match result with
            | Ok response ->
                return response
            | Error error ->
               return raise (Error.error2Exception error)
        }

        worker () |> Async.StartAsTask

    /// <summary>
    /// Delete timeseries.
    /// </summary>
    /// <param name="name">The name of the timeseries to delete.</param>
    /// <returns>List of created timeseries.</returns>
    [<Extension>]
    static member DeleteTimeseriesAsync (this: Client) (name: string) : Task<int> =
        let worker () : Async<int> = async {
            let! result = Internal.deleteTimeseriesResult name this.Fetch this.Ctx
            match result with
            | Ok response ->
                return response.StatusCode
            | Error error ->
               return raise (Error.error2Exception error)
        }

        worker () |> Async.StartAsTask

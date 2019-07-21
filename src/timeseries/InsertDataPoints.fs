namespace Fusion

open System.IO
open System.Net.Http
open System.Runtime.CompilerServices
open System.Threading.Tasks

open FSharp.Control.Tasks.V2
open Thoth.Json.Net

open Fusion
open Fusion.Api
open Fusion.Common
open Fusion.Timeseries
open Com.Cognite.V1.Timeseries.Proto

[<RequireQualifiedAccess>]
module InsertDataPoints =
    [<Literal>]
    let Url = "/timeseries/data"

    type DataPoints = {
        DataPoints: DataPointSeq
        Identity: Identity
    } with
        member this.ToProto : DataPointInsertionItem =
            let dpItem = DataPointInsertionItem ()
            match this.Identity with
            | CaseId id -> dpItem.Id <- id
            | CaseExternalId id -> dpItem.ExternalId <- id
            match this.DataPoints with
            | Numeric dps ->
                dpItem.NumericDatapoints <- NumericDatapoints ()
                dps |> Seq.map(fun dp ->
                    let pdp = NumericDatapoint ()
                    pdp.Timestamp <- dp.TimeStamp
                    pdp.Value <- dp.Value 
                    pdp
                ) |> dpItem.NumericDatapoints.Datapoints.AddRange
            | String dps ->
                dpItem.StringDatapoints <- StringDatapoints ()
                dps |> Seq.map(fun dp ->
                    let pdp = StringDatapoint ()
                    pdp.Timestamp <- dp.TimeStamp
                    pdp.Value <- dp.Value
                    pdp
                ) |> dpItem.StringDatapoints.Datapoints.AddRange 
            dpItem

    let dataPointsToProtobuf (items: DataPoints seq) : DataPointInsertionRequest =
        let request = DataPointInsertionRequest ()
        items
            |> Seq.map(fun item -> item.ToProto)
            |> request.Items.AddRange
        request

    let insertDataPoints (items: DataPointInsertionRequest) (fetch: HttpHandler<HttpResponseMessage, Stream, unit>) =

        POST
        >=> setVersion V10
        >=> setContent (Content.Protobuf items)
        >=> setResource Url
        >=> fetch
        >=> dispose

[<AutoOpen>]
module InsertDataPointsApi =
    /// **Description**
    ///
    /// Inserts a list of data points to a time series. If a data point is
    /// posted to a timestamp that is already in the series, the existing
    /// data point for that timestamp will be overwritten.
    ///
    /// **Parameters**
    ///   * `name` - The name of the timeseries to insert data points into.
    ///   * `items` - The list of data points to insert.
    ///   * `ctx` - The request HTTP context to use.
    ///
    /// **Output Type**
    ///   * `Async<Result<HttpResponse,ResponseError>>`
    ///
    let insertDataPoints (items: InsertDataPoints.DataPoints list) (next: NextHandler<unit, unit>) =
        InsertDataPoints.insertDataPoints (InsertDataPoints.dataPointsToProtobuf items) fetch next

    let insertDataPointsAsync (items: seq<InsertDataPoints.DataPoints>) =
        InsertDataPoints.insertDataPoints (InsertDataPoints.dataPointsToProtobuf items) fetch Async.single
    
    let insertDataPointsAsyncProto (items: DataPointInsertionRequest) =
        InsertDataPoints.insertDataPoints items fetch Async.single

[<Extension>]
type InsertDataExtensions =
    /// <summary>
    /// Insert data into named time series.
    /// </summary>
    /// <param name="name">The name of the timeseries to insert data into.</param>
    /// <param name="items">The list of data points to insert.</param>
    /// <returns>True if successful.</returns>
    [<Extension>]
    static member InsertDataAsync (this: Client) (items: DataPointInsertionRequest) : Task =
        task {
            let! ctx = insertDataPointsAsyncProto items this.Ctx
            match ctx.Result with
            | Ok _ -> return ()
            | Error error ->
               let err = error2Exception error
               return raise err
        } :> Task



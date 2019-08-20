namespace CogniteSdk.DataPoints

open System.IO
open System.Net.Http
open System.Runtime.CompilerServices
open System.Runtime.InteropServices
open System.Threading.Tasks
open System.Threading

open Com.Cognite.V1.Timeseries.Proto
open Oryx

open CogniteSdk
open CogniteSdk.TimeSeries


[<RequireQualifiedAccess>]
module Insert =
    [<Literal>]
    let Url = "/timeseries/data"

    type DataPoints = {
        /// List of either numeric or string datapoints to be inserted
        DataPoints: DataPointSeq
        /// Id of the timeseries to insert into
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

    let insertCore (items: DataPointInsertionRequest) (fetch: HttpHandler<HttpResponseMessage, Stream, unit>) =
        POST
        >=> setVersion V10
        >=> setContent (Content.Protobuf items)
        >=> setResource Url
        >=> fetch
        >=> dispose

    /// <summary>
    /// Insert data into one or more timeseries.
    /// </summary>
    /// <param name="items">The list of datapoint insertion requests.</param>
    /// <param name="next">Async handler to use.</param>
    let insert (items: DataPoints list) (next: NextHandler<unit, unit>) =
        insertCore (dataPointsToProtobuf items) fetch next

    /// <summary>
    /// Insert data into one or more timeseries.
    /// </summary>
    /// <param name="items">The list of datapoint insertion requests.</param>
    let insertAsync (items: seq<DataPoints>) =
        insertCore (dataPointsToProtobuf items) fetch Async.single

    /// <summary>
    /// Insert data into one or more timeseries.
    /// </summary>
    /// <param name="items">The list of datapoint insertion requests as c# protobuf objects.</param>
    let insertAsyncProto (items: DataPointInsertionRequest) =
         insertCore items fetch Async.single

[<Extension>]
type InsertDataPointsClientExtensions =
    /// <summary>
    /// Insert data into one or more timeseries.
    /// </summary>
    /// <param name="items">The list of datapoint insertion requests.</param>
    [<Extension>]
    static member InsertAsync (this: TimeSeries.DataPointsClientExtension, items: DataPointInsertionRequest, [<Optional>] token: CancellationToken) : Task =
        async {
            let! ctx = Insert.insertAsyncProto items this.Ctx
            match ctx.Result with
            | Ok _ -> return ()
            | Error error ->
               let err = error2Exception error
               return raise err
        } |> fun op -> Async.StartAsTask(op, cancellationToken = token):> Task

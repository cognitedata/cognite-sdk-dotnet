// Copyright 2019 Cognite AS
// SPDX-License-Identifier: Apache-2.0

namespace CogniteSdk.DataPoints

open System.Net.Http
open System.Runtime.CompilerServices
open System.Runtime.InteropServices
open System.Threading.Tasks
open System.Threading

open Com.Cognite.V1.Timeseries.Proto
open FSharp.Control.Tasks.V2.ContextInsensitive
open Oryx

open CogniteSdk


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
            | DataPointSeq.Numeric dps ->
                dpItem.NumericDatapoints <- NumericDatapoints ()
                dps |> Seq.map(fun dp ->
                    let pdp = NumericDatapoint ()
                    pdp.Timestamp <- dp.TimeStamp
                    pdp.Value <- dp.Value
                    pdp
                ) |> dpItem.NumericDatapoints.Datapoints.AddRange
            | DataPointSeq.String dps ->
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

    let insertCore (items: DataPointInsertionRequest) (fetch: HttpHandler<HttpResponseMessage, HttpResponseMessage>) =
        POST
        >=> setVersion V10
        >=> setContent (Content.Protobuf items)
        >=> setResource Url
        >=> fetch
        >=> withError decodeError

    /// <summary>
    /// Insert data into one or more timeseries.
    /// </summary>
    /// <param name="items">The list of datapoint insertion requests.</param>
    /// <param name="next">Async handler to use.</param>
    let insert (items: DataPoints list) (next: NextFunc<HttpResponseMessage, HttpResponseMessage>) =
        insertCore (dataPointsToProtobuf items) fetch next

    /// <summary>
    /// Insert data into one or more timeseries.
    /// </summary>
    /// <param name="items">The list of datapoint insertion requests.</param>
    let insertAsync (items: seq<DataPoints>) =
        insertCore (dataPointsToProtobuf items) fetch finishEarly

    /// <summary>
    /// Insert data into one or more timeseries.
    /// </summary>
    /// <param name="items">The list of datapoint insertion requests as c# protobuf objects.</param>
    let insertAsyncProto (items: DataPointInsertionRequest) =
         insertCore items fetch finishEarly

[<Extension>]
type InsertDataPointsClientExtensions =
    /// <summary>
    /// Insert data into one or more timeseries.
    /// </summary>
    /// <param name="items">The list of datapoint insertion requests.</param>
    [<Extension>]
    static member InsertAsync (this: DataPoints.ClientExtension, items: DataPointInsertionRequest, [<Optional>] token: CancellationToken) : Task =
        task {
            let ctx = this.Ctx |> Context.setCancellationToken token
            let! result = Insert.insertAsyncProto items ctx
            match result with
            | Ok _ -> return ()
            | Error error -> return raiseError error
        } :> Task

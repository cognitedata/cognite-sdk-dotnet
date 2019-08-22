// Copyright 2019 Cognite AS
// SPDX-License-Identifier: Apache-2.0

namespace CogniteSdk.DataPoints

open System.IO
open System.Net.Http
open System.Runtime.CompilerServices
open System.Runtime.InteropServices
open System.Threading
open System.Threading.Tasks

open Oryx
open Thoth.Json.Net
open CogniteSdk


[<CLIMutable>]
type DataPointsDelete = {
    InclusiveBegin : int64
    ExclusiveEnd : int64
    Id : Identity
}

[<RequireQualifiedAccess>]
module Delete =
    [<Literal>]
    let Url = "/timeseries/data/delete"

    type DeleteRequestDto = {
        /// Inclusive start time to delete from
        InclusiveBegin : int64
        /// Optional exclusive end time to delete to
        ExclusiveEnd : int64 option
        /// Id of timeseries to delete from
        Id : Identity
    } with
        static member FromDelete (item : DataPointsDelete) : DeleteRequestDto =
            {
                InclusiveBegin = item.InclusiveBegin
                ExclusiveEnd = if item.ExclusiveEnd = 0L then None else Some item.ExclusiveEnd
                Id = item.Id
            }
        member this.Encoder =
            Encode.object [
                yield "inclusiveBegin", Encode.int53 this.InclusiveBegin
                if this.ExclusiveEnd.IsSome then yield "exclusiveEnd", Encode.int53 this.ExclusiveEnd.Value
                match this.Id with
                | CaseId id -> yield "id", Encode.int53 id
                | CaseExternalId id -> yield "externalId", Encode.string id
            ]
    type DeleteRequest = {
        Items : DeleteRequestDto seq
    } with
        member this.Encoder =
            Encode.object [
                yield "items", this.Items |> Seq.map (fun it -> it.Encoder) |> Encode.seq
            ]

    let deleteCore (items: DeleteRequestDto seq) (fetch: HttpHandler<HttpResponseMessage, Stream, unit>) =
        let request : DeleteRequest = { Items = items }

        POST
        >=> setVersion V10
        >=> setContent (Content.JsonValue request.Encoder)
        >=> setResource Url
        >=> fetch
        >=> dispose

    /// <summary>
    /// Delete datapoints from a given start time to an optional end time for multiple timeseries.
    /// </summary>
    /// <param name="items">List of delete requests.</param>
    /// <param name="next">Async handler to use.</param>
    let delete (items: DeleteRequestDto seq) (next: NextFunc<unit, unit>) =
        deleteCore items fetch next

    /// <summary>
    /// Delete datapoints from a given start time to an optional end time for multiple timeseries.
    /// </summary>
    /// <param name = "items">List of delete requests.</param>
    let deleteAsync (items: DeleteRequestDto seq) =
        deleteCore items fetch Async.single

[<Extension>]
type DeleteDataPointsClientExtensions =
    /// <summary>
    /// Delete datapoints from a given start time to an optional end time for multiple timeseries.
    /// </summary>
    /// <param name = "items">List of delete requests.</param>
    [<Extension>]
    static member DeleteAsync (this: ClientExtension, items: DataPointsDelete seq, [<Optional>] token: CancellationToken) : Task =
        async {
            let items' = items |> Seq.map Delete.DeleteRequestDto.FromDelete
            let! ctx = Delete.deleteAsync items' this.Ctx
            match ctx.Result with
            | Ok _ -> return ()
            | Error error ->
                let err = error2Exception error
                return raise err
        } |> fun op -> Async.StartAsTask(op, cancellationToken = token) :> Task

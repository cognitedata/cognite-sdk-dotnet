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

[<RequireQualifiedAccess>]
module DeleteDataPoints =
    [<Literal>]
    let Url = "/timeseries/data/delete"

    [<CLIMutable>]
    type DeleteRequestPoco = {
        InclusiveBegin : int64
        ExclusiveEnd : int64
        Id : Identity
    }

    type DeleteRequestDto = {
        /// Inclusive start time to delete from
        InclusiveBegin : int64
        /// Optional exclusive end time to delete to
        ExclusiveEnd : int64 option
        /// Id of timeseries to delete from
        Id : Identity
    } with
        static member FromPoco (item : DeleteRequestPoco) : DeleteRequestDto =
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
    
    let deleteDataPoints (items: DeleteRequestDto seq) (fetch: HttpHandler<HttpResponseMessage, Stream, unit>) =
        let request : DeleteRequest = { Items = items }
        
        POST
        >=> setVersion V10
        >=> setContent (Content.JsonValue request.Encoder)
        >=> setResource Url
        >=> fetch
        >=> dispose

[<AutoOpen>]
module DeleteDataPointsApi =
    
    /// <summary>
    /// Delete datapoints from a given start time to an optional end time for multiple timeseries.
    /// </summary>
    /// <param name="items">List of delete requests.</param>
    /// <param name="next">Async handler to use.</param>
    let deleteDataPoints (items: DeleteDataPoints.DeleteRequestDto seq) (next: NextHandler<unit, unit>) =
        DeleteDataPoints.deleteDataPoints items fetch next
    
    /// <summary>
    /// Delete datapoints from a given start time to an optional end time for multiple timeseries.
    /// </summary>
    /// <param name = "items">List of delete requests.</param>
    let deleteDataPointsAsync (items: DeleteDataPoints.DeleteRequestDto seq) =
        DeleteDataPoints.deleteDataPoints items fetch Async.single

[<Extension>]
type DeleteDataPointsExtensions =
    /// <summary>
    /// Delete datapoints from a given start time to an optional end time for multiple timeseries.
    /// </summary>
    /// <param name = "items">List of delete requests.</param>
    [<Extension>]
    static member DeleteDataPointsAsync (this: Client) (items: DeleteDataPoints.DeleteRequestPoco seq) : Task =
        task {
            let items' = items |> Seq.map DeleteDataPoints.DeleteRequestDto.FromPoco
            let! ctx = deleteDataPointsAsync items' this.Ctx
            match ctx.Result with
            | Ok _ -> return ()
            | Error error ->
                let err = error2Exception error
                return raise err
        } :> Task

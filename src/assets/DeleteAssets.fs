// Copyright 2019 Cognite AS
// SPDX-License-Identifier: Apache-2.0

namespace CogniteSdk.Assets

open System.IO
open System.Net.Http
open System.Runtime.CompilerServices
open System.Runtime.InteropServices
open System.Threading
open System.Threading.Tasks

open FSharp.Control.Tasks.ContextInsensitive
open Oryx
open Thoth.Json.Net

open CogniteSdk

[<RequireQualifiedAccess>]
module Delete =
    [<Literal>]
    let Url = "/assets/delete"

    type AssetsDeleteRequest = {
        Items: Identity seq
        Recursive: bool
    } with
        member this.Encoder =
            Encode.object [
                yield "items", this.Items |> Seq.map (fun item -> item.Encoder) |> Encode.seq
                yield "recursive", Encode.bool this.Recursive
            ]

    let deleteCore (assets: Identity seq, recursive: bool) (fetch: HttpHandler<HttpResponseMessage, HttpResponseMessage, 'a>) =
        let decodeResponse = Decode.decodeError
        let request : AssetsDeleteRequest = {
            Items = assets
            Recursive = recursive
        }

        POST
        >=> setVersion V10
        >=> setContent (Content.JsonValue request.Encoder)
        >=> setResource Url
        >=> fetch
        >=> decodeResponse

    /// <summary>
    /// Delete multiple assets in the same project, along with all their descendants in the asset hierarchy if recursive is true.
    /// </summary>
    /// <param name="assets">The list of assets to delete.</param>
    /// <param name="recursive">If true, delete all children recursively.</param>
    /// <param name="next">Async handler to use</param>
    let delete (assets: Identity seq, recursive: bool) (next: NextFunc<HttpResponseMessage,'a>) =
        deleteCore (assets, recursive) fetch next

    /// <summary>
    /// Delete multiple assets in the same project, along with all their descendants in the asset hierarchy if recursive is true.
    /// </summary>
    /// <param name="assets">The list of assets to delete.</param>
    /// <param name="recursive">If true, delete all children recursively.</param>
    let deleteAsync<'a> (assets: Identity seq, recursive: bool) : HttpContext -> Task<Context<HttpResponseMessage>> =
        deleteCore (assets, recursive) fetch Task.FromResult


[<Extension>]
type DeleteAssetsExtensions =
    /// <summary>
    /// Delete multiple assets in the same project, along with all their descendants in the asset hierarchy if recursive is true.
    /// </summary>
    /// <param name="assets">The list of assets to delete.</param>
    /// <param name="recursive">If true, delete all children recursively.</param>
    [<Extension>]
    static member DeleteAsync(this: ClientExtension, ids: Identity seq, recursive: bool, [<Optional>] token: CancellationToken) : Task =
        task {
            let ctx = this.Ctx |> Context.setCancellationToken token
            let! ctx' = Delete.deleteAsync (ids, recursive) ctx
            match ctx'.Result with
            | Ok _ -> return ()
            | Error error ->
                return raise (error.ToException ())
        } :> _

    /// <summary>
    /// Delete multiple assets in the same project, along with all their descendants in the asset hierarchy if recursive is true.
    /// </summary>
    /// <param name="assets">The list of assets to delete.</param>
    /// <param name="recursive">If true, delete all children recursively.</param>
    [<Extension>]
    static member DeleteAsync(this: ClientExtension, ids: int64 seq, recursive: bool, [<Optional>] token: CancellationToken) : Task =
        this.DeleteAsync(ids |> Seq.map Identity.Id, recursive, token)

    /// <summary>
    /// Delete multiple assets in the same project, along with all their descendants in the asset hierarchy if recursive is true.
    /// </summary>
    /// <param name="assets">The list of assets to delete.</param>
    /// <param name="recursive">If true, delete all children recursively.</param>
    [<Extension>]
    static member DeleteAsync(this: ClientExtension, ids: string seq, recursive: bool, [<Optional>] token: CancellationToken) : Task =
        this.DeleteAsync(ids |> Seq.map Identity.ExternalId, recursive, token)

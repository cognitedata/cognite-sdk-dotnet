// Copyright 2019 Cognite AS
// SPDX-License-Identifier: Apache-2.0

namespace CogniteSdk.Assets

open System.IO
open System.Net.Http
open System.Runtime.CompilerServices
open System.Runtime.InteropServices
open System.Threading
open System.Threading.Tasks

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

    let deleteCore (assets: Identity seq, recursive: bool) (fetch: HttpHandler<HttpResponseMessage, Stream, 'a>) =
        let request : AssetsDeleteRequest = {
            Items = assets
            Recursive = recursive
        }

        POST
        >=> setVersion V10
        >=> setContent (Content.JsonValue request.Encoder)
        >=> setResource Url
        >=> fetch
        >=> dispose

    /// <summary>
    /// Delete multiple assets in the same project, along with all their descendants in the asset hierarchy if recursive is true.
    /// </summary>
    /// <param name="assets">The list of assets to delete.</param>
    /// <param name="recursive">If true, delete all children recursively.</param>
    /// <param name="next">Async handler to use</param>
    let delete (assets: Identity seq, recursive: bool) (next: NextHandler<unit,'a>) =
        deleteCore (assets, recursive) fetch next

    /// <summary>
    /// Delete multiple assets in the same project, along with all their descendants in the asset hierarchy if recursive is true.
    /// </summary>
    /// <param name="assets">The list of assets to delete.</param>
    /// <param name="recursive">If true, delete all children recursively.</param>
    let deleteAsync<'a> (assets: Identity seq, recursive: bool) : HttpContext -> Async<Context<unit>> =
        deleteCore (assets, recursive) fetch Async.single


[<Extension>]
type DeleteAssetsExtensions =
    /// <summary>
    /// Delete multiple assets in the same project, along with all their descendants in the asset hierarchy if recursive is true.
    /// </summary>
    /// <param name="assets">The list of assets to delete.</param>
    /// <param name="recursive">If true, delete all children recursively.</param>
    [<Extension>]
    static member DeleteAsync(this: ClientExtension, ids: Identity seq, recursive: bool, [<Optional>] token: CancellationToken) : Task =
        async {
            let! ctx = Delete.deleteAsync (ids, recursive) this.Ctx
            match ctx.Result with
            | Ok _ -> return ()
            | Error error ->
                let err = error2Exception error
                return raise err
        } |> fun op -> Async.StartAsTask(op, cancellationToken = token) :> _


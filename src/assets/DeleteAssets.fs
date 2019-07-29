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
module DeleteAssets =
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

    let deleteAssets (assets: Identity seq, recursive: bool) (fetch: HttpHandler<HttpResponseMessage, Stream, 'a>) =
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

[<AutoOpen>]
module DeleteAssetsApi =
    /// <summary>
    /// Delete multiple assets in the same project, along with all their descendants in the asset hierarchy if recursive is true.
    /// </summary>
    /// <param name="assets">The list of assets to delete.</param>
    /// <param name="recursive">If true, delete all children recursively.</param>
    /// <param name="next">Async handler to use</param>
    let deleteAssets (assets: Identity seq, recursive: bool) (next: NextHandler<unit,'a>) =
        DeleteAssets.deleteAssets (assets, recursive) fetch next
    
    /// <summary>
    /// Delete multiple assets in the same project, along with all their descendants in the asset hierarchy if recursive is true.
    /// </summary>
    /// <param name="assets">The list of assets to delete.</param>
    /// <param name="recursive">If true, delete all children recursively.</param>
    let deleteAssetsAsync<'a> (assets: Identity seq, recursive: bool) : HttpContext -> Async<Context<unit>> =
        DeleteAssets.deleteAssets (assets, recursive) fetch Async.single

[<Extension>]
type DeleteAssetsExtensions =
    /// <summary>
    /// Delete multiple assets in the same project, along with all their descendants in the asset hierarchy if recursive is true.
    /// </summary>
    /// <param name="assets">The list of assets to delete.</param>
    /// <param name="recursive">If true, delete all children recursively.</param>
    [<Extension>]
    static member DeleteAssetsAsync(this: Client, ids: Identity seq, recursive: bool) : Task =
        task {
            let! ctx = deleteAssetsAsync (ids, recursive) this.Ctx
            match ctx.Result with
            | Ok _ -> return ()
            | Error error ->
                let err = error2Exception error
                return raise err
        } :> Task


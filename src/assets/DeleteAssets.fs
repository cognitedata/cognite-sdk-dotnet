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
                yield "items", Encode.list [
                    for id in this.Items do
                        yield Encode.object [
                            match id with
                            | CaseId id -> yield "id", Encode.int53 id
                            | CaseExternalId id -> yield "externalId", Encode.string id
                        ]
                    ]
                yield "recursive", Encode.bool(this.Recursive)
                ]

    let deleteAssets (assets: Identity seq, recursive: bool) (fetch: HttpHandler<HttpResponseMessage, Stream, 'a>) =
        let request : AssetsDeleteRequest = {
            Items = assets
            Recursive = recursive
        }

        POST
        >=> setVersion V10
        >=> setBody request.Encoder
        >=> setResource Url
        >=> fetch
        >=> dispose

[<AutoOpen>]
module DeleteAssetsApi =
    /// **Description**
    ///
    /// Deletes multiple assets in the same project, along with all their descendants in the asset hierarchy.
    ///
    /// **Parameters**
    ///   * `assets` - The list of asset ids to delete.
    ///   * `ctx` - The request HTTP context to use.
    ///
    /// **Output Type**
    ///   * `Async<Result<HttpResponse,ResponseError>>`
    ///
    let deleteAssets (assets: Identity seq, recursive: bool) (next: NextHandler<bool,'a>) =
        DeleteAssets.deleteAssets (assets, recursive) fetch next

    let deleteAssetsAsync<'a> (assets: Identity seq, recursive: bool) : HttpContext -> Async<Context<bool>> =
        DeleteAssets.deleteAssets (assets, recursive) fetch Async.single

[<Extension>]
type DeleteAssetsExtensions =
      /// <summary>
    /// Delete assets.
    /// </summary>
    /// <param name="assets">The list of assets to delete.</param>
    /// <returns>HttpResponse with status code.</returns>
    [<Extension>]
    static member DeleteAssetsAsync(this: Client, assetIds: int64 seq, recursive: bool) : Task<bool> =
        task {
            let ids = Seq.map Identity.Id assetIds
            let! ctx = deleteAssetsAsync (ids, recursive) this.Ctx
            match ctx.Result with
            | Ok response ->
                return true
            | Error error ->
                let! err = error2Exception error
                return raise err
        }


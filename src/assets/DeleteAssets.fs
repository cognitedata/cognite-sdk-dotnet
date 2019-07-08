namespace Cognite.Sdk

open System
open System.Net.Http
open System.Runtime.CompilerServices
open System.Threading.Tasks

open FSharp.Control.Tasks.V2
open Thoth.Json.Net

open Cognite.Sdk
open Cognite.Sdk.Api
open Cognite.Sdk.Common


[<RequireQualifiedAccess>]
module DeleteAssets =
    [<Literal>]
    let Url = "/assets/delete"

    type AssetsDeleteRequest = {
        Items: Identity seq
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
                ]

    let deleteAssets (assets: Identity seq) (fetch: HttpHandler<HttpResponseMessage, string, 'a>) =
        let request : AssetsDeleteRequest = { Items = assets }
        let body = Encode.stringify  request.Encoder

        POST
        >=> setVersion V10
        >=> setBody body
        >=> setResource Url
        >=> fetch

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
    let deleteAssets (assets: Identity seq) (next: NextHandler<string,'a>) =
        DeleteAssets.deleteAssets assets fetch next

    let deleteAssetsAsync<'a> (assets: Identity seq) : HttpContext -> Async<Context<string>> =
        DeleteAssets.deleteAssets assets fetch Async.single

[<Extension>]
type DeleteAssetsExtensions =
      /// <summary>
    /// Delete assets.
    /// </summary>
    /// <param name="assets">The list of assets to delete.</param>
    /// <returns>HttpResponse with status code.</returns>
    [<Extension>]
    static member DeleteAssetsAsync(this: Client, assetIds: int64 seq) : Task<bool> =
        task {
            let ids = Seq.map Identity.Id assetIds
            let! ctx = deleteAssetsAsync ids this.Ctx
            match ctx.Result with
            | Ok response ->
                return true
            | Error error ->
               return raise (Error.error2Exception error)
        }


namespace Cognite.Sdk

open System.Net.Http
open System.Runtime.CompilerServices
open System.Threading.Tasks

open FSharp.Control.Tasks.V2
open Thoth.Json.Net

open Cognite.Sdk
open Cognite.Sdk.Common
open Cognite.Sdk.Api
open Cognite.Sdk.Assets

[<RequireQualifiedAccess>]
module GetAssetsByIds =
    [<Literal>]
    let Url = "/assets"
    type AssetRequest = {
        Items: Identity seq
    } with
        member this.Encoder  =
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


    type AssetResponse = {
        Items: AssetReadDto seq
    } with
         static member Decoder : Decoder<AssetResponse> =
            Decode.object (fun get -> {
                Items = get.Required.Field "items" (Decode.list AssetReadDto.Decoder |> Decode.map seq)
            })

    let getAssetsByIds (ids: Identity seq) (fetch: HttpHandler<HttpResponseMessage, string, 'a>) =
        let decoder = decodeResponse AssetResponse.Decoder (fun response -> response.Items)
        let request : AssetRequest = { Items = ids }
        let body = Encode.stringify request.Encoder
        let url = Url + "byids"

        POST
        >=> setVersion V10
        >=> setBody body
        >=> setResource Url
        >=> fetch
        >=> decoder


[<AutoOpen>]
module GetAssetsByIdsApi =
    /// **Description**
    ///
    /// Retrieve a list of all assets in the given project. The list is sorted alphabetically by name. This operation
    /// supports pagination.
    ///
    /// You can retrieve a subset of assets by supplying additional fields; Only assets satisfying all criteria will be
    /// returned. Names and descriptions are fuzzy searched using edit distance. The fuzziness parameter controls the
    /// maximum edit distance when considering matches for the name and description fields.
    ///
    /// **Parameters**
    ///   * `args` - list of parameters for getting assets.
    ///   * `ctx` - The request HTTP context to use.
    ///
    /// **Output Type**
    ///   * `Async<Result<Response,exn>>`
    ///
    let getAssetsByIds (ids: Identity seq) (next: NextHandler<AssetReadDto seq,'a>) : HttpContext -> Async<Context<'a>> =
        GetAssetsByIds.getAssetsByIds ids fetch next

    let getAssetsByIdsAsync (ids: Identity seq) =
        GetAssetsByIds.getAssetsByIds ids fetch Async.single


[<Extension>]
type GetAssetsByIdsExtensions =
     /// <summary>
    /// Retrieves information about multiple assets in the same project.
    /// A maximum of 1000 assets IDs may be listed per request and all
    /// of them must be unique.
    /// </summary>
    /// <param name="assetId">The id of the asset to get.</param>
    /// <returns>Asset with the given id.</returns>
    [<Extension>]
    static member GetAssetsByIdsAsync (this: Client, ids: seq<Identity>) : Task<_ seq> =
        task {
            let! ctx = getAssetsByIdsAsync ids this.Ctx
            match ctx.Result with
            | Ok assets ->
                return assets |> Seq.map (fun asset -> asset.ToPoco ())
            | Error error ->
                return raise (Error.error2Exception error)
        }




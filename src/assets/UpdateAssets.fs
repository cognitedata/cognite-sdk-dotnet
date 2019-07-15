namespace Cognite.Sdk

open System
open System.IO
open System.Collections.Generic
open System.Net.Http
open System.Runtime.CompilerServices
open System.Threading.Tasks

open FSharp.Control.Tasks.V2
open Thoth.Json.Net

open Cognite.Sdk
open Cognite.Sdk.Api
open Cognite.Sdk.Common

[<RequireQualifiedAccess>]
module UpdateAssets =
    [<Literal>]
    let Url = "/assets/update"

    /// Asset update parameters
    type Option =
        private
        /// Set the name of the asset. Often referred to as tag.
        | CaseName of string // Name cannot be null
        /// Set or clear the description of asset.
        | CaseDescription of string option
        /// Set or clear custom, application specific metadata. String key ->
        /// String value
        | CaseMetaData of Map<string, string> option
        // Set or clear the source of this asset
        | CaseSource of string option
        /// Set or clear ID of the asset in the source. Only applicable if
        /// source is specified. The combination of source and sourceId must be
        /// unique.
        | CaseExternalId of string option
        | CaseParentId of int64 option
        | CaseParentExternalId of string option

        static member SetName name =
            CaseName name

        static member SetDescription description =
            CaseDescription description

        static member SetMetaData (md : IDictionary<string, string>) =
            md |> Seq.map (|KeyValue|) |> Map.ofSeq |> Some |> CaseMetaData

        static member ClearMetaData () =
            CaseMetaData None

        static member SetSource source =
            Some source |> CaseSource

        static member ClearSource =
            CaseSource None

    let renderUpdateFields (arg: Option) =
        match arg with
        | CaseName name ->
            "name", Encode.object [
                "set", Encode.string name
            ]
        | CaseDescription optDesc ->
            "description", Encode.object [
                match optDesc with
                | Some desc -> yield "set", Encode.string desc
                | None -> yield "setNull", Encode.bool true
            ]
        | CaseMetaData optMeta ->
            "metadata", Encode.object [
                match optMeta with
                | Some meta -> yield "set", Encode.propertyBag meta
                | None -> yield "setNull", Encode.bool true
            ]
        | CaseSource optSource ->
            "source", Encode.object [
                match optSource with
                | Some source -> yield "set", Encode.string source
                | None -> yield "setNull", Encode.bool true
            ]
        | CaseExternalId optExternalId ->
            "externalId", Encode.object [
                match optExternalId with
                | Some externalId -> yield "set", Encode.string externalId
                | None -> yield "setNull", Encode.bool true
            ]
        | CaseParentExternalId optParentExternalId ->
            "externalId", Encode.object [
                match optParentExternalId with
                | Some parentExternalId -> yield "set", Encode.string parentExternalId
                | None -> yield "setNull", Encode.bool true
            ]
        | CaseParentId optParentId ->
            "parentId", Encode.object [
                match optParentId with
                | Some parentId -> yield "set", Encode.int53 parentId
                | None -> yield "setNull", Encode.bool true
            ]


    type AssetUpdateRequest = {
        Id: int64
        Params: Option seq
    } with
        member this.Encoder =
            Encode.object [
                yield ("id", Encode.int53 this.Id)
                for arg in this.Params do
                    yield renderUpdateFields arg
            ]


    type AssetsUpdateRequest = {
        Items: AssetUpdateRequest seq
    } with
        member this.Encoder =
            Encode.object [
                "items", Seq.map (fun (item:AssetUpdateRequest) -> item.Encoder) this.Items |> Encode.seq
            ]

    let updateAssets (args: (int64*Option list) list) (fetch: HttpHandler<HttpResponseMessage, Stream, 'a>) =
        let request : AssetsUpdateRequest = {
            Items = [
                for (assetId, args) in args do
                    yield { Id = assetId; Params = args }
            ]
        }

        let body = Encode.stringify request.Encoder

        POST
        >=> setVersion V10
        >=> setBody body
        >=> setResource Url
        >=> fetch
        >=> dispose

[<AutoOpen>]
module UpdateAssetsApi =
    /// **Description**
    ///
    /// Updates multiple assets within the same project. Updating assets does not replace the existing asset hierarchy.
    /// This operation supports partial updates, meaning that fields omitted from the requests are not changed.
    ///
    /// **Parameters**
    ///   * `args` - parameter of type `(int64 * UpdateArgs seq) seq`
    ///   * `ctx` - The request HTTP context to use.
    ///
    /// **Output Type**
    ///   * `Async<Result<string,exn>>`
    ///
    let updateAssets (args: (int64*(UpdateAssets.Option list)) list) (next: NextHandler<bool,'a>)  : HttpContext -> Async<Context<'a>> =
        UpdateAssets.updateAssets args fetch next

    let updateAssetsAsync (args: (int64*UpdateAssets.Option list) list) : HttpContext -> Async<Context<bool>> =
        UpdateAssets.updateAssets args fetch Async.single

[<Extension>]
type UpdateAssetsExtensions =
    /// <summary>
    /// Update assets.
    /// </summary>
    /// <param name="assets">The list of assets to update.</param>
    /// <returns>True of successful.</returns>
    [<Extension>]
    static member UpdateAssetsAsync (this: Client, assets: ValueTuple<int64, UpdateAssets.Option seq> seq) : Task<bool> =
        task {
            let assets' = assets |> Seq.map (fun struct (id, options) -> (id, options |> List.ofSeq)) |> List.ofSeq
            let! ctx = updateAssetsAsync assets' this.Ctx
            match ctx.Result with
            | Ok response ->
                return true
            | Error error ->
                let! err = error2Exception error
                return raise err
        }

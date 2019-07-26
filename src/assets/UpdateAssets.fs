namespace Fusion

open System
open System.IO
open System.Collections.Generic
open System.Net.Http
open System.Runtime.CompilerServices
open System.Threading.Tasks

open FSharp.Control.Tasks.V2
open Thoth.Json.Net

open Fusion
open Fusion.Api
open Fusion.Common
open Fusion.Assets

[<RequireQualifiedAccess>]
module UpdateAssets =
    [<Literal>]
    let Url = "/assets/update"

    type MetaDataChange = {
        Add : Map<string, string> option
        Remove : string seq
    }

    type MetaDataUpdate =
        | Set of Map<string, string>
        | Change of MetaDataChange

    /// Asset update parameters
    type Option =
        private
        /// Set the name of the asset. Often referred to as tag.
        | CaseName of string // Name cannot be null
        /// Set or clear the description of asset.
        | CaseDescription of string option
        /// Set or clear custom, application specific metadata. String key ->
        /// String value
        | CaseMetaData of MetaDataUpdate option
        // Set or clear the source of this asset
        | CaseSource of string option
        /// Set or clear ID of the asset in the source. Only applicable if
        /// source is specified. The combination of source and sourceId must be
        /// unique.
        | CaseExternalId of string option

        static member SetName name =
            CaseName name

        static member SetDescription description =
            CaseDescription description

        static member SetMetaData (md : IDictionary<string, string>) =
            md |> Seq.map (|KeyValue|) |> Map.ofSeq |> Set |> Some |> CaseMetaData

        static member SetMetaData (md : Map<string, string>) =
            md |> Set |> Some |> CaseMetaData

        static member ClearMetaData () =
            CaseMetaData None
        
        static member ChangeMetaData (add: IDictionary<string, string>, remove: string seq) =
            {
                Add =
                    if isNull add then
                        None
                    else
                        add |> Seq.map (|KeyValue|) |> Map.ofSeq |> Some
                Remove = if isNull remove then Seq.empty else remove
            } |> Change |> Some |> CaseMetaData

        static member ChangeMetaData (add: Map<string, string> option, remove: string seq) =
            {
                Add = add
                Remove = remove
            } |> Change |> Some |> CaseMetaData

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
            match optMeta with
            | Some meta ->
                match meta with
                | Set data ->
                    "metadata", Encode.object [
                        yield "set", Encode.propertyBag data
                    ]
                | Change data ->
                    "metadata", Encode.object [
                        if data.Add.IsSome then yield "add", Encode.propertyBag data.Add.Value
                        yield "remove", Encode.seq (Seq.map Encode.string data.Remove)
                    ]
            | None -> "set", Encode.object []
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


    type AssetUpdateRequest = {
        Id: Identity
        Params: Option seq
    } with
        member this.Encoder =
            Encode.object [
                yield 
                    match this.Id with
                    | Identity.CaseId id -> "id", Encode.int53 id
                    | Identity.CaseExternalId id -> "externalId", Encode.string id
                yield "update", Encode.object [
                    yield! this.Params |> Seq.map(renderUpdateFields)
                ]
            ]

    type AssetsUpdateRequest = {
        Items: AssetUpdateRequest seq
    } with
        member this.Encoder =
            Encode.object [
                "items", Seq.map (fun (item:AssetUpdateRequest) -> item.Encoder) this.Items |> Encode.seq
            ]

    type AssetResponse = {
        Items: AssetReadDto seq
    } with
         static member Decoder : Decoder<AssetResponse> =
            Decode.object (fun get -> {
                Items = get.Required.Field "items" (Decode.list AssetReadDto.Decoder |> Decode.map seq)
            })

    let updateAssets (args: (Identity * Option list) list) (fetch: HttpHandler<HttpResponseMessage, Stream, 'a>) =
        let decoder = decodeResponse AssetResponse.Decoder (fun res -> res.Items)
        let request : AssetsUpdateRequest = {
            Items = [
                yield! args |> Seq.map(fun (assetId, args) -> { Id = assetId; Params = args })
            ]
        }

        POST
        >=> setVersion V10
        >=> setBody request.Encoder
        >=> setResource Url
        >=> fetch
        >=> decoder

[<AutoOpen>]
module UpdateAssetsApi =
    /// **Description**
    ///
    /// Updates multiple assets within the same project. Updating assets does not replace the existing asset hierarchy.
    /// This operation supports partial updates, meaning that fields omitted from the requests are not changed.
    ///
    /// **Parameters**
    ///   * `args` - parameter of type `(Identity * UpdateArgs seq) seq`
    ///   * `ctx` - The request HTTP context to use.
    ///
    /// **Output Type**
    ///   * `Async<Result<string,exn>>`
    ///
    let updateAssets (args: (Identity * (UpdateAssets.Option list)) list) (next: NextHandler<AssetReadDto seq,'a>)  : HttpContext -> Async<Context<'a>> =
        UpdateAssets.updateAssets args fetch next

    let updateAssetsAsync (args: (Identity * UpdateAssets.Option list) list) : HttpContext -> Async<Context<AssetReadDto seq>> =
        UpdateAssets.updateAssets args fetch Async.single

[<Extension>]
type UpdateAssetsExtensions =
    /// <summary>
    /// Update assets.
    /// </summary>
    /// <param name="assets">The list of assets to update.</param>
    /// <returns>True of successful.</returns>
    [<Extension>]
    static member UpdateAssetsAsync (this: Client, assets: ValueTuple<Identity, UpdateAssets.Option seq> seq) : Task<AssetReadPoco seq> =
        task {
            let assets' = assets |> Seq.map (fun struct (id, options) -> (id, options |> List.ofSeq)) |> List.ofSeq
            let! ctx = updateAssetsAsync assets' this.Ctx
            match ctx.Result with
            | Ok response ->
                return response |> Seq.map (fun asset -> asset.ToPoco ()) 
            | Error error ->
                let err = error2Exception error
                return raise err
        }

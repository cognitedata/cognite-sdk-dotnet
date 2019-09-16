// Copyright 2019 Cognite AS
// SPDX-License-Identifier: Apache-2.0

namespace CogniteSdk.Assets

open System
open System.IO
open System.Collections.Generic
open System.Net.Http
open System.Runtime.CompilerServices
open System.Runtime.InteropServices
open System.Threading
open System.Threading.Tasks

open Oryx
open Thoth.Json.Net

open CogniteSdk
open FSharp.Control.Tasks.V2.ContextInsensitive


type MetaDataChange = {
    Add : Map<string, string> option
    Remove : string seq
}

type MetaDataUpdate =
    | Set of Map<string, string>
    | Change of MetaDataChange

/// Asset update parameters
type AssetUpdate =
    private
    | CaseName of string // Name cannot be null
    | CaseDescription of string option
    | CaseMetaData of MetaDataUpdate
    | CaseSource of string option
    | CaseExternalId of string option

    /// Set the name of the asset. Often referred to as tag.
    static member SetName name =
        CaseName name
    /// Set or clear the description of asset.
    static member SetDescription description =
        CaseDescription description
    /// Set metadata of asset. This removes any old metadata.
    static member SetMetaData (md : IDictionary<string, string>) =
        md |> Seq.map (|KeyValue|) |> Map.ofSeq |> Set |> CaseMetaData
    /// Set metadata of asset. This removes any old metadata.
    static member SetMetaData (md : Map<string, string>) =
        md |> Set |> CaseMetaData
    /// Change metadata of asset by adding new data as given in `add` and removing keys given in `remove`
    static member ChangeMetaData (add: IDictionary<string, string>, remove: string seq) =
        {
            Add =
                if isNull add then
                    None
                else
                    add |> Seq.map (|KeyValue|) |> Map.ofSeq |> Some
            Remove = if isNull remove then Seq.empty else remove
        } |> Change |> CaseMetaData
    /// Set the source of this asset
    static member SetSource source =
        Some source |> CaseSource
    /// Clear the source of this asset
    static member ClearSource =
        CaseSource None
    /// Set the externalId of asset. Must be unique within the project
    static member SetExternalId id =
        CaseExternalId id
    /// Clear the externalId of asset.
    static member ClearExternalId =
        CaseExternalId None

/// The functional asset update core module
[<RequireQualifiedAccess>]
module Update =
    [<Literal>]
    let Url = "/assets/update"

    let renderUpdateFields (arg: AssetUpdate) =
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
        | CaseMetaData meta ->
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

    type private AssetUpdateRequest = {
        Id: Identity
        Params: AssetUpdate seq
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

    type private AssetsUpdateRequest = {
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

    let updateCore (args: (Identity * AssetUpdate list) list) (fetch: HttpHandler<HttpResponseMessage, 'a>) =
        let decodeResponse = Decode.decodeResponse AssetResponse.Decoder (fun res -> res.Items)
        let request : AssetsUpdateRequest = {
            Items = [
                yield! args |> Seq.map(fun (assetId, args) -> { Id = assetId; Params = args })
            ]
        }
        POST
        >=> setVersion V10
        >=> setContent (Content.JsonValue request.Encoder)
        >=> setResource Url
        >=> fetch
        >=> decodeResponse

    /// <summary>
    /// Update one or more assets. Supports partial updates, meaning that fields omitted from the requests are not changed
    /// </summary>
    /// <param name="assets">The list of assets to update.</param>
    /// <param name="next">Async handler to use.</param>
    /// <returns>List of updated assets.</returns>
    let update (assets: (Identity * (AssetUpdate list)) list) (next: NextFunc<AssetReadDto seq,'a>)  : HttpContext -> Task<Context<'a>> =
        updateCore assets fetch next

    /// <summary>
    /// Update one or more assets. Supports partial updates, meaning that fields omitted from the requests are not changed
    /// </summary>
    /// <param name="assets">The list of assets to update.</param>
    /// <returns>List of updated assets.</returns>
    let updateAsync (assets: (Identity * AssetUpdate list) list) : HttpContext -> Task<Context<AssetReadDto seq>> =
        updateCore assets fetch Task.FromResult

[<Extension>]
type UpdateAssetsClientExtensions =
    /// <summary>
    /// Update one or more assets. Supports partial updates, meaning that fields omitted from the requests are not changed
    /// </summary>
    /// <param name="assets">The list of assets to update.</param>
    /// <returns>List of updated assets.</returns>
    [<Extension>]
    static member UpdateAsync (this: ClientExtension, assets: ValueTuple<Identity, AssetUpdate seq> seq, [<Optional>] token: CancellationToken) : Task<AssetEntity seq> =
        task {
            let ctx = this.Ctx |> Context.setCancellationToken token
            let assets' = assets |> Seq.map (fun struct (id, options) -> (id, options |> List.ofSeq)) |> List.ofSeq
            let! ctx' = Update.updateAsync assets' ctx
            match ctx'.Result with
            | Ok response ->
                return response |> Seq.map (fun asset -> asset.ToAssetEntity ())
            | Error error ->
                return raise (error.ToException ())
        }

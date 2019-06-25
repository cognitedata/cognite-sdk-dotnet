namespace Cognite.Sdk.Api

open System
open System.Collections.Generic
open System.Runtime.InteropServices
open System.Runtime.CompilerServices
open System.Threading.Tasks;

open FSharp.Control.Tasks.V2

open Cognite.Sdk
open Cognite.Sdk.Api
open Cognite.Sdk.Assets
open Cognite.Sdk.Common

[<Extension>]
///Helper methods for creating Assets.
type Asset =
    [<Extension>]
    static member TryGetParentId (this: AssetReadDto, [<Out>] parentId: byref<Int64>) =
        match this.ParentId with
        | Some id ->
            parentId <- id
            true
        | None -> false

    [<Extension>]
    static member HasParentId (this: AssetReadDto) : bool =
        this.ParentId.IsSome

    [<Extension>]
    static member GetParentId (this: AssetReadDto) =
        this.ParentId.Value

    /// Create a new Asset with name and description (non optional).
    /// Optional properties can then be added using Set* methods such as e.g {SetMetaData}.
    [<Extension>]
    static member Create (name: string) : AssetCreateDto =
        {
            ExternalId = None
            Name = name
            Description = None
            MetaData = Map.empty
            Source = None
            ParentId = None

            ParentExternalId = None
        }

    /// Description of asset.
    [<Extension>]
    static member SetDescription (this: AssetCreateDto, description: string) : AssetCreateDto =
        { this with Description = Some description }

    /// Set custom, application specific metadata. String key -> String value.
    [<Extension>]
    static member SetMetaData (this: AssetCreateDto, metaData: Dictionary<string, string>) : AssetCreateDto =
        let map =
            metaData
            |> Seq.map (|KeyValue|)
            |> Map.ofSeq
        { this with MetaData = map }

    /// Set the source of this asset.
    [<Extension>]
    static member SetSource (this: AssetCreateDto, source: string) : AssetCreateDto =
        { this with Source = Some source }

    /// External Id provided by client. Should be unique within the project.
    [<Extension>]
    static member SetExternalId (this: AssetCreateDto, externalId: string) : AssetCreateDto =
        { this with ExternalId = Some externalId }

    /// Javascript friendly internal ID given to the object.
    [<Extension>]
    static member SetParentId (this: AssetCreateDto, parentId: int64) : AssetCreateDto =
        { this with ParentId =  Some parentId  }

    /// External Id provided by client. Should be unique within the project.
    [<Extension>]
    static member SetParentExternalId (this: AssetCreateDto, parentExternalId: string) : AssetCreateDto =
        { this with ParentExternalId =  Some parentExternalId  }



type AssetUpdate private (id : int64, updates : UpdateParams list) =
    let id = id
    let updates = updates

    let updateParams : UpdateParams list = []

    new (id : int64) =
        AssetUpdate (id, [])

    member internal this.Id = id

    member internal this.Updates = updates

    /// Set the name of the asset. Often referred to as tag.
    member this.SetName (name : string) =
        AssetUpdate (id, SetName name :: updates)

    member this.SetDescription (desc : string) =
        /// Set or clear the description of asset.
       Some desc |> SetDescription

    member this.ClearDescription () =
        SetDescription None

    member this.SetMetaData (metaData : Dictionary<string, string>) =
        /// Set or clear custom, application specific metadata. String key -> String value
        metaData
        |> Seq.map (|KeyValue|)
        |> Map.ofSeq
        |> Some
        |> SetMetaData

    member this.SetSource (source : string) =
        // Set or clear the source of this asset
        Some source |> SetSource

    member this.SetParentId (parentId : int64) =
        Some parentId |> SetParentId

    member this.ClearParentId () =
        SetParentId None

    member this.SetParentExternalId (parentExternalId : string) =
        Some parentExternalId |> SetParentExternalId

    member this.ClearParentExternalId () =
        SetParentExternalId None

    member this.SetExternalId (externalId : string) =
        Some externalId |> SetExternalId

    member this.ClearExternalId () =
        SetExternalId None

[<Extension>]
type ClientAssetExtensions =

    /// <summary>
    /// Retrieves information about an asset in a certain project given an asset id.
    /// </summary>
    /// <param name="assetId">The id of the asset to get.</param>
    /// <returns>Asset with the given id.</returns>
    [<Extension>]
    static member GetAssetAsync (this: Client, assetId: int64) : Task<AssetReadDto> =
        task {
            let! result = Internal.getAssetResult assetId fetch this.Ctx
            match result with
            | Ok response ->
                return response
            | Error error ->
                return raise (Error.error2Exception error)
        }

    /// <summary>
    /// Retrieves information about multiple assets in the same project.
    /// A maximum of 1000 assets IDs may be listed per request and all
    /// of them must be unique.
    /// </summary>
    /// <param name="assetId">The id of the asset to get.</param>
    /// <returns>Asset with the given id.</returns>
    [<Extension>]
    static member GetAssetsByIdsAsync(this: Client, assetIds: seq<int64>) : Task<AssetResponse> =
        task {
            let ids = Seq.map Identity.Id assetIds
            let! result = Internal.getAssetsByIdsResult ids fetch this.Ctx
            match result with
            | Ok response ->
                return response
            | Error error ->
                return raise (Error.error2Exception error)
        }

    /// <summary>
    /// Retrieves information about multiple assets in the same project.
    /// A maximum of 1000 assets IDs may be listed per request and all
    /// of them must be unique.
    /// </summary>
    /// <param name="assetId">The id of the asset to get.</param>
    /// <returns>Asset with the given id.</returns>
    [<Extension>]
    static member GetAssetsByIdsAsync (this: Client, assetExternalIds: seq<string>) : Task<AssetResponse> =
        task {
            let ids = Seq.map Identity.ExternalId assetExternalIds
            let! result = Internal.getAssetsByIdsResult ids fetch this.Ctx
            match result with
            | Ok response ->
                return response
            | Error error ->
                return raise (Error.error2Exception error)
        }

    /// <summary>
    /// Create assets.
    /// </summary>
    /// <param name="assets">The assets to create.</param>
    /// <returns>List of created assets.</returns>
    [<Extension>]
    static member CreateAssetsAsync (this: Client, assets: AssetCreateDto seq) : Task<AssetReadDto seq> =
        task {
            let! result = Internal.createAssetsResult assets fetch this.Ctx
            match result with
            | Ok response ->
                return response
            | Error error ->
               return raise (Error.error2Exception error)
        }

    /// <summary>
    /// Delete assets.
    /// </summary>
    /// <param name="assets">The list of assets to delete.</param>
    /// <returns>HttpResponse with status code.</returns>
    [<Extension>]
    static member DeleteAssetsAsync(this: Client, assetIds: int64 seq) : Task<bool> =
        task {
            let ids = Seq.map Identity.Id assetIds
            let! result = Internal.deleteAssetsResult ids fetch this.Ctx
            match result with
            | Ok response ->
                return true
            | Error error ->
               return raise (Error.error2Exception error)
        }

    /// <summary>
    /// Delete assets.
    /// </summary>
    /// <param name="assets">The list of assets to delete.</param>
    /// <returns>HttpResponse with status code.</returns>
    [<Extension>]
    static member DeleteAssetsAsync(this: Client, assetExternalIds: string seq) : Task<bool> =
        task {
            let ids = Seq.map Identity.ExternalId assetExternalIds
            let! result = Internal.deleteAssetsResult ids fetch this.Ctx
            match result with
            | Ok response ->
                return true
            | Error error ->
               return raise (Error.error2Exception error)
        }

    /// <summary>
    /// Update assets.
    /// </summary>
    /// <param name="assets">The list of assets to delete.</param>
    /// <returns>HttpResponse with status code.</returns>
    [<Extension>]
    static member UpdateAssetsAsync (this: Client, assets: AssetUpdate seq) : Task<bool> =
        task {
            let! result = Internal.updateAssetsResult (assets |> Seq.map (fun asset -> asset.Id, asset.Updates) |> List.ofSeq ) fetch this.Ctx
            match result with
            | Ok response ->
                return true
            | Error error ->
               return raise (Error.error2Exception error)
        }

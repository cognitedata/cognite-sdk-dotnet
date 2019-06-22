namespace Cognite.Sdk.Api

open System
open System.Collections.Generic
open System.Runtime.InteropServices
open System.Runtime.CompilerServices
open System.Threading.Tasks;

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


type AssetArgs (args: GetParams list) =
    let args  = args

    member this.ExternalIdPrefix (prefix: string) =
        AssetArgs (ExternalIdPrefix prefix :: args)

    member this.Name (name: string) =
        AssetArgs (Name name :: args)

    member this.Source (source: string) =
        AssetArgs (Source source :: args)

    member this.Root (root: bool) =
        AssetArgs (Root root :: args)

    (*
    member this.MetaData (metaData: Dictionary<string, string>) =
        let map =
            metaData
            |> Seq.map (|KeyValue|)
            |> Map.ofSeq
            |> MetaData
        AssetArgs (map :: args)
    *)

    member this.ParentIds (ids: seq<int64>) =
        AssetArgs (ParentIds ids :: args)

    member this.MinCreatedTime (value: int64) =
        AssetArgs (MinCreatedTime value :: args)
    member this.MaxCreatedTime (value: int64) =
        AssetArgs (MaxCreatedTime value :: args)
    member this.MinLastUpdatedTime (value: int64) =
        AssetArgs (MinLastUpdatedTime value :: args)
    member this.MaxLastUpdatedTime (value: int64) =
        AssetArgs (MaxLastUpdatedTime value :: args)

    member internal this.Args = args

    static member Empty () =
        AssetArgs []

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
    /// Retrieve a list of all assets in the given project. The list is sorted alphabetically by name. This operation
    /// supports pagination.
    ///
    /// You can retrieve a subset of assets by supplying additional fields; Only assets satisfying all criteria will be
    /// returned. Names and descriptions are fuzzy searched using edit distance. The fuzziness parameter controls the
    /// maximum edit distance when considering matches for the name and description fields.
    /// </summary>
    /// <param name="args">The asset argument object containing parameters to get used for the asset query.</param>
    /// <returns>List of assets.</returns>
    [<Extension>]
    static member GetAssetsAsync (this: Client<AssetResponse>, args: AssetArgs) : Task<AssetResponse> =
        let worker () : Async<AssetResponse> = async {
            let! result = Internal.getAssetsResult args.Args this.Fetch this.Ctx
            match result with
            | Ok response ->
                return response
            | Error error ->
                return raise (Error.error2Exception error)
        }

        worker () |> Async.StartAsTask

    /// <summary>
    /// Retrieves information about an asset in a certain project given an asset id.
    /// </summary>
    /// <param name="assetId">The id of the asset to get.</param>
    /// <returns>Asset with the given id.</returns>
    [<Extension>]
    static member GetAssetAsync (this: Client<AssetReadDto>, assetId: int64) : Task<AssetReadDto> =
        let worker () : Async<AssetReadDto> = async {
            let! result = Internal.getAssetResult assetId this.Fetch this.Ctx
            match result with
            | Ok response ->
                return response
            | Error error ->
                return raise (Error.error2Exception error)
        }

        worker () |> Async.StartAsTask

    /// <summary>
    /// Retrieves information about multiple assets in the same project.
    /// A maximum of 1000 assets IDs may be listed per request and all
    /// of them must be unique.
    /// </summary>
    /// <param name="assetId">The id of the asset to get.</param>
    /// <returns>Asset with the given id.</returns>
    [<Extension>]
    static member GetAssetsByIdsAsync(this: Client<AssetResponse>, assetIds: seq<int64>) : Task<AssetResponse> =
        let worker () : Async<AssetResponse> = async {
            let ids = Seq.map Identity.Id assetIds
            let! result = Internal.getAssetsByIdsResult ids this.Fetch this.Ctx
            match result with
            | Ok response ->
                return response
            | Error error ->
                return raise (Error.error2Exception error)
        }

        worker () |> Async.StartAsTask


    /// <summary>
    /// Retrieves information about multiple assets in the same project.
    /// A maximum of 1000 assets IDs may be listed per request and all
    /// of them must be unique.
    /// </summary>
    /// <param name="assetId">The id of the asset to get.</param>
    /// <returns>Asset with the given id.</returns>
    [<Extension>]
    static member GetAssetsByIdsAsync (this: Client<AssetResponse>, assetExternalIds: seq<string>) : Task<AssetResponse> =
        let worker () : Async<AssetResponse> = async {
            let ids = Seq.map Identity.ExternalId assetExternalIds
            let! result = Internal.getAssetsByIdsResult ids this.Fetch this.Ctx
            match result with
            | Ok response ->
                return response
            | Error error ->
                return raise (Error.error2Exception error)
        }

        worker () |> Async.StartAsTask

    /// <summary>
    /// Create assets.
    /// </summary>
    /// <param name="assets">The assets to create.</param>
    /// <returns>List of created assets.</returns>
    [<Extension>]
    static member CreateAssetsAsync (this: Client<AssetReadDto seq>, assets: AssetCreateDto seq) : Task<AssetReadDto seq> =
        let worker () : Async<AssetReadDto seq> = async {
            let! result = Internal.createAssetsResult assets this.Fetch this.Ctx
            match result with
            | Ok response ->
                return response
            | Error error ->
               return raise (Error.error2Exception error)
        }

        worker () |> Async.StartAsTask

    /// <summary>
    /// Delete assets.
    /// </summary>
    /// <param name="assets">The list of assets to delete.</param>
    /// <returns>HttpResponse with status code.</returns>
    [<Extension>]
    static member DeleteAssetsAsync(this: Client<_>, assetIds: int64 seq) : Task<HttpResponse> =
        let worker () : Async<HttpResponse> = async {
            let ids = Seq.map Identity.Id assetIds
            let! result = Internal.deleteAssetsResult ids this.Fetch this.Ctx
            match result with
            | Ok response ->
                return HttpResponse(response.StatusCode, String.Empty)
            | Error error ->
               return raise (Error.error2Exception error)
        }

        worker () |> Async.StartAsTask

    /// <summary>
    /// Delete assets.
    /// </summary>
    /// <param name="assets">The list of assets to delete.</param>
    /// <returns>HttpResponse with status code.</returns>
    [<Extension>]
    static member DeleteAssetsAsync(this: Client<_>, assetExternalIds: string seq) : Task<HttpResponse> =
        let worker () : Async<HttpResponse> = async {
            let ids = Seq.map Identity.ExternalId assetExternalIds
            let! result = Internal.deleteAssetsResult ids this.Fetch this.Ctx
            match result with
            | Ok response ->
                return HttpResponse(response.StatusCode, String.Empty)
            | Error error ->
               return raise (Error.error2Exception error)
        }

        worker () |> Async.StartAsTask

    /// <summary>
    /// Update assets.
    /// </summary>
    /// <param name="assets">The list of assets to delete.</param>
    /// <returns>HttpResponse with status code.</returns>
    [<Extension>]
    static member UpdateAssetsAsync (this: Client<_>, assets: AssetUpdate seq) : Task<HttpResponse> =
        let worker () : Async<HttpResponse> = async {
            let! result = Internal.updateAssetsResult (assets |> Seq.map (fun asset -> asset.Id, asset.Updates) |> List.ofSeq ) (unbox this.Fetch) this.Ctx
            match result with
            | Ok response ->
                return HttpResponse(response.StatusCode, String.Empty)
            | Error error ->
               return raise (Error.error2Exception error)
        }

        worker () |> Async.StartAsTask

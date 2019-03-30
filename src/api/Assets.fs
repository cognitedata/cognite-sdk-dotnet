namespace Cognite.Sdk.Api

open System
open System.Collections.Generic
open System.Runtime.InteropServices
open System.Runtime.CompilerServices
open System.Threading.Tasks;

open Cognite.Sdk
open Cognite.Sdk.Api
open Cognite.Sdk.Assets

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
    static member Create (name: string) (description: string) : AssetCreateDto =
        {
            Name = name
            Description = description
            MetaData = Map.empty
            Source = None
            SourceId = None

            RefId = None
            ParentRef = None
        }

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

    /// The the source ID of the asset. Only applicable if source is specified.
    /// The combination of source and sourceId must be unique.
    [<Extension>]
    static member SetSourceId (this: AssetCreateDto, sourceId: string) : AssetCreateDto =
        { this with SourceId = Some sourceId }

    /// Set the reference ID used only in post request to disambiguate references to duplicate names.
    [<Extension>]
    static member SetRefId (this: AssetCreateDto, refId: string) : AssetCreateDto =
        { this with RefId = Some refId }

    /// Set the reference ID of parent, to disambiguate if multiple nodes have the same name.
    [<Extension>]
    static member SetParentRefId (this: AssetCreateDto, parentRefId: string) : AssetCreateDto =
        { this with ParentRef = ParentRefId parentRefId |> Some }

    [<Extension>]
    static member SetParentName (this: AssetCreateDto, parentName: string) : AssetCreateDto =
        { this with ParentRef = ParentName parentName |> Some }

    /// Set the ID of parent asset in CDP, if any. If parentName or parentRefId are also specified, this will be ignored.
    [<Extension>]
    static member SetParentId (this: AssetCreateDto, parentId: string) : AssetCreateDto =
        { this with ParentRef = ParentId parentId |> Some }

type AssetArgs (args: GetParams list) =
    let args  = args

    member this.Id (id: int64) =
        AssetArgs (Id id :: args)

    member this.Name (name: string) =
        AssetArgs (Name name :: args)

    member this.Description (description: string) =
        AssetArgs (Description description :: args)

    member this.Path (path: string) =
        AssetArgs (Path path :: args)

    member this.MetaData (metaData: Dictionary<string, string>) =
        let map =
            metaData
            |> Seq.map (|KeyValue|)
            |> Map.ofSeq
            |> MetaData
        AssetArgs (map :: args)

    member this.Depth (depth: int) =
        AssetArgs (Depth depth :: args)

    member internal this.Args = args

    static member Empty () =
        AssetArgs []

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
    static member GetAssetsAsync (this: Client) (args: AssetArgs) : Task<AssetReadDto List> =
        let worker () : Async<AssetReadDto List> = async {
            let! result = getAssets this.Ctx args.Args
            match result with
            | Ok response ->
                return ResizeArray<AssetReadDto> response
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
    static member GetAssetAsync (this: Client) (assetId: int64) : Task<AssetReadDto> =
        let worker () : Async<AssetReadDto> = async {
            let! result = getAsset this.Ctx assetId
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
    static member CreateAssetsAsync (this: Client) (assets: ResizeArray<AssetCreateDto>) : Task<AssetReadDto List> =
        let worker () : Async<AssetReadDto List> = async {
            let! result = createAssets this.Ctx (Seq.toList assets)
            match result with
            | Ok response ->
                return ResizeArray<AssetReadDto> response
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
    static member DeleteAssetsAsync (this: Client) (assets: ResizeArray<int64>) : Task<HttpResponse> =
        let worker () : Async<HttpResponse> = async {
            let! result = deleteAssets this.Ctx (Seq.toList assets)
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
    static member UpdateAssetsAsync (this: Client) (assets: ResizeArray<Tuple<int64, ResizeArray<UpdateParams>>>) : Task<HttpResponse> =
        let worker () : Async<HttpResponse> = async {
            let! result = updateAssets this.Ctx (assets |> Seq.map (fun (x, y) -> x, Seq.toList y) |> Seq.toList)
            match result with
            | Ok response ->
                return HttpResponse(response.StatusCode, String.Empty)
            | Error error ->
               return raise (Error.error2Exception error)
        }

        worker () |> Async.StartAsTask

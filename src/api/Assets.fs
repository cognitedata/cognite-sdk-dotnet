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

    /// Create new RequestAsset with all non-optional values.
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

    [<Extension>]
    static member SetMetaData (this: AssetCreateDto, metaData: Dictionary<string, string>) : AssetCreateDto =
        let map =
            metaData
            |> Seq.map (|KeyValue|)
            |> Map.ofSeq
        { this with MetaData = map }

    [<Extension>]
    static member SetSource (this: AssetCreateDto, source: string) : AssetCreateDto =
        { this with Source = Some source }

    [<Extension>]
    static member SetSourceId (this: AssetCreateDto, sourceId: string) : AssetCreateDto =
        { this with SourceId = Some sourceId }

    [<Extension>]
    static member SetRefId (this: AssetCreateDto, refId: string) : AssetCreateDto =
        { this with RefId = Some refId }

    [<Extension>]
    static member SetParentRefId (this: AssetCreateDto, parentRefId: string) : AssetCreateDto =
        { this with ParentRef = ParentRefId parentRefId |> Some }

    [<Extension>]
    static member SetParentName (this: AssetCreateDto, parentName: string) : AssetCreateDto =
        { this with ParentRef = ParentName parentName |> Some }

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
    static member GetAssets (this: Client) (args: AssetArgs) : Task<AssetReadDto List> =
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
    static member GetAsset (this: Client) (assetId: int64) : Task<AssetReadDto> =
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
    static member CreateAssets (this: Client) (assets: ResizeArray<AssetCreateDto>) : Task<AssetReadDto List> =
        let worker () : Async<AssetReadDto List> = async {
            let! result = createAssets this.Ctx (Seq.toList assets)
            match result with
            | Ok response ->
                return ResizeArray<AssetReadDto> response
            | Error error ->
               return raise (Error.error2Exception error)
        }

        worker () |> Async.StartAsTask

    [<Extension>]
    static member DeleteAssets (this: Client) (assets: ResizeArray<int64>) : Task<int> =
        let worker () : Async<int> = async {
            let! result = deleteAssets this.Ctx (Seq.toList assets)
            match result with
            | Ok response ->
                return response.StatusCode
            | Error error ->
               return raise (Error.error2Exception error)
        }

        worker () |> Async.StartAsTask

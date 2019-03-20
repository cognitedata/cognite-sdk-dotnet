namespace Cognite.Sdk.Api

open System
open System.Threading.Tasks;
open System.Runtime.InteropServices
open System.Runtime.CompilerServices
open System.Collections.Generic

open Cognite.Sdk
open Cognite.Sdk.Api
open Cognite.Sdk.Assets

[<Extension>]
type TimeseriesExtension =
    [<Extension>]
    static member TryGetParentId (this: ResponseAssetDto, [<Out>] parentId: byref<Int64>) =
        match this.ParentId with
        | Some id ->
            parentId <- id
            true
        | None -> false

    [<Extension>]
    static member HasParentId (this: ResponseAssetDto) : bool =
        this.ParentId.IsSome

    [<Extension>]
    static member GetParentId (this: ResponseAssetDto) =
        this.ParentId.Value

    /// Create new RequestAsset with all non-optional values.
    [<Extension>]
    static member Create (name: string) (description: string) (createdTime: int64) (lastUpdatedTime: int64) : RequestAssetDto =
        {
            Name = name
            Description = description
            MetaData = Map.empty
            Source = None
            SourceId = None
            CreatedTime = createdTime
            LastUpdatedTime = lastUpdatedTime

            RefId = None
            ParentRef = None
        }

    [<Extension>]
    static member SetMetaData (this: RequestAssetDto, metaData: Dictionary<string, string>) : RequestAssetDto =
        let map =
            metaData
            |> Seq.map (|KeyValue|)
            |> Map.ofSeq
        { this with MetaData = map }

    [<Extension>]
    static member SetSource (this: RequestAssetDto, source: string) : RequestAssetDto =
        { this with Source = Some source }

    [<Extension>]
    static member SetSourceId (this: RequestAssetDto, sourceId: string) : RequestAssetDto =
        { this with SourceId = Some sourceId }

    [<Extension>]
    static member SetRefId (this: RequestAssetDto, refId: string) : RequestAssetDto =
        { this with RefId = Some refId }

    [<Extension>]
    static member SetParentRefId (this: RequestAssetDto, parentRefId: string) : RequestAssetDto =
        { this with ParentRef = ParentRefId parentRefId |> Some }

    [<Extension>]
    static member SetParentName (this: RequestAssetDto, parentName: string) : RequestAssetDto =
        { this with ParentRef = ParentName parentName |> Some }

    [<Extension>]
    static member SetParentId (this: RequestAssetDto, parentId: string) : RequestAssetDto =
        { this with ParentRef = ParentId parentId |> Some }

type TimeseriesArgs (args: GetParams list) =
    let args  = args

    member this.Id(id: int64) =
        AssetArgs (Id id :: args)

    member this.Name(name: string) =
        AssetArgs (Name name :: args)

    member this.Description(description: string) =
        AssetArgs (Description description :: args)

    member this.Path(path: string) =
        AssetArgs (Path path :: args)

    member this.MetaData(metaData: Dictionary<string, string>) =
        let map =
            metaData
            |> Seq.map (|KeyValue|)
            |> Map.ofSeq
            |> MetaData
        AssetArgs (map :: args)

    member this.Depth(depth: int) =
        AssetArgs (Depth depth :: args)

    member internal this.Args = args

    static member Empty() =
        AssetArgs []

[<Extension>]
type ClientTimeseriesExtensions =
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
    static member GetAssets2 (this: Client) (args: AssetArgs) : Task<ResponseAssetDto List> =
        let worker () : Async<ResponseAssetDto List> = async {
            let! result = getAssets this.Ctx args.Args
            match result with
            | Ok response ->
                return ResizeArray<ResponseAssetDto> response
            | Error error ->
                return raise (Request.error2Exception error)
        }

        worker () |> Async.StartAsTask


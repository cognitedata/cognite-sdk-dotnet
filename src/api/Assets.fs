namespace Cognite.Sdk.Api

open System
open System.Threading.Tasks;
open System.Runtime.InteropServices
open System.Runtime.CompilerServices
open System.Collections.Generic

open Cognite.Sdk.Api
open Cognite.Sdk.Assets


[<Extension>]
type AssetExtension =
    [<Extension>]
    static member TryGetParentId (this: Response.Asset, [<Out>] parentId: byref<Int64>) =
        match this.ParentId with
        | Some id ->
            parentId <- id
            true
        | None -> false

    [<Extension>]
    static member HasParentId (this: Response.Asset) : bool =
        this.ParentId.IsSome

    [<Extension>]
    static member GetParentId (this: Response.Asset) =
        this.ParentId.Value

type AssetArgs (args: GetParams list) =
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
type ClientExtensions =
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
    static member GetAssets (this: Client) (args: AssetArgs) : Task<Response.Asset List> =
        let worker () : Async<Response.Asset List> = async {
            let! result = getAssets this.Ctx args.Args
            match result with
            | Ok response ->
                return ResizeArray<Response.Asset> response
            | Error ex ->
                return raise ex
        }

        worker () |> Async.StartAsTask


    /// <summary>
    /// Retrieves information about an asset in a certain project given an asset id.
    /// </summary>
    /// <param name="assetId">The id of the asset to get.</param>
    /// <returns>Asset with the given id.</returns>
    [<Extension>]
    static member GetAsset (this: Client) (assetId: int64) : Task<Response.Asset> =
        let worker () : Async<Response.Asset> = async {
            let! result = getAsset this.Ctx assetId
            match result with
            | Ok response ->
                return response
            | Error ex ->
                return raise ex
        }

        worker () |> Async.StartAsTask

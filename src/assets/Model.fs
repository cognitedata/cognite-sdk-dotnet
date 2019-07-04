namespace Cognite.Sdk.Assets

open System
open System.Text
open Cognite.Sdk.Common

/// C# compatible Asset POCO
type Asset () =
    member val ExternalId = String.Empty with get, set
    member val Name = String.Empty with get, set
    member val ParentId = 0L with get, set
    member val Description = String.Empty with get, set
    member val MetaData = dict [] with get, set
    member val Source = String.Empty with get, set
    member val Id = 0L with get, set
    member val CreatedTime = 0L with get, set
    member val LastUpdatedTime = 0L with get, set
    member val ParentExternalId = String.Empty with get, set

/// Asset type for responses.
type AssetReadDto = {
    /// External Id provided by client. Should be unique within the project.
    ExternalId: string option
    /// The name of the asset.
    Name: string
    /// The parent ID of the asset.
    ParentId: int64 option
    /// The description of the asset.
    Description: string option
    /// Custom, application specific metadata. String key -> String value
    MetaData: Map<string, string>
    /// The source of this asset
    Source: string option
    /// The Id of the asset.
    Id: int64
    /// Asset path depth (number of levels below root node).
    CreatedTime: int64
    /// The last time this asset was updated in CDF, in milliseconds since Jan 1, 1970.
    LastUpdatedTime: int64
} with
    member this.Asset () =
        let source = if this.Source.IsSome then this.Source.Value else Unchecked.defaultof<string>
        let metaData = this.MetaData |> Map.toSeq |> dict
        let externalId = if this.ExternalId.IsSome then this.ExternalId.Value else Unchecked.defaultof<string>
        let description = if this.Description.IsSome then this.Description.Value else Unchecked.defaultof<string>
        let parentId = if this.ParentId.IsSome then this.ParentId.Value else Unchecked.defaultof<int64>

        Asset (
            ExternalId = externalId,
            Name = this.Name,
            ParentId = parentId,
            Description = description,
            MetaData = metaData,
            Source = source,
            Id = this.Id,
            CreatedTime = this.CreatedTime,
            LastUpdatedTime = this.LastUpdatedTime
        )


/// Asset type for create requests.
type AssetCreateDto = {
    /// External Id provided by client. Should be unique within the project.
    ExternalId: string option
    /// Name of asset. Often referred to as tag.
    Name: string
    /// Javascript friendly internal ID given to the object.
    ParentId: int64 option
    /// Description of asset.
    Description: string option
    /// Custom, application specific metadata. String key -> String value
    MetaData: Map<string, string>
    /// The source of this asset (NOTE: will be replaced with external Id)
    Source: string option
    /// External Id provided by client. Should be unique within the project.
    ParentExternalId: string option
} with
    static member Create (asset: Asset) : AssetCreateDto =
        {
            ExternalId = if asset.ExternalId = null then None else Some asset.ExternalId
            Name = asset.Name
            ParentId = if asset.ParentId = 0L then None else Some asset.ParentId
            Description = if asset.Description = null then None else Some asset.Description
            MetaData = asset.MetaData |> Seq.map (|KeyValue|) |> Map.ofSeq
            Source = if asset.Source = null then None else Some asset.Source
            ParentExternalId = if asset.ParentExternalId = null then None else Some asset.ParentExternalId
        }


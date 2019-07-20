namespace Fusion.Assets

open System
open System.Collections.Generic

[<CLIMutable>]
type AssetReadPoco = {
    ExternalId : string
    Name : string
    ParentId : int64
    Description : string
    MetaData : IDictionary<string, string>
    Source : string
    Id : int64
    CreatedTime : int64
    LastUpdatedTime : int64
}

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
    /// Time when this asset was created in CDF in milliseconds since Jan 1, 1970.
    CreatedTime: int64
    /// The last time this asset was updated in CDF, in milliseconds since Jan 1, 1970.
    LastUpdatedTime: int64
} with
    /// Translates the domain type to a plain old crl object
    member this.ToPoco () : AssetReadPoco =
        let source = if this.Source.IsSome then this.Source.Value else Unchecked.defaultof<string>
        let metaData = this.MetaData |> Map.toSeq |> dict
        let externalId = if this.ExternalId.IsSome then this.ExternalId.Value else Unchecked.defaultof<string>
        let description = if this.Description.IsSome then this.Description.Value else Unchecked.defaultof<string>
        let parentId = if this.ParentId.IsSome then this.ParentId.Value else 0L

        {
            ExternalId = externalId
            Name = this.Name
            ParentId = parentId
            Description = description
            MetaData = metaData
            Source = source
            Id = this.Id
            CreatedTime = this.CreatedTime
            LastUpdatedTime = this.LastUpdatedTime
        }

/// C# compatible Asset POCO
[<CLIMutable>]
type AssetWritePoco = {
    ExternalId : string
    Name : string
    ParentId : int64
    Description : string
    MetaData : IDictionary<string, string>
    Source : string
    ParentExternalId : string
}

/// Asset type for create requests.
type AssetWriteDto = {
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
    static member FromPoco (asset: AssetWritePoco) : AssetWriteDto =
        let metaData =
            if not (isNull asset.MetaData) then
                asset.MetaData |> Seq.map (|KeyValue|) |> Map.ofSeq
            else
                Map.empty
        {
            ExternalId = if isNull asset.ExternalId then None else Some asset.ExternalId
            Name = asset.Name
            ParentId = if asset.ParentId = 0L then None else Some asset.ParentId
            Description = if isNull asset.Description then None else Some asset.Description
            MetaData = metaData
            Source = if isNull asset.Source then None else Some asset.Source
            ParentExternalId = if isNull asset.ParentExternalId then None else Some asset.ParentExternalId
        }


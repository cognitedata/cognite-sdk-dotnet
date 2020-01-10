// Copyright 2019 Cognite AS
// SPDX-License-Identifier: Apache-2.0

namespace Oryx.Cognite.Assets

open System.Collections.Generic
open Oryx


/// Asset type for responses.
type AssetReadDto = {
    /// External Id provided by client. Must be unique within the project.
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
    /// InternalId of the root object
    RootId: int64
} with
    /// Translates the domain type to a plain old CLR object
    member this.ToAssetEntity () : AssetEntity =
        let source = if this.Source.IsSome then this.Source.Value else Unchecked.defaultof<string>
        let metaData = this.MetaData |> Map.toSeq |> dict
        let externalId = if this.ExternalId.IsSome then this.ExternalId.Value else Unchecked.defaultof<string>
        let description = if this.Description.IsSome then this.Description.Value else Unchecked.defaultof<string>
        let parentId = if this.ParentId.IsSome then this.ParentId.Value else 0L

        AssetEntity(
            externalId = externalId,
            name = this.Name,
            description = description,
            parentId = parentId,
            metaData = metaData,
            source = source,
            id = this.Id,
            createdTime = this.CreatedTime,
            lastUpdatedTime = this.LastUpdatedTime,
            rootId = this.RootId,
            parentExternalId = null
        )
type AssetItemsReadDto = {
    Items: AssetReadDto seq
    NextCursor : string option
}

type AssetFilter =
    private
    | CaseName of string
    | CaseParentIds of int64 seq
    | CaseParentExternalIds of string seq
    | CaseRootIds of Oryx.Cognite.Identity seq
    | CaseMetaData of Map<string, string>
    | CaseSource of string
    | CaseCreatedTime of Oryx.Cognite.TimeRange
    | CaseLastUpdatedTime of Oryx.Cognite.TimeRange
    | CaseRoot of bool
    | CaseExternalIdPrefix of string

    /// Name of asset. Often referred to as tag.
    static member Name name = CaseName name
    /// Filter assets that have one of the ids listed as parent.
    static member ParentIds ids = CaseParentIds ids
    /// Filter out assets without rootId in list
    static member RootIds rootIds = CaseRootIds rootIds
    /// Filter on metadata
    static member MetaData (metaData : IDictionary<string, string>) =
        metaData |> Seq.map (|KeyValue|) |> Map.ofSeq |> CaseMetaData
    /// The source of this asset.
    static member Source source = CaseSource source
    /// Min/Max created time for this asset
    static member CreatedTime createdTime = CaseCreatedTime createdTime
    /// Min/Max last updated time for this asset
    static member LastUpdatedTime lastUpdatedTime = CaseLastUpdatedTime lastUpdatedTime
    /// True if the asset is root
    static member Root root = CaseRoot root
    /// Prefix on externalId
    static member ExternalIdPrefix externalIdPrefix = CaseExternalIdPrefix externalIdPrefix

/// Asset type for create requests.
type AssetWriteDto = {
    /// External Id provided by client. Must be unique within the project.
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
    /// External Id of parent asset provided by client. Must be unique within the project.
    ParentExternalId: string option
} with
    static member FromAssetEntity (asset: AssetEntity) : AssetWriteDto =
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

type ClientExtension internal (context: HttpContext) =
    member internal __.Ctx =
        context

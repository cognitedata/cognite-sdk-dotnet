// Copyright 2019 Cognite AS
// SPDX-License-Identifier: Apache-2.0

module FSharp.Oryx.Cognite.Assets

open System.Collections.Generic
open CogniteSdk.Types

open FSharp.Oryx.Cognite.Common
open CogniteSdk.Types.Common

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
    /// Translates the domain type to a FSharp type
    static member FromAssetEntity(asset: Assets.AssetReadDto) =
        {
            ExternalId = toOption asset.ExternalId
            Name = asset.Name
            ParentId = nullableToOption asset.ParentId
            Description = toOption asset.Description
            MetaData = toMap asset.MetaData
            Source = toOption asset.Source
            Id = asset.Id
            CreatedTime = asset.CreatedTime
            LastUpdatedTime = asset.LastUpdatedTime
            RootId = asset.RootId
        }

type AssetFilter =
    private
    | CaseName of string
    | CaseParentIds of int64 seq
    | CaseParentExternalIds of string seq
    | CaseRootIds of Oryx.Cognite.Identity seq
    | CaseAssetSubtreeIds of Identity seq
    | CaseMetaData of Map<string, string>
    | CaseSource of string
    | CaseCreatedTime of TimeRange
    | CaseLastUpdatedTime of TimeRange
    | CaseRoot of bool
    | CaseExternalIdPrefix of string
    /// Name of asset. Often referred to as tag.
    static member Name name = CaseName name
    /// Filter assets that have one of the ids listed as parent.
    static member ParentIds ids = CaseParentIds ids
    /// Filter out assets without rootId in list
    static member RootIds rootIds = CaseRootIds rootIds
    /// Only include assets in subtrees rooted at the specified assets (including the roots given). If the total
    /// size of the given subtrees exceeds 100,000 assets, an error will be returned.
    static member AssetSubtreeIds ids = CaseAssetSubtreeIds ids
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

type AssetQuery =
    private
    | CaseLimit of int
    | CaseCursor of string
    | CaseAggregatedProperties of string seq
    | CasePartition of string
    /// Max number of results to return
    static member Limit limit = CaseLimit limit
    /// Cursor return from previous request
    static member Cursor cursor = CaseCursor cursor
    /// Set of aggregated properties to include.
    static member AggregatedProperties props = CaseAggregatedProperties props
    /// Splits the data set into N partitions. You need to follow the cursors within each partition in order to
    /// receive all the data. Example: 1/10.
    static member Partition partition = CasePartition partition

let assetFsharpFiltersToCsharp (filters: AssetFilter seq) =
    let filter = Assets.AssetFilterDto()
    filters
    |> Seq.fold (fun (acc: Assets.AssetFilterDto) (f: AssetFilter) ->
        match f with
        | CaseName name -> acc.Name <- name
        | CaseParentIds ids -> acc.ParentIds <- ids
        | CaseParentExternalIds ids -> acc.ParentExternalIds <- ids
        | CaseRootIds ids -> acc.RootIds <- ids |> Seq.map (fun i -> i.ToIdentityDto)
        | CaseAssetSubtreeIds ids -> acc.AssetSubtreeIds <- ids
        | CaseMetaData metadata -> acc.MetaData <- metadata
        | CaseSource source -> acc.Source <- source
        | CaseCreatedTime t -> acc.CreatedTime <- t
        | CaseLastUpdatedTime t -> acc.LastUpdatedTime <- t
        | CaseRoot root -> acc.Root <- root
        | CaseExternalIdPrefix externalIdPrefix -> acc.ExternalIdPrefix <- externalIdPrefix
        acc
    ) filter

let assetFsharpQueriesToCsharp (queries: AssetQuery seq) (filters: AssetFilter seq) =
    let filter = assetFsharpFiltersToCsharp filters
    let query = Assets.AssetQuery()
    let csharpQuery =
        queries
        |> Seq.fold (fun (acc: Assets.AssetQuery) (q: AssetQuery) ->
            match q with
            | CaseLimit limit -> acc.Limit <- System.Nullable(limit)
            | CaseCursor cursor -> acc.Cursor <- cursor
            | CaseAggregatedProperties props -> acc.AggregatedProperties <- props
            | CasePartition partition -> acc.Partition <- partition
            acc
        ) query
    csharpQuery.Filter
    csharpQuery

// /// Asset type for create requests.
// type AssetWriteDto = {
//     /// External Id provided by client. Must be unique within the project.
//     ExternalId: string option
//     /// Name of asset. Often referred to as tag.
//     Name: string
//     /// Javascript friendly internal ID given to the object.
//     ParentId: int64 option
//     /// Description of asset.
//     Description: string option
//     /// Custom, application specific metadata. String key -> String value
//     MetaData: Map<string, string>
//     /// The source of this asset (NOTE: will be replaced with external Id)
//     Source: string option
//     /// External Id of parent asset provided by client. Must be unique within the project.
//     ParentExternalId: string option
// } with
//     static member FromAssetEntity (asset: AssetEntity) : AssetWriteDto =
//         let metaData =
//             if not (isNull asset.MetaData) then
//                 asset.MetaData |> Seq.map (|KeyValue|) |> Map.ofSeq
//             else
//                 Map.empty
//         {
//             ExternalId = if isNull asset.ExternalId then None else Some asset.ExternalId
//             Name = asset.Name
//             ParentId = if asset.ParentId = 0L then None else Some asset.ParentId
//             Description = if isNull asset.Description then None else Some asset.Description
//             MetaData = metaData
//             Source = if isNull asset.Source then None else Some asset.Source
//             ParentExternalId = if isNull asset.ParentExternalId then None else Some asset.ParentExternalId
//         }

// type ClientExtension internal (context: HttpContext) =
//     member internal __.Ctx =
//         context

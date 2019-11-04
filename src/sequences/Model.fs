// Copyright 2019 Cognite AS
// SPDX-License-Identifier: Apache-2.0

namespace CogniteSdk.Sequences

open System.Collections.Generic
open Thoth.Json.Net

open Oryx

open CogniteSdk
open CogniteSdk.Common

type ValueType =
    private
    | CaseString
    | CaseDouble
    | CaseLong
    static member String = CaseString
    static member Double = CaseDouble
    static member Long = CaseLong
    override this.ToString () =
        match this with
            | CaseString -> "STRING"
            | CaseDouble -> "DOUBLE"
            | CaseLong -> "LONG"
    static member Decoder: Decoder<ValueType> =
        Decode.string
        |> Decode.andThen (fun s ->
            match s.ToLower() with
            | "string" -> CaseString |> Decode.succeed
            | "double" -> CaseDouble |> Decode.succeed
            | "long" -> CaseLong |> Decode.succeed
            | _ -> sprintf "Could not decode valueType %A" s |> Decode.fail
        )

type ColumnEntity internal (name: string, externalId: string, description: string, valueType: ValueType, metadata: IDictionary<string, string>, createdTime: int64, lastUpdatedTime: int64) =
    /// The name of the column.
    member val Name : string = name with get, set
    /// The externalId of the column. Must be unique within the project.
    member val ExternalId : string = externalId with get, set
    /// The description of the column.
    member val Description : string = description with get, set
    /// The valueType of the column. Enum STRING, DOUBLE, LONG
    member val ValueType : ValueType = valueType with get, set
    /// Custom, application specific metadata. String key -> String value
    member val MetaData : IDictionary<string, string> = metadata with get, set
    /// Time when this column was created in CDF in milliseconds since Jan 1, 1970.
    member val CreatedTime : int64 = createdTime with get
    /// The last time this column was updated in CDF, in milliseconds since Jan 1, 1970.
    member val LastUpdatedTime : int64 = lastUpdatedTime with get

    /// Create new empty ColumnEntity. Set content using the properties.
    new () =
        ColumnEntity(name=null, externalId=null, description=null, valueType=ValueType.Double, metadata=null, createdTime=0L, lastUpdatedTime=0L)

type ColumnCreateDto = {
    Name: string option
    ExternalId: string
    Description: string option
    ValueType: ValueType
    MetaData: Map<string, string>
} with
    static member FromColumnEntity (entity: ColumnEntity) : ColumnCreateDto =
        let metadata =
            if not (isNull entity.MetaData) then
                entity.MetaData |> Seq.map (|KeyValue|) |> Map.ofSeq
            else
                Map.empty
        {
            Name = if isNull entity.Name then None else Some entity.Name
            ExternalId = entity.ExternalId
            Description = if isNull entity.Description then None else Some entity.Description
            ValueType = entity.ValueType
            MetaData = metadata
        }

type ColumnReadDto = {
    Name: string option
    ExternalId: string option
    Description: string option
    ValueType: ValueType
    MetaData: Map<string, string>
    CreatedTime: int64
    LastUpdatedTime: int64
} with
    /// Tranlates the domain type to a plain old CLR object
    member this.ToColumnEntity () : ColumnEntity =
        let metadata = this.MetaData |> Map.toSeq |> dict
        let name = this.Name |> Option.defaultValue Unchecked.defaultof<string>
        let externalId = this.ExternalId |> Option.defaultValue Unchecked.defaultof<string>
        let description = this.Description |> Option.defaultValue Unchecked.defaultof<string>
        ColumnEntity(
            name = name,
            externalId = externalId,
            description = description,
            valueType = this.ValueType,
            metadata = metadata,
            createdTime = this.CreatedTime,
            lastUpdatedTime = this.LastUpdatedTime
        )

type SequenceEntity internal (id: int64, name: string, externalId: string, description: string, assetId: int64, metadata: IDictionary<string, string>, columns: ColumnEntity seq, createdTime: int64, lastUpdatedTime: int64) =
    /// The Id of the sequence
    member val Id : int64 = id with get
    /// The name of the sequence.
    member val Name : string = name with get, set
    /// The description of the sequence.
    member val Description : string = description with get, set
    /// The valueType of the sequence. Enum STRING, DOUBLE, LONG
    member val AssetId : int64 = assetId with get, set
    /// The externalId of the sequence. Must be unique within the project.
    member val ExternalId : string = externalId with get, set
    /// Custom, application specific metadata. String key -> String value
    member val MetaData : IDictionary<string, string> = metadata with get, set
    /// Time when this sequence was created in CDF in milliseconds since Jan 1, 1970.
    member val Columns : ColumnEntity seq = columns with get, set
    /// Time when this sequence was created in CDF in milliseconds since Jan 1, 1970.
    member val CreatedTime : int64 = createdTime with get
    /// The last time this sequence was updated in CDF, in milliseconds since Jan 1, 1970.
    member val LastUpdatedTime : int64 = lastUpdatedTime with get

    /// Create new empty SequenceEntity. Set content using the properties.
    new () =
        SequenceEntity(id=0L, name=null, description=null, assetId=0L, externalId=null, metadata=null, columns=null, createdTime=0L, lastUpdatedTime=0L)

type SequenceCreateDto = {
    Name: string option
    Description: string option
    AssetId: int64 option
    ExternalId: string option
    MetaData: Map<string, string>
    Columns: ColumnCreateDto seq
} with
    static member FromSequenceCreateDto (entity: SequenceEntity) : SequenceCreateDto =
        let metadata =
            if not (isNull entity.MetaData) then
                entity.MetaData |> Seq.map (|KeyValue|) |> Map.ofSeq
            else
                Map.empty
        {
            Name = if isNull entity.Name then None else Some entity.Name
            Description = if isNull entity.Description then None else Some entity.Description
            AssetId = if entity.AssetId = 0L then None else Some entity.AssetId
            ExternalId = if isNull entity.ExternalId then None else Some entity.ExternalId
            MetaData = metadata
            Columns = Seq.map ColumnCreateDto.FromColumnEntity entity.Columns
        }

[<CLIMutable>]
type SequenceItems = {
    Items: SequenceEntity seq
    NextCursor: string
}

type SequenceReadDto = {
    Id: int64
    Name: string option
    Description: string option
    AssetId: int64 option
    ExternalId: string option
    MetaData: Map<string, string>
    Columns: ColumnReadDto seq
    CreatedTime: int64
    LastUpdatedTime: int64
} with
    /// Translates the domain type to a plain old CLR object
    member this.ToSequenceEntity () : SequenceEntity =
        let metadata = this.MetaData |> Map.toSeq |> dict
        let columns = this.Columns |> Seq.map (fun col -> col.ToColumnEntity())
        let name = this.Name |> Option.defaultValue Unchecked.defaultof<string>
        let description = this.Description |> Option.defaultValue Unchecked.defaultof<string>
        let assetId = this.AssetId |> Option.defaultValue 0L
        let externalId = this.ExternalId |> Option.defaultValue Unchecked.defaultof<string>
        SequenceEntity(
            id = this.Id,
            name = name,
            description = description,
            assetId = assetId,
            externalId = externalId,
            metadata = metadata,
            createdTime = this.CreatedTime,
            lastUpdatedTime = this.LastUpdatedTime,
            columns = columns
        )

type SequenceItemsReadDto = {
    Items: SequenceReadDto seq
    NextCursor : string option
}

type SequenceFilter =
    private
    | CaseName of string
    | CaseExternalIdPrefix of string
    | CaseMetaData of Map<string, string>
    | CaseAssetIds of int64 seq
    | CaseRootAssetIds of int64 seq
    | CaseCreatedTime of TimeRange
    | CaseLastUpdatedTime of TimeRange

    /// Return only sequences with this exact name.
    static member Name name = CaseName name
    /// Prefix on externalId
    static member ExternalIdPrefix externalIdPrefix = CaseExternalIdPrefix externalIdPrefix
    /// Filter on metadata
    static member MetaData (metaData : IDictionary<string, string>) =
        metaData |> Seq.map (|KeyValue|) |> Map.ofSeq |> CaseMetaData
    /// Return only sequences linked to one of the specified assets.
    static member AssetIds assetIds = CaseAssetIds assetIds
    /// Return only sequences linked to assets with one of these assets as the root asset.
    static member RootAssetIds rootAssetIds = CaseRootAssetIds rootAssetIds
    /// Min/Max created time for this sequence
    static member CreatedTime createdTime = CaseCreatedTime createdTime
    /// Min/Max last updated time for this sequence
    static member LastUpdatedTime lastUpdatedTime = CaseLastUpdatedTime lastUpdatedTime

type ClientExtension internal (context: HttpContext) =
    member internal __.Ctx =
        context

// Copyright 2019 Cognite AS
// SPDX-License-Identifier: Apache-2.0

namespace CogniteSdk.Sequences

open System.Collections.Generic
open Thoth.Json.Net

open Oryx
open CogniteSdk

type ColumnType =
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

    member this.GetType () =
        match this with
        | CaseString -> ValueType.STRING
        | CaseDouble -> ValueType.DOUBLE
        | CaseLong -> ValueType.LONG

    static member Decoder: Decoder<ColumnType> =
        Decode.string
        |> Decode.andThen (fun s ->
            match s.ToLower() with
            | "string" -> CaseString |> Decode.succeed
            | "double" -> CaseDouble |> Decode.succeed
            | "long" -> CaseLong |> Decode.succeed
            | _ -> sprintf "Could not decode valueType %A" s |> Decode.fail
        )

type ColumnCreateDto = {
    Name: string option
    ExternalId: string
    Description: string option
    ValueType: ColumnType
    MetaData: Map<string, string>
} with
    static member FromEntity (entity: ColumnEntity) : ColumnCreateDto =
        let metadata =
            if not (isNull entity.MetaData) then
                entity.MetaData |> Seq.map (|KeyValue|) |> Map.ofSeq
            else
                Map.empty
        {
            Name = if isNull entity.Name then None else Some entity.Name
            ExternalId = entity.ExternalId
            Description = if isNull entity.Description then None else Some entity.Description
            ValueType =
                match entity.ValueType with
                | ValueType.DOUBLE -> ColumnType.Double
                | ValueType.LONG -> ColumnType.Long
                | _ -> ColumnType.String
            MetaData = metadata
        }

type ColumnReadDto = {
    Name: string option
    ExternalId: string option
    Description: string option
    ValueType: ColumnType
    MetaData: Map<string, string>
    CreatedTime: int64
    LastUpdatedTime: int64
} with
    /// Tranlates the domain type to a plain old CLR object
    member this.ToEntity () : ColumnEntity =
        let metadata = this.MetaData |> Map.toSeq |> dict
        let name = this.Name |> Option.defaultValue Unchecked.defaultof<string>
        let externalId = this.ExternalId |> Option.defaultValue Unchecked.defaultof<string>
        let description = this.Description |> Option.defaultValue Unchecked.defaultof<string>

        ColumnEntity(
            name = name,
            externalId = externalId,
            description = description,
            valueType = this.ValueType.GetType (),
            metadata = metadata,
            createdTime = this.CreatedTime,
            lastUpdatedTime = this.LastUpdatedTime
        )

type SequenceCreateDto = {
    Name: string option
    Description: string option
    AssetId: int64 option
    ExternalId: string option
    MetaData: Map<string, string>
    Columns: ColumnCreateDto seq
} with
    static member FromEntity (entity: SequenceEntity) : SequenceCreateDto =
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
            Columns = Seq.map ColumnCreateDto.FromEntity entity.Columns
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
    member this.ToEntity () : SequenceEntity =
        let metadata = this.MetaData |> Map.toSeq |> dict
        let columns = this.Columns |> Seq.map (fun col -> col.ToEntity())
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

type RowDto = {
    RowNumber: int64
    Values: RowValue seq
} with
    member this.ToEntity() = RowEntity(this.RowNumber, this.Values)
    static member FromEntity(entity: RowEntity) =
        {
            RowNumber = entity.RowNumber
            Values = entity.Values
        }

type ColumnInfoReadDto = {
    ExternalId: string option
    Name: string option
    ValueType: ColumnType
} with
    member this.ToColumnInfoReadEntity() =
        let externalId = Option.defaultValue Unchecked.defaultof<string> this.ExternalId
        let name = Option.defaultValue Unchecked.defaultof<string> this.Name
        let valueType = this.ValueType.GetType ()

        ColumnInfoReadEntity(externalId, name, valueType)

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

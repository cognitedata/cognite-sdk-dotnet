// Copyright 2019 Cognite AS
// SPDX-License-Identifier: Apache-2.0

namespace CogniteSdk.Files

open System.Collections.Generic
open Oryx

type FileEntity internal (id: int64, externalId: string, name: string, mimeType: string, metadata: IDictionary<string, string>, assetIds: IEnumerable<int64>, source: string, createdTime: int64, lastUpdatedTime: int64, uploadedTime: int64, sourceCreatedTime: int64, sourceModifiedTime: int64, uploaded: bool) =

    member val ExternalId : string = externalId with get, set
    member val MimeType : string = mimeType with get, set
    member val MetaData : IDictionary<string, string> = metadata with get, set
    member val AssetIds : IEnumerable<int64> = assetIds with get, set
    member val Source : string = source with get, set
    member val SourceCreatedTime : int64 = sourceCreatedTime with get, set
    member val sourceModifiedTime : int64 = sourceModifiedTime with get, set
    member val UploadedTime : int64 = uploadedTime with get, set

    member val Name : string = name with get
    member val Id : int64 = id with get
    member val CreatedTime : int64 = createdTime with get
    member val LastUpdatedTime : int64 = lastUpdatedTime with get
    member val Uploaded : bool = uploaded with get

    new () =
        FileEntity(
            id=0L,
            externalId=null,
            name=null,
            mimeType=null,
            metadata=null,
            assetIds=null,
            source=null,
            createdTime=0L,
            uploadedTime=0L,
            lastUpdatedTime=0L,
            sourceCreatedTime=0L,
            sourceModifiedTime=0L,
            uploaded=false
        )

    new (
        externalId: string,
        name: string,
        mimeType: string,
        metadata: IDictionary<string, string>,
        assetIds: IEnumerable<int64>,
        source: string,
        uploadedTime: int64,
        sourceCreatedTime: int64,
        sourceModifiedTime: int64) =
        FileEntity(
            id=0L,
            externalId=externalId,
            name=name,
            mimeType=mimeType,
            metadata=metadata,
            assetIds=assetIds,
            source=source,
            createdTime=0L,
            uploadedTime=uploadedTime,
            lastUpdatedTime=0L,
            sourceCreatedTime=sourceCreatedTime,
            sourceModifiedTime=sourceModifiedTime,
            uploaded=false
        )

[<CLIMutable>]
type FileItems = {
    Items: FileEntity seq
    NextCursor: string
}

type FileReadDto = {
    Id : int64
    ExternalId : string option
    Name : string
    MimeType : string option
    MetaData : Map<string, string>
    AssetIds : int64 list
    Source : string option
    SourceCreatedTime : int64 option
    SourceModifiedTime : int64 option
    UploadedTime : int64 option
    CreatedTime : int64
    LastUpdatedTime : int64
    Uploaded : bool
} with
    member this.ToFileEntity () : FileEntity =
        let externalId = Option.defaultValue Unchecked.defaultof<string> this.ExternalId
        let mimeType = Option.defaultValue Unchecked.defaultof<string> this.MimeType
        let metadata = this.MetaData |> Map.toSeq |> dict
        let assetIds = this.AssetIds |> List.ofSeq
        let source = Option.defaultValue Unchecked.defaultof<string> this.Source
        let uploadedTime = Option.defaultValue Unchecked.defaultof<int64> this.UploadedTime
        let sourceCreatedTime = Option.defaultValue Unchecked.defaultof<int64> this.SourceCreatedTime
        let sourceModifiedTime = Option.defaultValue Unchecked.defaultof<int64> this.SourceModifiedTime
        FileEntity(
            id = this.Id,
            externalId = externalId,
            name = this.Name,
            mimeType = mimeType,
            metadata = metadata,
            assetIds = assetIds,
            source = source,
            uploadedTime = uploadedTime,
            sourceCreatedTime = sourceCreatedTime,
            sourceModifiedTime = sourceModifiedTime,
            createdTime = this.CreatedTime,
            lastUpdatedTime = this.LastUpdatedTime,
            uploaded = this.Uploaded
        )

type FileItemsReadDto = {
    Items : FileReadDto seq
    NextCursor : string option
}

type FileFilter =
    private
    | CaseName of string
    | CaseMimeType of string
    | CaseMetaData of Map<string, string>
    | CaseAssetIds of int64 seq
    | CaseSource of string
    | CaseCreatedTime of CogniteSdk.TimeRange
    | CaseLastUpdatedTime of CogniteSdk.TimeRange
    | CaseUploadedTime of CogniteSdk.TimeRange
    | CaseSourceCreatedTime of CogniteSdk.TimeRange
    | CaseSourceModifiedTime of CogniteSdk.TimeRange
    | CaseExternalIdPrefix of string
    | CaseUploaded of bool

    /// Name of the file
    static member Name name = CaseName name
    /// File type. E.g. text/plain, application/pdf, ..
    static member MimeType mimeType = CaseMimeType mimeType
    /// Custom, application specific metadata. String key -> String value
    static member MetaData metadata = CaseMetaData metadata
    /// Only include files that reference these specific asset IDs
    static member AssetIds assetIds = CaseAssetIds assetIds
    /// The source of this event
    static member Source source = CaseSource source
    /// Range between two timestamps.
    static member CreatedTime createdTime = CaseCreatedTime createdTime
    /// Range between two timestamps.
    static member LastUpdatedTime lastUpdatedTime = CaseLastUpdatedTime lastUpdatedTime
    /// Range between two timestamps.
    static member UploadedTime uploadedTime = CaseUploadedTime uploadedTime
    /// Range between two timestamps.
    static member SourceCreatedTime sourceCreatedTime = CaseSourceCreatedTime sourceCreatedTime
    /// Range between two timestamps.
    static member SourceModifiedTime sourceModifiedTime = CaseSourceModifiedTime sourceModifiedTime
    /// The external ID provided by the client. Must be unique within the project
    static member ExternalIdPrefix externalIdPrefix = CaseExternalIdPrefix externalIdPrefix
    /// Whether or not the actual file is uploaded
    static member Uploaded uploaded = CaseUploaded uploaded

type ClientExtension internal (context: HttpContext) =
    member internal __.Ctx =
        context

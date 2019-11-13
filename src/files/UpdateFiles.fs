// Copyright 2019 Cognite AS
// SPDX-License-Identifier: Apache-2.0

namespace CogniteSdk.Files

open System
open System.Collections.Generic
open System.Net.Http
open System.Runtime.CompilerServices
open System.Runtime.InteropServices
open System.Threading
open System.Threading.Tasks

open FSharp.Control.Tasks.V2.ContextInsensitive
open Oryx
open Thoth.Json.Net

open CogniteSdk

type MetaDataChange = {
    AddMetaData : Map<string, string> option
    RemoveMetaData : string seq
}

type MetaDataUpdate =
    | CaseSetMetaData of Map<string, string>
    | CaseChangeMetaData of MetaDataChange

type AssetIdsChange = {
    AddAssetIds : int64 seq
    RemoveAssetIds : int64 seq
}

type AssetIdsUpdate =
    | CaseSetAssetIds of int64 list
    | CaseChangeAssetIds of AssetIdsChange

/// File update parameters
type FileUpdate =
    private
    | UpdateExternalId of string option
    | UpdateSource of string option
    | UpdateMetaData of MetaDataUpdate option
    | UpdateAssetIds of AssetIdsUpdate option
    | UpdateSourceCreatedTime of int64 option
    | UpdateSourceModfiedTime of int64 option

    /// Set the externalId of file. Must be unique within the project
    static member SetExternalId id =
        UpdateExternalId id
    /// Clear the externalId of file.
    static member ClearExternalId =
        UpdateExternalId None
    /// Set the source of this file
    static member SetSource source =
        Some source |> UpdateSource
    /// Clear the source of this file
    static member ClearSource =
        UpdateSource None
    /// Set assetIds of file
    static member SetAssetIds (assetIds: int64 list) =
        assetIds |> CaseSetAssetIds |> Some |> UpdateAssetIds
    /// Clear all assetIds
    static member ClearAssetIds =
        UpdateAssetIds None
    /// Change assetIds of the file by adding new assetIds as given in `add` and removing assetIds given in `remove`
    static member ChangeAssetIds (add: int64 seq, remove: int64 seq) =
        {
            AddAssetIds = if isNull add then Seq.empty else add
            RemoveAssetIds = if isNull remove then Seq.empty else remove
        } |> CaseChangeAssetIds |> Some |> UpdateAssetIds
    /// Set metadata of the file. This removes any old metadata.
    static member SetMetaData (md : IDictionary<string, string>) =
        md |> Seq.map (|KeyValue|) |> Map.ofSeq |> CaseSetMetaData |> Some |> UpdateMetaData
    /// Remove all metadata from the file
    static member ClearMetaData =
        UpdateMetaData None
    /// Change metadata of the file by adding new data as given in `add` and removing keys given in `remove`
    static member ChangeMetaData (add: IDictionary<string, string>, remove: string seq) =
        {
            AddMetaData =
                if isNull add then
                    None
                else
                    add |> Seq.map (|KeyValue|) |> Map.ofSeq |> Some
            RemoveMetaData = if isNull remove then Seq.empty else remove
        } |> CaseChangeMetaData |> Some |> UpdateMetaData
    /// Set sourceCreatedTime
    static member SetSourceCreatedTime (time: int64) =
        time |> Some |> UpdateSourceCreatedTime
    /// Clear sourceCreatedTime
    static member ClearSourceCreatedTime =
        None |> UpdateSourceCreatedTime
    /// Set SourceModifiedTime
    static member SetSourceModifiedTime (time: int64) =
        time |> Some |> UpdateSourceModfiedTime
    /// Clear SourceModifiedTime
    static member ClearSourceModifiedTime =
        None |> UpdateSourceModfiedTime

/// The functional file update core module
[<RequireQualifiedAccess>]
module Update =
    [<Literal>]
    let Url = "/files/update"
    let renderUpdateFields (arg: FileUpdate) =
        match arg with
        | UpdateExternalId optExternalId ->
            "externalId", Encode.object [
                match optExternalId with
                | Some externalId -> yield "set", Encode.string externalId
                | None -> yield "setNull", Encode.bool true
            ]
        | UpdateSource optSource ->
            "source", Encode.object [
                match optSource with
                | Some source -> yield "set", Encode.string source
                | None -> yield "setNull", Encode.bool true
            ]
        | UpdateAssetIds optAssetIds ->
            "assetIds", Encode.object [
                match optAssetIds with
                | Some assetIdUpdate ->
                    match assetIdUpdate with
                    | CaseSetAssetIds assetIds ->
                        yield "set", Encode.int53list assetIds
                    | CaseChangeAssetIds change ->
                        yield "add", Encode.seq (Seq.map Encode.int53 change.AddAssetIds)
                        yield "remove", Encode.seq (Seq.map Encode.int53 change.RemoveAssetIds)
                | None -> yield "set", Encode.list []
            ]
        | UpdateMetaData optMeta ->
            "metadata", Encode.object [
                match optMeta with
                | Some (CaseSetMetaData data) ->
                        yield "set", Encode.propertyBag data
                | Some (CaseChangeMetaData data) ->
                    if data.AddMetaData.IsSome then yield "add", Encode.propertyBag data.AddMetaData.Value
                    yield "remove", Encode.seq (Seq.map Encode.string data.RemoveMetaData)
                | None -> yield "set", Encode.object []
            ]
        | UpdateSourceCreatedTime optSourceTime ->
            "sourceCreatedTime", Encode.object [
                match optSourceTime with
                | Some sourceTime -> yield "set", Encode.int53 sourceTime
                | None -> yield "setNull", Encode.bool true
            ]
        | UpdateSourceModfiedTime optSourceTime ->
            "sourceModifiedTime", Encode.object [
                match optSourceTime with
                | Some sourceTime -> yield "set", Encode.int53 sourceTime
                | None -> yield "setNull", Encode.bool true
            ]

    type private FileUpdateRequest = {
        Id: Identity
        Params: FileUpdate seq
    } with
        member this.Encoder =
            Encode.object [
                yield
                    match this.Id with
                    | CaseId id -> "id", Encode.int53 id
                    | CaseExternalId id -> "externalId", Encode.string id
                yield "update", Encode.object [
                    yield! this.Params |> Seq.map(renderUpdateFields)
                ]
            ]

    type private FilesUpdateRequest = {
        Items: FileUpdateRequest seq
    } with
        member this.Encoder =
            Encode.object [
                "items", Seq.map (fun (item:FileUpdateRequest) -> item.Encoder) this.Items |> Encode.seq
            ]

    type FileResponse = {
        Items: FileReadDto seq
    } with
         static member Decoder : Decoder<FileResponse> =
            Decode.object (fun get -> {
                Items = get.Required.Field "items" (Decode.list FileReadDto.Decoder |> Decode.map seq)
            })

    let updateCore (args: (Identity * FileUpdate list) list) (fetch: HttpHandler<HttpResponseMessage, 'a>) =
        let request : FilesUpdateRequest = {
            Items = [
                yield! args |> Seq.map(fun (fileId, args) -> { Id = fileId; Params = args })
            ]
        }

        POST
        >=> setVersion V10
        >=> setContent (Content.JsonValue request.Encoder)
        >=> setResource Url
        >=> fetch
        >=> withError decodeError
        >=> json FileResponse.Decoder
        >=> map (fun res -> res.Items)


    /// <summary>
    /// Update one or more files. Supports partial updates, meaning that fields omitted from the requests are not changed
    /// </summary>
    /// <param name="files">The list of files to update.</param>
    /// <param name="next">Async handler to use.</param>
    /// <returns>List of updated files.</returns>
    let update (files: (Identity * (FileUpdate list)) list) (next: NextFunc<FileReadDto seq,'a>)  : HttpContext -> HttpFuncResult<'a> =
        updateCore files fetch next

    /// <summary>
    /// Update one or more files. Supports partial updates, meaning that fields omitted from the requests are not changed
    /// </summary>
    /// <param name="files">The list of files to update.</param>
    /// <returns>List of updated files.</returns>
    let updateAsync (files: (Identity * FileUpdate list) list) : HttpContext -> HttpFuncResult<FileReadDto seq> =
        updateCore files fetch finishEarly

[<Extension>]
type UpdatefilesClientExtensions =
    /// <summary>
    /// Update one or more files. Supports partial updates, meaning that fields omitted from the requests are not changed
    /// </summary>
    /// <param name="files">The list of files to update.</param>
    /// <returns>List of updated files.</returns>
    [<Extension>]
    static member UpdateAsync (this: ClientExtension, files: ValueTuple<Identity, FileUpdate seq> seq, [<Optional>] token: CancellationToken) : Task<FileEntity seq> =
        task {
            let files' = files |> Seq.map (fun struct (id, options) -> (id, options |> List.ofSeq)) |> List.ofSeq
            let ctx = this.Ctx |> Context.setCancellationToken token
            let! result = Update.updateAsync files' ctx
            match result with
            | Ok ctx ->
                return ctx.Response |> Seq.map (fun file -> file.ToFileEntity ())
            | Error (ApiError error) -> return raise (error.ToException ())
            | Error (Panic error) -> return raise error
        }

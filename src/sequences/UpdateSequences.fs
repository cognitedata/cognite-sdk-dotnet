// Copyright 2019 Cognite AS
// SPDX-License-Identifier: Apache-2.0

namespace CogniteSdk.Sequences

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
    Add : Map<string, string> option
    Remove : string seq
}

type MetaDataUpdate =
    | Set of Map<string, string>
    | Change of MetaDataChange

/// Sequence update parameters
type SequenceUpdate =
    private
    | CaseName of string option
    | CaseDescription of string option
    | CaseAssetId of int64 option
    | CaseExternalId of string option
    | CaseMetaData of MetaDataUpdate

    /// Set or clear the name of the Sequence. Often referred to as tag.
    static member SetName name =
        CaseName name
    /// Set or clear the description of Sequence.
    static member SetDescription description =
        CaseDescription description
    /// Set or clear assetId of Sequence.
    static member SetAssetId assetId = CaseAssetId assetId
    /// Set metadata of Sequence. This removes any old metadata.
    static member SetMetaData (md : IDictionary<string, string>) =
        md |> Seq.map (|KeyValue|) |> Map.ofSeq |> Set |> CaseMetaData
    /// Set metadata of Sequence. This removes any old metadata.
    static member SetMetaData (md : Map<string, string>) =
        md |> Set |> CaseMetaData
    /// Change metadata of Sequence by adding new data as given in `add` and removing keys given in `remove`
    static member ChangeMetaData (add: IDictionary<string, string>, remove: string seq) =
        {
            Add =
                if isNull add then
                    None
                else
                    add |> Seq.map (|KeyValue|) |> Map.ofSeq |> Some
            Remove = if isNull remove then Seq.empty else remove
        } |> Change |> CaseMetaData
    /// Set the externalId of Sequence. Must be unique within the project
    static member SetExternalId id =
        CaseExternalId id
    /// Clear the externalId of Sequence.
    static member ClearExternalId =
        CaseExternalId None

/// The functional sequence update core module
[<RequireQualifiedAccess>]
module Update =
    [<Literal>]
    let Url = "/sequences/update"

    let renderUpdateFields (arg: SequenceUpdate) =
        match arg with
        | CaseName optName ->
            "name", Encode.object [
                match optName with
                | Some name -> yield "set", Encode.string name
                | None -> yield "setNull", Encode.bool true
            ]
        | CaseDescription optDesc ->
            "description", Encode.object [
                match optDesc with
                | Some desc -> yield "set", Encode.string desc
                | None -> yield "setNull", Encode.bool true
            ]
        | CaseAssetId optAssetId ->
            "assetId", Encode.object [
                match optAssetId with
                | Some assetId -> "set", Encode.int53 assetId
                | None -> "setNull", Encode.bool true
            ]
        | CaseMetaData meta ->
            match meta with
            | Set data ->
                "metadata", Encode.object [
                    yield "set", Encode.propertyBag data
                ]
            | Change data ->
                "metadata", Encode.object [
                    if data.Add.IsSome then yield "add", Encode.propertyBag data.Add.Value
                    yield "remove", Encode.seq (Seq.map Encode.string data.Remove)
                ]
        | CaseExternalId optExternalId ->
            "externalId", Encode.object [
                match optExternalId with
                | Some externalId -> yield "set", Encode.string externalId
                | None -> yield "setNull", Encode.bool true
            ]

    type private SequenceUpdateRequest = {
        Id: Identity
        Params: SequenceUpdate seq
    } with
        member this.Encoder =
            Encode.object [
                yield
                    match this.Id with
                    | Identity.CaseId id -> "id", Encode.int53 id
                    | Identity.CaseExternalId id -> "externalId", Encode.string id
                yield "update", Encode.object [
                    yield! this.Params |> Seq.map(renderUpdateFields)
                ]
            ]

    type private SequencesUpdateRequest = {
        Items: SequenceUpdateRequest seq
    } with
        member this.Encoder =
            Encode.object [
                "items", Seq.map (fun (item:SequenceUpdateRequest) -> item.Encoder) this.Items |> Encode.seq
            ]

    type SequenceResponse = {
        Items: SequenceReadDto seq
    } with
         static member Decoder : Decoder<SequenceResponse> =
            Decode.object (fun get -> {
                Items = get.Required.Field "items" (Decode.list SequenceReadDto.Decoder |> Decode.map seq)
            })

    let updateCore (args: (Identity * SequenceUpdate list) list) (fetch: HttpHandler<HttpResponseMessage, 'a>) =
        let decodeResponse = Decode.decodeResponse SequenceResponse.Decoder (fun res -> res.Items)
        let request : SequencesUpdateRequest = {
            Items = [
                yield! args |> Seq.map(fun (sequenceId, args) -> { Id = sequenceId; Params = args })
            ]
        }
        POST
        >=> setVersion V10
        >=> setContent (Content.JsonValue request.Encoder)
        >=> setResource Url
        >=> fetch
        >=> decodeResponse

    /// <summary>
    /// Update one or more Sequences. Supports partial updates, meaning that fields omitted from the requests are not changed
    /// </summary>
    /// <param name="sequences">The list of Sequences to update.</param>
    /// <param name="next">Async handler to use.</param>
    /// <returns>List of updated Sequences.</returns>
    let update (sequences: (Identity * (SequenceUpdate list)) list) (next: NextFunc<SequenceReadDto seq,'a>)  : HttpContext -> HttpFuncResult<'a> =
        updateCore sequences fetch next

    /// <summary>
    /// Update one or more Sequences. Supports partial updates, meaning that fields omitted from the requests are not changed
    /// </summary>
    /// <param name="sequences">The list of Sequences to update.</param>
    /// <returns>List of updated Sequences.</returns>
    let updateAsync (sequences: (Identity * SequenceUpdate list) list) : HttpContext -> HttpFuncResult<SequenceReadDto seq> =
        updateCore sequences fetch finishEarly

[<Extension>]
type UpdateSequencesClientExtensions =
    /// <summary>
    /// Update one or more Sequences. Supports partial updates, meaning that fields omitted from the requests are not changed
    /// </summary>
    /// <param name="sequences">The list of Sequences to update.</param>
    /// <returns>List of updated Sequences.</returns>
    [<Extension>]
    static member UpdateAsync (this: ClientExtension, sequences: ValueTuple<Identity, SequenceUpdate seq> seq, [<Optional>] token: CancellationToken) : Task<SequenceEntity seq> =
        task {
            let ctx = this.Ctx |> Context.setCancellationToken token
            let sequences' = sequences |> Seq.map (fun struct (id, options) -> (id, options |> List.ofSeq)) |> List.ofSeq
            let! result = Update.updateAsync sequences' ctx
            match result with
            | Ok ctx ->
                return ctx.Response |> Seq.map (fun sequence -> sequence.ToSequenceEntity ())
            | Error error ->
                return raise (error.ToException ())
        }

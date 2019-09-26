// Copyright 2019 Cognite AS
// SPDX-License-Identifier: Apache-2.0

namespace CogniteSdk.Events

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

/// Event update parameters
type EventUpdate =
    private
    | CaseStartTime of int64 option
    | CaseEndTime of int64 option
    | CaseDescription of string option
    | CaseMetaData of MetaDataUpdate option
    | CaseEventIds of int64 list option
    | CaseSource of string option
    | CaseType of string option
    | CaseSubType of string option
    | CaseExternalId of string option

    /// Set or clear the Start Time of the Event
    static member SetStartTime startTime =
        CaseStartTime startTime
    /// Set or clear the End Time of the Event
    static member SetEndTime endTime =
        CaseEndTime endTime
    /// Set the description of the Event.
    static member SetDescription description =
        CaseDescription description
    /// Clear the description of the Event.
    static member ClearDescription =
        CaseDescription None
    /// Set metadata of the Event. This removes any old metadata.
    static member SetMetaData (md : IDictionary<string, string>) =
        md |> Seq.map (|KeyValue|) |> Map.ofSeq |> Set |> Some |> CaseMetaData
    /// Remove all metadata from the Event
    static member ClearMetaData () =
        CaseMetaData None
    /// Change metadata of the Event by adding new data as given in `add` and removing keys given in `remove`
    static member ChangeMetaData (add: IDictionary<string, string>, remove: string seq) =
        {
            Add =
                if isNull add then
                    None
                else
                    add |> Seq.map (|KeyValue|) |> Map.ofSeq |> Some
            Remove = if isNull remove then Seq.empty else remove
        } |> Change |> Some |> CaseMetaData
    static member SetAssetIds (md: int64 list) =
        md |> Some |> CaseEventIds
    static member ClearAssetIds () =
        CaseEventIds None
    /// Set the source of this event
    static member SetSource source =
        Some source |> CaseSource
    /// Clear the source of this event
    static member ClearSource =
        CaseSource None
    /// Set the Type of this event
    static member SetType eventType =
        Some eventType |> CaseType
    /// Clear the Type of this event
    static member ClearType =
        CaseType None
    /// Set the SubType of this event
    static member SetSubType eventSubType =
        Some eventSubType |> CaseSubType
    /// Clear the SubType of this event
    static member ClearSubType =
        CaseSubType None
    /// Set the externalId of event. Must be unique within the project
    static member SetExternalId id =
        CaseExternalId id
    /// Clear the externalId of event.
    static member ClearExternalId =
        CaseExternalId None

/// The functional event update core module
[<RequireQualifiedAccess>]
module Update =
    [<Literal>]
    let Url = "/events/update"

    let renderUpdateFields (arg: EventUpdate) =
        match arg with
        | CaseStartTime startTime ->
            "startTime", Encode.object [
                match startTime with
                | Some stTime -> yield "set", Encode.int64 stTime
                | None -> yield "setNull", Encode.bool true
            ]
        | CaseEndTime endTime ->
            "endTime", Encode.object [
                match endTime with
                | Some edTime -> yield "set", Encode.int64 edTime
                | None -> yield "setNull", Encode.bool true
            ]
        | CaseDescription optDesc ->
            "description", Encode.object [
                match optDesc with
                | Some desc -> yield "set", Encode.string desc
                | None -> yield "setNull", Encode.bool true
            ]
        | CaseMetaData optMeta ->
            "metadata", Encode.object [
                match optMeta with
                | Some (Set data) ->
                    yield "set", Encode.propertyBag data
                | Some (Change data) ->
                    if data.Add.IsSome then yield "add", Encode.propertyBag data.Add.Value
                    yield "remove", Encode.seq (Seq.map Encode.string data.Remove)
                | None -> yield "set", Encode.object []
            ]
        | CaseEventIds optAssetIds ->
            match optAssetIds with
            | Some ids ->
                "assetIds", Encode.object [
                    yield "set", Encode.int53list ids
                ]
            | None -> "set", Encode.object []
        | CaseSource optSource ->
            "source", Encode.object [
                match optSource with
                | Some source -> yield "set", Encode.string source
                | None -> yield "setNull", Encode.bool true
            ]
        | CaseType optType ->
            "type", Encode.object [
                match optType with
                | Some evType -> yield "set", Encode.string evType
                | None -> yield "setNull", Encode.bool true
            ]
        | CaseSubType optSubType ->
            "type", Encode.object [
                match optSubType with
                | Some evSubType -> yield "set", Encode.string evSubType
                | None -> yield "setNull", Encode.bool true
            ]
        | CaseExternalId optExternalId ->
            "externalId", Encode.object [
                match optExternalId with
                | Some externalId -> yield "set", Encode.string externalId
                | None -> yield "setNull", Encode.bool true
            ]



    type private EventUpdateRequest = {
        Id: Identity
        Params: EventUpdate seq
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

    type private EventsUpdateRequest = {
        Items: EventUpdateRequest seq
    } with
        member this.Encoder =
            Encode.object [
                "items", Seq.map (fun (item:EventUpdateRequest) -> item.Encoder) this.Items |> Encode.seq
            ]

    type EventResponse = {
        Items: EventReadDto seq
    } with
         static member Decoder : Decoder<EventResponse> =
            Decode.object (fun get -> {
                Items = get.Required.Field "items" (Decode.list EventReadDto.Decoder |> Decode.map seq)
            })

    let updateCore (args: (Identity * EventUpdate list) list) (fetch: HttpHandler<HttpResponseMessage, 'a>) =
        let decodeResponse = Decode.decodeResponse EventResponse.Decoder (fun res -> res.Items)
        let request : EventsUpdateRequest = {
            Items = [
                yield! args |> Seq.map(fun (eventId, args) -> { Id = eventId; Params = args })
            ]
        }

        POST
        >=> setVersion V10
        >=> setContent (Content.JsonValue request.Encoder)
        >=> setResource Url
        >=> fetch
        >=> decodeResponse

    /// <summary>
    /// Update one or more events. Supports partial updates, meaning that fields omitted from the requests are not changed
    /// </summary>
    /// <param name="events">The list of events to update.</param>
    /// <param name="next">Async handler to use.</param>
    /// <returns>List of updated events.</returns>
    let update (events: (Identity * (EventUpdate list)) list) (next: NextFunc<EventReadDto seq,'a>)  : HttpContext -> Task<Context<'a>> =
        updateCore events fetch next

    /// <summary>
    /// Update one or more events. Supports partial updates, meaning that fields omitted from the requests are not changed
    /// </summary>
    /// <param name="events">The list of events to update.</param>
    /// <returns>List of updated events.</returns>
    let updateAsync (events: (Identity * EventUpdate list) list) : HttpContext -> Task<Context<EventReadDto seq>> =
        updateCore events fetch Task.FromResult

[<Extension>]
type UpdateEventsClientExtensions =
    /// <summary>
    /// Update one or more events. Supports partial updates, meaning that fields omitted from the requests are not changed
    /// </summary>
    /// <param name="events">The list of events to update.</param>
    /// <returns>List of updated events.</returns>
    [<Extension>]
    static member UpdateAsync (this: ClientExtension, events: ValueTuple<Identity, EventUpdate seq> seq, [<Optional>] token: CancellationToken) : Task<EventEntity seq> =
        task {
            let events' = events |> Seq.map (fun struct (id, options) -> (id, options |> List.ofSeq)) |> List.ofSeq
            let! ctx = Update.updateAsync events' this.Ctx
            match ctx.Result with
            | Ok response ->
                return response |> Seq.map (fun event -> event.ToEventEntity ())
            | Error error ->
                return raise (error.ToException ())
        }

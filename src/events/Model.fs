namespace CogniteSdk.Events

open System.Collections.Generic
open Oryx

/// Read/write event type.
type EventEntity internal (externalId: string, startTime: int64, endTime: int64, eventType: string, eventSubType: string, description: string, metadata: IDictionary<string, string>, assetIds: IEnumerable<int64>, source: string, id: int64, createdTime: int64, lastUpdatedTime: int64) =

    member val ExternalId : string = externalId with get, set
    member val StartTime : int64 = startTime with get, set
    member val EndTime : int64 = endTime with get, set
    member val Type : string = eventType with get, set
    member val SubType : string = eventSubType with get, set
    member val Description : string = description with get, set
    member val MetaData : IDictionary<string, string> = metadata with get, set
    member val AssetIds : IEnumerable<int64> = assetIds with get, set
    member val Source : string = source with get, set

    member val Id : int64 = id with get
    member val CreatedTime : int64 = createdTime with get
    member val LastUpdatedTime : int64 = lastUpdatedTime with get

    // Create new Event.
    new () =
        EventEntity(externalId=null, startTime=0L, endTime=0L, eventType=null, eventSubType=null, description=null, metadata=null, assetIds=null, source=null, id=0L, createdTime=0L, lastUpdatedTime=0L)
    // Create new Event.
    new (externalId: string, startTime: int64, endTime: int64, eventType: string, eventSubType: string, description: string, metadata: IDictionary<string, string>, assetIds: IEnumerable<int64>, source: string) =
        EventEntity(externalId=externalId, startTime=startTime, endTime=endTime, eventType=eventType, eventSubType=eventSubType, description=description, metadata=metadata, assetIds=assetIds, source=source, id=0L, createdTime=0L, lastUpdatedTime=0L)

/// Event type for responses.
type EventReadDto = {
    /// External Id provided by client. Should be unique within the project
    ExternalId: string option
    /// The number of milliseconds since 00:00:00 Thursday, 1 January 1970, Coordinated Universal Time (UTC), minus leap seconds.
    StartTime : int64 option
    /// The number of milliseconds since 00:00:00 Thursday, 1 January 1970, Coordinated Universal Time (UTC), minus leap seconds.
    EndTime : int64 option
    /// Type of the event, e.g 'failure'.
    Type : string option
    /// Subtype of the event, e.g 'electrical'.
    SubType : string option
    /// Textual description of the event.
    Description : string option
    /// Custom, application specific metadata. String key -> String value. Limits: Maximum length of key is 32 bytes, value 512 bytes, up to 16 key-value pairs.
    MetaData : Map<string, string>
    /// Asset IDs of related equipment that this event relates to.
    AssetIds : int64 list
    /// The source of this event.
    Source : string option
    /// The Id of the event.
    Id : int64
    /// The number of milliseconds since 00:00:00 Thursday, 1 January 1970, Coordinated Universal Time (UTC), minus leap seconds.
    CreatedTime : int64
    /// The number of milliseconds since 00:00:00 Thursday, 1 January 1970, Coordinated Universal Time (UTC), minus leap seconds.
    LastUpdatedTime : int64 
} with
    /// Translates the domain type to a plain old crl object
    member this.ToEvent () : EventEntity =
        let externalId = if this.ExternalId.IsSome then this.ExternalId.Value else Unchecked.defaultof<string>
        let startTime = if this.StartTime.IsSome then this.StartTime.Value else Unchecked.defaultof<int64>
        let endTime = if this.EndTime.IsSome then this.EndTime.Value else Unchecked.defaultof<int64>
        let eventType = if this.Type.IsSome then this.Type.Value else Unchecked.defaultof<string>
        let eventSubType =if this.SubType.IsSome then this.SubType.Value else Unchecked.defaultof<string>
        let description = if this.Description.IsSome then this.Description.Value else Unchecked.defaultof<string>
        let metadata = this.MetaData |> Map.toSeq |> dict
        let assetIds = this.AssetIds |> List.ofSeq
        let source = if this.Source.IsSome then this.Source.Value else Unchecked.defaultof<string>
        EventEntity(
            externalId = externalId,
            startTime = startTime,
            endTime = endTime,
            eventType = eventType,
            eventSubType = eventSubType,
            description = description,
            metadata = metadata,
            assetIds = assetIds,
            source = source
        )

/// Event type for create requests.
type EventWriteDto = {
    /// External Id provided by client. Should be unique within the project
    ExternalId: string option
    /// The number of milliseconds since 00:00:00 Thursday, 1 January 1970, Coordinated Universal Time (UTC), minus leap seconds.
    StartTime : int64 option
    /// The number of milliseconds since 00:00:00 Thursday, 1 January 1970, Coordinated Universal Time (UTC), minus leap seconds.
    EndTime : int64 option
    /// Type of the event, e.g 'failure'.
    Type : string option
    /// Subtype of the event, e.g 'electrical'.
    SubType : string option
    /// Textual description of the event.
    Description : string option
    /// Custom, application specific metadata. String key -> String value. Limits: Maximum length of key is 32 bytes, value 512 bytes, up to 16 key-value pairs.
    MetaData : Map<string, string>
    /// Asset IDs of related equipment that this event relates to.
    AssetIds : int64 seq
    /// The source of this event.
    Source : string option
} with
    static member FromEvent (event: EventEntity) : EventWriteDto =
        let metaData =
            if not (isNull event.MetaData) then
                event.MetaData |> Seq.map (|KeyValue|) |> Map.ofSeq
            else
                Map.empty
        let assetIds =
            if not (isNull event.AssetIds) then
                event.AssetIds |> Seq.toList
            else
                List.Empty
        {
            ExternalId = if isNull event.ExternalId then None else Some event.ExternalId
            StartTime = if  event.StartTime = 0L then None else Some event.StartTime
            EndTime = if  event.EndTime = 0L then None else Some event.EndTime
            Type = if isNull event.Type then None else Some event.Type
            SubType = if isNull event.SubType then None else Some event.SubType
            Description = if isNull event.Description then None else Some event.Description
            MetaData = metaData
            AssetIds = assetIds
            Source = if isNull event.Source then None else Some event.Source
        }

type ClientExtension internal (context: HttpContext) =
    member internal __.Ctx =
        context

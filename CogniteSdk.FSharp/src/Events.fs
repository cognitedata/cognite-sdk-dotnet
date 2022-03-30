namespace CogniteSdk.FSharp

open System

open Oryx
open Oryx.Cognite
open System.Collections.Generic

type EventFilter =
    { StartTime: TimeRange option
      EndTime: TimeFilter option
      ActiveAtTime: TimeRange option
      MetaData: Map<string, string>
      AssetIds: int64 list
      AssetExternalIds: string list
      RootAssetIds: Identifier list
      AssetSubtreeIds: Identifier list
      DataSetIds: Identifier list
      Source: string option
      Type: string option
      SubType: string option
      CreatedTime: TimeRange option
      LastUpdatedTime: TimeRange option
      ExternalIdPrefix: string option }

    member x.ToEventFilter() =
        let filter = CogniteSdk.EventFilter()

        x.StartTime
        |> Option.iter (fun x -> filter.StartTime <- x.ToTimeRange())

        x.EndTime
        |> Option.iter (fun x -> filter.EndTime <- x.ToTimeRange())

        x.ActiveAtTime
        |> Option.iter (fun x -> filter.ActiveAtTime <- x.ToTimeRange())

        x.ExternalIdPrefix
        |> Option.iter (fun x -> filter.ExternalIdPrefix <- x)

        x.Type |> Option.iter (fun x -> filter.Type <- x)

        x.SubType
        |> Option.iter (fun x -> filter.Subtype <- x)

        x.Source
        |> Option.iter (fun x -> filter.Source <- x)

        if not (List.isEmpty x.AssetIds) then
            filter.AssetIds <- x.AssetIds |> Array.ofList

        if not (List.isEmpty x.AssetExternalIds) then
            filter.AssetExternalIds <- x.AssetExternalIds |> Array.ofList

        if not (List.isEmpty x.AssetSubtreeIds) then
            filter.AssetSubtreeIds <-
                x.AssetSubtreeIds
                |> List.map (fun x -> x.ToIdentifier())
                |> Array.ofList

        if not (List.isEmpty x.RootAssetIds) then
            filter.RootAssetIds <-
                x.RootAssetIds
                |> List.map (fun x -> x.ToIdentifier())
                |> Array.ofList

        if not (List.isEmpty x.DataSetIds) then
            filter.DataSetIds <-
                x.DataSetIds
                |> List.map (fun x -> x.ToIdentifier())
                |> Array.ofList

        x.CreatedTime
        |> Option.iter (fun x -> filter.CreatedTime <- x.ToTimeRange())

        x.LastUpdatedTime
        |> Option.iter (fun x -> filter.LastUpdatedTime <- x.ToTimeRange())

        if not (Map.isEmpty x.MetaData) then
            filter.Metadata <- x.MetaData |> Dictionary

        filter

    /// Merge with other filter. Optional values are merged using `Option.orElse`.
    /// Collections are merged to the union of the two collections.
    member x.MergeWith(other: EventFilter option) : EventFilter =
        match other with
        | None -> x
        | Some other ->
            { StartTime = x.StartTime |> Option.orElse other.StartTime
              EndTime = x.EndTime |> Option.orElse other.EndTime
              ActiveAtTime = x.ActiveAtTime |> Option.orElse other.ActiveAtTime

              Source = x.Source |> Option.orElse other.Source
              Type = x.Type |> Option.orElse other.Type
              SubType = x.SubType |> Option.orElse other.SubType

              AssetIds = set (x.AssetIds @ other.AssetIds) |> List.ofSeq

              RootAssetIds =
                  set (x.RootAssetIds @ other.RootAssetIds)
                  |> List.ofSeq
              AssetExternalIds =
                set (x.AssetExternalIds @ other.AssetExternalIds)
                |> List.ofSeq
              AssetSubtreeIds =
                set (x.AssetSubtreeIds @ other.AssetSubtreeIds)
                |> List.ofSeq
              DataSetIds =
                set (x.DataSetIds @ other.DataSetIds)
                |> List.ofSeq
              MetaData =
                Map.toList x.MetaData @ Map.toList other.MetaData
                |> Map.ofList

              CreatedTime = x.CreatedTime |> Option.orElse other.CreatedTime
              LastUpdatedTime =
                x.LastUpdatedTime
                |> Option.orElse other.LastUpdatedTime
              ExternalIdPrefix =
                x.ExternalIdPrefix
                |> Option.orElse other.ExternalIdPrefix }

    static member Empty =
        { StartTime = None
          EndTime = None
          ActiveAtTime = None
          MetaData = Map.empty
          AssetIds = []
          AssetExternalIds = []
          RootAssetIds = []
          AssetSubtreeIds = []
          DataSetIds = []
          Source = None
          Type = None
          SubType = None
          CreatedTime = None
          LastUpdatedTime = None
          ExternalIdPrefix = None }

type EventQuery =
    { Filter: EventFilter option
      Limit: int32 option
      Cursor: string option
      Partition: string option
      Sort: string list }

    member x.ToEventQuery() =
        let query = CogniteSdk.EventQuery()

        x.Filter
        |> Option.map (fun x -> x.ToEventFilter())
        |> Option.iter (fun x -> query.Filter <- x)

        x.Limit |> Option.iter (fun x -> query.Limit <- x)

        x.Partition
        |> Option.iter (fun x -> query.Partition <- x)

        x.Cursor
        |> Option.iter (fun x -> query.Cursor <- x)

        if not (List.isEmpty x.Sort) then
            query.Sort <- x.Sort |> Array.ofList

        query

    static member Empty =
        { Filter = None
          Limit = None
          Cursor = None
          Partition = None
          Sort = [] }

type EventsSearch =
    { Filter: EventFilter option
      Limit: int option
      Search: SearchDescription }

    member x.ToEventSearch() =
        let search = CogniteSdk.EventSearch()

        x.Filter
        |> Option.iter (fun x -> search.Filter <- x.ToEventFilter())

        x.Limit
        |> Option.iter (fun x -> search.Limit <- Nullable(x))

        search.Search <- x.Search.ToSearch()

        search


module Events =
    let list (query: EventQuery) (source: HttpHandler<unit>) =
        match query.Limit with
        | Some x when x = 0 -> Common.emptyQuery<CogniteSdk.Event>
        | _ ->
            let query = query.ToEventQuery()
            source |> Events.list query

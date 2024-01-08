namespace CogniteSdk.FSharp

open System
open System.Collections.Generic

open Oryx
open Oryx.Cognite

type TimeSeriesFilter =
    { Name: string option
      Unit: string option
      UnitExternalId: string option
      UnitQuantity: string option
      IsString: bool option
      IsStep: bool option
      AssetIds: int64 list
      AssetExternalIds: string list
      AssetSubtreeIds: Identifier list
      RootAssetIds: int64 list
      DataSetIds: Identifier list
      MetaData: Map<string, string>
      CreatedTime: TimeRange option
      LastUpdatedTime: TimeRange option
      ExternalIdPrefix: string option
      Labels: LabelFilter option }

    member x.ToTimeSeriesFilter() =
        let filter = CogniteSdk.TimeSeriesFilter()

        x.Name |> Option.iter (fun x -> filter.Name <- x)

        x.ExternalIdPrefix |> Option.iter (fun x -> filter.ExternalIdPrefix <- x)

        x.Unit |> Option.iter (fun x -> filter.Unit <- x)

        x.UnitExternalId |> Option.iter (fun x -> filter.UnitExternalId <- x)

        x.UnitQuantity |> Option.iter (fun x -> filter.UnitQuantity <- x)

        x.IsStep |> Option.iter (fun x -> filter.IsStep <- x)

        x.IsString |> Option.iter (fun x -> filter.IsString <- x)

        if not (List.isEmpty x.AssetIds) then
            filter.AssetIds <- x.AssetIds |> Array.ofList

        if not (List.isEmpty x.RootAssetIds) then
            filter.RootAssetIds <- x.RootAssetIds |> Array.ofList

        if not (List.isEmpty x.AssetExternalIds) then
            filter.AssetExternalIds <- x.AssetExternalIds |> Array.ofList

        if not (List.isEmpty x.AssetSubtreeIds) then
            filter.AssetSubtreeIds <- x.AssetSubtreeIds |> List.map (fun x -> x.ToIdentifier()) |> Array.ofList

        if not (List.isEmpty x.DataSetIds) then
            filter.DataSetIds <- x.DataSetIds |> List.map (fun x -> x.ToIdentifier()) |> Array.ofList

        x.CreatedTime |> Option.iter (fun x -> filter.CreatedTime <- x.ToTimeRange())

        x.LastUpdatedTime
        |> Option.iter (fun x -> filter.LastUpdatedTime <- x.ToTimeRange())

        if not (Map.isEmpty x.MetaData) then
            filter.Metadata <- x.MetaData |> Dictionary

        filter

    /// Merge with other filter. Optional values are merged using `Option.orElse`.
    /// Collections are merged to the union of the two collections.
    member x.MergeWith(other: TimeSeriesFilter option) : TimeSeriesFilter =
        match other with
        | None -> x
        | Some other ->
            { Name = x.Name |> Option.orElse other.Name
              AssetIds = set (x.AssetIds @ other.AssetIds) |> List.ofSeq
              RootAssetIds = set (x.RootAssetIds @ other.RootAssetIds) |> List.ofSeq
              AssetExternalIds = set (x.AssetExternalIds @ other.AssetExternalIds) |> List.ofSeq
              AssetSubtreeIds = set (x.AssetSubtreeIds @ other.AssetSubtreeIds) |> List.ofSeq
              DataSetIds = set (x.DataSetIds @ other.DataSetIds) |> List.ofSeq
              MetaData = Map.toList x.MetaData @ Map.toList other.MetaData |> Map.ofList
              Unit = x.Unit |> Option.orElse other.Unit
              UnitExternalId = x.UnitExternalId |> Option.orElse other.UnitExternalId
              UnitQuantity = x.UnitQuantity |> Option.orElse other.UnitQuantity
              CreatedTime = x.CreatedTime |> Option.orElse other.CreatedTime
              LastUpdatedTime = x.LastUpdatedTime |> Option.orElse other.LastUpdatedTime
              IsStep = x.IsStep |> Option.orElse other.IsStep
              IsString = x.IsString |> Option.orElse other.IsString
              ExternalIdPrefix = x.ExternalIdPrefix |> Option.orElse other.ExternalIdPrefix
              Labels = x.Labels |> Option.orElse other.Labels }

    static member Empty =
        { Name = None
          AssetIds = []
          AssetExternalIds = []
          AssetSubtreeIds = []
          DataSetIds = []
          MetaData = Map.empty
          Unit = None
          UnitExternalId = None
          UnitQuantity = None
          IsStep = None
          IsString = None
          CreatedTime = None
          LastUpdatedTime = None
          RootAssetIds = []
          ExternalIdPrefix = None
          Labels = None }

type TimeSeriesQuery =
    { Filter: TimeSeriesFilter option
      Limit: int32 option
      Cursor: string option
      Partition: string option }

    member x.ToTimeSeriesQuery() =
        let query = CogniteSdk.TimeSeriesQuery()

        x.Filter
        |> Option.map (fun x -> x.ToTimeSeriesFilter())
        |> Option.iter (fun x -> query.Filter <- x)

        x.Limit |> Option.iter (fun x -> query.Limit <- x)

        x.Partition |> Option.iter (fun x -> query.Partition <- x)

        x.Cursor |> Option.iter (fun x -> query.Cursor <- x)

        query

    static member Empty =
        { Filter = None
          Limit = None
          Cursor = None
          Partition = None }

type TimeSeriesSearch =
    { Filter: TimeSeriesFilter option
      Limit: int option
      Search: Search }

    member x.ToTimeSeriesSearch() =
        let search = CogniteSdk.TimeSeriesSearch()

        x.Filter |> Option.iter (fun x -> search.Filter <- x.ToTimeSeriesFilter())

        x.Limit |> Option.iter (fun x -> search.Limit <- Nullable(x))

        search.Search <- x.Search.ToSearch()

        search


module TimeSeries =
    let list (query: TimeSeriesQuery) (source: HttpHandler<unit>) =
        match query.Limit with
        | Some x when x = 0 -> Common.emptyQuery<CogniteSdk.TimeSeries>
        | _ ->
            let query = query.ToTimeSeriesQuery()
            source |> TimeSeries.list query

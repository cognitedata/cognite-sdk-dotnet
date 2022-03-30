namespace CogniteSdk.FSharp

open System
open System.Collections.Generic

open Oryx
open Oryx.Cognite

type AssetFilter =
    { Name: string option
      ParentExternalIds: string list
      AssetSubtreeIds: Identifier list
      DataSetIds: Identifier list
      MetaData: Map<string, string>
      Source: string option
      CreatedTime: TimeRange option
      LastUpdatedTime: TimeRange option
      Root: bool option
      ExternalIdPrefix: string option
      Labels: LabelFilter option }

    member x.ToAssetFilter() =
        let filter = CogniteSdk.AssetFilter()

        x.Name |> Option.iter (fun x -> filter.Name <- x)

        x.ExternalIdPrefix
        |> Option.iter (fun x -> filter.ExternalIdPrefix <- x)

        x.Source
        |> Option.iter (fun x -> filter.Source <- x)

        x.Root |> Option.iter (fun x -> filter.Root <- x)

        if not (List.isEmpty x.ParentExternalIds) then
            filter.ParentExternalIds <- x.ParentExternalIds |> Array.ofList

        if not (List.isEmpty x.AssetSubtreeIds) then
            filter.AssetSubtreeIds <-
                x.AssetSubtreeIds
                |> List.map (fun x -> x.ToIdentifier())
                |> Array.ofList

        if not (List.isEmpty x.DataSetIds) then
            filter.DataSetIds <-
                x.DataSetIds
                |> List.map (fun x -> x.ToIdentifier())
                |> Array.ofList

        x.Labels
        |> Option.iter (fun x -> filter.Labels <- x.ToLabelFilter())

        x.CreatedTime
        |> Option.iter (fun x -> filter.CreatedTime <- x.ToTimeRange())

        x.LastUpdatedTime
        |> Option.iter (fun x -> filter.LastUpdatedTime <- x.ToTimeRange())

        if not (Map.isEmpty x.MetaData) then
            filter.Metadata <- x.MetaData |> Dictionary

        filter

    /// Merge with other filter. Optional values are merged using `Option.orElse`.
    /// Collections are merged to the union of the two collections.
    member x.MergeWith(other: AssetFilter option) : AssetFilter =
        match other with
        | None -> x
        | Some other ->
            { Name = x.Name |> Option.orElse other.Name
              ParentExternalIds =
                set (x.ParentExternalIds @ other.ParentExternalIds)
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
              Source = x.Source |> Option.orElse other.Source
              CreatedTime = x.CreatedTime |> Option.orElse other.CreatedTime
              LastUpdatedTime =
                x.LastUpdatedTime
                |> Option.orElse other.LastUpdatedTime
              Root = x.Root |> Option.orElse other.Root
              ExternalIdPrefix =
                x.ExternalIdPrefix
                |> Option.orElse other.ExternalIdPrefix
              Labels = x.Labels |> Option.orElse other.Labels }

    static member Empty =
        { Name = None
          ParentExternalIds = []
          AssetSubtreeIds = []
          DataSetIds = []
          MetaData = Map.empty
          Source = None
          CreatedTime = None
          LastUpdatedTime = None
          Root = None
          ExternalIdPrefix = None
          Labels = None }

type AssetQuery =
    { Filter: AssetFilter option
      Limit: int32 option
      Cursor: string option
      AggregatedProperties: string list
      Partition: string option }

    member x.ToAssetQuery() =
        let query = CogniteSdk.AssetQuery()

        x.Filter
        |> Option.map (fun x -> x.ToAssetFilter())
        |> Option.iter (fun x -> query.Filter <- x)

        x.Limit |> Option.iter (fun x -> query.Limit <- x)

        x.Partition
        |> Option.iter (fun x -> query.Partition <- x)

        x.Cursor
        |> Option.iter (fun x -> query.Cursor <- x)

        if not (List.isEmpty x.AggregatedProperties) then
            query.AggregatedProperties <- x.AggregatedProperties |> Array.ofList

        query

    static member Empty =
        { Filter = None
          Limit = None
          Cursor = None
          AggregatedProperties = []
          Partition = None }

type AssetSearch =
    { Filter: AssetFilter option
      Limit: int option
      Search: Search }

    member x.ToAssetSearch() =
        let search = CogniteSdk.AssetSearch()

        x.Filter
        |> Option.iter (fun x -> search.Filter <- x.ToAssetFilter())

        x.Limit
        |> Option.iter (fun x -> search.Limit <- Nullable(x))

        search.Search <- x.Search.ToSearch()

        search


module Assets =
    let list (query: AssetQuery) (source: HttpHandler<unit>) =
        match query.Limit with
        | Some x when x = 0 -> Common.emptyQuery<CogniteSdk.Asset>
        | _ ->
            let query = query.ToAssetQuery()
            source |> Oryx.Cognite.Assets.list query

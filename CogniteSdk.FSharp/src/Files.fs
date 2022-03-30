namespace CogniteSdk.FSharp

open System
open System.Collections.Generic

open Oryx
open Oryx.Cognite

type FileFilter =
    { Name: string option
      DirectoryPrefix: string option
      MimeType: string option
      MetaData: Map<string, string>
      AssetIds: int64 list
      AssetExternalIds: string list
      RootAssetIds: Identifier list
      DataSetIds: Identifier list
      AssetSubtreeIds: Identifier list
      Source: string option
      CreatedTime: TimeRange option
      LastUpdatedTime: TimeRange option
      UploadedTime: TimeRange option
      SourceCreatedTime: TimeRange option
      SourceModifiedTime: TimeRange option
      ExternalIdPrefix: string option
      Uploaded: bool option
      // TODO: GeoLocation
      Labels: LabelFilter option }

    member x.ToFileFilter() =
        let filter = CogniteSdk.FileFilter()

        x.Name |> Option.iter (fun x -> filter.Name <- x)

        x.DirectoryPrefix
        |> Option.iter (fun x -> filter.DirectoryPrefix <- x)

        x.ExternalIdPrefix
        |> Option.iter (fun x -> filter.ExternalIdPrefix <- x)

        x.MimeType
        |> Option.iter (fun x -> filter.MimeType <- x)

        x.Source
        |> Option.iter (fun x -> filter.Source <- x)

        x.Uploaded
        |> Option.iter (fun x -> filter.Uploaded <- x)

        if not (List.isEmpty x.AssetIds) then
            filter.AssetIds <- x.AssetIds |> Array.ofList

        if not (List.isEmpty x.RootAssetIds) then
            filter.RootAssetIds <-
                x.RootAssetIds
                |> List.map (fun x -> x.ToIdentifier())
                |> Array.ofList

        if not (List.isEmpty x.AssetExternalIds) then
            filter.AssetExternalIds <- x.AssetExternalIds |> Array.ofList

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


        x.CreatedTime
        |> Option.iter (fun x -> filter.CreatedTime <- x.ToTimeRange())

        x.LastUpdatedTime
        |> Option.iter (fun x -> filter.LastUpdatedTime <- x.ToTimeRange())

        x.UploadedTime
        |> Option.iter (fun x -> filter.UploadedTime <- x.ToTimeRange())

        x.SourceCreatedTime
        |> Option.iter (fun x -> filter.SourceCreatedTime <- x.ToTimeRange())

        x.SourceModifiedTime
        |> Option.iter (fun x -> filter.SourceModifiedTime <- x.ToTimeRange())

        x.Labels
        |> Option.iter (fun x -> filter.Labels <- x.ToLabelFilter())

        if not (Map.isEmpty x.MetaData) then
            filter.Metadata <- x.MetaData |> Dictionary

        filter

    member x.Merge(other: FileFilter option) : FileFilter =
        match other with
        | None -> x
        | Some other ->
            { Name = x.Name |> Option.orElse other.Name
              MimeType = x.MimeType |> Option.orElse other.MimeType
              DirectoryPrefix =
                x.DirectoryPrefix
                |> Option.orElse other.DirectoryPrefix
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
              Source = x.Source |> Option.orElse other.Source
              CreatedTime = x.CreatedTime |> Option.orElse other.CreatedTime
              LastUpdatedTime =
                x.LastUpdatedTime
                |> Option.orElse other.LastUpdatedTime

              SourceCreatedTime =
                  x.SourceCreatedTime
                  |> Option.orElse other.SourceCreatedTime
              SourceModifiedTime =
                x.SourceModifiedTime
                |> Option.orElse other.SourceModifiedTime

              UploadedTime = x.UploadedTime |> Option.orElse other.UploadedTime

              Uploaded = x.Uploaded |> Option.orElse other.Uploaded
              ExternalIdPrefix =
                x.ExternalIdPrefix
                |> Option.orElse other.ExternalIdPrefix
              Labels = x.Labels |> Option.orElse other.Labels }

    static member Empty: FileFilter =
        { Name = None
          DirectoryPrefix = None
          MimeType = None
          MetaData = Map.empty
          AssetIds = []
          AssetExternalIds = []
          RootAssetIds = []
          DataSetIds = []
          AssetSubtreeIds = []
          Source = None
          CreatedTime = None
          LastUpdatedTime = None
          UploadedTime = None
          SourceCreatedTime = None
          SourceModifiedTime = None
          ExternalIdPrefix = None
          Uploaded = None
          // TODO: GeoLocation
          Labels = None }

type FileQuery =
    { Filter: FileFilter option
      Limit: int32 option
      Cursor: string option
      Partition: string option }

    member x.ToFileQuery() =
        let query = CogniteSdk.FileQuery()

        x.Filter
        |> Option.map (fun x -> x.ToFileFilter())
        |> Option.iter (fun x -> query.Filter <- x)

        x.Limit |> Option.iter (fun x -> query.Limit <- x)

        // TODO: fix when available in the SDK
        // x.Partition
        // |> Option.iter (fun x -> query.Partition <- x)

        x.Cursor
        |> Option.iter (fun x -> query.Cursor <- x)

        query

    static member Empty =
        { Filter = None
          Limit = None
          Cursor = None
          Partition = None }

type FileSearch =
    { Filter: FileFilter option
      Limit: int option
      Search: SearchName }

    member x.ToFileSearch() =
        let search = CogniteSdk.FileSearch()

        x.Filter
        |> Option.iter (fun x -> search.Filter <- x.ToFileFilter())

        x.Limit
        |> Option.iter (fun x -> search.Limit <- Nullable(x))

        search.Search <- x.Search.ToSearch()

        search


module Files =
    let list (query: FileQuery) (source: HttpHandler<unit>) =
        match query.Limit with
        | Some x when x = 0 -> Common.emptyQuery<CogniteSdk.File>
        | _ ->
            let query = query.ToFileQuery()
            source |> Files.list query

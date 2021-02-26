// Copyright 2020 Cognite AS
// SPDX-License-Identifier: Apache-2.0

namespace Oryx.Cognite

open System
open System.Collections.Generic
open System.Net.Http

open Oryx
open Oryx.Cognite

open CogniteSdk

/// Various event HTTP handlers.

[<RequireQualifiedAccess>]
module Files =
    [<Literal>]
    let Url = "/files"

    /// Retrieves information about a file given a file id.
    let get (fileId: int64) : HttpHandler<unit, File> =
        withLogMessage "Files:get"
        >=> (Url +/ sprintf "%d" fileId |> getV10)
    /// Retrieves list of files matching filter, and a cursor if given limit is exceeded. Returns list of files matching given filters and optional cursor</returns>
    let list (query: FileQuery) : HttpHandler<unit, ItemsWithCursor<File>> =
        withLogMessage "Files:list"
        >=> list query Url

    /// Retrieves number of files matching filter. Returns number of files matching given filters</returns>
    let aggregate (query: FileQuery) : HttpHandler<unit, int32> =
        withLogMessage "Files:aggregate"
        >=> aggregate query Url

    /// Upload new file in the given project.
    let upload (file: FileCreate) (overwrite: bool) : HttpHandler<unit, FileUploadRead> =
        withLogMessage "Files:upload"
        >=> postV10 file Url

    /// Get download URL for file in the given project.
    let download (ids: Identity seq) : HttpHandler<unit, IEnumerable<FileDownload>> =
        withLogMessage "Files:download"
        >=> req {
            let url = Url +/ "downloadlink"
            let request = ItemsWithoutCursor<Identity>(Items = ids)
            let! ret =
                withCompletion HttpCompletionOption.ResponseHeadersRead
                >=> postV10<ItemsWithoutCursor<Identity>, ItemsWithoutCursor<FileDownload>> request url
            return ret.Items
        }

    let retrieve (ids: Identity seq) (ignoreUnknownIds: Nullable<bool>) : HttpHandler<unit, IEnumerable<File>> =
        withLogMessage "Files:retrieve"
        >=> retrieveIgnoreUnkownIds ids (Option.ofNullable ignoreUnknownIds) Url

    let search (query: FileSearch) : HttpHandler<unit, File seq> =
        withLogMessage "Files:search"
        >=> search query Url

    let delete (files: ItemsWithoutCursor<Identity>) : HttpHandler<unit, EmptyResponse> =
        withLogMessage "Files:delete"
        >=> delete files Url

    /// Update one or more assets. Supports partial updates, meaning that fields omitted from the requests are not changed. Returns list of updated assets.
    let update (query: IEnumerable<UpdateItem<FileUpdate>>) : HttpHandler<unit, File seq>  =
        withLogMessage "Files:update"
        >=> update query Url

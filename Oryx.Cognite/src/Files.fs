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
    let get (fileId: int64) (source: HttpHandler<unit>) : HttpHandler<File> =
        source |> withLogMessage "Files:get" |> (Url +/ sprintf "%d" fileId |> getV10)

    /// Retrieves list of files matching filter, and a cursor if given limit is exceeded. Returns list of files matching given filters and optional cursor</returns>
    let list (query: FileQuery) (source: HttpHandler<unit>) : HttpHandler<ItemsWithCursor<File>> =
        source |> withLogMessage "Files:list" |> list query Url

    /// Retrieves number of files matching filter. Returns number of files matching given filters</returns>
    let aggregate (query: FileQuery) (source: HttpHandler<unit>) : HttpHandler<int32> =
        source |> withLogMessage "Files:aggregate" |> aggregate query Url

    /// Upload new file in the given project.
    let upload (file: FileCreate) (overwrite: bool) (source: HttpHandler<unit>) : HttpHandler<FileUploadRead> =
        source |> withLogMessage "Files:upload" |> postV10 file Url

    let uploadFile(uri: Uri) (fileStream: StreamContent) (source: HttpHandler<unit>) =
        http {
            let! ret =
                source
                |> withCompletion HttpCompletionOption.ResponseHeadersRead
                |> put fileStream uri

            return ret.Items
        }

    /// Get download URL for file in the given project.
    let download (ids: Identity seq) (source: HttpHandler<unit>) : HttpHandler<IEnumerable<FileDownload>> =
        http {
            let url = Url +/ "downloadlink"
            let request = ItemsWithoutCursor<Identity>(Items = ids)

            let! ret =
                source
                |> withLogMessage "Files:download"
                |> withCompletion HttpCompletionOption.ResponseHeadersRead
                |> postV10<ItemsWithoutCursor<Identity>, ItemsWithoutCursor<FileDownload>> request url

            return ret.Items
        }

    let retrieve
        (ids: Identity seq)
        (ignoreUnknownIds: Nullable<bool>)
        (source: HttpHandler<unit>)
        : HttpHandler<IEnumerable<File>> =
        source
        |> withLogMessage "Files:retrieve"
        |> retrieveIgnoreUnknownIds ids (Option.ofNullable ignoreUnknownIds) Url

    let search (query: FileSearch) (source: HttpHandler<unit>) : HttpHandler<File seq> =
        source |> withLogMessage "Files:search" |> search query Url

    let delete (files: ItemsWithoutCursor<Identity>) (source: HttpHandler<unit>) : HttpHandler<EmptyResponse> =
        source |> withLogMessage "Files:delete" |> delete files Url

    /// Update one or more assets. Supports partial updates, meaning that fields omitted from the requests are not changed. Returns list of updated assets.
    let update (query: IEnumerable<UpdateItem<FileUpdate>>) (source: HttpHandler<unit>) : HttpHandler<File seq> =
        source |> withLogMessage "Files:update" |> update query Url

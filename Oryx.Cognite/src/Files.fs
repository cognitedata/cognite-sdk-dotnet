namespace Oryx.Cognite

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
    let get (fileId: int64) : HttpHandler<HttpResponseMessage, File, 'a> =
        logWithMessage "Files:get"
        >=> (Url +/ sprintf "%d" fileId |> getV10)
    /// Retrieves list of files matching filter, and a cursor if given limit is exceeded. Returns list of files matching given filters and optional cursor</returns>
    let list (query: FileQuery) : HttpHandler<HttpResponseMessage, ItemsWithCursor<File>, 'a> =
        logWithMessage "Files:list"
        >=> list query Url

    /// Upload new file in the given project.
    let upload (file: FileCreate) (overwrite: bool) : HttpHandler<HttpResponseMessage, FileUploadRead, 'a> =
        logWithMessage "Files:upload"
        >=> postV10 HttpCompletionOption.ResponseContentRead file Url

    /// Get download URL for file in the given project.
    let download (ids: Identity seq) : HttpHandler<HttpResponseMessage, IEnumerable<FileDownload>, 'a> =
        logWithMessage "Files:download"
        >=> req {
            let url = Url +/ "downloadlink"
            let request = ItemsWithoutCursor<Identity>(Items = ids)
            let! ret = postV10<ItemsWithoutCursor<Identity>, ItemsWithoutCursor<FileDownload>, 'a> HttpCompletionOption.ResponseHeadersRead request url
            return ret.Items
        }  

    let retrieve (ids: Identity seq) : HttpHandler<HttpResponseMessage, IEnumerable<File>, 'a> =
        logWithMessage "Files:retrieve"
        >=> retrieve ids Url

    let search (query: FileSearch) : HttpHandler<HttpResponseMessage, File seq, 'a> =
        logWithMessage "Files:search"
        >=> search query Url

    let delete (files: ItemsWithoutCursor<Identity>) : HttpHandler<HttpResponseMessage, EmptyResponse, 'a> =
        logWithMessage "Files:delete"
        >=> delete files Url

    /// Update one or more assets. Supports partial updates, meaning that fields omitted from the requests are not changed. Returns list of updated assets.
    let update (query: IEnumerable<UpdateItem<FileUpdate>>) : HttpHandler<HttpResponseMessage, File seq, 'a>  =
        logWithMessage "Files:update"
        >=> update query Url

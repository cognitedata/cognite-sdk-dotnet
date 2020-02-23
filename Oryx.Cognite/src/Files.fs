namespace Oryx.Cognite

open System.Net.Http

open Oryx
open Oryx.Cognite

open System.Collections.Generic
open CogniteSdk
open CogniteSdk.Files

/// Various event HTTP handlers.

[<RequireQualifiedAccess>]
module Files =
    [<Literal>]
    let Url = "/files"

    /// Retrieves information about a file given a file id.
    let get (fileId: int64) : HttpHandler<HttpResponseMessage, FileReadDto, 'a> =
        Url +/ sprintf "%d" fileId |> getV10
        >=> logWithMessage "Files:get"

    /// Retrieves list of files matching filter, and a cursor if given limit is exceeded. Returns list of files matching given filters and optional cursor</returns>
    let list (query: FileQueryDto) : HttpHandler<HttpResponseMessage, ItemsWithCursor<FileReadDto>, 'a> =
        list query Url
        >=> logWithMessage "Files:list"

    /// Upload new file in the given project.
    let upload (file: FileWriteDto) (overwrite: bool) : HttpHandler<HttpResponseMessage, FileUploadReadDto, 'a> =
        postV10 file Url
        >=> logWithMessage "Files:upload"

    /// Get download URL for file in the given project.
    let download (ids: Identity seq) : HttpHandler<HttpResponseMessage, IEnumerable<FileDownloadDto>, 'a> =
        req {
            let url = Url +/ "downloadlink"
            let request = ItemsWithoutCursor<Identity>(Items = ids)
            let! ret = postV10<ItemsWithoutCursor<Identity>, ItemsWithoutCursor<FileDownloadDto>, 'a> request url
            return ret.Items
        } >=> logWithMessage "Files:download"

    let retrieve (ids: Identity seq) : HttpHandler<HttpResponseMessage, IEnumerable<FileReadDto>, 'a> =
        retrieve ids Url
        >=> logWithMessage "Files:retrieve"


    let search (query: SearchQueryDto<FileFilterDto, FileSearchDto>) : HttpHandler<HttpResponseMessage, ItemsWithoutCursor<FileReadDto>, 'a> =
        Url +/ "search" |> postV10 query
        >=> logWithMessage "Files:search"

    let delete (files: ItemsWithoutCursor<Identity>) : HttpHandler<HttpResponseMessage, EmptyResponse, 'a> =
        Url +/ "delete" |> postV10 files
        >=> logWithMessage "Files:delete"

    /// Update one or more assets. Supports partial updates, meaning that fields omitted from the requests are not changed. Returns list of updated assets.
    let update (query: IEnumerable<UpdateItem<FileUpdateDto>>) : HttpHandler<HttpResponseMessage, FileReadDto seq, 'a>  =
        update query Url
        >=> logWithMessage "Files:update"

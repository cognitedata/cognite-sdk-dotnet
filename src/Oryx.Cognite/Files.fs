namespace Oryx.Cognite.Files

open System.Net.Http

open Oryx.Cognite

open CogniteSdk
open CogniteSdk.Files

/// Various event HTTP handlers.

[<RequireQualifiedAccess>]
module Files =
    [<Literal>]
    let Url = "/files"

    /// <summary>
    /// Retrieves information about an event given an event id.
    /// </summary>
    /// <param name="eventId">The id of the event to get.</param>
    /// <returns>Event with the given id.</returns>
    let get (fileId: int64) : HttpHandler<HttpResponseMessage, FileReadDto, 'a> =
        Url +/ sprintf "%d" fileId |> get

    /// <summary>
    /// Retrieves list of files matching filter, and a cursor if given limit is exceeded.
    /// </summary>
    /// <param name="query">Query filter</param>
    /// <param name="filters">Search filters</param>
    /// <returns>List of files matching given filters and optional cursor</returns>
    let list (query: FileQueryDto) : HttpHandler<HttpResponseMessage, ItemsWithCursor<FileReadDto>, 'a> =
        post query Url
    
    let download (ids: Identity seq) : HttpHandler<HttpResponseMessage, ItemsWithoutCursor<FileDownloadDto>, 'a> =
        Url +/ "download" |> post ids 

    let retrieve (ids: Identity seq) : HttpHandler<HttpResponseMessage, ItemsWithoutCursor<FileReadDto>, 'a> =
        Url +/ "byids" |> retrieve ids
        
    let search (query: SearchQueryDto<FileFilterDto, FileSearchDto>) : HttpHandler<HttpResponseMessage, ItemsWithoutCursor<FileReadDto>, 'a> =
        Url +/ "search" |> post query 
        
    let delete (files: ItemsWithoutCursor<Identity>) : HttpHandler<HttpResponseMessage, EmptyResponse, 'a> =
        Url +/ "delete" |> post files



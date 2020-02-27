// Copyright 2019 Cognite AS
// SPDX-License-Identifier: Apache-2.0

namespace Oryx.Cognite

open System.Collections.Generic
open System.Net.Http

open Oryx
open Oryx.Cognite

open CogniteSdk
open CogniteSdk.Sequences

/// Various event HTTP handlers.

[<RequireQualifiedAccess>]
module Sequences =
    [<Literal>]
    let Url = "/sequences"
    let RowsUrl = Url +/ "data"

    /// Retrieves list of sequences matching filter, and a cursor if given limit is exceeded.
    let list (query: SequenceQuery) : HttpHandler<HttpResponseMessage, ItemsWithCursor<SequenceRead>, 'a> =
        list query Url
        >=> logWithMessage "Sequences:get"

    /// Processes data requests, and returns the result. NB - This operation uses a dynamic limit on the number of rows returned based on the number and type of columns, use the provided cursor to paginate and retrieve all data.
    let listRows (query: SequenceRowQuery) : HttpHandler<HttpResponseMessage, SequenceDataRead, 'a> =
        Handler.list query RowsUrl
        >=> logWithMessage "Sequences:listRows"

    /// Create new sequences in the given project. Returns list of created sequences.
    let create (items: SequenceWrite seq) : HttpHandler<HttpResponseMessage, SequenceDataRead seq, 'a> =
        create items Url
        >=> logWithMessage "Sequences:create"

    let createRows(items: SequenceDataWrite seq) : HttpHandler<HttpResponseMessage, EmptyResponse, 'a> =
        Handler.createEmpty items RowsUrl
        >=> logWithMessage "Sequences:createRows"

    /// Delete multiple sequences in the same project. Returns empty result.
    let delete (items: Identity seq) : HttpHandler<HttpResponseMessage, EmptyResponse, 'a> =
        let req = ItemsWithoutCursor(Items=items)
        delete req Url
        >=> logWithMessage "Sequences:delete"

    let deleteRows (items: SequenceRowDelete seq) : HttpHandler<HttpResponseMessage, EmptyResponse, 'a> =
        let req = ItemsWithoutCursor(Items=items)
        Handler.delete req RowsUrl
        >=> logWithMessage "Sequences:deleteRows"

    /// Retrieves information about multiple sequences in the same project. A maximum of 1000 event IDs may be listed per
    /// request and all of them must be unique. Returns sequences with given ids.
    let retrieve (ids: Identity seq) : HttpHandler<HttpResponseMessage, SequenceRead seq, 'a> =
        retrieve ids Url
        >=> logWithMessage "Sequences:retrieve"

    /// Retrieves a list of sequences matching the given criteria. This operation does not support pagination. Returns list of sequences matching given criteria.
    let search (query: SequenceSearch) : HttpHandler<HttpResponseMessage, SequenceRead seq, 'a> =
        search query Url
        >=> logWithMessage "Sequences:search"

    /// Update one or more sequences. Supports partial updates, meaning that fields omitted from the requests are not changed. Returns list of updated sequences.</returns>
    let update (query: IEnumerable<UpdateItem<SequenceUpdate>>) : HttpHandler<HttpResponseMessage, SequenceRead seq, 'a>  =
        update query Url
        >=> logWithMessage "Sequences:update"


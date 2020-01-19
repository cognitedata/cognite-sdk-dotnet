// Copyright 2019 Cognite AS
// SPDX-License-Identifier: Apache-2.0

namespace Oryx.Cognite

open System.Net.Http

open Oryx.Cognite

open System.Collections.Generic
open CogniteSdk
open CogniteSdk.Sequences

/// Various event HTTP handlers.

[<RequireQualifiedAccess>]
module Sequences =
    [<Literal>]
    let Url = "/sequences"

    /// Retrieves list of sequences matching filter, and a cursor if given limit is exceeded.
    let list (query: SequenceQueryDto) : HttpHandler<HttpResponseMessage, ItemsWithCursor<SequenceReadDto>, 'a> =
        list query Url

    /// Processes data requests, and returns the result. NB - This operation uses a dynamic limit on the number of rows returned based on the number and type of columns, use the provided cursor to paginate and retrieve all data.
    let listRows (query: SequenceRowQuery) : HttpHandler<HttpResponseMessage, SequenceDataReadDto, 'a> =
        Handler.list query Url
    
    /// Create new sequences in the given project. Returns list of created sequences.
    let create (items: IEnumerable<SequenceWriteDto>) : HttpHandler<HttpResponseMessage, IEnumerable<SequenceReadDto>, 'a> =
        create items Url

    /// Delete multiple sequences in the same project. Returns empty result.
    let delete (items: IEnumerable<Identity>) : HttpHandler<HttpResponseMessage, EmptyResponse, 'a> =
        let req = ItemsWithoutCursor(Items=items)
        delete req Url

    /// Retrieves information about multiple sequences in the same project. A maximum of 1000 event IDs may be listed per
    /// request and all of them must be unique. Returns sequences with given ids.
    let retrieve (ids: Identity seq) : HttpHandler<HttpResponseMessage, IEnumerable<SequenceReadDto>, 'a> =
        retrieve ids Url

    /// Retrieves a list of sequences matching the given criteria. This operation does not support pagination. Returns list of sequences matching given criteria.
    let search (query: SearchQueryDto<SequenceFilterDto, SearchDto>) : HttpHandler<HttpResponseMessage, IEnumerable<SequenceReadDto>, 'a> =
        search query Url

    /// Update one or more sequences. Supports partial updates, meaning that fields omitted from the requests are not changed. Returns list of updated sequences.</returns>
    let update (query: IEnumerable<UpdateItemType<SequenceUpdateDto>>) : HttpHandler<HttpResponseMessage, IEnumerable<SequenceReadDto>, 'a>  =
        update query Url


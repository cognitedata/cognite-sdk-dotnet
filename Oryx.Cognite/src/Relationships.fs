// Copyright 2019 Cognite AS
// SPDX-License-Identifier: Apache-2.0

namespace Oryx.Cognite

open System.Net.Http

open Oryx
open Oryx.Cognite

open System.Collections.Generic
open CogniteSdk
open CogniteSdk.Relationships
open CogniteSdk.Types.Common

/// Various asset HTTP handlers.

[<RequireQualifiedAccess>]
module Relationships =
    [<Literal>]
    let Url = "/relationships"

    /// <summary>
    /// Retrieves list of relationships matching filter, and a cursor if given limit is exceeded
    /// </summary>
    /// <param name="query">The query with limit and cursor.</param>
    /// <returns>List of relationships.</returns>
    let list (query: RelationshipQueryDto) : HttpHandler<HttpResponseMessage, ItemsWithCursor<RelationshipReadDto>, 'a> =
        listPlayground query Url
        >=> logWithMessage "Relationships:list"

    /// <summary>
    /// Create new relationships in the given project.
    /// </summary>
    /// <param name="assets">The relationships to create.</param>
    /// <returns>List of created relationships.</returns>
    let create (items: RelationshipWriteDto seq) : HttpHandler<HttpResponseMessage, ItemsWithoutCursor<RelationshipReadDto>, 'a> =
        createPlayground items Url
        >=> logWithMessage "Relationships:create"

    /// <summary>
    /// Delete multiple relationships in the same project.
    /// </summary>
    /// <param name="relationships">The list of externalIds for relationships to delete.</param>
    /// <returns>Empty result.</returns>
    let delete (externalIds: string seq) : HttpHandler<HttpResponseMessage, EmptyResponse, 'a> =
        let relationships = externalIds |> Seq.map Identity.Create
        let items = ItemsWithoutCursor(Items=relationships)
        deletePlayground items Url
        >=> logWithMessage "Relationships:delete"

    /// <summary>
    /// Retrieves information about multiple relationships in the same project. A maximum of 1000 relationships IDs may be listed per
    /// request and all of them must be unique.
    /// </summary>
    /// <param name="ids">The ids of the relationships to get.</param>
    /// <returns>Relationships with given ids.</returns>
    let retrieve (ids: string seq) : HttpHandler<HttpResponseMessage, ItemsWithoutCursor<RelationshipReadDto>, 'a> =
        let relationships = ids |> Seq.map Identity.Create
        retrievePlayground relationships Url
        >=> logWithMessage "Relationships:retrieve"

    /// <summary>
    /// Retrieves a list of relationships matching the given criteria. This operation does not support pagination.
    /// </summary>
    /// <param name="query">relationships search query.</param>
    /// <returns>List of relationships matching given criteria.</returns>
    let search (query: RestrictedGraphQueryDto) : HttpHandler<HttpResponseMessage, RestrictedGraphQueryResultDto, 'a> =
        searchPlayground query Url
        >=> logWithMessage "Relationships:search"



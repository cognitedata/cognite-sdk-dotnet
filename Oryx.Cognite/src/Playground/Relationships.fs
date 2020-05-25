// Copyright 2019 Cognite AS
// SPDX-License-Identifier: Apache-2.0

namespace Oryx.Cognite.Playground

open System.Net.Http

open Oryx
open Oryx.Cognite.Playground

open System.Collections.Generic
open CogniteSdk
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
    let list (query: RelationshipQuery) : HttpHandler<HttpResponseMessage, ItemsWithCursor<Relationship>, 'a> =
        withLogMessage "Relationships:list"
        >=> listPlayground query Url

    /// <summary>
    /// Create new relationships in the given project.
    /// </summary>
    /// <param name="assets">The relationships to create.</param>
    /// <returns>List of created relationships.</returns>
    let create (items: RelationshipCreate seq) : HttpHandler<HttpResponseMessage, ItemsWithoutCursor<Relationship>, 'a> =
        withLogMessage "Relationships:create"
        >=> createPlayground items Url

    /// <summary>
    /// Delete multiple relationships in the same project.
    /// </summary>
    /// <param name="relationships">The list of externalIds for relationships to delete.</param>
    /// <returns>Empty result.</returns>
    let delete (externalIds: string seq) : HttpHandler<HttpResponseMessage, EmptyResponse, 'a> =
        let relationships = externalIds |> Seq.map Identity.Create
        let items = ItemsWithoutCursor(Items=relationships)
        withLogMessage "Relationships:delete"
        >=> deletePlayground items Url

    /// <summary>
    /// Retrieves information about multiple relationships in the same project. A maximum of 1000 relationships IDs may be listed per
    /// request and all of them must be unique.
    /// </summary>
    /// <param name="ids">The ids of the relationships to get.</param>
    /// <returns>Relationships with given ids.</returns>
    let retrieve (ids: string seq) : HttpHandler<HttpResponseMessage, ItemsWithoutCursor<Relationship>, 'a> =
        let relationships = ids |> Seq.map Identity.Create
        withLogMessage "Relationships:retrieve"
        >=> retrievePlayground relationships Url

    /// <summary>
    /// Retrieves a list of relationships matching the given criteria. This operation does not support pagination.
    /// </summary>
    /// <param name="query">relationships search query.</param>
    /// <returns>List of relationships matching given criteria.</returns>
    let search (query: RestrictedGraphQuery) : HttpHandler<HttpResponseMessage, RestrictedGraphQueryResult, 'a> =
        withLogMessage "Relationships:search"
        >=> searchPlayground query Url



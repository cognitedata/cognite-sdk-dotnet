// Copyright 2019 Cognite AS
// SPDX-License-Identifier: Apache-2.0

namespace Oryx.Cognite

open System
open System.Collections.Generic

open Oryx
open Oryx.Cognite

open CogniteSdk

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
    let list (query: RelationshipQuery) (source: HttpHandler<unit>) : HttpHandler<ItemsWithCursor<Relationship>> =
        source |> withLogMessage "Relationships:list" |> list query Url

    /// <summary>
    /// Create new relationships in the given project.
    /// </summary>
    /// <param name="items">The relationships to create.</param>
    /// <returns>List of created relationships.</returns>
    let create (items: RelationshipCreate seq) (source: HttpHandler<unit>) : HttpHandler<Relationship seq> =
        source |> withLogMessage "Relationships:create" |> create items Url

    /// <summary>
    /// Delete multiple relationships in the same project.
    /// </summary>
    /// <param name="externalIds">The list of externalIds for relationships to delete.</param>
    /// <returns>Empty result.</returns>
    let delete
        (externalIds: string seq)
        (ignoreUnknownIds: bool)
        (source: HttpHandler<unit>)
        : HttpHandler<EmptyResponse> =
        let relationships = externalIds |> Seq.map Identity.Create

        let items =
            ItemsWithIgnoreUnknownIds(Items = relationships, IgnoreUnknownIds = ignoreUnknownIds)

        source |> withLogMessage "Relationships:delete" |> delete items Url

    /// <summary>
    /// Retrieves information about multiple relationships in the same project. A maximum of 1000 relationships IDs may
    /// be listed per request and all of them must be unique.
    /// </summary>
    /// <param name="ids">The ids of the relationships to get.</param>
    /// <returns>Relationships with given ids.</returns>
    let retrieve
        (ids: string seq)
        (ignoreUnknownIds: Nullable<bool>)
        (source: HttpHandler<unit>)
        : HttpHandler<Relationship seq> =
        let relationships = ids |> Seq.map Identity.Create

        source
        |> withLogMessage "Relationships:retrieve"
        |> retrieveIgnoreUnknownIds relationships (Option.ofNullable ignoreUnknownIds) Url

    /// <summary>
    /// Updates multiple relationships within the same project. This operation supports partial updates, meaning that
    /// fields omitted from the requests are not changed. Returns list of updated relationships.
    /// </summary>
    /// <param name="query">List of update items</param>
    /// <returns> Updated relationships.</returns>
    let update
        (query: IEnumerable<UpdateItem<RelationshipUpdate>>)
        (source: HttpHandler<unit>)
        : HttpHandler<Relationship seq> =
        source |> withLogMessage "Relationships:update" |> update query Url

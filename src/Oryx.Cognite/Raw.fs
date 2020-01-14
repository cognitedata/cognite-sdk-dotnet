// Copyright 2019 Cognite AS
// SPDX-License-Identifier: Apache-2.0

namespace Oryx.Cognite

open System.Net.Http

open Oryx.Cognite

open CogniteSdk
open CogniteSdk.Raw

/// Various event HTTP handlers.

[<RequireQualifiedAccess>]
module Raw =
    [<Literal>]
    let Url = "/raw/dbs"

    /// <summary>
    /// Lists all databases in the same project. A maximum of 1000 database names may be listed per request and all of
    /// them must be unique.
    /// </summary>
    /// <param name="query">Query object containing limit and nextCursor</param>
    /// <returns>databases in project.</returns>
    let listDatabases (query: DatabaseQuery) : HttpHandler<HttpResponseMessage, ItemsWithCursor<DatabaseDto>, 'a> =
        let query' = query.ToQuery();
        list query' Url

    /// <summary>
    /// Create new databases in the given project.
    /// </summary>
    /// <param name="items">The events to create.</param>
    /// <returns>List of created databases.</returns>
    let createDatabases (items: ItemsWithoutCursor<DatabaseDto>) : HttpHandler<HttpResponseMessage, ItemsWithoutCursor<DatabaseDto>, 'a> =
        create items Url

    /// <summary>
    /// Delete multiple databases in the same project.
    /// </summary>
    /// <param name="query">The list of databases to delete.</param>
    /// <returns>Empty result.</returns>
    let deleteDatabases (query: DatabaseDeleteDto) : HttpHandler<HttpResponseMessage, EmptyResponse, 'a> =
        delete query Url

    /// <summary>
    /// List tables in database.
    /// </summary>
    /// <param name="database">The database to list tables from.</param>
    /// <param name="query">The query with optional limit and cursor.</param>
    /// <returns>List of tables.</returns>
    let listTables (database: string) (query: DatabaseQuery) : HttpHandler<HttpResponseMessage, ItemsWithCursor<TableDto>, 'a> =
        let url = Url +/ database +/ "tables"
        let query' = query.ToQuery()
        list query' url

    /// <summary>
    /// Create tables in database.
    /// </summary>
    /// <param name="database">The database to list tables from.</param>
    /// <param name="items">The tables to create.</param>
    /// <returns>List of created tables.</returns>
    let createTables (database: string) (items: ItemsWithoutCursor<TableDto>) : HttpHandler<HttpResponseMessage, ItemsWithoutCursor<TableDto>, 'a> =
        let url = Url +/ database +/ "tables"
        create items Url

    /// <summary>
    /// Delete multiple tables in the same database.
    /// </summary>
    /// <param name="items">The list of tables to delete.</param>
    /// <returns>Empty result.</returns>
    let deleteTables (database: string) (items: TableDeleteDto) : HttpHandler<HttpResponseMessage, EmptyResponse, 'a> =
        let url = Url +/ database +/ "tables/"
        delete items url

    /// <summary>
    /// Retrieve rows from a table.
    /// </summary>
    /// <param name="database">The ids of the events to get.</param>
    /// <param name="table">The ids of the events to get.</param>
    /// <param name="query">The Row query.</param>
    /// <returns>The retrieved rows.</returns>
    let retrieveRows (database: string) (table: string) (query: RowQueryDto): HttpHandler<HttpResponseMessage, ItemsWithCursor<RowReadDto>, 'a> =
        let url = Url +/ database +/ "tables" +/ table +/ "rows"
        let query' = query.ToQuery()
        list query' url

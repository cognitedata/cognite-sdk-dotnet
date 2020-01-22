// Copyright 2019 Cognite AS
// SPDX-License-Identifier: Apache-2.0

namespace Oryx.Cognite

open System.Net.Http

open Oryx.Cognite

open System.Collections.Generic
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
        getWithQuery query Url

    /// <summary>
    /// Create new databases in the given project.
    /// </summary>
    /// <param name="items">The events to create.</param>
    /// <returns>List of created databases.</returns>
    let createDatabases (items: IEnumerable<DatabaseDto>) : HttpHandler<HttpResponseMessage, IEnumerable<DatabaseDto>, 'a> =
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
        Url +/ database +/ "tables"
        |> getWithQuery query

    /// <summary>
    /// Create tables in database.
    /// </summary>
    /// <param name="database">The database to list tables from.</param>
    /// <param name="items">The tables to create.</param>
    /// <returns>List of created tables.</returns>
    let createTables (database: string) (items: TableDto seq) (ensureParent: bool) : HttpHandler<HttpResponseMessage, TableDto seq, 'a> =
        let query = TableCreateQuery(EnsureParent = ensureParent)
        Url +/ database +/ "tables" |> createWithQuery items query

    /// <summary>
    /// Delete multiple tables in the same database.
    /// </summary>
    /// <param name="items">The list of tables to delete.</param>
    /// <returns>Empty result.</returns>
    let deleteTables (database: string) (items: TableDeleteDto) : HttpHandler<HttpResponseMessage, EmptyResponse, 'a> =
        Url +/ database +/ "tables/delete" |> post items

    /// <summary>
    /// Retrieve rows from a table.
    /// </summary>
    /// <param name="database">The ids of the events to get.</param>
    /// <param name="table">The ids of the events to get.</param>
    /// <param name="query">The Row query.</param>
    /// <returns>The retrieved rows.</returns>
    let listRows (database: string) (table: string) (query: RowQueryDto): HttpHandler<HttpResponseMessage, ItemsWithCursor<RowReadDto>, 'a> =
        Url +/ database +/ "tables" +/ table +/ "rows"
        |> getWithQuery query

    /// <summary>
    /// Create rows in a table.
    /// </summary>
    /// <param name="database">The ids of the events to get.</param>
    /// <param name="table">The ids of the events to get.</param>
    /// <param name="dtos">The Rows to create.</param>
    /// <param name="ensureParent">Default: false. Create database/table if it doesn't exist already.</param>
    /// <returns>The retrieved rows.</returns>
    let createRows (database: string) (table: string) (dtos: RowWriteDto seq) (ensureParent: bool): HttpHandler<HttpResponseMessage, EmptyResponse, 'a> =
        let query = RowCreateQuery(EnsureParent = ensureParent)
        Url +/ database +/ "tables" +/ table +/ "rows"
        |> createWithQueryEmpty dtos query

    /// <summary>
    /// Delete rows in a table.
    /// </summary>
    /// <param name="database">The ids of the events to get.</param>
    /// <param name="table">The ids of the events to get.</param>
    /// <param name="dtos">The Rows to create.</param>
    /// <returns>The retrieved rows.</returns>
    let deleteRows (database: string) (table: string) (dtos: RowDeleteDto seq) : HttpHandler<HttpResponseMessage, EmptyResponse, 'a> =
        Url +/ database +/ "tables" +/ table +/ "rows" +/ "delete"
        |> delete dtos
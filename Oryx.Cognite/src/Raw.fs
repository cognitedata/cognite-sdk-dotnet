// Copyright 2019 Cognite AS
// SPDX-License-Identifier: Apache-2.0

namespace Oryx.Cognite

open System.Net.Http

open Oryx
open Oryx.Cognite

open System.Collections.Generic
open CogniteSdk

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
    let listDatabases (query: RawDatabaseQuery) : HttpHandler<HttpResponseMessage, ItemsWithCursor<RawDatabase>, 'a> =
        getWithQuery query Url
        >=> logWithMessage "Raw:get"

    /// <summary>
    /// Create new databases in the given project.
    /// </summary>
    /// <param name="items">The events to create.</param>
    /// <returns>List of created databases.</returns>
    let createDatabases (items: IEnumerable<RawDatabase>) : HttpHandler<HttpResponseMessage, IEnumerable<RawDatabase>, 'a> =
        create items Url
        >=> logWithMessage "Raw:createDatabases"

    /// <summary>
    /// Delete multiple databases in the same project.
    /// </summary>
    /// <param name="query">The list of databases to delete.</param>
    /// <returns>Empty result.</returns>
    let deleteDatabases (query: RawDatabaseDelete) : HttpHandler<HttpResponseMessage, EmptyResponse, 'a> =
        delete query Url
        >=> logWithMessage "Raw:deleteDatabases"

    /// <summary>
    /// List tables in database.
    /// </summary>
    /// <param name="database">The database to list tables from.</param>
    /// <param name="query">The query with optional limit and cursor.</param>
    /// <returns>List of tables.</returns>
    let listTables (database: string) (query: RawDatabaseQuery) : HttpHandler<HttpResponseMessage, ItemsWithCursor<RawTable>, 'a> =
        Url +/ database +/ "tables"
        |> getWithQuery query
        >=> logWithMessage "Raw:listTables"

    /// <summary>
    /// Create tables in database.
    /// </summary>
    /// <param name="database">The database to list tables from.</param>
    /// <param name="items">The tables to create.</param>
    /// <returns>List of created tables.</returns>
    let createTables (database: string) (items: RawTable seq) (ensureParent: bool) : HttpHandler<HttpResponseMessage, RawTable seq, 'a> =
        let query = RawTableCreateQuery(EnsureParent = ensureParent)
        Url +/ database +/ "tables" |> createWithQuery items query
        >=> logWithMessage "Raw:createTables"

    /// <summary>
    /// Delete multiple tables in the same database.
    /// </summary>
    /// <param name="items">The list of tables to delete.</param>
    /// <returns>Empty result.</returns>
    let deleteTables (database: string) (items: RawTableDelete) : HttpHandler<HttpResponseMessage, EmptyResponse, 'a> =
        Url +/ database +/ "tables/delete" |> post items
        >=> logWithMessage "Raw:deleteTables"

    /// <summary>
    /// Retrieve rows from a table.
    /// </summary>
    /// <param name="database">The ids of the events to get.</param>
    /// <param name="table">The ids of the events to get.</param>
    /// <param name="query">The Row query.</param>
    /// <returns>The retrieved rows.</returns>
    let listRows (database: string) (table: string) (query: RawRowQuery): HttpHandler<HttpResponseMessage, ItemsWithCursor<RawRow>, 'a> =
        Url +/ database +/ "tables" +/ table +/ "rows"
        |> getWithQuery query
        >=> logWithMessage "Raw:listRows"

    /// <summary>
    /// Create rows in a table.
    /// </summary>
    /// <param name="database">The ids of the events to get.</param>
    /// <param name="table">The ids of the events to get.</param>
    /// <param name="dtos">The Rows to create.</param>
    /// <param name="ensureParent">Default: false. Create database/table if it doesn't exist already.</param>
    /// <returns>The retrieved rows.</returns>
    let createRows (database: string) (table: string) (dtos: RawRowCreate seq) (ensureParent: bool): HttpHandler<HttpResponseMessage, EmptyResponse, 'a> =
        let query = RawRowCreateQuery(EnsureParent = ensureParent)
        Url +/ database +/ "tables" +/ table +/ "rows"
        |> createWithQueryEmpty dtos query
        >=> logWithMessage "Raw:createRows"

    /// <summary>
    /// Delete rows in a table.
    /// </summary>
    /// <param name="database">The ids of the events to get.</param>
    /// <param name="table">The ids of the events to get.</param>
    /// <param name="dtos">The Rows to create.</param>
    /// <returns>The retrieved rows.</returns>
    let deleteRows (database: string) (table: string) (dtos: RawRowDelete seq) : HttpHandler<HttpResponseMessage, EmptyResponse, 'a> =
        let query = ItemsWithoutCursor<RawRowDelete>(Items=dtos)
        Url +/ database +/ "tables" +/ table +/ "rows" +/ "delete"
        |> postV10 query
        >=> logWithMessage "Raw:deleteRows"

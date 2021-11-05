// Copyright 2020 Cognite AS
// SPDX-License-Identifier: Apache-2.0

namespace Oryx.Cognite

open System.Net.Http
open System.Text.Json
open System.Collections.Generic

open Oryx
open Oryx.Cognite

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
    let listDatabases (query: RawDatabaseQuery) : IHttpHandler<unit, ItemsWithCursor<RawDatabase>> =
        withLogMessage "Raw:get"
        >=> withCompletion HttpCompletionOption.ResponseHeadersRead
        >=> getWithQuery query Url

    /// <summary>
    /// Create new databases in the given project.
    /// </summary>
    /// <param name="items">The events to create.</param>
    /// <returns>List of created databases.</returns>
    let createDatabases (items: IEnumerable<RawDatabase>) : IHttpHandler<unit, IEnumerable<RawDatabase>> =
        withLogMessage "Raw:createDatabases"
        >=> create items Url

    /// <summary>
    /// Delete multiple databases in the same project.
    /// </summary>
    /// <param name="query">The list of databases to delete.</param>
    /// <returns>Empty result.</returns>
    let deleteDatabases (query: RawDatabaseDelete) : IHttpHandler<unit, EmptyResponse> =
        withLogMessage "Raw:deleteDatabases"
        >=> delete query Url

    /// <summary>
    /// List tables in database.
    /// </summary>
    /// <param name="database">The database to list tables from.</param>
    /// <param name="query">The query with optional limit and cursor.</param>
    /// <returns>List of tables.</returns>
    let listTables (database: string) (query: RawDatabaseQuery) : IHttpHandler<unit, ItemsWithCursor<RawTable>> =
        let url = Url +/ database +/ "tables"
        withLogMessage "Raw:listTables"
        >=> withCompletion HttpCompletionOption.ResponseHeadersRead
        >=> getWithQuery query url

    /// <summary>
    /// Create tables in database.
    /// </summary>
    /// <param name="database">The database to list tables from.</param>
    /// <param name="items">The tables to create.</param>
    /// <param name="ensureParent"></param>
    /// <returns>List of created tables.</returns>
    let createTables (database: string) (items: RawTable seq) (ensureParent: bool) : IHttpHandler<unit, RawTable seq> =
        let query = RawTableCreateQuery(EnsureParent = ensureParent)
        let url = Url +/ database +/ "tables"
        withLogMessage "Raw:createTables"
        >=> createWithQuery items query url

    /// <summary>
    /// Delete multiple tables in the same database.
    /// </summary>
    /// <param name="database">The database to delete tables from.</param>
    /// <param name="items">The list of tables to delete.</param>
    /// <returns>Empty result.</returns>
    let deleteTables (database: string) (items: RawTableDelete) : IHttpHandler<unit, EmptyResponse> =
        let url = Url +/ database +/ "tables/delete"
        withLogMessage "Raw:deleteTables"
        >=> post items url

    /// <summary>
    /// Retrieve rows from a table.
    /// </summary>
    /// <param name="database">The ids of the events to get.</param>
    /// <param name="table">The ids of the events to get.</param>
    /// <param name="query">The Row query.</param>
    /// <returns>The retrieved rows.</returns>
    let listRows (database: string) (table: string) (query: RawRowQuery): IHttpHandler<unit, ItemsWithCursor<RawRow<'T>>> =
        let url = Url +/ database +/ "tables" +/ table +/ "rows"
        withLogMessage "Raw:listRows"
        >=> withCompletion HttpCompletionOption.ResponseHeadersRead
        >=> getWithQuery query url

    /// <summary>
    /// Retrieve rows from a table, with specified json serializer options.
    /// </summary>
    /// <param name="database">The ids of the events to get.</param>
    /// <param name="table">The ids of the events to get.</param>
    /// <param name="query">The Row query.</param>
    /// <param name="options">Json serializer options to use when deserializing.</param>
    /// <returns>The retrieved rows.</returns>
    let listRowsWithOptions (database: string) (table: string) (query: RawRowQuery) (options: JsonSerializerOptions) : IHttpHandler<unit, ItemsWithCursor<RawRow<'T>>> =
        let url = Url +/ database +/ "tables" +/ table +/ "rows"
        withLogMessage "Raw:listRows"
        >=> withCompletion HttpCompletionOption.ResponseHeadersRead
        >=> getWithQueryOptions query url options

    /// <summary>
    /// Get a single row from table.
    /// </summary>
    /// <param name="database">The ids of the events to get.</param>
    /// <param name="table">The ids of the events to get.</param>
    /// <param name="key">Key of row to get.</param>
    /// <returns>The retrieved row.</returns>
    let getRow (database: string) (table: string) (key: string) : IHttpHandler<unit, RawRow<'T>> =
        let url = Url +/ database +/ "tables" +/ table +/ "rows" +/ key
        withLogMessage "Raw:getRow"
        >=> getV10 url

    /// <summary>
    /// Get a single row from table, with specified json serializer options.
    /// </summary>
    /// <param name="database">The ids of the events to get.</param>
    /// <param name="table">The ids of the events to get.</param>
    /// <param name="key">Key of row to get.</param>
    /// <param name="options">Json serializer options to use when deserializing.</param>
    /// <returns>The retrieved row.</returns>
    let getRowWithOptions (database: string) (table: string) (key: string) (options: JsonSerializerOptions) : IHttpHandler<unit, RawRow<'T>> =
        let url = Url +/ database +/ "tables" +/ table +/ "rows" +/ key
        withLogMessage "Raw:getRow"
        >=> getV10Options url options

    /// <summary>
    /// Create rows in a table.
    /// </summary>
    /// <param name="database">The ids of the events to get.</param>
    /// <param name="table">The ids of the events to get.</param>
    /// <param name="rows">The Rows to create.</param>
    /// <param name="ensureParent">Default: false. Create database/table if it doesn't exist already.</param>
    /// <returns>The retrieved rows.</returns>
    let createRows (database: string) (table: string) (rows: RawRowCreate<'T> seq) (ensureParent: bool): IHttpHandler<unit, EmptyResponse> =
        let query = RawRowCreateQuery(EnsureParent = ensureParent)
        let url = Url +/ database +/ "tables" +/ table +/ "rows"
        withLogMessage "Raw:createRows"
        >=> createWithQueryEmpty rows query url

    /// <summary>
    /// Create rows in a table, with specified json serializer options.
    /// </summary>
    /// <param name="database">The ids of the events to get.</param>
    /// <param name="table">The ids of the events to get.</param>
    /// <param name="rows">The Rows to create.</param>
    /// <param name="ensureParent">Default: false. Create database/table if it doesn't exist already.</param>
    /// <param name="options">Json serializer options to use when deserializing.</param>
    /// <returns>The retrieved rows.</returns>
    let createRowsWithOptions (database: string) (table: string) (rows: RawRowCreate<'T> seq) (ensureParent: bool) (options: JsonSerializerOptions) : IHttpHandler<unit, EmptyResponse> =
        let query = RawRowCreateQuery(EnsureParent = ensureParent)
        let url = Url +/ database +/ "tables" +/ table +/ "rows"
        withLogMessage "Raw:createRows"
        >=> createWithQueryEmptyOptions rows query url options

    /// <summary>
    /// Delete rows in a table.
    /// </summary>
    /// <param name="database">The ids of the events to get.</param>
    /// <param name="table">The ids of the events to get.</param>
    /// <param name="rows">The Rows to create.</param>
    /// <returns>The retrieved rows.</returns>
    let deleteRows (database: string) (table: string) (rows: RawRowDelete seq) : IHttpHandler<unit, EmptyResponse> =
        let query = ItemsWithoutCursor<RawRowDelete>(Items=rows)
        let url = Url +/ database +/ "tables" +/ table +/ "rows" +/ "delete"
        withLogMessage "Raw:deleteRows"
        >=> postV10 query url

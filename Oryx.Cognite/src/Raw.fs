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
    let listDatabases
        (query: RawDatabaseQuery)
        (source: HttpHandler<unit>)
        : HttpHandler<ItemsWithCursor<RawDatabase>> =
        source
        |> withLogMessage "Raw:get"
        |> withCompletion HttpCompletionOption.ResponseHeadersRead
        |> getWithQuery query Url

    /// <summary>
    /// Create new databases in the given project.
    /// </summary>
    /// <param name="items">The events to create.</param>
    /// <returns>List of created databases.</returns>
    let createDatabases
        (items: IEnumerable<RawDatabase>)
        (source: HttpHandler<unit>)
        : HttpHandler<IEnumerable<RawDatabase>> =
        source
        |> withLogMessage "Raw:createDatabases"
        |> create items Url

    /// <summary>
    /// Delete multiple databases in the same project.
    /// </summary>
    /// <param name="query">The list of databases to delete.</param>
    /// <returns>Empty result.</returns>
    let deleteDatabases (query: RawDatabaseDelete) (source: HttpHandler<unit>) : HttpHandler<EmptyResponse> =
        source
        |> withLogMessage "Raw:deleteDatabases"
        |> delete query Url

    /// <summary>
    /// List tables in database.
    /// </summary>
    /// <param name="database">The database to list tables from.</param>
    /// <param name="query">The query with optional limit and cursor.</param>
    /// <returns>List of tables.</returns>
    let listTables
        (database: string)
        (query: RawDatabaseQuery)
        (source: HttpHandler<unit>)
        : HttpHandler<ItemsWithCursor<RawTable>> =
        let url = Url +/ database +/ "tables"

        source
        |> withLogMessage "Raw:listTables"
        |> withCompletion HttpCompletionOption.ResponseHeadersRead
        |> getWithQuery query url

    /// <summary>
    /// Create tables in database.
    /// </summary>
    /// <param name="database">The database to list tables from.</param>
    /// <param name="items">The tables to create.</param>
    /// <param name="ensureParent"></param>
    /// <returns>List of created tables.</returns>
    let createTables
        (database: string)
        (items: RawTable seq)
        (ensureParent: bool)
        (source: HttpHandler<unit>)
        : HttpHandler<RawTable seq> =
        let query = RawTableCreateQuery(EnsureParent = ensureParent)
        let url = Url +/ database +/ "tables"

        source
        |> withLogMessage "Raw:createTables"
        |> createWithQuery items query url

    /// <summary>
    /// Delete multiple tables in the same database.
    /// </summary>
    /// <param name="database">The database to delete tables from.</param>
    /// <param name="items">The list of tables to delete.</param>
    /// <returns>Empty result.</returns>
    let deleteTables
        (database: string)
        (items: RawTableDelete)
        (source: HttpHandler<unit>)
        : HttpHandler<EmptyResponse> =
        let url = Url +/ database +/ "tables/delete"

        source
        |> withLogMessage "Raw:deleteTables"
        |> post items url

    /// <summary>
    /// Retrieve a list of cursors for parallel read.
    /// </summary>
    /// <param name="database">The database to read from.</param>
    /// <param name="table">The table to read from.</param>
    /// <returns>A list of retrieved cursors.</returns>
    let retrieveCursors
        (database: string)
        (table: string)
        (query: RawRowCursorsQuery)
        (source: HttpHandler<unit>)
        : HttpHandler<string seq> =
        let url = Url +/ database +/ "tables" +/ table +/ "cursors"

        source
        |> withLogMessage "Raw:retrieveCursors"
        |> getWithQuery query url

    /// <summary>
    /// Retrieve rows from a table.
    /// </summary>
    /// <param name="database">The ids of the events to get.</param>
    /// <param name="table">The ids of the events to get.</param>
    /// <param name="query">The Row query.</param>
    /// <returns>The retrieved rows.</returns>
    let listRows
        (database: string)
        (table: string)
        (query: RawRowQuery)
        (source: HttpHandler<unit>)
        : HttpHandler<ItemsWithCursor<RawRow<'T>>> =
        let url = Url +/ database +/ "tables" +/ table +/ "rows"

        source
        |> withLogMessage "Raw:listRows"
        |> withCompletion HttpCompletionOption.ResponseHeadersRead
        |> getWithQuery query url

    /// <summary>
    /// Retrieve rows from a table, with specified json serializer options.
    /// </summary>
    /// <param name="database">The ids of the events to get.</param>
    /// <param name="table">The ids of the events to get.</param>
    /// <param name="query">The Row query.</param>
    /// <param name="options">Json serializer options to use when deserializing.</param>
    /// <returns>The retrieved rows.</returns>
    let listRowsWithOptions
        (database: string)
        (table: string)
        (query: RawRowQuery)
        (options: JsonSerializerOptions)
        (source: HttpHandler<unit>)
        : HttpHandler<ItemsWithCursor<RawRow<'T>>> =
        let url = Url +/ database +/ "tables" +/ table +/ "rows"

        source
        |> withLogMessage "Raw:listRows"
        |> withCompletion HttpCompletionOption.ResponseHeadersRead
        |> getWithQueryOptions query url options

    /// <summary>
    /// Get a single row from table.
    /// </summary>
    /// <param name="database">The ids of the events to get.</param>
    /// <param name="table">The ids of the events to get.</param>
    /// <param name="key">Key of row to get.</param>
    /// <returns>The retrieved row.</returns>
    let getRow (database: string) (table: string) (key: string) (source: HttpHandler<unit>) : HttpHandler<RawRow<'T>> =
        let url =
            Url
            +/ database
            +/ "tables"
            +/ table
            +/ "rows"
            +/ key

        source
        |> withLogMessage "Raw:getRow"
        |> getV10 url

    /// <summary>
    /// Get a single row from table, with specified json serializer options.
    /// </summary>
    /// <param name="database">The ids of the events to get.</param>
    /// <param name="table">The ids of the events to get.</param>
    /// <param name="key">Key of row to get.</param>
    /// <param name="options">Json serializer options to use when deserializing.</param>
    /// <returns>The retrieved row.</returns>
    let getRowWithOptions
        (database: string)
        (table: string)
        (key: string)
        (options: JsonSerializerOptions)
        (source: HttpHandler<unit>)
        : HttpHandler<RawRow<'T>> =
        let url =
            Url
            +/ database
            +/ "tables"
            +/ table
            +/ "rows"
            +/ key

        source
        |> withLogMessage "Raw:getRow"
        |> getV10Options url options

    /// <summary>
    /// Create rows in a table.
    /// </summary>
    /// <param name="database">The ids of the events to get.</param>
    /// <param name="table">The ids of the events to get.</param>
    /// <param name="rows">The Rows to create.</param>
    /// <param name="ensureParent">Default: false. Create database/table if it doesn't exist already.</param>
    /// <returns>The retrieved rows.</returns>
    let createRows
        (database: string)
        (table: string)
        (rows: RawRowCreate<'T> seq)
        (ensureParent: bool)
        (source: HttpHandler<unit>)
        : HttpHandler<EmptyResponse> =
        let query = RawRowCreateQuery(EnsureParent = ensureParent)
        let url = Url +/ database +/ "tables" +/ table +/ "rows"

        source
        |> withLogMessage "Raw:createRows"
        |> createWithQueryEmpty rows query url

    /// <summary>
    /// Create rows in a table, with specified json serializer options.
    /// </summary>
    /// <param name="database">The ids of the events to get.</param>
    /// <param name="table">The ids of the events to get.</param>
    /// <param name="rows">The Rows to create.</param>
    /// <param name="ensureParent">Default: false. Create database/table if it doesn't exist already.</param>
    /// <param name="options">Json serializer options to use when deserializing.</param>
    /// <returns>The retrieved rows.</returns>
    let createRowsWithOptions
        (database: string)
        (table: string)
        (rows: RawRowCreate<'T> seq)
        (ensureParent: bool)
        (options: JsonSerializerOptions)
        (source: HttpHandler<unit>)
        : HttpHandler<EmptyResponse> =
        let query = RawRowCreateQuery(EnsureParent = ensureParent)
        let url = Url +/ database +/ "tables" +/ table +/ "rows"

        source
        |> withLogMessage "Raw:createRows"
        |> createWithQueryEmptyOptions rows query url options

    /// <summary>
    /// Delete rows in a table.
    /// </summary>
    /// <param name="database">The ids of the events to get.</param>
    /// <param name="table">The ids of the events to get.</param>
    /// <param name="rows">The Rows to create.</param>
    /// <returns>The retrieved rows.</returns>
    let deleteRows
        (database: string)
        (table: string)
        (rows: RawRowDelete seq)
        (source: HttpHandler<unit>)
        : HttpHandler<EmptyResponse> =
        let query = ItemsWithoutCursor<RawRowDelete>(Items = rows)

        let url =
            Url
            +/ database
            +/ "tables"
            +/ table
            +/ "rows"
            +/ "delete"

        source
        |> withLogMessage "Raw:deleteRows"
        |> postV10 query url

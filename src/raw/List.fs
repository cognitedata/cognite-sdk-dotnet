// Copyright 2019 Cognite AS
// SPDX-License-Identifier: Apache-2.0

namespace CogniteSdk.Raw

open System.Web
open System.Net.Http
open System.Runtime.CompilerServices
open System.Runtime.InteropServices
open System.Threading
open System.Threading.Tasks

open FSharp.Control.Tasks.V2.ContextInsensitive
open Oryx
open Thoth.Json.Net

open CogniteSdk

[<AutoOpen>]
module Items =
    [<Literal>]
    let Url = "/raw/dbs"

    type DatabaseResponse = {
        Items: DatabaseDto seq
        NextCursor: string option
    } with
         static member Decoder : Decoder<DatabaseResponse> =
            Decode.object (fun get -> {
                Items = get.Required.Field "items" (Decode.list DatabaseDto.Decoder |> Decode.map seq)
                NextCursor = get.Optional.Field "nextCursor" Decode.string
            })

    type TableResponse = {
        Items: TableDto seq
        NextCursor: string option
    } with
         static member Decoder : Decoder<TableResponse> =
            Decode.object (fun get -> {
                Items = get.Required.Field "items" (Decode.list TableDto.Decoder |> Decode.map seq)
                NextCursor = get.Optional.Field "nextCursor" Decode.string
            })

    type RowResponse = {
        Items: RowReadDto seq
        NextCursor: string option
    } with
         static member Decoder : Decoder<RowResponse> =
            Decode.object (fun get -> {
                Items = get.Required.Field "items" (Decode.list RowReadDto.Decoder |> Decode.map seq)
                NextCursor = get.Optional.Field "nextCursor" Decode.string
            })

    let listRowsCore (database: string) (table: string) (queryParameters: DatabaseRowQuery seq) (fetch: HttpHandler<HttpResponseMessage, 'a>) =
        let queries = queryParameters |> Seq.map DatabaseRowQuery.Render |> List.ofSeq
        let encodedDbName = HttpUtility.UrlEncode database
        let encodedTableName = HttpUtility.UrlEncode table

        GET
        >=> setVersion V10
        >=> setResource (Url + "/" + encodedDbName + "/tables/" + encodedTableName + "/rows")
        >=> addQuery queries
        >=> fetch
        >=> withError decodeError
        >=> json RowResponse.Decoder

    let listTablesCore (database: string) (queryParameters: DatabaseQuery seq) (fetch: HttpHandler<HttpResponseMessage, 'a>) =
        let queries = queryParameters |> Seq.map DatabaseQuery.Render |> List.ofSeq
        let encodedDbName = HttpUtility.UrlEncode database

        GET
        >=> setVersion V10
        >=> setResource (Url + "/" + encodedDbName + "/tables")
        >=> addQuery queries
        >=> fetch
        >=> withError decodeError
        >=> json TableResponse.Decoder

    let listDatabasesCore (queryParameters: DatabaseQuery seq) (fetch: HttpHandler<HttpResponseMessage, 'a>) =
        let queries = queryParameters |> Seq.map DatabaseQuery.Render |> List.ofSeq

        GET
        >=> setVersion V10
        >=> setResource Url
        >=> addQuery queries
        >=> fetch
        >=> withError decodeError
        >=> json DatabaseResponse.Decoder

    /// <summary>
    /// Lists all databases in the same project.
    /// A maximum of 1000 database names may be listed per request and all
    /// of them must be unique.
    /// </summary>
    /// <param name="queryParameters">Limit and nextCursor</param>
    /// <param name="next">Async handler to use.</param>
    /// <returns>databases in project.</returns>
    let listDatabases (queryParameters: DatabaseQuery seq) (next: NextFunc<DatabaseResponse,'a>) : HttpContext -> HttpFuncResult<'a> =
        listDatabasesCore queryParameters fetch next

    /// <summary>
    /// Lists all databases in the same project.
    /// A maximum of 1000 database names may be listed per request and all
    /// of them must be unique.
    /// </summary>
    /// <param name="queryParameters">Limit and nextCursor</param>
    /// <returns>databases in project.</returns>
    let listDatabasesAsync (queryParameters: DatabaseQuery seq) =
        listDatabasesCore queryParameters fetch finishEarly

    /// <summary>
    /// Lists all tables in the same project for a given database.
    /// A maximum of 1000 tables may be listed per request and all
    /// of them must be unique.
    /// </summary>
    /// <param name="queryParameters">Limit and nextCursor</param>
    /// <param name="next">Async handler to use.</param>
    /// <returns>tables in project.</returns>
    let listTables (database: string) (queryParameters: DatabaseQuery seq) (next: NextFunc<TableResponse,'a>) : HttpContext -> HttpFuncResult<'a> =
        listTablesCore database queryParameters fetch next

    /// <summary>
    /// Lists all tables in the same project for a given database.
    /// A maximum of 1000 tables may be listed per request and all
    /// of them must be unique.
    /// </summary>
    /// <param name="queryParameters">Limit and nextCursor</param>
    /// <returns>tables in project.</returns>
    let listTablesAsync (database: string) (queryParameters: DatabaseQuery seq) =
        listTablesCore database queryParameters fetch finishEarly

    /// <summary>
    /// Lists all rows in the same project for a given table in a given database.
    /// A maximum of 1000 rows may be listed per request and all
    /// of them must be unique.
    /// </summary>
    /// <param name="queryParameters">Limit and nextCursor</param>
    /// <param name="next">Async handler to use.</param>
    /// <returns>rows in table in database.</returns>
    let listRows (database: string) (table: string) (queryParameters: DatabaseRowQuery seq) (next: NextFunc<RowResponse,'a>) : HttpContext -> HttpFuncResult<'a> =
        listRowsCore database table queryParameters fetch next

    /// <summary>
    /// Lists all rows in the same project for a given table in a given database.
    /// A maximum of 1000 rows may be listed per request and all
    /// of them must be unique.
    /// </summary>
    /// <param name="queryParameters">Limit and nextCursor</param>
    /// <returns>rows in table in database.</returns>
    let listRowsAsync (database: string) (table: string) (queryParameters: DatabaseRowQuery seq) =
        listRowsCore database table queryParameters fetch finishEarly

[<Extension>]
type GetdatabasesByIdsClientExtensions =
    /// <summary>
    /// Lists all databases in the same project.
    /// A maximum of 1000 database names may be listed per request and all
    /// of them must be unique.
    /// </summary>
    /// <param name="queryParameters">Limit and nextCursor</param>
    /// <returns>databases in project.</returns>
    [<Extension>]
    static member ListDatabasesAsync (this: ClientExtension, queryParameters: DatabaseQuery seq, [<Optional>] token: CancellationToken) : Task<DatabaseItems> =
        task {
            let ctx = this.Ctx |> Context.setCancellationToken token
            let! result = Items.listDatabasesAsync queryParameters this.Ctx
            match result with
            | Ok ctx ->
                let databases = ctx.Response
                let cursor = databases.NextCursor |> Option.defaultValue Unchecked.defaultof<string>
                let items : DatabaseItems = {
                    NextCursor = cursor
                    Items = databases.Items |> Seq.map (fun database -> database.ToDatabaseEntity ())
                }
                return items
            | Error (ApiError error) -> return raise (error.ToException ())
            | Error (Panic error) -> return raise error
        }

    /// <summary>
    /// Lists all databases in the same project.
    /// A maximum of 1000 database names may be listed per request and all
    /// of them must be unique.
    /// </summary>
    /// <returns>databases in project.</returns>
    [<Extension>]
    static member ListDatabasesAsync (this: ClientExtension, [<Optional>] token: CancellationToken) : Task<DatabaseItems> =
        this.ListDatabasesAsync(Seq.empty, token)

    /// <summary>
    /// Lists all tables in the same project for a given database.
    /// A maximum of 1000 tables may be listed per request and all
    /// of them must be unique.
    /// </summary>
    /// <param name="queryParameters">Limit and nextCursor</param>
    /// <returns>tables in project.</returns>
    [<Extension>]
    static member ListTablesAsync (this: ClientExtension, database: string, queryParameters: DatabaseQuery seq, [<Optional>] token: CancellationToken) : Task<TableItems> =
        task {
            let ctx = this.Ctx |> Context.setCancellationToken token
            let! result = Items.listTablesAsync database queryParameters ctx
            match result with
            | Ok ctx ->
                let tables = ctx.Response
                let cursor = tables.NextCursor |> Option.defaultValue Unchecked.defaultof<string>
                let items : TableItems = {
                    NextCursor = cursor
                    Items = tables.Items |> Seq.map (fun table -> table.ToTableEntity ())
                }return items
            | Error (ApiError error) -> return raise (error.ToException ())
            | Error (Panic error) -> return raise error
        }

    /// <summary>
    /// Lists all tables in the same project for a given database.
    /// A maximum of 1000 tables may be listed per request and all
    /// of them must be unique.
    /// </summary>
    /// <returns>tables in project.</returns>
    [<Extension>]
    static member ListTablesAsync (this: ClientExtension,  database: string, [<Optional>] token: CancellationToken) : Task<TableItems> =
        this.ListTablesAsync(database, Seq.empty, token)

    /// <summary>
    /// Lists all rows in the same project for a given table in a given database.
    /// A maximum of 1000 rows may be listed per request and all
    /// of them must be unique.
    /// </summary>
    /// <param name="queryParameters">Limit and nextCursor</param>
    /// <returns>Rows in table in database.</returns>
    [<Extension>]
    static member ListRowsAsync (this: ClientExtension, database: string, table: string, queryParameters: DatabaseRowQuery seq, [<Optional>] token: CancellationToken) : Task<RowItems> =
        task {
            let ctx = this.Ctx |> Context.setCancellationToken token
            let! result = Items.listRowsAsync database table queryParameters ctx
            match result with
            | Ok ctx ->
                let rows = ctx.Response
                let cursor = rows.NextCursor |> Option.defaultValue Unchecked.defaultof<string>
                let items : RowItems = {
                    NextCursor = cursor
                    Items = rows.Items |> Seq.map (fun row -> row.ToRowEntity ())
                }return items
            | Error (ApiError error) -> return raise (error.ToException ())
            | Error (Panic error) -> return raise error
        }

    /// <summary>
    /// Lists all rows in the same project for a given table in a given database.
    /// A maximum of 1000 rows may be listed per request and all
    /// of them must be unique.
    /// </summary>
    /// <returns>Rows in table in database.</returns>
    [<Extension>]
    static member ListRowsAsync (this: ClientExtension,  database: string, table: string, [<Optional>] token: CancellationToken) : Task<RowItems> =
        this.ListRowsAsync(database, table, Seq.empty, token)
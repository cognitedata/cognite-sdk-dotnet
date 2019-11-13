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
open Oryx.Decode

[<AutoOpen>]
module Create =
    [<Literal>]
    let Url = "/raw/dbs"

    type Request = {
        Items: DatabaseDto seq
    } with
        member this.Encoder =
            Encode.object [
                yield "items", Seq.map (fun (item: DatabaseDto) -> item.Encoder) this.Items |> Encode.seq
            ]

    type DatabaseResponse = {
        Items: DatabaseDto seq
    } with
         static member Decoder : Decoder<DatabaseResponse> =
            Decode.object (fun get -> {
                Items = get.Required.Field "items" (Decode.list DatabaseDto.Decoder)
            })

    type TableResponse = {
        Items: TableDto seq
    } with
         static member Decoder : Decoder<TableResponse> =
            Decode.object (fun get -> {
                Items = get.Required.Field "items" (Decode.list TableDto.Decoder |> Decode.map seq)
            })

    type RowRequest = {
        Items: RowWriteDto seq
    } with
        member this.Encoder =
            Encode.object [
                yield "items", Seq.map (fun (item: RowWriteDto) -> item.Encoder) this.Items |> Encode.seq
            ]

    let createRowsCore (database: string) (table: string) (rows: RowWriteDto seq) (fetch: HttpHandler<HttpResponseMessage, 'a>) =
        let request : RowRequest = { Items = rows }
        let encodedDbName = HttpUtility.UrlEncode database
        let encodedTableName = HttpUtility.UrlEncode table

        POST
        >=> setVersion V10
        >=> setContent (Content.JsonValue request.Encoder)
        >=> setResource (Url + "/" + encodedDbName + "/tables/" + encodedTableName + "/rows")
        >=> fetch
        >=> withError decodeError

    let createTablesCore (database: string) (tables: string seq) (ensureParent: bool) (fetch: HttpHandler<HttpResponseMessage, 'a>) =
        let items: DatabaseDto seq = tables |> Seq.map (fun table -> { Name = table })
        let request : Request = { Items = items }
        let encodedDbName = HttpUtility.UrlEncode database

        POST
        >=> setVersion V10
        >=> setContent (Content.JsonValue request.Encoder)
        >=> setResource (Url + "/" + encodedDbName + "/tables")
        >=> addQuery [("ensureParent", ensureParent.ToString())]
        >=> fetch
        >=> withError decodeError
        >=> json TableResponse.Decoder
        >=> map (fun response -> response.Items)

    let createDatabasesCore (databases: string seq) (fetch: HttpHandler<HttpResponseMessage, 'a>) =
        let items: DatabaseDto seq = databases |> Seq.map (fun database -> { Name = database })
        let request : Request = { Items = items }

        POST
        >=> setVersion V10
        >=> setContent (Content.JsonValue request.Encoder)
        >=> setResource Url
        >=> fetch
        >=> withError decodeError
        >=> json DatabaseResponse.Decoder
        >=> map (fun response -> response.Items)

    /// <summary>
    /// Creates databases in project
    /// </summary>
    /// <param name="databases">Database names to create</param>
    /// <param name="next">Async handler to use.</param>
    /// <returns>databases in project.</returns>
    let createDatabases (databases: string seq) (next: NextFunc<DatabaseDto seq,'a>) : HttpContext -> HttpFuncResult<'a> =
        createDatabasesCore databases fetch next

    /// <summary>
    /// Creates databases in project
    /// </summary>
    /// <param name="databases">Database names to create</param>
    /// <returns>databases in project.</returns>
    let createDatabasesAsync (databases: string seq) =
        createDatabasesCore databases fetch finishEarly

    /// <summary>
    /// Creates tables in database in project
    /// </summary>
    /// <param name="databases">Database to create tables in</param>
    /// <param name="tables">Table names to create</param>
    /// <param name="ensureParent">Create database if it does not exist</param>
    /// <param name="next">Async handler to use.</param>
    /// <returns>Tables in database</returns>
    let createTables (database: string) (tables: string seq) (ensureParent: bool) (next: NextFunc<TableDto seq,'a>) : HttpContext -> HttpFuncResult<'a> =
        createTablesCore database tables ensureParent fetch next

    /// <summary>
    /// Creates tables in database in project
    /// </summary>
    /// <param name="databases">Database to create tables in</param>
    /// <param name="tables">Table names to create</param>
    /// <param name="ensureParent">Create database if it does not exist</param>
    /// <returns>Tables in database</returns>
    let createTablesAsync (database: string) (tables: string seq) (ensureParent: bool) =
        createTablesCore database tables ensureParent fetch finishEarly

    /// <summary>
    /// Creates rows in table in database in project
    /// </summary>
    /// <param name="database">Database to create rows in</param>
    /// <param name="table">Table to create rows in</param>
    /// <param name="rows">key-value JSON object. Rows to be inserted into the given table</param>
    /// <param name="next">Async handler to use.</param>
    /// <returns>HttpResponsMessage</returns>
    let createRows (database: string) (table: string) (rows: RowWriteDto seq) (next: NextFunc<HttpResponseMessage,'a>) : HttpContext -> HttpFuncResult<'a> =
        createRowsCore database table rows fetch next

    /// <summary>
    /// Creates rows in table in database in project
    /// </summary>
    /// <param name="database">Database to create rows in</param>
    /// <param name="table">Table to create rows in</param>
    /// <param name="rows">key-value JSON object. Rows to be inserted into the given table</param>
    /// <returns>HttpResponsMessage</returns>
    let createRowsAsync (database: string) (table: string) (rows: RowWriteDto seq) =
        createRowsCore database table rows fetch finishEarly

[<Extension>]
type CreateRawClientExtensions =
    /// <summary>
    /// Creates databases in project
    /// </summary>
    /// <param name="queryParameters">Database names to create</param>
    /// <returns>databases in project.</returns>
    [<Extension>]
    static member CreateDatabasesAsync (this: ClientExtension, databases: string seq, [<Optional>] token: CancellationToken) : Task<_ seq> =
        task {
            let ctx = this.Ctx |> Context.setCancellationToken token
            let! result = Create.createDatabasesAsync databases ctx
            match result with
            | Ok ctx ->
                let databases = ctx.Response
                return databases |> Seq.map (fun database -> database.ToDatabaseEntity ())
            | Error (ApiError error) -> return raise (error.ToException ())
            | Error (Panic error) -> return raise error
        }

    /// <summary>
    /// Creates tables in database in project
    /// </summary>
    /// <param name="database">Database to create tables in</param>
    /// <param name="tables">Table names to create</param>
    /// <param name="ensureParent">Create database if it doesn't exist already</param>
    /// <returns>databases in project.</returns>
    [<Extension>]
    static member CreateTablesAsync (this: ClientExtension, database: string, tables: string seq, ensureParent: bool, [<Optional>] token: CancellationToken) : Task<_ seq> =
        task {
            let ctx = this.Ctx |> Context.setCancellationToken token
            let! result = Create.createTablesAsync database tables ensureParent ctx
            match result with
            | Ok ctx ->
                let createdTables = ctx.Response
                return createdTables |> Seq.map (fun table -> table.ToTableEntity ())
            | Error (ApiError error) -> return raise (error.ToException ())
            | Error (Panic error) -> return raise error
        }

    /// <summary>
    /// Creates rows in table in database in project
    /// </summary>
    /// <param name="database">Database to create rows in</param>
    /// <param name="table">Table to create rows in</param>
    /// <param name="rows">key-value JSON object. Rows to be inserted into the given table</param>
    /// <returns>HttpResponsMessage</returns>
    [<Extension>]
    static member CreateRowsAsync (this: ClientExtension, database: string, table: string, rows: RowEntity seq, [<Optional>] token: CancellationToken) : Task =
        task {
            let rows' = Seq.map RowWriteDto.FromRowEntity rows
            let ctx = this.Ctx |> Context.setCancellationToken token
            let! result = Create.createRowsAsync database table rows' ctx
            match result with
            | Ok _ -> return ()
            | Error (ApiError error) -> return raise (error.ToException ())
            | Error (Panic error) -> return raise error
        } :> _

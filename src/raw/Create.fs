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

    let createTablesCore (database: string) (tables: string seq) (ensureParent: bool) (fetch: HttpHandler<HttpResponseMessage, 'a>) =
        let decodeResponse = Decode.decodeResponse TableResponse.Decoder (fun response -> response.Items)
        let items: DatabaseDto seq = tables |> Seq.map (fun table -> { Name = table })
        let request : Request = { Items = items }
        let encodedDbName = HttpUtility.UrlEncode database

        POST
        >=> setVersion V10
        >=> setContent (Content.JsonValue request.Encoder)
        >=> setResource (Url + "/" + encodedDbName + "/tables")
        >=> addQuery [("ensureParent", ensureParent.ToString())]
        >=> fetch
        >=> decodeResponse

    let createDatabasesCore (databases: string seq) (fetch: HttpHandler<HttpResponseMessage, 'a>) =
        let decodeResponse = Decode.decodeResponse DatabaseResponse.Decoder (fun response -> response.Items)
        let items: DatabaseDto seq = databases |> Seq.map (fun database -> { Name = database })
        let request : Request = { Items = items }

        POST
        >=> setVersion V10
        >=> setContent (Content.JsonValue request.Encoder)
        >=> setResource Url
        >=> fetch
        >=> decodeResponse

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
    /// <param name="databases">Database names to create</param>
    /// <param name="next">Async handler to use.</param>
    /// <returns>databases in project.</returns>
    let createTables (database: string) (tables: string seq) (ensureParent: bool) (next: NextFunc<TableDto seq,'a>) : HttpContext -> HttpFuncResult<'a> =
        createTablesCore database tables ensureParent fetch next

    /// <summary>
    /// Creates tables in database in project
    /// </summary>
    /// <param name="database">Database to create tables in</param>
    /// <param name="tables">Table names to create</param>
    /// <param name="ensureParent">Create database if it doesn't exist already</param>
    /// <returns>databases in project.</returns>
    let createTablesAsync (database: string) (tables: string seq) (ensureParent: bool) =
        createTablesCore database tables ensureParent fetch finishEarly

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
            | Error error ->
                return raise (error.ToException ())
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
            | Error error ->
                return raise (error.ToException ())
        }

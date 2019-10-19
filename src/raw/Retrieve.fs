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
module Retrieve =
    [<Literal>]
    let Url = "/raw/dbs"

    type DatabaseResponse = {
        Items: DatabaseReadDto seq
    } with
         static member Decoder : Decoder<DatabaseResponse> =
            Decode.object (fun get -> {
                Items = get.Required.Field "items" (Decode.list DatabaseReadDto.Decoder |> Decode.map seq)
            })

    type TableResponse = {
        Items: TableReadDto seq
    } with
         static member Decoder : Decoder<TableResponse> =
            Decode.object (fun get -> {
                Items = get.Required.Field "items" (Decode.list TableReadDto.Decoder |> Decode.map seq)
            })

    let tablesCore (database: string) (queryParameters: DatabaseQuery seq) (fetch: HttpHandler<HttpResponseMessage, 'a>) =
        let decodeResponse = Decode.decodeResponse TableResponse.Decoder (fun response -> response.Items)
        let queries = queryParameters |> Seq.map DatabaseQuery.Render |> List.ofSeq
        let encodedDbName = HttpUtility.UrlEncode database

        GET
        >=> setVersion V10
        >=> setResource (Url + "/" + encodedDbName + "/tables")
        >=> addQuery queries
        >=> fetch
        >=> decodeResponse

    let databasesCore (queryParameters: DatabaseQuery seq) (fetch: HttpHandler<HttpResponseMessage, 'a>) =
        let decodeResponse = Decode.decodeResponse DatabaseResponse.Decoder (fun response -> response.Items)
        let queries = queryParameters |> Seq.map DatabaseQuery.Render |> List.ofSeq

        GET
        >=> setVersion V10
        >=> setResource Url
        >=> addQuery queries
        >=> fetch
        >=> decodeResponse

    /// <summary>
    /// Retrieves information about multiple databases in the same project.
    /// A maximum of 1000 databases IDs may be listed per request and all
    /// of them must be unique.
    /// </summary>
    /// <param name="queryParameters">Limit and nextCursor</param>
    /// <param name="next">Async handler to use.</param>
    /// <returns>databases in project.</returns>
    let databases (queryParameters: DatabaseQuery seq) (next: NextFunc<DatabaseReadDto seq,'a>) : HttpContext -> Task<Context<'a>> =
        databasesCore queryParameters fetch next

    /// <summary>
    /// Retrieves information about multiple databases in the same project.
    /// A maximum of 1000 databases IDs may be listed per request and all
    /// of them must be unique.
    /// </summary>
    /// <param name="queryParameters">Limit and nextCursor</param>
    /// <returns>databases in project.</returns>
    let databasesAsync (queryParameters: DatabaseQuery seq) =
        databasesCore queryParameters fetch Task.FromResult

    /// <summary>
    /// Retrieves information about multiple tables in the same project.
    /// A maximum of 1000 tables IDs may be listed per request and all
    /// of them must be unique.
    /// </summary>
    /// <param name="queryParameters">Limit and nextCursor</param>
    /// <param name="next">Async handler to use.</param>
    /// <returns>tables in project.</returns>
    let tables (database: string) (queryParameters: DatabaseQuery seq) (next: NextFunc<TableReadDto seq,'a>) : HttpContext -> Task<Context<'a>> =
        tablesCore database queryParameters fetch next

    /// <summary>
    /// Retrieves information about multiple tables in the same project.
    /// A maximum of 1000 tables IDs may be listed per request and all
    /// of them must be unique.
    /// </summary>
    /// <param name="queryParameters">Limit and nextCursor</param>
    /// <returns>tables in project.</returns>
    let tablesAsync (database: string) (queryParameters: DatabaseQuery seq) =
        tablesCore database queryParameters fetch Task.FromResult


[<Extension>]
type GetdatabasesByIdsClientExtensions =
    /// <summary>
    /// Retrieves information about multiple databases in the same project.
    /// A maximum of 1000 databases IDs may be listed per request and all
    /// of them must be unique.
    /// </summary>
    /// <param name="queryParameters">Limit and nextCursor</param>
    /// <returns>databases in project.</returns>
    [<Extension>]
    static member DatabasesAsync (this: ClientExtension, queryParameters: DatabaseQuery seq, [<Optional>] token: CancellationToken) : Task<_ seq> =
        task {
            let ctx = this.Ctx |> Context.setCancellationToken token
            let! ctx = Retrieve.databasesAsync queryParameters this.Ctx
            match ctx.Result with
            | Ok databases ->
                return databases |> Seq.map (fun database -> database.ToDatabaseEntity ())
            | Error error ->
                return raise (error.ToException ())
        }

    /// <summary>
    /// Retrieves information about multiple databases in the same project.
    /// A maximum of 1000 databases IDs may be listed per request and all
    /// of them must be unique.
    /// </summary>
    /// <returns>databases in project.</returns>
    [<Extension>]
    static member DatabasesAsync (this: ClientExtension, [<Optional>] token: CancellationToken) : Task<_ seq> =
        this.DatabasesAsync(Seq.empty, token)

    /// <summary>
    /// Retrieves information about multiple tables in the same project.
    /// A maximum of 1000 tables IDs may be listed per request and all
    /// of them must be unique.
    /// </summary>
    /// <param name="queryParameters">Limit and nextCursor</param>
    /// <returns>tables in project.</returns>
    [<Extension>]
    static member TablesAsync (this: ClientExtension, database: string, queryParameters: DatabaseQuery seq, [<Optional>] token: CancellationToken) : Task<_ seq> =
        task {
            let ctx = this.Ctx |> Context.setCancellationToken token
            let! ctx = Retrieve.tablesAsync database queryParameters this.Ctx
            match ctx.Result with
            | Ok tables ->
                return tables |> Seq.map (fun table -> table.ToTableEntity ())
            | Error error ->
                return raise (error.ToException ())
        }

    /// <summary>
    /// Retrieves information about multiple tables in the same project.
    /// A maximum of 1000 tables IDs may be listed per request and all
    /// of them must be unique.
    /// </summary>
    /// <returns>tables in project.</returns>
    [<Extension>]
    static member TablesAsync (this: ClientExtension,  database: string, [<Optional>] token: CancellationToken) : Task<_ seq> =
        this.TablesAsync(database, Seq.empty, token)

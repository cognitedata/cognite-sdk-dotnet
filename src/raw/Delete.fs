// Copyright 2019 Cognite AS
// SPDX-License-Identifier: Apache-2.0

namespace CogniteSdk.Raw

open System.Net.Http
open System.Runtime.CompilerServices
open System.Runtime.InteropServices
open System.Threading
open System.Threading.Tasks

open FSharp.Control.Tasks.V2.ContextInsensitive
open Oryx
open Thoth.Json.Net

open CogniteSdk
open System.Web

[<AutoOpen>]
module Delete =
    [<Literal>]
    let Url = "/raw/dbs/delete"

    type DatabaseRequest = {
        Items: DatabaseDto seq
        Recursive: bool
    } with
        member this.Encoder =
            Encode.object [
                yield "items", Seq.map (fun (item: DatabaseDto) -> item.Encoder) this.Items |> Encode.seq
                yield "recursive", this.Recursive |> Encode.bool
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

    let deleteTablesCore (database: string) (tables: string seq) (fetch: HttpHandler<HttpResponseMessage, 'a>) =
        let decodeResponse = Decode.decodeError
        let items: DatabaseDto seq = tables |> Seq.map (fun table -> { Name = table })
        let request : Request = { Items = items }
        let encodedDbName = HttpUtility.UrlEncode database

        POST
        >=> setVersion V10
        >=> setContent (Content.JsonValue request.Encoder)
        >=> setResource (Url + "/" + encodedDbName + "/tables/delete")
        >=> fetch
        >=> decodeResponse

    let deleteDatabasesCore (databases: string seq) (recursive: bool) (fetch: HttpHandler<HttpResponseMessage, 'a>) =
        let decodeResponse = Decode.decodeError
        let items: DatabaseDto seq = databases |> Seq.map (fun database -> { Name = database })
        let request : DatabaseRequest = { Items = items; Recursive = recursive }

        POST
        >=> setVersion V10
        >=> setContent (Content.JsonValue request.Encoder)
        >=> setResource Url
        >=> fetch
        >=> decodeResponse

    /// <summary>
    /// Deletes databases in project
    /// </summary>
    /// <param name="databases">Database names to delete</param>
    /// <param name="recursive">Recursively delete tables in databases</param>
    /// <param name="next">Async handler to use.</param>
    /// <returns>HttpResponseMessage.</returns>
    let deleteDatabases (databases: string seq) (recursive: bool) (next: NextFunc<HttpResponseMessage,'a>) : HttpContext -> HttpFuncResult<'a> =
        deleteDatabasesCore databases recursive fetch next

    /// <summary>
    /// Deletes databases in project
    /// </summary>
    /// <param name="databases">Database names to delete</param>
    /// <param name="recursive">Recursively delete tables in databases</param>
    /// <returns>HttpResponseMessage.</returns>
    let deleteDatabasesAsync (databases: string seq) (recursive: bool) =
        deleteDatabasesCore databases recursive fetch finishEarly

    /// <summary>
    /// Deletes tables in database in project
    /// </summary>
    /// <param name="database">Database to delete tables from</param>
    /// <param name="tables">Table names to delete</param>
    /// <param name="next">Async handler to use.</param>
    /// <returns>HttpResponseMessage.</returns>
    let deleteTables (database: string) (tables: string seq) (next: NextFunc<HttpResponseMessage,'a>) : HttpContext -> HttpFuncResult<'a> =
        deleteTablesCore database tables fetch next

    /// <summary>
    /// Deletes tables in database in project
    /// </summary>
    /// <param name="database">Database to delete tables from</param>
    /// <param name="tables">Table names to delete</param>
    /// <returns>HttpResponseMessage.</returns>
    let deleteTablesAsync (database: string) (tables: string seq) =
        deleteTablesCore database tables fetch finishEarly

[<Extension>]
type DeleteRawClientExtensions =
    /// <summary>
    /// Deletes databases in project
    /// </summary>
    /// <param name="databases">Database names to delete</param>
    /// <param name="recursive">Recursively delete tables in databases</param>
    /// <returns>HttpResponseMessage.</returns>
    [<Extension>]
    static member DeleteDatabasesAsync (this: ClientExtension, databases: string seq, [<Optional; DefaultParameterValue(false)>] recursive: bool, [<Optional>] token: CancellationToken) : Task =
        task {
            let ctx = this.Ctx |> Context.setCancellationToken token
            let! result = Delete.deleteDatabasesAsync databases recursive ctx
            match result with
            | Ok _ -> return ()
            | Error error ->
                return raise (error.ToException ())
        } :> _

    /// <summary>
    /// Deletes tables in database in project
    /// </summary>
    /// <param name="database">Database to delete tables from</param>
    /// <param name="tables">Table names to delete</param>
    /// <returns>HttpResponseMessage.</returns>
    [<Extension>]
    static member DeleteTablesAsync (this: ClientExtension, database: string, tables: string seq, [<Optional>] token: CancellationToken) : Task =
        task {
            let ctx = this.Ctx |> Context.setCancellationToken token
            let! result = Delete.deleteTablesAsync database tables ctx
            match result with
            | Ok _ -> return ()
            | Error error ->
                return raise (error.ToException ())
        } :> _

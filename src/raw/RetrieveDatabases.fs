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

[<AutoOpen>]
module Retrieve =
    [<Literal>]
    let Url = "/raw/dbs"

    type DatabaseQuery =
        private
        | CaseLimit of int
        | CaseCursor of string

        /// Max number of results to return
        static member Limit limit =
            if limit > MaxLimitSize || limit < 1 then
                failwith "Limit must be set to 1000 or less"
            CaseLimit limit
        /// Cursor return from previous request
        static member Cursor cursor = CaseCursor cursor

        static member Render (this: DatabaseQuery) =
            match this with
            | CaseLimit limit -> "limit", limit.ToString()
            | CaseCursor cursor -> "cursor", cursor.ToString()

    type DatabaseResponse = {
        Items: DatabaseReadDto seq
    } with
         static member Decoder : Decoder<DatabaseResponse> =
            Decode.object (fun get -> {
                Items = get.Required.Field "items" (Decode.list DatabaseReadDto.Decoder |> Decode.map seq)
            })

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




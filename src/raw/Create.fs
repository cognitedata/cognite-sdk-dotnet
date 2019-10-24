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
            let! result = Create.createDatabasesAsync databases this.Ctx
            match result with
            | Ok ctx ->
                let databases = ctx.Response
                return databases |> Seq.map (fun database -> database.ToDatabaseEntity ())
            | Error error ->
                return raise (error.ToException ())
        }


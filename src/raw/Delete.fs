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
module Delete =
    [<Literal>]
    let Url = "/raw/dbs/delete"

    type Request = {
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

    let deleteDatabasesCore (databases: string seq) (recursive: bool) (fetch: HttpHandler<HttpResponseMessage, 'a>) =
        let decodeResponse = Decode.decodeError
        let items: DatabaseDto seq = databases |> Seq.map (fun database -> { Name = database })
        let request : Request = { Items = items; Recursive = recursive }

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
    /// <returns>databases in project.</returns>
    let deleteDatabases (databases: string seq) (recursive: bool) (next: NextFunc<HttpResponseMessage,'a>) : HttpContext -> HttpFuncResult<'a> =
        deleteDatabasesCore databases recursive fetch next

    /// <summary>
    /// Deletes databases in project
    /// </summary>
    /// <param name="databases">Database names to delete</param>
    /// <param name="recursive">Recursively delete tables in databases</param>
    /// <returns>databases in project.</returns>
    let deleteDatabasesAsync (databases: string seq) (recursive: bool) =
        deleteDatabasesCore databases recursive fetch finishEarly

[<Extension>]
type DeleteRawClientExtensions =
    /// <summary>
    /// Deletes databases in project
    /// </summary>
    /// <param name="databases">Database names to delete</param>
    /// <param name="recursive">Recursively delete tables in databases</param>
    /// <returns>databases in project.</returns>
    [<Extension>]
    static member DeleteDatabasesAsync (this: ClientExtension, databases: string seq, [<Optional; DefaultParameterValue(false)>] recursive: bool, [<Optional>] token: CancellationToken) : Task =
        task {
            let ctx = this.Ctx |> Context.setCancellationToken token
            let! result = Delete.deleteDatabasesAsync databases recursive this.Ctx
            match result with
            | Ok _ -> return ()
            | Error error ->
                return raise (error.ToException ())
        } :> _


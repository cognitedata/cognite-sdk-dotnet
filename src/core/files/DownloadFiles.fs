// Copyright 2019 Cognite AS
// SPDX-License-Identifier: Apache-2.0

namespace CogniteSdk.Files

open System.Net.Http
open System.Runtime.CompilerServices
open System.Runtime.InteropServices
open System.Threading
open System.Threading.Tasks

open FSharp.Control.Tasks.V2.ContextInsensitive
open Oryx
open Oryx.ResponseReaders
open Thoth.Json.Net

open CogniteSdk

[<AutoOpen>]
module DownloadLink =
    [<Literal>]
    let Url = "/files/downloadlink"
    type FileRequest = {
        Items: Identity seq
    } with
        member this.Encoder  =
            Encode.object [
                yield "items", this.Items |> Seq.map(fun id -> id.Encoder) |> Encode.seq
            ]

    type DownloadResponse = {
        Identity: Identity
        DownloadUrl: string
    } with
        static member Decoder : Decoder<DownloadResponse> =
            Decode.object (fun get ->
                let idOpt =
                    get.Optional.Field "id" Decode.int64
                    |> Option.map Identity.Id
                let externalIdOpt =
                    get.Optional.Field "externalId" Decode.string
                    |> Option.map Identity.ExternalId
                let identity = idOpt |> Option.orElse externalIdOpt
                {
                    Identity = identity.Value
                    DownloadUrl = get.Required.Field "downloadUrl" Decode.string
                }
            )
        member this.ToDownloadResponseEntity () : DownloadResponseEntity =
            match this.Identity with
            | Id id -> DownloadResponseEntity(id=id, externalId=null, downloadUrl=null)
            | ExternalId externalId -> DownloadResponseEntity(id=0L, externalId=externalId, downloadUrl=this.DownloadUrl)

    type FileDownloadLinkResponse = {
        Items: DownloadResponse seq
    } with
        static member Decoder : Decoder<FileDownloadLinkResponse> =
            Decode.object (fun get -> {
                Items = get.Required.Field "items" (Decode.list DownloadResponse.Decoder)
            })

    let getDownloadLinksCore (ids: Identity seq) (fetch: HttpHandler<HttpResponseMessage, 'a>) =
        let request : FileRequest = { Items = ids }

        POST
        >=> setVersion V10
        >=> setContent (Content.JsonValue request.Encoder)
        >=> setResource Url
        >=> fetch
        >=> withError decodeError
        >=> json FileDownloadLinkResponse.Decoder
        >=> map (fun response -> response.Items)

    /// <summary>
    /// Retrieves downloadUrl from multiple files in the same project.
    /// A maximum of 1000 files IDs may be listed per request and all
    /// of them must be unique.
    /// </summary>
    /// <param name="fileIds">The ids of the files to get.</param>
    /// <param name="next">Async handler to use.</param>
    /// <returns>Files downloadUrl.</returns>
    let getDownloadLinks (ids: Identity seq) (next: NextFunc<DownloadResponse seq,'a>) : HttpContext -> HttpFuncResult<'a> =
        getDownloadLinksCore ids fetch next

    /// <summary>
    /// Retrieves downloadUrl from multiple files in the same project.
    /// A maximum of 1000 files IDs may be listed per request and all
    /// of them must be unique.
    /// </summary>
    /// <param name="fileIds">The ids of the files to get.</param>
    /// <returns>Files downloadUrl.</returns>
    let getDownloadLinksAsync (ids: Identity seq) =
        getDownloadLinksCore ids fetch finishEarly


[<Extension>]
type GetDownloadFilesClientExtensions =
    /// <summary>
    /// Retrieves downloadUrl from multiple files in the same project.
    /// A maximum of 1000 files IDs may be listed per request and all
    /// of them must be unique.
    /// </summary>
    /// <param name="fileIds">The ids of the files to get.</param>
    /// <returns>Files downloadUrl.</returns>
    [<Extension>]
    static member GetDownloadLinksAsync (this: ClientExtension, ids: seq<Identity>, [<Optional>] token: CancellationToken) : Task<_ seq> =
        task {
            let ctx = this.Ctx |> Context.setCancellationToken token
            let! result = DownloadLink.getDownloadLinksAsync ids ctx
            match result with
            | Ok ctx ->
                return ctx.Response |> Seq.map (fun file -> file.ToDownloadResponseEntity ())
            | Error error -> return raiseError error
        }

    /// <summary>
    /// Retrieves downloadUrl from multiple files in the same project.
    /// A maximum of 1000 files IDs may be listed per request and all
    /// of them must be unique.
    /// </summary>
    /// <param name="fileIds">The ids of the files to get.</param>
    /// <returns>Files downloadUrl.</returns>
    [<Extension>]
    static member GetDownloadLinksAsync (this: ClientExtension, ids: seq<int64>, [<Optional>] token: CancellationToken) : Task<_ seq> =
        this.GetDownloadLinksAsync(ids |> Seq.map Identity.Id, token)

    /// <summary>
    /// Retrieves downloadUrl from multiple files in the same project.
    /// A maximum of 1000 files IDs may be listed per request and all
    /// of them must be unique.
    /// </summary>
    /// <param name="fileIds">The ids of the files to get.</param>
    /// <returns>Files downloadUrl.</returns>
    [<Extension>]
    static member GetDownloadLinksAsync (this: ClientExtension, ids: seq<string>, [<Optional>] token: CancellationToken) : Task<_ seq> =
        this.GetDownloadLinksAsync(ids |> Seq.map Identity.ExternalId, token)

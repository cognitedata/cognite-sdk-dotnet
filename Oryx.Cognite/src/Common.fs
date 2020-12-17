// Copyright 2020 Cognite AS
// SPDX-License-Identifier: Apache-2.0

/// Common types for the SDK.
namespace Oryx.Cognite

open System
open System.Diagnostics
open System.Reflection
open System.Text.Json

open Oryx
open CogniteSdk

type ApiVersion =
    | V05
    | V06
    | V10
    | Playground

    override this.ToString () =
        match this with
        | V05 -> "0.5"
        | V06 -> "0.6"
        | V10 -> "v1"
        | Playground -> "playground"

/// Place holders that may be used in debug messages.
module PlaceHolder =
    [<Literal>]
    let BaseUrl = "BaseUrl"

    [<Literal>]
    let Resource = "Resource"

    [<Literal>]
    let ApiVersion = "ApiVersion"

    [<Literal>]
    let Project = "Project"

    [<Literal>]
    let HasAppId = "HasAppId"

[<AutoOpen>]
module Common =

    /// Combines two URI string fragments.
    let combine (path1: string) (path2: string) =
        if path2.Length = 0 then
            path1
        else if path1.Length = 0 then
            path2
        else
            let ch = path1.[path1.Length - 1]
            if ch <> '/' then
                path1 + "/" + path2.TrimStart('/')
            else
                path1 + path2.TrimStart('/')

    let (+/) path1 path2 = combine path1 path2

    let jsonOptions =
        let options =
            JsonSerializerOptions(
                /// Allow extra comma at the end of a list of JSON values in an object or array is allowed (and ignored)
                AllowTrailingCommas=true,
                /// Convert property names on an object to camel-casing
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                /// Null values are ignored during serialization and deserialization.
                IgnoreNullValues = true
            )
        options.Converters.Add(MultiValueConverter())
        options

[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module Context =
    let urlBuilder (request: HttpRequest) =

        let items = request.Items
        let version =
            match Map.tryFind PlaceHolder.ApiVersion items with
            | Some (String version) -> version
            | _ -> failwith "API version not set."

        let project =
            match Map.tryFind PlaceHolder.Project items with
            | Some (String project) -> project
            | _ -> failwith "Client must set project."

        let resource =
            match Map.tryFind PlaceHolder.Resource items with
            | Some (String resource) -> resource
            | _ -> failwith "Resource not set."

        let baseUrl =
            match Map.tryFind PlaceHolder.BaseUrl items with
            | Some (Url url) -> url.ToString()
            | _ -> "https://api.cognitedata.com"

        if not (Map.containsKey PlaceHolder.HasAppId items)
        then failwith "Client must set the Application ID (appId)."

        sprintf "api/%s/projects/%s%s" version project resource
        |> combine baseUrl

    let private fileVersion = FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).FileVersion

    let withUrlBuilder ctx =
        Context.withUrlBuilder urlBuilder ctx

    /// Set the project to connect to.
    let withProject (project: string) (context: HttpContext) =
        { context with Request = { context.Request with Items = context.Request.Items.Add(PlaceHolder.Project, String project) } }

    let withAppId (appId: string) (context: HttpContext) =
        { context with Request = { context.Request with Headers = context.Request.Headers.Add ("x-cdp-app", appId); Items = context.Request.Items.Add(PlaceHolder.HasAppId, String "true") } }

    let withBaseUrl (baseUrl: Uri) (context: HttpContext) =
        { context with Request = { context.Request with Items = context.Request.Items.Add(PlaceHolder.BaseUrl, Url baseUrl) } }

    let create () =
        Context.defaultContext
        |> Context.withUrlBuilder urlBuilder
        |> Context.withHeader ("x-cdp-sdk", sprintf "CogniteNetSdk:%s" fileVersion)
        |> Context.withLogFormat "CDF ({Message}): {Url}\n→ {RequestContent}\n← {ResponseContent}"


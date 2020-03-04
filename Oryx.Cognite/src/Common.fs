// Copyright 2019 Cognite AS
// SPDX-License-Identifier: Apache-2.0

/// Common types for the SDK.
namespace Oryx.Cognite

open System
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

        let extra = request.Extra
        let version =
            match Map.tryFind "apiVersion" extra with
            | Some (String version) -> version
            | _ -> failwith "API version not set."

        let project =
            match Map.tryFind "Project" extra with
            | Some (String project) -> project
            | _ -> failwith "Client must set project."

        let resource =
            match Map.tryFind "Resource" extra with
            | Some (String resource) -> resource
            | _ -> failwith "Resource not set."

        let baseUrl =
            match Map.tryFind "BaseUrl" extra with
            | Some (Url url) -> url.ToString()
            | _ -> "https://api.cognitedata.com"

        if not (Map.containsKey "hasAppId" extra)
        then failwith "Client must set the Application ID (appId)."

        sprintf "api/%s/projects/%s%s" version project resource
        |> combine baseUrl

    let private version =
        let version = Assembly.GetExecutingAssembly().GetName().Version
        (version.Major, version.Minor, version.Build)

    let withUrlBuilder ctx =
        Context.withUrlBuilder urlBuilder ctx

    /// Set the project to connect to.
    let withProject (project: string) (context: HttpContext) =
        { context with Request = { context.Request with Extra = context.Request.Extra.Add("Project", String project) } }

    let withAppId (appId: string) (context: HttpContext) =
        { context with Request = { context.Request with Headers =  ("x-cdp-app", appId) :: context.Request.Headers; Extra = context.Request.Extra.Add("hasAppId", String "true") } }

    let withBaseUrl (serviceUrl: Uri) (context: HttpContext) =
        { context with Request = { context.Request with Extra = context.Request.Extra.Add("BaseUrl", Url serviceUrl) } }

    let create () =
        let major, minor, build = version
        Context.defaultContext
        |> Context.withUrlBuilder urlBuilder
        |> Context.withHeader ("x-cdp-sdk", sprintf "CogniteNetSdk:%d.%d.%d" major minor build)
        |> Context.withLogFormat "CDF ({Message}): {Url}\n→ {RequestContent}\n← {ResponseContent}"


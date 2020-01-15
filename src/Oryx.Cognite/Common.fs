// Copyright 2019 Cognite AS
// SPDX-License-Identifier: Apache-2.0

/// Common types for the SDK.
namespace Oryx.Cognite

open System.IO
open System.Reflection
open System.Text.Json

open Oryx

type ApiVersion =
    | V05
    | V06
    | V10

    override this.ToString () =
        match this with
        | V05 -> "0.5"
        | V06 -> "0.6"
        | V10 -> "v1"

[<AutoOpen>]
module Common =
    let (+/) path1 path2 = Path.Combine(path1, path2)

    let jsonOptions =
        JsonSerializerOptions(
            /// Allow extra comma at the end of a list of JSON values in an object or array is allowed (and ignored)
            AllowTrailingCommas=true,
            /// Convert property names on an object to camel-casing
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            /// Null values are ignored during serialization and deserialization.
            IgnoreNullValues = true
        )

[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module Context =
    let urlBuilder (request: HttpRequest) =
        let extra = request.Extra
        let version = extra.["apiVersion"]
        let project =
            if Map.containsKey "project" extra
            then extra.["project"]
            else failwith "Client must set project."

        let resource = extra.["resource"]
        let serviceUrl =
            if Map.containsKey "serviceUrl" extra
            then extra.["serviceUrl"]
            else "https://api.cognitedata.com"

        if not (Map.containsKey "hasAppId" extra)
        then failwith "Client must set the Application ID (appId)."

        sprintf "%s/api/%s/projects/%s%s" serviceUrl version project resource

    let private version =
        let version = Assembly.GetExecutingAssembly().GetName().Version
        (version.Major, version.Minor, version.Build)

    let setUrlBuilder ctx =
        Context.setUrlBuilder urlBuilder ctx

    /// Set the project to connect to.
    let setProject (project: string) (context: HttpContext) =
        { context with Request = { context.Request with Extra = context.Request.Extra.Add("project", project) } }

    let setAppId (appId: string) (context: HttpContext) =
        { context with Request = { context.Request with Headers =  ("x-cdp-app", appId) :: context.Request.Headers; Extra = context.Request.Extra.Add("hasAppId", "true") } }

    let setServiceUrl (serviceUrl: string) (context: HttpContext) =
        { context with Request = { context.Request with Extra = context.Request.Extra.Add("serviceUrl", serviceUrl) } }

    let create () =
        let major, minor, build = version
        Context.defaultContext
        |> Context.setUrlBuilder urlBuilder
        |> Context.addHeader ("x-cdp-sdk", sprintf "CogniteNetSdk:%d.%d.%d" major minor build)


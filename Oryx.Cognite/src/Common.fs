// Copyright 2020 Cognite AS
// SPDX-License-Identifier: Apache-2.0

/// Common types for the SDK.
namespace Oryx.Cognite

open System.Text.Json

open CogniteSdk

type ApiVersion =
    | V05
    | V06
    | V10
    | Playground

    override this.ToString() =
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
                // Allow extra comma at the end of a list of JSON values in an object or array is allowed (and ignored)
                AllowTrailingCommas = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                IgnoreNullValues = true
            )

        options.Converters.Add(MultiValueConverter())
        options.Converters.Add(ObjectToDictionaryJsonConverter())
        options.Converters.Add(AclConverter())
        options

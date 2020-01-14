// Copyright 2019 Cognite AS
// SPDX-License-Identifier: Apache-2.0

namespace CogniteSdk.Login

open Oryx
open Thoth.Json.Net

[<AutoOpen>]
module LoginExtensions =
    type LoginStatusDto with
        static member Decoder : Decoder<LoginStatusDto> =
            Decode.object (fun get ->
                let metadata = get.Optional.Field "metadata" (Decode.dict Decode.string)
                let assetIds = get.Optional.Field "assetIds" (Decode.list Decode.int64)

                {
                    User = get.Required.Field "user" Decode.string
                    LoggedIn = get.Required.Field "loggedIn" Decode.bool
                    Project = get.Required.Field "project" Decode.string
                    ProjectId = get.Required.Field "projectId" Decode.int64
                    ApiKeyId = get.Optional.Field "apiKeyId" Decode.int64
                })

    type LoginStatusItemsDto with
        static member Decoder : Decoder<LoginStatusItemsDto> =
            Decode.object (fun get -> {
                Data = get.Required.Field "data" LoginStatusDto.Decoder
            })
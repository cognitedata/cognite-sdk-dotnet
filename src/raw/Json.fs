// Copyright 2019 Cognite AS
// SPDX-License-Identifier: Apache-2.0

namespace CogniteSdk.Raw

open Oryx
open Thoth.Json.Net

[<AutoOpen>]
module RawJsonExtensions =
    type DatabaseReadDto with
        static member Decoder : Decoder<DatabaseReadDto> =
            Decode.object (fun get ->
                {
                    Name = get.Required.Field "name" Decode.string
                })

    type DatabaseItemsReadDto with
        static member Decoder : Decoder<DatabaseItemsReadDto> =
            Decode.object (fun get -> {
                Items = get.Required.Field "items" (Decode.list DatabaseReadDto.Decoder |> Decode.map seq)
                NextCursor = get.Optional.Field "nextCursor" Decode.string
            })

    type TableReadDto with
        static member Decoder : Decoder<TableReadDto> =
            Decode.object (fun get ->
                {
                    Name = get.Required.Field "name" Decode.string
                })

    type TableItemsReadDto with
        static member Decoder : Decoder<TableItemsReadDto> =
            Decode.object (fun get -> {
                Items = get.Required.Field "items" (Decode.list TableReadDto.Decoder |> Decode.map seq)
                NextCursor = get.Optional.Field "nextCursor" Decode.string
            })
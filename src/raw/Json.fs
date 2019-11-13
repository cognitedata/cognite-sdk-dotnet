// Copyright 2019 Cognite AS
// SPDX-License-Identifier: Apache-2.0

namespace CogniteSdk.Raw

open Oryx
open Thoth.Json.Net

[<AutoOpen>]
module RawJsonExtensions =
    type DatabaseDto with
        static member Decoder : Decoder<DatabaseDto> =
            Decode.object (fun get ->
                {
                    Name = get.Required.Field "name" Decode.string
                })
        member this.Encoder =
            Encode.object [
                yield "name", Encode.string this.Name
            ]

    type DatabaseItemsDto with
        static member Decoder : Decoder<DatabaseItemsDto> =
            Decode.object (fun get -> {
                Items = get.Required.Field "items" (Decode.list DatabaseDto.Decoder |> Decode.map seq)
                NextCursor = get.Optional.Field "nextCursor" Decode.string
            })

    type TableDto with
        static member Decoder : Decoder<TableDto> =
            Decode.object (fun get ->
                {
                    Name = get.Required.Field "name" Decode.string
                })
        member this.Encoder =
            Encode.object [
                yield "name", Encode.string this.Name
            ]

    type TableItemsReadDto with
        static member Decoder : Decoder<TableItemsReadDto> =
            Decode.object (fun get -> {
                Items = get.Required.Field "items" (Decode.list TableDto.Decoder |> Decode.map seq)
                NextCursor = get.Optional.Field "nextCursor" Decode.string
            })

    type RowReadDto with
        static member Decoder : Decoder<RowReadDto> =
            Decode.object (fun get ->
                {
                    Key = get.Required.Field "key" Decode.string
                    Columns = get.Required.Field "columns" (Decode.dict Decode.value)
                    LastUpdatedTime = get.Required.Field "lastUpdatedTime" Decode.int64
                })

    type RowItemsReadDto with
        static member Decoder : Decoder<RowItemsReadDto> =
            Decode.object (fun get -> {
                Items = get.Required.Field "items" (Decode.list RowReadDto.Decoder |> Decode.map seq)
                NextCursor = get.Optional.Field "nextCursor" Decode.string
            })

    type RowWriteDto with
        member this.Encoder =
            Encode.object [
                yield "key", Encode.string this.Key
                yield "columns", Encode.dict (this.Columns |> Seq.map (|KeyValue|) |> Map.ofSeq)
            ]

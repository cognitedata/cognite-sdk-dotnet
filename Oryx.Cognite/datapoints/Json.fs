// Copyright 2019 Cognite AS
// SPDX-License-Identifier: Apache-2.0

namespace CogniteSdk.DataPoints

open Thoth.Json.Net
open Oryx


[<AutoOpen>]
module TimeseriesExtensions =
    type NumericDataPointDto with
        static member Decoder : Decoder<NumericDataPointDto> =
            Decode.object (fun get ->
                {
                    TimeStamp = get.Required.Field "timestamp" Decode.int64
                    Value = get.Required.Field "value" Decode.float
                })

    type StringDataPointDto with
        static member Decoder : Decoder<StringDataPointDto> =
            Decode.object (fun get ->
                {
                    TimeStamp = get.Required.Field "timestamp" Decode.int64
                    Value = get.Required.Field "value" Decode.string
                })

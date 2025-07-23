// Copyright 2024 Cognite AS
// SPDX-License-Identifier: Apache-2.0

namespace Oryx.Cognite

open System
open System.Text.Json
open System.Text.Json.Serialization
open CogniteSdk.Beta

/// Converter for StreamTemplateName enum that ensures proper string serialization
type StreamTemplateNameConverter() =
    inherit JsonConverter<StreamTemplateName>()
    
    override _.Read(reader: byref<Utf8JsonReader>, _: Type, _: JsonSerializerOptions) : StreamTemplateName =
        let value = reader.GetString()
        match value with
        | "ImmutableTestStream" -> StreamTemplateName.ImmutableTestStream
        | "ImmutableDataStaging" -> StreamTemplateName.ImmutableDataStaging
        | "ImmutableNormalizedData" -> StreamTemplateName.ImmutableNormalizedData
        | "ImmutableArchive" -> StreamTemplateName.ImmutableArchive
        | "MutableTestStream" -> StreamTemplateName.MutableTestStream
        | "MutableLiveData" -> StreamTemplateName.MutableLiveData
        | _ -> failwithf "Unknown stream template name: %s" value

    override _.Write(writer: Utf8JsonWriter, value: StreamTemplateName, _: JsonSerializerOptions) : unit =
        let stringValue = 
            match value with
            | StreamTemplateName.ImmutableTestStream -> "ImmutableTestStream"
            | StreamTemplateName.ImmutableDataStaging -> "ImmutableDataStaging"
            | StreamTemplateName.ImmutableNormalizedData -> "ImmutableNormalizedData"
            | StreamTemplateName.ImmutableArchive -> "ImmutableArchive"
            | StreamTemplateName.MutableTestStream -> "MutableTestStream"
            | StreamTemplateName.MutableLiveData -> "MutableLiveData"
            | _ -> failwithf "Unknown stream template name value: %A" value
        writer.WriteStringValue(stringValue)

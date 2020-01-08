// Copyright 2019 Cognite AS
// SPDX-License-Identifier: Apache-2.0

namespace CogniteSdk.Assets

open Oryx
open Thoth.Json.Net

[<AutoOpen>]
module AssetJsonExtensions =

    type AssetFilter with
        static member Render (this: AssetFilter) =
            match this with
            | CaseName name -> "name", Encode.string name
            | CaseParentIds ids -> "parentIds", Encode.int53seq ids
            | CaseRootIds ids -> "rootIds", ids |> Seq.map(fun id -> id.Encoder) |> Encode.seq
            | CaseSource source -> "source", Encode.string source
            | CaseMetaData md -> "metadata", Encode.propertyBag md
            | CaseCreatedTime time -> "createdTime", time.Encoder
            | CaseLastUpdatedTime time -> "lastUpdatedTime", time.Encoder
            | CaseRoot root -> "root", Encode.bool root
            | CaseExternalIdPrefix prefix -> "externalIdPrefix", Encode.string prefix


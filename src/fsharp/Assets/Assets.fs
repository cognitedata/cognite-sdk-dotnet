// Copyright 2019 Cognite AS
// SPDX-License-Identifier: Apache-2.0

namespace FSharp.Cognite

open Oryx.Cognite

[<RequireQualifiedAccess>]
module Assets =

    let list (queries: Types.AssetQuery) (filters: AssetFilter seq) =
        Assets.list
        ()

// Copyright 2019 Cognite AS
// SPDX-License-Identifier: Apache-2.0

namespace Oryx.Cognite.Assets

open FSharp.Control.Tasks.V2.ContextInsensitive
open Oryx
open Oryx.SystemTextJson.ResponseReader
open Oryx.Cognite

open CogniteSdk.Types.Assets
open System.Net.Http


[<RequireQualifiedAccess>]
module Entity =
    [<Literal>]
    let Url = "/assets"


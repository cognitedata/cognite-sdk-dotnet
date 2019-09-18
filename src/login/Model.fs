// Copyright 2019 Cognite AS
// SPDX-License-Identifier: Apache-2.0

namespace CogniteSdk.Login

open System.Web
open Oryx

type ClientExtension internal (context: HttpContext) =
    member internal __.Ctx =
        context


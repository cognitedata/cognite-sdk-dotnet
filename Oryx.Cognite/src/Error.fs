// Copyright 2019 Cognite AS
// SPDX-License-Identifier: Apache-2.0

namespace Oryx.Cognite

open System

type JsonDecodeException (message: string) =
    inherit Exception(message)
    do ()

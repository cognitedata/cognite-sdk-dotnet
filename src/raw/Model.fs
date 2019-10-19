// Copyright 2019 Cognite AS
// SPDX-License-Identifier: Apache-2.0

namespace CogniteSdk.Raw

open System.Collections.Generic
open Oryx

/// Read/write raw database type.
type DatabaseEntity internal (name: string) =
    member val Name : string = name with get, set

    new () = DatabaseEntity(name=null)

type DatabaseReadDto = {
    Name: string
} with
    member this.ToDatabaseEntity () : DatabaseEntity =
        DatabaseEntity(this.Name)


[<CLIMutable>]
type DatabaseItems = {
    Items: DatabaseEntity seq
    NextCursor: string
}

type DatabaseItemsReadDto = {
    Items: DatabaseReadDto seq
    NextCursor : string option
}

type ClientExtension internal (context: HttpContext) =
    member internal __.Ctx =
        context

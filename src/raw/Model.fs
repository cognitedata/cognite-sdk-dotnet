// Copyright 2019 Cognite AS
// SPDX-License-Identifier: Apache-2.0

namespace CogniteSdk.Raw

open System.Collections.Generic
open Oryx
open CogniteSdk.Common

/// Read raw database type.
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

/// Read raw database table type.
type TableEntity internal (name: string) =
    member val Name : string = name with get, set

    new () = TableEntity(name=null)

type TableReadDto = {
    Name: string
} with
    member this.ToTableEntity () : TableEntity =
        TableEntity(this.Name)


[<CLIMutable>]
type TableItems = {
    Items: TableEntity seq
    NextCursor: string
}

type TableItemsReadDto = {
    Items: TableReadDto seq
    NextCursor : string option
}

type DatabaseQuery =
    private
    | CaseLimit of int
    | CaseCursor of string

    /// Max number of results to return
    static member Limit limit =
        if limit > MaxLimitSize || limit < 1 then
            failwith "Limit must be set to 1000 or less"
        CaseLimit limit
    /// Cursor return from previous request
    static member Cursor cursor = CaseCursor cursor

    static member Render (this: DatabaseQuery) =
        match this with
        | CaseLimit limit -> "limit", limit.ToString()
        | CaseCursor cursor -> "cursor", cursor.ToString()

type ClientExtension internal (context: HttpContext) =
    member internal __.Ctx =
        context

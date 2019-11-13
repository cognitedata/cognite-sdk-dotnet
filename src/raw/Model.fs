// Copyright 2019 Cognite AS
// SPDX-License-Identifier: Apache-2.0

namespace CogniteSdk.Raw

open System.Collections.Generic
open Oryx
open Thoth.Json.Net
open CogniteSdk.Common
open CogniteSdk

/// Read raw database type.
type DatabaseEntity internal (name: string) =
    member val Name : string = name with get, set

    new () = DatabaseEntity(name=null)

type DatabaseDto = {
    Name: string
} with
    member this.ToDatabaseEntity () : DatabaseEntity =
        DatabaseEntity(this.Name)


[<CLIMutable>]
type DatabaseItems = {
    Items: DatabaseEntity seq
    NextCursor: string
}

type DatabaseItemsDto = {
    Items: DatabaseDto seq
    NextCursor : string option
}

/// Read raw database table type.
type TableEntity internal (name: string) =
    member val Name : string = name with get, set

    new () = TableEntity(name=null)

type TableDto = {
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
    Items: TableDto seq
    NextCursor : string option
}

/// Read raw database table row type.
type RowEntity internal (key: string, columns: IDictionary<string, JsonValue>, lastUpdatedTime: int64) =
    member val Key : string = key with get, set
    member val Columns : IDictionary<string, JsonValue> = columns with get, set
    member val LastUpdatedTime : int64 = lastUpdatedTime with get, set

    new () = RowEntity(key=null, columns=null, lastUpdatedTime=0L)

type RowReadDto = {
    Key: string
    Columns: IDictionary<string, JsonValue>
    LastUpdatedTime: int64
} with
    member this.ToRowEntity () : RowEntity =
        RowEntity(this.Key, this.Columns, this.LastUpdatedTime)

type RowWriteDto = {
    Key: string
    Columns: IDictionary<string, JsonValue>
} with
    member this.ToRowEntity () : RowEntity =
        RowEntity(this.Key, this.Columns, 0L)
    static member FromRowEntity (entity: RowEntity) : RowWriteDto =
        { Key = entity.Key; Columns = entity.Columns }

[<CLIMutable>]
type RowItems = {
    Items: RowEntity seq
    NextCursor: string
}

type RowItemsReadDto = {
    Items: RowReadDto seq
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

type DatabaseRowQuery =
    private
    | CaseLimit of int
    | CaseCursor of string
    | CaseMinLastUpdatedTime of int64
    | CaseMaxLastUpdatedTime of int64
    | CaseColumns of string list

    /// Max number of results to return
    static member Limit limit =
        if limit > MaxLimitSize || limit < 1 then
            failwith "Limit must be set to 1000 or less"
        CaseLimit limit

    /// Cursor return from previous request
    static member Cursor cursor = CaseCursor cursor

    /// Minimum last time the row was updated
    static member MinLastUpdatedTime minLastUpdatedTime = CaseMinLastUpdatedTime minLastUpdatedTime

    /// Maximum last time the row was updated
    static member MaxLastUpdatedTime maxLastUpdatedTime = CaseMaxLastUpdatedTime maxLastUpdatedTime

    /// Ordered list of column keys
    static member Columns columns = CaseColumns columns

    static member Render (this: DatabaseRowQuery) =
        match this with
        | CaseLimit limit -> "limit", limit.ToString()
        | CaseCursor cursor -> "cursor", cursor.ToString()
        | CaseMinLastUpdatedTime t -> "minLastUpdatedTime", t.ToString()
        | CaseMaxLastUpdatedTime t -> "maxLastUpdatedTime", t.ToString()
        | CaseColumns columns -> "columns", String.concat "," columns

type ClientExtension internal (context: HttpContext) =
    member internal __.Ctx =
        context

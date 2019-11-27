// Copyright 2019 Cognite AS
// SPDX-License-Identifier: Apache-2.0

namespace CogniteSdk.Sequences

open System.Collections.Generic
open System.Runtime.InteropServices

open Oryx

type ValueType =
    | STRING = 1
    | DOUBLE = 2
    | LONG = 3

type RowValue =
    private
    | CaseString of string
    | CaseDouble of double
    | CaseLong of int64

    static member String str = CaseString str
    static member Double d = CaseDouble d
    static member Long l = CaseLong l

    override this.ToString() =
        match this with
        | CaseString value -> sprintf "STRING %s" value
        | CaseDouble value -> sprintf "DOUBLE %f" value
        | CaseLong value -> sprintf "LONG %d" value

    member this.ValueType () =
        match this with
        | CaseString _ -> ValueType.STRING
        | CaseDouble _ -> ValueType.DOUBLE
        | CaseLong _ -> ValueType.LONG

    member public this.TryGetValue([<Out>] result : string byref) : bool =
        match this with
        | CaseString value -> result <- value; true
        | _ -> false

    member public this.TryGetValue([<Out>] result : double byref) : bool =
        match this with
        | CaseDouble value -> result <- value; true
        | _ -> false

    member public this.TryGetValue([<Out>] result : int64 byref) : bool =
        match this with
        | CaseLong value -> result <- value; true
        | _ -> false

type ColumnEntity internal (name: string, externalId: string, description: string, valueType: ValueType, metadata: IDictionary<string, string>, createdTime: int64, lastUpdatedTime: int64) =
    /// The name of the column.
    member val Name : string = name with get, set
    /// The externalId of the column. Must be unique within the project.
    member val ExternalId : string = externalId with get, set
    /// The description of the column.
    member val Description : string = description with get, set
    /// The valueType of the column. Enum STRING, DOUBLE, LONG
    member val ValueType : ValueType = valueType with get, set
    /// Custom, application specific metadata. String key -> String value
    member val MetaData : IDictionary<string, string> = metadata with get, set
    /// Time when this column was created in CDF in milliseconds since Jan 1, 1970.
    member val CreatedTime : int64 = createdTime with get
    /// The last time this column was updated in CDF, in milliseconds since Jan 1, 1970.
    member val LastUpdatedTime : int64 = lastUpdatedTime with get

    /// Create new empty ColumnEntity. Set content using the properties.
    new () =
        ColumnEntity(name=null, externalId=null, description=null, valueType=ValueType.DOUBLE, metadata=null, createdTime=0L, lastUpdatedTime=0L)

type SequenceEntity internal (id: int64, name: string, externalId: string, description: string, assetId: int64, metadata: IDictionary<string, string>, columns: ColumnEntity seq, createdTime: int64, lastUpdatedTime: int64) =
    /// The Id of the sequence
    member val Id : int64 = id with get
    /// The name of the sequence.
    member val Name : string = name with get, set
    /// The description of the sequence.
    member val Description : string = description with get, set
    /// The valueType of the sequence. Enum STRING, DOUBLE, LONG
    member val AssetId : int64 = assetId with get, set
    /// The externalId of the sequence. Must be unique within the project.
    member val ExternalId : string = externalId with get, set
    /// Custom, application specific metadata. String key -> String value
    member val MetaData : IDictionary<string, string> = metadata with get, set
    /// Time when this sequence was created in CDF in milliseconds since Jan 1, 1970.
    member val Columns : ColumnEntity seq = columns with get, set
    /// Time when this sequence was created in CDF in milliseconds since Jan 1, 1970.
    member val CreatedTime : int64 = createdTime with get
    /// The last time this sequence was updated in CDF, in milliseconds since Jan 1, 1970.
    member val LastUpdatedTime : int64 = lastUpdatedTime with get

    /// Create new empty SequenceEntity. Set content using the properties.
    new () =
        SequenceEntity(id=0L, name=null, description=null, assetId=0L, externalId=null, metadata=null, columns=null, createdTime=0L, lastUpdatedTime=0L)

[<CLIMutable>]
type SequenceItems = {
    Items: SequenceEntity seq
    NextCursor: string
}

type RowEntity (rowNumber: int64, values: RowValue seq) =
    member val RowNumber : int64 = rowNumber with get, set
    member val values : RowValue seq = values with get, set

type ColumnInfoReadEntity(externalId: string, name: string, valueType: ValueType) =
    member val ExternalId : string = externalId with get, set
    member val Name : string = name with get, set
    member val ValueType : ValueType = valueType with get, set


type SequenceDataReadEntity(identity: int64, externalId: string, columns: ColumnInfoReadEntity seq, rows: RowEntity seq, nextCursor: string) =
    member val Id : int64 = identity with get, set
    member val ExternalId : string = externalId with get, set
    member val Columns : ColumnInfoReadEntity seq = columns with get, set
    member val Rows : RowEntity seq = rows with get, set

type ClientExtension internal (context: HttpContext) =
    member internal __.Ctx =
        context

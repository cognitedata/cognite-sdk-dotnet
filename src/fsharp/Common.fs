module FSharp.Oryx.Cognite.Common

open System
open CogniteSdk.Types.Common

let inline isNull value = obj.ReferenceEquals(value, null)
let inline toOption value =
    if isNull value then None
    else Some value
let inline nullableToOption (value: Nullable<'a>) : 'a option =
    if value.HasValue then Some value.Value
    else None

let inline toMap kvps =
    kvps
    |> Seq.map (|KeyValue|)
    |> Map.ofSeq

type ItemsWithCursor<'a> = {
    Items : 'a seq
    NextCursor : string option
} with
    static member FromItemsWithCursor (itemConverter: 'b -> 'a) (items: ResourceItemsWithCursor<'b>) =
        {
            Items = Seq.map itemConverter items.Items
            NextCursor = toOption items.NextCursor
        }
type Identity =
    internal
    | CaseId of int64
    | CaseExternalId of string

    static member Id id =
        CaseId id
    static member ExternalId id =
        CaseExternalId id
    member this.ToIdentityDto : CogniteSdk.Identity =
        match this with
        | CaseId identity -> IdentityId(identity) :> CogniteSdk.Identity
        | CaseExternalId externalId -> IdentityExternalId(externalId) :> CogniteSdk.Identity



[<AutoOpen>]
module Patterns =
    /// Active pattern to permit pattern matching over numeric values.
    let (|Integer|Float|String|) (value : Numeric) : Choice<int64, float, string>  =
        match value with
        | CaseInteger value -> Integer value
        | CaseFloat value -> Float value
        | CaseString value -> String value

    /// Active pattern to permit pattern matching over identity values.
    let (|Id|ExternalId|) (value : Identity) : Choice<int64, string>  =
        match value with
        | CaseId value -> Id value
        | CaseExternalId value -> ExternalId value

    let (|ParseInteger|_|) (str: string) =
       let mutable intvalue = 0
       if System.Int32.TryParse(str, &intvalue) then Some(intvalue)
       else None

    let (|ParseRegex|_|) regex str =
       let m = Regex(regex).Match(str)
       if m.Success
       then Some (List.tail [ for x in m.Groups -> x.Value ])
       else None

/// Id or ExternalId
type Numeric =
    internal
    | CaseString of string
    | CaseInteger of int64
    | CaseFloat of double

    static member String value =
        CaseString value

    static member Integer value =
        CaseInteger value

    static member Float value =
        CaseFloat value


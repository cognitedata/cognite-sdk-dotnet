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

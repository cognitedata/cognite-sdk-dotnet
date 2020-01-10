// Copyright 2019 Cognite AS
// SPDX-License-Identifier: Apache-2.0

namespace Oryx.Cognite

open System
open System.Collections.Generic
open Thoth.Json.Net

type JsonDecodeException (message: string) =
    inherit Exception(message)
    do ()

type ErrorValue =
    | IntegerValue of int
    | FloatValue of float
    | StringValue of string

type ResponseError = {
    Code : int
    Message : string
    Missing : Map<string, ErrorValue> seq
    Duplicated : Map<string, ErrorValue> seq
    RequestId : string option } with

        static member empty = {
           Code = 400
           Message = String.Empty
           Missing = Seq.empty
           Duplicated = Seq.empty
           RequestId = None
       }


type ResponseException (message: string) =
    inherit Exception(message)

    member val Code = 400 with get, set
    member val Missing : IEnumerable<IDictionary<string, IErrorValue>> = Seq.empty with get, set
    member val Duplicated : IEnumerable<IDictionary<string, IErrorValue>> = Seq.empty with get, set
    member val RequestId : string = null with get, set

[<AutoOpen>]
module Error =
    type ResponseError with
        member this.ToException () =
            let convertTypes (_: string) (value: ErrorValue) : IErrorValue =
                match value with
                | IntegerValue value -> IntegerValue value :> IErrorValue
                | FloatValue value -> FloatValue value :> _
                | StringValue value -> StringValue value :> _

            let ex = ResponseException this.Message
            ex.Code <- this.Code
            ex.Duplicated <- this.Duplicated |> Seq.map (Map.map convertTypes >> Map.toSeq >> dict)
            ex.Missing <- this.Missing |> Seq.map (Map.map convertTypes >> Map.toSeq >> dict)
            ex.RequestId <- if this.RequestId.IsSome then this.RequestId.Value else null
            ex

    [<Literal>]
    let MaxLimitSize = 1000


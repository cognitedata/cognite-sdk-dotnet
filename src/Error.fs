// Copyright 2019 Cognite AS
// SPDX-License-Identifier: Apache-2.0

namespace CogniteSdk

open System
open System.Collections.Generic

open Oryx

type ErrorValue () = do ()

type IntegerValue (value: int) =
    inherit ErrorValue ()
    member val Integer = value with get, set

    override this.ToString () =
        sprintf "%d" this.Integer

and FloatValue (value) =
    inherit ErrorValue ()

    member val Float = value with get, set
    override this.ToString () =
        sprintf "%f" this.Float

and StringValue (value) =
    inherit ErrorValue ()
    member val String = value with get, set
    override this.ToString () =
        sprintf "%s" this.String

type ResponseException (message: string) =
    inherit Exception(message)

    member val Code = 400 with get, set
    member val Missing : IEnumerable<IDictionary<string, ErrorValue>> = Seq.empty with get, set
    member val Duplicated : IEnumerable<IDictionary<string, ErrorValue>> = Seq.empty with get, set
    member val InnerException : Exception = null with get, set

[<AutoOpen>]
module Error =
    type ResponseError with
        member this.ToException () =
            let convertTypes (_: string) (value: Oryx.ErrorValue) =
                match value with
                | IntegerValue value -> IntegerValue value :> ErrorValue
                | FloatValue value -> FloatValue value :> _
                | StringValue value -> StringValue value :> _

            let ex = ResponseException this.Message
            ex.Code <- this.Code
            ex.Duplicated <- this.Duplicated |> Seq.map (Map.map convertTypes >> Map.toSeq >> dict)
            ex.Missing <- this.Missing |> Seq.map (Map.map convertTypes >> Map.toSeq >> dict)
            ex.InnerException <- if this.InnerException.IsSome then this.InnerException.Value else null
            ex

    [<Literal>]
    let MaxLimitSize = 1000


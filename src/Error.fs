// Copyright 2019 Cognite AS
// SPDX-License-Identifier: Apache-2.0

namespace CogniteSdk

open System
open System.Collections.Generic

open Oryx

type ResponseException (message : string) =
    inherit Exception(message)

    member val Code = 400 with get, set
    member val Missing : IEnumerable<IDictionary<string, ErrorValue>> = Seq.empty with get, set
    member val Duplicated : IEnumerable<IDictionary<string, ErrorValue>> = Seq.empty with get, set
    member val InnerException : Exception = null with get, set

and IntegerValue (value: int) =
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

[<AutoOpen>]
module Error =
    [<Literal>]
    let MaxLimitSize = 1000

    /// Convert Oryx types to CogniteSdk types for convenience to avoid having to open the Oryx namespace.
    let convertTypes (_: string) (value: ErrorValue) =
        match value with
        | :? Oryx.IntegerValue as value -> IntegerValue value.Integer :> ErrorValue
        | :? Oryx.FloatValue  as value -> FloatValue value.Float :> _
        | :? Oryx.StringValue  as value -> StringValue value.String :> _
        | _ -> failwith "Unknown type"

    /// **Description**
    ///
    /// Translate response error to exception that we can raise for the
    /// C# API.
    ///
    /// **Parameters**
    ///   * `error` - parameter of type `ResponseError`
    ///
    /// **Output Type**
    ///   * `exn`
    let error2Exception error =
        let ex = ResponseException error.Message
        ex.Code <- error.Code
        ex.Duplicated <- error.Duplicated |> Seq.map (Map.map convertTypes >> Map.toSeq >> dict)
        ex.Missing <- error.Missing |> Seq.map (Map.map convertTypes >> Map.toSeq >> dict)
        ex.InnerException <- if error.InnerException.IsSome then error.InnerException.Value else null
        ex :> Exception
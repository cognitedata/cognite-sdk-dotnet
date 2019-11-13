// Copyright 2019 Cognite AS
// SPDX-License-Identifier: Apache-2.0

namespace CogniteSdk

open System
open System.Collections.Generic
open Thoth.Json.Net

exception JsonDecodeException of string

type ErrorValue =
    | IntegerValue of int
    | FloatValue of float
    | StringValue of string

    with
        static member Decoder : Decoder<ErrorValue> =
            Decode.oneOf [
                Decode.int |> Decode.map IntegerValue
                Decode.float |> Decode.map FloatValue
                Decode.string |> Decode.map StringValue
            ]

type ResponseError = {
    Code : int
    Message : string
    Missing : Map<string, ErrorValue> seq
    Duplicated : Map<string, ErrorValue> seq
    InnerException : exn option } with
        static member Decoder : Decoder<ResponseError> =
            Decode.object (fun get ->
                {
                    Code = get.Required.Field "code" Decode.int
                    Message = get.Required.Field "message" Decode.string
                    Missing =
                        let missing = get.Optional.Field "missing" (Decode.array (Decode.dict ErrorValue.Decoder))
                        match missing with
                        | Some missing -> missing |> Seq.ofArray
                        | None -> Seq.empty
                    Duplicated =
                        let duplicated = get.Optional.Field "duplicated" (Decode.array (Decode.dict ErrorValue.Decoder))
                        match duplicated with
                        | Some duplicated -> duplicated |> Seq.ofArray
                        | None -> Seq.empty
                    InnerException = None
                })

        static member empty = {
           Code = 400
           Message = String.Empty
           Missing = Seq.empty
           Duplicated = Seq.empty
           InnerException = None
       }

type ApiResponseError = {
    Error : ResponseError
} with
    static member Decoder =
        Decode.object (fun get ->
            {
                Error = get.Required.Field "error" ResponseError.Decoder
            }
        )

type ErrorValueBase () = do ()

type IntegerValue (value: int) =
    inherit ErrorValueBase ()
    member val Integer = value with get, set

    override this.ToString () =
        sprintf "%d" this.Integer

and FloatValue (value) =
    inherit ErrorValueBase ()

    member val Float = value with get, set
    override this.ToString () =
        sprintf "%f" this.Float

and StringValue (value) =
    inherit ErrorValueBase ()
    member val String = value with get, set
    override this.ToString () =
        sprintf "%s" this.String

type ResponseException (message: string) =
    inherit Exception(message)

    member val Code = 400 with get, set
    member val Missing : IEnumerable<IDictionary<string, ErrorValueBase>> = Seq.empty with get, set
    member val Duplicated : IEnumerable<IDictionary<string, ErrorValueBase>> = Seq.empty with get, set
    member val InnerException : Exception = null with get, set

[<AutoOpen>]
module Error =
    type ResponseError with
        member this.ToException () =
            let convertTypes (_: string) (value: ErrorValue) =
                match value with
                | IntegerValue value -> IntegerValue value :> ErrorValueBase
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


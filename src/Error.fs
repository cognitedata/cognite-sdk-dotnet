namespace Fusion

open System
open System.Net.Http
open System.Collections.Generic

open Thoth.Json.Net

/// Will be raised if decoding a response fails.
exception DecodeException of string

type ErrorValue () =
    static member val Decoder : Decoder<ErrorValue> =
        Decode.oneOf [
            Decode.int |> Decode.map (fun value -> IntegerValue value :> ErrorValue)
            Decode.float |> Decode.map (fun value -> FloatValue value :> ErrorValue)
            Decode.string |> Decode.map (fun value ->StringValue value :> ErrorValue)
        ]

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

type ResponseException (message : string) =
    inherit Exception(message)

    member val Code = 400 with get, set
    member val Missing : IEnumerable<IDictionary<string, ErrorValue>> = Seq.empty with get, set
    member val Duplicated : IEnumerable<IDictionary<string, ErrorValue>> = Seq.empty with get, set

    with
        static member Decoder : Decoder<ResponseException> =
            Decode.object (fun get ->
                let message = get.Required.Field "message" Decode.string

                let error = ResponseException message
                error.Code <- get.Required.Field "code" Decode.int


                let missing = get.Optional.Field "missing" (Decode.array (Decode.dict ErrorValue.Decoder))
                match missing with
                | Some missing ->
                    error.Missing <- missing |> Seq.map (Map.toSeq >> dict)
                | None -> ()

                let duplicated = get.Optional.Field "duplicated" (Decode.array (Decode.dict ErrorValue.Decoder))
                match duplicated with
                | Some duplicated ->
                    error.Duplicated <- duplicated |> Seq.map (Map.toSeq >> dict)
                | None -> ()

                error
            )

type ResponseError =
    /// Exception (internal error). This should never happen.
    | Exception of exn
    /// JSON decode error (unable to decode the response)
    | DecodeError of string
    /// Error response from server.
    | HttpResponse of HttpResponseMessage*IO.Stream

type ErrorResponse = {
    Error : ResponseException
} with
    static member Decoder =
        Decode.object (fun get ->
            {
                Error = get.Required.Field "error" ResponseException.Decoder
            }
        )

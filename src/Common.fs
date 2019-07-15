/// Common types for the SDK.
namespace Cognite.Sdk

open System

open Thoth.Json.Net
open Newtonsoft.Json
open System.Text.RegularExpressions
open FSharp.Control.Tasks.V2.ContextInsensitive
open System.IO
open Newtonsoft.Json.Linq

/// Id or ExternalId
type Identity =
    internal
    | CaseId of int64
    | CaseExternalId of string

    static member Id id =
        CaseId id
    static member ExternalId id =
        CaseExternalId id

    member this.Encoder =
        Encode.object [
            match this with
            | CaseId id -> yield "id", Encode.int53 id
            | CaseExternalId id -> yield "externalId", Encode.string id
        ]

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

[<AutoOpen>]
module Patterns =
    /// Active pattern to permit pattern matching over numeric values.
    let (|Integer|Float|String|) (value : Numeric) : Choice<int64, float, string>  =
        match value with
        | CaseInteger value -> Integer value
        | CaseFloat value -> Float value
        | CaseString value -> String value

    let (|ParseInteger|_|) (str: string) =
       let mutable intvalue = 0
       if System.Int32.TryParse(str, &intvalue) then Some(intvalue)
       else None

    let (|ParseRegex|_|) regex str =
       let m = Regex(regex).Match(str)
       if m.Success
       then Some (List.tail [ for x in m.Groups -> x.Value ])
       else None

module Common =
    [<Literal>]
    let MaxLimitSize = 1000

    let decodeStreamAsync (decoder : Decoder<'a>) (stream : IO.Stream) =
        task {
            use tr = new StreamReader(stream)
            use jtr = new JsonTextReader(tr)
            let! json = JValue.ReadFromAsync jtr

            let result = Decode.fromValue "$" decoder json

            stream.Dispose ()
            return result
        }

    /// **Description**
    ///
    /// JSON decode response and map decode error string to exception so we
    /// don't get more response error types.
    ///
    /// **Parameters**
    ///   * `decoder` - parameter of type `'a`
    ///   * `result` - parameter of type `Result<'b,'c>`
    ///
    /// **Output Type**
    ///   * `Result<'d,'c>`
    ///
    /// **Exceptions**
    ///
    let decodeResponse<'a, 'b, 'c> (decoder : Decoder<'a>) (resultMapper : 'a -> 'b) (next: NextHandler<'b,'c>) (context: Context<Stream>) =
        async {
            let result = context.Result

            let! nextResult = async {
                match result with
                | Ok stream ->
                    let! ret = decodeStreamAsync decoder stream |> Async.AwaitTask
                    match ret with
                    | Ok value -> return value |> resultMapper |> Ok
                    | Error error -> return (DecodeError error |> Error)
                | Error err -> return Error err
            }

            return! next { Request = context.Request; Result = nextResult }
        }

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
    let error2Exception error = task {
        match error with
        | Exception ex -> return ex
        | DecodeError err -> return DecodeException err
        | HttpResponse (response, stream) ->
            let! result = decodeStreamAsync ErrorResponse.Decoder stream
            let error =
                match result with
                | Ok error -> error.Error
                | Error message ->
                    let error = ResponseException message
                    error.Code <-  int response.StatusCode
                    error
            return error :> Exception
    }

    type Decode.IGetters with
        member this.NullableField name decoder =
            match this.Optional.Field name decoder with
                | Some value -> Nullable(value)
                | None -> Nullable()

        member this.NullableReferenceField name decoder =
            match this.Optional.Field name decoder with
                | Some value -> value
                | None -> null



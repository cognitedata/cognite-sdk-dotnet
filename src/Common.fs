/// Common types for the SDK.
namespace Fusion

open System
open System.IO
open System.Text.RegularExpressions
open Thoth.Json.Net

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

    member this.Render =
        match this with
        | CaseId id -> "id", Encode.int64 id
        | CaseExternalId id -> "externalId", Encode.string id

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

type TimeRange = {
    Max: DateTime
    Min: DateTime
} with
    member this.Encoder =
        Encode.object [
            yield "max", DateTimeOffset(this.Max).ToUnixTimeMilliseconds() |> Encode.int64
            yield "min", DateTimeOffset(this.Min).ToUnixTimeMilliseconds() |> Encode.int64
        ]
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

module Common =
    [<Literal>]
    let MaxLimitSize = 1000

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
                    | Error message ->
                        return {
                            ResponseError.empty with
                                Message = message
                        } |> Error
                | Error err -> return Error err
            }

            return! next { Request = context.Request; Result = nextResult }
        }
    let decodeProtobuf<'b, 'c> (parser : Stream -> 'b) (next: NextHandler<'b, 'c>) (context : Context<Stream>) =
        async {
            let result = context.Result |> Result.map parser
            return! next { Request = context.Request; Result = result }
        }

    /// Handler for disposing the stream when it's not needed anymore.
    let dispose<'a> (next: NextHandler<unit,'a>) (context: Context<Stream>) =
        async {
            let nextResult = context.Result |> Result.map (fun stream -> stream.Dispose ())
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
    let error2Exception error =
        let ex = ResponseException error.Message
        ex.Code <- error.Code
        ex.Duplicated <- error.Duplicated |> Seq.map (Map.toSeq >> dict)
        ex.Missing <- error.Missing |> Seq.map (Map.toSeq >> dict)
        ex.InnerException <- if error.InnerException.IsSome then error.InnerException.Value else null
        ex :> Exception

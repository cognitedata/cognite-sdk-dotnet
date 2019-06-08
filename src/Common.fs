/// Common types for the SDK.
namespace Cognite.Sdk

open FSharp.Data
open Thoth.Json.Net
open Newtonsoft.Json
open Newtonsoft.Json.Linq

module Common =
    type Numeric =
        | NumString of string
        | NumInteger of int64
        | NumFloat of double

    /// Id or ExternalId
    type Identity =
        | Id of int64
        | ExternalId of string

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
    let decodeResponse decoder resultMapper (context: HttpContext) =
        let result =
            context.Result
            |> Result.map (fun response ->
                match response.Body with
                | Text text ->
                    //printfn "Got: %A" text
                    text
                | Binary _ ->
                    failwith "binary format not supported"
            )
            |> Result.bind (fun res ->
                let ret = Decode.fromString decoder res
                match ret with
                | Error error -> DecodeError error |> Error
                | Ok value -> Ok value
            )
            |> Result.map resultMapper

        Async.single { Request = context.Request; Result = result}

[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module Encode =
    let inline stringify encoder =
        Encode.toString 0 encoder

    /// Encode int64 to number (not to string as Thoth.Json.Net)
    let inline int53 (value : int64) : JsonValue =
        JValue(value) :> JsonValue

    let inline metaData (values: Map<string, string>) =  values |> Map.map (fun _ value -> Encode.string value) |> Encode.dict

    let inline int64seq (items: int64 seq) = Seq.map int53 items |> Encode.seq

/// Common types for the SDK.
namespace Cognite.Sdk

open FSharp.Data
open Thoth.Json.Net

module Common =

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
    let decodeResponse decoder (result: Result<HttpResponse, ResponseError>) =
        result
        |> Result.map (fun response ->
            match response.Body with
            | Text text ->
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
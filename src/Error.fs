namespace Cognite.Sdk

open System
open FSharp.Data

/// Will be raised if decoding a response fails.
exception DecodeException of string

type ResponseError =
    | RequestException of exn
    | DecodeError of string
    | ErrorResponse of HttpResponse

type RequestError = {
    Code: int
    Message: string
    Extra: Map<string, string>
}

module Error =
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
        match error with
        | RequestException ex ->
            ex
        | DecodeError err ->
            DecodeException err
        | ErrorResponse err ->
            // FIXME: Make a better error type
            Exception (err.ToString ())



namespace Cognite.Sdk

open System
open FSharp.Data

/// Will be raised if decoding a response fails.
exception DecodeException of string
exception MyFSharpError1 of string

type ResponseError =
    /// Exception (internal error). This should never happen.
    | RequestException of exn
    /// JSON decode error (unable to decode the response)
    | DecodeError of string
    /// Error response from server.
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
            // FIXME: Make a better error type. I.e decode into RequestError
            Exception (err.ToString ())



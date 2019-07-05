/// Common types for the SDK.
namespace Cognite.Sdk

open System

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
            | Identity.CaseId id -> yield "id", Encode.int53 id
            | Identity.CaseExternalId id -> yield "externalId", Encode.string id
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
    let decodeResponse<'a, 'b, 'c> (decoder : Decoder<'a>) (resultMapper : 'a -> 'b) (next: NextHandler<'b,'c>) (context: Context<string>) =
        let result =
            context.Result
            |> Result.bind (fun res ->
                let ret = Decode.fromString decoder res
                match ret with
                | Error error -> DecodeError error |> Error
                | Ok value -> Ok value
            )
            |> Result.map resultMapper

        next { Request = context.Request; Result = result }


    type Decode.IGetters with
        member this.NullableField name decoder =
            match this.Optional.Field name decoder with
                | Some value -> Nullable(value)
                | None -> Nullable()

        member this.NullableReferenceField name decoder =
            match this.Optional.Field name decoder with
                | Some value -> value
                | None -> null


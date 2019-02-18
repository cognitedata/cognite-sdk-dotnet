namespace Cognite.Sdk.Api

open Cognite.Sdk.Context

type Context () =
    let mutable context = defaultContext

    member this.AddHeader(header: struct (string*string))  =
        match header with
        | struct (a, b) ->
            context <- context |> addHeader (a, b)
        this


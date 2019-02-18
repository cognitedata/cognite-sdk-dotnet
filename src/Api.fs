namespace Cognite.Sdk.Api

open Cognite.Sdk.Context

type Context (context) =
    let context = context

    new() = Context(defaultContext)

    member this.AddHeader(name: string, value: string)  =
        context
        |> addHeader (name, value)
        |> Context

    member this.SetProject(project: string) =
        context
        |> setProject(project)
        |> Context

module Tests.Integration.Sequences

open System
open System.Net.Http
open FSharp.Control.Tasks.V2.ContextInsensitive

open Xunit
open Swensen.Unquote

open Oryx
open Thoth.Json.Net

open CogniteSdk
open CogniteSdk.Sequences
open Common

[<Trait("resource", "sequences")>]
[<Fact>]
let ``List Sequences with limit is Ok`` () = task {
    // Arrange
    let ctx = writeCtx ()
    let query = [ SequenceQuery.Limit 10 ]

    // Act
    let! res = Items.listAsync query [] ctx

    let ctx' =
        match res with
        | Ok ctx -> ctx
        | Error err -> raise <| err.ToException ()

    let dtos = ctx'.Response
    let len = Seq.length dtos.Items

    // Assert
    test <@ len > 0 @>
    test <@ ctx'.Request.Method = HttpMethod.Post @>
    test <@ ctx'.Request.Extra.["resource"] = "/sequences/list" @>
}

[<Trait("resource", "sequences")>]
[<Fact>]
let ``List Sequences with limit and filter is Ok`` () = task {
    // Arrange
    let ctx = writeCtx ()
    let query = [ SequenceQuery.Limit 10 ]
    let filter = [ SequenceFilter.Name "sdk-test-sequence" ]

    // Act
    let! res = Items.listAsync query filter ctx

    let ctx' =
        match res with
        | Ok ctx -> ctx
        | Error err -> raise <| err.ToException ()

    let dtos = ctx'.Response
    let len = Seq.length dtos.Items

    // Assert
    test <@ len > 0 @>
    test <@ ctx'.Request.Method = HttpMethod.Post @>
    test <@ ctx'.Request.Extra.["resource"] = "/sequences/list" @>
}

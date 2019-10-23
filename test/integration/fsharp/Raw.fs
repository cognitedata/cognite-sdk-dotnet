module Tests.Integration.Raw

open System
open System.Net.Http
open System.Threading.Tasks

open Xunit
open Swensen.Unquote

open Tests
open Common
open Oryx
open Oryx.Retry
open FSharp.Control.Tasks.V2.ContextInsensitive

open CogniteSdk
open CogniteSdk.Raw

[<Trait("resource", "raw")>]
[<Fact>]
let ``List Databases with limit is Ok`` () = task {
    // Arrange
    let ctx = writeCtx ()
    let query = [ DatabaseQuery.Limit 10 ]

    // Act
    let! res = Retrieve.databasesAsync query ctx

    let ctx' =
        match res with
        | Ok ctx -> ctx
        | Error err -> raise <| err.ToException ()

    let dtos = ctx'.Response
    let len =Seq.length dtos

    // Assert
    test <@ len > 0 @>
    test <@ ctx'.Request.Method = HttpMethod.Get @>
    test <@ ctx'.Request.Extra.["resource"] = "/raw/dbs" @>
}

[<Trait("resource", "raw")>]
[<Fact>]
let ``List Tables with limit is Ok`` () = task {
    // Arrange
    let ctx = writeCtx ()
    let query = [ DatabaseQuery.Limit 10 ]

    // Act
    let! res = Retrieve.tablesAsync "sdk-test-database" query ctx

    let ctx' =
        match res with
        | Ok ctx -> ctx
        | Error err -> raise <| err.ToException ()

    let dtos = ctx'.Response
    let len = Seq.length dtos

    // Assert
    test <@ len > 0 @>
    test <@ ctx'.Request.Method = HttpMethod.Get @>
    test <@ ctx'.Request.Extra.["resource"] = "/raw/dbs/sdk-test-database/tables" @>
}

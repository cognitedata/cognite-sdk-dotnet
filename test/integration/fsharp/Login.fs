module Tests.Integration.Login

open System
open System.Net.Http

open Swensen.Unquote
open Xunit

open Oryx
open CogniteSdk

open Common
open FSharp.Control.Tasks.V2.ContextInsensitive

[<Fact>]
let ``Login status is Ok`` () = task {
    // Arrange
    let ctx = readCtx ()

    // Act
    let! res = Login.Status.statusAsync ctx

    let ctx' =
        match res with
        | Ok ctx -> ctx
        | Error (ResponseError error) -> raise <| error.ToException ()
        | Error (Panic error) -> raise error

    let status = ctx'.Response

    // Assert
    test <@ ctx'.Request.Method = HttpMethod.Get @>
    test <@ status.Project = "publicdata" @>
}

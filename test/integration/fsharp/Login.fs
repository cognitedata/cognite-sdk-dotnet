module Tests.Integration.Login

open System
open System.Net.Http

open Swensen.Unquote
open Xunit

open Oryx
open CogniteSdk

open Common
open Tests
open FSharp.Control.Tasks.V2.ContextInsensitive

[<Fact>]
let ``Login redirect is Ok`` () = task {
    // Arrange
    let ctx = readCtx ()
    let redirectUri = Uri "https://api.cognitedata.com"

    // Act
    let! res = Login.Redirect.redirectAsync "publicdata" redirectUri None ctx

    let location =
        match res.Result with
        | Ok location -> Some location
        | Error _ -> None

    // Assert
    test <@ Result.isOk res.Result @>
    test <@ res.Request.Method = HttpMethod.Get @>
    test <@ location.Value.StartsWith "https://accounts.google.com" @>
}

[<Fact>]
let ``Login status is Ok`` () = task {
    // Arrange
    let ctx = readCtx ()

    // Act
    let! res = Login.Status.statusAsync ctx

    let status =
        match res.Result with
        | Ok status -> status
        | Error _ -> failwith "Cannot get status"

    // Assert
    test <@ Result.isOk res.Result @>
    test <@ res.Request.Method = HttpMethod.Get @>
    test <@ status.Project = "publicdata" @>
}

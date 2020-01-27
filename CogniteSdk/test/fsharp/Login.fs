module Tests.Integration.Login

open FSharp.Control.Tasks.V2.ContextInsensitive
open Swensen.Unquote
open Xunit

open Common

[<Fact>]
let ``Login status is Ok`` () = task {
    // Arrange

    // Act
    let! status = readClient.Login.StatusAsync ()

    // Assert
    test <@ status.Project = "publicdata" @>
}

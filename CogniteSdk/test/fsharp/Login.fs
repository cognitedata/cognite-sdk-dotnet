module Tests.Integration.Login

open FSharp.Control.TaskBuilder
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

[<Fact>]
let ``Login status not authenticated gives not loggedIn`` () = task {
    // Arrange

    // Act
    let! status = noAuthClient.Login.StatusAsync ()

    // Assert
    test <@ not status.LoggedIn @>
}

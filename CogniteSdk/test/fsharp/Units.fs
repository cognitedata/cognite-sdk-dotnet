module Tests.Integration.Units

open System
open System.Linq
open FSharp.Control.TaskBuilder
open Xunit
open Swensen.Unquote
open Common
open CogniteSdk

[<Fact>]
let ``Get unit by external id is Ok`` () =
    task {
        // Arrange
        let unitExternalId = "temperature:deg_c"

        // Act
        let! res = readClient.Units.GetUnitAsync unitExternalId

        let resExternalId = res.Items.First().ExternalId

        // Assert
        test <@ resExternalId = unitExternalId @>
    }

[<Fact>]
let ``Get unit by missing external id is Error`` () =
    task {
        // Arrange
        let unitExternalId = "bananas:yellow"

        // Act
        let! res =
            task {
                try
                    let! a = readClient.Units.GetUnitAsync unitExternalId
                    return Ok a
                with :? ResponseException as e ->
                    return Error e
            }

        let err = Result.getError res

        // Assert
        test <@ err.Code = 400 @>
        test <@ err.Message.Contains "was not found" @>
        test <@ not (isNull err.RequestId) @>
    }

[<Fact>]
let ``Retrieve units by external ids is Ok`` () =
    task {
        // Arrange
        let unitExternalIds =
            [ Identity("temperature:deg_c")
              Identity("temperature:deg_f")
              Identity("temperature:k") ]

        // Act
        let! res = readClient.Units.RetrieveUnitsAsync unitExternalIds

        let len = Seq.length res

        // Assert
        test <@ len = 3 @>
    }

[<Fact>]
let ``List all units in the unit catalog is Ok`` () =
    task {
        // Act
        let! res = readClient.Units.ListUnitsAsync()

        let len = Seq.length res.Items

        // Assert
        test <@ len >= 266 @>
    }

[<Fact>]
let ``List all unit systems in the unit catalog is Ok`` () =
    task {
        // Act
        let! res = readClient.Units.ListUnitSystemsAsync()

        let len = Seq.length res.Items

        // Assert
        test <@ len >= 3 @>
    }

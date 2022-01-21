module Tests.Integration.Functions

open System
open System.Collections.Generic

open FSharp.Control.TaskBuilder
open Swensen.Unquote
open Oryx
open Xunit

open CogniteSdk
open Common
open System.Text.Json

// [<Fact>]
// let ``List functions is Ok`` () = task {
//     // Act
//     let! res = writeClient.Playground.Functions.ListAsync()

//     let len = Seq.length res.Items
//     let testFunc = res.Items |> Seq.find (fun item -> item.Name = ".net test function")

//     // Assert
//     test <@ len > 0 @>
//     test <@ testFunc.Name = ".net test function" @>
// }

// [<Fact>]
// let ``Retrieve functions is Ok`` () = task {
//     // Arrange
//     let ids = [ Identity(8287208469954416L) ]
//     // Act
//     let! res = writeClient.Playground.Functions.RetrieveAsync(ids)
//     let len = Seq.length res
//     let testFunc = res |> Seq.head

//     test <@ len = 1 @>
//     test <@ testFunc.Name = ".net test function" @>
// }

// [<Fact>]
// let ``List functionCalls is Ok`` () = task {
//     // Arrange
//     let filter = FunctionCallFilter(Status="Completed")

//     // Act
//     let! res = writeClient.Playground.FunctionCalls.ListAsync(8287208469954416L, filter)
//     let len = Seq.length res.Items

//     // Assert
//     test <@ len > 0 @>
//     test <@ res.Items |> Seq.forall (fun call -> call.Status = "Completed") @>
// }

// TODO: Fix this
// [<Fact>]
// let ``List functionCall logs is Ok`` () = task {
//     // Act
//     let! res = writeClient.Playground.FunctionCalls.ListLogsAsync(8287208469954416L, 2504165293606494L)
//     let len = Seq.length res.Items

//     // Assert
//     test <@ len > 0 @>
// }

// [<Fact>]
// let ``Retrieve functionCalls is Ok`` () = task {
//     // Act
//     let! res = writeClient.Playground.FunctionCalls.GetAsync(8287208469954416L, 2504165293606494L)

//     test <@ res.Status = "Completed" @>
//     test <@ res.Id = 2504165293606494L @>
// }

// TODO: Fix this
// [<Fact>]
// let ``Retrieve response from functionCalls is Ok`` () = task {
//     // Act
//     let! res = writeClient.Playground.FunctionCalls.RetrieveResponse(8287208469954416L, 2504165293606494L)

//     test <@ res.Response.ToString() = "42" @>
// }

// TODO: Fix this
// [<Fact>]
// let ``Call function is Ok`` () = task {
//     // Act
//     let! res = writeClient.Playground.FunctionCalls.CallFunction(8287208469954416L, Identity("test"))

//     test <@ res.Status = "Running" @>
// }

// [<Fact>]
// let ``List function schedules is Ok`` () = task {
//     // Act
//     let! res = writeClient.Playground.FunctionSchedules.ListAsync()

//     let len = Seq.length res.Items
//     let testFunc = res.Items |> Seq.head

//     // Assert
//     test <@ len > 0 @>
//     test <@ testFunc.Name = "dotnet sdk test schedule" @>
// }
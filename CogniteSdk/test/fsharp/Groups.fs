module Tests.Integration.Groups

open System
open System.Collections.Generic

open FSharp.Control.Tasks
open Swensen.Unquote
open Oryx
open Xunit

open CogniteSdk
open Common

[<Fact>]
let ``List own groups is OK`` () = task {
    // Act
    let! res = writeClient.Groups.ListAsync()

    test <@ Seq.length res > 0 @>
    test <@ Seq.exists (fun (g: Group) ->
        Seq.exists (fun (cap: BaseAcl) -> cap :? GroupsAcl) g.Capabilities
    ) res @>
}

[<Fact>]
let ``List all groups is OK`` () = task {
    // Act
    let! res = writeClient.Groups.ListAsync true

    test <@ Seq.length res > 1 @>
    test <@ Seq.exists (fun (g: Group) -> g.Name = "admin") res @>
}

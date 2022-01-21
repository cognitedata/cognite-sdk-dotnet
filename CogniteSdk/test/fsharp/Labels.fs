module Tests.Integration.Labels

open System
open System.Collections.Generic

open FSharp.Control.TaskBuilder
open Swensen.Unquote
open Oryx
open Xunit

open CogniteSdk
open Common

[<Fact>]
let ``Create delete labels is OK`` () = task {
    // Arrange
    let externalIdString = Guid.NewGuid().ToString();
    let create = LabelCreate(ExternalId = externalIdString, Name = "Test label", Description = "description")

    // Act
    let! res = writeClient.Labels.CreateAsync [ create ]
    let! delRes = writeClient.Labels.DeleteAsync [ externalIdString ]

    let resExternalId = (Seq.head res).ExternalId
    
    // Assert
    test <@ resExternalId = externalIdString @>
}

[<Fact>]
let ``Create, filter, delete labels is OK`` () = task {
    // Arrange
    let externalIdString = Guid.NewGuid().ToString();
    let create = LabelCreate(ExternalId = externalIdString, Name = "Test label", Description = "description")

    let filter = LabelListFilter(Name = "Test label")
    let query = LabelQuery(Filter = filter)

    // Act
    let! res = writeClient.Labels.CreateAsync [ create ]
    let! listRes = writeClient.Labels.ListAsync query
    let! delRes = writeClient.Labels.DeleteAsync [ externalIdString ]

    let resExternalId = (Seq.head res).ExternalId

    let f = fun (lb : Label) -> lb.ExternalId = externalIdString

    // Assert
    test <@ resExternalId = externalIdString @>
    test <@ Seq.exists (fun (lb : Label) -> lb.ExternalId = externalIdString) listRes.Items @>
}
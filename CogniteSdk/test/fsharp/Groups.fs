module Tests.Integration.Groups

open FSharp.Control.Tasks
open Swensen.Unquote
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

[<Fact>]
let ``Create delete group is OK`` () = task {
    // Arrange
    let capabilities : BaseAcl list = [
        GroupsAcl(CurrentUserScope=BaseScope(), Actions=["LIST"]);
        RelationshipsAcl(All=BaseScope(), Actions=["READ"]);
        RawAcl(
            TableScope=RawTableScope(
                DbsToTables=dict["sdk-test-database", RawTableScopeWrapper(Tables=["sdk-test-table"])]
            ),
            Actions=["LIST"]
        )
    ]
    let group = GroupCreate(
        Name = "sdk-test-group",
        Capabilities=capabilities
    )

    // Act
    let! res = writeClient.Groups.CreateAsync [group]
    let! deleteRes = writeClient.Groups.DeleteAsync [(Seq.item 0 res).Id]

    // Assert
    test <@ Seq.length res = 1 @>
}
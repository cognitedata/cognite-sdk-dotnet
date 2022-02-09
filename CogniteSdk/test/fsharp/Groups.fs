module Tests.Integration.Groups

open System.Text.Json

open FSharp.Control.TaskBuilder
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
    let group =
        GroupCreate(
            Name = "sdk-test-group",
            Capabilities=capabilities
        )

    // Act
    let! res = writeClient.Groups.CreateAsync [group]
    let! deleteRes = writeClient.Groups.DeleteAsync [(Seq.item 0 res).Id]

    // Assert
    test <@ Seq.length res = 1 @>
}

[<Fact>]
let ``Token inspect for capabilities is OK`` () = task {
    // Act
    let! res = writeClient.Token.InspectAsync ()

    // Assert
    test <@ Seq.length res.Capabilities > 0 @>
}

[<Fact>]
let ``Deserialize, serialize idscope ACL is OK`` () =
    // Arrange
    let json = @"{""timeSeriesAcl"":{""actions"":[""READ"",""WRITE""],""scope"":{""idscope"":{""ids"":[""1234"",""4321""]}}}}"
    // Same, but ids are no longer strings. Groups create handles this fine
    let excepectedResult = @"{""timeSeriesAcl"":{""actions"":[""READ"",""WRITE""],""scope"":{""idscope"":{""ids"":[1234,4321]}}}}"

    // Act
    let res = JsonSerializer.Deserialize<BaseAcl>(json, Oryx.Cognite.Common.jsonOptions)

    let acl = 
        match res with
        | :? TimeSeriesAcl as acl -> acl
        | _ -> failwith "Result is not correctly deserialized"

    let baseAcl: BaseAcl = acl
    let reverse = JsonSerializer.Serialize(baseAcl, Oryx.Cognite.Common.jsonOptions)

    // Assert
    test <@ reverse = excepectedResult @>
    test <@ Seq.length acl.IdScope.Ids = 2 @>
    test <@ Seq.item 0 acl.IdScope.Ids = 1234 @>
    test <@ Seq.length acl.Actions = 2 @>
    test <@ Seq.item 0 acl.Actions = "READ" @>

[<Fact>]
let ``Deserialize unknown ACL is OK`` () =
    // Arrange
    let json = @"{""unknownAcl"":{""actions"":[""READ"",""WRITE""],""scope"":{""some-scope"":{""ids"":[""1234"",""4321""]}}}}"

    // Act
    let res = JsonSerializer.Deserialize<BaseAcl>(json, Oryx.Cognite.Common.jsonOptions)

    // Assert
    test <@ res.CapabilityName = "unknownAcl" @>
    test <@ Seq.length res.Actions = 2 @>
    test <@ Seq.item 0 res.Actions = "READ" @>
module Tests.Integration.Raw

open System
open System.Net.Http
open System.Threading.Tasks

open Xunit
open Swensen.Unquote

open Tests
open Common
open Oryx
open Oryx.Retry
open FSharp.Control.Tasks.V2.ContextInsensitive

open CogniteSdk
open CogniteSdk.Raw

[<Trait("resource", "raw")>]
[<Fact>]
let ``List Databases with limit is Ok`` () = task {
    // Arrange
    let ctx = writeCtx ()
    let query = [ DatabaseQuery.Limit 10 ]

    // Act
    let! res = Items.listDatabasesAsync query ctx

    let ctx' =
        match res with
        | Ok ctx -> ctx
        | Error err -> raise <| err.ToException ()

    let dtos = ctx'.Response
    let len =Seq.length dtos

    // Assert
    test <@ len > 0 @>
    test <@ ctx'.Request.Method = HttpMethod.Get @>
    test <@ ctx'.Request.Extra.["resource"] = "/raw/dbs" @>
}

[<Trait("resource", "raw")>]
[<Fact>]
let ``List Tables with limit is Ok`` () = task {
    // Arrange
    let ctx = writeCtx ()
    let query = [ DatabaseQuery.Limit 10 ]

    // Act
    let! res = Items.listTablesAsync "sdk-test-database" query ctx

    let ctx' =
        match res with
        | Ok ctx -> ctx
        | Error err -> raise <| err.ToException ()

    let dtos = ctx'.Response
    let len = Seq.length dtos

    // Assert
    test <@ len > 0 @>
    test <@ ctx'.Request.Method = HttpMethod.Get @>
    test <@ ctx'.Request.Extra.["resource"] = "/raw/dbs/sdk-test-database/tables" @>
}

[<Trait("resource", "raw")>]
[<Fact>]
let ``List Rows with limit is Ok`` () = task {
    // Arrange
    let ctx = writeCtx ()
    let query = [ DatabaseRowQuery.Limit 10 ]
    let expectedCols = """{
  "sdk-test-col": "sdk-test-value",
  "sdk-test-col2": "sdk-test-value2"
}"""

    // Act
    let! res = Items.listRowsAsync "sdk-test-database" "sdk-test-table" query ctx

    let ctx' =
        match res with
        | Ok ctx -> ctx
        | Error err -> raise <| err.ToException ()

    let dtos = ctx'.Response
    let len = Seq.length dtos

    // Assert
    test <@ len > 0 @>
    test <@ dtos |> Seq.exists (fun dto -> dto.Key = "sdk-test-row") @>
    test <@ dtos |> Seq.exists (fun dto -> dto.Columns.ToString() = expectedCols) @>
    test <@ ctx'.Request.Method = HttpMethod.Get @>
    test <@ ctx'.Request.Extra.["resource"] = "/raw/dbs/sdk-test-database/tables/sdk-test-table/rows" @>
}

[<Trait("resource", "raw")>]
[<Fact>]
let ``List Rows with limit and choose columns isOk`` () = task {
    // Arrange
    let ctx = writeCtx ()
    let query = [ DatabaseRowQuery.Limit 10; DatabaseRowQuery.Columns ["sdk-test-col2"] ]
    let expectedCols = """{
  "sdk-test-col2": "sdk-test-value2"
}"""

    // Act
    let! res = Items.listRowsAsync "sdk-test-database" "sdk-test-table" query ctx

    let ctx' =
        match res with
        | Ok ctx -> ctx
        | Error err -> raise <| err.ToException ()

    let dtos = ctx'.Response
    let len = Seq.length dtos
    // Assert
    test <@ len > 0 @>
    test <@ dtos |> Seq.exists (fun dto -> dto.Key = "sdk-test-row") @>
    test <@ dtos |> Seq.exists (fun dto -> dto.Columns.ToString() = expectedCols) @>
    test <@ ctx'.Request.Method = HttpMethod.Get @>
    test <@ ctx'.Request.Extra.["resource"] = "/raw/dbs/sdk-test-database/tables/sdk-test-table/rows" @>
}

[<Trait("resource", "raw")>]
[<Fact>]
let ``Create and delete database is Ok`` () = task {
    // Arrange
    let ctx = writeCtx ()
    let name = Guid.NewGuid().ToString().[..31]

    // Act
    let! createRes = Create.createDatabasesAsync [name] ctx
    let! res = Items.listDatabasesAsync [] ctx
    let! deleteRes = Delete.deleteDatabasesAsync [name] false ctx

    let createCtx =
        match createRes with
        | Ok ctx -> ctx
        | Error err -> raise <| err.ToException ()

    let ctx' =
        match res with
        | Ok ctx -> ctx
        | Error err -> raise <| err.ToException ()

    let deleteCtx =
        match deleteRes with
        | Ok ctx -> ctx
        | Error err -> raise <| err.ToException ()

    let dtos = ctx'.Response
    let len = Seq.length dtos

    // Assert
    test <@ createCtx.Request.Method = HttpMethod.Post @>
    test <@ createCtx.Request.Extra.["resource"] = "/raw/dbs" @>

    test <@ len > 0 @>
    test <@ dtos |> Seq.exists (fun dto -> dto.Name = name) @>
    test <@ ctx'.Request.Method = HttpMethod.Get @>
    test <@ ctx'.Request.Extra.["resource"] = "/raw/dbs" @>

    test <@ deleteCtx.Request.Method = HttpMethod.Post @>
    test <@ deleteCtx.Request.Extra.["resource"] = "/raw/dbs/delete" @>
}

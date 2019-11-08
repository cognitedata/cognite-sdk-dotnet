module Tests.Integration.Raw

open System
open System.Net.Http
open FSharp.Control.Tasks.V2.ContextInsensitive

open Xunit
open Swensen.Unquote

open Oryx
open Thoth.Json.Net

open CogniteSdk
open CogniteSdk.Raw
open Common

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

    let dtos = ctx'.Response.Items
    let len = Seq.length dtos

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

    let dtos = ctx'.Response.Items
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

    let dtos = ctx'.Response.Items
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

    let dtos = ctx'.Response.Items
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

    let dtos = ctx'.Response.Items
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

[<Trait("resource", "raw")>]
[<Fact>]
let ``Create and delete table in database is Ok`` () = task {
    // Arrange
    let ctx = writeCtx ()
    let dbName = Guid.NewGuid().ToString().[..31]
    let tableName = Guid.NewGuid().ToString().[..31]

    // Act
    let! createRes = Create.createTablesAsync dbName [tableName] true ctx
    let! res = Items.listTablesAsync dbName [] ctx
    let! deleteRes = Delete.deleteDatabasesAsync [dbName] true ctx

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

    let dtos = ctx'.Response.Items
    let len = Seq.length dtos

    // Assert
    test <@ createCtx.Request.Method = HttpMethod.Post @>
    test <@ createCtx.Request.Extra.["resource"] = "/raw/dbs/" + dbName + "/tables" @>

    test <@ len > 0 @>
    test <@ dtos |> Seq.exists (fun dto -> dto.Name = tableName) @>
    test <@ ctx'.Request.Method = HttpMethod.Get @>
    test <@ ctx'.Request.Extra.["resource"] = "/raw/dbs/" + dbName + "/tables" @>

    test <@ deleteCtx.Request.Method = HttpMethod.Post @>
    test <@ deleteCtx.Request.Extra.["resource"] = "/raw/dbs/delete" @>
}

[<Trait("resource", "raw")>]
[<Fact>]
let ``Create and delete rows from table in database is Ok`` () = task {
    // Arrange
    let ctx = writeCtx ()
    let dbName = Guid.NewGuid().ToString().[..31]
    let tableName = Guid.NewGuid().ToString().[..31]
    let rowKey = Guid.NewGuid().ToString().[..31]
    let column = Encode.object [ "test column", Encode.int 42 ]
    let rowDto = { Key = rowKey; Columns = column}

    // Act
    let! createTableRes = Create.createTablesAsync dbName [tableName] true ctx
    let! createRes = Create.createRowsAsync dbName tableName [rowDto] ctx
    let! res = Items.listRowsAsync dbName tableName [] ctx
    let! deleteRes = Delete.deleteDatabasesAsync [dbName] true ctx

    let createTableCtx =
        match createTableRes with
        | Ok ctx -> ctx
        | Error err -> raise <| err.ToException ()

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

    let dtos = ctx'.Response.Items
    let len = Seq.length dtos

    // Assert
    test <@ createTableCtx.Request.Method = HttpMethod.Post @>
    test <@ createTableCtx.Request.Extra.["resource"] = "/raw/dbs/" + dbName + "/tables" @>

    test <@ createCtx.Request.Method = HttpMethod.Post @>
    test <@ createCtx.Request.Extra.["resource"] = "/raw/dbs/" + dbName + "/tables/" + tableName + "/rows" @>

    test <@ len > 0 @>
    test <@ dtos |> Seq.exists (fun dto -> dto.Key = rowKey && dto.Columns.ToString() = column.ToString()) @>
    test <@ ctx'.Request.Method = HttpMethod.Get @>
    test <@ ctx'.Request.Extra.["resource"] = "/raw/dbs/" + dbName + "/tables/" + tableName + "/rows" @>

    test <@ deleteCtx.Request.Method = HttpMethod.Post @>
    test <@ deleteCtx.Request.Extra.["resource"] = "/raw/dbs/delete" @>
}

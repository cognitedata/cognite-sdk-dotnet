module Tests.Integration.Raw

open System
open System.Collections.Generic
open FSharp.Control.Tasks.V2.ContextInsensitive
open System.Text.Json

open Xunit
open Swensen.Unquote

open CogniteSdk

open Common

[<Trait("resource", "raw")>]
[<Fact>]
let ``List Databases with limit is Ok`` () = task {
    // Arrange
    let query = RawDatabaseQuery(Limit = Nullable 10)

    // Act
    let! res = writeClient.Raw.ListDatabasesAsync query

    // Assert
    test <@ Seq.length res.Items > 0 @>
}

[<Trait("resource", "raw")>]
[<Fact>]
let ``List Tables with limit is Ok`` () = task {
    // Arrange
    let query = RawDatabaseQuery(Limit = Nullable 10)

    // Act
    let! res = writeClient.Raw.ListTablesAsync("sdk-test-database", query)

    // Assert
    test <@ Seq.length res.Items > 0 @>
}

[<Trait("resource", "raw")>]
[<Fact>]
let ``List Rows with limit is Ok`` () = task {
    // Arrange
    let expectedCols = seq {
      "sdk-test-col", "sdk-test-value"
      "sdk-test-col2", "sdk-test-value2"
    }
    let query = RawRowQuery(Limit = Nullable 10)

    // Act
    let! res = writeClient.Raw.ListRowsAsync("sdk-test-database", "sdk-test-table", query)

    // Assert
    test <@ Seq.length res.Items > 0 @>
    test <@ res.Items |> Seq.exists (fun dto -> dto.Key = "sdk-test-row") @>
    test <@ res.Items |> Seq.exists (fun dto ->
        (dto.Columns.Item "sdk-test-col").ToString() = "sdk-test-value" &&
        (dto.Columns.Item "sdk-test-col2").ToString() = "sdk-test-value2") @>
}

[<Trait("resource", "raw")>]
[<Fact>]
let ``List Rows with limit and choose columns isOk`` () = task {
    // Arrange
    let query = RawRowQuery(Limit = Nullable 10, Columns = ["sdk-test-col2"])
    let expectedCols = """{
  "sdk-test-col2": "sdk-test-value2"
}"""

    // Act
    let! res = writeClient.Raw.ListRowsAsync("sdk-test-database", "sdk-test-table", query)

    // Assert
    test <@ Seq.length res.Items > 0 @>
    test <@ res.Items |> Seq.exists (fun dto -> dto.Key = "sdk-test-row") @>
    test <@ res.Items |> Seq.exists (fun dto -> (dto.Columns.Item "sdk-test-col2").ToString() = "sdk-test-value2") @>
}

[<Trait("resource", "raw")>]
[<Fact>]
let ``Create and delete database is Ok`` () = task {
    // Arrange
    let name = Guid.NewGuid().ToString().[..31]

    // Act
    let! createRes = writeClient.Raw.CreateDatabasesAsync [name]
    let! res = writeClient.Raw.ListDatabasesAsync()
    let! deleteRes = writeClient.Raw.DeleteDatabasesAsync([name])

    // Assert
    test <@ Seq.length res.Items > 0 @>
    test <@ res.Items |> Seq.exists (fun dto -> dto.Name = name) @>
}

[<Trait("resource", "raw")>]
[<Fact>]
let ``Create and delete table in database is Ok`` () = task {
    // Arrange
    let dbName = Guid.NewGuid().ToString().[..31]
    let tableName = Guid.NewGuid().ToString().[..31]

    // Act
    let! createRes = writeClient.Raw.CreateTablesAsync(dbName, [tableName], true)
    let! res = writeClient.Raw.ListTablesAsync(dbName);
    let! deleteRes = writeClient.Raw.DeleteDatabasesAsync([dbName], true)

    // Assert
    test <@ Seq.length res.Items > 0 @>
    test <@ res.Items |> Seq.exists (fun dto -> dto.Name = tableName) @>
}

[<Trait("resource", "raw")>]
[<Fact>]
let ``Create and delete rows from table in database is Ok`` () = task {
    // Arrange
    let dbName = Guid.NewGuid().ToString().[..31]
    let tableName = Guid.NewGuid().ToString().[..31]
    let rowKey = Guid.NewGuid().ToString().[..31]
    let doc = JsonDocument.Parse("42")
    let column = Dictionary(dict [ ("test column", doc.RootElement) ])
    let rowDto = RawRowCreate(Key = rowKey, Columns = column)

    // Act
    let! table = writeClient.Raw.CreateTablesAsync(dbName, [ tableName ], true)
    let! createRes = writeClient.Raw.CreateRowsAsync(dbName, tableName, [ rowDto ], true)
    let! res = writeClient.Raw.ListRowsAsync(dbName, tableName)
    let! deleteRowRes = writeClient.Raw.DeleteRowsAsync(dbName, tableName, [
        for row in res.Items do
            RawRowDelete(Key=row.Key)])
    let! deleteRes = writeClient.Raw.DeleteDatabasesAsync([ dbName ], true)

    // Assert
    test <@ Seq.length res.Items > 0 @>
    let column =
        res.Items
        |> Seq.filter (fun dto -> dto.Key = rowKey)
        |> Seq.map (fun row -> row.Columns)
        |> Seq.head
        |> Seq.head
    let key = column.Key
    let value = column.Value.ToString()
    test <@ key = "test column" && value = "42" @>
}
module CogniteSdk.Types.Test

open System
open System.Net.Http
open System.Text.Json

open Swensen.Unquote
open Xunit

open CogniteSdk
open System.Collections.Generic

type Base() =
    member val Items: int seq = null with get, set

    member val Url: Uri = null with get, set

type A () =
    inherit Base()
    member val ExternalId: string = "" with get, set

type B () =
    inherit Base()
    member val Id: int64 = 0L with get, set

[<Fact>]
let ``Test Inheritance`` () =
    // Arrange
    let b : Base = B(Id=42L, Items=[1; 2; 3]) :> _

    // Act
    let value = JsonSerializer.Serialize<Base>(b)
    let b = JsonSerializer.Deserialize<B>(value)

    // Assert
    test <@ b.Id = 0L @>

[<Fact>]
let ``Test IDictionary`` () =
    // Arrange
    let b = dict [("a", 42)] |> Dictionary

    // Act
    let value = JsonSerializer.Serialize<Dictionary<string, int>>(b)
    //let value = JsonSerializer.Serialize(b, typeof<B>)
    let b = JsonSerializer.Deserialize<Dictionary<string, int>>(value)

    // Assert
    test <@ b.["a"] = 42 @>


[<Fact>]
let ``Test Uri`` () =
    // Arrange
    let b = Base(Url=Uri("http://test.com/"))

    // Act
    let value = JsonSerializer.Serialize<Base>(b)
    //let value = JsonSerializer.Serialize(b, typeof<B>)
    let b = JsonSerializer.Deserialize<Base>(value)

    // Assert
    test <@ b.Url.ToString() = "http://test.com/" @>


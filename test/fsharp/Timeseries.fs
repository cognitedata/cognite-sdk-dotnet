module Tests.Timeseries

open System
open System.IO

open Xunit
open Swensen.Unquote

open Cognite.Sdk
open Cognite.Sdk.Timeseries
open Cognite.Sdk.Request


[<Fact>]
let ``Create timeseries is Ok`` () = async {
    // Arrenge
    let json = File.ReadAllText "Assets.json"
    let fetch = Fetch.fromJson json

    let ctx =
        defaultContext
        |> addHeader ("api-key", "test-key")

    // Act
    let! res = Internal.createTimeseries [] fetch ctx

    // Assert
    test <@ Result.isOk res.Result @>
    test <@ res.Request.Method = HttpMethod.POST @>
    test <@ res.Request.Resource = "/timeseries" @>
    test <@ res.Request.Query.IsEmpty @>
}

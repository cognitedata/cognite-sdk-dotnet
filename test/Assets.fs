module Tests.Test

open Expecto
open Cognite.Sdk
open Cognite.Sdk.Assets
open Cognite.Sdk.Context


let fetcher response (ctx: Context) =
    async {
        return response
    }

let item = """
{
    "id": 5406108165931348,
    "path": [ 5406108165931348 ],
    "depth": 0,
    "name": "string",
    "description": "string",
    "metadata": {},
    "source": "string",
    "sourceId": "string",
    "createdTime": 1549374256347,
    "lastUpdatedTime": 1549374256347
}
"""

[<Tests>]
let assetTests =
    testAsync "A simple test" {
        let response = sprintf """{ "data": { "items": [%s]}}""" item
        let fetch = fetcher response
        //printfn "%A" response

        let ctx =
            defaultContext
            |> addHeader(("api-key", "test"))

        let! result = getAssets ctx [ Name "string"]

        Expect.isOk result "Should be OK"
    }
module Tests.Integration.Common

open System
open System.Net.Http

open Fusion

let testApiKeyWrite = Environment.GetEnvironmentVariable "TEST_API_KEY_WRITE"
let testApiKeyRead = Environment.GetEnvironmentVariable "TEST_API_KEY_READ"

let createCtx (key: string) (project: string) (serviceUrl: string) =
    let client = new HttpClient ()
    defaultContext
    |> setHttpClient client
    |> addHeader ("api-key", key)
    |> setProject project
    |> setServiceUrl serviceUrl

let readCtx () = createCtx testApiKeyRead "publicdata" "https://api.cognitedata.com"
let writeCtx () = createCtx testApiKeyWrite "fusiondotnet-tests" "https://greenfield.cognitedata.com"

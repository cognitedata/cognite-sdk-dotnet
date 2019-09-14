module Tests.Integration.Common

open System
open System.Net.Http

open Oryx
open CogniteSdk

let testApiKeyWrite = Environment.GetEnvironmentVariable "TEST_API_KEY_WRITE"
let testApiKeyRead = Environment.GetEnvironmentVariable "TEST_API_KEY_READ"

let createCtx (key: string) (project: string) (serviceUrl: string) =
    let client = new HttpClient ()
    Context.create ()
    |> Context.setAppId "test"
    |> Context.setHttpClient client
    |> Context.addHeader ("api-key", key)
    |> Context.setProject project
    |> Context.setServiceUrl serviceUrl

let readCtx () = createCtx testApiKeyRead "publicdata" "https://api.cognitedata.com"
let writeCtx () = createCtx testApiKeyWrite "fusiondotnet-tests" "https://greenfield.cognitedata.com"

let optionToSeq (o: 'a option): 'a seq =
    match o with
    | Some a -> Seq.ofList [ a ]
    | None -> Seq.empty

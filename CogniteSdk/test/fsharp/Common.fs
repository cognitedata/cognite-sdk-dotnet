namespace Tests.Integration

open System
open System.Net.Http

open CogniteSdk

module Common =
    let createClient apiKey project url =
        let httpClient = new HttpClient();
        Client.Builder.Create(httpClient)
            .SetAppId("TestApp")
            .AddHeader("api-key", apiKey)
            .SetProject(project)
            .SetServiceUrl(url)
            .Build();

    let readClient =
        createClient
            (Environment.GetEnvironmentVariable "TEST_API_KEY_READ")
            "publicdata"
            "https://api.cognitedata.com"

    let writeClient =
        createClient
            (Environment.GetEnvironmentVariable "TEST_API_KEY_WRITE")
            "fusiondotnet-tests"
            "https://greenfield.cognitedata.com"

    let optionToSeq (o: 'a option): 'a seq =
        match o with
        | Some a -> Seq.ofList [ a ]
        | None -> Seq.empty

[<RequireQualifiedAccess>]
module Result =
    let isOk = function
        | Ok _ -> true
        | Error _ -> false

    let isError res = not (isOk res)

    let getError = function
        | Error e -> e
        | Ok a -> failwith "Result is not error"

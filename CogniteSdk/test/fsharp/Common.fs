namespace Tests.Integration

open System
open System.Net.Http

open CogniteSdk

module Common =
    let createClient apiKey project url includeMetadata =
        let handler = new HttpClientHandler(ServerCertificateCustomValidationCallback = (fun message cert chain errors -> true))
        let httpClient = new HttpClient(handler);

        Client.Builder.Create(httpClient)
            .SetAppId("TestApp")
            .AddHeader("api-key", apiKey)
            .SetProject(project)
            .SetBaseUrl(Uri(url))
            .IncludeMetadata(includeMetadata)
            .Build();


    let readClient =
        createClient
            (Environment.GetEnvironmentVariable "TEST_API_KEY_READ")
            "publicdata"
            "https://api.cognitedata.com"
            true

    let readClientWithoutMetadata =
        createClient
            (Environment.GetEnvironmentVariable "TEST_API_KEY_READ")
            "publicdata"
            "https://api.cognitedata.com"
            false

    let writeClient =
        createClient
            (Environment.GetEnvironmentVariable "TEST_API_KEY_WRITE")
            "fusiondotnet-tests"
            "https://greenfield.cognitedata.com"
            true

    let noAuthClient =
        let handler = new HttpClientHandler(ServerCertificateCustomValidationCallback = (fun message cert chain errors -> true))
        let httpClient = new HttpClient(handler);

        Client.Builder.Create(httpClient)
            .SetAppId("TestApp")
            .SetProject("publicdata")
            .SetBaseUrl(Uri("https://api.cognitedata.com"))
            .Build();

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

namespace Tests.Integration

open System
open System.Net.Http

open CogniteSdk

module Common =
    let createOAuth2SdkClient accessToken project url =
        let handler = new HttpClientHandler(ServerCertificateCustomValidationCallback = (fun message cert chain errors -> true))
        let httpClient = new HttpClient(handler);

        Client
            .Builder
            .Create(httpClient)
            .SetAppId("TestApp")
            .AddHeader("Authorization", $"Bearer {accessToken}")
            .SetProject(project)
            .SetBaseUrl(Uri(url))
            .Build()


    let readClient =
        let oAuth2AccessToken = Environment.GetEnvironmentVariable "TEST_TOKEN_READ"

        createOAuth2SdkClient
            oAuth2AccessToken
            "publicdata"
            "https://api.cognitedata.com"

    let writeClient =
        let oAuth2AccessToken = Environment.GetEnvironmentVariable "TEST_TOKEN_WRITE"

        createOAuth2SdkClient
            oAuth2AccessToken
            "fusiondotnet-tests"
            "https://greenfield.cognitedata.com"

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

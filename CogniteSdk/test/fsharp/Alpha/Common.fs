namespace Tests.Integration.Alpha

open System

module Common =
    open Tests.Integration.Common
    open Xunit

    let azureDevClient =
        let oAuth2AccessToken = Environment.GetEnvironmentVariable "TEST_TOKEN_WRITE"

        createOAuth2SdkClient oAuth2AccessToken "cognite-simulator-qualitycheck" "https://azure-dev.cognitedata.com"

    type FactIf(envVar: string, skipReason: string) =
        inherit FactAttribute()

        override this.Skip =
            let envFlag =
                Environment.GetEnvironmentVariable envVar
                |> Option.ofObj
                |> Option.map (fun x -> x = "true")
                |> Option.defaultValue false

            if envFlag then null else skipReason

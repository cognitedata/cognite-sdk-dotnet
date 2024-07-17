namespace Tests.Integration.Alpha

open System

module Common =
    open Tests.Integration.Common
    open Xunit
    open CogniteSdk.Alpha

    let testSimulatorCreate (simulatorExternalId: string) =
        SimulatorCreate(
            ExternalId = simulatorExternalId,
            Name = "test_sim",
            FileExtensionTypes = [ "json" ],
            StepFields = [
                SimulatorStepField(
                    StepType = "get/set",
                    Fields = [
                        SimulatorStepFieldParam(
                            Name = "address",
                            Label = "OpenServer Address",
                            Info = "Enter the address of the PROSPER variable, i.e. PROSPER.ANL. SYS. Pres"
                        )
                    ]
                )
                SimulatorStepField(
                    StepType = "command",
                    Fields = [
                        SimulatorStepFieldParam(
                            Name = "command",
                            Label = "OpenServer Command",
                            Info = "Enter the PROSPER command"
                        )
                    ]
                )
            ],
            ModelTypes = [
                SimulatorModelType(
                    Name = "Oil and Water Well",
                    Key = "OilWell"
                )
                SimulatorModelType(
                    Name = "Dry and Wet Gas Well",
                    Key = "GasWell"
                )
                SimulatorModelType(
                    Name = "Retrograde Condensate Well",
                    Key = "RetrogradeWell"
                )
            ],
            UnitQuantities = [
                SimulatorUnitQuantity(
                    Name = "test_quantity",
                    Label = "test_quantity",
                    Units = [
                        SimulatorUnitEntry(
                            Label = "test_unit",
                            Name = "test_unit"
                        )
                    ]
                )
            ]
        )

    let bluefieldClient =
        let oAuth2AccessToken = Environment.GetEnvironmentVariable "TEST_TOKEN_WRITE"

        createOAuth2SdkClient oAuth2AccessToken "cognite-simulator-qualitycheck" "https://bluefield.cognitedata.com"

    type FactIf(envVar: string, skipReason: string) =
        inherit FactAttribute()

        override this.Skip =
            let envFlag =
                Environment.GetEnvironmentVariable envVar
                |> Option.ofObj
                |> Option.map (fun x -> x = "true")
                |> Option.defaultValue false

            if envFlag then null else skipReason

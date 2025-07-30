module Tests.Integration.SimulatorModelDependencies

open System
open System.Collections.Generic

open FSharp.Control.TaskBuilder
open Xunit
open Swensen.Unquote

open CogniteSdk
open CogniteSdk.Alpha
open Tests.Integration.Alpha.Common
open Common


let now = DateTimeOffset.Now.ToUnixTimeMilliseconds()
let simulatorExternalId = $"test_sim_{now}"
let modelExternalId = $"test_model_deps_{now}"

[<Fact>]
[<Trait("resource", "simulatorModelDependencies")>]
[<Trait("api", "simulators")>]
let ``Create simulator with model dependencies support and model revisions with ext. deps`` () =
    task {

        let fileExtensionTypes = seq { "txt"; "json"; "out" }

        let simulatorToCreate =
            testSimulatorCreate(simulatorExternalId)
            |> fun simToCreate ->
                simToCreate.ModelDependencies <- [
                    SimulatorModelDependency(
                        FileExtensionTypes = fileExtensionTypes,
                        Fields = [
                            SimulatorModelDependencyFields(
                                Name = "test_field",
                                Label = "Test Field",
                                Info = "Test field for external deps"
                            )
                        ]
                    )
                ]
                simToCreate
        let! dataSetRes = writeClient.DataSets.RetrieveAsync([ new Identity("test-dataset") ])
        let dataSet = dataSetRes |> Seq.head
        let! testFileResRevision = writeClient.Files.RetrieveAsync([ new Identity("empty.json") ])
        let testFileIdRevision = testFileResRevision |> Seq.head |> (fun f -> f.Id)

        let! testFileResDependency = writeClient.Files.RetrieveAsync([ new Identity("empty2.json") ])
        let testFileIdDependency = testFileResDependency |> Seq.head |> (fun f -> f.Id)

        let modelToCreate =
            SimulatorModelCreate(
                ExternalId = modelExternalId,
                SimulatorExternalId = simulatorExternalId,
                Name = "test_model",
                Description = "test_model_description",
                DataSetId = dataSet.Id,
                Type = "OilWell"
            )

        // Create model revision with external dependencies
        let modelRevisionToCreate =
            SimulatorModelRevisionCreate(
                ExternalId = "test_model_revision_v1",
                ModelExternalId = modelExternalId,
                Description = "test_model_revision_description",
                FileId = testFileIdRevision,
                ExternalDependencies = [
                    SimulatorFileDependency(
                        File = SimulatorFileDependencyReference(Id = testFileIdDependency),
                        Arguments = Dictionary(dict [ "test_field", "test_value" ])
                    )
                ]
            )

        try
            let! simulatorRes = writeClient.Alpha.Simulators.CreateAsync([ simulatorToCreate ])

            let len = Seq.length simulatorRes
            test <@ len = 1 @>
            let itemRes = simulatorRes |> Seq.head
            test <@ itemRes.Name = simulatorToCreate.Name @>

            let lenModelDeps = Seq.length itemRes.ModelDependencies
            test <@ lenModelDeps = 1 @>
            let modelDep = itemRes.ModelDependencies |> Seq.head
            test <@ Seq.toArray modelDep.FileExtensionTypes = Seq.toArray fileExtensionTypes @>

            let! _ = writeClient.Alpha.Simulators.CreateSimulatorModelsAsync([ modelToCreate ])
            let! modelRevisionRes = writeClient.Alpha.Simulators.CreateSimulatorModelRevisionsAsync([ modelRevisionToCreate ])

            let modelRevisionCreated = modelRevisionRes |> Seq.head

            test <@ modelRevisionCreated.ExternalId = modelRevisionToCreate.ExternalId @>
            test <@ modelRevisionCreated.FileId = modelRevisionToCreate.FileId @>
            
            let externalDep = modelRevisionCreated.ExternalDependencies |> Seq.head
            test <@ externalDep.File.Id = testFileIdDependency @>
            test <@ externalDep.Arguments.["test_field"] = "test_value" @>

        finally
            writeClient.Alpha.Simulators.DeleteAsync([ new Identity(simulatorExternalId) ])
            |> ignore
        ()
    }

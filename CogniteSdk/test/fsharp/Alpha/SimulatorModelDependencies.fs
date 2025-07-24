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
        // test-dataset
        let! dataSetRes = writeClient.DataSets.RetrieveAsync([ new Identity("prosper-simulator-test") ])
        let dataSet = dataSetRes |> Seq.head
        // empty.json for publicdata
        // 3237317904039163 -> tests-dotnet.json in bluefield cognite-simulator-quality-check
        let! testFileResRevision = writeClient.Files.RetrieveAsync([ 3237317904039163L ])
        let testFileIdRevision = testFileResRevision |> Seq.head |> (fun f -> f.Id)

        let! testFileResDependency = writeClient.Files.RetrieveAsync([ 1104203827922893L ])
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
                ExternalId = "test_model_revision",
                ModelExternalId = modelExternalId,
                Description = "test_model_revision_description",
                FileId = testFileIdRevision,
                ExternalDependencies = [
                    SimulatorFileDependency(
                        File = SimulatorFileDependencyFileField(Id = testFileIdDependency),
                        Arguments = Dictionary(dict [ "test_field", "test_value" ])
                    )
                ]
            )

        try
            // Act
            let! simulatorRes = writeClient.Alpha.Simulators.CreateAsync([ simulatorToCreate ])

            // Assert
            let len = Seq.length simulatorRes
            test <@ len = 1 @>
            let itemRes = simulatorRes |> Seq.head

            test <@ itemRes.Name = simulatorToCreate.Name @>
            test <@ itemRes.CreatedTime >= now @>
            test <@ itemRes.LastUpdatedTime >= now @>

            let lenModelDeps = Seq.length itemRes.ModelDependencies
            test <@ lenModelDeps = 1 @>
            let modelDep = itemRes.ModelDependencies |> Seq.head
            test <@ Seq.toArray modelDep.FileExtensionTypes = Seq.toArray fileExtensionTypes @>

            let! _ = writeClient.Alpha.Simulators.CreateSimulatorModelsAsync([ modelToCreate ])
            let! modelRevisionRes = writeClient.Alpha.Simulators.CreateSimulatorModelRevisionsAsync([ modelRevisionToCreate ])

            let modelRevisionCreated = modelRevisionRes |> Seq.head

            // Assert
            test <@ modelRevisionCreated.ExternalId = modelRevisionToCreate.ExternalId @>
            test <@ modelRevisionCreated.ModelExternalId = modelRevisionToCreate.ModelExternalId @>
            test <@ modelRevisionCreated.Description = modelRevisionToCreate.Description @>
            test <@ modelRevisionCreated.FileId = modelRevisionToCreate.FileId @>
            
            let externalDep = modelRevisionCreated.ExternalDependencies |> Seq.head
            test <@ externalDep.File.Id = testFileIdDependency @>
            test <@ externalDep.Arguments.["test_field"] = "test_value" @>

            // Test list endpoint returns SimulatorModelRevision
            let! modelRevisionsListRes =
                writeClient.Alpha.Simulators.ListSimulatorModelRevisionsAsync(
                    new SimulatorModelRevisionQuery(
                        Filter = SimulatorModelRevisionFilter(ModelExternalIds = [ modelExternalId ])
                    )
                )

            let modelRevisionFound = modelRevisionsListRes.Items |> Seq.head
            test <@ modelRevisionFound.ExternalId = modelRevisionToCreate.ExternalId @>
            test <@ modelRevisionFound.ModelExternalId = modelRevisionToCreate.ModelExternalId @>

        finally
            writeClient.Alpha.Simulators.DeleteAsync([ new Identity(simulatorExternalId) ])
            |> ignore
        ()
    }
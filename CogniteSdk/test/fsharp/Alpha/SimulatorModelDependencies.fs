[<Fact>]
[<Trait("resource", "simulatorModels")>]
[<Trait("api", "simulators")>]
let ``Create and list simulator model revisions with external dependencies is Ok`` () =
    task {
        // Arrange
        let now = DateTimeOffset.Now.ToUnixTimeMilliseconds()
        let simulatorExternalId = $"test_sim_deps_{now}"
        let modelExternalId = $"test_model_deps_{now}"

        let! dataSetRes = writeClient.DataSets.RetrieveAsync([ new Identity("test-dataset") ])
        let dataSet = dataSetRes |> Seq.head

        let! testFileRes = writeClient.Files.RetrieveAsync([ new Identity("empty.json") ])
        let testFileId = testFileRes |> Seq.head |> (fun f -> f.Id)

        // Create a simulator with model dependencies
        let simulatorToCreate =
            testSimulatorCreate(simulatorExternalId)
            |> fun s -> s.ModelDependencies <- [
                SimulatorModelDependency(
                    FileExtensionTypes = [ "txt"; "json" ],
                    Fields = [
                        SimulatorModelDependencyFields(
                            Name = "test_field",
                            Label = "Test Field",
                            Info = "Test field for external deps"
                        )
                    ]
                )
            ]

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
                FileId = testFileId,
                ExternalDependencies = [
                    SimulatorFileDependency(
                        File = SimulatorFileDependencyFileField(Id = testFileId),
                        Arguments = Dictionary(dict [ "test_field", "test_value" ])
                    )
                ]
            )

        try
            // Act
            let! _ = writeClient.Alpha.Simulators.CreateAsync([ simulatorToCreate ])
            let! _ = writeClient.Alpha.Simulators.CreateSimulatorModelsAsync([ modelToCreate ])
            let! modelRevisionRes = writeClient.Alpha.Simulators.CreateSimulatorModelRevisionsAsync([ modelRevisionToCreate ])

            let modelRevisionCreated = modelRevisionRes |> Seq.head

            // Assert
            test <@ modelRevisionCreated.ExternalId = modelRevisionToCreate.ExternalId @>
            test <@ modelRevisionCreated.ModelExternalId = modelRevisionToCreate.ModelExternalId @>
            test <@ modelRevisionCreated.Description = modelRevisionToCreate.Description @>
            test <@ modelRevisionCreated.FileId = modelRevisionToCreate.FileId @>
            
            let externalDep = modelRevisionCreated.ExternalDependencies |> Seq.head
            test <@ externalDep.File.Id = testFileId @>
            test <@ externalDep.Arguments.["test_field"] = "test_value" @>

            // Test list endpoint returns SimulatorModelRevisionDetail
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
    }

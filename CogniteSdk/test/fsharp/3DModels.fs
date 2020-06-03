module Tests.Integration.ThreeDModels

open System
open System.Collections.Generic

open FSharp.Control.Tasks.V2.ContextInsensitive
open Swensen.Unquote
open Oryx
open Xunit

open CogniteSdk
open Common

[<Fact>]
let ``List 3D models with limit is Ok`` () = task {
    // Arrange
    let query = ThreeDModelQuery(Limit= Nullable 1)

    // Act
    let! res = readClient.ThreeDModels.ListAsync(query)
    let len = Seq.length res.Items
    let model = res.Items |> Seq.head

    // Assert
    test <@ len = 1 @>
    test <@ model.Name = "Valhall PH" @>
}

[<Fact>]
let ``Get 3D model by id is Ok`` () = task {
    // Arrange
    let modelId = 4715379429968321L

    // Act
    let! res = readClient.ThreeDModels.RetrieveAsync modelId

    // Assert
    test <@ res.Name = "Valhall PH" @>
}

[<Fact>]
let ``Create and delete 3D models is Ok`` () = task {
    // Arrange
    let name = "sdk-test-model-" + Guid.NewGuid().ToString();
    let dto = ThreeDModelCreate(Name=name)

    // Act
    let! res = writeClient.ThreeDModels.CreateAsync [dto]
    let model = Seq.head res
    let! delRes = writeClient.ThreeDModels.DeleteAsync [model.Id]

    // Assert
    test <@ model.Name = name @>
}

[<Fact>]
let ``Update 3D model name is Ok`` () = task {
    // Arrange
    let name = "sdk-test-model-" + Guid.NewGuid().ToString();
    let newName = "sdk-test-model-" + Guid.NewGuid().ToString();

    let dto = ThreeDModelCreate(Name=name)

    // Act
    let! res = writeClient.ThreeDModels.CreateAsync [dto]
    let model = Seq.head res
    let updateDto = ThreeDModelUpdateItem(model.Id)
    updateDto.Update <- ThreeDModelUpdate(Name=Update(newName))

    let! updateRes = writeClient.ThreeDModels.UpdateAsync [updateDto]
    let updatedModel = Seq.head updateRes
    let! delRes = writeClient.ThreeDModels.DeleteAsync [model.Id]

    // Assert
    test <@ model.Name = name @>
    test <@ updatedModel.Name = newName @>
}
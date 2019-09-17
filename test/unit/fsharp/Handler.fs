module Tests.Handler

open System.IO

open Xunit
open Swensen.Unquote

open System
open System.Net
open System.Net.Http
open System.Threading
open System.Threading.Tasks

open FSharp.Control.Tasks.V2


open Oryx
open Oryx.Retry

open CogniteSdk

    type HttpMessageHandlerStub (sendAsync: Func<HttpRequestMessage, CancellationToken, Task<HttpResponseMessage>>) =
        inherit HttpMessageHandler ()
        let sendAsync = sendAsync

        override self.SendAsync(request: HttpRequestMessage, cancellationToken: CancellationToken) : Task<HttpResponseMessage> =
            task {
                return! sendAsync.Invoke(request, cancellationToken)
            }

[<Fact>]
let ``Get asset with fusion return expression is Ok``() = task {
    // Arrange
    let mutable retries = 0
    let json = File.ReadAllText "Asset.json"

    let stub =
        Func<HttpRequestMessage,CancellationToken,Task<HttpResponseMessage>>(fun request token ->
        (task {
            retries <- retries + 1
            let responseMessage = new HttpResponseMessage(HttpStatusCode.OK)
            responseMessage.Content <- new StringContent(json)
            return responseMessage
        }))

    let client = new HttpClient(new HttpMessageHandlerStub(stub))

    let ctx =
        Context.create ()
        |> Context.setAppId "test"
        |> Context.setHttpClient client
        |> Context.setProject "test"
        |> Context.addHeader ("api-key", "test-key")

    // Act
    let req = oryx {
        let! result = Assets.Entity.get 42L
        return result
    }

    let! result = runHandler req ctx
    let retries' = retries

    // Assert
    test <@ Result.isOk result @>
    test <@ retries' = 1 @>
}

[<Fact>]
let ``Get asset with fusion returnFrom expression is Ok``() = task {
    // Arrange
    let mutable retries = 0
    let json = File.ReadAllText "Asset.json"

    let stub =
        Func<HttpRequestMessage,CancellationToken,Task<HttpResponseMessage>>(fun request token ->
        (task {
            retries <- retries + 1
            let responseMessage = new HttpResponseMessage(HttpStatusCode.OK)
            responseMessage.Content <- new StringContent(json)
            return responseMessage
        }))

    let client = new HttpClient(new HttpMessageHandlerStub(stub))

    let ctx =
        Context.create ()
        |> Context.setAppId "test"
        |> Context.setHttpClient client
        |> Context.setProject "test"
        |> Context.addHeader ("api-key", "test-key")

    // Act
    let req = oryx {
        return! Assets.Entity.get 42L
    }

    let! result = runHandler req ctx
    let retries' = retries

    // Assert
    test <@ Result.isOk result @>
    test <@ retries' = 1 @>
}

[<Fact>]
let ``Get asset with retry is Ok``() = task {
    // Arrange
    let mutable retries = 0
    let json = File.ReadAllText "Asset.json"

    let stub =
        Func<HttpRequestMessage,CancellationToken,Task<HttpResponseMessage>>(fun request token ->
        (task {
            retries <- retries + 1
            let responseMessage = new HttpResponseMessage(HttpStatusCode.OK)
            responseMessage.Content <- new StringContent(json)
            return responseMessage
        }))

    let client = new HttpClient(new HttpMessageHandlerStub(stub))

    let ctx =
        Context.create ()
        |> Context.setAppId "test"
        |> Context.setHttpClient client
        |> Context.setProject "test"
        |> Context.addHeader ("api-key", "test-key")

    // Act
    let req = Assets.Entity.get 42L |> retry 0<ms> 5
    let! result = runHandler req ctx
    let retries' = retries

    // Assert
    test <@ Result.isOk result @>
    test <@ retries' = 1 @>
}

[<Fact>]
let ``Get asset with retries on server internal error``() = task {
    // Arrange
    let mutable retries = 0
    let json = File.ReadAllText "Asset.json"

    let stub =
        Func<HttpRequestMessage,CancellationToken,Task<HttpResponseMessage>>(fun request token ->
        (task {
            retries <- retries + 1
            let responseMessage = new HttpResponseMessage(HttpStatusCode.InternalServerError)
            responseMessage.Content <- new StringContent(json)
            return responseMessage
        }))

    let client = new HttpClient(new HttpMessageHandlerStub(stub))

    let ctx =
        Context.create ()
        |> Context.setAppId "test"
        |> Context.setHttpClient client
        |> Context.setProject "test"
        |> Context.addHeader ("api-key", "test-key")

    // Act
    let req = Assets.Entity.get 42L |> retry 0<ms> 5
    let! result = runHandler req ctx
    let retries' = retries

    // Assert
    test <@ Result.isError result @>
    test <@ retries' = 6 @>
}

[<Fact>]
let ``Get asset without http client throws exception``() = task {
    // Arrange
    let json = File.ReadAllText "Asset.json"
    let stub =
        Func<HttpRequestMessage,CancellationToken,Task<HttpResponseMessage>>(fun request token ->
        (task {
            let responseMessage = new HttpResponseMessage(HttpStatusCode.InternalServerError)
            responseMessage.Content <- new StringContent(json)
            return responseMessage
        }))

    let client = new HttpClient(new HttpMessageHandlerStub(stub))

    let ctx =
        Context.create ()
        |> Context.setAppId "test"
        |> Context.setProject "test"
        |> Context.addHeader ("api-key", "test-key")

    // Act
    let req = Assets.Entity.get 42L
    let! result = task {
        try
            let! result = runHandler req ctx
            return false
        with
        | _ -> return true
    }

    // Assert
    test <@ result @>
}

[<Fact>]
let ``Get asset without appId throws exception``() = task {
    // Arrange
    let json = File.ReadAllText "Asset.json"
    let stub =
        Func<HttpRequestMessage,CancellationToken,Task<HttpResponseMessage>>(fun request token ->
        (task {
            let responseMessage = new HttpResponseMessage(HttpStatusCode.InternalServerError)
            responseMessage.Content <- new StringContent(json)
            return responseMessage
        }))

    let client = new HttpClient(new HttpMessageHandlerStub(stub))

    let ctx =
        Context.create ()
        |> Context.setHttpClient client
        |> Context.setProject "test"
        |> Context.addHeader ("api-key", "test-key")

    // Act
    let req = Assets.Entity.get 42L
    let! result = task {
        let! result = runHandler req ctx
        match result with
        | Ok _ ->
            return false
        | _ -> return true
    }

    // Assert
    test <@ result @>
}

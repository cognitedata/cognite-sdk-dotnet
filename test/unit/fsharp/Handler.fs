module Tests.Handler

open System.IO

open Xunit
open Swensen.Unquote

open System
open System.Net.Http
open System.Threading
open System.Threading.Tasks

open FSharp.Control.Tasks.V2


open Fusion
open System.Net

    type HttpMessageHandlerStub (sendAsync: Func<HttpRequestMessage, CancellationToken, Task<HttpResponseMessage>>) =
        inherit HttpMessageHandler ()
        let sendAsync = sendAsync

        override self.SendAsync(request: HttpRequestMessage, cancellationToken: CancellationToken) : Task<HttpResponseMessage> =
            task {
                return! sendAsync.Invoke(request, cancellationToken)
            }

[<Fact>]
let ``Get asset with retry is Ok``() = async {
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
        defaultContext
        |> setHttpClient client
        |> addHeader ("api-key", "test-key")

    // Act
    let req = getAsset 42L |> retry 0<ms> 5
    let! result = runHandler req ctx
    let retries' = retries

    // Assert
    test <@ Result.isOk result @>
    test <@ retries' = 1 @>
}

[<Fact>]
let ``Get asset with retries on server internal error``() = async {
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
        defaultContext
        |> setHttpClient client
        |> addHeader ("api-key", "test-key")

    // Act
    let req = getAsset 42L |> retry 0<ms> 5
    let! result = runHandler req ctx
    let retries' = retries

    // Assert
    test <@ Result.isError result @>
    test <@ retries' = 6 @>
}

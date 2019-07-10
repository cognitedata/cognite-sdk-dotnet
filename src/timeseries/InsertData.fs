namespace Cognite.Sdk

open System.Net.Http
open System.Runtime.CompilerServices
open System.Threading.Tasks

open FSharp.Control.Tasks.V2
open Thoth.Json.Net

open Cognite.Sdk
open Cognite.Sdk.Api
open Cognite.Sdk.Timeseries

[<RequireQualifiedAccess>]
module InsertData =
    [<Literal>]
    let Url = "/timeseries/data"

    type DataPoints = {
        DataPoints: DataPointDto seq
        Identity: Identity
    } with
        member this.Encoder =
            Encode.object [
                yield ("datapoints", Seq.map (fun (it: DataPointDto) -> it.Encoder) this.DataPoints |> Encode.seq)
                match this.Identity with
                | CaseId id -> yield "id", Encode.int64 id
                | CaseExternalId externalId -> yield "externalId", Encode.string externalId
            ]

    type PointRequest = {
        Items: DataPoints seq
    } with
        member this.Encoder =
            Encode.object [
                yield ("items", Seq.map (fun (it: DataPoints) -> it.Encoder) this.Items |> Encode.seq)
            ]

    let insertData (items: seq<DataPoints>) (fetch: HttpHandler<HttpResponseMessage, string, string>) =
        let request : PointRequest = { Items = items }
        let body = Encode.stringify request.Encoder

        POST
        >=> setVersion V10
        >=> setBody body
        >=> setResource Url
        >=> fetch

[<AutoOpen>]
module InsertDataApi =
    /// **Description**
    ///
    /// Inserts a list of data points to a time series. If a data point is
    /// posted to a timestamp that is already in the series, the existing
    /// data point for that timestamp will be overwritten.
    ///
    /// **Parameters**
    ///   * `name` - The name of the timeseries to insert data points into.
    ///   * `items` - The list of data points to insert.
    ///   * `ctx` - The request HTTP context to use.
    ///
    /// **Output Type**
    ///   * `Async<Result<HttpResponse,ResponseError>>`
    ///
    let insertDataBy (items: InsertData.DataPoints list) (next: NextHandler<string,string>) =
        InsertData.insertData items fetch next

    let insertDataAsync (items: seq<InsertData.DataPoints>) =
        InsertData.insertData items fetch Async.single

[<Extension>]
type InsertDataExtensions =
    /// <summary>
    /// Insert data into named time series.
    /// </summary>
    /// <param name="name">The name of the timeseries to insert data into.</param>
    /// <param name="items">The list of data points to insert.</param>
    /// <returns>True if successful.</returns>
    [<Extension>]
    static member InsertDataAsync (this: Client) (items: DataPointsWritePoco seq) : Task<bool> =
        let items' =
            Seq.map  (fun (item: DataPointsWritePoco) ->
                {
                    DataPoints = Seq.map DataPointDto.FromPoco item.DataPoints
                    Identity = item.Identity
                }: InsertData.DataPoints
            ) items

        task {
            let! ctx = insertDataAsync items' this.Ctx
            match ctx.Result with
            | Ok response ->
                return true
            | Error error ->
               return raise (Error.error2Exception error)
        }



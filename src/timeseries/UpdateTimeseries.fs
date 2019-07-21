namespace Fusion

open System
open System.Collections.Generic
open System.Net.Http
open System.IO
open System.Threading.Tasks
open System.Runtime.CompilerServices

open Thoth.Json.Net
open FSharp.Control.Tasks.V2

open Fusion
open Fusion.Api
open Fusion.Common
open Fusion.Timeseries

[<RequireQualifiedAccess>]
module UpdateTimeseries =
    [<Literal>]
    let Url = "/timeseries/update"
    
    type MetaDataChange = {
        Add : Map<string, string> option
        Remove : string seq
    }

    type MetaDataUpdate =
        | Set of Map<string, string>
        | Change of MetaDataChange
    
    type SecurityCategoriesChange = {
        Add : int64 seq
        Remove : int64 seq
    }

    type SecurityCategoriesUpdate =
        | Set of int64 seq
        | Change of SecurityCategoriesChange

    type Option =
        private
        | CaseExternalId of string option
        | CaseName of string option
        | CaseMetaData of MetaDataUpdate option
        | CaseUnit of string option
        | CaseAssetId of int64 option
        | CaseDescription of string option
        | CaseSecurityCategories of SecurityCategoriesUpdate option

        static member SetExternalId externalId =
            CaseExternalId externalId

        static member SetName name =
            CaseName name
        
        static member SetMetaData (md : IDictionary<string, string>) =
            md |> Seq.map (|KeyValue|) |> Map.ofSeq |> MetaDataUpdate.Set |> Some |> CaseMetaData

        static member SetMetaData (md : Map<string, string>) =
            md |> MetaDataUpdate.Set |> Some |> CaseMetaData

        static member ClearMetaData () =
            CaseMetaData None
        
        static member ChangeMetaData (add: IDictionary<string, string>, remove: string seq) : Option =
            ({
                Add =
                    if isNull add then
                        None
                    else
                        add |> Seq.map (|KeyValue|) |> Map.ofSeq |> Some
                Remove = if isNull remove then Seq.empty else remove
            } : MetaDataChange) |> MetaDataUpdate.Change |> Some |> CaseMetaData
        
        static member ChangeMetaData (add: Map<string, string> option, remove: string seq) : Option =
            ({
                Add = add
                Remove = remove
            } : MetaDataChange) |> MetaDataUpdate.Change |> Some |> CaseMetaData

        static member SetUnit unit =
            CaseUnit unit
        
        static member SetAssetId assetId =
            CaseAssetId assetId
        
        static member ClearAssetId () =
            CaseAssetId None

        static member SetDescription description =
            CaseDescription description
        
        static member SetSecurityCategories (ct : int64 seq) =
            ct |> Set |> Some |> CaseSecurityCategories
        
        static member ClearSecurityCategories () =
            CaseSecurityCategories None
        
        static member ChangeSecurityCategories (add: int64 seq) (remove: int64 seq) : Option =
            {
                Add = if isNull add then Seq.empty else add
                Remove = if isNull remove then Seq.empty else remove
            } |> Change |> Some |> CaseSecurityCategories

    let renderUpdateField (arg: Option) =
        match arg with
        | CaseExternalId id ->
            "externalId", Encode.object [
                match id with
                | Some value -> yield "set", Encode.string value
                | None -> yield "setNull", Encode.bool true
            ]
        | CaseName name ->
            "name", Encode.object [
                match name with
                | Some value -> yield "set", Encode.string value
                | None -> yield "setNull", Encode.bool true
            ]
        | CaseMetaData optMeta ->
            "metadata", Encode.object [
                match optMeta with
                | Some meta ->
                    match meta with
                    | MetaDataUpdate.Set data ->
                        yield "set", Encode.propertyBag data
                    | MetaDataUpdate.Change data ->
                        if data.Add.IsSome then yield "add", Encode.propertyBag data.Add.Value
                        yield "remove", Encode.seq (Seq.map Encode.string data.Remove)
                | None -> ()
            ]
        | CaseUnit unt ->
            "unit", Encode.object [
                match unt with
                | Some value -> yield "set", Encode.string value
                | None -> yield "setNull", Encode.bool true
            ]
        | CaseAssetId id ->
            "assetId", Encode. object [
                match id with
                | Some value -> yield "set", Encode.int64 value
                | None -> yield "setNull", Encode.bool true
            ]
        | CaseDescription desc ->
            "description", Encode.object [
                match desc with
                | Some value -> yield "set", Encode.string value
                | None -> yield "setNull", Encode.bool true
            ]
        | CaseSecurityCategories cat ->
            "securityCategories", Encode.object [
                match cat with
                | Some ct ->
                    match ct with
                    | Set data ->
                        yield "set", Encode.int53seq data
                    | Change data ->
                        yield "add", Encode.int53seq data.Add
                        yield "remove", Encode.int53seq data.Remove
                | None -> ()
            ]
    type TimeseriesUpdateRequest = {
        Id: Identity
        Params: Option seq
    } with
        member this.Encoder =
            Encode.object [
                yield
                    match this.Id with
                    | Identity.CaseId id -> "id", Encode.int53 id
                    | Identity.CaseExternalId id -> "externalId", Encode.string id
                yield "update", Encode.object [
                    yield! this.Params |> Seq.map(renderUpdateField)
                ]
            ]

    type TimeseriesUpdateRequests = {
        Items: TimeseriesUpdateRequest seq
    } with
        member this.Encoder =
            Encode.object [
                "items", this.Items |> Seq.map(fun item -> item.Encoder) |> Encode.seq
            ]
    type TimeseriesResponse = {
        Items: TimeseriesReadDto seq
    } with
        static member Decoder : Decoder<TimeseriesResponse> =
            Decode.object (fun get -> {
                Items = get.Required.Field "items" (Decode.list TimeseriesReadDto.Decoder)
            })
    
    let updateTimeseries (args: (Identity * Option list) list) (fetch: HttpHandler<HttpResponseMessage, Stream, 'a>) =
        let decoder = decodeResponse TimeseriesResponse.Decoder (fun res -> res.Items)
        let request : TimeseriesUpdateRequests = {
           Items = [
               yield! args |> Seq.map(fun (assetId, args) -> { Id = assetId; Params = args })
           ]
        }

        POST
        >=> setVersion V10
        >=> setContent (Content.JsonValue request.Encoder)
        >=> setResource Url
        >=> fetch
        >=> decoder

[<AutoOpen>]
module UpdateTimeseriesApi =
    /// **Description**
    /// Updates multiple timesreies within the same project.
    /// This operation supports partial updates, meaning that fields omitted from the requests are not changed
    /// **Parameters**
    ///   * `args` - `(Identity * UpdateTimeseries.Option list)` list of attributes to update
    ///   * `next` - `NextHandler<seq<TimeseriesReadDto>,'a>` async handler
    ///
    /// **Output Type**
    ///   * `HttpContext -> Async<Context<'a>>`
    ///
    /// **Exceptions**
    ///
    let updateTimeseries (args: (Identity * (UpdateTimeseries.Option list)) list) (next: NextHandler<TimeseriesReadDto seq, 'a>) : HttpContext -> Async<Context<'a>> =
        UpdateTimeseries.updateTimeseries args fetch next
    
    /// **Description**
    /// Updates multiple timesreies within the same project.
    /// This operation supports partial updates, meaning that fields omitted from the requests are not changed
    /// **Parameters**
    ///   * `args` - `(Identity * UpdateTimeseries.Option list) list` list of attributes to update
    ///
    /// **Output Type**
    ///   * `HttpContext -> Async<Context<seq<TimeseriesReadDto>>>`
    ///
    /// **Exceptions**
    ///
    let updateTimeseriesAsync (args: (Identity * (UpdateTimeseries.Option list)) list) : HttpContext -> Async<Context<TimeseriesReadDto seq>> =
        UpdateTimeseries.updateTimeseries args fetch Async.single

[<Extension>]
type UpdateTimeseriesExtensions =
    [<Extension>]
    static member UpdateTimeseriesAsync (this: Client, timeseries: ValueTuple<Identity, UpdateTimeseries.Option seq> seq) : Task<TimeseriesReadPoco seq> =
        task {
            let timeseries' = timeseries |> Seq.map (fun struct (id, options) -> (id, options |> List.ofSeq)) |> List.ofSeq
            let! ctx = updateTimeseriesAsync timeseries' this.Ctx
            match ctx.Result with
            | Ok response ->
                return response |> Seq.map (fun timeseries -> timeseries.ToPoco ())
            | Error error ->
                let err = error2Exception error
                return raise err
        }

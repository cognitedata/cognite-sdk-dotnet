/// Common types for the SDK.
namespace CogniteSdk

open System
 open System.Text.RegularExpressions
open System.Reflection

open Oryx
open Thoth.Json.Net

type ApiVersion =
    | V05
    | V06
    | V10

    override this.ToString () =
        match this with
        | V05 -> "0.5"
        | V06 -> "0.6"
        | V10 -> "v1"

/// Id or ExternalId
type Identity =
    internal
    | CaseId of int64
    | CaseExternalId of string

    static member Id id =
        CaseId id
    static member ExternalId id =
        CaseExternalId id

    member this.Encoder =
        Encode.object [
            match this with
            | CaseId id -> yield "id", Encode.int53 id
            | CaseExternalId id -> yield "externalId", Encode.string id
        ]

    member this.Render =
        match this with
        | CaseId id -> "id", Encode.int64 id
        | CaseExternalId id -> "externalId", Encode.string id

type Numeric =
    internal
    | CaseString of string
    | CaseInteger of int64
    | CaseFloat of double

    static member String value =
        CaseString value

    static member Integer value =
        CaseInteger value

    static member Float value =
        CaseFloat value

type TimeRange = {
    Max: DateTime
    Min: DateTime
} with
    member this.Encoder =
        Encode.object [
            yield "max", DateTimeOffset(this.Max).ToUnixTimeMilliseconds() |> Encode.int64
            yield "min", DateTimeOffset(this.Min).ToUnixTimeMilliseconds() |> Encode.int64
        ]
[<AutoOpen>]
module Patterns =
    /// Active pattern to permit pattern matching over numeric values.
    let (|Integer|Float|String|) (value : Numeric) : Choice<int64, float, string>  =
        match value with
        | CaseInteger value -> Integer value
        | CaseFloat value -> Float value
        | CaseString value -> String value

    /// Active pattern to permit pattern matching over identity values.
    let (|Id|ExternalId|) (value : Identity) : Choice<int64, string>  =
        match value with
        | CaseId value -> Id value
        | CaseExternalId value -> ExternalId value

    let (|ParseInteger|_|) (str: string) =
       let mutable intvalue = 0
       if System.Int32.TryParse(str, &intvalue) then Some(intvalue)
       else None

    let (|ParseRegex|_|) regex str =
       let m = Regex(regex).Match(str)
       if m.Success
       then Some (List.tail [ for x in m.Groups -> x.Value ])
       else None

[<AutoOpen>]
module Common =
    [<Literal>]
    let MaxLimitSize = 1000

module Context =
    let urlBuilder (request: HttpRequest) =
        let extra = request.Extra
        let version = extra.["apiVersion"]
        let project = extra.["project"]
        let resource = extra.["resource"]
        let serviceUrl =
            if Map.containsKey "serviceUrl" extra
            then extra.["serviceUrl"]
            else "https://api.cognitedata.com"

        if not (Map.containsKey "hasAppId" extra)
        then failwith "Client must set the Application ID (appId)"

        sprintf "%s/api/%s/projects/%s%s" serviceUrl version project resource

    let private version =
        let version = Assembly.GetExecutingAssembly().GetName().Version
        {| Major=version.Major; Minor=version.Minor; Build=version.Build |}

    /// Set the project to connect to.
    let setProject (project: string) (context: HttpContext) =
        { context with Request = { context.Request with Extra = context.Request.Extra.Add("project", project) } }

    let setAppId (appId: string) (context: HttpContext) =
        { context with Request = { context.Request with Headers =  ("x-cdp-app", appId) :: context.Request.Headers; Extra = context.Request.Extra.Add("hasAppId", "true") } }

    let setServiceUrl (serviceUrl: string) (context: HttpContext) =
        { context with Request = { context.Request with Extra = context.Request.Extra.Add("serviceUrl", serviceUrl) } }

    let create () =
        Context.defaultContext
        |> Context.setUrlBuilder urlBuilder

[<AutoOpen>]
module Handlers =
    let setResource (resource: string) (next: NextHandler<_,_>) (context: HttpContext) =
        next { context with Request = { context.Request with Extra = context.Request.Extra.Add("resource", resource) } }

    let setVersion (version: ApiVersion) (next: NextHandler<_,_>) (context: HttpContext) =
        next { context with Request = { context.Request with Extra = context.Request.Extra.Add("apiVersion", version.ToString ()) } }


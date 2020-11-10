// Copyright 2019 Cognite AS
// SPDX-License-Identifier: Apache-2.0

namespace Oryx.Cognite.Beta

open System
open System.Net.Http

open Oryx
open Oryx.Cognite
open Oryx.Cognite.Beta

open CogniteSdk
open CogniteSdk.Beta

[<RequireQualifiedAccess>]
module Domains =
    [<Literal>]
    let Url = "/domains"

    let BaseUrl = new Uri("http://localhost:8080") // new Uri("https://templates.cognite.ai")

    let withBaseUrl(baseUrl: Uri) (next: NextFunc<_,_>) (context: HttpContext) =
        next { context with Request = { context.Request with Items = context.Request.Items.Add(PlaceHolder.BaseUrl, Value.Url baseUrl) } }

    let domainUrlPrefix (domainRef: DomainRef) : String =
        Url +/ Uri.EscapeUriString(domainRef.ExternalId) +/ domainRef.Version.ToString()

    let domainUrl (domainRef: DomainRef) (path: String) : String =
        domainUrlPrefix (domainRef) +/ path

    /// <summary>
    /// Retrieves list of domains and a cursor if given limit is exceeded.
    /// </summary>
    /// <param name="query">The query with limit and cursor.</param>
    /// <returns>List of domains.</returns>
    let list (query: DomainQuery) : HttpHandler<HttpResponseMessage, ItemsWithCursor<Domain>, 'a> =
        withLogMessage "domains:list"
        >=> withBaseUrl BaseUrl
        >=> getWithQuery query Url

    /// <summary>
    /// Runs a query by using GraphQL against a domain.
    /// </summary>
    /// <param name="domainRef">Unique reference to the domain.</param>
    /// <param name="query">The GraphQL query.</param>
    /// <returns>The GraphQL result.</returns>
    let graphql (domainRef: DomainRef) (query: string) : HttpHandler<HttpResponseMessage, GraphQlResult> =
        withLogMessage "domain:schema"
        >=> withBaseUrl BaseUrl
        >=> post (GraphQlQuery ( Query = query )) (domainUrl domainRef "/graphql")

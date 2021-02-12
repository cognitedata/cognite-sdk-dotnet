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
module TemplateGroups =
    [<Literal>]
    let Url = "/templategroups"

    // TODO: Make TemplateGroupRef sdk type which consists of TemplateGroup ExternalID and version? Like DomainRef
    // TODO: Remove this and replace with in-line method?
    let domainUrlPrefix (domainRef: DomainRef) : String =
        Url
        +/ Uri.EscapeUriString(domainRef.ExternalId)
        +/ "versions"
        +/ domainRef.Version.ToString()

    // TODO: remove this and replace with in-line method in graphql?
    let domainUrl (domainRef: DomainRef) (path: String) : String =
        domainUrlPrefix domainRef +/ path

    /// <summary>
    /// Retrieves list of template groups and a cursor if given limit is exceeded.
    /// </summary>
    /// <param name="query">The query with limit and cursor.</param>
    /// <returns>List of domains.</returns>
    let list (query: TemplateGroupFilter) : HttpHandler<unit, ItemsWithCursor<TemplateGroup>, 'a> =
        let url = Url

        withLogMessage "templategroups:list"
        >=> list query url


    let listVersions (externalId: string) (query: TemplateGroupVersionFilter) : HttpHandler<unit, ItemsWithCursor<TemplateGroupVersion>, 'a> =
        let url = Url +/ externalId +/ "versions"

        withLogMessage "templategroups:listVersions"
        >=> list query url

    let listVersions (templateGroup: TemplateGroup) (query: TemplateGroupVersionFilter) : HttpHandler<unit, ItemsWithCursor<TemplateGroupVersion>, 'a> =
        listVersions (templateGroup.ExternalId) query


    /// <summary>
    /// Runs a query by using GraphQL against a domain.
    /// </summary>
    /// <param name="domainRef">Unique reference to the domain.</param>
    /// <param name="query">The GraphQL query.</param>
    /// <returns>The GraphQL result.</returns>
    let graphql (domainRef: DomainRef) (query: string) : HttpHandler<unit, GraphQlResult, 'a> =
        let url = domainUrl domainRef "/graphql"
        let query = GraphQlQuery(Query = query)

        withLogMessage "templategroups:schema"
        >=> postV10 query url

    // FIXME: We don't want JSON Graphql result
    // Return Query type which contains data of string?
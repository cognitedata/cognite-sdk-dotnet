// Copyright 2021 Cognite AS
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

    let templateGroupVersionsUrl externalId =
        Url +/ externalId +/ "versions"

    /// <summary>
    /// Retrieves list of template groups and a cursor if given limit is exceeded.
    /// </summary>
    /// <param name="query">The query with limit and cursor.</param>
    /// <returns>List of domains.</returns>
    let list (query: TemplateGroupFilter) : HttpHandler<unit, ItemsWithCursor<TemplateGroup>, 'a> =
        let url = Url

        withLogMessage "templategroups:list"
        >=> list query url

    /// <summary>
    /// Retrieves a list of versions of a given template group, and a version if given limit is exceeded.
    /// </summary>
    let listVersions (externalId: string) (query: TemplateGroupVersionFilter) : HttpHandler<unit, ItemsWithCursor<TemplateGroupVersion>, 'a> =
        let url = templateGroupVersionsUrl externalId

        withLogMessage "templategroups:listVersions"
        >=> Handler.list query url

    let listVersionsFromTemplateGroup (templateGroup: TemplateGroup) (query: TemplateGroupVersionFilter) : HttpHandler<unit, ItemsWithCursor<TemplateGroupVersion>, 'a> =
        listVersions (templateGroup.ExternalId) query


    /// <summary>
    /// Runs a query by using GraphQL against a domain.
    /// </summary>
    /// <param name="domainRef">Unique reference to the domain.</param>
    /// <param name="query">The GraphQL query.</param>
    /// <returns>The GraphQL result.</returns>
    //let graphql (domainRef: DomainRef) (query: string) : HttpHandler<unit, GraphQlResult, 'a> =
    let graphql (externalId: string) (templateGroupVersion: string) (query: string) : HttpHandler<unit, GraphQlResult, 'a> =
        let url =
            templateGroupVersionsUrl externalId
            +/ templateGroupVersion
            +/ "graphql"

        let query = GraphQlQuery(Query = query)

        withLogMessage "templategroups:schema"
        >=> postV10 query url

    let graphql (externalId: string) (templateGroupVersion:TemplateGroupVersion) (query: string) : HttpHandler<unit, GraphQlResult, 'a> =
        graphql
            externalId
            (templateGroupVersion.Version |> string)
            query

    // FIXME: We don't want JSON Graphql result
    // Return Query type which contains data of string?
// Copyright 2026 Cognite AS
// SPDX-License-Identifier: Apache-2.0

#nullable enable

using System;
using System.Collections.Generic;

namespace CogniteSdk.Alpha
{
    /// <summary>
    /// Request to register an integration's tasks, emitted views, merge mode, mappings and bindings
    /// with the CDF writer. Fully replaces the live config for the integration.
    /// </summary>
    public class CdfWriterSetupRequest
    {
        /// <summary>
        /// Creates an empty setup request.
        /// </summary>
        public CdfWriterSetupRequest()
        {
        }

        /// <summary>
        /// Creates a setup request with the given emitted views. The dictionary is copied.
        /// </summary>
        /// <param name="emittedViews">Map from emitted resource to the view its upsert items are shaped for</param>
        public CdfWriterSetupRequest(Dictionary<string, CdfWriterViewReference>? emittedViews)
        {
            EmittedViews = emittedViews is null ? null : new Dictionary<string, CdfWriterViewReference>(emittedViews);
        }

        /// <summary>
        /// External ID of the integration.
        /// </summary>
        public string? IntegrationId { get; set; }
        /// <summary>
        /// External ID of the extractor type, for example "cognite-opcua".
        /// </summary>
        public string? ExtractorType { get; set; }
        /// <summary>
        /// Version of the extractor.
        /// </summary>
        public string? ExtractorVersion { get; set; }
        /// <summary>
        /// Integration-wide upsert merge behaviour, <c>replace</c> or <c>preserve</c>.
        /// Defaults to <c>replace</c> when omitted.
        /// </summary>
        public string? Merge { get; set; }
        /// <summary>
        /// The extractor's integration tasks and the resources each one emits.
        /// </summary>
        public IEnumerable<CdfWriterTaskDeclaration>? Tasks { get; set; }
        /// <summary>
        /// Map from emitted resource, for example "CogniteTimeseries", to the view its upsert items are shaped for.
        /// </summary>
        public Dictionary<string, CdfWriterViewReference>? EmittedViews { get; private set; }

        /// <summary>
        /// Adds the view that the upsert items for an emitted resource are shaped for.
        /// </summary>
        /// <param name="resource">Emitted resource, for example "CogniteTimeseries"</param>
        /// <param name="view">View to shape the upsert items for</param>
        /// <exception cref="ArgumentNullException"><paramref name="resource"/> or <paramref name="view"/> is null</exception>
        public void AddEmittedView(string resource, CdfWriterViewReference view)
        {
            if (resource is null) throw new ArgumentNullException(nameof(resource));
            if (view is null) throw new ArgumentNullException(nameof(view));
            EmittedViews ??= new Dictionary<string, CdfWriterViewReference>();
            EmittedViews[resource] = view;
        }

        /// <summary>
        /// Mapping definitions. External IDs must be unique within the request.
        /// </summary>
        public IEnumerable<CdfWriterMapping>? Mappings { get; set; }
        /// <summary>
        /// Bindings attaching tasks to mappings.
        /// </summary>
        public IEnumerable<CdfWriterBinding>? Bindings { get; set; }
    }

    /// <summary>
    /// An integration task and the resources it emits.
    /// </summary>
    public class CdfWriterTaskDeclaration
    {
        /// <summary>
        /// The integration task name, verbatim.
        /// </summary>
        public string? Name { get; set; }
        /// <summary>
        /// Emitted resources, for example "CogniteTimeseries" or "edges".
        /// </summary>
        public IEnumerable<string>? Resources { get; set; }
    }

    /// <summary>
    /// Reference to a data modeling view.
    /// </summary>
    public class CdfWriterViewReference
    {
        /// <summary>
        /// Space of the view.
        /// </summary>
        public string? Space { get; set; }
        /// <summary>
        /// External ID of the view.
        /// </summary>
        public string? ExternalId { get; set; }
        /// <summary>
        /// Version of the view.
        /// </summary>
        public string? Version { get; set; }
    }

    /// <summary>
    /// A mapping definition.
    /// </summary>
    public class CdfWriterMapping
    {
        /// <summary>
        /// Creates an empty mapping.
        /// </summary>
        public CdfWriterMapping()
        {
        }

        /// <summary>
        /// Creates a mapping with the given resource configs. The dictionary is copied.
        /// </summary>
        /// <param name="resources">Map from emitted resource to its config block</param>
        public CdfWriterMapping(Dictionary<string, CdfWriterMappingResource>? resources)
        {
            Resources = resources is null ? null : new Dictionary<string, CdfWriterMappingResource>(resources);
        }

        /// <summary>
        /// Identifier of the mapping, scoped to the integration.
        /// </summary>
        public string? ExternalId { get; set; }
        /// <summary>
        /// Human-readable name.
        /// </summary>
        public string? Name { get; set; }
        /// <summary>
        /// Optional description.
        /// </summary>
        public string? Description { get; set; }
        /// <summary>
        /// Mapping kind, for example "spaceRouting".
        /// </summary>
        public string? Kind { get; set; }
        /// <summary>
        /// CDF API version to write with, <c>stable</c> or <c>beta</c>. Defaults to <c>stable</c> when omitted.
        /// </summary>
        public string? ApiVersion { get; set; }
        /// <summary>
        /// Map from emitted resource to its config block.
        /// </summary>
        public Dictionary<string, CdfWriterMappingResource>? Resources { get; private set; }

        /// <summary>
        /// Adds the config block for an emitted resource.
        /// </summary>
        /// <param name="resource">Emitted resource, for example "CogniteTimeseries"</param>
        /// <param name="config">Config block for the resource</param>
        /// <exception cref="ArgumentNullException"><paramref name="resource"/> or <paramref name="config"/> is null</exception>
        public void AddResource(string resource, CdfWriterMappingResource config)
        {
            if (resource is null) throw new ArgumentNullException(nameof(resource));
            if (config is null) throw new ArgumentNullException(nameof(config));
            Resources ??= new Dictionary<string, CdfWriterMappingResource>();
            Resources[resource] = config;
        }
    }

    /// <summary>
    /// Config for one emitted resource in a mapping.
    /// </summary>
    public class CdfWriterMappingResource
    {
        /// <summary>
        /// Space routing parameters.
        /// </summary>
        public CdfWriterSpaceRoutingParameters? Parameters { get; set; }
        /// <summary>
        /// Optional Kuiper expression reshaping upsert items.
        /// </summary>
        public string? Properties { get; set; }
        /// <summary>
        /// Create missing target nodes of direct relations. Defaults to true when omitted.
        /// </summary>
        public bool? AutoCreateDirectRelations { get; set; }
    }

    /// <summary>
    /// Space routing parameters for one resource.
    /// </summary>
    public class CdfWriterSpaceRoutingParameters
    {
        /// <summary>
        /// Path to the value the rules match, as segments. <c>["externalId"]</c> is the item's external ID,
        /// other paths are read from the upsert item's properties. Required together with <see cref="Rules"/>.
        /// </summary>
        public IEnumerable<string>? Property { get; set; }
        /// <summary>
        /// Rules tried in order, the first match wins.
        /// </summary>
        public IEnumerable<CdfWriterSpaceRoutingRule>? Rules { get; set; }
        /// <summary>
        /// Space used when no rule matches.
        /// </summary>
        public string? Default { get; set; }
    }

    /// <summary>
    /// A space routing rule.
    /// </summary>
    public class CdfWriterSpaceRoutingRule
    {
        /// <summary>
        /// Regex matched against the whole value.
        /// </summary>
        public string? Pattern { get; set; }
        /// <summary>
        /// Space to route matching items to.
        /// </summary>
        public string? Space { get; set; }
    }

    /// <summary>
    /// A binding attaching tasks to a mapping.
    /// </summary>
    public class CdfWriterBinding
    {
        /// <summary>
        /// Integration task names. Explicit, no wildcards.
        /// </summary>
        public IEnumerable<string>? TaskIds { get; set; }
        /// <summary>
        /// External ID of a mapping in the same request.
        /// </summary>
        public string? MappingId { get; set; }
    }

    /// <summary>
    /// Response from a successful setup.
    /// </summary>
    public class CdfWriterSetupResponse
    {
        /// <summary>
        /// External ID of the integration.
        /// </summary>
        public string? IntegrationId { get; set; }
        /// <summary>
        /// The config revision that was created.
        /// </summary>
        public int Version { get; set; }
    }
}

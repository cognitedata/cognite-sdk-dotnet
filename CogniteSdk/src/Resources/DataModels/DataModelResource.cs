using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CogniteSdk.DataModels;

namespace CogniteSdk.Resources.DataModels
{
    /// <summary>
    /// Base class for a resource for a specific view/container in CDF data models.
    /// </summary>
    public abstract class BaseDataModelResource<T>
    {
        /// <summary>
        /// The source view this model represents.
        /// </summary>
        public abstract ViewIdentifier View { get; }

        private readonly DataModelsResource _resource;
        private HashSet<ViewIdentifier> _allowedViewIdentifiers { get; }

        /// <summary>
        /// Create a new data model resource. The data models resource must be passed.
        /// </summary>
        /// <param name="resource"></param>
        /// <param name="allowedViewIdentifiers">View Identifiers this resource is allowed to query for in addition to the default view.</param>
        public BaseDataModelResource(DataModelsResource resource, IEnumerable<ViewIdentifier> allowedViewIdentifiers = null)
        {
            _resource = resource;
            _allowedViewIdentifiers = new HashSet<ViewIdentifier>(allowedViewIdentifiers ?? new List<ViewIdentifier>(), ViewIdentifier.ValueTypeEqualityComparer);
        }

        private bool _viewIsAllowed(ViewIdentifier view) => ViewIdentifier.ValueTypeEqualityComparer.Equals(view, View) || _allowedViewIdentifiers.Contains(view);
        private void _assertViewIsAllowed(ViewIdentifier view)
        {
            if (!_viewIsAllowed(view)) throw new InvalidOperationException($"Resource does not allow use of non-default view {view}");
        }

        /// <summary>
        /// Upsert a list of instances to the specified view.
        /// </summary>
        /// <param name="items">Instances to upsert.</param>
        /// <param name="options">General opions for upsert.</param>
        /// <param name="token">Cancellation token.</param>
        /// <returns>A list of upserted instances.</returns>
        public async Task<IEnumerable<SlimInstance>> UpsertAsync(IEnumerable<SourcedInstanceWrite<T>> items, UpsertOptions options, CancellationToken token = default)
        {
            var upserts = items.Select(r =>
            {
                var sources = new[] {
                    new InstanceData<T> {
                        Properties = r.Properties,
                        Source = View
                    }
                };
                if (r is SourcedNodeWrite<T> node)
                {
                    return (BaseInstanceWrite)new NodeWrite
                    {
                        ExistingVersion = node.ExistingVersion,
                        Space = node.Space,
                        ExternalId = node.ExternalId,
                        Type = node.Type,
                        Sources = sources
                    };
                }
                else if (r is SourcedEdgeWrite<T> edge)
                {
                    return new EdgeWrite
                    {
                        ExistingVersion = edge.ExistingVersion,
                        Space = edge.Space,
                        ExternalId = edge.ExternalId,
                        Type = edge.Type,
                        StartNode = edge.StartNode,
                        EndNode = edge.EndNode,
                        Sources = sources
                    };
                }
                else
                {
                    throw new ArgumentException("items must be a list of SourcedNode or SourcedEdge");
                }
            }).ToList();

            var opts = options ?? new UpsertOptions();

            return await _resource.UpsertInstances(new InstanceWriteRequest
            {
                AutoCreateDirectRelations = opts.AutoCreateDirectRelations,
                AutoCreateEndNodes = opts.AutoCreateEndNodes,
                AutoCreateStartNodes = opts.AutoCreateStartNodes,
                SkipOnVersionConflict = opts.SkipOnVersionConflict,
                Replace = opts.Replace,
                Items = upserts
            }, token);
        }

        private static T2 GetFromNestedDicts<T2>(Dictionary<string, Dictionary<string, T2>> properties, ViewIdentifier view)
        {
            if (!properties.TryGetValue(view.Space, out var bySource))
            {
                return default;
            }

            if (!bySource.TryGetValue($"{view.ExternalId}/{view.Version}", out var v))
            {
                return default;
            }
            return v;

        }

        /// <summary>
        /// Retrieve a list of instances by ID.
        /// </summary>
        /// <param name="ids">IDs to retrieve.</param>
        /// <param name="token">Cancellation token</param>
        /// <returns>Retrieved instances.</returns>
        public async Task<IEnumerable<SourcedInstance<T>>> RetrieveAsync(IEnumerable<InstanceIdentifierWithType> ids, CancellationToken token = default)
        {
            return await _retrieveAsync<T>(ids, View, token);
        }

        /// <summary>
        /// Retrieve a list of instances by ID with a specific view.
        /// </summary>
        /// <param name="ids">IDs to retrieve.</param>
        /// <param name="viewIdentifier">Custom view identifier</param>
        /// <param name="token">Cancellation token</param>
        /// <returns>Retrieved instances.</returns>
        /// <exception cref="InvalidOperationException">Throws if viewIdentifier was not defined on DataModelResource creation, and is not the default view.</exception>
        public async Task<IEnumerable<SourcedInstance<T2>>> RetrieveAsync<T2>(IEnumerable<InstanceIdentifierWithType> ids, ViewIdentifier viewIdentifier, CancellationToken token = default)
        {
            _assertViewIsAllowed(viewIdentifier);
            return await _retrieveAsync<T2>(ids, viewIdentifier, token);
        }

        private async Task<IEnumerable<SourcedInstance<T2>>> _retrieveAsync<T2>(IEnumerable<InstanceIdentifierWithType> ids, ViewIdentifier viewIdentifier, CancellationToken token)
        {
            var results = await _resource.RetrieveInstances<Dictionary<string, Dictionary<string, T2>>>(new InstancesRetrieve
            {
                Items = ids,
                Sources = new[] {
                    new InstanceSource {
                        Source = viewIdentifier
                    }
                }
            }, token);
            return FromRaw(results.Items, viewIdentifier);
        }

        /// <summary>
        /// Delete the given list of instances.
        /// </summary>
        /// <param name="ids">Instance IDs to delete.</param>
        /// <param name="token">Cancellation token.</param>
        /// <returns>Deleted instance IDs.</returns>
        public async Task<IEnumerable<InstanceIdentifierWithType>> DeleteAsync(IEnumerable<InstanceIdentifierWithType> ids, CancellationToken token = default)
        {
            return await _resource.DeleteInstances(ids, token);
        }

        private static IEnumerable<SourcedInstance<T2>> FromRaw<T2>(IEnumerable<BaseInstance<Dictionary<string, Dictionary<string, T2>>>> items, ViewIdentifier view)
        {
            return items.Select(r =>
            {
                if (r is Edge<Dictionary<string, Dictionary<string, T2>>> edge)
                {
                    return (SourcedInstance<T2>)new SourcedEdge<T2>
                    {
                        Space = r.Space,
                        ExternalId = r.ExternalId,
                        Type = r.Type,
                        Version = r.Version,
                        StartNode = edge.StartNode,
                        EndNode = edge.EndNode,
                        Properties = GetFromNestedDicts(r.Properties, view),
                        CreatedTime = r.CreatedTime,
                        LastUpdatedTime = r.LastUpdatedTime,
                        DeletedTime = r.DeletedTime
                    };
                }
                else
                {
                    return new SourcedNode<T2>
                    {
                        Space = r.Space,
                        ExternalId = r.ExternalId,
                        Type = r.Type,
                        Version = r.Version,
                        Properties = GetFromNestedDicts(r.Properties, view),
                        CreatedTime = r.CreatedTime,
                        LastUpdatedTime = r.LastUpdatedTime,
                        DeletedTime = r.DeletedTime
                    };
                }
            }).ToList();
        }

        /// <summary>
        /// Filter instances.
        /// </summary>
        /// <param name="filter">Filter</param>
        /// <param name="token">Cancellation token</param>
        /// <returns>Filtered items with optional cursor.</returns>
        public async Task<ItemsWithCursor<SourcedInstance<T>>> FilterAsync(SourcedInstanceFilter filter, CancellationToken token = default)
        {
            return await _filterAsync<T>(filter, View, token);
        }

        /// <summary>
        /// Filter instances with a specific view. 
        /// </summary>
        /// <param name="filter">Filter</param>
        /// <param name="viewIdentifier">Custom view identifier</param>
        /// <param name="token">Cancellation token</param>
        /// <returns>Filtered items with optional cursor.</returns>
        /// <exception cref="InvalidOperationException">Throws if viewIdentifier was not defined on DataModelResource creation, and is not the default view.</exception>
        public async Task<ItemsWithCursor<SourcedInstance<T2>>> FilterAsync<T2>(SourcedInstanceFilter filter, ViewIdentifier viewIdentifier, CancellationToken token = default)
        {
            _assertViewIsAllowed(viewIdentifier);
            return await _filterAsync<T2>(filter, viewIdentifier, token);
        }

        private async Task<ItemsWithCursor<SourcedInstance<T2>>> _filterAsync<T2>(SourcedInstanceFilter filter, ViewIdentifier viewIdentifier, CancellationToken token)
        {
            var res = await _resource.FilterInstances<Dictionary<string, Dictionary<string, T2>>>(new InstancesFilter
            {
                Sources = new[] {
                    new InstanceSource {
                        Source = viewIdentifier
                    }
                },
                InstanceType = filter.InstanceType,
                Sort = filter.Sort,
                Filter = filter.Filter,
                Cursor = filter.Cursor,
            }, token);

            return new ItemsWithCursor<SourcedInstance<T2>>
            {
                Items = FromRaw(res.Items, viewIdentifier),
                NextCursor = res.NextCursor
            };
        }

        /// <summary>
        /// Search instances.
        /// </summary>
        /// <param name="search">Search object.</param>
        /// <param name="token">Cancellation token</param>
        /// <returns>Instances matching search.</returns>
        public async Task<IEnumerable<SourcedInstance<T>>> SearchAsync(SourcedInstanceSearch search, CancellationToken token = default)
        {
            var res = await _resource.SearchInstances<Dictionary<string, Dictionary<string, T>>>(new InstancesSearch
            {
                InstanceType = search.InstanceType,
                Sort = search.Sort,
                Filter = search.Filter,
                Limit = search.Limit,
                Query = search.Query,
                Properties = search.Properties,
                View = View,
            }, token);

            return FromRaw(res.Items, View);
        }
    }
}

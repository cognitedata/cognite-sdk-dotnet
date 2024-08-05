using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CogniteSdk.Beta.DataModels;
using CogniteSdk.Resources.Beta;

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

        /// <summary>
        /// Create a new data model resource. The data models resource must be passed.
        /// </summary>
        /// <param name="resource"></param>
        public BaseDataModelResource(DataModelsResource resource)
        {
            _resource = resource;
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

            return await _resource.UpsertInstances(new InstanceWriteRequest
            {
                AutoCreateDirectRelations = options.AutoCreateDirectRelations,
                AutoCreateEndNodes = options.AutoCreateEndNodes,
                AutoCreateStartNodes = options.AutoCreateStartNodes,
                SkipOnVersionConflict = options.SkipOnVersionConflict,
                Replace = options.Replace,
                Items = upserts
            }, token);
        }

        private T GetFromNestedDicts(Dictionary<string, Dictionary<string, T>> properties)
        {
            if (!properties.TryGetValue(View.Space, out var bySource))
            {
                return default;
            }

            if (!bySource.TryGetValue($"{View.ExternalId}/{View.Version}", out var v))
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
        public async Task<IEnumerable<SourcedInstance<T>>> RetrieveAsync(IEnumerable<InstanceIdentifier> ids, CancellationToken token = default)
        {
            var results = await _resource.RetrieveInstances<Dictionary<string, Dictionary<string, T>>>(new InstancesRetrieve
            {
                Items = ids,
                Sources = new[] {
                    new InstanceSource {
                        Source = View
                    }
                }
            }, token);
            return FromRaw(results.Items);
        }

        /// <summary>
        /// Delete the given list of instances.
        /// </summary>
        /// <param name="ids">Instance IDs to delete.</param>
        /// <param name="token">Cancellation token.</param>
        /// <returns>Deleted instance IDs.</returns>
        public async Task<IEnumerable<InstanceIdentifier>> DeleteAsync(IEnumerable<InstanceIdentifier> ids, CancellationToken token = default)
        {
            return await _resource.DeleteInstances(ids, token);
        }

        private IEnumerable<SourcedInstance<T>> FromRaw(IEnumerable<BaseInstance<Dictionary<string, Dictionary<string, T>>>> items)
        {
            return items.Select(r =>
            {
                if (r is Edge<Dictionary<string, Dictionary<string, T>>> edge)
                {
                    return (SourcedInstance<T>)new SourcedEdge<T>
                    {
                        Space = r.Space,
                        ExternalId = r.ExternalId,
                        Type = r.Type,
                        Version = r.Version,
                        StartNode = edge.StartNode,
                        EndNode = edge.EndNode,
                        Properties = GetFromNestedDicts(r.Properties),
                        CreatedTime = r.CreatedTime,
                        LastUpdatedTime = r.LastUpdatedTime,
                        DeletedTime = r.DeletedTime
                    };
                }
                else
                {
                    return new SourcedNode<T>
                    {
                        Space = r.Space,
                        ExternalId = r.ExternalId,
                        Type = r.Type,
                        Version = r.Version,
                        Properties = GetFromNestedDicts(r.Properties),
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
            var res = await _resource.FilterInstances<Dictionary<string, Dictionary<string, T>>>(new InstancesFilter
            {
                Sources = new[] {
                    new InstanceSource {
                        Source = View
                    }
                },
                InstanceType = filter.InstanceType,
                Sort = filter.Sort,
                Filter = filter.Filter,
                Cursor = filter.Cursor,
            }, token);

            return new ItemsWithCursor<SourcedInstance<T>>
            {
                Items = FromRaw(res.Items),
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

            return FromRaw(res.Items);
        }
    }
}

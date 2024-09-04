using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace CogniteSdk.Beta.DataModels
{
    /// <summary>
    /// Base class for instances with data for a specific source,
    /// to be used with `BaseDataModelResource`.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class SourcedInstanceBase<T>
    {
        /// <summary>
        /// Instance space.
        /// </summary>
        public string Space { get; set; }
        /// <summary>
        /// Instance external ID.
        /// </summary>
        public string ExternalId { get; set; }
        /// <summary>
        /// Optional instance type, which is a relation to another node.
        /// Required for edges.
        /// </summary>
        public DirectRelationIdentifier Type { get; set; }
        /// <summary>
        /// The data to be ingested.
        /// </summary>
        public T Properties { get; set; }
    }

    /// <summary>
    /// Special type for instances with data for a specific source,
    /// to be used with `BaseDataModelResource`.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class SourcedInstanceWrite<T> : SourcedInstanceBase<T>
    {

        /// <summary>
        /// Fail the ingestion request if the instance version is greater than this value.
        /// </summary>
        public int? ExistingVersion { get; set; }
    }

    /// <summary>
    /// Node with a specific source, to be used with `BaseDataModelResource`.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class SourcedNodeWrite<T> : SourcedInstanceWrite<T>
    {
    }

    /// <summary>
    /// Edge with a specific source, to be used with `BaseDataModelResource`.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class SourcedEdgeWrite<T> : SourcedInstanceWrite<T>
    {
        /// <summary>
        /// Start node of this edge.
        /// </summary>
        public DirectRelationIdentifier StartNode { get; set; }
        /// <summary>
        /// End node of this edge.
        /// </summary>
        public DirectRelationIdentifier EndNode { get; set; }
    }

    /// <summary>
    /// Options for upserts to an instance of `BaseDataModelResource`.
    /// </summary>
    public class UpsertOptions
    {
        /// <summary>
        /// Should we create missing target nodes of direct relations?
        /// If the target-container constraint has been specified for a direct relation,
        /// the target node cannot be auto-created. If you want to point direct relations
        /// to a space where you have only read access, this option must be set to false.
        /// </summary>
        public bool AutoCreateDirectRelations { get; set; } = true;
        /// <summary>
        /// Should we create missing start nodes for edges when ingesting?
        /// By default, the start node of an edge must exist before we can ingest the edge.
        /// </summary>
        public bool AutoCreateStartNodes { get; set; }
        /// <summary>
        /// Should we create missing end nodes for edges when ingesting?
        /// By default, the end node of an edge must exist before we can ingest the edge.
        /// </summary>
        public bool AutoCreateEndNodes { get; set; }
        /// <summary>
        /// If existingVersion is specified on any of the nodes/edges in the input,
        /// the default behaviour is that the entire ingestion will fail when version
        /// conflicts occur. If skipOnVersionConflict is set to true,
        /// items with version conflicts will be skipped instead. If no version is
        /// specified for nodes/edges, it will do the write directly.
        /// </summary>
        public bool SkipOnVersionConflict { get; set; }
        /// <summary>
        /// How do we behave when a property value exists?
        /// Do we replace all matching and existing values with the supplied values (true)?
        /// Or should we merge in new values for properties together with the existing values
        /// (false)? Note: This setting applies for all nodes or edges specified in the ingestion call.
        /// </summary>
        public bool Replace { get; set; }
    }


    /// <summary>
    /// Special type for instances with data for a specific source,
    /// to be used with `BaseDataModelResource`.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class SourcedInstance<T> : SourcedInstanceBase<T>
    {
        /// <summary>
        /// Current version of this instance.
        /// </summary>
        public int Version { get; set; }
        /// <summary>
        /// Time when this view was created in CDF in milliseconds since Jan 1, 1970.
        /// </summary>
        public long CreatedTime { get; set; }
        /// <summary>
        /// The last time this view was updated in CDF, in milliseconds since Jan 1, 1970.
        /// </summary>
        /// <value></value>
        public long LastUpdatedTime { get; set; }
        /// <summary>
        /// Timestamp this node was soft deleted. This is only present in sync results.
        /// </summary>
        public long? DeletedTime { get; set; }
    }

    /// <summary>
    /// Node with a specific source, to be used with `BaseDataModelResource`.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class SourcedNode<T> : SourcedInstance<T>
    {
        /// <summary>
        /// Empty constructor.
        /// </summary>
        public SourcedNode()
        {
        }
        /// <summary>
        /// Constructor.
        /// </summary>
        public SourcedNode(SlimInstance instance, T properties)
        {
            Version = instance.Version;
            CreatedTime = instance.CreatedTime;
            LastUpdatedTime = instance.LastUpdatedTime;
            Space = instance.Space;
            ExternalId = instance.ExternalId;
            Properties = properties;
        }
        /// <summary>
        /// Constructor.
        /// </summary>
        public SourcedNode(SourcedInstance<T> instance)
        {
            Version = instance.Version;
            CreatedTime = instance.CreatedTime;
            LastUpdatedTime = instance.LastUpdatedTime;
            Space = instance.Space;
            ExternalId = instance.ExternalId;
            Properties = instance.Properties;
        }
    }

    /// <summary>
    /// Edge with a specific source, to be used with `BaseDataModelResource`.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class SourcedEdge<T> : SourcedInstance<T>
    {
        /// <summary>
        /// Start node of this edge.
        /// </summary>
        public DirectRelationIdentifier StartNode { get; set; }
        /// <summary>
        /// End node of this edge.
        /// </summary>
        public DirectRelationIdentifier EndNode { get; set; }
    }

    /// <summary>
    /// Simplified filter for retrieving instances with specified source.
    /// </summary>
    public class SourcedInstanceFilter : CursorQueryBase
    {
        /// <summary>
        /// The type of instance to query for, edges or nodes.
        /// </summary>
        /// <value></value>
        public InstanceType InstanceType { get; set; }
        /// <summary>
        /// Sort the results
        /// </summary>
        public IEnumerable<InstanceSort> Sort { get; set; }
        /// <summary>
        /// Filter the results.
        /// </summary>
        public IDMSFilter Filter { get; set; }
    }

    /// <summary>
    /// Simplified search filter for retrieving instances with specified source.
    /// </summary>
    public class SourcedInstanceSearch
    {
        /// <summary>
        /// The type of instance to query for, edges or nodes.
        /// </summary>
        /// <value></value>
        public InstanceType InstanceType { get; set; }
        /// <summary>
        /// Sort the results
        /// </summary>
        public IEnumerable<InstanceSort> Sort { get; set; }
        /// <summary>
        /// Filter the results.
        /// </summary>
        public IDMSFilter Filter { get; set; }
        /// <summary>
        /// Maximum number of instances to retrieve.
        /// </summary>
        public int Limit { get; set; }
        /// <summary>
        /// Free text search query.
        /// </summary>
        public string Query { get; set; }
        /// <summary>
        /// Optional array of properties you want to search through.
        /// If you do not specify one or more properties, the service
        /// will search all text fields within the view.
        /// </summary>
        /// <value></value>
        public IEnumerable<string> Properties { get; set; }
    }
}
namespace CogniteSdk.Relationships
{
    /// <summary>
    /// Resource identifier for relationship
    /// </summary>
    public class RelationshipResource
    {
        /// <summary>
        /// The type of resource, Enum: "asset" "timeSeries" "file" "threeD" "threeDRevision" "event" "sequence".
        /// </summary>
        public string Resource { get; set; }
        /// <summary>
        /// ExternalId of the resource.
        /// </summary>
        /// <value></value>
        public string ResourceId { get; set; }
    }
}
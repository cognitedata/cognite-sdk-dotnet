namespace CogniteSdk.DataPoints
{
    /// <summary>
    /// Data Points abstract write type. Either DataPointsDeleteById or DataPointsDeleteByExternalId
    /// </summary>
    public abstract class DataPointsWriteType
    {
        /// <summary>
        /// The list of datapoints. The limit per request is 100000 datapoints.
        /// </summary>
        public long DataPoints { get; set; }
    }

    /// <summary>
    /// Data Points abstract write by Id DTO
    /// </summary>
    public abstract class DataPointsWriteByIdDto
    {
        /// <summary>
        /// A server-generated ID for the object.
        /// </summary>
        public Identity Id { get; set; }
    }

    /// <summary>
    /// Data Points abstract write by External Id DTO
    /// </summary>
    public abstract class DataPointsWriteByExternalIdDto
    {
        /// <summary>
        /// The external ID provided by the client. Must be unique for the resource type.
        /// </summary>
        public Identity ExternalId { get; set; }
    }
}
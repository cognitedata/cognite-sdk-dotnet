using System.Collections.Generic;
using CogniteSdk;

namespace CogniteSdk.Sequences
{
    /// <summary>
    /// The sequence update items DTO base class.
    /// </summary>
    public abstract class SequenceUpdateItemsType
    {
        /// <summary>
        /// A description of changes that should be done to the sequence.
        /// </summary>
        public SequenceUpdateDto Update { get; set; }
    }

    /// <summary>
    /// The sequence update items DTO.
    /// </summary>
    public abstract class SequenceUpdateItemsByIdDto : SequenceUpdateItemsType
    {
        /// <summary>
        /// A server-generated ID for the object.
        /// </summary>
        public long Id { get; set; }
    }

    /// <summary>
    /// The sequence update items DTO.
    /// </summary>
    public abstract class SequenceUpdateItemsByExternalIdDto : SequenceUpdateItemsType
    {
        /// <summary>
        /// The external ID provided by the client. Must be unique for the resource type.
        /// </summary>
        public string ExternalId { get; set; }
    }

}
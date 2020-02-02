using CogniteSdk.Types.Common;

namespace CogniteSdk.Sequences
{
    /// <summary>
    /// The sequence update items DTO base class.
    /// </summary>
    public abstract class SequenceUpdateItemsDto : Identity
    {
        /// <summary>
        /// Create seqence update item using external Id.
        /// </summary>
        /// <param name="externalId">External Id to set.</param>
        public SequenceUpdateItemsDto(string externalId) : base(externalId)
        {
        }

        /// <summary>
        /// Create sequence update item using internal Id.
        /// </summary>
        /// <param name="internalId">Internal Id to set.</param>
        public SequenceUpdateItemsDto(long internalId) : base(internalId)
        {
        }

        /// <summary>
        /// A description of changes that should be done to the sequence.
        /// </summary>
        public SequenceUpdateDto Update { get; set; }

        /// <inheritdoc />
        public override string ToString() => Stringable.ToString<SequenceUpdateItemsDto>(this);
    }
}
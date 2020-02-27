namespace CogniteSdk.Sequences
{
    /// <summary>
    /// The sequence update items DTO base class.
    /// </summary>
    public class SequenceUpdateItem : UpdateItem<SequenceUpdate>
    {
        /// <summary>
        /// Initialize the sequence update item with an external Id.
        /// </summary>
        /// <param name="externalId">External Id to set.</param>
        public SequenceUpdateItem(string externalId) : base(externalId)
        {
        }

        /// <summary>
        /// Initialize the sequence update item with an internal Id.
        /// </summary>
        /// <param name="id">Internal Id to set.</param>
        public SequenceUpdateItem(long id) : base(id)
        {
        }
    }
}
using CogniteSdk.Types.Common;

namespace CogniteSdk.Relationships
{
    /// <summary>
    /// Query parameters for list relationships.
    /// </summary>
    public class RelationshipQueryDto : CursorQueryBase
    {
        /// <summary>
        /// Filter on relationships.
        /// </summary>
        public RelationshipFilterDto Filter { get; set; }
        /// <inheritdoc />
        public override string ToString() => Stringable.ToString<RelationshipQueryDto>(this);
    }
}
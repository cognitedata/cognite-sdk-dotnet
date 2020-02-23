namespace CogniteSdk.Relationships
{
    /// <summary>
    /// Dto to perform a graph query.
    /// </summary>
    public class RestrictedGraphQueryDto
    {
        /// <summary>
        /// Executable graph query, written in gremlin.
        /// </summary>
        public string Query { get; set; }
        /// <summary>
        /// Filter on relationships.
        /// </summary>
        public RelationshipFilterDto Filter { get; set; }
    }
}
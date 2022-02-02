using System;
using System.Collections.Generic;
using System.Text;

namespace CogniteSdk.Types.Transformations.Configs
{
    /// <summary>
    /// Object for retrieving transformations.
    /// </summary>
    public class TransformationsRetrieve : ItemsWithIgnoreUnknownIds<Identity>
    {
        /// <summary>
        /// Whether the transformations will be returned with last running and last created job details.
        /// </summary>
        public bool WithJobDetails { get; set; }
    }
}

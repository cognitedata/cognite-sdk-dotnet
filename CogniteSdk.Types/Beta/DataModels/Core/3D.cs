using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CogniteSdk.Beta.DataModels.Core
{
    /// <summary>
    /// Type representing a 3D object.
    /// </summary>
    public class Object3D : CoreInstanceBase, IObject3D
    {
    }

    /// <summary>
    /// This concept represents the link from an Object3D (such as asset or equipment)
    /// to a specific 3D model (represented by the Model3D concept).
    /// It is only applicable to edges.
    /// </summary>
    public class Connection3D
    {
        /// <summary>
        /// Model revision.
        /// </summary>
        public long RevisionId { get; set; }
        /// <summary>
        /// Revision of node within 3D model.
        /// </summary>
        public long RevisionNodeId { get; set; }
    }

    /// <summary>
    /// Representation of a 3D model in the core data model.
    /// </summary>
    public class Model3D : CoreInstanceBase
    {
    }
}
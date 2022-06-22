// Copyright 2022 Cognite AS
// SPDX-License-Identifier: Apache-2.0
using System.Collections.Generic;

namespace CogniteSdk
{

    /// <summary>
    /// A bounding volume represents a region in a point cloud
    /// </summary>
    public class BoundingVolume: ObjectMetadataMixin
    {
        /// <summary>
        /// The region of the annotation defined by a list of geometry primitives (cylinder and box).
        /// </summary>
        public List<Geometry> Region {get; set;}
        /// <summary>
        /// The asset this annotation is pointing to
        /// </summary>
        public AssetRef AssetRef{get; set;}
    }
    /// <summary>
    /// A 3D geometry model represented by exactly one of `cylinder` and `box`.
    /// </summary>
    public class Geometry
    {
        /// <summary>
        /// A box in 3D space, defined by a 4x4 row-major homogeneous transformation matrix that rotates and
        /// translates a unit box centered at the origin to it's location and orientation in 3D space.
        /// </summary>
        public Box Box {get; set;}
        /// <summary>
        /// A cylinder in 3D space, defined by the centers of two sides and the radius.
        /// </summary>
        public Cylinder Cylinder {get; set;}
    }
    /// <summary>
    /// A box in 3D space, defined by a 4x4 row-major homogeneous transformation matrix that rotates and
    /// translates a unit box centered at the origin to it's location and orientation in 3D space.
    /// </summary>
    public class Box: ObjectMetadataMixin {
        /// <summary>
        /// The homogeneous transformation matrix
        /// </summary>
        public double[] Matrix {get; set;}
    }
    /// <summary>
    /// A cylinder in 3D space, defined by the centers of two sides and the radius.
    /// </summary>
    public class Cylinder: ObjectMetadataMixin {
        /// <summary>
        /// The center of the first cap.
        /// </summary>
        public double[] CenterA {get; set;}
        /// <summary>
        /// The center of the second cap.
        /// </summary>
        public double[] CenterB {get; set;}
        /// <summary>
        /// The radius of the cylinder.
        /// </summary>  
        public double Radius {get; set;}
    }
    /// <summary>
    /// The metadata of a geometric object
    /// </summary>  
    public abstract class ObjectMetadataMixin {
        /// <summary>
        /// Mixin that can be used to add confidence score to a thing
        /// </summary> 
        public double Confidence {get; set;}
        /// <summary>
        /// "Mixin that can be used to add a label string to a thing
        /// </summary> 
        public string Label {get; set;}   
    }
    /// <summary>
    /// A reference to an asset.
    /// </summary> 
    public class AssetRef {
        /// <summary>
        /// The id of the asset
        /// </summary> 
        public long Id {get; set;} 
    }
}

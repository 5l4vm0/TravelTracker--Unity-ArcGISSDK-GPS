// COPYRIGHT 1995-2023 ESRI
// TRADE SECRETS: ESRI PROPRIETARY AND CONFIDENTIAL
// Unpublished material - all rights reserved under the
// Copyright Laws of the United States and applicable international
// laws, treaties, and conventions.
//
// For additional information, contact:
// Attn: Contracts and Legal Department
// Environmental Systems Research Institute, Inc.
// 380 New York Street
// Redlands, California 92373
// USA
//
// email: legal@esri.com
namespace Esri.GameEngine.RCQ
{
    /// <summary>
    /// Texture type.
    /// </summary>
    public enum ArcGISTextureSemantic
    {
        /// <summary>
        /// The mesh imagery (color) texture.
        /// </summary>
        /// <remarks>
        /// May be a texture atlas.
        /// </remarks>
        MeshPyramidImagery = 5,
        
        /// <summary>
        /// The feature index vertex attribute to UVRegions lookup table.
        /// </summary>
        /// <remarks>
        /// Used when the imagery texture is a texture atlas.
        /// </remarks>
        MeshPyramidUvRegions = 6,
        
        /// <summary>
        /// The feature index vertex attribute to feature id lookup table.
        /// </summary>
        MeshPyramidFeatureIds = 7,
        
        /// <summary>
        /// The feature index vertex attribute to feature attribute value lookup table.
        /// </summary>
        MeshPyramidAttributeValues = 8
    };
}
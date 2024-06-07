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
using System.Runtime.InteropServices;
using System;

namespace Esri.GameEngine.Geometry
{
    /// <summary>
    /// The polygon builder object is used to create a polygon.
    /// </summary>
    /// <since>1.0.0</since>
    [StructLayout(LayoutKind.Sequential)]
    public partial class ArcGISPolygonBuilder :
        ArcGISMultipartBuilder
    {
        #region Constructors
        /// <summary>
        /// Creates a polygon builder from a polygon.
        /// </summary>
        /// <remarks>
        /// Prior to v100.12, only polygons without curves could be used; passing in a polygon where
        /// <see cref="GameEngine.Geometry.ArcGISGeometry.HasCurves">ArcGISGeometry.HasCurves</see> is true would throw an exception.
        /// 
        /// From v100.12, polygons with curves are supported.
        /// </remarks>
        /// <param name="polygon">A polygon object.</param>
        /// <seealso cref="GameEngine.Geometry.ArcGISGeometryBuilder.HasCurves">ArcGISGeometryBuilder.HasCurves</seealso>
        /// <since>1.0.0</since>
        public ArcGISPolygonBuilder(ArcGISPolygon polygon) :
            base(IntPtr.Zero)
        {
            var errorHandler = Unity.ArcGISErrorManager.CreateHandler();
            
            var localPolygon = polygon == null ? System.IntPtr.Zero : polygon.Handle;
            
            Handle = PInvoke.RT_PolygonBuilder_createFromPolygon(localPolygon, errorHandler);
            
            Unity.ArcGISErrorManager.CheckError(errorHandler);
        }
        
        /// <summary>
        /// Creates a polygon builder.
        /// </summary>
        /// <param name="spatialReference">The builder's spatial reference.</param>
        /// <since>1.0.0</since>
        public ArcGISPolygonBuilder(ArcGISSpatialReference spatialReference) :
            base(IntPtr.Zero)
        {
            var errorHandler = Unity.ArcGISErrorManager.CreateHandler();
            
            var localSpatialReference = spatialReference == null ? System.IntPtr.Zero : spatialReference.Handle;
            
            Handle = PInvoke.RT_PolygonBuilder_createFromSpatialReference(localSpatialReference, errorHandler);
            
            Unity.ArcGISErrorManager.CheckError(errorHandler);
        }
        #endregion // Constructors
        
        #region Methods
        /// <summary>
        /// Creates a polyline with the values in the polygon builder.
        /// </summary>
        /// <remarks>
        /// Creates the polyline object described by this builder.
        /// </remarks>
        /// <returns>
        /// A polyline.
        /// </returns>
        /// <since>1.0.0</since>
        public ArcGISPolyline ToPolyline()
        {
            var errorHandler = Unity.ArcGISErrorManager.CreateHandler();
            
            var localResult = PInvoke.RT_PolygonBuilder_toPolyline(Handle, errorHandler);
            
            Unity.ArcGISErrorManager.CheckError(errorHandler);
            
            ArcGISPolyline localLocalResult = null;
            
            if (localResult != IntPtr.Zero)
            {
                localLocalResult = new ArcGISPolyline(localResult);
            }
            
            return localLocalResult;
        }
        #endregion // Methods
        
        #region Internal Members
        internal ArcGISPolygonBuilder(IntPtr handle) : base(handle)
        {
        }
        #endregion // Internal Members
    }
    
    internal static partial class PInvoke
    {
        #region P-Invoke Declarations
        [DllImport(Unity.Interop.Dll)]
        internal static extern IntPtr RT_PolygonBuilder_createFromPolygon(IntPtr polygon, IntPtr errorHandler);
        
        [DllImport(Unity.Interop.Dll)]
        internal static extern IntPtr RT_PolygonBuilder_createFromSpatialReference(IntPtr spatialReference, IntPtr errorHandler);
        
        [DllImport(Unity.Interop.Dll)]
        internal static extern IntPtr RT_PolygonBuilder_toPolyline(IntPtr handle, IntPtr errorHandler);
        #endregion // P-Invoke Declarations
    }
}
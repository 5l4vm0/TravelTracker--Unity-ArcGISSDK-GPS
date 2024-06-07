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
    /// The polyline builder object is used to create a polyline.
    /// </summary>
    /// <since>1.0.0</since>
    [StructLayout(LayoutKind.Sequential)]
    public partial class ArcGISPolylineBuilder :
        ArcGISMultipartBuilder
    {
        #region Constructors
        /// <summary>
        /// Creates a polyline builder from a polyline.
        /// </summary>
        /// <remarks>
        /// Prior to v100.12, only polylines without curves could be used; passing in a polygon where
        /// <see cref="GameEngine.Geometry.ArcGISGeometry.HasCurves">ArcGISGeometry.HasCurves</see> is true would throw an exception.
        /// 
        /// From v100.12, polylines with curves are supported.
        /// </remarks>
        /// <param name="polyline">A polyline object.</param>
        /// <seealso cref="GameEngine.Geometry.ArcGISGeometryBuilder.HasCurves">ArcGISGeometryBuilder.HasCurves</seealso>
        /// <since>1.0.0</since>
        public ArcGISPolylineBuilder(ArcGISPolyline polyline) :
            base(IntPtr.Zero)
        {
            var errorHandler = Unity.ArcGISErrorManager.CreateHandler();
            
            var localPolyline = polyline == null ? System.IntPtr.Zero : polyline.Handle;
            
            Handle = PInvoke.RT_PolylineBuilder_createFromPolyline(localPolyline, errorHandler);
            
            Unity.ArcGISErrorManager.CheckError(errorHandler);
        }
        
        /// <summary>
        /// Creates a polyline builder.
        /// </summary>
        /// <param name="spatialReference">The builder's spatial reference.</param>
        /// <since>1.0.0</since>
        public ArcGISPolylineBuilder(ArcGISSpatialReference spatialReference) :
            base(IntPtr.Zero)
        {
            var errorHandler = Unity.ArcGISErrorManager.CreateHandler();
            
            var localSpatialReference = spatialReference == null ? System.IntPtr.Zero : spatialReference.Handle;
            
            Handle = PInvoke.RT_PolylineBuilder_createFromSpatialReference(localSpatialReference, errorHandler);
            
            Unity.ArcGISErrorManager.CheckError(errorHandler);
        }
        #endregion // Constructors
        
        #region Internal Members
        internal ArcGISPolylineBuilder(IntPtr handle) : base(handle)
        {
        }
        #endregion // Internal Members
    }
    
    internal static partial class PInvoke
    {
        #region P-Invoke Declarations
        [DllImport(Unity.Interop.Dll)]
        internal static extern IntPtr RT_PolylineBuilder_createFromPolyline(IntPtr polyline, IntPtr errorHandler);
        
        [DllImport(Unity.Interop.Dll)]
        internal static extern IntPtr RT_PolylineBuilder_createFromSpatialReference(IntPtr spatialReference, IntPtr errorHandler);
        #endregion // P-Invoke Declarations
    }
}
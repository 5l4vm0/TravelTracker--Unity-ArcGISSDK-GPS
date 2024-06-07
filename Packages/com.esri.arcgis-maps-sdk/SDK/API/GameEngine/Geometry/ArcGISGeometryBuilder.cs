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
    /// A geometry builder object.
    /// </summary>
    /// <seealso cref="">GeometryEditor</seealso>
    /// <since>1.0.0</since>
    [StructLayout(LayoutKind.Sequential)]
    public partial class ArcGISGeometryBuilder
    {
        #region Properties
        /// <summary>
        /// The extent for the geometry in the builder.
        /// </summary>
        /// <remarks>
        /// The extent for the geometry in the builder which is an envelope and contains the same spatial reference
        /// as the input geometry.
        /// </remarks>
        /// <since>1.0.0</since>
        public ArcGISEnvelope Extent
        {
            get
            {
                var errorHandler = Unity.ArcGISErrorManager.CreateHandler();
                
                var localResult = PInvoke.RT_GeometryBuilder_getExtent(Handle, errorHandler);
                
                Unity.ArcGISErrorManager.CheckError(errorHandler);
                
                ArcGISEnvelope localLocalResult = null;
                
                if (localResult != IntPtr.Zero)
                {
                    localLocalResult = new ArcGISEnvelope(localResult);
                }
                
                return localLocalResult;
            }
        }
        
        /// <summary>
        /// A value indicating whether the geometry builder currently contains any curve segments.
        /// </summary>
        /// <remarks>
        /// ArcGIS software supports polygon and polyline geometries that contain curve segments (where <see cref="GameEngine.Geometry.ArcGISSegment.IsCurve">ArcGISSegment.IsCurve</see> is true, sometimes known as
        /// true curves or nonlinear segments). Curves may be present in certain types of data - for example Mobile Map
        /// Packages (MMPKs), or geometry JSON.
        /// 
        /// Prior to v100.12, only <see cref="GameEngine.Geometry.ArcGISLineSegment">ArcGISLineSegment</see> instances were supported when creating new geometries using a
        /// <see cref="GameEngine.Geometry.ArcGISMultipartBuilder">ArcGISMultipartBuilder</see>. Attempting to add curve geometries to a <see cref="GameEngine.Geometry.ArcGISMultipartBuilder">ArcGISMultipartBuilder</see> would cause an error.
        /// 
        /// From v100.12, you can use curves in a <see cref="GameEngine.Geometry.ArcGISMultipartBuilder">ArcGISMultipartBuilder</see>. New segment types <see cref="GameEngine.Geometry.ArcGISCubicBezierSegment">ArcGISCubicBezierSegment</see> and
        /// <see cref="GameEngine.Geometry.ArcGISEllipticArcSegment">ArcGISEllipticArcSegment</see> represent different types of curve that can be added to polygon and polyline
        /// geometries.
        /// </remarks>
        /// <seealso cref="GameEngine.Geometry.ArcGISMutablePart.HasCurves">ArcGISMutablePart.HasCurves</seealso>
        /// <seealso cref="GameEngine.Geometry.ArcGISGeometry.HasCurves">ArcGISGeometry.HasCurves</seealso>
        /// <seealso cref="GameEngine.Geometry.ArcGISSegment.IsCurve">ArcGISSegment.IsCurve</seealso>
        /// <since>1.0.0</since>
        public bool HasCurves
        {
            get
            {
                var errorHandler = Unity.ArcGISErrorManager.CreateHandler();
                
                var localResult = PInvoke.RT_GeometryBuilder_getHasCurves(Handle, errorHandler);
                
                Unity.ArcGISErrorManager.CheckError(errorHandler);
                
                return localResult;
            }
        }
        
        /// <summary>
        /// A value indicating if the geometry builder has M.
        /// </summary>
        /// <remarks>
        /// If an error occurs false is returned.
        /// M is a vertex value that is stored with the geometry builder.
        /// </remarks>
        /// <since>1.0.0</since>
        public bool HasM
        {
            get
            {
                var errorHandler = Unity.ArcGISErrorManager.CreateHandler();
                
                var localResult = PInvoke.RT_GeometryBuilder_getHasM(Handle, errorHandler);
                
                Unity.ArcGISErrorManager.CheckError(errorHandler);
                
                return localResult;
            }
        }
        
        /// <summary>
        /// A value indicating if the geometry builder has Z.
        /// </summary>
        /// <remarks>
        /// If an error occurs false is returned.
        /// Z typically represent elevations or heights.
        /// </remarks>
        /// <since>1.0.0</since>
        public bool HasZ
        {
            get
            {
                var errorHandler = Unity.ArcGISErrorManager.CreateHandler();
                
                var localResult = PInvoke.RT_GeometryBuilder_getHasZ(Handle, errorHandler);
                
                Unity.ArcGISErrorManager.CheckError(errorHandler);
                
                return localResult;
            }
        }
        
        /// <summary>
        /// Check if a geometry builder is empty or not.
        /// </summary>
        /// <remarks>
        /// Only check the geometry builder to see if it is empty. Does not check the spatial reference. Returns true if an error occurs.
        /// </remarks>
        /// <since>1.0.0</since>
        public bool IsEmpty
        {
            get
            {
                var errorHandler = Unity.ArcGISErrorManager.CreateHandler();
                
                var localResult = PInvoke.RT_GeometryBuilder_getIsEmpty(Handle, errorHandler);
                
                Unity.ArcGISErrorManager.CheckError(errorHandler);
                
                return localResult;
            }
        }
        
        /// <summary>
        /// Check if a geometry builder contains sufficient points to show a valid graphical sketch.
        /// </summary>
        /// <remarks>
        /// This can be used as an initial lightweight check to see if the current state of a builder produces a
        /// non-empty geometry; for example, it may be used to enable or disable functionality in an editing user
        /// interface. The exact requirements vary depending on the type of geometry produced by the builder:
        /// * A <see cref="GameEngine.Geometry.ArcGISPointBuilder">ArcGISPointBuilder</see> must contain non-NaN x,y coordinates
        /// * A <see cref="GameEngine.Geometry.ArcGISMultipointBuilder">ArcGISMultipointBuilder</see> must contain at least one valid Point
        /// * An <see cref="GameEngine.Geometry.ArcGISEnvelopeBuilder">ArcGISEnvelopeBuilder</see> must contain non-NaN minimum and maximum x,y coordinates
        /// * A <see cref="GameEngine.Geometry.ArcGISPolylineBuilder">ArcGISPolylineBuilder</see> must contain at least one <see cref="GameEngine.Geometry.ArcGISMutablePart">ArcGISMutablePart</see>. Each <see cref="GameEngine.Geometry.ArcGISMutablePart">ArcGISMutablePart</see> it contains must have:
        ///   * At least two valid points, or
        ///   * At least one <see cref="GameEngine.Geometry.ArcGISSegment">ArcGISSegment</see> where <see cref="GameEngine.Geometry.ArcGISSegment.IsCurve">ArcGISSegment.IsCurve</see> is true
        /// * A <see cref="GameEngine.Geometry.ArcGISPolygonBuilder">ArcGISPolygonBuilder</see> must contain at least one <see cref="GameEngine.Geometry.ArcGISMutablePart">ArcGISMutablePart</see>. Each <see cref="GameEngine.Geometry.ArcGISMutablePart">ArcGISMutablePart</see> it contains must have:
        ///   * At least three valid points, or
        ///   * At least one <see cref="GameEngine.Geometry.ArcGISSegment">ArcGISSegment</see> where <see cref="GameEngine.Geometry.ArcGISSegment.IsCurve">ArcGISSegment.IsCurve</see> is true
        /// 
        /// Note that this is not equivalent to topological simplicity, which is enforced by <see cref="GameEngine.Geometry.ArcGISGeometryEngine.Simplify">ArcGISGeometryEngine.Simplify</see>
        /// and checked using <see cref="GameEngine.Geometry.ArcGISGeometryEngine.IsSimple">ArcGISGeometryEngine.IsSimple</see>. Geometries must be topologically simple to be
        /// successfully saved in a geodatabase or used in some service operations.
        /// 
        /// Does not check the spatial reference. Returns false if an error occurs.
        /// 
        /// Prior to v100.8, only one part of a multipart polygon or polyline was required to have the minimum number
        /// (2 for a polyline, 3 for a polygon) of points, and only <see cref="GameEngine.Geometry.ArcGISLineSegment">ArcGISLineSegment</see> instances were supported in
        /// builders.
        /// 
        /// Prior to v100.12, a <see cref="GameEngine.Geometry.ArcGISPolygonBuilder">ArcGISPolygonBuilder</see> required at least three valid Points in each <see cref="GameEngine.Geometry.ArcGISMutablePart">ArcGISMutablePart</see>, and at
        /// least one part.
        /// </remarks>
        /// <since>1.0.0</since>
        public bool IsSketchValid
        {
            get
            {
                var errorHandler = Unity.ArcGISErrorManager.CreateHandler();
                
                var localResult = PInvoke.RT_GeometryBuilder_getIsSketchValid(Handle, errorHandler);
                
                Unity.ArcGISErrorManager.CheckError(errorHandler);
                
                return localResult;
            }
        }
        
        /// <summary>
        /// The type of geometry builder.
        /// </summary>
        /// <remarks>
        /// The geometry builder type for a specific geometry builder. Returns <see cref="GameEngine.Geometry.ArcGISGeometryBuilderType.Unknown">ArcGISGeometryBuilderType.Unknown</see> if an error occurs.
        /// </remarks>
        internal ArcGISGeometryBuilderType ObjectType
        {
            get
            {
                var errorHandler = Unity.ArcGISErrorManager.CreateHandler();
                
                var localResult = PInvoke.RT_GeometryBuilder_getObjectType(Handle, errorHandler);
                
                Unity.ArcGISErrorManager.CheckError(errorHandler);
                
                return localResult;
            }
        }
        
        /// <summary>
        /// The spatial reference for the geometry.
        /// </summary>
        /// <remarks>
        /// If the geometry does not have a spatial reference null is returned.
        /// </remarks>
        /// <since>1.0.0</since>
        public ArcGISSpatialReference SpatialReference
        {
            get
            {
                var errorHandler = Unity.ArcGISErrorManager.CreateHandler();
                
                var localResult = PInvoke.RT_GeometryBuilder_getSpatialReference(Handle, errorHandler);
                
                Unity.ArcGISErrorManager.CheckError(errorHandler);
                
                ArcGISSpatialReference localLocalResult = null;
                
                if (localResult != IntPtr.Zero)
                {
                    localLocalResult = new ArcGISSpatialReference(localResult);
                }
                
                return localLocalResult;
            }
        }
        #endregion // Properties
        
        #region Methods
        /// <summary>
        /// Creates a geometry builder using the geometry provided as a starting
        /// point for further modifications.
        /// </summary>
        /// <remarks>
        /// Prior to v100.12, only geometries without curves could be used; passing in a geometry where
        /// <see cref="GameEngine.Geometry.ArcGISGeometry.HasCurves">ArcGISGeometry.HasCurves</see> is true would throw an exception.
        /// 
        /// From v100.12, geometries with curves are supported.
        /// </remarks>
        /// <param name="geometry">The geometry to use as the starting point for further modifications.</param>
        /// <returns>
        /// A new geometry builder.
        /// </returns>
        /// <seealso cref="GameEngine.Geometry.ArcGISGeometryBuilder.HasCurves">ArcGISGeometryBuilder.HasCurves</seealso>
        /// <since>1.0.0</since>
        public static ArcGISGeometryBuilder Create(ArcGISGeometry geometry)
        {
            var errorHandler = Unity.ArcGISErrorManager.CreateHandler();
            
            var localGeometry = geometry.Handle;
            
            var localResult = PInvoke.RT_GeometryBuilder_createFromGeometry(localGeometry, errorHandler);
            
            Unity.ArcGISErrorManager.CheckError(errorHandler);
            
            ArcGISGeometryBuilder localLocalResult = null;
            
            if (localResult != IntPtr.Zero)
            {
                var objectType = GameEngine.Geometry.PInvoke.RT_GeometryBuilder_getObjectType(localResult, IntPtr.Zero);
                
                switch (objectType)
                {
                    case GameEngine.Geometry.ArcGISGeometryBuilderType.EnvelopeBuilder:
                        localLocalResult = new ArcGISEnvelopeBuilder(localResult);
                        break;
                    case GameEngine.Geometry.ArcGISGeometryBuilderType.MultipointBuilder:
                        localLocalResult = new ArcGISMultipointBuilder(localResult);
                        break;
                    case GameEngine.Geometry.ArcGISGeometryBuilderType.PointBuilder:
                        localLocalResult = new ArcGISPointBuilder(localResult);
                        break;
                    case GameEngine.Geometry.ArcGISGeometryBuilderType.PolygonBuilder:
                        localLocalResult = new ArcGISPolygonBuilder(localResult);
                        break;
                    case GameEngine.Geometry.ArcGISGeometryBuilderType.PolylineBuilder:
                        localLocalResult = new ArcGISPolylineBuilder(localResult);
                        break;
                    default:
                        localLocalResult = new ArcGISGeometryBuilder(localResult);
                        break;
                }
            }
            
            return localLocalResult;
        }
        
        /// <summary>
        /// Creates a geometry builder which builds geometries of the given
        /// type.
        /// </summary>
        /// <param name="geometryType">The builder's geometry type.</param>
        /// <param name="spatialReference">The builder's spatial reference.</param>
        /// <returns>
        /// A new geometry builder.
        /// </returns>
        /// <seealso cref="GameEngine.Geometry.ArcGISGeometryBuilder.HasCurves">ArcGISGeometryBuilder.HasCurves</seealso>
        /// <since>1.0.0</since>
        public static ArcGISGeometryBuilder Create(ArcGISGeometryType geometryType, ArcGISSpatialReference spatialReference)
        {
            var errorHandler = Unity.ArcGISErrorManager.CreateHandler();
            
            var localSpatialReference = spatialReference == null ? System.IntPtr.Zero : spatialReference.Handle;
            
            var localResult = PInvoke.RT_GeometryBuilder_createFromGeometryTypeAndSpatialReference(geometryType, localSpatialReference, errorHandler);
            
            Unity.ArcGISErrorManager.CheckError(errorHandler);
            
            ArcGISGeometryBuilder localLocalResult = null;
            
            if (localResult != IntPtr.Zero)
            {
                var objectType = GameEngine.Geometry.PInvoke.RT_GeometryBuilder_getObjectType(localResult, IntPtr.Zero);
                
                switch (objectType)
                {
                    case GameEngine.Geometry.ArcGISGeometryBuilderType.EnvelopeBuilder:
                        localLocalResult = new ArcGISEnvelopeBuilder(localResult);
                        break;
                    case GameEngine.Geometry.ArcGISGeometryBuilderType.MultipointBuilder:
                        localLocalResult = new ArcGISMultipointBuilder(localResult);
                        break;
                    case GameEngine.Geometry.ArcGISGeometryBuilderType.PointBuilder:
                        localLocalResult = new ArcGISPointBuilder(localResult);
                        break;
                    case GameEngine.Geometry.ArcGISGeometryBuilderType.PolygonBuilder:
                        localLocalResult = new ArcGISPolygonBuilder(localResult);
                        break;
                    case GameEngine.Geometry.ArcGISGeometryBuilderType.PolylineBuilder:
                        localLocalResult = new ArcGISPolylineBuilder(localResult);
                        break;
                    default:
                        localLocalResult = new ArcGISGeometryBuilder(localResult);
                        break;
                }
            }
            
            return localLocalResult;
        }
        
        /// <summary>
        /// Replaces the geometry in the builder with the new geometry.
        /// </summary>
        /// <remarks>
        /// This does not update the spatial reference
        /// of the builder. If the geometry is null, the builder is cleared.
        /// 
        /// Prior to v100.12, only geometries without curves could be used; passing in a geometry where
        /// <see cref="GameEngine.Geometry.ArcGISGeometry.HasCurves">ArcGISGeometry.HasCurves</see> is true would throw an exception.
        /// 
        /// From v100.12, geometries with curves are supported.
        /// </remarks>
        /// <param name="geometry">The geometry object.</param>
        /// <since>1.0.0</since>
        public void ReplaceGeometry(ArcGISGeometry geometry)
        {
            var errorHandler = Unity.ArcGISErrorManager.CreateHandler();
            
            var localGeometry = geometry == null ? System.IntPtr.Zero : geometry.Handle;
            
            PInvoke.RT_GeometryBuilder_replaceGeometry(Handle, localGeometry, errorHandler);
            
            Unity.ArcGISErrorManager.CheckError(errorHandler);
        }
        
        /// <summary>
        /// Creates a geometry with the values in the geometry builder.
        /// </summary>
        /// <returns>
        /// A geometry. This is passed to geometry functions.
        /// </returns>
        /// <since>1.0.0</since>
        public ArcGISGeometry ToGeometry()
        {
            var errorHandler = Unity.ArcGISErrorManager.CreateHandler();
            
            var localResult = PInvoke.RT_GeometryBuilder_toGeometry(Handle, errorHandler);
            
            Unity.ArcGISErrorManager.CheckError(errorHandler);
            
            ArcGISGeometry localLocalResult = null;
            
            if (localResult != IntPtr.Zero)
            {
                var objectType = GameEngine.Geometry.PInvoke.RT_Geometry_getObjectType(localResult, IntPtr.Zero);
                
                switch (objectType)
                {
                    case GameEngine.Geometry.ArcGISGeometryType.Envelope:
                        localLocalResult = new ArcGISEnvelope(localResult);
                        break;
                    case GameEngine.Geometry.ArcGISGeometryType.Multipoint:
                        localLocalResult = new ArcGISMultipoint(localResult);
                        break;
                    case GameEngine.Geometry.ArcGISGeometryType.Point:
                        localLocalResult = new ArcGISPoint(localResult);
                        break;
                    case GameEngine.Geometry.ArcGISGeometryType.Polygon:
                        localLocalResult = new ArcGISPolygon(localResult);
                        break;
                    case GameEngine.Geometry.ArcGISGeometryType.Polyline:
                        localLocalResult = new ArcGISPolyline(localResult);
                        break;
                    default:
                        localLocalResult = new ArcGISGeometry(localResult);
                        break;
                }
            }
            
            return localLocalResult;
        }
        #endregion // Methods
        
        #region Internal Members
        internal ArcGISGeometryBuilder(IntPtr handle) => Handle = handle;
        
        ~ArcGISGeometryBuilder()
        {
            if (Handle != IntPtr.Zero)
            {
                var errorHandler = Unity.ArcGISErrorManager.CreateHandler();
                
                PInvoke.RT_GeometryBuilder_destroy(Handle, errorHandler);
                
                Unity.ArcGISErrorManager.CheckError(errorHandler);
            }
        }
        
        internal IntPtr Handle { get; set; }
        #endregion // Internal Members
    }
    
    internal static partial class PInvoke
    {
        #region P-Invoke Declarations
        [DllImport(Unity.Interop.Dll)]
        internal static extern IntPtr RT_GeometryBuilder_getExtent(IntPtr handle, IntPtr errorHandler);
        
        [DllImport(Unity.Interop.Dll)]
        [return: MarshalAs(UnmanagedType.I1)]
        internal static extern bool RT_GeometryBuilder_getHasCurves(IntPtr handle, IntPtr errorHandler);
        
        [DllImport(Unity.Interop.Dll)]
        [return: MarshalAs(UnmanagedType.I1)]
        internal static extern bool RT_GeometryBuilder_getHasM(IntPtr handle, IntPtr errorHandler);
        
        [DllImport(Unity.Interop.Dll)]
        [return: MarshalAs(UnmanagedType.I1)]
        internal static extern bool RT_GeometryBuilder_getHasZ(IntPtr handle, IntPtr errorHandler);
        
        [DllImport(Unity.Interop.Dll)]
        [return: MarshalAs(UnmanagedType.I1)]
        internal static extern bool RT_GeometryBuilder_getIsEmpty(IntPtr handle, IntPtr errorHandler);
        
        [DllImport(Unity.Interop.Dll)]
        [return: MarshalAs(UnmanagedType.I1)]
        internal static extern bool RT_GeometryBuilder_getIsSketchValid(IntPtr handle, IntPtr errorHandler);
        
        [DllImport(Unity.Interop.Dll)]
        internal static extern ArcGISGeometryBuilderType RT_GeometryBuilder_getObjectType(IntPtr handle, IntPtr errorHandler);
        
        [DllImport(Unity.Interop.Dll)]
        internal static extern IntPtr RT_GeometryBuilder_getSpatialReference(IntPtr handle, IntPtr errorHandler);
        
        [DllImport(Unity.Interop.Dll)]
        internal static extern IntPtr RT_GeometryBuilder_createFromGeometry(IntPtr geometry, IntPtr errorHandler);
        
        [DllImport(Unity.Interop.Dll)]
        internal static extern IntPtr RT_GeometryBuilder_createFromGeometryTypeAndSpatialReference(ArcGISGeometryType geometryType, IntPtr spatialReference, IntPtr errorHandler);
        
        [DllImport(Unity.Interop.Dll)]
        internal static extern void RT_GeometryBuilder_replaceGeometry(IntPtr handle, IntPtr geometry, IntPtr errorHandler);
        
        [DllImport(Unity.Interop.Dll)]
        internal static extern IntPtr RT_GeometryBuilder_toGeometry(IntPtr handle, IntPtr errorHandler);
        
        [DllImport(Unity.Interop.Dll)]
        internal static extern void RT_GeometryBuilder_destroy(IntPtr handle, IntPtr errorHandle);
        #endregion // P-Invoke Declarations
    }
}
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
    /// Represents a step in the process of transforming between horizontal and/or vertical datums.
    /// </summary>
    /// <remarks>
    /// Each <see cref="GameEngine.Geometry.ArcGISHorizontalVerticalTransformationStep">ArcGISHorizontalVerticalTransformationStep</see> can be constructed from a well-known ID (WKID) that represents a
    /// horizontal (geographic) or a vertical transformation. This API supports thousands of predefined
    /// transformations, and all the supported WKIDs are documented in the 'Coordinate systems and transformations'
    /// topic in the developers guide.
    /// 
    /// One or more <see cref="GameEngine.Geometry.ArcGISHorizontalVerticalTransformationStep">ArcGISHorizontalVerticalTransformationStep</see> objects are combined into a <see cref="GameEngine.Geometry.ArcGISHorizontalVerticalTransformation">ArcGISHorizontalVerticalTransformation</see> object, and
    /// can then be used to control how coordinates are transformed in a
    /// <see cref="GameEngine.Geometry.ArcGISGeometryEngine.Project">ArcGISGeometryEngine.Project</see> method call.
    /// 
    /// A <see cref="GameEngine.Geometry.ArcGISHorizontalVerticalTransformationStep">ArcGISHorizontalVerticalTransformationStep</see> object is immutable.
    /// </remarks>
    /// <seealso cref="GameEngine.Geometry.ArcGISHorizontalVerticalTransformation">ArcGISHorizontalVerticalTransformation</seealso>
    /// <seealso cref="GameEngine.Geometry.ArcGISGeographicTransformationStep">ArcGISGeographicTransformationStep</seealso>
    /// <since>1.0.0</since>
    [StructLayout(LayoutKind.Sequential)]
    public partial class ArcGISHorizontalVerticalTransformationStep
    {
        #region Constructors
        /// <summary>
        /// Creates a new <see cref="GameEngine.Geometry.ArcGISHorizontalVerticalTransformationStep">ArcGISHorizontalVerticalTransformationStep</see> instance from a well-known ID.
        /// </summary>
        /// <remarks>
        /// Occasionally, WKIDs may change, and older codes may be deprecated in favor of a new code. Both old
        /// (deprecated) and new (latest) WKIDs continue to work for instantiation, as long as they are supported
        /// by the Projection Engine. The <see cref="GameEngine.Geometry.ArcGISHorizontalVerticalTransformationStep.WKID">ArcGISHorizontalVerticalTransformationStep.WKID</see> property returns the new
        /// (latest) WKID code.
        /// </remarks>
        /// <param name="WKID">The well-known ID of the transformation step to create.</param>
        /// <since>1.0.0</since>
        public ArcGISHorizontalVerticalTransformationStep(int WKID)
        {
            var errorHandler = Unity.ArcGISErrorManager.CreateHandler();
            
            Handle = PInvoke.RT_HorizontalVerticalTransformationStep_createWithWKID(WKID, errorHandler);
            
            Unity.ArcGISErrorManager.CheckError(errorHandler);
        }
        
        /// <summary>
        /// Creates a new <see cref="GameEngine.Geometry.ArcGISHorizontalVerticalTransformationStep">ArcGISHorizontalVerticalTransformationStep</see> instance from a well-known text string.
        /// </summary>
        /// <param name="WKText">The well-known text of the transformation step to create.</param>
        /// <since>1.0.0</since>
        public ArcGISHorizontalVerticalTransformationStep(string WKText)
        {
            var errorHandler = Unity.ArcGISErrorManager.CreateHandler();
            
            Handle = PInvoke.RT_HorizontalVerticalTransformationStep_createWithWKText(WKText, errorHandler);
            
            Unity.ArcGISErrorManager.CheckError(errorHandler);
        }
        #endregion // Constructors
        
        #region Properties
        /// <summary>
        /// True if this transformation step instance is an inverted transformation.
        /// </summary>
        /// <remarks>
        /// Transformations have a specific direction that is indicated by the
        /// <see cref="GameEngine.Geometry.ArcGISHorizontalVerticalTransformationStep.WKText">ArcGISHorizontalVerticalTransformationStep.WKText</see> value. An inverted transformation is used to transform
        /// geometries in the opposite direction to that indicated by the well-known text.
        /// <see cref="GameEngine.Geometry.ArcGISHorizontalVerticalTransformationStep">ArcGISHorizontalVerticalTransformationStep</see> has <see cref="GameEngine.Geometry.ArcGISDatumTransformation.InputSpatialReference">ArcGISDatumTransformation.InputSpatialReference</see> and
        /// <see cref="GameEngine.Geometry.ArcGISDatumTransformation.OutputSpatialReference">ArcGISDatumTransformation.OutputSpatialReference</see> properties that respect the inverse value of the contained
        /// transformation(s).
        /// </remarks>
        /// <seealso cref="GameEngine.Geometry.ArcGISHorizontalVerticalTransformationStep.GetInverse">ArcGISHorizontalVerticalTransformationStep.GetInverse</seealso>
        /// <since>1.0.0</since>
        public bool IsInverse
        {
            get
            {
                var errorHandler = Unity.ArcGISErrorManager.CreateHandler();
                
                var localResult = PInvoke.RT_HorizontalVerticalTransformationStep_getIsInverse(Handle, errorHandler);
                
                Unity.ArcGISErrorManager.CheckError(errorHandler);
                
                return localResult;
            }
        }
        
        /// <summary>
        /// True if any files needed by the Projection Engine for this transformation step are missing from the local
        /// file system.
        /// </summary>
        /// <since>1.0.0</since>
        public bool IsMissingProjectionEngineFiles
        {
            get
            {
                var errorHandler = Unity.ArcGISErrorManager.CreateHandler();
                
                var localResult = PInvoke.RT_HorizontalVerticalTransformationStep_getIsMissingProjectionEngineFiles(Handle, errorHandler);
                
                Unity.ArcGISErrorManager.CheckError(errorHandler);
                
                return localResult;
            }
        }
        
        /// <summary>
        /// A list of the Projection Engine files required to support this transformation step.
        /// </summary>
        /// <remarks>
        /// Each name in the list includes the full path. Projection Engine datasets are used in grid-based transforms.
        /// </remarks>
        /// <since>1.0.0</since>
        public Unity.ArcGISImmutableArray<string> ProjectionEngineFilenames
        {
            get
            {
                var errorHandler = Unity.ArcGISErrorManager.CreateHandler();
                
                var localResult = PInvoke.RT_HorizontalVerticalTransformationStep_getProjectionEngineFilenames(Handle, errorHandler);
                
                Unity.ArcGISErrorManager.CheckError(errorHandler);
                
                Unity.ArcGISImmutableArray<string> localLocalResult = null;
                
                if (localResult != IntPtr.Zero)
                {
                    localLocalResult = new Unity.ArcGISImmutableArray<string>(localResult);
                }
                
                return localLocalResult;
            }
        }
        
        /// <summary>
        /// The well-known ID or 0 if the transformation in this step does not have a well-known ID.
        /// </summary>
        /// <remarks>
        /// Occasionally, WKIDs may change, and an older code may be deprecated in favor of a new code. This property
        /// returns the new (latest) WKID code.
        /// </remarks>
        /// <since>1.0.0</since>
        public int WKID
        {
            get
            {
                var errorHandler = Unity.ArcGISErrorManager.CreateHandler();
                
                var localResult = PInvoke.RT_HorizontalVerticalTransformationStep_getWKID(Handle, errorHandler);
                
                Unity.ArcGISErrorManager.CheckError(errorHandler);
                
                return localResult;
            }
        }
        
        /// <summary>
        /// The well-known text of this transformation step instance.
        /// </summary>
        /// <since>1.0.0</since>
        public string WKText
        {
            get
            {
                var errorHandler = Unity.ArcGISErrorManager.CreateHandler();
                
                var localResult = PInvoke.RT_HorizontalVerticalTransformationStep_getWKText(Handle, errorHandler);
                
                Unity.ArcGISErrorManager.CheckError(errorHandler);
                
                return Unity.Convert.FromArcGISString(localResult);
            }
        }
        #endregion // Properties
        
        #region Methods
        /// <summary>
        /// Tests if this object is equal to a second <see cref="GameEngine.Geometry.ArcGISHorizontalVerticalTransformationStep">ArcGISHorizontalVerticalTransformationStep</see> object.
        /// </summary>
        /// <param name="rightHorizontalVerticalTransformationStep">Another <see cref="GameEngine.Geometry.ArcGISHorizontalVerticalTransformationStep">ArcGISHorizontalVerticalTransformationStep</see>.</param>
        /// <returns>
        /// True if the comparison succeeds and the objects are equal, false otherwise.
        /// </returns>
        /// <since>1.0.0</since>
        public bool Equals(ArcGISHorizontalVerticalTransformationStep rightHorizontalVerticalTransformationStep)
        {
            var errorHandler = Unity.ArcGISErrorManager.CreateHandler();
            
            var localRightHorizontalVerticalTransformationStep = rightHorizontalVerticalTransformationStep.Handle;
            
            var localResult = PInvoke.RT_HorizontalVerticalTransformationStep_equals(Handle, localRightHorizontalVerticalTransformationStep, errorHandler);
            
            Unity.ArcGISErrorManager.CheckError(errorHandler);
            
            return localResult;
        }
        
        /// <summary>
        /// Returns the inverse of this transformation step or null if the transformation is not invertible.
        /// </summary>
        /// <returns>
        /// A <see cref="GameEngine.Geometry.ArcGISHorizontalVerticalTransformationStep">ArcGISHorizontalVerticalTransformationStep</see> representing the inverse or null if an inverse does not exist.
        /// </returns>
        /// <since>1.0.0</since>
        public ArcGISHorizontalVerticalTransformationStep GetInverse()
        {
            var errorHandler = Unity.ArcGISErrorManager.CreateHandler();
            
            var localResult = PInvoke.RT_HorizontalVerticalTransformationStep_getInverse(Handle, errorHandler);
            
            Unity.ArcGISErrorManager.CheckError(errorHandler);
            
            ArcGISHorizontalVerticalTransformationStep localLocalResult = null;
            
            if (localResult != IntPtr.Zero)
            {
                localLocalResult = new ArcGISHorizontalVerticalTransformationStep(localResult);
            }
            
            return localLocalResult;
        }
        #endregion // Methods
        
        #region Internal Members
        internal ArcGISHorizontalVerticalTransformationStep(IntPtr handle) => Handle = handle;
        
        ~ArcGISHorizontalVerticalTransformationStep()
        {
            if (Handle != IntPtr.Zero)
            {
                var errorHandler = Unity.ArcGISErrorManager.CreateHandler();
                
                PInvoke.RT_HorizontalVerticalTransformationStep_destroy(Handle, errorHandler);
                
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
        internal static extern IntPtr RT_HorizontalVerticalTransformationStep_createWithWKID(int WKID, IntPtr errorHandler);
        
        [DllImport(Unity.Interop.Dll)]
        internal static extern IntPtr RT_HorizontalVerticalTransformationStep_createWithWKText([MarshalAs(UnmanagedType.LPStr)]string WKText, IntPtr errorHandler);
        
        [DllImport(Unity.Interop.Dll)]
        [return: MarshalAs(UnmanagedType.I1)]
        internal static extern bool RT_HorizontalVerticalTransformationStep_getIsInverse(IntPtr handle, IntPtr errorHandler);
        
        [DllImport(Unity.Interop.Dll)]
        [return: MarshalAs(UnmanagedType.I1)]
        internal static extern bool RT_HorizontalVerticalTransformationStep_getIsMissingProjectionEngineFiles(IntPtr handle, IntPtr errorHandler);
        
        [DllImport(Unity.Interop.Dll)]
        internal static extern IntPtr RT_HorizontalVerticalTransformationStep_getProjectionEngineFilenames(IntPtr handle, IntPtr errorHandler);
        
        [DllImport(Unity.Interop.Dll)]
        internal static extern int RT_HorizontalVerticalTransformationStep_getWKID(IntPtr handle, IntPtr errorHandler);
        
        [DllImport(Unity.Interop.Dll)]
        internal static extern IntPtr RT_HorizontalVerticalTransformationStep_getWKText(IntPtr handle, IntPtr errorHandler);
        
        [DllImport(Unity.Interop.Dll)]
        [return: MarshalAs(UnmanagedType.I1)]
        internal static extern bool RT_HorizontalVerticalTransformationStep_equals(IntPtr handle, IntPtr rightHorizontalVerticalTransformationStep, IntPtr errorHandler);
        
        [DllImport(Unity.Interop.Dll)]
        internal static extern IntPtr RT_HorizontalVerticalTransformationStep_getInverse(IntPtr handle, IntPtr errorHandler);
        
        [DllImport(Unity.Interop.Dll)]
        internal static extern void RT_HorizontalVerticalTransformationStep_destroy(IntPtr handle, IntPtr errorHandle);
        #endregion // P-Invoke Declarations
    }
}
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

namespace Esri.GameEngine.Attributes
{
    /// <summary>
    /// Defines an immutable collection of 64 bit integers
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal partial class ArcGISInt64ImmutableCollection
    {
        #region Properties
        /// <summary>
        /// Determines the number of values in the vector.
        /// </summary>
        /// <remarks>
        /// The number of values in the vector. If an error occurs a 0 will be returned.
        /// </remarks>
        internal ulong Size
        {
            get
            {
                var errorHandler = Unity.ArcGISErrorManager.CreateHandler();
                
                var localResult = PInvoke.RT_int64ImmutableCollection_getSize(Handle, errorHandler);
                
                Unity.ArcGISErrorManager.CheckError(errorHandler);
                
                return localResult.ToUInt64();
            }
        }
        #endregion // Properties
        
        #region Methods
        /// <summary>
        /// Get a value at a specific position.
        /// </summary>
        /// <remarks>
        /// Retrieves the value at the specified position.
        /// </remarks>
        /// <param name="position">The position which you want to get the value.</param>
        /// <returns>
        /// The value, int64, at the position requested.
        /// </returns>
        internal long At(ulong position)
        {
            var errorHandler = Unity.ArcGISErrorManager.CreateHandler();
            
            var localPosition = new UIntPtr(position);
            
            var localResult = PInvoke.RT_int64ImmutableCollection_at(Handle, localPosition, errorHandler);
            
            Unity.ArcGISErrorManager.CheckError(errorHandler);
            
            return localResult;
        }
        
        /// <summary>
        /// Does the vector contain the given value.
        /// </summary>
        /// <remarks>
        /// Does the vector contain a specific value.
        /// </remarks>
        /// <param name="value">The value you want to find.</param>
        /// <returns>
        /// True if the value is in the vector otherwise false.
        /// </returns>
        internal bool Contains(long value)
        {
            var errorHandler = Unity.ArcGISErrorManager.CreateHandler();
            
            var localResult = PInvoke.RT_int64ImmutableCollection_contains(Handle, value, errorHandler);
            
            Unity.ArcGISErrorManager.CheckError(errorHandler);
            
            return localResult;
        }
        
        /// <summary>
        /// Returns true if the two vectors are equal, false otherwise.
        /// </summary>
        /// <param name="vector2">The second vector.</param>
        /// <returns>
        /// Returns true if the two vectors are equal, false otherwise.
        /// </returns>
        internal bool Equals(Unity.ArcGISImmutableCollection<long> vector2)
        {
            var errorHandler = Unity.ArcGISErrorManager.CreateHandler();
            
            var localVector2 = vector2.Handle;
            
            var localResult = PInvoke.RT_int64ImmutableCollection_equals(Handle, localVector2, errorHandler);
            
            Unity.ArcGISErrorManager.CheckError(errorHandler);
            
            return localResult;
        }
        
        /// <summary>
        /// Get the first value in the vector.
        /// </summary>
        /// <returns>
        /// The value, int64, at the position requested.
        /// </returns>
        internal long First()
        {
            var errorHandler = Unity.ArcGISErrorManager.CreateHandler();
            
            var localResult = PInvoke.RT_int64ImmutableCollection_first(Handle, errorHandler);
            
            Unity.ArcGISErrorManager.CheckError(errorHandler);
            
            return localResult;
        }
        
        /// <summary>
        /// Retrieves the position of the given value in the vector.
        /// </summary>
        /// <param name="value">The value you want to find.</param>
        /// <returns>
        /// The position of the value in the vector, Max value of size_t otherwise.
        /// </returns>
        internal ulong IndexOf(long value)
        {
            var errorHandler = Unity.ArcGISErrorManager.CreateHandler();
            
            var localResult = PInvoke.RT_int64ImmutableCollection_indexOf(Handle, value, errorHandler);
            
            Unity.ArcGISErrorManager.CheckError(errorHandler);
            
            return localResult.ToUInt64();
        }
        
        /// <summary>
        /// Determines if there are any values in the vector.
        /// </summary>
        /// <remarks>
        /// Check if the vector object has any values in it.
        /// </remarks>
        /// <returns>
        /// true if the vector object contains no values otherwise false. Will return true if an error occurs.
        /// </returns>
        internal bool IsEmpty()
        {
            var errorHandler = Unity.ArcGISErrorManager.CreateHandler();
            
            var localResult = PInvoke.RT_int64ImmutableCollection_isEmpty(Handle, errorHandler);
            
            Unity.ArcGISErrorManager.CheckError(errorHandler);
            
            return localResult;
        }
        
        /// <summary>
        /// Get the last value in the vector.
        /// </summary>
        /// <returns>
        /// The value, int64, at the position requested.
        /// </returns>
        internal long Last()
        {
            var errorHandler = Unity.ArcGISErrorManager.CreateHandler();
            
            var localResult = PInvoke.RT_int64ImmutableCollection_last(Handle, errorHandler);
            
            Unity.ArcGISErrorManager.CheckError(errorHandler);
            
            return localResult;
        }
        
        /// <summary>
        /// Returns a value indicating a bad position within the vector.
        /// </summary>
        /// <returns>
        /// A size_t.
        /// </returns>
        internal static ulong Npos()
        {
            var errorHandler = Unity.ArcGISErrorManager.CreateHandler();
            
            var localResult = PInvoke.RT_int64ImmutableCollection_npos(errorHandler);
            
            Unity.ArcGISErrorManager.CheckError(errorHandler);
            
            return localResult.ToUInt64();
        }
        #endregion // Methods
        
        #region Internal Members
        internal ArcGISInt64ImmutableCollection(IntPtr handle) => Handle = handle;
        
        ~ArcGISInt64ImmutableCollection()
        {
            if (Handle != IntPtr.Zero)
            {
                var errorHandler = Unity.ArcGISErrorManager.CreateHandler();
                
                PInvoke.RT_int64ImmutableCollection_destroy(Handle, errorHandler);
                
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
        internal static extern UIntPtr RT_int64ImmutableCollection_getSize(IntPtr handle, IntPtr errorHandler);
        
        [DllImport(Unity.Interop.Dll)]
        internal static extern long RT_int64ImmutableCollection_at(IntPtr handle, UIntPtr position, IntPtr errorHandler);
        
        [DllImport(Unity.Interop.Dll)]
        [return: MarshalAs(UnmanagedType.I1)]
        internal static extern bool RT_int64ImmutableCollection_contains(IntPtr handle, long value, IntPtr errorHandler);
        
        [DllImport(Unity.Interop.Dll)]
        [return: MarshalAs(UnmanagedType.I1)]
        internal static extern bool RT_int64ImmutableCollection_equals(IntPtr handle, IntPtr vector2, IntPtr errorHandler);
        
        [DllImport(Unity.Interop.Dll)]
        internal static extern long RT_int64ImmutableCollection_first(IntPtr handle, IntPtr errorHandler);
        
        [DllImport(Unity.Interop.Dll)]
        internal static extern UIntPtr RT_int64ImmutableCollection_indexOf(IntPtr handle, long value, IntPtr errorHandler);
        
        [DllImport(Unity.Interop.Dll)]
        [return: MarshalAs(UnmanagedType.I1)]
        internal static extern bool RT_int64ImmutableCollection_isEmpty(IntPtr handle, IntPtr errorHandler);
        
        [DllImport(Unity.Interop.Dll)]
        internal static extern long RT_int64ImmutableCollection_last(IntPtr handle, IntPtr errorHandler);
        
        [DllImport(Unity.Interop.Dll)]
        internal static extern UIntPtr RT_int64ImmutableCollection_npos(IntPtr errorHandler);
        
        [DllImport(Unity.Interop.Dll)]
        internal static extern void RT_int64ImmutableCollection_destroy(IntPtr handle, IntPtr errorHandle);
        #endregion // P-Invoke Declarations
    }
}
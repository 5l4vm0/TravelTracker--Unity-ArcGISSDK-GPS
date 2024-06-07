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

namespace Esri.GameEngine.Security
{
    /// <summary>
    /// A class that contains the OAuth2 authorization credential.
    /// </summary>
    /// <since>1.0.0</since>
    [StructLayout(LayoutKind.Sequential)]
    public partial class ArcGISOAuthAuthorizationCredential
    {
        #region Constructors
        /// <summary>
        /// Creates a credential information object for OAuth 2.
        /// </summary>
        /// <param name="accessToken">The access token.</param>
        /// <param name="expirationDate">The date and time when the access token will expire.</param>
        /// <param name="refreshToken">The refresh token.</param>
        /// <since>1.0.0</since>
        public ArcGISOAuthAuthorizationCredential(string accessToken, DateTimeOffset expirationDate, string refreshToken)
        {
            var errorHandler = Unity.ArcGISErrorManager.CreateHandler();
            
            Handle = PInvoke.RT_GEOAuthAuthorizationCredential_create(accessToken, Unity.Convert.ToArcGISDateTime(expirationDate), refreshToken, errorHandler);
            
            Unity.ArcGISErrorManager.CheckError(errorHandler);
        }
        #endregion // Constructors
        
        #region Properties
        /// <summary>
        /// The access token.
        /// </summary>
        /// <since>1.0.0</since>
        public string AccessToken
        {
            get
            {
                var errorHandler = Unity.ArcGISErrorManager.CreateHandler();
                
                var localResult = PInvoke.RT_GEOAuthAuthorizationCredential_getAccessToken(Handle, errorHandler);
                
                Unity.ArcGISErrorManager.CheckError(errorHandler);
                
                return Unity.Convert.FromArcGISString(localResult);
            }
        }
        
        /// <summary>
        /// The date and time when the access token will expire.
        /// </summary>
        /// <since>1.0.0</since>
        public DateTimeOffset ExpirationDate
        {
            get
            {
                var errorHandler = Unity.ArcGISErrorManager.CreateHandler();
                
                var localResult = PInvoke.RT_GEOAuthAuthorizationCredential_getExpirationDate(Handle, errorHandler);
                
                Unity.ArcGISErrorManager.CheckError(errorHandler);
                
                return Unity.Convert.FromArcGISDateTime(localResult);
            }
        }
        
        /// <summary>
        /// The refresh token.
        /// </summary>
        /// <since>1.0.0</since>
        public string RefreshToken
        {
            get
            {
                var errorHandler = Unity.ArcGISErrorManager.CreateHandler();
                
                var localResult = PInvoke.RT_GEOAuthAuthorizationCredential_getRefreshToken(Handle, errorHandler);
                
                Unity.ArcGISErrorManager.CheckError(errorHandler);
                
                return Unity.Convert.FromArcGISString(localResult);
            }
        }
        #endregion // Properties
        
        #region Internal Members
        internal ArcGISOAuthAuthorizationCredential(IntPtr handle) => Handle = handle;
        
        ~ArcGISOAuthAuthorizationCredential()
        {
            if (Handle != IntPtr.Zero)
            {
                var errorHandler = Unity.ArcGISErrorManager.CreateHandler();
                
                PInvoke.RT_GEOAuthAuthorizationCredential_destroy(Handle, errorHandler);
                
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
        internal static extern IntPtr RT_GEOAuthAuthorizationCredential_create([MarshalAs(UnmanagedType.LPStr)]string accessToken, IntPtr expirationDate, [MarshalAs(UnmanagedType.LPStr)]string refreshToken, IntPtr errorHandler);
        
        [DllImport(Unity.Interop.Dll)]
        internal static extern IntPtr RT_GEOAuthAuthorizationCredential_getAccessToken(IntPtr handle, IntPtr errorHandler);
        
        [DllImport(Unity.Interop.Dll)]
        internal static extern IntPtr RT_GEOAuthAuthorizationCredential_getExpirationDate(IntPtr handle, IntPtr errorHandler);
        
        [DllImport(Unity.Interop.Dll)]
        internal static extern IntPtr RT_GEOAuthAuthorizationCredential_getRefreshToken(IntPtr handle, IntPtr errorHandler);
        
        [DllImport(Unity.Interop.Dll)]
        internal static extern void RT_GEOAuthAuthorizationCredential_destroy(IntPtr handle, IntPtr errorHandle);
        #endregion // P-Invoke Declarations
    }
}
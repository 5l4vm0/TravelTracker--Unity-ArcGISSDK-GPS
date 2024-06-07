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
    /// Authentication challenge for OAuth.
    /// </summary>
    /// <since>1.0.0</since>
    [StructLayout(LayoutKind.Sequential)]
    public partial class ArcGISOAuthAuthenticationChallenge :
        ArcGISAuthenticationChallenge
    {
        #region Properties
        /// <summary>
        /// The current authorization endpoint URI.
        /// </summary>
        /// <since>1.0.0</since>
        public string AuthorizeURI
        {
            get
            {
                var errorHandler = Unity.ArcGISErrorManager.CreateHandler();
                
                var localResult = PInvoke.RT_GEOAuthAuthenticationChallenge_getAuthorizeURI(Handle, errorHandler);
                
                Unity.ArcGISErrorManager.CheckError(errorHandler);
                
                return Unity.Convert.FromArcGISString(localResult);
            }
        }
        #endregion // Properties
        
        #region Methods
        /// <summary>
        /// Respond to the challenge with a token.
        /// </summary>
        /// <param name="token"></param>
        /// <since>1.0.0</since>
        public void Respond(string token)
        {
            var errorHandler = Unity.ArcGISErrorManager.CreateHandler();
            
            PInvoke.RT_GEOAuthAuthenticationChallenge_respond(Handle, token, errorHandler);
            
            Unity.ArcGISErrorManager.CheckError(errorHandler);
        }
        #endregion // Methods
        
        #region Internal Members
        internal ArcGISOAuthAuthenticationChallenge(IntPtr handle) : base(handle)
        {
        }
        #endregion // Internal Members
    }
    
    internal static partial class PInvoke
    {
        #region P-Invoke Declarations
        [DllImport(Unity.Interop.Dll)]
        internal static extern IntPtr RT_GEOAuthAuthenticationChallenge_getAuthorizeURI(IntPtr handle, IntPtr errorHandler);
        
        [DllImport(Unity.Interop.Dll)]
        internal static extern void RT_GEOAuthAuthenticationChallenge_respond(IntPtr handle, [MarshalAs(UnmanagedType.LPStr)]string token, IntPtr errorHandler);
        #endregion // P-Invoke Declarations
    }
}
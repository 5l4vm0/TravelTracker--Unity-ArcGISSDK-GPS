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
    /// A class that contains authentication information for OAuth 2.
    /// </summary>
    /// <since>1.0.0</since>
    [StructLayout(LayoutKind.Sequential)]
    public partial class ArcGISOAuthAuthenticationConfiguration :
        ArcGISAuthenticationConfiguration
    {
        #region Constructors
        /// <summary>
        /// Creates an authentication information object for OAuth 2.
        /// </summary>
        /// <param name="clientId">The client identifier.</param>
        /// <param name="clientSecret">The client secret. Mandatory for App Login.</param>
        /// <param name="redirectURI">The redirect URI. Mandatory for Named User Login.</param>
        /// <since>1.0.0</since>
        public ArcGISOAuthAuthenticationConfiguration(string clientId, string clientSecret, string redirectURI) :
            base(IntPtr.Zero)
        {
            var errorHandler = Unity.ArcGISErrorManager.CreateHandler();
            
            Handle = PInvoke.RT_GEOAuthAuthenticationConfiguration_create(clientId, clientSecret, redirectURI, errorHandler);
            
            Unity.ArcGISErrorManager.CheckError(errorHandler);
        }
        
        /// <summary>
        /// Creates an authentication information object for OAuth 2 including credentials.
        /// </summary>
        /// <param name="clientId">The client identifier.</param>
        /// <param name="clientSecret">The client secret. Mandatory for App Login.</param>
        /// <param name="redirectURI">The redirect URI. Mandatory for Named User Login.</param>
        /// <param name="authenticationCredential">The authentication credential.</param>
        /// <since>1.0.0</since>
        public ArcGISOAuthAuthenticationConfiguration(string clientId, string clientSecret, string redirectURI, ArcGISOAuthAuthorizationCredential authenticationCredential) :
            base(IntPtr.Zero)
        {
            var errorHandler = Unity.ArcGISErrorManager.CreateHandler();
            
            var localAuthenticationCredential = authenticationCredential.Handle;
            
            Handle = PInvoke.RT_GEOAuthAuthenticationConfiguration_createWithCredential(clientId, clientSecret, redirectURI, localAuthenticationCredential, errorHandler);
            
            Unity.ArcGISErrorManager.CheckError(errorHandler);
        }
        #endregion // Constructors
        
        #region Properties
        /// <summary>
        /// The current authorization credential.
        /// </summary>
        /// <since>1.0.0</since>
        public ArcGISOAuthAuthorizationCredential AuthorizationCredential
        {
            get
            {
                var errorHandler = Unity.ArcGISErrorManager.CreateHandler();
                
                var localResult = PInvoke.RT_GEOAuthAuthenticationConfiguration_getAuthorizationCredential(Handle, errorHandler);
                
                Unity.ArcGISErrorManager.CheckError(errorHandler);
                
                ArcGISOAuthAuthorizationCredential localLocalResult = null;
                
                if (localResult != IntPtr.Zero)
                {
                    localLocalResult = new ArcGISOAuthAuthorizationCredential(localResult);
                }
                
                return localLocalResult;
            }
        }
        
        /// <summary>
        /// The current client id.
        /// </summary>
        /// <since>1.0.0</since>
        public string ClientId
        {
            get
            {
                var errorHandler = Unity.ArcGISErrorManager.CreateHandler();
                
                var localResult = PInvoke.RT_GEOAuthAuthenticationConfiguration_getClientId(Handle, errorHandler);
                
                Unity.ArcGISErrorManager.CheckError(errorHandler);
                
                return Unity.Convert.FromArcGISString(localResult);
            }
        }
        
        /// <summary>
        /// The current client secret.
        /// </summary>
        /// <since>1.0.0</since>
        public string ClientSecret
        {
            get
            {
                var errorHandler = Unity.ArcGISErrorManager.CreateHandler();
                
                var localResult = PInvoke.RT_GEOAuthAuthenticationConfiguration_getClientSecret(Handle, errorHandler);
                
                Unity.ArcGISErrorManager.CheckError(errorHandler);
                
                return Unity.Convert.FromArcGISString(localResult);
            }
        }
        
        /// <summary>
        /// The current redirect uri.
        /// </summary>
        /// <since>1.0.0</since>
        public string RedirectURI
        {
            get
            {
                var errorHandler = Unity.ArcGISErrorManager.CreateHandler();
                
                var localResult = PInvoke.RT_GEOAuthAuthenticationConfiguration_getRedirectURI(Handle, errorHandler);
                
                Unity.ArcGISErrorManager.CheckError(errorHandler);
                
                return Unity.Convert.FromArcGISString(localResult);
            }
        }
        #endregion // Properties
        
        #region Events
        /// <summary>
        /// Sets the callback invoked when a new OAuth 2 authorization credential is issued.
        /// </summary>
        /// <since>1.0.0</since>
        public ArcGISOAuthAuthorizationCredentialIssuedEvent AuthorizationCredentialIssuedEvent
        {
            get
            {
                return _authorizationCredentialIssuedEventHandler.Delegate;
            }
            set
            {
                if (_authorizationCredentialIssuedEventHandler.Delegate == value)
                {
                    return;
                }
                
                var errorHandler = Unity.ArcGISErrorManager.CreateHandler();
                
                if (value != null)
                {
                    _authorizationCredentialIssuedEventHandler.Delegate = value;
                    
                    PInvoke.RT_GEOAuthAuthenticationConfiguration_setAuthorizationCredentialIssuedEventCallback(Handle, ArcGISOAuthAuthorizationCredentialIssuedEventHandler.HandlerFunction, _authorizationCredentialIssuedEventHandler.UserData, errorHandler);
                }
                else
                {
                    PInvoke.RT_GEOAuthAuthenticationConfiguration_setAuthorizationCredentialIssuedEventCallback(Handle, null, _authorizationCredentialIssuedEventHandler.UserData, errorHandler);
                    
                    _authorizationCredentialIssuedEventHandler.Dispose();
                }
                
                Unity.ArcGISErrorManager.CheckError(errorHandler);
            }
        }
        #endregion // Events
        
        #region Internal Members
        internal ArcGISOAuthAuthenticationConfiguration(IntPtr handle) : base(handle)
        {
        }
        
        ~ArcGISOAuthAuthenticationConfiguration()
        {
            if (Handle != IntPtr.Zero)
            {
                if (_authorizationCredentialIssuedEventHandler.Delegate != null)
                {
                    PInvoke.RT_GEOAuthAuthenticationConfiguration_setAuthorizationCredentialIssuedEventCallback(Handle, null, _authorizationCredentialIssuedEventHandler.UserData, IntPtr.Zero);
                    
                    _authorizationCredentialIssuedEventHandler.Dispose();
                }
            }
        }
        
        internal ArcGISOAuthAuthorizationCredentialIssuedEventHandler _authorizationCredentialIssuedEventHandler = new ArcGISOAuthAuthorizationCredentialIssuedEventHandler();
        #endregion // Internal Members
    }
    
    internal static partial class PInvoke
    {
        #region P-Invoke Declarations
        [DllImport(Unity.Interop.Dll)]
        internal static extern IntPtr RT_GEOAuthAuthenticationConfiguration_create([MarshalAs(UnmanagedType.LPStr)]string clientId, [MarshalAs(UnmanagedType.LPStr)]string clientSecret, [MarshalAs(UnmanagedType.LPStr)]string redirectURI, IntPtr errorHandler);
        
        [DllImport(Unity.Interop.Dll)]
        internal static extern IntPtr RT_GEOAuthAuthenticationConfiguration_createWithCredential([MarshalAs(UnmanagedType.LPStr)]string clientId, [MarshalAs(UnmanagedType.LPStr)]string clientSecret, [MarshalAs(UnmanagedType.LPStr)]string redirectURI, IntPtr authenticationCredential, IntPtr errorHandler);
        
        [DllImport(Unity.Interop.Dll)]
        internal static extern IntPtr RT_GEOAuthAuthenticationConfiguration_getAuthorizationCredential(IntPtr handle, IntPtr errorHandler);
        
        [DllImport(Unity.Interop.Dll)]
        internal static extern IntPtr RT_GEOAuthAuthenticationConfiguration_getClientId(IntPtr handle, IntPtr errorHandler);
        
        [DllImport(Unity.Interop.Dll)]
        internal static extern IntPtr RT_GEOAuthAuthenticationConfiguration_getClientSecret(IntPtr handle, IntPtr errorHandler);
        
        [DllImport(Unity.Interop.Dll)]
        internal static extern IntPtr RT_GEOAuthAuthenticationConfiguration_getRedirectURI(IntPtr handle, IntPtr errorHandler);
        
        [DllImport(Unity.Interop.Dll)]
        internal static extern void RT_GEOAuthAuthenticationConfiguration_setAuthorizationCredentialIssuedEventCallback(IntPtr handle, ArcGISOAuthAuthorizationCredentialIssuedEventInternal authorizationCredentialIssuedEvent, IntPtr userData, IntPtr errorHandler);
        #endregion // P-Invoke Declarations
    }
}
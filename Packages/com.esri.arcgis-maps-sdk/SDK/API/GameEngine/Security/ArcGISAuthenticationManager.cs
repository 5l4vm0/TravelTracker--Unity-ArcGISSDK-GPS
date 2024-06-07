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
    /// A class to manage authentication.
    /// </summary>
    /// <since>1.0.0</since>
    [StructLayout(LayoutKind.Sequential)]
    public partial class ArcGISAuthenticationManager
    {
        #region Properties
        /// <summary>
        /// The authentication configurations for URLs.
        /// </summary>
        /// <since>1.0.0</since>
        public static Unity.ArcGISDictionary<string, ArcGISAuthenticationConfiguration> AuthenticationConfigurations
        {
            get
            {
                var errorHandler = Unity.ArcGISErrorManager.CreateHandler();
                
                var localResult = PInvoke.RT_GEAuthenticationManager_getAuthenticationConfigurations(errorHandler);
                
                Unity.ArcGISErrorManager.CheckError(errorHandler);
                
                Unity.ArcGISDictionary<string, ArcGISAuthenticationConfiguration> localLocalResult = null;
                
                if (localResult != IntPtr.Zero)
                {
                    localLocalResult = new Unity.ArcGISDictionary<string, ArcGISAuthenticationConfiguration>(localResult);
                }
                
                return localLocalResult;
            }
        }
        #endregion // Properties
        
        #region Methods
        /// <summary>
        /// Starts the authorization flow on the provided authentication configuration.
        /// </summary>
        /// <remarks>
        /// The URL should include the `/sharing/rest` fragment.
        /// The provided authentication configuration won't be considered for requests authentication
        /// unless it gets manually added to <see cref="GameEngine.Security.ArcGISAuthenticationManager.AuthenticationConfigurations">ArcGISAuthenticationManager.AuthenticationConfigurations</see>.
        /// </remarks>
        /// <param name="restEndpointURL">The REST endpoint URL of the portal.</param>
        /// <param name="authenticationConfiguration">The authentication configuration to get authorized.</param>
        /// <returns>
        /// An <see cref="Unity.ArcGISFuture<T>">ArcGISFuture<T></see> that has no return value.
        /// </returns>
        /// <since>1.0.0</since>
        public static Unity.ArcGISFuture AuthorizeAuthenticationConfigurationAsync(string restEndpointURL, ArcGISAuthenticationConfiguration authenticationConfiguration)
        {
            var errorHandler = Unity.ArcGISErrorManager.CreateHandler();
            
            var localAuthenticationConfiguration = authenticationConfiguration.Handle;
            
            var localResult = PInvoke.RT_GEAuthenticationManager_authorizeAuthenticationConfigurationAsync(restEndpointURL, localAuthenticationConfiguration, errorHandler);
            
            Unity.ArcGISErrorManager.CheckError(errorHandler);
            
            Unity.ArcGISFuture localLocalResult = null;
            
            if (localResult != IntPtr.Zero)
            {
                localLocalResult = new Unity.ArcGISFuture(localResult);
            }
            
            return localLocalResult;
        }
        #endregion // Methods
        
        #region Events
        /// <summary>
        /// Sets the global callback invoked when an authentication challenge is issued.
        /// </summary>
        /// <since>1.0.0</since>
        public static ArcGISAuthenticationChallengeIssuedEvent AuthenticationChallengeIssued
        {
            get
            {
                return _authenticationChallengeIssuedHandler.Delegate;
            }
            set
            {
                if (_authenticationChallengeIssuedHandler.Delegate == value)
                {
                    return;
                }
                
                var errorHandler = Unity.ArcGISErrorManager.CreateHandler();
                
                if (value != null)
                {
                    _authenticationChallengeIssuedHandler.Delegate = value;
                    
                    PInvoke.RT_GEAuthenticationManager_setAuthenticationChallengeIssuedCallback(ArcGISAuthenticationChallengeIssuedEventHandler.HandlerFunction, _authenticationChallengeIssuedHandler.UserData, errorHandler);
                }
                else
                {
                    PInvoke.RT_GEAuthenticationManager_setAuthenticationChallengeIssuedCallback(null, _authenticationChallengeIssuedHandler.UserData, errorHandler);
                    
                    _authenticationChallengeIssuedHandler.Dispose();
                }
                
                Unity.ArcGISErrorManager.CheckError(errorHandler);
            }
        }
        #endregion // Events
        
        #region Internal Members
        internal static ArcGISAuthenticationChallengeIssuedEventHandler _authenticationChallengeIssuedHandler = new ArcGISAuthenticationChallengeIssuedEventHandler();
        #endregion // Internal Members
    }
    
    internal static partial class PInvoke
    {
        #region P-Invoke Declarations
        [DllImport(Unity.Interop.Dll)]
        internal static extern IntPtr RT_GEAuthenticationManager_getAuthenticationConfigurations(IntPtr errorHandler);
        
        [DllImport(Unity.Interop.Dll)]
        internal static extern IntPtr RT_GEAuthenticationManager_authorizeAuthenticationConfigurationAsync([MarshalAs(UnmanagedType.LPStr)]string restEndpointURL, IntPtr authenticationConfiguration, IntPtr errorHandler);
        
        [DllImport(Unity.Interop.Dll)]
        internal static extern void RT_GEAuthenticationManager_setAuthenticationChallengeIssuedCallback(ArcGISAuthenticationChallengeIssuedEventInternal authenticationChallengeIssued, IntPtr userData, IntPtr errorHandler);
        #endregion // P-Invoke Declarations
    }
}
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
    /// A callback invoked when a new OAuth2 authorization credential is issued.
    /// </summary>
    /// <since>1.0.0</since>
    public delegate void ArcGISOAuthAuthorizationCredentialIssuedEvent(ArcGISOAuthAuthorizationCredential authorizationCredential);
    
    internal delegate void ArcGISOAuthAuthorizationCredentialIssuedEventInternal(IntPtr userData, IntPtr authorizationCredential);
    
    internal class ArcGISOAuthAuthorizationCredentialIssuedEventHandler : Unity.ArcGISEventHandler<ArcGISOAuthAuthorizationCredentialIssuedEvent>
    {
        [Unity.MonoPInvokeCallback(typeof(ArcGISOAuthAuthorizationCredentialIssuedEventInternal))]
        internal static void HandlerFunction(IntPtr userData, IntPtr authorizationCredential)
        {
            if (userData == IntPtr.Zero)
            {
                return;
            }
            
            var callbackObject = (ArcGISOAuthAuthorizationCredentialIssuedEventHandler)((GCHandle)userData).Target;
            
            if (callbackObject == null)
            {
                return;
            }
            
            var callback = callbackObject.m_delegate;
            
            if (callback == null)
            {
                return;
            }
            
            ArcGISOAuthAuthorizationCredential localAuthorizationCredential = null;
            
            if (authorizationCredential != IntPtr.Zero)
            {
                localAuthorizationCredential = new ArcGISOAuthAuthorizationCredential(authorizationCredential);
            }
            
            callback(localAuthorizationCredential);
        }
    }
}
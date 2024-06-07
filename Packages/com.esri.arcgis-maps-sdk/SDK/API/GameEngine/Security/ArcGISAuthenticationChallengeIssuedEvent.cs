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
    /// A callback invoked when an authentication challenge is issued.
    /// </summary>
    /// <since>1.0.0</since>
    public delegate void ArcGISAuthenticationChallengeIssuedEvent(ArcGISAuthenticationChallenge authenticationChallenge);
    
    internal delegate void ArcGISAuthenticationChallengeIssuedEventInternal(IntPtr userData, IntPtr authenticationChallenge);
    
    internal class ArcGISAuthenticationChallengeIssuedEventHandler : Unity.ArcGISEventHandler<ArcGISAuthenticationChallengeIssuedEvent>
    {
        [Unity.MonoPInvokeCallback(typeof(ArcGISAuthenticationChallengeIssuedEventInternal))]
        internal static void HandlerFunction(IntPtr userData, IntPtr authenticationChallenge)
        {
            if (userData == IntPtr.Zero)
            {
                return;
            }
            
            var callbackObject = (ArcGISAuthenticationChallengeIssuedEventHandler)((GCHandle)userData).Target;
            
            if (callbackObject == null)
            {
                return;
            }
            
            var callback = callbackObject.m_delegate;
            
            if (callback == null)
            {
                return;
            }
            
            ArcGISAuthenticationChallenge localAuthenticationChallenge = null;
            
            if (authenticationChallenge != IntPtr.Zero)
            {
                var objectType = GameEngine.Security.PInvoke.RT_GEAuthenticationChallenge_getObjectType(authenticationChallenge, IntPtr.Zero);
                
                switch (objectType)
                {
                    case GameEngine.Security.ArcGISAuthenticationChallengeType.ArcGISOAuthAuthenticationChallenge:
                        localAuthenticationChallenge = new ArcGISOAuthAuthenticationChallenge(authenticationChallenge);
                        break;
                    default:
                        localAuthenticationChallenge = new ArcGISAuthenticationChallenge(authenticationChallenge);
                        break;
                }
            }
            
            callback(localAuthenticationChallenge);
        }
    }
}
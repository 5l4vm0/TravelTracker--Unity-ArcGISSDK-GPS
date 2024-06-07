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
using Esri.GameEngine;
#if UNITY_ANDROID && !UNITY_EDITOR
using Esri.GameEngine.Net;
#endif
using System;
using System.Reflection;

namespace Esri.Unity
{
	public static class Environment
	{
#if UNITY_ANDROID && !UNITY_EDITOR
		static ArcGISMapsSDK.Net.ArcGISHTTPRequestHandler HTTPRequestHandler;
#endif

		public static void Deinitialize()
		{
			ArcGISRuntimeEnvironment.Error = null;

#if UNITY_ANDROID && !UNITY_EDITOR
			ArcGISHTTPRequestHandler.RequestIssued = null;

			HTTPRequestHandler = null;
#endif
		}

		public static void Initialize(string productName, string productVersion, string tempDirectory, string installDirectory)
		{
#if UNITY_ANDROID && !UNITY_EDITOR
			HTTPRequestHandler = new ArcGISMapsSDK.Net.ArcGISHTTPRequestHandler();

			ArcGISHTTPRequestHandler.RequestIssued = HTTPRequestHandler.GetRequestIssuedHandler();
#endif

			var version = typeof(Environment).Assembly.GetName().Version.ToString();
			version = version.Substring(0, version.LastIndexOf('.'));

			ArcGISRuntimeEnvironment.SetRuntimeClient(Standard.ArcGISRuntimeClient.Unity, version);
			ArcGISRuntimeEnvironment.SetProductInfo(productName, productVersion);
			ArcGISRuntimeEnvironment.EnableBreakOnException(false);
			ArcGISRuntimeEnvironment.EnableLeakDetection(false);
			ArcGISRuntimeEnvironment.SetTempDirectory(tempDirectory);
			ArcGISRuntimeEnvironment.SetInstallDirectory(installDirectory);

			ArcGISRuntimeEnvironment.Error = delegate (Exception error)
			{
				UnityEngine.Debug.LogError(error.Message);
			};
		}

		public static string GetPluginCopyright()
		{
			var attributes = typeof(Environment).Assembly
				.GetCustomAttributes(typeof(AssemblyCopyrightAttribute), false);

			var assemblyCopyrightAttribute = (AssemblyCopyrightAttribute)attributes[0];

			return assemblyCopyrightAttribute.Copyright;
		}

		public static string GetPluginName()
		{
			var attributes = typeof(Environment).Assembly
				.GetCustomAttributes(typeof(AssemblyProductAttribute), false);

			var assemblyProductAttribute = (AssemblyProductAttribute)attributes[0];

			return assemblyProductAttribute.Product;
		}

		public static string GetPluginVersion()
		{
			var version = typeof(Environment).Assembly.GetName().Version.ToString();

			return version.Substring(0, version.LastIndexOf('.'));
		}
	}
}

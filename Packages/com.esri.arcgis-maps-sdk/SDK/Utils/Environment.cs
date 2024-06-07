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
using Esri.ArcGISMapsSDK.Security;
using Esri.GameEngine.Security;
using System.Runtime.CompilerServices;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.Networking;

[assembly: InternalsVisibleTo("ArcGISMapsSDK.Editor")]
[assembly: InternalsVisibleTo("EditTests")]
[assembly: InternalsVisibleTo("PlayTests")]
namespace Esri.ArcGISMapsSDK.Utils
{
#if UNITY_EDITOR
	[InitializeOnLoad]
#endif
	public class Environment
	{
		static bool initialized = false;
		static ArcGISAuthenticationChallengeHandler authenticationChallengeHandler;

		public static CertificateHandler CertificateHandler { get; set; }

#if UNITY_EDITOR
		static Environment()
		{
			Initialize();

			AssemblyReloadEvents.beforeAssemblyReload += AssemblyReloadEvents_beforeAssemblyReload;
		}

		private static void AssemblyReloadEvents_beforeAssemblyReload()
		{
			AssemblyReloadEvents.beforeAssemblyReload -= AssemblyReloadEvents_beforeAssemblyReload;

			Deinitialize();
		}
#endif

		internal static void Deinitialize()
		{
			if (initialized)
			{
				initialized = false;

				Unity.Environment.Deinitialize();

				ArcGISAuthenticationManager.AuthenticationChallengeIssued = null;
			}
		}

		public static bool HasAuthenticationChallengeHandler()
		{
			return authenticationChallengeHandler != null;
		}

		internal static void Initialize()
		{
			if (!initialized)
			{
				initialized = true;

				ArcGISMainThreadScheduler.Instance();

				string productName = Application.productName;
				string productVersion = Application.version;
				string tempDirectory = Application.temporaryCachePath;
				string installDirectory = Application.dataPath;

				Unity.Environment.Initialize(productName, productVersion, tempDirectory, installDirectory);

				ArcGISAuthenticationManager.AuthenticationChallengeIssued = delegate (ArcGISAuthenticationChallenge authenticationChallenge)
				{
					var authenticationChallengeType = authenticationChallenge.ObjectType;

					switch (authenticationChallengeType)
					{
						case ArcGISAuthenticationChallengeType.ArcGISOAuthAuthenticationChallenge:
							ArcGISMainThreadScheduler.Instance().Schedule(() =>
							{
								if (authenticationChallengeHandler != null)
								{
									authenticationChallengeHandler.HandleChallenge(authenticationChallenge);
								}
							});
							break;
						default:
							Debug.LogError("Unsupported AuthenticationChallenge type");
							return;
					}
				};
			}
		}

		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
		static void OnRuntimeMethodLoad()
		{
			Initialize();
		}

		public static void SetAuthenticationChallengeHandler(ArcGISAuthenticationChallengeHandler authenticationChallengeHandler)
		{
			Environment.authenticationChallengeHandler = authenticationChallengeHandler;
		}
	}
}

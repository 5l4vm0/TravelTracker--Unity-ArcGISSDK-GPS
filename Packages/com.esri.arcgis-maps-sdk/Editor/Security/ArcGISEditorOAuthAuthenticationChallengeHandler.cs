// COPYRIGHT 1995-2022 ESRI
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
using System;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using UnityEngine;

namespace Esri.ArcGISMapsSDK.Editor.Security
{
	public class ArcGISEditorOAuthAuthenticationChallengeHandler : ArcGISAuthenticationChallengeHandler
	{
		private HttpListener httpListener;

		public override ArcGISAuthenticationChallengeType GetAuthenticationChallengeType()
		{
			return ArcGISAuthenticationChallengeType.ArcGISOAuthAuthenticationChallenge;
		}

		public override void HandleChallenge(ArcGISAuthenticationChallenge authenticationChallenge)
		{
			var oauthAuthenticationChallenge = authenticationChallenge as ArcGISOAuthAuthenticationChallenge;

			var authorizeURI = oauthAuthenticationChallenge.AuthorizeURI;

			HandleChallengeInternal(oauthAuthenticationChallenge.AuthorizeURI).ContinueWith(authorizationCodeTask =>
			{
				if (authorizationCodeTask.IsFaulted)
				{
					Debug.LogError(authorizationCodeTask.Exception.Message);

					oauthAuthenticationChallenge.Cancel();
				}
				else if (authorizationCodeTask.IsCanceled)
				{
					oauthAuthenticationChallenge.Cancel();
				}
				else
				{
					var authorizationCode = authorizationCodeTask.Result;

					if (authorizationCode != null)
					{
						oauthAuthenticationChallenge.Respond(authorizationCode);
					}
					else
					{
						oauthAuthenticationChallenge.Cancel();
					}
				}
			});
		}

		private Task<string> HandleChallengeInternal(string authorizeURI)
		{
			var matches = Regex.Matches(authorizeURI, @"redirect_uri=([^&]*)", RegexOptions.IgnoreCase);

			if (matches.Count != 1)
			{
				return Task.FromException<string>(new ArgumentException("Invalid authorize URI"));
			}

			var redirectURI = matches[0].Groups[1].Value;

			if (redirectURI == "urn:ietf:wg:oauth:2.0:oob")
			{
				return Task.FromException<string>(new ArgumentException("\"urn:ietf:wg:oauth:2.0:oob\" is not a supported redirect URI"));
			}

			try
			{
				var uri = new Uri(redirectURI);

				if (uri.Scheme == "http" && uri.Host == "localhost")
				{
					redirectURI = uri.ToString();
				}
				else
				{
					return Task.FromException<string>(new ArgumentException("Invalid redirect URI"));
				}
			}
			catch
			{
				return Task.FromException<string>(new ArgumentException("Invalid redirect URI"));
			}

			var httpListenerPrefix = redirectURI;

			if (!httpListenerPrefix.EndsWith("/"))
			{
				httpListenerPrefix += "/";
			}

			httpListener = new HttpListener();
			httpListener.Prefixes.Add(httpListenerPrefix);
			httpListener.Start();

			var taskCompletionSource = new TaskCompletionSource<string>();

			httpListener.GetContextAsync().ContinueWith(task =>
			{
				if (!task.IsCompleted)
				{
					return;
				}

				var context = task.Result;

				context.Response.Close();

				httpListener.Stop();

				if (context.Request.QueryString.Get("error") != null)
				{
					taskCompletionSource.SetException(new Exception(String.Format("OAuth authorization error: {0}.", context.Request.QueryString.Get("error"))));
				}
				else
				{
					var code = context.Request.QueryString.Get("code");

					taskCompletionSource.SetResult(code);
				}
			});

			Application.OpenURL(authorizeURI);

			return taskCompletionSource.Task;
		}
	}
}

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
using Esri.ArcGISMapsSDK.Utils.GeoCoord;
using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

namespace Esri.ArcGISMapsSDK.Editor.UI
{
	public static class MapCreatorUtilities
	{
		public static IArcGISMapComponentInterface MapComponent
		{
			get
			{
				var maps = GameObject.FindObjectsOfType<MonoBehaviour>().OfType<IArcGISMapComponentInterface>();

				if (maps.Count() != 1)
				{
					return null;
				}

				return maps.ElementAt(0);
			}
		}

		internal static class Assets
		{
			private static string resourcesFolderPath;

			static Assets()
			{
				const string pluginRelativePath = "Assets/ArcGISMapsSDK/Editor/Resources";
				const string packageRelativePath = "Packages/com.esri.arcgis-maps-sdk/Editor/Resources";

				if (Directory.Exists(Path.GetFullPath(pluginRelativePath)))
				{
					resourcesFolderPath = pluginRelativePath;
				}
				else if (Directory.Exists(Path.GetFullPath(packageRelativePath)))
				{
					resourcesFolderPath = packageRelativePath;
				}
			}

			private static string GetFileRelativePath(string fileName)
			{
				return Path.Combine(resourcesFolderPath, fileName);
			}

			public static Texture2D LoadImage(string fileName)
			{
				string fileRelativePath = GetFileRelativePath($"Images/{fileName}");

				return AssetDatabase.LoadAssetAtPath<Texture2D>(fileRelativePath);
			}

			public static StyleSheet LoadStyleSheet(string fileName)
			{
				string fileRelativePath = GetFileRelativePath($"Styles/{fileName}");

				return AssetDatabase.LoadAssetAtPath<StyleSheet>(fileRelativePath);
			}

			public static VisualTreeAsset LoadVisualTreeAsset(string fileName)
			{
				string fileRelativePath = GetFileRelativePath($"Templates/{fileName}");

				return AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(fileRelativePath);
			}
		}

		public static DoubleField InitializeDoubleField(VisualElement element, string name, SerializedProperty serializedProperty, Action<double> valueChangedCallback = null, bool truncateDouble = false)
		{
			DoubleField doubleField = element.Query<DoubleField>($"{name}-text");

			if (doubleField == null)
			{
				Debug.LogError($"Double field {name}-text not found");
				return null;
			}

			if (serializedProperty != null)
			{
				doubleField.value = !truncateDouble ? serializedProperty.doubleValue : TruncateDoubleForUI(serializedProperty.doubleValue);
			}

			doubleField.RegisterValueChangedCallback(@event =>
			{
				if (@event.newValue != @event.previousValue)
				{
					if (serializedProperty != null)
					{
						serializedProperty.doubleValue = @event.newValue;
						serializedProperty.serializedObject.ApplyModifiedProperties();
					}

					if (valueChangedCallback != null)
					{
						valueChangedCallback(@event.newValue);
					}
				}
			});

			return doubleField;
		}

		public static IntegerField InitializeIntegerField(VisualElement element, string name, SerializedProperty serializedProperty = null, Action<int> valueChangedCallback = null)
		{
			IntegerField intField = element.Query<IntegerField>($"{name}-text");

			if (intField == null)
			{
				Debug.LogError($"Int field {name}-text not found");
				return null;
			}

			if (serializedProperty != null)
			{
				intField.value = serializedProperty.intValue;
			}

			intField.RegisterValueChangedCallback(@event =>
			{
				if (@event.newValue != @event.previousValue)
				{
					if (serializedProperty != null)
					{
						serializedProperty.intValue = @event.newValue;
						serializedProperty.serializedObject.ApplyModifiedProperties();
					}

					if (valueChangedCallback != null)
					{
						valueChangedCallback(@event.newValue);
					}
				}
			});

			return intField;
		}

		public static void UpdateArcGISPointLabels(DoubleField X, DoubleField Y, DoubleField Z, int wkid)
		{
			if (wkid == SpatialReferenceWkid.WGS84 || wkid == SpatialReferenceWkid.CGCS2000)
			{
				X.label = "Longitude";
				Y.label = "Latitude";

				if (Z != null)
				{
					Z.label = "Altitude";
				}
			}
			else
			{
				X.label = "X";
				Y.label = "Y";

				if (Z != null)
				{
					Z.label = "Z";
				}
			}
		}

		public static void MarkDirty()
		{
			if (EditorSceneManager.GetActiveScene() != null)
			{
				EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
			}
		}

		public static double TruncateDoubleForUI(double toTruncate)
		{
			const double roundFactor = 1000000;
			return Math.Truncate(toTruncate * roundFactor) / roundFactor;
		}
	}
}

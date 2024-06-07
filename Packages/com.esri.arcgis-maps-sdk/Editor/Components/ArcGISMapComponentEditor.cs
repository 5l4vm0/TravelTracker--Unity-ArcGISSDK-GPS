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
using Esri.ArcGISMapsSDK.Components;
using Esri.GameEngine.Map;
using UnityEditor;
using UnityEngine;

namespace Esri.ArcGISMapsSDK.Editor.Components
{
	static class Styles
	{
		public static readonly GUIContent EditorModeEnabled = EditorGUIUtility.TrTextContent("Enable Editor Mode", "When enabled, content from the ArcGIS Maps SDK will be shown in Edit mode.");
		public static readonly GUIContent OriginPosition = EditorGUIUtility.TrTextContent("Origin Position", "This real-world location will be used as Unity's 0,0,0 point. Using the ArcGIS Rebase component in Play mode or enabling 'Rebase With Scene View' in Edit mode updates this origin.");
		public static readonly GUIContent DataFetchWithSceneView = EditorGUIUtility.TrTextContent("Data Fetch Using Scene Camera", "When enabled, while navigating through the scene in Edit mode, the Scene camera is used to determine which data is fetched. When disabled, the ArcGIS Camera component will be used to fetch the data.");
		public static readonly GUIContent RebaseWithSceneView = EditorGUIUtility.TrTextContent("Rebase with Scene View", "When enabled, navigating around the scene in Edit mode will cause the Origin Position/HPRoot to be periodically updated. This is the same behavior the ArcGIS Rebase component provides in Play mode.");
		public static readonly GUIContent MeshCollidersEnabled = EditorGUIUtility.TrTextContent("Mesh Colliders Enabled", "When enabled, mesh colliders will be automatically generated for all the content from the ArcGIS Maps SDK. This will impact performance, but enable Raycasting and other workflows.");
	}

	[CustomEditor(typeof(ArcGISMapComponent))]
	public class ArcGISMapComponentEditor : UnityEditor.Editor
	{
		private bool showBasemapCategory = true;
		private bool showExtentCategory = true;
		private bool showOriginCategory = true;
		private bool showAuthenticationCategory = true;

		SerializedProperty apiKeyProp;
		SerializedProperty basemapProp;
		SerializedProperty basemapTypeProp;
		SerializedProperty basemapAuthProp;
		SerializedProperty editorModeEnabledProp;
		SerializedProperty mapTypeProp;
		SerializedProperty dataFetchWithSceneViewProp;
		SerializedProperty rebaseWithSceneViewProp;
		SerializedProperty elevationSourcesProp;
		SerializedProperty enableExtentProp;
		SerializedProperty extentProp;
		SerializedProperty extentShapeProp;
		SerializedProperty extentShapeDimensionsProp;
		SerializedProperty layersProp;
		SerializedProperty meshCollidersEnabledProp;
		SerializedProperty originPositionProp;
		SerializedProperty configurationsProp;

		void OnEnable()
		{
			apiKeyProp = serializedObject.FindProperty("apiKey");
			basemapProp = serializedObject.FindProperty("basemap");
			basemapTypeProp = serializedObject.FindProperty("basemapType");
			basemapAuthProp = serializedObject.FindProperty("basemapAuthentication");
			editorModeEnabledProp = serializedObject.FindProperty("editorModeEnabled");
			mapTypeProp = serializedObject.FindProperty("mapType");
			dataFetchWithSceneViewProp = serializedObject.FindProperty("dataFetchWithSceneView");
			rebaseWithSceneViewProp = serializedObject.FindProperty("rebaseWithSceneView");
			elevationSourcesProp = serializedObject.FindProperty("mapElevation.ElevationSources");
			enableExtentProp = serializedObject.FindProperty("enableExtent");
			extentProp = serializedObject.FindProperty("extent");
			extentShapeProp = extentProp.FindPropertyRelative("ExtentShape");
			extentShapeDimensionsProp = extentProp.FindPropertyRelative("ShapeDimensions");
			layersProp = serializedObject.FindProperty("layers");
			meshCollidersEnabledProp = serializedObject.FindProperty("meshCollidersEnabled");
			originPositionProp = serializedObject.FindProperty("originPosition");
			configurationsProp = serializedObject.FindProperty("configurations");
		}

		public override void OnInspectorGUI()
		{
			serializedObject.Update();

			var mapComponent = target as ArcGISMapComponent;

			EditorGUI.BeginChangeCheck();
			EditorGUILayout.PropertyField(editorModeEnabledProp, Styles.EditorModeEnabled);
			if (EditorGUI.EndChangeCheck())
			{
				mapComponent.EditorModeEnabled = editorModeEnabledProp.boolValue;
				EditorUtility.SetDirty(mapComponent);
			}

			if (mapComponent.EditorModeEnabled)
			{
				EditorGUI.BeginChangeCheck();
				EditorGUILayout.PropertyField(dataFetchWithSceneViewProp, Styles.DataFetchWithSceneView);
				if (EditorGUI.EndChangeCheck())
				{
					mapComponent.DataFetchWithSceneView = dataFetchWithSceneViewProp.boolValue;
					EditorUtility.SetDirty(mapComponent);
				}

				EditorGUI.BeginChangeCheck();
				EditorGUILayout.PropertyField(rebaseWithSceneViewProp, Styles.RebaseWithSceneView);
				if (EditorGUI.EndChangeCheck())
				{
					mapComponent.RebaseWithSceneView = rebaseWithSceneViewProp.boolValue;
					EditorUtility.SetDirty(mapComponent);
				}
			}

			EditorGUI.BeginChangeCheck();
			EditorGUILayout.PropertyField(meshCollidersEnabledProp, Styles.MeshCollidersEnabled);
			if (EditorGUI.EndChangeCheck())
			{
				mapComponent.MeshCollidersEnabled = meshCollidersEnabledProp.boolValue;
				EditorUtility.SetDirty(mapComponent);
			}

			EditorGUI.BeginChangeCheck();
			EditorGUILayout.PropertyField(mapTypeProp);
			if (EditorGUI.EndChangeCheck())
			{
				mapTypeProp.serializedObject.ApplyModifiedProperties();
				mapComponent.OnMapTypeChanged();
			}

			showOriginCategory = EditorGUILayout.BeginFoldoutHeaderGroup(showOriginCategory, "Origin Position");
			if (showOriginCategory)
			{
				EditorGUI.BeginChangeCheck();
				EditorGUILayout.PropertyField(originPositionProp, Styles.OriginPosition);
				if (EditorGUI.EndChangeCheck())
				{
					originPositionProp.serializedObject.ApplyModifiedProperties();
					mapComponent.OnOriginPositionChanged();
				}
			}
			EditorGUILayout.EndFoldoutHeaderGroup();

			showBasemapCategory = EditorGUILayout.BeginFoldoutHeaderGroup(showBasemapCategory, "Basemap");
			if (showBasemapCategory)
			{
				EditorGUI.BeginChangeCheck();
				EditorGUILayout.PropertyField(basemapProp);
				bool updateBasemap = false;
				updateBasemap |= EditorGUI.EndChangeCheck();

				EditorGUI.BeginChangeCheck();
				EditorGUILayout.PropertyField(basemapTypeProp);
				updateBasemap |= EditorGUI.EndChangeCheck();

				EditorGUI.BeginChangeCheck();
				EditorGUILayout.PropertyField(basemapAuthProp, new GUIContent("Authentication"));
				updateBasemap |= EditorGUI.EndChangeCheck();

				if (updateBasemap)
				{
					basemapProp.serializedObject.ApplyModifiedProperties();
					basemapTypeProp.serializedObject.ApplyModifiedProperties();
					basemapAuthProp.serializedObject.ApplyModifiedProperties();
					mapComponent.UpdateBasemap();
				}
			}
			EditorGUILayout.EndFoldoutHeaderGroup();

			EditorGUI.BeginChangeCheck();
			EditorGUILayout.PropertyField(elevationSourcesProp);
			if (EditorGUI.EndChangeCheck())
			{
				elevationSourcesProp.serializedObject.ApplyModifiedProperties();
				mapComponent.UpdateElevation();
			}

			if (mapTypeProp.enumNames[mapTypeProp.enumValueIndex] == "Local")
			{
				EditorGUI.BeginChangeCheck();
				EditorGUILayout.PropertyField(enableExtentProp);

				if (enableExtentProp.boolValue)
				{
					showExtentCategory = EditorGUILayout.BeginFoldoutHeaderGroup(showExtentCategory, "Extent");
					if (showExtentCategory)
					{
						EditorGUILayout.PropertyField(extentProp.FindPropertyRelative("UseOriginAsCenter"), new GUIContent("Use Origin Position as Center"));

						EditorGUI.BeginDisabledGroup(mapComponent.Extent.UseOriginAsCenter);
						EditorGUILayout.PropertyField(extentProp.FindPropertyRelative("GeographicCenter"));
						EditorGUI.EndDisabledGroup();

						GUIContent shapeLabel;

						if (extentShapeProp.enumNames[extentShapeProp.enumValueIndex] == "Circle")
						{
							shapeLabel = new GUIContent("Radius");
						}
						else if (extentShapeProp.enumNames[extentShapeProp.enumValueIndex] == "Square")
						{
							shapeLabel = new GUIContent("Length");
						}
						else
						{
							shapeLabel = new GUIContent("X");
						}

						EditorGUILayout.PropertyField(extentShapeProp);
						EditorGUILayout.PropertyField(extentShapeDimensionsProp.FindPropertyRelative("x"), shapeLabel);

						if (extentShapeProp.enumNames[extentShapeProp.enumValueIndex] == "Rectangle")
						{
							EditorGUILayout.PropertyField(extentShapeDimensionsProp.FindPropertyRelative("y"), new GUIContent("Y"));
						}
					}
					EditorGUILayout.EndFoldoutHeaderGroup();
				}

				if (EditorGUI.EndChangeCheck())
				{
					extentProp.serializedObject.ApplyModifiedProperties();
					enableExtentProp.serializedObject.ApplyModifiedProperties();
					mapComponent.UpdateExtent();
				}
			}

			EditorGUI.BeginChangeCheck();
			EditorGUILayout.PropertyField(layersProp);
			if (EditorGUI.EndChangeCheck())
			{
				layersProp.serializedObject.ApplyModifiedProperties();
				mapComponent.UpdateLayers();
			}

			showAuthenticationCategory = EditorGUILayout.BeginFoldoutHeaderGroup(showAuthenticationCategory, "Authentication");
			if (showAuthenticationCategory)
			{
				EditorGUI.BeginChangeCheck();
				EditorGUILayout.PropertyField(apiKeyProp, new GUIContent("API Key"));
				if (EditorGUI.EndChangeCheck())
				{
					apiKeyProp.serializedObject.ApplyModifiedProperties();
					mapComponent.InitializeArcGISMap();
					EditorUtility.SetDirty(mapComponent);
				}
			}
			EditorGUILayout.EndFoldoutHeaderGroup();

			EditorGUI.BeginChangeCheck();
			EditorGUILayout.PropertyField(configurationsProp, new GUIContent("Authentication Configurations"));
			if (EditorGUI.EndChangeCheck())
			{
				// this might need to be different
				configurationsProp.serializedObject.ApplyModifiedProperties();
				mapComponent.InitializeArcGISMap();
			}

			serializedObject.ApplyModifiedProperties();
		}
	}
}

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
using Esri.ArcGISMapsSDK.Components;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Esri.ArcGISMapsSDK.Editor.UI
{
	public class ArcGISMapCreatorSettingsTool : ArcGISMapCreatorTool
	{
		private static IArcGISMapComponentInterface mapComponent;

		private static Toggle editorModeToggle;
		private static Toggle meshColliderToggle;
		private static VisualElement rootElement;

		public override VisualElement GetContent()
		{
			rootElement = new VisualElement();
			rootElement.name = "ArcGISMapCreatorSettingsTool";

			var template = MapCreatorUtilities.Assets.LoadVisualTreeAsset("MapCreator/SettingsToolTemplate.uxml");
			template.CloneTree(rootElement);

			rootElement.styleSheets.Add(MapCreatorUtilities.Assets.LoadStyleSheet("MapCreator/SettingsToolStyle.uss"));

			mapComponent = MapCreatorUtilities.MapComponent;

			InitEditorModeToggle(rootElement);
			InitSceneViewDataFetchToggle(rootElement);
			InitSceneViewRebaseToggle(rootElement);
			InitMeshColliderToggle(rootElement);

			return rootElement;
		}

		public override Texture2D GetImage()
		{
			return MapCreatorUtilities.Assets.LoadImage("MapCreator/Toolbar/SettingsToolIcon.png");
		}

		public override string GetLabel()
		{
			return "Settings";
		}

		private static void InitEditorModeToggle(VisualElement rootElement)
		{
			editorModeToggle = rootElement.Query<Toggle>(name: "toggle-enable-editor-mode");
			editorModeToggle.RegisterValueChangedCallback(evnt =>
			{
				if (mapComponent != null)
				{
					mapComponent.EditorModeEnabled = evnt.newValue;
					MapCreatorUtilities.MarkDirty();
				}
			});

			if (mapComponent != null)
			{
				editorModeToggle.value = mapComponent.EditorModeEnabled;
			}
		}

		private static void InitSceneViewDataFetchToggle(VisualElement rootElement)
		{
			editorModeToggle = rootElement.Query<Toggle>(name: "toggle-enable-editor-mode-data-fetch");
			editorModeToggle.RegisterValueChangedCallback(evnt =>
			{
				if (mapComponent != null)
				{
					mapComponent.DataFetchWithSceneView = evnt.newValue;
					MapCreatorUtilities.MarkDirty();
				}
			});

			if (mapComponent != null)
			{
				editorModeToggle.value = mapComponent.DataFetchWithSceneView;
			}
		}

		private static void InitSceneViewRebaseToggle(VisualElement rootElement)
		{
			editorModeToggle = rootElement.Query<Toggle>(name: "toggle-enable-editor-mode-rebase");
			editorModeToggle.RegisterValueChangedCallback(evnt =>
			{
				if (mapComponent != null)
				{
					mapComponent.RebaseWithSceneView = evnt.newValue;
					MapCreatorUtilities.MarkDirty();
				}
			});

			if (mapComponent != null)
			{
				editorModeToggle.value = mapComponent.RebaseWithSceneView;
			}
		}

		private static void InitMeshColliderToggle(VisualElement rootElement)
		{
			meshColliderToggle = rootElement.Query<Toggle>(name: "toggle-enable-mesh-colliders");
			meshColliderToggle.RegisterValueChangedCallback(evnt =>
			{
				if (mapComponent != null)
				{
					mapComponent.MeshCollidersEnabled = evnt.newValue;
					MapCreatorUtilities.MarkDirty();
				}
			});

			if (mapComponent != null)
			{
				meshColliderToggle.value = mapComponent.MeshCollidersEnabled;
			}
		}
	}
}

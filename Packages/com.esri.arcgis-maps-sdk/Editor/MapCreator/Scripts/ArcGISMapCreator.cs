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
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

namespace Esri.ArcGISMapsSDK.Editor.UI
{
	public abstract class ArcGISMapCreatorTool
	{
		public abstract VisualElement GetContent();
		public abstract Texture2D GetImage();
		public abstract string GetLabel();

		public virtual void OnDeselected() { }
		public virtual void OnDestroy() { }
		public virtual void OnEnable() { }
		public virtual void OnSelected() { }
	}

	public class ArcGISMapCreator : EditorWindow
	{
		private string activeTool;
		private VisualElement activeToolContent;
		private StyleSheet toolbarButtonStyle;
		private VisualTreeAsset toolbarButtonTemplate;

		private Dictionary<string, Button> buttons = new Dictionary<string, Button>();
		private Dictionary<string, ArcGISMapCreatorTool> tools = new Dictionary<string, ArcGISMapCreatorTool>();

		[MenuItem("ArcGIS Maps SDK/Map Creator", false, 1)]
		private static void CreateMapCreatorEditorWindow()
		{
			GetWindow<ArcGISMapCreator>().Show();
		}

		ArcGISMapCreator()
		{
			tools.Add("map-tool", new ArcGISMapCreatorMapTool());
			tools.Add("camera-tool", new ArcGISMapCreatorCameraTool());
			tools.Add("basemap-tool", new ArcGISMapCreatorBasemapTool());
			tools.Add("elevation-tool", new ArcGISMapCreatorElevationTool());
			tools.Add("layers-tool", new ArcGISMapCreatorLayerTool());
			tools.Add("auth-tool", new ArcGISMapCreatorAuthTool());
			tools.Add("settings-tool", new ArcGISMapCreatorSettingsTool());
			tools.Add("help-tool", new ArcGISMapCreatorHelpTool());
		}

		private void OnDisable()
		{
			EditorSceneManager.activeSceneChangedInEditMode -= OnSceneChange;
		}

		private void OnEnable()
		{
			titleContent.text = "ArcGISMapsSDK";
			titleContent.image = MapCreatorUtilities.Assets.LoadImage("MapCreator/MapCreatorIcon.png");

			minSize = new Vector2(456, 100);

			EditorSceneManager.activeSceneChangedInEditMode += OnSceneChange;

			foreach (var tool in tools)
			{
				tool.Value.OnEnable();
			}

			toolbarButtonTemplate = MapCreatorUtilities.Assets.LoadVisualTreeAsset("MapCreator/ToolbarButtonTemplate.uxml");
			toolbarButtonStyle = MapCreatorUtilities.Assets.LoadStyleSheet("MapCreator/ToolbarButtonStyle.uss");
		}

		private void OnSceneChange(Scene prev, Scene next)
		{
			if (activeTool == null || activeTool == "")
			{
				return;
			}

			SetActiveTool(IsToolEnabled(activeTool) ? activeTool : "map-tool");
		}

		private void OnHierarchyChange()
		{
			UpdateToolbar();

			if (!IsToolEnabled(activeTool))
			{
				SetActiveTool("map-tool");
			}
		}

		private VisualElement BuildButton(ArcGISMapCreatorTool tool)
		{
			var element = new VisualElement();

			toolbarButtonTemplate.CloneTree(element);

			element.styleSheets.Add(toolbarButtonStyle);

			var image = element.Q<Image>();
			var label = element.Q<Label>();

			image.image = tool.GetImage();
			label.text = tool.GetLabel();

			return element;
		}

		private VisualElement BuildToolbar()
		{
			var toolbarElement = new VisualElement();

			toolbarElement.AddToClassList("toolbar-box");

			foreach (var tool in tools)
			{
				var buttonElement = BuildButton(tool.Value);

				var button = buttonElement.Q<Button>();

				button.RegisterCallback<MouseUpEvent>(evnt =>
				{
					SetActiveTool(tool.Key);
				});

				buttons[tool.Key] = button;

				toolbarElement.Add(buttonElement);
			}

			return toolbarElement;
		}

		private void CreateGUI()
		{
			rootVisualElement.styleSheets.Add(MapCreatorUtilities.Assets.LoadStyleSheet("MapCreator/MapCreatorStyle.uss"));

			rootVisualElement.Add(BuildToolbar());

			UpdateToolbar();

			SetActiveTool(Application.isPlaying ? "help-tool" : "map-tool");
		}

		private bool IsToolEnabled(string tool)
		{
			bool editorNoMap = tool == "map-tool" && !Application.isPlaying;
			bool editorMap = MapCreatorUtilities.MapComponent != null && !Application.isPlaying;

			// Always enable the help tool, even in play mode.
			return editorNoMap || editorMap || tool == "help-tool";
		}

		internal ArcGISMapCreatorTool GetActiveTool()
		{
			return tools[activeTool];
		}

		internal Button GetButton(string tool)
		{
			if (!buttons.ContainsKey(tool))
			{
				return null;
			}

			return buttons[tool];
		}

		private void OnDestroy()
		{
			if (activeTool != null && activeTool != "")
			{
				tools[activeTool].OnDestroy();
			}
		}

		private void SetActiveTool(string tool)
		{
			if (activeTool != null && activeTool != "")
			{
				GetButton(activeTool)?.RemoveFromClassList("button-selected");

				if (activeToolContent != null && rootVisualElement.Contains(activeToolContent))
				{
					rootVisualElement.Remove(activeToolContent);
				}

				tools[activeTool].OnDeselected();
			}

			activeTool = tool;

			tools[activeTool].OnSelected();
			activeToolContent = tools[activeTool].GetContent();

			GetButton(activeTool)?.AddToClassList("button-selected");

			if (!rootVisualElement.Contains(activeToolContent))
			{
				rootVisualElement.Add(activeToolContent);
			}
		}

		private void UpdateToolbar()
		{
			foreach (var tool in tools)
			{
				var button = buttons[tool.Key];

				button.SetEnabled(IsToolEnabled(tool.Key));
			}
		}
	}
}

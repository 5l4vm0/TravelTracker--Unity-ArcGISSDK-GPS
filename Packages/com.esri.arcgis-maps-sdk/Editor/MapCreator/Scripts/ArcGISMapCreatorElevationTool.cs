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
using Esri.ArcGISMapsSDK.Editor.Utils;
using Esri.ArcGISMapsSDK.Security;
using Esri.GameEngine.Elevation.Base;
using Esri.HPFramework;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Esri.ArcGISMapsSDK.Editor.UI
{
	public class ArcGISMapCreatorElevationTool : ArcGISMapCreatorTool
	{
		[Serializable]
		internal class ElevationItem
		{
			public OAuthAuthenticationConfigurationMapping Authentication = OAuthAuthenticationConfigurationMapping.NoConfiguration;
			public bool IsUserDefined;
			public string Name;
			public string Source;
		}

		internal class Impl
		{
			internal const string DefaultElevationSource = "https://elevation3d.arcgis.com/arcgis/rest/services/WorldElevation3D/Terrain3D/ImageServer";

			internal IArcGISMapComponentInterface mapComponent;

			internal ArcGISElevationSourceInstanceData BuildInstanceDataFromWidgetItem(ArcGISMapCreatorElevationWidget.Item item)
			{
				var elevationSourceInstanceData = new ArcGISElevationSourceInstanceData();
				var elevationItem = item.UserData as ElevationItem;
				{
					elevationSourceInstanceData.Authentication = elevationItem.Authentication;
					elevationSourceInstanceData.IsEnabled = item.IsEnabled;
					elevationSourceInstanceData.Name = item.Name;
					elevationSourceInstanceData.Source = elevationItem.Source;
					elevationSourceInstanceData.Type = ArcGISElevationSourceType.ArcGISImageElevationSource;
				}
				return elevationSourceInstanceData;
			}

			internal void UpdateElevationSources(List<ArcGISMapCreatorElevationWidget.Item> elevationWidgetItems, ArcGISMeshModificationsInstanceData meshModifications)
			{
				if (mapComponent == null)
				{
					return;
				}

				var elevationSources = new List<ArcGISElevationSourceInstanceData>();

				foreach (var elevationWidgetItem in elevationWidgetItems)
				{
					var elevationItem = elevationWidgetItem.UserData as ElevationItem;

					if (elevationItem.IsUserDefined || elevationWidgetItem.IsEnabled)
					{
						elevationSources.Insert(0, BuildInstanceDataFromWidgetItem(elevationWidgetItem));
					}
				}

				var mapElevation = new ArcGISMapElevationInstanceData();

				mapElevation.ElevationSources = elevationSources;
				mapElevation.MeshModifications = meshModifications;

				mapComponent.MapElevation = mapElevation;

				MapCreatorUtilities.MarkDirty();
			}
		}

		internal class AddElevationWindow : EditorWindow
		{
			[SerializeField]
			private ElevationItem ElevationItem = new ElevationItem();

			public event Action<ElevationItem> OnElevationItemAdded;

			public void OnEnable()
			{
				titleContent = new GUIContent("Add New Elevation Source");

				maxSize = new Vector2(452, 102 + 21);
				minSize = maxSize;
			}

			private void CreateGUI()
			{
				rootVisualElement.styleSheets.Add(MapCreatorUtilities.Assets.LoadStyleSheet("MapCreator/AddCustomElevationWindowStyle.uss"));

				var template = MapCreatorUtilities.Assets.LoadVisualTreeAsset("MapCreator/AddCustomElevationWindowTemplate.uxml");

				template.CloneTree(rootVisualElement);

				var addButton = rootVisualElement.Q<Button>("add-button");
				var cancelButton = rootVisualElement.Q<Button>("cancel-button");

				addButton.clicked += () =>
				{
					OnElevationItemAdded?.Invoke(ElevationItem);

					Close();
				};

				cancelButton.clicked += () =>
				{
					Close();
				};

				rootVisualElement.Bind(new SerializedObject(this));
			}
		}

		private Impl implementation;
		internal Impl Implementation
		{
			get => implementation;
		}

		private VisualElement Content;
		private ArcGISMapCreatorElevationWidget.Item DefaultElevationWidgetItem;
		private ArcGISMapCreatorElevationWidget ElevationWidget;
		private List<ArcGISMapCreatorElevationWidget.Item> ElevationWidgetItems = new List<ArcGISMapCreatorElevationWidget.Item>();
		private Toggle EnableAllToggle;

		private Toggle enableMeshModificationsToggle;
		private ArcGISMeshModificationsInstanceData meshModifications;
		private ArcGISMeshModificationTool meshModificationTool;

		public override VisualElement GetContent()
		{
			return Content;
		}

		public static string GetDefaultElevation()
		{
			return Impl.DefaultElevationSource;
		}

		public override Texture2D GetImage()
		{
			return MapCreatorUtilities.Assets.LoadImage("MapCreator/Toolbar/ElevationToolIcon.png");
		}

		public override string GetLabel()
		{
			return "Elevation";
		}

		public override void OnDeselected()
		{
			meshModificationTool?.OnDisable();
		}

		public override void OnDestroy()
		{
			OnDeselected();
		}

		public override void OnEnable()
		{
			implementation = new Impl();

			Content = new VisualElement();

			Content.style.flexGrow = 1;

			Content.styleSheets.Add(MapCreatorUtilities.Assets.LoadStyleSheet("MapCreator/ElevationToolStyle.uss"));

			var elevationToolTemplate = MapCreatorUtilities.Assets.LoadVisualTreeAsset("MapCreator/ElevationToolTemplate.uxml");

			elevationToolTemplate.CloneTree(Content);

			var openMeshModificationsToolButton = Content.Q<VisualElement>("open-mesh-modifications-tool");

			openMeshModificationsToolButton.RegisterCallback<MouseUpEvent>(evnt =>
			{
				OpenMeshModificationsTool();
			});

			enableMeshModificationsToggle = Content.Q<Toggle>(name: "enable-mesh-modifications");

			enableMeshModificationsToggle.RegisterValueChangedCallback(evnt =>
			{
				meshModifications.IsEnabled = evnt.newValue;

				PushElevationSettings();
			});

			ElevationWidget = Content.Q<ArcGISMapCreatorElevationWidget>();

			ElevationWidget.ElevationItems = ElevationWidgetItems;

			ElevationWidget.OnItemsChanged += ElevationWidget_OnItemsChanged;
			ElevationWidget.OnZoomTo += ElevationWidget_OnZoomTo;

			var addNewLabel = Content.Q<VisualElement>(className: "add-new");

			addNewLabel.RegisterCallback<MouseUpEvent>(evnt =>
			{
				var window = EditorWindow.GetWindow<AddElevationWindow>();

				window.OnElevationItemAdded += AddElevationWindow_OnElevationItemAdded;

				window.ShowModal();
			});

			EnableAllToggle = Content.Q<Toggle>(name: "enable-all");

			EnableAllToggle.RegisterValueChangedCallback(evnt =>
			{
				foreach (var elevationWidgetItem in ElevationWidgetItems)
				{
					elevationWidgetItem.IsEnabled = evnt.newValue;
				}

				PushElevationSettings();

				ElevationWidget.Rebuild();
			});
		}

		private void OnMeshModificationChanged(ArcGISMeshModificationsInstanceData meshModificationsInstanceData)
		{
			meshModifications = meshModificationsInstanceData;

			PushElevationSettings();
		}

		private async void ElevationWidget_OnZoomTo(ArcGISMapCreatorElevationWidget.Item item)
		{
			var mapComponent = GameObject.FindObjectOfType<ArcGISMapComponent>();

			if (mapComponent == null)
			{
				return;
			}

			var EditorCameraComponent = mapComponent.GetComponentInChildren<ArcGISEditorCameraComponent>();

			if (EditorCameraComponent == null || !EditorCameraComponent.enabled)
			{
				return;
			}

			var instanceData = implementation.BuildInstanceDataFromWidgetItem(item);

			bool success = await mapComponent.ZoomToElevationSource(EditorCameraComponent.gameObject, instanceData);

			if (!success)
			{
				return;
			}

			var hP = EditorCameraComponent.GetComponent<HPTransform>();
			SceneView.lastActiveSceneView.pivot = new Vector3(0, 0, 0);
			SceneView.lastActiveSceneView.LookAt(new Vector3(0, 0, 0), Quaternion.Euler(90, 0, 0));
			mapComponent.UniversePosition = hP.UniversePosition;
			SceneView.lastActiveSceneView.Repaint();
		}

		private void ElevationWidget_OnItemsChanged()
		{
			PushElevationSettings();

			UpdateEnableAllToggle();
		}

		private void AddElevationWindow_OnElevationItemAdded(ElevationItem elevationItem)
		{
			elevationItem.IsUserDefined = true;

			ElevationWidgetItems.Insert(0, new ArcGISMapCreatorElevationWidget.Item
			{
				CanBeRemoved = true,
				Image = MapCreatorUtilities.Assets.LoadImage("MapCreator/ElevationTool/Custom.png"),
				IsEnabled = true,
				Name = elevationItem.Name,
				UserData = elevationItem
			});

			ElevationWidget.Rebuild();

			PushElevationSettings();

			UpdateEnableAllToggle();
		}

		private void OpenMeshModificationsTool()
		{
			meshModificationTool.OnEnable(meshModifications);
		}

		private void UpdateEnableAllToggle()
		{
			var value = true;

			foreach (var elevationWidgetItem in ElevationWidgetItems)
			{
				if (!elevationWidgetItem.IsEnabled)
				{
					value = false;
					break;
				}
			}

			EnableAllToggle.SetValueWithoutNotify(value);
		}

		public override void OnSelected()
		{
			OnSelected(MapCreatorUtilities.MapComponent);
		}

		internal void OnSelected(IArcGISMapComponentInterface mapComponentInterface)
		{
			implementation.mapComponent = mapComponentInterface;

			PullElevationSettings();

			UpdateEnableAllToggle();

			var coordinatesConverter = new ArcGISCoordinatesConverter(mapComponentInterface);

			meshModificationTool = new ArcGISMeshModificationTool(null, OnMeshModificationChanged, coordinatesConverter);
		}

		private void PullElevationSettings()
		{
			DefaultElevationWidgetItem = new ArcGISMapCreatorElevationWidget.Item
			{
				CanBeRemoved = false,
				Image = MapCreatorUtilities.Assets.LoadImage("MapCreator/ElevationTool/Default.png"),
				IsEnabled = false,
				Name = "Terrain 3D",
				UserData = new ElevationItem
				{
					Name = "Terrain 3D",
					IsUserDefined = false,
					Source = GetDefaultElevation()
				}
			};

			ElevationWidgetItems.Clear();

			var mapComponent = implementation.mapComponent;

			if (mapComponent == null)
			{
				return;
			}

			bool wasDefaultElevationAdded = false;

			mapComponent.MapElevation?.ElevationSources?.ForEach(elevationSource =>
			{
				var isDefaultElevation = IsDefaultElevation(elevationSource.Source);

				var elevationWidgetItem = DefaultElevationWidgetItem;

				if (!isDefaultElevation)
				{
					elevationWidgetItem = new ArcGISMapCreatorElevationWidget.Item
					{
						CanBeRemoved = true,
						Image = MapCreatorUtilities.Assets.LoadImage("MapCreator/ElevationTool/Custom.png"),
						UserData = new ElevationItem
						{
							Source = elevationSource.Source
						}
					};
				}
				else
				{
					wasDefaultElevationAdded = true;
				}

				elevationWidgetItem.IsEnabled = elevationSource.IsEnabled;
				elevationWidgetItem.Name = elevationSource.Name;

				var elevationItem = elevationWidgetItem.UserData as ElevationItem;

				elevationItem.Authentication = elevationSource.Authentication;
				elevationItem.IsUserDefined = true;
				elevationItem.Name = elevationSource.Name;

				ElevationWidgetItems.Insert(0, elevationWidgetItem);
			});

			if (!wasDefaultElevationAdded)
			{
				ElevationWidgetItems.Add(DefaultElevationWidgetItem);
			}

			ElevationWidget.Rebuild();

			meshModifications = (ArcGISMeshModificationsInstanceData)mapComponent.MapElevation?.MeshModifications?.Clone();

			enableMeshModificationsToggle.value = meshModifications?.IsEnabled ?? true;
		}

		private void PushElevationSettings()
		{
			implementation.UpdateElevationSources(ElevationWidgetItems, meshModifications);
		}

		bool IsDefaultElevation(string url)
		{
			var elevationItem = DefaultElevationWidgetItem.UserData as ElevationItem;

			return elevationItem.Source.ToLower() == url.ToLower();
		}
	}
}

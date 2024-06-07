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
using Esri.ArcGISMapsSDK.Utils;
using Esri.GameEngine.Layers.Base;
using Esri.HPFramework;
using Esri.Unity;
using System;
using System.Threading.Tasks;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UIElements;

namespace Esri.ArcGISMapsSDK.Editor.UI
{
	public class ArcGISMapCreatorLayerTool : ArcGISMapCreatorTool
	{
		private class LayerContainer : ScriptableObject
		{
			public VisualElement row;
			public ArcGISLayerInstanceData layer;
		}

		[Serializable]
		private class LayerItem : ScriptableObject
		{
			public OAuthAuthenticationConfigurationMapping Authentication;
			public string Name;

			[FileSelector]
			public string Source;

			[EnumFilter(typeof(LayerTypes), (int)LayerTypes.ArcGISUnsupportedLayer, (int)LayerTypes.ArcGISUnknownLayer)]
			public LayerTypes Type;
		}

		private static ArcGISMapComponent mapComponent;

		private static VisualTreeAsset rowTemplate;
		private static VisualTreeAsset optionsTemplate;

		private static VisualElement rootElement;
		private static VisualElement layerHolder;
		private static VisualElement optionsMenuHolder;

		private static LayerContainer currentContainer;
		private static TextField currentNameField;
		private static Label currentNameLabel;

		[SerializeField]
		private static LayerItem layerItem;

		private static bool isMeshModificationToolActive = false;
		private static bool isSpatialFeatureFilterToolActive = false;
		private static ArcGISMeshModificationTool meshModificationTool;
		private static ArcGISSpatialFeatureFilterTool spatialFeatureFilterTool;
		private static SerializedProperty sketchToolSerializedProperty;

		public override VisualElement GetContent()
		{
			rootElement = new VisualElement();
			rootElement.name = "ArcGISMapCreatorLayerTool";

			mapComponent = (ArcGISMapComponent)MapCreatorUtilities.MapComponent;

			rootElement.styleSheets.Add(MapCreatorUtilities.Assets.LoadStyleSheet("MapCreator/LayerStyle.uss"));

			var toolTemplate = MapCreatorUtilities.Assets.LoadVisualTreeAsset("MapCreator/LayerToolTemplate.uxml");
			toolTemplate.CloneTree(rootElement);

			rowTemplate = MapCreatorUtilities.Assets.LoadVisualTreeAsset("MapCreator/LayerRowTemplate.uxml");

			optionsTemplate = MapCreatorUtilities.Assets.LoadVisualTreeAsset("MapCreator/LayerOptionsTemplate.uxml");

			layerHolder = rootElement.Query<VisualElement>(name: "layers-holder");

			InitAddDataFields();
			CreateOptionsMenu();
			CreateMapComponentLayers();

			rootElement.Bind(new SerializedObject(layerItem));

			return rootElement;
		}

		public override Texture2D GetImage()
		{
			return MapCreatorUtilities.Assets.LoadImage("MapCreator/Toolbar/LayerToolIcon.png");
		}

		public override string GetLabel()
		{
			return "Layers";
		}

		private static void InitAddDataFields()
		{
			Button clearButton = rootElement.Query<Button>(name: "button-add-data-clear");
			clearButton.clickable.activators.Clear();
			clearButton.RegisterCallback<MouseDownEvent>(evnt => ClearAddDataFields());

			Button addButton = rootElement.Query<Button>(name: "button-add-data-add");
			addButton.clickable.activators.Clear();
			addButton.RegisterCallback<MouseDownEvent>(evnt =>
			{
				if (layerItem.Name == string.Empty || layerItem.Source == string.Empty)
				{
					Debug.LogWarning("Please provide a name and a source to create a new layer");
					return;
				}

				if (mapComponent == null)
				{
					Debug.LogWarning("Please add a map component to the scene to create a new layer");
					return;
				}

				var layer = new ArcGISLayerInstanceData();

				layer.Authentication = layerItem.Authentication;
				layer.IsVisible = true;
				layer.Name = layerItem.Name;
				layer.Opacity = 1.0f;
				layer.Source = layerItem.Source;
				layer.Type = layerItem.Type;

				mapComponent.Layers.Add(layer);
				mapComponent.UpdateLayers();
				MapCreatorUtilities.MarkDirty();

				CreateLayerRow(layer);
				ClearAddDataFields();
			});
		}

		private static void ClearAddDataFields()
		{
			layerItem.Authentication = OAuthAuthenticationConfigurationMapping.NoConfiguration;
			layerItem.Name = string.Empty;
			layerItem.Source = string.Empty;
			layerItem.Type = LayerTypes.ArcGISImageLayer;
		}

		private static void CreateMapComponentLayers()
		{
			if (mapComponent == null)
			{
				return;
			}

			foreach (var layer in mapComponent.Layers)
			{
				CreateLayerRow(layer);
			}
		}

		public override void OnDeselected()
		{
			meshModificationTool?.OnDisable();
			spatialFeatureFilterTool?.OnDisable();

			isMeshModificationToolActive = false;
			isSpatialFeatureFilterToolActive = false;
		}

		public override void OnDestroy()
		{
			OnDeselected();
		}

		public override void OnSelected()
		{
			layerItem = ScriptableObject.CreateInstance<LayerItem>();

			var coordinatesConverter = new ArcGISCoordinatesConverter((ArcGISMapComponent)MapCreatorUtilities.MapComponent);

			meshModificationTool = new ArcGISMeshModificationTool(OnSketchToolClosed, OnMeshModificationChanged, coordinatesConverter);
			spatialFeatureFilterTool = new ArcGISSpatialFeatureFilterTool(OnSketchToolClosed, OnSpatialFeatureFilterChanged, coordinatesConverter);
		}

		private static void ResetLayers()
		{
			layerHolder.Clear();
			CreateMapComponentLayers();
			mapComponent.UpdateLayers();
			MapCreatorUtilities.MarkDirty();
		}

		private static void CreateLayerRow(ArcGISLayerInstanceData layer)
		{
			var row = new VisualElement();
			row.name = "Layer Row";
			layerHolder.Insert(0, row);
			rowTemplate.CloneTree(row);

			var container = ScriptableObject.CreateInstance<LayerContainer>();

			container.layer = layer;
			container.row = row;

			SetEnableToggle(container);
			SetLayerNameHolder(container);

			RegisterValueChangeCallback(container.row, "authentication-field");
			RegisterValueChangeCallback(container.row, "mesh-modifications-field");
			RegisterValueChangeCallback(container.row, "opacity-field");
			RegisterValueChangeCallback(container.row, "source-field");
			RegisterValueChangeCallback(container.row, "spatial-feature-filter-field");
			RegisterValueChangeCallback(container.row, "type-field", () =>
			{
				UpdateBSLButtonVisibility(layer, container.row);
				UpdateFieldVisibility(row, layer);
			});
			RegisterValueChangeCallback(container.row, "building-filter-field");

			UpdateBSLButtonVisibility(layer, container.row);
			UpdateFieldVisibility(row, layer);

			row.Bind(new SerializedObject(container));
		}

		private static void UpdateBSLButtonVisibility(ArcGISLayerInstanceData layer, VisualElement row)
		{
			row.Q<PropertyField>(name: "building-filter-field").style.display = layer.Type == LayerTypes.ArcGISBuildingSceneLayer ? DisplayStyle.Flex : DisplayStyle.None;
		}

		internal void OpenPropertyEditTool(SerializedProperty serializedProperty)
		{
			rootElement.SetEnabled(false);

			var value = serializedProperty.GetValue();

			var isMeshModifications = value is ArcGISMeshModificationsInstanceData;
			var isSpatialFeatureFilter = value is ArcGISSpatialFeatureFilterInstanceData;

			Assert.IsTrue(isMeshModifications || isSpatialFeatureFilter);

			if (isMeshModificationToolActive)
			{
				meshModificationTool.OnDisable();

				isMeshModificationToolActive = false;
			}
			else if (isSpatialFeatureFilterToolActive)
			{
				spatialFeatureFilterTool.OnDisable();

				isSpatialFeatureFilterToolActive = false;
			}

			sketchToolSerializedProperty = serializedProperty;

			isMeshModificationToolActive = isMeshModifications;
			isSpatialFeatureFilterToolActive = isSpatialFeatureFilter;

			if (isMeshModificationToolActive)
			{
				meshModificationTool.OnEnable((ArcGISMeshModificationsInstanceData)value);
			}
			else if (isSpatialFeatureFilterToolActive)
			{
				spatialFeatureFilterTool.OnEnable((ArcGISSpatialFeatureFilterInstanceData)value);
			}
		}

		private static void OnSketchToolClosed()
		{
			isMeshModificationToolActive = false;
			isSpatialFeatureFilterToolActive = false;

			sketchToolSerializedProperty = null;

			rootElement.SetEnabled(true);
		}

		private static void OnMeshModificationChanged(ArcGISMeshModificationsInstanceData meshModificationsInstanceData)
		{
			sketchToolSerializedProperty.SetValue(meshModificationsInstanceData);
			sketchToolSerializedProperty.serializedObject.ApplyModifiedProperties();

			mapComponent.UpdateLayers();
			MapCreatorUtilities.MarkDirty();
		}

		private static void OnSpatialFeatureFilterChanged(ArcGISSpatialFeatureFilterInstanceData spatialFeatureFilterInstanceData)
		{
			sketchToolSerializedProperty.SetValue(spatialFeatureFilterInstanceData);
			sketchToolSerializedProperty.serializedObject.ApplyModifiedProperties();

			mapComponent.UpdateLayers();
			MapCreatorUtilities.MarkDirty();
		}

		private static void SetLayerNameHolder(LayerContainer container)
		{
			TextField nameField = container.row.Query<TextField>(className: "layer-name-text");
			Label nameLabel = container.row.Query<Label>(className: "layer-name-label");

			SetLayerOptionsButton(container, nameField, nameLabel);

			nameLabel.RegisterCallback<MouseDownEvent>(evnt =>
			{
				evnt.StopImmediatePropagation();
				EnableLayerNameTextField(nameLabel, nameField, container.layer.Name);
			});

			nameField.RegisterCallback<FocusInEvent>(evnt =>
			{
				nameField.SelectAll();
			});

			nameField.RegisterCallback<FocusOutEvent>(evnt =>
			{
				var index = mapComponent.Layers.IndexOf(container.layer);
				if (container.layer.Name != nameField.value)
				{
					EditorUtility.SetDirty(mapComponent);
				}
				container.layer.Name = nameField.value;
				mapComponent.Layers[index] = container.layer;
				mapComponent.UpdateLayers();
				MapCreatorUtilities.MarkDirty();
				SetLayerNameLabel(nameLabel, nameField, AdjustStringSize(container.layer.Name));
			});

			nameField.RegisterCallback<KeyDownEvent>(evnt =>
			{
				if (evnt.keyCode != KeyCode.Return)
				{
					return;
				}

				evnt.StopImmediatePropagation();
				var index = mapComponent.Layers.IndexOf(container.layer);
				if (container.layer.Name != nameField.value)
				{
					EditorUtility.SetDirty(mapComponent);
				}
				container.layer.Name = nameField.value;
				mapComponent.Layers[index] = container.layer;
				mapComponent.UpdateLayers();
				MapCreatorUtilities.MarkDirty();
				SetLayerNameLabel(nameLabel, nameField, AdjustStringSize(container.layer.Name));
			});

			nameField.visible = false;
			nameField.style.display = DisplayStyle.None;
			nameLabel.text = AdjustStringSize(container.layer.Name);

			VisualElement layerNameHolder = container.row.Query<VisualElement>(className: "layer-name-holder");
			VisualElement foldoutToggle = container.row.Query<VisualElement>(className: "unity-foldout__input");
			foldoutToggle.Add(layerNameHolder);
		}

		private static void EnableLayerNameTextField(Label layerNameLabel, TextField layerNameField, string textValue)
		{
			layerNameLabel.style.display = DisplayStyle.None;

			layerNameField.visible = true;
			layerNameField.style.display = DisplayStyle.Flex;
			layerNameField.SetValueWithoutNotify(textValue);
		}

		private static void SetLayerNameLabel(Label layerNameLabel, TextField layerNameField, string textValue)
		{
			layerNameLabel.style.display = DisplayStyle.Flex;
			layerNameLabel.text = textValue;

			layerNameField.visible = false;
			layerNameField.style.display = DisplayStyle.None;
		}

		private static void SetLayerOptionsButton(LayerContainer container, TextField nameField, Label nameLabel)
		{
			Button optionsButton = container.row.Query<Button>(name: "layer-options-button");
			optionsButton.clickable.activators.Clear();
			optionsButton.RegisterCallback<MouseDownEvent>(evnt =>
			{
				evnt.StopImmediatePropagation();
				currentContainer = container;
				currentNameField = nameField;
				currentNameLabel = nameLabel;
				OpenOptionsMenu(optionsButton);
			});
		}

		private static void UpdateFieldVisibility(VisualElement visualElement, ArcGISLayerInstanceData layerInstanceData)
		{
			var meshModificationsField = visualElement.Q<PropertyField>("mesh-modifications-field");
			var spatialFeatureFilterField = visualElement.Q<PropertyField>("spatial-feature-filter-field");

			meshModificationsField.style.display = layerInstanceData.Type == LayerTypes.ArcGISIntegratedMeshLayer ? DisplayStyle.Flex : DisplayStyle.None;
			spatialFeatureFilterField.style.display = layerInstanceData.Type == LayerTypes.ArcGISBuildingSceneLayer || layerInstanceData.Type == LayerTypes.ArcGIS3DObjectSceneLayer ? DisplayStyle.Flex : DisplayStyle.None;
		}

		private static void CreateOptionsMenu()
		{
			optionsMenuHolder = new VisualElement();
			optionsMenuHolder.AddToClassList("layer-options-dropdown");
			optionsTemplate.CloneTree(optionsMenuHolder);

			rootElement.Add(optionsMenuHolder);

			Button moveUpButton = optionsMenuHolder.Query<Button>(name: "layer-options-move-up");
			moveUpButton.clickable.activators.Clear();
			moveUpButton.RegisterCallback<MouseDownEvent>(evnt =>
			{
				MoveLayerUp();
				CloseOptionsMenu();
			});

			Button moveDownButton = optionsMenuHolder.Query<Button>(name: "layer-options-move-down");
			moveDownButton.clickable.activators.Clear();
			moveDownButton.RegisterCallback<MouseDownEvent>(evnt =>
			{
				MoveLayerDown();
				CloseOptionsMenu();
			});

			Button renameButton = optionsMenuHolder.Query<Button>(name: "layer-options-rename");
			renameButton.clickable.activators.Clear();
			renameButton.RegisterCallback<MouseDownEvent>(evnt =>
			{
				EnableLayerNameTextField(currentNameLabel, currentNameField, currentContainer.layer.Name);
				CloseOptionsMenu();
			});

			Button removeButton = optionsMenuHolder.Query<Button>(name: "layer-options-remove");
			removeButton.clickable.activators.Clear();
			removeButton.RegisterCallback<MouseDownEvent>(evnt =>
			{
				RemoveLayer();
				CloseOptionsMenu();
			});

			Button zoomButton = optionsMenuHolder.Query<Button>(name: "layer-options-zoom");
			zoomButton.clickable.activators.Clear();
			zoomButton.RegisterCallback<MouseDownEvent>(async evnt =>
			{
				CloseOptionsMenu();
				await ZoomToLayer();
			});

			Button copyButton = optionsMenuHolder.Query<Button>(name: "layer-options-copy");
			copyButton.clickable.activators.Clear();
			copyButton.RegisterCallback<MouseDownEvent>(evnt =>
			{
				EditorGUIUtility.systemCopyBuffer = currentContainer.layer.Source;
				Debug.Log("Layer source copied to clipboard: " + currentContainer.layer.Source);
				CloseOptionsMenu();
			});

			CloseOptionsMenu();
		}

		private static void OpenOptionsMenu(Button optionsButton)
		{
			optionsMenuHolder.visible = !optionsMenuHolder.visible;

			if (optionsMenuHolder.visible)
			{
				optionsMenuHolder.style.display = DisplayStyle.Flex;
				optionsMenuHolder.transform.position = new Vector3(-15, 0, 0);
				var y = optionsButton.worldBound.y - optionsMenuHolder.worldBound.y;
				optionsMenuHolder.transform.position = new Vector3(-15, y + 17, 0);
			}
			else
			{
				CloseOptionsMenu();
			}
		}

		private static void RegisterValueChangeCallback(VisualElement visualElement, string fieldName, Action beforeUpdate = null)
		{
			var propertyField = visualElement.Q<PropertyField>(name: fieldName);

			propertyField.RegisterValueChangeCallback(evnt =>
			{
				beforeUpdate?.Invoke();

				mapComponent.UpdateLayers();

				MapCreatorUtilities.MarkDirty();
			});
		}

		private static void CloseOptionsMenu()
		{
			optionsMenuHolder.visible = false;
			optionsMenuHolder.style.display = DisplayStyle.None;
		}

		private static void MoveLayerDown()
		{
			var layerIndex = mapComponent.Layers.IndexOf(currentContainer.layer);

			if (layerIndex == 0)
			{
				return;
			}

			mapComponent.Layers.RemoveAt(layerIndex);
			mapComponent.Layers.Insert(layerIndex - 1, currentContainer.layer);
			MapCreatorUtilities.MarkDirty();
			ResetLayers();
		}

		private static void MoveLayerUp()
		{
			var layerIndex = mapComponent.Layers.IndexOf(currentContainer.layer);

			if (layerIndex >= layerHolder.childCount - 1)
			{
				return;
			}

			mapComponent.Layers.RemoveAt(layerIndex);
			mapComponent.Layers.Insert(layerIndex + 1, currentContainer.layer);
			MapCreatorUtilities.MarkDirty();
			ResetLayers();
		}

		private static void RemoveLayer()
		{
			layerHolder.Remove(currentContainer.row);
			mapComponent.Layers.Remove(currentContainer.layer);
			mapComponent.UpdateLayers();
			MapCreatorUtilities.MarkDirty();

			currentContainer = null;
			currentNameField = null;
			currentNameLabel = null;
		}

		private static async Task<bool> ZoomToLayer()
		{
			var Layers = mapComponent.Map.Layers;

			if (Layers.GetSize() == 0)
			{
				return false;
			}

			ArcGISLayer layer = null;

			for (ulong i = 0; i < Layers.GetSize(); i++)
			{
				var testLayer = Layers.At(i);

				if (testLayer.Source == currentContainer.layer.Source && testLayer.ObjectType == (ArcGISLayerType)currentContainer.layer.Type)
				{
					layer = testLayer;
					break;
				}
			}

			if (layer == null)
			{
				return false;
			}

#if UNITY_EDITOR
			if (!Application.isPlaying)
			{
				var EditorCameraComponent = mapComponent.GetComponentInChildren<ArcGISEditorCameraComponent>();

				if (EditorCameraComponent != null && EditorCameraComponent.enabled)
				{
					bool success = await mapComponent.ZoomToLayer(EditorCameraComponent.gameObject, layer);

					if (!success)
					{
						return false;
					}

					var hP = EditorCameraComponent.GetComponent<HPTransform>();
					SceneView.lastActiveSceneView.pivot = new Vector3(0, 0, 0);
					SceneView.lastActiveSceneView.LookAt(new Vector3(0, 0, 0), Quaternion.Euler(90, 0, 0));
					mapComponent.UniversePosition = hP.UniversePosition;
					SceneView.lastActiveSceneView.Repaint();

					return true;
				}
			}
#endif

			return false;
		}

		private static void SetEnableToggle(LayerContainer container)
		{
			Toggle enableToggle = container.row.Query<Toggle>(name: "layer-enable-toggle");
			enableToggle.RegisterValueChangedCallback(evnt =>
			{
				if (enableToggle.value == true)
				{
					var index = mapComponent.Layers.IndexOf(container.layer);
					container.layer.IsVisible = true;
					mapComponent.Layers[index] = container.layer;
				}
				else
				{
					var index = mapComponent.Layers.IndexOf(container.layer);
					container.layer.IsVisible = false;
					mapComponent.Layers[index] = container.layer;
				}
				mapComponent.UpdateLayers();
				MapCreatorUtilities.MarkDirty();
			});

			var enabled = container.layer.IsVisible;
			enableToggle.SetValueWithoutNotify(enabled);
		}

		private static string AdjustStringSize(string original)
		{
			if (original.Length > 20)
			{
				return original.Substring(0, 20) + " ...";
			}
			return original;
		}
	}
}

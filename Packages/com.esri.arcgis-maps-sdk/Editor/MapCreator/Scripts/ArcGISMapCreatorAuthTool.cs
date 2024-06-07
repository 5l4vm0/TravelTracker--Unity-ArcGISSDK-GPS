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
using UnityEngine;
using UnityEngine.UIElements;

namespace Esri.ArcGISMapsSDK.Editor.UI
{
	public class ArcGISMapCreatorAuthTool : ArcGISMapCreatorTool
	{
		private class ConfigContainer
		{
			public ConfigContainer() { }

			public ConfigContainer(VisualElement row, ArcGISAuthenticationConfigurationInstanceData data)
			{
				this.row = row;
				this.data = data;
			}

			public VisualElement row;
			public ArcGISAuthenticationConfigurationInstanceData data;
		}

		private static IArcGISMapComponentInterface mapComponent;

		private static VisualTreeAsset rowTemplate;
		private static VisualTreeAsset optionsTemplate;

		private static VisualElement rootElement;
		private static VisualElement configHolder;
		private static VisualElement optionsMenuHolder;

		private static TextField addDataClientField;
		private static TextField addDataUriField;
		private static TextField addDataNameField;

		private static ConfigContainer currentContainer;
		private static TextField currentNameField;
		private static Label currentNameLabel;

		public override VisualElement GetContent()
		{
			rootElement = new VisualElement();
			rootElement.name = "ArcGISMapCreatorAuthTool";

			mapComponent = MapCreatorUtilities.MapComponent;

			rootElement.styleSheets.Add(MapCreatorUtilities.Assets.LoadStyleSheet("MapCreator/LayerStyle.uss"));

			var toolTemplate = MapCreatorUtilities.Assets.LoadVisualTreeAsset("MapCreator/AuthToolTemplate.uxml");
			toolTemplate.CloneTree(rootElement);

			rowTemplate = MapCreatorUtilities.Assets.LoadVisualTreeAsset("MapCreator/AuthRowTemplate.uxml");

			optionsTemplate = MapCreatorUtilities.Assets.LoadVisualTreeAsset("MapCreator/AuthOptionsTemplate.uxml");

			configHolder = rootElement.Query<VisualElement>(name: "config-holder");

			InitApiKeyField();
			InitAddDataFields();
			CreateOptionsMenu();
			ResetConfigs();

			return rootElement;
		}

		public override Texture2D GetImage()
		{
			return MapCreatorUtilities.Assets.LoadImage("MapCreator/Toolbar/AuthToolIcon.png");
		}

		public override string GetLabel()
		{
			return "Auth";
		}

		private static void InitApiKeyField()
		{
			TextField apiKeyField = rootElement.Query<TextField>(name: "api-key-text");
			apiKeyField.RegisterValueChangedCallback(evnt =>
			{
				if (mapComponent != null)
				{
					mapComponent.APIKey = evnt.newValue;
					MapCreatorUtilities.MarkDirty();
				}
			});

			if (mapComponent != null)
			{
				apiKeyField.SetValueWithoutNotify(mapComponent.APIKey);
			}

			Label apiKeyDocLink = rootElement.Query<Label>(name: "api-key-documentation-link");
			apiKeyDocLink.RegisterCallback<MouseDownEvent>(evnt =>
			{
				Application.OpenURL(ArcGISMapCreatorHelpTool.URL_GetAPIKey);
			});
		}

		private static void InitAddDataFields()
		{
			addDataNameField = rootElement.Query<TextField>(name: "add-data-auth-name-text");
			addDataClientField = rootElement.Query<TextField>(name: "add-data-auth-client-text");
			addDataUriField = rootElement.Query<TextField>(name: "add-data-auth-uri-text");

			Button clearButton = rootElement.Query<Button>(name: "button-add-data-clear");
			clearButton.clickable.activators.Clear();
			clearButton.RegisterCallback<MouseDownEvent>(evnt => ClearAddDataFields());

			Button addButton = rootElement.Query<Button>(name: "button-add-data-add");
			addButton.clickable.activators.Clear();
			addButton.RegisterCallback<MouseDownEvent>(evnt =>
			{
				if (addDataNameField.value == string.Empty || addDataClientField.value == string.Empty || addDataUriField.value == string.Empty)
				{
					Debug.LogWarning("Please provide a name, client ID, and redirect URI to create a new configuration");
					return;
				}

				if (mapComponent == null)
				{
					Debug.LogWarning("Please add a map component to the scene to create a new configuration");
					return;
				}

				ArcGISAuthenticationConfigurationInstanceData data = new ArcGISAuthenticationConfigurationInstanceData();
				data.Name = addDataNameField.value;
				data.ClientID = addDataClientField.value;
				data.RedirectURI = addDataUriField.value;

				mapComponent.Configurations.Add(data);
				MapCreatorUtilities.MarkDirty();

				CreateConfigRow(data);
				ClearAddDataFields();
			});
		}

		private static void ClearAddDataFields()
		{
			addDataNameField.SetValueWithoutNotify(string.Empty);
			addDataClientField.SetValueWithoutNotify(string.Empty);
			addDataUriField.SetValueWithoutNotify(string.Empty);
		}

		private static void CreateConfigRows()
		{
			if (mapComponent == null)
			{
				return;
			}

			foreach (var data in mapComponent.Configurations)
			{
				CreateConfigRow(data);
			}
		}

		private static void ResetConfigs()
		{
			configHolder.Clear();
			CreateConfigRows();
		}

		private static void CreateConfigRow(ArcGISAuthenticationConfigurationInstanceData data)
		{
			var row = new VisualElement();
			row.name = "Configuration";
			configHolder.Add(row);
			rowTemplate.CloneTree(row);

			var container = new ConfigContainer(row, data);

			Foldout foldout = row.Query<Foldout>(className: "foldout-layer-row");
			foldout.value = false;

			SetConfigNameHolder(container);
			SetNameField(container);
			SetClientField(container);
			SetUriField(container);
		}

		private static void SetConfigNameHolder(ConfigContainer container)
		{
			TextField nameField = container.row.Query<TextField>(className: "layer-name-text");
			Label nameLabel = container.row.Query<Label>(className: "layer-name-label");

			SetConfigOptionsButton(container, nameField, nameLabel);

			nameLabel.RegisterCallback<MouseDownEvent>(evnt =>
			{
				evnt.StopImmediatePropagation();
				EnableConfigNameTextField(nameLabel, nameField);
			});

			nameField.RegisterCallback<FocusInEvent>(evnt =>
			{
				nameField.SelectAll();
			});

			nameField.RegisterCallback<FocusOutEvent>(evnt =>
			{
				var index = mapComponent.Configurations.IndexOf(container.data);
				container.data.Name = nameField.value;
				mapComponent.Configurations[index] = container.data;
				MapCreatorUtilities.MarkDirty();
				SetConfigNameLabel(nameLabel, nameField);
			});

			nameField.RegisterCallback<KeyDownEvent>(evnt =>
			{
				if (evnt.keyCode != KeyCode.Return)
				{
					return;
				}

				evnt.StopImmediatePropagation();
				var index = mapComponent.Configurations.IndexOf(container.data);
				container.data.Name = nameField.value;
				mapComponent.Configurations[index] = container.data;
				MapCreatorUtilities.MarkDirty();
				SetConfigNameLabel(nameLabel, nameField);
			});

			nameField.visible = false;
			nameField.style.display = DisplayStyle.None;
			nameLabel.text = container.data.Name;

			VisualElement configNameHolder = container.row.Query<VisualElement>(className: "layer-name-holder");
			VisualElement foldoutToggle = container.row.Query<VisualElement>(className: "unity-foldout__input");
			foldoutToggle.Add(configNameHolder);
		}

		private static void EnableConfigNameTextField(Label nameLabel, TextField nameField)
		{
			nameLabel.style.display = DisplayStyle.None;

			nameField.visible = true;
			nameField.style.display = DisplayStyle.Flex;
			nameField.SetValueWithoutNotify(nameLabel.text);
		}

		private static void SetConfigNameLabel(Label nameLabel, TextField nameField)
		{
			nameLabel.style.display = DisplayStyle.Flex;
			nameLabel.text = nameField.text;

			nameField.visible = false;
			nameField.style.display = DisplayStyle.None;
		}

		private static void SetConfigOptionsButton(ConfigContainer container, TextField nameField, Label nameLabel)
		{
			Button optionsButton = container.row.Query<Button>(name: "config-options-button");
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

		private static void CreateOptionsMenu()
		{
			optionsMenuHolder = new VisualElement();
			optionsMenuHolder.AddToClassList("layer-options-dropdown");
			optionsMenuHolder.AddToClassList("layer-options-dropdown-auth");
			optionsTemplate.CloneTree(optionsMenuHolder);

			rootElement.Add(optionsMenuHolder);

			Button moveUpButton = optionsMenuHolder.Query<Button>(name: "config-options-move-up");
			moveUpButton.clickable.activators.Clear();
			moveUpButton.RegisterCallback<MouseDownEvent>(evnt =>
			{
				MoveLayerUp();
				CloseOptionsMenu();
			});

			Button moveDownButton = optionsMenuHolder.Query<Button>(name: "config-options-move-down");
			moveDownButton.clickable.activators.Clear();
			moveDownButton.RegisterCallback<MouseDownEvent>(evnt =>
			{
				MoveLayerDown();
				CloseOptionsMenu();
			});

			Button renameButton = optionsMenuHolder.Query<Button>(name: "config-options-rename");
			renameButton.clickable.activators.Clear();
			renameButton.RegisterCallback<MouseDownEvent>(evnt =>
			{
				EnableConfigNameTextField(currentNameLabel, currentNameField);
				CloseOptionsMenu();
			});

			Button removeButton = optionsMenuHolder.Query<Button>(name: "config-options-remove");
			removeButton.clickable.activators.Clear();
			removeButton.RegisterCallback<MouseDownEvent>(evnt =>
			{
				RemoveLayer();
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

		private static void CloseOptionsMenu()
		{
			optionsMenuHolder.visible = false;
			optionsMenuHolder.style.display = DisplayStyle.None;
		}

		private static void MoveLayerUp()
		{
			var configIndex = mapComponent.Configurations.IndexOf(currentContainer.data);

			if (configIndex == 0)
			{
				return;
			}

			mapComponent.Configurations.RemoveAt(configIndex);
			mapComponent.Configurations.Insert(configIndex - 1, currentContainer.data);
			MapCreatorUtilities.MarkDirty();
			ResetConfigs();
		}

		private static void MoveLayerDown()
		{
			var configIndex = mapComponent.Configurations.IndexOf(currentContainer.data);

			if (configIndex >= configHolder.childCount - 1)
			{
				return;
			}

			mapComponent.Configurations.RemoveAt(configIndex);
			mapComponent.Configurations.Insert(configIndex + 1, currentContainer.data);
			MapCreatorUtilities.MarkDirty();
			ResetConfigs();
		}

		private static void RemoveLayer()
		{
			configHolder.Remove(currentContainer.row);
			mapComponent.Configurations.Remove(currentContainer.data);
			MapCreatorUtilities.MarkDirty();

			currentContainer = null;
			currentNameField = null;
			currentNameLabel = null;
		}

		private static void SetNameField(ConfigContainer container)
		{
			TextField field = container.row.Query<TextField>(name: "config-name-text");
			field.RegisterValueChangedCallback(evnt =>
			{
				var index = mapComponent.Configurations.IndexOf(container.data);
				container.data.Name = evnt.newValue;
				mapComponent.Configurations[index] = container.data;
				MapCreatorUtilities.MarkDirty();
			});

			field.SetValueWithoutNotify(container.data.Name);
		}

		private static void SetClientField(ConfigContainer container)
		{
			TextField field = container.row.Query<TextField>(name: "config-client-text");
			field.RegisterValueChangedCallback(evnt =>
			{
				var index = mapComponent.Configurations.IndexOf(container.data);
				container.data.ClientID = evnt.newValue;
				mapComponent.Configurations[index] = container.data;
				MapCreatorUtilities.MarkDirty();
			});

			field.SetValueWithoutNotify(container.data.ClientID);
		}

		private static void SetUriField(ConfigContainer container)
		{
			TextField sourceField = container.row.Query<TextField>(name: "config-uri-text");
			sourceField.RegisterValueChangedCallback(evnt =>
			{
				var index = mapComponent.Configurations.IndexOf(container.data);
				container.data.RedirectURI = evnt.newValue;
				mapComponent.Configurations[index] = container.data;
				MapCreatorUtilities.MarkDirty();
			});

			sourceField.SetValueWithoutNotify(container.data.RedirectURI);
		}
	}
}

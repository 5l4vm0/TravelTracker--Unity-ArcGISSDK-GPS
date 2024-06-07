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
using System;
using System.Collections.Generic;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Esri.ArcGISMapsSDK.Editor.UI
{
	public class ArcGISMapCreatorElevationWidget : VisualElement
	{
		public new class UxmlFactory : UxmlFactory<ArcGISMapCreatorElevationWidget> { }

		public class Item
		{
			public bool CanBeRemoved;
			public Texture2D Image;
			public bool IsEnabled;
			public string Name;
			public object UserData;
		}

		private VisualTreeAsset ElevationItemTemplate;
		private ListView ListView;

		private List<Item> elevationItems;
		public List<Item> ElevationItems
		{
			get
			{
				return elevationItems;
			}
			set
			{
				if (elevationItems != value)
				{
					elevationItems = value;

					ListView.itemsSource = elevationItems;
				}
			}
		}

		public event Action OnItemsChanged;
		public event Action<Item> OnZoomTo;

		public ArcGISMapCreatorElevationWidget()
		{
			styleSheets.Add(MapCreatorUtilities.Assets.LoadStyleSheet("MapCreator/ElevationCardStyle.uss"));

			ElevationItemTemplate = MapCreatorUtilities.Assets.LoadVisualTreeAsset("MapCreator/ElevationCardTemplate.uxml");

			SetupListView();
		}

		public void Rebuild()
		{
			ListView.Rebuild();
		}

		private void SetupListView()
		{
			ListView = new ListView();

			ListView.selectionType = SelectionType.None;

			ListView.makeItem = () =>
			{
				var element = new VisualElement();

				ElevationItemTemplate.CloneTree(element);

				var image = element.Q<Image>();

				image.scaleMode = ScaleMode.ScaleAndCrop;

				return element;
			};

			ListView.bindItem = (element, index) =>
			{
				var elevationItem = elevationItems[index];

				element.userData = elevationItem;

				var image = element.Q<Image>();
				var label = element.Q<Label>();
				var textField = element.Q<TextField>();
				var toggle = element.Q<Toggle>();
				var toolbarMenu = element.Q<ToolbarMenu>();

				textField.RegisterCallback<BlurEvent>(evnt =>
				{
					var centerVisualElement = element.Q<VisualElement>(className: "center");

					centerVisualElement.RemoveFromClassList("rename");

					elevationItem.Name = textField.value;
					label.text = textField.value;

					OnItemsChanged?.Invoke();
				});

				toggle.RegisterValueChangedCallback(evnt =>
				{
					elevationItem.IsEnabled = evnt.newValue;

					UpdateItemStyle(element);

					OnItemsChanged?.Invoke();
				});

				toolbarMenu.menu.AppendAction("Move up", action =>
				{
					var index = elevationItems.IndexOf(elevationItem);

					if (index == -1 || index < 1)
					{
						return;
					}

					var newIndex = index - 1;

					elevationItems.Remove(elevationItem);
					elevationItems.Insert(newIndex, elevationItem);

					Rebuild();

					OnItemsChanged?.Invoke();
				}, action =>
				{
					var index = elevationItems.IndexOf(elevationItem);

					if (index == -1 || index < 1)
					{
						return DropdownMenuAction.Status.Disabled;
					}

					return DropdownMenuAction.Status.Normal;
				});

				toolbarMenu.menu.AppendAction("Move down", action =>
				{
					var index = elevationItems.IndexOf(elevationItem);

					if (index == -1 || index > elevationItems.Count - 2)
					{
						return;
					}

					var newIndex = index + 1;

					elevationItems.Remove(elevationItem);
					elevationItems.Insert(newIndex, elevationItem);

					Rebuild();

					OnItemsChanged?.Invoke();
				}, action =>
				{
					var index = elevationItems.IndexOf(elevationItem);

					if (index == -1 || index > elevationItems.Count - 2)
					{
						return DropdownMenuAction.Status.Disabled;
					}

					return DropdownMenuAction.Status.Normal;
				});

				toolbarMenu.menu.AppendAction("Zoom to", action =>
				{
					OnZoomTo?.Invoke(elevationItem);
				});

				toolbarMenu.menu.AppendAction("Rename", action =>
				{
					var centerVisualElement = element.Q<VisualElement>(className: "center");

					centerVisualElement.AddToClassList("rename");

					textField.value = elevationItem.Name;

					textField.Focus();
				});

				toolbarMenu.menu.AppendAction("Remove", action =>
				{
					if (!elevationItem.CanBeRemoved)
					{
						return;
					}

					elevationItems.Remove(elevationItem);

					Rebuild();

					OnItemsChanged?.Invoke();
				}, action =>
				{
					if (!elevationItem.CanBeRemoved)
					{
						return DropdownMenuAction.Status.Disabled;
					}

					return DropdownMenuAction.Status.Normal;
				});

				image.image = elevationItem.Image;
				label.text = elevationItem.Name;
				textField.SetValueWithoutNotify(elevationItem.Name);
				toggle.SetValueWithoutNotify(elevationItem.IsEnabled);

				UpdateItemStyle(element);
			};

			ListView.unbindItem = (element, index) =>
			{
				var elevationItemVisualElement = element as VisualElement;

				elevationItemVisualElement.userData = null;
			};

			ListView.virtualizationMethod = CollectionVirtualizationMethod.DynamicHeight;

			Add(ListView);
		}

		private void UpdateItemStyle(VisualElement visualElement)
		{
			var elevationItem = (Item)visualElement.userData;

			if (elevationItem != null)
			{
				if (elevationItem.IsEnabled)
				{
					visualElement.hierarchy[0].RemoveFromClassList("disabled");
				}
				else
				{
					visualElement.hierarchy[0].AddToClassList("disabled");
				}
			}
		}
	}
}

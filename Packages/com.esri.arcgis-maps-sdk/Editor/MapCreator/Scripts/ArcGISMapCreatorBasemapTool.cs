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
using Esri.ArcGISMapsSDK.Security;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Esri.ArcGISMapsSDK.Editor.UI
{
	public class ArcGISMapCreatorBasemapTool : ArcGISMapCreatorTool
	{
		[Serializable]
		internal class BasemapItem
		{
			public OAuthAuthenticationConfigurationMapping Authentication;
			public string Name;
			public string Source;
			public BasemapTypes Type = BasemapTypes.Basemap;
		}

		internal class AddBasemapWindow : EditorWindow
		{
			[SerializeField]
			private BasemapItem BasemapItem = new BasemapItem();

			public event Action<BasemapItem> OnBasemapItemAdded;

			public void OnEnable()
			{
				titleContent = new GUIContent("Add New Basemap");

				maxSize = new Vector2(452, 102 + 21);
				minSize = maxSize;
			}

			private void CreateGUI()
			{
				rootVisualElement.styleSheets.Add(MapCreatorUtilities.Assets.LoadStyleSheet("MapCreator/AddCustomBasemapWindowStyle.uss"));

				var template = MapCreatorUtilities.Assets.LoadVisualTreeAsset("MapCreator/AddCustomBasemapWindowTemplate.uxml");

				template.CloneTree(rootVisualElement);

				var addButton = rootVisualElement.Q<Button>("add-button");
				var cancelButton = rootVisualElement.Q<Button>("cancel-button");

				addButton.clicked += () =>
				{
					OnBasemapItemAdded?.Invoke(BasemapItem);

					Close();
				};

				cancelButton.clicked += () =>
				{
					Close();
				};

				rootVisualElement.Bind(new SerializedObject(this));
			}
		}

		private List<ArcGISMapCreatorBasemapWidget.Item> BasemapWidgetItems = new List<ArcGISMapCreatorBasemapWidget.Item>();
		private ArcGISMapCreatorBasemapWidget BasemapWidget;
		private VisualElement Content;
		private ArcGISMapCreatorBasemapWidget.Item NoBasemapItem;
		private ArcGISMapCreatorBasemapWidget.Item SelectedItem;

		public override VisualElement GetContent()
		{
			return Content;
		}

		public static string GetDefaultBasemap()
		{
			return "https://www.arcgis.com/home/item.html?id=c7d2b5c334364e8fb5b73b0f4d6a779b";
		}

		public override Texture2D GetImage()
		{
			return MapCreatorUtilities.Assets.LoadImage("MapCreator/Toolbar/BasemapToolIcon.png");
		}

		public override string GetLabel()
		{
			return "Basemap";
		}

		private ArcGISMapCreatorBasemapWidget.Item BuildWidgetItem(string label, string imageName, BasemapTypes type, string url)
		{
			return new ArcGISMapCreatorBasemapWidget.Item
			{
				CanBeRemoved = false,
				ColorImage = MapCreatorUtilities.Assets.LoadImage($"MapCreator/BasemapTool/Color/{imageName}.png"),
				GrayscaleImage = MapCreatorUtilities.Assets.LoadImage($"MapCreator/BasemapTool/Grayscale/{imageName}.png"),
				Name = label,
				RequiresAPIKey = true,
				UserData = new BasemapItem
				{
					Name = label,
					Type = type,
					Source = url
				}
			};
		}

		public override void OnEnable()
		{
			#pragma warning disable format
			BasemapWidgetItems.Add(BuildWidgetItem("OpenStreetMap (Light Gray Canvas)",		"OpenStreetMapLightGrayCanvas",		BasemapTypes.Basemap, "https://www.arcgis.com/home/item.html?id=983b312ebd9b4d15a02e00f50acdb1c1"));
			BasemapWidgetItems.Add(BuildWidgetItem("Streets (with Relief)",					"StreetsWithRelief",				BasemapTypes.Basemap, "https://www.arcgis.com/home/item.html?id=03daad361e1849bc80cb7b70ed391379"));
			BasemapWidgetItems.Add(BuildWidgetItem("Imagery Hybrid",						"ImageryHybrid",					BasemapTypes.Basemap, "https://www.arcgis.com/home/item.html?id=ea3befe305494bb5b2acd77e1b3135dc"));
			BasemapWidgetItems.Add(BuildWidgetItem("Dark Gray Canvas",						"DarkGrayCanvas",					BasemapTypes.Basemap, "https://www.arcgis.com/home/item.html?id=7742cd5abef8497288dc81426266df9b"));
			BasemapWidgetItems.Add(BuildWidgetItem("Streets (Night)",						"StreetsNight",						BasemapTypes.Basemap, "https://www.arcgis.com/home/item.html?id=b22e146f927e413c92f75b5e4614354a"));
			BasemapWidgetItems.Add(BuildWidgetItem("Terrain with Labels",					"TerrainWithLabels",				BasemapTypes.Basemap, "https://www.arcgis.com/home/item.html?id=2ef1306b93c9459ca7c7b4f872c070b9"));
			BasemapWidgetItems.Add(BuildWidgetItem("Oceans",								"Oceans",							BasemapTypes.Basemap, "https://www.arcgis.com/home/item.html?id=b1dca7ef7b61466785901c41aed89ba5"));
			BasemapWidgetItems.Add(BuildWidgetItem("Navigation (Night)",					"NavigationNight",					BasemapTypes.Basemap, "https://www.arcgis.com/home/item.html?id=77073a29526046b89bb5622b6276e933"));
			BasemapWidgetItems.Add(BuildWidgetItem("OpenStreetMap (with Relief)",			"OpenStreetMapWithRelief",			BasemapTypes.Basemap, "https://www.arcgis.com/home/item.html?id=5b93370c7fc24ca3b8740abd2a55456a"));
			BasemapWidgetItems.Add(BuildWidgetItem("OpenStreetMap (Dark Gray Canvas)",		"OpenStreetMapDarkGrayCanvas",		BasemapTypes.Basemap, "https://www.arcgis.com/home/item.html?id=7371ca07df4047eaa5f02d09ef12b1a0"));
			BasemapWidgetItems.Add(BuildWidgetItem("Light Gray Canvas",						"LightGrayCanvas",					BasemapTypes.Basemap, "https://www.arcgis.com/home/item.html?id=0f74af7609054be8a29e0ba5f154f0a8"));
			BasemapWidgetItems.Add(BuildWidgetItem("Navigation",							"Navigation",						BasemapTypes.Basemap, "https://www.arcgis.com/home/item.html?id=78c096abedb9498380f5db1922f96aa0"));
			BasemapWidgetItems.Add(BuildWidgetItem("OpenStreetMap (Streets with Relief)",	"OpenStreetMapStreetsWithRelief",	BasemapTypes.Basemap, "https://www.arcgis.com/home/item.html?id=c6ec0420be5a4e36b57d1ef0f243b175"));
			BasemapWidgetItems.Add(BuildWidgetItem("OpenStreetMap (Streets)",				"OpenStreetMapStreets",				BasemapTypes.Basemap, "https://www.arcgis.com/home/item.html?id=bcefe41ce33943ac81d2fd801edd452c"));
			BasemapWidgetItems.Add(BuildWidgetItem("Topographic",							"Topographic",						BasemapTypes.Basemap, "https://www.arcgis.com/home/item.html?id=dd247558455c4ffab54566901a14f42c"));
			BasemapWidgetItems.Add(BuildWidgetItem("Mid-Century Map",						"MidCenturyMap",					BasemapTypes.Basemap, "https://www.arcgis.com/home/item.html?id=52d6a28f09704f04b33761ba7c4bf93f"));
			BasemapWidgetItems.Add(BuildWidgetItem("Colored Pencil Map",					"ColoredPencilMap",					BasemapTypes.Basemap, "https://www.arcgis.com/home/item.html?id=c0ddf27614bc407f92c35020a9b48afa"));
			BasemapWidgetItems.Add(BuildWidgetItem("Imagery",								"Imagery",							BasemapTypes.Basemap, "https://www.arcgis.com/home/item.html?id=c7d2b5c334364e8fb5b73b0f4d6a779b"));
			BasemapWidgetItems.Add(BuildWidgetItem("Community Map",							"CommunityMap",						BasemapTypes.Basemap, "https://www.arcgis.com/home/item.html?id=184f5b81589844699ca1e132d007920e"));
			BasemapWidgetItems.Add(BuildWidgetItem("Streets",								"Streets",							BasemapTypes.Basemap, "https://www.arcgis.com/home/item.html?id=e3e6df1d2f6a485d8a70f28fdd3ce19e"));
			BasemapWidgetItems.Add(BuildWidgetItem("Charted Territory Map",					"ChartedTerritoryMap",				BasemapTypes.Basemap, "https://www.arcgis.com/home/item.html?id=303e75b27af043fd835c981ab9accf84"));
			BasemapWidgetItems.Add(BuildWidgetItem("Modern Antique Map",					"ModernAntiqueMap",					BasemapTypes.Basemap, "https://www.arcgis.com/home/item.html?id=b69f2876ce4c4e9db185cdc857fcebc7"));
			BasemapWidgetItems.Add(BuildWidgetItem("OpenStreetMap",							"OpenStreetMap",					BasemapTypes.Basemap, "https://www.arcgis.com/home/item.html?id=695aef1564e84c06833542eb4d8c02d3"));
			BasemapWidgetItems.Add(BuildWidgetItem("Newspaper Map",							"NewspaperMap",						BasemapTypes.Basemap, "https://www.arcgis.com/home/item.html?id=e3c062eedf8b487b8bb5b9b08db1b7a9"));
			BasemapWidgetItems.Add(BuildWidgetItem("Nova Map",								"NovaMap",							BasemapTypes.Basemap, "https://www.arcgis.com/home/item.html?id=90f86b329f37499096d3715ac6e5ed1f"));
			#pragma warning restore format

			NoBasemapItem = new ArcGISMapCreatorBasemapWidget.Item
			{
				CanBeRemoved = false,
				ColorImage = MapCreatorUtilities.Assets.LoadImage($"MapCreator/BasemapTool/NoBasemap.png"),
				GrayscaleImage = MapCreatorUtilities.Assets.LoadImage($"MapCreator/BasemapTool/NoBasemap.png"),
				Name = "No Basemap",
				RequiresAPIKey = false,
				UserData = new BasemapItem
				{
					Name = "",
					Type = BasemapTypes.Basemap,
					Source = ""
				}
			};

			BasemapWidgetItems.Add(NoBasemapItem);

			Content = new VisualElement();

			Content.style.flexGrow = 1;

			Content.styleSheets.Add(MapCreatorUtilities.Assets.LoadStyleSheet("MapCreator/BasemapToolStyle.uss"));

			var basemapToolTemplate = MapCreatorUtilities.Assets.LoadVisualTreeAsset("MapCreator/BasemapToolTemplate.uxml");

			basemapToolTemplate.CloneTree(Content);

			BasemapWidget = Content.Q<ArcGISMapCreatorBasemapWidget>();

			BasemapWidget.BasemapItems = BasemapWidgetItems;
			BasemapWidget.OnSelectionChanged += BasemapWidget_OnSelectionChanged;

			var addNewLabel = Content.Q<VisualElement>(className: "add-new");

			addNewLabel.RegisterCallback<MouseUpEvent>(evnt =>
			{
				var window = EditorWindow.GetWindow<AddBasemapWindow>();

				window.OnBasemapItemAdded += (basemapItem) =>
				{
					var basemapWidgetItem = new ArcGISMapCreatorBasemapWidget.Item
					{
						CanBeRemoved = true,
						ColorImage = MapCreatorUtilities.Assets.LoadImage("MapCreator/BasemapTool/Custom.png"),
						Name = basemapItem.Name,
						RequiresAPIKey = false,
						UserData = basemapItem
					};

					basemapWidgetItem.GrayscaleImage = basemapWidgetItem.ColorImage;

					BasemapWidgetItems.Add(basemapWidgetItem);

					BasemapWidget.Rebuild();
				};

				window.ShowModal();
			});
		}

		public override void OnSelected()
		{
			LoadBasemapSettings();

			BasemapWidget.UpdateImages();
		}

		private void BasemapWidget_OnSelectionChanged(ArcGISMapCreatorBasemapWidget.Item item)
		{
			SelectedItem = item;

			var mapComponent = MapCreatorUtilities.MapComponent;

			if (mapComponent == null)
			{
				return;
			}

			if (SelectedItem != null)
			{
				var basemapItem = SelectedItem.UserData as BasemapItem;

				mapComponent.SetBasemapSourceAndType(basemapItem.Source, basemapItem.Type);
				mapComponent.BasemapAuthentication = basemapItem.Authentication;
			}
			else
			{
				mapComponent.SetBasemapSourceAndType(default(string), default(BasemapTypes));
				mapComponent.BasemapAuthentication = default(OAuthAuthenticationConfigurationMapping);
			}

			MapCreatorUtilities.MarkDirty();
		}

		private void LoadBasemapSettings()
		{
			var mapComponent = MapCreatorUtilities.MapComponent;

			if (mapComponent == null)
			{
				return;
			}

			var basemapAuthentication = mapComponent.BasemapAuthentication;
			var basemapType = mapComponent.BasemapType;
			var basemapUrl = mapComponent.Basemap;

			var basemapWidgetItem = GetBasemapWidgetItemFromUrl(basemapUrl);

			if (basemapWidgetItem == null)
			{
				basemapWidgetItem = new ArcGISMapCreatorBasemapWidget.Item
				{
					CanBeRemoved = true,
					ColorImage = MapCreatorUtilities.Assets.LoadImage("MapCreator/BasemapTool/Custom.png"),
					Name = "User Basemap",
					RequiresAPIKey = false,
					UserData = new BasemapItem
					{
						Authentication = basemapAuthentication,
						Name = "User Basemap",
						Source = basemapUrl,
						Type = basemapType
					}
				};

				basemapWidgetItem.GrayscaleImage = basemapWidgetItem.ColorImage;

				BasemapWidgetItems.Add(basemapWidgetItem);

				BasemapWidget.Rebuild();
			}

			BasemapWidget.SetSelectedItem(basemapWidgetItem);
		}

		ArcGISMapCreatorBasemapWidget.Item GetBasemapWidgetItemFromUrl(string url)
		{
			if (url == "")
			{
				return NoBasemapItem;
			}

			foreach (var basemapWidgetItem in BasemapWidgetItems)
			{
				var basemapItem = basemapWidgetItem.UserData as BasemapItem;

				if (basemapItem.Source == url)
				{
					return basemapWidgetItem;
				}
			}

			return null;
		}
	}
}

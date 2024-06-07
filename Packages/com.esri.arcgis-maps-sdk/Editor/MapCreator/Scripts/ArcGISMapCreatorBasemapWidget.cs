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
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace Esri.ArcGISMapsSDK.Editor.UI
{
	internal class GridView : VisualElement
	{
		public new class UxmlFactory : UxmlFactory<GridView> { }

		private ScrollView _scrollView;
		private VisualElement _contentContainer;

		private Dictionary<VisualElement, int> indexByVisualElement = new Dictionary<VisualElement, int>();

		private IList _itemsSource;

		public IList itemsSource
		{
			get => _itemsSource;
			set
			{
				if (_itemsSource != value)
				{
					_itemsSource = value;

					Rebuild();
				}
			}
		}

		Func<int, VisualElement> m_BuildItem;

		public Func<int, VisualElement> buildItem
		{
			get => m_BuildItem;
			set
			{
				m_BuildItem = value;

				Rebuild();
			}
		}

		public event Action onSelectionChange;

		private int _selectedIndex;
		private object _selectedItem;

		public int selectedIndex
		{
			get => _selectedIndex;
			set => _selectedIndex = value;
		}

		public GridView()
		{
			_contentContainer = new VisualElement();

			_contentContainer.AddToClassList("content-wrapper");

			_contentContainer.style.alignSelf = Align.FlexStart;
			_contentContainer.style.flexDirection = FlexDirection.Row;
			_contentContainer.style.flexWrap = Wrap.Wrap;

			_scrollView = new ScrollView();

			_scrollView.style.flexGrow = 1;

			_scrollView.Add(_contentContainer);
			hierarchy.Add(_scrollView);
		}

		public void ClearSelection()
		{
			_selectedIndex = -1;
			_selectedItem = null;

			foreach (var visualElement in indexByVisualElement.Keys)
			{
				visualElement.RemoveFromClassList(BaseVerticalCollectionView.itemSelectedVariantUssClassName);
			}
		}

		public void Rebuild()
		{
			_contentContainer.Clear();
			indexByVisualElement.Clear();

			var previousSelectedItem = _selectedItem;

			ClearSelection();

			if (_itemsSource != null)
			{
				for (var i = 0; i < _itemsSource.Count; ++i)
				{
					var item = m_BuildItem.Invoke(i);

					indexByVisualElement.Add(item, i);

					item.AddToClassList(BaseVerticalCollectionView.itemUssClassName);

					item.RegisterCallback<ClickEvent>(OnClickOnItem);

					_contentContainer.Add(item);

					if (previousSelectedItem != null && _itemsSource[i] == previousSelectedItem)
					{
						SetSelectionInternal(i, false);
					}
				}
			}

			if (previousSelectedItem != _selectedItem)
			{
				onSelectionChange.Invoke();
			}
		}

		void OnClickOnItem(ClickEvent evt)
		{
			var visualElement = evt.currentTarget as VisualElement;

			if (indexByVisualElement.ContainsKey(visualElement))
			{
				SetSelectionInternal(indexByVisualElement[visualElement], true);
			}
		}

		public void SetSelection(int index)
		{
			SetSelectionInternal(index, false);
		}

		internal void SetSelectionInternal(int index, bool notify)
		{
			if (_selectedIndex == index)
			{
				return;
			}

			ClearSelection();

			if (index < 0)
			{
				return;
			}
			else
			{
				_selectedIndex = index;
				_selectedItem = _itemsSource[index];

				foreach (var pair in indexByVisualElement)
				{
					if (pair.Value == _selectedIndex)
					{
						pair.Key.AddToClassList(BaseVerticalCollectionView.itemSelectedVariantUssClassName);
					}
				}
			}

			if (notify)
			{
				onSelectionChange.Invoke();
			}
		}
	}

	public class ArcGISMapCreatorBasemapWidget : VisualElement
	{
		public new class UxmlFactory : UxmlFactory<ArcGISMapCreatorBasemapWidget> { }

		public class Item
		{
			public bool CanBeRemoved;
			public Texture2D ColorImage;
			public Texture2D GrayscaleImage;
			public string Name;
			public bool RequiresAPIKey;
			public object UserData;
		}

		public event Action<Item> OnSelectionChanged;

		private VisualTreeAsset BasemapItemTemplate;
		private GridView GridView;

		private List<Item> basemapItems;
		public List<Item> BasemapItems
		{
			get
			{
				return basemapItems;
			}
			set
			{
				if (basemapItems != value)
				{
					basemapItems = value;

					GridView.itemsSource = basemapItems;
				}
			}
		}

		public ArcGISMapCreatorBasemapWidget()
		{
			styleSheets.Add(MapCreatorUtilities.Assets.LoadStyleSheet("MapCreator/BasemapCardStyle.uss"));

			BasemapItemTemplate = MapCreatorUtilities.Assets.LoadVisualTreeAsset("MapCreator/BasemapCardTemplate.uxml");

			SetupGridView();
		}

		private void GridView_onSelectionChange()
		{
			var basemapItem = GridView.selectedIndex > -1 ? BasemapItems[GridView.selectedIndex] : null;

			OnSelectionChanged?.Invoke(basemapItem);
		}

		public void Rebuild()
		{
			GridView.Rebuild();
		}

		public void SetSelectedItem(Item selectedItem)
		{
			GridView.SetSelection(basemapItems.IndexOf(selectedItem));
		}

		private void SetupGridView()
		{
			GridView = new GridView();

			GridView.style.flexGrow = 1;

			GridView.buildItem = (index) =>
			{
				var basemapItem = basemapItems[index];

				var element = new VisualElement();

				element.AddToClassList("card");

				if (basemapItem.CanBeRemoved)
				{
					element.AddToClassList("removable");
				}

				BasemapItemTemplate.CloneTree(element);

				var image = element.Q<Image>(className: "card-image");
				var label = element.Q<Label>(className: "card-label");

				image.scaleMode = ScaleMode.ScaleAndCrop;

				label.text = basemapItem.Name;

				element.userData = basemapItem;

				UpdateImage(element);

				var removeButton = element.Q<VisualElement>(className: "remove-button");

				removeButton.RegisterCallback<MouseUpEvent>(evnt =>
				{
					if (basemapItem.CanBeRemoved)
					{
						basemapItems.Remove(basemapItem);

						GridView.Rebuild();
					}
				});

				return element;
			};

			GridView.onSelectionChange += GridView_onSelectionChange;

			Add(GridView);
		}

		private void UpdateImage(VisualElement cardElement)
		{
			var basemapItem = cardElement?.userData as Item;

			if (basemapItem == null)
			{
				return;
			}

			var image = cardElement?.Q<Image>();

			if (image == null)
			{
				return;
			}

			var mapComponent = (ArcGISMapComponent)MapCreatorUtilities.MapComponent;

			if (mapComponent != null && basemapItem.RequiresAPIKey && mapComponent.GetEffectiveAPIKey() == "")
			{
				image.image = basemapItem.GrayscaleImage;
			}
			else
			{
				image.image = basemapItem.ColorImage;
			}
		}

		public void UpdateImages()
		{
			var cards = GridView.Query<VisualElement>(className: "card").ToList();

			cards.ForEach(card =>
			{
				UpdateImage(card);
			});
		}
	}
}

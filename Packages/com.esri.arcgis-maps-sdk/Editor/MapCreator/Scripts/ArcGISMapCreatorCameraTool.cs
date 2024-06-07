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
using Esri.ArcGISMapsSDK.Utils.GeoCoord;
using Esri.ArcGISMapsSDK.Utils.Math;
using Esri.HPFramework;
using Esri.GameEngine.Geometry;
using System;
using Unity.Mathematics;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Esri.ArcGISMapsSDK.Editor.UI
{
	public class ArcGISMapCreatorCameraTool : ArcGISMapCreatorTool
	{
		private static ArcGISCameraComponent selectedCamera;
		private static ArcGISLocationComponent selectedLocationComponent;

		private static VisualElement rootElement;

		private static DoubleField LocationFieldX;
		private static DoubleField LocationFieldY;
		private static DoubleField LocationFieldZ;
		private static IntegerField LocationFieldSR;

		private static DoubleField RotationFieldHeading;
		private static DoubleField RotationFieldPitch;
		private static DoubleField RotationFieldRoll;

		private static SceneView lastSceneView;

		public override VisualElement GetContent()
		{
			rootElement = new VisualElement();
			rootElement.name = "ArcGISMapCreatorCameraTool";

			var template = MapCreatorUtilities.Assets.LoadVisualTreeAsset("MapCreator/CameraToolTemplate.uxml");
			template.CloneTree(rootElement);

			FindCamera();

			InitializeCreateCameraButton();

			PopulateCameraFields();

			InitAlignCameraToViewButton();

			return rootElement;
		}

		public override Texture2D GetImage()
		{
			return MapCreatorUtilities.Assets.LoadImage("MapCreator/Toolbar/CameraToolIcon.png");
		}

		public override string GetLabel()
		{
			return "Camera";
		}

		private static void FindCamera()
		{
			selectedCamera = GameObject.FindObjectOfType<ArcGISCameraComponent>();

			if (selectedCamera != null)
			{
				if (selectedCamera.transform.parent == null || selectedCamera.transform.parent.GetComponent<ArcGISMapComponent>() == null)
				{
					Debug.LogWarning("Parent the ArcGIS Camera game object to a game object with an ArcGIS Map component to use the Camera UI tool");
				}

				selectedLocationComponent = selectedCamera.GetComponent<ArcGISLocationComponent>();

				if (selectedLocationComponent == null)
				{
					Debug.LogWarning("Attach an ArcGIS Location component to the ArcGIS Camera game object to use the full capability of the Camera UI tool");
				}
			}
		}

		private static void InitializeCreateCameraButton()
		{
			Button createCamButton = rootElement.Query<Button>(name: "button-create-camera");

			createCamButton.clickable.activators.Clear();
			createCamButton.RegisterCallback<MouseDownEvent>(evnt => CreateCamera(evnt));

			if (selectedCamera != null && selectedLocationComponent != null)
			{
				createCamButton.SetEnabled(false);
			}
		}

		private static void CreateCamera(MouseDownEvent evnt)
		{
			GameObject cameraComponentGameObject;

			if (Camera.main != null)
			{
				cameraComponentGameObject = Camera.main.gameObject;

				cameraComponentGameObject.name = "ArcGISCamera";
			}
			else
			{
				cameraComponentGameObject = new GameObject("ArcGISCamera");
				cameraComponentGameObject.AddComponent<Camera>();
				cameraComponentGameObject.tag = "MainCamera";
			}

			if (SceneView.lastActiveSceneView != null)
			{
				cameraComponentGameObject.transform.position = SceneView.lastActiveSceneView.camera.transform.position;
				cameraComponentGameObject.transform.rotation = SceneView.lastActiveSceneView.camera.transform.rotation;
			}

			var mapComponent = (ArcGISMapComponent)MapCreatorUtilities.MapComponent;

			if (mapComponent != null)
			{
				cameraComponentGameObject.transform.parent = mapComponent.transform;
			}

			selectedCamera = cameraComponentGameObject.AddComponent<ArcGISCameraComponent>();

			selectedLocationComponent = cameraComponentGameObject.AddComponent<ArcGISLocationComponent>();
			selectedLocationComponent.Position = new ArcGISPoint(LocationFieldX.value, LocationFieldY.value, LocationFieldZ.value, new ArcGISSpatialReference(LocationFieldSR.value));
			selectedLocationComponent.Rotation = new ArcGISRotation(RotationFieldHeading.value, RotationFieldPitch.value, RotationFieldRoll.value);

			PopulateCameraFields();

			var button = (Button)evnt.currentTarget;
			button.SetEnabled(false);

			Undo.RegisterCreatedObjectUndo(cameraComponentGameObject, "Create " + cameraComponentGameObject.name);

			Selection.activeGameObject = cameraComponentGameObject;
		}

		private static void PopulateCameraFields()
		{
			Action<int> intFieldValueChangedCallback = (int value) =>
			{
				try
				{

					var spatialRef = new ArcGISSpatialReference(value);

					if (selectedLocationComponent != null)
					{
						selectedLocationComponent.Position = new ArcGISPoint(selectedLocationComponent.Position.X, selectedLocationComponent.Position.Y, selectedLocationComponent.Position.Z, spatialRef);
						MapCreatorUtilities.MarkDirty();
					}

					MapCreatorUtilities.UpdateArcGISPointLabels(LocationFieldX, LocationFieldY, LocationFieldZ, LocationFieldSR.value);
				}
				catch
				{
				}
			};

			Action<double> XOriginChanged = (double value) =>
			{
				if (selectedLocationComponent != null)
				{
					selectedLocationComponent.Position = new ArcGISPoint(value, selectedLocationComponent.Position.Y, selectedLocationComponent.Position.Z, selectedLocationComponent.Position.SpatialReference);
					MapCreatorUtilities.MarkDirty();
				}
			};

			Action<double> YOriginChanged = (double value) =>
			{
				if (selectedLocationComponent != null)
				{
					selectedLocationComponent.Position = new ArcGISPoint(selectedLocationComponent.Position.X, value, selectedLocationComponent.Position.Z, selectedLocationComponent.Position.SpatialReference);
					MapCreatorUtilities.MarkDirty();
				}
			};

			Action<double> ZOriginChanged = (double value) =>
			{
				if (selectedLocationComponent != null)
				{
					selectedLocationComponent.Position = new ArcGISPoint(selectedLocationComponent.Position.X, selectedLocationComponent.Position.Y, value, selectedLocationComponent.Position.SpatialReference);
					MapCreatorUtilities.MarkDirty();
				}
			};

			Action<double> HeadingChanged = (double value) =>
			{
				if (selectedLocationComponent != null)
				{
					selectedLocationComponent.Rotation = new ArcGISRotation(value, selectedLocationComponent.Rotation.Pitch, selectedLocationComponent.Rotation.Roll);
					MapCreatorUtilities.MarkDirty();
				}
			};

			Action<double> PitchChanged = (double value) =>
			{
				if (selectedLocationComponent != null)
				{
					selectedLocationComponent.Rotation = new ArcGISRotation(selectedLocationComponent.Rotation.Heading, value, selectedLocationComponent.Rotation.Roll);
					MapCreatorUtilities.MarkDirty();
				}
			};

			Action<double> RollChanged = (double value) =>
			{
				if (selectedLocationComponent != null)
				{
					selectedLocationComponent.Rotation = new ArcGISRotation(selectedLocationComponent.Rotation.Heading, selectedLocationComponent.Rotation.Pitch, value);
					MapCreatorUtilities.MarkDirty();
				}
			};

			LocationFieldX = MapCreatorUtilities.InitializeDoubleField(rootElement, "cam-position-x", null, XOriginChanged);
			LocationFieldY = MapCreatorUtilities.InitializeDoubleField(rootElement, "cam-position-y", null, YOriginChanged);
			LocationFieldZ = MapCreatorUtilities.InitializeDoubleField(rootElement, "cam-position-z", null, ZOriginChanged);
			LocationFieldSR = MapCreatorUtilities.InitializeIntegerField(rootElement, "cam-position-wkid", null, intFieldValueChangedCallback);

			RotationFieldHeading = MapCreatorUtilities.InitializeDoubleField(rootElement, "cam-rotation-heading", null, HeadingChanged);
			RotationFieldPitch = MapCreatorUtilities.InitializeDoubleField(rootElement, "cam-rotation-pitch", null, PitchChanged);
			RotationFieldRoll = MapCreatorUtilities.InitializeDoubleField(rootElement, "cam-rotation-roll", null, RollChanged);

			if (selectedLocationComponent == null)
			{
				LocationFieldSR.SetValueWithoutNotify(SpatialReferenceWkid.WGS84);
			}
			else
			{
				LocationFieldX.SetValueWithoutNotify(MapCreatorUtilities.TruncateDoubleForUI(selectedLocationComponent.Position.X));
				LocationFieldY.SetValueWithoutNotify(MapCreatorUtilities.TruncateDoubleForUI(selectedLocationComponent.Position.Y));
				LocationFieldZ.SetValueWithoutNotify(MapCreatorUtilities.TruncateDoubleForUI(selectedLocationComponent.Position.Z));
				LocationFieldSR.SetValueWithoutNotify(selectedLocationComponent.Position.SpatialReference.WKID);

				RotationFieldHeading.SetValueWithoutNotify(MapCreatorUtilities.TruncateDoubleForUI(selectedLocationComponent.Rotation.Heading));
				RotationFieldPitch.SetValueWithoutNotify(MapCreatorUtilities.TruncateDoubleForUI(selectedLocationComponent.Rotation.Pitch));
				RotationFieldRoll.SetValueWithoutNotify(MapCreatorUtilities.TruncateDoubleForUI(selectedLocationComponent.Rotation.Roll));
			}

			MapCreatorUtilities.UpdateArcGISPointLabels(LocationFieldX, LocationFieldY, LocationFieldZ, LocationFieldSR.value);
		}

		private static void InitAlignCameraToViewButton()
		{
			Button AlignCameraToViewButton = rootElement.Query<Button>(name: "button-transfer-to-camera");
			AlignCameraToViewButton.clickable.activators.Clear();
			AlignCameraToViewButton.RegisterCallback<MouseDownEvent>(evnt =>
			{
				if (Application.isPlaying)
				{
					return;
				}

				if (lastSceneView == null || lastSceneView != SceneView.lastActiveSceneView)
				{
					lastSceneView = SceneView.lastActiveSceneView;
				}

				if (selectedCamera != null)
				{
					var cameraTransform = selectedCamera.GetComponent<HPTransform>();
					var mapComponent = cameraTransform.GetComponentInParent<ArcGISMapComponent>();

					Selection.activeGameObject = cameraTransform.gameObject;
					lastSceneView.AlignWithView();

					var worldPosition = math.inverse(mapComponent.WorldMatrix).HomogeneousTransformPoint(SceneView.lastActiveSceneView.camera.transform.position.ToDouble3());
					var geoPosition = mapComponent.View.WorldToGeographic(worldPosition);

					geoPosition = GeoUtils.ProjectToSpatialReference(geoPosition, new ArcGISSpatialReference(LocationFieldSR.value));

					if (!Double.IsNaN(geoPosition.X) && !Double.IsNaN(geoPosition.Y) && !Double.IsNaN(geoPosition.Z))
					{
						LocationFieldX.SetValueWithoutNotify(MapCreatorUtilities.TruncateDoubleForUI(geoPosition.X));
						LocationFieldY.SetValueWithoutNotify(MapCreatorUtilities.TruncateDoubleForUI(geoPosition.Y));
						LocationFieldZ.SetValueWithoutNotify(MapCreatorUtilities.TruncateDoubleForUI(geoPosition.Z));

						var worldRotation = mapComponent.UniverseRotation * SceneView.lastActiveSceneView.camera.transform.rotation;
						var geoRotation = GeoUtils.FromCartesianRotation(worldPosition, quaternionExtensions.ToQuaterniond(worldRotation), new ArcGISSpatialReference(LocationFieldSR.value), mapComponent.MapType);

						RotationFieldHeading.SetValueWithoutNotify(MapCreatorUtilities.TruncateDoubleForUI(geoRotation.Heading));
						RotationFieldPitch.SetValueWithoutNotify(MapCreatorUtilities.TruncateDoubleForUI(geoRotation.Pitch));
						RotationFieldRoll.SetValueWithoutNotify(MapCreatorUtilities.TruncateDoubleForUI(geoRotation.Roll));

						if (selectedLocationComponent != null)
						{
							selectedLocationComponent.SyncPositionWithHPTransform();
							selectedLocationComponent.Position = geoPosition;
							selectedLocationComponent.Rotation = geoRotation;
						}
					}
				}
			});
		}
	}
}

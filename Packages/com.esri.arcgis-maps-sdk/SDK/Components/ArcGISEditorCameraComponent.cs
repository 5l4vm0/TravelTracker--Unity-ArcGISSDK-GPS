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
using Esri.ArcGISMapsSDK.Utils.Math;
using Esri.HPFramework;
using Unity.Mathematics;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using System;
using Esri.GameEngine.View;

namespace Esri.ArcGISMapsSDK.Components
{
	[ExecuteInEditMode]
	[RequireComponent(typeof(Camera))]
	[RequireComponent(typeof(HPTransform))]
	[RequireComponent(typeof(ArcGISCameraComponent))]
	[AddComponentMenu("")]
	public class ArcGISEditorCameraComponent : MonoBehaviour
	{
		private ArcGISMapComponent mapComponent;
		private bool initialized = false;

		private double3 lastEditorCameraPosition;
		private double3 lastRootPosition;
		private HPTransform hpTransform;

		bool worldRepositionEnabled = false;

		// These values are used to adjust the clipping threshold.
		private const float AltitudeThreshold = 20000.0f;
		private const float FarClipDistance = 10000f;
		private const float NearClipDistance = 0.5f;

		// These values are used to adjust camera speed relative to altitude.
		private const float DefaultMaxSpeed = 10000f;
		private const float MaxSpeed = 600000.0f;
		private const float MinSpeed = 300.0f;
		private const float AltitudeScalar = 0.00001f;
		private const float AltitudePowerValue = 1.2f;
		private const float SpeedScalar = 0.02f;

		public bool WorldRepositionEnabled
		{
			get => worldRepositionEnabled;

			set
			{
				worldRepositionEnabled = value;
			}
		}

		public bool EditorViewEnabled
		{
			get => GetComponent<ArcGISCameraComponent>().enabled;

			set
			{
				GetComponent<ArcGISCameraComponent>().enabled = value;
			}
		}

		private void Awake()
		{
			GetComponent<Camera>().enabled = false;
			GetComponent<ArcGISCameraComponent>().enabled = false;
			hpTransform = GetComponent<HPTransform>();
			hpTransform.enabled = false;

#if UNITY_EDITOR
			if (SceneView.lastActiveSceneView?.cameraSettings != null)
			{
				SceneView.lastActiveSceneView.cameraSettings.speedMax = DefaultMaxSpeed;
				SceneView.lastActiveSceneView.cameraSettings.accelerationEnabled = false;
			}
#endif
		}

		private void OnEnable()
		{
#if UNITY_EDITOR
			if (!Application.isPlaying)
			{
				mapComponent = GetComponentInParent<ArcGISMapComponent>();

				GetComponent<ArcGISCameraComponent>().enabled = mapComponent.DataFetchWithSceneView;
				worldRepositionEnabled = mapComponent.RebaseWithSceneView;
				hpTransform.enabled = true;
				initialized = false;

				mapComponent.MapTypeChanged += new ArcGISMapComponent.MapTypeChangedEventHandler(() => { initialized = false; });
			}
#endif
		}

		private void OnDisable()
		{
#if UNITY_EDITOR
			if (!Application.isPlaying)
			{
				GetComponent<ArcGISCameraComponent>().enabled = false;
				hpTransform.enabled = false;

				mapComponent.MapTypeChanged -= new ArcGISMapComponent.MapTypeChangedEventHandler(() => { initialized = false; });
			}

			initialized = false;
#endif
		}

		private void OnTransformParentChanged()
		{
			OnEnable();
		}

		private void Update()
		{
#if UNITY_EDITOR
			if (!Application.isPlaying)
			{
				if (SceneView.lastActiveSceneView)
				{
					var rootDeltaPosition = initialized ? mapComponent.UniversePosition - lastRootPosition : double3.zero;
					hpTransform.UniversePosition = math.inverse(mapComponent.WorldMatrix).HomogeneousTransformPoint(SceneView.lastActiveSceneView.camera.transform.position.ToDouble3()) - rootDeltaPosition;
					hpTransform.UniverseRotation = mapComponent.UniverseRotation * SceneView.lastActiveSceneView.camera.transform.rotation;

					var camera = GetComponent<Camera>();
					camera.fieldOfView = SceneView.lastActiveSceneView.cameraSettings.fieldOfView;
					camera.aspect = SceneView.lastActiveSceneView.camera.aspect;

					if (mapComponent.View.SpatialReference != null)
					{
						var altitude = Math.Abs((float)mapComponent.View.AltitudeAtCartesianPosition(hpTransform.UniversePosition));

						// SceneView Camera NearPlane is updated a frame after update cameraSettings, because of that our GetCameraNearPlane doesn't work.
						// A very simple solution is implemented to resolve near-far distance problem
						float near = altitude > AltitudeThreshold ? FarClipDistance : NearClipDistance;
						var mapType = mapComponent.Map?.MapType ?? GameEngine.Map.ArcGISMapType.Global;
						var sr = mapComponent.View.SpatialReference;

						SceneView.lastActiveSceneView.cameraSettings.nearClip = near;
						SceneView.lastActiveSceneView.cameraSettings.farClip = (float)Math.Max(near, Utils.FrustumHelpers.CalculateFarPlaneDistance(altitude, mapType, sr));

						SceneView.lastActiveSceneView.cameraSettings.speed = (float)(Math.Pow(Math.Min(altitude * AltitudeScalar, 1), AltitudePowerValue) * MaxSpeed + MinSpeed) * SpeedScalar;
					}

					if (!initialized)
					{
						lastEditorCameraPosition = hpTransform.UniversePosition;
						SceneView.lastActiveSceneView.cameraSettings.dynamicClip = false;

						initialized = true;
					}
					else
					{
						var delta = lastEditorCameraPosition - hpTransform.UniversePosition;
						lastEditorCameraPosition = hpTransform.UniversePosition;

						if (worldRepositionEnabled && delta.Equals(double3.zero) && !mapComponent.UniversePosition.Equals(hpTransform.UniversePosition))
						{
							mapComponent.UniversePosition = hpTransform.UniversePosition;
							SceneView.lastActiveSceneView.pivot -= SceneView.lastActiveSceneView.camera.transform.position;
						}
					}

					lastRootPosition = mapComponent.UniversePosition;
				}
			}
#endif
		}
	}
}

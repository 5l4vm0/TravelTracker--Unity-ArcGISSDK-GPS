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
#if USE_HDRP_PACKAGE
using Esri.ArcGISMapsSDK.Utils;
using Esri.GameEngine.View;
using Esri.HPFramework;
using Unity.Mathematics;
#endif
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
#if USE_HDRP_PACKAGE
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;
#endif

namespace Esri.ArcGISMapsSDK.Components
{
	[DisallowMultipleComponent]
	[ExecuteAlways]
	[AddComponentMenu("ArcGIS Maps SDK/ArcGIS Sky Reposition")]
	public class ArcGISSkyRepositionComponent : MonoBehaviour
	{
#if USE_HDRP_PACKAGE
		private double3 localOrigin = double3.zero;
		private double3 cameraPosition = double3.zero;

		private PhysicallyBasedSky sky = null;
		private Fog fog = null;

		public ArcGISCameraComponent CameraComponent = null;
		public ArcGISMapComponent arcGISMapComponent = null;

		private void OnEnable()
		{
			Init();
		}

		private void Start()
		{
			if (CameraComponent == null)
			{
				Debug.LogError("CameraComponent cannot be null");
			}
			else if (arcGISMapComponent == null)
			{
				Debug.LogError("arcGISMapComponent cannot be null");
			}

			Init();
		}

		private void Update()
		{
			if (arcGISMapComponent != null)
			{
				UpdateSkyAndFog();
			}
		}

		private void Init()
		{
			//Disable this component if we are not using HDRP
			if (GraphicsSettings.renderPipelineAsset.GetType() != typeof(HDRenderPipelineAsset))
			{
				Debug.Log("ArcGISSkyRepositionComponent is only configured to work with the HDRP");
				this.enabled = false;
				return;
			}

			if (arcGISMapComponent != null)
			{
				if (GameObject.FindObjectOfType<Volume>())
				{
					Volume volume = GameObject.FindObjectOfType<Volume>();

					if (volume.profile.TryGet(out PhysicallyBasedSky tmpSky))
					{
						sky = tmpSky;
					}

					if (volume.profile.TryGet(out Fog tmpFog))
					{
						fog = tmpFog;
					}
				}
			}
		}

		private void UpdateSkyAndFog()
		{
			if (arcGISMapComponent.View.SpatialReference != null)
			{
				var currentLocalOrigin = arcGISMapComponent.UniversePosition;
				var currentCameraPosition = GetCurrentWorldCameraPosition();

				if (!localOrigin.Equals(currentLocalOrigin) || !cameraPosition.Equals(currentCameraPosition))
				{
					localOrigin = currentLocalOrigin;
					cameraPosition = currentCameraPosition;

					UpdateSkyParameters();
					UpdateFogParameters();
				}
			}
		}

		private void UpdateSkyParameters()
		{
			if (sky != null)
			{
				var altitude = arcGISMapComponent.View.AltitudeAtCartesianPosition(localOrigin);

				sky.sphericalMode.overrideState = true;

				if (arcGISMapComponent.MapType == Esri.GameEngine.Map.ArcGISMapType.Local)
				{
					sky.seaLevel.overrideState = true;
					sky.seaLevel.value = (float)-altitude;

					sky.sphericalMode.value = false;
				}
				else
				{
					sky.planetaryRadius.overrideState = true;
					sky.planetCenterPosition.overrideState = true;

					sky.sphericalMode.value = true;
					sky.planetaryRadius.value = (float)arcGISMapComponent.View.SpatialReference.SpheroidData.MajorSemiAxis;
					sky.planetCenterPosition.value = new Vector3(0, AtmosphereHelpers.CalculateGlobalViewSkyAtmosphereOffsetFrom(altitude, math.length(localOrigin), arcGISMapComponent.View.SpatialReference), 0);
				}
			}
		}

		private void UpdateFogParameters()
		{
			if (fog != null && fog.enabled.value)
			{
				var cameraAltitude = arcGISMapComponent.View.AltitudeAtCartesianPosition(cameraPosition);
				var baseAltitude = arcGISMapComponent.View.AltitudeAtCartesianPosition(localOrigin);

				fog.baseHeight.overrideState = true;
				fog.maximumHeight.overrideState = true;
				fog.meanFreePath.overrideState = true;

				fog.baseHeight.value = -(float)baseAltitude;
				fog.maximumHeight.value = (float)Utils.GeoCoord.GeoUtils.MaxTerrainAltitude;
				fog.meanFreePath.value = AtmosphereHelpers.CalculateFogMeanFreePathPropertyFrom(cameraAltitude, arcGISMapComponent.View.SpatialReference);
			}
		}

		private double3 GetCurrentWorldCameraPosition()
		{
			var cameraUniversePosition = new double3();

#if UNITY_EDITOR
			if (!Application.isPlaying)
			{
				if (SceneView.lastActiveSceneView)
				{
					cameraUniversePosition = math.inverse(arcGISMapComponent.WorldMatrix).HomogeneousTransformPoint(SceneView.lastActiveSceneView.camera.transform.position.ToDouble3());
				}
			}
			else
#endif
			{
				if (Camera.main)
				{
					cameraUniversePosition = math.inverse(arcGISMapComponent.WorldMatrix).HomogeneousTransformPoint(Camera.main.transform.position.ToDouble3());
				}
			}

			return cameraUniversePosition;
		}
#endif
	}
}

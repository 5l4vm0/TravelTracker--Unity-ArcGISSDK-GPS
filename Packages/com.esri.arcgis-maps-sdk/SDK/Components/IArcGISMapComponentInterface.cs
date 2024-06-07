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
using Esri.ArcGISMapsSDK.Security;
using Esri.GameEngine.Elevation.Base;
using Esri.GameEngine.Geometry;
using Esri.GameEngine.Map;
using Esri.GameEngine.View;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Mathematics;
using UnityEngine;

namespace Esri.ArcGISMapsSDK
{
	public interface IArcGISMapComponentInterface
	{
		// API Key Interface

		string APIKey { get; set; }

		// Basemap Interface

		string Basemap { get; set; }
		BasemapTypes BasemapType { get; set; }
		OAuthAuthenticationConfigurationMapping BasemapAuthentication { get; set; }
		void SetBasemapSourceAndType(string source, BasemapTypes type);

		// Elevation Interface

		[Obsolete("Elevation is deprecated, please use MapElevation instead.")]
		string Elevation { get; set; }
		[Obsolete("ElevationAuthentication is deprecated, please use MapElevation instead.")]
		OAuthAuthenticationConfigurationMapping ElevationAuthentication { get; }
		[Obsolete("ElevationSources is deprecated, please use MapElevation instead.")]
		List<ArcGISElevationSourceInstanceData> ElevationSources { get; set; }
		ArcGISMapElevationInstanceData MapElevation { get; set; }

		// Extent Interface

		bool EnableExtent { get; set; }
		ArcGISExtentInstanceData Extent { get; set; }

		// Layers Interface

		List<ArcGISLayerInstanceData> Layers { get; set; }

		// Map Interface

		ArcGISPoint OriginPosition { get; set; }
		ArcGISMapType MapType { get; set; }
		bool MeshCollidersEnabled { get; set; }
		ArcGISView View { get; }
		double3 UniversePosition { get; set; }
		Quaternion UniverseRotation { get; }
		double4x4 WorldMatrix { get; }
		Vector3 GeographicToEngine(ArcGISPoint position);
		ArcGISPoint EngineToGeographic(Vector3 position);

		// Authentication Interface

		List<ArcGISAuthenticationConfigurationInstanceData> Configurations { get; }

		// Editor Interface

#if UNITY_EDITOR

		bool EditorModeEnabled { get; set; }
		bool DataFetchWithSceneView { get; set; }
		bool RebaseWithSceneView { get; set; }

#endif

		// Zoom Interface

		Task<bool> ZoomToElevationSource(GameObject gameObject, ArcGISElevationSourceInstanceData elevationSourceInstanceData);
		Task<bool> ZoomToElevationSource(GameObject gameObject, ArcGISElevationSource elevationSource);
		bool ZoomToExtent(GameObject gameObject, Esri.GameEngine.Extent.ArcGISExtent extent);
		Task<bool> ZoomToLayer(GameObject gameObject, Esri.GameEngine.Layers.Base.ArcGISLayer layer);

		// Raycast Interface

		Physics.ArcGISRaycastHit GetArcGISRaycastHit(RaycastHit raycastHit);
	}
}

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
using Esri.ArcGISMapsSDK.SDK.Utils;
using Esri.ArcGISMapsSDK.Security;
using Esri.ArcGISMapsSDK.Utils;
using Esri.GameEngine.Geometry;
using Esri.GameEngine.Layers;
using Esri.GameEngine.Layers.Base;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Esri.ArcGISMapsSDK.Components
{
	[Serializable]
	public class ArcGISPolygonInstanceData : ICloneable
	{
		public List<ArcGISPoint> Points = new List<ArcGISPoint>();

		public object Clone()
		{
			var polygonInstanceData = new ArcGISPolygonInstanceData();

			polygonInstanceData.Points = Points.Clone();

			return polygonInstanceData;
		}

		public override bool Equals(object obj)
		{
			return obj is ArcGISPolygonInstanceData data &&
				   EqualityComparer<List<ArcGISPoint>>.Default.Equals(Points, data.Points);
		}

		public override int GetHashCode()
		{
			return HashCode.Combine(Points);
		}

		public static bool operator ==(ArcGISPolygonInstanceData left, ArcGISPolygonInstanceData right)
		{
			return EqualityComparer<ArcGISPolygonInstanceData>.Default.Equals(left, right);
		}

		public static bool operator !=(ArcGISPolygonInstanceData left, ArcGISPolygonInstanceData right)
		{
			return !(left == right);
		}
	}

	[Serializable]
	public class ArcGISMeshModificationInstanceData : ICloneable
	{
		public ArcGISPolygonInstanceData Polygon = new ArcGISPolygonInstanceData();
		public ArcGISMeshModificationType Type = ArcGISMeshModificationType.Clip;

		public object Clone()
		{
			var meshModificationInstanceData = new ArcGISMeshModificationInstanceData();

			meshModificationInstanceData.Polygon = (ArcGISPolygonInstanceData)Polygon.Clone();
			meshModificationInstanceData.Type = Type;

			return meshModificationInstanceData;
		}

		public override bool Equals(object obj)
		{
			return obj is ArcGISMeshModificationInstanceData data &&
				   EqualityComparer<ArcGISPolygonInstanceData>.Default.Equals(Polygon, data.Polygon) &&
				   Type == data.Type;
		}

		public override int GetHashCode()
		{
			return HashCode.Combine(Polygon, Type);
		}

		public static bool operator ==(ArcGISMeshModificationInstanceData left, ArcGISMeshModificationInstanceData right)
		{
			return EqualityComparer<ArcGISMeshModificationInstanceData>.Default.Equals(left, right);
		}

		public static bool operator !=(ArcGISMeshModificationInstanceData left, ArcGISMeshModificationInstanceData right)
		{
			return !(left == right);
		}
	}

	[Serializable]
	public class ArcGISSpatialFeatureFilterInstanceData
	{
		public bool IsEnabled = false;
		public List<ArcGISPolygonInstanceData> Polygons = new List<ArcGISPolygonInstanceData>();
		public ArcGISSpatialFeatureFilterSpatialRelationship Type = ArcGISSpatialFeatureFilterSpatialRelationship.Disjoint;

		public override bool Equals(object obj)
		{
			return obj is ArcGISSpatialFeatureFilterInstanceData data &&
				   IsEnabled == data.IsEnabled &&
				   EqualityComparer<List<ArcGISPolygonInstanceData>>.Default.Equals(Polygons, data.Polygons) &&
				   Type == data.Type;
		}

		public override int GetHashCode()
		{
			return HashCode.Combine(IsEnabled, Polygons, Type);
		}

		public static bool operator ==(ArcGISSpatialFeatureFilterInstanceData left, ArcGISSpatialFeatureFilterInstanceData right)
		{
			return EqualityComparer<ArcGISSpatialFeatureFilterInstanceData>.Default.Equals(left, right);
		}

		public static bool operator !=(ArcGISSpatialFeatureFilterInstanceData left, ArcGISSpatialFeatureFilterInstanceData right)
		{
			return !(left == right);
		}
	}

	[Serializable]
	public class ArcGISMeshModificationsInstanceData : ICloneable
	{
		public bool IsEnabled = true;
		public List<ArcGISMeshModificationInstanceData> MeshModifications = new List<ArcGISMeshModificationInstanceData>();

		public object Clone()
		{
			var meshModificationsInstanceData = new ArcGISMeshModificationsInstanceData();

			meshModificationsInstanceData.IsEnabled = IsEnabled;
			meshModificationsInstanceData.MeshModifications = MeshModifications.Clone();

			return meshModificationsInstanceData;
		}

		public override bool Equals(object obj)
		{
			return obj is ArcGISMeshModificationsInstanceData data &&
				   IsEnabled == data.IsEnabled &&
				   EqualityComparer<List<ArcGISMeshModificationInstanceData>>.Default.Equals(MeshModifications, data.MeshModifications);
		}

		public override int GetHashCode()
		{
			return HashCode.Combine(IsEnabled, MeshModifications);
		}

		public static bool operator ==(ArcGISMeshModificationsInstanceData left, ArcGISMeshModificationsInstanceData right)
		{
			return EqualityComparer<ArcGISMeshModificationsInstanceData>.Default.Equals(left, right);
		}

		public static bool operator !=(ArcGISMeshModificationsInstanceData left, ArcGISMeshModificationsInstanceData right)
		{
			return !(left == right);
		}
	}

	[Serializable]
	public class ArcGISLayerInstanceData
	{
		public string Name;
		[EnumFilter(typeof(LayerTypes), (int)LayerTypes.ArcGISUnsupportedLayer, (int)LayerTypes.ArcGISUnknownLayer)]
		public LayerTypes Type;
		[FileSelector]
		public string Source;
		[Range(0, 1)]
		public float Opacity;
		public bool IsVisible;
		public OAuthAuthenticationConfigurationMapping Authentication;
		public ArcGISBuildingSceneLayerFilterInstanceData BuildingSceneLayerFilter = new ArcGISBuildingSceneLayerFilterInstanceData();
		[HideInInspector]
		public ArcGISMeshModificationsInstanceData MeshModifications = new ArcGISMeshModificationsInstanceData();
		[HideInInspector]
		public ArcGISSpatialFeatureFilterInstanceData SpatialFeatureFilter = new ArcGISSpatialFeatureFilterInstanceData();
		[NonSerialized]
		public ArcGISLayer APIObject;

		public override bool Equals(object obj)
		{
			return obj is ArcGISLayerInstanceData data &&
				   Name == data.Name &&
				   Type == data.Type &&
				   Source == data.Source &&
				   Opacity == data.Opacity &&
				   IsVisible == data.IsVisible &&
				   EqualityComparer<OAuthAuthenticationConfigurationMapping>.Default.Equals(Authentication, data.Authentication) &&
				   EqualityComparer<ArcGISBuildingSceneLayerFilterInstanceData>.Default.Equals(BuildingSceneLayerFilter, data.BuildingSceneLayerFilter) &&
				   EqualityComparer<ArcGISMeshModificationsInstanceData>.Default.Equals(MeshModifications, data.MeshModifications) &&
				   EqualityComparer<ArcGISSpatialFeatureFilterInstanceData>.Default.Equals(SpatialFeatureFilter, data.SpatialFeatureFilter);
		}

		public override int GetHashCode()
		{
			HashCode hash = new HashCode();
			hash.Add(Name);
			hash.Add(Type);
			hash.Add(Source);
			hash.Add(Opacity);
			hash.Add(IsVisible);
			hash.Add(Authentication);
			hash.Add(BuildingSceneLayerFilter);
			hash.Add(MeshModifications);
			hash.Add(SpatialFeatureFilter);
			return hash.ToHashCode();
		}

		public static bool operator ==(ArcGISLayerInstanceData left, ArcGISLayerInstanceData right)
		{
			return EqualityComparer<ArcGISLayerInstanceData>.Default.Equals(left, right);
		}

		public static bool operator !=(ArcGISLayerInstanceData left, ArcGISLayerInstanceData right)
		{
			return !(left == right);
		}
	}
}

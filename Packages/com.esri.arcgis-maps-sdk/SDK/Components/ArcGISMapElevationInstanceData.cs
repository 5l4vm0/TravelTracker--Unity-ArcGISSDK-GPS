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

namespace Esri.ArcGISMapsSDK.Components
{
	[Serializable]
	public class ArcGISMapElevationInstanceData
	{
		public List<ArcGISElevationSourceInstanceData> ElevationSources = new List<ArcGISElevationSourceInstanceData>();
		public ArcGISMeshModificationsInstanceData MeshModifications = new ArcGISMeshModificationsInstanceData();

		public override bool Equals(object obj)
		{
			return obj is ArcGISMapElevationInstanceData data &&
				   EqualityComparer<List<ArcGISElevationSourceInstanceData>>.Default.Equals(ElevationSources, data.ElevationSources) &&
				   EqualityComparer<ArcGISMeshModificationsInstanceData>.Default.Equals(MeshModifications, data.MeshModifications);
		}

		public override int GetHashCode()
		{
			return HashCode.Combine(ElevationSources, MeshModifications);
		}

		public static bool operator ==(ArcGISMapElevationInstanceData left, ArcGISMapElevationInstanceData right)
		{
			return EqualityComparer<ArcGISMapElevationInstanceData>.Default.Equals(left, right);
		}

		public static bool operator !=(ArcGISMapElevationInstanceData left, ArcGISMapElevationInstanceData right)
		{
			return !(left == right);
		}
	}
}

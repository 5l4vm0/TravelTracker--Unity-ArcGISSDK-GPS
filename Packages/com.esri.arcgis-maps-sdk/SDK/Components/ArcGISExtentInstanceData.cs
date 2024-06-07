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
using Esri.ArcGISMapsSDK.SDK.Utils;
using Esri.GameEngine.Geometry;
using System;
using System.Collections.Generic;
using Unity.Mathematics;

namespace Esri.ArcGISMapsSDK.Components
{
	[Serializable]
	public struct ArcGISExtentInstanceData
	{
		[HideAltitude]
		public ArcGISPoint GeographicCenter;
		public MapExtentShapes ExtentShape;
		public double2 ShapeDimensions;
		public bool UseOriginAsCenter;

		public override bool Equals(object obj)
		{
			return obj is ArcGISExtentInstanceData data &&
				   EqualityComparer<ArcGISPoint>.Default.Equals(GeographicCenter, data.GeographicCenter) &&
				   ExtentShape == data.ExtentShape &&
				   ShapeDimensions.Equals(data.ShapeDimensions) &&
				   UseOriginAsCenter == data.UseOriginAsCenter;
		}

		public override int GetHashCode()
		{
			return HashCode.Combine(GeographicCenter, ExtentShape, ShapeDimensions, UseOriginAsCenter);
		}

		public static bool operator !=(ArcGISExtentInstanceData lhs, ArcGISExtentInstanceData rhs)
		{
			return !lhs.Equals(rhs);
		}

		public static bool operator ==(ArcGISExtentInstanceData lhs, ArcGISExtentInstanceData rhs)
		{
			return lhs.Equals(rhs);
		}
	}
}

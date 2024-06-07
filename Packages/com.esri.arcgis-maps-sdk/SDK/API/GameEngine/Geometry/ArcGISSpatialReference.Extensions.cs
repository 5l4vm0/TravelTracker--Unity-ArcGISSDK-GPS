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

namespace Esri.GameEngine.Geometry
{
	public partial class ArcGISSpatialReference : ICloneable
	{
		public object Clone()
		{
			return new ArcGISSpatialReference(WKID, VerticalWKID);
		}

		public override bool Equals(object obj)
		{
			return obj is ArcGISSpatialReference data && Equals(data);
		}

		public override int GetHashCode()
		{
			return Handle.GetHashCode();
		}

		public static bool operator !=(ArcGISSpatialReference lhs, ArcGISSpatialReference rhs)
		{
			return !(lhs == rhs);
		}

		public static bool operator ==(ArcGISSpatialReference lhs, ArcGISSpatialReference rhs)
		{
			if (ReferenceEquals(lhs, rhs))
			{
				return true;
			}

			if (((object)lhs == null) || ((object)rhs == null))
			{
				return false;
			}

			return lhs.Equals(rhs);
		}
	}
}

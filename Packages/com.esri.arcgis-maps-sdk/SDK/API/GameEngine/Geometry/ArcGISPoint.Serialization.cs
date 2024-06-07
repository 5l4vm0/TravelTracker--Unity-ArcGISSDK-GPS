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
using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace Esri.GameEngine.Geometry
{
	[Serializable]
	public partial class ArcGISPoint : ISerializationCallbackReceiver
	{
		[FormerlySerializedAs("X")]
		[SerializeField]
		private double x;

		[FormerlySerializedAs("Y")]
		[SerializeField]
		private double y;

		[FormerlySerializedAs("Z")]
		[SerializeField]
		private double z;

		[SerializeField]
		private int SRWkid;

		public void OnBeforeSerialize()
		{
			try
			{
				x = X;
				y = Y;
				z = Z;
				SRWkid = SpatialReference.WKID;
			}
			catch
			{
				x = 0;
				y = 0;
				z = 0;
				SRWkid = 4326;
			}
		}

		public void OnAfterDeserialize()
		{
			ArcGISSpatialReference spatialReference;

			try
			{
				spatialReference = new ArcGISSpatialReference(SRWkid);
			}
			catch
			{
				SRWkid = 4326;

				spatialReference = new ArcGISSpatialReference(SRWkid);
			}

			var errorHandler = Unity.ArcGISErrorManager.CreateHandler();

			if (Handle != IntPtr.Zero)
			{
				PInvoke.RT_Geometry_destroy(Handle, errorHandler);

				Unity.ArcGISErrorManager.CheckError(errorHandler);
			}

			errorHandler = Unity.ArcGISErrorManager.CreateHandler();

			var localSpatialReference = spatialReference == null ? IntPtr.Zero : spatialReference.Handle;

			Handle = PInvoke.RT_Point_createWithXYZSpatialReference(x, y, z, localSpatialReference, errorHandler);

			Unity.ArcGISErrorManager.CheckError(errorHandler);
		}
	}
}


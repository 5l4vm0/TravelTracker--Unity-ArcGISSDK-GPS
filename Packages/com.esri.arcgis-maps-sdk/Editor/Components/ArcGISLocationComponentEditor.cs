// COPYRIGHT 1995-2021 ESRI
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
using Esri.GameEngine.Geometry;
using UnityEditor;

namespace Esri.ArcGISMapsSDK.Editor.Components
{
	[CustomEditor(typeof(ArcGISLocationComponent))]
	public class ArcGISLocationComponentEditor : UnityEditor.Editor
	{
		public override void OnInspectorGUI()
		{
			var locationComponent = target as ArcGISLocationComponent;

			if (locationComponent.GetComponentInParent<ArcGISMapComponent>() == null)
			{
				EditorGUILayout.HelpBox("GameObjects with an ArcGIS Location Component attached must be child to a GameObject with an ArcGIS Map Component attached.", MessageType.Warning);
			}

			if (locationComponent.enabled)
			{
				var position = locationComponent.Position;
				var rotation = locationComponent.Rotation;

				if (Draw(ref position, ref rotation))
				{
					locationComponent.Position = position;
					locationComponent.Rotation = rotation;

					EditorUtility.SetDirty(locationComponent);
				}
			}
		}

		private static bool Draw(ref ArcGISPoint position, ref ArcGISRotation rotation)
		{
			bool result = false;

			var oldPosition = position;
			position = EditorUtilities.ArcGISPointField("Position", position, false);

			if (!oldPosition.IsValid || oldPosition.X != position.X || oldPosition.Y != position.Y || oldPosition.Z != position.Z || oldPosition.SpatialReference.WKID != position.SpatialReference.WKID)
			{
				result = true;
			}

			var oldRotation = rotation;
			rotation = EditorUtilities.ArcGISRotationField("Rotation", rotation);

			if (oldRotation != rotation)
			{
				result = true;
			}

			return result;
		}
	}
}

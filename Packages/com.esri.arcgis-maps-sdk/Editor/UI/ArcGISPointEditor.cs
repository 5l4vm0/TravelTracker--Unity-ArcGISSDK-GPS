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
using Esri.ArcGISMapsSDK.Utils.GeoCoord;
using Esri.GameEngine.Geometry;
using Esri.Unity;
using System;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Esri.ArcGISMapsSDK.Editor.UI
{
	[CustomPropertyDrawer(typeof(ArcGISPoint))]
	public class ArcGISPointEditor : PropertyDrawer
	{
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			var hideAltitude = fieldInfo.GetCustomAttributes<HideAltitudeAttribute>().ToArray().Length > 0;

			var xProp = property.FindPropertyRelative("x");
			var yProp = property.FindPropertyRelative("y");
			var zProp = property.FindPropertyRelative("z");
			var srProp = property.FindPropertyRelative("SRWkid");

			var xLabel = "X";
			var yLabel = "Y";
			var zLabel = "Z";
			var srLabel = "Spatial Reference WKID";

			if (srProp.intValue == SpatialReferenceWkid.WGS84 || srProp.intValue == SpatialReferenceWkid.CGCS2000)
			{
				xLabel = "Longitude";
				yLabel = "Latitude";
				zLabel = "Altitude";
			}

			int rectIndex = 0;

			Func<Rect> GetRect = () =>
			{
				return new Rect(position.x, position.y + rectIndex++ * EditorGUIUtility.singleLineHeight + (rectIndex - 1) * EditorGUIUtility.standardVerticalSpacing, position.width, EditorGUIUtility.singleLineHeight);
			};

			using (var propertyScope = new EditorGUI.PropertyScope(position, label, property))
			using (var checkScope = new EditorGUI.ChangeCheckScope())
			{
				EditorGUI.LabelField(GetRect(), label);

				using (new EditorGUI.IndentLevelScope())
				{
					EditorGUI.PropertyField(GetRect(), xProp, new GUIContent(xLabel));
					EditorGUI.PropertyField(GetRect(), yProp, new GUIContent(yLabel));

					if (!hideAltitude)
					{
						EditorGUI.PropertyField(GetRect(), zProp, new GUIContent(zLabel));
					}

					EditorGUI.PropertyField(GetRect(), srProp, new GUIContent(srLabel));

					if (checkScope.changed)
					{
						EditorUtility.SetDirty(property.serializedObject.targetObject);
					}
				}
			}
		}

		public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
		{
			var hideAltitude = fieldInfo.GetCustomAttributes<HideAltitudeAttribute>().ToArray().Length > 0;

			var rows = hideAltitude ? 4 : 5;

			return rows * EditorGUIUtility.singleLineHeight + (rows - 1) * EditorGUIUtility.standardVerticalSpacing;
		}
	}
}

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
using Esri.ArcGISMapsSDK.Security;
using Esri.ArcGISMapsSDK.Components;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Esri.ArcGISMapsSDK.Editor.UI
{
	[CustomPropertyDrawer(typeof(OAuthAuthenticationConfigurationMapping))]
	public class OAuthAuthenticationConfigurationMappingEditor : PropertyDrawer
	{
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			var indexProperty = property.FindPropertyRelative("ConfigurationIndex");

			EditorGUI.BeginProperty(position, label, property);
			position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);
			var indent = EditorGUI.indentLevel;
			EditorGUI.indentLevel = 0;

			var propRect = new Rect(position.x, position.y, position.width, position.height);

			var configNames = new List<string>();
			configNames.Add("None");

			var configurations = OAuthAuthenticationConfigurationMappingExtensions.Configurations;

			if (configurations != null)
			{
				for (var i = 0; i < configurations.Count; i++)
				{
					configNames.Add(configurations[i].Name);
				}
			}

			EditorGUI.BeginChangeCheck();
			var selectedIndex = EditorGUI.Popup(propRect, indexProperty.intValue + 1, configNames.ToArray());
			if (EditorGUI.EndChangeCheck())
			{
				indexProperty.intValue = selectedIndex - 1;
				EditorUtility.SetDirty(indexProperty.serializedObject.targetObject);
			}

			EditorGUI.indentLevel = indent;
			EditorGUI.EndProperty();
		}
	}
}

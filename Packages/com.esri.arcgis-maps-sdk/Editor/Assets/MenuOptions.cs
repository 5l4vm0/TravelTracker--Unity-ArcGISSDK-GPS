using Esri.ArcGISMapsSDK.Components;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Esri.ArcGISMapsSDK.Editor.Assets
{
	static class MenuOptions
	{
		[MenuItem("GameObject/ArcGISMaps SDK/Camera", false, 10)]
		static void AddCamera(MenuCommand menuCommand)
		{
			var cameraComponentGameObject = new GameObject("ArcGISCamera");

			cameraComponentGameObject.AddComponent<Camera>();

			if (SceneView.lastActiveSceneView != null)
			{
				cameraComponentGameObject.transform.position = SceneView.lastActiveSceneView.camera.transform.position;
				cameraComponentGameObject.transform.rotation = SceneView.lastActiveSceneView.camera.transform.rotation;
			}

			if (!Camera.main)
			{
				cameraComponentGameObject.tag = "MainCamera";
			}

			AttachToMap(cameraComponentGameObject, menuCommand);

			cameraComponentGameObject.AddComponent<ArcGISCameraComponent>();

			Undo.RegisterCreatedObjectUndo(cameraComponentGameObject, "Create " + cameraComponentGameObject.name);

			Selection.activeGameObject = cameraComponentGameObject;
		}

		[MenuItem("GameObject/ArcGISMaps SDK/Map", false, 10)]
		static void AddMap(MenuCommand menuCommand)
		{
			CreateMapGameObject();
		}

		static void AttachToMap(GameObject element, MenuCommand menuCommand)
		{
			GameObject parent = menuCommand.context as GameObject;

			if (parent == null)
			{
				parent = GetOrCreateMapGameObject();
			}

			SceneManager.MoveGameObjectToScene(element, parent.scene);

			if (element.transform.parent == null)
			{
				Undo.SetTransformParent(element.transform, parent.transform, true, "Parent " + element.name);
			}

			GameObjectUtility.EnsureUniqueNameForSibling(element);
		}

		static GameObject CreateMapGameObject()
		{
			var mapComponentGameObject = new GameObject("ArcGISMap");

			mapComponentGameObject.AddComponent<ArcGISMapComponent>();

			GameObjectUtility.EnsureUniqueNameForSibling(mapComponentGameObject);

			Undo.RegisterCreatedObjectUndo(mapComponentGameObject, "Create " + mapComponentGameObject.name);

			return mapComponentGameObject;
		}

		static GameObject GetOrCreateMapGameObject()
		{
			GameObject selectedGo = Selection.activeGameObject;

			var mapComponent = selectedGo != null ? selectedGo.GetComponentInParent<ArcGISMapComponent>() : null;

			if (IsValidMap(mapComponent))
			{
				return mapComponent.gameObject;
			}

			ArcGISMapComponent[] mapComponents = StageUtility.GetCurrentStageHandle().FindComponentsOfType<ArcGISMapComponent>();

			for (int i = 0; i < mapComponents.Length; i++)
			{
				if (IsValidMap(mapComponents[i]))
				{
					return mapComponents[i].gameObject;
				}
			}

			return CreateMapGameObject();
		}

		static bool IsValidMap(ArcGISMapComponent mapComponent)
		{
			if (mapComponent == null || !mapComponent.gameObject.activeInHierarchy)
			{
				return false;
			}

			return true;
		}
	}
}

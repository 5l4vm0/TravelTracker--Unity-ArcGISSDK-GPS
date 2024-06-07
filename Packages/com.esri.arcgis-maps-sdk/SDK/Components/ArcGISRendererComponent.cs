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
using Esri.ArcGISMapsSDK.Renderer;
using Esri.ArcGISMapsSDK.Renderer.Renderables;
using Esri.ArcGISMapsSDK.Utils;
using Unity.Mathematics;
using UnityEngine;

namespace Esri.ArcGISMapsSDK.Components
{
	[DisallowMultipleComponent]
	[ExecuteAlways]
	[AddComponentMenu("")]
	public class ArcGISRendererComponent : MonoBehaviour
	{
		public bool disableTerrainOcclusion = false;

		private ArcGISMapComponent mapComponent;
		private ArcGISRenderer arcGISRenderer = null;

#if USE_HDRP_PACKAGE || USE_URP_PACKAGE
		public ArcGISTerrainOcclusionRenderer IntegratedMeshOcclusionRender { get; private set; } = null;
#endif

		public event ArcGISExtentUpdatedEventHandler ExtentUpdated;

		private void OnEnable()
		{
			mapComponent = gameObject.GetComponentInParent<ArcGISMapComponent>();

#if UNITY_EDITOR
			UnityEditor.EditorApplication.update += OnForceRenderUpdate;
#endif

			if (mapComponent)
			{
				mapComponent.EditorModeEnabledChanged += new ArcGISMapComponent.EditorModeEnabledChangedEventHandler(OnEditorModeEnabledChanged);
				mapComponent.MeshCollidersEnabledChanged += new ArcGISMapComponent.MeshCollidersEnabledChangedEventHandler(OnMeshCollidersEnabledChanged);
			}

#if USE_HDRP_PACKAGE || USE_URP_PACKAGE
			if (!disableTerrainOcclusion && IntegratedMeshOcclusionRender == null)
			{
				IntegratedMeshOcclusionRender = new ArcGISTerrainOcclusionRenderer(GetArcGISRenderer().RenderableProvider, GetWorldMatrix);
			}
#endif
		}

		private double4x4 GetWorldMatrix()
		{
			return mapComponent ? mapComponent.WorldMatrix : double4x4.identity;
		}

		private void OnDisable()
		{
#if UNITY_EDITOR
			UnityEditor.EditorApplication.update -= OnForceRenderUpdate;
#endif

			if (mapComponent)
			{
				mapComponent.EditorModeEnabledChanged -= new ArcGISMapComponent.EditorModeEnabledChangedEventHandler(OnEditorModeEnabledChanged);
				mapComponent.MeshCollidersEnabledChanged -= new ArcGISMapComponent.MeshCollidersEnabledChangedEventHandler(OnMeshCollidersEnabledChanged);
			}

#if USE_HDRP_PACKAGE || USE_URP_PACKAGE
			if (IntegratedMeshOcclusionRender != null)
			{
				IntegratedMeshOcclusionRender.Release();
				IntegratedMeshOcclusionRender = null;
			}
#endif
		}

		private void Start()
		{
			if (mapComponent == null)
			{
				Debug.LogError("An ArcGISMapComponent could not be found. Please make sure this GameObject is a child of a GameObject with an ArcGISMapComponent attached");

				enabled = false;
				return;
			}
		}

		private void Update()
		{
			if (mapComponent != null && mapComponent.ShouldEditorComponentBeUpdated() && GetArcGISRenderer() != null)
			{
				GetArcGISRenderer().Update();
			}
		}

		private void OnDestroy()
		{
			if (arcGISRenderer != null)
			{
				arcGISRenderer.Release();
			}
		}

		private void OnTransformParentChanged()
		{
			OnEnable();
		}

		internal IRenderable GetRenderableByGameObject(GameObject gameObject)
		{
			return GetArcGISRenderer().GetRenderableByGameObject(gameObject);
		}

		private ArcGISRenderer GetArcGISRenderer()
		{
			if (arcGISRenderer == null)
			{
				if (mapComponent.View != null)
				{
					arcGISRenderer = new ArcGISRenderer(mapComponent.View, gameObject, mapComponent.MeshCollidersEnabled && Application.isPlaying);

					arcGISRenderer.ExtentUpdated += delegate (ArcGISExtentUpdatedEventArgs e)
					{
						ExtentUpdated?.Invoke(e);
					};
				}
			}

			return arcGISRenderer;
		}

		private void ResetArcGISRenderer()
		{
			if (arcGISRenderer != null)
			{
				arcGISRenderer.Release();
			}

			arcGISRenderer = null;
		}

		private void OnEditorModeEnabledChanged()
		{
			if (mapComponent != null && !mapComponent.ShouldEditorComponentBeUpdated())
			{
				ResetArcGISRenderer();
			}
		}

		private void OnMeshCollidersEnabledChanged()
		{
			if (mapComponent != null && arcGISRenderer != null)
			{
				var areMeshCollidersEnabled = mapComponent.MeshCollidersEnabled && Application.isPlaying;
#if UNITY_EDITOR
				if (ArcGISProjectSettingsAsset.Instance?.ForceCollisionEnabledInEditorWorld == true)
				{
					areMeshCollidersEnabled = true;
				}
#endif
				arcGISRenderer.AreMeshCollidersEnabled = areMeshCollidersEnabled;
			}
		}

#if UNITY_EDITOR
		private void OnForceRenderUpdate()
		{
			if (!Application.isPlaying && mapComponent.ShouldEditorComponentBeUpdated())
			{
				UnityEditor.EditorApplication.QueuePlayerLoopUpdate();
				UnityEditor.SceneView.RepaintAll();
			}
		}
#endif
	}
}

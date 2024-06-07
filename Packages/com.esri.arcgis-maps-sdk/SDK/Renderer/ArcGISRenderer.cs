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
using Esri.ArcGISMapsSDK.Renderer.GPUComputing;
using Esri.ArcGISMapsSDK.Renderer.GPUResources;
using Esri.ArcGISMapsSDK.Renderer.Renderables;
using Esri.GameEngine.RCQ;
using Esri.GameEngine.View;
using Unity.Mathematics;
using UnityEngine;

namespace Esri.ArcGISMapsSDK.Renderer
{
	public class ArcGISRenderer
	{
		private readonly RenderCommandClient renderCommandClient;
		private readonly ArcGISView view;
		private readonly INormalMapGenerator normalMapGenerator;
		private readonly IImageComposer imageComposer;
		private readonly GPUResourcesProvider gpuResourceProvider = new GPUResourcesProvider();
		private readonly RenderableProvider renderableProvider;

		private readonly RenderCommandThrottle renderCommandThrottle = new RenderCommandThrottle();
		private static readonly bool throttlingManagerEnabled = true;

		private DecodedRenderCommandQueue currentRenderCommandQueue;

		internal IRenderableProvider RenderableProvider => renderableProvider;

		public event ArcGISExtentUpdatedEventHandler ExtentUpdated;

		public bool AreMeshCollidersEnabled
		{
			get
			{
				return renderableProvider.AreMeshCollidersEnabled;
			}
			set
			{
				renderableProvider.AreMeshCollidersEnabled = value;
			}
		}

		public ArcGISRenderer(ArcGISView view, GameObject gameObject, bool areMeshCollidersEnabled)
		{
			while (gameObject.transform.childCount != 0)
			{
#if UNITY_EDITOR
				if (Application.isEditor)
				{
					GameObject.DestroyImmediate(gameObject.transform.GetChild(0).gameObject);
				}
				else
#endif
				{
					GameObject.Destroy(gameObject.transform.GetChild(0).gameObject);
				}
			}

			if (SystemInfo.graphicsDeviceType == UnityEngine.Rendering.GraphicsDeviceType.OpenGLES3 ||
					SystemInfo.graphicsDeviceType == UnityEngine.Rendering.GraphicsDeviceType.Metal)
			{
				normalMapGenerator = new NormalMapGeneratorPS(view);
				imageComposer = new ImageComposerPS();
			}
			else
			{
				normalMapGenerator = new NormalMapGeneratorCS(view);
				imageComposer = new ImageComposerCS();
			}

			this.view = view;
			renderableProvider = new RenderableProvider(500, gameObject, areMeshCollidersEnabled);
			renderCommandClient = new RenderCommandClient(gpuResourceProvider, renderableProvider, imageComposer, normalMapGenerator);

			renderCommandClient.ExtentUpdated += delegate (ArcGISExtentUpdatedEventArgs e)
			{
				ExtentUpdated?.Invoke(e);
			};

			currentRenderCommandQueue = new DecodedRenderCommandQueue(view.RenderCommandServer.GetRenderCommands());
		}

		public void Update()
		{
			if (view.SpatialReference == null)
			{
				// The normal map generator compute shader needs a spatial reference
				return;
			}

			var renderCommandServer = view.RenderCommandServer;

			if (!throttlingManagerEnabled)
			{
				currentRenderCommandQueue = new DecodedRenderCommandQueue(renderCommandServer.GetRenderCommands());

				var renderCommand = currentRenderCommandQueue.GetNextCommand();

				while (renderCommand != null)
				{
					var callbackTokens = renderCommandClient.ExecuteRenderCommand(renderCommand);

					if (callbackTokens.Length > 0)
					{
						var nativeArray = new global::Unity.Collections.NativeArray<ulong>(callbackTokens, global::Unity.Collections.Allocator.Temp);

						renderCommandServer.NotifyRenderableHasGPUResources(nativeArray.Reinterpret<byte>(sizeof(ulong)));
					}

					renderCommand = currentRenderCommandQueue.GetNextCommand();
				}
			}
			else
			{
				renderCommandThrottle.Clear();

				var renderCommand = currentRenderCommandQueue.GetNextCommand();

				if (renderCommand == null)
				{
					currentRenderCommandQueue = new DecodedRenderCommandQueue(renderCommandServer.GetRenderCommands());
					renderCommand = currentRenderCommandQueue.GetNextCommand();
				}

				while (renderCommand != null)
				{
					var callbackTokens = renderCommandClient.ExecuteRenderCommand(renderCommand);

					if (callbackTokens.Length > 0)
					{
						var nativeArray = new global::Unity.Collections.NativeArray<ulong>(callbackTokens, global::Unity.Collections.Allocator.Temp);

						renderCommandServer.NotifyRenderableHasGPUResources(nativeArray.Reinterpret<byte>(sizeof(ulong)));
					}

					if (renderCommandThrottle.DoThrottle(renderCommand))
					{
						// Break and defer processing of the remaining commands to next Update
						break;
					}

					renderCommand = currentRenderCommandQueue.GetNextCommand();
				}
			}
		}

		public void Release()
		{
			gpuResourceProvider.Release();
			renderableProvider.Release();
		}

		internal IRenderable GetRenderableByGameObject(GameObject gameObject)
		{
			return renderableProvider.GetRenderableFrom(gameObject);
		}
	}
}

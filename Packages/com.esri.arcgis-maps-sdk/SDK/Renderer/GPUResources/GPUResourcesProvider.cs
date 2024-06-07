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
using Esri.GameEngine.RCQ;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace Esri.ArcGISMapsSDK.Renderer.GPUResources
{
	internal class GPUResourcesProvider : IGPUResourcesProvider
	{
		private readonly Dictionary<uint, IGPUResourceMaterial> materials = new Dictionary<uint, IGPUResourceMaterial>();
		private readonly Dictionary<uint, IGPUResourceMesh> meshes = new Dictionary<uint, IGPUResourceMesh>();
		private readonly Dictionary<uint, IGPUResourceTexture2D> textures = new Dictionary<uint, IGPUResourceTexture2D>();
		private readonly Dictionary<uint, IGPUResourceRenderTexture> renderTextures = new Dictionary<uint, IGPUResourceRenderTexture>();

		public IReadOnlyDictionary<uint, IGPUResourceMaterial> Materials => materials;
		public IReadOnlyDictionary<uint, IGPUResourceMesh> Meshes => meshes;
		public IReadOnlyDictionary<uint, IGPUResourceTexture2D> Textures => textures;
		public IReadOnlyDictionary<uint, IGPUResourceRenderTexture> RenderTextures => renderTextures;

		public IGPUResourceMaterial CreateMaterial(uint id, ArcGISRenderableType renderableType, Material customMaterial)
		{
			Material material = null;

			if (customMaterial)
			{
				material = new Material(customMaterial);
			}
			else
			{
				var shaderPath = renderableType == ArcGISRenderableType.Tile ? "TileSurface" : "SceneNodeSurface";

				if (GraphicsSettings.renderPipelineAsset != null)
				{
					var renderType = GraphicsSettings.renderPipelineAsset.GetType().ToString();

#if USE_URP_PACKAGE
					if (renderType == "UnityEngine.Rendering.Universal.UniversalRenderPipelineAsset")
					{
						material = new Material(Resources.Load<Material>("Materials/URP/" + shaderPath));
					}
#endif

#if USE_HDRP_PACKAGE
					if (renderType == "UnityEngine.Rendering.HighDefinition.HDRenderPipelineAsset")
					{
						material = new Material(Resources.Load<Material>("Materials/HDRP/" + shaderPath));
					}
#endif
				}

				if (material == null)
				{
					material = new Material(Resources.Load<Shader>("Shaders/Legacy/" + shaderPath));
				}
			}

			var resourceMaterial = new GPUResourceMaterial(material);
			materials.Add(id, resourceMaterial);

			return resourceMaterial;
		}

		public IGPUResourceMesh CreateMesh(uint id)
		{
			var resourceMesh = new GPUResourceMesh(new Mesh());
			meshes.Add(id, resourceMesh);

			return resourceMesh;
		}

		public IGPUResourceRenderTexture CreateRenderTexture(uint id, uint width, uint height, ArcGISTextureFormat format, bool useMipMaps)
		{
			var rendertextureFormat = GetUnityRendertextureFormatFromInternal(format);

			var nativeRenderTexture = new RenderTexture((int)width, (int)height, 0, rendertextureFormat, RenderTextureReadWrite.Linear);
			nativeRenderTexture.enableRandomWrite = true;
			nativeRenderTexture.autoGenerateMips = false;
			nativeRenderTexture.useMipMap = useMipMaps;
			nativeRenderTexture.anisoLevel = 9;
			nativeRenderTexture.Create();

			var resourceRenderTexture = new GPUResourceRenderTexture(nativeRenderTexture);
			renderTextures.Add(id, resourceRenderTexture);

			return resourceRenderTexture;
		}

		public IGPUResourceTexture2D CreateTexture(uint id, uint width, uint height, ArcGISTextureFormat format, bool isSRGB)
		{
			var textureFormat = GetUnityTextureFormatFromInternal(format);
			Texture2D nativeTexture = new Texture2D((int)width, (int)height, textureFormat, false, !isSRGB);

			nativeTexture.wrapMode = TextureWrapMode.Clamp;
			nativeTexture.filterMode = FilterMode.Bilinear;

			var resourceTexture = new GPUResourceTexture2D(nativeTexture);
			textures.Add(id, resourceTexture);

			return resourceTexture;
		}

		public void DestroyMaterial(uint id)
		{
			Debug.Assert(materials.ContainsKey(id));

			var material = materials[id];
			material.Destroy();
			materials.Remove(id);
		}

		public void DestroyMesh(uint id)
		{
			Debug.Assert(meshes.ContainsKey(id));

			var mesh = meshes[id];
			mesh.Destroy();
			meshes.Remove(id);
		}

		public void DestroyTexture(uint id)
		{
			Debug.Assert(textures.ContainsKey(id));

			var texture = textures[id];
			texture.Destroy();
			textures.Remove(id);
		}

		public void DestroyRenderTexture(uint id)
		{
			Debug.Assert(renderTextures.ContainsKey(id));

			var renderTexture = renderTextures[id];
			renderTexture.Release();
			renderTexture.Destroy();
			renderTextures.Remove(id);
		}

		public void Release()
		{
			foreach (var texture in textures)
			{
				texture.Value.Destroy();
			}

			foreach (var renderTexture in renderTextures)
			{
				renderTexture.Value.Release();
				renderTexture.Value.Destroy();
			}

			textures.Clear();
			renderTextures.Clear();
		}

		private static UnityEngine.TextureFormat GetUnityTextureFormatFromInternal(ArcGISTextureFormat textureFormat)
		{
			switch (textureFormat)
			{
				case ArcGISTextureFormat.RGB8UNorm:
					return UnityEngine.TextureFormat.RGB24;
				case ArcGISTextureFormat.R32Float:
					return UnityEngine.TextureFormat.RFloat;
				case ArcGISTextureFormat.RGBA16UNorm:
					return UnityEngine.TextureFormat.RGBAFloat;
				case ArcGISTextureFormat.RGBA8UNorm:
					return UnityEngine.TextureFormat.RGBA32;
				case ArcGISTextureFormat.BGRA8UNorm:
					return UnityEngine.TextureFormat.BGRA32;
				case ArcGISTextureFormat.DXT1:
					return UnityEngine.TextureFormat.DXT1;
				case ArcGISTextureFormat.DXT5:
					return UnityEngine.TextureFormat.DXT5;
				case ArcGISTextureFormat.RGBA32Float:
					return UnityEngine.TextureFormat.RGBAFloat;
				case ArcGISTextureFormat.ETC2RGB8:
				case ArcGISTextureFormat.ETC2SRGB8:
					return UnityEngine.TextureFormat.ETC2_RGB;
				case ArcGISTextureFormat.ETC2RGB8PunchthroughAlpha1:
				case ArcGISTextureFormat.ETC2SRGB8PunchthroughAlpha1:
					return UnityEngine.TextureFormat.ETC2_RGBA1;
				case ArcGISTextureFormat.ETC2EacRGBA8:
				case ArcGISTextureFormat.ETC2EacSRGBA8:
					return UnityEngine.TextureFormat.ETC2_RGBA8;
				case ArcGISTextureFormat.R32Norm:
					return UnityEngine.TextureFormat.RGBA32;
				default:
					Debug.LogError("Texture format not supported!");
					return UnityEngine.TextureFormat.RGB24;
			}
		}

		private static RenderTextureFormat GetUnityRendertextureFormatFromInternal(ArcGISTextureFormat textureFormat)
		{
			switch (textureFormat)
			{
				case ArcGISTextureFormat.RGBA8UNorm:
					return RenderTextureFormat.ARGB32;
				case ArcGISTextureFormat.BGRA8UNorm:
					return RenderTextureFormat.BGRA32;
				case ArcGISTextureFormat.R32Float:
					return RenderTextureFormat.RFloat;
				case ArcGISTextureFormat.RGBA16UNorm:
					return RenderTextureFormat.ARGBHalf;
				case ArcGISTextureFormat.RGBA32Float:
					return RenderTextureFormat.ARGBFloat;
				default:
					Debug.LogError("RenderTexture format not supported!");
					return RenderTextureFormat.ARGB32;
			}
		}
	}
}

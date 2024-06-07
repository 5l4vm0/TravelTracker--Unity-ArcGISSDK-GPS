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
using Esri.ArcGISMapsSDK.Renderer.GPUResources;
using UnityEngine;

namespace Esri.ArcGISMapsSDK.Renderer.GPUComputing
{
	internal class ImageComposerPS : IImageComposer
	{
		private readonly Material material = null;
		private RenderTexture tempRenderTexture = null;
		private const int numTexturesPerPass = 7;

		public ImageComposerPS()
		{
			material = new Material(Resources.Load<Shader>("Shaders/Utils/PS/BlendImage"));
		}

		public void Compose(ComposableImage[] inputImages, IGPUResourceRenderTexture output)
		{
			UpdateTempTexture(output.Width, output.Height);

			int numIterations = inputImages.Length / numTexturesPerPass + (inputImages.Length % numTexturesPerPass == 0 ? 0 : 1);
			float[] opacities = new float[numTexturesPerPass];
			Vector4[] offsets = new Vector4[numTexturesPerPass];

			RenderTexture[] switchingRenderTargets = { tempRenderTexture, output.NativeRenderTexture };
			int switchingRenderTargetsIndex = 0;

			for (int i = 0; i < numIterations; i++)
			{
				int numTexturesPerIteration = i == numIterations - 1 && inputImages.Length % numTexturesPerPass != 0 ? inputImages.Length % numTexturesPerPass : numTexturesPerPass;

				for (int tex = 0; tex < numTexturesPerPass; tex++)
				{
					var texture = Texture2D.blackTexture;

					if (tex < numTexturesPerIteration)
					{
						opacities[tex] = inputImages[(numTexturesPerPass * i + tex)].opacity;
						offsets[tex] = inputImages[(numTexturesPerPass * i + tex)].extent;

						texture = inputImages[(numTexturesPerPass * i + tex)].image.NativeTexture;
					}

					material.SetTexture("Input" + tex, texture);
				}

				material.SetInt("OutputShouldBeSampled", i != 0 ? 1 : 0);
				material.SetFloatArray("Opacities", opacities);
				material.SetVectorArray("OffsetsAndScales", offsets);
				material.SetInt("NumTextures", numTexturesPerIteration);
				material.SetTexture("LastOutputRenderTexture", switchingRenderTargets[switchingRenderTargetsIndex]);

				Graphics.Blit(switchingRenderTargets[switchingRenderTargetsIndex], switchingRenderTargets[(switchingRenderTargetsIndex + 1) % 2], material);
				switchingRenderTargetsIndex = (switchingRenderTargetsIndex + 1) % 2;
			}

			if (numIterations % 2 == 0)
			{
				Graphics.Blit(switchingRenderTargets[switchingRenderTargetsIndex], switchingRenderTargets[(switchingRenderTargetsIndex + 1) % 2]);
			}
		}

		private void UpdateTempTexture(int width, int height)
		{
			if (tempRenderTexture == null || tempRenderTexture.width != width || tempRenderTexture.height != height)
			{
				if (tempRenderTexture != null)
				{
					tempRenderTexture.Release();
				}

				tempRenderTexture = new RenderTexture((int)width, (int)height, 0, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Linear);
				tempRenderTexture.autoGenerateMips = false;
				tempRenderTexture.useMipMap = false;
				tempRenderTexture.Create();
			}
		}
	}
}

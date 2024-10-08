void UVRegionTransform_float(float2 uv, Texture2D lut, float lutIndex, float useUVRegionLUT, out float2 outUV)
{
	float4 value = useUVRegionLUT == 1.0 ?
		lut.Load(int3((uint)(lutIndex + 0.5f), 0, 0)) :
		float4(0, 0, 1, 1);

	if (value.x == 0 && value.y == 0 && value.z == 0 && value.w == 0)
	{
		outUV = uv;
	}
	else
	{
		outUV = frac(uv) * (value.zw - value.xy) + value.xy;
	}
}
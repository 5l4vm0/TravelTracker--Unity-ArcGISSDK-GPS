Shader "Custom/GrayFilterWithAlphaOnClick"
{
    Properties
    {
        _MainTex("Texture", 2D) = "white" {}
        _MaskTex("Mask Texture", 2D) = "white"{}
        _FilterStrength("Filter Strength", Range(0, 1)) = 1.0
        _Alpha("Alpha", Range(0, 1)) = 1.0

    }
        SubShader
        {
            Tags { "RenderType" = "Transparent" "Queue" = "Transparent" }
            LOD 200

            Pass
            {
                Blend SrcAlpha OneMinusSrcAlpha
                CGPROGRAM
                #pragma vertex vert
                #pragma fragment frag

                #include "UnityCG.cginc"

                struct appdata
                {
                    float4 vertex : POSITION;
                    float2 uv : TEXCOORD0;
                };

                struct v2f
                {
                    float2 uv : TEXCOORD0;
                    float4 vertex : SV_POSITION;
                };

                sampler2D _MainTex;
                sampler2D _MaskTex;
                float4 _MainTex_ST;
                float _FilterStrength;
                float _Alpha;


                v2f vert(appdata v)
                {
                    v2f o;
                    o.vertex = UnityObjectToClipPos(v.vertex);
                    o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                    return o;
                }

                fixed4 frag(v2f i) : SV_Target
                {
                    fixed4 col = tex2D(_MainTex, i.uv);

                // Convert color to grayscale
                float gray = dot(col.rgb, float3(0.299, 0.587, 0.114));

                // Mix the original color with the grayscale color based on _FilterStrength
                col.rgb = lerp(float3(gray, gray, gray), col.rgb, _FilterStrength);

                // Apply alpha transparency
                col.a *= _Alpha;

                // Sample the mask texture to determine alpha
                float maskAlpha = tex2D(_MaskTex, i.uv).r;

                // Apply the mask alpha
                col.a *= maskAlpha;

                return col;
            }
            ENDCG
        }
        }
            FallBack "Diffuse"
}

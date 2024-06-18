Shader "Custom/GrayFilterWithAlphaOnClick"
{
    Properties
    {
        _MainTex("Texture", 2D) = "white" {}
        _FilterStrength("Filter Strength", Range(0, 1)) = 1.0
        _Alpha("Alpha", Range(0, 1)) = 1.0
        _MousePos("Mouse Position", Vector) = (0,0,0,0)
        _MouseRadius("Mouse Radius", Float) = 0.1
        _AspectRatio("Aspect Ratio", Float) = 1.0
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
                float4 _MainTex_ST;
                float _FilterStrength;
                float _Alpha;
                float2 _MousePos;
                float _MouseRadius;
                float _AspectRatio;

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

                // Adjust the UV coordinates by the aspect ratio
                float2 adjustedUV = float2(i.uv.x, i.uv.y / _AspectRatio);

                // Adjust the mouse position by the aspect ratio
                float2 adjustedMousePos = float2(_MousePos.x, _MousePos.y / _AspectRatio);

                // Get the distance from the current pixel to the adjusted mouse position
                float dist = distance(adjustedUV, adjustedMousePos);

                // If the distance is within the radius, set alpha to 0
                if (dist < _MouseRadius)
                {
                    col.a = 0;
                }

                return col;
            }
            ENDCG
        }
        }
            FallBack "Diffuse"
}

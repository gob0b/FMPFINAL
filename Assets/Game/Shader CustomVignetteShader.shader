Shader "Custom/VignetteShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _VignetteIntensity ("Intensity", Range(0, 1)) = 0.5
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct v2f
            {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            sampler2D _MainTex;
            float _VignetteIntensity;

            v2f vert (appdata_base v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.texcoord;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float2 uv = i.uv - 0.5;
                float vignette = 1.0 - smoothstep(0.4, 0.6, length(uv) * 2.0);
                fixed4 col = tex2D(_MainTex, i.uv);
                col.rgb *= (1.0 - _VignetteIntensity * vignette);
                return col;
            }
            ENDCG
        }
    }
}



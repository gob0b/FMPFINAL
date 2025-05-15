Shader "Custom/MovingStaticWithCurvature"
{
    Properties
    {
        _MainTex ("Base (RGB)", 2D) = "white" {}
        _NoiseTex ("Noise Texture", 2D) = "black" {}
        _GrainAmount ("Grain Amount", Range(0, 1)) = 0.5
        _CurvatureAmount ("Curvature Amount", Range(0, 1)) = 0.2
        _Speed ("Movement Speed", Range(0, 10)) = 1
        _TimeMultiplier ("Time Multiplier", Range(0, 10)) = 1
        _StaticIntensity ("Static Intensity", Range(0, 1)) = 0.5
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

            uniform float _Speed;
            uniform float _TimeMultiplier;
            uniform float _GrainAmount;
            uniform float _CurvatureAmount;
            uniform float _StaticIntensity;
            uniform float _MainTex_ST;
            sampler2D _MainTex;
            sampler2D _NoiseTex;

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            float generateNoise(float2 uv)
            {
                return tex2D(_NoiseTex, uv).r;
            }

            float2 applyCurvature(float2 uv, float time)
            {
                float curvature = sin(time * _Speed + uv.y * 10.0) * _CurvatureAmount;
                uv.x += curvature;
                uv.y += curvature;
                return uv;
            }

            float generateStatic(float2 uv, float time)
            {
                uv = applyCurvature(uv, time);
                return generateNoise(uv) * _StaticIntensity;
            }

            half4 frag(v2f i) : SV_Target
            {
                float time = _Time.y * _TimeMultiplier;
                float staticEffect = generateStatic(i.uv, time);

                half4 col = tex2D(_MainTex, i.uv);
                half4 noise = half4(staticEffect, staticEffect, staticEffect, 1.0);

                col.rgb += noise.rgb * _GrainAmount;

                return col;
            }
            ENDCG
        }
    }
}

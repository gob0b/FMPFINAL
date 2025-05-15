Shader "Custom/StaticTransition"
{
    Properties
    {
        _MainTex ("Base (RGB)", 2D) = "white" { }
        _Noise ("Noise Texture", 2D) = "white" { }
        _NoiseStrength ("Noise Strength", Range(0, 1)) = 1
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

            sampler2D _MainTex;
            sampler2D _Noise;
            float _NoiseStrength;

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            half4 frag(v2f i) : SV_Target
            {
                float2 uv = i.uv;
                half4 color = tex2D(_MainTex, uv);

                // Add static noise to simulate TV interference
                half4 noise = tex2D(_Noise, uv * 5.0); // Tile the noise texture
                color.rgb += noise.rgb * _NoiseStrength;

                return color;
            }
            ENDCG
        }
    }
}


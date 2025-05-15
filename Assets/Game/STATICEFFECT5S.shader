Shader "Custom/StaticShader"
{
    Properties
    {
        _MainTex ("Base Texture", 2D) = "white" { }
        _NoiseTex ("Noise Texture", 2D) = "white" { }
        _Intensity ("Static Intensity", Range(0, 5)) = 3.0 // Adjust intensity range
        _Speed ("Noise Speed", Range(0, 10)) = 2.0 // Speed of noise movement
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

            sampler2D _MainTex;
            sampler2D _NoiseTex;
            float _Intensity;
            float _Speed;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 pos : POSITION;
                float2 uv : TEXCOORD0;
            };

            v2f vert(appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            half4 frag(v2f i) : SV_Target
            {
                half4 color = tex2D(_MainTex, i.uv);
                half4 noise = tex2D(_NoiseTex, i.uv + _Time.y * _Speed);
                color.rgb += noise.rgb * _Intensity;
                return color;
            }
            ENDCG
        }
    }
}


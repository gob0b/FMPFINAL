Shader "Custom/TVTurnOnHorizontal"
{
    Properties
    {
        _MainTex ("Base (RGB)", 2D) = "white" { }
        _PowerOn ("Power On", Range(0,1)) = 0
        _Noise ("Noise Texture", 2D) = "white" { }
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
            float _PowerOn;
            
            float _NoiseStrength = 0.1; // Adjust static noise strength
            
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

                // Horizontal power-on effect (reveals from center horizontally)
                if (uv.x < 0.5)
                    uv.x = lerp(0.0, uv.x * 2.0, _PowerOn); // Fade in from the middle
                
                // Apply static noise texture
                half4 color = tex2D(_MainTex, uv);
                half4 noise = tex2D(_Noise, uv * 10.0); // Tile the noise texture
                color.rgb += noise.rgb * _PowerOn * _NoiseStrength;

                return color;
            }
            ENDCG
        }
    }
}


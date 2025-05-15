Shader "Custom/VintageTVTurnOnWithCurvature"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Progress ("Progress", Range(0,1)) = 0.0 // Controls the effect (0 = off, 1 = fully on)
        _Curvature ("Screen Curvature", Range(0,1)) = 0.3 // Amount of curvature for CRT effect
        _FlickerStrength ("Flicker Strength", Range(0,1)) = 0.2
        _ScanlineStrength ("Scanline Strength", Range(0,1)) = 0.1
    }
    SubShader
    {
        Tags { "Queue"="Overlay" "RenderType"="Transparent" }
        Pass
        {
            Blend SrcAlpha OneMinusSrcAlpha
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata_t
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
            float _Progress;
            float _Curvature;
            float _FlickerStrength;
            float _ScanlineStrength;

            // Random noise function (for flicker effect)
            float random(float2 uv)
            {
                return frac(sin(dot(uv.xy, float2(12.9898, 78.233))) * 43758.5453);
            }

            v2f vert(appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;

                // Apply curvature effect (radial distortion)
                float2 centeredUV = o.uv - 0.5;
                float curvatureAmount = _Curvature * (dot(centeredUV, centeredUV)); // Radial distortion
                o.uv += centeredUV * curvatureAmount;

                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                // Simulate scanline effect (horizontal lines moving over time)
                float scanline = sin(i.uv.y * 600.0 + _Time.y * 50.0) * _ScanlineStrength;

                // Simulate flicker effect (random brightness variation)
                float flicker = sin(_Time.y * 50.0) * _FlickerStrength * (1.0 - _Progress);

                // Get texture color
                fixed4 col = tex2D(_MainTex, i.uv);

                // Simulate the TV flicker by blending random noise with the texture
                float noise = random(i.uv * _Time.y * 10.0) * _FlickerStrength * (1.0 - _Progress);
                col.rgb += noise;

                // Apply the transparency before full TV turn-on
                float alpha = _Progress;

                // Apply curvature, flickering, and scanlines
                return fixed4(col.rgb + scanline + flicker, alpha);
            }
            ENDCG
        }
    }
}

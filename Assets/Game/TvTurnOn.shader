Shader "Custom/VintageTVTurnOn"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Progress ("Progress", Range(0,1)) = 0 // Controls the effect (0 = off, 1 = fully on)
        _FlickerStrength ("Flicker Strength", Range(0,1)) = 0.2
        _Curvature ("Screen Curvature", Range(0,1)) = 0.2
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
            float _FlickerStrength;
            float _Curvature;

            v2f vert (appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;

                // Apply screen curvature (warping effect)
                float2 centeredUV = o.uv - 0.5;
                float curvatureAmount = _Curvature * (dot(centeredUV, centeredUV)); // Radial distortion
                o.uv += centeredUV * curvatureAmount;

                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // Simulate scanline effect (horizontal lines moving over time)
                float scanline = sin(i.uv.y * 600.0 + _Time.y * 50.0) * 0.05;

                // Simulate flicker effect at start (random brightness variation)
                float flicker = sin(_Time.y * 50.0) * _FlickerStrength * (1.0 - _Progress);

                // Create the TV power-on effect (horizontal strip expanding)
                float center = abs(i.uv.y - 0.5);
                float mask = smoothstep(0.05, 0.2, _Progress - center); 

                // Get TV screen color
                fixed4 col = tex2D(_MainTex, i.uv);

                // Apply mask, scanlines, and flicker
                return col * mask + scanline + flicker;
            }
            ENDCG
        }
    }
}

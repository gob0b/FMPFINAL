Shader "Custom/VintageTVTurnOnWithDelay"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Progress ("Progress", Range(0,1)) = 0
        _FlickerStrength ("Flicker Strength", Range(0,1)) = 0.5
        _Curvature ("Screen Curvature", Range(0,1)) = 0.2
        _NoiseStrength ("Noise Strength", Range(0,1)) = 0.5
        _FlickerDelay ("Flicker Start Delay", Range(0,5)) = 1 // Delay for flicker to start
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
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
            float _FlickerStrength;
            float _Curvature;
            float _NoiseStrength;
            float _FlickerDelay;

            // Random noise function (based on UV coordinates and time)
            float random(float2 uv)
            {
                return frac(sin(dot(uv.xy + _Time.y, float2(12.9898, 78.233))) * 43758.5453);
            }

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
                // Simulate flicker after a delay based on _FlickerDelay and _Progress
                float flickerDelay = max(0.0, _Time.y - _FlickerDelay); // Apply delay before flicker starts
                float flickerEffect = smoothstep(0.0, 1.0, flickerDelay); // Smooth transition for flicker

                // Generate random noise effect for static
                float noise = random(i.uv * _Time.y * 10.0) * _NoiseStrength * (1.0 - _Progress);
                // Simulate scanline effect (horizontal lines moving over time)
                float scanline = sin(i.uv.y * 600.0 + _Time.y * 50.0) * 0.05;

                // Simulate flicker effect (random brightness variation)
                float flicker = sin(_Time.y * 50.0) * _FlickerStrength * flickerEffect * (1.0 - _Progress);

                // Create the TV power-on effect (horizontal strip expanding)
                float center = abs(i.uv.y - 0.5);
                float mask = smoothstep(0.05, 0.2, _Progress - center); 

                // Get TV screen color
                fixed4 col = tex2D(_MainTex, i.uv);

                // Blend noise into the effect before TV fully turns on
                fixed3 finalColor = lerp(fixed3(noise, noise, noise), col.rgb, _Progress);

                // Apply transparency based on progress (see-through before full turn-on)
                float alpha = _Progress;

                // Apply mask, scanlines, flicker, and noise
                return fixed4(finalColor * mask + scanline + flicker, alpha);
            }
            ENDCG
        }
    }
}


Shader "Custom/URP_WaterRipple_Full"
{
    Properties
    {
        _BaseMap ("Texture", 2D) = "white" {}
        _Color ("Color Tint", Color) = (0.3, 0.6, 1, 0.7)
        _WaveSpeed ("Wave Speed", Float) = 1.5
        _WaveScale ("Wave Scale", Float) = 10.0
        _WaveStrength ("Wave Strength", Float) = 0.05
        _AmbientStrength ("Ambient Strength", Float) = 0.3
    }

    SubShader
    {
        Tags 
        { 
            "RenderPipeline"="UniversalRenderPipeline"
            "RenderType"="Transparent"
            "Queue"="Transparent"
        }

        Pass
        {
            Name "ForwardLit"
            Tags { "LightMode"="UniversalForward" }

            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off
            Cull Back

            HLSLPROGRAM

            #pragma vertex vert
            #pragma fragment frag

            // Enable additional lights
            #pragma multi_compile _ _ADDITIONAL_LIGHTS

            // URP Includes
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

            TEXTURE2D(_BaseMap);
            SAMPLER(sampler_BaseMap);

            float4 _Color;
            float _WaveSpeed;
            float _WaveScale;
            float _WaveStrength;
            float _AmbientStrength;

            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS : NORMAL;
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float2 uv : TEXCOORD0;
                float3 normalWS : TEXCOORD1;
                float3 positionWS : TEXCOORD2;
            };

            Varyings vert (Attributes v)
            {
                Varyings o;

                float3 worldPos = TransformObjectToWorld(v.positionOS.xyz);

                // 🌊 Wave animation
                float wave =
                    sin(worldPos.x * _WaveScale + _Time.y * _WaveSpeed) *
                    cos(worldPos.z * _WaveScale + _Time.y * _WaveSpeed);

                worldPos.y += wave * _WaveStrength;

                o.positionWS = worldPos;
                o.positionHCS = TransformWorldToHClip(worldPos);
                o.uv = v.uv;
                o.normalWS = TransformObjectToWorldNormal(v.normalOS);

                return o;
            }

            half4 frag (Varyings i) : SV_Target
            {
                float3 normal = normalize(i.normalWS);

                float4 tex = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, i.uv);
                float3 baseColor = tex.rgb * _Color.rgb;

                // 🌑 Ambient (prevents black)
                float3 finalColor = baseColor * _AmbientStrength;

                // ☀️ Main light (usually directional)
                Light mainLight = GetMainLight();
                float3 mainDir = normalize(mainLight.direction);
                float mainDiffuse = saturate(dot(normal, -mainDir));

                finalColor += baseColor * mainLight.color * mainDiffuse;

                // 💡 Additional lights (THIS includes spotlights)
                uint lightCount = GetAdditionalLightsCount();

                for (uint lightIndex = 0; lightIndex < lightCount; lightIndex++)
                {
                    Light light = GetAdditionalLight(lightIndex, i.positionWS);

                    float3 lightDir = normalize(light.direction);
                    float diff = saturate(dot(normal, -lightDir));

                    finalColor += baseColor * light.color * diff;
                }

                return float4(finalColor, _Color.a);
            }

            ENDHLSL
        }
    }
}
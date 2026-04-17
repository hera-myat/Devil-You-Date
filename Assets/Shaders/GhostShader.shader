Shader "Custom/GhostShader"
{
    Properties
    {
        [MainColor] _BaseColor("Base Color Tint", Color) = (0.7, 0.7, 0.7, 1)
        [MainTexture] _BaseMap("Base Texture", 2D) = "white" {}

        _AmbientStrength("Ambient Strength", Range(0, 1)) = 0.25

        _EmissionColor("Emission Color", Color) = (1, 0.1, 0.1, 1)
        _EmissionIntensity("Emission Intensity", Range(0, 5)) = 1.2
        _PulseSpeed("Pulse Speed", Range(0, 10)) = 2.0
        _PulseMin("Pulse Min", Range(0, 1)) = 0.2

        _RimColor("Rim Color", Color) = (1, 0.2, 0.2, 1)
        _RimPower("Rim Power", Range(0.5, 8)) = 3.0
        _RimIntensity("Rim Intensity", Range(0, 3)) = 0.35
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" "Queue"="Geometry" "RenderPipeline"="UniversalPipeline" }

        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

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
                float3 viewDirWS : TEXCOORD2;
                float3 positionWS : TEXCOORD3;
            };

            TEXTURE2D(_BaseMap);
            SAMPLER(sampler_BaseMap);

            CBUFFER_START(UnityPerMaterial)
                float4 _BaseColor;
                float4 _BaseMap_ST;
                float4 _EmissionColor;
                float4 _RimColor;
                float _AmbientStrength;
                float _EmissionIntensity;
                float _PulseSpeed;
                float _PulseMin;
                float _RimPower;
                float _RimIntensity;
            CBUFFER_END

            Varyings vert(Attributes IN)
            {
                Varyings OUT;

                float3 positionWS = TransformObjectToWorld(IN.positionOS.xyz);

                OUT.positionHCS = TransformWorldToHClip(positionWS);
                OUT.positionWS = positionWS;
                OUT.uv = TRANSFORM_TEX(IN.uv, _BaseMap);
                OUT.normalWS = TransformObjectToWorldNormal(IN.normalOS);
                OUT.viewDirWS = GetWorldSpaceViewDir(positionWS);

                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                float3 normalWS = normalize(IN.normalWS);
                float3 viewDirWS = normalize(IN.viewDirWS);

                half4 baseTex = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, IN.uv);
                half3 baseColor = baseTex.rgb * _BaseColor.rgb;

                Light mainLight = GetMainLight();
                float3 lightDir = normalize(mainLight.direction);

                float diffuse = saturate(dot(normalWS, lightDir));
                float lighting = _AmbientStrength + diffuse * mainLight.distanceAttenuation;

                half3 litColor = baseColor * lighting;

                float pulse = (_PulseMin + (1.0 - _PulseMin) * (0.5 + 0.5 * sin(_Time.y * _PulseSpeed)));
                half3 emission = _EmissionColor.rgb * _EmissionIntensity * pulse;

                float rim = 1.0 - saturate(dot(normalWS, viewDirWS));
                rim = pow(rim, _RimPower) * _RimIntensity;
                half3 rimGlow = _RimColor.rgb * rim;

                half3 finalColor = litColor + emission + rimGlow;

                return half4(finalColor, _BaseColor.a * baseTex.a);
            }
            ENDHLSL
        }
    }
}

Shader "Custom/HauntedTreeLeavesURP"
{
    Properties
    {
        [MainColor] _BaseColor("Base Color Tint", Color) = (0.75, 0.75, 0.75, 1)
        [MainTexture] _BaseMap("Base Texture", 2D) = "white" {}

        _RimColor("Rim Color", Color) = (0.55, 0.7, 0.9, 1)
        _RimPower("Rim Power", Range(0.5, 8)) = 3
        _RimIntensity("Rim Intensity", Range(0, 2)) = 0.35

        _SwayStrength("Sway Strength", Range(0, 0.5)) = 0.03
        _SwaySpeed("Sway Speed", Range(0, 5)) = 1.2
        _HeightStart("Height Start", Float) = 0.3
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" "Queue"="Geometry" "RenderPipeline"="UniversalPipeline" }

        Pass
        {
            Cull Off

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

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
            };

            TEXTURE2D(_BaseMap);
            SAMPLER(sampler_BaseMap);

            CBUFFER_START(UnityPerMaterial)
                float4 _BaseColor;
                float4 _BaseMap_ST;
                float4 _RimColor;
                float _RimPower;
                float _RimIntensity;
                float _SwayStrength;
                float _SwaySpeed;
                float _HeightStart;
            CBUFFER_END

            Varyings vert(Attributes IN)
            {
                Varyings OUT;

                float3 positionOS = IN.positionOS.xyz;
                float heightFactor = saturate(positionOS.y - _HeightStart);
                float3 worldPos = TransformObjectToWorld(positionOS);

                float swayWave = sin(_Time.y * _SwaySpeed + positionOS.y * 0.2);
                float swayAmount = swayWave * _SwayStrength * heightFactor;

                float2 windDir = float2(1.0, 0.5);

                positionOS.x += swayAmount * windDir.x;
                positionOS.z += swayAmount * windDir.y;

                float3 finalWorldPos = TransformObjectToWorld(positionOS);

                OUT.positionHCS = TransformWorldToHClip(finalWorldPos);
                OUT.uv = TRANSFORM_TEX(IN.uv, _BaseMap);
                OUT.normalWS = TransformObjectToWorldNormal(IN.normalOS);
                OUT.viewDirWS = GetWorldSpaceViewDir(finalWorldPos);

                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                float3 normalWS = normalize(IN.normalWS);
                float3 viewDirWS = normalize(IN.viewDirWS);

                half4 baseTex = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, IN.uv);
                half3 baseColor = baseTex.rgb * _BaseColor.rgb;

                float rim = 1.0 - saturate(dot(normalWS, viewDirWS));
                rim = pow(rim, _RimPower) * _RimIntensity;

                half3 finalColor = baseColor + (_RimColor.rgb * rim);

                return half4(finalColor, baseTex.a * _BaseColor.a);
            }
            ENDHLSL
        }
    }
}
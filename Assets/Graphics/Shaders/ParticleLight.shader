Shader "Custom/URP/ParticleLight_AllLights"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Intensity ("Light Intensity", Float) = 1
        _Color ("Silhouette Color", Color) = (1,1,1,1)
    }

    SubShader
    {
        Tags
        {
            "Queue"="Transparent"
            "RenderType"="Transparent"
            "RenderPipeline"="UniversalPipeline"
        }

        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        Cull Off

        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #pragma multi_compile_fragment _ _ADDITIONAL_LIGHTS

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float3 worldPos : TEXCOORD1;
                float2 uv : TEXCOORD0;
            };

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);
            float _Intensity;
            float4 _Color;

            Varyings vert (Attributes v)
            {
                Varyings o;
                o.positionHCS = TransformObjectToHClip(v.positionOS.xyz);
                o.worldPos = TransformObjectToWorld(v.positionOS.xyz);
                o.uv = v.uv;
                return o;
            }

            half4 frag (Varyings i) : SV_Target
            {
                half4 tex = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv);

                float lightFactor = 0;

                #if defined(_ADDITIONAL_LIGHTS)
                uint count = GetAdditionalLightsCount();
                for (uint l = 0; l < count; l++)
                {
                    Light light = GetAdditionalLight(l, i.worldPos);
                    lightFactor = max(lightFactor, light.distanceAttenuation);
                }
                #endif

                float alpha = tex.a * lightFactor * _Intensity;

                clip(alpha - 0.01);

                return half4(
                    _Color.rgb,      // чистый цвет
                    alpha * _Color.a // альфа = маска * свет * интенсивность
                );
            }

            ENDHLSL
        }
    }
}

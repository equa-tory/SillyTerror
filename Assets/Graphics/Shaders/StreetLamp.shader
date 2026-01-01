Shader "Custom/StreetLamp"
{
    Properties
    {
        _ColorTop ("Top Color", Color) = (1,1,1,1)
        _ColorBottom ("Bottom Color", Color) = (0,0,0,1)

        _MinHeight ("Min Height", Float) = 0
        _Height ("Gradient Height", Float) = 1
        _Power ("Gradient Power", Float) = 1

        _EmissionStrength ("Emission Strength", Float) = 1
    }

    SubShader
    {
        Tags { "RenderPipeline"="UniversalRenderPipeline" "RenderType"="Opaque" }

        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS   : NORMAL;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float3 positionWS : TEXCOORD0;
                float3 normalWS   : TEXCOORD1;
            };

            float4 _ColorTop;
            float4 _ColorBottom;
            float _MinHeight;
            float _Height;
            float _Power;
            float _EmissionStrength;

            Varyings vert (Attributes IN)
            {
                Varyings OUT;
                OUT.positionCS = TransformObjectToHClip(IN.positionOS.xyz);
                OUT.positionWS = TransformObjectToWorld(IN.positionOS.xyz);
                OUT.normalWS = TransformObjectToWorldNormal(IN.normalOS);
                return OUT;
            }

            half4 frag (Varyings IN) : SV_Target
            {
                float y = IN.positionWS.y;

                // проверка горизонтальности
                float horizontal = abs(IN.normalWS.y); // 1 если поверхность горизонтальна
                float3 color = lerp(_ColorBottom.rgb, _ColorTop.rgb, pow(saturate((y - _MinHeight) / _Height), _Power));

                // если горизонтальная — ставим верхний цвет
                if (horizontal > 0.95) // порог можно регулировать
                    color = _ColorTop.rgb;

                color *= _EmissionStrength;
                return float4(color, 1.0);
            }
            ENDHLSL
        }
    }
}

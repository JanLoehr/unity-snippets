Shader "Hidden/Custom/OutlineShader"
{
    HLSLINCLUDE

        #include "../StdLib.hlsl"

		float4 _OutlineColor;
        float _OutlineThicknessX;
        float _OutlineThicknessY;
        float _OutlineStrength;

        TEXTURE2D_SAMPLER2D(_MainTex, sampler_MainTex);
        TEXTURE2D_SAMPLER2D(_OutlineTex, sampler_OutlineTex);

        float4 Frag(VaryingsDefault i) : SV_Target
        {
            float4 color = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.texcoord);

            float kernelNormalization = 1.0/69.24;

            float kernel[11] = {
                0.25 * kernelNormalization,
                1.11 * kernelNormalization,
                3.56 * kernelNormalization,
                8.02 * kernelNormalization,
                13.5 * kernelNormalization,
                16.0 * kernelNormalization,
                13.5 * kernelNormalization,
                8.02 * kernelNormalization,
                3.56 * kernelNormalization,
                1.11 * kernelNormalization,
                0.25 * kernelNormalization
            };

            float2 texelOffsetX = float2(1, 0) * _OutlineThicknessX;
            float2 texelOffsetY = float2(0, 1) * _OutlineThicknessY;

            float4 maskBlurSum = float4(0,0,0,0);

            float4 c = SAMPLE_TEXTURE2D(_OutlineTex, sampler_OutlineTex, i.texcoord);
            if (c.r + c.g + c.b <= 0)
            {
                float4 maskColor;

                for (int j = 0; j < 11; j++) 
                {
                    maskColor = SAMPLE_TEXTURE2D(_OutlineTex, sampler_OutlineTex, i.texcoord + texelOffsetX * (j - 5));
                    float luminance = dot(maskColor.rgb, float3(0.2126729, 0.7151522, 0.0721750));
                    maskBlurSum += luminance.xxxx * kernel[j];
                }

                for (int k = 0; k < 11; k++) 
                {
                    maskColor = SAMPLE_TEXTURE2D(_OutlineTex, sampler_OutlineTex, i.texcoord + texelOffsetY * (k - 5));
                    float luminance = dot(maskColor.rgb, float3(0.2126729, 0.7151522, 0.0721750));
                    maskBlurSum += luminance.xxxx * kernel[k];
                }
            }
            
            return color + maskBlurSum * _OutlineColor * _OutlineStrength;
        }

    ENDHLSL

    SubShader
    {
        Cull Off ZWrite Off ZTest Always

        Pass
        {
            HLSLPROGRAM

                #pragma vertex VertDefault
                #pragma fragment Frag

            ENDHLSL
        }
    }
}
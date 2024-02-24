Shader "Hidden/Universal Render Pipeline/PostProcess/Grayscale"
{
    HLSLINCLUDE
    #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
    #include "Packages/com.unity.render-pipelines.core/Runtime/Utilities/Blit.hlsl"

    // Convert linear RGB to linear luminance
    float linearLuminance(float3 linearRGB)
    {
        return dot(linearRGB, float3(0.2126, 0.7152, 0.0722));
    }

    // Convert linear luminance to grayscale
    float3 linearToGrayscale(float linearY)
    {
        return float3(linearY, linearY, linearY);
    }

    // 외부로부터 전달받을 세기 값
    float _Intensity;
    
    half4 Frag(Varyings input) : SV_Target
    {
        // VR 또는 기타 스테레오 렌더링을 위한 코드
        // UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);

        // VR 또는 기타 스테레오 렌더링을 위한 코드
        // float2 uv = UnityStereoTransformScreenSpaceTex(input.texcoord); 

        half4 color = SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_LinearClamp, input.texcoord);

        // 선형 RGB를 선형 휘도 값으로 변환
        float luminance = linearLuminance(color.rgb);

        // 선형 휘도 값을 그레이스케일로 변환
        half3 grayScaleColor = linearToGrayscale(luminance);

        // 그레이스케일로 변환된 색상을 원래 색상과 intensity 값에 따라 섞음
        color.rgb = lerp(color.rgb, grayScaleColor, _Intensity);
        
        return color;
    }
    ENDHLSL

    SubShader
    {
        Tags { "RenderType" = "Opaque" "RenderPipeline" = "UniversalPipeline" }
        LOD 100
        ZTest Always ZWrite Off Cull Off

        Pass
        {
            Name "Custom Post Process"

            HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment Frag
            ENDHLSL
        }
    }
}
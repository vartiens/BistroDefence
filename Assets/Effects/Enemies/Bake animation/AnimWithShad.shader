Shader "Unlit/AnimWithShad"

   {Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _PosTex("Position texture", 2D) = "black" {}
        _NmlTex("Normal texture", 2D) = "white" {}
        _Length("Animation length", float) = 1
        _Scale("Scale", float) = 1
        _DT("Delta time", float) = 0
    }
    SubShader
    {
        Tags { 
            "RenderPipeline" = "UniversalPipeline"
            "RenderType"="Opaque" 
            "Queue"="Geometry+0"
        }
//LOD100

        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

            // GPU Instancing
            #pragma multi_compile_instancing
            // make fog work
            #pragma multi_compile_fog

            // Receive Shadow
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS_CASCADE
            #pragma multi_compile _ _ADDITIONAL_LIGHTS
            #pragma multi_compile _ _ADDITIONAL_LIGHT_SHADOWS
            #pragma multi_compile _ _SHADOWS_SOFT


#define TS _PosTex_TexelSize

struct appdata
{
    float4 vertex : POSITION;
    float2 uv : TEXCOORD0;
    float3 normal : NORMAL;
                UNITY_VERTEX_INPUT_INSTANCE_ID
};

struct v2f
{
    float4 vertex : SV_POSITION;
    float2 uv : TEXCOORD0;
    float fogCoord : TEXCOORD1;
    float3 normal : NORMAL;
    float4 shadowCoord : TEXCOORD2;
    UNITY_VERTEX_INPUT_INSTANCE_ID
                UNITY_VERTEX_OUTPUT_STEREO
            };

            CBUFFER_START(UnityPerMaterial)

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);
half4 _MainTex_ST;
sampler2D _PosTex, _NmlTex;
float4 _PosTex_TexelSize;
float _Length, _DT, _Scale;

CBUFFER_END

            v2f vert (
appdata v, uint vid : SV_VertexID)
            {
//float t = (_Time.y - _DT) / _Length;
float t = _DT / _Length;
                t = fmod(t, 1.0);
float x = (vid + 0.5) * TS.x;
float y = t;
float4 pos = tex2Dlod(_PosTex, float4(x, y, 0, 0));
float3 normal = tex2Dlod(_NmlTex, float4(x, y, 0, 0));
                pos.g-=1;
v2f o;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_TRANSFER_INSTANCE_ID(v, o); 
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o); //VR ¼³Á¤

                //  o.vertex = TransformObjectToHClip(v.vertex.xyz); // Same as MVP
                // Texture coords * scale
                o.vertex = TransformObjectToHClip(pos.xyz * _Scale); // Same as MVP
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.normal = TransformObjectToWorldNormal(normal); // Need extra matrix for Normal transform
                o.fogCoord = ComputeFogFactor(o.vertex.z);

VertexPositionInputs vertexInput = GetVertexPositionInputs(v.vertex.xyz);
                o.shadowCoord = GetShadowCoord(vertexInput);
                return
o;
            }

half4 frag(v2f i) : SV_Target
{
    UNITY_SETUP_INSTANCE_ID(i);
    UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);

    Light mainLight = GetMainLight(i.shadowCoord);

    float4 col = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv);

    float NdotL = saturate(dot(_MainLightPosition.xyz, i.normal)); // Use NdotL for lighting
    half3 ambient = SampleSH(i.normal);

    col.rgb *= NdotL * _MainLightColor.rgb * mainLight.shadowAttenuation * mainLight.distanceAttenuation + ambient;
    col.rgb = MixFog(col.rgb, i.fogCoord);
                
    return col;
}
            ENDHLSL
        }

        Pass
        {
Name"ShadowCaster"

            Tags
{"LightMode" = "ShadowCaster"
}

Cull Back

HLSLPROGRAM

            #pragma prefer_hlslcc gles
            #pragma exclude_renderers d3d11_9x
            #pragma target 2.0

            #pragma vertex ShadowPassVertex
            #pragma fragment ShadowPassFragment
            
            #pragma shader_feature _ALPHATEST_ON

            // GPU Instancing
            #pragma multi_compile_instancing
        
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
#include "Packages/com.unity.render-pipelines.universal/Shaders/LitInput.hlsl"
#include "Packages/com.unity.render-pipelines.universal/Shaders/ShadowCasterPass.hlsl"


            CBUFFER_START(UnityPerMaterial)

half4 _TintColor;
sampler2D _MainTex, _PosTex, _NmlTex;
float4 _MainTex_ST;
float _Alpha;
float4 _PosTex_TexelSize;
float _Length, _DT, _Scale;

#define TS _PosTex_TexelSize
            CBUFFER_END

struct VertexInput
{
    float4 vertex : POSITION;
    float4 normal : NORMAL;
            
            #if _ALPHATEST_ON
            float2 uv     : TEXCOORD0;
            #endif

            UNITY_VERTEX_INPUT_INSTANCE_ID  
};
        
struct VertexOutput
{
    float4 vertex : SV_POSITION;
#if _ALPHATEST_ON
            float2 uv     : TEXCOORD0;
#endif
    UNITY_VERTEX_INPUT_INSTANCE_ID
            UNITY_VERTEX_OUTPUT_STEREO

            };

    VertexOutput ShadowPassVertex(VertexInput v, uint vid : SV_VertexID)
    {
        float t = _DT / _Length;
        
        t = fmod(t, 1.0);
        float x = (vid + 0.5) * TS.x;
        float y = t;
        float4 pos = tex2Dlod(_PosTex, float4(x, y, 0, 0));
        float3 normal = tex2Dlod(_NmlTex, float4(x, y, 0, 0));
        
        VertexOutput o;
        UNITY_SETUP_INSTANCE_ID(v);
        UNITY_TRANSFER_INSTANCE_ID(v, o);
        UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
        
        float3 positionWS = TransformObjectToWorld(pos.xyz * 0);
        float3 normalWS = TransformObjectToWorldNormal(normal);
        
        float4 positionCS = TransformWorldToHClip(ApplyShadowBias(positionWS, normalWS, _MainLightPosition.xyz));
            
        o.vertex = positionCS;
#if _ALPHATEST_ON
            o.uv = v.uv * _MainTex_ST.xy + _MainTex_ST.zw; ;
#endif

        return o;
    }

    half4 ShadowPassFragment(VertexOutput i) : SV_TARGET
    {
        UNITY_SETUP_INSTANCE_ID(i);
        UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);
            
#if _ALPHATEST_ON
                float4 col = tex2D(_MainTex, i.uv);
                clip(col.a - _Alpha);
#endif

        return 0;
    }

            ENDHLSL
}
    }
}

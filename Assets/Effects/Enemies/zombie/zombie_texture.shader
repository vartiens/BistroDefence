Shader "Custom/zombie_texture"
{
    Properties
    {
       _MainTex("Texture", 2D) = "white" {}
       _PosTex("Position texture", 2D) = "black" {}
       _NmlTex("Normal texture", 2D) = "white" {}
       _Length("Animation length", float) = 1
       _DT("Delta time", float) = 0
    }
    SubShader{
       Pass{
           CGPROGRAM
           #pragma vertex vert
           #pragma fragment frag

#include "UnityCG.cginc"

#define TS _PosTex_TexelSize
         
struct appdata
{
    float2 texcoord : TEXCOORD0;
    
     UNITY_VERTEX_INPUT_INSTANCE_ID
};
    
struct v2f
{
    float2 uv : TEXCOORD0;
    float3 normal : TEXCOORD1;
    float4 vertex : SV_Position;
    
     UNITY_VERTEX_OUTPUT_STEREO
};

sampler2D _MainTex, _PosTex, _NmlTex;
float4 _PosTex_TexelSize;
float _Length, _DT;

v2f vert(appdata v, uint vid : SV_VertexID)
{
    float t = (_Time.y - _DT) / _Length;
    t = fmod(t, 1.0);
    float x = (vid + 0.5) * TS.x;
    float y = t;
    float4 pos = tex2Dlod(_PosTex, float4(x, y, 0, 0));
    float3 normal = tex2Dlod(_NmlTex, float4(x, y, 0, 0));
    pos.g = pos.g - 0.8;
    v2f o;
    
    UNITY_SETUP_INSTANCE_ID(v);
    UNITY_INITIALIZE_OUTPUT(v2f, o);
    UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
    
    o.vertex = UnityObjectToClipPos(pos);
    o.normal = UnityObjectToWorldNormal(normal);
    o.uv = v.texcoord;
    
    return o;
}
    
half4 frag(v2f i) : SV_Target
{
    UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);
    
    half diff = dot(i.normal, float3(0, 1, 0)) * 0.5 + 0.5;
    half4 col = tex2D(_MainTex, i.uv);
    return diff * col;
}


           ENDCG
       }
   }
}

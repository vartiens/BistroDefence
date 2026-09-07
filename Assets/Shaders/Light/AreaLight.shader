/*
사용시 주의사항
Base Texture 사용 시에는 Tiling을 X=1, Y=1로 설정해야 한다.
대체 왜 material 따라 되다안되다 하는지는 모른다.
*/

Shader"Custom/AreaLight" {
 
    Properties{
        _MainTex("Base (RGB)", 2D) = "white" {}
        _BaseColor("Base Color", Color) = (1,1,1,1)
        _UseTextures("Use Base Textures?", Int) = 0
        _DiscardColor("Background Color to Discard", Color) = (1,1,1,1)
 
        _EmissiveMap("Emissive Map", 2D) = "white" {}
        _EmissionColor("Glow Color", Color) = (1,1,1,1)
        _EmissionIntensity("Emission Intensity", Float) = 1.0
    }
 
    SubShader{
        Lighting On
 
        Material
        {
            Emission[_EmissionColor]
        }
 
        Pass{
            SetTexture[_MainTex]{ combine texture }
            SetTexture[_EmissiveMap]{ combine primary lerp(texture) previous }
        }
         
        //Tags {"Queue" = "Transparent" "RenderType" = "Transparent"}
        LOD 200
        Cull Back
        Blend SrcAlpha OneMinusSrcAlpha
       
        CGPROGRAM
        // addshadow used to add shadow collector and caster passes following vertex modification
        //#pragma surface surf Lambert alpha:blend addshadow
        #pragma surface surf Lambert addshadow novertexlights
 
        // Access the shaderlab properties
        sampler2D _MainTex;
        sampler2D _EmissiveMap;
        int _UseTextures;
        fixed4 _BaseColor;
        fixed4 _DiscardColor;
        fixed4 _EmissionColor;
        float _EmissionIntensity;
 
        // Basic input structure to the shader function
        // requires only a single set of UV texture mapping coordinates
        struct Input
        {
            float2 uv_MainTex;
            float2 uv_EmissiveMap;
        };
 
        // This is where the curvature is applied
        /*void vert(inout appdata_full v)
        {
                    // Transform the vertex coordinates from model space into world space
            // MC -> WC 좌표계변환
            float4 vv = mul(unity_ObjectToWorld, v.vertex);
 
                    // Now adjust the coordinates to be relative to the camera position
            vv.xyz -= _WorldSpaceCameraPos.xyz;
 
                    // Reduce the y coordinate (i.e. lower the "height") of each vertex based
                    // on the square of the distance from the camera in the z axis, multiplied
                    // by the chosen curvature factor
            // 카메라에서 멀수록 y좌표 감소
            vv = float4(0.0f, (vv.z * vv.z) * -_Curvature, 0.0f, 0.0f);
 
                    // Now apply the offset back to the vertices in model space
            v.vertex += mul(unity_WorldToObject, vv);
        }*/

        void surf(Input IN, inout SurfaceOutput o)
        {
            half4 c = tex2D(_MainTex, IN.uv_MainTex);
            half4 e = tex2D(_EmissiveMap, IN.uv_EmissiveMap);
    
            // Base texture map의 검은색 점을 discard = 네온사인의 글자 부분만 렌더링
            // 어느 색깔을 discard할지 지정할 수 있다.
            if (_UseTextures == 1)
            {
                half4 t = c - _DiscardColor;
                float blackTest = t.r + t.g + t.b;
                if (blackTest < 0) blackTest *= -1;
                clip(blackTest - 0.01); // 오직 비슷한 색깔만 삭제 (더 어두운 색깔들도 남김)
            }
    
            c *= _BaseColor;
            e *= _EmissionColor;
            
            o.Albedo = c.rgb + e.rgb;
            o.Emission = e.rgb * _EmissionIntensity;
            o.Specular = 1.0f;
            o.Gloss = 1.0f;
            o.Alpha = c.a;
        }

        ENDCG
    }
}

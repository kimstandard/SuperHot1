Shader "Custom/SpecularMask"
{
    Properties
    {
        _Color ("Specular Color", Color) = (1,1,1,1)
        _Threshold ("Specular Threshold", Range(0,1)) = 0.5
        _Smoothness ("Smoothness", Range(0.01,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 1.0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            Tags {"LightMode" = "ForwardBase"}
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float3 worldNormal : TEXCOORD0;
                float3 worldPos : TEXCOORD1;
            };

            fixed4 _Color;
            float _Threshold;
            float _Smoothness;
            float _Metallic;

            v2f vert (appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.worldNormal = UnityObjectToWorldNormal(v.normal);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // Lighting direction (assumes main directional light)
                float3 lightDir = normalize(_WorldSpaceLightPos0.xyz);

                // View direction
                float3 viewDir = normalize(_WorldSpaceCameraPos - i.worldPos);

                // Reflection direction
                float3 reflectDir = reflect(-lightDir, normalize(i.worldNormal));

                // Specular calculation (Phong-style)
                float spec = max(0, dot(viewDir, reflectDir));
                spec = pow(spec, _Smoothness * 256);

                // Metallic influence
                spec *= _Metallic;

                // Threshold cutoff ¡æ pure black/white mask
                float mask = step(_Threshold, spec);

                return _Color * mask;
            }
            ENDCG
        }
    }
}


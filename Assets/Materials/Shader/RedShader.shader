Shader "Custom/RedMetallicStandardLike"
{
    Properties
    {
        _Color ("Albedo", Color) = (1,0,0,1)
        _Metallic ("Metallic", Range(0,1)) = 1
        _Smoothness ("Smoothness", Range(0,1)) = 0.5
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
        #pragma surface surf Standard fullforwardshadows

        fixed4 _Color;
        half _Metallic;
        half _Smoothness;

        struct Input
        {
            float dummy; // 최소 1개 필드 필요
        };

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            o.Albedo = _Color.rgb;
            o.Metallic = _Metallic;
            o.Smoothness = _Smoothness;
            o.Alpha = _Color.a;
        }
        ENDCG
    }
    FallBack "Standard"
}

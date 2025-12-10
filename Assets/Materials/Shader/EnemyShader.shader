Shader "Custom/EnemyRedToOrangeCel"
{
    Properties
    {
        _BaseColor ("Base Color", Color) = (1, 0, 0, 1)           // 일반 빨강
        _ShadowColor ("Shadow Color", Color) = (0.3, 0, 0, 1)     // 어두운 빨강
        _HighlightColor ("Highlight Color", Color) = (1, 0.5, 0, 1) // 주황색
        _Metallic ("Metallic", Range(0,1)) = 0.0
        _Smoothness ("Smoothness", Range(0,1)) = 0.5
        _Steps ("Cel Shading Steps", Range(1,5)) = 3
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 300

        CGPROGRAM
        #pragma surface surf Standard fullforwardshadows

        fixed4 _BaseColor;
        fixed4 _ShadowColor;
        fixed4 _HighlightColor;
        half _Metallic;
        half _Smoothness;
        int _Steps;

        struct Input
        {
            float3 worldNormal;
            INTERNAL_DATA
        };

       void surf (Input IN, inout SurfaceOutputStandard o)
{
    fixed3 lightDir = normalize(_WorldSpaceLightPos0.xyz);
    float NdotL = max(0, dot(normalize(IN.worldNormal), lightDir));

    // 셀 셰이딩 단계 계산
    float step = floor(NdotL * _Steps) / (_Steps - 1);
    step = saturate(step);

    fixed3 color;
    if (_Steps <= 2)
    {
        color = (step < 0.5) ? _ShadowColor.rgb : _HighlightColor.rgb;
    }
    else
    {
        if (step < 0.33)
            color = _ShadowColor.rgb;
        else if (step < 0.66)
            color = _BaseColor.rgb;
        else
            color = _HighlightColor.rgb;
    }

    // Metal workflow correction
    // Metal = 1이면 거울 반사 → 알베도에서 스페큘러용 컬러로 보정
    o.Albedo = lerp(color, fixed3(0.04,0.04,0.04), _Metallic);
    o.Metallic = _Metallic;
    o.Smoothness = _Smoothness;
    o.Alpha = _BaseColor.a;
}

        ENDCG
    }
    FallBack "Diffuse"
}


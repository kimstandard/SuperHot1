Shader "Custom/EnemyCelRedOrange"
{
    Properties
    {
        _BaseColor ("Base Color", Color) = (1, 0, 0, 1)
        _ShadowColor ("Shadow Color", Color) = (0.3, 0, 0, 1)
        _HighlightColor ("Highlight Color", Color) = (1, 0.6, 0, 1)
        _HighlightIntensity ("Highlight Intensity", Range(1,10)) = 4
        _HighlightThreshold ("Highlight Threshold", Range(0,1)) = 0.66
        _Metallic ("Metallic", Range(0,1)) = 0.0
        _Smoothness ("Smoothness", Range(0,1)) = 0.5
        _Steps ("Cel Steps", Range(2,5)) = 3
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
        float _HighlightIntensity;
        float _HighlightThreshold;

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
            float3 normalWorld = normalize(IN.worldNormal);
            float3 lightDir = normalize(_WorldSpaceLightPos0.xyz);
            float NdotL = max(0, dot(normalWorld, lightDir));

            float stepped = floor(NdotL * _Steps) / (_Steps - 1);
            stepped = saturate(stepped);

            fixed3 finalColor;

            if (_Steps <= 2)
            {
                finalColor = (stepped < 0.5) ? _ShadowColor.rgb : (_HighlightColor.rgb * _HighlightIntensity);
            }
            else
            {
                if (stepped < 0.33)
                    finalColor = _ShadowColor.rgb;
                else if (stepped < _HighlightThreshold)
                    finalColor = _BaseColor.rgb;
                else
                    finalColor = _HighlightColor.rgb * _HighlightIntensity;
            }

            o.Albedo = finalColor;
            o.Metallic = _Metallic;
            o.Smoothness = _Smoothness;
            o.Alpha = _BaseColor.a;
        }
        ENDCG
    }
    FallBack "Diffuse"
}

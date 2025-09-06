Shader "UI/GlowOutline"
{
    Properties
    {
        _MainTex ("Sprite", 2D) = "white" {}
        _OutlineColor ("Outline Color", Color) = (1,1,1,1)
        _OutlineThickness ("Outline Thickness", Range(0,10)) = 2
        _GlowIntensity ("Glow Intensity", Range(0,5)) = 1
    }

    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        LOD 200
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4 _MainTex_ST;
            fixed4 _OutlineColor;
            float _OutlineThickness;
            float _GlowIntensity;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);

                // Simple outline by sampling neighbor UVs
                float outline = 0;
                float2 offset = _OutlineThickness / _ScreenParams.xy;
                outline += tex2D(_MainTex, i.uv + float2(offset.x, 0)).a;
                outline += tex2D(_MainTex, i.uv - float2(offset.x, 0)).a;
                outline += tex2D(_MainTex, i.uv + float2(0, offset.y)).a;
                outline += tex2D(_MainTex, i.uv - float2(0, offset.y)).a;

                if (col.a < 0.1 && outline > 0)
                {
                    return _OutlineColor * _GlowIntensity;
                }

                return col;
            }
            ENDCG
        }
    }
}

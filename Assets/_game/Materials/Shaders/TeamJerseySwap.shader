Shader "UI/TeamJerseySwap"
{
    Properties
    {
        _MainTex ("Base Texture", 2D) = "white" {}
        _Color1 ("Primary Color", Color) = (1,1,1,1)
        _Color2 ("Secondary Color", Color) = (1,0,0,1)
        _Color3 ("Accent Color", Color) = (0,0,1,1)
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
            fixed4 _Color1, _Color2, _Color3;

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

                // Assume jersey mask encoding:
                // Red channel → Primary
                // Green channel → Secondary
                // Blue channel → Accent

                float primary = col.r;
                float secondary = col.g;
                float accent = col.b;

                fixed4 finalCol = primary * _Color1 + secondary * _Color2 + accent * _Color3;
                finalCol.a = col.a; // keep alpha

                return finalCol;
            }
            ENDCG
        }
    }
}

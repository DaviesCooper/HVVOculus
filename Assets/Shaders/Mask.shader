Shader "Unlit/Mask"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Radius ("Radius", Range(0,1)) = 0
        [Toggle] _Invert ("Invert", Float) = 0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float _Radius;
            float _Invert;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float2 pos = (i.uv * 2) - 1;
                float inside = 1 - step(_Radius, length(pos));

                //
                float4 inCol = (_Invert == 0) * float4(0, 0, 0, 1) + (_Invert ==1) * float4(0,0,0,0);
                float4 outCol = (_Invert == 0) * float4(0,0,0,0) + (_Invert == 1) * float4(0, 0, 0, 1);

                return (inside * inCol) + (!inside * outCol);
            }
            ENDCG
        }
    }
}

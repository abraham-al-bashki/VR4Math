Shader "Custom/Sector"
{
    Properties
    {
        _Color("Color", Color) = (1,1,1,1)
        _Angle("Angle", Range(0,360)) = 120.0
    }
        SubShader
    {
        Tags { "RenderType" = "Transparent" "Queue" = "Transparent" "PreviewType" = "Plane" }

        Pass
        {
            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off
            Cull Off

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            half _Angle;
            half4 _Color;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            v2f vert(appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            half4 frag(v2f i) : SV_Target
            {
                //Re-map t$$anonymous$$s now rather than later
                float2 pos = -(i.uv * 2.0 - 1.0);

                //Calculate the angle of our current pixel around the circle
                float theta = degrees(atan2(pos.x, pos.y)) + 180.0;

                //Get circle and sector masks
                float circle = length(pos) <= 1.0;
                float sector = theta <= _Angle;

                //Return the desired colour masked by the circle and sector
                return _Color * (circle * sector);
            }
            ENDCG
        }
    }
}

Shader "Unlit/Vertex"
{

    SubShader
    {

            ZWrite On
            ZTest Less
            Cull Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
      
            //StructuredBuffer<int> _VertexBuffer;

            struct v2f
            {
                float4 pos : SV_POSITION;
                float size : PSIZE;
            };

    

            struct structurePS
            {
                float4 target0 : SV_Target0;
                float4 target1 : SV_Target1;
            };

            v2f vert(uint v : SV_VertexID)

            {
                v2f o;
                o.pos.x = 0;
                o.pos.y = 0;
                o.pos.z = 0.01;
                o.pos.w = 1;
                o.size = 10;
                return o;
            }
            /*
            fixed4 frag(v2f i) : SV_Target
            {
                return float4(1,1,1,1);

            }
                        */
            structurePS frag(v2f i)
            {
                structurePS ret;
                ret.target0 = float4(1, 1, 1, 1);
                ret.target1 = i.pos;
                return ret;
            }

            ENDCG
        }
    }
}

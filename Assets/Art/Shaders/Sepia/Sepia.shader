Shader "Custom/Sepiatone" {
    Properties {
        _MainTex ("Texture", 2D) = "white" { }
    }


    SubShader {
        Pass {
                    
        CGPROGRAM
        #pragma vertex vert_img
        #pragma fragment frag
        #include "UnityCG.cginc"

        sampler2D _MainTex;
        struct v2f {
            float4  pos : SV_POSITION;
            float2  uv : TEXCOORD0;
        };

        half4 _MainTex_ST;
        v2f vert (appdata_base v)
        {
            v2f o;
            o.pos = UnityObjectToClipPos (v.vertex);
            o.uv = TRANSFORM_TEX (v.texcoord, _MainTex);
            return o;
        }

        half4 frag (v2f i) : COLOR
        {
            half4 texcol = tex2D (_MainTex, i.uv);
            fixed Y = dot (fixed3(0.299, 0.587, 0.114), texcol.rgb);
            fixed4 sepiaConvert = float4 (0.191, -0.054, -0.221, 0.0);
            fixed4 output = sepiaConvert + Y;
            //texcol.rgb = dot(texcol.rgb, float3(0.3, 0.59, 0.11));
            return output;
        }

        //fixed4 frag (v2f_img i) : SV_Target
      //  {	
       //     fixed4 original = tex2D(_MainTex, UnityStereoScreenSpaceUVAdjust(i.uv, _MainTex_ST));
            
            // get intensity value (Y part of YIQ color space)
      //      fixed Y = dot (fixed3(0.299, 0.587, 0.114), original.rgb);

            // Convert to Sepia Tone by adding constant 	fixed4 sepiaConvert = float4 (0.191, -0.054, -0.221, 0.0);

      //      fixed4 sepiaConvert = float4 (0.191, -0.054, -0.221, 0.0);
      //      fixed4 output = sepiaConvert + Y;
     //       output.a = original.a;
      //      
     //       return output;
     //   }
        ENDCG

    }
}

Fallback "VertexLit"

}
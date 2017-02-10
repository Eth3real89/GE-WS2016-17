// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "Enviro/VolumeClouds_ShadowCast" {
    Properties {
        _CloudsMap ("Cloud Map", 2D) = "white" {}
         [HideInInspector]_Scale ("Scaling", Float ) = 1500
        _CloudCover ("CloudCover", Float ) = -0.25
        _CloudAlpha ("CloudAlpha", Float ) = 5
        _AlphaCut ("CloudAlphaCut", Float ) = 0.01
        _Speed1Layer ("Speed1Layer", Float ) = 0.1
        _Speed2Layer ("Speed2Layer", Float ) = 4
        _WindDir ("Wind Direction", Vector) = (1,0,0,0)
      	[HideInInspector]_Offset ("Offset", Float ) = 1

    }
    SubShader {
    	Tags 
    	{ 
    		"RenderType"="Opaque"
    	}
        LOD 200
		Pass {
            Name "ShadowCaster"
			Tags 
			{ 
				"LightMode" = "ShadowCaster" 
			}

			Cull Front
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            
            #define UNITY_PASS_SHADOWCASTER
            
            #include "UnityCG.cginc"
            #include "Lighting.cginc"
            
            #pragma fragmentoption ARB_precision_hint_fastest
            #pragma multi_compile_shadowcaster
            #pragma multi_compile_fog

            #pragma target 3.0
            #pragma glsl
            #pragma exclude_renderers gles


            float4 _TimeEditor;
            fixed _Scale;
            fixed _CloudCover;
            fixed _CloudAlpha;
            fixed _AlphaCut;
            fixed _Offset;
            fixed _Speed1Layer;
            fixed _Speed2Layer;
            fixed4 _WindDir;
            sampler2D _CloudsMap;
			
            struct VertexInput {
                float4 vertex   : POSITION;
                float3 normal	: NORMAL;
				float4 vertexColor : COLOR;
            };
            struct VertexOutput {
            	float4 Pos : POSITION1;
                float4 posWorld : TEXCOORD1;
                float4 vertexColor : COLOR;
                V2F_SHADOW_CASTER;
            };
			
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.vertexColor = v.vertexColor;
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);

                TRANSFER_SHADOW_CASTER(o)
                return o;
            }
            fixed4 frag(VertexOutput i) : Color {
                
                fixed4 objPos = mul(unity_ObjectToWorld, float4(0,0,0,1));
                fixed2 worldUV = (i.posWorld.xz-objPos.xz * 0) / _Scale;
                
                fixed2 baseAnimation = (_Time.g + _TimeEditor.g) * 0.001 * (_WindDir.rb * _Offset);
                fixed2 newUV = worldUV + (baseAnimation * _Speed1Layer) + fixed2(_Offset,_Offset);
				fixed2 newUV2 = worldUV + (baseAnimation * _Speed2Layer) + fixed2(0.0, 0.5) + fixed2(_Offset,_Offset);
            	fixed4 cloudTexture = tex2D(_CloudsMap, newUV);
            	fixed4 cloudTexture2 = tex2D(_CloudsMap, newUV2);
            	fixed baseMorph = ((saturate(cloudTexture.a + _CloudCover) * i.vertexColor.a) - cloudTexture2.a);
            	fixed cloudMorph = baseMorph * _CloudAlpha;
            	fixed cloudAlphaCut = cloudMorph -_AlphaCut;

            	clip(saturate(ceil(cloudAlphaCut)) - 0.5);

                SHADOW_CASTER_FRAGMENT(i)
            }
            ENDCG
        }
    }
}

// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'

Shader "Enviro/VolumeClouds" {
    Properties {
        _BaseColor ("BaseColor", Color) = (1,1,1,0.5)
        _ShadingColor ("ShadingColor", Color) = (0, 0, 0, 0.5)
 
        _Density ("Density", Float ) = 0.5
        
        _CloudsMap ("Clouds Map", 2D) = "white" {}
         [HideInInspector]_Scale ("Scaling", Float ) = 4000

        _CloudCover ("CloudCover", Float ) = -0.25
        _CloudAlpha ("CloudAlpha", Float ) = 5
        _CloudAlphaCut ("CloudAlphaCut", Float ) = 0.01
        
        _Speed1Layer ("Speed1Layer", Float ) = 0.1
        _Speed2Layer ("Speed2Layer", Float ) = 4
        _WindDir ("Wind Direction", Vector) = (1,0,0,0)

        _HorizonBlend ("Horizon Blending", Range (0, 10)) = 2

        //_Exposure ("Exposure Intensity", Float ) = 1

        _ambient ("ambient light intensity", Float ) = 1
        _direct ("direct light intensity", Float ) = 1
        _nightColorMod ("night light intensity", Float ) = 1


        [HideInInspector]_Offset ("Offset", Float ) = 1
        [HideInInspector]_CloudNormalsDirection ("_CloudNormalsDirection", Vector) = (1, -1, -1, 0)
    }
    SubShader {
        Tags {
        	"IgnoreProjector"="True"
        	"Queue" = "Transparent"
        	"RenderType"="Transparent"
        }
        LOD 200
        Pass {
            Name "ForwardBase"
            Tags {
                "LightMode"="ForwardBase"
            }
            Blend SrcAlpha OneMinusSrcAlpha

            ZWrite On
          	ZTest On

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            
            #define UNITY_PASS_FORWARDBASE
            
            #include "UnityCG.cginc"
            #pragma multi_compile_fwdbase
            #pragma target 3.0
            #pragma glsl

            #include "AutoLight.cginc" 
            #include "Lighting.cginc"

            uniform fixed4 _TimeEditor;
            uniform fixed _Speed1Layer;
            uniform fixed _Speed2Layer;
            uniform fixed _Scale;
            uniform fixed _Offset;
            uniform fixed _CloudCover;
            uniform fixed4 _BaseColor;

            uniform fixed _CloudAlpha;
            uniform fixed _CloudAlphaCut;
            uniform fixed _Density;
            uniform sampler2D _CloudsMap;
            uniform float4 _CloudsMap_ST;
            uniform fixed4 _WindDir;
			uniform fixed _HorizonBlend;
            uniform fixed4 _ShadingColor;
            uniform fixed4 _CloudNormalsDirection;
           // uniform float _Exposure;
            uniform float _ambient;
            uniform float _direct;
            uniform float _nightColorMod;

            struct VertexInput {
                half4 vertex : POSITION;
                half4 vertexColor : COLOR;
                half3 normal : NORMAL;
            	float2 texcoord0 : TEXCOORD0;
         
            };

            struct VertexOutput {
                half4 pos : SV_POSITION;
                half4 posWorld : TEXCOORD0;
                half3 normalDir : TEXCOORD1;
                half4 vertexColor : COLOR;
            	float2 uv0 : TEXCOORD5;
            };

           	VertexOutput vert (VertexInput v) {
                VertexOutput o;
                UNITY_INITIALIZE_OUTPUT(VertexOutput, o);
                o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                o.normalDir = mul(half4(v.normal,0), unity_WorldToObject).xyz;
                o.vertexColor = v.vertexColor;
                o.uv0 = v.texcoord0;
                return o;
            }
            
            
            
            fixed4 frag(VertexOutput i) : COLOR {
    
                fixed2 baseAnimation = (_Time.g + _TimeEditor.g) * 0.001 * (_WindDir.rb * _Offset) + _CloudsMap_ST.ba;
                fixed2 wUV = lerp(i.posWorld.xz / _Scale * _CloudsMap_ST.rg, i.uv0/(_CloudsMap_ST.rg * _Scale*0.0005), 0.0);
                
                fixed2 nUV = wUV + (baseAnimation * _Speed1Layer) + fixed2(_Offset,_Offset);
                fixed2 nUV2 = wUV + (baseAnimation * _Speed2Layer) + fixed2(0.0, 0.5) + fixed2(_Offset,_Offset);
                fixed4 cloudTexture = tex2D(_CloudsMap, nUV);
                fixed4 cloudTexture2 = tex2D(_CloudsMap, nUV2);

                fixed baseMorph = ((saturate(cloudTexture.a + _CloudCover) * i.vertexColor.a) - cloudTexture2.a);
                fixed3 baseMorphNormals = ((cloudTexture.rgb* 2 - 1 + _CloudCover) * i.vertexColor.a) - (cloudTexture2.rgb*2-1);

				fixed cloudMorph = baseMorph * _CloudAlpha;
        		cloudMorph *= pow(saturate(1-length(i.uv0 * 2.0+-1.0)), _HorizonBlend);
                fixed cloudAlphaCut = cloudMorph -_CloudAlphaCut;
                 
                clip(saturate(ceil(cloudAlphaCut)) - 0.5);
                fixed fakeDepth = saturate(-_Density + (i.vertexColor.b * _CloudNormalsDirection.g + 1)/2);

                cloudMorph = saturate(cloudMorph);

			    fixed3 lColor = (_BaseColor.rgb * pow(_LightColor0 * _nightColorMod, _direct)) * (fakeDepth * 2);
			    fixed3 finalColor = lerp(_ShadingColor.rgb, lColor, fakeDepth);

			   finalColor *= pow(ShadeSH9(half4(i.normalDir, 1.0)),_ambient);
			 //  finalColor = finalColor * _nightColorMod;
			    //ToneMapping
			    //finalColor = saturate( 1.0 - exp(-_Exposure * finalColor));

               return fixed4(finalColor, cloudMorph);
            }
            ENDCG
		}

    }
    FallBack "Diffuse"
}

Shader "Enviro/Particles/Lit Alpha-Blended " {
Properties {
   _TintColor ("Tint Color", Color) = (0.5,0.5,0.5,0.5)
   _MainTex ("Particle Texture", 2D) = "white" {}
   _InvFade ("Soft Particles Factor", Range(0.01,3.0)) = 1.0
}
 
Category {
   Tags { "Queue"="Transparent+699" "IgnoreProjector"="True" "RenderType"="Transparent"  }
   Blend SrcAlpha OneMinusSrcAlpha
   ColorMask RGB
   Cull Off 
   Lighting On 
   ZWrite Off
 
   
   // ---- Fragment program cards
   SubShader {
     Pass {
 
       CGPROGRAM
       #pragma vertex vert
       #pragma fragment frag
       #pragma fragmentoption ARB_precision_hint_fastest
       #pragma multi_compile_particles
       
       #include "UnityCG.cginc"
 
       sampler2D _MainTex;
       float4 _TintColor;
 
       struct appdata_t {
         float4 vertex : POSITION;
         fixed4 color : COLOR;
         float2 texcoord : TEXCOORD0;
         float3 normal : NORMAL;
       };
 
       struct v2f {
         float4 pos : POSITION;
         fixed4 color : COLOR;
         float2 texcoord : TEXCOORD0;
         float2 DisCoord : TEXCOORD1;
 
         float4 projPos : TEXCOORD2;
 
         float3 VL : TEXCOORD3;
 
         float3 normal :TEXCOORD4;
         float3 ViewT : TEXCOORD5;
 
       };
       
       float4 _MainTex_ST;
       float4 _Dist_ST;
 
       v2f vert (appdata_t v)
       {
         v2f o;
         o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
 
         o.projPos = ComputeScreenPos (o.pos);
         COMPUTE_EYEDEPTH(o.projPos.z);
 
         o.color = v.color;
         o.texcoord = TRANSFORM_TEX(v.texcoord,_MainTex);
         o.DisCoord = TRANSFORM_TEX(v.texcoord,_Dist);
         o.normal = normalize(v.normal);
         o.ViewT = normalize(ObjSpaceViewDir(v.vertex));
         o.VL = ShadeVertexLights(v.vertex, dot(o.normal,o.ViewT));
         return o;
       }
 
       sampler2D _CameraDepthTexture;
       float _InvFade;
       
       fixed4 frag (v2f i) : COLOR
       {
 
         float4 tex = tex2D(_MainTex, i.texcoord);
 
         float sceneZ = LinearEyeDepth (UNITY_SAMPLE_DEPTH(tex2Dproj(_CameraDepthTexture, UNITY_PROJ_COORD(i.projPos))));
         float partZ = i.projPos.z;
         float fade = saturate (_InvFade * (sceneZ-partZ));
         i.color.a *= fade;
 
         float4 VertL = (dot(i.ViewT,i.VL));
         return 2.0f * float4(i.VL,0) * i.color * _TintColor * tex;
       }
       ENDCG
     }
   }    
   
   // ---- Dual texture cards
   SubShader {
     Pass {
       SetTexture [_MainTex] {
         constantColor [_TintColor]
         combine constant * primary
       }
       SetTexture [_MainTex] {
         combine texture * previous DOUBLE
       }
     }
   }
   
   // ---- Single texture cards (does not do color tint)
   SubShader {
     Pass {
       SetTexture [_MainTex] {
         combine texture * primary
       }
     }
   }
}
}
 
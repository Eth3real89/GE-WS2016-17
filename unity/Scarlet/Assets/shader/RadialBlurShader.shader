// Upgrade NOTE: replaced 'samplerRECT' with 'sampler2D'
// Upgrade NOTE: replaced 'texRECT' with 'tex2D'

Shader "Custom/RadialBlur" {
Properties {
    _MainTex ("Input", RECT) = "white" {}
    _BlurStrength ("", Float) = 0.5
    _BlurWidth ("", Float) = 0.5
    _HaveRedBlur("", Float) = 0
}
    SubShader {
        Pass {
            ZTest Always Cull Off ZWrite Off
            Fog { Mode off }
       
    CGPROGRAM
// Upgrade NOTE: excluded shader from DX11, Xbox360, OpenGL ES 2.0 because it uses unsized arrays
#pragma exclude_renderers xbox360 gles
   
    #pragma vertex vert_img
    #pragma fragment frag
    #pragma fragmentoption ARB_precision_hint_fastest
 
    #include "UnityCG.cginc"
 
    uniform sampler2D _MainTex;
    uniform half _BlurStrength;
    uniform half _BlurWidth;
    uniform half _HaveRedBlur;
 
    half4 frag (v2f_img i) : COLOR {
        half4 color = tex2D(_MainTex, i.uv);
       
        // some sample positions
        half samples[10] = 
        {
        	-0.08, -0.05, -0.03, 
        	-0.02, -0.01, +0.01, 
        	+0.02, +0.03, +0.05, 
        	+0.08
        };
       
        //vector to the middle of the screen
        half2 dir = 0.5 - i.uv;
       
        //distance to center
        half dist = sqrt(dir.x * dir.x + dir.y * dir.y);
       
        //normalize direction
        dir = dir / dist;
       
        //additional samples towards center of screen
        half4 sum = color;
        for(int n = 0; n < 10; n++)
        {
            sum += tex2D(_MainTex, i.uv + dir * samples[n] * _BlurWidth);
        }
       
        //eleven samples... 1 / 11 = 0.09 where 0 and 9 are repeated periodically
        sum *= 0.090909091;
       
        //weighten blur depending on distance to screen center
        half t = dist * _BlurStrength;
        t = clamp(t, 0.0, 1.0);

        if(sum.r < 0.5 && _HaveRedBlur == 1)
        {
		sum.r = 0.5;
        }


        //blend original with blur
        return lerp(color, sum, t);
    }
    ENDCG
        }
    }
}
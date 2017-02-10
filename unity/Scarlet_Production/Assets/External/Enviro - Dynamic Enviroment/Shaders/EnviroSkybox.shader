// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "Enviro/Skybox"
{
	Properties
	{
		_Stars   ("Stars Cubemap", Cube)        = "black" {}
		_StarsNoise   ("Stars Noise Cubemap", Cube)        = "black" {}
		_noiseScale ("Noise Scale", Float ) = 240
		_noiseIntensity ("Noise Intensity", Float ) = 1
	}
	SubShader 
	{
	    Tags { "Queue"="Background" "RenderType"="Background" "PreviewType"="Skybox" "IgnoreProjector"="True" }
	    Cull Back     
		Fog {Mode Off} 
		ZWrite Off    

    	Pass 
    	{
			CGPROGRAM
			#pragma target 3.0
			#pragma vertex vert
			#pragma fragment frag

			uniform float3 _waveLenght;
			uniform float3 _Br;
			uniform float3 _Bm;
			uniform float3 _mieG;

			uniform float  _SunIntensity;

			uniform float  _Exposure;
			uniform float  _SkyLuminance;
			uniform float  _scatteringPower;
			uniform float  _SunDiskSize;
		    uniform float  _SunDiskIntensity;
		    uniform float  _StarsIntensity;
			uniform float4 _scatteringColor;
			uniform float4  _sunDiskColor;
			uniform samplerCUBE  _Stars;
			uniform samplerCUBE  _StarsNoise;
			uniform float     _SkyCorrection;
			uniform float3    _SunDir;
			uniform float4x4  _Sun;
			uniform float4x4  _Noise;
			uniform float4  _weatherColor;

			uniform float _noiseScale;
			uniform half _noiseIntensity;

			struct appdata
			{
			    float4 vertex   : POSITION;
			};

			struct v2f 
			{
    			float4 Position : SV_POSITION;
    			float4 WorldPosition : TEXCOORD0;
    			float3 sunPos : TEXCOORD1;
    			float4 fading : TEXCOORD2;
    			float2 night : TEXCOORD3;
    			float3 noise : TEXCOORD4;

			};

			v2f vert(appdata v)
			{
    			v2f o;
    			UNITY_INITIALIZE_OUTPUT(v2f, o);		
    			o.Position = mul(UNITY_MATRIX_MVP, v.vertex);
    			o.WorldPosition = normalize(mul((float4x4)unity_ObjectToWorld, v.vertex)).xyzw;
    			float3 viewDir = normalize(o.WorldPosition+float3(0.0,0.05,0.0));	
    			o.fading.x = saturate( _SunDir.y + 0.25 );                        
			    o.fading.y = saturate(clamp(1.0 - _SunDir.y, 0.0, 0.5));                          
    			o.noise = mul((float3x3)_Noise,v.vertex.xyz); 						
    			o.sunPos   = mul((float3x3)_Sun,v.vertex.xyz);
    			o.night.x = pow(max(0.0,viewDir.y),1.25);
				
    			return o;
			}

			float4 frag(v2f i) : SV_Target
			{
		
			   float3 viewDir = normalize(i.WorldPosition);
			   float cosTheta = dot( viewDir, _SunDir );
			   viewDir = normalize(i.WorldPosition + float3(0.0, 0.2 ,0.0));

			   float  zen = acos(saturate(viewDir.y));
			   float  alb = (cos(zen) + 0.5 * pow(93.885 - ((zen * 180.0) / 3.141592), - 0.253)); // pi
			   float3 fex = exp(-(_Br * (4.5 / alb)  + _Bm * (1.25 / alb)));

			   float  rayPhase = 1.0 + pow(cosTheta,2.0);                 
			   float  miePhase = _mieG.x / pow(_mieG.y - _mieG.z * cosTheta, 1); 
			    
			   float3 BrTheta  = 0.059683 * _Br * rayPhase; //(3.0/(16.0*pi)) 
			   float3 BmTheta  = 0.079577  * _Bm * miePhase; //(1.0/(4.0*pi))
			   float3 BrmTheta = (BrTheta + BmTheta * 2.0) / ((_Bm + _Br) * 0.75);   

			   float3 scattering = BrmTheta * _SunIntensity * (1.0 - fex);
			   scattering *= saturate((lerp( _scatteringPower , pow(2000.0 * BrmTheta * fex,0.5),i.fading.y) * 0.05));
			   scattering *= _SkyLuminance * _scatteringColor.rgb;
			   scattering *= pow((1 - fex),1);
			   scattering *= i.fading.x;

			   float3 sunDisk = min(2, pow((1-cosTheta) * (_SunDiskSize * 100) , -2 )) * fex * _SunDiskIntensity;

			   sunDisk *= saturate(_sunDiskColor);

			   float3 skyFinalize = saturate((pow( 1.0 - fex, 2.0) * 0.234) * (1 - i.fading.x)); 

			   skyFinalize *= _SkyLuminance;
			   skyFinalize = saturate(lerp(float4(0,0,0,0), skyFinalize, saturate(dot(viewDir.y + 0.3, float3(0,1,0)))) * (1-fex));

			   float fadeStar = i.night.x * _StarsIntensity * (10.0); 
			   float starFlicker = texCUBE(_StarsNoise, i.noise.xyz) * 2.0;
			   float3 stars = saturate(texCUBE(_Stars, i.sunPos.xyz) * fadeStar) * starFlicker;
			  
			   skyFinalize += stars;

			   scattering += sunDisk;

			   float3 finalS = (scattering + skyFinalize);
	
			   //Tonemapping
			   finalS = saturate( 1.0 - exp( -_Exposure * finalS ));

			   finalS = pow(finalS,_SkyCorrection);

			   finalS = lerp(finalS, saturate(finalS * (_weatherColor + 0.0001)),_weatherColor.a);
			   	// Color bandińg fix NOT WORKING WITH UNITY REFLECTION AT THE MOMENT!
				//float2 wcoord = (i.WorldPosition.xy/i.WorldPosition.w) * _noiseScale;
				//float4 dither = ( dot( float2( 171.0f, 231.0f ), wcoord.xy ) );
				//dither.rgb = frac( dither / float3( 103.0f, 71.0f, 97.0f ) ) - float3( 0.5f, 0.5f, 0.5f );

			    //return float4(finalS,1.0) + (dither/255.0f) * _noiseIntensity;
			   return float4(finalS,1.0);
			}
			ENDCG
    	}
    }
}
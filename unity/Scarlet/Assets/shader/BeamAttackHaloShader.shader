Shader "Custom/BeamAttackHalo"
{
	Properties
	{
		_FadePower("Fade Power", Range(1, 10)) = 5.0
		_MainTex ("Texture", 2D) = "white" {}
	}
		SubShader
	{
		Tags{ "RenderType"="Transparent" "Queue"="Transparent" "IgnoreProjector"="True" "ForceNoShadowCasting"="True"}
		Blend SrcAlpha DstAlpha


		Pass
	{
		CGPROGRAM
#pragma vertex vert
#pragma fragment frag

#include "UnityCG.cginc"

	struct appdata
	{
		float4 vertex : POSITION;
		float3 normal : NORMAL;
		float2 uv : TEXCOORD0;
	};

	struct v2f
	{
		float2 uv : TEXCOORD0;
		float4 vertex : SV_POSITION;
		float dotProduct : VIEWDIRECTION;
	};

	float _FadePower;

	v2f vert(appdata v)
	{
		v2f o;
		o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
		float3 viewDir = normalize(WorldSpaceViewDir(v.vertex));
		viewDir.y = 0;
		viewDir = normalize(viewDir);

		o.dotProduct = dot(v.normal, viewDir);
		o.uv = v.uv;

		return o;
	}

	sampler2D _MainTex;

	fixed4 frag(v2f i) : SV_Target
	{
		fixed4 col = tex2D(_MainTex, i.uv);

		if (i.dotProduct <= 0.1) {
			clip(-1);
		}
		else if (i.dotProduct > 0.1) {
			col.a = col.a * pow(i.dotProduct, _FadePower);

		}

		col.r = 0.6;

		return col;
	}
		ENDCG
	}
	}
}

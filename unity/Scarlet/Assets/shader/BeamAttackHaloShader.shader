Shader "Custom/BeamAttackHalo"
{
	Properties
	{
		_Color("Color", Color) = (1, 1, 1, 1)
		_FadePower("Fade Power", Range(1, 10)) = 5.0
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
	};

	struct v2f
	{
		float4 vertex : SV_POSITION;
		float dotProduct : VIEWDIRECTION;
	};

	float4 _Color;
	float _FadePower;

	v2f vert(appdata v)
	{
		v2f o;
		o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
		float3 viewDir = normalize(WorldSpaceViewDir(v.vertex));
		viewDir.y = 0;
		viewDir = normalize(viewDir);

		o.dotProduct = dot(v.normal, viewDir);
		return o;
	}

	fixed4 frag(v2f i) : SV_Target
	{
		if (i.dotProduct <= 0.1) {
			clip(-1);
		}
		else if (i.dotProduct > 0.1) {
			_Color.w = _Color.w*pow(i.dotProduct, _FadePower);

		}

		return _Color;
	}
		ENDCG
	}
	}
}

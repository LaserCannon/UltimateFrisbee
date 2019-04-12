// Upgrade NOTE: replaced '_Projector' with 'unity_Projector'
// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Projector/Projector Multiply Black"
{ 
Properties
{
    _ShadowTex ("Cookie", 2D) = "gray" { TexGen ObjectLinear }
    _ShadowStrength ("Strength",float) = 1
}

Subshader
{
    Tags { "RenderType"="Transparent"  "Queue"="Transparent+100"}
    Pass
    {
        ZWrite Off
        Offset -1, -1

        //Fog { Mode Off }

        Blend DstColor Zero
        
		CGPROGRAM
		#pragma vertex vert
		#pragma fragment frag
		#pragma fragmentoption ARB_fog_exp2
		#pragma fragmentoption ARB_precision_hint_fastest
		#include "UnityCG.cginc"
		
		struct v2f
		{
			float4 pos : SV_POSITION;
			float2 uv_Main     : TEXCOORD0;
		};
		
		sampler2D _ShadowTex;
		float4x4 unity_Projector;
		float _ShadowStrength;
		
		v2f vert(appdata_tan v)
		{
			v2f o;
			o.pos = UnityObjectToClipPos (v.vertex);
			o.uv_Main = mul (unity_Projector, v.vertex).xy;
			return o;
		}
		
		half4 frag (v2f i) : COLOR
		{
			half4 tex = tex2D(_ShadowTex, i.uv_Main);
			half strength = (1 - tex.a*_ShadowStrength);
			tex = (strength,strength,strength,strength);
			return tex;
		}
		ENDCG

    }
}
}
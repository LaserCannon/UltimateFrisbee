Shader "Unlit/Color" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		CGPROGRAM
		#pragma surface surf Lambert


		half4 _Color;
			
		struct Input {
			half dummy;
		};

		void surf (Input IN, inout SurfaceOutput o) {
			o.Emission = _Color.rgb;
		}
		ENDCG
	} 
	FallBack "Diffuse"
}

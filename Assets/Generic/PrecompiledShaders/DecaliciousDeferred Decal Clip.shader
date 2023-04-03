Shader "Decalicious/Deferred Decal Clip" {
	Properties {
		_MaskTex ("Mask", 2D) = "white" {}
		[PerRendererData] _MaskMultiplier ("Mask (Multiplier)", Float) = 1
		_MaskNormals ("Mask Normals?", Float) = 1
		[PerRendererData] _LimitTo ("Limit To", Float) = 0
		_MaskClip ("MaskClip", Range(0, 1)) = 1
		_MainTex ("Albedo", 2D) = "white" {}
		[HDR] _Color ("Albedo (Multiplier)", Vector) = (1,1,1,1)
		_EmissionMultiplier ("Emission (Multiplier)", Float) = 0
		[Normal] _NormalTex ("Normal", 2D) = "bump" {}
		_NormalMultiplier ("Normal (Multiplier)", Float) = 1
		_SpecularTex ("Specular", 2D) = "white" {}
		_SpecularMultiplier ("Specular (Multiplier)", Vector) = (0.2,0.2,0.2,1)
		_SmoothnessTex ("Smoothness", 2D) = "white" {}
		_SmoothnessMultiplier ("Smoothness (Multiplier)", Range(0, 1)) = 0.5
		_DecalBlendMode ("Blend Mode", Float) = 0
		_DecalSrcBlend ("SrcBlend", Float) = 1
		_DecalDstBlend ("DstBlend", Float) = 10
		_NormalBlendMode ("Normal Blend Mode", Float) = 0
		_AngleLimit ("Angle Limit", Float) = 0.5
	}
	//DummyShaderTextExporter
	SubShader{
		Tags { "RenderType"="Opaque" }
		LOD 200
		CGPROGRAM
#pragma surface surf Standard
#pragma target 3.0

		sampler2D _MainTex;
		fixed4 _Color;
		struct Input
		{
			float2 uv_MainTex;
		};
		
		void surf(Input IN, inout SurfaceOutputStandard o)
		{
			fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
			o.Albedo = c.rgb;
			o.Alpha = c.a;
		}
		ENDCG
	}
	//CustomEditor "ThreeEyedGames.DecalShaderGUI"
}
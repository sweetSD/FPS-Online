Shader "Decal"
{
	Properties
	{
		_MainTex("Base (RGB)", 2D) = "white" {}
		_Alpha("Alpha", Range(0, 1)) = 1
	}

	SubShader
	{
		Tags { "RenderType" = "Opaque" "Queue" = "Geometry+1" "ForceNoShadowCasting" = "True" }
		LOD 200
		Offset -1, -1

		CGPROGRAM
		#pragma surface surf Lambert decal:blend

		sampler2D _MainTex;
		half _Alpha;

		struct Input 
		{
			float2 uv_MainTex;
		};

		void surf(Input IN, inout SurfaceOutput o) 
		{
			half4 c = tex2D(_MainTex, IN.uv_MainTex);
			o.Albedo = c.rgb;
			o.Alpha = c.a * _Alpha;
		}
		ENDCG
	}
}
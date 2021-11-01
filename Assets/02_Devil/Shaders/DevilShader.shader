Shader "DevilShader"
{
	Properties
	{
		_Color("Color", Color) = (1,1,1,1)
		_MainTex("Main Texture", 2D) = "black" {}
		_Cutout( "Alpha Cutout", Float ) = 0.5
	}

	SubShader
	{
		Tags { "RenderType" = "TransparentCutout" "Queue" = "AlphaTest" "IgnoreProjector" = "True" }
		Cull Off
		CGPROGRAM
		#pragma target 3.0
		#pragma surface surf Lambert fullforwardshadows alphatest:_Cutout addshadow
		
    struct Input
		{
			float2 uv_MainTex;
		};

		uniform sampler2D _MainTex;
    uniform fixed4 _Color;

		void surf(Input i, inout SurfaceOutput o)
		{
			float4 albedo = tex2D(_MainTex, i.uv_MainTex) * _Color;
			o.Albedo = albedo.rgb;
      o.Alpha = albedo.a;
		}

		ENDCG
	}

	Fallback "Standard (Specular setup)"
	Fallback "Diffuse"
}

Shader "TM/HalfLambert" {
	Properties {
		_MainTint ("Main Tint", Color) = (0.5,0.5,0.5,0.5)
		_MainTex ("Base (RGB)", 2D) = "white" {}
//		_RimPower ("Rim Power", Range(0.01,3.0)) = 1.5
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		CGPROGRAM
//		#include "CustomLighting.gcinc"
		#pragma surface surf HalfLambert
		
		fixed _RimPower;
		
		inline fixed4 LightingHalfLambert (SurfaceOutput s, fixed3 lightDir, fixed3 viewDir, fixed atten)
		{
			//Calculate the Half vector
//			fixed3 halfVector = normalize(lightDir + viewDir);
						
			//diffuse lighting
			fixed NdotL = max(0, dot(s.Normal, lightDir));
			
			//more dot products
//			fixed EdotH = max (0, dot (viewDir , halfVector));
//			fixed NdotH = max (0, dot (s.Normal , halfVector));
//			fixed NdotE = max (0, dot (s.Normal , viewDir));
						
			//halfLambert
			fixed halfLambert =  pow((NdotL * 0.5 + 0.5),2.0);
			//rim		
//			fixed rimLight = 1-NdotE;
			
//			rimLight = pow(rimLight, _RimPower) * NdotH;
			
			fixed4 finalColor;
			finalColor.rgb = (s.Albedo * _LightColor0.rgb )  * (halfLambert * atten  * 2);
			finalColor.a = 0.5;
			return finalColor;
		}

		sampler2D _MainTex;
		fixed4 _MainTint;

		struct Input {
			float2 uv_MainTex;
		};

		void surf (Input IN, inout SurfaceOutput o) {
			
			half4 c = tex2D (_MainTex, IN.uv_MainTex);
			o.Albedo = c.rgb *_MainTint;
			o.Alpha =  c.a;
		}
		ENDCG
	} 
	//FallBack "Diffuse"
}

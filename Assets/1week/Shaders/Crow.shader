Shader "Custom/Crow" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_Glossiness ("Smoothness", Range(0,1)) = 0.5
		_Metallic ("Metallic", Range(0,1)) = 0.0

		//追加プロパティ
		_BodySize("BodySize", float) = 0.0
		_FlapSpeed ("FlapSpeed", float) = 0.0
		_FlapOffset("FlapOffset", float) = 0.0
		_FlapScaleX("FlapScaleX", float) = 0.0
		_FlapScaleY("FlapScaleY", float) = 0.0
		_FlapAngle("FlapAngle", float) = 0.0
	}
	SubShader {
		Tags { "RenderType"="Opaque" "DisableBatching" = "True" }
		LOD 200

		CGPROGRAM
		// 頂点シェーダを追加
		#pragma surface surf Standard fullforwardshadows vertex:vert addshadow
		#pragma target 3.0

		//GPUインスタンシング可能
		#pragma multi_compile_instancing
		#define PI 3.14159

		sampler2D _MainTex;

		struct Input {
			float2 uv_MainTex;
		};

		half _Glossiness;
		half _Metallic;
		
		//追加のプロパティ
		half _BodySize;
		//half _FlapSpeed;
		//half _FlapOffset;
		half _FlapScaleX;
		half _FlapScaleY;
		half _FlapAngle;

		UNITY_INSTANCING_BUFFER_START(Props)
		//インスタンスごとのプロパティ
			UNITY_DEFINE_INSTANCED_PROP(fixed4, _Color)
			UNITY_DEFINE_INSTANCED_PROP(float, _FlapSpeed)
			UNITY_DEFINE_INSTANCED_PROP(float, _FlapOffset)
		UNITY_INSTANCING_BUFFER_END(Props)

		//頂点シェーダ
		void vert(inout appdata_full v)
		{
			UNITY_SETUP_INSTANCE_ID(v);

			//flap:羽ばたき
			float flap = abs(v.vertex.x) * (sin(_Time.y * UNITY_ACCESS_INSTANCED_PROP(Props, _FlapSpeed) + UNITY_ACCESS_INSTANCED_PROP(Props, _FlapOffset)));
			//float flap = abs(v.vertex.x) * (sin(_Time.y * _FlapSpeed + _FlapOffset));
			//体の部分は動かさない
			flap *= step(_BodySize/100, abs(v.vertex.x));
			//足元は動かさない（多少強引）
			flap *= step(0.0, v.vertex.y);
			v.vertex.x +=  (v.vertex.x < 0)? flap*flap * _FlapScaleX : -flap*flap * _FlapScaleX;
            v.vertex.y += _FlapScaleY * flap * sin(_FlapAngle * PI / 180);
			v.vertex.z += _FlapScaleY * flap * cos(_FlapAngle * PI / 180);
		}

		void surf (Input IN, inout SurfaceOutputStandard o) {
			// テクスチャとカラーの計算を加算合成に変更
			fixed4 c = (tex2D (_MainTex, IN.uv_MainTex) + UNITY_ACCESS_INSTANCED_PROP(Props, _Color) * 2)/3;
			//fixed4 c = tex2D (_MainTex, IN.uv_MainTex);
			o.Albedo = c.rgb;

			o.Metallic = _Metallic;
			o.Smoothness = _Glossiness;
			//o.Alpha = c.a;
		}
		ENDCG
	}
	FallBack "Diffuse"
}

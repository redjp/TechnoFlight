Shader "Unlit/CrowFlag"
{
	Properties
	{
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Texture", 2D) = "white" {}

		//追加プロパティ
		_BodySize("BodySize", float) = 0.0
		_FlapSpeed ("FlapSpeed", float) = 0.0
		_FlapOffset("FlapOffset", float) = 0.0
		_FlapScaleX("FlapScaleX", float) = 0.0
		_FlapScaleY("FlapScaleY", float) = 0.0
		_FlapAngle("FlapAngle", float) = 0.0
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" "DisableBatching" = "True" }
		LOD 100

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			#pragma multi_compile_instancing
			#define PI 3.14159
			
			#include "UnityCG.cginc"

			struct appdata
			{
				UNITY_VERTEX_INPUT_INSTANCE_ID
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			
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

			v2f vert (appdata v)
			{
				v2f o;
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

				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 col = (tex2D (_MainTex, i.uv) + UNITY_ACCESS_INSTANCED_PROP(Props, _Color) * 2)/3;;

				return col;
			}
			ENDCG
		}
	}
}

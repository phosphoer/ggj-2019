Shader "Custom/Opaque"
{
  Properties
  {
    _Color ("Color", Color) = (1,1,1,1)
    _MainTex ("Texture", 2D) = "white" {}
    _FogScale ("Fog Scale", float) = 1

    [Enum(Off,0,On,1)] 
    _ZWrite ("ZWrite", Float) = 1
    
    [Enum(UnityEngine.Rendering.CompareFunction)] 
    _ZTest ("ZTest", Float) = 4

    [Enum(UnityEngine.Rendering.CullMode)] 
    _CullMode ("Cull Mode", Float) = 2
  }
  SubShader
  {
    Tags { "RenderType"="Opaque" }
    ZWrite [_ZWrite]
    ZTest [_ZTest]
    Cull [_CullMode]
    
    Pass
    {
      CGPROGRAM
      #pragma vertex vert
      #pragma fragment frag
      #pragma multi_compile_fog			

      #include "UnityCG.cginc"

      struct appdata
      {
        float4 vertex : POSITION;
        fixed4 color : COLOR;
        fixed3 normal: NORMAL;
        float2 uv : TEXCOORD0;
      };

      struct v2f
      {
        float4 pos : SV_POSITION;
        float3 normal : NORMAL;
        fixed4 color : COLOR;
        float2 uv : TEXCOORD0;
        UNITY_FOG_COORDS(1)
      };

      sampler2D _MainTex;
      float4 _Color;
      float _FogScale;

      v2f vert (appdata v)
      {
        v2f o;
        o.pos = UnityObjectToClipPos(v.vertex);
        o.normal = UnityObjectToWorldNormal(v.normal);
        o.uv = v.uv;
        o.color = v.color;

        UNITY_TRANSFER_FOG(o, o.pos);

        return o;
      }

      fixed4 frag (v2f i) : SV_Target
      {
        fixed4 color = _Color * tex2D(_MainTex, i.uv) * i.color;
        fixed4 fogColor = _Color * tex2D(_MainTex, i.uv) * i.color;
        UNITY_APPLY_FOG(i.fogCoord, fogColor);
        return lerp(color, fogColor, _FogScale); 
      }
      ENDCG
    }
  }

  FallBack "VertexLit"
}

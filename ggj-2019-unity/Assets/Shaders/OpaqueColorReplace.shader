Shader "Custom/OpaqueColorReplace"
{
  Properties
  {
    _Color ("Color", Color) = (1,1,1,1)
    _MainTex ("Texture", 2D) = "white" {}
    _FogScale ("Fog Scale", float) = 1
    _ReplaceColor ("Replace Color", Color) = (1, 0, 1, 1)
    _ReplaceWithColor ("Replace With Color", Color) = (1, 1, 1, 1)
    _ReplacementThreshold ("Replacement Threshold", float) = 0.1
    _HighlightColor ("Highlight Color", Color) = (0, 0, 0, 0)
    _LightingScale ("Lighting Scale", float) = 0.1

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
        fixed3 normal: NORMAL;
        fixed4 color : COLOR;
        float2 uv : TEXCOORD0;
        float3 worldPos : TEXCOORD1;
      };

      struct v2f
      {
        float4 pos : SV_POSITION;
        fixed3 worldNormal: NORMAL;
        fixed4 color : COLOR;
        float2 uv : TEXCOORD0;
        float3 worldPos : TEXCOORD2;
        UNITY_FOG_COORDS(2)
      };

      sampler2D _MainTex;
      fixed4 _Color;
      fixed4 _ReplaceColor;
      fixed4 _ReplaceWithColor;
      fixed4 _HighlightColor;
      fixed _ReplacementThreshold;
      fixed _LightingScale;
      fixed _FogScale;

      v2f vert (appdata v)
      {
        v2f o;
        o.pos = UnityObjectToClipPos(v.vertex);
        o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
        o.worldNormal = UnityObjectToWorldNormal(v.normal);
        o.uv = v.uv;
        o.color = v.color;

        UNITY_TRANSFER_FOG(o, o.pos);

        return o;
      }

      fixed4 frag (v2f i) : SV_Target
      {
        fixed4 texColor = tex2D(_MainTex, i.uv);
        fixed distToReplaceColor = length(_ReplaceColor - texColor);
        fixed replaceAmount = 1 - saturate(distToReplaceColor / _ReplacementThreshold);
        fixed4 texColorReplaced = lerp(texColor, _ReplaceWithColor, replaceAmount);
        fixed4 color = texColorReplaced * _Color * i.color;
        color.a = 1;
        color.rgb += _HighlightColor.rgb;

        float lightDot = max(dot(normalize(i.worldNormal), float3(1, 1, 1)), 0);
        lightDot = saturate(ceil(lightDot - 0.9)) * _LightingScale;
        color += lightDot;

        fixed4 fogColor = color;
        UNITY_APPLY_FOG(i.fogCoord, fogColor);
        return lerp(color, fogColor, _FogScale); 
      }
      ENDCG
    }
  }

  FallBack "VertexLit"
}

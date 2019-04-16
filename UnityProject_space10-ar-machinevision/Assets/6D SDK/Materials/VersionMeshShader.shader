Shader "6D/VersionMeshShader" {
    Properties {
        _Alpha ("Alpha", Range(0,1)) = 0.5
        _DistanceClose ("Distance Close", Range(0,1000)) = 1.0
        _DistanceFar ("Distance Far", Range(0,1000)) = 10.0
    }
    SubShader {
        Tags {"Queue"="Geometry-10" "RenderType"="Transparent" }
        LOD 100

        Pass {
            ZWrite On
            ColorMask 0
       
            CGPROGRAM
            #include "UnityCG.cginc"
            #pragma vertex vert
            #pragma fragment frag
     
            struct v2f {
                float4 pos : SV_POSITION;
            };
     
            v2f vert (appdata_base v) {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                return o;
            }
     
            half4 frag (v2f i) : COLOR {
                return half4 (0,0,0,0);
            }
            ENDCG  
        }

        Pass {
            Blend SrcAlpha OneMinusSrcAlpha

            CGPROGRAM
            #include "UnityCG.cginc"
            #include "UnityLightingCommon.cginc"
            #pragma vertex vert
            #pragma fragment frag
         
            struct v2f {
                float4 pos : SV_POSITION;
                fixed4 color : COLOR0;
                fixed4 diff : COLOR1;
            };

            fixed _Alpha;
            half _DistanceClose;
            half _DistanceFar;
            
            v2f vert (appdata_full v) {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                half3 normal = UnityObjectToWorldNormal(v.normal);
                half l = min(1.0, max(0.0, (length(ObjSpaceViewDir(v.vertex)) - _DistanceClose) / (_DistanceFar - _DistanceClose)));
                o.color.rgb = v.color.rgb;
                o.color.a = lerp(_Alpha, 0.0, l);
                half nl = max(0.0, dot(normal, _WorldSpaceLightPos0.xyz));
                o.diff = nl * _LightColor0;
                o.diff.a = 1.0;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target {
                return (i.diff * i.color + i.color) * 0.5;
            }
            ENDCG
        }
    } 
}
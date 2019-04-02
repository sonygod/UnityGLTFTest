// Shader created with Shader Forge v1.38 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.38;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,cgin:,lico:1,lgpr:1,limd:0,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,imps:True,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:0,bsrc:0,bdst:1,dpts:2,wrdp:True,dith:0,atcv:False,rfrpo:True,rfrpn:Refraction,coma:15,ufog:True,aust:True,igpj:False,qofs:0,qpre:2,rntp:3,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.1280277,fgcg:0.1953466,fgcb:0.2352941,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,atwp:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:False,fnfb:False,fsmp:False;n:type:ShaderForge.SFN_Final,id:9361,x:32887,y:32686,varname:node_9361,prsc:2|custl-3840-RGB,clip-2151-OUT;n:type:ShaderForge.SFN_Tex2d,id:3840,x:32397,y:32699,ptovrint:False,ptlb:MainTex,ptin:_MainTex,varname:node_3840,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:b66bceaf0cc0ace4e9bdc92f14bba709,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Tex2d,id:336,x:32253,y:32910,ptovrint:False,ptlb:Nosie,ptin:_Nosie,varname:node_336,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:28c7aad1372ff114b90d330f8a2dd938,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Slider,id:9805,x:31745,y:33135,ptovrint:False,ptlb:speedSlider,ptin:_speedSlider,varname:node_9805,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0.4672706,max:1;n:type:ShaderForge.SFN_Subtract,id:2427,x:32443,y:33028,varname:node_2427,prsc:2|A-336-R,B-8200-OUT;n:type:ShaderForge.SFN_Lerp,id:2151,x:32644,y:33160,varname:node_2151,prsc:2|A-6421-OUT,B-2427-OUT,T-8200-OUT;n:type:ShaderForge.SFN_Vector1,id:6421,x:32443,y:33173,varname:node_6421,prsc:2,v1:1;n:type:ShaderForge.SFN_Time,id:4836,x:31175,y:33448,varname:node_4836,prsc:2;n:type:ShaderForge.SFN_ValueProperty,id:5450,x:31175,y:33635,ptovrint:False,ptlb:speed,ptin:_speed,varname:node_5450,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:1;n:type:ShaderForge.SFN_Divide,id:2284,x:31374,y:33501,varname:node_2284,prsc:2|A-4836-T,B-5450-OUT;n:type:ShaderForge.SFN_Clamp01,id:9596,x:31568,y:33501,varname:node_9596,prsc:2|IN-2284-OUT;n:type:ShaderForge.SFN_Get,id:8200,x:32087,y:33267,varname:node_8200,prsc:2|IN-6344-OUT;n:type:ShaderForge.SFN_Set,id:6344,x:32121,y:33363,varname:speed,prsc:2|IN-9805-OUT;n:type:ShaderForge.SFN_Set,id:8464,x:31601,y:33655,varname:autoSpped,prsc:2|IN-9596-OUT;proporder:3840-336-9805-5450;pass:END;sub:END;*/

Shader "Shader Forge/NewShader" {
    Properties {
        _MainTex ("MainTex", 2D) = "white" {}
        _Nosie ("Nosie", 2D) = "white" {}
        _speedSlider ("speedSlider", Range(0, 1)) = 0.4672706
        _speed ("speed", Float ) = 1
        [HideInInspector]_Cutoff ("Alpha cutoff", Range(0,1)) = 0.5
    }
    SubShader {
        Tags {
            "Queue"="AlphaTest"
            "RenderType"="TransparentCutout"
        }
        Pass {
            Name "FORWARD"
            Tags {
                "LightMode"="ForwardBase"
            }
            
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDBASE
            #include "UnityCG.cginc"
            #pragma multi_compile_fwdbase_fullshadows
            #pragma multi_compile_fog
            #pragma only_renderers d3d9 d3d11 glcore gles 
            #pragma target 3.0
            uniform sampler2D _MainTex; uniform float4 _MainTex_ST;
            uniform sampler2D _Nosie; uniform float4 _Nosie_ST;
            uniform float _speedSlider;
            struct VertexInput {
                float4 vertex : POSITION;
                float2 texcoord0 : TEXCOORD0;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                UNITY_FOG_COORDS(1)
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.pos = UnityObjectToClipPos( v.vertex );
                UNITY_TRANSFER_FOG(o,o.pos);
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
                float4 _Nosie_var = tex2D(_Nosie,TRANSFORM_TEX(i.uv0, _Nosie));
                float speed = _speedSlider;
                float node_8200 = speed;
                clip(lerp(1.0,(_Nosie_var.r-node_8200),node_8200) - 0.5);
////// Lighting:
                float4 _MainTex_var = tex2D(_MainTex,TRANSFORM_TEX(i.uv0, _MainTex));
                float3 finalColor = _MainTex_var.rgb;
                fixed4 finalRGBA = fixed4(finalColor,1);
                UNITY_APPLY_FOG(i.fogCoord, finalRGBA);
                return finalRGBA;
            }
            ENDCG
        }
        Pass {
            Name "ShadowCaster"
            Tags {
                "LightMode"="ShadowCaster"
            }
            Offset 1, 1
            Cull Back
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_SHADOWCASTER
            #include "UnityCG.cginc"
            #include "Lighting.cginc"
            #pragma fragmentoption ARB_precision_hint_fastest
            #pragma multi_compile_shadowcaster
            #pragma multi_compile_fog
            #pragma only_renderers d3d9 d3d11 glcore gles 
            #pragma target 3.0
            uniform sampler2D _Nosie; uniform float4 _Nosie_ST;
            uniform float _speedSlider;
            struct VertexInput {
                float4 vertex : POSITION;
                float2 texcoord0 : TEXCOORD0;
            };
            struct VertexOutput {
                V2F_SHADOW_CASTER;
                float2 uv0 : TEXCOORD1;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.pos = UnityObjectToClipPos( v.vertex );
                TRANSFER_SHADOW_CASTER(o)
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
                float4 _Nosie_var = tex2D(_Nosie,TRANSFORM_TEX(i.uv0, _Nosie));
                float speed = _speedSlider;
                float node_8200 = speed;
                clip(lerp(1.0,(_Nosie_var.r-node_8200),node_8200) - 0.5);
                SHADOW_CASTER_FRAGMENT(i)
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}

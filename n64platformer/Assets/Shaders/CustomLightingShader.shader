// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'

Shader "Unlit/CustomLightingShader"
{
    // Shader-specific properties that will be accessed in the subshaders defined below

    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _PointLight0("Point Light 0", Vector) = (0, 0, 0, 0)
        _PointLight1("Point Light 1", Vector) = (0, 0, 0, 0)
    }

    // From: https://docs.unity3d.com/Manual/SL-SubShader.html
    // SubShaders let you define different GPU settings and shader programs for different hardware, 
    // render pipelines, and runtime settings. Some Shader objects contain only a single SubShader; 
    // others contain multiple SubShaders to support a range of different configurations.

    SubShader 
    {
        // From: https://docs.unity3d.com/Manual/SL-SubShaderTags.html
        // Tags are key-value pairs of data. Unity uses predefined keys and values to determine how 
        // and when to use a given SubShader, or you can also create your own custom SubShader tags 
        // with custom values. You can access SubShader tags from C# code.
        Tags { "RenderType"="Opaque" }
        
        // From: https://docs.unity3d.com/Manual/SL-SubShaderTags.html
        // Use this technique to fine-tune shader performance on different hardware. This is useful 
        // when a SubShader is theoretically supported by a user’s hardware, but the hardware is not 
        // capable of running it well.
        
        // Note: Note: Inside your Shader block, you must put your SubShaders in descending LOD order. 
        // For example, if you have SubShaders with LOD values of 200, 100, and 500, you must put the 
        // SubShader with the LOD value of 500 first, followed by the one with a LOD value of 200, followed 
        // by the one with a LOD value of 100. This is because Unity selects the first valid SubShader it finds, 
        // so if it finds one with a lower LOD first it will always use that.

        LOD 100

        Pass
        {
            CGPROGRAM /* Needed to specify what to run in a pass? Not entirely sure why this is here */
            #pragma vertex vert /* implementation dependent compiler control for vertex function */
            #pragma fragment frag /* implementation dependent compiler control for fragment function */
            // make fog work
            #pragma multi_compile_fog 
            /* 
            multi_compile_fog expands to several variants to handle different fog types (off/linear/exp/exp2).
            I assume shaders will automatically compile the right fog variant type based on what I choose in the
            project settings..?
            */

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float3 normal : NORMAL;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex; 
            float4 _MainTex_ST;
            float4 _PointLight0;
            float4 _PointLight1;

            v2f vert (appdata v) /* Vertex Pass */
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex); /* Convert Object to Clip Space */
                o.normal = mul(unity_WorldToObject, float4(v.normal, 0.0) ).xyz;
                o.uv = TRANSFORM_TEX(v.uv, _MainTex); /* Scale and Offset Texture Coordinates */
                UNITY_TRANSFER_FOG(o,o.vertex); /* Calculate and store Fog Coordinates in the TEXCOORD1 channel */
                return o;
            }

            fixed4 frag (v2f i) : SV_Target /* Pixel/Fragment Pass */
            {
                // sample the texture

                /* N Dot L with point0 */
                // _PointLight0 = normalize(_PointLight0);
                // _PointLight1 = normalize(_PointLight1);
                
                float NDotL = 1 - (dot(_PointLight0.xyz, i.normal) + 1 ) / 2;
                NDotL += 1 - (dot(_PointLight1.xyz, i.normal) + 1 ) / 2;
                NDotL = min(NDotL, 1);

                fixed4 col = tex2D(_MainTex, i.uv); /* Read the specific color at the specified pixel */
                col *= NDotL;
                // apply fog
                // UNITY_APPLY_FOG(i.fogCoord, col); /* Apply fog at the respective pixel coordinate */
                return col;
            }

            /* Needed to specify the end of a shader pass I presume? */
            ENDCG
        }
    }
}

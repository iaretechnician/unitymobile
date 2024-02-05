// Made with Amplify Shader Editor v1.9.2.1
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "RCCP_WheelBlur"
{
	Properties
	{
		_WheelBlurTexture("Wheel Blur Texture", 2D) = "white" {}
		_BlurIntensity("Blur Intensity", Range( 0 , 1)) = 1
		_RimColor("Rim Color", Color) = (1,1,1,1)
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Transparent"  "Queue" = "Transparent+0" "IgnoreProjector" = "True" }
		Cull Off
		CGPROGRAM
		#pragma target 3.0
		#pragma surface surf Standard alpha:fade keepalpha 
		struct Input
		{
			float2 uv_texcoord;
		};

		uniform float4 _RimColor;
		uniform sampler2D _WheelBlurTexture;
		uniform float4 _WheelBlurTexture_ST;
		uniform float _BlurIntensity;

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float2 uv_WheelBlurTexture = i.uv_texcoord * _WheelBlurTexture_ST.xy + _WheelBlurTexture_ST.zw;
			float4 tex2DNode1 = tex2D( _WheelBlurTexture, uv_WheelBlurTexture );
			o.Albedo = ( _RimColor * tex2DNode1 ).rgb;
			o.Alpha = ( _BlurIntensity * tex2DNode1.a );
		}

		ENDCG
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=19201
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;160.9799,-7.661187;Float;False;True;-1;2;ASEMaterialInspector;0;0;Standard;RCCP_WheelBlur;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;False;False;False;False;False;False;Off;0;False;;0;False;;False;0;False;;0;False;;False;0;Transparent;0.5;True;False;0;False;Transparent;;Transparent;All;12;all;True;True;True;True;0;False;;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;2;15;10;25;False;0.5;True;2;5;False;;10;False;;0;0;False;;0;False;;0;False;;0;False;;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;True;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;;-1;0;False;;0;0;0;False;0.1;False;;0;False;;False;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
Node;AmplifyShaderEditor.SamplerNode;1;-650.5978,15.47895;Inherit;True;Property;_WheelBlurTexture;Wheel Blur Texture;0;0;Create;True;0;0;0;False;0;False;-1;bcfeef4f045edec46b3a6d32b2f929e0;bcfeef4f045edec46b3a6d32b2f929e0;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;34;-373.8139,-235.3724;Inherit;False;Property;_RimColor;Rim Color;2;0;Create;True;0;0;0;False;0;False;1,1,1,1;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;35;-19.9889,-74.78075;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;5;-234.2204,268.9893;Inherit;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;2;-250.8053,104.4329;Inherit;False;Property;_BlurIntensity;Blur Intensity;1;0;Create;True;0;0;0;False;0;False;1;0;0;1;0;1;FLOAT;0
WireConnection;0;0;35;0
WireConnection;0;9;5;0
WireConnection;35;0;34;0
WireConnection;35;1;1;0
WireConnection;5;0;2;0
WireConnection;5;1;1;4
ASEEND*/
//CHKSM=F15C2BEDC88ACA19D1EBB4E42A175E93EB43F40B
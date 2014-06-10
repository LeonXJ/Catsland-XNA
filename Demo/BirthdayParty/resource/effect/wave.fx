/*
 * Tutorial
 * XNA Shader programming
 * www.gamecamp.no
 * 
 * by: Petri T. Wilhelmsen
 * e-mail: petriw@gmail.com
 * 
 * Feel free to ask me a question, give feedback or correct mistakes!
 * This shader is mostly based on the shader "post edgeDetect" from nVidias Shader library:
 * http://developer.download.nvidia.com/shaderlibrary/webpages/shader_library.html
 */


// Global variables
// This will use the texture bound to the object( like from the sprite batch ).
texture ColorMap;
sampler ColorMapSampler = sampler_state
{
   Texture = <ColorMap>;
   MinFilter = Linear;
   MagFilter = Linear;
   MipFilter = Linear;   
   //AddressU  = Clamp;
   //AddressV  = Clamp;
};
//A timer we can use for whatever purpose we want
float fTimer;

struct OUT
{
    float4 Pos : POSITION;
    float2 Tex : TEXCOORD0;
};

OUT VS(float4 pos : POSITION, float2 tex : TEXCOORD)
{
    OUT Out = (OUT)0;
    Out.Pos = pos;
    Out.Tex = tex;
    return Out;
}


float4 PS(float2 Tex: TEXCOORD0) : COLOR
{
	// Use the timer to move the texture coordinated before using them to lookup
	// in the ColorMapSampler. This makes the scene look like its underwater
	// or something similar :)
	
	Tex.x += sin(fTimer+Tex.x*10)*0.1f;
	Tex.y += cos(fTimer+Tex.y*10)*0.1f;
	
	float4 Color = tex2D(ColorMapSampler, Tex);	
	
    return Color;

}

technique PostProcess
{
	pass P0
	{
		// A post process shader only needs a pixel shader.
		VertexShader = compile vs_2_0 VS();
        PixelShader = compile ps_2_0 PS();
	}
}
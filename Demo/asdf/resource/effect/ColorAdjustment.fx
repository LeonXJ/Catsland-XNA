

texture ColorMap;
sampler ColorMapSampler = sampler_state
{
   Texture = <ColorMap>;
   MinFilter = Linear;
   MagFilter = Linear;
   MipFilter = Linear;   
   AddressU  = Clamp;
   AddressV  = Clamp;
};

float Saturability;
float Illuminate;

struct OUT
{
    float4 Pos : POSITION;
    float2 Tex : TEXCOORD0;
};

OUT VS(float4 pos : POSITION, float2 tex : TEXCOORD0)
{
    OUT Out = (OUT)0;
    Out.Pos = pos;
    Out.Tex = tex;
    return Out;
}

float4 PS(float2 Tex: TEXCOORD) : COLOR
{
	float4 color = tex2D(ColorMapSampler, Tex);
	float4 illumColor = clamp(color + float4(Illuminate, Illuminate, Illuminate, 0.0), 0.0, 1.0);
	float lum = dot(illumColor, float4(0.3, 0.59, 0.11, 0.0));

	return lerp(float4(lum, lum, lum, illumColor.w), illumColor, Saturability);

}

technique ColorAdjustment
{
	pass P0
	{
		// A post process shader only needs a pixel shader.
		VertexShader = compile vs_2_0 VS();
        PixelShader = compile ps_2_0 PS();
	}
}
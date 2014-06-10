

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

float InnerRadius;
float OuterRadius;

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
	float4 color = tex2D(ColorMapSampler, Tex);
	float2 delta = float2(Tex.x - 0.5, Tex.y - 0.5);
	float dist2 = delta.x * delta.x + delta.y * delta.y;
	float innerRadius2 = InnerRadius * InnerRadius;
	float outerRadius2 = OuterRadius * OuterRadius;
	float darkness = 1.0 - clamp((dist2 - innerRadius2) / 
		(outerRadius2 - innerRadius2), 0.0, 1.0);
	return float4(color.xyz * darkness, color.w);

}

technique Vignette
{
	pass P0
	{
		VertexShader = compile vs_2_0 VS();
        PixelShader = compile ps_2_0 PS();
	}	
}
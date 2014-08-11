

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

texture DownSampledMap;
sampler DownSampledMapSampler = sampler_state
{
	Texture = <DownSampledMap>;
	MinFilter = Linear;
	MagFilter = Linear;
	MipFilter = Linear;
	AddressU = Clamp;
	AddressV = Clamp;	
};

float Exposure;

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

float4 exposure(float4 _color, float _gray, float _ex)
{
	float b = (4 * _ex - 1);
	float a = 1 - b;
	float f = _gray * (a * _gray + b);
	return float4(f * _color.xyz, _color.w);
}

float4 PS_1(float2 Tex: TEXCOORD0) : COLOR
{
	return tex2D(ColorMapSampler, Tex);
}

float4 PS_2(float2 Tex: TEXCOORD0) : COLOR
{
	float4 downSampledColor = tex2D(DownSampledMapSampler, Tex);
	float lum = 0.27 * downSampledColor.x + 0.67 * downSampledColor.y + 0.06 * downSampledColor.z;
	float4 color = tex2D(ColorMapSampler, Tex);
	return exposure(color, lum, Exposure);
}

technique HDR
{
	pass P0
	{
		VertexShader = compile vs_2_0 VS();
        PixelShader = compile ps_2_0 PS_1();
	}
	pass P1
	{
		VertexShader = compile vs_2_0 VS();
		PixelShader = compile ps_2_0 PS_2();
	}
}
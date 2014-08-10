

texture ColorMap;
sampler ColorMapSampler = sampler_state
{
   Texture = <ColorMap>;
   MinFilter = Point;
   MagFilter = Point;
   MipFilter = Point;
   AddressU = Clamp;
   AddressV = Clamp;
};

texture AccColorMap;
sampler AccColorMapSampler = sampler_state
{
   Texture = <AccColorMap>;
   MinFilter = Point;
   MagFilter = Point;
   MipFilter = Point;
   AddressU = Clamp;
   AddressV = Clamp;
};

float gBlurIntensity;

struct VS_OUT
{
    float4 Pos : POSITION;
    float2 Tex : TEXCOORD0;
};

VS_OUT VS(float4 pos : POSITION, float2 tex : TEXCOORD0)
{
    VS_OUT Out = (VS_OUT)0;
    Out.Pos = pos;
    Out.Tex = tex;
    if(tex.x > 0.5){
      Out.Tex.x = 1.0;
    }
    else{
      Out.Tex.x = 0.0;
    }
    if(tex.y > 0.5){
      Out.Tex.y = 1.0;
    }
    else{
      Out.Tex.y = 0.0;
    }
    

    return Out;
}

float4 PS(VS_OUT IN) : COLOR
{
    float4 color = tex2D(ColorMapSampler, IN.Tex);
    float4 accColor = tex2D(AccColorMapSampler, IN.Tex);
    return lerp(color, accColor, gBlurIntensity);
}

float4 PS_Final(VS_OUT IN) : COLOR
{
    return tex2D(ColorMapSampler, IN.Tex);
}

technique Main
{
	pass Blur
	{
		  VertexShader = compile vs_2_0 VS();
      ZEnable = false;
      ZWriteEnable = false;
      AlphaBlendEnable = false;
      CullMode = None;
      PixelShader = compile ps_2_0 PS();
	}
  pass Final
  {
      VertexShader = compile vs_2_0 VS();
      ZEnable = false;
      ZWriteEnable = false;
      AlphaBlendEnable = false;
      CullMode = None;
      PixelShader = compile ps_2_0 PS_Final();
  }
}
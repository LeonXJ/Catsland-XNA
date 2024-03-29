texture LightMap;
sampler LightMapSampler = sampler_state
{
   Texture = <LightMap>;
   MinFilter = Linear;
   MagFilter = Linear;
   MipFilter = Linear;   
   AddressU  = Clamp;
   AddressV  = Clamp;
};

texture PreColor;
sampler PreColorSampler = sampler_state
{
   Texture = <PreColor>;
   MinFilter = Linear;
   MagFilter = Linear;
   MipFilter = Linear;   
   AddressU  = Clamp;
   AddressV  = Clamp;
};


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
  float4 acc = tex2D(PreColorSampler, Tex);
  float4 light = tex2D(LightMapSampler, Tex);
  light = light * light.a;

  return acc + float4(light.rgb, 1.0);
}

technique Main
{
  pass P0
  {
      VertexShader = compile vs_2_0 VS();
      ZEnable = false;
      ZWriteEnable = false;
      AlphaBlendEnable = false;
      PixelShader = compile ps_2_0 PS();
  }
}
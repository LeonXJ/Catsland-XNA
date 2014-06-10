uniform float gHighlightThreshold;
uniform float2 gScreenSize;
uniform float gBlurWidth;
uniform float gSceneIntensity;
uniform float gGlowIntensity;
uniform float gHighlightIntensity;


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

texture BlurMap;
sampler BlurSampler = sampler_state
{
   Texture = <BlurMap>;
   MinFilter = Linear;
   MagFilter = Linear;
   MipFilter = Linear;
   AddressU  = Clamp;
   AddressV  = Clamp;
};


#define DOWNSAMPLE_SCALE 0.25
#define NSAMPLES 7

const half weights[NSAMPLES] = {
  0.05,
  0.1,
  0.2,
  0.3,
  0.2,
  0.1,
  0.05,
};

////////////////////////////////////
// Data Structure
////////////////////////////////////

struct DownsampleVertexOutput
{
  float4 Position    : POSITION;
  float2 TexCoord[4] : TEXCOORD0;
};

struct BlurVertexOutput
{
    float4 Position    : POSITION;
    float2 TexCoord[8] : TEXCOORD0;
};

struct FinalVertexOutput
{
    float4 Position  : POSITION;
    float2 TexCoord0 : TEXCOORD0;
    float2 TexCoord1 : TEXCOORD1;
};

///////////////////////////////////
// Vertex Shader
///////////////////////////////////

DownsampleVertexOutput VS_Downsample(float4 Position : POSITION,
                                  float2 TexCoord : TEXCOORD0,
                                  uniform float2 WindowSize)
{
    DownsampleVertexOutput OUT = (DownsampleVertexOutput)0;
    float2 texelSize = 1.0 / (WindowSize * DOWNSAMPLE_SCALE);
    float2 s = TexCoord;
    OUT.Position = Position;
    OUT.TexCoord[0] = s;
    OUT.TexCoord[1] = s + float2(2, 0) * texelSize;
    OUT.TexCoord[2] = s + float2(2, 2) * texelSize;
    OUT.TexCoord[3] = s + float2(0, 2) * texelSize;
    return OUT;
}

BlurVertexOutput VS_Blur(float4 Position : POSITION,
                                float2 TexCoord : TEXCOORD0,
                                uniform float2 direction,
                                uniform float BlurWidth,
                                uniform float2 WindowSize)
{
    BlurVertexOutput OUT = (BlurVertexOutput)0;
    OUT.Position = Position;
    float2 texelSize = BlurWidth / WindowSize;
    float2 s = TexCoord - texelSize*(NSAMPLES-1)*0.5*direction;
    for(int i=0; i<NSAMPLES; i++)
    {
        OUT.TexCoord[i] = s + texelSize*i*direction;
    }
    return OUT;
}

FinalVertexOutput VS_Final(float4 Position : POSITION,
                           float2 TexCoord : TEXCOORD0,
                           uniform float2 WindowSize)
{
    FinalVertexOutput OUT;
    float2 texelSize = 1.0 / WindowSize;
    OUT.Position = Position;
    OUT.TexCoord0 = TexCoord;
    OUT.TexCoord1 = TexCoord;
    return OUT;
}

///////////////////////////////////
// Pixel Shader
///////////////////////////////////

half luminance(half3 c)
{
  return dot(c, float3(0.3, 0.59, 0.11));
}

half highlights(half3 c, uniform float HighlightThreshold)
{
  return smoothstep(HighlightThreshold, 1.0, luminance(c.rgb));
}

half4 PS_Downsample(DownsampleVertexOutput IN,
                    uniform sampler2D OrigSampler,
                    uniform float HighlightThreshold)
                    : COLOR
{
    half4 c;
    c = tex2D(OrigSampler, IN.TexCoord[0]) * 0.25;
    c += tex2D(OrigSampler, IN.TexCoord[1]) * 0.25;
    c += tex2D(OrigSampler, IN.TexCoord[2]) * 0.25;
    c += tex2D(OrigSampler, IN.TexCoord[3]) * 0.25;
    c.a = highlights(c.rgb, HighlightThreshold);
    return c;
}

half4 PS_Blur(BlurVertexOutput IN,
              uniform sampler2D OrigSampler,
              uniform half weight[7])
              : COLOR
{
    half4 c = 0;
    for(int i=0; i<NSAMPLES; i++){
        c += tex2D(OrigSampler, IN.TexCoord[i]) * weight[i];
    }
    return c;
}

half4 PS_Final(FinalVertexOutput IN,
               uniform sampler2D OrigSampler,
               uniform sampler2D BlurredSampler,
               uniform float SceneIntensity,
               uniform float GlowIntensity,
               uniform float HighlightIntensity)
               : COLOR
{
    half4 orig = tex2D(OrigSampler, IN.TexCoord0);
    half4 blur = tex2D(BlurredSampler, IN.TexCoord1);
    return SceneIntensity*orig + 
           GlowIntensity*blur +
           HighlightIntensity*blur.a;
}

technique Main
{
    pass Downsample
    {
        VertexShader = compile vs_3_0 VS_Downsample(gScreenSize);
        ZEnable = false;
        ZWriteEnable = false;
        AlphaBlendEnable = false;
        CullMode = None;
        PixelShader = compile ps_3_0 PS_Downsample(ColorMapSampler,
                                                  gHighlightThreshold);
    }

    pass GlowV
    {
        VertexShader = compile vs_3_0 VS_Blur(float2(0,1), gBlurWidth, gScreenSize);
        ZEnable = false;
        ZWriteEnable = false;
        AlphaBlendEnable = false;
        CullMode = None;
        PixelShader = compile ps_3_0 PS_Blur(BlurSampler, weights);
    }

    pass GlowH
    {
        VertexShader = compile vs_3_0 VS_Blur(float2(1,0), gBlurWidth, gScreenSize);
        ZEnable = false;
        ZWriteEnable = false;
        AlphaBlendEnable = false;
        CullMode = None;
        PixelShader = compile ps_3_0 PS_Blur(BlurSampler, weights);
    }

    pass Final
    {
        VertexShader = compile vs_3_0 VS_Final(gScreenSize);
        ZEnable = false;
        ZWriteEnable = false;
        AlphaBlendEnable = false;
        CullMode = None;
        PixelShader = compile ps_3_0 PS_Final(ColorMapSampler,
                                              BlurSampler,
                                              gSceneIntensity,
                                              gGlowIntensity,
                                              gHighlightIntensity);
    }
}

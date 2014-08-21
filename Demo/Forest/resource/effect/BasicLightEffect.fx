float4x4 World;
float4x4 View;
float4x4 Projection;
float Alpha;
float LightInstance;
float4 BiasColor;
texture DiffuseMap;
texture LightMap;
sampler ColorMapSampler = sampler_state
{
   Texture = <DiffuseMap>;
   MinFilter = Linear;
   MagFilter = Linear;
   MipFilter = Linear;   
   AddressU  = Clamp;
   AddressV  = Clamp;
};
sampler LightMapSampler = sampler_state
{
    Texture = <LightMap>;
    MinFilter = Linear;
    MagFilter = Linear;
    MipFilter = Linear;
    AddressU  = Clamp;
    AddressV  = Clamp;
};

// TODO: add effect parameters here.

struct VertexShaderInput
{
    float4 Position : POSITION0;
    float2 UV : TEXCOORD0;
    // TODO: add input channels such as texture
    // coordinates and vertex colors here.
};

struct VertexShaderOutput
{
    float4 Position : POSITION0;
    float2 UV : TEXCOORD0;
    float2 PositionInView : TEXCOORD1;

    // TODO: add vertex shader outputs such as colors and texture
    // coordinates here. These values will automatically be interpolated
    // over the triangle, and provided as input to your pixel shader.
};

VertexShaderOutput VertexShaderFunction(VertexShaderInput input)
{
    VertexShaderOutput output;

    float4 worldPosition = mul(input.Position, World);
    float4 viewPosition = mul(worldPosition, View);
    output.Position = mul(viewPosition, Projection);

    // TODO: add your vertex shader code here.
    output.UV = input.UV;
    output.PositionInView = output.Position.xy;
    return output;
}

float4 PixelShaderFunction(VertexShaderOutput input) : COLOR0
{
    // TODO: add your pixel shader code here.
    float2 screenUV = float2(input.PositionInView.x * 0.5 + 0.5, -input.PositionInView.y*0.5 + 0.5);
    float4 light = tex2D(LightMapSampler, screenUV);
    light.rgb = light.rgb*light.a;
    float4 originalColor = tex2D(ColorMapSampler, input.UV) * float4(1.0, 1.0, 1.0, Alpha) + BiasColor;
    originalColor = clamp(originalColor, 0.0, 1.0);
    return float4(originalColor.rgb * light.rgb * LightInstance + (1.0 - LightInstance) * originalColor.rgb, originalColor.a);
}

technique Technique1
{
    pass Pass1
    {
        VertexShader = compile vs_2_0 VertexShaderFunction();
        AlphaBlendEnable = true;
        SrcBlend = SrcAlpha;
        DestBlend = InvSrcAlpha;
        CullMode = None;
        ZEnable = false;
        ZWriteEnable = false;
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}

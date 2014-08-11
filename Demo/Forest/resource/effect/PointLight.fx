float4x4 World;
float4x4 View;
float4x4 Projection;

float3 LightPositionInWorld;
float4 DiffuseColor;
float OutRadius;
float InRadius;

struct VertexShaderInput
{
    float4 Position : POSITION0;
};

struct VertexShaderOutput
{
    float4 Position : POSITION;
    float4 PositionInWorld : TEXCOORD;
};

VertexShaderOutput VertexShaderFunction(VertexShaderInput input)
{
    VertexShaderOutput output;

    float4 worldPosition = mul(input.Position, World);
    float4 viewPosition = mul(worldPosition, View);
    output.Position = mul(viewPosition, Projection);
    output.PositionInWorld = worldPosition;

    return output;
}

float4 PixelShaderFunction(VertexShaderOutput input) : COLOR0
{
    float3 delta = input.PositionInWorld.xyz - LightPositionInWorld;
    float sd = delta.x * delta.x + delta.y * delta.y;
    float intensity = DiffuseColor.a;
    if(sd > OutRadius * OutRadius){
        intensity = 0.0;
    }
    else if(sd > InRadius * InRadius){
        intensity *= (OutRadius * OutRadius - sd) / (OutRadius * OutRadius - InRadius * InRadius);
    }

    return float4(DiffuseColor.rgb * intensity, 1.0);
}

technique Main
{
    pass P0
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

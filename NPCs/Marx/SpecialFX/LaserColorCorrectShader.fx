sampler uImage0 : register(s0);
float uTime;
float2 uScreenPosition;
float2 uScreenResolution;
float uIntensity;
float4 uColor;
float4 uSecondaryColor;
float uProgress;
float2 uTargetPosition;
float2 uDirection;

float4 SplitFX(float4 color : COLOR0, float2 coords : TEXCOORD0) : COLOR0
{
    float4 col = tex2D(uImage0, coords);

    float luminance = dot(col.rgb, float3(0.299, 0.587, 0.114));

    col.rgb = lerp(float3(luminance, luminance, luminance), col.rgb, uIntensity);
    col.rgb *= uProgress;
    return col;
}



technique Technique1
{
    pass SplitPass
    {
        PixelShader = compile ps_3_0 SplitFX();
    }
}
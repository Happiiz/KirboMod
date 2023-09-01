sampler uImage0 : register(s0);
texture maskTexture;
sampler uImage1 = sampler_state
{
    Texture = (maskTexture);
    AddressU = wrap;
    AddressV = wrap;
};
float uOpacity;
float time;
float s;
float l;
float gradientScale;
float invLerp(float a, float b, float t)
{
    return (t - a) / (b - a);
}
float remap(float inMin, float inMax, float outMin, float outMax, float inValue)
{
    return lerp(outMin, outMax, invLerp(inMin, inMax, inValue));
}

float hue2rgb(float c, float t1, float t2, float cAgain)//dumb fix lmao
{
    if (c < 0.0)
    {
        c += 1.0;
    }
    if (c > 1.0)
    {
        c -= 1.0;
    }
    if (6.0 * c < 1.0)
    {
        return t1 + (t2 - t1) * 6.0 * c;
    }
    if (2.0 * c < 1.0)
    {
        return t2;
    }
    if (3.0 * c < 2.0)
    {
        return t1 + (t2 - t1) * (2.0 / 3.0 - c) * 6.0;
    }
    return t1;
}
//yes this is decopiled code
float4 hsl2RGBAAttempt2(float sat, float light, float opacity, float hueOffset)
{
    float r;
    float g;
    float b;
    //if (sat == 0)
    //{
    //    return float4(light, light, light, light);
    //}
    float hue = hueOffset * gradientScale;
    hue %= 1.0;
    float num2 = light * (1.0 + sat);
    if (l >= 0.5)
        num2 = light + s - light * sat;
    float t = 2.0 * light - num2;
    float c = hue + 0.33333;
    float c2 = hue;
    float c3 = hue - 0.33333;
    c = hue2rgb(c, t, num2, c);
    c2 = hue2rgb(c2, t, num2, c2);
    float num3 = hue2rgb(c3, t, num2, c3);
    return float4(c, c2, num3, 1);
}


float calcBrightnessForTopRightCut(float2 coords)
{
    coords.y = 1-coords.y;
    coords *= 12;
    coords -= 10.5;
    coords = clamp(coords, 0, 1);
    float result = max(coords.x, coords.y);
    result = clamp(result, 0, 1);
    return 1 - result;
}
float calcDiagonalStrip(float2 coords)
{
    float result = distance(coords.x, 1 - coords.y);
    result = 1 - result;
    result = remap(0, 1, -8, 1, result);
    return clamp(result, 0, 1);
    
}
float calcHueOffset(float2 coords, float bladeShape)//blade shape before multiply by 2!
{
    float result = coords.x;
    if(coords.x < 1-coords.y)
        result = 1-coords.y;
    result += time * 0.2f;
    result *= 12.0;
    result = floor(result);
    result /= 12.0;
    result %= 1.0;
    return result +bladeShape / 4;

}

float4 swordGradient(float4 sampleColor : COLOR0, float2 coords : TEXCOORD0) : COLOR0
{
    float4 curCol = tex2D(uImage0, coords);
    if (curCol.a == 0)
        return curCol;
    if ((curCol.r == 1 && curCol.g == 1 && curCol.b == 1)||(curCol.r == 0 && curCol.g == 0 && curCol.b == 0))
    {
    
        float opacity = curCol.a * uOpacity;
        coords *= 32;
        coords.y += 1;
        coords = floor(coords);
        coords /= 32;
        
        float diagonalStrip = calcDiagonalStrip(coords);

        float brightnessForTopRightCut = calcBrightnessForTopRightCut(coords);

        float bladeShape = min(diagonalStrip, brightnessForTopRightCut);

        float hueOffset = calcHueOffset(coords, bladeShape);
        bladeShape *= 2;
        bladeShape = clamp(bladeShape, 0, 1);
        float4 rgba = hsl2RGBAAttempt2(s, 0.5f, opacity, hueOffset);
        rgba += float4(bladeShape, bladeShape, bladeShape, 1);
        return rgba;
    }
    return curCol;
}

technique Technique1
{
    pass HslScrollPass
    {
        PixelShader = compile ps_3_0 swordGradient();
    }
}
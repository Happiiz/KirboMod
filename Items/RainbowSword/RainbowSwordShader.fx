sampler uImage0 : register(s0);
float uOpacity;
float h;
float s;
float l;
float gradientScale;
float4 TestGradient(float4 sampleColor : COLOR0, float2 coords : TEXCOORD0) : COLOR0
{
    //w is opacity
    float4 curCol = tex2D(uImage0, coords);
    if ((curCol.r == 1.0 && curCol.g == 1.0 && curCol.b == 1.0) || (curCol.r == 0.0 && curCol.g == 0.0 && curCol.b == 0.0))
    {
        float w = curCol.a * uOpacity;
        coords *= 32;
        coords = floor(coords);
        coords /= 32;
        float2 centeredCoords = float2(coords.x - 0.5, coords.y - 0.5);
        float hueOffset = (centeredCoords.y - centeredCoords.x) * gradientScale;
        //h is the base hue
        float h2 = (h + hueOffset * gradientScale) % 6.0;
        h2 += 0.5;
        h2 = floor(h2);
        float l2 = (2.0 * l - 1.0); 
        if ((curCol.r == 0.0 && curCol.g == 0.0 && curCol.b == 0.0))
            l2 = (2.0 * (l * 0.5f) - 1.0);//for outline
        float l3 = sqrt(l2 * l2); //abs function not liking global variables
        float c = (1 - l3) * s; //c means chroma, hsl to rgb math
        float x = c * (1 - abs(h2 % 2.0 - 1)); //hsl to rgb math
        //l is lightness, add lightness depending on distance to center
        float m = (l) - c * 0.5; //hsl to rgb math
        x += m; //hsl to rgb math
        c += m; //hsl to rgb math
        if (h2 < 1)
            return float4(c, x, m, 1) * w;
        if (h2 < 2)
            return float4(x, c, m, 1) * w;
        if (h2 < 3)
            return float4(m, c, x, 1) * w;
        if (h2 < 4)
            return float4(m, x, c, 1) * w;
        if (h2 < 5)
            return float4(x, m, c, 1) * w;
        return float4(c, m, x, 1) * w;
    }
    return curCol;
}

technique Technique1
{
    pass HslScrollPass
    {
        PixelShader = compile ps_3_0 TestGradient();
    }
}
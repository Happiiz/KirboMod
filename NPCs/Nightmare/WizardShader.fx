sampler uImage0 : register(s0);
texture bgTexture;
texture capeTexture;
texture perlinTexture;
texture randTexture;
texture palette1Texture;
texture palette2Texture;
texture bodyMapTexture;
float2 frameSize;
float time;
float deathCounterNormalized;
sampler bg = sampler_state
{
    Texture = (bgTexture);
    AddressU = wrap;
    AddressV = wrap;
    Filter = point;
};
sampler cape = sampler_state
{
    Texture = (capeTexture);
    AddressU = wrap;
    AddressV = wrap;
    Filter = point;
};
sampler perlin = sampler_state
{
    Texture = (perlinTexture);
    AddressU = wrap;
    AddressV = wrap;
    Filter = point;
};
sampler rand = sampler_state
{
    Texture = (randTexture);
    AddressU = wrap;
    AddressV = wrap;
    Filter = point;
};
sampler palette1 = sampler_state
{
    Texture = (palette1Texture);
    AddressU = wrap;
    AddressV = wrap;
    Filter = point;
};
sampler palette2 = sampler_state
{
    Texture = (palette2Texture);
    AddressU = wrap;
    AddressV = wrap;
    Filter = point;
};
sampler bodyMap = sampler_state
{
    Texture = (bodyMapTexture);
    AddressU = wrap;
    AddressV = wrap;
    Filter = point;
};

float4 random(float2 st)
{
    return tex2D(rand, st);
}
float4 getCapeColorSmoothed(float t)
{
    float t2 = t - 0.14285714285;
    return float4(1, 1, 1, 1);
}
float4 deathFade(float2 origCoords, float4 currentCol)
{
    origCoords.y *= 25; //frame count
    origCoords.y /= 40;
    origCoords.x /= 2;
    if (currentCol.a < 0.02)//if transparent or near transparent OR white or close to white
    {
        return 0;
    }
    if (deathCounterNormalized < 0.001)
    {
        return currentCol;
    }
    if (distance(float3(1, 1, 1), currentCol.rgb) < 0.01)
    {
        return currentCol;
    }
    float2 scrollingCoords = origCoords;
    float time2 = time * 0.003;
    scrollingCoords.x += time2;
    float noiseSample = tex2D(perlin, scrollingCoords).b;
    scrollingCoords = origCoords;
    scrollingCoords.x -= time2;
    float noiseSample2 = tex2D(perlin, scrollingCoords).a;
    noiseSample /= max(0.001, noiseSample2);
    noiseSample %= 1;
    float cutoffValue = noiseSample - deathCounterNormalized;
    if (cutoffValue < 0)
    {
        return 0;
    }
    if (cutoffValue < 0.1)
    {
        return lerp(float4(1, 0, 1, 0), currentCol, cutoffValue/0.1);
    }
    return currentCol;

}
float4 getCapeColor(float t)
{
    if (t < 0.14285714285)
    {
        return float4(5 / 255, 0, 208 / 255, 0);
    }
    if (t < 0.14285714285 * 2)
    {
        return float4(4 / 255, 0, 161 / 255, 0);
    }
    if (t < 0.14285714285 * 3)
    {
        return float4(3 / 255, 0, 120 / 255, 0);
    }
    if (t < 0.14285714285 * 4)
    {
        return float4(0, 9/255, 84 / 255, 0);
    }
    if (t < 0.14285714285 * 5)
    {
        return float4(45 / 255, 0, 120 / 255, 0);
    }
    if (t < 0.14285714285 * 6)
    {
        return float4(89 / 255, 0, 161 / 255, 0);
    }
    return float4(179 / 255, 0, 208 / 255, 0);

}
float4 wizardShader(float4 color : COLOR0, float2 coords : TEXCOORD0) : COLOR0
{
    //pixelate
    //coords.x *= frameSize.x;
    //coords.y *= frameSize.y;
    //coords = ceil(coords);
    //coords.x /= frameSize.x;
    //coords.y /= frameSize.y; 
    //end of pixelation
    float4 currentCol = tex2D(uImage0, coords);
    
    if (currentCol.g == 1 && currentCol.b == 1 && currentCol.r == 0)
    {
        float2 origCoords = coords;
        coords.x *= frameSize.x / 2;
        coords.y *= frameSize.y / 2;
        coords.y *= 25;
        coords = ceil(coords);
        coords.x /= frameSize.x / 2;
        coords.y /= frameSize.y / 2;
        float4 randWithTime = random(coords.xy + 2 + time * 0.1);
        float body = tex2D(bodyMap, coords);
        if (body > .5)
        {
            return deathFade(origCoords, tex2D(palette1, float2(time / 15, 0))); //15 is length of palette
        }
        return deathFade(origCoords, tex2D(palette2, float2(time / 4, 0))); //4 is length of palette
    }
    else if (currentCol.r == 0 && currentCol.g == 0 && currentCol.b == 0 && currentCol.a == 1)
    {
        float2 origCoords = coords;
        coords.x += time * 0.02;
        coords.y *= 25; //frame count
        coords.y += .6;
      
       
        float4 bgColor = tex2D(cape, coords);
        if (length(bgColor.rgb) > 0.05)//if not black (or near black)
        {
            bgColor.xyz *= 0.6;
            return deathFade(origCoords, bgColor);
        }
        coords = origCoords;
        coords.y *= 20;
        coords.x /= 20;
        float2 scrollingCoords = coords;
        float time2 = time * 0.003;
        scrollingCoords.x += time2;
        float noiseSample = tex2D(perlin, scrollingCoords).r;
        scrollingCoords = coords;
        scrollingCoords.x -= time2;
        float noiseSample2 = tex2D(perlin, scrollingCoords).g;
        noiseSample /= max(0.001, noiseSample2);
        noiseSample %= 1;
        //noiseSample = sqrt(noiseSample);
        //noiseSample = abs(frac((noiseSample - 1) / (2)) * 2.0 - 1);
        //float4 purple = float4(89.0 / 255.0, 0, 103.0 / 255.0, 1); // float4(89.0 / 255.0, 0, 103.0 / 255.0, 0);
        //float4 brightBlue = float4(0.0, 0.0, 0.6, 1.0);
        float4 blue = float4(0, 16.0 / 255.0, 103.0 / 255.0, 1); //float4(0, 16.0/255.0, 103.0 / 255.0, 0);
        float4 black = 0;
      
        //noiseSample = noiseSample * noiseSample * noiseSample;
        float4 finalCol =  lerp(black, blue, noiseSample);
        finalCol.xyz *= 0.6;
        return deathFade(origCoords, finalCol);
        //coords.x += time;
        //coords.y *= 25;//frame count
        //float4 bgColor = tex2D(cape, coords);
        //bgColor.rgb *= .5;
        //if (bgColor.r != 0 || bgColor.g != 0 || bgColor.b != 0)
        //    return bgColor;
    }
    else if (currentCol.r == 0.50196078431 && currentCol.g == 0.50196078431 && currentCol.b == 0.50196078431 && currentCol.a == 1)
    {
        float2 origCoords = coords;
        coords.x -= time * 0.02;
        coords.y *= 25; //frame count
        coords.y += .6;
      
       
        float4 bgColor = tex2D(cape, coords);
        if (length(bgColor.rgb) > 0.05)//if not black (or near black)
        {
            return bgColor;
        }
        coords = origCoords;
        coords.y *= 20;
        coords.x /= 20;
        float2 scrollingCoords = coords;
        float time2 = time * 0.003;
        scrollingCoords.x += time2;
        float noiseSample = tex2D(perlin, scrollingCoords).r;
        scrollingCoords = coords;
        scrollingCoords.x -= time2;
        float noiseSample2 = tex2D(perlin, scrollingCoords).g;
        noiseSample /= max(0.001, noiseSample2);
        noiseSample %= 1;
        //noiseSample = sqrt(noiseSample);
        //noiseSample = abs(frac((noiseSample - 1) / (2)) * 2.0 - 1);
        float4 purple = float4(89.0/255.0, 0, 103.0/255.0, 1); // float4(89.0 / 255.0, 0, 103.0 / 255.0, 0);
        float4 brightBlue = float4(0.0, 0.0, 0.6, 1.0);
        float4 blue = float4(0, 16.0/255.0, 103.0/255.0, 1); //float4(0, 16.0/255.0, 103.0 / 255.0, 0);
        float4 black = float4(0, 0, 0, 1);
        //noiseSample = noiseSample * noiseSample * noiseSample;
        return deathFade(origCoords, lerp(black, blue, noiseSample));
    }
    return deathFade(coords, currentCol);
}

technique Technique1
{
    pass WizardPass
    {
        PixelShader = compile ps_3_0 wizardShader();
    }
}
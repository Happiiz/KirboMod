sampler uImage0 : register(s0);
texture bgTexture;
texture capeTexture;
float2 frameSize;
float time;

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
float random(float2 st)//copied this random function from somewhere else lol
{
    return frac(sin(dot(st.xy,
                         float2(12.9898, 78.233))) *
        43758.5453123);
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
        float2 glitchCoords = coords + time * 2;
        coords.x *= frameSize.x / 2;
        coords.y *= frameSize.y / 2;
        coords.y *= 25;
        coords = ceil(coords);
        coords.x /= frameSize.x / 2;
        coords.y /= frameSize.y / 2;
        float rand = random(coords.xy + 2 + time);
        if (rand < .125)
        {
           return float4(.03, 0.47, 1, 1);
        }
        else if (rand < .25)
        {
            return float4(0.9, 0.06, .03, 1);
        }
        else if(rand < .625)
        {
            return float4(0.3, 0.3, 0.4, 1);
        }
        else
        {
            return float4(0.5, 0.5, 0.4, 1);

        }
    }
    else if (currentCol.r == 0 && currentCol.g == 0 && currentCol.b == 0 && currentCol.a == 1)
    {
        coords.x += time;
        coords.y *= 25;//frame count
        float4 bgColor = tex2D(cape, coords);
        bgColor.rgb *= .5;
        if (bgColor.r != 0 || bgColor.g != 0 || bgColor.b != 0)
            return bgColor;
    }
    else if (currentCol.r == 0.50196078431 && currentCol.g == 0.50196078431 && currentCol.b == 0.50196078431 && currentCol.a == 1)
    {
        coords.x -= time;
        coords.y *= 25; //frame count
        coords.y += .6;
        float4 bgColor = tex2D(cape, coords);
        //if (bgColor.r != 0 || bgColor.g != 0 || bgColor.b != 0)
            return bgColor;
    }
    return currentCol;
}

technique Technique1
{
    pass WizardPass
    {
        PixelShader = compile ps_3_0 wizardShader();
    }
}
sampler uImage0 : register(s0); 
texture bgTexture;
float rotationAmount;

sampler bg = sampler_state
{
    Texture = (bgTexture);
    AddressU = wrap;
    AddressV = wrap;
    Filter = point;
};

float easeOutInSin(float progress01)
{
    return 0.5 - (asin(1 - 2 * progress01)) / (3.1415);
}
float4 wingShader(float4 color : COLOR0, float2 coords : TEXCOORD0) : COLOR0
{
    //pixelate
    coords *= 50;
    coords = ceil(coords);
    coords /= 50;
    //end of pixelation
    
    float sphereRadius = 0.9;//regular value: 0.9
    float2 center = 0.5;
    float2 originalCoords = coords;
    coords.x -= center.x;//make the center the origin
    float absCoordY = abs(1 - originalCoords.y * 2);
    float chordWidth = sqrt(sphereRadius - absCoordY * absCoordY);
    coords.x /= chordWidth;
    coords.x += center.x;//unmake center the origin
    coords.x = easeOutInSin(coords.x);//easing expects in 0 to 1 range
    float4 currentColor = tex2D(uImage0, originalCoords);
    float dist = 1.6 - distance(originalCoords, 0.5f) * 2; //for shading
    if (currentColor.b != 1 || currentColor.r == 1)//check if pixel is outside the mask
    {
        currentColor.rgb *= dist;
       return currentColor;//pixels outside the sphere get discarded
    }        
    coords.x += rotationAmount;//scrolling
    
    float4 result = tex2D(bg, coords);
    
    result.rgb *= dist;

    return result;
}

technique Technique1
{
    pass WingsPass
    {
        PixelShader = compile ps_3_0 wingShader();
    }
}
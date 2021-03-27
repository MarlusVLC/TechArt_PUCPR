using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ApplyFilter : MonoBehaviour
{
    [SerializeField][Range(0,1)] private float binaryThreshold;
    [SerializeField] [Range(0, 1)] private float brightnessScale;
    [SerializeField] [Range(0, 1)] private float blendingScale;
    [SerializeField] private Texture2D secondTexture;
    [SerializeField] private Filters filter;
    
    private Renderer _render;
    private Texture2D _texture;
    private Texture2D _originalFirstTexture;
    private Color[] newPixels;

    private enum Filters
    {
        GrayScale_Average,
        GrayScale_Luminance,
        Binary,
        Negate,
        Brightness,
        Subtract,
        Sum,
        Blend
    };

    private void Awake()
    {
        _render = GetComponent<Renderer>();
        _texture = Instantiate(_render.material.mainTexture) as Texture2D;
        _originalFirstTexture = Instantiate(_render.material.mainTexture) as Texture2D;
        _render.material.mainTexture = _texture;
        
    }

    void Start()
    {

        
        
        // int mipCount = Mathf.Min(3, _texture.mipmapCount);
        int mipCount = _texture.mipmapCount;
        
        for (int mip=0; mip< mipCount; mip++)
        {
            Color[] pixels = _texture.GetPixels(mip);
            if (secondTexture != null)
            {
                newPixels = secondTexture.GetPixels(mip);
            }
            for (int p = 0; p < pixels.Length; p++)
            {
                switch (filter)
                {
                    case Filters.GrayScale_Average:
                        pixels[p] = GrayscaleAverage(pixels[p]);
                        break;
                    case Filters.GrayScale_Luminance:
                        pixels[p] = GrayscaleLuminance(pixels[p]);
                        break;
                    case Filters.Binary:
                        pixels[p] = Binary(pixels[p], binaryThreshold);
                        break;
                    case Filters.Negate:
                        pixels[p] = Negate(pixels[p]);
                        break;
                    case Filters.Brightness:
                        pixels[p] = Brightness(pixels[p], brightnessScale);
                        break;
                    case Filters.Subtract:
                        pixels[p] = Subtract(pixels[p], newPixels[p]);
                        break;
                    case Filters.Sum:
                        pixels[p] = Sum(pixels[p], newPixels[p]);
                        break;
                    case Filters.Blend:
                        pixels[p] = Blend(pixels[p], newPixels[p], blendingScale);
                        break;
                }

            }
            _texture.SetPixels(pixels, mip);
            newPixels = null;
        }
        _texture.Apply(false);
    }

    private void OnValidate()
    {
        if (Application.isPlaying)
        { 
            
            
            // int mipCount = Mathf.Min(3, _texture.mipmapCount);
            int mipCount = _texture.mipmapCount;
            
            for (int mip=0; mip< mipCount; mip++)
            {
                Color[] originalPixels = _originalFirstTexture.GetPixels(mip);
                Color[] pixels = _texture.GetPixels(mip);
                // if (secondTexture != null)
                // {
                    newPixels = secondTexture.GetPixels(mip);
                // }
                for (int p = 0; p < pixels.Length; p++)
                {
                    switch (filter)
                    {
                        case Filters.GrayScale_Average:
                            pixels[p] = GrayscaleAverage(originalPixels[p]);
                            break;
                        case Filters.GrayScale_Luminance:
                            pixels[p] = GrayscaleLuminance(originalPixels[p]);
                            break;
                        case Filters.Binary:
                            pixels[p] = Binary(originalPixels[p], binaryThreshold);
                            break;
                        case Filters.Negate:
                            pixels[p] = Negate(originalPixels[p]);
                            break;
                        case Filters.Brightness:
                            pixels[p] = Brightness(originalPixels[p], brightnessScale);
                            break;
                        case Filters.Subtract:
                            pixels[p] = Subtract(originalPixels[p], newPixels[p]);
                            break;
                        case Filters.Sum:
                            pixels[p] = Sum(originalPixels[p], newPixels[p]);
                            break;
                        case Filters.Blend:
                            pixels[p] = Blend(originalPixels[p], newPixels[p], blendingScale);
                            break;
                    }
                }
                _texture.SetPixels(pixels, mip);
                newPixels = null;
            }
            _texture.Apply(false);
        }
        
    }
    
    

    private Color GrayscaleAverage(Color pixel)
    {
        float color = (pixel.r + pixel.g + pixel.b) / 3.0f;
        return new Color(color, color, color, pixel.a); 
    }

    private Color GrayscaleLuminance(Color pixel)
    {
        float color = (pixel.r*0.299f + pixel.g*0.587f + pixel.b*0.114f);
        return new Color(color, color, color, pixel.a);
    }

    private Color Binary(Color pixel, float channelScale)
    {
        pixel.r = pixel.r > channelScale ? 1 : 0;
        pixel.g = pixel.g > channelScale ? 1 : 0;
        pixel.b = pixel.g > channelScale ? 1 : 0;
        return pixel;
    }

    private Color Negate(Color pixel)
    {
        pixel.r = 1 - pixel.r;
        pixel.g = 1 - pixel.g;
        pixel.b = 1 - pixel.b;
        return pixel;
    }

    private Color Brightness(Color pixel, float _brightnessScale)
    {
        pixel.r *= _brightnessScale;
        pixel.g *= _brightnessScale;
        pixel.b *= _brightnessScale;
        return pixel;
    }

    private Color Subtract(Color subtrahend, Color minuend)
    {
        subtrahend.r -= minuend.r;
        subtrahend.g -= minuend.g;
        subtrahend.b -= minuend.b;
        return subtrahend;
    }
    
    private Color Sum(Color pixelA, Color pixelB)
    {
        pixelA.r += pixelB.r;
        pixelA.g += pixelB.g;
        pixelA.b += pixelB.b;
        return pixelA;
    }

    private Color Blend(Color pixelA, Color pixelB, float alpha)
    {
        pixelA.r = (1 - alpha) * pixelA.r + alpha * pixelB.r;
        pixelA.g = (1 - alpha) * pixelA.g + alpha * pixelB.g;
        pixelA.b = (1 - alpha) * pixelA.b + alpha * pixelB.b;
        return pixelA;
    }
    
    
    
}

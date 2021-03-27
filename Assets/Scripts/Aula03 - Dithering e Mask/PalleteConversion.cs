using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using DefaultNamespace;
using UnityEngine;

public class PalleteConversion : MonoBehaviour
{

    enum ColorPallete
    {
        Summer,
        Neon,
        Custom
    }
    
    [Tooltip("Essa sera a imagem usada na conversão")]
    [SerializeField] private Texture2D texture;
    
    [Tooltip("se só useDithering estiver ativado, ele executará o dithering, mas sem conversão de cores")]
    [SerializeField] private bool useDithering;
    
    [Tooltip("Ativa a conversão de cores com o Dithering")]
    [SerializeField] private bool convertColorOnDithering;
    
    [Tooltip("Determina a quantidade de cores disponíveis ao dithering -> (quantizingThreshold)³. " +
             "Só funciona se useDithering estiver ativado")]
    [SerializeField] private float quantizingThreshold ;

    [SerializeField] private ColorPallete _colorPallete;
    
    [Tooltip("Paleta de cor para qual a imagem será convertida." +
             "Só funciona se useDithering estiver desativado ou ativado com convertColorOnDithering")]
    [SerializeField] private Color[] customColorPalette;



    // private Color[] colorPallete;
    //
    // private Color[] SummerPallete
    // {
    //     
    // }
    //
    
    private Renderer _render;

    
    // Start is called before the first frame update
    void Awake()
    {
        if(_render == null)
            _render = GetComponent<Renderer>();
        
        
        
        if (useDithering)
        {
            ApplyDithering(quantizingThreshold, customColorPalette, convertColorOnDithering );
            
        }
        else
        {
            PalleteConvert(customColorPalette);
        }
    }
    
    void OnValidate(){
        if(!Application.isPlaying)
            return;

        if(_render == null)
            _render = GetComponent<Renderer>();

        if (useDithering)
        {
            ApplyDithering(quantizingThreshold, customColorPalette, convertColorOnDithering );
            
        }
        else
        {
            PalleteConvert(customColorPalette);
        }


    }




    void Binarize(float parts)
    {
        Texture2D texture = Instantiate(this.texture);
        _render.material.mainTexture = texture;
        
        Color[] pixels = texture.GetPixels(0);
        for (int x = 0; x < texture.width; x++)
        {
            for (int y = 0; y < texture.height; y++)
            {
                int index = x + y * this.texture.width;
                Color pixel = pixels[index];
    
                float r = Mathf.Round(parts*pixel.r) * 1/parts;
                float g = Mathf.Round(parts*pixel.g) * 1/parts;
                float b = Mathf.Round(parts*pixel.b) * 1/parts;
    
                pixels[index] = new Color(r, g, b);
            }
        }
        texture.SetPixels(pixels, 0);
        texture.Apply();
        Debug.Log("Dithring done");
    }


    void PalleteConvert(Color[] palette)
    {
        Texture2D texture = Instantiate(this.texture);
        _render.material.mainTexture = texture;

        Color[] pixels = texture.GetPixels(0);
        for (int y = 0; y < texture.height - 1; y++)
        {
            for (int x = 1; x < texture.width - 1; x++)
            {
                pixels[index(x, y, texture)].FindClosestPaletteColor(palette);
            }
        }
        texture.SetPixels(pixels, 0);
        texture.Apply();
        Debug.Log("Dithring done");
    }
    
    
    
    void ApplyDithering(float parts, Color[] palette = null, bool ColorSwap = false)
    {
        Texture2D texture = Instantiate(this.texture);
        _render.material.mainTexture = texture;
    
        Color[] pixels = texture.GetPixels(0);
        for (int y = 0; y < texture.height-1; y++)
        {
            for (int x = 1; x < texture.width-1; x++)
            {
                Color oldPixel = pixels[index(x, y, texture)];
                
                Color pixel = ColorSwap ? findClosestPaletteColor(palette,pixels[index(x,y,this.texture)])
                        : pixels[index(x,y,this.texture)];
                
                
    
                float r = Mathf.Round(parts*pixel.r) * 1/parts;
                float g = Mathf.Round(parts*pixel.g) * 1/parts;
                float b = Mathf.Round(parts*pixel.b) * 1/parts;
    
                pixels[index(x,y,this.texture)] = new Color(r, g, b);
                
                float errR = oldPixel.r - r;
                float errG = oldPixel.g - g;
                float errB = oldPixel.b - b;
    
    
    
                int index_ = index(x + 1, y, texture);
                Color currColor = pixels[index_];
                currColor.r += errR * 7 / 16.0f;
                currColor.g += errG * 7 / 16.0f;
                currColor.b += errB * 7 / 16.0f;
                pixels[index_] = currColor;
                
                
                index_ = index(x - 1, y + 1, texture);
                currColor = pixels[index_];
                currColor.r += errR * 3 / 16.0f;
                currColor.g += errG * 3 / 16.0f;
                currColor.b += errB * 3 / 16.0f;
                pixels[index_] = currColor;
                
                
                
                index_ = index(x, y + 1, texture);
                currColor = pixels[index_];
                currColor.r += errR * 5 / 16.0f;
                currColor.g += errG * 5 / 16.0f;
                currColor.b += errB * 5 / 16.0f;
                pixels[index_] = currColor;
                
                
                index_ = index(x + 1, y + 1, texture);
                currColor = pixels[index_];
                currColor.r += errR * 1 / 16.0f;
                currColor.g += errG * 1 / 16.0f;
                currColor.b += errB * 1 / 16.0f;
                pixels[index_] = currColor; 
                
            }
        }
        texture.SetPixels(pixels, 0);
        texture.Apply();
        Debug.Log("Dithring done");
    }
    
    
    

    
    
    private int index(int x, int y, Texture texture)
    {
        return x + y * texture.width;
    }
    
    
    

    private Color findClosestPaletteColor(Color[] palette, Color pixel)
    {
        Color closestColor = Color.black;
        float closestDist = float.PositiveInfinity;
        foreach (Color color in palette)
        {
            float currDist = pixel.Distance(color);
            if (currDist < closestDist)
            {
                closestDist = currDist;
                closestColor = color;
            }
        }

        return closestColor;
    }
    
    
    
}

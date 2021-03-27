using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Darken : MonoBehaviour
{
    
    private Renderer _render;

    [SerializeField]
    private Texture2D texture;

    [SerializeField][Range(0,1)] private float threshold;
    // Start is called before the first frame update
    void Awake()
    {
        if(_render == null)
            _render = GetComponent<Renderer>();
        
        ApplyDithering(threshold);
    }
    
    void OnValidate(){
        if(!Application.isPlaying)
            return;

        if(_render == null)
            _render = GetComponent<Renderer>();

        ApplyDithering(threshold);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // void ApplyDithering()
    // {
    //     Texture2D texture = Instantiate(this.texture);
    //     _render.material.mainTexture = texture;
    //     
    //     Color[] pixels = texture.GetPixels(0);
    //     for (int x = 0; x < texture.width; x++)
    //     {
    //         for (int y = 0; y < texture.height; y++)
    //         {
    //             int index = x + y * this.texture.width;
    //             Color pixel = pixels[index];
    //
    //             float r = Mathf.Round(pixel.r);
    //             float g = Mathf.Round(pixel.g);
    //             float b = Mathf.Round(pixel.b);
    //
    //             pixels[index] = new Color(r, g, b);
    //         }
    //     }
    //     texture.SetPixels(pixels, 0);
    //     texture.Apply();
    //     Debug.Log("Dithring done");
    // }
    

    void ApplyDithering(float _limit)
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
    
                if (pixel.grayscale <= _limit)
                    pixels[index] = Color.black;
                
            }
        }
        texture.SetPixels(pixels, 0);
        texture.Apply();
        Debug.Log("Dithring done");
    }
    
    
}

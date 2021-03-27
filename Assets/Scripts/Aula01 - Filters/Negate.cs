using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Negate : MonoBehaviour
{
    void Start()
    {
        Renderer render = GetComponent<Renderer>();

        Texture2D texture = Instantiate(render.material.mainTexture) as Texture2D;

        render.material.mainTexture = texture;

        int mipCount = Mathf.Min(3, texture.mipmapCount);

        for (int mip = 0; mip < mipCount; mip++) {
            Color[] pixels = texture.GetPixels(mip);
            for (int pixel = 0; pixel < pixels.Length;pixel++) {
                // float color =  (1-pixels[pixel].r  + 1-pixels[pixel].g + 1-pixels[pixel].b ) ;
                float newR = 1 - pixels[pixel].r;
                float newG = 1 - pixels[pixel].g;
                float newB = 1 - pixels[pixel].b;
                pixels[pixel] = new Color(newR, newG, newB);
                // pixels[pixel] = new Color(color, color, color, pixels[pixel].a);

            }
            texture.SetPixels(pixels, mip);
        }

        texture.Apply(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
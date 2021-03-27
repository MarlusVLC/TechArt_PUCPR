using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Binary : MonoBehaviour
{
    [SerializeField] [Range(0, 1)] private float scale;
    void Start()
    {
        
        
        Renderer render = GetComponent<Renderer>();

        Texture2D texture = Instantiate(render.material.mainTexture) as Texture2D;

        render.material.mainTexture = texture;

        int mipCount = Mathf.Min(3, texture.mipmapCount);

        for (int mip = 0; mip < mipCount; mip++) {
            Color[] pixels = texture.GetPixels(mip);
            for (int pixel = 0; pixel < pixels.Length;pixel++) {
                
                float newR;
                if (pixels[pixel].r > scale)
                {
                    newR = 1;
                }
                else
                {
                    newR = 0;
                }

                newR = pixels[pixel].r > scale ? 1 : 0; 
                float newG = pixels[pixel].g > scale ? 1 : 0; 
                float newB = pixels[pixel].b > scale ? 1 : 0;
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
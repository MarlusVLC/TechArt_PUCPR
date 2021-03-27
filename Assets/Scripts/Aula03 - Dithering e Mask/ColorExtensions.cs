using System.Collections;
using System.Collections.Generic;
using UnityEngine;



namespace DefaultNamespace


{
    public static class ColorExtensions
    {
        public static float Distance(this Color color1, Color color2)
        {
            float rDist = color1.r - color2.r;
            float bDist = color1.b - color2.b;
            float gDist = color1.g - color2.g;


            return Mathf.Sqrt(rDist * rDist + bDist * bDist + gDist * gDist);
        }

        public static float SimpleArithmeticAverage(this Color color)
        {
            return (color.r + color.g + color.b) / 3;
        }
        
        
        
        public static void FindClosestPaletteColor(this ref Color pixel, Color[] palette)
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

            pixel = closestColor;
        }
    }
    

}
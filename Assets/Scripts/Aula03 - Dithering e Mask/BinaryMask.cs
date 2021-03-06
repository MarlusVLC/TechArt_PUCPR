using UnityEngine;

namespace DefaultNamespace
{
    public class BinaryMask : MonoBehaviour
    {
        [SerializeField] private Texture2D toBeMaskedTexture;
        [SerializeField] private Texture2D maskImage;
        [SerializeField] private bool invertMask = false;
        [SerializeField] private bool useAlpha = false;

        private Renderer _render;
        private Texture2D _placeHolderTex;

        
        void Awake()
        {
            if(_render == null)
                _render = GetComponent<Renderer>();
            
        


            Mask(invertMask);

        }
    
        void OnValidate(){
            if(!Application.isPlaying)
                return;

            // if(_render == null)
            //     _render = GetComponent<Renderer>();
            
            Mask(invertMask);

        }




        void Mask(bool invert = false)
        {
            
            _render = GetComponent<Renderer>();
            Texture2D mainTex = Instantiate(_render.material.mainTexture) as Texture2D;
            _render.material.mainTexture = mainTex ;

            Texture2D texture = Instantiate(toBeMaskedTexture);
            Texture2D mask = Instantiate(maskImage);
        
            Color[] texPixels = texture.GetPixels(0);
            Color[] maskPixels = mask.GetPixels(0);
            
            for (int i = 0; i < texPixels.Length; i++)
            {
                if (invert)
                {
                    if (maskPixels[i].SimpleArithmeticAverage() <  0.156f)
                    {
                        texPixels[i] = Color.black;
                    }
                }
                else
                {
                    if (maskPixels[i].SimpleArithmeticAverage() > 0.9f)
                    {
                        texPixels[i] = Color.white;
                    }
                }

            }
            mainTex.SetPixels(texPixels, 0);
            mainTex.Apply();
            Debug.Log("Masking done");
        }
    }
    
    
    
    
}
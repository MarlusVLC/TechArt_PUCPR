using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KernelProf : MonoBehaviour
{
    private Renderer _render;

    [SerializeField]
    private Texture2D texture;

    private enum Effect{
        None,
        SmoothingMedian,SmoothingGaussian,SmoothingCross,
        Laplace, LaplaceDiagonal,
        Sharpen,Emboss,
        Sobel,Prewitt
    }
    [SerializeField]
    private Effect currentEffect;



    void Start()
    {
        if(_render == null)
            _render = GetComponent<Renderer>();

        UpdateEffect();
        
    }
    void OnValidate(){
        if(!Application.isPlaying)
            return;

        if(_render == null)
            _render = GetComponent<Renderer>();

        UpdateEffect();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void UpdateEffect(){
        switch (currentEffect)
        {
            case Effect.None:
                RunKernel(_filterNone);
                break;
            case Effect.SmoothingMedian:
                RunKernel(_filterSmoothingMedian);
                break;

            case Effect.SmoothingGaussian:
                RunKernel(_filterSmoothingMedian);
                break;

            case Effect.SmoothingCross:
                RunKernel(_filterSmoothingCross);
                break;

            case Effect.Laplace:
                RunKernel(_filterLaplace);
                break;

            case Effect.LaplaceDiagonal:
                RunKernel(_filterLaplaceDiagonal);
                break;

            case Effect.Sharpen:
                RunKernel(_filterSharpen);
                break;

            case Effect.Emboss:
                RunKernel(_filterEmboss);
                break;

            case Effect.Sobel:
                RunKernel(_filterSobelX,_filterSobelY);
                break;

            case Effect.Prewitt:
                RunKernel(_filterPrewittX,_filterPrewittY);
                break;
        }
        
    }
    private float[] _filterNone = new float[9]{
        0f,     0f,     0f,
        0f,     1f,     0f,
        0f,     0f,     0f
    };

    private float[] _filterSmoothingMedian = new float[9]{
        0.111f,     0.111f,     0.111f,
        0.111f,     0.111f,     0.111f,
        0.111f,     0.111f,     0.111f
    };

    private float[] _filterSmoothingGaussian = new float[9]{
        0.0625f,     0.125f,     0.0625f,
        0.125f,     0.25f,     0.125f,
        0.0625f,     0.125f,     0.0625f
    };

    private float[] _filterSmoothingCross = new float[9]{
        0f,     0.2f,     0f,
        0.2f,     0.2f,     0.2f,
        0f,     0.2f,     0f
    };

    private float[] _filterLaplace = new float[9]{
        0f,     1f,     0f,
        1f,     -4f,     1f,
        0f,     1f,     0f
    };

    private float[] _filterLaplaceDiagonal = new float[9]{
        0.5f,     1f,     0.5f,
        1f,     -6f,     1f,
        0.5f,     1f,     0.5f
    };

    private float[] _filterSharpen = new float[9]{
        0f,     -1f,     0f,
        -1f,     5f,     -1f,
        0f,     -1f,     0f
    };

    private float[] _filterEmboss = new float[9]{
        -2f,     -1f,     0f,
        -1f,     0f,     1f,
        0f,     1f,     2f
    };

    private float[] _filterSobelX = new float[9]{
        -1f,     0f,     1f,
        -2f,     0f,     2f,
        -1f,     0f,     1f
    };

    private float[] _filterSobelY = new float[9]{
        1f,     2f,     1f,
        0f,     0f,     0f,
        -1f,     -2f,     -1f
    };

    private float[] _filterPrewittX = new float[9]{
        -1f,     0f,     1f,
        -1f,     0f,     1f,
        -1f,     0f,     1f
    };

    private float[] _filterPrewittY = new float[9]{
        1f,     1f,     1f,
        0f,     0f,     0f,
        -1f,     -1f,     -1f
    };

    void RunKernel(float[] filter1,float[] filter2 = null){
        Texture2D texture = Instantiate(this.texture) as Texture2D;
        _render.material.mainTexture = texture;

        Color[] pixels = texture.GetPixels(0);
        Color[] newPixels = new Color[pixels.Length];

        for (int i = 0; i < pixels.Length; i++)   //Percorre todos os pixels da imagem original
        {
            Color color1 = new Color(0f,0f,0f,0f); //cor que será modificada
            Color color2 = new Color(0f,0f,0f,0f);

            for(int y=-1;y<=1;y++){     //Percore as 3 linhas do kernel
                // i/texture.width é um int, então retorna um número inteiro. o +y (que pode ser -1, 0 e 1 é pra considerar um vizinho do
                // ponto central no eixo Y(
                int posY = i/texture.width + y;   
                if(posY < 0 || posY >= texture.height) //Executa o skipping. Os pixels nao existem nessas posições
                    continue;

                for (int x = -1; x <= 1; x++){ //Percore as 3 colunas do kernel
                    //Pega o resto da divisao, que indica uma posicao no eixto. o +x indica um vizinho ou o próprio ponto cental no eixo x
                    int posX = i%texture.width + x;  

                    if(posX < 0 || posX >= texture.width) //aplica o skipping
                        continue;

                    int pos = posY*texture.width + posX; //posY*width pega um indice de linha, já posX pega o indice de cluna
                    //y+1 evita que que retorne um valor na posicao -1. (y*3) alinha o valor na linha certa. +(x+1) alinha na coluna certa
                    int posMatrix = (y+1)*3 + (x+1);  
                    
                    color1 += filter1[posMatrix] * pixels[pos]; //executaa operacao pixel por pixel da multiplicao KERNEL*IMAGEM;
                    //Vídeo parado em 01:07:56
                    
                    if(filter2 != null)
                        color2 += filter2[posMatrix] * pixels[pos];
                }
            }
            if(filter2 != null){
                color1.r = Mathf.Sqrt(color1.r*color1.r + color2.r*color2.r);
                color1.g = Mathf.Sqrt(color1.g*color1.g + color2.g*color2.g);
                color1.b = Mathf.Sqrt(color1.b*color1.b + color2.b*color2.b);
            }

            newPixels[i] = new Color(color1.r,color1.g,color1.b,pixels[i].a);
        }
        texture.SetPixels(newPixels,0);
        texture.Apply(false);
        Debug.Log("Kernel Processed");
    }
}

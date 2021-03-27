using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Convolute : MonoBehaviour
{
    
    //int posY =  i/texture.width + y;

    private enum Kernel
    {
        AverageSmooth,
        GaussianSmooth,
        CrossSmooth,
        LaplaceBorders,
        LaplaceDiagonals,
        SobelBorders,
        PrewittBorders,
        Sharpen,
        Emboss,
        Custom,
    }

    private float[,] AverageFilter = new float[3, 3]
    {
        {0.11111111f, 0.11111111f, 0.11111111111f},
        {0.111111111111111111f, 0.111111111111f, 0.111111111f},
        {0.11111111111111f, 0.11111111111f, 0.111111111111f}
    };
    
    private float[,] GaussianFilter = new float[3, 3]
    {
        {0.0625f, 0.125f, 0.0625f},
        {0.125f, 0.25f, 0.125f},
        {0.06f, 0.125f, 0.0625f}
    };
    
    private float[,] CrossFilter = new float[3, 3]
    {
        {0, 0.2f, 0},
        {0.2f, 0.2f, 0.2f},
        {0, 0.2f, 0f}
    };

    private float[,] Laplace = new float[3, 3]
    {
        {0, 1, 0},
        {1, -4, 1},
        {0, 1, 0}
    };

    private float[,] LaplaceDiagonal = new float[3, 3]
    {
        {0.5f, 1, 0.5f},
        {1, -6, 1},
        {0.5f, 1, 0.5f}
    };

    private float[,,] Sobel = new float[2, 3, 3]
    {
        {
            {-1, 0, 1},
            {-2, 0, 2},
            {-1, 0, 1},
        },
        {
            {1, 2, 1},
            {0, 0, 0},
            {-1, -2, -1},
        }
    };
    
    private float[,] Sharpen = new float[3, 3]
    {
        {0, -1, 0},
        {-1, 5, -1},
        {0, -1, 0}
    };

    private float[,] Emboss = new float[3, 3]
    {
        {-2f, -1f, 0},
        {-1f, 0, 1f},
        {0, 1f, 2f}
    };
    
    private float[,] Custom = new float[3,3]
    {
        {-0.1f, -0.2f, -0.3f},
        {-0.4f, 4, -0.5f},
        {-0.6f, -0.7f, -0.8f}
    }; 


    [SerializeField] private Kernel _kernel;

    private Renderer _render;
    private Texture2D _texture;
    private Color[,] copyOfTex;
    
    


    private void Awake()
    {
        _render = GetComponent<Renderer>();
        _texture = Instantiate(_render.material.mainTexture) as Texture2D;
        _render.material.mainTexture = _texture;
    }

    void Start()
    {

        
        copyOfTex = Texture2DMatrix(_texture);
        chooseKernel(_texture,copyOfTex,_kernel);

        
        _texture.Apply(false);
    }

    void OnValidate()
    {
        if (!Application.isPlaying)
            return;
        
        // Color[,] copyOfTex = Texture2DMatrix(_texture);
        chooseKernel(_texture,copyOfTex,_kernel);

        _texture.Apply(false);
    }

    private Color[,] Texture2DMatrix(Texture2D texture)
    {
        int w = texture.width;
        int h = texture.height;
        Color[,] texCopy = new Color[h, w];

        for (int x = 0; x < w; x++)
        {
            for (int y = 0; y < h; y++)
            {
                texCopy[x, y] = texture.GetPixel(x, y);
            }
        }

        return texCopy;
    }

    private void SetFilter(in Texture2D originalImg, Color[,] overMatrix, float[,] kernel)
    {
        int w = originalImg.width;
        int h = originalImg.height;
        Color[,] currBlock;

        for (int x = 0; x < w; x++)
        {
            for (int y = 0; y < h; y++)
            {
                currBlock = new Color[kernel.GetLength(1), kernel.GetLength(0)];
                // currBlock[1, 1] = overMatrix[x, y];

                float[,] currBlock_R = new float[kernel.GetLength(1), kernel.GetLength(0)];
                float[,] currBlock_G = new float[kernel.GetLength(1), kernel.GetLength(0)];
                float[,] currBlock_B = new float[kernel.GetLength(1), kernel.GetLength(0)];


                for (int i = 0; i < currBlock.GetLength(1); i++)
                {
                    for (int j = 0; j < currBlock.GetLength(0); j++)
                    {
                        if (
                            (x - 1 == -1 && i == 0) ||
                            (y - 1 == -1 && j == 0) ||
                            (x + 1 == w && i == currBlock.GetLength(1) - 1) ||
                            (y + 1 == h && j == currBlock.GetLength(1) - 1)
                        )
                        {
                            currBlock_R[i, j] = 0;
                            currBlock_G[i, j] = 0;
                            currBlock_B[i, j] = 0;
                            continue;
                        }


                        // currBlock[i, j] = overMatrix[x + (i - 1), y + (j - 1)];
                        currBlock_R[i, j] = overMatrix[x + (i - 1), y + (j - 1)].r;
                        currBlock_G[i, j] = overMatrix[x + (i - 1), y + (j - 1)].g;
                        currBlock_B[i, j] = overMatrix[x + (i - 1), y + (j - 1)].b;
                    }
                }

                float newR = GetKernelApplication(currBlock_R, kernel);
                float newG = GetKernelApplication(currBlock_G, kernel);
                float newB = GetKernelApplication(currBlock_B, kernel);

                originalImg.SetPixel(x, y, new Color(newR, newG, newB));
            }
        }
    }
    
    private void SetFilter(in Texture2D originalImg, Color[,] overMatrix, float[,,] kernel)
    {
        int w = originalImg.width;
        int h = originalImg.height;
        Color[,] currBlock;

        for (int x = 0; x < w; x++)
        {
            for (int y = 0; y < h; y++)
            {
                currBlock = new Color[kernel.GetLength(2), kernel.GetLength(1)];
                // currBlock[1, 1] = overMatrix[x, y];

                float[,] currBlock_R = new float[kernel.GetLength(2), kernel.GetLength(1)];
                float[,] currBlock_G = new float[kernel.GetLength(2), kernel.GetLength(1)];
                float[,] currBlock_B = new float[kernel.GetLength(2), kernel.GetLength(1)];


                for (int i = 0; i < currBlock.GetLength(1); i++)
                {
                    for (int j = 0; j < currBlock.GetLength(0); j++)
                    {
                        if (
                            (x - 1 == -1 && i == 0) ||
                            (y - 1 == -1 && j == 0) ||
                            (x + 1 == w && i == currBlock.GetLength(1) - 1) ||
                            (y + 1 == h && j == currBlock.GetLength(1) - 1)
                        )
                        {
                            currBlock_R[i, j] = 0;
                            currBlock_G[i, j] = 0;
                            currBlock_B[i, j] = 0;
                            continue;
                        }


                        // currBlock[i, j] = overMatrix[x + (i - 1), y + (j - 1)];
                        currBlock_R[i, j] = overMatrix[x + (i - 1), y + (j - 1)].r;
                        currBlock_G[i, j] = overMatrix[x + (i - 1), y + (j - 1)].g;
                        currBlock_B[i, j] = overMatrix[x + (i - 1), y + (j - 1)].b;
                    }
                }

                float newR = GetKernelApplication(currBlock_R, kernel);
                float newG = GetKernelApplication(currBlock_G, kernel);
                float newB = GetKernelApplication(currBlock_B, kernel);

                originalImg.SetPixel(x, y, new Color(newR, newG, newB, overMatrix[x,y].a));
            }
        }
    }
    
    
    
    
    
    
    

    private float GetKernelApplication(float[,] componentMatrix, float[,] kernel)
    {
        float result = 0;
        for (int x = 0; x < componentMatrix.GetLength(1); x++)
        {
            for (int y = 0; y < componentMatrix.GetLength(0); y++)
            {
                result += componentMatrix[x, y] * kernel[x, y];
            }
        }

        return result;
    }

    private float GetKernelApplication(float[,] componentMatrix, float[,,] kernel)
    {
        float[,] gX = new float[3, 3];
        float[,] gY = new float[3, 3];

        for (int x = 0; x < componentMatrix.GetLength(1); x++)
        {
            for (int y = 0; y < componentMatrix.GetLength(0); y++)
            {
                gX[x, y] = componentMatrix[x, y] * kernel[0, x, y];
                gY[x, y] = componentMatrix[x, y] * kernel[1, x, y];
            }
        }
        
        float result = 0;
        for (int x = 0; x < componentMatrix.GetLength(1); x++)
        {
            for (int y = 0; y < componentMatrix.GetLength(0); y++)
            {
                result += Mathf.Sqrt(gX[x,y]*gX[x,y]+gY[x,y]*gY[x,y]);
            }
        }


        return result;

    }




    private void chooseKernel(in Texture2D texture, Color[,] copyOfTex, Kernel _kernel)
    {
        switch (_kernel)
        {
            case (Kernel.AverageSmooth):
                SetFilter(texture, copyOfTex, AverageFilter);
                break;
            case (Kernel.GaussianSmooth):
                SetFilter(texture, copyOfTex, GaussianFilter);
                break;
            case (Kernel.CrossSmooth):
                SetFilter(texture, copyOfTex, CrossFilter);
                break;
            case (Kernel.LaplaceBorders):
                SetFilter(texture, copyOfTex, Laplace);
                break;
            case (Kernel.LaplaceDiagonals):
                SetFilter(texture, copyOfTex, LaplaceDiagonal);
                break;
            case (Kernel.SobelBorders):
                SetFilter(texture, copyOfTex, Sobel);
                Debug.Log(" precisa ser corrigido");
                break;
            case (Kernel.PrewittBorders):
                Debug.Log(" ainda nao implementado");
                break;
            case (Kernel.Sharpen):
                SetFilter(texture, copyOfTex, Sharpen);
                break;
            case (Kernel.Emboss):
                SetFilter(texture, copyOfTex, Emboss);
                break;
            case (Kernel.Custom):
                SetFilter(texture, copyOfTex, Custom);
                break;
        }
    }
}
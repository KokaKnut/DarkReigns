using UnityEngine;
using System.Collections;

public class ImgScanner : MonoBehaviour {

    public Texture2D[] images;

    public byte[,] ScanImg(int i)
    {
        byte[,] data = new byte[images[i].width, images[i].height];

        Color32[] allPixels = images[i].GetPixels32();

        for(int x=0; x < data.GetLength(0); x++)
        {
            for (int y = 0; y < data.GetLength(1); y++)
            {
                data[x, y] = allPixels[(y * data.GetLength(0)) + x].r;
            }
        }

        return data;
    }
}
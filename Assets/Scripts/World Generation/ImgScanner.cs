using UnityEngine;
using System.Collections.Generic;

public class ImgScanner : MonoBehaviour {

    public Texture2D[] images;

    // new colors go here and in "Start()"
    public enum ColorKey
    {
        black,
        white,
        red,
        blue,
        green,
        yellow
    }

    Dictionary<Color32, ColorKey> colorsDictionary;

    public ColorKey[,] ScanImg(int i)
    {
        BuildColorsDictionary();

        ColorKey[,] data = new ColorKey[images[i].width, images[i].height];

        Color32[] allPixels = images[i].GetPixels32();

        ColorKey test;

        for(int x=0; x < data.GetLength(0); x++)
        {
            for (int y = 0; y < data.GetLength(1); y++)
            {
                if (colorsDictionary.TryGetValue(allPixels[(y * data.GetLength(0)) + x], out test))
                    data[x, y] = test;
                else
                    Debug.LogError("No match found for a color in image at index " + i);
            }
        }

        return data;
    }

    void BuildColorsDictionary()
    {
        if (colorsDictionary == null)
        {
            colorsDictionary = new Dictionary<Color32, ColorKey>();

            colorsDictionary.Add(new Color32(0, 0, 0, 0), ColorKey.black);
            colorsDictionary.Add(new Color32(255, 255, 255, 0), ColorKey.white);
            colorsDictionary.Add(new Color32(255, 0, 0, 0), ColorKey.red);
            colorsDictionary.Add(new Color32(0, 0, 255, 0), ColorKey.blue);
            colorsDictionary.Add(new Color32(0, 255, 0, 0), ColorKey.green);
            colorsDictionary.Add(new Color32(255, 255, 0, 0), ColorKey.yellow);
        }
    }
}
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImageHelper
{
    public static ImageHelper imageHelper;
    private Dictionary<string, Sprite> imageDict;

    public static void init(LevelConfig levelConfig)
    {
        imageHelper = new ImageHelper(levelConfig);
    }

    private ImageHelper(LevelConfig levelConfig)
    {
        imageDict = new Dictionary<string, Sprite>();
        for (int i = 0; i < levelConfig.IngredientWeights.Length; i++)
        {
            string path = levelConfig.IngredientWeights[i].Name;
            // Sprite sprite = Resources.Load<Sprite>(path);
            Sprite sprite = null;
            imageDict.Add(levelConfig.IngredientWeights[i].Name, sprite);
        }
    }

    public Sprite getImageDictionary(string name)
    {
        if (name.Equals(""))
        {
            return null;
        }
        return this.imageDict[name];
    }

    public static ImageHelper getInstance()
    {
        return imageHelper;
    }
    
}

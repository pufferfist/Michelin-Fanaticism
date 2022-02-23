using System;
using System.Collections;
using System.Collections.Generic;
using MenuNameSpace;
using UnityEngine;

[Serializable]
public class IngredientWeights{
    public  string Name;
    public  int Weight;

}
[Serializable]
public class LevelConfigObj {
    public IngredientWeights[] IngredientsWeights;
    
}
[Serializable]
public class LevelConfigList {
    public int Level;
    public LevelConfigObj[] LevelConfigObj;
    
}

public class MenuConfig
{
    public Recipe[] menus;
}
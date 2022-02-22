using System;
using System.Collections;
using System.Collections.Generic;
using MenuNameSpace;
using UnityEngine;

[Serializable]
public class IngridientWeights{
    public  string Name;
    public  int Weight;

}
[Serializable]
public class LevelConfigObj {
    public IngridientWeights[] IngridientsWeights;
    
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
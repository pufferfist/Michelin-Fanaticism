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

public class LevelConfigObj {
    public IngridientWeights[] IngridientsWeights;
    
}

public class MenuConfig
{
    public Recipe[] menus;
}
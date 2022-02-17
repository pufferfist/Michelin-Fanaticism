using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class IngridientWeights{
    public  string Name;
    public  int Weight;

}

public class LevelConfigObj {
    public IngridientWeights[] IngridientsWeights;
    
}
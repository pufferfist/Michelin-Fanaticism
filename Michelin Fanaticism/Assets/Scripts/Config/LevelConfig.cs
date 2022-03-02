using System;
using System.Collections;
using System.Collections.Generic;
using MenuNameSpace;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class IngredientWeights{
    public string Name;
    public int Weight;
}
[Serializable]
public class ItemWeights{
    public string Name;
    public int Weight;
}
[Serializable]
public class RecipeInfo{
    public int ID;
    public string Name;
	public float TotalTime;
	public int Score;
	public List<string> Ingredients;
}
[Serializable]
public class LevelConfig {
	public int Level;
	public int BagSlot;
	public int RecipeSlot;
	public RecipeInfo[] Recipes;
    public IngredientWeights[] IngredientWeights;
	public ItemWeights[] ItemWeights;
}
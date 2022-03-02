using UnityEngine;
using UnityEngine.TestTools;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

namespace MenuNameSpace
{
    public class Recipe: ICloneable
    {
        public int id;
        public string name;
        public float totalTime;
        public float startTime;
        public float remainTime;
        public int score;
        public List<string> ingredients;

        public Recipe(int id, string name, float totalTime, int score, List<string> ingredients)
        {
            this.id = id;
            this.name = name;
            this.totalTime = totalTime;
            this.remainTime = totalTime;
            this.score = score;
            this.ingredients = ingredients;
        }

        public object Clone()
        {
            Recipe newRecipe = new Recipe(this.id, this.name, this.totalTime, this.score, this.ingredients);
            newRecipe.startTime = Time.time;
            return newRecipe;
        }
    }
    
}
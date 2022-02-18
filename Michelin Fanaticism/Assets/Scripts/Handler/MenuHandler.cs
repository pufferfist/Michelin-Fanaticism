using System;
using System.Collections.Generic;
using System.Linq;
using MenuNameSpace;
using UnityEngine;

namespace DefaultNamespace
{
    public class MenuHandler
    {
        //service dependency
        private UIHandler uiHandler;
        
        private Recipe[] recipes;
        private List<Recipe> easyRecipes;
        private float modifyTimer;//last modify time(menu finished, expired or added)
        private float updateTimer;//last slider update time

        public MenuHandler(MenuConfig config=null)
        {
            recipes = new Recipe[3];

            if (config==null)
            {
                string[] ingredients1 = {"Beef", "Lettuce", "Bread"};
                string[] ingredients2 = {"Chicken", "Lettuce", "Bread"};
                string[] ingredients3 = {"Bread", "Strawberry"};
                string[] ingredients4 = {"Bread", "Strawberry", "Beef"};
                Recipe menu1 = new Recipe(1, "Burger", 30,new List<string>(ingredients1));
                Recipe menu2 = new Recipe(2, "ChickenSandwich", 30,new List<string>(ingredients2));
                Recipe menu3 = new Recipe(3, "SummerPudding", 30,new List<string>(ingredients3));
                Recipe menu4 = new Recipe(4, "HealthyFood", 30,new List<string>(ingredients4));
                easyRecipes = new List<Recipe>(){menu1,menu2,menu3,menu4};
                
                addRecipe();
            }
            else
            {
                //todo
            }
            
        }
        /*
         * return null: not finished
         * called by gameManager if a pickup happens
         */
        public Recipe checkFinish(Stack<String> collected)
        {
            for (int i = 0; i < recipes.Length; i++)
            {
                Recipe recipe = recipes[i];
                bool equal = collected.ToArray().OrderBy(i => i).SequenceEqual(
                    recipe.ingredients.ToArray().OrderBy(i => i));
                if (equal)
                {
                    recipes[i] = null;
                    return recipe;
                }
            }
            return null;
        }

        //called by gameManager per frame
        public void updateRecipes()
        {
            //update remaining time
            if (Time.time-updateTimer>0.5)
            {
                for (int i=0;i<recipes.Length;i++)
                {
                    recipes[i].remainTime -= Time.time - updateTimer;
                    if (recipes[i].remainTime<0)
                    {
                        recipes[i] = null;
                    }
                }

                updateTimer = Time.time;
                uiHandler.updateMenuPanel(recipes);
            }
            
            //add one menu per 5s
            if (Time.time-modifyTimer>5)
            {
                addRecipe();
                uiHandler.updateMenuPanel(recipes);
            }
            
        }
        
        //fill empty menu slot
        //same recipe can't use the same one object for different remain time, so we clone a new one.
        private void addRecipe()
        {
            for (int i = 0; i < recipes.Length; i++)
            {
                if (recipes[i]==null)
                {
                    recipes[i] = easyRecipes[UnityEngine.Random.Range(0, easyRecipes.Count)].Clone() as Recipe;
                    modifyTimer = Time.time;
                    break;
                }
            } 
        }
    }
}
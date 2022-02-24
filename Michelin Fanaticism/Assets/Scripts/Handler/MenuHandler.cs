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
        private float modifyTimer; //last modify time(menu finished, expired or added)
        private bool lastAdd;// true if last modifyTimer change is by add;
        private float updateTimer; //last slider update time

        public MenuHandler(UIHandler uiHandler, LevelConfig levelConfig)
        {
            this.uiHandler = uiHandler;
            recipes = new Recipe[3];
			easyRecipes = new List<Recipe>();
			for (int i = 0; i < levelConfig.Recipes.Length; ++i) {
				easyRecipes.Add(new Recipe(levelConfig.Recipes[i].ID,
										   levelConfig.Recipes[i].Name,
										   levelConfig.Recipes[i].TotalTime,
										   levelConfig.Recipes[i].Score,
										   levelConfig.Recipes[i].Ingredients));
			}
			addRecipe();
        }
        /*
         * return null: no finished recipe
         * called by gameManager if a pickup happens
         */
        public Recipe checkFinish(Stack<String> collected)
        {
            for (int i = 0; i < recipes.Length; i++)
            {
                Recipe recipe = recipes[i];
                if (recipe != null)
                {
                    bool equal = collected.ToArray().OrderBy(i => i).SequenceEqual(
                        recipe.ingredients.ToArray().OrderBy(i => i));
                    if (equal)
                    {
                        expireRecipe(i);
                        uiHandler.updateMenuPanel(recipes);
                        return recipe;
                    }
                }
            }

            return null;
        }

        //called by gameManager per frame
        public void updateRecipes()
        {
            //update remaining time per 0.5s
            if (Time.time - updateTimer > 0.5)
            {
                for (int i = 0; i < recipes.Length; i++)
                {
                    if (recipes[i] == null)
                    {
                        continue;
                    }

                    recipes[i].remainTime -= Time.time - updateTimer;
                    if (recipes[i].remainTime < 0)
                    {
                        expireRecipe(i);
                    }
                }

                updateTimer = Time.time;
                uiHandler.updateMenuPanel(recipes);
            }

            //add one menu per 5s
            if (Time.time - modifyTimer > 5)
            {
                addRecipe();
            }
        }

        //fill empty menu slot
        //same recipe can't use the same one object for different remain time, so we clone a new one.
        private void addRecipe()
        {
            for (int i = 0; i < recipes.Length; i++)
            {
                if (recipes[i] == null)
                {
                    recipes[i] = easyRecipes[UnityEngine.Random.Range(0, easyRecipes.Count)].Clone() as Recipe;
                    modifyTimer = Time.time;
                    lastAdd = true;
                    break;
                }
            }
            uiHandler.updateMenuPanel(recipes);
        }

        private void expireRecipe(int index)
        {
            recipes[index] = null;
            if (lastAdd)
            {
                modifyTimer = Time.time;
            }
            lastAdd = false;
        }
    }
}
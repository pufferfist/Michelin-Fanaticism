using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MenuNameSpace;
using UnityEngine;
using UnityEngine.UI;

namespace DefaultNamespace
{
    public class MenuHandler
    {
        //service dependency
        private UIHandler uiHandler;

        private Recipe[] recipes;
        private List<Recipe> candidateRecipes;
        private float modifyTimer; //last modify time(menu finished, expired or added)
        private bool lastAdd;// true if last modifyTimer change is by add;
        private float updateTimer; //last slider update time
        private int newRecipeSpeed;

        public MenuHandler(UIHandler uiHandler, LevelConfig levelConfig)
        {
            this.uiHandler = uiHandler;
            recipes = new Recipe[levelConfig.RecipeSlot];
            newRecipeSpeed = levelConfig.newRecipeSpeed==0?5:levelConfig.newRecipeSpeed;//default speed: per 5s
			candidateRecipes = new List<Recipe>();
			for (int i = 0; i < levelConfig.Recipes.Length; ++i) {
				candidateRecipes.Add(new Recipe(levelConfig.Recipes[i].ID,
										   levelConfig.Recipes[i].Name,
										   levelConfig.Recipes[i].TotalTime,
										   levelConfig.Recipes[i].Score,
										   levelConfig.Recipes[i].Ingredients));
			}
            updateTimer = Time.time;
			addRecipe();
        }
        /*
         * return null: no finished recipe
         * called by gameManager if a pickup happens
         */
        public Recipe checkFinish(List<String> collected)
        {
            int expireIndex=-1;
            float minRemainTime = float.MaxValue;
            for (int i = 0; i < recipes.Length; i++)
            {
                Recipe recipe = recipes[i];
                if (recipe != null)
                {
                    bool equal = collected.ToArray().OrderBy(i => i).SequenceEqual(
                        recipe.ingredients.ToArray().OrderBy(i => i));
                    if (equal&&recipe.remainTime<minRemainTime)
                    {
                        expireIndex = i;
                        minRemainTime = recipe.remainTime;
                    }
                }
            }

            if (expireIndex==-1)
            {
                return null;
            }

            Recipe expired = recipes[expireIndex];
            // expireRecipe(expireIndex);
            // uiHandler.updateMenuPanel(recipes);
            return expired;
        }
        
        public IEnumerator Fadeout(Recipe finishedRecipe,Action<bool> done)
        {
            //do your thing here
            yield return uiHandler.shineBeforeUpdateMenuPanel(finishedRecipe.index);
            // yield return null;

            expireRecipe(finishedRecipe.index);
            done(true);
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
            if (Time.time - modifyTimer > newRecipeSpeed)
            {
                addRecipe();
            }
        }
        
        //called by gameManager per frame when on hold
        public void updateOnHoldTimer()
        {
            modifyTimer = Time.time;
            updateTimer = Time.time;
        }

        //fill empty menu slot
        //same recipe can't use the same one object for different remain time, so we clone a new one.
        private void addRecipe()
        {
            for (int i = 0; i < recipes.Length; i++)
            {
                if (recipes[i] == null)
                {
                    recipes[i] = candidateRecipes[UnityEngine.Random.Range(0, candidateRecipes.Count)].Clone() as Recipe;
                    recipes[i].index = i;
                    modifyTimer = Time.time;
                    lastAdd = true;
                    break;
                }
            }
            uiHandler.updateMenuPanel(recipes);
        }

        public void expireRecipe(int index)
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
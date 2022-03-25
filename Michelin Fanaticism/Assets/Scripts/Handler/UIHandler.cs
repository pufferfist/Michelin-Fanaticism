using System;
using System.Collections.Generic;
using MenuNameSpace;
using UnityEngine;
using UnityEngine.UI;

namespace DefaultNamespace
{
    public class UIHandler
    {
        private LevelConfig levelConfig;
        private GameObject[] collectedPanel;
        private GameObject[] menuPanel;
        private Text score;
        private Text timer;
        private Text lives;

        //init: find ui elements' reference
        public UIHandler(GameObject ui,LevelConfig levelConfig)
        {
            this.levelConfig = levelConfig;
            Transform hud = ui.transform.Find("Hud");
            collectedPanel = new GameObject[2];
            menuPanel = new GameObject[3];

            //find collected panel
            for (int i = 0; i < collectedPanel.Length; i++)
            {
                collectedPanel[i] = hud.Find("CollectedPanel" + (i + 1)).gameObject;
                resetCollectedPanel(i);
                if (i+1>levelConfig.BagSlot)
                {
                    collectedPanel[i].SetActive(false);
                }
            }

            if (levelConfig.Level>=3)
            {
                collectedPanel[0].transform.position += new Vector3(0, 20, 0);
            }
            
            //find menu panel
            for (int i = 0; i < menuPanel.Length; i++)
            {
                menuPanel[i] = hud.Find("MenuPanel").Find("Recipe" + (i + 1)).gameObject;
                menuPanel[i].SetActive(false);
            }

            //find score
            score = hud.Find("ScoreBar").Find("Scores").GetComponent<Text>();

            //find timer
            timer = hud.Find("TimeBar").Find("Time").GetComponent<Text>();
            
            //find lives
            Transform health = hud.Find("Health");

            Transform stuPanel = hud.Find("StuPanel");
            GameObject switchTip = stuPanel.Find("SwitchTip").gameObject;
            GameObject dropTip = stuPanel.Find("DropTip").gameObject;
            GameObject healthTip = stuPanel.Find("HealthTip").gameObject;
            GameObject barrierTip = stuPanel.Find("BarrierTip").gameObject;
            GameObject heartTip = stuPanel.Find("HeartTip").gameObject;
            GameObject backPackTip = stuPanel.Find("BackpackTip").gameObject;
            GameObject recipeTip = hud.Find("StuPanel").Find("RecipeTip").gameObject;
            GameObject targetTip = hud.Find("StuPanel").Find("TargetTip").gameObject;
            //第一关基础的AD控制
            if (levelConfig.Level==1){
                dropTip.SetActive(true);
                recipeTip.SetActive(true);
                backPackTip.SetActive(true);
            }else{
                dropTip.SetActive(false);
                 recipeTip.SetActive(false);
                 backPackTip.SetActive(false);
            }

            //第二关增加更多菜谱
            if (levelConfig.Level==2){
                targetTip.SetActive(true);
            }else{
                 targetTip.SetActive(false);
            }

            //第三关切换背包
            if(levelConfig.Level==3){
                switchTip.SetActive(true);
            }else{
                switchTip.SetActive(false);
            }

            if(levelConfig.Level==4){
                healthTip.SetActive(true);
                barrierTip.SetActive(true);
                    heartTip.SetActive(true);
            }else{
                healthTip.SetActive(false);
                barrierTip.SetActive(false);
                heartTip.SetActive(false);
            }            
            
            

            if (levelConfig.Level<=3)
            {
                health.gameObject.SetActive(false);
            }
            else
            {
                lives = health.Find("Lives").GetComponent<Text>();
            }
            
        }

        public void updateCollectedPanel(int id, List<String> ingres)
        {
            resetCollectedPanel(id);
            int i = 0;
            foreach (String ingre in ingres)
            {
                collectedPanel[id].transform.GetChild(i).GetComponent<Image>().sprite = ImageHelper.getInstance().getImageDictionary(ingre);
                i++;
            }
        }

        public void updateMenuPanel(Recipe[] recipeList)
        {
            for (int i = 0; i < recipeList.Length; i++)
                //update everything
            {
                GameObject recipe = menuPanel[i];
                if (recipeList[i] != null && !recipe.activeSelf)
                {
                    //empty the ingredient list in one recipe
                    foreach (Transform obj in recipe.transform)
                    {
                        if (obj.CompareTag("Ingredient"))
                        {
                            obj.GetComponent<Image>().sprite = null;
                        }
                        else // this is timer
                        {
                            Slider timer = obj.GetComponent<Slider>();
                            timer.minValue = 0;
                            timer.maxValue = recipeList[i].totalTime;
                            timer.value = recipeList[i].remainTime;
                        }
                    }

                    //update the displayed recipe using input recipe list
                    for (int j = 0; j < recipeList[i].ingredients.Count; j++)
                    {
                        recipe.transform.GetChild(j).GetComponent<Image>().sprite = 
                            ImageHelper.getInstance().getImageDictionary(recipeList[i].ingredients[j]);
                    }

                    recipe.SetActive(true);
                }
                else if (recipeList[i] != null && recipe.activeSelf)
                    //update only timer
                {
                    recipe.transform.Find("timer").GetComponent<Slider>().value = recipeList[i].remainTime;
                }
                else
                {
                    recipe.SetActive(false);
                }
            }
        }

        public void updateScore(int score)
        {
            this.score.text = score.ToString();
        }

        public void updateTime(int time)
        {
            this.timer.text = $"{time / 60:D2}:{time % 60:D2}";
        }
        
        public void updateLives(int live)
        {
            if(lives == null) return;
            this.lives.text = live.ToString();
        }

        public void switchBag(int activeBag)
        {
            if (levelConfig.Level<=2)
            {
                return;
            }
            collectedPanel[activeBag].transform.position += new Vector3(0, 20, 0);
            collectedPanel[activeBag^1].transform.position += new Vector3(0, -20, 0);
        }

        private void resetCollectedPanel(int index)
        {
            foreach (Transform ingre in collectedPanel[index].transform)
            {
                ingre.GetComponent<Image>().sprite = null;
            }
        }
    }
}
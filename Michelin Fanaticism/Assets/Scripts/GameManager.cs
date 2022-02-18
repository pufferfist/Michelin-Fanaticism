using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DefaultNamespace;
using UnityEngine;
using MenuNameSpace;
using UnityEngine.UI;
using Random = UnityEngine.Random;
using MenuToolsSpace;
using PanelSpace;
using UnityEngine.Analytics;
using UnityEngine.Serialization;

public class GameManager : MonoBehaviour
{
    public static GameManager gm;

    public GameState gameState;

    public MainCharacter character;

    public GameObject ui;
    private UIHandler uiHandler;

    public Text scoreText;

    //menu1
    public Text ingredientText1;
    public Text ingredientText2;
    public Text ingredientText3;

    public Slider timeSlider1;

    //menu2
    public Text ingredientText4;
    public Text ingredientText5;
    public Text ingredientText6;

    public Slider timeSlider2;

    //menu3
    public Text ingredientText7;
    public Text ingredientText8;
    public Text ingredientText9;
    public Slider timeSlider3;

    public Text timeText;

    // names in collected panel1
    public Text currentIngredientText1;
    public Text currentIngredientText2;

    public Text currentIngredientText3;

    // names in collected panel2
    public Text currentIngredientText4;
    public Text currentIngredientText5;
    public Text currentIngredientText6;
    public GameObject recipe1;
    public GameObject recipe2;
    public GameObject recipe3;
    private const float EXPIRE_TIME = 30.0f;
    private List<Recipe> easyMenus;
    private List<Recipe> toBeDoneMenus;
    private Recipe currentMenu1;
    private Recipe currentMenu2;
    private Recipe currentMenu3;
    private Panel panel1 = new Panel();
    private Panel panel2 = new Panel();

    private List<string> pickedIngredients1;
    private List<string> pickedIngredients2;
    private int currentScore;
    public int resTime = 60;
    public int successScore;
    private float gameStartTime;
    private float updateTimer;
    public GameObject successPanel;
    public GameObject failPanel;
    private float recipe2UpdateTime;
    private float recipe3UpdateTime;

    //data tracking
    private Dictionary<String, int> recipePopularity;

    private void setGameState(GameState state)
    {
        gameState = state;
        character.changeState(state);
    }

    private void Start()
    {
        if (gm == null)
            gm = GetComponent<GameManager>();
        uiHandler = new UIHandler(ui);
        
        currentScore = 0;
        setGameState(GameState.Playing);
        initialMenuAndPanel();
        initialToBeDoneMenus();
        showMenus();
        showPanels();
        // updateMenu();
        gameStartTime = Time.time;
        recipe2UpdateTime = gameStartTime;
        recipe3UpdateTime = gameStartTime;

        //init data tracking variables
        recipePopularity = new Dictionary<string, int>
        {
            {"ChickenSandwich", 0},
            {"Burger", 0},
            {"SummerPudding", 0},
            {"HealthyFood", 0}
        };
    }

    private void Update()
    {
        switch (gameState)
        {
            case GameState.Playing:

                scoreText.text = currentScore + "";

                //Update every second and display the remaining time.
                if ((Time.time - gameStartTime) >= 1)
                {
                    gameStartTime = Time.time;
                    resTime--;
                    timeText.text = string.Format("{0:D2}:{1:D2}", resTime / 60, resTime % 60);
                }

                //If there is no remaining time, game is over.
                if (resTime <= 0)
                {
                    setGameState(GameState.GameOver);
                    // Application.Quit();
                }

                //Update recipe
                // if (currentMenu1.endTime <= Time.time)
                // {
                //     updateMenu();
                // }

                if (Time.time - updateTimer > 0.5)
                {
                    showMenus();
                    showPanels();
                    updateSlider();
                    sliderExpire();
                    updateTimer = Time.time;
                }

                break;
            case GameState.GameOver:
                setGameState(GameState.OnHold);
                //score tracking
                //todo add a enum object to indicate current level
                // AnalyticsResult scoreAnalytics = Analytics.CustomEvent("TotalScore",
                //     new Dictionary<string, object>
                //     {
                //         {"level", "tbf"},
                //         {"score", currentScore}
                //     });
                // Debug.Log("analyticResult:" + scoreAnalytics + ", current score: " + currentScore);
                //
                // Dictionary<String, object> popularityResult = new Dictionary<string, object>();
                // recipePopularity.ToList().ForEach(x => popularityResult.Add(x.Key, x.Value));
                // AnalyticsResult popularityAnalytics = Analytics.CustomEvent("Recipe Popularity",
                //     popularityResult);
                // Debug.Log("popularityResult:" + popularityAnalytics);
                // recipePopularity.ToList().ForEach(x => Debug.Log(x.Key + " " + x.Value));


                //stop game--by speed
                GameObject.FindGameObjectWithTag("Player").GetComponent<Rigidbody>().velocity = Vector3.zero;

                if (currentScore >= successScore)
                {
                    successPanel.SetActive(true);
                }
                else
                {
                    failPanel.SetActive(true);
                }

                break;

            case GameState.OnHold:
                break;
        }
    }

    public bool canPickUp(string ingredientName)
    {
        
        return false;
    }

    //see if it can be picked
    public bool canPickUpWithThreeMenusTwoPanels(string ingredientName)
    {
        Boolean isPanel1Busy = panel1.getIsBusy();
        Boolean isPanel2Busy = panel2.getIsBusy();
        int preparingMenuOfPanel1 = panel1.getForWhichMenu();
        int preparingMenuOfPanel2 = panel2.getForWhichMenu();

        //1.first find if the 2 panels are in use
        if (isPanel1Busy && isPanel2Busy)
        {
            // see if someone need and add to it
            // they both need
            if (toBeDoneMenus[preparingMenuOfPanel1].ingredients.Contains(ingredientName) &&
                !panel1.getPickedIngre().Contains(ingredientName) &&
                toBeDoneMenus[preparingMenuOfPanel2].ingredients.Contains(ingredientName) &&
                !panel2.getPickedIngre().Contains(ingredientName))
            {
                //panel1 & panel2 need(ingreName in recipe but not appears in pickedIngreList) simultaneously, give it to the old one
                if (preparingMenuOfPanel1 > preparingMenuOfPanel2)
                {
                    panel2.addIngre(ingredientName);
                }
                else
                {
                    panel1.addIngre(ingredientName);
                }

                finishAMenu();
                return true;
            }

            //only panel1 need
            if (toBeDoneMenus[preparingMenuOfPanel1].ingredients.Contains(ingredientName) &&
                !panel1.getPickedIngre().Contains(ingredientName))
            {
                //panel1 needs
                panel1.addIngre(ingredientName);
                finishAMenu();
                return true;
            }

            //only panel2 need
            if (toBeDoneMenus[preparingMenuOfPanel2].ingredients.Contains(ingredientName) &&
                !panel2.getPickedIngre().Contains(ingredientName))
            {
                //panel2 needs
                panel2.addIngre(ingredientName);
                finishAMenu();
                return true;
            }
        }
        // both p1 & p2 are not busy
        else if (!isPanel1Busy && !isPanel2Busy)
        {
            for (var i = 0; i < toBeDoneMenus.Count; i++)
            {
                if (toBeDoneMenus[i].ingredients.Contains(ingredientName))
                {
                    //if a recipe need the ingre,assign the recipe to panel1
                    panel1.setForWhichMenu(i);
                    panel1.addIngre(ingredientName);
                    return true;
                }
            }
        }
        // one of them is busy
        else
        {
            //如果一闲一忙：忙的需要就给忙的，忙的不需要就看看另两个待做菜单有没有需要这个材料的，有就给闲的分配上，没有就啥也不干
            if (isPanel1Busy)
            {
                //p1 busy
                if (toBeDoneMenus[preparingMenuOfPanel1].ingredients.Contains(ingredientName) &&
                    !panel1.getPickedIngre().Contains(ingredientName))
                {
                    //p1 need
                    panel1.addIngre(ingredientName);
                    finishAMenu();
                    return true;
                }

                //see if it is needed in other 2 ToBeDoneMenus
                for (var i = 0; i < toBeDoneMenus.Count; i++)
                {
                    if (i != preparingMenuOfPanel1)
                    {
                        //escape the one that is being prepared
                        if (toBeDoneMenus[i].ingredients.Contains(ingredientName))
                        {
                            //if a recipe need the ingre,assign the recipe to panel2
                            panel2.setForWhichMenu(i);
                            panel2.addIngre(ingredientName);
                            return true;
                        }
                    }
                }
            }
            else
            {
                //p2 busy
                if (toBeDoneMenus[preparingMenuOfPanel2].ingredients.Contains(ingredientName) &&
                    !panel2.getPickedIngre().Contains(ingredientName))
                {
                    //p2 need
                    panel2.addIngre(ingredientName);
                    finishAMenu();
                    return true;
                }

                //see if it is needed in other 2 ToBeDoneMenus
                for (var i = 0; i < toBeDoneMenus.Count; i++)
                {
                    if (i != preparingMenuOfPanel2)
                    {
                        //escape the one that is being prepared
                        if (toBeDoneMenus[i].ingredients.Contains(ingredientName))
                        {
                            //if a recipe need the ingre,assign the recipe to panel1
                            panel1.setForWhichMenu(i);
                            panel1.addIngre(ingredientName);
                            return true;
                        }
                    }
                }
            }
        }

        // showPanels();
        return false;
    }

    private void addScore(Recipe recipe)
    {
        if (recipe.name == "ChickenSandwich")
        {
            currentScore += 2;
            recipePopularity["ChickenSandwich"]++;
        }
        else if (recipe.name == "Burger")
        {
            currentScore += 3;
            recipePopularity["Burger"]++;
        }
        else if (recipe.name == "SummerPudding")
        {
            currentScore += 4;
            recipePopularity["SummerPudding"]++;
        }
        else if (recipe.name == "HealthyFood")
        {
            currentScore += 5;
            recipePopularity["HealthyFood"]++;
        }
    }
    
    private void initialTimeSilder(Slider timeSlider, float max, float min, float value)
    {
        // timeSlider.maxValue = EXPIRE_TIME;
        // timeSlider.minValue = 0;
        // timeSlider.value = EXPIRE_TIME;
        timeSlider.maxValue = max;
        timeSlider.minValue = min;
        timeSlider.value = value;
    }

    private void initialMenuAndPanel()
    {
        initialTimeSilder(timeSlider1, EXPIRE_TIME, 0, EXPIRE_TIME);
        initialTimeSilder(timeSlider2, EXPIRE_TIME, 0, EXPIRE_TIME);
        initialTimeSilder(timeSlider3, EXPIRE_TIME, 0, EXPIRE_TIME);
        recipe1.SetActive(true);
        recipe2.SetActive(true);
        recipe3.SetActive(true);
        string[] ingredients1 = {"Beef", "Lettuce", "Bread"};
        string[] ingredients2 = {"Chicken", "Lettuce", "Bread"};
        string[] ingredients3 = {"Bread", "Strawberry"};
        string[] ingredients4 = {"Bread", "Strawberry", "Beef"};
        Recipe menu1 = new Recipe(1, "Burger", 30,new List<string>(ingredients1));
        Recipe menu2 = new Recipe(2, "ChickenSandwich", 30,new List<string>(ingredients2));
        Recipe menu3 = new Recipe(3, "SummerPudding", 30,new List<string>(ingredients3));
        Recipe menu4 = new Recipe(4, "HealthyFood", 30,new List<string>(ingredients4));
        easyMenus = new List<Recipe>();
        easyMenus.Add(menu1);
        easyMenus.Add(menu2);
        easyMenus.Add(menu3);
        easyMenus.Add(menu4);
        pickedIngredients1 = new List<string>();
        pickedIngredients2 = new List<string>();
    }

    private void initialToBeDoneMenus()
    {
        toBeDoneMenus = new List<Recipe>();
        toBeDoneMenus.Add(MenuTools.getAMenu(easyMenus));
        toBeDoneMenus.Add(MenuTools.getAMenu(easyMenus));
        toBeDoneMenus.Add(MenuTools.getAMenu(easyMenus));
    }

    private void finishAMenu()
    {
        //if a panel has enough ingre
        if (panel1.getIsBusy() &&
            panel1.getPickedIngre().Count == toBeDoneMenus[panel1.getForWhichMenu()].ingredients.Count)
        {
            addScore(toBeDoneMenus[panel1.getForWhichMenu()]);
            //移除待做
            toBeDoneMenus.RemoveAt(panel1.getForWhichMenu());
            //增加待做
            toBeDoneMenus.Add(MenuTools.getAMenu(easyMenus));
            //重置panel
            panel1.reset();
        }

        if (panel2.getIsBusy() &&
            panel2.getPickedIngre().Count == toBeDoneMenus[panel2.getForWhichMenu()].ingredients.Count)
        {
            addScore(toBeDoneMenus[panel2.getForWhichMenu()]);
            //移除待做
            toBeDoneMenus.RemoveAt(panel2.getForWhichMenu());
            //增加待做
            toBeDoneMenus.Add(MenuTools.getAMenu(easyMenus));
            //重置panel
            panel2.reset();
        }
        // showMenus();
    }

    private void expireMenu(int whichMenu)
    {
        //移除待做
        toBeDoneMenus.RemoveAt(whichMenu);
        //增加待做
        toBeDoneMenus.Add(MenuTools.getAMenu(easyMenus));
        if (panel1.getForWhichMenu() == whichMenu)
        {
            panel1.reset();
        }
        else if (panel2.getForWhichMenu() == whichMenu)
        {
            panel2.reset();
        }
    }

    private void showMenus()
    {
        Recipe[] toBeDoneMenusArray = toBeDoneMenus.ToArray();
        if (toBeDoneMenusArray[0] != null)
        {
            if (toBeDoneMenusArray[0].ingredients.Count == 3)
            {
                ingredientText1.text = toBeDoneMenusArray[0].ingredients[0];
                ingredientText2.text = toBeDoneMenusArray[0].ingredients[1];
                ingredientText3.text = toBeDoneMenusArray[0].ingredients[2];
            }
            else if (toBeDoneMenusArray[0].ingredients.Count == 2)
            {
                ingredientText1.text = toBeDoneMenusArray[0].ingredients[0];
                ingredientText2.text = toBeDoneMenusArray[0].ingredients[1];
                ingredientText3.text = "";
            }
            else if (toBeDoneMenusArray[0].ingredients.Count == 1)
            {
                ingredientText1.text = toBeDoneMenusArray[0].ingredients[0];
                ingredientText2.text = "";
                ingredientText3.text = "";
            }
        }

        if (toBeDoneMenusArray[1] != null)
        {
            if (toBeDoneMenusArray[1].ingredients.Count == 3)
            {
                ingredientText4.text = toBeDoneMenusArray[1].ingredients[0];
                ingredientText5.text = toBeDoneMenusArray[1].ingredients[1];
                ingredientText6.text = toBeDoneMenusArray[1].ingredients[2];
            }
            else if (toBeDoneMenusArray[1].ingredients.Count == 2)
            {
                ingredientText4.text = toBeDoneMenusArray[1].ingredients[0];
                ingredientText5.text = toBeDoneMenusArray[1].ingredients[1];
                ingredientText6.text = "";
            }
            else if (toBeDoneMenusArray[1].ingredients.Count == 1)
            {
                ingredientText4.text = toBeDoneMenusArray[1].ingredients[0];
                ingredientText5.text = "";
                ingredientText6.text = "";
            }
        }

        if (toBeDoneMenusArray[2] != null)
        {
            if (toBeDoneMenusArray[2].ingredients.Count == 3)
            {
                ingredientText7.text = toBeDoneMenusArray[2].ingredients[0];
                ingredientText8.text = toBeDoneMenusArray[2].ingredients[1];
                ingredientText9.text = toBeDoneMenusArray[2].ingredients[2];
            }
            else if (toBeDoneMenusArray[2].ingredients.Count == 2)
            {
                ingredientText7.text = toBeDoneMenusArray[2].ingredients[0];
                ingredientText8.text = toBeDoneMenusArray[2].ingredients[1];
                ingredientText9.text = "";
            }
            else if (toBeDoneMenusArray[2].ingredients.Count == 1)
            {
                ingredientText7.text = toBeDoneMenusArray[2].ingredients[0];
                ingredientText8.text = "";
                ingredientText9.text = "";
            }
        }
    }

    private int showPanelTimes;

    private void showPanels()
    {
        //Debug.Log(showPanelTimes++);
        if (panel1.getPickedIngre().Count == 0)
        {
            currentIngredientText1.text = "";
            currentIngredientText2.text = "";
            currentIngredientText3.text = "";
        }
        else if (panel1.getPickedIngre().Count == 1)
        {
            currentIngredientText1.text = panel1.getPickedIngre()[0];
        }
        else if (panel1.getPickedIngre().Count == 2)
        {
            currentIngredientText1.text = panel1.getPickedIngre()[0];
            currentIngredientText2.text = panel1.getPickedIngre()[1];
        }
        else if (panel1.getPickedIngre().Count == 3)
        {
            currentIngredientText1.text = panel1.getPickedIngre()[0];
            currentIngredientText2.text = panel1.getPickedIngre()[1];
            currentIngredientText3.text = panel1.getPickedIngre()[2];
        }

        if (panel2.getPickedIngre().Count == 0)
        {
            currentIngredientText4.text = "";
            currentIngredientText5.text = "";
            currentIngredientText6.text = "";
        }
        else if (panel2.getPickedIngre().Count == 1)
        {
            currentIngredientText4.text = panel2.getPickedIngre()[0];
        }
        else if (panel2.getPickedIngre().Count == 2)
        {
            currentIngredientText4.text = panel2.getPickedIngre()[0];
            currentIngredientText5.text = panel2.getPickedIngre()[1];
        }
        else if (panel2.getPickedIngre().Count == 3)
        {
            currentIngredientText4.text = panel2.getPickedIngre()[0];
            currentIngredientText5.text = panel2.getPickedIngre()[1];
            currentIngredientText6.text = panel2.getPickedIngre()[2];
        }
    }

    private void updateSlider()
    {
        timeSlider1.value = Time.time - toBeDoneMenus[0].startTime;
        timeSlider2.value = Time.time - toBeDoneMenus[1].startTime;
        timeSlider3.value = Time.time - toBeDoneMenus[2].startTime;
    }

    private void sliderExpire()
    {
        if (timeSlider1.value >= EXPIRE_TIME)
        {
            expireMenu(0);
        }

        if (timeSlider2.value >= EXPIRE_TIME)
        {
            expireMenu(1);
        }

        if (timeSlider2.value >= EXPIRE_TIME)
        {
            expireMenu(2);
        }
    }
}
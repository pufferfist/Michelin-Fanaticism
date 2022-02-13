using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using MenuNameSpace;
using UnityEngine.UI;
using Random = UnityEngine.Random;
using MenuToolsSpace;
using PanelSpace;

public class GameManager : MonoBehaviour
{
    static public GameManager gm;

    public enum GameState
    {
        Playing,
        GameOver
    }

    public GameState gameState;

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
    private List<Menu> easyMenus;
    private List<Menu> toBeDoneMenus;
    private Menu currentMenu1;
    private Menu currentMenu2;
    private Menu currentMenu3;
    private Panel panel1 = new Panel();
    private Panel panel2 = new Panel();

    private List<string> pickedIngredients1;
    private List<string> pickedIngredients2;
    private int currentScore;
    public int resTime = 60;
    public int[] successScore = {10};
    private float gameStartTime;
    public GameObject successPanel;
    public GameObject failPanel;
    private float recipe2UpdateTime;
    private float recipe3UpdateTime;

    private void Start()
    {
        if (gm == null)
            gm = GetComponent<GameManager>();
        currentScore = 0;
        gm.gameState = GameState.Playing;
        initialMenuAndPanel();
        initialToBeDoneMenus();
        showMenus(toBeDoneMenus);
        // updateMenu();
        gameStartTime = Time.time;
        recipe2UpdateTime = gameStartTime;
        recipe3UpdateTime = gameStartTime;
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
                    gameState = GameState.GameOver;
                    // Application.Quit();
                }

                //Update menu
                if (currentMenu1.endTime <= Time.time)
                {
                    updateMenu();
                }

                //Update timesLide for the current menu.
                timeSlider1.value = Time.time - currentMenu1.startTime;
                if (recipe2.activeSelf == true)
                {
                    timeSlider2.value = Time.time - currentMenu2.startTime;
                }
                
                break;
            case GameState.GameOver:
                Debug.Log("Game is Over");
                //Application.Quit();
                GameObject.FindGameObjectWithTag("Player").GetComponent<Rigidbody>().velocity = Vector3.zero;
                //stop game--by speed
                if (currentScore >= successScore[0])
                {
                    successPanel.SetActive(true);
                }
                else
                {
                    failPanel.SetActive(true);
                }

                break;
        }

    }

    public bool canPickUp(string ingredientName)
    {

        if (currentMenu1.ingredients.Contains(ingredientName) && !pickedIngredients1.Contains(ingredientName))
        {
            if (pickedIngredients1.Count == 0)
            {
                currentIngredientText1.text = ingredientName;
            }
            else if (pickedIngredients1.Count == 1)
            {
                currentIngredientText2.text = ingredientName;
            }
            else if (pickedIngredients1.Count == 2)
            {
                currentIngredientText3.text = ingredientName;
            }

            pickedIngredients1.Add(ingredientName);
            if (pickedIngredients1.Count == currentMenu1.ingredients.Count)
            {
                addScore(currentMenu1);
                updateMenu();
            }

            return true;
        }

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
            if (toBeDoneMenus[preparingMenuOfPanel1].ingredients.Contains(ingredientName) &&
                !panel1.getPickedIngre().Contains(ingredientName) &&
                toBeDoneMenus[preparingMenuOfPanel2].ingredients.Contains(ingredientName) &&
                !panel2.getPickedIngre().Contains(ingredientName))
            {
                //panel1 & panel2 needs(ingreName in menu but not appears in pickedIngreList) simultaneously, give it to the old one
                if (preparingMenuOfPanel1 > preparingMenuOfPanel2)
                {
                    panel2.addIngre(ingredientName);
                }
                else
                {
                    panel1.addIngre(ingredientName);
                }
            }
            if (toBeDoneMenus[preparingMenuOfPanel1].ingredients.Contains(ingredientName) &&
                !panel1.getPickedIngre().Contains(ingredientName))
            {
                //panel1 needs
                panel1.addIngre(ingredientName);
            }else if (toBeDoneMenus[preparingMenuOfPanel2].ingredients.Contains(ingredientName) &&
                      !panel2.getPickedIngre().Contains(ingredientName))
            {
                //panel2 needs
                panel2.addIngre(ingredientName);
            }
        }
        else if (!isPanel1Busy && !isPanel2Busy)
        {
            // both p1 & p2 are not busy
            panel1.addIngre(ingredientName);
        }
        else
        {
            // one of them is busy
            if (isPanel1Busy)
            {
                //p1 busy
                if (toBeDoneMenus[preparingMenuOfPanel1].ingredients.Contains(ingredientName) &&
                    !panel1.getPickedIngre().Contains(ingredientName))
                {
                    //p1 need
                    panel1.addIngre(ingredientName);
                }
                else
                {
                    //see if it is needed in other 2 ToBePrepareMenus
                    for (var i = 0; i < toBeDoneMenus.Count; i++)
                    {
                        if (i != preparingMenuOfPanel1)
                        {//escape the one that is being prepared
                            if (toBeDoneMenus[i].ingredients.Contains(ingredientName))
                            {
                                //if a menu need the ingre,assign the menu to panel2
                                panel2.setForWhichMenu(i);
                                panel2.addIngre(ingredientName);
                            }
                        }
                    }
                }
            }
            else
            {
                //p2 busy
            }
        }
        //2.if there is a free one, assign it to a menu according to what ingre is now being picked.
        
        //3.add the ingre to the assigned panel
        //2.1 if they are all busy, 
        // first find if it is still needed
        int needIn1 = MenuTools.getIngreNeedNum(currentMenu1, ingredientName);
        int needIn2 = MenuTools.getIngreNeedNum(currentMenu2, ingredientName);
        int needIn3 = MenuTools.getIngreNeedNum(currentMenu3, ingredientName);
        // int needNum = 2;
        // int needNum = needIn1 + needIn2 + needIn3;
        int haveTotal = getIngreHaveNum(ingredientName);
        Boolean isAdded = false;
        //see if the ingredient is still need:
        //still need
        showMenus(toBeDoneMenus);
        // if (currentMenu1.ingredients.Contains(ingredientName) ||
        //     currentMenu2.ingredients.Contains(ingredientName) && needNum > haveTotal)
        {
            //if add to the first panel
            if (pickedIngredients1.Count == 0)
            {
                currentIngredientText1.text = ingredientName;
                isAdded = true;
            }
            else if (pickedIngredients1.Count == 1 && !pickedIngredients1.Contains(ingredientName))
            {
                currentIngredientText2.text = ingredientName;
                isAdded = true;
            }
            else if (pickedIngredients1.Count == 2 && !pickedIngredients1.Contains(ingredientName))
            {
                currentIngredientText3.text = ingredientName;
                isAdded = true;
            }

            if (isAdded)
            {
                pickedIngredients1.Add(ingredientName);
            }
            //need but first panel cannot be added, add to second
            else
            {
                if (pickedIngredients2.Count == 0)
                {
                    currentIngredientText4.text = ingredientName;
                }
                else if (pickedIngredients2.Count == 1)
                {
                    currentIngredientText5.text = ingredientName;
                }
                else if (pickedIngredients2.Count == 2)
                {
                    currentIngredientText6.text = ingredientName;
                }

                pickedIngredients2.Add(ingredientName);
            }
            finishAMenu();
            return true;
        }

        return false;
    }

    private void addScore(Menu menu)
    {
        if (menu.name == "ChickenSandwich")
        {
            currentScore += 2;
        }
        else if (menu.name == "Burger")
        {
            currentScore += 3;
        }
        else if (menu.name == "SummerPudding")
        {
            currentScore += 4;
        }
        else if (menu.name == "HealthyFood")
        {
            currentScore += 5;
        }
    }

    private void updateMenu()
    {
        timeSlider1.value = EXPIRE_TIME;
        pickedIngredients1 = new List<string>();
        //clear panel1
        currentIngredientText1.text = "";
        currentIngredientText2.text = "";
        currentIngredientText3.text = "";
        //clear menu1
        ingredientText1.text = "";
        ingredientText2.text = "";
        ingredientText3.text = "";
        //select a menu to show
        currentMenu1 = MenuTools.getAMenu(easyMenus);
        //set up menu expiration time
        currentMenu1.startTime = Time.time;
        currentMenu1.endTime = Time.time + EXPIRE_TIME;
        //show the menu
        if (currentMenu1.ingredients.Count == 3)
        {
            ingredientText1.text = currentMenu1.ingredients[0];
            ingredientText2.text = currentMenu1.ingredients[1];
            ingredientText3.text = currentMenu1.ingredients[2];
        }
        else if (currentMenu1.ingredients.Count == 2)
        {
            ingredientText1.text = currentMenu1.ingredients[0];
            ingredientText2.text = currentMenu1.ingredients[1];
        }
        else if (currentMenu1.ingredients.Count == 1)
        {
            ingredientText1.text = currentMenu1.ingredients[0];
        }
    }

    //update which menu
    // private void updateWhichMenu(int whichMenu)
    // {
    //     if (whichMenu == 1)
    //     {
    //         updateMenu();
    //     }
    //     else if (whichMenu == 2)
    //     {
    //         if (recipe2.activeSelf == false)
    //         {
    //             recipe2.SetActive(true);
    //         }
    //
    //         currentMenu2 = MenuTools.getAMenu(easyMenus);
    //         setUpMenu(currentMenu2);
    //     }
    //     else if (whichMenu == 3)
    //     {
    //         if (recipe3.activeSelf == false)
    //         {
    //             recipe3.SetActive(true);
    //         }
    //
    //         currentMenu3 = MenuTools.getAMenu(easyMenus);
    //         setUpMenu(currentMenu3);
    //     }
    // }
    

    //get How many Ingredients we have
    private int getIngreHaveNum(String ingredientName)
    {
        int haveNum = 0;
        if (pickedIngredients1 != null)
        {
            for (var i = 0; i < pickedIngredients1.Count; i++)
            {
                if (pickedIngredients1[i] == ingredientName)
                {
                    haveNum++;
                }
            }
        }

        if (pickedIngredients2 != null)
        {
            for (var i = 0; i < pickedIngredients2.Count; i++)
            {
                if (pickedIngredients2[i] == ingredientName)
                {
                    haveNum++;
                }
            }
        }

        return haveNum;
    }

    // private void setUpMenu(Menu menu)
    // {
    //     //set up menu expiration time
    //     menu.startTime = Time.time;
    //     menu.endTime = Time.time + EXPIRE_TIME;
    //     //show the menu
    //     if (menu == currentMenu1)
    //     {
    //         timeSlider1.value = EXPIRE_TIME;
    //         if (menu.ingredients.Count == 3)
    //         {
    //             ingredientText1.text = menu.ingredients[0];
    //             ingredientText2.text = menu.ingredients[1];
    //             ingredientText3.text = menu.ingredients[2];
    //         }
    //         else if (menu.ingredients.Count == 2)
    //         {
    //             ingredientText1.text = menu.ingredients[0];
    //             ingredientText2.text = menu.ingredients[1];
    //         }
    //         else if (menu.ingredients.Count == 1)
    //         {
    //             ingredientText1.text = menu.ingredients[0];
    //         }
    //     }
    //     else if (menu == currentMenu2)
    //     {
    //         timeSlider2.value = EXPIRE_TIME;
    //         if (menu.ingredients.Count == 3)
    //         {
    //             ingredientText4.text = menu.ingredients[0];
    //             ingredientText5.text = menu.ingredients[1];
    //             ingredientText6.text = menu.ingredients[2];
    //         }
    //         else if (menu.ingredients.Count == 2)
    //         {
    //             ingredientText4.text = menu.ingredients[0];
    //             ingredientText5.text = menu.ingredients[1];
    //         }
    //         else if (menu.ingredients.Count == 1)
    //         {
    //             ingredientText4.text = menu.ingredients[0];
    //         }
    //     }
    //     else if (menu == currentMenu3)
    //     {
    //         timeSlider3.value = EXPIRE_TIME;
    //         if (menu.ingredients.Count == 3)
    //         {
    //             ingredientText7.text = menu.ingredients[0];
    //             ingredientText8.text = menu.ingredients[1];
    //             ingredientText9.text = menu.ingredients[2];
    //         }
    //         else if (menu.ingredients.Count == 2)
    //         {
    //             ingredientText7.text = menu.ingredients[0];
    //             ingredientText8.text = menu.ingredients[1];
    //         }
    //         else if (menu.ingredients.Count == 1)
    //         {
    //             ingredientText7.text = menu.ingredients[0];
    //         }
    //     }
    //
    //     menu.startTime = Time.time;
    //     menu.endTime = Time.time + EXPIRE_TIME;
    // }

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
        Menu menu1 = new Menu(1, "Burger", new List<string>(ingredients1));
        Menu menu2 = new Menu(2, "ChickenSandwich", new List<string>(ingredients2));
        Menu menu3 = new Menu(3, "SummerPudding", new List<string>(ingredients3));
        Menu menu4 = new Menu(4, "HealthyFood", new List<string>(ingredients4));
        easyMenus = new List<Menu>();
        easyMenus.Add(menu1);
        easyMenus.Add(menu2);
        easyMenus.Add(menu3);
        easyMenus.Add(menu4);
        pickedIngredients1 = new List<string>();
        pickedIngredients2 = new List<string>();
    }

    private void initialToBeDoneMenus()
    {
        toBeDoneMenus = new List<Menu>();
        toBeDoneMenus.Add(MenuTools.getAMenu(easyMenus));
        toBeDoneMenus.Add(MenuTools.getAMenu(easyMenus));
        toBeDoneMenus.Add(MenuTools.getAMenu(easyMenus));
    }

    private void finishAMenu()
    {
        if (pickedIngredients1.Count == currentMenu1.ingredients.Count)
        {
            addScore(currentMenu1);
            // updateWhichMenu(1);
        }

        if (pickedIngredients2.Count == currentMenu2.ingredients.Count)
        {
            addScore(currentMenu2);
            recipe2.SetActive(false);
            currentIngredientText4.text = "";
            currentIngredientText5.text = "";
            currentIngredientText6.text = "";
        }
    }

    private void showMenus(List<Menu> toBeDoneMenus)
    {
        Menu[] toBeDoneMenusArray = toBeDoneMenus.ToArray();
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
}
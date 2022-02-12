using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using MenuNameSpace;
using UnityEngine.UI;
using Random = UnityEngine.Random;

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
    public GameObject panel1;
    public GameObject panel2;
    private const float EXPIRE_TIME = 30.0f;
    private List<Menu> easyMenus;
    private List<Menu> hardMenus;
    private Menu currentMenu1;
    private Menu currentMenu2;
    private Menu currentMenu3;
    private List<string> pickedIngredients1;
    private List<string> pickedIngredients2;
    private int currentScore;
    private int resTime=60;
    private float gameStartTime;
    private float recipe2UpdateTime;
    private float recipe3UpdateTime;
    private void Start()
    {
        if (gm == null) 
            gm = GetComponent<GameManager>();
        currentScore = 0;
        gm.gameState = GameState.Playing;
        timeSlider1.maxValue = EXPIRE_TIME;
        timeSlider1.minValue = 0;
        timeSlider1.value = EXPIRE_TIME;
        recipe1.SetActive(true);
        recipe2.SetActive(false);
        recipe3.SetActive(false);
        panel1.SetActive(true);
        panel2.SetActive(true);
        string[] ingredients1 = {"Beef", "Lettuce", "Bread"};
        string[] ingredients2 = {"Chicken", "Lettuce", "Bread"};
        string[] ingredients3 = {"Bread", "Strawberry"};
        string[] ingredients4 = {"Bread", "Strawberry","Beef"};
        Menu menu1 = new Menu(1, "Burger", new List<string>(ingredients1));
        Menu menu2 = new Menu(2, "ChickenSandwich", new List<string>(ingredients2));
        Menu menu3 = new Menu(3, "SummerPudding", new List<string>(ingredients3));
        Menu menu4 = new Menu(3, "HealthyFood", new List<string>(ingredients4));
        easyMenus = new List<Menu>();
        easyMenus.Add(menu1);
        easyMenus.Add(menu2);
        easyMenus.Add(menu3);
        hardMenus = new List<Menu>();
        hardMenus.Add(menu4);
        pickedIngredients1 = new List<string>();
        pickedIngredients2 = new List<string>();
        updateMenu();
        gameStartTime = Time.time;
        recipe2UpdateTime = gameStartTime;
        recipe3UpdateTime = gameStartTime;
    }

    private void Update()
    {
        switch (gameState)
        {
            case GameState.Playing:
                
                scoreText.text = currentScore+"";
                
                //Update every second and display the remaining time.
                if ((Time.time - gameStartTime) >= 1)
                {
                    gameStartTime = Time.time;
                    resTime--;
                    timeText.text = string.Format("{0:D2}:{1:D2}", resTime/ 60, resTime % 60);
                    
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
                if (Time.time - recipe2UpdateTime > 3f && recipe2.activeSelf==false)
                {
                    recipe2.SetActive(true);
                    currentMenu2 = getAMenu();
                    setUpMenu(currentMenu2);
                    recipe2UpdateTime = Time.time;
                }
                break;
            case GameState.GameOver:
                Debug.Log("Game is Over");
                Application.Quit();
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
            else if(pickedIngredients1.Count==2)
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
    
    public bool canPickUpWithThreeMenusTwoPanels(string ingredientName)
    {
        // first find if it is still needed
        int needIn1 = getIngreNeedNum(currentMenu1, ingredientName);
        int needIn2 = getIngreNeedNum(currentMenu2, ingredientName);
        int needIn3 = getIngreNeedNum(currentMenu3, ingredientName);
        // int needNum = 2;
        int needNum = needIn1 + needIn2 + needIn3;
        int haveTotal = getIngreHaveNum(ingredientName);
        Boolean isAdded = false;
        //still need
        if (currentMenu1.ingredients.Contains(ingredientName) ||currentMenu2.ingredients.Contains(ingredientName) && needNum > haveTotal)
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
            else if(pickedIngredients1.Count==2 && !pickedIngredients1.Contains(ingredientName))
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
                else if(pickedIngredients2.Count==2)
                {
                    currentIngredientText6.text = ingredientName;
                }
                pickedIngredients2.Add(ingredientName);
            }
            
            if (pickedIngredients1.Count == currentMenu1.ingredients.Count)
            {
                addScore(currentMenu1);
                updateWhichMenu(1);
            }

            if (pickedIngredients2.Count == currentMenu2.ingredients.Count)
            {
                addScore(currentMenu2);
                recipe2.SetActive(false);
                currentIngredientText4.text = "";
                currentIngredientText5.text = "";
                currentIngredientText6.text = "";
            }
            return true;
        }
        return false;
    }

    private void addScore(Menu menu)
    {
        if (menu.name == "ChickenSandwich")
        {
            currentScore += 2;
        } else if (menu.name == "Burger")
        {
            currentScore += 3;
        }else if (menu.name == "SummerPudding")
        {
            currentScore += 4;
        }else if (menu.name == "HealthyFood")
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
        currentMenu1 = getAMenu();
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
        else if(currentMenu1.ingredients.Count==1)
        {
            ingredientText1.text = currentMenu1.ingredients[0];
        }
    }
    //update which menu
    private void updateWhichMenu(int whichMenu)
    {
        if (whichMenu == 1) {
            updateMenu();
        }
        else if (whichMenu == 2)
        {
            if (recipe2.activeSelf == false)
            {
                recipe2.SetActive(true);
            }
            currentMenu2 = getAMenu();
            setUpMenu(currentMenu2);
        }else if (whichMenu == 3)
        {
            if (recipe3.activeSelf == false)
            {
                recipe3.SetActive(true);
            }
            currentMenu3 = getAMenu();
            setUpMenu(currentMenu3);
        }
    }
    //get How many this ingredients are needed
    private int getIngreNeedNum(Menu menu, String ingredientName)
    {
        int needNum = 0;
        if (menu != null)
        {
            for (var i = 0; i < menu.ingredients.Count; i++)
            {
                if (ingredientName == menu.ingredients[i])
                {
                    needNum++;
                }
            }
        }

        return needNum;
    }
    
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

    private Menu getAMenu()
    {
        int isHard = Random.Range(0, 2);
        if (isHard == 1)
        {
            int index = Random.Range(0, hardMenus.Count);
            return hardMenus[index];
        }
        else
        {
            int index = Random.Range(0, easyMenus.Count);
            return easyMenus[index];
        }
    }

    private void setUpMenu(Menu menu)
    {
        //set up menu expiration time
        menu.startTime = Time.time;
        menu.endTime = Time.time + EXPIRE_TIME;
        //show the menu
        if ( menu == currentMenu1 )
        {
            timeSlider1.value = EXPIRE_TIME;
            if (menu.ingredients.Count == 3)
            {
                ingredientText1.text = menu.ingredients[0];
                ingredientText2.text = menu.ingredients[1];
                ingredientText3.text = menu.ingredients[2];
            }
            else if (menu.ingredients.Count == 2)
            {
                ingredientText1.text = menu.ingredients[0];
                ingredientText2.text = menu.ingredients[1];
            }
            else if(menu.ingredients.Count==1)
            {
                ingredientText1.text = menu.ingredients[0];
            }
        }else if (menu == currentMenu2)
        {
            timeSlider2.value = EXPIRE_TIME;
            if (menu.ingredients.Count == 3)
            {
                ingredientText4.text = menu.ingredients[0];
                ingredientText5.text = menu.ingredients[1];
                ingredientText6.text = menu.ingredients[2];
            }
            else if (menu.ingredients.Count == 2)
            {
                ingredientText4.text = menu.ingredients[0];
                ingredientText5.text = menu.ingredients[1];
            }
            else if(menu.ingredients.Count==1)
            {
                ingredientText4.text = menu.ingredients[0];
            }
        }else if (menu == currentMenu3)
        {
            timeSlider3.value = EXPIRE_TIME;
            if (menu.ingredients.Count == 3)
            {
                ingredientText7.text = menu.ingredients[0];
                ingredientText8.text = menu.ingredients[1];
                ingredientText9.text = menu.ingredients[2];
            }
            else if (menu.ingredients.Count == 2)
            {
                ingredientText7.text = menu.ingredients[0];
                ingredientText8.text = menu.ingredients[1];
            }
            else if(menu.ingredients.Count==1)
            {
                ingredientText7.text = menu.ingredients[0];
            }
        }
        menu.startTime = Time.time;
        menu.endTime = Time.time + EXPIRE_TIME;
    }
    
}
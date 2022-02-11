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
    public Text ingredientText1;
    public Text ingredientText2;
    public Text ingredientText3;
    public Text ingredientText4;
    public Text ingredientText5;
    public Text ingredientText6;
    public Text ingredientText7;
    public Text ingredientText8;
    public Text ingredientText9;
    public Text timeText;
    public Slider timeSlider1;
    public Slider timeSlider2;
    public Slider timeSlider3;
    public Text currentIngredientText1;
    public Text currentIngredientText2;
    public Text currentIngredientText3;
    private const float EXPIRE_TIME = 30.0f;
    private List<Menu> easyMenus;
    private List<Menu> hardMenus;
    private Menu currentMenu;
    private List<string> pickedIngredients;
    private int currentScore;
    private int resTime=60;
    private float gameStartTime;
    private void Start()
    {
        if (gm == null) 
            gm = GetComponent<GameManager>();
        currentScore = 0;
        gm.gameState = GameState.Playing;
        timeSlider1.maxValue = EXPIRE_TIME;
        timeSlider1.minValue = 0;
        timeSlider1.value = EXPIRE_TIME;
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
        pickedIngredients = new List<string>();
        updateMenu();
        gameStartTime = Time.time;
        
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
                if (currentMenu.endTime <= Time.time) 
                {
                    updateMenu();
                }
                
                //Update timesLide for the current menu.
                timeSlider1.value = Time.time - currentMenu.startTime;
                break;
            case GameState.GameOver:
                Debug.Log("Game is Over");
                Application.Quit();
                break;
        }
        
    }
    
    public bool canPickUp(string ingredientName)
    {
        
        if (currentMenu.ingredients.Contains(ingredientName) && !pickedIngredients.Contains(ingredientName))
        {
            if (pickedIngredients.Count == 0)
            {
                currentIngredientText1.text = ingredientName;
            }
            else if (pickedIngredients.Count == 1)
            {
                currentIngredientText2.text = ingredientName;
            }
            else if(pickedIngredients.Count==2)
            {
                currentIngredientText3.text = ingredientName;
            }
            pickedIngredients.Add(ingredientName);
            if (pickedIngredients.Count == currentMenu.ingredients.Count)
            {
                addScore(currentMenu);
                updateMenu();
            }
            return true;
        }
        return false;
    }

    private void addScore(Menu menu)
    {
        Boolean isFind = false;
        for (var i = 0; i < easyMenus.Count; i++)
        {
            if (menu.name == easyMenus[i].name)
            {
                currentScore++;
                isFind = true;
                break;
            }
        }
        if (!isFind)
        {
            for (var i = 0; i < hardMenus.Count; i++)
            {
                if (menu.name == hardMenus[i].name)
                {
                    currentScore+=3; 
                }
            }
        }

    }
    
    private void updateMenu()
    {
        timeSlider1.value = EXPIRE_TIME;
        pickedIngredients = new List<string>();
        currentIngredientText1.text = "";
        currentIngredientText2.text = "";
        currentIngredientText3.text = "";
        ingredientText1.text = "";
        ingredientText2.text = "";
        ingredientText3.text = "";
        int isHard = Random.Range(0, 2);
        if (isHard == 1)
        {
            int index = Random.Range(0, hardMenus.Count);
            currentMenu = hardMenus[index];
        }
        else
        {
            int index = Random.Range(0, easyMenus.Count);
            currentMenu = easyMenus[index];
        }
        currentMenu.startTime = Time.time;
        currentMenu.endTime = Time.time + EXPIRE_TIME;
        if (currentMenu.ingredients.Count == 3)
        {
            ingredientText1.text = currentMenu.ingredients[0];
            ingredientText2.text = currentMenu.ingredients[1];
            ingredientText3.text = currentMenu.ingredients[2];
        }
        else if (currentMenu.ingredients.Count == 2)
        {
            ingredientText1.text = currentMenu.ingredients[0];
            ingredientText2.text = currentMenu.ingredients[1];
        }
        else if(currentMenu.ingredients.Count==1)
        {
            ingredientText1.text = currentMenu.ingredients[0];
        }
    }
}
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
    public Text timeText;
    public Slider timeSlider;
    public Text currentIngredientText1;
    public Text currentIngredientText2;
    public Text currentIngredientText3;
    private const float EXPIRE_TIME = 30.0f;
    private List<Menu> menus;
    private Menu currentMenu;
    private List<string> pickedIngredients;
    private int currentScore;
    private int resTime=180;
    private float gameStartTime;
    private void Start()
    {
        if (gm == null) 
            gm = GetComponent<GameManager>();
        currentScore = 0;
        gm.gameState = GameState.Playing;
        timeSlider.maxValue = EXPIRE_TIME;
        timeSlider.minValue = 0;
        timeSlider.value = EXPIRE_TIME;
        string[] ingredients1 = {"Beef", "Lettuce", "Bread"};
        string[] ingredients2 = {"Chicken", "Lettuce", "Bread"};
        string[] ingredients3 = {"Bread", "Strawberry"};
        Menu menu1 = new Menu(1, "Burger", new List<string>(ingredients1));
        Menu menu2 = new Menu(2, "ChickenSandwich", new List<string>(ingredients2));
        Menu menu3 = new Menu(3, "SummerPudding", new List<string>(ingredients3));
        menus = new List<Menu>();
        menus.Add(menu1);
        menus.Add(menu2);
        menus.Add(menu3);
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
                }
                
                //Update menu
                if (currentMenu.endTime <= Time.time) 
                {
                    updateMenu();
                }
                
                //Update timesLide for the current menu.
                timeSlider.value = Time.time - currentMenu.startTime;
                break;
            case GameState.GameOver:
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
                addScore();
                updateMenu();
            }
            return true;
        }
        return false;
    }

    private void addScore()
    {
        currentScore++;
    }
    
    private void updateMenu()
    {
        timeSlider.value = EXPIRE_TIME;
        pickedIngredients = new List<string>();
        currentIngredientText1.text = "";
        currentIngredientText2.text = "";
        currentIngredientText3.text = "";
        int index = Random.Range(0, menus.Count);
        currentMenu = menus[index];
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
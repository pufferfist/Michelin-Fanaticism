using System;
using System.Collections.Generic;
using System.Linq;
using DefaultNamespace;
using UnityEngine;
using MenuNameSpace;
using UnityEngine.Analytics;

public class GameManager : MonoBehaviour
{
    public static GameManager gm;

    public GameState gameState;

    public MainCharacter character;

    public GameObject ui;
    private UIHandler uiHandler;
    private MenuHandler menuHandler;
    private CollectedHandler collectedHandler;
    
    private int currentActiveBag;//indicates which bag is currently used  0: the left one 1: the right one
    
    private int currentScore;
    public int resTime = 60;
    public int lives = 5;
    public int successScore;
    private float gameProcessingTimer;//game timer last update time
    
    public GameObject successPanel;
    public GameObject failPanel;

    //data tracking
    private Dictionary<String, int> recipePopularity;

    private void setGameState(GameState state)
    {
        gameState = state;
        character.changeState(state);
    }

    private void Start()
    {
        setGameState(GameState.Playing);
        if (gm == null)
            gm = GetComponent<GameManager>();
        uiHandler = new UIHandler(ui);
        menuHandler = new MenuHandler(uiHandler);
        collectedHandler = new CollectedHandler(uiHandler);
        
        currentScore = 0;
        currentActiveBag = 0;

        uiHandler.updateScore(currentScore);
        uiHandler.updateTime(resTime);
        uiHandler.updateLives(lives);
        
        gameProcessingTimer = Time.time;

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
                if (Input.GetKeyUp(KeyCode.S))
                {
                    currentActiveBag ^= 1;
                    uiHandler.switchBag(currentActiveBag);
                }
                
                if (Input.GetKeyUp(KeyCode.X))
                {
                    collectedHandler.drop(currentActiveBag);
                }

                uiHandler.updateScore(currentScore);

                //Update every second and display the remaining time.
                if ((Time.time - gameProcessingTimer) >= 1)
                {
                    gameProcessingTimer = Time.time;
                    resTime--;
                    uiHandler.updateTime(resTime);
                }

                menuHandler.updateRecipes();
                
                //If there is no remaining time, game is over.
                if (resTime <= 0)
                {
                    setGameState(GameState.GameOver);
                }

                break;
            case GameState.GameOver:
                setGameState(GameState.OnHold);
                //score tracking
                //todo add a enum object to indicate current level
                AnalyticsResult scoreAnalytics = Analytics.CustomEvent("TotalScore",
                    new Dictionary<string, object>
                    {
                        {"level", "tbf"},
                        {"score", currentScore}
                    });
                Debug.Log("analyticResult:" + scoreAnalytics + ", current score: " + currentScore);
                
                Dictionary<String, object> popularityResult = new Dictionary<string, object>();
                recipePopularity.ToList().ForEach(x => popularityResult.Add(x.Key, x.Value));
                AnalyticsResult popularityAnalytics = Analytics.CustomEvent("Recipe Popularity",
                    popularityResult);
                Debug.Log("popularityResult:" + popularityAnalytics);
                recipePopularity.ToList().ForEach(x => Debug.Log(x.Key + " " + x.Value));


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


    public bool canPickUp(string ingredient)
    {
        Stack<String> pickUp = collectedHandler.pickUp(currentActiveBag,ingredient);//collected handler will update the ui
        if (pickUp != null)
        {
            Recipe finish = menuHandler.checkFinish(pickUp);//menu handler will update the ui
            if (finish != null)
            {
                collectedHandler.finish(currentActiveBag);
                currentScore += finish.score;
                uiHandler.updateScore(currentScore);
                recipePopularity[finish.name]++;
            }

            return true;
        }
        else
        {
            //todo prompt player that bag is full
        }
        return false;
    }

    public void looseLife()
    {
        if (lives == 0)
        {
            gameState = GameState.GameOver;
            return;
        }
        lives -= 1;
        uiHandler.updateLives(lives);
    }

    public void addLife()
    {
        lives += 1;
        uiHandler.updateLives(lives);
    }
}
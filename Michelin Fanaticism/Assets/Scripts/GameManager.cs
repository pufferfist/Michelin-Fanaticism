using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DefaultNamespace;
using UnityEngine;
using MenuNameSpace;
using UnityEngine.Analytics;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager gm;
    private LevelConfig levelConfig;

    public GameState gameState;

    public MainCharacter character;

    public GameObject ui;
    private UIHandler uiHandler;
    private MenuHandler menuHandler;
    private CollectedHandler collectedHandler;
    private AnalyticsHandler analyticsHandler;
    
    private int currentActiveBag;//indicates which bag is currently used  0: the left one 1: the right one
    
	public int currentLevel;
    private int currentScore;
    public int resTime;
    public int lives;
    public int successScore;
    private float gameProcessingTimer;//game timer last update time
    private float lastFrameTime;
    private float gameTimer;//a fake timer. Time will pause when game pauses

    public GameObject successPanel;
    public GameObject failPanel;

    public GameObject stuPanel;
    public static GameManager instance;

    void Awake(){
        instance = this;
    }

    //data tracking
    private Dictionary<String, int> recipePopularity;
    private Dictionary<int, float> trackPopularity;
    private int barrierEncountered;
    private float lastSwitch;//the time player last switch track
    private int currentTrack;

    private void setGameState(GameState state)
    {
        gameState = state;
        character.changeState(state);
    }

    private void Start()
    {
        if (gm == null)
            gm = GetComponent<GameManager>();
        GameObject  configReader = GameObject.FindGameObjectsWithTag("ConfigReader")[0];
        ConfigReader cr = configReader.GetComponent<ConfigReader>();
        levelConfig = cr.configResult;
        ImageHelper.init(levelConfig);
        uiHandler = new UIHandler(ui,levelConfig);
        menuHandler = new MenuHandler(uiHandler, levelConfig);
        collectedHandler = new CollectedHandler(uiHandler,levelConfig);
		uiHandler.setCollectedHandler(collectedHandler);
        analyticsHandler = new AnalyticsHandler(levelConfig);
        currentLevel = levelConfig.Level;
        currentScore = 0;
        currentActiveBag = 0;

        successScore = levelConfig.SuccessScore;
        resTime = levelConfig.LevelTime;
        lives = levelConfig.MaxHealth;
        
        uiHandler.updateScore(currentScore);
        uiHandler.updateTime(resTime);
        if (currentLevel >= 4)
        {
            uiHandler.updateLives(lives);
        }
        gameTimer = Time.time;
        gameProcessingTimer = gameTimer;
        lastFrameTime = gameTimer;
        //init data tracking variables
        
        RecipeInfo[] levelRecipes = levelConfig.Recipes;
        recipePopularity = new Dictionary<string, int>{};
        foreach (RecipeInfo recipeInfo in levelRecipes)
        {
            recipePopularity.Add(recipeInfo.Name,0);
        }
        
        trackPopularity = new Dictionary<int, float>{{0,0},{1,0},{2,0}};
        barrierEncountered = 0;
        currentTrack = 1;
        lastSwitch = gameTimer;
        setGameState(GameState.OnHold);
    }

    private void Update()
    {
        switch (gameState)
        {
            case GameState.Playing:
                gameTimer += Time.time - lastFrameTime;
                
                float dv = Input.GetAxis("Mouse ScrollWheel");
                if (dv > 0&&currentLevel>=3&&currentActiveBag==1)
                {
                    currentActiveBag = 0;
                    uiHandler.switchBag(currentActiveBag);
                }
                if (dv < 0&&currentLevel>=3&&currentActiveBag==0)
                {
                    currentActiveBag = 1;
                    uiHandler.switchBag(currentActiveBag);
                }
                
                // if (Input.GetKeyUp(KeyCode.Space)||Input.GetKeyUp(KeyCode.S))
                // {
                //     collectedHandler.drop(currentActiveBag);
                // }

                uiHandler.updateScore(currentScore);

                //Update every second and display the remaining time.
                if ((gameTimer - gameProcessingTimer) >= 1)
                {
                    gameProcessingTimer = gameTimer;
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
                analyticsHandler.upload(currentScore,recipePopularity,barrierEncountered,trackPopularity,currentScore >= successScore);
                foreach (KeyValuePair<int,float> statistic in trackPopularity)
                {
                    Debug.Log(statistic.Key+": "+statistic.Value);
                }
                
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
                menuHandler.updateOnHoldTimer();
                break;
        }

        lastFrameTime = Time.time;
    }


    public bool canPickUp(string ingredient)
    {
        List<String> pickUp = collectedHandler.pickUp(currentActiveBag,ingredient);//collected handler will update the ui
        if (pickUp != null)
        {
            finish(pickUp);
            return true;
        }
        else
        {
            //todo prompt player that bag is full
        }
        return false;
    }

    public void finish(List<String>collectedList)
    {
        Recipe finish = menuHandler.checkFinish(collectedList);//menu handler will update the ui
        if (finish != null)
        {
            collectedHandler.finish(currentActiveBag);
            StartCoroutine(menuHandler.Fadeout(finish, done => {
                if(done != null && done) {
                    currentScore += finish.score;
                    uiHandler.updateScore(currentScore);
                    AudioSource[] audioSources = character.GetComponents<AudioSource>();
                    AudioSource audioSource = audioSources[1];
                    audioSource.Play();
                    recipePopularity[finish.name]++;
                }
            }));
        }
    }

    public void looseLife()
    {
        lives -= 1;
        uiHandler.updateLives(lives);
        barrierEncountered++;
        
        if (lives == 0)
        {
            setGameState(GameState.GameOver);
            return;
        }
    }

    public void addLife()
    {
        if (lives>=levelConfig.MaxHealth)
        {
            return;
        }
        lives += 1;
        uiHandler.updateLives(lives);
    }

	public void speedChange(int delta)
    {
        character.changeSpeed(delta);
    }
    
    public void StartGame(){
        stuPanel.SetActive(false);
        setGameState(GameState.Playing);
    }

    public void switchTrack(bool direction)
    {
        if (direction&&currentTrack>0)
        {
            trackPopularity[currentTrack] += gameTimer - lastSwitch;
            lastSwitch = gameTimer;
            currentTrack--;
        }
        else if(!direction&&currentTrack<2)
        {
            trackPopularity[currentTrack] += gameTimer - lastSwitch;
            lastSwitch = gameTimer;
            currentTrack++;
        }
    }
}
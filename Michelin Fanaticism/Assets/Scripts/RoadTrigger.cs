using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class IngredientSelector
{
    LevelConfig m_configObj;
    Dictionary<string, int> indexMap;
    List<int> randomMap;
    public void init(int level,GameObject[] ingredientSourceObject){
      
          GameObject  configReader = GameObject.FindGameObjectsWithTag("ConfigReader")[0];
          ConfigReader cr = configReader.GetComponent<ConfigReader>();
       
          LevelConfig configObj = cr.configResult;
          indexMap = new Dictionary<string, int>();
          randomMap = new List<int>();
          for(int i=0;i<ingredientSourceObject.Length;i++){
              indexMap[ingredientSourceObject[i].name] = i;
          }
          for(int i=0;i<configObj.IngredientsWeights.Length;i++){
              for(int j=0;j<configObj.IngredientsWeights[i].Weight;j++){
                    randomMap.Add(indexMap[configObj.IngredientsWeights[i].Name]);
              }
          }
    }
    public int nextIngredientTypeIndex(){
        return randomMap[Random.Range(0,randomMap.Count-1)];
    }
   
}
public class RoadTrigger : MonoBehaviour
{
    
    public GameObject[] ingredientSourceObject;
    public int roadLength = 600;
    public int roadWidth = 30;
    // ingredientInterval
    public int ingredientInterval = 20;
    // for the firstRoad
    public bool  startToCreate = false;

    // ingredient type guarantee
    // bool  gachaGuarantee = true;

    GameObject[] roadObject;  
    GameObject triggerObject;
    GameObject curRoadObject;  
    GameObject nextRoadObject;
    List<GameObject> ingredientDynamicObject;
    List<GameObject> ingredientDynamicObjectYoung;
    Dictionary<int , bool> ingredientMap;

    public double densityRatio = 0.01;
    int maxIngredientId = 2147483647;
    public int roadWidthCount = 3;
    void Start()
    {

        Debug.Log("OnTriggerStart");
        ingredientDynamicObject =new List<GameObject> ();
        ingredientDynamicObjectYoung = new List<GameObject> ();
        roadObject =  GameObject.FindGameObjectsWithTag("OriginRoad");
        
        triggerObject =  GameObject.FindGameObjectsWithTag("RoadTrigger")[0];
        if(roadObject[0].transform.position.x<roadObject[1].transform.position.x){
            CreateIngredients(roadObject[1]);
        }else{
            CreateIngredients(roadObject[0]);
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
    bool CheckMoveRoad(){
        if(triggerObject.transform.position.x < curRoadObject.transform.position.x-roadLength/2 ){
            return false;
        }
        return true;
    }
    // extend road and trigger
    void MoveRoad(){
        nextRoadObject.transform.position = new Vector3(nextRoadObject.transform.position.x+roadLength*2, nextRoadObject.transform.position.y, nextRoadObject.transform.position.z);
        triggerObject.transform.position = new Vector3(triggerObject.transform.position.x+roadLength, triggerObject.transform.position.y, triggerObject.transform.position.z);
    }
    // clear old road ingredient
     void ClearIngredients(){
        foreach (GameObject obj in ingredientDynamicObject)
        {
            Destroy(obj);
        }
        ingredientDynamicObject.Clear();
         foreach (GameObject obj in ingredientDynamicObjectYoung)
        {
            ingredientDynamicObject.Add(obj);
        }
        ingredientDynamicObjectYoung.Clear();
    }
    void GetIngredientsConfig(int level){
        // LevelConfigObj configObj = ConfigReader.LoadJsonFromFile<LevelConfigObj>(level);
     
        
    }
    // create ingredients randomly
    void CreateIngredients(GameObject road){
        IngredientSelector ingredientSelector = new IngredientSelector();
        ingredientSelector.init(1,ingredientSourceObject);
        // Debug.Log("create ingredient for "+road.name);


        int lengthMapCount = (roadLength/ingredientInterval)+1;
        // total ingrident counts need to be generate
        // control by densityRatio and ingredientInterval
        int ingredientCounts = (int)( densityRatio * lengthMapCount * roadWidthCount);
        
        
        Debug.Log("ingredientCounts:"+ingredientCounts.ToString());
        Debug.Log("lengthMapCount:"+lengthMapCount.ToString());

        // map to check overlapped
        ingredientMap = new Dictionary<int , bool>();
        List<int> staticWidth = new List<int>{-5,0,5};

        for(int i=0;i<ingredientCounts;i++){
            
            int x = Random.Range(1,lengthMapCount);
            int y = Random.Range(0,roadWidthCount);
            // Debug.Log("x:"+x.ToString());
            // Debug.Log("y:"+y.ToString());
            // Debug.Log("lengthMapCount:"+lengthMapCount.ToString());
            // Debug.Log("roadWidthCount:"+roadWidthCount.ToString());
            if(ingredientMap.ContainsKey(x*roadWidthCount+y)){
                //Debug.Log("ingredient not pass "+x.ToString() + " " +y.ToString());
                i--;
                continue;
            }

            ingredientMap.Add(x*roadWidthCount+y, true);
           
            int sourceIndex = ingredientSelector.nextIngredientTypeIndex();
            GameObject obj = (GameObject)Instantiate(ingredientSourceObject[sourceIndex]);
        
            obj.name  = ingredientSourceObject[sourceIndex].name +" "+ Random.Range(0,maxIngredientId).ToString();
            int roadWidthInterval = roadWidth/roadWidthCount;
            obj.transform.position = road.transform.position + new Vector3(-roadLength/2+x*ingredientInterval, ingredientSourceObject[sourceIndex].transform.position.y , staticWidth[y]);
            obj.SetActive(true);
            ingredientDynamicObjectYoung.Add(obj);
            
        }
    }
    // road extend trigger
    void OnTriggerEnter(Collider co) {
        Debug.Log("OnTriggerEnter");
        if(roadObject[0].transform.position.x<roadObject[1].transform.position.x){
           curRoadObject = roadObject[1];
           nextRoadObject = roadObject[0];
        }else{
           curRoadObject = roadObject[0];
           nextRoadObject = roadObject[1];
        }
        if (co.CompareTag("Player") && CheckMoveRoad()){

            Debug.Log("OnTriggerValid");
            MoveRoad();
            ClearIngredients();
            CreateIngredients(nextRoadObject);
        }

    }
}

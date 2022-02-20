using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class IngridientSelector
{
    LevelConfigObj m_configObj;
    Dictionary<string, int> indexMap;
    List<int> randomMap;
    public void init(int level,GameObject[] ingredientSourceObject){
      
          GameObject  configReader = GameObject.FindGameObjectsWithTag("ConfigReader")[0];
          ConfigReader cr = configReader.GetComponent<ConfigReader>();
       
           LevelConfigObj configObj = cr.configResult.LevelConfigObj[level-1];
          indexMap = new Dictionary<string, int>();
          randomMap = new List<int>();
          for(int i=0;i<ingredientSourceObject.Length;i++){
              indexMap[ingredientSourceObject[i].name] = i;
          }
          for(int i=0;i<configObj.IngridientsWeights.Length;i++){
              for(int j=0;j<configObj.IngridientsWeights[i].Weight;j++){
                    randomMap.Add(indexMap[configObj.IngridientsWeights[i].Name]);
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
    // ingridientCloseFactor is small means ingridient can be closer
    public int ingridientCloseFactor = 2;
    // for the firstRoad
    public bool  startToCreate = false;

    // ingridient type guarantee
    bool  gachaGuarantee = true;

    GameObject[] roadObject;  
    GameObject triggerObject;
    GameObject curRoadObject;  
    GameObject nextRoadObject;
    List<GameObject> ingridientDynamicObject;
    List<GameObject> ingridientDynamicObjectYoung;
    List<bool>  ingredientMap;
    public double densityRatio = 0.01;
    int maxIngridientId = 2147483647;
    void Start()
    {

        Debug.Log("OnTriggerStart");
        ingridientDynamicObject =new List<GameObject> ();
        ingridientDynamicObjectYoung = new List<GameObject> ();
        roadObject =  GameObject.FindGameObjectsWithTag("OriginRoad");
        
        triggerObject =  GameObject.FindGameObjectsWithTag("RoadTrigger")[0];
        if(roadObject[0].transform.position.x<roadObject[1].transform.position.x){
            CreateIngridients(roadObject[1]);
        }else{
            CreateIngridients(roadObject[0]);
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
    // clear old road ingridient
     void ClearIngridients(){
        foreach (GameObject obj in ingridientDynamicObject)
        {
            Destroy(obj);
        }
        ingridientDynamicObject.Clear();
         foreach (GameObject obj in ingridientDynamicObjectYoung)
        {
            ingridientDynamicObject.Add(obj);
        }
        ingridientDynamicObjectYoung.Clear();
    }
    void GetIngridientsConfig(int level){
        // LevelConfigObj configObj = ConfigReader.LoadJsonFromFile<LevelConfigObj>(level);
     
        
    }
    // create ingridients randomly
    void CreateIngridients(GameObject road){
        IngridientSelector ingridientSelector = new IngridientSelector();
        ingridientSelector.init(1,ingredientSourceObject);
        Debug.Log("create ingridient for "+road.name);
        int ingridientCounts = (int)( densityRatio * (roadLength/ingridientCloseFactor) * (roadWidth/ingridientCloseFactor));
        int widthMap = (roadWidth/ingridientCloseFactor)+1;
        int lengthMap = (roadLength/ingridientCloseFactor)+1 ;
        ingredientMap = new List<bool>(new bool[(widthMap)*(lengthMap)]);
        int sourceTypeCount = ingredientSourceObject.Length;
        int sourceIndex = 0;
        for(int i=0;i<ingridientCounts;i++){
            if(sourceTypeCount == 0){
                break;
            }
          
            int x = Random.Range(0,(int)(roadLength/ingridientCloseFactor));
            int y = Random.Range(0,(int)(roadWidth/ingridientCloseFactor));

            if(ingredientMap[x*widthMap+y]){
                //Debug.Log("ingridient not pass "+x.ToString() + " " +y.ToString());
                i--;
                continue;
            }

            ingredientMap[x*widthMap+y] = true;
            x = x* ingridientCloseFactor;
            y = y* ingridientCloseFactor;
            sourceIndex = ingridientSelector.nextIngredientTypeIndex();
            GameObject obj = (GameObject)Instantiate(ingredientSourceObject[sourceIndex]);
        
            obj.name  = ingredientSourceObject[sourceIndex].name +" "+ Random.Range(0,maxIngridientId).ToString();
            obj.transform.position = road.transform.position + new Vector3(x-roadLength/2+1, 0 , y-roadWidth/2+1);
            obj.SetActive(true);
            ingridientDynamicObjectYoung.Add(obj);
            
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
            ClearIngridients();
            CreateIngridients(nextRoadObject);
        }

    }
}

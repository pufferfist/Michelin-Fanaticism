using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoadTrigger : MonoBehaviour
{
    public GameObject nextRoadObject;
    public int roadLength = 300;
    public bool skipFirstTrigger = true;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void OnTriggerEnter(Collider co) {

        if (co.CompareTag("Player")){
            if(skipFirstTrigger){
                skipFirstTrigger = false;
                return;
            }
            nextRoadObject.transform.position = new Vector3(nextRoadObject.transform.position.x+roadLength, nextRoadObject.transform.position.y, nextRoadObject.transform.position.z);

     }

    }
}

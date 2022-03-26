using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace DefaultNamespace
{
    public class MouseHandler : MonoBehaviour, IPointerDownHandler
    {
        public CollectedHandler collectedHandler;
        public int index, k;

        void Start()
        {
        }

        // Update is called once per frame
        void Update()
        {
        }
        
        public void OnPointerDown(PointerEventData eventData)
        {
            Debug.Log("mouse down" + index.ToString() + k.ToString());
            collectedHandler.drop(index, k);
        }
    }
}

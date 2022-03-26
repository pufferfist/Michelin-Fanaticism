using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace DefaultNamespace
{
    public class MouseHandler : MonoBehaviour
    {
        public CollectedHandler collectedHandler;
        public int index, k;

        void Start()
        {
        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetKeyUp(KeyCode.T))
            {
                Debug.Log(Input.mousePosition);
            }
        }

        void OnMouseDown(MouseDownEvent evt)
        {   
            bool leftMouseButtonPressed = 0 != (evt.pressedButtons & (1 << (int)MouseButton.LeftMouse));
            bool rightMouseButtonPressed = 0 != (evt.pressedButtons & (1 << (int)MouseButton.RightMouse));
            bool middleMouseButtonPressed = 0 != (evt.pressedButtons & (1 << (int)MouseButton.MiddleMouse));
            Debug.Log($"Mouse Down event. Triggered by {(MouseButton)evt.button}.");
            Debug.Log($"Pressed buttons: Left button: {leftMouseButtonPressed} Right button: {rightMouseButtonPressed} Middle button: {middleMouseButtonPressed}");
            Debug.Log("mouse down" + index.ToString() + k.ToString());
            collectedHandler.drop(index, k);
        }
    }
}

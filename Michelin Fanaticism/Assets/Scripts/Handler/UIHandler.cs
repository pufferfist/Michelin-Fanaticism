using System;
using System.Collections.Generic;
using MenuNameSpace;
using UnityEngine;
using UnityEngine.UI;

namespace DefaultNamespace
{
    public class UIHandler
    {
        private GameObject[] collectedPanel;
        private GameObject[] menuPanel;

        //init: find panel's reference
        public UIHandler(GameObject ui)
        {
            collectedPanel = new GameObject[2];
            menuPanel = new GameObject[3];
            for (int i = 0; i < collectedPanel.Length; i++)
            {
                collectedPanel[i] = ui.transform.Find("Hud").Find("CollectedPanel" + (i + 1)).gameObject;
                Debug.Log(collectedPanel[i].name);
            }

            for (int i = 0; i < menuPanel.Length; i++)
            {
                menuPanel[i] = ui.transform.Find("Hud").Find("MenuPanel").Find("Recipe" + (i + 1)).gameObject;
                Debug.Log(menuPanel[i].name);
            }
        }

        public void updateCollectedPanel(int id, Stack<String> ingres)
        {
            int i = 0;
            foreach (String ingre in ingres)
            {
                collectedPanel[id].transform.GetChild(i).GetComponent<Text>().text = ingre;
                i++;
            }
            
        }

        public void updateMenuPanel(Recipe[] recipeList)
        {
            
        }

        private void resetCollectedPanel(int index)
        {
            foreach (Transform ingre in collectedPanel[index].transform)
            {
                ingre.GetComponent<Text>().text="";
            }
        }
    }
}
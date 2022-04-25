using System;
using System.Collections.Generic;
using UnityEngine;
namespace DefaultNamespace
{
    public class CollectedHandler
    {
        private List<String>[] collected;
        private UIHandler uiHandler;

        public CollectedHandler(UIHandler uiHandler,LevelConfig levelConfig)
        {
            collected = new List<string>[levelConfig.BagSlot];
            for (int i = 0; i < collected.Length; i++)
            {
                collected[i] = new List<string>();
            }
            this.uiHandler = uiHandler;
        }
        
        //return null if stack is full
        public List<String> pickUp(int index, String ingre)
        {
            if (collected[index].Count >= 3)
            {
                return null;
            }
            else
            {
                collected[index].Add(ingre);
                collected[index].Sort();
                uiHandler.updateCollectedPanel(index,collected[index]);
                return collected[index];
            }
        }

        public List<String> drop(int index, int k = 0)
        {
            if (collected[index].Count>=1 && k < collected[index].Count)
            {
                MainCharacter mainCharacter = GameManager.gm.character;
                AudioSource[] audioSources = mainCharacter.GetComponents<AudioSource>();
                AudioSource audioSource = audioSources[4];
                audioSource.Play();
                collected[index].RemoveAt(k);
                uiHandler.updateCollectedPanel(index,collected[index]);
            }
            GameManager.gm.finish(collected[index]);
            
            return collected[index];
        }

        public void finish(int index)
        {
            collected[index].Clear();
            uiHandler.updateCollectedPanel(index, new List<string>());
        }
    }
}
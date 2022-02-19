using System;
using System.Collections.Generic;

namespace DefaultNamespace
{
    public class CollectedHandler
    {
        private Stack<String>[] collected;
        private UIHandler uiHandler;

        public CollectedHandler(UIHandler uiHandler)
        {
            collected = new Stack<string>[2];
            for (int i = 0; i < collected.Length; i++)
            {
                collected[i] = new Stack<string>();
            }
            this.uiHandler = uiHandler;
        }
        
        //return null if stack is full
        public Stack<String> pickUp(int index, String ingre)
        {
            if (collected[index].Count >= 3)
            {
                return null;
            }
            else
            {
                collected[index].Push(ingre);
                uiHandler.updateCollectedPanel(index,collected[index]);
                return collected[index];
            }
        }

        public Stack<String> drop(int index, String ingre)
        {
            if (collected[index].Count>=1)
            {
                collected[index].Pop();
            }

            return collected[index];
        }

        public void finish(int index)
        {
            collected[index].Clear();
            uiHandler.updateCollectedPanel(index,new Stack<string>());
        }
    }
}
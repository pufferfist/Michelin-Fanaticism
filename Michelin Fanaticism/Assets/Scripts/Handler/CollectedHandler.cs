using System;
using System.Collections.Generic;

namespace DefaultNamespace
{
    public class CollectedHandler
    {
        private Stack<String>[] collected;

        public CollectedHandler()
        {
            collected = new Stack<string>[2];
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
    }
}
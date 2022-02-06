using UnityEngine;
using UnityEngine.TestTools;
using System;
using System.Collections;
using System.Collections.Generic;

namespace MenuNameSpace
{
    public struct Menu
    {
        public int menuId;
        public string name;
        public float startTime;
        public float endTime;
        public List<string> ingredients;
        public Menu(int menuId, string name, List<string> ingredients)
        {
            this.menuId = menuId;
            this.name = name;
            this.ingredients = ingredients;
            this.startTime = 0;
            this.endTime = 0;
        }
        
    }
    
}
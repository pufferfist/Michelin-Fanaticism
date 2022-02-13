using System;
using System.Collections.Generic;
using MenuNameSpace;
using UnityEngine;
using Random = UnityEngine.Random;
namespace MenuToolsSpace
{
    public class MenuTools
    {
        public static Menu getAMenu(List<Menu> Menus)
        {
            Menu menu = Menus[Random.Range(0, Menus.Count)];
            menu.startTime = Time.time;
            menu.endTime = menu.startTime + 30;
            return menu;
        }
        
        public static int getIngreNeedNum(Menu menu, String ingredientName)
        {
            int needNum = 0;
            if (menu != null)
            {
                for (var i = 0; i < menu.ingredients.Count; i++)
                {
                    if (ingredientName == menu.ingredients[i])
                    {
                        needNum++;
                    }
                }
            }

            return needNum;
        }
        

    }
}
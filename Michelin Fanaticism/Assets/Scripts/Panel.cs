using System;
using System.Collections.Generic;

namespace PanelSpace
{
    public class Panel
    {
        private int forwhichmenu = -1;
        private List<string> pickedIngredients;
        private Boolean isBusy;
        public Panel()
        {
            isBusy = false;
            pickedIngredients = new List<string>();
        }

        public void setForWhichMenu(int whichMenu)
        {
            forwhichmenu = whichMenu;
        }
        public int getForWhichMenu()
        {
            return forwhichmenu;
        }

        public Boolean getIsBusy()
        {
            return isBusy;
        }

        public void reset()
        {
            forwhichmenu = -1;
            pickedIngredients.Clear();
            isBusy = false;
        }

        public void addIngre(String Ingre)
        {
            pickedIngredients.Add(Ingre);
            isBusy = true;
        }

        public List<String> getPickedIngre()
        {
            return pickedIngredients;
        }
    }
}
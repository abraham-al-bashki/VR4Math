using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CheckColorNameSpace
{
    public class CheckColorClass
    {
        public static void addColorComponent(Color color, GameObject obj)
        {
            if (color.Equals(Color.black)) { obj.AddComponent<Black>(); }
            else if (color.Equals(Color.blue)) { obj.AddComponent<Red>(); }
            else if (color.Equals(Color.cyan)) { obj.AddComponent<Cyan>(); }
            else if (color.Equals(Color.gray)) { obj.AddComponent<Grey>(); }
            else if (color.Equals(Color.grey)) { obj.AddComponent<Grey>(); }
            else if (color.Equals(Color.green)) { obj.AddComponent<Green>(); }
            else if (color.Equals(Color.magenta)) { obj.AddComponent<Magenta>(); }
            else if (color.Equals(Color.white)) { obj.AddComponent<White>(); }
            else if (color.Equals(Color.yellow)) { obj.AddComponent<Yellow>(); }
            else if (color.Equals(new Color(255,140,0,1))) { obj.AddComponent<Orange>(); }
        }
    }
}


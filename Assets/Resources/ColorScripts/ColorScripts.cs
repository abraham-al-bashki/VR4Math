using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorScripts
{
    public static bool getRightColorScript(Color color, GameObject gameObj)
    {
        if (Black.color == color) { gameObj.AddComponent<Black>(); return true; }
        else if (Blue.color == color) { gameObj.AddComponent<Blue>(); return true; }
        else if (Cyan.color == color) { gameObj.AddComponent<Cyan>(); return true; }
        else if (Green.color == color) { gameObj.AddComponent<Green>(); return true; }
        else if (Grey.color == color) { gameObj.AddComponent<Grey>(); return true; }
        else if (Magenta.color == color) { gameObj.AddComponent<Magenta>(); return true; }
        else if (Orange.color == color) { gameObj.AddComponent<Orange>(); return true; }
        else if (Red.color == color) { gameObj.AddComponent<Red>(); return true; }
        else if (White.color == color) { gameObj.AddComponent<White>(); return true; }
        else if (Yellow.color == color) { gameObj.AddComponent<Yellow>(); return true; }
        return false;
    }
}

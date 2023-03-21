using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LA_Object_Translate : ICommand
{
    LA_Object _instance;
    Vector3 _translation;
    Vector3 _prevTranslation;
    public LA_Object_Translate(LA_Object instance, Vector3 translation)
    {
        _instance = instance;
        _prevTranslation = instance.getTranslation();
        _translation = translation;
    }
    public void execute()
    {
        _instance.translate(_translation.x, _translation.y, _translation.z);
        _instance.draw();
    }

    public void undo()
    {
        _instance.translate(_prevTranslation.x, _prevTranslation.y, _prevTranslation.z);
        _instance.draw();
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class LA_Object_Scale : ICommand
{
    LA_Object _instance;
    float _scale;
    float _prevScale;
    public LA_Object_Scale(LA_Object instance, float scale)
    {
        _instance = instance;
        Vector3 lossyScale = instance.getScaling().lossyScale;
        float[] lossyscaleArr = new float[3] { lossyScale.x, lossyScale.y, lossyScale.z };
        _prevScale = lossyscaleArr.Min();
        _scale = scale;
    }

    public void execute()
    {
        _instance.scale(_scale);
        _instance.draw();
    }

    public void undo()
    {
        _instance.scale(_prevScale);
        _instance.draw();
    }
}

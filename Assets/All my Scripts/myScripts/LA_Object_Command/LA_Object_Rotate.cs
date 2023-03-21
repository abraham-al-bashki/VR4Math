using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LA_Object_Rotate : ICommand
{
    LA_Object _instance;
    Vector3 _eulerAngles;
    Vector3 _eulerAnglesInverse;
    public LA_Object_Rotate(LA_Object instance, Vector3 eulerAngles)
    {
        _instance = instance;
        _eulerAngles = eulerAngles;
        Quaternion currentRotation = _instance.getRotation().rotation * Quaternion.Euler(eulerAngles);
        _eulerAnglesInverse = Matrix4x4.Rotate(currentRotation).inverse.rotation.eulerAngles;
    }
    public void execute()
    {
        _instance.rotate(_eulerAngles.x, _eulerAngles.y, _eulerAngles.z);
        _instance.draw();
    }

    public void undo()
    {
        _instance.rotate(_eulerAnglesInverse.x, _eulerAnglesInverse.y, _eulerAnglesInverse.z);
        _instance.draw();
    }
}

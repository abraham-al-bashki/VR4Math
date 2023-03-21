using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LA_Object_Transform : ICommand
{
    LA_Object _instance;
    bool _hasExecuted;
    bool _hasUndo;
    float _scale;
    float _prevScale;
    Vector3 _translation;
    Vector3 _prevTranslation;
    Vector3 _eulerAngles;
    Vector3 _eulerAnglesInverse;

    public LA_Object_Transform(LA_Object instance, Vector3? translation, Vector3? eulerAngles, float? scale) //data types are Nullable<Vector3> and Nullable<int>
    {
        _instance = instance;
        _prevScale = getScale();
        _prevTranslation = instance.getTranslation(); //current "translation", actually just the position
        if (!(translation.HasValue)) { _translation = _prevTranslation; }
        else { _translation = translation.Value; }
        if (!(eulerAngles.HasValue)) { _eulerAngles = Vector3.zero; _eulerAnglesInverse = Vector3.zero; }
        else { 
            _eulerAnglesInverse = (Matrix4x4.Rotate(Quaternion.Euler(eulerAngles.Value)).inverse).rotation.eulerAngles;
            _eulerAngles = eulerAngles.Value;
        }
        if (!(scale.HasValue)) { _scale = _prevScale; }
        else { _scale = scale.Value; }
    }
    public void execute()
    {
        if (_hasExecuted) { return; }
        _instance.scale(_scale);
        _instance.translate(_translation.x, _translation.y, _translation.z);
        _instance.rotate(_eulerAngles.x, _eulerAngles.y, _eulerAngles.z);
        _instance.draw();
        _hasExecuted = true;
    }

    public void undo()
    {
        if (_hasUndo) { return; } //if has done undo then return
        if (!_hasExecuted) { return; } //if has not executed then return
        _instance.scale(_prevScale);
        _instance.translate(_prevTranslation.x, _prevTranslation.y, _prevTranslation.z);
        _instance.rotate(_eulerAnglesInverse.x, _eulerAnglesInverse.y, _eulerAnglesInverse.z);
        _instance.draw();
        _hasUndo = true;
    }

    /*internal void scaleValues(float scale)
    {
        _prevScale = getScale();
        _scale = scale;
    }
    internal void translationValues(Vector3 translation)
    {
        _prevTranslation = _instance.getTranslation();
        _translation = translation;
    }
    internal void rotationValues(Vector3 eulerAngles)
    {
        _eulerAngles = eulerAngles;
        Quaternion currentRotation = _instance.getRotation().rotation * Quaternion.Euler(eulerAngles);
        _eulerAnglesInverse = Matrix4x4.Rotate(currentRotation).inverse.rotation.eulerAngles;
    }*/
    internal float getScale()
    {
        Vector3 lossyScale = _instance.getScaling().lossyScale;
        float[] lossyscaleArr = new float[3] { lossyScale.x, lossyScale.y, lossyScale.z };
        return lossyscaleArr.Min();
    }
}

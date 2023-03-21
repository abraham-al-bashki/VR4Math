using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public interface LA_Object
{
    public void scale(float scalar);
    public void translate(float x, float y, float z);
    public void rotate(float eulerX, float eulerY, float eulerZ);

    public Matrix4x4 getRotation();

    public Matrix4x4 getScaling();

    public Vector3 getTranslation();

    public GameObject getGameObject();
    public void draw();
    public string toString();
    public void destroy(Controller controller);
    public string getName();
    public void setName(string name);
}

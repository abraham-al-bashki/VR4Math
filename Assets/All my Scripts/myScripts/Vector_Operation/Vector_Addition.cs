using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vector_Addition : Vector_Operation
{
    LA_Vector vector_1;
    LA_Vector vector_2;
    LA_Vector vector_fixed;
    Controller _controller;
    private bool _isActive;
    public Vector_Addition(Controller controller)
    {
        _controller = controller;
        vector_1 = new LA_Vector(Vector3.zero, Vector3.forward * 3, Color.black);
        vector_2 = new LA_Vector(Vector3.zero, Vector3.one * 3, Color.black);
        vector_1.draw();
        vector_2.draw();

        Matrix4x4 mat_1 = Matrix4x4.Scale(vector_1.getScaling().m00 * Vector3.one);
        Matrix4x4 mat_2 = Matrix4x4.Scale(vector_2.getScaling().m00 * Vector3.one);
        vector_fixed = new LA_Vector(Vector3.zero, (mat_1 * vector_1.getDirection()) + (mat_2 * vector_2.getDirection()), Color.red);
        vector_fixed.draw();

        setActive(false);
    }
    public void myUpdate()
    {
        if (!_isActive) { return; }
        if ((vector_fixed != null) && (vector_1.getGameObject().transform.hasChanged || vector_2.getGameObject().transform.hasChanged))
        {
            vector_fixed.destroy();
            Matrix4x4 mat_1 = Matrix4x4.Scale(vector_1.getScaling().m00 * Vector3.one);
            Matrix4x4 mat_2 = Matrix4x4.Scale(vector_2.getScaling().m00 * Vector3.one);

            vector_fixed = new LA_Vector(Vector3.zero, (mat_1 * vector_1.getDirection()) +  (mat_2 * vector_2.getDirection()), Color.red);
            vector_fixed.draw();
            vector_1.getGameObject().transform.hasChanged = false;
            vector_2.getGameObject().transform.hasChanged = false;
        }
        if ((vector_fixed == null) && (vector_1.getGameObject().transform.hasChanged || vector_2.getGameObject().transform.hasChanged))
        {
            Matrix4x4 mat_1 = Matrix4x4.Scale(vector_1.getScaling().m00 * Vector3.one);
            Matrix4x4 mat_2 = Matrix4x4.Scale(vector_2.getScaling().m00 * Vector3.one);

            vector_fixed = new LA_Vector(Vector3.zero, (mat_1 * vector_1.getDirection()) + (mat_2 * vector_2.getDirection()), Color.red);
            vector_fixed.draw();
            vector_1.getGameObject().transform.hasChanged = false;
            vector_2.getGameObject().transform.hasChanged = false;
        }
    }

    public void setActive(bool setter)
    {
        vector_1.getGameObject().transform.parent.gameObject.SetActive(setter);
        vector_2.getGameObject().transform.parent.gameObject.SetActive(setter);
        vector_fixed.getGameObject().transform.parent.gameObject.SetActive(setter);
        _isActive = setter;
    }
    public void setActiveAtStart(bool setter)
    {
        vector_1.getGameObject().transform.parent.gameObject.SetActive(setter);
        vector_2.getGameObject().transform.parent.gameObject.SetActive(setter);
        _isActive = setter;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;
using System;
using System.Globalization;

public class LA_Tree : LA_Object, SaveAssets
{
    string _name = "";
    Vector3 _centerPoint;
    Vector3 _dimensions;
    float _scale;
    Vector3 _orientation;
    private bool _rotationFlag;
    Transform _parent;
    Transform _tree;
    Controller _controller;
    bool scalingIsZero;
    ToStringStates _toStringState;

    enum ToStringStates
    {
        oneMatrix,
        matriceFocus,
        rotationMatricesExpanded,
        scalingMatrixFocus
    }

    public LA_Tree(Vector3 centerPoint, Vector3 dimensions, Vector3 orientation, Color color)
    {
        _parent = new GameObject().transform;
        _parent.gameObject.AddComponent<LA_Object_Component>().laobject = this;

#if UNITY_EDITOR

        GameObject temp = (GameObject)AssetDatabase.LoadAssetAtPath("Assets/Resources/Geometry/Prefab/Tree.prefab", typeof(GameObject));
        _tree = ((GameObject)PrefabUtility.InstantiatePrefab(temp)).transform;
#else
        _tree = GameObject.Instantiate<GameObject>(Resources.Load<GameObject>("Geometry/Prefab/Tree")).transform;
#endif
        _centerPoint = centerPoint;
        _orientation = orientation;
        _rotationFlag = true;
        _tree.SetParent(_parent);
        ColorScripts.getRightColorScript(color, _tree.gameObject);

        draw();
        float[] dimensionsArr = new float[3] { dimensions.x, dimensions.y, dimensions.z };
        _scale = dimensionsArr.Min();
        _dimensions = dimensions / _scale;
        scalingIsZero = true;
        if (dimensions == Vector3.zero) { scalingIsZero = false; }
        _tree.gameObject.layer = LayerMask.NameToLayer("LA_Object");

        _toStringState = ToStringStates.oneMatrix;
    }

    public LA_Tree(Transform parent, Transform tree, Vector3 orientation, Vector3 centerPoint, float scale, Vector3 dimensions)
    {
        _parent = parent;
        _tree = tree;
        _orientation = orientation;
        _centerPoint = centerPoint;
        _scale = scale;
        _dimensions = dimensions;

        scalingIsZero = true;
        if (dimensions == Vector3.zero) { scalingIsZero = false; }

        _toStringState = ToStringStates.oneMatrix;
        _rotationFlag = false;
    }

    public void rotate(float eulerX, float eulerY, float eulerZ)
    {
        _orientation = new Vector3(eulerX, eulerY, eulerZ);
        _rotationFlag = true;
    }

    public void scale(float scale)
    {
        _scale = scale;
    }

    public void translate(float x, float y, float z)
    {
        _centerPoint = new Vector3(x, y, z);
    }

    public void draw()
    {
        _tree.position = _centerPoint;
        if (_rotationFlag) { _tree.rotation = _tree.rotation * Quaternion.Euler(_orientation); }
        _rotationFlag = false;
        _tree.localScale = _dimensions * _scale;
        if (_tree.lossyScale != Vector3.zero)
        {
            scalingIsZero = false;
        }
        else
        {
            scalingIsZero = true;
        }
    }

    public Matrix4x4 getRotation()
    {
        return Matrix4x4.Rotate(_tree.transform.rotation);
    }

    public Matrix4x4 getScaling()
    {
        return Matrix4x4.Scale(Vector3.one * _scale);
    }

    public Vector3 getTranslation()
    {
        return _tree.transform.position;
    }

    public Matrix4x4 getMatrix()
    {
        return Matrix4x4.TRS(getTranslation(), _tree.transform.rotation, Vector3.one * _scale);
    }
    public string toString()
    {
        string format = "F5";
        IFormatProvider formatProvider = CultureInfo.InvariantCulture.NumberFormat;

        Vector3 t = getTranslation();
        string tRow0 = String.Format("|{0}\t|",
                t[0].ToString(format, formatProvider));
        string tRow1 = String.Format("|{0}\t|",
                t[1].ToString(format, formatProvider));
        string tRow2 = String.Format("|{0}\t|",
                t[2].ToString(format, formatProvider));

        if (_toStringState == ToStringStates.oneMatrix)
        {
            Matrix4x4 m = getMatrix();
            string mRow0 = String.Format("|{0}\t\t{1}\t\t{2}\t\t|",
                m[0, 0].ToString(format, formatProvider), m[1, 0].ToString(format, formatProvider), m[2, 0].ToString(format, formatProvider));
            string mRow1 = String.Format("|{0}\t\t{1}\t\t{2}\t\t|",
                m[0, 1].ToString(format, formatProvider), m[1, 1].ToString(format, formatProvider), m[2, 1].ToString(format, formatProvider));
            string mRow2 = String.Format("|{0}\t\t{1}\t\t{2}\t\t|",
                m[0, 2].ToString(format, formatProvider), m[1, 2].ToString(format, formatProvider), m[2, 2].ToString(format, formatProvider));
            return mRow0 + "|x|\t\t" + tRow0 + "\n" +
                   mRow1 + "|y|\t+\t" + tRow1 + "\n" +
                   mRow2 + "|z|\t\t" + tRow2;
        }
        tRow0 = "|t1|";
        tRow1 = "|t2|";
        tRow2 = "|t3|";
        float t1 = t.x;
        float t2 = t.y;
        float t3 = t.z;
        Vector3 eulerAngles = getRotation().rotation.eulerAngles;
        float α = eulerAngles.x;
        float β = eulerAngles.y;
        float γ = eulerAngles.z;
        Matrix4x4 sm = getScaling();
        string smRow0 = String.Format("|{0}\t{1}\t{2}\t|",
            sm[0, 0].ToString(format, formatProvider), sm[1, 0].ToString(format, formatProvider), sm[2, 0].ToString(format, formatProvider));
        string smRow1 = String.Format("|{0}\t{1}\t{2}\t|",
            sm[0, 1].ToString(format, formatProvider), sm[1, 1].ToString(format, formatProvider), sm[2, 1].ToString(format, formatProvider));
        string smRow2 = String.Format("|{0}\t{1}\t{2}\t|",
            sm[0, 2].ToString(format, formatProvider), sm[1, 2].ToString(format, formatProvider), sm[2, 2].ToString(format, formatProvider));
        float s1 = sm[0, 0];
        float s2 = sm[1, 1];
        float s3 = sm[2, 2];
        string rmRow0;
        string rmRow1;
        string rmRow2;
        if (_toStringState == ToStringStates.scalingMatrixFocus)
        {

            return "\t\t" + smRow0 + "|x| + " + tRow0 + "\n" +
                "R(α,β,γ) +\t" + smRow1 + "|y| + " + tRow1 + "\n" +
                "\t\t" + smRow2 + "|z| + " + tRow2 + "\n" +
                "α = " + α.ToString() + ",\tβ = " + β.ToString() + ",\tγ = " + γ.ToString() + ",\n" +
                "t1 = " + t1.ToString() + ",\tt2 = " + t2.ToString() + ",\tt3 = " + t3.ToString();
        }
        else if (_toStringState == ToStringStates.matriceFocus)
        {
            smRow0 = "|s1 0 0|";
            smRow1 = "|0 s2 0|";
            smRow2 = "|0 0 s3|";
            Matrix4x4 rm = getRotation();
            rmRow0 = String.Format("|{0}\t{1}\t{2}\t|",
            sm[0, 0].ToString(format, formatProvider), rm[1, 0].ToString(format, formatProvider), rm[2, 0].ToString(format, formatProvider));
            rmRow1 = String.Format("|{0}\t{1}\t{2}\t|",
                rm[0, 1].ToString(format, formatProvider), rm[1, 1].ToString(format, formatProvider), rm[2, 1].ToString(format, formatProvider));
            rmRow2 = String.Format("|{0}\t{1}\t{2}\t|",
                rm[0, 2].ToString(format, formatProvider), rm[1, 2].ToString(format, formatProvider), rm[2, 2].ToString(format, formatProvider));

            return rmRow0 + smRow0 + "|x| + " + tRow0 + "\n" +
                rmRow1 + smRow1 + "|y| + " + tRow1 + "\n" +
                rmRow2 + smRow2 + "|z| + " + tRow2 + "\n\n" +
                "s1 = " + s1.ToString() + ",\t\ts2 = " + s2.ToString() + ",\t\ts3 = " + s3.ToString() + ",\n" +
                "t1 = " + t1.ToString() + ",\t\tt2 = " + t2.ToString() + ",\t\tt3 = " + t3.ToString(); ;
        }
        smRow0 = "|s1 0 0|";
        smRow1 = "|0 s2 0|";
        smRow2 = "|0 0 s3|";

        rmRow0 = "|cos(α)\t0\t sin(α)|" + "|1\t0\t\t0\t|" + "|cos(γ)\t-sin(γ)\t0|";
        rmRow1 = "|0\t\t1\t0\t|" + "|0\tcos(β)\t-sin(β)|" + "|sin(γ)\tcos(γ)\t\t0|";
        rmRow2 = "|-sin(α)\t0 \tcos(α)|" + "|0\tsin(β)\t\tcos(β)|" + "|0\t\t0\t\t1|";

        return rmRow0 + smRow0 + "|x| + " + tRow0 + "\n" +
                rmRow1 + smRow1 + "|y| + " + tRow1 + "\n" +
                rmRow2 + smRow2 + "|z| + " + tRow2 + "\n\n" +
                "α = " + α.ToString() + ",\tβ = " + β.ToString() + ",\tγ = " + γ.ToString() + ",\t" +
                "s1 = " + s1.ToString() + ",\t\ts2 = " + s2.ToString() + ",\t\ts3 = " + s3.ToString() + ",\n" +
                "t1 = " + t1.ToString() + ",\t\tt2 = " + t2.ToString() + ",\t\tt3 = " + t3.ToString();
    }
    public void save(string saveFolder)
    {
#if UNITY_EDITOR
        string path = "Assets/Resources/" + saveFolder + "/LA_Tree.prefab";
        path = AssetDatabase.GenerateUniqueAssetPath(path);
        if (PrefabUtility.IsPartOfAnyPrefab(_parent.gameObject))
        {
            PrefabUtility.UnpackPrefabInstance(_parent.gameObject, PrefabUnpackMode.Completely, InteractionMode.AutomatedAction);
        }
        PrefabUtility.SaveAsPrefabAsset(_parent.gameObject, path, out bool prefabSuccess);
#endif
    }
    public GameObject getGameObject() { return _tree.gameObject; }
    public void subscribe(Controller controller) { controller.attach(this); _controller = controller; }
    public void destroy(Controller controller)
    {
        if (_controller != null) { _controller.destroy(this); _controller = null; } //unsubscribe
        GameObject.Destroy(_parent.gameObject);
    }

    public string getName()
    {
        return _name;
    }

    public void setName(string name)
    {
        _name = name;
    }
}

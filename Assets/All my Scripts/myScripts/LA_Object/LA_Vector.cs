using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Globalization;
using System;
using System.Runtime.CompilerServices;
using UnityEditor;
using System.Linq;

public class LA_Vector : LA_Object
{
    private string _name = "";
    private Controller _controller;
    private Vector3 _initialStartPoint;
    private Vector3 _initialEndPoint;
    private Vector3 _endPoint;
    private ToStringStates _toStringState;
    private Vector3 _orientation;
    private Vector3 _origin;
    private bool _rotationFlag;
    private Transform _parent;
    private GameObject _line;
    private LineRenderer _lr;
    private GameObject _vectorhead;
    private LineRenderer _vhr;
    private List<BoxCollider> colliders;

    /*
     * The save/load implementation can only work on assets, which is why there need to be a rework. The control flow will still be
     * somewhere else, but these LA_Object scripts need to be converted to an asset (a MonoBehaviour attached to a Game Object) in 
     * the scene, so it can be saved in a state that the user wants to have in the VR. 
     * Also, the variables will be empty on load which is why a the use of a start method (from the MonoBehaviour), that will extract 
     * information (from the Game Objects and its Components) to be reinserted into the empty variables.
     * The rework on the structure in the Hierachy (from Unity's Editor), will the root as an empty Game Object. The children will be 
     * parentless, meaning all the game objects created is there (including the GUI for VR related to its own LA_Object).
     * GUI Objects made one on one, because a list with variables can not be saved, while GUI Objects in the scene can saved!
     */

    //only do this start code for oculus quest 2
    enum ToStringStates
    {
        oneMatrix,
        matriceFocus,
        rotationMatricesExpanded,
        scalingMatrixFocus
    }
    public LA_Vector(Vector3 startPoint, Vector3 endPoint, Color color)
    {
        _parent = new GameObject().transform;
        _parent.name = "Vector";
        _parent.gameObject.AddComponent<LA_Object_Component>().laobject = this;

        _line = new GameObject();
        _lr = _line.AddComponent<LineRenderer>();
        _lr.useWorldSpace = false; //must be turned to false to use parenet-child between gameobjects
        _line.transform.SetParent(_parent);
        _line.name = "Vector Shaft";
        _line.gameObject.layer = LayerMask.NameToLayer("LA_Object");
        ColorScripts.getRightColorScript(color, _line);
        //BoxCollider col = _line.AddComponent<BoxCollider>();
        //col.center = startPoint;
        //col.size = Vector3.one;
        colliders = new List<BoxCollider>();

        _vectorhead = new GameObject();
        _vhr = _vectorhead.AddComponent<LineRenderer>();
        _vhr.useWorldSpace = false; //must be turned to false to use parenet-child between gameobjects
        _vectorhead.transform.SetParent(_parent);
        _vectorhead.name = "Vector Head";
        _vectorhead.gameObject.layer = LayerMask.NameToLayer("LA_Object");
        ColorScripts.getRightColorScript(color, _vectorhead);


        _origin = startPoint;
        _orientation = Vector3.zero;
        _rotationFlag = true;

        draw();
        _initialStartPoint = Vector3.zero;
        _initialEndPoint = endPoint;
        _endPoint = endPoint;

        _toStringState = ToStringStates.oneMatrix;
    }

    public LA_Vector(Vector3 endPoint, Color color)
    {
        _parent = new GameObject().transform;
        _parent.name = "Vector";
        _parent.gameObject.AddComponent<LA_Object_Component>().laobject = this;

        _line = new GameObject();
        _lr = _line.AddComponent<LineRenderer>();
        _lr.useWorldSpace = false; //must be turned to false to use parenet-child between gameobjects
        _line.transform.SetParent(_parent);
        _line.name = "Vector Shaft";
        _line.gameObject.layer = LayerMask.NameToLayer("LA_Object");
        ColorScripts.getRightColorScript(color, _line);
        colliders = new List<BoxCollider>();

        _vectorhead = new GameObject();
        _vhr = _vectorhead.AddComponent<LineRenderer>();
        _vhr.useWorldSpace = false; //must be turned to false to use parenet-child between gameobjects
        _vectorhead.transform.SetParent(_parent);
        _vectorhead.name = "Vector Head";
        _vectorhead.gameObject.layer = LayerMask.NameToLayer("LA_Object");
        ColorScripts.getRightColorScript(color, _vectorhead);

        //getRightColorScript(color);
        _origin = _initialStartPoint;
        _orientation = Vector3.zero;
        _rotationFlag = true;

        draw();
        _initialStartPoint = Vector3.zero;
        _initialEndPoint = endPoint;
        _endPoint = endPoint;

        _toStringState = ToStringStates.oneMatrix;
    }

    public LA_Vector(Transform parent, GameObject line, GameObject vectorhead, LineRenderer lr, LineRenderer vhr, Vector3 orientation, Vector3 origin, Vector3 initialStartPoint, Vector3 initialEndPoint, Vector3 endPoint)
    {
        _parent = parent;
        _line = line;
        //_line.tag = "tagged";
        _vectorhead = vectorhead;
        _lr = lr;
        _vhr = vhr;
        _orientation = orientation;
        _origin = origin;
        _initialStartPoint = initialStartPoint;
        _initialEndPoint = initialEndPoint;
        _endPoint = endPoint;

        _toStringState = ToStringStates.oneMatrix;
        _rotationFlag = false;
    }

    public void draw()
    {
        _lr.positionCount = 3;
        _lr.SetPosition(0, _initialStartPoint);
        _lr.SetPosition(1, _initialEndPoint);
        _lr.SetPosition(2, _endPoint);
        _lr.endWidth = 0.05f;
        _lr.startWidth = 0.1f;
        _line.transform.position = _origin;

        _vhr.positionCount = 2;
        Vector3 startpointForVectorhead = _endPoint - (_initialEndPoint - _initialStartPoint).normalized;
        _vhr.SetPosition(0, startpointForVectorhead);
        _vhr.SetPosition(1, _endPoint);
        _vhr.endWidth = 0.005f;
        _vhr.startWidth = 1;
        _vectorhead.transform.position = _origin;
        if (_rotationFlag)
        {
            _line.transform.rotation = _line.transform.rotation * Quaternion.Euler(_orientation.x, _orientation.y, _orientation.z);
            _vectorhead.transform.rotation = _vectorhead.transform.rotation * Quaternion.Euler(_orientation.x, _orientation.y, _orientation.z);
            _rotationFlag = false;
        }
        //if(_initialStartPoint != _initialEndPoint) { createMeshCollider(); }
        if (_initialStartPoint != _initialEndPoint) { createBoxColliders(); }
    }

    public void scale(float scale)
    {
        Vector3 vectorToScale = _initialEndPoint - _initialStartPoint;
        Quaternion noRotation = Quaternion.identity;
        Vector3 noTranslation = Vector3.zero;
        Matrix4x4 scalingMatrix = Matrix4x4.TRS(noTranslation, noRotation, Vector3.one * scale);
        Vector3 vectorScaled = scalingMatrix * vectorToScale;
        _endPoint = _initialStartPoint + vectorScaled;
    }
    public void translate(float x, float y, float z)
    {
        _origin = new Vector3(x,y,z);
    }

    public void rotate(float eulerX, float eulerY, float eulerZ)
    {
        _orientation = new Vector3(eulerX, eulerY, eulerZ);
        _rotationFlag = true;
    }
    public Vector3 getDirection() { return (_lr.GetPosition(1) - _lr.GetPosition(0)).normalized; }
    public Matrix4x4 getRotation()
    {
        return Matrix4x4.Rotate(_line.transform.rotation);
    }

    public Matrix4x4 getScaling()
    {
        float scale = Mathf.Abs(Vector3.Magnitude(_endPoint - _initialStartPoint)) / Mathf.Abs(Vector3.Magnitude(_initialEndPoint - _initialStartPoint));
        return Matrix4x4.Scale(new Vector3(scale, scale, scale));
    }

    public Vector3 getTranslation()
    {
        return _lr.transform.position;
    }

    public Matrix4x4 getMatrix()
    {
        float scale = Mathf.Abs(Vector3.Magnitude(_endPoint - _initialStartPoint)) / Mathf.Abs(Vector3.Magnitude(_initialEndPoint - _initialStartPoint));
        return Matrix4x4.TRS(getTranslation(), _line.transform.rotation ,Vector3.one * scale);
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
            
            return "\t\t" + smRow0 +  "|x| + " + tRow0 + "\n" +
                "R(α,β,γ) +\t" + smRow1 + "|y| + " + tRow1 + "\n" +
                "\t\t" + smRow2 + "|z| + " + tRow2 + "\n" +
                "α = " + α.ToString() + ",\tβ = " + β.ToString() + ",\tγ = " + γ.ToString() + ",\n" +
                "t1 = " + t1.ToString() + ",\tt2 = " + t2.ToString() + ",\tt3 = " + t3.ToString();
        }
        else if(_toStringState == ToStringStates.matriceFocus)
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
        for(int i = 0; i < colliders.Count; i++){ GameObject.Destroy(colliders[i]); }
#if UNITY_EDITOR
        string path = "Assets/Resources/" + saveFolder + "/LA_Vector.prefab";
        path = AssetDatabase.GenerateUniqueAssetPath(path);
        PrefabUtility.SaveAsPrefabAsset(_parent.gameObject, path, out bool prefabSuccess);
#endif
        createBoxColliders();
    }
    public GameObject getGameObject(){ return _line; }
    //public void subscribe(Controller controller) { controller.attach(this); }
    public void destroy()
    {
        //if(_controller != null) { _controller.destroy(this); _controller = null; } //unsubscribe
        GameObject.Destroy(_parent.gameObject);
    }
    public void createBoxColliders()
    {
        //1x1x1 (of magnitude √3) square collider each along the vector line from the startpoint
        //(1/√3)x(1/√3)x(1/√3) (of magnitude 1 AKA normalized)
        int nColliders = colliders.Count;
        int roundedMagnitude = Mathf.RoundToInt(Vector3.Magnitude(_endPoint - _initialStartPoint));
        if(nColliders == roundedMagnitude) { return; }
        Matrix4x4 mat = Matrix4x4.TRS(getTranslation(), getRotation().rotation, Vector3.one * getScaling()[0]);
        Vector3 direction = (mat * getDirection()).normalized;
        if (nColliders == 0)
        {
            for(int i = 0; i < roundedMagnitude; i++)
            {
                colliders.Add(_line.AddComponent<BoxCollider>());
                colliders[i].center = _initialStartPoint + direction * (i + 0.5f);
                colliders[i].size = Vector3.one.normalized;
            }
        }
        else if(nColliders < roundedMagnitude)
        {
            for (int i = nColliders; i < roundedMagnitude; i++)
            {
                colliders.Add(_line.AddComponent<BoxCollider>());
                colliders[i].center = _initialStartPoint + direction * (i + 0.5f);
                colliders[i].size = Vector3.one.normalized;
            }
        }
        else
        {
            for(int i = roundedMagnitude; i < nColliders; i++)
            {
                GameObject.Destroy(colliders[i]);
            }
        }
    }
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

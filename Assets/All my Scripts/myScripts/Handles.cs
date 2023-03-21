using UnityEngine;
using System.Linq;
using UnityEngine.Animations;
using System;

public class Handles
{
    private Transform _target;
    private Transform _constraint;
    private UnityEngine.Animations.ParentConstraint _pc;
    private GameObject _zRotationAxis;
    private GameObject _xRotationAxis;
    private GameObject _yRotationAxis;
    private Vector _zTranslationAxis;
    private Vector _xTranslationAxis;
    private Vector _yTranslationAxis;
    private Vector _scaleAxis;
    public GameObject _hitBox;
    private HandleType _selectedHandle;
    public HandleType selectedHandle { get => _selectedHandle; }


    public enum HandleType
    {
        TranslationTool,
        RotationTool,
        ScaleTool
    }

    public Handles(Transform target) 
    {
        createConstraintManager();
        initMyFakeHandles();
        selectTarget(target);
    }
    public Handles()
    {
        createConstraintManager();
        initMyFakeHandles();
    }
    public void myLateUpdate()
    {
        if (_target == null) { return; }
        if (_target.hasChanged)
        {
            adjustScaleOnScaleHandle();
            adjustScaleOnTranslationHandles();
            adjustScaleOnRotationHandles();
        }
    }
    public void initMyFakeHandles()
    {
        createRotationHandles();
        createTranslationHandles();
        createScaleHandle();
    }

    public void selectTarget(Transform target)
    {
        _target = target;
        selectObjectForConstraintManager(target);
    }
    public void selectHandle(HandleType handleType)
    {
        int len = _constraint.childCount;
        for(int i = 0; i < len; i++)
        {
            _constraint.GetChild(i).gameObject.SetActive(false);
        }
        if (_target == null) { return; }
        switch (handleType) 
        {
            case HandleType.TranslationTool:
                break;
            case HandleType.RotationTool:
                _selectedHandle = HandleType.RotationTool;
                adjustScaleOnRotationHandles();
                _zRotationAxis.SetActive(true);
                _yRotationAxis.SetActive(true);
                _xRotationAxis.SetActive(true);
                return;
            case HandleType.ScaleTool:
                _selectedHandle = HandleType.ScaleTool;
                adjustScaleOnScaleHandle();
                _scaleAxis.setActive(true);
                return;
            default: break;
        }
        _selectedHandle = HandleType.TranslationTool;
        adjustScaleOnTranslationHandles();
        _zTranslationAxis.setActive(true);
        _yTranslationAxis.setActive(true);
        _xTranslationAxis.setActive(true);
    }

    public bool checkHandleIsActive()
    {
        int len = _constraint.childCount;
        for(int i = 0; i < len; i++)
        {
            return _constraint.GetChild(i).gameObject.activeInHierarchy;
        }
        return false;
    }
    public Vector3 chooseAxisBasedOnCamera(GameObject handle)
    {
        Vector3 axis;
        if (handle.gameObject == _zRotationAxis) { axis = Vector3.forward; }
        else if (handle.gameObject == _yRotationAxis) { axis = Vector3.up; }
        else { axis = Vector3.right; }

        int index = 0;
        Vector3[] axisArr = new Vector3[2] { axis, -axis };
        Vector3 directionTowardsCamera = _constraint.InverseTransformPoint((Camera.main.transform.position - _constraint.lossyScale)).normalized;
        if (Vector3.Angle(directionTowardsCamera, axisArr[0]) <= Vector3.Angle(directionTowardsCamera, axisArr[1])) { index = 0; }
        else { index = 1; }//original
        return axisArr[index];
    }

    public void setParentHitBox(bool b) { if (!b) { _hitBox.transform.SetParent(null); return; } _hitBox.transform.SetParent(_constraint); _hitBox.transform.rotation = _constraint.rotation; }

    public void setLayerMask(GameObject o, int layer)
    {
        o.layer = layer; //this does not compile error even if the string is not in the LayerMask list. The list has to also be inputted from the editor.
    }

    public void selectObjectForConstraintManager(Transform target)
    {
        if (_pc.sourceCount > 0)
        {
            int len = _pc.sourceCount;
            for (int i = 0; i < len; i++) { _pc.RemoveSource(i); }
        }
        UnityEngine.Animations.ConstraintSource source = new UnityEngine.Animations.ConstraintSource();
        source.sourceTransform = target;
        source.weight = 1; //https://docs.unity3d.com/Manual/Constraints.html
        _pc.AddSource(source);
        _pc.constraintActive = true;
    }

    public void createConstraintManager()
    {
        if(_constraint != null) { return; }
        _constraint = new GameObject().transform;
        _constraint.name = "ContraintManager_Handles";
        _pc = _constraint.gameObject.AddComponent<UnityEngine.Animations.ParentConstraint>();
        UnityEngine.Animations.ConstraintSource source = new UnityEngine.Animations.ConstraintSource();
        source.weight = 1; //https://docs.unity3d.com/Manual/Constraints.html
        _pc.AddSource(source);
        _pc.constraintActive = true;
    }
    public void createRotationHandles()
    {
        if (_zRotationAxis != null) { GameObject.Destroy(_zRotationAxis); }
        if (_xRotationAxis != null) { GameObject.Destroy(_xRotationAxis); }
        if (_yRotationAxis != null) { GameObject.Destroy(_yRotationAxis); }
        if (_constraint.localScale == Vector3.zero) { return; }

        //Circles made with one line renderer respectively
        GameObject[] rotationAxis = new GameObject[3];
        LineRenderer[] lr = new LineRenderer[3];
        int len = 10;
        for (int i = 0; i < 3; i++)
        {
            rotationAxis[i] = new GameObject(); //circle around axis
            setLayerMask(rotationAxis[i], LayerMask.NameToLayer("Handle"));
            rotationAxis[i].transform.position = _constraint.position;
            rotationAxis[i].transform.SetParent(_constraint);
            rotationAxis[i].transform.localScale = Vector3.one; //Extracted the use of _target.
            lr[i] = rotationAxis[i].AddComponent<LineRenderer>();
            lr[i].useWorldSpace = false; //must be turned to false to use parenet-child between gameobjects
            lr[i].sharedMaterial = new Material(Shader.Find("Unlit/Color"));
            lr[i].sharedMaterial.doubleSidedGI = true; //both sides are non-transparent
            lr[i].positionCount = len * 4;
            lr[i].endWidth = 0.05f;
            lr[i].startWidth = 0.05f;
        }
        float inc = 1 / ((float)len - 1);
        for (int i = 0; i < len; i++)
        {
            lr[0].SetPosition(i, Vector3.Slerp(Vector3.up, Vector3.right, ((float)i) * inc));
            lr[1].SetPosition(i, Vector3.Slerp(Vector3.up, Vector3.forward, ((float)i) * inc));
            lr[2].SetPosition(i, Vector3.Slerp(Vector3.forward, Vector3.right, ((float)i) * inc));
        }
        for (int i = 0; i < len; i++)
        {
            lr[0].SetPosition(i + len, Vector3.Slerp(Vector3.right, Vector3.down, ((float)i) * inc));
            lr[1].SetPosition(i + len, Vector3.Slerp(Vector3.forward, Vector3.down, ((float)i) * inc));
            lr[2].SetPosition(i + len, Vector3.Slerp(Vector3.right, Vector3.back, ((float)i) * inc));
        }
        for (int i = 0; i < len; i++)
        {
            lr[0].SetPosition(i + len * 2, Vector3.Slerp(Vector3.down, Vector3.left, ((float)i) * inc));
            lr[1].SetPosition(i + len * 2, Vector3.Slerp(Vector3.down, Vector3.back, ((float)i) * inc));
            lr[2].SetPosition(i + len * 2, Vector3.Slerp(Vector3.back, Vector3.left, ((float)i) * inc));
        }
        for (int i = 0; i < len; i++)
        {
            lr[0].SetPosition(i + len * 3, Vector3.Slerp(Vector3.left, Vector3.up, ((float)i) * inc));
            lr[1].SetPosition(i + len * 3, Vector3.Slerp(Vector3.back, Vector3.up, ((float)i) * inc));
            lr[2].SetPosition(i + len * 3, Vector3.Slerp(Vector3.left, Vector3.forward, ((float)i) * inc));
        }
        lr[0].sharedMaterial.color = Color.blue;
        lr[1].sharedMaterial.color = Color.red;
        lr[2].sharedMaterial.color = Color.green;
        _zRotationAxis = rotationAxis[0];
        _xRotationAxis = rotationAxis[1];
        _yRotationAxis = rotationAxis[2];

        _zRotationAxis.gameObject.name = "z";//testing
        _yRotationAxis.gameObject.name = "y";//testing
        _xRotationAxis.gameObject.name = "x";//testing
        _zRotationAxis.gameObject.SetActive(false);
        _yRotationAxis.gameObject.SetActive(false);
        _xRotationAxis.gameObject.SetActive(false);
    }

    public void createTranslationHandles()
    {
        if (_zTranslationAxis != null) { _zTranslationAxis.destroy(); _zTranslationAxis = null; }
        if (_yTranslationAxis != null) { _yTranslationAxis.destroy(); _yTranslationAxis = null; }
        if (_xTranslationAxis != null) { _xTranslationAxis.destroy(); _xTranslationAxis = null; }
        if (_constraint.localScale == Vector3.zero) { return; }
        if (_hitBox == null) { createHitBoxForOutside(); }//testing

        GameObject[] vectorHead = new GameObject[3];
        GameObject[] vectorShaft = new GameObject[3];
        Vector[] vector = new Vector[3];
        for(int i = 0; i < 3; i++)
        {
            vectorHead[i] = new GameObject();
            setLayerMask(vectorHead[i], LayerMask.NameToLayer("Handle"));
            vectorHead[i].transform.position = _constraint.position;
            vectorHead[i].transform.SetParent(_constraint);
            vectorHead[i].transform.localScale = Vector3.one;

            vectorShaft[i] = new GameObject();
            setLayerMask(vectorShaft[i], LayerMask.NameToLayer("Handle"));
            vectorShaft[i].transform.position = _constraint.position;
            vectorShaft[i].transform.SetParent(_constraint);
            vectorShaft[i].transform.localScale = Vector3.one;
        }

        LineRenderer[] lr = new LineRenderer[6];
        int len = lr.Length;
        for (int i = 0; i < len; i = i + 2)
        {
            lr[i] = vectorShaft[i / 2].AddComponent<LineRenderer>();
            lr[i].useWorldSpace = false; //must be turned to false to use parenet-child between gameobjects
            lr[i].sharedMaterial = new Material(Shader.Find("Unlit/Color"));
            lr[i].sharedMaterial.doubleSidedGI = true; //both sides are non-transparent
            lr[i].positionCount = 2;
            lr[i].startWidth = 0.05f;
            lr[i].endWidth = 0.05f;
            lr[i + 1] = vectorHead[i / 2].AddComponent<LineRenderer>();
            lr[i + 1].useWorldSpace = false; //must be turned to false to use parenet-child between gameobjects
            lr[i + 1].sharedMaterial = new Material(Shader.Find("Unlit/Color"));
            lr[i + 1].sharedMaterial.doubleSidedGI = true; //both sides are non-transparent
            lr[i + 1].positionCount = 2;
            lr[i + 1].startWidth = 1;
            lr[i + 1].endWidth = 0.005f;
        }
        for(int i = 0; i < 2; i++)
        {
            lr[i].sharedMaterial.color = Color.blue;
            lr[i + 2].sharedMaterial.color = Color.green;
            lr[i + 4].sharedMaterial.color = Color.red;
        }
        for (int i = 0; i < len; i = i + 2)
        {
            lr[i].SetPosition(0, Vector3.zero);
        }
        lr[1].SetPosition(0, (Vector3.forward - Vector3.zero) - (Vector3.forward - Vector3.zero) * (1f / 3f));
        lr[3].SetPosition(0, (Vector3.up - Vector3.zero) - (Vector3.up - Vector3.zero) * (1f / 3f));
        lr[5].SetPosition(0, (Vector3.right - Vector3.zero) - (Vector3.right - Vector3.zero) * (1f / 3f));
        for (int i = 0; i < 2; i++)
        {
            lr[i].SetPosition(1, Vector3.forward);
            lr[i + 2].SetPosition(1, Vector3.up);
            lr[i + 4].SetPosition(1, Vector3.right);
        }

        for (int i = 0; i < 3; i++) { 
            vector[i] = new Vector(vectorShaft[i], vectorHead[i], true);
            vector[i].setActive(false);
        }
        _zTranslationAxis = vector[0]; _yTranslationAxis = vector[1]; _xTranslationAxis = vector[2];
        _zTranslationAxis.setName("z");//testing
        _yTranslationAxis.setName("y");//testing
        _xTranslationAxis.setName("x");//testing
    }

    public void createScaleHandle()
    {
        if (_scaleAxis != null) { _scaleAxis.destroy(); _scaleAxis = null; }
        if (_constraint.localScale == Vector3.zero) { return; }
        if(_hitBox == null) { createHitBoxForOutside(); }//testing

        GameObject vectorHead = new GameObject();
        setLayerMask(vectorHead, LayerMask.NameToLayer("Handle"));
        vectorHead.transform.position = _constraint.position;
        vectorHead.transform.SetParent(_constraint);
        vectorHead.transform.localScale = Vector3.one;

        GameObject vectorShaft = new GameObject();
        setLayerMask(vectorShaft, LayerMask.NameToLayer("Handle"));
        vectorShaft.transform.position = _constraint.position;
        vectorShaft.transform.SetParent(_constraint);
        vectorShaft.transform.localScale = Vector3.one;

        LineRenderer[] lr = new LineRenderer[2]{ vectorShaft.AddComponent<LineRenderer>(), vectorHead.AddComponent<LineRenderer>()};
        for(int i = 0; i < 2; i++)
        {
            lr[i].useWorldSpace = false; //must be turned to false to use parenet-child between gameobjects
            lr[i].sharedMaterial = new Material(Shader.Find("Unlit/Color"));
            lr[i].sharedMaterial.doubleSidedGI = true; //both sides are non-transparent
            lr[i].sharedMaterial.color = Color.black;
            lr[i].positionCount = 2;
        }
        lr[0].SetPosition(0, Vector3.zero);
        //lr[0].SetPosition(1, Vector3.one.normalized);
        lr[0].SetPosition(1, Vector3.forward);//testing
        float size = (1f / 3f);
        //lr[1].SetPosition(0, (Vector3.one.normalized - Vector3.zero) - (Vector3.one.normalized - Vector3.zero) * size);
        //lr[1].SetPosition(1, Vector3.one.normalized);
        lr[1].SetPosition(0, (Vector3.forward - Vector3.zero) - (Vector3.forward - Vector3.zero) * size);//testing
        lr[1].SetPosition(1, Vector3.forward);//testing
        vectorShaft.transform.localRotation = Quaternion.FromToRotation(Vector3.forward, Vector3.one.normalized);
        vectorHead.transform.localRotation = Quaternion.FromToRotation(Vector3.forward, Vector3.one.normalized);
        lr[0].startWidth = 0.05f;
        lr[0].endWidth = 0.05f;
        lr[1].startWidth = 1f * size;
        lr[1].endWidth = 1f * size;

        Vector vector = new Vector(vectorShaft, vectorHead, false);
        _scaleAxis = vector;
        _scaleAxis.setActive(false);

    }

    public void createHitBoxForOutside()
    {
        _hitBox = GameObject.CreatePrimitive(PrimitiveType.Cube);
        setLayerMask(_hitBox, LayerMask.NameToLayer("HitBox"));
        _hitBox.transform.position = _constraint.position;
        _hitBox.transform.SetParent(_constraint);
        _hitBox.transform.localScale = Vector3.one;
        Material mat = _hitBox.GetComponent<MeshRenderer>().material;
        mat.shader = Shader.Find("Standard");
        mat.SetOverrideTag("RenderType", "Transparent");
        mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
        mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        mat.SetInt("_ZWrite", 0);
        mat.DisableKeyword("_ALPHATEST_ON");
        mat.EnableKeyword("_ALPHABLEND_ON");
        mat.DisableKeyword("_ALPHAPREMULTIPLY_ON");
        mat.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;
        mat.color = new Color(0, 0, 0, 0);
        _hitBox.SetActive(false);
    }

    public void adjustScaleOnRotationHandles()
    {
        if (_constraint == null) { return; }
        if (_target == null) { return; }
        if (_constraint.GetComponent<UnityEngine.Animations.ParentConstraint>().GetSource(0).sourceTransform == null) { return; }
        if ((_zRotationAxis == null) || (_yRotationAxis == null) || (_xRotationAxis == null)) { createRotationHandles(); }
        if(_selectedHandle != HandleType.RotationTool) { return; }

        //adjusting the size of the circles made with line renderer
        float[] dimensions = new float[3] { _target.lossyScale.x, _target.lossyScale.y, _target.lossyScale.z };
        float radius = dimensions.Max();
        float outsideOfTarget = 1f;
        GameObject[] rotationAxis = new GameObject[3] { _zRotationAxis, _xRotationAxis, _yRotationAxis };
        LineRenderer[] lr = new LineRenderer[3];
        for(int i = 0; i < 3; i++)
        {
            rotationAxis[i].transform.localScale = Vector3.one * radius * outsideOfTarget;
            lr[i] = rotationAxis[i].GetComponent<LineRenderer>();
            lr[i].startWidth = Mathf.LerpUnclamped(0.05f, 1f, radius / 100f);
            lr[i].endWidth = Mathf.LerpUnclamped(0.05f, 1f, radius / 100f);
            createMeshCollider(lr[i].gameObject.transform);
        }
    }

    public void adjustScaleOnTranslationHandles()
    {
        if (_constraint == null) { return; }
        if (_target == null) { return; }
        if (_constraint.GetComponent<UnityEngine.Animations.ParentConstraint>().GetSource(0).sourceTransform == null) { return; }
        if ((_zTranslationAxis == null) || (_yTranslationAxis == null) || (_xTranslationAxis == null)) { createTranslationHandles(); }
        if (_selectedHandle != HandleType.TranslationTool) { return; }

        float[] dimensions = new float[3] { _target.lossyScale.x, _target.lossyScale.y, _target.lossyScale.z};
        float scale = dimensions.Max() + 1;
        if ((dimensions.Max() > 0) && (_selectedHandle == HandleType.TranslationTool))
        {
            _zTranslationAxis.setActive(true);
            _yTranslationAxis.setActive(true);
            _xTranslationAxis.setActive(true);
            _zTranslationAxis.scaleVector(scale);
            _yTranslationAxis.scaleVector(scale);
            _xTranslationAxis.scaleVector(scale);
        }
        else
        {
            _zTranslationAxis.setActive(false);
            _yTranslationAxis.setActive(false);
            _xTranslationAxis.setActive(false);
        }
        _zTranslationAxis.createCollider(this);
        _yTranslationAxis.createCollider(this);
        _xTranslationAxis.createCollider(this);
    }

    public void adjustScaleOnScaleHandle()
    {
        if (_constraint == null) { return; }
        if (_target == null) { return; }
        if (_constraint.GetComponent<UnityEngine.Animations.ParentConstraint>().GetSource(0).sourceTransform == null) { return; }
        if (_scaleAxis == null) { createScaleHandle(); }
        if (_selectedHandle != HandleType.ScaleTool) { return; }

        float[] dimensions = new float[3] { _target.lossyScale.x, _target.lossyScale.y, _target.lossyScale.z };
        float scale = dimensions.Max() + 1;
        if((dimensions.Max() > 0) && (_selectedHandle == HandleType.ScaleTool))
        {
            _scaleAxis.setActive(true);
            _scaleAxis.scaleVector(scale);
        }
        else
        {
            _scaleAxis.setActive(false);
        }
        _scaleAxis.createCollider(this);

    }

    public void setScaleOnHitBox(GameObject handle)
    {
        if (handle == null) { _hitBox.SetActive(false); }

        _hitBox.transform.localRotation = Quaternion.identity; //reset
        _hitBox.transform.localScale = Vector3.one; //reset
        float valOut = 100; //outside Of Target
        float valMin = Mathf.Epsilon; //lowest positive float value
        if (_selectedHandle == HandleType.TranslationTool)
        {
            _hitBox.SetActive(true);
            Vector3 fromRotationDirection_1;
            Vector3 fromRotationDirection_2;
            Vector3 dimensions_1;
            Vector3 dimensions_2;
            if (_zTranslationAxis.compareHandle(handle)) 
            { 
                dimensions_1 = new Vector3(valMin, valOut, valOut);//plane cutting x-axis
                dimensions_2 = new Vector3(valOut, valMin, valOut);//plane cutting y-axis
                //_hitBox.transform.localScale = new Vector3(valOut, valOut, valMin);//plane cutting z-axis; bad
                fromRotationDirection_1 = _hitBox.transform.right;
                fromRotationDirection_2 = _hitBox.transform.up;
            }
            else if (_yTranslationAxis.compareHandle(handle)) 
            { 
                dimensions_1 = new Vector3(valOut, valOut, valMin);//plane cutting z-axis
                dimensions_2 = new Vector3(valMin, valOut, valOut);//plane cutting x-axis
                //_hitBox.transform.localScale = new Vector3(valOut, valMin, valOut);//plane cutting y-axis; bad
                fromRotationDirection_1 = _hitBox.transform.forward;
                fromRotationDirection_2 = _hitBox.transform.right;
            }
            else if (_xTranslationAxis.compareHandle(handle)) 
            {
                dimensions_1 = new Vector3(valOut, valOut, valMin);//plane cutting z-axis
                dimensions_2 = new Vector3(valOut, valMin, valOut);//plane cutting y-axis
                //_hitBox.transform.localScale = new Vector3(valMin, valOut, valOut);//plane cutting x-axis; bad
                fromRotationDirection_1 = _hitBox.transform.forward;
                fromRotationDirection_2 = _hitBox.transform.up;
            }
            else 
            { 
                _hitBox.SetActive(false); return; 
            }
            Vector3 toRotationDirection = Camera.main.transform.position - _constraint.position;
            Vector3[] dimensions = new Vector3[4] { dimensions_1, dimensions_1, dimensions_2, dimensions_2 };
            float[] angle = new float[4] { 
                Vector3.Angle(fromRotationDirection_1, toRotationDirection), Vector3.Angle(-fromRotationDirection_1, toRotationDirection),
                Vector3.Angle(fromRotationDirection_2, toRotationDirection), Vector3.Angle(-fromRotationDirection_2, toRotationDirection)};
            float angleCompare = angle[0];
            int angleIndex = 0;
            for(int i = 1; i < angle.Length; i++)
            {
                if(angleCompare >= angle[i]) { angleCompare = angle[i]; angleIndex = i; }
            }
            _hitBox.transform.localScale = dimensions[angleIndex];
        }
        else if (_selectedHandle == HandleType.ScaleTool)
        {
            _hitBox.SetActive(true);
            Vector3 fromRotationDirection_1;
            Vector3 fromRotationDirection_2;
            Vector3 dimensions_1;
            Vector3 dimensions_2;
            if (_scaleAxis.compareHandle(handle))
            {
                dimensions_1 = new Vector3(valMin, valOut, valOut);//plane cutting x-axis
                dimensions_2 = new Vector3(valOut, valMin, valOut);//plane cutting y-axis
                //_hitBox.transform.localScale = new Vector3(valOut, valOut, valMin);//plane cutting z-axis; bad
                fromRotationDirection_1 = _hitBox.transform.right;
                fromRotationDirection_2 = _hitBox.transform.up;
            }
            else 
            { 
                _hitBox.SetActive(false); return; 
            }
            Vector3 toRotationDirection = Camera.main.transform.position - _constraint.position;
            Vector3[] dimensions = new Vector3[4] { dimensions_1, dimensions_1, dimensions_2, dimensions_2 };
            float[] angle = new float[4] {
                Vector3.Angle(fromRotationDirection_1, toRotationDirection), Vector3.Angle(-fromRotationDirection_1, toRotationDirection),
                Vector3.Angle(fromRotationDirection_2, toRotationDirection), Vector3.Angle(-fromRotationDirection_2, toRotationDirection)};
            float angleCompare = angle[0];
            int angleIndex = 0;
            for (int i = 1; i < angle.Length; i++)
            {
                if (angleCompare >= angle[i]) { angleCompare = angle[i]; angleIndex = i; }
            }
            _hitBox.transform.localScale = dimensions[angleIndex];
            _hitBox.transform.localRotation = _scaleAxis.initialLocalRotation;
        }
        else if (_selectedHandle == HandleType.RotationTool)
        {
            _hitBox.SetActive(true);
            if (_zRotationAxis == handle)
            {
                _hitBox.transform.localScale = new Vector3(valOut, valOut, valMin);//plane cutting z-axis; bad
            }
            else if (_yRotationAxis == handle)
            {
                _hitBox.transform.localScale = new Vector3(valOut, valMin, valOut);//plane cutting y-axis;
            }
            else if (_xRotationAxis == handle)
            {
                _hitBox.transform.localScale = new Vector3(valMin, valOut, valOut);//plane cutting x-axis

            }
            else
            {
                _hitBox.SetActive(false); return;
            }
        }
    }

    public void createMeshCollider(Transform handle) 
    {
        MeshCollider col;
        LineRenderer lr;
        if (!handle.TryGetComponent<LineRenderer>(out lr)) { return; }
        if (!handle.TryGetComponent<MeshCollider>(out col))
        {
            col = handle.gameObject.AddComponent<MeshCollider>();
        }
        Mesh mesh = new Mesh();
        lr.BakeMesh(mesh, false);
        col.sharedMesh = mesh;
        if(lr.positionCount < 4) { return; }
        col.convex = true;
    }

    internal class Vector
    {
        private readonly GameObject _vectorShaft;
        private readonly GameObject _vectorHead;
        internal bool Empty { get => ((_vectorHead == null) || (_vectorShaft == null)); }
        internal bool _triangle;
        internal Vector3 _localDirection;
        internal Quaternion _initialLocalRotation;
        internal Vector3 localDirection { get => _localDirection; }
        internal Quaternion initialLocalRotation { get => _initialLocalRotation; }
        public Vector(GameObject vectorShaft, GameObject vectorHead, bool triangle)
        {
            _vectorShaft = vectorShaft;
            _vectorHead = vectorHead;
            _triangle = triangle;
            _initialLocalRotation = vectorShaft.transform.localRotation;
            LineRenderer lr = _vectorShaft.GetComponent<LineRenderer>();
            Vector3[] vectorShaftPoints = new Vector3[lr.positionCount];
            lr.GetPositions(vectorShaftPoints);
            int len = vectorShaftPoints.Length;
            Matrix4x4 mat = Matrix4x4.Rotate(_initialLocalRotation);
            Vector3 localDirection = mat * (vectorShaftPoints[len - 1] - vectorShaftPoints[0]).normalized;
            _localDirection = localDirection;
        }

        internal void destroy()
        {
            GameObject.Destroy(_vectorShaft);
            GameObject.Destroy(_vectorHead);
        }

        internal void setActive(bool b)
        {
            _vectorShaft.SetActive(b);
            _vectorHead.SetActive(b);
        }

        internal void setLocalScale(Vector3 s)
        {
            _vectorShaft.transform.localScale = s;
            _vectorHead.transform.localScale = s;
        }

        internal void scaleVector(float scale)
        {
            Vector3[] vectorShaftPoints = new Vector3[_vectorShaft.GetComponent<LineRenderer>().positionCount];
            Vector3[] vectorHeadPoints = new Vector3[_vectorHead.GetComponent<LineRenderer>().positionCount];
            LineRenderer vslr = _vectorShaft.GetComponent<LineRenderer>();
            LineRenderer vhlr = _vectorHead.GetComponent<LineRenderer>();
            vslr.GetPositions(vectorShaftPoints);
            vhlr.GetPositions(vectorHeadPoints);
            int len = vectorShaftPoints.Length;
            Vector3 direction = (vectorShaftPoints[len - 1] - vectorShaftPoints[0]).normalized;
            Vector3 endPoint = direction * scale;
            vslr.SetPosition(len - 1, endPoint);
            vhlr.SetPosition(0, endPoint - endPoint * (1f / 10f));
            vhlr.SetPosition(len - 1, endPoint);
            vslr.startWidth = Mathf.LerpUnclamped(0.05f, 1f, scale / 100f);
            vslr.endWidth = Mathf.LerpUnclamped(0.05f, 1f, scale / 100f);
            vhlr.startWidth = Mathf.LerpUnclamped((2f / 3f), 1f, scale / 5f);
            if (_triangle) { vhlr.endWidth = 0; }
            else { vhlr.endWidth = Mathf.LerpUnclamped((2f / 3f), 1f, scale / 5f); }
        }

        //mesh collider is bad without convex, I think. At least this one (without convex) doesn't work at the back
        public void createCollider(Handles handles)
        {
            handles.createMeshCollider(_vectorHead.transform);
            //handles.createMeshCollider(_vectorShaft.transform);
            BoxCollider col;
            if(!_vectorShaft.TryGetComponent<BoxCollider>(out col)) { col = _vectorShaft.AddComponent<BoxCollider>(); }
            col.size = Vector3.one; //reset collider scale
            col.center = Vector3.zero; //reset the pivot point of the game object
            LineRenderer lr = _vectorHead.GetComponent<LineRenderer>();
            Vector3 direction = lr.GetPosition(lr.positionCount - 1).normalized;
            float magnitude = lr.GetPosition(lr.positionCount - 1).magnitude;
            float size = Mathf.LerpUnclamped(1f, 2f, magnitude / 100f);
            if (direction == Vector3.right) { col.size = new Vector3(magnitude, size, size); col.center = Vector3.right * (magnitude / 2);  }
            else if (direction == Vector3.up) { col.size = new Vector3(size, magnitude, size); col.center = Vector3.up * (magnitude / 2); }
            else if (direction == Vector3.forward) { col.size = new Vector3(size, size, magnitude); col.center = Vector3.forward * (magnitude / 2); }
        }

        //testing
        public void setName(string s)
        {
            _vectorHead.gameObject.name = s + "Head";
            _vectorShaft.gameObject.name = s + "Shaft";
        }

        public bool compareHandle(GameObject handle)
        {
            if (_vectorHead == handle) { return true; }
            return _vectorShaft == handle;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Main : MonoBehaviour
{
    private Handles _handles;
    [SerializeField] private Transform _cube;
    [SerializeField] private Transform _cube2;
    [SerializeField] private int selected = 0;
    private int _selectedPrevState = 0;
    //private GameObject leftHandController;
    //private myLeftRayCastHit _script;

    private void Awake()
    {
        //leftHandController = GameObject.Find("LeftHand Controller");
        //if (!leftHandController.TryGetComponent<myLeftRayCastHit>(out _script)) { _script = leftHandController.AddComponent<myLeftRayCastHit>(); }
        //my plan is to have a Mono script where the flow is controller from, and then instances from other scripts (not Mono scripts) to be used.
        //an interface for lefthand and righthand, we know what methods and where to place them.
    }
    void Start()
    {
        //_handles = new Handles(_cube);
        //_script._handles = _handles;
        //_handles = new Handles(_cube);
        //GameObject axis = _handles._zRotationAxis;
        //int size = axis.GetComponent<LineRenderer>().positionCount;
        //int index = (int)((size / 4) * 2);
        //Vector3 whereItBeginsOnCircle = axis.GetComponent<LineRenderer>().GetPosition(index);
        //_handles.rotateCircleSector(whereItBeginsOnCircle, axis.transform);
        //int angle = 90;
        //_handles.changeAngleOfCircleSector(angle);
        //_handles.selectTool(Handles.HandleType.TranslationTool);
        Debug.Log(_cube.GetComponent<MeshFilter>().name);

    }
    // Update is called once per frame
    /*private void Update()
    {
        if(_selectedPrevState != selected)
        {
            switch (selected)
            {
                case 0: _handles.selectTool(Handles.HandleType.TranslationTool); _selectedPrevState = selected; return;
                case 1: _handles.selectTool(Handles.HandleType.RotationTool); _selectedPrevState = selected; return;
                case 2: _handles.selectTool(Handles.HandleType.ScaleTool); _selectedPrevState = selected; return;
                default: selected = _selectedPrevState; return;
            }
        }
    }
    void LateUpdate()
    {
        if(_handles._target == null) { return; }
        if (_handles._target.hasChanged)
        {
            _handles.adjustScaleOnScaleHandle();
            _handles.adjustScaleOnTranslationHandles();
            _handles.adjustScaleOnRotationHandles();
        }
    }*/
    //private void FixedUpdate() =>_handles.updateRotCirkelSector();
    /// <summary>
    /// Find the index for line renderer array in inputted axis game object with the help of inputted collider hit. 
    /// The purpose is to be used in rotateCircleSektor(Vector3, GameObject axis).
    /// </summary>
    /// <param name="hit"> Collider hit </param>
    /// <param name="axis"> Rotation axis handle </param>
    /// <returns> Index for getting a position in line renderer array on the axis parameter</returns>
    public int findClosestPointHit(Vector3 hit, GameObject axis)
    {
        Vector3 hitOnUnitCircle = hit.normalized;
        float targetAngle = Mathf.Asin(hitOnUnitCircle.x) * Mathf.Rad2Deg;
        int len = axis.GetComponent<LineRenderer>().positionCount;
        Vector3[] arr = new Vector3[len];
        axis.GetComponent<LineRenderer>().GetPositions(arr);
        float max = float.MaxValue;
        float angle;
        int index = 0;
        for (int i = 0; i < len; i++)
        {
            /*if(axis == _handles.xRotationAxis) { angle = Mathf.Asin(arr[i].z) * Mathf.Rad2Deg; }
            else if( axis == _handles.zRotationAxis) { angle = Mathf.Asin(arr[i].x) * Mathf.Rad2Deg; }
            else { angle = Mathf.Asin(arr[i].x) * Mathf.Rad2Deg; }
            
            if (max > Mathf.Abs(angle - targetAngle)){ max = Mathf.Abs(angle - targetAngle); }
            else { index = i - 1; break; }*/
        }
        return index;
    }
}

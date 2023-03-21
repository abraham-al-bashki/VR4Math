using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using TMPro;
using UnityEngine.InputSystem;
using System.Linq;

public class myLeftRayCastHit
{
    private Transform _leftHandController;
    private Controller _controller;
    private LineRenderer _lr;
    private Vector3 _origin;
    private Vector3 _direction;
    XRIDefaultInputActions _inputActions;
    private Coroutine _holdPressing;
    private uint _id;
    private Handles _handles;
    private bool _flag;
    private bool _rotateFlag;
    private bool _translateFlag;
    private bool _scalingFlag;
    private GameObject _VRSimulator;

    public myLeftRayCastHit(Controller controller, Handles handles, GameObject? leftHandController)
    {
        _controller = controller;
        _VRSimulator = GameObject.Find("XR Device Simulator");
        try {
            if (leftHandController == null) { _leftHandController = GameObject.Find("LeftHand Controller").transform; }
            else { _leftHandController = leftHandController.transform; }
        } catch (NullReferenceException e) { _leftHandController = GameObject.Instantiate(Resources.Load<GameObject>("SceneAssets/XR Origin").GetComponentInChildren<LineRenderer>().transform); }
        _handles = handles; 
        
    }
    public void myOnEnable()
    {
        _inputActions.LeftTrigger.Enable();
        _inputActions.LeftPrimary.Enable();
        _inputActions.LeftSecondary.Enable();
    }
    public void myOnDisable()
    {
        _inputActions.LeftTrigger.Disable();
        _inputActions.LeftPrimary.Disable();
        _inputActions.LeftSecondary.Disable();
    }
    public void myAwake()
    {
        _inputActions = new XRIDefaultInputActions();
        _inputActions.LeftTrigger.LeftTriggerPress.performed += context => rayCastInteraction(context);
        _inputActions.LeftTrigger.LeftTriggerRelease.performed += context => rayCastInteraction(context);
        _inputActions.LeftPrimary.LeftPrimaryPress.performed += context => rayCastInteraction(context);
        _inputActions.LeftPrimary.LeftPrimaryRelease.performed += context => rayCastInteraction(context);
        _inputActions.LeftSecondary.LeftSecondaryPress.performed += context => rayCastInteraction(context);
        _inputActions.LeftSecondary.LeftSecondaryRelease.performed += context => rayCastInteraction(context);
    }
    public void myUpdate()
    {
        if (updateRayCastDirection())
        {
            /*if (Physics.Raycast(_origin, _direction, out RaycastHit hit, Mathf.Infinity))
            {
                //Debug.Log("hovering");
                if (hit.transform.TryGetComponent<LineRenderer>(out LineRenderer comp))
                {
                    //Debug.Log("color: " + comp.material.color);
                }
                
            }*/
        }
    }

    private bool updateRayCastDirection() {
#if UNITY_EDITOR
        if(_VRSimulator == null)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            _origin = ray.origin;
            _direction = ray.direction;
            return true;
        }
        else
        {
            _lr = _leftHandController.GetComponent<LineRenderer>();
            if (_lr.positionCount > 1)
            {
                _origin = _lr.GetPosition(0);
                _direction = (_lr.GetPosition(1) - _lr.GetPosition(0)).normalized;
                return true;
            }
            return false;
        }
#else
        _lr = _leftHandController.GetComponent<LineRenderer>();
        if (_lr.positionCount > 1)
        {
            _origin = _lr.GetPosition(0);
            _direction = (_lr.GetPosition(1) - _lr.GetPosition(0)).normalized;
            return true;
        }
        return false;
#endif
    }

    public void rayCastInteraction(InputAction.CallbackContext context)
    {
        Debug.Log("Works!");
        if(context.canceled && (_holdPressing != null)) 
        { 

        }
        if (!context.performed) { return; }
        if (_holdPressing != null) { }
        else if (_inputActions.LeftTrigger.LeftTriggerPress.name.Equals(context.action.name))
        {
            updateRayCastDirection();
            RaycastHit hit;
            if (Physics.Raycast(_origin, _direction, out hit, Mathf.Infinity, LayerMask.GetMask("LA_Object"))) 
            {
                LA_Object_Component potentialNewTarget = (hit.transform.parent.GetComponent<LA_Object_Component>());
                _flag = (_controller.getTarget != potentialNewTarget.laobject) && (potentialNewTarget.laobject != null);
                if (_flag) { _controller.selectTarget(potentialNewTarget.laobject); }
            }

            if (_flag) { _flag = false; }
            else if(_handles.selectedHandle == Handles.HandleType.TranslationTool) 
            {
                Debug.Log("Coro Active");
                _id = 1;
                _translateFlag = true;
                _holdPressing = _controller.StartCoroutine(translation(context));
            }
            else if (_handles.selectedHandle == Handles.HandleType.RotationTool)
            {
                Debug.Log("Coro Active");
                _id = 2;
                _rotateFlag = true;
                _holdPressing = _controller.StartCoroutine(rotation(context));
            }
            else if (_handles.selectedHandle == Handles.HandleType.ScaleTool)
            {
                Debug.Log("Coro Active");
                _id = 3;
                _scalingFlag = true;
                _holdPressing = _controller.StartCoroutine(scale(context));
            }
        }
        else if (_inputActions.LeftPrimary.LeftPrimaryPress.name.Equals(context.action.name))
        {
            Debug.Log("Coro Active");
            _id = 4;
            if(_handles.selectedHandle == Handles.HandleType.TranslationTool) { _handles.selectHandle(Handles.HandleType.RotationTool); }
            else if (_handles.selectedHandle == Handles.HandleType.RotationTool) { _handles.selectHandle(Handles.HandleType.ScaleTool); }
            else if (_handles.selectedHandle == Handles.HandleType.ScaleTool) { _handles.selectHandle(Handles.HandleType.TranslationTool); }

        }
        else if (_inputActions.LeftSecondary.LeftSecondaryPress.name.Equals(context.action.name))
        {
            Debug.Log("Coro Active");
            _id = 5;
            if (_handles.selectedHandle == Handles.HandleType.TranslationTool) { _handles.selectHandle(Handles.HandleType.ScaleTool); }
            else if (_handles.selectedHandle == Handles.HandleType.RotationTool) { _handles.selectHandle(Handles.HandleType.TranslationTool); }
            else if (_handles.selectedHandle == Handles.HandleType.ScaleTool) { _handles.selectHandle(Handles.HandleType.RotationTool); }
        }
        else 
        {
            return;
        }
        if (_holdPressing == null) { }
        else if (_inputActions.LeftTrigger.LeftTriggerRelease.name.Equals(context.action.name) && (_id == 1))
        {
            Debug.Log("Coro Deactive");
            //_controller.StopCoroutine(_holdPressing);
            //_handles.setScaleOnHitBox(null);
            _translateFlag = false;
            _holdPressing = null;
            _id = 0;//translate
        }
        else if (_inputActions.LeftTrigger.LeftTriggerRelease.name.Equals(context.action.name) && (_id == 2))
        {
            Debug.Log("Coro Deactive");
            //_controller.StopCoroutine(_holdPressing);
            //_handles.setScaleOnHitBox(null);
            //_handles.setParentHitBox(true);
            _rotateFlag = false;
            _holdPressing = null;
            _id = 0;//rotate
        }
        else if (_inputActions.LeftTrigger.LeftTriggerRelease.name.Equals(context.action.name) && (_id == 3))
        {
            Debug.Log("Coro Deactive");
            //_controller.StopCoroutine(_holdPressing);
            //_handles.setScaleOnHitBox(null);
            _scalingFlag = false;
            _holdPressing = null;
            _id = 0;
        }
    }
    public IEnumerator rotation(InputAction.CallbackContext context)
    {
        RaycastHit hit;
        Debug.Log("Goes into rotation method!");
        if (Physics.Raycast(_origin, _direction, out hit, Mathf.Infinity, LayerMask.GetMask("Handle")))
        {
            Debug.Log("HIT");
            Quaternion initialRotation = hit.transform.root.rotation;
            Transform handle = hit.transform;
            Transform constraintManager = handle.root;
            Vector3 initialCollidingPoint = constraintManager.InverseTransformPoint(hit.point).normalized;
            float start = Time.time;
            float duration = 20f;
            _handles.setScaleOnHitBox(handle.gameObject);
            Vector3 axis = _handles.chooseAxisBasedOnCamera(handle.gameObject);
            int index;
            float[] angle = new float[3] { 0, 0, 0};
            if ((axis == Vector3.right) || (axis == Vector3.left)) { index = 0; }
            else if ((axis == Vector3.up) || (axis == Vector3.down)) { index = 1; }
            else { index = 2; }
            _handles.setParentHitBox(false);
            while (_rotateFlag)
            {
                yield return new WaitForSeconds(0.1f);
                if ((Time.time - start) > duration) { _rotateFlag = false; break; }
                RaycastHit hitAgain;
                if (updateRayCastDirection() && Physics.Raycast(_origin, _direction, out hitAgain, Mathf.Infinity, LayerMask.GetMask("HitBox")))
                {
                    Debug.Log("Hitx2");
                    Vector3 otherCollidingPoint = _handles._hitBox.transform.InverseTransformPoint(hitAgain.point).normalized;
                    angle[index] = Vector3.SignedAngle(initialCollidingPoint, otherCollidingPoint, axis);
                    _controller.getTarget.getGameObject().transform.rotation = initialRotation * Quaternion.Euler(angle[0],angle[1],angle[2]);
                    if(_controller.getTarget is LA_Vector)
                    {
                        _controller.getTarget.getGameObject().transform.parent.GetChild(1).rotation = initialRotation * Quaternion.Euler(angle[0], angle[1], angle[2]);
                    }
                }
            }
            _controller.getTarget.getGameObject().transform.rotation = initialRotation;//reset
            if (_controller.getTarget is LA_Vector)
            {
                _controller.getTarget.getGameObject().transform.parent.GetChild(1).rotation = initialRotation;//reset
            }
            _controller.addLA_Object_Rotate(new Vector3(angle[0], angle[1], angle[2]));//final
            _handles.setScaleOnHitBox(null);
            _handles.setParentHitBox(true);
        }
        yield return 0;
    }

    public IEnumerator translation(InputAction.CallbackContext context)
    {
        RaycastHit hit;
        Debug.Log("Goes into translation method!");
        if (Physics.Raycast(_origin, _direction, out hit, Mathf.Infinity, LayerMask.GetMask("Handle")))
        {
            Debug.Log("HIT");
            Transform handle = hit.transform;
            Transform constraintManager = handle.root;
            Vector3 axis = handle.GetComponent<LineRenderer>().GetPosition(1).normalized;
            float start = Time.time;
            float duration = 20f;
            Vector3 initialCollidingPoint = constraintManager.InverseTransformPoint(hit.point);
            _handles.setScaleOnHitBox(handle.gameObject);
            Vector3 initialPositionOfTarget = _controller.getTarget.getGameObject().transform.position;
            Vector3 deltaAxis = constraintManager.InverseTransformPoint(initialPositionOfTarget);
            Vector3 otherCollidingPoint = initialCollidingPoint;
            while (_translateFlag)
            {
                yield return new WaitForSeconds(0.1f);
                if ((Time.time - start) > duration) { _translateFlag = false; break; }
                RaycastHit hitAgain;
                if (updateRayCastDirection() && Physics.Raycast(_origin, _direction, out hitAgain, Mathf.Infinity, LayerMask.GetMask("HitBox")))
                {
                    Debug.Log("Hitx2");
                    otherCollidingPoint = constraintManager.InverseTransformPoint(hitAgain.point);
                    deltaAxis = (otherCollidingPoint - initialCollidingPoint);
                    if(axis == Vector3.right) { deltaAxis.z = 0; deltaAxis.y = 0; }
                    else if (axis == Vector3.up) { deltaAxis.z = 0; deltaAxis.x = 0; }
                    else if (axis == Vector3.forward) { deltaAxis.x = 0; deltaAxis.y = 0; }
                    _controller.getTarget.getGameObject().transform.position = constraintManager.TransformPoint(deltaAxis);
                    if (_controller.getTarget is LA_Vector)
                    {
                        _controller.getTarget.getGameObject().transform.parent.GetChild(1).position = constraintManager.TransformPoint(deltaAxis);
                    }
                }
            }
            _controller.getTarget.getGameObject().transform.position = initialPositionOfTarget;//reset
            if (_controller.getTarget is LA_Vector)
            {
                _controller.getTarget.getGameObject().transform.parent.GetChild(1).position = initialPositionOfTarget;//reset
            }
            _controller.addLA_Object_Translation(constraintManager.TransformPoint(deltaAxis));//final
            _handles.setScaleOnHitBox(null);
            yield return 0;
        }
    }

    public IEnumerator scale(InputAction.CallbackContext context)
    {
        RaycastHit hit;
        Debug.Log("Goes into scale method!");
        if (Physics.Raycast(_origin, _direction, out hit, Mathf.Infinity, LayerMask.GetMask("Handle")))
        {
            Debug.Log("HIT");
            Transform handle = hit.transform;
            Transform constraintManager = handle.root;
            float start = Time.time;
            float duration = 20f;
            Vector3 initialCollidingPoint = constraintManager.InverseTransformPoint(hit.point);
            _handles.setScaleOnHitBox(handle.gameObject);
            Vector3 intialScaleVector = _controller.getTarget.getScaling().lossyScale;
            float distance = 1f;
            while (_scalingFlag)
            {
                yield return new WaitForSeconds(0.1f);
                if ((Time.time - start) > duration) { _scalingFlag = false; break; }
                RaycastHit hitAgain;
                if (updateRayCastDirection() && Physics.Raycast(_origin, _direction, out hitAgain, Mathf.Infinity, LayerMask.GetMask("HitBox")))
                {
                    Debug.Log("Hitx2");
                    Vector3 otherCollidingPoint = constraintManager.InverseTransformPoint(hitAgain.point);
                    Vector3 deltaZAxis = (otherCollidingPoint - initialCollidingPoint);
                    deltaZAxis.x = 0; deltaZAxis.y = 0;
                    distance = deltaZAxis.magnitude;
                    _controller.getTarget.getGameObject().transform.localScale = intialScaleVector * distance;
                    if (_controller.getTarget is LA_Vector)
                    {
                        _controller.getTarget.getGameObject().transform.parent.GetChild(1).localScale = intialScaleVector * distance;
                    }
                }
            }
            _controller.getTarget.getGameObject().transform.localScale = intialScaleVector;//reset
            if (_controller.getTarget is LA_Vector)
            {
                _controller.getTarget.getGameObject().transform.parent.GetChild(1).localScale = intialScaleVector;//reset
            }
            _controller.addLA_Object_Scale(distance);//final
            _handles.setScaleOnHitBox(null);
            yield return 0;
        }
    }

    private float calculateAngle(Vector3 from, Vector3 to, Vector3 axis)
    {
        float angle_Unity = Vector3.SignedAngle(from, to, axis);
        if(angle_Unity > 0) { return angle_Unity; }
        return 360 + angle_Unity;
    }
}

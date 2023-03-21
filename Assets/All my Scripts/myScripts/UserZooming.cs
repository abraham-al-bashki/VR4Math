using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserZooming
{
    private Transform _cameraOffset;
    private float _scroll;
    private float _currentInternalScroll;
    private float _maxInternalScroll;
    private float _minInternalScroll;
    private Controller _contoller;
    Vector3 _maxPositionOffset;
    Vector3 _maxEulerAnglesOffset;
    public UserZooming(Controller controller)
    {
        _contoller = controller;
        _scroll = Input.mouseScrollDelta.y; //presumably zero at no rotation of mouse wheel
        _currentInternalScroll = 0;
        _maxInternalScroll = 10;
        _minInternalScroll = 0;
        _cameraOffset = Camera.main.transform.parent;
        _maxPositionOffset = new Vector3(0, 3, -10);
        _maxEulerAnglesOffset = new Vector3(10, 0, 0);
    }

    public void myLateUpdate()
    {
        //Debug.Log(Input.mouseScrollDelta.y);
        if(Input.mouseScrollDelta.y != 0)
        {
            Debug.Log(Input.mouseScrollDelta.y);
            if (!_contoller.MenuBarVisible) { return; }
            else if ((- Input.mouseScrollDelta.y > 0) && (_currentInternalScroll <= _maxInternalScroll)) //scroll up
            {
                float yOffset = Mathf.Lerp(0, _maxPositionOffset.y, _currentInternalScroll / _maxInternalScroll);
                float zOffset = Mathf.Lerp(0, _maxPositionOffset.z, _currentInternalScroll / _maxInternalScroll);
                float xRotationOffset = Mathf.Lerp(0, _maxEulerAnglesOffset.x, _currentInternalScroll / _maxInternalScroll);
                _cameraOffset.localPosition = new Vector3(0, yOffset, zOffset);
                _cameraOffset.localEulerAngles = new Vector3(xRotationOffset, 0, 0);
                _currentInternalScroll++;
            }
            else if ((- Input.mouseScrollDelta.y < 0) && (_currentInternalScroll >= _minInternalScroll)) //scroll down
            {
                float yOffset = Mathf.Lerp(0, _maxPositionOffset.y, _currentInternalScroll / _maxInternalScroll);
                float zOffset = Mathf.Lerp(0, _maxPositionOffset.z, _currentInternalScroll / _maxInternalScroll);
                float xRotationOffset = Mathf.Lerp(0, _maxEulerAnglesOffset.x, _currentInternalScroll / _maxInternalScroll);
                _cameraOffset.localPosition = new Vector3(0, yOffset, zOffset);
                _cameraOffset.localEulerAngles = new Vector3(xRotationOffset, 0, 0);
                _currentInternalScroll--;
            }
            
        }
    }
}

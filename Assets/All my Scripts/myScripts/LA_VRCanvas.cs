using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit.UI;
using System.Linq;
using System;

public class LA_VRCanvas
{
    private Transform _target; 
    private Transform _constraint;
    private UnityEngine.Animations.PositionConstraint _pc;
    private UnityEngine.Animations.ScaleConstraint _sc;
    private UnityEngine.Animations.AimConstraint _ac;
    private GameObject _canvas;
    private Transform _textObj;
    private RectTransform _tectRect;
    private TMP_Text _text;

    public LA_VRCanvas()
    {
        createCanvas();
        createConstraintManager();
        _canvas.transform.SetParent(_constraint, true);
        RectTransform rect = _textObj.GetComponent<RectTransform>();
    }

    public void selectTarget(Transform target)
    {
        _target = target;
        selectObjectForConstraintManager(target);
        _canvas.transform.localPosition = new Vector3(_target.localScale.x, 0, _target.localScale.z);
        /*float[] lossyScale = new float[3] { _target.lossyScale.x, _target.lossyScale.y, _target.lossyScale.z };
        float minScale = lossyScale.Min();
        float origScale = 0.01f;
        _canvas.transform.localScale = minScale * origScale * new Vector3(1 / _target.lossyScale.x, 1 / _target.lossyScale.y, 1 / _target.lossyScale.z);*/
    }
    public void createConstraintManager()
    {
        if (_constraint != null) { return; }
        _constraint = new GameObject().transform;
        _constraint.name = "ContraintManager_LA_Canvas";
        _pc = _constraint.gameObject.AddComponent<UnityEngine.Animations.PositionConstraint>();
        //_sc = _constraint.gameObject.AddComponent<UnityEngine.Animations.ScaleConstraint>();
        _ac = _canvas.gameObject.AddComponent<UnityEngine.Animations.AimConstraint>();
        UnityEngine.Animations.ConstraintSource source = new UnityEngine.Animations.ConstraintSource();
        source.weight = 1; //https://docs.unity3d.com/Manual/Constraints.html
        _pc.AddSource(source);
        //_pc.constraintActive = true;
        //_sc.AddSource(source);
        //_sc.constraintActive = true;

        source = new UnityEngine.Animations.ConstraintSource();
        
        source.sourceTransform = Camera.main.transform;
        
        source.weight = 1; //https://docs.unity3d.com/Manual/Constraints.html
        _ac.aimVector = Vector3.back;
        _ac.AddSource(source);
        _ac.rotationOffset = Vector3.left * 35;
        _ac.constraintActive = true;
    }
    public void selectObjectForConstraintManager(Transform target)
    {
        if (_pc.sourceCount > 0)
        {
            int len = _pc.sourceCount;
            for (int i = 0; i < len; i++) { _pc.RemoveSource(i); }
        }
        /*if (_sc.sourceCount > 0)
        {
            int len = _sc.sourceCount;
            for (int i = 0; i < len; i++) { _sc.RemoveSource(i); }
        }*/
        if (_ac.sourceCount > 0)
        {
            int len = _ac.sourceCount;
            for (int i = 0; i < len; i++) { _ac.RemoveSource(i); }
        }
        UnityEngine.Animations.ConstraintSource source = new UnityEngine.Animations.ConstraintSource();
        source.sourceTransform = target;
        source.weight = 1; //https://docs.unity3d.com/Manual/Constraints.html
        _pc.AddSource(source);
        //_pc.constraintActive = true;
        //_sc.AddSource(source);
        //_sc.constraintActive = true;

        source = new UnityEngine.Animations.ConstraintSource();
        source.sourceTransform = Camera.main.transform;
        source.weight = 1; //https://docs.unity3d.com/Manual/Constraints.html
        _ac.aimVector = Vector3.back;
        _ac.AddSource(source);
        _ac.rotationOffset = Vector3.left * 35;
        _ac.constraintActive = true;
    }
    public void createCanvas()
    {
        _canvas = new GameObject();
        Canvas canvas = _canvas.AddComponent<Canvas>();
        CanvasScaler cs = _canvas.AddComponent<CanvasScaler>();
        _canvas.AddComponent<GraphicRaycaster>();
        _canvas.AddComponent<TrackedDeviceGraphicRaycaster>();
        _canvas.layer = LayerMask.NameToLayer("UI"); 
        _canvas.name = "LA_Canvas";

        RectTransform rect = _canvas.GetComponent<RectTransform>();
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        Vector3 scale = 2 * Vector3.right;
        rect.localScale = scale + Vector3.one * 0.01f; //this make same length as cube with scale (1,1,1).
        
        canvas.worldCamera = Camera.main;
        canvas.renderMode = RenderMode.WorldSpace;
        canvas.additionalShaderChannels |= AdditionalCanvasShaderChannels.TexCoord1;
        canvas.additionalShaderChannels |= AdditionalCanvasShaderChannels.Normal;
        canvas.additionalShaderChannels |= AdditionalCanvasShaderChannels.Tangent;

        cs.dynamicPixelsPerUnit = 1;
        cs.referencePixelsPerUnit = 1;

        _textObj = GameObject.Instantiate(Resources.Load<GameObject>("GUIComponent/Text/Text")).transform;
        _textObj.SetParent(_canvas.transform, false);
        _text = _textObj.GetComponent<TMP_Text>();
        _textObj.gameObject.layer = LayerMask.NameToLayer("UI");
        _textObj.name = "Text (TMP)";

        _tectRect = _textObj.GetComponent<RectTransform>();
        _tectRect.localScale = Vector3.one;
        _tectRect.offsetMax = Vector2.zero;
        _tectRect.anchorMin = Vector2.zero;

        _text.enableAutoSizing = true;
        _text.fontSizeMin = 1;
        _text.fontSizeMax = 200;
        _text.alignment = TextAlignmentOptions.TopLeft;
        _text.enableWordWrapping = false;
        _text.color = Color.black;

        GameObject imageObj = new GameObject();
        imageObj.transform.SetParent(_textObj, false);
        Image image = imageObj.AddComponent<Image>();
        image.color = new Color(1,1,1,0.2f);
    }

    private bool resetOffsetForText(RectTransform rect)
    {
        if(rect == null) { return true; }
        rect.offsetMax = Vector2.zero;
        rect.anchorMin = Vector2.zero;
        return (rect.anchorMax == Vector2.zero) && (rect.anchorMin == Vector2.zero);
    }

    public void setText(string tex)
    {
        _text.text = tex;
    }

    public void myLateUpdate()
    {
        resetOffsetForText(_tectRect);
        if((_target != null) && _target.hasChanged) { 
            float[] lossyScale = new float[3] { _target.lossyScale.x, _target.lossyScale.y, _target.lossyScale.z };
            float minScale = lossyScale.Min();
            float origScale = 0.01f * 2f;
            //_canvas.transform.localScale = minScale * origScale * new Vector3(3 / _target.lossyScale.x, 1 / _target.lossyScale.y, 1 / _target.lossyScale.z);
            //_canvas.transform.localScale = origScale * new Vector3(3 / _target.lossyScale.x, 1 / _target.lossyScale.y, 1 / _target.lossyScale.z); //new
            //_canvas.transform.localPosition = (Camera.main.transform.right - Camera.main.transform.forward) * minScale;//new Vector3(_target.localScale.x, 0, _target.localScale.z);
            _canvas.transform.position = Camera.main.transform.root.position + (Camera.main.transform.root.right + Camera.main.transform.root.forward * 3) + Vector3.down * 4;
            _canvas.transform.localScale = new Vector3(0.05f, 0.1f, 0.01f);
        }
    }
}

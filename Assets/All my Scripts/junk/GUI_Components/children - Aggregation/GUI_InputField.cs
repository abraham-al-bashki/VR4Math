using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GUI_InputField
{
    private RectTransform _inputFieldTransform;
    private TMP_InputField _inputField;
    private TMP_Text _text;
    public TMP_InputField inputField { get => _inputField;  }
    public RectTransform inputFieldRectTransform { get => _inputFieldTransform;  }
    public TMP_Text text { get => _text;  }
    public GUI_InputField(GUI_Panel panel, float xMin, float yMin, float xMax, float yMax)
    {
        assembleInputField();
        _inputFieldTransform.SetParent(panel.panelRectTransform);
        panel.childrenGUIInputFields.Add(this);
        _inputFieldTransform.anchorMin = new Vector2(xMin, yMin);
        _inputFieldTransform.anchorMax = new Vector2(xMax, yMax);
        _inputFieldTransform.offsetMin = Vector2.zero;
        _inputFieldTransform.offsetMax = Vector2.zero;
    }
    public GUI_InputField(RectTransform canvas, float xMin, float yMin, float xMax, float yMax)
    {
        assembleInputField();
        _inputFieldTransform.SetParent(canvas);
        _inputFieldTransform.anchorMin = new Vector2(xMin, yMin);
        _inputFieldTransform.anchorMax = new Vector2(xMax, yMax);
        _inputFieldTransform.offsetMin = Vector2.zero;
        _inputFieldTransform.offsetMax = Vector2.zero;
    }

    private void assembleInputField()
    {
        Transform tempInstanceParent;
        Transform tempInstanceChild;
        tempInstanceParent = ((GameObject)GameObject.Instantiate(Resources.Load("GUIComponent/InputField/Text Area"))).transform;
        tempInstanceChild = ((GameObject)GameObject.Instantiate(Resources.Load("GUIComponent/InputField/Placeholder"))).transform;
        tempInstanceChild.SetParent(tempInstanceParent);
        tempInstanceChild = ((GameObject)GameObject.Instantiate(Resources.Load("GUIComponent/InputField/Text"))).transform;
        _text = tempInstanceChild.GetComponent<TMP_Text>();
        tempInstanceChild.SetParent(tempInstanceParent);
        tempInstanceChild = tempInstanceParent;
        tempInstanceParent = ((GameObject)GameObject.Instantiate(Resources.Load("GUIComponent/InputField/InputField (TMP)"))).transform;
        tempInstanceChild.SetParent(tempInstanceParent);
        _inputField = tempInstanceParent.GetComponent<TMP_InputField>();
        _inputFieldTransform = tempInstanceParent.GetComponent<RectTransform>();
    }
}

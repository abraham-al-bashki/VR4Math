using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GUI_Dropdown
{
    private RectTransform _dropdownTransform;
    private TMP_Dropdown _dropdown;
    private TMP_Text _text;
    public TMP_Dropdown inputField { get => _dropdown; }
    public RectTransform inputFieldRectTransform { get => _dropdownTransform; }
    public TMP_Text text { get => _text; }
    public GUI_Dropdown(GUI_Panel panel, float xMin, float yMin, float xMax, float yMax)
    {
        assembleDropdown();
        _dropdownTransform.SetParent(panel.panelRectTransform);
        panel.childrenGUIDropdowns.Add(this);
        _dropdownTransform.anchorMin = new Vector2(xMin, yMin);
        _dropdownTransform.anchorMax = new Vector2(xMax, yMax);
        _dropdownTransform.offsetMin = Vector2.zero;
        _dropdownTransform.offsetMax = Vector2.zero;
    }

    private void assembleDropdown()
    {
        Transform tempInstanceParent;
        Transform tempInstanceChild;
        tempInstanceParent = ((GameObject)GameObject.Instantiate(Resources.Load("GUIComponent/Dropdown/Dropdown"))).transform;
        tempInstanceChild = ((GameObject)GameObject.Instantiate(Resources.Load("GUIComponent/Dropdown/Label"))).transform;
        _text = tempInstanceChild.GetComponent<TMP_Text>();
        tempInstanceChild.SetParent(tempInstanceParent);
        tempInstanceChild = ((GameObject)GameObject.Instantiate(Resources.Load("GUIComponent/Dropdown/Arrow"))).transform;
        tempInstanceChild.SetParent(tempInstanceParent);
        tempInstanceChild = ((GameObject)GameObject.Instantiate(Resources.Load("GUIComponent/Dropdown/Template"))).transform;
        tempInstanceChild.SetParent(tempInstanceParent);
        _dropdown = tempInstanceParent.GetComponent<TMP_Dropdown>();
        _dropdownTransform = tempInstanceParent.GetComponent<RectTransform>();
    }
}

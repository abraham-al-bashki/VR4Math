using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GUI_Toggle
{
    private RectTransform _toggleTransform;
    private UnityEngine.UI.Toggle _toggle;
    private TMP_Text _text;
    public UnityEngine.UI.Toggle toggle { get => _toggle; }
    public RectTransform toggleRectTransform { get => _toggleTransform; }
    public TMP_Text text { get => _text; }

    public GUI_Toggle(GUI_Panel panel, float xMin, float yMin, float xMax, float yMax)
    {
        assembleDropdown();
        _toggleTransform.SetParent(panel.panelRectTransform);
        panel.childrenGUIToggles.Add(this);
        _toggleTransform.anchorMin = new Vector2(xMin, yMin);
        _toggleTransform.anchorMax = new Vector2(xMax, yMax);
        _toggleTransform.offsetMin = Vector2.zero;
        _toggleTransform.offsetMax = Vector2.zero;
    }
    private void assembleDropdown()
    {
        Transform tempInstanceParent;
        Transform tempInstanceChild;
        tempInstanceParent = ((GameObject)GameObject.Instantiate(Resources.Load("GUIComponent/Dropdown/Toggle"))).transform;
        tempInstanceChild = ((GameObject)GameObject.Instantiate(Resources.Load("GUIComponent/Dropdown/Background"))).transform;
        tempInstanceChild.SetParent(tempInstanceParent);
        tempInstanceChild = ((GameObject)GameObject.Instantiate(Resources.Load("GUIComponent/Dropdown/Label"))).transform;
        _text = tempInstanceChild.GetComponent<TMP_Text>();
        tempInstanceChild.SetParent(tempInstanceParent);
        _toggle = tempInstanceParent.GetComponent<UnityEngine.UI.Toggle>();
        _toggleTransform = tempInstanceParent.GetComponent<RectTransform>();
    }
}

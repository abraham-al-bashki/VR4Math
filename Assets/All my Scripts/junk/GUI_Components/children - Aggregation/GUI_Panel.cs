using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class GUI_Panel
{
    private RectTransform _panelRectTransform;
    private List<GUI_Panel> _childrenGUIPanels;
    private List<GUI_InputField> _childrenGUIInputFields;
    private List<GUI_Dropdown> _childrenGUIDropdowns;
    private List<GUI_Text> _childrenGUITexts;
    private List<GUI_Button> _childrenGUIButtons;
    private List<GUI_Toggle> _childrenGUIToggles;
    private List<GUI_Slider> _childrenGUISliders;
    //more to be continued

    public RectTransform panelRectTransform
    {
        get => _panelRectTransform;
    }
    public List<GUI_Panel> childrenGUIPanels
    {
        get => _childrenGUIPanels;
    }
    public List<GUI_InputField> childrenGUIInputFields
    {
        get => _childrenGUIInputFields;
    }
    public List<GUI_Dropdown> childrenGUIDropdowns
    {
        get => _childrenGUIDropdowns;
    }
    public List<GUI_Text> childrenGUITexts
    {
        get => _childrenGUITexts;
    }
    public List<GUI_Button> childrenGUIButtons
    {
        get => _childrenGUIButtons;
    }
    public List<GUI_Toggle> childrenGUIToggles
    {
        get => _childrenGUIToggles;
    }
    public List<GUI_Slider> childrenGUISliders
    {
        get => _childrenGUISliders;
    }

    public GUI_Panel(RectTransform canvas, float xMin, float yMin, float xMax, float yMax)
    {
        init();
        assemblePanel();
        _panelRectTransform.SetParent(canvas);
        _panelRectTransform.anchorMin = new Vector2(xMin, yMin);
        _panelRectTransform.anchorMax = new Vector2(xMax, yMax);
        _panelRectTransform.offsetMin = Vector2.zero;
        _panelRectTransform.offsetMax = Vector2.zero;
    }
    public GUI_Panel(GUI_Panel panel, float xMin, float yMin, float xMax, float yMax)
    {
        init();
        assemblePanel();
        _panelRectTransform.SetParent(panel.panelRectTransform);
        panel.childrenGUIPanels.Add(this);
        _panelRectTransform.anchorMin = new Vector2(xMin, yMin);
        _panelRectTransform.anchorMax = new Vector2(xMax, yMax);
        _panelRectTransform.offsetMin = Vector2.zero;
        _panelRectTransform.offsetMax = Vector2.zero;
    }

    public GUI_Panel(GUI_Panel panel, Vector2 minAnchors, Vector2 maxAnchors)
    {
        init();
        assemblePanel();
        _panelRectTransform.SetParent(panel.panelRectTransform);
        panel.childrenGUIPanels.Add(this);
        _panelRectTransform.anchorMin = minAnchors;
        _panelRectTransform.anchorMax = maxAnchors;
        _panelRectTransform.offsetMin = Vector2.zero;
        _panelRectTransform.offsetMax = Vector2.zero;
    }

    private void assemblePanel()
    {
        _panelRectTransform = ((GameObject)GameObject.Instantiate(Resources.Load("GUIComponent/Panel/Panel"))).GetComponent<RectTransform>();
    }

    private void init()
    {
    _childrenGUIPanels = new List<GUI_Panel>();
    _childrenGUIInputFields = new List<GUI_InputField>();
    _childrenGUIDropdowns = new List<GUI_Dropdown>();
    _childrenGUITexts = new List<GUI_Text>();
    _childrenGUIButtons = new List<GUI_Button>();
    _childrenGUIToggles = new List<GUI_Toggle>();
    _childrenGUISliders = new List<GUI_Slider>();
}
}

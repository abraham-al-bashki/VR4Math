using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GUI_Slider
{
    private RectTransform _sliderTransform;
    private UnityEngine.UI.Slider _slider;
    public UnityEngine.UI.Slider slider { get => _slider; }
    public RectTransform sliderTransform { get => _sliderTransform; }

    public GUI_Slider(GUI_Panel panel, float xMin, float yMin, float xMax, float yMax)
    {
        assembleSlider();
        _sliderTransform.SetParent(panel.panelRectTransform);
        panel.childrenGUISliders.Add(this);
        _sliderTransform.anchorMin = new Vector2(xMin, yMin);
        _sliderTransform.anchorMax = new Vector2(xMax, yMax);
        _sliderTransform.offsetMin = Vector2.zero;
        _sliderTransform.offsetMax = Vector2.zero;
    }

    private void assembleSlider()
    {
        _sliderTransform = ((GameObject)GameObject.Instantiate(Resources.Load("GUIComponent/Slider/Slider"))).GetComponent<RectTransform>();
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GUI_Button
{
    private RectTransform _buttonRectTransform;
    private UnityEngine.UI.Button _button;
    private TMP_Text _text;
    public UnityEngine.UI.Button button { get => _button; }
    public RectTransform buttonRectTransform { get => _buttonRectTransform; }
    public TMP_Text text { get => _text; }

    public GUI_Button(GUI_Panel panel, float xMin, float yMin, float xMax, float yMax)
    {
        assembleButton();
        _buttonRectTransform.SetParent(panel.panelRectTransform);
        panel.childrenGUIButtons.Add(this);
        while (!setOffsetToZero(_text.rectTransform));
        defaultFontSettings();
        _buttonRectTransform.anchorMin = new Vector2(xMin, yMin);
        _buttonRectTransform.anchorMax = new Vector2(xMax, yMax);
        while (!setOffsetToZero(_buttonRectTransform));
    }

    public GUI_Button(GUI_Panel panel, float xMin, float xMax, AnchorPresets align)
    {
        assembleButton();
        _buttonRectTransform.SetParent(panel.panelRectTransform);
        panel.childrenGUIButtons.Add(this);
        while (!setOffsetToZero(_text.rectTransform));
        defaultFontSettings();
        setAnchorPresets(align);
        _buttonRectTransform.anchorMin = new Vector2(xMin, _buttonRectTransform.anchorMin.y);
        _buttonRectTransform.anchorMax = new Vector2(xMax, _buttonRectTransform.anchorMax.y);
        _buttonRectTransform.pivot = new Vector2(0.5f, _buttonRectTransform.pivot.y);
        while (!setOffsetToZero(_buttonRectTransform));
        _buttonRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 10);
    }
    private void assembleButton()
    {
        Transform tempInstanceParent;
        Transform tempInstanceChild;
        tempInstanceParent = ((GameObject)GameObject.Instantiate(Resources.Load("GUIComponent/Button/Button"))).transform;
        tempInstanceChild = ((GameObject)GameObject.Instantiate(Resources.Load("GUIComponent/Button/Text (TMP)"))).transform;
        _text = tempInstanceChild.GetComponent<TMP_Text>();
        tempInstanceChild.SetParent(tempInstanceParent);
        _button = tempInstanceParent.GetComponent<UnityEngine.UI.Button>();
        _buttonRectTransform = tempInstanceParent.GetComponent<RectTransform>();
    }

    private void defaultFontSettings()
    {
        _text.fontSize = 12;
        _text.color = Color.black;
    }

    private bool setOffsetToZero(RectTransform tranf)
    {
        tranf.offsetMin = Vector2.zero;
        tranf.offsetMax = Vector2.zero;
        if ((tranf.offsetMin == Vector2.zero) && (tranf.offsetMax == Vector2.zero))
        {
            return true;
        }
        return false;
    }

    private void setAnchorPresets(AnchorPresets align)
    {
        Vector2 anchorPresets;
        Vector2 pivot;
        switch (align)
        {
            case (AnchorPresets.TopLeft):
                {
                    anchorPresets = new Vector2(0, 1);
                    pivot = new Vector2(0, 1);
                    break;
                }
            case (AnchorPresets.TopCenter):
                {
                    anchorPresets = new Vector2(0.5f, 1);
                    pivot = new Vector2(0.5f, 1);
                    break;
                }
            case (AnchorPresets.TopRight):
                {
                    anchorPresets = new Vector2(1, 1);
                    pivot = new Vector2(1, 1);
                    break;
                }

            case (AnchorPresets.MiddleLeft):
                {
                    anchorPresets = new Vector2(0, 0.5f);
                    pivot = new Vector2(0, 0.5f);
                    break;
                }
            case (AnchorPresets.MiddleCenter):
                {
                    anchorPresets = new Vector2(0.5f, 0.5f);
                    pivot = new Vector2(0.5f, 0.5f);
                    break;
                }
            case (AnchorPresets.MiddleRight):
                {
                    anchorPresets = new Vector2(1, 0.5f);
                    pivot = new Vector2(1, 0.5f);
                    break;
                }

            case (AnchorPresets.BottomLeft):
                {
                    anchorPresets = new Vector2(0, 0);
                    pivot = new Vector2(0, 0);
                    break;
                }
            case (AnchorPresets.BottonCenter):
                {
                    anchorPresets = new Vector2(0.5f, 0);
                    pivot = new Vector2(0.5f, 0);
                    break;
                }
            case (AnchorPresets.BottomRight):
                {
                    anchorPresets = new Vector2(1, 0);
                    pivot = new Vector2(1, 0);
                    break;
                }
            default:
                {
                    return;
                }
        }
        buttonRectTransform.anchorMin = anchorPresets;
        buttonRectTransform.anchorMax = anchorPresets;
        _buttonRectTransform.pivot = pivot;
    }
    public enum AnchorPresets
    {
        TopLeft,
        TopCenter,
        TopRight,

        MiddleLeft,
        MiddleCenter,
        MiddleRight,

        BottomLeft,
        BottonCenter,
        BottomRight,
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GUI_Text
{
    private RectTransform _textTransform;
    private TMP_Text _text;
    public RectTransform textRectTransform { get => _textTransform;  }
    public TMP_Text text { get => _text;  }
    public GUI_Text(GUI_Panel panel, float xMin, float yMin, float xMax, float yMax)
    {
        assembleText();
        _textTransform.SetParent(panel.panelRectTransform);
        panel.childrenGUITexts.Add(this);
        _textTransform.anchorMin = new Vector2(xMin, yMin);
        _textTransform.anchorMax = new Vector2(xMax, yMax);
        while(!setOffsetToZero());
    }

    public GUI_Text(GUI_Panel panel, Vector2 minAnchors, Vector2 maxAnchors)
    {
        assembleText();
        _textTransform.SetParent(panel.panelRectTransform);
        panel.childrenGUITexts.Add(this);
        _textTransform.anchorMin = minAnchors;
        _textTransform.anchorMax = maxAnchors;
        while (!setOffsetToZero());
    }

    public void assembleText()
    {
        _textTransform = ((GameObject)GameObject.Instantiate(Resources.Load("GUIComponent/Text/Text"))).GetComponent<RectTransform>();
        _text = _textTransform.GetComponent<TMP_Text>();
        defaultFontSettings();
    }

    private bool setOffsetToZero()
    {
        _textTransform.offsetMax = Vector2.zero;
        _textTransform.offsetMin = Vector2.zero;
        if((_textTransform.offsetMax == Vector2.zero) && (_textTransform.offsetMin == Vector2.zero))
        {
            return true;
        }
        return false;
    }

    private void defaultFontSettings()
    {
        _text.fontSize = 12;
        _text.horizontalAlignment = HorizontalAlignmentOptions.Center;
        _text.verticalAlignment = VerticalAlignmentOptions.Middle;
        _text.enableWordWrapping = false;
        _text.color = Color.black;
    }
}

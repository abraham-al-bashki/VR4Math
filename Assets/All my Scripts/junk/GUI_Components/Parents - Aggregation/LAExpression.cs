using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LAExpression
{
    private RectTransform _LAExpRectTransform;
    private GUI_Panel _panel;
    private GUI_Vector _translationVector;
    private GUI_Matrix _scaleMatrix;
    private GUI_Matrix _zRotationMatrix;
    private GUI_Matrix _yRotationMatrix;
    private GUI_Matrix _xRotationMatrix;

    public LAExpression(GUI_Panel parentPanel, Vector2 minAnchors, Vector2 maxAnchors)
    {
        _panel = new GUI_Panel(parentPanel, minAnchors, maxAnchors);
        _LAExpRectTransform = _panel.panelRectTransform;
        _scaleMatrix = new GUI_Matrix(_panel, new Vector2(0,0), new Vector2(0.3f,1));
    }

}

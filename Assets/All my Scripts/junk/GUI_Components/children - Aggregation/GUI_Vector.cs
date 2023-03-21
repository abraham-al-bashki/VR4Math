using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GUI_Vector 
{
    private GUI_Panel _vector;
    private GUI_Text[] _vectorElements;
    public RectTransform matrixRectTransform
    {
        get => _vector.panelRectTransform;
    }
    public GUI_Text[] matrixElements
    {
        get => _vectorElements;
    }

    public GUI_Vector(GUI_Panel parentPanel, float xMin, float yMin, float xMax, float yMax)
    {
        _vectorElements = new GUI_Text[9];
        _vector = new GUI_Panel(parentPanel, xMin, yMin, xMax, yMax);
        createAndAssebleMatrix();
        _vector.panelRectTransform.SetParent(parentPanel.panelRectTransform);
    }

    public GUI_Vector(GUI_Panel parentPanel, Vector2 anchorMin, Vector2 anchorMax)
    {
        _vectorElements = new GUI_Text[9];
        _vector = new GUI_Panel(parentPanel, anchorMin, anchorMax);
        createAndAssebleMatrix();
        _vector.panelRectTransform.SetParent(parentPanel.panelRectTransform);
    }

    private void createAndAssebleMatrix()
    {
        Vector2 anchorMin = Vector2.zero;
        Vector2 anchorMax = new Vector2(0.05f, 1);
        GUI_Panel tempPanel = new GUI_Panel(_vector, anchorMin, anchorMax);
        GUI_Text tempText = new GUI_Text(tempPanel, Vector2.zero, Vector2.one);
        tempText.text.text = "[";
        fixFontOfTheBracketsInMatrix(tempText.text);

        anchorMin = new Vector2(0.95f, 0);
        anchorMax = Vector2.one;
        tempPanel = new GUI_Panel(_vector, anchorMin, anchorMax);
        tempText = new GUI_Text(tempPanel, Vector2.zero, Vector2.one);
        tempText.text.text = "]";
        fixFontOfTheBracketsInMatrix(tempText.text);

        Vector2 helpVectorForY = Vector2.up * 0.3f;
        Vector2 anchorMinInitial = new Vector2(0.05f, 0.65f);
        Vector2 anchorMaxInitial = new Vector2(0.95f, 0.95f);
        for (int column = 0; column < 3; column++)
        {
            anchorMin = anchorMinInitial - helpVectorForY * column;
            anchorMax = anchorMaxInitial - helpVectorForY * column;
            tempPanel = new GUI_Panel(_vector, anchorMin, anchorMax);
            _vectorElements[column] = new GUI_Text(tempPanel, Vector2.zero, Vector2.one);
            fixFontOfTheElementsInMatrix(_vectorElements[column].text);
        }
        putDefaultElementsInMatrix();
    }

    private void fixFontOfTheElementsInMatrix(TMP_Text textObj)
    {
        textObj.enableAutoSizing = true;
        textObj.fontSizeMin = 5;
        textObj.fontSizeMax = 150;
        textObj.horizontalAlignment = HorizontalAlignmentOptions.Center;
        textObj.verticalAlignment = VerticalAlignmentOptions.Middle;
        textObj.enableWordWrapping = false;
        textObj.color = Color.black;
    }

    private void fixFontOfTheBracketsInMatrix(TMP_Text textObj)
    {
        textObj.enableAutoSizing = true;
        textObj.fontSizeMin = 5;
        float unhingedFontSize = textObj.fontSize;
        int adjustAutoSize = 8;
        textObj.fontSizeMin = unhingedFontSize * adjustAutoSize;
        textObj.fontSizeMax = unhingedFontSize * (adjustAutoSize + 1);
        textObj.alignment = TextAlignmentOptions.Midline;
        textObj.horizontalAlignment = HorizontalAlignmentOptions.Center;
        textObj.enableWordWrapping = false;
        textObj.color = Color.black;
    }

    private void putDefaultElementsInMatrix()
    {
        _vectorElements[0].text.text = "x";
        _vectorElements[1].text.text = "y";
        _vectorElements[2].text.text = "z";
    }
}

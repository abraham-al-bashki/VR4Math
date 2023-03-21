using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GUI_Matrix
{
    private GUI_Panel _matrix;
    private GUI_Text[] _matrixElements;
    public RectTransform matrixRectTransform
    {
        get => _matrix.panelRectTransform;
    }
    public GUI_Text[] matrixElements
    {
        get => _matrixElements;
    }

    public GUI_Matrix(GUI_Panel parentPanel, float xMin, float yMin, float xMax, float yMax)
    {
        _matrixElements = new GUI_Text[9];
        _matrix = new GUI_Panel(parentPanel, xMin, yMin, xMax, yMax);
        createAndAssebleMatrix();
        _matrix.panelRectTransform.SetParent(parentPanel.panelRectTransform);
    }

    public GUI_Matrix(GUI_Panel parentPanel, Vector2 anchorMin, Vector2 anchorMax)
    {
        _matrixElements = new GUI_Text[9];
        _matrix = new GUI_Panel(parentPanel, anchorMin, anchorMax);
        createAndAssebleMatrix();
        _matrix.panelRectTransform.SetParent(parentPanel.panelRectTransform);
    }

    private void createAndAssebleMatrix()
    {
        Vector2 anchorMin = Vector2.zero;
        Vector2 anchorMax = new Vector2(0.05f, 1);
        /*GUI_Panel tempPanel = new GUI_Panel(_matrix, anchorMin, anchorMax);
        GUI_Text tempText = new GUI_Text(tempPanel, Vector2.zero, Vector2.one);
        tempText.text.text = "[";
        fixFontOfTheBracketsInMatrix(tempText.text);*/
        GUI_Panel tempPanel = new GUI_Panel(_matrix, anchorMin, anchorMax);
        tempPanel.panelRectTransform.GetComponent<UnityEngine.UI.Image>().color = Color.black;

        anchorMin = new Vector2(0.95f, 0);
        anchorMax = Vector2.one;
        /*tempPanel = new GUI_Panel(_matrix, anchorMin, anchorMax);
        tempText = new GUI_Text(tempPanel, Vector2.zero, Vector2.one);
        tempText.text.text = "]";
        fixFontOfTheBracketsInMatrix(tempText.text);*/
        tempPanel = new GUI_Panel(_matrix, anchorMin, anchorMax);
        tempPanel.panelRectTransform.GetComponent<UnityEngine.UI.Image>().color = Color.black;

        Vector2 helpVectorForX = Vector2.right * 0.3f;
        Vector2 helpVectorForY = Vector2.up * 0.3f;
        Vector2 anchorMinInitial = new Vector2(0.05f, 0.65f);
        Vector2 anchorMaxInitial = new Vector2(0.35f, 0.95f);
        for (int column = 0; column < 3; column++)
        {
            for (int row = 0; row < 3; row++)
            {
                anchorMin = anchorMinInitial + helpVectorForX * row - helpVectorForY * column;
                anchorMax = anchorMaxInitial + helpVectorForX * row - helpVectorForY * column;
                tempPanel = new GUI_Panel(_matrix, anchorMin, anchorMax);
                _matrixElements[column * 3 + row] = new GUI_Text(tempPanel, Vector2.zero, Vector2.one);
                fixFontOfTheElementsInMatrix(_matrixElements[column * 3 + row].text);
            }
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

    /*private void fixFontOfTheBracketsInMatrix(TMP_Text textObj)
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
    }*/

    private void putDefaultElementsInMatrix()
    {
        _matrixElements[0].text.text = "a";
        _matrixElements[1].text.text = "b";
        _matrixElements[2].text.text = "c";
        _matrixElements[3].text.text = "d";
        _matrixElements[4].text.text = "e";
        _matrixElements[5].text.text = "f";
        _matrixElements[6].text.text = "g";
        _matrixElements[7].text.text = "h";
        _matrixElements[8].text.text = "i";
    }
}

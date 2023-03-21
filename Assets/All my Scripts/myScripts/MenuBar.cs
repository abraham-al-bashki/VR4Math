using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;

public class MenuBar
{
    private Controller _controller;

    bool menuBarVisible = true;
    private Vector2 scroll = Vector2.zero;

    string camPosition = "0,0,0";
    string newPosition;
    string newScaling;
    string newRotation;

    string[] colors = { "WT", "BL", "BK", "GN", "RD", "GY"};
    int selectedColorIndex = 0;

    string txtFieldPos;
    string txtFieldRot = "0,0,0";
    string txtFieldScale = "1";
    string positionInputCanvas = "0,0,0";

    private string titleValue = "";
    private string textValue = "";
    Vector3 CanvasPosition;
    string newTitle;
    string newText;

    string[] menuItems = { "Hide", "Edit", "GameObjects", "AddedObjects", "TextCanvas", "UpdateText", "Matrix","Help" };
    List<bool> menuItemPanels = new List<bool>();

    List<(string positionInput, string scalingInput, string rotationInput, GameObject prefab)> objectData;

    List<string> canvasNames = new List<string>();
    private int canvasIndex;
    int selectedCanvasIndex = -1;

    List<string> GONames = new List<string>();
    int selectedGOIndex = -1;

    public MenuBar(Controller controller)
    {
        _controller = controller;
    }
    public void myStart()
    {
        for (int i = 0; i < menuItems.Length; i++)
        {
            menuItemPanels.Add(false);
        }
        //(string, string, GameObject) p = ("0,0,0", "0,0,0", Resources.Load<GameObject>("OBJ"));
        objectData = new List<(string, string, string, GameObject)>
            {
                ("0,0,0", "1,1,1", "0,0,0", Resources.Load<GameObject>("Cube")),
                ("0,0,0", "1,1,1", "0,0,0", Resources.Load<GameObject>("Sphere")),
                ("0,0,0", "1,1,1", "0,0,0", Resources.Load<GameObject>("Cylinder")),
                ("0,0,0", "1,1,1", "0,0,0", Resources.Load<GameObject>("Capsule")),
                ("0,0,0", "1,1,1", "0,0,0", Resources.Load<GameObject>("Plane")),
                ("0,0,0", txtFieldScale, "0,0,0", Resources.Load<GameObject>("Tree")),
                ("0,0,0", "1,1,1", "", Resources.Load<GameObject>("Vector")),
                ("0,0,0", "1,1,1", "0,0,0", Resources.Load<GameObject>("Cuboid"))
            };
    
    }

    public void myOnGUI()
    {
        GUI.color = Color.white;
        GUI.skin.box.normal.background = TextureGUI(1, 1, new Color(0.0f, 0.0f, 0.0f, 0.75f));

        GUI.skin.button.fontSize = 20;

        if (menuBarVisible)
        {
            // Create the menu bar
            GUILayout.BeginArea(new Rect(0, 0, Screen.width, 100));
            GUILayout.BeginHorizontal();

            for (int i = 0; i < menuItems.Length; i++)
            {
                if (GUILayout.Button(menuItems[i], GUILayout.Width(Screen.width / menuItems.Length)))
                {
                    MenuItemClicked(menuItems[i]);
                }
            }

            GUILayout.EndHorizontal();
            GUILayout.EndArea();
        }
        else
        {
            if (GUI.Button(new Rect(10, 10, 100, 30), "MenuBar"))
            {
                menuBarVisible = true;
            }
        }

        for (int i = menuItemPanels.Count - 1; i >= 0; i--)
        {
            string menuItemPanelKey = menuItems[i];
            bool menuItemPanelValue = menuItemPanels[i];

            GUI.skin.label.fontSize = 20;
            GUI.skin.label.fontStyle = FontStyle.Normal;

            // If the panel is visible, draw it
            if (menuItemPanelValue)
            {
                // Draw panel
                GUI.Box(new Rect(0, Screen.height / 20, Screen.width, Screen.height / 1.5f), "");

                if (GUI.Button(new Rect(Screen.width - 50, Screen.height / 20, 50, 30), "X"))
                {
                    menuItemPanels[i] = false;
                }

                if (menuItemPanelKey == "Edit")
                {
                    GUILayout.BeginArea(new Rect(5, 20 * 5, Screen.width - 10, 90 * 5));
                    scroll = GUILayout.BeginScrollView(scroll, GUILayout.Width(Screen.width - 10), GUILayout.Height(90 * 5));
                    GUILayout.BeginVertical();

                    if (GUI.Button(new Rect(10, 200, 150, 40), "Undo"))
                    {
                        _controller.undoLA_Object_Command();
                    }
                    if (GUI.Button(new Rect(10, 250, 150, 40), "Save"))
                    {
                        _controller.newSave("Save 1");
                    }

                    if (camPosition == null) { camPosition = "0,0,0"; }

                    GUILayout.Label("Camere Position:", GUILayout.Width(150));
                    camPosition = GUILayout.TextField(camPosition, GUILayout.Width(100));

                    if (GUILayout.Button("Update", GUILayout.Width(100), GUILayout.Height(30)))
                    {
                        Vector3 CamPosition = ParsePosition(camPosition);

                        if (CamPosition != Vector3.zero)
                        {
                            Camera.main.transform.position = CamPosition;
                        }

                        camPosition = "0,0,0";
                    }

                    scroll.y += Input.mouseScrollDelta.y * 10;
                    GUILayout.EndVertical();
                    GUILayout.EndScrollView();
                    GUILayout.EndArea();
                }

                // If the GameObjects panel is visible, add the options to create an object with specific position, scale, rotation and color
                else if (menuItemPanelKey == "GameObjects")
                {
                    GUILayout.BeginArea(new Rect(5, 20 * 5, Screen.width - 10, 90 * 5));
                    scroll = GUILayout.BeginScrollView(scroll, GUILayout.Width(Screen.width - 10), GUILayout.Height(90 * 5));
                    GUILayout.BeginVertical();
                    GUI.skin.label.fontStyle = FontStyle.Normal;

                    for (int j = 0; j < objectData.Count; j++)
                    {
                        string positionInput = objectData[j].positionInput;
                        string scalingInput = objectData[j].scalingInput;
                        string rotationInput = objectData[j].rotationInput;

                        GameObject prefab = objectData[j].prefab;

                        GUI.Label(new Rect(10, 0, 250, 30), "Center Point: (x,y,z)");
                        GUI.Label(new Rect(230, 0, 250, 30), "Dimensions: (x,y,z)");
                        GUI.Label(new Rect(450, 0, 250, 30), "Euler Angles°: (x,y,z)");
                        GUI.Label(new Rect(670, 0, 250, 30), "Choose a Color:");
                        GUI.Label(new Rect(990, 0, 250, 30), "Add Objects:");

                        if (newPosition == null) { newPosition = "0,0,0"; }
                        else if (newScaling == null) { newScaling = "1,1,1"; }
                        else if (newRotation == null) { newRotation = "0,0,0"; }

                        newPosition = GUI.TextField(new Rect(10, 30 + 40 * j, 200, 30), positionInput);
                        newScaling = GUI.TextField(new Rect(230, 30 + 40 * j, 200, 30), scalingInput);
                        newRotation = GUI.TextField(new Rect(450, 30 + 40 * j, 200, 30), rotationInput);

                        //// new
                        ///

                        selectedColorIndex = GUI.SelectionGrid(new Rect(670, 30 + 40 * j, 300, 30), selectedColorIndex, colors, 6);

                        if (true)
                        {
                            objectData[j] = (newPosition, newScaling, newRotation, prefab);
                        }

                        // Add textfields for the new prefab
                        if (objectData[j].Item4 != null && objectData[j].Item4.gameObject.name == "Vector")
                        {
                            newPosition = GUI.TextField(new Rect(10, 30 + 40 * j, 200, 30), positionInput);
                            newScaling = GUI.TextField(new Rect(230, 30 + 40 * j, 200, 30), scalingInput);

                            if (prefab != null && GUI.Button(new Rect(990, 30 + 40 * j, 150, 30), "Add " + prefab.name))
                            {
                                prefab.tag = "tagged";
                                string newGOName = prefab.name + GONames.Count;
                                GONames.Add(newGOName);

                                AddObject(prefab, ParsePosition(newPosition), ParsePosition(newScaling), Vector3.zero, colors[selectedColorIndex]); // Set rotation to zero for the new prefab

                                //Reset
                                newPosition = "0,0,0";
                                newScaling = "1,1,1";
                                selectedColorIndex = 0;
                            }
                            
                        }

                        if (objectData[j].Item4 != null && objectData[j].Item4.gameObject.name == "Tree")
                        {
                            newPosition = GUI.TextField(new Rect(10, 30 + 40 * j, 200, 30), positionInput);
                            txtFieldScale = GUI.TextField(new Rect(230, 30 + 40 * j, 200, 30), scalingInput);
                            newRotation = GUI.TextField(new Rect(450, 30 + 40 * j, 200, 30), rotationInput);

                            if (prefab != null && GUI.Button(new Rect(990, 30 + 40 * j, 150, 30), "Add " + prefab.name))
                            {
                                prefab.tag = "tagged";
                                string newGOName = prefab.name + GONames.Count;
                                GONames.Add(newGOName);

                                AddObject(prefab, ParsePosition(newPosition), ParsePosition(txtFieldScale), ParsePosition(newRotation), colors[selectedColorIndex]); // Set rotation to zero for the new prefab

                                //Reset
                                newPosition = "0,0,0";
                                newScaling = "1,1,1";
                                txtFieldScale = "1";
                                selectedColorIndex = 0;
                            }

                        }

                        else
                        {
                            newPosition = GUI.TextField(new Rect(10, 30 + 40 * j, 200, 30), positionInput);
                            newScaling = GUI.TextField(new Rect(230, 30 + 40 * j, 200, 30), scalingInput);
                            newRotation = GUI.TextField(new Rect(450, 30 + 40 * j, 200, 30), rotationInput);

                            if (prefab != null && GUI.Button(new Rect(990, 30 + 40 * j, 150, 30), "Add " + prefab.name))
                            {
                                prefab.tag = "tagged";
                                string newGOName = prefab.name + GONames.Count;
                                GONames.Add(newGOName);

                                AddObject(prefab, ParsePosition(newPosition), ParsePosition(newScaling), ParsePosition(newRotation), colors[selectedColorIndex]);

                                //Reset
                                newPosition = "0,0,0";
                                newScaling = "1,1,1";
                                newRotation = "0,0,0";
                                selectedColorIndex = 0;
                            }
                        }
                        /*
                        selectedColorIndex = GUI.SelectionGrid(new Rect(670, 30 + 40 * j, 300, 30), selectedColorIndex, colors, 6);

                        if (true)
                        {
                            objectData[j] = (newPosition, newScaling, newRotation, prefab);
                        }

                        if (prefab != null && GUI.Button(new Rect(990, 30 + 40 * j, 150, 30), "Add " + prefab.name))
                        {
                            if (objectData[j].Item4.gameObject.name == "NewPrefab")
                            {
                                prefab.tag = "tagged";
                                string newGOName = prefab.name + GONames.Count;
                                GONames.Add(newGOName);

                                AddObject(prefab, ParsePosition(newPosition), ParsePosition(newScaling), Vector3.zero, colors[selectedColorIndex]); // Set rotation to zero for the new prefab

                                newPosition = "0,0,0";
                                newScaling = "0,0,0";
                                newRotation = "0,0,0";
                                selectedColorIndex = 0;
                            }
                            else
                            {
                                prefab.tag = "tagged";
                                string newGOName = prefab.name + GONames.Count;
                                GONames.Add(newGOName);

                                AddObject(prefab, ParsePosition(newPosition), ParsePosition(newScaling), ParsePosition(newRotation), colors[selectedColorIndex]);

                                newPosition = "0,0,0";
                                newScaling = "0,0,0";
                                newRotation = "0,0,0";
                                selectedColorIndex = 0;
                            }
                            //prefab.tag = "tagged";
                            //string newGOName = prefab.name + GONames.Count;
                            //GONames.Add(newGOName);

                            //AddObject(prefab, ParsePosition(newPosition), ParsePosition(newScaling), ParsePosition(newRotation), colors[selectedColorIndex]);

                            ////Reset
                            //newPosition = "0,0,0";
                            //newScaling = "0,0,0";
                            //newRotation = "0,0,0";
                            //selectedColorIndex = 0;
                        }*/
                    }
                    scroll.y += Input.mouseScrollDelta.y * 10;
                    GUILayout.EndVertical();
                    GUILayout.EndScrollView();
                    GUILayout.EndArea();
                }

                // If the AddedObjects panel is visible, show the objects with the options to edit
                else if (menuItemPanelKey == "AddedObjects")
                {
                    GUILayout.BeginArea(new Rect(5, 20 * 5, Screen.width - 10, 90 * 5));
                    scroll = GUILayout.BeginScrollView(scroll, GUILayout.Width(Screen.width - 10), GUILayout.Height(90 * 5));
                    GUILayout.BeginVertical();

                    // Add title to the panel
                    GUIStyle titleStyle = new GUIStyle(GUI.skin.label);
                    titleStyle.fontStyle = FontStyle.Bold;
                    GUILayout.Label("If you set the scaling to zero, you won't see the object!", titleStyle);

                    GUI.skin.label.fontSize = 15;
                    GUI.skin.label.fontStyle = FontStyle.Normal;

                    // Find all the objects in the scene
                    //List<GameObject> allObjects = (GameObject.FindGameObjectsWithTag("tagged")).ToList();
                    List<LA_Object> allObjects = _controller.LA_Objects;

                    //for (int j = 0; j < allObjects.Count; j++)
                    for (int j = 0; j < _controller.LA_Objects.Count(); j++)
                    {
                        //if (allObjects[j].transform.parent != null)
                        if (allObjects[j].getGameObject().transform.parent != null)
                        {
                            // Show the object name and options in separate columns
                            GUILayout.BeginHorizontal();

                            // Show object name
                            //GUILayout.Label(GONames[j] , GUILayout.Width(200));

                            GUILayout.Space(5);
                            GUILayout.Label("Position:", GUILayout.Width(60));
                            Vector3 v = allObjects[j].getGameObject().transform.position;
                            //txtFieldPos = v.x + "," + v.y + "," + v.z;
                            txtFieldPos = GUILayout.TextField(txtFieldPos, GUILayout.Width(100));
                            //txtFieldPos = GUILayout.TextField(allObjects[j].getGameObject().transform.position.ToString(), GUILayout.Width(100));
                            GUILayout.Space(5);
                            GUILayout.Label("Rotation:", GUILayout.Width(60));
                            txtFieldRot = GUILayout.TextField(txtFieldRot, GUILayout.Width(100));
                            //txtFieldRot = GUILayout.TextField(allObjects[j].getGameObject().transform.eulerAngles.ToString(), GUILayout.Width(100));
                            GUILayout.Space(5);
                            GUILayout.Label("Scale:", GUILayout.Width(60));
                            txtFieldScale = allObjects[j].getScaling()[0].ToString();
                            txtFieldScale = GUILayout.TextField(txtFieldScale, GUILayout.Width(100));
                            GUILayout.Space(5);

                            if (GUILayout.Button("Update", GUILayout.Width(100), GUILayout.Height(30)))
                            {
                                Vector3 objPosition = ParsePosition(txtFieldPos);
                                Vector3 objRotation = ParsePosition(txtFieldRot);

                                float scale = ParseFloat(txtFieldScale);
                                /*
                                //ändra här
                                //if (objPosition != Vector3.zero) { allObjects[j].transform.position = objPosition; }
                                if (objPosition != Vector3.zero) { _controller.addLA_Object_Translation(allObjects[j], objPosition);}
                                //if (objRotation != Vector3.zero) { allObjects[j].transform.eulerAngles = objRotation; }
                                if (objRotation != Vector3.zero) { _controller.addLA_Object_Rotate(allObjects[j], objRotation); }
                                //if (scale != 0) { allObjects[j].transform.localScale = new Vector3(scale, scale, scale); }
                                if (scale != 0) { _controller.addLA_Object_Scale(allObjects[j], scale); }
                                */
                                if((objPosition != Vector3.zero) || (objRotation != Vector3.zero) || (scale != 0))
                                {
                                    //_controller.addLA_Object_Command(allObjects[j], objPosition, objRotation, scale, LA_Object_Transformation.TransformationApply.All);
                                    
                                }
                                _controller.addLA_Object_Transform(allObjects[j], objPosition, objRotation, scale);
                                //Reset text fields

                                //txtFieldPos = "0,0,0";
                                //txtFieldRot = "0,0,0";
                                //txtFieldScale = "1";
                            }

                            // Show delete button next to obj name
                            else if (GUILayout.Button("Delete", GUILayout.Width(100), GUILayout.Height(30)))
                            {
                                // Delete the object from the scene
                                //ändra här
                                //GONames.RemoveAt(j);
                                //GameObject.Destroy(allObjects[j]);
                                //_controller.destroy(allObjects[j].GetComponent<LA_Object_Component>().laobject);
                                _controller.destroy(allObjects[j]);
                                //allObjects.RemoveAt(j);
                            }

                            GUILayout.EndHorizontal();
                        }
                    }
                    scroll.y += Input.mouseScrollDelta.y * 10;

                    GUILayout.EndVertical();
                    GUILayout.EndScrollView();
                    GUILayout.EndArea();

                }

                //If the TextCanvas is panel is visiible, add the option to create a text canvas in a specific position
                else if (menuItemPanelKey == "TextCanvas")
                {
                    GUILayout.BeginArea(new Rect(5, 20 * 5, Screen.width - 10, 90 * 5));
                    GUILayout.BeginVertical();
                    scroll = GUILayout.BeginScrollView(scroll, GUILayout.Width(Screen.width - 10), GUILayout.Height(90 * 5));

                    //Title
                    GUILayout.BeginHorizontal();
                    GUILayout.Label("Title:", GUILayout.Width(100));
                    titleValue = GUILayout.TextField(titleValue);
                    GUILayout.EndHorizontal();

                    //Text
                    GUILayout.BeginHorizontal();
                    GUILayout.Label("Text:", GUILayout.Width(100));
                    textValue = GUILayout.TextArea(textValue, GUILayout.Height(80));
                    GUILayout.EndHorizontal();

                    //Position
                    GUILayout.BeginHorizontal();
                    GUILayout.Label("Position:", GUILayout.Width(100));
                    positionInputCanvas = GUILayout.TextField(positionInputCanvas);
                    GUILayout.EndHorizontal();

                    //Button
                    GUILayout.BeginHorizontal();
                    GUILayout.FlexibleSpace();
                    if (GUILayout.Button("Create Canvas", GUILayout.Height(50), GUILayout.Width(200)))
                    {
                        CanvasPosition = ParsePosition(positionInputCanvas);

                        if (true)
                        {
                            GameObject canvasPrefab = Resources.Load<GameObject>("CanvasPrefab");

                            GameObject canvas = GameObject.Instantiate(canvasPrefab);
                            canvas.transform.position = CanvasPosition;
                            canvas.tag = "MyCanvasTag";
                            string newName = canvas.name = "CanvasText" + canvasIndex;
                            canvasNames.Add(newName);
                            canvasIndex++;

                            TMP_Text[] textComponents = canvas.GetComponentsInChildren<TMP_Text>();
                            TMP_Text titleText = canvas.GetComponentInChildren<TMP_Text>(true);
                            textComponents[1].text = textValue;
                            titleText.text = titleValue;

                            //Reset text fields
                            titleValue = "";
                            textValue = "";
                            positionInputCanvas = "0,0,0";
                        }
                    }
                    GUILayout.FlexibleSpace();
                    GUILayout.EndHorizontal();

                    scroll.y += Input.mouseScrollDelta.y * 10;

                    GUILayout.EndVertical();
                    GUILayout.EndScrollView();
                    GUILayout.EndArea();
                }
                else if (menuItemPanelKey == "UpdateText")
                {
                    GUILayout.BeginArea(new Rect(5, 20 * 5, Screen.width - 10, 90 * 5));
                    scroll = GUILayout.BeginScrollView(scroll, GUILayout.Width(Screen.width - 10), GUILayout.Height(90 * 5));
                    GUILayout.BeginHorizontal();

                    // Display text canvases in the scene as a dropdown on the left side of the panel
                    GUILayout.BeginVertical(GUILayout.Width(100));
                    for (int j = 0; j < canvasNames.Count; j++)
                    {
                        if (GUILayout.Button(canvasNames[j], GUILayout.Width(150), GUILayout.Height(40)))
                        {
                            selectedCanvasIndex = j;
                        }
                    }
                    GUILayout.EndVertical();

                    // Display the text canvas update options on the right of the panel
                    GUILayout.BeginVertical(GUILayout.Width(Screen.width - 150));

                    if (selectedCanvasIndex >= 0 && selectedCanvasIndex < canvasNames.Count)
                    {
                        // Find the selected canvas game object
                        string canvasName = canvasNames[selectedCanvasIndex];
                        GameObject canvasGO = GameObject.Find(canvasName);

                        if (canvasGO != null)
                        {
                            TMP_Text[] textComponents = canvasGO.GetComponentsInChildren<TMP_Text>();

                            if (textComponents.Length > 0 && textComponents[0] != null)
                            {
                                GUILayout.Label("New Title: ", GUILayout.Width(100));
                                newTitle = GUILayout.TextField(newTitle, GUILayout.Width(300));
                            }

                            if (textComponents.Length > 1 && textComponents[1] != null)
                            {
                                GUILayout.Label("New Text: :", GUILayout.Width(100));
                                newText = GUILayout.TextField(newText, GUILayout.Height(120), GUILayout.Width(300));
                            }


                            GUILayout.BeginHorizontal();

                            if (GUILayout.Button("Update Text", GUILayout.Height(50), GUILayout.Width(200)))
                            {
                                if (textComponents.Length > 0 && textComponents[0] != null)
                                {
                                    textComponents[0].text = newTitle;
                                }
                                if (textComponents.Length > 1 && textComponents[1] != null)
                                {
                                    textComponents[1].text = newText;
                                }

                                //Reset text fields
                                newTitle = "";
                                newText = "";
                            }
                            if (GUILayout.Button("Delete Canvas", GUILayout.Height(50), GUILayout.Width(200)))
                            {
                                GameObject.DestroyImmediate(canvasGO);
                                canvasNames.RemoveAt(selectedCanvasIndex);

                                selectedCanvasIndex = -1;
                            }
                            GUILayout.EndHorizontal();
                        }
                    }
                    else
                    {
                        GUILayout.Label("To update the text first add a text canvas and then choose from the list, here to the left. ");
                    }
                    scroll.y += Input.mouseScrollDelta.y * 10;


                    GUILayout.EndVertical();
                    GUILayout.EndHorizontal();
                    GUILayout.EndScrollView();
                    GUILayout.EndArea();
                }
                else if (menuItemPanelKey == "Matrix")
                {
                    GUILayout.BeginArea(new Rect(5, 20 * 5, Screen.width - 10, 90 * 5));
                    scroll = GUILayout.BeginScrollView(scroll, GUILayout.Width(Screen.width - 10), GUILayout.Height(90 * 5));
                    GUILayout.BeginHorizontal();

                    //Show Object names on the left side of the panel
                    GUILayout.BeginVertical(GUILayout.Width(100));
                    for (int j = 0; j < GONames.Count; j++)
                    {
                        if (GUILayout.Button(GONames[j], GUILayout.Width(150), GUILayout.Height(40)))
                        {
                            selectedGOIndex = j;
                        }
                    }
                    GUILayout.EndVertical();

                    GUILayout.BeginVertical(GUILayout.Width(Screen.width - 150));
                    if (selectedGOIndex >= 0 && selectedGOIndex < GONames.Count)
                    {
                        //ändra här
                        string objName = GONames[selectedGOIndex];
                        foreach(string o in GONames){ Debug.Log(o); }
                        //GameObject GO = GameObject.Find(objName);
                        GameObject GO = _controller.LA_Objects[0].getGameObject();
                        //Debug.Log("SelectedGOIndex, objName: " + objName);
                        if (GO != null)
                        {
                            //ändra här
                            //Matrix
                            //GUILayout.Label("Matrix Here: : ", GUILayout.Width(100));
                            //Debug.Log("GO not equal null");
                            string matrix = ((LA_Cuboid)GO.transform.parent.GetComponent<LA_Object_Component>().laobject).toString();
                            GUI.Label(new Rect(300, 100, 1000, 500), matrix);
                        }
                    }
                    else
                    {
                        GUILayout.Label("To see objects Matrix first add an object and then choose from the list, here to the left. ");
                    }

                    scroll.y += Input.mouseScrollDelta.y * 10;

                    GUILayout.EndVertical();
                    GUILayout.EndHorizontal();
                    GUILayout.EndScrollView();
                    GUILayout.EndArea();
                }
            }
        }
    }

    // Creates texture of a given color, this for panel background
    private Texture2D TextureGUI(int width, int height, Color color)
    {
        Color[] pixels = new Color[width * height];
        for (int i = 0; i < pixels.Length; i++)
        {
            pixels[i] = color;
        }
        Texture2D tex = new Texture2D(width, height);
        tex.SetPixels(pixels);
        tex.Apply();
        return tex;
    }

    //Add Object to the scene with a specific position, scale, rotation and color
    void AddObject(GameObject prefab, Vector3 position, Vector3 scaling, Vector3 rotation, string color)
    {
        try
        {
            //ändra här
            //var Obj = Instantiate(prefab, position, Quaternion.Euler(rotation));
            //Obj.transform.localScale = scaling;

            //Renderer renderer = Obj.GetComponent<Renderer>();
            //if (renderer != null)
            Color colorInput;
            {
                switch (color)
                {
                    case "WT":
                        colorInput = Color.white;
                        //renderer.material.color = Color.white;
                        break;
                    case "BL":
                        colorInput = Color.blue;
                        //renderer.material.color = Color.blue;
                        break;
                    case "BK":
                        colorInput = Color.black;
                        //renderer.material.color = Color.black;
                        break;
                    case "GN":
                        colorInput = Color.green;
                        //renderer.material.color = Color.green;
                        break;
                    case "RD":
                        colorInput = Color.red;
                        //renderer.material.color = Color.red;
                        break;
                    case "GY":
                        colorInput = Color.gray;
                        //renderer.material.color = Color.gray;
                        break;
                    default:
                        colorInput = Color.magenta;
                        break;
                }
            }
            _controller.attach(new LA_Cuboid(position, scaling, rotation, colorInput));
        }
        catch (System.Exception e)
        {
            Debug.LogError("Error instantiating: " + e);
        }
    }

    //This for parsing text fields (x,y,z)
    Vector3 ParsePosition(string positionInput)
    {
        string[] components = positionInput.Split(',');
        if (components.Length == 1)
        {
            float scale = float.Parse(components[0]);
            return new Vector3(scale, scale, scale);
            //return Vector3.zero;
        }

        else if (components.Length == 3)
        {
            float x = float.Parse(components[0]);
            float y = float.Parse(components[1]);
            float z = float.Parse(components[2]);
            return new Vector3(x, y, z);
        }
        else
        {
            return Vector3.zero;
        }
    }

    //This for parsing text field for scale, 
    private float ParseFloat(string f)
    {
        float result;
        if (float.TryParse(f, out result))
        {
            return result;
        }
        else
        {
            return 0;
        }
    }

    //When menu Item is clicked open it's corresponding panel, if the same menu item clicked twice, hide panel.
    void MenuItemClicked(string menuItem)
    {
        int menuItemIndex = System.Array.IndexOf(menuItems, menuItem);

        if (menuItemIndex >= 0 && menuItemIndex < menuItemPanels.Count)
        {
            bool panelVisible = menuItemPanels[menuItemIndex];

            // Hide the menu bar 
            if (menuItem == menuItems[0])
            {
                // Hide each menu item
                menuBarVisible = false;

                foreach (string item in menuItems)
                {
                    menuItemPanels[System.Array.IndexOf(menuItems, item)] = false;
                }
            }
            else
            {
                foreach (bool item in menuItemPanels)
                {
                    if (item)
                    {
                        menuItemPanels[System.Array.IndexOf(menuItems, menuItems[System.Array.IndexOf(menuItemPanels.ToArray(), item)])] = false;
                        break;
                    }
                }

                menuItemPanels[menuItemIndex] = true;
            }

            if (panelVisible)
            {
                menuItemPanels[menuItemIndex] = false;
            }
            Debug.Log("Clicked on menu item: " + menuItem);
        }
    }
}
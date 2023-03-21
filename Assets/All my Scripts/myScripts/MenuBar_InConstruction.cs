using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;
using System;

public class MenuBar_InConstruction
{
    Controller _controller;
    bool menuBarVisible = true;
    private Vector2 scroll = Vector2.zero;

    string camPosition = "0,0,0";
    string[] newPosition;
    string[] newScaling;
    string[] newRotation;

    string[] colors = { "WT", "BL", "BK", "GN", "RD", "GY"};
    Color[] colorsNotString = { Color.white, Color.blue, Color.black, Color.green, Color.red, Color.grey };
    int selectedColorIndex = 0;

    //string txtFieldPos="";// = "0,0,0";
    string[] txtFieldPos;
    string[] txtFieldRot;
    string[] txtFieldScale;
    //string txtFieldRot = "0,0,0";
    string txtFieldTreeScale = "1";
    string positionInputCanvas = "0,0,0";

    private string titleValue = "";
    private string textValue = "";
    Vector3 CanvasPosition;
    string newTitle;
    string newText;

    string[] menuItems = { "Hide", "Edit", "GameObjects", "AddedObjects", "TextCanvas", "UpdateText", "Matrix","Help" };
    List<bool> menuItemPanels = new List<bool>();

    //List<(string positionInput, string scalingInput, string rotationInput, GameObject prefab)> objectData;
    List<(string positionInput, string scalingInput, string rotationInput, string name, Func<Vector3, Vector3, Vector3, Color, LA_Object> func)> objectData;

    List<string> canvasNames = new List<string>();
    private int canvasIndex;
    int selectedCanvasIndex = -1;

    //List<string> GONames = new List<string>();
    //List<string> GOVectorName = new List<string>();

    int selectedGOIndex = -1;

    bool addToggle; bool subToggle; bool crossToggle;

    public bool MenuBarVisible { get => menuBarVisible; }
    public bool Vector_Addition { get => addToggle; set => addToggle = value; }
    public bool Vector_Substraction { get => subToggle; set => subToggle = value; }
    public bool Cross_Product { get => crossToggle; set => crossToggle = value; }

    public MenuBar_InConstruction(Controller controller)
    {
        _controller = controller;
    }
    public void myStart()
    {
        for (int i = 0; i < menuItems.Length; i++)
        {
            menuItemPanels.Add(false);
        }
        objectData = new List<(string, string, string, string, Func<Vector3, Vector3, Vector3, Color, LA_Object>)>
            {
                ("0,0,0", "1,1,1", "0,0,0", "Cube", (v1, v2, v3, col) => new LA_Cuboid(v1, v2, v3, col)),
                ("0,0,0", "1,1,1", "0,0,0", "Ellipsoid", (v1, v2, v3, col) => new LA_Ellipsoid(v1, v2, v3, col)),
                ("0,0,0", "1,1,1", "0,0,0", "EllipticCylinder", (v1, v2, v3, col) => new LA_EllipticCylinder(v1, v2, v3, col)),
                ("0,0,0", "1,1,1", "0,0,0", "Capsule", (v1, v2, v3, col) => new LA_Capsule(v1, v2, v3, col)),
                ("0,0,0", "1,1,1", "0,0,0", "Rectangle", (v1, v2, v3, col) => new LA_Rectangle(v1, v2, v3, col)),
                ("0,0,0", "1", "0,0,0", "Tree", (v1, v2, v3, col) => new LA_Tree(v1, v2, v3, col)),
                ("0,0,0", "1,1,1", "0,0,0", "Mountain", (v1, v2, v3, col) => new LA_Mountain(v1, v2, v3, col)),
                ("0,0,0", "1,1,1", "", "Vector", (v1, v2, v3, col) => new LA_Vector(v1, v2, col)),
                ("0,0,0", "1,1,1", "0,0,0", "House", (v1, v2, v3, col) => new LA_House(v1, v2, v3, col)),
                ("0,0,0", "1,1,1", "0,0,0", "EllipticCone", (v1, v2, v3, col) => new LA_EllipticCone(v1, v2, v3, col)),
                //("0,0,0", "1,1,1", "0,0,0", "EllipticSquarePyramid", (v1, v2, v3, col) => new LA_EllipticSquarePyramid(v1, v2, v3, col)),
                ("0,0,0", "1,1,1", "0,0,0", "EllipticTriangularPrism", (v1, v2, v3, col) => new LA_EllipticTriangularPrism(v1, v2, v3, col)),
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
                GUI.Box(new Rect(0, Screen.height / 20, Screen.width, Screen.height / 1.1f), "");

                if (GUI.Button(new Rect(Screen.width - 50, Screen.height / 20, 50, 30), "X"))
                {
                    menuItemPanels[i] = false;
                }

                if (menuItemPanelKey == "Edit")
                {
                    GUILayout.BeginArea(new Rect(5, 20 * 5, Screen.width - 10, 90 * 5));
                    //scroll = GUILayout.BeginScrollView(scroll, GUILayout.Width(Screen.width - 10), GUILayout.Height(90 * 5));
                    GUILayout.BeginVertical();

                    GUIStyle style = new GUIStyle(GUI.skin.label);
                    style.fontStyle = FontStyle.Bold;
                    style.fontSize = 22;
                    GUI.Label(new Rect(10, 0, 250, 30), "Edit:", style);

                    // Add the toggles
                    GUIStyle toggleStyle = new GUIStyle(GUI.skin.toggle);
                    toggleStyle.fontSize = 18;
                    GUI.Label(new Rect(10, 160, 300, 30), "Toggles:");
                    addToggle = GUI.Toggle(new Rect(10, 190, 150, 30), addToggle, " Addition", toggleStyle);
                    subToggle = GUI.Toggle(new Rect(10, 220, 150, 30), subToggle, " Subtraction", toggleStyle);
                    crossToggle = GUI.Toggle(new Rect(10, 250, 150, 30), crossToggle, " Cross product", toggleStyle);

                    _controller.Vector_Operations[0].setActive(addToggle);
                    _controller.Vector_Operations[1].setActive(subToggle);
                    _controller.Vector_Operations[2].setActive(crossToggle);

                    if (GUI.Button(new Rect(10, 300, 150, 40), "Undo"))
                    {
                        _controller.undoLA_Object_Command();
                    }
                    if (GUI.Button(new Rect(10, 350, 150, 40), "Save 1"))
                    {
                        _controller.newSave("Save 1");
                    }
                    if (GUI.Button(new Rect(10, 400, 150, 40), "Save 2"))
                    {
                        _controller.newSave("Save 2");
                    }

                    if (camPosition == "") { camPosition = "0,0,0"; }

                    //GUILayout.Label("Camera Position:", GUILayout.Width(150));
                    GUI.Label(new Rect(10, 40, 300, 30), "Camera Position:");
                    camPosition = GUI.TextField(new Rect(10, 70, 150, 30), camPosition);


                    //camPosition = GUILayout.TextField(camPosition, GUILayout.Width(100));

                    if (GUI.Button(new Rect(10, 100, 150, 40), "Update"))
                    {
                        Vector3 CamPosition = ParsePosition(camPosition);

                        if (CamPosition != Vector3.zero)
                        {
                            Camera.main.transform.root.position = CamPosition;
                        }
                        //Reset
                        camPosition = "0,0,0";
                    }

                    //scroll.y += Input.mouseScrollDelta.y * 10;
                    GUILayout.EndVertical();
                    //GUILayout.EndScrollView();
                    GUILayout.EndArea();
                }

                // If the GameObjects panel is visible, add the options to create an object with specific position, scale, rotation and color
                else if (menuItemPanelKey == "GameObjects")
                {
                    if (newPosition == null || newPosition.Length != objectData.Count)
                    {
                        newPosition = new string[objectData.Count];
                        newScaling = new string[objectData.Count];
                        newRotation = new string[objectData.Count];

                        for (int j = 0; j < objectData.Count; j++)
                        {
                            //if (allObjects[j].getGameObject().transform.parent == null)
                            {
                                newPosition[j] = "0,0,0";//"0,0,0";
                                newScaling[j] = "1,1,1"; //string.Format("{0},{1},{2}", objectData[j].getGameObject().transform.eulerAngles.x.ToString("0"), allObjects[j].getGameObject().transform.eulerAngles.y.ToString("0"), allObjects[j].getGameObject().transform.eulerAngles.z.ToString("0"));
                                newRotation[j] = "0,0,0"; //string.Format("{0}", objectData[j].getGameObject().transform.localScale.x.ToString("0"));

                            }
                        }
                    }
                    //GUILayout.BeginArea(new Rect(5, 20 * 5, Screen.width - 10, 90 * 5));
                    //scroll = GUILayout.BeginScrollView(scroll, GUILayout.Width(Screen.width - 10), GUILayout.Height(90 * 5));
                    GUILayout.BeginVertical();

                    //GUIStyle style = new GUIStyle(GUI.skin.label);
                    //style.fontStyle = FontStyle.Bold;
                    //style.fontSize = 22;
                    //GUI.Label(new Rect(10, 0, 250, 30), "GameObjecs:", style);

                    GUILayout.BeginHorizontal();
                    GUI.Label(new Rect(10, 40, 250, 30), "Center Point: (x,y,z)");
                    GUI.Label(new Rect(230, 40, 250, 30), "Dimensions: (x,y,z)");
                    GUI.Label(new Rect(450, 40, 250, 30), "Euler Angles: (x,y,z)");
                    GUI.Label(new Rect(670, 40, 250, 30), "Choose a Color:");
                    GUI.Label(new Rect(990, 40, 250, 30), "Add Objects:");
                    GUILayout.EndHorizontal();

                    for (int j = 0; j < objectData.Count; j++)
                    {
                        string positionInput = objectData[j].positionInput;
                        string scalingInput = objectData[j].scalingInput;
                        string rotationInput = objectData[j].rotationInput;


                        /*if (newPosition == null) { newPosition[j] = "0,0,0"; }
                        else if (newScaling == null) { newScaling[j] = "1,1,1"; }
                        else if (newRotation == null) { newRotation[j] = "0,0,0"; }
                        else if (txtFieldTreeScale == null) { txtFieldTreeScale = "1"; }*/

                        //if (objectData[j].func != null && objectData[j].name != "Vector")
                        {
                            //newPosition[j] = GUI.TextField(new Rect(10, 85 + 40 * j, 200, 30), positionInput);
                            //newScaling[j] = GUI.TextField(new Rect(230, 85 + 40 * j, 200, 30), scalingInput);
                            //newRotation[j] = GUI.TextField(new Rect(450, 85 + 40 * j, 200, 30), rotationInput);
                            newPosition[j] = GUI.TextField(new Rect(10, 85 + 40 * j, 200, 30), newPosition[j]);
                            newScaling[j] = GUI.TextField(new Rect(230, 85 + 40 * j, 200, 30), newScaling[j]);
                            newRotation[j] = GUI.TextField(new Rect(450, 85 + 40 * j, 200, 30), newRotation[j]);
                        }
                        /*else
                        {
                            newPosition = GUI.TextField(new Rect(10, 125 + 40 * j, 200, 30), positionInput);
                            newScaling = GUI.TextField(new Rect(230, 125 + 40 * j, 200, 30), scalingInput);
                        }*/

                        //objectData[j] = (newPosition, newScaling, newRotation, objectData[j].name, objectData[j].func);
                        
                        // Add textfields for the new prefab
                        /*if (objectData[j].func != null && objectData[j].name == "Vector")
                        {
                            GUI.Label(new Rect(10, 365, 250, 30), "Start Point: (x,y,z)");
                            GUI.Label(new Rect(230, 365, 250, 30), "End Point: (x,y,z)");
                            
                            selectedColorIndex = GUI.SelectionGrid(new Rect(670, 125 + 40 * j, 300, 30), selectedColorIndex, colors, 6);

                            if (objectData[j].func != null && GUI.Button(new Rect(990, 125 + 40 * j, 150, 30), "Add " + objectData[j].name))
                            {
                                //prefab.tag = "tagged"; //ändra
                                //string newGOName = objectData[j].name + objectData.Count; //ändra
                                //GONames.Add(newGOName); //ändra

                                //AddObject(prefab, ParsePosition(newPosition), ParsePosition(newScaling), Vector3.zero, colors[selectedColorIndex]); // Set rotation to zero for the new prefab //ändra
                                //_controller.attach(new LA_Vector(ParsePosition(newPosition), ParsePosition(newScaling), colorsNotString[selectedColorIndex]));
                                LA_Object laobject = objectData[j].func(ParsePosition(newPosition), ParsePosition(txtFieldTreeScale), ParsePosition(newRotation), colorsNotString[selectedColorIndex]);
                                //laobject.setName(newGOName);
                                _controller.attach(laobject);

                                //Reset
                                newPosition = "0,0,0";
                                newScaling = "1,1,1";
                                selectedColorIndex = 0;
                            }
                            
                        }
                        else if (objectData[j].func != null && objectData[j].name == "Tree")
                        {
                            selectedColorIndex = GUI.SelectionGrid(new Rect(670, 85 + 40 * j, 300, 30), selectedColorIndex, colors, 6);

                            if (objectData[j].func != null && GUI.Button(new Rect(990, 85 + 40 * j, 150, 30), "Add " + objectData[j].name))
                            {
                                //prefab.tag = "tagged"; //ändra
                                //string newGOName = objectData[j].name + objectData.Count; //ändra
                                //GONames.Add(newGOName); //ändra

                                //AddObject(prefab, ParsePosition(newPosition), ParsePosition(txtFieldTreeScale), ParsePosition(newRotation), colors[selectedColorIndex]); // Set rotation to zero for the new prefab //ändra
                                LA_Object laobject = objectData[j].func(ParsePosition(newPosition), ParsePosition(txtFieldTreeScale), ParsePosition(newRotation), colorsNotString[selectedColorIndex]);
                                //laobject.setName(newGOName);
                                _controller.attach(laobject);
                                //Reset
                                newPosition = "0,0,0";
                                newRotation = "0,0,0";
                                txtFieldTreeScale = "1";
                                selectedColorIndex = 0;
                            }

                        }

                        else*/
                        {
                            selectedColorIndex = GUI.SelectionGrid(new Rect(670, 85 + 40 * j, 300, 30), selectedColorIndex, colors, 6);

                            if (objectData[j].func != null && GUI.Button(new Rect(990, 85 + 40 * j, 150, 30), "Add " + objectData[j].name))
                            {
                                //prefab.tag = "tagged"; //ändra
                                //string newGOName = objectData[j].name + objectData.Count; //ändra
                                //GONames.Add(newGOName); //ändra

                                //AddObject(prefab, ParsePosition(newPosition), ParsePosition(newScaling), ParsePosition(newRotation), colors[selectedColorIndex]); //ändra
                                LA_Object laobject = objectData[j].func(ParsePosition(newPosition[j]), ParsePosition(newScaling[j]), ParsePosition(newRotation[j]), colorsNotString[selectedColorIndex]);
                                //laobject.setName(newGOName);
                                _controller.attach(laobject);

                                //Reset
                                newPosition[j] = "0,0,0";
                                newScaling[j] = "1,1,1";
                                newRotation[j] = "0,0,0";
                                selectedColorIndex = 0;
                            }
                        }
                    }
                    //scroll.y += Input.mouseScrollDelta.y * 10;
                    GUILayout.EndVertical();
                    //GUILayout.EndScrollView();
                    //GUILayout.EndArea();
                }

                // If the AddedObjects panel is visible, show the objects with the options to edit
                else if (menuItemPanelKey == "AddedObjects")
                {
                    GUILayout.BeginArea(new Rect(5, 20 * 5, Screen.width - 10, 90 * 5));
                    scroll = GUILayout.BeginScrollView(scroll, GUILayout.Width(Screen.width - 10), GUILayout.Height(90 * 5));
                    GUILayout.BeginVertical();

                    GUIStyle style = new GUIStyle(GUI.skin.label);
                    style.fontStyle = FontStyle.Bold;
                    style.fontSize = 22;
                    GUILayout.Label("AddedObjects:", style, GUILayout.Width(250));

                    GUI.skin.label.fontSize = 15;
                    GUI.skin.label.fontStyle = FontStyle.Normal;

                    // Find all the objects in the scene
                    //List<GameObject> allObjects = (GameObject.FindGameObjectsWithTag("tagged")).ToList(); //ändra
                    List<LA_Object> allObjects = _controller.LA_Objects;

                    if (txtFieldPos == null || txtFieldPos.Length != allObjects.Count)
                    {
                        txtFieldPos = new string[allObjects.Count];
                        txtFieldRot = new string[allObjects.Count];
                        txtFieldScale = new string[allObjects.Count];

                        for (int j = 0; j < allObjects.Count; j++)
                        {
                            //if (allObjects[j].getGameObject().transform.parent == null)
                            {
                                txtFieldPos[j] = string.Format("{0},{1},{2}", allObjects[j].getGameObject().transform.position.x.ToString("0"), allObjects[j].getGameObject().transform.position.y.ToString("0"), allObjects[j].getGameObject().transform.position.z.ToString("0"));
                                txtFieldRot[j] = string.Format("{0},{1},{2}", allObjects[j].getGameObject().transform.eulerAngles.x.ToString("0"), allObjects[j].getGameObject().transform.eulerAngles.y.ToString("0"), allObjects[j].getGameObject().transform.eulerAngles.z.ToString("0"));
                                txtFieldScale[j] = string.Format("{0}", allObjects[j].getGameObject().transform.localScale.x.ToString("0"));

                            }
                        }
                    }


                    for (int j = 0; j < allObjects.Count; j++)
                    {

                        //if (allObjects[j].getGameObject().transform.parent == null)
                        {

                            GUILayout.BeginHorizontal();

                            //GUILayout.Label(GONames[j], GUILayout.Width(200)); //ändra
                            GUILayout.Label(allObjects[j].getName(), GUILayout.Width(200)); //ändra
                            GUILayout.Space(5);
                            GUILayout.Label("Position:", GUILayout.Width(60));
                            txtFieldPos[j] = GUILayout.TextField(txtFieldPos[j], GUILayout.Width(100));
                            GUILayout.Space(5);
                            GUILayout.Label("Rotation:", GUILayout.Width(60));
                            txtFieldRot[j] = GUILayout.TextField(txtFieldRot[j], GUILayout.Width(100));
                            GUILayout.Space(5);
                            GUILayout.Label("Scale:", GUILayout.Width(60));
                            txtFieldScale[j] = GUILayout.TextField(txtFieldScale[j], GUILayout.Width(100));
                            GUILayout.Space(5);


                            if (GUILayout.Button("Update", GUILayout.Width(100), GUILayout.Height(30)))
                            {
                                Vector3 objPosition = ParsePosition(txtFieldPos[j]);
                                Vector3 objRotation = ParsePosition(txtFieldRot[j]);
                                Vector3 objDimensions = ParsePosition(txtFieldScale[j]);
                                float[] objDimensions_Temp = new float[3] { objDimensions.x, objDimensions.y, objDimensions.z };
                                float scale = objDimensions_Temp.Min();

                                Vector3? objPosition_Temp = null;
                                Vector3? objRotation_Temp = null;
                                float? scale_Temp = null;
                                if (objPosition != Vector3.zero) { objPosition_Temp = objPosition; } //ändra
                                if (objRotation != Vector3.zero) { objRotation_Temp = objRotation; } //ändra
                                if (scale != 0) { scale_Temp = scale; } //ändra
                                if(objPosition_Temp.HasValue || objRotation_Temp.HasValue || scale_Temp.HasValue)
                                {
                                    _controller.addLA_Object_Transform(allObjects[j], objPosition, objRotation, scale);
                                }

                                //Reset text fields
                                //txtFieldPos = "0,0,0";
                                //txtFieldRot = "0,0,0";
                                //txtFieldScale = "1";
                            }

                            // Show delete button next to obj name
                            if (GUILayout.Button("Delete", GUILayout.Width(100), GUILayout.Height(30)))
                            {
                                // Delete the object from the scene
                                //GONames.RemoveAt(j); //ändra
                                //Destroy(allObjects[j]); //ändra
                                //allObjects.RemoveAt(j); //ändra
                                _controller.destroy(allObjects[j]);

                                txtFieldPos = txtFieldPos.Where((val, idx) => idx != j).ToArray();
                                txtFieldRot = txtFieldRot.Where((val, idx) => idx != j).ToArray();
                                txtFieldScale = txtFieldScale.Where((val, idx) => idx != j).ToArray();
                                break;
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

                    GUIStyle style = new GUIStyle(GUI.skin.label);
                    style.fontStyle = FontStyle.Bold;
                    style.fontSize = 22;
                    GUILayout.Label("TextCanvas:", style, GUILayout.Width(250));

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

                    GUIStyle style = new GUIStyle(GUI.skin.label);
                    style.fontStyle = FontStyle.Bold;
                    style.fontSize = 22;
                    GUILayout.Label("UpdateText:", style, GUILayout.Width(250));

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
                        //GUILayout.Label("To update the text first add a text canvas and then choose from the list, here to the left. ");
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

                    GUIStyle style = new GUIStyle(GUI.skin.label);
                    style.fontStyle = FontStyle.Bold;
                    style.fontSize = 22;
                    GUILayout.Label("Matrix:", style, GUILayout.Width(250));

                    //Show Object names on the left side of the panel
                    GUILayout.BeginVertical(GUILayout.Width(100));
                    //for (int j = 0; j < GONames.Count; j++)
                    for (int j = 0; j < _controller.LA_Objects.Count; j++)
                    {
                        //if (GUILayout.Button(GONames[j], GUILayout.Width(150), GUILayout.Height(40)))
                        if (GUILayout.Button(_controller.LA_Objects[j].getName(), GUILayout.Width(150), GUILayout.Height(40)))
                        {
                            selectedGOIndex = j;
                        }
                    }
                    GUILayout.EndVertical();

                    GUILayout.BeginVertical(GUILayout.Width(Screen.width - 150));
                    //if (selectedGOIndex >= 0 && selectedGOIndex < GONames.Count)
                    if (selectedGOIndex >= 0 && selectedGOIndex < _controller.LA_Objects.Count)
                    {
                        //string objName = GONames[selectedGOIndex];
                        string matrix = _controller.LA_Objects[selectedGOIndex].toString();
                        //GameObject GO = GameObject.Find(objName);

                        //if (GO != null)
                        {
                            //Matrix
                            //GUI.Label(new Rect(200, 200, 1000, 500), "Matrix Here: —");
                            GUI.Label(new Rect(200, 200, 1000, 500), matrix);

                        }
                    }
                    else
                    {
                        //GUILayout.Label("To see objects Matrix first add an object and then choose from the list, here to the left. ");
                    }

                    scroll.y += Input.mouseScrollDelta.y * 10;

                    GUILayout.EndVertical();
                    GUILayout.EndHorizontal();
                    GUILayout.EndScrollView();
                    GUILayout.EndArea();
                }
                else if (menuItemPanelKey == "Help")
                {
                    GUILayout.BeginVertical();
                    GUI.Label(new Rect(200, 200, 1000, 500), "W, A, S, D on the keyboard are used for user movement.");
                    GUI.Label(new Rect(200, 300, 1000, 500), "Zoom in and out by using the mouse wheel.");
                    GUI.Label(new Rect(200, 400, 1000, 500), "Click on the linear algebra object to instantiate the handles for visual manipulation. B and N on they keyboard lets you switch the type of handles.");
                    GUI.Label(new Rect(200, 500, 1000, 500), "Vector operations can be found in the Edit section. Toggle on/off to add in the VR world or not.");
                    GUI.Label(new Rect(200, 600, 1000, 500), "Instantiate your linear algebra object in the GameObjects section.");
                    GUILayout.EndVertical();
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
            //var Obj = Instantiate(prefab, position, Quaternion.Euler(rotation)); //ändra
            //Obj.transform.localScale = scaling;

            //Renderer renderer = Obj.GetComponent<Renderer>();
            //if (renderer != null)
            {
                switch (color)
                {
                    case "WT":
                        //renderer.material.color = Color.white;
                        break;
                    case "BL":
                        //renderer.material.color = Color.blue;
                        break;
                    case "BK":
                        //renderer.material.color = Color.black;
                        break;
                    case "GN":
                        //renderer.material.color = Color.green;
                        break;
                    case "RD":
                        //renderer.material.color = Color.red;
                        break;
                    case "GY":
                        //renderer.material.color = Color.gray;
                        break;
                }
            }
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
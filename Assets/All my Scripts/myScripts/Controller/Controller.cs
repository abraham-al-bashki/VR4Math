using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro;
using UnityEditor;
using System.IO;
using System.Text;

public class Controller : MonoBehaviour, LoadAssets
{
    internal List<LA_Object> _list; //for onGUI
    //private List<LA_Vector> _vectors; //for vector addition, cross product...
    internal List<ICommand> _commands;
    //private int currentIndexForCommands;
    private LA_Object _target;
    private Handles _handles;
    private LA_VRCanvas _canvas;
    private myLeftRayCastHit _myLeft;
    //private MenuBar _menubar;
    private MenuBar_InConstruction _menubar;
    private UserZooming _userZooming;
    private List<Vector_Operation> vector_Operations;
    [SerializeField] private int selected = 0;
    [SerializeField] TMP_Text text;
    private int _selectedPrevState = 0;
    private static bool _isSavingProgress = false;
    public static bool isSavingProgress { get => _isSavingProgress; private set => _isSavingProgress = value; }
    public LA_Object getTarget { get => _target; private set => _target = value; }
    public Handles getHandles { get => _handles; private set => _handles = value; }
    public List<LA_Object> LA_Objects { get => _list; private set => _list = value; }
    public List<Vector_Operation> Vector_Operations { get => vector_Operations; private set => vector_Operations = value; }
    public bool MenuBarVisible { get => _menubar.MenuBarVisible; }

    private void Awake()
    {
        _isSavingProgress = true;
        _list = new List<LA_Object>();
        vector_Operations = new List<Vector_Operation>();
        //currentIndexForCommands = -1;
        _commands = new List<ICommand>();
        _handles = new Handles();
        _canvas = new LA_VRCanvas();
        GameObject leftHandController = GameObject.Find("LeftHand Controller");
        _myLeft = new myLeftRayCastHit(this, _handles, leftHandController);
        //_menubar = new MenuBar(this);
        _menubar = new MenuBar_InConstruction(this);
        _userZooming = new UserZooming(this);
        _myLeft.myAwake();
        vector_Operations.Add(new Vector_Addition(this));
        vector_Operations.Add(new Vector_Subtraction(this));
        vector_Operations.Add(new Cross_Product(this));
    }
    private void OnEnable()
    {
        _myLeft.myOnEnable();
    }
    private void OnDisable()
    {
        _myLeft.myOnDisable();
    }
    private void Start()
    {
        load("Save 1"); //cannot to be in Awake/OnEnable because Awake are not always before OnEnable
        _menubar.myStart();
    }
    private void OnGUI()
    {
        _menubar.myOnGUI();
    }
    private void Update()
    {
        _isSavingProgress = false;
        _myLeft.myUpdate();
        for(int i = 0; i < vector_Operations.Count; i++) { vector_Operations[i].myUpdate(); }
        if (_selectedPrevState != selected)
        {
            switch (selected)
            {
                case 0: _handles.selectHandle(Handles.HandleType.TranslationTool); _selectedPrevState = selected; return;
                case 1: _handles.selectHandle(Handles.HandleType.RotationTool); _selectedPrevState = selected; return;
                case 2: _handles.selectHandle(Handles.HandleType.ScaleTool); _selectedPrevState = selected; return;
                default: selected = _selectedPrevState; return;
            }
        }
        
    }
    private void LateUpdate()
    {
        _handles.myLateUpdate();
        _canvas.myLateUpdate();
        _userZooming.myLateUpdate();
    }

    public void load(string saveFolder)
    {
#if UNITY_EDITOR
        string path = "Assets/Resources/" + saveFolder;
        string[] files = Directory.GetFiles(path, "*.prefab", SearchOption.TopDirectoryOnly);
        foreach (var file in files)
        {
            GameObject temp = (GameObject) AssetDatabase.LoadAssetAtPath(file, typeof(GameObject));
            temp = (GameObject)PrefabUtility.InstantiatePrefab(temp);
            if (temp.transform.TryGetComponent<LA_Object_Component>(out LA_Object_Component loc))
            {
                loc.controller = this;
                //loc.OnEnable();
                if (loc.laobject != null)
                {
                    attach(loc.laobject);
                }
                else
                {
                    loc.OnEnable();
                    attach(loc.laobject);
                }
            }
        }
#else
        GameObject[] arr = Resources.LoadAll(saveFolder, typeof(GameObject)).Cast<GameObject>().ToArray();
        int len = arr.Length;
        _list = new List<LA_Object>();
        for (int i = 0; i < len; i++)
        {
            GameObject temp = GameObject.Instantiate(arr[i]);
            //createStaticMesh(temp); //why is this making the info in Update GUI not visible
            if (temp.transform.TryGetComponent<LA_Object_Component>(out LA_Object_Component loc))
            {
                loc.controller = this;
                if (loc.laobject != null)
                {
                    attach(loc.laobject);
                }
            }
        }
#endif
        string textPath = saveFolder + "/Vector_Operations";
        StringReader reader = new StringReader(Resources.Load<TextAsset>(textPath).text);
        string line = reader.ReadLine();
        if (line.Equals("True")) { vector_Operations[0].setActive(true); _menubar.Vector_Addition = true; }//set active Vector Addition in GUI & on the GameObjects
        else { vector_Operations[0].setActive(false); _menubar.Vector_Addition = false; }
        line = reader.ReadLine();
        if (line.Equals("True")) { vector_Operations[1].setActive(true); _menubar.Vector_Substraction = true; }//set active Vector Substraction in GUI & on the GameObjects
        else { vector_Operations[1].setActive(false); _menubar.Vector_Substraction = false; }
        line = reader.ReadLine();
        if (line.Equals("True")) { vector_Operations[2].setActive(true); _menubar.Cross_Product = true; }//set active Cross Product in GUI & on the GameObjects
        else { vector_Operations[2].setActive(false); _menubar.Cross_Product = false; }
        reader.Close();
    }
    public void createStaticMesh(GameObject instance)
    {
        MeshFilter filterInstance = instance.GetComponentInChildren<MeshFilter>();
        //if (filterInstance == null) { return null; }
        if (filterInstance == null) { return; }
        string mf = filterInstance.name;
        GameObject mesh;
        if (mf.Equals("Cube")) { mesh = GameObject.CreatePrimitive(PrimitiveType.Cube); }
        else if (mf.Equals(CompareMeshFilter.sphereFilter.mesh.name)) { mesh = GameObject.CreatePrimitive(PrimitiveType.Sphere); }
        else if (mf.Equals(CompareMeshFilter.cylinderFilter.mesh.name)) { mesh = GameObject.CreatePrimitive(PrimitiveType.Cylinder); }
        else if (mf.Equals(CompareMeshFilter.capsuleFilter.mesh.name)) { mesh = GameObject.CreatePrimitive(PrimitiveType.Capsule); }
        else if (mf.Equals(CompareMeshFilter.quadFilter.mesh.name)) { mesh = GameObject.CreatePrimitive(PrimitiveType.Quad); }
        else if (mf.Equals(CompareMeshFilter.treeFilter.mesh.name)) { mesh = GameObject.Instantiate<GameObject>(Resources.Load<GameObject>("Geometry/Prefab/Tree")); }
        /*else if (mf.Equals(CompareMeshFilter.mountainFilter.mesh.name)) {  }
        else if (mf.Equals(CompareMeshFilter.houseFilter.mesh.name)) {  }
        else if (mf.Equals(CompareMeshFilter.coneFilter.mesh.name)) {  }
        else if (mf.Equals(CompareMeshFilter.squarePyramidFilter.mesh.name)) {  }
        else if (mf.Equals(CompareMeshFilter.triangularPrismFilter.mesh.name)) {  }*/
        //else { return null; }
        else { return; }
        Transform newTransform = mesh.transform;
        newTransform.position = filterInstance.transform.position;
        newTransform.rotation = filterInstance.transform.rotation;
        newTransform.localScale = filterInstance.transform.localScale;
        ColorScripts.getRightColorScript(filterInstance.GetComponent<MeshRenderer>().material.color, mesh);
        newTransform.SetParent(instance.transform);
        DestroyImmediate(filterInstance.gameObject);
        //return newTransform.parent.gameObject;
    }
    public void newSave(string saveFolder)
    {
#if UNITY_EDITOR
        string[] folder = { "Assets/Resources/" + saveFolder };
        foreach (var asset in AssetDatabase.FindAssets("", folder))
        {
            var path = AssetDatabase.GUIDToAssetPath(asset);
            AssetDatabase.DeleteAsset(path);
        }
        AssetDatabase.Refresh();
        foreach (var laobject in _list)
        {
            ((SaveAssets)laobject).save(saveFolder);
        }
        string textPath = "Assets/Resources/" + saveFolder + "/Vector_Operations.txt";
        //StreamWriter writer = new StreamWriter(textPath, false);
        string str = string.Format("{0}\n{1}\n{2}\n", _menubar.Vector_Addition.ToString(), _menubar.Vector_Substraction.ToString(), _menubar.Cross_Product.ToString());
        //writer.WriteLine(str);
        //writer.Close();
        File.WriteAllText(textPath, str);

        AssetDatabase.ImportAsset(textPath);
        TextAsset textAsset = (TextAsset)Resources.Load(saveFolder + "/Vector_Operations");
        Debug.Log(textAsset.text);

#endif
    }

    public void newFile(string saveFolder)
    {
#if UNITY_EDITOR
        string[] folder = { "Assets/Resources/" + saveFolder };
        foreach (var asset in AssetDatabase.FindAssets("", folder))
        {
            var path = AssetDatabase.GUIDToAssetPath(asset);
            AssetDatabase.DeleteAsset(path);
        }
        foreach (var laobject in _list)
        {
            laobject.destroy(this);
        }
#endif
    }

    public void selectTarget(LA_Object target)
    {
        _handles.selectTarget(target.getGameObject().transform);
        _canvas.selectTarget(target.getGameObject().transform);
        _target = target;
        if (!_handles.checkHandleIsActive()) { _handles.selectHandle(Handles.HandleType.TranslationTool); }
    }

    public void addLA_Object_Scale(float scale)
    {
        LA_Object_Scale command = new LA_Object_Scale(_target, scale);
        _commands.Add(command);
        command.execute();
        //currentIndexForCommands++;
        _canvas.setText(_target.toString());
    }
    public void addLA_Object_Rotate(Vector3 eulerAngles)
    {
        LA_Object_Rotate command = new LA_Object_Rotate(_target, eulerAngles);
        _commands.Add(command);
        command.execute();
        //currentIndexForCommands++;
        _canvas.setText(_target.toString());
    }
    public void addLA_Object_Translation(Vector3 translation)
    {
        LA_Object_Translate command = new LA_Object_Translate(_target, translation);
        _commands.Add(command);
        command.execute();
        //currentIndexForCommands++;
        _canvas.setText(_target.toString());
    }
    public void addLA_Object_Scale(LA_Object o, float scale)
    {
        LA_Object_Scale command = new LA_Object_Scale(o, scale);
        _commands.Add(command);
        command.execute();
        //currentIndexForCommands++;
        if (_target == o) { _canvas.setText(_target.toString()); }
    }

    public void addLA_Object_Rotate(LA_Object o, Vector3 eulerAngles)
    {
        LA_Object_Rotate command = new LA_Object_Rotate(o, eulerAngles);
        _commands.Add(command);
        command.execute();
        //currentIndexForCommands++;
        if (_target == o) { _canvas.setText(_target.toString()); }
    }

    public void addLA_Object_Translation(LA_Object o, Vector3 translation)
    {
        LA_Object_Translate command = new LA_Object_Translate(o, translation);
        _commands.Add(command);
        command.execute();
        //currentIndexForCommands++;
        if (_target == o) { _canvas.setText(_target.toString()); }
    }
    public void addLA_Object_Transform(LA_Object o, Vector3? translation, Vector3? eulerAngles, float? scale)
    {
        LA_Object_Transform command = new LA_Object_Transform(o, translation, eulerAngles, scale);
        _commands.Add(command);
        command.execute();
        //currentIndexForCommands++;
        if(_target == o) { _canvas.setText(_target.toString()); }
    }

    public void undoLA_Object_Command()
    {
        //if (currentIndexForCommands == -1) { return; }
        //_commands[currentIndexForCommands].undo();
        //_commands.RemoveAt(currentIndexForCommands);
        //currentIndexForCommands--;
        if (_commands.Count <= 0) { return; }
        int index = _commands.Count - 1;
        _commands[index].undo();
        _commands.RemoveAt(index);
    }

    public void attach(LA_Object o)
    {
        o.draw();
        bool isElliptic = (o.getGameObject().transform.localScale / o.getScaling()[0]) != Vector3.one;
        if ((o is LA_Cuboid) && isElliptic) { o.setName("Cuboid" + LA_Objects.Count); }
        else if ((o is LA_Cuboid) && !isElliptic) { o.setName("Cube" + LA_Objects.Count); }
        else if (o is LA_Vector) { o.setName("Vector" + LA_Objects.Count); }
        else if ((o is LA_Ellipsoid) && isElliptic) { o.setName("Ellipsoid" + LA_Objects.Count); }
        else if ((o is LA_Ellipsoid) && !isElliptic) { o.setName("Sphere" + LA_Objects.Count); }
        else if ((o is LA_EllipticCylinder) && isElliptic) { o.setName("EllipticCylinder" + LA_Objects.Count); }
        else if ((o is LA_EllipticCylinder) && !isElliptic) { o.setName("Cylinder" + LA_Objects.Count); }
        else if ((o is LA_Capsule)) { o.setName("Capsule" + LA_Objects.Count); }
        else if ((o is LA_Rectangle)) { o.setName("Rectangle" + LA_Objects.Count); }
        else if ((o is LA_Tree)) { o.setName("Tree" + LA_Objects.Count); }
        else if ((o is LA_House)) { o.setName("House" + LA_Objects.Count); }
        else if ((o is LA_Mountain)) { o.setName("Mountain" + LA_Objects.Count); }
        else if ((o is LA_EllipticCone) && isElliptic) { o.setName("EllipticCone" + LA_Objects.Count); }
        else if ((o is LA_EllipticCone) && !isElliptic) { o.setName("Cone" + LA_Objects.Count); }
        else if ((o is LA_EllipticTriangularPrism) && isElliptic) { o.setName("EllipticTriangularPrism" + LA_Objects.Count); }
        else if ((o is LA_EllipticTriangularPrism) && !isElliptic) { o.setName("TriangularPrism" + LA_Objects.Count); }

        _list.Add(o);
    }
    public void destroy(LA_Object o)
    {
        o.destroy(this);
        _list.Remove(o);
    }
}

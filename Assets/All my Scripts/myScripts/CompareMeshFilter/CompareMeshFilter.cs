using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CompareMeshFilter : MonoBehaviour
{
    private static MeshFilter _cubeFilter;
    private static MeshFilter _cylinderFilter;
    private static MeshFilter _sphereFilter;
    private static MeshFilter _capsuleFilter;
    private static MeshFilter _planeFilter;
    private static MeshFilter _quadFilter;
    private static MeshFilter _treeFilter;
    private static MeshFilter _mountainFilter;
    private static MeshFilter _houseFilter;
    private static MeshFilter _triangularPrismFilter;
    private static MeshFilter _coneFilter;
    private static MeshFilter _squarePyramidFilter;

    public static MeshFilter cubeFilter { get => _cubeFilter; }
    public static MeshFilter cylinderFilter { get => _cylinderFilter; }
    public static MeshFilter sphereFilter { get => _sphereFilter; }
    public static MeshFilter capsuleFilter { get => _capsuleFilter; }
    public static MeshFilter planeFilter { get => _planeFilter; }
    public static MeshFilter quadFilter { get => _quadFilter; }
    public static MeshFilter treeFilter { get => _treeFilter; }
    public static MeshFilter mountainFilter { get => _mountainFilter; }
    public static MeshFilter houseFilter { get => _houseFilter; }
    public static MeshFilter triangularPrismFilter { get => _triangularPrismFilter; }
    public static MeshFilter coneFilter { get => _coneFilter; }
    public static MeshFilter squarePyramidFilter { get => _squarePyramidFilter; }
    private void Awake()
    {
        Transform parent = new GameObject().transform;
        parent.name = "MeshFilters";

        GameObject gameObj = GameObject.CreatePrimitive(PrimitiveType.Cube);
        _cubeFilter = gameObj.GetComponent<MeshFilter>();
        Destroy(gameObj.GetComponent<MeshRenderer>());
        gameObj.transform.localScale = Vector3.zero;
        Destroy(gameObj.GetComponent<BoxCollider>());
        gameObj.name = "CubeFilter";
        gameObj.transform.SetParent(parent);

        gameObj = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        _cylinderFilter = gameObj.GetComponent<MeshFilter>();
        Destroy(gameObj.GetComponent<MeshRenderer>());
        Destroy(gameObj.GetComponent<CapsuleCollider>());
        gameObj.name = "CylinderFilter";
        gameObj.transform.SetParent(parent);

        gameObj = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        _sphereFilter = gameObj.GetComponent<MeshFilter>();
        Destroy(gameObj.GetComponent<MeshRenderer>());
        Destroy(gameObj.GetComponent<SphereCollider>());
        gameObj.name = "SphereFilter";
        gameObj.transform.SetParent(parent);

        gameObj = GameObject.CreatePrimitive(PrimitiveType.Capsule);
        _capsuleFilter = gameObj.GetComponent<MeshFilter>();
        Destroy(gameObj.GetComponent<MeshRenderer>());
        Destroy(gameObj.GetComponent<CapsuleCollider>());
        gameObj.name = "CapsuleFilter";
        gameObj.transform.SetParent(parent);

        gameObj = GameObject.CreatePrimitive(PrimitiveType.Plane);
        _planeFilter = gameObj.GetComponent<MeshFilter>();
        Destroy(gameObj.GetComponent<MeshRenderer>());
        Destroy(gameObj.GetComponent<MeshCollider>());
        gameObj.name = "PlaneFilter";
        gameObj.transform.SetParent(parent);

        gameObj = GameObject.CreatePrimitive(PrimitiveType.Quad);
        _quadFilter = gameObj.GetComponent<MeshFilter>();
        Destroy(gameObj.GetComponent<MeshRenderer>());
        Destroy(gameObj.GetComponent<MeshCollider>());
        gameObj.name = "QuadFilter";
        gameObj.transform.SetParent(parent);

        gameObj = Instantiate(Resources.Load<GameObject>("Geometry/Prefab/Tree"));
        _treeFilter = gameObj.GetComponent<MeshFilter>();
        Destroy(gameObj.GetComponent<MeshRenderer>());
        Destroy(gameObj.GetComponent<MeshCollider>());
        gameObj.name = "TreeFilter";
        gameObj.transform.SetParent(parent);
        
        gameObj = Instantiate(Resources.Load<GameObject>("Geometry/Prefab/Mountain"));
        _mountainFilter = gameObj.GetComponent<MeshFilter>();
        Destroy(gameObj.GetComponent<MeshRenderer>());
        Destroy(gameObj.GetComponent<MeshCollider>());
        gameObj.name = "MountainFilter";
        gameObj.transform.SetParent(parent);

        gameObj = Instantiate(Resources.Load<GameObject>("Geometry/Prefab/House"));
        _houseFilter = gameObj.GetComponent<MeshFilter>();
        Destroy(gameObj.GetComponent<MeshRenderer>());
        Destroy(gameObj.GetComponent<MeshCollider>());
        gameObj.name = "HouseFilter";
        gameObj.transform.SetParent(parent);

        gameObj = Instantiate(Resources.Load<GameObject>("Geometry/Prefab/TriangularPrism"));
        _triangularPrismFilter = gameObj.GetComponent<MeshFilter>();
        Destroy(gameObj.GetComponent<MeshRenderer>());
        Destroy(gameObj.GetComponent<MeshCollider>());
        gameObj.name = "TriangularPrismFilter";
        gameObj.transform.SetParent(parent);

        gameObj = Instantiate(Resources.Load<GameObject>("Geometry/Prefab/Cone"));
        _coneFilter = gameObj.GetComponent<MeshFilter>();
        Destroy(gameObj.GetComponent<MeshRenderer>());
        Destroy(gameObj.GetComponent<MeshCollider>());
        gameObj.name = "ConeFilter";
        gameObj.transform.SetParent(parent);

        gameObj = Instantiate(Resources.Load<GameObject>("Geometry/Prefab/SquarePyramid"));
        _squarePyramidFilter = gameObj.GetComponent<MeshFilter>();
        Destroy(gameObj.GetComponent<MeshRenderer>());
        Destroy(gameObj.GetComponent<MeshCollider>());
        gameObj.name = "SquarePyramidFilter";
        gameObj.transform.SetParent(parent);
    }
}

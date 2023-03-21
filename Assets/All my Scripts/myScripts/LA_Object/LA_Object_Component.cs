using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LA_Object_Component : MonoBehaviour
{
    private LA_Object _laobject;
    private bool _isFirstTime = true;
    private Controller _controller;
    public LA_Object laobject { get { return _laobject; } set { if (_isFirstTime) { _laobject = value; _isFirstTime = false; } } }
    public Controller controller { set { _controller = value; } }

    public void OnEnable()
    {

        if (!Controller.isSavingProgress) { return; }
        if (_laobject != null) { return; }
        isLineRender();
        isMeshFilter();
    }

    private void isLineRender()
    {
        LineRenderer[] lr = transform.GetComponentsInChildren<LineRenderer>();
        if (lr.Length != 2) { return; }
        //Extract data from the Transform and from the LineRenderer component
        //need to apply rotation, translation only through Transform
        //need to keep the initial points the user input in LineRenderer
        //Solution of keeping the scaling data is to add a third point into LineRenderer
        //Comprimise of having rotation with transform which lines not starting from the...
        //...World Space's origin will still rotate around the World Space's origin!.
        GameObject _line;
        GameObject _vectorhead;
        if (Mathf.Abs(Vector3.Magnitude(lr[0].GetPosition(0) - lr[0].GetPosition(lr[0].positionCount - 1))) < Mathf.Abs(Vector3.Magnitude(lr[1].GetPosition(0) - lr[1].GetPosition(lr[1].positionCount - 1))))
        {
            _line = lr[1].gameObject;
            _vectorhead = lr[0].gameObject;
        }
        else if (Mathf.Abs(Vector3.Magnitude(lr[0].GetPosition(0) - lr[0].GetPosition(lr[0].positionCount - 1))) > Mathf.Abs(Vector3.Magnitude(lr[1].GetPosition(0) - lr[1].GetPosition(lr[1].positionCount - 1))))
        {
            _line = lr[0].gameObject;
            _vectorhead = lr[1].gameObject;
        }
        else
        {
            return;
        }
        Transform _parent = transform;
        LineRenderer _lr = _line.GetComponent<LineRenderer>();
        LineRenderer _vhr = _vectorhead.GetComponent<LineRenderer>();
        Vector3 _orientation = _line.transform.rotation.eulerAngles;
        Vector3 _origin = _line.transform.position;
        Vector3 _initialStartPoint = _lr.GetPosition(0);
        Vector3 _initialEndPoint = _lr.GetPosition(_lr.positionCount - 2);
        Vector3 _endPoint = _lr.GetPosition(_lr.positionCount - 1);
        _laobject = new LA_Vector(_parent, _line, _vectorhead, _lr, _vhr, _orientation, _origin, _initialStartPoint, _initialEndPoint, _endPoint);
        _isFirstTime = true;
        if (_controller != null) { _controller.attach(_laobject); }
    }
    private void isMeshFilter()
    {
        MeshRenderer[] mr = transform.GetComponentsInChildren<MeshRenderer>();
        if (mr.Length != 1) { return; }
        Transform _parent = transform;
        Transform _shape = mr[0].gameObject.transform;
        Vector3 _centerPoint = _shape.position;
        float[] dimensions = new float[3] { _shape.localScale.x, _shape.localScale.y, _shape.localScale.z };
        float _scale = dimensions.Min();
        Vector3 _dimensions = _shape.localScale / _scale;
        Vector3 _orientation = transform.eulerAngles;
        
        string mf = _shape.GetComponent<MeshFilter>().sharedMesh.name;
        if (mf.Equals(CompareMeshFilter.cubeFilter.mesh.name)) { _laobject = new LA_Cuboid(_parent, _shape, _orientation, _centerPoint, _scale, _dimensions); } 
        else if (mf.Equals(CompareMeshFilter.sphereFilter.mesh.name)) { _laobject = new LA_Ellipsoid(_parent, _shape, _orientation, _centerPoint, _scale, _dimensions); }
        else if(mf.Equals(CompareMeshFilter.cylinderFilter.mesh.name)) { _laobject = new LA_EllipticCylinder(_parent, _shape, _orientation, _centerPoint, _scale, _dimensions); }
        else if(mf.Equals(CompareMeshFilter.capsuleFilter.mesh.name)) { _laobject = new LA_Capsule(_parent, _shape, _orientation, _centerPoint, _scale, _dimensions); }
        else if (mf.Equals(CompareMeshFilter.quadFilter.mesh.name)) { _laobject = new LA_Rectangle(_parent, _shape, _orientation, _centerPoint, _scale, _dimensions); }
        else if (mf.Equals(CompareMeshFilter.treeFilter.mesh.name)) { _laobject = new LA_Tree(_parent, _shape, _orientation, _centerPoint, _scale, _dimensions); }
        else if (mf.Equals(CompareMeshFilter.mountainFilter.mesh.name)) { _laobject = new LA_Mountain(_parent, _shape, _orientation, _centerPoint, _scale, _dimensions); }
        else if (mf.Equals(CompareMeshFilter.houseFilter.mesh.name)) { _laobject = new LA_House(_parent, _shape, _orientation, _centerPoint, _scale, _dimensions); }
        else if (mf.Equals(CompareMeshFilter.coneFilter.mesh.name)) { _laobject = new LA_EllipticCone(_parent, _shape, _orientation, _centerPoint, _scale, _dimensions); }
        else if (mf.Equals(CompareMeshFilter.squarePyramidFilter.mesh.name)) { _laobject = new LA_EllipticSquarePyramid(_parent, _shape, _orientation, _centerPoint, _scale, _dimensions); }
        else if (mf.Equals(CompareMeshFilter.triangularPrismFilter.mesh.name)) { _laobject = new LA_EllipticTriangularPrism(_parent, _shape, _orientation, _centerPoint, _scale, _dimensions); }

        if (mf.Equals("Cube")) { _laobject = new LA_Cuboid(_parent, _shape, _orientation, _centerPoint, _scale, _dimensions); }
        else if (mf.Equals("Sphere")) { _laobject = new LA_Ellipsoid(_parent, _shape, _orientation, _centerPoint, _scale, _dimensions); }
        else if (mf.Equals("Cylinder")) { _laobject = new LA_EllipticCylinder(_parent, _shape, _orientation, _centerPoint, _scale, _dimensions); }
        else if (mf.Equals("Capsule")) { _laobject = new LA_Capsule(_parent, _shape, _orientation, _centerPoint, _scale, _dimensions); }
        else if (mf.Equals("Quad")) { _laobject = new LA_Rectangle(_parent, _shape, _orientation, _centerPoint, _scale, _dimensions); }
        else if (mf.Equals("Tree Instance")) { _laobject = new LA_Tree(_parent, _shape, _orientation, _centerPoint, _scale, _dimensions); }
        else if (mf.Equals("Mountain Instance")) { _laobject = new LA_Mountain(_parent, _shape, _orientation, _centerPoint, _scale, _dimensions); }
        else if (mf.Equals("House Instance")) { _laobject = new LA_House(_parent, _shape, _orientation, _centerPoint, _scale, _dimensions); }
        else if (mf.Equals("Cone Instance")) { _laobject = new LA_EllipticCone(_parent, _shape, _orientation, _centerPoint, _scale, _dimensions); }
        else if (mf.Equals("SquarePyramid Instance")) { _laobject = new LA_EllipticSquarePyramid(_parent, _shape, _orientation, _centerPoint, _scale, _dimensions); }
        else if (mf.Equals("TriangularPrism Instance")) { _laobject = new LA_EllipticTriangularPrism(_parent, _shape, _orientation, _centerPoint, _scale, _dimensions); }

        _isFirstTime = true;
        //if ((_controller != null) && (_laobject != null)) { _controller.attach(_laobject); }//this would never work
        
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class checkVertices : MonoBehaviour
{
    List<checkVertices> _list;
    private int _num;
    public checkVertices(int i, GameObject obj)
    {
        _num = i;
        obj.AddComponent(this.GetType());
    }
    void Start()
    {
        /*
        Debug.Log("Scale: " + transform.localScale);
        Vector3[] readVertices = GetComponent<MeshFilter>().mesh.vertices;
        for (int i = 0; i < readVertices.Length; i++) { Debug.Log(readVertices[i]); }
        */
        /*
        Debug.Log("Scale: " + transform.localScale);
        int[] readTriangles = GetComponent<MeshFilter>().mesh.triangles;
        for (int i = 0; i < readTriangles.Length; i++) { Debug.Log(readTriangles[i]); }
        */
        /*
        Debug.Log("Scale: " + transform.localScale);
        Vector3[] readNormals = GetComponent<MeshFilter>().mesh.normals;
        for (int i = 0; i < readNormals.Length; i++) { Debug.Log(readNormals[i]); }
        */
        /**/
        //Debug.Log("Scale: " + transform.localScale);
        //Debug.Log(GetComponent<MeshFilter>().mesh.bounds.size);


        /*Vector3[] readNorm = GetComponent<MeshFilter>().mesh.normals;
        for(int i = 0; i < readNorm.Length; i++)
        {
            Debug.Log(i + ", angle: " + Mathf.Acos(Vector3.Dot(readNorm[i], Vector3.up)) * Mathf.Rad2Deg);
            Debug.Log("coord: " + readNorm[i]);
        }*/

        _list = new List<checkVertices>();
        for(int i = 0+1; i < 2+1; i++)
        {
            _list.Add(new checkVertices(i, gameObject));
        }
        Debug.Log(_num);
    }

}

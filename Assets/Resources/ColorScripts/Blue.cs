using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blue : MonoBehaviour
{
    private static readonly Color _color = Color.blue;
    private bool _isLrOrMr;
    public static Color color { get => _color; }
    public bool isLrOrMr { get => _isLrOrMr; }
    void Awake()
    {
        _isLrOrMr = true;
        LineRenderer lr;
        if (gameObject.TryGetComponent<LineRenderer>(out lr))
        {
            lr.material = new Material(Shader.Find("Unlit/Color"));
            lr.material.color = _color;
        }
        MeshRenderer mr;
        if (gameObject.TryGetComponent<MeshRenderer>(out mr))
        {
            mr.material = new Material(Shader.Find("Unlit/Color"));
            mr.material.color = _color;
        }
        if((lr == null) && (mr == null)) { _isLrOrMr = false; }
    }
}

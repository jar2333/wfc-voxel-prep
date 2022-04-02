using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[ExecuteInEditMode]
public class RenderLabels : MonoBehaviour
{

    public string front;
    public string right;
    public string back;
    public string left;
    public string top;
    public string bottom;

    void OnDrawGizmos()
    {
        Handles.Label(transform.position + new Vector3(1,0,0), front);
        Handles.Label(transform.position + new Vector3(-1,0,0), back);
        Handles.Label(transform.position + new Vector3(0,0,1), right);
        Handles.Label(transform.position + new Vector3(0,0,-1), left);
        Handles.Label(transform.position + new Vector3(0,1,0), top);
        Handles.Label(transform.position + new Vector3(0,-1,0), bottom);
    }
}

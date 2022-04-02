using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class RenderCube : MonoBehaviour
{

    public bool isActive = true;
    public float size = 1;
    
    void OnDrawGizmosSelected()
    {
        // Draw a semitransparent blue cube at the transforms position
        Gizmos.color = new Color(1, 0, 0, 0.3f);
        if (isActive) {
            Gizmos.DrawCube(transform.position,  new Vector3(size, size, size));
        }
    }
}

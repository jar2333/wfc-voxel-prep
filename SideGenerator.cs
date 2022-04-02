using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class SideGenerator  {

    public SideGenerator(float meshEdge) {
        this.meshEdge = meshEdge;
    }
    
    public float meshEdge;
    
    //The mapping between "sides" (rename to "face configurations") and socket 
    //A string array is a leftover from trying an array of face configuration objects to string. Honestly, a string array is pretty dumb, it can just be a string.
    private Dictionary<Side, string> horizontalSocketDict = new Dictionary<Side, string>(Side.CreateSetComparer()); //custom comparer works
    private Dictionary<Side, string> verticalSocketDict   = new Dictionary<Side, string>(Side.CreateSetComparer()); //separate vertical and horizontal lookup tables
    private int topSocket = 1; //two topSocket counters not strictly necessary for functionality
    
    private string GetVerticalSocket(Side side) {
        Side rotatedSide1 = side.RotateSide();
        Side rotatedSide2 = rotatedSide1.RotateSide();
        Side rotatedSide3 = rotatedSide2.RotateSide();
        
        // symmetries: A: 1234 (4-way), B: 12 (2-way), C: 1 (None)
        bool isSymmetricA = side.SetEquals(rotatedSide1); //4-way symmetry
        bool isSymmetricB = side.SetEquals(rotatedSide2) && rotatedSide1.SetEquals(rotatedSide3); //2-way symmetry

        string sock;

        if(verticalSocketDict.ContainsKey(side)) {
            sock = verticalSocketDict[side];
            return sock;
        }

        if (isSymmetricA) {
            sock = topSocket.ToString() + "I";
            verticalSocketDict.Add(side, sock);
        }
        else if (isSymmetricB) {
            sock = topSocket.ToString();
            verticalSocketDict.Add(side,         sock + "-0");
            verticalSocketDict.Add(rotatedSide1, sock + "-1");
            sock += "-0";
        }
        else {
            sock = topSocket.ToString();
            verticalSocketDict.Add(side        , sock + "_0");
            verticalSocketDict.Add(rotatedSide1, sock + "_1");
            verticalSocketDict.Add(rotatedSide2, sock + "_2");
            verticalSocketDict.Add(rotatedSide3, sock + "_3");
            sock += "_0";
        }
        topSocket += 1;
        return sock;
    }

    private string GetHorizontalSocket(Side side) {
        Side mirroredSide = side.MirrorSide();

        bool isSymmetric = side.SetEquals(mirroredSide);

        string sock;

        if (horizontalSocketDict.ContainsKey(side)) {
            sock = horizontalSocketDict[side];
            return sock;
        }

        if (isSymmetric) {
            sock = topSocket.ToString() + "S";
            horizontalSocketDict.Add(side, sock);
        }
        else {
            sock = topSocket.ToString();
            horizontalSocketDict.Add(side, sock);
            horizontalSocketDict.Add(mirroredSide, sock + "F");
        }
        topSocket += 1;
        return sock;

    }

    private Cube GetCube(Side[] hor, Side[] ver) {
        return new Cube(hor[0], hor[1], hor[2], hor[3], ver[0], ver[1]);
    }

    private Side[] GetHorizontalSides(Vector3[] verteces) {
        var frontSide = new Side();
        var rightSide = new Side();
        var backSide  = new Side();
        var leftSide  = new Side();

        foreach (var vertex in verteces) {
            if (Mathf.Approximately(vertex.x, meshEdge)) {
                frontSide.Add(new Vector2(vertex.z, vertex.y));
            }
            if (Mathf.Approximately(vertex.z, meshEdge)) {
                rightSide.Add(new Vector2(-1 * vertex.x, vertex.y));
            }
            if (Mathf.Approximately(vertex.x, -1 * meshEdge)) {
                backSide.Add(new Vector2(-1 * vertex.z, vertex.y));
            }
            if (Mathf.Approximately(vertex.z, -1 * meshEdge)){
                leftSide.Add(new Vector2(vertex.x, vertex.y));
            }
        }

        // This order is important
        //     y--------+
        //    /|       /|
        //   / |      / |
        //  +--|-----+  |
        //  |  o-----|--z
        //  | /      | /
        //  |/       |/
        //  x--------+    x          z         -x        -z
        return new[] {frontSide, rightSide, backSide, leftSide};
    }

    private Side[] GetVerticalSides(Vector3[] verteces) {
        var topSide    = new Side();
        var bottomSide = new Side();

        foreach (var vertex in verteces) {
            if (Mathf.Approximately(vertex.y, meshEdge)) {
                topSide.Add(new Vector2(vertex.z, vertex.x)); 
            }
            if (Mathf.Approximately(vertex.y, -1 * meshEdge)){
                bottomSide.Add(new Vector2(vertex.z, vertex.x));
            }
        }
        //               y         -y
        return new[] {topSide, bottomSide};
    }

}

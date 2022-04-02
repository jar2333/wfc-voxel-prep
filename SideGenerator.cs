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
    private Dictionary<string[], string> horizontalSocketDict = new Dictionary<string[], string>(new MyClassSpecialComparer()); //custom comparer works
    private Dictionary<string[], string> verticalSocketDict = new Dictionary<string[], string>(new MyClassSpecialComparer()); //separate vertical and horizontal lookup tables
    private int topSocket = 1; //two topSocket counters not strictly necessary for functionality
    
    private string GetVerticalSocket(Vector2[] side) {
        Vector2[] rotatedSide1 = RotateSide(side);
        Vector2[] rotatedSide2 = RotateSide(rotatedSide1);
        Vector2[] rotatedSide3 = RotateSide(rotatedSide2);

        string[] strSide = Array.ConvertAll<Vector2, string>(side, v => v.ToString("F7"));
        string[] strRotatedSide1 = Array.ConvertAll<Vector2, string>(rotatedSide1, v => v.ToString("F7"));
        string[] strRotatedSide2 = Array.ConvertAll<Vector2, string>(rotatedSide2, v => v.ToString("F7"));
        string[] strRotatedSide3 = Array.ConvertAll<Vector2, string>(rotatedSide3, v => v.ToString("F7"));

        Array.Sort(strSide);
        Array.Sort(strRotatedSide1);
        Array.Sort(strRotatedSide2);
        Array.Sort(strRotatedSide3);

        // symmetries: 1234, 12, 1

        bool isSymmetricA = Compare(strSide, strRotatedSide1); //4-way symmetry
        bool isSymmetricB = Compare(strSide, strRotatedSide2) && Compare(strRotatedSide1, strRotatedSide3); //2-way symmetry

        string sock;

        if(verticalSocketDict.ContainsKey(strSide)) {
            sock = verticalSocketDict[strSide];
            return sock;
        }

        if (isSymmetricA) {
            sock = topSocket.ToString() + "I";
            verticalSocketDict.Add(strSide, sock);
        }
        else if (isSymmetricB) {
            sock = topSocket.ToString();
            verticalSocketDict.Add(strSide, sock + "-0");
            verticalSocketDict.Add(strRotatedSide1, sock + "-1");
            sock += "-0";
        }
        else {
            sock = topSocket.ToString();
            verticalSocketDict.Add(strSide, sock + "_0");
            verticalSocketDict.Add(strRotatedSide1, sock + "_1");
            verticalSocketDict.Add(strRotatedSide2, sock + "_2");
            verticalSocketDict.Add(strRotatedSide3, sock + "_3");
            sock += "_0";
        }
        topSocket += 1;
        return sock;
    }

    private string GetHorizontalSocket(Vector2[] side) {
        Vector2[] mirroredSide = MirrorSide(side);

        string[] strSide = Array.ConvertAll<Vector2, string>(side, v => v.ToString("F7"));
        string[] strMirroredSide = Array.ConvertAll<Vector2, string>(mirroredSide, v => v.ToString("F7"));
        Array.Sort(strSide);
        Array.Sort(strMirroredSide);

        bool isSymmetric = Compare(strSide, strMirroredSide);

        string sock;

        if (horizontalSocketDict.ContainsKey(strSide)) {
            sock = horizontalSocketDict[strSide];
            return sock;
        }

        if (isSymmetric) {
            sock = topSocket.ToString() + "S";
            horizontalSocketDict.Add(strSide, sock);
        }
        else {
            sock = topSocket.ToString();
            horizontalSocketDict.Add(strSide, sock);
            horizontalSocketDict.Add(strMirroredSide, sock + "F");
        }
        topSocket += 1;
        return sock;

    }

    private Vector2[][] GetHorizontalSides(Vector3[] verteces) {
        List<Vector2> frontSide = new List<Vector2>();
        List<Vector2> rightSide = new List<Vector2>();
        List<Vector2> backSide = new List<Vector2>();
        List<Vector2> leftSide = new List<Vector2>();

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
        //         y--------+
        //        /|       /|
        //       / |      / |
        //      +--|-----+  |
        //      |  o-----|--z
        //      | /      | /
        //      |/       |/
        //      x--------+
        return new Vector2[][] {frontSide.ToArray(), rightSide.ToArray(), backSide.ToArray(), leftSide.ToArray()};
    }

    private Vector2[][] GetVerticalSides(Vector3[] verteces) {
        List<Vector2> topSide = new List<Vector2>();
        List<Vector2> bottomSide = new List<Vector2>();

        foreach (var vertex in verteces) {
            if (Mathf.Approximately(vertex.y, meshEdge)) {
                topSide.Add(new Vector2(vertex.z, vertex.x)); //removed *-1 to test if top and bottom get same socket for same rotation if same side (should)
            }
            if (Mathf.Approximately(vertex.y, -1 * meshEdge)){
                bottomSide.Add(new Vector2(vertex.z, vertex.x)); //is same mapping as above
            }
        }

        return new Vector2[][] {topSide.ToArray(), bottomSide.ToArray()};
    }

    private Vector2[] MirrorSide(Vector2[] side) {
        return Array.ConvertAll<Vector2, Vector2>(side, v => new Vector2(-1 * v.x, v.y));
    }

    private Vector2[] RotateSide(Vector2[] side) {
        return Array.ConvertAll<Vector2, Vector2>(side, v => new Vector2(-1 * v.y, v.x));
    }

    private bool Compare(string[] A, string[] B) { //convert to string so that linq uses the string Equals instead of Vector Equals
        return A.SequenceEqual(B);
    }
    
    public class MyClassSpecialComparer : IEqualityComparer<string[]> {
        
        public bool Equals (string[] x, string[] y) { 
            return x.SequenceEqual(y);
        }

        public int GetHashCode(string[] x) {
            int h = 0;
            foreach (var s in x) {
                h += s.GetHashCode();
            }
            return h;
        }
    }
}

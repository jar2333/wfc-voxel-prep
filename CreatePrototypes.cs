using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class CreatePrototypes : MonoBehaviour
{

    [Serializable]
    public class Prototype {
        //         y--------+
        //        /|       /|
        //       / |      / |
        //      +--|-----+  |
        //      |  o-----|--z
        //      | /      | /
        //      |/       |/
        //      x--------+
        public Prototype(string meshPath, int rotation, string[] hsockets, string[] vsockets) {
            this.meshPath = meshPath;
            this.rotation = rotation;
            this.front = hsockets[0];
            this.right= hsockets[1];
            this.back = hsockets[2];
            this.left = hsockets[3];
            this.top = vsockets[0];
            this.bottom = vsockets[1];
        }
        
        string meshPath;
        int rotation;
        string front;
        string right;
        string back;
        string left;
        string top;
        string bottom;
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

    //NOTE: This works really well, but if there are even small 1-vertex differences they will get different sockets. Thus human review is unavoidable.
    //Even then it's easy, any excess sockets can be "collapsed" (example, when we get 3 instead of 2F, in manual review just make all 3 -> 2F and 3F -> 2)

    //TODO
    //Modify this to run over array of meshes, get the sockets for each mesh, and save the prototypes (1 for each rotations). Serializable meshes?

    //Used as base for previews. Has gizmos attached and a mesh renderer.
    public GameObject previewBasePrefab;

    //List of previews in scene
    public List<GameObject> previewList = new List<GameObject>();

    //The distance between center of cube mesh and sides of cube mesh. I found this in an adhoc way. Wow.
    public float meshEdge;

    //The meshes that will be turned into previews ("test objects")
    public Mesh[] meshArray;

    //The mapping between "sides" (rename to "face configurations") and socket 
    //A string array is a leftover from trying an array of face configuration objects to string. Honestly, a string array is pretty dumb, it can just be a string.
    private Dictionary<string[], string> horizontalSocketDict = new Dictionary<string[], string>(new MyClassSpecialComparer()); //custom comparer works
    private Dictionary<string[], string> verticalSocketDict = new Dictionary<string[], string>(new MyClassSpecialComparer()); //separate vertical and horizontal lookup tables
    private int topSocket = 1; //two topSocket counters not strictly necessary for functionality

    private void Awake()
    {
        
    }
    
    [ContextMenu("Test Sockets by Generating Previews")]
    private void TestSockets() {
        DeletePreviewsAndReset();
        foreach (var mesh in meshArray) {

            Vector3[] verteces = mesh.vertices;

            Vector2[][] horizontalSides = GetHorizontalSides(verteces);
            Vector2[][] verticalSides = GetVerticalSides(verteces);

            string[] horizontalSockets = Array.ConvertAll<Vector2[], string>(horizontalSides, p => GetHorizontalSocket(p));
            string[] verticalSockets = Array.ConvertAll<Vector2[], string>(verticalSides, p => GetVerticalSocket(p));

            Debug.Log(mesh.name);
            Debug.Log(String.Join("\n", horizontalSockets) + "\n" + String.Join("\n", verticalSockets));

            //Create objects to view the mesh, use gizmos to illustrate sockets
            CreatePreviews(mesh, horizontalSockets, verticalSockets);
        }
    }

    [ContextMenu("Delete Test Objects and Reset")] 
    private void DeletePreviewsAndReset() {
        foreach (var g in previewList) {
            DestroyImmediate(g);
        }
        previewList.Clear();
        Reset();
    }

    [ContextMenu("Create Prototypes")]
    private void CreateProtototypes()
    {
        Reset();

        List<Prototype> prototypes = new List<Prototype>();

        foreach (var mesh in meshArray) {
            Vector3[] verteces = mesh.vertices;

            Vector2[][] horizontalSides = GetHorizontalSides(verteces);
            Vector2[][] verticalSides = GetVerticalSides(verteces);

            string[] horizontalSockets = Array.ConvertAll<Vector2[], string>(horizontalSides, p => GetHorizontalSocket(p));
            string[] verticalSockets = Array.ConvertAll<Vector2[], string>(verticalSides, p => GetVerticalSocket(p));

            for (int i = 0; i < 4; i++)  {
                string[] hsock = ShiftHorizontalSocket(horizontalSockets, i);
                string[] vsock = Array.ConvertAll<string, string>(verticalSockets, s => ShiftVerticalSocket(s, i));

                prototypes.Add(new Prototype("", i, hsock, vsock));
            }
            //create all 4 rotation prototypes ^
            //if vertical socket = x_i, then create 4 protos with sockets x_(i%4) x_((i+1)%4) x_((i+2)%4) x_((i+3)%4) respectively
            //and shift the horizontal sockets by 1 
            //FORMAT:
            //Same mesh for all 4
            //   1          2          3          4
            //t_(i%4)  t_((i+1)%4) t_((i+2)%4) t_((i+3)%4)
            //f,r,b,l --> l,f,r,b --> b,l,f,r --> r,b,l,f
            //b_(i%4)  b_((i+1)%4) b_((i+2)%4) b_((i+3)%4)

            //Add them to list, serialize at the end
        }
    }
    
    private void CreatePreviews(Mesh mesh, string[] hs, string[] vs) {
        GameObject newObject = Instantiate(previewBasePrefab);
        newObject.transform.parent = transform;
        newObject.name = mesh.name;
        newObject.transform.position = new Vector3(0, 0, 3 * previewList.Count );
        newObject.transform.localScale = new Vector3(100,100,100);

        MeshFilter filter = newObject.GetComponent<MeshFilter>();
        filter.mesh = mesh;

        RenderLabels l = newObject.GetComponent<RenderLabels>();
        l.front = hs[0];
        l.right = hs[1];
        l.back = hs[2];
        l.left = hs[3];
        l.top = vs[0];
        l.bottom = vs[1];

        previewList.Add(newObject);
    }

    //Shift right i times
    private string[] ShiftHorizontalSocket(string[] sarr, int i) {
        string[] shifted = new string[4];
        for (int j = 0; j < 4; j++) {
            shifted[(j+i) % 4] = sarr[j];
        }
        return shifted;
    }

    private string ShiftVerticalSocket(string s, int i) {
        int len = s.Length;
        if (s[len-1].Equals('I')) {
            return s;
        }
        else if (s[len-2].Equals('-')) {
            int x = (int)Char.GetNumericValue(s[2]);
            return $"{s[0]}-{(x + i) % 2}";
        }
        else {
            int x = (int)Char.GetNumericValue(s[2]);
            return $"{s[0]}_{(x + i) % 4}";
        }
    }

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

    private void Reset() {
        horizontalSocketDict.Clear();
        verticalSocketDict.Clear();
        topSocket = 1;
    }

    private void PrintSide(string s, Vector2[] side) {
        string st = s + "[";
        foreach(var v in side) {
            st += ", " + v.ToString("F7");
        }
        st += "]";
        Debug.Log(st);
    }

    private void PrintSide(Vector2[] side) {
        PrintSide("", side);
    }

}

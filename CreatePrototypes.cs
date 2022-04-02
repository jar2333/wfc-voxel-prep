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
        // x: front
        // z: right
        //-x: back
        //-z: left
        // y: top
        //-y: bottom
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

    private readonly SocketGenerator _socketGenerator = new SocketGenerator();

    [ContextMenu("Create Sockets and then previews")]
    private void TestSockets() {
        DeletePreviewsAndReset();
        
        foreach (var mesh in meshArray) {
            Vector3[] verteces = mesh.vertices;
            Cube c = CubeFactory.GetCube(verteces, meshEdge);
            var sockets = GetSocketsFromCube(c);

            Debug.Log(mesh.name);
            Debug.Log(String.Join("\n", sockets.Item1) + "\n" + String.Join("\n", sockets.Item2));

            //Create objects to view the mesh, use gizmos to illustrate sockets
            CreatePreviews(mesh, sockets.Item1, sockets.Item2);
        }
    }

    [ContextMenu("Delete Previews and Reset Sockets")] 
    private void DeletePreviewsAndReset() {
        foreach (var g in previewList) {
            DestroyImmediate(g);
        }
        previewList.Clear();
        Reset();
    }

    /*
     * 
     */
    [ContextMenu("Create Prototypes")]
    private void CreateProtototypes() {
        Reset();

        List<Prototype> prototypes = new List<Prototype>();

        foreach (var mesh in meshArray) {
            Vector3[] verteces = mesh.vertices;
            Cube c = CubeFactory.GetCube(verteces, meshEdge);
            var sockets = GetSocketsFromCube(c);

            for (int i = 0; i < 4; i++)  {
                string[] hsock = ShiftHorizontalSockets(sockets.Item1, i);
                string[] vsock = ShiftVerticalSockets(sockets.Item2, i);

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
    
    /*
     * Creates previews of the prototypes and displays them in the scene
     * Warning" lots of hardcoded values
     */
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
    
    private void Reset() {
        _socketGenerator.Reset();
    }
    
    private string[] ShiftHorizontalSockets(string[] sarr, int i) {
        string[] shifted = new string[4];
        for (int j = 0; j < 4; j++) {
            shifted[(j+i) % 4] = sarr[j];
        }
        return shifted;
    }
    
    private string[] ShiftVerticalSockets(string[] sarr, int i) {
        return Array.ConvertAll<string, string>(sarr, s => VertShift(s, i));
    }

    private string VertShift(string s, int i) {
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

    private (string[], string[]) GetSocketsFromCube(Cube c)
    {
        string[] horizontalSockets = Array.ConvertAll<Side, string>(c.HorizontalSides, 
            p => _socketGenerator.GetHorizontalSocket(p));
        string[] verticalSockets   = Array.ConvertAll<Side, string>(c.VerticalSides, 
            p => _socketGenerator.GetVerticalSocket(p));
        return (horizontalSockets, verticalSockets);
    }

}

using UnityEngine;

public class CubeFactory  {
    
    public static Cube GetCube(Vector3[] verteces, float meshEdge) {
        Side[] hor = GetHorizontalSides(verteces, meshEdge);
        Side[] ver = GetVerticalSides(verteces, meshEdge);
        return new Cube(hor[0], hor[1], hor[2], hor[3], ver[0], ver[1]);
    }

    private static Side[] GetHorizontalSides(Vector3[] verteces, float meshEdge) {
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

    private static Side[] GetVerticalSides(Vector3[] verteces, float meshEdge) {
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

using System;using System.Collections.Generic;
using System.Linq;
using UnityEngine;

//The sides of a cube, each side is a set of vertices
public class Side : HashSet<Vector2> {
    //Construct Side with custom vertex comparer!
    public Side() : base(new VertexComparer()) {}
    public Side(IEnumerable<Vector2> v) : base(v, new VertexComparer()) {}

    public Side MirrorSide() {
        return new Side(this.Select(v => new Vector2(-1 * v.x, v.y)));
    }
    
    public Side RotateSide() {
        return new Side(this.Select(v => new Vector2(-1 * v.y, v.x)));
    }

    //Two verteces are equal if they are approx equal (float stuff grrrr)
    private class VertexComparer : IEqualityComparer<Vector2> {
        public bool Equals(Vector2 v1, Vector2 v2) {
            bool x = Mathf.Approximately(v1.x, v2.x);
            bool y = Mathf.Approximately(v1.y, v2.y);
            return x && y;
        }
        public int GetHashCode(Vector2 v) { //might not work??
            return v.GetHashCode();
        }
    }
}

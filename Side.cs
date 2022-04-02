using System;using System.Collections.Generic;
using System.Linq;
using System.Text;
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

    //override ToString
    public override string ToString()
    {
        var strings = this.Select(v => v.ToString());
        return String.Join("\n", strings);
    }

    //override Equals
    public override bool Equals(object obj) {
        if (obj == null) {
            return false;
        }
        return this.SetEquals(obj as Side);
    }

    //Two verteces are equal if they are approx equal (float stuff grrrr)
    private class VertexComparer : IEqualityComparer<Vector2> {
        public bool Equals(Vector2 v1, Vector2 v2) {
            bool x = FastApproximately(v1.x, v2.x);
            bool y = FastApproximately(v1.y, v2.y);
            return x && y;
        }
        public int GetHashCode(Vector2 v) { //might not work??
            var aString = v.x.ToString("F7");
            var bString = v.y.ToString("F7");
            return (aString, bString).GetHashCode();
        }
        
        private bool FastApproximately(float a, float b) //converts to string, ugh
        {
            var aString = a.ToString("F7");
            var bString = b.ToString("F7");
            return aString.Equals(bString);

        }
    }
}

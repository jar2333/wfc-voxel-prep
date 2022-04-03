using System;using System.Collections.Generic;
using System.Linq;
using UnityEngine;

//The sides of a cube, each side is a set of vertices
public class Side : HashSet<Vertex> {
    //Construct Side with custom vertex comparer!
    public Side() : base() {}
    public Side(IEnumerable<Vertex> v) : base(v) {}

    public Side MirrorSide() {
        return new Side(this.Select(v => v.Mirror()));
    }
    
    public Side RotateSide() {
        return new Side(this.Select(v => v.Rotate()));
    }

    public void Add(Vector2 v) {
        base.Add(new Vertex(v));
    }
    
    //override ToString
    public override string ToString() {
        var strings = this.Select(v => v.ToString());
        return String.Join("\n", strings);
    }
    
    //override Equals
    public override bool Equals(object other) {
        if (other == null)
        {
            return false;
        } 
        return SetEquals(other as Side);
    }
    
    //override GetHashCode
    public override int GetHashCode()
    {
        int h = 0;
        foreach (var vertex in this)
        {
            h += h + vertex.GetHashCode();
        }
        return h;
    }
}

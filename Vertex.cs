using System;
using UnityEngine;

public class Vertex
{
    private string x;
    private string y;

    private float a;
    private float b;

    public Vertex(Vector2 v) : this(v.x, v.y) { }

    public Vertex(float a, float b)
    {
        string x = a.ToString(FORMAT);
        string y = b.ToString(FORMAT);
        this.x = x;
        this.y = y;
        this.a = a;
        this.b = b;
    }

    public override string ToString()
    {
        return $"({x}, {y})";
    }

    public override bool Equals(object obj) {
        if (obj == null)
        {
            return false;
        }
        Vertex other = obj as Vertex;
        return x.Equals(other.x) && y.Equals(other.y);
    }

    public override int GetHashCode() {
        return x.GetHashCode() + y.GetHashCode();
    }
    
    public Vertex Mirror() {
        return new Vertex(-1 * this.a, this.b);
    }
    
    public Vertex Rotate() {
        return new Vertex(-1 * this.b, this.a);
    }

    private readonly string FORMAT = "F7";
}
